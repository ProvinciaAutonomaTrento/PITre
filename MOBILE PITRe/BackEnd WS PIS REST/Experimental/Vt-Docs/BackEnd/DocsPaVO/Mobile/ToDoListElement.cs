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

        public string AccessRights { get; set; }
        public static ToDoListElement buildInstance(infoToDoList input, Trasmissione trasm, UserInfo userInfo)
        {
            ToDoListElement res = new ToDoListElement();
            res.Oggetto = input.oggetto;
            res.DataDoc = input.dataInvio;
            res.Note = input.noteGenerali;
            res.Extension = input.cha_img;
            // modifica per vedere il mittente delegato in todolist
            if (!string.IsNullOrEmpty(input.ut_delegato))
                res.Mittente = input.ut_delegato + " delegato da " + input.utenteMittente;
            else
                res.Mittente = input.utenteMittente;
            res.Ragione = getLabelTypeEvent(input.ragione);
            if (string.IsNullOrWhiteSpace(res.Ragione))
                res.Ragione =input.ragione;
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
            if (string.IsNullOrWhiteSpace(res.Ragione))
                res.Ragione = input.TYPE_EVENT;
            res.Extension = (!string.IsNullOrEmpty(input.EXTENSION) ? input.EXTENSION : "-");
            res.Oggetto = input.ITEMS.ITEM3;
            if (input.TYPE_EVENT.ToUpper().Contains("TRASM_"))
            {
                res.IdTrasm = idTrasmissione;
               
            }
            else
                res.IdTrasm = string.Empty;
            res.IdEvento = input.ID_EVENT;
            res.AccessRights = input.ACCESSRIGHTS;
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

        public static ToDoListElement buildInstance(Notification.Notification input, string idTrasmissione, string tipoRagione)
        {
            ToDoListElement res = new ToDoListElement();
            res.Mittente = input.PRODUCER;
            res.DataDoc = input.DTA_EVENT.ToShortDateString();
            res.Ragione = getLabelTypeEvent(input.TYPE_EVENT);
            if (string.IsNullOrWhiteSpace(res.Ragione))
                res.Ragione = input.TYPE_EVENT;
            res.Extension = (!string.IsNullOrEmpty(input.EXTENSION) ? input.EXTENSION : "-");
            res.Oggetto = input.ITEMS.ITEM3;
            if (input.TYPE_EVENT.ToUpper().Contains("TRASM_"))
            {
                res.IdTrasm = idTrasmissione;
                if (!string.IsNullOrEmpty(tipoRagione) && tipoRagione.ToUpper() == "W")
                    res.HasWorkflow = true;
            }
            else
                res.IdTrasm = string.Empty;
            res.IdEvento = input.ID_EVENT;
            res.AccessRights = input.ACCESSRIGHTS;
            if (!string.IsNullOrEmpty(input.ITEMS.ITEM2))
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
                if (input.ITEMS != null && !string.IsNullOrWhiteSpace(input.ITEMS.ITEM1))
                    res.Segnatura = input.ITEMS.ITEM1;
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
            if (eventTypeExtended.ToUpper().StartsWith("TRASM_"))
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
            string result = "";
            if (!string.IsNullOrEmpty(eventTypeExtended))
            {
                eventTypeExtended = eventTypeExtended.ToUpper().Replace('_', ' ').Trim();
                
                switch (eventTypeExtended)
                {
                    case "MODIFIED OBJECT PROTO":
                        result = "Mod. oggetto protocollo";
                        break;
                    case "MODIFIED OBJECT DOC":
                        result = "Modificato oggetto del Documento";
                        break;
                    case "CHECK TRASM FOLDER":
                        result = "Visto Trasmissione F.";
                        break;
                    case "CHECK TRASM DOCUMENT":
                        result = "Visto Trasmissione D.";
                        break;
                    case "ACCEPT TRASM FOLDER":
                        result = "Accettazione Trasmissione F.";
                        break;
                    case "ACCEPT TRASM DOCUMENT":
                        result = "Accettazione Trasmissione D.";
                        break;
                    case "REJECT TRASM FOLDER":
                        result = "Rifiuto Trasmissione F.";
                        break;
                    case "REJECT TRASM DOCUMENT":
                        result = "Rifiuto Trasmissione D.";
                        break;
                    case "DOC CAMBIO STATO":
                        result = "Cambio stato documento";
                        break;
                    case "DOCUMENTOCONVERSIONEPDF":
                        result = "Conversione PDF";
                        break;
                    case "ANNULLA PROTO":
                        result = "Annullamento protocollo";
                        break;
                    case "RECEIPT EXCEPTION SIMPLIFIED INTEROPERABILITY":
                        result = "Notifica di eccezione IS";
                        break;
                    case "NO DELIVERY SEND PEC":
                        result = "Ricezione mancata consegna/con errori.";
                        break;
                    case "NO DELIVERY SEND SIMPLIFIED INTEROPERABILITY":
                        result = "Ricevuta di mancata consegna IS";
                        break;
                    case "EXCEPTION INTEROPERABILITY PEC":
                        result = "Eccezione interoperabilità PEC";
                        break;
                    case "RECORD PREDISPOSED":
                        result = "Protocollazione predisposto";
                        break;
                    case "EXCEPTION SEND SIMPLIFIED INTEROPERABILITY":
                        result = "Ricevuta di eccezione IS";
                        break;
                    case "DOC SIGNATURE":
                        result = "Documento firmato";
                        break;
                    case "CLOSE TASK DOCUMENT":
                        result = "Chiusura Task D.";
                        break;
                    case "CLOSE TASK FOLDER":
                        result = "Chiusura Task F.";
                        break;
                    case "CANCEL TASK DOCUMENT":
                        result = "Annullamento Task D.";
                        break;
                    case "CANCEL TASK FOLDER":
                        result = "Annullamento Task F.";
                        break;
                    case "INTERROTTO PROCESSO DOCUMENTO DAL TITOLARE":
                        result = "Interrotto processo dal titolare Doc";
                        break;
                    case "INTERROTTO PROCESSO ALLEGATO DAL TITOLARE":
                        result = "Interrotto processo dal titolare All";
                        break;
                    case "INTERROTTO PROCESSO ALLEGATO DAL PROPONENTE":
                        result = "Interrotto processo dal proponente All";
                        break;
                    case "INTERROTTO PROCESSO DOCUMENTO DAL PROPONENTE":
                        result = "Interrotto processo dal proponente	Doc";
                        break;
                    case "CONCLUSIONE PROCESSO LF ALLEGATO":
                        result = "Conclusione processo All";
                        break;
                    case "CONCLUSIONE PROCESSO LF DOCUMENTO":
                        result = "Conclusione processo Doc";
                        break;
                    case "TRONCAMENTO PROCESSO":
                        result = "Troncamento del processo";
                        break;
                    case "INTERROTTO PROCESSO DOCUMENTO DA ADMIN":
                        result = "Interrotto processo da Amministratore Doc";
                        break;
                    case "INTERROTTO PROCESSO ALLEGATO DA ADMIN":
                        result = "Interrotto processo da Amministratore All";
                        break;
                    case "INTERROTTO PROCESSO":
                        result = "Interruzione del processo di firma";
                        break;
                    case "INSERIMENTO DOCUMENTO LF":
                        result = "Inserimento di un documento in libro firma";
                        break;
                    case "CONCLUSIONE PROCESSO LF":
                        result = "Conclusione del processo di firma";
                        break;
                    case "PROCESSO FIRMA ERRORE PASSO AUTOMATICO":
                        result = "Errore passo automatico";
                        break;
                    case "CONCLUSIONE PROCESSO AUTOMATICO LF":
                        result = "Conclusione processo automatico";
                        break;
                    case "PROCESSO FIRMA DESTINATARI NON INTEROP":
                        result = "Presenza destinatari non interoperanti";
                        break;
                }      
            }
            return result;
        }  
    }

}
