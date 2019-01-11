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
	/// 기종 전환 (영식 함전 52형(숙련))
	/// </summary>
	internal class F25 : CountableTracker
	{
		public override QuestType Type => QuestType.Monthly;
		public override int Id => 628;

		public override int Maximum => 2;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.DestroyItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var fleet = Homeport.Instance.Organization.Fleets[1];
				var slotitems = fleet?.Ships[0]?.Slots;

				if (!slotitems.Any(x => x.Item.Info.Id == 96 && x.Item.Proficiency == 7)) return; // Max proficiency Type 0 Fighter Model 21 (Skilled)

				var homeportSlotitems = manager.slotitemTracker.SlotItems;
				Current = Current.Add(args.itemList.Count(x => (homeportSlotitems[x]?.Info.Id ?? 0) == 21)) // Type 0 Fighter Model 52
							.Max(Maximum);
			};
		}
	}
}
