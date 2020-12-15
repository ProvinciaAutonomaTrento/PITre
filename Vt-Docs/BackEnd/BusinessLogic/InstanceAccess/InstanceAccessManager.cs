using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using log4net;
using DocsPaVO.InstanceAccess;
using DocsPaVO.documento;
using System.IO;
using BusinessLogic.Report;
using System.Reflection;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;

namespace BusinessLogic.InstanceAccess
{
    public class InstanceAccessManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(InstanceAccessManager));
        public static Dictionary<string, string> infoDoc = new Dictionary<string, string>();
        /// <summary>
        /// Restituisce la lista di tutte le istanze d'accesso
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGroup"></param>
        /// <returns></returns>
        public static List<DocsPaVO.InstanceAccess.InstanceAccess> GetInstanceAccess(string idPeople, string idGroup, DocsPaVO.utente.InfoUtente infoUtente)
        {
            List<DocsPaVO.InstanceAccess.InstanceAccess> listInstanceAccess = new List<DocsPaVO.InstanceAccess.InstanceAccess>();
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                listInstanceAccess = instanceAccessDB.GetInstanceAccess(idPeople, idGroup, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: GetInstanceAccess ", e);
            }
            return listInstanceAccess;
        }

        /// <summary>
        /// Ritorna l'istanza di accesso avente l'id specificato
        /// </summary>
        /// <param name="idInstanceAccess"></param>
        /// <returns></returns>
        public static DocsPaVO.InstanceAccess.InstanceAccess GetInstanceAccessById(string idInstanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.InstanceAccess.InstanceAccess instanceAccess = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                instanceAccess = instanceAccessDB.GetInstanceAccessById(idInstanceAccess, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: GetInstanceAccessById ", e);
            }
            return instanceAccess;
        }

        /// <summary>
        /// Inserisce una nuova istanza d'accesso
        /// </summary>
        /// <param name="instanceAccess"></param>
        /// <returns></returns>
        public static DocsPaVO.InstanceAccess.InstanceAccess InsertInstanceAccess(DocsPaVO.InstanceAccess.InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.InstanceAccess.InstanceAccess instance = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                instance = instanceAccessDB.InsertInstanceAccess(instanceAccess, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: InsertInstanceAccess ", e);
            }
            return instance;
        }

        /// <summary>
        /// Inserisce la lista di documenti relativi ad un'istanza d'accesso
        /// </summary>
        /// <param name="listInstanceAccessDocuments"></param>
        /// <returns></returns>
        public static bool InsertInstanceAccessDocuments(List<InstanceAccessDocument> listInstanceAccessDocuments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                result = instanceAccessDB.InsertInstanceAccessDocuments(listInstanceAccessDocuments, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: InsertInstanceAccessDocuments ", e);
            }
            return result;
        }

        /// <summary>
        /// Inserisce la lista degli allegati di un documento appartenente ad un'istanza d'accesso
        /// </summary>
        /// <param name="listInstanceAccessAttachments"></param>
        /// <returns></returns>
        public static bool InsertInstanceAccessAttachments(List<InstanceAccessAttachments> listInstanceAccessAttachments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                result = instanceAccessDB.InsertInstanceAccessAttachments(listInstanceAccessAttachments, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: InsertInstanceAccessAttachments ", e);
            }
            return result;
        }

        /// <summary>
        /// Rimuove la lista di documenti legati ad un'istanza di accesso
        /// </summary>
        /// <param name="listInstanceAccessDocuments"></param>
        /// <returns></returns>
        public static bool RemoveInstanceAccessDocuments(List<InstanceAccessDocument> listInstanceAccessDocuments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                 // Contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                    result = instanceAccessDB.RemoveInstanceAccessDocuments(listInstanceAccessDocuments, infoUtente);

                    //se stò rimuovendo la dichiarazione di conformità, rumuovo definitivamente il documento
                    if (result)
                    {
                        DocsPaVO.ProfilazioneDinamica.Templates template = GetTemplate(infoUtente);
                        if (template != null)
                        {
                            InstanceAccessDocument doc = (from d in listInstanceAccessDocuments
                                                          where d.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(template.DESCRIZIONE)
                                                          select d).FirstOrDefault();
                            if (doc != null)
                            {
                                SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, doc.INFO_DOCUMENT.DOCNUMBER, doc.INFO_DOCUMENT.DOCNUMBER);
                                if (schedaDoc != null)
                                {
                                    string note = "Dichiarazione rimossa dall'utente " + infoUtente.userId;
                                    string error = string.Empty;
                                    result = BusinessLogic.Documenti.DocManager.CestinaDocumento(infoUtente, schedaDoc, schedaDoc.tipoProto, note, out error);
                                }
                            }
                        }
                    }
                    if (result)
                    {
                        transactionContext.Complete();
                    }
                }

            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: RemoveInstanceAccessDocuments ", e);
            }
            return result;
        }

        /// <summary>
        /// Rimuove la lista di allegati di un documento appartenente ad un'istanza di accesso
        /// </summary>
        /// <param name="listInstanceAccessAttachments"></param>
        /// <returns></returns>
        public static bool RemoveInstanceAccessAttachments(List<InstanceAccessAttachments> listInstanceAccessAttachments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                result = instanceAccessDB.RemoveInstanceAccessAttachments(listInstanceAccessAttachments, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: RemoveInstanceAccessAttachments ", e);
            }
            return result;
        }

        /// <summary>
        /// Aggiorna l'istanza d'accesso
        /// </summary>
        /// <param name="instanceAccess"></param>
        /// <returns></returns>
        public static DocsPaVO.InstanceAccess.InstanceAccess UpdateInstanceAccess(DocsPaVO.InstanceAccess.InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.InstanceAccess.InstanceAccess instance = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                instance = instanceAccessDB.UpdateInstanceAccess(instanceAccess, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: UpdateInstanceAccess ", e);
            }
            return instance;
        }

        public static void UpdateIdProfileDownload(string idProfile, string idInstanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                instanceAccessDB.UpdateIdProfileDownload(idProfile, idInstanceAccess, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: UpdateIdProfileDownload ", e);
            }
        }

        public static string GetIdProfileDownload(string idInstanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string idProfile = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                idProfile = instanceAccessDB.GetIdProfileDownload(idInstanceAccess, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: GetIdProfileDownload ", e);
            }
            return idProfile;
        }

        /// <summary>
        /// Aggiorna il tipo di richiesta per i documenti passati in input
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool UpdateInstanceAccessDocuments(List<InstanceAccessDocument> documents, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                result = instanceAccessDB.UpdateInstanceAccessDocuments(documents, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: UpdateInstanceAccess ", e);
            }
            return result;
        }

        public static DocsPaVO.InstanceAccess.InstanceAccess CreateDeclarationDocument(DocsPaVO.InstanceAccess.InstanceAccess instance, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            bool result = true;
            DocsPaVO.InstanceAccess.InstanceAccess newInstance = null;
            string reportDichiarazioneConformita = string.Empty;
            string detailsDocument = @"{\rtf1\ansi";
            string typeRequest = string.Empty;
            DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
            try
            {
                // Avvio del contesto transazionale
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    DocsPaVO.documento.SchedaDocumento newDoc = Documenti.DocManager.NewSchedaDocumento(infoUtente);
                    newDoc.oggetto = new DocsPaVO.documento.Oggetto() { descrizione = "Dichiarazione di conformita' per l'istanza " + instance.ID_INSTANCE_ACCESS };
                    newDoc.tipoProto = "G";
                    DocsPaVO.ProfilazioneDinamica.Templates template = GetTemplate(infoUtente);
                    if (template != null)
                    {
                        InstanceAccessDocument instanceDoc = (from i in instance.DOCUMENTS where i.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(template.DESCRIZIONE) select i).FirstOrDefault();
                        //Se esiste già un documento di conformità, lo rimuovo dall'istanza
                        if (instanceDoc != null)
                        {
                            RemoveInstanceAccessDocuments(new List<InstanceAccessDocument>() { instanceDoc }, infoUtente);
                            instance.DOCUMENTS.RemoveAll(d => d.ID_INSTANCE_ACCESS_DOCUMENT.Equals(instanceDoc.ID_INSTANCE_ACCESS_DOCUMENT));
                        }
                        newDoc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                        newDoc.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                        newDoc.tipologiaAtto.descrizione = template.DESCRIZIONE.ToString();
                        newDoc.template = template;

                        //Preparazione del modello RTF da allegare al documento di conformità che verrà creato
                        reportDichiarazioneConformita = ReportUtils.stringFile(template.PATH_MODELLO_1);
                        //reportDichiarazioneConformita = reportDichiarazioneConformita.Replace("XDESCRIZIONE", instance.DESCRIPTION);
                        reportDichiarazioneConformita = reportDichiarazioneConformita.Replace("XID_ISTANZA", instance.ID_INSTANCE_ACCESS);
                        reportDichiarazioneConformita = reportDichiarazioneConformita.Replace("XDATA_CREAZIONE", instance.CREATION_DATE.ToShortDateString());
                        /*
                        reportDichiarazioneConformita = instance.RICHIEDENTE != null ? reportDichiarazioneConformita.Replace("XRICHIEDENTE", instance.RICHIEDENTE.descrizione + " (" + instance.RICHIEDENTE.codiceRubrica + ") ")
                            : reportDichiarazioneConformita.Replace("XRICHIEDENTE", string.Empty);
                        reportDichiarazioneConformita = instance.REQUEST_DATE.Equals(DateTime.MinValue) ? reportDichiarazioneConformita.Replace("XDATA_RICHIESTA", string.Empty) : reportDichiarazioneConformita.Replace("XDATA_RICHIESTA", instance.REQUEST_DATE.ToShortDateString());
                        string protoRequest = string.Empty;

                        if (!string.IsNullOrEmpty(instance.ID_DOCUMENT_REQUEST))
                        {
                            InfoDocumento infoDoc = Documenti.DocManager.GetInfoDocumento(infoUtente, instance.ID_DOCUMENT_REQUEST, instance.ID_DOCUMENT_REQUEST);
                            if (infoDoc != null && !string.IsNullOrEmpty(infoDoc.segnatura))
                            {
                                protoRequest = infoDoc.segnatura + " " + infoDoc.oggetto;
                            }
                            else
                            {
                                protoRequest = infoDoc.idProfile + " " + infoDoc.oggetto;
                            }
                        }
                        reportDichiarazioneConformita = reportDichiarazioneConformita.Replace("XPROTOCOLLO_RICHIESTA", protoRequest);
                        reportDichiarazioneConformita = reportDichiarazioneConformita.Replace("XNOTE", instance.NOTE);
                         * */
                    }
                    newDoc = Documenti.DocSave.addDocGrigia(newDoc, infoUtente, ruolo);
                    if (newDoc != null)
                    {
                        instance.DOCUMENTS = (from d in instance.DOCUMENTS orderby d.TYPE_REQUEST ascending select d).ToList();
                        if (instance.DOCUMENTS != null)
                        {
                            foreach (InstanceAccessDocument doc in instance.DOCUMENTS)
                            {
                                SchedaDocumento schedaDoc = Documenti.DocManager.getDettaglioNoSecurity(infoUtente, doc.INFO_DOCUMENT.DOCNUMBER);
                                bool resultAtt = true;
                                if (doc.ENABLE)
                                {
                                    FileRequest fileReq = schedaDoc.documenti[0] as FileRequest;
                                    resultAtt = AddAttachment(newDoc.docNumber, doc.INFO_DOCUMENT.OBJECT, fileReq, infoUtente);
                                    detailsDocument += BuildDocumentRTF(doc);
                                }
                                if (resultAtt)
                                {
                                    if (schedaDoc.allegati != null && schedaDoc.allegati.Count > 0)
                                    {
                                        foreach (Allegato a in schedaDoc.allegati)
                                        {
                                            if ((from att in doc.ATTACHMENTS where att.ID_ATTACH.Equals(a.docNumber) && att.ENABLE select att).FirstOrDefault() != null)
                                            {
                                                detailsDocument += BuildAttachmentRTF(a, doc);
                                                if (!AddAttachment(newDoc.docNumber, schedaDoc.oggetto.descrizione, a, infoUtente))
                                                {
                                                    result = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    result = false;
                                }
                                if (!result)
                                {
                                    break;
                                }
                            }
                            if (result)
                            {
                                detailsDocument += @"}";
                                reportDichiarazioneConformita = reportDichiarazioneConformita.Replace("XDOCUMENTI", detailsDocument);
                                SchedaDocumento schedaDoc = Documenti.DocManager.getDettaglioNoSecurity(infoUtente, newDoc.docNumber);
                                fileDoc.content = ReportUtils.toByteArray(reportDichiarazioneConformita);
                                fileDoc.length = fileDoc.content.Length;
                                fileDoc.contentType = "application/rtf";
                                fileDoc.name = "DichiarazioneConformita.rtf";
                                FileRequest fileReq = schedaDoc.documenti[0] as FileRequest;
                                Documenti.FileManager.putFile(fileReq, fileDoc, infoUtente);
                                InstanceAccessDocument instanceDoc = new InstanceAccessDocument()
                                {
                                    ID_INSTANCE_ACCESS = instance.ID_INSTANCE_ACCESS,
                                    DOCNUMBER = newDoc.docNumber,
                                    ENABLE = true,
                                    INFO_DOCUMENT = new InfoDocument() { DOCNUMBER = newDoc.docNumber },
                                    ATTACHMENTS = BuildInstanceAccessAttachments(schedaDoc.allegati)
                                };
                                result = InstanceAccessManager.InsertInstanceAccessDocuments(new List<InstanceAccessDocument>() { instanceDoc }, infoUtente);
                                if (result)
                                {
                                    newInstance = InstanceAccessManager.GetInstanceAccessById(instance.ID_INSTANCE_ACCESS, infoUtente);
                                    if (newInstance == null)
                                        result = false;
                                }
                            }
                            if (result)
                            {
                                transactionContext.Complete();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: CreateDeclarationDocument ", e);
                return null;
            }
            return newInstance;
        }

        private static string BuildDocumentRTF(InstanceAccessDocument doc)
        {
            string result = string.Empty;
            result += @"\line\tab\b Tipo richiesta: \b0 " + doc.TYPE_REQUEST;
            if (string.IsNullOrEmpty(doc.INFO_DOCUMENT.ID_DOCUMENTO_PRINCIPALE))
            {
                result += @"\line\tab\b Doc: \b0 " + (string.IsNullOrEmpty(doc.INFO_DOCUMENT.NUMBER_PROTO) ? doc.INFO_DOCUMENT.DOCNUMBER : doc.INFO_DOCUMENT.NUMBER_PROTO);
                result += @"\line\tab\b Data protocollo/data creazione grigio: \b0 " + doc.INFO_DOCUMENT.DATE_CREATION.ToShortDateString();
                result += @"\line\tab\b Tipologia documento: \b0 " + doc.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO + @" - \b Numero repertorio: \b0 " + doc.INFO_DOCUMENT.COUNTER_REPERTORY;
                result += @"\line\tab\b Oggetto: \b0 " + doc.INFO_DOCUMENT.OBJECT;
                result += @"\line\tab\b Hash: \b0\fs20  " + doc.INFO_DOCUMENT.HASH + @"\fs24";
                result += @"\line\tab\b Classificazione: \b0 " + (doc.INFO_PROJECT != null ? doc.INFO_PROJECT.CODE_CLASSIFICATION : string.Empty);
                result += @"\line\tab\b Codice fascicolo: \b0 " + (doc.INFO_PROJECT != null ? doc.INFO_PROJECT.CODE_PROJECT : string.Empty);
                result += @"\line\tab\b Descrizione fascicolo: \b0 " + (doc.INFO_PROJECT != null ? doc.INFO_PROJECT.DESCRIPTION_PROJECT : string.Empty);
            }
            else
            {
                string num_proto = string.Empty;
                string dataProto = string.Empty;
                string dataCreazione = string.Empty;
                Dictionary<string, string> dati = BusinessLogic.Documenti.DocManager.GetInfoDocument(doc.INFO_DOCUMENT.ID_DOCUMENTO_PRINCIPALE);
                if (dati != null && dati.Count > 0)
                {
                    if (dati.ContainsKey("num_proto"))
                    {
                        num_proto = dati["num_proto"];
                    }
                    if (dati.ContainsKey("data_creazione"))
                    {
                        dataCreazione = dati["data_creazione"];
                    }
                    if (dati.ContainsKey("data_protocollazione"))
                    {
                        dataProto = dati["data_protocollazione"];
                    }
                }
                result += @"\line\tab\b Doc: \b0 " + (string.IsNullOrEmpty(num_proto) ? doc.INFO_DOCUMENT.ID_DOCUMENTO_PRINCIPALE : num_proto);
                result += @"\line\tab\b Data protocollo/data creazione grigio: \b0 " + (string.IsNullOrEmpty(num_proto) ? dataCreazione : dataProto);
                result += @"\line\b\tab\ Descrizione: \b0  " + doc.INFO_DOCUMENT.OBJECT;
                result += @"\line\b\tab\ Hash: \b0\fs20 " + doc.INFO_DOCUMENT.HASH + @"\fs24";

            }
            result = @"\i" + result + @"\i0\line";
            return result;
        }

        private static string BuildAttachmentRTF(Allegato all, InstanceAccessDocument doc)
        {
            string result = string.Empty;
            string tab = string.Empty;
            DocsPaDB.Query_DocsPAWS.Documenti document = new DocsPaDB.Query_DocsPAWS.Documenti();
            string hash = string.Empty;
            document.GetImpronta(out hash, all.versionId, all.docNumber);
            tab = doc.ENABLE ? @"\tab\tab" : @"\tab";
            if (!doc.ENABLE)
            {
                result += @"\line\b" + tab + @" Tipo richiesta: \b0 " + doc.TYPE_REQUEST;

                string num_proto = string.Empty;
                string dataProto = string.Empty;
                string dataCreazione = string.Empty;
                Dictionary<string, string> dati = BusinessLogic.Documenti.DocManager.GetInfoDocument(doc.INFO_DOCUMENT.DOCNUMBER);
                if (dati != null && dati.Count > 0)
                {
                    if (dati.ContainsKey("num_proto"))
                    {
                        num_proto = dati["num_proto"];
                    }
                    if (dati.ContainsKey("data_creazione"))
                    {
                        dataCreazione = dati["data_creazione"];
                    }
                    if (dati.ContainsKey("data_protocollazione"))
                    {
                        dataProto = dati["data_protocollazione"];
                    }
                }
                result += @"\line\b" + tab + @" Doc: \b0 " + (string.IsNullOrEmpty(num_proto) ? doc.INFO_DOCUMENT.DOCNUMBER : num_proto);
                result += @"\line\b" + tab + @" Data protocollo/data creazione grigio: \b0 " + (string.IsNullOrEmpty(num_proto) ? dataCreazione : dataProto);
                result += @"\line\b" + tab + @" Classificazione: \b0 " + (doc.INFO_PROJECT != null ? doc.INFO_PROJECT.CODE_CLASSIFICATION : string.Empty);
                result += @"\line\b" + tab + @" Codice fascicolo: \b0 " + (doc.INFO_PROJECT != null ? doc.INFO_PROJECT.CODE_PROJECT : string.Empty);
                result += @"\line\b" + tab + @" Descrizione fascicolo: \b0 " + (doc.INFO_PROJECT != null ? doc.INFO_PROJECT.DESCRIPTION_PROJECT : string.Empty);
            }
            result += @"\line\b" + tab + @" Descrizione: \b0  " + all.descrizione;
            result += @"\line\b " + tab + @" Hash: \b0\fs20 " + hash + @"\fs24";

            result = @"\i" + result + @"\i0\line";
            return result;
        }

        public static DocsPaVO.documento.SchedaDocumento ForwardsInstanceAccess(DocsPaVO.InstanceAccess.InstanceAccess instance, out int totalFileSizeInstance, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.documento.SchedaDocumento documento = null;
            int fileSize;
            totalFileSizeInstance = 0;
            try
            {
                DocsPaVO.ProfilazioneDinamica.Templates template = GetTemplate(infoUtente);
                           
                // Creazione scheda documento finale (con utilizzo del repository context, indipendentemente dal fatto che sia disabilito o meno)
                documento = Documenti.DocManager.NewSchedaDocumento(infoUtente, true);
                documento.tipoProto = "P";
                documento.predisponiProtocollazione = true;
                documento.protocollo = new DocsPaVO.documento.ProtocolloUscita();
                documento.protocollo.daProtocollare = "1";

                DocsPaVO.utente.Corrispondente corr = ruolo.uo;
                ((DocsPaVO.documento.ProtocolloUscita)documento.protocollo).mittente = corr;

                Documenti.SessionRepositoryFileManager repositoryFileManager = Documenti.SessionRepositoryFileManager.GetFileManager(documento.repositoryContext);
                documento.oggetto.descrizione = instance.DESCRIPTION;

                if (instance.DOCUMENTS != null)
                {
                    foreach (InstanceAccessDocument doc in instance.DOCUMENTS)
                    {
                        SchedaDocumento schedaDoc = Documenti.DocManager.getDettaglioNoSecurity(infoUtente, doc.INFO_DOCUMENT.DOCNUMBER);
                        if (doc.ENABLE)
                        {
                            DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)schedaDoc.documenti[0];
                            Int32.TryParse(versioneCorrente.fileSize, out fileSize);
                            // Impostazione del documento da inoltrare come allegato al documento finale
                            DocsPaVO.documento.Allegato allegatoPrincipale = new DocsPaVO.documento.Allegato
                            {
                                descrizione = "Inoltro documento principale " + schedaDoc.oggetto.descrizione,
                                fileName = versioneCorrente.fileName,
                                fileSize = versioneCorrente.fileSize,
                                path = versioneCorrente.path,
                                repositoryContext = versioneCorrente.repositoryContext,
                                subVersion = versioneCorrente.subVersion,
                                version = "1",
                                versionId = versioneCorrente.versionId,
                                versionLabel = DocsPaDB.Query_DocsPAWS.Documenti.FormatCodiceAllegato(documento.allegati.Count + 1),
                                numeroPagine = 0,
                                dataInserimento = string.Empty
                            };

                            if (fileSize > 0 && !string.IsNullOrEmpty(versioneCorrente.fileName))
                            {
                                totalFileSizeInstance += Convert.ToInt32(versioneCorrente.fileSize);
                                if(((double)totalFileSizeInstance/1048576) > 20)
                                {
                                    return null;
                                }
                                // Reperimento del file associato alla versione corrente
                                DocsPaVO.documento.FileDocumento file = Documenti.FileManager.getFileFirmato(versioneCorrente, infoUtente, false);

                                // Copia del file nel repository temporaneo
                                repositoryFileManager.SetFile(allegatoPrincipale, file);
                            }

                            allegatoPrincipale.repositoryContext = documento.repositoryContext;
                            documento.allegati.Add(allegatoPrincipale);

                        }
                        if ((schedaDoc.allegati != null && schedaDoc.allegati.Count > 0) &&
                            (template == null || !doc.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(template.DESCRIZIONE)))
                        {
                            foreach (Allegato allegato in schedaDoc.allegati)
                            {
                                if ((from att in doc.ATTACHMENTS where att.ID_ATTACH.Equals(allegato.docNumber) && att.ENABLE select att).FirstOrDefault() != null)
                                {
                                    allegato.descrizione = string.Format("Inoltro {0}", allegato.descrizione);
                                    allegato.dataInserimento = string.Empty;

                                    if (allegato.descrizione.Length > 2000)
                                        allegato.descrizione = string.Format("{0}...", allegato.descrizione.Substring(0, 1900));

                                    // Reperimento versionlabel per l'allegato
                                    allegato.versionLabel = DocsPaDB.Query_DocsPAWS.Documenti.FormatCodiceAllegato(documento.allegati.Count + 1);
                                    Int32.TryParse(allegato.fileSize, out fileSize);

                                    if (fileSize > 0 && !string.IsNullOrEmpty(allegato.fileName))
                                    {
                                        totalFileSizeInstance += Convert.ToInt32(allegato.fileSize);
                                        if (((double)totalFileSizeInstance / 1048576) > 20)
                                        {
                                            return null;
                                        }
                                        // Reperimento del file associato alla versione corrente
                                        DocsPaVO.documento.FileDocumento file = Documenti.FileManager.getFileFirmato(allegato, infoUtente, false);

                                        // Copia del file nel repository temporaneo
                                        repositoryFileManager.SetFile(allegato, file);
                                    }

                                    allegato.repositoryContext = documento.repositoryContext;
                                    documento.allegati.Add(allegato);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: ForwardsInstanceAccess ", e);
                return null;
            }
            return documento;
        }

        private static bool AddAttachment(string docnumber, string descrizione, FileRequest fileReq, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            try
            {
                Allegato all = new Allegato();
                all.docNumber = docnumber;
                all.TypeAttachment = 1;
                all.descrizione = descrizione.Replace("/", "//").Replace("'","''");
                all.dataInserimento = DateTime.Today.ToString();
                all.version = "1";
                all = Documenti.AllegatiManager.aggiungiAllegato(infoUtente, all);
                if (all != null)
                {
                    if ((!string.IsNullOrEmpty(fileReq.fileSize)) && Convert.ToInt32(fileReq.fileSize) > 0)
                    {
                        DocsPaVO.documento.FileDocumento fileDoc = Documenti.FileManager.getFileFirmato(fileReq, infoUtente, false);
                        if (fileDoc != null)
                        {
                            Documenti.FileManager.putFile(all, fileDoc, infoUtente);
                        }
                    }
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: AddAttachment ", e);
                return false;
            }
            return result;
        }

        /// <summary>
        /// Restituisce lo stato del download dell'istanza
        /// </summary>
        /// <param name="idInstanceAcces"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static string GetStateDownloadInstanceAccess(string idInstanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string stateDownload = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                stateDownload = instanceAccessDB.GetStateDownloadInstanceAccess(idInstanceAccess, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: GetStateDownloadInstanceAccess ", e);
                return stateDownload;
            }
            return stateDownload;
        }

        public static DocsPaVO.ProfilazioneDinamica.Templates GetTemplate(DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.ProfilazioneDinamica.Templates template = null;
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                template = instanceAccessDB.GetTemplate(infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: GetTemplate ", e);
                return null;
            }
            return template;
        }

        private static List<InstanceAccessAttachments> BuildInstanceAccessAttachments(ArrayList attachments)
        {
            List<InstanceAccessAttachments> instanceAttachments = new List<InstanceAccessAttachments>();
            if (attachments != null && attachments.Count > 0)
            {
                foreach (Allegato a in attachments)
                {
                    InstanceAccessAttachments att = new InstanceAccessAttachments()
                    {
                        ID_ATTACH = a.docNumber,
                        ENABLE = true
                    };
                    instanceAttachments.Add(att);
                }
            }
            return instanceAttachments;
        }

        /// <summary>
        /// Aggiorna il tipo di richiesta per i documenti passati in input
        /// </summary>
        /// <param name="documents"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool CreateDownloadInstanceAccess(DocsPaVO.InstanceAccess.InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
            try
            {              
                instanceAccessDB.UpdateInstanceStartDownload(instanceAccess, infoUtente, "1");
                result = PrepareInstanceZip(instanceAccess, infoUtente);
                if (result)
                {
                    instanceAccessDB.UpdateInstanceStartDownload(instanceAccess, infoUtente, "2");
                }
                else
                {
                    instanceAccessDB.UpdateInstanceStartDownload(instanceAccess, infoUtente, "0");
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: CreateDownloadInstanceAccess ", e);
                instanceAccessDB.UpdateInstanceStartDownload(instanceAccess, infoUtente, "0");
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Rimozione del file Zip contenente il download dell'istanza
        /// </summary>
        /// <param name="instanceAccess"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static bool RemoveInstanceZip(DocsPaVO.InstanceAccess.InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                instanceAccessDB.UpdateInstanceStartDownload(instanceAccess, infoUtente, "0");

                string rootPath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_INSTANCE_ACCESS_PATH");
                DocsPaVO.amministrazione.InfoAmministrazione infoAmm = null;
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                infoAmm = amm.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);
                string istancePath = Path.Combine(rootPath, infoAmm.Codice);
                istancePath = Path.Combine(istancePath, instanceAccess.ID_INSTANCE_ACCESS + ".ZIP");
                istancePath = replaceInvalidChar(istancePath);
                Directory.Delete(istancePath, true);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: RemoveInstanceZip ", e);
            }
            return result;
        }

        public static bool UpdateInstanceAccessDocumentEnable(List<InstanceAccessDocument> listInstanceAccessDocument, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                instanceAccessDB.UpdateInstanceAccessDocumentEnable(listInstanceAccessDocument, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: UpdateInstanceAccessDocumentEnable ", e);
            }
            return result;
        }

        public static bool UpdateInstanceAccessAttachmentsEnable(List<InstanceAccessAttachments> listInstanceAccessAttachments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.InstanceAccessDB instanceAccessDB = new DocsPaDB.Query_DocsPAWS.InstanceAccessDB();
                instanceAccessDB.UpdateInstanceAccessAttachmentsEnable(listInstanceAccessAttachments, infoUtente);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: UpdateInstanceAccessDocumentEnable ", e);
            }
            return result;
        }

        #region DOWNLOAD INSTANCE

        private static bool PrepareInstanceZip(DocsPaVO.InstanceAccess.InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            string rootPath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_INSTANCE_ACCESS_PATH");
            infoDoc.Clear();
            string istancePath = string.Empty;
            try
            {
                DocsPaVO.ProfilazioneDinamica.Templates template = GetTemplate(infoUtente);
                if (!string.IsNullOrEmpty(rootPath))
                {
                    DocsPaVO.amministrazione.InfoAmministrazione infoAmm = null;
                    // Inizio a creare l'istanza per il download
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    infoAmm = amm.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);
                    istancePath = Path.Combine(rootPath, infoAmm.Codice);
                    istancePath = Path.Combine(istancePath, instanceAccess.ID_INSTANCE_ACCESS);
                    istancePath = replaceInvalidChar(istancePath);

                    if (Directory.Exists(istancePath))
                    {
                        Directory.Delete(istancePath, true);
                    }
                    new staticfiles.WriteStaticFiles(Path.Combine(istancePath, "static"));

                    //Per ciascuna elemento tutti i documenti con i relativi allegati
                    if (instanceAccess != null && instanceAccess.DOCUMENTS != null && instanceAccess.DOCUMENTS.Count > 0)
                    {
                        string currPrj = string.Empty;
                        //cicla per tutti gli elementi delll'istanza 
                        for (int j = 0; j < instanceAccess.DOCUMENTS.Count; j++)
                        {
                            InstanceAccessDocument item = instanceAccess.DOCUMENTS[j];
                            //Prima di scrivere il documento su files system uso l'oggetto SaveFolder per estendere
                            //il root path con il percorso del fascicolo
                            if (item.INFO_PROJECT != null && item.INFO_PROJECT.ID_PROJECT != currPrj)
                            {
                                currPrj = item.INFO_PROJECT.ID_PROJECT;
                                {
                                    string pf = Path.Combine(istancePath, Path.Combine("Fascicoli", replaceInvalidChar(item.INFO_PROJECT.CODE_PROJECT)));
                                    pf = replaceInvalidChar(pf);
                                    if (!Directory.Exists(pf))
                                        Directory.CreateDirectory(pf);
                                    //DocsPaConservazione.Metadata.XmlFascicolo xfa = new Metadata.XmlFascicolo(infoCons, itemsCons.ID_Project, sf.folderTree);
                                    //if (!dryRun)
                                    //    new InfoDocXml().saveMetadatiString(Path.Combine(pf, itemsCons.ID_Project), xfa.XmlFile);

                                }

                            }
                            //Mi creo il file XML dell'istanza
                            BusinessLogic.InstanceAccess.XmlInstance istanza = new InstanceAccess.XmlInstance(instanceAccess, infoUtente);
                            //Ci salva dentro il file XML dell'istanza
                            saveMetadatiString(Path.Combine(istancePath, "dati_istanza"), istanza.XmlFile);
                            bool isDichiarazioneConformita = template != null && (item.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(template.DESCRIZIONE));
                            if (putDocumenti(istancePath, item, isDichiarazioneConformita, infoUtente) != 1)
                            {
                                result = false;   
                                break;
                            }
                        }
                    }
                    if (result)
                    {
                        IndexFolderHtml(instanceAccess, infoUtente, istancePath, template);
                        if (!ZipFolder(instanceAccess.ID_INSTANCE_ACCESS, istancePath))
                        {
                            result = false;
                        }
                    }
                    if (Directory.Exists(istancePath))
                    {
                        Directory.Delete(istancePath, true);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in BusinessLogic.InstanceAccess.InstanceAccessManager  - metodo: PrepareInstanceZip ", e);
                if (Directory.Exists(istancePath))
                {
                    Directory.Delete(istancePath, true);
                }
                if (File.Exists(istancePath + ".ZIP"))
                {
                    File.Delete(istancePath + ".ZIP");
                }
                result = false;
            }
            return result;
        }

        private static void IndexFolderHtml(DocsPaVO.InstanceAccess.InstanceAccess instanceAccess, DocsPaVO.utente.InfoUtente infoUtente, string htmlPath, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            bool res = false;
            string[] htmlPathArray = new string[7];
            string[] titolo = new string[7];
            TextWriter twriter = null;
            titolo[0] = string.Empty;

            htmlPathArray[0] = Path.Combine(htmlPath, "index.html");

            //nuova struttura directory!!!!!!!!!!!
            htmlPath = Path.Combine(htmlPath, "html");
            if (!Directory.Exists(htmlPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(htmlPath);
            }

            titolo[1] = "Ricerca per numero documento";
            htmlPathArray[1] = Path.Combine(htmlPath, "RicDocNumber.html");
            res = createHTML(instanceAccess, htmlPathArray[1], "docNumber", titolo[1], template);
            titolo[2] = "Ricerca per segnatura o numero di documento";
            htmlPathArray[2] = Path.Combine(htmlPath, "RicSegnatura.html");
            res = createHTML(instanceAccess, htmlPathArray[2], "segnatura", titolo[2], template);
            titolo[3] = "Ricerca per descrizione oggetto";
            htmlPathArray[3] = Path.Combine(htmlPath, "RicOggetto.html");
            res = createHTML(instanceAccess, htmlPathArray[3], "oggetto", titolo[3], template);
            titolo[4] = "Ricerca per codice fascicolo";
            htmlPathArray[4] = Path.Combine(htmlPath, "RicFascicolo.html");
            res = createHTML(instanceAccess, htmlPathArray[4], "fascicolo", titolo[4], template);
            titolo[5] = "Ricerca per data di creazione o protocollazione";
            htmlPathArray[5] = Path.Combine(htmlPath, "RicData.html");
            res = createHTML(instanceAccess, htmlPathArray[5], "data", titolo[5], template);
            titolo[6] = "Ricerca per nome file";
            htmlPathArray[6] = Path.Combine(htmlPath, "RicFileName.html");
            res = createHTML(instanceAccess, htmlPathArray[6], "fileName", titolo[6], template);
            try
            {
                if (!File.Exists(htmlPathArray[0]))
                {
                    twriter = new StreamWriter(htmlPathArray[0], false, Encoding.UTF8);
                    twriter.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
                    twriter.WriteLine("<HTML>");
                    twriter.WriteLine("<HEAD>");
                    twriter.WriteLine("<TITLE> INDICE RICERCHE </TITLE>");
                    twriter.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"static/main.css\" />");
                    twriter.WriteLine("</HEAD>");
                    twriter.WriteLine("<BODY>");
                    twriter.WriteLine("<div class=\"site-container\">");
                    twriter.WriteLine("<div class=\"header\">");
                    twriter.WriteLine("<img src=\"static/logo.jpg\" />");
                    twriter.WriteLine("</div>");
                    twriter.WriteLine("<div class=\"site-title\">");
                    twriter.WriteLine("<h3>Tutti i file contenuti all’interno di questo supporto sono UFFICIALI</h3>");
                    twriter.WriteLine("</div>");
                    twriter.WriteLine("<div class=\"body-content\">");
                    twriter.WriteLine("<h4 class=\"title-indicizza\">Indicizza per: </h4>");
                    twriter.WriteLine("<div class=\"col-sx\">");
                    twriter.WriteLine("<ul class=\"index\">");

                    for (int i = 1; i < htmlPathArray.Length; i++)
                    {
                        string path_relativo = "." + '\u002F'.ToString() + "html" + '\u002F'.ToString() + Path.GetFileName(htmlPathArray[i]);
                        twriter.WriteLine("<li><a target=\"content\" href='" + path_relativo + "'>" + titolo[i] + "</a></li>");
                    }

                    twriter.WriteLine("</ul>");
                    twriter.WriteLine("</div>");
                    twriter.WriteLine("<div class=\"col-dx\">");
                    twriter.WriteLine("<iframe name=\"content\" class=\"content-frame\"></iframe>");
                    twriter.WriteLine("</div>");
                    twriter.WriteLine("<div class=\"cl\"></div>");
                    twriter.WriteLine("<div class=\"sep\"></div>");
                    twriter.WriteLine("<table class=\"footer-table\">");
                    twriter.WriteLine(String.Format("<tr><th>Ente:</th><td>{0}</td></tr>", BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione).Descrizione + " - " + BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione).Codice));
                    twriter.WriteLine(String.Format("<tr><th>Istanza numero:</th><td>{0}</td></tr>", instanceAccess.ID_INSTANCE_ACCESS));
                    twriter.WriteLine(String.Format("<tr><th>Descrizione:</th><td>{0}</td></tr>", instanceAccess.DESCRIPTION));
                    twriter.WriteLine(String.Format("<tr><th>Richiedente:</th><td>{0}</td></tr>", instanceAccess.RICHIEDENTE.descrizione));
                    twriter.WriteLine(String.Format("<tr><th>Data richiesta:</th><td>{0}</td></tr>", instanceAccess.CREATION_DATE));
                    twriter.WriteLine(String.Format("<tr><th>Id documento  richiesta:</th><td>{0}</td></tr>", instanceAccess.ID_DOCUMENT_REQUEST));
                    twriter.WriteLine(String.Format("<tr><th>Note istanza:</th><td>{0}</td></tr>", instanceAccess.NOTE));
                    twriter.WriteLine("</table>");
                    twriter.WriteLine("</div>");
                    twriter.WriteLine("</div>");
                    twriter.WriteLine("<script>");
                    twriter.WriteLine("var elems = document.getElementsByTagName(\"a\");");
                    twriter.WriteLine("for ( var i=0; i<elems.length; i++) {");
                    twriter.WriteLine("elems[i].onclick = function () {");
                    twriter.WriteLine("setActive(this);");
                    twriter.WriteLine("};");
                    twriter.WriteLine("}");
                    twriter.WriteLine("function setActive(elem){");
                    twriter.WriteLine("for ( var i=0; i<elems.length; i++) {");
                    twriter.WriteLine("elems[i].className = \"\";");
                    twriter.WriteLine("}");
                    twriter.WriteLine("elem.className = \"active\";");
                    twriter.WriteLine("}");
                    twriter.WriteLine("</script>");
                    twriter.WriteLine("</BODY>");
                    twriter.WriteLine("</HTML>");

                }
            }
            catch (Exception exHtml)
            {
                string err = "Errore nella creazione del main index Html : " + exHtml.Message;
                logger.Debug(err);
            }
            finally
            {
                if (twriter != null)
                {
                    twriter.Flush();
                    twriter.Close();
                }
            }
        }



        private static bool createHTML(DocsPaVO.InstanceAccess.InstanceAccess instanceAccess, string HtmlPath, string tipoRic, string title, DocsPaVO.ProfilazioneDinamica.Templates template)
        {
            List<InstanceAccessDocument> listDocuments = instanceAccess.DOCUMENTS;
            bool result = true;
            TextWriter twriter = null;
            string paragrafo = string.Empty;

            if (listDocuments != null && listDocuments.Count() > 0)
            {
                try
                {
                    //Ordino la lista in base al tipo di ricerca
                    switch (tipoRic)
                    {
                        case "docNumber":
                            listDocuments = (from d in listDocuments orderby d.INFO_DOCUMENT.DOCNUMBER descending select d).ToList();
                            break;
                        case "segnatura":
                            listDocuments = (from d in listDocuments orderby d.INFO_DOCUMENT.NUMBER_PROTO descending, d.INFO_DOCUMENT.DOCNUMBER descending select d).ToList();
                            break;
                        case "oggetto":
                            listDocuments = (from d in listDocuments orderby d.INFO_DOCUMENT.OBJECT ascending select d).ToList();
                            break;
                        case "fascicolo":
                            listDocuments.OrderBy(x => x.INFO_PROJECT != null).ThenByDescending(x => x.INFO_PROJECT.CODE_PROJECT);
                            break;
                        case "data":
                            var appoggio = (from d in listDocuments
                                            select new
                                            {
                                                doc = d,
                                                data = infoDoc[d.ID_INSTANCE_ACCESS_DOCUMENT]
                                            });
                            appoggio.OrderByDescending(x => x.data).Select(x => x.doc);
                            listDocuments = (from a in appoggio select a.doc).ToList();
                            break;
                        case "fileName":
                            listDocuments = (from d in listDocuments orderby d.INFO_DOCUMENT.FILE_NAME ascending select d).ToList();
                            break;
                    }

                    if (!File.Exists(HtmlPath))
                    {
                        twriter = new StreamWriter(HtmlPath, false, Encoding.UTF8);
                        twriter.WriteLine("<HTML>");
                        twriter.WriteLine("<HEAD>");
                        twriter.WriteLine("<TITLE> Ricerca per " + title + "  </TITLE>");
                        twriter.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" media=\"all\" href=\"../static/main.css\" />");
                        twriter.WriteLine("</HEAD>");
                        twriter.WriteLine("<BODY class=\"content\">");
                        twriter.WriteLine("<h3 align=center> Ricerca per " + title + "  </h3>");
                        twriter.WriteLine("<br><br>");
                        twriter.WriteLine("<ol type='1'>");

                        ArrayList arrayTipologia = new ArrayList();

                        for (int i = 0; i < listDocuments.Count; i++)
                        {
                            bool isDichiarazioneConformita = template != null && (listDocuments[i].INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(template.DESCRIZIONE));
                            //i tag <ul> e <li> lo chiudo e lo riapro in questo punto dopo aver controllato se il paragrafo
                            //è diverso da quello appena scritto!!!
                            //SE è IL PRIMO ELEMENTO NON DEVO METTERE I TAG DI CHIUSURA ALL'INIZIO!!!!!
                            string valoreRicerca = ValoreRicerca(listDocuments[i], tipoRic);
                            if (i > 0 && paragrafo != valoreRicerca)
                            {
                                twriter.WriteLine("</ul>");
                                twriter.WriteLine("</li>");
                                twriter.WriteLine("<li><b>" + valoreRicerca + "</b>");
                                twriter.WriteLine("<ul type='disc'>");
                                paragrafo = valoreRicerca;
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    twriter.WriteLine("<li><b>" + valoreRicerca + "</b>");
                                    twriter.WriteLine("<ul type='disc'>");
                                    paragrafo = valoreRicerca;
                                }
                            }
                            string pathFasc = string.Empty;
                            if (listDocuments[i].INFO_PROJECT != null && !string.IsNullOrEmpty(listDocuments[i].INFO_PROJECT.CODE_PROJECT))
                            {
                                pathFasc = @"/Fascicoli/" + listDocuments[i].INFO_PROJECT.CODE_PROJECT;
                            }
                            string fileNameDocument = listDocuments[i].INFO_DOCUMENT.FILE_NAME.Replace('\u005C', '\u002F');
                            if (string.IsNullOrEmpty(fileNameDocument) || !listDocuments[i].ENABLE)
                            {
                                twriter.WriteLine("<li>" + pathFasc + "/Documenti/" + listDocuments[i].INFO_DOCUMENT.DOCNUMBER + "</a></li>");
                            }
                            else
                            {
                                twriter.WriteLine("<li><a target=\"_blank\" href='.." + pathFasc + "/Documenti/" + listDocuments[i].INFO_DOCUMENT.DOCNUMBER + "/" + fileNameDocument + "'>"
                                    + pathFasc + "/Documenti/" + listDocuments[i].INFO_DOCUMENT.DOCNUMBER + "/" + fileNameDocument + "</a></li>");
                            }
                            twriter.WriteLine("<li type='circle'><b>File metadati: </b><a target=\"_blank\" href='.." + pathFasc + "/Documenti/" + listDocuments[i].INFO_DOCUMENT.DOCNUMBER + "/" + (string.IsNullOrEmpty(fileNameDocument) ? "fileMetadati" : fileNameDocument) + ".xml'>" +
                                pathFasc + "/Documenti/" + listDocuments[i].INFO_DOCUMENT.DOCNUMBER + "/" + (string.IsNullOrEmpty(fileNameDocument) ? "fileMetadati" : fileNameDocument) + ".xml</a></li>");
                            if (listDocuments[i].ATTACHMENTS != null && listDocuments[i].ATTACHMENTS.Count > 0 && !isDichiarazioneConformita)
                            {
                                for (int j = 0; j < listDocuments[i].ATTACHMENTS.Count; j++)
                                {
                                    if (listDocuments[i].ATTACHMENTS[j].ENABLE)
                                    {
                                        twriter.WriteLine("<ul type='square'>");
                                        if (!string.IsNullOrEmpty(listDocuments[i].ATTACHMENTS[j].FILE_NAME))
                                        {
                                            twriter.WriteLine("<li><b>File allegato: </b><a target=\"_blank\" href='.." + pathFasc + "/Documenti/" + listDocuments[i].INFO_DOCUMENT.DOCNUMBER + "/Allegati/" + listDocuments[i].ATTACHMENTS[j].FILE_NAME.Replace('\u005C', '\u002F') + "'>"
                                              + pathFasc + "/Documenti/" + listDocuments[i].INFO_DOCUMENT.DOCNUMBER + "/Allegati/" + listDocuments[i].ATTACHMENTS[j].FILE_NAME + "</a></li>");
                                        
                                        }
                                        twriter.WriteLine("</ul>");
                                    }
                                }
                            }

                        }
                        twriter.WriteLine("</ol>");
                        twriter.WriteLine("</BODY>");
                        twriter.WriteLine("</HTML>");
                    }
                }
                catch (Exception exHtml)
                {
                    string err = "Errore nella creazione della pagina " + HtmlPath + " : " + exHtml.Message;
                    logger.Debug(err);
                    result = false;
                }
                finally
                {
                    if (twriter != null)
                    {
                        twriter.Flush();
                        twriter.Close();
                    }
                }
            }
            return result;
        }


        private static int putDocumenti(string instancePath, DocsPaVO.InstanceAccess.InstanceAccessDocument doc, bool isDichiarazioneConformita, DocsPaVO.utente.InfoUtente infoUtente)
        {
            int result = 1;
            string err = string.Empty;
            DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();

            DocsPaVO.documento.SchedaDocumento sch = new DocsPaVO.documento.SchedaDocumento();
            sch = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, doc.INFO_DOCUMENT.DOCNUMBER);
            DocsPaVO.documento.FileDocumento fd = null;
            string dataCreazione_or_proto = string.Empty;
            if (sch.protocollo != null && !string.IsNullOrEmpty(sch.protocollo.dataProtocollazione))
            {
                dataCreazione_or_proto = sch.protocollo.dataProtocollazione;
            }
            else
            {
                CultureInfo culture = new CultureInfo("it-IT");
                string[] formati = { "dd/MM/yyyy HH.mm.ss", "dd/MM/yyyy H.mm.ss", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy H:mm:ss", "dd/MM/yyyy" };
                dataCreazione_or_proto = DateTime.ParseExact(sch.dataCreazione, formati, culture.DateTimeFormat, System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToShortDateString();
            }
            infoDoc.Add(doc.ID_INSTANCE_ACCESS_DOCUMENT, dataCreazione_or_proto);
            try
            {
                //In questo modo recupero, se esiste, il file fisico associato all'ultima versione del documento
                if (sch.documenti != null && sch.documenti[0] != null)
                {
                    try
                    {
                        DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sch.documenti[0];
                        if (Int32.Parse(((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileSize) > 0)
                        {
                            fd = BusinessLogic.Documenti.FileManager.getFileFirmato(fr, infoUtente, true);
                            if (fd == null)
                                throw new Exception("Errore nel reperimento del file principale.");
                        }
                        //salvataggio nella relativa cartella di conservazione del file
                        if (!saveFile(fd, instancePath, doc, sch, fr, true, infoUtente))
                            throw new Exception("Errore nella scrittura del file principale.");
                        result = 1;
                    }
                    catch (Exception ex)
                    {
                        err = "Errore nel reperimento del file principale : " + ex.Message;
                        logger.Debug(err);
                        result = 0;
                    }
                }
                //Recupero tutti gli allegati associati al documento corrente
                if (!isDichiarazioneConformita)
                {
                    for (int i = 0; sch.allegati != null && i < sch.allegati.Count; i++)
                    {
                        DocsPaVO.documento.Allegato documentoAllegato = (DocsPaVO.documento.Allegato)sch.allegati[i];
                        if (doc.ATTACHMENTS.Find(a => a.ID_ATTACH.Equals(documentoAllegato.docNumber) && a.ENABLE) != null)
                        {
                            DocsPaVO.documento.FileDocumento fdAll = null;
                            if (Int32.Parse(documentoAllegato.fileSize) > 0)
                            {
                                try
                                {
                                    DocsPaVO.documento.FileRequest frAll = (DocsPaVO.documento.FileRequest)sch.allegati[i];
                                    //fdAll = BusinessLogic.Documenti.FileManager.getFile(frAll, infoUtente);
                                    fdAll = BusinessLogic.Documenti.FileManager.getFileFirmato(frAll, infoUtente, true);
                                    if (fdAll == null)
                                        throw new Exception("Errore nel reperimento dell'allegato numero" + i.ToString());

                                    //salvataggio nella relativa cartella di conservazione del file dell'allegato
                                    if (!saveFile(fdAll, instancePath, doc, sch, frAll, false, infoUtente))
                                        throw new Exception("Errore nella scrittura del file allegato.");
                                }
                                catch (Exception exc)
                                {
                                    err = "Errore nel reperimento dell'allegato numero " + i.ToString() + " : " + exc.Message;
                                    logger.Debug(err);
                                    result = 0;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exGen)
            {
                err = "Errore nel reperimento del file principale : " + exGen.Message;
                logger.Debug(err);
                result = 0;
            }
            return result;
        }


        private static bool saveFile(DocsPaVO.documento.FileDocumento fileDoc, string instancePath, DocsPaVO.InstanceAccess.InstanceAccessDocument doc, DocsPaVO.documento.SchedaDocumento schDoc, DocsPaVO.documento.FileRequest objFileRequest, bool isDoc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            string err = string.Empty;
            string fullName = string.Empty;

            //nuova struttura directory!!!!!!!!!!!
            string rootXml = Path.Combine(instancePath, "Chiusura");
            //string rootXml = root_path;
            string pathDoc = schDoc.docNumber;
            if (doc.INFO_PROJECT != null)
            {
                instancePath = Path.Combine(instancePath, Path.Combine("Fascicoli", replaceInvalidChar(doc.INFO_PROJECT.CODE_PROJECT)));
                pathDoc = Path.Combine(instancePath, Path.Combine("Documenti", schDoc.docNumber));
            }
            else
            {
                pathDoc = Path.Combine("Documenti", schDoc.docNumber);
            }

            //Creo una sottocartella (nominata con il DocNumber) per contenere anche gli allegati ed i metadati
            string pathContainsAttach = Path.Combine(instancePath, Path.Combine("Documenti", schDoc.docNumber));

            //Inizializzo il path relativo del supporto destinato alla Conservazione ed elimino i caratteri speciali
            pathDoc = replaceInvalidChar(pathDoc);
            string path_supporto = '\u005C'.ToString() + pathDoc;

            //se è un allegato devo creare la sottocartella per gli allegati
            bool isAll = !isDoc;
            if (isAll)
            {
                pathContainsAttach = Path.Combine(pathContainsAttach, "Allegati");
                path_supporto = Path.Combine(path_supporto, "Allegati");
            }

            //normalizzo il percorso eliminando i caratteri speciali
            pathContainsAttach = replaceInvalidChar(pathContainsAttach);

            if (!Directory.Exists(pathContainsAttach))
            {
                Directory.CreateDirectory(pathContainsAttach);
            }

            //nuova struttura directory!!!!!!!!!!!
            //normalizzo il percorso eliminando i caratteri speciali
            rootXml = replaceInvalidChar(rootXml);

            if (!Directory.Exists(rootXml))
            {
                Directory.CreateDirectory(rootXml);
            }
            string fileName = string.Empty;
            if (fileDoc != null)
            {
                fileName = replaceInvalidCharFile(fileDoc.nomeOriginale);

                fullName = pathContainsAttach + '\u005C'.ToString() + fileName;
                path_supporto = Path.Combine(path_supporto, fileName);
                //Se è un allegato oppure è un documento abilitato all'interno dell'istanza, salvo il file
                if (!isDoc || doc.ENABLE)
                {
                    FileStream file = null;
                    try
                    {
                        file = File.Create(fullName);
                        file.Write(fileDoc.content, 0, fileDoc.length);
                    }
                    catch (Exception e)
                    {
                        err = "Errore nella gestione del salvataggio del File " + e.Message;
                        logger.Debug(err);
                        result = false;
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        if (file != null)
                        {
                            file.Flush();
                            file.Close();
                        }
                    }
                }
            }
            //Se è il documento principale salvo i metadati su file XML

            if (isDoc)
            {
                try
                {
                    string nameXml = string.IsNullOrEmpty(fileName) ? "fileMetadati" : fileName;
                    BusinessLogic.InstanceAccess.XmlDocument xdoc = new BusinessLogic.InstanceAccess.XmlDocument(fileDoc, schDoc, objFileRequest, doc, infoUtente);
                    saveMetadatiString(Path.Combine(pathContainsAttach, nameXml), xdoc.XmlFile);
                }
                catch (Exception eXml)
                {
                    err = "Errore nella scrittura del file XML dei metadati." + eXml.Message;
                    logger.Debug(err);
                    result = false;
                }
            }
            return result;
        }

        private static string ValoreRicerca(InstanceAccessDocument doc, string tipoRic)
        {
            string paragrafo = string.Empty;

            switch (tipoRic)
            {
                case "docNumber":
                    paragrafo = doc.INFO_DOCUMENT.DOCNUMBER;
                    break;
                case "segnatura":
                    if (!string.IsNullOrEmpty(doc.INFO_DOCUMENT.NUMBER_PROTO))
                    {
                        paragrafo = doc.INFO_DOCUMENT.NUMBER_PROTO;
                    }
                    else
                    {
                        paragrafo = doc.INFO_DOCUMENT.DOCNUMBER;
                    }
                    break;
                case "oggetto":
                    if (!string.IsNullOrEmpty(doc.INFO_DOCUMENT.OBJECT))
                    {
                        paragrafo = doc.INFO_DOCUMENT.OBJECT;
                    }
                    else
                    {
                        paragrafo = "Descrizione oggetto mancante";
                    }
                    break;
                case "fascicolo":
                    if (doc.INFO_PROJECT != null && !string.IsNullOrEmpty(doc.INFO_PROJECT.CODE_PROJECT))
                    {
                        paragrafo = doc.INFO_PROJECT.CODE_PROJECT;
                    }
                    else
                    {
                        paragrafo = "Documenti non inseriti tramite fascicolo";
                    }
                    break;
                case "data":
                    paragrafo = infoDoc[doc.ID_INSTANCE_ACCESS_DOCUMENT].ToString();
                    break;
                case "fileName":
                    if (!string.IsNullOrEmpty(doc.INFO_DOCUMENT.FILE_NAME))
                    {
                        paragrafo = doc.INFO_DOCUMENT.FILE_NAME;
                    }
                    else
                    {
                        paragrafo = "nessun documento acquisito";
                    }
                    break;

                default:
                    paragrafo = doc.INFO_DOCUMENT.DOCNUMBER;
                    break;
            }

            return paragrafo;
        }

        private static string replaceInvalidChar(string path)
        {
            string resultPath = path;
            char[] invalid = System.IO.Path.GetInvalidPathChars();
            for (int i = 0; i < invalid.Length; i++)
            {
                resultPath = resultPath.Replace(invalid[i], '\u005F');
            }
            return resultPath;
        }

        private static string replaceInvalidCharFile(string fileName)
        {
            string resultPath = fileName;
            char[] invalid = System.IO.Path.GetInvalidFileNameChars();
            for (int i = 0; i < invalid.Length; i++)
            {
                resultPath = resultPath.Replace(invalid[i], '\u005F');
            }
            return resultPath;
        }

        /// <summary>
        /// Scrive su file XML i metadati da una stringa
        /// </summary>
        /// <param name="systemID"></param>
        /// <param name="fullName"></param>
        /// <param name="contenutoXML"></param>
        /// <returns></returns>
        private static bool saveMetadatiString(string fullName, string contenutoXML)
        {
            bool result = false;
            TextWriter textWr = null;
            XmlTextWriter XmlWr = null;
            try
            {
                string pathXml = fullName + ".xml";
                textWr = new StreamWriter(pathXml, false, Encoding.UTF8);
                XmlWr = new XmlTextWriter(textWr);


                if (!string.IsNullOrEmpty(contenutoXML))
                {
                    XmlWr.WriteRaw(contenutoXML);
                    result = true;
                }
                else
                    result = false;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
            finally
            {
                if (textWr != null)
                {
                    XmlWr.Flush();
                    textWr.Flush();
                    XmlWr.Close();
                    textWr.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// Comprime l'intera cartella dell'istanza di accesso
        /// </summary>
        /// <param name="idIstanza"></param>
        /// <param name="sourceDirecotry"></param>
        /// <returns></returns>
        private static bool ZipFolder(string idIstanza, string sourceDirecotry)
        {
            bool result = false;
            string zipFilePath = sourceDirecotry + ".ZIP";
            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }
            FastZip fastZip = new FastZip();
            try
            {
                fastZip.CreateZip(zipFilePath, sourceDirecotry, true, "");
                result = true;
            }
            catch (Exception exZip)
            {
                string err = "Errore nella creazione del file Zip dell'istanza di accesso. " + exZip.Message;
                logger.Debug(err);
                result = false;
            }

            return result;
        }

        #endregion


    }
}
