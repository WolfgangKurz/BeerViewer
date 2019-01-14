import { SubscribeKcsapi } from "../Base/KcsApi";
import { kcsapi_port } from "../Interfaces/kcsapi_port";
import { kcsapi_basic } from "../Interfaces/kcsapi_basic";
import { kcsapi_member_updatecomment } from "../Interfaces/kcsapi_member";

export class Admiral extends RawDataWrapper<kcsapi_basic> {
    public get MemberId(): string { return this.raw.api_member_id }
    public get Nickname(): string { return this.raw.api_nickname }
    public get Comment(): string { return this.raw.api_comment }

    public get Level(): number { return this.raw.api_level }
    public get Rank(): AdmiralRank { return this.raw.api_rank }
    public get Experience(): number { return this.raw.api_experience }
    public get ExperienceForNextLevel(): number {
        return Experience.Admiral[this.raw.api_level + 1].Total
            - this.raw.api_experience;
    }

    public get SortieWins(): number { return this.raw.api_st_win }
    public get SortieLoses(): number { return this.raw.api_st_lose }
    public get SortieWinningRate(): number {
        const battleCount = this.SortieWins + this.SortieLoses;
        return battleCount === 0 ? 0 : this.SortieWins / battleCount;
    }

    public get MaximumShips(): number { return this.raw.api_max_chara }
    public get MaximumEquips(): number { return this.raw.api_max_slotitem }

    public get ResourceLimit(): number { return (this.raw.api_level + 3) * 250 }

    constructor(data: kcsapi_basic) {
        super(data);

        SubscribeKcsapi<kcsapi_port>(
            "api_port/port",
            x => this.UpdateData(x.api_basic)
        );
        SubscribeKcsapi<{}, kcsapi_member_updatecomment>(
            "api_req_member/updatecomment",
            (x, y) => this.raw.api_comment = y.api_cmt
        );
    }
}