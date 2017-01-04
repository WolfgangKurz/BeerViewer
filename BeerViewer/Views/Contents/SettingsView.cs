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

		public SettingsView()
		{
			InitializeComponent();

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

			chkCriticalNotify.Checked = Settings.BattleInfo_CriticalEnabled.Value;
			chkCriticalNotify.CheckedChanged += (s, e) => Settings.BattleInfo_CriticalEnabled.Value = chkCriticalNotify.Checked;
			chkBattleEndNotify.Checked = Settings.BattleInfo_IsEnabledBattleEndNotify.Value;
			chkBattleEndNotify.CheckedChanged += (s, e) => Settings.BattleInfo_IsEnabledBattleEndNotify.Value = chkBattleEndNotify.Checked;
		}
	}
}
