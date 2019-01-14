import { Admiral } from "./Admiral";
import { Materials } from "./Materials";
import { Ship } from "./Ship";
import { kcsapi_basic } from "../Interfaces/kcsapi_basic";

export class Homeport {
    public static readonly Instance: Homeport = new Homeport;

    public Admiral: Admiral | null = null;
    public Materials: Materials | null = null;
    public Ships: Ship[] = [];

    constructor() {
    }
}