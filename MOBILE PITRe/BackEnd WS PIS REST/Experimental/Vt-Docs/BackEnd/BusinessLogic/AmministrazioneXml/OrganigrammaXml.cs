//using System;
//using System.Xml;
//using Debugger = DocsPaUtils.LogsManagement.Debugger;

//namespace BusinessLogic.AmministrazioneXml
//{
//    /// <summary>
//    /// Classe per la gestione dei registri di DocsPA tramite XML
//    /// </summary>
//    public class OrganigrammaXml
//    {
//        private ErrorCode errorCode;
//        private XmlDocument parser;

//        private DocsPaVO.utente.InfoUtente infoUtente;

//        /// <summary>
//        /// Acquisisce uno stream XML
//        /// </summary>
//        /// <param name="xmlSource"></param>
//        public OrganigrammaXml(string xmlSource)
//        {
//            errorCode = ErrorCode.NoError;

//            try
//            {
//                // Validazione file XML
//                parser = new XmlDocument();
//                parser.LoadXml(xmlSource);
//            }
//            catch(Exception exception)
//            {
//                logger.Debug("Errore durante la validazione dell'XML", exception);
//                errorCode = ErrorCode.BadXmlFile;
//            }
//        }

//        /// <summary>
//        /// </summary>
//        public OrganigrammaXml()
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
//                if(errorCode == ErrorCode.NoError)
//                {
//                    DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
					
//                    if(!amministrazioneXml.ClearOrganigramma())
//                    {
//                        throw new Exception();
//                    }
					
//                    amministrazioneXml.Dispose();
//                }
//            }
//            catch(Exception exception)
//            {
//                logger.Debug("Errore durante la cancellazione del titolario", exception);
//                errorCode = ErrorCode.GenericError;
//            }

//            if(errorCode != ErrorCode.NoError)
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
//            catch(Exception exception)
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

//            if(!nulled)
//            {
//                result = "";
//            }

//            XmlNode child = node.SelectSingleNode(fieldName);

//            if(child != null)
//            {
//                if(child.InnerText != "")
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
//            catch(Exception exception)
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
//                //				if(!Login())
//                //				{
//                //					throw new Exception();
//                //				}

//                if(errorCode == ErrorCode.NoError)
//                {
//                    XmlAttribute attribute = node.Attributes["dropAll"];

//                    if(attribute != null)
//                    {
//                        dropAll |= Boolean.Parse(attribute.InnerText);
//                    }

//                    if(dropAll)
//                    {
//                        this.DropAll();
//                    }
						
//                    UserData userData = new UserData(false);
//                    NavigateXml(node, idAmm, 0, "0");
//                }
//            }
//            catch(Exception exception)
//            {
//                logger.Debug("Errore durante la creazione dell'XML", exception);
//                errorCode = ErrorCode.GenericError;
//            }

//            if(errorCode != ErrorCode.NoError)
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
//                if(errorCode == ErrorCode.NoError)
//                {
//                    NavigateXmlForUpdate(node, idAmm, 0, "0");
//                }
//            }
//            catch(Exception exception)
//            {
//                logger.Debug("Errore durante la lettura dell'XML", exception);
//                errorCode = ErrorCode.GenericError;
//            }

//            if(errorCode != ErrorCode.NoError)
//            {
//                result = false;	
//            }

//            return result;
//        }

//        public bool ExportStructure(XmlDocument doc,XmlNode amministrazione,string idAmm,string parent)
//        {
//            bool result=true;
//            try
//            {

//                XmlNode organigramma=null;

//                if(parent=="0")
//                {
//                    organigramma=amministrazione.AppendChild (doc.CreateElement("ORGANIGRAMMA"));
//                }

//                //esportazione organigramma
//                System.Data.DataSet dataSetOrganigramma;
//                DocsPaDB.Query_DocsPAWS.AmministrazioneXml ammXml= new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
//                result=ammXml.Exp_GetUO(out dataSetOrganigramma,idAmm,parent);
//                if(!result)
//                {
//                    throw new Exception();
//                }

//                if(dataSetOrganigramma!= null)
//                {
//                    if(dataSetOrganigramma.Tables[0].Rows.Count > 0 )
//                    {
//                        if(parent!="0")
//                        {
//                            organigramma=amministrazione.AppendChild (doc.CreateElement("UOS"));
//                        }
//                        foreach( System.Data.DataRow rowUO in dataSetOrganigramma.Tables["UO"].Rows)
//                        {

