app.controller('productController', ['$scope', '$location', 'productService', function ($scope, $location, productService) {

	$scope.product = '';
	productService.getProduct().then(
		function(response) {
			$scope.product = response;
		},
		function(errorMessage) {
			alert(errorMessage);
		}
	);



}]);