using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BeerViewer.Models.kcsapi.mst;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models.Wrapper
{
	public class ShipType : SvData<kcsapi_mst_stype>, IIdentifiable
	{
		public int Id => this.api_data.api_id;
		public int SortNumber => this.api_data.api_sortno;
		public string Name => this.api_data.api_name;

		internal ShipType(kcsapi_mst_stype RawData) : base(RawData) { }

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Name}\"";
	}
}
