import { Master } from "System/Master/Master";
import { Homeport } from "System/Homeport/Homeport";

export type HTTPCallback = (Response: string, Request: { [key: string]: string }) => void;

declare global {
	export interface Window {
		API: Communicator;
		INTERNAL: Internal;

		i18n: { [key: string]: string };
		_i18n(text: string): Promise<string>;

		Master: Master;
		Homeport: Homeport;
	}
}

/** Functions on Communicator(window.API) */
export interface Communicator {
	/** Notice to communicator that browser has initialized. */
	Initialized(): void;

	/** Call reserved system "command" */
	SystemCall(command: string): Promise<boolean>;

	/** Register value change observer to object what registered to communicator.
	 * "callback" will be called when changed, and observer registered.
	 */
	ObserveData(namespace: string, path: string, callback: Function): void;

	/** Get single data from object what registered to communicator */
	GetData(namespace: string, path: string): Promise<any>;

	/** Get single i18n text */
	i18n(text: string): Promise<string>;

	/** Get i18n texts as json table */
	i18nSet(): Promise<{ [key: string]: string; }>;

	/** Get all loadable modules */
	GetModuleList(): Promise<ModuleInfo[]>;

	/** Open DevTools for browser */
	DevTools(): void;

	/** Deliver "text" to logger */
	Log(text: string): void;

	/** Subscribe HTTP packets for specific "url" */
	SubscribeHTTP(url: string, callback: HTTPCallback): Promise<number>;

	/** Unsubscribe HTTP packet observer with id that returned from `SubscribeHTTP` */
	UnsubscribeHTTP(SubscribeId: number): Promise<boolean>;

	/** Get all settable settings list */
	GetSettings(): Promise<SettingInfo[]>;

	/** Save settings has updated */
	UpdateSetting(Provider: string, Name: string, Value: any): Promise<boolean>;

	/** Reload current page of Main game frame */
	ReloadMainFrame(): void;

	/** Notify that Main frame has resized */
	NotifyMainFrameResized(width: number, height: number): void;
}

/** Callable from Communicator */
export interface Internal {
	Initialized(): Promise<void>;
}

/** Simple module information for `Module.ts` */
export interface ModuleInfo {
	/** Name of module */
	Name: string;

	/** Raw name of module */
	RawName: string;

	/** Content of *.html if exists, empty otherwise. */
	Template: string;

	/** *.js exists? */
	Scripted: boolean;

	/** *.css exists? */
	Styled: boolean;
}

/** Setting information for `Settings` built-in module */
export interface SettingInfo {
	Type: string;

	Name: string;
	Provider: string;
	Value: string | number | boolean;

	DisplayName: string;
	Description: string | null | undefined;
	Caution: string | null | undefined;

	Enums: any[] | null | undefined;
}

/** Requset data for HTTP request/response */
export class HTTPRequest {
	[key: string]: string[] | string | number;

	constructor(data: any) {
		if (data === undefined || data === null) return;

		for (let key in data) {
			const value = data[key], nvalue = Number(value);
			if (nvalue.toString() === value)
				this[key] = nvalue; // Real numeric
			else
				this[key] = value;
		}
	}
}
