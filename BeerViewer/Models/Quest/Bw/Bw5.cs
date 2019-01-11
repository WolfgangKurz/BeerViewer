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
	/// 해상호위전
	/// </summary>
	internal class Bw5 : CountableTracker
	{
		public override QuestType Type => QuestType.Weekly;
		public override int Id => 228;

		public override int Maximum => 15;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				this.Current = this.Current.Add(
						args.EnemyShips
							.Where(x => x.Type == 13 || x.Type == 14)
							.Where(x => x.MaxHp != int.MaxValue && x.NowHp <= 0)
							.Count()
					).Max(Maximum);
			};
		}
	}
}
