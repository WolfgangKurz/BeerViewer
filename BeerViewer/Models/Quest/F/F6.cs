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
	/// 신 조함 건조 지령
	/// </summary>
	internal class F6 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 606;

		public override int Maximum => 1;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.CreateShipEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
