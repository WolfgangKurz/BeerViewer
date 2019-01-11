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
		public Materials Materials { get; private set; }

		public Organization Organization { get; private set; }
		public Itemyard Itemyard { get; private set; }
		public Dockyard Dockyard { get; private set; }
		public Repairyard Repairyard { get; private set; }
		public Quests Quests { get; private set; }

		public void Ready()
		{
			if (IsReady) return;

			this.Materials = new Materials();
			this.Itemyard = new Itemyard(this);
			this.Organization = new Organization(this);
			this.Repairyard = new Repairyard(this);
			this.Dockyard = new Dockyard();
			this.Quests = new Quests();

			var proxy = Proxy.Instance;
			proxy.Register<kcsapi_require_info>(Proxy.api_get_member_require_info, x =>
			{
				this.UpdateAdmiral(x.Data.api_basic);
				this.Itemyard.Update(x.Data.api_slot_item);
				this.Dockyard.Update(x.Data.api_kdock);
			});
			proxy.Register<kcsapi_port>(Proxy.api_port, x =>
			{
				this.UpdateAdmiral(x.Data.api_basic);
				this.Organization.Update(x.Data.api_ship);
				this.Repairyard.Update(x.Data.api_ndock);
				this.Organization.Update(x.Data.api_deck_port);
				this.Organization.Combined = x.Data.api_combined_flag != 0;
				this.Materials.Update(x.Data.api_material);
			});
			proxy.Register<kcsapi_basic>(Proxy.api_get_member_basic, x => this.UpdateAdmiral(x.Data));
		}

		internal void UpdateAdmiral(kcsapi_basic data)
		{
			this.Admiral = new Admiral(data);
		}
	}
}
