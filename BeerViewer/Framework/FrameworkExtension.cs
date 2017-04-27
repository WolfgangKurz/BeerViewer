using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BeerViewer.Framework
{
	internal static class FrameworkExtension
	{
		internal static Color FromRgb(int rgb)
			=> Color.FromArgb(0xff, (rgb & 0xff0000) >> 16, (rgb & 0xff00) >> 8, rgb & 0xff);
	}
}
