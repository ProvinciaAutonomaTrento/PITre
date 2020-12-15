using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.ProfilazioneDinamica;
using BusinessLogic.Interoperabilità;
using DocsPaWS.Sanita.VO.Requests;
using BusinessLogic.ProfilazioneDinamica;
using DocsPaVO.documento;
using System.Xml;
using DocsPaVO.filtri;
using log4net;

namespace DocsPaWS.Sanita.Interoperabilita
{
    public class InteropSchedaDocHandler : IInteropSchedaDocHandler
    {
        private static ILog logger = LogManager.GetLogger(typeof(InteropSchedaDocHandler));
        Templates _template;
        MailProcessor _processor;

        public InteropSchedaDocHandler(InteropCreazioneDocRequest request)
        {
            try
            {
                _template = ProfilazioneDocumenti.getTemplateById(request.StatusTemplate.IdTemplate);
                logger.Debug("template " + _template.DESCRIZIONE + " trovato");
                OggettoCustom oc = _template.ELENCO_OGGETTI.Cast<OggettoCustom>().Single(e => request.StatusTemplate.StatusTemplateFieldName.Equals(e.DESCRIZIONE));
                logger.Debug("Oggetto custom trovato " + oc.DESCRIZIONE);
                oc.VALORE_DATABASE = request.StatusTemplate.StatusNotProcessedValue;
                _processor = new MailProcessor(request.FiltriMail);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella costruzione del template: " + e.Message);
            }
        }

        public void CustomizeSchedaDocNoSegnatura(SchedaDocumento schedaDoc, CMMsg mail,string filePath)
        {
            HandleSchedaDoc(schedaDoc, mail);
        }

        public void CustomizeSchedaDocSegnatura(SchedaDocumento schedaDoc, CMMsg mail, string filePath)
        {
            HandleSchedaDoc(schedaDoc, mail);
        }

        private void HandleSchedaDoc(SchedaDocumento schedaDoc, CMMsg mail)
        {
            if (_template != null)
            {
                bool validMail = _processor.IsValidMail(mail);
                if (!validMail)
                {
                    logger.Debug("Mail non valida");
                    return;
                }
                logger.Debug("Aggiunta template " + _template.DESCRIZIONE);
                schedaDoc.template = _template;
                schedaDoc.tipologiaAtto = new TipologiaAtto();
                schedaDoc.tipologiaAtto.descrizione = _template.DESCRIZIONE;
                schedaDoc.tipologiaAtto.systemId = _template.SYSTEM_ID.ToString();
            }
            else
            {
                logger.Debug("Template nullo");
            }
        }

    }
}
