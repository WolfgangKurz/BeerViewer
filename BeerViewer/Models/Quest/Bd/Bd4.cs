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
	/// 적 공모 3척 격침
	/// </summary>
	internal class Bd4 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 211;

		public override int Maximum => 3;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				this.Current = this.Current.Add(
						args.EnemyShips
							.Where(x => x.Type == 7 || x.Type == 11 || x.Type == 18)
							.Where(x => x.MaxHp != int.MaxValue && x.NowHp <= 0)
							.Count()
					).Max(Maximum);
			};
		}
	}
}
