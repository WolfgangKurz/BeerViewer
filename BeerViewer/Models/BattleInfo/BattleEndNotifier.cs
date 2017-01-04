using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BeerViewer.Core;
using BeerViewer.Models.Raw;

namespace BeerViewer.Models.BattleInfo
{
	public class BattleEndNotifier : Notifier
	{
		#region IsEnabled 프로퍼티
		public bool IsEnabled
		{
			get { return Settings.BattleInfo_IsEnabledBattleEndNotify.Value; }
			set
			{
				if (Settings.BattleInfo_IsEnabledBattleEndNotify.Value == value)
				{
					Settings.BattleInfo_IsEnabledBattleEndNotify.Value = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region CriticalEnabled 프로퍼티
		public bool CriticalEnabled
		{
			get { return Settings.BattleInfo_CriticalEnabled.Value; }
			set
			{
				if (Settings.BattleInfo_CriticalEnabled.Value == value)
				{
					Settings.BattleInfo_CriticalEnabled.Value = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public BattleEndNotifier()
		{
			if (Helper.IsInDesignMode) return;

			var proxy = Proxy.Instance;
			proxy.Register(Proxy.api_req_combined_battle_battleresult, e => this.NotifyEndOfBattle());
			proxy.Register("/kcsapi/api_req_practice/battle_result", e => this.NotifyEndOfBattle());
			proxy.Register(Proxy.api_req_sortie_battleresult, e => this.NotifyEndOfBattle());
			proxy.Register(Proxy.api_req_map_start, e => this.IsCriticalCheck());
			proxy.Register("/kcsapi/api_req_map/next", e => this.IsCriticalCheck());
			proxy.Register(Proxy.api_port, e => this.BackHome());
		}
		private bool IsCriticalCheck()
		{
			if (!CriticalEnabled) return false;
			if (Settings.BattleInfo_FirstIsCritical.Value || Settings.BattleInfo_SecondIsCritical.Value)
			{
				this.Notify(
					NotificationType.CriticalState,
					"대파알림",
					"대파된 칸무스가 있습니다!",
					true
				);
				return true;
			}
			else return false;
		}
		private void NotifyEndOfBattle()
		{
			if (!IsCriticalCheck() || !CriticalEnabled)
				this.Notify(
					NotificationType.BattleEnd,
					"전투종료",
					"전투가 종료되었습니다"
				);
		}
		private void BackHome()
		{
			Helper.SetCritical(false);
			if (Settings.BackHome_AutoSelectTab.Value)
				frmMain.Instance?.UpdateTab("General");
		}

		private void Notify(string type, string title, string message, bool IsCritical = false)
		{
			var MainWindow = frmMain.Instance;
			bool notify = false;

			if (IsCritical && CriticalEnabled)
			{
				if (Settings.BattleInfo_EnableColorChange.Value)
					Helper.SetCritical(true);

				notify = true;
			}
			else if (this.IsEnabled)
				notify = true;

			if (notify)
				NotifyManager.Notify(type, title, message);
		}
	}
}
