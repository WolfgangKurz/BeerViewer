export type AjaxEventAfter = { (): void };
export type AjaxEventCallback = { (response: string): void };
export type AjaxEventProgress = { (current: number, total: number, http: XMLHttpRequest): void };
export type AjaxEventAbort = { (event: Event, http: XMLHttpRequest): void };
export type AjaxEventError = { (event: ErrorEvent, http: XMLHttpRequest): void };

export interface AjaxConfig {
	method?: string;
	url?: string;
	async?: boolean;
	mime?: string;
	after?: AjaxEventAfter;
	progress?: AjaxEventProgress;
	error?: AjaxEventError;
	aborted?: AjaxEventAbort;
	callback?: AjaxEventCallback;
	post?: FormData | string | null;
}

declare global {
	export interface String {
		int(): number;
		float(): number;
		format(...params: any[]): string;
	}
	export interface StringConstructor {
		format(...params: any[]): string;
	}
	export interface Number {
		format(): string;
	}

	export interface Document {
		event(x: string, y: EventListenerOrEventListenerObject, z?: boolean | AddEventListenerOptions): Document;
		trigger(x: string): Document;
	}
	export interface HTMLCollection {
		addClass(x: string): HTMLCollection;
		css(x: string, y: string | null): HTMLCollection;
		each(x: { (this: Element, item: any): boolean }): void;
		event(x: string, y: EventListenerOrEventListenerObject, z?: boolean | AddEventListenerOptions): HTMLCollection;
		hasClass(x: string): boolean;
		removeClass(x: string): HTMLCollection;
		trigger(x: string): HTMLCollection;
	}
	export interface HTMLDocument {
		event(x: string, y: EventListenerOrEventListenerObject, z?: boolean | AddEventListenerOptions): HTMLDocument;
		trigger(x: string): HTMLDocument;
	}
	export interface Element {
		addClass(x: string): Element;
		after(x: Element | Text | string, y: Node | null): Element;
		append(x: Element | Text | string): Element;
		attr(x: string, y: any): Element | string | null;
		before(x: Element | Text | string, y: Node | null): Element;
		child(selector: string): Element;
		childs(selector: string): NodeListOf<Element>;
		clone(): Node;
		event(this: Element, x: string, y: EventListenerOrEventListenerObject, z?: boolean | AddEventListenerOptions): Element;
		find(selector: string): Element | null;
		findAll(selector: string): NodeListOf<Element>;
		hasClass(x: string): boolean;
		html(x?: string): string | Element;
		next(): Element | null;
		outerhtml(x?: string): string | Element;
		parent(x: string): (Node & ParentNode) | null;
		prepend(x: Element): Element;
		prev(): Element | null;
		prop(x: string, y: any): Element | any;
		removeClass(x: string): Element;
		trigger(x: string): Element;
		val(x: string): any;
		is(x: string): boolean;
		css(x: string, y: string | null): Element | string;
	}
	export interface NodeList {
		addClass(x: string): NodeList;
		css(x: string, y: string | null): NodeList;
		each(x: { (this: Node, item: any): boolean }): void;
		event(x: string, y: EventListenerOrEventListenerObject, z?: boolean | AddEventListenerOptions): NodeList;
		hasClass(x: string): boolean;
		removeClass(x: string): NodeList;
		trigger(x: string): NodeList;
	}
	export interface Node {
		prev(): Node | null;
		next(): Node | null;
	}

	export interface Window {
		$: I$;
		HTMLDocument: HTMLDocument;
		Document: Document;
	}

	export interface EventListener {
		(this: Node, event: Event): void;
	}
	export interface EventListenerObject {
		handleEvent(this: Node, evt: Event): void;
	}

	export interface I$ {
		(x: string): Element | null;
		all: (x: string) => NodeListOf<Element>;
		new: I$new;
	}
	export interface I$new {
		(x: string, y: string): Element;
		text: (x: string) => Text;
	}
}

