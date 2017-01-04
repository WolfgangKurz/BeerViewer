using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	public enum FleetSpeed
	{
		Fast,
		Low,
		Hybrid,
	}

	public static class FleetSpeedExtension
	{
		public static string ToStateString(this FleetSpeed speed)
		{
			switch (speed)
			{
				case FleetSpeed.Fast:
					return "고속함대";
				case FleetSpeed.Low:
					return "저속함대";
				default:
					return "속도혼성함대";
			}
		}
	}
}