//                            string idUO=rowUO["SYSTEM_ID"].ToString ();
//                            XmlNode UO=organigramma.AppendChild (doc.CreateElement("UO"));
//                            XmlNode datiUO=UO.AppendChild (doc.CreateElement("DATI"));
//                            datiUO.AppendChild(doc.CreateElement("CODICE")).InnerText		= rowUO["VAR_CODICE"].ToString ().ToUpper();
//                            datiUO.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText	= rowUO["VAR_DESC_CORR"].ToString ();
//                            datiUO.AppendChild(doc.CreateElement("RUBRICA")).InnerText		= rowUO["VAR_COD_RUBRICA"].ToString ().ToUpper();
//                            string cha_pa=rowUO["CHA_PA"].ToString ();
//                            string var_codice_aoo="";
//                            if(cha_pa!=null)
//                            {
//                                if(cha_pa=="1")
//                                {
//                                    var_codice_aoo=rowUO["VAR_CODICE_AOO"].ToString ();
//                                }
//                            }
//                            datiUO.AppendChild(doc.CreateElement("AOO")).InnerText = var_codice_aoo;

//                            ExportStructure(doc,UO,idAmm,rowUO["SYSTEM_ID"].ToString ());

//                            //esportazione dei ruoli della UO
//                            System.Data.DataSet dataSetRuoli;
//                            result=ammXml.Exp_GetRuoliUO(out dataSetRuoli,idAmm,idUO);
//                            if(!result)
//                            {
//                                throw new Exception();
//                            }

//                            if(dataSetRuoli!= null)
//                            {
//                                XmlNode ruoli=UO.AppendChild (doc.CreateElement("RUOLI"));
//                                foreach( System.Data.DataRow rowRuoliUO in dataSetRuoli.Tables["RuoliUO"].Rows)
//                                {
//                                    XmlNode ruolo=ruoli.AppendChild (doc.CreateElement("RUOLO"));
//                                    XmlNode datiRuolo=ruolo.AppendChild (doc.CreateElement("DATI"));
//                                    datiRuolo.AppendChild(doc.CreateElement("CODICESTRUTTURA")).InnerText	= rowRuoliUO["CODICESTRUTTURA"].ToString ();
//                                    datiRuolo.AppendChild(doc.CreateElement("CODICE")).InnerText			= rowRuoliUO["CODICE"].ToString ();
//                                    datiRuolo.AppendChild(doc.CreateElement("DESCRIZIONE")).InnerText		= rowRuoliUO["DESCRIZIONE"].ToString ();
//                                    datiRuolo.AppendChild(doc.CreateElement("RUBRICA")).InnerText			= rowRuoliUO["RUBRICA"].ToString ();
//                                    if(rowRuoliUO["RIFERIMENTO"]!=null)
//                                    {
//                                        datiRuolo.AppendChild(doc.CreateElement("RIFERIMENTO")).InnerText = rowRuoliUO["RIFERIMENTO"].ToString();
//                                    }
//                                    else
//                                    {
//                                        datiRuolo.AppendChild(doc.CreateElement("RIFERIMENTO")).InnerText = "";
//                                    }

//                                    string idRuolo=rowRuoliUO["CODICE"].ToString ();
//                                    string systemIdRuolo=rowRuoliUO["ID"].ToString ();

//                                    //esportazioni degli utenti del ruolo
//                                    System.Data.DataSet dataSetUtenti;
//                                    result=ammXml.Exp_GetUtentiRuoloUO(out dataSetUtenti,idAmm,idRuolo);
//                                    if(!result)
//                                    {
//                                        throw new Exception();
//                                    }

//                                    if(dataSetUtenti!= null)
//                                    {
//                                        XmlNode utenti=ruolo.AppendChild (doc.CreateElement("UTENTI"));
//                                        foreach( System.Data.DataRow rowUtentiUO in dataSetUtenti.Tables["UTENTIRUOLO"].Rows)
//                                        {
//                                            XmlNode utente=utenti.AppendChild(doc.CreateElement("UTENTE"));
//                                            utente.InnerText = rowUtentiUO["USER_ID"].ToString ().ToUpper();
//                                            if(rowUtentiUO["CHA_PREFERITO"]!=null)
//                                            {
//                                                if(rowUtentiUO["CHA_PREFERITO"].ToString()=="1")
//                                                {
//                                                    XmlAttribute attributo=utente.OwnerDocument.CreateAttribute("RUOLOPREFERITO");
//                                                    attributo.InnerText="True";
//                                                    utente.Attributes.Append(attributo);
//                                                }
//                                            }

