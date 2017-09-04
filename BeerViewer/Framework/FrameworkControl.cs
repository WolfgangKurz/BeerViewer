using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace BeerViewer.Framework
{
	public class FrameworkControl : IDisposable
	{
		#region X
		/// <summary>
		/// X position of control
		/// </summary>
		public int X
		{
			get { return this._X; }
			set
			{
				if (this._X != value)
				{
					this._X = value;
					this.Invalidate();
				}
			}
		}
		private int _X { get; set; }
		#endregion

		#region Y
		/// <summary>
		/// Y position of control
		/// </summary>
		public int Y
		{
			get { return this._Y; }
			set
			{
				if (this._Y != value)
				{
					this._Y = value;
					this.Invalidate();
				}
			}
		}
		private int _Y { get; set; }
		#endregion

		#region Width
		/// <summary>
		/// Width size of control
		/// </summary>
		public int Width
		{
			get { return this._Width; }
			set
			{
				if (this._Width != value)
				{
					this._Width = value;
					this.Resize?.Invoke(this, EventArgs.Empty);
					this.Invalidate();
				}
			}
		}
		private int _Width { get; set; }
		#endregion

		#region Height
		/// <summary>
		/// Height size of control
		/// </summary>
		public int Height
		{
			get { return this._Height; }
			set
			{
				if (this._Height != value)
				{
					this._Height = value;
					this.Resize?.Invoke(this, EventArgs.Empty);
					this.Invalidate();
				}
			}
		}
		private int _Height { get; set; }
		#endregion

		#region Visible
		/// <summary>
		/// Visible of control
		/// </summary>
		public bool Visible
		{
			get { return this._Visible; }
			set
			{
				if (this._Visible != value)
				{
					this._Visible = value;
					this.Invalidate();
				}
			}
		}
		private bool _Visible { get; set; }
		#endregion


		#region IsActive
		/// <summary>
		/// Returns is mouse active(click) on control
		/// </summary>
		public bool IsActive
		{
			get { return this._IsActive; }
			protected set
			{
				if (this._IsActive != value)
				{
					this._IsActive = value;
					this.Invalidate();
				}
			}
		}
		private bool _IsActive { get; set; }
		#endregion

		#region IsHover
		/// <summary>
		/// Returns is mouse hover on control
		/// </summary>
		public bool IsHover
		{
			get { return this._IsHover; }
			protected set
			{
				if (this._IsHover != value)
				{
					this._IsHover = value;
					this.Invalidate();
				}
			}
		}
		private bool _IsHover { get; set; }
		#endregion

		public FrameworkRenderer Renderer { get; private set; }

		public FrameworkContainer Parent { get; set; }
		public bool IsChild => this.Parent != null;


		/// <summary>
		/// Bound rectangle of control
		/// </summary>
		public Rectangle Bound => new Rectangle(this.X, this.Y, this.Width, this.Height);

		/// <summary>
		/// Bound rectangle without position of control
		/// </summary>
		public Rectangle ClientBound => new Rectangle(0, 0, this.Width, this.Height);

		/// <summary>
		/// <paramref name="pt"/> is in bound of control?
		/// </summary>
		/// <param name="pt">Point to check</param>
		/// <returns>True if contains, False if not.</returns>
		public bool Contains(Point pt) => this.Bound.Contains(pt);

		#region Events
		public event EventHandler Invalidated;

		public event MouseEventHandler MouseMove;
		public event MouseEventHandler MouseDown;
		public event MouseEventHandler MouseUp;
		public event EventHandler MouseLeave;
		public event EventHandler Click;

		public event EventHandler Resize;
		public event PaintEventHandler Paint;
		#endregion

		#region Initializers
		public FrameworkControl(FrameworkRenderer Renderer)
		{
			this.Renderer = Renderer;
			this._X = this._Y = 0;
			this._Width = 100;
			this._Height = 32;
			this.Visible = true;
		}
		public FrameworkControl() : this(null) { }

		public FrameworkControl(FrameworkRenderer Renderer, int X, int Y) : this(Renderer)
		{
			this._X = X;
			this._Y = Y;
		}
		public FrameworkControl(int X, int Y) : this(null, X, Y) { }

		public FrameworkControl(FrameworkRenderer Renderer, int X, int Y, int Width, int Height) : this(Renderer, X, Y)
		{
			this._Width = Width;
			this._Height = Height;
		}
		public FrameworkControl(int X, int Y, int Width, int Height) : this(null, X, Y, Width, Height) { }
		#endregion

		public virtual bool OnMouseMove(Point pt)
		{
			if (this.Visible && (this.IsActive || this.ClientBound.Contains(pt)))
			{
				this.IsHover = true;

				this.MouseMove?.Invoke(this, new MouseEventArgs(IsActive ? MouseButtons.Left : MouseButtons.None, 0, pt.X, pt.Y, 0));
				return true;
			}
			else if (this.IsHover) this.OnMouseLeave();

			return false;
		}
		public virtual bool OnMouseDown(Point pt)
		{
			if (this.Visible && this.ClientBound.Contains(pt))
			{
				this.Renderer?._SetCapture();
				this.IsActive = true;

				this.MouseDown?.Invoke(this, new MouseEventArgs(MouseButtons.Left, 0, pt.X, pt.Y, 0));
				return true;
			}
			return false;
		}
		public virtual bool OnMouseUp(Point pt)
		{
			bool prev = this.IsActive;

			this.IsActive = false;
			this.Renderer?._ReleaseCapture();

			if (this.Visible && (this.ClientBound.Contains(pt) || prev))
			{
				this.MouseUp?.Invoke(this, new MouseEventArgs(MouseButtons.None, 0, pt.X, pt.Y, 0));

				if (prev) this.Click?.Invoke(this, EventArgs.Empty);
				return true;
			}
			return false;
		}
		public virtual void OnMouseLeave()
		{
			if (!this.IsHover) return;

			this.IsHover = false;
			this.MouseLeave?.Invoke(this, EventArgs.Empty);
		}

		public virtual void OnPaint(Graphics g)
		{
			if (!this.Visible) return;

			if (g.Clip.IsVisible(this.Bound))
			{
				var state = g.Save();

				g.TranslateTransform(this.X, this.Y);
				g.Clip = new Region(this.ClientBound);
				this.Update(g);

				g.Restore(state);
			}
		}

		/// <summary>
		/// Notice need to render
		/// </summary>
		public void Invalidate()
		{
			this.InvalidateRaw();
			this.Invalidated?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Paint control
		/// </summary>
		public void Update(Graphics g)
		{
			this.Paint?.Invoke(this, new PaintEventArgs(g, this.Bound));
		}

		/// <summary>
		/// Dispose allocated resources
		/// </summary>
		public virtual void Dispose()
		{
		}

		public void SetRenderer(FrameworkRenderer Renderer)
			=> this.Renderer = Renderer;
	}
}
