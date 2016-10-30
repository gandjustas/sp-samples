///<reference path="../jquery/jQueryFiles/jquery-1.8.3.js" />
///<reference path="../jquery/jQueryFiles/jquery-ui-1.9.2.js" />
///<reference path="jsrender.js" />
///<reference path="amplify.js" />

(function ($, amplify, undefined) {
    "use strict";

    $.templateManager = $.templateManager || (function () {

        function register(name, url) {
            var siteServerRelativeUrl = _spPageContextInfo.siteServerRelativeUrl != "/"
                    ? _spPageContextInfo.siteServerRelativeUrl
                    : "";

            url = url || (siteServerRelativeUrl + '/SiteAssets/templates/' + name + '.tmpl.html');

            amplify.request.define('js-render-tmpl-' + name, 'ajax', {
                url: url,
                cache: true,
                dataType: 'html',
                type: 'GET'
            });
        }

        function get(name) {
            var d = new $.Deferred();
            var tmpl = $.templates[name];
            if (tmpl) {
                d.resolve(tmpl);
            } else {
                amplify.request({
                    resourceId: 'js-render-tmpl-' + name,
                    success: function (data) {
                        d.resolve($.templates(name, data));
                    },
                    error: function (data, status) { d.reject(status) }
                });
            }
            return d;
        }

        return {
            register: register,
            get: get
        };
    })();


})(jQuery.noConflict(), amplify);
