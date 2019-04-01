/// <reference path="../../../node_modules/ts-nameof/ts-nameof.d.ts" />
import Vue from "vue";
import { Ship } from "System/Homeport/Ship";
import { AirSupremacy } from "System/Models/AirSupremacy";
import { RepairDock } from "System/Homeport/RepairDock";
import { ConstructionDock } from "System/Homeport/ConstructionDock";
import { IModule, GetModuleTemplate } from "System/Module";
import { Homeport } from "System/Homeport/Homeport";
import { FleetState } from "System/Enums/FleetEnums";
import { ObservableCallback } from "System/Base/Observable";
import { mapState } from "vuex";
import { ShipInfo } from "System/Master/Wrappers/ShipInfo";
import Quest from "System/Homeport/Quest/Quest";
import Settings, { Settings as SettingsClass } from "System/Settings";

const getTextWidth = function (text: string, font: string) {
	const canvas: HTMLCanvasElement = (<any>getTextWidth).canvas || ((<any>getTextWidth).canvas = document.createElement("canvas"));
	const context = canvas.getContext("2d");
	if (!context) return 0; // Failed to get canvas context..?
	context.font = font;
	const m = context.measureText(text);
	return m.width;
};

interface FleetData {
	Id: number;
	State: string;
	Ships: Ship[];

	TotalLevel: number;
	AvgLevel: number;
	Speed: string;
	LoS: string;
	AA: AirSupremacy;

	SupplyFuel: number;
	SupplyAmmo: number;
	SupplyBauxite: number;

	IsConditionRestoring: boolean;
	ConditionRestoringText: string;

	NameSize: number;

	_Observations: [Ship, ObservableCallback][];
}
interface RepairDockData {
	Id: number;
	Ship: string;
	RemainingTime: string;
	IsCompleted: boolean;
	State: RepairDock.DockState;
	StateText: string;
}
interface ConstructionDockData {
	Id: number;
	Ship: string;
	RemainingTime: string;
	IsCompleted: boolean;
	State: ConstructionDock.DockState;
	StateText: string;
}

class Overview implements IModule {
	private readonly speedList = {
		0: "fleet_speed_immovable",
		5: "fleet_speed_slow",
		10: "fleet_speed_fast",
		15: "fleet_speed_faster",
		20: "fleet_speed_fastest"
	};
	private readonly rangeList = {
		0: "ship_range_none",
		1: "ship_range_short",
		2: "ship_range_middle",
		3: "ship_range_long",
		4: "ship_range_far"
	};

	private Data = {
		SelectedTab: <number>1,
		Fleets: <FleetData[]>[],
		RepairDock: <RepairDockData[]>[],
		ConstructionDock: <ConstructionDockData[]>[],
		Quests: <ReadonlyArray<Quest>>[],

		LosType: <string>"",

		ExpDispType: "Remining",
		TooltipStatsDisplay: true
	}

