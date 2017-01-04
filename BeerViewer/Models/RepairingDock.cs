using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	public class RepairingDock : TimerNotifier, IIdentifiable
	{
		private readonly Homeport homeport;
		private bool notificated;

		#region Level 프로퍼티
		private int _Level;
		public int Level
		{
			get { return this._Level; }
			private set
			{
				if (this._Level != value)
				{
					this._Level = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Id 프로퍼티
		private int _Id;
		public int Id
		{
			get { return this._Id; }
			private set
			{
				if (this._Id != value)
				{
					this._Id = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region State 프로퍼티
		private RepairingDockState _State;
		public RepairingDockState State
		{
			get { return this._State; }
			private set
			{
				if (this._State != value)
				{
					this._State = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region ShipId 프로퍼티
		private int _ShipId;
		public int ShipId
		{
			get { return this._ShipId; }
			private set
			{
				if (this._ShipId != value)
				{
					this._ShipId = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Ship 프로퍼티
		private Ship target;
		public Ship Ship
		{
			get { return this.target; }
			private set
			{
				if (this.target != value)
				{
					var oldShip = this.target;
					var newShip = value;
					if (oldShip != null) oldShip.Situation &= ~ShipSituation.Repair;
					if (newShip != null) newShip.Situation |= ShipSituation.Repair;

					this.target = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region CompleteTime 프로퍼티
		private DateTimeOffset? _CompleteTime;
		public DateTimeOffset? CompleteTime
		{
			get { return this._CompleteTime; }
			private set
			{
				if (this._CompleteTime != value)
				{
					this._CompleteTime = value;
					this.notificated = false;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Remaining 프로퍼티
		private TimeSpan? _Remaining;
		public TimeSpan? Remaining
		{
			get { return this._Remaining; }
			private set
			{
				if (this._Remaining != value)
				{
					this._Remaining = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public event EventHandler<RepairingCompletedEventArgs> Completed;

		internal RepairingDock(Homeport parent, kcsapi_ndock rawData)
		{
			this.homeport = parent;
			this.Update(rawData);
		}

		internal void Update(kcsapi_ndock rawData)
		{
			this.Id = rawData.api_id;
			this.State = (RepairingDockState)rawData.api_state;
			this.ShipId = rawData.api_ship_id;
			this.Ship = this.State == RepairingDockState.Repairing ? this.homeport.Organization.Ships[this.ShipId] : null;
			this.CompleteTime = this.State == RepairingDockState.Repairing
				? (DateTimeOffset?)Const.UnixEpoch.AddMilliseconds(rawData.api_complete_time)
				: null;
			this.Level = this.State == RepairingDockState.Repairing ? this.Ship.Level : 0;
		}

		internal void Finish()
		{
			this.State = RepairingDockState.Unlocked;
			this.ShipId = -1;
			this.Ship = null;
			this.CompleteTime = null;
		}


		protected override void Tick()
		{
			base.Tick();

			if (this.CompleteTime.HasValue)
			{
				var remaining = this.CompleteTime.Value - DateTimeOffset.Now;
				if (remaining.Ticks < 0) remaining = TimeSpan.Zero;

				this.Remaining = remaining;

				if (!this.notificated && this.Completed != null && remaining <= TimeSpan.FromSeconds(Const.NotificationTime))
				{
					this.Completed(this, new RepairingCompletedEventArgs(this.Id, this.Ship));
					this.notificated = true;
				}
			}
			else
			{
				this.Remaining = null;
			}
		}
	}
}