//                                        }
//                                    }
									
//                                    //esportazioni dei tipi funzioni associati al ruolo
//                                    System.Data.DataSet dataSetFunzioni;
//                                    result=ammXml.Exp_GetFunzioniRuoloUO(out dataSetFunzioni,idAmm,systemIdRuolo);
//                                    if(!result)
//                                    {
//                                        throw new Exception();
//                                    }

//                                    if(dataSetFunzioni!= null)
//                                    {
//                                        XmlNode utenti=ruolo.AppendChild (doc.CreateElement("TIPIFUNZIONE"));
//                                        foreach( System.Data.DataRow rowFunzioniUO in dataSetFunzioni.Tables["FUNZIONIRUOLO"].Rows)
//                                        {
//                                            utenti.AppendChild(doc.CreateElement("CODICE")).InnerText = rowFunzioniUO["CODICE"].ToString ();
//                                        }
//                                    }

//                                    //esportazioni dei registri associati al ruolo
//                                    System.Data.DataSet dataSetRegistri;
//                                    result=ammXml.Exp_GetRegistriRuoloUO(out dataSetRegistri,idAmm,systemIdRuolo);
//                                    if(!result)
//                                    {
//                                        throw new Exception();
//                                    }

//                                    if(dataSetRegistri!= null)
//                                    {
//                                        XmlNode registri=ruolo.AppendChild (doc.CreateElement("REGISTRI"));
//                                        foreach( System.Data.DataRow rowRegistriUO in dataSetRegistri.Tables["REGISTRIRUOLO"].Rows)
//                                        {
//                                            registri.AppendChild(doc.CreateElement("CODICE")).InnerText = rowRegistriUO["CODICE"].ToString ();
//                                        }
//                                    }

//                                }
//                            }

//                        }
//                    }
//                }
//            }
//            catch(Exception exception)
//            {
//                logger.Debug("Errore durante la lettura dei dati organigramma.", exception);
//                result=false;
//            }

//            return result;
//        }


//        ///// <summary>
//        ///// Procedura per la lettura dell'XML
//        ///// </summary>
//        ///// <param name="rootNode"></param>
//        //private void NavigateXml(XmlNode rootNode, string idAmm, int livello, string idParent)
//        //{
//        //    XmlNodeList nodiUO = rootNode.SelectNodes("UO");
//        //    string idUO=null;

//        //    // Estrazione dati e nodi sottostanti
//        //    foreach(XmlNode node in nodiUO)
//        //    {
//        //        // Leggi dati
//        //        XmlNode dati = node.SelectSingleNode("DATI");

//        //        string codice	   = this.GetXmlField("CODICE", dati, false);
//        //        string descrizione = this.GetXmlField("DESCRIZIONE", dati, false);
//        //        string rubrica	   = this.GetXmlField("RUBRICA", dati, false);
//        //        string AOO	       = this.GetXmlField("AOO", dati, false);
//        //        descrizione=DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione);

//        //        DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage("Import unita' organizzativa " + codice + " dell'amministrazione: " + codice);

//        //        DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
//        //        idUO = amministrazioneXml.NewUO(idAmm, codice, descrizione, rubrica, livello, idParent, AOO);

//        //        if(idUO==null)
//        //        {
//        //            throw new Exception();
//        //        }

//        //        // Leggi ruoli
//        //        XmlNodeList ruoli = node.SelectSingleNode("RUOLI").SelectNodes("RUOLO");

//        //        // Estrazione dati e nodi sottostanti
//        //        foreach(XmlNode ruolo in ruoli)
//        //        {
//        //            // Dati
//        //            XmlNode datiRuolo = ruolo.SelectSingleNode("DATI");
//        //            string codiceStruttura = this.GetXmlField("CODICESTRUTTURA", datiRuolo, false);
//        //            codice		   = this.GetXmlField("CODICE", datiRuolo, false);					
//        //            descrizione	   = this.GetXmlField("DESCRIZIONE", datiRuolo, false);
//        //            rubrica	       = this.GetXmlField("RUBRICA", datiRuolo, false);
//        //            string riferimento    = this.GetXmlField("RIFERIMENTO", datiRuolo, false);
//        //            if(riferimento!=null)
//        //            {
//        //                if(riferimento!="1") riferimento="";
//        //            }
//        //            else
//        //            {
//        //                riferimento="";
//        //            }

