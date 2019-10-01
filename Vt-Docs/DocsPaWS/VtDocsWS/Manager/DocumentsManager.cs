using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Collections;
using DocsPaVO.utente;
using DocsPaVO.documento;
using DocsPaVO.filtri;
using DocsPaVO.amministrazione;
using DocsPaVO.ricerche;
using VtDocsWS.WebServices;
using System.Web.Hosting;
using System.Data;
using DocsPaVO.Spedizione;
using log4net;


namespace VtDocsWS.Manager
{
    public class DocumentsManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocumentsManager));

        public static Services.Documents.AddDocInProject.AddDocInProjectResponse AddDocInProject(Services.Documents.AddDocInProject.AddDocInProjectRequest request)
        {
            Services.Documents.AddDocInProject.AddDocInProjectResponse response = new Services.Documents.AddDocInProject.AddDocInProjectResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "AddDocInProject");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if ((string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject)) || string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new PisException("REQUIRED_CODE_AND_ID");
                }

                if (!string.IsNullOrEmpty(request.CodeProject) && !string.IsNullOrEmpty(request.IdProject) && !string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new PisException("REQUIRED_ONLY_CODE_OR_ID_PROJECT");
                }

                string msg = string.Empty;
                DocsPaVO.documento.InfoDocumento infoDoc = null;
                ArrayList fascicoli = new ArrayList();
                DocsPaVO.fascicolazione.Fascicolo fascicolo = null;

                //Controllo se esiste il documento
                try
                {

                    infoDoc = BusinessLogic.Documenti.DocManager.GetInfoDocumento(infoUtente, request.IdDocument, request.IdDocument);
                }
                catch
                {
                    //Documento non trovato
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                if (infoDoc != null)
                {
                    //Controllo se esiste il fascicolo
                    try
                    {
                        if (!string.IsNullOrEmpty(request.IdProject))
                        {
                            fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(request.IdProject, infoUtente);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(request.CodeProject))
                            {
                                //Il codice è di un sottofascicolo
                                if (request.CodeProject.IndexOf("//") > -1)
                                {
                                    string separatore = "//";
                                    // MODIFICA per inserimento in sottocartelle
                                    string[] separatoreAr = new string[] { separatore };

                                    string[] pathCartelle = request.CodeProject.Split(separatoreAr, StringSplitOptions.RemoveEmptyEntries);

                                    fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloDaCodice(infoUtente, pathCartelle[0], null, false, false);

                                    if (pathCartelle.Length > 1)
                                    {
                                        ArrayList cartelle = BusinessLogic.Fascicoli.FascicoloManager.getListaFolderDaIdFascicolo(infoUtente, fascicolo.systemID, null, false, false);
                                        // caricamento folder base
                                        string idParentFolder = fascicolo.systemID;
                                        fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)((from DocsPaVO.fascicolazione.Folder f in cartelle where f.idParent == idParentFolder select f).ToList().FirstOrDefault());
                                        idParentFolder = fascicolo.folderSelezionato.systemID;
                                        try
                                        {
                                            for (int i = 1; i < pathCartelle.Length; i++)
                                            {

                                                fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)((from DocsPaVO.fascicolazione.Folder f in cartelle where f.idParent == idParentFolder && f.descrizione == pathCartelle[i] select f).ToList().First());
                                                idParentFolder = fascicolo.folderSelezionato.systemID;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception("Cartella non trovata nel fascicolo");
                                        }
                                    }

                                    //int posSep = request.CodeProject.IndexOf("//");
                                    //string codiceFascicolo = request.CodeProject.Substring(0, posSep);
                                    //string descrFolder = request.CodeProject.Substring(posSep + separatore.Length);
                                    //fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFolderDaCodiceFascicolo(infoUtente, codiceFascicolo, descrFolder, null, false, false);
                                    //if (fascicoli != null && fascicoli.Count > 0)
                                    //{
                                    //    if (fascicoli.Count == 1)
                                    //    {
                                    //        //calcolo fascicolo sottofascicolo
                                    //        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(((DocsPaVO.fascicolazione.Folder)fascicoli[0]).idFascicolo, infoUtente);
                                    //        if (fascicolo != null)
                                    //        {
                                    //            //folder selezionato è l'ultimo
                                    //            fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)fascicoli[fascicoli.Count - 1];
                                    //        }
                                    //    }
                                    //    else
                                    //    {
                                    //        //Fascicoli multipli
                                    //        throw new PisException("MULTIPLE");
                                    //    }
                                    //}
                                }
                                else
                                {
                                    fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoliDaCodice(infoUtente, request.CodeProject, null, false, false, "I");
                                    if (fascicoli != null && fascicoli.Count > 0)
                                    {
                                        if (fascicoli.Count == 1)
                                        {
                                            fascicolo = (DocsPaVO.fascicolazione.Fascicolo)fascicoli[0];
                                        }
                                        else
                                        {
                                            //Fascicoli multipli
                                            throw new PisException("MULTIPLE");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //Fascicolo non trovato
                                throw new PisException("PROJECT_NOT_FOUND");
                            }
                        }
                    }
                    catch (PisException pisEx1)
                    {
                        //Fascicolo non trovato
                        throw new PisException("PROJECT_NOT_FOUND");
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }

                    if (fascicolo != null)
                    {
                        //Caso del sottofascicolo
                        if (fascicolo.folderSelezionato != null)
                        {
                            BusinessLogic.Fascicoli.FolderManager.addDocFolder(infoUtente, request.IdDocument, fascicolo.folderSelezionato.systemID, false, out msg);
                        }
                        else
                        {
                            BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente, request.IdDocument, fascicolo.systemID, false, out msg);
                        }
                    }
                    else
                    {
                        //Fascicolo non trovato
                        throw new PisException("PROJECT_NOT_FOUND");
                    }
                }
                else
                {
                    //Documento non trovato
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.CreateDocument.CreateDocumentResponse CreateDocument(Services.Documents.CreateDocument.CreateDocumentRequest request, bool noClassificationRequired)
        {
            Services.Documents.CreateDocument.CreateDocumentResponse response = new Services.Documents.CreateDocument.CreateDocumentResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "CreateDocument");
                bool setStatoDiagrammaIniziale = false;
                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                DocsPaVO.DiagrammaStato.Stato statoIniziale = null;

                if (!noClassificationRequired)
                {
                    //Se è attiva la fascicolazione obbligatoria non è possibile utilizzare questo metodo
                    string fascRapidaRequired = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "FE_FASC_RAPIDA_REQUIRED");
                    if (!string.IsNullOrEmpty(fascRapidaRequired) && fascRapidaRequired.Equals("TRUE"))
                        throw new PisException("FASC_RAPIDA_REQUIRED");
                }

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (request != null && string.IsNullOrEmpty(request.Document.DocumentType))
                {
                    throw new PisException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && string.IsNullOrEmpty(request.Document.Object))
                {
                    throw new PisException("MISSING_OBJECT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (!request.Document.DocumentType.ToUpper().Equals("A") && !request.Document.DocumentType.ToUpper().Equals("P") && !request.Document.DocumentType.ToUpper().Equals("I") && !request.Document.DocumentType.ToUpper().Equals("G")))
                {
                    throw new PisException("MISSING_TYPE_DOCUMENT");
                }

                if (request != null && !string.IsNullOrEmpty(request.Document.DocumentType) && (request.Document.DocumentType.ToUpper().Equals("A") || request.Document.DocumentType.ToUpper().Equals("P") || request.Document.DocumentType.ToUpper().Equals("I")) && string.IsNullOrEmpty(request.CodeRegister))
                {
                    throw new PisException("REQUIRED_REGISTER");
                }

                if (request != null && request.Document != null && request.Document.MainDocument != null && request.Document.MainDocument.Content != null && request.Document.MainDocument.Content.Length > 0)
                {
                    int maxfilesize = Utils.maxFileSizePermitted();
                    if (maxfilesize > 0 && request.Document.MainDocument.Content.Length > maxfilesize)
                    {
                        throw new Exception("Dimensione del file troppo grande");
                    }
                }

                //Reperimento ruolo utente
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                DocsPaVO.utente.Corrispondente mittente = new Corrispondente();

                try
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                    if (ruolo == null)
                    {
                        //Ruolo non trovato
                        throw new PisException("ROLE_NO_EXIST");
                    }
                }
                catch
                {
                    //Ruolo non trovato
                    throw new PisException("ROLE_NO_EXIST");
                }


                // **************************************************************************
                // Per accordi con il cliente viene effettuato questo controllo:
                // se Type == "O" allore viene impostato il valore di CorrespondentType = "O"
                // Controllo effettuato per: Sender, Recipients, RecipientsCC
                // **************************************************************************
                //
                if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.Type) && request.Document.Sender.Type.Equals("O"))
                    request.Document.Sender.CorrespondentType = "O";

                if (request.Document.MultipleSenders != null)
                {
                    for (int i = 0; i < request.Document.MultipleSenders.Length; i++)
                    {
                        Domain.Correspondent c = request.Document.MultipleSenders[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }

                if (request.Document.Recipients != null)
                {
                    for (int i = 0; i < request.Document.Recipients.Length; i++)
                    {
                        Domain.Correspondent c = request.Document.Recipients[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }

                if (request.Document.RecipientsCC != null)
                {
                    for (int i = 0; i < request.Document.RecipientsCC.Length; i++)
                    {
                        Domain.Correspondent c = request.Document.RecipientsCC[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }
                //


                ///Creazione schedaDocumento con i dati essenziali e obbligatori
                DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.NewSchedaDocumento(infoUtente, false);

                ///Imposto l'oggetto
                if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.Object))
                {
                    schedaDoc.oggetto.descrizione = request.Document.Object;
                    schedaDoc.oggetto.daAggiornare = true;
                }

                //Imposto le note, soltanto visibili a tutti
                if (request != null && request.Document != null && request.Document != null && request.Document.Note != null && request.Document.Note.Length > 0)
                {
                    schedaDoc.noteDocumento = new List<DocsPaVO.Note.InfoNota>();
                    foreach (Domain.Note nota in request.Document.Note)
                    {
                        DocsPaVO.Note.InfoNota tempNot = new DocsPaVO.Note.InfoNota();
                        tempNot.DaInserire = true;
                        tempNot.Testo = nota.Description;
                        tempNot.TipoVisibilita = DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti;
                        tempNot.UtenteCreatore = new DocsPaVO.Note.InfoUtenteCreatoreNota();
                        tempNot.UtenteCreatore.IdRuolo = infoUtente.idGruppo;
                        tempNot.UtenteCreatore.IdUtente = infoUtente.idPeople;
                        schedaDoc.noteDocumento.Add(tempNot);
                    }
                }
                //

                ///Imposto il template
                if (request != null && request.Document != null && request.Document.Template != null && (!string.IsNullOrEmpty(request.Document.Template.Id) || (!string.IsNullOrEmpty(request.Document.Template.Name))))
                {
                    DocsPaVO.ProfilazioneDinamica.Templates template = null;
                    if (!string.IsNullOrEmpty(request.Document.Template.Id))
                    {
                        template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(request.Document.Template.Id);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.Document.Template.Name))
                        {
                            template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(request.Document.Template.Name, infoUtente.idAmministrazione);
                        }
                    }

                    if (template != null)
                    {
                        //Modifica Lembo 10-01-2013: Controllo se l'utente è abilitato all'utilizzo del template.
                        //Solo ruoli con diritti di scrittura ammessi.
                        string controlFilter = string.Format("{0} and id_ruolo={1} and (diritti='2')", template.SYSTEM_ID.ToString(), infoUtente.idGruppo);
                        ArrayList controlloVisibilita = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getRuoliTipoDoc(controlFilter);
                        if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                        {
                            throw new PisException("TEMPLATE_NOT_ROLE_EDITABLE");
                        }
                        // Modifica Lembo 11-01-2013: Utilizzo il metodo che controlla la visibilità dei campi.
                        //schedaDoc.template = Utils.GetTemplateFromPis(request.Document.Template, template, false);

                        // Modifica Elaborazione XML da PIS req.2

                        Domain.File fileDaPassare = ((request.Document.MainDocument != null && request.Document.MainDocument.Content != null) ? request.Document.MainDocument : null);

                        schedaDoc.template = Utils.GetTemplateFromPisVisibility(request.Document.Template, template, false, infoUtente.idGruppo, "D", request.CodeApplication, infoUtente, fileDaPassare, request.CodeRegister, request.CodeRF);
                        schedaDoc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                        schedaDoc.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                        schedaDoc.tipologiaAtto.descrizione = template.DESCRIZIONE.ToString();
                        int idDiagramma = 0;
                        idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(schedaDoc.template.SYSTEM_ID.ToString());
                        if (idDiagramma != 0)
                        {
                            setStatoDiagrammaIniziale = true;
                            diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                            if (diagramma != null)
                            {
                                if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                                {
                                    foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                    {
                                        if (stato.STATO_INIZIALE == true) statoIniziale = stato;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //Template non trovato
                        throw new PisException("TEMPLATE_NOT_FOUND");
                    }
                }
                ///

                ///Se privato
                if (request != null && request.Document != null && request.Document.PrivateDocument)
                {
                    schedaDoc.privato = "1";
                }
                ///

                ///Se personale
                if (request != null && request.Document != null && request.Document.PersonalDocument)
                {
                    schedaDoc.personale = "1";
                }
                ///

                DocsPaVO.documento.SchedaDocumento schedaDocResult = null;
                DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione = new DocsPaVO.documento.ResultProtocollazione();
                bool daAggiornareUffRef = false;

                ///CASO DOCUMENTO GRIGIO (NON PROTOCOLLATO)
                if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("G"))
                {
                    schedaDoc.tipoProto = "G";
                    schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                }
                ///

                //Controlli solo per protocolli
                if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && !request.Document.DocumentType.ToUpper().Equals("G"))
                {

                    if (request.Document.Sender == null || string.IsNullOrEmpty(request.Document.Sender.CorrespondentType))
                    {
                        if (!request.Document.DocumentType.ToUpper().Equals("P"))
                        {
                            //Mittente non presente
                            throw new PisException("REQUIRED_SENDER");
                        }
                        else
                        {
                            mittente = Utils.RisolviCorrispondente(ruolo.uo.systemId, infoUtente);
                        }
                    }
                    else
                    {
                        if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.CorrespondentType) && request.Document.Sender.CorrespondentType.Equals("O"))
                        {
                            mittente = Utils.GetCorrespondentFromPis(request.Document.Sender, infoUtente);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(request.Document.Sender.Id))
                            {
                                mittente = Utils.RisolviCorrispondente(request.Document.Sender.Id, infoUtente);
                                if (mittente == null)
                                    //Mittente non trovato
                                    throw new PisException("SENDER_NOT_FOUND");
                            }
                        }
                    }

                    if (request.Document.DocumentType.ToUpper().Equals("I") || request.Document.DocumentType.ToUpper().Equals("P"))
                    {
                        if (request.Document.Recipients == null || request.Document.Recipients.Length == 0)
                        {
                            //Destinatario non presente
                            throw new PisException("REQUIRED_RECIPIENT");
                        }
                    }

                    if (string.IsNullOrEmpty(request.CodeRegister))
                    {
                        //Registro mancante
                        throw new PisException("REQUIRED_REGISTER");
                    }
                    else
                    {
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRegister);
                        if (reg == null)
                        {
                            //Registro mancante
                            throw new PisException("REGISTER_NOT_FOUND");
                        }
                        else
                        {
                            schedaDoc.registro = reg;
                        }
                    }

                    if (!string.IsNullOrEmpty(request.CodeRF))
                    {
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.CodeRF);
                        if (reg != null)
                        {
                            schedaDoc.id_rf_prot = reg.systemId;
                            schedaDoc.id_rf_invio_ricevuta = reg.systemId;
                            schedaDoc.cod_rf_prot = reg.codRegistro;
                        }
                        else
                        {
                            //RF non trovato
                            throw new PisException("RF_NOT_FOUND");
                        }
                    }
                }
                ArrayList listaIdDestinatari = new ArrayList();
                ///CASO PROTOCOLLO IN ARRIVO (ANCHE PREDISPOSTO)
                if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("A"))
                {
                    schedaDoc.tipoProto = "A";
                    schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloEntrata();

                    ((ProtocolloEntrata)schedaDoc.protocollo).mittente = mittente;

                    if (request.Document.MultipleSenders != null && request.Document.MultipleSenders.Length > 0)
                    {
                        ((ProtocolloEntrata)schedaDoc.protocollo).mittenti = new ArrayList();
                        foreach (Domain.Correspondent corrTemp in request.Document.MultipleSenders)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;
                            if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                            {
                                corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(corrTemp.Id))
                                {
                                    corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                    if (corrBySys == null)
                                    {
                                        //Mittente non trovato
                                        throw new PisException("SENDER_NOT_FOUND");
                                    }
                                }
                            }
                            ((ProtocolloEntrata)schedaDoc.protocollo).mittenti.Add(corrBySys);
                        }
                    }

                    //Data protocollo mittente
                    if (!string.IsNullOrEmpty(request.Document.DataProtocolSender))
                    {
                        ((ProtocolloEntrata)schedaDoc.protocollo).dataProtocolloMittente = request.Document.DataProtocolSender;
                    }

                    //Protocollo mittente
                    if (!string.IsNullOrEmpty(request.Document.ProtocolSender))
                    {
                        ((ProtocolloEntrata)schedaDoc.protocollo).descrizioneProtocolloMittente = request.Document.ProtocolSender;
                    }

                    //Data arrivo TODO
                    if (!string.IsNullOrEmpty(request.Document.ArrivalDate))
                    {
                        // Variabile Booleana per il controllo di validità della data passata in input;
                        bool valid = false;
                        // Data ritornata in output dal TryParseExact per la data arrivo passata in input
                        DateTime dateVal;

                        // Pattern di validità per una data valida
                        string pattern = "dd/MM/yyyy HH:mm:ss";
                        string pattern2 = "dd/MM/yyyy HH:mm";

                        try
                        {
                            /*
                             *  Date Format Example: 01/03/2012
                             *  dd = day (01)
                             *  MM = month (03)
                             *  yyyy = year (2012)
                             *  / = date separator
                             */

                            if (!DateTime.TryParseExact(request.Document.ArrivalDate, pattern, null, System.Globalization.DateTimeStyles.None, out dateVal))
                            {
                                if (!DateTime.TryParseExact(request.Document.ArrivalDate, pattern2, null, System.Globalization.DateTimeStyles.None, out dateVal))
                                {
                                    throw new Exception("Formato Data non corretto");
                                }
                            }

                            valid = true;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Formato Data non corretto");
                        }

                        if (valid)
                        {
                            // Controllo che la data inserita non sia posteriore alla data odierna
                            DateTime dateVal2;
                            // Variabile Booleana per il controllo di validità della data DateTime.Now trasformato nel formato dd/MM/yyyy;
                            bool dateNow = true;

                            if (!DateTime.TryParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), pattern, null, System.Globalization.DateTimeStyles.None, out dateVal2))
                            {
                                // Non dovrebbe entrare mai in questo controllo.
                                dateNow = false;
                                throw new Exception("Formato Data non corretto");
                            }

                            if (dateNow)
                            {
                                // dateVal: data passata in input;
                                // dateVal2: DateTime.Now convertito nel formato dd/MM/yyyy
                                int result = DateTime.Compare(dateVal, dateVal2);

                                if (result > 0)
                                    // Data di input è successiva alla data odierna;
                                    //((Documento)schedaDoc.documenti[0]).dataArrivo = string.Empty;
                                    throw new Exception("La data di arrivo non può essere posteriore alla data di segnatura");
                                else
                                    // Data di input valida e conforme;
                                    ((Documento)schedaDoc.documenti[0]).dataArrivo = request.Document.ArrivalDate;
                            }
                        }
                    }

                    if (request.Document.Predisposed)
                    {
                        schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                        if (schedaDocResult != null)
                        {
                            schedaDocResult.predisponiProtocollazione = true;
                            schedaDocResult.tipoProto = request.Document.DocumentType;
                            schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                        }
                    }
                    else
                    {
                        schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                    }

                    if (schedaDocResult != null)
                    {
                        //Mezzo di spedizione
                        if (!string.IsNullOrEmpty(request.Document.MeansOfSending))
                        {
                            ArrayList mezziSpedizione = BusinessLogic.Amministrazione.MezziSpedizioneManager.ListaMezziSpedizione(infoUtente.idAmministrazione, true);
                            foreach (MezzoSpedizione m_spediz in mezziSpedizione)
                            {
                                if (m_spediz.Descrizione.ToUpper().Equals(request.Document.MeansOfSending.ToUpper()))
                                {
                                    BusinessLogic.Documenti.ProtoManager.collegaMezzoSpedizioneDocumento(infoUtente, m_spediz.IDSystem, schedaDocResult.systemId);
                                    break;
                                }
                            }

                        }
                    }
                }
                ///

                ///CASO PROTOCOLLO IN PARTENZA (ANCHE PREDISPOSTO)
                if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("P"))
                {
                    schedaDoc.tipoProto = "P";
                    schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloUscita();

                    ((ProtocolloUscita)schedaDoc.protocollo).mittente = mittente;

                    if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                    {
                        ((ProtocolloUscita)schedaDoc.protocollo).destinatari = new ArrayList();
                        foreach (Domain.Correspondent corrTemp in request.Document.Recipients)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;

                            // Verifica se occasionale
                            if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                            {
                                corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                            }
                            else
                            {
                                // Se non è occasionale, lo risolve sulla rubrica
                                corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }

                            }
                            if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                {
                                    listaIdDestinatari.Add(c2.systemId);
                                    ((ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(c2);
                                }
                            }
                            else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                {
                                    listaIdDestinatari.Add(c3.systemId);                                    
                                    ((ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(c3);
                                }
                            }
                            else
                            {
                                listaIdDestinatari.Add(corrBySys.systemId);
                                ((ProtocolloUscita)schedaDoc.protocollo).destinatari.Add(corrBySys);
                            }
                        }
                    }

                    if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                    {
                        ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                        foreach (Domain.Correspondent corrTemp in request.Document.RecipientsCC)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;

                            // Verifica se occasionale
                            if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                            {
                                corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                            }
                            else
                            {
                                corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }
                            }
                            if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                {
                                    if(listaIdDestinatari== null || listaIdDestinatari.Count <1 || !listaIdDestinatari.Contains(c2.systemId))
                                        ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(c2);
                                }
                            }
                            else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                {
                                    if (listaIdDestinatari == null || listaIdDestinatari.Count < 1 || !listaIdDestinatari.Contains(c3.systemId))
                                        ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(c3);
                                }
                            }
                            else
                            {
                                if (listaIdDestinatari == null || listaIdDestinatari.Count < 1 || !listaIdDestinatari.Contains(corrBySys.systemId))
                                    ((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                            }

                            //((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                        }
                    }

                    if (request.Document.Predisposed)
                    {
                        schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                        if (schedaDocResult != null)
                        {
                            schedaDocResult.predisponiProtocollazione = true;
                            schedaDocResult.tipoProto = request.Document.DocumentType;
                            schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                        }
                    }
                    else
                    {
                        schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                    }
                }
                ///

                ///CASO PROTOCOLLO INTERNO (ANCHE PREDISPOSTO)
                if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("I"))
                {
                    schedaDoc.tipoProto = "I";
                    schedaDoc.protocollo = new DocsPaVO.documento.ProtocolloInterno();

                    ((ProtocolloInterno)schedaDoc.protocollo).mittente = mittente;

                    if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                    {
                        ((ProtocolloInterno)schedaDoc.protocollo).destinatari = new ArrayList();
                        foreach (Domain.Correspondent corrTemp in request.Document.Recipients)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;

                            // Verifica se occasionale
                            if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                            {
                                corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                            }
                            else
                            {
                                // Se non è occasionale, lo risolve sulla rubrica
                                corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }

                            }
                            if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                {
                                    listaIdDestinatari.Add(c2.systemId);
                                    ((ProtocolloInterno)schedaDoc.protocollo).destinatari.Add(c2);
                                }
                            }
                            else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                {
                                    listaIdDestinatari.Add(c3.systemId);
                                    ((ProtocolloInterno)schedaDoc.protocollo).destinatari.Add(c3);
                                }
                            }
                            else
                            {
                                listaIdDestinatari.Add(corrBySys.systemId);
                                ((ProtocolloInterno)schedaDoc.protocollo).destinatari.Add(corrBySys);
                            }
                        }
                    }

                    if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                    {
                        ((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza = new ArrayList();
                        foreach (Domain.Correspondent corrTemp in request.Document.RecipientsCC)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;

                            // Verifica se occasionale
                            if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                            {
                                corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                            }
                            else
                            {
                                corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }
                            }
                            if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "F" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByRF = BusinessLogic.Rubrica.RF.getCorrispondentiByCodRF(corrBySys.codiceRubrica);
                                foreach (DocsPaVO.utente.Corrispondente c2 in corrByRF)
                                {
                                    if (listaIdDestinatari == null || listaIdDestinatari.Count < 1 || !listaIdDestinatari.Contains(c2.systemId))
                                        ((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.Add(c2);
                                }
                            }
                            else if (corrBySys.tipoCorrispondente != null && corrBySys.tipoCorrispondente == "L" && corrBySys.tipoIE == null)
                            {
                                ArrayList corrByLista = BusinessLogic.Rubrica.ListeDistribuzione.getCorrispondentiByCodLista(corrBySys.codiceRubrica, infoUtente.idAmministrazione, infoUtente);
                                foreach (DocsPaVO.utente.Corrispondente c3 in corrByLista)
                                {
                                    if (listaIdDestinatari == null || listaIdDestinatari.Count < 1 || !listaIdDestinatari.Contains(c3.systemId))
                                        ((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.Add(c3);
                                }
                            }
                            else
                            {
                                if (listaIdDestinatari == null || listaIdDestinatari.Count < 1 || !listaIdDestinatari.Contains(corrBySys.systemId))
                                    ((ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                            }

                            //((ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Add(corrBySys);
                        }
                    }

                    if (request.Document.Predisposed)
                    {
                        schedaDocResult = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDoc, infoUtente, ruolo);
                        if (schedaDocResult != null)
                        {
                            schedaDocResult.predisponiProtocollazione = true;
                            schedaDocResult.tipoProto = request.Document.DocumentType;
                            schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, schedaDocResult, false, out daAggiornareUffRef, ruolo);
                        }
                    }
                    else
                    {
                        schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDoc, ruolo, infoUtente, out risultatoProtocollazione);
                    }
                }
                ///

                //Se ho anche un file da aggiungere
                if (schedaDocResult != null && !string.IsNullOrEmpty(schedaDocResult.docNumber) && request.Document.MainDocument != null)
                {
                    //Crea il file o se esistente una nuova versione
                    // 5. Acquisizione del file al documento
                    DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)schedaDocResult.documenti[0];

                    DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                    {
                        name = request.Document.MainDocument.Name,
                        fullName = request.Document.MainDocument.Name,
                        content = request.Document.MainDocument.Content,
                        contentType = request.Document.MainDocument.MimeType,
                        length = request.Document.MainDocument.Content.Length,
                        bypassFileContentValidation = true
                    };

                    string errorMessage;

                    if (!BusinessLogic.Documenti.FileManager.putFile(ref versioneCorrente, fileDocumento, infoUtente, out errorMessage))
                    {
                        throw new PisException("FILE_CREATION_ERROR");
                    }


                }

                if (schedaDocResult != null)
                {
                    if (setStatoDiagrammaIniziale)
                    {
                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(schedaDocResult.systemId, statoIniziale.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);

                    }
                    response.Document = Utils.GetDetailsDocument(schedaDocResult, infoUtente, false);

                    if (setStatoDiagrammaIniziale)
                        response.Document.Template.StateDiagram.StateOfDiagram[0] = Utils.GetStateOfDiagram(statoIniziale, diagramma.SYSTEM_ID.ToString());

                    if (!string.IsNullOrEmpty(infoUtente.diSistema) && infoUtente.diSistema == "1")
                    {
                        bool pulito = Utils.CleanRightsExtSys(schedaDocResult.docNumber, infoUtente.idPeople, infoUtente.idGruppo);
                    }
                }

                if (!string.IsNullOrEmpty(request.Document.IdParent))
                {
                    int idDocParent = 0;
                    if (Int32.TryParse(request.Document.IdParent, out idDocParent))
                    {
                        BusinessLogic.Documenti.DocManager.UpdateRispostaProtocollo(request.Document.IdParent, schedaDocResult.docNumber);
                    }
                    else
                    {
                        throw new Exception("Valore non valido per il campo Document.IdParent");
                    }
                }
                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.EditDocStateDiagram.EditDocStateDiagramResponse EditDocStateDiagram(Services.Documents.EditDocStateDiagram.EditDocStateDiagramRequest request)
        {
            Services.Documents.EditDocStateDiagram.EditDocStateDiagramResponse response = new Services.Documents.EditDocStateDiagram.EditDocStateDiagramResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "EditDocStateDiagram");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.IdDocument) && !string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                if (string.IsNullOrEmpty(request.StateOfDiagram))
                {
                    throw new PisException("REQUIRED_STATE_OF_DIAGRAM");
                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.Signature))
                        {
                            documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(request.Signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                if (documento != null)
                {
                    if (documento.template != null)
                    {
                        int idDiagramma = 0;
                        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                        DocsPaVO.DiagrammaStato.Stato statoAttuale = null;
                        idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(documento.template.SYSTEM_ID.ToString());
                        if (idDiagramma != 0)
                        {
                            diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                            if (diagramma != null)
                            {
                                if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                                {
                                    statoAttuale = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(documento.docNumber);
                                    bool result = false;


                                    if (statoAttuale == null)
                                    {
                                        foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                        {
                                            if (stato.DESCRIZIONE.ToUpper().Equals(request.StateOfDiagram.ToUpper()))
                                            {
                                                BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(documento.docNumber, stato.SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                                result = true;
                                                Utils.ExecAutoTrasmByIdStatus(infoUtente, documento, stato, documento.template.SYSTEM_ID.ToString());
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < diagramma.PASSI.Count; i++)
                                        {
                                            DocsPaVO.DiagrammaStato.Passo step = (DocsPaVO.DiagrammaStato.Passo)diagramma.PASSI[i];
                                            if (step.STATO_PADRE.SYSTEM_ID == statoAttuale.SYSTEM_ID)
                                            {
                                                for (int j = 0; j < step.SUCCESSIVI.Count; j++)
                                                {
                                                    if (((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).DESCRIZIONE.ToUpper().Equals(request.StateOfDiagram.ToUpper()))
                                                    {
                                                        BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(documento.docNumber, ((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).SYSTEM_ID.ToString(), diagramma, infoUtente.idPeople, infoUtente, string.Empty);
                                                        result = true;
                                                        Utils.ExecAutoTrasmByIdStatus(infoUtente, documento, (DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j], documento.template.SYSTEM_ID.ToString());
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (!result)
                                    {
                                        throw new PisException("STATEOFDIAGRAM_NOT_FOUND");
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new PisException("DIAGRAM_NOT_FOUND");
                        }
                    }
                    else
                    {
                        throw new PisException("TEMPLATE_NOT_FOUND");
                    }
                }
                else
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.EditDocument.EditDocumentResponse EditDocument(Services.Documents.EditDocument.EditDocumentRequest request)
        {
            Services.Documents.EditDocument.EditDocumentResponse response = new Services.Documents.EditDocument.EditDocumentResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "EditDocument");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (request.Document == null || string.IsNullOrEmpty(request.Document.Id))
                {
                    throw new PisException("REQUIRED_DOCUMENT");
                }

                //Reperimento ruolo utente
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                DocsPaVO.utente.Corrispondente mittente = new Corrispondente();

                try
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                    if (ruolo == null)
                    {
                        //Ruolo non trovato
                        throw new PisException("ROLE_NO_EXIST");
                    }
                }
                catch
                {
                    //Ruolo non trovato
                    throw new PisException("ROLE_NO_EXIST");
                }


                // **************************************************************************
                // Per accordi con il cliente viene effettuato questo controllo:
                // se Type == "O" allore viene impostato il valore di CorrespondentType = "O"
                // Controllo effettuato per: Sender, Recipients, RecipientsCC
                // **************************************************************************
                //
                if (request.Document.Sender != null && !string.IsNullOrEmpty(request.Document.Sender.Type) && request.Document.Sender.Type.Equals("O"))
                    request.Document.Sender.CorrespondentType = "O";

                if (request.Document.MultipleSenders != null)
                {
                    for (int i = 0; i < request.Document.MultipleSenders.Length; i++)
                    {
                        Domain.Correspondent c = request.Document.MultipleSenders[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }

                if (request.Document.Recipients != null)
                {
                    for (int i = 0; i < request.Document.Recipients.Length; i++)
                    {
                        Domain.Correspondent c = request.Document.Recipients[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }

                if (request.Document.RecipientsCC != null)
                {
                    for (int i = 0; i < request.Document.RecipientsCC.Length; i++)
                    {
                        Domain.Correspondent c = request.Document.RecipientsCC[i];
                        if (c != null && !string.IsNullOrEmpty(c.Type) && c.Type.Equals("O"))
                            c.CorrespondentType = "O";
                    }
                }
                //


                //Documento salvato
                DocsPaVO.documento.SchedaDocumento documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.Document.Id, request.Document.DocNumber);
                bool protocollaPredisposto = false;
                // Modifica per protocollare un predisposto
                if (!request.Document.Predisposed &&
                    (request.Document.DocumentType == "A" ||
                    request.Document.DocumentType == "P" ||
                    request.Document.DocumentType == "I") &&
                    documento.protocollo != null &&
                    !string.IsNullOrEmpty(documento.protocollo.daProtocollare) &&
                    documento.protocollo.daProtocollare == "1")
                {
                    protocollaPredisposto = true;
                }

                ///Imposto l'oggetto
                if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.Object))
                {
                    documento.oggetto.descrizione = request.Document.Object;
                    documento.oggetto.daAggiornare = true;
                }

                ///Imposto il template
                if (request != null && request.Document != null && request.Document.Template != null && (!string.IsNullOrEmpty(request.Document.Template.Id) || (!string.IsNullOrEmpty(request.Document.Template.Name))))
                {
                    DocsPaVO.ProfilazioneDinamica.Templates template = null;
                    if (!string.IsNullOrEmpty(request.Document.Template.Id))
                    {
                        template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(request.Document.Template.Id);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.Document.Template.Name))
                        {
                            template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(request.Document.Template.Name, infoUtente.idAmministrazione);
                        }
                    }

                    if (template != null)
                    {
                        // Modifica per gestire tipologie modificate in seguito al salvataggio di un documento.
                        if (documento.template != null)
                        {
                            template = documento.template;
                            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg1 in documento.template.ELENCO_OGGETTI)
                            {
                                DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue oldObjText = new DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue();
                                oldObjText.IDTemplate = documento.template.SYSTEM_ID.ToString();
                                oldObjText.ID_Doc_Fasc = documento.systemId;
                                oldObjText.ID_Oggetto = ogg1.SYSTEM_ID.ToString();
                                oldObjText.Valore = ogg1.VALORE_DATABASE;
                                oldObjText.Tipo_Ogg_Custom = ogg1.TIPO.DESCRIZIONE_TIPO;
                                oldObjText.ID_People = infoUtente.idPeople;
                                oldObjText.ID_Ruolo_In_UO = infoUtente.idCorrGlobali;
                                template.OLD_OGG_CUSTOM.Add(oldObjText);
                            }
                        }
                        //Modifica Lembo 10-01-2013: Controllo se l'utente è abilitato all'utilizzo del template.
                        //Solo ruoli con diritti di scrittura ammessi.
                        string controlFilter = string.Format("{0} and id_ruolo={1} and (diritti='2')", template.SYSTEM_ID.ToString(), infoUtente.idGruppo);
                        ArrayList controlloVisibilita = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getRuoliTipoDoc(controlFilter);
                        if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                        {
                            throw new PisException("TEMPLATE_NOT_ROLE_EDITABLE");
                        }
                        //Modifica Lembo 11-01-2013: metodo che controlla la visibilità anche per l'edit.
                        //documento.template = Utils.GetTemplateFromPis(request.Document.Template, template, false);

                        // Modifica Elaborazione XML da PIS req.2. Non si può associare il template nella editDocument
                        documento.template = Utils.GetTemplateFromPisVisibility(request.Document.Template, template, false, infoUtente.idGruppo, "D", request.CodeApplication, infoUtente,null,null,null,true);
                        documento.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                        documento.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                        documento.tipologiaAtto.descrizione = template.DESCRIZIONE.ToString();
                        documento.daAggiornareTipoAtto = true;
                        
                    }
                    else
                    {
                        //Template non trovato
                        throw new PisException("TEMPLATE_NOT_FOUND");
                    }
                }
                ///

                ///Se privato
                if (request != null && request.Document != null && request.Document.PrivateDocument)
                {
                    documento.privato = "1";
                }
                ///

                DocsPaVO.documento.SchedaDocumento schedaDocResult = null;
                DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione = new ResultProtocollazione();
                bool daAggiornareUffRef = false;

                ///CASO DOCUMENTO GRIGIO (NON PROTOCOLLATO)
                if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("G"))
                {

                }
                ///

                //Controlli solo per protocolli
                if (request != null && request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && !request.Document.DocumentType.ToUpper().Equals("G"))
                {
                    if (request.Document.Sender == null && !string.IsNullOrEmpty(request.Document.Sender.CorrespondentType))
                    {
                        //Mittente non presente
                        throw new PisException("REQUIRED_SENDER");
                    }
                    else
                    {
                        if (request.Document.Sender.CorrespondentType.Equals("O"))
                        {
                            mittente = Utils.GetCorrespondentFromPis(request.Document.Sender, infoUtente);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(request.Document.Sender.Id))
                            {
                                mittente = Utils.RisolviCorrispondente(request.Document.Sender.Id, infoUtente);
                                if (mittente == null)
                                {
                                    //Mittente non trovato
                                    throw new PisException("SENDER_NOT_FOUND");
                                }
                            }
                        }
                    }

                    if (request.Document.DocumentType.ToUpper().Equals("I") || request.Document.DocumentType.ToUpper().Equals("P"))
                    {
                        if (request.Document.Recipients == null || request.Document.Recipients.Length == 0)
                        {
                            //Destinatario non presente
                            throw new PisException("REQUIRED_RECIPIENT");
                        }
                    }

                }

                ///CASO PROTOCOLLO IN ARRIVO (ANCHE PREDISPOSTO)
                if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("A"))
                {
                    ((ProtocolloEntrata)documento.protocollo).mittente = mittente;
                    ((ProtocolloEntrata)documento.protocollo).daAggiornareMittente = true;

                    if (request.Document.MultipleSenders != null && request.Document.MultipleSenders.Length > 0)
                    {
                        ((ProtocolloEntrata)documento.protocollo).mittenti = new ArrayList();
                        foreach (Domain.Correspondent corrTemp in request.Document.MultipleSenders)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;
                            if (corrTemp.CorrespondentType.Equals("O"))
                            {
                                corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(corrTemp.Id))
                                {
                                    corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                    if (corrBySys == null)
                                    {
                                        //Mittente non trovato
                                        throw new PisException("SENDER_NOT_FOUND");
                                    }
                                }
                            }
                            ((ProtocolloEntrata)documento.protocollo).mittenti.Add(corrBySys);
                            ((ProtocolloEntrata)documento.protocollo).daAggiornareMittentiMultipli = true;
                        }
                    }

                    //Data protocollo mittente
                    if (!string.IsNullOrEmpty(request.Document.DataProtocolSender))
                    {
                        ((ProtocolloEntrata)documento.protocollo).dataProtocolloMittente = request.Document.DataProtocolSender;
                    }

                    //Protocollo mittente
                    if (!string.IsNullOrEmpty(request.Document.ProtocolSender))
                    {
                        ((ProtocolloEntrata)documento.protocollo).descrizioneProtocolloMittente = request.Document.ProtocolSender;
                    }

                    //Data arrivo TODO
                    if (!string.IsNullOrEmpty(request.Document.ArrivalDate))
                    {
                        // Variabile Booleana per il controllo di validità della data passata in input;
                        bool valid = false;
                        // Data ritornata in output dal TryParseExact per la data arrivo passata in input
                        DateTime dateVal;

                        // Pattern di validità per una data valida
                        string pattern = "dd/MM/yyyy HH:mm:ss";
                        string pattern2 = "dd/MM/yyyy HH:mm";

                        try
                        {
                            /*
                             *  Date Format Example: 01/03/2012
                             *  dd = day (01)
                             *  MM = month (03)
                             *  yyyy = year (2012)
                             *  / = date separator
                             */

                            if (!DateTime.TryParseExact(request.Document.ArrivalDate, pattern, null, System.Globalization.DateTimeStyles.None, out dateVal))
                            {
                                if (!DateTime.TryParseExact(request.Document.ArrivalDate, pattern2, null, System.Globalization.DateTimeStyles.None, out dateVal))
                                {
                                    throw new Exception("Formato Data non corretto");
                                }
                            }

                            valid = true;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Formato Data non corretto");
                        }

                        if (valid)
                        {
                            // Controllo che la data inserita non sia posteriore alla data odierna
                            DateTime dateVal2;
                            // Variabile Booleana per il controllo di validità della data DateTime.Now trasformato nel formato dd/MM/yyyy;
                            bool dateNow = true;

                            if (!DateTime.TryParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), pattern, null, System.Globalization.DateTimeStyles.None, out dateVal2))
                            {
                                // Non dovrebbe entrare mai in questo controllo.
                                dateNow = false;
                                throw new Exception("Formato Data non corretto");
                            }

                            if (dateNow)
                            {
                                // dateVal: data passata in input;
                                // dateVal2: DateTime.Now convertito nel formato dd/MM/yyyy
                                int result = DateTime.Compare(dateVal, dateVal2);

                                if (result > 0)
                                    // Data di input è successiva alla data odierna;
                                    //((Documento)schedaDoc.documenti[0]).dataArrivo = string.Empty;
                                    throw new Exception("La data di arrivo non può essere posteriore alla data di segnatura");
                                else
                                    // Data di input valida e conforme;
                                    ((Documento)documento.documenti[0]).dataArrivo = request.Document.ArrivalDate;
                            }
                        }
                    }
                }
                ///

                ///CASO PROTOCOLLO IN PARTENZA (ANCHE PREDISPOSTO)
                if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("P"))
                {

                    ((ProtocolloUscita)documento.protocollo).mittente = mittente;
                    ((ProtocolloUscita)documento.protocollo).daAggiornareMittente = true;

                    if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                    {
                        ((ProtocolloUscita)documento.protocollo).destinatari = new ArrayList();
                        foreach (Domain.Correspondent corrTemp in request.Document.Recipients)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;

                            // Verifica se occasionale
                            if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                            {
                                corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                            }
                            else
                            {
                                // Se non è occasionale, lo risolve sulla rubrica
                                corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }
                            }
                            ((ProtocolloUscita)documento.protocollo).destinatari.Add(corrBySys);
                            ((ProtocolloUscita)documento.protocollo).daAggiornareDestinatari = true;
                        }
                    }

                    if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                    {
                        ((ProtocolloUscita)documento.protocollo).destinatariConoscenza = new ArrayList();
                        foreach (Domain.Correspondent corrTemp in request.Document.RecipientsCC)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = null;

                            // Verifica se occasionale
                            if (corrTemp != null && !string.IsNullOrEmpty(corrTemp.CorrespondentType) && corrTemp.CorrespondentType.Equals("O"))
                            {
                                corrBySys = Utils.GetCorrespondentFromPis(corrTemp, infoUtente);
                            }
                            else
                            {
                                // Se non è occasionale, lo risolve sulla rubrica
                                corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                                if (corrBySys == null)
                                {
                                    //Destinatario non trovato
                                    throw new PisException("RECIPIENT_NOT_FOUND");
                                }
                            }
                            ((ProtocolloUscita)documento.protocollo).destinatariConoscenza.Add(corrBySys);
                            ((ProtocolloUscita)documento.protocollo).daAggiornareDestinatariConoscenza = true;
                        }
                    }

                    if (request.Document.Predisposed)
                    {
                        documento.predisponiProtocollazione = true;
                    }
                }
                ///

                ///CASO PROTOCOLLO INTERNO (ANCHE PREDISPOSTO)
                if (request.Document != null && !string.IsNullOrEmpty(request.Document.DocumentType) && request.Document.DocumentType.ToUpper().Equals("I"))
                {

                    ((ProtocolloInterno)documento.protocollo).mittente = mittente;
                    ((ProtocolloInterno)documento.protocollo).daAggiornareMittente = true;

                    if (request.Document.Recipients != null && request.Document.Recipients.Length > 0)
                    {
                        ((ProtocolloInterno)documento.protocollo).destinatari = new ArrayList();
                        foreach (Domain.Correspondent corrTemp in request.Document.Recipients)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                            if (corrBySys == null)
                            {
                                //Destinatario non trovato
                                throw new PisException("RECIPIENT_NOT_FOUND");
                            }
                            ((ProtocolloInterno)documento.protocollo).destinatari.Add(corrBySys);
                            ((ProtocolloInterno)documento.protocollo).daAggiornareDestinatari = true;
                        }
                    }

                    if (request.Document.RecipientsCC != null && request.Document.RecipientsCC.Length > 0)
                    {
                        ((ProtocolloInterno)documento.protocollo).destinatariConoscenza = new ArrayList();
                        foreach (Domain.Correspondent corrTemp in request.Document.RecipientsCC)
                        {
                            DocsPaVO.utente.Corrispondente corrBySys = Utils.RisolviCorrispondente(corrTemp.Id, infoUtente);
                            if (corrBySys == null)
                            {
                                //Destinatario non trovato
                                throw new PisException("RECIPIENT_NOT_FOUND");
                            }
                            ((ProtocolloInterno)documento.protocollo).destinatariConoscenza.Add(corrBySys);
                            ((ProtocolloInterno)documento.protocollo).daAggiornareDestinatariConoscenza = true;
                        }
                    }

                    if (request.Document.Predisposed)
                    {
                        documento.predisponiProtocollazione = true;
                    }
                }

                if (!protocollaPredisposto)
                    schedaDocResult = BusinessLogic.Documenti.DocSave.save(infoUtente, documento, false, out daAggiornareUffRef, ruolo);
                else
                {
                    // controllo se presente un RF
                    if (request.Document.Register != null && !string.IsNullOrEmpty(request.Document.Register.Code))
                    {
                        DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistroByCode(request.Document.Register.Code);
                        if (reg != null )
                        {
                            // solo se è un rf e non è chiuso aggiungo le informazioni di protocollazione RF
                            if (reg.chaRF == "1" && reg.stato != "C")
                            {
                                documento.id_rf_prot = reg.systemId;
                                documento.id_rf_invio_ricevuta = reg.systemId;
                                documento.cod_rf_prot = reg.codRegistro;
                            }
                        }
                        else
                        {
                            //RF non trovato
                            throw new PisException("RF_NOT_FOUND");
                        }
                    }

                    DocsPaVO.documento.ResultProtocollazione resultProto = new ResultProtocollazione();
                    schedaDocResult = BusinessLogic.Documenti.ProtoManager.protocolla(documento, ruolo, infoUtente, out resultProto);
                }
                if (schedaDocResult != null)
                {
                    response.Document = Utils.GetDetailsDocument(schedaDocResult, infoUtente, false);
                }
                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.GetDocument.GetDocumentResponse GetDocument(Services.Documents.GetDocument.GetDocumentRequest request)
        {
            Services.Documents.GetDocument.GetDocumentResponse response = new Services.Documents.GetDocument.GetDocumentResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetDocument");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.IdDocument) && !string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                Domain.Document responseDocument = new Domain.Document();

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();
                bool FileConFirma = false;
                if (!string.IsNullOrEmpty(request.GetFileWithSignature) && request.GetFileWithSignature == "1")
                    FileConFirma = true;
                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.Signature))
                        {
                            documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(request.Signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                if (documento != null)
                {
                    //Dati in comune per tutti
                    responseDocument.Id = documento.systemId;
                    responseDocument.Object = documento.oggetto.descrizione;
                    if (!string.IsNullOrEmpty(documento.privato) && documento.privato.Equals("1"))
                    {
                        responseDocument.PrivateDocument = true;
                    }
                    else
                    {
                        responseDocument.PrivateDocument = false;
                    }
                    responseDocument.CreationDate = documento.dataCreazione;
                    responseDocument.DocNumber = documento.docNumber;
                    responseDocument.DocumentType = documento.tipoProto;

                    if (BusinessLogic.Documenti.DocumentConsolidation.IsConfigEnabled())
                    {
                        if (documento.ConsolidationState != null && documento.ConsolidationState.State > DocumentConsolidationStateEnum.None)
                        {
                            string message = string.Empty;
                            // Diabilitazione controlli su documento consolidato
                            if (documento.ConsolidationState.State == DocumentConsolidationStateEnum.Step1)
                            {
                                responseDocument.ConsolidationState = "ConsolidationState1";
                            }
                            else if (documento.ConsolidationState.State == DocumentConsolidationStateEnum.Step2)
                            {
                                responseDocument.ConsolidationState = "ConsolidationState2";
                            }
                        }
                    }
                    else
                    {
                        responseDocument.ConsolidationState = string.Empty;
                    }

                    if (documento.noteDocumento != null && documento.noteDocumento.Count > 0)
                    {
                        responseDocument.Note = new Domain.Note[documento.noteDocumento.Count];
                        int y = 0;
                        foreach (DocsPaVO.Note.InfoNota tempNot in documento.noteDocumento)
                        {
                            Domain.Note nota = new Domain.Note();
                            nota.Description = tempNot.Testo;
                            nota.Id = tempNot.Id;
                            nota.User = new Domain.User();
                            nota.User.Id = tempNot.UtenteCreatore.IdUtente;
                            nota.User.Description = tempNot.UtenteCreatore.DescrizioneUtente;
                            responseDocument.Note[y] = nota;
                            y++;
                        }
                    }

                    //Se il documento è in cestino
                    if (!string.IsNullOrEmpty(documento.inCestino) && documento.inCestino.Equals("1"))
                    {
                        responseDocument.InBasket = true;
                    }
                    else
                    {
                        responseDocument.InBasket = false;
                    }

                    if (documento.template != null)
                    {
                        responseDocument.Template = Utils.GetDetailsTemplateDoc(documento.template, documento.docNumber);
                    }

                    //Documento principale
                    if (documento.documenti != null && documento.documenti.Count > 0)
                    {
                        DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)documento.documenti[0];
                        responseDocument.MainDocument = Utils.GetFile(versioneCorrente, request.GetFile, infoUtente, false, false, string.Empty, null, FileConFirma);
                    }

                    //Prendi allegati
                    if (documento.allegati != null && documento.allegati.Count > 0)
                    {
                        if (string.IsNullOrEmpty(request.CodeApplication) || (request.CodeApplication.ToUpper() != "ALBO_TELEMATICO" && request.CodeApplication.ToUpper() != "GDOC"))
                        {
                            int y = 0;
                            responseDocument.Attachments = new Domain.File[documento.allegati.Count];
                            foreach (DocsPaVO.documento.FileRequest tempAll in documento.allegati)
                            {
                                responseDocument.Attachments[y] = Utils.GetFile(tempAll, request.GetFile, infoUtente, false, false, string.Empty, null, FileConFirma);
                                y++;
                            }
                        }
                        else
                        {
                            ArrayList allegatiAlbo = new ArrayList();
                            foreach (DocsPaVO.documento.FileRequest tempAll in documento.allegati)
                            {
                                if (BusinessLogic.Documenti.AllegatiManager.getIsAllegatoEsterno(tempAll.versionId)!="1" &&
                                    BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(tempAll.versionId)!="1" &&
                                    BusinessLogic.Documenti.AllegatiManager.getIsAllegatoPEC(tempAll.versionId)!="1")
                                {
                                    allegatiAlbo.Add(Utils.GetFile(tempAll, request.GetFile, infoUtente, false, false, string.Empty, null, FileConFirma));

                                }
                            }
                            if (allegatiAlbo.Count > 0)
                                responseDocument.Attachments = (Domain.File[])allegatiAlbo.ToArray(typeof(Domain.File));
                            else
                                responseDocument.Attachments = null;
                        }
                    }

                    if (documento.protocollo != null)
                    {

                        if (documento.protocollo.protocolloAnnullato != null)
                        {
                            responseDocument.Annulled = true;
                            //responseDocument.AnnulmentDate = documento.protocollo.protocolloAnnullato.dataAnnullamento;
                        }
                        else
                        {
                            responseDocument.Annulled = false;
                        }

                        if (!string.IsNullOrEmpty(documento.protocollo.segnatura))
                        {
                            responseDocument.Signature = documento.protocollo.segnatura;
                        }
                        else
                        {
                            responseDocument.Predisposed = true;
                        }

                        if (!string.IsNullOrEmpty(documento.protocollo.dataProtocollazione))
                        {
                            responseDocument.ProtocolDate = documento.protocollo.dataProtocollazione;
                        }

                        if (documento.registro != null)
                        {
                            responseDocument.Register = Utils.GetRegister(documento.registro);
                        }


                        //CASO PROTOCOLLO IN ARRIVO
                        if (documento.tipoProto.Equals("A"))
                        {
                            if ((ProtocolloEntrata)documento.protocollo != null && ((ProtocolloEntrata)documento.protocollo).mittente != null)
                            {
                                responseDocument.Sender = Utils.GetCorrespondent(((ProtocolloEntrata)documento.protocollo).mittente, infoUtente);
                            }

                            if ((ProtocolloEntrata)documento.protocollo != null && ((ProtocolloEntrata)documento.protocollo).mittenti != null && ((ProtocolloEntrata)documento.protocollo).mittenti.Count > 0)
                            {
                                responseDocument.MultipleSenders = new Domain.Correspondent[((ProtocolloEntrata)documento.protocollo).mittenti.Count];
                                int y = 0;
                                foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloEntrata)documento.protocollo).mittenti)
                                {
                                    responseDocument.MultipleSenders[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                    y++;
                                }
                            }

                            if (((ProtocolloEntrata)documento.protocollo).descrizioneProtocolloMittente != null)
                            {
                                responseDocument.ProtocolSender = ((ProtocolloEntrata)documento.protocollo).descrizioneProtocolloMittente;
                            }

                            if (((ProtocolloEntrata)documento.protocollo) != null && !string.IsNullOrEmpty(((ProtocolloEntrata)documento.protocollo).dataProtocolloMittente))
                            {
                                responseDocument.DataProtocolSender = ((ProtocolloEntrata)documento.protocollo).dataProtocolloMittente;
                            }

                            if (documento.descMezzoSpedizione != null)
                            {
                                responseDocument.MeansOfSending = documento.descMezzoSpedizione;
                            }

                            if (((ProtocolloEntrata)documento.protocollo) != null && string.IsNullOrEmpty(((ProtocolloEntrata)documento.protocollo).dataProtocolloMittente))
                            {
                                responseDocument.ProtocolDate = ((ProtocolloEntrata)documento.protocollo).dataProtocollazione;
                            }

                            if (documento.documenti != null && documento.documenti != null && documento.documenti.Count > 0)
                            {
                                responseDocument.ArrivalDate = ((DocsPaVO.documento.Documento)documento.documenti[0]).dataArrivo;
                            }
                        }

                        //CASO PROTOCOLLO IN PARTENZA
                        if (documento.tipoProto.Equals("P"))
                        {
                            if ((ProtocolloUscita)documento.protocollo != null && ((ProtocolloUscita)documento.protocollo).mittente != null)
                            {
                                responseDocument.Sender = Utils.GetCorrespondent(((ProtocolloUscita)documento.protocollo).mittente, infoUtente);
                            }

                            if ((ProtocolloUscita)documento.protocollo != null && ((ProtocolloUscita)documento.protocollo).destinatari != null && ((ProtocolloUscita)documento.protocollo).destinatari.Count > 0)
                            {
                                responseDocument.Recipients = new Domain.Correspondent[((ProtocolloUscita)documento.protocollo).destinatari.Count];
                                int y = 0;
                                foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloUscita)documento.protocollo).destinatari)
                                {
                                    responseDocument.Recipients[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                    y++;
                                }
                            }

                            if ((ProtocolloUscita)documento.protocollo != null && ((ProtocolloUscita)documento.protocollo).destinatariConoscenza != null && ((ProtocolloUscita)documento.protocollo).destinatariConoscenza.Count > 0)
                            {
                                responseDocument.RecipientsCC = new Domain.Correspondent[((ProtocolloUscita)documento.protocollo).destinatariConoscenza.Count];
                                int y = 0;
                                foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloUscita)documento.protocollo).destinatariConoscenza)
                                {
                                    responseDocument.RecipientsCC[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                    y++;
                                }
                            }
                        }

                        //CASO PROTOCOLLO INTERNO
                        if (documento.tipoProto.Equals("I"))
                        {
                            if ((ProtocolloInterno)documento.protocollo != null && ((ProtocolloInterno)documento.protocollo).mittente != null)
                            {
                                responseDocument.Sender = Utils.GetCorrespondent(((ProtocolloInterno)documento.protocollo).mittente, infoUtente);
                            }

                            if ((ProtocolloInterno)documento.protocollo != null && ((ProtocolloInterno)documento.protocollo).destinatari != null && ((ProtocolloInterno)documento.protocollo).destinatari.Count > 0)
                            {
                                responseDocument.Recipients = new Domain.Correspondent[((ProtocolloInterno)documento.protocollo).destinatari.Count];
                                int y = 0;
                                foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloInterno)documento.protocollo).destinatari)
                                {
                                    responseDocument.Recipients[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                    y++;
                                }
                            }

                            if ((ProtocolloInterno)documento.protocollo != null && ((ProtocolloInterno)documento.protocollo).destinatariConoscenza != null && ((ProtocolloInterno)documento.protocollo).destinatariConoscenza.Count > 0)
                            {
                                responseDocument.RecipientsCC = new Domain.Correspondent[((ProtocolloInterno)documento.protocollo).destinatariConoscenza.Count];
                                int y = 0;
                                foreach (DocsPaVO.utente.Corrispondente tempCorr in ((ProtocolloInterno)documento.protocollo).destinatariConoscenza)
                                {
                                    responseDocument.RecipientsCC[y] = Utils.GetCorrespondent(tempCorr, infoUtente);
                                    y++;
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                response.Document = responseDocument;

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.GetDocumentStateDiagram.GetDocumentStateDiagramResponse GetDocumentStateDiagram(Services.Documents.GetDocumentStateDiagram.GetDocumentStateDiagramRequest request)
        {
            Services.Documents.GetDocumentStateDiagram.GetDocumentStateDiagramResponse response = new Services.Documents.GetDocumentStateDiagram.GetDocumentStateDiagramResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetDocumentStateDiagram");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.IdDocument) && !string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                Domain.StateOfDiagram stateOfDiagramResponse = new Domain.StateOfDiagram();

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.Signature))
                        {
                            documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(request.Signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                if (documento != null)
                {
                    if (documento.template != null)
                    {
                        int idDiagramma = 0;
                        DocsPaVO.DiagrammaStato.Stato stato = null;
                        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                        idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(documento.template.SYSTEM_ID.ToString());
                        if (idDiagramma != 0)
                        {
                            diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                            if (diagramma != null)
                            {
                                stato = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(documento.docNumber);
                                if (stato != null)
                                {
                                    stateOfDiagramResponse = Utils.GetStateOfDiagram(stato, diagramma.SYSTEM_ID.ToString());
                                }
                                else
                                {
                                    throw new PisException("DOC_STATEOFDIAGRAM_NOT_FOUND");
                                }
                            }
                        }
                        else
                        {
                            throw new PisException("DIAGRAM_NOT_FOUND");
                        }
                    }
                    else
                    {
                        throw new PisException("TEMPLATE_NOT_FOUND");
                    }
                }
                else
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                response.StateOfDiagram = stateOfDiagramResponse;

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.GetFileDocumentById.GetFileDocumentByIdResponse GetFileDocumentById(Services.Documents.GetFileDocumentById.GetFileDocumentByIdRequest request)
        {
            Services.Documents.GetFileDocumentById.GetFileDocumentByIdResponse response = new Services.Documents.GetFileDocumentById.GetFileDocumentByIdResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetFileDocumentById");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new PisException("REQUIRED_ID");
                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();
                bool FileConFirma = false;
                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        if (!string.IsNullOrEmpty(request.VersionId) && request.VersionId.Contains("CERCA.TRE"))
                            documento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, request.IdDocument);
                        else
                            documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                        if (documento != null && documento.documenti != null && documento.documenti.Count > 0)
                        {
                            int numVersione = 0;
                            if (!string.IsNullOrEmpty(request.VersionId))
                            {
                                if (request.VersionId.ToUpper() == "SIGNED")
                                {
                                    FileConFirma = true;
                                }
                                else if (request.VersionId.Contains("CERCA.TRE"))
                                {
                                    FileConFirma = true;
                                    if(request.VersionId.Contains("CERCA.TRE_"))
                                    {
                                        bool result = Int32.TryParse(request.VersionId.Replace("CERCA.TRE_",""), out numVersione);
                                        if (!result)
                                        {
                                            throw new PisException("REQUIRED_INTEGER");
                                        }
                                        else
                                        {
                                            if (documento.documenti.Count < numVersione || numVersione <= 0)
                                            {
                                                throw new PisException("FILE_VERSION_NOT_FOUND");
                                            }
                                            else
                                            {
                                                numVersione = documento.documenti.Count - numVersione;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    bool result = Int32.TryParse(request.VersionId, out numVersione);
                                    if (!result)
                                    {
                                        throw new PisException("REQUIRED_INTEGER");
                                    }
                                    else
                                    {
                                        if (documento.documenti.Count < numVersione || numVersione <= 0)
                                        {
                                            throw new PisException("FILE_VERSION_NOT_FOUND");
                                        }
                                        else
                                        {
                                            numVersione = documento.documenti.Count - numVersione;
                                        }
                                    }
                                }
                            }
                            DocsPaVO.documento.FileRequest versione = (DocsPaVO.documento.FileRequest)documento.documenti[numVersione];
                            response.File = Utils.GetFile(versione, true, infoUtente, false, false, string.Empty, null, FileConFirma);
                        }
                        else
                        {
                            throw new PisException("DOCUMENT_NOT_FOUND");
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.GetFileWithSignatureAndSignerInfo.GetFileWithSignatureAndSignerInfoResponse GetFileWithSignatureAndSignerInfo(Services.Documents.GetFileWithSignatureAndSignerInfo.GetFileWithSignatureAndSignerInfoRequest request)
        {
            Services.Documents.GetFileWithSignatureAndSignerInfo.GetFileWithSignatureAndSignerInfoResponse response = new Services.Documents.GetFileWithSignatureAndSignerInfo.GetFileWithSignatureAndSignerInfoResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetFileWithSignatureOrStamp");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.IdDocument) && !string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                /*
                if (request.WithSignature && request.WithSignerInfo)
                {
                    throw new PisException("REQUIRED_ONLY_SIGNATURE_OR_STAMP");
                }
                
                if (!request.WithSignature && !request.WithSignerInfo)
                {
                    throw new PisException("REQUIRED_SIGNATURE_OR_STAMP");
                }*/

                string path = HostingEnvironment.MapPath("~/XML/labelPdf.xml");

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.Signature))
                        {
                            documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(request.Signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                try
                {

                    if (documento != null && documento.documenti != null && documento.documenti.Count > 0)
                    {
                        int numVersione = 0;
                        DocsPaVO.documento.FileRequest versione = (DocsPaVO.documento.FileRequest)documento.documenti[numVersione];

                        labelPdf pos = new labelPdf();
                        if (!string.IsNullOrEmpty(request.SignaturePosition))
                            pos.position = request.SignaturePosition;
                        else
                            pos.position = "15-30";

                        if (!string.IsNullOrEmpty(request.SignatureColor))
                            pos.sel_color = request.SignatureColor;
                        else
                            pos.sel_color = "1";

                        if (!string.IsNullOrEmpty(request.SignatureFont))
                            pos.sel_font = request.SignatureFont;
                        else
                            pos.sel_font = "1";

                        if (request.SignatureVertical)
                            pos.orientamento = "verticale";
                        else
                            pos.orientamento = "orizzontale";

                        pos.tipoLabel = true;

                        if (!request.WithSignature)
                            pos.notimbro = true;

                        if (request.WithSignerInfo)
                        {
                            pos.digitalSignInfo = new labelPdfDigitalSignInfo { printFormatSign = labelPdfDigitalSignInfo.TypePrintFormatSign.Sign_Extended };
                            if (request.SignerInfoOnLastPage)
                            {
                                pos.digitalSignInfo.printOnFirstPage = false;
                                pos.digitalSignInfo.printOnLastPage = true;
                            }
                            else
                            {
                                pos.digitalSignInfo.printOnFirstPage = true;
                                pos.digitalSignInfo.printOnLastPage = false;
                            }
                        }

                        response.File = Utils.GetFile(versione, true, infoUtente, true, false, path, documento, false, pos);
                    }
                    else
                    {
                        throw new PisException("DOCUMENT_NOT_FOUND");
                    }


                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.GetFileWithSignatureOrStamp.GetFileWithSignatureOrStampResponse GetFileWithSignatureOrStamp(Services.Documents.GetFileWithSignatureOrStamp.GetFileWithSignatureOrStampRequest request)
        {
            Services.Documents.GetFileWithSignatureOrStamp.GetFileWithSignatureOrStampResponse response = new Services.Documents.GetFileWithSignatureOrStamp.GetFileWithSignatureOrStampResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetFileWithSignatureOrStamp");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.IdDocument) && !string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                if (request.WithSignature && request.WithStamp)
                {
                    throw new PisException("REQUIRED_ONLY_SIGNATURE_OR_STAMP");
                }

                if (!request.WithSignature && !request.WithStamp)
                {
                    throw new PisException("REQUIRED_SIGNATURE_OR_STAMP");
                }

                string path = HostingEnvironment.MapPath("~/XML/labelPdf.xml");

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.Signature))
                        {
                            documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(request.Signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                try
                {

                    if (documento != null && documento.documenti != null && documento.documenti.Count > 0)
                    {
                        int numVersione = 0;
                        DocsPaVO.documento.FileRequest versione = (DocsPaVO.documento.FileRequest)documento.documenti[numVersione];
                        response.File = Utils.GetFile(versione, true, infoUtente, request.WithSignature, request.WithStamp, path, documento, false);
                    }
                    else
                    {
                        throw new PisException("DOCUMENT_NOT_FOUND");
                    }


                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.GetTemplateDoc.GetTemplateDocResponse GetTemplateDoc(Services.Documents.GetTemplateDoc.GetTemplateDocRequest request)
        {
            Services.Documents.GetTemplateDoc.GetTemplateDocResponse response = new Services.Documents.GetTemplateDoc.GetTemplateDocResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetTemplateDoc");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.DescriptionTemplate) && string.IsNullOrEmpty(request.IdTemplate))
                {
                    throw new PisException("REQUIRED_CODE_OR_ID_TEMPLATE");
                }

                if (!string.IsNullOrEmpty(request.DescriptionTemplate) && !string.IsNullOrEmpty(request.IdTemplate))
                {
                    throw new PisException("REQUIRED_ONLY_CODE_OR_ID_TEMPLATE");
                }

                Domain.Template templateResponse = new Domain.Template();

                //DocsPaVO.utente.Registro registro = null;
                DocsPaVO.ProfilazioneDinamica.Templates template = null;

                if (!string.IsNullOrEmpty(request.DescriptionTemplate))
                {
                    // Causa errore in presenza di più template con stessa descrizione eseguo un fix
                    DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] listaTemplate = null;
                    listaTemplate = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getListTemplatesLiteByRole(infoUtente.idAmministrazione, infoUtente.idGruppo);

                    if (listaTemplate != null && listaTemplate.Length > 0)
                    {
                        foreach (DocsPaVO.ProfilazioneDinamicaLite.TemplateLite TL in listaTemplate)
                        {
                            if (request.DescriptionTemplate.ToUpper().Equals(TL.name.ToUpper()))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(TL.system_id);
                            }
                        }
                    }
                    //template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(request.DescriptionTemplate);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.IdTemplate))
                    {
                        try
                        {
                            template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(request.IdTemplate);
                        }
                        catch
                        {
                            //Template non trovato
                            throw new PisException("TEMPLATE_NOT_FOUND");
                        }
                    }
                }
                //Modifica Lembo 10-01-2013: Controllo se l'utente è abilitato all'utilizzo del template.
                // per ora facciamo visualizzare sia quelli con diritti di scrittura che diritti di visualizzazione.
                string controlFilter = string.Format("{0} and id_ruolo={1} and (diritti='1' or diritti='2')", template.SYSTEM_ID.ToString(), infoUtente.idGruppo);
                ArrayList controlloVisibilita = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getRuoliTipoDoc(controlFilter);
                if (controlloVisibilita == null || controlloVisibilita.Count == 0)
                {
                    throw new PisException("TEMPLATE_NOT_ROLE_VISIBLE");
                }


                if (template != null)
                {
                    templateResponse.Name = template.DESCRIZIONE;
                    templateResponse.Id = template.SYSTEM_ID.ToString();

                    DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                    template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));


                    if (oggettiCustom != null && oggettiCustom.Length > 0)
                    {
                        templateResponse.Fields = new Domain.Field[oggettiCustom.Length];
                        int numField = 0;
                        //Modifica Lembo 11-01-2013: Prelevo i diritti per popolare l'attributo Rights
                        DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[] dirittiOggetti =
                            (DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli[])(BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getDirittiCampiTipologiaDoc(infoUtente.idGruppo, template.SYSTEM_ID.ToString())).ToArray(typeof(DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli));

                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in oggettiCustom)
                        {

                            Domain.Field fieldTemp = new Domain.Field();

                            fieldTemp.Id = oggettoCustom.SYSTEM_ID.ToString();
                            fieldTemp.Name = oggettoCustom.DESCRIZIONE;
                            if (!string.IsNullOrEmpty(oggettoCustom.CAMPO_OBBLIGATORIO) && oggettoCustom.CAMPO_OBBLIGATORIO == "SI")
                            {
                                fieldTemp.Required = true;
                            }
                            else
                            {
                                fieldTemp.Required = false;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                            {
                                fieldTemp.Type = "MultipleChoise";
                                if (oggettoCustom.ELENCO_VALORI != null && oggettoCustom.ELENCO_VALORI.Count > 0)
                                {
                                    fieldTemp.MultipleChoice = new string[oggettoCustom.ELENCO_VALORI.Count];
                                    for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Count; i++)
                                    {
                                        fieldTemp.MultipleChoice[i] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE;
                                    }
                                }
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Corrispondente"))
                            {
                                fieldTemp.Type = "Correspondent";
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CampoDiTesto"))
                            {
                                fieldTemp.Type = "TextField";
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("SelezioneEsclusiva"))
                            {
                                fieldTemp.Type = "ExclusiveSelection";
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("MenuATendina"))
                            {
                                fieldTemp.Type = "DropDown";
                                if (oggettoCustom.ELENCO_VALORI != null && oggettoCustom.ELENCO_VALORI.Count > 0)
                                {
                                    fieldTemp.MultipleChoice = new string[oggettoCustom.ELENCO_VALORI.Count];
                                    for (int i = 0; i < oggettoCustom.ELENCO_VALORI.Count; i++)
                                    {
                                        fieldTemp.MultipleChoice[i] = ((DocsPaVO.ProfilazioneDinamica.ValoreOggetto)oggettoCustom.ELENCO_VALORI[i]).VALORE;
                                    }
                                }
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Contatore"))
                            {
                                fieldTemp.Type = "Counter";
                                if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                                {
                                    DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                    if (reg != null)
                                    {
                                        fieldTemp.CodeRegisterOrRF = reg.codRegistro;
                                    }
                                }
                                fieldTemp.CounterToTrigger = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Data"))
                            {
                                fieldTemp.Type = "Date";
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE;
                            }

                            if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("ContatoreSottocontatore"))
                            {
                                fieldTemp.Type = "SubCounter";
                                if (oggettoCustom.TIPO_CONTATORE.Equals("A") || oggettoCustom.TIPO_CONTATORE.Equals("R"))
                                {
                                    DocsPaVO.utente.Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(oggettoCustom.ID_AOO_RF);
                                    if (reg != null)
                                    {
                                        fieldTemp.CodeRegisterOrRF = reg.codRegistro;
                                    }
                                }
                                fieldTemp.CounterToTrigger = oggettoCustom.CONTATORE_DA_FAR_SCATTARE;
                                fieldTemp.Value = oggettoCustom.VALORE_DATABASE + "-" + oggettoCustom.VALORE_SOTTOCONTATORE; ;
                            }
                            //Modifica Lembo 11-01-2013: popolo l'attributo Rights
                            if (!(oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("Separatore"))
                            {
                                if (dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()) != null)
                                {
                                    if (dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()).INS_MOD_OGG_CUSTOM == "1")
                                        fieldTemp.Rights = "INSERT_AND_MODIFY";
                                    else if (dirittiOggetti.FirstOrDefault(e => e.ID_OGGETTO_CUSTOM.ToUpperInvariant() == oggettoCustom.SYSTEM_ID.ToString().ToUpperInvariant()).VIS_OGG_CUSTOM == "1")
                                        fieldTemp.Rights = "SEARCH_AND_VIEW";
                                    else
                                        fieldTemp.Rights = "NONE";
                                }
                                else
                                    fieldTemp.Rights = "NONE";
                            }
                            else
                            {
                                fieldTemp.Rights = "NONE";
                            }
                            templateResponse.Fields[numField] = fieldTemp;
                            numField++;
                        }
                    }

                    int idDiagramma = 0;
                    DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                    idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(template.SYSTEM_ID.ToString());
                    if (idDiagramma != 0)
                    {
                        diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());
                        if (diagramma != null)
                        {
                            Domain.StateDiagram responseDiagram = new Domain.StateDiagram();
                            responseDiagram.Description = diagramma.DESCRIZIONE;
                            responseDiagram.Id = diagramma.SYSTEM_ID.ToString();
                            int y = 0;
                            if (diagramma.STATI != null && diagramma.STATI.Count > 0)
                            {
                                responseDiagram.StateOfDiagram = new Domain.StateOfDiagram[diagramma.STATI.Count];

                                foreach (DocsPaVO.DiagrammaStato.Stato statoD in diagramma.STATI)
                                {
                                    Domain.StateOfDiagram stateDiagram = new Domain.StateOfDiagram();
                                    stateDiagram.Description = statoD.DESCRIZIONE;
                                    stateDiagram.Id = statoD.SYSTEM_ID.ToString();
                                    stateDiagram.InitialState = statoD.STATO_INIZIALE;
                                    stateDiagram.FinaleState = statoD.STATO_FINALE;
                                    stateDiagram.DiagramId = responseDiagram.Id;
                                    responseDiagram.StateOfDiagram[y] = stateDiagram;
                                    y++;
                                }
                                templateResponse.StateDiagram = responseDiagram;
                            }
                        }
                    }
                }
                else
                {
                    //Template non trovato
                    throw new PisException("TEMPLATE_NOT_FOUND");
                }

                response.Template = templateResponse;

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.GetTemplatesDocuments.GetTemplatesDocumentsResponse GetTemplatesDocuments(Services.Documents.GetTemplatesDocuments.GetTemplatesDocumentsRequest request)
        {
            Services.Documents.GetTemplatesDocuments.GetTemplatesDocumentsResponse response = new Services.Documents.GetTemplatesDocuments.GetTemplatesDocumentsResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetTemplatesDocuments");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente


                DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] listaTemplate = null;
                listaTemplate = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getListTemplatesLiteByRole(infoUtente.idAmministrazione, infoUtente.idGruppo);

                if (listaTemplate != null && listaTemplate.Length > 0)
                {
                    Domain.Template[] templateResponse = new Domain.Template[listaTemplate.Length];

                    for (int y = 0; y < listaTemplate.Length; y++)
                    {
                        Domain.Template tempTemp = new Domain.Template();
                        tempTemp.Name = listaTemplate[y].name;
                        tempTemp.Id = listaTemplate[y].system_id;
                        templateResponse[y] = tempTemp;
                    }

                    response.Templates = templateResponse;
                }
                else
                {
                    //Nessun template trovato
                    throw new PisException("TEMPLATES_NOT_FOUND");
                }

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.SearchDocuments.SearchDocumentsResponse SearchDocuments(Services.Documents.SearchDocuments.SearchDocumentsRequest request)
        {
            Services.Documents.SearchDocuments.SearchDocumentsResponse response = new Services.Documents.SearchDocuments.SearchDocumentsResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "SearchDocuments");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (request.Filters == null || request.Filters.Length == 0)
                {
                    throw new PisException("REQUIRED_FILTER");
                }

                //Chiamata al metodo CheckFilterType(request.Filters)
                Utils.CheckFilterTypes(request.Filters);

                //Converti i filtri di ricerca
                DocsPaVO.filtri.FiltroRicerca[][] qV = Utils.GetFiltersDocumentsFromPis(request.Filters, infoUtente);

                Domain.Document[] responseDocuments = null;

                int nRec = 0;
                int numTotPage = 0;
                // questo valore da problemi con la max row searchable. Lo imposto a true comunque.
                //bool allDocuments = false;
                bool allDocuments = true;
                int pageSize = 20;
                int numPage = 1;

                if (request.ElementsInPage == null && request.PageNumber == null)
                {
                    allDocuments = true;
                }
                else
                {
                    if ((request.ElementsInPage != null && request.PageNumber == null) || (request.ElementsInPage == null && request.PageNumber != null))
                    {
                        //Paginazione non valida
                        throw new PisException("INVALID_PAGINATION");
                    }
                    else
                    {
                        pageSize = request.ElementsInPage ?? 20;
                        numPage = request.PageNumber ?? 1;
                        allDocuments = false;
                    }
                }

                List<DocsPaVO.ricerche.SearchResultInfo> idProfileList = null;
                List<DocsPaVO.ricerche.SearchResultInfo> toSet = new List<SearchResultInfo>();
                ArrayList objListaInfoDocumenti = null;
                numTotPage = 0;
                nRec = 0;
                #region Estrazione template da ricerca con filtro TEMPLATE_EXTRACTION
                // Tentativo di estrazione template per ricerca PIS
                bool estrazioneTemplate = false;
                Domain.Filter filtroTemplate = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "TEMPLATE") select filtro).FirstOrDefault();
                List<DocsPaVO.Grid.Field> visibilityFields = new List<DocsPaVO.Grid.Field>();
                if (filtroTemplate != null)
                {
                    Domain.Filter filtroEstrazione = (from filtro in request.Filters where (filtro != null && !string.IsNullOrEmpty(filtro.Name) && filtro.Name.ToUpper() == "TEMPLATE_EXTRACTION") select filtro).FirstOrDefault();
                    if (filtroEstrazione!=null && filtroEstrazione.Value.ToUpper() == "TRUE")
                    {
                        estrazioneTemplate = true;
                        DocsPaVO.Grid.Field x1 = new DocsPaVO.Grid.Field();
                        DocsPaVO.ProfilazioneDinamica.Templates template = null;
                        if (filtroTemplate != null)
                        {
                            int idTplTemp = 0;
                            if (Int32.TryParse(filtroTemplate.Value, out idTplTemp))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(idTplTemp.ToString());
                            
                            }else if (filtroTemplate.Template != null && !string.IsNullOrEmpty(filtroTemplate.Template.Id))
                            {
                                template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateById(filtroTemplate.Template.Id);
                            }
                            else
                            {
                                if (filtroTemplate.Template != null && !string.IsNullOrEmpty(filtroTemplate.Template.Name))
                                {
                                    template = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateByDescrizione(filtroTemplate.Template.Name, infoUtente.idAmministrazione);
                                }
                            }
                        }
                        if (template != null)
                        {
                            DocsPaVO.Grid.Field campoProf = null;
                            int i = 0;
                            foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in template.ELENCO_OGGETTI)
                            {
                                campoProf = new DocsPaVO.Grid.Field();
                                campoProf.AssociatedTemplateId = template.SYSTEM_ID.ToString();
                                campoProf.AssociatedTemplateName = template.DESCRIZIONE;
                                campoProf.CanAssumeMultiValues = false;
                                campoProf.CustomObjectId = ogg.SYSTEM_ID;
                                campoProf.FieldId = "T" + ogg.SYSTEM_ID;
                                campoProf.IsTruncable = true;
                                campoProf.Label = ogg.DESCRIZIONE;
                                campoProf.OriginalLabel = ogg.DESCRIZIONE;
                                campoProf.MaxLength = 1000000;
                                campoProf.OracleDbColumnName = ogg.SYSTEM_ID.ToString();
                                campoProf.SqlServerDbColumnName = ogg.SYSTEM_ID.ToString();
                                campoProf.Position = i;
                                i++;
                                campoProf.Visible = true;
                                campoProf.Width = 100;
                                visibilityFields.Add(campoProf);
                                
                            }
                        }
                        else
                        {
                            throw new PisException("TEMPLATE_NOT_FOUND");
                        }
                        
                    }
                }
                // fine estrazione template da ricerca
                #endregion
                if(!estrazioneTemplate)
                objListaInfoDocumenti = BusinessLogic.Documenti.InfoDocManager.getQueryPagingCustom(infoUtente, qV, numPage, pageSize, true, allDocuments, false, null, null, out numTotPage, out nRec, request.GetTotalDocumentsNumber, out toSet);
                else
                    objListaInfoDocumenti = BusinessLogic.Documenti.InfoDocManager.getQueryPagingCustom(infoUtente, qV, numPage, pageSize, true, allDocuments, true, visibilityFields.ToArray(), null, out numTotPage, out nRec, request.GetTotalDocumentsNumber, out toSet);
                
                idProfileList = toSet;

                if (objListaInfoDocumenti != null && objListaInfoDocumenti.Count > 0)
                {
                    responseDocuments = new Domain.Document[objListaInfoDocumenti.Count];
                    int y = 0;
                    foreach (DocsPaVO.Grids.SearchObject obj in objListaInfoDocumenti)
                    {
                        responseDocuments[y] = Utils.GetDocumentFromSearchObject(obj,estrazioneTemplate,visibilityFields.ToArray(),request.CodeAdm);
                        y++;
                    }

                    if (request.GetTotalDocumentsNumber && toSet != null)
                        response.TotalDocumentsNumber = toSet.Count;
                }
                else
                {
                    //Documenti non trovati
                    //throw new PisException("DOCUMENTS_NOT_FOUND");

                    if (request.GetTotalDocumentsNumber)
                        response.TotalDocumentsNumber = 0;

                    responseDocuments = new Domain.Document[0];
                }

                //if (request.GetTotalDocumentsNumber && toSet != null)
                //    response.TotalDocumentsNumber = toSet.Count;

                response.Documents = responseDocuments;
                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.SendDocument.SendDocumentResponse SendDocument(Services.Documents.SendDocument.SendDocumentRequest request)
        {
            Services.Documents.SendDocument.SendDocumentResponse response = new Services.Documents.SendDocument.SendDocumentResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "SendDocument");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.IdDocument) && !string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                //Reperimento ruolo utente
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                if (ruolo == null)
                {
                    //Ruolo non trovato
                    throw new PisException("ROLE_NO_EXIST");
                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.Signature))
                        {
                            documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(request.Signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                if (documento != null)
                {
                    ///TODO
                    DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizione = new DocsPaVO.Spedizione.SpedizioneDocumento();

                    //filtro dagli allegati da spedire quelli associati a notifiche di tipo PEC
                    ArrayList listAllegati = new ArrayList();
                    if (documento.allegati != null && documento.allegati.Count > 0)
                    {
                        foreach (Allegato a in documento.allegati)
                        {
                            if (a.versionId != null)
                            {
                                if (!(BusinessLogic.Documenti.AllegatiManager.getIsAllegatoPEC(a.versionId)).Equals("1") &&
                                    !(BusinessLogic.Documenti.AllegatiManager.getIsAllegatoIS(a.versionId)).Equals("1"))
                                    listAllegati.Add(a);
                            }
                        }
                        documento.allegati = listAllegati;
                    }

                    infoSpedizione = Utils.GetInfoSpedizione(documento, infoUtente);

                    List<Registro> listRegistriRF = new List<Registro>();
                    string mailspedizione = "";
                    if (documento.registro != null && !string.IsNullOrEmpty(documento.registro.systemId))
                    {
                        Registro reg = BusinessLogic.Utenti.RegistriManager.getRegistro(documento.registro.systemId);
                        ArrayList arrReg = BusinessLogic.Utenti.RegistriManager.getListaRegistriRfRuolo(infoUtente.idCorrGlobali, "1", documento.registro.systemId);
                        foreach (Registro registro in arrReg)
                        {
                            DataSet ds = BusinessLogic.Amministrazione.RegistroManager.GetRightRuoloMailRegistro(registro.systemId, ruolo.systemId);
                            if (ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                            {
                                foreach (DataRow row in ds.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                                {
                                    if (row["SPEDISCI"].ToString().Equals("1"))
                                    {
                                        listRegistriRF.Add(registro);
                                        mailspedizione= row["EMAIL_REGISTRO"].ToString();
                                        break;
                                    }
                                }
                            }
                        }

                        //prendo il registro corrente
                        DataSet dsReg = BusinessLogic.Amministrazione.RegistroManager.GetRightRuoloMailRegistro(documento.registro.systemId, ruolo.systemId);
                        if (dsReg.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows.Count > 0)
                        {
                            foreach (DataRow row in dsReg.Tables["RIGHT_RUOLO_MAIL_REGISTRI"].Rows)
                            {
                                if (row["SPEDISCI"].ToString().Equals("1"))
                                {
                                    listRegistriRF.Add(reg);
                                    mailspedizione = row["EMAIL_REGISTRO"].ToString();
                                    break;
                                }
                            }
                        }

                        if (listRegistriRF != null && listRegistriRF.Count > 0)
                        {
                            infoSpedizione.IdRegistroRfMittente = listRegistriRF[0].systemId;
                            infoSpedizione.mailAddress = listRegistriRF[0].email;
                            if (!string.IsNullOrEmpty(mailspedizione)) infoSpedizione.mailAddress = mailspedizione;
                        }
                        else
                        {
                            response.Success = false;
                        }

                        if (infoSpedizione != null)
                        {
                            if (infoSpedizione.DestinatariEsterni != null && infoSpedizione.DestinatariEsterni.Count > 0)
                            {
                                foreach (DestinatarioEsterno es in infoSpedizione.DestinatariEsterni)
                                {
                                    es.IncludiInSpedizione = true;
                                }
                            }

                            if (infoSpedizione.DestinatariInterni != null && infoSpedizione.DestinatariInterni.Count > 0)
                            {
                                foreach (DestinatarioInterno es in infoSpedizione.DestinatariInterni)
                                {
                                    es.IncludiInSpedizione = true;
                                }
                            }
                        }
                    }
                    logger.Debug("infoSpedizione.IdRegistroRfMittente = " + infoSpedizione.IdRegistroRfMittente);
                    logger.Debug("infoSpedizione.mailAddress =" + infoSpedizione.mailAddress);
                    infoSpedizione = BusinessLogic.Spedizione.SpedizioneManager.SpedisciDocumento(infoUtente, documento, infoSpedizione);
                    response.Success = infoSpedizione.Spedito;
                    if (!response.Success)
                    {
                        throw new PisException("SEND_DOCUMENT_FAILED");
                    }
                }
                else
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.GetDocumentsInProject.GetDocumentsInProjectResponse GetDocumentsInProject(Services.Documents.GetDocumentsInProject.GetDocumentsInProjectRequest request)
        {
            Services.Documents.GetDocumentsInProject.GetDocumentsInProjectResponse response = new Services.Documents.GetDocumentsInProject.GetDocumentsInProjectResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetDocumentsInProject");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) || !string.IsNullOrEmpty(request.CodeProject)) && !string.IsNullOrEmpty(request.IdProject))
                {
                    throw new PisException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject)) || (string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject)))
                {
                    throw new PisException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }


                Domain.Document[] responseDocuments = null;

                int nRec = 0;
                int numTotPage = 0;
                bool allDocuments = false;
                int pageSize = 20;
                int numPage = 1;

                if (request.ElementsInPage == null && request.PageNumber == null)
                {
                    allDocuments = true;
                }
                else
                {
                    if ((request.ElementsInPage != null && request.PageNumber == null) || (request.ElementsInPage == null && request.PageNumber != null))
                    {
                        //Paginazione non valida
                        throw new PisException("INVALID_PAGINATION");
                    }
                }

                ArrayList objListaDocumenti = null;
                List<DocsPaVO.ricerche.SearchResultInfo> idProfileList = null;
                DocsPaVO.fascicolazione.Folder folder = null;
                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
                DocsPaVO.fascicolazione.Fascicolo[] fascicoli = null;

                if (!string.IsNullOrEmpty(request.IdProject))
                {
                    fascicolo.systemID = request.IdProject;
                    folder = BusinessLogic.Fascicoli.FolderManager.getFolderByIdFascicolo(infoUtente.idPeople, infoUtente.idGruppo, fascicolo);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject))
                    {
                        if (request.CodeProject.IndexOf("//") > -1)
                        {
                            string separatore = "//";
                            // MODIFICA per inserimento in sottocartelle
                            string[] separatoreAr = new string[] { separatore };

                            string[] pathCartelle = request.CodeProject.Split(separatoreAr, StringSplitOptions.RemoveEmptyEntries);

                            fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloDaCodice(infoUtente, pathCartelle[0], null, false, false);

                            if (pathCartelle.Length > 1)
                            {
                                ArrayList cartelle = BusinessLogic.Fascicoli.FascicoloManager.getListaFolderDaIdFascicolo(infoUtente, fascicolo.systemID, null, false, false);
                                // caricamento folder base
                                string idParentFolder = fascicolo.systemID;
                                fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)((from DocsPaVO.fascicolazione.Folder f in cartelle where f.idParent == idParentFolder select f).ToList().FirstOrDefault());
                                idParentFolder = fascicolo.folderSelezionato.systemID;
                                try
                                {
                                    for (int i = 1; i < pathCartelle.Length; i++)
                                    {

                                        fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)((from DocsPaVO.fascicolazione.Folder f in cartelle where f.idParent == idParentFolder && f.descrizione == pathCartelle[i] select f).ToList().First());
                                        idParentFolder = fascicolo.folderSelezionato.systemID;
                                    }
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("Cartella non trovata nel fascicolo");
                                }
                            }
                            folder = fascicolo.folderSelezionato;

                            //int posSep = request.CodeProject.IndexOf("//");
                            //string codiceFascicolo = request.CodeProject.Substring(0, posSep);
                            //string descrFolder = request.CodeProject.Substring(posSep + separatore.Length);
                            //fascicoli = BusinessLogic.Fascicoli.FascicoloManager.getListaFolderDaCodiceFascicolo(infoUtente, codiceFascicolo, descrFolder, null, false, false);
                            //if (fascicoli != null && fascicoli.Count > 0)
                            //{
                            //    if (fascicoli.Count == 1)
                            //    {
                            //        //calcolo fascicolo sottofascicolo
                            //        fascicolo = BusinessLogic.Fascicoli.FascicoloManager.getFascicoloById(((DocsPaVO.fascicolazione.Folder)fascicoli[0]).idFascicolo, infoUtente);
                            //        if (fascicolo != null)
                            //        {
                            //            //folder selezionato è l'ultimo
                            //            fascicolo.folderSelezionato = (DocsPaVO.fascicolazione.Folder)fascicoli[fascicoli.Count - 1];
                            //        }
                            //    }
                            //    else
                            //    {
                            //        //Fascicoli multipli
                            //        throw new PisException("MULTIPLE");
                            //    }
                            //}
                        }
                        else
                        {
                            fascicoli = BusinessLogic.Fascicoli.FascicoloManager.GetFascicoloDaCodiceConSecurity(request.CodeProject, infoUtente.idAmministrazione, request.ClassificationSchemeId, false, infoUtente);
                            if (fascicoli != null && fascicoli.Length > 0)
                            {
                                DocsPaVO.fascicolazione.Fascicolo searchFasc = null;

                                if (fascicoli.Length == 1)
                                {
                                    searchFasc = fascicoli[0];
                                }
                                else
                                {
                                    searchFasc = fascicoli[1];
                                }

                                folder = BusinessLogic.Fascicoli.FolderManager.getFolderByIdFascicolo(infoUtente.idPeople, infoUtente.idGruppo, searchFasc);
                            }
                        }
                    }
                }

                if (folder != null)
                {
                    // Lista dei fitri da restituire
                    DocsPaVO.filtri.FiltroRicerca[][] orderRicerca = new FiltroRicerca[1][];
                    orderRicerca = new FiltroRicerca[1][];
                    orderRicerca[0] = new FiltroRicerca[1];
                    FiltroRicerca[] fVlist = new FiltroRicerca[3];
                    fVlist[0] = new FiltroRicerca()
                    {
                        argomento = "ORACLE_FIELD_FOR_ORDER",
                        valore = "NVL (a.dta_proto, a.creation_time)",
                        nomeCampo = "D9",
                    };
                    fVlist[1] = new FiltroRicerca()
                    {
                        argomento = "SQL_FIELD_FOR_ORDER",
                        valore = "ISNULL (a.dta_proto, a.creation_time)",
                        nomeCampo = "D9",
                    };
                    fVlist[2] = new FiltroRicerca()
                    {
                        argomento = "ORDER_DIRECTION",
                        valore = "DESC",
                        nomeCampo = "D9",
                    };

                    orderRicerca[0] = fVlist;

                    objListaDocumenti = BusinessLogic.Fascicoli.FolderManager.getDocumentiPagingCustom(infoUtente, folder, null, numPage, out numTotPage, out nRec, request.GetTotalDocumentsNumber, out idProfileList, false, allDocuments, null, null, pageSize, orderRicerca);

                    if (objListaDocumenti != null && objListaDocumenti.Count > 0)
                    {
                        responseDocuments = new Domain.Document[objListaDocumenti.Count];
                        int y = 0;
                        foreach (DocsPaVO.Grids.SearchObject obj in objListaDocumenti)
                        {
                            responseDocuments[y] = Utils.GetDocumentFromSearchObject(obj,false,null,request.CodeAdm);
                            y++;
                        }
                    }
                    else
                    {
                        //Documenti non trovati
                        throw new PisException("DOCUMENTS_NOT_FOUND");
                    }
                }
                else
                {
                    //Fascicolo non trovato
                    throw new PisException("PROJECT_NOT_FOUND");
                }

                if (request.GetTotalDocumentsNumber && idProfileList != null)
                    response.TotalDocumentsNumber = idProfileList.Count;

                response.Documents = responseDocuments;
                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.UploadFileToDocument.UploadFileToDocumentResponse UploadFileToDocument(Services.Documents.UploadFileToDocument.UploadFileToDocumentRequest request)
        {
            Services.Documents.UploadFileToDocument.UploadFileToDocumentResponse response = new Services.Documents.UploadFileToDocument.UploadFileToDocumentResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "UploadFileToDocument");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new PisException("REQUIRED_ID");
                }

                if (request.File == null)
                {
                    throw new PisException("REQUIRED_FILE");
                }

                if (string.IsNullOrEmpty(request.Description))
                {
                    throw new PisException("REQUIRED_DESCRIPTION");
                }

                //
                // Controllo HashFile
                string hash = string.Empty;
                byte[] contentFile = null;
                string hashObbligatorio = string.Empty;
                //Chiave per abilitare l'utilizzo dell'hash obbligatorio
                hashObbligatorio = System.Configuration.ConfigurationManager.AppSettings["HASH_OBBLIGATORIO"];

                if (!string.IsNullOrEmpty(hashObbligatorio) && hashObbligatorio.Equals("1"))
                {
                    // Hash obbligatorio
                    if (string.IsNullOrEmpty(request.HashFile))
                    {
                        throw new Exception("Hash del file obbligatorio.");
                    }
                    else
                    {
                        hash = request.HashFile;
                    }
                    int maxfilesize = Utils.maxFileSizePermitted();
                    if (request.File.Content.Length <= 0)
                    {
                        throw new Exception("Content File obbligatorio.");
                    }
                    else if (maxfilesize > 0 && request.File.Content.Length > maxfilesize)
                    {
                        throw new Exception("Dimensione del file troppo grande");
                    }
                    else
                    {
                        contentFile = request.File.Content;
                    }

                    //
                    // Controllo Hash del File
                    if (!string.IsNullOrEmpty(hash) && contentFile != null)
                    {
                        System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
                        byte[] impronta = sha.ComputeHash(contentFile);
                        string impr = BitConverter.ToString(impronta).Replace("-", "");

                        if (!impr.Equals(hash))
                        {
                            throw new Exception("Hash del file non corrispondente.");
                        }
                    }

                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                string erroreMessage = string.Empty;

                
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                        if (documento != null)
                        {
                            //Crea un nuovo allegato
                            if (request.CreateAttachment)
                            {
                                DocsPaVO.documento.FileRequest allegato = new DocsPaVO.documento.Allegato
                                {
                                    docNumber = request.IdDocument,
                                    descrizione = request.Description
                                };
                                try
                                {
                                    // Acquisizione dell'allegato
                                    allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, ((DocsPaVO.documento.Allegato)allegato));
                                }
                                catch (Exception exallegato)
                                {
                                    logger.Error(exallegato);
                                    throw new PisException("ERROR_ADD_ATTACHMENT");
                                }
                                    //definisco l'allegato come esterno
                                try
                                {
                                    if (!string.IsNullOrEmpty(request.AttachmentType) && request.AttachmentType == "E")
                                    {
                                        if (!BusinessLogic.Documenti.AllegatiManager.setFlagAllegatiEsterni(allegato.versionId, allegato.docNumber))
                                        {
                                            BusinessLogic.Documenti.AllegatiManager.rimuoviAllegato((DocsPaVO.documento.Allegato)allegato, infoUtente);
                                            throw new PisException("ERROR_ADD_ATTACHMENT");
                                        }
                                    }
                                }
                                catch (PisException pisEx)
                                {
                                    response.Error = new Services.ResponseError
                                    {
                                        Code = pisEx.ErrorCode,
                                        Description = pisEx.Description
                                    };

                                    response.Success = false;
                                    return response;
                                }

                                // Acquisizione file allegato
                                // 5. Acquisizione del file al documento
                                DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                                {
                                    name = request.File.Name,
                                    fullName = request.File.Name,
                                    content = request.File.Content,
                                    contentType = request.File.MimeType,
                                    length = request.File.Content.Length,
                                    bypassFileContentValidation = true
                                };


                                if (!BusinessLogic.Documenti.FileManager.putFile(ref allegato, fileDocumento, infoUtente, out erroreMessage))
                                {
                                    throw new PisException("FILE_CREATION_ERROR");
                                }
                            }
                            else
                            {
                                //Crea il file o se esistente una nuova versione
                                // 5. Acquisizione del file al documento
                                DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)documento.documenti[0];

                                DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                                {
                                    name = request.File.Name,
                                    fullName = request.File.Name,
                                    content = request.File.Content,
                                    contentType = request.File.MimeType,
                                    length = request.File.Content.Length,
                                    bypassFileContentValidation = true
                                };

                                string errorMessage;

                                //Se esiste già una versione del file ne creo una nuova
                                if (versioneCorrente != null && !string.IsNullOrEmpty(versioneCorrente.fileName))
                                {
                                    DocsPaVO.documento.FileRequest fileRequest = new DocsPaVO.documento.Allegato
                                    {
                                        docNumber = request.IdDocument,
                                        descrizione = request.Description
                                    };

                                    versioneCorrente = BusinessLogic.Documenti.VersioniManager.addVersion(fileRequest, infoUtente, false);
                                }

                                if (!BusinessLogic.Documenti.FileManager.putFile(ref versioneCorrente, fileDocumento, infoUtente, out errorMessage))
                                {
                                    throw new PisException("FILE_CREATION_ERROR");
                                }
                            }
                        }
                        else
                        {
                            throw new PisException("DOCUMENT_NOT_FOUND");
                        }
                    }
                    else
                    {
                        throw new PisException("DOCUMENT_NOT_FOUND");
                    }
                
                

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.GetStampAndSignature.GetStampAndSignatureResponse GetStampAndSignature(Services.Documents.GetStampAndSignature.GetStampAndSignatureRequest request)
        {
            Services.Documents.GetStampAndSignature.GetStampAndSignatureResponse response = new Services.Documents.GetStampAndSignature.GetStampAndSignatureResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetStampAndSignature");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.IdDocument) && !string.IsNullOrEmpty(request.Signature))
                {
                    throw new PisException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.Signature))
                        {
                            documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(request.Signature, infoUtente);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                try
                {

                    if (documento != null)
                    {
                        response.Stamp = Utils.GetStampAndSignature(documento, infoUtente);

                    }
                    else
                    {
                        throw new PisException("DOCUMENT_NOT_FOUND");
                    }


                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.GetDocumentFilters.GetDocumentFiltersResponse GetDocumentFilters(Services.Documents.GetDocumentFilters.GetDocumentFiltersRequest request)
        {
            Services.Documents.GetDocumentFilters.GetDocumentFiltersResponse response = new Services.Documents.GetDocumentFilters.GetDocumentFiltersResponse();

            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetDocumentFilters");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                List<Domain.Filter> listaFiltri = new List<Domain.Filter>();

                listaFiltri.Add(new Domain.Filter() { Name = "YEAR", Description = "Inserire il valore dell’anno dei documenti", Type = Domain.FilterTypeEnum.Number });
                listaFiltri.Add(new Domain.Filter() { Name = "IN_PROTOCOL", Description = "Con valore true cerca i protocolli in entrata", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "OUT_PROTOCOL", Description = "Con valore true cerca i protocolli in uscita", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "INTERNAL_PROTOCOL", Description = "Con valore true cerca i protocolli interni", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "NOT_PROTOCOL", Description = "Con valore true cerca i documenti non protocollati", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "PREDISPOSED", Description = "Con valore true cerca i predisposti", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "ATTACHMENTS", Description = "Con valore true cerca gli allegati", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "PRINTS", Description = "Con valore true cerca le stampe", Type = Domain.FilterTypeEnum.Bool });
                listaFiltri.Add(new Domain.Filter() { Name = "NUM_PROTOCOL_FROM", Description = "Filtro utilizzato per la ricerca per intervallo su numero di protocollo, questo valore è il limite inferiore", Type = Domain.FilterTypeEnum.Number });
                listaFiltri.Add(new Domain.Filter() { Name = "NUM_PROTOCOL_TO", Description = "Filtro utilizzato per la ricerca per intervallo su numero di protocollo, questo valore è il limite superiore", Type = Domain.FilterTypeEnum.Number });
                listaFiltri.Add(new Domain.Filter() { Name = "CREATION_DATE_FROM", Description = "Filtro utilizzato per intervallo su data creazione, questo valore è il limite inferiore", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "CREATION_DATE_TO", Description = "Filtro utilizzato per intervallo su data creazione, questo valore è il limite superiore", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "PROTOCOL_DATE_FROM", Description = "Filtro utilizzato per intervallo su data protocollo, questo valore è il limite inferiore", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "PROTOCOL_DATE_TO", Description = "Filtro utilizzato per intervallo su data protocollo, questo valore è il limite superiore", Type = Domain.FilterTypeEnum.Date });
                listaFiltri.Add(new Domain.Filter() { Name = "SENDER_RECIPIENT", Description = "Filtro utilizzato sui mittenti/destinatari di un documento, inserire l’id del corrispondente", Type = Domain.FilterTypeEnum.Number });
                listaFiltri.Add(new Domain.Filter() { Name = "TEMPLATE", Description = "Filtro utilizzato per la ricerca della tipologia dei documenti", Type = Domain.FilterTypeEnum.Number });
                listaFiltri.Add(new Domain.Filter() { Name = "DOCNUMBER_FROM", Description = "Filtro utilizzato per intervallo sul docnumner questo valore è il limite inferiore", Type = Domain.FilterTypeEnum.Number });
                listaFiltri.Add(new Domain.Filter() { Name = "DOCNUMBER_TO", Description = "Filtro utilizzato per intervallo sul docnumner questo valore è il limite superiore", Type = Domain.FilterTypeEnum.Number });
                listaFiltri.Add(new Domain.Filter() { Name = "REGISTER", Description = "Filtro utilizzato con il codice del registro", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "OBJECT", Description = "Filtro utilizzato per la ricerca per oggetto di un documento", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "FULL_TEXT_SEARCH", Description = "Filtro utilizzato per la ricerca FullText", Type = Domain.FilterTypeEnum.String });
                listaFiltri.Add(new Domain.Filter() { Name = "TEMPLATE_EXTRACTION", Description = "Filtro utilizzato per l'estrazione dei campi profilati di una tipologia. Necessita della presenza del filtro Template.", Type = Domain.FilterTypeEnum.String });

                response.Filters = listaFiltri.ToArray();

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.CreateDocumentAndAddInProject.CreateDocumentAndAddInProjectResponse CreateDocumentAndAddInProject(Services.Documents.CreateDocumentAndAddInProject.CreateDocumentAndAddInProjectRequest request)
        {
            Services.Documents.CreateDocumentAndAddInProject.CreateDocumentAndAddInProjectResponse response = new Services.Documents.CreateDocumentAndAddInProject.CreateDocumentAndAddInProjectResponse();

            try
            {
                bool inserimentoInSottoFasc = false;
                // Verifica l'esistenza del fascicolo
                //
                VtDocsWS.Services.Projects.GetProject.GetProjectRequest getProjectRequest = new Services.Projects.GetProject.GetProjectRequest();
                getProjectRequest.UserName = request.UserName;
                getProjectRequest.CodeAdm = request.CodeAdm;
                getProjectRequest.AuthenticationToken = request.AuthenticationToken;
                getProjectRequest.CodeRoleLogin = request.CodeRoleLogin;
                getProjectRequest.IdProject = request.IdProject;
                if (string.IsNullOrEmpty(request.CodeProject))
                {
                    getProjectRequest.CodeProject = request.CodeProject;
                }
                else
                {
                    if (request.CodeProject.IndexOf("//") > -1)
                    {
                        string separatore = "//";
                        // MODIFICA per inserimento in sottocartelle
                        string[] separatoreAr = new string[] { separatore };

                        string[] pathCartelle = request.CodeProject.Split(separatoreAr, StringSplitOptions.RemoveEmptyEntries);
                        getProjectRequest.CodeProject = pathCartelle[0];
                        inserimentoInSottoFasc = true;
                    }else
                        getProjectRequest.CodeProject = request.CodeProject;
                }
                getProjectRequest.ClassificationSchemeId = request.ClassificationSchemeId;

                VtDocsWS.Services.Projects.GetProject.GetProjectResponse getProjectResponse = ProjectsManager.GetProject(getProjectRequest);

                //if (getProjectResponse != null && getProjectResponse.Success && getProjectResponse.Project != null && !getProjectResponse.Project.Open)
                //{
                //    throw new PisException("PROJECT_NOT_FOUND");
                //}

                if (!getProjectResponse.Success)
                    throw new PisException(getProjectResponse.Error.Code);

                    // Se il fascicolo esiste, crea il documento e lo inserisce nel fascicolo
                    //
                if (getProjectResponse != null && getProjectResponse.Success && getProjectResponse.Project != null)
                {
                    // Crea il documento
                    //
                    VtDocsWS.Services.Documents.CreateDocument.CreateDocumentRequest createDocRequest = new Services.Documents.CreateDocument.CreateDocumentRequest();
                    createDocRequest.UserName = request.UserName;
                    createDocRequest.CodeAdm = request.CodeAdm;
                    createDocRequest.AuthenticationToken = request.AuthenticationToken;
                    createDocRequest.CodeRegister = request.CodeRegister;
                    createDocRequest.Document = request.Document;
                    createDocRequest.CodeRF = request.CodeRF;
                    createDocRequest.CodeRoleLogin = request.CodeRoleLogin;

                    VtDocsWS.Services.Documents.CreateDocument.CreateDocumentResponse createDocResponse = DocumentsManager.CreateDocument(createDocRequest, true);

                    if (!createDocResponse.Success)
                    {
                        // Gestione errori Elaborazione XML
                        if (createDocResponse.Error.Code != "APPLICATION_ERROR")
                            throw new PisException(createDocResponse.Error.Code);
                        else
                            throw new Exception(createDocResponse.Error.Description);
                    }
                    // Inserisce nel fascicolo
                    //
                    VtDocsWS.Services.Documents.AddDocInProject.AddDocInProjectRequest addDocInProjectRequest = new Services.Documents.AddDocInProject.AddDocInProjectRequest();
                    addDocInProjectRequest.UserName = request.UserName;
                    addDocInProjectRequest.CodeAdm = request.CodeAdm;
                    addDocInProjectRequest.CodeRoleLogin = request.CodeRoleLogin;
                    addDocInProjectRequest.AuthenticationToken = request.AuthenticationToken;
                    if (!inserimentoInSottoFasc)
                    {
                        addDocInProjectRequest.IdProject = getProjectResponse.Project.Id;
                    }
                    else
                    {
                        addDocInProjectRequest.CodeProject = request.CodeProject;                        
                    }
                    addDocInProjectRequest.IdDocument = createDocResponse.Document.Id;
                    try
                    {
                        VtDocsWS.Services.Documents.AddDocInProject.AddDocInProjectResponse addDocInProjectResponse = DocumentsManager.AddDocInProject(addDocInProjectRequest);
                    }
                    catch (PisException PisExAddDoc)
                    {
                        string errMsg = String.Format("Errore durante AddDocInProject. Messaggio:\"{0}\". Il documento con id {1} dovrà essere aggiunto al fascicolo in un secondo momento", PisExAddDoc.Message, createDocResponse.Document.Id);
                        throw new Exception(errMsg);
                    }
                    catch (Exception ExAddDoc)
                    {
                        string errMsg = String.Format("Errore durante AddDocInProject. Messaggio:\"{0}\". Il documento con id {1} dovrà essere aggiunto al fascicolo in un secondo momento", ExAddDoc.Message, createDocResponse.Document.Id);
                        throw new Exception(errMsg);                    
                    }
                    //if (!getProjectResponse.Success)
                    //    throw new PisException(addDocInProjectResponse.Error.Code);

                    response.Document = createDocResponse.Document;
                }

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        /// <summary>
        /// Servizio che permette alla coppia (ruolo,utente) di monitorare un documento presente nel sistema.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Services.FollowDomainObject.FollowResponse FollowDocument(Services.FollowDomainObject.FollowRequest request)
        {
            Services.FollowDomainObject.FollowResponse response = new Services.FollowDomainObject.FollowResponse();
            try
            {
                //Controllo se l'utente si è correttamente autenticatoInizio controllo autenticazione utente
                DocsPaVO.utente.InfoUtente userInfo = Utils.CheckAuthentication(request, "FollowDocument");

                if (request.Operation == null || string.IsNullOrEmpty(request.CodeApplication) || string.IsNullOrEmpty(request.IdObject))
                {
                    throw new Exception("I campi Operation, CodeApplication, IdObject sono obbligatori.");
                }
                // Costruisco l'oggetto FollowDomainObject
                DocsPaVO.Notification.FollowDomainObject followDomainObject = new DocsPaVO.Notification.FollowDomainObject();
                switch (request.Operation)
                {
                    case Domain.OperationFollow.AddDoc:
                        followDomainObject.Operation = DocsPaVO.Notification.FollowDomainObject.OperationFollow.AddDoc;
                        break;
                    case Domain.OperationFollow.RemoveDoc:
                        followDomainObject.Operation = DocsPaVO.Notification.FollowDomainObject.OperationFollow.RemoveDoc;
                        break;
                    default:
                        throw new Exception("Il campo operazione deve essere AddDocument o RemoveDocument");
                }
                followDomainObject.App = request.CodeApplication;
                followDomainObject.IdAmm = userInfo.idAmministrazione;
                followDomainObject.IdProfile = request.IdObject;
                followDomainObject.IdPeople = userInfo.idPeople;
                followDomainObject.IdGroup = userInfo.idGruppo;

                //eseguo l'operazione di monitora/non monitorare più il documento.
                BusinessLogic.ServiceNotifications.Notification.Follow(followDomainObject);
                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }


        /// <summary>
        /// Consente di Annullare un Documento Protocollato
        /// </summary>
        /// <param name="Request">In input l'oggetto InvalidDocumentRequest</param>
        /// <returns>Restituisce l'oggetto InvalidDocumentResponse</returns>
        public static Services.Documents.InvalidateDocument.InvalidateDocumentResponse InvalidateDocument(Services.Documents.InvalidateDocument.InvalidateDocumentRequest Request)
        {
            Services.Documents.InvalidateDocument.InvalidateDocumentResponse Response = new Services.Documents.InvalidateDocument.InvalidateDocumentResponse();
            Response.Success = false;
            try
            {
                InfoUtente infoUtente = Utils.CheckAuthentication(Request, "InvalidateDocument");
                DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.ProtoManager.ricercaScheda(Request.Segnature, infoUtente);
                if (schedaDocumento != null && !string.IsNullOrEmpty(schedaDocumento.accessRights))
                {
                    int accessRights = -1;
                    int.TryParse(schedaDocumento.accessRights, out accessRights);
                    //verifica dei diritti
                    if (accessRights >= new DocsPaVO.HMDiritti.HMdiritti().HMdiritti_Write ||
                        accessRights >= new DocsPaVO.HMDiritti.HMdiritti().HMdiritti_Eredita)
                    {
                        DocsPaVO.documento.ProtocolloAnnullato protoAnn = new DocsPaVO.documento.ProtocolloAnnullato();
                        protoAnn.autorizzazione = Request.Description;

                        //AGGIUNTO SAB  TRY e CATCH
                        try
                        {

                            Response.Success = BusinessLogic.Documenti.ProtoManager.annullaProtocollo(infoUtente, ref schedaDocumento, protoAnn);
                        }
                        catch (Exception e)
                        {
                            new Exception("errore annullamento: " + e.Message);
                        }

                        if (!Response.Success)
                            Response.Error = new PisException("INVALIDATE_DOCUMENT_ERROR");
                    }
                    else
                        Response.Error = new PisException("INVALIDATE_DOCUMENT_NO_DIRITTI");

                }
                else
                    Response.Error = new PisException("DOCUMENT_NOT_FOUND");

                schedaDocumento = null;
                infoUtente = null;
            }
            catch (PisException pisex)
            {
                Response.Error = pisex;
            }
            return Response;
        }

        public static Services.Documents.GetLinkDocByID.GetLinkDocByIDResponse GetLinkDocByID(Services.Documents.GetLinkDocByID.GetLinkDocByIDRequest request)
        {
            Services.Documents.GetLinkDocByID.GetLinkDocByIDResponse response = new Services.Documents.GetLinkDocByID.GetLinkDocByIDResponse();
            try
            {
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;
                DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizione = new DocsPaVO.Spedizione.SpedizioneDocumento();
                response.Success = false;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetLinkDocByID");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente



                //Reperimento ruolo utente
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                if (ruolo == null)
                {
                    //Ruolo non trovato
                    throw new PisException("ROLE_NO_EXIST");
                }
                // Fine reperimento ruolo

                // Controllo documento
                //if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                //{
                //    throw new PisException("REQUIRED_ID_OR_SIGNATURE");
                //}

                //if (!string.IsNullOrEmpty(request.IdDocument) && !string.IsNullOrEmpty(request.Signature))
                //{
                //    throw new PisException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                //}
                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new PisException("REQUIRED_ID");
                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    }
                    //else
                    //{
                    //    if (!string.IsNullOrEmpty(request.Signature))
                    //    {
                    //        documento = BusinessLogic.Documenti.DocManager.GetDettaglioBySignature(request.Signature, infoUtente);
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                string pathFE = "";
                if (System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"] != null)
                    pathFE = System.Configuration.ConfigurationManager.AppSettings["URL_PATH_IS"].ToString();

                if (!string.IsNullOrEmpty(pathFE))
                {
                    response.DocumentLink = string.Format("{0}CheckInOut/OpenDirectLink.aspx?idAmministrazione={1}&idObj={2}&tipoProto={3}&from=record", pathFE, infoUtente.idAmministrazione, documento.systemId, documento.tipoProto);
                    response.Success = true;
                }
                else
                {
                    throw new Exception("Path del frontend non disponibile: impossibile generare il link.");
                }
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }
            return response;
        }

        public static Services.Documents.GetEnvelopedFileById.GetEnvelopedFileByIdResponse GetEnvelopedFileById(Services.Documents.GetEnvelopedFileById.GetEnvelopedFileByIdRequest request)
        {
            Services.Documents.GetEnvelopedFileById.GetEnvelopedFileByIdResponse response = new Services.Documents.GetEnvelopedFileById.GetEnvelopedFileByIdResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "GetEnvelopedFileById");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new PisException("REQUIRED_ID");
                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();
                bool FileConFirma = !request.GetFileWithoutEnvelope;
                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                        if (documento != null && documento.documenti != null && documento.documenti.Count > 0)
                        {
                            int numVersione = 0;
                            if (!string.IsNullOrEmpty(request.VersionId))
                            {
                                
                                    bool result = Int32.TryParse(request.VersionId, out numVersione);
                                    if (!result)
                                    {
                                        throw new PisException("REQUIRED_INTEGER");
                                    }
                                    else
                                    {
                                        if (documento.documenti.Count < numVersione || numVersione <= 0)
                                        {
                                            throw new PisException("FILE_VERSION_NOT_FOUND");
                                        }
                                        else
                                        {
                                            numVersione = documento.documenti.Count - numVersione;
                                        }
                                    }
                                
                            }
                            DocsPaVO.documento.FileRequest versione = (DocsPaVO.documento.FileRequest)documento.documenti[numVersione];
                            response.File = Utils.GetFile(versione, true, infoUtente, false, false, string.Empty, null, FileConFirma);
                            response.OriginalFileName = response.File.Name;
                            if (request.GetOnlyFileInfo) response.File.Content = null;
                            if (!string.IsNullOrEmpty(versione.firmato) && versione.firmato == "1") response.IsDocumentSigned = true;
                            else response.IsDocumentSigned = false;

                            response.FileExtension = versione.fileName.Substring(versione.fileName.LastIndexOf('.'), (versione.fileName.Length - versione.fileName.LastIndexOf('.')));
                            
                        }
                        else
                        {
                            throw new PisException("DOCUMENT_NOT_FOUND");
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }

                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }

        public static Services.Documents.AddAttachment.AddAttachmentResponse AddAttachment(Services.Documents.AddAttachment.AddAttachmentRequest request)
        {
            Services.Documents.AddAttachment.AddAttachmentResponse response = new Services.Documents.AddAttachment.AddAttachmentResponse();
            try
            {

                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "UploadFileToDocument");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new PisException("REQUIRED_ID");
                }

                if (request.File == null)
                {
                    throw new PisException("REQUIRED_FILE");
                }

                if (string.IsNullOrEmpty(request.Description))
                {
                    throw new PisException("REQUIRED_DESCRIPTION");
                }

                //
                // Controllo HashFile
                string hash = string.Empty;
                byte[] contentFile = null;
                string hashObbligatorio = string.Empty;
                //Chiave per abilitare l'utilizzo dell'hash obbligatorio
                hashObbligatorio = System.Configuration.ConfigurationManager.AppSettings["HASH_OBBLIGATORIO"];

                if (!string.IsNullOrEmpty(hashObbligatorio) && hashObbligatorio.Equals("1"))
                {
                    // Hash obbligatorio
                    if (string.IsNullOrEmpty(request.HashFile))
                    {
                        throw new Exception("Hash del file obbligatorio.");
                    }
                    else
                    {
                        hash = request.HashFile;
                    }
                    int maxfilesize = Utils.maxFileSizePermitted();
                    if (request.File.Content.Length <= 0)
                    {
                        throw new Exception("Content File obbligatorio.");
                    }
                    else if (maxfilesize > 0 && request.File.Content.Length > maxfilesize)
                    {
                        throw new Exception("Dimensione del file troppo grande");
                    }
                    else
                    {
                        contentFile = request.File.Content;
                    }

                    //
                    // Controllo Hash del File
                    if (!string.IsNullOrEmpty(hash) && contentFile != null)
                    {
                        System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
                        byte[] impronta = sha.ComputeHash(contentFile);
                        string impr = BitConverter.ToString(impronta).Replace("-", "");

                        if (!impr.Equals(hash))
                        {
                            throw new Exception("Hash del file non corrispondente.");
                        }
                    }

                }

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                string erroreMessage = string.Empty;


                if (!string.IsNullOrEmpty(request.IdDocument))
                {
                    documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    if (documento != null)
                    {
                        //Crea un nuovo allegato

                        DocsPaVO.documento.FileRequest allegato = new DocsPaVO.documento.Allegato
                        {
                            docNumber = request.IdDocument,
                            descrizione = request.Description
                        };

                        // Acquisizione dell'allegato
                        allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, ((DocsPaVO.documento.Allegato)allegato));
                        //definisco l'allegato come esterno
                        try
                        {
                            if (!string.IsNullOrEmpty(request.AttachmentType) && request.AttachmentType == "E")
                            {
                                if (!BusinessLogic.Documenti.AllegatiManager.setFlagAllegatiEsterni(allegato.versionId, allegato.docNumber))
                                {
                                    BusinessLogic.Documenti.AllegatiManager.rimuoviAllegato((DocsPaVO.documento.Allegato)allegato, infoUtente);
                                    throw new PisException("ERROR_ADD_ATTACHMENT");
                                }
                            }
                        }
                        catch (PisException pisEx)
                        {
                            response.Error = new Services.ResponseError
                            {
                                Code = pisEx.ErrorCode,
                                Description = pisEx.Description
                            };

                            response.Success = false;
                            return response;
                        }

                        // Acquisizione file allegato
                        // 5. Acquisizione del file al documento
                        DocsPaVO.documento.FileDocumento fileDocumento = new DocsPaVO.documento.FileDocumento
                        {
                            name = request.File.Name,
                            fullName = request.File.Name,
                            content = request.File.Content,
                            contentType = request.File.MimeType,
                            length = request.File.Content.Length,
                            bypassFileContentValidation = true
                        };


                        if (!BusinessLogic.Documenti.FileManager.putFile(ref allegato, fileDocumento, infoUtente, out erroreMessage))
                        {
                            throw new PisException("FILE_CREATION_ERROR");
                        }
                        else
                        {
                            response.IdObject = allegato.docNumber;
                        }
                        
                    }
                    else
                    {
                        throw new PisException("DOCUMENT_NOT_FOUND");
                    }
                }
                else
                {
                    throw new PisException("DOCUMENT_NOT_FOUND");
                }



                response.Success = true;
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }
    }
}
