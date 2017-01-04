using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.BattleInfo
{
	public enum AirSupremacy
	{
		항공전없음 = -1,
		제공권동등 = 0,   // Air parity
		제공권확보 = 1,  // Air supremacy
		항공우세 = 2,   // Air superiority
		항공열세 = 3,   // Air denial
		제공권상실 = 4,  // Air incapability
	}
}
