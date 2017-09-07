using System;
using System.Runtime.InteropServices;

namespace BeerViewer.Framework
{
	#region Enums
	internal enum DWMWINDOWATTRIBUTE : uint
	{
		DWMWA_NCRENDERING_ENABLED = 1,
		DWMWA_NCRENDERING_POLICY,
		DWMWA_TRANSITIONS_FORCEDISABLED,
		DWMWA_ALLOW_NCPAINT,
		DWMWA_CAPTION_BUTTON_BOUNDS,
		DWMWA_NONCLIENT_RTL_LAYOUT,
		DWMWA_FORCE_ICONIC_REPRESENTATION,
		DWMWA_FLIP3D_POLICY,
		DWMWA_EXTENDED_FRAME_BOUNDS,
		DWMWA_HAS_ICONIC_BITMAP,
		DWMWA_DISALLOW_PEEK,
		DWMWA_EXCLUDED_FROM_PEEK,
		DWMWA_CLOAK,
		DWMWA_CLOAKED,
		DWMWA_FREEZE_REPRESENTATION,
		DWMWA_LAST
	}

	internal enum DWMNCRENDERINGPOLICY : uint
	{
		DWMNCRP_USEWINDOWSTYLE,
		DWMNCRP_DISABLED,
		DWMNCRP_ENABLED,
		DWMNCRP_LAST
	}

	internal enum SystemMetric : int
	{
		SM_ARRANGE = 56,
		SM_CLEANBOOT = 67,
		SM_CMONITORS = 80,
		SM_CMOUSEBUTTONS = 43,
		SM_CONVERTIBLESLATEMODE = 0x2003,
		SM_CXBORDER = 5,
		SM_CXCURSOR = 13,
		SM_CXDLGFRAME = 7,
		SM_CXDOUBLECLK = 36,
		SM_CXDRAG = 68,
		SM_CXEDGE = 45,
		SM_CXFIXEDFRAME = 7,
		SM_CXFOCUSBORDER = 83,
		SM_CXFRAME = 32,
		SM_CXFULLSCREEN = 16,
		SM_CXHSCROLL = 21,
		SM_CXHTHUMB = 10,
		SM_CXICON = 11,
		SM_CXICONSPACING = 38,
		SM_CXMAXIMIZED = 61,
		SM_CXMAXTRACK = 59,
		SM_CXMENUCHECK = 71,
		SM_CXMENUSIZE = 54,
		SM_CXMIN = 28,
		SM_CXMINIMIZED = 57,
		SM_CXMINSPACING = 47,
		SM_CXMINTRACK = 34,
		SM_CXPADDEDBORDER = 92,
		SM_CXSCREEN = 0,
		SM_CXSIZE = 30,
		SM_CXSIZEFRAME = 32,
		SM_CXSMICON = 49,
		SM_CXSMSIZE = 52,
		SM_CXVIRTUALSCREEN = 78,
		SM_CXVSCROLL = 2,
		SM_CYBORDER = 6,
		SM_CYCAPTION = 4,
		SM_CYCURSOR = 14,
		SM_CYDLGFRAME = 8,
		SM_CYDOUBLECLK = 37,
		SM_CYDRAG = 69,
		SM_CYEDGE = 46,
		SM_CYFIXEDFRAME = 8,
		SM_CYFOCUSBORDER = 84,
		SM_CYFRAME = 33,
		SM_CYFULLSCREEN = 17,
		SM_CYHSCROLL = 3,
		SM_CYICON = 12,
		SM_CYICONSPACING = 39,
		SM_CYKANJIWINDOW = 18,
		SM_CYMAXIMIZED = 62,
		SM_CYMAXTRACK = 60,
		SM_CYMENU = 15,
		SM_CYMENUCHECK = 72,
		SM_CYMENUSIZE = 55,
		SM_CYMIN = 29,
		SM_CYMINIMIZED = 58,
		SM_CYMINSPACING = 48,
		SM_CYMINTRACK = 35,
		SM_CYSCREEN = 1,
		SM_CYSIZE = 31,
		SM_CYSIZEFRAME = 33,
		SM_CYSMCAPTION = 51,
		SM_CYSMICON = 50,
		SM_CYSMSIZE = 53,
		SM_CYVIRTUALSCREEN = 79,
		SM_CYVSCROLL = 20,
		SM_CYVTHUMB = 9,
		SM_DBCSENABLED = 42,
		SM_DEBUG = 22,
		SM_DIGITIZER = 94,
		SM_IMMENABLED = 82,
		SM_MAXIMUMTOUCHES = 95,
		SM_MEDIACENTER = 87,
		SM_MENUDROPALIGNMENT = 40,
		SM_MIDEASTENABLED = 74,
		SM_MOUSEPRESENT = 19,
		SM_MOUSEHORIZONTALWHEELPRESENT = 91,
		SM_MOUSEWHEELPRESENT = 75,
		SM_NETWORK = 63,
		SM_PENWINDOWS = 41,
		SM_REMOTECONTROL = 0x2001,
		SM_REMOTESESSION = 0x1000,
		SM_SAMEDISPLAYFORMAT = 81,
		SM_SECURE = 44,
		SM_SERVERR2 = 89,
		SM_SHOWSOUNDS = 70,
		SM_SHUTTINGDOWN = 0x2000,
		SM_SLOWMACHINE = 73,
		SM_STARTER = 88,
		SM_SWAPBUTTON = 23,
		SM_TABLETPC = 86,
		SM_XVIRTUALSCREEN = 76,
		SM_YVIRTUALSCREEN = 77,
	}

