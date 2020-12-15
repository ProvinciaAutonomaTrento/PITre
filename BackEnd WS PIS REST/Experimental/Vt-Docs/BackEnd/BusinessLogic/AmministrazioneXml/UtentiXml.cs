//using System;
//using System.Configuration;
//using System.Xml;
//using Debugger = DocsPaUtils.LogsManagement.Debugger;

//namespace BusinessLogic.AmministrazioneXml
//{
//    /// <summary>
//    /// Classe per la gestione dei registri di DocsPA tramite XML
//    /// </summary>
//    public class UtentiXml
//    {
//        private ErrorCode errorCode;
//        private XmlDocument parser;

//        private DocsPaVO.utente.InfoUtente infoUtente;

//        /// <summary>
//        /// Acquisisce uno stream XML
//        /// </summary>
//        /// <param name="xmlSource"></param>
//        public UtentiXml(string xmlSource)
//        {
//            errorCode = ErrorCode.NoError;

//            try
//            {
//                // Validazione file XML
//                parser = new XmlDocument();
//                parser.LoadXml(xmlSource);
//            }
//            catch (Exception exception)
//            {
//                logger.Debug("Errore durante la validazione dell'XML", exception);
//                errorCode = ErrorCode.BadXmlFile;
//            }
//        }

//        /// <summary>
//        /// </summary>
//        public UtentiXml()
//        {
//        }

//        /// <summary>Ritorna l'ultimo codice di errore</summary>
//        /// <returns></returns>
//        public ErrorCode GetErrorCode()
//        {
//            return errorCode;
//        }

//        /// <summary>
//        /// Cancella la struttura dei documenti esistenti
//        /// </summary>
//        /// <returns></returns>
//        public bool DropAll()
//        {
//            bool result = true;

//            try
//            {
//                if (errorCode == ErrorCode.NoError)
//                {
//                    DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();

//                    if (!amministrazioneXml.ClearUtenti())
//                    {
//                        throw new Exception();
//                    }

//                    amministrazioneXml.Dispose();
//                }
//            }
//            catch (Exception exception)
//            {
//                logger.Debug("Errore durante la cancellazione del titolario", exception);
//                errorCode = ErrorCode.GenericError;
//            }

//            if (errorCode != ErrorCode.NoError)
//            {
//                result = false;
//            }

//            return result;
//        }

//        /// <summary>
//        /// Caricamento dati per login
//        /// </summary>
//        /// <returns></returns>
//        private bool Login()
//        {
//            bool result = true; // Presume successo

//            try
//            {
//                UserData userData = new UserData(true);
//                GetUserData(ref userData, parser.DocumentElement.SelectSingleNode("DATI"));

//                DocsPaVO.utente.UserLogin userLogin = new DocsPaVO.utente.UserLogin(userData.utente, userData.password, userData.idAmm, "");

//                string library = DocsPaDB.Utils.Personalization.getInstance(userLogin.IdAmministrazione).getLibrary();

//                DocsPaVO.utente.Utente utente;
//                DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager(userLogin, library);
//                DocsPaVO.utente.UserLogin.LoginResult loginResult;
//                userManager.LoginUser(out utente, out loginResult);

//                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

//                this.infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
//            }
//            catch (Exception exception)
//            {
//                logger.Debug("Errore durante la login.", exception);
//                result = false;
//            }

//            return result;
//        }

//        /// <summary>
//        /// Ritorna il testo contenuto in un dato tag XML
//        /// </summary>
//        /// <param name="fieldName"></param>
//        /// <param name="node"></param>
//        /// <returns></returns>
//        private string GetXmlField(string fieldName, XmlNode node, bool nulled)
//        {
//            string result = null;

//            if (!nulled)
//            {
//                result = "";
//            }

//            XmlNode child = node.SelectSingleNode(fieldName);

//            if (child != null)
//            {
//                if (child.InnerText != "")
//                {
//                    result = child.InnerText;
//                }
//            }

//            return result;
//        }

//        /// <summary>
//        /// </summary>
//        /// <param name="userData"></param>
//        /// <param name="node"></param>
//        /// <returns></returns>
//        private bool GetUserData(ref UserData userData, XmlNode node)
//        {
//            bool result = true; // Presume successo

