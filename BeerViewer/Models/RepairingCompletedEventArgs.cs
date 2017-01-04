using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	public class RepairingCompletedEventArgs : EventArgs
	{
		public int DockId { get; private set; }
		public Ship Ship { get; private set; }

		public RepairingCompletedEventArgs(int id, Ship ship)
		{
			this.DockId = id;
			this.Ship = ship;
		}
	}
}
