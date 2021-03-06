import { DisposableObservableDataWrapper } from "System/Base/Wrapper";
import { SubscribeKcsapi } from "System/Base/KcsApi";
import { kcsapi_port } from "System/Interfaces/kcsapi_port";
import { kcsapi_basic } from "System/Interfaces/kcsapi_basic";
import { kcsapi_member_updatecomment } from "System/Interfaces/kcsapi_member";

export class Admiral extends DisposableObservableDataWrapper<kcsapi_basic> {
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

		this.ManagedDisposable.Add(
			SubscribeKcsapi<kcsapi_port>(
				"api_port/port",
				x => this.UpdateData(x.api_basic)
			)
		);
		this.ManagedDisposable.Add(
			SubscribeKcsapi<{}, kcsapi_member_updatecomment>(
				"api_req_member/updatecomment",
				(x, y) => {
					this.raw.api_comment = y.api_cmt;
					this.RaisePropertyChanged(nameof(this.Comment));
				}
			)
		);
	}
}