using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Mobile
{
    public class DocManager
    {
        public static DocsPaVO.documento.SchedaDocumento getDettaglioMobile(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string docNumber)
        {
            DocsPaDB.Query_DocsPAWS.Mobile.DocumentiMobile doc = new DocsPaDB.Query_DocsPAWS.Mobile.DocumentiMobile();
            DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
            schedaDoc = doc.GetDettaglioMobile(infoUtente, idProfile, docNumber, true);

            if (schedaDoc == null)
            {
                throw new Exception();
            }
            else
            {
                // Reperimento informazioni se il documento è in stato checkout,
                // solo per i documenti non di tipo stampa registro
                schedaDoc.checkOutStatus = BusinessLogic.CheckInOut.CheckInOutServices.GetCheckOutStatus(schedaDoc.systemId, schedaDoc.docNumber, infoUtente);
            }

            try
            {
                DocsPaDocumentale.Documentale.DocumentManager docManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                DocsPaVO.documento.Documento docPrinc = (schedaDoc.documenti[0] as DocsPaVO.documento.Documento);
                string ext = docManager.GetFileExtension(schedaDoc.docNumber, docPrinc.version);
                DocsPaVO.documento.Applicazione app = new DocsPaVO.documento.Applicazione();
                app.estensione = ext;
                docPrinc.applicazione = app;
                schedaDoc.documenti[0] = docPrinc;
            }
            catch (Exception)
            {
            }

            return schedaDoc;
        }
    }
}
