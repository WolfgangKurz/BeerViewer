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
	/// 수뢰전대 남서쪽으로!
	/// </summary>
	internal class Bm3 : CountableTracker
	{
		public override QuestType Type => QuestType.Monthly;
		public override int Id => 257;

		public override int Maximum => 1;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 1 || args.MapAreaId != 4) return; // 1-4
				if ("敵機動部隊" != args.EnemyName) return; // boss
				if (args.Rank != "S") return;

				var fleet = Homeport.Instance.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;

				if (fleet?.Ships[0]?.Info.ShipType.Id != 3) return; // Flagship is not CL
				if (fleet?.Ships.Any(x => x.Info.ShipType.Id != 2 && x.Info.ShipType.Id != 3) ?? false) return; // Ship exists not DD, CL
				if (fleet?.Ships.Count(x => x.Info.ShipType.Id == 3) > 3) return; // CL over 3
				if (fleet?.Ships.Count(x => x.Info.ShipType.Id == 2) < 1) return; // DD under 1

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
