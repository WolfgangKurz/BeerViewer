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
	/// 해상수송로의 안전확보에 힘쓰자!
	/// </summary>
	internal class Bw10 : CountableTracker
	{
		public override QuestType Type => QuestType.Weekly;
		public override int Id => 261;

		public override int Maximum => 3;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 1 || args.MapAreaId != 5) return; // 1-5
				if ("敵通商破壊主力艦隊" != args.EnemyName) return; // boss
				if (!"SA".Contains(args.Rank)) return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
