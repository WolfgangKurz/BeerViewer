using System.Linq;

using BeerViewer.Models.Raw;
using BeerViewer.Models.Enums;
using BeerViewer.Models.kcsapi.mst;

namespace BeerViewer.Models.Wrapper
{
	public class ShipInfo : RawDataWrapper<kcsapi_mst_ship>, IIdentifiable
	{
		private ShipType _ShipType;

		public static int[] AircraftShipTypes { get; } = new[] { 6, 7, 10, 11, 14, 16, 18, 20, 22 };

		#region Wrapping
		public int Id => base.RawData.api_id;
		public int SortId => this.RawData.api_sortno;

		public string Name => this.RawData.api_name;
		public ShipType ShipType => this._ShipType
			?? (ShipType)(this._ShipType = Master.Instance.ShipTypes[this.RawData.api_stype])
			?? null;

		public int[] Slots => this.RawData.api_maxeq;
		public ShipSpeed Speed => (ShipSpeed)this.RawData.api_soku;

		public int? NextRemodelingLevel => this.RawData.api_afterlv == 0 ? null : (int?)this.RawData.api_afterlv;

		public string Kana => this.RawData.api_yomi;

		public int HP => this.RawData.api_taik.Get(0) ?? 0;
		public int MaxArmor => this.RawData.api_souk.Get(1) ?? 0;

		public int MaxFirepower => this.RawData.api_houg.Get(1) ?? 0;
		public int MaxTorpedo => this.RawData.api_raig.Get(1) ?? 0;
		public int MaxAA => this.RawData.api_tyku.Get(1) ?? 0;

		public int MinLuck => this.RawData.api_luck.Get(0) ?? 0;
		public int MaxLuck => this.RawData.api_luck.Get(1) ?? 0;

		public int MaxFuel => this.RawData.api_fuel_max;
		public int MaxBull => this.RawData.api_bull_max;

		public int SlotCount => this.RawData.api_slot_num;
		#endregion

		public bool IsAirCraft => ShipInfo.AircraftShipTypes.Contains(this.ShipType?.Id ?? -1);

		internal ShipInfo(kcsapi_mst_ship api_data) : base(api_data) { }

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Name}\", ShipType = \"{this.ShipType?.Name}\"";


		private static ShipInfo _Empty { get; } = new ShipInfo(new kcsapi_mst_ship
		{
			api_id = 0,
			api_name = "???"
		});
		public static ShipInfo Empty() => _Empty;
	}
}
