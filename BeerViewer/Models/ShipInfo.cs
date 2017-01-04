using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	/// <summary>
	/// 칸무스 데이터
	/// </summary>
	public class ShipInfo : RawDataWrapper<kcsapi_mst_ship>, IIdentifiable
	{
		private ShipType shipType;

		/// <summary>
		/// 칸무스 시스템 Id
		/// </summary>
		public int Id => this.RawData.api_id;

		public int SortId => this.RawData.api_sortno;

		/// <summary>
		/// 칸무스 이름
		/// </summary>
		public string Name => Translator.ShipNameTable.ContainsKey(this.RawData.api_name)
			? Translator.ShipNameTable[this.RawData.api_name]
			: this.RawData.api_name;

		/// <summary>
		/// 함종
		/// </summary>
		public ShipType ShipType => this.shipType
			?? (this.shipType = DataStorage.Instance.Master.ShipTypes[this.RawData.api_stype])
			?? ShipType.Dummy;

		/// <summary>
		/// 항공모함으로 분류되는지 여부
		/// </summary>
		public bool IsAirCraft => ShipInfo.AircraftShipTypes.Contains(this.ShipType.Id);

		/// <summary>
		/// 각 슬롯의 최대 탑재량
		/// </summary>
		public int[] Slots => this.RawData.api_maxeq;

		/// <summary>
		/// 속력
		/// </summary>
		public ShipSpeed Speed => (ShipSpeed)this.RawData.api_soku;

		/// <summary>
		/// 다음 개장이 가능해지는 레벨
		/// </summary>
		public int? NextRemodelingLevel => this.RawData.api_afterlv == 0
			? null : (int?)this.RawData.api_afterlv;

		#region 쓰이지 않는 더미 데이터
		/// <summary>
		/// 요미가나
		/// </summary>
		public string Kana => this.RawData.api_yomi;

		/// <summary>
		/// 최대 화력
		/// </summary>
		public int MaxFirepower => this.RawData.api_houg.Get(1) ?? 0;

		/// <summary>
		/// 최대 장갑
		/// </summary>
		public int MaxArmor => this.RawData.api_souk.Get(1) ?? 0;

		/// <summary>
		/// 최대 뇌장
		/// </summary>
		public int MaxTorpedo => this.RawData.api_raig.Get(1) ?? 0;

		/// <summary>
		/// 최대 대공
		/// </summary>
		public int MaxAA => this.RawData.api_tyku.Get(1) ?? 0;

		/// <summary>
		/// 최소 운
		/// </summary>
		public int MinLuck => this.RawData.api_luck.Get(0) ?? 0;

		/// <summary>
		/// 최대 운
		/// </summary>
		public int MaxLuck => this.RawData.api_luck.Get(1) ?? 0;

		/// <summary>
		/// 체력
		/// </summary>
		public int HP => this.RawData.api_taik.Get(0) ?? 0;

		/// <summary>
		/// 연료량
		/// </summary>
		public int MaxFuel => this.RawData.api_fuel_max;

		/// <summary>
		/// 탄약량
		/// </summary>
		public int MaxBull => this.RawData.api_bull_max;

		/// <summary>
		/// 장비 슬롯 갯수
		/// </summary>
		public int SlotCount => this.RawData.api_slot_num;
		#endregion


		internal ShipInfo(kcsapi_mst_ship RawData) : base(RawData) { }

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Name}\", ShipType = \"{this.ShipType.Name}\"";
		}

		#region Static Members
		public static int[] AircraftShipTypes => new[] { 6, 7, 10, 11, 14, 16, 18, 20, 22 };

		public static ShipInfo Dummy { get; } = new ShipInfo(new kcsapi_mst_ship
		{
			api_id = 0,
			api_name = "???"
		});
		#endregion
	}
}
