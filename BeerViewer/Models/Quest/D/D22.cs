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
	/// 수송선단 호위를 강화하라!
	/// </summary>
	internal class D22 : CountableTracker
	{
		public override QuestType Type => QuestType.Monthly;
		public override int Id => 424;

		public override int Maximum => 4;

		protected override int CheckCut80 => 3; // 80% when 3/4

		public override void RegisterEvent(TrackManager manager)
		{
			manager.MissionResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				if (!args.IsSuccess) return;
				if (args.Name != "海上護衛任務") return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
