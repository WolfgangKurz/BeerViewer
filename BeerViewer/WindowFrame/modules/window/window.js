"use strict";
!function () {
	(async function () {
		await CefSharp.BindObjectAsync("API", "bound");
	})();

	window.modules.register("window", {
		init: function () {
			$("#top-area").event("mousedown", function (e) {
				e.preventDefault();
				window.API.requestWindowMove();
				return false;
			});
		}
	});
}();