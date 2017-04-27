using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace BeerViewer
{
	public class Constants
	{
		public static string GameURL => "http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854";
		public static string GameURI => "eval(\"document.cookie = 'cklg=ja;expires=Sun, 09 Feb 2019 09:00:09 GMT;domain=dmm.com;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2019 09:00:09 GMT;domain=osapi.dmm.com;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2019 09:00:09 GMT;domain=203.104.209.7;path=/';document.cookie = 'ckcy=1;expires=Sun, 09 Feb 2019 09:00:09 GMT;domain=www.dmm.com;path=/netgame/';\");location.href=\"" + Constants.GameURL + "/\";";

		public static string StartupPage { get; } = "https://raw.githubusercontent.com/WolfgangKurz/BeerViewer/v2.0/Resources/internal/startup.html";
	}
}
