///<reference path="jsrender.js"/>
(function ($, undefined) {
    var webServerRelativeUrl = _spPageContextInfo.webServerRelativeUrl != "/"
                                ? _spPageContextInfo.webServerRelativeUrl
                                : "";

    $.dataManager.register('search.json', webServerRelativeUrl + '/_vti_bin/search.json.svc/Query');
    $.templateManager.register('jsRenderSearchResults');


    $.widget('gandjustas.jsRenderSearchResults', {
        _create: function () {
            var dataDeferred = $.dataManager.get('search.json', { q: this.options.query });
            var templateDeferred = $.templateManager.get('jsRenderSearchResults');

            var self = this;
            $.when(dataDeferred, templateDeferred)
             .done(function (data, tmpl) {
                 self.element.html(tmpl.render(data.Results));
             });
        }
    });                       
})(jQuery.noConflict())