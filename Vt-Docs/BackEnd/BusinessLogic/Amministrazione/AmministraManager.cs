using System;
using System.Collections;
using DocsPaDB;
using DocsPaVO.amministrazione;
using DocsPaVO.Validations;
using System.Text.RegularExpressions;
using System.IO;
using System.Configuration;
using BusinessLogic.Interoperabilità;
using log4net;
using System.Collections.Generic;

namespace BusinessLogic.Amministrazione
{

	/// <summary>
	/// </summary>
	public class AmministraManager
	{
        private static ILog logger = LogManager.GetLogger(typeof(AmministraManager));
		#region LOGIN AMM.RI PROFILATI



        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldPassword"></param>
        /// <returns></returns>
        public static ValidationResultInfo ChangeAdminPassword(DocsPaVO.utente.UserLogin user, string oldPassword)
        {
            DocsPaVO.Validations.ValidationResultInfo result = null;

            if (AdminPasswordConfig.IsSupportedPasswordConfig())
            {
                // Se è abilitata la gestione configurazioni password,
                // la password viene modificata seguendo i criteri di validazione impostati
                result = BusinessLogic.Utenti.Login.ChangePassword(user, oldPassword);
            }
            else
            {
                result = new ValidationResultInfo();

                DocsPaDB.Query_DocsPAWS.Amministrazione dmAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                
                if (!dmAmm.ChangeAdminPwd(user.UserName, user.Password))
                    result.BrokenRules.Add(new BrokenRule("CHANGE_PASSWORD_ERROR", string.Format("Errore nella modifica della password per l'amministratore: utente '{0}' non riconosciuto", user.UserName), BrokenRule.BrokenRuleLevelEnum.Error));

                result.Value = (result.BrokenRules.Count == 0);
            }

            return result;
        }

		/// <summary>
		/// verifica se l'utente è:
		///							-	System Admin
		///							-	Super Admin
		///							-	User Admin
		///							-	NON è collegato al sistema
		/// </summary>
		/// <param name="userid"></param>
		/// <param name="password"></param>
		/// <param name="sessionID"></param>
		/// <param name="datiAmministratore"></param>
		/// <returns>	0	=	tutto ok; 
		///				1	=	errore generico;		
		///				99	=	non riconosciuto;
		///				100	=	utente già connesso 
		///				200 =	utente presente su più amministrazioni (tranne per il SYSTEM ADMIN [tipo = 1])
		///	</returns>
        public static EsitoOperazione LoginAmministratoreProfilato(DocsPaVO.utente.UserLogin userLogin, bool forceLogin, out DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente)
		{
            EsitoOperazione result = new EsitoOperazione();
            infoUtente = null;

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
            {
                DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

                DocsPaVO.utente.UserLogin.LoginResult loginResult;
                
                // Connessione dell'utente nel documentale corrente
                if (!userManager.LoginAdminUser(userLogin, forceLogin, out infoUtente, out loginResult))
                {
                    if (loginResult == DocsPaVO.utente.UserLogin.LoginResult.UNKNOWN_USER)
                    {
                        // Utente amministratore non riconosciuto
                        result.Codice = 99;
                        result.Descrizione = string.Format("Utente {0} non riconosciuto", userLogin.UserName);
                    }
                    else if (loginResult == DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN)
                    {
                        // Utente amministratore già collegato
                        result.Codice = 100;
                        result.Descrizione = string.Format("Utente {0} già collegato", userLogin.UserName);
                    }
                    else if (loginResult == DocsPaVO.utente.UserLogin.LoginResult.DTCM_SERVICE_NO_CONTACT)
                    {
                        // Utente amministratore già collegato
                        result.Codice = 100;
                        result.Descrizione = string.Format("Errore, Servizi del Documentale DTCM non sono raggiungibili.", userLogin.UserName);
                    }
                    else
                    {
                        result.Codice = 101;
                        result.Descrizione = string.Format("Errore nella connessione dell'utente {0}", userLogin.UserName);
                    }
                }
                else
                {
                    // Completamento della transazione
                    transactionContext.Complete();
                }
            }

            return result;
		}	
			
		#endregion
		
		#region LOGIN  [...deprecated...]
        ///// <summary>
        ///// verifica se l'utente è:
        /////							-	Super-Amministratore
        /////							-	Amministratore di titolario
        /////							-	NON è collegato al sistema
        ///// </summary>
        ///// <param name="userid"></param>
        ///// <param name="password"></param>
        ///// <param name="sessionID"></param>
        ///// <returns>	0	=	tutto ok; 
        /////				1	=	errore generico;
        /////				2	=	super-admin; 
        /////				3	=	amm.re di titolario; 
        /////				99	=	non riconosciuto;
        /////				102	=	già connesso (super-admin)
        /////				103	=	già connesso (amm.re di titolario)
        /////	</returns>
        //public static EsitoOperazione LoginAmministratore(string userid, string password, string sessionID)
        //{
        //    bool amministratoreTitolario = false;
        //    string IDpeople = null;
        //    string TipoAmministratore = null;
        //    string data_conn = null;

        //    EsitoOperazione esito = new EsitoOperazione();
        //    EsitoOperazione esitoLogin = new EsitoOperazione();

        //    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_LOGIN");
        //    queryDef.setParam("param1", userid);
        //    queryDef.setParam("param2", password);

