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
using BeerViewer.Views.Catalogs;
using BeerViewer.Views.Controls;

namespace BeerViewer.Views.Contents
{
	public partial class GeneralView : UserControl
	{
		private class ResourceNameValueDisplayItem : FlatComboBox.NameValueDisplayItem
		{
			public override string Name { get; set; } = "???";
			public override string Value => this.ValueGetter?.Invoke() ?? "???";

			public Func<string> ValueGetter { get; set; } = () => "???";

			public ResourceNameValueDisplayItem(string Name, Func<string> ValueGetter)
			{
				this.Name = Name;
				this.ValueGetter = ValueGetter;
			}
		}

		private NotifyHost notifyHost { get; set; }
		private Homeport homeport { get; set; }

		private HQRecord HQRecorder { get; }

		public GeneralView()
		{
			InitializeComponent();

			DataStorage.Instance.PropertyEvent(nameof(DataStorage.Initialized), () =>
			{
				Action<bool> Enabler = (x) =>
				{
					btnShipList.Enabled = x;
					btnItemList.Enabled = x;
					btnAkashi.Enabled = x;
				};
				var Enabled = DataStorage.Instance.Initialized;

				if (this.InvokeRequired)
					this.Invoke(() => Enabler(Enabled));
				else
					Enabler(Enabled);
			}, true);

			this.HQRecorder = new HQRecord();
			this.HQRecorder.Load();

			this.comboResources1.SelectedIndexChanged += (s, e) => Settings.ResourceSelected1.Value = this.comboResources1.SelectedIndex;
			this.comboResources2.SelectedIndexChanged += (s, e) => Settings.ResourceSelected2.Value = this.comboResources2.SelectedIndex;
		}

