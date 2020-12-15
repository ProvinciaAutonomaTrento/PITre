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
using System.Xml;


namespace DocsPaConservazione
{
    public class EsibizioneFileManager
    {

        private static ILog logger = LogManager.GetLogger(typeof(EsibizioneFileManager));
        protected bool createZip;
        
        public EsibizioneFileManager()
        {
        }

        private DocsPaVO.utente.InfoUtente _infoUtenteAsync = null;

        private string _idEsibizioneAsync = string.Empty;

        private bool _isInPreparazioneAsync = false;

        private EsibizioneFileManager(DocsPaVO.utente.InfoUtente infoUtenteAsync, string idEsibizioneAsync)
        {
            this._infoUtenteAsync = infoUtenteAsync;
            this._idEsibizioneAsync = idEsibizioneAsync;

        }

        private delegate DocsPaVO.areaConservazione.esitoCons ChiudiIstanzaEsibizioneDelegate(string rootPath, string idEsibizione, InfoUtente infoUtente, bool localStore);

        private static Dictionary<string, EsibizioneFileManager> _fmanager = new Dictionary<string, EsibizioneFileManager>();


        private static string GetDictKey(string idEsibizione)
        {
            return idEsibizione;
        }


        public static void ChiudiIstanzaEsibizioneAsync(string rootPath, string idEsibizione, InfoUtente infoUtenteConservazione, bool localStore)
        {
            string key = GetDictKey(idEsibizione);
            logger.Debug("inizio");
            DocsPaVO.Logger.CodAzione.Esito logResponse = DocsPaVO.Logger.CodAzione.Esito.KO;
            try
            {

                logger.Debug("Qui");
                if (!_fmanager.ContainsKey(key))
                {
                    EsibizioneFileManager fm = new EsibizioneFileManager(infoUtenteConservazione, idEsibizione);
                    fm._isInPreparazioneAsync = true;

                    // Avvio task di verifica asincrono
                    
                    ChiudiIstanzaEsibizioneDelegate del = new ChiudiIstanzaEsibizioneDelegate(fm.ChiudiIstanzaEsibizione);
                    IAsyncResult result = del.BeginInvoke(rootPath, idEsibizione, infoUtenteConservazione, localStore, new AsyncCallback(fm.ChiudiIstanzaEsibizioneExecuted), fm);
                    
                    _fmanager.Add(key, fm);

                    logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;
                }
                else
                {
                    // Task già in esecuzione
                }
            }
            catch (Exception e)
            {
                logger.ErrorFormat("Errore : {0} stk {1}", e.Message, e.StackTrace);
                throw e;
            }
        }

        private void ChiudiIstanzaEsibizioneExecuted(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                EsibizioneFileManager fm = (EsibizioneFileManager)result.AsyncState;

            }

        }

