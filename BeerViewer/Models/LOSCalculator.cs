using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Framework;
using BeerViewer.Models.Enums;
using BeerViewer.Models.Wrapper;
using BeerViewer.Modules;

namespace BeerViewer.Models
{
	/// <summary>
	/// LOS Calculator
	/// </summary>
	public interface ICalcLOS
	{
		string Id { get; }
		string Name { get; }

		bool HasCombinedSettings { get; }
		double Calc(Fleet[] fleets);
	}

	public abstract class LOSCalcLogic : ICalcLOS
	{
		private static readonly Dictionary<string, ICalcLOS> logics = new Dictionary<string, ICalcLOS>();

		public static IEnumerable<ICalcLOS> Logics => logics.Values;

		public static ICalcLOS Get(string key)
		{
			ICalcLOS logic;
			return logics.TryGetValue(key, out logic) ? logic : new LOSType1();
		}

		static LOSCalcLogic()
		{
			// ReSharper disable ObjectCreationAsStatement
			new LOSType1();
			new LOSType2();
			new LOSType3();
			new LOSType4();
			new LOSType5();
			new LOSType6();
			// ReSharper restore ObjectCreationAsStatement
		}

		public abstract string Id { get; }
		public abstract string Name { get; }
		public virtual bool HasCombinedSettings { get; } = false;
		public abstract double Calc(Fleet[] fleets);

		protected LOSCalcLogic()
		{
			// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			var key = this.Id;
			if (key != null && !logics.ContainsKey(key))
				logics.Add(key, this);
		}
	}

	/// <summary>
	/// Cn식
	/// </summary>
	public abstract class LOSTypeCn : LOSCalcLogic
	{
		public abstract double Cn { get; }
		public override bool HasCombinedSettings { get; } = true;

		public override double Calc(Fleet[] fleets)
		{
			if (fleets == null || fleets.Length == 0) return 0;

			var ships = this.GetTargetShips(fleets)
						.Where(x => !x.Situation.HasFlag(ShipSituation.Evacuation))
						.Where(x => !x.Situation.HasFlag(ShipSituation.Tow))
						.ToArray();

			if (!ships.Any()) return 0;

			var itemScore = ships
				.SelectMany(x => x.EquippedItems)
				.Select(x => x.Item)
				.Sum(x => (x.Info.LOS + GetLevelCoefficient(x)) * GetTypeCoefficient(x.Info.Type));

			var shipScore = ships
				.Select(x => x.LOS - x.EquippedItems.Sum(s => s.Item.Info.RawData.api_saku))
				.Sum(x => Math.Sqrt(x));

			var admiralScore = Math.Ceiling(Homeport.Instance.Admiral.Level * 0.4);

			var isCombined = 1 < fleets.Count()
							 && Settings.IsLoSIncludeFirstFleet
							 && Settings.IsLoSIncludeSecondFleet;
			var vacancyScore = ((isCombined ? 12 : 6) - ships.Length) * 2;

			return itemScore * this.Cn + shipScore - admiralScore + vacancyScore;
		}

		private Ship[] GetTargetShips(Fleet[] fleets)
		{
			if (fleets.Count() == 1)
				return fleets.Single().Ships;

			if (Settings.IsLoSIncludeFirstFleet && Settings.IsLoSIncludeSecondFleet)
				return fleets.SelectMany(x => x.Ships).ToArray();

			if (Settings.IsLoSIncludeFirstFleet)
				return fleets.First().Ships;

			if (Settings.IsLoSIncludeSecondFleet)
				return fleets.Last().Ships;

			return new Ship[0];
		}
		private static double GetLevelCoefficient(SlotItem item)
		{
			switch (item.Info.Type)
			{
				case SlotItemType.ReconSeaplane:
					return Math.Sqrt(item.Level) * 1.2;

				case SlotItemType.SmallRader:
				case SlotItemType.LargeRader:
				case SlotItemType.LargeRader_II:
					return Math.Sqrt(item.Level) * 1.25;

				default:
					return 0;
			}
		}
		private static double GetTypeCoefficient(SlotItemType type)
		{
			switch (type)
			{
				case SlotItemType.CarrierBased_Fighter:
				case SlotItemType.CarrierBased_DiveBomber:
				case SlotItemType.SmallRader:
				case SlotItemType.LargeRader:
				case SlotItemType.LargeRader_II:
				case SlotItemType.AntiSubmarinePatrolAircraft:
				case SlotItemType.Searchlight:
				case SlotItemType.CommandFacility:
				case SlotItemType.AviationPersonnel:
				case SlotItemType.SurfaceShipPersonnel:
				case SlotItemType.LargeSonar:
				case SlotItemType.LargeFlyingBoat:
				case SlotItemType.LargeSearchlight:
				case SlotItemType.SeaplaneFighter:
				case SlotItemType.JetPowered_Fighter: // Maybe
				case SlotItemType.JetPowered_FighterBomber:
					return 0.6;

				case SlotItemType.CarrierBased_TorpedoBomber:
				case SlotItemType.JetPowered_Bomber: // Maybe
					return 0.8;

				case SlotItemType.CarrierBased_ReconPlane:
				case SlotItemType.CarrierBased_ReconPlane_II:
					return 1.0;

				case SlotItemType.SeaplaneBomber:
					return 1.1;

				case SlotItemType.ReconSeaplane:
				case SlotItemType.JetPowered_ReconPlane: // Maybe
					return 1.2;

				default:
					return .0;
			}
		}
	}

