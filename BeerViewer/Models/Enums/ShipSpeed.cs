namespace BeerViewer.Models.Enums
{
	public enum ShipSpeed
	{
		Immovable = 0,
		Slow = 5,
		Fast = 10,
		Faster = 15,
		Fastest = 20,
	}

	public static class ShipSpeedExtension
	{
		public static string ToLanguageString(this ShipSpeed speed)
		{
			switch (speed)
			{
				case ShipSpeed.Slow:
					return "slow";
				case ShipSpeed.Fast:
					return "fast";
				case ShipSpeed.Faster:
					return "faster";
				case ShipSpeed.Fastest:
					return "fastest";

				default:
					return "immovable";
			}
		}
	}
}
