using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BeerViewer.Models;

namespace BeerViewer.Views.Controls
{
	public partial class QuestsView : UserControl
	{
		#region HeaderName 프로퍼티
		private string _HeaderName { get; set; } = "임무";
		public string HeaderName
		{
			get { return this._HeaderName; }
			set
			{
				if (this._HeaderName != value)
				{
					this._HeaderName = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion

		#region PlaceHolder 프로퍼티
		private string _PlaceHolder { get; set; }
		public string PlaceHolder
		{
			get { return this._PlaceHolder; }
			set
			{
				if (this._PlaceHolder != value)
				{
					this._PlaceHolder = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion

		#region Quests 프로퍼티
		private Quest[] _Quests { get; set; }
		public Quest[] Quests
		{
			get { return this._Quests; }
			set
			{
				if (this._Quests != value)
				{
					this._Quests = value;
					this.Invalidate();
				}
			}
		}
		#endregion

		private Dictionary<Rectangle, Quest> QuestMap { get; set; } = new Dictionary<Rectangle, Quest>();
		private Quest CurrentQuest { get; set; }

		public QuestsView()
		{
			InitializeComponent();

			this.Paint += (s, e) =>
			{
				var g = e.Graphics;
				var Width = this.Width - this.Padding.Left - this.Padding.Right;
				var Height = this.Height - this.Padding.Top - this.Padding.Bottom;

				g.TranslateTransform(this.Padding.Left, this.Padding.Top);
				QuestMap.Clear();

				#region Draw Header
				using (SolidBrush b = new SolidBrush(Color.FromArgb(0x30, 0x90, 0x90, 0x90)))
					g.FillRectangle(b, new Rectangle(0, 0, 30, Height));

				using (SolidBrush b = new SolidBrush(Color.White))
				{
					Size sz = TextRenderer.MeasureText(HeaderName, this.Font);

					var state = g.Save();
					g.TranslateTransform(-sz.Width / 2, -sz.Height / 2, MatrixOrder.Append);
					g.RotateTransform(-90, MatrixOrder.Append);
					g.TranslateTransform(15, Height / 2, MatrixOrder.Append);

					g.DrawString(HeaderName, this.Font, b, 0, 0);
					g.Restore(state);
				}
				#endregion

				#region Draw Placeholder
				if ((this.Quests?.Length ?? 0) == 0)
				{
					TextRenderer.DrawText(
						g,
						this.PlaceHolder,
						this.Font,
						new Rectangle(30, 0, Width - 30, Height),
						Color.White,
						TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
						| TextFormatFlags.WordBreak
					);
				}
				#endregion
				#region Draw Quest
				else
				{
					var y = 0;
					foreach (var quest in this.Quests)
					{
						var x = 30;

						QuestMap.Add(new Rectangle(30, y, Width - 30, 24), quest);

						// Quest Category Indicator
						using (SolidBrush b = new SolidBrush(QuestCategoryColor(quest.Category)))
							g.FillRectangle(b, new Rectangle(x + 4, y + 4, 16, 16));
						x += 24;

						// Quest Title
						Size sz = TextRenderer.MeasureText(quest.Title, this.Font);
						TextRenderer.DrawText(
							g,
							quest.Title,
							this.Font,
							new Rectangle(x, y, sz.Width, 24),
							Color.White,
							TextFormatFlags.VerticalCenter
						);
						x += sz.Width;

						// Quest Progress
						if (quest.State == QuestState.Accomplished || quest.Progress != QuestProgress.None)
						{
							string text = QuestProgresText(quest.Progress, quest.State);
							sz = TextRenderer.MeasureText(text, this.Font);

							using (SolidBrush b = new SolidBrush(QuestProgresColor(quest.Progress, quest.State)))
								g.FillRectangle(b, new Rectangle(x, y + 1, sz.Width + 8, 22));

							TextRenderer.DrawText(
								g,
								text,
								this.Font,
								new Rectangle(x + 4, y, sz.Width, 24),
								Color.White,
								TextFormatFlags.VerticalCenter
							);
						}

						y += 24;
					}
				}
				#endregion
			};
			this.MouseMove += (s, e) =>
			{
				if (!this.QuestMap.Any(x => x.Key.Contains(e.X, e.Y)))
				{
					CurrentQuest = null;
					toolTip.Hide(this);
					return;
				}

				var item = this.QuestMap.FirstOrDefault(x => x.Key.Contains(e.X, e.Y)).Value;
				if (item == CurrentQuest) return;

				CurrentQuest = item;
				toolTip.Show(CurrentQuest.Title + Environment.NewLine + CurrentQuest.Detail, this);
			};
			this.MouseDown += (s, e) =>
			{
				if (!this.QuestMap.Any(x => x.Key.Contains(e.X, e.Y)))
				{
					CurrentQuest = null;
					toolTip.Hide(this);
					return;
				}

				var item = this.QuestMap.FirstOrDefault(x => x.Key.Contains(e.X, e.Y)).Value;
				CurrentQuest = item;
				toolTip.Show(CurrentQuest.Title + Environment.NewLine + CurrentQuest.Detail, this);
			};

			var toolTipFont = new Font(this.Font.FontFamily, 10);
			toolTip.Popup += (s, e) =>
			{
				if (CurrentQuest == null)
				{
					e.Cancel = true;
					return;
				}

				var sz = TextRenderer.MeasureText(CurrentQuest.Detail, toolTipFont);
				e.ToolTipSize = new Size(sz.Width + 6, sz.Height * 2 + 6);
			};
			toolTip.Draw += (s, e) =>
			{
				var g = e.Graphics;
				g.Clear(Color.FromArgb(0x27, 0x27, 0x2F));
				g.DrawRectangle(
					new Pen(Color.FromArgb(0x44, 0x44, 0x4A), 1.0f),
					new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width - 1, e.Bounds.Height - 1)
				);
				TextRenderer.DrawText(
					g,
					e.ToolTipText,
					toolTipFont,
					e.Bounds,
					Color.FromArgb(255, 255, 255),
					TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
				);
			};
		}

		protected override void OnPaddingChanged(EventArgs e)
		{
			this.RequestUpdate();
			base.OnPaddingChanged(e);
		}
		public override Size GetPreferredSize(Size proposedSize)
		{
			if ((this.Quests?.Length ?? 0) == 0)
				return new Size(
					30 + 24 + this.Padding.Left + this.Padding.Right,
					48 + this.Padding.Top + this.Padding.Bottom
				);
			else
				return new Size(
					30 + 24
					+ this.Quests.Sum(x =>
						TextRenderer.MeasureText(x.Title, this.Font).Width
						+ (x.State == QuestState.Accomplished || x.Progress != QuestProgress.None
						 ? TextRenderer.MeasureText(QuestProgresText(x.Progress, x.State), this.Font).Width + 8
						 : 0)
					) + this.Padding.Left + this.Padding.Right,
					24 * Math.Max(2, this.Quests.Length)
					+ this.Padding.Top + this.Padding.Bottom
				);
		}

		private Color QuestCategoryColor(QuestCategory category)
		{
			switch (category)
			{
				case QuestCategory.Composition:
					return Color.FromArgb(0x2A, 0x7D, 0x46);
				case QuestCategory.Sortie:
					return Color.FromArgb(0xB5, 0x3B, 0x36);
				case QuestCategory.Expeditions:
					return Color.FromArgb(0x3B, 0xA0, 0x9D);
				case QuestCategory.Practice:
					return Color.FromArgb(0x8D, 0xC6, 0x60);
				case QuestCategory.Supply:
					return Color.FromArgb(0xB2, 0x93, 0x2E);
				case QuestCategory.Building:
					return Color.FromArgb(0x64, 0x44, 0x3B);
				case QuestCategory.Remodelling:
					return Color.FromArgb(0xA9, 0x87, 0xBA);
				case QuestCategory.Sortie2:
					return Color.FromArgb(0xB5, 0x3B, 0x36);
			}
			return Color.Gray;
		}
		private Color QuestProgresColor(QuestProgress progress, QuestState state)
		{
			if (state == QuestState.Accomplished)
				return Color.FromArgb(0x32, 0x64, 0x80);

			switch (progress)
			{
				case QuestProgress.Progress50:
					return Color.FromArgb(0x20, 0x80, 0x40);
				case QuestProgress.Progress80:
					return Color.FromArgb(0x20, 0x60, 0x40);
				default:
					return Color.Transparent;
			}
		}
		private string QuestProgresText(QuestProgress progress, QuestState state)
		{
			if (state == QuestState.Accomplished)
				return "완료";

			switch (progress)
			{
				case QuestProgress.Progress50:
					return "50%";
				case QuestProgress.Progress80:
					return "80%";
				default:
					return "";
			}
		}

		public void RequestUpdate()
		{
			if (this.InvokeRequired)
			{
				this.Invoke(RequestUpdate);
				return;
			}
			this.Invalidate();
			this.PerformAutoScale();
			this.PerformLayout();
		}
	}
}
