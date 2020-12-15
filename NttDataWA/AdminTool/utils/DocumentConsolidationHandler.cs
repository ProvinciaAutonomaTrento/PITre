using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SAAdminTool.DocsPaWR;

namespace SAAdminTool.utils
{
    public delegate void DocumentConsolidatedDelegate(object sender, DocumentConsolidatedEventArgs e);

    public class DocumentConsolidationHandler
    {

        private event DocumentConsolidatedDelegate _consolidated = null;
        private InfoUtente _userInfo;

        public DocumentConsolidationHandler(DocumentConsolidatedDelegate consolidated,InfoUtente userInfo)
        {
            _consolidated = consolidated;
            _userInfo = userInfo;
        }

        public MassiveOperationReport ConsolidateDocumentMassive(DocsPaWR.DocumentConsolidationStateEnum toState,List<MassiveOperationTarget> items)
        {
            SAAdminTool.utils.MassiveOperationReport report = new utils.MassiveOperationReport();
            DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();
            foreach (MassiveOperationTarget mot in items)
            {
                utils.MassiveOperationReport.MassiveOperationResultEnum result = utils.MassiveOperationReport.MassiveOperationResultEnum.KO;
                string message = string.Empty;
                try
                {
                    DocsPaWR.DocumentConsolidationStateInfo info = ws.ConsolidateDocumentById_AM(_userInfo, mot.Id, toState);
                    if (info.State == toState)
                    {
                        result = utils.MassiveOperationReport.MassiveOperationResultEnum.OK;
                        message = "Il documento è stato consolidato correttamente";
                        // Notifica evento di consolidamento del documento
                        if (this._consolidated != null)
                            this._consolidated(this, new DocumentConsolidatedEventArgs { Info = info });
                    }
                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    ApplicationException originalEx = DocsPaUtils.Exceptions.SoapExceptionParser.GetOriginalException(ex);

                    result = utils.MassiveOperationReport.MassiveOperationResultEnum.KO;
                    message = originalEx.Message;
                }
                finally
                {
                    report.AddReportRow(mot.Codice, result, message);
                }
            }
            return report;
        }

        public void ConsolidateDocument(SchedaDocumento document,DocsPaWR.DocumentConsolidationStateEnum toState)
        {
            try
            {
                DocsPaWR.DocsPaWebService ws = ProxyManager.getWS();

                DocsPaWR.InfoUtente userInfo = UserManager.getInfoUtente();
                // Contesto di esecuzione dell'azione di consolidamento nel dettaglio del documento
                DocsPaWR.DocumentConsolidationStateInfo info = ws.ConsolidateDocumentById(userInfo, document.systemId, toState);
                document.ConsolidationState = info;

                    // Notifica evento di consolidamento del documento
                if (this._consolidated != null)
                        this._consolidated(this, new DocumentConsolidatedEventArgs { Info = info });
                    
            }catch (Exception ex)
            {
                DocsPaUtils.Exceptions.SoapExceptionParser.ThrowOriginalException(ex);
            }
        }
    }

    public class DocumentConsolidatedEventArgs : System.EventArgs
    {
        public DocsPaWR.DocumentConsolidationStateInfo Info { get; set; }
    }
}