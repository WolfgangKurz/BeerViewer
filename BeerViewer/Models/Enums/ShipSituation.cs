using System;

namespace BeerViewer.Models.Enums
{
	[Flags]
	public enum ShipSituation : int
	{
		None = 0,
		Repair = 1,
		Evacuation = 1 << 1,
		Tow = 1 << 2,
		HeavilyDamaged = 1 << 3,
		DamageControlled = 1 << 4,
	}
}
