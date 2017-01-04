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
	public class FlatButton : Control
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
		public override Color BackColor
		{
			get { return base.BackColor; }
			set
			{
				if (this.BackColor != value)
				{
					base.BackColor = value;
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

		private Color _BorderColor { get; set; }
		[Description("Border Color"), Category("Appearance")]
		public Color BorderColor
		{
			get { return this._BorderColor; }
			set
			{
				if (this._BorderColor != value)
				{
					this._BorderColor = value;
					this.Invalidate();
				}
			}
		}

		private Color _OverBorderColor { get; set; }
		[Description("Over Border Color"), Category("Appearance")]
		public Color OverBorderColor
		{
			get { return this._OverBorderColor; }
			set
			{
				if (this._OverBorderColor != value)
				{
					this._OverBorderColor = value;
					this.Invalidate();
				}
			}
		}

		private Color _DownBorderColor { get; set; }
		[Description("Down Border Color"), Category("Appearance")]
		public Color DownBorderColor
		{
			get { return this._DownBorderColor; }
			set
			{
				if (this._DownBorderColor != value)
				{
					this._DownBorderColor = value;
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

		public FlatButton()
		{
			this.DoubleBuffered = true;

			this.DoubleClick += (s, e) => this.OnClick(e);
			this.MouseEnter += (s, e) =>this.State |= ButtonState.Over;
			this.MouseLeave += (s, e) => this.State &= ~ButtonState.Over;
			this.MouseDown += (s, e) => this.State |= ButtonState.Down;
			this.MouseUp += (s, e) => this.State &= ~ButtonState.Down;
			this.Resize += (s, e) => this.Invalidate();
			this.Paint += (s, e) =>
			{
				Graphics g = e.Graphics;
				Color borderColor, backColor;

				switch (this.State)
				{
					case ButtonState.Down | ButtonState.Over:
						borderColor = DownBorderColor;
						backColor = DownBackColor;
						break;
					case ButtonState.Down:
					case ButtonState.Over:
						borderColor = OverBorderColor;
						backColor = OverBackColor;
						break;
					default:
						borderColor = BorderColor;
						backColor = BackColor;
						break;
				}

				g.Clear(backColor);
				using (Pen p = new Pen(borderColor, 1.0f))
					g.DrawRectangle(p, new Rectangle(0, 0, this.Width - 1, this.Height - 1));

				if (this.BackgroundImage != null)
				{
					var image = this.BackgroundImage;
					g.DrawImage(
						image,
						new Rectangle(
							this.Width / 2 - image.Width / 2,
							this.Height / 2 - image.Height / 2,
							image.Width,
							image.Height
						)
					);
				}
				else
				{
					TextRenderer.DrawText(
						g,
						this.Text,
						this.Font,
						this.ClientRectangle,
						this.Enabled ? this.ForeColor : Color.Gray,
						TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
					);
				}
			};
		}
	}
}
