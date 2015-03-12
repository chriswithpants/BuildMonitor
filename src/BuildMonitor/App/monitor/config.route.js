(function() {
    'use strict';

    angular
        .module('app.monitor')
        .run(appRun);

    appRun.$inject = ['routehelper'];

    /* @ngInject */
    function appRun(routehelper) {
        routehelper.configureRoutes(getRoutes());
    }

    function getRoutes() {
        return [
            {
                url: '/monitor',
                config: {
                    templateUrl: 'app/monitor/monitor.html',
                    controller: 'Monitor',
                    controllerAs: 'vm',
                    title: 'monitor'
                }
            }
        ];
    }
})();
