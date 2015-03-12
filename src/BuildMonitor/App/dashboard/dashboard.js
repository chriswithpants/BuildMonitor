(function() {
    'use strict';

    angular
        .module('app.dashboard')
        .controller('Dashboard', Dashboard);

    Dashboard.$inject = ['$q', 'logger'];

    function Dashboard($q, logger) {
        /*jshint validthis: true */
        var vm = this;

        activate();

        function activate() {
            var promises = [];
            return $q.all(promises).then(function() {
                logger.info('Activated Dashboard View');
            });
        }
    }

})();
