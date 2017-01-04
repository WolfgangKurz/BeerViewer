using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BeerViewer.Models;

namespace BeerViewer.Core
{
	internal class NotifyHost
	{
		private class CompositeDisposable : IDisposable
		{
			private List<Action> DisposeAction { get; } = new List<Action>();

			public void Add(Action action)
			{
				this.DisposeAction.Add(action);
			}
			public void Dispose()
			{
				foreach (var action in DisposeAction)
					action?.Invoke();
				DisposeAction.Clear();
			}
		}

		private CompositeDisposable dockyardDisposables;
		private CompositeDisposable repairyardDisposables;
		private CompositeDisposable organizationDisposables;

		public NotifyHost(Homeport homeport)
		{
			homeport.PropertyEvent(nameof(homeport.Dockyard), () => UpdateDockyard(homeport.Dockyard), true);
			homeport.PropertyEvent(nameof(homeport.Repairyard), () => UpdateRepairyard(homeport.Repairyard), true);
			homeport.PropertyEvent(nameof(homeport.Organization), () => UpdateFleets(homeport.Organization), true);
		}

		#region Dockyard
		private void UpdateDockyard(Dockyard dockyard)
		{
			this.dockyardDisposables?.Dispose();
			this.dockyardDisposables = new CompositeDisposable();

			foreach (var dock in dockyard.Docks.Values)
			{
				dock.Completed += this.HandleDockyardCompleted;
				this.dockyardDisposables.Add(() => dock.Completed -= this.HandleDockyardCompleted);
			}
		}

		private void HandleDockyardCompleted(object sender, BuildingCompletedEventArgs args)
		{
			if (!Settings.Notify_BuildComplete.Value) return;

			var shipName = args.Ship.Name;
			NotifyManager.Notify(
				Notification.Types.BuildingCompleted,
				"건조 완료",
				string.Format("공창 제 {0} 도크의 {1} 건조가 완료되었습니다", args.DockId, shipName)
			);
		}
		#endregion

		#region Repairyard
		private void UpdateRepairyard(Repairyard repairyard)
		{
			this.repairyardDisposables?.Dispose();
			this.repairyardDisposables = new CompositeDisposable();

			foreach (var dock in repairyard.Docks.Values)
			{
				dock.Completed += this.HandleRepairyardCompleted;
				this.repairyardDisposables.Add(() => dock.Completed -= this.HandleRepairyardCompleted);
			}
		}

		private void HandleRepairyardCompleted(object sender, RepairingCompletedEventArgs args)
		{
			if (!Settings.Notify_RepairComplete.Value) return;

			NotifyManager.Notify(
				Notification.Types.RepairingCompleted,
				"정비 완료",
				string.Format("입거 제{0}도크 {1} 정비가 완료되었습니다", args.DockId, args.Ship.Info.Name)
			);
		}
		#endregion

		#region Fleet
		private void UpdateFleets(Organization organization)
		{
			organization.PropertyEvent(nameof(organization.Fleets), () =>
			{
				this.organizationDisposables?.Dispose();
				this.organizationDisposables = new CompositeDisposable();

				foreach (var fleet in organization.Fleets.Values)
				{
					fleet.Expedition.Returned += this.HandleExpeditionReturned;
					this.organizationDisposables.Add(() => fleet.Expedition.Returned -= this.HandleExpeditionReturned);

					fleet.State.Condition.Rejuvenated += this.HandleConditionRejuvenated;
					this.organizationDisposables.Add(() => fleet.State.Condition.Rejuvenated -= this.HandleConditionRejuvenated);
				}
			}, true);
		}

		private void HandleExpeditionReturned(object sender, ExpeditionReturnedEventArgs args)
		{
			if (!Settings.Notify_ExpeditionComplete.Value) return;

			NotifyManager.Notify(
				Notification.Types.ExpeditionReturned,
				"원정 완료",
				string.Format("「{0}」 원정에서 복귀했습니다", args.FleetName)
			);
		}

		private void HandleConditionRejuvenated(object sender, ConditionRejuvenatedEventArgs args)
		{
			if (!Settings.Notify_ConditionComplete.Value) return;

			NotifyManager.Notify(
				Notification.Types.FleetRejuvenated,
				"피로회복완료",
				string.Format("「{0}」의 피로회복이 완료되었습니다", args.FleetName)
			);
		}
		#endregion
	}
}
