using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Performancer
{
	internal class BlockURIs
	{
		private static IReadOnlyCollection<Regex> BlockHosts { get; }
		private static IReadOnlyCollection<Regex> BlockQueries { get; }

		static BlockURIs()
		{
			BlockHosts = new string[]
				{
					@"spdmg-backend2\.i-mobile\.co\.jp", // アイモバイル(i-mobile)
					@"t\.co", // Twitter
					@"px\.ladsp\.com",
					@"[^\.]+\.[^\.]+\.impact-ad\.jp",
					@"[^\.]+\.[^\.]+\.gmossp-sp\.jp",
					@".+\.microad\.jp",
					@".+\.adingo\.jp",
					@"rt\.gsspat\.jp",
					@"dex\.advg\.jp",
					@"bypass\.ad-stir\.com",
					@"x9\.shinobi\.jp",
					@"cnt\.fout\.jp",
					@"tg\.socdm\.com",
					@".+\.+\.doubleclick\.net",
					@"comcluster\.cxense\.com",
					@"ad\.maist\.jp",
					@"i\.socdm\.com",
				}
				.Select(x => new Regex(x, RegexOptions.Compiled))
				.ToArray();

			BlockQueries = new string[] { }
				.Select(x => new Regex(x, RegexOptions.Compiled))
				.ToArray();
		}

		public static bool IsBlockURI(string Host, string PathAndQuery)
			=> BlockHosts.Any(x => x.IsMatch(Host))
				|| BlockQueries.Any(x => x.IsMatch(PathAndQuery));
	}
}
