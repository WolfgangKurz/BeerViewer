using System;
using System.Linq;

using BeerViewer.Network;
using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models
{
	public class Repairyard : Notifier
	{
		private readonly Homeport homeport;

		#region Docks Property
		private MemberTable<RepairingDock> _Docks;
		public MemberTable<RepairingDock> Docks
		{
			get { return this._Docks; }
			set
			{
				if (this._Docks != value)
				{
					this._Docks = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		internal Repairyard(Homeport parent)
		{
			var proxy = Proxy.Instance;
			this.homeport = parent;
			this.Docks = new MemberTable<RepairingDock>();

			proxy.Register<kcsapi_ndock[]>(Proxy.api_get_member_ndock, x => this.Update(x.Data));
			proxy.Register(Proxy.api_req_nyukyo_start, e =>
			{
				var x = e.TryParse();
				if (x == null) return;

				this.Start(x);
			});
			proxy.Register(Proxy.api_req_nyukyo_speedchange, e =>
			{
				var x = e.TryParse();
				if (x == null) return;

				this.ChangeSpeed(x);
			});
		}

		public bool CheckRepairing(int shipId) =>
			this.Docks.Values.Where(x => x.Ship != null).Any(x => x.ShipId == shipId);

		public bool CheckRepairing(Fleet fleet)
		{
			var repairingShipIds = this.Docks.Values.Where(x => x.Ship != null).Select(x => x.Ship.Id);
			return fleet.Ships.Any(x => repairingShipIds.Any(id => id == x.Id));
		}

		internal void Update(kcsapi_ndock[] source)
		{
			if (this.Docks.Count == source.Length)
			{
				foreach (var raw in source)
					this.Docks[raw.api_id]?.Update(raw);
			}
			else
			{
				foreach (var dock in this.Docks)
					dock.Value?.Dispose();

				this.Docks = new MemberTable<RepairingDock>(source.Select(x => new RepairingDock(this.homeport, x)));
			}
		}

		private void Start(SvData data)
		{
			try
			{
				var ship = this.homeport.Organization.Ships[int.Parse(data.Request["api_ship_id"])];
				var highspeed = data.Request["api_highspeed"] == "1";

				if (highspeed) ship.Repair();
			}
			catch { }
		}

		private void ChangeSpeed(SvData data)
		{
			try
			{
				var dock = this.Docks[int.Parse(data.Request["api_ndock_id"])];
				var ship = dock.Ship;

				dock.Finish();
				ship.Repair();
			}
			catch { }
		}
	}
}
