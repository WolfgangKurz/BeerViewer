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
	/// 적 잠수함 제압
	/// </summary>
	internal class Bd8 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 230;

		public override int Maximum => 6;

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

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
