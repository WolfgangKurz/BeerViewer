using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	/// <summary>
	/// 장비 데이터
	/// </summary>
	public class SlotItemInfo : RawDataWrapper<kcsapi_mst_slotitem>, IIdentifiable
	{
		private SlotItemType? type;
		private SlotItemIconType? iconType;
		private int? categoryId;

		public int Id => this.RawData.api_id;

		public string Name => Translator.SlotItemTable.ContainsKey(this.RawData.api_name)
			? Translator.SlotItemTable[this.RawData.api_name]
			: this.RawData.api_name;

		public SlotItemType Type => this.type
			?? (SlotItemType)(this.type = (SlotItemType)(this.RawData.api_type.Get(2) ?? 0));

		public SlotItemIconType IconType => this.iconType
			?? (SlotItemIconType)(this.iconType = (SlotItemIconType)(this.RawData.api_type.Get(3) ?? 0));

		public int CategoryId => this.categoryId
			?? (int)(this.categoryId = this.RawData.api_type.Get(2) ?? int.MaxValue);

		/// <summary>
		/// 화력
		/// </summary>
		public int Firepower => this.RawData.api_houg;

		/// <summary>
		/// 장갑
		/// </summary>
		public int Armor => this.RawData.api_souk;

		/// <summary>
		/// 뇌장
		/// </summary>
		public int Torpedo => this.RawData.api_raig;

		/// <summary>
		/// 대공
		/// </summary>
		public int AA => this.RawData.api_tyku;

		/// <summary>
		/// 폭장
		/// </summary>
		public int Bomb => this.RawData.api_baku;

		/// <summary>
		/// 대잠
		/// </summary>
		public int ASW => this.RawData.api_tais;

		/// <summary>
		/// 명중
		/// </summary>
		public int Hit => this.RawData.api_houm;

		/// <summary>
		/// 회피
		/// </summary>
		public int Evade => this.RawData.api_houk;

		/// <summary>
		/// 색적
		/// </summary>
		public int ViewRange => this.RawData.api_saku;

		public bool IsNumerable => this.Type.IsNumerable();

		public bool IsFirstEncounter => this.Type == SlotItemType.艦上偵察機
									   || this.Type == SlotItemType.水上偵察機;

		public bool IsSecondEncounter => this.Type == SlotItemType.艦上偵察機
									   || this.Type == SlotItemType.艦上攻撃機
									   || this.Type == SlotItemType.水上偵察機;

		public double SecondEncounter => this.ViewRange * 0.07;

		public SlotItemEquipType EquipType { get; }

		internal SlotItemInfo(kcsapi_mst_slotitem rawData, MasterTable<SlotItemEquipType> types) : base(rawData)
		{
			this.EquipType = types[rawData.api_type?[2] ?? 0] ?? SlotItemEquipType.Dummy;
		}
		public string ToolTipData
		{
			get
			{
				var tooltip = (this.RawData.api_houg != 0 ? "화력:" + this.RawData.api_houg : "")
					+ (this.RawData.api_raig != 0 ? " 뇌장:" + this.RawData.api_raig : "")
					+ (this.AA != 0 ? " 대공:" + this.AA : "")
					+ (this.RawData.api_souk != 0 ? " 장갑:" + this.RawData.api_souk : "")
					+ (this.RawData.api_baku != 0 ? " 폭장:" + this.RawData.api_baku : "")
					+ (this.RawData.api_tais != 0 ? " 대잠:" + this.RawData.api_tais : "")
					+ (this.RawData.api_houm != 0 ? " 명중:" + this.RawData.api_houm : "")
					+ (this.RawData.api_houk != 0 ? " 회피:" + this.RawData.api_houk : "")
					+ (this.RawData.api_saku != 0 ? " 색적:" + this.RawData.api_saku : "");

				if (tooltip.Length < 1) tooltip = "스테이터스 없음";
				return tooltip.Trim();
			}
		}

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Name}\", Type = {{{string.Join(", ",this.RawData.api_type)}}}";
		}

		#region Static Members
		public static SlotItemInfo Dummy { get; } = new SlotItemInfo(
			new kcsapi_mst_slotitem()
			{
				api_id = 0,
				api_name = "???",
			},
			new MasterTable<SlotItemEquipType>()
		);
		#endregion
	}
}
