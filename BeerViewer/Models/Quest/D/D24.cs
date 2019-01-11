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
	/// 해상통상항로의 경계를 엄중히 하라!
	/// </summary>
	internal class D24 : MultipleTracker
	{
		public override QuestType Type => QuestType.Quarterly;
		public override int Id => 402;

		public override int[] Maximum => new int[] { 1, 1, 1, 1 };

		public override void RegisterEvent(TrackManager manager)
		{
			manager.MissionResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				if (!args.IsSuccess) return;

				switch (args.Name)
				{
					case "警備任務":
						this.Current[0] = this.Current[0].Add(1).Max(Maximum[0]);
						break;

					case "対潜警戒任務":
						this.Current[1] = this.Current[1].Add(1).Max(Maximum[1]);
						break;

					case "海上護衛任務":
						this.Current[2] = this.Current[2].Add(1).Max(Maximum[2]);
						break;

					case "強行偵察任務":
						this.Current[3] = this.Current[3].Add(1).Max(Maximum[3]);
						break;
				}
			};
		}
	}
}
