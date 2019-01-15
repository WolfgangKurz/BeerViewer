import { Observable } from "../Base/Observable";

export class Ship extends Observable {
    public Situation: Ship.Situation;

    constructor(){
        super();
        this.Situation = Ship.Situation.None;
    }

    public Repair(){

    }
}
export namespace Ship {
    export enum Situation {
        /** Nothing special */
        None = 0,
        
        /** Repairing */
        Repairing = 1,
        
        /** Damaged ship */
        Evacuation = 1 << 1,

        /** Escort ship */
        Tow = 1 << 2,
        
        /** Damaged, not evacuated */
        HeavilyDamaged = 1 << 3,
        
        /** Damaged, and Damage control used */
		DamageControlled = 1 << 4
    }
}