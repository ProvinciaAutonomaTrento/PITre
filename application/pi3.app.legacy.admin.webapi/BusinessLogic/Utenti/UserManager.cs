// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using BusinessLogic.RubricaComune;
using DocsPaVO.utente;
using Pi3.Core.Services.File.Spreadsheet;
using Serilog;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.OleDb;
using System.Runtime.CompilerServices;
using System.Xml;

namespace BusinessLogic.Utenti
{
    public class UserManager
    {
        private static ILogger logger = Log.ForContext(typeof(UserManager));

        public static DocsPaVO.utente.Utente getUtenteById(string idPeople)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.getUtenteById(idPeople);
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

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByIdPeople(string idPeople, DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.utente.InfoUtente user)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.GetCorrispondenteByIdPeople(user.idAmministrazione, idPeople, tipoIE);
        }

        public static Utente[] getUserInRoleByIdCorrGlobali(string idCorrGloRole)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            return users.getUserInRoleByIdCorrGlobali(idCorrGloRole);
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

        public static string getSuperUserAuthenticationToken()
        {
            DocsPaDocumentale.Documentale.UserManager userManager = new DocsPaDocumentale.Documentale.UserManager();

            return userManager.GetSuperUserAuthenticationToken();
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteBySystemID(string system_id)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.GetCorrispondenteBySystemID(system_id);
        }

        public static bool isUserDisabled(string userName, string idAmm)
        {
            return isUserDisabled(userName, string.Empty, idAmm);
        }

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

        private static DocsPaVO.utente.Ruolo getRuoloData(DataSet dataSet, DataRow ruoloRow)
        {
            DocsPaDB.Query_DocsPAWS.Utenti ut = new DocsPaDB.Query_DocsPAWS.Utenti();
            DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
            ruolo = ut.GetRuoloData(dataSet, ruoloRow);
            ut.Dispose();

            return ruolo;
        }

        /// <summary>
        /// getRuolo By Id_RUOLo_in_uo=dpa_corr_globali.system_id
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Ruolo getRuolo(string idRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            return utenti.GetRuolo(idRuolo);
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

        public static DocsPaVO.utente.Corrispondente getCorrispondente(string idCorrispondente, bool isEnable)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            return utenti.GetCorrispondente(idCorrispondente, isEnable);
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteBySystemIDDisabled(string system_id)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.GetCorrispondenteBySystemIDDisabled(system_id);
        }

