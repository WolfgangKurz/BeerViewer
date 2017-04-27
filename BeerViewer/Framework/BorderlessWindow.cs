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
					nccsp.rgrc0.bottom += -8 + 2 - 1;
					nccsp.rgrc0.top = -2;
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

			int x = m.LParam.ToInt32() & 0x0000FFFF;
			int y = (int)((m.LParam.ToInt32() & 0xFFFF0000) >> 16);
			Point pt = this.PointToClient(new Point(x, y));

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

		public BorderlessWindow() : this(false) { }

		public BorderlessWindow(bool IsDialog = false)
		{
			this.DoubleBuffered = true;

			this.Resize += (s, e) => this.Invalidate();

			if (!IsDialog)
			{
				var RenderInvalidate = new Action(() =>
				{
					var w = this.ClientSize.Width;
					this.Invalidate(new Rectangle(w - 96, 0, 96, 28));
				});
				Point ptMouseDown = Point.Empty;

				this.MouseMove += (s, e) => RenderInvalidate();
				this.MouseLeave += (s, e) => RenderInvalidate();

				this.MouseDown += (s, e) =>
				{
					ptMouseDown = e.Location;
					RenderInvalidate();
				};
				this.MouseUp += (s, e) =>
				{
					RenderInvalidate();
					ProcSystemButton(ptMouseDown, e.Location);
				};
			}

			this.Paint += (s, e) =>
			{
				e.Graphics.TranslateTransform(1, 1);
				e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

				if (!IsDialog)
					this.RenderSystemButtons(e.Graphics);
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


		protected Rectangle CloseButtonRectangle => new Rectangle(this.ClientSize.Width - 32, 0, 32, 28);
		protected Rectangle MaximizeButtonRectangle => new Rectangle(this.ClientSize.Width - 64, 0, 32, 28);
		protected Rectangle RestoreButtonRectangle => this.MaximizeButtonRectangle;
		protected Rectangle MinimizeButtonRectangle => new Rectangle(this.ClientSize.Width - 96, 0, 32, 28);

		private void RenderSystemButtons(Graphics g)
		{
			var w = this.ClientSize.Width;
			int focus = -1;
			bool active = false;

			var pt = this.PointToClient(Cursor.Position);
			if (pt.Y >= 0 && pt.Y < 28)
			{
				if (CloseButtonRectangle.Contains(pt)) focus = 2;
				else if (MaximizeButtonRectangle.Contains(pt)) focus = 1;
				else if (MinimizeButtonRectangle.Contains(pt)) focus = 0;

				active = (MouseButtons == MouseButtons.Left);
			}

			var hoverBrush = new SolidBrush(FrameworkExtension.FromRgb(0x313131));
			var activeBrush = new SolidBrush(FrameworkExtension.FromRgb(0x575757));

			if (focus >= 0)
				g.FillRectangle(active ? activeBrush : hoverBrush, new Rectangle(w - 96 + (focus * 32), 0, 32, 28));

			g.DrawImage(
				Properties.Resources.System_Buttons,
				new Rectangle(w - 96, 0, 96, 28),
				new Rectangle(0, (this.WindowState == FormWindowState.Maximized ? 28 : 0), 96, 28),
				GraphicsUnit.Pixel
			);
		}
		private void ProcSystemButton(Point ptDown, Point ptUp)
		{
			if (CloseButtonRectangle.Contains(ptDown) && CloseButtonRectangle.Contains(ptUp))
				this.Close();

			else if (MaximizeButtonRectangle.Contains(ptDown) && MaximizeButtonRectangle.Contains(ptUp))
				this.WindowState = (this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized);

			else if (MinimizeButtonRectangle.Contains(ptDown) && MinimizeButtonRectangle.Contains(ptUp))
				this.WindowState = FormWindowState.Minimized;
		}
	}
}
