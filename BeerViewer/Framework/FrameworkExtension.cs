using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BeerViewer.Framework
{
	internal static class FrameworkExtension
	{
		/// <summary>
		/// Create <see cref="Color"/> object from RGB integer
		/// </summary>
		/// <param name="rgb">RGB integer, 0xRRGGBB</param>
		/// <returns>Created <see cref="Color"/> object</returns>
		internal static Color FromRgb(int rgb)
			=> Color.FromArgb(0xff, (rgb & 0xff0000) >> 16, (rgb & 0xff00) >> 8, rgb & 0xff);

		/// <summary>
		/// ForEach for not <see cref="IList{T}"/> object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"><see cref="IEnumerable{T}"/> object</param>
		/// <param name="_iterator">Iterator action</param>
		/// <returns></returns>
		internal static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> _iterator)
		{
			foreach (var item in collection)
				_iterator?.Invoke(item);

			return collection;
		}
	}
}
