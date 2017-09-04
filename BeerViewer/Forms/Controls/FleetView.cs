using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Models;

namespace BeerViewer.Forms.Controls
{
	internal class FleetView : FrameworkControl
	{
		public Fleet Fleet { get; protected set; }

		#region Initializers
		public FleetView() : base()
		{
			this.Initialize();
		}
		public FleetView(int X, int Y) : base(X, Y)
		{
			this.Initialize();
		}
		public FleetView(int X, int Y, int Width, int Height) : base(X, Y, Width, Height)
		{
			this.Initialize();
		}
		#endregion

		public void Initialize()
		{
			this.Paint += (s, e) =>
			{
				var g = e.Graphics;
			};
		}

		public void SetFleet(Fleet Fleet)
		{
			this.Fleet = Fleet;
			this.Invalidate();
		}
	}
}
