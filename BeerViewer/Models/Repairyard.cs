using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	public class Repairyard : Notifier
	{
		private readonly Homeport homeport;

		#region Docks 프로퍼티
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

			proxy.Register(Proxy.api_get_member_ndock, e =>
			{
				var x = e.TryParse<kcsapi_ndock[]>();
				if (x == null) return;

				this.Update(x.Data);
			});
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

				if (highspeed)
				{
					ship.Repair();
					this.homeport.Organization.GetFleet(ship.Id)?.State.Update();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("입거 개시 해석 실패: {0}", ex);
			}
		}

		private void ChangeSpeed(SvData data)
		{
			try
			{
				var dock = this.Docks[int.Parse(data.Request["api_ndock_id"])];
				var ship = dock.Ship;

				dock.Finish();
				ship.Repair();

				this.homeport.Organization.GetFleet(ship.Id)?.State.Update();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("고속수복재 해석 실패: {0}", ex);
			}
		}
	}
}
