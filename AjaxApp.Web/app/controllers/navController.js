app.controller('navController', ['$scope', '$location', 'authenticationService', function ($scope, $location, authenticationService) {

	$scope.logOutClick = function() {
		authenticationService.logOut();
		$location.path('/home');
	}

	$scope.authData = authenticationService.getAuthData();

}]);