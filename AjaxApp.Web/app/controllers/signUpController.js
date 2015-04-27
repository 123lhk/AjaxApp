app.controller('signupController', ['$scope', 'authenticationService', '$location', 'utilityService', '$interval', function ($scope, authenticationService, $location, utilityService, $interval) {

	$scope.signupDetail = {email:'', password:'', confirmPassword:''};
	$scope.signupSuccess = false;
	$scope.signupAlerts = [];
	$scope.countDown = 3;

	$scope.signupClick = function($event) {
		$event.preventDefault();
		$event.stopPropagation();

		var result = authenticationService.signup($scope.signupDetail);

		result.then(
			function (date) {
				$scope.signupSuccess = true;
				$scope.counter = $interval(function () {
					$scope.countDown = $scope.countDown - 1;
					if ($scope.countDown <= 0) {
						$interval.cancel($scope.counter);
						$location.path('/login');
					}
				}, 1000);

			},
			function (modelState) {
				var errors = [];
				angular.forEach(modelState, function(val, key) {
					angular.forEach(val, function(message, key2) {
						errors.push(message);
					});

				});
				var alerts = utilityService.constructAlerts(errors, 'danger');
				utilityService.pushArray($scope.signupAlerts, alerts);

			}
		);
	}

	$scope.closeAlert = function (index) {
		$scope.signupAlerts.splice(index, 1);
	};


}]);