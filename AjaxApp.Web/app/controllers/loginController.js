app.controller('loginController', ['$scope', 'authenticationService', '$location', 'utilityService', function ($scope, authenticationService, $location, utilityService) {

	$scope.loginDetail = {};
	$scope.loginAlerts = [];

	$scope.loginClick = function($event) {
		$event.preventDefault();
		$event.stopPropagation();

		var result = authenticationService.login($scope.loginDetail);

		result.then(
			function(date) {
				$location.path('/product');
			},
			function(errorMessage) {
				var alerts = utilityService.constructAlerts([errorMessage], 'danger');
				utilityService.pushArray($scope.loginAlerts, alerts);
			}
		);


	}

	$scope.closeAlert = function (index) {
		$scope.loginAlerts.splice(index, 1);
	};

}]);