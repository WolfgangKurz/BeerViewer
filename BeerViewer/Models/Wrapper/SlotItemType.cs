using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi.mst;

namespace BeerViewer.Models.Wrapper
{
	public class SlotItemType : SvData<kcsapi_mst_slotitem_equiptype>, IIdentifiable
	{
		public int Id => this.api_data.api_id;
		public string Name => this.api_data.api_name;

		internal SlotItemType(kcsapi_mst_slotitem_equiptype RawData) : base(RawData) { }

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Name}\"";
	}
}