	internal enum GESTUREID : int
	{
		GID_NONE = 0,
		GID_BEGIN = 1,
		GID_END = 2,
		GID_ZOOM = 3,
		GID_PAN = 4,
		GID_ROTATE = 5,
		GID_TWOFINGERTAP = 6,
		GID_PRESSANDTAP = 7
	}

	[Flags]
	internal enum GESTURECONFIGFLAG : int
	{
		GC_ALLGESTURES = 0x0001,

		GC_ZOOM = 0x0001,

		GC_PAN = 0x0001,
		GC_PAN_WITH_SINGLE_FINGER_VERTICALLY = 0x0002,
		GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY = 0x0004,
		GC_PAN_WITH_GUTTER = 0x0008,
		GC_PAN_WITH_INERTIA = 0x0010,

		GC_ROTATE = 0x0001,

		GC_TWOFINGERTAP = 0x0001,

		GC_PRESSANDTAP = 0x0001
	}

	[Flags]
	internal enum GESTUREFLAG : uint
	{
		GF_BEGIN = 0x0001,
		GF_INERTIA = 0x0002,
		GF_END = 0x0004
	}

	[Flags]
	internal enum TABLET_DISABLE_FLAGS : int
	{
		TABLET_DISABLE_NONE = 0x00000000,
		TABLET_DISABLE_PRESSANDHOLD = 0x00000001,
		TABLET_DISABLE_PENTAPFEEDBACK = 0x00000008,
		TABLET_DISABLE_PENBARRELFEEDBACK = 0x00000010,
		TABLET_DISABLE_TOUCHUIFORCEON = 0x00000100,
		TABLET_DISABLE_TOUCHUIFORCEOFF = 0x00000200,
		TABLET_DISABLE_TOUCHSWITCH = 0x00008000,
		TABLET_DISABLE_FLICKS = 0x00010000,
		TABLET_ENABLE_FLICKSONCONTEXT = 0x00020000,
		TABLET_ENABLE_FLICKLEARNINGMODE = 0x00040000,
		TABLET_DISABLE_SMOOTHSCROLLING = 0x00080000,
		TABLET_DISABLE_FLICKFALLBACKKEYS = 0x00100000,
		TABLET_ENABLE_MULTITOUCHDATA = 0x01000000
	}

	[Flags]
	internal enum WindowStyleEx : int
	{
		WS_EX_LAYERED = 0x00040000,
		WS_EX_APPWINDOW = 0x00080000
	}

