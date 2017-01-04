using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	public static class EnumerableEx
	{
		public static IEnumerable<TResult> Return<TResult>(TResult value)
		{
			yield return value;
		}

		public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			var set = new HashSet<TKey>(EqualityComparer<TKey>.Default);

			foreach (var item in source)
			{
				var key = keySelector(item);
				if (set.Add(key)) yield return item;
			}
		}

		public static bool HasItems<T>(this IEnumerable<T> source)
		{
			return source != null && source.Any();
		}
	}
}
