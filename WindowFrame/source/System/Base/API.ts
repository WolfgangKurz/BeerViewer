import { fns } from "./Base";

export class BaseAPI {
    private callbacks: { [key: string]: ((value: any) => void)[] } = {};
    constructor() {
        const _this = this;
        const _origin_i18n: { [key: string]: string } = {};
        window.INTERNAL = {
            async Initialized() {
                const set = await window.API.i18nSet();
                const props = Object.getOwnPropertyNames(_origin_i18n);
                for (let i = 0; i < props.length; i++) delete _origin_i18n[props[i]];
                for (let i in set) _origin_i18n[i] = set[i];
            },
            zoomMainFrame(_zoomFactor: number | string) {
                const zoomFactor = typeof _zoomFactor === "number"
                    ? _zoomFactor / 100
                    : parseFloat(_zoomFactor) / 100;

                $("#MAIN_FRAME")
                    .css("transform", `scale(${zoomFactor})`)
                    .css("marginRight", 1200 * (zoomFactor - 1) + "px")
                    .css("marginBottom", 720 * (zoomFactor - 1) + "px");

                fns(_this.callbacks.zoomMainFrame, zoomFactor);
            },
            loadMainFrame(url: string) {
                $("#MAIN_FRAME").prop("src", url);

                fns(_this.callbacks.loadMainFrame, url);
            }
        };
        window.i18n
            = new Proxy(_origin_i18n, {
                get: function (target, name) {
                    if (name in target) return target[name.toString()];
                    return name;
                }
            });
        window._i18n
            = async function (text: string): Promise<string> {
                return await window.API.i18n(text);
            };
    }

    Event(type: string, callback: (value: any) => void): void {
        if (!(type in this.callbacks))
            this.callbacks[type] = [];

        this.callbacks[type].push(callback);
    }
}
export default BaseAPI;