app.service('authenticationService', ['$http', '$q', 'localStorageService','serviceSetting', function ($http, $q, localStorageService, serviceSetting) {
	var serviceBase = serviceSetting.apiServiceBaseUri;

	this.login = function (loginDetail) {

		var deferred = $q.defer();
		var requestData = 'grant_type=password&username=' + loginDetail.username + '&password=' + loginDetail.password;

		$http({
			method: 'POST',
			url: serviceBase + '/token', 
			data: requestData, 		
			headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
		}).success(function(response) {
			localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName });
			deferred.resolve(response.data);

		}).error(function (response) {
			if (!angular.isObject(response) || !response.error_description) {
				deferred.reject("Login Failed!");
			} else {
				deferred.reject(response.error_description);
			}
			
		});

		return deferred.promise;
	}

}]);