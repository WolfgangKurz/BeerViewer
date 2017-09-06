using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BeerViewer.Framework
{
	public class FrameworkContainer : FrameworkControl, IDisposable
	{
		public IReadOnlyCollection<FrameworkControl> Controls => this._Controls;
		private List<FrameworkControl> _Controls { get; set; } = new List<FrameworkControl>();

		private EventHandler ControlInvalidate = null;

		#region Scrollable Property
		private bool _Scrollable = true;
		public bool Scrollable
		{
			get { return this._Scrollable; }
			set
			{
				if (this._Scrollable != value)
				{
					this._Scrollable = value;
					if (!value) // When scroll disabled, reset scroll position
						this._ScrollX = this._ScrollY = 0;

					this.Invalidate();
				}
			}
		}
		#endregion
		#region ScrollX Property
		private int _ScrollX;
		public int ScrollX
		{
			get { return this._ScrollX; }
			set
			{
				if (this._ScrollX != value)
				{
					this._ScrollX = value;
					this.Invalidate();
				}
			}
		}
		#endregion
		#region ScrollY Property
		private int _ScrollY;
		public int ScrollY
		{
			get { return this._ScrollY; }
			set
			{
				if (this._ScrollY != value)
				{
					this._ScrollY = value;
					this.Invalidate();
				}
			}
		}
		#endregion

		/// <summary>
		/// Client width without scroll size
		/// </summary>
		public int ClientWidth => this.IsScrollVisibleY ? this.Width - 16 : this.Width;

		/// <summary>
		/// Client height without scroll size
		/// </summary>
		public int ClientHeight => this.IsScrollVisibleX ? this.Height - 16 : this.Height;

		/// <summary>
		/// Will container displays Horizontal scroll?
		/// </summary>
		public bool IsScrollVisibleX
		{
			get
			{
				if (!this.Scrollable) return false;

				var sz = this.GetClientSize();
				return sz.Width > this.Width;
			}
		}

		/// <summary>
		/// Will container displays Vertical scroll?
		/// </summary>
		public bool IsScrollVisibleY
		{
			get
			{
				if (!this.Scrollable) return false;

				var sz = this.GetClientSize();
				return sz.Height > this.Height;
			}
		}

		#region Initializers
		public FrameworkContainer() : base()
			=> this.Initialize();

		public FrameworkContainer(FrameworkRenderer Renderer) : base(Renderer)
			=> this.Initialize();

		public FrameworkContainer(int X, int Y) : base(X, Y)
			=> this.Initialize();

		public FrameworkContainer(FrameworkRenderer Renderer, int X, int Y) : base(Renderer, X, Y)
			=> this.Initialize();

		public FrameworkContainer(int X, int Y, int Width, int Height) : base(X, Y, Width, Height)
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
					if (!control.Visible) continue;

					if (g.Clip.IsVisible(control.Bound))
					{
						var state = g.Save();

						g.TranslateTransform(control.X, control.Y);

						g.IntersectClip(control.ClientBound);
						g.IntersectClip(new Rectangle(-control.X, -control.Y, this.ClientWidth, this.ClientHeight));

						if (this.Scrollable)
							g.TranslateTransform(-this.ScrollX, -this.ScrollY);

						control.Update(g);

						g.Restore(state);
					}
				}

				if (this.Scrollable) this.DrawScroll(g);
			};
		}
		private void DrawScroll(Graphics g)
		{
			var scX = this.IsScrollVisibleX;
			var scY = this.IsScrollVisibleY;
			if (!scX && !scY) return;

			const int MarginSize = 5;
			Func<int, int, int> CalcThumbSize = (viewportSize, contentSize) =>
			{
				var viewableRatio = (double)viewportSize / contentSize;
				var thumbHeight = (viewportSize - MarginSize) * viewableRatio;
				return (int)thumbHeight;
			};

			var cSz = this.GetClientSize();
			var cx = this.ClientWidth;
			var cy = this.ClientHeight;

			var state = g.Save();
			g.SmoothingMode = SmoothingMode.AntiAlias;
			if (scX)
			{
				var thumb = CalcThumbSize(cy, cSz.Height);
				var bound = new Rectangle(
					MarginSize + (int)(this.ScrollX * (cSz.Width - cx) / (cx - thumb)),
					cy + MarginSize,
					thumb, 16 - (MarginSize * 2)
				);
				g.FillRoundedRectangle(bound, 3, Constants.brushActiveFace);
			}
			if (scY)
			{
				var thumb = CalcThumbSize(cy, cSz.Height);
				var bound = new Rectangle(
					cx + MarginSize,
					MarginSize + (int)(this.ScrollY * (cSz.Height - cy) / (cy - thumb)),
					16 - (MarginSize * 2), thumb
				);
				g.FillRoundedRectangle(bound, 3, Constants.brushActiveFace);
			}
			g.Restore(state);
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
		/// Get client size includes child bounds
		/// </summary>
		/// <returns></returns>
		public Size GetClientSize()
		{
			var sz = new Rectangle(
				0, 0,
				this.Width, this.Height
			);
			foreach (var control in this.Controls)
				sz = Rectangle.Union(sz, control.Bound);

			return new Size(sz.Width, sz.Height);
		}

		/// <summary>
		/// Add control to renderer
		/// </summary>
		/// <param name="Control">Control to add</param>
		public void AddControl(FrameworkControl Control)
		{
			Control.Invalidated += this.ControlInvalidate;
			Control.Parent = this;
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
