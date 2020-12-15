using System;
using System.Xml;
using System.Collections;
using System.Data;
using log4net;

namespace BusinessLogic.AmministrazioneXml
{
    /// <summary>
    /// Classe per la gestione del titolario di DocsPA tramite XML
    /// </summary>
    public class TitolarioXml
    {
        private ILog logger = LogManager.GetLogger(typeof(TitolarioXml));
        private ErrorCode errorCode;
        private XmlDocument parser;
        private bool soloNodiT;

        private DocsPaVO.utente.InfoUtente infoUtente;

        public TitolarioXml()
        {
            errorCode = ErrorCode.NoError;
        }

        /// <summary>
        /// Carica un file XML
        /// </summary>
        /// <param name="xmlSource"></param>
        public TitolarioXml(string xmlPath, out bool xmlLoadResult)
        {
            errorCode = ErrorCode.NoError;
            xmlLoadResult = true;

            try
            {
                // Validazione file XML
                parser = new XmlDocument();
                parser.Load(xmlPath);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la validazione dell'XML", exception);
                errorCode = ErrorCode.BadXmlFile;
                xmlLoadResult = false;
            }
        }

        /// <summary>
        /// Acquisisce uno stream XML
        /// </summary>
        /// <param name="xmlSource"></param>
        public TitolarioXml(string xmlDoc)
        {
            errorCode = ErrorCode.NoError;

            try
            {
                // Validazione file XML
                parser = new XmlDocument();
                parser.LoadXml(xmlDoc);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la validazione dell'XML", exception);
                errorCode = ErrorCode.BadXmlFile;
            }
        }

        /// <summary>Ritorna l'ultimo codice di errore</summary>
        /// <returns></returns>
        public ErrorCode GetErrorCode()
        {
            return errorCode;
        }