//            try
//            {
//                //				userData.utente		 = GetXmlField("UTENTE",	  node);
//                //				userData.password	 = GetXmlField("PASSWORD",	  node);
//                //				userData.ruolo		 = GetXmlField("RUOLO",		  node);
//                //				userData.idAmm		 = GetXmlField("IDAMM",		  node);
//                //				userData.registro	 = GetXmlField("REGISTRO",	  node);
//                //				userData.descrizione = GetXmlField("DESCRIZIONE", node);
//                //				userData.codice		 = GetXmlField("CODICE",	  node);
//            }
//            catch (Exception exception)
//            {
//                logger.Debug("Errore durante la lettura dei dati utente.", exception);
//                result = false;
//            }

//            return result;
//        }

//        /// <summary>
//        /// Crea la struttura documentale a partire dall'XML passato al costruttore
//        /// </summary>
//        /// <returns></returns>
//        public bool CreateStructure(XmlNode node, string idAmm, bool dropAll)
//        {
//            bool result = true;
//            errorCode = ErrorCode.NoError;

//            try
//            {
//                if (errorCode == ErrorCode.NoError)
//                {
//                    XmlAttribute attribute = node.Attributes["dropAll"];

//                    if (attribute != null)
//                    {
//                        dropAll |= Boolean.Parse(attribute.InnerText);
//                    }

//                    if (dropAll)
//                    {
//                        this.DropAll();
//                    }

//                    UserData userData = new UserData(false);
//                    NavigateXml(node, idAmm);
//                }
//            }
//            catch (Exception exception)
//            {
//                logger.Debug("Errore durante la creazione dell'XML", exception);
//                errorCode = ErrorCode.GenericError;
//            }

//            if (errorCode != ErrorCode.NoError)
//            {
//                result = false;
//            }

//            return result;
//        }

//        public bool UpdateStructure(XmlNode node, string idAmm)
//        {
//            bool result = true;
//            errorCode = ErrorCode.NoError;

//            try
//            {
//                if (errorCode == ErrorCode.NoError)
//                {
//                    NavigateXmlForUpdate(node, idAmm);
//                }
//            }
//            catch (Exception exception)
//            {
//                logger.Debug("Errore durante la lettura dell'XML", exception);
//                errorCode = ErrorCode.GenericError;
//            }

//            if (errorCode != ErrorCode.NoError)
//            {
//                result = false;
//            }

//            return result;
//        }

//        /// <summary>
//        /// Procedura per la lettura dell'XML
//        /// </summary>
//        /// <param name="rootNode"></param>
//        private void NavigateXml(XmlNode rootNode, string idAmm)
//        {
//            XmlNodeList nodiUtente = rootNode.SelectNodes("UTENTE");

//            // Estrazione dati e nodi sottostanti
//            foreach (XmlNode node in nodiUtente)
//            {
//                string userId = this.GetXmlField("USERID", node, false);
//                string password = this.GetXmlField("PASSWORD", node, false);
//                string nome = this.GetXmlField("NOME", node, false);
//                string cognome = this.GetXmlField("COGNOME", node, false);
//                string rubrica = this.GetXmlField("RUBRICA", node, false);
//                string registro = this.GetXmlField("REGISTRO", node, false);
//                string email = this.GetXmlField("EMAIL", node, false);
//                string smtp = this.GetXmlField("SMTP", node, false);
//                string portaSmtp = this.GetXmlField("PORTA_SMTP", node, false);
//                string amministratore = this.GetXmlField("AMMINISTRATORE", node, false);
//                string dominio = this.GetXmlField("DOMINIO", node, false);
//                string abilitato = this.GetXmlField("ABILITATO", node, false);
//                string notifica = this.GetXmlField("NOTIFICA", node, false);
//                string sede = this.GetXmlField("SEDE", node, false);
//                string email_allegata = this.GetXmlField("EMAIL_ALLEGATA", node, false);

//                userId = userId.ToUpper();

//                DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage(8, 10, "Import utente: " + userId);

