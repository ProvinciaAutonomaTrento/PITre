using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InformaticaTrentinaPCL.AnalyticsCore;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Signature.MVP;
using InformaticaTrentinaPCL.Signature.Network;
using InformaticaTrentinaPCL.Utils;
using static InformaticaTrentinaPCL.Home.Network.LibroFirmaResponseModel;

namespace InformaticaTrentinaPCL.Signature
{
    public class SignaturePresenter : ISignaturePresenter
    {
        protected ISignatureView view;
        protected ISignatureModel model;
        protected SessionData sessionData;
        protected IAnalyticsManager analyticsManager;
        protected SignatureObject signatureObject;
        AbstractDocumentListItem currentDocument;
        protected IReachability reachability;

        protected List<AbstractDocumentListItem> listStateDaFirmareOTPDocSignatureC = new List<AbstractDocumentListItem>();
        protected List<AbstractDocumentListItem> listStateDaFirmareOTPDocSignatureP = new List<AbstractDocumentListItem>();

        public SignaturePresenter(ISignatureView view, INativeFactory nativeFactory)
        {
            this.view = view;
            this.sessionData = nativeFactory.GetSessionData();
            this.analyticsManager = nativeFactory.GetAnalyticsManager();
            this.signatureObject = new SignatureObject();
            this.reachability = nativeFactory.GetReachability();
            this.listStateDaFirmareOTPDocSignatureP = new List<AbstractDocumentListItem>();
            this.listStateDaFirmareOTPDocSignatureC = new List<AbstractDocumentListItem>();

#if CUSTOM
            this.model = new DummySignatureModel();
#else
            this.model = new SignatureModel();
#endif
        }

        public void Dispose()
        {
            model.Dispose();
        }

        #region IImplementation
        public async void RequestOTP()
        {
            view.OnUpdateLoader(true);
            RequestOTPRequestModel.Body body = new RequestOTPRequestModel.Body(signatureObject.aliasCertificato, signatureObject.dominioCertificato, null, null, null, null);
            RequestOTPRequestModel request = new RequestOTPRequestModel(body, sessionData.userInfo.token);

            RequestOTPResponseModel response = await model.RequestOTP(request);
            view.OnUpdateLoader(false);
            ManageResponse(response);
        }

        public async void SignDocument(AbstractDocumentListItem abstractDocument)
        {            
            currentDocument = abstractDocument;
            view.OnUpdateLoader(true);
            SignDocumentRequestModel.Body body = new SignDocumentRequestModel.Body(
                signatureObject.aliasCertificato, signatureObject.dominioCertificato,
                abstractDocument.GetIdDocumento(), signatureObject.OTP, signatureObject.PIN, NetworkConstants.TIPO_FIRMA_CADES); //TODO Check tipo firma
            SignDocumentRequestModel request = new SignDocumentRequestModel(body, sessionData.userInfo.token);

            SignDocumentResponseModel response = await model.SignDocument(request);
            view.OnUpdateLoader(false);
            List<AbstractDocumentListItem> listDocumentForThankYouPage = new List<AbstractDocumentListItem>();
            listDocumentForThankYouPage.Add(abstractDocument);
            ManageResponse(response, listDocumentForThankYouPage);
        }

        /// <summary>
        /// Signs the documents hsm.
        /// </summary>
        /// <param name="listStateDaFirmareOTP">List state da firmare otp.</param>

        public async void SignDocumentsHSM(List<AbstractDocumentListItem> listStateDaFirmareOTP, SignFlowType signFlowType)
        {
            await CallSignDocumentHSM(listStateDaFirmareOTP, signFlowType);
        }

        /// <summary>
        /// Calls the sign document hsm.
        /// </summary>
        /// <returns>The sign document hsm.</returns>
        /// <param name="listStateDaFirmareOTP">List state da firmare otp.</param>
        public async Task CallSignDocumentHSM(List<AbstractDocumentListItem> listStateDaFirmareOTP, SignFlowType signFlowType)
        {            
            view.OnUpdateLoader(true);
            RequestOTPRequestModel.Body body = new RequestOTPRequestModel.Body(
                signatureObject.aliasCertificato,
                signatureObject.dominioCertificato,
                otpFirma: signatureObject.OTP,
                pinCertificato: signatureObject.PIN,
                tipoFirma: GetDocumentType(listStateDaFirmareOTP.First()),
                cofirma: signFlowType == SignFlowType.PARALLELA);
            RequestOTPRequestModel request = new RequestOTPRequestModel(body, sessionData.userInfo.token);

            SignDocumentResponseModel response = await model.SignDocumentsHSM(request);
            view.OnUpdateLoader(false);
            ManageResponse(response, listStateDaFirmareOTP);
        }

