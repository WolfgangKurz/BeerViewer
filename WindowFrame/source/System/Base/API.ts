type HTTPRequest = { [name: string]: string | number };
type HTTPCallback = (Response: string, Request: HTTPRequest) => void;

interface Window {
    API: Communicator;
}

/** Simple module information for module.js */
interface ModuleInfo {
    /** Name of module */
    Name: string;

    /** Content of *.html if exists, empty otherwise. */
    Template: string;

    /** *.js exists? */
    Scripted: boolean;

    /** *.css exists? */
    Styled: boolean;
}
/** Functions on Communicator(window.API) */
interface Communicator {
    /** Notice to communicator that browser has initialized. */
    Initialized(): void;

    /** Call reserved system "command" */
    SystemCall(command: string): boolean;

    /** Register value change observer to object what registered to communicator.
     * "callback" will be called when changed, and observer registered.
     */
    ObserveData(namespace: string, path: string, callback: Function): void;

    /** Get single data from object what registered to communicator */
    GetData(namespace: string, path: string): any;

    /** Get single i18n text */
    i18n(text: string): string;

    /** Get i18n texts as json table */
    i18nSet(): { [key: string]: string; };

    /** Get all loadable modules */
    GetModuleList(): ModuleInfo[];

    /** Open DevTools for browser */
    DevTools(): void;

    /** Deliver "text" to logger */
    Log(text: string): void;

    /** Subscribe browser HTTP packets for specific "url" */
    SubscribeHTTP(url: string, callback: HTTPCallback): void;
}
