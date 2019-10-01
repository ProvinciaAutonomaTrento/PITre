using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.trasmissione;
using DocsPaVO.documento;
using DocsPaVO.fascicolazione;

namespace DocsPaVO.Mobile
{
    public class ToDoListElement
    {
        public string Oggetto
        {
            get;
            set;
        }

        public string TipoProto
        {
            get;
            set;
        }

        public String Segnatura
        {
            get;
            set;
        }

        public string DataDoc
        {
            get;
            set;
        }

        public ElementType Tipo
        {
            get;
            set;
        }

        public string Id
        {
            get;
            set;
        }

        public string IdTrasm
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        public string Mittente
        {
            get;
            set;
        }

        public string Ragione
        {
            get;
            set;
        }

        public bool HasWorkflow
        {
            get;
            set;
        }

        public string Extension
        {
            get;
            set;
        }

        public string IdEvento
        {
            get;
            set;
        }
        public static ToDoListElement buildInstance(infoToDoList input, Trasmissione trasm, UserInfo userInfo)
        {
            ToDoListElement res = new ToDoListElement();
            res.Oggetto = input.oggetto;
            res.DataDoc = input.dataInvio;
            res.Note = input.noteGenerali;
            res.Extension = input.cha_img;
            // modifica per vedere il mittente delegato in todolist
            if (!string.IsNullOrEmpty(input.ut_delegato))
                res.Mittente = input.ut_delegato + " sostituto di " + input.utenteMittente;
            else
                res.Mittente = input.utenteMittente;
            res.Ragione = input.ragione;
            res.TipoProto = input.tipoProto;
            res.Segnatura = input.infoDoc;
            if (res.Segnatura.StartsWith("IdDoc:")) res.Segnatura = null;
            if (trasm != null)
            {
                foreach (TrasmissioneSingola temp in trasm.trasmissioniSingole)
                {
                    foreach (TrasmissioneUtente tempUt in temp.trasmissioneUtente)
                    {
                        if (tempUt.utente != null)
                        {
                            if (userInfo.UserId.Equals(tempUt.utente.userId))
                            {
                                if ("W".Equals(temp.ragione.tipo)) res.HasWorkflow = true;
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(input.sysIdFasc))
            {
                res.Id = input.sysIdFasc;
                res.Tipo = ElementType.FASCICOLO;
            }
            else
            {
                res.Id = input.sysIdDoc;
                res.Tipo = ElementType.DOCUMENTO;
            }
            res.IdTrasm = input.sysIdTrasm;
            return res;
        }

        public static ToDoListElement buildInstance(InfoDocumento input)
        {
            ToDoListElement res = new ToDoListElement();
            res.Oggetto = input.oggetto;
            res.DataDoc = input.dataApertura;
            res.Id = input.idProfile;
            res.Tipo = ElementType.DOCUMENTO;
            res.Mittente = input.mittDoc;
            res.TipoProto = input.tipoProto;
            res.Segnatura = input.segnatura;
            return res;
        }

        public static ToDoListElement buildInstance(Folder input)
        {
            ToDoListElement res = new ToDoListElement();
            res.Oggetto = input.descrizione;
            res.Id = input.systemID;
            res.Tipo = ElementType.FOLDER;
            return res;
        }

        public static ToDoListElement buildInstance(Notification.Notification input, string idTrasmissione)
        {
            ToDoListElement res = new ToDoListElement();
            res.Mittente = input.PRODUCER;
            res.DataDoc = input.DTA_EVENT.ToShortDateString();
            res.Ragione = getLabelTypeEvent(input.TYPE_EVENT);
            res.Extension = (!string.IsNullOrEmpty(input.EXTENSION) ? input.EXTENSION : "-");
            res.Oggetto = input.ITEMS.ITEM3;
            if (input.TYPE_EVENT.ToUpper().Contains("TRASM_"))
                res.IdTrasm = idTrasmissione;
            else
                res.IdTrasm = string.Empty;
            res.IdEvento = input.ID_EVENT;
            if(!string.IsNullOrEmpty(input.ITEMS.ITEM2))
            {
                res.Segnatura = input.ITEMS.ITEM2;
                string lblTipoProto = res.Segnatura.Substring(res.Segnatura.IndexOf("(") + 1, (res.Segnatura.IndexOf(")") - (res.Segnatura.IndexOf("(") + 1)));
                string tipoProto = GetTipoProto(lblTipoProto);
                res.Segnatura = res.Segnatura.Replace("(" + lblTipoProto + ")", "");
                res.TipoProto = tipoProto;
            }
            else
            {
                res.Segnatura = null;
                res.TipoProto = "G";
            }
            res.Id = input.ID_OBJECT;
            if (input.DOMAINOBJECT.Equals("DOCUMENTO"))
            {
                res.Tipo = ElementType.DOCUMENTO;
            }
            else if (input.DOMAINOBJECT.Equals("FASCICOLO"))
            {
                res.Tipo = ElementType.FASCICOLO;
            }
            return res;
        }

        private static string GetTipoProto(string tipoProto)
        {
            string result = string.Empty;
            switch (tipoProto)
            { 
                case "<label>lblPartenza</label>":
                    result = "P";
                    break;
                case "<label>lblArrivo</label>":
                    result = "A";
                    break;
                case "<label>lblInterno</label>":
                    result = "I";
                    break;
            }
            return result; 
        }

        private static string getLabelTypeEvent(string eventTypeExtended)
        {
            string result = string.Empty;
            if (eventTypeExtended.ToUpper().Contains("TRASM_"))
            {
                result = (eventTypeExtended.ToUpper().Replace("TRASM_DOC_", "T. ").Replace("TRASM_FOLDER_", "T. ")).Replace("_", " ");
                //result = Utils.Languages.GetLabelFromCode("IndexTrasmDocFolder", UIManager.UserManager.GetUserLanguage()) + eventTypeExtended;
            }
            else
            {
                result = GetTypeEvent(eventTypeExtended).ToUpper();
                //result = Utils.Languages.GetLabelFromCode(eventTypeExtended, UIManager.UserManager.GetUserLanguage());
            }
            return result;

        }

        private static string GetTypeEvent(string eventTypeExtended)
        {
            switch (eventTypeExtended)
            {
                case "MODIFIED_OBJECT_PROTO":
                    eventTypeExtended = "Mod. oggetto protocollo";
                    break;
                case "MODIFIED_OBJECT_DOC":
                    eventTypeExtended = "Modificato oggetto del Documento";
                    break;
                case "CHECK_TRASM_FOLDER":
                    eventTypeExtended = "Visto Trasmissione F.";
                    break;
                case "CHECK_TRASM_DOCUMENT":
                    eventTypeExtended = "Visto Trasmissione D.";
                    break;
                case "ACCEPT_TRASM_FOLDER":
                    eventTypeExtended = "Accettazione Trasmissione F.";
                    break;
                case "ACCEPT_TRASM_DOCUMENT":
                    eventTypeExtended = "Accettazione Trasmissione D.";
                    break;
                case "REJECT_TRASM_FOLDER":
                    eventTypeExtended = "Rifiuto Trasmissione F.";
                    break;
                case "REJECT_TRASM_DOCUMENT":
                    eventTypeExtended = "Rifiuto Trasmissione D.";
                    break;
                case "DOC_CAMBIO_STATO":
                    eventTypeExtended = "Cambio stato documento";
                    break;
                case "DOCUMENTOCONVERSIONEPDF":
                    eventTypeExtended = "Conversione PDF";
                    break;
                case "ANNULLA_PROTO":
                    eventTypeExtended = "Annullamento protocollo";
                    break;
                case "RECEIPT_EXCEPTION_SIMPLIFIED_INTEROPERABILITY":
                    eventTypeExtended = "Notifica di eccezione IS";
                    break;
                case "NO_DELIVERY_SEND_PEC":
                    eventTypeExtended = "Ricezione mancata consegna/con errori.";
                    break;
                case "EXCEPTION_INTEROPERABILITY_PEC":
                    eventTypeExtended = "Eccezione interoperabilità PEC";
                    break;
                case "RECORD_PREDISPOSED":
                    eventTypeExtended = "Protocollazione predisposto";
                    break;
                case "EXCEPTION_SEND_SIMPLIFIED_INTEROPERABILITY":
                    eventTypeExtended = "Ricevuta di eccezione IS";
                    break;
                case "DOC_SIGNATURE":
                    eventTypeExtended = "Documento firmato";
                    break;
                case "CLOSE_TASK_DOCUMENT":
                    eventTypeExtended = "Chiusura Task D.";
                    break;
                case "CLOSE_TASK_FOLDER":
                    eventTypeExtended = "Chiusura Task F.";
                    break;
                case "CANCEL_TASK_DOCUMENT":
                    eventTypeExtended = "Annullamento Task D.";
                    break;
                case "CANCEL_TASK_FOLDER":
                    eventTypeExtended = "Annullamento Task F.";
                    break;
                case "INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE":
                    eventTypeExtended = "Interrotto processo dal titolare Doc";
                    break;
                case "INTERROTTO_PROCESSO_ALLEGATO_DAL_TITOLARE":
                    eventTypeExtended = "Interrotto processo dal titolare All";
                    break;
                case "INTERROTTO_PROCESSO_ALLEGATO_DAL_PROPONENTE":
                    eventTypeExtended = "Interrotto processo dal proponente All";
                    break;
                case "INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE":
                    eventTypeExtended = "Interrotto processo dal proponente	Doc";
                    break;
                case "CONCLUSIONE_PROCESSO_LF_ALLEGATO":
                    eventTypeExtended = "Conclusione processo All";
                    break;
                case "CONCLUSIONE_PROCESSO_LF_DOCUMENTO":
                    eventTypeExtended = "Conclusione processo Doc";
                    break;
                case "TRONCAMENTO_PROCESSO":
                    eventTypeExtended = "Anomalia nel processo";
                    break;
                case "INTERROTTO_PROCESSO_DOCUMENTO_DA_ADMIN":
                    eventTypeExtended = "Interrotto processo da Amministratore Doc";
                    break;
                case "INTERROTTO_PROCESSO_ALLEGATO_DA_ADMIN":
                    eventTypeExtended = "Interrotto processo da Amministratore All";
                    break;
                case "INTERROTTO_PROCESSO":
                    eventTypeExtended = "Interruzione del processo di firma";
                    break;
                case "INSERIMENTO_DOCUMENTO_LF":
                    eventTypeExtended = "Inserimento di un documento in libro firma";
                    break;
                case "CONCLUSIONE_PROCESSO_LF":
                    eventTypeExtended = "Conclusione del processo di firma";
                    break;
            }      
            return eventTypeExtended;
        }  
    }

}
