﻿(function() {
    'use strict';

    angular
        .module('app.core')
        .constant('_', _)
        .constant('toastr', toastr)
        .constant('moment', moment)
        .constant('angularMomentConfig', { timezone: 'Australia/Brisbane' });

})();