//                if (notifica == null) notifica = "E";
//                if (abilitato == null) abilitato = "";
//                if (abilitato == "1")
//                {
//                    abilitato = "Y";
//                }
//                else
//                {
//                    abilitato = "N";
//                }
//                if (amministratore != "" && amministratore != null)
//                {
//                    amministratore = "1";
//                }

//                // Inserisci l'utente
//                DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager(null, null);
//                string peopleId = userManager.NewPeople(idAmm, userId, password, nome, cognome, amministratore, email, abilitato, notifica, sede, email_allegata);

//                if (peopleId != null)
//                {
//                    DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();

//                    if (!amministrazioneXml.NewUtente(peopleId, idAmm, userId, password, nome, cognome, rubrica, registro, email, smtp, portaSmtp))
//                    {
//                        throw new Exception();
//                    }

//                    if (dominio != "") dominio = dominio + @"\" + userId;
//                    amministrazioneXml.SetUserNetworkAliases(peopleId, dominio);
//                }
//            }
//        }

//        private void NavigateXmlForUpdate(XmlNode rootNode, string idAmm)
//        {
//            XmlNodeList nodiUtente = rootNode.SelectNodes("UTENTE");

//            // Estrazione dati e nodi sottostanti
//            foreach (XmlNode node in nodiUtente)
//            {

//                //legge l'attributo che definisce la modalità
//                XmlAttribute attribute = node.Attributes["MODE"];
//                string mode = "";
//                if (attribute != null)
//                {
//                    mode = attribute.InnerText.ToUpper();
//                }

//                string userId = this.GetXmlField("USERID", node, false);
//                string password = this.GetXmlField("PASSWORD", node, false);
//                string nome = this.GetXmlField("NOME", node, false);
//                string cognome = this.GetXmlField("COGNOME", node, false);
//                string rubrica = this.GetXmlField("RUBRICA", node, false);
//                string registro = this.GetXmlField("REGISTRO", node, false);
//                string email = this.GetXmlField("EMAIL", node, false);
//                string smtp = this.GetXmlField("SMTP", node, false);
//                string portaSmtp = this.GetXmlField("PORTA_SMTP", node, false);
//                string amministratore = this.GetXmlField("AMMINISTRATORE", node, false);
//                string dominio = this.GetXmlField("DOMINIO", node, false);
//                string abilitato = this.GetXmlField("ABILITATO", node, false);
//                string notifica = this.GetXmlField("NOTIFICA", node, false);
//                string sede = this.GetXmlField("SEDE", node, false);
//                string email_allegata = this.GetXmlField("EMAIL_ALLEGATA", node, false);

//                if (notifica == null) notifica = "";
//                if (abilitato == null) abilitato = "";
//                if (abilitato == "1")
//                {
//                    abilitato = "N";
//                }
//                else
//                {
//                    abilitato = "Y";
//                }

//                nome = DocsPaUtils.Functions.Functions.ReplaceApexes(nome);
//                cognome = DocsPaUtils.Functions.Functions.ReplaceApexes(cognome);
//                sede = DocsPaUtils.Functions.Functions.ReplaceApexes(sede);

//                userId = userId.ToUpper();

//                if (portaSmtp == null || portaSmtp == "") portaSmtp = "NULL";

//                if (amministratore != "" && amministratore != null)
//                {
//                    amministratore = "1";
//                }
//                if (email_allegata != "" && email_allegata != null)
//                {
//                    email_allegata = "1";
//                }

//                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
//                DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager(null, null);

//                string peopleId;
//                if (mode == "CREATED")
//                {
//                    // Crea l'utente
//                    peopleId = userManager.NewPeople(idAmm, userId, password, nome, cognome, amministratore, email, abilitato, notifica, sede, email_allegata);
//                    if (abilitato == "Y")
//                    {
//                        amministrazioneXml.DisablePeople(idAmm, userId);
//                    }
//                    if (peopleId != null)
//                    {
//                        if (!amministrazioneXml.NewUtente(peopleId, idAmm, userId, password, nome, cognome, rubrica, registro, email, smtp, portaSmtp))
//                        {
//                            throw new Exception();
//                        }
//                        //gestione del NETWORK_ALIASES
//                        if (dominio != "") dominio = dominio + @"\" + userId;
//                        amministrazioneXml.SetUserNetworkAliases(peopleId, dominio);

