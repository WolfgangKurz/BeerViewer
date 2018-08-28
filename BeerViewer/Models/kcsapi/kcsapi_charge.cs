namespace BeerViewer.Models.kcsapi
{
	public class kcsapi_charge
	{
		public kcsapi_charge_ship[] api_ship { get; set; }
		public int[] api_material { get; set; }
		public int api_use_bou { get; set; }
	}
}
