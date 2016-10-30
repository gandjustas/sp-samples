///#source 1 1 /ScriptFiles/defaultWidget.js
///<reference path="../jquery/jQueryFiles/jquery-1.8.3.js" />
///<reference path="../jquery/jQueryFiles/jquery-ui-1.9.2.js" />
///<reference path="jsrender.js" />
///<reference path="amplify.js" />
///<reference path="jquery.webpart.js" />
(function ($, undefined) {
    "use strict"

    $.widget('sharepoint.defaultWidget', {
        _create: function () {
            //beware parameter encoding
            if (!this.options.templateName || !this.options.serviceUrl)
                return;

            $.templateManager.register(this.options.templateName, this.options.templateUrl);
            var uid = (Math.random() * 10000).toString();
            $.dataManager.register(uid, this.options.serviceUrl);

            var self = this.element;
            $.when(
                $.dataManager.get(uid),
                $.templateManager.get(this.options.templateName)
                )
             .done(function (data, tmpl) { self.html(tmpl.render(data)); });
        }

    });
})(jQuery.noConflict());
///#source 1 1 /ScriptFiles/dataManager.js
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

///#source 1 1 /ScriptFiles/jquery.webpart.js
///<reference path="../jquery/jQueryFiles/jquery-1.8.3.js" />
///<reference path="../jquery/jQueryFiles/jquery-ui-1.9.2.js" />
///<reference path="jsrender.js" />
///<reference path="amplify.js" />
(function ($, undefined) {
    "use strict";

    $(function () {
        $('div[data-widget-name]').each(function () {
            var item = $(this);
            var widgetName = item.data('widget-name');
            if (item[widgetName]) {
                createWidget(widgetName, item);
            } else {
                var scriptUrl = item.data('script-url');                
                if (!scriptUrl) {
                    var siteServerRelativeUrl = _spPageContextInfo.siteServerRelativeUrl != "/"
                            ? _spPageContextInfo.siteServerRelativeUrl
                            : "";

                    scriptUrl = siteServerRelativeUrl + '/SiteAssets/'+widgetName+'.js';
                }

                loadScript(scriptUrl, function () {
                        createWidget(widgetName, item);                    
                });
            }


        });
    });


    function loadScript(url, callback) {

        var script = document.createElement("script")
        script.type = "text/javascript";

        if (script.readyState) {  //IE
            script.onreadystatechange = function () {
                if (script.readyState == "loaded" ||
                        script.readyState == "complete") {
                    script.onreadystatechange = null;
                    callback();
                }
            };
        } else {  //Others
            script.onload = function () {
                callback();
            };
        }

        script.src = url;
        document.getElementsByTagName("head")[0].appendChild(script);
    }


    function createWidget(name, item) {
        item[name](item.data());
    };
})(jQuery.noConflict());

///#source 1 1 /ScriptFiles/templateManager.js
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

