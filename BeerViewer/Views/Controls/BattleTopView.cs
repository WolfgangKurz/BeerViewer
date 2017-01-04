using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BeerViewer.Core;
using BeerViewer.Models.BattleInfo;

namespace BeerViewer.Views.Controls
{
	public partial class BattleTopView : UserControl
	{
		#region BattleType 프로퍼티
		private string _BattleType { get; set; }
		[Browsable(false)]
		public string BattleType
		{
			get { return this._BattleType; }
			set
			{
				if (this._BattleType != value)
				{
					this._BattleType = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion
		#region BattleSituation 프로퍼티
		private BattleSituation _BattleSituation { get; set; } = BattleSituation.없음;
		[Browsable(false)]
		public BattleSituation BattleSituation
		{
			get { return this._BattleSituation; }
			set
			{
				if (this._BattleSituation != value)
				{
					this._BattleSituation = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion
		#region AirSupremacy 프로퍼티
		private AirSupremacy _AirSupremacy { get; set; } = AirSupremacy.항공전없음;
		[Browsable(false)]
		public AirSupremacy AirSupremacy
		{
			get { return this._AirSupremacy; }
			set
			{
				if (this._AirSupremacy != value)
				{
					this._AirSupremacy = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion
		#region UpdatedTime 프로퍼티
		private DateTimeOffset _UpdatedTime { get; set; } = default(DateTimeOffset);
		[Browsable(false)]
		public DateTimeOffset UpdatedTime
		{
			get { return this._UpdatedTime; }
			set
			{
				if (this._UpdatedTime != value)
				{
					this._UpdatedTime = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion
		#region DropShipName 프로퍼티
		private string _DropShipName { get; set; }
		[Browsable(false)]
		public string DropShipName
		{
			get { return this._DropShipName; }
			set
			{
				if (this._DropShipName != value)
				{
					this._DropShipName = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion
		#region SupportUsed 프로퍼티
		private UsedSupport _SupportUsed { get; set; } = UsedSupport.Unset;
		[Browsable(false)]
		public UsedSupport SupportUsed
		{
			get { return this._SupportUsed; }
			set
			{
				if (this._SupportUsed != value)
				{
					this._SupportUsed = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion
		#region FlareUsed 프로퍼티
		private UsedFlag _FlareUsed { get; set; } = UsedFlag.Unset;
		[Browsable(false)]
		public UsedFlag FlareUsed
		{
			get { return this._FlareUsed; }
			set
			{
				if (this._FlareUsed != value)
				{
					this._FlareUsed = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion
		#region NightReconScouted 프로퍼티
		private UsedFlag _NightReconScouted { get; set; } = UsedFlag.Unset;
		[Browsable(false)]
		public UsedFlag NightReconScouted
		{
			get { return this._NightReconScouted; }
			set
			{
				if (this._NightReconScouted != value)
				{
					this._NightReconScouted = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion
		#region AntiAirFired 프로퍼티
		private AirFireFlag _AntiAirFired { get; set; } = AirFireFlag.Unset;
		[Browsable(false)]
		public AirFireFlag AntiAirFired
		{
			get { return this._AntiAirFired; }
			set
			{
				if (this._AntiAirFired != value)
				{
					this._AntiAirFired = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion

		#region ResultRank 프로퍼티
		private BattleRank _ResultRank { get; set; } = BattleRank.없음;
		[Browsable(false)]
		public BattleRank ResultRank
		{
			get { return this._ResultRank; }
			set
			{
				if (this._ResultRank != value)
				{
					this._ResultRank = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion
		#region AirResultRank 프로퍼티
		private BattleRank _AirResultRank { get; set; } = BattleRank.없음;
		[Browsable(false)]
		public BattleRank AirResultRank
		{
			get { return this._AirResultRank; }
			set
			{
				if (this._AirResultRank != value)
				{
					this._AirResultRank = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion

		#region get 프로퍼티
		protected string BattleTypeText => this.BattleType ?? "";
		protected string BattleSituationText => this.BattleSituation != BattleSituation.없음
			? this.BattleSituation.ToString() : "";
		protected string AirSupremacyText => this.AirSupremacy != AirSupremacy.항공전없음
			? this.AirSupremacy.ToString() : "";

		protected string UpdatedTimeText => this.UpdatedTime != default(DateTimeOffset)
			? this.UpdatedTime.ToString("HH:mm:ss") : "--:--:--";
		protected string DropShipNameText => this.DropShipName ?? "";
		protected string SupportUsedText => this.SupportUsed.ToReadableString();

		protected string FlareUsedText => this.FlareUsed.ToReadableString();
		protected string NightReconScoutedText => this.NightReconScouted.ToReadableString();
		protected string AntiAirFiredText => this.AntiAirFired.ToReadableString();

		protected bool AirRankAvailable => this.ResultRank == BattleRank.공습전;
		#endregion

		private List<MapCellData> CellList { get; set; } = new List<MapCellData>();
		private Size LatestSize { get; set; } = new Size(400, 80);

		public BattleTopView()
		{
			InitializeComponent();

			this.Paint += (s, e) =>
			{
				var g = e.Graphics;
				var Width = this.Width - this.Padding.Left - this.Padding.Right;
				var Height = this.Height - this.Padding.Top - this.Padding.Bottom;

				var cells = new int[] { 68, 0, 68, 64, 68, 64 };

				cells[1] = Width - cells.Sum();
				cells[1] = Math.Max(cells[1], TextRenderer.MeasureText(this.BattleTypeText, this.Font).Width);
				cells[1] = Math.Max(cells[1], TextRenderer.MeasureText(this.BattleSituationText, this.Font).Width);
				cells[1] = Math.Max(cells[1], TextRenderer.MeasureText(this.AirSupremacyText, this.Font).Width);

				int h = TextRenderer.MeasureText(" ", this.Font).Height + 2;
				int x = 0, y = 1;

				Color colorGray = Color.FromArgb(0x30, 0x90, 0x90, 0x90);
				Color colorWhite = Color.White;

				x = 0;
				TextRenderer.DrawText(g, "전투종류 :", this.Font, new Rectangle(x, y, cells[0], h), colorGray, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
				TextRenderer.DrawText(g, "교전형태 :", this.Font, new Rectangle(x, y + h, cells[0], h), colorGray, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
				TextRenderer.DrawText(g, "제공종류 :", this.Font, new Rectangle(x, y + h * 2, cells[0], h), colorGray, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);

				x += cells[0] + cells[1];
				TextRenderer.DrawText(g, "갱신시간 :", this.Font, new Rectangle(x, y, cells[0], h), colorGray, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
				TextRenderer.DrawText(g, "드롭 :", this.Font, new Rectangle(x, y + h, cells[0], h), colorGray, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
				TextRenderer.DrawText(g, "지원함대 :", this.Font, new Rectangle(x, y + h * 2, cells[0], h), colorGray, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);

				x += cells[2] + cells[3];
				TextRenderer.DrawText(g, "조명탄 :", this.Font, new Rectangle(x, y, cells[0], h), colorGray, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
				TextRenderer.DrawText(g, "야간정찰 :", this.Font, new Rectangle(x, y + h, cells[0], h), colorGray, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
				TextRenderer.DrawText(g, "대공컷인 :", this.Font, new Rectangle(x, y + h * 2, cells[0], h), colorGray, TextFormatFlags.Right | TextFormatFlags.VerticalCenter);


				x = cells[0];
				TextRenderer.DrawText(g, this.BattleTypeText, this.Font, new Rectangle(x, y, cells[1], h), colorWhite, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
				TextRenderer.DrawText(g, this.BattleSituationText, this.Font, new Rectangle(x, y + h, cells[1], h), colorWhite, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
				TextRenderer.DrawText(g, this.AirSupremacyText, this.Font, new Rectangle(x, y + h * 2, cells[1], h), colorWhite, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

				x += cells[1] + cells[2];
				TextRenderer.DrawText(g, this.UpdatedTimeText, this.Font, new Rectangle(x, y, cells[3], h), colorWhite, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
				TextRenderer.DrawText(g, this.DropShipNameText, this.Font, new Rectangle(x, y + h, cells[3], h), colorWhite, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
				TextRenderer.DrawText(g, this.SupportUsedText, this.Font, new Rectangle(x, y + h * 2, cells[3], h), colorWhite, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

				x += cells[3] + cells[4];
				TextRenderer.DrawText(g, this.FlareUsedText, this.Font, new Rectangle(x, y, cells[5], h), colorWhite, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
				TextRenderer.DrawText(g, this.NightReconScoutedText, this.Font, new Rectangle(x, y + h, cells[5], h), colorWhite, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
				TextRenderer.DrawText(g, this.AntiAirFiredText, this.Font, new Rectangle(x, y + h * 2, cells[5], h), colorWhite, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

				y += h * 3 + 2 + 4;
				x = 4;
				var cellList = CellList.ToArray().Reverse(); // 최신을 처음으로
				bool isLatestCell = true;
				foreach (var cell in cellList)
				{
					var cellTypeText = GetCellTypeText(cell.CellType);
					var cellName = GetCellName(cell.CellName);

					int w, w1;
					w = w1 = TextRenderer.MeasureText(cellName, this.Font).Width;
					w += TextRenderer.MeasureText(cellTypeText, this.Font).Width;

					if (x + w + 6 >= Width - 4)
					{
						x = 4;
						y += h + 2;
					}

					Color cellColor = GetCellColor(cell.CellType);
					Brush textColor = Brushes.White;
					if (!isLatestCell) // 지난 셀들은 반투명하게
					{
						cellColor = Color.FromArgb(0x58, cellColor);
						textColor = new SolidBrush(Color.FromArgb(0x82, Color.White));
					}

					using (SolidBrush b = new SolidBrush(cellColor))
						g.FillRectangle(b, new Rectangle(x, y, w + 6, h + 2));

					g.SetClip(new Rectangle(x + 3, y + 1, w1, h));
					g.DrawString(
						cellName,
						this.Font,
						textColor,
						x + 5,
						y + 1
					);
					g.ResetClip();

					g.SetClip(new Rectangle(x + 3 + w1, y + 1, w - w1, h));
					g.DrawString(
						cellTypeText,
						this.Font,
						textColor,
						x + 3 + w1,
						y + 1
					);
					g.ResetClip();

					x += w + 6 + 3;
					isLatestCell = false;
				}
				y += h + 2 + 2;

				using (Font f = new Font(this.Font.FontFamily, 14, FontStyle.Bold))
				{
					string rankText = this.ResultRank.ToString();
					string airRankText = this.AirResultRank.ToString();
					Color rankColor = GetRankColor(this.ResultRank);
					Color airRankColor = GetRankColor(this.AirResultRank);
					Size sz = TextRenderer.MeasureText(rankText, f);
					int w, w1, w2;

					w1 = sz.Width;
					w = w1;
					if (AirRankAvailable)
					{
						w2 = TextRenderer.MeasureText(airRankText, f).Width;
						w += w2;
					}

					TextRenderer.DrawText(g, rankText, f, new Point(Width / 2 - w / 2, y + 4), rankColor);
					if (AirRankAvailable)
						TextRenderer.DrawText(g, airRankText, f, new Point(Width / 2 - w / 2 + w1, y + 4), airRankColor);

					y += sz.Height + 12;
				}

				var ResultSize = new Size(
					cells.Sum() + this.Padding.Left + this.Padding.Right,
					y + this.Padding.Top + this.Padding.Bottom
				);
				if (ResultSize.Width != LatestSize.Width || ResultSize.Height != LatestSize.Height)
				{
					LatestSize = ResultSize;
					this.PerformAutoScale();
					this.PerformLayout();
				}
			};
		}
		public override Size GetPreferredSize(Size proposedSize) => LatestSize;

		public void RequestUpdate()
		{
			if (this.InvokeRequired)
			{
				this.Invoke(RequestUpdate);
				return;
			}
			this.Invalidate();
		}

		public void ApplyCell(MapCellData cell)
		{
			if(cell.IsFirst) this.CellList.Clear();
			this.CellList.Add(cell);

			this.RequestUpdate();
		}

		private string GetCellName(string CellName)
		{
			return MapAreaData.MapAreaTable
				.SingleOrDefault(x => x.Key == CellName).Value
				?? CellName;
		}
		private Color GetCellColor(CellType CellType)
		{
			switch (CellType)
			{
				case CellType.연습전: return Color.FromArgb(0x8D, 0xC6, 0x60);

				case CellType.보급: return Color.FromArgb(0x3F, 0xBD, 0x2B);
				case CellType.소용돌이: return Color.FromArgb(0xA3, 0x3C, 0xEA);
				case CellType.상륙지점: return Color.FromArgb(0x0A, 0x8A, 0xB9);

				case CellType.전투:
				case CellType.야전: return Color.FromArgb(0xF0, 0x17, 0x17);
				case CellType.보스: return Color.FromArgb(0xD4, 0x0C, 0x0C);
				case CellType.공습전: return Color.FromArgb(0x51, 0xA6, 0xC5);

				default: return Color.FromArgb(0x60, 0x60, 0x60);
			}
		}
		private string GetCellTypeText(CellType CellType)
		{
			switch (CellType)
			{
				case CellType.연습전: return "연습전";

				case CellType.보급: return "자원획득";
				case CellType.소용돌이:return "소용돌이";
				case CellType.상륙지점: return "기분탓";

				case CellType.전투:
				case CellType.야전: return "적군조우";
				case CellType.보스: return "보스전";
				case CellType.공습전: return "공습전";

				default: return "???";
			}
		}
		private Color GetRankColor(BattleRank ResultRank)
		{
			switch (ResultRank)
			{
				case BattleRank.에러: return Color.Red;

				case BattleRank.완전승리S: return Color.FromArgb(0xFF, 0xE5, 0x58);
				case BattleRank.S승리: return Color.Gold;

				case BattleRank.A승리: return Color.FromArgb(0xD1, 0x00, 0x00);
				case BattleRank.B승리: return Color.FromArgb(0xD4, 0x33, 0x53);

				case BattleRank.C패배: return Color.FromArgb(0x42, 0x42, 0x8F);
				case BattleRank.D패배: return Color.FromArgb(0x42, 0x42, 0x8F);
				case BattleRank.E패배: return Color.FromArgb(0x42, 0x42, 0x8F);

				case BattleRank.공습전: return Color.FromArgb(0x52, 0xA6, 0x61);

				case BattleRank.없음:
				default:
					return Color.Transparent;
			}
		}
	}
}
