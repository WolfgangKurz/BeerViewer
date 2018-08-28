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
	/// 로호작전
	/// </summary>
	internal class Bw4 : CountableTracker
	{
		public override QuestType Type => QuestType.Weekly;
		public override int Id => 221;

		public override int Maximum => 50;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				Current = Current.Add(
						args.EnemyShips
							.Where(x => x.Type == 15)
							.Where(x => x.MaxHp != int.MaxValue && x.NowHp <= 0)
							.Count()
					).Max(Maximum);
			};
		}
	}
}
