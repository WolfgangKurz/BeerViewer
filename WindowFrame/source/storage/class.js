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
			const _class = function (homeport, data) {
				const _homeport = homeport;
				const RawData = {
					Id: 0,
					State: -1,
					ShipId: 0,
					Ship: null,
					CompleteTime: 0,
					Remaining: 0
				};
				_class.prototype.Update = function (data) {
					RawData.Id = data.api_id;
					RawData.State = data.api_state;
					RawData.ShipId = data.api_ship_id;
					if (RawData.State === Enums.RepairDockState.Repairing) {
						RawData.Ship = _homeport.Ships[RawData.ShipId];
						RawData.CompleteTime = data.api_complete_time;
					} else {
						RawData.Ship = null;
						RawData.CompleteTime = null;
					}
				};
				_class.prototype.Finish = function () {
					RawData.State = Enums.RepairDockState.Unlocked;
				};
				_class.prototype.Test = function () {
					return RawData;
				};

				this.Update(data);
			};
			return _class;
		})()
	};
	window.Enums = {
		RepairDockState: {
			Locked: -1,
			Unlocked: 0,
			Repairing: 1
		}
	};

	Object.freeze(window.Class);
	Object.freeze(window.Enums);
	for (let i in window.Enums)
		Object.freeze(window.Enums[i]);
}();
