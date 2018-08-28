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
	/// 대공기관총 양산
	/// </summary>
	internal class F34 : CountableTracker
	{
		public override QuestType Type => QuestType.Weekly;
		public override int Id => 638;

		public override int Maximum => 6;

		public override void RegisterEvent(TrackManager manager)
		{
			manager.DestroyItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var master = Master.Instance.SlotItems;
				var homeportSlotitems = manager.slotitemTracker.SlotItems;
				var cnt = args.itemList.Count(x => (homeportSlotitems[x]?.Info.Type ?? SlotItemType.None) == SlotItemType.AAGun);

				Current = Current.Add(1).Max(Maximum);
			};
		}
	}
}
