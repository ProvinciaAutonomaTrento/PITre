using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BusinessLogic.Modelli.AsposeModelProcessor;
using log4net;

namespace BusinessLogic.Modelli
{

    public enum TipoRicevuta
    {
        PresaVisione
    }

    public class StampaRicevutaGenerica
    {
        private static ILog logger = LogManager.GetLogger(typeof(StampaRicevutaGenerica));

        public static DocsPaVO.documento.FileDocumento Create(DocsPaVO.utente.InfoUtente userInfo, string idDocument, TipoRicevuta tipo)
        {
            DocsPaVO.documento.FileDocumento fdoc = new DocsPaVO.documento.FileDocumento();

            switch (tipo)
            {
                case TipoRicevuta.PresaVisione:
                    fdoc = CreaRicevutaPresaVisione(userInfo, idDocument);
                    break;
                default:
                    fdoc = null;
                    break;
            }

            return fdoc;
        }

        #region Metodi privati

        private static DocsPaVO.documento.FileDocumento CreaRicevutaPresaVisione(DocsPaVO.utente.InfoUtente userInfo, string idDocument)
        {
            DocsPaVO.documento.FileDocumento fdoc = new DocsPaVO.documento.FileDocumento();

            try
            {
                DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(userInfo, idDocument);
                DocsPaVO.documento.Documento doc = (DocsPaVO.documento.Documento)schedaDoc.documenti[0];
                DocsPaVO.Procedimento.Procedimento procedimento = BusinessLogic.Procedimenti.ProcedimentiManager.GetProcedimentoByIdDoc(idDocument);
                DocsPaVO.utente.Corrispondente corr = BusinessLogic.Utenti.UserManager.getCorrispondenteBySystemID(procedimento.Autore);

                string text = string.Format("L'utente {0}, in data {1}, ha preso visione del documento {2} sul Portale dei procedimenti amministrativi.\r\nHash del file: {3}", 
                                            corr.descrizione,
                                            procedimento.Documenti[0].DataVisualizzazione,
                                            schedaDoc.protocollo.segnatura,
                                            GetImpronta(doc)
                                           );
                
                PdfModelProcessor processor = new PdfModelProcessor();
                fdoc = processor.CreaRicevuta(userInfo, idDocument, text);
                fdoc.name = "Ricevuta di presa visione.pdf";
                fdoc.fullName = fdoc.name;
                fdoc.nomeOriginale = fdoc.name;
                fdoc.contentType = "application/pdf";
                fdoc.estensioneFile = "pdf";
            }
            catch (Exception ex)
            {
                fdoc = null;
                logger.Error("Errore in CreaRicevutaPresaVisione", ex);
            }

            return fdoc;
        }

        #endregion

        #region Metodi di utilità
        private static string GetImpronta(DocsPaVO.documento.Documento doc)
        {
            string hash = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.Documenti docs = new DocsPaDB.Query_DocsPAWS.Documenti();
                docs.GetImpronta(out hash, doc.versionId, doc.docNumber);
            }
            catch (Exception ex)
            {
                hash = "non disponibile";
            }
            return hash;
        }
        #endregion
    }

    
}
