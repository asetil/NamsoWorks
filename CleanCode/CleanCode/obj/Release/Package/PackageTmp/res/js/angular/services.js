osmanApp.service('Data', function () {
    this.message = "I'm data from a service";
    this.sayHello = function () {
        return "Hi!";
    };
});

osmanApp.factory('GithubFactory', function ($http, $q) {
    var GithubFactory = {};
    GithubFactory.search = function (repoName) {
        var deferred = $q.defer();
        $http.get('https://api.github.com/search/repositories?q=' + repoName + '&sort=stars&order=desc')
            .then(function (response) {
                deferred.resolve(response.data);
            });
        return deferred.promise;
    };
    return GithubFactory;
});

osmanApp.factory('productService', ['$resource', '$q', function ($resource, $q) {
    return {
        getProductList: function () {
            var deferred = $q.defer();
            $resource('/Product/GetList', {}, { query: { method: 'GET', isArray: true } }).query(
                function (cv) {
                    deferred.resolve(cv);
                },
                function (response) {
                    //handle response
                }
            );
            return deferred.promise;
        },

        addProduct: function (productId) {
            var deferred = $q.defer();
            $resource('/Product/Add', { id: productId }).save(function (result) {
                deferred.resolve(result);
            }, function (response) {
                //handle response
            });

            return deferred.promise;
        }
    };
}]);