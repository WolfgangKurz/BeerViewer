using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Framework
{
	public delegate void ScrollEventHandler(object sender, ScrollEventArgs e);
	public class ScrollEventArgs : EventArgs
	{
		public ScrollEventArgs(int deltaX, int deltaY)
		{
			this.DeltaX = deltaX;
			this.DeltaY = deltaY;
		}

		public int DeltaX { get; }
		public int DeltaY { get; }
	}
}
