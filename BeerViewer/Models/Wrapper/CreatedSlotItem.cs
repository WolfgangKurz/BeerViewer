using System;

using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;
using BeerViewer.Modules;

namespace BeerViewer.Models.Wrapper
{
	public class CreatedSlotItem : RawDataWrapper<kcsapi_createitem>
	{
		public bool Succeed => this.RawData.api_create_flag == 1;

		public SlotItemInfo SlotItemInfo { get; }

		public CreatedSlotItem(kcsapi_createitem RawData) : base(RawData)
		{
			try
			{
				this.SlotItemInfo = this.Succeed
					? Master.Instance.SlotItems[this.RawData.api_slot_item.api_slotitem_id]
					: Master.Instance.SlotItems[int.Parse(this.RawData.api_fdata.Split(',')[1])];

				Logger.Log("createitem: {0} - {1}", this.Succeed, this.SlotItemInfo.Name);
			}
			catch (Exception ex)
			{
				Logger.Log(ex.ToString());
			}
		}
	}
}
