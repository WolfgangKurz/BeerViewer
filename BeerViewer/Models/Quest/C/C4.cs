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
	/// 대규모연습
	/// </summary>
	internal class C4 : CountableTracker
	{
		public override QuestType Type => QuestType.Weekly;
		public override int Id => 302;

		public override int Maximum => 20;

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
