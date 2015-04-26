app.controller('loginController', ['$scope', 'authenticationService', '$location', function ($scope, authenticationService, $location) {

	$scope.loginDetail = {};
	$scope.text = 'aaaaaa';

	$scope.loginClick = function($event) {
		$event.preventDefault();
		$event.stopPropagation();

		var result = authenticationService.login($scope.loginDetail);

		result.then(
			function(date) {
				$location.path('/product');
			},
			function(errorMessage) {
				alert(errorMessage);
			}
		);


	}

}]);