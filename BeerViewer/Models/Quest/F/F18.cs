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
	/// 장비의 개수강화
	/// </summary>
	internal class F18 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 619;

		public override int Maximum => 1;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.RemodelEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