		public void SetHomeport(Homeport homeport)
		{
			this.homeport = homeport;
			if (homeport == null) return;

			notifyHost = new NotifyHost(homeport);

			Action UpdateInfo = () =>
			{
				var shipsMax = homeport.Admiral?.MaxShipCount ?? 0;
				var ships = homeport.Organization?.Ships.Count(x => x.Value != null) ?? 0;

				var slotMax = homeport.Admiral?.MaxSlotItemCount ?? 0;
				var slots = homeport.Itemyard?.SlotItemsCount;

				if (labelShipCount.InvokeRequired)
				{
					labelShipCount.Invoke(() => labelShipCount.Text = $"소속칸무스: {ships}/{shipsMax}");
					labelSlotitemCount.Invoke(() => labelSlotitemCount.Text = $"보유장비: {slots}/{slotMax}");
				}
				else
				{
					labelShipCount.Text = $"소속칸무스: {ships}/{shipsMax}";
					labelSlotitemCount.Text = $"보유장비: {slots}/{slotMax}";
				}
			};

			// Resources
			homeport.PropertyEvent(nameof(homeport.Materials), () =>
			{
				AttachResources(homeport.Materials);

				homeport.Materials.PropertyEvent(nameof(homeport.Materials.Fuel), () => this.UpdateResources(homeport.Materials));
				homeport.Materials.PropertyEvent(nameof(homeport.Materials.Ammo), () => this.UpdateResources(homeport.Materials));
				homeport.Materials.PropertyEvent(nameof(homeport.Materials.Steel), () => this.UpdateResources(homeport.Materials));
				homeport.Materials.PropertyEvent(nameof(homeport.Materials.Bauxite), () => this.UpdateResources(homeport.Materials));
				homeport.Materials.PropertyEvent(nameof(homeport.Materials.RepairBuckets), () => this.UpdateResources(homeport.Materials));
				homeport.Materials.PropertyEvent(nameof(homeport.Materials.BuildMaterials), () => this.UpdateResources(homeport.Materials));
				homeport.Materials.PropertyEvent(nameof(homeport.Materials.DevMaterials), () => this.UpdateResources(homeport.Materials));
				homeport.Materials.PropertyEvent(nameof(homeport.Materials.ImproveMaterials), () => this.UpdateResources(homeport.Materials));
				this.UpdateResources(homeport.Materials);
			}, true);

			// Fleets & Expedition
			homeport.Organization.PropertyEvent(nameof(homeport.Organization.Fleets), () =>
			{
				homeport.Admiral.PropertyEvent(nameof(homeport.Organization.Ships), () => UpdateInfo(), true);

				var fleets = homeport.Organization.Fleets.Select(x => x.Value);
				foreach (var fleet in fleets)
				{
					UpdateFleetState(fleet);

					if (fleet != null)
					{
						fleet.Expedition.PropertyEvent(nameof(fleet.Expedition.Id), () => UpdateFleetState(fleet));
						fleet.Expedition.PropertyEvent(nameof(fleet.Expedition.Remaining), () => UpdateFleetState(fleet));

						fleet.State.Condition.PropertyEvent(nameof(fleet.State.Condition.Remaining), () => UpdateFleetState(fleet));
						fleet.State.Updated += (s, e) => UpdateFleetState(fleet);
					}
				}
			}, true);

			// NDock
			homeport.Repairyard.PropertyEvent(nameof(homeport.Repairyard.Docks), () =>
			{
				var nDocks = homeport.Repairyard.Docks.Select(x => x.Value);
				foreach (var dock in nDocks)
				{
					dock.PropertyEvent(nameof(dock.Ship), () => UpdateNDock(dock));
					dock.PropertyEvent(nameof(dock.Remaining), () => UpdateNDock(dock));
					UpdateNDock(dock);
				}
			}, true);

			// KDock
			homeport.Dockyard.PropertyEvent(nameof(homeport.Dockyard.Docks), () =>
			{
				var kDocks = homeport.Dockyard.Docks.Select(x => x.Value);
				foreach (var dock in kDocks)
				{
					dock.PropertyEvent(nameof(dock.Ship), () => UpdateKDock(dock));
					dock.PropertyEvent(nameof(dock.Remaining), () => UpdateKDock(dock));
					UpdateKDock(dock);
				}
			}, true);

			// Quests
			homeport.Quests.PropertyEvent(nameof(homeport.Quests.Current), () =>
			{
				if (homeport.Quests.All.Count == 0) return;
				UpdateQuests(homeport.Quests.Current);
			}, true);

			// Admiral
			homeport.PropertyEvent(nameof(homeport.Admiral), () =>
			{
				this.UpdateHQRecord(this.homeport.Admiral);
				UpdateInfo();
			}, true);

			// Itemyard
			homeport.PropertyEvent(nameof(homeport.Itemyard), () =>
			{
				homeport.Itemyard.PropertyEvent(nameof(homeport.Itemyard.SlotItemsCount), () => UpdateInfo());
			});

			catalogCalculator?.SetHomeport(this.homeport);
			catalogShips?.SetHomeport(this.homeport);
			UpdateInfo();
		}

		private void AttachResources(Materials materials)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(() => AttachResources(materials));
				return;
			}

