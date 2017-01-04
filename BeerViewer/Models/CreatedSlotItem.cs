using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	/// <summary>
	/// 공창에서 생성한 장비 데이터
	/// </summary>
	public class CreatedSlotItem : RawDataWrapper<kcsapi_createitem>
	{
		public bool Succeed => this.RawData.api_create_flag == 1;

		public SlotItemInfo SlotItemInfo { get; }

		public CreatedSlotItem(kcsapi_createitem rawData)
			: base(rawData)
		{
			try
			{
				this.SlotItemInfo = this.Succeed
					? DataStorage.Instance.Master.SlotItems[rawData.api_slot_item.api_slotitem_id]
					: DataStorage.Instance.Master.SlotItems[int.Parse(rawData.api_fdata.Split(',')[1])];

				System.Diagnostics.Debug.WriteLine("createitem: {0} - {1}", this.Succeed, this.SlotItemInfo.Name);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}
	}
}
