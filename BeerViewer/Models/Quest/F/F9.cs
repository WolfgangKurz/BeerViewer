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
	/// 군축조약 대응
	/// </summary>
	internal class F9 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 609;

		public override int Maximum => 2;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.DestroyShipEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
