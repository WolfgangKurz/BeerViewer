"use strict";
!function () {
	if (!window.modules) throw "Cannot find `module`";

	window.modules.register("logger", {
		logger: null,

		log: function (text) {
			const logger = this.logger;
			if (logger === null) return;

			logger.prepend(
				$.new("div", "logger-log").append($.new.text(text))
			);
		},

		init: function () {
			const _this = this;
			const logger = $.new("div").prop("id", "logger-container");
			_this.logger = logger;

			window.CALLBACK.register("Logged", function (text) {
				_this.log(text);
			});
			_this.log("Logger module has been loaded");

			window.modules.areas.register("sub", "logger", "Log", "", logger);
		}
	});
}();