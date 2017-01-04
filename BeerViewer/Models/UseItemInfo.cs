using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	/// <summary>
	/// 소비 아이템 데이터
	/// </summary>
	public class UseItemInfo : RawDataWrapper<kcsapi_mst_useitem>, IIdentifiable
	{
		public int Id => this.RawData.api_id;
		public string Name => this.RawData.api_name;

		internal UseItemInfo(kcsapi_mst_useitem rawData) : base(rawData) { }

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Name}\"";
		}
	}
}
