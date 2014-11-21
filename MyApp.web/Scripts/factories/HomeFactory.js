var HomeFactory = function ($http, $q) {

    var factory = {};

    factory.GetUserInfo = function () {

        var deferredObject = $q.defer();

        $http.post('/Account/GetUserInfo', {}).
        success(function (data) {
            deferredObject.resolve(data);
        }).
        error(function () {
            deferredObject.resolve(null);
        });



        return deferredObject.promise;
    }

    factory.LogOff = function () {

        var deferredObject = $q.defer();

        $http.post('/Account/LogOff', {}).
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

HomeFactory.$inject = ['$http', '$q'];