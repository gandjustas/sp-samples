using Microsoft.SharePoint.Client.Services;
using System.ServiceModel.Activation;
using System.Runtime.InteropServices;
using System;
using Microsoft.SharePoint;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace CleanupTimerJob
{
    [Guid("36471285-d168-49ea-b191-6c83cfe1fe3e")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceContract]
    public class CleanupService
    {
        [OperationContract]
        [WebInvoke(
            UriTemplate = "/SubmitLibraryToCleanup({listId})",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        public void SubmitLibraryToCleanup(string listId)
        {
            CleanupUtility.AddCleanupWorkitem(SPContext.Current.Web, new Guid(listId));
        }
    }
}
