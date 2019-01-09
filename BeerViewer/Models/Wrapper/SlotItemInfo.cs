using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Models.Raw;
using BeerViewer.Models.Enums;
using BeerViewer.Models.kcsapi.mst;

namespace BeerViewer.Models.Wrapper
{
	public class SlotItemInfo : RawDataWrapper<kcsapi_mst_slotitem>, IIdentifiable
	{
		#region For singleton
		private SlotItemType? _Type;
		private SlotItemIconType? _IconType;
		private int? _CategoryId;
		#endregion

		public int Id => this.RawData.api_id;
		public string Name => this.RawData.api_name;

		public SlotItemType Type => this._Type
			?? (SlotItemType)(this._Type = (SlotItemType)(this.RawData.api_type.Get(2) ?? 0));

		public SlotItemIconType IconType => this._IconType
			?? (SlotItemIconType)(this._IconType = (SlotItemIconType)(this.RawData.api_type.Get(3) ?? 0));

		public int CategoryId => this._CategoryId
			?? (int)(this._CategoryId = this.RawData.api_type.Get(2) ?? int.MaxValue);

		public virtual int Firepower => this.RawData.api_houg;
		public virtual int Torpedo => this.RawData.api_raig;
		public virtual int AA => this.RawData.api_tyku;
		public virtual int Bombing => this.RawData.api_baku;
		public virtual int ASW => this.RawData.api_tais;
		public virtual int Armor => this.RawData.api_souk;
		public virtual int Accuracy => this.RawData.api_houm;
		public virtual int Evation => this.RawData.api_houk;
		public virtual int LOS => this.RawData.api_saku;

		public bool IsNumerable => this.Type.IsNumerable();

		public bool IsFirstEncounter => this.Type == SlotItemType.CarrierBased_ReconPlane
										|| this.Type == SlotItemType.ReconSeaplane;

		public bool IsSecondEncounter => this.Type == SlotItemType.CarrierBased_ReconPlane
										|| this.Type == SlotItemType.CarrierBased_TorpedoBomber
										|| this.Type == SlotItemType.ReconSeaplane;

		public double SecondEncounter => this.LOS * 0.07;

		public string ToolTipData
		{
			get
			{
				var tooltips = new string[] {
					(this.Firepower != 0 ? "Firepower:" + this.Firepower : ""),
					(this.Torpedo != 0 ? "Torpedo:" + this.Torpedo : ""),
					(this.AA != 0 ? "AA:" + this.AA : ""),
					(this.Bombing != 0 ? "Bombing:" + this.Bombing : ""),
					(this.ASW != 0 ? "ASW:" + this.ASW : ""),
					(this.Armor != 0 ? "Armor:" + this.Armor : ""),
					(this.Accuracy != 0 ? "Accuracy:" + this.Accuracy : ""),
					(this.Evation != 0 ? "Evasion:" + this.Evation : ""),
					(this.LOS != 0 ? "LOS:" + this.LOS : "")
				};

				if (tooltips.Length == 0) return "No status";
				return string.Join(Environment.NewLine, tooltips).Trim();
			}
		}

		public SlotItemEquipType EquipType { get; }

		internal SlotItemInfo(kcsapi_mst_slotitem api_data, MasterTable<SlotItemEquipType> types) : base(api_data)
		{
			this.EquipType = types[api_data.api_type?[2] ?? 0] ?? SlotItemEquipType.Empty;
		}

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Name}\", Type = {{{string.Join(", ",this.RawData?.api_type ?? new int[0])}}}";

		public static SlotItemInfo Empty { get; } = new SlotItemInfo(
			new kcsapi_mst_slotitem()
			{
				api_id = 0,
				api_name = "???",
			},
			new MasterTable<SlotItemEquipType>()
		);
	}
}
