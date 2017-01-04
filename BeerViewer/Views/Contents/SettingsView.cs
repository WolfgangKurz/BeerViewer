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
using BeerViewer.Models;

namespace BeerViewer.Views.Contents
{
	public partial class SettingsView : UserControl
	{
		private class ViewRangeCalcLogicDisplayPair
		{
			public ICalcViewRange Key { get; set; }
			public string Display { get; set; }

			public override string ToString() => Display;
		}
		private class ZoomDisplayPair
		{
			public double Zoom { get; set; }
			public override string ToString() =>
				string.Format(
					"{0} % ({1}x{2})",
					Math.Floor(Zoom * 100),
					Math.Floor(800 * Zoom),
					Math.Floor(480 * Zoom)
				);
		}

		private double neutral;
		private static readonly double[] zoomTable =
		{
			0.25, 0.50, 0.75, 0.80, 0.85,
			1.00, 1.25, 1.50, 1.75,
			2.00, 2.50,
			3.00,
			4.00,
		};

		public SettingsView()
		{
			InitializeComponent();

			neutral = Settings.BrowserZoom.Value;
			bool ZoomComboEventAttached = false;

			foreach (var zoom in zoomTable)
			{
				this.comboZoom.Items.Add(new ZoomDisplayPair { Zoom = zoom });
				if (zoom == neutral)
					this.comboZoom.SelectedIndex = this.comboZoom.Items.Count - 1;
			}
			DataStorage.Instance.PropertyEvent(nameof(DataStorage.Ready), () =>
			{
				if (!DataStorage.Instance.Ready) return;
				if (ZoomComboEventAttached) return;

				ZoomComboEventAttached = true;
				this.comboZoom.SelectedIndexChanged += (s, e) =>
				{
					var item = comboZoom.SelectedItem as ZoomDisplayPair;
					if (item != null)
					{
						frmMain.Instance.SetZoom(item.Zoom);
						Settings.BrowserZoom.Value = item.Zoom;
					}
				};

				var _item = comboZoom.SelectedItem as ZoomDisplayPair;
				if (_item != null)
					frmMain.Instance.SetZoom(_item.Zoom);
			}, true);

			this.comboMainLayout.Items.Add("가로");
			this.comboMainLayout.Items.Add("세로");
			this.comboMainLayout.SelectedIndex = Settings.VerticalMode.Value ? 1 : 0;
			this.comboMainLayout.SelectedIndexChanged += (s, e) =>
				Settings.VerticalMode.Value = (this.comboMainLayout.SelectedIndex == 0 ? false : true);

			this.comboViewRangeType.SelectedIndexChanged += (s, e) =>
			{
				ViewRangeCalcLogicDisplayPair selected = this.comboViewRangeType.SelectedItem as ViewRangeCalcLogicDisplayPair;
				if(selected==null)
					labelViewRangeDescription.Text = "-";
				else
					labelViewRangeDescription.Text = selected.Key.Description;
			};
			foreach (var logic in ViewRangeCalcLogic.Logics)
			{
				var item = new ViewRangeCalcLogicDisplayPair
				{
					Key = logic,
					Display = logic.Name.ToString()
				};
				this.comboViewRangeType.Items.Add(item);

				if (logic.Id == Settings.ViewRangeCalcType.Value)
					this.comboViewRangeType.SelectedItem = item;
			}

			chkViewRangeCalcFirstFleet.Checked = Settings.IsViewRangeCalcIncludeFirstFleet.Value;
			chkViewRangeCalcFirstFleet.CheckedChanged += (s, e) => Settings.IsViewRangeCalcIncludeFirstFleet.Value = chkViewRangeCalcFirstFleet.Checked;
			chkViewRangeCalcSecondFleet.Checked = Settings.IsViewRangeCalcIncludeSecondFleet.Value;
			chkViewRangeCalcSecondFleet.CheckedChanged += (s, e) => Settings.IsViewRangeCalcIncludeSecondFleet.Value = chkViewRangeCalcSecondFleet.Checked;

			chkExpeditionNotify.Checked = Settings.Notify_ExpeditionComplete.Value;
			chkExpeditionNotify.CheckedChanged += (s, e) => Settings.Notify_ExpeditionComplete.Value = chkExpeditionNotify.Checked;
			chkBuildNotify.Checked = Settings.Notify_BuildComplete.Value;
			chkBuildNotify.CheckedChanged += (s, e) => Settings.Notify_BuildComplete.Value = chkBuildNotify.Checked;
			chkRepairNotify.Checked = Settings.Notify_RepairComplete.Value;
			chkRepairNotify.CheckedChanged += (s, e) => Settings.Notify_RepairComplete.Value = chkRepairNotify.Checked;
			chkConditionNotify.Checked = Settings.Notify_ConditionComplete.Value;
			chkConditionNotify.CheckedChanged += (s, e) => Settings.Notify_ConditionComplete.Value = chkConditionNotify.Checked;

			chkBattleInfoAutoSelectTab.Checked = Settings.BattleInfo_AutoSelectTab.Value;
			chkBattleInfoAutoSelectTab.CheckedChanged += (s, e) => Settings.BattleInfo_AutoSelectTab.Value = chkBattleInfoAutoSelectTab.Checked;
			chkBattleInfoDetailAirCombat.Checked = Settings.BattleInfo_DetailKouku.Value;
			chkBattleInfoDetailAirCombat.CheckedChanged += (s, e) => Settings.BattleInfo_DetailKouku.Value = chkBattleInfoDetailAirCombat.Checked;
			chkCriticalColor.Checked = Settings.BattleInfo_EnableColorChange.Value;
			chkCriticalColor.CheckedChanged += (s, e) => Settings.BattleInfo_EnableColorChange.Value = chkCriticalColor.Checked;

			chkGeneralAutoSelect.Checked = Settings.BackHome_AutoSelectTab.Value;
			chkGeneralAutoSelect.CheckedChanged += (s, e) => Settings.BackHome_AutoSelectTab.Value = chkGeneralAutoSelect.Checked;
			chkCriticalNotify.Checked = Settings.BattleInfo_CriticalEnabled.Value;
			chkCriticalNotify.CheckedChanged += (s, e) => Settings.BattleInfo_CriticalEnabled.Value = chkCriticalNotify.Checked;
			chkBattleEndNotify.Checked = Settings.BattleInfo_IsEnabledBattleEndNotify.Value;
			chkBattleEndNotify.CheckedChanged += (s, e) => Settings.BattleInfo_IsEnabledBattleEndNotify.Value = chkBattleEndNotify.Checked;

			this.comboFlashQuality.Items.Add("High");
			this.comboFlashQuality.Items.Add("Medium");
			this.comboFlashQuality.Items.Add("Low");
			this.comboFlashQuality.SelectedIndex = Settings.FlashQuality.Value;
			this.comboFlashQuality.SelectedIndexChanged += (s, e) =>
				Settings.FlashQuality.Value = this.comboFlashQuality.SelectedIndex;
		}

		public void SetBackColor(Color color)
		{
			if(this.layoutMain.InvokeRequired)
			{
				this.layoutMain.Invoke(() => SetBackColor(color));
				return;
			}

			this.layoutMain.BackColor = color;
		}

		private void btnGameStart_Click(object sender, EventArgs e)
		{
			Helper.PrepareBrowser(frmMain.Instance?.Browser, true);
		}

		private void btnLogout_Click(object sender, EventArgs e)
		{
			frmMain.Instance?.Browser?.Navigate(Const.LogoutURL);
		}

		private void btnCookie_Click(object sender, EventArgs e)
		{
			frmMain.Instance?.Browser?.Navigate(
				string.Format(
					"javascript:void(eval(\"{0};alert('설정완료');\"));",
					Const.PatchCookie
				)
			);
		}
	}
}