window.$ = (function () {
	const obj = <I$>(x: string): Element | null => document.querySelector(x);
	obj.all = (x: string): NodeListOf<Element> => document.querySelectorAll(x);

	const obj_new = <I$new>(x: string, y: string): Element => {
		const el = document.createElement(x);
		if (typeof y !== "undefined") el.className = y;
		return el;
	};
	obj_new.text = (x: string): Text => document.createTextNode(x);
	obj.new = obj_new;

	obj.ajax = (opt: AjaxConfig): void => {
		const h: any = new XMLHttpRequest();
		const o: AjaxConfig = Object.assign({
			method: "GET",
			url: "",
			async: true,
			mime: "text/html",
			after: function () { },
			progress: function () { },
			error: function () { },
			aborted: function () { },
			callback: function () { },
			post: null
		}, opt);
		h.config = o;

		const eventLoad = () => {
			if (h.readyState === 4) {
				h.config.callback(h.responseText, h);
				h.config.after(h);
			}
		};
		const eventAbort = (e: Event) => {
			h.config.aborted(e, h);
			h.config.after(h);
		};
		const eventError = (e: ErrorEvent) => {
			h.config.error(e, h);
			h.config.after(h);
		};
		const eventProgress = (e: ProgressEvent) => {
			if (!e.lengthComputable) h.config.progress(1, 2, h);
			else h.config.progress(e.loaded, e.total, h);
		};

		if (h.upload) h.upload.addEventListener("progress", eventProgress);
		else h.addEventListener("progress", eventProgress);
		h.addEventListener("loadend", eventLoad);
		h.addEventListener("error", eventError);
		h.addEventListener("abort", eventAbort);

		h.open(o.method, o.url, o.async);
		if (o.method && o.method.toUpperCase() === "POST" && !(o.post && o.post instanceof FormData))
			h.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

		h.overrideMimeType(o.mime);
		h.send(o.post);
		return h;
	}
	return obj;
})();
var $ = window.$;

Element.prototype.find
	= function (selector: string): Element | null {
		return this.querySelector(selector);
	};
Element.prototype.findAll
	= function (selector: string): NodeListOf<Element> {
		return this.querySelectorAll(selector);
	};

Element.prototype.child
	= function (selector: string): Element {
		const y = "_TMP$" + Math.random().toFixed(12).substr(2);
		const z = this.id;
		let u = null;

		this.id = y;
		u = $("#" + y.replace("$", "\\$") + ">" + selector);
		if (z.length === 0)
			this.removeAttribute("id");
		else
			this.id = z;
		return u as Element;
	};
Element.prototype.childs
	= function (selector: string): NodeListOf<Element> {
		const y = "_TMP$" + Math.random().toFixed(12).substr(2);
		const z = this.id;
		let u = null;

		this.id = y, u = $.all("#" + y.replace("$", "\\$") + ">" + selector);
		if (z.length === 0)
			this.removeAttribute("id");
		else
			this.id = z;
		return u;
	};
Element.prototype.prev
	= function (): Element | null {
		return this.previousElementSibling;
	};
Element.prototype.next
	= function (): Element | null {
		return this.nextElementSibling;
	};

Node.prototype.prev
	= function (): Node | null {
		return this.previousSibling;
	}
Node.prototype.next
	= function (): Node | null {
		return this.nextSibling;
	}

Element.prototype.prop
	= function (x: string, y: any): Element | any {
		if (typeof y === "undefined")
			return (<any>this)[x];
		(<any>this)[x] = y;
		return this;
	};
Element.prototype.attr
	= function (x: string, y: any): Element | string | null {
		if (typeof y === "undefined") return this.getAttribute(x);
		this.setAttribute(x, y);
		return this;
	};
HTMLElement.prototype.css
	= function (x: string, y: string | null): Element | string {
		if (typeof y === "undefined") {
			const style = window.getComputedStyle ? window.getComputedStyle(this as Element) : this.style;
			return style.getPropertyValue(x);
		}
		this.style.setProperty(x, y);
		return this;
	};

NodeList.prototype.each
	= function (x: { (this: Node, item: any): boolean }): void {
		for (let i = 0; i < this.length; i++)
			if (x.apply(this[i], [i]) === false) break;
	};
HTMLCollection.prototype.each
	= function (x: { (this: Node, item: any): boolean }): void {
		for (let i = 0; i < this.length; i++)
			if (x.apply(this[i], [i]) === false) break;
	};

