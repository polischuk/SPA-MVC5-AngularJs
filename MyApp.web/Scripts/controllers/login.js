var LoginController = function ($scope, Page, LoginFactory, $location) {
 
    Page.setTitle("MVC5 angular | contacts page");
    $scope.userName = "admin";
    $scope.password = "q1w2e3";
    $scope.loginClick = function() {
        LoginFactory.Login($scope.userName, $scope.password).then(function (result) {
            if(result === "OK")
                $location.path('/');
        });
    };
}
LoginController.$inject = ['$scope', 'Page', 'LoginFactory', '$location'];

