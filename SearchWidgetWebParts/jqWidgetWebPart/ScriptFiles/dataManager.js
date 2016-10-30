///<reference path="../jquery/jQueryFiles/jquery-1.8.3.js" />
///<reference path="../jquery/jQueryFiles/jquery-ui-1.9.2.js" />
///<reference path="jsrender.js" />
///<reference path="amplify.js" />

(function ($, amplify, undefined) {
    "use strict";

    $.dataManager = $.dataManager || (function () {

        function register(resourceId, url, type) {
            amplify.request.define(resourceId, 'ajax', {
                url: url,
                cache: true,
                dataType: 'json',
                type: type || 'GET',

            });
        }

        function get(resourceId, options) {
            var d = new $.Deferred();

            amplify.request({
                resourceId: resourceId,
                data: options,
                success: function (data) { d.resolve(data) },
                error: function (data, status) { d.reject(status) }
            });
            return d;
        }

        return {
            register: register,
            get: get
        }
    })();

})(jQuery.noConflict(), amplify);
