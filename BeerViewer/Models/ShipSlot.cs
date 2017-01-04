using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Models.Raw;

namespace BeerViewer.Models
{
	public class ShipSlot : Notifier
	{
		public ShipInfo Owner { get; }
		public SlotItem Item { get; }

		public int Maximum { get; private set; }
		public int Lost { get; private set; }
		public bool IsAirplane => this.Item.Info.Type.IsNumerable();

		public string Tooltip => this.Item.Info.Id == 0 ? null : this.Item.NameWithLevel;
		public bool Equipped => this.Item != null && this.Item != SlotItem.Dummy;

		#region Current 프로퍼티
		private int _Current;
		public int Current
		{
			get { return this._Current; }
			set
			{
				if (this._Current != value)
				{
					this._Current = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public ShipSlot(Ship owner, SlotItem item, int maximum, int current)
		{
			this.Owner = owner.Info ?? ShipInfo.Dummy;
			this.Item = item ?? SlotItem.Dummy;

			this.Maximum = maximum;
			this.Current = current;
			this.Lost = Maximum - Current;
		}
	}
}
