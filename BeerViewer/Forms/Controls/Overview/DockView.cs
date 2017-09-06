using System;
using System.Drawing;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Forms.Controls;
using BeerViewer.Models;
using BeerViewer.Models.Enums;

namespace BeerViewer.Forms.Controls.Overview
{
	internal class DockView : FrameworkControl
	{
		#region Initializers
		public DockView() : base()
			=> this.Initialize();

		public DockView(FrameworkRenderer Renderer) : base(Renderer)
			=> this.Initialize();

		public DockView(int X, int Y) : base(X, Y)
			=> this.Initialize();

		public DockView(FrameworkRenderer Renderer, int X, int Y) : base(Renderer, X, Y)
			=> this.Initialize();

		public DockView(int X, int Y, int Width, int Height) : base(X, Y, Width, Height)
			=> this.Initialize();

		public DockView(FrameworkRenderer Renderer, int X, int Y, int Width, int Height) : base(Renderer, X, Y, Width, Height)
			=> this.Initialize();
		#endregion

		private void Initialize()
		{
			#region PropertyEventListener
			{
				var dockyard = Homeport.Instance.Dockyard;
				var repairyard = Homeport.Instance.Repairyard;

				dockyard.PropertyEvent(nameof(dockyard.Docks), () =>
				{
					dockyard.Docks?.ForEach(x =>
					{
						var dock = x.Value;
						if (dock == null) return;

						dock.PropertyEvent(nameof(dock.Remaining), () => this.Invalidate());
						dock.PropertyEvent(nameof(dock.State), () => this.Invalidate());
						dock.PropertyEvent(nameof(dock.Ship), () => this.Invalidate());
					});
					this.Invalidate();
				}, true);
				repairyard.PropertyEvent(nameof(repairyard.Docks), () =>
				{
					repairyard.Docks?.ForEach(x =>
					{
						var dock = x.Value;
						if (dock == null) return;

						dock.PropertyEvent(nameof(dock.Remaining), () => this.Invalidate());
						dock.PropertyEvent(nameof(dock.State), () => this.Invalidate());
						dock.PropertyEvent(nameof(dock.Ship), () => this.Invalidate());
					});
					this.Invalidate();
				}, true);
			}
			#endregion

			this.Paint += this.OnPaint;
		}

		private int DrawBitmapNumber(Graphics g, string Text, Point pos, bool OnCenter, int ColorIndex = 0)
		{
			int p = 4, h = 7;
			int p2 = p + 2, hh = h / 2;
			var table = "0123456789[]:";
			int x = -p2 + pos.X - (OnCenter ? Text.Length * p2 / 2 : 0);
			int y = pos.Y - (OnCenter ? hh : 0);

			foreach (var c in Text)
			{
				x += (c == ' ' ? 2 : p2);

				int offset = table.IndexOf(c);
				if (offset < 0) continue;

				g.DrawImage(
					Constants.BitmapNumber,
					new Rectangle(x, y, p, h),
					new Rectangle(offset * p, h * ColorIndex, p, h),
					GraphicsUnit.Pixel
				);
			}
			return x - pos.X;
		}
		private void OnPaint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			var bY = 0;

			#region Repair & Construct dock
			Brush textColor;

			var dockWidth = this.Width - 12;
			if (dockWidth < 188) dockWidth = (dockWidth - 0) / 1;
			else if (dockWidth < 396) dockWidth = (dockWidth - 4) / 2;
			else dockWidth = (dockWidth - 12) / 4;
			dockWidth = Math.Max(dockWidth, 1);

			var perLine = Math.Max(1, (this.Width - 12) / dockWidth);
			var lines = (int)Math.Ceiling(4.0 / perLine);

