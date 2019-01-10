"use strict";
!function () {
	// x: Response as Json object, y: Request parameters as Json object
	window.SubscribeKcsapi = function (url, callback) {
		window.API.SubscribeHTTP("/kcsapi/"+url, (x, y) => {
			try {
				const svdata = x.substr(0, 7) === "svdata=" ? x.substr(7) : x;
				const json = JSON.parse(svdata);
				json.api_result === 1 && callback && callback(json.api_data, y);
			} catch (e) {
				console.warn("Expected json, but not.", e);
			}
		});
	};

	window.Class = {
		RepairDock: (function () {
			const _class = function (data) {
				const RawData = {
					Id: 0,
					Level: 0,
					State: -1,
					ShipId: 0,
					Ship: null,
					CompleteTime: 0,
					Remaining: 0
				};
				_class.prototype.Update = function (data) {
					RawData.Id = data;
				};
				_class.prototype.Test = function () {
					return RawData;
				};

				this.Update(data);
			};
			return _class;
		})()
	};
	window.Enum = {
		RepairDockState: {
			Locked: -1,
			Unlocked: 0,
			Repairing: 1
		}
	};

	Object.freeze(window.Class);
	Object.freeze(window.Enum);
	for (let i in window.Enum)
		Object.freeze(window.Enum[i]);
}();
