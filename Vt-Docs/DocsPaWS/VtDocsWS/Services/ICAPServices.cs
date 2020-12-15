using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VtDocsWS.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICAPServices" in both code and config file together.
    [ServiceContract(Namespace = "http://nttdata.com/2012/Pi3")]
    public interface ICAPServices
    {
        [OperationContract]
        VtDocsWS.Services.CAPServices.StoreOpportunity.StoreOpportunityResponse StoreOpportunity(VtDocsWS.Services.CAPServices.StoreOpportunity.StoreOpportunityRequest request);

        [OperationContract]
        VtDocsWS.Services.CAPServices.Authenticate.AuthenticateResponse Authenticate(VtDocsWS.Services.CAPServices.Authenticate.AuthenticateRequest request);

        [OperationContract]
        VtDocsWS.Services.CAPServices.GetDocsInOpportunity.GetDocsInOpportunityResponse GetDocsInOpportunity(VtDocsWS.Services.CAPServices.GetDocsInOpportunity.GetDocsInOpportunityRequest request);

        [OperationContract]
        VtDocsWS.Services.CAPServices.DownloadDoc.DownloadDocResponse DownloadDoc(VtDocsWS.Services.CAPServices.DownloadDoc.DownloadDocRequest request);

        [OperationContract]
        VtDocsWS.Services.CAPServices.GetOpportunities.GetOpportunitiesResponse GetOpportunities(VtDocsWS.Services.CAPServices.GetOpportunities.GetOpportunitiesRequest request);

        [OperationContract]
        VtDocsWS.Services.CAPServices.GetOppApprovals.GetOppApprovalsResponse GetOppApprovals(VtDocsWS.Services.CAPServices.GetOppApprovals.GetOppApprovalsRequest request);

        [OperationContract]
        VtDocsWS.Services.CAPServices.ApproveOpportunity.ApproveOpportunityResponse ApproveOpportunity(VtDocsWS.Services.CAPServices.ApproveOpportunity.ApproveOpportunityRequest request);

    }
}
