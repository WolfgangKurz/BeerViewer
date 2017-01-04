using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BeerViewer.Models
{
	public class SallyArea
	{
		private static Color[] SallyAreaColorList { get; } = new Color[]
		{
			Color.Transparent,
			Color.FromArgb(0x3B,0x8A,0x30),
			Color.FromArgb(0xAB,0x6E,0x35),
			Color.FromArgb(0xD2,0xAE,0xAE),
			Color.FromArgb(0x40,0xA0,0x13),
			Color.FromArgb(0x78,0x18,0xD8)
		};
		private static string[] SallyAreaNameList { get; }
			= new string[] {"", "1분류", "2분류", "3분류", "4분류", "5분류" };


		public static Color SallyAreaColor(int api_sally_area)
			=> SallyAreaColorList[api_sally_area];

		public static string SallyAreaName(int api_sally_area)
			=> SallyAreaNameList[api_sally_area];
	}
}
