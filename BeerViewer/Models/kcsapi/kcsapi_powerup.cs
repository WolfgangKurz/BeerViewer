﻿namespace BeerViewer.Models.kcsapi
{
	public class kcsapi_powerup
	{
		public int api_powerup_flag { get; set; }
		public kcsapi_ship2 api_ship { get; set; }
		public kcsapi_deck[] api_deck { get; set; }
	}
}
