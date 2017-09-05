using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

using BeerViewer.Network;
using BeerViewer.Models.Enums;
using BeerViewer.Models.Raw;
using BeerViewer.Models.kcsapi;

namespace BeerViewer.Models
{
	public class Fleet : TimerNotifier, IIdentifiable
	{
		private readonly Homeport homeport;
		private Ship[] originalShips;

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

		#region Name Property
		private string _Name;
		public string Name
		{
			get { return this._Name; }
			internal set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region Ships Property
		private Ship[] _Ships = new Ship[0];
		public Ship[] Ships
		{
			get { return this._Ships; }
			private set
			{
				if (this._Ships != value)
				{
					this._Ships = value;
					this.RaisePropertyChanged();
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object ShipsUpdated { get; set; }
		#endregion

		#region FleetSpeed Property
		public ShipSpeed Speed
			=> this.Ships?.Select(x => x.Speed).Min()
				?? ShipSpeed.Immovable;
		#endregion

		#region IsInSortie Property
		private bool _IsInSortie;
		public bool IsInSortie
		{
			get { return this._IsInSortie; }
			set
			{
				if (this._IsInSortie != value)
				{
					this._IsInSortie = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public Expedition Expedition { get; }


		private int MinimumCondition;
		private DateTimeOffset? RejuvenateTime { get; set; }

		public TimeSpan? RejuvenateRemaining
			=> !this.RejuvenateTime.HasValue ? (TimeSpan?)null
			: this.RejuvenateTime.Value < DateTimeOffset.Now ? TimeSpan.Zero
			: this.RejuvenateTime.Value - DateTimeOffset.Now;

		public string RejuvenateText => this.RejuvenateRemaining.HasValue
			? $"{(int)this.RejuvenateRemaining.Value.TotalHours:D2}:{this.RejuvenateRemaining.Value.ToString(@"mm\:ss")}"
			: "--:--:--";

		internal Fleet(Homeport parent, kcsapi_deck Data)
		{
			this.homeport = parent;
			this.CompositeDisposable.Add(this.Expedition = new Expedition(this));

			this.Update(Data);
		}

		internal void Update(kcsapi_deck Data)
		{
			this.Id = Data.api_id;
			this.Name = Data.api_name;

			this.Expedition.Update(Data.api_mission);
			this.UpdateShips(Data.api_ship.Select(id => this.homeport.Organization.Ships[id]).ToArray());

			this.UpdateCondition();
		}

		#region Sortie, Homing
		internal void Sortie()
		{
			if (!this.IsInSortie)
				this.IsInSortie = true;
		}
		internal void Homing()
		{
			if (this.IsInSortie)
				this.IsInSortie = false;
		}
		#endregion

		#region Change, Unset
		internal Ship Change(int index, Ship ship)
		{
			var current = this.originalShips[index];

			List<Ship> list;
			if (index == -1)
			{
				list = this.originalShips.Take(1).ToList();
			}
			else
			{
				list = this.originalShips.ToList();
				list[index] = ship;
				list.RemoveAll(x => x == null);
			}

			var ships = new Ship[this.originalShips.Length];
			Array.Copy(list.ToArray(), ships, list.Count);

			this.UpdateShips(ships);

			return current;
		}

		internal void Unset(int index)
		{
			var list = this.originalShips.ToList();
			list[index] = null;
			list.RemoveAll(x => x == null);

			var ships = new Ship[this.originalShips.Length];
			Array.Copy(list.ToArray(), ships, list.Count);

			this.UpdateShips(ships);
		}

		internal void UnsetAll()
		{
			var list = this.originalShips.Take(1).ToList();
			var ships = new Ship[this.originalShips.Length];
			Array.Copy(list.ToArray(), ships, list.Count);

			this.UpdateShips(ships);
		}

		#endregion

		private void UpdateShips(Ship[] ships)
		{
			this.originalShips = ships;
			this.Ships = ships.Where(x => x != null).ToArray();
		}
		private void UpdateCondition()
		{
			var condition = this.Ships.Min(x => x.Condition);
			var goal = 49;

			// Require time not changed
			if (MinimumCondition == condition)
				return;

			MinimumCondition = condition;
			if (condition >= goal)
			{
				this.RejuvenateTime = (DateTimeOffset?)null;
			}
			else
			{
				var rejuvenate = DateTimeOffset.Now;

				var value = (goal - condition + 2) / 3 * 3; // Integral dividing
				rejuvenate = rejuvenate.AddMinutes(value);

				this.RejuvenateTime = rejuvenate;
			}
		}

		protected override void Tick()
		{
			base.Tick();

			if (this.RejuvenateTime.HasValue)
			{
				this.RaisePropertyChanged(nameof(RejuvenateRemaining));
				this.RaisePropertyChanged(nameof(RejuvenateText));
			}
		}

		internal void RaiseShipsUpdated()
		{
			this.RaisePropertyChanged(nameof(this.ShipsUpdated));
		}

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Name}\", Ships = {string.Join(",", this.Ships.Select(s => "\"" + s.Info.Name + "\""))}";
		}
	}
}
