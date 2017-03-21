using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BeerViewer.Core;
using BeerViewer.Models;
using BeerViewer.Views.Controls;

namespace BeerViewer.Views.Catalogs
{
	public partial class catalogShips : Form
	{
		private Homeport homeport { get; set; }

		private Dictionary<int, FlatCheckBox> ShipTypeCheckeds { get; set; } = new Dictionary<int, FlatCheckBox>();
		private bool IgnoreShipType { get; set; } = false;

		public catalogShips()
		{
			InitializeComponent();

			layoutShipType.Resize += (s, e) => layoutShipType.Invalidate();
			layoutShipType.Paint += (s, e) =>
			{
				using (Pen p = new Pen(Color.FromArgb(0x30, 0x90, 0x90, 0x90), 1.0f))
					e.Graphics.DrawRectangle(p, new Rectangle(
						layoutShipType.ClientRectangle.Left,
						layoutShipType.ClientRectangle.Top,
						layoutShipType.ClientRectangle.Width - 1,
						layoutShipType.ClientRectangle.Height - 1 - 8));
			};
			layoutShipFilter.Resize += (s, e) => layoutShipFilter.Invalidate();
			layoutShipFilter.Paint += (s, e) =>
			{
				using (Pen p = new Pen(Color.FromArgb(0x30, 0x90, 0x90, 0x90), 1.0f))
					e.Graphics.DrawRectangle(p, new Rectangle(
						layoutShipFilter.ClientRectangle.Left,
						layoutShipFilter.ClientRectangle.Top,
						layoutShipFilter.ClientRectangle.Width - 1,
						layoutShipFilter.ClientRectangle.Height - 1 - 8));
			};

			this.expandFilter.ExpandedChanged += (s, e) =>
			{
				layoutFilters.Visible = expandFilter.Expanded;
			};

			InitializeCatalog();
		}

