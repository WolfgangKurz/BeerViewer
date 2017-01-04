using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace BeerViewer.Views.Controls
{
	public partial class GeneralTableView : UserControl
	{
		public class HeaderValue
		{
			public string Header { get; set; } = "Header";
			public string Value { get; set; } = "Value";
			public Color ForeColor { get; set; } = Color.White;
			public Color BackColor { get; set; } = Color.FromArgb(0x30, 0x90, 0x90, 0x90);

			public bool HeaderVisible { get; set; } = true;
			public bool Visible { get; set; } = true;
		}

		#region TableName 프로퍼티
		private string _TableName { get; set; } = "Table";
		public string TableName
		{
			get { return this._TableName; }
			set
			{
				if (this._TableName != value)
				{
					this._TableName = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion

		#region TableCells 프로퍼티
		private HeaderValue[] _TableCells { get; set; }
		public HeaderValue[] TableCells
		{
			get { return this._TableCells; }
			set
			{
				if (this._TableCells != value)
				{
					this._TableCells = value;
					this.RequestUpdate();
				}
			}
		}
		#endregion

		public GeneralTableView()
		{
			InitializeComponent();

			this.Paint += (s, e) =>
			{
				var g = e.Graphics;
				var Width = this.Width - this.Padding.Left - this.Padding.Right;
				var Height = this.Height - this.Padding.Top - this.Padding.Bottom;

				g.TranslateTransform(this.Padding.Left, this.Padding.Top);

				using (SolidBrush b = new SolidBrush(Color.FromArgb(0x30, 0x90, 0x90, 0x90)))
				{
					g.FillRectangle(b, new Rectangle(0, 0, 30, Height));

					if (TableCells?.Length > 0)
					{
						var x = 30;
						var w = (Width - 30 + (TableCells.Length - 1)) / TableCells.Length;
						for (var i = 0; i < TableCells.Length; i++)
						{
							var cell = TableCells[i];

							if (cell.HeaderVisible)
								g.FillRectangle(b, new Rectangle(x + 1, 0, w - 1, Height / 2));

							if (cell.Visible)
							{
								using (SolidBrush bb = new SolidBrush(cell.BackColor))
									g.FillRectangle(bb, new Rectangle(x + 1, Height / 2 + 1, w - 1, Height / 2));
							}
							x += w;
						}
					}
				}

				using (SolidBrush b = new SolidBrush(Color.White))
				{
					Size sz = TextRenderer.MeasureText(TableName, this.Font);

					var state = g.Save();
					g.TranslateTransform(-sz.Width / 2, -sz.Height / 2, MatrixOrder.Append);
					g.RotateTransform(-90, MatrixOrder.Append);
					g.TranslateTransform(15, Height / 2, MatrixOrder.Append);

					g.DrawString(TableName, this.Font, b, 0, 0);
					g.Restore(state);
				}

				if (TableCells?.Length > 0)
				{
					var x = 30;
					var w = (Width - 30 + (TableCells.Length - 1)) / TableCells.Length;
					for (var i = 0; i < TableCells.Length; i++)
					{
						var cell = TableCells[i];

						if (cell.HeaderVisible)
							TextRenderer.DrawText(
								g,
								cell.Header,
								this.Font,
								new Rectangle(x + 1, 0, w - 1, Height / 2),
								Color.White,
								TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
								| TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding
							);
						if (cell.Visible)
							TextRenderer.DrawText(
								g,
								cell.Value,
								this.Font,
								new Rectangle(x + 1, Height / 2 + 1, w - 1, Height / 2),
								cell.ForeColor,
								TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
								| TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding
							);
						x += w;
					}
				}
			};
		}

		protected override void OnPaddingChanged(EventArgs e)
		{
			this.RequestUpdate();
			base.OnPaddingChanged(e);
		}

		public void RequestUpdate()
		{
			if (this.InvokeRequired)
				this.Invoke(this.Invalidate);
			else
				this.Invalidate();
		}
	}
}
