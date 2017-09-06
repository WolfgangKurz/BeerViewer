using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

using BeerViewer.Framework;
using BeerViewer.Forms.Controls;
using BeerViewer.Models;
using BeerViewer.Models.Enums;
using BeerViewer.Models.Wrapper;
using BeerViewer.Models.Quest;

namespace BeerViewer.Forms.Controls.Overview
{
	internal class QuestsView : FrameworkControl
	{
		#region Initializers
		public QuestsView() : base()
			=> this.Initialize();

		public QuestsView(FrameworkRenderer Renderer) : base(Renderer)
			=> this.Initialize();

		public QuestsView(int X, int Y) : base(X, Y)
			=> this.Initialize();

		public QuestsView(FrameworkRenderer Renderer, int X, int Y) : base(Renderer, X, Y)
			=> this.Initialize();

		public QuestsView(int X, int Y, int Width, int Height) : base(X, Y, Width, Height)
			=> this.Initialize();

		public QuestsView(FrameworkRenderer Renderer, int X, int Y, int Width, int Height) : base(Renderer, X, Y, Width, Height)
			=> this.Initialize();
		#endregion

		private void Initialize()
		{
			#region PropertyEventListener
			{
				var quest = Homeport.Instance.Quests;
				quest.PropertyEvent(nameof(quest.Current), () => this.Invalidate(), true);
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

			var quests = Homeport.Instance.Quests.Current;

			foreach(var q in quests)
			{
				Brush brushCategory = Brushes.Gray;
				switch (q.Category)
				{
					case QuestCategory.Composition:
						brushCategory = Constants.brushDeepGreenAccent;
						break;

					case QuestCategory.Sortie:
					case QuestCategory.Sortie2:
						brushCategory = Constants.brushRedAccent;
						break;

					case QuestCategory.Practice:
						brushCategory = Constants.brushGreenAccent;
						break;

					case QuestCategory.Expeditions:
						brushCategory = Constants.brushBlueAccent;
						break;

					case QuestCategory.Supply:
						brushCategory = Constants.brushYellowAccent;
						break;

					case QuestCategory.Building:
						brushCategory = Constants.brushBrownAccent;
						break;

					case QuestCategory.Remodelling:
						brushCategory = Constants.brushPurpleAccent;
						break;
				}

				var txt = q.Progress.ToProgressString();
				if (q.IsTrackable())
				{

				}
			}
		}
	}

	internal static class QuestExtension
	{
		public static string ToProgressString(this QuestProgress Progress)
		{
			switch (Progress)
			{
				case QuestProgress.Progress80:
					return "80%";

				case QuestProgress.Progress50:
					return "50%";

				default:
					return "";
			}
		}
		public static bool IsTrackable(this Quest quest)
		{
			return TrackManager.Instance.trackingAvailable
				.Any(x => x.Id == quest.Id);
		}
	}
}
