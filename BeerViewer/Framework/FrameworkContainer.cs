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
				var v = value.InRange(0, this.ScrollMaximumX);
				if (!Scrollable || !IsScrollVisibleX) v = 0; // Scroll not available

				if (this._ScrollX != v)
				{
					this._ScrollX = v;
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
				var v = value.InRange(0, this._ScrollMaximumY);
				if (!Scrollable || !IsScrollVisibleY) v = 0; // Scroll not available

				if (this._ScrollY != v)
				{
					this._ScrollY = v;
					this.Invalidate();
				}
			}
		}
		#endregion

		#region ScrollMaximumX Property
		private int _ScrollMaximumX;
		private int ScrollMaximumX {
			get { return this._ScrollMaximumX; }
			set
			{
				if (this._ScrollMaximumX != value)
				{
					this._ScrollMaximumX = value;
					if (this.ScrollX > value) this.ScrollX = value;
				}
			}
		}
		#endregion
		#region ScrollMaximumY Property
		private int _ScrollMaximumY;
		private int ScrollMaximumY {
			get { return this._ScrollMaximumY; }
			set
			{
				if (this._ScrollMaximumY != value)
				{
					this._ScrollMaximumY = value;
					if (this.ScrollX > value) this.ScrollX = value;
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
				this.ScrollMaximumX = Math.Max(0, sz.Width - this.Width);
				return this.ScrollMaximumX > 0;
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
				this.ScrollMaximumY = Math.Max(0, sz.Height - this.Height);
				return this.ScrollMaximumY > 0;
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

			this.Resize += (s, e) =>
			{
				this.ScrollX = this.ScrollX; // Recalculate scrolls
				this.ScrollY = this.ScrollY;
			};

			this.Paint += (s, e) =>
			{
				var g = e.Graphics;

				foreach (var control in this.Controls)
				{
					if (!control.Visible) continue;

					var before = g.Save();
					g.TranslateTransform(-this.ScrollX, -this.ScrollY);

					if (g.Clip.IsVisible(control.Bound))
					{
						g.TranslateTransform(control.X, control.Y);

						g.IntersectClip(control.ClientBound);
						g.IntersectClip(new Rectangle(-control.X + this.ScrollX, -control.Y + this.ScrollY, this.ClientWidth, this.ClientHeight));

						control.Update(g);
					}

					g.Restore(before);
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
				var thumbHeight = (viewportSize - MarginSize * 2) * viewableRatio;
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
					MarginSize + (int)(this.ScrollX * (cx - thumb - (MarginSize * 2)) / (cSz.Width - cx)),
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
					MarginSize + (int)(this.ScrollY * (cy - thumb - (MarginSize * 2)) / (cSz.Height - cy)),
					16 - (MarginSize * 2), thumb
				);
				g.FillRoundedRectangle(bound, 3, Constants.brushActiveFace);
			}
			g.Restore(state);
		}

		public override bool OnMouseDown(Point pt)
		{
			pt.X += this.ScrollX;
			pt.Y += this.ScrollY;

			return base.OnMouseDown(pt);
		}
		public override bool OnMouseMove(Point pt)
		{
			pt.X += this.ScrollX;
			pt.Y += this.ScrollY;

			return base.OnMouseMove(pt);
		}
		public override bool OnMouseUp(Point pt)
		{
			pt.X += this.ScrollX;
			pt.Y += this.ScrollY;

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
		public override bool OnMouseWheel(Point pt, int delta)
		{
			pt.X += this.ScrollX;
			pt.Y += this.ScrollY;

			this.ScrollY -= delta;
			return base.OnMouseWheel(pt, delta);
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
				if(control.Visible)
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
