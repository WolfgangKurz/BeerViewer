using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi.mst;

namespace BeerViewer.Models.Wrapper
{
	public class SlotItemEquipType : SvData<kcsapi_mst_slotitem_equiptype>, IIdentifiable
	{
		public int Id => this.api_data.api_id;
		public string Name => this.api_data.api_name;

		internal SlotItemEquipType(kcsapi_mst_slotitem_equiptype RawData) : base(RawData) { }

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Name}\"";

		public static SlotItemEquipType Empty { get; } = new SlotItemEquipType(new kcsapi_mst_slotitem_equiptype
		{
			api_id = 0,
			api_name = "???"
		});
	}
}
