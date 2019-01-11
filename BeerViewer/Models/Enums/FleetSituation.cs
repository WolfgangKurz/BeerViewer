using System;

namespace BeerViewer.Models.Enums
{
	[Flags]
	public enum FleetSituation : int
	{
		Empty = 0,
		Homeport = 1,
		Combined = 1 << 1,
		Sortie = 1 << 2,
		Expedition = 1 << 3,
		HeavilyDamaged = 1 << 4,
		InShortSupply = 1 << 5,
		Repairing = 1 << 6,
		FlagshipIsRepairShip = 1 << 7,
		Rejuvenating = 1 << 8
	}
}
