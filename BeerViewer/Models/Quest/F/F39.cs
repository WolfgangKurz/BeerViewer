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
	/// 주력 육공의 조달
	/// </summary>
	internal class F39 : MultipleTracker
	{
		public override QuestType Type => QuestType.Quarterly;
		public override int Id => 643;

		public override int[] Maximum => new int[] { 2, 1, 2 };

		public override void RegisterEvent(TrackManager manager)
		{
			manager.DestroyItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var homeport = Homeport.Instance;
				var slotitems = homeport.Itemyard.SlotItems.Select(x => x.Value).ToArray();
				slotitems = slotitems.Where(x => homeport.Organization.Ships.Any(y => !y.Value.Slots.Select(z => z.Item.Id).Contains(x.Id)))
					.Where(x => x.RawData.api_locked == 0).ToArray(); // Not equiped, Not locked

				this.Current[0] = this.Current[0].Add(args.itemList.Count(x => x == 20)).Max(Maximum[0]); // Type 0 Fighter Model 21
				this.Current[1] = slotitems.Count(x => x.Info.Id == 168).Max(Maximum[1]); // Type 96 Land-based Attack Aircraft
				this.Current[2] = slotitems.Count(x => x.Info.Id == 16).Max(Maximum[2]); // Type 97 Torpedo Bomber
			};
			manager.EquipEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var homeport = Homeport.Instance;
				var slotitems = homeport.Itemyard.SlotItems.Select(x => x.Value).ToArray();
				slotitems = slotitems.Where(x => homeport.Organization.Ships.Any(y => !y.Value.Slots.Select(z => z.Item.Id).Contains(x.Id)))
					.Where(x => x.RawData.api_locked == 0).ToArray(); // Not equiped, Not locked

				this.Current[1] = slotitems.Count(x => x.Info.Id == 168).Max(Maximum[1]); // Type 96 Land-based Attack Aircraft
				this.Current[2] = slotitems.Count(x => x.Info.Id == 16).Max(Maximum[2]); // Type 97 Torpedo Bomber
			};
		}
	}
}
