using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ControlCollection = System.Windows.Forms.Control.ControlCollection;

using BeerViewer.Models;

namespace BeerViewer.Views.Controls
{
	class BattleShipTooltip : ToolTip
	{
		private Ship Ship { get; set; }

		public BattleShipTooltip()
		{
			this.Popup += (s, e) =>
			{
				e.ToolTipSize = CalcSize(this.Ship);
			};
			this.Draw += (s, e) =>
			{
				var g = e.Graphics;
				g.Clear(Color.FromArgb(0x27, 0x27, 0x2F));
				g.DrawRectangle(
					new Pen(Color.FromArgb(0x44, 0x44, 0x4A), 1.0f),
					new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width - 1, e.Bounds.Height - 1)
				);
				e.DrawText();
			};
			this.OwnerDraw = true;
			this.ShowAlways = true;
		}

		private Size CalcSize(Ship ship)
		{
			return new Size(260, 80);
		}

		public void SetToolTip(Control control, Ship ship)
		{
			this.Ship = ship;
			AttachToopTip(control);
		}

		private void AttachToopTip(Control target)
		{
			this.SetToolTip(target, " ");
			foreach (Control control in target.Controls)
				AttachToopTip(control);
		}
	}
}
