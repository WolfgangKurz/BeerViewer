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
	/// 원정을 3회 성공시켜라!
	/// </summary>
	internal class D2 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 402;

		public override int Maximum => 3;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.MissionResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				if (!args.IsSuccess) return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
