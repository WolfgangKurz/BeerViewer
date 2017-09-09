using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Drawing;

namespace BeerViewer.Framework
{
	public partial class BorderlessWindow : Form
	{
		#region Window processing
		protected override void WndProc(ref Message m)
		{
			if (DesignMode)
			{
				base.WndProc(ref m);
				return;
			}

			switch ((WindowMessages)m.Msg)
			{
				case WindowMessages.WM_NCHITTEST:
					m.Result = (IntPtr)NCHitTest(m);
					return;

				case WindowMessages.WM_NCCALCSIZE:
					NCCalcSize(ref m);
					return;

				case WindowMessages.WM_DWMCOMPOSITIONCHANGED:
					handleCompositionChanged();
					return;

					/*
				case WindowMessages.WM_NCPAINT:
					break;
					*/

				case WindowMessages.WM_WINDOWPOSCHANGED:
					// Need to call this method (Resize event fired from here){
					typeof(Form)
						.GetMethod("UpdateWindowState", BindingFlags.NonPublic | BindingFlags.Instance)
						?.Invoke(this, new object[] { });

					// base.DefWndProc(ref m);
					// Call Control.WmWindowPosChanged
					{
						var args = new object[] { m };
						var method = typeof(Control).GetMethod("WmWindowPosChanged", BindingFlags.NonPublic | BindingFlags.Instance);
						method?.Invoke(this, args);
					}

					// And RestoreWindowBoundsIfNecessary method must never called
					return;

				case WindowMessages.WM_GESTURENOTIFY:
					this.SetupGesture(this.MeasureGesturable(ref m));
					break;

				case WindowMessages.WM_GESTURE:
					this.DecodeGesture(ref m);
					return;

				case WindowMessages.WM_TABLET_QUERYSYSTEMGESTURESTATUS:
					// Disable "Press & Hold" right click
					m.Result = new IntPtr((int)TABLET_DISABLE_FLAGS.TABLET_DISABLE_PRESSANDHOLD);
					return;

				case WindowMessages.WM_WININICHANGE:
					// Tablet-mode changed
					{
						var msg = Marshal.PtrToStringUni(m.LParam);
						if (
							(msg == "ConvertibleSlateMode") // Win8.1 or earlier
							|| (msg == "UserInteractionMode") // Win10 or later
						)
						{
							FrameworkExtension.SetTabletMode(!FrameworkExtension.IsTabletMode(true));
							this.OnResize(EventArgs.Empty);
						}
					}
					break;
			}

			base.WndProc(ref m);
		}

		private bool CompositionEnabled = false;
		private void handleCompositionChanged()
		{
			try
			{
				int enabled = 0;
				FrameworkHelper.DwmIsCompositionEnabled(ref enabled);
				CompositionEnabled = (enabled == 1);

				if (CompositionEnabled)
				{
					// SetWindowTheme(this.Handle, string.Empty, string.Empty);

					uint v = (uint)DWMNCRENDERINGPOLICY.DWMNCRP_ENABLED;
					FrameworkHelper.DwmSetWindowAttribute(
						this.Handle,
						DWMWINDOWATTRIBUTE.DWMWA_NCRENDERING_POLICY,
						ref v,
						sizeof(uint) // sizeof(DWORD)
					);

					MARGINS margins = new MARGINS()
					{
						leftWidth = 1,
						rightWidth = 1,
						topHeight = 1,
						bottomHeight = 1
					};
					FrameworkHelper.DwmExtendFrameIntoClientArea(this.Handle, ref margins);
				}
			}
			catch { }
		}

