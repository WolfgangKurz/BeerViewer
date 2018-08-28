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
	/// 수상타격부대 남방으로!
	/// </summary>
	internal class Bm4 : CountableTracker
	{
		public override QuestType Type => QuestType.Monthly;
		public override int Id => 259;

		public override int Maximum => 1;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var shipList = new int[]
				{
					131, // 大和
					136, // 大和改
					143, // 武蔵
					148, // 武蔵改
					80,  // 長門
					275, // 長門改
					81,  // 陸奥
					276, // 陸奥改
					26,  // 扶桑
					286, // 扶桑改
					411, // 扶桑改二
					27,  // 山城
					287, // 山城改
					412, // 山城改二
					77,  // 伊勢
					82,  // 伊勢改
					87,  // 日向
					88,  // 日向改
				};

				if (args.MapWorldId != 5 || args.MapAreaId != 1) return; // 5-1
				if ("敵前線司令艦隊" != args.EnemyName) return; // boss
				if (args.Rank != "S") return;

				var fleet = Homeport.Instance.Organization.Fleets.FirstOrDefault(x => x.Value.IsInSortie).Value;

				if (!fleet?.Ships.Any(x => x.Info.ShipType.Id == 3) ?? false) return; // No CL
				if (fleet?.Ships.Count(x => shipList.Contains(x.Info.Id)) < 3) return;
				// Sum of Yamato class, Nagato class, Fuso class, Ise class ships under 3

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