	internal enum WindowMessages : int
	{
		WM_WININICHANGE = 0x1A,
		WM_SETTINGCHANGE = WM_WININICHANGE,

		WM_WINDOWPOSCHANGED = 0x47,
		WM_NCPAINT = 0x85,
		WM_NCCALCSIZE = 0x83,
		WM_NCHITTEST = 0x84,

		WM_GESTURENOTIFY = 0x011A,
		WM_GESTURE = 0x0119,

		WM_TABLET_DEFBASE = 0x02C0,
		WM_TABLET_QUERYSYSTEMGESTURESTATUS = (WM_TABLET_DEFBASE + 12),

		WM_THEMECHANGED = 0x031A,
		WM_DWMCOMPOSITIONCHANGED = 0x031E
	}

	internal enum HitTestValue : int
	{
		HTCLIENT = 0x01,
		HTCAPTION = 0x02,
		HTSYSMENU = 0x03,
		HTGROWBOX = 0x04,
		HTMENU = 0x05,
		HTHSCROLL = 0x06,
		HTVSCROLL = 0x07,
		HTMINBUTTON = 0x08,
		HTMAXBUTTON = 0x09,
		HTLEFT = 0x0A,
		HTRIGHT = 0x0B,
		HTTOP = 0x0C,
		HTTOPLEFT = 0x0D,
		HTTOPRIGHT = 0x0E,
		HTBOTTOM = 0x0F,
		HTBOTTOMLEFT = 0x10,
		HTBOTTOMRIGHT = 0x11,
		HTBORDER = 0x12,
		HTOBJECT = 0x13,
		HTCLOSE = 0x14,
		HTHELP = 0x15
	}
	#endregion

	#region Structures
	[StructLayout(LayoutKind.Sequential)]
	internal struct RECT
	{
		public int left, top, right, bottom;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct WINDOWPOS
	{
		public IntPtr hwnd;
		public IntPtr hwndinsertafter;
		public int x, y, cx, cy;
		public int flags;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct NCCALCSIZE_PARAMS
	{
		public RECT rgrc0, rgrc1, rgrc2;
		public WINDOWPOS lppos;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct MARGINS
	{
		public int leftWidth;
		public int rightWidth;
		public int topHeight;
		public int bottomHeight;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct GESTURECONFIG
	{
		public GESTUREID dwID;
		public GESTURECONFIGFLAG dwWant;
		public GESTURECONFIGFLAG dwBlock;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct POINTS
	{
		public ushort x;
		public ushort y;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct GESTUREINFO
	{
		public uint cbSize;
		public GESTUREFLAG dwFlags;
		public GESTUREID dwID;
		public IntPtr hWndTarget;
		public POINTS ptsLocation;
		public uint dwInstanceID;
		public uint dwSequenceID;
		public ulong ullArguments;
		public uint cbExtraArgs;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct GESTURENOTIFYSTRUCT
	{
		public uint cbSize;
		public GESTUREFLAG dwFlags;
		public IntPtr hWndTarget;
		public POINTS ptsLocation;
		public uint dwInstanceID;
	}
	#endregion

	internal class FrameworkHelper
	{
		#region WinAPI
		[DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
		public static extern Int32 SetWindowTheme(IntPtr hWnd, String subAppName, String subIdList);

		[DllImport("user32.dll", ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

		[DllImport("dwmapi.dll")]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref uint attrValue, uint attrSize);

		[DllImport("dwmapi.dll")]
		public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

		[DllImport("dwmapi.dll")]
		public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

		[DllImport("user32.dll")]
		public static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetGestureConfig(IntPtr hWnd, uint dwReserved, uint cIDs, GESTURECONFIG[] pGestureConfig, uint cbSize);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetGestureInfo(IntPtr hGestureInfo, ref GESTUREINFO pGestureInfo);

		[DllImport("user32.dll")]
		public static extern int GetSystemMetrics(SystemMetric smIndex);

		[DllImport("user32.dll")]
		public static extern IntPtr SetCapture(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();
		#endregion
	}
}
