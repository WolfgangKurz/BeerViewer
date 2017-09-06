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
	/// 칸무스 건조 함대강화
	/// </summary>
	internal class F8 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 608;

		public override int Maximum => 3;

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
