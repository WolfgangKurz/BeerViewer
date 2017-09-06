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
	/// 함대 대정비
	/// </summary>
	internal class E3 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 503;

		public override int Maximum => 5;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.RepairStartEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
