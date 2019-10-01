using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;

namespace VtDocsWS.Services
{
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface IProceedings
    {
        [OperationContract]
        VtDocsWS.Services.Proceedings.StartProceeding.StartProceedingResponse StartProceeding(VtDocsWS.Services.Proceedings.StartProceeding.StartProceedingRequest request);

        [OperationContract]
        VtDocsWS.Services.Proceedings.GetProceeding.GetProceedingResponse GetProceeding(VtDocsWS.Services.Proceedings.GetProceeding.GetProceedingRequest request);

        [OperationContract]
        VtDocsWS.Services.Proceedings.AddDocToProceeding.AddDocToProceedingResponse AddDocToProceeding(VtDocsWS.Services.Proceedings.AddDocToProceeding.AddDocToProceedingRequest request);

        [OperationContract]
        VtDocsWS.Services.Proceedings.GetUnreadNotifications.GetUnreadNotificationsResponse GetUnreadNotifications(VtDocsWS.Services.Proceedings.GetUnreadNotifications.GetUnreadNotificationsRequest request);

        [OperationContract]
        VtDocsWS.Services.Proceedings.SetReadNotifications.SetReadNotificationsResponse SetReadNotifications(VtDocsWS.Services.Proceedings.SetReadNotifications.SetReadNotificationsRequest request);

    }
}
