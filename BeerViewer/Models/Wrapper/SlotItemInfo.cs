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
	public class SlotItemInfo : SvData<kcsapi_mst_slotitem>, IIdentifiable
	{
		#region For singleton
		private SlotItemTypes? _Type;
		private SlotItemIcons? _IconType;
		private int? _CategoryId;
		#endregion

		public int Id => this.api_data.api_id;
		public string Name => this.api_data.api_name;

		public SlotItemTypes Type => this._Type
			?? (SlotItemTypes)(this._Type = (SlotItemTypes)(this.api_data.api_type.Get(2) ?? 0));

		public SlotItemIcons IconType => this._IconType
			?? (SlotItemIcons)(this._IconType = (SlotItemIcons)(this.api_data.api_type.Get(3) ?? 0));

		public int CategoryId => this._CategoryId
			?? (int)(this._CategoryId = this.api_data.api_type.Get(2) ?? int.MaxValue);

		public virtual int Firepower => this.api_data.api_houg;
		public virtual int Torpedo => this.api_data.api_raig;
		public virtual int AA => this.api_data.api_tyku;
		public virtual int Bombing => this.api_data.api_baku;
		public virtual int ASW => this.api_data.api_tais;
		public virtual int Armor => this.api_data.api_souk;
		public virtual int Accuracy => this.api_data.api_houm;
		public virtual int Evation => this.api_data.api_houk;
		public virtual int LOS => this.api_data.api_saku;

		public bool IsNumerable => this.Type.IsNumerable();

		public bool IsFirstEncounter => this.Type == SlotItemTypes.CarrierBased_ReconPlane
										|| this.Type == SlotItemTypes.ReconSeaplane;

		public bool IsSecondEncounter => this.Type == SlotItemTypes.CarrierBased_ReconPlane
										|| this.Type == SlotItemTypes.CarrierBased_TorpedoBomber
										|| this.Type == SlotItemTypes.ReconSeaplane;

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

		public SlotItemType EquipType { get; }

		internal SlotItemInfo(kcsapi_mst_slotitem api_data, MasterTable<SlotItemType> types) : base(api_data)
		{
			this.EquipType = types[api_data.api_type?[2] ?? 0] ?? null;
		}

		public override string ToString()
			=> $"ID = {this.Id}, Name = \"{this.Name}\", Type = {{{string.Join(", ",this.api_data.api_type)}}}";
	}
}
