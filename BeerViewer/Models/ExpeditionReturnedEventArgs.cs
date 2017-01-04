using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
