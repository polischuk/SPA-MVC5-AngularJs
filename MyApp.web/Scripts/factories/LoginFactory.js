var LoginFactory = function ($http, $q) {

    var factory = {};

    factory.Login = function (name, pass) {

        var deferredObject = $q.defer();

        $http.post('/Account/Login', {
            userName: name,
            password: pass
        }).
        success(function (data) {
            deferredObject.resolve(data);

        }).
        error(function () {
            deferredObject.resolve(null);
        });



        return deferredObject.promise;
    }
    return factory;
};

LoginFactory.$inject = ['$http', '$q'];