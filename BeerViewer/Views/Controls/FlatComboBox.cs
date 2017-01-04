using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace BeerViewer.Views.Controls
{
	[ToolboxBitmap(typeof(ComboBox))]
	public class FlatComboBox : ComboBox
	{
		public class NameValueDisplayItem
		{
			public virtual string Name { get; set; }
			public virtual string Value { get; set; }

			public override string ToString() => $"{Name} : {Value}";
		}

		#region UseSelectable (Ignore Value)
		[Browsable(false)]
		[Category("Behaviour")]
		[DefaultValue(false)]
		public bool UseSelectable
		{
			get { return GetStyle(ControlStyles.Selectable); }
			set { }
		}
		#endregion

		#region DrawMode (Ignore Value)
		[DefaultValue(DrawMode.OwnerDrawVariable)]
		[Browsable(false)]
		public new DrawMode DrawMode
		{
			get { return DrawMode.OwnerDrawVariable; }
			set { base.DrawMode = DrawMode.OwnerDrawVariable; }
		}
		#endregion

		#region DropDownStyle (Ignore Value)
		[DefaultValue(ComboBoxStyle.DropDownList)]
		[Browsable(false)]
		public new ComboBoxStyle DropDownStyle
		{
			get { return ComboBoxStyle.DropDownList; }
			set { base.DropDownStyle = ComboBoxStyle.DropDownList; }
		}
		#endregion

		#region BackColor (Hide Value)
		[Browsable(false)]
		public new Color BackColor
		{
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}
		#endregion

		#region ForeColor (Hide Value)
		[Browsable(false)]
		public new Color ForeColor
		{
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		#endregion

		#region PromptText
		private string _PromptText = "";
		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DefaultValue("")]
		[Category("Appearance")]
		public string PromptText
		{
			get { return _PromptText; }
			set
			{
				_PromptText = value.Trim();
				Invalidate();
			}
		}
		#endregion

		private bool isHovered = false;
		private bool isPressed = false;
		private bool isDropdowned = false;
		private bool DrawPrompt => SelectedIndex == -1;

		public FlatComboBox()
		{
			this.BackColor = Color.FromArgb(0x27, 0x27, 0x2f);

			SetStyle(ControlStyles.SupportsTransparentBackColor |
					 ControlStyles.OptimizedDoubleBuffer |
					 ControlStyles.ResizeRedraw |
					 ControlStyles.AllPaintingInWmPaint |
					 ControlStyles.UserPaint, true);

			SetStyle(ControlStyles.Selectable |
					 ControlStyles.StandardClick, false);

			base.DrawMode = DrawMode.OwnerDrawVariable;
			base.DropDownStyle = ComboBoxStyle.DropDownList;
		}

		protected override void OnDropDown(EventArgs e)
		{
			this.DropDownHeight = (GetPreferredSize(Size.Empty).Height + 6) * Math.Min(15, this.Items.Count) + 2;

			isDropdowned = true;
			Invalidate();
		}
		protected override void OnDropDownClosed(EventArgs e)
		{
			isDropdowned = false;
			if (!this.ClientRectangle.Contains(this.PointToClient(Control.MousePosition)))
			{
				isHovered = false;
				this.Capture = true;
				base.OnMouseLeave(e);
			}

			Invalidate();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			try { e.Graphics.Clear(BackColor); }
			catch { Invalidate(); }
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				if (GetStyle(ControlStyles.AllPaintingInWmPaint))
					OnPaintBackground(e);

				OnPaintForeground(e);
			}
			catch { Invalidate(); }
		}

		protected virtual void OnPaintForeground(PaintEventArgs e)
		{
			var g = e.Graphics;

			Rectangle boxRect = new Rectangle(0, 0, Width - 1, Height - 1);
			Color ForeColor = Color.FromArgb(0xFF, 0xFF, 0xFF);
			Color BorderColor = Color.FromArgb(0x44, 0x44, 0x4A);
			Color BackColor = GetBackColor();
			Color CaretColor = Color.FromArgb(0x83, 0x83, 0x83);
			Color CaretBackColor = BackColor;

			if (!Enabled)
				ForeColor = Color.FromArgb(0x98, 0x98, 0x98);

			using (SolidBrush b = new SolidBrush(BackColor))
				g.FillRectangle(b, boxRect);

			using (Pen p = new Pen(BorderColor, 1.0f))
				g.DrawRectangle(p, boxRect);

			if (Enabled)
			{
				if ((isHovered && isPressed) || this.isDropdowned)
				{
					CaretBackColor = Color.FromArgb(0x51, 0x75, 0x8E);
					CaretColor = Color.FromArgb(0xFF, 0xFF, 0xFF);
				}
				else if (isHovered)
				{
					CaretBackColor = Color.FromArgb(0x27, 0x27, 0x2F);
					CaretColor = Color.FromArgb(0x51, 0x75, 0x8E);
				}
			}

			using (SolidBrush b = new SolidBrush(CaretBackColor))
				g.FillRectangle(
					b,
					new Rectangle(Width - 20, 1, 19, Height - 2)
				);
			using (SolidBrush b = new SolidBrush(CaretColor)) // ▼
				g.FillPolygon(
					b,
					new Point[] {
						new Point(Width - 14, (Height / 2) - 2),
						new Point(Width - 7, (Height / 2) - 2),
						new Point(Width - 11, (Height / 2) + 2)
					}
				);

			Rectangle textRect = new Rectangle(2, 2, Width - 20, Height - 4);
			if (this.SelectedItem is NameValueDisplayItem)
			{
				Rectangle rect = new Rectangle(textRect.Left, textRect.Top, textRect.Width, textRect.Bottom - 4);
				var item = this.SelectedItem as NameValueDisplayItem;

				TextRenderer.DrawText(
					g,
					item.Name + " :",
					new Font(this.Font.FontFamily, 8),
					rect,
					ForeColor,
					TextFormatFlags.Left | TextFormatFlags.Bottom
				);
				TextRenderer.DrawText(
					g,
					item.Value,
					new Font(this.Font.FontFamily, 9),
					rect,
					ForeColor,
					TextFormatFlags.Right | TextFormatFlags.Bottom
				);
			}
			else
			{
				TextRenderer.DrawText(
					g,
					Text,
					this.Font,
					textRect,
					ForeColor,
					TextFormatFlags.Left | TextFormatFlags.VerticalCenter
				);
			}
			if (DrawPrompt) DrawTextPrompt(g);
		}

		protected override void OnMeasureItem(MeasureItemEventArgs e)
		{
			e.ItemHeight = GetPreferredSize(Size.Empty).Height + 6;
		}
		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (!isDropdowned) return;

			var g = e.Graphics;
			Color FocusedBackColor = Color.FromArgb(0x57, 0x57, 0x5f);

			if (e.Index >= 0)
			{
				Color foreColor = Color.White;
				if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
				{
					using (SolidBrush b = new SolidBrush(FocusedBackColor))
						g.FillRectangle(
							b,
							new Rectangle(
								e.Bounds.Left,
								e.Bounds.Top,
								e.Bounds.Width,
								e.Bounds.Height
							)
						);
					foreColor = Color.White;
				}
				else
				{
					using (SolidBrush b = new SolidBrush(this.BackColor))
						g.FillRectangle(
							b,
							new Rectangle(
								e.Bounds.Left,
								e.Bounds.Top,
								e.Bounds.Width,
								e.Bounds.Height
							)
						);
				}

				Rectangle textRect = new Rectangle(0, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height);
				if (Items[e.Index] is NameValueDisplayItem)
				{
					Rectangle rect = new Rectangle(0, e.Bounds.Top, e.Bounds.Width, e.Bounds.Height - 4);
					var item = Items[e.Index] as NameValueDisplayItem;

					TextRenderer.DrawText(
						g,
						item.Name + " :",
						new Font(this.Font.FontFamily, 8),
						rect,
						foreColor,
						TextFormatFlags.Left | TextFormatFlags.Bottom
					);
					TextRenderer.DrawText(
						g,
						item.Value,
						new Font(this.Font.FontFamily, 9),
						rect,
						foreColor,
						TextFormatFlags.Right | TextFormatFlags.Bottom
					);
				}
				else
				{
					TextRenderer.DrawText(
						g,
						Items[e.Index].ToString(),
						new Font(this.Font.FontFamily, 10),
						textRect,
						foreColor,
						TextFormatFlags.Left | TextFormatFlags.VerticalCenter
					);
				}
			}
			else
				base.OnDrawItem(e);
		}

		private void DrawTextPrompt()
		{
			using (Graphics graphics = CreateGraphics())
				DrawTextPrompt(graphics);
		}

		private void DrawTextPrompt(Graphics g)
		{
			Rectangle textRect = new Rectangle(4, 2, Width - 24, Height - 4);
			TextRenderer.DrawText(
				g,
				_PromptText,
				this.Font,
				textRect,
				SystemColors.GrayText,
				GetBackColor(),
				TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
			);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				isHovered = true;
				isPressed = true;
				Invalidate();
			}
			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			Invalidate();
			base.OnKeyUp(e);
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			isHovered = true;
			Invalidate();
			base.OnMouseEnter(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				isPressed = true;
				Invalidate();
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			isPressed = false;
			Invalidate();
			base.OnMouseUp(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			isHovered = false;
			Invalidate();
			base.OnMouseLeave(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (!this.Capture) return;
			if (!this.ClientRectangle.Contains(this.PointToClient(Control.MousePosition)))
				Capture = false;

			base.OnMouseMove(e);
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			Size preferredSize;
			base.GetPreferredSize(proposedSize);

			using (var g = CreateGraphics())
			{
				string measureText = Text.Length > 0 ? Text : "MeasureText";
				proposedSize = new Size(int.MaxValue, int.MaxValue);
				preferredSize = TextRenderer.MeasureText(g, measureText, this.Font, proposedSize, TextFormatFlags.Left | TextFormatFlags.LeftAndRightPadding | TextFormatFlags.VerticalCenter);
				preferredSize.Height += 4;
			}

			return preferredSize;
		}

		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			base.OnSelectedIndexChanged(e);
			Invalidate();
		}

		private const int OCM_COMMAND = 0x2111;
		private const int WM_PAINT = 15;

		[System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (((m.Msg == WM_PAINT) || (m.Msg == OCM_COMMAND)) && DrawPrompt)
				DrawTextPrompt();
		}

		private Color GetBackColor()
		{
			if (!Enabled)
				return Color.FromArgb(0x57, 0x57, 0x5F);
			else if (isHovered || this.isDropdowned)
				return Color.FromArgb(0x47, 0x47, 0x4F);
			else
				return Color.FromArgb(0x27, 0x27, 0x2F);
		}
	}
}
