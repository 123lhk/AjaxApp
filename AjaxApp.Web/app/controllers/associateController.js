app.controller('associateController', ['$scope', 'authenticationService', '$location', 'utilityService', 'serviceSetting', '$interval', function ($scope, authenticationService, $location, utilityService, serviceSetting, $interval) {

	$scope.savedSuccessfully = false;
	$scope.signupAlerts = [];
	$scope.countDown = 3;

	$scope.registerData = {
		Email: authenticationService.externalAuthData.userName,
		Provider: authenticationService.externalAuthData.provider,
		ExternalAccessToken: authenticationService.externalAuthData.externalAccessToken
	};

	$scope.registerExternal = function () {
		authenticationService.registerExternal($scope.registerData).then(function (response) {
			$scope.savedSuccessfully = true;
			$scope.counter = $interval(function () {
				$scope.countDown = $scope.countDown - 1;
				if ($scope.countDown <= 0) {
					$interval.cancel($scope.counter);
					$location.path('/product');
				}
			}, 1000);

		},
        function (modelState) {
        	var errors = [];
        	angular.forEach(modelState, function (val, key) {
        		angular.forEach(val, function (message, key2) {
        			errors.push(message);
        		});

        	});
        	var alerts = utilityService.constructAlerts(errors, 'danger');
        	utilityService.pushArray($scope.signupAlerts, alerts);
        });
	};

	$scope.closeAlert = function (index) {
		$scope.signupAlerts.splice(index, 1);
	};

}]);