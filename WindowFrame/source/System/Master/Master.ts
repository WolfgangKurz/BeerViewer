import { SubscribeKcsapi } from "../Base/KcsApi";
import { kcsapi_start2 } from "../Interfaces/Master/kcsapi_start2";

export class Master {
    public static Instance: Master = new Master();
    private IsReady: boolean = false;

    public Ready(): void {
        if (this.IsReady) return;
        this.IsReady = true;

        SubscribeKcsapi<kcsapi_start2>("/api_start2", x => {

        });
    }
}