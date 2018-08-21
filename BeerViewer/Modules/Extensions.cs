using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Modules
{
	internal static class Extensions
	{
		public static Uri UriOrBlank(string Url)
			=> new Uri(string.IsNullOrEmpty(Url) ? "about:blank" : Url);

		public static string ToEvaluatableString(this string Script)
		{
			return Script
				.Replace("\r", "")
				.Replace("\n", "")
				.Replace("\t", "");
		}
	}
}
