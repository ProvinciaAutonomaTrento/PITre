using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.DocsPaWR;
using NttDataWA.UIManager;
using NttDataWA.Utils;
using NttDatalLibrary;


namespace NttDataWA.Utils
{

    public delegate void DocumentConsolidatedDelegate(object sender, DocumentConsolidatedEventArgs e);

    public class DocumentConsolidationHandler
    {

        private event DocumentConsolidatedDelegate _consolidated = null;
        private InfoUtente _userInfo;
        private static DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.GetWS();

        public DocumentConsolidationHandler(DocumentConsolidatedDelegate consolidated, InfoUtente userInfo)
        {
            _consolidated = consolidated;
            _userInfo = userInfo;
        }

        public MassiveOperationReport ConsolidateDocumentMassive(DocsPaWR.DocumentConsolidationStateEnum toState, List<MassiveOperationTarget> items)
        {
            MassiveOperationReport report = new MassiveOperationReport();

            foreach (MassiveOperationTarget mot in items)
            {
                MassiveOperationReport.MassiveOperationResultEnum result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                string message = string.Empty;
                try
                {
                    DocsPaWR.DocumentConsolidationStateInfo info = docsPaWS.ConsolidateDocumentById_AM(_userInfo, mot.Id, toState);
                    if (info.State == toState)
                    {
                        result = MassiveOperationReport.MassiveOperationResultEnum.OK;
                        message = "Il documento è stato consolidato correttamente";
                        // Notifica evento di consolidamento del documento
                        if (this._consolidated != null)
                            this._consolidated(this, new DocumentConsolidatedEventArgs { Info = info });
                    }
                }
                catch (System.Web.Services.Protocols.SoapException ex)
                {
                    ApplicationException originalEx = SoapExceptionParser.GetOriginalException(ex);
                    result = MassiveOperationReport.MassiveOperationResultEnum.KO;
                    message = originalEx.Message;
                }
                finally
                {
                    report.AddReportRow(mot.Codice, result, message);
                }
            }
            return report;
        }

        public void ConsolidateDocument(SchedaDocumento document, DocsPaWR.DocumentConsolidationStateEnum toState)
        {
            try
            {
                DocsPaWR.InfoUtente userInfo = UserManager.GetInfoUser();
                // Contesto di esecuzione dell'azione di consolidamento nel dettaglio del documento
                DocsPaWR.DocumentConsolidationStateInfo info = docsPaWS.ConsolidateDocumentById(userInfo, document.systemId, toState);
                document.ConsolidationState = info;

                // Notifica evento di consolidamento del documento
                if (this._consolidated != null)
                    this._consolidated(this, new DocumentConsolidatedEventArgs { Info = info });

            }
            catch (Exception ex)
            {
                //DocsPaUtils.Exceptions.SoapExceptionParser.ThrowOriginalException(ex);
            }
        }
    }

    public class DocumentConsolidatedEventArgs : System.EventArgs
    {
        public DocsPaWR.DocumentConsolidationStateInfo Info { get; set; }
    }

}