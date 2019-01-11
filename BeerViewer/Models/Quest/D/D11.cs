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
	/// 남방쥐수송을 계속 실시하라!
	/// </summary>
	internal class D11 : CountableTracker
	{
		public override QuestType Type => QuestType.Weekly;
		public override int Id => 411;

		public override int Maximum => 6;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.MissionResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				if (!args.IsSuccess) return;
				if (!args.Name.Contains("東京急行")) return;

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
