app.service('utilityService', [function () {

	this.constructAlerts = function (msgs, type) {

		var alerts = [];

		angular.forEach(msgs, function (value, key) {
			var alert = { msg: value, type: type };

			alerts.push(alert);
		});

		return alerts;
	};

	this.pushArray = function (arr, arr2) {
		arr.push.apply(arr, arr2);
	}
}]);