	constructor() {
		const _this = this;
		Vue.component("overview-component", {
			data: () => this.Data,
			template: GetModuleTemplate(),

			computed: mapState({
				i18n: "i18n"
			}),
			methods: {
				GetConditionLevel(condition: number): string {
					let level: string = "0";

					if (condition >= 50)
						level = "+1";
					else if (condition >= 40)
						level = "0";
					else if (condition >= 30)
						level = "-1";
					else if (condition >= 20)
						level = "-2";
					else
						level = "-3";

					return level;
				},
				ParseShipState(ship: Ship): string {
					const states = [];
					if (ship.State & Ship.State.Repairing)
						states.push("repairing");
					else if (ship.State & Ship.State.Evacuation)
						states.push("evacuation");
					else if (ship.State & Ship.State.Tow)
						states.push("tow");
					else if (ship.State & Ship.State.HeavilyDamaged)
						states.push("damaged");
					else if (ship.State & Ship.State.DamageControlled)
						states.push("damagecontrolled");

					return states.join(" ");
				},
				SelectFleet(id: number): void {
					_this.SelectFleet(id);
				},
				numFormat(value: number): string {
					return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
				},
				speedStringify(value: keyof Overview["speedList"]): string {
					if (value in _this.speedList)
						return _this.speedList[value];
					else
						return "fleet_speed_unknown";
				},
				rangeStringify(value: keyof Overview["rangeList"]): string {
					if (value in _this.rangeList)
						return _this.rangeList[value];
					else
						return "ship_range_unknown";
				}
			},
			watch: {
				Fleets: {
					handler(val: FleetData[]) {
						const font = `14px ${getComputedStyle(document.body)["fontFamily"]}`;
						// See overview.scss:137

						val.forEach(fleet => {
							const ships = fleet.Ships;
							fleet.SupplyFuel = ships.reduce((a, c) => a + c.UsedFuel, 0);
							fleet.SupplyAmmo = ships.reduce((a, c) => a + c.UsedAmmo, 0);
							fleet.SupplyBauxite = ships.reduce((a, c) => a + c.UsedBauxite, 0);
							fleet.TotalLevel = fleet.Ships
								.reduce((a, c) => a + c.Level, 0);
							fleet.AvgLevel = fleet.Ships.length > 0
								? fleet.TotalLevel / fleet.Ships.length
								: 0;

							fleet.NameSize = ships
								.reduce((a, c) => Math.max(a, getTextWidth(this.i18n[c.Info.Name] || c.Info.Name || "???", font)), 0)
								+ 6; // See overview.scss:131
						});
					},
					deep: true
				}
			}
		});
		SettingsClass.Instance.Register({
			Provider: "Overview",
			Name: "ExpDispType",
			DisplayName: "Exp Display Type",
			Value: "Remaining",
			Type: "String",
			Enums: ["Remaining", "Next"],
			Description: "Experience display type"
		});
		SettingsClass.Instance.Register({
			Provider: "Overview",
			Name: "TooltipStatsDisplay",
			DisplayName: "Display Status",
			Value: true,
			Type: "Boolean",
			Description: "Display ship's current status on tooltip"
		});

		SettingsClass.Instance.Observe("Overview.ExpDispType", value => this.Data.ExpDispType = value.toString());
		SettingsClass.Instance.Observe("Overview.TooltipStatsDisplay", value => this.Data.TooltipStatsDisplay = <boolean>value);
		this.Data.ExpDispType = Settings.Overview.ExpDispType.Value.toString();
		this.Data.TooltipStatsDisplay = <boolean>Settings.Overview.TooltipStatsDisplay.Value;
	}

	private SelectFleet(id: number): void {
		this.Data.SelectedTab = id;
	}

	init(): void {
		Homeport.Instance.Observe(() => this.updateFleets(), nameof(Homeport.Instance.Fleets));

		// Repair docks
		Homeport.Instance.RepairDock!.Observe(() => this.updateRepairDocks(), nameof(Homeport.Instance.RepairDock!.Docks));

		// Construction docks
		Homeport.Instance.ConstructionDock!.Observe(() => this.updateConstructionDocks(), nameof(Homeport.Instance.ConstructionDock!.Docks));

		// Setup quests
		Homeport.Instance.Quests!.Observe(() => this.updateQuests(), nameof(Homeport.Instance.Quests!.All));


		window.addEventListener("resize", () => this.updateSize());
		this.updateSize();

		window.modules.areas.register("side", "overview", "Overview", "", "overview-component");
	}

	private GetRemainingText(remaining: number): string {
		const asSecs = Math.floor(remaining / 1000);
		const hours = Math.floor((asSecs / 3600) % 60);
		const mins = Math.floor((asSecs / 60) % 60);
		const secs = (asSecs % 60);
		return `${("0" + hours).slice(-2)}:${("0" + mins).slice(-2)}:${("0" + secs).slice(-2)}`;
	}

	private updateSize() {
		const el = [
			$('.fleet[data-display="1"] .ship td:first-of-type'),
			$('.fleet[data-display="1"] .ship td:last-of-type')
		];
		if (el[0].length === 0 || el[1].length === 0) return;

		const leftSize = el[0][0].clientWidth;
		const rightSize = el[1][0].clientWidth;
		const modes = [];
		if (rightSize < 128) modes.push("mini");
		if (rightSize < 84) modes.push("tiny");
		if (rightSize < 76) modes.push("minimal");
		if (leftSize > 104) modes.push("mininame");
		$("#overview-container").attr("data-modes", modes.join(" "));
	}

