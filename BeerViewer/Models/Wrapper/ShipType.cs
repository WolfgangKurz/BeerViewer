using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BeerViewer.Models.kcsapi.mst;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models.Wrapper
{
	public class ShipType : RawDataWrapper<kcsapi_mst_stype>, IIdentifiable
	{
		public int Id => this.RawData.api_id;
		public int SortNumber => this.RawData.api_sortno;
		public string Name => this.RawData.api_name;

		internal ShipType(kcsapi_mst_stype Data) : base(Data) { }

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Name}\"";
	}
}
