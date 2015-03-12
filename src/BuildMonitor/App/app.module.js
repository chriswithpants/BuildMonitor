(function() {
    'use strict';

    angular
        .module('app', [
            'app.core',
            /*
             * Feature areas
             */
            'app.monitor',
            'app.dashboard',
            'app.layout'
        ]);

})();