using System;
using System.Collections;
using System.Data;
using DocsPaDB;
using DocsPaUtils.Interfaces.DbManagement;
using System.Collections.Generic;
using DocsPaDbManagement.Functions;
using log4net;

namespace BusinessLogic.Amministrazione
{
    /// <summary>
    /// </summary>
    public class UtenteManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(UtenteManager));
        /// <summary></summary>
        /// <param name="utente"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static ArrayList GetRuoliUtente(DocsPaVO.utente.Utente utente)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            ArrayList result = amm.getRuoliUtente(utente);

            return result;
        }

        /// <summary>
        /// Restituisce una Hashtable per una data amministrazione
        /// in cui la chiave è il codice dell'utente e il valore è un array
        /// di codici rubrica dei ruoli a cui questo utente appartiene
        /// </summary>
        /// <param name="id_amm"></param>
        /// <returns></returns>
        public static Hashtable GetRuoliUtenteSemplice(string id_amm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.GetRuoliUtenteSemplice(id_amm);
        }

        /// <summary></summary>
        /// <param name="ruoliAdd"></param>
        /// <param name="ruoliRemove"></param>
        /// <param name="utente"></param>
        /// <param name="infoUtente"></param>
        public static void SetRuoliUtente(DocsPaVO.utente.Ruolo[] ruoliAdd, DocsPaVO.utente.Ruolo[] ruoliRemove, DocsPaVO.utente.Utente utente)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            if (amm.setRuoliUtente(ruoliAdd, ruoliRemove, utente))
            {
                throw new Exception();
            }
        }

        /// <summary></summary>
        /// <param name="corr"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Utente GetInfoUtente(DocsPaVO.utente.Corrispondente corr)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.utente.Utente utente = amm.GetInfoUtente((DocsPaVO.utente.Utente)corr);

            if (utente == null)
            {
                throw new Exception();
            }

            return utente;
        }

        /// <summary></summary>
        /// <param name="idAmm"></param>
        /// <param name="search"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        public static ArrayList GetUtenti(string idAmm, string search)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            ArrayList result = amm.getUtenti(idAmm, search);

            return result;
        }

        /// <summary></summary>
        /// <param name="utente"></param>
        /// <param name="newPWD"></param>
        /// <param name="oldPWD"></param>
        /// <param name="infoUtente"></param>
        public static void UpdateUserPassword(DocsPaVO.utente.Utente utente, string newPWD, string oldPWD)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            if (!amm.updateUserPassword(utente, newPWD, oldPWD))
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Lista Registri associati all'utente amministratore per la gestione del titolario
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public static ArrayList AmmGetListRegistriUtente(string idAmm, string idCorrGlob)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListRegAssUtenteAdmin(idAmm, idCorrGlob);
            dbAmm = null;

            DocsPaVO.amministrazione.OrgRegistro registro = null;

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_REGISTRI_LIST"].Rows)
                {
                    registro = new DocsPaVO.amministrazione.OrgRegistro();

                    registro.IDRegistro = row["IDREGISTRO"].ToString();
                    registro.Codice = row["CODICE"].ToString();
                    registro.Descrizione = row["DESCRIZIONE"].ToString();
                    registro.Associato = row["ASSOCIATO"].ToString();

                    retValue.Add(registro);

                    registro = null;
                }
            }

            return retValue;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmInsRegistriUtenteAdmin(DocsPaVO.amministrazione.OrgRegistro[] listaRegistri, string idCorrGlob)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            DocsPaUtils.Query queryDef = null;
            string commandText = string.Empty;

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    // prima elimina...
                    esito = AmmEliminaRegistriUtenteAdmin(idCorrGlob);

                    if (esito.Codice.Equals(0))
                    {
                        // ...poi inserisce					
                        foreach (DocsPaVO.amministrazione.OrgRegistro registro in listaRegistri)
                        {
                            queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_REGISTRI_ASS_UTENTE");
                            queryDef.setParam("idCorrGlob", idCorrGlob);
                            queryDef.setParam("idRegistro", registro.IDRegistro);
                            commandText = queryDef.getSQL();
                            logger.Debug(commandText);

                            dbProvider.ExecuteNonQuery(commandText);

                            commandText = null;
                            queryDef = null;
                        }
                    }
                }
                catch
                {
                    esito.Codice = 1;
                    esito.Descrizione = "si è verificato un errore: inserimento associazione registro/utente";
                }
                finally
                {
                    dbProvider.Dispose();
                }
            }

            return esito;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmEliminaRegistriUtenteAdmin(string idCorrGlob)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("D_REGISTRI_ASS_UTENTE");
                    queryDef.setParam("idCorrGlob", idCorrGlob);
                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        esito.Codice = 1;
                        esito.Descrizione = "si è verificato un errore: eliminazione associazione registro/utente";
                    }
                }
                catch
                {
                    esito.Codice = 1;
                    esito.Descrizione = "si è verificato un errore: eliminazione associazione registro/utente";
                }
                finally
                {
                    dbProvider.Dispose();
                }
            }

            return esito;
        }

        public static bool AmmCheckRegAssUtente(string idCorrGlob)
        {
            bool esito = false;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_REGISTRI_ASS_UTENTE");
            queryDef.setParam("idCorrGlob", idCorrGlob);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DBProvider dbProvider = new DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        esito = true;
                    }
                }
            }

            return esito;
        }

        public static bool AmmCheckRespAOO(string idpeople, string idGruppo)
        {
            //List<string> lista = new List<string>();
            bool esito = false;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_UTENTE_RESP_AOO");
            if (!string.IsNullOrEmpty(idGruppo))
                queryDef.setParam("idPeople", idpeople + " AND ID_RUOLO_AOO = " + idGruppo);
            else
                queryDef.setParam("idPeople", idpeople);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DBProvider dbProvider = new DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {

                        esito = true;
                    }

                }
            }

            return esito;
        }

        public static List<string> getAmmRespAOO(string idpeople)
        {
            List<string> listaAOO = new List<string>();
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_UTENTE_RESP_AOO");
            queryDef.setParam("idPeople", idpeople);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DBProvider dbProvider = new DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    while (reader.Read())
                    {
                        string codice = reader["VAR_CODICE"].ToString();

                        listaAOO.Add(codice);

                    }

                }
            }

            return listaAOO;
        }

        public static bool Checkconnection()
        {
            DocsPaDocumentale.Documentale.UserManager usm = new DocsPaDocumentale.Documentale.UserManager();
            return usm.Checkconnection();

        }

        #region gestione user administrator - voci menu  //SABRINA
         public static ArrayList AmmGetListMenuUtente(string idAmm, string idCorrGlob)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            ArrayList retValue = new ArrayList();
            DataSet ds = dbAmm.GetListMenuAssUtenteAdmin(idAmm, idCorrGlob);
            dbAmm = null;

            DocsPaVO.amministrazione.Menu voceMenu = null;

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_MENU_LIST"].Rows)
                {
                    voceMenu = new DocsPaVO.amministrazione.Menu();

                    voceMenu.IDMenu = row["IDMENU"].ToString();
                    voceMenu.Codice = row["CODICE"].ToString();
                    voceMenu.Descrizione = row["DESCRIZIONE"].ToString();
                    voceMenu.Associato = row["ASSOCIATO"].ToString();
                    if (row["VISIBILITA"]!=null)
                        voceMenu.Visibilita = row["VISIBILITA"].ToString();

                    retValue.Add(voceMenu);
                    voceMenu = null;
                }
            }

            return retValue;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmInsMenuUtenteAdmin(DocsPaVO.amministrazione.Menu[] listaMenu, string idCorrGlob, string idAmm)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            DocsPaUtils.Query queryDef = null;
            string commandText = string.Empty;

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    // prima elimina...
                    esito = AmmEliminaMenuUtenteAdmin(idCorrGlob);

                    if (esito.Codice.Equals(0))
                    {
                        // ...poi inserisce		
                        bool isTitolario = false;
                        foreach (DocsPaVO.amministrazione.Menu voceMenu in listaMenu)
                        {
                            queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("I_MENU_ASS_UTENTE");
                            queryDef.setParam("idCorrGlob", idCorrGlob);
                            queryDef.setParam("idVoceMenu", voceMenu.IDMenu);
                            queryDef.setParam("idAmm", idAmm);
                            commandText = queryDef.getSQL();
                            logger.Debug(commandText);

                            dbProvider.ExecuteNonQuery(commandText);
                            if (voceMenu.Codice.ToUpper().Equals("TITOLARIO"))
                                isTitolario = true;
                            commandText = null;
                            queryDef = null;
                        }
                        //se non è presente la voce di titolario elimino eventuali associazioni con i registri
                        if (!isTitolario)
                            AmmEliminaRegistriUtenteAdmin(idCorrGlob);

                    }
                }
                catch
                {
                    esito.Codice = 1;
                    esito.Descrizione = "si è verificato un errore: inserimento associazione menu/utente";
                }
                finally
                {
                    dbProvider.Dispose();
                }
            }

            return esito;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmEliminaMenuUtenteAdmin(string idCorrGlob)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            using (DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("D_MENU_ASS_UTENTE");
                    queryDef.setParam("idCorrGlob", idCorrGlob);
                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    if (!dbProvider.ExecuteNonQuery(commandText))
                    {
                        esito.Codice = 1;
                        esito.Descrizione = "si è verificato un errore: eliminazione associazione menu/utente";
                    }
                }
                catch
                {
                    esito.Codice = 1;
                    esito.Descrizione = "si è verificato un errore: eliminazione associazione menu/utente";
                }
                finally
                {
                    dbProvider.Dispose();
                }
            }

            return esito;
        }

        public static bool AmmCheckMenuAssUtente(string idCorrGlob)
        {
            bool esito = false;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_MENU_ASS_UTENTE");
            queryDef.setParam("idCorrGlob", idCorrGlob);
            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DBProvider dbProvider = new DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        esito = true;
                    }
                }
            }

            return esito;
        } 
        public static string GetDispositivoStampaUtente(string idPeople)
        {

            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            List<DocsPaVO.amministrazione.DispositivoStampaEtichetta> list = amm.GetDispositiviStampaEtichetta();


            string stringa_dispositivo = "";
            DocsPaUtils.Query queryDef_stampautente = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_DISPOSITIVO_STAMPA_UTENTE");
            queryDef_stampautente.setParam("idPeople", idPeople);
            string cm_stampautente = queryDef_stampautente.getSQL();
            logger.Debug(cm_stampautente);
            
            
            
            
            using (DBProvider dbProvider = new DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(cm_stampautente)){
                    while (reader.Read())
                    {
                        if(reader.GetValue(0)!=DBNull.Value)
                            stringa_dispositivo = list.Find(x=>x.Id == int.Parse(reader.GetValue(0).ToString())).Code;
                        else
                            stringa_dispositivo = list.Find(x=>x.Id == int.Parse(reader.GetValue(1).ToString())).Code;
                    }
                }
            }
            return stringa_dispositivo;
        }
        #endregion

        public static bool AmmVerificaGestioneChiavi(string idPeople)
        {
            bool esito = false;
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_VERIFICA_GESTIONE_CHIAVI");
            queryDef.setParam("idPeople", idPeople);
            string commandText = queryDef.getSQL();
            string outValue = string.Empty;
            logger.Debug(commandText);

            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteScalar(out outValue, commandText); // ExecuteReader(commandText))
                if (!string.IsNullOrEmpty(outValue) && outValue.Equals("1"))
                    esito = true;
            }

            return esito;
        }
    }
}
