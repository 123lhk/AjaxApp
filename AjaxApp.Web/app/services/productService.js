app.service('productService', ['$http', '$q',  'serviceSetting', function ($http, $q, serviceSetting) {
	var serviceBase = serviceSetting.apiServiceBaseUri;

	this.getProduct = function() {

		var deferred = $q.defer();
		$http({
			method: 'Get',
			url: serviceBase + '/api/Values/1'
		}).success(function(response) {
			deferred.resolve(response);
		}).error(function (response) {
			if (!response.Message) {
				deferred.reject('Internal error!');
			} else {
				deferred.reject(response.Message);
			}
			
		});
		return deferred.promise;
	};

}]);