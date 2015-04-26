app.controller('loginController', ['$scope', 'authenticationService', function ($scope, authenticationService) {

	$scope.loginDetail = {};
	$scope.text = 'aaaaaa';

	$scope.loginClick = function($event) {
		$event.preventDefault();
		$event.stopPropagation();

		var result = authenticationService.login($scope.loginDetail);

		result.then(
			function(date) {
				alert('logined in!');
			},
			function(errorMessage) {
				alert(errorMessage);
			}
		);


	}

}]);