using BeerViewer.Models.Raw;
using BeerViewer.Models;

namespace BeerViewer.Models.Wrapper
{
	public class ShipSlot : Notifier
	{
		public ShipInfo Owner { get; }
		public SlotItem Item { get; }

		public int Maximum { get; private set; }
		public int Lost { get; private set; }

		public bool IsAircraft => this.Item.Info.Type.IsNumerable();

		public string Tooltip => this.Item.Info.ToolTipData;
		public bool Equipped => this.Item != null && this.Item != SlotItem.Empty;

		#region Current Property
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
			this.Owner = owner.Info ?? ShipInfo.Empty;
			this.Item = item ?? SlotItem.Empty;

			this.Maximum = maximum;
			this.Current = current;
			this.Lost = Maximum - Current;
		}
	}
}
