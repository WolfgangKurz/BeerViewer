using System;
using System.Linq;

using BeerViewer.Network;
using BeerViewer.Models.Raw;
using BeerViewer.Models.Wrapper;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models
{
	public class Homeport : Notifier
	{
		public static Homeport Instance { get; } = new Homeport();
		public bool IsReady { get; private set; } = false;

		public Admiral Admiral { get; private set; }
		public Materials Materials { get; }

		public Organization Organization { get; }
		public Itemyard Itemyard { get; }
		public Dockyard Dockyard { get; }
		public Repairyard Repairyard { get; }
		public Quests Quests { get; }

		public void Ready()
		{
			if (IsReady) return;

			var proxy = Proxy.Instance;
			proxy.Register<kcsapi_require_info>(Proxy.api_get_member_require_info, x =>
			{
				this.UpdateAdmiral(x.api_data.api_basic);
				this.Itemyard.Update(x.api_data.api_slot_item);
				this.Dockyard.Update(x.api_data.api_kdock);
			});
		}

		internal void UpdateAdmiral(kcsapi_basic data)
		{
			this.Admiral = new Admiral(data);
		}
	}
}