        public string GetDocumentType(AbstractDocumentListItem document)
        {
            if (document != null && document is SignDocumentModel)
            {
                SignDocumentModel signDocumentModel = (SignDocumentModel)document;

                if (signDocumentModel.tipoFirma.ToLower().Equals(NetworkConstants.CONSTANT_TIPO_FIRMA_DOC_SIGNATURE_P))
                {
                    return NetworkConstants.TIPO_FIRMA_PADES;
                }
                else if (signDocumentModel.tipoFirma.ToLower().Equals(NetworkConstants.CONSTANT_TIPO_FIRMA_DOC_SIGNATURE_cades))
                {
                    return NetworkConstants.TIPO_FIRMA_CADES;
                }
            }
            return LocalizedString.PDF_LABEL.Get();
        }

        public void UpdateAlias(string alias)
        {
            signatureObject.aliasCertificato = alias;
            CheckData();
        }

        public void UpdateDomain(string domain)
        {
            signatureObject.dominioCertificato = domain;
            CheckData();
        }

        public void UpdateOTP(string OTP)
        {
            signatureObject.OTP = OTP;
            CheckData();
        }

        public void UpdatePIN(string PIN)
        {
            signatureObject.PIN = PIN;
            CheckData();
        }


        public void OnViewReady()
        {
            CheckData();
        }
#endregion
#region ManageResponse
        private void ManageResponse(RequestOTPResponseModel response)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        view.OnOTPRequested();
                        break;
                    default:
                        view.ShowError(LocalizedString.REQUEST_OTP_ERROR.Get());
                        //  view.ShowError("REQUEST_OTP_ERROR");
                        //  view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                        break;
                }
            }
        }

        private void ManageResponse(SignDocumentResponseModel response,List<AbstractDocumentListItem> listStateDaFirmareOTP)
        {
            ResponseHelper responseHelper = new ResponseHelper(view, response, reachability);
            if (responseHelper.IsValidResponse())
            {
                switch (response.Code)
                {
                    case 0:
                        foreach (var docSigned in response.InfoFirma)
                        {
                            if (!("OK".Equals(docSigned.Status)))
                            {
                                RemoveItemFromList(listStateDaFirmareOTP, docSigned.IdDocumento);
                            } 
                        }
                        view.SignatureDone(Ext.CreateStringForThankYouPage(listStateDaFirmareOTP));
                        break;
                    default:
                        view.ShowError(LocalizedString.GENERIC_ERROR.Get());
                        break;
                }
            }
        }

        private void RemoveItemFromList(List<AbstractDocumentListItem> listStateDaFirmareOTP, string documentIdToRemove)
        {
            foreach (var doc in listStateDaFirmareOTP)
            {
                
                if (documentIdToRemove.Equals(doc.GetIdDocumento()))
                {
                    listStateDaFirmareOTP.Remove(doc);
                    return;
                }
            }
        }
#endregion
#region Utility
        void CheckData()
        {
            /*view.EnableRequestOTPButton(!string.IsNullOrEmpty(signatureObject.alias) &&
                                        !string.IsNullOrEmpty(signatureObject.domain) &&
                                        !string.IsNullOrEmpty(signatureObject.PIN));*/
            view.EnableRequestOTPButton(true);

            view.EnableSignatureButton(!string.IsNullOrEmpty(signatureObject.aliasCertificato) &&
                                       !string.IsNullOrEmpty(signatureObject.dominioCertificato) &&
                                       !string.IsNullOrEmpty(signatureObject.PIN) &&
                                       !string.IsNullOrEmpty(signatureObject.OTP));
        }
#endregion
    }

    public class SignatureObject
    {

        public string aliasCertificato { get; set; }
        public string dominioCertificato { get; set; }
        public string PIN { get; set; }
        public string OTP { get; set; }

    }
}
