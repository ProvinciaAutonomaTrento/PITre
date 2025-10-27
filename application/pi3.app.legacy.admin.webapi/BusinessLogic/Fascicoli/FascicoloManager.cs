// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.fascicolazione;
using DocsPaVO.ricerche;
using Serilog;
using System.Collections;

namespace BusinessLogic.Fascicoli
{
    public class FascicoloManager
    {
        private static ILogger logger = Log.ForContext(typeof(FascicoloManager));

        public static System.Collections.ArrayList getFascicoliDaDocNoSecurity(DocsPaVO.utente.InfoUtente infoUtente, string idProfile)
        {
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

            listaFascicoli = fascicoli.GetFascicoliDaDocNoSecurity(infoUtente, idProfile);

            if (listaFascicoli == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli (getFascicoliDaDoc)");
                throw new Exception("F_System");
            }

            return listaFascicoli;
        }

        public static DocsPaVO.fascicolazione.Fascicolo[] GetFascicoloDaCodiceNoSecurityConservazione(string codiceFasc, string idAmm, string titolario, bool soloGenerali, bool isRicFasc, string idRegistro)
        {
            DocsPaVO.fascicolazione.Fascicolo[] retValue = null;

            using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
                retValue = fascicoli.GetFascicoloDaCodiceNoSecurityConservazione(codiceFasc, idAmm, titolario, soloGenerali, isRicFasc, idRegistro);

            return retValue;
        }

        public static System.Collections.ArrayList getFascicoliDaDoc(DocsPaVO.utente.InfoUtente infoUtente, string idProfile)
        {
            #region Codice Commentato
            /*			logger.Debug("getFascicoliDaDoc");
						System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

						DocsPa_V15_Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
						try
						{
							db.openConnection();
							string queryString =
								"SELECT DISTINCT A.SYSTEM_ID, A.DESCRIPTION, A.CHA_TIPO_PROJ, A.VAR_CODICE, A.ID_AMM, " +
								"A.NUM_LIVELLO, A.CHA_TIPO_FASCICOLO, A.ID_FASCICOLO, A.ID_PARENT, A.VAR_COD_ULTIMO, " +
								DocsPaWS.Utils.dbControl.toChar("A.DTA_APERTURA",false) +" AS DTA_APERTURA, " +
								DocsPaWS.Utils.dbControl.toChar("A.DTA_CHIUSURA",false) + " AS DTA_CHIUSURA, " +
								"A.CHA_STATO, A.ID_TIPO_PROC, A.VAR_NOTE, C.CHA_TIPO_DIRITTO " +
								"FROM PROJECT A, SECURITY C WHERE A.CHA_TIPO_PROJ = 'F' AND A.SYSTEM_ID IN " +
								"(SELECT A.ID_FASCICOLO FROM PROJECT A, PROJECT_COMPONENTS B WHERE A.SYSTEM_ID=B.PROJECT_ID AND B.LINK=" + infoDocumento.idProfile + ") " +
								" AND A.SYSTEM_ID=C.THING " +
								"AND (C.PERSONORGROUP="+sicurezza.idPeople+" OR C.PERSONORGROUP ="+sicurezza.idGruppo+") ORDER BY A.SYSTEM_ID";
							logger.Debug(queryString);

							DataSet dataSet = new DataSet();
							db.fillTable(queryString, dataSet, "PROJECT");

							//creazione della lista oggetti
							foreach(DataRow dataRow in dataSet.Tables["PROJECT"].Rows)
							{
								listaFascicoli.Add(getFascicolo(dataSet, dataRow));
							}
							dataSet.Dispose();
							db.closeConnection();
						}
						catch (Exception e)
						{
							logger.Debug (e.Message);
							db.closeConnection();
							throw new Exception("F_System");
						}		*/
            #endregion

            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

            listaFascicoli = fascicoli.GetFascicoliDaDoc(infoUtente, idProfile);

            if (listaFascicoli == null)
            {
                logger.Debug("Errore nella gestione dei fascicoli (getFascicoliDaDoc)");
                throw new Exception("F_System");
            }

            return listaFascicoli;
        }

        public static bool addDocFascicolo(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string idFascicolo, bool fascRapida, out string msg)
        {
            DocsPaVO.fascicolazione.Folder folder = FolderManager.getFolder(infoUtente.idPeople, infoUtente.idGruppo, idFascicolo);

            bool outValue = false;
            msg = string.Empty;

            if (folder != null)
            {
                outValue = FolderManager.addDocFolder(infoUtente, idProfile, folder.systemID, fascRapida, out msg);
            }
            else
            {
                logger.Debug("Errore nella gestione dei fascicoli. Root folder non trovato. (addDocFascicolo)");
                throw new Exception("Root folder non trovato");
            }

            return outValue;
        }