//                    }

//                    // gestione specifica per Filenet
//                    string documentType = ConfigurationManager.AppSettings["documentale"];
//                    if (documentType.ToUpper() == "FILENET")
//                    {
//                        string defaultFNETGroup = ConfigurationManager.AppSettings["FNET_userGroup"];
//                        bool result = userManager.AddUserFilenet(userId, password, idAmm, string.Format("{0} {1}", cognome, nome), defaultFNETGroup);
//                        if (!result)
//                            throw new Exception(string.Format("Errore durante l'inserimento dell'utente {0} in Filenet", userId));
//                    }

//                }

//                if (mode == "MODIFIED")
//                {
//                    // gestione specifica per Filenet
//                    string oldPwd = "";
//                    string documentType = ConfigurationManager.AppSettings["documentale"];
//                    if (documentType.ToUpper() == "FILENET")
//                    {
//                        DocsPaDB.Query_DocsPAWS.Utenti user = new DocsPaDB.Query_DocsPAWS.Utenti();
//                        oldPwd = user.GetPasswordUserFilenet(userId);
//                    }

//                    // Modifica l'utente
//                    if (!userManager.UpdatePeople(idAmm, userId, password, nome, cognome, amministratore, email, notifica, sede, email_allegata))
//                    {
//                        throw new Exception();
//                    }
//                    if (abilitato == "N")
//                    {
//                        amministrazioneXml.EnablePeople(idAmm, userId);
//                    }
//                    else
//                    {
//                        amministrazioneXml.DisablePeople(idAmm, userId);
//                    }
//                    if (!amministrazioneXml.UpdateUtente(idAmm, userId, password, nome, cognome, rubrica, registro, email))
//                    {
//                        throw new Exception();
//                    }
//                    peopleId = amministrazioneXml.GetUserByName(userId, idAmm);
//                    if (peopleId != null)
//                    {
//                        if (dominio != "") dominio = dominio + @"\" + userId;
//                        amministrazioneXml.SetUserNetworkAliases(peopleId, dominio);
//                    }

//                    // gestione specifica per Filenet
//                    if (documentType.ToUpper() == "FILENET")
//                    {
//                        bool result = userManager.UpdateUserFilenet(userId, oldPwd, password, string.Format("{0} {1}", cognome, nome), idAmm);
//                        if (!result)
//                            throw new Exception(string.Format("Errore durante la modifica dell'utente {0} in Filenet", userId));
//                    }
//                }

//                if (mode == "DELETED")
//                {
//                    if (!userManager.DeletePeople(idAmm, userId))
//                    {
//                        throw new Exception();
//                    }

//                    string documentType = ConfigurationManager.AppSettings["documentale"];
//                    if (documentType.ToUpper() == "FILENET")
//                    {
//                        bool result = userManager.DisableUserFilenet(userId);
//                        if (!result)
//                            throw new Exception(string.Format("Errore durante la cancellazione dell'utente {0} in Filenet", userId));
//                    }
//                }
//            }
//        }

//        public bool ExportStructure(XmlDocument doc, XmlNode amministrazione, string idAmm)
//        {
//            bool result = true; //presume successo
//            try
//            {
//                System.Data.DataSet dataSetUtenti;
//                DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
//                result = amministrazioneXml.Exp_GetUtenti(out dataSetUtenti, idAmm);
//                if (!result)
//                {
//                    throw new Exception();
//                }

//                if (dataSetUtenti != null)
//                {
//                    XmlNode utenti = amministrazione.AppendChild(doc.CreateElement("UTENTI"));

