﻿namespace BeerViewer.Models.kcsapi
{
	public class kcsapi_combined_each_battle
	{
		public int api_deck_id { get; set; }
		public int[] api_ship_ke { get; set; }
		public int[] api_ship_ke_combined { get; set; }
		public int[] api_ship_lv { get; set; }
		public int[] api_ship_lv_combined { get; set; }
		public int[] api_nowhps { get; set; }
		public int[] api_maxhps { get; set; }
		public int[] api_nowhps_combined { get; set; }
		public int[] api_maxhps_combined { get; set; }
		public int api_midnight_flag { get; set; }
		public int[][] api_eSlot { get; set; }
		public int[][] api_eSlot_combined { get; set; }
		public int[][] api_eKyouka { get; set; }
		public int[][] api_fParam { get; set; }
		public int[][] api_eParam { get; set; }
		public int[][] api_fParam_combined { get; set; }
		public int[][] api_eParam_combined { get; set; }
		public int[] api_search { get; set; }
		public int[] api_formation { get; set; }
		public kcsapi_data_airbase_injection api_air_base_injection { get; set; }
		public kcsapi_data_kouku api_injection_kouku { get; set; }
		public kcsapi_data_airbaseattack[] api_air_base_attack { get; set; }
		public int[] api_stage_flag { get; set; }
		public kcsapi_data_kouku api_kouku { get; set; }

		public int api_support_flag { get; set; }
		public kcsapi_data_support_info api_support_info { get; set; }
		public int api_opening_taisen_flag { get; set; }
		public kcsapi_data_hougeki api_opening_taisen { get; set; }
		public int api_opening_flag { get; set; }
		public kcsapi_data_raigeki api_opening_atack { get; set; }
		public int[] api_hourai_flag { get; set; }
		public kcsapi_data_hougeki api_hougeki1 { get; set; }
		public kcsapi_data_raigeki api_raigeki { get; set; }
		public kcsapi_data_hougeki api_hougeki2 { get; set; }
		public kcsapi_data_hougeki api_hougeki3 { get; set; }
	}
}
