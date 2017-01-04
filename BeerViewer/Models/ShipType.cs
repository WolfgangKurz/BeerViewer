using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	/// <summary>
	/// 함종 데이터
	/// </summary>
	public class ShipType : RawDataWrapper<kcsapi_mst_stype>, IIdentifiable
	{
		public int Id => this.RawData.api_id;
		public int SortNumber => this.RawData.api_sortno;
		public string Name => Translator.ShipTypeTable.ContainsKey(this.RawData.api_id)
			? Translator.ShipTypeTable[this.RawData.api_id]
			: this.RawData.api_name;

		public ShipType(kcsapi_mst_stype RawData) : base(RawData) { }

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Name}\"";
		}

		#region Static Members
		public static ShipType Dummy { get; } = new ShipType(new kcsapi_mst_stype
		{
			api_id = 999,
			api_sortno = 999,
			api_name = "???",
		});
		#endregion
	}
}