        //    string commandText = queryDef.getSQL();
        //    logger.Debug("Login amministratore:");
        //    logger.Debug("1 - ricerca tra i super-amministratori...");

        //    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
        //    {
        //        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
        //        {
        //            if (reader.Read())
        //            {
        //                IDpeople = reader.GetValue(reader.GetOrdinal("ID")).ToString();
        //                TipoAmministratore = reader.GetValue(reader.GetOrdinal("TIPO")).ToString();
        //            }
        //            else
        //            {
        //                esito.Codice = 99; // utente non riconosciuto
        //                return esito;
        //            }
        //        }

        //        switch (TipoAmministratore)
        //        {
        //            case "0":
        //                esito.Codice = 99; // utente non riconosciuto
        //                return esito;
        //            case "1":
        //                esito.Codice = 2; // utente super-amministratore						
        //                break;
        //            case "2":
        //                esito.Codice = 3; // utente amministratore di titolario
        //                amministratoreTitolario = true;
        //                break;
        //        }

        //        // verifica se non è già collegato...
        //        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_USER_LOGIN_AMM");
        //        queryDef.setParam("param1", userid);
        //        commandText = queryDef.getSQL();
        //        logger.Debug("3 - verifica se l'amministratore è già collegato...");
        //        dbProvider.ExecuteScalar(out data_conn, commandText);
        //    }

        //    if (data_conn != null)
        //    {
        //        // utente amministratore già collegato dal giorno...
        //        esito.Descrizione = data_conn;

        //        if (!amministratoreTitolario)
        //            esito.Codice = 102; // di tipo super-amministratore
        //        else
        //            esito.Codice = 103; // di tipo amm.re di titolario
        //    }
        //    else
        //    {
        //        esitoLogin = LoginNewSessionAmministratore(userid, sessionID);
        //        if (!esitoLogin.Codice.Equals(0))
        //            esito.Codice = 1; // errore!
        //    }

        //    return esito;
        //}

        //public static EsitoOperazione UpdLoginAmm(string userid, string sessionID) 
        //{
        //    EsitoOperazione esito = new EsitoOperazione();			

        //    esito = DeleteLoginAmministrazione(userid); //prima elimina...
        //    if(esito.Codice.Equals(0))
        //        esito = LoginNewSessionAmministratore(userid,sessionID); //poi inserisce
            
        //    return esito;
        //}

        //public static EsitoOperazione LoginNewSessionAmministratore(string userid, string sessionID) 
        //{
        //    EsitoOperazione esito = new EsitoOperazione();

        //    int rowsAffected;

        //    // aggiunge nuovo record in dpa_login
        //    DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("LOCK_USER_LOGIN");			
        //    queryDef.setParam("param1",userid);
        //    queryDef.setParam("param2","NULL");
        //    queryDef.setParam("param3","ADMINISTRATOR");
        //    queryDef.setParam("param4",DocsPaDbManagement.Functions.Functions.GetDate(true));
        //    queryDef.setParam("param5",sessionID);					
        //    string commandText=queryDef.getSQL();
			
        //    DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
        //    dbProvider.ExecuteNonQuery(commandText,out rowsAffected);
            
        //    if (rowsAffected == 0)
        //    {
        //        esito.Codice = 1;
        //        esito.Descrizione = "Errore nella login dell'utente amministratore";
        //    }

        //    return esito;
        //}

		public static EsitoOperazione UpdateLoginAmministrazione(string userid, string sessionID)
		{
			EsitoOperazione esito = new EsitoOperazione();

			int rowsAffected;

			// modifica record in dpa_login
			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("U_LOGIN_AMM");						
			queryDef.setParam("param1",DocsPaDbManagement.Functions.Functions.GetDate(true));
			queryDef.setParam("param2",sessionID);	
			queryDef.setParam("param3",userid);	
			string commandText=queryDef.getSQL();
			
			DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();
			dbProvider.ExecuteNonQuery(commandText,out rowsAffected);
				if(rowsAffected==0)
					esito.Codice = 1;				

			return esito;
		}

        //public static EsitoOperazione DeleteLoginAmministrazione(string userid)
        //{
        //    EsitoOperazione esito = new EsitoOperazione();

        //    // elimina record in dpa_login
        //    DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("D_LOGIN_AMM");									
        //    queryDef.setParam("param1",userid);	
        //    string commandText=queryDef.getSQL();
			
        //    DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider();			
        //    if(!dbProvider.ExecuteNonQuery(commandText))				
        //            esito.Codice = 1;				

        //    return esito;
        //}

        public bool CheckSessionID(string sessionID)
        {
            bool retValue = false;

            string valore = null;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_ADMIN_USER_LOGIN_ON_SESSION_ID");
            queryDef.setParam("param1", sessionID);
            string commandText = queryDef.getSQL();

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                dbProvider.ExecuteScalar(out valore, commandText);
                if (valore != null && !valore.Equals("0"))
                    retValue = true;

            }
            return retValue;
        }

        /// <summary>
        /// Logout dell'utente amministratore
        /// </summary>
        /// <param name="adminUser"></param>
        /// <returns></returns>
        public bool LogoutAmministratore(DocsPaVO.amministrazione.InfoUtenteAmministratore adminUser)
		{
            bool retValue = false;

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
            {
                DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

                retValue = userManager.LogoutUser(adminUser.dst);

                if (retValue)
                    transactionContext.Complete();
            }

            return retValue;
		}

