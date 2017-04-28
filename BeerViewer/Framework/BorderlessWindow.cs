using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace BeerViewer.Framework
{
	public partial class BorderlessWindow : Form
	{
		#region Const Values
		private const uint WM_NCPAINT = 0x85;
		private const uint WM_NCCALCSIZE = 0x83;
		private const uint WM_NCHITTEST = 0x84;

		private const int HTCLIENT = 0x01;
		private const int HTCAPTION = 0x02;
		private const int HTSYSMENU = 0x03;
		private const int HTGROWBOX = 0x04;
		private const int HTMENU = 0x05;
		private const int HTHSCROLL = 0x06;
		private const int HTVSCROLL = 0x07;
		private const int HTMINBUTTON = 0x08;
		private const int HTMAXBUTTON = 0x09;
		private const int HTLEFT = 0x0A;
		private const int HTRIGHT = 0x0B;
		private const int HTTOP = 0x0C;
		private const int HTTOPLEFT = 0x0D;
		private const int HTTOPRIGHT = 0x0E;
		private const int HTBOTTOM = 0x0F;
		private const int HTBOTTOMLEFT = 0x10;
		private const int HTBOTTOMRIGHT = 0x11;
		private const int HTBORDER = 0x12;
		private const int HTOBJECT = 0x13;
		private const int HTCLOSE = 0x14;
		private const int HTHELP = 0x15;
		#endregion

		#region Structures
		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int left, top, right, bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct WINDOWPOS
		{
			public IntPtr hwnd;
			public IntPtr hwndinsertafter;
			public int x, y, cx, cy;
			public int flags;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct NCCALCSIZE_PARAMS
		{
			public RECT rgrc0, rgrc1, rgrc2;
			public WINDOWPOS lppos;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct MARGINS
		{
			public int leftWidth;
			public int rightWidth;
			public int topHeight;
			public int bottomHeight;
		}
		#endregion

		#region WinAPI
		[DllImport("uxtheme", CharSet = CharSet.Unicode)]
		private static extern Int32 SetWindowTheme(IntPtr hWnd, String subAppName, String subIdList);

		[DllImport("user32.dll", ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

		[DllImport("dwmapi.dll")]
		private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

		[DllImport("dwmapi.dll")]
		private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

		[DllImport("dwmapi.dll")]
		private static extern int DwmIsCompositionEnabled(ref int pfEnabled);
		#endregion

		protected override void WndProc(ref Message m)
		{
			if (DesignMode)
				base.WndProc(ref m);

			switch ((uint)m.Msg)
			{
				case WM_NCHITTEST:
					m.Result = (IntPtr)NCHitTest(m);
					break;
				case WM_NCCALCSIZE:
					NCCalcSize(ref m);
					break;
				case WM_NCPAINT:
					if (Environment.OSVersion.Version.Major >= 6)
					{
						int enabled = 0;
						DwmIsCompositionEnabled(ref enabled);

						if (enabled == 1)
						{
							int v = 2;
							DwmSetWindowAttribute(this.Handle, 2, ref v, 4);

							MARGINS margins = new MARGINS()
							{
								bottomHeight = 1,
								leftWidth = 1,
								rightWidth = 1,
								topHeight = 1
							};
							DwmExtendFrameIntoClientArea(this.Handle, ref margins);
						}
					}
					base.WndProc(ref m);
					break;
				default:
					base.WndProc(ref m);
					break;
			}
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
		private int NCHitTest(Message m)
		{
			RECT rc;
			GetWindowRect(m.HWnd, out rc);

			int w = rc.right - rc.left;
			int h = rc.bottom - rc.top;

			/*
			int x = m.LParam.ToInt32() & 0x0000FFFF;
			int y = (int)((m.LParam.ToInt32() & 0xFFFF0000) >> 16);
			if ((x & 0x8000) != 0) x = -(x ^ 0x8000 + 1);
			if ((y & 0x8000) != 0) y = -(y ^ 0x8000 + 1);

			Point pt = this.PointToClient(new Point(x, y));
			*/
			Point pt = this.PointToClient(new Point(m.LParam.ToInt32()));

			if(this.WindowState != FormWindowState.Maximized)
			{
				if (pt.X < 4 && pt.Y < 4) return HTTOPLEFT;
				else if (pt.X >= w - 4 && pt.Y < 4) return HTTOPRIGHT;
				else if (pt.X < 4 && pt.Y >= h - 4) return HTBOTTOMLEFT;
				else if (pt.X >= w - 4 && pt.Y >= h - 4) return HTBOTTOMRIGHT;

				else if (pt.X < 4) return HTLEFT;
				else if (pt.X >= w - 4) return HTRIGHT;
				else if (pt.Y < 4) return HTTOP;
				else if (pt.Y >= h - 4) return HTBOTTOM;
			}

			if (pt.Y < 28)
			{
				var cSize = this.CaptionSize(w, h);

				if (cSize.Contains(pt)) return HTCAPTION;
				else return HTCLIENT;
			}
			return HTCLIENT;
		}

		protected virtual Rectangle CaptionSize(int Width, int Height)
			=> new Rectangle(0, 0, Width - 96, 28);

		protected FrameworkRenderer Renderer { get; private set; }

		public BorderlessWindow() : this(false) { }
		public BorderlessWindow(bool IsDialog = false)
		{
			Renderer = new Framework.FrameworkRenderer(this);

			this.BackColor = FrameworkExtension.FromRgb(0x222225);
			this.DoubleBuffered = true;

			if (!IsDialog)
			{
				#region System Buttons
				var CloseButton = new FrameworkControl(0, 1, 32, 28);
				var MaximizeButton = new FrameworkControl(0, 1, 32, 28);
				var MinimizeButton = new FrameworkControl(0, 1, 32, 28);

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
					CloseButton.X = w - 32;
					MaximizeButton.X = w - 64;
					MinimizeButton.X = w - 96;

					this.Invalidate();
				};
			}

			this.Paint += (s, e) =>
			{
				e.Graphics.TranslateTransform(1, 1);
				e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			};
		}

		public new Size ClientSize
		{
			get
			{
				var sz = new Size(base.ClientSize.Width - 1, base.ClientSize.Height - 1);
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
	}
}
