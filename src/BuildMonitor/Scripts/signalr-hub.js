(function() {
    'use-strict';

    angular
        .module('SignalR', [])
        .constant('$', $)
        .factory('Hub', Hub);

    Hub.$inject = ['$'];

    function Hub($) {
    
        //This will allow same connection to be used for all Hubs
        //It also keeps connection as singleton.
        var globalConnection = null;

        function initNewConnection(options) {
            var connection = null;
            if (options && options.rootPath) {
                connection = $.hubConnection(options.rootPath, { useDefaultPath: false });
            } else {
                connection = $.hubConnection();
            }

            connection.logging = (options && options.logging ? true : false);
            return connection;
        }

        function getConnection(options) {
            var useSharedConnection = !(options && options.useSharedConnection === false);
            if (useSharedConnection) {
                return globalConnection === null ? globalConnection = initNewConnection(options) : globalConnection;
            }
            else {
                return initNewConnection(options);
            }
        }

        return function (hubName, options) {
            var hub = this;

            hub.connection = getConnection(options);
            hub.proxy = hub.connection.createHubProxy(hubName);

            hub.on = function (event, fn) {
                hub.proxy.on(event, fn);
            };
            hub.invoke = function (method, args) {
                return hub.proxy.invoke.apply(hub.proxy, arguments);
            };
            hub.disconnect = function () {
                hub.connection.stop();
            };
            hub.connect = function () {
                hub.connection.start();
            };

            if (options && options.listeners) {
                angular.forEach(options.listeners, function (fn, event) {
                    hub.on(event, fn);
                });
            }
            if (options && options.methods) {
                angular.forEach(options.methods, function (method) {
                    hub[method] = function () {
                        var args = $.makeArray(arguments);
                        args.unshift(method);
                        return hub.invoke.apply(hub, args);
                    };
                });
            }
            if (options && options.queryParams) {
                hub.connection.qs = options.queryParams;
            }
            if (options && options.errorHandler) {
                hub.connection.error(options.errorHandler);
            }

            //Adding additional property of promise allows to access it in rest of the application.
            hub.promise = hub.connection.start();
            return hub;
        };
    }

})();
