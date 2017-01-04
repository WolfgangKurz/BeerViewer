using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.Raw
{
	public class kcsapi_port
	{
		public kcsapi_material[] api_material { get; set; }
		public kcsapi_deck[] api_deck_port { get; set; }
		public kcsapi_ndock[] api_ndock { get; set; }
		public kcsapi_ship2[] api_ship { get; set; }
		public kcsapi_basic api_basic { get; set; }
		public int api_combined_flag { get; set; }
		//public Api_Log[] api_log { get; set; }
	}
}