		private void NCCalcSize(ref Message m)
		{
			if (m.WParam != IntPtr.Zero)
			{
				var screen = Screen.FromHandle(m.HWnd);
				var nccsp = (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(m.LParam, typeof(NCCALCSIZE_PARAMS));

				if (
					(screen.WorkingArea.Left == nccsp.rgrc0.left + 8)
					&& (screen.WorkingArea.Top == nccsp.rgrc0.top + 8)
					&& (screen.WorkingArea.Right == nccsp.rgrc0.right - 8)
					&& (screen.WorkingArea.Bottom == nccsp.rgrc0.bottom - 8)
				)
				{
					nccsp.rgrc0.left += 8 - 2;
					nccsp.rgrc0.top += 8 - 2;
					nccsp.rgrc0.bottom += -8 + 2;
					nccsp.rgrc0.right += 2;
				}

				nccsp.rgrc0.bottom++;

				Marshal.StructureToPtr(nccsp, m.LParam, true);
			}

			m.Result = IntPtr.Zero;
		}
		private HitTestValue NCHitTest(Message m)
		{
			RECT rc;
			FrameworkHelper.GetWindowRect(m.HWnd, out rc);

			int w = rc.right - rc.left;
			int h = rc.bottom - rc.top;

			Point pt = this.PointToClient(new Point(m.LParam.ToInt32()));

			if (this.WindowState != FormWindowState.Maximized)
			{
				int frame_size = FrameworkHelper.GetSystemMetrics(SystemMetric.SM_CXFRAME)
					+ FrameworkHelper.GetSystemMetrics(SystemMetric.SM_CXPADDEDBORDER);

				int diagonal_width = (frame_size * 2)
					+ FrameworkHelper.GetSystemMetrics(SystemMetric.SM_CXBORDER);

				if (pt.Y < frame_size)
				{
					if (pt.X < diagonal_width) return HitTestValue.HTTOPLEFT;
					if (pt.X >= w - diagonal_width) return HitTestValue.HTTOPRIGHT;
					return HitTestValue.HTTOP;
				}

				if (pt.Y >= h - frame_size)
				{
					if (pt.X < diagonal_width) return HitTestValue.HTBOTTOMLEFT;
					if (pt.X >= w - diagonal_width) return HitTestValue.HTBOTTOMRIGHT;
					return HitTestValue.HTBOTTOM;
				}

				if (pt.X < frame_size) return HitTestValue.HTLEFT;
				if (pt.X >= w - frame_size) return HitTestValue.HTRIGHT;
			}

			if (pt.Y < 28)
			{
				var cSize = this.CaptionSize(w, h);

				if (cSize.Contains(pt)) return HitTestValue.HTCAPTION;
				else return HitTestValue.HTCLIENT;
			}
			return HitTestValue.HTCLIENT;
		}

		protected override CreateParams CreateParams
		{
			get
			{
				var b = base.CreateParams;
				if (!DesignMode)
					b.ExStyle |= (int)(WindowStyleEx.WS_EX_LAYERED | WindowStyleEx.WS_EX_APPWINDOW);
				return b;
			}
		}
		#endregion

		protected virtual Rectangle CaptionSize(int Width, int Height)
			=> new Rectangle(0, 0, Width - 32 * (FrameworkExtension.IsTabletMode() ? 2 : 3), 28);
		public new Size ClientSize
		{
			get
			{
				var sz = base.ClientSize;
				sz.Width -= 2;
				sz.Height -= 3;

				if (this.WindowState == FormWindowState.Maximized)
				{
					sz.Width -= 8;
					sz.Height -= 8;
				}
				return sz;
			}
			set
			{
				base.ClientSize = value;
			}
		}

		protected FrameworkRenderer Renderer { get; private set; }
		private Brush BackColorBrush;

		public FrameworkControl CloseButton { get; } = null;
		public FrameworkControl MaximizeButton { get; } = null;
		public FrameworkControl MinimizeButton { get; } = null;

		public BorderlessWindow() : this(false) { }
		public BorderlessWindow(bool IsDialog = false)
		{
			FrameworkHelper.SetLayeredWindowAttributes(this.Handle, 0x000000, 255, 0x02); // LWA_ALPHAKEY
			handleCompositionChanged();

			Renderer = new Framework.FrameworkRenderer(this);

			this.DoubleBuffered = true;

			this.BackColorChanged += (s, e) =>
			{
				this.BackColorBrush?.Dispose();
				this.BackColorBrush = new SolidBrush(this.BackColor);
			};
			this.BackColor = Constants.colorNormalFace;

			if (!IsDialog)
			{
				#region System Buttons
				this.CloseButton = new FrameworkControl(0, 1, 32, 28);
				this.MaximizeButton = new FrameworkControl(0, 1, 32, 28);
				this.MinimizeButton = new FrameworkControl(0, 1, 32, 28);

				CloseButton.Paint += (s, e) =>
				{
					var c = s as FrameworkControl;
					var g = e.Graphics;

					if (c.IsHover) g.FillRectangle(c.IsActive ? Constants.brushActiveFace : Constants.brushHoverFace, c.ClientBound);
					g.DrawImage(
						Properties.Resources.System_Buttons,
						new Rectangle(0, 0, 32, 28),
						new Rectangle(64, (this.WindowState == FormWindowState.Maximized ? 28 : 0), 32, 28),
						GraphicsUnit.Pixel
					);
				};
				MaximizeButton.Paint += (s, e) =>
				{
					var c = s as FrameworkControl;
					var g = e.Graphics;

					if (c.IsHover) g.FillRectangle(c.IsActive ? Constants.brushActiveFace : Constants.brushHoverFace, c.ClientBound);
					g.DrawImage(
						Properties.Resources.System_Buttons,
						new Rectangle(0, 0, 32, 28),
						new Rectangle(32, (this.WindowState == FormWindowState.Maximized ? 28 : 0), 32, 28),
						GraphicsUnit.Pixel
					);
				};
				MinimizeButton.Paint += (s, e) =>
				{
					var c = s as FrameworkControl;
					var g = e.Graphics;

					if (c.IsHover) g.FillRectangle(c.IsActive ? Constants.brushActiveFace : Constants.brushHoverFace, c.ClientBound);
					g.DrawImage(
						Properties.Resources.System_Buttons,
						new Rectangle(0, 0, 32, 28),
						new Rectangle(0, (this.WindowState == FormWindowState.Maximized ? 28 : 0), 32, 28),
						GraphicsUnit.Pixel
					);
				};

				CloseButton.Click += (s, e) => this.Close();
				MaximizeButton.Click += (s, e) => this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
				MinimizeButton.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

				this.Renderer.AddControl(CloseButton);
				this.Renderer.AddControl(MaximizeButton);
				this.Renderer.AddControl(MinimizeButton);
				#endregion

				this.Resize += (s, e) =>
				{
					var w = this.ClientSize.Width;

					if (FrameworkExtension.IsTabletMode())
					{
						CloseButton.X = w - 32;
						MinimizeButton.X = w - 64;

						MaximizeButton.Visible = false;
					}
					else
					{
						CloseButton.X = w - 32;
						MaximizeButton.X = w - 64;
						MinimizeButton.X = w - 96;

						MaximizeButton.Visible = true;
					}

					this.Invalidate();
				};
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			this.Invalidate();
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;

			g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;

			g.FillRectangle(
				this.BackColorBrush,
				new Rectangle(new Point(1, 1), this.ClientSize)
			);

			g.TranslateTransform(1, 1);
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

			var sz = this.ClientSize;
			g.SetClip(new Rectangle(1, 1, sz.Width, sz.Height));

			base.OnPaint(e);
		}

		public event ScrollEventHandler TouchPanEvent;
		public void OnTouchPanEvent(ScrollEventArgs e)
		{
			this.Renderer.OnScroll(this.containerGestureTarget, e);
			this.TouchPanEvent?.Invoke(this, e);
		}

		private FrameworkContainer containerGestureTarget;
		private Point pointGestureBase = new Point();
		private void DecodeGesture(ref Message m)
		{
			var gi = new GESTUREINFO();
			gi.cbSize = (uint)Marshal.SizeOf(typeof(GESTUREINFO));

			if (!FrameworkHelper.GetGestureInfo(m.LParam, ref gi))
				return;

			Point pt;

			switch (gi.dwID)
			{
				case GESTUREID.GID_BEGIN:
				case GESTUREID.GID_END:
					break;

				// Use only PAN gesture
				case GESTUREID.GID_PAN:
					pt = this.PointToClient(new Point(gi.ptsLocation.x, gi.ptsLocation.y));

					switch (gi.dwFlags)
					{
						case GESTUREFLAG.GF_BEGIN:
							this.pointGestureBase = pt;
							this.containerGestureTarget = this.Renderer.PeekContainer(pt);
							break;

						default:
							if (pt == pointGestureBase) break;

							pointGestureBase = pt;
							this.OnTouchPanEvent(
								new ScrollEventArgs(
									pt.X - this.pointGestureBase.X,
									pt.Y - this.pointGestureBase.Y
								)
							);
							break;
					}
					break;
			}
		}

		private bool[] MeasureGesturable(ref Message m)
		{
			var gns = (GESTURENOTIFYSTRUCT)Marshal.PtrToStructure(m.LParam, typeof(GESTURENOTIFYSTRUCT));
			var pt = new Point(gns.ptsLocation.x, gns.ptsLocation.y);

			var ctr = this.Renderer.PeekContainer(pt);
			if (ctr == null) return null;

			return new bool[] {
				ctr.IsScrollVisibleX,
				ctr.IsScrollVisibleY
			};
		}
		private void SetupGesture(bool[] bGesturable)
		{
			var listGestureConfig = new List<GESTURECONFIG>();

			if (bGesturable != null)
			{
				GESTURECONFIGFLAG f = 0;
				if (bGesturable[0]) f |= GESTURECONFIGFLAG.GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY;
				if (bGesturable[1]) f |= GESTURECONFIGFLAG.GC_PAN_WITH_SINGLE_FINGER_VERTICALLY;

				listGestureConfig.Add(
					new GESTURECONFIG
					{
						dwID = GESTUREID.GID_PAN,
						dwWant = f,
						dwBlock = 0
					}
				);
			}
			else
			{
				listGestureConfig.Add(
					new GESTURECONFIG
					{
						dwID = GESTUREID.GID_NONE,
						dwWant = 0,
						dwBlock = 0
					}
				);
			}

			var arrayGestureConfig = listGestureConfig.ToArray();
			FrameworkHelper.SetGestureConfig(
				this.Handle,
				0,
				(uint)arrayGestureConfig.Length,
				arrayGestureConfig,
				(uint)Marshal.SizeOf(typeof(GESTURECONFIG))
			);
		}
	}
}
