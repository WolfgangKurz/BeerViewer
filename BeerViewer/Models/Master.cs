using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	/// <summary>
	/// 플레이어 데이터에 존재하지 않는 마스터 데이터
	/// </summary>
	public class Master
	{
		/// <summary>
		/// 전체 칸무스 데이터
		/// </summary>
		public MasterTable<ShipInfo> Ships { get; }

		/// <summary>
		/// 전체 장비 타입 데이터
		/// </summary>
		public MasterTable<SlotItemEquipType> SlotItemEquipTypes { get; }

		/// <summary>
		/// 전체 장비 데이터
		/// </summary>
		public MasterTable<SlotItemInfo> SlotItems { get; }

		/// <summary>
		/// 전체 소비 아이템 데이터
		/// </summary>
		public MasterTable<UseItemInfo> UseItems { get; }

		/// <summary>
		/// 함종 데이터
		/// </summary>
		public MasterTable<ShipType> ShipTypes { get; }

		/// <summary>
		/// 전체 임무 데이터
		/// </summary>
		public MasterTable<Mission> Missions { get; }

		/// <summary>
		/// 전체 해역 데이터
		/// </summary>
		public MasterTable<MapArea> MapAreas { get; }

		/// <summary>
		/// 전체 맵 데이터
		/// </summary>
		public MasterTable<MapInfo> MapInfos { get; }


		internal Master(kcsapi_start2 start2)
		{
			this.ShipTypes = new MasterTable<ShipType>(start2.api_mst_stype.Select(x => new ShipType(x)));
			this.Ships = new MasterTable<ShipInfo>(start2.api_mst_ship.Select(x => new ShipInfo(x)));
			this.SlotItemEquipTypes = new MasterTable<SlotItemEquipType>(start2.api_mst_slotitem_equiptype.Select(x => new SlotItemEquipType(x)));
			this.SlotItems = new MasterTable<SlotItemInfo>(start2.api_mst_slotitem.Select(x => new SlotItemInfo(x, this.SlotItemEquipTypes)));
			this.UseItems = new MasterTable<UseItemInfo>(start2.api_mst_useitem.Select(x => new UseItemInfo(x)));
			this.Missions = new MasterTable<Mission>(start2.api_mst_mission.Select(x => new Mission(x)));
			this.MapInfos = new MasterTable<MapInfo>(start2.api_mst_mapinfo.Select(x => new MapInfo(x)));
			this.MapAreas = new MasterTable<MapArea>(start2.api_mst_maparea.Select(x => new MapArea(x, this.MapInfos)));
		}
	}
}
