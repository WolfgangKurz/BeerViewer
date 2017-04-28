using System.Linq;

using BeerViewer.Models.Raw;
using BeerViewer.Models.Enums;
using BeerViewer.Models.kcsapi.mst;

namespace BeerViewer.Models.Wrapper
{
	public class ShipInfo : SvData<kcsapi_mst_ship>, IIdentifiable
	{
		private ShipType _ShipType;

		public static int[] AircraftShipTypes { get; } = new[] { 6, 7, 10, 11, 14, 16, 18, 20, 22 };

		#region Wrapping
		public int Id => base.api_data.api_id;
		public int SortId => this.api_data.api_sortno;

		public string Name => this.api_data.api_name;
		public ShipType ShipType => this._ShipType
			?? (ShipType)(this._ShipType = Master.Instance.ShipTypes[this.api_data.api_stype])
			?? null;

		public int[] Slots => this.api_data.api_maxeq;
		public ShipSpeed Speed => (ShipSpeed)this.api_data.api_soku;

		public int? NextRemodelingLevel => this.api_data.api_afterlv == 0 ? null : (int?)this.api_data.api_afterlv;

		public string Kana => this.api_data.api_yomi;

		public int HP => this.api_data.api_taik.Get(0) ?? 0;
		public int MaxArmor => this.api_data.api_souk.Get(1) ?? 0;

		public int MaxFirepower => this.api_data.api_houg.Get(1) ?? 0;
		public int MaxTorpedo => this.api_data.api_raig.Get(1) ?? 0;
		public int MaxAA => this.api_data.api_tyku.Get(1) ?? 0;

		public int MinLuck => this.api_data.api_luck.Get(0) ?? 0;
		public int MaxLuck => this.api_data.api_luck.Get(1) ?? 0;

		public int MaxFuel => this.api_data.api_fuel_max;
		public int MaxBull => this.api_data.api_bull_max;

		public int SlotCount => this.api_data.api_slot_num;
		#endregion

		public bool IsAirCraft => ShipInfo.AircraftShipTypes.Contains(this.ShipType.Id);

		internal ShipInfo(kcsapi_mst_ship api_data) : base(api_data) { }

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Name}\", ShipType = \"{this.ShipType.Name}\"";


		public static ShipInfo Empty { get; } = new ShipInfo(new kcsapi_mst_ship
		{
			api_id = 0,
			api_name = "???"
		});
	}
}
