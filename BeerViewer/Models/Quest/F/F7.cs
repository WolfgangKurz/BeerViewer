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
	/// 장비 개발 집중강화
	/// </summary>
	internal class F7 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 607;

		public override int Maximum => 3;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.CreateItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