//        //            DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage("Import del ruolo " + codice + " dell'unita' organizzativa " + codice);

//        //            string idGruppo;
//        //            string idRuolo;
						
//        //            try
//        //            {
//        //                DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager(null, null);
//        //                idGruppo = userManager.NewGroup(codice, descrizione);
                        
//        //                bool result = amministrazioneXml.NewRuoloUO(idAmm, codice, descrizione, codiceStruttura, rubrica,idUO, idGruppo, riferimento,out idRuolo);

//        //                if(!result || idGruppo == null || idRuolo == null)
//        //                {
//        //                    throw new Exception();
//        //                }

//        //                // Utenti
//        //                XmlNodeList utenti = ruolo.SelectSingleNode("UTENTI").SelectNodes("UTENTE");

//        //                foreach(XmlNode nodoUtente in utenti)
//        //                {
//        //                    string utente = nodoUtente.InnerText;
//        //                    DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage("Import dell'utente " + utente + " del ruolo " + codice);
//        //                    bool ruoloPreferito=false;
//        //                    XmlAttribute attribute=nodoUtente.Attributes["RUOLOPREFERITO"];
//        //                    if(attribute!=null)
//        //                    {
//        //                        if(attribute.InnerText.ToUpper()=="TRUE")
//        //                        {
//        //                            ruoloPreferito=true;
//        //                        }
//        //                    }
//        //                    string idUtente = amministrazioneXml.NewUtenteRuolo(idGruppo, utente,ruoloPreferito);

//        //                    if(idUtente == null)
//        //                    {
//        //                        throw new Exception();
//        //                    }
//        //                }

//        //                // Tipi funzione
//        //                XmlNodeList tipiFunzione = ruolo.SelectSingleNode("TIPIFUNZIONE").SelectNodes("CODICE");

//        //                foreach(XmlNode nodoFunzione in tipiFunzione)
//        //                {
//        //                    string tipoFunzione = nodoFunzione.InnerText;
//        //                    DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage("Import del tipo funzione " + tipoFunzione + " del ruolo " + codice);
						
//        //                    if(!amministrazioneXml.NewFunzioneRuolo(idRuolo, tipoFunzione))
//        //                    {
//        //                        throw new Exception();
//        //                    }
//        //                }

//        //                // Registri
//        //                XmlNodeList registri = ruolo.SelectSingleNode("REGISTRI").SelectNodes("CODICE");

//        //                foreach(XmlNode nodoRegistro in registri)
//        //                {
//        //                    string registro = nodoRegistro.InnerText;
//        //                    DocsPaUtils.LogsManagement.ImportExportLogger.getInstance("import", 0).SetMessage("Import del registro " + registro + " del ruolo " + codice);

//        //                    if(!amministrazioneXml.NewRegistroRuolo(idRuolo, registro))
//        //                    {
//        //                        throw new Exception();
//        //                    }
//        //                }
//        //            }
//        //            catch(Exception exception)
//        //            {
//        //                logger.Debug("Errore nella gestione del ruolo.", exception);
//        //            }
//        //        }

//        //        // Iterazione su ruoli
//        //        XmlNode UOS = node.SelectSingleNode("UOS");
				
//        //        if(UOS!=null)
//        //        {
//        //            this.NavigateXml(UOS, idAmm, livello + 1, idUO);
//        //        }
//        //    }
//        ////}

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        ///// <param name="rootNode"></param>
//        ///// <param name="idAmm"></param>
//        ///// <param name="livello"></param>
//        ///// <param name="idParent"></param>
//        //private void NavigateXmlForUpdate(XmlNode rootNode, string idAmm, int livello, string idParent)
//        //{
//        //    XmlNodeList nodiUO = rootNode.SelectNodes("UO");
//        //    string idUO=null;

//        //    // Estrazione dati e nodi sottostanti
//        //    foreach(XmlNode node in nodiUO)
//        //    {
//        //        DocsPaDB.Query_DocsPAWS.AmministrazioneXml amministrazioneXml = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
//        //        DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager(null, null);
//        //        bool result ;

//        //        XmlAttribute attribute=node.Attributes["MODE"];
//        //        string mode="";
//        //        if(attribute!=null)
//        //        {
//        //            mode=attribute.InnerText.ToUpper();
//        //        }

//        //        XmlNode dati = node.SelectSingleNode("DATI");

