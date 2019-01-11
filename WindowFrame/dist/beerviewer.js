"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
define("vendor/vue.tippy", ["require", "exports", "vue", "tippy.js"], function (require, exports, vue_1, tippy_js_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    vue_1 = __importDefault(vue_1);
    tippy_js_1 = __importDefault(tippy_js_1);
    const TIPPY = function (el, binding, vnode) {
        const handlers = (vnode.data && vnode.data.on) || (vnode.componentOptions && vnode.componentOptions.listeners);
        const opts = Object.assign({ reactive: false, showOnLoad: false }, binding.value || {});
        if (handlers && handlers["show"])
            opts.onShow = () => handlers["show"].fns(el, vnode);
        if (handlers && handlers["shown"])
            opts.onShown = () => handlers["shown"].fns(el, vnode);
        if (handlers && handlers["hidden"])
            opts.onHidden = () => handlers["hidden"].fns(el, vnode);
        if (handlers && handlers["hide"])
            opts.onHide = () => handlers["hide"].fns(el, vnode);
        if (opts.html) {
            const selector = opts.html;
            if (opts.reactive || !(typeof selector === "string")) {
                opts.html = selector instanceof Element
                    ? selector
                    : selector instanceof vue_1.default
                        ? selector.$el
                        : document.querySelector(selector);
            }
            else {
                const elem = document.querySelector(opts.html);
                if (elem) {
                    if (elem._tipppyReferences)
                        elem._tipppyReferences.push(el);
                    else
                        elem._tipppyReferences = [el];
                }
                else {
                    console.error(`[TippyExt] Selector "${opts.html}" not found`);
                    return;
                }
            }
        }
        {
            let opt = Object.assign({}, tippy_js_1.default.defaults, opts);
            for (let i in opt)
                if (!(i in tippy_js_1.default.defaults))
                    delete opt[i];
            tippy_js_1.default(el, opt);
        }
        if (opts.showOnLoad)
            el._tippy.show();
        vue_1.default.nextTick(() => {
            if (handlers && handlers["init"])
                handlers["init"].fns(el._tippy, el);
        });
    };
    const VueTippy = {
        install(Vue, options) {
            Vue.directive("tippy-target", {
                componentUpdated: function (el) {
                    const refs = el._tipppyReferences;
                    if (refs && refs.length > 0) {
                        Vue.nextTick(() => {
                            refs.filter((x) => x._tippy)
                                .forEach((ref) => {
                                if (ref._tippy) {
                                    const content = ref._tippy.popper.querySelector(".tippy-content");
                                    content.innerHTML = el.innerHTML;
                                }
                            });
                        });
                    }
                },
                unbind: function (el) {
                    delete el._tipppyReference;
                }
            });
            Vue.directive("tippy", {
                inserted: function (el, binding, vnode) {
                    Vue.nextTick(() => TIPPY(el, binding, vnode));
                },
                unbind: function (el) {
                    if (el._tippy)
                        el._tippy.destroy();
                },
                componentUpdated: function (el, binding, vnode) {
                    const ts = JSON.stringify;
                    const opts = binding.value || {};
                    const oldOpts = binding.oldValue || {};
                    if (el._tippy) {
                        if (ts(opts) !== ts(oldOpts))
                            Vue.nextTick(() => TIPPY(el, binding, vnode));
                        if (el._tippy.popperInstance) {
                            if (opts.show)
                                el._tippy.show();
                            else if (!opts.show && opts.trigger === "manual")
                                el._tippy.hide();
                        }
                    }
                }
            });
        }
    };
    exports.default = VueTippy;
});
define("base", ["require", "exports", "tippy.js", "vue", "vendor/vue.tippy"], function (require, exports, tippy_js_2, vue_2, vue_tippy_js_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    tippy_js_2 = __importDefault(tippy_js_2);
    vue_2 = __importDefault(vue_2);
    vue_tippy_js_1 = __importDefault(vue_tippy_js_1);
    tippy_js_2.default.setDefaults({
        arrow: true,
        boundary: "viewport",
        placement: "bottom",
        size: "large",
        theme: "light-border",
        interactive: false,
        trigger: 'mouseenter focus',
        hideOnClick: false,
        performance: true
    });
    vue_2.default.directive("dom", function (el, binding) {
        if (binding.oldValue && binding.oldValue instanceof Element && binding.oldValue.parentNode === el)
            binding.oldValue.remove();
        if (binding.value instanceof Element)
            el.append(binding.value);
    });
    vue_2.default.use(vue_tippy_js_1.default, tippy_js_2.default.defaults);
});
var Experience;
(function (Experience) {
    Experience.Admiral = {
        1: { Level: 1, Next: 100, Total: 0 },
        2: { Level: 2, Next: 200, Total: 100 },
        3: { Level: 3, Next: 300, Total: 300 },
        4: { Level: 4, Next: 400, Total: 600 },
        5: { Level: 5, Next: 500, Total: 1000 },
        6: { Level: 6, Next: 600, Total: 1500 },
        7: { Level: 7, Next: 700, Total: 2100 },
        8: { Level: 8, Next: 800, Total: 2800 },
        9: { Level: 9, Next: 900, Total: 3600 },
        10: { Level: 10, Next: 1000, Total: 4500 },
        11: { Level: 11, Next: 1100, Total: 5500 },
        12: { Level: 12, Next: 1200, Total: 6600 },
        13: { Level: 13, Next: 1300, Total: 7800 },
        14: { Level: 14, Next: 1400, Total: 9100 },
        15: { Level: 15, Next: 1500, Total: 10500 },
        16: { Level: 16, Next: 1600, Total: 12000 },
        17: { Level: 17, Next: 1700, Total: 13600 },
        18: { Level: 18, Next: 1800, Total: 15300 },
        19: { Level: 19, Next: 1900, Total: 17100 },
        20: { Level: 20, Next: 2000, Total: 19000 },
        21: { Level: 21, Next: 2100, Total: 21000 },
        22: { Level: 22, Next: 2200, Total: 23100 },
        23: { Level: 23, Next: 2300, Total: 25300 },
        24: { Level: 24, Next: 2400, Total: 27600 },
        25: { Level: 25, Next: 2500, Total: 30000 },
        26: { Level: 26, Next: 2600, Total: 32500 },
        27: { Level: 27, Next: 2700, Total: 35100 },
        28: { Level: 28, Next: 2800, Total: 37800 },
        29: { Level: 29, Next: 2900, Total: 40600 },
        30: { Level: 30, Next: 3000, Total: 43500 },
        31: { Level: 31, Next: 3100, Total: 46500 },
        32: { Level: 32, Next: 3200, Total: 49600 },
        33: { Level: 33, Next: 3300, Total: 52800 },
        34: { Level: 34, Next: 3400, Total: 56100 },
        35: { Level: 35, Next: 3500, Total: 59500 },
        36: { Level: 36, Next: 3600, Total: 63000 },
        37: { Level: 37, Next: 3700, Total: 66600 },
        38: { Level: 38, Next: 3800, Total: 70300 },
        39: { Level: 39, Next: 3900, Total: 74100 },
        40: { Level: 40, Next: 4000, Total: 78000 },
        41: { Level: 41, Next: 4100, Total: 82000 },
        42: { Level: 42, Next: 4200, Total: 86100 },
        43: { Level: 43, Next: 4300, Total: 90300 },
        44: { Level: 44, Next: 4400, Total: 94600 },
        45: { Level: 45, Next: 4500, Total: 99000 },
        46: { Level: 46, Next: 4600, Total: 103500 },
        47: { Level: 47, Next: 4700, Total: 108100 },
        48: { Level: 48, Next: 4800, Total: 112800 },
        49: { Level: 49, Next: 4900, Total: 117600 },
        50: { Level: 50, Next: 5000, Total: 122500 },
        51: { Level: 51, Next: 5200, Total: 127500 },
        52: { Level: 52, Next: 5400, Total: 132700 },
        53: { Level: 53, Next: 5600, Total: 138100 },
        54: { Level: 54, Next: 5800, Total: 143700 },
        55: { Level: 55, Next: 6000, Total: 149500 },
        56: { Level: 56, Next: 6200, Total: 155500 },
        57: { Level: 57, Next: 6400, Total: 161700 },
        58: { Level: 58, Next: 6600, Total: 168100 },
        59: { Level: 59, Next: 6800, Total: 174700 },
        60: { Level: 60, Next: 7000, Total: 181500 },
        61: { Level: 61, Next: 7300, Total: 188500 },
        62: { Level: 62, Next: 7600, Total: 195800 },
        63: { Level: 63, Next: 7900, Total: 203400 },
        64: { Level: 64, Next: 8200, Total: 211300 },
        65: { Level: 65, Next: 8500, Total: 219500 },
        66: { Level: 66, Next: 8800, Total: 228000 },
        67: { Level: 67, Next: 9100, Total: 236800 },
        68: { Level: 68, Next: 9400, Total: 245900 },
        69: { Level: 69, Next: 9700, Total: 255300 },
        70: { Level: 70, Next: 10000, Total: 265000 },
        71: { Level: 71, Next: 10400, Total: 275000 },
        72: { Level: 72, Next: 10800, Total: 285400 },
        73: { Level: 73, Next: 11200, Total: 296200 },
        74: { Level: 74, Next: 11600, Total: 307400 },
        75: { Level: 75, Next: 12000, Total: 319000 },
        76: { Level: 76, Next: 12400, Total: 331000 },
        77: { Level: 77, Next: 12800, Total: 343400 },
        78: { Level: 78, Next: 13200, Total: 356200 },
        79: { Level: 79, Next: 13600, Total: 369400 },
        80: { Level: 80, Next: 14000, Total: 383000 },
        81: { Level: 81, Next: 14500, Total: 397000 },
        82: { Level: 82, Next: 15000, Total: 411500 },
        83: { Level: 83, Next: 15500, Total: 426500 },
        84: { Level: 84, Next: 16000, Total: 442000 },
        85: { Level: 85, Next: 16500, Total: 458000 },
        86: { Level: 86, Next: 17000, Total: 474500 },
        87: { Level: 87, Next: 17500, Total: 491500 },
        88: { Level: 88, Next: 18000, Total: 509000 },
        89: { Level: 89, Next: 18500, Total: 527000 },
        90: { Level: 90, Next: 19000, Total: 545500 },
        91: { Level: 91, Next: 20000, Total: 564500 },
        92: { Level: 92, Next: 22000, Total: 584500 },
        93: { Level: 93, Next: 25000, Total: 606500 },
        94: { Level: 94, Next: 30000, Total: 631500 },
        95: { Level: 95, Next: 40000, Total: 661500 },
        96: { Level: 96, Next: 60000, Total: 701500 },
        97: { Level: 97, Next: 90000, Total: 761500 },
        98: { Level: 98, Next: 148500, Total: 851500 },
        99: { Level: 99, Next: 300000, Total: 1000000 },
        100: { Level: 100, Next: 0, Total: 1000000 },
        101: { Level: 101, Next: 10000, Total: 1010000 },
        102: { Level: 102, Next: 1000, Total: 1011000 },
        103: { Level: 103, Next: 2000, Total: 1013000 },
        104: { Level: 104, Next: 3000, Total: 1016000 },
        105: { Level: 105, Next: 4000, Total: 1020000 },
        106: { Level: 106, Next: 5000, Total: 1025000 },
        107: { Level: 107, Next: 6000, Total: 1031000 },
        108: { Level: 108, Next: 7000, Total: 1038000 },
        109: { Level: 109, Next: 8000, Total: 1046000 },
        110: { Level: 110, Next: 9000, Total: 1055000 },
        111: { Level: 111, Next: 10000, Total: 1065000 },
        112: { Level: 112, Next: 12000, Total: 1077000 },
        113: { Level: 113, Next: 14000, Total: 1091000 },
        114: { Level: 114, Next: 16000, Total: 1107000 },
        115: { Level: 115, Next: 18000, Total: 1125000 },
        116: { Level: 116, Next: 20000, Total: 1145000 },
        117: { Level: 117, Next: 23000, Total: 1168000 },
        118: { Level: 118, Next: 26000, Total: 1194000 },
        119: { Level: 119, Next: 29000, Total: 1223000 },
        120: { Level: 120, Next: 32000, Total: 1255000 },
        121: { Level: 121, Next: 35000, Total: 1290000 },
        122: { Level: 122, Next: 39000, Total: 1329000 },
        123: { Level: 123, Next: 43000, Total: 1372000 },
        124: { Level: 124, Next: 47000, Total: 1419000 },
        125: { Level: 125, Next: 51000, Total: 1470000 },
        126: { Level: 126, Next: 55000, Total: 1525000 },
        127: { Level: 127, Next: 59000, Total: 1584000 },
        128: { Level: 128, Next: 63000, Total: 1647000 },
        129: { Level: 129, Next: 67000, Total: 1714000 },
        130: { Level: 130, Next: 71000, Total: 1785000 },
        131: { Level: 131, Next: 75000, Total: 1860000 },
        132: { Level: 132, Next: 80000, Total: 1940000 },
        133: { Level: 133, Next: 85000, Total: 2025000 },
        134: { Level: 134, Next: 90000, Total: 2115000 },
        135: { Level: 135, Next: 95000, Total: 2210000 },
        136: { Level: 136, Next: 100000, Total: 2310000 },
        137: { Level: 137, Next: 105000, Total: 2415000 },
        138: { Level: 138, Next: 110000, Total: 2525000 },
        139: { Level: 139, Next: 115000, Total: 2640000 },
        140: { Level: 140, Next: 120000, Total: 2760000 },
        141: { Level: 141, Next: 127000, Total: 2887000 },
        142: { Level: 142, Next: 134000, Total: 3021000 },
        143: { Level: 143, Next: 141000, Total: 3162000 },
        144: { Level: 144, Next: 148000, Total: 3310000 },
        145: { Level: 145, Next: 155000, Total: 3465000 },
        146: { Level: 146, Next: 163000, Total: 3628000 },
        147: { Level: 147, Next: 171000, Total: 3799000 },
        148: { Level: 148, Next: 179000, Total: 3978000 },
        149: { Level: 149, Next: 187000, Total: 4165000 },
        150: { Level: 150, Next: 195000, Total: 4360000 },
        151: { Level: 151, Next: 204000, Total: 4564000 },
        152: { Level: 152, Next: 213000, Total: 4777000 },
        153: { Level: 153, Next: 222000, Total: 4999000 },
        154: { Level: 154, Next: 231000, Total: 5230000 },
        155: { Level: 155, Next: 240000, Total: 5470000 }
    };
})(Experience || (Experience = {}));
class Settings {
}
function fns(Handler, ...Params) {
    if (Handler) {
        if (Handler.length)
            Handler.forEach(x => typeof x === "function" && x(...Params));
        else if (typeof Handler === "function")
            Handler(...Params);
    }
}
function using(resource, func) {
    try {
        func(resource);
    }
    finally {
        resource.Dispose();
    }
}
function SubscribeKcsapi(url, callback) {
    window.API.SubscribeHTTP("/kcsapi/" + url, (x, y) => {
        try {
            const svdata = x.startsWith("svdata=") ? x.substr(7) : x;
            const json = JSON.parse(svdata.toString());
            if (json.api_result === 1 && callback)
                callback(json.api_data, y);
        }
        catch (e) {
            console.warn("Expected json, but not.", e);
        }
    });
}
;
class Notifier {
    constructor() {
        this.PropertyChanged = null;
    }
    RaisePropertyChanged(propertyName = null) {
        if (this.PropertyChanged === null)
            return;
        if (this.PropertyChanged.length) {
            const list = this.PropertyChanged;
            list.forEach(x => x(propertyName));
            return;
        }
        this.PropertyChanged(propertyName);
    }
    PropertyEvent(PropertyName, Handler, RaiseRegistered = false) {
        if (this.PropertyChanged === null) {
            this.PropertyChanged = x => {
                if (x === PropertyName && Handler)
                    Handler();
            };
        }
        if (RaiseRegistered && Handler)
            Handler();
    }
}
class TickNotifier extends Notifier {
    constructor() {
        super();
        this.handler = setInterval(this.Tick, 1000);
    }
    Tick() { }
    Dispose() {
        clearInterval(this.handler);
    }
}
class Ticker {
    constructor() {
        this.handler = setInterval(this.Tick, 1000);
    }
    Tick() { }
    Dispose() {
        clearInterval(this.handler);
    }
}
class RawDataWrapper {
    constructor(RawData) {
        this._raw = RawData;
    }
    get raw() {
        return this._raw;
    }
    UpdateData(RawData) {
        this._raw = RawData;
    }
}
var AdmiralRank;
(function (AdmiralRank) {
    AdmiralRank[AdmiralRank["FleetAdmiral"] = 1] = "FleetAdmiral";
    AdmiralRank[AdmiralRank["Admiral"] = 2] = "Admiral";
    AdmiralRank[AdmiralRank["ViceAdmiral"] = 3] = "ViceAdmiral";
    AdmiralRank[AdmiralRank["RearAdmiral"] = 4] = "RearAdmiral";
    AdmiralRank[AdmiralRank["Captain"] = 5] = "Captain";
    AdmiralRank[AdmiralRank["Commander"] = 6] = "Commander";
    AdmiralRank[AdmiralRank["Novice_Commander"] = 7] = "Novice_Commander";
    AdmiralRank[AdmiralRank["Lieutenant_Commander"] = 8] = "Lieutenant_Commander";
    AdmiralRank[AdmiralRank["Vice_Lieutenant_Commander"] = 9] = "Vice_Lieutenant_Commander";
    AdmiralRank[AdmiralRank["Novice_Lieutenant_Commander"] = 10] = "Novice_Lieutenant_Commander";
})(AdmiralRank || (AdmiralRank = {}));
class Admiral extends RawDataWrapper {
    get MemberId() { return this.raw.api_member_id; }
    get Nickname() { return this.raw.api_nickname; }
    get Comment() { return this.raw.api_comment; }
    get Level() { return this.raw.api_level; }
    get Rank() { return this.raw.api_rank; }
    get Experience() { return this.raw.api_experience; }
    get ExperienceForNextLevel() {
        return Experience.Admiral[this.raw.api_level + 1].Total
            - this.raw.api_experience;
    }
    get SortieWins() { return this.raw.api_st_win; }
    get SortieLoses() { return this.raw.api_st_lose; }
    get SortieWinningRate() {
        const battleCount = this.SortieWins + this.SortieLoses;
        return battleCount === 0 ? 0 : this.SortieWins / battleCount;
    }
    get MaximumShips() { return this.raw.api_max_chara; }
    get MaximumEquips() { return this.raw.api_max_slotitem; }
    get ResourceLimit() { return (this.raw.api_level + 3) * 250; }
    constructor(data) {
        super(data);
        SubscribeKcsapi("api_port/port", x => this.UpdateData(x.api_basic));
        SubscribeKcsapi("api_req_member/updatecomment", (x, y) => this.raw.api_comment = y.api_cmt);
    }
}
class Homeport {
    constructor() {
        this.Admiral = null;
        this.Materials = null;
        this.Ships = [];
    }
}
Homeport.Instance = new Homeport;
class Materials {
    get Fuel() { return this._Fuel; }
    get Ammo() { return this._Ammo; }
    get Steel() { return this._Steel; }
    get Bauxite() { return this._Bauxite; }
    get InstantConstruction() { return this._InstantConstruction; }
    get RepairBucket() { return this._RepairBucket; }
    get DevelopmentMaterial() { return this._DevelopmentMaterial; }
    get ImprovementMaterial() { return this._ImprovementMaterial; }
    constructor() {
        this._Fuel = 0;
        this._Ammo = 0;
        this._Steel = 0;
        this._Bauxite = 0;
        this._InstantConstruction = 0;
        this._RepairBucket = 0;
        this._DevelopmentMaterial = 0;
        this._ImprovementMaterial = 0;
        SubscribeKcsapi("api_port/port", x => this.Update(x.api_material));
        SubscribeKcsapi("api_get_member/material", x => this.Update(x));
        SubscribeKcsapi("api_req_hokyu/charge", x => this.Update(x.api_material));
        SubscribeKcsapi("api_req_kousyou/destroyship", x => this.Update(x.api_material));
        SubscribeKcsapi("api_req_kousyou/destroyitem2", x => this.Update(x.api_get_material));
        SubscribeKcsapi("api_req_air_corps/supply", x => this.Update([
            x.api_after_fuel,
            this.Ammo,
            this.Steel,
            x.api_after_bauxite
        ]));
        SubscribeKcsapi("api_req_air_corps/set_plane", (x, y) => {
            if (y["api_item_id"] === "-1")
                return;
            if (x["api_plane_info"].length >= 2)
                return;
            this.Update([
                this.Fuel,
                this.Ammo,
                this.Steel,
                x.api_after_bauxite
            ]);
        });
    }
    Update(source) {
        if (!source || source.length == 0)
            return;
        const type = typeof source[0].api_value;
        if (source.length >= 4 && type === "undefined") {
            const casted = source;
            this._Fuel = casted[0];
            this._Ammo = casted[1];
            this._Steel = casted[2];
            this._Bauxite = casted[3];
        }
        else if (source.length >= 8 && type !== "undefined") {
            const casted = source;
            this._Fuel = casted[0].api_value;
            this._Ammo = casted[1].api_value;
            this._Steel = casted[2].api_value;
            this._Bauxite = casted[3].api_value;
            this._InstantConstruction = casted[4].api_value;
            this._RepairBucket = casted[5].api_value;
            this._DevelopmentMaterial = casted[6].api_value;
            this._ImprovementMaterial = casted[7].api_value;
        }
    }
}
class RepairDock {
    constructor() {
        this.Docks = [];
        SubscribeKcsapi("api_get_member/ndock", x => this.Update(x));
        SubscribeKcsapi("api_req_nyukyo/start", (x, y) => this.Start(y));
        SubscribeKcsapi("api_req_nyukyo/speedchange", (x, y) => this.ChangeSpeed(y));
    }
    Update(source) {
        if (this.Docks.length === source.length)
            source.forEach(raw => this.Docks[raw.api_id].Update(raw));
        else {
            this.Docks.forEach(dock => dock.Dispose());
            this.Docks = source.map(x => new RepairDock.Dock(x));
        }
    }
    Start(request) {
        const ship = Homeport.Instance.Ships[request.api_ship_id];
        if (!ship)
            return;
        if (request.api_highspeed === 1)
            ship.Repair();
    }
    ChangeSpeed(request) {
        try {
            const dock = this.Docks[request.api_ndock_id];
            const ship = dock.Ship;
            dock.Finish();
            if (ship)
                ship.Repair();
        }
        catch (_a) { }
    }
}
(function (RepairDock) {
    let DockState;
    (function (DockState) {
        DockState[DockState["Locked"] = -1] = "Locked";
        DockState[DockState["Free"] = 0] = "Free";
        DockState[DockState["Repairing"] = 1] = "Repairing";
    })(DockState = RepairDock.DockState || (RepairDock.DockState = {}));
    class Dock extends TickNotifier {
        constructor(dock) {
            super();
            this.notified = false;
            this.Completed = null;
            this.Id = -1;
            this.State = DockState.Locked;
            this.ShipId = -1;
            this._Ship = null;
            this.CompleteTime = 0;
            this.Remaining = 0;
            this.Update(dock);
        }
        get Ship() { return this._Ship; }
        set Ship(ship) {
            if (this._Ship)
                this._Ship.Situation &= ~Ship.Situation.Repairing;
            if (ship)
                ship.Situation |= Ship.Situation.Repairing;
            this._Ship = ship;
        }
        Update(ndock) {
            this.Id = 0;
        }
        Finish() {
            this.State = RepairDock.DockState.Free;
            this.ShipId = -1;
            this.Ship = null;
            this.CompleteTime = 0;
        }
        Tick() {
            if (this.CompleteTime !== 0) {
                let remaining = this.CompleteTime - Date.now();
                if (remaining < 0)
                    remaining = 0;
                this.Remaining = remaining;
                if (!this.notified && this.Completed && remaining <= Settings.NotificationTime * 1000) {
                    fns(this.Completed, this, this.Id, this.Ship);
                    this.notified = true;
                }
            }
            else
                this.Remaining = 0;
        }
    }
    RepairDock.Dock = Dock;
})(RepairDock || (RepairDock = {}));
class Ship {
    constructor() {
        this.Situation = Ship.Situation.None;
    }
    Repair() {
    }
}
(function (Ship) {
    let Situation;
    (function (Situation) {
        Situation[Situation["None"] = 0] = "None";
        Situation[Situation["Repairing"] = 1] = "Repairing";
        Situation[Situation["Evacuation"] = 2] = "Evacuation";
        Situation[Situation["Tow"] = 4] = "Tow";
        Situation[Situation["HeavilyDamaged"] = 8] = "HeavilyDamaged";
        Situation[Situation["DamageControlled"] = 16] = "DamageControlled";
    })(Situation = Ship.Situation || (Ship.Situation = {}));
})(Ship || (Ship = {}));
//# sourceMappingURL=beerviewer.js.map