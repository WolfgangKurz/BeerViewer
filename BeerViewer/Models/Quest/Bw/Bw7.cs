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
	/// 적 북방함대 주력 격멸
	/// </summary>
	internal class Bw7 : CountableTracker
	{
		public override QuestType Type => QuestType.Weekly;
		public override int Id => 241;

		public override int Maximum => 5;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var BossNameList = new string[]
				{
					"深海棲艦泊地艦隊",	 // 3-3
					"深海棲艦北方艦隊中枢", // 3-4
					"北方増援部隊主力"	  // 3-5
				};

				if (args.MapWorldId != 3 || (args.MapAreaId != 3 && args.MapAreaId != 4 && args.MapAreaId != 5)) return; // 3-3 3-4 3-5
				if (!BossNameList.Contains(args.EnemyName)) return; // boss
				if (!"SAB".Contains(args.Rank)) return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
