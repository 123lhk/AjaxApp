app.service('authenticationService', ['$http', '$q', 'localStorageService','serviceSetting', function ($http, $q, localStorageService, serviceSetting) {
	var serviceBase = serviceSetting.apiServiceBaseUri;
	var that = this;

	var authenticationStatus = {
		isAuth: false,
		userName: ''
	}
	this.authenticationStatus = authenticationStatus;
	var externalAuthData = {
		provider: "",
		userName: "",
		externalAccessToken: ""
	};
	this.externalAuthData = externalAuthData;

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
			authenticationStatus.isAuth = true;
			authenticationStatus.userName = response.userName;
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

	this.logOut = function () {

		localStorageService.remove('authorizationData');

		authenticationStatus.isAuth = false;
		authenticationStatus.userName = "";
	};

	this.getAuthData = function() {
		var authData = localStorageService.get('authorizationData');
		if (authData) {
			authenticationStatus.isAuth = true;
			authenticationStatus.userName = authData.userName;
		} else {
			authenticationStatus.isAuth = false;
			authenticationStatus.userName = "";
		}

		return authenticationStatus;
	};


	this.signup = function(signupDetail) {
		var deferred = $q.defer();
		var requestData = {
			'Email' : signupDetail.email,
			'Password' : signupDetail.password,
			'ConfirmPassword' : signupDetail.confirmPassword
		};

		$http({
			method: 'POST',
			url: serviceBase + '/api/account/register',
			data: requestData
		}).success(function (response) {
			
			deferred.resolve("User registered!");

		}).error(function (response) {
			if (!angular.isObject(response) || !response.ModelState) {
				deferred.reject("Login Failed!");
			} else {
				deferred.reject(response.ModelState);
			}

		});

		return deferred.promise;
	}

	this.obtainAccessToken = function (externalData) {

		var deferred = $q.defer();

		$http.get(serviceBase + '/api/account/ObtainLocalAccessToken', { params: { provider: externalData.provider, externalAccessToken: externalData.externalAccessToken } }).success(function (response) {

			localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName });

			authenticationStatus.isAuth = true;
			authenticationStatus.userName = response.userName;

			deferred.resolve(response);

		}).error(function (err, status) {
			that.logOut();
			if (!angular.isObject(response) || !response.ModelState) {
				deferred.reject("Login Failed!");
			} else {
				deferred.reject(response.ModelState);
			}
		});

		return deferred.promise;

	};

	this.registerExternal = function (registerExternalData) {

		var deferred = $q.defer();

		$http.post(serviceBase + '/api/account/registerexternal', registerExternalData).success(function (response) {

			localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName });

			authenticationStatus.isAuth = true;
			authenticationStatus.userName = response.userName;

			deferred.resolve(response);

		}).error(function (err, status) {
			that.logOut();
			deferred.reject(err.ModelState);
		});

		return deferred.promise;
	};

}]);