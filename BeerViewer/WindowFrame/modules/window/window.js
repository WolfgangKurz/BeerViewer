"use strict";
!function () {
	window.modules.register("window", {
		init: function () {
			window.CALLBACK.register("WindowState", function (_state) {
				const state = parseInt(_state);
				$("#top-systembutton").attr("data-windowstate", state);
			});

			$.all("#top-systembutton > .system-button").event("click", function () {
				window.API.SystemCall(this.attr("data-role"));
			});

			$("#top-menubutton > button").event("click", function (e) {
				const target = this.parent();
				if (target.hasClass("menu-open"))
					this.parent().removeClass("menu-open");
				else
					this.parent().addClass("menu-open");
			});
			$("#top-menu-overlay").event("click", function (e) {
				$("#top-menubutton > button").trigger("click");
			});

			!function () {
				const getProgressColor = function (progress) {
					if (progress >= 75)
						return "#388e3c";
					else if (progress >= 50)
						return "#fdd835";
					else if (progress >= 25)
						return "#f57c00";
					else
						return "#c62828";
				};
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
								.css("left", (100 * i / strips) + "%")
						);
					}

					target.prepend($.new("div", "progress-bar"));

					updateProgress(target);
				};
				const updateProgress = function (target) {
					if (!target.is('[data-type="progress"]')) return;
					if (!target.is("[data-progress-binded]")) return;

					const progress = parseFloat(target.attr("data-progress"));

					target.find(".progress-bar")
						.css("background-color", getProgressColor(progress))
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