        /// <summary>
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fascicolo"></param>		
        public static void setFascicolo(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Interfaces.IProjectManager projectManager = new DocsPaDocumentale.Documentale.ProjectManager(infoUtente);

                //Verifica se si sta modificando il campo controllato per la tracciatura del log
                DocsPaDB.Query_DocsPAWS.Fascicoli fasc = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                string controllato = fasc.isControllatoModified(fascicolo.systemID);

                if (!projectManager.ModifyProject(fascicolo))
                {
                    string message = string.Format("Errore nell'aggiornamento del fascicolo con id {0}", fascicolo.systemID);
                    logger.Debug(message);
                    throw new Exception(message);
                }
                else
                {
                    // Controllo se il fascicolo ha una struttura template associata
                    string key_beprojectstructure = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROJECT_STRUCTURE");
                    if (!string.IsNullOrEmpty(key_beprojectstructure) && key_beprojectstructure.Equals("1"))
                    {
                        Folder[] folders = new DocsPaDB.Query_DocsPAWS.Fascicoli().GetFascicoloTemplate(fascicolo.systemID, fascicolo.idClassificazione, infoUtente.idAmministrazione);
                        fascicolo.HasStrutturaTemplate = !(folders == null);
                    }

                    //Richiamo il metodo per il calcolo della atipicità del fascicolo
                    DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                    fascicolo.InfoAtipicita = documentale.CalcolaAtipicita(infoUtente, fascicolo.systemID, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO);

                    transactionContext.Complete();
                    if (!string.IsNullOrEmpty(controllato) && (controllato != fascicolo.controllato))
                    {
                        //è stata fatta modifica sul campo controllato => tracciatura log
                        UserLog.UserLog.WriteLog(infoUtente, "FASCCONTROLLATO", fascicolo.codice, "Modificata proprietà controllato del fascicolo " + fascicolo.systemID, DocsPaVO.Logger.CodAzione.Esito.OK);
                    }
                }
            }
        }

        public static DocsPaVO.fascicolazione.Fascicolo getFascicoloByIdNoSecurity(string idFascicolo)
        {
            logger.Debug("getFascicoloById. idFascicolo: " + idFascicolo);

            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
            fascicolo = fascicoli.getFascicoloByIdNoSecurity(idFascicolo);

            if (fascicolo == null)
            {
                logger.Debug("getFascicoloById = NULL: fascicolo non trovato!!!");
            }
            fascicoli.Dispose();
            return fascicolo;
        }

        public static DocsPaVO.fascicolazione.Fascicolo getFascicoloDaCodice(DocsPaVO.utente.InfoUtente infoUtente, string codiceFascicolo, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione)
        {

            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            fascicolo = fascicoli.GetFascicoloDaCodice(infoUtente, codiceFascicolo, registro, enableUffRef, enableProfilazione);

            if (fascicolo == null)
            {
                logger.Debug("GetFascicoloDaCodice = NULL: fascicolo non trovato!!!");

            }

            fascicoli.Dispose();

            return fascicolo;
        }

        public static DocsPaVO.fascicolazione.Fascicolo getFascicoloDaCodice2(string idAmministrazione, string idGruppo, string idPeople, string codiceFascicolo, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione, string idTitolario)
        {

            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;
            DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();

            fascicolo = fascicoli.GetFascicoloDaCodice2(idAmministrazione, idGruppo, idPeople, codiceFascicolo, registro, enableUffRef, enableProfilazione, idTitolario);

            if (fascicolo == null)
            {
                logger.Debug("getFascicoloDaCodice2 = NULL: fascicolo non trovato!!!");

            }

            fascicoli.Dispose();

            return fascicolo;
        }

        public static ArrayList getListaFascicoliPaging(DocsPaVO.utente.InfoUtente infoUtente,
                DocsPaVO.fascicolazione.Classificazione objClassificazione,
                DocsPaVO.utente.Registro registro,
                DocsPaVO.filtri.FiltroRicerca[] filtriFascicoli,
                bool enableUfficioRef,
                bool enableProfilazione,
                bool childs,
                 out int numTotPage, out int nRec, int numPage, int pageSize,
                bool getSystemIdList, out System.Collections.Generic.List<SearchResultInfo> idProjectList, byte[] datiExcel, string serverPath)
        {
            return getListaFascicoliPaging(infoUtente, objClassificazione, registro, filtriFascicoli, null, enableUfficioRef, enableProfilazione, childs, out numTotPage, out nRec, numPage, pageSize, getSystemIdList, out idProjectList, datiExcel, serverPath);
        }

        public static ArrayList getListaFascicoliPaging(DocsPaVO.utente.InfoUtente infoUtente,
                DocsPaVO.fascicolazione.Classificazione objClassificazione,
                DocsPaVO.utente.Registro registro,
                DocsPaVO.filtri.FiltroRicerca[] filtriFascicoli,
                DocsPaVO.filtri.FiltroRicerca[] filtriDocumentiInFascicoli,
                bool enableUfficioRef,
                bool enableProfilazione,
                bool childs,
                out int numTotPage, out int nRec, int numPage, int pageSize,
                bool getSystemIdList, out System.Collections.Generic.List<SearchResultInfo> idProjectList, byte[] datiExcel, string serverPath)
        {
            ArrayList listaFascicoli = null;

            using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
                listaFascicoli = fascicoli.GetListaFascicoliPaging(infoUtente, objClassificazione, registro, filtriFascicoli, filtriDocumentiInFascicoli, enableUfficioRef, enableProfilazione, childs, out numTotPage, out nRec, numPage, pageSize, getSystemIdList, out idProjectList, datiExcel, serverPath);

            if (listaFascicoli == null)
                throw new ApplicationException("Errore nella ricerca dei fascicoli");
            else
                return listaFascicoli;
        }

        public static DocsPaVO.fascicolazione.Fascicolo[] GetFascicoloDaCodiceNoSecurity(string codiceFasc, string idAmm, string titolario, bool soloGenerali)
        {
            DocsPaVO.fascicolazione.Fascicolo[] retValue = null;

            using (DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli())
                retValue = fascicoli.GetFascicoloDaCodiceNoSecurity(codiceFasc, idAmm, titolario, soloGenerali);

            return retValue;
        }
    }
}
