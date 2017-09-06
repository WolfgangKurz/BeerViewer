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
	/// 적 동방함대 격멸
	/// </summary>
	internal class Bw6 : CountableTracker
	{
		public override QuestType Type => QuestType.Weekly;
		public override int Id => 229;

		public override int Maximum => 12;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var BossNameList = new string[]
				{
					"東方派遣艦隊",			// 4-1
					"東方主力艦隊",			// 4-2, 4-3
					"敵東方中枢艦隊",		// 4-4
					"リランカ島港湾守備隊"	// 4-5
				};

				if (args.MapWorldId != 4) return; // 4 해역
				if (!BossNameList.Contains(args.EnemyName)) return; // boss
				if (!"SAB".Contains(args.Rank)) return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
