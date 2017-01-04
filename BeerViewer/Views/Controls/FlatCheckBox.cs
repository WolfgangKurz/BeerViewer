using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BeerViewer.Views.Controls
{
	[ToolboxBitmap(typeof(CheckBox))]
	public class FlatCheckBox : CheckBox
	{
		private int boxSize => 14;
		private bool isHovered = false;
		private bool isPressed = false;
		private bool isFocused = false;

		public FlatCheckBox()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor |
					 ControlStyles.ResizeRedraw |
					 ControlStyles.UserPaint, true);
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			base.OnPaintBackground(pevent);
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			try { OnPaintForeground(e); }
			catch { Invalidate(); }
		}

		protected virtual void OnPaintForeground(PaintEventArgs e)
		{
			var g = e.Graphics;
			Color BorderColor, ForeColor;

			g.Clear(BackColor);

			if (!Enabled)
			{
				ForeColor = Color.FromArgb(0x98, 0x98, 0x98);
				BorderColor = Color.FromArgb(0x98, 0x98, 0x98);
			}
			else
			{
				if (isHovered)
				{
					ForeColor = Color.FromArgb(0x51, 0x75, 0x8E);
					BorderColor = Color.FromArgb(0x51, 0x75, 0x8E);
				}
				else
				{
					ForeColor = Color.FromArgb(0xFF, 0xFF, 0xFF);
					BorderColor = Color.FromArgb(0x44, 0x44, 0x4A);
				}
			}

			Rectangle textRect = new Rectangle(16, 0, Width - 16, Height);
			Rectangle boxRect = new Rectangle(0, Height / 2 - 6, 12, 12);
			using (Pen p = new Pen(BorderColor))
			{
				switch (CheckAlign)
				{
					case ContentAlignment.TopLeft:
						boxRect = new Rectangle(0, 0, boxSize, boxSize);
						break;
					case ContentAlignment.MiddleLeft:
						boxRect = new Rectangle(0, Height / 2 - (boxSize / 2), boxSize, boxSize);
						break;
					case ContentAlignment.BottomLeft:
						boxRect = new Rectangle(0, Height - (boxSize + 1), boxSize, boxSize);
						break;
					case ContentAlignment.TopCenter:
						boxRect = new Rectangle(Width / 2 - (boxSize / 2), 0, boxSize, boxSize);
						textRect = new Rectangle((boxSize + 4), boxRect.Top + boxRect.Height - (boxSize + 1), Width - (boxSize + 4) / 2, Height);
						break;
					case ContentAlignment.BottomCenter:
						boxRect = new Rectangle(Width / 2 - (boxSize / 2), Height - (boxSize + 1), boxSize, boxSize);
						textRect = new Rectangle((boxSize + 4), -10, Width - (boxSize + 4) / 2, Height);
						break;
					case ContentAlignment.MiddleCenter:
						boxRect = new Rectangle(Width / 2 - (boxSize / 2), Height / 2 - (boxSize / 2), boxSize, boxSize);
						break;
					case ContentAlignment.TopRight:
						boxRect = new Rectangle(Width - (boxSize + 1), 0, boxSize, boxSize);
						textRect = new Rectangle(0, 0, Width - (boxSize + 4), Height);
						break;
					case ContentAlignment.MiddleRight:
						boxRect = new Rectangle(Width - (boxSize + 1), Height / 2 - (boxSize / 2), boxSize, boxSize);
						textRect = new Rectangle(0, 0, Width - (boxSize + 4), Height);
						break;
					case ContentAlignment.BottomRight:
						boxRect = new Rectangle(Width - (boxSize + 1), Height - (boxSize + 1), boxSize, boxSize);
						textRect = new Rectangle(0, 0, Width - (boxSize + 4), Height);
						break;
				}

				g.DrawRectangle(p, boxRect);
			}

			Color fillColor;
			if (isPressed)
				fillColor = Color.FromArgb(0x51, 0x75, 0x8E);
			else
				fillColor = Color.FromArgb(0x27, 0x27, 0x2F);

			using (SolidBrush b = new SolidBrush(fillColor))
			{
				Rectangle boxCheck = new Rectangle(boxRect.Left + 1, boxRect.Top + 1, boxSize - 1, boxSize - 1);
				g.FillRectangle(b, boxCheck);
			}

			if (Checked)
			{
				using (SolidBrush b = new SolidBrush(Color.FromArgb(0xC6, 0xC6, 0xC6)))
				{
					if (CheckState == CheckState.Indeterminate)
					{
						Rectangle boxCheck = new Rectangle(boxRect.Left + 3, boxRect.Top + 3, boxSize - 5, boxSize - 5);
						g.FillRectangle(b, boxCheck);
					}
					else
					{
						g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

						g.FillPolygon(
							b,
							new Point[] {
								new Point(boxRect.Left+4, boxRect.Top+6),
								new Point(boxRect.Left+2, boxRect.Top+7),
								new Point(boxRect.Left+6, boxRect.Top+11),
								new Point(boxRect.Left+7, boxRect.Top+11),
								new Point(boxRect.Left+12, boxRect.Top+4),
								new Point(boxRect.Left+11, boxRect.Top+3),
								new Point(boxRect.Left+10, boxRect.Top+3),
								new Point(boxRect.Left+7, boxRect.Top+8),
								new Point(boxRect.Left+6, boxRect.Top+8),
							}
						);

						g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
					}
				}
			}

			TextRenderer.DrawText(
				g,
				Text,
				this.Font,
				textRect,
				ForeColor,
				GetTextFormatFlags(TextAlign)
			);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			isFocused = true;
			isHovered = true;
			Invalidate();

			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			isFocused = false;
			isHovered = false;
			isPressed = false;
			Invalidate();

			base.OnLostFocus(e);
		}

		protected override void OnEnter(EventArgs e)
		{
			isFocused = true;
			isHovered = true;
			Invalidate();

			base.OnEnter(e);
		}

		protected override void OnLeave(EventArgs e)
		{
			isFocused = false;
			isHovered = false;
			isPressed = false;
			Invalidate();

			base.OnLeave(e);
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
			//Remove this code cause this prevents the focus color
			//isHovered = false;
			//isPressed = false;
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
			//This will check if control got the focus
			//If not thats the only it will remove the focus color
			if (!isFocused)
			{
				isHovered = false;
			}
			Invalidate();

			base.OnMouseLeave(e);
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			Invalidate();
		}

		protected override void OnCheckedChanged(EventArgs e)
		{
			base.OnCheckedChanged(e);
			Invalidate();
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			Size preferredSize;
			using (var g = CreateGraphics())
			{
				proposedSize = new Size(int.MaxValue, int.MaxValue);
				preferredSize = TextRenderer.MeasureText(
					g,
					Text,
					this.Font,
					proposedSize,
					GetTextFormatFlags(TextAlign)
				);
				preferredSize.Width += 16;

				if (CheckAlign == ContentAlignment.TopCenter || CheckAlign == ContentAlignment.BottomCenter)
					preferredSize.Height += 16;
			}

			return preferredSize;
		}

		public static TextFormatFlags GetTextFormatFlags(ContentAlignment textAlign)
		{
			TextFormatFlags controlFlags = TextFormatFlags.Default;

			switch (textAlign)
			{
				case ContentAlignment.TopLeft:
					controlFlags |= TextFormatFlags.Top | TextFormatFlags.Left;
					break;
				case ContentAlignment.TopCenter:
					controlFlags |= TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
					break;
				case ContentAlignment.TopRight:
					controlFlags |= TextFormatFlags.Top | TextFormatFlags.Right;
					break;

				case ContentAlignment.MiddleLeft:
					controlFlags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
					break;
				case ContentAlignment.MiddleCenter:
					controlFlags |= TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
					break;
				case ContentAlignment.MiddleRight:
					controlFlags |= TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
					break;

				case ContentAlignment.BottomLeft:
					controlFlags |= TextFormatFlags.Bottom | TextFormatFlags.Left;
					break;
				case ContentAlignment.BottomCenter:
					controlFlags |= TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
					break;
				case ContentAlignment.BottomRight:
					controlFlags |= TextFormatFlags.Bottom | TextFormatFlags.Right;
					break;
			}
			return controlFlags;
		}
	}
}