	/// <summary>
	/// Simple Sum
	/// </summary>
	public class LOSType1 : LOSCalcLogic
	{
		public override sealed string Id => "LOSCalc.Type1";
		public override string Name => i18n.Current["loscalc_type1"];

		public override double Calc(Fleet[] fleets)
		{
			if (fleets == null || fleets.Length == 0) return 0;

			return fleets.SelectMany(x => x.Ships).Sum(x => x.LOS);
		}
	}

	/// <summary>
	/// 2-5 (Old)
	/// </summary>
	public class LOSType2 : LOSCalcLogic
	{
		public override sealed string Id => "LOSCalc.Type2";
		public override string Name => i18n.Current["loscalc_type2"];

		public override double Calc(Fleet[] fleets)
		{
			if (fleets == null || fleets.Length == 0) return 0;
			var ships = fleets.SelectMany(x => x.Ships).ToArray();

			var spotter = ships.SelectMany(
				x => x.EquippedItems
					.Where(s => s.Item.Info.RawData.api_type.Get(1) == 7)
					.Where(s => s.Current > 0)
					.Select(s => s.Item.Info.RawData.api_saku)
				).Sum();

			var radar = ships.SelectMany(
				x => x.EquippedItems
					.Where(s => s.Item.Info.RawData.api_type.Get(1) == 8)
					.Select(s => s.Item.Info.RawData.api_saku)
				).Sum();

			return (spotter * 2) + radar + (int)Math.Sqrt(ships.Sum(x => x.LOS) - spotter - radar);
		}
	}

	/// <summary>
	/// 2-5 (Autumn)
	/// </summary>
	public class LOSType3 : LOSCalcLogic
	{
		public override sealed string Id => "LOSCalc.Type3";
		public override string Name => i18n.Current["loscalc_type3"];

		public override double Calc(Fleet[] fleets)
		{
			if (fleets == null || fleets.Length == 0) return 0;
			var ships = fleets.SelectMany(x => x.Ships).ToArray();

			var itemScore = ships
				.SelectMany(x => x.EquippedItems)
				.Select(x => x.Item.Info)
				.GroupBy(
					x => x.Type,
					x => x.RawData.api_saku,
					(type, scores) => new { type, score = scores.Sum() })
				.Aggregate(.0, (score, item) => score + GetScore(item.type, item.score));

			var shipScore = ships
				.Select(x => x.LOS - x.EquippedItems.Sum(s => s.Item.Info.RawData.api_saku))
				.Select(x => Math.Sqrt(x))
				.Sum() * 1.69;

			var level = (((Homeport.Instance.Admiral.Level + 4) / 5) * 5);
			var admiralScore = level * -0.61;

			return itemScore + shipScore + admiralScore;
		}

		private static double GetScore(SlotItemType type, int score)
		{
			switch (type)
			{
				case SlotItemType.CarrierBased_DiveBomber:
					return score * 1.04;
				case SlotItemType.CarrierBased_TorpedoBomber:
					return score * 1.37;
				case SlotItemType.CarrierBased_ReconPlane:
				case SlotItemType.CarrierBased_ReconPlane_II:
					return score * 1.66;

				case SlotItemType.ReconSeaplane:
					return score * 2.00;
				case SlotItemType.SeaplaneBomber:
					return score * 1.78;

				case SlotItemType.SmallRader:
					return score * 1.00;
				case SlotItemType.LargeRader:
				case SlotItemType.LargeRader_II:
					return score * .99;

				case SlotItemType.Searchlight:
					return score * 0.91;
			}
			return .0;
		}
	}

	/// <summary>
	/// Formula 33 (Cn=1)
	/// </summary>
	public class LOSType4 : LOSTypeCn
	{
		public override sealed string Id => "LOSCalc.Type4";
		public override string Name => i18n.Current["loscalc_type4"];
		public override double Cn => 1;
	}

	/// <summary>
	/// Formula 33 (Cn=3)
	/// </summary>
	public class LOSType5 : LOSTypeCn
	{
		public override sealed string Id => "LOSCalc.Type5";
		public override string Name => i18n.Current["loscalc_type5"];
		public override double Cn => 3;
	}

	/// <summary>
	/// Formula 33 (Cn=4)
	/// </summary>
	public class LOSType6 : LOSTypeCn
	{
		public override sealed string Id => "LOSCalc.Type6";
		public override string Name => i18n.Current["loscalc_type6"];
		public override double Cn => 4;
	}
}
