using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Core
{
	internal static class Extensions
	{
		/// <summary>
		/// <see cref="Int32" /> 형 배열에 안전하게 엑세스
		/// </summary>
		public static int? Get(this int[] array, int index)
		{
			return (index >= 0 && index < array?.Length) ? (int?)array[index] : null;
		}
	}
}
