export class BaseAPI {
    constructor() {
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
                const frame: HTMLElement | null = document.querySelector("#MAIN_FRAME");
                if (frame === null) return;

                frame.style["transform"] = `scale(${zoomFactor})`;
                frame.style["marginRight"] = 1200 * (zoomFactor - 1) + "px";
                frame.style["marginBottom"] = 720 * (zoomFactor - 1) + "px";
            },
            loadMainFrame(url: string) {
                const frame: HTMLElement | null = document.querySelector("#MAIN_FRAME");
                if (frame === null) return;

                (<HTMLIFrameElement>frame).src = url;
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
}
export default BaseAPI;