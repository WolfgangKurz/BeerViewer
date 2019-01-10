using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi.mst;

namespace BeerViewer.Models.Wrapper
{
	public class SlotItemEquipType : RawDataWrapper<kcsapi_mst_slotitem_equiptype>, IIdentifiable
	{
		public int Id => this.RawData.api_id;
		public string Name => this.RawData.api_name;

		internal SlotItemEquipType(kcsapi_mst_slotitem_equiptype Data) : base(Data) { }

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Name}\"";

		private static SlotItemEquipType _Empty { get; } = new SlotItemEquipType(new kcsapi_mst_slotitem_equiptype
		{
			api_id = 0,
			api_name = "???"
		});
		public static SlotItemEquipType Empty() => _Empty;
	}
}
