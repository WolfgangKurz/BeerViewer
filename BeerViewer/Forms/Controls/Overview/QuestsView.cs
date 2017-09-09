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
using BeerViewer.Modules;

namespace BeerViewer.Forms.Controls.Overview
{
	internal class QuestsView : FrameworkControl
	{
		private TrackManager mTrackManager { get; set; }

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

				mTrackManager = TrackManager.Instance;
				mTrackManager.QuestsEventChanged += (s, e) => this.Invalidate();
			}
			#endregion

			this.Paint += this.OnPaint;
		}

		private void OnPaint(object sender, PaintEventArgs e)
		{
			var g = e.Graphics;
			g.Clear(Constants.colorNormalFace);

			var bY = 2;

			var quests = Homeport.Instance.Quests.Current;

			var qProc = quests
				.Where(x => x != null)
				.Select(q =>
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
							brushCategory = Constants.brushLightBlueAccent;
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

					var textProgress = q.Progress.ToProgressString();
					Brush brushProgress = Constants.brushActiveFace;
					if (q.IsTrackable())
					{
						textProgress = q.TrackableProgress();
						brushProgress = q.IsCompleted()
							? Constants.brushLightBlueAccent
							: Constants.brushYellowAccent;
					}

					return new
					{
						Quest = q,
						CategoryBrush = brushCategory,
						ProgressBrush = brushProgress,
						ProgressText = textProgress,
						ProgressWidth = (int)g.MeasureString(textProgress, Constants.fontDefault).Width
					};
				});

			foreach (var q in qProc)
			{
				var rcTitle = new Rectangle(
					6 + 6, bY,
					this.Width - 6 - 6 - q.ProgressWidth - 6,
					14
				);

				g.FillRectangle(q.CategoryBrush, new Rectangle(6, bY, 4, 14));

				var s = g.Save();

				var q_name = $"quest_{q.Quest.Id}_name";
				var title = i18n.Current[q_name];

				g.DrawString(
					(title == q_name) ? q.Quest.Title : title,
					Constants.fontDefault,
					Brushes.White,
					rcTitle,
					new StringFormat
					{
						Trimming = StringTrimming.EllipsisCharacter,
						LineAlignment = StringAlignment.Center
					}
				);
				g.DrawString(
					q.ProgressText,
					Constants.fontDefault,
					q.ProgressBrush,
					new Rectangle(
						rcTitle.Right, bY,
						q.ProgressWidth, 14
					),
					new StringFormat
					{
						FormatFlags = StringFormatFlags.NoWrap,
						Trimming = StringTrimming.None,
						LineAlignment = StringAlignment.Center
					}
				);

				g.Restore(s);
				bY += 14 + 4 + 4;
			}
			bY += 2;

			this.Height = bY;
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

		public static bool IsCompleted(this Quest quest)
		{
			var tm = TrackManager.Instance;
			try
			{
				var tq = tm.AllQuests?.FirstOrDefault(x => x.Id == quest.Id);
				return tq.GetCurrent() == tq.GetMaximum();
			}
			catch { }
			return false;
		}
		public static string TrackableProgress(this Quest quest)
		{
			var tm = TrackManager.Instance;
			try
			{
				var tq = tm.AllQuests?.FirstOrDefault(x => x.Id == quest.Id);
				return $"{tq.GetCurrent()}/{tq.GetMaximum()}";
			}
			catch { }
			return "?/?";
		}
	}
}
