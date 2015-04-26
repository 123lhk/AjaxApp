var app = angular.module('ajaxApp', ['ngAnimate', 'ui.bootstrap', 'ngRoute', 'LocalStorageModule', 'angular-loading-bar']);

app.config(function ($routeProvider, $locationProvider) {

	$routeProvider.when("/product", {
		controller: 'productController',
		templateUrl: "/app/views/products.html"
	});

	$routeProvider.when("/login", {
		controller: 'loginController',
		templateUrl: "/app/views/login.html"
	});

	$routeProvider.when("/home", {
		controller: 'loginController',
		templateUrl: "/app/views/home.html"
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