using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
