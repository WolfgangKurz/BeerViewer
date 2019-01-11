using System;

using BeerViewer.Models.Enums;
using BeerViewer.Models.Wrapper;
using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models
{
	public class BuildingCompletedEventArgs : EventArgs
	{
		public int DockId { get; }
		public ShipInfo Ship { get; }

		public BuildingCompletedEventArgs(int id, ShipInfo ship)
		{
			this.DockId = id;
			this.Ship = ship;
		}
	}

	public class BuildingDock : TimerNotifier, IIdentifiable
	{
		private bool notificated;

		#region Id Property
		private int _Id;
		public int Id
		{
			get { return this._Id; }
			private set
			{
				this._Id = value;
				this.RaisePropertyChanged();
			}
		}
		#endregion

		#region State Property
		private BuildingDockState _State;
		public BuildingDockState State
		{
			get { return this._State; }
			private set
			{
				if (this._State != value)
				{
					this._State = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(IsCompleted));
				}
			}
		}
		#endregion

		#region Ship Property
		private ShipInfo _Ship;
		public ShipInfo Ship
		{
			get { return this._Ship; }
			private set
			{
				if (this._Ship != value)
				{
					this._Ship = value;
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

		public bool IsCompleted => this.State == BuildingDockState.Completed
			? true
			: this.Remaining.HasValue
				? this.Remaining.Value == TimeSpan.Zero
				: false;
		#endregion

		public event EventHandler<BuildingCompletedEventArgs> Completed;

		internal BuildingDock(kcsapi_kdock Data)
		{
			this.Update(Data);
		}

		internal void Update(kcsapi_kdock Data)
		{
			this.Id = Data.api_id;
			this.State = (BuildingDockState)Data.api_state;
			this.Ship = this.State == BuildingDockState.Building || this.State == BuildingDockState.Completed
				? Master.Instance.Ships[Data.api_created_ship_id]
				: null;
			this.CompleteTime = this.State == BuildingDockState.Building
				? (DateTime?)Extensions.UnixEpoch.AddMilliseconds(Data.api_complete_time)
				: null;
		}

		internal void Finish()
		{
			this.State = BuildingDockState.Completed;
			this.CompleteTime = null;
		}

		protected override void Tick()
		{
			base.Tick();

			if (this.CompleteTime.HasValue)
			{
				var remaining = this.CompleteTime.Value.Subtract(DateTime.Now);
				if (remaining.Ticks < 0) remaining = TimeSpan.Zero;

				this.Remaining = remaining;

				if (!this.notificated && this.Completed != null && remaining.Ticks <= 0)
				{
					this.Completed(this, new BuildingCompletedEventArgs(this.Id, this.Ship));
					this.notificated = true;
					this.Finish();
				}
			}
			else
			{
				this.Remaining = null;
			}
		}
	}
}
