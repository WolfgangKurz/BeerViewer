using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	public class ConditionRejuvenatedEventArgs : EventArgs
	{
		public string FleetName { get; }
		public int MinCondition { get; }

		public ConditionRejuvenatedEventArgs(string fleetName, int minCondtion)
		{
			this.FleetName = fleetName;
			this.MinCondition = minCondtion;
		}
	}
}
