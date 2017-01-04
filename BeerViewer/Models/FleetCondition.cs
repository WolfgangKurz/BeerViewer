using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	public class FleetCondition : TimerNotifier
	{
		private Ship[] ships;
		private bool notificated;
		private int minCondition;

		public bool IsEnabled { get; set; }
		public string Name { get; set; }

		#region RejuvenateTime / IsRejuvenating 프로퍼티
		private DateTimeOffset? _RejuvenateTime;
		public DateTimeOffset? RejuvenateTime
		{
			get { return this._RejuvenateTime; }
			private set
			{
				if (this._RejuvenateTime != value)
				{
					this._RejuvenateTime = value;
					this.notificated = false;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(this.IsRejuvenating));
				}
			}
		}

		public bool IsRejuvenating => this.RejuvenateTime.HasValue;
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

		public event EventHandler<ConditionRejuvenatedEventArgs> Rejuvenated;

		internal void Update(Ship[] s)
		{
			this.ships = s;

			if (this.ships.Length == 0)
			{
				this.RejuvenateTime = null;
				return;
			}

			var condition = this.ships.Min(x => x.Condition);
			if (condition != this.minCondition)
			{
				this.minCondition = condition;

				var rejuvnate = DateTimeOffset.Now; // 회복 완료 시간

				while (condition < 49)
				{
					rejuvnate = rejuvnate.AddMinutes(3);
					condition += 3;
					if (condition > 49) condition = 49;
				}

				this.RejuvenateTime = rejuvnate <= DateTimeOffset.Now
					? (DateTimeOffset?)null
					: rejuvnate;
			}
		}

		protected override void Tick()
		{
			base.Tick();

			if (this.RejuvenateTime.HasValue && this.IsEnabled)
			{
				var remaining = this.RejuvenateTime.Value.Subtract(DateTimeOffset.Now);
				if (remaining.Ticks < 0) remaining = TimeSpan.Zero;

				this.Remaining = remaining;

				if (!this.notificated && this.Rejuvenated != null && remaining.Ticks <= 0)
				{
					this.Rejuvenated(this, new ConditionRejuvenatedEventArgs(this.Name, 0));
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
