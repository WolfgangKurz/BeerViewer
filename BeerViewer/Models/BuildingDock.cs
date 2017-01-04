using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	public class BuildingDock : TimerNotifier, IIdentifiable
	{
		private bool notificated;

		#region Id 프로퍼티
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

		#region State 프로퍼티
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
				}
			}
		}
		#endregion

		#region Ship 프로퍼티
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

		public event EventHandler<BuildingCompletedEventArgs> Completed;

		internal BuildingDock(kcsapi_kdock rawData)
		{
			this.Update(rawData);
		}

		internal void Update(kcsapi_kdock rawData)
		{
			this.Id = rawData.api_id;
			this.State = (BuildingDockState)rawData.api_state;
			this.Ship = this.State == BuildingDockState.Building || this.State == BuildingDockState.Completed
				? DataStorage.Instance.Master.Ships[rawData.api_created_ship_id]
				: null;
			this.CompleteTime = this.State == BuildingDockState.Building
				? (DateTimeOffset?)Const.UnixEpoch.AddMilliseconds(rawData.api_complete_time)
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
				var remaining = this.CompleteTime.Value.Subtract(DateTimeOffset.Now);
				if (remaining.Ticks < 0) remaining = TimeSpan.Zero;

				this.Remaining = remaining;

				if (!this.notificated && this.Completed != null && remaining.Ticks <= 0)
				{
					this.Completed(this, new BuildingCompletedEventArgs(this.Id, this.Ship));
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