//        //        string codice	   = this.GetXmlField("CODICE", dati, false);
//        //        string descrizione = this.GetXmlField("DESCRIZIONE", dati, false);
//        //        string rubrica	   = this.GetXmlField("RUBRICA", dati, false);
//        //        string AOO	       = this.GetXmlField("AOO", dati, false);
//        //        descrizione=DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione);
//        //        codice=codice.ToUpper();
//        //        rubrica=rubrica.ToUpper();

//        //        if(mode=="CREATED")
//        //        {
//        //            // crea la UO
//        //            idUO = amministrazioneXml.NewUO(idAmm, codice, descrizione, rubrica, livello, idParent,AOO);
//        //            if(idUO == null)
//        //            {
//        //                logger.Debug("Errore nella creazione nuova UO");
//        //                throw new Exception();
//        //            }
//        //        }
//        //        else if(mode=="MODIFIED")
//        //        {
//        //            // modifica i dati della UO
//        //            idUO = amministrazioneXml.UpdateUO(idAmm, codice, descrizione, rubrica,AOO);
//        //            if(idUO == null)
//        //            {
//        //                logger.Debug("Errore nell'aggiornamento UO");
//        //                throw new Exception();
//        //            }
//        //        }
//        //        else if(mode=="DELETED")
//        //        {
					
//        //            //cancella tutte le informazioni relative al nodo UO
//        //            if(!amministrazioneXml.DeleteDatiUO(idAmm,codice))
//        //            {
//        //                logger.Debug("Errore nella cancellazione dati UO");
//        //                throw new Exception();
//        //            }
//        //        }
//        //        else
//        //        {
//        //            //non è una modifica ne una cancellazione, recupera l'id della UO e prosegue
//        //            idUO=amministrazioneXml.GetUOByName(codice,idAmm);
//        //            if(idUO==null)
//        //            {
//        //                logger.Debug("Errore nella lettura dati UO");
//        //                //logger.DebugAdm(true,"Errore nella lettura dati UO",null);
//        //                throw new Exception();
//        //            }
//        //        }

//        //        //si itera sulle informazioni solo se non si sta cancellando il nodo
//        //        if(mode!="DELETED")
//        //        {
//        //            // Leggi ruoli
//        //            XmlNode tagRuoli=node.SelectSingleNode("RUOLI");
//        //            if(tagRuoli!=null)
//        //            {
//        //                XmlNodeList ruoli = tagRuoli.SelectNodes("RUOLO");

//        //                // Estrazione dati e nodi sottostanti
//        //                foreach(XmlNode ruolo in ruoli)
//        //                {
//        //                    //legge l'attributo mode del tag ruolo
//        //                    attribute=ruolo.Attributes["MODE"];
//        //                    string roleMode="";
//        //                    if(attribute!=null)
//        //                    {
//        //                        roleMode =attribute.InnerText.ToUpper();
//        //                    }
							
//        //                    //se la UO è stata creata, creo automaticamente anche i ruoli
//        //                    if(mode=="CREATED") roleMode="CREATED";

//        //                    XmlNode datiRuolo = ruolo.SelectSingleNode("DATI");
//        //                    string codiceStruttura = this.GetXmlField("CODICESTRUTTURA", datiRuolo, false);
//        //                    codice		   = this.GetXmlField("CODICE", datiRuolo, false);					
//        //                    descrizione	   = this.GetXmlField("DESCRIZIONE", datiRuolo, false);
//        //                    descrizione    = DocsPaUtils.Functions.Functions.ReplaceApexes(descrizione);
//        //                    rubrica		   = this.GetXmlField("RUBRICA", datiRuolo, false);
//        //                    string riferimento	   = this.GetXmlField("RIFERIMENTO", datiRuolo, false);
//        //                    if(riferimento!=null)
//        //                    {
//        //                        if(riferimento!="1") riferimento="";
//        //                    }
//        //                    else
//        //                    {
//        //                        riferimento="";
//        //                    }

//        //                    codiceStruttura=codiceStruttura.ToUpper();
//        //                    codice=codice.ToUpper();
//        //                    rubrica=rubrica.ToUpper();

//        //                    if(roleMode=="MODIFIED" || roleMode=="DELETED")
//        //                    {
//        //                        //cancella tutte le informazioni relative al ruolo
//        //                        //amministrazioneXml.DeleteDatiRuoliUO(idAmm,codice);
//        //                        amministrazioneXml.DeleteUtentiFunzioniRegistriRuoliUO(idAmm,codice);
//        //                        //userManager.DeleteGroup(codice);
//        //                        /*if(roleMode=="DELETED")
//        //                        {
//        //                            amministrazioneXml.DeleteCorrispondente(idAmm,codice);
//        //                        }*/
//        //                    }

