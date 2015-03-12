(function () {
    'use strict';

    angular
        .module('app.core')
        .factory('dataservice', dataservice);

    dataservice.$inject = ['$http', '$location', '$q', 'exception', 'logger'];

    /* @ngInject */
    function dataservice($http, $location, $q, exception, logger) {
        var isPrimed = false;
        var primePromise;

        var service = {
            getBuildStatuses: getBuildStatuses,
            ready: ready
        };

        return service;

        function getBuildStatuses() {
            return $http.get('/api/buildstatuses')
                .then(getBuildStatusesComplete)
                .catch(function (message) {
                    exception.catcher('XHR Failed for getBuildStatuses')(message);
                    $location.url('/');
                });

            function getBuildStatusesComplete(data, status, headers, config) {
                return data.data;
            }
        }

        function prime() {
            // This function can only be called once.
            if (primePromise) {
                return primePromise;
            }

            primePromise = $q.when(true).then(success);
            return primePromise;

            function success() {
                isPrimed = true;
                logger.info('Primed data');
            }
        }

        function ready(nextPromises) {
            var readyPromise = primePromise || prime();

            return readyPromise
                .then(function() { return $q.all(nextPromises); })
                .catch(exception.catcher('"ready" function failed'));
        }

    }
})();
