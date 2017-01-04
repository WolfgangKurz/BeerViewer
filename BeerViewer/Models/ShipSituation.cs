using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	[Flags]
	public enum ShipSituation
	{
		None = 0,
		Repair = 1,
		Evacuation = 1 << 1,
		Tow = 1 << 2,
		HeavilyDamaged = 1 << 3,
		DamageControlled = 1 << 4,
	}
}
