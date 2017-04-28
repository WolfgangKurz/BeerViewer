using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models.Raw
{
	internal static class Extensions
	{
		public static int? Get(this int[] array, int index)
		{
			if (index < 0 || index >= array.Length) return null;
			return array[index];
		}
	}
}
