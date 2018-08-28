namespace BeerViewer.Models.kcsapi
{
	public class kcsapi_require_info
	{
		public kcsapi_basic api_basic { get; set; }
		public kcsapi_slotitem[] api_slot_item { get; set; }
		public kcsapi_kdock[] api_kdock { get; set; }
		public kcsapi_useitem[] api_useitem { get; set; }
	}
}
