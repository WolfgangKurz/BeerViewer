using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	/// <summary>
	/// 둘 이상의 함대가 연합된 연합함대
	/// </summary>
	public class CombinedFleet : DisposableNotifier
	{
		#region Name 프로퍼티
		private string _Name;
		public string Name
		{
			get { return this._Name; }
			set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Fleets 프로퍼티
		private Fleet[] _Fleets;
		public Fleet[] Fleets
		{
			get { return this._Fleets; }
			set
			{
				if (this._Fleets != value)
				{
					this._Fleets = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public FleetState State { get; }

		public CombinedFleet(Homeport parent, params Fleet[] fleets)
		{
			if (fleets == null || fleets.Length == 0) throw new ArgumentException();

			this.Fleets = fleets;
			this.State = new FleetState(parent, fleets);
			this.CompositeDisposable.Add(this.State);

			foreach (var fleet in fleets)
			{
				this.CompositeDisposable.Add(new PropertyChangedEventListener(fleet)
				{
					{ nameof(Fleet.Name), (sender, args) => this.UpdateName() },
				});

				var source = fleet;
				this.CompositeDisposable.Add(new WeakEventListener<EventHandler, EventArgs>(
					h => new EventHandler(h),
					h => source.State.Updated += h,
					h => source.State.Updated -= h,
					(sender, args) => this.State.Update()));
				this.CompositeDisposable.Add(new WeakEventListener<EventHandler, EventArgs>(
					h => new EventHandler(h),
					h => source.State.Calculated += h,
					h => source.State.Calculated -= h,
					(sender, args) => this.State.Calculate()));
			}

			this.UpdateName();

			this.State.Calculate();
			this.State.Update();
		}

		private void UpdateName()
		{
			this.Name = string.Join(
				Environment.NewLine,
				this.Fleets.Select(x => x.Name)
			);
		}
	}
}
