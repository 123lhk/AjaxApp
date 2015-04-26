var app = angular.module('ajaxApp', ['ngAnimate', 'ui.bootstrap', 'ngRoute', 'LocalStorageModule', 'angular-loading-bar']);

app.config(function ($routeProvider, $locationProvider) {

	

	$routeProvider.when("/login", {
		controller: 'loginController',
		templateUrl: "/app/views/login.html"
	});

	
	$routeProvider.otherwise({ redirectTo: "/home" });

	//$locationProvider.html5Mode(true);
});

//change this
app.constant('serviceSetting', {
	apiServiceBaseUri: 'http://localhost:59400'
});