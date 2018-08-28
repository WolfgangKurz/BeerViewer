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
	/// 정예 함전대의 신 편성 (영전21형 숙련, 월간)
	/// </summary>
	internal class F22 : MultipleTracker
	{
		public override QuestType Type => QuestType.Monthly;
		public override int Id => 626;

		public override int[] Maximum => new int[] { 2, 1 };

		public override void RegisterEvent(TrackManager manager)
		{
			manager.DestroyItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var flagshipTable = new int[]
				{
					89,  // Houshou
					285, // Houshou Kai
				};

				var fleet = Homeport.Instance.Organization.Fleets[1];
				if (!flagshipTable.Any(x => x == (fleet?.Ships[0]?.Info.Id ?? 0))) return; // Flagship is not Houshou

				var slotitems = fleet?.Ships[0]?.Slots;
				if (!slotitems.Any(x => x.Item.Info.Id == 20 && x.Item.Proficiency == 7)) return; // Max proficiency Type 0 Fighter Model 21

				var homeportSlotitems = manager.slotitemTracker.SlotItems;
				this.Current[0] = this.Current[0].Add(args.itemList.Count(x => (homeportSlotitems[x]?.Info.Id ?? 0) == 20)).Max(2); // Type 0 Fighter Model 21
				this.Current[1] = this.Current[1].Add(args.itemList.Count(x => (homeportSlotitems[x]?.Info.Id ?? 0) == 19)).Max(1); // Type 96 Fighter
			};
		}
	}
}
