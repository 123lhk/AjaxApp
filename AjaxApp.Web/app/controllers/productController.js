app.controller('productController', ['$scope', '$location', 'productService', function ($scope, $location, productService) {

	$scope.product = [];
	
	$scope.gridOptions = {
		multiselect: false,
		enableRowSelection: true,
		columnDefs:
			[{
				field: 'Name',
				displayName: 'Name'
			},
			{
				field: 'Price',
				displayName: 'Price'
			},
			{
				field: 'Made',
				displayName: 'Made'
			},
			{
				field: 'NoOfStock',
				displayName: 'Stocks'
			}]
	};

	productService.getProduct().then(
		function(response) {
			$scope.product = response.Products;
			$scope.gridOptions.data = $scope.product;
		},
		function(errorMessage) {
			alert(errorMessage);
		}
	);

	



}]);