using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BeerViewer.Models;

namespace BeerViewer.Views.Controls
{
	public partial class SlotitemListTable : UserControl
	{
		public int RowSize => 22;

		public string[] Headers { get; } = new string[] {
				"장비명", "총계", "강화도", "숙련도",
				"보유수 (여분)", "장비중인 칸무스"
		};
		public int[] ColumnSizes { get; private set; } = new int[6];

		public Homeport homeport { get; private set; }

		private TableView tableView { get; }

		public SlotitemListTable()
		{
			InitializeComponent();

			this.Resize += (s, e) =>
			{
				this.panelDataView.Location = new Point(1, this.RowSize);
				this.panelDataView.Size = new Size(
					this.ClientSize.Width - 2,
					this.ClientSize.Height - (this.RowSize - 1) - 2
				);
				this.RequestUpdate();
			};
			this.Paint += (s, e) =>
			{
				var g = e.Graphics;
				var Width = this.Width - this.Padding.Left - this.Padding.Right;
				var Height = this.Height - this.Padding.Top - this.Padding.Bottom;

				g.Clear(this.BackColor);

				int x, y = 0;
				using (Pen p = new Pen(Color.FromArgb(83, 83, 83), 1.0f))
				{
					g.DrawRectangle(p, new Rectangle(0, y, Width - 1, Height - y - 1));
					g.DrawRectangle(p, new Rectangle(0, RowSize, Width - 1, Height - RowSize - 1 - 1));

					#region Header Rendering
					x = this.panelDataView.AutoScrollPosition.X;
					y = 0;

					using (SolidBrush b = new SolidBrush(Color.FromArgb(0x20, 0x20, 0x20)))
						g.FillRectangle(b, new Rectangle(0, y, Width, RowSize));

					g.DrawRectangle(p, new Rectangle(0, y, Width - 1, RowSize - 1));

					for (var i = 0; i < ColumnSizes.Length; i++)
					{
						g.DrawRectangle(p, new Rectangle(x, 0, ColumnSizes[i], RowSize - 1));
						TextRenderer.DrawText(
							g,
							Headers[i],
							this.Font,
							new Rectangle(x + 3, y, ColumnSizes[i] - 8, RowSize),
							Color.White,
							TextFormatFlags.VerticalCenter | TextFormatFlags.Left
						);
						x += ColumnSizes[i];
					}
					#endregion
				}
			};

			tableView = new TableView(this);
			tableView.Margin = new Padding(0, 0, 0, 0);
			tableView.Padding = new Padding(0, 0, 0, 0);
			tableView.Dock = DockStyle.Top;
			tableView.AutoSize = true;
			tableView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panelDataView.Controls.Add(tableView);

			panelDataView.Scroll += (s, e) => this.Invalidate(new Rectangle(0, 0, this.Width, this.RowSize + 2));
		}
		public void SetHomeport(Homeport homeport)
		{
			this.homeport = homeport;
			this.RequestUpdate();

			if (homeport == null) return;

			homeport.Organization.PropertyEvent(nameof(homeport.Itemyard.SlotItems), () => tableView.RequestUpdate(), true);
		}
		public void RequestUpdate()
		{
			if (this.InvokeRequired)
			{
				this.Invoke(RequestUpdate);
				return;
			}

			this.PerformAutoScale();
			this.PerformLayout();
			this.Invalidate();
		}

		private partial class TableView : UserControl
		{
			public int RowSize => Owner?.RowSize ?? 22;

			public string[] Headers => Owner?.Headers;
			public int[] ColumnSizes => Owner?.ColumnSizes;

			protected Dictionary<Rectangle, SlotItem> SlotItemMap { get; private set; } = new Dictionary<Rectangle, SlotItem>();
			protected SlotItem CurrentItem { get; private set; }
			public Homeport homeport => Owner?.homeport;

			private Size LatestSize { get; set; } = new Size(640, 480);

			private SlotitemListTable Owner { get; }
			private ToolTip toolTip { get; }


			private System.ComponentModel.IContainer components = null;
			protected override void Dispose(bool disposing)
			{
				if (disposing && (components != null))
				{
					components.Dispose();
				}
				base.Dispose(disposing);
			}

