app.controller('navController', ['$scope', '$location', 'authenticationService', '$cookies', function ($scope, $location, authenticationService) {

	$scope.logOutClick = function() {
		authenticationService.logOut();
		$location.path('/home');

	}

	$scope.authData = authenticationService.getAuthData();

	$scope.SetCollapse = function () {
		$scope.navCollapsed = true;
	}

}]);