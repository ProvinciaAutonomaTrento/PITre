// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaDB;
using Serilog;
using System.Collections;
using System.Data;

namespace BusinessLogic.Amministrazione
{
    public class UtenteManager
    {
        private static ILogger logger = Serilog.Log.ForContext(typeof(UtenteManager));

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
                    if (row["VISIBILITA"] != null)
                        voceMenu.Visibilita = row["VISIBILITA"].ToString();

                    retValue.Add(voceMenu);
                    voceMenu = null;
                }
            }

            return retValue;
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

        public static bool Checkconnection()
        {
            DocsPaDocumentale.Documentale.UserManager usm = new DocsPaDocumentale.Documentale.UserManager();
            return usm.Checkconnection();

        }
    }
}
