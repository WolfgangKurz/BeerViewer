using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;

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
					if (control.OnMouseMove(pt))
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
					if (control.OnMouseDown(pt)) break;
			};
			this.OwnerMouseUpEvent = (s, e) =>
			{
				var pt = new Point(e.X, e.Y);
				foreach (var control in this.Controls)
					if (control.OnMouseUp(pt)) break;
			};
			this.OwnerPaintEvent = (s, e) =>
			{
				foreach (var control in this.Controls)
					control.OnPaint(e.Graphics);
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
			this.Owner.Paint += this.OwnerPaintEvent;
			#endregion
		}

		/// <summary>
		/// Add control to renderer
		/// </summary>
		/// <param name="Control">Control to add</param>
		public void AddControl(FrameworkControl Control)
		{
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
	}
}
