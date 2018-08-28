namespace BeerViewer.Models.kcsapi
{
	public class kcsapi_remodel_slot
	{
		public int api_remodel_flag { get; set; }
		public int[] api_remodel_id { get; set; }
		public int[] api_after_material { get; set; }
		public int api_voice_ship_id { get; set; }
		public int api_voice_id { get; set; }
		public kcsapi_remodel_after_slot api_after_slot { get; set; }
		public int[] api_use_slot_id { get; set; }
	}
}