//                    foreach (System.Data.DataRow rowUtente in dataSetUtenti.Tables["UTENTI"].Rows)
//                    {
//                        XmlNode utente = utenti.AppendChild(doc.CreateElement("UTENTE"));
//                        utente.AppendChild(doc.CreateElement("USERID")).InnerText = rowUtente["USER_ID"].ToString().ToUpper();
//                        utente.AppendChild(doc.CreateElement("PASSWORD")).InnerText = rowUtente["USER_PASSWORD"].ToString();
//                        utente.AppendChild(doc.CreateElement("NOME")).InnerText = rowUtente["VAR_NOME"].ToString();
//                        utente.AppendChild(doc.CreateElement("COGNOME")).InnerText = rowUtente["VAR_COGNOME"].ToString();
//                        utente.AppendChild(doc.CreateElement("RUBRICA")).InnerText = rowUtente["VAR_COD_RUBRICA"].ToString();
//                        utente.AppendChild(doc.CreateElement("EMAIL")).InnerText = rowUtente["VAR_EMAIL"].ToString();
//                        utente.AppendChild(doc.CreateElement("AMMINISTRATORE")).InnerText = rowUtente["CHA_AMMINISTRATORE"].ToString();
//                        string dominio = amministrazioneXml.GetUserNetworkAliases(rowUtente["SYSTEM_ID"].ToString());
//                        if (dominio == null) dominio = "";
//                        if (dominio != "")
//                        {
//                            int pos = dominio.IndexOf(@"\");
//                            if (pos > 0)
//                            {
//                                dominio = dominio.Substring(0, pos);
//                            }
//                        }
//                        utente.AppendChild(doc.CreateElement("DOMINIO")).InnerText = dominio;
//                        string abilitato = "";
//                        if (rowUtente["DISABLED"] != null)
//                        {
//                            if (rowUtente["DISABLED"].ToString() == "N")
//                            {
//                                abilitato = "1";
//                            }
//                        }
//                        utente.AppendChild(doc.CreateElement("ABILITATO")).InnerText = abilitato;
//                        string notifica = "";
//                        if (rowUtente["CHA_NOTIFICA"] != null)
//                        {
//                            notifica = rowUtente["CHA_NOTIFICA"].ToString();
//                        }
//                        utente.AppendChild(doc.CreateElement("NOTIFICA")).InnerText = notifica;
//                        utente.AppendChild(doc.CreateElement("SEDE")).InnerText = rowUtente["VAR_SEDE"].ToString();
//                        utente.AppendChild(doc.CreateElement("EMAIL_ALLEGATA")).InnerText = rowUtente["CHA_NOTIFICA_CON_ALLEGATO"].ToString();
//                    }
//                }
//            }
//            catch (Exception exception)
//            {
//                logger.Debug("Errore durante l'esportazione degli utenti", exception);
//                result = false;
//            }
//            return result;
//        }

//        /// <summary>
//        /// verifica se l'utente da eliminare in amministrazione è connesso a docspa
//        /// </summary>
//        /// <param name="userId">UserId dell'utente</param>
//        /// <param name="idAmm">idAmm dell'utente</param>
//        /// <returns>bool</returns>
//        public bool CheckUserLogin(string userId, string idAmm)
//        {
//            bool result = false;
//            try
//            {
//                string idAmministrazione;
//                DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
//                DocsPaDB.Query_DocsPAWS.AmministrazioneXml adm = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
//                idAmministrazione = adm.GetAdminByName(idAmm);
//                if (idAmministrazione != null)
//                {
//                    result = ut.CheckUserLogin(userId, idAmministrazione);
//                }
//            }
//            catch (Exception exception)
//            {
//                logger.Debug("Errore nella BusinessLogic.AmministrazioneXml.CheckUserLogin: ", exception);
//            }

//            return result;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="visibilita"></param>
//        /// <param name="todolist"></param>
//        /// <param name="adl"></param>
//        /// <param name="trasmissioni"></param>
//        /// <param name="userId"></param>
//        /// <param name="idAmm"></param>
//        public void GetUserStatus(out bool visibilita, out bool todolist, out bool adl, out bool trasmissioni, string userId, string idAmm, string codRuolo)
//        {
//            string peopleId;
//            string ammId;
//            string ruoloId;
//            DocsPaDB.Query_DocsPAWS.AmministrazioneXml utenti = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();

//            visibilita = false;
//            trasmissioni = false;
//            adl = false;
//            todolist = false;

//            peopleId = utenti.GetUserByName(userId, idAmm);
//            ammId = utenti.GetAdminByName(idAmm);
//            if (codRuolo != null)
//            {
//                ruoloId = utenti.GetRuoloUOByName(codRuolo, ammId);
//            }

