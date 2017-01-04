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
	public class FlatExpanderButton : Control
	{
		[Flags]
		private enum ButtonState
		{
			Normal = 0x00,
			Over = 0x01,
			Down = 0x02,
		}

		[Description("Button Text"), Category("Appearance")]
		public override string Text
		{
			get { return base.Text; }
			set
			{
				if (base.Text != value)
				{
					base.Text = value;
					this.Invalidate();
				}
			}
		}

		[Description("Text Color"), Category("Appearance")]
		public override Color ForeColor
		{
			get { return base.ForeColor; }
			set
			{
				if (this.ForeColor != value)
				{
					base.ForeColor = value;
					this.Invalidate();
				}
			}
		}

		[Description("Background Color"), Category("Appearance")]
		private Color _BackColor { get; set; }
		public new Color BackColor
		{
			get { return this._BackColor; }
			set
			{
				if (this._BackColor != value)
				{
					this._BackColor = value;
					this.Invalidate();
				}
			}
		}

		private Color _OverBackColor { get; set; }
		[Description("Over Background Color"), Category("Appearance")]
		public Color OverBackColor
		{
			get { return this._OverBackColor; }
			set
			{
				if (this._OverBackColor != value)
				{
					this._OverBackColor = value;
					this.Invalidate();
				}
			}
		}

		private Color _DownBackColor { get; set; }
		[Description("Down Background Color"), Category("Appearance")]
		public Color DownBackColor
		{
			get { return this._DownBackColor; }
			set
			{
				if (this._DownBackColor != value)
				{
					this._DownBackColor = value;
					this.Invalidate();
				}
			}
		}

		private ButtonState _State { get; set; }
		private ButtonState State
		{
			get { return this._State; }
			set
			{
				if (this._State != value)
				{
					this._State = value;
					Invalidate();
				}
			}
		}

		[Description("Enabled"), Category("Behavior")]
		public new bool Enabled
		{
			get { return base.Enabled; }
			set
			{
				if (this.Enabled != value)
				{
					base.Enabled = value;
					this.Invalidate();
				}
			}
		}

		[Description("Expanded"), Category("Behavior")]
		private bool _Expanded { get; set; }
		public bool Expanded
		{
			get { return this._Expanded; }
			set
			{
				if (this._Expanded != value)
				{
					this._Expanded = value;
					this.ExpandedChanged?.Invoke(this, new EventArgs());
					this.Invalidate();
				}
			}
		}

		public event EventHandler ExpandedChanged;

		public FlatExpanderButton()
		{
			this.DoubleBuffered = true;

			this.MouseEnter += (s, e) => this.State |= ButtonState.Over;
			this.MouseLeave += (s, e) => this.State &= ~ButtonState.Over;
			this.MouseDown += (s, e) => this.State |= ButtonState.Down;
			this.MouseUp += (s, e) => this.State &= ~ButtonState.Down;
			this.Resize += (s, e) => this.Invalidate();

			this.DoubleClick += (s, e) => this.Expanded = !this.Expanded;
			this.Click += (s, e) => this.Expanded = !this.Expanded;
			this.Paint += (s, e) =>
			{
				Graphics g = e.Graphics;
				Color backColor, textColor = this.Enabled ? this.ForeColor : Color.Gray;

				g.TranslateTransform(this.Padding.Left, this.Padding.Top);

				switch (this.State)
				{
					case ButtonState.Down | ButtonState.Over:
						backColor = DownBackColor;
						break;
					case ButtonState.Down:
					case ButtonState.Over:
						backColor = OverBackColor;
						break;
					default:
						backColor = BackColor;
						break;
				}

				Rectangle rect = this.ClientRectangle;
				rect.Width -= this.Padding.Left + this.Padding.Right;
				rect.Height -= this.Padding.Top + this.Padding.Bottom;

				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				{
					using (SolidBrush b = new SolidBrush(backColor))
						g.FillEllipse(b, new Rectangle(3, 3, 17, 17));

					using (Pen p = new Pen(textColor, 1.0f))
						g.DrawEllipse(p, new Rectangle(3, 3, 17, 17));

					using (SolidBrush b = new SolidBrush(textColor))
					{
						int x = 11;

						if (this.Expanded)
						{
							g.FillPolygon(
								b,
								new Point[] {
								new Point(x - 3, x + 2),
								new Point(x + 4, x + 2),
								new Point(x + 1, x - 2),
								new Point(x, x - 2)
								}
							);
						}
						else
						{
							g.FillPolygon(
								b,
								new Point[] {
								new Point(x - 3, x - 1),
								new Point(x + 4, x - 1),
								new Point(x + 1, x + 3),
								new Point(x, x + 3)
								}
							);
						}
					}
					g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
				}

				rect.X += 26;
				TextRenderer.DrawText(
					g,
					this.Text,
					this.Font,
					rect,
					textColor,
					TextFormatFlags.Left | TextFormatFlags.VerticalCenter
				);
			};
		}
	}
}
