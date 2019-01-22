import { IDisposable } from "../Base/Interfaces/IDisposable";
import { Master } from "../Master/Master";
import { Homeport } from "../Homeport/Homeport";
import { Settings } from "../Settings";

export type HTTPCallback = (Response: string, Request: HTTPRequest) => void;

declare global {
    export interface Window {
        API: Communicator;
        INTERNAL: Internal;

        i18n: { [key: string]: string };
        _i18n(text: string): Promise<string>;

        Master: Master;
        Homeport: Homeport;
        Settings: Settings;
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
    SubscribeHTTP(url: string, callback: HTTPCallback): number;

    /** Unsubscribe HTTP packet observer with id that returned from `SubscribeHTTP` */
    UnsubscribeHTTP(SubscribeId: number): boolean;
}

/** Callable from Communicator */
export interface Internal {
    Initialized(): Promise<void>;
    zoomMainFrame(zoomFactor: number | string): void;
    loadMainFrame(url: string): void;
}

/** Simple module information for module.js */
export interface ModuleInfo {
    /** Name of module */
    Name: string;

    /** Content of *.html if exists, empty otherwise. */
    Template: string;

    /** *.js exists? */
    Scripted: boolean;

    /** *.css exists? */
    Styled: boolean;
}

/** Subscribe information */
export class SubscribeInfo implements IDisposable {
    private _Id: number;
    public get Id(): number { return this._Id }

    constructor(Id: number) {
        this._Id = Id;
    }

    Dispose(): void {
        if (this.Id >= 0)
            window.API.UnsubscribeHTTP(this.Id);
    }
}

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