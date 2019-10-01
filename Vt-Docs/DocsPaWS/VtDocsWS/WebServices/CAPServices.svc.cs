using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using VtDocsWS.Services;
using log4net;

namespace VtDocsWS.WebServices
{
    /// <summary>
    /// Metodi per servizi CAP EMEA
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(Namespace = "http://nttdata.com/2012/Pi3")]
    public class CAPServices : ICAPServices
    {
        private ILog logger = LogManager.GetLogger(typeof(CAPServices));

        
        public Services.CAPServices.StoreOpportunity.StoreOpportunityResponse StoreOpportunity(Services.CAPServices.StoreOpportunity.StoreOpportunityRequest request)
        {
            logger.Info("BEGIN");
            Services.CAPServices.StoreOpportunity.StoreOpportunityResponse response = Manager.CAPServicesManager.StoreOpportunity(request);

            Utils.CheckFaultException(response);
            logger.Info("END");
            return response;
        }

        public Services.CAPServices.Authenticate.AuthenticateResponse Authenticate(Services.CAPServices.Authenticate.AuthenticateRequest request)
        {
            logger.Info("BEGIN");
            Services.CAPServices.Authenticate.AuthenticateResponse response = Manager.CAPServicesManager.Authenticate(request);

            Utils.CheckFaultException(response);

            logger.Info("END");
            return response;
        }

        public Services.CAPServices.GetDocsInOpportunity.GetDocsInOpportunityResponse GetDocsInOpportunity(Services.CAPServices.GetDocsInOpportunity.GetDocsInOpportunityRequest request)
        {
            logger.Info("BEGIN");
            Services.CAPServices.GetDocsInOpportunity.GetDocsInOpportunityResponse response = Manager.CAPServicesManager.GetDocsInOpportunity(request);

            Utils.CheckFaultException(response);

            logger.Info("END");
            return response;
        }

        public Services.CAPServices.DownloadDoc.DownloadDocResponse DownloadDoc(Services.CAPServices.DownloadDoc.DownloadDocRequest request)
        {
            logger.Info("BEGIN");
            Services.CAPServices.DownloadDoc.DownloadDocResponse response = Manager.CAPServicesManager.DownloadDoc(request);

            Utils.CheckFaultException(response);

            logger.Info("END");
            return response;
        }

        public Services.CAPServices.GetOppApprovals.GetOppApprovalsResponse GetOppApprovals(VtDocsWS.Services.CAPServices.GetOppApprovals.GetOppApprovalsRequest request)
        {
            logger.Info("BEGIN");
            Services.CAPServices.GetOppApprovals.GetOppApprovalsResponse response = Manager.CAPServicesManager.GetOppApprovals(request);

            Utils.CheckFaultException(response);

            logger.Info("END");
            return response;
        }

        public Services.CAPServices.ApproveOpportunity.ApproveOpportunityResponse ApproveOpportunity(VtDocsWS.Services.CAPServices.ApproveOpportunity.ApproveOpportunityRequest request)
        {
            logger.Info("BEGIN");
            Services.CAPServices.ApproveOpportunity.ApproveOpportunityResponse response = Manager.CAPServicesManager.ApproveOpportunity(request);

            Utils.CheckFaultException(response);

            logger.Info("END");
            return response;
        }

        public Services.CAPServices.GetOpportunities.GetOpportunitiesResponse GetOpportunities(VtDocsWS.Services.CAPServices.GetOpportunities.GetOpportunitiesRequest request)
        {
            logger.Info("BEGIN");
            Services.CAPServices.GetOpportunities.GetOpportunitiesResponse response = Manager.CAPServicesManager.GetOpportunities(request);

            Utils.CheckFaultException(response);

            logger.Info("END");
            return response;
        }
    }
}
