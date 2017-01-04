using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeerViewer.Core
{
	/// <summary>
	/// 상수 모음
	/// </summary>
	internal class Const
	{
		/// <summary>
		/// UNIX 타임스탬프 시작지점
		/// </summary>
		public static DateTimeOffset UnixEpoch { get; } = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

		/// <summary>
		/// 게임 접속 URL (해외 차단 우회)
		/// </summary>
		public static string GameURL { get; } =
			@"javascript:void(eval('document.cookie=\'cklg=ja;expires=Sun, 09 Feb 2019 09:00:09 GMT;domain=dmm.com;path=/\';"
			+ @"document.cookie=\'ckcy=1;expires=Sun, 09 Feb 2019 09:00:09 GMT;domain=osapi.dmm.com;path=/\';"
			+ @"document.cookie=\'ckcy=1;expires=Sun, 09 Feb 2019 09:00:09 GMT;domain=203.104.209.7;path=/\';"
			+ @"document.cookie=\'ckcy=1;expires=Sun, 09 Feb 2019 09:00:09 GMT;domain=www.dmm.com;path=/netgame/\';'));"
			+ @"location.href='http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854/';";

		/// <summary>
		/// <see cref="NotificationTime"/>초 남았을 때 알림
		/// </summary>
		public static int NotificationTime { get; } = 60;
	}
}
