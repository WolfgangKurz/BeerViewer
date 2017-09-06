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
	/// 오키노시마 해역 영격전
	/// </summary>
	internal class Bq1 : CountableTracker
	{
		public override QuestType Type => QuestType.Quarterly;
		public override int Id => 822;

		public override int Maximum => 2;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 2 || args.MapAreaId != 4) return; // 2-4
				if ("敵侵攻中核艦隊" != args.EnemyName) return; // boss
				if (args.Rank != "S") return;

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
