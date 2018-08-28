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
	/// 해상호위강화 월간
	/// </summary>
	internal class Bm5 : CountableTracker
	{
		public override QuestType Type => QuestType.Monthly;
		public override int Id => 265;

		public override int Maximum => 10;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 1 || args.MapAreaId != 5) return; // 1-1
				if ("敵通商破壊主力艦隊" != args.EnemyName) return; // boss
				if (!"SA".Contains(args.Rank)) return;

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