        public DocsPaVO.areaConservazione.esitoCons ChiudiIstanzaEsibizione(string rootPath, string idEsibizione, InfoUtente infoUtenteEsib, bool isLocalStore)
        {

            //cablato
            //bool localstore = true;
            bool localstore = isLocalStore;
            bool isCertificata = false;

            DocsPaVO.areaConservazione.esitoCons result = new DocsPaVO.areaConservazione.esitoCons();

            int check = 0;
            result.esito = true;
            result.messaggio = "Errore nel reperimento dei documenti numero: ";
            string PathFasc = string.Empty;
            string indexPath = string.Empty;
            SaveFolder sf = null;

            DocsPaVO.areaConservazione.ItemsEsibizione[] itemsEsibList;
            DocsPaVO.areaConservazione.ItemsEsibizione[] appoItems;
            ArrayList items = new ArrayList();

            ArrayList searchFilters = new ArrayList();
            searchFilters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idIstanza", valore = idEsibizione });
            searchFilters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idAmm", valore = infoUtenteEsib.idAmministrazione });

            DocsPaConsManager consManager = new DocsPaConsManager();
            DocsPaVO.areaConservazione.InfoEsibizione[] infoEsibList = consManager.GetInfoEsibizione(infoUtenteEsib, searchFilters);

            //Recupero tutte le istanze di conservazione sottoscritte
            if (infoEsibList != null && infoEsibList.Length > 0)
            {
                
                bool dryRun = false; //Non crea file o cartelle

                DocsPaVO.areaConservazione.InfoEsibizione infoEsib = infoEsibList[0];

                //InfoUtente infoUtente = consManager.getInfoUtenteConservazione(infoCons);
                itemsEsibList = consManager.GetItemsEsibizione(infoUtenteEsib, idEsibizione);

                string esibizionePath = Path.Combine(rootPath, BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoEsib.IdAmm).Codice);
                esibizionePath = Path.Combine(esibizionePath, infoEsib.SystemID);
                indexPath = esibizionePath;
                string chiusuraPath = Path.Combine(esibizionePath, "Chiusura");

                //Controlla la path dell'Istanza, se non esiste la crea
                esibizionePath = replaceInvalidChar(esibizionePath);
                if (!Directory.Exists(esibizionePath))
                    CreateDir(esibizionePath, dryRun);

                //Per ciascuna istanza di conservazione salvo tutti i documenti con i relativi allegati
                if (itemsEsibList != null && itemsEsibList.Length > 0)
                {
                    string currPrj = string.Empty;
                    string istancePath = string.Empty;
                    //appoItems = new DocsPaVO.areaConservazione.ItemsConservazione[itemsConsList.Length];
                    
                    for (int j = 0; j < itemsEsibList.Length; j++)
                    {
                        DocsPaVO.areaConservazione.ItemsEsibizione itemsEsib = itemsEsibList[j];
                        //Prima di scrivere il documento su files system uso l'oggetto SaveFolder per estendere
                        //il root path con il percorso del fascicolo

                        //inserisco nel root path l'id conservazione del documento
                        istancePath = Path.Combine(esibizionePath, itemsEsib.ID_Conservazione);
                        itemsEsib.relative_path = Path.Combine(".", itemsEsib.ID_Conservazione);


                        //creo la directory relativa all'istanza di conservazione e ci inserisco il relativo file di chiusura
                        if (!Directory.Exists(istancePath))
                        {
                            CreateDir(istancePath, dryRun);

                            //string fileChiusuraXML = new DocsPaConsManager().GetIstanzaBinaryField(itemsEsib.ID_Conservazione, "VAR_FILE_CHIUSURA");
                            //string fullname = istancePath + '\u005C'.ToString() + "file_chiusura";
                            //bool esito = new InfoDocXml().saveMetadatiString(fullname, fileChiusuraXML);

                            //file di chiusura
                            bool esito1 = saveFileChiusura(itemsEsib.ID_Conservazione, istancePath, false, localstore, dryRun);
                            
                            //file di chiusura firmato
                            bool esito2 = saveFileChiusura(itemsEsib.ID_Conservazione, istancePath, true, localstore, dryRun);

                            bool esito = (esito1 && esito2);

                            if (!esito)
                            {
                                result.esito = false;
                                result.messaggio += " errore nella scrittura del file di chiusura dell'istanza di conservazione \n";
                            }

                            // Inserisco il rapporto di versamento dell'istanza
                            esito = this.saveRapportoVersamento(itemsEsib.ID_Conservazione, istancePath);
                            if (!esito)
                            {
                                result.esito = false;
                                result.messaggio += string.Format(" errore nella scrittura del rapporto di versamento dell'istanza di conservazione {0}\r\n", itemsEsib.ID_Conservazione);
                            }
                                                
                    

                        }

                        if (!string.IsNullOrEmpty(itemsEsib.ID_Project))
                        {

                            if (itemsEsib.ID_Project != currPrj)
                            {

                                sf = new SaveFolder(itemsEsib.ID_Project, replaceInvalidChar(itemsEsib.CodFasc));
                                currPrj = itemsEsib.ID_Project;

                                {
                                    string pf = Path.Combine(istancePath, Path.Combine("Fascicoli", replaceInvalidChar(itemsEsib.CodFasc)));
                                    itemsEsib.relative_path = Path.Combine(itemsEsib.relative_path, Path.Combine("Fascicoli", replaceInvalidChar(itemsEsib.CodFasc)));
                                    pf = replaceInvalidChar(pf);
                                    if (!Directory.Exists(pf))
                                        CreateDir(pf, dryRun);
                                    DocsPaConservazione.Metadata.XmlFascEsibizione xfasc = new Metadata.XmlFascEsibizione(infoEsib, itemsEsib.ID_Project, sf.folderTree);
                                    if (!dryRun)
                                        new InfoDocXml().saveMetadatiString(Path.Combine(pf, itemsEsib.ID_Project), xfasc.XmlFile);

                                }

                            }
                            if (sf != null)
                            {
                                PathFasc = Path.Combine("Fascicoli", sf.getFolderDocument(itemsEsib.ID_Profile));
                            }
                        }
                        else
                        {
                            //nuova struttura directory!!!!!!!!!!!
                            PathFasc = "";
                        }


                        //check = putDocumenti_InEsibizione(infoEsib, ref itemsEsib, infoUtenteEsib, rootPath, istancePath, XmlName, usfi, dryRun);
                        check = putDocumenti_InEsibizione(infoEsib, ref itemsEsib, infoUtenteEsib, istancePath, PathFasc, dryRun, localstore);
                        if (check == 0)
                        {
                            //il documento esiste ma nn si riesce a recuperare il file
                            result.esito = false;
                            result.messaggio = result.messaggio + itemsEsib.DocNumber + "; ";

                            // return result;                                
                        }
                        else
                        {
                            if (check == -1)
                            {
                                //se il documento è stato cancellato
                                result.esito = false;
                                result.messaggio = result.messaggio + itemsEsib.DocNumber + "; ";//"Il documento numero: " + itemsCons.DocNumber + " è stato rimosso, non è più possibile metterlo in conservazione";

                                // return result;
                            }
                            else
                            {
                                //appoItems[j] = itemsCons;
                                items.Add(itemsEsib);

                            }
                        }
                    }


                    //se l'istanza è con certificazione inserisco il file di chiusura
                    if (infoEsib.isRichiestaCertificazione)
                    {
                        isCertificata = true;
                        bool esitoSalvaCert = this.saveFileCertificazione(esibizionePath, infoEsib.idProfileCertificazione, infoEsib.SystemID, infoUtenteEsib, dryRun);
                        if (!esitoSalvaCert)
                        {
                            result.esito = false;
                            result.messaggio = result.messaggio + " errore nel salvataggio del documento di certificazione! \n ";
                        }
                    }                    

                    //creo il pdf con l'elenco dei documenti con i relativi hyperlink
                    if (!dryRun)
                    {
                        appoItems = new DocsPaVO.areaConservazione.ItemsEsibizione[items.Count];
                        items.CopyTo(appoItems);
                        if (appoItems.Length > 0)
                        {
                            logger.Debug("indice");

                            FileDocumento indexEsib = new BusinessLogic.Conservazione.EsibizioneManager().CreaIndiceDoc(infoEsib, appoItems);
                            FileStream file = null;



                            string filename = replaceInvalidCharFile(indexEsib.name);
                            filename = indexPath + '\u005C'.ToString() + filename;

                            file = File.Create(filename);
                            file.Write(indexEsib.content, 0, indexEsib.length);

                            if (file != null)
                            {
                                file.Flush();
                                file.Close();
                            }
                            else
                            {
                                result.esito = false;
                                result.messaggio += " Errore nella generazione del file di indice! \n";
                            }

                        }
                    }

                    //invio l'istanza allo storage remoto
                    this.createZipFile(idEsibizione);
                    if (!this.SubmitToRemoteFolder(idEsibizione))
                    {
                        result.esito = false;
                        result.messaggio += " Errone nell'invio dei file sullo storage remoto! \n";
                    }
                }


                if (result.esito)
                {
                    //aggiorno lo stato istanza in "CHIUSA"
                    if (!consManager.ChiudiIstanzaEsibizione(infoUtenteEsib, idEsibizione, isCertificata))
                    {
                        result.esito = false;
                        result.messaggio += " errore nell'aggiornamento della tabella DPA_AREA_ESIBIZIONE! \n";
                    }
                }
            }


