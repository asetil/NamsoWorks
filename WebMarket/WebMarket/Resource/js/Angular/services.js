webmarketApp.factory('regionService', ['$resource', '$q', function ($resource, $q) {
    return {
        getRegions: function () {
            var deferred = $q.defer();
            $resource('/Address/GetDistricts', {}, { query: { method: 'POST', isArray: false } }).query(
                function (result) {
                    deferred.resolve(result);
                },
                function (response) {
                    //handle response
                }
            );
            return deferred.promise;
        },

        saveRegion: function (regionID) {
            var deferred = $q.defer();
            $resource('/Helper/RefreshRegion', { regionID: regionID }, { query: { method: 'POST' } }).save(function (result) {
                deferred.resolve(result);
            }, function (response) {
                //handle response
            });
            return deferred.promise;
        }
    };
}]);