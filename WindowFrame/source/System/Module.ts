import Vue, { VueConstructor } from "vue";
import BaseAPI from "./Base/API";

declare global {
	interface Window {
		modules: Modules;
		CALLBACK: Callback;
		BaseAPI: BaseAPI;
	}
}

export type CallbackFunction = (...Parameter: any) => void;
export interface IModule {
	init(): void;
}
class ModuleInfo {
	public ModuleObject: IModule | null;
	public Name: string;
	public Template: string;
	public Script: string | null;
	public Style: string | null;

	constructor(ModuleObject: IModule | null, Name: string, Template: string, Script: string | null, Style: string | null) {
		this.ModuleObject = ModuleObject;
		this.Name = Name;
		this.Template = Template;
		this.Script = Script;
		this.Style = Style;
	}
}
export class Module {
	public Area: string;
	public Id: string;
	public Name: string;
	public Icon: string;
	public Displaying: boolean;
	public ComponentName: string;

	constructor(Area: string, Id: string, Name: string, Icon: string, Displaying: boolean, ComponentName: string) {
		this.Area = Area;
		this.Id = Id;
		this.Name = Name;
		this.Icon = Icon;
		this.Displaying = Displaying;
		this.ComponentName = ComponentName;
	}
}
class ModuleArea {
	public Name: string;
	public Type: string;
	public Modules: Array<Module>;
	public Element: HTMLElement | null;

	constructor(Name: string, Type: string, Modules: Array<Module>, TargetElement: HTMLElement | null) {
		this.Name = Name;
		this.Type = Type;
		this.Modules = Modules;
		this.Element = TargetElement;
	}
}
class MenuTool {
	public Id: string;
	public Name: string;
	public Action: Function;

	constructor(Id: string, Name: string, Action: Function) {
		this.Id = Id;
		this.Name = Name;
		this.Action = Action;
	}
}
class ModuleAreas {
	private _Areas: { [Area: string]: ModuleArea } = {};
	private _Tools: { [id: string]: MenuTool } = {};

	public get Areas(): { [Area: string]: ModuleArea } {
		return this._Areas;
	}
	public get Tools(): { [id: string]: MenuTool } {
		return this._Tools;
	}

	private ValidIcon(icon: string): boolean {
		return ["", "game", "plugin", "devtool", "setting"].indexOf(icon) >= 0;
	}

	public init(): void {
		["Top", "Main", "Side", "Sub"].forEach(name => {
			const id = name.toLowerCase();
			this._Areas[id] = new ModuleArea(name, id, [], document.querySelector(`#${id}-module-area`));
		});

		// Main browser
		this.register("main", "game", "Game", "game", "game-component");

		// Devtools
		this._Tools["devtools"] = new MenuTool(
			"devtools",
			"DevTools",
			function () { window.API.DevTools() }
		);
	}

	public register(area: string, id: string, name: string, icon: string, componentName: string): void {
		if (!(area in this._Areas)) throw `Area '${area}' not supported`;
		const areaElem = this._Areas[area];

		if (areaElem.Modules.some(x => x.Id === id)) throw "Already registered name";

		areaElem.Modules.push(new Module(
			area,
			id,
			name,
			this.ValidIcon(icon) ? icon : "unknown",
			areaElem.Modules.length === 0 || area === "top",
			componentName
		));
	}
}

class Modules {
	public static Instance: Modules = new Modules();

	private _initialized: boolean;
	private list: { [name: string]: ModuleInfo };
	public areas: ModuleAreas;

	constructor() {
		this._initialized = false;
		this.list = {};
		this.areas = new ModuleAreas();
	}

	public initialized(): boolean {
		return this._initialized;
	}
	public load(name: string, template: string, script: boolean, css: boolean, base: string = "modules"): void {
		if (name in this.list) throw "Tried to load module already loaded";

		this.list[name] = new ModuleInfo(
			null, name, template,
			script ? `${base}/${name}/${name}.js` : null,
			css ? `${base}/${name}/${name}.css` : null
		);
		const module = this.list[name];
		const el_module = document.createElement("div");
		el_module.setAttribute("data-module", module.Name);

		const el_template = document.createElement("div");
		el_module.append(el_template);
		el_template.outerHTML = module.Template;

		if (module.Script) {
			const script = document.createElement("script");
			script.type = "text/javascript";
			script.src = module.Script;
			el_module.append(script);
		}
		if (module.Style) {
			const link = document.createElement("link");
			link.rel = "stylesheet";
			link.type = "text/css";
			link.href = module.Style;
			el_module.append(link);
		}

		const modules = document.querySelector("#modules");
		if (modules) modules.append(el_module);
	}
	public register(name: string, module: IModule): IModule {
		if (!(name in this.list)) throw `Tried to register '${name}' module, but not loaded`;
		this.list[name].ModuleObject = module;

		const obj = this.list[name].ModuleObject;
		if (this._initialized && obj) obj.init();
		return module;
	}
	public init(): void {
		for (let i in this.list) {
			const obj = this.list[i].ModuleObject;
			if (obj) obj.init();
		}
		this._initialized = true;
	}
	public get(name: string): IModule | null {
		if (!(name in this.list)) return null;
		if (this.list[name].ModuleObject === null) return null;
		return this.list[name].ModuleObject;
	}
}
export class Callback {
	public static Instance: Callback = new Callback();

	private callbacks: { [name: string]: Array<Function> } = {};

	public register(name: string, callback: CallbackFunction): void {
		if (!(name in this.callbacks))
			this.callbacks[name] = [];

		this.callbacks[name].push(callback);
	}
	public unregister(name: string, callback: CallbackFunction): void {
		if (!(name in this.callbacks)) return;

		const index = this.callbacks[name].indexOf(callback);
		if (index >= 0)
			this.callbacks[name].splice(index, 1);
	}
	public call(name: string, ...params: any[]): boolean {
		if (!(name in this.callbacks)) return false;

		const list = this.callbacks[name];
		list.forEach(x => x.apply(null, params));
		return true;
	}
}

export default Modules;