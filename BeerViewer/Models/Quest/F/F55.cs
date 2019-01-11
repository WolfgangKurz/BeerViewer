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
	/// 해상보급 물자의 조달
	/// </summary>
	internal class F55 : MultipleTracker
	{
		public override QuestType Type => QuestType.Daily;
		public override int Id => 663;

		public override int[] Maximum => new int[] { 10, 1 };

		public override void RegisterEvent(TrackManager manager)
		{
			manager.DestroyItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var homeport = Homeport.Instance;
				var slotitems = homeport.Itemyard.SlotItems.Select(x => x.Value).ToArray();
				slotitems = slotitems.Where(x => homeport.Organization.Ships.Any(y => !y.Value.Slots.Select(z => z.Item.Id).Contains(x.Id)))
					.Where(x => x.RawData.api_locked == 0).ToArray(); // 장비중이지 않고 잠기지 않은 장비들

				var homeportSlotitems = homeport.Itemyard.SlotItems;
				this.Current[0] = this.Current[0]
					.Add(
						args.itemList
							.Count(x => (homeportSlotitems[x]?.Info.Type ?? SlotItemType.None) == SlotItemType.LargeCaliberMainGun
								|| (homeportSlotitems[x]?.Info.Type ?? SlotItemType.None) == SlotItemType.LargeCaliberMainGun_II)
					)
					.Max(Maximum[0]); // Large Caliber Main Gun

				this.Current[1] = homeport.Materials.Steel >= 18000 ? 1 : 0; // Steel
			};
		}
	}
}
