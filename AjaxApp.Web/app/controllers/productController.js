app.controller('productController', ['$scope', '$location', 'productService', function ($scope, $location, productService) {

	$scope.product = '';
	productService.GetProduct().then(
		function(response) {
			$scope.product = response;
		},
		function(errorMessage) {
			alert(errorMessage);
		}
	);



}]);