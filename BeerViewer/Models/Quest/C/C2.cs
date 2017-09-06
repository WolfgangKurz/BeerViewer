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
	/// "연습"으로 훈련도 향상!
	/// </summary>
	internal class C2 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 303;

		public override int Maximum => 3;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.PracticeResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