	private updateFleets(): void {
		const newData: FleetData[] = [];
		Homeport.Instance.Fleets.forEach(x => {
			if (x === null) return;

			const fleet: FleetData = {
				Id: x.Id,
				State: "empty",
				Ships: [],

				TotalLevel: 0,
				AvgLevel: 0,
				Speed: "",
				LoS: "0.00",
				AA: new AirSupremacy(0, 0),

				SupplyFuel: 0,
				SupplyAmmo: 0,
				SupplyBauxite: 0,

				IsConditionRestoring: false,
				ConditionRestoringText: "--:--:--",

				NameSize: 0,

				_Observations: []
			};
			x.Observe((_, value: FleetState) => {
				let state = "empty"; // Empty fleet (no ships)

				if ((value & FleetState.Sailing) !== 0) // In sortie
					state = "sortie";
				else if ((value & FleetState.Expedition) !== 0) // In expedition
					state = "expedition";

				else if ((value & FleetState.HeavilyDamaged) !== 0) // Homeport, Heavily damaged
					state = "damaged";
				else if ((value & FleetState.NeedSupply) !== 0) // Homeport, Short supply
					state = "not-ready";
				else if ((value & FleetState.Repairing) !== 0) // Homeport, Repairing
					state = "not-ready";
				else if ((value & FleetState.FlagshipRepairShip) !== 0) // Homeport, Flagship is repairship
					state = "not-ready";
				else if ((value & FleetState.ConditionRestoring) !== 0) // Homeport, Condition restoring
					state = "not-ready";

				else if ((value & FleetState.Homeport) !== 0) // Homeport, ready
					state = "ready";

				fleet.State = state;
			}, nameof(x.State));
			x.Observe((_, value: 0 | 5 | 10 | 15 | 20) => fleet.Speed = window.i18n[this.speedList[value]], nameof(x.FleetSpeed));

			x.Observe((_, value: number) => fleet.LoS = value.toFixed(2), nameof(x.LoS));
			x.Observe((_, value: AirSupremacy) => fleet.AA = this.AAConvert(value), nameof(x.AirSupremacy));
			x.Observe((_, value: boolean) => fleet.IsConditionRestoring = value, nameof(x.IsConditionRestoring));
			x.Observe((_, value: number) => fleet.ConditionRestoringText = this.GetRemainingText(value), nameof(x.ConditionRestoreTime));
			x.Observe((_, value: Ship[]) => {
				fleet.Ships = value;

				fleet._Observations.forEach(y => y[0].Deobserve(y[1]));
				value.forEach(x => {
					const f: ObservableCallback = (_, value) => {
						this.Data.Fleets = newData; // Update?
					};
					x.Observe(f);
					fleet._Observations.push([x, f]);
				});
			}, nameof(x.Ships));
			newData.push(fleet);
		});

		this.Data.Fleets = newData;
		if (newData.length <= this.Data.SelectedTab && newData.length > 0)
			this.SelectFleet(0);
	}
	private updateRepairDocks(): void {
		const docks: RepairDockData[] = [];
		Homeport.Instance.RepairDock!.Docks.forEach(x => {
			const dock: RepairDockData = {
				Id: x.Id,
				Ship: "???",
				RemainingTime: "--:--:--",
				IsCompleted: false,
				State: RepairDock.DockState.Locked,
				StateText: ""
			};
			x.Observe((_, value: Ship | null) => dock.Ship = (value && value.Info.Name) || "???", nameof(x.Ship));
			x.Observe((_, value: RepairDock.DockState) => {
				dock.State = value;
				dock.StateText = RepairDock.DockState[value].toLowerCase();
			}, nameof(x.State));
			x.Observe((_, value: number) => {
				dock.IsCompleted = value <= 0;
				dock.RemainingTime = this.GetRemainingText(value);
			}, nameof(x.Remaining));

			docks.push(dock);
		});

		this.Data.RepairDock = docks;
	}
	private updateConstructionDocks(): void {
		const docks: ConstructionDockData[] = [];
		Homeport.Instance.ConstructionDock!.Docks.forEach(x => {
			const dock: ConstructionDockData = {
				Id: x.Id,
				Ship: "???",
				RemainingTime: "--:--:--",
				IsCompleted: false,
				State: ConstructionDock.DockState.Locked,
				StateText: ""
			};
			x.Observe((_, value: ShipInfo) => dock.Ship = (value && value.Name) || "???", nameof(x.Ship));
			x.Observe((_, value: ConstructionDock.DockState) => {
				dock.State = value;
				dock.StateText = ConstructionDock.DockState[value].toLowerCase();
			}, nameof(x.State));
			x.Observe((_, value: number) => {
				dock.IsCompleted = value <= 0;
				dock.RemainingTime = this.GetRemainingText(value);
			}, nameof(x.Remaining));

			docks.push(dock);
		});

		this.Data.ConstructionDock = docks;
	}
	private updateQuests():void {
		this.Data.Quests = Homeport.Instance.Quests!.All;
	}

	private AAConvert(AA: AirSupremacy): AirSupremacy{
		return new AirSupremacy(
			parseFloat(AA.Minimum.toFixed(2)),
			parseFloat(AA.Maximum.toFixed(2))
		);
	}
}

window.modules.register("overview", new Overview());
export default Overview;