			for (var i = 0; i < 4; i++)
			{
				var line = i / perLine;
				var dock = Homeport.Instance.Repairyard.Docks[i + 1];
				textColor = Brushes.White;

				var _s = g.Save();
				g.IntersectClip(new Rectangle(
					6 + (dockWidth + 4) * (i % perLine),
					bY + 6 + line * 32,
					dockWidth, 28
				));

				if (dock?.State == RepairingDockState.Repairing)
				{
					var name = i18n.Current[dock.Ship.Info.Name];
					g.FillRectangle(
						Constants.brushBrownAccent,
						6 + (dockWidth + 4) * (i % perLine),
						bY + 6 + line * 32,
						dockWidth, 28
					);

					var txt = i18n.Current.fleet_done;
					if (dock.Remaining.HasValue)
						txt = $"{(int)dock.Remaining.Value.TotalHours:D2}:{dock.Remaining.Value.ToString(@"mm\:ss")}";

					var slWidth = (int)g.MeasureString($"{name}  {txt}", Constants.fontDefault).Width;
					if (slWidth >= dockWidth) // Double Line
					{
						g.DrawString(
							name,
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
								bY + 6 + line * 32 - 1
							),
							new StringFormat { Alignment = StringAlignment.Center }
						);
						g.DrawString(
							txt,
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
								bY + 6 + line * 32 - 1 + 14
							),
							new StringFormat { Alignment = StringAlignment.Center }
						);
					}
					else // Single Line
					{
						g.DrawString(
							$"{name}  {txt}",
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
								bY + 6 + line * 32 + 14 - 8
							),
							new StringFormat { Alignment = StringAlignment.Center }
						);
					}
				}
				else
				{
					Brush face;
					if ((dock?.State ?? RepairingDockState.Locked) == RepairingDockState.Locked)
					{
						face = Constants.brushHoverFace;
						textColor = Brushes.Gray;
					}
					else face = Constants.brushActiveFace;

					g.FillRectangle(
						face,
						6 + (dockWidth + 4) * (i % perLine),
						bY + 6 + line * 32,
						dockWidth, 28
					);

					var txt = (
						(dock?.State ?? RepairingDockState.Locked) == RepairingDockState.Locked
							? i18n.Current.fleet_locked
							: i18n.Current.fleet_repair_empty
					) as string;

					g.DrawString(
						txt,
						Constants.fontDefault,
						textColor,
						new Point(
							6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
							bY + 6 + line * 32 + 14 - 8
						),
						new StringFormat { Alignment = StringAlignment.Center }
					);
				}

				g.Restore(_s);
			}
			bY += 6 + 6 + lines * 32 - 4;

			for (var i = 0; i < 4; i++)
			{
				var line = i / perLine;
				var dock = Homeport.Instance.Dockyard.Docks[i + 1];
				textColor = Brushes.White;

				var _s = g.Save();
				g.IntersectClip(new Rectangle(
					6 + (dockWidth + 4) * (i % perLine),
					bY + 6 + line * 32,
					dockWidth, 28
				));

				if (dock?.State == BuildingDockState.Building || dock?.State == BuildingDockState.Completed)
				{
					var name = i18n.Current[dock.Ship.Name];
					g.FillRectangle(
						dock.State == BuildingDockState.Completed
							? Constants.brushGreenAccent
							: Constants.brushBlueAccent,
						6 + (dockWidth + 4) * (i % perLine),
						bY + 6 + line * 32,
						dockWidth, 28
					);

					var txt = i18n.Current.fleet_done;
					if (dock.Remaining.HasValue)
						txt = $"{(int)dock.Remaining.Value.TotalHours:D2}:{dock.Remaining.Value.ToString(@"mm\:ss")}";

					var slWidth = (int)g.MeasureString($"{name}  {txt}", Constants.fontDefault).Width;
					if (slWidth >= dockWidth) // Double Line
					{
						g.DrawString(
							name,
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
								bY + 6 + line * 32 - 1
							),
							new StringFormat { Alignment = StringAlignment.Center }
						);
						g.DrawString(
							txt,
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
								bY + 6 + line * 32 - 1 + 14
							),
							new StringFormat { Alignment = StringAlignment.Center }
						);
					}
					else // Single Line
					{
						g.DrawString(
							$"{name}  {txt}",
							Constants.fontDefault,
							textColor,
							new Point(
								6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
								bY + 6 + line * 32 + 14 - 8
							),
							new StringFormat { Alignment = StringAlignment.Center }
						);
					}
				}
				else
				{
					Brush face;
					if ((dock?.State ?? BuildingDockState.Locked) == BuildingDockState.Locked)
					{
						face = Constants.brushHoverFace;
						textColor = Brushes.Gray;
					}
					else face = Constants.brushActiveFace;

					g.FillRectangle(
						face,
						6 + (dockWidth + 4) * (i % perLine),
						bY + 6 + line * 32,
						dockWidth, 28
					);

					var txt = (
						(dock?.State ?? BuildingDockState.Locked) == BuildingDockState.Locked
							? i18n.Current.fleet_locked
							: i18n.Current.fleet_dock_empty
					) as string;

					g.DrawString(
						txt,
						Constants.fontDefault,
						textColor,
						new Point(
							6 + (dockWidth + 4) * (i % perLine) + (dockWidth / 2),
							bY + 6 + line * 32 + 14 - 8
						),
						new StringFormat { Alignment = StringAlignment.Center }
					);
				}

				g.Restore(_s);
			}
			bY += 6 + 6 + lines * 32 - 4;
			#endregion

			g.DrawLine(
				Constants.penActiveFace,
				new Point(4, bY + 2 - 1),
				new Point(this.Width - 4, bY + 2 - 1)
			);
			bY += 6;

			this.Height = bY; // Dock info height
		}
	}
}