        public static DocsPaVO.utente.Utente getUtente(string idPeople)
        {
            /*string queryString = "SELECT SYSTEM_ID, USER_ID, ID_AMM, VAR_COGNOME, VAR_NOME, VAR_TELEFONO, EMAIL_ADDRESS, CHA_NOTIFICA, CHA_AMMINISTRATORE,CHA_NOTIFICA_CON_ALLEGATO  FROM PEOPLE WHERE SYSTEM_ID="+idPeople;*/
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            return utenti.GetUtente(idPeople);
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

        public static ArrayList getListaIdAmmUtente(DocsPaVO.utente.UserLogin objLogin)
        {
            ArrayList List_idAmm = null;
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            utenti.GetIdAmmUtente(out List_idAmm, objLogin.UserName, objLogin.Modulo);
            return List_idAmm;
        }

        public static DocsPaVO.utente.Ruolo getRuoloById(string idCorrGlobali)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.getRuoloById(idCorrGlobali);
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByCodRubrica(string codice, DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.utente.InfoUtente user, IBaseRubricaComuneService rubricaComuneService)
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
                        corr = RubricaComune.RubricaServices.UpdateCorrispondente(user, codice, rubricaComuneService);
                }
                else if (string.IsNullOrEmpty(corr.systemId) || corr.inRubricaComune)
                {
                    logger.Debug("cerco Rubrica Comune");
                    corr = RubricaComune.RubricaServices.UpdateCorrispondente(user, corr, rubricaComuneService);
                }
            }

            return corr;
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteCompletoBySystemId(string systemId, DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.utente.InfoUtente user)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.GetCorrispondenteCompletoBySystemId(user.idAmministrazione, systemId, tipoIE);
        }

        public static string getRuoloRespUofromUo(string id_Uo, string tipoRuolo, string idCorr)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.getRuoloRespUoFromUo(id_Uo, tipoRuolo, idCorr);
        }

        public static DocsPaVO.utente.Corrispondente getCorrispondenteByCodRubrica(string codice, DocsPaVO.utente.InfoUtente user, IBaseRubricaComuneService rubricaComuneService)
        {
            return getCorrispondenteByCodRubrica(codice, DocsPaVO.addressbook.TipoUtente.GLOBALE, user, rubricaComuneService);
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

        public static List<UserMinimalInfo> GetUsersInRoleMinimalInfo(string roleId)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            return users.GetUsersInRoleMinimalInfo(roleId);
        }

        public static DocsPaVO.utente.Utente getUtente(string userName, string idAmministrazione)
        {
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            return utenti.GetUtente(userName, idAmministrazione);
        }

        public static DocsPaVO.utente.Ruolo getRuoloByCodice(string codice)
        {
            DocsPaDB.Query_DocsPAWS.Utenti u = new DocsPaDB.Query_DocsPAWS.Utenti();
            return u.getRuoloByCodice(codice);
        }

        public static bool importaUtenti(DocsPaVO.utente.InfoUtente infoUtente, byte[] dati, string nomeFile, string codiceAmm, bool update, ref int utInseriti, ref int utAggiornati,
            ISpreadsheetService spreadsheetService, string serverPath)
        {

            bool result = true;
            SpreadsheetModel model = null;
            try
            {

                if (!Directory.Exists(serverPath + "\\Modelli\\Import\\"))
                    Directory.CreateDirectory(serverPath + "\\Modelli\\Import\\");

                DocsPaDB.Utils.SimpleLog sl = new DocsPaDB.Utils.SimpleLog(serverPath + "\\Modelli\\Import\\logImportUtenti");

                try
                {

                    //Controllo se esiste la Directory "Import" nel path dove vengono salvati i modelli per la profilazione dinamica.
                    //Se esiste copio il file excel li' dentro, altrimenti la creo e ci copio il file.
                    //In ogni caso poichè il nome del file è fisso, anche se quest'ultimo esiste viene sovrascritto.
                    logger.Debug("Metodo \"importaUtenti\" classe \"UserManager\" : inizio lettura file \"importUtenti.xlsx\"");

                    try
                    {
                        var stream = new MemoryStream(dati);
                        model = spreadsheetService.Read(stream).Result;
                    }
                    catch (Exception e)
                    {
                        logger.Error("Errore durante la lettura del file. Dettaglio eccezione: " + e.Message);
                        return false;
                    }


                    if (!Directory.Exists(serverPath + "\\Modelli\\Import\\"))
                        Directory.CreateDirectory(serverPath + "\\Modelli\\Import\\");

                    string idAmministrazione = string.Empty;

                    sl.Log("Inizio importazione utenti - " + System.DateTime.Now.ToString());

                    foreach (var sheetModel in model.Sheets)
                    {
                        int row = 1;
                        var cellToCheck = new List<int>() { 0, 1, 2, 3 };

                        while (true)
                        {
                            var rowCells = sheetModel.Cells.Where(itm => itm.Row == row).ToList();

                            if (rowCells != null && rowCells.Any())
                            {
                                // check campi indispensabili
                                var toCheck = rowCells.Where(row => cellToCheck.Contains(row.Column) && string.IsNullOrWhiteSpace(row.ValueAsString)).ToList();
                                if (toCheck == null || !toCheck.Any())
                                {
                                    // riga valida
                                    var codAmmo = rowCells.Where(cell => cell.Column == 1).First().ValueAsString.ToUpper();

                                    if (codiceAmm.ToUpper() == codAmmo)
                                    {

                                        //Per quanto riguarda il controllo dell'esistenza di un utente con questi dati,
                                        //demando il tutto al metodo "AmmInsNuovoUtente", che trovando eventuali ripetizioni non procede all'inserimento
                                        DocsPaVO.amministrazione.OrgUtente utente = new DocsPaVO.amministrazione.OrgUtente();


                                        //utente.UserId = get_string(xlsReader, 0).ToUpper();
                                        //utente.CodiceRubrica = get_string(xlsReader, 0).ToUpper();
                                        utente.UserId = getStringValue(rowCells, 0);
                                        utente.CodiceRubrica = getStringValue(rowCells, 0);
                                        utente.Codice = utente.CodiceRubrica;
                                        utente.IDAmministrazione = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(getStringValue(rowCells, 1));
                                        idAmministrazione = utente.IDAmministrazione;

                                        utente.Nome = getStringValue(rowCells, 2, false);
                                        utente.Cognome = getStringValue(rowCells, 3, false);
                                        utente.Dominio = getStringValue(rowCells, 4, false);
                                        utente.Password = getStringValue(rowCells, 5, false);
                                        utente.Sede = getStringValue(rowCells, 7, false);
                                        utente.Email = getStringValue(rowCells, 8, false);
                                        utente.NotificaTrasm = getStringValue(rowCells, 9, false);
                                        utente.Abilitato = getStringValue(rowCells, 10, false);
                                        utente.Amministratore = getStringValue(rowCells, 11, false);

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
                                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWUSER", utente.IDPeople, "Creazione utente " + utente.Codice + " - " + utente.Nome + " " + utente.Cognome, DocsPaVO.Logger.CodAzione.Esito.OK, idAmministrazione);
                                            }
                                            else if (esito.Codice == -1)
                                            {
                                                sl.Log("");
                                                sl.Log("Utente non Inserito - UserId: " + utente.UserId + " - Nome: " + utente.Nome + " - Cognome: " + utente.Cognome);
                                                BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "AMMNEWUSER", utente.IDPeople, "Creazione utente " + utente.Codice + " - " + utente.Nome + " " + utente.Cognome, DocsPaVO.Logger.CodAzione.Esito.KO, idAmministrazione);
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    // riga non valida
                                    sl.Log("");
                                    sl.Log("WARNING : Campi obbligatori non inseriti nel modello");
                                }

                                row++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    sl.Log("");
                    sl.Log("Fine importazione utenti - " + System.DateTime.Now.ToString());
                    sl.Log("Utenti Inseriti : " + utInseriti + " - Utenti Aggiornati : " + utAggiornati);
                    logger.Debug("Fine importazione utenti - " + System.DateTime.Now.ToString());
                    logger.Debug("Utenti Inseriti : " + utInseriti + " - Utenti Aggiornati : " + utAggiornati);
                    logger.Debug("Metodo \"importaUtenti\" classe \"UserManager\" : fine lettura file \"importUtenti.xls\"");
                }
                catch (Exception ex)
                {
                    sl.Log("Errore durante l'importazione: " + ex.Message);
                    logger.Debug("Metodo \"importaUtenti\" classe \"UserManager\" ERRORE : " + ex.Message);
                    result = false;
                    return result;
                }

                return result;
            }
            catch (Exception ex)
            {

                logger.Debug("Metodo \"importaUtenti\" classe \"UserManager\" ERRORE : " + ex.Message);
                result = false;
                return result;
            }

            string getStringValue( List<CellModel> cellList, int column, bool toUpper = true ) {
                if (toUpper)
                    return cellList.Where(cell => cell.Column == column).FirstOrDefault()?.ValueAsString?.ToUpper();
                else
                    return cellList.Where(cell => cell.Column == column).FirstOrDefault()?.ValueAsString ?? "";
            }
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

        public static String GetRoleDescriptionByIdGroup(String idGroup)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            return users.GetRoleDescriptionByIdGroup(idGroup);

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

        public static String GetRoleDescriptionByIdCorrGlobali(String idCorrGlobali)
        {
            DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti();
            return users.GetRoleDescriptionByIdCorrGlobali(idCorrGlobali);
        }
    }
}
