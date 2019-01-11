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
	/// 적 동방 중추함대 격멸
	/// </summary>
	internal class Bw8 : CountableTracker
	{
		public override QuestType Type => QuestType.Weekly;
		public override int Id => 242;

		public override int Maximum => 1;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 4 || args.MapAreaId != 4) return; // 4-4
				if ("敵東方中枢艦隊" != args.EnemyName) return; // boss
				if (!"SAB".Contains(args.Rank)) return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
