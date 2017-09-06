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
	/// "연습"으로 다른 제독 압도!
	/// </summary>
	internal class C3 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 304;

		public override int Maximum => 5;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.PracticeResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				if (!args.IsSuccess) return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
