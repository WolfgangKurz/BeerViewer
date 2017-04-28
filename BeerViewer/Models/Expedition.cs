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
		public static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

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

		public bool IsInExecution => this.ReturnTime.HasValue;
		#endregion

		public event EventHandler<ExpeditionReturnedEventArgs> Returned;

		public Expedition(Fleet fleet)
		{
			this.fleet = fleet;
		}

		internal void Update(long[] rawData)
		{
			if (rawData.Length != 4 || rawData.All(x => x == 0))
			{
				this.Id = -1;
				this.Mission = null;
				this.ReturnTime = null;
			}
			else
			{
				this.Id = (int)rawData[1];
				this.Mission = Master.Instance.Missions[this.Id];
				this.ReturnTime = UnixEpoch.AddMilliseconds(rawData[2]);
				this.UpdateCore();
			}
		}

		private void UpdateCore()
		{
			this.RaisePropertyChanged(nameof(this.Remaining));

			if (!this.notificated && this.Returned != null && this.Remaining <= TimeSpan.FromSeconds(Configuration.NotificationTime))
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
