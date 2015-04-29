app.controller('productController', ['$scope', '$location', 'productService', function ($scope, $location, productService) {

	$scope.products = [];
	
	productService.getProduct().then(
		function(response) {
			$scope.products = response.Products;
		},
		function(errorMessage) {
			alert(errorMessage);
		}
	);

	



}]);