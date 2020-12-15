using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using DocsPaVO.utente;
using DocsPaVO.documento;
using System.Linq;
using log4net;


namespace BusinessLogic.AcquisizioneMassiva
{
    /// <summary>
    /// Classe per la gestione dell'upload dei file acquisiti
    /// mediante la procedura di acquisizione massiva documenti
    /// </summary>
    public sealed class PutFileHelper
    {
        private static ILog logger = LogManager.GetLogger(typeof(PutFileHelper));
        /// <summary>
        /// 
        /// </summary>
        private PutFileHelper()
        { }

        #region Public methods


        public static bool isAquisito(string docNumber, string versionId)
        {
            string impronta=null;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            doc.GetImpronta(out impronta, versionId, docNumber);
            if (impronta.Length> 0)
                return true;

            return false;

        }
        /// <summary>
        /// Estrae le estensioni dal nome
        /// </summary>
        /// <param name="infile"></param>
        /// <returns></returns>
        static string getExts(string infile)
        {
            string fname = System.IO.Path.GetFileNameWithoutExtension(infile);

            if (fname.Contains('.'))
                fname = fname.Remove(fname.IndexOf('.'));

            string extname = infile.Replace(fname, string.Empty);
            return extname;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string PutFile(string xml)
        {
            string retval = null;
            try
            {
                XmlDocument xmlDoc = GetXmlDocument(xml);
                if (xmlDoc == null)
                    throw new ApplicationException("Errore nella lettura dello stream XML");

                string fileName = GetXmlSingleNodeValue("fileName", xmlDoc);
                string docNumber = GetXmlSingleNodeValue("docNumber", xmlDoc);
                string registroProtocollo = GetXmlSingleNodeValue("registroProtocollo", xmlDoc);
                string protocollo = GetXmlSingleNodeValue("protocollo", xmlDoc);
                string annoProtocollo = GetXmlSingleNodeValue("annoProtocollo", xmlDoc);
                string idAmministrazione = GetXmlSingleNodeValue("idAmministrazione", xmlDoc);
                string versionAlgo = GetXmlSingleNodeValue("versionAlgo", xmlDoc);
                string noteVersione = GetXmlSingleNodeValue("noteVersione", xmlDoc);

                //VersionAlgo = current /vuoto  -> Comportamento iniziale.
                //VersionAlgo = next   -> solo se esiste la versione iniziale.
                //VersionAlgo = append ->  aggiunge una versione. 

                if (!string.IsNullOrEmpty(docNumber) &&
                    !string.IsNullOrEmpty(registroProtocollo) &&
                    !string.IsNullOrEmpty(protocollo) && protocollo != "0" &&
                    !string.IsNullOrEmpty(annoProtocollo) && annoProtocollo != "0")
                {
                    // Reperimento docNumber dai dati identificativi del protocollo
                    docNumber = BusinessLogic.Documenti.DocManager.GetDocNumber(idAmministrazione, registroProtocollo, protocollo, annoProtocollo);
                }

                string numTotaleAllegati = GetXmlSingleNodeValue("numTotaleAllegati", xmlDoc);
                string numDiAllegati = GetXmlSingleNodeValue("numDiAllegati", xmlDoc);
                string numPagineAllegato = GetXmlSingleNodeValue("numPagineAllegato", xmlDoc);
                string idCorrGlobali = GetXmlSingleNodeValue("idCorrGlobali", xmlDoc);
                string idPeople = GetXmlSingleNodeValue("idPeople", xmlDoc);
                string userID = GetXmlSingleNodeValue("userID", xmlDoc);
                string idGruppo = GetXmlSingleNodeValue("idGruppo", xmlDoc);
                string dst = GetXmlSingleNodeValue("dst", xmlDoc);

                // Validazione parametri di input
                List<string> missingParams = new List<string>();

                if (string.IsNullOrEmpty(fileName)) missingParams.Add("Nome file mancante");
                if (string.IsNullOrEmpty(docNumber)) missingParams.Add("ID documento mancante");
                if (string.IsNullOrEmpty(numTotaleAllegati)) missingParams.Add("Numero totale allegati mancante");
                if (string.IsNullOrEmpty(numDiAllegati)) missingParams.Add("Numero di allegati mancante");
                if (string.IsNullOrEmpty(numPagineAllegato)) missingParams.Add("Numero pagine allegato mancante");
                if (string.IsNullOrEmpty(idCorrGlobali)) missingParams.Add("IdCorrGlobali mancante");
                if (string.IsNullOrEmpty(idPeople)) missingParams.Add("IdPeople mancante");
                if (string.IsNullOrEmpty(userID)) missingParams.Add("User ID mancante");
                if (string.IsNullOrEmpty(idGruppo)) missingParams.Add("IdGruppo mancante");
                if (string.IsNullOrEmpty(idAmministrazione)) missingParams.Add("IdAmministrazione mancante");
                if (string.IsNullOrEmpty(dst)) missingParams.Add("Dst mancante");

                if (missingParams.Count > 0)
                {
                    // Parametri mancanti
                    string missingParamMessage = string.Empty;
                    foreach (string param in missingParams)
                    {
                        if (missingParamMessage != string.Empty)
                            missingParamMessage += "; ";
                        missingParamMessage += param;
                    }
                    throw new ApplicationException(missingParamMessage);
                }

                // Controllo valori numerici
                CheckForNumericValue(docNumber, "docNumber");
                CheckForNumericValue(numTotaleAllegati, "numTotaleAllegati");
                CheckForNumericValue(numDiAllegati, "numDiAllegati");
                CheckForNumericValue(numPagineAllegato, "numPagineAllegato");
                CheckForNumericValue(idCorrGlobali, "idCorrGlobali");
                CheckForNumericValue(idPeople, "idPeople");
                CheckForNumericValue(idGruppo, "idGruppo");
                CheckForNumericValue(idAmministrazione, "idAmministrazione");

                // Creazione oggetto InfoUtente
                InfoUtente infoUtente = new InfoUtente();
                infoUtente.idCorrGlobali = idCorrGlobali;
                infoUtente.idPeople = idPeople;
                infoUtente.userId = userID;
                infoUtente.idGruppo = idGruppo;
                infoUtente.idAmministrazione = idAmministrazione;
                infoUtente.dst = dst;


                SchedaDocumento sd = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, docNumber);
                List<Documento> documenti = sd.documenti.Cast<Documento>().ToList();
                string segnatura = null;
                if (sd.protocollo != null)
                    segnatura = sd.protocollo.segnatura;

                //Controlli del caso:

                //Nel caso di versioni successive , la prima deve esistere e essere già aquisita, nel caso non lo sia da errore
            
                
                
                
                try
                {
                    bool docAquisito = isAquisito(docNumber, documenti.FirstOrDefault().versionId);
                    if (versionAlgo == "next")
                    {
                        if (!isAquisito(docNumber, documenti.FirstOrDefault().versionId))
                            throw new ApplicationException(string.Format("L’ultima versione del documento con identificativo {0} non risulta acquisita.", docNumber));
                    }
                    else if (versionAlgo == "" || versionAlgo == "current")
                    {
                        if (Int32.Parse(numDiAllegati) == 0)
                        {

                            if (isAquisito(docNumber, documenti.FirstOrDefault().versionId))
                            {
                                if (segnatura != null && segnatura != string.Empty)
                                    throw new ApplicationException(string.Format("Il documento con identificativo {0} e segnatura {1} risulta gia' acquisito.", docNumber, segnatura));
                                else
                                    throw new ApplicationException(string.Format("Il documento con identificativo {0} risulta gia' acquisito.", docNumber));

                            }
                        }
                        else
                        {
                            if (!BusinessLogic.Documenti.DocManager.DocumentoIsAcquisito(docNumber))
                                throw new ApplicationException(string.Format("Docnumber {0} - Allegato {1} - Non risulta acquisito il documento principale.", docNumber, numDiAllegati));
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Errore durante la verifica dell'acquisizione del documento:\n" + ex.Message);
                }

                // Reperimento percorso in cui effettuare l'upload dei documenti acquisiti
                string pathFile = Configurations.GetPathAcquisizioneBatch();

                if (!Directory.Exists(pathFile))
                    throw new ApplicationException("Directory di acquisizione inesistente, controllare il valore presente nella chiave 'pathAcquisizioneBatch' nel file web.config della WS.");
                //modifica
                string pathFileName = string.Empty;
              
                bool pathInCache = false;
                if (Documenti.CacheFileManager.isActiveCaching(idAmministrazione)) //server.MachineName))
                {
                    DocsPaVO.Caching.InfoFileCaching info = Documenti.CacheFileManager.getFileDaCache(docNumber, BusinessLogic.Documenti.VersioniManager.getLatestVersionID(docNumber, infoUtente), infoUtente.idAmministrazione);
                    if (info != null)
                    {
                        pathFileName = info.CacheFilePath;
                        pathInCache = true;
                    }
                 }


                if(!pathInCache)
                    pathFileName = string.Format(@"{0}\{1}", pathFile, fileName);
                //fine modifica
                if (!File.Exists(pathFileName))
                    throw new ApplicationException(string.Format("File '{0}' inesistente", pathFileName));

                try
                {
                    FileRequest fileReq = new Documento();
                    FileDocumento fileDoc = new FileDocumento();


                   
                    string response = BusinessLogic.Documenti.FileManager.PutFileBatchNoSecurity(fileName, docNumber, numTotaleAllegati,
                        numDiAllegati, numPagineAllegato, infoUtente, pathFileName, ref fileReq, ref fileDoc);

                    if (response.ToLower().Equals("y"))
                    {
                        if (numDiAllegati != "0")
                            // Inserimento allegato
                            fileReq = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(infoUtente, (DocsPaVO.documento.Allegato)fileReq);
                        else
                        {

                            // Reperimento VersionId
                            fileReq.versionId = BusinessLogic.Documenti.VersioniManager.getLatestVersionID(docNumber, infoUtente);
                            ((Documento)fileReq).daInviare = "1";

                            if (versionAlgo == "append" || versionAlgo == "next")
                            {
                                //Controllo se il documento risulta di prima acquisizione
                                if (isAquisito(docNumber, documenti.FirstOrDefault().versionId))
                                {
                                    string desc = noteVersione;
                                    if (String.IsNullOrEmpty(desc))
                                        desc = "Acquisito Massivamente";

                                    fileReq = new Documento();
                                    fileReq.docNumber = docNumber;
                                    fileReq.descrizione = desc;

                                    fileReq = BusinessLogic.Documenti.VersioniManager.addVersion(fileReq, infoUtente, false);
                                    if (fileReq != null)
                                    {
                                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIVERSIONE", fileReq.docNumber, string.Format("Aggiunta massivamente la versione {0} al documento {1}.", fileReq.version, fileReq.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                                        retval = fileReq.version;
                                    }
                                    else
                                    {
                                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOAGGIUNGIVERSIONE", fileReq.docNumber, string.Format("Aggiunta massivamente la versione {0} al documento {1}.", fileReq.version, fileReq.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                                    }
                                }
                                
                            }
                            
                        }

                        // Inserimento del file nel documentale
                        fileDoc.cartaceo = true;
                        fileDoc.name = String.Format("{0}_ACQMASSIVA{1}", Guid.NewGuid().ToString(), getExts(fileDoc.name));
                        fileReq = BusinessLogic.Documenti.FileManager.putFile(fileReq, fileDoc, infoUtente);
                        if (fileReq!=null)
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", fileReq.docNumber, string.Format("{0} {1}", "Acquisito massivamente il documento N.ro:", fileReq.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);

                    }
                    else
                    {
                        BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "DOCUMENTOPUTFILE", fileReq.docNumber, string.Format("{0} {1}", "Acquisito massivamente il documento N.ro:", fileReq.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                        throw new ApplicationException(response);
                    }
                }
                catch
                {
                    throw new ApplicationException("Errore durante l'acquisizione del documento/allegato");
                }

                // Dopo aver acquisito il file nel documentale, 
                // viene rimosso il file temporaneamente copiato nella cartella di appoggio del server
                try { System.IO.File.Delete(pathFileName); }
                catch { }
            }
            catch (Exception ex)
            {
                logger.Debug(string.Format("Errore in BusinessLogic.AcquisizioneMassiva.PutFileHelper:\\n{0}", ex.Message));

                throw ex;
            }
            return retval;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private static XmlDocument GetXmlDocument(string xml)
        {
            XmlDocument document = new XmlDocument();

            try
            {
                document.LoadXml(xml);
            }
            catch
            {
                document = null;
            }

            return document;
        }

        /// <summary>
        /// Validazione valore numerico
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        private static void CheckForNumericValue(string value, string name)
        {
            if (!IsNumericValue(value))
                throw new ApplicationException(string.Format("Il valore fornito per il parametro {0} deve essere numerico", name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        private static bool IsNumericValue(string value)
        {
            int result;
            return Int32.TryParse(value, out result);
        }

        /// <summary>
        /// Reperimento valore nodo xml
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="document"></param>
        /// <returns></returns>
        private static string GetXmlSingleNodeValue(string nodeName, XmlDocument document)
        {
            XmlNode node = document.DocumentElement.SelectSingleNode(nodeName);
            if (node != null)
                return node.InnerText;
            else
                return string.Empty;
        }

        #endregion
    }
}
