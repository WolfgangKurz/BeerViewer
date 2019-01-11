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
	/// 남방해역 산호제도 앞바다의 제공권을 쥐어라!
	/// </summary>
	internal class Bw9 : CountableTracker
	{
		public override QuestType Type => QuestType.Weekly;
		public override int Id => 243;

		public override int Maximum => 2;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId != 5 || args.MapAreaId != 2) return; // 5-2
				if ("敵機動部隊本隊" != args.EnemyName) return; // boss
				if (args.Rank != "S") return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
