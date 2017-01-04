using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.BattleInfo.Raw
{
	public class battle_midnight_battle : ICommonBattleMembers
	{
		public int api_deck_id { get; set; }
		public int[] api_nowhps { get; set; }
		public int[] api_ship_ke { get; set; }
		public int[] api_ship_lv { get; set; }
		public int[] api_maxhps { get; set; }
		public int[][] api_eSlot { get; set; }
		public int[][] api_eKyouka { get; set; }
		public int[][] api_fParam { get; set; }
		public int[][] api_eParam { get; set; }
		public int[] api_touch_plane { get; set; }
		public int[] api_flare_pos { get; set; }
		public Midnight_Hougeki api_hougeki { get; set; }
	}
}
