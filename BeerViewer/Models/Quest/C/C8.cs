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
	/// 정예 함대 연습
	/// </summary>
	internal class C8 : CountableTracker
	{
		public override QuestType Type => QuestType.Monthly;
		public override int Id => 311;

		public override int Maximum => 7;

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
