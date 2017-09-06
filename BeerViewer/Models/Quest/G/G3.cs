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
	/// 근대화개수를 진행하여 군비를 갖춰라!
	/// </summary>
	internal class G3 : CountableTracker
	{
		public override QuestType Type => QuestType.Weekly;
		public override int Id => 703;

		public override int Maximum => 15;

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