            //nuova gestione dei messaggi di errore **********************************
            DocsPaVO.Logger.CodAzione.Esito logResponse = DocsPaVO.Logger.CodAzione.Esito.KO;
            if (result.esito)
            {
                result.messaggio = "Istanza di esibizione creata con successo!";
                logResponse = DocsPaVO.Logger.CodAzione.Esito.OK;
            }
            else
            {
                logger.Debug(result.messaggio);
            }

            return result;
        }

        /// <summary>
        /// Questo metodo si occupa di salvare nella folder di conservazione il documento passato come
        /// parametro con tutti i relativi allegati
        /// </summary>
        /// <param name="infoEsib"></param>
        /// <param name="itemsEsib"></param>
        /// <param name="infoUtente"></param>
        /// <param name="pathCons"></param>
        /// <param name="pathFasc"></param>
        /// <param name="fileXml"></param>
        /// <param name="usfi"></param>
        /// <param name="dryRun"></param>
        /// <returns></returns>
        protected int putDocumenti_InEsibizione(DocsPaVO.areaConservazione.InfoEsibizione infoEsib, ref DocsPaVO.areaConservazione.ItemsEsibizione itemsEsib, DocsPaVO.utente.InfoUtente infoUtente, string pathCons, string pathFasc, bool dryRun, bool localStore)
        {
            int result = 1;
            string err = string.Empty;
            DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();

            Dictionary<String, String> docsIstanza = getFilesFromUniSincro(itemsEsib.ID_Conservazione, localStore);
            string info = docsIstanza[itemsEsib.ID_Profile];

            //costruisco la scheda documento (mi serve per recuperare i timestamp)
            //DocsPaVO.documento.SchedaDocumento sch = new DocsPaVO.documento.SchedaDocumento();
            //sch = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, itemsEsib.ID_Profile);

            try
            {

                //documento principale
                string infoDocumento = docsIstanza[itemsEsib.ID_Profile];

                string pathFile = infoDocumento.Split('§')[2];
                string docNumber = infoDocumento.Split('§')[1];
                string impronta = infoDocumento.Split('§')[3];

                byte[] contentFile = getFileFromStore(itemsEsib.ID_Conservazione, pathFile, localStore);

                //reperisco il file coi metadati
                string pathXml = pathFile + ".xml";
                byte[] contentXml = getFileFromStore(itemsEsib.ID_Conservazione, pathXml, localStore);

                //salvataggio nella relativa cartella di conservazione del file
                if (!saveFileEsib(contentFile, contentXml, ref itemsEsib, itemsEsib.ID_Profile, null, pathCons, pathFasc, pathFile, true, dryRun))
                    throw new Exception("Errore nella scrittura del file principale");

                //timestamp
                ArrayList timeStampArray = this.getNomeTSDoc(itemsEsib.ID_Profile, contentXml, impronta);
                if (timeStampArray.Count > 0)
                {
                    //se ci sono timestamp associati li recupero e li salvo nella cartella del documento
                    for (int i = 0; i < timeStampArray.Count; i++)
                    {
                        string tsName = timeStampArray[i].ToString();
                        string pathTS = Path.GetDirectoryName(pathFile) + '\u005C'.ToString() + tsName;
                        if (!saveFileTSEsib(tsName, pathTS, ref itemsEsib, docNumber, pathCons, pathFasc, dryRun, localStore))
                            throw new Exception("Errore nella scrittura del timestamp n. " + i);
                    }
                }


                //ciclo su allegati
                foreach (KeyValuePair<String, String> infoAll in docsIstanza)
                {
                    string path = infoAll.Value.Split('§')[2];
                    //se è allegato contiene nel path il docnumber ma il suo iddoc è diverso da docnumber
                    if (!(infoAll.Key == docNumber) && ((infoAll.Value.Split('§')[2]).Contains(docNumber)))
                    {
                        string pathAll = infoAll.Value.Split('§')[2];
                        string idDocAll = infoAll.Value.Split('§')[1];
                        string improntaAll = infoAll.Value.Split('§')[3];

                        byte[] contentAll = getFileFromStore(itemsEsib.ID_Conservazione, pathAll, localStore);

                        //salvataggio nella cartella 'Allegati'
                        if (!saveFileEsib(contentAll, null, ref itemsEsib, docNumber, idDocAll, pathCons, pathFasc, pathAll, false, dryRun))
                            throw new Exception("Errore nella scrittura dell'allegato ID=" + idDocAll);

                        //timestamp
                        ArrayList timeStampAll = this.getNomeTSAll(idDocAll, pathAll, contentXml, improntaAll);
                        if (timeStampAll.Count > 0)
                        {
                            for (int i = 0; i < timeStampAll.Count; i++)
                            {
                                string tsName = timeStampAll[i].ToString();
                                string pathTS = Path.GetDirectoryName(pathAll) + '\u005C'.ToString() + tsName;
                                if (!saveFileTSAllEsib(tsName, pathTS, ref itemsEsib, docNumber, pathCons, pathFasc, dryRun, localStore))
                                    throw new Exception(string.Format("Errore nella scrittura del timestamp {0} dell'allegato {1}", i, idDocAll));
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                result = 0;
            }
            return result;
        }

        #region OLD CODE
        /// <summary>
        /// Questo metodo salva il file fisico associato all'oggetto fileDoc nell'apposita cartella ricavando
        /// l'informazione del percorso a partire dall'oggetto InfoEsibizione e DocNumber
        /// </summary>
        /// <param name="fileDoc"></param>
        /// <param name="infoCons"></param>
        /// <param name="itemsCons"></param>
        /// <param name="schDoc"></param>
        /// <param name="isDoc">se �è true salva XML metadati non si usa per gli allegati</param>
        /// <param name="root_path"></param>
        /// <param name="objFileRequest"></param>
        /// <returns></returns>
        protected bool saveFile(DocsPaVO.documento.FileDocumento fileDoc, DocsPaVO.areaConservazione.InfoEsibizione infoEsib, ref DocsPaVO.areaConservazione.ItemsEsibizione itemsEsib, DocsPaVO.documento.SchedaDocumento schDoc, bool isDoc, string root_path, DocsPaVO.documento.FileRequest objFileRequest, string path_fasc, bool dryRun)
        {
            bool result = false;
            string err = string.Empty;
            string fullName = string.Empty;
            //root_path = Path.Combine(root_path, BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(infoEsib.IdAmm).Codice);
            //root_path = Path.Combine(root_path, infoEsib.SystemID);
            //root_path = Path.Combine(root_path, itemsEsib.ID_Conservazione);

            //root_path = Path.Combine(root_path, infoCons.SystemID);

            //nuova struttura directory!!!!!!!!!!!
            //string rootXml = Path.Combine(root_path, "Chiusura");
            //string rootXml = root_path;
            string pathDoc = schDoc.docNumber;

            if (!string.IsNullOrEmpty(path_fasc))
            {
                root_path = Path.Combine(root_path, path_fasc);
                pathDoc = Path.Combine(path_fasc, Path.Combine("Documenti", schDoc.docNumber));
                itemsEsib.relative_path = Path.Combine(itemsEsib.relative_path, Path.Combine("Documenti", schDoc.docNumber));
            }
            else
            {
                pathDoc = Path.Combine("Documenti", schDoc.docNumber);
                itemsEsib.relative_path = Path.Combine(itemsEsib.relative_path, Path.Combine("Documenti", schDoc.docNumber));
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
                CreateDir(path_cons, dryRun);
            }

            //nuova struttura directory!!!!!!!!!!!
            //normalizzo il percorso eliminando i caratteri speciali
            /*
            rootXml = replaceInvalidChar(rootXml);

            if (!Directory.Exists(rootXml))
            {
                CreateDir(rootXml, dryRun);
            }
            */
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
            itemsEsib.relative_path = itemsEsib.relative_path + '\u005C'.ToString() + fileName;

            //aggiungo il path relativo del file per il link Html
            if (isDoc)
            {
                //itemsEsib.pathCD = path_supporto;
                //aggiunta di ulteriori campi all'oggetto itemsCons
                //addFieldsToItemsCons(schDoc, ref itemsCons);
            }
            else
            {
                //aggiungo il path relativo per tutti gli allegati del documento corrente
                //itemsCons.path_allegati.Add(path_supporto);
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
                //string root_cons = Path.Combine(rootXml, XmlName + infoCons.SystemID + ".xml");
                //Creazione del file XML generale da firmare

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
                        Metadata.XmlDocEsibizione xmlDoc = new Metadata.XmlDocEsibizione(infoEsib, fileDoc, schDoc, objFileRequest);
                        bool esito = new InfoDocXml().saveMetadatiString(fullName, xmlDoc.XmlFile);
                    }
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
        #endregion

        protected bool saveFileEsib(byte[] content, byte[] contentXml, ref DocsPaVO.areaConservazione.ItemsEsibizione itemsEsib, string docNumber, string idAllegato , string path_istanza, string path_fasc, string filename, bool isDoc, bool dryRun)
        {

            bool result = false;
            string err = string.Empty;
            
            string root_path = path_istanza;
            //path assoluto dove salvare il documento
            string path_cons = string.Empty;
            //path relativo del documento all'interno della directory dell'istanza di esibizione (serve per l'indice)
            string pathDoc = itemsEsib.ID_Conservazione;
            //nome file comprensivo del path
            string fullName = string.Empty;

            //calcolo l'alberatura delle directory
            if (!string.IsNullOrEmpty(path_fasc))
            {
                root_path = Path.Combine(root_path, path_fasc);
                pathDoc = Path.Combine(pathDoc, path_fasc);
                pathDoc = Path.Combine(pathDoc, Path.Combine("Documenti", docNumber));
                //itemsEsib.relative_path = Path.Combine(itemsEsib.relative_path, Path.Combine("Documenti", docNumber));
            }
            else
            {
                pathDoc = Path.Combine(pathDoc, Path.Combine("Documenti", docNumber));
                //itemsEsib.relative_path = Path.Combine(itemsEsib.relative_path, Path.Combine("Documenti", docNumber));
            }

            //imposto il path assoluto
            path_cons = Path.Combine(root_path, Path.Combine("Documenti", docNumber));

            //se è un allegato devo includere anche la sottocartella per gli allegati
            bool isAll = !isDoc;
            if (isAll)
            {
                path_cons = Path.Combine(path_cons, "Allegati");
            }

            //normalizzo il percorso eliminando i caratteri speciali
            path_cons = replaceInvalidChar(path_cons);

            //se non esiste la directory la creo
            if (!Directory.Exists(path_cons))
            {
                CreateDir(path_cons, dryRun);
            }

            //modifica: il path e il filename li prendo dal file di chiusura
            //fullName = path_istanza + filename;

            //estraggo il nomefile dal path nel file di chiusura
            string nomefile = Path.GetFileName(filename);
            nomefile = replaceInvalidCharFile(nomefile);

            //costruisco il nome del file comprensivo di percosrso
            fullName = path_cons + '\u005C'.ToString() + nomefile;

            #region OLD CODE
            //fullName = path_cons + '\u005C'.ToString() + fileName;
            //itemsEsib.relative_path = itemsEsib.relative_path + '\u005C'.ToString() + fileName;
            #endregion 

            //se è il documento principale ho bisogno del path per l'indice dei documenti
            if (isDoc)
            {
                //itemsEsib.relative_path = replaceInvalidChar(itemsEsib.ID_Conservazione) + filename;
                itemsEsib.relative_path = pathDoc + '\u005C'.ToString() + nomefile;
            }
            //se è un allegato inserisco il path relativo nella stringa
            else
            {
                if (idAllegato != null)
                {
                    string relpath = Path.Combine(pathDoc, "Allegati") + '\u005C'.ToString() + nomefile;
                    itemsEsib.path_allegati.Add(idAllegato + "§" + relpath);
                }
            }

            FileStream file = null;

            try
            {
                if (!dryRun)
                {
                    file = File.Create(fullName);
                    file.Write(content, 0, content.Length);
                    result = true;
                }
                else
                {
                    result = true;
                }
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

            //Se è il documento principale scrivo anche il file XML coi metadati
            if (isDoc)
            {
                string fileNameXml = fullName + ".xml";
                FileStream fileXml = null;

                try
                {
                    if (!dryRun)
                    {
                        fileXml = File.Create(fileNameXml);
                        fileXml.Write(contentXml, 0, contentXml.Length);
                        result = true;
                    }
                    else
                    {
                        result = true;
                    }
                }
                catch (Exception eXml)
                {
                    err = "Errore nella scrittura del file XML dei metadati." + eXml.Message;
                    logger.Debug(err);
                    result = false;
                }
                finally
                {
                    if (fileXml != null)
                    {
                        fileXml.Flush();
                        fileXml.Close();
                    }

                }
            }

            return result;
        }

        protected bool saveFileTSEsib(string tsname, string tspath, ref DocsPaVO.areaConservazione.ItemsEsibizione itemsEsib, string docNumber, string path_istanza, string path_fasc, bool dryRun, bool localStore)
        {

            bool result = false;
            string err = string.Empty;

            string root_path = path_istanza;
            //path assoluto dove salvare il documento
            string path_cons = string.Empty;
            //path relativo del documento all'interno della directory dell'istanza di esibizione (serve per l'indice)
            //string pathDoc = docNumber;
            //nome file comprensivo del path
            string fullName = string.Empty;

            //calcolo l'alberatura delle directory
            if (!string.IsNullOrEmpty(path_fasc))
            {
                root_path = Path.Combine(root_path, path_fasc);
                //pathDoc = Path.Combine(path_fasc, Path.Combine("Documenti", docNumber));
                //itemsEsib.relative_path = Path.Combine(itemsEsib.relative_path, Path.Combine("Documenti", docNumber));
            }
            else
            {
                //pathDoc = Path.Combine("Documenti", docNumber);
                //itemsEsib.relative_path = Path.Combine(itemsEsib.relative_path, Path.Combine("Documenti", docNumber));
            }

            //imposto il path assoluto
            path_cons = Path.Combine(root_path, Path.Combine("Documenti", docNumber));

            //normalizzo il percorso eliminando i caratteri speciali
            path_cons = replaceInvalidChar(path_cons);

            //costruisco il nome del timestamp
            fullName = path_cons + '\u005C'.ToString() + tsname;
            
            //recupero il timestamp
            byte[] content = getFileFromStore(itemsEsib.ID_Conservazione, tspath, localStore);
            if (content == null)
                return true;

            FileStream file = null;

            try
            {
                if (!dryRun)
                {
                    file = File.Create(fullName);
                    file.Write(content, 0, content.Length);
                    result = true;
                }
                else
                {
                    result = true;
                }
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

            return result;
        }

        protected bool saveFileTSAllEsib(string tsname, string tspath, ref DocsPaVO.areaConservazione.ItemsEsibizione itemsEsib, string docNumber, string path_istanza, string path_fasc, bool dryRun, bool localStore)
        {
            bool result = false;
            string err = string.Empty;

            string root_path = path_istanza;
            //path assoluto dove salvare il documento
            string path_cons = string.Empty;
            //nome file comprensivo del path
            string fullName = string.Empty;

            //calcolo l'alberatura delle directory
            if (!string.IsNullOrEmpty(path_fasc))
            {
                root_path = Path.Combine(root_path, path_fasc);
                //pathDoc = Path.Combine(path_fasc, Path.Combine("Documenti", docNumber));
                //itemsEsib.relative_path = Path.Combine(itemsEsib.relative_path, Path.Combine("Documenti", docNumber));
            }
            else
            {
                //pathDoc = Path.Combine("Documenti", docNumber);
                //itemsEsib.relative_path = Path.Combine(itemsEsib.relative_path, Path.Combine("Documenti", docNumber));
            }

            //imposto il path assoluto
            path_cons = Path.Combine(root_path, Path.Combine("Documenti", docNumber));
            //aggiungo al path la cartella Allegati
            path_cons = Path.Combine(path_cons, "Allegati");

            //normalizzo il percorso eliminando i caratteri speciali
            path_cons = replaceInvalidChar(path_cons);


            //costruisco il nome del timestamp
            fullName = path_cons + '\u005C'.ToString() + tsname;

            //recupero il timestamp
            byte[] content = getFileFromStore(itemsEsib.ID_Conservazione, tspath, localStore);
            if (content == null)
                return true;

            FileStream file = null;

            try
            {
                if (!dryRun)
                {
                    file = File.Create(fullName);
                    file.Write(content, 0, content.Length);
                    result = true;
                }
                else
                {
                    result = true;
                }
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

            return result;
        }

        /// <summary>
        /// Metodo per il salvataggio del file di certificazione
        /// </summary>
        /// <param name="path"></param>
        /// <param name="idCertificazione"></param>
        /// <param name="infoUtente"></param>
        /// <param name="dryRun"></param>
        /// <returns></returns>
        protected bool saveFileCertificazione(string path, string idCertificazione, string idEsibizione, InfoUtente infoUtente, bool dryRun)
        {
            bool result = false;
            
            DocsPaVO.documento.FileDocumento fileCert = null;
            DocsPaVO.documento.SchedaDocumento documento = BusinessLogic.Documenti.DocManager.getDettaglioNoSecurity(infoUtente, idCertificazione);

            DocsPaVO.documento.FileRequest fileRequest = (DocsPaVO.documento.FileRequest)documento.documenti[0];

            fileCert = BusinessLogic.Documenti.FileManager.getFileFirmato(fileRequest, infoUtente, false);
            //fileCert = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente);

            string fileName = replaceInvalidCharFile(fileCert.name);
            
            //string fileName = replaceInvalidCharFile(string.Format("Istanza_Esibizione_{0}_Certificazione.pdf.p7m", idEsibizione));
            //string fullName = path + '\u005C'.ToString() + fileName;
            string fullName = path + '\u005C'.ToString() + "Certificazione.pdf.p7m";

            FileStream fs = null;

            try
            {

                if (!dryRun)
                {
                    fs = File.Create(fullName);
                    fs.Write(fileCert.content, 0, fileCert.length);
                    result = true;
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception exc)
            {
                string err = "Errore nella gestione del salvataggio del File " + exc.Message;
                logger.Debug(err);
                result = false;
                throw new Exception(exc.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Flush();
                    fs.Close();
                }
            }

            return result;

        }

        public bool saveMarcaCertificazione(string rootpath, string idEsibizione, string marcaBase64, string idAmm)
        {
            logger.Debug("Inizio - saveMarcaCertificazione");
            bool retVal = false;
            FileStream fs = null;

            rootpath = Path.Combine(rootpath, BusinessLogic.Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(idAmm).Codice);

            string path = Path.Combine(rootpath, idEsibizione);
            logger.Debug("Path: " + path);
            path = replaceInvalidChar(path);

            //se non esiste la directory la creo
            if (!Directory.Exists(path))
            {
                CreateDir(path, false);
            }

            //string nomeFile = path + '\u005C'.ToString() + "file_chiusura.TSR";
            string nomeFile = path + '\u005C'.ToString() + "Certificazione.TSR";
            logger.Debug("NomeFile: " + nomeFile);

            if (!string.IsNullOrEmpty(marcaBase64))
            {
                try
                {
                    byte[] base64 = Convert.FromBase64String(marcaBase64);
                    fs = File.Create(nomeFile);
                    fs.Write(base64, 0, base64.Length);
                    retVal = true;
                }
                catch (Exception ex)
                {
                    retVal = false;
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Flush();
                        fs.Close();
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per il salvataggio dei file di chiusura delle istanze di conservazione
        /// </summary>
        /// <param name="idCons">id instanza di conservazione</param>
        /// <param name="path">path comprensivo di id istanza cons</param>
        /// <param name="isFirmato">se true salva il file firmato</param>
        /// <param name="localStore"></param>
        /// <param name="dryRun"></param>
        /// <returns></returns>
        protected bool saveFileChiusura(string idCons, string path, bool isFirmato, bool localStore, bool dryRun)
        {
            bool retVal = false;

            string pathFileChiusura = string.Empty;
            string fileName = string.Empty;

            if (!isFirmato)
            {
                pathFileChiusura = "\\Chiusura\\file_chiusura.xml";
                fileName = path + '\u005C'.ToString() + "file_chiusura.xml";
            }
            else
            {
                pathFileChiusura = "\\Chiusura\\file_chiusura.XML.P7M";
                fileName = path + '\u005C'.ToString() + "file_chiusura.XML.P7M";
            }

            //recupero il file di chiusura
            byte[] content = getFileFromStore(idCons, pathFileChiusura, localStore);

            FileStream file = null;
            try
            {
                if (!dryRun)
                {
                    file = File.Create(fileName);
                    file.Write(content, 0, content.Length);
                    retVal = true;
                }
                else
                {
                    retVal = true;
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nella scrittura del file di chiusura - " + ex.Message);
                retVal = false;
            }
            finally
            {
                file.Flush();
                file.Close();
            }

            return retVal;
        }

        protected bool saveRapportoVersamento(string idCons, string istancePath)
        {
            bool result = false;
            DocsPaConsManager manager = new DocsPaConsManager();
            string path = Path.Combine(istancePath, "rapporto_versamento.xml");
            logger.Debug("PATH: " + path);

            // Lettura rapporto da db
            string rapportoVersamento = manager.GetIstanzaBinaryField(idCons, "RAPPORTO_VERSAMENTO");

            if (!string.IsNullOrEmpty(rapportoVersamento))
            {
                FileStream file = null;
                
                try
                {
                    byte[] content = System.Text.Encoding.UTF8.GetBytes(rapportoVersamento);
                    file = File.Create(path);
                    file.Write(content, 0, content.Length);
                    result = true;
                }
                catch (Exception ex)
                {
                    logger.DebugFormat("{0} \r\n{1}", ex.Message,ex.StackTrace);
                    result = false;
                }
                finally
                {
                    file.Flush();
                    file.Close();
                }
            }

            return result;
        }

        #region utils

        /// <summary>
        /// Metodo per l'analisi del file chiusura.xml, nel quale sono registrati gli id dei file, il loro content type, il path e l'hash.
        /// </summary>
        /// <param name="infoutente">l'utente loggato</param>
        /// <param name="idConservazione">id dell'istanza sulla quale si sta effettuando il test di leggibilità</param>
        /// <returns>oggetto Dictionary con i dati dei file, utilizzando come chiave l'id del documento</returns>
        public static Dictionary<String, String> getFilesFromUniSincro(string idConservazione, bool localStore)
        {
            Stream fileChiusura;
            XmlDocument xmlFile = new XmlDocument();
            fileChiusura = new MemoryStream(new FileManager().getFileFromStore(idConservazione, "\\Chiusura\\file_chiusura.xml", localStore));
            fileChiusura.Seek(0, SeekOrigin.Begin);
            Dictionary<String, String> retval = new Dictionary<string, string>();
            xmlFile.Load(fileChiusura);

            XmlNodeList nodes = xmlFile.GetElementsByTagName("sincro:File");
            foreach (XmlNode n in nodes)
            {
                string formato = n.Attributes[0].InnerText;
                string idDocumento = ((XmlNode)n.ChildNodes[0]).InnerText;
                string pathFile = ((XmlNode)n.ChildNodes[1]).InnerText;
                string hashSupporto = ((XmlNode)n.ChildNodes[2]).InnerText;
                try
                {
                    retval.Add(idDocumento, string.Format("{0}§{1}§{2}§{3}", formato, idDocumento, pathFile, hashSupporto));
                }
                catch (Exception ex)
                {
                    // probabile id ripetuta. non fare nulla. 
                    // Il documento è presente in più fascicoli inseriti in conservazione. L'hash è lo stesso.
                }
            }
            return retval;
        }

        /// <summary>
        /// Costruisce i nomi dei timestamp associati ad un documento
        /// </summary>
        /// <param name="docNumber">id profile del documento</param>
        /// <param name="contentXml">metadati del documento</param>
        /// <param name="impronta">impronta del documento</param>
        /// <returns>elenco nomi ts</returns>
        private ArrayList getNomeTSDoc(string docNumber, byte[] contentXml, string impronta)
        {

            ArrayList retVal = new ArrayList();
            ArrayList numSerie = new ArrayList();
            

            XmlDocument infoXml = new XmlDocument();
            MemoryStream ms = new MemoryStream(contentXml);
            infoXml.Load(ms);

            try
            {
                //seleziono tutti i file riportati nell'xml
                XmlNodeList nodes = infoXml.GetElementsByTagName("File");
                foreach (XmlNode n in nodes)
                {
                    logger.Debug("nodo - impronta: " + n.Attributes["Impronta"].InnerText);
                    //seleziono il documento a partire dall'impronta
                    if (n.Attributes["Impronta"].InnerText == impronta)
                    {
                        //cerco tutti i ts associati al file
                        foreach (XmlNode tsnode in n.ChildNodes)
                        {
                            if (tsnode.Name == "MarcaTemporale")
                            {
                                string nserie = tsnode.Attributes["NumeroSerie"].InnerText;
                                numSerie.Add(nserie);
                            }

                        }
                    }
                }

                foreach (string nserie in numSerie)
                {
                    string nome = "TS" + docNumber + "-" + nserie + ".tsr";
                    retVal.Add(nome);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                //retVal = string.Empty;
            }

            return retVal;
        }

        /// <summary>
        /// Costruisce i nomi dei timestamp associati ad un allegato
        /// </summary>
        /// <param name="idAll">id profile dell'allegato</param>
        /// <param name="pathAll">path dell'allegato</param>
        /// <param name="contentXml">metadati del documento cui l'allegato appartiene</param>
        /// <param name="impronta">impronta dell'allegato</param>
        /// <returns>elenco nomi ts</returns>
        private ArrayList getNomeTSAll(string idAll, string pathAll, byte[] contentXml, string impronta)
        {

            ArrayList retVal = new ArrayList();
            ArrayList numSerie = new ArrayList();

            XmlDocument infoXml = new XmlDocument();
            MemoryStream ms = new MemoryStream(contentXml);
            infoXml.Load(ms);

            try
            {
                string nameAll = Path.GetFileNameWithoutExtension(pathAll);
                XmlNodeList nodes = infoXml.GetElementsByTagName("File");
                foreach (XmlNode n in nodes)
                {
                    if (n.Attributes["Impronta"].InnerText == impronta)
                    {
                        foreach (XmlNode tsnode in n.ChildNodes)
                        {
                            if (tsnode.Name == "MarcaTemporale")
                            {
                                string nserie = tsnode.Attributes["NumeroSerie"].InnerText;
                                numSerie.Add(nserie);
                            }
                        }
                    }

                }

                foreach (string nserie in numSerie)
                {
                    string nome = "TSAll-" + nameAll + "-" + idAll + "-" + nserie + ".tsr";
                    retVal.Add(nome);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return retVal;
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

        private byte[] getFileFromStore(string idCons, string path, bool localStore)
        {
            FileManager manager = new FileManager();
            return manager.getFileFromStore(idCons, path, localStore);

        }

        private void CreateDir(string path, bool dryRun)
        {
            if (!dryRun)
                Directory.CreateDirectory(path);

        }

        public void createZipFile(string idCons)
        {
            string err = string.Empty;
            createZip = ZipFolder(idCons);
        }

        private bool ZipFolder(string idIstanza)
        {
            bool result = false;
            bool remoteStorage = false;
            //modifica Lembo 16-11-2012
            //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["CONSERVAZIONE_REMOTE_STORAGE_URL"])||!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSERVAZIONE_REMOTE_STORAGE_URL")))
            //    remoteStorage = true;
            if (!string.IsNullOrEmpty(DocsPaConsManager.getEsibizioneRemoteStorageUrl()))
            {
                if (!string.IsNullOrEmpty(DocsPaConsManager.getConservazioneRemoteStorageUrl()))
                    remoteStorage = true;
            }

            DocsPaConsManager consManager = new DocsPaConsManager();

            FastZip fastZip = new FastZip();
            bool recurse = true;
            string zipFile = EsibizionePathManager.GetPathFileZip(idIstanza);

            try
            {
                if (!remoteStorage)
                {
                    fastZip.CreateZip(zipFile, EsibizionePathManager.GetRootPathIstanza(idIstanza), recurse, "");
                }

                result = true;
            }
            catch (Exception exZip)
            {
                string err = "Errore nella creazione del file Zip dell'istanza di conservazione. " + exZip.Message;
                logger.Debug(err);
                result = false;
            }

            return result;
        }

        public bool SubmitToRemoteFolder(string idIstanza)
        {
            bool result = false;

            string dctmCServerAddressRoot = DocsPaConsManager.getEsibizioneRemoteStorageUrl();
            if(string.IsNullOrEmpty(dctmCServerAddressRoot))
                dctmCServerAddressRoot = DocsPaConsManager.getConservazioneRemoteStorageUrl();

            if (string.IsNullOrEmpty(dctmCServerAddressRoot))
            {
                logger.Info("Storage remoto non configurato");
                return true;
            }

            DocsPaConsManager consManager = new DocsPaConsManager();

            try
            {
                string uploadDir = dctmCServerAddressRoot + "/" + EsibizionePathManager.GetCodiceAmministrazione(idIstanza) + "/";
                httpSubmit hs = new httpSubmit(EsibizionePathManager.GetRootPathIstanza(idIstanza), uploadDir);
                if (hs.Success)
                {
                    //cancella istanza locale
                    Directory.Delete(EsibizionePathManager.GetRootPathIstanza(idIstanza), true);
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

            return result;
        }

        #endregion
    }
}
