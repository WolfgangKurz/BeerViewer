import { EventKind, EventId } from "./Enums";

export interface kcsapi_map_start {
	api_maparea_id: number;
	api_mapinfo_no: number;
	api_no: number;

	api_event_id: EventId;
	api_event_kind: EventKind;
	api_eventmap: any;

	/** If zero, no nodes to go (End of map reached) */
	api_next: number;
}
export interface kcsapi_map_next extends kcsapi_map_start {
	api_m1?: number; // Map extended?
}