NodeList.prototype.css
	= function (x: string, y: string | null): NodeList {
		this.each(function () {
			if (this instanceof HTMLElement) this.css(x, y);
			return true;
		});
		return this;
	};
HTMLCollection.prototype.css
	= function (x: string, y: string | null): HTMLCollection {
		this.each(function () {
			if (this instanceof HTMLElement) this.css(x, y);
			return true;
		});
		return this;
	};

Element.prototype.html
	= function (x?: string): string | Element {
		if (typeof x === "undefined") return this.innerHTML;
		this.innerHTML = x;
		return this;
	};
Element.prototype.outerhtml
	= function (x?: string): string | Element {
		if (typeof x === "undefined") return this.outerHTML;
		this.outerHTML = x;
		return this;
	};

Element.prototype.prepend
	= function (x: Element | Text | string): Element {
		if (x instanceof Element || x instanceof Element || x instanceof Text)
			this.insertBefore(x, this.firstChild);
		else
			this.insertBefore($.new.text(x), this.firstChild);
		return this;
	};
Element.prototype.append
	= function (x: Element | Text | string): Element {
		if (x instanceof Element || x instanceof Element || x instanceof Text)
			this.appendChild(x);
		else
			this.appendChild($.new.text(x));
		return this;
	};

Element.prototype.before
	= function (x: Element | Text | string, y: Node | null): Element {
		if (x instanceof Element || x instanceof Text)
			this.insertBefore(x, y);
		else
			this.insertBefore($.new.text(x), y);
		return this;
	};
Element.prototype.after
	= function (x: Element | Text | string, y: Node | null): Element {
		if (!y) throw "y cannot be null";
		if (x instanceof Element || x instanceof Text)
			this.insertBefore(x, y.next());
		else
			this.insertBefore($.new.text(x), y.next());
		return this;
	};

Element.prototype.val
	= function (x: string): any {
		if (typeof x === "undefined") return (<any>this).value;
		(<any>this).value = x;
		return this;
	};
Element.prototype.clone
	= function (): Node {
		return this.cloneNode(true);
	};

if (window.HTMLDocument) HTMLDocument.prototype.event
	= function (x: string, y: EventListenerOrEventListenerObject, z?: boolean | AddEventListenerOptions): HTMLDocument {
		const e = x.split(" ");
		for (let i = 0; i < e.length; i++)
			this.addEventListener(e[i], y, z);
		return this;
	};
else if (window.Document) Document.prototype.event
	= function (x: string, y: EventListenerOrEventListenerObject, z?: boolean | AddEventListenerOptions): Document {
		const e = x.split(" ");
		for (let i = 0; i < e.length; i++)
			this.addEventListener(e[i], y, z);
		return this;
	};

Element.prototype.event
	= function (x: string, y: EventListenerOrEventListenerObject, z?: boolean | AddEventListenerOptions): Element {
		const e = x.split(" ");
		for (let i = 0; i < e.length; i++)
			this.addEventListener(e[i], y, z);

		return this;
	};
NodeList.prototype.event
	= function (x: string, y: EventListenerOrEventListenerObject, z?: boolean | AddEventListenerOptions): NodeList {
		this.each(function () {
			if (this instanceof Element) this.event(x, y, z);
			return true;
		});
		return this;
	};
HTMLCollection.prototype.event
	= function (x: string, y: EventListenerOrEventListenerObject, z?: boolean | AddEventListenerOptions): HTMLCollection {
		this.each(function () {
			if (this instanceof Element) this.event(x, y, z);
			return true;
		});
		return this;
	};

if (window.HTMLDocument) HTMLDocument.prototype.trigger
	= function (x: string): HTMLDocument {
		const d = x.split(" ");
		for (let i = 0; i < d.length; i++) {
			const e = document.createEvent("HTMLEvents");
			e.initEvent(d[i], false, true);
			this.dispatchEvent(e);
		}
		return this;
	};
else if (window.Document) Document.prototype.trigger
	= function (x: string): Document {
		const d = x.split(" ");
		for (let i = 0; i < d.length; i++) {
			const e = document.createEvent("HTMLEvents");
			e.initEvent(d[i], false, true);
			this.dispatchEvent(e);
		}
		return this;
	};
