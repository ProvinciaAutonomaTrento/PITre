using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using DocsPaVO.utente;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using DocsPaVO.documento;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace DocsPaConservazione
{
    public class FileManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(FileManager));
        protected bool createZip;
        private string uniSincroXml = null;

        private void CreateDir(string path, bool dryRun)
        {
            if (!dryRun)
                Directory.CreateDirectory(path);

        }

        /// <summary>
        /// 
        /// </summary>
        private DocsPaVO.utente.InfoUtente _infoUtenteAsync = null;

        /// <summary>
        /// 
        /// </summary>
        private string _idConservazioneAsync = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private bool _isInPreparazioneAsync = false;

        /// <summary>
        /// 
        /// </summary>
        private string _noteIstanza = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        private int _copieSupportiRimovibili = 0;

        /// <summary>
        /// 
        /// </summary>
        public FileManager()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtenteAsync"></param>
        /// <param name="idConservazioneAsync"></param>
        /// <param name="noteIstanza"></param>
        /// <param name="copieSupportiRimovibili"></param>
        private FileManager(DocsPaVO.utente.InfoUtente infoUtenteAsync, string idConservazioneAsync, string noteIstanza, int copieSupportiRimovibili)
        {
            this._infoUtenteAsync = infoUtenteAsync;
            this._idConservazioneAsync = idConservazioneAsync;
            this._noteIstanza = noteIstanza;
            this._copieSupportiRimovibili = copieSupportiRimovibili;
        }

        /// <summary>
        /// 
        /// </summary>
        private delegate DocsPaVO.areaConservazione.esitoCons MettiInLavorazioneDelegate(string rootPath, string XmlName, string idCons, string ReadmePath, string ReadmePathWS, InfoUtente infoUtenteConservazione);

        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<string, FileManager> _fmanager = new Dictionary<string, FileManager>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        private static string GetDictKey(string idConservazione)
        {
            return idConservazione;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idConservazione"></param>
        /// <returns></returns>
        public static bool IsInPreparazioneAsync(string idConservazione)
        {
            string key = GetDictKey( idConservazione);

            if (_fmanager.ContainsKey(key))
            {
                // Task in esecuzione, reperimento della percentuale di esecuzione
                FileManager fm = _fmanager[key];

                return fm._isInPreparazioneAsync;
            }
            else
                return false;
        }

        internal static bool IsConsolidationEnabled()
        {
            string value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSOLIDAMENTO");

            if (!string.IsNullOrEmpty(value) && value.Equals("1"))
            {
                return value == "1";
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="XmlName"></param>
        /// <param name="idCons"></param>
        /// <param name="ReadmePath"></param>
        /// <param name="ReadmePathWS"></param>
        /// <param name="infoUtenteConservazione"></param>
        /// <returns></returns>
        public static void MettinInLavorazioneAsync(string rootPath, string XmlName, string idCons, string ReadmePath, string ReadmePathWS, InfoUtente infoUtenteConservazione, string noteIstanza, int copieSupportiRimovibili)
        {
            string key = GetDictKey(idCons);
            logger.Debug("inizio");
            DocsPaVO.Logger.CodAzione.Esito logResponse = DocsPaVO.Logger.CodAzione.Esito.KO;
            try
            {
                if (IsConsolidationEnabled())
                {
                    //Gestione del consolidamento
                    DocsPaConsManager consManager = new DocsPaConsManager();
                    DocsPaVO.areaConservazione.InfoConservazione[] infoConsList = consManager.getInfoConservazione(idCons);
                    if (infoConsList != null && infoConsList.Length > 0)
                    {
                        if (infoConsList[0].consolida)
                        {
                            DocsPaVO.areaConservazione.ItemsConservazione[] itemsConsList = consManager.getItemsConservazioneById(idCons, infoUtenteConservazione);
                            if (itemsConsList != null && itemsConsList.Length > 0)
                            {
                                foreach (DocsPaVO.areaConservazione.ItemsConservazione item in itemsConsList)
                                {
                                    if (!BusinessLogic.Documenti.DocumentConsolidation.IsDocumentConsoldated(infoUtenteConservazione, item.ID_Profile, DocumentConsolidationStateEnum.Step2))
                                    {
                                        BusinessLogic.Documenti.DocumentConsolidation.ConsolidateNoSecurity(infoUtenteConservazione, item.ID_Profile, DocumentConsolidationStateEnum.Step2);
                                    }
                                }
                            }
                        }
                    }
                    //Fine gestione del consolidamento
                }

                logger.Debug("Qui");
                if (!_fmanager.ContainsKey(key))
                {
                    FileManager fm = new FileManager(infoUtenteConservazione, idCons, noteIstanza, copieSupportiRimovibili);
                    fm._isInPreparazioneAsync = true;

                    // Avvio task di verifica asincrono
                    MettiInLavorazioneDelegate del = new MettiInLavorazioneDelegate(fm.MettinInLavorazione);
                    IAsyncResult result = del.BeginInvoke(rootPath, XmlName, idCons, ReadmePath, ReadmePathWS, infoUtenteConservazione,
                                new AsyncCallback(fm.MettinInLavorazioneExecuted), fm);

                    _fmanager.Add(key, fm);

                    logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;

                    // Il log della creazione dello storage deve essere spostato dall'inserimento in lavorazione
                    // Al caricamento effettivo, dopo la firma.
                    /*
                    BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtenteConservazione,
                              "CREAZIONE_STORAGE",
                              idCons,
                              String.Format("Memorizzazione nel repository del sistema di conservazione dell’istanza  {0}", idCons),
                              logResponse);

                    // Modifica scrittura Log e Registro di Conservazione per la scrittura della Creazione Storage
                    DocsPaVO.Conservazione.RegistroCons regCons = new DocsPaVO.Conservazione.RegistroCons();
                    regCons.idAmm = infoUtenteConservazione.idAmministrazione;
                    regCons.idIstanza = idCons;
                    regCons.tipoOggetto = "I";
                    regCons.tipoAzione = "";
                    regCons.userId = infoUtenteConservazione.userId;
                    regCons.codAzione = "CREAZIONE_STORAGE";
                    regCons.descAzione = "Memorizzazione nel repository del sistema di conservazione dell'istanza " + idCons;
                    regCons.esito = "1";
                    RegistroConservazione rc = new RegistroConservazione();
                    rc.inserimentoInRegistroCons(regCons, infoUtenteConservazione);
                    */

                }
                else
                {
                    // Task di metti in lavorazione già in esecuzione per l'utente
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore : {0} stk {1}", e.Message, e.StackTrace);
                throw e;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        private void MettinInLavorazioneExecuted(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                FileManager fm = (FileManager)result.AsyncState;
                
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    DocsPaConsManager consManager = new DocsPaConsManager();

                    if (!consManager.IsConservazioneInterna(fm._idConservazioneAsync))
                    {
                        DocsPaVO.areaConservazione.TipoSupporto[] tipiSupporto = consManager.getListaTipoSupporto();

                        // 1. Creazione del supporto remoto
                        int newIdSupportoRemoto;
                        if (consManager.SetDpaSupporto("0", "", "", "", "", "", "", "", "", "",
                                    fm._idConservazioneAsync,
                                    tipiSupporto.First(e => e.TipoSupp == "REMOTO").SystemId,
                                    StatoIstanza.IN_LAVORAZIONE,
                                    this._noteIstanza,
                                    "I",
                                    "",
                                    "",
                                    "",
                                    out newIdSupportoRemoto) == -1)
                        {

                        }

                        // 2. Creazione dei supporti rimovibili rimovibili
                        for (int i = 1; i <= fm._copieSupportiRimovibili; i++)
                        {
                            int newId = 0;
                            if (consManager.SetDpaSupporto(i.ToString(), "", "", "", "", "", "", "", "", "",
                                    fm._idConservazioneAsync,
                                    tipiSupporto.First(e => e.TipoSupp == "RIMOVIBILE").SystemId,
                                    StatoIstanza.IN_LAVORAZIONE,
                                    this._noteIstanza,
                                    "I",
                                    "",
                                    "",
                                    "",
                                    out newId) == -1)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        this._copieSupportiRimovibili = 0;
                    }

                    string values = " SET CHA_STATO='" + StatoIstanza.IN_LAVORAZIONE + "', VAR_NOTE='" + this._noteIstanza + "', COPIE_SUPPORTI='" + this._copieSupportiRimovibili + "' WHERE SYSTEM_ID='" + this._idConservazioneAsync + "'";
                    consManager.UpdateInfoConservazione(values);

                    transactionContext.Complete();
                }

                fm._isInPreparazioneAsync = false;

                string key = GetDictKey(fm._idConservazioneAsync);

                // Rimozione dal dictionary del task di metti in lavorazione
                if (_fmanager.ContainsKey(key))
                {
                    _fmanager.Remove(key);
                }
            }
        }

        /// <summary>
        /// Avvio il processo di conservazione per l'istanza passata come parametro
        /// </summary>
        /// <param name="rootPath">root directory di tutte le istanze di conservazione</param>
        /// <param name="XmlName">prefisso del file Xml generale dell'istanza di conservazione</param>
        /// <param name="idCons">id dell'istanza da sottomettere al processo di conservazione</param>
        /// <returns></returns>
        public DocsPaVO.areaConservazione.esitoCons MettinInLavorazione(string rootPath, string XmlName, string idCons, string ReadmePath, string ReadmePathWS, InfoUtente infoUtenteConservazione)
        {
            //nuova gestione dei messaggi di errore **********************************
            logger.Debug("MettinInLavorazione - BEGIN");
            DocsPaVO.areaConservazione.esitoCons result = new DocsPaVO.areaConservazione.esitoCons();
            int check = 0;
            result.esito = true;
            result.messaggio = "Errore nel reperimento dei documenti numero: ";
            string PathFasc = string.Empty;
            SaveFolder sf = null;
            DocsPaVO.areaConservazione.ItemsConservazione[] itemsConsList;
            DocsPaVO.areaConservazione.ItemsConservazione[] appoItems;
            ArrayList items = new ArrayList();

            DocsPaConsManager consManager = new DocsPaConsManager();
            DocsPaVO.areaConservazione.InfoConservazione[] infoConsList = consManager.getInfoConservazione(idCons);
          
                   
            //Recupero tutte le istanze di conservazione sottoscritte
            if (infoConsList != null && infoConsList.Length > 0)
            {
                for (int i = 0; i < infoConsList.Length; i++)
                {
                    bool dryRun = false; //Non crea file o cartelle
                    
                    DocsPaVO.areaConservazione.InfoConservazione infoCons = infoConsList[i];

                    if (infoCons.TipoConservazione.ToUpper() == TipoIstanzaConservazione.CONSERVAZIONE_INTERNA.ToString().ToUpper())
                        dryRun = true;

             //       dryRun = true;
                    InfoUtente infoUtente = consManager.getInfoUtenteConservazione(infoCons);
                    itemsConsList = consManager.getItemsConservazioneById(infoCons.SystemID, infoUtente);
                    //Costrisco il path dell'istanza
                    string istancePath = Path.Combine(rootPath, BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoCons.IdAmm).Codice);
                    istancePath = Path.Combine(istancePath, infoCons.SystemID);
                    string chiusuraPath = Path.Combine(istancePath, "Chiusura");
                    
                    //Controlla la path dell'Istanza, se non esiste la crea
                    istancePath = replaceInvalidChar(istancePath);
                    if (!Directory.Exists(istancePath))
                        CreateDir(istancePath,dryRun);

                    chiusuraPath = replaceInvalidChar(chiusuraPath);
                    if (!Directory.Exists(chiusuraPath))
                        CreateDir(chiusuraPath,dryRun);
      

                    //Mi creo il file XML dell'istanza
                    DocsPaConservazione.Metadata.XmlIstanza istanza = new Metadata.XmlIstanza(infoCons, infoUtenteConservazione);
                    //Ci salva dentro il file XML dell'istanza
                    if (!dryRun)
                        new InfoDocXml().saveMetadatiString(Path.Combine(istancePath, "dati_istanza"), istanza.XmlFile);

                    //Per ciascuna istanza di conservazione salvo tutti i documenti con i relativi allegati
                    if (itemsConsList != null && itemsConsList.Length > 0)
                    {
                        string currPrj = string.Empty;
                        //appoItems = new DocsPaVO.areaConservazione.ItemsConservazione[itemsConsList.Length];
                        //cicla per tutti gli elementi delll'istanza di conservazione
                        DocsPaConservazione.Metadata.UniSincroFileInfo usfi = new Metadata.UniSincroFileInfo();
                        for (int j = 0; j < itemsConsList.Length; j++)
                        {
                            DocsPaVO.areaConservazione.ItemsConservazione itemsCons = itemsConsList[j];
                            //Prima di scrivere il documento su files system uso l'oggetto SaveFolder per estendere
                            //il root path con il percorso del fascicolo
                            if (!string.IsNullOrEmpty(itemsCons.ID_Project))
                            {
                                if (itemsCons.ID_Project != currPrj)
                                {

                                    sf = new SaveFolder(itemsCons.ID_Project, replaceInvalidChar(itemsCons.CodFasc));
                                    currPrj = itemsCons.ID_Project;

                                    
                                    {
                                        string pf = Path.Combine(istancePath, Path.Combine("Fascicoli", replaceInvalidChar(itemsCons.CodFasc)));
                                        pf = replaceInvalidChar(pf);
                                        if (!Directory.Exists(pf))
                                            CreateDir(pf,dryRun);
                                        DocsPaConservazione.Metadata.XmlFascicolo xfa = new Metadata.XmlFascicolo(infoCons, itemsCons.ID_Project, sf.folderTree);
                                        if (!dryRun)
                                            new InfoDocXml().saveMetadatiString(Path.Combine(pf, itemsCons.ID_Project), xfa.XmlFile);
                                       
                                    }
                                     
                                }
                                if (sf != null)
                                {
                                    PathFasc = Path.Combine("Fascicoli", sf.getFolderDocument(itemsCons.ID_Profile));   
                                }
                            }
                            else
                            {
                                //nuova struttura directory!!!!!!!!!!!
                                PathFasc = "";  
                            }

                      
                            check = putDocumenti_InConservazione(infoCons, ref itemsCons, infoUtente, rootPath, PathFasc, XmlName,usfi,dryRun);
                            if (check == 0)
                            {
                                //il documento esiste ma nn si riesce a recuperare il file
                                result.esito = false;
                                result.messaggio = result.messaggio + itemsCons.DocNumber + "; ";
                                BusinessLogic.Documenti.areaConservazioneManager.UpdateEsitoDpaItemsCons(itemsCons.SystemID, "0");                                
                               // return result;                                
                            }
                            else
                            {
                                if (check == -1)
                                {
                                    //se il documento è stato cancellato
                                    result.esito = false;
                                    result.messaggio = result.messaggio + itemsCons.DocNumber + "; ";//"Il documento numero: " + itemsCons.DocNumber + " è stato rimosso, non è più possibile metterlo in conservazione";
                                    BusinessLogic.Documenti.areaConservazioneManager.UpdateEsitoDpaItemsCons(itemsCons.SystemID, "2");                                                                   
                                    // return result;
                                }
                                else
                                {
                                    //appoItems[j] = itemsCons;
                                    items.Add(itemsCons);
                                    BusinessLogic.Documenti.areaConservazioneManager.UpdateEsitoDpaItemsCons(itemsCons.SystemID, "1");                                
                                }
                            }
                        }

                        //creazione file chiusura secondo standard unisincro
                        DocsPaConservazione.Metadata.XmlUniSincro xus = new Metadata.XmlUniSincro(usfi.FileList, infoUtenteConservazione, infoCons);
                        consManager.SaveIstanzaBinaryField(infoCons.SystemID, "VAR_FILE_CHIUSURA", xus.XmlFile);

                        if (!dryRun)
                        {
                            new InfoDocXml().saveMetadatiString(Path.Combine(chiusuraPath, "file_chiusura"), xus.XmlFile);
                            new DocsPaConservazione.Schemi.WriteSchemi(PathManager.GetFolderSchemi(idCons));
                            new DocsPaConservazione.staticfiles.WriteStaticFiles(PathManager.GetFolderStaticFiles(idCons));

                            appoItems = new DocsPaVO.areaConservazione.ItemsConservazione[items.Count];
                            items.CopyTo(appoItems);
                            if (appoItems.Length > 0)
                            {
                                //Crea l'indice ed i relativi file html
                                string HtmlPath = Path.Combine(rootPath, BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoCons.IdAmm).Codice);
                                HtmlPath = Path.Combine(HtmlPath, infoCons.SystemID);

                                //nuova struttura directory!!!!!!!!!!!
                                //normalizzo il percorso eliminando i caratteri speciali
                                string HtmlDir = Path.Combine(HtmlPath, "html");
                                HtmlDir = replaceInvalidChar(HtmlDir);
                                if (!Directory.Exists(HtmlDir))
                                {
                                    CreateDir(HtmlDir,dryRun);
                                }
                                
                                string Ente = BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoCons.IdAmm).Descrizione;
                                string tipoIstanza = (from a in TipoIstanzaConservazione.Tipi where a.Codice == infoCons.TipoConservazione.ToUpper() select a.Descrizione).FirstOrDefault();

                                IndexFolderHtml makeHtml = new IndexFolderHtml(appoItems, HtmlPath, idCons, tipoIstanza, Ente, infoCons.Note);
                                
                                //Scrivo i files Readme
                                new DocsPaConservazione.Readme.WriteReadme(PathManager.GetFolderReadme(idCons));

                            }
                        }
                        else
                        {
                            result.messaggio = "Non è stato possibile recuperare alcun documento";
                        }

                        // Creazione rapporto di versamento
                        try
                        {
                            if (!consManager.RapportoVersamentoCreate(infoCons))
                            {
                                throw new Exception("Errore nel salvataggio del rapporto di versamento");
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.DebugFormat("Errore nella creazione del rapporto di versamento! {0} \r\n {1}", ex.Message, ex.StackTrace);
                            throw ex;
                        }

                    }
                }
            }
            //nuova gestione dei messaggi di errore **********************************
            DocsPaVO.Logger.CodAzione.Esito logResponse = DocsPaVO.Logger.CodAzione.Esito.KO;
            if(result.esito)
            {
                result.messaggio = "Istanza di conservazione creata con successo!";
                logResponse= DocsPaVO.Logger.CodAzione.Esito.OK;
            }
           //result.esito = false;


         //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(
         //            infoUtenteConservazione,
         //           "ACCETTAZIONE_ISTANZA",
         //           idCons,
         //           String.Format("Accettazione istanza {0} ", idCons),
         //           logResponse);

            return result;
        }
        /// <summary>
        /// Questo metodo si occupa di salvare nella folder di conservazione il documento passato come
        /// parametro con tutti i relativi allegati
        /// </summary>
        /// <param name="infoCons"></param>
        /// <param name="itemsCons"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        protected int putDocumenti_InConservazione(DocsPaVO.areaConservazione.InfoConservazione infoCons, ref DocsPaVO.areaConservazione.ItemsConservazione itemsCons, DocsPaVO.utente.InfoUtente infoUtente, string pathCons, string pathFasc, string fileXml,Metadata.UniSincroFileInfo usfi,bool dryRun)
        {

            logger.Debug("BEGIN");
            int result = 1;
            string err = string.Empty;
            DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();
                                    
            DocsPaVO.documento.SchedaDocumento sch = new DocsPaVO.documento.SchedaDocumento();
            sch = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, itemsCons.ID_Profile);
            DocsPaVO.documento.FileDocumento fd = null;
            
            //fa il controllo su scheda.inCestino ed in caso avvisa della rimozione del documento con
            //result a -1.
            if (sch.inCestino == null)
            {
                sch.inCestino = string.Empty;
            }

            if (sch.inCestino != "1")
            {
                try
                {
                    //In questo modo recupero, se esiste, il file fisico associato all'ultima versione del documento
                    if (sch.documenti != null && sch.documenti[0] != null &&
                        Int32.Parse(((DocsPaVO.documento.FileRequest)sch.documenti[0]).fileSize) > 0)
                    {
                        try
                        {
                            DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)sch.documenti[0];
                            fd = BusinessLogic.Documenti.FileManager.getFileFirmato(fr, infoUtente, true);
                            if (fd == null)
                                throw new Exception("Errore nel reperimento del file principale.");

                            //salvataggio nella relativa cartella di conservazione del file
                            if (!saveFile(fd, infoCons, ref itemsCons, sch, true, pathCons, fr, pathFasc, fileXml,usfi,dryRun))
                                throw new Exception("Errore nella scrittura del file principale.");

                            //NUOVA FUNZIONALITA': RICHIESTO TIMESTAMP ASSOCIATI AD OGNI DOCUMENTO
                            ArrayList timestampArray = timestampDoc.getTimestampsDoc(infoUtente, fr);// timestampDoc.getTimestampsDoc(infoUtente, fr);
                            if (timestampArray.Count > 0)
                            {

                                for (int i = 0; i < timestampArray.Count; i++)
                                {
                                    DocsPaVO.documento.TimestampDoc tmdoc = (DocsPaVO.documento.TimestampDoc)timestampArray[i];
                                    if (!saveTSDoc(tmdoc, infoCons, ref itemsCons, sch, pathCons, pathFasc,dryRun))
                                        throw new Exception("Errore nella scrittura del timestamp.");
                                }
                            }
                            result = 1;
                        }
                        catch (Exception ex)
                        {
                            err = "Errore nel reperimento del file principale : " + ex.Message;
                            logger.Debug(err);
                            result = 0;
                        }
                        //Recupero tutti gli allegati associati al documento corrente
                        for (int i = 0; sch.allegati != null && i < sch.allegati.Count; i++)
                        {
                            DocsPaVO.documento.Allegato documentoAllegato = (DocsPaVO.documento.Allegato)sch.allegati[i];
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
                                    if (!saveFile(fdAll, infoCons, ref itemsCons, sch, false, pathCons, frAll, pathFasc, fileXml, usfi,dryRun))
                                        throw new Exception("Errore nella scrittura del file allegato.");

                                    //NUOVA FUNZIONALITA': RICHIESTO TIMESTAMP ASSOCIATI AD OGNI ALLEGATO
                                    ArrayList timestampArray = timestampDoc.getTimestampsDoc(infoUtente, frAll);
                                    if (timestampArray.Count > 0)
                                    {
                                        for (int iAll = 0; iAll < timestampArray.Count; iAll++)
                                        {
                                            DocsPaVO.documento.TimestampDoc tmdoc = (DocsPaVO.documento.TimestampDoc)timestampArray[iAll];
                                            itemsCons.path_allegati[i] = itemsCons.path_allegati[i].ToString() + "@" + tmdoc.DOC_NUMBER + "-" + tmdoc.NUM_SERIE;
                                            
                                            if (!saveTSAll(fdAll, tmdoc, infoCons, ref itemsCons, sch, pathCons, pathFasc,dryRun))
                                                throw new Exception("Errore nella scrittura del timestamp.");
                                        }
                                    }
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
                catch (Exception exGen)
                {
                    err = "Errore nel reperimento del file principale : " + exGen.Message;
                    logger.Debug(err);
                    result = 0;
                }
            }
            else
            {
                result = -1;
            }
            return result;
        }


        /// <summary>
        /// Questo metodo salva il file fisico associato all'oggetto fileDoc nell'apposita cartella ricavando
        /// l'informazione del percorso a partire dall'oggetto InfoConservazione e DocNumber
        /// </summary>
        /// <param name="fileDoc"></param>
        /// <param name="infoCons"></param>
        /// <param name="itemsCons"></param>
        /// <param name="schDoc"></param>
        /// <param name="isDoc">se �è true salva XML metadati non si usa per gli allegati</param>
        /// <param name="root_path"></param>
        /// <param name="objFileRequest"></param>
        /// <returns></returns>
        protected  bool saveFile(DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.areaConservazione.InfoConservazione infoCons, ref DocsPaVO.areaConservazione.ItemsConservazione itemsCons, DocsPaVO.documento.SchedaDocumento schDoc, bool isDoc, string root_path, DocsPaVO.documento.FileRequest objFileRequest, string path_fasc, string XmlName, Metadata.UniSincroFileInfo  usfi, bool dryRun)
        {
            bool result = false;
            string err = string.Empty;
            string fullName = string.Empty;
            root_path = Path.Combine(root_path, BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoCons.IdAmm).Codice);
            root_path = Path.Combine(root_path, infoCons.SystemID);

            //nuova struttura directory!!!!!!!!!!!
            string rootXml = Path.Combine(root_path, "Chiusura");
            //string rootXml = root_path;
            string pathDoc = schDoc.docNumber;

            if (!string.IsNullOrEmpty(path_fasc))
            {
                root_path = Path.Combine(root_path, path_fasc);
                pathDoc = Path.Combine(path_fasc, Path.Combine("Documenti", schDoc.docNumber));
            }
            else
            {
                pathDoc = Path.Combine("Documenti", schDoc.docNumber);
            }

            //Creo una sottocartella (nominata con il DocNumber) per contenere anche gli allegati ed i metadati
            string path_cons = Path.Combine(root_path, Path.Combine("Documenti", schDoc.docNumber));

            //Inizializzo il path relativo del supporto destinato alla Conservazione ed elimino i caratteri speciali
            pathDoc = replaceInvalidChar(pathDoc);
            string path_supporto = '\u005C'.ToString() + pathDoc;

            //se è un allegato devo creare la sottocartella per gli allegati
            bool isAll = !isDoc;
            if (isAll)
            {
                path_cons = Path.Combine(path_cons, "Allegati");
                path_supporto = Path.Combine(path_supporto, "Allegati");
            }

            //normalizzo il percorso eliminando i caratteri speciali
            path_cons = replaceInvalidChar(path_cons);

            if (!Directory.Exists(path_cons))
            {
                CreateDir(path_cons,dryRun);
            }

            //nuova struttura directory!!!!!!!!!!!
            //normalizzo il percorso eliminando i caratteri speciali
            rootXml = replaceInvalidChar(rootXml);

            if (!Directory.Exists(rootXml))
            {
                CreateDir(rootXml,dryRun);
            }

            //normalizzo il nome file eliminando i caratteri speciali
           
            /*
            string fileName = replaceInvalidCharFile(schDoc.docNumber +  System.IO.Path.GetExtension(fileDoc.name));

            if (isAll)
            {
                fileName = replaceInvalidCharFile(objFileRequest.docNumber + System.IO.Path.GetExtension(fileDoc.name));
            }
            */
            string fileName = replaceInvalidCharFile(fileDoc.name);

            fullName = path_cons + '\u005C'.ToString() + fileName;
            path_supporto = Path.Combine(path_supporto, fileName);

            //aggiungo il path relativo del file per il link Html
            if (isDoc)
            {
                itemsCons.pathCD = path_supporto;
                //aggiunta di ulteriori campi all'oggetto itemsCons
                addFieldsToItemsCons(schDoc, ref itemsCons);
            }
            else
            {
                //aggiungo il path relativo per tutti gli allegati del documento corrente
                itemsCons.path_allegati.Add(path_supporto);
            }

            FileStream file = null;
            try
            {
                if (!dryRun)
                {
                    file = File.Create(fullName);
                    file.Write(fileDoc.content, 0, fileDoc.length);
                    result = true;
                }
                else
                {
                    result = true;
                }
                //Viene scritta l'informazione relativa al file corrente nel file Xml dell'istanza di conservazione
                InfoFolderXml infoFolder = new InfoFolderXml();
                //Devo passare come parametro il prefisso del file XML e tale parametro deve essere letto da Web.config
                string root_cons = Path.Combine(rootXml, XmlName + infoCons.SystemID + ".xml");
                //Creazione del file XML generale da firmare
 

                { //Add into unisincro file...
                    string impronta;
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    doc.GetImpronta(out impronta, objFileRequest.versionId, objFileRequest.docNumber);
                    usfi.addUniSincroFileInfo(fileDoc.contentType, objFileRequest.docNumber, path_supporto, impronta);
                }

          
                /*
                if (!infoFolder.setInfoFolderXml(objFileRequest, path_supporto, root_cons, schDoc, infoCons, isAll, itemsCons.CodFasc))
                    throw new Exception("Errore nella scrittura del file XML generale");
                 */
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
            //Se è il documento principale salvo i metadati su file XML
            
            if (isDoc)
            {
                try
                {
                    if (!dryRun)
                    {
                        DocsPaConservazione.Metadata.XmlDocumento xdoc = new Metadata.XmlDocumento(infoCons, fileDoc, schDoc, objFileRequest);
                        new InfoDocXml().saveMetadatiString(Path.Combine(path_cons, fileName), xdoc.XmlFile);
                    }
                    /*
                    InfoDocXml docXML = new InfoDocXml();
                    if (!docXML.saveMetadati(itemsCons.SystemID, fullName))
                        throw new Exception("Errore nella scrittura del file XML dei metadati.");
                     */
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

        protected  bool saveTSDoc(DocsPaVO.documento.TimestampDoc timeStamp, DocsPaVO.areaConservazione.InfoConservazione infoCons, ref DocsPaVO.areaConservazione.ItemsConservazione itemsCons, DocsPaVO.documento.SchedaDocumento schDoc, string root_path, string path_fasc,bool dryRun)
        {
            bool result = false;
            string err = string.Empty;
            string fullName = string.Empty;
            root_path = Path.Combine(root_path, BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoCons.IdAmm).Codice);
            root_path = Path.Combine(root_path, infoCons.SystemID);
            //nuova struttura directory!!!!!!!!!!!
            string rootXml = Path.Combine(root_path, "Chiusura");
            //string rootXml = root_path;
            string pathDoc = schDoc.docNumber;
            if (!string.IsNullOrEmpty(path_fasc))
            {
                root_path = Path.Combine(root_path, path_fasc);
                pathDoc = Path.Combine(path_fasc, Path.Combine("Documenti", schDoc.docNumber));
            }
            else
            {
                pathDoc = Path.Combine("Documenti", schDoc.docNumber);
            }

            //Creo una sottocartella (nominata con il DocNumber) per contenere anche gli allegati ed i metadati
            string path_cons = Path.Combine(root_path, Path.Combine("Documenti", schDoc.docNumber));

            //Inizializzo il path relativo del supporto destinato alla Conservazione ed elimino i caratteri speciali
            pathDoc = replaceInvalidChar(pathDoc);
            string path_supporto = '\u005C'.ToString() + pathDoc;

            ////se è un allegato devo creare la sottocartella per gli allegati
            //bool isAll = !isDoc;
            //if (isAll)
            //{
            //    path_cons = Path.Combine(path_cons, "allegati");
            //    path_supporto = Path.Combine(path_supporto, "allegati");
            //}

            //path_cons = Path.Combine(path_cons, "timestamp");
            //path_supporto = Path.Combine(path_supporto, "timestamp");

            //normalizzo il percorso eliminando i caratteri speciali
            path_cons = replaceInvalidChar(path_cons);

            if (!Directory.Exists(path_cons))
            {
                CreateDir(path_cons,dryRun);
            }

            //nuova struttura directory!!!!!!!!!!!
            //normalizzo il percorso eliminando i caratteri speciali
            rootXml = replaceInvalidChar(rootXml);

            if (!Directory.Exists(rootXml))
            {
                CreateDir(rootXml,dryRun);
            }
            itemsCons.pathTimeStamp = path_supporto;
           

            //normalizzo il nome file eliminando i caratteri speciali
            string fileName = replaceInvalidCharFile("TS"+timeStamp.DOC_NUMBER+ "-" +timeStamp.NUM_SERIE+".tsr");

            fullName = path_cons + '\u005C'.ToString() + fileName;
            path_supporto = Path.Combine(path_supporto, fileName);

            ////aggiungo il path relativo del file per il link Html
            //if (isDoc)
            //{
            //    itemsCons.pathCD = path_supporto;
            //    //aggiunta di ulteriori campi all'oggetto itemsCons
            //    addFieldsToItemsCons(schDoc, ref itemsCons);
            //}
            //else
            //{
            //    //aggiungo il path relativo per tutti gli allegati del documento corrente
            //    itemsCons.path_allegati.Add(path_supporto);
            //}

            FileStream file = null;
            try
            {
                if (!dryRun)
                {
                    file = File.Create(fullName);
                    file.Write(Convert.FromBase64String(timeStamp.TSR_FILE), 0, Convert.FromBase64String(timeStamp.TSR_FILE).Length);
                    result = true;
                }
                ////Viene scritta l'informazione relativa al file corrente nel file Xml dell'istanza di conservazione
                //InfoFolderXml infoFolder = new InfoFolderXml();
                ////Devo passare come parametro il prefisso del file XML e tale parametro deve essere letto da Web.config
                //string root_cons = Path.Combine(rootXml, XmlName + infoCons.SystemID + ".xml");
                ////Creazione del file XML generale da firmare
                //if (!infoFolder.setInfoFolderXml(objFileRequest, path_supporto, root_cons, schDoc, infoCons, isAll, itemsCons.CodFasc))
                //    throw new Exception("Errore nella scrittura del file XML generale");
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
            ////Se è il documento principale salvo i metadati su file XML
            //if (isDoc)
            //{
            //    try
            //    {
            //        InfoDocXml docXML = new InfoDocXml();
            //        if (!docXML.saveMetadati(itemsCons.SystemID, fullName))
            //            throw new Exception("Errore nella scrittura del file XML dei metadati.");
            //    }
            //    catch (Exception eXml)
            //    {
            //        err = "Errore nella scrittura del file XML dei metadati." + eXml.Message;
            //        logger.Debug(err);
            //        result = false;
            //    }
            //}
            return result;
        }

        protected  bool saveTSAll(DocsPaVO.documento.FileDocumento fdAll, DocsPaVO.documento.TimestampDoc timeStamp, DocsPaVO.areaConservazione.InfoConservazione infoCons, ref DocsPaVO.areaConservazione.ItemsConservazione itemsCons, DocsPaVO.documento.SchedaDocumento schDoc, string root_path, string path_fasc,bool dryRun)
        {
            bool result = false;
            string err = string.Empty;
            string fullName = string.Empty;
            root_path = Path.Combine(root_path, BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoCons.IdAmm).Codice);
            root_path = Path.Combine(root_path, infoCons.SystemID);
            //nuova struttura directory!!!!!!!!!!!
            string rootXml = Path.Combine(root_path, "Chiusura");
            //string rootXml = root_path;
            string pathDoc = schDoc.docNumber;
            if (!string.IsNullOrEmpty(path_fasc))
            {
                root_path = Path.Combine(root_path, path_fasc);
                pathDoc = Path.Combine(path_fasc, Path.Combine("Documenti", schDoc.docNumber));
            }
            else
            {
                pathDoc = Path.Combine("Documenti", schDoc.docNumber);
            }

            //Creo una sottocartella (nominata con il DocNumber) per contenere anche gli allegati ed i metadati
            string path_cons = Path.Combine(root_path, Path.Combine("Documenti", schDoc.docNumber));

            //Inizializzo il path relativo del supporto destinato alla Conservazione ed elimino i caratteri speciali
            pathDoc = replaceInvalidChar(pathDoc);
            string path_supporto = '\u005C'.ToString() + pathDoc;

            path_cons = Path.Combine(path_cons, "Allegati");
            path_supporto = Path.Combine(path_supporto, "Allegati");

            //normalizzo il percorso eliminando i caratteri speciali
            path_cons = replaceInvalidChar(path_cons);

            if (!Directory.Exists(path_cons))
            {
                CreateDir(path_cons,dryRun);
            }

            //nuova struttura directory!!!!!!!!!!!
            //normalizzo il percorso eliminando i caratteri speciali
            rootXml = replaceInvalidChar(rootXml);

            if (!Directory.Exists(rootXml))
            {
                CreateDir(rootXml,dryRun);
            }

            //normalizzo il nome file eliminando i caratteri speciali
            //DA CONTROLLARE
            
            string nomeAll = fdAll.fullName.Substring(0, fdAll.fullName.Length - 4);
            path_supporto = Path.Combine(path_supporto, nomeAll);
            string fileName = replaceInvalidCharFile("TSAll-" + nomeAll + "-" + timeStamp.DOC_NUMBER + "-" + timeStamp.NUM_SERIE + ".tsr");

            fullName = path_cons + '\u005C'.ToString() + fileName;
            path_supporto = Path.Combine(path_supporto, fileName);

            ////aggiungo il path relativo del file per il link Html
            //if (isDoc)
            //{
            //    itemsCons.pathCD = path_supporto;
            //    //aggiunta di ulteriori campi all'oggetto itemsCons
            //    addFieldsToItemsCons(schDoc, ref itemsCons);
            //}
            //else
            //{
            //    //aggiungo il path relativo per tutti gli allegati del documento corrente
            //    itemsCons.path_allegati.Add(path_supporto);
            //}
            if (!dryRun)
            {
                FileStream file = null;
                try
                {
                    file = File.Create(fullName);
                    file.Write(Convert.FromBase64String(timeStamp.TSR_FILE), 0, Convert.FromBase64String(timeStamp.TSR_FILE).Length);
                    result = true;

                    ////Viene scritta l'informazione relativa al file corrente nel file Xml dell'istanza di conservazione
                    //InfoFolderXml infoFolder = new InfoFolderXml();
                    ////Devo passare come parametro il prefisso del file XML e tale parametro deve essere letto da Web.config
                    //string root_cons = Path.Combine(rootXml, XmlName + infoCons.SystemID + ".xml");
                    ////Creazione del file XML generale da firmare
                    //if (!infoFolder.setInfoFolderXml(objFileRequest, path_supporto, root_cons, schDoc, infoCons, isAll, itemsCons.CodFasc))
                    //    throw new Exception("Errore nella scrittura del file XML generale");
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
            else
            {
                result = true;
            }
            return result;
        }


        #region upload_download

        private byte[] strToByte(string inVal)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, inVal);
            ms.Seek(0, 0);
            return  ms.ToArray();
        }

        /// <summary>
        /// Download del file xml da firmare se è stata avviata la procedura di conservazione
        /// </summary>
        /// <param name="idCons"></param>
        /// <returns></returns>
        public byte[] downloadSignXml(string idCons,InfoUtente Utente)
        {
            string err = string.Empty;
            string xmlChiusura = PathManager.GetPathFileChiusura(Utente, idCons);
            //string root_cons = Path.Combine(rootXml, XmlName + idCons + ".xml");

            byte[] buffer = null;
            if (File.Exists(xmlChiusura))
            {
                try
                {
                    // leggo il file e restituisco un array di byte[]  
                    using (FileStream fs = new FileStream(xmlChiusura, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int)fs.Length);
                    }
                }
                catch (Exception ex)
                {
                    err = "Il file da firmare non è pronto per il download" + ex.Message;
                    logger.Debug(err);

                    return buffer;
                }
            }
            else
            {
                string usFile = new DocsPaConsManager().getUniSincroXmlFromDB(idCons);

                if (!String.IsNullOrEmpty(usFile))
                {
                    buffer = System.Text.Encoding.Default.GetBytes(usFile);

                }
                else
                {
                    err = "Il file da firmare non è stato creato"; 
                    logger.Debug(err);
                }
            }

            return buffer;
        }

        /// <summary>
        /// Upload del file xml firmato con conseguente richiesta di marca temporale e creazione del file
        /// zip dell'istanza di conservazione
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="XmlName"></param>
        /// <param name="idCons"></param>
        /// <param name="SignedContent"></param>
        /// <returns></returns>
        public bool uploadSignXml(string idCons, byte[] SignedContent, string MarcaBase64, string progressivoMarca,InfoUtente utente)
        {
            string err = string.Empty;
            bool result = false;
            bool checkTsr = false;
            bool estemporanea = new DocsPaConsManager().IsConservazioneInterna(idCons);

            string rootXml = PathManager.GetFolderChiusura(utente, idCons);

            //Verifica ed inserimento dei dati di firma
            bool check = checkXml(SignedContent, idCons);
            DocsPaConsManager dpaCM = new DocsPaConsManager();

            dpaCM.updateUniSincroP7MInDB(idCons, SignedContent);
            //dpaCM.updateUniSincroTSRInDB(idCons, MarcaBase64);

            if (!estemporanea)
            {
                if (Directory.Exists(rootXml))
                {
                    FileStream file = null;
                    try
                    {
                        file = File.Create(PathManager.GetPathFileChiusuraP7M(utente, idCons));
                        file.Write(SignedContent, 0, SignedContent.Length);

                        string enableTSA = "0";

                        if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ENABLE_TIMESTAMP"]))
                        {
                            enableTSA = ConfigurationManager.AppSettings["ENABLE_TIMESTAMP"].ToString();
                        }

                        if (enableTSA == "1")
                        {
                            //Viene creato il file xml finale comprendente la conversione base64 del file p7m e tsr; 
                            //inoltre crea anche il file binario .tsr sempre nella directory principale.
                            TimestampManager tm = new TimestampManager();
                            checkTsr = tm.timeSignXml(utente, SignedContent, MarcaBase64, idCons, progressivoMarca);
                        }
                        else
                            checkTsr = true;

                    }
                    catch (Exception ex)
                    {
                        err = "Il file da firmare non è pronto per l'upload " + ex.Message;
                        logger.Debug(err);
                    }
                    finally
                    {
                        if (file != null)
                        {
                            file.Flush();
                            file.Close();
                        }
                    }
                    //Crea il file ZIP solo se è stata generata correttamente la marca temporale
                    if (checkTsr)
                    {
                        //Ok anche se non crea il file ZIP, nella fase di download se necessario viene rigenerato
                        result = true;
                        //Crea il file zip della corrente istanza di conservazione

                        //string pathCons = Path.Combine(PathManager.GetRootFolderAmministrazione(utente), idCons);
                        //string zipFilename = Path.Combine(rootPath, idCons + ".zip");
                       // if (!estemporanea)
                      //      createZip = ZipFolder(idCons);
                    }
                }
                else
                {
                    err = "La directory per l'upload non è accessibile o non esiste";
                    logger.Debug(err);
                }
            }
            else
            {
                // Chiusura istanza estemporanea
                DocsPaConsManager consManager = new DocsPaConsManager();
                consManager.ChiudiIstanzaEstemporanea(utente, idCons);

                result = true;
            }
            return result;
        }

        /// <summary>
        /// Rigenera la marca temporale dell'istanza di conservazione passata come parametro
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="XmlName"></param>
        /// <param name="idCons"></param>
        /// <param name="SignedContent"></param>
        /// <param name="MarcaBase64"></param>
        /// <returns></returns>
        public bool rigeneraMarca(string rootPath, string idCons, byte[] SignedContent, string MarcaBase64, string numeroMarca,InfoUtente utente)
        {
            string err = string.Empty;
            bool result = false;
            //string rootXml = Path.Combine(rootPath, utente.idAmministrazione );
            //rootXml = Path.Combine(rootXml, idCons);
            ////nuova struttura directory!!!!!!!!!!!
            //rootXml = Path.Combine(rootXml, "Chiusura");

            //modifica Lembo 16-11-2012
            //string dctmCServerAddressRoot = string.Empty;
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"]))
            //    dctmCServerAddressRoot = ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"].ToString();
            //else
            //{
            //    dctmCServerAddressRoot = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSERVAZIONE_REMOTE_STORAGE_URL");
            //} 
            string dctmCServerAddressRoot = DocsPaConsManager.getConservazioneRemoteStorageUrl();


            if (String.IsNullOrEmpty(dctmCServerAddressRoot))
            {

                string rootXml = PathManager.GetFolderChiusura(utente, idCons);

                if (Directory.Exists(rootXml))
                {
                    FileStream file = null;
                    try
                    {
                        //Viene creato il file xml finale comprendente la conversione base64 del file p7m e tsr; 
                        //inoltre crea anche il file binario .tsr sempre nella directory principale.
                        TimestampManager tm = new TimestampManager();
                        bool checkTsr = tm.timeSignXml(utente, SignedContent, MarcaBase64, idCons, numeroMarca);

                        //Crea il file ZIP solo se è stata generata correttamente la marca temporale
                        if (checkTsr)
                        {
                            //Ok anche se non crea il file ZIP, nella fase di download se necessario viene rigenerato
                            result = true;

                            //Crea il file zip della corrente istanza di conservazione
                            //string pathCons = Path.Combine(rootPath, idCons);
                            //string zipFilename = Path.Combine(rootPath, idCons + ".zip");

                            //createZip = ZipFolder(idCons);
                        }
                    }
                    catch (Exception ex)
                    {
                        err = "Errore nell'apposizione della marca temporale: " + ex.Message;
                        logger.Debug(err);
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
                else
                {
                    err = "La directory per l'upload non è accessibile";
                    logger.Debug(err);
                }


            }
            else
            {
                try
                {
                    byte[] base64 = Convert.FromBase64String(MarcaBase64);
                    httpSubmit hs = new httpSubmit();
                    result = hs.sendFile(PathManager.GetUrlFileChiusuraTSR(utente, idCons, numeroMarca), base64);

                }
                catch
                {
                    err = "Errore inviando il file allo store roemoto";
                    logger.Debug(err);
                    result = false;
                }
            }


            return result;
        }

        /// <summary>
        /// Restituisce true se recupera il nome del firmatario e se la verifica firma va a buon fine
        /// </summary>
        /// <param name="signedXml"></param>
        /// <returns></returns>
        private bool checkXml(byte[] signedXml, string idCons)
        {
            bool check = false;
            //Estrazione dei dati del firmatario e verifica del documento p7m
            try
            {
                CAPICOM.SignedDataClass si = new CAPICOM.SignedDataClass();
                //ASCIIEncoding ae = new ASCIIEncoding();
                //string Base64SignedXml = ae.GetString(signedXml);
                string Base64SignedXml = Convert.ToBase64String(signedXml);
                si.Verify(Base64SignedXml, false, CAPICOM.CAPICOM_SIGNED_DATA_VERIFY_FLAG.CAPICOM_VERIFY_SIGNATURE_ONLY);
                CAPICOM.Signer s = (CAPICOM.Signer)si.Signers[1];
                CAPICOM.Certificate cert = (CAPICOM.Certificate)s.Certificate;
                string nome = cert.GetInfo(CAPICOM.CAPICOM_CERT_INFO_TYPE.CAPICOM_CERT_INFO_SUBJECT_SIMPLE_NAME);
                //CAPICOM.UtilitiesClass ut = new CAPICOM.UtilitiesClass();

                //System.Security.Cryptography.Pkcs.ContentInfo ci = new ContentInfo(signedXml);
                //System.Security.Cryptography.Pkcs.SignedCms signedContent = new SignedCms(ci);
                //bool detach = signedContent.Detached;
                //signedContent.Decode(signedXml);
                //System.Security.Cryptography.X509Certificates.X509Certificate2 certificate = signedContent.Certificates[0];
                //string firmatario = certificate.SubjectName.Name; //DEVI PRENDERE IL CN!!!!

                string upInfoCons = " SET VAR_FIRMA_RESPONSABILE='" + nome + "' WHERE SYSTEM_ID='" + idCons + "'";
                DocsPaConsManager cm = new DocsPaConsManager();
                if (!cm.UpdateInfoConservazione(upInfoCons))
                    throw new Exception("Errore nell'inserimento dei dati del firmatario.");
                check = true;
            }
            catch (Exception exSign)
            {
                logger.Debug("Il file xml non è firmato e/o " + exSign);
            }
            return check;
        }

        /// <summary>
        /// Download del file zip dell'istanza di conservazione conseguenziale alla firma ed alla marcatura
        /// temporale
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="idCons"></param>
        /// <returns></returns>
        public void createZipFile(string idCons)
        {
            string err = string.Empty;
            
            createZip = ZipFolder(idCons);

            
            //if (File.Exists(zipFilename))
            //{
            //    try
            //    {
                    
            //        //// leggo il file e restituisco un array di byte[]  
            //        //using (FileStream fs = new FileStream(zipFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
            //        //{
            //        //    buffer = new byte[fs.Length];
            //        //    fs.Read(buffer, 0, (int)fs.Length);
            //        //}
            //    }
            //    catch (Exception ex)
            //    {
            //        err = "Il file zip non è pronto per il download " + ex.Message;
            //        logger.Debug(err);
            //        return null;
            //    }
            //}
            //else
            //{
            //    err = "Il file zip non è stato creato";
            //    logger.Debug(err);
            //}
            //return zipFilename;
        }

        /// <summary>
        /// Salva i files Readme
        /// </summary>
        /// <param name="rootPath"></param>
        /// <returns></returns>
        public bool salvaReadme(string ReadmePath, string idCons, string fileName, string rootPath, string ReadmePathWS)
        {
            string err = string.Empty;
            bool result = false;

            #region copia Readme da WS a path configurato
            string sourcePath0 = Path.Combine(ReadmePath, fileName);
            if (!File.Exists(sourcePath0))
            {
                if (!Directory.Exists(ReadmePath) && !string.IsNullOrEmpty(ReadmePath))
                {
                    try
                    {
                        Directory.CreateDirectory(ReadmePath);
                    }
                    catch (Exception exD)
                    {
                        err = "Non è stato possibile creare la directory configurata" + exD.Message;
                        logger.Debug(err);
                        ReadmePath = string.Empty;
                    }
                }
                if (!string.IsNullOrEmpty(ReadmePath))
                {
                    FileStream fs0 = null;
                    FileStream file0 = null;
                    try
                    {
                        string sourcePath = Path.Combine(ReadmePathWS, fileName);
                        fs0 = new FileStream(sourcePath, FileMode.Open);
                        byte[] buf = new byte[(int)fs0.Length];
                        fs0.Read(buf, 0, (int)fs0.Length);
                        string root_cons = Path.Combine(ReadmePath, fileName);
                        file0 = File.Create(root_cons);
                        file0.Write(buf, 0, buf.Length);

                        result = true;
                    }
                    catch (Exception ex)
                    {
                        err = "I files Readme non sono presenti nella cartella configurata " + ex.Message;
                        logger.Debug(err);
                    }
                    finally
                    {
                        if (file0 != null)
                        {
                            file0.Flush();
                            file0.Close();
                        }
                        if (fs0 != null)
                        {
                            fs0.Flush();
                            fs0.Close();
                        }
                    }
                }
                else
                {
                    ReadmePath = ReadmePathWS;
                }
            }
            #endregion

            string rootReadme = Path.Combine(rootPath, idCons);
            FileStream fs = null;
            FileStream file = null;
            try
            {
                string sourcePath = Path.Combine(ReadmePath, fileName);
                fs = new FileStream(sourcePath, FileMode.Open);
                byte[] buf = new byte[(int)fs.Length];
                fs.Read(buf, 0, (int)fs.Length);
                string root_cons = Path.Combine(rootReadme, fileName);
                file = File.Create(root_cons);
                file.Write(buf, 0, buf.Length);

                result = true;
            }
            catch (Exception ex)
            {
                err = "I files Readme non sono presenti nella cartella configurata " + ex.Message;
                logger.Debug(err);
            }
            finally
            {
                if (file != null)
                {
                    file.Flush();
                    file.Close();
                }
                if (fs != null)
                {
                    fs.Flush();
                    fs.Close();
                }
            }
            return result;
        }

        #endregion

        #region utils
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
        /// Invia l'istanza su un supporto remoto e rimuove il supporto locale
        /// </summary>
        /// <param name="idIstanza">Identificativo dell'istanza</param>
        /// <returns></returns>
        public bool SubmitToRemoteFolder(string idIstanza)
        {
            bool result = false;
            //modifica Lembo 16-11-2012
            //string dctmCServerAddressRoot = string.Empty;
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"]))
            //{
            //    dctmCServerAddressRoot = ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"].ToString();
            //}
            //else
            //{
            //    dctmCServerAddressRoot = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSERVAZIONE_REMOTE_STORAGE_URL");
            
            //}
            string dctmCServerAddressRoot = DocsPaConsManager.getConservazioneRemoteStorageUrl();

            if(string.IsNullOrEmpty(dctmCServerAddressRoot))
            {
                logger.Info("Storage remoto non configurato");
                return true;
            }
            
            DocsPaConsManager consManager = new DocsPaConsManager();
            if (!consManager.IsConservazioneInterna(idIstanza))
            {
                try
                {
                    string uploadDir = dctmCServerAddressRoot + "/" + PathManager.GetCodiceAmministrazione(idIstanza) + "/";
                    httpSubmit hs = new httpSubmit(PathManager.GetRootPathIstanza(idIstanza), uploadDir);
                    if (hs.Success)
                    {
                        //cancella istanza locale
                        // Gabriele Melini 19-12-2013
                        // bug allegati notifiche chiusura
                        // sposto la cancellazione della directory locale
                        // al termine dell'invio della notifica di chiusura
                        //Directory.Delete(PathManager.GetRootPathIstanza(idIstanza), true);
                        result = true;
                    }
                    else
                    {
                        string err = "Errore nell'invio dell'istanza allo storage remoto";
                        logger.Debug(err);
                        result = false;
                    }
                }
                catch (Exception ex)
                {
                    string err = "Errore nell'invio dell'istanza allo storage remoto. " + ex.Message;
                    logger.Debug(err);
                    result = false;
                }

            }
            return result;
        }

        /// <summary>
        /// Comprime l'intera cartella dell'istanza di conservazione e restituisce il relativo file zip
        /// </summary>
        /// <param name="pathFolder"></param>
        /// <param name="zipName"></param>
        /// <returns></returns>
        private bool ZipFolder(string idIstanza)
        {
            bool result = false;
            bool remoteStorage = false;
            //modifica Lembo 16-11-2012
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"])||!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSERVAZIONE_REMOTE_STORAGE_URL")))
            //    remoteStorage = true;
            if(!string.IsNullOrEmpty(DocsPaConsManager.getConservazioneRemoteStorageUrl()))
                remoteStorage=true;

            DocsPaConsManager consManager = new DocsPaConsManager();
            if (!consManager.IsConservazioneInterna(idIstanza))
            {
                FastZip fastZip = new FastZip();
                bool recurse = true;
                string zipFile = PathManager.GetPathFileZip(idIstanza);

                try
                {
                    if (!remoteStorage)
                    {
                        fastZip.CreateZip(zipFile, PathManager.GetRootPathIstanza(idIstanza), recurse, "");
                    }
  
                    result = true;
                }
                catch (Exception exZip)
                {
                    string err = "Errore nella creazione del file Zip dell'istanza di conservazione. " + exZip.Message;
                    logger.Debug(err);
                    result = false;
                }
            }
            else
            {
                // L'istanza è senza supporto fisico, l'archivio non deve essere creato
                result = true;
            }

            return result;
        }

        //Metodo che aggiunge elementi dalla scheda documento all'oggetto item conservazione
        private static bool addFieldsToItemsCons(DocsPaVO.documento.SchedaDocumento scheda, ref DocsPaVO.areaConservazione.ItemsConservazione items)
        {
            bool result = false;
            
            if (scheda.creatoreDocumento != null)
            {
                string idCreatore = scheda.creatoreDocumento.idPeople;
                if (!string.IsNullOrEmpty(idCreatore))
                {
                    DocsPaConsManager cm = new DocsPaConsManager();
                    items.creatoreDocumento = cm.getFullName(idCreatore);
                    result = true;
                }
            }

            if (scheda.protocollo != null)
            {
                if (scheda.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloEntrata))
                {
                    DocsPaVO.documento.ProtocolloEntrata protE = (DocsPaVO.documento.ProtocolloEntrata)scheda.protocollo;
                    items.mittente = protE.mittente.descrizione;
                    result = true;
                }
                if (scheda.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloInterno))
                {
                    DocsPaVO.documento.ProtocolloInterno protI = (DocsPaVO.documento.ProtocolloInterno)scheda.protocollo;
                    items.mittente = protI.mittente.descrizione;
                    result = true;
                }
                if (scheda.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita))
                {
                    DocsPaVO.documento.ProtocolloUscita protU = (DocsPaVO.documento.ProtocolloUscita)scheda.protocollo;
                    items.mittente = protU.mittente.descrizione;
                    result = true;
                }
            }

            return result;
        }


        public string getFileHashFromStore(string idConservazione, string pathFile, bool localStore)
        {
            bool remoteStore = false;
            //modifica Lembo 16-11-2012
            //string dctmCServerAddressRoot = string.Empty;
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"]))
            //{
            //    dctmCServerAddressRoot = ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"].ToString();
            //    remoteStore = true;
            //}
            //else if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSERVAZIONE_REMOTE_STORAGE_URL")))
            //{
            //    dctmCServerAddressRoot = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSERVAZIONE_REMOTE_STORAGE_URL");
            //    remoteStore = true;
            //}
            string dctmCServerAddressRoot = DocsPaConsManager.getConservazioneRemoteStorageUrl();
            if (!string.IsNullOrEmpty(dctmCServerAddressRoot))
                remoteStore = true;

            if (remoteStore)
            {
                byte[] content = getFileFromStore(idConservazione, pathFile + ".sha256",false);
                System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
                return enc.GetString(content);
            }
            else
            {
                byte[] content = getFileFromStore(idConservazione, pathFile,true);
                SHA256Managed sha = new SHA256Managed();
                byte[] impronta = sha.ComputeHash(content);
                return BitConverter.ToString(impronta).Replace("-", "");
            }
        }
        /// <summary>
        /// Ricava un file dallo storage remoto, specificando l'istanza
        /// </summary>
        /// <param name="idConservazione">id conservazione</param>
        /// <param name="pathFile"> path relativo del file in conservazione</param>
        /// <returns>bytearray del file</returns>
        public byte[] getFileFromStore(string idConservazione, string pathFile, bool localStore)
        {

            bool remoteStore = false;
            
            string pathFileConservato = string.Empty;

            //modifica Lembo 16-11-2012
            // string.Empty;
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"]))
            //{
            //    dctmCServerAddressRoot = ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"].ToString();
            //    remoteStore = true;
            //}
            //else if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSERVAZIONE_REMOTE_STORAGE_URL")))
            //{
            //    dctmCServerAddressRoot = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSERVAZIONE_REMOTE_STORAGE_URL");
            //    remoteStore = true;
            //}
            string dctmCServerAddressRoot = DocsPaConsManager.getConservazioneRemoteStorageUrl();
            if (!string.IsNullOrEmpty(dctmCServerAddressRoot)) 
                remoteStore = true;

            if (localStore)
            {
                pathFileConservato = PathManager.GetRootPathIstanza(idConservazione) + pathFile;
            }
            else
            {
                if (!remoteStore)
                    pathFileConservato = PathManager.GetRootPathIstanza(idConservazione) + pathFile;
                else
                    pathFileConservato = String.Format("{0}{1}", PathManager.GetUrlIstanza(idConservazione), pathFile.Replace("\\", "/"));
            }

            byte[] content = null;

            if (!remoteStore)
            {
                if (File.Exists(pathFileConservato))
                    content = System.IO.File.ReadAllBytes(pathFileConservato);
            }
            else
            {
                content = new httpSubmit().downloadFile(pathFileConservato);
            }

            return content;

        }

        #endregion
    }

    public class httpSubmit
    {
        private static ILog logger = LogManager.GetLogger(typeof(httpSubmit));
        class submitFile
        {
            public string localPath;
            public Uri remotePath;
            public string sha256Hash;
            public bool success;
        }

        private List<submitFile> submitFileList = null;
        private string serverRoot;
        private string filepath;
        private bool success = false;

        public bool Success
        {
            get { return success; }
        }
        public string Filepath
        {
            get { return filepath; }
            set { filepath = value; }
        }

        private List<submitFile> SubmitFileList
        {
            get { return submitFileList; }
            set { submitFileList = value; }
        }

        public string ServerRoot
        {
            get { return serverRoot; }
            set { serverRoot = value; }
        }


        public httpSubmit()
        {
            
        }
        public httpSubmit(string Path)
        {
            SubmitFiles(Path, serverRoot);
        }
        public httpSubmit(string Path, string HttpServerRoot)
        {
            SubmitFiles(Path, HttpServerRoot);
        }
        

        private void SubmitFiles(string path, string serverRoot)
        {
            List<String> toProcess = getFilesFromDir(path);
            submitFileList = new List<submitFile>();
            foreach (String s in toProcess)
                submitFileList.Add(new submitFile { localPath = s, remotePath = makeUri(s, path, serverRoot), sha256Hash = CalcolaImpronta256(s) });

            sendFileList(submitFileList);

            success = true;
            foreach (submitFile sf in submitFileList)
                if (!sf.success)
                    success = false;

        }

        private List<string> getFilesFromDir(string path)
        {
            List<String> retval = new List<String>();
            foreach (string p in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
                retval.Add(p);

            return retval;
        }

        Uri makeUri(String file, String path, String svrRootUrl)
        {
            Uri urlRoot = new Uri(path);
            Uri urlWs = new Uri(svrRootUrl);
            String relative = urlRoot.MakeRelativeUri(new Uri(file)).ToString();
            Uri retval = new Uri(urlWs, relative);
            return retval;
        }

        public string CalcolaImpronta256(string filePath)
        {

            SHA256Managed sha = new SHA256Managed();
            byte[] impronta = sha.ComputeHash(File.ReadAllBytes(filePath));
            return BitConverter.ToString(impronta).Replace("-", "");
        }


        public byte[] downloadFile(string url)
        {
            logger.DebugFormat("DownloadFileFromStore {0}", url);
            var wc = new WebClient();
            byte[] cont= wc.DownloadData (new Uri(url));
            logger.DebugFormat("DownloadFileFromStore {0} ->{1}bytes", url,cont.Length);
            return cont;
        }

        public bool sendFile(string remotePath, byte[] contents)
        {
            logger.DebugFormat("sending {0} len: {1} ", remotePath, contents.Length);
            DateTime starttime = DateTime.Now;
            var wc = new WebClient();
            bool retval;
            SHA256Managed sha = new SHA256Managed();
            try
            {
                
                // Supera problemi di HTTPS con certificati scaduti o siti non validi
                // Mantiene attivo l'encryption
                ServicePointManager.ServerCertificateValidationCallback +=
                    delegate(object sender, X509Certificate certificate, X509Chain chain,
                    SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };



                byte[] impronta = sha.ComputeHash(contents);
                string localHash = BitConverter.ToString(impronta).Replace("-", "");

                byte[] response = wc.UploadData(remotePath, "PUT", contents);
                string hash = wc.DownloadString(remotePath + ".sha256");
                if (hash.ToLower () == localHash.ToLower ())
                    retval = true;
                else
                    retval = false;
            }
            catch 
            {
                retval = false;

            }
            string seconds = (DateTime.Now - starttime).TotalSeconds.ToString();
            logger.DebugFormat("sended {0} took {1} seconds", remotePath,seconds );
            return retval;

        }

        void sendFileList(List<submitFile> lista)
        {
            var wc = new WebClient();
            int tryCounter = 0;
            logger.DebugFormat("FileList Sending {0} files to storage", lista.Count.ToString());
            DateTime starttimeFileList = DateTime.Now;
            foreach (submitFile sf in lista)
            {
                logger.DebugFormat("sending {0} -> {1} ",sf.localPath , sf.remotePath );
                DateTime starttimeFile = DateTime.Now;

                tryCounter = 0;
                try
                {
                    do
                    {
                        byte[] response = wc.UploadData(sf.remotePath, "PUT", File.ReadAllBytes(sf.localPath));
                        string hash = wc.DownloadString(sf.remotePath + ".sha256");
                        if (hash.ToLower () == sf.sha256Hash.ToLower())
                            sf.success = true;
                        else
                        {
                            logger.ErrorFormat("error sending {0} , Bad Hash {1} : {2}", sf.localPath, sf.sha256Hash.ToUpper () , hash.ToUpper());
                            sf.success = false;
                            tryCounter++;
                        }
                        if (tryCounter >10)
                            break;
                    } while (sf.success == false);
                }
                catch (Exception e)
                {
                    logger.ErrorFormat("error sending {0} , ex {1}, st {2}", sf.localPath, e.Message, e.StackTrace);
                    sf.success = false;
                    break;
                    //se no ce potemo morì...
                }

                string secondsFile = (DateTime.Now - starttimeFile).TotalSeconds.ToString();
                logger.DebugFormat("sended {0} took {1} seconds", sf.localPath, secondsFile);
            }
            string secondsList = (DateTime.Now - starttimeFileList).TotalSeconds.ToString();
            logger.DebugFormat("FileList sended it took {0} seconds",  secondsList);
        }

    }
}