		private void InitializeCatalog()
		{
			Action ShipTypeInitializer = null;
			ShipTypeInitializer = () =>
			{
				if (layoutShipTypeList.InvokeRequired)
				{
					layoutShipTypeList.Invoke(ShipTypeInitializer);
					return;
				}

				ShipTypeCheckeds.Clear();
				layoutShipTypeList.Controls.Clear();
				foreach (var type in DataStorage.Instance.Master.ShipTypes)
				{
					var chkCheck = new FlatCheckBox();
					chkCheck.Text = type.Value.Name;
					chkCheck.Checked = true;
					chkCheck.AutoSize = true;
					chkCheck.CheckedChanged += (s, e) => ShipTypeUpdate(false);

					layoutShipTypeList.Controls.Add(chkCheck);
					ShipTypeCheckeds.Add(type.Value.Id, chkCheck);
				}
			};

			ShipTypeInitializer();
			chkShipTypeAll.CheckedChanged += (s, e) => ShipTypeUpdate(true);

			this.SuspendLayout();
			BuildShipTypePreset("구축함", new int[] { 2 });
			BuildShipTypePreset("경순・뇌순", new int[] { 3, 4 });
			BuildShipTypePreset("중순・항순", new int[] { 5, 6 });
			BuildShipTypePreset("전함", new int[] { 8, 9, 10, 12 });
			BuildShipTypePreset("항전・항순", new int[] { 6, 10 });
			BuildShipTypePreset("항공모함", new int[] { 7, 11, 18 });
			BuildShipTypePreset("잠수함", new int[] { 13, 14 });
			this.ResumeLayout();

			#region Level
			#region KeyPress Processing
			this.textLevelFrom.KeyPress += (s, e) =>
			{
				int isNumber = 0;
				e.Handled = !int.TryParse(e.KeyChar.ToString(), out isNumber);
			};
			this.textLevelTo.KeyPress += (s, e) =>
			{
				int isNumber = 0;
				e.Handled = !int.TryParse(e.KeyChar.ToString(), out isNumber);
			};
			#endregion

			#region Value Processing
			this.textLevelFrom.TextChanged += (s, e) =>
			{
				int value = 2;
				int.TryParse(this.textLevelFrom.Text, out value);
				shipListTable.LevelFrom = value;
			};
			this.textLevelTo.TextChanged += (s, e) =>
			{
				int value = 155;
				int.TryParse(this.textLevelTo.Text, out value);
				shipListTable.LevelTo = value;
			};
			#endregion

			#region Button Processing
			this.btnLevelAll.Click += (s, e) =>
			{
				this.textLevelFrom.Text = "1";
				this.textLevelTo.Text = "155";
			};
			this.btnLevel1.Click += (s, e) =>
			{
				this.textLevelFrom.Text = "1";
				this.textLevelTo.Text = "1";
			};
			this.btnLevelAbove2.Click += (s, e) =>
			{
				this.textLevelFrom.Text = "2";
				this.textLevelTo.Text = "155";
			};
			#endregion
			#endregion

			#region Lock
			this.radioLockAll.CheckedChanged += (s, e) =>
			{
				if (radioLockAll.Checked)
					shipListTable.LockFilter = ShipListTable.FilterValues.Both;
			};
			this.radioLockYes.CheckedChanged += (s, e) =>
			{
				if (radioLockYes.Checked)
					shipListTable.LockFilter = ShipListTable.FilterValues.Yes;
			};
			this.radioLockNo.CheckedChanged += (s, e) =>
			{
				if (radioLockNo.Checked)
					shipListTable.LockFilter = ShipListTable.FilterValues.No;
			};
			#endregion

			this.chkExceptExpedition.CheckedChanged += (s, e) =>
				shipListTable.ExceptExpedition = this.chkExceptExpedition.Checked;

			#region Speed
			this.chkSpeedSuperfast.CheckedChanged += (s, e) =>
			{
				if (chkSpeedSuperfast.Checked)
					shipListTable.SpeedFilter |= ShipListTable.FilterValues.SuperFast;
				else
					shipListTable.SpeedFilter &= ~ShipListTable.FilterValues.SuperFast;
			};
			this.chkSpeedFastPlus.CheckedChanged += (s, e) =>
			{
				if (chkSpeedFastPlus.Checked)
					shipListTable.SpeedFilter = ShipListTable.FilterValues.FastPlus;
				else
					shipListTable.SpeedFilter &= ~ShipListTable.FilterValues.FastPlus;
			};
			this.chkSpeedFast.CheckedChanged += (s, e) =>
			{
				if (chkSpeedFast.Checked)
					shipListTable.SpeedFilter = ShipListTable.FilterValues.Fast;
				else
					shipListTable.SpeedFilter &= ~ShipListTable.FilterValues.Fast;
			};
			this.chkSpeedSlow.CheckedChanged += (s, e) =>
			{
				if (chkSpeedSlow.Checked)
					shipListTable.SpeedFilter = ShipListTable.FilterValues.Slow;
				else
					shipListTable.SpeedFilter &= ~ShipListTable.FilterValues.Slow;
			};
			#endregion

			#region Powerup
			this.radioPowerUpAll.CheckedChanged += (s, e) =>
			{
				if (radioPowerUpAll.Checked)
					shipListTable.PowerupFilter = ShipListTable.FilterValues.Both;
			};
			this.radioPowerUpEnd.CheckedChanged += (s, e) =>
			{
				if (radioPowerUpEnd.Checked)
					shipListTable.PowerupFilter = ShipListTable.FilterValues.End;
			};
			this.radioPowerUpNotEnd.CheckedChanged += (s, e) =>
			{
				if (radioPowerUpNotEnd.Checked)
					shipListTable.PowerupFilter = ShipListTable.FilterValues.NotEnd;
			};
			#endregion

			this.shipListTable.LevelFrom = 2;
			this.shipListTable.LevelTo = 155;
			this.shipListTable.LockFilter = ShipListTable.FilterValues.Both;
			this.shipListTable.ExceptExpedition = false;
			this.shipListTable.SpeedFilter = ShipListTable.FilterValues.Both;
			this.shipListTable.PowerupFilter = ShipListTable.FilterValues.Both;
		}
		private void BuildShipTypePreset(string Name, int[] Targets)
		{
			if (layoutShipTypePreset.InvokeRequired)
			{
				layoutShipTypePreset.Invoke(() => BuildShipTypePreset(Name, Targets));
				return;
			}

			FlatButton presetButton = new FlatButton();
			presetButton.Text = Name;
			presetButton.BackColor = Color.FromArgb(39, 39, 47);
			presetButton.BorderColor = Color.FromArgb(83, 83, 83);
			presetButton.DownBackColor = Color.FromArgb(81, 117, 142);
			presetButton.DownBorderColor = Color.FromArgb(59, 71, 83);
			presetButton.OverBackColor = Color.FromArgb(89, 89, 89);
			presetButton.OverBorderColor = Color.FromArgb(59, 71, 83);
			presetButton.Size = new Size(76, 28);
			presetButton.Click += (s, e) =>
			{
				IgnoreShipType = true;

				foreach (var item in ShipTypeCheckeds)
					item.Value.Checked = Targets.Contains(item.Key);
				chkShipTypeAll.Checked = ShipTypeCheckeds.All(x => x.Value.Checked);

				IgnoreShipType = false;
				ShipTypeUpdate(false);
			};

			layoutShipTypePreset.Controls.Add(presetButton);
		}

		private void ShipTypeUpdate(bool AllCheck)
		{
			if (IgnoreShipType) return;
			if (this.InvokeRequired)
			{
				this.Invoke(InitializeCatalog);
				return;
			}

			IgnoreShipType = true;
			{
				if (!AllCheck)
					chkShipTypeAll.Checked = ShipTypeCheckeds.All(x => x.Value.Checked);
				else
				{
					var m = chkShipTypeAll.Checked;
					foreach (var control in ShipTypeCheckeds)
						control.Value.Checked = m;
				}
			}
			IgnoreShipType = false;

			shipListTable.ShipTypes = ShipTypeCheckeds
				.Where(x=>x.Value.Checked).Select(x => x.Key).ToArray();
		}

		public void SetHomeport(Homeport homeport)
		{
			this.homeport = homeport;
			if (homeport == null) return;

			homeport.Organization.PropertyEvent(nameof(homeport.Organization.Ships), () => shipListTable.SetHomeport(this.homeport), true);
		}
	}
}
