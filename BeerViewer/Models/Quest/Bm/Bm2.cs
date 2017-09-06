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
	/// 잠수함대 출격하라!
	/// </summary>
	internal class Bm2 : CountableTracker
	{
		public override QuestType Type => QuestType.Monthly;
		public override int Id => 256;

		public override int Maximum => 3;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 6 || args.MapAreaId != 1) return; // 6-1
				if ("敵回航中空母" != args.EnemyName) return; // boss
				if (args.Rank != "S") return;

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
