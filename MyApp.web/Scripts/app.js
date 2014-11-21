var app = angular.module('app', ['ngRoute']);
app.controller('GlobalController', GlobalController);
app.controller('HomeController', HomeController);
app.controller('LoginController', LoginController);

app.factory('HomeFactory', HomeFactory);
app.factory('LoginFactory', LoginFactory);
app.factory('Page', function () {
    var title = 'Angular + MVC5';
    return {
        title: function () { return title; },
        setTitle: function (newTitle) { title = newTitle; }
    };
});

var configFunction = function ($routeProvider, $httpProvider, $locationProvider) {
    $locationProvider.html5Mode({
        enabled: true,
        requireBase: false
    });
    $routeProvider.
        when('/', {
            templateUrl: '/templates/home/index.html',
            controller: HomeController
        })
     .when('/login', {
         templateUrl: '/templates/login/index.html',
         controller: LoginController
     })
    ;

}
configFunction.$inject = ['$routeProvider', '$httpProvider', '$locationProvider'];

app.config(configFunction);

