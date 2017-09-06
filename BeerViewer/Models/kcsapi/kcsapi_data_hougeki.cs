namespace BeerViewer.Models.kcsapi
{
	public class kcsapi_data_hougeki
	{
		public int[] api_at_eflag { get; set; }
		public int[] api_at_list { get; set; }
		public int[] api_at_type { get; set; }
		public object[] api_df_list { get; set; }
		public object[] api_si_list { get; set; }
		public object[] api_cl_list { get; set; }
		public object[] api_damage { get; set; }
	}
	public class kcsapi_data_midnight_hougeki
	{
		public int[] api_at_eflag { get; set; }
		public int[] api_at_list { get; set; }
		public object[] api_df_list { get; set; }
		public object[] api_si_list { get; set; }
		public object[] api_cl_list { get; set; }
		public int[] api_sp_list { get; set; }
		public object[] api_damage { get; set; }
	}
}
