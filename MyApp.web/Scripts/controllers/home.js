var HomeController = function ($scope, $location, HomeFactory, Page) {
    Page.setTitle("MVC5 + angularjs app");
    $scope.IsAuthorize = false;
    HomeFactory.GetUserInfo().then(function (result) {
        $scope.IsAuthorize = result.status === "OK" ? true : false;
        $scope.User = result.data;
    });

    $scope.LogOff = function() {
        HomeFactory.LogOff().then(function (result) {
            $scope.IsAuthorize = result.status === "OK" ?false: true;
        });
    }

}
HomeController.$inject = ['$scope', '$location', 'HomeFactory', 'Page'];