			var combos = new FlatComboBox[] {
				this.comboResources1,
				this.comboResources2
			};
			foreach (var combo in combos)
			{
				combo.Items.Clear();

				combo.Items.Add(new ResourceNameValueDisplayItem("연료", () => materials.Fuel.ToString()));
				combo.Items.Add(new ResourceNameValueDisplayItem("탄약", () => materials.Ammo.ToString()));
				combo.Items.Add(new ResourceNameValueDisplayItem("강재", () => materials.Steel.ToString()));
				combo.Items.Add(new ResourceNameValueDisplayItem("보크사이트", () => materials.Bauxite.ToString()));

				combo.Items.Add(new ResourceNameValueDisplayItem("고속수복재", () => materials.RepairBuckets.ToString()));
				combo.Items.Add(new ResourceNameValueDisplayItem("고속건조재", () => materials.BuildMaterials.ToString()));
				combo.Items.Add(new ResourceNameValueDisplayItem("개발자재", () => materials.DevMaterials.ToString()));
				combo.Items.Add(new ResourceNameValueDisplayItem("개수자재", () => materials.ImproveMaterials.ToString()));
			}
			this.comboResources1.SelectedIndex = Settings.ResourceSelected1.Value;
			this.comboResources2.SelectedIndex = Settings.ResourceSelected2.Value;
		}
		private void UpdateResources(Materials materials)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(() => UpdateResources(materials));
				return;
			}

			var combos = new FlatComboBox[] {
				this.comboResources1,
				this.comboResources2
			};
			foreach (var combo in combos)
				combo.Refresh();
		}

		private void UpdateFleetState(Fleet fleet)
		{
			Action<int, string, string, Color> UpdateDisplay = (idx, name, state, color) =>
			{
				var cell = tableFleet.TableCells[idx - 1];
				cell.Header = name;
				cell.Value = state;
				cell.BackColor = color;
				cell.Visible = state != null;

				tableFleet.RequestUpdate();
			};

			if (fleet == null)
				UpdateDisplay(fleet.Id, "", null, Color.Transparent);

			else
			{
				if (fleet.Expedition.Id > 0)
				{
					var time = fleet.Expedition.Remaining;
					var remaining = time.HasValue
						? $"{(int)time.Value.TotalHours:D2}:{time.Value.ToString(@"mm\:ss")}"
						: "--:--:--";
					UpdateDisplay(fleet.Id, $"{fleet.Expedition.Id}번 원정", remaining, Color.FromArgb(0x20, 0x40, 0x80));
				}

				else if (fleet.IsInSortie)
					UpdateDisplay(fleet.Id, $"{fleet.Id}번 함대", "출격중", Color.FromArgb(0x7E, 0x01, 0x01));

				else if (fleet.State.Situation == FleetSituation.Empty)
					UpdateDisplay(fleet.Id, $"{fleet.Id}번 함대", "없음", Color.FromArgb(0x3E, 0x3E, 0x42));


				else if (fleet.State.HeavilyDamaged)
					UpdateDisplay(fleet.Id, $"{fleet.Id}번 함대", "대파 있음", Color.FromArgb(0xCF, 0x00, 0x00));

				else if (fleet.State.Repairing)
					UpdateDisplay(fleet.Id, $"{fleet.Id}번 함대", "수리중", Color.FromArgb(0x5A, 0x40, 0x20));

				else if (fleet.State.InShortSupply)
					UpdateDisplay(fleet.Id, $"{fleet.Id}번 함대", "보급 필요", Color.FromArgb(0x5A, 0x40, 0x20));

				else if (fleet.State.FlagshipIsRepairShip)
					UpdateDisplay(fleet.Id, $"{fleet.Id}번 함대", "공작함 기함", Color.FromArgb(0x5A, 0x40, 0x20));

				else if (fleet.State.Condition.IsRejuvenating)
				{
					var time = fleet.State.Condition.Remaining;
					var remaining = time.HasValue
						? $"{time.Value.ToString(@"mm\:ss")}"
						: "--:--";
					UpdateDisplay(fleet.Id, $"{fleet.Id}번 함대", "회복중 " + remaining, Color.FromArgb(0x5A, 0x40, 0x20));
				}

				else
					UpdateDisplay(fleet.Id, $"{fleet.Id}번 함대", "출격 가능", Color.FromArgb(0x28, 0x64, 0x14));
			}
		}
		private void UpdateNDock(RepairingDock dock)
		{
			Action<int, string, string> UpdateDisplay = (idx, name, time) =>
			{
				var cell = tableRepair.TableCells[idx - 1];
				cell.Header = name;
				cell.Value = time;
				cell.Visible = time != null;
				if (time == "" || time == "--:--:--")
					cell.ForeColor = Color.Gray;
				else
					cell.ForeColor = Color.White;

				tableRepair.RequestUpdate();
			};

			switch (dock.State)
			{
				case RepairingDockState.Locked:
					UpdateDisplay(dock.Id, "", null);
					break;
				case RepairingDockState.Unlocked:
					UpdateDisplay(dock.Id, "", "--:--:--");
					break;
				case RepairingDockState.Repairing:
					var time = dock.Remaining;
					var remaining = time.HasValue
						? $"{(int)time.Value.TotalHours:D2}:{time.Value.ToString(@"mm\:ss")}"
						: "--:--:--";
					UpdateDisplay(dock.Id, dock.Ship.LvName, remaining);
					break;
			}
		}
		private void UpdateKDock(BuildingDock dock)
		{
			Action<int, string, string> UpdateDisplay = (idx, name, time) =>
			{
				var cell = tableBuild.TableCells[idx - 1];
				cell.Header = name;
				cell.Value = time;
				cell.Visible = time != null;
				if (time == "" || time == "--:--:--")
					cell.ForeColor = Color.Gray;
				else
					cell.ForeColor = Color.White;

				tableBuild.RequestUpdate();
			};

			switch (dock.State) {
				case BuildingDockState.Locked:
					UpdateDisplay(dock.Id, "", null);
					break;
				case BuildingDockState.Unlocked:
					UpdateDisplay(dock.Id, "", "--:--:--");
					break;
				case BuildingDockState.Building:
					var time = dock.Remaining;
					var remaining = time.HasValue
						? $"{(int)time.Value.TotalHours:D2}:{time.Value.ToString(@"mm\:ss")}"
						: "--:--:--";
					UpdateDisplay(dock.Id, dock.Ship.Name, remaining);
					break;
				case BuildingDockState.Completed:
					UpdateDisplay(dock.Id, dock.Ship.Name, "완료");
					break;
			}
		}
		private void UpdateQuests(IEnumerable<Quest> quests)
		{
			this.listQuests.Quests = quests.Where(x => x != null).ToArray();
			this.listQuests.RequestUpdate();
		}
		private void UpdateHQRecord(Admiral admiral)
		{
			if (tableHQRecord.InvokeRequired)
			{
				this.tableHQRecord.Invoke(() => UpdateHQRecord(admiral));
				return;
			}

			this.HQRecorder.Updated();

			HQRecord.HQRecordElement res;
			int diff;

			res = this.HQRecorder.GetRecordMonth();
			diff = admiral.Experience - res.HQExp;
			tableHQRecord.TableCells[0].Value = string.Format("{0} exp. / {1:n2}", diff, diff * 7 / 10000.0);

			res = this.HQRecorder.GetRecordDay();
			diff = admiral.Experience - res.HQExp;
			tableHQRecord.TableCells[1].Value = string.Format("{0} exp. / {1:n2}", diff, diff * 7 / 10000.0);

			res = this.HQRecorder.GetRecordPrevious();
			diff = admiral.Experience - res.HQExp;
			tableHQRecord.TableCells[2].Value = string.Format("{0} exp. / {1:n2}", diff, diff * 7 / 10000.0);

			tableHQRecord.TableCells[0].Visible = true;
			tableHQRecord.TableCells[1].Visible = true;
			tableHQRecord.TableCells[2].Visible = true;

			tableHQRecord.RequestUpdate();
		}

		private catalogCalc catalogCalculator { get; set; }
		private void btnCalculator_Click(object sender, EventArgs e)
		{
			if (catalogCalculator == null)
			{
				catalogCalculator = new catalogCalc();
				catalogCalculator.FormClosed += (s, _) => this.catalogCalculator = null;
			}
			catalogCalculator.Show();

			if (this.homeport != null)
				catalogCalculator.SetHomeport(this.homeport);
		}
		private catalogShips catalogShips { get; set; }
		private void btnShipList_Click(object sender, EventArgs e)
		{
			if (catalogShips == null)
			{
				catalogShips = new catalogShips();
				catalogShips.FormClosed += (s, _) => this.catalogShips = null;
			}
			catalogShips.Show();

			if (this.homeport != null)
				catalogShips.SetHomeport(this.homeport);
		}
		private catalogSlotitems catalogSlotitems { get; set; }
		private void btnItemList_Click(object sender, EventArgs e)
		{
			if (catalogSlotitems == null)
			{
				catalogSlotitems = new catalogSlotitems();
				catalogSlotitems.FormClosed += (s, _) => this.catalogSlotitems = null;
			}
			catalogSlotitems.Show();

			if (this.homeport != null)
				catalogSlotitems.SetHomeport(this.homeport);
		}
	}
}
