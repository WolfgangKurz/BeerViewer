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
	internal class F41 : MultipleTracker
	{
		public override QuestType Type => QuestType.Monthly;
		public override int Id => 645;

		public override int[] Maximum => new int[] { 1, 1, 1, 2, 1 };

		public override void RegisterEvent(TrackManager manager)
		{
			manager.EquipEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var homeport = Homeport.Instance;
				var slotitems = homeport.Itemyard.SlotItems.Select(x => x.Value).ToArray();
				slotitems = slotitems.Where(x => homeport.Organization.Ships.Any(y => !y.Value.Slots.Select(z => z.Item.Id).Contains(x.Id)))
					.Where(x => x.RawData.api_locked == 0).ToArray(); // Not equiped, Not locked

				this.Current[1] = homeport.Materials.Fuel >= 750 ? 1 : 0; // Fuel
				this.Current[2] = homeport.Materials.Ammo >= 750 ? 1 : 0; // Ammo
				this.Current[3] = slotitems.Count(x => x.Info.Id == 75).Max(Maximum[3]); // Canister
				this.Current[4] = slotitems.Count(x => x.Info.Id == 36).Max(Maximum[4]); // Type 91 AP Shell
			};
			manager.DestroyItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var homeport = Homeport.Instance;
				var slotitems = homeport.Itemyard.SlotItems.Select(x => x.Value).ToArray();
				slotitems = slotitems.Where(x => homeport.Organization.Ships.Any(y => !y.Value.Slots.Select(z => z.Item.Id).Contains(x.Id)))
					.Where(x => x.RawData.api_locked == 0).ToArray(); // Not equiped, Not locked

				var homeportSlotitems = homeport.Itemyard.SlotItems;
				this.Current[0] = this.Current[0].Add(args.itemList.Count(x => (homeportSlotitems[x]?.Info.Id ?? 0) == 35)).Max(Maximum[0]); // Type 3 AA Shell

				this.Current[1] = homeport.Materials.Fuel >= 750 ? 1 : 0; // Fuel
				this.Current[2] = homeport.Materials.Ammo >= 750 ? 1 : 0; // Ammo
				this.Current[3] = slotitems.Count(x => x.Info.Id == 75).Max(Maximum[3]); // Canister
				this.Current[4] = slotitems.Count(x => x.Info.Id == 36).Max(Maximum[4]); // Type 91 AP Shell
			};
		}
	}
}
