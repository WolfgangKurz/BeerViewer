namespace BeerViewer.Models.kcsapi
{
	public class kcsapi_kdock_getship
	{
		public int api_id { get; set; }
		public int api_ship_id { get; set; }
		public kcsapi_kdock[] api_kdock { get; set; }
		public kcsapi_ship2 api_ship { get; set; }
		public kcsapi_slotitem[] api_slotitem { get; set; }
	}
}
