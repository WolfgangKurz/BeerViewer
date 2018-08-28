using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace BeerViewer.Framework
{
	public sealed class FrameworkRenderer : IDisposable
	{
		public IReadOnlyCollection<FrameworkControl> Controls => this._Controls;
		private List<FrameworkControl> _Controls { get; set; } = new List<FrameworkControl>();

		private Control Owner { get; set; }

		private MouseEventHandler OwnerMouseMoveEvent = null;
		private MouseEventHandler OwnerMouseDownEvent = null;
		private MouseEventHandler OwnerMouseUpEvent = null;
		private MouseEventHandler OwnerMouseWheelEvent = null;
		private EventHandler OwnerMouseLeaveEvent = null;
		private PaintEventHandler OwnerPaintEvent = null;

		private EventHandler ControlInvalidate = null;

		public FrameworkRenderer(Control Owner)
		{
			this.Owner = Owner;

			#region Initialize event handlers
			this.ControlInvalidate = (s, e) =>
			{
				var control = s as FrameworkControl;
				if (control == null || !control.Visible) return;

				this.Owner.Invalidate(control.Bound);
			};

			this.OwnerMouseMoveEvent = (s, e) =>
			{
				var pt = new Point(e.X, e.Y);
				foreach (var control in this.Controls)
				{
					var cpt = new Point(pt.X - control.X, pt.Y - control.Y);
					if (control.OnMouseMove(cpt))
					{
						this.Controls.Where(x => x != control)
							.ToList().ForEach(x => x.OnMouseLeave());
						break;
					}
				}
			};
			this.OwnerMouseDownEvent = (s, e) =>
			{
				var pt = new Point(e.X, e.Y);
				foreach (var control in this.Controls)
				{
					var cpt = new Point(pt.X - control.X, pt.Y - control.Y);
					if (control.OnMouseDown(cpt)) break;
				}
			};
			this.OwnerMouseUpEvent = (s, e) =>
			{
				var pt = new Point(e.X, e.Y);
				foreach (var control in this.Controls)
				{
					var cpt = new Point(pt.X - control.X, pt.Y - control.Y);
					if (control.OnMouseUp(cpt)) break;
				}
			};
			this.OwnerMouseWheelEvent = (s, e) =>
			{
				var pt = new Point(e.X, e.Y);
				foreach (var control in this.Controls)
				{
					var cpt = new Point(pt.X - control.X, pt.Y - control.Y);
					if (control.OnMouseWheel(cpt, e.Delta)) break;
				}
			};
			this.OwnerPaintEvent = (s, e) =>
			{
				var g = e.Graphics;
				g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
				foreach (var control in this.Controls)
				{
					var _s = g.Save();
					g.IntersectClip(control.Bound);

					control.OnPaint(g);

					g.Restore(_s);
				}
			};
			this.OwnerMouseLeaveEvent = (s, e) =>
			{
				foreach (var control in this.Controls)
					control.OnMouseLeave();
			};
			#endregion

			#region Register event handlers
			this.Owner.MouseMove += this.OwnerMouseMoveEvent;
			this.Owner.MouseDown += this.OwnerMouseDownEvent;
			this.Owner.MouseUp += this.OwnerMouseUpEvent;
			this.Owner.MouseLeave += this.OwnerMouseLeaveEvent;
			this.Owner.MouseWheel += this.OwnerMouseWheelEvent;
			this.Owner.Paint += this.OwnerPaintEvent;
			#endregion
		}

		/// <summary>
		/// Add control to renderer
		/// </summary>
		/// <param name="Control">Control to add</param>
		public void AddControl(FrameworkControl Control)
		{
			Control.SetRenderer(this);
			Control.Invalidated += this.ControlInvalidate;
			this._Controls.Add(Control);

			Control.Invalidate();
		}

		public void Dispose()
		{
			#region Unregister event handlers
			this.Owner.MouseMove -= this.OwnerMouseMoveEvent;
			this.Owner.MouseDown -= this.OwnerMouseDownEvent;
			this.Owner.MouseUp -= this.OwnerMouseUpEvent;
			this.Owner.Paint -= this.OwnerPaintEvent;
			#endregion

			#region Unregister invalidate event handler
			foreach (var control in this.Controls)
				control.Invalidated -= ControlInvalidate;
			#endregion

			this._Controls = null;
		}

		internal void _SetCapture() => FrameworkHelper.SetCapture(this.Owner.Handle);
		internal void _ReleaseCapture() => FrameworkHelper.ReleaseCapture();

		internal void Invalidate(Rectangle rect)
			=> this.Owner?.Invalidate(rect);

		internal FrameworkContainer PeekContainer(Point pt)
		{
			FrameworkContainer output = null;

			output = this.Controls
				.Where(x => typeof(FrameworkContainer).IsAssignableFrom(x.GetType()))
				.Where(x => x.Visible && x.Bound.Contains(pt))
				.Select(x => x as FrameworkContainer)
				.LastOrDefault();

			if (output == null) return null;

			var bPt = pt;
			while (true)
			{
				var pPt = new Point(bPt.X, bPt.Y);
				pPt.Offset(-output.X, -output.Y);

				var childContainer = this.Controls
					.Where(x => typeof(FrameworkContainer).IsAssignableFrom(x.GetType()))
					.Where(x => x.Visible && x.Bound.Contains(pPt))
					.Select(x => x as FrameworkContainer)
					.LastOrDefault();

				if (childContainer != null) // Child container exists
				{
					bPt = pPt;
					output = childContainer;
					continue;
				}

				var childControl = output.Controls
					.Where(x => x.Visible && x.Bound.Contains(pPt))
					.LastOrDefault();

				if (childControl?.Focusable ?? false) return null;
				return output;
			}
		}

		internal void OnScroll(FrameworkContainer containerGestureTarget, ScrollEventArgs e)
		{
			if (containerGestureTarget == null) return;
			containerGestureTarget.OnScroll(-e.DeltaX, -e.DeltaY);
		}
	}

	internal static class FrameworkControlExtension
	{
		public static void InvalidateRaw(this FrameworkContainer control)
			=> control.Renderer.Invalidate(control.Bound);

		public static void InvalidateRaw(this FrameworkControl control)
		{
			if (control.IsChild)
				control.Parent.Invalidate();
			else if (control.Renderer != null)
				control.Renderer.Invalidate(control.Bound);
		}
	}
}