			public TableView(SlotitemListTable Owner)
			{
				// SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);
				this.DoubleBuffered = true;
				this.Owner = Owner;
				this.Font = Owner?.Font;

				this.components = new System.ComponentModel.Container();
				toolTip = new ToolTip(this.components);
				toolTip.OwnerDraw = true;

				this.Resize += (s, e) => this.RequestUpdate();
				this.Paint += (s, e) =>
				{
					var g = e.Graphics;
					var Width = this.Width - this.Padding.Left - this.Padding.Right;
					var Height = this.Height - this.Padding.Top - this.Padding.Bottom;

					g.Clear(this.BackColor);

					Font font8 = new Font(this.Font.FontFamily, 8);

					var Ships = homeport?.Organization?.Ships;
					var SlotItems = homeport?.Itemyard?.SlotItems.Select(_ => _.Value)
						.OrderBy(_ => _.Info.Id)
						.GroupBy(_ => _.Info.Id);

					#region Cell Size Calculate
					ColumnSizes[0] = SlotItems?.Count() > 0
						? (SlotItems?.Max(__ =>
							__.Max(_ => TextRenderer.MeasureText(_.Info.Name.ToString(), this.Font).Width)
						) ?? 0) + 20
						: 0;
					ColumnSizes[1] = SlotItems?.Count() > 0
						? (SlotItems?.Max(_ => TextRenderer.MeasureText(_.Count().ToString(), this.Font).Width) ?? 0)
						: 0;
					ColumnSizes[2] = ColumnSizes[3] = ColumnSizes[4] = 0;


					ColumnSizes[0] = Math.Max(ColumnSizes[0], TextRenderer.MeasureText(Headers[0], this.Font).Width) + 8;
					ColumnSizes[1] = Math.Max(ColumnSizes[1], TextRenderer.MeasureText(Headers[1], this.Font).Width) + 8;
					ColumnSizes[2] = Math.Max(ColumnSizes[2], TextRenderer.MeasureText(Headers[2], this.Font).Width) + 8;
					ColumnSizes[3] = Math.Max(ColumnSizes[3], TextRenderer.MeasureText(Headers[3], this.Font).Width) + 8;
					ColumnSizes[4] = Math.Max(ColumnSizes[4], TextRenderer.MeasureText(Headers[4], this.Font).Width) + 8;
					ColumnSizes[5] = 0;

					ColumnSizes[5] = (this.Owner?.Width ?? this.Width) - ColumnSizes.Sum() - 1 - 18;
					#endregion

					int x, y = 0;
					using (Pen p = new Pen(Color.FromArgb(83, 83, 83), 1.0f))
					{
						SlotItemMap.Clear();

						#region Data Rendering
						if (SlotItems != null)
						{
							Color colorWhite = Color.White;
							Color colorDarkGray = Color.FromArgb(0x20, 0x90, 0x90, 0x90);
							Color colorGray = Color.FromArgb(0x40, 0xC4, 0xC4, 0xC4);
							Color colorLevel = Color.FromArgb(0x45, 0xA9, 0xA5);
							Color colorProficiency123 = Color.FromArgb(0x98, 0xB3, 0xCE);
							Color colorProficiency4567 = Color.FromArgb(0xD4, 0x9C, 0x0F);
							int idx = 0;
							y = 0;

							foreach (var slotitemIdGroup in SlotItems)
							{
								if (slotitemIdGroup == null) continue;

								int widthValue, nx;
								x = 0;
								y -= 2;

								var slotitemLevelGroup = slotitemIdGroup
									.GroupBy(_ => _.NameWithLevel);

								if (idx % 2 == 1)
								{
									int py = y;

									#region Calcuate Equipped Ships Size
									foreach (var slotitems in slotitemLevelGroup)
									{
										int nx2 = x;
										var EquippedShips = Ships.Where(
											_ => _.Value.Slots.Any(__ => slotitems.Any(___ => ___.Id == __.Item.Id))
												|| (slotitems.Any(___ => ___.Id == _.Value.ExSlot?.Item.Id))
										).Select(_ => _.Value);

										foreach (var ship in EquippedShips)
										{
											int sizeName = 0;
											int sizeLevel = 0;
											int sizeCount = 0;
											int hasCount = ship.Slots.Count(_ => slotitems.Any(__ => __.Id == _.Item.Id))
											+ (slotitems.Any(_ => _.Id == ship.ExSlot?.Item.Id) ? 1 : 0);

											sizeName = TextRenderer.MeasureText(ship.Info.Name, this.Font).Width - 2;
											sizeLevel = TextRenderer.MeasureText("Lv." + ship.Level.ToString(), font8).Width;
											if (hasCount > 1)
												sizeCount = TextRenderer.MeasureText("x " + hasCount.ToString(), font8).Width + 1;

											if ((x - nx2) + sizeName + sizeLevel + sizeCount > ColumnSizes[5])
											{
												x = nx2;
												y += RowSize;
											}
											x += sizeName + sizeLevel + sizeCount + 8;
										}
									}
									#endregion

									using (SolidBrush b = new SolidBrush(Color.FromArgb(0x18, 0x90, 0x90, 0x90)))
										g.FillRectangle(b, new Rectangle(0, py, Width, y - py + RowSize));

									x = 0;
									y = py;
								}

								#region Icon And Name
								Image icon = ImageAssets.GetSlotIconImage(slotitemIdGroup.First().Info.IconType);
								g.DrawImage(icon, new Rectangle(x + 4, y + 2, 18, 18));

								TextRenderer.DrawText(
									g,
									slotitemIdGroup.First().Info.Name,
									this.Font,
									new Rectangle(x + 20 + 3, y, ColumnSizes[0] - 8, RowSize),
									colorWhite,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								SlotItemMap.Add(new Rectangle(x, y, ColumnSizes[0], RowSize), slotitemIdGroup.First());
								x += ColumnSizes[0];
								#endregion

								#region Items count (Group)
								TextRenderer.DrawText(
									g,
									slotitemIdGroup.Count().ToString(),
									this.Font,
									new Rectangle(x + 3, y, ColumnSizes[1] - 8, RowSize),
									colorWhite,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								x += ColumnSizes[1];
								#endregion

								nx = x;
								foreach (var slotitems in slotitemLevelGroup)
								{
									if (slotitems == null) continue;

									var slotitem = slotitems.First();

									#region Item Level
									TextRenderer.DrawText(
										g,
										"★" + (slotitem.Level == 10 ? "max" : "+" + slotitem.Level.ToString()),
										this.Font,
										new Rectangle(x + 3, y, ColumnSizes[2] - 8, RowSize),
										slotitem.Level > 0 ? colorLevel : colorDarkGray,
										TextFormatFlags.VerticalCenter | TextFormatFlags.Left
									);
									x += ColumnSizes[2];
									#endregion

									#region Item Proficiency
									TextRenderer.DrawText(
										g,
										"+" + slotitem.Proficiency.ToString(),
										this.Font,
										new Rectangle(x + 3, y, ColumnSizes[3] - 8, RowSize),
										slotitem.Proficiency >= 4
										? colorProficiency4567
										: (
											slotitem.Proficiency >= 1 && slotitem.Proficiency <= 3
											? colorProficiency123
											: colorDarkGray
										),
										TextFormatFlags.VerticalCenter | TextFormatFlags.Left
									);
									x += ColumnSizes[3];
									#endregion

									#region Items count
									widthValue = TextRenderer.MeasureText(
										slotitems.Count().ToString(),
										this.Font
									).Width;

									TextRenderer.DrawText(
										g,
										slotitems.Count().ToString(),
										this.Font,
										new Rectangle(x + 3, y, ColumnSizes[4] - 8, RowSize),
										colorWhite,
										TextFormatFlags.VerticalCenter | TextFormatFlags.Left
									);
									TextRenderer.DrawText(
										g,
										"(" + slotitems.Count(
											_ => !Ships.Any(
												__ => __.Value.Slots.Any(___ => ___.Item.Id == _.Id)
													|| (__.Value.ExSlot?.Item.Id == _.Id)
											)
										) + ")",
										font8,
										new Rectangle(x + widthValue + 3, y, ColumnSizes[4] - 8, RowSize - 4),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
									x += ColumnSizes[4];
									#endregion

									#region Equipped Ships
									int nx2 = x;
									var EquippedShips = Ships.Where(
										_ => _.Value.Slots.Any(__ => slotitems.Any(___ => ___.Id == __.Item.Id))
											|| (slotitems.Any(___ => ___.Id == _.Value.ExSlot?.Item.Id))
									).Select(_ => _.Value);

									foreach (var ship in EquippedShips)
									{
										int sizeName = 0;
										int sizeLevel = 0;
										int sizeCount = 0;
										int hasCount = ship.Slots.Count(_ => slotitems.Any(__ => __.Id == _.Item.Id))
										+ (slotitems.Any(_ => _.Id == ship.ExSlot?.Item.Id) ? 1 : 0);

										sizeName = TextRenderer.MeasureText(ship.Info.Name, this.Font).Width - 2;
										sizeLevel = TextRenderer.MeasureText("Lv." + ship.Level.ToString(), font8).Width;
										if (hasCount > 1)
											sizeCount = TextRenderer.MeasureText("x " + hasCount.ToString(), font8).Width + 1;

										if ((x - nx2) + sizeName + sizeLevel + sizeCount > ColumnSizes[5])
										{
											x = nx2;
											y += RowSize;
										}

										TextRenderer.DrawText(g, ship.Info.Name, this.Font, new Point(x, y + 4), colorWhite);
										TextRenderer.DrawText(g, "Lv." + ship.Level.ToString(), font8, new Point(x + sizeName, y + 4 + 2), colorGray);
										if (hasCount > 1)
										{
											using (SolidBrush b = new SolidBrush(Color.FromArgb(0x20, 0x20, 0x20)))
												g.FillRectangle(b, new Rectangle(x + sizeName + sizeLevel, y + 4, sizeCount, RowSize - 4 - 4));

											TextRenderer.DrawText(g, "x " + hasCount.ToString(), font8, new Point(x + sizeName + sizeLevel + 1, y + 4 + 1), colorGray);
										}
										x += sizeName + sizeLevel + sizeCount + 8;
									}
									#endregion

									y += RowSize;
									x = nx;
								} // GroupBy LevelProficiency

								idx++;
								y += 3;
								g.DrawLine(p, 0, y - 3, Width, y - 3);
							} // GroupBy Id
						}
						#endregion
					}

					var ResultSize = new Size(
						ColumnSizes.Sum() + this.Padding.Left + this.Padding.Right + 1,
						y + this.Padding.Top + this.Padding.Bottom + 1
					);
					if (ResultSize.Width != LatestSize.Width || ResultSize.Height != LatestSize.Height)
					{
						LatestSize = ResultSize;
						this.PerformAutoScale();
						this.PerformLayout();
						this.Owner.RequestUpdate();
					}
				};

				var toolTipFont = new Font(this.Font.FontFamily, 10);
				toolTip.Popup += (s, e) =>
				{
					if (CurrentItem == null)
					{
						e.Cancel = true;
						return;
					}

					var text = CurrentItem.Info.Name + Environment.NewLine + CurrentItem.Info.ToolTipData;
					var sz = TextRenderer.MeasureText(text, toolTipFont);
					e.ToolTipSize = new Size(sz.Width + 6, sz.Height + 6);
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

				this.MouseMove += (s, e) =>
				{
					if (!this.SlotItemMap.Any(x => x.Key.Contains(e.X, e.Y)))
					{
						CurrentItem = null;
						toolTip.Hide(this);
						return;
					}

					var item = this.SlotItemMap.FirstOrDefault(x => x.Key.Contains(e.X, e.Y)).Value;
					if (item == CurrentItem) return;

					CurrentItem = item;
					toolTip.Show(CurrentItem.Info.Name + Environment.NewLine + CurrentItem.Info.ToolTipData, this);
				};
				this.MouseDown += (s, e) =>
				{
					if (!this.SlotItemMap.Any(x => x.Key.Contains(e.X, e.Y)))
					{
						CurrentItem = null;
						toolTip.Hide(this);
						return;
					}

					var item = this.SlotItemMap.FirstOrDefault(x => x.Key.Contains(e.X, e.Y)).Value;
					CurrentItem = item;
					toolTip.Show(CurrentItem.Info.Name + Environment.NewLine + CurrentItem.Info.ToolTipData, this);
				};
			}

			public void RequestUpdate()
			{
				if (this.InvokeRequired)
				{
					this.Invoke(RequestUpdate);
					return;
				}

				this.PerformAutoScale();
				this.PerformLayout();
				this.Invalidate();
			}
			public override Size GetPreferredSize(Size proposedSize) => new Size(this.Width, LatestSize.Height);
		} // class
	}
}
