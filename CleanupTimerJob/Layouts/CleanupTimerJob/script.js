///<reference path="http://sharepoint/_layouts/MicrosoftAjax.js"/>
///<reference path="http://sharepoint/_layouts/SP.debug.js"/>
function GetWebUrl() {
  var webUrl = _spPageContextInfo.webServerRelativeUrl;
  if (webUrl.substr(webUrl.length - 1, 1) != "/") {
    webUrl += "/";
  }
  return webUrl; 
}

function submitLibraryToCleanup(isOldSchool) {
    var listId = SP.ListOperation.Selection.getSelectedList();
    if (listId) {
        if (isOldSchool) {
            __doPostBack('CleanupPostBackEvent', listId);
        } else {
            var notification = null;
            var request = new Sys.Net.WebRequest();
            request.set_url(GetWebUrl() + "_vti_bin/CleanupTimerJob/CleanupService.svc/SubmitLibraryToCleanup("+listId+")");
            request.set_httpVerb("POST");
            request.add_completed(function(executor, eventArgs) {
                SP.UI.Notify.removeNotification(notification);
                SP.UI.Notify.addNotification("Done",false);
            });             
            notification = SP.UI.Notify.addNotification("Submitting library to cleanup",true);
            request.invoke();
        }
    }
}

