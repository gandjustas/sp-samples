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
