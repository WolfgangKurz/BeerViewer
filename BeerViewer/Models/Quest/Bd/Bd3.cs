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
	/// 적 함대 10회 요격
	/// </summary>
	internal class Bd3 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 210;

		public override int Maximum => 10;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.BattleResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