		#endregion

		#region Amministrazione

		public static ArrayList GetAmministrazioni()
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
			ArrayList result = amm.GetDatiAmministrazione();			
			return result;
		}

        //MEV utente - Multi-Amministrazione
        public static ArrayList GetAmministrazioniByUser(string userId, bool controllo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            ArrayList result = amm.GetDatiAmministrazioneByUser(userId, controllo);
            return result;
        }

        public static ArrayList GetDatiAmministrazioneByUserAdministrator(string userId, bool controllo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            ArrayList result = amm.GetDatiAmministrazioneByUserAdministrator(userId, controllo);
            return result;
        }

        public static string GetPasswordUtenteMultiAmm(string userId)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            string result = amm.GetPasswordUtenteMultiAmm(userId);
            return result;
        }

        public static bool SetPasswordUtenteMultiAmm(string userId, string password)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            bool result = amm.SetPasswordUtenteMultiAmm(userId, password);
            return result;
        }

        public static bool ModificaPasswordUtenteMultiAmm(string userId, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            bool result = amm.ModificaPasswordUtenteMultiAmm(userId, idAmm);
            return result;
        }

        public static DocsPaVO.documento.TipologiaAtto InsertTipoAtto(DocsPaVO.documento.TipologiaAtto tipoAtto, DocsPaVO.utente.InfoUtente infoUtente)
		{
			DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

			if(! amm.InsertTipoAtto(ref tipoAtto))
			{
				throw new Exception();
			}

			return tipoAtto;
		}

        public static bool setNews(string idAmm, string news, bool enable_news)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            bool result = amm.setNews(idAmm, news, enable_news);
            return result;
        }

        public static string getNews(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            string result = amm.getNews(idAmm);
            return result;
        }

        public static bool setBanner(string idAmm, string banner)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            bool result = amm.setBanner(idAmm, banner);
            return result;
        }

        public static string getBanner(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            string result = amm.getBanner(idAmm);
            return result;
        }

       ///// <summary>
        ///// Metodo che restituisce un oggetto info timbro opportunamente valorizzato con i dati letti dal DB
        ///// </summary>
        ///// <returns></returns>
        //public static DocsPaVO.amministrazione.InfoTimbro datiTimbro()
        //{
        //    InfoTimbro timbro = new InfoTimbro();
        //    DocsPaUtils.Query queryDef1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_AMMINISTRA3_TIMBRO");
        //    string fields_timbro = "A.SYSTEM_ID AS ID," +
        //                           "A.VAR_NOME AS CARATTERE," +
        //                           "A.DIMENSIONE AS DIMENSIONE";
        //    queryDef1.setParam("param1", fields_timbro);
        //    fields_timbro = "FROM DPA_CARAT_TIMBRO A ORDER BY ID";
        //    queryDef1.setParam("param2", fields_timbro);
        //    string commandText = queryDef1.getSQL();
        //    logger.Debug(commandText);
        //    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
        //    {
        //        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
        //        {
        //            while (reader.Read())
        //            {
        //                DocsPaVO.amministrazione.carattere carat = new carattere();
        //                carat.id = reader.GetValue(reader.GetOrdinal("ID")).ToString();
        //                carat.caratName = reader.GetValue(reader.GetOrdinal("CARATTERE")).ToString();
        //                carat.dimensione = reader.GetValue(reader.GetOrdinal("DIMENSIONE")).ToString();
        //                timbro.carattere.Add(carat);
        //            }
        //        }
        //    }
        //    DocsPaUtils.Query queryDef2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_AMMINISTRA3_TIMBRO");
        //    fields_timbro = "C.SYSTEM_ID AS ID," +
        //                    "C.VAR_NOME AS COLORE," +
        //                    "C.DESCRIZIONE AS DESCRIZIONE";
        //    queryDef2.setParam("param1", fields_timbro);
        //    fields_timbro = "FROM DPA_COLORE_TIMBRO C ORDER BY ID";
        //    queryDef2.setParam("param2", fields_timbro);
        //    commandText = queryDef2.getSQL();
        //    logger.Debug(commandText);
        //    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
        //    {
        //        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
        //        {
        //            while (reader.Read())
        //            {
        //                DocsPaVO.amministrazione.color colore = new color();
        //                colore.id = reader.GetValue(reader.GetOrdinal("ID")).ToString();
        //                colore.colName = reader.GetValue(reader.GetOrdinal("COLORE")).ToString();
        //                colore.descrizione = reader.GetValue(reader.GetOrdinal("DESCRIZIONE")).ToString();
        //                timbro.color.Add(colore);
        //            }
        //        }
        //    }
        //    DocsPaUtils.Query queryDef3 = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_AMMINISTRA3_TIMBRO");
        //    fields_timbro = "P.SYSTEM_ID AS ID," +
        //                    "P.TIPO_POS AS POSIZIONE," +
        //                    "P.POS_X AS POSX," +
        //                    "P.POS_Y AS POSY";
        //    queryDef3.setParam("param1", fields_timbro);
        //    fields_timbro = "FROM DPA_POSIZ_TIMBRO P ORDER BY ID";
        //    queryDef3.setParam("param2", fields_timbro);
        //    commandText = queryDef3.getSQL();
        //    logger.Debug(commandText);
        //    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
        //    {
        //        using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
        //        {
        //            while (reader.Read())
        //            {
        //                DocsPaVO.amministrazione.posizione position = new posizione();
        //                position.id = reader.GetValue(reader.GetOrdinal("ID")).ToString();
        //                position.posName = reader.GetValue(reader.GetOrdinal("POSIZIONE")).ToString();
        //                position.PosX = reader.GetValue(reader.GetOrdinal("POSX")).ToString();
        //                position.PosY = reader.GetValue(reader.GetOrdinal("POSY")).ToString();
        //                timbro.positions.Add(position);
        //            }
        //        }
        //    }
        //    return timbro;
        //}

		/// <summary>
		/// Lista di tutte le Amministrazioni disponibili sul DB
		/// </summary>
		/// <returns></returns>
		public static InfoAmministrazione[] AmmGetListAmministrazioni()
		{
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaVO.amministrazione.InfoAmministrazione[] infoAmm = null;
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    infoAmm = amm.AmmGetListAmministrazioni();

                    // Completamento transazione
                    transactionContext.Complete();
                }
                catch (Exception ex)
                {
                    logger.Debug(string.Format("Errore AmmGetListAmministrazioni : ", ex.Message));
                }
                return infoAmm;
            }
		}
		
		public static DocsPaVO.amministrazione.InfoAmministrazione AmmGetInfoAmmCorrente(string idAmm)
		{
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaVO.amministrazione.InfoAmministrazione infoAmm = null;
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    infoAmm = amm.AmmGetInfoAmmCorrente(idAmm);
                    
                    // Completamento transazione
                    transactionContext.Complete();
                }
                catch (Exception ex)
                {
                    logger.Debug(string.Format("Errore AmmGetInfoAmmCorrente : ", ex.Message));
                }
                return infoAmm;
            }            
		}

        public static bool insertFile(DocsPaVO.utente.InfoUtente infoUtente, byte[] dati, string nomeFile, string serverPath)
        {
            bool result = true;
            
            try
            {
                //Controllo se esiste la Directory nel path dove vengono salvate le immagini modificabili.
                //Se esiste copio il file excel li' dentro, altrimenti la creo e ci copio il file.
                //In ogni caso poichè il nome del file è fisso, anche se quest'ultimo esiste viene sovrascritto.
                logger.Debug("Metodo \"insertFile\" classe \"AmministraManager\" : inizio salvataggio file " + nomeFile);
                if (Directory.Exists(serverPath))
                {
                    FileStream fs1 = new FileStream(serverPath + "\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs1.Write(dati, 0, dati.Length);
                    fs1.Close();
                }
                else
                {
                    Directory.CreateDirectory(serverPath);
                    FileStream fs1 = new FileStream(serverPath + "\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs1.Write(dati, 0, dati.Length);
                    fs1.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Metodo \"insertFile\" classe \"AmministraManager\" ERRORE : " + ex.Message);
                result = false;
                return result;
            }

            return result;
        }

        public static bool existFile(string nomeFile, string serverPath)
        {
            bool result = true;

            try
            {
                serverPath = serverPath + "\\" + nomeFile;
                logger.Debug("Metodo \"existFile\" classe \"AmministraManager\" : inizio verifica esistenza file " + nomeFile);
                return File.Exists(serverPath);
            }
            catch (Exception ex)
            {
                logger.Debug("Metodo \"existFile\" classe \"AmministraManager\" ERRORE : " + ex.Message);
                return false;
            }
        }

        public static string findFontColor(string idAmm)
        {
            string result = "";

            try
            {
                logger.Debug("Metodo \"findFontColor\" classe \"AmministraManager\" : inizio verifica esistenza colore del testo per l'amministrazione " + idAmm);
                //return File.Exists(serverPath);
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_FONT_COLOR");
                queryDef.setParam("param1", idAmm);
                string commandText = queryDef.getSQL();

                //string commandText = "SELECT FONTCOLOR FROM DPA_AMMINISTRA WHERE SYSTEM_ID = " + idAmm;
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            result = reader.GetValue(reader.GetOrdinal("FONTCOLOR")).ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Metodo \"findFontColor\" classe \"AmministraManager\" ERRORE : " + ex.Message);
                result = "";
            }
            return result;
        }

        public static string findPulsColor(string idAmm)
        {
            string result = "";

            try
            {
                logger.Debug("Metodo \"findPulsColor\" classe \"AmministraManager\" : inizio verifica esistenza colore della pulsantiera per l'amministrazione " + idAmm);
                //DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_PULS_COLOR");
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PULS_COLOR");
                //queryDef.setParam("param1", idAmm);
                //string commandText = queryDef.getSQL();
                string commandText = "SELECT PULS_COLOR FROM DPA_AMMINISTRA WHERE SYSTEM_ID = " + idAmm;

                logger.Debug(commandText);

                DocsPaDB.DBProvider dbProv = new DBProvider();

                string res;
                if (dbProv.ExecuteScalar(out res, commandText))
                    result = res;
                else
                    result = "";

                //using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                //{
                //    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                //    {
                //        while (reader.Read())
                //        {
                //            int uuu = reader.GetOrdinal("PULS_COLOR");
                //            result = reader.GetValue(reader.GetOrdinal("PULS_COLOR")).ToString();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                logger.Debug("Metodo \"findPulsColor\" classe \"AmministraManager\" ERRORE : " + ex.Message);
                result = "";
            }
            return result;
        }

        public static string findRecords(string idAmm)
        {
            string result = "";

            try
            {
                logger.Debug("Metodo \"findRecords\" classe \"AmministraManager\" : inizio ricerca del limite dei record per l'archiviazione nello storico dell'amministrazione " + idAmm);
                //return File.Exists(serverPath);
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_NUM_RECORD");
                queryDef.setParam("param1", idAmm);
                string commandText = queryDef.getSQL();

                //string commandText = "SELECT NUM_RECORD, CHA_ARCHIVIAZIONE_LOG FROM DPA_AMMINISTRA WHERE SYSTEM_ID = " + idAmm;
                logger.Debug(commandText);

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            result += reader.GetValue(reader.GetOrdinal("CHA_ARCHIVIAZIONE_LOG")).ToString() + "^" + reader.GetValue(reader.GetOrdinal("NUM_RECORD")).ToString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Debug("Metodo \"findRecord\" classe \"AmministraManager\" ERRORE : " + ex.Message);
                result = null;
            }
            return result;
        }

        public static bool modifyRecordNumber(string idAmm, char chk, int num)
        {
            bool result = true;

            try
            {
                logger.Debug("Metodo \"modifyRecordNumber\" classe \"AmministraManager\" : inizio ricerca del limite dei record per l'archiviazione nello storico dell'amministrazione " + idAmm);
                //return File.Exists(serverPath);
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_NUM_RECORD");
                queryDef.setParam("param1", idAmm);
                queryDef.setParam("param2", chk.ToString());
                queryDef.setParam("param3", num.ToString());
                string commandText = queryDef.getSQL();

                //string commandText = "UPDATE DPA_AMMINISTRA SET CHA_ARCHIVIAZIONE_LOG = '" + chk.ToString() + "', NUM_RECORD = " + num + " WHERE SYSTEM_ID = " + idAmm;
                logger.Debug(commandText);

                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                result = dbProvider.ExecuteNonQuery(commandText);
            }
            catch (Exception ex)
            {
                logger.Debug("Metodo \"modifyRecordNumber\" classe \"AmministraManager\" ERRORE : " + ex.Message);
                result = false;
            }
            return result;
        }

        public static bool changeColor(string idAmm, string color)
        {
            bool result = false;

            try
            {
                logger.Debug("Metodo \"changeColor\" classe \"AmministraManager\" : inizio modifica colore del testo per l'amministrazione " + idAmm);
                //return File.Exists(serverPath);
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_FONT_COLOR");
                queryDef.setParam("param1", idAmm);
                queryDef.setParam("param2", color);
                string commandText = queryDef.getSQL();

                //string commandText = "UPDATE DPA_AMMINISTRA SET FONTCOLOR = '" + color + "' WHERE SYSTEM_ID = " + idAmm;
                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                result = dbProvider.ExecuteNonQuery(commandText);

            }
            catch (Exception ex)
            {
                logger.Debug("Metodo \"changeColor\" classe \"AmministraManager\" ERRORE : " + ex.Message);
                result = false;
            }
            return result;
        }

        public static bool changePulsColor(string idAmm, string color)
        {
            bool result = false;

            try
            {
                logger.Debug("Metodo \"changePulsColor\" classe \"AmministraManager\" : inizio modifica colore della pulsantiera per l'amministrazione " + idAmm);
                //return File.Exists(serverPath);
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_PULS_COLOR");
                queryDef.setParam("param1", idAmm);
                queryDef.setParam("param2", color);
                string commandText = queryDef.getSQL();

                DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();
                result = dbProvider.ExecuteNonQuery(commandText);

            }
            catch (Exception ex)
            {
                logger.Debug("Metodo \"changePulsColor\" classe \"AmministraManager\" ERRORE : " + ex.Message);
                result = false;
            }
            return result;
        }

        public static DocsPaVO.amministrazione.InfoAmministrazione GetInfoAmmAppartenenzaUtente(string userid, string pwd)
		{
            InfoAmministrazione amm = null;
            
			DocsPaUtils.Query queryDef=DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_APPARTENENZA_UTENTE");			
			queryDef.setParam("param1",userid);

			string commandText=queryDef.getSQL();
			
			using (DocsPaDB.DBProvider dbProvider=new DocsPaDB.DBProvider())
			{
				using (System.Data.IDataReader reader=dbProvider.ExecuteReader(commandText))
				{
					while (reader.Read())
					{
                        amm = new InfoAmministrazione();

						amm.IDAmm = reader.GetValue(reader.GetOrdinal("ID")).ToString();
						amm.Codice = reader.GetValue(reader.GetOrdinal("CODICE")).ToString();
						amm.Descrizione = reader.GetValue(reader.GetOrdinal("DESCR")).ToString();							
					}
				}	
			}
			return amm;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoAmministrazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
		public static EsitoOperazione InsertAmministrazione(InfoAmministrazione infoAmministrazione, InfoUtenteAmministratore infoUtente)
		{
            EsitoOperazione retValue = null;

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
            {
                DocsPaDocumentale.Interfaces.IAmministrazioneManager ammMng = new DocsPaDocumentale.Documentale.AmministrazioneManager(infoUtente);

                retValue = ammMng.Insert(infoAmministrazione);

                if (retValue.Codice == 0)
                {
                    if (FormatiDocumento.Configurations.SupportedFileTypesEnabled)
                    {
                        // Inserimento dei formati documento predefiniti
                        ValidationResultInfo validationResult = FormatiDocumento.SupportedFormatsManager.InitializeDefaultFileTypes(Convert.ToInt32(infoAmministrazione.IDAmm));

                        if (!validationResult.Value)
                        {
                            retValue.Codice = -1;
                            retValue.Descrizione = ((BrokenRule)validationResult.BrokenRules[0]).Description;
                        }
                    }
   
                    transactionContext.Complete();
                }
            }

            return retValue;
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoAmministrazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static EsitoOperazione UpdateAmministrazione(InfoAmministrazione infoAmministrazione, InfoUtenteAmministratore infoUtente)
        {
             EsitoOperazione retValue = null;

             // Creazione contesto transazionale
             using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
             {
                 DocsPaDocumentale.Interfaces.IAmministrazioneManager ammMng = new DocsPaDocumentale.Documentale.AmministrazioneManager(infoUtente);

                 retValue = ammMng.Update(infoAmministrazione);

                 if (retValue.Codice == 0)
                     transactionContext.Complete();
             }

             return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoAmministrazione"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static EsitoOperazione DeleteAmministrazione(InfoAmministrazione infoAmministrazione, InfoUtenteAmministratore infoUtente)
        {
            EsitoOperazione retValue = null;

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new TransactionContext())
            {
                DocsPaDocumentale.Interfaces.IAmministrazioneManager ammMng = new DocsPaDocumentale.Documentale.AmministrazioneManager(infoUtente);

                retValue = ammMng.Delete(infoAmministrazione);

                if (retValue.Codice == 0)
                {
                    if (FormatiDocumento.Configurations.SupportedFileTypesEnabled)
                    {
                        // Rimozione dei formati documento predefiniti
                        ValidationResultInfo validationResult = FormatiDocumento.SupportedFormatsManager.ClearFileTypes(Convert.ToInt32(infoAmministrazione.IDAmm));

                        if (!validationResult.Value)
                        {
                            retValue.Codice = -1;
                            retValue.Descrizione = ((BrokenRule)validationResult.BrokenRules[0]).Description;
                        }
                    }

                    transactionContext.Complete();
                }
            }

            return retValue;
        }

        public ArrayList getListaTemi()
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            ArrayList result = amm.GetListaTemi();
            return result;
        }

        public bool setTemaAmministrazione(string idAmm, int valore)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                result = amm.setTemaAmministrazione(idAmm, valore);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Amministra Manager - metodo setTemaAmministrazione " + e);
            }
            return result;
        }

        public bool setColoreSegnatura(string idAmm, int valore)
        {
            bool result = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                result = amm.setColoreSegnatura(idAmm, valore);
            }
            catch (Exception e)
            {
                logger.Debug("Errore in Amministra Manager - metodo setColoreSegnatura " + e);
            }
            return result;
        }

        #endregion

        public static string getIdAmmCorrGlobali(string idAmm)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    string result = amm.getIdAmmCorrGlobali(idAmm);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in AmministraManager.cs  - metodo: getIdAmmCorrGlobali", e);
                    return null;
                }
            }
        }

        public static Disservizio getDisservizio()
        {
            Disservizio disservizio = new Disservizio();
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    disservizio = amm.getDisservizio();
                    transactionContext.Complete();
                    return disservizio;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in AmministraManager.cs  - metodo: getDisservizio", e);
                    return disservizio;
                }

            }
        }

        public static bool creaDisservizio(Disservizio disservizio)
        {
            bool result = true;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    amm.creaDisservizio(disservizio);
                    transactionContext.Complete();
                    return result;
                }
                catch(Exception e)
                {
                    logger.Debug("Errore in AmministraManager.cs  - metodo: creaDisservizio", e);
                    result= false;
                    return result;
                }
            }
        }

        public static bool updateDisservizio(Disservizio disservizio)
        {
            bool result = true;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    amm.updateDisservizio(disservizio);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in AmministraManager.cs  - metodo: updateDisservizio", e);
                    result = false;
                    return result;
                }
            }
        }

        public static bool cambiaStatoDisservizio(string stato, string system_id)
        {
            bool result = true;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    amm.cambiaStatoDisservizio(stato, system_id);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in AmministraManager.cs  - metodo: cambiaStatoDisservizio", e);
                    result = false;
                    return result;
                }
            }
        }

        public static bool deleteDisservizio(string system_id)
        {
            bool result = true;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    amm.deleteDisservizio(system_id);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in AmministraManager.cs  - metodo: deleteDisservizio", e);
                    result = false;
                    return result;
                }
            }

        }

        public static bool setStatoAccettazioneDisservizio(string stato)
        {
            bool result = true;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    amm.setStatoAccettazioneDisservizio(stato);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in AmministraManager.cs  - metodo: setStatoAccettazioneDisservizio", e);
                    result = false;
                    return result;
                }
            }
        }
        

        public static bool setStatoNotificaDisservizio(string systemId, string stato)
        {
            bool result = true;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    amm.setNotificaDisservizio(systemId, stato);
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in AmministraManager.cs  - metodo: setStatoNotificaDisservizio", e);
                    result = false;
                    return result;
                }
            }

        }

        public static bool InviaNotificaEmailDisservizio(string idAmm, string mailAmm, string mailUtente, string bodyMail, string subject)
        {
            bool ret = true;
            System.Data.DataSet ds;
            SvrPosta svr = null;
            try
            {
                //si costruisce il corpo della mail
                logger.Debug("Costruzione mail");
                //string subject = "Notifica Disservizio";
                DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
                obj.getSmtp(out ds, idAmm);

                if (ds.Tables["SERVER"].Rows.Count == 0)
                {

                    return ret;
                }

                //				string server = ds.Tables["SERVER"].Rows[0]["VAR_SMTP"].ToString();
                //				string smtp_user = ds.Tables["SERVER"].Rows[0]["VAR_USER_SMTP"].ToString();
                //				string smtp_pwd = ds.Tables["SERVER"].Rows[0]["VAR_PWD_SMTP"].ToString();
                string smtp_user = "";
                string smtp_pwd = "";
                string server = ds.Tables["SERVER"].Rows[0]["VAR_SMTP"].ToString();
                if (!ds.Tables["SERVER"].Rows[0]["VAR_USER_SMTP"].Equals(System.DBNull.Value))
                    smtp_user = ds.Tables["SERVER"].Rows[0]["VAR_USER_SMTP"].ToString();
                if (!ds.Tables["SERVER"].Rows[0]["VAR_PWD_SMTP"].Equals(System.DBNull.Value))
                    smtp_pwd = DocsPaUtils.Security.Crypter.Decode(ds.Tables["SERVER"].Rows[0]["VAR_PWD_SMTP"].ToString(), smtp_user);

                string port = ds.Tables["SERVER"].Rows[0]["NUM_PORTA_SMTP"].ToString();
                string SmtpSsl = ds.Tables["SERVER"].Rows[0]["CHA_SMTP_SSL"].ToString();
                string SmtpSTA = ds.Tables["SERVER"].Rows[0]["CHA_SMTP_STA"].ToString();

                svr = new SvrPosta(server,
                            smtp_user,
                            smtp_pwd,
                            port,
                            Path.GetTempPath(),
                          CMClientType.SMTP, SmtpSsl, SmtpSTA);

                if (mailUtente != "")
                {
                    bool res = Notifica.notificaMailDisservizio(svr, mailUtente, mailAmm, subject, bodyMail);
                }
               

                return true;
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella creazione della mail di notifica di disservizio (InviaNotificaDisservizio)", e);
                return false;
            }
        }

        public static bool accettaDisservizio(string systemId)
        {
            bool result = true;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    //A = esiste diss e accett ; N = non accettato ; F = non esiste disservizio
                    amm.setStatoAccettazioneDisservizioUtente(systemId, "A");
                    transactionContext.Complete();
                    return result;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in AmministraManager.cs  - metodo: accettaDisservizio", e);
                    result = false;
                    return result;
                }
            }
        }

        public static string getStatoAccettazioneUtente(string systemId)
        {
            string stato = string.Empty;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    //A = esiste diss e accett ; N = non accettato ; F = non esiste disservizio
                    stato = amm.getStatoAccettazioneUtente(systemId);
                    transactionContext.Complete();
                    return stato;
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in AmministraManager.cs  - metodo: getStatoAccettazioneUtente", e);

                    return stato;
                }
            }

        }
        
        /// <summary>
        /// Restituisce la lista dei dispositivi di stampa etichetta censiti nel sistema
        /// </summary>
        /// <returns></returns>
        public static List<DocsPaVO.amministrazione.DispositivoStampaEtichetta> GetDispositiviStampaEtichetta()
        {
            using (DocsPaDB.Query_DocsPAWS.Amministrazione amministrazioneDb = new DocsPaDB.Query_DocsPAWS.Amministrazione())
            {
                return amministrazioneDb.GetDispositiviStampaEtichetta();
            }
        }

        /// <summary>
        /// Restituisce il nome dell'applicazione dalla tabella del DB
        /// </summary>
        /// <returns></returns>
        public static string getApplicationName()
        {
            string retValue = string.Empty;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    retValue = amm.getApplicationName();
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore nel metodo getApplicationName");
                }
            }
            return retValue;
        }


        public static string GestioneMessaggioLogin(string message, string password )
        {
            string retVal = "";
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

                if (!string.IsNullOrEmpty(password) && password == "UpMsgLogin20XX")
                {
                    if (amm.updLoginMessage(message)) retVal = message;
                    else throw new Exception("errore");
                }
                else
                {
                    retVal = amm.getLoginMessage();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in GestioneMessaggioLogin");
            }
            return retVal;
        }


        public static bool UpdateMessaggioLogin(string message)
        {
            bool retVal = false;
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                if (amm.updLoginMessage(message))
                    retVal = true;
                else 
                    throw new Exception("errore");
            }
            catch (Exception ex)
            {
                logger.Error("Errore in GestioneMessaggioLogin");
            }
            return retVal;
        }

        /// <summary>
        /// Ritorna se l'amministrazione ha come parametro obbligatorio la scelta del registro nella creazione dei modelli di trasmissione
        /// </summary>
        /// <returns></returns>
        public static string getSeRegistroObbl(string idAmm, string nome)
        {
            string retValue = string.Empty;
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    retValue = amm.getSeRegistroObbl(idAmm, nome);
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    logger.Debug("Errore nel metodo getApplicationName");
                }
            }
            return retValue;
        }

        public static DocsPaVO.amministrazione.VisualizzaStatoDoc GetInfoDocument(string docnumber, string numProto, string anno, string idAmm, string codiceRegistro)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.GetInfoDocument(docnumber, numProto, anno, idAmm, codiceRegistro);
        }

        public static List<DocsPaVO.documento.FascicolazioneTipiDocumento> GetFascicolazioneTipiDocumento(string idAmm, DocsPaVO.utente.InfoUtente infoutente)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.GetFascicolazioneTipiDocumento(idAmm, infoutente);
        }

        public static bool SetFascicolazioneTipiDocumento(List<DocsPaVO.documento.FascicolazioneTipiDocumento> listTipiDoc, string idAmm, DocsPaVO.utente.InfoUtente infoutente)
        {
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            return doc.SetFascicolazioneTipiDocumento(listTipiDoc, idAmm, infoutente);
        }

        public static bool IsEnabledTrasmissioneAutomaticaAmm(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.IsEnabledTrasmissioneAutomaticaAmm(idAmm);
        }

        public static InfoUtenteAmministratore GetDatiAmministratore(string userId, string idAmm)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                return amm.GetDatiAmministratore(userId, idAmm);
            }
            catch (Exception e)
            {
                logger.Error("Errore nel reperimento dei dati amministrazione");
                return null;
            }
        }

        public static bool SbloccoCasella(string email, string idRegistro)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                return amm.SbloccoCasella(email, idRegistro);
            }
            catch (Exception e)
            {
                logger.Error("Errore in SbloccoCasella " + e.Message);
                return false;
            }
        }

        #region Trasmissioni pendenti Utenti in Ruolo
        public static bool ExistsTrasmissioniPendentiConWorkflowUtente(string idRuoloInUO, string idPeople)
        {
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                return amm.ExistsTrasmissioniPendentiConWorkflowUtente(idRuoloInUO, idPeople);
            }
            catch (Exception e)
            {
                logger.Error("Errore in ExistsTrasmissioniPendentiConWorkflowUtente " + e.Message);
                return false;
            }
        }

        public static DocsPaVO.documento.FileDocumento GetReportTrasmissioniPendentiUtente(string idPeople, string idCorrGlobalRuolo, string formato)
        {
            DocsPaVO.documento.FileDocumento report = new DocsPaVO.documento.FileDocumento();
            try
            {

                // filtri report
                List<DocsPaVO.filtri.FiltroRicerca> filters = new List<DocsPaVO.filtri.FiltroRicerca>();
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idPeople", valore = idPeople });
                filters.Add(new DocsPaVO.filtri.FiltroRicerca() { argomento = "idCorrGlobali", valore = idCorrGlobalRuolo });

                // request generazione report
                DocsPaVO.Report.PrintReportRequest request = new DocsPaVO.Report.PrintReportRequest();
                request.SearchFilters = filters;

                // selezione formato report
                switch (formato)
                {
                    case "XLS":
                        request.ReportType = DocsPaVO.Report.ReportTypeEnum.Excel;
                        break;

                    case "ODS":
                        request.ReportType = DocsPaVO.Report.ReportTypeEnum.ODS;
                        break;
                }

                string descrizione = string.Empty;

                string descrizioneRuolo = BusinessLogic.Utenti.UserManager.GetRoleDescriptionByIdCorrGlobali(idCorrGlobalRuolo);
                string descrizioneUtente = BusinessLogic.Utenti.UserManager.getUtente(idPeople).descrizione;

                request.ContextName = "ExportTrasmissioniPendenti";
                request.ReportKey = "ExportTrasmissioniPendenti";
                request.Title = string.Empty;

                if (!string.IsNullOrEmpty(descrizioneRuolo) && !string.IsNullOrEmpty(descrizioneUtente))
                    request.SubTitle = string.Format("Utente: {0} - Ruolo: {1}", descrizioneUtente, descrizioneRuolo);
                else if (!string.IsNullOrEmpty(descrizioneRuolo))
                    request.SubTitle = string.Format("Ruolo: {0}", descrizioneRuolo);
                else if (!string.IsNullOrEmpty(descrizioneUtente))
                    request.SubTitle = string.Format("Utente: {0}", descrizioneUtente);

                request.AdditionalInformation = string.Empty;

                report = BusinessLogic.Reporting.ReportGeneratorCommand.GetReport(request).Document;

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return report;
        }
        #endregion

        public static string GetCodAmmById(string idAmm)
        {
            string codiceAmm = string.Empty;
            try
            {
                using (DocsPaDB.Query_Utils.Utils dbUtils = new DocsPaDB.Query_Utils.Utils())
                    codiceAmm = dbUtils.getCodAmm(idAmm);
            }
            catch (Exception ex)
            {
                logger.Error("Errore in GetCodAmm " + ex.Message);
                codiceAmm = string.Empty;
            }
            return codiceAmm;
        }

        public static bool IsUtenteVerticale(string idPeople)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
            bool result = utente.IsUtenteVerticale(idPeople);
            return result;
        }

        public static bool SetUtenteVerticale(string userId, string idAmm, bool isVerticale)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
            bool result = utente.SetUtenteVerticale(userId, idAmm, isVerticale);
            return result;
        }
    }
}