        /// <summary>
        /// Cancella la struttura dei documenti esistenti
        /// </summary>
        /// <returns></returns>
        public bool DropAll()
        {
            bool result = true;

            try
            {
                if (errorCode == ErrorCode.NoError)
                {
                    DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();

                    if (!amministrazioneXml.ClearTitolario())
                    {
                        throw new Exception();
                    }

                    amministrazioneXml.Dispose();
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la cancellazione del titolario", exception);
                errorCode = ErrorCode.GenericError;
            }

            if (errorCode != ErrorCode.NoError)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Caricamento dati per login
        /// </summary>
        /// <returns></returns>
        private bool Login()
        {
            bool result = true; // Presume successo

            try
            {
                UserData userData = new UserData(true);
                GetUserData(ref userData, parser.DocumentElement.SelectSingleNode("TITOLARIO/DATI"));

                DocsPaVO.utente.UserLogin userLogin = new DocsPaVO.utente.UserLogin(userData.utente, userData.password, userData.idAmm, "");

                DocsPaVO.utente.Utente utente;
                DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();
                DocsPaVO.utente.UserLogin.LoginResult loginResult;
                userManager.LoginUser(userLogin, out utente, out loginResult);

                if (utente == null)
                {
                    return false;
                }

                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                this.infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la login.", exception);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Ritorna il testo contenuto in un dato tag XML
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private string GetXmlField(string fieldName, XmlNode node, string defaultValue)
        {
            string result = defaultValue;

            XmlNode child = node.SelectSingleNode(fieldName);

            if (child != null)
            {
                if (child.InnerText != "")
                {
                    result = child.InnerText;
                }
            }

            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="userData"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool GetUserData(ref UserData userData, XmlNode node)
        {
            bool result = true; // Presume successo

            try
            {
                userData.utente = GetXmlField("UTENTE", node, userData.utente);
                userData.password = GetXmlField("PASSWORD", node, userData.password);
                userData.ruolo = GetXmlField("RUOLO", node, userData.ruolo);
                userData.registro = GetXmlField("REGISTRO", node, userData.registro);
                userData.descrizione = GetXmlField("DESCRIZIONE", node, userData.descrizione);
                userData.codice = GetXmlField("CODICE", node, userData.codice);

                if (userData.descrizione != null)
                {
                    userData.descrizione = DocsPaUtils.Functions.Functions.ReplaceApexes(userData.descrizione);
                }
                if (userData.codice != null)
                {
                    userData.codice = userData.codice.ToUpper();
                }

                // Trova id amministrazione
                string adminId = GetXmlField("IDAMM", node, "");

                if (adminId != null && adminId != "")
                {
                    DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                    userData.idAmm = amministrazioneXml.GetAdminByName(adminId);
                }

                // Trova registro
                if (userData.registro != null && userData.registro != "")
                {
                    DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                    userData.idRegistro = amministrazioneXml.GetRegByName(userData.registro);
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la lettura dei dati utente.", exception);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Estrae un utente che abbia un relativo gruppo per
        /// utilizzarlo per la creazione dei tag DATI nell'XML.
        /// I tag da avere per un corretto caricamento del 
        /// titolario sono:
        ///		IDAMM (gia' presente perche' creato dall'export)
        ///		UTENTE
        ///		PASSWORD
        ///		RUOLO
        /// </summary>
        /// <param name="dataNode"></param>
        /// <returns></returns>
        private bool UpdateAccessData(XmlNode dataNode)
        {
            bool result = true;

            try
            {
                // Trova utente e ruolo
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();

                string utente;
                string password;
                string ruolo;

                amministrazioneXml.GetTitolarioAccessData(dataNode.SelectSingleNode("IDAMM").InnerText,
                                                          out utente,
                                                          out password,
                                                          out ruolo);

                if (dataNode.SelectSingleNode("UTENTE") == null) dataNode.AppendChild(dataNode.OwnerDocument.CreateElement("UTENTE"));
                if (dataNode.SelectSingleNode("PASSWORD") == null) dataNode.AppendChild(dataNode.OwnerDocument.CreateElement("PASSWORD"));
                if (dataNode.SelectSingleNode("RUOLO") == null) dataNode.AppendChild(dataNode.OwnerDocument.CreateElement("RUOLO"));

                dataNode.SelectSingleNode("UTENTE").InnerText = utente;
                dataNode.SelectSingleNode("PASSWORD").InnerText = password;
                dataNode.SelectSingleNode("RUOLO").InnerText = ruolo;
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la lettura dei dati per l'accesso in scrittura al titolario.", exception);
                result = false;
            }

            return result;
        }

        ///// <summary>
        ///// Crea la struttura documentale a partire dall'XML passato al costruttore
        ///// </summary>
        ///// <returns></returns>
        //public bool CreateStructure()
        //{
        //    bool result = true;
        //    errorCode = ErrorCode.NoError;

        //    try
        //    {
        //        if (errorCode == ErrorCode.NoError)
        //        {
        //            XmlAttribute attribute = parser.DocumentElement.SelectSingleNode("TITOLARIO").Attributes["dropAll"];

        //            if (attribute != null)
        //            {
        //                bool dropAll = Boolean.Parse(attribute.InnerText);

        //                if (dropAll)
        //                {
        //                    this.DropAll();
        //                }
        //            }

        //            UserData userData = new UserData(false);

        //            foreach (XmlNode node in parser.DocumentElement.SelectNodes("TITOLARIO"))
        //            {
        //                // Estrarre un utente che abbia un relativo gruppo ed utilizzarlo per la creazione dei tag DATI nell'XML.						
        //                if (!this.UpdateAccessData(node.SelectSingleNode("DATI"))) throw new Exception();

        //                // Login
        //                if (!Login()) throw new Exception();

        //                // Leggi totale nodi titolario
        //                int totalItems = 0;

        //                if (node.Attributes["totalItems"] != null)
        //                {
        //                    totalItems = Int32.Parse(node.Attributes["totalItems"].InnerText);
        //                }
        //                else
        //                {
        //                    logger.Debug("Impossibile leggere il totale dei nodi titolario");
        //                }

        //                DocsPaUtils.LogsManagement.ImportExportLogger.ResetLog("import", totalItems);
        //                NavigateTree(node, 0, userData, "0", "", totalItems, "", 0);
        //            }
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        logger.Debug("Errore durante la creazione dell'XML", exception);
        //        errorCode = ErrorCode.GenericError;
        //    }

        //    if (errorCode != ErrorCode.NoError)
        //    {
        //        result = false;
        //    }

        //    DocsPaUtils.LogsManagement.ImportExportLogger.ResetLog("import", 0);

        //    return result;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public bool UpdateStructure()
        //{
        //    bool result = true;
        //    errorCode = ErrorCode.NoError;

        //    DocsPaUtils.LogsManagement.ProgressLogger pl = DocsPaUtils.LogsManagement.ProgressLogger.getInstance(false);
        //    int idMessage = pl.NewMessage("Caricamento Titolario");

        //    try
        //    {
        //        logger.Debug("Titolario->UpdateStructure");

        //        UserData userData = new UserData(false);
        //        XmlNode docspa = parser.SelectSingleNode("DOCSPA");
        //        if (docspa != null)
        //        {
        //            //DocsPaUtils.LogsManagement.ProgressLogger pl=DocsPaUtils.LogsManagement.ProgressLogger.getInstance(false);
        //            //int idMessage=pl.NewMessage("Caricamento Titolario");

        //            foreach (XmlNode node in docspa.SelectNodes("TITOLARIO"))
        //            {
        //                if (!Login())
        //                {
        //                    pl.UpdateMessage(idMessage, null, false, true);

        //                    logger.Debug("Errore Login Documentale.");
        //                    throw new Exception();
        //                }

        //                NavigateTreeForUpdate(node, 0, userData, "0", "", "", 1);


        //            }
        //            //pl.UpdateMessage(idMessage,null,true,false);
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        logger.Debug("Errore durante la lettura dell'XML", exception);
        //        errorCode = ErrorCode.GenericError;
        //    }

        //    if (errorCode != ErrorCode.NoError)
        //    {
        //        result = false;
        //    }

        //    if (!result)
        //    {
        //        pl.UpdateMessage(idMessage, null, true, true);
        //    }
        //    else
        //    {
        //        pl.UpdateMessage(idMessage, null, true, false);
        //    }

        //    return result;
        //}


        ///// <summary>
        ///// Procedura ricorsiva per la lettura dell'XML
        ///// </summary>
        ///// <param name="rootNode"></param>
        ///// <param name="livello"></param>
        ///// <param name="idParent"></param>
        ///// <param name="utente"></param>
        ///// <param name="ruolo"></param>
        ///// <param name="idAmm"></param>
        ///// <param name="registro"></param>
        //private void NavigateTree(XmlNode rootNode, int livello, UserData userData, string idParent, string idFascicolo, int totalItems, string ordinamento, int contatore)
        //{
        //    //logger.Debug("Analisi in corso del titolario: ID Fascicolo: " + idFascicolo + " - Livello " + livello);

        //    // Acquisisci tipologia nodo
        //    string tipo = null;
        //    XmlAttribute attribute = rootNode.Attributes["TIPO"];

        //    if (attribute != null)
        //    {
        //        tipo = attribute.InnerText.ToUpper();
        //    }

        //    string ordering = ordinamento;

        //    // Estrazione dati e nodi sottostanti
        //    foreach (XmlNode node in rootNode.ChildNodes)
        //    {
        //        switch (node.Name)
        //        {
        //            case "DATI":
        //                if (!GetUserData(ref userData, node))
        //                {
        //                    errorCode = ErrorCode.GenericError;
        //                    throw new Exception();
        //                }

        //                // Scrittura del nodo nel database
        //                if (userData.ruolo != "" && userData.idAmm != "")
        //                {
        //                    if (tipo == "TITOLARIO")
        //                    {
        //                        // Aggiorna il file di log
        //                        DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", totalItems).IncreaseCounter();

        //                        ordering = ordinamento + contatore.ToString("0000");

        //                        // Acquisizione utente
        //                        DocsPaVO.utente.InfoUtente infoUtenteXml;

        //                        DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
        //                        amministrazioneXml.GetUserByName(out infoUtenteXml, userData.utente, userData.ruolo);

        //                        infoUtenteXml.idAmministrazione = userData.idAmm;
        //                        //infoUtente.idAmministrazione = userData.idAmm;

        //                        if (infoUtenteXml.idGruppo == null || infoUtenteXml.idPeople == null)
        //                        {
        //                            errorCode = ErrorCode.BadUser;
        //                            throw new Exception();
        //                        }

        //                        this.infoUtente.idGruppo = infoUtenteXml.idGruppo;
        //                        this.infoUtente.idPeople = infoUtenteXml.idPeople;

        //                        // Acquisizione nodo titolario
        //                        DocsPaVO.fascicolazione.Classificazione nodoTitolario = new DocsPaVO.fascicolazione.Classificazione();
        //                        nodoTitolario.descrizione = userData.descrizione;
        //                        nodoTitolario.registro = new DocsPaVO.utente.Registro();
        //                        nodoTitolario.registro.systemId = userData.idRegistro;
        //                        if (nodoTitolario.registro.systemId.Equals("") || nodoTitolario.registro.systemId == null) nodoTitolario.registro.systemId = "NULL";
        //                        nodoTitolario.codice = userData.codice;
        //                        nodoTitolario.livello = livello.ToString();

        //                        // Creazione Titolario
        //                        DocsPaVO.fascicolazione.Classificazione result = BusinessLogic.Fascicoli.TitolarioManager.newTitolario(this.infoUtente, nodoTitolario, idParent, true);

        //                        idParent = result.systemID;
        //                        amministrazioneXml.UpdateOrdinamento(result.systemID, ordering);

        //                        //inserimento delle regole di visibilità per i ruoli
        //                        XmlNode ruoli = rootNode.SelectSingleNode("RUOLI");
        //                        if (ruoli != null)
        //                        {
        //                            foreach (XmlNode ruolo in ruoli.SelectNodes("RUOLO"))
        //                            {
        //                                string gruppo = ruolo.InnerText;
        //                                amministrazioneXml.NewVisibilita(result.systemID, amministrazioneXml.GetGroupByName(gruppo));
        //                            }
        //                        }

        //                        logger.Debug("Importato il nodo Titolario: " + nodoTitolario.codice + " - " + nodoTitolario.descrizione + " (Livello " + livello + ")");
        //                    }
        //                    else if (tipo != null && tipo.StartsWith("FASCICOLO"))
        //                    {
        //                        // Aggiorna il file di log
        //                        DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", totalItems).IncreaseCounter();

        //                        // Acquisizione utente
        //                        DocsPaVO.utente.InfoUtente infoUtenteXml;

        //                        DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
        //                        amministrazioneXml.GetUserByName(out infoUtenteXml, userData.utente, userData.ruolo);

        //                        infoUtenteXml.idAmministrazione = userData.idAmm;

        //                        if (infoUtenteXml.idGruppo == null || infoUtenteXml.idPeople == null)
        //                        {
        //                            errorCode = ErrorCode.BadUser;
        //                            throw new Exception();
        //                        }

        //                        this.infoUtente.idGruppo = infoUtenteXml.idGruppo;
        //                        this.infoUtente.idPeople = infoUtenteXml.idPeople;

        //                        // Acquisizione nodo fascicolo procedimentale
        //                        DocsPaVO.fascicolazione.Fascicolo nodoFascicolo = new DocsPaVO.fascicolazione.Fascicolo();
        //                        nodoFascicolo.descrizione = userData.descrizione;
        //                        nodoFascicolo.codice = userData.codice;
        //                        nodoFascicolo.idClassificazione = idParent;
        //                        nodoFascicolo.stato = "A";

        //                        if (tipo.EndsWith("PROCEDIMENTALE"))
        //                        {
        //                            nodoFascicolo.tipo = "P";
        //                        }
        //                        else if (tipo.EndsWith("GENERALE"))
        //                        {
        //                            nodoFascicolo.tipo = "G";
        //                        }

        //                        // Creazione fascicolo
        //                        DocsPaVO.fascicolazione.Fascicolo result = BusinessLogic.Fascicoli.FascicoloManager.NewFascicoloXml(userData.idRegistro, nodoFascicolo, this.infoUtente);

        //                        //copia le visibilità del nodo superiore
        //                        amministrazioneXml.CopyVisibilita2(idParent, result.systemID);

        //                        idParent = result.systemID;
        //                        idFascicolo = result.systemID;

        //                        logger.Debug("Importato il nodo fascicolo: " + nodoFascicolo.codice + " - " + nodoFascicolo.descrizione + " (Livello " + livello + ")");
        //                    }
        //                    else if (tipo != null && tipo.EndsWith("FOLDER"))
        //                    {
        //                        // Aggiorna il file di log
        //                        DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", totalItems).IncreaseCounter();

        //                        // Acquisizione utente
        //                        DocsPaVO.utente.InfoUtente infoUtenteXml;

        //                        DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
        //                        amministrazioneXml.GetUserByName(out infoUtenteXml, userData.utente, userData.ruolo);

        //                        infoUtenteXml.idAmministrazione = userData.idAmm;

        //                        if (infoUtenteXml.idGruppo == null || infoUtenteXml.idPeople == null)
        //                        {
        //                            errorCode = ErrorCode.BadUser;
        //                            throw new Exception();
        //                        }

        //                        this.infoUtente.idGruppo = infoUtenteXml.idGruppo;
        //                        this.infoUtente.idPeople = infoUtenteXml.idPeople;

        //                        // Acquisizione nodo folder
        //                        DocsPaVO.fascicolazione.Folder nodoFolder = new DocsPaVO.fascicolazione.Folder();
        //                        nodoFolder.descrizione = userData.descrizione;
        //                        nodoFolder.idParent = idParent;
        //                        nodoFolder.idFascicolo = idFascicolo;

        //                        // Creazione folder
        //                        if (!BusinessLogic.Fascicoli.FolderManager.NewFolderXml(ref nodoFolder, infoUtente))
        //                        {
        //                            throw new Exception();
        //                        }

        //                        //copia le visibilità del nodo superiore
        //                        amministrazioneXml.CopyVisibilita2(idParent, nodoFolder.systemID);

        //                        idParent = nodoFolder.systemID;

        //                        logger.Debug("Importato il nodo folder: " + nodoFolder.descrizione + " (Livello " + livello + ")");
        //                    }
        //                }

        //                break;

        //            case "NODI":
        //                int contatoreNodo = 1;

        //                foreach (XmlNode childNode in node.ChildNodes)
        //                {
        //                    NavigateTree(childNode, livello + 1, userData, idParent, idFascicolo, totalItems, ordering, contatoreNodo);
        //                    contatoreNodo++;
        //                }

        //                break;
        //        }
        //    }
        //}

        //private void NavigateTreeForUpdate(XmlNode rootNode, int livello, UserData userData, string idParent, string idFascicolo, string ordinamento, int contatore)
        //{
        //    // Acquisisci tipologia nodo
        //    //logger.Debug("NavigateTreeForUpdate");
        //    string tipo = null;
        //    string mode = null;
        //    string idNodo = idParent;
        //    XmlAttribute attribute = rootNode.Attributes["TIPO"];

        //    if (attribute != null)
        //    {
        //        tipo = attribute.InnerText.ToUpper();
        //        XmlAttribute modeAttr = rootNode.Attributes["MODE"];
        //        if (modeAttr != null)
        //        {
        //            mode = modeAttr.InnerText.ToUpper();
        //        }
        //    }
        //    string ordering = ordinamento;

        //    // Estrazione dati e nodi sottostanti
        //    //logger.Debug("Livello: " + livello + " contatore: " + contatore);
        //    foreach (XmlNode node in rootNode.ChildNodes)
        //    {
        //        DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();

        //        //logger.Debug(node.Name);

        //        switch (node.Name)
        //        {

        //            case "DATI":
        //                if (!GetUserData(ref userData, node))
        //                {
        //                    errorCode = ErrorCode.GenericError;
        //                    throw new Exception();
        //                }

        //                if (userData.ruolo == null || userData.ruolo == "")
        //                {
        //                    userData.ruolo = amministrazioneXml.GetUserRole(userData.utente);
        //                }
        //                // Scrittura del nodo nel database
        //                //if(userData.ruolo != "" && userData.idAmm != "")
        //                if (userData.idAmm != "")
        //                {
        //                    if (tipo == "TITOLARIO")
        //                    {
        //                        ordering = ordinamento + contatore.ToString("0000");
        //                        //logger.Debug(ordering);
        //                        // Acquisizione utente
        //                        DocsPaVO.utente.InfoUtente infoUtenteXml;

        //                        amministrazioneXml.GetUserByName(out infoUtenteXml, userData.utente, userData.ruolo);

        //                        infoUtenteXml.idAmministrazione = userData.idAmm;
        //                        infoUtente.idAmministrazione = userData.idAmm;

        //                        //if(infoUtenteXml.idGruppo == null || infoUtenteXml.idPeople == null)
        //                        if (infoUtenteXml.idPeople == null)
        //                        {
        //                            errorCode = ErrorCode.BadUser;
        //                            throw new Exception();
        //                        }

        //                        this.infoUtente.idGruppo = infoUtenteXml.idGruppo;
        //                        this.infoUtente.idPeople = infoUtenteXml.idPeople;
        //                        this.infoUtente.userId = userData.utente;

        //                        // Acquisizione nodo titolario
        //                        DocsPaVO.fascicolazione.Classificazione nodoTitolario = new DocsPaVO.fascicolazione.Classificazione();
        //                        nodoTitolario.descrizione = userData.descrizione;

        //                        nodoTitolario.registro = new DocsPaVO.utente.Registro();
        //                        //nodoTitolario.registro.systemId = amministrazioneXml.GetRegByName(userData.registro);
        //                        nodoTitolario.registro.systemId = userData.idRegistro;
        //                        if (nodoTitolario.registro.systemId == "") nodoTitolario.registro.systemId = null;
        //                        nodoTitolario.codice = userData.codice;
        //                        nodoTitolario.livello = livello.ToString();
        //                        //logger.Debug("TIT: " + userData.descrizione );	

        //                        if (mode == "MODIFIED")
        //                        {
        //                            // Modifica Titolario
        //                            //legge l'ID
        //                            DocsPaDB.Query_DocsPAWS.Fascicoli progetto = new DocsPaDB.Query_DocsPAWS.Fascicoli();
        //                            string systemId = progetto.GetProjectID(nodoTitolario.codice, "T", userData.idAmm);
        //                            if (systemId != null)
        //                            {
        //                                idNodo = systemId;
        //                                nodoTitolario.systemID = systemId;
        //                                DocsPaVO.fascicolazione.Classificazione result = BusinessLogic.Fascicoli.TitolarioManager.updateTitolario(nodoTitolario);
        //                                //idParent = result.systemID;
        //                                amministrazioneXml.UpdateOrdinamento(idNodo, ordering);
        //                                //cancella tutte le visibilità del nodo
        //                                amministrazioneXml.DeleteVisibilita(result.systemID);
        //                                //reinserisce le visibilità
        //                                XmlNode ruoli = rootNode.SelectSingleNode("RUOLI");
        //                                if (ruoli != null)
        //                                {
        //                                    foreach (XmlNode ruolo in ruoli.SelectNodes("RUOLO"))
        //                                    {
        //                                        string gruppo = ruolo.InnerText;
        //                                        amministrazioneXml.NewVisibilita(result.systemID, amministrazioneXml.GetGroupByName(gruppo));
        //                                    }
        //                                }
        //                                //trasferisce le visibilità del nodo titolario al fascicolo generale e al root folder
        //                                //legge il fascicolo generale associato al nodo titolario
        //                                string idFascGen = amministrazioneXml.GetIdFascicoloGenerale(idNodo);
        //                                if (idFascGen != null)
        //                                {
        //                                    //elimina le visibilità del fascicolo generale
        //                                    amministrazioneXml.DeleteVisibilita(idFascGen);
        //                                    //copia le visibilità del nodo superiore
        //                                    amministrazioneXml.CopyVisibilita2(idNodo, idFascGen);

        //                                    System.Data.DataSet folder;

        //                                    //legge i folder associati al fascicolo
        //                                    if (amministrazioneXml.GetFolderFascicolo(idFascGen, out folder))
        //                                    {
        //                                        if (folder != null)
        //                                        {
        //                                            foreach (System.Data.DataRow row in folder.Tables[0].Rows)
        //                                            {
        //                                                //elimina le visibilità del fascicolo generale
        //                                                amministrazioneXml.DeleteVisibilita(row["SYSTEM_ID"].ToString());
        //                                                //copia le visibilità del nodo superiore
        //                                                amministrazioneXml.CopyVisibilita2(idFascGen, row["SYSTEM_ID"].ToString());
        //                                            }
        //                                        }
        //                                    }

        //                                }

        //                            }
        //                        }
        //                        else if (mode == "CREATED")
        //                        {
        //                            // Creazione Titolario
        //                            DocsPaVO.fascicolazione.Classificazione result = BusinessLogic.Fascicoli.TitolarioManager.newTitolario(this.infoUtente, nodoTitolario, idParent, true);
        //                            idNodo = result.systemID;
        //                            if (idNodo != null)
        //                            {
        //                                amministrazioneXml.UpdateOrdinamento(idNodo, ordering);
        //                                amministrazioneXml.DeleteVisibilita(idNodo);
        //                                //reinserisce le visibilità
        //                                XmlNode ruoli = rootNode.SelectSingleNode("RUOLI");
        //                                if (ruoli != null)
        //                                {
        //                                    foreach (XmlNode ruolo in ruoli.SelectNodes("RUOLO"))
        //                                    {
        //                                        string gruppo = ruolo.InnerText;
        //                                        amministrazioneXml.NewVisibilita(idNodo, amministrazioneXml.GetGroupByName(gruppo));
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        else if (mode == "DELETED")
        //                        {
        //                            //TO DO - cancellazione nodo titolario
        //                        }
        //                        else if (mode == "ORDERED")
        //                        {
        //                            //riordinamento del nodo
        //                            DocsPaDB.Query_DocsPAWS.Fascicoli progetto = new DocsPaDB.Query_DocsPAWS.Fascicoli();
        //                            idNodo = progetto.GetProjectID(nodoTitolario.codice, "T", userData.idAmm);
        //                            if (idNodo != null)
        //                            {
        //                                amministrazioneXml.UpdateOrdinamento(idNodo, ordering);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            DocsPaDB.Query_DocsPAWS.Fascicoli progetto = new DocsPaDB.Query_DocsPAWS.Fascicoli();
        //                            idNodo = progetto.GetProjectID(userData.codice, "T", userData.idAmm);
        //                        }

        //                    }
        //                    else if (tipo != null && tipo.StartsWith("FASCICOLO"))
        //                    {
        //                        // Acquisizione utente
        //                        DocsPaVO.utente.InfoUtente infoUtenteXml;

        //                        //DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
        //                        amministrazioneXml.GetUserByName(out infoUtenteXml, userData.utente, userData.ruolo);

        //                        infoUtenteXml.idAmministrazione = userData.idAmm;

        //                        //if(infoUtenteXml.idGruppo == null || infoUtenteXml.idPeople == null)
        //                        if (infoUtenteXml.idPeople == null)
        //                        {
        //                            errorCode = ErrorCode.BadUser;
        //                            throw new Exception();
        //                        }

        //                        this.infoUtente.idGruppo = infoUtenteXml.idGruppo;
        //                        this.infoUtente.idPeople = infoUtenteXml.idPeople;

        //                        // Acquisizione nodo fascicolo procedimentale
        //                        DocsPaVO.fascicolazione.Fascicolo nodoFascicolo = new DocsPaVO.fascicolazione.Fascicolo();
        //                        nodoFascicolo.descrizione = userData.descrizione;
        //                        nodoFascicolo.codice = userData.codice;
        //                        nodoFascicolo.idClassificazione = idParent;
        //                        nodoFascicolo.stato = "A";

        //                        if (tipo.EndsWith("PROCEDIMENTALE"))
        //                        {
        //                            nodoFascicolo.tipo = "P";
        //                        }
        //                        else if (tipo.EndsWith("GENERALE"))
        //                        {
        //                            nodoFascicolo.tipo = "G";
        //                        }

        //                        if (mode == "CREATED")
        //                        {
        //                            // Creazione fascicolo
        //                            DocsPaVO.fascicolazione.Fascicolo result = BusinessLogic.Fascicoli.FascicoloManager.NewFascicoloXml(userData.idRegistro, nodoFascicolo, this.infoUtente);
        //                            idNodo = result.systemID;
        //                            idFascicolo = result.systemID;
        //                            //cancella le visibilità inserite
        //                            amministrazioneXml.DeleteVisibilita(result.systemID);
        //                            //copia le visibilità del nodo superiore
        //                            amministrazioneXml.CopyVisibilita2(idParent, idFascicolo);

        //                        }
        //                        if (mode == "MODIFIED")
        //                        {
        //                            DocsPaDB.Query_DocsPAWS.Fascicoli progetto = new DocsPaDB.Query_DocsPAWS.Fascicoli();
        //                            string systemId = progetto.GetProjectID(nodoFascicolo.codice, "F", userData.idAmm);
        //                            if (systemId != null)
        //                            {
        //                                nodoFascicolo.systemID = systemId;
        //                                DocsPaVO.fascicolazione.Fascicolo result = BusinessLogic.Fascicoli.FascicoloManager.updateFascicolo(nodoFascicolo);
        //                                idNodo = result.systemID;
        //                                idFascicolo = result.systemID;
        //                            }
        //                        }
        //                        if (mode == "DELETED")
        //                        {
        //                            //TO DO -> cancellazione fascicolo
        //                        }
        //                    }
        //                    else if (tipo != null && tipo.EndsWith("FOLDER"))
        //                    {
        //                        // Acquisizione utente
        //                        DocsPaVO.utente.InfoUtente infoUtenteXml;

        //                        //DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
        //                        amministrazioneXml.GetUserByName(out infoUtenteXml, userData.utente, userData.ruolo);

        //                        infoUtenteXml.idAmministrazione = userData.idAmm;

        //                        //if(infoUtenteXml.idGruppo == null || infoUtenteXml.idPeople == null)
        //                        if (infoUtenteXml.idPeople == null)
        //                        {
        //                            errorCode = ErrorCode.BadUser;
        //                            throw new Exception();
        //                        }

        //                        this.infoUtente.idGruppo = infoUtenteXml.idGruppo;
        //                        this.infoUtente.idPeople = infoUtenteXml.idPeople;

        //                        // Acquisizione nodo folder
        //                        DocsPaVO.fascicolazione.Folder nodoFolder = new DocsPaVO.fascicolazione.Folder();
        //                        nodoFolder.descrizione = userData.descrizione;
        //                        nodoFolder.idParent = idParent;
        //                        nodoFolder.idFascicolo = idFascicolo;

        //                        if (mode == "CREATED")
        //                        {
        //                            // Creazione folder
        //                            if (!BusinessLogic.Fascicoli.FolderManager.NewFolderXml(ref nodoFolder, infoUtente))
        //                            {
        //                                throw new Exception();
        //                            }

        //                            idNodo = nodoFolder.systemID;
        //                            //cancella le visibilità inserite
        //                            amministrazioneXml.DeleteVisibilita(idNodo);
        //                            //copia le visibilità del nodo superiore
        //                            amministrazioneXml.CopyVisibilita2(nodoFolder.idFascicolo, idNodo);

        //                        }
        //                        if (mode == "MODIFIED")
        //                        {
        //                            // modifica folder
        //                            //									DocsPaDB.Query_DocsPAWS.Fascicoli progetto=new DocsPaDB.Query_DocsPAWS.Fascicoli();
        //                            //									string systemId=progetto.GetProjectID(nodoFolder.codice,"C");
        //                            //									if(systemId!=null)
        //                            //									{
        //                            //										nodoFascicolo.systemID=systemId;
        //                            //										if(!BusinessLogic.Fascicoli.FolderManager.modifyFolder(nodoFolder))
        //                            //										{
        //                            //											throw new Exception();
        //                            //										}
        //                            //
        //                            //										idParent = nodoFolder.systemID;
        //                            //									}
        //                        }
        //                        if (mode == "DELETED")
        //                        {
        //                        }

        //                    }
        //                }

        //                break;

        //            case "NODI":
        //                int contatoreNodo = 1;
        //                foreach (XmlNode childNode in node.ChildNodes)
        //                {
        //                    NavigateTreeForUpdate(childNode, livello + 1, userData, idNodo, idFascicolo, ordering, contatoreNodo);
        //                    contatoreNodo++;
        //                }

        //                break;
        //        }

        //    }
        //}

        public bool ExportStructure(bool soloNodiTitolario)
        {
            bool result = false;
            XmlDocument doc = ExportStructureCommon();
            if (doc != null)
            {
                result = true;

            }
            return result;
        }

        /*public bool ExportStructure(bool soloNodiTitolario)
        {
            bool result = true;
            try
            {
                //salva il flag per i soli nodi titolario
                soloNodiT=soloNodiTitolario;
                System.Data.DataSet amministrazioni;

                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                result=amministrazioneXml.Exp_GetAmministrazioni(out amministrazioni);
                if(!result)
                {
                    throw new Exception();
                }

                string idAmm;
                string codiceAmm;

                foreach( System.Data.DataRow row in amministrazioni.Tables["AMMINISTRAZIONI"].Rows)
                {
					
                    codiceAmm=row["VAR_CODICE_AMM"].ToString();
                    idAmm=row["SYSTEM_ID"].ToString();

                    XmlDocument doc = new XmlDocument();

                    //Create the root element and 
                    //add it to the document.
                    XmlNode docspa=doc.AppendChild(doc.CreateElement("DOCSPA"));
                    XmlNode root=docspa.AppendChild(doc.CreateElement("TITOLARIO"));
                    XmlNode datiRoot=root.AppendChild(doc.CreateElement("DATI"));
					
					
                    datiRoot.AppendChild(doc.CreateElement("IDAMM")).InnerText = codiceAmm;

                    result=ExportStructureRecursive(doc,root,idAmm,"0");

                    doc.Save ("C:\\Titolario_" + codiceAmm + ".xml");

                }
            }
            catch(Exception exception)
            {
                logger.Debug("Errore durante la creazione dell'XML->Titolario", exception);
                result=false;
            }

            return result;
        }*/

        public XmlDocument ExportStructureXmlDocument()
        {
            return ExportStructureCommon();
        }

        public XmlDocument ExportStructureCommon()
        {
            XmlDocument outputDoc = null;
            try
            {
                //salva il flag per i soli nodi titolario
                soloNodiT = true;
                System.Data.DataSet amministrazioni;

                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                if (!amministrazioneXml.Exp_GetAmministrazioni(out amministrazioni))
                {
                    throw new Exception();
                }

                string idAmm;
                string codiceAmm;

                XmlDocument doc = new XmlDocument();
                XmlNode docspa = doc.AppendChild(doc.CreateElement("DOCSPA"));

                foreach (System.Data.DataRow row in amministrazioni.Tables["AMMINISTRAZIONI"].Rows)
                {

                    codiceAmm = row["VAR_CODICE_AMM"].ToString().ToUpper();
                    idAmm = row["SYSTEM_ID"].ToString();

                    //Create the root element and 
                    //add it to the document.
                    XmlNode root = docspa.AppendChild(doc.CreateElement("TITOLARIO"));
                    XmlNode datiRoot = root.AppendChild(doc.CreateElement("DATI"));

                    datiRoot.AppendChild(doc.CreateElement("IDAMM")).InnerText = codiceAmm;

                    if (!ExportStructureRecursive(doc, root, idAmm, "0"))
                    //if(!ExportStructureRecursive_2(doc,root,idAmm))  - GADAMO: ipotesi di gestione diversa del file XML del titolario
                    {
                        throw new Exception();
                    }

                }
                outputDoc = doc;
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la creazione dell'XML->Titolario per amministrazione", exception);
                outputDoc = null;
            }

            logger.Debug("Amministrazione: Export XML titolario: OK!");
            return outputDoc;
        }

        private bool ExportStructureRecursive(XmlDocument doc, XmlNode node, string idAmm, string parent)
        {
            bool result = true;
            try
            {
                System.Data.DataSet dataSet;
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                result = amministrazioneXml.Exp_GetNodiTitolario(out dataSet, idAmm, parent, soloNodiT);

                if (!result)
                {
                    throw new Exception();
                }
                if (dataSet != null)
                {
                    if (dataSet.Tables["NODITITOLARIO"].Rows.Count > 0)
                    {
                        XmlNode nodi = node.AppendChild(doc.CreateElement("NODI"));
                        foreach (System.Data.DataRow row in dataSet.Tables["NODITITOLARIO"].Rows)
                        {
                            XmlNode nodo = nodi.AppendChild(doc.CreateElement("NODO"));

                            string tipo = row["CHA_TIPO_PROJ"].ToString();
                            if (tipo == "T")
                            {
                                tipo = "Titolario";
                            }
                            else if (tipo == "F")
                            {
                                string tipoFasc = row["CHA_TIPO_PROJ"].ToString();
                                if (tipoFasc == "G")
                                {
                                    tipo = "FascicoloGenerale";
                                }
                                else
                                {
                                    tipo = "FascicoloProcedimentale";
                                }
                            }
                            else if (tipo == "C")
                            {
                                tipo = "Folder";
                                if (row["ID_PARENT"].ToString() == row["ID_FASCICOLO"].ToString())
                                {
                                    tipo = "RootFolder";
                                }
                            }
                            XmlNode attr = doc.CreateNode(XmlNodeType.Attribute, "TIPO", "");
                            attr.Value = tipo;
                            //Add the attribute to the document.
                            nodo.Attributes.SetNamedItem(attr);

                            XmlNode datiNodo = nodo.AppendChild(doc.CreateElement("DATI"));
                            datiNodo.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText = row["DESCRIPTION"].ToString();
                            string codice = row["VAR_CODICE"].ToString().ToUpper();
                            if (codice != null && codice != "")
                            {
                                //logger.Debug("Nodo Titolario: " + codice);
                                datiNodo.AppendChild(doc.CreateElement("CODICE")).InnerText = codice;
                                string codiceRegistro = "";

                                if (row["ID_REGISTRO"] != null)
                                {
                                    if (row["ID_REGISTRO"].ToString() != "")
                                    {
                                        codiceRegistro = amministrazioneXml.Exp_GetRegistroByID(row["ID_REGISTRO"].ToString());
                                    }
                                }

                                if (codiceRegistro == null) codiceRegistro = "";
                                datiNodo.AppendChild(doc.CreateElement("REGISTRO")).InnerText = codiceRegistro;
                                //esport delle visibilità per ruolo

                                System.Data.DataSet ruoliDataSet;
                                amministrazioneXml.Exp_GetVisibilitaRuolo(out ruoliDataSet, row["SYSTEM_ID"].ToString());
                                if (ruoliDataSet != null)
                                {
                                    if (ruoliDataSet.Tables["VISIBILITARUOLO"].Rows.Count > 0)
                                    {
                                        XmlNode ruoli = nodo.AppendChild(doc.CreateElement("RUOLI"));
                                        foreach (System.Data.DataRow rowRuoli in ruoliDataSet.Tables["VISIBILITARUOLO"].Rows)
                                        {
                                            ruoli.AppendChild(doc.CreateElement("RUOLO")).InnerText = rowRuoli["GROUP_ID"].ToString();
                                        }
                                    }
                                }
                            }

                            result = ExportStructureRecursive(doc, nodo, idAmm, row["SYSTEM_ID"].ToString());
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Debug("Errore durante la creazione dell'XML", exception);
                result = false;
            }

            return result;
        }

        #region GADAMO: ipotesi di gestione diversa del file XML del titolario

        //		/// <summary>
        //		/// TEST GADAMO
        //		/// </summary>
        //		/// <returns></returns>
        //		public XmlDocument ExportSecurityXmlDocument()
        //		{
        //			XmlDocument doc = new XmlDocument();
        //			try
        //			{
        //				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
        //				
        //				//esport delle visibilità per ruolo e registro
        //				System.Data.DataSet ruoliDataSet;
        //				amministrazioneXml.Exp_GetVisibilitaRuoloReg(out ruoliDataSet);
        //				if(ruoliDataSet!=null)
        //				{
        //					XmlNode nodoroot = doc.AppendChild(doc.CreateElement("SECURITY"));
        //					if(ruoliDataSet.Tables["VISIBILITARUOLO"].Rows.Count > 0)
        //					{
        //						foreach( System.Data.DataRow row in ruoliDataSet.Tables["VISIBILITARUOLO"].Rows)
        //						{
        //							XmlNode nodo = nodoroot.AppendChild(doc.CreateElement("NODO"));
        //
        //							XmlNode attrAmm = doc.CreateNode(XmlNodeType.Attribute, "AMM","");
        //							attrAmm.Value = row["VAR_CODICE_AMM"].ToString();							
        //							nodo.Attributes.SetNamedItem(attrAmm);
        //
        //							XmlNode attrID = doc.CreateNode(XmlNodeType.Attribute, "ID","");
        //							attrID.Value = row["THING"].ToString();							
        //							nodo.Attributes.SetNamedItem(attrID);
        //
        //							XmlNode attrRuolo = doc.CreateNode(XmlNodeType.Attribute, "RUOLO","");
        //							attrRuolo.Value = row["GROUP_ID"].ToString();							
        //							nodo.Attributes.SetNamedItem(attrRuolo);
        //
        //							XmlNode attrReg = doc.CreateNode(XmlNodeType.Attribute, "REG","");
        //							attrReg.Value = row["VAR_CODICE"].ToString();							
        //							nodo.Attributes.SetNamedItem(attrReg);
        //						}
        //					}
        //				}
        //			}
        //			catch(Exception exception)
        //			{
        //				logger.Debug("Errore durante la creazione dell'XML Security", exception);
        //				doc = null;
        //			}
        //
        //			return doc;
        //		}

        //		/// <summary>
        //		/// TEST GADAMO
        //		/// </summary>
        //		/// <param name="doc"></param>
        //		/// <param name="node"></param>
        //		/// <returns>bool</returns>
        //		private bool ExportStructureRecursive_2(XmlDocument doc, XmlNode node, string idAmm)
        //		{
        //			bool result = true;
        //			try
        //			{
        //				System.Data.DataSet dataSet;
        //				DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
        //				result = amministrazioneXml.Exp_GetNodiTitolario_2(out dataSet,idAmm);
        //
        //				if(!result)
        //				{
        //					throw new Exception();
        //				}
        //				if(dataSet != null)
        //				{
        //					if(dataSet.Tables["NODITITOLARIO"].Rows.Count > 0)
        //					{
        //						XmlNode nodi=node.AppendChild(doc.CreateElement("NODI"));
        //						foreach( System.Data.DataRow row in dataSet.Tables["NODITITOLARIO"].Rows)
        //						{
        //							XmlNode nodo=nodi.AppendChild(doc.CreateElement("NODO"));
        //
        //							XmlNode attrID = doc.CreateNode(XmlNodeType.Attribute, "ID","");
        //							attrID.Value = row["SYSTEM_ID"].ToString();							
        //							nodo.Attributes.SetNamedItem(attrID);
        //
        //							XmlNode attrIDP = doc.CreateNode(XmlNodeType.Attribute, "PARENT","");
        //							attrIDP.Value = row["ID_PARENT"].ToString();							
        //							nodo.Attributes.SetNamedItem(attrIDP);
        //
        //							XmlNode attrCod = doc.CreateNode(XmlNodeType.Attribute, "COD","");
        //							attrCod.Value = row["VAR_CODICE"].ToString();							
        //							nodo.Attributes.SetNamedItem(attrCod);
        //
        //							XmlNode attrDesc = doc.CreateNode(XmlNodeType.Attribute, "DESC","");
        //							attrDesc.Value = row["DESCRIPTION"].ToString();							
        //							nodo.Attributes.SetNamedItem(attrDesc);
        //
        //							XmlNode attrLiv = doc.CreateNode(XmlNodeType.Attribute, "LIV","");
        //							attrLiv.Value = row["NUM_LIVELLO"].ToString();							
        //							nodo.Attributes.SetNamedItem(attrLiv);
        //						}
        //					}
        //				}
        //			}
        //			catch(Exception exception)
        //			{
        //				logger.Debug("Errore durante la creazione dell'XML del TITOLARIO", exception);
        //				result=false;
        //			}
        //
        //			return result;
        //		}	

        #endregion

        #region gestione TITOLARIO diretto al db (gadamo)

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string NodoTit(string codAmm, string idParent)
        {
            System.Data.DataSet ds;

            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                ds = obj.Nodo_Titolario(codAmm, idParent);
                if (ds != null)
                {
                    result = ds.GetXml();
                }
                else
                {
                    result = null;
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="idParent"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public string NodoTitSecurity(string idAmm, string idParent, string idGruppo, string idRegistro, string idTitolario)
        {
            System.Data.DataSet ds;

            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                ds = obj.Nodo_Titolario_Security(idAmm, idParent, idGruppo, idRegistro, idTitolario);
                if (ds != null)
                {
                    result = ds.GetXml();
                }
                else
                {
                    result = null;
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codAmm"></param>
        /// <returns></returns>
        public string registriInAmm(string codAmm)
        {
            System.Data.DataSet ds;

            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                ds = obj.RegistriInAmministrazione(codAmm);
                if (ds != null)
                {
                    result = ds.GetXml();
                }
                else
                {
                    result = null;
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public string registriInAmmRestricted(string sessionID)
        {
            System.Data.DataSet ds;

            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                ds = obj.RegistriInAmministrazioneRestricted(sessionID);
                if (ds != null)
                {
                    result = ds.GetXml();
                }
                else
                {
                    result = null;
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public string securityNodoRuoli(string idNodo, string codAmm)
        {
            System.Data.DataSet ds;

            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                ds = obj.securityNodo_Ruoli(idNodo, codAmm);
                if (ds != null)
                {
                    result = ds.GetXml();
                }
                else
                {
                    result = null;
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public string filtroRicTit(string codice, string descrizione, string idAmm)
        {
            System.Data.DataSet ds;

            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                ds = obj.filtroRicTitolario(codice, descrizione, idAmm);
                if (ds != null)
                {
                    result = ds.GetXml();
                }
                else
                {
                    result = null;
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public string filtroRicTitAmm(string codice, string descrizione, string codAmm, string idRegistro)
        {
            System.Data.DataSet ds;

            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                ds = obj.filtroRicTitolarioAmm(codice, descrizione, codAmm, idRegistro);
                if (ds != null)
                {
                    result = ds.GetXml();
                }
                else
                {
                    result = null;
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public string cercaNodoByCod(string codClass, string idAmm, string idReg, string idTitolario)
        {
            System.Data.DataSet ds;

            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                ds = obj.cercaNodoCod(codClass, idAmm, idReg, idTitolario);
                if (ds != null)
                {
                    result = ds.GetXml();
                }
                else
                {
                    result = null;
                }
            }
            catch
            {
                result = null;
            }
            return result;
        }

        public string cercaNodoRoot(string idrecord, string idparent, int livello)
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            System.Data.DataSet dataSet;

            DataColumn dc;
            ds.Tables.Add("Nodi");
            dc = new DataColumn("id");
            ds.Tables["Nodi"].Columns.Add(dc);
            dc = new DataColumn("padre");
            ds.Tables["Nodi"].Columns.Add(dc);
            dc = new DataColumn("livello");
            ds.Tables["Nodi"].Columns.Add(dc);

            string result;
            DataRow row;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();

                for (int n = livello; n >= 1; n--)
                {
                    dataSet = obj.ricercaRoot(idrecord);

                    if (dataSet.Tables[0].Rows.Count > 0)
                    {
                        idparent = dataSet.Tables[0].Rows[0]["PADRE"].ToString();
                    }

                    row = ds.Tables["Nodi"].NewRow();
                    row["id"] = idrecord;
                    row["padre"] = idparent;
                    row["livello"] = Convert.ToString(n);
                    ds.Tables["Nodi"].Rows.Add(row);

                    logger.Debug("idrecord= " + idrecord);
                    logger.Debug("idparent= " + idparent);
                    logger.Debug("livello = " + Convert.ToString(n));

                    idrecord = idparent;
                }

                if (ds != null)
                {
                    result = ds.GetXml();
                }
                else
                {
                    result = null;
                }

            }
            catch
            {
                result = null;
            }
            return result;
        }

        public string AggiungeNewNodo(
                                    string padre,
                                    string codice,
                                    string descrizione,
                                    string idregistro,
                                    string livello,
                                    string codAmm,
                                    string codliv,
                                    string r_w)
        {
            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                result = obj.insertNodoTit(padre, codice, descrizione, idregistro, livello, codAmm, codliv, r_w);
            }
            catch
            {
                result = "9";
            }
            return result;
        }

        public string DeleteNodo(string idrecord)
        {
            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                result = obj.delNodo(idrecord);
            }
            catch
            {
                result = "9";
            }
            return result;
        }

        public string getCodLiv(string codliv, string livello, string codAmm, string idTitolario, string idRegistro)
        {
            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                result = obj.getCodiceLivello(codliv, livello, codAmm, idTitolario, idRegistro);
            }
            catch
            {
                result = "";
            }
            return result;
        }

        public string spostaNodoTit(string currentCodLiv, string newCodLiv, string codAmm, string idRegistro)
        {
            string result;

            try
            {
                DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                result = obj.moveNodoTitolario(currentCodLiv, newCodLiv, codAmm, idRegistro);
            }
            catch
            {
                result = "9";
            }
            return result;
        }

        #endregion
    }
}
