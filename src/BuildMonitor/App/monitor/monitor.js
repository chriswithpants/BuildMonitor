(function() {
    'use strict';

    angular
        .module('app.monitor')
        .controller('Monitor', Monitor);

    Monitor.$inject = ['_', '$timeout', 'monitorHub', 'dataservice', 'logger'];

    /* @ngInject */
    function Monitor(_, $timeout, monitorHub, dataservice, logger) {
        /*jshint validthis: true */
        var vm = this;
        vm.items = [];
        vm.title = 'Build Monitor';

        activate();

        function activate() {
            monitorHub.buildStatusUpdated = buildStatusUpdated;

            return getBuildStatuses().then(function () {
                logger.info('Activated Build Monitor View');
            });
        }

        function getBuildStatuses() {
            return dataservice.getBuildStatuses().then(function (data) {
                vm.items = data;
                return vm.items;
            });
        }

        function buildStatusUpdated(model) {
            $timeout(function () {
                addBuildStatus(null, model);
            });
        }

        function addBuildStatus(e, model) {
            var idx = _.findIndex(vm.items, { 'id': model.id });

            if (idx < 0) {
                vm.items.push(model);
            } else {
                vm.items[idx] = model;
            }
        }
    }
})();
