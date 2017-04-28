using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.kcsapi
{
	public class kcsapi_combined_battle_battleresult
	{
		public int[] api_ship_id { get; set; }
		public string api_win_rank { get; set; }
		public int api_get_exp { get; set; }
		public int api_mvp { get; set; }
		public int api_mvp_combined { get; set; }
		public int api_member_lv { get; set; }
		public int api_member_exp { get; set; }
		public int api_get_base_exp { get; set; }
		public int[] api_get_ship_exp { get; set; }
		public int[] api_get_ship_exp_combined { get; set; }
		public int[][] api_get_exp_lvup { get; set; }
		public int[][] api_get_exp_lvup_combined { get; set; }
		public int api_dests { get; set; }
		public int api_destsf { get; set; }
		public string api_quest_name { get; set; }
		public int api_quest_level { get; set; }
		public api_enemy_info api_enemy_info { get; set; }
		public int api_first_clear { get; set; }
		public int[] api_get_flag { get; set; }
		public api_get_ship api_get_ship { get; set; }
		public int api_get_exmap_rate { get; set; }
		public int api_get_exmap_useitem_id { get; set; }
		public int api_escape_flag { get; set; }
		public api_escape api_escape { get; set; }
	}

	public class api_enemy_info
	{
		public string api_level { get; set; }
		public string api_rank { get; set; }
		public string api_deck_name { get; set; }
	}

	public class api_get_ship
	{
		public int api_ship_id { get; set; }
		public string api_ship_type { get; set; }
		public string api_ship_name { get; set; }
		public string api_ship_getmes { get; set; }
	}

	public class api_escape
	{
		public int[] api_escape_idx { get; set; }
		public int[] api_tow_idx { get; set; }
	}
}
