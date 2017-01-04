using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	[Flags]
	public enum AirSuperiorityCalculationOptions
	{
		Default = Maximum,

		Minimum = InternalProficiencyMinValue | Fighter,
		Maximum = InternalProficiencyMaxValue | Fighter | Attacker | SeaplaneBomber,

		/// <summary>함상전투기, 수상전투기, 분식전투기, 분식전투폭격기</summary>
		Fighter = 0x0001,

		/// <summary>함상공격기, 함상폭격기, 분식공격기, 분식폭격기</summary>
		Attacker = 0x0002,

		/// <summary>수상폭격기</summary>
		SeaplaneBomber = 0x0004,

		/// <summary>숙련도 최소치 계산</summary>
		InternalProficiencyMinValue = 0x0100,

		/// <summary>숙련도 최대치 계산</summary>
		InternalProficiencyMaxValue = 0x0200,
	}

	public static class AirSuperiorityPotential
	{
		/// <summary>
		/// 칸무스 제공 능력 계산
		/// </summary>
		public static int GetAirSuperiorityPotential(this Ship ship, AirSuperiorityCalculationOptions options = AirSuperiorityCalculationOptions.Default)
		{
			return ship.EquippedItems
				.Select(x => GetAirSuperiorityPotential(x.Item, x.Current, options))
				.Sum();
		}

		/// <summary>
		/// 장비 탑재량으로 슬롯단위 제공 계산
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
				case SlotItemType.艦上戦闘機:
				case SlotItemType.水上戦闘機:
				case SlotItemType.噴式戦闘機:
				case SlotItemType.噴式戦闘爆撃機:
					return new FighterCalculator();

				case SlotItemType.艦上攻撃機:
				case SlotItemType.艦上爆撃機:
				case SlotItemType.噴式攻撃機:
					// case SlotItemType.噴式偵察機: ??
					return new AttackerCalculator();

				case SlotItemType.水上爆撃機:
					return new SeaplaneBomberCalculator();

				default:
					return EmptyCalculator.Instance;
			}
		}

		private abstract class AirSuperiorityCalculator
		{
			public abstract AirSuperiorityCalculationOptions Options { get; }

			public int GetAirSuperiority(SlotItem slotItem, int onslot, AirSuperiorityCalculationOptions options)
			{
				// 장비 대공치와 슬롯 탑재량 수치
				var airSuperiority = this.GetAirSuperiority(slotItem, onslot);

				// 장비 숙련도 제공 보너스
				airSuperiority += this.GetProficiencyBonus(slotItem, options);

				return (int)airSuperiority;
			}

			protected virtual double GetAirSuperiority(SlotItem slotItem, int onslot)
			{
				return slotItem.Info.AA * Math.Sqrt(onslot);
			}

			protected abstract double GetProficiencyBonus(SlotItem slotItem, AirSuperiorityCalculationOptions options);
		}

		#region AirSuperiorityCalculator 派生型

		private class FighterCalculator : AirSuperiorityCalculator
		{
			public override AirSuperiorityCalculationOptions Options => AirSuperiorityCalculationOptions.Fighter;

			protected override double GetAirSuperiority(SlotItem slotItem, int onslot)
			{
				// 장비 개수 대공 보너스 (★ x 0.2)
				return (slotItem.Info.AA + slotItem.Level * 0.2) * Math.Sqrt(onslot);
			}

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

			public int GetInternalValue(AirSuperiorityCalculationOptions options)
			{
				if (options.HasFlag(AirSuperiorityCalculationOptions.InternalProficiencyMinValue)) return this.internalMinValue;
				if (options.HasFlag(AirSuperiorityCalculationOptions.InternalProficiencyMaxValue)) return this.internalMaxValue;
				return (this.internalMaxValue + this.internalMinValue) / 2;
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

		private static Proficiency GetProficiency(this SlotItem slotItem) => proficiencies[Math.Max(Math.Min(slotItem.Proficiency, 7), 0)];
	}
}
