var app = angular.module('ajaxApp', ['ngAnimate', 'ui.bootstrap', 'ngRoute', 'LocalStorageModule', 'angular-loading-bar', 'ui.grid', 'ui.grid.selection', 'angular.filter', 'ngCookies']);

app.config(function ($routeProvider, $locationProvider) {

	$routeProvider.when("/product", {
		controller: 'productController',
		templateUrl: "/app/views/products.html"
	});

	$routeProvider.when("/login", {
		controller: 'loginController',
		templateUrl: "/app/views/login.html"
	});

	$routeProvider.when("/signup", {
		controller: 'signupController',
		templateUrl: "/app/views/signup.html"
	});

	$routeProvider.when("/home", {
		controller: 'loginController',
		templateUrl: "/app/views/home.html"
	});

	$routeProvider.when("/associate", {
		controller: "associateController",
		templateUrl: "/app/views/associate.html"
	});
	
	$routeProvider.otherwise({ redirectTo: "/home" });

	//$locationProvider.html5Mode(true);
});

//change this
app.constant('serviceSetting', {
	apiServiceBaseUri: 'http://localhost:59400'
});

app.config(function ($httpProvider) {
	$httpProvider.interceptors.push('requestInterceptor');
});

app.run(['authenticationService', function (authenticationService) {
	authenticationService.getAuthData();
}]);