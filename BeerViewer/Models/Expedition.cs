using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	public class Expedition : TimerNotifier, IIdentifiable
	{
		public static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

		private readonly Fleet fleet;
		private bool notificated;

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

		#region Mission 프로퍼티
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

		#region ReturnTime / Remaining / IsInExecution 프로퍼티
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
				this.Mission = DataStorage.Instance.Master.Missions[this.Id];
				this.ReturnTime = Const.UnixEpoch.AddMilliseconds(rawData[2]);
				this.UpdateCore();
			}
		}

		private void UpdateCore()
		{
			this.RaisePropertyChanged(nameof(this.Remaining));

			if (!this.notificated && this.Returned != null && this.Remaining <= TimeSpan.FromSeconds(Const.NotificationTime))
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
