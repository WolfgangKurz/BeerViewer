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

		/// <summary>
		/// Get limited value in range
		/// </summary>
		/// <param name="Value">Value to limit</param>
		/// <param name="Min">Minimum value</param>
		/// <param name="Max">Maximum value</param>
		/// <returns></returns>
		internal static int InRange(this int Value, int Min, int Max)
			=> (Value < Min) ? Min : ((Value > Max) ? Max : Value);

		/// <summary>
		/// Get array's value safely
		/// </summary>
		/// <param name="Array">Source array</param>
		/// <param name="Index">Index value</param>
		/// <returns></returns>
		internal static int? Get(this int[] Array, int Index)
			=> Index < 0 || Index >= Array.Length
				? null
				: (int?)Array[Index];
	}
}
