using System;
using System.Collections;
using System.Data;
using DocsPaDocumentale;
using System.IO;
using System.Data.OleDb;
using System.Linq;
using System.Collections.Generic;
using log4net;
using DocsPaVO.utente;
using DocsPaDB.Query_Utils;
using DocsPaDB.Query_DocsPAWS.Reporting;
using DocsPaUtils.Security;

namespace BusinessLogic.Utenti
{
    /// <summary>
    /// </summary>
    public class UserManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(UserManager));
        /// <summary>
        /// legge un corrispondente identificato dal suo id. restituisce un oggetto Corrispondente
        /// </summary>
        /// <param name="db"></param>
        /// <param name="idCorrispondente"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Corrispondente getCorrispondente(string idCorrispondente, bool isEnable)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            return utenti.GetCorrispondente(idCorrispondente, isEnable);

            #region Codice Commentato
            /*logger.Debug("getCorrispondente");
			DocsPaVO.utente.Corrispondente corrispondente = new DocsPaVO.utente.Corrispondente();
			string queryString = 
				"SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI " +
				"WHERE SYSTEM_ID=" + idCorrispondente;
			logger.Debug (queryString);		
			string idPeople = null;
			try {
				idPeople = db.executeScalar(queryString).ToString();
			} catch (Exception) {}
			
			
			if (idPeople != null && !idPeople.Equals(""))
				corrispondente = getUtente(db,idPeople);
			else 
				corrispondente = getRuolo(db,idCorrispondente);
			corrispondente.systemId = idCorrispondente;
			//corrispondente.notificaConAllegato=true;
			return corrispondente;*/
            #endregion
        }

        public static string getSignTypePreference(string idPeople)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.GetSignTypePreference(idPeople);
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByCodRubricaNotDisabled(string codice, DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.utente.InfoUtente user)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.GetCorrispondenteByCodRubricaNotDisabled(user.idAmministrazione, codice, tipoIE);
        }

        public static bool setSignTypePreference(string idPeople, string chaPreference)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.SetSignTypePreference(idPeople, chaPreference);
        }

        /// <summary>
        /// aggiunge dettagli ad un corr occ.
        /// </summary>
        /// <param name="corrispondente"></param>
        public static void addDettagliCorrOcc(DocsPaVO.utente.Corrispondente corrispondente)
        {

            DocsPaDB.Query_DocsPAWS.Documenti.addDettagliCorrOcc(corrispondente);


        }
        /// <summary>
        /// legge l'id dell'amministrazione di appartenenza di un utente passato come oggetto Login
        /// </summary>
        /// <param name="db"></param>
        /// <param name="objLogin"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static string getIdAmmUtente(DocsPaVO.utente.UserLogin objLogin)
        {
            return getIdAmmUtente(objLogin.UserName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string getIdAmmUtente(string userId)
        {
            string idAmm = null;
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            utenti.GetIdAmmUtente(out idAmm, userId);
            return idAmm;
        }

        //aggiunto sabrina
        public static ArrayList getListaIdAmmUtente(DocsPaVO.utente.UserLogin objLogin)
        {
            ArrayList List_idAmm = null;
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            utenti.GetIdAmmUtente(out List_idAmm, objLogin.UserName, objLogin.Modulo);
            return List_idAmm;
        }
        /// <summary>
        /// utilizzato dall'interoperabilità senza mail, per effettuare la login 
        /// con l'utente specificato nella DPA_EL_REGISTRI.ID_PEOPLE_AOO, così che risulti come 
        /// creatore del documento
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public string getPassword(string idPeople)
        {

            DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();

            return ut.GetPassword(idPeople);
        }
        /// <summary>
        /// get dst da DPA_LOGIN.DST
        /// </summary>
        public string getDST(string userId)
        {
            DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();

            return ut.GetDST(userId);
        }
        /// <summary>
        /// legge un utente identificato per id. Restituisce un oggetto Utente
        /// </summary>
        /// <param name="db"></param>
        /// <param name="idPeople"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Utente getUtente(string idPeople)
        {
            /*string queryString = "SELECT SYSTEM_ID, USER_ID, ID_AMM, VAR_COGNOME, VAR_NOME, VAR_TELEFONO, EMAIL_ADDRESS, CHA_NOTIFICA, CHA_AMMINISTRATORE,CHA_NOTIFICA_CON_ALLEGATO  FROM PEOPLE WHERE SYSTEM_ID="+idPeople;*/
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            return utenti.GetUtente(idPeople);
        }

        /// <summary>
        /// legge un utente identificato per id. Restituisce un oggetto Utente
        /// </summary>
        /// <param name="db"></param>
        /// <param name="idPeople"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Utente getUtenteByEmail(string idAmm, string email)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            return utenti.GetUtenteByEmail(idAmm, email);
        }


        /// <summary>
        /// legge un utente identificato per id. Restituisce un oggetto Utente
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Utente getUtenteNoFiltroDisabled(string idPeople)
        {
            /*string queryString = "SELECT SYSTEM_ID, USER_ID, ID_AMM, VAR_COGNOME, VAR_NOME, VAR_TELEFONO, EMAIL_ADDRESS, CHA_NOTIFICA, CHA_AMMINISTRATORE,CHA_NOTIFICA_CON_ALLEGATO  FROM PEOPLE WHERE SYSTEM_ID="+idPeople;*/
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            return utenti.GetUtenteNoFiltroDisabled(idPeople);
        }

        /// <summary>
        /// legge un utente identificato per nome e id amministrazione. Restituisce un oggetto Utente
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userName"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Utente getUtente(string userName, string idAmministrazione)
        {
            #region Codice Commentato
            /*string queryString =
				"SELECT SYSTEM_ID, USER_ID, ID_AMM, VAR_COGNOME, VAR_NOME, VAR_TELEFONO, EMAIL_ADDRESS, CHA_NOTIFICA, CHA_AMMINISTRATORE,CHA_NOTIFICA_CON_ALLEGATO FROM PEOPLE WHERE UPPER(USER_ID)='"+userName.ToUpper() +"'";
			if(idAmministrazione != null)
				queryString += " AND ID_AMM='"+idAmministrazione+"'";*/
            #endregion

            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            return utenti.GetUtente(userName, idAmministrazione);
        }

        /// <summary>
        /// Reperimento dei dati dell'utente a partire dalla matricola
        /// </summary>
        /// <param name="matricola"></param>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Utente getUtenteByMatricola(string matricola, string idAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            return utenti.GetUtenteByMatricola(matricola, idAmministrazione);
        }

        #region Metodo Commentato
        /*private static DocsPaVO.utente.Utente getUserData (DocsPa_V15_Utils.Database db, string queryString) {
			logger.Debug("getUtente");
			DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
			//ricerca delle caratteristiche utente nella tabella people
			logger.Debug(queryString);
			DataSet dataSet = new DataSet();

			db.fillTable(queryString,dataSet,"PEOPLE");
			
			if(dataSet.Tables["PEOPLE"].Rows.Count>0) {		
				DataRow utenteRow = dataSet.Tables["PEOPLE"].Rows[0];
				utente.idPeople = utenteRow["SYSTEM_ID"].ToString();
				utente.userId = utenteRow["USER_ID"].ToString();
				utente.descrizione = utenteRow["VAR_COGNOME"].ToString()+" "+utenteRow["VAR_NOME"].ToString();
				utente.telefono = utenteRow["VAR_TELEFONO"].ToString();
				utente.email = utenteRow["EMAIL_ADDRESS"].ToString();
				utente.notifica = utenteRow["CHA_NOTIFICA"].ToString();
				utente.amministratore = fromCharToBool(utenteRow["CHA_AMMINISTRATORE"].ToString());
				utente.assegnante = fromCharToBool(utenteRow["CHA_AMMINISTRATORE"].ToString());
				utente.assegnatario = fromCharToBool(utenteRow["CHA_AMMINISTRATORE"].ToString());
				utente.idAmministrazione = utenteRow["ID_AMM"].ToString();			
				utente.notificaConAllegato=fromCharToBool(utenteRow["CHA_NOTIFICA_CON_ALLEGATO"].ToString());
			}
			dataSet.Dispose();

			return utente;
		}*/
        #endregion

        /// <summary>
        /// getRuolo By Id_RUOLo_in_uo=dpa_corr_globali.system_id
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Ruolo getRuolo(string idRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            return utenti.GetRuolo(idRuolo);

            #region Codice Commentato
            /*logger.Debug("getRuolo");
			DocsPaVO.utente.Ruolo objRuolo = null;
			DataSet dataSet = new DataSet();
			string queryString = "";
			queryString = 
				"SELECT A.PEOPLE_SYSTEM_ID, B.SYSTEM_ID, B.ID_GRUPPO, C.NUM_LIVELLO, B.ID_REGISTRO, " +
				"C.VAR_CODICE, C.VAR_DESC_RUOLO, D.NUM_LIVELLO AS NUM_LIVELLO_UO, B.ID_UO, B.VAR_COD_RUBRICA " +
				"FROM PEOPLEGROUPS A, DPA_CORR_GLOBALI B, DPA_TIPO_RUOLO C, DPA_CORR_GLOBALI D " +
				"WHERE B.ID_GRUPPO=A.GROUPS_SYSTEM_ID AND B.ID_TIPO_RUOLO=C.SYSTEM_ID AND D.SYSTEM_ID=B.ID_UO "+
				"AND B.SYSTEM_ID = "+idRuolo;
			logger.Debug(queryString);
			DataSet dataSet = new DataSet();
			db.fillTable(queryString,dataSet,"RUOLI");
			
			if(dataSet.Tables["RUOLI"].Rows.Count>0) {				
				// cerco il valore di livello più alto per le UO associate all'utente in modo
				// da limitare il dimensione della tabella delle UO.
				string maxLivello = dataSet.Tables["RUOLI"].Select("","NUM_LIVELLO_UO desc")[0]["NUM_LIVELLO_UO"].ToString();
				logger.Debug("maxlivello:"+maxLivello);
				queryString =
					"SELECT * FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='U' " +
					"AND CHA_TIPO_IE='I' AND DTA_FINE IS NULL AND NUM_LIVELLO <=" + 
					maxLivello;
				logger.Debug(queryString);
				db.fillTable(queryString,dataSet,"UO");
				obj.getRuoloMaxLivello(dataSet,maxLivello);
				objRuolo = getRuoloData(db,dataSet,dataSet.Tables["RUOLI"].Rows[0]);
			}

			dataSet.Dispose();
			
			return objRuolo;*/
            #endregion
        }

        /// <summary>
        /// Questo metodo ricerca il ruolo e lo ritorna anche se è disabilitato
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Ruolo getRuoloEnabledAndDisabled(string idRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            return utenti.GetRuoloEnabledAndDisabled(idRuolo);
        }

        //metodo per la gestione dell'ingresso a DocsPA dal portale
        public ArrayList getVisibilitaRuolo(string docNumber, string idGruppi)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            return utenti.VisibilitaRuolo(docNumber, idGruppi);
        }

        //*********************************************************************************************
        // IACOZZILLI GIORDANO 03122013
        // Per aggiungere il nuovo PIS GetRolesForEnabledActions devo creare una nuova GetRuoliUtente
        //*********************************************************************************************
        /// <summary>
        /// Funzione che Getta i ruoli di un determinato utente con una determinata Funzione associata.
        /// </summary>
        /// <param name="idPeople">Id people</param>
        /// <param name="tipofunzione">tipo funzione per la get dei ruoli</param>
        /// <returns></returns>
        public static ArrayList getRuoliUtenteForEnabledActions(string idPeople, string tipofunzione, string idAmministrazione)
        {
            ArrayList ruoli = new ArrayList();
            DataSet dataSet = new DataSet();

            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            utenti.GetRuoliUOUtenteForEnabledActions(dataSet, idPeople, tipofunzione, idAmministrazione);

            // cerco il valore di livello più alto per le UO associate all'utente in modo
            // da limitare il dimensione della tabella delle UO.
            if (dataSet.Tables["RUOLIXFUNC"].Rows.Count > 0)
            {

                string maxLivello = dataSet.Tables["RUOLIXFUNC"].Select("", "NUM_LIVELLO desc")[0]["NUM_LIVELLO"].ToString();

                #region Codice Commentato
                /*string commandString4="SELECT * FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND DTA_FINE IS NULL AND NUM_LIVELLO <=" + maxLivello;
				
				// TODO: Utilizzare il progetto DocsPaDbManagement
				database.fillTable(commandString4,dataSet,"UO");*/
                #endregion

                //Inizio modifica luluciani
                //NEW  
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USA_CONNECTBYPRIOR_OR_WITH")) && (DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USA_CONNECTBYPRIOR_OR_WITH")).Equals("1"))
                {
                    foreach (DataRow ruoloRow in dataSet.Tables["RUOLIXFUNC"].Rows)
                    {
                        utenti.GetRuoliUtente(dataSet, ruoloRow, maxLivello, idPeople, false);
                        if (ruoloRow["CHA_PREFERITO"] != null)
                        {
                            if (ruoloRow["CHA_PREFERITO"].ToString() == "1")
                                ruoli.Add(getRuoloData(dataSet, ruoloRow));
                        }
                    }
                }
                //OLD  
                else
                {
                    utenti.GetRuoliUtente(dataSet, maxLivello, idPeople);
                    foreach (DataRow ruoloRow in dataSet.Tables["RUOLIXFUNC"].Rows)
                    {
                        if (ruoloRow["CHA_PREFERITO"] != null)
                        {
                            if (ruoloRow["CHA_PREFERITO"].ToString() == "1")
                                ruoli.Add(getRuoloData(dataSet, ruoloRow));
                        }
                    }
                }
                //Fine modifica luluciani  

                foreach (DataRow ruoloRow in dataSet.Tables["RUOLIXFUNC"].Rows)
                {
                    if (ruoloRow["CHA_PREFERITO"] == null)
                    {
                        ruoli.Add(getRuoloData(dataSet, ruoloRow));
                    }
                    else
                    {
                        if (ruoloRow["CHA_PREFERITO"].ToString() != "1")
                            ruoli.Add(getRuoloData(dataSet, ruoloRow));
                    }
                }
            }

            return ruoli;
        }

        //*********************************************************************************************
        // ABBATANGELI GIANLUIGI 18112015
        // Restituisce tutti i ruoli di un utente, abilitati ad un specifica micro funzione
        //*********************************************************************************************
        /// <summary>
        /// Funzione che Getta i ruoli di un determinato utente con una determinata Micro Funzione associata.
        /// </summary>
        /// <param name="idPeople">Id people</param>
        /// <param name="tipofunzione">tipo micro funzione per la get dei ruoli</param>
        /// <returns></returns>
        public static ArrayList getRolesForEnabledSingleFunction(string idPeople, string tipoMicrofunzione, string idAmministrazione)
        {
            ArrayList ruoli = new ArrayList();
            DataSet dataSet = new DataSet();

            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            utenti.GetRuoliUOUtenteForEnabledSingleFunction(dataSet, idPeople, tipoMicrofunzione, idAmministrazione);

            // cerco il valore di livello più alto per le UO associate all'utente in modo
            // da limitare il dimensione della tabella delle UO.
            if (dataSet.Tables["RUOLIXFUNC"].Rows.Count > 0)
            {

                string maxLivello = dataSet.Tables["RUOLIXFUNC"].Select("", "NUM_LIVELLO desc")[0]["NUM_LIVELLO"].ToString();

                #region Codice Commentato
                /*string commandString4="SELECT * FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND DTA_FINE IS NULL AND NUM_LIVELLO <=" + maxLivello;
				
				// TODO: Utilizzare il progetto DocsPaDbManagement
				database.fillTable(commandString4,dataSet,"UO");*/
                #endregion

                //Inizio modifica luluciani
                //NEW  
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USA_CONNECTBYPRIOR_OR_WITH")) && (DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USA_CONNECTBYPRIOR_OR_WITH")).Equals("1"))
                {
                    foreach (DataRow ruoloRow in dataSet.Tables["RUOLIXFUNC"].Rows)
                    {
                        utenti.GetRuoliUtente(dataSet, ruoloRow, maxLivello, idPeople, false);
                        if (ruoloRow["CHA_PREFERITO"] != null)
                        {
                            if (ruoloRow["CHA_PREFERITO"].ToString() == "1")
                                ruoli.Add(getRuoloData(dataSet, ruoloRow));
                        }
                    }
                }
                //OLD  
                else
                {
                    utenti.GetRuoliUtente(dataSet, maxLivello, idPeople);
                    foreach (DataRow ruoloRow in dataSet.Tables["RUOLIXFUNC"].Rows)
                    {
                        if (ruoloRow["CHA_PREFERITO"] != null)
                        {
                            if (ruoloRow["CHA_PREFERITO"].ToString() == "1")
                                ruoli.Add(getRuoloData(dataSet, ruoloRow));
                        }
                    }
                }
                //Fine modifica luluciani  

                foreach (DataRow ruoloRow in dataSet.Tables["RUOLIXFUNC"].Rows)
                {
                    if (ruoloRow["CHA_PREFERITO"] == null)
                    {
                        ruoli.Add(getRuoloData(dataSet, ruoloRow));
                    }
                    else
                    {
                        if (ruoloRow["CHA_PREFERITO"].ToString() != "1")
                            ruoli.Add(getRuoloData(dataSet, ruoloRow));
                    }
                }
            }

            return ruoli;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public static ArrayList getRuoliUtente(string idPeople)
        {
            ArrayList ruoli = new ArrayList();
            DataSet dataSet = new DataSet();

            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            //ricerca dei ruoli e relative UO dell'utente
            #region Codice Commentato
            /*string commandString3="SELECT A.PEOPLE_SYSTEM_ID, B.SYSTEM_ID, B.ID_GRUPPO, C.NUM_LIVELLO, C.VAR_CODICE, C.VAR_DESC_RUOLO, B.NUM_LIVELLO, B.ID_UO, B.VAR_COD_RUBRICA, B.ID_REGISTRO FROM PEOPLEGROUPS A, DPA_CORR_GLOBALI B, DPA_TIPO_RUOLO C where B.ID_GRUPPO=A.GROUPS_SYSTEM_ID and b.id_tipo_ruolo=c.system_id and A.PEOPLE_SYSTEM_ID="+idPeople+" ORDER BY A.CHA_PREFERITO DESC";

			// TODO: Utilizzare il progetto DocsPaDbManagement
			database.fillTable(commandString3,dataSet,"RUOLI");*/
            #endregion

            utenti.GetRuoliUOUtente(dataSet, idPeople);

            // cerco il valore di livello più alto per le UO associate all'utente in modo
            // da limitare il dimensione della tabella delle UO.
            if (dataSet.Tables["RUOLI"].Rows.Count > 0)
            {

                string maxLivello = dataSet.Tables["RUOLI"].Select("", "NUM_LIVELLO desc")[0]["NUM_LIVELLO"].ToString();

                #region Codice Commentato
                /*string commandString4="SELECT * FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND DTA_FINE IS NULL AND NUM_LIVELLO <=" + maxLivello;
				
				// TODO: Utilizzare il progetto DocsPaDbManagement
				database.fillTable(commandString4,dataSet,"UO");*/
                #endregion

                //Inizio modifica luluciani
                //NEW  
                if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USA_CONNECTBYPRIOR_OR_WITH")) && (DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USA_CONNECTBYPRIOR_OR_WITH")).Equals("1"))
                {
                    foreach (DataRow ruoloRow in dataSet.Tables["RUOLI"].Rows)
                    {
                        utenti.GetRuoliUtente(dataSet, ruoloRow, maxLivello, idPeople, false);
                        if (ruoloRow["CHA_PREFERITO"] != null)
                        {
                            if (ruoloRow["CHA_PREFERITO"].ToString() == "1")
                                ruoli.Add(getRuoloData(dataSet, ruoloRow));
                        }
                    }
                }
                //OLD  
                else
                {
                    utenti.GetRuoliUtente(dataSet, maxLivello, idPeople);
                    foreach (DataRow ruoloRow in dataSet.Tables["RUOLI"].Rows)
                    {
                        if (ruoloRow["CHA_PREFERITO"] != null)
                        {
                            if (ruoloRow["CHA_PREFERITO"].ToString() == "1")
                                ruoli.Add(getRuoloData(dataSet, ruoloRow));
                        }
                    }
                }
                //Fine modifica luluciani  

                foreach (DataRow ruoloRow in dataSet.Tables["RUOLI"].Rows)
                {
                    if (ruoloRow["CHA_PREFERITO"] == null)
                    {
                        ruoli.Add(getRuoloData(dataSet, ruoloRow));
                    }
                    else
                    {
                        if (ruoloRow["CHA_PREFERITO"].ToString() != "1")
                            ruoli.Add(getRuoloData(dataSet, ruoloRow));
                    }
                }
            }

            return ruoli;
        }

        public static ArrayList getRuoliUtenteByIdCorr(string idCorr)
        {
            ArrayList ruoli = new ArrayList();
            DataSet dataSet = new DataSet();

            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            //ricerca dei ruoli e relative UO dell'utente
            #region Codice Commentato
            /*string commandString3="SELECT A.PEOPLE_SYSTEM_ID, B.SYSTEM_ID, B.ID_GRUPPO, C.NUM_LIVELLO, C.VAR_CODICE, C.VAR_DESC_RUOLO, B.NUM_LIVELLO, B.ID_UO, B.VAR_COD_RUBRICA, B.ID_REGISTRO FROM PEOPLEGROUPS A, DPA_CORR_GLOBALI B, DPA_TIPO_RUOLO C where B.ID_GRUPPO=A.GROUPS_SYSTEM_ID and b.id_tipo_ruolo=c.system_id and A.PEOPLE_SYSTEM_ID="+idPeople+" ORDER BY A.CHA_PREFERITO DESC";

			// TODO: Utilizzare il progetto DocsPaDbManagement
			database.fillTable(commandString3,dataSet,"RUOLI");*/
            #endregion

            utenti.GetRuoliUOUtenteByIdCorr(dataSet, idCorr);

            // cerco il valore di livello più alto per le UO associate all'utente in modo
            // da limitare il dimensione della tabella delle UO.
            if (dataSet.Tables["RUOLI"].Rows.Count > 0)
            {

                string maxLivello = dataSet.Tables["RUOLI"].Select("", "NUM_LIVELLO desc")[0]["NUM_LIVELLO"].ToString();

                #region Codice Commentato
                /*string commandString4="SELECT * FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP='U' AND CHA_TIPO_IE='I' AND DTA_FINE IS NULL AND NUM_LIVELLO <=" + maxLivello;
				
				// TODO: Utilizzare il progetto DocsPaDbManagement
				database.fillTable(commandString4,dataSet,"UO");*/
                #endregion

                utenti.GetRuoliUtente(dataSet, maxLivello, idCorr);

                foreach (DataRow ruoloRow in dataSet.Tables["RUOLI"].Rows)
                {
                    //modifica luluciani
                    //utenti.GetRuoliUtente(dataSet, ruoloRow, maxLivello, idPeople);

                    if (ruoloRow["CHA_PREFERITO"] != null)
                    {
                        if (ruoloRow["CHA_PREFERITO"].ToString() == "1")
                            ruoli.Add(getRuoloData(dataSet, ruoloRow));
                    }
                }

                foreach (DataRow ruoloRow in dataSet.Tables["RUOLI"].Rows)
                {
                    if (ruoloRow["CHA_PREFERITO"] == null)
                    {
                        ruoli.Add(getRuoloData(dataSet, ruoloRow));
                    }
                    else
                    {
                        if (ruoloRow["CHA_PREFERITO"].ToString() != "1")
                            ruoli.Add(getRuoloData(dataSet, ruoloRow));
                    }
                }

                /*foreach(DataRow ruoloRow in dataSet.Tables["RUOLI"].Rows) 
                {											
                    ruoli.Add(getRuoloData(dataSet, ruoloRow));
                }*/
            }

            return ruoli;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="ruoloRow"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.Ruolo getRuoloData(DataSet dataSet, DataRow ruoloRow)
        {
            #region Codice Commentato
            //memorizzo l'id del ruolo per scriverla una sola volta
            /*string id = ruoloRow["SYSTEM_ID"].ToString();	
            DocsPaVO.utente.UnitaOrganizzativa uo = UserManager.getUnitaOrganizzativa(dataSet, dataSet.Tables["UO"].Select("SYSTEM_ID='"+ruoloRow["ID_UO"].ToString()+"'","SYSTEM_ID")[0]);
            DocsPaVO.utente.Ruolo ruolo=new DocsPaVO.utente.Ruolo(
                ruoloRow["SYSTEM_ID"].ToString(),
                ruoloRow["VAR_DESC_RUOLO"].ToString()+" "+uo.descrizione,
                ruoloRow["VAR_CODICE"].ToString(),
                ruoloRow["NUM_LIVELLO"].ToString(),
                ruoloRow["ID_GRUPPO"].ToString(),
                null, 
                UserManager.getFunzioni(id));
            ruolo.uo = uo;
            ruolo.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();
            ruolo.idRegistro = ruoloRow["ID_REGISTRO"].ToString();
            ruolo.registri = RegistriManager.getRegistriRuolo(id);
            return ruolo;*/
            #endregion

            DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
            ruolo = ut.GetRuoloData(dataSet, ruoloRow);
            ut.Dispose();

            return ruolo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public static ArrayList getFunzioni(string idRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            return utenti.GetFunzioni(idRuolo);

            #region Codice Commentato
            //ricerca delle funzioni associate al ruolo
            /*string queryString =
                "SELECT A.SYSTEM_ID, A.COD_FUNZIONE, A.VAR_DESC_FUNZIONE, " +
                "A.ID_TIPO_FUNZIONE, B.VAR_COD_TIPO, B.VAR_DESC_TIPO_FUN " +
                "FROM DPA_FUNZIONI A, DPA_TIPO_FUNZIONE B, DPA_TIPO_F_RUOLO C " +
                "WHERE A.ID_TIPO_FUNZIONE=B.SYSTEM_ID AND " +
                "B.SYSTEM_ID=C.ID_TIPO_FUNZ AND C.ID_RUOLO_IN_UO=" + idRuolo;

            DataSet dataSet = new DataSet();
            db.fillTable(queryString,dataSet,"FUNZIONI");
            DataSet dataSet = new DataSet();
			
            ArrayList funzioni=new ArrayList();
            foreach(DataRow funzioneRow in dataSet.Tables["FUNZIONI"].Rows) {
                DocsPaVO.utente.Funzione funzione=new DocsPaVO.utente.Funzione(
                    funzioneRow["SYSTEM_ID"].ToString(),
                    funzioneRow["VAR_DESC_FUNZIONE"].ToString(),
                    funzioneRow["COD_FUNZIONE"].ToString(),
                    funzioneRow["ID_TIPO_FUNZIONE"].ToString(),
                    funzioneRow["VAR_COD_TIPO"].ToString(),
                    funzioneRow["VAR_DESC_TIPO_FUN"].ToString()
                    );
                funzioni.Add(funzione);
            }
            dataSet.Dispose();
            return funzioni;*/
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="uoRow"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.UnitaOrganizzativa getUnitaOrganizzativa(DataSet dataSet, DataRow uoRow)
        {
            //ricerca delle UO
            DocsPaVO.utente.UnitaOrganizzativa uo = new DocsPaVO.utente.UnitaOrganizzativa();
            uo.systemId = uoRow["SYSTEM_ID"].ToString();
            uo.descrizione = uoRow["VAR_DESC_CORR"].ToString();
            uo.codice = uoRow["VAR_CODICE"].ToString();
            uo.livello = uoRow["NUM_LIVELLO"].ToString();
            DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
            sp.serverSMTP = uoRow["VAR_SMTP"].ToString();
            sp.portaSMTP = uoRow["NUM_PORTA_SMTP"].ToString();
            uo.serverPosta = sp;
            uo.idRegistro = uoRow["ID_REGISTRO"].ToString();

            //si ricava la parentela
            if (!uoRow["ID_PARENT"].ToString().Equals("0"))
            {
                uo.parent = getParents(uoRow["ID_PARENT"].ToString(), dataSet.Tables["UO"]);
            }

            return uo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idParent"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.UnitaOrganizzativa getParents(string idParent, DataTable dt)
        {
            DataRow parentRow = dt.Select("SYSTEM_ID='" + idParent + "'")[0];
            DocsPaVO.utente.UnitaOrganizzativa parent = new DocsPaVO.utente.UnitaOrganizzativa();
            parent.systemId = parentRow["SYSTEM_ID"].ToString();
            parent.descrizione = parentRow["VAR_DESC_CORR"].ToString();
            parent.codice = parentRow["VAR_CODICE"].ToString();
            parent.livello = parentRow["NUM_LIVELLO"].ToString();
            DocsPaVO.utente.ServerPosta sp = new DocsPaVO.utente.ServerPosta();
            sp.serverSMTP = parentRow["VAR_SMTP"].ToString();
            sp.portaSMTP = parentRow["NUM_PORTA_SMTP"].ToString();
            parent.serverPosta = sp;
            parent.idRegistro = parentRow["ID_REGISTRO"].ToString();

            if (!parentRow["ID_PARENT"].ToString().Equals("0"))
            {
                parent.parent = getParents(parentRow["ID_PARENT"].ToString(), dt);
            }

            return parent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool fromCharToBool(string str)
        {
            bool result = true;

            if (!str.Equals("1"))
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// DeleteCorrispondenteEsterno
        /// </summary>
        /// <param name="idCorrGlobali"></param>
        /// <param name="flagListe"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool DeleteCorrispondenteEsterno(string idCorrGlobali, int flagListe, DocsPaVO.utente.InfoUtente user, out string message)
        {
            bool retValue = false;

            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            retValue = utenti.DeleteCorrispondenteEsterno(idCorrGlobali, flagListe, user, out message);

            return retValue;
        }

        public static bool ModifyCorrispondenteEsterno(DocsPaVO.utente.DatiModificaCorr datiModifica, InfoUtente user, out string message)
        {
            bool retValue = false;

            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            string newIdCorrGlobali;
            retValue = utenti.ModifyCorrispondenteEsterno(datiModifica, user, out newIdCorrGlobali, out message);

            return retValue;
        }

        //Overload del Metodo ModifyCorrispondenteEsterno per farmi restituire anche l'id del nuovo corrispondente (IACOPINO M. - 04/11/2011)
        public static bool ModifyCorrispondenteEsterno(DocsPaVO.utente.DatiModificaCorr datiModifica, InfoUtente user, out string newIdCorrGlobali, out string message)
        {
            bool retValue = false;

            //Emanuela :aggiungo nel backend i controlli per codice fiscale e partita iva
            string messaggio = string.Empty;
            if ((!string.IsNullOrEmpty(datiModifica.codFiscale) || (!string.IsNullOrEmpty(datiModifica.partitaIva))))
                validaCampi(datiModifica, ref messaggio);
            if (!string.IsNullOrEmpty(messaggio))
            {
                logger.Debug(messaggio);
                newIdCorrGlobali = string.Empty;
                message = "KO";
                retValue = false;
                return retValue;
            }
            //Fine codice Emanuela 

            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            if (datiModifica.tipoCorrispondente !=null && datiModifica.tipoCorrispondente.Equals("O"))
                retValue = utenti.InsertOccasionale(datiModifica, out newIdCorrGlobali, out message);              
            else
                retValue = utenti.ModifyCorrispondenteEsterno(datiModifica, user, out newIdCorrGlobali, out message);

            if (!string.IsNullOrEmpty(DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_ENABLE_FLUSSO_AUTOMATICO")) &&
                        DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "FE_ENABLE_FLUSSO_AUTOMATICO").ToString().Equals("1"))
            {
                if (retValue)
                {
                    if (!string.IsNullOrEmpty(newIdCorrGlobali) && !newIdCorrGlobali.Equals("0"))
                    {
                        if(datiModifica.interoperanteRGS)
                            BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.InsertCorrispondenteRGS(newIdCorrGlobali, true);
                    }
                    else
                    {
                        bool isInteroperanteRGS = BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.CheckIsInteroperanteRGS(datiModifica.idCorrGlobali);

                        if (!isInteroperanteRGS && datiModifica.interoperanteRGS)
                            BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.InsertCorrispondenteRGS(datiModifica.idCorrGlobali, true);

                        if (isInteroperanteRGS && !datiModifica.interoperanteRGS)
                            BusinessLogic.FlussoAutomatico.FlussoAutomaticoManager.DeleteCorrispondenteRGS(datiModifica.idCorrGlobali);
                    } 
                }
            }

            return retValue;
        }

        //Emanuela: METODO PER IL CONTROLLO SULLA VALIDITà DEI CAMPI DEL CORRISPONDENTE
        private static void validaCampi(DocsPaVO.utente.DatiModificaCorr datiModifica, ref string messaggio)
        {
           
            if (((datiModifica.tipoCorrispondente != null && datiModifica.tipoCorrispondente.ToUpper() == "U") || string.IsNullOrEmpty(datiModifica.cognome)))
            {
                if ((datiModifica.codFiscale != null && !datiModifica.codFiscale.Equals("")) && ((datiModifica.codFiscale.Length == 11 && BusinessLogic.Rubrica.DPA3_RubricaSearchAgent.CheckVatNumber(datiModifica.codFiscale) != 0) || (datiModifica.codFiscale.Length == 16 && BusinessLogic.Rubrica.DPA3_RubricaSearchAgent.CheckTaxCode(datiModifica.codFiscale) != 0) || (datiModifica.codFiscale.Length != 11 && datiModifica.codFiscale.Length != 16)))
                {
                    messaggio = "Attenzione, il campo CODICE FISCALE non è valido";
                    return;
                }
            }
            else
            {
                        
                if (datiModifica.codFiscale != null && !datiModifica.codFiscale.Equals("") && BusinessLogic.Rubrica.DPA3_RubricaSearchAgent.CheckTaxCode(datiModifica.codFiscale) != 0)
                {
                    messaggio = "Attenzione, il campo CODICE FISCALE non è valido";
                    return;
                }
            }
    

            if (datiModifica.partitaIva != null && !datiModifica.partitaIva.Equals("") && BusinessLogic.Rubrica.DPA3_RubricaSearchAgent.CheckVatNumber(datiModifica.partitaIva) != 0)
            {
                messaggio = "Attenzione, il campo PARTITA IVA non è valido";
                return;
            }
        }


        #region Metodi di supporto per Nuova Rubrica

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByCodRubrica(string codice, DocsPaVO.utente.InfoUtente user, string condRegistri, bool storicizzato)
        {
            return getCorrispondenteByCodRubrica(codice, DocsPaVO.addressbook.TipoUtente.GLOBALE, user, condRegistri, storicizzato);
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByCodRubrica(string codice, DocsPaVO.utente.InfoUtente user)
        {
            return getCorrispondenteByCodRubrica(codice, DocsPaVO.addressbook.TipoUtente.GLOBALE, user);
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByEmail(string mail, DocsPaVO.utente.InfoUtente user, DocsPaVO.utente.Registro reg, out string messaggioErrore)
        {
            return getCorrispondenteByEmail(mail, user, reg.systemId, out messaggioErrore);
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByEmail(string mail, DocsPaVO.utente.InfoUtente user, DocsPaVO.utente.Registro reg)
        {
            return getCorrispondenteByEmail(mail, user, reg.systemId);
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByEmailCodiceAmmCodiceAoo(string mail, DocsPaVO.utente.InfoUtente user, DocsPaVO.utente.Registro reg, string codiceAmm, string codiceAoo)
        {
            return getCorrispondenteByEmailCodiceAmmCodiceAoo(mail, user, reg.systemId, codiceAmm, codiceAoo);
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteBySystemID(string system_id)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.GetCorrispondenteBySystemID(system_id);
        }

        public static DocsPaVO.utente.Canale GetCanalePreferenzialeByIdCorr(string system_id)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.GetCanalePreferenzialeByIdCorr(system_id);
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteBySystemIDDisabled(string system_id)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.GetCorrispondenteBySystemIDDisabled(system_id);
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByCodRubrica(string codice, DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.utente.InfoUtente user, string condRegistri, bool storicizzato)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();

            DocsPaVO.utente.Corrispondente corr = u.GetCorrispondenteByCodRubrica(user.idAmministrazione, codice, tipoIE, condRegistri, storicizzato);
            if (corr != null)
                logger.Debug("u: " + corr.systemId);
            if (RubricaComune.Configurazioni.GetConfigurazioni(user).GestioneAbilitata)
            {
                if (corr == null)
                {
                    // Se il corrispondente non è stato trovato in docspa,
                    // viene effettuata la ricerca del corrispondente in rubrica comune
                    if (!string.IsNullOrEmpty(codice))
                        corr = RubricaComune.RubricaServices.UpdateCorrispondente(user, codice);
                }
                else if (string.IsNullOrEmpty(corr.systemId) || corr.inRubricaComune)
                {
                    logger.Debug("cerco Rubrica Comune");
                    corr = RubricaComune.RubricaServices.UpdateCorrispondente(user, corr);
                }
            }

            return corr;
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByCodRubrica(string codice, DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.utente.InfoUtente user)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();

            DocsPaVO.utente.Corrispondente corr = u.GetCorrispondenteByCodRubrica(user.idAmministrazione, codice, tipoIE);
            if (corr != null)
                logger.Debug("u: " + corr.systemId);
            if (RubricaComune.Configurazioni.GetConfigurazioni(user).GestioneAbilitata)
            {
                if (corr == null)
                {
                    // Se il corrispondente non è stato trovato in docspa,
                    // viene effettuata la ricerca del corrispondente in rubrica comune
                    if (!string.IsNullOrEmpty(codice))
                        corr = RubricaComune.RubricaServices.UpdateCorrispondente(user, codice);
                }
                else if (string.IsNullOrEmpty(corr.systemId) || corr.inRubricaComune)
                {
                    logger.Debug("cerco Rubrica Comune");
                    corr = RubricaComune.RubricaServices.UpdateCorrispondente(user, corr);
                }
            }

            return corr;
        }

        /// <summary>
        /// Restituisce un corrispondente di rubrica comune o di rubica locale con storicizzazione del corrispondete di rubrica locale nel caso in cui il corrispondente è presente anche in rubrica comune
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="user"></param>
        /// <param name="id_reg"></param>
        /// <param name="messaggioErrore"></param>
        /// <returns>DocsPaVO.utente.Corrispondente</returns>
        public static DocsPaVO.utente.Corrispondente getCorrispondenteByEmail(string mail, DocsPaVO.utente.InfoUtente user, string id_reg, out string messaggioErrore)
        {
            string check_same_mailbox = string.Empty;
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            messaggioErrore = string.Empty;
            bool rc = false;
            int rows = 0;
            DocsPaVO.utente.Corrispondente corr = u.GetCorrispondenteByEmail(user.idAmministrazione, mail, id_reg, out rows);
            DocsPaVO.utente.Corrispondente corr1 = null;

            if (RubricaComune.Configurazioni.GetConfigurazioni(user).GestioneAbilitata)
            {
                if (corr == null)
                {
                    // Se il corrispondente non è stato trovato in docspa,
                    // viene effettuata la ricerca del corrispondente in rubrica comune
                    if (!string.IsNullOrEmpty(mail))
                        corr1 = RubricaComune.RubricaServices.UpdateCorrispondenteByEmail(user, mail);
                    if (corr1 != null)
                    {
                        corr = corr1;
                        messaggioErrore = "RC";
                    }

                }
                else if (string.IsNullOrEmpty(corr.systemId) || corr.inRubricaComune)
                {
                    logger.Debug("cerco Rubrica Comune");
                    corr1 = RubricaComune.RubricaServices.UpdateCorrispondente(user, corr);
                    if (corr1 != null)
                    {
                        // inserisco un ulteriore controllo per evitare di cancellare i corrispondenti interni
                        if (!corr.tipoIE.ToUpper().Equals("I") && corr.systemId != corr1.systemId)
                        {
                        bool cancellato = BusinessLogic.Utenti.UserManager.DeleteCorrispondenteEsterno(corr.systemId, 0, user, out messaggioErrore);
                        if (!string.IsNullOrEmpty(messaggioErrore)
                            && !cancellato)
                            messaggioErrore = "(CODINTEROP3)";
                        }
                        corr = corr1;
                        messaggioErrore = "RC";
                    }
                }
                else
                    if (!string.IsNullOrEmpty(mail)
                        && corr != null)
                    {
                        corr1 = RubricaComune.RubricaServices.UpdateCorrispondenteByEmail(user, mail);
                        if (corr1 != null)
                        {
                            // inserisco un ulteriore controllo per evitare di cancellare i corrispondenti interni
                            if (!corr.tipoIE.ToUpper().Equals("I") && corr.systemId != corr1.systemId)
                            {
                                bool cancellato = BusinessLogic.Utenti.UserManager.DeleteCorrispondenteEsterno(corr.systemId, 0, user, out messaggioErrore);
                                if (!string.IsNullOrEmpty(messaggioErrore)
                                    && !cancellato)
                                    messaggioErrore = "(CODINTEROP3)";
                            }
                            corr = corr1;
                            messaggioErrore = "RC";
                        }
                    }
             }
            check_same_mailbox = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(user.idAmministrazione, "CHECK_MAILBOX_INTEROPERANTE");
            if (check_same_mailbox != null && check_same_mailbox.Equals("1"))
            {
                if(rows>1)
                    messaggioErrore += "#2";
                else if(rows==1)
                {
                    messaggioErrore += "#1";
                }
            }
               
            return corr;
        }


        public static DocsPaVO.utente.Corrispondente getCorrispondenteByEmail(string mail, DocsPaVO.utente.InfoUtente user, string id_reg)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();

            DocsPaVO.utente.Corrispondente corr = u.GetCorrispondenteByEmail(user.idAmministrazione, mail, id_reg);

            if (RubricaComune.Configurazioni.GetConfigurazioni(user).GestioneAbilitata)
            {
                if (corr == null)
                {
                    // Se il corrispondente non è stato trovato in docspa,
                    // viene effettuata la ricerca del corrispondente in rubrica comune
                    if (!string.IsNullOrEmpty(mail))
                        corr = RubricaComune.RubricaServices.UpdateCorrispondenteByEmail(user, mail);
                }
                else if (string.IsNullOrEmpty(corr.systemId) || corr.inRubricaComune)
                {
                    logger.Debug("cerco Rubrica Comune");
                    corr = RubricaComune.RubricaServices.UpdateCorrispondente(user, corr);
                }
            }

            return corr;
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByEmailCodiceAmmCodiceAoo(string mail, DocsPaVO.utente.InfoUtente user, string id_reg, string codiceAmm, string codiceAoo)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();

            DocsPaVO.utente.Corrispondente corr = u.getCorrispondenteByEmailCodiceAmmCodiceAoo(user.idAmministrazione, mail, id_reg, codiceAmm, codiceAoo);

            if (RubricaComune.Configurazioni.GetConfigurazioni(user).GestioneAbilitata)
            {
                if (corr == null)
                {
                    // Se il corrispondente non è stato trovato in docspa,
                    // viene effettuata la ricerca del corrispondente in rubrica comune
                    if (!string.IsNullOrEmpty(mail))
                        corr = RubricaComune.RubricaServices.UpdateCorrispondenteByEmail(user, mail, codiceAmm, codiceAoo);
                }
                else if (string.IsNullOrEmpty(corr.systemId) || corr.inRubricaComune)
                {
                    logger.Debug("cerco Rubrica Comune");
                    corr = RubricaComune.RubricaServices.UpdateCorrispondente(user, corr);
                }
            }

            return corr;
        }

        //nuovo metodo sab
        public static DocsPaVO.utente.Corrispondente getCorrispondenteByCodRubrica(string codice, DocsPaVO.addressbook.TipoUtente tipoIE, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.GetCorrispondenteByCodRubrica(idAmm, codice, tipoIE);
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteCompletoBySystemId(string systemId, DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.utente.InfoUtente user)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.GetCorrispondenteCompletoBySystemId(user.idAmministrazione, systemId, tipoIE);
        }

        public static DocsPaVO.utente.Utente getUtenteById(string idPeople)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.getUtenteById(idPeople);
        }

        public static ArrayList getListaUtentiByIdRuolo(string idRuolo)
        {
            ArrayList result = null;

            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();

            result = u.GetListaUtentiByIdRuolo(idRuolo);

            return result;
        }


        public static DocsPaVO.utente.Ruolo getRuoloById(string idCorrGlobali)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.getRuoloById(idCorrGlobali);
        }

        #endregion

        #region ImportaUtenti
        public static bool importaUtenti(DocsPaVO.utente.InfoUtente infoUtente, byte[] dati, string nomeFile, string serverPath, string codiceAmm, bool update, ref int utInseriti, ref int utAggiornati)
        {
            bool result = true;

            OleDbConnection xlsConn = new OleDbConnection();
            OleDbDataReader xlsReader = null;

            try
            {
                if (!Directory.Exists(serverPath + "\\Modelli\\Import\\"))
                    Directory.CreateDirectory(serverPath + "\\Modelli\\Import\\");
            }
            catch (Exception e)
            {
                return false;
            }

            DocsPaDB.Utils.SimpleLog sl = new DocsPaDB.Utils.SimpleLog(serverPath + "\\Modelli\\Import\\logImportUtenti");

            try
            {



                //Controllo se esiste la Directory "Import" nel path dove vengono salvati i modelli per la profilazione dinamica.
                //Se esiste copio il file excel li' dentro, altrimenti la creo e ci copio il file.
                //In ogni caso poichè il nome del file è fisso, anche se quest'ultimo esiste viene sovrascritto.
                logger.Debug("Metodo \"importaUtenti\" classe \"UserManager\" : inizio scrittura file \"importUtenti.xls\"");

                sl.Log("Inizio scrittura file di import in cartella temporanea server");



                try
                {
                    FileStream fs1 = new FileStream(serverPath + "\\Modelli\\Import\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs1.Write(dati, 0, dati.Length);
                    fs1.Close();
                }
                catch (Exception e)
                {
                    sl.Log("Errore nella creazione del file temporaneo. Dettaglio eccezione: " + e.ToString());
                    return false;
                }

                sl.Log("Fine scrittura file import in cartella temporanea");

                logger.Debug("Metodo \"importaUtenti\" classe \"UserManager\" : fine scrittura file \"importUtenti.xls\"");

                logger.Debug("Metodo \"importaUtenti\" classe \"UserManager\" : inizio lettura file \"importUtenti.xls\"");

                sl.Log("Inizio importazione utenti - " + System.DateTime.Now.ToString());
                sl.Log("Apertura file di importazione");

                try
                {
                    //Comincio la lettura del file appena scritto
                 //   xlsConn.ConnectionString = "Provider=" + provider + "Data Source=" + serverPath + "\\Modelli\\Import\\" + nomeFile + ";Extended Properties="+extender+"";
                    xlsConn.ConnectionString = "Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source=" + serverPath + "\\Modelli\\Import\\" + nomeFile + ";Extended Properties='" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "IMEX=1';";
                    xlsConn.Open();
                }
                catch (Exception e)
                {
                    sl.Log("Errore durante l'apertura del file temporaneo. Dettaglio eccezione: " + e.Message);
                    return false;
                }
                OleDbCommand xlsCmd;

                sl.Log("File di importazione aperto");

                // Interrogo il foglio excel per selezionare tutti gli
                // utenti con il campo storicizza diverso da null
                //xlsCmd = new OleDbCommand("select count([Storicizza$]) from [Utenti$] where ([Storicizza$] != null)", xlsConn);

                //int num = Int32.Parse(xlsCmd.ExecuteScalar().ToString());
                sl.Log("Selezione degli utenti da importare");

                try
                {

                    xlsCmd = new OleDbCommand("select * from [Utenti$]", xlsConn);
                    xlsReader = xlsCmd.ExecuteReader();
                }
                catch (Exception e)
                {
                    sl.Log("Errore durante la selezione degli utenti da importare. Dettaglio eccezione: " + e.Message);
                    return false;
                }

                string idAmministrazione = string.Empty;

                while (xlsReader.Read())
                {

                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReader, 0) == "/")
                        break;

                    //Controllo che il codice dell'amministrazione è uguale a quello della amministrazione
                    //dalla quali si sta effettuando l'importazione degli utenti
                    if (codiceAmm.ToUpper() == get_string(xlsReader, 1).ToUpper())
                    {
                        //Verifico che i campi obbligatori "USER_ID - CODICE AMMINISTRAZIONE - NOME - COGNOME" siano presenti
                        //nel foglio excel, altrimenti ingnoro l'inserimento
                        if (get_string(xlsReader, 0) != "" && get_string(xlsReader, 1) != "" && get_string(xlsReader, 2) != "" && get_string(xlsReader, 3) != "")
                        {
                            //Per quanto riguarda il controllo dell'esistenza di un utente con questi dati,
                            //demando il tutto al metodo "AmmInsNuovoUtente", che trovando eventuali ripetizioni non procede all'inserimento
                            DocsPaVO.amministrazione.OrgUtente utente = new DocsPaVO.amministrazione.OrgUtente();
                            utente.UserId = get_string(xlsReader, 0).ToUpper();
                            utente.CodiceRubrica = get_string(xlsReader, 0).ToUpper();
                            utente.Codice = utente.CodiceRubrica;
                            //Io dal foglio ho il codice dell'amministrazione ma l'utente ha bisogno dell'ID
                            //Quindi effettuo una query per recuperarlo
                            utente.IDAmministrazione = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(get_string(xlsReader, 1).ToUpper());
                            idAmministrazione = utente.IDAmministrazione;

                            utente.Nome = get_string(xlsReader, 2);
                            utente.Cognome = get_string(xlsReader, 3);
                            utente.Dominio = get_string(xlsReader, 4);
                            utente.Password = get_string(xlsReader, 5);
                            utente.Sede = get_string(xlsReader, 7);
                            utente.Email = get_string(xlsReader, 8);
                            utente.NotificaTrasm = get_string(xlsReader, 9);
                            utente.Abilitato = get_string(xlsReader, 10);
                            utente.Amministratore = get_string(xlsReader, 11);

                            //logger.Debug("Metodo \"importaUtenti\" classe \"UserManager\" : inserimento utente : "+utente.UserId.ToString()); 
                            DocsPaVO.amministrazione.EsitoOperazione esito = BusinessLogic.Amministrazione.OrganigrammaManager.AmmInsNuovoUtente(infoUtente, utente);

                            if (esito.Codice == 1 || esito.Codice == 2)
                            {
                                if (update)
                                {
                                    //logger.Debug("Metodo \"importaUtenti\" classe \"UserManager\" : modifica utente : " + utente.UserId.ToString());
                                    DocsPaVO.utente.Utente ut = UserManager.getUtente(utente.UserId, idAmministrazione);
                                    DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
                                    DocsPaVO.utente.Corrispondente corr = u.GetCorrispondenteByCodRubrica(idAmministrazione, utente.CodiceRubrica, DocsPaVO.addressbook.TipoUtente.INTERNO);
                                    if (ut.idPeople != null && corr.systemId != null)
                                    {
                                        utente.IDAmministrazione = idAmministrazione;
                                        utente.IDPeople = ut.idPeople;
                                        utente.IDCorrGlobale = corr.systemId;
                                        BusinessLogic.Amministrazione.OrganigrammaManager.AmmModUtente(infoUtente, utente);
                                        utAggiornati++;
                                        sl.Log("");
                                        sl.Log("Utente Aggiornato - UserId: " + utente.UserId + " - Nome: " + utente.Nome + " - Cognome: " + utente.Cognome);
                                    }
                                }
                            }
                            else
                            {
                                if (esito.Codice != -1 || esito.Codice != 1 || esito.Codice != 2)
                                {
                                    utInseriti++;
                                    sl.Log("");
                                    sl.Log("Utente Inserito - UserId: " + utente.UserId + " - Nome: " + utente.Nome + " - Cognome: " + utente.Cognome);
                                }
                                else if (esito.Codice == -1)
                                {
                                    sl.Log("");
                                    sl.Log("Utente non Inserito - UserId: " + utente.UserId + " - Nome: " + utente.Nome + " - Cognome: " + utente.Cognome);


                                }
                            }
                        }
                    }
                }
                sl.Log("");
                sl.Log("Fine importazione utenti - " + System.DateTime.Now.ToString());
                sl.Log("Utenti Inseriti : " + utInseriti + " - Utenti Aggiornati : " + utAggiornati);
                logger.Debug("Metodo \"importaUtenti\" classe \"UserManager\" : fine lettura file \"importUtenti.xls\"");
            }
            catch (Exception ex)
            {
                sl.Log("Errore durante l'importazione: " + ex.Message);
                logger.Debug("Metodo \"importaUtenti\" classe \"UserManager\" ERRORE : " + ex.Message);
                result = false;
                return result;
            }
            finally
            {
                sl.Log("Chiusura file temporaneo.");
                xlsReader.Close();
                xlsConn.Close();
            }

            return result;
        }

        private static string get_string(OleDbDataReader dr, int field)
        {
            if (dr[field] == null || dr[field] == System.DBNull.Value)
                return "";
            else
                return dr[field].ToString();
        }

        public static ArrayList getLogImportUtenti(string serverPath)
        {
            ArrayList fileLog = new ArrayList();
            string sLine = string.Empty;

            try
            {
                StreamReader objReader = new StreamReader(serverPath + "\\Modelli\\Import\\logImportUtenti.log");
                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        fileLog.Add(sLine);
                }
                objReader.Close();

                return fileLog;
            }
            catch (Exception e)
            {
                logger.Debug("Metodo \"getLogImportUtenti\" classe \"UserManager\" ERRORE : " + e.Message);
                return fileLog;
            }
        }

        #endregion

        //metodo per la gestione dell'ingresso a DocsPA dal portale
        public string removeVisibilita(string docNumber, DocsPaVO.utente.Corrispondente corr)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            return utenti.RemoveVisibilita(docNumber, corr);
        }

        public static ArrayList getListaCorrispondentiByDescrizione(string descrizione, string tipoIE, string idAmm)
        {
            ArrayList result = null;

            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();

            result = u.GetListaCorrispondentiByDescrizione(idAmm, descrizione, tipoIE);

            return result;

        }

        public string getCssAmministrazione(string idAmm)
        {
            string result = string.Empty;
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            result = u.GetCssAmministrazione(idAmm);
            return result;
        }

        public string getSegnAmm(string idAmm)
        {
            string result = string.Empty;
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            result = u.GetSegnAmm(idAmm);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool isUserDisabled(string userName, string idAmm)
        {
            return isUserDisabled(userName, string.Empty, idAmm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="modulo"></param>
        /// <returns></returns>
        public static bool isUserDisabled(string userName, string modulo, string idAmm)
        {
            bool result = false;

            using (DocsPaDB.Query_DocsPAWS.Utenti userDb = new DocsPaDB.Query_DocsPAWS.Utenti())
            {
                if (DocsPaUtils.Moduli.ModuliAuthManager.IsModuloCentroServizi(modulo))
                {
                    // Autenticazione al modulo centro servizi di conservazione
                    result = userDb.IsUtenteDisabledCentroServizi(userName);
                }
                else
                {
                    // Autenticazione standard
                    result = userDb.IsUtenteDisabled(userName, idAmm);
                }
            }

            return result;
        }


        /// <summary>
        /// Reperimento token di autenticazione per il superutente nell'ambito del documentale corrente
        /// </summary>
        /// <returns></returns>
        public static string getSuperUserAuthenticationToken()
        {
            DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

            return userManager.GetSuperUserAuthenticationToken();
        } 

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByIdPeople(string idPeople, DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.utente.InfoUtente user)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.GetCorrispondenteByIdPeople(user.idAmministrazione, idPeople, tipoIE);
        }
        public static string getRuoloRespUofromUo(string id_Uo, string tipoRuolo, string idCorr)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.getRuoloRespUoFromUo(id_Uo, tipoRuolo, idCorr);
        }

        public static string getCodiceCorrispondente(string docnumber, string idOggettoCustom, string idTemplate)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.getCodiceCorrispondente(docnumber, idOggettoCustom, idTemplate);
        }


        public static DocsPaVO.utente.Corrispondente getCorrispondenteByCodRubricaRubricaComune(string codice, DocsPaVO.utente.InfoUtente user)
        {

            DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
            if (RubricaComune.Configurazioni.GetConfigurazioni(user).GestioneAbilitata)
            {
                // Se il corrispondente non è stato trovato in docspa,
                // viene effettuata la ricerca del corrispondente in rubrica comune
                logger.Debug("cerco Rubrica Comune");
                if (!string.IsNullOrEmpty(codice))
                    corr = RubricaComune.RubricaServices.UpdateCorrispondente(user, codice);
            }

            return corr;
        }
        public static DocsPaVO.utente.Corrispondente getCorrispondenteByEmailAndDescrizione(string mail, DocsPaVO.utente.InfoUtente user, DocsPaVO.utente.Registro reg, string descrizione)
        {
            return getCorrispondenteByEmailAndDescrizione(mail, user, reg.systemId, descrizione);
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByEmailAndDescrizione(string mail, DocsPaVO.utente.InfoUtente user, DocsPaVO.utente.Registro reg, string descrizione,out string rows, string codiceAmm)
        {
            return getCorrispondenteByEmailAndDescrizione(mail, user, reg.systemId, descrizione, out rows, codiceAmm);
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByEmailAndDescrizione(string mail, DocsPaVO.utente.InfoUtente user, string id_reg, string descrizione)
        {
            logger.Debug("getCorrispondenteByEmailAndDescrizione start");
            
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaVO.utente.Corrispondente corr = u.GetCorrispondenteByEmailAndDescrizione(user.idAmministrazione, mail, id_reg, descrizione);
            
            if (RubricaComune.Configurazioni.GetConfigurazioni(user).GestioneAbilitata)
            {
                if (corr == null)
                {
                    // Se il corrispondente non è stato trovato in docspa,
                    // viene effettuata la ricerca del corrispondente in rubrica comune
                    if (!string.IsNullOrEmpty(mail) && !string.IsNullOrEmpty(descrizione))
                        corr = RubricaComune.RubricaServices.UpdateCorrispondenteByEmailAndDescrizione(user, mail, descrizione);
                }
                else
                    if (string.IsNullOrEmpty(corr.systemId) || corr.inRubricaComune)
                    {
                        corr = RubricaComune.RubricaServices.UpdateCorrispondente(user, corr);
                    }
            }

            return corr;
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByEmailAndDescrizione(string mail, DocsPaVO.utente.InfoUtente user, string id_reg, string descrizione,out string righe, string codiceAmm)
        {
            logger.Debug("getCorrispondenteByEmailAndDescrizione start");
            string rows = "";
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaVO.utente.Corrispondente corr = u.GetCorrispondenteByEmailAndDescrizione(user.idAmministrazione, mail, id_reg, descrizione,out rows, codiceAmm);
            righe = rows;
            if (RubricaComune.Configurazioni.GetConfigurazioni(user).GestioneAbilitata)
            {
                if (corr == null)
                {
                    
                    // Se il corrispondente non è stato trovato in docspa,
                    // viene effettuata la ricerca del corrispondente in rubrica comune
                    if (!string.IsNullOrEmpty(mail) && !string.IsNullOrEmpty(descrizione))
                        corr = RubricaComune.RubricaServices.UpdateCorrispondenteByEmailAndDescrizione(user, mail,descrizione);
                }
                else
                    if (string.IsNullOrEmpty(corr.systemId) || corr.inRubricaComune)
                    {
                        corr = RubricaComune.RubricaServices.UpdateCorrispondente(user, corr);
                    }
            }
            
            return corr;
        }

        public static DocsPaVO.utente.Ruolo getRuoloByIdGruppo(string idGruppo)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
                    DocsPaVO.utente.Ruolo ruoloResult = ut.GetRuoloByIdGruppo(idGruppo);
                    transactionContext.Complete();
                    return ruoloResult;                    
                }
                catch (Exception e)
                {
                    logger.Debug("Errore in UserManager  - metodo: getRuoloByIdGruppo", e);
                    return null;
                }
            }
        }

        /// <summary>
        /// Funzione che dato un corrispondente di tipo ruolo ritorna un dataset con la lista degli utenti del ruolo se sottoposto al ruolo dell'utente
        /// </summary>
        /// </returns>
        public static DataSet getUtentiInRuoloSottoposto(DocsPaVO.utente.InfoUtente infoUtente, string corrispondente)
        {
            DataSet ds = new DataSet();
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

           // DocsPaVO.utente.Ruolo newRuolo = utenti.GetRuolo(corrispondente);

            DocsPaVO.utente.Ruolo newRuolo = utenti.getRuoloById(corrispondente);

            DocsPaVO.utente.Ruolo mioRuolo = utenti.GetRuoloByIdGruppo(infoUtente.idGruppo);

            ArrayList listaRuoliInf;

            DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();

            DocsPaVO.trasmissione.TipoOggetto tipo = new DocsPaVO.trasmissione.TipoOggetto();

            listaRuoliInf = gerarchia.getGerarchiaInf(mioRuolo, null, null, tipo);

            DocsPaVO.utente.Ruolo[] tmpRuolo = (DocsPaVO.utente.Ruolo[])listaRuoliInf.ToArray(typeof(DocsPaVO.utente.Ruolo));

            DocsPaVO.utente.Ruolo ruo = tmpRuolo.Where(e => e.idGruppo == newRuolo.idGruppo).FirstOrDefault();

            if (ruo != null || mioRuolo.systemId.Equals(corrispondente))
            {
                ds = utenti.getUtentiInRuoloSottoposto(infoUtente, newRuolo.idGruppo);
            }
            else
            {
                ds = null;
            }

            return ds;
        }

        public static DocsPaVO.utente.Corrispondente getDettagliIndirizzoCorrispondente(string systemId)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
            return utente.getDettagliIndirizzoCorrispondente(systemId);
        }

        public static List<string> getListaEmailUtentiAmm(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
            return ut.getListaEmailUtentiAmm(idAmm);
        }

        
        //metodo per il recupero dei titoli dei corrispondenti da visualizzare in rubrica
        public static ArrayList getListaTitoli()
        {
            DocsPaDB.Query_DocsPAWS.Utenti utente = new DocsPaDB.Query_DocsPAWS.Utenti();
            return utente.getListaTitoli();
        }

        /// <summary>
        /// Metodo per la ricostruzione dell'info utente a partire da informazioni su ruolo e utente
        /// </summary>
        /// <param name="user">Oggetto da cui prelevare le informaizoni di base sull'utente</param>
        /// <param name="role">Se null, non vengono riempite le informazioni sul ruolo</param>
        /// <returns></returns>
        public static DocsPaVO.utente.InfoUtente GetInfoUtente(DocsPaVO.utente.Utente user, DocsPaVO.utente.Ruolo role)
        {
            DocsPaVO.utente.InfoUtente retVal = new DocsPaVO.utente.InfoUtente();

            retVal.idPeople = user.idPeople;
            retVal.dst = user.dst;
            retVal.idAmministrazione = user.idAmministrazione;
            retVal.userId = user.userId;
            retVal.sede = user.sede;
            if (user.urlWA != null)
                retVal.urlWA = user.urlWA;
            //retVal.delegato = us ????
            if (role != null)
            {
                retVal.idCorrGlobali = role.systemId;
                retVal.idGruppo = role.idGruppo;
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per il recupero della storia di un ruolo
        /// </summary>
        /// <param name="request">Request con le informazioni sul ruolo di cui recuperare la storia</param>
        /// <returns>Response con la storia del ruolo</returns>
        public static DocsPaVO.utente.RoleHistoryResponse GetRoleHistory(DocsPaVO.utente.RoleHistoryRequest request)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();

            return new DocsPaVO.utente.RoleHistoryResponse()
                {
                    RoleHistoryItems = users.GetRoleHistory(request.IdCorrGlobRole)
                };
        }

        /// <summary>
        /// Metodo per verificare un dato ruolo è storicizzato
        /// </summary>
        /// <param name="authorCorrGlob">Id corr globale del ruolo da verificare</param>
        /// <returns>True se il ruolo è storicizzato</returns>
        public static bool CheckIfRoleDisabled(String authorCorrGlob)
        {
            using (DocsPaDB.Query_DocsPAWS.Utenti usersManager = new DocsPaDB.Query_DocsPAWS.Utenti())
            {
                return usersManager.CheckIfRoleDisabled(authorCorrGlob);
            }
        }

        /// <summary>
        /// Metodo per il recupero della lista degli id corr globali relativi alla
        /// catena di storicizzazione di un ruolo
        /// </summary>
        /// <param name="idCorrGloRole">Id del ruolo di cui ricostruire la catena di id</param>
        /// <returns>Lista degli id dei ruoli della catena</returns>
        public static List<String> GetRoleChain(String idCorrGloRole)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            return users.GetRoleChain(idCorrGloRole);
        }


        /// <summary>
        /// Metodo per resettare il campo VAR_INSERT_INTEROP su DPA_CORR_GLOBALI
        /// </summary>
        /// <param name="corrSystemId">Id del corrispondente</param>
        public static int resetCorrVarInsertIterop(string corrSystemId, string val)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            return users.resetCorrVarInsertIterop(corrSystemId,val);
        }

        /// <summary>
        /// Metodo per resettare il campo Cod_Rubrica su DPA_CORR_GLOBALI
        /// </summary>
        /// <param name="corrSystemId">Id del corrispondente</param>
        public static int resetCodRubCorrIterop(string corrSystemId, string val)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            return users.resetCodRubCorrIterop(corrSystemId, val);
        }

        /// <summary>
        /// Metodo per il recupero di tutti i corrispondenti per una data mail e registro
        /// </summary>
        /// <param name="email"></param>
        /// <param name="idRegistri"></param>
        /// <returns></returns>
        public static DataSet GetCorrByEmail(string email,string idRegistri)
        {
            DataSet ds = new DataSet();
            string[] idRegs = null;
            string idamm = string.Empty;
            Registro reg = null;
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            if (!string.IsNullOrEmpty(idRegistri))
            {
                if (idRegistri.Contains(','))
                {
                    idRegs = idRegistri.Split(',');
                }
                else
                {
                    idRegs = new string[] { idRegistri };
                }
            }
            if (idRegs != null && idRegs.Length > 0)
            {
                reg = BusinessLogic.Utenti.RegistriManager.getRegistro(idRegs[0].Replace("'", ""));
                idamm = reg.idAmministrazione;
            }
            users.GetCorrByEmail(email, idRegistri, idamm, out ds);

            return ds;
        }

        /// <summary>
        /// Metodo per il recupero di tutti i corrispondenti per una data mail e registro
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="descr">Descrizione</param>
        /// <param name="idRegistri">Registro</param>
        /// <returns>Dataset</returns>
        public static DataSet GetCorrByEmailAndDescr(string email,string descr, string idRegistri)
        {
            DataSet ds = new DataSet();
            string[] idRegs = null;
            string idAmm = string.Empty;
            Registro reg = null;
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            if (!string.IsNullOrEmpty(idRegistri))
            {
                if (idRegistri.Contains(','))
                {
                    idRegs = idRegistri.Split(',');
                }
                else
                {
                    idRegs = new string[] { idRegistri };
                }
            }
            if (idRegs != null && idRegs.Length > 0)
            {
                reg = BusinessLogic.Utenti.RegistriManager.getRegistro(idRegs[0].Replace("'", ""));
                idAmm = reg.idAmministrazione;
            }
            users.GetCorrByEmailAndDescr(email, descr, idRegistri, idAmm, out ds);

            return ds;
        }

        /// <summary>
        /// Verifica se esiste almeno un corrispondente nell'amministrazione del corrispondente passato in input al metodo
        /// </summary>
        /// <param name="corr"></param>
        /// <returns></returns>
        public static bool ExistCorrInAmm(Corrispondente corr)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            return users.GetNumCorrInAMM(corr.codiceRubrica, corr.idAmministrazione, corr.codiceAmm);
        }

        /// <summary>
        /// Metodo per aggiornate il mittente dei documenti di un corrispondente creato da interop
        /// </summary>
        /// <param name="oldCorrId">Vecchio Id mittente</param>
        /// <param name="newCorrId">Nuovo Id mittente</param>
        /// <param name="rows">Righe aggiornate</param>
        /// <return></return>
        
        public static void updateDocArrivoFromInterop(string oldCorrId, string newCorrId, out int rows)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            users.updateDocArrivoFromInterop(oldCorrId,newCorrId,out rows);
        }


        /// <summary>
        /// Metodo per aggiornate il mittente del documenti di un corrispondente creato da interop
        /// </summary>
        /// <param name="oldCorrId">Id del documento</param>
        /// <param name="newCorrId">Nuovo Id mittente</param>
        /// <param name="rows">Righe aggiornate</param>
        /// <return></return>

        public static void updateDocArrivoFromInteropOccasionale(string docId, string newCorrId, out int rows)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            users.updateDocArrivoFromInteropOccasionale(docId, newCorrId, out rows);
        }


        /// <summary>
        /// Metodo per il recupero della lista degli id corr globali relativi alla
        /// catena di storicizzazione di un ruolo
        /// </summary>
        /// <param name="idCorrGloRole">Id del ruolo di cui ricostruire la catena di id</param>
        /// <returns>Lista degli id dei ruoli della catena</returns>
        public static Utente[] getUserInRoleByIdCorrGlobali(string idCorrGloRole)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            return users.getUserInRoleByIdCorrGlobali(idCorrGloRole);
        }

        public static Utente[] getUserInRoleByIdGruppo(string idGruppo)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            return users.getUserInRoleByIdGruppo(idGruppo);
        }

        /// <summary>
        /// Metodo per il recupero delle informazioni minimali sugli utente di un ruolo
        /// </summary>
        /// <param name="roleId">Id del ruolo</param>
        /// <returns>Lista degli utenti dei ruolo</returns>
        public static List<UserMinimalInfo> GetUsersInRoleMinimalInfo(string roleId)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            return users.GetUsersInRoleMinimalInfo(roleId);
        }

        public static String GetRoleDescriptionByIdGroup(String idGroup)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            return users.GetRoleDescriptionByIdGroup(idGroup);

        }

        public static String GetRoleDescriptionByIdCorrGlobali(String idCorrGlobali)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            return users.GetRoleDescriptionByIdCorrGlobali(idCorrGlobali);
        }

        public static List<DocsPaVO.utente.Utente> GetUtentiByCodQualifica(String codQualifica)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utDb = new DocsPaDB.Query_DocsPAWS.Utenti();
            return utDb.GetUtentiByCodQualifica(codQualifica);
        }

        public static string GetDescOldByCorr(string systemId)
        {
            string descOld = string.Empty;
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            return users.GetDescOldByCorr(systemId);
        }
        public static string countTipoFromTipologia(string tipoOggetto, string tipologiaDoc)
        {
            DocsPaDB.Query_Utils.Utils utility = new Utils();
            string ris = string.Empty;
            ris = utility.countTipoFromTipologia(tipoOggetto, tipologiaDoc);
            return ris;
        }

        public static string countTipoFromTipologiaFasc(string tipoOggetto, string idTipoFasc)
        {
            DocsPaDB.Query_Utils.Utils utility = new Utils();
            string ris = string.Empty;
            ris = utility.countTipoFromTipologiaFasc(tipoOggetto, idTipoFasc);
            return ris;
        }

        public static string countDeferimenti(string from, string to, string ufficio, string idTipoAtto)
        {
            DocsPaDB.Query_Utils.Utils utility = new Utils();
            string ris = string.Empty;
            ris = utility.countDeferimenti(from, to, ufficio, idTipoAtto);
            return ris;
        }

        public static string countDecretiEsaminati(string from, string to, string ufficio)
        {
            DocsPaDB.Query_Utils.Utils utility = new Utils();
            string ris = string.Empty;
            ris = utility.countDecretiEsaminati(from, to, ufficio);
            return ris;
        }

        public static DataSet getRegistriByTipologia(string desc_tipologia)
        {
            DocsPaDB.Query_Utils.Utils utility = new Utils();
            DataSet ris = new DataSet();
            ris = utility.getRegistriByTipologia(desc_tipologia);
            return ris;
        }

        public static DataSet getRuoliByRegistro(string idRegistro)
        {
            DocsPaDB.Query_Utils.Utils utility = new Utils();
            DataSet ris = new DataSet();
            ris = utility.getRuoliByRegistro(idRegistro);
            return ris;
        }

        public static string getCorrFromElencoDecreti(InfoUtente userInfo, List<DocsPaVO.filtri.FiltroRicerca> searchFilters, string query, out int numRows)
        {
            string ris = string.Empty;
            numRows = 0;
            DataSet ds = new DataSet();
            switch (query)
            {
                case "CDC_REPORT_ELENCO_DECRETI_RESTITUITI_SRC_2":
                    DocsPaDB.Query_DocsPAWS.Reporting.ElencoDecretiRestituitiSRCReport eleResSRC = new ElencoDecretiRestituitiSRCReport();
                    ds = eleResSRC.GetElencoDescreti(userInfo, searchFilters, out numRows);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        ris = dr["corr"] as string;
                    }
                    break;
                case "CDC_REPORT_ELENCO_DECRETI_RESTITUITI_SCCLA_2":
                    DocsPaDB.Query_DocsPAWS.Reporting.ElencoDecretiRestituitiSCCLAReport eleResSCCLA = new ElencoDecretiRestituitiSCCLAReport();
                    ds = eleResSCCLA.GetElencoDescreti(userInfo, searchFilters, query, out numRows);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        ris = dr["corr"] as string;
                    }
                    break;
                case "CDC_REPORT_ELENCO_DECRETI_RESTITUITI_CON_RILIEVO_SRC_2":
                    DocsPaDB.Query_DocsPAWS.Reporting.ElencoDecretiRestituitiConRilievoSRCReport eleResRilSRC = new ElencoDecretiRestituitiConRilievoSRCReport();
                    ds = eleResRilSRC.GetElencoDescreti(userInfo, searchFilters, out numRows);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        ris = dr["corr"] as string;
                    }
                    break;
                case "CDC_REPORT_ELENCO_DECRETI_RESTITUITI_CON_RILIEVO_SCCLA_2":
                    DocsPaDB.Query_DocsPAWS.Reporting.ElencoDecretiRestituitiConRilievoSCCLAReport eleResRilSCCLA = new ElencoDecretiRestituitiConRilievoSCCLAReport();
                    ds = eleResRilSCCLA.GetElencoDescreti(userInfo, searchFilters, query, out numRows);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        ris = dr["corr"] as string;
                    }
                    break;
                case "CDC_REPORT_ELENCO_DECRETI_SCCLA_2":
                    DocsPaDB.Query_DocsPAWS.Reporting.ElencoDecretiSCCLAReport ele = new ElencoDecretiSCCLAReport();
                    ds = ele.GetElencoDescreti(userInfo, searchFilters, query, out numRows);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        ris = dr["corr"] as string;
                    }
                    break;
                case "CDC_REPORT_ELENCO_DECRETI_SRC_2":
                    DocsPaDB.Query_DocsPAWS.Reporting.ElencoDecretiSRCReport eleSRC = new ElencoDecretiSRCReport();
                    ds = eleSRC.GetElencoDescreti(userInfo, searchFilters, query, out numRows);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        ris = dr["corr"] as string;
                    }
                    break;
            }

            return ris;
        }

        public static DataSet foundKeyWord(string word, string idRegistro)
        {
            DocsPaDB.Query_Utils.Utils utility = new Utils();
            DataSet ris = new DataSet();
            ris = utility.foudKeyWord(word, idRegistro);
            return ris;
        }

        public static DocsPaVO.utente.Ruolo getRuoloByCodice(string codice)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.getRuoloByCodice(codice);
        }

        /// <summary>
        /// legge un utente identificato per nome e id amministrazione. Restituisce un oggetto Utente
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userName"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Utente getUtenteByCodice(string userName, string codiceAmm)
        {

            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            return utenti.getUtenteByCodice(userName, codiceAmm);
        }

        /// <summary>
        /// Torna l'utente automatico per l'amministrazione
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Utente GetUtenteAutomatico(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            return utenti.GetUtenteAutomatico(idAmm);
        }

        public static Canale GetDatiCanPref_Experimental(Corrispondente corr)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.GetDatiCanPref(corr);
        }


        /// <summary>
        /// Metodo per il recupero di un attributo di un corrispondente attivo a partire dal suo codice
        /// </summary>
        /// <param name="codCorr">Codice del corrispondente</param>
        /// <param name="corrAttribute">Atrributo da recuperare</param>
        /// <returns>Attributo richiesto per il corrispondente</returns>
        public static String GetInternalCorrAttributeByCorrCode(String codCorr, DocsPaDB.Query_DocsPAWS.Utenti.CorrAttribute corrAttribute, String adminId)
        {
            return new DocsPaDB.Query_DocsPAWS.Utenti().GetInternalCorrAttributeByCorrCode(codCorr, corrAttribute, adminId);
        
        }

        /// <summary>
        /// Restituisce la lista dei 'destinatari' / 'destinatari in cc' associati ad un documento con il relativo canale preferenziale
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="typeDest">Può assumere D(estrai le info sui destinatari), C(estrati le info sui destinatari in CC)</param>
        /// <returns>List</returns>
        public static List<DocsPaVO.utente.Corrispondente> GetPrefChannelAllDest(string idProfile, string typeDest)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.GetPrefChannelAllDest(idProfile, typeDest);
        }

        /// <summary>
        /// Restituisce la lista degli id opportunity
        /// </summary>
        /// <param name="codiceU"></param>      
        /// <returns>List</returns>
        public static List<string> getIdOpportunityList(string codiceU)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.GetIdOpportunityList(codiceU);
        }

        #region gestione memento

        /// <summary>
        /// legge il memento per un prticolare IDpeople
        /// </summary>
        /// <param name="db"></param>
        /// <param name="idPeople"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public static string[] getMementoUtente(InfoUtente infoutente)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            string idPeople = infoutente.delegato != null && !string.IsNullOrEmpty(infoutente.delegato.idPeople) ? infoutente.delegato.idPeople : infoutente.idPeople;
            return utenti.GetMementoUtente (idPeople, infoutente.idAmministrazione);
        }

        public static bool setMementoUtente(InfoUtente infoutente,string dominio, string alias)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            string idPeople = infoutente.delegato != null && !string.IsNullOrEmpty(infoutente.delegato.idPeople) ? infoutente.delegato.idPeople : infoutente.idPeople;
            return utenti.SetMementoUtente(idPeople, infoutente.idAmministrazione, dominio,alias);
        }

        #endregion 

        #region HSMPIN

        public static HSMPin selectHSMPin(string idPeople, string idAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            /* D.O. 14/12/2016
             * Il pin che viene estratto dal DB non è stato ancora decriptato!
             */
            return u.SelectHSMPin(idPeople, idAmministrazione);
        }

        public static bool insertHSMPin(HSMPin hsmPin)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            String encryptedPin = null;
            CryptoString crypto = new CryptoString(hsmPin.idPeople);
            encryptedPin = crypto.Encrypt(hsmPin.pin);
            hsmPin.pin = encryptedPin;
            return u.InsertHSMPin(hsmPin);
        }

        public static bool updateHSMPin(HSMPin hsmPin)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            String encryptedPin = null;
            CryptoString crypto = new CryptoString(hsmPin.idPeople);
            encryptedPin = crypto.Encrypt(hsmPin.pin);
            hsmPin.pin = encryptedPin;
            return u.UpdateHSMPin(hsmPin);
        }

        public static bool deleteHSMPin(String idPeople, String idAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.DeleteHSMPin(idPeople, idAmministrazione);
        }
        #endregion HSMPIN

        #region HSMPARAMETERS

        public static HSMParameters selectHSMAmministrationParameters(string idAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.SelectHSMParameters(idAmministrazione);
        }
        #endregion HSMPARAMETERS
    }
}
