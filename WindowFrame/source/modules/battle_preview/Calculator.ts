import FleetData from "./Models/FleetData";
import ShipData from "./Models/ShipData";

class ShipBattleInfo {
	public IsEnemy: boolean = false;

	constructor(calculator: Calculator, index: number, ship: ShipData){

	}
}

export default class Calculator {
	private Ships = {
		Alias: {
			First: <ShipBattleInfo[] | null>null,
			Second: <ShipBattleInfo[] | null>null
		},
		Enemy: {
			First: <ShipBattleInfo[] | null>null,
			Second: <ShipBattleInfo[] | null>null
		}
	};

	public Initialize(aliasFirst: FleetData, aliasSecond: FleetData, enemyFirst: FleetData, enemySecond: FleetData): Calculator {
		// Alias fleets
		const aliasShipsFirst = aliasFirst.Ships;

		this.Ships.Alias.First = [];
		for (var i = 0; i < aliasShipsFirst.length; i++)
			this.Ships.Alias.First[i] = new ShipBattleInfo(this, i + 1, aliasShipsFirst[i]);

		if (aliasSecond == null || aliasSecond.Ships.length == 0) {
			// Single fleet
			this.Ships.Alias.Second = null;
		}
		else {
			// Combined fleet
			const aliasShipsSecond = aliasSecond.Ships;
			this.Ships.Alias.Second = [];

			for (var i = 0; i < aliasShipsSecond.length; i++)
				this.Ships.Alias.Second[i] = new ShipBattleInfo(this, 6 + i + 1, aliasShipsSecond[i]);
		}

		// Enemy fleets
		const enemyShipsFirst = enemyFirst.Ships;

		this.Ships.Enemy.First = [];
		for (var i = 0; i < enemyShipsFirst.length; i++){
			this.Ships.Enemy.First[i] = new ShipBattleInfo(this, i + 1, enemyShipsFirst[i]);
			this.Ships.Enemy.First[i].IsEnemy = true;
		}

		if (enemySecond == null || enemySecond.Ships.length == 0) {
			// Single fleet
			this.Ships.Enemy.Second = null;
		}
		else {
			// Combined fleet
			var enemyShipsSecond = enemySecond.Ships;
			this.Ships.Enemy.Second = [];

			for (var i = 0; i < enemyShipsSecond.length; i++) {
				this.Ships.Enemy.Second[i] = new ShipBattleInfo(this, 6 + i + 1, enemyShipsSecond[i]);
				this.Ships.Enemy.Second[i].IsEnemy = true;
			}
		}

		// Clear damage logs
		// this._AllDamageLog.Clear();

		// Event
		// Updated?.Invoke(this, EventArgs.Empty);

		return this;
	}
}