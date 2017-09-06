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
	/// 함선의 근대화개수를 실시하자!
	/// </summary>
	internal class G2 : CountableTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 702;

		public override int Maximum => 2;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.PowerUpEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				if (!args.IsSuccess) return;

				this.Current = this.Current.Add(1).Max(Maximum);
			};
		}
	}
}
