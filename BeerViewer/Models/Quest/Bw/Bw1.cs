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
	/// 아호작전
	/// </summary>
	internal class Bw1 : MultipleTracker
	{
		public override QuestType Type => QuestType.Weekly;
		public override int Id => 214;

		public override int[] Maximum => new int[] { 36, 6, 24, 12 };

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				// 출격
				if (args.IsFirstCombat)
					this.Current[0] = this.Current[0].Add(1).Max(36);

				// S 승리
				if (args.Rank == "S")
					this.Current[1] = this.Current[1].Add(1).Max(6);

				// 보스전
				if (args.IsBoss)
				{
					this.Current[2] = this.Current[2].Add(1).Max(24);

					// 보스전 승리
					if ("SAB".Contains(args.Rank))
						this.Current[3] = this.Current[3].Add(1).Max(12);
				}
			};
		}
	}
}
