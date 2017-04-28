using System.Collections.Generic;
using System.Linq;

using BeerViewer.Network;
using BeerViewer.Models.Raw;
using BeerViewer.Models.Wrapper;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models
{
	public class Master
	{
		public static Master Instance { get; } = new Master();
		public bool IsReady { get; private set; } = false;

		public MasterTable<ShipType> ShipTypes { get; private set; }
		public MasterTable<ShipInfo> Ships { get; private set; }

		public MasterTable<SlotItemEquipType> SlotItemEquipTypes { get; private set; }
		public MasterTable<SlotItemInfo> SlotItems { get; private set; }
		public MasterTable<UseItemInfo> UseItems { get; private set; }

		public MasterTable<Mission> Missions { get; private set; }

		public MasterTable<MapInfo> MapInfos { get; private set; }
		public MasterTable<MapArea> MapAreas { get; private set; }

		public void Ready()
		{
			if (IsReady) return;

			Proxy.Instance.Register<kcsapi_start2>(Proxy.api_start2, x =>
			{
				this.ShipTypes = new MasterTable<ShipType>(x.api_data.api_mst_stype.Select(y => new ShipType(y)));
				this.Ships = new MasterTable<ShipInfo>(x.api_data.api_mst_ship.Select(y => new ShipInfo(y)));

				this.SlotItemEquipTypes = new MasterTable<SlotItemEquipType>(x.api_data.api_mst_slotitem_equiptype.Select(y => new SlotItemEquipType(y)));
				this.SlotItems = new MasterTable<SlotItemInfo>(x.api_data.api_mst_slotitem.Select(y => new SlotItemInfo(y, this.SlotItemEquipTypes)));
				this.UseItems = new MasterTable<UseItemInfo>(x.api_data.api_mst_useitem.Select(y => new UseItemInfo(y)));

				this.Missions = new MasterTable<Mission>(x.api_data.api_mst_mission.Select(y => new Mission(y)));

				this.MapInfos = new MasterTable<MapInfo>(x.api_data.api_mst_mapinfo.Select(y => new MapInfo(y)));
				this.MapAreas = new MasterTable<MapArea>(x.api_data.api_mst_maparea.Select(y => new MapArea(y, this.MapInfos)));
			});
		}
	}
}
