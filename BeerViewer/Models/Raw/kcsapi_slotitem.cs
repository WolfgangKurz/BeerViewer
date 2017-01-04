using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.Raw
{
	public class kcsapi_slotitem
	{
		public int api_id { get; set; }
		public int api_slotitem_id { get; set; }
		public int api_level { get; set; }
		public int api_locked { get; set; }
		public int api_alv { get; set; }
	}
}
