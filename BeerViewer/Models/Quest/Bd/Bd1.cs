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
	/// 적 함대 격파!
	/// </summary>
	internal class Bd1 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 201;

		public override int Maximum => 1;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				if (!"SAB".Contains(args.Rank)) return; // 승리 랭크

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
