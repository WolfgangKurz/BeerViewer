using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.Win32;

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

		/// <summary>
		/// Draw rounded rectangle
		/// </summary>
		/// <param name="bound">Bound to draw</param>
		/// <param name="radius">Corner radius size</param>
		/// <param name="pen">Pen to draw</param>
		internal static void DrawRoundedRectangle(this Graphics g, Rectangle bound, int radius, Pen pen)
		{
			int strokeOffset = Convert.ToInt32(Math.Ceiling(pen.Width));
			bound = Rectangle.Inflate(bound, -strokeOffset, -strokeOffset);

			pen.EndCap = pen.StartCap = LineCap.Round;

			GraphicsPath path = new GraphicsPath();
			path.AddArc(bound.X, bound.Y, radius, radius, 180, 90);
			path.AddArc(bound.X + bound.Width - radius, bound.Y, radius, radius, 270, 90);
			path.AddArc(bound.X + bound.Width - radius, bound.Y + bound.Height - radius, radius, radius, 0, 90);
			path.AddArc(bound.X, bound.Y + bound.Height - radius, radius, radius, 90, 90);
			path.CloseAllFigures();

			g.DrawPath(pen, path);
		}

		/// <summary>
		/// Fill rounded rectangle
		/// </summary>
		/// <param name="bound">Bound to draw</param>
		/// <param name="radius">Corner radius size</param>
		/// <param name="brush">Brush to fill</param>
		internal static void FillRoundedRectangle(this Graphics g, Rectangle bound, int radius, Brush brush)
		{
			GraphicsPath path = new GraphicsPath();
			path.AddArc(bound.X, bound.Y, radius, radius, 180, 90);
			path.AddArc(bound.X + bound.Width - radius, bound.Y, radius, radius, 270, 90);
			path.AddArc(bound.X + bound.Width - radius, bound.Y + bound.Height - radius, radius, radius, 0, 90);
			path.AddArc(bound.X, bound.Y + bound.Height - radius, radius, radius, 90, 90);
			path.CloseAllFigures();

			g.FillPath(brush, path);
		}

		/// <summary>
		/// Running device is Tablet and in Tablet-mode?
		/// </summary>
		internal static bool IsTabletMode(bool NeedFresh = false)
		{
			if (NeedFresh)
			{
				try
				{
					var tabletMode = (int)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ImmersiveShell", "TabletMode", 0);
					return (tabletMode == 1);
				}
				catch
				{
					var ConvertibleSlateMode = FrameworkHelper.GetSystemMetrics(SystemMetric.SM_CONVERTIBLESLATEMODE);
					return ConvertibleSlateMode == 0;
				}
			}
			return TabletMode;
		}

		internal static void SetTabletMode(bool IsTablet) => TabletMode = IsTablet;
		private static bool TabletMode { get; set; }

		/// <summary>
		/// Get <see cref="WindowInfo"/> from Form window
		/// </summary>
		internal static WindowInfo GetWindowInformation(this Form form)
		{
			return new WindowInfo
			{
				Left = form.Left,
				Top = form.Top,
				Width = form.ClientSize.Width,
				Height = form.ClientSize.Height
			};
		}
	}
}
