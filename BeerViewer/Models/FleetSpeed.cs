using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	public enum FleetSpeed
	{
		SuperFast,
		FastPlus,
		Fast,
		Slow,

		Hybrid_Slow, // 저속 혼성
		Hybrid_Fast, // 고속 혼성
		Hybrid_FastPlus, // 고속+ 혼성
	}


	public static class FleetSpeedExtension
	{
		public static bool IsFast(this FleetSpeed speed)
		{
			return (speed == FleetSpeed.Slow || speed == FleetSpeed.Hybrid_Slow)
				? false : true;
		}

		public static string ToStateString(this FleetSpeed speed)
		{
			switch (speed)
			{
				case FleetSpeed.SuperFast:
					return "초고속함대";
				case FleetSpeed.FastPlus:
					return "고속+함대";
				case FleetSpeed.Fast:
					return "고속함대";
				case FleetSpeed.Slow:
					return "저속함대";

				case FleetSpeed.Hybrid_FastPlus:
					return "고속+혼성함대";
				case FleetSpeed.Hybrid_Fast:
					return "고속혼성함대";
				case FleetSpeed.Hybrid_Slow:
					return "저속혼성함대";

				default:
					return "알수없음";
			}
		}
	}
}
