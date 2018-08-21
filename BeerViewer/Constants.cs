using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Drawing;
using BeerViewer.Framework;

namespace BeerViewer
{
	public class Constants
	{
		public static string GameURL => "http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854";
		// public static string GameURL => "about:blank";

		public static string DMMCookie => "document.cookie='cklg=ja;expires=Sun, 09 Feb 2055 09:00:09 GMT;domain=dmm.com;path=/';"
			+ "document.cookie='ckcy=1;expires=Sun, 09 Feb 2055 09:00:09 GMT;domain=osapi.dmm.com;path=/';"
			+ "document.cookie='ckcy=1;expires=Sun, 09 Feb 2055 09:00:09 GMT;domain=203.104.209.7;path=/';"
			+ "document.cookie='ckcy=1;expires=Sun, 09 Feb 2055 09:00:09 GMT;domain=www.dmm.com;path=/netgame/';";

		public static string UserStyleSheet => "body { margin:0; overflow:hidden }"
			+ "#game_frame { position:fixed; left:50%; top:-16px; margin-left:-600px; z-index:1 }"
			+ ".area-pickupgame, .area-menu { display: none !important }";

		public static Image BitmapNumber => BeerViewer.Properties.Resources.Numbers;

		public static Color colorNormalFace { get; } = FrameworkExtension.FromRgb(0x222225);
		public static Color colorHoverFace { get; } = FrameworkExtension.FromRgb(0x313131);
		public static Color colorActiveFace { get; } = FrameworkExtension.FromRgb(0x575757);

		public static Brush brushNormalFace { get; } = new SolidBrush(colorNormalFace);
		public static Brush brushHoverFace { get; } = new SolidBrush(colorHoverFace);
		public static Brush brushActiveFace { get; } = new SolidBrush(colorActiveFace);
		public static Brush brushDisabled => brushActiveFace;
		public static Brush brushHint => brushActiveFace;


		public static Color colorRedAccent { get; } = FrameworkExtension.FromRgb(0xc62828);
		public static Color colorBlueAccent { get; } = FrameworkExtension.FromRgb(0x1565c0);
		public static Color colorLightBlueAccent { get; } = FrameworkExtension.FromRgb(0x3ba09d);
		public static Color colorGreenAccent { get; } = FrameworkExtension.FromRgb(0x388e3c);
		public static Color colorYellowAccent { get; } = FrameworkExtension.FromRgb(0xfdd835);
		public static Color colorOrangeAccent { get; } = FrameworkExtension.FromRgb(0xf57c00);
		public static Color colorBrownAccent { get; } = FrameworkExtension.FromRgb(0x6d4c41);
		public static Color colorDeepGreenAccent { get; } = FrameworkExtension.FromRgb(0x289010);
		public static Color colorPurpleAccent { get; } = FrameworkExtension.FromRgb(0xA987BA);

		public static Brush brushRedAccent { get; } = new SolidBrush(colorRedAccent);
		public static Brush brushBlueAccent { get; } = new SolidBrush(colorBlueAccent);
		public static Brush brushLightBlueAccent { get; } = new SolidBrush(colorLightBlueAccent);
		public static Brush brushGreenAccent { get; } = new SolidBrush(colorGreenAccent);
		public static Brush brushYellowAccent { get; } = new SolidBrush(colorYellowAccent);
		public static Brush brushOrangeAccent { get; } = new SolidBrush(colorOrangeAccent);
		public static Brush brushBrownAccent { get; } = new SolidBrush(colorBrownAccent);
		public static Brush brushDeepGreenAccent { get; } = new SolidBrush(colorDeepGreenAccent);
		public static Brush brushPurpleAccent { get; } = new SolidBrush(colorPurpleAccent);

		public static Pen penNormalFace { get; } = new Pen(FrameworkExtension.FromRgb(0x222225));
		public static Pen penHoverFace { get; } = new Pen(FrameworkExtension.FromRgb(0x313131));
		public static Pen penActiveFace { get; } = new Pen(FrameworkExtension.FromRgb(0x575757));
		public static Pen penDisabled => penActiveFace;
		public static Pen penHint => penActiveFace;

		public static Pen penRedAccent { get; } = new Pen(colorRedAccent);
		public static Pen penBlueAccent { get; } = new Pen(colorBlueAccent);
		public static Pen penGreenAccent { get; } = new Pen(colorGreenAccent);
		public static Pen penYellowAccent { get; } = new Pen(colorYellowAccent);
		public static Pen penOrangeAccent { get; } = new Pen(colorOrangeAccent);


		public static string FontFamily { get; } = "Malgun Gothic";
		public static float FontDPI { get; } = 0.75f;

		public static Font fontDefault { get; } = new Font(FontFamily, 12 * FontDPI);
		public static Font fontSmall { get; } = new Font(FontFamily, 11 * FontDPI);
		public static Font fontBig { get; } = new Font(FontFamily, 14 * FontDPI);
	}
}
