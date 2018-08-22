"use strict";
! function () {
	// Polyfill
	if (!Element.prototype.matches) {
		Element.prototype.matches =
			Element.prototype.matchesSelector ||
			Element.prototype.mozMatchesSelector ||
			Element.prototype.msMatchesSelector ||
			Element.prototype.oMatchesSelector ||
			Element.prototype.webkitMatchesSelector ||
			function (s) {
				const matches = (this.document || this.ownerDocument).querySelectorAll(s),
					i = matches.length;
				while (--i >= 0 && matches.item(i) !== this);
				return i > -1;
			};
	}

	// Defines
	const $ = function (x) {
		return document.querySelector(x);
	};
	$.all = function (x) {
		return document.querySelectorAll(x);
	};

	$.new = function (x, y) {
		x = document.createElement(x);
		if (typeof y !== "undefined") x.className = y;
		return x;
	};
	$.new.text = function (x, y) {
		return document.createTextNode(x);
	};

	HTMLElement.prototype.find = function (x) {
		return this.querySelector(x);
	};
	HTMLElement.prototype.findAll = function (x) {
		return this.querySelectorAll(x);
	};
	HTMLElement.prototype.child = function (x) {
		const y = "_TMP$" + Math.random().toFixed(12).substr(2);
		const z = this.id;
		let u = null;

		this.id = y;
		u = $("#" + y.replace("$", "\\$") + ">" + x);
		if (z.length === 0)
			this.removeAttribute("id");
		else
			this.id = z;
		return u;
	};
	HTMLElement.prototype.childs = function (x) {
		const y = "_TMP$" + Math.random().toFixed(12).substr(2);
		const z = this.id;
		let u = null;

		this.id = y, u = $.all("#" + y.replace("$", "\\$") + ">" + x);
		if (z.length === 0)
			this.removeAttribute("id");
		else
			this.id = z;
		return u;
	};
	HTMLElement.prototype.prev = function () {
		return this.previousElementSibling;
	};
	HTMLElement.prototype.next = function () {
		return this.nextElementSibling;
	};

	HTMLElement.prototype.prop = function (x, y) {
		if (typeof y === "undefined") return this[x];
		this[x] = y;
		return this;
	};
	HTMLElement.prototype.attr = function (x, y) {
		if (typeof y === "undefined") return this.getAttribute(x);
		this.setAttribute(x, y);
		return this;
	};
	HTMLElement.prototype.css = function (x, y) {
		if (typeof y === "undefined") {
			const style = window.getComputedStyle ? window.getComputedStyle(this) : this.style;
			return style[x];
		}
		this.style[x] = y;
		return this;
	};
	NodeList.prototype.css = function (x, y) {
		this.each(function () {
			this.css(x, y);
		});
		return this;
	};
	HTMLCollection.prototype.css = function (x, y) {
		this.each(function () {
			this.css(x, y);
		});
		return this;
	};

	HTMLElement.prototype.html = function (x) {
		if (typeof x === "undefined") return this.innerHTML;
		this.innerHTML = x;
		return this;
	};
	HTMLElement.prototype.outerhtml = function (x) {
		if (typeof x === "undefined") return this.outerHTML;
		this.outerHTML = x;
		return this;
	};

	HTMLElement.prototype.prepend = function (x) {
		if (x instanceof HTMLElement || x instanceof Text)
			this.insertBefore(x, this.firstChild);
		else
			this.insertBefore($.new.text(x), this.firstChild);
		return this;
	};
	HTMLElement.prototype.append = function (x) {
		if (x instanceof HTMLElement || x instanceof Text)
			this.appendChild(x);
		else
			this.appendChild($.new.text(x));
		return this;
	};

	HTMLElement.prototype.before = function (x, y) {
		if (x instanceof HTMLElement || x instanceof Text)
			this.insertBefore(x, y);
		else
			this.insertBefore($.new.text(x), y);
		return this;
	};
	HTMLElement.prototype.after = function (x, y) {
		if (x instanceof HTMLElement || x instanceof Text)
			this.insertBefore(x, y.next());
		else
			this.insertBefore($.new.text(x), y.next());
		return this;
	};

	HTMLElement.prototype.val = function (x) {
		if (typeof x === "undefined") return this.value;
		this.value = x;
		return this;
	};
	HTMLElement.prototype.clone = function () {
		return this.cloneNode(true);
	};

	NodeList.prototype.each = function (x) {
		for (let i = 0; i < this.length; i++)
			if (x.apply(this[i], [i]) === false) break;
	};
	HTMLCollection.prototype.each = function (x) {
		for (let i = 0; i < this.length; i++)
			if (x.apply(this[i], [i]) === false) break;
	};

	if (window.HTMLDocument) HTMLDocument.prototype.event = function (x, y, z) {
		const e = x.split(" ");
		for (let i = 0; i < e.length; i++) this.addEventListener(e[i], y, z);
		return this;
	};
	else if (window.Document) Document.prototype.event = function (x, y, z) {
		const e = x.split(" ");
		for (let i = 0; i < e.length; i++) this.addEventListener(e[i], y, z);
		return this;
	};
	HTMLElement.prototype.event = function (x, y, z) {
		const e = x.split(" ");
		for (let i = 0; i < e.length; i++) this.addEventListener(e[i], y, z);
		return this;
	};
	NodeList.prototype.event = function (x, y, z) {
		this.each(function () {
			this.event(x, y, z);
		});
		return this;
	};
	HTMLCollection.prototype.event = function (x, y, z) {
		this.each(function () {
			this.event(x, y, z);
		});
		return this;
	};

	if (window.HTMLDocument)
		HTMLDocument.prototype.trigger = function (x) {
			const d = x.split(" ");
			for (let i = 0; i < d.length; i++) {
				if (document.createEventObject) this.fireEvent("on" + d[i]);
				else {
					const e = document.createEvent("HTMLEvents");
					e.initEvent(d[i], false, true);
					this.dispatchEvent(e);
				}
			}
			return this;
		};
	else if (window.Document)
		Document.prototype.trigger = function (x) {
			const d = x.split(" ");
			for (let i = 0; i < d.length; i++) {
				if (document.createEventObject) this.fireEvent("on" + d[i]);
				else {
					const e = document.createEvent("HTMLEvents");
					e.initEvent(d[i], false, true);
					this.dispatchEvent(e);
				}
			}
			return this;
		};
	HTMLElement.prototype.trigger = function (x) {
		const d = x.split(" ");
		for (let i = 0; i < d.length; i++) {
			if (document.createEventObject) this.fireEvent("on" + d[i]);
			else {
				const e = document.createEvent("HTMLEvents");
				e.initEvent(d[i], false, true), this.dispatchEvent(e);
			}
		}
		return this;
	};
	NodeList.prototype.trigger = function (x) {
		this.each(function () {
			this.trigger(x);
		});
		return this;
	};
	HTMLCollection.prototype.trigger = function (x) {
		this.each(function () {
			this.trigger(x);
		});
		return this;
	};

	HTMLElement.prototype.is = function (x) {
		return this.matches(x);
	};
	HTMLElement.prototype.parent = function (x) {
		if (typeof x === "undefined") return this.parentNode;
		let y = this;
		while (y !== null && y.tagName.toLowerCase() !== "body" && !y.is(x)) y = y.parentNode;
		return y;
	};

	HTMLElement.prototype.addClass = function (x) {
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
	HTMLElement.prototype.removeClass = function (x) {
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
	HTMLElement.prototype.hasClass = function (x) {
		const y = this.className.split(" ");
		for (let i = 0, j; i < y.length; i++) {
			if (y[i].trim().length === 0) continue;
			if (y[i] === x) return true;
		}
		return false;
	};
	NodeList.prototype.addClass = function (x) {
		this.each(function () {
			this.addClass(x);
		});
		return this;
	};
	NodeList.prototype.removeClass = function (x) {
		this.each(function () {
			this.removeClass(x);
		});
		return this;
	};
	NodeList.prototype.hasClass = function (x) {
		return this[0].hasClass(x);
	};
	HTMLCollection.prototype.addClass = function (x) {
		this.each(function () {
			this.addClass(x);
		});
		return this;
	};
	HTMLCollection.prototype.removeClass = function (x) {
		this.each(function () {
			this.removeClass(x);
		});
		return this;
	};
	HTMLCollection.prototype.hasClass = function (x) {
		return this[0].hasClass(x);
	};

	String.prototype.int = function () {
		return parseInt(this);
	};
	String.prototype.float = function () {
		return parseFloat(this);
	};
	String.format = function (template) {
		for (let i = 1; i < arguments.length; i++) {
			const reg = new RegExp("\\{" + (i - 1) + "\\}", "g");
			template = template.replace(reg, arguments[i]);
		}
		return template;
	};

	Number.prototype.format = function () {
		return this.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
	};

	$.ajax = function (opt) {
		const h = new XMLHttpRequest();

		const eventLoad = function () {
			if (h.readyState === 4) {
				h.config.callback(h.responseText, h);
				h.config.after(h);
			}
		};
		const eventAbort = function (e) {
			h.config.aborted(e, h);
			h.config.after(h);
		};
		const eventError = function (e) {
			h.config.error(e, h);
			h.config.after(h);
		};
		const eventProgress = function (e) {
			if (!e.lengthComputable) h.config.progress(1, 2, h);
			else h.config.progress(e.loaded || e.position, e.total, h);
		};

		const o = {
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
		};
		for (let k in opt) o[k] = opt[k];

		h.config = o;

		if (h.upload) h.upload.addEventListener("progress", eventProgress);
		else h.addEventListener("progress", eventProgress);

		h.addEventListener("loadend", eventLoad);
		h.addEventListener("error", eventError);
		h.addEventListener("abort", eventAbort);

		h.open(o.method, o.url, o.async);
		if (o.method.toUpperCase() === "POST" && !(o.post instanceof FormData))
			h.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");

		h.overrideMimeType(o.mime);
		h.send(o.post);
		return h;
	};

	window["$"] = $;
	window["$common"] = $;
}();