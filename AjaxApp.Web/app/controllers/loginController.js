app.controller('loginController', ['$scope', 'authenticationService', '$location', 'utilityService', 'serviceSetting', function ($scope, authenticationService, $location, utilityService, serviceSetting) {

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

	$scope.authExternalProvider = function (provider) {

		var redirectUri = location.protocol + '//' + location.host + '/authcomplete.html';

		var externalProviderUrl = serviceSetting.apiServiceBaseUri + "/api/Account/ExternalLogin?provider=" + provider
                                                                    + "&responseType=token"
                                                                    + "&redirect_uri=" + redirectUri;
		window.$windowScope = $scope;

		var oauthWindow = window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=600,height=750");
	};

	$scope.authCompletedCB = function (fragment) {

		$scope.$apply(function () {

			if (fragment.haslocalaccount === 'False') {

				authenticationService.logOut();

				authenticationService.externalAuthData = {
					provider: fragment.provider,
					userName: fragment.external_user_name,
					externalAccessToken: fragment.external_access_token
				};

				$location.path('/associate');

			}
			else {
				//Obtain access token and redirect to orders
				var externalData = { provider: fragment.provider, externalAccessToken: fragment.external_access_token };
				authenticationService.obtainAccessToken(externalData).then(function (response) {

					$location.path('/product');

				},
             function (err) {
             	$scope.message = err.error_description;
	             alert($scope.message);
             });
			}

		});
	}

	$scope.closeAlert = function (index) {
		$scope.loginAlerts.splice(index, 1);
	};

}]);