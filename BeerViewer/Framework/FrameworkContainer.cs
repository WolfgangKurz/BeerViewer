using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace BeerViewer.Framework
{
	public class FrameworkContainer : FrameworkControl, IDisposable
	{
		public IReadOnlyCollection<FrameworkControl> Controls => this._Controls;
		private List<FrameworkControl> _Controls { get; set; } = new List<FrameworkControl>();

		private EventHandler ControlInvalidate = null;

		#region Initializers
		public FrameworkContainer() : base()
			=> this.Initialize();

		public FrameworkContainer(FrameworkRenderer Renderer) : base(Renderer)
			=> this.Initialize();

		public FrameworkContainer(int X, int Y) : base( X, Y)
			=> this.Initialize();

		public FrameworkContainer(FrameworkRenderer Renderer, int X, int Y) : base(Renderer, X, Y)
			=> this.Initialize();

		public FrameworkContainer(int X, int Y, int Width, int Height) : base( X, Y, Width, Height)
			=> this.Initialize();

		public FrameworkContainer(FrameworkRenderer Renderer, int X, int Y, int Width, int Height) : base(Renderer, X, Y, Width, Height)
			=> this.Initialize();
		#endregion

		public void Initialize()
		{
			this.ControlInvalidate = (s, e) =>
			{
				var control = s as FrameworkControl;
				if (control == null || !control.Visible) return;

				this.Invalidate();
			};

			this.MouseMove += (s, e) =>
			{
				var pt = new Point(e.X, e.Y);
				if (this.Controls.Any(_ => _.IsActive))
				{
					var control = this.Controls.First(_ => _.IsActive);
					var cpt = new Point(pt.X - control.X, pt.Y - control.Y);

					control.OnMouseMove(cpt);
				}
				else
				{
					foreach (var control in this.Controls)
					{
						var cpt = new Point(pt.X - control.X, pt.Y - control.Y);
						if (control.OnMouseMove(cpt))
						{
							this.Controls.Where(x => x != control)
								.ToList().ForEach(x => x.OnMouseLeave());

							control.OnMouseMove(cpt);
							break;
						}
					}
				}
			};
			this.MouseDown += (s, e) =>
			{
				var pt = new Point(e.X, e.Y);
				foreach (var control in this.Controls)
				{
					var cpt = new Point(pt.X - control.X, pt.Y - control.Y);
					if (control.OnMouseDown(cpt)) break;
				}
			};
			this.MouseUp += (s, e) =>
			{
				var pt = new Point(e.X, e.Y);
				if (this.Controls.Any(_ => _.IsActive))
				{
					var control = this.Controls.First(_ => _.IsActive);
					var cpt = new Point(pt.X - control.X, pt.Y - control.Y);

					control.OnMouseUp(cpt);
				}
				else
				{
					foreach (var control in this.Controls)
					{
						var cpt = new Point(pt.X - control.X, pt.Y - control.Y);
						if (control.OnMouseUp(cpt)) break;
					}
				}
			};
			this.MouseLeave += (s, e) =>
			{
				foreach (var control in this.Controls)
					control.OnMouseLeave();
			};

			this.Paint += (s, e) =>
			{
				var g = e.Graphics;

				foreach (var control in this.Controls)
				{
					if (!control.Visible) return;

					var state = g.Save();

					g.TranslateTransform(control.X, control.Y);
					g.Clip = new Region(control.ClientBound);
					g.IntersectClip(new Rectangle(-control.X, -control.Y, this.ClientBound.Width, this.ClientBound.Height));

					control.Update(g);

					g.Restore(state);
				}
			};
		}

		public override bool OnMouseUp(Point pt)
		{
			if (base.OnMouseUp(pt))
			{
				foreach (var control in this.Controls)
				{
					var cpt = new Point(pt.X - control.X, pt.Y - control.Y);
					if (control.OnMouseUp(cpt))
						return true;
				}
				return true;
			}
			return false;
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

		public override void Dispose()
		{
			#region Unregister invalidate event handler
			foreach (var control in this.Controls)
			{
				control.Invalidated -= this.ControlInvalidate;
				control.Dispose();
			}
			#endregion

			this._Controls = null;
		}
	}
}
