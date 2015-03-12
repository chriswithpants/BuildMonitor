(function() {
    'use strict';

    angular
        .module('app.monitor')
        .factory('monitorHub', monitorHub);

    monitorHub.$inject = ['Hub', 'exception'];

    /* @ngInject */
    function monitorHub(Hub, exception) {
        var service = {
            buildStatusUpdated: buildStatusUpdated
        };

        var hub = new Hub('MonitorHub', {
            useSharedConnection: true,
            logging: true,
            listeners: {
                buildStatusUpdated: function(status) {
                    service.buildStatusUpdated(status);
                }
            },
            methods: [
            ],
            queryParams: {},
            errorHandler: exception.catcher
        });

        return service;

        //////////////
        function buildStatusUpdated(model) {
        }
    }

})();
