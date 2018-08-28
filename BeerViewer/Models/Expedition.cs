using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Network;
using BeerViewer.Models;
using BeerViewer.Models.Raw;
using BeerViewer.Models.Wrapper;

namespace BeerViewer.Models
{
	public class ExpeditionReturnedEventArgs : EventArgs
	{
		public string FleetName { get; }

		internal ExpeditionReturnedEventArgs(string fleetName)
		{
			this.FleetName = fleetName;
		}
	}

	public class Expedition : TimerNotifier, IIdentifiable
	{
		private readonly Fleet fleet;
		private bool notificated;

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

		#region Mission Property
		private Mission _Mission;
		public Mission Mission
		{
			get { return this._Mission; }
			private set
			{
				if (this._Mission != value)
				{
					this._Mission = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region ReturnTime / Remaining / IsInExecution Property
		private DateTimeOffset? _ReturnTime;
		public DateTimeOffset? ReturnTime
		{
			get { return this._ReturnTime; }
			private set
			{
				if (this._ReturnTime != value)
				{
					this._ReturnTime = value;
					this.notificated = false;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(this.Remaining));
					this.RaisePropertyChanged(nameof(this.IsInExecution));
				}
			}
		}

		public TimeSpan? Remaining
			=> !this.ReturnTime.HasValue ? (TimeSpan?)null
			: this.ReturnTime.Value < DateTimeOffset.Now ? TimeSpan.Zero
			: this.ReturnTime.Value - DateTimeOffset.Now;

		public string RemainingText => this.Remaining.HasValue
			? $"{(int)this.Remaining.Value.TotalHours:D2}:{this.Remaining.Value.ToString(@"mm\:ss")}"
			: "--:--:--";

		public bool IsInExecution => this.ReturnTime.HasValue;
		#endregion

		public LimitedValue Progress
		{
			get
			{
				if (!this.ReturnTime.HasValue) return new LimitedValue();
				if (this.Mission == null) return new LimitedValue();

				var start = this.ReturnTime.Value.Subtract(TimeSpan.FromMinutes(this.Mission.RawData.api_time));
				var value = (int)DateTimeOffset.Now.Subtract(start).TotalSeconds;
				return new LimitedValue(value, this.Mission.RawData.api_time * 60, 0);
			}
		}

		public event EventHandler<ExpeditionReturnedEventArgs> Returned;

		public Expedition(Fleet fleet)
		{
			this.fleet = fleet;
		}

		internal void Update(long[] Data)
		{
			if (Data.Length != 4 || Data.All(x => x == 0))
			{
				this.Id = -1;
				this.Mission = null;
				this.ReturnTime = null;
			}
			else
			{
				this.Id = (int)Data[1];
				this.Mission = Master.Instance.Missions[this.Id];
				this.ReturnTime = Extensions.UnixEpoch.AddMilliseconds(Data[2]);
				this.UpdateCore();
			}
		}

		private void UpdateCore()
		{
			this.RaisePropertyChanged(nameof(this.Remaining));
			this.RaisePropertyChanged(nameof(this.RemainingText));
			this.RaisePropertyChanged(nameof(this.Progress));

			if (!this.notificated && this.Returned != null && this.Remaining <= TimeSpan.FromSeconds(Settings.NotificationTime.Value))
			{
				this.Returned(this, new ExpeditionReturnedEventArgs(this.fleet.Name));
				this.notificated = true;
			}
		}

		protected override void Tick()
		{
			base.Tick();
			this.UpdateCore();
		}
	}
}
