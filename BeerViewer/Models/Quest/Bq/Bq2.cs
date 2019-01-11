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
	/// 전과확장임무! 「Z작전」 전단작전
	/// </summary>
	internal class Bq2 : MultipleTracker
	{
		public override QuestType Type => QuestType.Quarterly;
		public override int Id => 854;

		public override int[] Maximum => new int[] { 1, 1, 1, 1 };

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				if (args.MapWorldId == 2 && args.MapAreaId == 4) // 2-4
					if ("敵侵攻中核艦隊" == args.EnemyName) // Boss
						if ("SA".Contains(args.Rank))// A or better
							this.Current[0] = this.Current[0].Add(1).Max(Maximum[0]);

				if (args.MapWorldId == 6 && args.MapAreaId == 1) // 6-1
					if ("敵回航中空母" == args.EnemyName) // Boss
						if ("SA".Contains(args.Rank))// A or better
							this.Current[1] = this.Current[1].Add(1).Max(Maximum[1]);

				if (args.MapWorldId == 6 && args.MapAreaId == 3) // 6-3
					if ("留守泊地旗艦艦隊" == args.EnemyName) // Boss
						if ("SA".Contains(args.Rank))// A or better
							this.Current[2] = this.Current[2].Add(1).Max(Maximum[2]);

				if (args.MapWorldId == 6 && args.MapAreaId == 4) // 6-4
					if ("離島守備隊" == args.EnemyName) // Boss
						if ("S".Contains(args.Rank))// S
							this.Current[3] = this.Current[3].Add(1).Max(Maximum[3]);
			};
		}
	}
}
