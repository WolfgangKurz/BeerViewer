export default class AirBattleResult {
	private _Name: string;
	public get Name(): string { return this._Name }

	private _IsHappen: boolean;
	public get IsHappen(): boolean { return this._IsHappen }

	private _AliasCount: number;
	public get AliasCount(): number { return this._AliasCount }
	private _AliasLostCount: number;
	public get AliasLostCount(): number { return this._AliasLostCount }

	public get AliasRemainingCount(): number { return this.AliasCount - this.AliasLostCount }

	private _EnemyCount: number;
	public get EnemyCount(): number { return this._EnemyCount }
	private _EnemyLostCount: number;
	public get EnemyLostCount(): number { return this._EnemyLostCount }

	public get EnemyRemainingCount(): number { return this.EnemyCount - this.EnemyLostCount }

	constructor( name: string);
	constructor(name: string, fCount:number, fLost:number, eCount:number, eLost:number, isHappen:boolean);
	constructor(name: string, fCount?:number, fLost?:number, eCount?:number, eLost?:number, isHappen?:boolean) {
		this._Name = name;
		this._IsHappen = isHappen || false;
		this._AliasCount = fCount || 0;
		this._AliasLostCount = fLost || 0;
		this._EnemyCount = eCount || 0;
		this._EnemyLostCount = eLost || 0;
	}
}