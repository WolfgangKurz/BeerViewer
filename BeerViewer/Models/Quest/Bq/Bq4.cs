using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeerViewer.Models;
using BeerViewer.Models.Raw;
using BeerViewer.Models.Enums;

namespace BeerViewer.Models.Quest
{
	/// <summary>
	/// 전선의 항공정찰을 실시하라!
	/// </summary>
	internal class Bq4 : CountableTracker
	{
		public override QuestType Type => QuestType.Quarterly;
		public override int Id => 862;

		public override int Maximum => 2;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 6 || args.MapAreaId != 3) return; // 6-3
				if ("留守泊地旗艦艦隊" != args.EnemyName) return; // boss
				if (!"SA".Contains(args.Rank)) return;

				var fleet = Homeport.Instance.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;
				if (fleet?.Ships.Count(x => x.Info.ShipType.Id == 16) < 1) return; // AV under 1
				if (fleet?.Ships.Count(x => x.Info.ShipType.Id == 3) < 2) return; // CL under 2

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
