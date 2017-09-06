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
	/// 남서제도해역 제해권 획득
	/// </summary>
	internal class Bd7 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 226;

		public override int Maximum => 5;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var BossNameList = new string[]
				{
					"敵主力艦隊",	 // 2-1, 2-5
					"敵通商破壊艦隊", // 2-2
					"敵主力打撃群",   // 2-3
					"敵侵攻中核艦隊", // 2-4
				};

				if (args.MapWorldId != 2) return; // 2 해역
				if (!BossNameList.Contains(args.EnemyName)) return; // 보스명
				if (!"SAB".Contains(args.Rank)) return; // 승리 랭크

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
