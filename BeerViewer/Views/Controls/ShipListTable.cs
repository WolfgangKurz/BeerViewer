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
	public partial class ShipListTable : UserControl
	{
		[Flags]
		public enum FilterValues
		{
			Yes = 1,
			No = 2,

			Fast = 1,
			Slow = 2,

			End = 1,
			NotEnd = 2,

			Both = Yes | No
		}

		public int RowSize => 22;

		public string[] Headers { get; } = new string[] {
				"", "ID", "함종", "함명",
				"레벨", "개장레벨",
				"피로도", "최대 HP",
				"화력", "뇌장", "화력+뇌장",
				"장갑","대공","대잠","운",
				"색적","출격해역","수리시간"
			};
		public int[] ColumnSizes { get; private set; } = new int[18];

		public Homeport homeport { get; private set; }

		#region ShipTypes
		private int[] _ShipTypes { get; set; }
		public int[] ShipTypes
		{
			get { return this._ShipTypes; }
			set
			{
				if (this._ShipTypes != value)
				{
					this._ShipTypes = value;
					this.RequestUpdate();
					this.tableView?.RequestUpdate();
				}
			}
		}
		#endregion

		#region LevelFrom
		private int _LevelFrom { get; set; }
		public int LevelFrom
		{
			get { return this._LevelFrom; }
			set
			{
				if (this._LevelFrom != value)
				{
					this._LevelFrom = value;
					this.RequestUpdate();
					this.tableView?.RequestUpdate();
				}
			}
		}
		#endregion
		#region LevelTo
		private int _LevelTo { get; set; }
		public int LevelTo
		{
			get { return this._LevelTo; }
			set
			{
				if (this._LevelTo != value)
				{
					this._LevelTo = value;
					this.RequestUpdate();
					this.tableView?.RequestUpdate();
				}
			}
		}
		#endregion

		#region ExceptExpedition
		private bool _ExceptExpedition { get; set; }
		public bool ExceptExpedition
		{
			get { return this._ExceptExpedition; }
			set
			{
				if (this._ExceptExpedition != value)
				{
					this._ExceptExpedition = value;
					this.RequestUpdate();
					this.tableView?.RequestUpdate();
				}
			}
		}
		#endregion

		#region LockFilter
		private FilterValues _LockFilter { get; set; }
		public FilterValues LockFilter
		{
			get { return this._LockFilter; }
			set
			{
				if (this._LockFilter != value)
				{
					this._LockFilter = value;
					this.RequestUpdate();
					this.tableView?.RequestUpdate();
				}
			}
		}
		#endregion
		#region SpeedFilter
		private FilterValues _SpeedFilter { get; set; }
		public FilterValues SpeedFilter
		{
			get { return this._SpeedFilter; }
			set
			{
				if (this._SpeedFilter != value)
				{
					this._SpeedFilter = value;
					this.RequestUpdate();
					this.tableView?.RequestUpdate();
				}
			}
		}
		#endregion
		#region PowerupFilter
		private FilterValues _PowerupFilter { get; set; }
		public FilterValues PowerupFilter
		{
			get { return this._PowerupFilter; }
			set
			{
				if (this._PowerupFilter != value)
				{
					this._PowerupFilter = value;
					this.RequestUpdate();
					this.tableView?.RequestUpdate();
				}
			}
		}
		#endregion

		private TableView tableView { get; }

		public ShipListTable()
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

			homeport.Organization.PropertyEvent(nameof(homeport.Organization.Ships), () => tableView.RequestUpdate(), true);
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

			public Homeport homeport => Owner?.homeport;

			public int[] ShipTypes => Owner?.ShipTypes;
			public int LevelFrom => Owner?.LevelFrom ?? 2;
			public int LevelTo => Owner?.LevelTo ?? 155;
			public bool ExceptExpedition => Owner?.ExceptExpedition ?? false;
			public FilterValues LockFilter => Owner?.LockFilter ?? FilterValues.Both;
			public FilterValues SpeedFilter => Owner?.SpeedFilter ?? FilterValues.Both;
			public FilterValues PowerupFilter => Owner?.PowerupFilter ?? FilterValues.Both;


			private Size LatestSize { get; set; } = new Size(640, 480);

			private ShipListTable Owner { get; }

			public TableView(ShipListTable Owner)
			{
				SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);
				this.Owner = Owner;

				this.Resize += (s, e) => this.RequestUpdate();
				this.Paint += (s, e) =>
				{
					var g = e.Graphics;
					var Width = this.Width - this.Padding.Left - this.Padding.Right;
					var Height = this.Height - this.Padding.Top - this.Padding.Bottom;

					g.Clear(this.BackColor);

					#region Filtering
					var Ships = homeport?.Organization?.Ships.Select(_ => _.Value);
					if (Ships != null)
					{
						if (ShipTypes != null)
							Ships = Ships.Where(_ => this.ShipTypes.Contains(_.Info.ShipType.Id));

						Ships = Ships.Where(_ =>
							(_.Level >= this.LevelFrom) && (_.Level <= this.LevelTo)
						);

						if (this.ExceptExpedition)
							Ships = Ships.Where(_ =>
								(_.FleetId == -1) || ((!homeport?.Organization?.Fleets[_.FleetId]?.Expedition?.IsInExecution) ?? false)
							);

						if (this.LockFilter == FilterValues.Yes)
							Ships = Ships.Where(_ => _.IsLocked);
						else if (this.LockFilter == FilterValues.No)
							Ships = Ships.Where(_ => !_.IsLocked);

						if (this.SpeedFilter == FilterValues.Fast)
							Ships = Ships.Where(_ => _.Info.Speed == ShipSpeed.Fast);
						else if (this.SpeedFilter == FilterValues.Slow)
							Ships = Ships.Where(_ => _.Info.Speed == ShipSpeed.Slow);

						if (this.PowerupFilter == FilterValues.End)
							Ships = Ships.Where(_ => _.IsMaxModernized);
						else if (this.PowerupFilter == FilterValues.NotEnd)
							Ships = Ships.Where(_ => !_.IsMaxModernized);
					}
					Ships = Ships?.OrderByDescending(_ => _.Exp);
					#endregion

					Font font7 = new Font(this.Font.FontFamily, 7);

					int widthLevelLv = TextRenderer.MeasureText("Lv.", font7).Width - 4;
					int widthLevelNext = TextRenderer.MeasureText("Next:", font7).Width - 3;
					int widthLevelInfo = widthLevelLv + widthLevelNext + 6;
					int widthPowerupMax = TextRenderer.MeasureText("MAX", font7).Width - 3;

					#region Cell Size Calculate
					ColumnSizes[1] = Ships?.Count() > 0 ? (Ships?.Max(_ => TextRenderer.MeasureText(_.Id.ToString(), this.Font).Width) ?? 0) : 0;
					ColumnSizes[2] = Ships?.Count() > 0 ? (Ships?.Max(_ => TextRenderer.MeasureText(_.Info.ShipType.Name, this.Font).Width) ?? 0) : 0;
					ColumnSizes[3] = Ships?.Count() > 0 ? (Ships?.Max(_ => TextRenderer.MeasureText(_.Info.Name, this.Font).Width) ?? 0) : 0;
					ColumnSizes[4] = Ships?.Count() > 0 ? (Ships?.Max(_ =>
						TextRenderer.MeasureText(_.Level.ToString(), this.Font).Width
						+ TextRenderer.MeasureText(_.ExpForNextLevel.ToString(), font7).Width
						+ widthLevelInfo
					) ?? 0) : 0;
					ColumnSizes[8] = Ships?.Count() > 0 ? (Ships?.Max(_ =>
						TextRenderer.MeasureText(_.Firepower.Current.ToString(), this.Font).Width
						+ widthPowerupMax
					) ?? 0) : 0;
					ColumnSizes[9] = Ships?.Count() > 0 ? (Ships?.Max(_ =>
						TextRenderer.MeasureText(_.Torpedo.Current.ToString(), this.Font).Width
						+ widthPowerupMax
					) ?? 0) : 0;
					ColumnSizes[10] = Ships?.Count() > 0 ? (Ships?.Max(_ =>
						TextRenderer.MeasureText(_.YasenFp.Current.ToString(), this.Font).Width
						+ widthPowerupMax
					) ?? 0) : 0;
					ColumnSizes[11] = Ships?.Count() > 0 ? (Ships?.Max(_ =>
						TextRenderer.MeasureText(_.Armor.Current.ToString(), this.Font).Width
						+ widthPowerupMax
					) ?? 0) : 0;
					ColumnSizes[12] = Ships?.Count() > 0 ? (Ships?.Max(_ =>
						TextRenderer.MeasureText(_.AA.Current.ToString(), this.Font).Width
						+ widthPowerupMax
					) ?? 0) : 0;
					ColumnSizes[13] = Ships?.Count() > 0 ? (Ships?.Max(_ =>
						TextRenderer.MeasureText(_.ASW.Current.ToString(), this.Font).Width
						+ widthPowerupMax
					) ?? 0) : 0;
					ColumnSizes[14] = Ships?.Count() > 0 ? (Ships?.Max(_ =>
						TextRenderer.MeasureText(_.Luck.Current.ToString(), this.Font).Width
						+ widthPowerupMax
					) ?? 0) : 0;

					ColumnSizes[0] = Math.Max(
						TextRenderer.MeasureText(Ships?.Count().ToString(), this.Font).Width,
						16
					) + 8;
					ColumnSizes[1] = Math.Max(ColumnSizes[1], TextRenderer.MeasureText(Headers[1], this.Font).Width) + 8;
					ColumnSizes[2] = Math.Max(ColumnSizes[2], TextRenderer.MeasureText(Headers[2], this.Font).Width) + 8;
					ColumnSizes[3] = Math.Max(ColumnSizes[3], TextRenderer.MeasureText(Headers[3], this.Font).Width) + 8;
					ColumnSizes[4] = Math.Max(ColumnSizes[4], TextRenderer.MeasureText(Headers[4], this.Font).Width) + 8;
					ColumnSizes[5] = TextRenderer.MeasureText(Headers[5], this.Font).Width + 8;
					ColumnSizes[6] = TextRenderer.MeasureText(Headers[6], this.Font).Width + 8;
					ColumnSizes[7] = TextRenderer.MeasureText(Headers[7], this.Font).Width + 8;
					ColumnSizes[8] = Math.Max(ColumnSizes[8], TextRenderer.MeasureText(Headers[8], this.Font).Width) + 8;
					ColumnSizes[9] = Math.Max(ColumnSizes[9], TextRenderer.MeasureText(Headers[9], this.Font).Width) + 8;
					ColumnSizes[10] = Math.Max(ColumnSizes[10], TextRenderer.MeasureText(Headers[10], this.Font).Width) + 8;
					ColumnSizes[11] = Math.Max(ColumnSizes[11], TextRenderer.MeasureText(Headers[11], this.Font).Width) + 8;
					ColumnSizes[12] = Math.Max(ColumnSizes[12], TextRenderer.MeasureText(Headers[12], this.Font).Width) + 8;
					ColumnSizes[13] = Math.Max(ColumnSizes[13], TextRenderer.MeasureText(Headers[13], this.Font).Width) + 8;
					ColumnSizes[14] = Math.Max(ColumnSizes[14], TextRenderer.MeasureText(Headers[14], this.Font).Width) + 8;
					ColumnSizes[15] = TextRenderer.MeasureText(Headers[15], this.Font).Width + 8;
					ColumnSizes[16] = TextRenderer.MeasureText(Headers[16], this.Font).Width + 8;
					ColumnSizes[17] = Math.Max(
						TextRenderer.MeasureText("00:00:00", this.Font).Width,
						TextRenderer.MeasureText(Headers[17], this.Font).Width
					) + 8;
					#endregion

					int x, y = 0;
					using (Pen p = new Pen(Color.FromArgb(83, 83, 83), 1.0f))
					{
						#region Data Rendering
						if (Ships != null)
						{
							Color colorWhite = Color.White;
							Color colorDarkGray = Color.FromArgb(0x20, 0x90, 0x90, 0x90);
							Color colorGray = Color.FromArgb(0x40, 0xC4, 0xC4, 0xC4);
							int idx = 0;
							y = 0;

							foreach (var ship in Ships)
							{
								int widthValue;
								x = 0;
								y -= 2;

								if (idx % 2 == 1)
									using (SolidBrush b = new SolidBrush(Color.FromArgb(0x18, 0x90, 0x90, 0x90)))
										g.FillRectangle(b, new Rectangle(0, y, Width, RowSize));

								#region Index
								TextRenderer.DrawText(
									g,
									(idx + 1).ToString(),
									this.Font,
									new Rectangle(x + 3, y, ColumnSizes[0] - 8, RowSize),
									colorDarkGray,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								x += ColumnSizes[0];
								#endregion

								#region Ship ID
								TextRenderer.DrawText(
									g,
									ship.Id.ToString(),
									this.Font,
									new Rectangle(x + 3, y, ColumnSizes[1] - 8, RowSize),
									colorGray,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								x += ColumnSizes[1];
								#endregion

								#region Ship Type Name
								TextRenderer.DrawText(
									g,
									ship.Info.ShipType.Name,
									this.Font,
									new Rectangle(x + 3, y, ColumnSizes[2] - 8, RowSize),
									colorGray,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								x += ColumnSizes[2];
								#endregion

								#region Ship Name
								TextRenderer.DrawText(
									g,
									ship.Info.Name,
									this.Font,
									new Rectangle(x + 3, y, ColumnSizes[3] - 8, RowSize),
									colorWhite,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								x += ColumnSizes[3];
								#endregion

								#region Ship Level
								widthValue = TextRenderer.MeasureText(ship.Level.ToString(), this.Font).Width;
								TextRenderer.DrawText(
									g,
									"Lv.",
									font7,
									new Rectangle(x + 3, y, ColumnSizes[4] - 8, RowSize - 3),
									colorGray,
									TextFormatFlags.Bottom | TextFormatFlags.Left
								);
								TextRenderer.DrawText(
									g,
									ship.Level.ToString(),
									this.Font,
									new Rectangle(x + widthLevelLv + 2 + 3, y, ColumnSizes[4] - 8 - widthLevelLv - 2, RowSize - 3),
									colorWhite,
									TextFormatFlags.Bottom | TextFormatFlags.Left
								);
								TextRenderer.DrawText(
									g,
									"Next:",
									font7,
									new Rectangle(
										x + widthLevelLv + 2 + widthValue + 2 + 3,
										y,
										ColumnSizes[4] - 8 - widthLevelLv - 2 - widthValue - 4,
										RowSize - 3
									),
									colorGray,
									TextFormatFlags.Bottom | TextFormatFlags.Left
								);
								TextRenderer.DrawText(
									g,
									ship.ExpForNextLevel.ToString(),
									font7,
									new Rectangle(
										x + widthLevelLv + 2 + widthValue + 2 + widthLevelNext + 3,
										y,
										ColumnSizes[4] - 8 - widthLevelLv - 2 - widthValue - 4 - widthLevelNext - 3,
										RowSize - 3
									),
									colorGray,
									TextFormatFlags.Bottom | TextFormatFlags.Left
								);
								x += ColumnSizes[4];
								#endregion

								#region Ship Remodel
								if (ship.RemodelLevel == -1)
								{
									using (SolidBrush b = new SolidBrush(Color.FromArgb(0x38, 0x4C, 0xD1)))
										g.FillRectangle(b, new Rectangle(x, y, ColumnSizes[5], RowSize));

									TextRenderer.DrawText(
										g,
										"개장가능",
										this.Font,
										new Rectangle(x + 3, y, ColumnSizes[5] - 8, RowSize),
										colorWhite,
										TextFormatFlags.VerticalCenter | TextFormatFlags.Left
									);
								}
								else
								{
									TextRenderer.DrawText(
										g,
										"Lv.",
										font7,
										new Rectangle(x + 3, y, ColumnSizes[5] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
									TextRenderer.DrawText(
										g,
										ship.RemodelLevel.ToString(),
										this.Font,
										new Rectangle(x + widthLevelLv + 3, y, ColumnSizes[5] - 8, RowSize),
										colorWhite,
										TextFormatFlags.VerticalCenter | TextFormatFlags.Left
									);
								}
								x += ColumnSizes[5];
								#endregion

								#region Ship Condition
								using (SolidBrush b = new SolidBrush(GetCondColor(ship.ConditionType)))
									g.FillRectangle(b, new Rectangle(x + 6, y + 7, 9, 9));
								TextRenderer.DrawText(
									g,
									ship.Condition.ToString(),
									this.Font,
									new Rectangle(x + 14 + 3, y, ColumnSizes[6] - 8, RowSize),
									colorWhite,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								x += ColumnSizes[6];
								#endregion

								#region Ship Max HP
								TextRenderer.DrawText(
									g,
									ship.HP.Maximum.ToString(),
									this.Font,
									new Rectangle(x + 3, y, ColumnSizes[7] - 8, RowSize),
									colorWhite,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								x += ColumnSizes[7];
								#endregion

								#region Firepower
								widthValue = TextRenderer.MeasureText(ship.Firepower.Current.ToString(), this.Font).Width - 4;
								if (ship.Firepower.IsMax)
									using (SolidBrush b = new SolidBrush(Color.FromArgb(0x7F, 0xB1, 0x3B, 0x2A)))
										g.FillRectangle(b, new Rectangle(x, y, ColumnSizes[8], RowSize));
								TextRenderer.DrawText(
									g,
									ship.Firepower.Current.ToString(),
									this.Font,
									new Rectangle(x + 3, y, ColumnSizes[8] - 8, RowSize),
									colorWhite,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								if (ship.Firepower.IsMax)
									TextRenderer.DrawText(
										g,
										"MAX",
										font7,
										new Rectangle(x + widthValue + 2 + 3, y, ColumnSizes[8] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
								else
									TextRenderer.DrawText(
										g,
										"+" + ship.Firepower.Shortfall.ToString(),
										font7,
										new Rectangle(x + widthValue + 3, y, ColumnSizes[8] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
								x += ColumnSizes[8];
								#endregion

								#region Torpedo
								widthValue = TextRenderer.MeasureText(ship.Torpedo.Current.ToString(), this.Font).Width - 4;
								if (ship.Torpedo.IsMax)
									using (SolidBrush b = new SolidBrush(Color.FromArgb(0x7F, 0x29, 0x70, 0xAB)))
										g.FillRectangle(b, new Rectangle(x, y, ColumnSizes[9], RowSize));
								TextRenderer.DrawText(
									g,
									ship.Torpedo.Current.ToString(),
									this.Font,
									new Rectangle(x + 3, y, ColumnSizes[9] - 8, RowSize),
									colorWhite,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								if (ship.Torpedo.IsMax)
									TextRenderer.DrawText(
										g,
										"MAX",
										font7,
										new Rectangle(x + widthValue + 2 + 3, y, ColumnSizes[9] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
								else
									TextRenderer.DrawText(
										g,
										"+" + ship.Torpedo.Shortfall.ToString(),
										font7,
										new Rectangle(x + widthValue + 3, y, ColumnSizes[9] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
								x += ColumnSizes[9];
								#endregion

								#region Firepower + Torpedo
								widthValue = TextRenderer.MeasureText(ship.YasenFp.Current.ToString(), this.Font).Width - 4;
								if (ship.YasenFp.IsMax)
									using (SolidBrush b = new SolidBrush(Color.FromArgb(0x66, 0x33, 0x99)))
										g.FillRectangle(b, new Rectangle(x, y, ColumnSizes[10], RowSize));
								TextRenderer.DrawText(
									g,
									ship.YasenFp.Current.ToString(),
									this.Font,
									new Rectangle(x + 3, y, ColumnSizes[10] - 8, RowSize),
									colorWhite,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								if (ship.YasenFp.IsMax)
									TextRenderer.DrawText(
										g,
										"MAX",
										font7,
										new Rectangle(x + widthValue + 2 + 3, y, ColumnSizes[10] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
								else
									TextRenderer.DrawText(
										g,
										"+" + ship.YasenFp.Shortfall.ToString(),
										font7,
										new Rectangle(x + widthValue + 3, y, ColumnSizes[10] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
								x += ColumnSizes[10];
								#endregion

								#region Armor
								widthValue = TextRenderer.MeasureText(ship.Armor.Current.ToString(), this.Font).Width - 4;
								if (ship.Armor.IsMax)
									using (SolidBrush b = new SolidBrush(Color.FromArgb(0x7F, 0xD8, 0x98, 0x1A)))
										g.FillRectangle(b, new Rectangle(x, y, ColumnSizes[11], RowSize));
								TextRenderer.DrawText(
									g,
									ship.Armor.Current.ToString(),
									this.Font,
									new Rectangle(x + 3, y, ColumnSizes[11] - 8, RowSize),
									colorWhite,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								if (ship.Armor.IsMax)
									TextRenderer.DrawText(
										g,
										"MAX",
										font7,
										new Rectangle(x + widthValue + 2 + 3, y, ColumnSizes[11] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
								else
									TextRenderer.DrawText(
										g,
										"+" + ship.Armor.Shortfall.ToString(),
										font7,
										new Rectangle(x + widthValue + 3, y, ColumnSizes[11] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
								x += ColumnSizes[11];
								#endregion

								#region AA
								widthValue = TextRenderer.MeasureText(ship.AA.Current.ToString(), this.Font).Width - 4;
								if (ship.AA.IsMax)
									using (SolidBrush b = new SolidBrush(Color.FromArgb(0x7F, 0xDF, 0x6A, 0x0C)))
										g.FillRectangle(b, new Rectangle(x, y, ColumnSizes[12], RowSize));
								TextRenderer.DrawText(
									g,
									ship.AA.Current.ToString(),
									this.Font,
									new Rectangle(x + 3, y, ColumnSizes[12] - 8, RowSize),
									colorWhite,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								if (ship.AA.IsMax)
									TextRenderer.DrawText(
										g,
										"MAX",
										font7,
										new Rectangle(x + widthValue + 2 + 3, y, ColumnSizes[12] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
								else
									TextRenderer.DrawText(
										g,
										"+" + ship.AA.Shortfall.ToString(),
										font7,
										new Rectangle(x + widthValue + 3, y, ColumnSizes[12] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
								x += ColumnSizes[12];
								#endregion

								#region ASW
								widthValue = TextRenderer.MeasureText(ship.ASW.Current.ToString(), this.Font).Width - 4;
								if (ship.ASW.IsMax)
									using (SolidBrush b = new SolidBrush(Color.FromArgb(0x8B, 0xA2, 0xB0)))
										g.FillRectangle(b, new Rectangle(x, y, ColumnSizes[13], RowSize));
								TextRenderer.DrawText(
									g,
									ship.ASW.Current.ToString(),
									this.Font,
									new Rectangle(x + 3, y, ColumnSizes[13] - 8, RowSize),
									colorWhite,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								if (ship.ASW.IsMax)
									TextRenderer.DrawText(
										g,
										"MAX",
										font7,
										new Rectangle(x + widthValue + 2 + 3, y, ColumnSizes[13] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
								else
									TextRenderer.DrawText(
										g,
										"+" + ship.ASW.Shortfall.ToString(),
										font7,
										new Rectangle(x + widthValue + 3, y, ColumnSizes[13] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
								x += ColumnSizes[13];
								#endregion

								#region Luck
								widthValue = TextRenderer.MeasureText(ship.Luck.Current.ToString(), this.Font).Width - 4;
								if (ship.Luck.IsMax)
									using (SolidBrush b = new SolidBrush(Color.FromArgb(0x7F, 0x80, 0x80, 0x80)))
										g.FillRectangle(b, new Rectangle(x, y, ColumnSizes[14], RowSize));
								TextRenderer.DrawText(
									g,
									ship.Luck.Current.ToString(),
									this.Font,
									new Rectangle(x + 3, y, ColumnSizes[14] - 8, RowSize),
									colorWhite,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								if (ship.Luck.IsMax)
									TextRenderer.DrawText(
										g,
										"MAX",
										font7,
										new Rectangle(x + widthValue + 2 + 3, y, ColumnSizes[14] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
								else
									TextRenderer.DrawText(
										g,
										"+" + ship.Luck.Shortfall.ToString(),
										font7,
										new Rectangle(x + widthValue + 3, y, ColumnSizes[14] - 8, RowSize - 3),
										colorGray,
										TextFormatFlags.Bottom | TextFormatFlags.Left
									);
								x += ColumnSizes[14];
								#endregion

								#region View Range
								TextRenderer.DrawText(
									g,
									ship.ViewRange.ToString(),
									this.Font,
									new Rectangle(x + 3, y, ColumnSizes[15] - 8, RowSize),
									colorWhite,
									TextFormatFlags.VerticalCenter | TextFormatFlags.Left
								);
								x += ColumnSizes[15];
								#endregion

								#region Sally Area
								if (ship.SallyArea > 0)
								{
									using (SolidBrush b = new SolidBrush(SallyArea.SallyAreaColor(ship.SallyArea)))
										g.FillRectangle(b, new Rectangle(x, y, ColumnSizes[16], RowSize));

									TextRenderer.DrawText(
										g,
										SallyArea.SallyAreaName(ship.SallyArea),
										this.Font,
										new Rectangle(x + 3, y, ColumnSizes[16] - 8, RowSize),
										colorWhite,
										TextFormatFlags.VerticalCenter | TextFormatFlags.Left
									);
								}
								x += ColumnSizes[16];
								#endregion

								#region Repair Time
								if ((ship?.TimeToRepair ?? TimeSpan.Zero) != TimeSpan.Zero)
								{
									TextRenderer.DrawText(
										g,
										ship.RepairTimeString,
										this.Font,
										new Rectangle(x + 3, y, ColumnSizes[17] - 8, RowSize),
										colorWhite,
										TextFormatFlags.VerticalCenter | TextFormatFlags.Left
									);
								}
								x += ColumnSizes[17];
								#endregion

								g.DrawLine(p, 0, y + RowSize, Width, y + RowSize);

								y += RowSize + 3;
								idx++;
							}
						}
						#endregion
					}

					var ResultSize = new Size(
						ColumnSizes.Sum() + this.Padding.Left + this.Padding.Right + 1,
						((Ships?.Count() ?? 0) * (RowSize + 1)) + this.Padding.Top + this.Padding.Bottom + 1
					);
					if (ResultSize.Width != LatestSize.Width || ResultSize.Height != LatestSize.Height)
					{
						LatestSize = ResultSize;
						this.PerformAutoScale();
						this.PerformLayout();
						this.Owner.RequestUpdate();
					}
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
			public override Size GetPreferredSize(Size proposedSize) => LatestSize;

			private Color GetCondColor(ConditionType condType)
			{
				switch (condType)
				{
					case ConditionType.Brilliant:
						return Color.FromArgb(0xFF, 0xFF, 0x40);
					case ConditionType.Tired:
						return Color.FromArgb(0xFF, 0xC8, 0x80);
					case ConditionType.OrangeTired:
						return Color.FromArgb(0xFF, 0x80, 0x20);
					case ConditionType.RedTired:
						return Color.FromArgb(0xFF, 0x20, 0x20);
					default:
						return Color.White;
				}
			}
		} // class
	}
}