Element.prototype.trigger
	= function (x: string): Element {
		const d = x.split(" ");
		for (let i = 0; i < d.length; i++) {
			const e = document.createEvent("HTMLEvents");
			e.initEvent(d[i], false, true), this.dispatchEvent(e);
		}
		return this;
	};
NodeList.prototype.trigger
	= function (x: string): NodeList {
		this.each(function () {
			if (this instanceof Element) this.trigger(x);
			return true;
		});
		return this;
	};
HTMLCollection.prototype.trigger
	= function (x: string): HTMLCollection {
		this.each(function () {
			if (this instanceof Element)
				this.trigger(x);
			return true;
		});
		return this;
	};

Element.prototype.is
	= function (x: string): boolean { return this.matches(x) };

Element.prototype.parent
	= function (x: string): (Node & ParentNode) | null {
		if (typeof x === "undefined") return this.parentNode;
		let y: Element | null = this as Element;
		while (
			y !== null
			&& y.tagName
			&& y.tagName.toLowerCase() !== "body"
			&& !y.is(x)
		) y = y.parentNode as (Element | null);
		return y;
	};

Element.prototype.addClass
	= function (x: string): Element {
		const y = x.split(" "), z = this.className.split(" ");

		for (let i = 0; i < y.length; i++) {
			if (y[i].trim().length === 0) continue;
			if (z.indexOf(y[i]) >= 0) continue;
			z.push(y[i]);
		}
		this.className = z.filter(function (_) {
			return _.length > 0;
		}).join(" ");
		return this;
	};
Element.prototype.removeClass
	= function (x: string): Element {
		const y = x.split(" "), z = this.className.split(" ");

		for (let i = 0, j; i < y.length; i++) {
			if (y[i].trim().length === 0) continue;
			while ((j = z.indexOf(y[i])) >= 0) z.splice(j, 1);
		}
		this.className = z.filter(function (_) {
			return _.length > 0;
		}).join(" ");
		return this;
	};
Element.prototype.hasClass
	= function (x: string): boolean {
		const y = this.className.split(" ");
		for (let i = 0, j; i < y.length; i++) {
			if (y[i].trim().length === 0) continue;
			if (y[i] === x) return true;
		}
		return false;
	};
NodeList.prototype.addClass
	= function (x: string): NodeList {
		this.each(function () {
			if (this instanceof Element) this.addClass(x);
			return true;
		});
		return this;
	};
NodeList.prototype.removeClass
	= function (x: string): NodeList {
		this.each(function () {
			if (this instanceof Element) this.removeClass(x);
			return true;
		});
		return this;
	};
NodeList.prototype.hasClass
	= function (x: string): boolean {
		let result = false;
		this.each(function () {
			if (this instanceof Element && this.hasClass(x)) {
				result = true;
				return false; // stop loop
			}
			return true;
		});
		return result;
	};
HTMLCollection.prototype.addClass
	= function (x: string): HTMLCollection {
		this.each(function () {
			if (this instanceof Element) this.addClass(x);
			return true;
		});
		return this;
	};
HTMLCollection.prototype.removeClass
	= function (x: string): HTMLCollection {
		this.each(function () {
			if (this instanceof Element) this.removeClass(x);
			return true;
		});
		return this;
	};
HTMLCollection.prototype.hasClass
	= function (x: string): boolean {
		let result = false;
		this.each(function () {
			if (this instanceof Element && this.hasClass(x)) {
				result = true;
				return false; // stop loop
			}
			return true;
		});
		return result;
	};

String.prototype.int
	= function (): number { return parseInt(this as string) };

String.prototype.float
	= function (): number { return parseFloat(this as string) };

String.prototype.format
	= function (...params: any[]): string {
		let template = this.toString();
		for (let i = 0; i < params.length; i++) {
			const reg = new RegExp(`\\{${i}\\}`, "g");
			template = template.replace(reg, params[i]);
		}
		return template;
	};

String.format = (template: string, ...params: any[]) => template.format(...params);

Number.prototype.format
	= function (): string { return this.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") };

export default $;