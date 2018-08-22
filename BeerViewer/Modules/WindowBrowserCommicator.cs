using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeerViewer.Modules
{
	internal class WindowBrowserCommicator
	{
		internal const int WM_NCLBUTTONDOWN = 0xA1;
		internal const int HTCAPTION = 0x2;

		[DllImport("User32.dll")]
		internal static extern bool ReleaseCapture();
		[DllImport("User32.dll")]
		internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		private Form Owner { get; }
		private IntPtr OwnerHandle { get; }

		internal WindowBrowserCommicator(Form Owner)
		{
			this.Owner = Owner;
			this.OwnerHandle = Owner?.Handle ?? IntPtr.Zero;
		}

		public void requestWindowMove()
		{
			ReleaseCapture();
			SendMessage(this.OwnerHandle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
		}
	}
}
