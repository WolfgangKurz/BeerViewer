"use strict";
!function () {
	window.OpenMenu = function (open) {
		const target = $("#top-menubutton");
		const openClass = "menu-open";

		if (open === undefined) {
			if (target.hasClass(openClass))
				window.OpenMenu(false);
			else
				window.OpenMenu(true);
		} else if (open)
			target.addClass(openClass);
		else
			target.removeClass(openClass);

		return true;
	};

	window.modules.register("window", {
		init: function () {
			window.CALLBACK.register("WindowState", function (_state) {
				const state = parseInt(_state);
				$("#top-systembutton").attr("data-windowstate", state);
			});
			window.CALLBACK.register("FocusState", function (focused) {
				const mainWindow = $("html");
				if (focused)
					mainWindow.addClass("focused");
				else
					mainWindow.removeClass("focused");
			});

			$.all("#top-systembutton > .system-button").event("click", function (e) {
				window.API.SystemCall(this.attr("data-role"));
			});

			$("#top-menubutton > button").event("click", e => OpenMenu());
			$("#top-menu-overlay").event("click", e => $("#top-menubutton > button").trigger("click"));

			!function () {
				const getProgressColor = (progress, strips) => Math.min(strips - 1, Math.floor(progress / (100 / strips)));
				const rebindProgress = function (target) {
					if (!target.is('[data-type="progress"]')) return;
					if (target.is("[data-progress-binded]")) return;
					target.attr("data-progress-binded", "1");

					target.findAll(".progress-strip").each(function () {
						this.remove();
					});

					let strips = target.attr("data-progress-strip");
					if (!strips)
						strips = 1;
					else
						strips = parseInt(strips);

					for (let i = 1; i < strips; i++) {
						target.prepend(
							$.new("div", "progress-strip")
								.css("left", `${100 * i / strips}%`)
						);
					}

					target.prepend($.new("div", "progress-bar"));

					updateProgress(target);
				};
				const updateProgress = function (target) {
					if (!target.is('[data-type="progress"]')) return;
					if (!target.is("[data-progress-binded]")) return;

					const progress = parseFloat(target.attr("data-progress"));
					const strip = parseInt(target.attr("data-progress-strip"));

					target.find(".progress-bar")
						.attr("data-color", getProgressColor(progress, strip))
						.css("width", progress + "%");
				};

				var observer = new MutationObserver(function (m) {
					m.forEach(function (x) {
						if (x.type === "childList") {
							x.target.findAll('[data-type="progress"]')
								.each(function () {
									rebindProgress(this);
								});
						} else if (x.type === "attributes") {
							if (x.target.is('[data-type="progress"][data-progress-binded]')) {
								switch (x.attributeName) {
									case "data-progress":
										updateProgress(x.target);
										break;
									case "data-progress-strip":
										rebindProgress(x.target);
										break;
								}
							}
						}
					});
				});
				observer.observe(
					document.body,
					{ attributes: true, childList: true, subtree: true }
				);

				$.all('[data-type="progress"]').each(function () {
					rebindProgress(this);
				});
			}();

			$("body").append(
				$.new("div").prop("id", "cursor-overlay")
					.append($.new("div", "cursor-nwse ht-topleft"))
					.append($.new("div", "cursor-nwse ht-bottomright"))
					.append($.new("div", "cursor-nesw ht-topright"))
					.append($.new("div", "cursor-nesw ht-bottomleft"))
					.append($.new("div", "cursor-ns ht-top"))
					.append($.new("div", "cursor-ns ht-bottom"))
					.append($.new("div", "cursor-we ht-left"))
					.append($.new("div", "cursor-we ht-right"))
			);
		}
	});
}();