//            //verifica le visibilità
//            //visibilita=utenti.CheckVisibilitaUtente(peopleId);

//            //verifica la presenza di record nell'area di lavoro
//            adl = utenti.CheckADLUtente(peopleId);

//            //verifica la presenza di elementi nella todo list
//            if (codRuolo != null)
//            {
//                ruoloId = utenti.GetRuoloUOByName(codRuolo, ammId);
//                todolist = utenti.CheckTODOLISTUtente(peopleId, ruoloId);
//            }
//            else
//            {
//                //stiamo venendo dall'anagrafica utenti
//                todolist = utenti.CheckTODOLISTUtente(peopleId);
//            }

//            //verifica la presenza di trasmissioni
//            //trasmissioni=utenti.CheckTrasmissioniUtente(peopleId);

//        }

//        public bool DeleteUser(string userId, string idAmm, string[] newUser, string codRuolo)
//        {
//            bool result = false;

//            bool visibilita = false;
//            bool trasmissioni = false;
//            bool adl = false;
//            bool todolist = false;

//            string idAmministrazione;

//            DocsPaDB.Query_DocsPAWS.AmministrazioneXml amm = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
//            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

//            idAmministrazione = amm.GetAdminByName(idAmm);

//            if (idAmministrazione != null)
//            {
//                GetUserStatus(out visibilita, out todolist, out adl, out trasmissioni, userId, idAmministrazione, codRuolo);

//                //disabilita l'utente
//                amm.DisablePeople(idAmministrazione, userId);

//                //legge l'id_people dell'utente
//                string idPeople = amm.GetUserByNameAndIdAmm(userId, idAmministrazione);
//                if (idPeople != null && idPeople != "")
//                {

//                    if (adl)
//                    {
//                        //cancella l'area di lavoro dell'utente
//                        utenti.DeleteADLUtente(idPeople);
//                    }

//                    if (todolist && newUser != null)
//                    {
//                        //ciclo sulle assegnazioni per i vari ruoli
//                        string utente;
//                        string ruolo;

//                        for (int i = 0; i <= newUser.GetUpperBound(0); i++)
//                        {
//                            int p = newUser[i].IndexOf("@");
//                            if (p > 0 && p < newUser[i].Length)
//                            {
//                                //legge l'id della people nella dpa_corr_globali per il ruolo
//                                utente = amm.GetUserByNameAndIdAmm(newUser[i].Substring(0, p), idAmministrazione);
//                                //legge l'id del ruolo in UO
//                                ruolo = amm.GetRuoloUOByName(newUser[i].Substring(p + 1), idAmministrazione);
//                                if (utente != null && ruolo != null)
//                                {
//                                    //esegue l'aggiornamento della todolist
//                                    logger.Debug("Aggiorna TODO List: vecchio utente: " + idPeople + "; nuovo utente: " + utente + "ruolo: " + ruolo);
//                                    if (!amm.UpdateTodoListUtente(idPeople, utente, ruolo))
//                                        return false;
//                                }
//                            }
//                        }
//                    }
//                }

//                result = true;
//            }

//            return result;
//        }

//        public bool DeleteUserInRole(string codUtente, string codAmm, string codNewUtente, string codRuolo)
//        {
//            bool result = false;

//            DocsPaDB.Query_DocsPAWS.AmministrazioneXml amm = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
//            result = amm.DeleteUtenteInRuoloUO(codUtente, codAmm, codNewUtente, codRuolo);

//            return result;
//        }

//        public bool DeleteOrDisablePeople(string codUtente, string codAmm, bool delete)
//        {
//            bool result = false;

//            DocsPaDB.Query_DocsPAWS.AmministrazioneXml amm = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
//            result = amm.DeleteOrDisablePeople(codUtente, codAmm, delete);

//            return result;
//        }

//        public bool MoveUserFromRole(string codUtente, string codAmm, string codOldRole, string codNewRole, string codUtenteErede)
//        {
//            bool result = false;

//            DocsPaDB.Query_DocsPAWS.AmministrazioneXml amm = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
//            result = amm.MoveUserFromRole(codUtente, codAmm, codOldRole, codNewRole, codUtenteErede);

//            return result;
//        }

//    }
//}
