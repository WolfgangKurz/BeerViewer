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

		public static string DMMCookie => "document.cookie='cklg=ja;expires=Sun, 09 Feb 2055 09:00:09 GMT;domain=dmm.com;path=/';"
			+ "document.cookie='ckcy=1;expires=Sun, 09 Feb 2055 09:00:09 GMT;domain=osapi.dmm.com;path=/';"
			+ "document.cookie='ckcy=1;expires=Sun, 09 Feb 2055 09:00:09 GMT;domain=203.104.209.7;path=/';"
			+ "document.cookie='ckcy=1;expires=Sun, 09 Feb 2055 09:00:09 GMT;domain=www.dmm.com;path=/netgame/';";

		public static string UserStyleSheet => "body { margin:0; overflow:hidden }"
			+ "#game_frame { position:fixed; left:50%; top:-16px; margin-left:-450px; z-index:1 }"
			+ ".area-pickupgame, .area-menu { display: none !important }";

		public static Brush brushHoverFace { get; } = new SolidBrush(FrameworkExtension.FromRgb(0x313131));
		public static Brush brushActiveFace { get; } = new SolidBrush(FrameworkExtension.FromRgb(0x575757));
	}
}
