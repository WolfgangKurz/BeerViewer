using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	/// <summary>
	/// 장비 타입
	/// </summary>
	public class SlotItemEquipType : RawDataWrapper<kcsapi_mst_slotitem_equiptype>, IIdentifiable
	{
		public int Id { get; }

		public string Name { get; }

		public SlotItemEquipType(kcsapi_mst_slotitem_equiptype RawData) : base(RawData)
		{
			this.Id = RawData.api_id;
			this.Name = RawData.api_name;
		}

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Name}\"";
		}

		#region static members

		public static SlotItemEquipType Dummy { get; } = new SlotItemEquipType(new kcsapi_mst_slotitem_equiptype
		{
			api_id = 0,
			api_name = "???",
		});

		#endregion
	}
}
