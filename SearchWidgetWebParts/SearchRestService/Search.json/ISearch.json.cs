using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SearchRestService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISearch" in both code and config file together.
    [ServiceContract]
    public interface ISearch
    {
        [OperationContract]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Stream Query(string q, int top, int skip, string select, string orderBy, bool includeRefiners, string refiners);
    }
}

