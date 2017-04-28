using System;

using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models.Wrapper
{
	public class CreatedSlotItem : SvData<kcsapi_createitem>
	{
		public bool Succeed => this.api_data.api_create_flag == 1;

		public SlotItemInfo SlotItemInfo { get; }

		public CreatedSlotItem(kcsapi_createitem rawData) : base(rawData)
		{
			try
			{
				this.SlotItemInfo = this.Succeed
					? Master.Instance.SlotItems[rawData.api_slot_item.api_slotitem_id]
					: Master.Instance.SlotItems[int.Parse(rawData.api_fdata.Split(',')[1])];

				System.Diagnostics.Debug.WriteLine("createitem: {0} - {1}", this.Succeed, this.SlotItemInfo.Name);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}
	}
}
