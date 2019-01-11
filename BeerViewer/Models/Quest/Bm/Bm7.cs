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
	/// 수상반격부대 돌입하라!
	/// </summary>
	internal class Bm7 : CountableTracker
	{
		public override QuestType Type => QuestType.Monthly;
		public override int Id => 266;

		public override int Maximum => 1;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 2 || args.MapAreaId != 5) return; // 2-5
				if ("敵主力艦隊" != args.EnemyName) return; // boss
				if (args.Rank != "S") return;

				var fleet = Homeport.Instance.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;

				if (fleet?.Ships[0]?.Info.ShipType.Id != 2) return; // Flagship is not DD
				if (fleet?.Ships.Count(x => x.Info.ShipType.Id == 2) != 4) return; // Not 4 DDs
				if (fleet?.Ships.Count(x => x.Info.ShipType.Id == 3) != 1) return; // Not 1 CL
				if (fleet?.Ships.Count(x => x.Info.ShipType.Id == 5) != 1) return; // Not 1 CA

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
