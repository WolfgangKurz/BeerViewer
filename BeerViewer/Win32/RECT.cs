using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace BeerViewer.Win32
{
	[StructLayout(LayoutKind.Sequential)]
	internal class RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;
	}
}
