app.service('requestInterceptor', ['$q', '$location', 'localStorageService', function($q, $location, localStorageService) {
	this.request = function(config) {
		if (!config.headers) {
			config.headers = {};
		}

		var authData = localStorageService.get('authorizationData');
		if (authData) {
			config.headers.Authorization = 'Bearer ' + authData.token;
		}

		return config;
	};


	this.responseError = function(rejection) {
		if (rejection.status === 401) {
			$location.path('/login');
		}
		return $q.reject(rejection);
	}

}]);