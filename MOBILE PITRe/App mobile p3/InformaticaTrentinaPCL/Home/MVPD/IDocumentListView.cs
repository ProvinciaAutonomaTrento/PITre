using System.Collections.Generic;
using InformaticaTrentinaPCL.Home.ActionDialog;
using InformaticaTrentinaPCL.Filter;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.OpenFile.Network;
using InformaticaTrentinaPCL.OpenFile;

namespace InformaticaTrentinaPCL.Home.MVP
{
    public interface IDocumentListView : IGeneralView
    {
        void UpdateList(List<AbstractDocumentListItem> documents);
        void OnDocumentUpdated(AbstractDocumentListItem document);
        void DoOpenFirma(AbstractDocumentListItem abstractDocument);
        void DoSignAll();
        void DoRejectAll();
        void DoOpenViewOtpCades(List<AbstractDocumentListItem> listStateDaFirmareOTP, int TotalRecordCountSigned);
        void DoOpenViewOtpCadesNotSigned(List<AbstractDocumentListItem> listStateDaFirmareOTP, int TotalRecordCountSigned);
        void DoOpenViewOtpPades(List<AbstractDocumentListItem> listStateDaFirmareOTP,int TotalRecordCountSigned);
        void ShowAlertWithNumberDocuments(string description, string actionType);
        void DoOpenDialog(List<DialogItem> itemList);
        void DoAccetta(AbstractDocumentListItem abstractDocument);
        void DoAccettaEADL(AbstractDocumentListItem abstractDocument);
        void DoRifiuta(AbstractDocumentListItem abstractDocument);
        void DoApriDocumento(AbstractDocumentListItem abstractDocument);
        void DoApriFascicolo(AbstractDocumentListItem abstractDocument);
        void DoRimuoviDaADL(AbstractDocumentListItem abstractDocument);
        void DoCondividi(AbstractDocumentListItem abstractDocument);
        void DoAssegna(AbstractDocumentListItem abstractDocument);
        void DoViewed(AbstractDocumentListItem abstractDocument);
        void DoViewedADL(AbstractDocumentListItem abstractDocument);
        void DoInserisciInADL(AbstractDocumentListItem abstractDocument);
        void UpdateFilterUI(FilterModel filterModel);
        void OpenFilterView(FilterModel filterModel);
        void OnShareLinkReady(string link);
        void DoApriDocumentoCondiviso(SharedDocumentBundle sharedDocument);
        void OnRemoveFromADLOk(string message);
        void OnAddInADLOk(string message);
        void OnViewedOk(string message);
        void OnViewedADLOk(string message);
        void CompletedActionOK(Dictionary<string,string> extra);
        void OnSignCompleted(string message);
    }
}
