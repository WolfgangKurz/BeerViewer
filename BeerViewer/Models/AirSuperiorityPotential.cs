using System;
using System.Collections.Generic;
using System.Linq;

using BeerViewer.Framework;
using BeerViewer.Models.Enums;
using BeerViewer.Models.Wrapper;

namespace BeerViewer.Models
{
	[Flags]
	public enum AirSuperiorityCalculationOptions
	{
		Default = Maximum,

		Minimum = InternalProficiencyMinValue | Fighter,
		Maximum = InternalProficiencyMaxValue | Fighter | Attacker | SeaplaneBomber,

		/// <summary>Fighter, Seaplane Fighter</summary>
		Fighter = 0x0001,

		/// <summary>Dive Bomber, Torpedo Bomber</summary>
		Attacker = 0x0002,

		/// <summary>Seaplane Bomber</summary>
		SeaplaneBomber = 0x0004,

		/// <summary>Minimum proficiency value</summary>
		InternalProficiencyMinValue = 0x0100,

		/// <summary>Maximum proficiency value</summary>
		InternalProficiencyMaxValue = 0x0200,
	}

	public static class AirSuperiorityPotential
	{
		/// <summary>
		/// 艦娘の制空能力を計算します。
		/// </summary>
		public static int GetAirSuperiorityPotential(this Ship ship, AirSuperiorityCalculationOptions options = AirSuperiorityCalculationOptions.Default)
		{
			return ship.EquippedItems
				.Select(x => GetAirSuperiorityPotential(x.Item, x.Current, options))
				.Sum();
		}

		/// <summary>
		/// 装備と搭載数を指定して、スロット単位の制空能力を計算します。
		/// </summary>
		public static int GetAirSuperiorityPotential(this SlotItem slotItem, int onslot, AirSuperiorityCalculationOptions options = AirSuperiorityCalculationOptions.Default)
		{
			var calculator = slotItem.GetCalculator();
			return options.HasFlag(calculator.Options) && onslot >= 1
				? calculator.GetAirSuperiority(slotItem, onslot, options)
				: 0;
		}

		private static AirSuperiorityCalculator GetCalculator(this SlotItem slotItem)
		{
			switch (slotItem.Info.Type)
			{
				case SlotItemType.CarrierBased_Fighter:
				case SlotItemType.SeaplaneFighter:
					return new FighterCalculator();

				case SlotItemType.CarrierBased_TorpedoBomber:
				case SlotItemType.CarrierBased_DiveBomber:
					return new AttackerCalculator();

				case SlotItemType.SeaplaneBomber:
					return new SeaplaneBomberCalculator();

				default:
					return EmptyCalculator.Instance;
			}
		}

		private abstract class AirSuperiorityCalculator
		{
			public abstract AirSuperiorityCalculationOptions Options { get; }

			public int GetAirSuperiority(SlotItem slotItem, int onslot, AirSuperiorityCalculationOptions options)
				=> (int)(
					this.GetAirSuperiority(slotItem, onslot) // Value from AA stat & Slot count
					+ this.GetProficiencyBonus(slotItem, options) // Bonus from proficiency
				);

			protected virtual double GetAirSuperiority(SlotItem slotItem, int onslot)
				=> slotItem.Info.AA * Math.Sqrt(onslot);

			protected abstract double GetProficiencyBonus(SlotItem slotItem, AirSuperiorityCalculationOptions options);
		}

		#region AirSuperiorityCalculator Derived types
		private class FighterCalculator : AirSuperiorityCalculator
		{
			public override AirSuperiorityCalculationOptions Options => AirSuperiorityCalculationOptions.Fighter;

			protected override double GetAirSuperiority(SlotItem slotItem, int onslot)
				=> (slotItem.Info.AA + slotItem.Level * 0.2) // Improvement bonus (★ x 0.2)
					* Math.Sqrt(onslot);

			protected override double GetProficiencyBonus(SlotItem slotItem, AirSuperiorityCalculationOptions options)
			{
				var proficiency = slotItem.GetProficiency();
				return Math.Sqrt(proficiency.GetInternalValue(options) / 10.0) + proficiency.FighterBonus;
			}
		}
		private class AttackerCalculator : AirSuperiorityCalculator
		{
			public override AirSuperiorityCalculationOptions Options => AirSuperiorityCalculationOptions.Attacker;

			protected override double GetProficiencyBonus(SlotItem slotItem, AirSuperiorityCalculationOptions options)
			{
				var proficiency = slotItem.GetProficiency();
				return Math.Sqrt(proficiency.GetInternalValue(options) / 10.0);
			}
		}
		private class SeaplaneBomberCalculator : AirSuperiorityCalculator
		{
			public override AirSuperiorityCalculationOptions Options => AirSuperiorityCalculationOptions.SeaplaneBomber;

			protected override double GetProficiencyBonus(SlotItem slotItem, AirSuperiorityCalculationOptions options)
			{
				var proficiency = slotItem.GetProficiency();
				return Math.Sqrt(proficiency.GetInternalValue(options) / 10.0) + proficiency.SeaplaneBomberBonus;
			}
		}
		private class EmptyCalculator : AirSuperiorityCalculator
		{
			public static EmptyCalculator Instance { get; } = new EmptyCalculator();

			public override AirSuperiorityCalculationOptions Options => ~AirSuperiorityCalculationOptions.Default;
			protected override double GetAirSuperiority(SlotItem slotItem, int onslot) => .0;
			protected override double GetProficiencyBonus(SlotItem slotItem, AirSuperiorityCalculationOptions options) => .0;

			private EmptyCalculator() { }
		}
		#endregion

		private class Proficiency
		{
			private readonly int internalMinValue;
			private readonly int internalMaxValue;

			public int FighterBonus { get; }
			public int SeaplaneBomberBonus { get; }

			public Proficiency(int internalMin, int internalMax, int fighterBonus, int seaplaneBomberBonus)
			{
				this.internalMinValue = internalMin;
				this.internalMaxValue = internalMax;
				this.FighterBonus = fighterBonus;
				this.SeaplaneBomberBonus = seaplaneBomberBonus;
			}

			/// <summary>
			/// Get internal proficiency value
			/// </summary>
			public int GetInternalValue(AirSuperiorityCalculationOptions options)
			{
				if (options.HasFlag(AirSuperiorityCalculationOptions.InternalProficiencyMinValue)) return this.internalMinValue;
				if (options.HasFlag(AirSuperiorityCalculationOptions.InternalProficiencyMaxValue)) return this.internalMaxValue;
				return (this.internalMaxValue + this.internalMinValue) / 2; // Moderately
			}
		}

		private static readonly Dictionary<int, Proficiency> proficiencies = new Dictionary<int, Proficiency>()
		{
			{ 0, new Proficiency(0, 9, 0, 0) },
			{ 1, new Proficiency(10, 24, 0, 0) },
			{ 2, new Proficiency(25, 39, 2, 1) },
			{ 3, new Proficiency(40, 54, 5, 1) },
			{ 4, new Proficiency(55, 69, 9, 1) },
			{ 5, new Proficiency(70, 84, 14, 3) },
			{ 6, new Proficiency(85, 99, 14, 3) },
			{ 7, new Proficiency(100, 120, 22, 6) },
		};

		private static Proficiency GetProficiency(this SlotItem slotItem)
			=> proficiencies[slotItem.Proficiency.InRange(0, 7)];
	}
}
