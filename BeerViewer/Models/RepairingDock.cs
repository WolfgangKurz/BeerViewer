using System;

using BeerViewer.Models.Enums;
using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models
{
	public class RepairingCompletedEventArgs : EventArgs
	{
		public int DockId { get; private set; }
		public Ship Ship { get; private set; }

		public RepairingCompletedEventArgs(int id, Ship ship)
		{
			this.DockId = id;
			this.Ship = ship;
		}
	}

	public class RepairingDock : TimerNotifier, IIdentifiable
	{
		private readonly Homeport homeport;
		private bool notificated;

		#region Level Property
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

		#region Id Property
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

		#region State Property
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

		#region ShipId Property
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

		#region Ship Property
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

		#region CompleteTime Property
		private DateTime? _CompleteTime;
		public DateTime? CompleteTime
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

		#region Remaining Property
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
					this.RaisePropertyChanged(nameof(RemainingText));
					this.RaisePropertyChanged(nameof(IsCompleted));
				}
			}
		}

		public string RemainingText => this.Remaining.HasValue
			? $"{(int)this.Remaining.Value.TotalHours:D2}:{this.Remaining.Value.ToString(@"mm\:ss")}"
			: "--:--:--";

		public bool IsCompleted => this.Remaining.HasValue
			? this.Remaining.Value == TimeSpan.Zero
			: false;
		#endregion

		public event EventHandler<RepairingCompletedEventArgs> Completed;

		internal RepairingDock(Homeport parent, kcsapi_ndock Data)
		{
			this.homeport = parent;
			this.Update(Data);
		}

		internal void Update(kcsapi_ndock Data)
		{
			this.Id = Data.api_id;
			this.State = (RepairingDockState)Data.api_state;
			this.ShipId = Data.api_ship_id;
			this.Ship = this.State == RepairingDockState.Repairing ? this.homeport.Organization.Ships[this.ShipId] : null;
			this.CompleteTime = this.State == RepairingDockState.Repairing
				? (DateTime?)Extensions.UnixEpoch.AddMilliseconds(Data.api_complete_time)
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
				var remaining = this.CompleteTime.Value - DateTime.Now;
				if (remaining.Ticks < 0) remaining = TimeSpan.Zero;

				this.Remaining = remaining;

				if (!this.notificated && this.Completed != null && remaining <= TimeSpan.FromSeconds(Settings.NotificationTime))
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
