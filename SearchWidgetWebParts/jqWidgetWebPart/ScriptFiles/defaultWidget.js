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