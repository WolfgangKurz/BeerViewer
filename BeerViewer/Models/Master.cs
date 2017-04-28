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

		public MasterTable<ShipInfo> Ships { get; private set; }
		public MasterTable<ShipType> ShipTypes { get; private set; }

		public MasterTable<SlotItemInfo> SlotItems { get; }
		public MasterTable<SlotItemType> SlotItemEquipTypes { get; }
		public MasterTable<UseItemInfo> UseItems { get; }

		public MasterTable<Mission> Missions { get; }

		public MasterTable<MapArea> MapAreas { get; }
		public MasterTable<MapInfo> MapInfos { get; }

		public Master()
		{
			this.Ships = new MasterTable<ShipInfo>();
			this.ShipTypes = new MasterTable<ShipType>();
		}
		public void Ready()
		{
			if (IsReady) return;

			Proxy.Instance.Register(Proxy.api_start2, e =>
			{
				var x = e.TryParse<kcsapi_start2>();
				if (x == null) return;

				this.Ships = new MasterTable<ShipInfo>(x.api_data.api_mst_ship.Select(y => new ShipInfo(y)));
				this.ShipTypes = new MasterTable<ShipType>(x.api_data.api_mst_stype.Select(y => new ShipType(y)));
			});
		}
	}
}