//        //                    if(roleMode=="MODIFIED" || roleMode=="CREATED")
//        //                    {
//        //                        //reinserisce le informazioni del ruolo
//        //                        string idGruppo;
//        //                        string idRuolo;
								
//        //                        if(roleMode=="CREATED")
//        //                        {
//        //                            idGruppo = userManager.NewGroup(codice, descrizione);

//        //                            result = amministrazioneXml.NewRuoloUO(idAmm, codice, descrizione, codiceStruttura, rubrica, idUO, idGruppo,riferimento, out idRuolo);

//        //                            if(!result || idGruppo == null || idRuolo == null)
//        //                            {
//        //                                logger.Debug("Errore nella gestione ruoli UO");
//        //                                throw new Exception();
//        //                            }
//        //                        }
//        //                        else //caso MODIFIED
//        //                        {
//        //                            //recupera l'id del gruppo
//        //                            idGruppo = amministrazioneXml.GetGroupByName(codice);
//        //                            result = amministrazioneXml.UpdateRuoloUO(idAmm, codice, descrizione, rubrica,riferimento, out idRuolo);									
//        //                            if(!result || idGruppo == null || idRuolo == null)
//        //                            {
//        //                                logger.Debug("Errore nella gestione ruoli UO");
//        //                                throw new Exception();
//        //                            }
//        //                        }

//        //                        // Utenti
//        //                        XmlNode tagUtenti=ruolo.SelectSingleNode("UTENTI");
//        //                        if(tagUtenti!=null)
//        //                        {
//        //                            XmlNodeList utenti = tagUtenti.SelectNodes("UTENTE");

//        //                            foreach(XmlNode nodoUtente in utenti)
//        //                            {
//        //                                string utente = nodoUtente.InnerText;
//        //                                bool ruoloPreferito=false;
//        //                                attribute=nodoUtente.Attributes["RUOLOPREFERITO"];
//        //                                if(attribute!=null)
//        //                                {
//        //                                    if(attribute.InnerText.ToUpper()=="TRUE")
//        //                                    {
//        //                                        ruoloPreferito=true;
//        //                                    }
//        //                                }

//        //                                string idUtente = amministrazioneXml.NewUtenteRuolo(idGruppo,utente,ruoloPreferito);
//        //                                if(idUtente == null)
//        //                                {
//        //                                    logger.Debug("Errore nella lettura utenti UO");
//        //                                    throw new Exception();
//        //                                }
//        //                            }
//        //                        }
//        //                        // Tipi funzione
//        //                        XmlNode tagTipiFunzione=ruolo.SelectSingleNode("TIPIFUNZIONE");
//        //                        if(tagTipiFunzione!=null)
//        //                        {
//        //                            XmlNodeList tipiFunzione = tagTipiFunzione.SelectNodes("CODICE");

//        //                            foreach(XmlNode nodoFunzione in tipiFunzione)
//        //                            {
//        //                                string tipoFunzione = nodoFunzione.InnerText;
						
//        //                                if(!amministrazioneXml.NewFunzioneRuolo(idRuolo, tipoFunzione))
//        //                                {
//        //                                    logger.Debug("Errore nella lettura funzioni ruolo UO");
//        //                                    throw new Exception();
//        //                                }
//        //                            }
//        //                        }
//        //                        // Registri
//        //                        XmlNode tagRegistri=ruolo.SelectSingleNode("REGISTRI");
//        //                        if(tagRegistri!=null)
//        //                        {
//        //                            XmlNodeList registri = tagRegistri.SelectNodes("CODICE");

//        //                            foreach(XmlNode nodoRegistro in registri)
//        //                            {
//        //                                string registro = nodoRegistro.InnerText;

//        //                                if(!amministrazioneXml.NewRegistroRuolo(idRuolo, registro))
//        //                                {
//        //                                    logger.Debug("Errore nella lettura registri ruolo");
//        //                                    throw new Exception();
//        //                                }
//        //                            }
//        //                        }
//        //                    }
//        //                }
//        //            }
//        //        }

//        //        // Iterazione
//        //        XmlNode UOS = node.SelectSingleNode("UOS");
//        //        if(UOS!=null)
//        //        {
//        //            this.NavigateXmlForUpdate(UOS, idAmm, livello + 1, idUO);
//        //        }
//        //    }
//        //}
//    }
//}
