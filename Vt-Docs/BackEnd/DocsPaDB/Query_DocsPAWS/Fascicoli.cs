using System;
using DocsPaUtils;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Configuration;
using System.IO;
using DocsPaVO.ricerche;
using DocsPaVO.filtri;
using System.Linq;
using DocsPaVO.ProfilazioneDinamica;
using log4net;
using System.Text;
using DocsPaVO.fascicolazione;
using DocsPaVO.Grid;
using DocsPaVO.Grids;
using System.Text.RegularExpressions;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Summary description for Fascicoli.
    /// </summary>
    public class Fascicoli : DBProvider
    {
        private static ILog logger = LogManager.GetLogger(typeof(Fascicoli));
        private string tipo_contatore = string.Empty;

        private string chaiTableDef = string.Empty;

        private static bool extAppControlEnabled = (DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ENABLE_GEST_EXT_APPS") == "1" ? true : false);

        #region Fascicolo Manager

        public Fascicoli()
        {
            //no methods here
        }

        /// <summary>
        /// return, valore dal web.config dei WS, se gli indici Testuali sono abilitati o no.
        /// </summary>
        public static string Cfg_USE_TEXT_INDEX
        {
            get
            {
                //OLD  string eme = System.Configuration.ConfigurationManager.AppSettings["USE_TEXT_INDEX"];
                string eme = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "USE_TEXT_INDEX");
                if (eme == null || eme == "")
                    eme = "0";

                return eme;

            }
        }

        /// <summary>
        /// return, valore dal web.config dei WS, se gli ARCHIVIO DI DEPOSITO è abilitato o no.
        /// </summary>
        public static bool Cfg_ARCHIVIO_DEPOSITO
        {
            get
            {
                //string eme = System.Configuration.ConfigurationManager.AppSettings["ARCHIVIO_DEPOSITO"];
                //return (eme != null && eme != "" && eme.ToLower() == "true") ? true : false;

                string eme = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ARCHIVIO_DEPOSITO");
                if (eme == null || !eme.Equals("1"))
                    return false;

                return true;
            }
        }
        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="DataApertura"></param>
        /// <param name="DataChiusura"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <param name="systemId"></param>
        /// <param name="codFascicolo"></param>
        public static string GetQueryFascicoli(string idAmministrazione, string idGruppo, string idPeople, DocsPaVO.utente.Registro registro, bool enableUfficioRef, bool enableProfilazione, string codFascicolo, string insRic)
        {
            DocsPaUtils.Query q;
            if (codFascicolo != null && codFascicolo != "")
            {
                logger.Debug("Lista fascicoli - S_J_PROJECT__SECURITY2_bis");
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__SECURITY2_BIS");
            }
            else
            {
                logger.Debug("Lista fascicoli - S_J_PROJECT__SECURITY2");
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__SECURITY2");
            }

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
            q.setParam("param3", idAmministrazione);
            q.setParam("idGruppo", idGruppo);
            q.setParam("idPeople", idPeople);

            string security = string.Empty;
            bool IS_ARCHIVISTA_DEPOSITO;
            DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
            IS_ARCHIVISTA_DEPOSITO = ut.isUtArchivistaDeposito(idPeople, idGruppo);
            string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
            if (string.IsNullOrEmpty(idRuoloPubblico))
                idRuoloPubblico = "0";
            if (IS_ARCHIVISTA_DEPOSITO)
            {
                //SAB 20/05/2013 corretto da sabrina xchè mancava la gestione sql server
                string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"].ToUpper();
                if (dbType.ToUpper() == "SQL")

                    security = " (@dbuser@.checkSecurity(A.SYSTEM_ID, @param5@, @param4@, @idRuoloPubblico@, 'F') > 0)";
                else
                    security = " (checkSecurity(A.SYSTEM_ID, @param5@, @param4@, @idRuoloPubblico@,'F') > 0)";
            }
            else
            {
                if (IndexSecurity())
                    security = " EXISTS (select /*+INDEX(e) */ 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @param5@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                else
                    security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @param5@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
            }

            q.setParam("security", security);
            q.setParam("param4", idGruppo);
            q.setParam("param5", idPeople);
            q.setParam("idRuoloPubblico", idRuoloPubblico);
            if (registro != null)
            {
                //q.setParam("param6","=" + registro.systemId + ")");
                q.setParam("param6", "AND (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO =" + registro.systemId + ")");
            }
            else
            {
                //q.setParam("param6"," IS NOT NULL)");
                q.setParam("param6", "");
            }
            if (enableUfficioRef)
            {
                q.setParam("param7", " , A.ID_UO_REF");
            }
            else
            {
                q.setParam("param7", "");
            }

            q.setParam("profilazione", "");

            q.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

            if (string.IsNullOrEmpty(insRic) || insRic.Equals("R"))
                q.setParam("condFasc", " AND A.CHA_TIPO_PROJ = 'F' ");
            if (insRic.Equals("I"))
                q.setParam("condFasc", " AND ( (A.CHA_TIPO_FASCICOLO = 'P' AND CHA_STATO = 'A') OR (A.CHA_TIPO_FASCICOLO = 'G' AND ID_TITOLARIO IN (SELECT SYSTEM_ID FROM PROJECT WHERE ID_TITOLARIO = 0 AND CHA_STATO = 'A') ) ) ");

            string queryString = q.getSQL();

            if (codFascicolo != null && codFascicolo != "")
            {
                queryString += " AND UPPER(A.VAR_CODICE)='" + codFascicolo.ToUpper() + "'";
            }

            logger.Debug(queryString);
            return queryString;

        }

        public static string GetQueryFascicoli(string idAmministrazione, string idGruppo, string idPeople, DocsPaVO.utente.Registro[] registro, bool enableUfficioRef, bool enableProfilazione, string codFascicolo, bool daGrigio)
        {
            DocsPaUtils.Query q;
            if (codFascicolo != null && codFascicolo != "")
            {
                logger.Debug("Lista fascicoli - S_J_PROJECT__SECURITY2_bis");
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__SECURITY2_BIS");
            }
            else
            {
                logger.Debug("Lista fascicoli - S_J_PROJECT__SECURITY2");
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__SECURITY2");
            }

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
            q.setParam("param3", idAmministrazione);
            string security = string.Empty;

            bool IS_ARCHIVISTA_DEPOSITO;
            DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
            IS_ARCHIVISTA_DEPOSITO = ut.isUtArchivistaDeposito(idPeople, idGruppo);
            string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
            if (IS_ARCHIVISTA_DEPOSITO)
            {
                string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"].ToUpper();
                if (dbType.ToUpper() == "SQL")

                    security = " (@dbuser@.checkSecurity(A.SYSTEM_ID, @param5@, @param4@, @idRuoloPubblico@, 'F') > 0)";
                else
                    security = " (checkSecurity(A.SYSTEM_ID, @param5@, @param4@, @idRuoloPubblico@, 'F') > 0)";
            }
            else
            {
                if (IndexSecurity())
                    security = " EXISTS (select /*+INDEX(e) */ 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @param5@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                else
                    security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @param5@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
            }

            q.setParam("security", security);
            q.setParam("param4", idGruppo);
            q.setParam("param5", idPeople);
            q.setParam("idRuoloPubblico", !string.IsNullOrEmpty(idRuoloPubblico) ? idRuoloPubblico : "0");

            string listaIdReg = string.Empty;
            if (registro != null && registro.Length > 0)
            {
                foreach (DocsPaVO.utente.Registro r in registro)
                {
                    listaIdReg += "," + r.systemId;
                }
                if (listaIdReg.Trim() != string.Empty)
                    listaIdReg = listaIdReg.Substring(1, listaIdReg.Length - 1);

                q.setParam("param6", "AND (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO  IN (" + listaIdReg + "))");
            }
            else
            {
                //q.setParam("param6"," IS NOT NULL)");
                q.setParam("param6", "");
            }
            if (enableUfficioRef)
            {
                q.setParam("param7", " , A.ID_UO_REF");
            }
            else
            {
                q.setParam("param7", "");
            }

            if (enableProfilazione)
            {
                ModelFasc mdFasc = new ModelFasc();
                if (mdFasc.existTemplatesFasc())
                {
                    //q.setParam("profilazione", ", DPA_ASS_TEMPLATES_FASC");
                    string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                    if (dbType.ToUpper() == "ORACLE")
                        q.setParam("profilazione", " left join DPA_ASS_TEMPLATES_FASC F on F.ID_PROJECT=TO_CHAR(A.SYSTEM_ID) ");
                    else
                        q.setParam("profilazione", " left join DPA_ASS_TEMPLATES_FASC F on F.ID_PROJECT=A.SYSTEM_ID ");
                }
                else
                {
                    q.setParam("profilazione", " ");
                }
            }
            else
            {
                q.setParam("profilazione", "");
            }

            q.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

            q.setParam("condFasc", " AND A.CHA_TIPO_PROJ = 'F' ");

            string queryString = q.getSQL();

            if (codFascicolo != null && codFascicolo != "")
            {
                queryString += " AND UPPER(A.VAR_CODICE)='" + codFascicolo.ToUpper() + "'";
            }

            logger.Debug(queryString);
            return queryString;

        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="DataApertura"></param>
        /// <param name="DataChiusura"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <param name="systemId"></param>
        /// <param name="codFascicolo"></param>
        public static string GetQueryFascicoli2(string idAmministrazione, string idGruppo, string idPeople, DocsPaVO.utente.Registro registro, bool enableUfficioRef, bool enableProfilazione, string codFascicolo, string idTitolario)
        {
            DocsPaUtils.Query q;

            logger.Debug("Lista fascicoli - S_J_PROJECT__SECURITY2");
            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__SECURITY2");

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
            q.setParam("param3", idAmministrazione);
            string security = string.Empty;
            bool IS_ARCHIVISTA_DEPOSITO;
            DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
            IS_ARCHIVISTA_DEPOSITO = ut.isUtArchivistaDeposito(idPeople, idGruppo);
            string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
            if (IS_ARCHIVISTA_DEPOSITO)
            {
                string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                if (dbType.ToUpper() == "SQL")
                    security = " (@dbuser@.checkSecurity(A.SYSTEM_ID, @param5@, @param4@, @idRuoloPubblico@,'F') > 0)";
                else
                    security = " (checkSecurity(A.SYSTEM_ID, @param5@, @param4@, @idRuoloPubblico@,'F') > 0)";
            }
            else
            {
                if (IndexSecurity())
                    security = " EXISTS (select /*+INDEX(e) */  'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @param5@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                else
                    security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @param5@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
            }

            q.setParam("security", security);
            q.setParam("param4", idGruppo);
            q.setParam("param5", idPeople);
            q.setParam("idRuoloPubblico", !string.IsNullOrEmpty(idRuoloPubblico) ? idRuoloPubblico : "0");
            if (idTitolario == null || idTitolario == "")
            {
                q.setParam("param8", "");
            }
            else
            {
                if (idTitolario.IndexOf(",") != 0)
                {
                    q.setParam("param8", " AND A.ID_TITOLARIO in(" + idTitolario + ") ");
                }
                else
                {
                    q.setParam("param8", " AND A.ID_TITOLARIO = " + idTitolario);
                }
            }

            if (registro != null)
            {
                //q.setParam("param6","=" + registro.systemId + ")");
                q.setParam("param6", "AND (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO =" + registro.systemId + ")");
            }
            else
            {
                //q.setParam("param6"," IS NOT NULL)");
                q.setParam("param6", "");
            }
            if (enableUfficioRef)
            {
                q.setParam("param7", " , A.ID_UO_REF");
            }
            else
            {
                q.setParam("param7", "");
            }

            if (enableProfilazione)
            {
                ModelFasc mdFasc = new ModelFasc();
                if (mdFasc.existTemplatesFasc())
                {
                    //q.setParam("profilazione", ", DPA_ASS_TEMPLATES_FASC");
                    string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                    if (dbType.ToUpper() == "ORACLE")
                        q.setParam("profilazione", " left join DPA_ASS_TEMPLATES_FASC F on F.ID_PROJECT=TO_CHAR(A.SYSTEM_ID) ");
                    else
                        q.setParam("profilazione", " left join DPA_ASS_TEMPLATES_FASC F on F.ID_PROJECT=A.SYSTEM_ID ");
                }
                else
                {
                    q.setParam("profilazione", " ");
                }
            }
            else
            {
                q.setParam("profilazione", "");
            }

            q.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

            q.setParam("condFasc", " AND A.CHA_TIPO_PROJ = 'F' ");

            string queryString = q.getSQL();

            if (codFascicolo != null && codFascicolo != "")
            {
                queryString += " AND UPPER(A.VAR_CODICE)='" + codFascicolo.ToUpper() + "'";
            }

            logger.Debug(queryString);
            return queryString;

        }

        public void UpdateFolderLevels(string idpeople, string idgruppo, string idamm, Folder folder)
        {
            logger.Debug("UpdateFolderLevels");
            string livellopadre = string.Empty;
            string foldercodlivello = string.Empty;
            string folderlevel = string.Empty;

            try
            {
                if (folder.idParent == folder.idFascicolo)
                {
                    foldercodlivello = folder.codicelivello;
                    folderlevel = "0";
                }
                else
                {
                    // Recupero i dati del padre
                    Folder parent = GetFolderById(idpeople, idgruppo, folder.idParent);
                    int ifolderlevel = Convert.ToInt32(string.IsNullOrEmpty(parent.livello) ? "0" : parent.livello) + 1;
                    int ifoldercodlivello = GetCodiceLivello(parent.codicelivello, ifolderlevel.ToString(), folder.idFascicolo, idamm);
                    foldercodlivello = string.Format("{0:0000}", ifoldercodlivello);
                    folderlevel = ifolderlevel.ToString();

                    livellopadre = string.IsNullOrEmpty(parent.livello) ? string.Empty : parent.codicelivello;
                }

                // Aggiorno il folder
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_CODLIV3");

                q.setParam("param1", string.Format("var_cod_liv1 = '{0}{1}', num_livello = {2}",
                    livellopadre, foldercodlivello, folderlevel));
                q.setParam("param2", string.Format("system_id = {0}", folder.systemID));

                logger.Debug(q.getSQL());
                ExecuteNonQuery(q.getSQL());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private int GetCodiceLivello(string codicelivello, string livello, string idfasc, string idamm)
        {
            logger.Debug("GetCodiceLivello");
            DataSet dsResult = null;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COD_LIV_C");

            q.setParam("param1", codicelivello);
            q.setParam("param2", idamm);
            q.setParam("param3", livello);
            q.setParam("param4", string.Format("AND ID_FASCICOLO={0}", idfasc));

            logger.Debug(q.getSQL());
            ExecuteQuery(out dsResult, q.getSQL());

            if (dsResult == null)
                throw new Exception("Errore nel recupero del codice livello");

            return Convert.ToInt32(dsResult.Tables[0].Rows[0][0]);
        }

        public ArrayList GetFolderByCodFasc(DocsPaVO.utente.InfoUtente infoUtente, string codiceFascicolo, string descrizioneFolder, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione)
        {
            DocsPaVO.fascicolazione.Folder folderObject = null;
            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;
            ArrayList lstFolder = new ArrayList();
            string[] separatore = {"//"};
            int inizio = 0;
            try
            {
                string queryString = GetQueryFascicoli(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, registro, enableUffRef, enableProfilazione, codiceFascicolo, "R");


                System.Data.DataSet dataSet;

                this.ExecuteQuery(out dataSet, "PROJECT", queryString);
                foreach (System.Data.DataRow dataRow in dataSet.Tables["PROJECT"].Rows)
                {
                    fascicolo = GetFascicolo(infoUtente, dataSet, dataRow, enableProfilazione);

                }


                string[] appo = descrizioneFolder.Split(separatore, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < appo.Length; i++)
                {
                    string updateString = "";
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FOLDER_BY_DESCR");
                    q.setParam("param1", fascicolo.systemID);
                    q.setParam("param2", infoUtente.idPeople);
                    q.setParam("param3", infoUtente.idGruppo);
                    q.setParam("param4", " AND UPPER(P.DESCRIPTION) = '" + appo[i].ToUpper().Replace("'", "''") + "'");
                    if (inizio == 0)
                        q.setParam("param5", " AND P.ID_PARENT in (select system_id from project where id_parent=" + fascicolo.systemID + ")");
                    else
                        q.setParam("param5", " AND P.ID_PARENT = " + ((DocsPaVO.fascicolazione.Folder)lstFolder[i - 1]).systemID);

                    string security = string.Empty;
                    string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                    if (string.IsNullOrEmpty(idRuoloPubblico))
                        idRuoloPubblico = "0";

                    q.setParam("idRuoloPubblico", idRuoloPubblico);

                    updateString = q.getSQL();
                    logger.Debug(updateString);
                    inizio++;
                    //database.fillTable(updateString,dataSet,"FOLDER");
                    this.ExecuteQuery(out dataSet, "FOLDER", updateString);
                    if (dataSet.Tables["FOLDER"].Rows.Count > 0)
                    {
                        this.FillArrayListFoldersDocument(lstFolder, dataSet.Tables["FOLDER"]);
                    }
                    else
                    {
                        lstFolder = null;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                lstFolder = null;
            }

            return lstFolder;
        }

        public ArrayList GetFolderByIdFasc(DocsPaVO.utente.InfoUtente infoUtente, string idFascicolo, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione)
        {
            DocsPaVO.fascicolazione.Folder folderObject = null;
            ArrayList lstFolder = new ArrayList();
            try
            {

                string updateString = "";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FOLDER_BY_IDFASC");
                q.setParam("param1", idFascicolo);
                q.setParam("param2", infoUtente.idPeople);
                q.setParam("param3", infoUtente.idGruppo);

                updateString = q.getSQL();
                logger.Debug(updateString);

                System.Data.DataSet dataSet;

                //database.fillTable(updateString,dataSet,"FOLDER");
                this.ExecuteQuery(out dataSet, "FOLDER", updateString);
                if (dataSet.Tables["FOLDER"].Rows.Count > 0)
                {
                    this.FillArrayListFoldersDocument(lstFolder, dataSet.Tables["FOLDER"]);
                }

            }
            catch (Exception)
            {
                lstFolder = null;
            }

            return lstFolder;
        }

        public static string GetQueryFascicoloInClassifica(string idAmministrazione, string idGruppo, string idPeople, string idRegistro, bool enableUfficioRef, string codFascicolo, string idTitolario, string systemId)
        {

            DocsPaUtils.Query q;

            logger.Debug("INIZIO GetQueryFascicoliInClassifica - S_J_PROJECT__SECURITY_CLASSIFICHE_DOCUMENTO");

            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__SECURITY_CLASSIFICHE_DOCUMENTO");

            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
            q.setParam("param3", idAmministrazione);
            string security = string.Empty;
            string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
            if (string.IsNullOrEmpty(idRuoloPubblico))
                idRuoloPubblico = "0";
            if (Cfg_ARCHIVIO_DEPOSITO)
                security = " (checkSecurity(A.SYSTEM_ID, @param5@, @param4@, @idRuoloPubblico@,'F') > 0)";
            else
            {
                if (IndexSecurity())
                    security = " EXISTS (select /*+INDEX(e) */  'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @param5@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                else
                    security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @param5@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
            }

            q.setParam("security", security);
            q.setParam("param4", idGruppo);
            q.setParam("param5", idPeople);
            q.setParam("idRuoloPubblico", idRuoloPubblico);
            q.setParam("param9", idTitolario);
            q.setParam("param10", systemId);
            if (idRegistro != null)
            {
                if (idRegistro != String.Empty)
                {
                    q.setParam("param6", "=" + idRegistro);
                }
                else
                {
                    q.setParam("param6", " IS NULL");
                }
            }
            if (enableUfficioRef)
            {
                q.setParam("param7", " , A.ID_UO_REF");
            }
            else
            {
                q.setParam("param7", "");
            }

            q.setParam("param8", codFascicolo.ToUpper());

            string queryString = q.getSQL();

            logger.Debug(queryString);

            logger.Debug("FINE GetQueryFascicoliInClassifica - S_J_PROJECT__SECURITY_CLASSIFICHE_DOCUMENTO");

            return queryString;
        }



        public static string GetQueryListaFascicoli(string idAmministrazione, string idGruppo, string idPeople, DocsPaVO.utente.Registro registro, bool enableUfficioRef, bool enableProfilazione, string varcodliv1, bool child)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__SECURITY6");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
            q.setParam("param9", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_UO_LF", false));
            q.setParam("param3", idAmministrazione);
            q.setParam("param4", idGruppo);
            q.setParam("param5", idPeople);

            string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
            if (string.IsNullOrEmpty(idRuoloPubblico))
                idRuoloPubblico = "0";
            q.setParam("idRuoloPubblico", idRuoloPubblico);

            if (registro != null)
            {
                q.setParam("param6", "=" + registro.systemId + ")");
            }
            else
            {
                q.setParam("param6", " IS NOT NULL)");
            }
            if (child)
            {
                q.setParam("param7", "AND A.VAR_COD_LIV1 LIKE '" + varcodliv1 + "%'");
            }
            else
            {
                q.setParam("param7", "AND  A.VAR_COD_LIV1 = '" + varcodliv1 + "'");
            }

            if (enableUfficioRef)//se è abilitato l'ufficio referente
            {
                q.setParam("param8", ", ID_UO_REF");
            }
            else
            {
                q.setParam("param8", "");
            }

            string queryString = q.getSQL();

            return queryString;
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="objInfoFascicolo"></param>
        /// <param name="objSicurezza"></param>
        /// <param name="debug"></param>
        /// <returns>Fascicolo o 'null' se si sono verificati errori</returns>
        public DocsPaVO.fascicolazione.Fascicolo GetDettaglio(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.InfoFascicolo objInfoFascicolo, bool enableUffRef)
        {

            DocsPaVO.fascicolazione.Fascicolo objFascicolo = null;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__SECURITY3");

                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                //DocsPaWS.Utils.dbControl.toChar("A.DTA_APERTURA",false));
                q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
                //DocsPaWS.Utils.dbControl.toChar("A.DTA_CHIUSURA",false));
                q.setParam("param7", DocsPaDbManagement.Functions.Functions.ToChar(("A.DTA_UO_LF"), false));
                if (objInfoFascicolo.idFascicolo != null)
                {
                    q.setParam("param3", objInfoFascicolo.idFascicolo);
                }
                else
                {
                    q.setParam("param3", "null");
                }
                q.setParam("param4", infoUtente.idGruppo);
                q.setParam("param5", infoUtente.idPeople);
                if (enableUffRef)
                {
                    q.setParam("param6", " , A.ID_UO_REF");
                }
                else
                {
                    q.setParam("param6", "");
                }


                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                if (string.IsNullOrEmpty(idRuoloPubblico))
                    idRuoloPubblico = "0";
                q.setParam("idRuoloPubblico", idRuoloPubblico);
                string queryString = q.getSQL();

                logger.Debug(queryString);

                System.Data.DataSet dataSet; //= new System.Data.DataSet();
                //db.fillTable(queryString, dataSet, "PROJECT");	
                this.ExecuteQuery(out dataSet, "PROJECT", queryString);

                // TODO: calcolare l'intero oggetto classificazione
                DocsPaVO.fascicolazione.Classificazione objClassificazione = new DocsPaVO.fascicolazione.Classificazione();
                objClassificazione.systemID = objInfoFascicolo.idClassificazione;

                System.Data.DataRow dataRow = dataSet.Tables["PROJECT"].Rows[0];
                objFascicolo = GetFascicolo(infoUtente, dataSet, dataRow);
                dataSet.Dispose();

                SetDataVistaSP(infoUtente, objFascicolo.systemID, "F");
                // old 12/11/2007 - SetDataVista(idPeople,objFascicolo.systemID);

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                //throw new Exception("F_System");				

                logger.Debug("F_System");
                objFascicolo = null;

            }

            return objFascicolo;
        }

        /// <summary>
        /// Reperimento numero classificazioni di un documento
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <returns></returns>
        public int GetCountClassificazioniDocumento(string idProfile, string idGruppo, string idPeople)
        {
            int retValue = 0;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_CLASSIFICAZIONI_DOCUMENTO");
                queryDef.setParam("idProfile", idProfile);
                queryDef.setParam("idPeople", idPeople);
                queryDef.setParam("idGroup", idGruppo);

                string idAmm = "0";
                if (!string.IsNullOrEmpty(idPeople))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                    idAmm = u.GetIdAmm(idPeople);
                }
                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
                queryDef.setParam("idRuoloPubblico", !string.IsNullOrEmpty(idRuoloPubblico) ? idRuoloPubblico : "0");

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DBProvider dbProvider = new DBProvider())
                {
                    string outParam;
                    if (dbProvider.ExecuteScalar(out outParam, commandText))
                        retValue = Convert.ToInt32(outParam);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return retValue;
        }

        /// <summary>
        /// Sostituisce l'omonimo metodo in DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <param name="sicurezza"></param>
        /// <returns>ArrayList o 'null' se si è verificato un errore</returns>
        public System.Collections.ArrayList GetFascicoliDaDoc(DocsPaVO.utente.InfoUtente infoUtente, string idProfile)
        {
            logger.Debug("getFascicoliDaDoc");
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__SECURITY4");

                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                //DocsPaWS.Utils.dbControl.toChar("A.DTA_APERTURA",false));
                q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
                //DocsPaWS.Utils.dbControl.toChar("A.DTA_CHIUSURA",false));
                q.setParam("param3", idProfile);
                q.setParam("param4", infoUtente.idPeople);
                q.setParam("param5", infoUtente.idGruppo);

                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                if (string.IsNullOrEmpty(idRuoloPubblico))
                    idRuoloPubblico = "0";
                q.setParam("idRuoloPubblico", idRuoloPubblico);

                string queryString = q.getSQL();

                logger.Debug(queryString);

                System.Data.DataSet dataSet;//= new System.Data.DataSet();
                //db.fillTable(queryString, dataSet, "PROJECT");	
                this.ExecuteQuery(out dataSet, "PROJECT", queryString);


                //creazione della lista oggetti
                //listaFascicoli.AddRange(dataSet.Tables["PROJECT"].DefaultView);


                #region modifica 12 dic 2006 per non far vedere classifiche di documenti
                //su nodi non visibili al ruolo corrente
                //				DocsPaVO.fascicolazione.Fascicolo fasc;
                //				foreach(System.Data.DataRow dataRow in dataSet.Tables["PROJECT"].Rows) 
                //				{
                //					fasc = GetFascicolo(dataSet, dataRow);
                //					if(VisRuoloNodoTitolario(fasc.idClassificazione,idGruppo,idPeople))
                //						listaFascicoli.Add(fasc);
                //				}  
                //fine modifica
                #endregion

                foreach (System.Data.DataRow dataRow in dataSet.Tables["PROJECT"].Rows)
                {
                    listaFascicoli.Add(GetFascicolo(infoUtente, dataSet, dataRow));
                }

                dataSet.Dispose();
                //db.closeConnection();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();

                //throw new Exception("F_System");

                logger.Debug("F_System");

                listaFascicoli = null;
            }

            return listaFascicoli;
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="fascicolo"></param>
        /// <param name="stato"></param>
        /// <param name="DataApertura"></param>
        /// <param name="DataChiusura"></param>
        /// <param name="Description"></param>
        /// <param name="systemID"></param>
        public bool SetFascicolo(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            bool result = true; // Presume successo
            //
            // Mev Ospedale Maggiore Policlinico
            bool riclassificazione = false;
            string newCodeFasc = string.Empty;
            // End Mev Ospedale Maggiore Policlinico
            // 


            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            try
            {

                //db.openConnection();

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_6_PARAMS");

                q.setParam("param1", "'" + fascicolo.stato + "'");
                q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToDate(fascicolo.apertura));

                //Chiusura/Apertura
                if (fascicolo.chiusura != null && !fascicolo.chiusura.Equals(""))
                {
                    //CHIUDE
                    q.setParam("param3", ", DTA_CHIUSURA =" + DocsPaDbManagement.Functions.Functions.ToDate(fascicolo.chiusura));
                    //Colui che ha chiuso il fascicolo
                    if (!string.IsNullOrEmpty(fascicolo.chiudeFascicolo.idPeople) && !string.IsNullOrEmpty(fascicolo.chiudeFascicolo.idCorrGlob_UO) && !string.IsNullOrEmpty(fascicolo.chiudeFascicolo.idCorrGlob_Ruolo))
                    {
                        q.setParam("param11", " , ID_RUOLO_CHIUSURA = " + fascicolo.chiudeFascicolo.idCorrGlob_Ruolo + " , ID_UO_CHIUSURA= " + fascicolo.chiudeFascicolo.idCorrGlob_UO + " , ID_AUTHOR_CHIUSURA= " + fascicolo.chiudeFascicolo.idPeople);
                    }
                }
                else
                {
                    //APRE
                    q.setParam("param3", ",DTA_CHIUSURA = " + DocsPaDbManagement.Functions.Functions.ToDate(fascicolo.chiusura));
                    q.setParam("param11", " , ID_RUOLO_CHIUSURA = '' , ID_UO_CHIUSURA= '' , ID_AUTHOR_CHIUSURA= ''");
                }

                //Descrizione	
                if (fascicolo.descrizione != null)
                {
                    q.setParam("param5", ", DESCRIPTION = '" + fascicolo.descrizione.Replace("'", "''") + "'");
                }
                else
                {
                    q.setParam("param5", "");

                }

                //Locazione fisica
                if (fascicolo.idUoLF != null && fascicolo.idUoLF != "")
                {
                    q.setParam("param7", ", ID_UO_LF = " + Convert.ToInt32(fascicolo.idUoLF));
                }
                else
                {
                    q.setParam("param7", "");
                }

                //Data locazione fisica
                if (fascicolo.dtaLF != null && fascicolo.dtaLF != "")
                {
                    q.setParam("param8", ", DTA_UO_LF = " + DocsPaDbManagement.Functions.Functions.ToDate(fascicolo.dtaLF));
                }
                else
                {
                    q.setParam("param8", "");
                }

                //Ufficio referente
                if (fascicolo.ufficioReferente != null && fascicolo.ufficioReferente.systemId != null && fascicolo.ufficioReferente.systemId != "")
                {
                    q.setParam("param9", " , ID_UO_REF = " + fascicolo.ufficioReferente.systemId);
                }
                else
                {
                    q.setParam("param9", "");
                }



                //Cartaceo
                string cartaceo = string.Empty;
                if (fascicolo.cartaceo)
                    cartaceo = "1";
                else
                    cartaceo = "0";

                q.setParam("param6", ", CARTACEO = " + cartaceo);

                //q.setParam("pubblico", fascicolo.pubblico ? ", CHA_PUBBLICO = '1'" : ", CHA_PUBBLICO = '0'");

                string controllato = string.Empty;
                if (!string.IsNullOrEmpty(fascicolo.controllato))
                    controllato = fascicolo.controllato;
                else
                    controllato = "0";

                q.setParam("controllato", ", cha_controllato = " + controllato);

                //Tipo fascicolo
                if (fascicolo.template != null)
                {
                    q.setParam("tipoFascicolo", ", ID_TIPO_FASC =" + fascicolo.template.SYSTEM_ID.ToString());
                }
                else
                {
                    q.setParam("tipoFascicolo", "");
                }

                //Data Scadenza
                if (fascicolo.dtaScadenza != null && fascicolo.dtaScadenza != "")
                {
                    q.setParam("dataScadenza", ", DTA_SCADENZA = " + DocsPaDbManagement.Functions.Functions.ToDate(fascicolo.dtaScadenza));
                }
                else
                {
                    q.setParam("dataScadenza", "");
                }

                //
                // Mev Ospedale Maggiore Policlinico
                // Se la funzionalità Riclassificazione non è attiva, i valori NodoRiclassificazione_Codice e NodoRiclassificazione_SystemID saranno string.Empty
                // altrimenti andranno ad aggiornare nella tabella PROJECT i campi ID_PARENT, VAR_CODICE, VAR_CHIAVE_FASC
                if (!string.IsNullOrEmpty(fascicolo.NodoRiclassificazione_Codice) && !string.IsNullOrEmpty(fascicolo.NodoRiclassificazione_SystemID))
                {
                    logger.Debug("MEV OSPEDALE MAGGIORE POLICLINICO: Start Riclassificazione Fascicolo");
                    // Il campo "var_codice" è nel formato identificato dal campo VAR_FORMATO_FASCICOLATURA della tabella DPA_AMMINISTRA
                    if (infoUtente != null && !string.IsNullOrEmpty(infoUtente.idAmministrazione))
                    {
                        // Devo calcolare il nuovo codice Fascicolo

                        // Parametri: 
                        // 1. IDAmministrazione
                        // 2. Codice del Nodo di titolario
                        // 3. Data di Creazione del fascicolo
                        // 4. SystemID del Nodo di titolario
                        // 5. Numero progressivo del fascicolo
                        // 6. OnlyFormatCode: true --> non ricalcola il prograssivo del fascicolo
                        newCodeFasc = this.calcolaCodiceFascicolo(infoUtente.idAmministrazione, fascicolo.NodoRiclassificazione_Codice, fascicolo.dataCreazione, fascicolo.NodoRiclassificazione_SystemID, ref fascicolo.numFascicolo, true);
                        if (!string.IsNullOrEmpty(newCodeFasc))
                        {
                            logger.Debug("Nuovo Codice del Fascicolo: " + newCodeFasc);
                            //update project set 
                            //id_parent=<id_nodo_clasisifica_selezionato>,
                            //var_codice=<nuovo codice>,
                            //var_chiave_fasc=<IDPARENT_anno_numeroFASC_IDREGISTRO> -- ValoreUnivoco
                            //where system_id=<id_fascicolo>
                            string var_chiave_fasc = string.Empty;

                            switch (dbType)
                            {
                                case "SQL":
                                    var_chiave_fasc = "(CONVERT([varchar],getdate(),(109)))";
                                    break;
                                case "ORACLE":
                                    var_chiave_fasc = "TO_CHAR(SYSTIMESTAMP, 'YYYYMMDDHH24MISSFF8')";
                                    break;
                            }

                            q.setParam("riclassificazione", ", ID_PARENT =" + fascicolo.NodoRiclassificazione_SystemID +
                                                            ", VAR_CODICE =" + "'" + newCodeFasc + "'" +
                                                            ", VAR_CHIAVE_FASC =" + var_chiave_fasc);
                        }

                        riclassificazione = true;

                    }
                    logger.Debug("MEV OSPEDALE MAGGIORE POLICLINICO: End Riclassificazione Fascicolo");
                }
                else
                {
                    q.setParam("riclassificazione", "");
                }
                // End Mev OspedaleMaggiore Policlinico
                //

                q.setParam("param10", " WHERE SYSTEM_ID=" + fascicolo.systemID);

                string updateString = q.getSQL();

                logger.Debug(updateString);
                //
                // Mev Ospedale Maggiore Policlinico
                // Se l'update va a buon fine e siamo in un caso di riclassificazione va cambiato il codice del fascicolo

                //this.ExecuteNonQuery(updateString);
                if (this.ExecuteNonQuery(updateString) && riclassificazione)
                {
                    fascicolo.codice = newCodeFasc;
                }

                // End MEV Ospedale Maggiore Policlinico
                //

                // Note fascicolo
                this.UpdateNoteFascicolo(infoUtente, fascicolo);

                //Proiflazione dinamica
                if (fascicolo.template != null)
                {
                    ModelFasc modelFasc = new ModelFasc();
                    modelFasc.salvaInserimentoUtenteProfDimFasc(fascicolo.template, fascicolo.systemID);
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();
                //throw new Exception("F_System");
                logger.Debug("F_System");

                result = false;
            }

            return result;
        }


        public string isControllatoModified(string system_id)
        {
            string queryControllato = string.Empty;
            DocsPaUtils.Query queryC = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_CONTROLLATO_FASC");
            queryC.setParam("system_id", system_id);
            queryControllato = queryC.getSQL();
            logger.Debug(queryControllato);
            string isControllato;
            this.ExecuteScalar(out isControllato, queryControllato);
            return isControllato;
        }


        /// <summary>
        /// Verifica se la chiave di configurazione da amministrazione è attiva o meno
        /// </summary>
        /// <returns></returns>
        public static bool IndexSecurity()
        {
            bool ret = false;
            try
            {
                string value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_INDEX_SECURITY");

                if (!string.IsNullOrEmpty(value) && value.Equals("1"))
                    ret = true;
                else
                    ret = false;

                return ret;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool FolderHasDocument(Folder item)
        {
            bool result = false;

            try
            {
                string num_documents = string.Empty;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FOLDER_HAS_DOCUMENTS");
                q.setParam("param1", item.systemID);
                string query = q.getSQL();

                logger.Debug(query);
                ExecuteScalar(out num_documents, query);
                result = Convert.ToInt32(num_documents) > 0;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="oldDiritto"></param>
        /// <param name="debug"></param>
        /// <returns>ArrayList o 'null' se si è verificato un errore.</returns>
        public System.Collections.ArrayList GetDocFolderFasc(string idFascicolo, string oldDiritto)
        {
            logger.Debug("getDocFolderFasc");
            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            //bool dbOpen = false;
            System.Data.DataSet ds; //= new System.Data.DataSet();

            System.Collections.ArrayList result = new System.Collections.ArrayList();

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project10");
                q.setParam("param1", idFascicolo);
                string queryFolderString = q.getSQL();


                //db.fillTable(queryFolderString,ds,"FOLDER");
                this.ExecuteQuery(out ds, "FOLDER", queryFolderString);
                System.Collections.ArrayList folderFascList = new System.Collections.ArrayList();

                //riempie l'array list delle folder del fascicolo
                /*for(int j=0;j<ds.Tables["FOLDER"].Rows.Count;j++)
                {
                    folderFascList.Add(ds.Tables["FOLDER"].Rows[j]["SYSTEM_ID"].ToString());
                }*/
                foreach (DataRow dr in ds.Tables["FOLDER"].Rows)
                {
                    folderFascList.Add(dr["SYSTEM_ID"].ToString());
                }

                //trova tutti i documenti appartenenti alle folders
                DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__COMPONENTS");

                string queryDocString = System.String.Empty;
                for (int j = 0; j < folderFascList.Count; j++)
                {
                    queryDocString = queryDocString + folderFascList[j].ToString();
                    if (j < folderFascList.Count - 1)
                    {
                        queryDocString = queryDocString + ",";
                    }
                }
                q1.setParam("param1", queryDocString);
                q1.setParam("param2", oldDiritto);

                queryDocString = q1.getSQL();
                logger.Debug(queryDocString);
                DataSet dsDocs = new DataSet(); ;
                //db.fillTable(queryDocString,ds,"DOCS");
                this.ExecuteQuery(dsDocs, "DOCS", queryDocString);

                //si verifica se il documento appartiene ad altre folder diverse
                DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponents2");
                string queryFolderDocString = System.String.Empty;
                for (int k = 0; k < dsDocs.Tables["DOCS"].Rows.Count; k++)
                {
                    System.Collections.ArrayList folderDocList = new System.Collections.ArrayList();

                    //string queryFolderDocString="SELECT PROJECT_ID FROM PROJECT_COMPONENTS WHERE LINK="+ds.Tables["DOCS"].Rows[k]["LINK"].ToString();
                    q2.setParam("param1", dsDocs.Tables["DOCS"].Rows[k]["LINK"].ToString());
                    queryFolderDocString = q2.getSQL();

                    logger.Debug(queryFolderDocString);
                    //db.fillTable(queryFolderDocString,ds,"FOLDER_DOC");
                    this.ExecuteQuery(dsDocs, "FOLDER_DOC", queryFolderDocString);
                    //riempie l'array list delle folder
                    for (int m = 0; m < dsDocs.Tables["FOLDER_DOC"].Rows.Count; m++)
                    {
                        folderDocList.Add(dsDocs.Tables["FOLDER_DOC"].Rows[m]["PROJECT_ID"].ToString());
                    }
                    //si verifica se le folder del documento sono contenute nell'insieme delle folder del fascicolo
                    if (isContained(folderDocList, folderFascList))
                    {
                        logger.Debug("Aggiunto doc " + dsDocs.Tables["DOCS"].Rows[k]["LINK"].ToString());
                        result.Add(dsDocs.Tables["DOCS"].Rows[k]["LINK"].ToString());
                    }
                    dsDocs.Tables["FOLDER_DOC"].Reset();
                }
            }
            catch (Exception e)
            {
                /*
                if(dbOpen)
                {
                    //db.closeConnection();
                }
                */

                logger.Debug(e.Message);
                //throw new Exception("F_System");
                logger.Debug("F_System");

                result = null;
            }

            return result;
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="list"></param>
        /// <param name="container"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private static bool isContained(System.Collections.ArrayList list, System.Collections.ArrayList container)
        {
            logger.Debug("isContained");
            for (int i = 0; i < list.Count; i++)
            {
                bool contained = false;
                for (int j = 0; j < container.Count; j++)
                {
                    if (list[i].ToString().Equals(container[j].ToString()))
                    {
                        contained = true;
                    }
                }
                if (contained == false) return false;
            }
            return true;
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="corr"></param>
        /// <param name="fasc"></param>
        /// <param name="debug"></param>
        public bool SospendiRiattivaUtente(string idPeople, DocsPaVO.utente.Corrispondente corr, DocsPaVO.fascicolazione.Fascicolo fasc)
        {
            bool result = true; // Presume successo

            logger.Debug("sospendiRiattivaUtente");
            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();
            //bool dbOpen=false;	
            bool transOpen = false;

            try
            {
                //db.openConnection();				
                //dbOpen=true;

                //si controlla se l'utente ha diritti di proprietario sul fascicolo
                //string utenteString="SELECT CHA_TIPO_DIRITTO FROM SECURITY WHERE THING="+fasc.systemID+" AND PERSONORGROUP="+infoUtente.idPeople;

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Security2");
                q.setParam("param1", fasc.systemID);
                q.setParam("param2", idPeople);
                string utenteString = q.getSQL();


                logger.Debug(utenteString);
                string dirittoUtente = null;
                string resScalar;
                this.ExecuteScalar(out resScalar, utenteString);
                if (resScalar != null)
                //if(db.executeScalar(utenteString)!=null)
                {
                    //dirittoUtente=db.executeScalar(utenteString).ToString();
                    this.ExecuteScalar(out dirittoUtente, utenteString);
                }
                if (dirittoUtente != null && dirittoUtente.Equals("P"))
                {
                    //si controlla quali sono i diritti del corrispondente
                    string idCorr = System.String.Empty;
                    if (corr.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
                    {
                        idCorr = ((DocsPaVO.utente.Utente)corr).idPeople;
                    }
                    else
                    {
                        idCorr = ((DocsPaVO.utente.Ruolo)corr).idGruppo;
                    }
                    logger.Debug("idCorr=" + idCorr);

                    //string corrString="SELECT CHA_TIPO_DIRITTO FROM SECURITY WHERE THING="+fasc.systemID+" AND PERSONORGROUP="+idCorr;
                    DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_Security2");
                    q1.setParam("param1", fasc.systemID);
                    q1.setParam("param1", idPeople);
                    string corrString = q.getSQL();


                    //string dirittoCorr=db.executeScalar(corrString).ToString();
                    string dirittoCorr;
                    this.ExecuteScalar(out dirittoCorr, corrString);

                    logger.Debug("dirittoCorr=" + dirittoCorr);
                    if (dirittoCorr.Equals("S") || dirittoCorr.Equals("T"))
                    {
                        //bisogna eseguire degli update
                        System.Collections.ArrayList updateQueries = new System.Collections.ArrayList();
                        string updateString = "";
                        string tipoDiritto = "";
                        string tipoDirittoDoc = "";
                        //string oldTipoDirittoDoc = "";
                        string accessRight = "";
                        if (dirittoCorr.Equals("S"))
                        {
                            tipoDiritto = "T";
                            tipoDirittoDoc = "F";
                            //oldTipoDirittoDoc="S";
                            accessRight = "45";
                        }
                        else
                        {
                            tipoDiritto = "S";
                            tipoDirittoDoc = "S";
                            //oldTipoDirittoDoc="F";
                            accessRight = "0";
                        }
                        //updateString="UPDATE SECURITY SET CHA_TIPO_DIRITTO='"+tipoDiritto+"',ACCESSRIGHTS="+accessRight+" WHERE THING="+fasc.systemID+" AND PERSONORGROUP="+idCorr;

                        DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("U_SECURITY");
                        q2.setParam("param1", tipoDiritto);
                        q2.setParam("param2", accessRight);
                        q2.setParam("param3", fasc.systemID);
                        q2.setParam("param4", idCorr);
                        updateString = q2.getSQL();


                        logger.Debug(updateString);
                        updateQueries.Add(updateString);

                        //update dei documenti del fascicolo che verificano certe condizioni
                        System.Collections.ArrayList docsUpdateList = new System.Collections.ArrayList();
                        //(fasc.systemID,oldTipoDirittoDoc);
                        for (int i = 0; i < docsUpdateList.Count; i++)
                        {
                            //string updateDocString="UPDATE SECURITY SET CHA_TIPO_DIRITTO='"+tipoDirittoDoc+"',ACCESSRIGHTS="+accessRight+" WHERE THING="+docsUpdateList[i].ToString()+" AND PERSONORGROUP="+idCorr;
                            DocsPaUtils.Query q3 = DocsPaUtils.InitQuery.getInstance().getQuery("U_SECURITY");
                            q3.setParam("param1", tipoDirittoDoc);
                            q3.setParam("param2", accessRight);
                            q3.setParam("param3", docsUpdateList[i].ToString());
                            q3.setParam("param4", idCorr);
                            string updateDocString = q2.getSQL();
                            updateQueries.Add(updateDocString);
                        }

                        //si eseguono gli update
                        //db.beginTransaction();
                        this.BeginTransaction();
                        transOpen = true;
                        for (int m = 0; m < updateQueries.Count; m++)
                        {
                            //db.executeNonQuery(updateQueries[m].ToString());
                            this.ExecuteNonQuery(updateQueries[m].ToString());

                            logger.Debug("Eseguita: " + updateQueries[m]);
                        }


                        //db.commitTransaction();
                        this.CommitTransaction();

                        transOpen = false;

                        //db.closeConnection();						
                        //dbOpen=false;														
                    }
                }
                else
                {

                    //db.closeConnection();

                    //l'utente non è proprietario del fascicolo: eccezione
                    //throw new Exception("F_System");
                    logger.Debug("F_System");

                    result = false;
                }
            }
            catch (Exception e)
            {
                if (transOpen)
                {
                    //db.rollbackTransaction();
                    this.RollbackTransaction();
                }

                /*
                if(dbOpen)
                {
                    //db.closeConnection();					
                }
                */

                logger.Debug(e.Message);
                //throw new Exception("F_System");
                logger.Debug("F_System");

                result = false;
            }

            return result;
        }

        public CreatoreFascicolo GetCreatoreFascicolo(string systemID)
        {
            CreatoreFascicolo creatore = null;
            string queryString = string.Empty;

            Query q = InitQuery.getInstance().getQuery("S_CREATORE_FASCICOLO");
            q.setParam("param1", systemID);
            queryString = q.getSQL();
            logger.Debug(queryString);

            using (DBProvider dbProvider = new DBProvider())
            {
                using (DataSet ds = new DataSet())
                {
                    dbProvider.ExecuteQuery(ds, queryString);
                    creatore = GetCreatoreFascicolo(ds.Tables[0].Rows[0]);
                }
            }

            return creatore;
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dataRow"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private static DocsPaVO.fascicolazione.Fascicolo GetFascicolo2(System.Data.DataSet dataSet, System.Data.DataRow dataRow, bool enableProfilazione)
        {
            DocsPaVO.fascicolazione.Fascicolo objFascicolo = new DocsPaVO.fascicolazione.Fascicolo();
            string codiceRegistro = "";

            objFascicolo.systemID = dataRow["SYSTEM_ID"].ToString();
            objFascicolo.apertura = dataRow["DTA_APERTURA"].ToString().Trim();
            objFascicolo.chiusura = dataRow["DTA_CHIUSURA"].ToString().Trim();
            objFascicolo.codice = dataRow["VAR_CODICE"].ToString();
            objFascicolo.descrizione = dataRow["DESCRIPTION"].ToString();
            objFascicolo.stato = dataRow["CHA_STATO"].ToString();
            objFascicolo.tipo = dataRow["CHA_TIPO_FASCICOLO"].ToString();
            objFascicolo.idClassificazione = dataRow["ID_PARENT"].ToString();
            objFascicolo.codUltimo = dataRow["VAR_COD_ULTIMO"].ToString();
            objFascicolo.idRegistroNodoTit = dataRow["ID_REGISTRO"].ToString();
            objFascicolo.idTitolario = dataRow["ID_TITOLARIO"].ToString();
            if (dataRow.Table.Columns.Contains("NUM_MESI_CONSERVAZIONE"))
            {
                objFascicolo.numMesiConservazione = dataRow["NUM_MESI_CONSERVAZIONE"].ToString();
            }
            if (dataRow.Table.Columns.Contains("ACCESSRIGHTS"))
            {
                objFascicolo.accessRights = dataRow["ACCESSRIGHTS"].ToString();
            }

            //			//nuovo per popolare il campo descrizione del registro a cui il fascicolo è associato
            if (objFascicolo.idRegistroNodoTit != null && objFascicolo.idRegistroNodoTit != String.Empty)
            {
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                codiceRegistro = utenti.GetCodiceRegistroBySystemId(objFascicolo.idRegistroNodoTit);
                objFascicolo.codiceRegistroNodoTit = codiceRegistro;

            }

            //Se presenti inseriamo i valori di locazione fisica
            if (dataRow.Table.Columns.Contains("DTA_UO_LF") && dataRow.Table.Columns.Contains("ID_UO_LF"))
            {
                if (dataRow["DTA_UO_LF"] != null && dataRow["ID_UO_LF"] != null)
                {
                    objFascicolo.dtaLF = dataRow["DTA_UO_LF"].ToString();
                    objFascicolo.idUoLF = dataRow["ID_UO_LF"].ToString();
                }
            }
            if (dataRow.Table.Columns.Contains("ID_UO_REF"))
            {
                if (dataRow["ID_UO_REF"] != null && !objFascicolo.tipo.Equals("G"))
                {
                    //per ora popolo solamente la system_id dell'ufficio referente
                    objFascicolo.ufficioReferente = GetCorrispondenteUfficioReferenteFascicolo(dataRow["ID_UO_REF"].ToString());
                }
                else
                {
                    objFascicolo.ufficioReferente = null;
                }
            }
            //objFascicolo.dirittoUtente=dataRow["CHA_TIPO_DIRITTO"].ToString();

            // Gestione fascicolo cartaceo
            if (dataRow["CARTACEO"] != DBNull.Value)
            {
                int cartaceo;
                if (Int32.TryParse(dataRow["CARTACEO"].ToString(), out cartaceo))
                    objFascicolo.cartaceo = (cartaceo > 0);
            }

            // Gestione fascicolo privato
            if (dataRow.Table.Columns.Contains("CHA_PRIVATO"))
            {
                if (dataRow["CHA_PRIVATO"] != DBNull.Value)
                {
                    objFascicolo.privato = dataRow["CHA_PRIVATO"].ToString();
                }
                else
                {
                    objFascicolo.privato = null;
                }
            }

            // Gestione fascicolazione consentita
            if (dataRow.Table.Columns.Contains("CHA_CONSENTI_CLASS"))
            {
                if (dataRow["CHA_CONSENTI_CLASS"] != DBNull.Value)
                {
                    objFascicolo.isFascConsentita = dataRow["CHA_CONSENTI_CLASS"].ToString();
                }
                else
                {
                    objFascicolo.isFascConsentita = null;
                }
            }
            // Gestione Classificazione consentita
            if (dataRow.Table.Columns.Contains("CHA_CONSENTI_FASC"))
            {
                if (dataRow["CHA_CONSENTI_FASC"] != DBNull.Value)
                {
                    objFascicolo.isFascicolazioneConsentita = dataRow["CHA_CONSENTI_FASC"].ToString().Equals("1");
                }
                else
                {
                    objFascicolo.isFascicolazioneConsentita = true;
                }
            }

            //Profilazione dinamica fascicolo
            if (enableProfilazione)
            {
                ModelFasc modelFasc = new ModelFasc();
                objFascicolo.template = modelFasc.getTemplateFascDettagli(objFascicolo.systemID);
            }
            //Fine profilazione dinamica fascicolo

            //Num Fascicolo
            if (dataRow.Table.Columns.Contains("NUM_FASCICOLO"))
                objFascicolo.numFascicolo = dataRow["NUM_FASCICOLO"].ToString();

            objFascicolo.creatoreFascicolo = GetCreatoreFascicolo(dataRow);

            return objFascicolo;
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dataRow"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private static DocsPaVO.fascicolazione.Fascicolo GetFascicolo(DocsPaVO.utente.InfoUtente infoUtente, System.Data.DataSet dataSet, System.Data.DataRow dataRow, bool enableProfilazione)
        {
            DocsPaVO.fascicolazione.Fascicolo objFascicolo = new DocsPaVO.fascicolazione.Fascicolo();

            objFascicolo.systemID = dataRow["SYSTEM_ID"].ToString();
            objFascicolo.apertura = dataRow["DTA_APERTURA"].ToString().Trim();
            objFascicolo.chiusura = dataRow["DTA_CHIUSURA"].ToString().Trim();
            objFascicolo.codice = dataRow["VAR_CODICE"].ToString();
            objFascicolo.descrizione = dataRow["DESCRIPTION"].ToString();
            objFascicolo.stato = dataRow["CHA_STATO"].ToString();
            objFascicolo.tipo = dataRow["CHA_TIPO_FASCICOLO"].ToString();
            objFascicolo.idClassificazione = dataRow["ID_PARENT"].ToString();
            objFascicolo.codUltimo = dataRow["VAR_COD_ULTIMO"].ToString();
            objFascicolo.idRegistroNodoTit = dataRow["ID_REGISTRO"].ToString();
            //modifica
            if (dataRow.Table.Columns.Contains("contatore"))
                objFascicolo.contatore = dataRow["contatore"].ToString();
            //fine modifica
            if (dataRow.Table.Columns.Contains("NUM_MESI_CONSERVAZIONE"))
            {
                objFascicolo.numMesiConservazione = dataRow["NUM_MESI_CONSERVAZIONE"].ToString();
            }

            //if (dataRow.Table.Columns.Contains("mesiDaChiusura"))
            //{
            //    objFascicolo.numMesiChiusura = dataRow["mesiDaChiusura"].ToString();
            //}

            if (dataRow.Table.Columns.Contains("IN_SCARTO"))
            {
                objFascicolo.inScarto = dataRow["IN_SCARTO"].ToString();
            }

            if (dataRow.Table.Columns.Contains("IN_CONSERVAZIONE"))
            {
                if (dataRow["IN_CONSERVAZIONE"] != DBNull.Value)
                {
                    objFascicolo.inConservazione = dataRow["IN_CONSERVAZIONE"].ToString();
                }
            }

            if (dataRow.Table.Columns.Contains("CHA_IN_ARCHIVIO"))
            {
                if (dataRow["CHA_IN_ARCHIVIO"] != DBNull.Value)
                {
                    objFascicolo.inArchivio = dataRow["CHA_IN_ARCHIVIO"].ToString();
                }
            }
            if (dataRow.Table.Columns.Contains("CHA_PUBBLICO"))
            {
                if (dataRow["CHA_PUBBLICO"] != DBNull.Value)
                {
                    objFascicolo.pubblico = dataRow["CHA_PUBBLICO"].ToString().Equals("1") ? true : false;
                }
                else
                {
                    objFascicolo.pubblico = false;
                }
            }

            if (dataRow.Table.Columns.Contains("ID_TITOLARIO"))
                objFascicolo.idTitolario = dataRow["ID_TITOLARIO"].ToString();

            if (dataRow.Table.Columns.Contains("ACCESSRIGHTS"))
                objFascicolo.accessRights = dataRow["ACCESSRIGHTS"].ToString();

            //nuovo per popolare il campo descrizione del registro a cui il fascicolo è associato
            if (objFascicolo.idRegistroNodoTit != null && objFascicolo.idRegistroNodoTit != String.Empty)
            {
                //nuova gestione Paginata
                if (dataRow.Table.Columns.Contains("CODREG"))
                    objFascicolo.codiceRegistroNodoTit = dataRow["CODREG"].ToString();
            }

            //Se presenti inseriamo i valori di locazione fisica
            if (dataRow.Table.Columns.Contains("DTA_UO_LF") && dataRow.Table.Columns.Contains("ID_UO_LF"))
            {
                if (dataRow["DTA_UO_LF"] != null && dataRow["ID_UO_LF"] != null)
                {
                    objFascicolo.dtaLF = dataRow["DTA_UO_LF"].ToString();
                    objFascicolo.idUoLF = dataRow["ID_UO_LF"].ToString();
                }
            }
            if (dataRow.Table.Columns.Contains("ID_UO_REF"))
            {
                if (dataRow["ID_UO_REF"] != null && !objFascicolo.tipo.Equals("G"))
                {
                    //per ora popolo solamente la system_id dell'ufficio referente
                    objFascicolo.ufficioReferente = GetCorrispondenteUfficioReferenteFascicolo(dataRow["ID_UO_REF"].ToString());
                }
                else
                {
                    objFascicolo.ufficioReferente = null;
                }
            }
            //objFascicolo.dirittoUtente=dataRow["CHA_TIPO_DIRITTO"].ToString();

            // Gestione fascicolo cartaceo
            if (dataRow["CARTACEO"] != DBNull.Value)
            {
                int cartaceo;
                if (Int32.TryParse(dataRow["CARTACEO"].ToString(), out cartaceo))
                    objFascicolo.cartaceo = (cartaceo > 0);
            }

            // Gestione fascicolo privato
            if (dataRow.Table.Columns.Contains("CHA_PRIVATO"))
            {
                if (dataRow["CHA_PRIVATO"] != DBNull.Value)
                {
                    objFascicolo.privato = dataRow["CHA_PRIVATO"].ToString();
                }
                else
                {
                    objFascicolo.privato = null;
                }
            }

            if (dataRow.Table.Columns.Contains("CODTIT"))
            {
                if (dataRow["CODTIT"] != DBNull.Value)
                {
                    objFascicolo.codiceGerarchia = dataRow["CODTIT"].ToString();
                }
                else
                {
                    objFascicolo.codiceGerarchia = null;
                }
            }

            objFascicolo.controllato = "0";
            if (dataRow.Table.Columns.Contains("cha_controllato"))
            {
                objFascicolo.controllato = dataRow["cha_controllato"].ToString();
            }

            // Gestione fascicolazione consentita
            if (dataRow.Table.Columns.Contains("CHA_CONSENTI_CLASS"))
            {
                if (dataRow["CHA_CONSENTI_CLASS"] != DBNull.Value)
                {
                    objFascicolo.isFascConsentita = dataRow["CHA_CONSENTI_CLASS"].ToString();
                }
                else
                {
                    objFascicolo.isFascConsentita = null;
                }
            }

            // Gestione Classificazione consentita
            if (dataRow.Table.Columns.Contains("CHA_CONSENTI_FASC"))
            {
                if (dataRow["CHA_CONSENTI_FASC"] != DBNull.Value)
                {
                    objFascicolo.isFascicolazioneConsentita = dataRow["CHA_CONSENTI_FASC"].ToString().Equals("1");
                }
                else
                {
                    objFascicolo.isFascicolazioneConsentita = true;
                }
            }

            //Profilazione dinamica fascicolo
            if (enableProfilazione)
            {
                ModelFasc modelFasc = new ModelFasc();
                objFascicolo.template = modelFasc.getTemplateFascDettagli(objFascicolo.systemID);
            }
            //Fine profilazione dinamica fascicolo

            // Caricamento note del fascicolo
            FetchNoteFascicolo(infoUtente, objFascicolo);

            //Data Scadenza
            if (dataRow["DTA_SCADENZA"] != null && dataRow["DTA_SCADENZA"].ToString() != "")
                objFascicolo.dtaScadenza = dataRow["DTA_SCADENZA"].ToString();

            //Num Fascicolo
            if (dataRow.Table.Columns.Contains("NUM_FASCICOLO"))
                objFascicolo.numFascicolo = dataRow["NUM_FASCICOLO"].ToString();

            //Fascicolo controllato
            objFascicolo.controllato = "0";
            if (dataRow.Table.Columns.Contains("CHA_CONTROLLATO"))
            {
                if (dataRow["CHA_CONTROLLATO"] != DBNull.Value)
                {
                    objFascicolo.controllato = dataRow["CHA_CONTROLLATO"].ToString();
                }

            }

            // Reperimento dell'ultima nota di visibilità generale inserita nel documento
            GetUltimaNotaFascicolo(objFascicolo, infoUtente);

            objFascicolo.creatoreFascicolo = GetCreatoreFascicolo(dataRow);

            //informazioni su chi ha chiuso il fascicolo
            objFascicolo.chiudeFascicolo = GetChiudeFascicolo(dataRow);

            // Impostazione del flag per indicare se il fascicolo è in ADL
            //  objFascicolo.InAreaLavoro = GetIsProjectInADL(objFascicolo.systemID);
            if (dataRow.Table.Columns.Contains("in_adl"))
            {
                objFascicolo.InAreaLavoro = dataRow["in_adl"].ToString();
            }
            else
            {
                objFascicolo.InAreaLavoro = GetIsProjectInADLUtente(objFascicolo.systemID, infoUtente);
            }

            return objFascicolo;
        }

        /// <summary>
        /// Questa funzione si occupa di verificare se un fascicolo si trova in ADL.
        /// Restituisce 1 se il fascicolo si trova in ADL, 0 altrimenti
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns>1 se il fascicolo è in ADL, 0 altrimenti</returns>
        private static string GetIsProjectInADL(string objectId)
        {
            // Valore da restituire
            String toReturn = "0";

            using (DBProvider dbProvider = new DBProvider())
            {
                // Prelevamento della query da eseguire
                Query q = InitQuery.getInstance().getQuery("S_DPAAreaLavoro");
                q.setParam("param1", "COUNT(*)");
                q.setParam("param2", String.Format("ID_PROJECT = {0}", objectId));

                try
                {
                    // Esecuzione della query
                    dbProvider.ExecuteScalar(out toReturn, q.getSQL());
                }
                catch (Exception e) { }

            }

            // Restituzione del valore calcolato
            return toReturn;

        }

        /// <summary>
        /// Metodo per la ricrca dell'ultima nota di visibilità generale inserita
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="infoUtente"></param>
        private static void GetUltimaNotaFascicolo(DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            // Creo un nuovo oggetto Note che si occuperà di cercare le ultime note
            // inserite per un certo fascicolo
            Note note = new Note(infoUtente);

            // Creo un nuovo oggetto associazione nota
            DocsPaVO.Note.AssociazioneNota associazione = new DocsPaVO.Note.AssociazioneNota(DocsPaVO.Note.AssociazioneNota.OggettiAssociazioniNotaEnum.Fascicolo, fascicolo.systemID);

            // Ultima nota
            DocsPaVO.Note.InfoNota ultima = note.GetUltimaNota(associazione);

            if (ultima != null)
                fascicolo.ultimaNota = note.GetUltimaNotaAsString(associazione);

        }

        private static DocsPaVO.fascicolazione.Fascicolo GetFascicolo(DocsPaVO.utente.InfoUtente infoUtente, System.Data.DataSet dataSet, System.Data.DataRow dataRow)
        {
            DocsPaVO.fascicolazione.Fascicolo objFascicolo = new DocsPaVO.fascicolazione.Fascicolo();
            string codiceRegistro = "";

            objFascicolo.systemID = dataRow["SYSTEM_ID"].ToString();
            objFascicolo.apertura = dataRow["DTA_APERTURA"].ToString().Trim();
            objFascicolo.chiusura = dataRow["DTA_CHIUSURA"].ToString().Trim();
            objFascicolo.codice = dataRow["VAR_CODICE"].ToString();
            objFascicolo.descrizione = dataRow["DESCRIPTION"].ToString();
            objFascicolo.stato = dataRow["CHA_STATO"].ToString();
            objFascicolo.tipo = dataRow["CHA_TIPO_FASCICOLO"].ToString();
            objFascicolo.idClassificazione = dataRow["ID_PARENT"].ToString();
            objFascicolo.codUltimo = dataRow["VAR_COD_ULTIMO"].ToString();
            objFascicolo.idRegistroNodoTit = dataRow["ID_REGISTRO"].ToString();
            objFascicolo.idTitolario = dataRow["ID_TITOLARIO"].ToString();

            if (dataRow.Table.Columns.Contains("NUM_MESI_CONSERVAZIONE"))
            {
                objFascicolo.numMesiConservazione = dataRow["NUM_MESI_CONSERVAZIONE"].ToString();
            }

            if (dataRow.Table.Columns.Contains("IN_CONSERVAZIONE"))
            {
                if (dataRow["IN_CONSERVAZIONE"] != DBNull.Value)
                {
                    objFascicolo.inConservazione = dataRow["IN_CONSERVAZIONE"].ToString();
                }
            }
            if (dataRow.Table.Columns.Contains("ACCESSRIGHTS"))
            {
                objFascicolo.accessRights = dataRow["ACCESSRIGHTS"].ToString();
            }

            //			//nuovo per popolare il campo descrizione del registro a cui il fascicolo è associato
            if (objFascicolo.idRegistroNodoTit != null && objFascicolo.idRegistroNodoTit != String.Empty)
            {
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                codiceRegistro = utenti.GetCodiceRegistroBySystemId(objFascicolo.idRegistroNodoTit);
                objFascicolo.codiceRegistroNodoTit = codiceRegistro;

            }

            //Se presenti inseriamo i valori di locazione fisica
            if (dataRow.Table.Columns.Contains("DTA_UO_LF") && dataRow.Table.Columns.Contains("ID_UO_LF"))
            {
                if (dataRow["DTA_UO_LF"] != null && dataRow["ID_UO_LF"] != null)
                {
                    objFascicolo.dtaLF = dataRow["DTA_UO_LF"].ToString();
                    objFascicolo.idUoLF = dataRow["ID_UO_LF"].ToString();
                }
            }
            if (dataRow.Table.Columns.Contains("ID_UO_REF"))
            {
                if (dataRow["ID_UO_REF"] != null && !objFascicolo.tipo.Equals("G"))
                {
                    //per ora popolo solamente la system_id dell'ufficio referente
                    objFascicolo.ufficioReferente = GetCorrispondenteUfficioReferenteFascicolo(dataRow["ID_UO_REF"].ToString());
                }
                else
                {
                    objFascicolo.ufficioReferente = null;
                }
            }
            //objFascicolo.dirittoUtente=dataRow["CHA_TIPO_DIRITTO"].ToString();

            // Gestione fascicolo cartaceo
            if (dataRow["CARTACEO"] != DBNull.Value)
            {
                int cartaceo;
                if (Int32.TryParse(dataRow["CARTACEO"].ToString(), out cartaceo))
                    objFascicolo.cartaceo = (cartaceo > 0);
            }

            // Gestione fascicolo privato
            if (dataRow.Table.Columns.Contains("CHA_PRIVATO"))
            {
                if (dataRow["CHA_PRIVATO"] != DBNull.Value)
                {
                    objFascicolo.privato = dataRow["CHA_PRIVATO"].ToString();
                }
                else
                {
                    objFascicolo.privato = null;
                }
            }

            //Gestione fascicolo pubblico
            if (dataRow.Table.Columns.Contains("CHA_PUBBLICO"))
            {
                if (dataRow["CHA_PUBBLICO"] != DBNull.Value)
                {
                    objFascicolo.pubblico = dataRow["CHA_PUBBLICO"].ToString().Equals("1") ? true : false;
                }
                else
                {
                    objFascicolo.pubblico = false;
                }
            }

            // Gestione fascicolo controllato
            objFascicolo.controllato = "0";
            if (dataRow.Table.Columns.Contains("CHA_CONTROLLATO"))
            {
                if (dataRow["CHA_CONTROLLATO"] != DBNull.Value)
                {
                    objFascicolo.controllato = dataRow["CHA_CONTROLLATO"].ToString();
                }

            }

            // Gestione fascicolazione consentita
            if (dataRow.Table.Columns.Contains("CHA_CONSENTI_CLASS"))
            {
                if (dataRow["CHA_CONSENTI_CLASS"] != DBNull.Value)
                {
                    objFascicolo.isFascConsentita = dataRow["CHA_CONSENTI_CLASS"].ToString();
                }
                else
                {
                    objFascicolo.isFascConsentita = null;
                }
            }

            // Gestione Classificazione consentita
            if (dataRow.Table.Columns.Contains("CHA_CONSENTI_FASC"))
            {
                if (dataRow["CHA_CONSENTI_FASC"] != DBNull.Value)
                {
                    objFascicolo.isFascicolazioneConsentita = dataRow["CHA_CONSENTI_FASC"].ToString().Equals("1");
                }
                else
                {
                    objFascicolo.isFascicolazioneConsentita = true;
                }
            }

            // Caricamento note del fascicolo
            FetchNoteFascicolo(infoUtente, objFascicolo);

            //Data Scadenza
            if (dataRow.Table.Columns.Contains("DTA_SCADENZA"))
                objFascicolo.dtaScadenza = dataRow["DTA_SCADENZA"].ToString();

            objFascicolo.creatoreFascicolo = GetCreatoreFascicolo(dataRow);


            //Num Fascicolo
            if (dataRow.Table.Columns.Contains("NUM_FASCICOLO"))
                objFascicolo.numFascicolo = dataRow["NUM_FASCICOLO"].ToString();

            //Aggiunta campo visilbità dell'utente. Se 1 l'utente può vedere il fascicolo
            if (dataRow.Table.Columns.Contains("SICUREZZA"))
                objFascicolo.sicurezzaUtente = dataRow["SICUREZZA"].ToString();

            if (dataRow.Table.Columns.Contains("CHA_FASC_PRIMARIA"))
            {
                if (dataRow["CHA_FASC_PRIMARIA"] != DBNull.Value)
                {
                    objFascicolo.isFascPrimaria = dataRow["CHA_FASC_PRIMARIA"].ToString();
                }
            }

            if (dataRow.Table.Columns.Contains("DTA_CREAZIONE"))
            {
                objFascicolo.dataCreazione = dataRow["DTA_CREAZIONE"].ToString();
            }

            objFascicolo.InAreaLavoro = GetIsProjectInADLUtente(objFascicolo.systemID, infoUtente);

            //***************************************************************
            //GIORDANO IACOZZILLI
            //17/07/2013
            //Gestione dell'icona della copia del docuemnto/fascicolo in deposito.
            //+FIX per le 100000 di query che ghettano il fascicolo.
            //***************************************************************
            if (dataRow.Table.Columns.Contains("CHA_IN_ARCHIVIO"))
            {
                objFascicolo.inArchivio = dataRow["CHA_IN_ARCHIVIO"] != null ? dataRow["CHA_IN_ARCHIVIO"].ToString() : "0";
            }
            //***************************************************************
            //FINE
            //***************************************************************

            //ABBATANGELI GIANLUIGI - gestione applicazioni esterne
            objFascicolo.codiceApplicazione = dataRow["COD_EXT_APP"].ToString();
            if (extAppControlEnabled && (!string.IsNullOrEmpty(objFascicolo.codiceApplicazione)))
            {
                objFascicolo.accessRights = (string.Compare(objFascicolo.codiceApplicazione, infoUtente.codWorkingApplication) == 0 ? objFascicolo.accessRights : "45");
                //objFascicolo.accessRights = (sameApplication(objFascicolo.codiceApplicazione, infoUtente.extApplications) ? objFascicolo.accessRights : "45");
            }

            //Reperimento atipicità del documento
            objFascicolo.InfoAtipicita = DocsPaDB.Query_DocsPAWS.Documentale.GetInfoAtipicita(dataRow, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO);

            return objFascicolo;
        }

        //Abbatangeli Gianluigi - funzione che confronta le applicazioni tra oggetto ed utente)
        private static bool sameApplication(string codAppFasc, ArrayList applicationList)
        {
            bool esito = false;

            foreach (DocsPaVO.utente.ExtApplication extapp in applicationList)
            {
                if (extapp.codice == codAppFasc)
                {
                    esito = true;
                    break;
                }
            }

            return esito;
        }

        //Cerca dato l'id di un documento fascicolato quale sia il nome del fascicolo definito come principale
        public string GetFascicolazionePrimaria(DocsPaVO.utente.InfoUtente infoUtente, string idProfile)
        {
            string descrizione = "";
            string sicurezza = "0";
            string codice = "";
            try
            {
                string queryString;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_FASC_PRIMARIA");
                q.setParam("idProfile", idProfile);
                q.setParam("idPeople", infoUtente.idPeople);
                q.setParam("idGruppo", infoUtente.idGruppo);
                queryString = q.getSQL();
                logger.Debug(queryString);
                using (DBProvider dbProvider = new DBProvider())
                {
                    IDataReader dr = dbProvider.ExecuteReader(queryString);
                    if (dr != null && dr.FieldCount > 0)
                    {
                        while (dr.Read())
                        {
                            descrizione = dr.GetValue(0).ToString();
                            sicurezza = dr.GetValue(1).ToString();
                            codice = dr.GetValue(2).ToString();
                        }
                    }
                }
                if (sicurezza.Equals("0"))
                    descrizione = codice + " - Descrizione non visualizzabile";
                else
                    descrizione = codice + " " + descrizione;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
            return descrizione;
        }

        //recupera il codice di classificazione di un documento dato il suo id, recupera quello principale nel caso
        //la fasc principale sia attiva altrimenti recupera l'ultima classifica apportata al doc in ordine di tempo
        public string GetfascicolazioneFromDoc(string idProfile)
        {
            string codiceClassifica = string.Empty;
            try
            {

                string queryString;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_CODICE_CLASSIFICA");
                q.setParam("idProfile", idProfile);
                queryString = q.getSQL();
                logger.Debug(queryString);
                using (DBProvider dbProvider = new DBProvider())
                {
                    using (IDataReader dr = dbProvider.ExecuteReader(queryString))
                    {
                        if (dr != null && dr.FieldCount > 0)
                        {
                            while (dr.Read())
                            {
                                codiceClassifica = dr.GetValue(0).ToString();

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return codiceClassifica;
        }

        /// Per un dato documento classificato in più fascicoli imposta come fascicolo principale un nuovo dato fascicolo
        public bool CambiaFascicolazionePrimaria(DocsPaVO.utente.InfoUtente infoUtente, string idProject, string idProfile)
        {
            bool result = false;
            try
            {
                string queryString;
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("ANNULLA_FASCICOLAZIONE_PRIMARIA");
                q.setParam("param1", idProfile);
                queryString = q.getSQL();
                logger.Debug(queryString);
                if (this.ExecuteNonQuery(queryString))
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("NUOVA_FASCICOLAZIONE_PRIMARIA");
                    q.setParam("param1", idProfile);
                    q.setParam("idProject", idProject);
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    result = this.ExecuteNonQuery(queryString);
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }
            return result;
        }

        public static DocsPaVO.utente.Corrispondente GetCorrispondenteUfficioReferenteFascicolo(string systemId)
        {

            DocsPaVO.utente.Corrispondente corrUr = new DocsPaVO.utente.Corrispondente();
            corrUr.systemId = systemId;
            return corrUr;
        }

        /// <summary>
        /// Ritorna una lista di fascicoli con un determinato codice
        /// </summary>
        /// <param name="codiceFascicolo"></param>
        /// <param name="registro"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns>ArrayList di fascicoli o 'null' se si è verificato un errore.</returns>
        public ArrayList GetListaFascicoliDaCodice(DocsPaVO.utente.InfoUtente infoUtente, string codiceFascicolo, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione, string insRic)
        {
            logger.Debug("getFascicoloDaCodice");
            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;



            Hashtable hashFasc = new Hashtable();
            ArrayList listaFascicoli = new ArrayList();
            try
            {

                string queryString = GetQueryFascicoli(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, registro, enableUffRef, enableProfilazione, codiceFascicolo, insRic);


                System.Data.DataSet dataSet;
                this.ExecuteQuery(out dataSet, "PROJECT", queryString);
                foreach (System.Data.DataRow dataRow in dataSet.Tables["PROJECT"].Rows)
                {
                    fascicolo = GetFascicolo(infoUtente, dataSet, dataRow, enableProfilazione);
                    if (!hashFasc.ContainsKey(fascicolo.systemID))
                    {
                        hashFasc.Add(fascicolo.systemID, fascicolo);
                        listaFascicoli.Add(fascicolo);

                    }
                }
                dataSet.Dispose();

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);

                logger.Debug("F_System");


                listaFascicoli = null;
            }


            return listaFascicoli;
        }

        /// <summary>
        /// Ritorna una lista di fascicoli con un determinato codice
        /// </summary>
        /// <param name="codiceFascicolo"></param>
        /// <param name="registro"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns>ArrayList di fascicoli o 'null' se si è verificato un errore.</returns>
        public ArrayList GetListaFascicoliDaCodice(DocsPaVO.utente.InfoUtente infoUtente, string codiceFascicolo, DocsPaVO.utente.Registro[] registro, bool enableUffRef, bool enableProfilazione, bool daGrigio)
        {
            logger.Debug("getFascicoloDaCodice");
            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;



            Hashtable hashFasc = new Hashtable();
            ArrayList listaFascicoli = new ArrayList();
            try
            {

                string queryString = GetQueryFascicoli(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, registro, enableUffRef, enableProfilazione, codiceFascicolo, daGrigio);


                System.Data.DataSet dataSet;
                this.ExecuteQuery(out dataSet, "PROJECT", queryString);
                foreach (System.Data.DataRow dataRow in dataSet.Tables["PROJECT"].Rows)
                {
                    fascicolo = GetFascicolo(infoUtente, dataSet, dataRow, enableProfilazione);
                    if (!hashFasc.ContainsKey(fascicolo.systemID))
                    {
                        hashFasc.Add(fascicolo.systemID, fascicolo);
                        listaFascicoli.Add(fascicolo);

                    }
                }
                dataSet.Dispose();

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);

                logger.Debug("F_System");


                listaFascicoli = null;
            }


            return listaFascicoli;
        }

        /// <summary>
        /// Ritorna un fascicolo con un determinato codice
        /// </summary>
        /// <param name="codiceFascicolo"></param>
        /// <param name="registro"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns>Fascicolo o 'null' se si è verificato un errore.</returns>
        public ArrayList GetFascicoloDaCodice3(string idAmministrazione, string idGruppo, string idPeople, string codiceFascicolo, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione)
        {
            logger.Debug("getFascicoloDaCodice");
            ArrayList listaFascicoli = new ArrayList();

            try
            {
                string queryString = GetQueryFascicoli(idAmministrazione, idGruppo, idPeople, registro, enableUffRef, enableProfilazione, codiceFascicolo, "R");


                System.Data.DataSet dataSet;
                this.ExecuteQuery(out dataSet, "PROJECT", queryString);
                foreach (System.Data.DataRow dataRow in dataSet.Tables["PROJECT"].Rows)
                {
                    DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();
                    fascicolo = GetFascicolo2(dataSet, dataRow, enableProfilazione);
                    listaFascicoli.Add(fascicolo);

                }
                dataSet.Dispose();

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("F_System");
            }

            return listaFascicoli;

        }


        /// <summary>
        /// Ritorna un fascicolo con un determinato codice
        /// </summary>
        /// <param name="codiceFascicolo"></param>
        /// <param name="registro"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns>Fascicolo o 'null' se si è verificato un errore.</returns>
        public DocsPaVO.fascicolazione.Fascicolo GetFascicoloDaCodice2(string idAmministrazione, string idGruppo, string idPeople, string codiceFascicolo, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione, string idTitolario)
        {
            logger.Debug("getFascicoloDaCodice");
            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;

            try
            {
                string queryString = GetQueryFascicoli2(idAmministrazione, idGruppo, idPeople, registro, enableUffRef, enableProfilazione, codiceFascicolo, idTitolario);


                System.Data.DataSet dataSet;
                this.ExecuteQuery(out dataSet, "PROJECT", queryString);
                foreach (System.Data.DataRow dataRow in dataSet.Tables["PROJECT"].Rows)
                {
                    fascicolo = GetFascicolo2(dataSet, dataRow, enableProfilazione);

                }
                dataSet.Dispose();

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);

                logger.Debug("F_System");

                fascicolo = null;

            }

            return fascicolo;

        }

        /// <summary>
        /// Ritorna un fascicolo con un determinato codice
        /// </summary>
        /// <param name="codiceFascicolo"></param>
        /// <param name="registro"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns>Fascicolo o 'null' se si è verificato un errore.</returns>
        public DocsPaVO.fascicolazione.Fascicolo GetFascicoloDaCodice(DocsPaVO.utente.InfoUtente infoUtente, string codiceFascicolo, DocsPaVO.utente.Registro registro, bool enableUffRef, bool enableProfilazione)
        {
            logger.Debug("getFascicoloDaCodice");
            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;

            try
            {
                string queryString = GetQueryFascicoli(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, registro, enableUffRef, enableProfilazione, codiceFascicolo, "R");


                System.Data.DataSet dataSet;
                this.ExecuteQuery(out dataSet, "PROJECT", queryString);
                foreach (System.Data.DataRow dataRow in dataSet.Tables["PROJECT"].Rows)
                {
                    fascicolo = GetFascicolo(infoUtente, dataSet, dataRow, enableProfilazione);

                }
                dataSet.Dispose();

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);

                logger.Debug("F_System");

                fascicolo = null;

            }

            return fascicolo;

        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="codiceFascicolo"></param>
        /// <param name="registro"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns>Fascicolo o 'null' se si è verificato un errore.</returns>
        public DocsPaVO.fascicolazione.Fascicolo GetFascicoloById(string idFascicolo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            logger.Debug("GetFascicoloById");
            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;

            try
            {
                string query = "";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT_FASCICOLOByID");
                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
                q.setParam("param3", idFascicolo);
                q.setParam("param4", infoUtente.idPeople);
                q.setParam("param5", infoUtente.idGruppo);
                q.setParam("param6", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CREAZIONE", false));

                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                q.setParam("idRuoloPubblico", !string.IsNullOrEmpty(idRuoloPubblico) ? idRuoloPubblico : "0");

                // per SQL
                if (dbType.ToUpper() == "SQL")
                {
                    q.setParam("dbuser", getUserDB());
                }

                query = q.getSQL();
                logger.Debug(query);
                System.Data.DataSet dataSet;
                this.ExecuteQuery(out dataSet, "PROJECT", query);
                if (dataSet.Tables["PROJECT"].Rows.Count > 0)
                {
                    fascicolo = GetFascicolo(infoUtente, dataSet, dataSet.Tables["PROJECT"].Rows[0]);
                    SetDataVistaSP(infoUtente, fascicolo.systemID, "F");
                }
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("F_System");

                fascicolo = null;
            }

            return fascicolo;
        }

        /// <summary>
        /// Reperimento di un fascicolo da systemID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <param name="registro"></param>
        /// <param name="enableUffRef"></param>
        /// <returns></returns>
        public DocsPaVO.fascicolazione.Fascicolo GetFascicolo(string id,
                                                              DocsPaVO.utente.InfoUtente infoUtente,
                                                              DocsPaVO.utente.Registro registro,
                                                              bool enableUffRef,
                                                              bool enableProfilazione)
        {
            logger.Debug("GetFascicolo");

            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_FASCICOLO_BY_SYSTEM_ID");

                queryDef.setParam("dataApertura", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                queryDef.setParam("dataChiusura", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
                queryDef.setParam("systemID", id);
                queryDef.setParam("idAmministrazione", infoUtente.idAmministrazione);
                queryDef.setParam("idGruppo", infoUtente.idGruppo);
                queryDef.setParam("idPeople", infoUtente.idPeople);
                // per SQL
                if (dbType.ToUpper() == "SQL")
                {
                    queryDef.setParam("dbuser", getUserDB());
                }

                string commandText = queryDef.getSQL();

                string filterRegistro = "A.ID_REGISTRO IS NULL";
                if (registro != null)
                    filterRegistro += " OR A.ID_REGISTRO=" + registro.systemId;

                commandText += " AND (" + filterRegistro + ")";

                logger.Debug("S_GET_FASCICOLO_BY_SYSTEM_ID: " + commandText);

                DataSet ds;

                if (this.ExecuteQuery(out ds, "TableFascicolo", commandText))
                {
                    if (ds.Tables["TableFascicolo"].Rows.Count > 0)
                        fascicolo = GetFascicolo(infoUtente, ds, ds.Tables["TableFascicolo"].Rows[0], enableProfilazione);
                }
                else
                {
                    throw new ApplicationException("Errore in esecuzione della query 'S_GET_FASCICOLO_BY_SYSTEM_ID'");
                }

                // old 12/11/2007 -  SetDataVista(idPeople, id);
                SetDataVistaSP(infoUtente, id, "F");
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return fascicolo;
        }

        public DocsPaVO.fascicolazione.Fascicolo GetFascicoloInClassifica(DocsPaVO.utente.InfoUtente infoUtente, string codiceFascicolo, string idRegistro, bool enableUffRef, string idTitolario, bool enableProfilazione, string systemId)
        {
            logger.Debug("GetFascicoloInClassifica");
            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;

            try
            {
                System.Data.DataSet dataSet;

                string queryString = GetQueryFascicoloInClassifica(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, idRegistro, enableUffRef, codiceFascicolo, idTitolario, systemId);

                this.ExecuteQuery(out dataSet, "DOC_CLASSIFICATI", queryString);

                if (dataSet.Tables["DOC_CLASSIFICATI"].Rows.Count > 0)
                {
                    System.Data.DataRow dataRow = dataSet.Tables["DOC_CLASSIFICATI"].Rows[0];
                    //foreach (System.Data.DataRow dataRow in dataSet.Tables["DOC_CLASSIFICATI"].Rows)
                    //{
                    fascicolo = GetFascicolo(infoUtente, dataSet, dataRow, enableProfilazione);
                    //}
                }
                dataSet.Dispose();

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);

                logger.Debug("F_System");

                fascicolo = null;
            }

            return fascicolo;
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="objClassificazione"></param>
        /// <param name="infoUtente"></param>
        /// <param name="objListaFiltri"></param>
        /// <param name="childs"></param>
        /// <param name="debug"></param>
        /// <returns>ArrayList o 'null' se si è verificato un errore.</returns>
        public System.Collections.ArrayList GetListaFascicoli(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Classificazione objClassificazione, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, bool enableUffRef, bool enableProfilazione, bool childs, byte[] datiExcel, string serverPath)
        {
            logger.Debug("getListaFascicoli");
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();
            try
            {
                string queryString = "";
                //Controllo inserito per la gestione dei multipla dei titolari
                if (objClassificazione != null)
                    queryString = GetQueryListaFascicoli(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, objClassificazione.registro, enableUffRef, enableProfilazione, objClassificazione.varcodliv1, childs);
                else
                    queryString = GetQueryFascicoli(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, null, enableUffRef, enableProfilazione, null, "R");
                string withClause = String.Empty;
                GetSqlQuery(infoUtente.idGruppo, infoUtente.idPeople, objListaFiltri, false, ref queryString, out withClause);

                //FILTRO EXCEL
                getFiltroExcel(infoUtente.idAmministrazione, ref queryString, datiExcel, serverPath, objListaFiltri);

                queryString += " ORDER BY A.DTA_CREAZIONE DESC";

                if (!String.IsNullOrEmpty(withClause))
                    queryString = withClause + " " + queryString;

                logger.Debug(queryString);
                System.Data.DataSet dataSet = new System.Data.DataSet();
                this.ExecuteQuery(out dataSet, "PROJECT", queryString);



                //creazione della lista oggetti
                foreach (System.Data.DataRow dataRow in dataSet.Tables["PROJECT"].Rows)
                {
                    listaFascicoli.Add(GetFascicolo(infoUtente, dataSet, dataRow, enableProfilazione));
                }
                dataSet.Dispose();

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();

                //throw new Exception("F_System");
                logger.Debug("F_System");

                listaFascicoli = null;
            }

            return listaFascicoli;
        }

        public string filtroExcel(byte[] datiExcel, string nomeFile, string nomeAttributo, string serverPath)
        {
            string valoriAttributo = string.Empty;
            OleDbConnection xlsConn = new OleDbConnection();
            OleDbDataReader xlsReader = null;
            try
            {
                //Creazione directory nel caso in cui non esista
                if (Directory.Exists(serverPath))
                {
                    FileStream fs1 = new FileStream(serverPath + "\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs1.Write(datiExcel, 0, datiExcel.Length);
                    fs1.Close();
                }
                else
                {
                    Directory.CreateDirectory(serverPath + "\\");
                    FileStream fs1 = new FileStream(serverPath + "\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    fs1.Write(datiExcel, 0, datiExcel.Length);
                    fs1.Close();
                }
                logger.Debug("Metodo \"filtroExcel\" classe \"Fascicoli\" : inizio lettura file ");
                //Lettura del file appena scritto 
                //xlsConn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + serverPath + "\\" + nomeFile + ";Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                xlsConn.ConnectionString = "Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source=" + serverPath + "\\" + nomeFile + ";Extended Properties='" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "IMEX=1'";
                xlsConn.Open();
                OleDbCommand xlsCmd;
                xlsCmd = new OleDbCommand("select * from [FASCICOLI$]", xlsConn);
                xlsReader = xlsCmd.ExecuteReader();

                //Esistono dei record nel foglio excel
                if (xlsReader.HasRows)
                {
                    if (nomeAttributo.StartsWith("TIPOLOGIA"))
                    {
                        string[] attributi = nomeAttributo.Split('&');
                        nomeAttributo = attributi[3];
                    }
                    //Ricerca della posizione della colonna associata all'attributo selezionato
                    int pos = 0;
                    for (int i = 0; i < xlsReader.FieldCount; i++)
                    {
                        if (xlsReader.GetName(i).ToUpper() == nomeAttributo.ToUpper())
                        {
                            pos = i;
                            break;
                        }
                    }
                    while (xlsReader.Read())
                    {
                        //Controllo se si è arrivati all'ultima riga
                        if (get_string(xlsReader, 0) == "/")
                            break;
                        if (!string.IsNullOrEmpty(get_string(xlsReader, pos)))
                        {
                            if (nomeAttributo.ToUpper() == "NUMERO_FASCICOLO")
                                valoriAttributo += get_string(xlsReader, pos) + ",";
                            else
                                if (nomeAttributo.ToUpper() == "DATA_APERTURA")
                                {
                                    valoriAttributo += "'" + Convert.ToDateTime(get_string(xlsReader, pos)).ToShortDateString() + "',";
                                    //valoriAttributi = valoriAttributi.Substring(0, 11);
                                }
                                else
                                    valoriAttributo += "'" + get_string(xlsReader, pos) + "',";
                        }
                    }
                    valoriAttributo = valoriAttributo.Substring(0, valoriAttributo.Length - 1);
                    logger.Debug("Metodo \"filtroExcel\" classe \"Fascicoli\" : fine lettura file ");
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Metodo \"filtroExcel\" classe \"Fascicoli\" ERRORE : " + ex.Message);
            }
            finally
            {
                xlsReader.Close();
                xlsConn.Close();
            }
            return valoriAttributo;
        }

        private static string get_string(OleDbDataReader dr, int field)
        {
            if (dr[field] == null || dr[field] == System.DBNull.Value)
                return "";
            else
                return dr[field].ToString().Trim();
        }

        public System.Collections.ArrayList GetListaFascicoli(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Classificazione objClassificazione, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, bool enableUffRef, bool enableProfilazione, bool childs, DocsPaVO.utente.Registro registro, byte[] datiExcel, string serverPath)
        {

            logger.Debug("getListaFascicoli");
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

            try
            {
                string queryString = string.Empty;

                if (objClassificazione != null)
                    queryString = GetQueryListaFascicoli(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, objClassificazione.registro, enableUffRef, enableProfilazione, objClassificazione.varcodliv1, childs);
                else
                    queryString = GetQueryFascicoli(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, registro, enableUffRef, enableProfilazione, null, "R");

                String withClause = String.Empty;
                GetSqlQuery(infoUtente.idGruppo, infoUtente.idPeople, objListaFiltri, false, ref queryString, out withClause);

                //FILTRO EXCEL
                getFiltroExcel(infoUtente.idAmministrazione, ref queryString, datiExcel, serverPath, objListaFiltri);

                queryString += " ORDER BY A.DTA_CREAZIONE DESC";

                if (!String.IsNullOrEmpty(withClause))
                    queryString = withClause + " " + queryString;

                logger.Debug(queryString);

                System.Data.DataSet dataSet = new System.Data.DataSet();
                this.ExecuteQuery(out dataSet, "PROJECT", queryString);

                //creazione della lista oggetti
                foreach (System.Data.DataRow dataRow in dataSet.Tables["PROJECT"].Rows)
                {
                    listaFascicoli.Add(GetFascicolo(infoUtente, dataSet, dataRow, enableProfilazione));
                }
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                listaFascicoli = null;
            }

            return listaFascicoli;
        }

        private void getFiltroExcel(string idAmm, ref string queryString, byte[] datiExcel, string serverPath, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri)
        {
            //RICERCA FASCICOLO FILTRATA CON ELENCO VALORI SU FILE EXCEL
            if (datiExcel != null && !string.IsNullOrEmpty(serverPath))
            {
                string nomeFile = string.Empty;
                string nomeAttributo = string.Empty;
                //ricerca nel filtro il nome del fileExcel e la colonna del fileExcel da importare
                DocsPaVO.filtri.FiltroRicerca f;
                for (int i = 0; i < objListaFiltri.Length; i++)
                {
                    f = objListaFiltri[i];
                    if (!string.IsNullOrEmpty(f.valore))
                    {
                        if (f.argomento == "FILE_EXCEL")
                        {
                            nomeFile = f.valore;
                        }
                        if (f.argomento == "ATTRIBUTO_EXCEL")
                        {
                            nomeAttributo = f.valore;
                        }
                    }
                }

                //Chiamata al metodo per la gestione della lettura dei valori inseriti nel foglio excel
                if (!string.IsNullOrEmpty(nomeFile))
                {
                    string valoriAttributi = string.Empty;
                    //restituisce una stringa con tutti i valori della colonna del foglio excel che si vuole importare
                    valoriAttributi = filtroExcel(datiExcel, nomeFile, nomeAttributo, serverPath);
                    if (!string.IsNullOrEmpty(valoriAttributi))
                    {
                        string[] attributiTipologia;
                        if (nomeAttributo.StartsWith("TIPOLOGIA"))
                        {
                            attributiTipologia = nomeAttributo.Split('&');
                            string tipologia = "";
                            DocsPaDB.Query_DocsPAWS.ModelFasc mdFasc = new ModelFasc();
                            DocsPaVO.ProfilazioneDinamica.Templates profilo = mdFasc.getTemplateFascById(attributiTipologia[2]);

                            if (profilo != null && profilo.IPER_FASC_DOC == "1")
                            {
                                tipologia = "";
                            }
                            else

                                tipologia = " AND a.id_tipo_fasc = " + attributiTipologia[2];


                            //E' il valore dell'oggetto della tipologia
                            nomeAttributo = attributiTipologia[3];
                            //verifica del tipo_oggetto
                            string tipo_oggetto = getTipoOggettoFasc(attributiTipologia[4]);


                            if (tipo_oggetto == "Corrispondente")

                                //queryString += tipologia + " and exists (select tf.SYSTEM_ID from dpa_ass_templates_fasc tf where  a.id_tipo_fasc=tf.id_template " +
                                //          " and tf.id_oggetto = " + attributiTipologia[4] + " and a.system_id=tf.id_project " +
                                //          " AND tf.VALORE_OGGETTO_DB in (select system_id from dpa_corr_globali where (ID_AMM IS NULL OR ID_AMM = " + idAmm + ")" +
                                //          " and upper(var_cod_rubrica) in (" + valoriAttributi + "))) ";

                                queryString += tipologia + " and exists (select tf.SYSTEM_ID from dpa_ass_templates_fasc tf where  a.id_tipo_fasc=tf.id_template " +
                                        " and tf.id_oggetto = " + attributiTipologia[4] + " and a.system_id=tf.id_project " +
                                        " AND tf.VALORE_OGGETTO_DB in (select system_id FROM dpa_corr_globali u WHERE " +
                                        " system_id in (SELECT system_id FROM dpa_corr_globali START WITH system_id = system_id  and (id_amm IS NULL OR id_amm = " + idAmm + ") and UPPER (var_cod_rubrica) IN (" + valoriAttributi + ") CONNECT BY PRIOR id_old = system_id))) ";
                            else

                                queryString += tipologia +
                                        " and exists (select tf.SYSTEM_ID from dpa_ass_templates_fasc tf where  a.id_tipo_fasc=tf.id_template " +
                                        " and tf.id_oggetto = " + attributiTipologia[4] + " and a.system_id=tf.id_project " +
                                        " AND tf.VALORE_OGGETTO_DB in (" + valoriAttributi + "))";

                        }
                        else
                        {
                            switch (nomeAttributo)
                            {
                                case "NUMERO_FASCICOLO":
                                    queryString += " and A.NUM_FASCICOLO IN ( " + valoriAttributi + ") ";
                                    break;
                                case "DATA_APERTURA":
                                    //valoriAttributi = valoriAttributi.Substring(0, 11);
                                    queryString += " and to_char(a.DTA_APERTURA, 'dd/mm/yyyy') IN ( " + valoriAttributi + ") ";
                                    break;
                                case "DESCRIZIONE_FASCICOLO":
                                    queryString += " and a.DESCRIPTION IN ( " + valoriAttributi + ") ";
                                    break;
                                case "CODICE_NODO":
                                    queryString += " and A.VAR_CODICE IN ( " + valoriAttributi + ") ";
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private string getTipoOggettoFasc(string valoreOggetto)
        {
            string result = string.Empty;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_TIPOOGGETTO_TIPOFASC");
                queryDef.setParam("id_oggetto", valoreOggetto);
                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DBProvider dbProvider = new DBProvider())
                {
                    string outParam;
                    if (dbProvider.ExecuteScalar(out outParam, commandText))
                        result = outParam;
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="objClassificazione"></param>
        /// <param name="registro"></param>
        /// <param name="filtriFascicoli"></param>
        /// <param name="filtriDocumentiInFascicoli">
        /// Filtri per estrarre i fascicoli contenenti i documenti
        /// </param>
        /// <param name="enableUfficioRef"></param>
        /// <param name="enableProfilazione"></param>
        /// <param name="childs"></param>
        /// <param name="numTotPage"></param>
        /// <param name="totalRecordCount"></param>
        /// <param name="requestedPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="getSystemIdList">True se bisogna restituire anche la lista dei system id dei fascicoli restituiti dalla ricerca</param>
        /// <param name="idProjectList">Lista dei system id dei fascicoli restituiti dalla ricerca. Verrà valorizzata solo se getSystemIdList è true</param>
        /// <returns></returns>
        public ArrayList GetListaFascicoliPaging(DocsPaVO.utente.InfoUtente infoUtente,
                            DocsPaVO.fascicolazione.Classificazione objClassificazione,
                            DocsPaVO.utente.Registro registro,
                            DocsPaVO.filtri.FiltroRicerca[] filtriFascicoli,
                            DocsPaVO.filtri.FiltroRicerca[] filtriDocumentiInFascicoli,
                            bool enableUfficioRef,
                            bool enableProfilazione,
                            bool childs,
                            out int numTotPage,
                            out int totalRecordCount,
                            int requestedPage,
                            int pageSize,
                            bool getSystemIdList,
                             out List<SearchResultInfo> idProjectList, byte[] datiExcel, string serverPath)
        {
            logger.Debug("getListaFascicoliPaging");

            ArrayList listaFascicoli = new ArrayList();

            totalRecordCount = 0;
            numTotPage = 0;

            // La lista dei system id da restituire
            List<SearchResultInfo> idProjecs = null;

            try
            {
                //parametri di queryString Comuni.
                string queryString = "";

                //nuova paginazione:
                //prima la count con i filtri.
                queryString = setCommonParameter(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople,
                                                 objClassificazione, registro,
                                                 filtriFascicoli, filtriDocumentiInFascicoli, enableUfficioRef,
                                                 enableProfilazione, childs,
                                                 out numTotPage, 0,
                                                 totalRecordCount,
                                                 "DATACOUNT", pageSize, getSystemIdList, datiExcel, serverPath);

                if (getSystemIdList)
                {
                    //eseguo la query count
                    using (DBProvider dbProvider = new DBProvider())
                    {
                        string field = string.Empty;
                        //if (dbProvider.ExecuteScalar(out field, queryString))
                        //    totalRecordCount = Convert.ToInt32(field);
                        IDataReader dr = dbProvider.ExecuteReader(queryString);

                        // Se sono richiesti i system id dei fascicoli, viene inizializzata
                        // la lista
                        if (getSystemIdList)
                            idProjecs = new List<SearchResultInfo>();

                        while (dr.Read())
                        {
                            field = dr.GetValue(0).ToString();

                            // Se è richiesta la lista dei system id dei fascicoli,
                            // viene aggiunto field alla lista dei system id
                            if (getSystemIdList)
                            {
                                SearchResultInfo temp = new SearchResultInfo();
                                temp.Id = field;
                                temp.Codice = dr.GetValue(1).ToString();
                                idProjecs.Add(temp);
                            }
                        }

                        // Se è richiesta la lista dei system id dei fascicoli, viene
                        // calcolato il numero di risultati restituiti dalla ricerca
                        if (getSystemIdList)
                            field = idProjecs.Count.ToString();

                        if (field != string.Empty)
                            totalRecordCount = Convert.ToInt32(field);
                    }
                }
                else
                {
                    using (DBProvider dbProvider = new DBProvider())
                    {
                        IDataReader dr = dbProvider.ExecuteReader(queryString);
                        while (dr.Read())
                        {
                            totalRecordCount = Convert.ToInt32(dr.GetValue(0).ToString());
                        }
                    }
                }

                if (totalRecordCount >= 0)//> 0)
                {
                    logger.Debug("Trovati " + totalRecordCount + " Fascicoli");
                    //poi la fill con i filtri
                    //procedo con la fill dei dati 
                    queryString = setCommonParameter(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople,
                                                objClassificazione, registro,
                                                filtriFascicoli, filtriDocumentiInFascicoli, enableUfficioRef,
                                                enableProfilazione, childs,
                                                out numTotPage,
                                                requestedPage,
                                                totalRecordCount,
                                                 "DATAFILL", pageSize, false, datiExcel, serverPath);


                    DataSet ds = new DataSet();
                    using (DBProvider dbProvider = new DBProvider())
                    {
                        dbProvider.ExecuteQuery(ds, queryString);

                        if (ds != null && ds.Tables[0] != null)
                        {
                            foreach (System.Data.DataRow dataRow in ds.Tables[0].Rows)
                            {
                                listaFascicoli.Add(GetFascicoloLite(infoUtente, ds, dataRow, enableProfilazione));
                            }
                        }

                        ds.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);

                listaFascicoli = null;
            }

            // Salvataggio della lista dei system id
            idProjectList = idProjecs;

            return listaFascicoli;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="objClassificazione"></param>
        /// <param name="registro"></param>
        /// <param name="filtriFascicoli"></param>
        /// <param name="enableUfficioRef"></param>
        /// <param name="enableProfilazione"></param>
        /// <param name="childs"></param>
        /// <param name="numTotPage"></param>
        /// <param name="totalRecordCount"></param>
        /// <param name="requestedPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="getSystemIdList">True se è richiesta anche la lista dei system id dei fascicoli restituiti dalla ricerca</param>
        /// <param name="idProjectList">Lista dei system id dei fascicoli restituiti dalla ricerca. Verrà valorizzata solo se getSystemIdList è true</param>
        /// <returns></returns>
        public ArrayList GetListaFascicoliPaging(DocsPaVO.utente.InfoUtente infoUtente,
            DocsPaVO.fascicolazione.Classificazione objClassificazione,
            DocsPaVO.utente.Registro registro,
            DocsPaVO.filtri.FiltroRicerca[] filtriFascicoli,
            bool enableUfficioRef,
            bool enableProfilazione,
            bool childs,
            out int numTotPage,
            out int totalRecordCount,
            int requestedPage,
            int pageSize,
            bool getSystemIdList,
             out List<SearchResultInfo> idProjectList, byte[] datiExcel, string serverPath)
        {
            return this.GetListaFascicoliPaging(infoUtente, objClassificazione, registro, filtriFascicoli, null,
                enableUfficioRef, enableProfilazione, childs, out numTotPage, out totalRecordCount, requestedPage, pageSize,
                getSystemIdList, out idProjectList, datiExcel, serverPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtriRicerca"></param>
        /// <param name="nomeFiltro"></param>
        /// <returns></returns>
        private string GetValoreFiltroRicerca(DocsPaVO.filtri.FiltroRicerca[] filtriRicerca, string nomeFiltro)
        {
            foreach (DocsPaVO.filtri.FiltroRicerca filtro in filtriRicerca)
                if (filtro.argomento == nomeFiltro)
                    return filtro.valore;
            return null;
        }

        /// <summary>
        /// Reperimento query per la ricerca dei fascicoli in base ai documenti contenuti
        /// </summary>
        /// <returns></returns>
        private string getQueryFascicoliDocumenti(DocsPaVO.filtri.FiltroRicerca[] filtriRicercaDocumenti)
        {
            string commandText = " SELECT P.ID_PARENT FROM PROJECT P, PROJECT_COMPONENTS PC, PROFILE A {0} WHERE P.SYSTEM_ID = PC.PROJECT_ID AND PC.LINK = A.SYSTEM_ID ";

            string queryFrom = string.Empty;
            string filterString = this.GetFilterStringQueryDocumentiInFascicolo(new DocsPaVO.filtri.FiltroRicerca[1][] { filtriRicercaDocumenti }, ref queryFrom);

            commandText = string.Format(commandText, queryFrom);

            if (!string.IsNullOrEmpty(filterString))
                commandText += filterString;

            return commandText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <param name="objClassificazione"></param>
        /// <param name="registro"></param>
        /// <param name="objListaFiltri"></param>?
        /// <param name="filtriDocumentiInFascicoli"></param>
        /// <param name="enableUfficioRef"></param>
        /// <param name="enableProfilazione"></param>
        /// <param name="childs"></param>
        /// <param name="numTotPage"></param>
        /// <param name="requestedPage"></param>
        /// <param name="totalRecordCount"></param>
        /// <param name="queryType"></param>
        /// <param name="pageSize"></param>
        /// <param name="getSystemIdList">True se bisogna restituire anche la lista dei system id dei fascicoli restituiti dalla ricerca</param>
        /// <returns></returns>
        private string setCommonParameter(string idAmm,
                                          string idGruppo,
                                          string idPeople,
                                          DocsPaVO.fascicolazione.Classificazione objClassificazione,
                                          DocsPaVO.utente.Registro registro,
                                          DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                            DocsPaVO.filtri.FiltroRicerca[] filtriDocumentiInFascicoli,
                                          bool enableUfficioRef,
                                          bool enableProfilazione,
                                          bool childs,
                                          out int numTotPage,
                                          int requestedPage,
                                          int totalRecordCount,
                                          string queryType,
                                          int pageSize,
                                           bool getSystemIdList, byte[] datiExcel, string serverPath)
        {
            numTotPage = 0;
            try
            {
                DocsPaUtils.Query q = null;
                string queryString = string.Empty;
                int startRow = 0;
                int endRow = 0;
                string filterString = string.Empty;
                string filterString1 = string.Empty;
                bool cons = false;
                bool mancCons = false;
                int sysId = 0;
                int caseDescr = 0;
                bool UOsottoposte = false;

                foreach (DocsPaVO.filtri.FiltroRicerca fil in objListaFiltri)
                {
                    if (fil.argomento.Equals("CONSERVAZIONE") && fil.valore != null)
                    {
                        cons = true;
                        if (fil.valore == "0")
                            mancCons = true;
                    }

                    if (fil.argomento.Equals("DESC_PEOPLE_CREATORE") && fil.valore != null && !fil.valore.Equals(""))
                    {
                        string query = "SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE UPPER(VAR_DESC_CORR) LIKE '%" + fil.valore.ToUpper() + "%'";
                        using (DBProvider dbProvider = new DBProvider())
                        {
                            string systId = string.Empty;
                            if (dbProvider.ExecuteScalar(out systId, query))
                            {
                                if (!string.IsNullOrEmpty(systId))
                                    sysId = Convert.ToInt32(systId);
                            }
                        }
                        caseDescr = 1;
                    }

                    if (fil.argomento.Equals("DESC_RUOLO_CREATORE") && fil.valore != null && !fil.valore.Equals(""))
                    {
                        string query = "SELECT ID_GRUPPO FROM DPA_CORR_GLOBALI WHERE UPPER(VAR_DESC_CORR) LIKE '%" + fil.valore.ToUpper() + "%'";
                        using (DBProvider dbProvider = new DBProvider())
                        {
                            string systId = string.Empty;
                            if (dbProvider.ExecuteScalar(out systId, query))
                            {
                                if (!string.IsNullOrEmpty(systId))
                                    sysId = Convert.ToInt32(systId);
                            }
                        }
                        caseDescr = 2;

                    }

                    if (fil.argomento.Equals("DESC_UO_CREATORE") && fil.valore != null && !fil.valore.Equals(""))
                    {
                        string query = "SELECT ID_UO FROM DPA_CORR_GLOBALI WHERE UPPER(VAR_DESC_CORR) LIKE '%" + fil.valore.ToUpper() + "%'";
                        using (DBProvider dbProvider = new DBProvider())
                        {
                            string systId = string.Empty;
                            if (dbProvider.ExecuteScalar(out systId, query))
                            {
                                if (!string.IsNullOrEmpty(systId))
                                    sysId = Convert.ToInt32(systId);
                            }
                        }
                        caseDescr = 3;
                    }

                    if (fil.argomento.Equals("UO_SOTTOPOSTE") && fil.valore != null && !fil.valore.Equals(""))
                        UOsottoposte = true;
                }




                if (queryType.ToUpper() == "DATACOUNT")
                {
                    // solo COUNT dei dati
                    if (cons && mancCons)
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_FASCICOLI_PAGING_UNION");
                    else
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_FASCICOLI_PAGING");
                }

                // Se getSystemIdList è true e queryType è DATACOUNT...
                if (getSystemIdList &&
                    queryType.ToUpper() == "DATACOUNT")
                {
                    // ...viene invocata la query per la restituzione dei system id dei
                    // fascicoli
                    if (cons && mancCons)
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("COUNT_PROJECT_MASSIVE_OPERATION_UNION");
                    else
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("COUNT_PROJECT_MASSIVE_OPERATION");
                }

                if (queryType.ToUpper() == "DATAFILL")
                { // solo FILL dei dati
                    if (cons && mancCons)
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASCICOLI_PAGING_UNION");
                    else
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASCICOLI_PAGING");

                    if (dbType.ToUpper().Equals("SQL"))
                    {
                        q.setParam("adl", this.getUserDB() + ".getInADL(A.SYSTEM_ID,'F'," + idGruppo + "," + idPeople + ") AS IN_ADL ");
                    }
                    else
                    {
                        q.setParam("adl", " getInADL(A.SYSTEM_ID,'F'," + idGruppo + "," + idPeople + ") AS IN_ADL ");
                    }

                    //Laura 9 Aprile
                    if (cons && mancCons && dbType.ToUpper().Equals("SQL"))
                    {
                        string paging = string.Empty;
                        q.setParam("paging", paging);

                        string listDocuments = string.Empty;
                        q.setParam("listDocuments", listDocuments);

                        string valoriCustom = string.Empty;

                        q.setParam("valoriCustom", valoriCustom);
                        String contatore = string.Empty;
                        q.setParam("contatore", contatore);

                        q.setParam("idGruppo", idGruppo);
                        q.setParam("idPeople", idPeople);
                    }

                }





                #region Ordinamento

                // Recupero dei filtri di ricerca relarivi all'ordinamento
                FiltroRicerca oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
                FiltroRicerca sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
                FiltroRicerca profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
                FiltroRicerca orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();

                if (orderDirection == null)
                {
                    orderDirection = new FiltroRicerca()
                    {
                        argomento = "ORDER_DIRECTION",
                        valore = "DESC"

                    };

                }

                // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
                String extractFieldValue = String.Empty;

                // Ordinamento ed ordinamento inverso
                String order = String.Empty, reverseOrder = String.Empty;

                if (this.dbType == "SQL")
                {
                    // DB SQL Server
                    // Se bisogna ordinare per campo custom...
                    if (profilationField != null)
                    {
                        // ...recupero del dettaglio dell'oggetto custom
                        OggettoCustom obj = new ModelFasc().getOggettoById(profilationField.valore);

                        if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                            // ...viene preparata la funzione per estrarre il valore del campo profilato
                            extractFieldValue = String.Format(", Convert(int, @dbuser@.GetValProfObjPrj(A.SYSTEM_ID, {0})) AS CUSTOM_FIELD", profilationField.valore);
                        else
                            // ...viene preparata la funzione per estrarre il valore del campo profilato
                            extractFieldValue = String.Format(", @dbuser@.GetValProfObjPrj(A.SYSTEM_ID, {0}) AS CUSTOM_FIELD", profilationField.valore);

                        // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                        order = String.Format("CUSTOM_FIELD {0}", orderDirection.valore);
                        reverseOrder = String.Format("CUSTOM_FIELD {0}", orderDirection.valore == "ASC" ? "DESC" : "ASC");
                    }
                    else
                    {
                        // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                        if (sqlField != null)
                        {
                            // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                            extractFieldValue = String.Format(", {0} AS ORDER_STANDARD", sqlField.valore);
                            order = String.Format("ORDER_STANDARD {0}", orderDirection.valore);
                            reverseOrder = String.Format("ORDER_STANDARD {0}", orderDirection.valore == "ASC" ? "DESC" : "ASC");
                        }
                        else
                        {
                            // Altrimenti viene creato il filtro standard
                            extractFieldValue = String.Empty;
                            order = String.Format("A.DTA_CREAZIONE {0}", orderDirection.valore);
                            reverseOrder = String.Format("A.DTA_CREAZIONE {0}", orderDirection.valore == "ASC" ? "DESC" : "ASC");
                        }
                    }

                }
                else
                {
                    // DB ORACLE
                    // Se bisogna ordinare per campo custom...
                    if (profilationField != null)
                    {
                        // ...recupero del dettaglio dell'oggetto custom
                        OggettoCustom obj = new ModelFasc().getOggettoById(profilationField.valore);

                        if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        {
                            // ...viene preparata la funzione per estrarre il valore del campo profilato
                            extractFieldValue = String.Format(", to_number(GetValProfObjPrj(A.SYSTEM_ID, {0}))", profilationField.valore);
                            // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                            order = String.Format("to_number(GetValProfObjPrj(A.SYSTEM_ID, {0})) {1}", profilationField.valore, orderDirection.valore);
                        }
                        else
                        {
                            // ...viene preparata la funzione per estrarre il valore del campo profilato
                            extractFieldValue = String.Format(", GetValProfObjPrj(A.SYSTEM_ID, {0})", profilationField.valore);
                            // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                            order = String.Format("GetValProfObjPrj(A.SYSTEM_ID, {0}) {1}", profilationField.valore, orderDirection.valore);
                        }

                        reverseOrder = String.Empty;
                    }
                    else
                    {
                        // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                        if (oracleField != null)
                        {
                            // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                            extractFieldValue = String.Empty;
                            order = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                            reverseOrder = String.Empty;
                        }
                        else
                        {
                            // Altrimenti viene creato il filtro standard
                            extractFieldValue = String.Empty;
                            // order = String.Format("A.DTA_CREAZIONE {0}", orderDirection.valore);
                            //  order = String.Format("A.DTA_CREAZIONE {0}", orderDirection.valore);
                            //Nel caso non ho le griglie custum ma ho una tipologia con un campo profilato
                            FiltroRicerca contatoreNoCustom = objListaFiltri.Where(e => e.argomento == "CONTATORE_GRIGLIE_NO_CUSTOM").FirstOrDefault();
                            if (contatoreNoCustom != null)
                            {
                                order = String.Format("getContatoreFascContatore (a.system_id, '" + contatoreNoCustom.valore + "') {0}", orderDirection.valore);
                            }
                            else
                            {
                                order = String.Format("A.DTA_CREAZIONE {0}", orderDirection.valore);
                            }
                            reverseOrder = String.Empty;
                        }
                    }
                }

                #endregion

                if (order.Equals("A.DTA_CREAZIONE DESC") || order.Equals("A.DTA_CREAZIONE ASC") || order.Equals("CHA_TIPO_FASCICOLO DESC") || order.Equals("CHA_TIPO_FASCICOLO ASC") || order.Equals("UPPER(TRIM(DESCRIPTION)) DESC") || order.Equals("UPPER(TRIM(DESCRIPTION)) ASC") || order.Equals("ORDER BY DTA_APERTURA DESC") || order.Equals("ORDER BY DTA_APERTURA ASC") || order.Equals("DTA_CHIUSURA DESC") || order.Equals("DTA_CHIUSURA ASC"))
                {
                    order += ", a.system_id DESC";
                }


                //Laura 9 aprile
                if (this.dbType == "SQL" && order.Equals(string.Empty))
                    order = " A.DTA_CREAZIONE DESC ";


                if (cons && mancCons && dbType.ToUpper().Equals("SQL") && queryType.ToUpper() == "DATAFILL")
                {
                    q.setParam("order", order);
                }

                // Impostazione del parametro per l'estrazione del valore assunto da un campo profilato,
                // per l'ordinamento e per l'ordinamento inverso
                q.setParam("customFieldForOrder", extractFieldValue);
                q.setParam("customOrder", order);
                q.setParam("customOrder1", reverseOrder);


                string userDB = getUserDB();
                if (!string.IsNullOrEmpty(userDB))
                    q.setParam("dbuser", userDB);

                //modifica
                // q.setParam("tipoContatore", "'" + tipo_contatore + "'");
                //fine modifica

                // Ricerca con CodiceClassifica
                if (objClassificazione != null && !string.IsNullOrEmpty(objClassificazione.varcodliv1))
                {
                    q.setParam("tblCL1", " ,(SELECT A.SYSTEM_ID FROM PROJECT A, SECURITY B WHERE " +
                               " (A.SYSTEM_ID = B.THING) AND (B.ACCESSRIGHTS) > 0 AND " +
                               " (B.PERSONORGROUP= @idPeo@ OR B.PERSONORGROUP= @idGrp@ ) " +
                               " AND A.ID_AMM = @idAmm@ AND A.CHA_TIPO_PROJ = 'T' " +
                               " @idReg@ @varCodLiv@) C ");

                    q.setParam("whereTlbCL1", " AND (A.ID_PARENT = C.SYSTEM_ID)");

                    // esiste solo se objClassificazione è valido
                    if (childs) q.setParam("varCodLiv", "AND A.VAR_COD_LIV1 LIKE '" + objClassificazione.varcodliv1 + "%'");
                    else q.setParam("varCodLiv", "AND  A.VAR_COD_LIV1 = '" + objClassificazione.varcodliv1 + "'");
                }
                else
                {
                    q.setParam("tblCL1", "");
                    q.setParam("whereTlbCL1", "");
                }

                //common Where Condition
                q.setParam("idAmm", idAmm);


                //registro
                if (registro != null) q.setParam("idReg", " AND (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO =" + registro.systemId + ")");
                else q.setParam("idReg", "");

                if (queryType == "DATAFILL") // solo FILL dei dati aggiungo i filtri
                {

                    // operazioni Matematiche per calcolo paginazione
                    // Determina il num di pagine totali 

                    numTotPage = (totalRecordCount / pageSize);

                    if (numTotPage != 0)
                    {
                        if ((totalRecordCount % numTotPage) > 0) numTotPage++;
                    }
                    else numTotPage = 1;

                    startRow = ((requestedPage * pageSize) - pageSize) + 1;
                    endRow = (startRow - 1) + pageSize;

                    q.setParam("startRow", startRow.ToString());
                    q.setParam("endRow", endRow.ToString());

                    // INIZIO - Parametri specifici per SqlServer
                    // TODO : rovesciamento criteri di ordinamento dedicati a SQL e count SQL
                    // INIZIO - Parametri specifici per SqlServer
                    // il numero totale di righe da estrarre equivale 
                    // al limite inferiore dell'ultima riga da estrarre


                    int pageSizeSqlServer = pageSize;
                    int totalRowsSqlServer = (requestedPage * pageSize);
                    if ((totalRecordCount - totalRowsSqlServer) <= 0)
                    {
                        pageSizeSqlServer -= System.Math.Abs(totalRecordCount - totalRowsSqlServer);
                        totalRowsSqlServer = totalRecordCount;
                    }

                    q.setParam("pageSize", pageSizeSqlServer.ToString()); // Dimensione pagina
                    q.setParam("totalRows", totalRowsSqlServer.ToString());


                    // FINE - Parametri specifici per SqlServer
                }

                q.setParam("dtaAp", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                q.setParam("dtaCh", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));

                // flitri GUI
                String withClause = String.Empty;
                GetSqlQuery(idGruppo, idPeople, objListaFiltri, UOsottoposte, ref filterString, out withClause);

                //FILTRO EXCEL
                if (datiExcel != null)
                {
                    getFiltroExcel(idAmm, ref filterString, datiExcel, serverPath, objListaFiltri);
                }

                if (cons)
                {
                    if (mancCons)
                    {
                        filterString1 = filterString + " AND A.SYSTEM_ID IN (SELECT DISTINCT ID_PROJECT FROM DPA_ITEMS_CONSERVAZIONE F, PROJECT G WHERE G.SYSTEM_ID = F.ID_PROJECT AND F.CHA_STATO <> 'C')";
                        filterString += " AND A.SYSTEM_ID NOT IN (SELECT DISTINCT ID_PROJECT FROM DPA_ITEMS_CONSERVAZIONE WHERE NOT ID_PROJECT IS NULL)";
                    }
                }
                //q.setParam("profilazione", " ");
                if (sysId >= 0 && caseDescr > 0)
                {
                    switch (caseDescr)
                    {
                        case 1:
                            if (dbType.ToUpper() == "SQL")
                                filterString += " AND " + userDB + ".checkSecurityProprietario(A.SYSTEM_ID, " + sysId + ", 0) = 1";
                            else
                                filterString += " AND checkSecurityProprietario(A.SYSTEM_ID, " + sysId + ", 0) = 1";
                            break;

                        case 2:
                            if (dbType.ToUpper() == "SQL")
                                filterString += " AND " + userDB + ".checkSecurityProprietario(A.SYSTEM_ID, 0, " + sysId + ") = 1";
                            else
                                filterString += " AND checkSecurityProprietario(A.SYSTEM_ID, 0, " + sysId + ") = 1";
                            break;

                        case 3:
                            if (dbType.ToUpper() == "SQL")
                            {
                                //filterString += " AND " + userDB + ".checkSecurityUO(A.SYSTEM_ID, " + sysId + ") = 1";
                                if (sysId != 0)
                                    filterString += " AND ID_UO_CREATORE = " + sysId;
                            }
                            else
                                filterString += " AND checkSecurityUO(A.SYSTEM_ID, " + sysId + ") = 1";
                            break;
                    }
                }

                if (filtriDocumentiInFascicoli != null)
                {
                    string queryFiltriDocumenti = this.getQueryFascicoliDocumenti(filtriDocumentiInFascicoli);

                    // Sono stati forniti dei filtri per la ricerca dei documenti 
                    filterString = string.Concat(filterString,
                                string.Format(" AND A.SYSTEM_ID IN ({0})", queryFiltriDocumenti));
                }

                //aggiungo i filtri GUI al query
                if (filterString != null) q.setParam("guiFilters", filterString);
                if (filterString1 != null) q.setParam("guiFilters1", filterString1);


                string security = string.Empty;
                bool IS_ARCHIVISTA_DEPOSITO;
                DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
                IS_ARCHIVISTA_DEPOSITO = ut.isUtArchivistaDeposito(idPeople, idGruppo);
                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
                if (IS_ARCHIVISTA_DEPOSITO)
                {
                    if (dbType.ToUpper() == "SQL")
                        security = " (@dbuser@.checkSecurity(A.SYSTEM_ID, @idPeo@, @idGrp@, @idRuoloPubblico@,'F') > 0)";
                    else
                        security = " (checkSecurity(A.SYSTEM_ID, @idPeo@, @idGrp@, @idRuoloPubblico@,'F') > 0)";
                }
                else
                {
                    if (IndexSecurity())
                        security = " EXISTS (select /*+INDEX(e) */  'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @idPeo@, @idGrp@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                    else
                        security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @idPeo@, @idGrp@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                }
                if (security == string.Empty)
                {
                    if (IndexSecurity())
                        security = " EXISTS (select /*+INDEX(e) */  'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @idPeo@, @idGrp@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                    else
                        security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @idPeo@, @idGrp@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                }

                q.setParam("security", security);
                q.setParam("idGrp", idGruppo);
                q.setParam("idPeo", idPeople);
                q.setParam("idRuoloPubblico", !string.IsNullOrEmpty(idRuoloPubblico) ? idRuoloPubblico : "0");

                if (dbType.ToUpper().Equals("ORACLE"))
                {
                    if (!string.IsNullOrEmpty(tipo_contatore))
                    {
                        q.setParam("tipoContatore_cond", " , getContatoreFasc(a.system_id,@tipoContatore@) as contatore,getContatoreFasc(a.system_id,@tipoContatore@) as contatoreOrdinamento");
                        q.setParam("tipoContatore", "'" + tipo_contatore + "'");
                    }
                    else
                    {
                        q.setParam("tipoContatore_cond", "");
                        q.setParam("tipoContatore", "''");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(tipo_contatore))
                    {
                        q.setParam("tipoContatore", "'" + tipo_contatore + "'");
                    }
                    else
                    {
                        q.setParam("tipoContatore", "''");
                    }
                }


                //rilascio il query string
                queryString = q.getSQL();

                if (!String.IsNullOrEmpty(withClause))
                    queryString = withClause + " " + queryString;

                logger.Debug(queryType + ": " + queryString);

                return queryString;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return null;
            }
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="objListaFiltri"></param>
        /// <returns></returns>
        private void GetSqlQuery(string idGruppo, string idPeople, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, bool UOsottoposte, ref string queryStr, out string withClause)
        {
            withClause = String.Empty;
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

            if (objListaFiltri == null) return;
            DocsPaVO.filtri.FiltroRicerca f;
            for (int i = 0; i < objListaFiltri.Length; i++)
            {
                f = objListaFiltri[i];
                if (f.valore != null && !f.valore.Equals(""))
                {
                    switch (f.argomento)
                    {
                        case "APERTURA_IL":
                            queryStr += " AND DTA_APERTURA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            queryStr += " AND DTA_APERTURA < " + DocsPaDbManagement.Functions.Functions.ToDate(f.valore + " 23:59:59");

                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "APERTURA_SUCCESSIVA_AL":
                            queryStr += " AND DTA_APERTURA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);

                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "APERTURA_PRECEDENTE_IL":
                            queryStr += " AND DTA_APERTURA<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);

                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "APERTURA_SC":
                            // data apertura nella settimana corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_APERTURA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND DTA_APERTURA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryStr += " AND DTA_APERTURA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND DTA_APERTURA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "APERTURA_MC":
                            // data apertura nel mese corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_APERTURA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND DTA_APERTURA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryStr += " AND DTA_APERTURA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND DTA_APERTURA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "APERTURA_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND to_char(DTA_APERTURA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryStr += " AND DTA_APERTURA>=(SELECT getdate()) AND DTA_APERTURA<=(SELECT getdate()) ";
                            break;
                        case "CHIUSURA_IL":
                            queryStr += " AND DTA_CHIUSURA=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);

                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "CHIUSURA_SUCCESSIVA_AL":
                            queryStr += " AND DTA_CHIUSURA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "CHIUSURA_PRECEDENTE_IL":
                            queryStr += " AND DTA_CHIUSURA<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            break;
                        case "CHIUSURA_SC":
                            // data chiusura nella settimana corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_CHIUSURA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND DTA_CHIUSURA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryStr += " AND DTA_CHIUSURA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND DTA_CHIUSURA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                            break;
                        case "CHIUSURA_MC":
                            // data chiusura nel mese corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_CHIUSURA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND DTA_CHIUSURA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryStr += " AND DTA_CHIUSURA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND DTA_CHIUSURA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "CHIUSURA_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND to_char(DTA_CHIUSURA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryStr += " AND DTA_CHIUSURA>=(SELECT getdate()) AND DTA_CHIUSURA<=(SELECT getdate()) ";
                            break;
                        case "STATO":
                            queryStr += " AND (CHA_STATO='" + f.valore.ToUpper() + "' )";
                            break;
                        case "TITOLO":
                            if (Cfg_USE_TEXT_INDEX.Equals("0"))
                            {
                                //whereStr += " AND  (UPPER(A.DESCRIPTION) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%' )";
                                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("&&");
                                string[] lista = regex.Split(f.valore);
                                queryStr += "AND UPPER(A.DESCRIPTION) LIKE '%" + lista[0].ToUpper().Replace("'", "''") + "%'";
                                for (int k = 1; k < lista.Length; k++)
                                    queryStr += " AND UPPER(A.DESCRIPTION) LIKE '%" + lista[k].ToUpper().Replace("'", "''") + "%'";
                            }
                            else
                            {
                                if (Cfg_USE_TEXT_INDEX.Equals("1"))
                                {
                                    //string searchMittDest = " AND g.SYSTEM_id in ( \n " +
                                    //                        " select system_id from table(fulltext_onvar_desc_corr ( \n " +
                                    //                        "'" + DocsPaUtils.Functions.Functions.ReplaceApexes(f.valore).ToUpper() + "'))) \n";
                                    //queryStr += " A.SYSTEM_ID IN (SELECT F.ID_PROFILE FROM DPA_DOC_ARRIVO_PAR F ,DPA_CORR_GLOBALI G  where F.ID_PROFILE=a.system_id and  G.SYSTEM_ID=F.ID_MITT_DEST " + searchMittDest + ") ";
                                }
                                else
                                {
                                    if (Cfg_USE_TEXT_INDEX.Equals("2"))
                                    {
                                        string value = DocsPaUtils.Functions.Functions.ReplaceApexes(f.valore).ToUpper();
                                        string valueA = value;
                                        if (valueA.Contains("&&"))
                                            valueA = valueA.Replace("&&", "");
                                        bool casoA = false;
                                        if (value.Substring(0, value.Length - 1).Contains("%") && !value.Substring(0, value.Length - 1).Contains("%&&"))
                                            casoA = true;
                                        if (value.Contains("&&"))
                                        {
                                            string result = string.Empty;
                                            foreach (string filter in new Regex("&&").Split(value))
                                                if (!string.IsNullOrEmpty(filter))
                                                    result += filter + " AND ";
                                            value = result.Substring(0, result.Length - 5);
                                        }
                                        if (value.Contains("%") && value.IndexOf("%") != value.Length - 1)
                                        {
                                            bool finale = value.EndsWith("%");
                                            string result = string.Empty;
                                            foreach (string filter in new Regex("%").Split(value))
                                                if (!string.IsNullOrEmpty(filter))
                                                    result += filter + "% AND ";
                                            value = result.Substring(0, result.Length - 6);
                                            if (finale)
                                                value = value + "%";
                                        }
                                        if (value.ToUpper().Contains(" AND  AND "))
                                            value = value.ToUpper().Replace(" AND  AND ", " AND ");

                                        queryStr += " AND " + DocsPaDbManagement.Functions.Functions.GetContainsTextQuery("A.DESCRIPTION", value) + " ";
                                        if (casoA)
                                            queryStr += " and upper(A.DESCRIPTION) like upper('%" + valueA + "%')";
                                    }
                                }
                            }

                            break;
                        case "TIPO_FASCICOLO":
                            queryStr += " AND  (A.CHA_TIPO_FASCICOLO = '" + f.valore.ToUpper() + "' ) ";
                            break;
                        // Maurizio Tammacco aggiunti filtri per anno fascicolo e numero fascicolo
                        case "ANNO_FASCICOLO":
                            queryStr += " AND (ANNO_CREAZIONE = " + f.valore + " )";
                            break;
                        case "NUMERO_FASCICOLO":
                            queryStr += " AND (NUM_FASCICOLO = " + f.valore + " )";
                            break;
                        // Federica Franci aggiunti filtri per data di creazione del fascicolo
                        case "CREAZIONE_IL":
                            queryStr += " AND DTA_CREAZIONE>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            queryStr += " AND DTA_CREAZIONE < " + DocsPaDbManagement.Functions.Functions.ToDate(f.valore + " 23:59:59");
                            break;
                        case "CREAZIONE_SUCCESSIVA_AL":
                            //queryStr += " AND DTA_CREAZIONE>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            queryStr += " AND DTA_CREAZIONE>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "CREAZIONE_PRECEDENTE_IL":
                            //queryStr += " AND DTA_CREAZIONE<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            queryStr += "AND DTA_CREAZIONE<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "CREAZIONE_SC":
                            // data creazione nella settimana corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_CREAZIONE>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND DTA_CREAZIONE<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryStr += " AND DTA_CREAZIONE>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND DTA_CREAZIONE<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "CREAZIONE_MC":
                            // data creazione nel mese corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_CREAZIONE>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND DTA_CREAZIONE<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryStr += " AND DTA_CREAZIONE>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND DTA_CREAZIONE<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "CREAZIONE_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND to_char(DTA_CREAZIONE, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryStr += " AND DTA_CREAZIONE>=(SELECT getdate()) AND DTA_CREAZIONE<=(SELECT getdate()) ";
                            break;
                        case "CREAZIONE_IERI":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND to_date(to_char(DTA_CREAZIONE,'dd/mm/yyyy'),'dd/mm/yyyy') = trunc(sysdate -1 ,'DD') ";
                            else
                                queryStr += " AND DATEDIFF(DD, DTA_CREAZIONE, GETDATE() -1) = 0 ";
                            break;
                        case "CREAZIONE_ULTIMI_SETTE_GIORNI":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_CREAZIONE>=(select to_date(to_char(sysdate - 7)) from dual) ";
                            else
                                queryStr += " AND DTA_CREAZIONE>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) ";
                            break;
                        case "CREAZIONE_ULTMI_TRENTUNO_GIORNI":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_CREAZIONE>=(select to_date(to_char(sysdate - 31)) from dual) ";
                            else
                                queryStr += " AND DTA_CREAZIONE>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) ";
                            break;
                        //Locazione Fisica
                        case "ID_UO_LF":
                            queryStr += " AND ID_UO_LF = " + f.valore;
                            break;
                        case "DATA_LF_IL":
                            queryStr += " AND DTA_UO_LF = " + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            break;
                        case "DATA_LF_PRECEDENTE_IL":
                            queryStr += " AND DTA_UO_LF <= " + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            break;
                        case "DATA_LF_SUCCESSIVA_AL":
                            queryStr += " AND DTA_UO_LF >= " + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            break;
                        case "DATA_LF_SC":
                            // data lf nella settimana corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_UO_LF>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND DTA_UO_LF<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryStr += " AND DTA_UO_LF>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND DTA_UO_LF<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "DATA_LF_MC":
                            // data lf nel mese corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_UO_LF>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND DTA_UO_LF<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryStr += " AND DTA_UO_LF>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND DTA_UO_LF<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "DATA_LF_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND to_char(DTA_UO_LF, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryStr += " AND DTA_UO_LF>=(SELECT getdate()) AND DTA_UO_LF<=(SELECT getdate()) ";
                            break;
                        case "ID_UO_REF":
                            queryStr += " AND ID_UO_REF = " + f.valore;
                            break;
                        case "VAR_NOTE":
                            System.Text.RegularExpressions.Regex regexNote = new System.Text.RegularExpressions.Regex("&&");

                            string[] listaNote = regexNote.Split(f.valore);

                            string userDb = getUserDB();
                            if (!string.IsNullOrEmpty(userDb))
                                userDb += ".";

                            foreach (string item in listaNote)
                            {
                                string rf = "";
                                string[] ricNote = SplittaStringaRicercaNote(item);
                                if (ricNote.Length > 2 && !string.IsNullOrEmpty(ricNote[2]))
                                    rf = ricNote[2];
                                else
                                    rf = "''";
                                queryStr += " AND " + string.Format("{0}GetCountNote('F', A.SYSTEM_ID, '{1}', {2}, {3}, '{4}', {5}) > 0", userDb, ricNote[0].Replace("'", "''"), idPeople, idGruppo, ricNote[1], rf);
                            }
                            break;

                        case "TIPOLOGIA_FASCICOLO":
                            DocsPaDB.Query_DocsPAWS.ModelFasc mdFasc = new ModelFasc();
                            DocsPaVO.ProfilazioneDinamica.Templates profilo = mdFasc.getTemplateFascById(f.valore);

                            #region VECCHIO CODICE NON RIMUOVERE PER IL MOMENTO
                            //if(profilo.IPERFASCICOLO != "1")
                            //    queryStr += " AND A.SYSTEM_ID = DPA_ASS_TEMPLATES_FASC.ID_PROJECT AND DPA_ASS_TEMPLATES_FASC.ID_TEMPLATE = " + f.valore;
                            //else
                            //    queryStr += " AND A.SYSTEM_ID = DPA_ASS_TEMPLATES_FASC.ID_PROJECT ";
                            #endregion

                            if (profilo != null && profilo.IPER_FASC_DOC != "1")
                            {
                                queryStr += " AND a.id_tipo_fasc = " + profilo.SYSTEM_ID.ToString();
                            }
                            tipo_contatore = this.tipoContatoreTemplates(f.valore);
                            break;
                        case "PROFILAZIONE_DINAMICA":
                            DocsPaDB.Query_DocsPAWS.ModelFasc modelFasc = new ModelFasc();
                            queryStr += modelFasc.getSeriePerRicercaProfilazione(f.template, "");
                            if (f.template != null && !string.IsNullOrEmpty(f.template.SYSTEM_ID.ToString()))
                                tipo_contatore = this.tipoContatoreTemplates(f.template.SYSTEM_ID.ToString());
                            break;
                        //Laura 9 Aprile
                        case "DIAGRAMMA_STATO_FASC":
                            if ((f.nomeCampo != null && f.nomeCampo.ToUpper() == "UNEQUALS"))
                                queryStr += "  AND A.SYSTEM_ID IN (SELECT ID_PROJECT FROM DPA_DIAGRAMMI WHERE DPA_DIAGRAMMI.ID_STATO != " + f.valore + ") ";
                            else
                            {
                                queryStr += "  AND A.SYSTEM_ID IN (SELECT ID_PROJECT FROM DPA_DIAGRAMMI WHERE DPA_DIAGRAMMI.ID_STATO = " + f.valore + ") ";
                            }
                            break;

                        case "ID_TITOLARIO":
                            queryStr += " AND A.ID_TITOLARIO IN (" + f.valore + ") ";
                            break;
                        case "ID_REGISTRO":
                            queryStr += " AND (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO = " + f.valore + ") ";
                            break;

                        case "DOC_IN_FASC_ADL":
                            //split dei valori
                            string[] val = f.valore.Split('@');
                            queryStr += " and exists (select id_project from dpa_area_lavoro d where d.id_project=a.system_id and id_people=" + val[0] + " and id_ruolo_in_uo =" + val[1] + ")";
                            break;

                        case "SOTTOFASCICOLO":
                            queryStr += " AND A.SYSTEM_ID in (select id_fascicolo from project where CHA_TIPO_PROJ='C' AND UPPER(description) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%' and id_fascicolo != id_parent)";
                            //queryStr += " AND A.SYSTEM_ID in (select id_fascicolo from project where CHA_TIPO_PROJ='C' AND UPPER(description) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%' )";
                            break;
                        case "SCADENZA_IL":
                            queryStr += " AND DTA_SCADENZA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            queryStr += " AND DTA_SCADENZA < " + DocsPaDbManagement.Functions.Functions.ToDate(f.valore + " 23:59:59");

                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "SCADENZA_SUCCESSIVA_AL":
                            queryStr += " AND DTA_SCADENZA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);

                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "SCADENZA_PRECEDENTE_IL":
                            queryStr += " AND DTA_SCADENZA<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);

                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "SCADENZA_SC":
                            // data scadenza nella settimana corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_SCADENZA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND DTA_SCADENZA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryStr += " AND DTA_SCADENZA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND DTA_SCADENZA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "SCADENZA_MC":
                            // data scadenza nel mese corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_SCADENZA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND DTA_SCADENZA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryStr += " AND DTA_SCADENZA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND DTA_SCADENZA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "SCADENZA_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND to_char(DTA_SCADENZA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryStr += " AND DTA_SCADENZA>=(SELECT getdate()) AND DTA_SCADENZA<=(SELECT getdate()) ";
                            break;
                        //Gabriele Melini 30-8-2013
                        //il campo cha_stato della tabella dpa_items_conservazione non è mai impostato a C
                        //seleziono i documenti appartenenti ad almeno un'istanza chiusa (cha_stato='C' in dpa_area_conservazione)

                        case "CONSERVAZIONE":
                            if (f.valore.Equals("1"))
                                //queryStr += " AND A.SYSTEM_ID IN (SELECT ID_PROJECT FROM DPA_ITEMS_CONSERVAZIONE F, PROJECT G WHERE G.SYSTEM_ID = F.ID_PROJECT AND F.CHA_STATO = 'C')";
                                queryStr += " AND A.SYSTEM_ID IN (SELECT ID_PROJECT FROM DPA_ITEMS_CONSERVAZIONE F, PROJECT G WHERE G.SYSTEM_ID = F.ID_PROJECT AND F.id_conservazione IN (SELECT system_id from dpa_area_conservazione WHERE cha_stato='C' OR cha_stato='V'))";
                            //if (f.valore.Equals("0"))
                            //    queryStr += " AND A.SYSTEM_ID NOT IN (SELECT ID_PROJECT FROM DPA_ITEMS_CONSERVAZIONE F, PROJECT G WHERE NOT F.ID_PROJECT IS NULL) OR (A.SYSTEM_ID = D.ID_PROJECT AND D.CHA_STATO <> 'C'))";
                            break;
                        case "ID_PEOPLE_CREATORE":
                            if (f.valore != null && !f.valore.Equals("") && !f.valore.Equals("0"))
                            {
                                userDb = getUserDB();
                                if (!string.IsNullOrEmpty(userDb))
                                    userDb += ".";
                                queryStr += " AND " + userDb + "checkSecurityProprietario(A.SYSTEM_ID, " + f.valore + ", 0) = 1";
                            }
                            break;

                        //ABBATANGELI GIANLUIGI - filtro per applicazione
                        case "COD_EXT_APP":
                            if (f.valore != null && !f.valore.Equals(""))
                                queryStr += " AND COD_EXT_APP = '" + f.valore + "'";
                            break;

                        case "DESC_PEOPLE_CREATORE":
                            if (f.valore != null && !f.valore.Equals(""))
                                queryStr += " AND AUTHOR IN (SELECT SYSTEM_ID FROM PEOPLE WHERE UPPER(FULL_NAME) LIKE '%" + f.valore.ToUpper() + "%')";
                            break;
                        case "ID_RUOLO_CREATORE":
                            if (f.valore != null && !f.valore.Equals("") && !f.valore.Equals("0"))
                            {
                                userDb = getUserDB();
                                if (!string.IsNullOrEmpty(userDb))
                                    userDb += ".";
                                queryStr += " AND " + userDb + "checkSecurityProprietario(A.SYSTEM_ID, 0," + f.valore + ") = 1";
                            }
                            break;
                        case "DESC_RUOLO_CREATORE":
                            if (f.valore != null && !f.valore.Equals(""))
                                queryStr += " AND ID_RUOLO_CREATORE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper() + "%')";
                            break;
                        case "ID_UO_CREATORE":
                            if (f.valore != null && !f.valore.Equals("") && !f.valore.Equals("0"))
                            {
                                userDb = getUserDB();
                                if (!string.IsNullOrEmpty(userDb))
                                    userDb += ".";
                                //queryStr += " AND " + userDb + "checkSecurityUO(A.SYSTEM_ID, " + f.valore + ") = 1";
                                // verifico che sia stato settato il check relativo alle UO sottoposte
                                if (UOsottoposte)
                                    if (!dbType.ToUpper().Equals("SQL"))
                                        queryStr += " AND ID_UO_CREATORE IN (select system_id from dpa_corr_globali where cha_tipo_urp='U' start with system_id = " + f.valore + " connect by prior system_id = id_parent)";
                                    else
                                    {
                                        queryStr += " AND ID_UO_CREATORE IN ( SELECT system_id FROM gerarchia )";
                                        withClause = "WITH gerarchia(system_id) AS (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE CHA_TIPO_URP = 'U' AND SYSTEM_ID = " + f.valore + " UNION ALL SELECT ricorsivo.SYSTEM_ID  FROM DPA_CORR_GLOBALI as ricorsivo, gerarchia WHERE gerarchia.SYSTEM_ID = ricorsivo.id_parent AND ricorsivo.CHA_TIPO_URP = 'U')";
                                    }
                                else
                                    queryStr += " AND ID_UO_CREATORE = " + f.valore;
                            }
                            break;
                        case "DESC_UO_CREATORE":
                            if (f.valore != null && !f.valore.Equals(""))
                                queryStr += " AND ID_UO_CREATORE IN (SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE UPPER(VAR_DESC_CORR) LIKE '%" + f.valore.ToUpper() + "%')";
                            break;
                        case "CODICE_FASCICOLO":
                            if (!string.IsNullOrEmpty(f.valore))
                                queryStr += " AND UPPER(A.VAR_CODICE) = UPPER('" + f.valore + "')";
                            break;
                        case "VISIBILITA_T_A":
                            if (f.valore.ToUpper().Equals("T"))
                                queryStr += " AND CHA_COD_T_A = 'T' ";
                            if (f.valore.ToUpper().Equals("A"))
                                queryStr += " AND CHA_COD_T_A != 'T' ";
                            break;

                        case "ID_AUTHOR":
                            FiltroRicerca corrTypeAuthorId = objListaFiltri.Where(e => e.argomento == CommonSearchFilters.CORR_TYPE_AUTHOR.ToString()).FirstOrDefault();
                            StringBuilder filterConditionAuthorId = new StringBuilder(" AND ");
                            // Se esiste il filtro CORR_TYPE_AUTHOR, viene costruito il filtro per system_id del creatore
                            if (corrTypeAuthorId != null)
                            {
                                switch (corrTypeAuthorId.valore)
                                {
                                    case "R":
                                        FiltroRicerca searchHistoricized = objListaFiltri.Where(e => e.argomento == CommonSearchFilters.EXTEND_TO_HISTORICIZED_AUTHOR.ToString()).FirstOrDefault();
                                        if (searchHistoricized != null && Convert.ToBoolean(searchHistoricized.valore))
                                        {

                                            Query q = null;
                                            Query sqlDb = null;

                                            if (dbType == "SQL")
                                            {
                                                q = InitQuery.getInstance().getQuery("S_GET_ROLE_CHAIN_ID_CORR_GLOBALI_IN_CLAUSOLE");

                                                sqlDb = InitQuery.getInstance().getQuery("S_GET_ROLE_CHAIN_ID_CORR_GLOBALI_OUT_CLAUSOLE");
                                                sqlDb.setParam("idCorrGlob", f.valore);
                                                chaiTableDef = sqlDb.getSQL();

                                            }
                                            else
                                            {
                                                q = InitQuery.getInstance().getQuery("S_GET_ROLE_CHAIN_ID_CORR_GLOBALI");
                                                q.setParam("idCorrGlob", f.valore);
                                            }

                                            filterConditionAuthorId.AppendFormat("id_ruolo_creatore IN ({0})", q.getSQL());
                                        }
                                        else
                                            filterConditionAuthorId.AppendFormat("id_ruolo_creatore = {0} ", f.valore);
                                        break;
                                    case "P":
                                        filterConditionAuthorId.AppendFormat("author IN (SELECT id_people FROM dpa_corr_globali WHERE system_id = {0}) ", f.valore);
                                        break;
                                    case "U":
                                        filterConditionAuthorId.AppendFormat("id_uo_creatore = {0} ", f.valore);
                                        break;
                                }

                            }

                            queryStr += filterConditionAuthorId.ToString();
                            break;

                        case "DESC_AUTHOR":
                            FiltroRicerca corrTypeAuthorDescr = objListaFiltri.Where(e => e.argomento == CommonSearchFilters.CORR_TYPE_AUTHOR.ToString()).FirstOrDefault();
                            StringBuilder filterConditionAuthorDescr = new StringBuilder(" AND EXISTS(SELECT 'x' FROM dpa_corr_globali cg WHERE ");
                            // Se esiste il filtro CORR_TYPE_AUTHOR, viene costruito il filtro per system_id del creatore
                            if (corrTypeAuthorDescr != null)
                            {
                                switch (corrTypeAuthorDescr.valore)
                                {
                                    case "R":
                                        // Se biosgna estenere la ricerca ai ruoli storicizzati, devono essere considerati anche
                                        // quelli con dta_fine valorizzata altrimenti devono essere considerati solo quelli con
                                        // dta_fine non impostata
                                        FiltroRicerca searchHistoricized = objListaFiltri.Where(e => e.argomento == CommonSearchFilters.EXTEND_TO_HISTORICIZED_AUTHOR.ToString()).FirstOrDefault();
                                        if (searchHistoricized != null && !Convert.ToBoolean(searchHistoricized.valore))
                                            filterConditionAuthorDescr.Append("cg.dta_fine IS NULL AND ");

                                        filterConditionAuthorDescr.Append("cg.system_id = id_ruolo_creatore");
                                        break;
                                    case "P":
                                        filterConditionAuthorDescr.Append("cg.id_people = author");
                                        break;
                                    case "U":
                                        filterConditionAuthorDescr.Append("cg.system_id = id_uo_creatore");
                                        break;
                                }

                                //filterConditionAuthorDescr.AppendFormat(" AND UPPER(var_desc_corr) LIKE UPPER('%{0}%'))", f.valore);
                                if (Cfg_USE_TEXT_INDEX.Equals("0"))
                                    filterConditionAuthorDescr.AppendFormat(" AND UPPER(var_desc_corr) LIKE UPPER('%{0}%'))", f.valore);
                                else
                                {
                                    if (Cfg_USE_TEXT_INDEX.Equals("1"))
                                    {
                                        //string searchMittDest = " AND g.SYSTEM_id in ( \n " +
                                        //                        " select system_id from table(fulltext_onvar_desc_corr ( \n " +
                                        //                        "'" + DocsPaUtils.Functions.Functions.ReplaceApexes(f.valore).ToUpper() + "'))) \n";
                                        //queryStr += " A.SYSTEM_ID IN (SELECT F.ID_PROFILE FROM DPA_DOC_ARRIVO_PAR F ,DPA_CORR_GLOBALI G  where F.ID_PROFILE=a.system_id and  G.SYSTEM_ID=F.ID_MITT_DEST " + searchMittDest + ") ";
                                    }
                                    else
                                    {
                                        if (Cfg_USE_TEXT_INDEX.Equals("2"))
                                        {
                                            string value = DocsPaUtils.Functions.Functions.ReplaceApexes(f.valore).ToUpper();
                                            string valueA = value;
                                            if (valueA.Contains("&&"))
                                                valueA = valueA.Replace("&&", "");
                                            bool casoA = false;
                                            if (value.Substring(0, value.Length - 1).Contains("%") && !value.Substring(0, value.Length - 1).Contains("%&&"))
                                                casoA = true;
                                            if (value.Contains("&&"))
                                            {
                                                string result = string.Empty;
                                                foreach (string filter in new Regex("&&").Split(value))
                                                    if (!string.IsNullOrEmpty(filter))
                                                        result += filter + " AND ";
                                                value = result.Substring(0, result.Length - 5);
                                            }
                                            if (value.Contains("%") && value.IndexOf("%") != value.Length - 1)
                                            {
                                                bool finale = value.EndsWith("%");
                                                string result = string.Empty;
                                                foreach (string filter in new Regex("%").Split(value))
                                                    if (!string.IsNullOrEmpty(filter))
                                                        result += filter + "% AND ";
                                                value = result.Substring(0, result.Length - 6);
                                                if (finale)
                                                    value = value + "%";
                                            }
                                            if (value.ToUpper().Contains(" AND  AND "))
                                                value = value.ToUpper().Replace(" AND  AND ", " AND ");

                                            filterConditionAuthorDescr.AppendFormat(" AND {0} ", DocsPaDbManagement.Functions.Functions.GetContainsTextQuery("upper(cg.VAR_DESC_CORR)", value));
                                            if (casoA)
                                                filterConditionAuthorDescr.AppendFormat(" and upper(cg.VAR_DESC_CORR) like upper('%" + valueA + "%')");
                                            filterConditionAuthorDescr.Append(")");
                                        }
                                    }
                                }
                            }

                            queryStr += filterConditionAuthorDescr.ToString();
                            break;
                        case "ID_OWNER":
                            FiltroRicerca corrTypeOwnerId = objListaFiltri.Where(e => e.argomento == CommonSearchFilters.CORR_TYPE_OWNER.ToString()).FirstOrDefault();
                            StringBuilder filterConditionOwnerId = new StringBuilder(" AND EXISTS(SELECT 'x' FROM security sec WHERE a.cha_tipo_fascicolo != 'G' And sec.thing = a.system_id AND sec.cha_tipo_diritto = 'P' AND sec.personorgroup IN (SELECT ");
                            // Se esiste il filtro CORR_TYPE_OWNER, viene costruito il filtro per system_id del proprietario
                            if (corrTypeOwnerId != null)
                            {
                                switch (corrTypeOwnerId.valore)
                                {
                                    case "R":
                                        filterConditionOwnerId.Append("cg.id_gruppo");
                                        filterConditionOwnerId.AppendFormat(" FROM dpa_corr_globali cg WHERE cg.system_id = {0}))", f.valore);
                                        break;
                                    case "P":
                                        filterConditionOwnerId.Append("cg.id_people");
                                        filterConditionOwnerId.AppendFormat(" FROM dpa_corr_globali cg WHERE cg.system_id = {0}))", f.valore);
                                        break;
                                    case "U":
                                        filterConditionOwnerId.Append("cg.id_gruppo");
                                        filterConditionOwnerId.AppendFormat(" FROM dpa_corr_globali cg WHERE cg.ID_UO = {0}))", f.valore);
                                        break;
                                }

                            }

                            
                            queryStr += filterConditionOwnerId.ToString();
                            break;

                        case "DESC_OWNER":
                            FiltroRicerca corrTypeOwnerDescr = objListaFiltri.Where(e => e.argomento == CommonSearchFilters.CORR_TYPE_OWNER.ToString()).FirstOrDefault();
                            StringBuilder filterConditionOwnerDescr = new StringBuilder(" AND EXISTS(SELECT 'x' FROM security sec WHERE sec.thing = a.system_id AND a.cha_tipo_fascicolo != 'G' And sec.cha_tipo_diritto = 'P' AND sec.personorgroup IN (SELECT ");
                            // Se esiste il filtro CORR_TYPE_OWNER, viene costruito il filtro per descrizione del proprietario
                            if (corrTypeOwnerDescr != null)
                            {
                                switch (corrTypeOwnerDescr.valore)
                                {
                                    case "R":
                                        filterConditionOwnerDescr.Append("cg.id_gruppo");
                                        break;
                                    case "P":
                                        filterConditionOwnerDescr.Append("cg.id_people");
                                        break;
                                }

                            }


                            //filterConditionOwnerDescr.AppendFormat(" FROM dpa_corr_globali cg WHERE dta_fine IS NULL AND UPPER(cg.var_desc_corr) LIKE UPPER('%{0}%')))", f.valore);
                            if (Cfg_USE_TEXT_INDEX.Equals("0"))
                                filterConditionOwnerDescr.AppendFormat(" FROM dpa_corr_globali cg WHERE dta_fine IS NULL AND UPPER(cg.var_desc_corr) LIKE UPPER('%{0}%')))", f.valore);
                            else
                            {
                                if (Cfg_USE_TEXT_INDEX.Equals("1"))
                                {
                                    //string searchMittDest = " AND g.SYSTEM_id in ( \n " +
                                    //                                " select system_id from table(fulltext_onvar_desc_corr ( \n " +
                                    //                                "'" + DocsPaUtils.Functions.Functions.ReplaceApexes(f.valore).ToUpper() + "'))) \n";
                                    //queryStr += " A.SYSTEM_ID IN (SELECT F.ID_PROFILE FROM DPA_DOC_ARRIVO_PAR F ,DPA_CORR_GLOBALI G  where F.ID_PROFILE=a.system_id and  G.SYSTEM_ID=F.ID_MITT_DEST " + searchMittDest + ") ";
                                }
                                else
                                {
                                    if (Cfg_USE_TEXT_INDEX.Equals("2"))
                                    {
                                        string value = DocsPaUtils.Functions.Functions.ReplaceApexes(f.valore).ToUpper();
                                        string valueA = value;
                                        if (valueA.Contains("&&"))
                                            valueA = valueA.Replace("&&", "");
                                        bool casoA = false;
                                        if (value.Substring(0, value.Length - 1).Contains("%") && !value.Substring(0, value.Length - 1).Contains("%&&"))
                                            casoA = true;
                                        if (value.Contains("&&"))
                                        {
                                            string result = string.Empty;
                                            foreach (string filter in new Regex("&&").Split(value))
                                                if (!string.IsNullOrEmpty(filter))
                                                    result += filter + " AND ";
                                            value = result.Substring(0, result.Length - 5);
                                        }
                                        if (value.Contains("%") && value.IndexOf("%") != value.Length - 1)
                                        {
                                            bool finale = value.EndsWith("%");
                                            string result = string.Empty;
                                            foreach (string filter in new Regex("%").Split(value))
                                                if (!string.IsNullOrEmpty(filter))
                                                    result += filter + "% AND ";
                                            value = result.Substring(0, result.Length - 6);
                                            if (finale)
                                                value = value + "%";
                                        }
                                        if (value.ToUpper().Contains(" AND  AND "))
                                            value = value.ToUpper().Replace(" AND  AND ", " AND ");
                                        //filterConditionOwnerDescr.AppendFormat(" FROM dpa_corr_globali cg WHERE dta_fine IS NULL AND UPPER(cg.var_desc_corr) LIKE UPPER('%{0}%')))", DocsPaDbManagement.Functions.Functions.GetContainsTextQuery("G.VAR_DESC_CORR", value));
                                        filterConditionOwnerDescr.AppendFormat(" FROM dpa_corr_globali cg WHERE dta_fine IS NULL AND {0} )", DocsPaDbManagement.Functions.Functions.GetContainsTextQuery("upper(cg.VAR_DESC_CORR)", value));
                                        if (casoA)
                                            filterConditionOwnerDescr.AppendFormat(" and upper(cg.var_desc_corr) like upper('%" + valueA + "%')");
                                        filterConditionOwnerDescr.Append(")");
                                    }
                                }
                            }
                            queryStr += filterConditionOwnerDescr.ToString();
                            break;

                        case "IN_CONSERVAZIONE":
                            if (f.valore.Equals("F"))
                            {
                                if (this.dbType == "SQL")
                                {
                                    string dbUser = DocsPaDbManagement.Functions.Functions.GetDbUserSession() + ".";
                                    queryStr += " AND " + dbUser + "getInConservazioneNoSec(null,a.system_id,'F') is not null";
                                }
                                else
                                {
                                    queryStr += " AND getInConservazioneNoSec(null,a.system_id,'F') is not null";
                                }
                            }
                            break;

                        //
                        // MEV CS 1.4 Esibizione
                        // Aggiunto nuovo filtro per ricerca esibizione
                        case "IN_CONSERVAZIONE_ESIB":
                            if (f.valore.Equals("F"))
                            {
                                if (this.dbType == "SQL")
                                {
                                    string dbUser = DocsPaDbManagement.Functions.Functions.GetDbUserSession() + ".";
                                    queryStr += " AND " + dbUser + "getInConsNoSecPerEsib(null,a.system_id,'F') is not null";
                                }
                                else
                                {
                                    queryStr += " AND getInConsNoSecPerEsib(null,a.system_id,'F') is not null";
                                }
                            }
                            break;
                        // End MEV
                        //
                    }
                }
            }
        }

        private static string[] SplittaStringaRicercaNote(string valore)
        {
            // La stringa da ricercare nelle note, oltre al testo da
            // ricercare contiene anche la tipologia di ricerca 
            // richiesta
            string[] separatore = new string[] { "@-@" };
            string[] ricNote = valore.Split(separatore, StringSplitOptions.None);
            return ricNote;
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="idClassificazione"></param>
        /// <param name="codice"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="tipo"></param>
        /// <param name="idRegistro"></param>
        /// <param name="fascicolo"></param>
        /// <param name="gerarchia"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <param name="db"></param>
        public bool newFascicolo(DocsPaVO.utente.InfoUtente infoUtente, string idRegistro,
            DocsPaVO.fascicolazione.Fascicolo fascicolo,
            DocsPaVO.fascicolazione.Classifica[] gerarchia, bool enableUffRef, string chiaveFascicolo)
        {
            bool retValue = false;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT");
                q.setParam("param1", fascicolo.idClassificazione);
                q.setParam("param2", fascicolo.codice);
                q.setParam("param3", infoUtente.idAmministrazione);
                q.setParam("param4", fascicolo.tipo);
                q.setParam("param5", DocsPaDbManagement.Functions.Functions.GetDate());

                if (!string.IsNullOrEmpty(fascicolo.idRegistro))
                    q.setParam("param6", fascicolo.idRegistro);
                else
                    q.setParam("param6", " NULL");

                int numLiv = 1;
                if (gerarchia != null)
                    numLiv += gerarchia.Length;
                string[] cod = fascicolo.codice.Split(DocsPaDB.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getSepFascicolo().ToCharArray());
                string[] liv = cod[0].Split(DocsPaDB.Utils.Personalization.getInstance(infoUtente.idAmministrazione).getSeparator().ToCharArray());

                if (enableUffRef && fascicolo.ufficioReferente != null)
                    q.setParam("param8", " ,ID_UO_REF = " + fascicolo.ufficioReferente.systemId);
                else
                    q.setParam("param8", "");

                if (!string.IsNullOrEmpty(fascicolo.idUoLF))
                    q.setParam("param14", fascicolo.idUoLF);
                else
                    q.setParam("param14", "NULL");

                q.setParam("param15", DocsPaDbManagement.Functions.Functions.ToDate(fascicolo.dtaLF));
                q.setParam("param9", "");

                // Maurizio Tammacco - 10/12/2004 Nuovi campi DTA_CREAZIONE, NUM_FASCICOLO, ANNO_CREAZIONE
                int numFascicolo;
                if (fascicolo.codUltimo == null || fascicolo.codUltimo == "")
                {
                    numFascicolo = 0;
                }
                else
                {
                    try
                    {
                        numFascicolo = Int32.Parse(fascicolo.codUltimo);
                    }
                    catch
                    {
                        numFascicolo = 0;
                    }
                }

                q.setParam("param11", DocsPaDbManagement.Functions.Functions.GetDate());
                q.setParam("param12", numFascicolo.ToString());
                q.setParam("param13", DateTime.Today.Year.ToString());

                //introdotta CONSTRAINTS su VAR_CHIAVE_FASCICOLO
                if (!string.IsNullOrEmpty(chiaveFascicolo))
                    q.setParam("param16", " , VAR_CHIAVE_FASC = " + chiaveFascicolo);

                string controllato = string.Empty;
                if (string.IsNullOrEmpty(fascicolo.controllato))
                    controllato = "0";
                else
                    controllato = fascicolo.controllato;

                q.setParam("controllato", ", cha_controllato = " + controllato);

                if (fascicolo.cartaceo)
                    q.setParam("cartaceo", "1");
                else
                    q.setParam("cartaceo", "0");

                if (fascicolo.privato == "1")
                    q.setParam("privato", "1");
                else
                    q.setParam("privato", "0");

                q.setParam("pubblico", fascicolo.pubblico ? "1" : "0");

                //Tipo fascicolo
                if (fascicolo.template != null)
                    q.setParam("tipoFascicolo", ", ID_TIPO_FASC =" + fascicolo.template.SYSTEM_ID.ToString());
                else
                    q.setParam("tipoFascicolo", "");

                q.setParam("param17", fascicolo.creatoreFascicolo.idPeople.ToString());
                q.setParam("param18", fascicolo.creatoreFascicolo.idCorrGlob_Ruolo);
                q.setParam("param19", fascicolo.creatoreFascicolo.idCorrGlob_UO);

                //ABBATANGELI GIANLUIGI - Parametro che indica su quale applicazione si sta lavorando il fascicolo
                q.setParam("param20", fascicolo.codiceApplicazione);

                q.setParam("param10", " WHERE SYSTEM_ID = " + fascicolo.systemID);

                string commandText = q.getSQL();
                logger.Debug(commandText);

                retValue = dbProvider.ExecuteNonQuery(commandText);

                if (retValue)
                {
                    // Aggiornamento note del fascicolo
                    UpdateNoteFascicolo(infoUtente, fascicolo);
                }
            }

            return retValue;
        }

        public bool aggiornaDpaRegFasc(ref  string codUltimo, string idClassificazione, string idRegistro, DocsPaDB.DBProvider dbProvider)
        {
            bool retValue = false;
            DocsPaUtils.Query q;
            string updateString = "";
            int numFascicolo;
            if (codUltimo != null && codUltimo.Equals(""))
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_REG_FASC");

                q.setParam("param1", idClassificazione);
                if (idRegistro != null && idRegistro != "")
                {
                    q.setParam("param2", " = " + idRegistro);
                }
                else
                {
                    q.setParam("param2", "IS NULL");
                }
                string queryString = q.getSQL();
                logger.Debug(queryString);
                string result;
                dbProvider.ExecuteScalar(out result, queryString);
                numFascicolo = Int32.Parse(result);
                codUltimo = numFascicolo.ToString();
            }

            q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_REG_FASC");
            numFascicolo = Convert.ToInt32(codUltimo);
            numFascicolo = numFascicolo + 1;
            q.setParam("param1", numFascicolo.ToString());
            q.setParam("param2", idClassificazione);

            if (idRegistro != null && idRegistro != "")
            {
                q.setParam("param3", " = " + idRegistro);
            }
            else
            {
                q.setParam("param3", "IS NULL");
            }
            updateString = q.getSQL();

            retValue = dbProvider.ExecuteNonQuery(updateString);

            return retValue;

        }

        /// <summary>
        /// Ritorna il numero fascicolo contenuto nella tabella DPA_REG_FASC relativo al registro corrente
        /// </summary>
        /// <param name="idTitolario">system_id del nodo di titolario</param>
        /// <param name="idRegistro">system_id del registro seleziono dall'utente</param>
        /// <param name="dbProvider"></param>
        /// <returns></returns>
        public bool getFascNumRif(string idTitolario, string idRegistro, out string numFascicolo)
        {
            bool retValue = false;
            numFascicolo = "";
            string selectString = "";
            DBProvider dbProvider = new DBProvider();
            try
            {
                dbProvider.BeginTransaction();
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_REG_FASC");
                q.setParam("param1", idTitolario);

                if (idRegistro != null && idRegistro != "")
                {
                    q.setParam("param2", " = " + idRegistro);
                }
                else
                {
                    q.setParam("param2", "IS NULL");
                }
                selectString = q.getSQL();
                logger.Debug(selectString);
                if (dbProvider.ExecuteScalar(out numFascicolo, selectString))
                {
                    retValue = true;
                    dbProvider.CommitTransaction();
                }
            }
            catch (Exception ex)
            {
                retValue = false;
                dbProvider.RollbackTransaction();
                logger.Debug(ex.Message);
            }
            finally
            {
                dbProvider.CloseConnection();
            }
            return retValue;

        }
        #endregion

        //public void SetDataVista(string idPeople, string idProfile)
        //{
        //    string queryTemp = "CHA_VISTA='0' AND ID_PEOPLE=" + idPeople + " AND ID_TRASM_SINGOLA IN (SELECT A.SYSTEM_ID FROM DPA_TRASM_SINGOLA A, DPA_TRASMISSIONE B WHERE A.ID_TRASMISSIONE=B.SYSTEM_ID AND B.ID_PROJECT=" + 
        //        idProfile + ")";
        //    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPATrasmUtente");
        //    q.setParam("param1","CHA_VISTA = '1', DTA_VISTA=" + DocsPaDbManagement.Functions.Functions.GetDate());
        //    q.setParam("param2", queryTemp);
        //    string queryString = q.getSQL();
        //    this.ExecuteNonQuery(queryString);
        //}

        public void SetDataVista(string idPeople, string idProject)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPATrasmUtente_setDataVista");

            q.setParam("dataOdierna", DocsPaDbManagement.Functions.Functions.GetDate());
            q.setParam("tipoObj", "ID_PROJECT");
            q.setParam("idObj", idProject);
            q.setParam("idPeople", idPeople);

            string queryString = q.getSQL();
            logger.Debug(queryString);

            this.ExecuteNonQuery(queryString);
        }

        /// <summary>
        /// Setta la data di vista per le trasmissioni nel caso di fascicoli
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <param name="idOggetto"></param>
        /// <param name="tipoOggetto"></param>
        /// <returns></returns>
        public bool SetDataVistaSP(DocsPaVO.utente.InfoUtente infoUtente, string idOggetto, string tipoOggetto)
        {
            bool retValue = false;
            //modifica
            int delegato = 0;
            //fine modifica
            try
            {
                this.BeginTransaction();

                logger.Debug("INIZIO SetDataVistaSP per i fascicoli");

                // Creazione parametri per la Store Procedure
                ArrayList parameters = new ArrayList();
                DocsPaUtils.Data.ParameterSP outParam;
                outParam = new DocsPaUtils.Data.ParameterSP("resultValue", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);

                parameters.Add(this.CreateParameter("idPeople", infoUtente.idPeople));
                parameters.Add(this.CreateParameter("idOggetto", idOggetto));
                parameters.Add(this.CreateParameter("tipoOggetto", tipoOggetto));
                parameters.Add(this.CreateParameter("idGruppo", infoUtente.idGruppo));

                if (infoUtente.delegato != null)
                    delegato = Convert.ToInt32(infoUtente.delegato.idPeople);
                parameters.Add(this.CreateParameter("idDelegato", delegato));

                parameters.Add(outParam);

                if (Cfg_SET_DATA_VISTA_GRD == "2")
                    this.ExecuteStoredProcedure("SPsetDataVista_V2", parameters, null);
                else
                    this.ExecuteStoredProcedure("SPsetDataVista", parameters, null);

                if (outParam.Valore != null && outParam.Valore.ToString() != "" && outParam.Valore.ToString() != "1")
                {
                    retValue = true;
                    logger.Debug("STORE PROCEDURE SetDataVistaSP per i fascicoli: esito positivo");
                }
                if (retValue)
                    this.CommitTransaction();
                else
                    this.RollbackTransaction();
            }
            catch (Exception e)
            {
                logger.Debug("STORE PROCEDURE SetDataVistaSP per i fascicoli: esito negativo" + e.Message);
                this.RollbackTransaction();
                retValue = false;
            }
            finally
            {
                this.CloseConnection();
                logger.Debug("FINE SetDataVistaSP per i fascicoli");
            }

            return retValue;
        }

        public bool SetDataVistaSP_TASTOVISTO(DocsPaVO.utente.InfoUtente infoUtente, string idOggetto, string tipoOggetto)
        {
            bool retValue = false;
            int delegato = 0;
            try
            {
                this.BeginTransaction();

                logger.Debug("INIZIO SetDataVistaSP_TV");

                // Creazione parametri per la Store Procedure
                ArrayList parameters = new ArrayList();
                DocsPaUtils.Data.ParameterSP outParam;
                outParam = new DocsPaUtils.Data.ParameterSP("resultValue", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);

                parameters.Add(this.CreateParameter("idPeople", infoUtente.idPeople));
                parameters.Add(this.CreateParameter("idOggetto", idOggetto));
                parameters.Add(this.CreateParameter("tipoOggetto", tipoOggetto));
                parameters.Add(this.CreateParameter("idGruppo", infoUtente.idGruppo));
                if (infoUtente.delegato != null)
                    delegato = Convert.ToInt32(infoUtente.delegato.idPeople);
                parameters.Add(this.CreateParameter("idDelegato", delegato));


                parameters.Add(outParam);

                this.ExecuteStoredProcedure("SPsetDataVista_TV", parameters, null);

                if (outParam.Valore != null && outParam.Valore.ToString() != "" && outParam.Valore.ToString() != "1")
                {
                    retValue = true;
                    logger.Debug("STORE PROCEDURE SetDataVistaSP_TV: esito positivo");
                }


                //logger.Debug("INIZIO SP_DELETE_TODOLIST");
                //// Creazione parametri per la Store Procedure
                //parameters = new ArrayList();
                //outParam = new DocsPaUtils.Data.ParameterSP("resultValue", new Int32(), 10, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);

                //parameters.Add(this.CreateParameter("idPeople", idPeople));
                //parameters.Add(this.CreateParameter("idOggetto", idOggetto));
                //parameters.Add(this.CreateParameter("tipoOggetto", tipoOggetto));
                //parameters.Add(this.CreateParameter("sysTrasmSingola", ));
                //parameters.Add(outParam);

                //this.ExecuteStoredProcedure("SP_DELETE_TODOLIST", parameters, null);

                //if (outParam.Valore != null && outParam.Valore.ToString() != "" && outParam.Valore.ToString() != "1")
                //{
                //    retValue = true;
                //    logger.Debug("STORE PROCEDURE SP_DELETE_TODOLIST: esito positivo");
                //}


                if (retValue)
                    this.CommitTransaction();
                else
                    this.RollbackTransaction();
            }
            catch (Exception e)
            {
                logger.Debug("STORE PROCEDURE SetDataVistaSP: esito negativo" + e.Message);
                this.RollbackTransaction();
                retValue = false;
            }
            finally
            {
                this.CloseConnection();
                logger.Debug("FINE SetDataVistaSP");
            }

            return retValue;
        }

        public static string Cfg_SET_DATA_VISTA_GRD
        {
            get
            {
                string eme = ConfigurationManager.AppSettings["SET_DATA_VISTA_GRD"];
                return (eme != null) ? eme : "0";
            }
        }

        #region Folder Manager
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="infoUtente"></param>
        public void NewFolder(string idAmministrazione, DocsPaVO.fascicolazione.Folder folder)
        {
            //Veronica
            //controllo se il folderParent è privato, in tal caso anche questo folder viene creato come privato
            string privato = null;
            string pubblico = null;
            DataSet ds = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");
                        q.setParam("param1", "CHA_PRIVATO, CHA_PUBBLICO");
            q.setParam("param2", "where SYSTEM_ID = " + folder.idFascicolo);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteQuery(out ds, "project", queryString);

            if (ds.Tables["project"] != null && ds.Tables["project"].Rows.Count > 0)
            {
                DataRow row = ds.Tables["project"].Rows[0];
                privato = !string.IsNullOrEmpty(row["CHA_PRIVATO"].ToString()) ? row["CHA_PRIVATO"].ToString() : "0";
                pubblico = !string.IsNullOrEmpty(row["CHA_PUBBLICO"].ToString()) ? row["CHA_PUBBLICO"].ToString() : "0";
            }

            DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_2");
            q1.setParam("param1", folder.idParent);
            q1.setParam("param2", idAmministrazione);
            q1.setParam("param3", folder.idFascicolo);
            q1.setParam("param4", DocsPaDbManagement.Functions.Functions.GetDate());
            q1.setParam("param5", folder.systemID);
            q1.setParam("param6", privato);
            q1.setParam("pubblico", pubblico);
            string updateString = q1.getSQL();
            logger.Debug(updateString);
            this.ExecuteNonQuery(updateString);
        }

        private DocsPaUtils.Data.ParameterSP CreateParameter(string name, object value)
        {
            return new DocsPaUtils.Data.ParameterSP(name, value);
        }

        public bool NewFolder(string idAmministrazione, DocsPaVO.fascicolazione.Folder folder, DocsPaDB.DBProvider dbProvider)
        {
            bool result = false;
            int rtn;

            //Veronica
            //controllo se il folderParent è privato, in tal caso anche questo folder viene creato come privato
            string privato = null;
            string pubblico = null;
            DataSet ds = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");
            q.setParam("param1", "CHA_PRIVATO, CHA_PUBBLICO");
            q.setParam("param2", "where SYSTEM_ID = " + folder.idFascicolo);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteQuery(out ds, "project", queryString);

            if (ds.Tables["project"] != null && ds.Tables["project"].Rows.Count > 0)
            {
                DataRow row = ds.Tables["project"].Rows[0];
                privato = !string.IsNullOrEmpty(row["CHA_PRIVATO"].ToString()) ? row["CHA_PRIVATO"].ToString() : "0";
                pubblico = !string.IsNullOrEmpty(row["CHA_PUBBLICO"].ToString()) ? row["CHA_PUBBLICO"].ToString() : "0";
            }

            DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_2");
            q1.setParam("param1", folder.idParent);
            q1.setParam("param2", idAmministrazione);
            q1.setParam("param3", folder.idFascicolo);
            q1.setParam("param4", DocsPaDbManagement.Functions.Functions.GetDate());
            q1.setParam("param5", folder.systemID);
            q1.setParam("param6", privato);
            q1.setParam("pubblico", pubblico);

            string updateString = q1.getSQL();
            logger.Debug("Update dati Folder: metodo NewFolder" + updateString);
            result = dbProvider.ExecuteNonQuery(updateString, out rtn);
            logger.Debug("NewFolder...aggiornamento dati della folder - numero record aggiornati: " + rtn);
            if (rtn > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool updateIdTitolario(string idTitolario, string idFascicolo, string idFolder, DocsPaDB.DBProvider dbProvider)
        {
            bool result = false;
            int rtn;

            DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_3");
            q1.setParam("idTitolario", idTitolario);
            q1.setParam("idFascicolo", idFascicolo);
            q1.setParam("idFolder", idFolder);

            string updateString = q1.getSQL();
            logger.Debug("Update ID_TITOLARIO Fascicolo e Folder: metodo updateIdTitolario" + updateString);
            result = dbProvider.ExecuteNonQuery(updateString, out rtn);

            if (rtn > 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemID"></param>
        /// <returns></returns>
        public int GetFolderCount(string systemID)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project8");

            q.setParam("param1", systemID);
            string queryString = q.getSQL();


            string numFigli;
            this.ExecuteScalar(out numFigli, queryString);


            return Int32.Parse(numFigli);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemIdDocument"></param>
        /// <returns></returns>
        public System.Collections.ArrayList GetFoldersDocument(string systemIdDocument)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__PROJECT_COMPONENTS__GET_FOLDERS_DOCUMENT");
            q.setParam("param1", systemIdDocument);
            q.setParam("param2", "");
            string sqlText = q.getSQL();

            logger.Debug(sqlText);

            DataSet ds = new DataSet();
            this.ExecuteQuery(out ds, sqlText);

            q = null;

            // Creazione oggetti Folder e caricamento in un ArrayList
            System.Collections.ArrayList retValue = new System.Collections.ArrayList();
            this.FillArrayListFoldersDocument(retValue, ds.Tables[0]);
            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="systemIdDocument"></param>
        /// <returns></returns>
        public System.Collections.ArrayList GetFoldersDocument(string systemIdDocument, string systemIdFascicolo)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__PROJECT_COMPONENTS__GET_FOLDERS_DOCUMENT");
            q.setParam("param1", systemIdDocument);
            q.setParam("param2", " AND P.ID_FASCICOLO=" + systemIdFascicolo + " ");
            string sqlText = q.getSQL();

            logger.Debug(sqlText);

            DataSet ds = new DataSet();
            this.ExecuteQuery(out ds, sqlText);

            q = null;

            // Creazione oggetti Folder e caricamento in un ArrayList
            System.Collections.ArrayList retValue = new System.Collections.ArrayList();
            this.FillArrayListFoldersDocument(retValue, ds.Tables[0]);
            return retValue;
        }

        private void FillArrayListFoldersDocument(System.Collections.ArrayList arrayList, DataTable dtFoldersDocument)
        {
            foreach (DataRow row in dtFoldersDocument.Rows)
            {
                // Creazione oggetto fascicolo
                arrayList.Add(new Folder()
                {
                    systemID = row["ID_FOLDER"].ToString(),
                    idParent = row["ID_PARENT_FOLDER"].ToString(),
                    idFascicolo = row["ID_FASCICOLO"].ToString(),
                    descrizione = row["FOLDER_DESCRIPTION"].ToString(),
                    dtaApertura = row["APERTURA"].ToString().Trim(),
                    livello = (!dtFoldersDocument.Columns.Contains("NUM_LIVELLO") || row.IsNull("NUM_LIVELLO")) ? "" : row["NUM_LIVELLO"].ToString(),
                    codicelivello = (!dtFoldersDocument.Columns.Contains("VAR_COD_LIV1") || row.IsNull("VAR_COD_LIV1")) ? "" : row["VAR_COD_LIV1"].ToString()
                });
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns>Folder o 'null' se si è verificato un errore.</returns>
        public DocsPaVO.fascicolazione.Folder GetFolderById(string idPeople, string idGruppo, string idFolder)
        {
            //DocsPaWS.Utils.Database database = DocsPaWS.Utils.dbControl.getDatabase();			
            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();

            try
            {

                //database.openConnection();

                System.Data.DataSet dataSet;// = new System.Data.DataSet();

                string query = "";
                string queryName = "S_J_PROJECT_FOLDERByID";

                bool IS_ARCHIVISTA_DEPOSITO;
                DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
                IS_ARCHIVISTA_DEPOSITO = ut.isUtArchivistaDeposito(idPeople, idGruppo);

                if (IS_ARCHIVISTA_DEPOSITO)
                    queryName = "S_J_PROJECT_FOLDERByID_DEPOSITO";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);
                q.setParam("param1", idFolder);
                q.setParam("param2", idPeople);
                q.setParam("param3", idGruppo);

                string idAmm = "0";
                if (!string.IsNullOrEmpty(idPeople))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                    idAmm = u.GetIdAmm(idPeople);
                }
                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
                q.setParam("idRuoloPubblico", !string.IsNullOrEmpty(idRuoloPubblico) ? idRuoloPubblico : "0");

                query = q.getSQL();
                logger.Debug(query);

                //database.fillTable(updateString,dataSet,"FOLDER");
                this.ExecuteQuery(out dataSet, "FOLDER", query);

                if (dataSet.Tables["FOLDER"].Rows.Count > 0)
                {
                    folderObject = GetFolderData(dataSet.Tables["FOLDER"].Rows[0], dataSet.Tables["FOLDER"]);
                }
            }
            catch (Exception)
            {
                //				logger.Debug (e.Message);				


                //database.closeConnection();
                //NB: L'istruzione seguente è solo per evitare il warning
                //	in attesa di un futuro utilizzo del messaggio della variabile e.
                //string ErrMsg = e.Message;
                //throw new Exception("F_System");
                logger.Debug("F_System");

                folderObject = null;
            }

            return folderObject;
        }

        public ArrayList GetFolderByDescr(string idPeople, string idGruppo, string idFasc, string descrFolder)
        {
            ArrayList lstFolder = new ArrayList();

            try
            {
                System.Data.DataSet dataSet;

                string updateString = "";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FOLDER_BY_DESCR");
                q.setParam("param1", idFasc);
                q.setParam("param2", idPeople);
                q.setParam("param3", idGruppo);
                q.setParam("param4", " AND UPPER(P.DESCRIPTION) LIKE '%" + descrFolder.ToUpper() + "%'");
                //if (inizio == 0)
                //   q.setParam("param5", " AND P.ID_PARENT in (select system_id from project where id_parent=" + fascicolo.systemID + ")");
                //else
                //   q.setParam("param5", " AND P.ID_PARENT = " + ((DocsPaVO.fascicolazione.Folder)lstFolder[i - 1]).systemID);

                string idAmm = "0";
                if (!string.IsNullOrEmpty(idPeople))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                    idAmm = u.GetIdAmm(idPeople);
                }

                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
                if (string.IsNullOrEmpty(idRuoloPubblico))
                    idRuoloPubblico = "0";
                q.setParam("idRuoloPubblico", idRuoloPubblico);

                updateString = q.getSQL();
                logger.Debug(updateString);
                //inizio++;
                //database.fillTable(updateString,dataSet,"FOLDER");
                this.ExecuteQuery(out dataSet, "FOLDER", updateString);
                if (dataSet.Tables["FOLDER"].Rows.Count > 0)
                {
                    this.FillArrayListFoldersDocument(lstFolder, dataSet.Tables["FOLDER"]);
                }
                else
                {
                    lstFolder = null;

                }
            }
            catch (Exception)
            {
                logger.Debug("F_System");

                lstFolder = null;
            }

            return lstFolder;
        }

        /// <summary>
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns>Folder o 'null' se si è verificato un errore.</returns>
        public DocsPaVO.fascicolazione.Folder GetFolder(string idPeople, string idGruppo, string idFascicolo)
        {
            //DocsPaWS.Utils.Database database = DocsPaWS.Utils.dbControl.getDatabase();			
            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();

            try
            {

                //database.openConnection();

                System.Data.DataSet dataSet;// = new System.Data.DataSet();

                /*string commandString1=
                    " SELECT DISTINCT A.* FROM PROJECT A, SECURITY B " +
                    " WHERE A.SYSTEM_ID=B.THING AND A.ID_FASCICOLO=" + idFascicolo +
                    " AND (B.PERSONORGROUP=" + infoUtente.idPeople + " OR B.PERSONORGROUP=" + infoUtente.idGruppo + ") AND B.ACCESSRIGHTS > 0";*/

                string updateString = "";


                /*  SAB 16/05/2013 per gestione deposito  modifica momentanea x capire come gestire il checksecurity in tutti i casi */
                string queryName;
                DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
                if (ut.isUtArchivistaDeposito(idPeople, idGruppo))
                    queryName = "S_J_PROJECT__SECURITY5_DEPOSITO";
                else
                    queryName = "S_J_PROJECT__SECURITY5";


                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);
                q.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                q.setParam("param1", idFascicolo);
                q.setParam("param2", idPeople);
                q.setParam("param3", idGruppo);

                string idAmm = "0";
                if (!string.IsNullOrEmpty(idPeople))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                    idAmm = u.GetIdAmm(idPeople);
                }
                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
                if (string.IsNullOrEmpty(idRuoloPubblico))
                    idRuoloPubblico = "0";
                q.setParam("idRuoloPubblico", idRuoloPubblico);


                updateString = q.getSQL();

                string valorechiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_PROJECT_LEVEL");
                if (!string.IsNullOrEmpty(valorechiave) && valorechiave.Equals("1"))
                    updateString = updateString.Replace("ORDER BY DESCRIPTION ASC", "ORDER BY A.SYSTEM_ID ASC");

                logger.Debug(updateString);

                //database.fillTable(updateString,dataSet,"FOLDER");
                this.ExecuteQuery(out dataSet, "FOLDER", updateString);

                System.Data.DataRow[] folderRootRows = dataSet.Tables["FOLDER"].Select("ID_PARENT=" + idFascicolo);
                if (folderRootRows.Length > 0)
                {
                    folderObject = GetFolderData(folderRootRows[0], dataSet.Tables["FOLDER"]);
                }
            }
            catch (Exception)
            {
                //				logger.Debug (e.Message);				


                //database.closeConnection();
                //NB: L'istruzione seguente è solo per evitare il warning
                //	in attesa di un futuro utilizzo del messaggio della variabile e.
                //string ErrMsg = e.Message;
                //throw new Exception("F_System");
                logger.Debug("F_System");

                folderObject = null;
            }

            return folderObject;
        }

        public DocsPaVO.fascicolazione.Folder GetFolderByIdFascicolo(string idPeople, string idGruppo, string idFascicolo)
        {
            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();

            try
            {
                System.Data.DataSet dataSet;// = new System.Data.DataSet();

                string updateString = "";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__SECURITY_BY_ID_FASCICOLO");
                q.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                q.setParam("param1", idFascicolo);
                q.setParam("param2", idPeople);
                q.setParam("param3", idGruppo);

                string idAmm = "0";
                if (!string.IsNullOrEmpty(idPeople))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                    idAmm = u.GetIdAmm(idPeople);
                }
                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
                if (string.IsNullOrEmpty(idRuoloPubblico))
                    idRuoloPubblico = "0";
                q.setParam("idRuoloPubblico", idRuoloPubblico);

                updateString = q.getSQL();
                logger.Debug(updateString);

                //database.fillTable(updateString,dataSet,"FOLDER");
                this.ExecuteQuery(out dataSet, "FOLDER", updateString);

                System.Data.DataRow[] folderRootRows = dataSet.Tables["FOLDER"].Select("ID_PARENT=" + idFascicolo);
                if (folderRootRows.Length > 0)
                {
                    folderObject = GetFolderData(folderRootRows[0], dataSet.Tables["FOLDER"]);
                }
            }
            catch (Exception)
            {
                logger.Debug("F_System");
                folderObject = null;
            }

            return folderObject;
        }

        public DocsPaVO.fascicolazione.Folder GetFolderByIdFascicoloAndIdFolder(string idPeople, string idGruppo, string idFascicolo, string idFolder)
        {
            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();

            try
            {
                System.Data.DataSet dataSet;// = new System.Data.DataSet();

                string updateString = "";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__SECURITY_BY_ID_FASCICOLO");
                q.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                q.setParam("param1", idFascicolo);
                q.setParam("param2", idPeople);
                q.setParam("param3", idGruppo);

                string idAmm = "0";
                if (!string.IsNullOrEmpty(idPeople))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                    idAmm = u.GetIdAmm(idPeople);
                }
                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
                if (string.IsNullOrEmpty(idRuoloPubblico))
                    idRuoloPubblico = "0";
                q.setParam("idRuoloPubblico", idRuoloPubblico);

                updateString = q.getSQL();
                logger.Debug(updateString);

                //database.fillTable(updateString,dataSet,"FOLDER");
                this.ExecuteQuery(out dataSet, "FOLDER", updateString);

                System.Data.DataRow[] folderRootRows = dataSet.Tables["FOLDER"].Select("SYSTEM_ID=" + idFolder);
                if (folderRootRows.Length > 0)
                {
                    folderObject = GetFolderData(folderRootRows[0], dataSet.Tables["FOLDER"]);
                }
            }
            catch (Exception)
            {
                logger.Debug("F_System");
                folderObject = null;
            }

            return folderObject;
        }


        /* ABBATANGELI GIANLUIGI
         * Nuova funzione che mi restituisce una determiunata cartella e predispone il caricamento delle cartelle figlie */
        public DocsPaVO.fascicolazione.Folder GetFolderAndChildByIdFascicoloIdFolder(string idPeople, string idGruppo, string idFascicolo, string idFolder)
        {
            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();

            try
            {
                System.Data.DataSet dataSet;// = new System.Data.DataSet();

                string updateString = "";
                bool IS_ARCHIVISTA_DEPOSITO;
                DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
                IS_ARCHIVISTA_DEPOSITO = ut.isUtArchivistaDeposito(idPeople, idGruppo);
                string query = "S_J_PROJECT__FOLDERB_AND_CHILD";
                if (IS_ARCHIVISTA_DEPOSITO)
                    query = "S_J_PROJECT__FOLDERB_AND_CHILD_DEPOSITO";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery(query);
                q.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                q.setParam("param1", idFascicolo);
                q.setParam("param2", idFolder);
                q.setParam("param3", idPeople);
                q.setParam("param4", idGruppo);

                string idAmm = "0";
                if (!string.IsNullOrEmpty(idPeople))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                    idAmm = u.GetIdAmm(idPeople);
                }
                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
                q.setParam("idRuoloPubblico", !string.IsNullOrEmpty(idRuoloPubblico) ? idRuoloPubblico : "0");

                updateString = q.getSQL();
                logger.Debug(updateString);

                //database.fillTable(updateString,dataSet,"FOLDER");
                this.ExecuteQuery(out dataSet, "FOLDER", updateString);

                DataTable table = dataSet.Tables["FOLDER"];
                System.Data.DataRow[] folderRootRows = table.Select("SYSTEM_ID=" + idFolder);
                if (folderRootRows.Length > 0)
                {
                    folderObject = GetFolderData(folderRootRows[0], dataSet.Tables["FOLDER"]);
                }
            }
            catch (Exception)
            {
                logger.Debug("F_System");
                folderObject = null;
            }

            return folderObject;
        }

        /// <summary>
        /// Ritorna l'oggetto folder compreso il 1 folderChilds
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <param name="idFascicolo"></param>
        /// <param name="idFolder"></param>
        /// <returns></returns>
        public DocsPaVO.fascicolazione.Folder GetFolderPrimoChild(string idPeople, string idGruppo, string idFasc, string idFolder)
        {
            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();

            try
            {
                System.Data.DataSet dataSet;
                string query = "";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__SECURITY5");
                q.setParam("param1", idFasc);
                q.setParam("param2", idPeople);
                q.setParam("param3", idGruppo);

                string idAmm = "0";
                if (!string.IsNullOrEmpty(idPeople))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                    idAmm = u.GetIdAmm(idPeople);
                }
                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
                q.setParam("idRuoloPubblico", !string.IsNullOrEmpty(idRuoloPubblico) ? idRuoloPubblico : "0");

                query = q.getSQL();
                logger.Debug(query);

                this.ExecuteQuery(out dataSet, "FOLDER", query);
                if (idFolder == null || idFolder == string.Empty)
                {
                    System.Data.DataRow[] folderRootRows = dataSet.Tables["FOLDER"].Select("ID_PARENT=" + idFasc);
                    if (folderRootRows.Length > 0)
                    {
                        folderObject = GetFolderDataPrimoChilds(folderRootRows[0], dataSet.Tables["FOLDER"]);
                    }
                }
                else
                {

                    System.Data.DataRow[] folderRootRows = dataSet.Tables["FOLDER"].Select("SYSTEM_ID=" + idFolder);
                    if (folderRootRows.Length > 0)
                    {
                        folderObject = GetFolderDataPrimoChilds(folderRootRows[0], dataSet.Tables["FOLDER"]);
                    }

                }
            }
            catch (Exception)
            {
                logger.Debug("F_System");
                folderObject = null;
            }

            return folderObject;
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="infoUtente"></param>
        public int DelFolder(DocsPaVO.fascicolazione.Folder folder, DocsPaVO.utente.InfoUtente infoUtente)
        {
            int numFigli = 0;

            try
            {
                // verifico se è un root folder
                if (folder.idParent.Equals(folder.idFascicolo))
                {
                    logger.Debug("Errore nella gestione dei Fascicoli (Query - DelFolder). Non si può cancellare un root folder!");
                    throw new Exception("Non si può cancellare un root folder");
                }

                // posso cancellare il folder solo se non ha figli
                /*string queryString =
                    "SELECT COUNT(*) FROM PROJECT WHERE ID_PARENT=" + folder.systemID;
                numFigli = Int32.Parse(db.executeScalar(queryString).ToString());*/
                DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                numFigli = fascicoli.GetFolderCount(folder.systemID);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                numFigli = -1;
            }

            return numFigli;
        }


        internal DocsPaVO.fascicolazione.Folder GetFolderDataPrimoChilds(System.Data.DataRow dataRow, System.Data.DataTable table)
        {

            DocsPaVO.fascicolazione.Folder folder = new DocsPaVO.fascicolazione.Folder();
            folder.descrizione = dataRow["DESCRIPTION"].ToString();
            folder.systemID = dataRow["SYSTEM_ID"].ToString();
            folder.idFascicolo = dataRow["ID_FASCICOLO"].ToString();
            folder.idParent = dataRow["ID_PARENT"].ToString();

            folder.childs = GetFolderPrimoChildren(folder.systemID, table);

            return folder;
        }

        private System.Collections.ArrayList GetFolderPrimoChildren(string parent_id, System.Data.DataTable table)
        {

            System.Data.DataRow[] folderChildrenRows = table.Select("ID_PARENT=" + parent_id);

            System.Collections.ArrayList folderChildren = new System.Collections.ArrayList();
            for (int i = 0; i < folderChildrenRows.Length; i++)
            {
                folderChildren.Add(GetFolderDataPrimoChilds(folderChildrenRows[i], table));
            }

            return folderChildren;
        }



        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        internal DocsPaVO.fascicolazione.Folder GetFolderData(System.Data.DataRow dataRow, System.Data.DataTable table)
        {
            DocsPaVO.fascicolazione.Folder folder = new DocsPaVO.fascicolazione.Folder();
            folder.descrizione = dataRow["DESCRIPTION"].ToString();
            folder.systemID = dataRow["SYSTEM_ID"].ToString();
            folder.idFascicolo = dataRow["ID_FASCICOLO"].ToString();
            folder.idParent = dataRow["ID_PARENT"].ToString();

            if (table.Columns.Contains("NUM_LIVELLO"))
                folder.livello = dataRow.IsNull("NUM_LIVELLO") ? string.Empty : dataRow["NUM_LIVELLO"].ToString();

            if (table.Columns.Contains("VAR_COD_LIV1"))
                folder.codicelivello = dataRow.IsNull("VAR_COD_LIV1") ? string.Empty : dataRow["VAR_COD_LIV1"].ToString();

            folder.childs = GetFolderChildren(folder.systemID, table);
            return folder;
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="parent_id"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private System.Collections.ArrayList GetFolderChildren(string parent_id, System.Data.DataTable table)
        {
            DataTable table2 = table;
            System.Data.DataRow[] folderChildrenRows = table2.Select("ID_PARENT=" + parent_id);

            System.Collections.ArrayList folderChildren = new System.Collections.ArrayList();
            for (int i = 0; i < folderChildrenRows.Length; i++)
            {
                folderChildren.Add(GetFolderData(folderChildrenRows[i], table));
            }

            return folderChildren;
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="folder"></param>
        /// <returns>Folder o 'null' se si è verificato un errore.</returns>
        public DocsPaVO.fascicolazione.Folder ModifyFolder(DocsPaVO.fascicolazione.Folder folder)
        {
            //DocsPaWS.Utils.Database database = DocsPaWS.Utils.dbControl.getDatabase();
            try
            {

                //				database.openConnection();

                //string updateString="UPDATE PROJECT SET DESCRIPTION='"+folder.descrizione+"' WHERE CHA_TIPO_PROJ='C' AND SYSTEM_ID="+folder.systemID;
                //logger.Debug(updateString);

                DocsPaUtils.Query q =
                    DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_DESCRIPTION_CHA_TIPO_PROJ_C");
                q.setParam("param1", DocsPaUtils.Functions.Functions.ReplaceApexes(folder.descrizione));
                q.setParam("param2", folder.systemID);
                string updateString = q.getSQL();


                //database.executeNonQuery(updateString);
                this.ExecuteNonQuery(updateString);
            }
            catch (Exception e)
            {
                //logger.Debug(e.Message);
                string ErrMsg = e.Message;

                //database.closeConnection();

                //throw new Exception("F_System");
                logger.Debug("F_System");

                folder = null;
            }

            return folder;
        }

        public System.Collections.ArrayList GetDocumenti(string idGruppo, string idPeople, DocsPaVO.fascicolazione.Folder objFolder, DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca)
        {
            System.Collections.ArrayList listaDocumenti = new System.Collections.ArrayList();

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROFILE__SECURITY__PROJECT_COMPONENTS__OGGETTARIO__DOCUMENTTYPES");
                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.CREATION_DATE", false));
                //DocsPaWS.Utils.dbControl.toChar("A.CREATION_DATE",false));

                // per SQL
                if (dbType.ToUpper() == "SQL")
                {
                    q.setParam("dbuser", getUserDB());
                }

                if (objFolder.systemID != null)
                {
                    q.setParam("param2", objFolder.systemID);
                }
                else
                {
                    q.setParam("param2", "null");
                }
                q.setParam("param3", idGruppo);
                q.setParam("param4", idPeople);

                if (filtriRicerca != null)
                {
                    string queryFrom = string.Empty;
                    string sqlFilter = this.GetFilterStringQueryDocumentiInFascicolo(filtriRicerca, ref queryFrom);
                    q.setParam("param5", sqlFilter);
                    q.setParam("param6", queryFrom);
                }
                q.setParam("param7", objFolder.idFascicolo);

                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataSet;
                this.ExecuteQuery(out dataSet, "PROFILE", queryString);
                //db.fillTable(queryString, dataSet, "PROFILE");	
                //creazione della lista oggetti

                foreach (DataRow dataRow in dataSet.Tables["PROFILE"].Rows)
                {
                    Documenti doc = new Documenti();
                    listaDocumenti.Add(doc.GetInfoDocumento(idGruppo, idPeople, dataRow["SYSTEM_ID"].ToString(), true));
                }

                dataSet.Dispose();
                //db.closeConnection();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();

                //throw new Exception("F_System");
                logger.Debug("F_System");

                listaDocumenti = null;
            }

            return listaDocumenti;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <param name="objFolder"></param>
        /// <param name="filtriRicerca"></param>
        /// <param name="mittDest_indirizzo"></param>
        /// <param name="selectedDocumentsId">Id dei documenti selezionati</param>
        /// <returns></returns>
        public System.Collections.ArrayList GetDocumentiExport(string idGruppo, string idPeople, DocsPaVO.fascicolazione.Folder objFolder, DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca, bool mittDest_indirizzo, String[] selectedDocumentsId)
        {
            System.Collections.ArrayList listaDocumenti = new System.Collections.ArrayList();

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROFILE__SECURITY__PROJECT_COMPONENTS__OGGETTARIO__DOCUMENTTYPES");
                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.CREATION_DATE", false));
                //DocsPaWS.Utils.dbControl.toChar("A.CREATION_DATE",false));

                // per SQL
                if (dbType.ToUpper() == "SQL")
                {
                    q.setParam("dbuser", getUserDB());
                }

                if (objFolder.systemID != null)
                {
                    q.setParam("param2", objFolder.systemID);
                }
                else
                {
                    q.setParam("param2", "null");
                }
                q.setParam("param3", idGruppo);
                q.setParam("param4", idPeople);

                if (filtriRicerca != null)
                {
                    string queryFrom = string.Empty;
                    string sqlFilter = this.GetFilterStringQueryDocumentiInFascicolo(filtriRicerca, ref queryFrom);
                    q.setParam("param5", sqlFilter);
                    q.setParam("param6", queryFrom);
                }
                q.setParam("param7", objFolder.idFascicolo);
                if (mittDest_indirizzo)
                {
                    q.setParam("param8", "corrcat_address(A.system_id, A.cha_tipo_proto) as MITT_DEST,");
                }

                // Gestione esportazione dei soli documenti selezionati
                StringBuilder temp = new StringBuilder(" AND ( A.SYSTEM_ID IN(");
                if (selectedDocumentsId != null && selectedDocumentsId.Length > 0)
                {
                    /*    foreach (string id in selectedDocumentsId)
                            temp.Append(id + ",");

                        temp = temp.Remove(temp.Length - 1, 1);*/
                    int i = 0;
                    foreach (string id in selectedDocumentsId)
                    {
                        temp.Append(id);
                        if (i < selectedDocumentsId.Length - 1)
                        {
                            if (i % 998 == 0 && i > 0)
                            {
                                temp.Append(") OR A.SYSTEM_ID IN (");
                            }
                            else
                            {
                                temp.Append(", ");
                            }
                        }
                        else
                        {
                            temp.Append(")");
                        }
                        i++;
                    }

                }

                temp.Append(")");

                // temp.Append(")");
                q.setParam("param9", temp.ToString());


                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataSet;
                this.ExecuteQuery(out dataSet, "PROFILE", queryString);
                //db.fillTable(queryString, dataSet, "PROFILE");	
                //creazione della lista oggetti

                foreach (DataRow dataRow in dataSet.Tables["PROFILE"].Rows)
                {
                    Documenti doc = new Documenti();
                    if (mittDest_indirizzo)
                    {
                        DocsPaVO.documento.InfoDocumento infoDoc = doc.GetInfoDocumento(idGruppo, idPeople, dataRow["SYSTEM_ID"].ToString(), false);
                        infoDoc.mittDoc = dataRow["MITT_DEST"].ToString();
                        listaDocumenti.Add(infoDoc);
                    }
                    else
                        listaDocumenti.Add(doc.GetInfoDocumento(idGruppo, idPeople, dataRow["SYSTEM_ID"].ToString(), true));

                }

                dataSet.Dispose();
                //db.closeConnection();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //db.closeConnection();

                //throw new Exception("F_System");
                logger.Debug("F_System");

                listaDocumenti = null;
            }

            return listaDocumenti;
        }



        /// <summary>
        /// il metodo restituisce la stringa SQL all'omonimo metodo chiamante
        /// </summary>
        /// <remarks>
        /// La successiva implementazione consisterà nello spostare la logica del metodo
        /// chiamante all'interno di questo
        /// </remarks>
        /// <param name="objFolder"></param>
        /// <param name="infoUtente"></param>
        /// <returns>ArrayList o 'null' se si è verificato un errore.</returns>
        public System.Collections.ArrayList GetDocumenti(string idGruppo, string idPeople, DocsPaVO.fascicolazione.Folder objFolder)
        {
            return this.GetDocumenti(idGruppo, idPeople, objFolder, null);
        }

        private string GetFilterStringQueryDocumentiInFascicolo(DocsPaVO.filtri.FiltroRicerca[][] objQueryList, ref string queryFrom)
        {
            //string returnVal = "";
            string queryWhere = "";
            string andStr;
            int numAndStr = 0;
            System.Collections.ArrayList listaOR = new System.Collections.ArrayList();
            string UserDB = String.Empty;
            if (dbType.ToUpper() == "SQL") UserDB = getUserDB();

            for (int i = 0; i < objQueryList.Length; i++)
            {
                andStr = " (";
                numAndStr = 0;
                for (int j = 0; j < objQueryList[i].Length; j++)
                {
                    DocsPaVO.filtri.FiltroRicerca f = objQueryList[i][j];

                    if (f.valore != null && !f.valore.Equals(""))
                    {
                        switch (f.argomento)
                        {
                            case "TIPO":
                                if (numAndStr++ > 0)
                                    andStr += " AND ";

                                if (f.valore.Equals("T"))
                                    andStr += "A.CHA_TIPO_PROTO IN ('A', 'P', 'I', 'G')";
                                else
                                    andStr += "A.CHA_TIPO_PROTO='" + f.valore + "'";
                                break;

                            case "TIPO_ATTO":
                                if (numAndStr++ > 0)
                                    andStr += " AND ";
                                andStr += "A.ID_TIPO_ATTO=" + f.valore;
                                tipo_contatore = tipoContatoreTemplatesDoc(f.valore);
                                break;

                            case "PROFILAZIONE_DINAMICA":
                                DocsPaDB.Query_DocsPAWS.Model model = new Model();
                                andStr += model.getSeriePerRicercaProfilazione(f.template, "");
                                if (f.template != null && !string.IsNullOrEmpty(f.template.SYSTEM_ID.ToString()))
                                    tipo_contatore = this.tipoContatoreTemplatesDoc(f.template.SYSTEM_ID.ToString());
                                break;

                            case "SEGNATURA":
                                if (numAndStr++ > 0)
                                    andStr += " AND ";
                                andStr += "A.VAR_SEGNATURA='" + f.valore + "'";
                                break;

                            case "NUM_PROTOCOLLO":
                                if (numAndStr++ > 0)
                                    andStr += " AND ";
                                andStr += "A.NUM_PROTO=" + f.valore;
                                break;

                            case "NUM_PROTOCOLLO_AL":
                                if (numAndStr++ > 0)
                                    andStr += " AND ";
                                andStr += "A.NUM_PROTO<=" + f.valore;
                                break;

                            case "NUM_PROTOCOLLO_DAL":
                                if (numAndStr > 0)
                                    andStr += " AND ";
                                numAndStr += 1;
                                andStr += "A.NUM_PROTO>=" + f.valore;
                                break;

                            case "DATA_PROT_IL":
                                if (numAndStr > 0)
                                    andStr += " AND ";
                                numAndStr += 1;
                                andStr += "A.DTA_PROTO>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND " +
                                          "A.DTA_PROTO<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                                break;

                            case "DATA_PROT_SUCCESSIVA_AL":
                                if (numAndStr > 0)
                                    andStr += " AND ";
                                numAndStr += 1;
                                andStr += "A.DTA_PROTO>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                                break;

                            case "DATA_PROT_PRECEDENTE_IL":
                                if (numAndStr > 0)
                                    andStr += " AND ";
                                numAndStr += 1;
                                andStr += "A.DTA_PROTO<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                                break;

                            case "ANNO_PROTOCOLLO":
                                if (numAndStr > 0)
                                    andStr += " AND ";
                                numAndStr += 1;
                                andStr += "A.NUM_ANNO_PROTO=" + f.valore;
                                break;

                            case "DOCNUMBER":
                                if (numAndStr++ > 0)
                                    andStr += " AND ";
                                andStr += "A.DOCNUMBER=" + f.valore;
                                break;

                            case "DOCNUMBER_DAL":
                                if (numAndStr > 0)
                                    andStr += " AND ";
                                numAndStr += 1;
                                andStr += "A.DOCNUMBER>=" + f.valore;
                                break;

                            case "DOCNUMBER_AL":
                                if (numAndStr++ > 0)
                                    andStr += " AND ";
                                andStr += "A.DOCNUMBER<=" + f.valore;
                                break;

                            case "DATA_CREAZIONE_IL":
                                if (numAndStr > 0)
                                    andStr += " AND ";
                                numAndStr += 1;
                                andStr += "A.CREATION_DATE>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true) + " AND " +
                                          "A.CREATION_DATE<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                                break;

                            case "DATA_CREAZIONE_SUCCESSIVA_AL":
                                if (numAndStr > 0)
                                    andStr += " AND ";
                                numAndStr += 1;
                                andStr += "A.CREATION_DATE>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                                break;

                            case "DATA_CREAZIONE_PRECEDENTE_IL":
                                if (numAndStr > 0)
                                    andStr += " AND ";
                                numAndStr += 1;
                                andStr += "A.CREATION_DATE<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                                break;

                            case "OGGETTO":
                                if (numAndStr > 0)
                                    andStr += " AND ";
                                numAndStr += 1;
                                //andStr += "UPPER(A.VAR_PROF_OGGETTO) LIKE '%" + DocsPaUtils.Functions.Functions.ReplaceApexes(f.valore).ToUpper() + "%'";				
                                // La stringa di testo contenuta nel campo oggetto è messa in
                                // AND utilizzando come separatore la stringa fissa '&&'
                                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("&&");
                                string[] lista = regex.Split(f.valore);
                                andStr += "UPPER(A.VAR_PROF_OGGETTO) LIKE '%" + lista[0].ToUpper().Replace("'", "''") + "%'";
                                for (int k = 1; k < lista.Length; k++)
                                    andStr += " AND UPPER(A.VAR_PROF_OGGETTO) LIKE '%" + lista[k].ToUpper().Replace("'", "''") + "%'";
                                break;


                            case "ID_MITT_DEST":
                                queryWhere += "AND F.ID_PROFILE=A.SYSTEM_ID ";
                                queryFrom += ", DPA_DOC_ARRIVO_PAR F";
                                if (numAndStr > 0)
                                    andStr += " AND ";
                                numAndStr += 1;
                                andStr += "F.ID_MITT_DEST=" + f.valore + " AND CHA_TIPO_MITT_DEST='M'";
                                break;

                            case "MITT_DEST":
                                queryWhere += "AND F.ID_PROFILE=A.SYSTEM_ID AND G.SYSTEM_ID=F.ID_MITT_DEST ";
                                if (queryFrom.IndexOf(" ,DPA_DOC_ARRIVO_PAR F ,DPA_CORR_GLOBALI G ") < 0) queryFrom += " ,DPA_DOC_ARRIVO_PAR F ,DPA_CORR_GLOBALI G ";
                                if (numAndStr++ > 0)
                                    andStr += " AND ";
                                andStr += "UPPER(G.VAR_DESC_CORR) LIKE '%" + DocsPaUtils.Functions.Functions.ReplaceApexes(f.valore).ToUpper() + "%' AND CHA_TIPO_MITT_DEST='M'";
                                break;

                            case "ID_DESTINATARIO":
                                queryWhere += "AND F.ID_PROFILE=A.SYSTEM_ID ";
                                queryFrom += ", DPA_DOC_ARRIVO_PAR F";
                                if (numAndStr > 0)
                                    andStr += " AND ";
                                numAndStr += 1;
                                andStr += "F.ID_MITT_DEST=" + f.valore + " AND CHA_TIPO_MITT_DEST IN ('D', 'C', 'F')";
                                break;

                            case "ID_DESCR_DESTINATARIO":
                                queryWhere += "AND F.ID_PROFILE=A.SYSTEM_ID AND G.SYSTEM_ID=F.ID_MITT_DEST ";
                                if (queryFrom.IndexOf(" ,DPA_DOC_ARRIVO_PAR F ,DPA_CORR_GLOBALI G ") < 0) queryFrom += " ,DPA_DOC_ARRIVO_PAR F ,DPA_CORR_GLOBALI G ";
                                if (numAndStr++ > 0)
                                    andStr += " AND ";
                                andStr += "UPPER(G.VAR_DESC_CORR) LIKE '%" + DocsPaUtils.Functions.Functions.ReplaceApexes(f.valore).ToUpper() + "%' AND CHA_TIPO_MITT_DEST IN ('D', 'C', 'F')";
                                break;

                            case "FIRMATO":
                                andStr += " AND ";
                                numAndStr += 1;
                                if (f.valore == "1")
                                {
                                    if (!string.IsNullOrEmpty(UserDB))
                                        andStr += UserDB + ".getchafirmato(A.DOCNUMBER) = '1'";
                                    else
                                        andStr += "getchafirmato(A.DOCNUMBER) = '1'";
                                }
                                else
                                    if (f.valore == "0")
                                    {
                                        if (!string.IsNullOrEmpty(UserDB))
                                            andStr += UserDB + ".getchafirmato(A.DOCNUMBER) = '0'";
                                        else
                                            andStr += "getchafirmato(A.DOCNUMBER) = '0'";
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(UserDB))
                                        {
                                            andStr += UserDB + ".getchaimg(A.SYSTEM_ID)<>'0'";
                                        }
                                        else
                                        {
                                            andStr += "getchaimg(A.SYSTEM_ID)<>'0'";
                                        }
                                    }
                                break;

                            case "TIPO_FILE_ACQUISITO":
                                queryWhere += "AND A.DOCNUMBER=COMPONENTS.DOCNUMBER ";
                                if (queryFrom.IndexOf(" ,COMPONENTS") < 0)
                                    queryFrom += " ,COMPONENTS ";

                                // if (numAndStr > 0)
                                andStr += " AND ";
                                numAndStr += 1;
                                andStr += "UPPER(COMPONENTS.EXT)='" + f.valore.ToUpper() + "' AND COMPONENTS.VERSION_ID=(select max(versions.version_id)  from versions, components where" +
                                " versions.version_id=components.version_id AND versions.docnumber=A.DOCNUMBER)";
                                break;
                        }
                    }
                }
                andStr += ") ";
                if (andStr.Length > 4)
                {
                    listaOR.Add(andStr);
                    numAndStr = 0;
                }
            }
            if (listaOR.Count > 0)
            {
                queryWhere += " AND (" + (string)listaOR[0];
                for (int i = 1; i < numAndStr; i++)
                    queryWhere += " OR " + listaOR[i];
                queryWhere += ") ";

            }
            return queryWhere;
        }

        public System.Collections.ArrayList GetDocumentiPaging(
                string idGruppo,
                string idPeople,
                DocsPaVO.fascicolazione.Folder objFolder,
                DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca,
                int numPage,
                out int numTotPage,
                out int nRec,
                bool compileIdProfileList,
                out List<SearchResultInfo> idProfiles)
        {
            nRec = 0;
            numTotPage = 0;
            System.Collections.ArrayList listaDocumenti = new System.Collections.ArrayList();
            List<SearchResultInfo> idProfileList = null;
            try
            {
                string queryFrom = null;
                string sqlFilter = string.Empty;

                if (string.IsNullOrEmpty(objFolder.systemID))
                {
                    listaDocumenti = null;
                    throw new Exception("ID folder in GetDocumentiPaging fascicolo è vuoto.");
                }
                if (filtriRicerca != null)
                {
                    queryFrom = string.Empty;
                    sqlFilter = this.GetFilterStringQueryDocumentiInFascicolo(filtriRicerca, ref queryFrom);

                }

                // per SQL

                nRec = this.GetCountDocumenti(idGruppo, idPeople, objFolder.systemID, queryFrom, sqlFilter, compileIdProfileList, out idProfileList);
                // Reperimento del numero di elementi da visualizzare per pagina
                int pageSize = 15;
                int pageSizeSqlServer = pageSize;

                // per query sqlserver:
                // il numero totale di righe da estrarre equivale 
                // al limite inferiore dell'ultima riga da estrarre
                int totalRowsSqlServer = (numPage * pageSize);
                if ((nRec - totalRowsSqlServer) <= 0)
                {
                    pageSizeSqlServer -= System.Math.Abs(nRec - totalRowsSqlServer);
                    totalRowsSqlServer = nRec;
                }



                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROFILE_SECURITY_PROJECT_COMPONENTS_DOCUMENTTYPES_NEW");

                string security = string.Empty;

                if (dbType.ToUpper().Equals("ORACLE"))
                {
                    if (!string.IsNullOrEmpty(tipo_contatore))
                    {
                        q.setParam("tipo_doc_cond", " , GetContatoreDoc(a.system_id, @tipo_doc@) as CONTATORE, getContatoreDocOrdinamento(a.system_id, @tipo_doc@) as contatore_ordinamento ");
                        q.setParam("tipo_doc", "'" + tipo_contatore + "'");
                    }
                    else
                    {
                        q.setParam("tipo_doc_cond", "");
                        q.setParam("tipo_doc", "'" + tipo_contatore + "'");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(tipo_contatore))
                        q.setParam("tipo_doc", "'" + tipo_contatore + "'");
                    else
                        q.setParam("tipo_doc", "'" + tipo_contatore + "'");
                }

                bool IS_ARCHIVISTA_DEPOSITO;
                DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
                IS_ARCHIVISTA_DEPOSITO = ut.isUtArchivistaDeposito(idPeople, idGruppo);
                string idAmm = "0";
                if (!string.IsNullOrEmpty(idPeople))
                {
                    DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                    idAmm = u.GetIdAmm(idPeople);
                }
                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
                if (string.IsNullOrEmpty(idRuoloPubblico))
                    idRuoloPubblico = "0";
                if (IS_ARCHIVISTA_DEPOSITO)
                {
                    if (dbType.ToUpper() == "SQL")
                        security = " (@dbuser@.checkSecurityDocumento(A.SYSTEM_ID, @param3@, @param4@, @idRuoloPubblico@,'D') > 0)";
                    else
                        security = " (checkSecurityDocumento(A.SYSTEM_ID, @param3@, @param4@, @idRuoloPubblico@,'D') > 0)";
                }
                else
                {
                    if (IndexSecurity())
                        security = " EXISTS (select /*+INDEX(e) */  'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in (@param3@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                    else
                        security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in (@param3@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                }
                if (security == string.Empty)
                {
                    if (IndexSecurity())
                        security = " EXISTS (select /*+INDEX(e) */  'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in (@param3@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                    else
                        security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in (@param3@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                }

                q.setParam("security", security);

                if (dbType.ToUpper() == "SQL")
                {
                    q.setParam("dbuser", getUserDB());
                }

                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.CREATION_DATE", false));
                q.setParam("param2", objFolder.systemID);
                q.setParam("param7", objFolder.idFascicolo);
                q.setParam("param4", idGruppo);
                q.setParam("param3", idPeople);
                q.setParam("idRuoloPubblico", idRuoloPubblico);

                if (filtriRicerca != null)
                {
                    q.setParam("param5", sqlFilter);
                    q.setParam("param6", queryFrom);
                }


                //// Parametri specifici per query sqlserver
                q.setParam("pageSize", pageSizeSqlServer.ToString());
                q.setParam("totalRows", totalRowsSqlServer.ToString());




                //q.setParam("rowCount", maxRisultatiQuery.ToString());

                //// Parametri validi per tutte le query
                // q.setParam("idGruppo", idGruppo);
                //q.setParam("idPeople", idPeople);
                //q.setParam("filters", filters);

                //// Parametro contentente le tabelle addizionali da aggiungere alla query
                // queryDef.setParam("from", fromTables);

                //// Parametri per l'impostazione dell'ordinamento
                //q.setParam("order", orderCriteria);



                numTotPage = (nRec / pageSize);

                int startRow = ((numPage * pageSize) - pageSize) + 1;
                int endRow = (startRow - 1) + pageSize;

                q.setParam("startRow", startRow.ToString());
                q.setParam("endRow", endRow.ToString());



                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataSet;
                //this.ExecutePaging(out dataSet,out numTotPage,out nRec,numPage,20,queryString,"PROFILE");
                this.ExecuteQuery(out dataSet, queryString);
                //creazione della lista oggetti
                dataSet.Tables["ResultTable"].TableName = "DOCUMENTI";

                // Reperimento oggetto arraylist di oggetti "InfoDocumento"
                Documenti doc = new Documenti();
                listaDocumenti = doc.GetArrayListDocumenti(dataSet.Tables["DOCUMENTI"], true);
                doc.Dispose();
                doc = null;

                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);


                listaDocumenti = null;
            }

            idProfiles = idProfileList;
            return listaDocumenti;
        }

        /// <summary>
        /// Reperimento del numero totale di documenti estratti nella query paginata
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <param name="objQueryList"></param>
        /// <param name="compileIdProfileList">True se bisogna compilare la lista dei system id dei documenti all'interno del fascicolo</param>
        /// <param name="idProfiles">Lista dei system id dei documenti contenuti all'interno del fascicolo</param>
        /// <returns></returns>
        private int GetCountDocumenti(string idGruppo,
                                        string idPeople,
                                        string objFolderID,
                                        string fromTables,
                                        string filterString,
                                        bool compileIdProfileList,
                                        out List<SearchResultInfo> idProfiles)
        {
            // Lista dei system id dei documenti contenuti all'interno del fascicolo
            List<SearchResultInfo> idProfilesList = null;

            int retValue = 0;

            string queryName = string.Empty;

            // Se bisogna compilare la lista dei system id dei documenti, viene eseguita la query
            // per il reperimento dei system id dei documenti altrimenti viene eseguita quella
            // per il solo conteggio dei documenti
            if (compileIdProfileList)
                queryName = "S_GET_COUNT_ROWS_RICERCA_DOCUMENTI_IN_FASCICOLO_MASSIVE_OPERATION";
            else
                queryName = "S_GET_COUNT_ROWS_RICERCA_DOCUMENTI_IN_FASCICOLO";


            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery(queryName);

            string queryFrom = string.Empty;
            queryFrom = fromTables;

            // Parametri specifici per query oracle
            //string preferedIndex = string.Empty;
            //if (!(filterString.IndexOf("A.VAR_PROF_OGGETTO") > -1 ||
            //    filterString.IndexOf("G.VAR_DESC_CORR") > -1))
            //// Impostazione indice da utilizzare
            //{
            //    if (filterString.ToUpper().IndexOf("NVL(A.DTA_PROTO") > -1)
            //        //preferedIndex = "/*+index (a indx_profile_data)*/";
            //        //preferedIndex = "/*+index (a )*/";
            //        preferedIndex = " ";
            //    else
            //        if (filterString.IndexOf("A.CREATION_") > -1)
            //            preferedIndex = "/*+index (a indx_profile_time)*/";
            //        else
            //            preferedIndex = "/*+index (a indx_profile6)*/";
            //}
            //queryDef.setParam("index", preferedIndex);
            string security = string.Empty;
            bool IS_ARCHIVISTA_DEPOSITO;
            DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
            IS_ARCHIVISTA_DEPOSITO = ut.isUtArchivistaDeposito(idPeople, idGruppo);
            string idAmm = "0";
            if (!string.IsNullOrEmpty(idPeople))
            {
                DocsPaDB.Query_DocsPAWS.Utenti u = new Utenti();
                idAmm = u.GetIdAmm(idPeople);
            }
            string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
            if (string.IsNullOrEmpty(idRuoloPubblico))
                idRuoloPubblico = "0";
            if (IS_ARCHIVISTA_DEPOSITO)
            {
                if (dbType.ToUpper() == "SQL")
                    security = "(@dbuser@.checkSecurityDocumento(A.SYSTEM_ID, @param3@, @param4@, @idRuoloPubblico@,'D') > 0)";
                else
                    security = "(checkSecurityDocumento(A.SYSTEM_ID, @param3@, @param4@, @idRuoloPubblico@,'D') > 0)";
            }
            else
            {
                if (IndexSecurity())
                    security = " EXISTS (select /*+index (e) */ 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in (@param3@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                else
                    security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in (@param3@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
            }
            if (security == string.Empty)
            {
                if (IndexSecurity())
                    security = " EXISTS (select /*+index (e) */ 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in (@param3@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                else
                    security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in (@param3@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
            }

            queryDef.setParam("security", security);


            queryDef.setParam("param2", objFolderID);
            queryDef.setParam("param4", idGruppo);
            queryDef.setParam("param3", idPeople);
            queryDef.setParam("idRuoloPubblico", idRuoloPubblico);
            queryDef.setParam("param5", filterString);
            queryDef.setParam("param6", queryFrom);
            // Il flag indica se includere nella ricerca anche i documenti di tipo allegato
            queryDef.setParam("from", fromTables);
            queryDef.setParam("filters", filterString);
            queryDef.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

            queryDef.setParam("totalRows", GetMaxRisultatiQuery());

            string commandText = queryDef.getSQL();

            logger.Debug(commandText);

            string field;

            // Se bisogna caricare la lista dei documenti, viene popolata la lista dei
            // system id
            if (compileIdProfileList)
            {
                DataSet dataSet;
                idProfilesList = new List<SearchResultInfo>();
                // Esecuzione della query
                if (this.ExecuteQuery(out dataSet, commandText))
                    foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                    {
                        SearchResultInfo temp = new SearchResultInfo();
                        temp.Id = dataRow["SYSTEM_ID"].ToString();
                        temp.Codice = dataRow["CODICE"].ToString();
                        idProfilesList.Add(temp);
                    }

                retValue = idProfilesList.Count;

            }
            else
            {
                // Esecuzione del conteggio dei documenti contenuti nel fascicolo
                if (this.ExecuteScalar(out field, commandText))
                    Int32.TryParse(field, out retValue);
            }

            idProfiles = idProfilesList;
            return retValue;
        }
        private string GetMaxRisultatiQuery()
        {

            string maxRowCount = System.Configuration.ConfigurationManager.AppSettings["numeroMaxRisultatiQuery"];
            if (string.IsNullOrEmpty(maxRowCount))
                maxRowCount = "1000000";

            return maxRowCount;
        }
        public System.Collections.ArrayList GetDocumentiPaging(
                string idGruppo,
                string idPeople,
                DocsPaVO.fascicolazione.Folder objFolder,
                int numPage,
                out int numTotPage,
                out int nRec,
                bool compileIdProfileList,
                out List<SearchResultInfo> idProfiles)
        {

            List<SearchResultInfo> idProfileList = null;
            ArrayList toReturn = this.GetDocumentiPaging(idGruppo,
                                            idPeople,
                                            objFolder,
                                            null,
                                            numPage,
                                            out numTotPage,
                                            out nRec,
                                            compileIdProfileList,
                                            out idProfileList);
            idProfiles = idProfileList;
            return toReturn;
        }



        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="idProfile"></param>
        /// <param name="idFolder"></param>
        /// <param name="infoUtente"></param>
        public bool AddDocFolder(DocsPaVO.utente.InfoUtente infoUtente, string idGruppo, string idProfile, string idFolder)
        {
            logger.Info("BEGIN");
            bool result = false;

            try
            {
                string queryString = "";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponents");
                q.setParam("param1", idFolder);
                q.setParam("param2", idProfile);
                queryString = q.getSQL();
                logger.Debug(queryString);

                string resOut;

                this.ExecuteScalar(out resOut, queryString);

                if (resOut.Equals("0"))//caso in cui il documento non è stato fasciolato nella folder corrente
                {
                    string insertString = "";

                    this.BeginTransaction();

                    //NUOVO REQUISITO: FASCICOLAZIONE PRIMARIA
                    //Prima di inserire il doc in fascicolo, si verifica che non sia già stato inserito in un altro
                    //fascicolo, se non lo è e se è attiva la chiave di configurazione "CHA_FASC_PRIMARIA" allora
                    //si inserisce il documento in fascicolo impostanto il nuovo campo CHA_FASC_PRIMARIA a 1
                    string queryFascPrimaria = string.Empty;
                    DocsPaUtils.Query queryFascPrim = DocsPaUtils.InitQuery.getInstance().getQuery("S_ESISTE_FASC_PRIMARIA");
                    queryFascPrim.setParam("link", idProfile);
                    queryFascPrimaria = queryFascPrim.getSQL();
                    logger.Debug(queryFascPrimaria);
                    string fascPrimaria;
                    this.ExecuteScalar(out fascPrimaria, queryFascPrimaria);
                    if (Convert.ToInt32(fascPrimaria) > 0)
                        fascPrimaria = "0";
                    else
                        fascPrimaria = "1";


                    DocsPaUtils.Query qInsert = DocsPaUtils.InitQuery.getInstance().getQuery("I_PROJECT_COMPONENTS");
                    qInsert.setParam("param1", idFolder);
                    qInsert.setParam("param2", idProfile);
                    qInsert.setParam("dataClassifica", DocsPaDbManagement.Functions.Functions.GetDate());
                    qInsert.setParam("fascPrimaria", fascPrimaria);
                    insertString = qInsert.getSQL();
                    logger.Debug(insertString);
                    if (!this.ExecuteNonQuery(insertString))
                        throw new Exception();

                    string updateString = "";
                    DocsPaUtils.Query qUpdate = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROFILE_CHA_FASCICOLATO");
                    qUpdate.setParam("param1", idProfile);
                    updateString = qUpdate.getSQL();
                    logger.Debug(updateString);
                    if (!this.ExecuteNonQuery(updateString))
                        throw new Exception();




                    //REQUISITO: visibilità dei documenti slegata dai fascicoli in cui devono essere inseriti
                    //Se il fascicolo è controllato i documenti in esso inseriti non ne ereditano le ACL
                    string queryControllato = string.Empty;
                    DocsPaUtils.Query queryC = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_CONTROLLATO_FOLDER");
                    queryC.setParam("system_id", idFolder);
                    queryControllato = queryC.getSQL();
                    logger.Debug(queryControllato);
                    string isControllato;
                    this.ExecuteScalar(out isControllato, queryControllato);
                    //Se il fascicolo è controllato allora il documento non eredita le ACL
                    if (string.IsNullOrEmpty(isControllato) || isControllato.Equals("0"))
                    {
                        updateDocTrustees(idGruppo, idProfile, idFolder);
                    }
                    creaProtTitRifMitt(infoUtente, idGruppo, idProfile, idFolder);

                    result = true;

                    this.CommitTransaction();
                }
                else
                {
                    // se il doc è già fascicolato ritorno true
                    //questa modifica è stata introdotta a seguito di un bug dovuto al checkin del
                    //12aprile2007 quando è stato cambiato il valore di ritorno di dafault del metodo da true a false,
                    result = false;
                }
            }
            catch (Exception e)
            {
                string ErrMsg = e.Message;
                this.RollbackTransaction();

                logger.Debug("F_System");

                result = false;
            }
            finally
            {
                this.CloseConnection();
            }
            logger.Info("END");
            return result;
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="infoDoc"></param>
        public bool DeleteDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Folder folder, string idProfile)
        {
            bool result = true; // Presume successo

            try
            {
                string deleteQuery = "";
                string queryString = "";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_PROJECT_COMPONENT_WHERE_TYPE");
                q.setParam("param1", idProfile);
                //old
                //				DocsPaUtils.Query q_1 = DocsPaUtils.InitQuery.getInstance().getQuery("GET_PROJECT_ID_FROM_LINK");
                //				q_1.setParam("param1",idProfile);
                //				DataSet ds = new DataSet();
                //				this.ExecuteQuery(ds,q_1.getSQL());
                //				q.setParam("param2",ds.Tables[0].Rows[0][0].ToString());
                // fine old				
                //new:
                q.setParam("param2", folder.systemID);
                // fine new

                deleteQuery = q.getSQL();
                this.ExecuteNonQuery(deleteQuery);

                DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponents6");
                q2.setParam("param1", idProfile);
                queryString = q2.getSQL();
                logger.Debug(queryString);
                string resOut;
                this.ExecuteScalar(out resOut, queryString);
                //SE IL DOC NON è PIU' contenuto in nessun fascicolo allora rimetto a '0' il
                //campo CHA_FASCICOLATO SULLA PROFILE
                if (resOut.Equals("0"))
                {
                    string updateString = "";
                    DocsPaUtils.Query qUpdate = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROFILE_CHA_FASCICOLATO_ZERO");
                    qUpdate.setParam("param1", idProfile);
                    updateString = qUpdate.getSQL();
                    logger.Debug(updateString);
                    this.ExecuteNonQuery(updateString);
                }

                eliminaProtTitRifMitt(infoUtente, folder, idProfile);
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                result = false;
            }

            return result;
        }

        public bool DeleteDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Folder folder, string idProfile, string numberOfRow)
        {
            bool result = true; // Presume successo

            try
            {
                string deleteQuery = "";
                string queryString = "";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_PROJECT_COMPONENT_WHERE_TYPE_2");
                q.setParam("param1", idProfile);
                //				DocsPaUtils.Query q_1 = DocsPaUtils.InitQuery.getInstance().getQuery("GET_PROJECT_ID_FROM_LINK");
                //				q_1.setParam("param1",idProfile);
                //				DataSet ds = new DataSet();
                //				this.ExecuteQuery(ds,q_1.getSQL());
                //				if(ds.Tables[0].Rows.Count > 1)
                //					q.setParam("param2",ds.Tables[0].Rows[Int16.Parse(numberOfRow)][0].ToString());
                //				else
                //					q.setParam("param2",ds.Tables[0].Rows[0][0].ToString());


                q.setParam("param2", folder.idFascicolo);
                deleteQuery = q.getSQL();
                this.ExecuteNonQuery(deleteQuery);

                DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponents6");
                q2.setParam("param1", idProfile);
                queryString = q2.getSQL();
                logger.Debug(queryString);
                string resOut;
                this.ExecuteScalar(out resOut, queryString);
                //SE IL DOC NON è PIU' contenuto in nessun fascicolo allora rimetto a '0' il
                //campo CHA_FASCICOLATO SULLA PROFILE
                if (resOut.Equals("0"))
                {
                    string updateString = "";
                    DocsPaUtils.Query qUpdate = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROFILE_CHA_FASCICOLATO_ZERO");
                    qUpdate.setParam("param1", idProfile);
                    updateString = qUpdate.getSQL();
                    logger.Debug(updateString);
                    this.ExecuteNonQuery(updateString);
                }
                else
                {

                    //TODO: chiave fasc_primaria
                    string queryFascPrimaria = string.Empty;
                    DocsPaUtils.Query queryFascPrim = DocsPaUtils.InitQuery.getInstance().getQuery("S_ESISTE_FASC_PRIMARIA");
                    queryFascPrim.setParam("link", idProfile);
                    queryFascPrimaria = queryFascPrim.getSQL();
                    logger.Debug(queryFascPrimaria);
                    string fascPrimaria;
                    this.ExecuteScalar(out fascPrimaria, queryFascPrimaria);
                    if (Convert.ToInt32(fascPrimaria) == 0)
                    {
                        string qString = string.Empty;
                        DocsPaUtils.Query queryFascicoli = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_FASCICOLI_FASC_PRIMARIA");
                        queryFascicoli.setParam("param1", idProfile);
                        qString = queryFascicoli.getSQL();
                        logger.Debug(qString);
                        DataSet dataSet;
                        this.ExecuteQuery(out dataSet, "FASCICOLI", qString);
                        if (dataSet.Tables[0].Rows.Count > 0)
                        {

                            string data = dataSet.Tables["FASCICOLI"].Rows[0]["DTA_CLASS"].ToString();
                            string data2;
                            string system_id = dataSet.Tables["FASCICOLI"].Rows[0]["PROJECT_ID"].ToString();
                            for (int i = 1; i < dataSet.Tables["FASCICOLI"].Rows.Count; i++)
                            {
                                data2 = dataSet.Tables["FASCICOLI"].Rows[i]["DTA_CLASS"].ToString();
                                if (Convert.ToDateTime(data2) < Convert.ToDateTime(data))
                                {
                                    system_id = dataSet.Tables["FASCICOLI"].Rows[i]["PROJECT_ID"].ToString();
                                }
                            }
                            if (!string.IsNullOrEmpty(system_id))
                            {
                                string updateFascPrimaria = "";
                                DocsPaUtils.Query qUpdatePrimaria = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_COMPONENTS_CHA_FASC_PRIMARIA");
                                qUpdatePrimaria.setParam("param1", system_id);
                                qUpdatePrimaria.setParam("param2", idProfile);
                                updateFascPrimaria = qUpdatePrimaria.getSQL();
                                logger.Debug(updateFascPrimaria);
                                this.ExecuteNonQuery(updateFascPrimaria);
                            }
                        }

                    }
                }

                eliminaProtTitRifMitt(infoUtente, folder, idProfile);
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                result = false;
            }

            return result;
        }


        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="db"></param>
        /// <param name="idProfile"></param>
        /// <param name="idFolder"></param>
        /// <param name="infoUtente"></param>
        public void updateDocTrustees(string idGruppo, string idProfile, string idFolder)
        {
            // l'operazione va fatta solo se si tratta di un fascicolo procedimentale
            string queryString = "";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project9");
            q.setParam("param1", idFolder);
            queryString = q.getSQL();
            string result = "";
            this.ExecuteScalar(out result, queryString);

            if (!result.ToUpper().Equals("P"))
            {
                return;
            }

            // leggo il registro cui è associato il documento
            queryString = "";
            DocsPaUtils.Query qSelect = DocsPaUtils.InitQuery.getInstance().getQuery("S_Profile4");
            qSelect.setParam("param1", idProfile);
            queryString = qSelect.getSQL();
            string idReg = "";
            string filtroReg = "";
            string filtroReg2 = "";

            this.ExecuteScalar(out idReg, queryString);
            // string tabRegistro = "";
            // string condRegistro = "";
            string union = string.Empty;


            //Emanuela: poichè se si rimuove le visibilita del fascicolo ad un ruolo non viene rimossa anche per i sottofascicoli, nel caso di inserimento di un doc
            //in un sottofascicolo il ruolo ottiene i diritti sul doc; vado quindi ad inserire nella security solo se esiste la coppiaesiste la coppia ruolo/idParent 
            //estraggo l'id parent del fascicolo
            queryString = "";
            DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROJECT_ID_FASCICOLO");
            q1.setParam("param1", idFolder);
            queryString = q1.getSQL();
            string idFascicolo = string.Empty;
            this.ExecuteScalar(out idFascicolo, queryString);
            // aggiungo al documento tutti i diritti dati al fascicolo		
            string updateString = "";
            DocsPaUtils.Query qUpdate = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY_SELECT_MODIFICATA");

            if (idReg != null && idReg.ToString() != null && !idReg.Equals(""))
            {
                filtroReg = " AND b.id_registro = " + idReg.ToString() + " ";

                /*    tabRegistro = ", DPA_L_RUOLO_REG B, DPA_CORR_GLOBALI C";
                    condRegistro = "PERSONORGROUP=C.ID_GRUPPO AND C.SYSTEM_ID=B.ID_RUOLO_IN_UO AND B.ID_REGISTRO=" + idReg.ToString() + " AND ";
                    union = " UNION SELECT DISTINCT " + idProfile + ", PERSONORGROUP, ACCESSRIGHTS, " + idGruppo + ", 'F' " +
                            " FROM SECURITY, DPA_L_RUOLO_REG B, DPA_CORR_GLOBALI C,PEOPLEGROUPS D " +
                            " WHERE PERSONORGROUP=D.PEOPLE_SYSTEM_ID AND D.DTA_FINE IS NULL AND " +
                            " C.ID_GRUPPO=D.GROUPS_SYSTEM_ID AND " +
                            " C.SYSTEM_ID=B.ID_RUOLO_IN_UO " + " AND ACCESSRIGHTS!=0 ";
                    */
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["NO_FILTRO_AOO"]) &&
                    System.Configuration.ConfigurationManager.AppSettings["NO_FILTRO_AOO"] == "1")
                {

                    /*  union +=" AND THING=" + idFolder + " AND NOT PERSONORGROUP IN " +
                      "(SELECT PERSONORGROUP FROM SECURITY WHERE THING=" + idProfile + ")";*/
                }
                else
                {
                    filtroReg2 = " AND B.ID_REGISTRO=" + idReg.ToString() + " ";
                    /*  union += " AND B.ID_REGISTRO=" + idReg.ToString() +
                      " AND THING=" + idFolder + " AND NOT PERSONORGROUP IN " +
                      "(SELECT PERSONORGROUP FROM SECURITY WHERE THING=" + idProfile + ")";*/
                }
                qUpdate.setParam("param8", filtroReg);
                qUpdate.setParam("param9", filtroReg2);
            }
            else
            {
                qUpdate = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY_SELECT_MODIFICATA_SEMPL");
            }

            qUpdate.setParam("param1", idProfile);
            qUpdate.setParam("param2", idGruppo);
            // qUpdate.setParam("param3", tabRegistro);
            // qUpdate.setParam("param4", condRegistro);
            qUpdate.setParam("param5", idFascicolo);
            //qUpdate.setParam("param6", idProfile);
            //  qUpdate.setParam("param7", union); 
            updateString = qUpdate.getSQL();
            logger.Debug(updateString);
            this.ExecuteNonQuery(updateString);

            //SE C'E' PIU' DI UN DOCUMENTO CON ACCESSRIGHTS A 255

            string updateStringDoc = "";
            DocsPaUtils.Query qUpdateDocSec = DocsPaUtils.InitQuery.getInstance().getQuery("U_SECURITY_ACCESSRIGHTS_DOCUMENTO");
            qUpdateDocSec.setParam("idProfile", idProfile);
            updateStringDoc = qUpdateDocSec.getSQL();
            logger.Debug(updateStringDoc);
            this.ExecuteNonQuery(updateStringDoc);

        }

        /// <summary>
        /// GetIdFolderDoc, con filtro sui registri
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="regs"></param>
        /// <returns></returns>
        public System.Collections.ArrayList GetIdFolderDoc(string idFascicolo, ArrayList regs)
        {
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            System.Data.DataSet ds = new DataSet();
            System.Collections.ArrayList lista = new System.Collections.ArrayList();
            //bool dbOpen=false;
            try
            {
                //db.openConnection();

                //trova tutte le folder appartenenti al fascicolo
                //string queryFolderString="SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ='C' AND ID_FASCICOLO="+idFascicolo;
                string queryFolderString = "";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project10");

                q.setParam("param1", idFascicolo);
                //db.fillTable(queryFolderString,ds,"FOLDER");
                queryFolderString = q.getSQL();
                this.ExecuteQuery(ds, "FOLDER", queryFolderString);

                for (int i = 0; i < ds.Tables["FOLDER"].Rows.Count; i++)
                {
                    lista.Add(ds.Tables["FOLDER"].Rows[i]["SYSTEM_ID"].ToString());
                }

                //trova tutti i documenti appartenenti alle folders
                string queryDocString = "";
                string DocString = "";
                DocsPaUtils.Query qSelect = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROJECT_COMPONENTS_4");
                //"SELECT LINK FROM PROJECT_COMPONENTS,PROFILE  WHERE PROIFLE.SYSTEM_ID=PROJECT_COMPONENTS.LINK AND ( PROFILE.ID_REGISTRO IS NULL OR PROFILE.ID_REGISTRO IN (...) ) AND PROJECT_ID IN (";

                for (int j = 0; j < ds.Tables["FOLDER"].Rows.Count; j++)
                {
                    queryDocString = queryDocString + ds.Tables["FOLDER"].Rows[j]["SYSTEM_ID"].ToString();
                    if (j < ds.Tables["FOLDER"].Rows.Count - 1)
                    {
                        queryDocString = queryDocString + ",";
                    }
                }
                //				queryDocString=queryDocString+")";
                qSelect.setParam("param1", queryDocString);
                if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["NO_FILTRO_AOO"]) &&
                    System.Configuration.ConfigurationManager.AppSettings["NO_FILTRO_AOO"] == "1")
                {
                    qSelect.setParam("param2", "");
                }
                else
                {
                    if (regs != null && regs.Count > 0)
                    {
                        string r = "(";
                        for (int i = 0; i < regs.Count; i++)
                        {
                            DocsPaVO.utente.Registro registro = (DocsPaVO.utente.Registro)regs[i];
                            if (i == 0)
                                r += registro.systemId;
                            else
                                r += "," + registro.systemId;
                        }
                        r += ")";
                        qSelect.setParam("param2", " AND ( P.ID_REGISTRO IS NULL OR P.ID_REGISTRO IN " + r + " )");
                    }
                }
                DocString = qSelect.getSQL();
                //db.fillTable(queryDocString,ds,"DOC");
                this.ExecuteQuery(ds, "DOC", DocString);
                for (int k = 0; k < ds.Tables["DOC"].Rows.Count; k++)
                {
                    lista.Add(ds.Tables["DOC"].Rows[k]["LINK"].ToString());
                }
            }
            catch (Exception e)
            {

                /*if(dbOpen)
                {
                    //db.closeConnection();

                }*/
                //throw e;
                logger.Debug(e.ToString());

                lista = null;
            }

            return lista;
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <returns>ArrayList o 'null' se si è verificato un errore.</returns>
        public System.Collections.ArrayList GetIdFolderDoc(string idFascicolo)
        {
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            System.Data.DataSet ds = new DataSet();
            System.Collections.ArrayList lista = new System.Collections.ArrayList();
            //bool dbOpen=false;
            try
            {
                //db.openConnection();

                //trova tutte le folder appartenenti al fascicolo
                //string queryFolderString="SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ='C' AND ID_FASCICOLO="+idFascicolo;
                string queryFolderString = "";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project10");

                q.setParam("param1", idFascicolo);
                //db.fillTable(queryFolderString,ds,"FOLDER");
                queryFolderString = q.getSQL();
                this.ExecuteQuery(ds, "FOLDER", queryFolderString);

                for (int i = 0; i < ds.Tables["FOLDER"].Rows.Count; i++)
                {
                    lista.Add(ds.Tables["FOLDER"].Rows[i]["SYSTEM_ID"].ToString());
                }

                //trova tutti i documenti appartenenti alle folders
                string queryDocString = "";
                string DocString = "";
                DocsPaUtils.Query qSelect = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponents3");
                //"SELECT LINK FROM PROJECT_COMPONENTS WHERE PROJECT_ID IN (";

                for (int j = 0; j < ds.Tables["FOLDER"].Rows.Count; j++)
                {
                    queryDocString = queryDocString + ds.Tables["FOLDER"].Rows[j]["SYSTEM_ID"].ToString();
                    if (j < ds.Tables["FOLDER"].Rows.Count - 1)
                    {
                        queryDocString = queryDocString + ",";
                    }
                }
                //				queryDocString=queryDocString+")";
                qSelect.setParam("param1", queryDocString);
                DocString = qSelect.getSQL();

                //db.fillTable(queryDocString,ds,"DOC");
                this.ExecuteQuery(ds, "DOC", DocString);
                for (int k = 0; k < ds.Tables["DOC"].Rows.Count; k++)
                {
                    lista.Add(ds.Tables["DOC"].Rows[k]["LINK"].ToString());
                }
            }
            catch (Exception e)
            {

                /*if(dbOpen)
                {
                    //db.closeConnection();

                }*/
                //throw e;
                logger.Debug(e.ToString());

                lista = null;
            }

            return lista;
        }


        public string CheckFolder(string idParent, string descrizione)
        {
            string result = null;
            try
            {
                string query = "";
                //string counter;

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_FOLDER_ROOT");
                q.setParam("param1", idParent);
                q.setParam("param2", descrizione);

                query = q.getSQL();

                this.ExecuteScalar(out result, query);
                //if(counter!="0") result=true;

            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                result = null;
            }
            return result;
        }

        #endregion

        #region Projects Manager

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="idProject"></param>
        /// <param name="idParent"></param>
        public void SetVisibilita(string idProject, string idParent)
        {

            string insertStr = "";
            //"INSERT INTO SECURITY (THING, PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO) " +
            //"(SELECT " + idProject + ", PERSONORGROUP, ACCESSRIGHTS, ID_GRUPPO_TRASM, CHA_TIPO_DIRITTO FROM SECURITY WHERE THING=" + idParent + ")";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY_SELECT_FROM_SECURITY");

            q.setParam("param1", idProject);
            q.setParam("param2", idParent);
            insertStr = q.getSQL();

            bool res = this.ExecuteNonQuery(insertStr);

        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="idProject"></param>
        /// <param name="personOrGroup"></param>
        public void SetVisibilitaInsert(string idProject, string personOrGroup)
        {

            string insertStr = "";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY_TIPO_DIRITTO_P");

            q.setParam("param1", idProject);
            q.setParam("param2", personOrGroup);
            q.setParam("param3", personOrGroup);

            insertStr = q.getSQL();

            this.ExecuteLockedNonQuery(insertStr);

        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="idProject"></param>
        /// <param name="personOrGroup"></param>
        public void SetVisibilitaDelete(string idProject, string personOrGroup)
        {

            string deleteStr = "";
            DocsPaUtils.Query qDelete = DocsPaUtils.InitQuery.getInstance().getQuery("D_SECURITY_PERSONGROUP");

            qDelete.setParam("param1", idProject);
            qDelete.setParam("param2", personOrGroup);

            deleteStr = qDelete.getSQL();

            this.ExecuteLockedNonQuery(deleteStr);
        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tipoProj"></param>
        /// <param name="listaID"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public System.Collections.ArrayList GetChildren(string tipoProj, System.Collections.ArrayList listaID)
        {

            System.Collections.ArrayList listaIdFascicoli = new System.Collections.ArrayList();
            string queryStr;
            DataSet dataSet;

            string idProject = (string)listaID[0];
            for (int i = 1; i < listaID.Count; i++)
                idProject += "," + (string)listaID[i];
            if (tipoProj.Equals("T"))
            {
                //queryStr = "" ;
                //    "SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ='F' AND ID_PARENT IN (" + idProject + ")";

                DocsPaUtils.Query qParent = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project6");
                qParent.setParam("param1", idProject);

                queryStr = qParent.getSQL();

                /*
                System.Data.IDataReader dr;
                dr = db.executeReader(queryStr);
                while(dr.Read()) 
                {
                    listaID.Add(dr.GetValue(0).ToString());
                    listaIdFascicoli.Add(dr.GetValue(0).ToString());
                }
                dr.Close();
                */

                this.ExecuteQuery(out dataSet, queryStr);

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    listaID.Add(row[0].ToString());
                    listaIdFascicoli.Add(row[0].ToString());
                }
            }
            else if (tipoProj.Equals("F"))
            {
                listaIdFascicoli = listaID;
            }

            string idFascicoli = (string)listaIdFascicoli[0];

            for (int i = 1; i < listaIdFascicoli.Count; i++)
                idFascicoli += "," + (string)listaIdFascicoli[i];
            queryStr = "";
            //"SELECT SYSTEM_ID FROM PROJECT WHERE CHA_TIPO_PROJ='C' AND ID_FASCICOLO IN (" + idFascicoli + ")";

            DocsPaUtils.Query qFascicolo = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project7");
            qFascicolo.setParam("param1", idFascicoli);

            queryStr = qFascicolo.getSQL();

            /*
            dr = db.executeReader(queryStr);
            while(dr.Read())
                listaID.Add(dr.GetValue(0).ToString());
            dr.Close();
            */

            this.ExecuteQuery(out dataSet, queryStr);

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                listaID.Add(row[0].ToString());
            }

            dataSet.Dispose();

            return listaID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="idProject"></param>
        public void GetVisibilita(out System.Data.DataSet ds, string idProject)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__SECURITY__TIPO_RUOLO");
            q.setParam("param1", idProject);
            string queryStringRuoli = q.getSQL();
            logger.Debug("Query visibilità di un fascicolo - RUOLI: " + queryStringRuoli);
            this.ExecuteQuery(out ds, "DIRITTI_RUOLI", queryStringRuoli);

        }

        public void GetVisibilita_rimossi(out System.Data.DataSet ds, string idProject)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__DELETED_SECURITY__TIPO_RUOLO_FASC");
            q.setParam("param1", idProject);
            string sql = q.getSQL();
            logger.Debug("Query visibilità di un fascicolo - RUOLI RIMOSSI: " + sql);
            this.ExecuteQuery(out ds, "DIRITTI_RUOLI_RIMOSSI", sql);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="idProject"></param>
        public void GetVisibilita1(out System.Data.DataSet ds, string idProject)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLE__SECURITY2");
            q.setParam("param1", idProject);
            string queryStringUtenti = q.getSQL();
            logger.Debug("Query visibilità fascicolo - UTENTI: " + queryStringUtenti);
            this.ExecuteQuery(out ds, "DIRITTI_UTENTI", queryStringUtenti);

        }

        public void GetVisibilita_UtentiRimossi(out System.Data.DataSet ds, string idProject)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_DPA_CORR_GLOBALI__PEOPLE__DELETED_SECURITY_UTENTI");
            q.setParam("param1", idProject);
            string queryStringUtenti = q.getSQL();
            logger.Debug("Query visibilità fascicolo - UTENTI RIMOSSI: " + queryStringUtenti);
            this.ExecuteQuery(out ds, "DIRITTI_UTENTI_RIMOSSI", queryStringUtenti);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="infoUtente"></param>
        /// <param name="objRuolo"></param>
        public void SetProjectTrustees(string idPeople, string thing, DocsPaVO.utente.Ruolo objRuolo, string idClassificazione, out System.Collections.ArrayList listaRuoliSup)
        {
            string sqlString = "";
            string idGruppoTrasm = objRuolo.idGruppo;
            /*sqlString = 
                "UPDATE SECURITY SET CHA_TIPO_DIRITTO='P' WHERE THING=" + thing +
                " AND PERSONORGROUP=" + infoUtente.idPeople;*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_SECURITY_DPA_CORR_GLOBALI");
            q.setParam("param1", thing);
            q.setParam("param2", idPeople);
            sqlString = q.getSQL();
            this.ExecuteNonQuery(sqlString);

            sqlString = "";
            /*sqlString = 
                "INSERT INTO SECURITY (THING,PERSONORGROUP,ACCESSRIGHTS,ID_GRUPPO_TRASM,CHA_TIPO_DIRITTO) " +
                "VALUES (" + thing + "," + idGruppoTrasm + ",255," + idGruppoTrasm + ",'P')";*/
            DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY_CHA_TIPO_DIRITTO_P");
            q1.setParam("param1", thing);
            q1.setParam("param2", idGruppoTrasm);
            q1.setParam("param3", idGruppoTrasm);
            sqlString = q1.getSQL();

            //Caso in cui si sta creando un fascicolo con delega
            //if (delegato != null)
            //{
            //    DocsPaUtils.Query qDel = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");
            //    qDel.setParam("param1", thing + "," + delegato.idPeople + "," + 255 + "," + delegato.idGruppo + ",'D'");
            //    string queryStringDel = qDel.getSQL();
            //    logger.Debug(queryStringDel);
            //    this.ExecuteNonQuery(queryStringDel);

            //}

            this.ExecuteNonQuery(sqlString);

            // aggiungo al documento tutti i diritti dati al fascicolo

            DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
            listaRuoliSup = gerarchia.getGerarchiaSup(objRuolo, null, idClassificazione, DocsPaVO.trasmissione.TipoOggetto.FASCICOLO);
            if (listaRuoliSup == null)
            {
                logger.Debug("Errore nella gestione dei Fascicoli (Query - SetProjectTrustees)");
                throw new Exception("Errore in SetProjectTrustees");
            }

            for (int i = 0; i < listaRuoliSup.Count; i++)
            {
                DocsPaVO.utente.Ruolo ruoloSup = (DocsPaVO.utente.Ruolo)listaRuoliSup[i];
                /*sqlString = 
                    "INSERT INTO SECURITY (THING,PERSONORGROUP,ACCESSRIGHTS,ID_GRUPPO_TRASM,CHA_TIPO_DIRITTO) " +
                    "VALUES (" + thing + "," + ruoloSup.idGruppo + ",47," + 
                    idGruppoTrasm + ",'A')";				*/

                DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY_CHA_TIPO_DIRITTO_A");
                q2.setParam("param1", thing);
                q2.setParam("param2", ruoloSup.idGruppo);
                q2.setParam("param3", idGruppoTrasm);
                sqlString = q2.getSQL();
                logger.Debug(sqlString);
                this.ExecuteNonQuery(sqlString);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="infoUtente"></param>
        /// <param name="objRuolo"></param>
        public bool SetProjectTrustees(string idPeople, string thing, DocsPaVO.utente.Ruolo objRuolo, string idClassificazione, string idRegistro, bool isPrivato, out System.Collections.ArrayList listaRuoliSup, DocsPaVO.utente.InfoUtente delegato, bool pubblico = false)
        {
            bool retValue = false;
            listaRuoliSup = new ArrayList();

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.BeginTransaction();

                string sqlString = string.Empty;
                string idGruppoTrasm = objRuolo.idGruppo;

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_SECURITY_DPA_CORR_GLOBALI");
                q.setParam("param1", thing);
                q.setParam("param2", idPeople);
                sqlString = q.getSQL();
                retValue = dbProvider.ExecuteNonQuery(sqlString);

                sqlString = string.Empty;
                if (retValue)
                {
                    DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY_CHA_TIPO_DIRITTO_P");
                    q1.setParam("param1", thing);
                    q1.setParam("param2", idGruppoTrasm);
                    q1.setParam("param3", idGruppoTrasm);
                    sqlString = q1.getSQL();

                    retValue = dbProvider.ExecuteNonQuery(sqlString);
                }
                else
                {
                    retValue = false;
                }
                if (retValue)
                {
                    //Caso in cui si sta creando un fascicolo con delega
                    if (delegato != null)
                    {
                        DocsPaUtils.Query qDel = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");
                        qDel.setParam("param1", thing + "," + delegato.idPeople + "," + 255 + "," + delegato.idGruppo + ",'D', NULL");
                        string queryStringDel = qDel.getSQL();
                        logger.Debug(queryStringDel);
                        this.ExecuteNonQuery(queryStringDel);

                    }

                    DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();

                    if (idRegistro == null || (idRegistro != null && idRegistro.ToString().Equals("")))
                    {
                        //se il registro è null, la visibilità deve essere estesaa tutti i ruoli che vedono il nodo     
                        listaRuoliSup = gerarchia.getGerarchiaSup(objRuolo, null, idClassificazione, DocsPaVO.trasmissione.TipoOggetto.FASCICOLO, dbProvider);
                    }
                    else
                    {
                        //se il registro è diverso da NULL allora la visibilità deve essere estesa solamente ai superiori che vedono sia il nodo e il registro        
                        listaRuoliSup = gerarchia.getGerarchiaSup(objRuolo, idRegistro, idClassificazione, DocsPaVO.trasmissione.TipoOggetto.FASCICOLO, dbProvider);
                    }
                    if (listaRuoliSup == null)
                    {
                        logger.Debug("Errore nella gestione dei Fascicoli (Query - SetProjectTrustees)");
                        throw new Exception("Errore in SetProjectTrustees");
                    }

                    //Se il fascicolo è privato si da la visibilità solo all'utente creatore e al ruolo di appartenenza,
                    //non ai superiori
                    if (!isPrivato)
                    {
                        for (int i = 0; i < listaRuoliSup.Count; i++)
                        {
                            DocsPaVO.utente.Ruolo ruoloSup = (DocsPaVO.utente.Ruolo)listaRuoliSup[i];

                            DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY_CHA_TIPO_DIRITTO_A");
                            q2.setParam("param1", thing);
                            q2.setParam("param2", ruoloSup.idGruppo);
                            q2.setParam("param3", idGruppoTrasm);
                            sqlString = q2.getSQL();
                            logger.Debug(sqlString);
                            if (!dbProvider.ExecuteNonQuery(sqlString))
                            {
                                return false;
                            }
                        }
                    }

                    //Nel caso di fascicolo pubblico dò diritti in lettura al ruolo pubblico definito in amministrazione
                    if (pubblico)
                    {
                        string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(objRuolo.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                        if (!string.IsNullOrEmpty(idRuoloPubblico) && !idRuoloPubblico.Equals("0"))
                        {
                            DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");
                            q1.setParam("param1", thing + "," + idRuoloPubblico + "," + 63 + "," + idRuoloPubblico + ",'A', NULL");
                            string query = q1.getSQL();
                            logger.Debug(query);
                            this.ExecuteNonQuery(query);
                        }
                    }
                }
                else
                {
                    retValue = false;
                }

                if (retValue)
                    dbProvider.CommitTransaction();
                else
                    dbProvider.RollbackTransaction();
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="thing"></param>
        /// <param name="objRuolo"></param>
        /// <param name="idClassificazione"></param>
        /// <param name="isFolder"></param>
        public void SetProjectTrustees(string idPeople, string thing, DocsPaVO.utente.Ruolo objRuolo, string idClassificazione, bool isFolder, out System.Collections.ArrayList listaRuoliSup, DocsPaVO.utente.InfoUtente delegato)
        {
            string sqlString = "";
            string idGruppoTrasm = objRuolo.idGruppo;
            /*sqlString = 
                "UPDATE SECURITY SET CHA_TIPO_DIRITTO='P' WHERE THING=" + thing +
                " AND PERSONORGROUP=" + infoUtente.idPeople;*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_SECURITY_DPA_CORR_GLOBALI");
            q.setParam("param1", thing);
            q.setParam("param2", idPeople);
            sqlString = q.getSQL();
            this.ExecuteNonQuery(sqlString);

            sqlString = "";
            DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY_CHA_TIPO_DIRITTO_P");
            q1.setParam("param1", thing);
            q1.setParam("param2", idGruppoTrasm);
            q1.setParam("param3", idGruppoTrasm);
            sqlString = q1.getSQL();

            this.ExecuteNonQuery(sqlString);

            //Caso in cui si sta creando un fascicolo con delega
            if (delegato != null)
            {
                DocsPaUtils.Query qDel = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");
                qDel.setParam("param1", thing + "," + delegato.idPeople + "," + 255 + "," + delegato.idGruppo + ",'D', NULL");
                string queryStringDel = qDel.getSQL();
                logger.Debug(queryStringDel);
                this.ExecuteNonQuery(queryStringDel);

            }

            //si estende la visibilità ai superiori
            DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
            listaRuoliSup = gerarchia.getGerarchiaSup(objRuolo, null, idClassificazione, DocsPaVO.trasmissione.TipoOggetto.FASCICOLO);
            if (listaRuoliSup == null)
            {
                logger.Debug("Errore nella gestione dei Fascicoli (Query - SetProjectTrustees)");
                throw new Exception("Errore in SetProjectTrustees");
            }
            for (int i = 0; i < listaRuoliSup.Count; i++)
            {
                DocsPaVO.utente.Ruolo ruoloSup = (DocsPaVO.utente.Ruolo)listaRuoliSup[i];
                DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY_CHA_TIPO_DIRITTO_A");
                q2.setParam("param1", thing);
                q2.setParam("param2", ruoloSup.idGruppo);
                q2.setParam("param3", idGruppoTrasm);
                sqlString = q2.getSQL();
                logger.Debug(sqlString);
                this.ExecuteNonQuery(sqlString);
            }

            /* se si sta creando una Folder, allora si da la visibilità di essa a 
            tutti coloro che vedono il fascicolo in cui essa è contenuta 
            (il tipo diritto è F ovvero TRASMESSO IN FASCICOLO)*/
            if (isFolder)
            {
                string insert_security = "";
                DocsPaUtils.Query qInsert = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY_SELECT_PROJECT");

                qInsert.setParam("param1", thing);
                qInsert.setParam("param2", idGruppoTrasm);
                qInsert.setParam("param3", idClassificazione);
                qInsert.setParam("param4", thing);

                insert_security = qInsert.getSQL();
                this.ExecuteNonQuery(insert_security);
            }
        }

        /// <summary>
        /// Setta la visibilità nella security relativamente alla Folder/Root, nella transazione corrente
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="thing"></param>
        /// <param name="objRuolo"></param>
        /// <param name="idClassificazione"></param>
        /// <param name="isFolder"></param>
        /// <param name="dbProvider">Oggetto Db Provider per la gestione della transazione</param>
        public bool SetProjectTrustees(string idPeople, string thing, DocsPaVO.utente.Ruolo objRuolo, string idClassificazione, bool isFolder, string idRegistro, bool isPrivato, out System.Collections.ArrayList listaRuoliSup, DocsPaVO.utente.InfoUtente delegato, bool pubblico = false)
        {
            bool retValue = false;
            listaRuoliSup = new ArrayList();

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.BeginTransaction();

                string sqlString = string.Empty;
                string idGruppoTrasm = objRuolo.idGruppo;

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_SECURITY_DPA_CORR_GLOBALI");
                q.setParam("param1", thing);
                q.setParam("param2", idPeople);
                sqlString = q.getSQL();
                retValue = dbProvider.ExecuteNonQuery(sqlString);

                if (retValue)
                {
                    sqlString = "";
                    DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY_CHA_TIPO_DIRITTO_P");
                    q1.setParam("param1", thing);
                    q1.setParam("param2", idGruppoTrasm);
                    q1.setParam("param3", idGruppoTrasm);
                    sqlString = q1.getSQL();

                    retValue = dbProvider.ExecuteNonQuery(sqlString);

                    if (retValue)
                    {
                        //Caso in cui si sta creando un fascicolo con delega
                        if (delegato != null)
                        {
                            DocsPaUtils.Query qDel = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");
                            qDel.setParam("param1", thing + "," + delegato.idPeople + "," + 255 + "," + delegato.idGruppo + ",'D', NULL");
                            string queryStringDel = qDel.getSQL();
                            logger.Debug(queryStringDel);
                            this.ExecuteNonQuery(queryStringDel);

                        }

                        //si estende la visibilità ai superiori
                        DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();

                        if (idRegistro == null || (idRegistro != null && idRegistro.ToString().Equals("")))
                        {
                            //se il registro è null, la visibilità deve essere estesaa tutti i ruoli che vedono il nodo
                            listaRuoliSup = gerarchia.getGerarchiaSup(objRuolo, null, idClassificazione, DocsPaVO.trasmissione.TipoOggetto.FASCICOLO, dbProvider);
                        }
                        else
                        {
                            //se il registro è diverso da NULL allora la visibilità deve essere estesa solamente ai superiori che vedono sia il nodo e il registro    
                            listaRuoliSup = gerarchia.getGerarchiaSup(objRuolo, idRegistro, idClassificazione, DocsPaVO.trasmissione.TipoOggetto.FASCICOLO, dbProvider);
                        }

                        if (listaRuoliSup == null)
                        {
                            logger.Debug("Errore nella gestione dei Fascicoli (Query - SetProjectTrustees)");
                            throw new Exception("Errore in SetProjectTrustees");
                        }

                        //Se il fascicolo è privato si da la visibilità solo all'utente creatore e al ruolo di appartenenza,
                        //non ai superiori
                        if (!isPrivato)
                        {
                            for (int i = 0; i < listaRuoliSup.Count; i++)
                            {
                                DocsPaVO.utente.Ruolo ruoloSup = (DocsPaVO.utente.Ruolo)listaRuoliSup[i];
                                DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY_CHA_TIPO_DIRITTO_A");
                                q2.setParam("param1", thing);
                                q2.setParam("param2", ruoloSup.idGruppo);
                                q2.setParam("param3", idGruppoTrasm);
                                sqlString = q2.getSQL();
                                logger.Debug(sqlString);
                                if (!dbProvider.ExecuteNonQuery(sqlString))
                                {
                                    return false;
                                }
                            }
                        }

                        //Nel caso di fascicolo pubblico dò diritti in lettura al ruolo pubblico definito in amministrazione
                        if (pubblico)
                        {
                            string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(objRuolo.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                            if (!string.IsNullOrEmpty(idRuoloPubblico) && !idRuoloPubblico.Equals("0"))
                            {
                                DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");
                                q2.setParam("param1", thing + "," + idRuoloPubblico + "," + 63 + "," + idRuoloPubblico + ",'A', NULL");
                                string query = q2.getSQL();
                                logger.Debug(query);
                                this.ExecuteNonQuery(query);
                            }
                        }

                        /* se si sta creando una Folder, allora si da la visibilità di essa a 
                        tutti coloro che vedono il fascicolo in cui essa è contenuta 
                        (il tipo diritto è F ovvero TRASMESSO IN FASCICOLO)*/
                        if (isFolder)
                        {
                            string insert_security = "";
                            DocsPaUtils.Query qInsert = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY_SELECT_PROJECT");

                            qInsert.setParam("param1", thing);
                            qInsert.setParam("param2", idGruppoTrasm);
                            qInsert.setParam("param3", idClassificazione);
                            qInsert.setParam("param4", thing);

                            insert_security = qInsert.getSQL();
                            dbProvider.ExecuteNonQuery(insert_security);
                        }
                    }
                }
                else
                {
                    retValue = false;
                }

                if (retValue)
                    dbProvider.CommitTransaction();
                else
                    dbProvider.RollbackTransaction();
            }

            return retValue;
        }

        #endregion

        #region Titolario Manager

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classifica"></param>
        /// <param name="idRegistro"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns>ArrayList o 'null' se si è verificato un errore.</returns>
        public System.Collections.ArrayList GetFigliClassifica(string idGruppo, string idPeople, DocsPaVO.fascicolazione.Classifica classifica, string idRegistro, string idAmm)
        {
            System.Collections.ArrayList lista = new System.Collections.ArrayList();
            //DocsPaWS.Utils.Database db = DocsPaWS.Utils.dbControl.getDatabase();
            try
            {
                //db.openConnection();

                string queryString = "";
                #region codice commentato

                /*"SELECT A.SYSTEM_ID, A.VAR_COD_LIV1, A.VAR_COD_LIV2, A.VAR_COD_LIV3, " +
					"A.VAR_COD_LIV4, A.VAR_COD_LIV5, A.VAR_COD_LIV6, A.VAR_COD_LIV7, " +
					"A.VAR_COD_LIV8, A.DESCRIPTION, A.NUM_LIVELLO, A.VAR_CODICE " + 
					"FROM PROJECT A, SECURITY B  WHERE A.SYSTEM_ID=B.THING AND A.CHA_TIPO_PROJ='T' AND " + 
					"(B.PERSONORGROUP=" + infoUtente.idGruppo + " OR B.PERSONORGROUP=" + infoUtente.idPeople + ")  AND B.ACCESSRIGHTS > 0 " +
					" AND (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO='" + idRegistro + "')";*/
                #endregion
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__SECURITY");
                q.setParam("param1", idGruppo);
                q.setParam("param2", idPeople);
                q.setParam("param3", idRegistro);
                
                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
                q.setParam("idRuoloPubblico", !string.IsNullOrEmpty(idRuoloPubblico) ? idRuoloPubblico : "0");
                
                string par4 = "";
                if (classifica != null && classifica.systemId != null)
                    //queryString += " AND A.ID_PARENT=" + classifica.systemId;
                    par4 = " AND A.ID_PARENT=" + classifica.systemId;
                else
                    //queryString += " AND A.NUM_LIVELLO=1"; 
                    par4 = " AND A.NUM_LIVELLO=1";

                par4 += " AND ID_AMM=" + idAmm;

                q.setParam("param4", par4);

                queryString = q.getSQL();


                logger.Debug(queryString);

                #region codice commentato
                /*
				System.Data.IDataReader dr = db.executeReader(queryString);
				while (dr.Read()) 
				{	
					DocsPaVO.fascicolazione.Classifica c = new DocsPaVO.fascicolazione.Classifica();
					
					c.systemId = dr.GetValue(0).ToString();	
					//int numLivello = Int32.Parse(dr.GetValue(10).ToString());
					//c.codice = dr.GetValue(numLivello).ToString();
					c.codice = dr.GetValue(11).ToString();
					c.descrizione = dr.GetValue(9).ToString();

					lista.Add(c);
				}
				dr.Close();
				*/

                #endregion

                DataSet dataSet;
                this.ExecuteQuery(out dataSet, queryString);

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    DocsPaVO.fascicolazione.Classifica c = new DocsPaVO.fascicolazione.Classifica();

                    c.systemId = row[0].ToString();
                    c.codice = row[4].ToString();
                    c.descrizione = row[2].ToString();

                    lista.Add(c);
                }

                //db.closeConnection();
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                //db.closeConnection();
                //throw new Exception("F_System");

                lista = null;
            }

            return lista;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classifica"></param>
        /// <param name="idRegistro"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns>ArrayList o 'null' se si è verificato un errore.</returns>
        public System.Collections.ArrayList GetFigliClassifica2(string idGruppo, string idPeople, DocsPaVO.fascicolazione.Classifica classifica, string idRegistro, string idAmm, string idTitolario)
        {
            System.Collections.ArrayList lista = new System.Collections.ArrayList();
            try
            {
                string queryString = "";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT__SECURITY_BIS");
                string security = string.Empty;

                bool IS_ARCHIVISTA_DEPOSITO;
                DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
                IS_ARCHIVISTA_DEPOSITO = ut.isUtArchivistaDeposito(idPeople, idGruppo);
                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
                if (IS_ARCHIVISTA_DEPOSITO)
                    security = " (checkSecurity(A.SYSTEM_ID, @param2@, @param1@, @idRuoloPubblico@,'F') > 0)";
                else
                {
                    if (IndexSecurity())
                        security = " EXISTS (select /*+INDEX(e) */  'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @param2@, @param1@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                    else
                        security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @param2@, @param1@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                }

                q.setParam("security", security);
                q.setParam("param1", idGruppo);
                q.setParam("param2", idPeople);
                q.setParam("param3", idRegistro);
                q.setParam("idRuoloPubblico", !string.IsNullOrEmpty(idRuoloPubblico) ? idRuoloPubblico : "0");

                string par4 = "";
                if (classifica != null && classifica.systemId != null)
                    par4 = " AND A.ID_PARENT=" + classifica.systemId;
                else
                    par4 = " AND A.NUM_LIVELLO=1";

                par4 += " AND ID_AMM=" + idAmm;

                q.setParam("param4", par4);

                if (idTitolario != null && idTitolario != "")
                    q.setParam("param5", " AND A.ID_TITOLARIO in(" + idTitolario + ")");
                else
                    q.setParam("param5", "");

                queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataSet;

                this.ExecuteQuery(out dataSet, queryString);

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    DocsPaVO.fascicolazione.Classifica c = new DocsPaVO.fascicolazione.Classifica();

                    c.systemId = row[0].ToString();
                    c.codice = row[4].ToString();
                    c.descrizione = row[2].ToString();

                    lista.Add(c);
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                lista = null;
            }

            return lista;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idClassificazione"></param>
        /// <param name="codiceClassificazione"></param>
        /// <param name="registro"></param>
        /// <param name="debug"></param>
        /// <returns>Array di oggetti Classifica o 'null' se si è verificato un errore.</returns>
        public DocsPaVO.fascicolazione.Classifica[] GetGerarchia(string idClassificazione, string codiceClassificazione, DocsPaVO.utente.Registro registro, string idAmm)
        {
            logger.Info("BEGIN");
            DocsPaVO.fascicolazione.Classifica[] lista = null;

            try
            {
                int numLivello = 0;
                string idParent = "0";

                string queryString = "";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project1");

                //add per filtro su registro
                if (registro != null)
                {
                    string condRegistro = "";
                    condRegistro = " (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO='" + registro.systemId + "') ";
                    queryString += condRegistro;
                    queryString += " AND ";
                    q.setParam("param1", queryString);
                }
                else
                {
                    q.setParam("param1", " ");
                }
                //end add 

                string partialSQL = "";
                if (idClassificazione != null)
                    partialSQL += "A.SYSTEM_ID=" + idClassificazione;
                else
                    partialSQL += "UPPER(A.VAR_CODICE)='" + codiceClassificazione.ToUpper() + "'";

                partialSQL += " AND ID_AMM=" + idAmm;

                q.setParam("param2", partialSQL);

                queryString = q.getSQL();
                logger.Debug(queryString);

                DataSet dataSet;
                this.ExecuteQuery(out dataSet, queryString);

                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    numLivello = Int32.Parse(row["NUM_LIVELLO"].ToString());
                    lista = new DocsPaVO.fascicolazione.Classifica[numLivello];

                    for (int i = 0; i < numLivello; i++)
                    {
                        lista[i] = new DocsPaVO.fascicolazione.Classifica();
                    }

                    numLivello -= 1;
                    lista[numLivello].systemId = row["SYSTEM_ID"].ToString();
                    lista[numLivello].descrizione = row["DESCRIPTION"].ToString();
                    lista[numLivello].codice = row["VAR_CODICE"].ToString();
                    lista[numLivello].idTitolario = row["ID_TITOLARIO"].ToString();
                    lista[numLivello].bloccaNodiFigli = row["CHA_BLOCCA_FIGLI"].ToString();
                    lista[numLivello].contatoreAttivo = row["CHA_CONTA_PROT_TIT"].ToString();
                    lista[numLivello].numProtoTit = row["NUM_PROT_TIT"].ToString();


                    if (row["CHA_RW"].ToString() != null || row["CHA_RW"].ToString() != "")
                    {
                        if (row["CHA_RW"].Equals("R"))
                        {
                            lista[numLivello].cha_ReadOnly = true;
                        }
                        else
                        {
                            lista[numLivello].cha_ReadOnly = false;
                        }
                    }

                    idParent = row["ID_PARENT"].ToString();
                }

                queryString = "";
                while (!idParent.Equals("0") && numLivello > 0)
                {
                    numLivello -= 1;
                    lista[numLivello].systemId = idParent;

                    DocsPaUtils.Query q1 = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project2");

                    q1.setParam("param1", idParent);
                    queryString = q1.getSQL();

                    this.ExecuteQuery(out dataSet, queryString);

                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        lista[numLivello].descrizione = row["DESCRIPTION"].ToString();
                        lista[numLivello].codice = row["VAR_CODICE"].ToString();
                        lista[numLivello].idTitolario = row["ID_TITOLARIO"].ToString();
                        lista[numLivello].bloccaNodiFigli = row["CHA_BLOCCA_FIGLI"].ToString();
                        lista[numLivello].contatoreAttivo = row["CHA_CONTA_PROT_TIT"].ToString();
                        lista[numLivello].numProtoTit = row["NUM_PROT_TIT"].ToString();
                        idParent = row["ID_PARENT"].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                lista = null;
            }
            logger.Info("END");
            return lista;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="idClassificazione"></param>
        /// <param name="codiceClassificazione"></param>
        /// <param name="registro"></param>
        /// <param name="debug"></param>
        /// <returns>Array di oggetti Classifica o 'null' se si è verificato un errore.</returns>
        public DocsPaVO.fascicolazione.Classifica[] GetGerarchia2(string idClassificazione, string codiceClassificazione, DocsPaVO.utente.Registro registro, string idAmm, string idTitolario)
        {
            DocsPaVO.fascicolazione.Classifica[] lista = null;

            try
            {
                //Preparo ed eseguo la query
                int numLivello = 0;
                string idParent = "0";
                string queryString = "";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project1_BIS");

                if (registro != null)
                {
                    string condRegistro = "";
                    condRegistro = " (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO='" + registro.systemId + "') ";
                    queryString += condRegistro;
                    queryString += " AND ";
                    q.setParam("param1", queryString);
                }
                else
                {
                    q.setParam("param1", " ");
                }

                string partialSQL = "";
                if (idClassificazione != null)
                    partialSQL += "A.SYSTEM_ID=" + idClassificazione;
                else
                    partialSQL += "UPPER(A.VAR_CODICE)='" + codiceClassificazione.ToUpper() + "'";

                partialSQL += " AND ID_AMM=" + idAmm;

                q.setParam("param2", partialSQL);

                if (idTitolario != null && idTitolario != "")
                    q.setParam("param3", " AND A.ID_TITOLARIO in(" + idTitolario + ")");
                else
                    q.setParam("param3", "");

                queryString = q.getSQL();
                logger.Debug(queryString);

                DataSet dataSet;
                this.ExecuteQuery(out dataSet, queryString);

                //Analizzo il risultato della query e preparo la lista da restituire 
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    numLivello = Int32.Parse(row["NUM_LIVELLO"].ToString());
                    lista = new DocsPaVO.fascicolazione.Classifica[numLivello];

                    for (int i = 0; i < numLivello; i++)
                    {
                        lista[i] = new DocsPaVO.fascicolazione.Classifica();
                    }

                    numLivello -= 1;
                    lista[numLivello].systemId = row["SYSTEM_ID"].ToString();
                    lista[numLivello].descrizione = row["DESCRIPTION"].ToString();
                    lista[numLivello].codice = row["VAR_CODICE"].ToString();
                    lista[numLivello].idTitolario = row["ID_TITOLARIO"].ToString();
                    lista[numLivello].bloccaNodiFigli = row["CHA_BLOCCA_FIGLI"].ToString();
                    lista[numLivello].contatoreAttivo = row["CHA_CONTA_PROT_TIT"].ToString();
                    lista[numLivello].numProtoTit = row["NUM_PROT_TIT"].ToString();

                    if (row["CHA_RW"].ToString() != null || row["CHA_RW"].ToString() != "")
                    {
                        if (row["CHA_RW"].Equals("R"))
                        {
                            lista[numLivello].cha_ReadOnly = true;
                        }
                        else
                        {
                            lista[numLivello].cha_ReadOnly = false;
                        }
                    }

                    idParent = row["ID_PARENT"].ToString();
                }

                queryString = "";
                while (!idParent.Equals("0") && numLivello > 0)
                {
                    numLivello -= 1;
                    lista[numLivello].systemId = idParent;
                    DocsPaUtils.Query q1 =
                        DocsPaUtils.InitQuery.getInstance().getQuery("S_Project2");

                    q1.setParam("param1", idParent);
                    queryString = q1.getSQL();

                    this.ExecuteQuery(out dataSet, queryString);

                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        lista[numLivello].descrizione = row["DESCRIPTION"].ToString();
                        lista[numLivello].codice = row["VAR_CODICE"].ToString();
                        lista[numLivello].idTitolario = row["ID_TITOLARIO"].ToString();
                        lista[numLivello].bloccaNodiFigli = row["CHA_BLOCCA_FIGLI"].ToString();
                        lista[numLivello].contatoreAttivo = row["CHA_CONTA_PROT_TIT"].ToString();
                        lista[numLivello].numProtoTit = row["NUM_PROT_TIT"].ToString();
                        idParent = row["ID_PARENT"].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                lista = null;
            }
            return lista;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="codice"></param>
        /// <param name="descrizione"></param>
        /// <param name="codAmm"></param>
        /// <returns></returns>
        public System.Data.DataSet filtroRicTitolarioDocspa(string codice, string descrizione, string note, string indice, string idAmm, string idGruppo, string idRegistro, string idTitolario)
        {
            System.Data.DataSet ds;
            DocsPaUtils.Query q;
            string Sql = "";

            try
            {
                //modifica SABRINA x gestione DEPOSITO
                DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
                if (ut.isUtArchivistaDeposito(null, idGruppo))
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FILTRO_RIC_TIT_DOCSPA_DEPOSITO");
                else
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FILTRO_RIC_TIT_DOCSPA");

                if (codice != null && codice != "")
                    q.setParam("param1", "AND UPPER(A.var_codice) = UPPER('" + codice.Replace("'", "''") + "')");
                if (descrizione != null && descrizione != "")
                    q.setParam("param2", "AND UPPER(A.description) LIKE UPPER('%" + descrizione.Replace("'", "''") + "%')");
                q.setParam("param3", idAmm);
                q.setParam("param4", idGruppo);
                q.setParam("param5", idRegistro);
                q.setParam("param6", idTitolario);

                //Note nodo
                if (note != null && note != "")
                    q.setParam("paramNote", " AND UPPER(A.VAR_NOTE)  LIKE UPPER('%" + note + "%') ");
                else
                    q.setParam("paramNote", "");

                //Indice sistematico
                if (indice != null && indice != "")
                {
                    q.setParam("paramFrom", " ,DPA_ASS_INDX_SIS, DPA_INDX_SIS ");
                    string paramIndice = " AND A.SYSTEM_ID = DPA_ASS_INDX_SIS.ID_PROJECT " +
                                            " AND DPA_ASS_INDX_SIS.ID_INDICE_SIS = DPA_INDX_SIS.SYSTEM_ID " +
                                            " AND UPPER(DPA_INDX_SIS.VOCE_INDICE) LIKE UPPER('%" + indice + "%')";
                    q.setParam("paramIndice", paramIndice);
                }
                else
                {
                    q.setParam("paramFrom", "");
                    q.setParam("paramIndice", "");
                }

                Sql = q.getSQL();
                logger.Debug("filtroRicTitolarioDocsPa - sql: " + Sql);

                if (!this.ExecuteQuery(out ds, "QUERY", Sql))
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message.ToString(), ex);
                ds = null;
            }

            return ds;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="registro"></param>
        /// <param name="codiceClassifica"></param>
        /// <param name="debug"></param>
        /// <returns>ArrayList o 'null' se si e è verificato un errore.</returns>
        public System.Collections.ArrayList GetTitolario(string idAmministrazione, string idGruppo, string idPeople, DocsPaVO.utente.Registro registro, string codiceClassifica, bool getFigli)
        {
            logger.Debug("getTitolario");
            //DocsPaWS.Utils.Database database=DocsPaWS.Utils.dbControl.getDatabase();

            string codiceRegistro = "";
            //database.openConnection();

            System.Data.DataSet dataSet; //= new System.Data.DataSet();
            System.Collections.ArrayList listaObject = new System.Collections.ArrayList();

            try
            {
                string condRegistro = "";
                if (registro != null)
                    condRegistro = " AND (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO='" + registro.systemId + "')";

                //estrae le classificazioni e i fascicoli
                string commandString1 = "";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project3");

                //se sono un amministratore non devo fare la join con la security
                DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
                if (idGruppo != null && !idGruppo.Equals("") && (!ut.isUtArchivistaDeposito(idPeople, idGruppo)))
                    //commandString1 += 
                    q.setParam("param1", ", SECURITY B WHERE A.SYSTEM_ID=B.THING AND B.ACCESSRIGHTS > 0 AND (B.PERSONORGROUP=" + idPeople + " OR B.PERSONORGROUP=" + idGruppo + ") AND ");
                else
                    //commandString1 += " WHERE ";
                    q.setParam("param1", " WHERE ");


                /*commandString1 += 
                    "ID_AMM='"+infoUtente.idAmministrazione+"' AND CHA_TIPO_PROJ='T'" 
                    + condRegistro;*/
                q.setParam("param2", idAmministrazione);
                q.setParam("param3", condRegistro);

                //istanzia la class di personalizzazione 
                string separator = DocsPaDB.Utils.Personalization.getInstance(idAmministrazione).getSeparator();
                bool estraiTitolario = true;

                if (codiceClassifica != null && !codiceClassifica.Equals(""))
                {
                    string varCodLiv1 = GetVarCodLiv1(codiceClassifica, idAmministrazione, registro);
                    if (varCodLiv1 != null && varCodLiv1 != "")
                    {
                        /*OLD if(getFigli)
                        {
                            q.setParam("param4"," AND (UPPER(VAR_CODICE) = '" + codiceClassifica.ToUpper() + "' OR UPPER(VAR_CODICE) LIKE '" + codiceClassifica.ToUpper() + separator + "%')");
                        }
                        else
                        {
                            q.setParam("param4"," AND (UPPER(VAR_CODICE) = '" + codiceClassifica.ToUpper() + "')");
                        }*/
                        if (getFigli)
                        {
                            q.setParam("param4", " AND VAR_COD_LIV1 LIKE '" + varCodLiv1 + "%'");
                        }
                        else
                        {
                            q.setParam("param4", " AND VAR_COD_LIV1 = '" + varCodLiv1 + "'");
                        }
                    }
                    else
                    {
                        estraiTitolario = false;
                    }
                }
                else
                {
                    q.setParam("param4", "");
                }

                q.setParam("param5", "");
                q.setParam("param6", " AND A.id_titolario = (select system_id from project where cha_stato= 'A' and var_codice = 'T' and ID_AMM = " + idAmministrazione + ") ");

                //commandString1 += " ORDER BY NUM_LIVELLO, VAR_COD_LIV1, VAR_COD_LIV2, VAR_COD_LIV3, VAR_COD_LIV4, VAR_COD_LIV5, VAR_COD_LIV6, VAR_COD_LIV7, VAR_COD_LIV8";
                //logger.Debug(commandString1);
                if (estraiTitolario)
                {
                    commandString1 = q.getSQL();
                    logger.Debug(commandString1);

                    //database.fillTable(commandString1,dataSet,"CLASSIFICAZIONI");

                    this.ExecuteQuery(out dataSet, "CLASSIFICAZIONI", commandString1);
                    //modifica sabrina --- ??? inserito controllo per verificare se ci sono risultati
                    if (dataSet.Tables["CLASSIFICAZIONI"].Rows.Count > 0)
                    {
                        string numLivello = dataSet.Tables["CLASSIFICAZIONI"].Rows[0]["NUM_LIVELLO"].ToString();

                        //si estraggono le classificazioni root
                        System.Data.DataRow[] rootClassRows = dataSet.Tables["CLASSIFICAZIONI"].Select("NUM_LIVELLO=" + numLivello);
                        foreach (System.Data.DataRow dr in rootClassRows)
                        {
                            DocsPaVO.fascicolazione.Classificazione rootClass = new DocsPaVO.fascicolazione.Classificazione();
                            rootClass.systemID = dr["SYSTEM_ID"].ToString();
                            rootClass.descrizione = dr["DESCRIPTION"].ToString();
                            rootClass.codice = dr["VAR_CODICE"].ToString();
                            rootClass.varcodliv1 = dr["VAR_COD_LIV1"].ToString();
                            rootClass.codUltimo = GetCodUltimo(dr["VAR_COD_ULTIMO"].ToString());
                            //Introdotto per tenere traccia dell'id registro del nodo di titolario
                            rootClass.idRegistroNodoTit = dr["ID_REGISTRO"].ToString();

                            //nuovo per popolare il campo descrizione del registro a cui il fascicolo è associato
                            if (rootClass.idRegistroNodoTit != null && rootClass.idRegistroNodoTit != String.Empty)
                            {
                                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                                codiceRegistro = utenti.GetCodiceRegistroBySystemId(rootClass.idRegistroNodoTit);
                                rootClass.codiceRegistroNodoTit = codiceRegistro;

                            }
                            if (registro != null)
                                rootClass.registro = registro;
                            else if (dr["ID_REGISTRO"] != null)
                            {
                                rootClass.registro = GetRegistro(dr["ID_REGISTRO"].ToString());
                            }

                            //ricerca delle classificazioni figlie
                            System.Collections.ArrayList classificazioni = GetClassificazioni(/*database,*/ rootClass.codice, dataSet.Tables["CLASSIFICAZIONI"], rootClass.systemID, registro, separator);
                            for (int k = 0; k < classificazioni.Count; k++)
                            {
                                rootClass.childs.Add(classificazioni[k]);
                            }

                            rootClass.idTipoFascicolo = dr["ID_TIPO_FASC"].ToString();
                            rootClass.bloccaTipoFascicolo = dr["CHA_BLOCCA_FASC"].ToString();
                            listaObject.Add(rootClass);

                        }
                    }
                    dataSet.Dispose();
                }

                //database.closeConnection();
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                //database.closeConnection();
                //throw new Exception("F_System");
                listaObject = null;
            }

            return listaObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="registro"></param>
        /// <param name="codiceClassifica"></param>
        /// <param name="debug"></param>
        /// <returns>ArrayList o 'null' se si e è verificato un errore.</returns>
        public System.Collections.ArrayList GetTitolario2(string idAmministrazione, string idGruppo, string idPeople, DocsPaVO.utente.Registro registro, string codiceClassifica, bool getFigli, string idTitolario)
        {
            logger.Debug("getTitolario");
            //DocsPaWS.Utils.Database database=DocsPaWS.Utils.dbControl.getDatabase();

            string codiceRegistro = "";
            //database.openConnection();

            System.Data.DataSet dataSet; //= new System.Data.DataSet();
            System.Collections.ArrayList listaObject = new System.Collections.ArrayList();

            try
            {
                string condRegistro = "";
                if (registro != null)
                    condRegistro = " AND (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO='" + registro.systemId + "')";

                //estrae le classificazioni e i fascicoli
                string commandString1 = "";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project3");

                //se sono un amministratore non devo fare la join con la security
                //SAB modifica per gestione DEPOSITO 
                DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
                if (idGruppo != null && !idGruppo.Equals("") && (!ut.isUtArchivistaDeposito(idPeople, idGruppo)))
                    q.setParam("param1", ", SECURITY B WHERE A.SYSTEM_ID=B.THING AND B.ACCESSRIGHTS > 0 AND (B.PERSONORGROUP=" + idPeople + " OR B.PERSONORGROUP=" + idGruppo + ") AND ");
                else
                    //commandString1 += " WHERE ";
                    q.setParam("param1", " WHERE ");


                /*commandString1 += 
                    "ID_AMM='"+infoUtente.idAmministrazione+"' AND CHA_TIPO_PROJ='T'" 
                    + condRegistro;*/
                q.setParam("param2", idAmministrazione);
                q.setParam("param3", condRegistro);

                //istanzia la class di personalizzazione 
                string separator = DocsPaDB.Utils.Personalization.getInstance(idAmministrazione).getSeparator();
                bool estraiTitolario = true;

                if (codiceClassifica != null && !codiceClassifica.Equals(""))
                {
                    string varCodLiv1 = GetVarCodLiv1_2(codiceClassifica, idAmministrazione, registro, idTitolario);
                    if (varCodLiv1 != null && varCodLiv1 != "")
                    {
                        /*OLD if(getFigli)
                        {
                            q.setParam("param4"," AND (UPPER(VAR_CODICE) = '" + codiceClassifica.ToUpper() + "' OR UPPER(VAR_CODICE) LIKE '" + codiceClassifica.ToUpper() + separator + "%')");
                        }
                        else
                        {
                            q.setParam("param4"," AND (UPPER(VAR_CODICE) = '" + codiceClassifica.ToUpper() + "')");
                        }*/
                        if (getFigli)
                        {
                            q.setParam("param4", " AND VAR_COD_LIV1 LIKE '" + varCodLiv1 + "%'");
                        }
                        else
                        {
                            q.setParam("param4", " AND VAR_COD_LIV1 = '" + varCodLiv1 + "'");
                        }
                    }
                    else
                    {
                        estraiTitolario = false;
                    }
                }
                else
                {
                    q.setParam("param4", "");
                }

                if (idTitolario != null && idTitolario != "")
                    q.setParam("param5", " AND A.ID_TITOLARIO in(" + idTitolario + ")");
                else
                    q.setParam("param5", "");

                q.setParam("param6", "");

                //commandString1 += " ORDER BY NUM_LIVELLO, VAR_COD_LIV1, VAR_COD_LIV2, VAR_COD_LIV3, VAR_COD_LIV4, VAR_COD_LIV5, VAR_COD_LIV6, VAR_COD_LIV7, VAR_COD_LIV8";
                //logger.Debug(commandString1);
                if (estraiTitolario)
                {
                    commandString1 = q.getSQL();
                    logger.Debug(commandString1);

                    //database.fillTable(commandString1,dataSet,"CLASSIFICAZIONI");

                    this.ExecuteQuery(out dataSet, "CLASSIFICAZIONI", commandString1);
                    //modifica sabrina --- ??? inserito controllo per verificare se ci sono risultati
                    if (dataSet.Tables["CLASSIFICAZIONI"].Rows.Count > 0)
                    {
                        string numLivello = dataSet.Tables["CLASSIFICAZIONI"].Rows[0]["NUM_LIVELLO"].ToString();

                        //si estraggono le classificazioni root
                        System.Data.DataRow[] rootClassRows = dataSet.Tables["CLASSIFICAZIONI"].Select("NUM_LIVELLO=" + numLivello);
                        foreach (System.Data.DataRow dr in rootClassRows)
                        {
                            DocsPaVO.fascicolazione.Classificazione rootClass = new DocsPaVO.fascicolazione.Classificazione();
                            rootClass.systemID = dr["SYSTEM_ID"].ToString();
                            rootClass.descrizione = dr["DESCRIPTION"].ToString();
                            rootClass.codice = dr["VAR_CODICE"].ToString();
                            rootClass.varcodliv1 = dr["VAR_COD_LIV1"].ToString();
                            rootClass.codUltimo = GetCodUltimo(dr["VAR_COD_ULTIMO"].ToString());
                            //Introdotto per tenere traccia dell'id registro del nodo di titolario
                            rootClass.idRegistroNodoTit = dr["ID_REGISTRO"].ToString();

                            //nuovo per popolare il campo descrizione del registro a cui il fascicolo è associato
                            if (rootClass.idRegistroNodoTit != null && rootClass.idRegistroNodoTit != String.Empty)
                            {
                                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                                codiceRegistro = utenti.GetCodiceRegistroBySystemId(rootClass.idRegistroNodoTit);
                                rootClass.codiceRegistroNodoTit = codiceRegistro;

                            }
                            if (registro != null)
                                rootClass.registro = registro;
                            else if (dr["ID_REGISTRO"] != null)
                            {
                                rootClass.registro = GetRegistro(dr["ID_REGISTRO"].ToString());
                            }

                            //ricerca delle classificazioni figlie
                            System.Collections.ArrayList classificazioni = GetClassificazioni(/*database,*/ rootClass.codice, dataSet.Tables["CLASSIFICAZIONI"], rootClass.systemID, registro, separator);
                            for (int k = 0; k < classificazioni.Count; k++)
                            {
                                rootClass.childs.Add(classificazioni[k]);
                            }

                            rootClass.idTipoFascicolo = dr["ID_TIPO_FASC"].ToString();
                            rootClass.bloccaTipoFascicolo = dr["CHA_BLOCCA_FASC"].ToString();
                            listaObject.Add(rootClass);

                        }
                    }
                    dataSet.Dispose();
                }

                //database.closeConnection();
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                //database.closeConnection();
                //throw new Exception("F_System");
                listaObject = null;
            }

            return listaObject;
        }

        /// <summary>
        /// </summary>
        /// <param name="codice"></param>
        /// <returns>un valore stringa o stringa vuota se in eccezione</returns>
        internal string GetCodUltimo(string codice)
        {
            string result = "1";

            if (codice != null && codice != "")
            {
                try
                {
                    int numCodice = Int32.Parse(codice) + 1;
                    result = numCodice.ToString();
                }
                catch (Exception)
                {
                    result = "";
                }
            }

            return result;
        }

        /// <summary>
        /// Reperimento primo codice fascicolo disponibile
        /// per un id titolario
        /// </summary>
        /// <param name="systemIdTitolario"></param>
        /// <returns></returns>
        public string GetCodiceFascicoloDispPerTitolario(string systemIdTitolario)
        {
            string codFasc = string.Empty;
            string sqlCommand = "SELECT MAX (VAR_COD_ULTIMO) + 1 " +
                              "FROM PROJECT A " +
                              "WHERE system_id='" + systemIdTitolario + "'";

            this.ExecuteScalar(out codFasc, sqlCommand);

            return codFasc;
        }

        /// <summary>
        /// </summary>
        /// <param name="db"></param>
        /// <param name="idRegistro"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        private DocsPaVO.utente.Registro GetRegistro(string idRegistro)
        {
            DocsPaVO.utente.Registro reg = null;
            if (!(idRegistro != null && !idRegistro.Equals("")))
            {
                return reg;
            }
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            utenti.GetRegistro(idRegistro, ref reg);
            #region codice commentato
            /*
			System.Data.IDataReader dr = db.executeReader(queryString);
															   
			if(dr.Read())
			{
				reg = new DocsPaVO.utente.Registro();

				reg.systemId = dr.GetValue(0).ToString();
				reg.codRegistro = dr.GetValue(1).ToString();
				reg.codice = dr.GetValue(2).ToString();
				reg.descrizione = dr.GetValue(3).ToString();
				reg.email = dr.GetValue(4).ToString();
				reg.stato = dr.GetValue(5).ToString();				
				reg.idAmministrazione = dr.GetValue(6).ToString();
				reg.codAmministrazione = DocsPaWS.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
				reg.dataApertura = dr.GetValue(7).ToString();
				reg.dataChiusura = dr.GetValue(8).ToString();
				reg.dataUltimoProtocollo = dr.GetValue(9).ToString();
			}
			dr.Close();
			*/

            /*DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            foreach(DataRow row in dataSet.Tables[0].Rows)
            {
                reg = new DocsPaVO.utente.Registro();

                reg.systemId = row[0].ToString();
                reg.codRegistro = row[1].ToString();
                reg.codice = row[2].ToString();
                reg.descrizione = row[3].ToString();
                reg.email = row[4].ToString();
                reg.stato = row[5].ToString();				
                reg.idAmministrazione = row[6].ToString();
                reg.codAmministrazione = DocsPaDB.Utils.Personalization.getInstance(reg.idAmministrazione).getCodiceAmministrazione();
                reg.dataApertura = row[7].ToString();
                reg.dataChiusura = row[8].ToString();
                reg.dataUltimoProtocollo = row[9].ToString();
            }*/
            #endregion

            return reg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codice_parent"></param>
        /// <param name="table"></param>
        /// <param name="parent_id"></param>
        /// <param name="registro"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        private System.Collections.ArrayList GetClassificazioni(string codice_parent, System.Data.DataTable table, string parent_id, DocsPaVO.utente.Registro registro, string separator)
        {
            /* Non inserire output su file di log o su debug perche' la procedura e' 
             * ricorsiva ed ogni informazione scritta rende meno leggibile il log.
             */
            System.Data.DataRow[] classificazioniRows = table.Select("ID_PARENT=" + parent_id);
            System.Collections.ArrayList classificazioni = new System.Collections.ArrayList();

            foreach (DataRow dr in classificazioniRows)
            {
                DocsPaVO.fascicolazione.Classificazione classificazione = new DocsPaVO.fascicolazione.Classificazione();
                classificazione.codice = dr["VAR_CODICE"].ToString();
                classificazione.descrizione = dr["DESCRIPTION"].ToString();
                classificazione.systemID = dr["SYSTEM_ID"].ToString();
                classificazione.codUltimo = GetCodUltimo(dr["VAR_COD_ULTIMO"].ToString());

                if (registro != null)
                {
                    classificazione.registro = registro;
                }
                else if (dr["ID_REGISTRO"] != null)
                {
                    classificazione.registro = GetRegistro(dr["ID_REGISTRO"].ToString());
                }

                System.Collections.ArrayList classChildren = GetClassificazioni(classificazione.codice, table, dr["SYSTEM_ID"].ToString(), registro, separator);

                for (int j = 0; j < classChildren.Count; j++)
                {
                    classificazione.childs.Add(classChildren[j]);
                }

                classificazioni.Add(classificazione);
            }

            return classificazioni;
        }

        public string GetVarCodLiv1(string codClassifica, string idAmm, DocsPaVO.utente.Registro registro)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_VAR_COD_LIV1");
            q.setParam("param1", idAmm);
            q.setParam("param2", "upper(VAR_CODICE)='" + codClassifica.ToUpper() + "'");
            if (registro != null)
            {
                q.setParam("param3", "AND (ID_REGISTRO IS NULL OR ID_REGISTRO = '" + registro.systemId + "')");
            }
            else
            {
                q.setParam("param3", "");
            }
            string myString = q.getSQL();
            string res;
            this.ExecuteScalar(out res, myString);
            return res;
        }

        public string GetVarCodLiv1_2(string codClassifica, string idAmm, DocsPaVO.utente.Registro registro, string idTitolario)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_VAR_COD_LIV1_BIS");
            q.setParam("param1", idAmm);
            q.setParam("param2", "upper(VAR_CODICE)='" + codClassifica.ToUpper() + "'");
            if (registro != null)
            {
                q.setParam("param3", "AND (ID_REGISTRO IS NULL OR ID_REGISTRO = '" + registro.systemId + "')");
            }
            else
            {
                q.setParam("param3", "");
            }

            if (idTitolario != null && idTitolario != "")
                q.setParam("param4", " AND ID_TITOLARIO in(" + idTitolario + ")");
            else
                q.setParam("param4", "");

            string myString = q.getSQL();
            string res;
            logger.Debug("GET_VAR_COD_LIV1: " + myString);
            this.ExecuteScalar(out res, myString);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personOrGroup"></param>
        /// <param name="thing"></param>
        /// <returns></returns>
        public int GetSecurityCount(string personOrGroup, string thing)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Security");
            q.setParam("param1", personOrGroup);
            q.setParam("param2", thing);

            string res;
            this.ExecuteScalar(out res, q.getSQL());

            return Int32.Parse(res);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public bool ExistingCode(DocsPaVO.fascicolazione.Classificazione nodoTitolario, string idAmministrazione)
        {
            #region codice commentato
            //string queryStr ="";
            /*"SELECT COUNT(*) FROM PROJECT WHERE CHA_TIPO_PROJ = 'T' " +
                "AND VAR_CODICE = '" + nodoTitolario.codice + 
                "' AND ID_AMM = " + infoUtente.idAmministrazione;
                */
            //if(!db.executeScalar(queryStr).ToString().Equals("0"))
            //	throw new Exception("Il codice scelto è già presente");

            #endregion
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project5");
            q.setParam("param1", nodoTitolario.codice);
            q.setParam("param2", idAmministrazione);
            if (nodoTitolario.registro != null && nodoTitolario.registro.systemId != null && !nodoTitolario.registro.Equals("") && !nodoTitolario.registro.systemId.Equals(""))
            {
                q.setParam("param3", nodoTitolario.registro.systemId);
            }
            else
            {
                q.setParam("param3", "''");
            }
            string res = "";

            string command = q.getSQL();
            this.ExecuteScalar(out res, command);

            if (res.Equals("0"))
                return true;
            else
                return false;
        }
        /// <summary>
        /// </summary>
        /// <param name="db"></param>
        /// <param name="idParent"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public void GetProjectList(string idParent, ref string codParent, ref int numLiv)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project4");
            q.setParam("param1", idParent);
            string queryString = q.getSQL();
            #region codice commentato
            /*
			db.openConnection();
			System.Data.IDataReader idr = db.executeReader(queryString);
			db.closeConnection();

			if (dr.Read()) 
			{
				codParent = dr.GetValue(0).ToString();
				object livello = dr.GetValue(1);
					
				if (livello != null)
				{
					numLiv = Int32.Parse(livello.ToString()) + 1;
				}
				else 
				{
					numLiv = 1;
				}
			}
			
			dr.Close();
			*/
            #endregion
            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                DataRow row = dataSet.Tables[0].Rows[0];
                codParent = row[0].ToString();

                if (row[1] != null)
                {
                    numLiv = Int32.Parse(row[1].ToString()) + 1;
                }
                else
                {
                    numLiv = 1;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idParent"></param>
        /// <param name="numLivello"></param>
        /// <param name="codice"></param>
        /// <param name="idAmministrazione"></param>
        /// <param name="idRegistro"></param>
        /// <param name="liv"></param>
        /// <param name="systemID"></param>
        public void UpdateProject(string idParent, int numLivello, string codice, string idAmministrazione,
            string idRegistro, string systemID)
        {
            DocsPaUtils.Query q =
                DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_CHA_TIPO_PROJ_T_WHERE_SYSTEMID");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.GetDate());
            q.setParam("param2", idParent);
            q.setParam("param3", numLivello.ToString());
            q.setParam("param4", codice);
            q.setParam("param5", idAmministrazione);
            q.setParam("param6", idRegistro);


            string updateString = "";
            //			for(int i=1; i <= numLivello ; i++)
            //				updateString += ", VAR_COD_LIV" + i.ToString() + "='" + liv[i-1] + "'";
            //			
            if (updateString == "")
                q.setParam("param7", "");
            else
                q.setParam("param7", updateString);

            q.setParam("param8", systemID);
            string queryString = q.getSQL();
            logger.Debug(queryString);
            this.ExecuteNonQuery(queryString);


        }


        public bool CheckProject(string SystemId)
        {
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("CHECK_PROJECT");
                q.setParam("param1", SystemId);
                string queryStr = q.getSQL();
                string outRes;
                logger.Debug(queryStr);
                this.ExecuteScalar(out outRes, queryStr);
                if (!outRes.Equals("0")) result = true;
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public string CheckCodice(/*DocsPaWS.Utils.Database db,*/string idAmministrazione,
            string codiceParent, string nextVal)
        {
            /*string queryStr =
                "SELECT COUNT(*) FROM PROJECT WHERE CHA_TIPO_PROJ = 'T' " +
                "AND VAR_CODICE = '" + ret + 
                "' AND ID_AMM = " + infoUtente.idAmministrazione;
            logger.Debug(queryStr);*/
            string ret = "";
            if (codiceParent != null)
                ret = codiceParent;
            ret += nextVal;

            DocsPaUtils.Query q =
                DocsPaUtils.InitQuery.getInstance().getQuery("S_Project11");
            q.setParam("param1", ret);
            q.setParam("param2", idAmministrazione);
            string queryStr = q.getSQL();
            /*if(!db.executeScalar(queryStr).ToString().Equals("0"))		
                ret = null;*/
            string outRes;
            this.ExecuteScalar(out outRes, queryStr);
            if (!outRes.Equals("0"))
                ret = null;

            return ret;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <returns></returns>
        public string GetCodiceFiglioTitolario(string idAmministrazione)
        {
            string queryStr = "";
            /*	"SELECT COUNT(*) FROM PROJECT WHERE CHA_TIPO_PROJ = 'T' " +
                "AND (NUM_LIVELLO = 0 OR NUM_LIVELLO IS NULL) AND ID_AMM = " + infoUtente.idAmministrazione;*/
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project12");
            q.setParam("param1", idAmministrazione);
            queryStr = q.getSQL();
            //nextVal = Int32.Parse(db.executeScalar(queryStr).ToString()) + 1;
            string ret = "";
            this.ExecuteScalar(out ret, queryStr);
            return ret;
        }

        public bool GetCountSottofascDoc(string project_Id, out string nFasc)
        {
            bool result = false;
            nFasc = "";
            try
            {
                //la query estrae il numero di sottocartelle figlie di quella data
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project13");
                q.setParam("param1", project_Id);
                string queryStr = q.getSQL();
                logger.Debug("GetCountSottofascDoc: S_Project13 - param1 = " + project_Id);
                string outRes;
                this.ExecuteScalar(out outRes, queryStr);
                /* Se la prima query ritorna qualcosa, allora non eseguo la seconda query.
                 * Il fascicolo selezionato contiene un sottofascicolo quindi non posso eliminarlo
                 * (importante: il sottofascicolo potrebbe esserci ma non essere visibile all'utente che tenta 
                 * di rimuovere il fascicolo) */
                if (!outRes.Equals("0"))
                {
                    result = false;
                    nFasc = outRes;
                }
                else
                {
                    /* Se il fascicolo selezionato non ha sottofascicoli eseguo la seconda query per vedere
                     * se contiene dei documenti (importenate: anche documenti che io potrei non vedere) */
                    DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponents5");
                    q2.setParam("param1", project_Id);
                    queryStr = q2.getSQL();
                    logger.Debug("GetCountSottofascDoc: S_ProjectComponents5 - param1 = " + project_Id);
                    string res;
                    this.ExecuteScalar(out res, queryStr);
                    //se il fascicolo contiene dei documenti non posso eliminarlo
                    if (!res.Equals("0"))
                    {
                        result = false;
                    }
                    else
                    {
                        /* A questo punto poichè il fascicolo selezionato non contiene nè sottofascicoli
                         * nè documenti è possibile rimuoverlo*/
                        result = true;
                    }

                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante la rimozione di un folder", e);
                result = false;
            }
            return result;
        }

        public bool GetCountfascAndFolderDoc(string project_Id, out string nFasc)
        {
            bool result = false;
            nFasc = "";
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponentsAll");
                q.setParam("idFasc", project_Id);
                string queryStr = q.getSQL();
                logger.Debug("GetCountfascAndFolderDoc: S_ProjectComponentsAll - idFasc = " + project_Id);
                string outRes;
                this.ExecuteScalar(out outRes, queryStr);
                /* Se la prima query ritorna qualcosa, allora non eseguo la seconda query.
                 * Il fascicolo selezionato contiene un sottofascicolo quindi non posso eliminarlo
                 * (importante: il sottofascicolo potrebbe esserci ma non essere visibile all'utente che tenta 
                 * di rimuovere il fascicolo) */
                if (!outRes.Equals("0"))
                {
                    result = false;
                    nFasc = outRes;
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore durante la rimozione di un folder", e);
                result = false;
            }
            return result;
        }


        public string GetProjectID(string codice, string tipo, string idAmm)
        {
            string queryStr = "";
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("SELECT_PROJECT_ID");
            q.setParam("param1", tipo);
            q.setParam("param2", codice);
            q.setParam("param3", idAmm);
            queryStr = q.getSQL();
            string ret = null;
            this.ExecuteScalar(out ret, queryStr);
            return ret;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="idProject"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public int DeleteTitolario(string idProject)
        {
            string queryString = "";
            //"SELECT COUNT(*) FROM PROJECT_COMPONENTS 
            //WHERE PROJECT_ID IN (" + idProject + ")";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponents4");
            q.setParam("param1", idProject);
            queryString = q.getSQL();
            //int ret = Int32.Parse(db.executeScalar(queryString).ToString());
            string ret;
            this.ExecuteScalar(out ret, queryString);

            return Int32.Parse(ret);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idProject"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool DeleteFascicoloAndFigli(string idProject)
        {
            string queryString = "";
            bool retVal = false;
            string ret;

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_SECURITY_PROJECT_AND_CHILD");
                q.setParam("idFasc", idProject);
                string commandText = q.getSQL();
                
                dbProvider.ExecuteNonQuery(commandText);

                DocsPaUtils.Query q2 = DocsPaUtils.InitQuery.getInstance().getQuery("D_PROJECT_AND_CHILD");
                q2.setParam("idFasc", idProject);
                commandText = q2.getSQL();
                //int ret = Int32.Parse(db.executeScalar(queryString).ToString());

                dbProvider.ExecuteNonQuery(commandText);
                dbProvider.CommitTransaction();

                retVal = true;
                }
                catch (Exception ex)
                {
                    logger.Debug("Errore nella creazione del riscontro : " + ex.Message);
                    dbProvider.RollbackTransaction();
                    retVal = false;
                }
                finally
                {
                    dbProvider.Dispose();
                }
            }

            return retVal;
        }

        /// <summary>
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="infoUtente"></param>
        /// <param name="descrizione"></param>
        /// <param name="systemID"></param>
        /// <returns>Classificazione o 'null' se si è verificato un errore.</returns>
        public DocsPaVO.fascicolazione.Classificazione UpdateTitolario(DocsPaVO.fascicolazione.Classificazione nodoTitolario)
        {
            try
            {
                if (nodoTitolario != null && nodoTitolario.systemID != null)
                {
                    if (!(nodoTitolario.descrizione != null && !nodoTitolario.descrizione.Equals("")))
                    {
                        //throw new Exception("Verificare il campo descrizione");
                        logger.Debug("Errore nella gestione dei Fascicoli (Query - UpdateTitolario)");
                        throw new Exception("Verificare il campo descrizione");
                    }

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_DESCRIPTION");
                    q.setParam("param1", nodoTitolario.descrizione.Replace("'", "''"));
                    q.setParam("param2", nodoTitolario.systemID);
                    string updateStr = q.getSQL();
                    this.ExecuteNonQuery(updateStr);
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());

                nodoTitolario = null;
            }

            return nodoTitolario;
        }

        public DocsPaVO.fascicolazione.Fascicolo UpdateFascicolo(DocsPaVO.fascicolazione.Fascicolo nodoFascicolo)
        {
            try
            {
                if (nodoFascicolo != null && nodoFascicolo.systemID != null)
                {
                    if (!(nodoFascicolo.descrizione != null && !nodoFascicolo.descrizione.Equals("")))
                    {
                        //throw new Exception("Verificare il campo descrizione");
                        logger.Debug("Errore nella gestione dei Fascicoli (Query - UpdateFascicolo)");
                        throw new Exception("Verificare il campo descrizione");
                    }

                    if (nodoFascicolo.descrizione.Contains("°"))
                        nodoFascicolo.descrizione.Replace("°", "&ordm;");

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_DESCRIPTION");
                    q.setParam("param1", nodoFascicolo.descrizione.Replace("'", "''"));
                    q.setParam("param2", nodoFascicolo.systemID);
                    string updateStr = q.getSQL();
                    this.ExecuteNonQuery(updateStr);
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());

                nodoFascicolo = null;
            }

            return nodoFascicolo;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="corrispondente"></param>
        /// <returns></returns>
        public string GetIdUtenteRuolo(DocsPaVO.utente.Corrispondente corrispondente)
        {
            if (corrispondente.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
            {
                return ((DocsPaVO.utente.Utente)corrispondente).idPeople;
            }
            else if (corrispondente.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
            {
                return ((DocsPaVO.utente.Ruolo)corrispondente).idGruppo;
            }
            else
            {
                logger.Debug("Errore nella gestione dei Fascicoli (Query - GetIdUtenteRuolo)");
                throw new Exception("Tipo non supportato");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodoTitolario"></param>
        /// <param name="corrRemove"></param>
        public void CheckChildsPermission(DocsPaVO.fascicolazione.Classificazione nodoTitolario, DocsPaVO.utente.Corrispondente[] corrRemove)
        {
            if (corrRemove == null || nodoTitolario.childs == null)
                return;
            if (corrRemove.Length > 0 && nodoTitolario.childs.Count > 0)
            {
                string personOrGroup = "(" + GetIdUtenteRuolo(corrRemove[0]);
                for (int i = 1; i < corrRemove.Length; i++)
                    GetIdUtenteRuolo(corrRemove[i]);
                personOrGroup += ")";
                string thing = "(" + ((DocsPaVO.fascicolazione.Classificazione)nodoTitolario.childs[0]).systemID;

                for (int i = 1; i < nodoTitolario.childs.Count; i++)
                    thing += "," + ((DocsPaVO.fascicolazione.Classificazione)nodoTitolario.childs[i]).systemID;
                thing += ")";
                int securityCount = GetSecurityCount(personOrGroup, thing);
                if (securityCount.ToString().Equals("0"))
                {
                    logger.Debug("Errore nella gestione dei Fascicoli (Query - CheckChildPermission)");
                    throw new Exception("Impossibile rimuovere i ruoli selezionati");
                }
            }

        }


        public void SetVisibilita(string tipoProj, System.Collections.ArrayList listaID, DocsPaVO.utente.Corrispondente[] corrAdd, DocsPaVO.utente.Corrispondente[] corrRemove)
        {
            string personOrGroup;

            if (listaID == null)
            {
                return;
            }

            if (listaID.Count == 0)
            {
                return;
            }

            listaID = GetChildren(tipoProj, listaID);

            string idProject;

            if (corrAdd != null && corrAdd.Length > 0 && listaID.Count > 0)
            {
                for (int j = 0; j < listaID.Count; j++)
                {
                    idProject = (string)listaID[j];

                    for (int i = 0; i < corrAdd.Length; i++)
                    {
                        if (corrAdd[i].GetType().Equals(typeof(DocsPaVO.utente.Utente)))
                        {
                            personOrGroup = ((DocsPaVO.utente.Utente)corrAdd[i]).idPeople;
                        }
                        else if (corrAdd[i].GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
                        {
                            personOrGroup = ((DocsPaVO.utente.Ruolo)corrAdd[i]).idGruppo;
                        }
                        else
                        {
                            logger.Debug("Errore nella gestione dei Fascicoli (Query - SetVisibilita)");
                            throw new Exception("Tipo non supportato");
                        }

                        SetVisibilitaInsert(idProject, personOrGroup);

                    }
                }
            }

            if (corrRemove != null && corrRemove.Length > 0 && listaID.Count > 0)
            {
                idProject = (string)listaID[0];

                for (int i = 1; i < listaID.Count; i++)
                {
                    idProject += "," + (string)listaID[i];
                }

                personOrGroup = "(" + GetIdUtenteRuolo(corrRemove[0]);

                for (int i = 1; i < corrRemove.Length; i++)
                {
                    personOrGroup += "," + GetIdUtenteRuolo(corrRemove[i]);
                }

                personOrGroup += ")";

                SetVisibilitaDelete(idProject, personOrGroup);

            }
        }


        public void SetAutorizzazioniNodoTitolario(DocsPaVO.fascicolazione.Classificazione nodoTitolario, DocsPaVO.utente.Corrispondente[] corrAdd, DocsPaVO.utente.Corrispondente[] corrRemove, bool ereditaDiritti)
        {
            logger.Debug("setAutorizzazioniNodoTitolario");

            if (!ereditaDiritti)
                CheckChildsPermission(nodoTitolario, corrRemove);
            try
            {
                this.BeginTransaction();
                System.Collections.ArrayList listaID = new System.Collections.ArrayList();
                listaID.Add(nodoTitolario.systemID);
                if (nodoTitolario.childs != null)
                {
                    for (int i = 0; i < nodoTitolario.childs.Count; i++)
                        listaID.Add(((DocsPaVO.fascicolazione.Classificazione)nodoTitolario.childs[i]).systemID);
                }
                SetVisibilita("T", listaID, corrAdd, corrRemove);
                this.CommitTransaction();

            }
            catch (Exception e)
            {
                this.RollbackTransaction();
                logger.Debug("Errore nella gestione dei Fascicoli (Query - SetAutorizzazioniNodoTitolario)", e);
                throw new Exception(e.Message);
            }
        }


        #endregion

        #region codice commentato
        /*public void GetDettaglio(string DataApertura,string DataChiusura,string idFascicolo,string idGruppo)
		{
		
			DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROJECT_SECURITY_SHORT");

			q.setParam("param1",DataApertura);
			q.setParam("param2",DataChiusura);
			q.setParam("param3",idFascicolo);
			q.setParam("param4",idGruppo);

			string queryString = q.getSQL();
		}*/


        //		public DocsPaVO.fascicolazione.Folder GetFolder(string idFascicolo, DocsPaVO.utente.InfoUtente infoUtente) 
        //		{
        //			
        //			DocsPaWS.Utils.Database database = DocsPaWS.Utils.dbControl.getDatabase();			
        //			DocsPaVO.fascicolazione.Folder folderObject= new DocsPaVO.fascicolazione.Folder();
        //			try 
        //			{
        //				
        //				//database.openConnection();
        //				
        //				System.Data.DataSet dataSet= new System.Data.DataSet();
        //				
        //				string commandString1=
        //					" SELECT DISTINCT A.* FROM PROJECT A, SECURITY B " +
        //					" WHERE A.SYSTEM_ID=B.THING AND A.ID_FASCICOLO=" + idFascicolo +
        //					" AND (B.PERSONORGROUP=" + infoUtente.idPeople + " OR B.PERSONORGROUP=" + infoUtente.idGruppo + ") AND B.ACCESSRIGHTS > 0";
        //					
        //				
        //				
        //				//database.fillTable(commandString1,dataSet,"FOLDER");
        //
        //				System.Data.DataRow[] folderRootRows = dataSet.Tables["FOLDER"].Select("ID_PARENT=" + idFascicolo);
        //				
        //				//TODO : Togliere il commento dopo aver implementato
        //				//		getFolderData
        //				/*if (folderRootRows.Length > 0) 
        //					folderObject = getFolderData(folderRootRows[0],dataSet.Tables["FOLDER"]);*/
        //				
        //			} 
        //			catch (Exception e) 
        //			{
        //				
        //				
        //				//database.closeConnection();
        //				throw new Exception("F_System");
        //			}
        //			return folderObject;
        //		}

        //riguarda GetTitolario
        //					for(int i=0;i< rootClassRows.Length;i++) 
        //					{
        //						DocsPaVO.fascicolazione.Classificazione rootClass=new DocsPaVO.fascicolazione.Classificazione();
        //						rootClass.systemID = rootClassRows[i]["SYSTEM_ID"].ToString();
        //						rootClass.descrizione= rootClassRows[i]["DESCRIPTION"].ToString();
        //						rootClass.codice = rootClassRows[i]["VAR_CODICE"].ToString();
        //						
        //						rootClass.codUltimo = GetCodUltimo(rootClassRows[i]["VAR_COD_ULTIMO"].ToString());
        //						if(registro != null)
        //							rootClass.registro = registro;
        //						else if(rootClassRows[i]["ID_REGISTRO"] != null)
        //						{
        //														
        //							rootClass.registro = GetRegistro(rootClassRows[i]["ID_REGISTRO"].ToString());
        //							
        //							//RegistriManager.getRegistro(database, rootClassRows[i]["ID_REGISTRO"].ToString());
        //						}		
        //
        //					
        //						//ricerca delle classificazioni figlie
        //						System.Collections.ArrayList classificazioni=GetClassificazioni(/*database,*/ rootClass.codice,dataSet.Tables["CLASSIFICAZIONI"],rootClass.systemID, registro, separator);
        //						for(int k=0;k<classificazioni.Count;k++) 
        //						{
        //							rootClass.childs.Add(classificazioni[k]);
        //						}
        //						listaObject.Add(rootClass);
        //
        //					}
        //				}
        //fine GetTitolario


        //codice riguardante GetClassificazioni
        //	for(int i=0;i<classificazioniRows.Length;i++) 
        //	{
        //	DocsPaVO.fascicolazione.Classificazione classificazione=new DocsPaVO.fascicolazione.Classificazione();
        //	classificazione.codice=classificazioniRows[i]["VAR_CODICE"].ToString();
        //	classificazione.descrizione=classificazioniRows[i]["DESCRIPTION"].ToString();
        //	classificazione.systemID=classificazioniRows[i]["SYSTEM_ID"].ToString();
        //	//classificazione.accessRights = classificazioniRows[i]["ACCESSRIGHTS"].ToString();
        //	classificazione.codUltimo = GetCodUltimo(classificazioniRows[i]["VAR_COD_ULTIMO"].ToString());
        //	if(registro != null)
        //		classificazione.registro = registro;
        //	else if(classificazioniRows[i]["ID_REGISTRO"] != null)
        //		classificazione.registro = GetRegistro(/*db,*/ classificazioniRows[i]["ID_REGISTRO"].ToString());
        //	//
        //	System.Collections.ArrayList classChildren=GetClassificazioni(/*db,*/ classificazione.codice,table,classificazioniRows[i]["SYSTEM_ID"].ToString(), registro, separator);
        //	for(int j=0;j<classChildren.Count;j++)
        //	{
        //		classificazione.childs.Add(classChildren[j]);
        //	}
        //	classificazioni.Add(classificazione);
        //	}
        //fine Get

        #endregion

        /// <summary>
        /// Metodo per avere i dati di un campo della tabella Project (query limitata ad un solo dato)
        /// </summary>
        /// <param name="campo">nome campo della tabella Project</param>
        /// <param name="condizione">condizione WHERE (Es: "WHERE SYSTEM_ID = x"</param>
        /// <returns></returns>
        public string GetProjectData(string campo, string condizione)
        {
            string retValue = null;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");
            q.setParam("param1", campo);
            q.setParam("param2", condizione);
            string queryString = q.getSQL();
            logger.Debug(queryString);

            this.ExecuteScalar(out retValue, queryString);

            return retValue;
        }

        //		public bool creazioneFascicoloConTransazione(DocsPaVO.fascicolazione.Classificazione classificazione, DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, bool enableUffRef)
        //		{
        //			bool result = true;
        //			
        //			DocsPaDB.Query_DocsPAWS.AmministrazioneXml objAX = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
        //			
        //			try 
        //			{						
        //				// Inizio transazione sul database
        //				//this.BeginTransaction();
        //				logger.Debug(" *** INIZIO TRANSAZIONE CREAZIONE FASCICOLO ***");
        //
        //				// 1 - Si calcola il formato del fascicolo
        //				fascicolo.codice = calcolaCodiceFascicolo(infoUtente.idAmministrazione,classificazione.codice, fascicolo.apertura, classificazione.systemID, ref fascicolo.codUltimo, true);
        //			
        //				// 2 - Si verifica che il codice sia univoco
        //				if (!objAX.CheckUniqueCode("PROJECT","VAR_CODICE",fascicolo.codice,"AND ((ID_REGISTRO IS NULL OR ID_REGISTRO="+classificazione.registro.systemId+" ) ) AND ID_AMM =" + infoUtente.idAmministrazione + " AND ID_PARENT = " + classificazione.systemID + ""))
        //				{
        //					// 3 - Si verifica se il codice è già presente viene calcolato il nuovo codice
        //					fascicolo.codice = calcolaCodiceFascicolo(infoUtente.idAmministrazione,classificazione.codice, fascicolo.apertura, classificazione.systemID, ref fascicolo.codUltimo, false);
        //				}
        //				
        //				// 4 - Si aggiorna il record relativo al fascicolo con i nuovi dati
        //				fascicolo = nuovoFascicolo(infoUtente.idAmministrazione, classificazione.registro, classificazione.systemID, classificazione.codice,fascicolo, enableUffRef);
        //
        //				// 5 - Si aggiorna il codUltimo (solo per ora perchè andrà tolta questa gestione)
        //				newFascicolo(fascicolo.codUltimo,fascicolo.idClassificazione);
        //
        //				// 6 - Si estende la visibilità sul fascicolo creato
        //				SetProjectTrustees(infoUtente.idPeople, fascicolo.systemID, ruolo, fascicolo.idClassificazione);
        //
        //				//this.CommitTransaction();
        //				//this.CloseConnection();
        //			}
        //			catch
        //			{
        //				//this.RollbackTransaction();
        //				logger.Debug("*** ESEGUITA ROLLBACK DELLA TRANSAZIONE CREAZIONE FASCICOLO: metodo creazioneFascicoloConTransazione ***");				
        //
        //				result = false;
        //			}
        //			
        //			return result;
        //		}

        /// <summary>
        /// Formatta il codice della fascicolatura a seconda del formato
        /// specificato per la corrente amministrazione
        /// </summary>
        /// <param name="idAmm">Id Amministrazione</param>
        /// <param name="codTitolo">codice classifica</param>
        /// <param name="data">data corrente</param>
        /// <param name="codFascicolo">codice del fascicolo</param>
        /// <returns></returns>

        /// <summary>
        /// Formatta il codice della fascicolatura a seconda del formato
        /// specificato per la corrente amministrazione
        /// </summary>
        /// <param name="idAmm">Id Amministrazione</param>
        /// <param name="codTitolo">codice classifica</param>
        /// <param name="data">data corrente</param>
        /// <param name="codFascicolo">codice del fascicolo</param>
        /// <param name="onlyFormatCode">True se deve essere solamente formattato il codice, false se deve essere
        /// anche calcolato il nuovo codice fascicolo</param>
        /// <returns>Codice formattato</returns>
        private string calcolaCodiceFascicolo(string idAmm, string codTitolo, string data, string systemIdTitolario, ref string codFascicolo, bool onlyFormatCode)
        {
            string format = "";

            if (!onlyFormatCode)
            {
                // caso in cui devo calcolare il codice fascicolo e poi formattarlo

                string sqlCommand = "SELECT MAX (NUM_FASCICOLO) + 1 " +
                    "FROM PROJECT A " +
                    "WHERE CHA_TIPO_PROJ = 'F' AND CHA_TIPO_FASCICOLO = 'P' AND ID_PARENT ='" + systemIdTitolario + "'";

                this.ExecuteScalar(out codFascicolo, sqlCommand);

            }
            logger.Debug("getCodiceFascicolo");
            logger.Debug("codTitolo " + codTitolo);
            logger.Debug("data " + data);
            logger.Debug("codFascicolo " + codFascicolo);

            format = DocsPaDB.Utils.Personalization.getInstance(idAmm).FormatoFascicolatura;
            logger.Debug("format = " + format);

            format = format.Replace("COD_TITOLO", codTitolo);
            format = format.Replace("DATA_COMP", data.Substring(0, 10));
            format = format.Replace("DATA_ANNO", data.Substring(6, 4));
            format = format.Replace("NUM_PROG", codFascicolo);

            return format;
        }

        public bool VisRuoloNodoTitolario(string project_Id, string idGruppo, string idPeople)
        {
            bool result = false;

            try
            {
                //la query estrae il numero di sottocartelle figlie di quella data
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("VIS_RUOLO_NODO_TIT");
                q.setParam("param1", idGruppo);
                q.setParam("param2", idPeople);
                q.setParam("param3", project_Id);

                string queryStr = q.getSQL();
                logger.Debug("VisRuoloNodoTitolario");
                string outRes;
                this.ExecuteScalar(out outRes, queryStr);

                if (outRes.Equals("0"))
                {
                    result = false; // il ruolo/utente non vede il nodo di titolario

                }
                else
                {
                    result = true; // il ruolo vede il titolario
                }

            }
            catch (Exception e)
            {
                logger.Debug("Errore nel metodo: VisRuoloNodoTitolario", e);
                result = false;
            }
            return result;
        }

        public string GetDescLF(string id)
        {
            string descLF = string.Empty;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CORR_GLOB_GENERIC");

            q.setParam("param1", "VAR_DESC_CORR");
            q.setParam("param2", "system_id = " + id);
            string query = q.getSQL();
            this.ExecuteScalar(out descLF, query);

            return descLF;
        }

        public bool IsDocumentoClassificatoInFolder(string idProfile, string idFolder)
        {

            bool result = false;
            int retValue = 0;

            try
            {
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponents");
                queryDef.setParam("param1", idFolder);
                queryDef.setParam("param2", idProfile);

                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                using (DBProvider dbProvider = new DBProvider())
                {
                    string outParam;
                    if (dbProvider.ExecuteScalar(out outParam, commandText))
                        retValue = Convert.ToInt32(outParam);
                }
                if (retValue > 0)
                {
                    result = true; // il doc è già classificato nella folder corrente
                }

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
            }

            return result;
        }


        public string getUserDB()
        {
            return DocsPaDbManagement.Functions.Functions.GetDbUserSession();
        }

        #region ACL
        public bool EditingFascACL(out string descrizione, DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto, string pOrG, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            descrizione = "";
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.BeginTransaction();

                    string queryString = "";
                    string personOrGroup = "";
                    if (pOrG == "R")
                    {
                        descrizione = "Revoca diritto a ruolo ";
                        personOrGroup = ((DocsPaVO.utente.Ruolo)dirittoOggetto.soggetto).idGruppo;
                    }
                    else
                    {
                        descrizione = "Revoca diritto a utente ";
                        personOrGroup = ((DocsPaVO.utente.Utente)dirittoOggetto.soggetto).idPeople;
                    }
                    string tipoDiritto = getTipoDiritto(dirittoOggetto.tipoDiritto.ToString());

                    DocsPaUtils.Query q;

                    //ricerca di id_gruppo_trasm in security
                    string idGruppoTrasm = "";
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SECURITY_GENERIC");
                    q.setParam("param1", "ID_GRUPPO_TRASM");
                    q.setParam("param2", "WHERE THING IN (" + dirittoOggetto.idObj + ", " + dirittoOggetto.rootFolder + ") AND ACCESSRIGHTS=" + dirittoOggetto.accessRights + " AND PERSONORGROUP=" + personOrGroup);
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    dbProvider.ExecuteScalar(out idGruppoTrasm, queryString);
                    if (idGruppoTrasm.Equals(null))
                        idGruppoTrasm = "";

                    descrizione += dirittoOggetto.soggetto.codiceRubrica;
                    //rimozione dalla security dell'utente a cui togliere i diritti
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("D_SECURITY");
                    q.setParam("param1", "THING IN (" + dirittoOggetto.idObj + ", " + dirittoOggetto.rootFolder + ") AND PERSONORGROUP=" + personOrGroup + " AND ACCESSRIGHTS=" + dirittoOggetto.accessRights);
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }

                    //inserimento nella deleted_security dell'utente a cui sono stati rimossi i diritti
                    string note = "Diritto rimosso da: " + infoUtente.userId;
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DELETED_SECURITY");
                    if (idGruppoTrasm.Equals(""))
                        q.setParam("param1", dirittoOggetto.idObj + "," + personOrGroup + "," + dirittoOggetto.accessRights + ",'','" + tipoDiritto + "','" + note + "'," + DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")) + ",'" + infoUtente.idPeople + "','" + infoUtente.idGruppo + "', NULL");
                    else
                        q.setParam("param1", dirittoOggetto.idObj + "," + personOrGroup + "," + dirittoOggetto.accessRights + "," + idGruppoTrasm + ",'" + tipoDiritto + "','" + note + "'," + DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")) + ",'" + infoUtente.idPeople + "','" + infoUtente.idGruppo + "', NULL");
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }

                    //inserimento nella deleted_security dell'utente a cui sono stati rimossi i diritti
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DELETED_SECURITY");
                    if (idGruppoTrasm.Equals(""))
                        q.setParam("param1", dirittoOggetto.rootFolder + "," + personOrGroup + "," + dirittoOggetto.accessRights + ",'','" + tipoDiritto + "','" + note + "'," + DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")) + ",'" + infoUtente.idPeople + "','" + infoUtente.idGruppo + "', NULL");
                    else
                        q.setParam("param1", dirittoOggetto.rootFolder + "," + personOrGroup + "," + dirittoOggetto.accessRights + "," + idGruppoTrasm + ",'" + tipoDiritto + "','" + note + "'," + DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")) + ",'" + infoUtente.idPeople + "','" + infoUtente.idGruppo + "', NULL");
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }


                    //se si revocano i diritti all'utente proprietario => si revocano anche al ruolo proprietario
                    //e viceversa. Il proprietario del documento diventa l'utente e il ruolo del revocante
                    if (tipoDiritto == "P")
                    {
                        string personOrGroupRU = "";
                        string accessRight = "";
                        string idGruppoTrasmRU = "";
                        string cod_rubrica = "";
                        IDataReader dr = null;

                        //ex-proprietario
                        //ricerca del ruolo se utente o ricerca dell'utente proprietario se ruolo
                        //il record trovato va eliminato dalla security e aggiunto nella deleted_security
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SECURITY__DPA_CORR_GLOBALI__PROPRIETARIO");
                        if (pOrG == "R")
                        {
                            descrizione += " e a utente ";
                            q.setParam("param3", "C.ID_PEOPLE=S.PERSONORGROUP");
                        }
                        else
                        {
                            descrizione += " e a ruolo ";
                            q.setParam("param3", "C.ID_GRUPPO=S.PERSONORGROUP");
                        }
                        q.setParam("param1", dirittoOggetto.idObj);
                        q.setParam("param2", "'P' AND PERSONORGROUP <> " + personOrGroup);
                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        using (dr = dbProvider.ExecuteReader(queryString))
                        {
                            if (dr == null)
                            {
                                throw new Exception("Errore in EditingACL");
                            }
                            if (dr != null && dr.FieldCount > 0)
                            {
                                while (dr.Read())
                                {
                                    personOrGroupRU = dr.GetValue(0).ToString();
                                    accessRight = dr.GetValue(1).ToString();
                                    idGruppoTrasmRU = dr.GetValue(2).ToString();
                                    cod_rubrica = dr.GetValue(3).ToString();
                                }
                            }
                        }
                        if (dr != null && (!dr.IsClosed))
                            dr.Close();
                        if ((personOrGroupRU != null && personOrGroupRU == "") && (accessRight != null && accessRight != ""))
                            result = false;
                        descrizione += cod_rubrica;


                        //rimozione dalla security 
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("D_SECURITY");
                        q.setParam("param1", "THING IN (" + dirittoOggetto.idObj + ", " + dirittoOggetto.rootFolder + ") AND PERSONORGROUP=" + personOrGroupRU + " AND ACCESSRIGHTS=" + accessRight);
                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        if (!dbProvider.ExecuteNonQuery(queryString))
                        {
                            result = false;
                            dbProvider.RollbackTransaction();
                            throw new Exception();
                        }
                        //inserimento nella deleted_security
                        note = "Diritto rimosso da: " + infoUtente.userId;
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DELETED_SECURITY");
                        if (idGruppoTrasmRU.Equals("") || idGruppoTrasmRU.Equals(null))
                            q.setParam("param1", dirittoOggetto.idObj + "," + personOrGroupRU + "," + accessRight + ",'','P','" + note + "'," + DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")) + ",'" + infoUtente.idPeople + "','" + infoUtente.idGruppo + "', NULL");
                        else
                            q.setParam("param1", dirittoOggetto.idObj + "," + personOrGroupRU + "," + accessRight + "," + idGruppoTrasmRU + ",'P','" + note + "'," + DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")) + ",'" + infoUtente.idPeople + "','" + infoUtente.idGruppo + "', NULL");
                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        if (!dbProvider.ExecuteNonQuery(queryString))
                        {
                            result = false;
                            dbProvider.RollbackTransaction();
                            throw new Exception();
                        }

                        //inserimento nella deleted_security anche della rootFolder
                        note = "Diritto rimosso da: " + infoUtente.userId;
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DELETED_SECURITY");
                        if (idGruppoTrasmRU.Equals("") || idGruppoTrasmRU.Equals(null))
                            q.setParam("param1", dirittoOggetto.rootFolder + "," + personOrGroupRU + "," + accessRight + ",'','P','" + note + "'," + DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")) + ",'" + infoUtente.idPeople + "','" + infoUtente.idGruppo + "', NULL");
                        else
                            q.setParam("param1", dirittoOggetto.rootFolder + "," + personOrGroupRU + "," + accessRight + "," + idGruppoTrasmRU + ",'P','" + note + "'," + DocsPaDbManagement.Functions.Functions.ToDate(DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")) + ",'" + infoUtente.idPeople + "','" + infoUtente.idGruppo + "', NULL");
                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        if (!dbProvider.ExecuteNonQuery(queryString))
                        {
                            result = false;
                            dbProvider.RollbackTransaction();
                            throw new Exception();
                        }

                        //nuovo proprietario
                        //UTENTE REVOCANTE E SUO RUOLO VENGONO INSERITI COME PROPRIETARI NELLA SECURITY
                        //UTENTE
                        int accessR = 0;
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");
                        q.setParam("param1", dirittoOggetto.idObj + "," + infoUtente.idPeople + "," + accessR + ",'','P', NULL");
                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        if (!dbProvider.ExecuteNonQuery(queryString))
                        {
                            result = false;
                            dbProvider.RollbackTransaction();
                            throw new Exception();
                        }
                        //RUOLO
                        accessR = 255;
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");
                        q.setParam("param1", dirittoOggetto.idObj + "," + infoUtente.idGruppo + "," + accessR + ",'','P', NULL");
                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        if (!dbProvider.ExecuteNonQuery(queryString))
                        {
                            result = false;
                            dbProvider.RollbackTransaction();
                            throw new Exception();
                        }

                        // nuovo proprietario anche per la rootFolder
                        accessR = 0;
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");
                        q.setParam("param1", dirittoOggetto.rootFolder + "," + infoUtente.idPeople + "," + accessR + ",'','P', NULL");
                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        if (!dbProvider.ExecuteNonQuery(queryString))
                        {
                            result = false;
                            dbProvider.RollbackTransaction();
                            throw new Exception();
                        }
                        //RUOLO
                        accessR = 255;
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");
                        q.setParam("param1", dirittoOggetto.rootFolder + "," + infoUtente.idGruppo + "," + accessR + ",'','P', NULL");
                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        if (!dbProvider.ExecuteNonQuery(queryString))
                        {
                            result = false;
                            dbProvider.RollbackTransaction();
                            throw new Exception();
                        }
                    }
                    descrizione += ". Tipo diritto: " + setTipoDiritto(dirittoOggetto);

                    //Calcolo l'eventuale atipicià del fascicolo
                    Documentale documentale = new Documentale();
                    documentale.CalcolaAtipicita(infoUtente, dirittoOggetto.idObj, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO);

                    dbProvider.CommitTransaction();
                }
            }
            catch (Exception e)
            {
                logger.Debug("F_System");
                result = false;
            }

            return result;
        }

        public bool RipristinaACL(out string descrizione, DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto, string pOrG, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            descrizione = "";
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.BeginTransaction();
                    IDataReader dr = null;
                    string queryString = "";
                    string personOrGroup = "";
                    if (pOrG == "R")
                    {
                        descrizione = "Ripristino diritto a ruolo ";
                        personOrGroup = ((DocsPaVO.utente.Ruolo)dirittoOggetto.soggetto).idGruppo;
                    }
                    else
                    {
                        descrizione = "Ripristino diritto a utente ";
                        personOrGroup = ((DocsPaVO.utente.Utente)dirittoOggetto.soggetto).idPeople;
                    }
                    string tipoDiritto = getTipoDiritto(dirittoOggetto.tipoDiritto.ToString());

                    DocsPaUtils.Query q;
                    if (tipoDiritto == "P")
                    {
                        //rimozione dei vecchi proprietari dalla security
                        string personOrGroupP = "";
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_SECURITY_PROPRIETARIO");
                        q.setParam("param1", dirittoOggetto.idObj);
                        q.setParam("param2", "'P'");

                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        using (dr = dbProvider.ExecuteReader(queryString))
                        {
                            if (dr == null)
                            {
                                throw new Exception("Errore in EditingACL");
                            }

                            if (dr != null && dr.FieldCount > 0)
                            {
                                while (dr.Read())
                                {
                                    personOrGroupP = dr.GetValue(0).ToString();
                                    //rimozione dalla security 
                                    q = DocsPaUtils.InitQuery.getInstance().getQuery("D_SECURITY");
                                    q.setParam("param1", "THING IN (" + dirittoOggetto.idObj + ", " + dirittoOggetto.rootFolder + ") AND PERSONORGROUP=" + personOrGroupP + " AND CHA_TIPO_DIRITTO= 'P'");
                                    queryString = q.getSQL();
                                    logger.Debug(queryString);
                                    if (!dbProvider.ExecuteNonQuery(queryString))
                                    {
                                        result = false;
                                        dbProvider.RollbackTransaction();
                                        throw new Exception();
                                    }
                                }
                            }
                        }
                        if (dr != null && (!dr.IsClosed))
                            dr.Close();
                        if (personOrGroupP != null && personOrGroupP == "")
                            result = false;
                    }
                    //ricerca di id_gruppo_trasm in deleted_security
                    string idGruppoTrasm = "";
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DELETED_SECURITY");
                    q.setParam("param1", "ID_GRUPPO_TRASM");
                    q.setParam("param2", "WHERE THING IN (" + dirittoOggetto.idObj + ", " + dirittoOggetto.rootFolder + ") AND ACCESSRIGHTS=" + dirittoOggetto.accessRights + " AND PERSONORGROUP=" + personOrGroup);
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    dbProvider.ExecuteScalar(out idGruppoTrasm, queryString);
                    if (idGruppoTrasm.Equals(null))
                        idGruppoTrasm = "";



                    //rimozione dalla deleted_security dell'utente a cui ripristinare i diritti
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DELETED_SECURITY");
                    q.setParam("param1", "THING IN (" + dirittoOggetto.idObj + ", " + dirittoOggetto.rootFolder + ") AND PERSONORGROUP=" + personOrGroup + " AND ACCESSRIGHTS=" + dirittoOggetto.accessRights);
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }

                    descrizione += dirittoOggetto.soggetto.codiceRubrica;
                    //inserimento nella security dell'utente a cui devono essere ripristinati i diritti
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");
                    if (idGruppoTrasm.Equals(""))
                        q.setParam("param1", dirittoOggetto.idObj + "," + personOrGroup + "," + dirittoOggetto.accessRights + ",'','" + tipoDiritto + "', NULL");
                    else
                        q.setParam("param1", dirittoOggetto.idObj + "," + personOrGroup + "," + dirittoOggetto.accessRights + "," + idGruppoTrasm + ",'" + tipoDiritto + "', NULL");
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_Security");
                    if (idGruppoTrasm.Equals(""))
                        q.setParam("param1", dirittoOggetto.rootFolder + "," + personOrGroup + "," + dirittoOggetto.accessRights + ",'','" + tipoDiritto + "', NULL");
                    else
                        q.setParam("param1", dirittoOggetto.rootFolder + "," + personOrGroup + "," + dirittoOggetto.accessRights + "," + idGruppoTrasm + ",'" + tipoDiritto + "', NULL");
                    queryString = q.getSQL();
                    logger.Debug(queryString);
                    if (!dbProvider.ExecuteNonQuery(queryString))
                    {
                        result = false;
                        dbProvider.RollbackTransaction();
                        throw new Exception();
                    }

                    //se si ripristinano i diritti all'utente proprietario => si ripristinano anche al ruolo proprietario
                    //e viceversa.
                    if (tipoDiritto == "P")
                    {
                        string personOrGroupRU = "";
                        string accessRight = "";
                        string idGruppoTrasmRU = "";
                        string cod_rubrica = "";

                        //ex-proprietario
                        //ricerca del ruolo se utente o ricerca dell'utente proprietario se ruolo
                        //il record trovato va eliminato dalla deleted_security e aggiunto nella security
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DELETED_SECURITY_PROPRIETARIO");
                        if (pOrG == "R")
                        {
                            descrizione += " e a utente ";
                            q.setParam("param4", "C.ID_PEOPLE=S.PERSONORGROUP");
                        }
                        else
                        {
                            descrizione += " e a ruolo ";
                            q.setParam("param4", "C.ID_GRUPPO=S.PERSONORGROUP");
                        }
                        q.setParam("param1", dirittoOggetto.idObj);
                        q.setParam("param2", "P");
                        q.setParam("param3", personOrGroup);
                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        using (dr = dbProvider.ExecuteReader(queryString))
                        {
                            if (dr == null)
                            {
                                throw new Exception("Errore in EditingACL");
                            }
                            if (dr != null && dr.FieldCount > 0)
                            {
                                while (dr.Read())
                                {
                                    personOrGroupRU = dr.GetValue(0).ToString();
                                    accessRight = dr.GetValue(1).ToString();
                                    idGruppoTrasmRU = dr.GetValue(2).ToString();
                                    cod_rubrica = dr.GetValue(3).ToString();
                                }
                            }
                        }
                        if (dr != null && (!dr.IsClosed))
                            dr.Close();
                        if ((personOrGroupRU != null && personOrGroupRU == "") && (accessRight != null && accessRight != ""))
                            result = false;
                        descrizione += cod_rubrica;

                        //rimozione dalla deleted_security 
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DELETED_SECURITY");
                        q.setParam("param1", "THING IN (" + dirittoOggetto.idObj + ", " + dirittoOggetto.rootFolder + ") AND PERSONORGROUP=" + personOrGroupRU + " AND ACCESSRIGHTS=" + accessRight);
                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        if (!dbProvider.ExecuteNonQuery(queryString))
                        {
                            result = false;
                            dbProvider.RollbackTransaction();
                            throw new Exception();
                        }
                        //inserimento nella security
                        //note = "Diritto rimosso";
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY");
                        if (idGruppoTrasmRU.Equals("") || idGruppoTrasmRU.Equals(null))
                            q.setParam("param1", dirittoOggetto.idObj + "," + personOrGroupRU + "," + accessRight + ",'','P', NULL");
                        else
                            q.setParam("param1", dirittoOggetto.idObj + "," + personOrGroupRU + "," + accessRight + "," + idGruppoTrasmRU + ",'P', NULL");
                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        if (!dbProvider.ExecuteNonQuery(queryString))
                        {
                            result = false;
                            dbProvider.RollbackTransaction();
                            throw new Exception();
                        }

                        q = DocsPaUtils.InitQuery.getInstance().getQuery("I_SECURITY");
                        if (idGruppoTrasmRU.Equals("") || idGruppoTrasmRU.Equals(null))
                            q.setParam("param1", dirittoOggetto.rootFolder + "," + personOrGroupRU + "," + accessRight + ",'','P', NULL");
                        else
                            q.setParam("param1", dirittoOggetto.rootFolder + "," + personOrGroupRU + "," + accessRight + "," + idGruppoTrasmRU + ",'P', NULL");
                        queryString = q.getSQL();
                        logger.Debug(queryString);
                        if (!dbProvider.ExecuteNonQuery(queryString))
                        {
                            result = false;
                            dbProvider.RollbackTransaction();
                            throw new Exception();
                        }
                    }
                    descrizione += ". Tipo diritto: " + setTipoDiritto(dirittoOggetto);

                    //Calcolo l'eventuale atipicià del fascicolo
                    Documentale documentale = new Documentale();
                    documentale.CalcolaAtipicita(infoUtente, dirittoOggetto.idObj, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico.FASCICOLO);

                    dbProvider.CommitTransaction();
                }
            }
            catch (Exception e)
            {
                logger.Debug("F_System");
                result = false;
            }

            return result;
        }

        private string setTipoDiritto(DocsPaVO.fascicolazione.DirittoOggetto fascDir)
        {
            if (fascDir.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_ACQUISITO))
                return (string.IsNullOrEmpty(fascDir.CopiaVisibilita) || fascDir.CopiaVisibilita.Equals("0") ? "ACQUISITO" : "ACQUISITO PER COPIA VISIBILITA");
            else
                if (fascDir.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_PROPRIETARIO))
                    return "PROPRIETARIO";
                else
                    if (fascDir.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_TRASMISSIONE))
                        return "TRASMISSIONE";
                    else
                        if (fascDir.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_SOSPESO))
                            return "SOSPESO";
                        else
                            if (fascDir.tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_DELEGATO))
                                return "SOSTITUTO";
            return "";
        }

        private string getTipoDiritto(string tipoDiritto)
        {
            string diritto = "";
            if (tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_PROPRIETARIO.ToString()))
            {
                diritto = "P";
            }
            if (tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_TRASMISSIONE.ToString()))
            {
                diritto = "T";
            }
            if (tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_SOSPESO.ToString()))
            {
                diritto = "S";
            }
            if (tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_ACQUISITO.ToString()))
            {
                diritto = "A";
            }
            if (tipoDiritto.Equals(DocsPaVO.fascicolazione.TipoDiritto.TIPO_DELEGATO.ToString()))
            {
                diritto = "D";
            }
            return diritto;
        }


        #endregion

        #region Gestione Note documento

        /// <summary>
        /// Caricamento note fascicolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fascicolo"></param>        
        private static void FetchNoteFascicolo(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            Note noteDb = new Note(infoUtente);

            DocsPaVO.Note.AssociazioneNota associazione = new DocsPaVO.Note.AssociazioneNota(DocsPaVO.Note.AssociazioneNota.OggettiAssociazioniNotaEnum.Fascicolo, fascicolo.systemID);

            // Reperimento ultima nota visibile per il documento
            DocsPaVO.Note.InfoNota ultimaNota = noteDb.GetUltimaNota(associazione);

            if (ultimaNota != null)
                fascicolo.noteFascicolo = noteDb.GetNote(associazione, null);
        }

        /// <summary>
        /// Aggiornamento note fascicolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fascicolo"></param>
        public virtual void UpdateNoteFascicolo(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            if (fascicolo.noteFascicolo != null)
            {
                Note noteDb = new Note(infoUtente);

                fascicolo.noteFascicolo = noteDb.Update(new DocsPaVO.Note.AssociazioneNota(DocsPaVO.Note.AssociazioneNota.OggettiAssociazioniNotaEnum.Fascicolo, fascicolo.systemID),
                                                        fascicolo.noteFascicolo);

            }
        }

        #endregion

        #region trasferimento in deposito
        /// Ritorna un fascicolo con un determinato codice
        /// </summary>
        /// <param name="codiceFascicolo"></param>
        /// <param name="registro"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns>ArrayList di fascicoli o 'null' se si è verificato un errore.</returns>
        public DocsPaVO.fascicolazione.Fascicolo GetFascicoloDaArchiviare(DocsPaVO.utente.InfoUtente infoUtente, string codiceFascicolo)
        {
            logger.Debug("GetFascicoloDaArchiviare");
            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;
            try
            {
                string queryString = GetQueryFascicoliTD(infoUtente.idAmministrazione, codiceFascicolo);
                System.Data.DataSet dataSet;
                this.ExecuteQuery(out dataSet, "PROJECT", queryString);
                foreach (System.Data.DataRow dataRow in dataSet.Tables["PROJECT"].Rows)
                {
                    fascicolo = GetFascicolo(infoUtente, dataSet, dataRow, false);

                }
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("F_System");
                fascicolo = null;
            }
            return fascicolo;
        }


        public static string GetQueryFascicoliTD(string idAmministrazione, string codFascicolo)
        {
            DocsPaUtils.Query q;
            logger.Debug("Lista fascicoli - S_PROJECT_TRASFERIMENTO_DEPOSITO");
            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROJECT_TRASFERIMENTO_DEPOSITO");
            q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("DTA_APERTURA", false));
            q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("DTA_CHIUSURA", false));
            q.setParam("param3", idAmministrazione);
            string userDB = DocsPaDbManagement.Functions.Functions.GetDbUserSession();
            if (!string.IsNullOrEmpty(userDB))
                q.setParam("dbuser", userDB);
            q.setParam("stato", "A");
            q.setParam("tipofasc", "G");
            string queryString = q.getSQL();
            if (codFascicolo != null && codFascicolo != "")
            {
                queryString += " AND UPPER(VAR_CODICE)='" + codFascicolo.ToUpper() + "'";
            }
            logger.Debug(queryString);
            return queryString;
        }

        public System.Collections.ArrayList GetListaFascicoliDaArchiviare(DocsPaVO.utente.InfoUtente infoUtente,
            DocsPaVO.fascicolazione.Classificazione objClassificazione,
            DocsPaVO.utente.Registro registro,
            DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
            bool enableUfficioRef, bool enableProfilazione,
            bool childs,
            out int numTotPage,
            out int totalRecordCount,
            int requestedPage,
            int pageSize)
        {
            logger.Debug("getListaFascicoliPaging");
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();
            totalRecordCount = 0;
            numTotPage = 0;
            try
            {
                //parametri di queryString Comuni.
                string queryString = "";
                //nuova paginazione:
                //prima la count con i filtri.
                queryString = setParameter(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople,
                                                 objClassificazione, registro,
                                                 objListaFiltri, enableUfficioRef,
                                                 enableProfilazione, childs,
                                                 out numTotPage, 0,
                                                 totalRecordCount,
                                                 "DATACOUNT", pageSize);
                //eseguo la query count
                using (DBProvider dbProvider = new DBProvider())
                {
                    string field = string.Empty;
                    IDataReader dr = dbProvider.ExecuteReader(queryString);
                    while (dr.Read())
                    {
                        field = dr.GetValue(0).ToString();
                    }
                    if (field != string.Empty)
                        totalRecordCount = Convert.ToInt32(field);
                }

                if (totalRecordCount > 0)
                {
                    logger.Debug("Trovati " + totalRecordCount + " Fascicoli");
                    //poi la fill con i filtri
                    //procedo con la fill dei dati 
                    queryString = setParameter(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople,
                                                objClassificazione, registro,
                                                objListaFiltri, enableUfficioRef,
                                                enableProfilazione, childs,
                                                out numTotPage,
                                                requestedPage,
                                                totalRecordCount,
                                                "DATAFILL", pageSize);


                    DataSet ds = new DataSet();
                    using (DBProvider dbProvider = new DBProvider())
                    {
                        dbProvider.ExecuteQuery(ds, queryString);

                        if (ds != null && ds.Tables[0] != null)
                        {
                            foreach (System.Data.DataRow dataRow in ds.Tables[0].Rows)
                            {
                                listaFascicoli.Add(GetFascicolo(infoUtente, ds, dataRow, enableProfilazione));
                            }
                        }
                        ds.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                listaFascicoli = null;
            }
            return listaFascicoli;
        }

        private string setParameter(string idAmm,
                                          string idGruppo,
                                          string idPeople,
                                          DocsPaVO.fascicolazione.Classificazione objClassificazione,
                                          DocsPaVO.utente.Registro registro,
                                          DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                          bool enableUfficioRef,
                                          bool enableProfilazione,
                                          bool childs,
                                          out int numTotPage,
                                          int requestedPage,
                                          int totalRecordCount,
                                          string queryType,
                                          int pageSize
                                          )
        {
            numTotPage = 0;
            try
            {
                DocsPaUtils.Query q = null;
                string queryString = string.Empty;
                int startRow = 0;
                int endRow = 0;
                string filterString = string.Empty;

                if (queryType.ToUpper() == "DATACOUNT")
                {
                    // solo COUNT dei dati
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_FASCICOLI__TRASFERIMENTO_DEPOSITO");
                }
                if (queryType.ToUpper() == "DATAFILL") // solo FILL dei dati
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASCICOLI_TRASFERIMENTO_DEPOSITO");

                string userDB = getUserDB();
                if (!string.IsNullOrEmpty(userDB))
                    q.setParam("dbuser", userDB);

                // Ricerca con CodiceClassifica
                if (objClassificazione != null)
                {
                    q.setParam("tblCL1", " ,(SELECT A.SYSTEM_ID FROM PROJECT A WHERE " +
                                   " A.ID_AMM = @idAmm@ AND A.CHA_TIPO_PROJ = 'T' " +
                                   " @idReg@ @varCodLiv@) C ");

                    q.setParam("whereTlbCL1", " AND (A.ID_PARENT = C.SYSTEM_ID)");

                    // esiste solo se objClassificazione è valido
                    if (childs) q.setParam("varCodLiv", "AND A.VAR_COD_LIV1 LIKE '" + objClassificazione.varcodliv1 + "%'");
                    else q.setParam("varCodLiv", "AND  A.VAR_COD_LIV1 = '" + objClassificazione.varcodliv1 + "'");
                }
                else
                {
                    q.setParam("tblCL1", "");
                    q.setParam("whereTlbCL1", "");
                }
                //common Where Condition
                q.setParam("idAmm", idAmm);

                //q.setParam("tipofasc", "G");
                //q.setParam("stato", "A");

                //registro
                if (registro != null) q.setParam("idReg", " AND (ID_REGISTRO IS NULL OR ID_REGISTRO =" + registro.systemId + ")");
                else q.setParam("idReg", "");

                if (queryType == "DATAFILL") // solo FILL dei dati aggiungo i filtri
                {

                    // operazioni Matematiche per calcolo paginazione
                    // Determina il num di pagine totali 
                    numTotPage = (totalRecordCount / pageSize);
                    if (numTotPage != 0)
                    {
                        if ((totalRecordCount % numTotPage) > 0) numTotPage++;
                    }
                    else numTotPage = 1;
                    startRow = ((requestedPage * pageSize) - pageSize) + 1;
                    endRow = (startRow - 1) + pageSize;
                    q.setParam("startRow", startRow.ToString());
                    q.setParam("endRow", endRow.ToString());

                    // INIZIO - Parametri specifici per SqlServer
                    // TODO : rovesciamento criteri di ordinamento dedicati a SQL e count SQL
                    // INIZIO - Parametri specifici per SqlServer
                    // il numero totale di righe da estrarre equivale 
                    // al limite inferiore dell'ultima riga da estrarre


                    int pageSizeSqlServer = pageSize;
                    int totalRowsSqlServer = (requestedPage * pageSize);
                    if ((totalRecordCount - totalRowsSqlServer) <= 0)
                    {
                        pageSizeSqlServer -= System.Math.Abs(totalRecordCount - totalRowsSqlServer);
                        totalRowsSqlServer = totalRecordCount;
                    }

                    q.setParam("pageSize", pageSizeSqlServer.ToString()); // Dimensione pagina
                    q.setParam("totalRows", totalRowsSqlServer.ToString());


                    // FINE - Parametri specifici per SqlServer

                }
                q.setParam("dtaAp", DocsPaDbManagement.Functions.Functions.ToChar("DTA_APERTURA", false));
                q.setParam("dtaCh", DocsPaDbManagement.Functions.Functions.ToChar("DTA_CHIUSURA", false));

                // flitri GUI
                GetSqlQueryA(idGruppo, idPeople, objListaFiltri, ref filterString);

                //q.setParam("profilazione", " ");

                //aggiungo i filtri GUI al query
                if (filterString != null) q.setParam("guiFilters", filterString);

                //rilascio il query string
                queryString = q.getSQL();

                logger.Debug(queryType + ": " + queryString);

                return queryString;

            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return null;
            }

        }

        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="objListaFiltri"></param>
        /// <returns></returns>
        private void GetSqlQueryA(string idGruppo, string idPeople, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, ref string queryStr)
        {
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

            if (objListaFiltri == null) return;
            DocsPaVO.filtri.FiltroRicerca f;
            for (int i = 0; i < objListaFiltri.Length; i++)
            {
                f = objListaFiltri[i];
                if (f.valore != null && !f.valore.Equals(""))
                {
                    switch (f.argomento)
                    {
                        case "APERTURA_IL":
                            queryStr += " AND DTA_APERTURA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            queryStr += " AND DTA_APERTURA < " + DocsPaDbManagement.Functions.Functions.ToDate(f.valore + " 23:59:59");

                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "APERTURA_SUCCESSIVA_AL":
                            queryStr += " AND DTA_APERTURA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);

                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "APERTURA_PRECEDENTE_IL":
                            queryStr += " AND DTA_APERTURA<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);

                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "APERTURA_SC":
                            // data apertura nella settimana corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_APERTURA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND DTA_APERTURA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryStr += " AND DTA_APERTURA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND DTA_APERTURA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "APERTURA_MC":
                            // data apertura nel mese corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_APERTURA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND DTA_APERTURA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryStr += " AND DTA_APERTURA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND DTA_APERTURA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "APERTURA_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND to_char(DTA_APERTURA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryStr += " AND DTA_APERTURA>=(SELECT getdate()) AND DTA_APERTURA<=(SELECT getdate()) ";
                            break;
                        case "CHIUSURA_IL":
                            queryStr += " AND DTA_CHIUSURA=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);

                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "CHIUSURA_SUCCESSIVA_AL":
                            queryStr += " AND DTA_CHIUSURA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "CHIUSURA_PRECEDENTE_IL":
                            queryStr += " AND DTA_CHIUSURA<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "CHIUSURA_SC":
                            // data chiusura nella settimana corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_CHIUSURA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND DTA_CHIUSURA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryStr += " AND DTA_CHIUSURA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND DTA_CHIUSURA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "CHIUSURA_MC":
                            // data chiusura nel mese corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_CHIUSURA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND DTA_CHIUSURA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryStr += " AND DTA_CHIUSURA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND DTA_CHIUSURA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "CHIUSURA_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND to_char(DTA_CHIUSURA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryStr += " AND DTA_CHIUSURA>=(SELECT getdate()) AND DTA_CHIUSURA<=(SELECT getdate()) ";
                            break;
                        //case "STATO":
                        //    queryStr += " AND (CHA_STATO='" + f.valore.ToUpper() + "' )";
                        //    break;
                        case "TITOLO":
                            //	whereStr += " AND  (UPPER(A.DESCRIPTION) LIKE '%" + f.valore.ToUpper().Replace("'","''") + "%' )"; 
                            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("&&");
                            string[] lista = regex.Split(f.valore);
                            queryStr += "AND UPPER(DESCRIPTION) LIKE '%" + lista[0].ToUpper().Replace("'", "''") + "%'";
                            for (int k = 1; k < lista.Length; k++)
                                queryStr += " AND UPPER(DESCRIPTION) LIKE '%" + lista[k].ToUpper().Replace("'", "''") + "%'";
                            break;
                        //case "TIPO_FASCICOLO":
                        //    queryStr += " AND  (CHA_TIPO_FASCICOLO = '" + f.valore.ToUpper() + "' ) ";
                        //    break;
                        // Maurizio Tammacco aggiunti filtri per anno fascicolo e numero fascicolo
                        case "ANNO_FASCICOLO":
                            queryStr += " AND (ANNO_CREAZIONE = " + f.valore + " )";
                            break;
                        case "NUMERO_FASCICOLO":
                            queryStr += " AND (NUM_FASCICOLO = " + f.valore + " )";
                            break;
                        // Federica Franci aggiunti filtri per data di creazione del fascicolo
                        case "CREAZIONE_IL":
                            queryStr += " AND DTA_CREAZIONE>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            queryStr += " AND DTA_CREAZIONE < " + DocsPaDbManagement.Functions.Functions.ToDate(f.valore + " 23:59:59");
                            break;
                        case "CREAZIONE_SUCCESSIVA_AL":
                            //queryStr += " AND DTA_CREAZIONE>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            queryStr += " AND DTA_CREAZIONE>=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, true);
                            break;
                        case "CREAZIONE_PRECEDENTE_IL":
                            //queryStr += " AND DTA_CREAZIONE<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            queryStr += "AND DTA_CREAZIONE<=" + DocsPaDbManagement.Functions.Functions.ToDateBetween(f.valore, false);
                            break;
                        case "CREAZIONE_SC":
                            // data creazione nella settimana corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_CREAZIONE>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND DTA_CREAZIONE<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryStr += " AND DTA_CREAZIONE>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND DTA_CREAZIONE<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "CREAZIONE_MC":
                            // data creazione nel mese corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_CREAZIONE>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND DTA_CREAZIONE<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryStr += " AND DTA_CREAZIONE>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND DTA_CREAZIONE<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "CREAZIONE_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND to_char(DTA_CREAZIONE, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryStr += " AND DTA_CREAZIONE>=(SELECT getdate()) AND DTA_CREAZIONE<=(SELECT getdate()) ";
                            break;
                        //Locazione Fisica
                        case "ID_UO_LF":
                            queryStr += " AND ID_UO_LF = " + f.valore;
                            break;
                        case "DATA_LF_IL":
                            queryStr += " AND DTA_UO_LF = " + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            break;
                        case "DATA_LF_PRECEDENTE_IL":
                            queryStr += " AND DTA_UO_LF <= " + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            break;
                        case "DATA_LF_SUCCESSIVA_AL":
                            queryStr += " AND DTA_UO_LF >= " + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            break;
                        case "DATA_LF_SC":
                            // data lf nella settimana corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_UO_LF>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND DTA_UO_LF<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryStr += " AND DTA_UO_LF>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND DTA_UO_LF<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "DATA_LF_MC":
                            // data lf nel mese corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_UO_LF>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND DTA_UO_LF<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryStr += " AND DTA_UO_LF>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND DTA_UO_LF<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "DATA_LF_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND to_char(DTA_UO_LF, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryStr += " AND DTA_UO_LF>=(SELECT getdate()) AND DTA_UO_LF<=(SELECT getdate()) ";
                            break;
                        case "ID_UO_REF":
                            queryStr += " AND ID_UO_REF = " + f.valore;
                            break;
                        //case "VAR_NOTE":
                        //    System.Text.RegularExpressions.Regex regexNote = new System.Text.RegularExpressions.Regex("&&");

                        //    string[] listaNote = regexNote.Split(f.valore);

                        //    string userDb = getUserDB();
                        //    if (!string.IsNullOrEmpty(userDb))
                        //        userDb += ".";

                        //    foreach (string item in listaNote)
                        //        queryStr += " AND " + string.Format("{0}GetCountNote('F', A.SYSTEM_ID, '{1}', {2}, {3}) > 0", userDb, item.Replace("'", "''"), idPeople, idGruppo);

                        //    break;

                        case "TIPOLOGIA_FASCICOLO":
                            DocsPaDB.Query_DocsPAWS.ModelFasc mdFasc = new ModelFasc();
                            DocsPaVO.ProfilazioneDinamica.Templates profilo = mdFasc.getTemplateFascById(f.valore);


                            if (profilo != null && profilo.IPER_FASC_DOC != "1")
                            {
                                queryStr += " AND id_tipo_fasc = " + profilo.SYSTEM_ID.ToString();
                            }
                            tipo_contatore = this.tipoContatoreTemplates(f.valore);
                            break;
                        case "PROFILAZIONE_DINAMICA":
                            DocsPaDB.Query_DocsPAWS.ModelFasc modelFasc = new ModelFasc();
                            queryStr += modelFasc.getSeriePerRicercaProfilazione(f.template, "");
                            if (f.template != null && !string.IsNullOrEmpty(f.template.SYSTEM_ID.ToString()))
                                tipo_contatore = this.tipoContatoreTemplates(f.template.SYSTEM_ID.ToString());
                            break;

                        case "DIAGRAMMA_STATO_FASC":
                            queryStr += "  AND SYSTEM_ID IN (SELECT ID_PROJECT FROM DPA_DIAGRAMMI WHERE DPA_DIAGRAMMI.ID_STATO = " + f.valore + ") ";
                            break;

                        case "ID_TITOLARIO":
                            queryStr += " AND ID_TITOLARIO IN (" + f.valore + ") ";
                            break;
                        case "ID_REGISTRO":
                            queryStr += " AND (ID_REGISTRO IS NULL OR ID_REGISTRO = " + f.valore + ") ";
                            break;

                        case "DOC_IN_FASC_ADL":
                            //split dei valori
                            string[] val = f.valore.Split('@');
                            queryStr += " and exists (select id_project from dpa_area_lavoro d where d.id_project=system_id and id_people=" + val[0] + " and id_ruolo_in_uo =" + val[1] + ")";
                            break;

                        case "SOTTOFASCICOLO":
                            queryStr += " AND A.SYSTEM_ID in (select id_fascicolo from project where CHA_TIPO_PROJ='C' AND UPPER(description) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%' and id_fascicolo != id_parent)";
                            //queryStr += " AND A.SYSTEM_ID in (select id_fascicolo from project where CHA_TIPO_PROJ='C' AND UPPER(description) LIKE '%" + f.valore.ToUpper().Replace("'", "''") + "%' )";
                            break;
                        case "SCADENZA_IL":
                            queryStr += " AND DTA_SCADENZA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);
                            queryStr += " AND DTA_SCADENZA < " + DocsPaDbManagement.Functions.Functions.ToDate(f.valore + " 23:59:59");

                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "SCADENZA_SUCCESSIVA_AL":
                            queryStr += " AND DTA_SCADENZA>=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);

                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "SCADENZA_PRECEDENTE_IL":
                            queryStr += " AND DTA_SCADENZA<=" + DocsPaDbManagement.Functions.Functions.ToDate(f.valore);

                            //DocsPaWS.Utils.dbControl.toDate(f.valore,false);				
                            break;
                        case "SCADENZA_SC":
                            // data scadenza nella settimana corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_SCADENZA>=(select to_date(to_char(sysdate+ (1-to_char(sysdate,'D')))) startdayofweek from dual) AND DTA_SCADENZA<(select to_date(to_char(sysdate+ (8-to_char(sysdate,'D')))) enddayofweek from dual) ";
                            else
                                queryStr += " AND DTA_SCADENZA>=(select DATEADD(DAY,-DATEPART(WEEKDAY,(DATEADD(DAY,7-DATEPART(WEEKDAY,GETDATE()),GETDATE())))+(7-DATEPART(WEEKDAY,GETDATE()))+2 ,GETDATE())) AND DTA_SCADENZA<=(select DATEADD(DAY , 8-DATEPART(WEEKDAY,GETDATE()),GETDATE())) ";
                            break;
                        case "SCADENZA_MC":
                            // data scadenza nel mese corrente
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND DTA_SCADENZA>=(select to_date(trunc(sysdate,'MM')) as start_date from dual) AND DTA_SCADENZA<(select to_date(last_day(sysdate)+1) as DAY from dual) ";
                            else
                                queryStr += " AND DTA_SCADENZA>=(SELECT DATEADD(dd,-(DAY(getdate())-1),getdate())) AND DTA_SCADENZA<=(SELECT DATEADD(dd,-(DAY(DATEADD(mm,1,getdate()))),DATEADD(mm,1,getdate()))) ";
                            break;
                        case "SCADENZA_TODAY":
                            if (!dbType.ToUpper().Equals("SQL"))
                                queryStr += " AND to_char(DTA_SCADENZA, 'DD/MM/YYYY') = (select to_char(sysdate, 'DD/MM/YYYY') from dual)";
                            else
                                queryStr += " AND DTA_SCADENZA>=(SELECT getdate()) AND DTA_SCADENZA<=(SELECT getdate()) ";
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Restituisce la lista dei documenti classificati in un dato fascicolo generale
        /// per poterli archiviare. (Paginata)
        /// </summary>
        /// <param name="idGruppo"></param>
        /// <param name="idPeople"></param>
        /// <param name="fascicolo"></param>
        /// <param name="numPage"></param>
        /// <param name="numTotPage"></param>
        /// <param name="nRec"></param>
        /// <param name="anno"></param>
        /// <returns></returns>
        public System.Collections.ArrayList GetDocumentiDaArchiviarePaging(
                string idGruppo,
                string idPeople,
                DocsPaVO.fascicolazione.Fascicolo fascicolo,
                int numPage,
                out int numTotPage,
                out int nRec,
                string anno)
        {
            nRec = 0;
            numTotPage = 0;

            System.Collections.ArrayList listaDocumenti = new System.Collections.ArrayList();

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DOCUMENTI_INFASCICOLO_TRASFERIMENTO_DEPOSITO");

                // per SQL
                if (dbType.ToUpper() == "SQL")
                {
                    q.setParam("dbuser", getUserDB());
                }

                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.CREATION_DATE", false));
                DocsPaVO.fascicolazione.Folder folder = GetFolderNOSecurity(idPeople, idGruppo, fascicolo.systemID);
                q.setParam("param2", folder.systemID);
                q.setParam("param3", idGruppo);
                q.setParam("param4", idPeople);
                if (!string.IsNullOrEmpty(anno))
                    q.setParam("anno", "and TO_NUMBER(TO_CHAR(CREATION_DATE,'YYYY'))='" + anno + "'");
                else
                    q.setParam("anno", "");
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataSet;
                //this.ExecuteQuery(out dataSet, "DOCUMENTI", queryString);
                this.ExecutePaging(out dataSet, out numTotPage, out nRec, numPage, 10, queryString, "PROFILE");

                //creazione della lista oggetti
                dataSet.Tables["PagingTable"].TableName = "DOCUMENTI";
                nRec = dataSet.Tables["DOCUMENTI"].Rows.Count;

                // Reperimento oggetto arraylist di oggetti "InfoDocumento"
                Documenti doc = new Documenti();
                listaDocumenti = doc.GetArrayListDocumenti(dataSet.Tables["DOCUMENTI"], true);
                doc.Dispose();
                doc = null;
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("F_System");

                listaDocumenti = null;
            }

            return listaDocumenti;
        }

        public DocsPaVO.fascicolazione.Folder GetFolderNOSecurity(string idPeople, string idGruppo, string idFascicolo)
        {
            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();
            try
            {
                System.Data.DataSet dataSet;// = new System.Data.DataSet();
                string updateString = "";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT_NO_SECURITY");
                q.setParam("param1", idFascicolo);
                updateString = q.getSQL();
                logger.Debug(updateString);
                this.ExecuteQuery(out dataSet, "FOLDER", updateString);
                System.Data.DataRow[] folderRootRows = dataSet.Tables["FOLDER"].Select("ID_PARENT=" + idFascicolo);
                if (folderRootRows.Length > 0)
                {
                    folderObject = GetFolderData(folderRootRows[0], dataSet.Tables["FOLDER"]);
                }
            }
            catch (Exception)
            {
                logger.Debug("F_System");
                folderObject = null;
            }
            return folderObject;
        }

        public System.Collections.ArrayList getSottoFascicoli(string fascID)
        {
            ArrayList fasc = new ArrayList();
            try
            {

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SOTTO_FASCICOLI");
                q.setParam("system_id", fascID);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataSet;
                this.ExecuteQuery(out dataSet, queryString);
                if (dataSet != null && dataSet.Tables[0] != null)
                {
                    foreach (System.Data.DataRow dataRow in dataSet.Tables[0].Rows)
                    {
                        fasc.Add(dataRow["SYSTEM_ID"].ToString());
                    }
                }
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("F_System");
                fasc = null;
            }
            return fasc;
        }

        public System.Collections.ArrayList GetDocumentiDaArchiviare(
                  string idGruppo,
                  string idPeople,
                  DocsPaVO.fascicolazione.Fascicolo fascicolo,

                  string anno)
        {
            System.Collections.ArrayList listaDocumenti = new System.Collections.ArrayList();
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DOCUMENTI_INFASCICOLO_TRASFERIMENTO_DEPOSITO");

                // per SQL
                if (dbType.ToUpper() == "SQL")
                {
                    q.setParam("dbuser", getUserDB());
                }

                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.CREATION_DATE", false));
                DocsPaVO.fascicolazione.Folder folder = GetFolderNOSecurity(idPeople, idGruppo, fascicolo.systemID);
                q.setParam("param2", folder.systemID);
                q.setParam("param3", idGruppo);
                q.setParam("param4", idPeople);
                if (!string.IsNullOrEmpty(anno))
                    q.setParam("anno", "and TO_NUMBER(TO_CHAR(CREATION_DATE,'YYYY'))='" + anno + "'");
                else
                    q.setParam("anno", "");
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataSet;
                this.ExecuteQuery(out dataSet, "DOCUMENTI", queryString);
                //creazione della lista oggetti
                // Reperimento oggetto arraylist di oggetti "InfoDocumento"
                Documenti doc = new Documenti();
                listaDocumenti = doc.GetArrayListDocumenti(dataSet.Tables["DOCUMENTI"], true);
                doc.Dispose();
                doc = null;
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("F_System");

                listaDocumenti = null;
            }

            return listaDocumenti;
        }


        //OK!!
        //Inserimento di un fascicolo in deposito
        public bool TrasfInDepFasc(string idProject, string valore, string tipoOp)
        {
            bool result = false;
            result = this.AddFascInDeposito(idProject, valore, tipoOp);
            return result;
        }

        //OK!!
        //Aggiunge un fascicolo in deposito.
        //Fascicoli procedimentali: metto
        //1: anche se il fascicolo contiene anche solo un documento che in deposito ha valore 2
        //perchè è classificato in altri fascicoli oltre quello in questione
        //Fascicoli generali: metto
        //2: sempre e comunque
        public bool AddFascInDeposito(string idProject, string valore, string tipoOp)
        {
            bool result = false;
            DocsPaUtils.Query q;
            string updateString = "";
            DBProvider dbProvider = new DBProvider();
            try
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("TRASF_DEP_FASC_PROC");
                //Operazione trasferimento in deposito
                if (tipoOp == "IN_DEPOSITO")
                    q.setParam("valore", valore);
                else
                {
                    //Operazione trasferimento in corrente
                    q.setParam("valore", "0");
                }
                q.setParam("system_id", idProject);
                updateString = q.getSQL();
                result = dbProvider.ExecuteNonQuery(updateString);
                //si tratta di un fascicolo procedimentale, allora cancello il fascicolo
                //dall'area di lavoro e dalla todolist
                //mentre i fascicoli generali non si cancellano perchè rimangono sempre a 2
                if (result && valore == "1")
                {
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPAAreaLavoro");
                    q.setParam("param1", "ID_PROJECT = " + idProject);
                    updateString = q.getSQL();
                    dbProvider.ExecuteNonQuery(updateString);

                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPATrasmUtente_archivio");
                    q.setParam("id_project", idProject);
                    updateString = q.getSQL();
                    dbProvider.ExecuteNonQuery(updateString);
                }

            }
            catch
            {
                result = false;
            }
            finally
            {
                dbProvider.Dispose();
            }
            return result;
        }

        #endregion

        #region Area Scarto
        public ArrayList GetListaScarto(DocsPaVO.utente.InfoUtente infoUtente, int numPage, out int numTotPage, out int totalRecordCount)
        {
            logger.Debug("GetListaScarto");
            ArrayList listaScarto = new ArrayList();

            numTotPage = 0;
            totalRecordCount = 0;
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {


                    string err = string.Empty;
                    ArrayList retValue = new ArrayList();
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_LISTA_SCARTO");
                    queryDef.setParam("idPeople", infoUtente.idPeople);
                    queryDef.setParam("idRuoloInUO", infoUtente.idGruppo);
                    queryDef.setParam("dtasc", DocsPaDbManagement.Functions.Functions.ToChar("DATA_SCARTO", false));
                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);

                    DataSet dataSet = new DataSet();

                    this.ExecutePaging(out dataSet, out numTotPage, out totalRecordCount, numPage, 10, commandText, "SCARTO");
                    //creazione della lista oggetti
                    dataSet.Tables["PagingTable"].TableName = "SCARTO";
                    totalRecordCount = dataSet.Tables["SCARTO"].Rows.Count;
                    if (dataSet != null && dataSet.Tables[0] != null)
                    {
                        foreach (System.Data.DataRow dataRow in dataSet.Tables[0].Rows)
                        {
                            DocsPaVO.AreaScarto.InfoScarto infoScarto = new DocsPaVO.AreaScarto.InfoScarto();
                            infoScarto.systemID = dataRow["SYSTEM_ID"].ToString();
                            infoScarto.idAmm = dataRow["ID_AMM"].ToString().Trim();
                            infoScarto.idPeople = dataRow["ID_PEOPLE"].ToString().Trim();
                            infoScarto.idRuoloInUo = dataRow["ID_RUOLO_IN_UO"].ToString();
                            infoScarto.stato = dataRow["CHA_STATO"].ToString();
                            infoScarto.note = dataRow["VAR_NOTE"].ToString();
                            infoScarto.descrizione = dataRow["VAR_DESCRIZIONE"].ToString();
                            infoScarto.data_Apertura = dataRow["DATA_APERTURA"].ToString();
                            infoScarto.data_Scarto = dataRow["DATA_SCARTO"].ToString().Trim();
                            infoScarto.estremi_richiesta = dataRow["VAR_ESTREMI_RICHIESTA"].ToString();
                            infoScarto.estremi_autorizzazione = dataRow["VAR_ESTREMI_AUTORIZ"].ToString();
                            listaScarto.Add(infoScarto);
                        }
                    }
                    dataSet.Dispose();

                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione Area Scarto (metodo: GetListaScarto)");
                listaScarto = null;
            }

            return listaScarto;
        }

        //Recupera la lista dei fascicoli procedimentali chiusi in deposito per un dato fascicolo generale
        //OK
        public System.Collections.ArrayList GetListaFascicoliInDeposito(DocsPaVO.utente.InfoUtente infoUtente,
            DocsPaVO.fascicolazione.Fascicolo fascicolo,
            int numPage, out int numTotPage, out int totalRecordCount)
        {
            logger.Debug("GetListaFascicoliInDeposito");
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

            numTotPage = 0;
            totalRecordCount = 0;
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    string idParent = "";
                    DocsPaUtils.Query queryDef;
                    string queryString = "";

                    //Cerco l'id del fascicolo generale selezionato associato al titolario selezionato
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");
                    queryDef.setParam("param1", "system_id");
                    queryDef.setParam("param2", "where var_codice='" + fascicolo.codice + "' and cha_tipo_proj='T' and id_titolario = " + fascicolo.idTitolario);

                    queryString = queryDef.getSQL();
                    dbProvider.ExecuteScalar(out idParent, queryString);


                    //Cerco tutti i fascicoli procedimentali chiusi e in stato in deposito, associati al fascicolo generale selezionato 
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASCICOLI_SCARTO_PAGING");
                    queryDef.setParam("idPeo", infoUtente.idPeople);
                    queryDef.setParam("idGrp", infoUtente.idGruppo);
                    queryDef.setParam("idAmm", infoUtente.idAmministrazione);

                    queryDef.setParam("idParent", "id_parent = " + idParent + "and ");

                    queryDef.setParam("dtaAp", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                    queryDef.setParam("dtaCh", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));

                    string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                    if (string.IsNullOrEmpty(idRuoloPubblico))
                        idRuoloPubblico = "0";
                    queryDef.setParam("idRuoloPubblico", idRuoloPubblico);

                    queryString = queryDef.getSQL();

                    DataSet dataSet = new DataSet();

                    this.ExecutePaging(out dataSet, out numTotPage, out totalRecordCount, numPage, 10, queryString, "PROFILE");
                    //creazione della lista oggetti
                    dataSet.Tables["PagingTable"].TableName = "FASCICOLI";
                    totalRecordCount = dataSet.Tables["FASCICOLI"].Rows.Count;
                    if (dataSet != null && dataSet.Tables[0] != null)
                    {
                        foreach (System.Data.DataRow dataRow in dataSet.Tables[0].Rows)
                        {
                            DocsPaVO.fascicolazione.Fascicolo fasc = GetFascicolo(infoUtente, dataSet, dataRow, false);
                            listaFascicoli.Add(fasc);
                        }
                    }
                    dataSet.Dispose();
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione Area Scarto (metodo: GetListaFascicoliInDeposito)");
                listaFascicoli = null;
            }

            return listaFascicoli;
        }

        public System.Collections.ArrayList GetListaFascicoliRicInDeposito(DocsPaVO.utente.InfoUtente infoUtente,
            string tipoRic,
            int numPage, out int numTotPage, out int totalRecordCount)
        {
            logger.Debug("GetListaFascicoliRicInDeposito");
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

            numTotPage = 0;
            totalRecordCount = 0;
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    string idParent = "";
                    DocsPaUtils.Query queryDef;
                    string queryString = "";


                    //Cerco tutti i fascicoli procedimentali chiusi e in stato in deposito
                    if (tipoRic == "T")
                    {
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASCICOLI_SCARTO_PAGING");
                        queryDef.setParam("idPeo", infoUtente.idPeople);
                        queryDef.setParam("idGrp", infoUtente.idGruppo);
                        queryDef.setParam("idAmm", infoUtente.idAmministrazione);
                        queryDef.setParam("idParent", "");

                        queryDef.setParam("dtaAp", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                        queryDef.setParam("dtaCh", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));

                        string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                        if (string.IsNullOrEmpty(idRuoloPubblico))
                            idRuoloPubblico = "0";
                        queryDef.setParam("idRuoloPubblico", idRuoloPubblico);

                        queryString = queryDef.getSQL();
                    }
                    //else TODO ricerca per max num conservazione

                    DataSet dataSet = new DataSet();
                    this.ExecutePaging(out dataSet, out numTotPage, out totalRecordCount, numPage, 10, queryString, "PROFILE");
                    //creazione della lista oggetti
                    dataSet.Tables["PagingTable"].TableName = "FASCICOLI";
                    totalRecordCount = dataSet.Tables["FASCICOLI"].Rows.Count;
                    if (dataSet != null && dataSet.Tables[0] != null)
                    {
                        foreach (System.Data.DataRow dataRow in dataSet.Tables[0].Rows)
                        {
                            DocsPaVO.fascicolazione.Fascicolo fasc = GetFascicolo(infoUtente, dataSet, dataRow, false);
                            listaFascicoli.Add(fasc);
                        }
                    }
                    dataSet.Dispose();
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione Area Scarto (metodo: GetListaFascicoliInDeposito)");
                listaFascicoli = null;
            }

            return listaFascicoli;
        }

        public System.Collections.ArrayList GetALLFascicoliInDeposito(DocsPaVO.utente.InfoUtente infoUtente, string tipoRic)
        {
            logger.Debug("GetListaFascicoliRicInDeposito");
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    string idParent = "";
                    DocsPaUtils.Query queryDef;
                    string queryString = "";

                    //Cerco tutti i fascicoli procedimentali chiusi e in stato in deposito
                    if (tipoRic == "T")
                    {
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASCICOLI_SCARTO_PAGING");
                        queryDef.setParam("idPeo", infoUtente.idPeople);
                        queryDef.setParam("idGrp", infoUtente.idGruppo);
                        queryDef.setParam("idAmm", infoUtente.idAmministrazione);
                        queryDef.setParam("idParent", "");

                        queryDef.setParam("dtaAp", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                        queryDef.setParam("dtaCh", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));

                        string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                        if (string.IsNullOrEmpty(idRuoloPubblico))
                            idRuoloPubblico = "0";
                        queryDef.setParam("idRuoloPubblico", idRuoloPubblico);

                        queryString = queryDef.getSQL();
                    }
                    //else TODO ricerca per max num conservazione

                    DataSet dataSet = new DataSet();
                    this.ExecuteQuery(out dataSet, "FASCICOLI", queryString);
                    //creazione della lista oggetti
                    if (dataSet != null && dataSet.Tables[0] != null)
                    {
                        foreach (System.Data.DataRow dataRow in dataSet.Tables[0].Rows)
                        {
                            DocsPaVO.fascicolazione.Fascicolo fasc = GetFascicolo(infoUtente, dataSet, dataRow, false);
                            listaFascicoli.Add(fasc);
                        }
                    }
                    dataSet.Dispose();
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione Area Scarto (metodo: GetListaFascicoliInDeposito)");
                listaFascicoli = null;
            }

            return listaFascicoli;
        }


        public System.Collections.ArrayList GetAllFascicoliByCODInDeposito(DocsPaVO.utente.InfoUtente infoUtente,
            DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            logger.Debug("GetAllFascicoliByCODInDeposito");
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    string idParent = "";
                    DocsPaUtils.Query queryDef;
                    string queryString = "";

                    //Cerco l'id del fascicolo generale selezionato associato al titolario selezionato
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project0");
                    queryDef.setParam("param1", "system_id");
                    queryDef.setParam("param2", "where var_codice='" + fascicolo.codice + "' and cha_tipo_proj='T' and id_titolario = " + fascicolo.idTitolario);

                    queryString = queryDef.getSQL();
                    dbProvider.ExecuteScalar(out idParent, queryString);

                    //Cerco tutti i fascicoli procedimentali chiusi e in stato in deposito, associati al fascicolo generale selezionato 
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASCICOLI_SCARTO_PAGING");
                    queryDef.setParam("idPeo", infoUtente.idPeople);
                    queryDef.setParam("idGrp", infoUtente.idGruppo);
                    queryDef.setParam("idAmm", infoUtente.idAmministrazione);
                    queryDef.setParam("idParent", idParent);

                    queryDef.setParam("dtaAp", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                    queryDef.setParam("dtaCh", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));

                    string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                    if (string.IsNullOrEmpty(idRuoloPubblico))
                        idRuoloPubblico = "0";
                    queryDef.setParam("idRuoloPubblico", idRuoloPubblico);

                    queryString = queryDef.getSQL();

                    DataSet dataSet = new DataSet();

                    this.ExecuteQuery(out dataSet, "FASCICOLI", queryString);
                    if (dataSet != null && dataSet.Tables[0] != null)
                    {
                        foreach (System.Data.DataRow dataRow in dataSet.Tables[0].Rows)
                        {
                            DocsPaVO.fascicolazione.Fascicolo fasc = GetFascicolo(infoUtente, dataSet, dataRow, false);
                            listaFascicoli.Add(fasc);
                        }
                    }
                    dataSet.Dispose();
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione Area Scarto (metodo: GetAllFascicoliByCODInDeposito)");
                listaFascicoli = null;
            }

            return listaFascicoli;
        }

        public System.Collections.ArrayList GetListaFascicoliInScarto(DocsPaVO.utente.InfoUtente infoUtente,
            DocsPaVO.AreaScarto.InfoScarto infoScarto,
            int numPage, out int numTotPage, out int totalRecordCount)
        {
            logger.Debug("GetListaFascicoliInScarto");
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

            numTotPage = 0;
            totalRecordCount = 0;
            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    string idParent = "";
                    DocsPaUtils.Query queryDef;
                    string queryString = "";
                    DataSet dataSet;
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_FASC_IN_ISTANZA");
                    queryDef.setParam("idPeo", infoUtente.idPeople);
                    queryDef.setParam("idGrp", infoUtente.idGruppo);

                    string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                    if (string.IsNullOrEmpty(idRuoloPubblico))
                        idRuoloPubblico = "0";
                    queryDef.setParam("idRuoloPubblico", idRuoloPubblico);

                    queryDef.setParam("idAmm", infoUtente.idAmministrazione);
                    queryDef.setParam("dtaAp", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                    queryDef.setParam("dtaCh", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
                    queryDef.setParam("idScarto", infoScarto.systemID);
                    queryString = queryDef.getSQL();



                    this.ExecutePaging(out dataSet, out numTotPage, out totalRecordCount, numPage, 10, queryString, "PROFILE");
                    //creazione della lista oggetti
                    dataSet.Tables["PagingTable"].TableName = "FASCICOLI";
                    totalRecordCount = dataSet.Tables["FASCICOLI"].Rows.Count;
                    if (dataSet != null && dataSet.Tables[0] != null)
                    {
                        foreach (System.Data.DataRow dataRow in dataSet.Tables[0].Rows)
                        {
                            DocsPaVO.fascicolazione.Fascicolo fasc = GetFascicolo(infoUtente, dataSet, dataRow, false);
                            listaFascicoli.Add(fasc);
                        }
                    }
                    dataSet.Dispose();
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione Area Scarto (metodo: GetListaFascicoliInScarto)");
                listaFascicoli = null;
            }

            return listaFascicoli;
        }

        public System.Collections.ArrayList GetListaFascicoliInScartoNoPaging(DocsPaVO.utente.InfoUtente infoUtente,
           DocsPaVO.AreaScarto.InfoScarto infoScarto)
        {
            logger.Debug("GetListaFascicoliInScartoNoPaging");
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

            try
            {
                using (DBProvider dbProvider = new DBProvider())
                {
                    DocsPaUtils.Query queryDef;
                    string queryString = "";
                    DataSet dataSet = new DataSet();
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_FASC_IN_ISTANZA");
                    queryDef.setParam("idPeo", infoUtente.idPeople);
                    queryDef.setParam("idGrp", infoUtente.idGruppo);
                    queryDef.setParam("idAmm", infoUtente.idAmministrazione);
                    queryDef.setParam("dtaAp", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                    queryDef.setParam("dtaCh", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
                    queryDef.setParam("idScarto", infoScarto.systemID);
                    queryString = queryDef.getSQL();

                    this.ExecuteQuery(dataSet, "FASCICOLI", queryString);

                    //creazione della lista oggetti
                    if (dataSet != null && dataSet.Tables[0] != null)
                    {
                        foreach (System.Data.DataRow dataRow in dataSet.Tables[0].Rows)
                        {
                            DocsPaVO.fascicolazione.Fascicolo fasc = GetFascicolo(infoUtente, dataSet, dataRow, false);
                            listaFascicoli.Add(fasc);
                        }
                    }
                    dataSet.Dispose();
                }
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Scarto (metodo: GetListaFascicoliInScartoNoPaging)");
                listaFascicoli = null;
            }

            return listaFascicoli;
        }

        public bool CambiaStatoScarto(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.AreaScarto.InfoScarto infoScarto, string nuovoCampo)
        {
            bool result = true;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_CambiaStatoScarto");
                if (string.IsNullOrEmpty(nuovoCampo))
                {
                    if (infoScarto.stato == "S")
                    {
                        q.setParam("stato", "'" + infoScarto.stato + "', DATA_SCARTO = sysdate ");
                    }
                    else
                        q.setParam("stato", "'" + infoScarto.stato + "'");
                }
                else
                {
                    if (nuovoCampo.Equals("estremi_richiesta"))
                    {
                        q.setParam("stato", "'" + infoScarto.stato + "', var_estremi_richiesta = '" + infoScarto.estremi_richiesta + "'");
                    }
                    if (nuovoCampo.Equals("estremi_autorizzazione"))
                    {
                        q.setParam("stato", "'" + infoScarto.stato + "', var_estremi_autoriz = '" + infoScarto.estremi_autorizzazione + "'");
                    }
                }

                q.setParam("sysId", infoScarto.systemID);

                string queryString = q.getSQL();
                this.ExecuteNonQuery(queryString);

                if (infoScarto.stato == "S")
                {
                    //in caso di stato ad S vuol dire che si devono eliminare fisicamente tutti i fascicoli procedimentali chiusi appartenenti all'istanza
                    //in question
                }
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Scarto (metodo: CambiaStatoScarto)");
                result = false;

            }
            return result;
        }

        public bool UpdateScarto(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.AreaScarto.InfoScarto infoScarto)
        {
            bool result = true;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPAAreaScarto");

                q.setParam("desc", infoScarto.descrizione);
                q.setParam("note", infoScarto.note);
                q.setParam("sysId", infoScarto.systemID);
                q.setParam("stato", "A");
                string queryString = q.getSQL();
                this.ExecuteNonQuery(queryString);
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Scarto (metodo: UpdateScarto)");
                result = false;

            }
            return result;
        }


        //Verifica che non ci sia già una nuova istanza di scarto per un dato utente e gruppo
        //OK
        public int isPrimaIstanzaScarto(string idPeople, string idGruppo)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAAreaScarto");
            int result = 0;
            try
            {
                q.setParam("idPeople", idPeople);
                q.setParam("idGruppo", idGruppo);
                string commandText = q.getSQL();
                DataSet ds = new DataSet();
                this.ExecuteQuery(ds, commandText);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    result = 0;
                }
                else
                {
                    result = 1;
                }
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Scarto (metodo: isPrimaIstanzaScarto)");
                result = -1;
            }
            return result;
        }

        public bool addAllFascAreaScarto(DocsPaVO.fascicolazione.Fascicolo fascicolo, DocsPaVO.utente.InfoUtente infoUtente, string tipoRic)
        {
            bool result = true;

            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();
            if (tipoRic == "C")
                listaFascicoli = this.GetAllFascicoliByCODInDeposito(infoUtente, fascicolo);
            else
                listaFascicoli = this.GetALLFascicoliInDeposito(infoUtente, tipoRic);
            foreach (DocsPaVO.fascicolazione.Fascicolo fasc in listaFascicoli)
            {
                //Recupero i documenti nel fascicolo
                List<SearchResultInfo> tempList = getIdDocFasc(fasc.systemID);
                ArrayList docs = new ArrayList();
                foreach (SearchResultInfo temp in tempList) docs.Add(temp.Id);
                //for (int i = 0; i < docs.Length; i++)
                foreach (string s in docs)
                {
                    //per ogni documento richiamo addDocumentiFascAreaScarto
                    DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();
                    DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
                    schedaDoc = doc.GetDettaglio(infoUtente, s, "", false);
                    string sysId = this.addDocAreaScarto(schedaDoc.systemId, fasc.systemID, schedaDoc.docNumber, infoUtente, fasc.codice);
                    if (string.IsNullOrEmpty(sysId))
                    {
                        result = false;
                        return result;
                    }
                }
            }
            return result;
        }

        //Aggiunge il documenti di un singolo fascicolo all'area di scarto
        //OK
        public string addDocumentiFascAreaScarto(string idProfile, string idProject, string docNumber, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string result = String.Empty;
            if (idProject != null && idProject != String.Empty)
            {
                DocsPaDB.Query_DocsPAWS.Fascicoli fascicoli = new DocsPaDB.Query_DocsPAWS.Fascicoli();
                DocsPaVO.fascicolazione.Fascicolo fasc = fascicoli.GetFascicoloById(idProject, infoUtente);
                result = this.addDocAreaScarto(idProfile, idProject, docNumber, infoUtente, fasc.codice);
            }
            return result;
        }

        //Aggiunge il singolo documento di un fascicolo all'area di scarto
        //OK
        public string addDocAreaScarto(string idProfile, string idProject, string docNumber, DocsPaVO.utente.InfoUtente infoUtente, string codFasc)
        {
            DocsPaDB.Query_DocsPAWS.Documenti documenti = new DocsPaDB.Query_DocsPAWS.Documenti();
            DocsPaVO.documento.SchedaDocumento schedaDoc = documenti.GetDettaglio(infoUtente, idProfile, docNumber, true);
            ArrayList parameters = new ArrayList();
            string result = String.Empty;
            DBProvider dbProvider = new DBProvider();
            try
            {
                parameters.Add(this.CreateParameter("idAmm", Convert.ToInt32(infoUtente.idAmministrazione)));
                parameters.Add(this.CreateParameter("idPeople", Convert.ToInt32(infoUtente.idPeople)));
                parameters.Add(this.CreateParameter("idProfile", Convert.ToInt32(idProfile)));
                parameters.Add(this.CreateParameter("idProject", Convert.ToInt32(idProject)));
                parameters.Add(this.CreateParameter("oggetto", schedaDoc.oggetto.descrizione));
                parameters.Add(this.CreateParameter("tipoDoc", schedaDoc.tipoProto));
                parameters.Add(this.CreateParameter("idGruppo", Convert.ToInt32(infoUtente.idGruppo)));
                if (schedaDoc.registro != null)
                {
                    if (schedaDoc.registro.systemId != String.Empty)
                    {
                        parameters.Add(this.CreateParameter("idRegistro", Convert.ToInt32(schedaDoc.registro.systemId)));
                    }
                }
                else
                {
                    parameters.Add(this.CreateParameter("idRegistro", DBNull.Value));
                }

                DocsPaUtils.Data.ParameterSP versionIdParam = new DocsPaUtils.Data.ParameterSP("result", 0, 0, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.Int32);
                parameters.Add(versionIdParam);

                DataSet ds = null;
                if (dbProvider.ExecuteStoredProcedure("SP_INSERT_AREA_SCARTO", parameters, ds) != -1)
                {
                    result = Convert.ToString(versionIdParam.Valore);
                }
                else
                {
                    result = "-1";
                }
            }
            catch
            {
                result = "-1";
            }
            finally
            {
                dbProvider.Dispose();
            }
            return result;
        }

        //Elimina i documenti di un fascicolo dall'istanza dell'area di scarto
        //OK
        public bool DeleteAreaScarto(string idProject, string idIstanza, bool deleteIstanza, string systemId)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPAItemsScarto");
            bool result = true;
            try
            {
                string query = "";
                //elimino l'intero fascicolo con i suoi documenti dall'istanza di scarto
                if (idProject != null && idProject != String.Empty)
                    query = " ID_PROJECT=" + idProject + " AND CHA_STATO='" + "N'";

                if (!string.IsNullOrEmpty(systemId))
                    query = " SYSTEM_ID=" + systemId;

                q.setParam("param", query);
                string queryString = q.getSQL();
                this.ExecuteNonQuery(queryString);

                if (deleteIstanza)
                {
                    DocsPaUtils.Query qDelete = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPAAreaConservazione");
                    qDelete.setParam("idIstanza", "'" + idIstanza + "'");
                    this.ExecuteNonQuery(qDelete.getSQL());
                }
            }
            catch
            {
                logger.Debug("Errore nella gestione Area Scarto (metodo: DeleteAreaScarto)");
                result = false;

            }
            return result;
        }

        public System.Collections.ArrayList GetDocumentiDaScartare(
              string idGruppo,
              string idPeople, string fascId,
              DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            System.Collections.ArrayList listaDocumenti = new System.Collections.ArrayList();

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DOCUMENTI_INFASCICOLO_Scarto");

                // per SQL
                if (dbType.ToUpper() == "SQL")
                {
                    q.setParam("dbuser", getUserDB());
                }

                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.CREATION_DATE", false));
                if (!string.IsNullOrEmpty(fascId) && fascicolo == null)
                    q.setParam("param2", fascId);
                else
                {
                    DocsPaVO.fascicolazione.Folder folder = GetFolderNOSecurity(idPeople, idGruppo, fascicolo.systemID);
                    q.setParam("param2", folder.systemID);
                }
                q.setParam("param3", idGruppo);
                q.setParam("param4", idPeople);
                string queryString = q.getSQL();
                logger.Debug(queryString);
                DataSet dataSet = new DataSet();
                this.ExecuteQuery(out dataSet, "DOCUMENTI", queryString);

                //creazione della lista oggetti
                // Reperimento oggetto arraylist di oggetti "InfoDocumento"
                Documenti doc = new Documenti();
                listaDocumenti = doc.GetArrayListDocumenti(dataSet.Tables["DOCUMENTI"], true);
                doc.Dispose();
                doc = null;
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("F_System");

                listaDocumenti = null;
            }

            return listaDocumenti;
        }

        public ArrayList getIdDocSottoFasc(string idFascicolo)
        {
            System.Data.DataSet ds = new DataSet();
            System.Collections.ArrayList lista = new System.Collections.ArrayList();
            try
            {
                //string queryFolderString = "";

                //DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project10");

                //q.setParam("param1", idFascicolo);

                //queryFolderString = q.getSQL();
                //this.ExecuteQuery(ds, "FOLDER", queryFolderString);

                //string queryDocString = "";
                string DocString = "";
                DocsPaUtils.Query qSelect = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponents3");

                //for (int j = 0; j < ds.Tables["FOLDER"].Rows.Count; j++)
                //{
                //    queryDocString = queryDocString + ds.Tables["FOLDER"].Rows[j]["SYSTEM_ID"].ToString();
                //    if (j < ds.Tables["FOLDER"].Rows.Count - 1)
                //    {
                //        queryDocString = queryDocString + ",";
                //    }
                //}

                qSelect.setParam("param1", idFascicolo);
                DocString = qSelect.getSQL();

                this.ExecuteQuery(ds, "DOC", DocString);
                for (int k = 0; k < ds.Tables["DOC"].Rows.Count; k++)
                {
                    lista.Add(ds.Tables["DOC"].Rows[k]["LINK"].ToString());
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                lista = null;
            }

            return lista;
        }


        //Elimina un fascicolo fisicamente
        public bool EliminaFasc(DocsPaVO.utente.InfoUtente infoUtente, string fascId)
        {
            bool result = false;
            DocsPaUtils.Query q;
            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DBProvider())
                {
                    result = EliminaFascicolo(infoUtente, fascId, dbProvider);
                }
            }
            catch
            {
                logger.Debug("Errore nella gestione dei fascicoli (Query - EliminaFasc)");
                throw new Exception("F_System");
            }
            return result;
        }

        //Elimina fisicamente un singolo fascicolo
        private bool EliminaFascicolo(DocsPaVO.utente.InfoUtente infoUtente, string fascId, DBProvider dbProvider)
        {
            bool result = false;
            try
            {
                ArrayList parameters = new ArrayList();
                parameters.Add(this.CreateParameter("idProject", Convert.ToInt32(fascId)));

                int resultSP = dbProvider.ExecuteStoreProcedure("SP_RIMUOVI_FASCICOLI", parameters);
                result = Convert.ToBoolean(resultSP);

                logger.Debug("Chiamata SP 'SP_RIMUOVI_FASCICOLI'. Esito: " + Convert.ToString(result));

                if (result)
                {
                    logger.Debug("Eseguita Commit alla Stored Procedure: SP_RIMUOVI_FASCICOLI");
                }
                else
                {
                    logger.Debug("Eseguita Rollback alla Stored Procedure: SP_RIMUOVI_FASCICOLI");
                }

            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }


        #endregion


        #region AreaConservazione
        //restituisce l'ID di tutti i documenti presenti nel fascicoli compresi quelli dei sottofascicoli
        public List<SearchResultInfo> getIdDocFasc(string idFascicolo)
        {
            System.Data.DataSet ds = new DataSet();
            List<SearchResultInfo> lista = new List<SearchResultInfo>();
            try
            {
                string queryFolderString = "";

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project10");

                q.setParam("param1", idFascicolo);

                queryFolderString = q.getSQL();
                this.ExecuteQuery(ds, "FOLDER", queryFolderString);

                string queryDocString = "";
                string DocString = "";
                DocsPaUtils.Query qSelect = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponents4");

                for (int j = 0; j < ds.Tables["FOLDER"].Rows.Count; j++)
                {
                    queryDocString = queryDocString + ds.Tables["FOLDER"].Rows[j]["SYSTEM_ID"].ToString();
                    if (j < ds.Tables["FOLDER"].Rows.Count - 1)
                    {
                        queryDocString = queryDocString + ",";
                    }
                }
                qSelect.setParam("param1", queryDocString);
                DocString = qSelect.getSQL();

                this.ExecuteQuery(ds, "DOC", DocString);
                for (int k = 0; k < ds.Tables["DOC"].Rows.Count; k++)
                {
                    SearchResultInfo temp = new SearchResultInfo();
                    temp.Id = ds.Tables["DOC"].Rows[k]["LINK"].ToString();
                    temp.Codice = ds.Tables["DOC"].Rows[k]["CODICE"].ToString();
                    lista.Add(temp);
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                lista = null;
            }

            return lista;
        }


        #endregion

        /// <summary>
        /// Reperimento di tutti gli id dei documenti contenuti in un fascicolo
        /// </summary>
        /// <param name="codiceFascicolo">
        /// Codice del fascicolo
        /// </param>
        /// <returns></returns>
        public string[] getIdDocumentiInFascicolo(string codiceFascicolo)
        {
            string commandText = getQueryIdDocumentiInFascicolo(codiceFascicolo);
            logger.Debug(commandText);

            List<string> documenti = new List<string>();

            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader reader = dbProvider.ExecuteReader(commandText))
                    while (reader.Read())
                        documenti.Add(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(reader, "ID_DOC", false).ToString());
            }

            return documenti.ToArray();
        }

        /// <summary>
        /// Reperimento query di tutti gli id dei documenti contenuti in un fascicolo
        /// </summary>
        /// <param name="codiceFascicolo"></param>
        /// <returns></returns>
        public static string getQueryIdDocumentiInFascicolo(string codiceFascicolo)
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_ID_DOCUMENTI_IN_FASCICOLO");
            queryDef.setParam("codiceFascicolo", codiceFascicolo);
            return queryDef.getSQL();
        }

        /// <summary>
        /// Calcolo del protocollo titolario e del riferimento mittente per il documento
        /// </summary>
        /// <param name="codiceFascicolo"></param>
        /// <returns></returns>
        public void creaProtTitRifMitt(DocsPaVO.utente.InfoUtente infoUtente, string idGruppo, string idProfile, string idFolder)
        {
            string protTit = DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableProtocolloTitolario();
            bool rifMit = DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableRiferimentiMittente();

            try
            {
                if (!string.IsNullOrEmpty(protTit) || rifMit)
                {
                    Amministrazione amm = new Amministrazione();

                    //Recupero il folder
                    DocsPaVO.fascicolazione.Folder folder = this.GetFolderById(infoUtente.idPeople, idGruppo, idFolder);

                    //Recupero il fascicolo
                    DocsPaVO.fascicolazione.Fascicolo fascicolo = this.GetFascicoloById(folder.idFascicolo, infoUtente);

                    //Recupero il nodo di titolario
                    DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario = amm.getNodoTitolario(fascicolo.idClassificazione);

                    //Recupero il formato del protocollo titolario
                    DocsPaVO.amministrazione.InfoAmministrazione infoAmm = amm.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione);

                    //Recupero il titolario
                    DocsPaVO.amministrazione.OrgTitolario titolario = amm.getTitolarioById(nodoTitolario.ID_Titolario);

                    //Recupero i dati del documento
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROFILE_PROTO_TIT");
                    q.setParam("param1", " SYSTEM_ID = " + idProfile);
                    string commandText = q.getSQL();
                    logger.Debug("Protocollo Titolario creaProtTitRifMitt :" + commandText);
                    DataSet ds = new DataSet();
                    this.ExecuteQuery(ds, commandText);

                    //Protocollo Titolario Attivo
                    //Creo il Protocollo Titolario e il Riferimento Mittente che sarà un protocollo di titolario senza sottonumero progressivo
                    if (!string.IsNullOrEmpty(protTit))
                    {
                        //Controllo se non esiste già un protocollo titolario per il documento
                        if (ds.Tables[0].Rows.Count > 0 && string.IsNullOrEmpty(ds.Tables[0].Rows[0]["PROT_TIT"].ToString()))
                        {
                            //Procedo solo se esistono le condizione per generare un protocollo di titolario
                            if (folder != null &&
                                fascicolo != null &&
                                nodoTitolario != null &&
                                titolario != null &&
                                infoAmm != null &&
                                !string.IsNullOrEmpty(nodoTitolario.numProtoTit) &&
                                !string.IsNullOrEmpty(fascicolo.numFascicolo) &&
                                //!string.IsNullOrEmpty(nodoTitolario.dataCreazione) &&
                                //!string.IsNullOrEmpty(numDocInFasc) &&
                                !string.IsNullOrEmpty(infoAmm.formatoProtTitolario)
                                )
                            {
                                //Calcolo il numero di documenti con protocollo titolario presenti nel fascicolo
                                string numDocInFasc = string.Empty;
                                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROTO_TIT_NUM_DOC_IN_FASC");
                                q.setParam("idFasc", fascicolo.systemID);
                                commandText = q.getSQL();
                                logger.Debug("Protocollo Titolario creaProtTitRifMitt :" + commandText);
                                DataSet dsNumDocInFasc = new DataSet();
                                this.ExecuteQuery(dsNumDocInFasc, commandText);
                                if (dsNumDocInFasc.Tables[0].Rows.Count > 0 && !string.IsNullOrEmpty(dsNumDocInFasc.Tables[0].Rows[0]["NUM_DOC_IN_FASC"].ToString()))
                                {
                                    //Incremento e aggiorno il numero di documenti in un fascicolo
                                    int docInFasc = Convert.ToInt32(dsNumDocInFasc.Tables[0].Rows[0]["NUM_DOC_IN_FASC"].ToString());
                                    numDocInFasc = docInFasc.ToString();
                                    docInFasc++;
                                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROTO_TIT_NUM_DOC_IN_FASC");
                                    q.setParam("idFasc", fascicolo.systemID);
                                    q.setParam("numDocInFasc", docInFasc.ToString());
                                    commandText = q.getSQL();
                                    logger.Debug("Protocollo Titolario creaProtTitRifMitt :" + commandText);
                                    this.ExecuteNonQuery(commandText);
                                }
                                else
                                {
                                    //Creo per il fascicolo il contatore dei documenti contenuti in esso
                                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_PROTO_TIT_NUM_DOC_IN_FASC");
                                    q.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                                    q.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_PROTO_TIT"));
                                    q.setParam("idAmm", infoAmm.IDAmm);
                                    q.setParam("idFasc", fascicolo.systemID);
                                    q.setParam("numDocInFasc", "1");
                                    commandText = q.getSQL();
                                    logger.Debug("Protocollo Titolario creaProtTitRifMitt :" + commandText);
                                    this.ExecuteNonQuery(commandText);
                                    numDocInFasc = "0";
                                }

                                /*
                                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROJC_PROTO_TIT");
                                q.setParam("projectId", folder.systemID);
                                commandText = q.getSQL();
                                logger.Debug("Protocollo Titolario creaProtTitRifMitt :" + commandText);
                                DataSet dsNumDocInFasc = new DataSet();
                                this.ExecuteQuery(dsNumDocInFasc,commandText);
                                string numDocInFasc = string.Empty;
                                if(dsNumDocInFasc.Tables[0].Rows.Count > 0)
                                    numDocInFasc = dsNumDocInFasc.Tables[0].Rows.Count.ToString();
                                else
                                    numDocInFasc = "0";
                                

                                //Recupero anno di creazione nodo
                                string anno = string.Empty;
                                DateTime dataCreazione = DocsPaUtils.Functions.Functions.ToDate(nodoTitolario.dataCreazione);                            
                                */

                                //Creazione protocollo titolario
                                string protocolloTitolario = infoAmm.formatoProtTitolario;
                                protocolloTitolario = protocolloTitolario.Replace("CONT_TIT", nodoTitolario.numProtoTit);
                                protocolloTitolario = protocolloTitolario.Replace("NUM_FASC", fascicolo.numFascicolo);
                                protocolloTitolario = protocolloTitolario.Replace("PROG_DOC", numDocInFasc);
                                protocolloTitolario = protocolloTitolario.Replace("DESC_TIT", titolario.DescrizioneLite);
                                protocolloTitolario = protocolloTitolario.Replace("COD_TITOLO", nodoTitolario.Codice);
                                protocolloTitolario = protocolloTitolario.Replace("DATA_COMP", ds.Tables[0].Rows[0]["DTA_PROTO"].ToString());
                                protocolloTitolario = protocolloTitolario.Replace("DATA_ANNO", ds.Tables[0].Rows[0]["NUM_ANNO_PROTO"].ToString());

                                //Provvedo ad inserire il protocollo di titolario per il documento in questione
                                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROFILE_PROTO_TIT");
                                q.setParam("protoTitolario", protocolloTitolario);
                                q.setParam("numInFasc", numDocInFasc);
                                q.setParam("idFascProtTit", fascicolo.systemID);
                                q.setParam("numProtTit", nodoTitolario.numProtoTit);
                                q.setParam("idTitolario", nodoTitolario.ID_Titolario);
                                q.setParam("dataProtoTit", DocsPaDbManagement.Functions.Functions.GetDate());
                                q.setParam("param1", " SYSTEM_ID = " + idProfile);
                                commandText = q.getSQL();
                                logger.Debug("Protocollo Titolario creaProtTitRifMitt :" + commandText);
                                this.ExecuteNonQuery(commandText);

                                //Provvedo ad inserire il protocollo titolario nella project components
                                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJC_PROTO_TIT");
                                q.setParam("protoTit", protocolloTitolario);
                                q.setParam("param1", " LINK = " + idProfile + " AND PROJECT_ID = " + folder.systemID);
                                commandText = q.getSQL();
                                logger.Debug("Protocollo Titolario creaProtTitRifMitt :" + commandText);
                                this.ExecuteNonQuery(commandText);

                                //Solo se il protocollo è in uscita
                                if (ds.Tables[0].Rows[0]["CHA_TIPO_PROTO"].ToString() == "P")
                                {
                                    //Creazione del protocollo di titolario senza progressivo documento per il riferimento mittente
                                    string protocolloTitolarioNoSottonumero = infoAmm.formatoProtTitolario;
                                    protocolloTitolarioNoSottonumero = protocolloTitolarioNoSottonumero.Replace("CONT_TIT", nodoTitolario.numProtoTit);
                                    protocolloTitolarioNoSottonumero = protocolloTitolarioNoSottonumero.Replace("NUM_FASC", fascicolo.numFascicolo);
                                    protocolloTitolarioNoSottonumero = protocolloTitolarioNoSottonumero.Replace("PROG_DOC", "PROG_DOC");
                                    protocolloTitolarioNoSottonumero = protocolloTitolarioNoSottonumero.Replace("DESC_TIT", titolario.DescrizioneLite);
                                    protocolloTitolarioNoSottonumero = protocolloTitolarioNoSottonumero.Replace("COD_TITOLO", nodoTitolario.Codice);
                                    protocolloTitolarioNoSottonumero = protocolloTitolarioNoSottonumero.Replace("DATA_COMP", ds.Tables[0].Rows[0]["DTA_PROTO"].ToString());
                                    protocolloTitolarioNoSottonumero = protocolloTitolarioNoSottonumero.Replace("DATA_ANNO", ds.Tables[0].Rows[0]["NUM_ANNO_PROTO"].ToString());
                                    int indexOfPROG_DOC = protocolloTitolarioNoSottonumero.IndexOf("PROG_DOC");
                                    if (indexOfPROG_DOC != -1)
                                    {
                                        string stringaDaEliminare = string.Empty;
                                        if (indexOfPROG_DOC == 0)
                                            stringaDaEliminare = protocolloTitolarioNoSottonumero.Substring(indexOfPROG_DOC, 9);
                                        else
                                            stringaDaEliminare = protocolloTitolarioNoSottonumero.Substring(indexOfPROG_DOC - 1, 9);
                                        protocolloTitolarioNoSottonumero = protocolloTitolarioNoSottonumero.Replace(stringaDaEliminare, "");
                                    }

                                    //Provvedo ad inserire il riferimento mittente per il documento in questione
                                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROFILE_RIF_MIT");
                                    q.setParam("riffMitt", protocolloTitolarioNoSottonumero);
                                    q.setParam("param1", " SYSTEM_ID = " + idProfile);
                                    commandText = q.getSQL();
                                    logger.Debug("Protocollo Titolario creaProtTitRifMitt :" + commandText);
                                    this.ExecuteNonQuery(commandText);
                                }
                            }
                        }
                    }

                    //Riferimento Mittente attivo
                    //Creo il riferimento mittente solo se il protocollo di titolario non è attivo perchè altrimenti l'ho creato sopra
                    //e sarebbe il protocollo di titolario senza sottonumero ed il tipo di protocollo è in uscita
                    if (rifMit && string.IsNullOrEmpty(protTit) && ds.Tables[0].Rows[0]["CHA_TIPO_PROTO"].ToString() == "P")
                    {
                        //Procedo solo se esistono le condizione per generare un riferimento mittente
                        if (fascicolo != null &&
                            nodoTitolario != null &&
                            !string.IsNullOrEmpty(fascicolo.numFascicolo) &&
                            !string.IsNullOrEmpty(nodoTitolario.dataCreazione) &&
                            !string.IsNullOrEmpty(infoAmm.formatoProtTitolario)
                            )
                        {
                            //Recupero anno di creazione nodo
                            string anno = string.Empty;
                            DateTime dataCreazione = DocsPaUtils.Functions.Functions.ToDate(nodoTitolario.dataCreazione);

                            //Creazione riferimento mittente
                            string riferimentoMittente = infoAmm.formatoProtTitolario;
                            //Eventualmente elimino il PROG_DOC, CONT_TIT, DESC_TIT dalla stringa di segnatura
                            int indexOfPROG_DOC = riferimentoMittente.IndexOf("PROG_DOC");

                            string stringaDaEliminare = string.Empty;
                            if (indexOfPROG_DOC != -1)
                            {
                                if (indexOfPROG_DOC == 0)
                                    stringaDaEliminare = riferimentoMittente.Substring(indexOfPROG_DOC, 9);
                                else
                                    stringaDaEliminare = riferimentoMittente.Substring(indexOfPROG_DOC - 1, 9);
                                riferimentoMittente = riferimentoMittente.Replace(stringaDaEliminare, "");
                            }
                            int indexOfCONT_TIT = riferimentoMittente.IndexOf("CONT_TIT");
                            if (indexOfCONT_TIT != -1)
                            {
                                if (indexOfCONT_TIT == 0)
                                    stringaDaEliminare = riferimentoMittente.Substring(indexOfCONT_TIT, 9);
                                else
                                    stringaDaEliminare = riferimentoMittente.Substring(indexOfCONT_TIT - 1, 9);
                                riferimentoMittente = riferimentoMittente.Replace(stringaDaEliminare, "");
                            }
                            int indexOfDESC_TIT = riferimentoMittente.IndexOf("DESC_TIT");
                            if (indexOfDESC_TIT != -1)
                            {
                                if (indexOfDESC_TIT == 0)
                                    stringaDaEliminare = riferimentoMittente.Substring(indexOfDESC_TIT, 9);
                                else
                                    stringaDaEliminare = riferimentoMittente.Substring(indexOfDESC_TIT - 1, 9);
                                riferimentoMittente = riferimentoMittente.Replace(stringaDaEliminare, "");
                            }

                            riferimentoMittente = riferimentoMittente.Replace("COD_TITOLO", nodoTitolario.Codice);
                            riferimentoMittente = riferimentoMittente.Replace("DATA_ANNO", ds.Tables[0].Rows[0]["NUM_ANNO_PROTO"].ToString());
                            riferimentoMittente = riferimentoMittente.Replace("DATA_COMP", ds.Tables[0].Rows[0]["DTA_PROTO"].ToString());
                            riferimentoMittente = riferimentoMittente.Replace("NUM_FASC", fascicolo.numFascicolo);

                            //Provvedo ad inserire il riferimento mittente per il documento in questione
                            q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROFILE_RIF_MIT");
                            q.setParam("riffMitt", riferimentoMittente);
                            q.setParam("param1", " SYSTEM_ID = " + idProfile);
                            commandText = q.getSQL();
                            logger.Debug("Protocollo Titolario creaProtTitRifMitt :" + commandText);
                            this.ExecuteNonQuery(commandText);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore creaProtTitRifMitt : " + e.Message);
            }
        }

        /// <summary>
        /// Rimozione della classifica ed eventuale protocollo titolario e riferimento mittente di un documento
        /// </summary>
        /// <param name="codiceFascicolo"></param>
        /// <returns></returns>
        public void eliminaProtTitRifMitt(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Folder folder, string idProfile)
        {
            string protTit = DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableProtocolloTitolario();
            bool rifMit = DocsPaUtils.Configuration.CustomConfigurationBaseManager.isEnableRiferimentiMittente();

            try
            {
                if (!string.IsNullOrEmpty(protTit) || rifMit)
                {
                    Documenti doc = new Documenti();
                    Amministrazione amm = new Amministrazione();

                    //Recupero la scheda documento
                    DocsPaVO.documento.SchedaDocumento schedaDoc = doc.GetSchedaDocumentoByID(infoUtente, idProfile);

                    //Protocollo Titolario Attivo
                    if (!string.IsNullOrEmpty(protTit))
                    {
                        //Procedo solo se esistono le condizione per eliminare un protocollo di titolario
                        if (schedaDoc != null &&
                            !string.IsNullOrEmpty(folder.idFascicolo) &&
                            !string.IsNullOrEmpty(schedaDoc.protocolloTitolario) &&
                            !string.IsNullOrEmpty(schedaDoc.numInFasc)
                            )
                        {
                            //Verifico che effettivamente si sta eliminando la classifica che ha generato il protocollo titolario
                            if (schedaDoc.idFascProtoTit == folder.idFascicolo)
                            {
                                //Provvedo ad eliminare il protocollo di titolario per il documento in questione
                                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROFILE_PROTO_TIT");
                                q.setParam("protoTitolario", "");
                                q.setParam("numInFasc", "null");
                                q.setParam("idFascProtTit", "null");
                                q.setParam("numProtTit", "null");
                                q.setParam("idTitolario", "null");
                                q.setParam("param1", " SYSTEM_ID = " + idProfile);
                                q.setParam("dataProtoTit", "null");
                                string commandText = q.getSQL();
                                logger.Debug("Protocollo Titolario eliminaProtTitRifMitt :" + commandText);
                                this.ExecuteNonQuery(commandText);

                                //Provvedo ad eliminare il protocollo titolario nella project components
                                q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJC_PROTO_TIT");
                                q.setParam("protoTit", "");
                                q.setParam("param1", " LINK = " + idProfile + " AND PROJECT_ID = " + folder.systemID);
                                commandText = q.getSQL();
                                logger.Debug("Protocollo Titolario eliminaProtTitRifMitt :" + commandText);
                                this.ExecuteNonQuery(commandText);

                                //Decremento il numero di documenti contenuti nel fascicolo
                                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROTO_TIT_NUM_DOC_IN_FASC");
                                q.setParam("idFasc", folder.idFascicolo);
                                commandText = q.getSQL();
                                logger.Debug("Protocollo Titolario eliminaProtTitRifMitt :" + commandText);
                                DataSet ds = new DataSet();
                                this.ExecuteQuery(ds, commandText);
                                if (ds.Tables[0].Rows.Count != 0 && !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["NUM_DOC_IN_FASC"].ToString()))
                                {
                                    int maxNumDocInFasc = Convert.ToInt32(ds.Tables[0].Rows[0]["NUM_DOC_IN_FASC"].ToString());
                                    if (maxNumDocInFasc > 0)
                                        maxNumDocInFasc--;
                                    q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROTO_TIT_NUM_DOC_IN_FASC");
                                    q.setParam("idFasc", folder.idFascicolo);
                                    q.setParam("numDocInFasc", maxNumDocInFasc.ToString());
                                    commandText = q.getSQL();
                                    logger.Debug("Protocollo Titolario eliminaProtTitRifMitt :" + commandText);
                                    this.ExecuteNonQuery(commandText);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore eliminaProtTitRifMitt : " + e.Message);
            }
        }

        /// <summary>
        /// Verifico se è possibile rimuovere un fascicolo senza rompere la catena dei sottonumeri
        /// </summary>
        /// <param name="codiceFascicolo"></param>
        /// <returns></returns>
        public DocsPaVO.Validations.ValidationResultInfo canRemoveClassificazione(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, DocsPaVO.fascicolazione.Folder folder)
        {
            DocsPaVO.Validations.ValidationResultInfo result = null;

            try
            {
                Documenti doc = new Documenti();

                //Recupero la scheda documento
                DocsPaVO.documento.SchedaDocumento schedaDoc = doc.GetSchedaDocumentoByID(infoUtente, idProfile);

                //Recupero il fascicolo
                DocsPaVO.fascicolazione.Fascicolo fascicolo = this.GetFascicoloById(folder.idFascicolo, infoUtente);

                //Se effettivamente il documento ha un protocollo titolario procedo?
                if (!string.IsNullOrEmpty(schedaDoc.protocolloTitolario) && fascicolo != null && fascicolo.systemID == schedaDoc.idFascProtoTit)
                {
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROTO_TIT_NUM_DOC_IN_FASC");
                    q.setParam("idFasc", fascicolo.systemID);
                    string commandText = q.getSQL();
                    logger.Debug("Protocollo Titolario canRemoveClassificazione :" + commandText);
                    DataSet ds = new DataSet();
                    this.ExecuteQuery(ds, commandText);

                    if (ds.Tables[0].Rows.Count != 0 && !string.IsNullOrEmpty(ds.Tables[0].Rows[0]["NUM_DOC_IN_FASC"].ToString()))
                    {
                        int maxNumDocInFasc = Convert.ToInt32(ds.Tables[0].Rows[0]["NUM_DOC_IN_FASC"].ToString());
                        int numInFasc = Convert.ToInt32(schedaDoc.numInFasc.ToString());
                        if (numInFasc < maxNumDocInFasc - 1)
                        {
                            //Non posso declassificare il documento perchè nel fascicolo esistono documenti con sottonumeri successivi
                            result = new DocsPaVO.Validations.ValidationResultInfo();
                            DocsPaVO.Validations.BrokenRule br = new DocsPaVO.Validations.BrokenRule("1", "Esistono documenti con sottonumero successivo", DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error);
                            result.BrokenRules.Add(br);
                        }
                    }

                    /*
                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_MAX_NUM_DOC_IN_FASC");
                    q.setParam("idFascicolo", fascicolo.systemID);
                    string commandText = q.getSQL();
                    logger.Debug("Protocollo Titolario canRemoveClassificazione :" + commandText);
                    DataSet ds = new DataSet();
                    this.ExecuteQuery(ds, commandText);

                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        int maxNumDocInFasc = Convert.ToInt32(ds.Tables[0].Rows[0]["MAX_NUM_IN_FASC"].ToString());
                        int numInFasc = Convert.ToInt32(schedaDoc.numInFasc.ToString());
                        if (numInFasc < maxNumDocInFasc)
                        {
                            result = new DocsPaVO.Validations.ValidationResultInfo();
                            DocsPaVO.Validations.BrokenRule br = new DocsPaVO.Validations.BrokenRule("1", "Esistono documenti protocollati con sottonumero successivo", DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error);
                            result.BrokenRules.Add(br);                            
                        }
                    }
                    */
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore canRemoveClassificazione : " + e.Message);
                result = null;
            }

            return result;
        }
        internal static DocsPaVO.fascicolazione.CreatoreFascicolo GetCreatoreFascicolo(DataRow dataRow)
        {
            DocsPaVO.fascicolazione.CreatoreFascicolo objCreatore = new DocsPaVO.fascicolazione.CreatoreFascicolo();
            objCreatore.idPeople = dataRow["AUTHOR"].ToString();
            objCreatore.idCorrGlob_Ruolo = dataRow["ID_RUOLO_CREATORE"].ToString();
            objCreatore.idCorrGlob_UO = dataRow["ID_UO_CREATORE"].ToString();
            if (dataRow.Table.Columns.Contains("ID_PEOPLE_DELEGATO"))
            {
                if (dataRow["ID_PEOPLE_DELEGATO"] != DBNull.Value)
                {
                    objCreatore.idPeopleDelegato = dataRow["ID_PEOPLE_DELEGATO"].ToString();
                }
            }
            if (string.Compare(objCreatore.idCorrGlob_UO.Trim(), string.Empty, true) != 0)
            {
                DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                objCreatore.uo_codiceCorrGlobali = documentale.DOC_GetUoById(objCreatore.idCorrGlob_UO); //recupera il nome della UO
            }

            return objCreatore;
        }


        //modiifca
        private string tipoContatoreTemplates(string idTemplate)
        {
            ModelFasc model = new ModelFasc();
            DocsPaVO.ProfilazioneDinamica.Templates template = model.getTemplateFascById(idTemplate);

            if (template != null && template.ELENCO_OGGETTI != null)
            {

                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                {
                    if (oggettoCustom.DA_VISUALIZZARE_RICERCA.Equals("1"))
                        return oggettoCustom.TIPO_CONTATORE;
                }
            }
            return string.Empty;
        }
        //fine modifica
        
        public void creaRiscontroMittente(DocsPaVO.fascicolazione.RiscontroMittente riscontroMittente)
        {
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    string riscontro = riscontroMittente.riferimentoMittente;
                    string[] splitUno = riscontro.Split('/');
                    if (riscontro.Contains('|') && riscontro.Contains('/'))
                    {
                        if (splitUno.Length > 1)
                        {
                            riscontro = splitUno[0];
                        }
                    }

                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("INSERT_DPA_RISCONTRI_CLASSIFICA");
                    queryMng.setParam("colID", DocsPaDbManagement.Functions.Functions.GetSystemIdColName());
                    queryMng.setParam("id", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_RISCONTRI_CLASSIFICA"));
                    queryMng.setParam("riffMitt", riscontro);
                    queryMng.setParam("idCorrGlob", riscontroMittente.idCorrGlobaliMittente);
                    queryMng.setParam("idTitolarioDest", riscontroMittente.idTitolarioDestinatario);
                    queryMng.setParam("idRegDest", riscontroMittente.idRegistroDestinatario);
                    queryMng.setParam("classDest", riscontroMittente.codClassificaDestinatario);
                    queryMng.setParam("codFascDest", riscontroMittente.codFascicoloDestinatario);
                    queryMng.setParam("protTitDest", riscontroMittente.protocolloTitolarioDestinatario);
                    queryMng.setParam("dtaRiscontro", DocsPaDbManagement.Functions.Functions.GetDate());

                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - creaRiscontroMittente - Fascicoli.cs - QUERY : " + commandText);
                    logger.Debug("SQL - creaRiscontroMittente - Fascicoli.cs - QUERY : " + commandText);
                    dbProvider.ExecuteNonQuery(commandText);
                }
                catch (Exception ex)
                {
                    logger.Debug("Errore nella creazione del riscontro : " + ex.Message);
                    dbProvider.RollbackTransaction();
                }
                finally
                {
                    dbProvider.Dispose();
                }
            }
        }

        public void eliminaRiscontroMittente(DocsPaVO.fascicolazione.RiscontroMittente riscontroMittente)
        {
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("SELECT_DPA_RISCONTRI_CLASSIFICA");
                    queryMng.setParam("idCorrGlob", riscontroMittente.idCorrGlobaliMittente);

                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - eliminaRiscontroMittente - Fascicoli.cs - QUERY : " + commandText);
                    logger.Debug("SQL - eliminaRiscontroMittente - Fascicoli.cs - QUERY : " + commandText);
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            //if (ds.Tables[0].Rows[i]["RIFF_MITT"].ToString() == riscontroMittente.riferimentoMittente)
                            if (confrontaRiscontroMittente(ds.Tables[0].Rows[i], riscontroMittente))
                            {
                                queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("DELETE_DPA_RISCONTRI_CLASSIFICA");
                                queryMng.setParam("systemId", ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString());
                                commandText = queryMng.getSQL();
                                System.Diagnostics.Debug.WriteLine("SQL - eliminaRiscontroMittente - Fascicoli.cs - QUERY : " + commandText);
                                logger.Debug("SQL - eliminaRiscontroMittente - Fascicoli.cs - QUERY : " + commandText);
                                dbProvider.ExecuteNonQuery(commandText);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Debug("Errore nella eliminazione del riscontro : " + ex.Message);
                }
            }
        }

        public DocsPaVO.fascicolazione.RiscontroMittente cercaRiscontroMittente(DocsPaVO.fascicolazione.RiscontroMittente riscontroMittente)
        {
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("SELECT_DPA_RISCONTRI_CLASSIFICA");
                    queryMng.setParam("idCorrGlob", riscontroMittente.idCorrGlobaliMittente);

                    string commandText = queryMng.getSQL();
                    System.Diagnostics.Debug.WriteLine("SQL - cercaRiscontroMittente - Fascicoli.cs - QUERY : " + commandText);
                    logger.Debug("SQL - cercaRiscontroMittente - Fascicoli.cs - QUERY : " + commandText);
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            //if (ds.Tables[0].Rows[i]["RIFF_MITT"].ToString() == riscontroMittente.riferimentoMittente)
                            if (confrontaRiscontroMittente(ds.Tables[0].Rows[i], riscontroMittente))
                            {
                                DocsPaVO.fascicolazione.RiscontroMittente riscontroResult = new DocsPaVO.fascicolazione.RiscontroMittente();
                                riscontroResult.systemId = ds.Tables[0].Rows[i]["SYSTEM_ID"].ToString();
                                riscontroResult.riferimentoMittente = ds.Tables[0].Rows[i]["RIFF_MITT"].ToString();
                                riscontroResult.idCorrGlobaliMittente = ds.Tables[0].Rows[i]["ID_CORR_GLOB"].ToString();
                                riscontroResult.idTitolarioDestinatario = ds.Tables[0].Rows[i]["ID_TITOLARIO_DEST"].ToString();
                                riscontroResult.idRegistroDestinatario = ds.Tables[0].Rows[i]["ID_REG_DEST"].ToString();
                                riscontroResult.codClassificaDestinatario = ds.Tables[0].Rows[i]["CLASS_DEST"].ToString();
                                riscontroResult.codFascicoloDestinatario = ds.Tables[0].Rows[i]["COD_FASC_DEST"].ToString();
                                riscontroResult.protocolloTitolarioDestinatario = ds.Tables[0].Rows[i]["PROT_TIT_DEST"].ToString();
                                riscontroResult.dtaRiscontro = ds.Tables[0].Rows[i]["DTA_RISCONTRO"].ToString();

                                return riscontroResult;
                            }
                        }
                    }
                    return null;

                }
                catch (Exception ex)
                {
                    logger.Debug("Errore nella ricerca del riscontro : " + ex.Message);
                    return null;
                }
            }
        }

        public bool confrontaRiscontroMittente(DataRow rowRiscontro, DocsPaVO.fascicolazione.RiscontroMittente oggRiscontro)
        {
            try
            {
                string[] splitRiscontro = oggRiscontro.riferimentoMittente.Split('$');

                //Sono nel caso di un riscontro per i CC
                if (splitRiscontro.Length == 2 && splitRiscontro[1] == "CC")
                {
                    string[] splitPraticaCC = splitRiscontro[0].Split('\\');
                    if (rowRiscontro["PROT_TIT_DEST"].ToString() == splitPraticaCC[0])
                        return true;
                }

                //Sono nel caso di un riscontro con protocollo titolario
                if (splitRiscontro.Length == 2 && splitRiscontro[1] != "CC")
                {
                    string[] splitRowRiscontro = rowRiscontro["RIFF_MITT"].ToString().Split('$');
                    if (splitRowRiscontro.Length == 2)
                    {
                        if (splitRiscontro[0] == splitRowRiscontro[0])
                            return true;
                    }
                    if (splitRowRiscontro.Length == 1)
                    {
                        if (splitRiscontro[1] == splitRowRiscontro[0])
                            return true;
                    }
                }

                //Riscontro normale
                if (splitRiscontro.Length == 1)
                {
                    string[] splitRowRiscontro = rowRiscontro["RIFF_MITT"].ToString().Split('$');
                    if (splitRowRiscontro.Length == 2)
                    {
                        if (splitRiscontro[0] == splitRowRiscontro[1])
                            return true;
                    }
                    if (splitRowRiscontro.Length == 1)
                    {
                        if (splitRiscontro[0] == splitRowRiscontro[0])
                            return true;
                    }
                }

                //Nel caso di elimia riscontro
                //Sono nel caso di un riscontro per i CC
                if (rowRiscontro["RIFF_MITT"].ToString() == oggRiscontro.riferimentoMittente)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore nel confronto del riscontro mittente : " + ex.Message);
                return false;
            }
        }

        public string getCountFascicoliDocumento(string idProfile)
        {
            {
                string resOut = string.Empty;

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponents6");

                q.setParam("param1", idProfile);
                string query = q.getSQL();
                this.ExecuteScalar(out resOut, query);

                return resOut;
            }
        }

        /// <summary>
        /// Funzione per il reperimento del numero di trasmissioni relative al fascicolo
        /// con id segnalato
        /// </summary>
        /// <param name="idProject">L'id del fascicolo di cui contare le trasmissioni che lo coinvolgono</param>
        /// <returns>Il numero di trasmissioni che coinvolgono il fascicolo con l'id segnalato</returns>
        public int GetCountTrasmissioniFascicolo(int idProject)
        {
            // Il valore da restituire
            int toReturn = 0;

            // Il valore in cui verrà salvato il risultato della query
            string retValue = String.Empty;

            try
            {
                // Reperimento della query per il conteggio delle trasmissioni
                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_COUNT_TRASMISSIONI_FASCICOLO");

                // Impostazione del parametro
                queryDef.setParam("idProject", idProject.ToString());

                // Reperimento della query SQL da eseguire
                string commandText = queryDef.getSQL();
                logger.Debug(commandText);

                // Esecuzione della query
                this.ExecuteScalar(out retValue, commandText);

            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                throw new Exception("Errore in GetCountTrasmissioniDocumento");
            }
            finally
            {
                CloseConnection();
            }

            // Se la query ha restituito un valore valido viene convertito ad intero
            if (!String.IsNullOrEmpty(retValue))
                toReturn = Int32.Parse(retValue);

            // Restituzione del numero di trasmissioni che coinvolgono il fascicolo segnalato
            return toReturn;

        }

        /// <summary>
        /// Prende la lista dei fascicoli in sui è stato fascicolato un documento senza security
        /// </summary>
        /// <param name="infoDocumento"></param>
        /// <param name="sicurezza"></param>
        /// <returns>ArrayList o 'null' se si è verificato un errore</returns>
        public System.Collections.ArrayList GetFascicoliDaDocNoSecurity(DocsPaVO.utente.InfoUtente infoUtente, string idProfile)
        {
            logger.Debug("getFascicoliDaDocNoSecurity");
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT_NO_SECURITY2");

                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
                q.setParam("param3", idProfile);
                q.setParam("param4", infoUtente.idPeople);
                q.setParam("param5", infoUtente.idGruppo);

                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                q.setParam("idRuoloPubblico", !string.IsNullOrEmpty(idRuoloPubblico) ? idRuoloPubblico : "0");

                string queryString = q.getSQL();

                logger.Debug(queryString);

                System.Data.DataSet dataSet;
                this.ExecuteQuery(out dataSet, "PROJECT", queryString);

                foreach (System.Data.DataRow dataRow in dataSet.Tables["PROJECT"].Rows)
                {
                    listaFascicoli.Add(GetFascicolo(infoUtente, dataSet, dataRow));
                }

                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("F_System");

                listaFascicoli = null;
            }

            return listaFascicoli;
        }

        public bool fascIsVisibleByUser(string systemId, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;

            return result;
        }

        public string getAccessRightFascBySystemID(string fascSystemId, DocsPaVO.utente.InfoUtente infoUtente)
        {
            string result = string.Empty;
            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                try
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_ACCESS_RIGHTS");
                    queryDef.setParam("param1", infoUtente.idGruppo);
                    queryDef.setParam("param2", infoUtente.idPeople);
                    queryDef.setParam("param3", fascSystemId);

                    string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                    if (string.IsNullOrEmpty(idRuoloPubblico))
                        idRuoloPubblico = "0";
                    queryDef.setParam("idRuoloPubblico", idRuoloPubblico);

                    string commandText = queryDef.getSQL();
                    logger.Debug(commandText);
                    DataSet ds = new DataSet();
                    dbProvider.ExecuteQuery(ds, commandText);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        result = ds.Tables[0].Rows[0]["ACCESSRIGHTS"].ToString();
                    }
                    ds.Dispose();
                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message);
                }
            }
            return result;
        }

        public string getRootFolderFasc(string idFasc)
        {
            string resOut = string.Empty;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_ROOTFOLDER_FASC");

            q.setParam("param1", idFasc);
            string query = q.getSQL();
            this.ExecuteScalar(out resOut, query);

            return resOut;
        }

        internal static DocsPaVO.fascicolazione.ChiudeFascicolo GetChiudeFascicolo(DataRow dataRow)
        {
            DocsPaVO.fascicolazione.ChiudeFascicolo objChiusura = new DocsPaVO.fascicolazione.ChiudeFascicolo();
            if (dataRow.Table.Columns.Contains("ID_AUTHOR_CHIUSURA"))
            {
                if (dataRow["ID_AUTHOR_CHIUSURA"] != DBNull.Value)
                {
                    objChiusura.idPeople = dataRow["ID_AUTHOR_CHIUSURA"].ToString();
                }
            }
            if (dataRow.Table.Columns.Contains("ID_RUOLO_CHIUSURA"))
            {
                if (dataRow["ID_RUOLO_CHIUSURA"] != DBNull.Value)
                {
                    objChiusura.idCorrGlob_Ruolo = dataRow["ID_RUOLO_CHIUSURA"].ToString();
                }
            }
            if (dataRow.Table.Columns.Contains("ID_UO_CHIUSURA"))
            {
                if (dataRow["ID_UO_CHIUSURA"] != DBNull.Value)
                {
                    objChiusura.idCorrGlob_UO = dataRow["ID_UO_CHIUSURA"].ToString();
                    if (string.Compare(objChiusura.idCorrGlob_UO.Trim(), string.Empty, true) != 0)
                    {
                        DocsPaDB.Query_DocsPAWS.Documentale documentale = new DocsPaDB.Query_DocsPAWS.Documentale();
                        objChiusura.uo_codiceCorrGlobali = documentale.DOC_GetUoById(objChiusura.idCorrGlob_UO); //recupera il nome della UO
                    }
                }
            }

            return objChiusura;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idFolder"></param>
        /// <returns></returns>
        public bool isFascicoloGenerale(DocsPaVO.utente.InfoUtente infoUtente, string idFolder)
        {
            bool retValue = false;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("select cha_tipo_fascicolo from project where system_id in (select id_fascicolo from project where system_id =  {0})", idFolder);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    retValue = (field == "G");
            }

            return retValue;
        }

        /// <summary>
        /// Verifica se il titolario di appartenenza del folder in cui inserire il documento è chiuso o meno
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idFolder">
        /// Id folder
        /// </param>
        /// <returns></returns>
        public bool isTitolarioChiuso(DocsPaVO.utente.InfoUtente infoUtente, string idFolder)
        {
            bool retValue = false;

            // Verifica se il folder in cui fascicolare il documento
            // si riferisce, qualora fascicolo generale, ad un titolario aperto, altrimenti impedisce l'operazione.
            // Se il folder si riferisce ad un fascicolo procedimentale, viene consentita l'operazione (a meno che non sia chiuso)
            string commandText = string.Format("select cha_stato as stato from project where system_id in (select id_titolario from project where system_id in ( select id_fascicolo from project where system_id = {0}) and cha_tipo_fascicolo = 'G')", idFolder);
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                {
                    if (!string.IsNullOrEmpty(field))
                        retValue = (field != "A");
                    else
                        retValue = true;
                }
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idFolder"></param>
        /// <returns></returns>
        public bool isFascicoloProcedimentaleAperto(DocsPaVO.utente.InfoUtente infoUtente, string idFolder)
        {
            bool retValue = false;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("select cha_stato from project where system_id in (select id_fascicolo from project where system_id =  {0}) and cha_tipo_fascicolo = 'P'", idFolder);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    retValue = (field == "A");
            }

            return retValue;
        }

        // <summary>
        /// Questa funzione si occupa di verificare se un fascicolo si trova in ADL per un determinato utente.
        /// Restituisce 1 se il fascicolo si trova in ADL, 0 altrimenti
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns>1 se il fascicolo è in ADL, 0 altrimenti</returns>
        private static string GetIsProjectInADLUtente(string objectId, DocsPaVO.utente.InfoUtente infoUtente)
        {
            // Valore da restituire
            String toReturn = "0";

            using (DBProvider dbProvider = new DBProvider())
            {
                // Prelevamento della query da eseguire
                DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                DocsPaVO.utente.Ruolo newRuolo = new DocsPaVO.utente.Ruolo();
                // newRuolo = utenti.GetRuoloByIdGruppo(infoUtente.idGruppo);

                Query q = InitQuery.getInstance().getQuery("S_DPAAreaLavoro");
                q.setParam("param1", "COUNT(*)");
                string query = "ID_PROJECT = " + objectId + " AND id_people = " + infoUtente.idPeople + "AND id_ruolo_in_uo = " + infoUtente.idCorrGlobali + "";
                q.setParam("param2", query);

                try
                {
                    // Esecuzione della query
                    dbProvider.ExecuteScalar(out toReturn, q.getSQL());
                }
                catch (Exception e) { }

            }

            // Restituzione del valore calcolato
            return toReturn;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="idProject"></param>
        public void GetVisibilitaSemplificata(out System.Data.DataSet ds, string idProject)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SECURITY_PROFILE_SEMPL_FASC");
            q.setParam("param1", idProject);
            string queryStringRuoli = q.getSQL();
            logger.Debug("Query visibilità di un fascicolo semplificata - RUOLI: " + queryStringRuoli);
            this.ExecuteQuery(out ds, "DIRITTI_RUOLI", queryStringRuoli);

        }

        public void GetVisibilita_rimossiSemplificata(out System.Data.DataSet ds, string idProject)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SECURITY_FASC_SEMPL_RUOLI_DELETE");
            q.setParam("param1", idProject);
            string sql = q.getSQL();
            logger.Debug("Query visibilità di un fascicolo - RUOLI RIMOSSI: " + sql);
            this.ExecuteQuery(out ds, "DIRITTI_RUOLI_DELETED", sql);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="idProject"></param>
        public void GetVisibilita1Semplificata(out System.Data.DataSet ds, string idProject)
        {

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SECURITY_PROFILE_SEMPL_FASC_UTENTI");
            q.setParam("param1", idProject);
            string queryStringUtenti = q.getSQL();
            logger.Debug("Query visibilità fascicolo - UTENTI: " + queryStringUtenti);
            this.ExecuteQuery(out ds, "DIRITTI_UTENTI", queryStringUtenti);

        }

        public void GetVisibilita_UtentiRimossiSemplificata(out System.Data.DataSet ds, string idProject)
        {
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SECURITY_PROFILE_SEMPL_FASC_UTENTI_DELETE");
            q.setParam("param1", idProject);
            string queryStringUtenti = q.getSQL();
            logger.Debug("Query visibilità fascicolo - UTENTI RIMOSSI: " + queryStringUtenti);
            this.ExecuteQuery(out ds, "DIRITTI_UTENTI_RIMOSSI", queryStringUtenti);
        }

        private static DocsPaVO.fascicolazione.Fascicolo GetFascicoloLite(DocsPaVO.utente.InfoUtente infoUtente, System.Data.DataSet dataSet, System.Data.DataRow dataRow, bool enableProfilazione)
        {
            DocsPaVO.fascicolazione.Fascicolo objFascicolo = new DocsPaVO.fascicolazione.Fascicolo();

            objFascicolo.systemID = dataRow["SYSTEM_ID"].ToString();
            objFascicolo.apertura = dataRow["DTA_APERTURA"].ToString().Trim();
            objFascicolo.chiusura = dataRow["DTA_CHIUSURA"].ToString().Trim();
            objFascicolo.codice = dataRow["VAR_CODICE"].ToString();
            objFascicolo.descrizione = dataRow["DESCRIPTION"].ToString();
            objFascicolo.stato = dataRow["CHA_STATO"].ToString();
            objFascicolo.tipo = dataRow["CHA_TIPO_FASCICOLO"].ToString();
            objFascicolo.idClassificazione = dataRow["ID_PARENT"].ToString();
            objFascicolo.codUltimo = dataRow["VAR_COD_ULTIMO"].ToString();
            objFascicolo.idRegistroNodoTit = dataRow["ID_REGISTRO"].ToString();

            if (dataRow.Table.Columns.Contains("DTA_SCADENZA"))
                objFascicolo.dtaScadenza = dataRow["DTA_SCADENZA"].ToString().Trim();

            if (dataRow.Table.Columns.Contains("contatore"))
                objFascicolo.contatore = dataRow["contatore"].ToString();

            if (dataRow.Table.Columns.Contains("NUM_MESI_CONSERVAZIONE"))
            {
                objFascicolo.numMesiConservazione = dataRow["NUM_MESI_CONSERVAZIONE"].ToString();
            }

            if (dataRow.Table.Columns.Contains("IN_SCARTO"))
            {
                objFascicolo.inScarto = dataRow["IN_SCARTO"].ToString();
            }

            if (dataRow.Table.Columns.Contains("IN_CONSERVAZIONE"))
            {
                if (dataRow["IN_CONSERVAZIONE"] != DBNull.Value)
                {
                    objFascicolo.inConservazione = dataRow["IN_CONSERVAZIONE"].ToString();
                }
            }

            if (dataRow.Table.Columns.Contains("CHA_IN_ARCHIVIO"))
            {
                if (dataRow["CHA_IN_ARCHIVIO"] != DBNull.Value)
                {
                    objFascicolo.inArchivio = dataRow["CHA_IN_ARCHIVIO"].ToString();
                }
            }

            if (dataRow.Table.Columns.Contains("ID_TITOLARIO"))
                objFascicolo.idTitolario = dataRow["ID_TITOLARIO"].ToString();

            if (dataRow.Table.Columns.Contains("ACCESSRIGHTS"))
                objFascicolo.accessRights = dataRow["ACCESSRIGHTS"].ToString();

            //nuovo per popolare il campo descrizione del registro a cui il fascicolo è associato
            if (objFascicolo.idRegistroNodoTit != null && objFascicolo.idRegistroNodoTit != String.Empty)
            {
                //nuova gestione Paginata
                if (dataRow.Table.Columns.Contains("CODREG"))
                    objFascicolo.codiceRegistroNodoTit = dataRow["CODREG"].ToString();
            }

            // Gestione fascicolo cartaceo
            if (dataRow["CARTACEO"] != DBNull.Value)
            {
                int cartaceo;
                if (Int32.TryParse(dataRow["CARTACEO"].ToString(), out cartaceo))
                    objFascicolo.cartaceo = (cartaceo > 0);
            }

            // Gestione fascicolo privato
            if (dataRow.Table.Columns.Contains("CHA_PRIVATO"))
            {
                if (dataRow["CHA_PRIVATO"] != DBNull.Value)
                {
                    objFascicolo.privato = dataRow["CHA_PRIVATO"].ToString();
                }
                else
                {
                    objFascicolo.privato = null;
                }
            }

            // Gestione fascicolo pubblico
            if (dataRow.Table.Columns.Contains("CHA_PUBBLICO"))
            {
                if (dataRow["CHA_PUBBLICO"] != DBNull.Value)
                {
                    objFascicolo.pubblico = dataRow["CHA_PUBBLICO"].ToString().Equals("1") ? true : false;
                }
                else
                {
                    objFascicolo.pubblico = false;
                }
            }

            if (dataRow.Table.Columns.Contains("CODTIT"))
            {
                if (dataRow["CODTIT"] != DBNull.Value)
                {
                    objFascicolo.codiceGerarchia = dataRow["CODTIT"].ToString();
                }
                else
                {
                    objFascicolo.codiceGerarchia = null;
                }
            }

            //Profilazione dinamica fascicolo
            if (enableProfilazione)
            {
                ModelFasc modelFasc = new ModelFasc();
                objFascicolo.template = modelFasc.getTemplateFascDettagli(objFascicolo.systemID);
            }
            //Fine profilazione dinamica fascicolo

            //Num Fascicolo
            if (dataRow.Table.Columns.Contains("NUM_FASCICOLO"))
                objFascicolo.numFascicolo = dataRow["NUM_FASCICOLO"].ToString();

            if (infoUtente != null)
            {
                // Reperimento dell'ultima nota di visibilità generale inserita nel documento
                GetUltimaNotaFascicolo(objFascicolo, infoUtente);
            }

            // Impostazione del flag per indicare se il fascicolo è in ADL
            //  objFascicolo.InAreaLavoro = GetIsProjectInADL(objFascicolo.systemID);
            //objFascicolo.InAreaLavoro = GetIsProjectInADLUtente(objFascicolo.systemID, infoUtente);
            if (dataRow.Table.Columns.Contains("IN_ADL"))
            {
                objFascicolo.InAreaLavoro = dataRow["IN_ADL"].ToString();
            }
            return objFascicolo;
        }

        public System.Collections.ArrayList getQueryExportFasc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Classificazione objClassificazione, DocsPaVO.filtri.FiltroRicerca[] objListaFiltri, bool enableUffRef, bool enableProfilazione, bool childs, DocsPaVO.utente.Registro registro, byte[] datiExcel, string serverPath, string[] idProjectsList)
        {

            logger.Debug("getListaFascicoli");
            System.Collections.ArrayList listaFascicoli = new System.Collections.ArrayList();

            try
            {
                string queryString = string.Empty;

                if (objClassificazione != null)
                    queryString = GetQueryListaFascicoli(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, objClassificazione.registro, enableUffRef, enableProfilazione, objClassificazione.varcodliv1, childs);
                else
                    queryString = GetQueryFascicoli(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople, registro, enableUffRef, enableProfilazione, null, "R");
                String withClause = String.Empty;
                GetSqlQuery(infoUtente.idGruppo, infoUtente.idPeople, objListaFiltri, false, ref queryString, out withClause);

                //FILTRO EXCEL
                getFiltroExcel(infoUtente.idAmministrazione, ref queryString, datiExcel, serverPath, objListaFiltri);

                // Se la lista dei system id è valorizzata, viene aggiunta un filtro dui system id
                if (idProjectsList != null &&
                    idProjectsList.Length > 0)
                {
                    /*         queryString += " AND A.SYSTEM_ID IN (";

                                   foreach (string id in idProjectsList)
                                       queryString += id + ", ";

                                   queryString = queryString.Remove(queryString.Length - 2);
                                   queryString += ")";*/
                    int i = 0;
                    queryString += " AND ( A.SYSTEM_ID IN(";
                    foreach (string id in idProjectsList)
                    {
                        queryString += id;
                        if (i < idProjectsList.Length - 1)
                        {
                            if (i % 998 == 0 && i > 0)
                            {
                                queryString += ") OR A.SYSTEM_ID IN (";
                            }
                            else
                            {
                                queryString += ", ";
                            }
                        }
                        else
                        {
                            queryString += ")";
                        }
                        i++;
                    }
                    queryString += ")";
                }

                queryString += " ORDER BY A.DTA_CREAZIONE DESC";

                if (!String.IsNullOrEmpty(withClause))
                    queryString = withClause + " " + queryString;

                logger.Debug(queryString);

                System.Data.DataSet dataSet = new System.Data.DataSet();
                this.ExecuteQuery(out dataSet, "PROJECT", queryString);

                //creazione della lista oggetti
                foreach (System.Data.DataRow dataRow in dataSet.Tables["PROJECT"].Rows)
                {
                    listaFascicoli.Add(GetFascicolo(infoUtente, dataSet, dataRow, enableProfilazione));
                }
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                listaFascicoli = null;
            }

            return listaFascicoli;
        }

        /// <summary>
        /// Funzione per il reperimento degli id dei fascicoli e sottofascicoli in
        /// cui è fascicolato un documento
        /// </summary>
        /// <param name="idProfile">Id del documento di cui prelevare gli id dei fascicoli e sottofascicoli</param>
        /// <returns>Lista degli id fascicolo / sottofascicolo</returns>
        public List<FascOrFolderBaseInfo> GetBaseInfoFascOrFolderByIdDoc(String idProfile)
        {
            logger.Debug("GetBaseInfoFascOrFolderByIdDoc");

            // Lista da restituire
            List<FascOrFolderBaseInfo> retVal = new List<FascOrFolderBaseInfo>();

            // Esecuzione query ed estrazione dei dati
            Query q = InitQuery.getInstance().getQuery("S_GET_ID_FASC_OR_SOTTOFASC_DA_ID_DOC");
            q.setParam("idProfile", idProfile);
            string query = q.getSQL();
            logger.Debug("Query recupero informazioni di basei su fascicoli in cui è contenuto un documento:  " + query);
            DataSet dataSet = new DataSet();

            if (this.ExecuteQuery(out dataSet, query) && dataSet.Tables.Count == 1 && dataSet.Tables[0].Rows.Count > 0)
                retVal.AddRange(this.ExtractBaseInfo(dataSet));

            return retVal;
        }

        /// <summary>
        /// Funzione per l'estrazione dei dati di un fascicolo
        /// </summary>
        /// <param name="dataSet">Dataset da cui estrarre i dati</param>
        /// <returns>Collection con le informazioni</returns>
        private IEnumerable<FascOrFolderBaseInfo> ExtractBaseInfo(DataSet dataSet)
        {
            List<FascOrFolderBaseInfo> retVal = new List<FascOrFolderBaseInfo>();

            foreach (DataRow row in dataSet.Tables[0].Rows)
                retVal.Add(new FascOrFolderBaseInfo()
                    {
                        Id = row["ID_FASC_OR_SOTTOFASC"].ToString(),
                        Type = row["FAR_OR_SOTTOFASC"].ToString() == "F" ? FascOrFolderBaseInfo.TypeEnum.Fascicolo : FascOrFolderBaseInfo.TypeEnum.Folder
                    });

            return retVal;
        }

        //modiifca
        private string tipoContatoreTemplatesDoc(string idTemplate)
        {
            Model model = new Model();
            DocsPaVO.ProfilazioneDinamica.Templates template = model.getTemplateById(idTemplate);

            if (template != null && template.ELENCO_OGGETTI != null)
            {

                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                {
                    if (oggettoCustom.DA_VISUALIZZARE_RICERCA.Equals("1"))
                        return oggettoCustom.TIPO_CONTATORE;
                }
            }
            return string.Empty;
        }

        public ArrayList GetListaFascicoliPagingExportOrder(DocsPaVO.utente.InfoUtente infoUtente,
          DocsPaVO.fascicolazione.Classificazione objClassificazione,
          DocsPaVO.utente.Registro registro,
          DocsPaVO.filtri.FiltroRicerca[] filtriFascicoli,
          bool enableUfficioRef,
          bool enableProfilazione,
          bool childs, String[] idProfileList)
        {
            return this.GetListaFascicoliExportOrder(infoUtente, objClassificazione, registro, filtriFascicoli, null,
                enableUfficioRef, enableProfilazione, childs, idProfileList);
        }

        public ArrayList GetListaFascicoliExportOrder(DocsPaVO.utente.InfoUtente infoUtente,
                           DocsPaVO.fascicolazione.Classificazione objClassificazione,
                           DocsPaVO.utente.Registro registro,
                           DocsPaVO.filtri.FiltroRicerca[] filtriFascicoli,
                           DocsPaVO.filtri.FiltroRicerca[] filtriDocumentiInFascicoli,
                           bool enableUfficioRef,
                           bool enableProfilazione,
                           bool childs, String[] idProfileList)
        {
            logger.Debug("getListaFascicoliPaging");

            ArrayList listaFascicoli = new ArrayList();

            try
            {
                //parametri di queryString Comuni.
                string queryString = "";

                //poi la fill con i filtri
                //procedo con la fill dei dati 
                queryString = setCommonParameterExportOrder(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople,
                                            objClassificazione, registro,
                                            filtriFascicoli, filtriDocumentiInFascicoli, enableUfficioRef,
                                            enableProfilazione, childs, idProfileList);


                DataSet ds = new DataSet();
                using (DBProvider dbProvider = new DBProvider())
                {
                    dbProvider.ExecuteQuery(ds, queryString);

                    if (ds != null && ds.Tables[0] != null)
                    {
                        foreach (System.Data.DataRow dataRow in ds.Tables[0].Rows)
                        {
                            listaFascicoli.Add(GetFascicoloLite(infoUtente, ds, dataRow, enableProfilazione));
                        }
                    }

                    ds.Dispose();
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);

                listaFascicoli = null;
            }

            return listaFascicoli;
        }

        private string setCommonParameterExportOrder(string idAmm,
                                  string idGruppo,
                                  string idPeople,
                                  DocsPaVO.fascicolazione.Classificazione objClassificazione,
                                  DocsPaVO.utente.Registro registro,
                                  DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                    DocsPaVO.filtri.FiltroRicerca[] filtriDocumentiInFascicoli,
                                  bool enableUfficioRef,
                                  bool enableProfilazione,
                                  bool childs, String[] idProfileList)
        {
            try
            {
                DocsPaUtils.Query q = null;
                string queryString = string.Empty;
                int startRow = 0;
                int endRow = 0;
                string filterString = string.Empty;
                string filterString1 = string.Empty;
                bool cons = false;
                bool mancCons = false;
                int sysId = 0;
                int caseDescr = 0;
                bool UOsottoposte = false;

                foreach (DocsPaVO.filtri.FiltroRicerca fil in objListaFiltri)
                {
                    if (fil.argomento.Equals("CONSERVAZIONE") && fil.valore != null)
                    {
                        cons = true;
                        if (fil.valore == "0")
                            mancCons = true;
                    }

                    if (fil.argomento.Equals("DESC_PEOPLE_CREATORE") && fil.valore != null && !fil.valore.Equals(""))
                    {
                        string query = "SELECT ID_PEOPLE FROM DPA_CORR_GLOBALI WHERE UPPER(VAR_DESC_CORR) LIKE '%" + fil.valore.ToUpper() + "%'";
                        using (DBProvider dbProvider = new DBProvider())
                        {
                            string systId = string.Empty;
                            if (dbProvider.ExecuteScalar(out systId, query))
                            {
                                if (!string.IsNullOrEmpty(systId))
                                    sysId = Convert.ToInt32(systId);
                            }
                        }
                        caseDescr = 1;
                    }

                    if (fil.argomento.Equals("DESC_RUOLO_CREATORE") && fil.valore != null && !fil.valore.Equals(""))
                    {
                        string query = "SELECT ID_GRUPPO FROM DPA_CORR_GLOBALI WHERE UPPER(VAR_DESC_CORR) LIKE '%" + fil.valore.ToUpper() + "%'";
                        using (DBProvider dbProvider = new DBProvider())
                        {
                            string systId = string.Empty;
                            if (dbProvider.ExecuteScalar(out systId, query))
                            {
                                if (!string.IsNullOrEmpty(systId))
                                    sysId = Convert.ToInt32(systId);
                            }
                        }
                        caseDescr = 2;

                    }

                    if (fil.argomento.Equals("DESC_UO_CREATORE") && fil.valore != null && !fil.valore.Equals(""))
                    {
                        string query = "SELECT ID_UO FROM DPA_CORR_GLOBALI WHERE UPPER(VAR_DESC_CORR) LIKE '%" + fil.valore.ToUpper() + "%'";
                        using (DBProvider dbProvider = new DBProvider())
                        {
                            string systId = string.Empty;
                            if (dbProvider.ExecuteScalar(out systId, query))
                            {
                                if (!string.IsNullOrEmpty(systId))
                                    sysId = Convert.ToInt32(systId);
                            }
                        }
                        caseDescr = 3;
                    }

                    if (fil.argomento.Equals("UO_SOTTOPOSTE") && fil.valore != null && !fil.valore.Equals(""))
                        UOsottoposte = true;
                }

                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASCICOLI_PAGING_EXPORT");

                if (dbType.ToUpper().Equals("SQL"))
                {
                    q.setParam("adl", this.getUserDB() + ".getInADL(A.SYSTEM_ID,'F'," + idGruppo + "," + idPeople + ") AS IN_ADL ");
                }
                else
                {
                    q.setParam("adl", " getInADL(A.SYSTEM_ID,'F'," + idGruppo + "," + idPeople + ") AS IN_ADL ");
                }

                // Gestione esportazione dei soli documenti selezionati
                StringBuilder temp = new StringBuilder(" AND ( A.SYSTEM_ID IN(");
                if (idProfileList != null && idProfileList.Length > 0)
                {

                    int i = 0;
                    foreach (string id in idProfileList)
                    {
                        temp.Append(id);
                        if (i < idProfileList.Length - 1)
                        {
                            if (i % 998 == 0 && i > 0)
                            {
                                temp.Append(") OR A.SYSTEM_ID IN (");
                            }
                            else
                            {
                                temp.Append(", ");
                            }
                        }
                        else
                        {
                            temp.Append(")");
                        }
                        i++;
                    }

                }
                temp.Append(")");

                q.setParam("idFasc", temp.ToString());

                #region Ordinamento

                // Recupero dei filtri di ricerca relarivi all'ordinamento
                FiltroRicerca oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
                FiltroRicerca sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
                FiltroRicerca profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
                FiltroRicerca orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();

                if (orderDirection == null)
                {
                    orderDirection = new FiltroRicerca()
                    {
                        argomento = "ORDER_DIRECTION",
                        valore = "DESC"

                    };

                }

                // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
                String extractFieldValue = String.Empty;

                // Ordinamento ed ordinamento inverso
                String order = String.Empty, reverseOrder = String.Empty;

                if (this.dbType == "SQL")
                {
                    // DB SQL Server
                    // Se bisogna ordinare per campo custom...
                    if (profilationField != null)
                    {
                        // ...recupero del dettaglio dell'oggetto custom
                        OggettoCustom obj = new ModelFasc().getOggettoById(profilationField.valore);

                        if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                            // ...viene preparata la funzione per estrarre il valore del campo profilato
                            extractFieldValue = String.Format(", Convert(int, @dbuser@.GetValProfObjPrj(A.SYSTEM_ID, {0})) AS CUSTOM_FIELD", profilationField.valore);
                        else
                            // ...viene preparata la funzione per estrarre il valore del campo profilato
                            extractFieldValue = String.Format(", @dbuser@.GetValProfObjPrj(A.SYSTEM_ID, {0}) AS CUSTOM_FIELD", profilationField.valore);

                        // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                        order = String.Format("CUSTOM_FIELD {0}", orderDirection.valore);
                        reverseOrder = String.Format("CUSTOM_FIELD {0}", orderDirection.valore == "ASC" ? "DESC" : "ASC");
                    }
                    else
                    {
                        // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                        if (sqlField != null)
                        {
                            // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                            extractFieldValue = String.Format(", {0} AS ORDER_STANDARD", sqlField.valore);
                            order = String.Format("ORDER_STANDARD {0}", orderDirection.valore);
                            reverseOrder = String.Format("ORDER_STANDARD {0}", orderDirection.valore == "ASC" ? "DESC" : "ASC");
                        }
                        else
                        {
                            // Altrimenti viene creato il filtro standard
                            extractFieldValue = String.Empty;
                            order = String.Format("A.DTA_CREAZIONE {0}", orderDirection.valore);
                            reverseOrder = String.Format("A.DTA_CREAZIONE {0}", orderDirection.valore == "ASC" ? "DESC" : "ASC");
                        }
                    }

                }
                else
                {
                    // DB ORACLE
                    // Se bisogna ordinare per campo custom...
                    if (profilationField != null)
                    {
                        // ...recupero del dettaglio dell'oggetto custom
                        OggettoCustom obj = new ModelFasc().getOggettoById(profilationField.valore);

                        if (obj.TIPO.DESCRIZIONE_TIPO.ToUpper() == "CONTATORE")
                        {
                            // ...viene preparata la funzione per estrarre il valore del campo profilato
                            extractFieldValue = String.Format(", to_number(GetValProfObjPrj(A.SYSTEM_ID, {0}))", profilationField.valore);
                            // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                            order = String.Format("to_number(GetValProfObjPrj(A.SYSTEM_ID, {0})) {1}", profilationField.valore, orderDirection.valore);
                        }
                        else
                        {
                            // ...viene preparata la funzione per estrarre il valore del campo profilato
                            extractFieldValue = String.Format(", GetValProfObjPrj(A.SYSTEM_ID, {0})", profilationField.valore);
                            // ...viene preparato il filtro per ordinamento e per l'ordinamento inverso
                            order = String.Format("GetValProfObjPrj(A.SYSTEM_ID, {0}) {1}", profilationField.valore, orderDirection.valore);
                        }

                        reverseOrder = String.Empty;
                    }
                    else
                    {
                        // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                        if (oracleField != null)
                        {
                            // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                            extractFieldValue = String.Empty;
                            order = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                            reverseOrder = String.Empty;
                        }
                        else
                        {
                            // Altrimenti viene creato il filtro standard
                            extractFieldValue = String.Empty;
                            //  order = String.Format("A.DTA_CREAZIONE {0}", orderDirection.valore);
                            //Nel caso non ho le griglie custum ma ho una tipologia con un campo profilato
                            FiltroRicerca contatoreNoCustom = objListaFiltri.Where(e => e.argomento == "CONTATORE_GRIGLIE_NO_CUSTOM").FirstOrDefault();
                            if (contatoreNoCustom != null)
                            {
                                order = String.Format("getContatoreFascContatore (a.system_id, '" + contatoreNoCustom.valore + "') {0}", orderDirection.valore);
                            }
                            else
                            {
                                order = String.Format("A.DTA_CREAZIONE {0}", orderDirection.valore);
                            }

                            reverseOrder = String.Empty;
                        }
                    }
                }

                #endregion

                if (order.Equals("A.DTA_CREAZIONE DESC") || order.Equals("A.DTA_CREAZIONE ASC") || order.Equals("CHA_TIPO_FASCICOLO DESC") || order.Equals("CHA_TIPO_FASCICOLO ASC") || order.Equals("UPPER(TRIM(DESCRIPTION)) DESC") || order.Equals("UPPER(TRIM(DESCRIPTION)) ASC") || order.Equals("ORDER BY DTA_APERTURA DESC") || order.Equals("ORDER BY DTA_APERTURA ASC") || order.Equals("DTA_CHIUSURA DESC") || order.Equals("DTA_CHIUSURA ASC"))
                {
                    order += ", a.system_id DESC";
                }

                // Impostazione del parametro per l'estrazione del valore assunto da un campo profilato,
                // per l'ordinamento e per l'ordinamento inverso
                q.setParam("customFieldForOrder", extractFieldValue);
                q.setParam("customOrder", order);
                q.setParam("customOrder1", reverseOrder);


                string userDB = getUserDB();
                if (!string.IsNullOrEmpty(userDB))
                    q.setParam("dbuser", userDB);

                //modifica
                // q.setParam("tipoContatore", "'" + tipo_contatore + "'");
                //fine modifica

                // Ricerca con CodiceClassifica
                if (objClassificazione != null && !string.IsNullOrEmpty(objClassificazione.varcodliv1))
                {
                    q.setParam("tblCL1", " ,(SELECT A.SYSTEM_ID FROM PROJECT A, SECURITY B WHERE " +
                               " (A.SYSTEM_ID = B.THING) AND (B.ACCESSRIGHTS) > 0 AND " +
                               " (B.PERSONORGROUP= @idPeo@ OR B.PERSONORGROUP= @idGrp@ ) " +
                               " AND A.ID_AMM = @idAmm@ AND A.CHA_TIPO_PROJ = 'T' " +
                               " @idReg@ @varCodLiv@) C ");

                    q.setParam("whereTlbCL1", " AND (A.ID_PARENT = C.SYSTEM_ID)");

                    // esiste solo se objClassificazione è valido
                    if (childs) q.setParam("varCodLiv", "AND A.VAR_COD_LIV1 LIKE '" + objClassificazione.varcodliv1 + "%'");
                    else q.setParam("varCodLiv", "AND  A.VAR_COD_LIV1 = '" + objClassificazione.varcodliv1 + "'");
                }
                else
                {
                    q.setParam("tblCL1", "");
                    q.setParam("whereTlbCL1", "");
                }

                //common Where Condition
                q.setParam("idAmm", idAmm);


                //registro
                if (registro != null) q.setParam("idReg", " AND (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO =" + registro.systemId + ")");
                else q.setParam("idReg", "");

                q.setParam("startRow", startRow.ToString());
                q.setParam("endRow", endRow.ToString());

                // INIZIO - Parametri specifici per SqlServer
                // TODO : rovesciamento criteri di ordinamento dedicati a SQL e count SQL
                // INIZIO - Parametri specifici per SqlServer
                // il numero totale di righe da estrarre equivale 
                // al limite inferiore dell'ultima riga da estrarre

                q.setParam("dtaAp", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                q.setParam("dtaCh", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));

                String withClause = String.Empty;
                GetSqlQuery(idGruppo, idPeople, objListaFiltri, UOsottoposte, ref filterString, out withClause);

                if (cons)
                {
                    if (mancCons)
                    {
                        filterString1 = filterString + " AND A.SYSTEM_ID IN (SELECT DISTINCT ID_PROJECT FROM DPA_ITEMS_CONSERVAZIONE F, PROJECT G WHERE G.SYSTEM_ID = F.ID_PROJECT AND F.CHA_STATO <> 'C')";
                        filterString += " AND A.SYSTEM_ID NOT IN (SELECT DISTINCT ID_PROJECT FROM DPA_ITEMS_CONSERVAZIONE WHERE NOT ID_PROJECT IS NULL)";
                    }
                }
                //q.setParam("profilazione", " ");
                if (sysId >= 0 && caseDescr > 0)
                {
                    switch (caseDescr)
                    {
                        case 1:
                            if (dbType.ToUpper() == "SQL")
                                filterString += " AND " + userDB + ".checkSecurityProprietario(A.SYSTEM_ID, " + sysId + ", 0) = 1";
                            else
                                filterString += " AND checkSecurityProprietario(A.SYSTEM_ID, " + sysId + ", 0) = 1";
                            break;

                        case 2:
                            if (dbType.ToUpper() == "SQL")
                                filterString += " AND " + userDB + ".checkSecurityProprietario(A.SYSTEM_ID, 0, " + sysId + ") = 1";
                            else
                                filterString += " AND checkSecurityProprietario(A.SYSTEM_ID, 0, " + sysId + ") = 1";
                            break;

                        case 3:
                            if (dbType.ToUpper() == "SQL")
                            {
                                //filterString += " AND " + userDB + ".checkSecurityUO(A.SYSTEM_ID, " + sysId + ") = 1";
                                if (sysId != 0)
                                    filterString += " AND ID_UO_CREATORE = " + sysId;
                            }
                            else
                                filterString += " AND checkSecurityUO(A.SYSTEM_ID, " + sysId + ") = 1";
                            break;
                    }
                }

                if (filtriDocumentiInFascicoli != null)
                {
                    string queryFiltriDocumenti = this.getQueryFascicoliDocumenti(filtriDocumentiInFascicoli);

                    // Sono stati forniti dei filtri per la ricerca dei documenti 
                    filterString = string.Concat(filterString,
                                string.Format(" AND A.SYSTEM_ID IN ({0})", queryFiltriDocumenti));
                }

                //aggiungo i filtri GUI al query
                if (filterString != null) q.setParam("guiFilters", filterString);
                if (filterString1 != null) q.setParam("guiFilters1", filterString1);

                bool IS_ARCHIVISTA_DEPOSITO;
                DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
                IS_ARCHIVISTA_DEPOSITO = ut.isUtArchivistaDeposito(idPeople, idGruppo);
                string security = string.Empty;
                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
                if (IS_ARCHIVISTA_DEPOSITO)
                {
                    if (dbType.ToUpper() == "SQL")
                        security = " (@dbuser@.checkSecurity(A.SYSTEM_ID, @idPeo@, @idGrp@, @idRuoloPubblico@,'F') > 0)";
                    else
                        security = " (checkSecurity(A.SYSTEM_ID, @idPeo@, @idGrp@, @idRuoloPubblico@,'F') > 0)";
                }
                else
                {
                    if (IndexSecurity())
                        security = " EXISTS (select /*+INDEX(e) */  'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @idPeo@, @idGrp@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                    else
                        security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @idPeo@, @idGrp@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                }
                if (security == string.Empty)
                {
                    if (IndexSecurity())
                        security = " EXISTS (select /*+INDEX(e) */  'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @idPeo@, @idGrp@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                    else
                        security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @idPeo@, @idGrp@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                }

                q.setParam("security", security);
                q.setParam("idGrp", idGruppo);
                q.setParam("idPeo", idPeople);
                q.setParam("idRuoloPubblico", !string.IsNullOrEmpty(idRuoloPubblico) ? idRuoloPubblico : "0");
                //SAB gestione deposito
                q.setParam("dbuser", getUserDB());

                if (dbType.ToUpper().Equals("ORACLE"))
                {
                    if (!string.IsNullOrEmpty(tipo_contatore))
                    {
                        q.setParam("tipoContatore_cond", " , getContatoreFasc(a.system_id,@tipoContatore@) as contatore,getContatoreFasc(a.system_id,@tipoContatore@) as contatoreOrdinamento");
                        q.setParam("tipoContatore", "'" + tipo_contatore + "'");
                    }
                    else
                    {
                        q.setParam("tipoContatore_cond", "");
                        q.setParam("tipoContatore", "''");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(tipo_contatore))
                    {
                        q.setParam("tipoContatore", "'" + tipo_contatore + "'");
                    }
                    else
                    {
                        q.setParam("tipoContatore", "''");
                    }
                }

                //rilascio il query string
                queryString = q.getSQL();


                return queryString;
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                return null;
            }
        }

        public ArrayList GetListaFascicoliPagingCustom(DocsPaVO.utente.InfoUtente infoUtente,
                           DocsPaVO.fascicolazione.Classificazione objClassificazione,
                           DocsPaVO.utente.Registro registro,
                           DocsPaVO.filtri.FiltroRicerca[] filtriFascicoli,
                           DocsPaVO.filtri.FiltroRicerca[] filtriDocumentiInFascicoli,
                           bool enableUfficioRef,
                           bool enableProfilazione,
                           bool childs,
                           out int numTotPage,
                           out int totalRecordCount,
                           int requestedPage,
                           int pageSize,
                           bool getSystemIdList,
                            out List<SearchResultInfo> idProjectList, byte[] datiExcel, string serverPath, bool showGridPersonalization, bool export, Field[] visibleFieldsTemplate, String[] documentsSystemId, bool security)
        {
            logger.Debug("getListaFascicoliPagingCustom");

            ArrayList listaFascicoli = new ArrayList();

            totalRecordCount = 0;
            numTotPage = 0;

            // La lista dei system id da restituire
            List<SearchResultInfo> idProjecs = null;

            try
            {
                //parametri di queryString Comuni.
                string queryString = "";

                //nuova paginazione:
                //prima la count con i filtri.
                queryString = setCommonParameterCustom(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople,
                                                 objClassificazione, registro,
                                                 filtriFascicoli, filtriDocumentiInFascicoli, enableUfficioRef,
                                                 enableProfilazione, childs,
                                                 out numTotPage, 0,
                                                 totalRecordCount,
                                                 "DATACOUNT", pageSize, getSystemIdList, datiExcel, serverPath, showGridPersonalization, export, visibleFieldsTemplate, documentsSystemId, security);

                if (getSystemIdList)
                {
                    //eseguo la query count
                    using (DBProvider dbProvider = new DBProvider())
                    {
                        string field = string.Empty;
                        //if (dbProvider.ExecuteScalar(out field, queryString))
                        //    totalRecordCount = Convert.ToInt32(field);
                        IDataReader dr = dbProvider.ExecuteReader(queryString);

                        // Se sono richiesti i system id dei fascicoli, viene inizializzata
                        // la lista
                        if (getSystemIdList)
                            idProjecs = new List<SearchResultInfo>();

                        while (dr.Read())
                        {
                            field = dr.GetValue(0).ToString();

                            // Se è richiesta la lista dei system id dei fascicoli,
                            // viene aggiunto field alla lista dei system id
                            if (getSystemIdList)
                            {
                                SearchResultInfo temp = new SearchResultInfo();
                                temp.Id = field;
                                temp.Codice = dr.GetValue(1).ToString();
                                idProjecs.Add(temp);
                            }
                        }

                        // Se è richiesta la lista dei system id dei fascicoli, viene
                        // calcolato il numero di risultati restituiti dalla ricerca
                        if (getSystemIdList)
                            field = idProjecs.Count.ToString();

                        if (field != string.Empty)
                            totalRecordCount = Convert.ToInt32(field);
                    }
                }
                else
                {
                    using (DBProvider dbProvider = new DBProvider())
                    {
                        IDataReader dr = dbProvider.ExecuteReader(queryString);
                        while (dr.Read())
                        {
                            totalRecordCount = Convert.ToInt32(dr.GetValue(0).ToString());
                        }
                    }
                }

                /* ABBATANGELI GIANLUIGI
                 * Aggiunto il valore di configurazione MAX_ROW_SEARCHABLE
                 * che determina il numero massimo di righe accettatte
                 * come risultato di una ricerca fasciloli  
                 * tranne il caso in cui sto eseguendo un export */
                int maxRowSearchable = (export ? 0 : Cfg_MAX_ROW_SEARCHABLE(infoUtente.idAmministrazione));

                if (maxRowSearchable == 0 || totalRecordCount <= maxRowSearchable)
                {
                    if (totalRecordCount >= 0)//> 0)
                    {
                        logger.Debug("Trovati " + totalRecordCount + " Fascicoli");
                        //poi la fill con i filtri
                        //procedo con la fill dei dati 
                        queryString = setCommonParameterCustom(infoUtente.idAmministrazione, infoUtente.idGruppo, infoUtente.idPeople,
                                                    objClassificazione, registro,
                                                    filtriFascicoli, filtriDocumentiInFascicoli, enableUfficioRef,
                                                    enableProfilazione, childs,
                                                    out numTotPage,
                                                    requestedPage,
                                                    totalRecordCount,
                                                     "DATAFILL", pageSize, false, datiExcel, serverPath, showGridPersonalization, export, visibleFieldsTemplate, documentsSystemId, security);


                        DataSet ds = new DataSet();
                        using (DBProvider dbProvider = new DBProvider())
                        {
                            dbProvider.ExecuteQuery(ds, queryString);

                            if (ds != null && ds.Tables[0] != null)
                            {
                                logger.Debug("GetDatiFascicoloCustom");
                                foreach (System.Data.DataRow dataRow in ds.Tables[0].Rows)
                                {
                                    listaFascicoli.Add(GetDatiFascicoloCustom(ds, dataRow, visibleFieldsTemplate));
                                }   
                            }

                            ds.Dispose();
                        }
                    }
                }
                else
                {
                    /* ABBATANGELI GIANLUIGI
                    * Non carico i documenti perchè raggiunto il numero massimo 
                    * di righe per la ricerca ed imposto numTotPage = -2. */
                    numTotPage = -2;
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                listaFascicoli = null;
            }

            // Salvataggio della lista dei system id
            idProjectList = idProjecs;

            return listaFascicoli;
        }

        /* ABBATANGELI GIANLUIGI
         * Caricamento dal database del valore int relativo al 
         * numero massimo di righe accettate come risultato di ricerca */
        /// <summary>
        /// return, numero massimo di righe per le ricerche.
        /// </summary>
        public int Cfg_MAX_ROW_SEARCHABLE(string idAmministrazione)
        {
            int result = 0;
            string value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmministrazione, "MAX_ROW_SEARCHABLE");

            if (string.IsNullOrEmpty(value))
            {
                value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "MAX_ROW_SEARCHABLE");
            }
            if (!string.IsNullOrEmpty(value))
            {
                result = Convert.ToInt32(value);
            }
            return result;
        }
        private string setCommonParameterCustom(string idAmm,
                                         string idGruppo,
                                         string idPeople,
                                         DocsPaVO.fascicolazione.Classificazione objClassificazione,
                                         DocsPaVO.utente.Registro registro,
                                         DocsPaVO.filtri.FiltroRicerca[] objListaFiltri,
                                           DocsPaVO.filtri.FiltroRicerca[] filtriDocumentiInFascicoli,
                                         bool enableUfficioRef,
                                         bool enableProfilazione,
                                         bool childs,
                                         out int numTotPage,
                                         int requestedPage,
                                         int totalRecordCount,
                                         string queryType,
                                         int pageSize,
                                          bool getSystemIdList, byte[] datiExcel, string serverPath, bool showGridPersonalization, bool export, Field[] visibleFieldsTemplate, String[] documentsSystemId, bool conSecurity)
        {
            numTotPage = 0;
            try
            {
                DocsPaUtils.Query q = null;
                string queryString = string.Empty;
                int startRow = 0;
                int endRow = 0;
                string filterString = string.Empty;
                string filterString1 = string.Empty;
                bool cons = false;
                bool mancCons = false;
                int sysId = 0;
                int caseDescr = 0;
                bool UOsottoposte = false;
                bool from_conservazione = false;
                bool from_esibizione = false;
                string idParent = string.Empty;

                if (objListaFiltri == null)
                    objListaFiltri = new FiltroRicerca[] { };

                foreach (DocsPaVO.filtri.FiltroRicerca fil in objListaFiltri)
                {
                    if (fil.argomento.Equals("CONSERVAZIONE") && fil.valore != null)
                    {
                        cons = true;
                        if (fil.valore == "0")
                            mancCons = true;
                    }

                    if (fil.argomento.Equals("DESC_PEOPLE_CREATORE") && fil.valore != null && !fil.valore.Equals(""))
                    {
                        caseDescr = 1;
                    }

                    if (fil.argomento.Equals("DESC_RUOLO_CREATORE") && fil.valore != null && !fil.valore.Equals(""))
                    {
                        caseDescr = 2;

                    }

                    if (fil.argomento.Equals("DESC_UO_CREATORE") && fil.valore != null && !fil.valore.Equals(""))
                    {
                        caseDescr = 3;
                    }

                    if (fil.argomento.Equals("UO_SOTTOPOSTE") && fil.valore != null && !fil.valore.Equals(""))
                        UOsottoposte = true;

                    if (fil.argomento.Equals("IN_CONSERVAZIONE") && fil.valore != null && !string.IsNullOrEmpty(fil.valore))
                    {
                        if (fil.valore.Equals("F"))
                        {
                            from_conservazione = true;
                        }
                    }

                    if (fil.argomento.Equals("IN_CONSERVAZIONE_ESIB") && fil.valore != null && !string.IsNullOrEmpty(fil.valore))
                    {
                        if (fil.valore.Equals("F"))
                        {
                            from_esibizione = true;
                        }
                    }
                    //Gabriele Melini 06-11-2013
                    //ricerca con codice fascicolo da conservazione
                    if (fil.argomento.Equals("CONSERVAZIONE_CODFASC") && fil.valore != null && !string.IsNullOrEmpty(fil.valore))
                    {
                        idParent = fil.valore;
                    }
                    
                }




                if (queryType.ToUpper() == "DATACOUNT")
                {
                    // solo COUNT dei dati
                    if (cons && mancCons)
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_FASCICOLI_PAGING_UNION");
                    else
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_COUNT_FASCICOLI_PAGING");
                }

                // Se getSystemIdList è true e queryType è DATACOUNT...
                if (getSystemIdList &&
                    queryType.ToUpper() == "DATACOUNT")
                {
                    // ...viene invocata la query per la restituzione dei system id dei
                    // fascicoli
                    if (cons && mancCons)
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("COUNT_PROJECT_MASSIVE_OPERATION_UNION");
                    else
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("COUNT_PROJECT_MASSIVE_OPERATION");
                }

                if (queryType.ToUpper() == "DATAFILL")
                { // solo FILL dei dati
                    if (cons && mancCons)
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASCICOLI_PAGING_UNION");
                    else
                    {
                        q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASCICOLI_PAGING_CUSTOM");
                    }
                    // Parametri specifici per query oracle
                    string preferedIndex = string.Empty;
                    if (!export)
                    {
                        preferedIndex = "/*+ FIRST_ROWS(" + pageSize + ") */";
                    }
                    else
                    {
                        preferedIndex = "/*+ FIRST_ROWS(" + totalRecordCount + ") */";
                    }
                    q.setParam("index", preferedIndex);
                }

                //Valori campi custom
                string valoriCustom = string.Empty;

                if (visibleFieldsTemplate != null && visibleFieldsTemplate.Length > 0)
                {
                    foreach (Field d in visibleFieldsTemplate)
                    {
                        if (this.dbType == "SQL")
                        {
                            valoriCustom = valoriCustom + ", " + this.getUserDB() + ".GetValProfObjPrj (system_id, " + d.CustomObjectId.ToString() + ") AS A" + d.CustomObjectId.ToString();
                        }
                        else
                        {
                            valoriCustom = valoriCustom + ", " + "GetValProfObjPrj (system_id, " + d.CustomObjectId.ToString() + ") AS A" + d.CustomObjectId.ToString();
                        }
                    }
                }

                q.setParam("valoriCustom", valoriCustom);

                if (from_conservazione)
                {
                    if (this.dbType == "SQL")
                    {

                        q.setParam("conservazione", " ," + getUserDB() + ".getInConservazioneNoSec(null,a.system_id,'F') as istanzeConservazione ");
                    }
                    else
                    {
                        q.setParam("conservazione", " ,getInConservazioneNoSec(null,a.system_id,'F') as istanzeConservazione ");
                    }

                    
                }
                else
                {
                    q.setParam("conservazione", string.Empty);
                }

                //
                // Mev CS 1.4 - Esibizione
                if (from_esibizione)
                {
                    if (this.dbType == "SQL")
                    {

                        q.setParam("esibizione", " ," + getUserDB() + ".getInConsNoSecPerEsib(null,a.system_id,'F') as istanzeConservazione ");
                    }
                    else
                    {
                        q.setParam("esibizione", " ,getInConsNoSecPerEsib(null,a.system_id,'F') as istanzeConservazione ");
                    }
                }
                else
                {
                    q.setParam("esibizione", string.Empty);
                }
                // End Mev CS 1.4- Esibizione
                //

                #region Ordinamento

                // Recupero dei filtri di ricerca relarivi all'ordinamento
                FiltroRicerca oracleField = objListaFiltri.Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
                FiltroRicerca sqlField = objListaFiltri.Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
                FiltroRicerca profilationField = objListaFiltri.Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
                FiltroRicerca orderDirection = objListaFiltri.Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();

                FiltroRicerca contatatore_no_custom = objListaFiltri.Where(e => e.argomento == "CONTATORE_GRIGLIE_NO_CUSTOM").FirstOrDefault();


                // Function da utilizzare per estrarre i valori del campo profilato da utilizzare per l'ordinamento
                String extractFieldValue = String.Empty;
                String extractFieldValue2 = String.Empty;
                String order = String.Empty;
                String reverseOrder = String.Empty;

                String contatore = string.Empty;

                if (this.dbType == "SQL")
                {
                    // DB SQL Server
                    // Se bisogna ordinare per campo custom...
                    reverseOrder = String.Empty;
                    if (profilationField != null)
                    {
                        if (contatatore_no_custom != null && !showGridPersonalization)
                        {
                            if (orderDirection.valore.Equals("ASC"))
                            {
                                extractFieldValue = ", " + " ISNULL(convert(int, @dbuser@.GETCONTATOREFASCCONTATORE(a.system_id, '" + contatatore_no_custom.valore + "')),'zzzzzzzzzz')";
                                extractFieldValue2 = "ISNULL(convert(int, @dbuser@.GETCONTATOREFASCCONTATORE(a.system_id, '" + contatatore_no_custom.valore + "')),'zzzzzzzzzz')";
                            }
                            else
                            {
                                extractFieldValue = ", " + "convert(int, @dbuser@.GETCONTATOREFASCCONTATORE (a.system_id, '" + contatatore_no_custom.valore + "'))";
                                extractFieldValue2 = "convert(int, @dbuser@.GETCONTATOREFASCCONTATORE (a.system_id, '" + contatatore_no_custom.valore + "'))";
                            }
                            contatore = ", @dbuser@.getcContatoreFasc(docnumber,'R') as contatore";
                        }
                        else
                        {
                            Field d = visibleFieldsTemplate.Where(e => e.CustomObjectId.ToString() == profilationField.valore).FirstOrDefault();
                            if (orderDirection.valore.Equals("ASC"))
                            {
                                if (d.IsNumber)
                                {
                                    extractFieldValue = String.Format(", ISNULL(convert(int, @dbuser@.GetValProfObjPrjOrder(a.system_id, {0})),999999999)", profilationField.valore);
                                    extractFieldValue2 = String.Format(" ISNULL(convert(int, @dbuser@.GetValProfObjPrjOrder(a.system_id, {0})),999999999)", profilationField.valore);
                                }
                                else
                                {
                                    extractFieldValue = String.Format(", ISNULL(@dbuser@.GetValProfObjPrj(a.system_id, {0}),'zzzzzzzzzz')", profilationField.valore);
                                    extractFieldValue2 = String.Format(" ISNULL(@dbuser@.GetValProfObjPrj(a.system_id, {0}),'zzzzzzzzzz')", profilationField.valore);
                                }
                            }
                            else
                            {
                                if (d.IsNumber)
                                {
                                    extractFieldValue = String.Format(", convert(int, @dbuser@.GetValProfObjPrj(a.system_id, {0}))", profilationField.valore);
                                    extractFieldValue2 = String.Format(" convert(int, @dbuser@.GetValProfObjPrj(a.system_id, {0}))", profilationField.valore);
                                }
                                else
                                {
                                    extractFieldValue = String.Format(", @dbuser@.GetValProfObjPrj(a.system_id, {0})", profilationField.valore);
                                    extractFieldValue2 = String.Format(" @dbuser@.GetValProfObjPrj(a.system_id, {0})", profilationField.valore);
                                }
                            }
                        }

                        order = String.Format("{0} {1}", extractFieldValue2, orderDirection.valore);

                        order = order + ", A.DTA_CREAZIONE DESC";
                    }
                    else
                    {
                        // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                        if (sqlField != null)
                        {
                            // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                            extractFieldValue = String.Empty;
                            if (orderDirection.valore.Equals("ASC"))
                            {
                                if (sqlField.nomeCampo.Equals("P5") || sqlField.nomeCampo.Equals("P6") || sqlField.nomeCampo.Equals("P6") || sqlField.nomeCampo.Equals("P20"))
                                {
                                    order = "ISNULL(" + String.Format("{0} ,'9999-12-31 23:59:59.997') {1}", sqlField.valore, orderDirection.valore);
                                }
                                else
                                {
                                    if (sqlField.nomeCampo.Equals("P13") || sqlField.nomeCampo.Equals("P15") || sqlField.nomeCampo.Equals("P14"))
                                    {
                                        order = "ISNULL(" + String.Format("{0} ,999999999) {1}", sqlField.valore, orderDirection.valore);
                                    }
                                    else
                                    {
                                        if (sqlField.nomeCampo.Equals("P11"))
                                        {
                                            order = "ISNULL(" + String.Format("{0} ,999) {1}", sqlField.valore, orderDirection.valore);
                                        }
                                        else
                                        {
                                            order = "ISNULL(" + String.Format("{0} ,'zzzzzzzzzz') {1}", sqlField.valore, orderDirection.valore);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                order = String.Format("{0} {1}", sqlField.valore, orderDirection.valore);
                            }
                            order = order + ", A.DTA_CREAZIONE DESC";
                        }
                    }

                }
                else
                {
                    // DB ORACLE
                    // Se bisogna ordinare per campo custom...
                    reverseOrder = String.Empty;
                    if (profilationField != null)
                    {
                        if (contatatore_no_custom != null && !showGridPersonalization)
                        {
                            extractFieldValue = ", " + "to_number(getContatoreFascContatore (a.system_id, '" + contatatore_no_custom.valore + "'))";
                            extractFieldValue2 = "to_number(getContatoreFascContatore (a.system_id, '" + contatatore_no_custom.valore + "'))";
                            contatore = ", GETCONTATOREFASC(a.system_id,'R') as contatore";
                        }
                        else
                        {
                            Field d = visibleFieldsTemplate.Where(e => e.CustomObjectId.ToString() == profilationField.valore).FirstOrDefault();

                            if (d.IsNumber)
                            {
                                extractFieldValue = String.Format(", to_number(GetValProfObjPrjOrder(A.SYSTEM_ID, {0}))", profilationField.valore);
                                extractFieldValue2 = String.Format(" to_number(GetValProfObjPrjOrder(A.SYSTEM_ID, {0}))", profilationField.valore);
                            }
                            else
                            {
                                extractFieldValue = String.Format(", GetValProfObjPrj(A.SYSTEM_ID, {0})", profilationField.valore);
                                extractFieldValue2 = String.Format(" GetValProfObjPrj(A.SYSTEM_ID, {0})", profilationField.valore);
                            }
                        }

                        order = String.Format("{0} {1}", extractFieldValue2, orderDirection.valore);

                        order = order + " NULLS LAST, A.DTA_CREAZIONE DESC";
                    }
                    else
                    {
                        // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                        if (oracleField != null)
                        {
                            // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                            extractFieldValue = String.Empty;
                            order = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                            order = order + " NULLS LAST, A.DTA_CREAZIONE DESC";
                        }
                        else
                        {
                            // Altrimenti viene creato il filtro standard
                            extractFieldValue = String.Empty;
                            //Nel caso non ho le griglie custum ma ho una tipologia con un campo profilato
                            FiltroRicerca contatoreNoCustom = objListaFiltri.Where(e => e.argomento == "CONTATORE_GRIGLIE_NO_CUSTOM").FirstOrDefault();
                            if (contatoreNoCustom != null)
                            {
                                order = String.Format("TO_NUMBER(getContatoreFascContatore (a.system_id, 'R')) {0}", orderDirection.valore);
                            }
                            else
                            {
                                if (orderDirection != null)
                                {
                                    order = String.Format("A.DTA_CREAZIONE {0}", orderDirection.valore);
                                }
                                else
                                {
                                    order = String.Format("A.DTA_CREAZIONE DESC");
                                }
                            }

                            order = order + " NULLS LAST, A.DTA_CREAZIONE DESC";
                        }
                    }
                }

                #endregion
                if (this.dbType == "SQL" && order.Equals(string.Empty))
                    order = " A.DTA_CREAZIONE DESC ";

                q.setParam("order", order);
                q.setParam("contatore", contatore);

                // Impostazione del parametro per l'estrazione del valore assunto da un campo profilato,
                // per l'ordinamento e per l'ordinamento inverso
                q.setParam("customFieldForOrder", extractFieldValue);
                q.setParam("customOrder", order);
                q.setParam("customOrder1", reverseOrder);


                string userDB = getUserDB();
                if (!string.IsNullOrEmpty(userDB))
                    q.setParam("dbuser", userDB);

                //modifica
                // q.setParam("tipoContatore", "'" + tipo_contatore + "'");
                //fine modifica

                // Ricerca con CodiceClassifica
                if (objClassificazione != null && !string.IsNullOrEmpty(objClassificazione.varcodliv1))
                {
                    q.setParam("tblCL1", " ,(SELECT A.SYSTEM_ID FROM PROJECT A WHERE EXISTS  " +
                               " ( SELECT 'X' FROM SECURITY B WHERE (A.SYSTEM_ID = B.THING) AND (B.ACCESSRIGHTS) > 0 AND " +
                               " (B.PERSONORGROUP= @idPeo@ OR B.PERSONORGROUP= @idGrp@ ) " +
                               " AND A.ID_AMM = @idAmm@ AND A.CHA_TIPO_PROJ = 'T' " +
                               " @idReg@ @varCodLiv@) ) C ");

                    q.setParam("whereTlbCL1", " AND (A.ID_PARENT = C.SYSTEM_ID)");

                    // esiste solo se objClassificazione è valido
                    if (childs) q.setParam("varCodLiv", "AND A.VAR_COD_LIV1 LIKE '" + objClassificazione.varcodliv1 + "%'");
                    else q.setParam("varCodLiv", "AND  A.VAR_COD_LIV1 = '" + objClassificazione.varcodliv1 + "'");
                }
                else
                {
                    q.setParam("tblCL1", "");
                    q.setParam("whereTlbCL1", "");
                }

                //common Where Condition
                q.setParam("idAmm", idAmm);


                //registro
                if (registro != null) q.setParam("idReg", " AND (A.ID_REGISTRO IS NULL OR A.ID_REGISTRO =" + registro.systemId + ")");
                else q.setParam("idReg", "");

                if (queryType == "DATAFILL") // solo FILL dei dati aggiungo i filtri
                {

                    // operazioni Matematiche per calcolo paginazione
                    // Determina il num di pagine totali 

                    numTotPage = (totalRecordCount / pageSize);

                    if (numTotPage != 0)
                    {
                        //oldcode paginazione sbagliata
                        //if ((totalRecordCount % numTotPage) > 0) numTotPage++;
                        if ((totalRecordCount % pageSize) > 0) numTotPage++;
                    }
                    else numTotPage = 1;

                    startRow = ((requestedPage * pageSize) - pageSize) + 1;
                    endRow = (startRow - 1) + pageSize;


                    string paging = string.Empty;
                    if (this.dbType == "SQL")
                    {
                        if (!export)
                        {
                            paging = "WHERE Row <= " + endRow.ToString() + " AND Row >=" + startRow.ToString();
                        }
                        else
                        {
                            paging = "WHERE Row <= " + totalRecordCount.ToString();
                        }
                    }
                    else
                    {
                        if (!export)
                        {
                            paging = "WHERE ROWNUM <= " + endRow.ToString() + " ) a WHERE rnum >=" + startRow.ToString();
                        }
                        else
                        {
                            paging = "WHERE ROWNUM <= " + totalRecordCount.ToString() + " ) a WHERE rnum >=" + startRow.ToString();
                        }
                    }

                    string listDocuments = string.Empty;
                    if (export)
                    {
                        if (documentsSystemId != null && documentsSystemId.Length > 0)
                        {
                            int i = 0;
                            listDocuments += " AND ( A.SYSTEM_ID IN(";
                            foreach (string id in documentsSystemId)
                            {
                                listDocuments += id;
                                if (i < documentsSystemId.Length - 1)
                                {
                                    if (i % 998 == 0 && i > 0)
                                    {
                                        listDocuments += ") OR A.SYSTEM_ID IN (";
                                    }
                                    else
                                    {
                                        listDocuments += ", ";
                                    }
                                }
                                else
                                {
                                    listDocuments += ")";
                                }
                                i++;
                            }
                            listDocuments += ")";
                        }

                    }

                    q.setParam("listDocuments", listDocuments);

                    q.setParam("paging", paging);

                    int pageSizeSqlServer = pageSize;
                    int totalRowsSqlServer = (requestedPage * pageSize);
                    if ((totalRecordCount - totalRowsSqlServer) <= 0)
                    {
                        pageSizeSqlServer -= System.Math.Abs(totalRecordCount - totalRowsSqlServer);
                        totalRowsSqlServer = totalRecordCount;
                    }

                    q.setParam("pageSize", pageSizeSqlServer.ToString()); // Dimensione pagina
                    q.setParam("totalRows", totalRowsSqlServer.ToString());


                    // FINE - Parametri specifici per SqlServer
                }

                q.setParam("dtaAp", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                q.setParam("dtaCh", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));

                // flitri GUI
                String withClause = String.Empty;
                GetSqlQuery(idGruppo, idPeople, objListaFiltri, UOsottoposte, ref filterString, out withClause);

                //FILTRO EXCEL
                if (datiExcel != null)
                {
                    getFiltroExcel(idAmm, ref filterString, datiExcel, serverPath, objListaFiltri);
                }

                //Gabriele Melini 06-11-2013
                //filtro per codice fascicolo da conservazione
                if (from_conservazione || from_esibizione)
                {
                    if (!string.IsNullOrEmpty(idParent))
                        filterString = filterString + " AND A.ID_PARENT=" + idParent + " ";
                }
                //fine filtro

                if (cons)
                {
                    if (mancCons)
                    {
                        filterString1 = filterString + " AND A.SYSTEM_ID IN (SELECT DISTINCT ID_PROJECT FROM DPA_ITEMS_CONSERVAZIONE F, PROJECT G WHERE G.SYSTEM_ID = F.ID_PROJECT AND F.CHA_STATO <> 'C')";
                        filterString += " AND A.SYSTEM_ID NOT IN (SELECT DISTINCT ID_PROJECT FROM DPA_ITEMS_CONSERVAZIONE WHERE NOT ID_PROJECT IS NULL)";
                    }
                }
                //q.setParam("profilazione", " ");
                if (sysId >= 0 && caseDescr > 0)
                {
                    switch (caseDescr)
                    {
                        case 1:
                            if (dbType.ToUpper() == "SQL")
                                filterString += " AND " + userDB + ".checkSecurityProprietario(A.SYSTEM_ID, " + sysId + ", 0) = 1";
                            else
                                filterString += " AND checkSecurityProprietario(A.SYSTEM_ID, " + sysId + ", 0) = 1";
                            break;

                        case 2:
                            if (dbType.ToUpper() == "SQL")
                                filterString += " AND " + userDB + ".checkSecurityProprietario(A.SYSTEM_ID, 0, " + sysId + ") = 1";
                            else
                                filterString += " AND checkSecurityProprietario(A.SYSTEM_ID, 0, " + sysId + ") = 1";
                            break;

                        case 3:
                            if (dbType.ToUpper() == "SQL")
                            {
                                //filterString += " AND " + userDB + ".checkSecurityUO(A.SYSTEM_ID, " + sysId + ") = 1";
                                if (sysId != 0)
                                    filterString += " AND ID_UO_CREATORE = " + sysId;
                            }
                            else
                                filterString += " AND checkSecurityUO(A.SYSTEM_ID, " + sysId + ") = 1";
                            break;
                    }
                }

                if (filtriDocumentiInFascicoli != null)
                {
                    string queryFiltriDocumenti = this.getQueryFascicoliDocumenti(filtriDocumentiInFascicoli);

                    // Sono stati forniti dei filtri per la ricerca dei documenti 
                    filterString = string.Concat(filterString,
                                string.Format(" AND A.SYSTEM_ID IN ({0})", queryFiltriDocumenti));
                }

                //aggiungo i filtri GUI al query
                if (filterString != null) q.setParam("guiFilters", filterString);
                if (filterString1 != null) q.setParam("guiFilters1", filterString1);

                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(idAmm, "ENABLE_FASCICOLO_PUBBLICO");
                if (string.IsNullOrEmpty(idRuoloPubblico))
                    idRuoloPubblico = "0";
                bool IS_ARCHIVISTA_DEPOSITO;
                DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
                IS_ARCHIVISTA_DEPOSITO = ut.isUtArchivistaDeposito(idPeople, idGruppo);
                string security = string.Empty;
                if (IS_ARCHIVISTA_DEPOSITO)
                {
                    if (dbType.ToUpper() == "SQL")
                        security = " (" + getUserDB() + ".checkSecurity(A.SYSTEM_ID, @idPeople@, @idGruppo@, @idRuoloPubblico@, 'F') > 0)";
                    else
                        security = " (checkSecurity(A.SYSTEM_ID, @idPeople@, @idGruppo@, @idRuoloPubblico@, 'F') > 0)";
                }
                else
                {
                    if (IndexSecurity())
                        security = " EXISTS (select /*+INDEX(e) */  'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @idPeo@, @idGrp@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                    else
                        security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @idPeo@, @idGrp@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                }

                if (conSecurity)
                {
                    if (security == string.Empty)
                    {
                        if (IndexSecurity())
                            security = " EXISTS (select /*+INDEX(e) */  'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @idPeo@, @idGrp@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                        else
                            security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in ( @idPeo@, @idGrp@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                    }
                }
                else
                {
                    security = " A.SYSTEM_ID = A.SYSTEM_ID";

                }

                q.setParam("security", security);
                q.setParam("idGrp", idGruppo);
                q.setParam("idPeo", idPeople);
                q.setParam("idGruppo", idGruppo);
                q.setParam("idPeople", idPeople);
                q.setParam("idRuoloPubblico", idRuoloPubblico);

                //rilascio il query string
                queryString = q.getSQL();

                if (dbType == "SQL" && !string.IsNullOrEmpty(chaiTableDef))
                {
                    queryString = chaiTableDef + queryString;
                    chaiTableDef = string.Empty;
                }

                if (!String.IsNullOrEmpty(withClause))
                    queryString = withClause + " " + queryString;

                logger.Debug(queryType + ": " + queryString);

                return queryString;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return null;
            }
        }

        public DocsPaVO.Grids.SearchObject GetDatiFascicoloCustom(DataSet dataSet, DataRow dataRow, Field[] visibleFieldsTemplate)
        {
            //logger.Debug("GetDatiFascicoloCustom");
            DocsPaVO.Grids.SearchObjectField objField = new DocsPaVO.Grids.SearchObjectField();
            DocsPaVO.Grids.SearchObject objDoc = new DocsPaVO.Grids.SearchObject();
            objDoc.SearchObjectField = new List<DocsPaVO.Grids.SearchObjectField>();

            objDoc.SearchObjectID = dataRow["SYSTEM_ID"].ToString();

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["CHA_TIPO_FASCICOLO"].ToString();
            objField.SearchObjectFieldID = "P1";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["ID_REGISTRO"].ToString();
            objField.SearchObjectFieldID = "ID_REGISTRO";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["CODTIT"].ToString();
            objField.SearchObjectFieldID = "P2";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["VAR_CODICE"].ToString();
            objField.SearchObjectFieldID = "P3";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["description"].ToString();
            objField.SearchObjectFieldID = "P4";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["DTA_APERTURA"].ToString();
            objField.SearchObjectFieldID = "P5";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["DTA_CHIUSURA"].ToString();
            objField.SearchObjectFieldID = "P6";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["AOO"].ToString();
            objField.SearchObjectFieldID = "P7";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["ULTIMA_NOTA"].ToString();
            objField.SearchObjectFieldID = "P8";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["CHA_PRIVATO"].ToString();
            objField.SearchObjectFieldID = "P9";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["cod_titolario"].ToString();
            objField.SearchObjectFieldID = "P10";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["cartaceo"].ToString();
            objField.SearchObjectFieldID = "P11";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["cha_in_archivio"].ToString();
            objField.SearchObjectFieldID = "P12";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["in_conservazione"].ToString();
            objField.SearchObjectFieldID = "P13";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["num_fasc"].ToString();
            objField.SearchObjectFieldID = "P14";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["NUM_MESI_CONSERVAZIONE"].ToString();
            objField.SearchObjectFieldID = "P15";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["stato"].ToString();
            objField.SearchObjectFieldID = "P16";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["desc_autore"].ToString();
            objField.SearchObjectFieldID = "P17";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["desc_ruolo"].ToString();
            objField.SearchObjectFieldID = "P18";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["desc_uo"].ToString();
            objField.SearchObjectFieldID = "P19";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["IN_ADL"].ToString();
            objField.SearchObjectFieldID = "IN_ADL";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["IN_ADLROLE"].ToString();
            objField.SearchObjectFieldID = "IN_ADLROLE";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["ID_TIPO_FASC"].ToString();
            objField.SearchObjectFieldID = "ID_TIPO_FASC";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["DESC_TIPO_ATTO"].ToString();
            objField.SearchObjectFieldID = "U1";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["DTA_CREAZIONE"].ToString();
            objField.SearchObjectFieldID = "P20";
            objDoc.SearchObjectField.Add(objField);

            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["DESC_COLL"].ToString();
            objField.SearchObjectFieldID = "P22";
            objDoc.SearchObjectField.Add(objField);

            if (dataRow.Table.Columns.Contains("dta_ins_adl"))
            {
                objField = new DocsPaVO.Grids.SearchObjectField();
                objField.SearchObjectFieldValue = dataRow["dta_ins_adl"].ToString();
                objField.SearchObjectFieldID = "DTA_ADL";
                objDoc.SearchObjectField.Add(objField);
            }
            if (dataRow.Table.Columns.Contains("var_motivo_adl"))
            {
                objField = new DocsPaVO.Grids.SearchObjectField();
                objField.SearchObjectFieldValue = dataRow["var_motivo_adl"].ToString();
                objField.SearchObjectFieldID = "MOTIVO_ADL";
                objDoc.SearchObjectField.Add(objField);
            }

            objField = new DocsPaVO.Grids.SearchObjectField();
            if (!String.IsNullOrEmpty(dataRow["cha_cod_t_a"].ToString()))
            {
                objField.SearchObjectFieldValue = new DocsPaVO.Security.InfoAtipicita() { CodiceAtipicita = dataRow["cha_cod_t_a"].ToString() }.DescrizioneAtipicita;
                objField.SearchObjectFieldValue = objField.SearchObjectFieldValue.Substring(1, objField.SearchObjectFieldValue.Length - 6);
            }
            else
                objField.SearchObjectFieldValue = String.Empty;
            objField.SearchObjectFieldID = "P23";
            objDoc.SearchObjectField.Add(objField);


            objField = new DocsPaVO.Grids.SearchObjectField();
            objField.SearchObjectFieldValue = dataRow["ESISTE_NOTA"].ToString();
            objField.SearchObjectFieldID = "ESISTE_NOTA";
            objDoc.SearchObjectField.Add(objField);

            if (dataRow.Table.Columns.Contains("CONTATORE"))
            {
                objField = new DocsPaVO.Grids.SearchObjectField();
                objField.SearchObjectFieldValue = dataRow["CONTATORE"].ToString();
                objField.SearchObjectFieldID = "CONTATORE";
                objDoc.SearchObjectField.Add(objField);

            }

            if (dataRow.Table.Columns.Contains("ISTANZECONSERVAZIONE"))
            {
                objField = new DocsPaVO.Grids.SearchObjectField();
                objField.SearchObjectFieldValue = dataRow["ISTANZECONSERVAZIONE"].ToString();
                objField.SearchObjectFieldID = "ISTANZECONSERVAZIONE";
                objDoc.SearchObjectField.Add(objField);
            }

            if (dataRow.Table.Columns.Contains("COD_EXT_APP"))
            {
                objField = new DocsPaVO.Grids.SearchObjectField();
                objField.SearchObjectFieldValue = dataRow["COD_EXT_APP"].ToString();
                objField.SearchObjectFieldID = "COD_EXT_APP";
                objDoc.SearchObjectField.Add(objField);
            }

            if (visibleFieldsTemplate != null && visibleFieldsTemplate.Length > 0)
            {
                foreach (Field d in visibleFieldsTemplate)
                {
                    objField = new DocsPaVO.Grids.SearchObjectField();
                    string nameColumn = "A" + d.CustomObjectId.ToString();
                    objField.SearchObjectFieldValue = dataRow[nameColumn].ToString();
                    objField.SearchObjectFieldID = d.FieldId;
                    objDoc.SearchObjectField.Add(objField);
                }
            }
            return objDoc;
        }

        public SearchObject GetObjectFascicoloBySystemId(string systemId, DocsPaVO.utente.InfoUtente infoUtente)
        {
            SearchObject result = null;
            string queryString = string.Empty;

            DocsPaUtils.Query q = null;
            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_FASCICOLI_GET_OBJECT_BY_SYSTEM_ID");
            q.setParam("systemId", systemId);
            q.setParam("dtaAp", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
            q.setParam("dtaCh", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
            q.setParam("idPeople", infoUtente.idPeople);
            q.setParam("idGruppo", infoUtente.idGruppo);
            q.setParam("idAmm", infoUtente.idAmministrazione);

            string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
            if (string.IsNullOrEmpty(idRuoloPubblico))
                idRuoloPubblico = "0";
            q.setParam("idRuoloPubblico", idRuoloPubblico);

            if (this.dbType == "SQL")
            {
                q.setParam("dbuser", getUserDB());
            }

            //rilascio il query string
            queryString = q.getSQL();

            DataSet ds = new DataSet();
            ArrayList tempArr = new ArrayList();
            using (DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteQuery(ds, queryString);

                if (ds != null && ds.Tables[0] != null)
                {
                    logger.Debug("GetDatiFascicoloCustom");
                    foreach (System.Data.DataRow dataRow in ds.Tables[0].Rows)
                    {
                        Field[] visibleFieldsTemplate = null;
                        tempArr.Add(GetDatiFascicoloCustom(ds, dataRow, visibleFieldsTemplate));
                    }
                }

                ds.Dispose();
            }

            result = (SearchObject)tempArr[0];

            return result;
        }

        public System.Collections.ArrayList GetDocumentiPagingCustom(
               DocsPaVO.utente.InfoUtente infoUtente,
               DocsPaVO.fascicolazione.Folder objFolder,
               DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca,
               int numPage,
               out int numTotPage,
               out int nRec,
               bool compileIdProfileList,
               out List<SearchResultInfo> idProfiles,
                bool gridPersonalization,
                        bool export,
                        Field[] visibleFieldsTemplate,
                        String[] documentsSystemId, int pageSize, DocsPaVO.filtri.FiltroRicerca[][] orderRicerca)
        {
            nRec = 0;
            numTotPage = 0;
            System.Collections.ArrayList listaDocumenti = new System.Collections.ArrayList();
            List<SearchResultInfo> idProfileList = null;
            FiltroRicerca contatatore_no_custom = null;
            try
            {
                string queryFrom = null;
                string sqlFilter = string.Empty;

                if (string.IsNullOrEmpty(objFolder.systemID))
                {
                    listaDocumenti = null;
                    throw new Exception("ID folder in GetDocumentiPagingCustom fascicolo è vuoto.");
                }
                if (filtriRicerca != null)
                {
                    queryFrom = string.Empty;
                    sqlFilter = this.GetFilterStringQueryDocumentiInFascicolo(filtriRicerca, ref queryFrom);
                    contatatore_no_custom = filtriRicerca[0].Where(e => e.argomento == "CONTATORE_GRIGLIE_NO_CUSTOM").FirstOrDefault();

                }

                // per SQL

                nRec = this.GetCountDocumenti(infoUtente.idGruppo, infoUtente.idPeople, objFolder.systemID, queryFrom, sqlFilter, compileIdProfileList, out idProfileList);
                // Reperimento del numero di elementi da visualizzare per pagina

                int pageSizeSqlServer = pageSize;

                // per query sqlserver:
                // il numero totale di righe da estrarre equivale 
                // al limite inferiore dell'ultima riga da estrarre
                int totalRowsSqlServer = (numPage * pageSize);
                if ((nRec - totalRowsSqlServer) <= 0)
                {
                    pageSizeSqlServer -= System.Math.Abs(nRec - totalRowsSqlServer);
                    totalRowsSqlServer = nRec;
                }



                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROFILE_SECURITY_PROJECT_COMPONENTS_DOCUMENTTYPES_NEW_CUSTOM");

                string security = string.Empty;

                bool IS_ARCHIVISTA_DEPOSITO;
                DocsPaDB.Query_DocsPAWS.Utenti ut = new Utenti();
                IS_ARCHIVISTA_DEPOSITO = ut.isUtArchivistaDeposito(infoUtente.idPeople, infoUtente.idGruppo);
                string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
                if (string.IsNullOrEmpty(idRuoloPubblico))
                    idRuoloPubblico = "0";
                if (IS_ARCHIVISTA_DEPOSITO)
                {
                    if (dbType.ToUpper() == "SQL")
                        security = " (@dbuser@.checkSecurityDocumento(A.SYSTEM_ID, @param3@, @param4@, @idRuoloPubblico@,'D') > 0)";
                    else
                        security = " (checkSecurityDocumento(A.SYSTEM_ID, @param3@, @param4@, @idRuoloPubblico@,'D') > 0)";
                }
                else
                {

                    if (IndexSecurity())
                        security = " EXISTS (select /*+index (e) */ 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in (@param3@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";
                    else
                        security = " EXISTS (select 'x' from security e where a.system_id=e.thing and e.PERSONORGROUP in (@param3@, @param4@, @idRuoloPubblico@) and e.ACCESSRIGHTS>0) ";

                }

                // Parametri specifici per query oracle
                string preferedIndex = string.Empty;
                if (!export)
                {
                    preferedIndex = "/*+ FIRST_ROWS(" + pageSize + ") */";
                }
                else
                {
                    preferedIndex = "/*+ FIRST_ROWS(" + nRec + ") */";
                }

                q.setParam("index", preferedIndex);


                //Valori campi custom
                string valoriCustom = string.Empty;

                if (visibleFieldsTemplate != null && visibleFieldsTemplate.Length > 0)
                {
                    foreach (Field d in visibleFieldsTemplate)
                    {
                        if (this.dbType == "SQL")
                        {
                            valoriCustom = valoriCustom + ", " + this.getUserDB() + ".getvalcampoprofdoc (docnumber, " + d.CustomObjectId.ToString() + ") AS A" + d.CustomObjectId.ToString();
                        }
                        else
                        {
                            valoriCustom = valoriCustom + ", " + "getvalcampoprofdoc (docnumber, " + d.CustomObjectId.ToString() + ") AS A" + d.CustomObjectId.ToString();
                        }
                    }
                }

                #region Ordinamento

                // Recupero dei filtri di ricerca relarivi all'ordinamento
                FiltroRicerca oracleField = orderRicerca[0].Where(e => e.argomento == "ORACLE_FIELD_FOR_ORDER").FirstOrDefault();
                FiltroRicerca sqlField = orderRicerca[0].Where(e => e.argomento == "SQL_FIELD_FOR_ORDER").FirstOrDefault();
                FiltroRicerca profilationField = orderRicerca[0].Where(e => e.argomento == "PROFILATION_FIELD_FOR_ORDER").FirstOrDefault();
                FiltroRicerca orderDirection = orderRicerca[0].Where(e => e.argomento == "ORDER_DIRECTION").FirstOrDefault();



                String extractFieldValue = String.Empty;
                String extractFieldValue2 = String.Empty;

                String contatore = string.Empty;
                String order = string.Empty;
                String reverseOrder = string.Empty;

                if (this.dbType == "SQL")
                {
                    // DB SQL Server
                    // Se bisogna ordinare per campo custom...
                    reverseOrder = String.Empty;
                    if (profilationField != null)
                    {
                        if (contatatore_no_custom != null && !gridPersonalization)
                        {
                            if (orderDirection.valore.Equals("ASC"))
                            {
                                extractFieldValue = ", " + " ISNULL(convert(int, @dbuser@.getcontatoredocordinamento (a.system_id, '" + contatatore_no_custom.valore + "')),'zzzzzzzzzzzz')";
                                extractFieldValue2 = "ISNULL(convert(int, @dbuser@.getcontatoredocordinamento (a.system_id, '" + contatatore_no_custom.valore + "')),'zzzzzzzzzzzz')";
                            }
                            else
                            {
                                extractFieldValue = ", " + "convert(int, @dbuser@.getcontatoredocordinamento (a.system_id, '" + contatatore_no_custom.valore + "'))";
                                extractFieldValue2 = "convert(int, @dbuser@.getcontatoredocordinamento (a.system_id, '" + contatatore_no_custom.valore + "'))";
                            }
                            contatore = ", @dbuser@.getcontatoredoc(docnumber,'R') as contatore";
                        }
                        else
                        {
                            Field d = visibleFieldsTemplate.Where(e => e.CustomObjectId.ToString() == profilationField.valore).FirstOrDefault();
                            if (orderDirection.valore.Equals("ASC"))
                            {
                                if (d.IsNumber)
                                {
                                    extractFieldValue = String.Format(", ISNULL(convert(int, @dbuser@.getValCampoProfDoc(DOCNUMBER, {0})),999999)", profilationField.valore);
                                    extractFieldValue2 = String.Format(" ISNULL(convert(int, @dbuser@.getValCampoProfDoc(DOCNUMBER, {0})),999999)", profilationField.valore);
                                }
                                else
                                {
                                    extractFieldValue = String.Format(", ISNULL(@dbuser@.getValCampoProfDoc(DOCNUMBER, {0}),'zzzzzzzzzzzz')", profilationField.valore);
                                    extractFieldValue2 = String.Format(" ISNULL(@dbuser@.getValCampoProfDoc(DOCNUMBER, {0}),'zzzzzzzzzzzz')", profilationField.valore);
                                }
                            }
                            else
                            {
                                if (d.IsNumber)
                                {
                                    extractFieldValue = String.Format(", convert(int, @dbuser@.getValCampoProfDoc(DOCNUMBER, {0}))", profilationField.valore);
                                    extractFieldValue2 = String.Format(" convert(int, @dbuser@.getValCampoProfDoc(DOCNUMBER, {0}))", profilationField.valore);
                                }
                                else
                                {
                                    extractFieldValue = String.Format(", @dbuser@.getValCampoProfDoc(DOCNUMBER, {0})", profilationField.valore);
                                    extractFieldValue2 = String.Format(" @dbuser@.getValCampoProfDoc(DOCNUMBER, {0})", profilationField.valore);
                                }
                            }
                        }

                        order = String.Format("{0} {1}", extractFieldValue2, orderDirection.valore);

                        order = order + ", ISNULL (a.dta_proto, a.creation_time) DESC";
                    }
                    else
                    {
                        // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                        if (sqlField != null)
                        {
                            // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                            extractFieldValue = String.Empty;
                            if (orderDirection.valore.Equals("ASC"))
                            {
                                if (sqlField.nomeCampo.Equals("D1") || sqlField.nomeCampo.Equals("D12"))
                                {
                                    order = "ISNULL(" + String.Format("{0} ,999999999) {1}", sqlField.valore, orderDirection.valore);
                                }
                                else
                                {
                                    if (sqlField.nomeCampo.Equals("D11") || sqlField.nomeCampo.Equals("D14") || sqlField.nomeCampo.Equals("D9"))
                                    {
                                        order = "ISNULL(" + String.Format("{0} ,'9999-12-31 23:59:59.997') {1}", sqlField.valore, orderDirection.valore);
                                    }
                                    else
                                    {
                                        order = "ISNULL(" + String.Format("{0} ,'zzzzzzzzzzzz') {1}", sqlField.valore, orderDirection.valore);
                                    }
                                }
                            }
                            else
                            {
                                order = String.Format("{0} {1}", sqlField.valore, orderDirection.valore);
                            }

                            order = order + ", ISNULL (a.dta_proto, a.creation_time) DESC";
                        }
                    }

                }
                else
                {
                    // DB ORACLE
                    // Se bisogna ordinare per campo custom...
                    reverseOrder = String.Empty;
                    if (profilationField != null)
                    {
                        if (contatatore_no_custom != null && !gridPersonalization)
                        {
                            extractFieldValue = ", " + "to_number(getcontatoredocordinamento (a.system_id, '" + contatatore_no_custom.valore + "'))";
                            extractFieldValue2 = "to_number(getcontatoredocordinamento (a.system_id, '" + contatatore_no_custom.valore + "'))";
                            contatore = ", getcontatoredoc(a.docnumber,'R') as contatore";
                        }
                        else
                        {
                            Field d = visibleFieldsTemplate.Where(e => e.CustomObjectId.ToString() == profilationField.valore).FirstOrDefault();

                            if (d.IsNumber)
                            {
                                extractFieldValue = String.Format(", to_number(getValCampoProfDocOrder(A.DOCNUMBER, {0}))", profilationField.valore);
                                extractFieldValue2 = String.Format(" to_number(getValCampoProfDocOrder(A.DOCNUMBER, {0}))", profilationField.valore);
                            }
                            else
                            {
                                extractFieldValue = String.Format(", getValCampoProfDoc(A.DOCNUMBER, {0})", profilationField.valore);
                                extractFieldValue2 = String.Format(" getValCampoProfDoc(A.DOCNUMBER, {0})", profilationField.valore);
                            }
                        }

                        order = String.Format("{0} {1}", extractFieldValue2, orderDirection.valore);

                        order = order + " NULLS LAST, NVL (a.dta_proto, a.creation_time) DESC";
                    }
                    else
                    {
                        // Altrimenti se è valorizzato il campo da utilizzare per l'ordinamento...
                        if (oracleField != null && contatatore_no_custom == null)
                        {
                            // ...creazione di filtro e impostazione dell'ordine diretto ed inverso
                            extractFieldValue = String.Empty;
                            order = String.Format("{0} {1}", oracleField.valore, orderDirection.valore);
                            order = order + " NULLS LAST, NVL (a.dta_proto, a.creation_time) DESC";
                        }
                        else
                        {
                            // Altrimenti viene creato il filtro standard
                            extractFieldValue = String.Empty;
                            //Nel caso non ho le griglie custum ma ho una tipologia con un campo profilato

                            if (contatatore_no_custom != null)
                            {
                                order = String.Format("TO_NUMBER(getcontatoredocordinamento (a.system_id, 'R')) {0}", orderDirection.valore);
                                contatore = ", getcontatoredoc(a.docnumber,'R') as contatore";
                            }
                            else
                            {
                                order = String.Format("NVL(A.DTA_PROTO,A.CREATION_TIME) {0}", orderDirection.valore);
                            }

                            order = order + " NULLS LAST, NVL (a.dta_proto, a.creation_time) DESC";
                        }
                    }
                }

                #endregion


                q.setParam("contatore", contatore);

                q.setParam("security", security);

                if (dbType.ToUpper() == "SQL")
                {
                    q.setParam("dbuser", getUserDB());
                }

                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.CREATION_DATE", false));
                q.setParam("param2", objFolder.systemID);
                q.setParam("param7", objFolder.idFascicolo);
                q.setParam("param4", infoUtente.idGruppo);
                q.setParam("param3", infoUtente.idPeople);
                q.setParam("idRuoloPubblico", idRuoloPubblico);

                if (filtriRicerca != null)
                {
                    q.setParam("param5", sqlFilter);
                    q.setParam("param6", queryFrom);
                }
                else
                    q.setParam("param5", " "); //ABBATANGELI - va in errore con il seguente codice: q.setParam("param5", " WHERE 1=1 ");


                // Parametri specifici per query sqlserver
                q.setParam("pageSize", pageSizeSqlServer.ToString());
                q.setParam("totalRows", totalRowsSqlServer.ToString());
                q.setParam("reverseOrder", reverseOrder);

                // Parametri per l'impostazione dell'ordinamento
                q.setParam("order", order);



                numTotPage = (nRec / pageSize);

                int startRow = ((numPage * pageSize) - pageSize) + 1;
                int endRow = (startRow - 1) + pageSize;

                string paging = string.Empty;
                if (this.dbType == "SQL")
                {
                    if (!export)
                    {
                        paging = "WHERE Row <= " + endRow.ToString() + " AND Row >=" + startRow.ToString();
                    }
                    else
                    {
                        paging = "WHERE Row <= " + nRec.ToString() + " AND Row >=" + startRow.ToString();
                    }
                }
                else
                {
                    if (!export)
                    {
                        paging = "WHERE ROWNUM <= " + endRow.ToString() + " ) a WHERE rnum >=" + startRow.ToString();
                    }
                    else
                    {
                        paging = "WHERE ROWNUM <= " + nRec.ToString() + " ) a WHERE rnum >=" + startRow.ToString();
                    }
                }

                string listDocuments = string.Empty;
                if (export)
                {
                    if (documentsSystemId != null &&
    documentsSystemId.Length > 0)
                    {
                        int i = 0;
                        listDocuments += " AND ( A.SYSTEM_ID IN(";
                        foreach (string id in documentsSystemId)
                        {
                            listDocuments += id;
                            if (i < documentsSystemId.Length - 1)
                            {
                                if (i % 998 == 0 && i > 0)
                                {
                                    listDocuments += ") OR A.SYSTEM_ID IN (";
                                }
                                else
                                {
                                    listDocuments += ", ";
                                }
                            }
                            else
                            {
                                listDocuments += ")";
                            }
                            i++;
                        }
                        listDocuments += ")";
                    }

                }

                q.setParam("listDocuments", listDocuments);
                q.setParam("valoriCustom", valoriCustom);

                q.setParam("paging", paging);

                q.setParam("idGruppo", infoUtente.idGruppo);
                q.setParam("idPeople", infoUtente.idPeople);

                string tipoRicevutaInteroperante = "";

                if (filtriRicerca != null)
                {
                    foreach (DocsPaVO.filtri.FiltroRicerca[] filterArray in filtriRicerca)
                    {
                        foreach (DocsPaVO.filtri.FiltroRicerca filterItem in filterArray)
                        {

                            if (filterItem.argomento.Equals("DOC_SPEDITI"))
                                tipoRicevutaInteroperante = filterItem.valore;
                        }
                    }
                }

                q.setParam("tipoRicevutaInteroperante", "'" + tipoRicevutaInteroperante + "'");

                string queryString = q.getSQL();

                logger.Debug(queryString);
                DataSet dataSet;
                Documenti doc = new Documenti();
                if (this.ExecuteQuery(out dataSet, "DOCUMENTI", queryString))
                {

                    foreach (DataRow dataRow in dataSet.Tables["DOCUMENTI"].Rows)
                    {
                        listaDocumenti.Add(doc.GetDatiDocumentoCustom(dataSet, dataRow, visibleFieldsTemplate));
                    }

                    dataSet.Dispose();

                }
                else
                {
                    logger.Debug("Errore nell'esecuzione della query in 'AppendListaDocumentiPagingCustom'");

                    throw new ApplicationException("Errore nell'esecuzione della query in 'AppendListaDocumentiPagingCustom'");
                }

                doc.Dispose();
                doc = null;

                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);


                listaDocumenti = null;
            }

            idProfiles = idProfileList;
            return listaDocumenti;
        }

        public bool IsFascicoloGeneraleFromIdFascicolo(string idFascicolo)
        {
            bool retValue = false;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                string commandText = string.Format("select cha_tipo_fascicolo from project where system_id = {0}", idFascicolo);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                    retValue = (field == "G");
            }

            return retValue;

        }

        /// <summary>
        /// Restituisce gli id dei documenti contenuti in un fascicolo. E' simile alla funzione GetIdFolderDoc 
        /// solo che questa non restituisce anche gli id dei folder
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <returns>ArrayList o 'null' se si è verificato un errore.</returns>
        public List<String> GetIdDocInFolder(string idFascicolo)
        {
            List<String> retVal = new List<string>();
            DataSet ds = new DataSet();
            try
            {
                //trova tutte le folder appartenenti al fascicolo
                string queryFolderString = "";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_Project10");

                q.setParam("param1", idFascicolo);
                queryFolderString = q.getSQL();

                this.ExecuteQuery(ds, "FOLDER", queryFolderString);

                //trova tutti i documenti appartenenti alle folders
                string queryDocString = "";
                string DocString = "";
                DocsPaUtils.Query qSelect = DocsPaUtils.InitQuery.getInstance().getQuery("S_ProjectComponents3");
                //"SELECT LINK FROM PROJECT_COMPONENTS WHERE PROJECT_ID IN (";

                for (int j = 0; j < ds.Tables["FOLDER"].Rows.Count; j++)
                {
                    queryDocString = queryDocString + ds.Tables["FOLDER"].Rows[j]["SYSTEM_ID"].ToString();
                    if (j < ds.Tables["FOLDER"].Rows.Count - 1)
                    {
                        queryDocString = queryDocString + ",";
                    }
                }
                //				queryDocString=queryDocString+")";
                qSelect.setParam("param1", queryDocString);
                DocString = qSelect.getSQL();

                //db.fillTable(queryDocString,ds,"DOC");
                this.ExecuteQuery(ds, "DOC", DocString);
                for (int k = 0; k < ds.Tables["DOC"].Rows.Count; k++)
                {
                    retVal.Add(ds.Tables["DOC"].Rows[k]["LINK"].ToString());
                }
            }
            catch (Exception e)
            {

                /*if(dbOpen)
                {
                    //db.closeConnection();

                }*/
                //throw e;
                logger.Debug(e.ToString());

                retVal = null;
            }

            return retVal;
        }

        public Fascicolo[] GetFascicoloDaCodiceNoSecurity(string codiceFasc, string idAmm, string idTitolario, bool soloGenerali)
        {
            Fascicolo[] result = null;

            DataSet ds = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Prelevamento della query da eseguire
                Query q = InitQuery.getInstance().getQuery("S_GET_FASCICOLI_DA_CODICE_NOSEC");
                q.setParam("codFasc", codiceFasc);
                q.setParam("idAmm", idAmm);
                //SAB aggiunto db_user per sql server
                q.setParam("db_user", this.getUserDB());
                if (!string.IsNullOrEmpty(idTitolario))
                {
                    q.setParam("altro", " and id_titolario in (" + idTitolario + ")");
                }
                else
                {
                    q.setParam("altro", string.Empty);
                }

                try
                {
                    // Esecuzione della query
                    int i = 0;
                    dbProvider.ExecuteQuery(out ds, "FOLDER", q.getSQL());
                    if (ds != null && ds.Tables["FOLDER"].Rows.Count > 0)
                    {
                        result = new Fascicolo[ds.Tables["FOLDER"].Rows.Count];

                        foreach (DataRow row in ds.Tables["FOLDER"].Rows)
                        {
                            Fascicolo fld = new Fascicolo();
                            fld.systemID = row["SYSTEM_ID"].ToString();
                            fld.codice = row["VAR_CODICE"].ToString();
                            fld.codiceRegistroNodoTit = row["COD_TITOLARIO"].ToString();
                            fld.descrizione = row["DESCRIPTION"].ToString();
                            fld.idTitolario = row["ID_TITOLARIO"].ToString();

                            result[i] = fld;
                            i++;
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Debug(e.Message);
                }

            }

            return result;
        }

        /// <summary>
        /// MEV CS 1.4
        /// Gabriele Melini 07/11/2013
        /// Metodo duplicato per le ricerche per codice fascicolo in Conservazione/Esibizione
        /// aggiunta nella where la condizione CHA_TIPO_PROJ='F'
        /// per evitare la duplicazione dei risultati
        /// </summary>
        /// <param name="codiceFasc"></param>
        /// <param name="idAmm"></param>
        /// <param name="idTitolario"></param>
        /// <param name="soloGenerali"></param>
        /// <returns></returns>
        public Fascicolo[] GetFascicoloDaCodiceNoSecurityConservazione(string codiceFasc, string idAmm, string idTitolario, bool soloGenerali, bool isRicFasc, string idRegistro)
        {
            Fascicolo[] result = null;

            DataSet ds = new DataSet();
            string cha_tipo_proj = "F";
            if (isRicFasc)
                cha_tipo_proj = "T";

            using (DBProvider dbProvider = new DBProvider())
            {
                // Prelevamento della query da eseguire
                Query q = InitQuery.getInstance().getQuery("S_GET_FASCICOLI_DA_CODICE_NOSEC");
                q.setParam("codFasc", codiceFasc);
                q.setParam("idAmm", idAmm);
                //SAB aggiunto db_user per sql server
                q.setParam("db_user", this.getUserDB());
                string queryWhere = string.Empty;
                if (!string.IsNullOrEmpty(idTitolario))
                {
                    //q.setParam("altro", string.Format(" and id_titolario in (" + idTitolario + ") and cha_tipo_proj='{0}' ", cha_tipo_proj));
                    queryWhere = string.Format(" and id_titolario in (" + idTitolario + ") and cha_tipo_proj='{0}' ", cha_tipo_proj);
                }
                else
                {
                    //q.setParam("altro", string.Format(" and cha_tipo_proj='{0}' ", cha_tipo_proj));
                    queryWhere = string.Format(" and cha_tipo_proj='{0}' ", cha_tipo_proj);
                }
                if (!string.IsNullOrEmpty(idRegistro))
                {
                    queryWhere = queryWhere + " and (id_registro=" + idRegistro + " or id_registro is null) ";
                }
                q.setParam("altro", queryWhere);

                try
                {
                    // Esecuzione della query
                    int i = 0;
                    dbProvider.ExecuteQuery(out ds, "FOLDER", q.getSQL());
                    if (ds != null && ds.Tables["FOLDER"].Rows.Count > 0)
                    {
                        result = new Fascicolo[ds.Tables["FOLDER"].Rows.Count];

                        foreach (DataRow row in ds.Tables["FOLDER"].Rows)
                        {
                            Fascicolo fld = new Fascicolo();
                            fld.systemID = row["SYSTEM_ID"].ToString();
                            fld.codice = row["VAR_CODICE"].ToString();
                            fld.codiceRegistroNodoTit = row["COD_TITOLARIO"].ToString();
                            fld.descrizione = row["DESCRIPTION"].ToString();
                            fld.idTitolario = row["ID_TITOLARIO"].ToString();

                            result[i] = fld;
                            i++;
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Debug(e.Message);
                }

            }

            return result;
        }

        public Fascicolo[] GetFascicoloDaCodiceConSecurity(string codiceFasc, string idAmm, string idTitolario, bool soloGenerali, DocsPaVO.utente.InfoUtente infoUtente)
        {
            Fascicolo[] result = null;

            DataSet ds = new DataSet();

            using (DBProvider dbProvider = new DBProvider())
            {
                // Prelevamento della query da eseguire
                Query q = InitQuery.getInstance().getQuery("S_GET_FASCICOLI_DA_CODICE_CONSEC");
                q.setParam("codFasc", codiceFasc);
                q.setParam("idAmm", idAmm);
                if (!string.IsNullOrEmpty(idTitolario))
                {
                    q.setParam("altro", " and id_titolario in (" + idTitolario + ")");
                }
                else
                {
                    q.setParam("altro", string.Empty);
                }

                q.setParam("idPeople", infoUtente.idPeople);
                q.setParam("idGruppo", infoUtente.idGruppo);
                // per SQL
                if (dbType.ToUpper() == "SQL")
                {
                    q.setParam("dbuser", getUserDB());
                }

                try
                {
                    // Esecuzione della query
                    int i = 0;
                    dbProvider.ExecuteQuery(out ds, "FOLDER", q.getSQL());
                    if (ds != null && ds.Tables["FOLDER"].Rows.Count > 0)
                    {
                        result = new Fascicolo[ds.Tables["FOLDER"].Rows.Count];

                        foreach (DataRow row in ds.Tables["FOLDER"].Rows)
                        {
                            Fascicolo fld = new Fascicolo();
                            fld.systemID = row["SYSTEM_ID"].ToString();
                            fld.codice = row["VAR_CODICE"].ToString();
                            fld.codiceRegistroNodoTit = row["COD_TITOLARIO"].ToString();
                            fld.descrizione = row["DESCRIPTION"].ToString();
                            fld.idTitolario = row["ID_TITOLARIO"].ToString();

                            result[i] = fld;
                            i++;
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Debug(e.Message);
                }

            }

            return result;
        }


        /// <summary>
        /// sosttituisce l'omonimo metodo in 
        /// DocsPAWS.fascicoli.ProjectManager.cs
        /// </summary>
        /// <param name="codiceFascicolo"></param>
        /// <param name="registro"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns>Fascicolo o 'null' se si è verificato un errore.</returns>
        public DocsPaVO.fascicolazione.Fascicolo getFascicoloByIdNoSecurity(string idFascicolo)
        {
            logger.Debug("GetFascicoloById");
            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;

            try
            {
                string query = "";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT_FASCICOLOByID_NO_SECURITY");
                q.setParam("param1", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_APERTURA", false));
                q.setParam("param2", DocsPaDbManagement.Functions.Functions.ToChar("A.DTA_CHIUSURA", false));
                q.setParam("param3", idFascicolo);
                // per SQL
                if (dbType.ToUpper() == "SQL")
                {
                    q.setParam("dbuser", getUserDB());
                }

                query = q.getSQL();

                System.Data.DataSet dataSet;
                this.ExecuteQuery(out dataSet, "PROJECT", query);
                if (dataSet.Tables["PROJECT"].Rows.Count > 0)
                    fascicolo = GetFascicoloLite(null, dataSet, dataSet.Tables["PROJECT"].Rows[0], false);
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                logger.Debug("F_System");

                fascicolo = null;
            }

            return fascicolo;
        }

        public DocsPaVO.fascicolazione.Fascicolo[] GetListaFascicoliPolicyConservazione(DocsPaVO.Conservazione.Policy policy, string lastSystemId)
        {
            Fascicolo[] result = null;
            string filtri = string.Empty;
            string from = string.Empty;
            string altriFiltri = string.Empty;



            if (!string.IsNullOrEmpty(policy.idTemplate) && !(policy.idTemplate).Equals("-1"))
            {
                altriFiltri = altriFiltri + " AND a.id_tipo_fasc = " + policy.idTemplate;
                if (policy.template != null)
                {
                    DocsPaDB.Query_DocsPAWS.ModelFasc model = new ModelFasc();
                    altriFiltri = altriFiltri + model.getSeriePerRicercaProfilazione(policy.template, "");
                }
            }
            if (!string.IsNullOrEmpty(policy.idStatoDiagramma) && !(policy.idStatoDiagramma).Equals("-1"))
            {
                altriFiltri = altriFiltri + " AND a.system_id IN(select di.id_project from dpa_diagrammi di where  di.id_stato = " + policy.idStatoDiagramma + " AND di.id_project = a.system_id)";
            }

            if (!string.IsNullOrEmpty(policy.idAOO) && !(policy.idAOO).Equals("-1"))
            {
                altriFiltri = altriFiltri + " AND a.ID_RUOLO_CREATORE in (select el.ID_RUOLO_IN_UO  from dpa_l_ruolo_reg el where el.ID_REGISTRO = " + policy.idAOO + ")";
            }
            if (!string.IsNullOrEmpty(policy.idRf) && !(policy.idRf).Equals("-1"))
            {
                altriFiltri = altriFiltri + " AND a.ID_RUOLO_CREATORE in (select el.ID_RUOLO_IN_UO  from dpa_l_ruolo_reg el where el.ID_REGISTRO = " + policy.idRf + ")";
            }

            if (!string.IsNullOrEmpty(policy.idUoCreatore) && !(policy.idUoCreatore).Equals("-1"))
            {
                if (!policy.uoSottoposte)
                {
                    altriFiltri = altriFiltri + " AND a.ID_UO_CREATORE = " + policy.idUoCreatore;
                }
                else
                {
                    altriFiltri = altriFiltri + " AND a.ID_UO_CREATORE IN (select c.SYSTEM_ID from dpa_corr_globali c start with c.SYSTEM_ID = " + policy.idUoCreatore + " connect by prior c.SYSTEM_ID = c.ID_PARENT AND c.CHA_TIPO_URP = 'U' AND c.ID_AMM=" + policy.idAmministrazione + ")";
                }
            }


            if (policy.includiSottoNodi)
            {
                filtri = filtri + " b.system_id IN (SELECT pr_c.system_id FROM project pr_c WHERE pr_c.id_fascicolo IN (SELECT a.system_id FROM project a " + from + " WHERE a.cha_tipo_proj = 'F' AND NVL (a.cha_tipo_fascicolo, 'N') != 'G' and a.ID_AMM = " + policy.idAmministrazione + " " + altriFiltri + " CONNECT BY PRIOR a.system_id = a.id_parent START WITH a.system_id = " + policy.classificazione + "))";
            }
            else
            {
                filtri = filtri + " b.system_id IN (SELECT pr_c.system_id FROM project pr_c WHERE pr_c.id_fascicolo IN (SELECT a.system_id FROM project a " + from + " WHERE a.cha_tipo_fascicolo != 'G' AND a.ID_AMM = " + policy.idAmministrazione + " " + altriFiltri + " and a.id_parent = " + policy.classificazione + "))";
            }

            DataSet ds = new DataSet();

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_PROJECT_WITH_POLICY");
            queryDef.setParam("filtri", filtri);

            string commandText = queryDef.getSQL();

            using (DocsPaDB.DBProvider dbProvider = new DBProvider())
            {
                dbProvider.ExecuteQuery(ds, queryDef.getSQL());

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    result = new Fascicolo[ds.Tables[0].Rows.Count];
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DocsPaVO.fascicolazione.Fascicolo tempInfo = new DocsPaVO.fascicolazione.Fascicolo();
                        tempInfo.systemID = ds.Tables[0].Rows[i]["system_id"].ToString();
                        tempInfo.idClassificazione = ds.Tables[0].Rows[i]["id_fascicolo"].ToString();
                        tempInfo.codice = ds.Tables[0].Rows[i]["description"].ToString();
                        result[i] = tempInfo;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="idFascicolo"></param>
        /// <param name="infoUtente"></param>
        /// <param name="debug"></param>
        /// <returns>Folder o 'null' se si è verificato un errore.</returns>
        public DocsPaVO.fascicolazione.Folder GetFolderByIdNoSecurity(string idPeople, string idGruppo, string idFolder)
        {
            //DocsPaWS.Utils.Database database = DocsPaWS.Utils.dbControl.getDatabase();			
            DocsPaVO.fascicolazione.Folder folderObject = new DocsPaVO.fascicolazione.Folder();

            try
            {

                //database.openConnection();

                System.Data.DataSet dataSet;// = new System.Data.DataSet();

                string query = "";
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_J_PROJECT_FOLDERByID_NOSEC");
                q.setParam("param1", idFolder);
                query = q.getSQL();


                //database.fillTable(updateString,dataSet,"FOLDER");
                this.ExecuteQuery(out dataSet, "FOLDER", query);

                if (dataSet.Tables["FOLDER"].Rows.Count > 0)
                {
                    folderObject = GetFolderData(dataSet.Tables["FOLDER"].Rows[0], dataSet.Tables["FOLDER"]);
                }
            }
            catch (Exception)
            {
                //				logger.Debug (e.Message);				


                //database.closeConnection();
                //NB: L'istruzione seguente è solo per evitare il warning
                //	in attesa di un futuro utilizzo del messaggio della variabile e.
                //string ErrMsg = e.Message;
                //throw new Exception("F_System");
                logger.Debug("F_System");

                folderObject = null;
            }

            return folderObject;
        }

        public DocsPaVO.documento.Tab GetProjectTab(string projectId, DocsPaVO.utente.InfoUtente infoUser)
        {
            DocsPaVO.documento.Tab retval = new DocsPaVO.documento.Tab();
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_PROJECT_TAB");

            queryDef.setParam("projectId", projectId);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            DataSet dataSet;
            if (this.ExecuteQuery(out dataSet, "PROFILE", commandText))
            {
                if (dataSet.Tables["PROFILE"] != null && dataSet.Tables["PROFILE"].Rows != null && dataSet.Tables["PROFILE"].Rows.Count > 0)
                {
                    retval.TransmissionsNumber = dataSet.Tables["PROFILE"].Rows[0]["trans"].ToString();
                    if (!dataSet.Tables["PROFILE"].Rows[0]["deleteSecurity"].ToString().Equals("0"))
                    {
                        retval.DeletedSecurity = true;
                    }
                }
            }

            return retval;
        }

        public bool GetIfDocumentiCountVisibleIsEgualNotVisible(
               DocsPaVO.utente.InfoUtente infoUtente,
               DocsPaVO.fascicolazione.Folder objFolder)
        {
            bool result = false;

            try
            {
                if (string.IsNullOrEmpty(objFolder.systemID))
                {
                    result = false;
                    throw new Exception("ID folder in GetIfDocumentiCountVisibleIsEgualNotVisible fascicolo è vuoto.");
                }

                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROJECT_DOCUMENTS_COUNT_VISIBLE_NOTVISIBLE");

                q.setParam("idgruppo", infoUtente.idGruppo);
                q.setParam("idpeople", infoUtente.idPeople);
                q.setParam("id", objFolder.systemID);

                string queryString = q.getSQL();

                logger.Debug(queryString);
                DataSet dataSet;
                if (this.ExecuteQuery(out dataSet, "DOCUMENTI", queryString))
                {
                    if (dataSet.Tables["DOCUMENTI"].Rows.Count > 0)
                    {
                        if (int.Parse(dataSet.Tables["DOCUMENTI"].Rows[0][0].ToString()) == int.Parse(dataSet.Tables["DOCUMENTI"].Rows[0][1].ToString()))
                            result = true;
                    }
                    else
                    {
                        // anche se non ci sono documenti il match è true: 0 è sempre uguale a 0
                        result = true;
                    }
                }
                else
                {
                    logger.Debug("Errore nell'esecuzione della query in 'GetIfDocumentiCountVisibleIsEgualNotVisible'");
                    throw new ApplicationException("Errore nell'esecuzione della query in 'GetIfDocumentiCountVisibleIsEgualNotVisible'");
                }

                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }

            return result;
        }

        public bool MoveFolder(string folderId, string parentId)
        {
            bool result = false;

            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_PROJECT_MOVE_FOLDER");

                q.setParam("id_parent", parentId);
                q.setParam("system_id", folderId);

                string updateString = q.getSQL();

                logger.Debug(updateString);
                this.ExecuteNonQuery(updateString);
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }

            return result;
        }

        //Laura 25 Marzo
        public int GetCountDocumentiCustom(
               DocsPaVO.utente.InfoUtente infoUtente,
               DocsPaVO.fascicolazione.Folder objFolder,
               DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca,
               out List<SearchResultInfo> idProfiles
               )
        {
            string queryFrom = null;
            string sqlFilter = string.Empty;
            int totDoc = 0;
            if (filtriRicerca != null)
            {
                queryFrom = string.Empty;
                sqlFilter = this.GetFilterStringQueryDocumentiInFascicolo(filtriRicerca, ref queryFrom);
            }

            totDoc = this.GetCountDocumenti(infoUtente.idGruppo, infoUtente.idPeople, objFolder.systemID, queryFrom, sqlFilter, false, out idProfiles);
            return totDoc;
        }


        /// <summary>
        /// Funzione per reperire lo stato di apertura/chiusura del fascicolo
        /// </summary>
        /// <param name="idProject">Id del fascicolo</param>
        /// <returns>stato del fascicolo/returns>
        public string GetChaStateProject(String idProject)
        {
            logger.Debug(" DocsPaDB.Query_DocssPAWS.Fascicoli.GetChaStateProject INIZIO");

            string retVal = string.Empty;
            try
            {
                // Esecuzione query ed estrazione dei dati
                Query q = InitQuery.getInstance().getQuery("S_CHA_STATO_PROJECT");
                q.setParam("idProject", idProject);
                string query = q.getSQL();
                logger.Debug("Query per recupero informazione stato apertura/chiusura del fascicolo:  " + query);
                DataSet dataSet = new DataSet();

                if (this.ExecuteQuery(out dataSet, "chaState", query))
                {
                    if (dataSet.Tables["chaState"] != null && dataSet.Tables["chaState"].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataSet.Tables["chaState"].Rows)
                        {
                            retVal = !string.IsNullOrEmpty(row["CHA_STATO"].ToString()) ? row["CHA_STATO"].ToString() : string.Empty;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("DocsPaDB.Query_DocssPAWS.Fascicoli.GetChaStateProject " + e.Message);
                return string.Empty;
            }
            logger.Debug(" DocsPaDB.Query_DocssPAWS.Fascicoli.GetChaStateProject FINE");
            return retVal;
        }

        public Folder[] GetFascicoloTemplate(string idfascicolo, string idtitolario, string idAmm)
        {
            Folder[] folders = null;

            try
            {
                DataSet dsTemplate = new DataSet();
                string idtipofascicolo = GetTipoFascicoloByIdFascicolo(idfascicolo);
                DocsPaUtils.Data.ParameterSP output = new DocsPaUtils.Data.ParameterSP("ID_OBJECT", 0, DocsPaUtils.Data.DirectionParameter.ParamOutput);

                ExecuteStoredProcedure("SP_GET_PROJECT_STRUCTURE", new ArrayList()
                {
                    new DocsPaUtils.Data.ParameterSP("ID_FASCICOLO", idtipofascicolo),
                    new DocsPaUtils.Data.ParameterSP("ID_TITOLARIO", idtitolario),
                    new DocsPaUtils.Data.ParameterSP("ID_TEMPLATE", string.Empty),
                    new DocsPaUtils.Data.ParameterSP("ID_AMM", idAmm),
                    output
                }, dsTemplate);

                if (dsTemplate.Tables[0].Rows.Count <= 0)
                    throw new Exception(string.Format(
                        "Nessuna riga trovata dalla store SP_GET_PROJECT_STRUCTURE({0},{1},'')",
                        idtipofascicolo, idtitolario));

                folders = new Folder[dsTemplate.Tables[0].Rows.Count];
                for (int i = 0; i < dsTemplate.Tables[0].Rows.Count; i++)
                {
                    DataRow row = dsTemplate.Tables[0].Rows[i];
                    folders[i] = new Folder()
                    {
                        systemID = Convert.ToString(row["SYSTEM_ID"]),
                        idParent = row["ID_PARENT"] != null ? Convert.ToString(row["ID_PARENT"]) : string.Empty,
                        descrizione = Convert.ToString(row["NAME"])
                    };
                }
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
            }

            return folders;
        }

        internal string GetTipoFascicoloByIdFascicolo(string id)
        {
            string result = string.Empty;

            try
            {
                // Esecuzione query ed estrazione dei dati
                Query q = InitQuery.getInstance().getQuery("S_TIPO_FASCICOLO");
                q.setParam("ID_FASCICOLO", id);
                DataSet data = new DataSet();
                logger.Debug(q.getSQL());
                ExecuteQuery(out data, q.getSQL());

                if (data.Tables[0].Rows.Count > 0)
                    result = Convert.ToString(data.Tables[0].Rows[0][0]);
            }
            catch (Exception e)
            {
                logger.Debug("DocsPaDB.Query_DocssPAWS.Fascicoli.GetTipoFascicoloByIdFascicolo " + e.Message);
            }

            return result;
        }

        public bool RenameFolder(string systemid, string newname)
        {
            bool result = false;

            try
            {
                Query oQuery = InitQuery.getInstance().getQuery("U_FOLDERNAME_PROJECT");
                oQuery.setParam("SYSTEM_ID", systemid);
                oQuery.setParam("NEWNAME", newname);

                string sql = oQuery.getSQL();
                logger.Debug(sql);

                ExecuteNonQuery(sql);
                result = true;
            }
            catch (Exception ex)
            {
                logger.Debug("DocsPaDB.Query_DocssPAWS.Fascicoli.RenameFolder",ex);
            }

            return result;
        }

        #region DESCRIZIONE FASCICOLO

        public bool InsertDescrioneFascicolo(DescrizioneFascicolo descrizioneFasc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            string query;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_DESCRIZIONI_FASC");
                string idProcesso = string.Empty;
                if (DBType.ToUpper().Equals("ORACLE"))
                    q.setParam("systemId", DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_DESCRIZIONI_FASC"));
                q.setParam("descrizione", descrizioneFasc.Descrizione.Replace("'", "''"));
                q.setParam("codice", descrizioneFasc.Codice.Replace("'", "''"));
                q.setParam("idRegistro", string.IsNullOrEmpty(descrizioneFasc.IdRegistro) ? "0" : descrizioneFasc.IdRegistro);
                q.setParam("idAmm", infoUtente.idAmministrazione);

                query = q.getSQL();
                logger.Debug("InsertDescrioneFascicolo: " + query);
                if (!ExecuteNonQuery(query))
                    result = false;
            }
            catch (Exception e)
            {
                logger.Error("Errore in InsertDescrioneFascicolo " + e.Message);
                result = false;
            }
            return result;
        }

        public List<DescrizioneFascicolo> GetListDescrizioniFascicolo(List<DocsPaVO.fascicolazione.FiltroDescrizioniFascicolo> filters, DocsPaVO.utente.InfoUtente infoUtente, int numPage, int pageSize, out int numTotPage, out int nRec)
        {
            logger.Debug("Inizio Metodo GetListDescrizioniFascicolo");
            List<DescrizioneFascicolo> listDescFasc = new List<DescrizioneFascicolo>();
            DescrizioneFascicolo desc;
            numTotPage = 0;
            nRec = 0;
            try
            {
                string query;
                string idTrasmSingola = string.Empty;
                string dtaAccettata = string.Empty;
                DataSet ds = new DataSet();
                string condition = BindConditionFiltersDescFasc(filters);
                nRec = GetListDescrizioniFascicoloCount(condition, infoUtente.idAmministrazione);
                if (nRec > 0)
                {
                    numTotPage = (nRec / pageSize);
                    int startRow = ((numPage * pageSize) - pageSize) + 1;
                    int endRow = (startRow - 1) + pageSize;
                    string paging = string.Empty;

                    if (DBType == "SQL")
                    {
                        paging = "WHERE Row <= " + endRow.ToString() + " AND Row >=" + startRow.ToString();
                    }
                    else
                    {
                        paging = "WHERE ROWNUM <= " + endRow.ToString() + " ) a WHERE rnum >=" + startRow.ToString();
                    }

                    DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_DESCRIZIONI_FASC");
                    q.setParam("idAmm", infoUtente.idAmministrazione);
                    q.setParam("condition", condition);
                    q.setParam("paging", paging);
                    query = q.getSQL();
                    logger.Debug("GetListDescrizioniFascicolo: " + query);

                    if (ExecuteQuery(out ds, query))
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                desc = new DescrizioneFascicolo();
                                desc.SystemId = row["SYSTEM_ID"].ToString();
                                desc.IdAmm = row["ID_AMM"].ToString();
                                desc.Descrizione = !string.IsNullOrEmpty(row["VAR_DESCRIZIONE"].ToString()) ? row["VAR_DESCRIZIONE"].ToString() : string.Empty;
                                desc.Codice = !string.IsNullOrEmpty(row["VAR_CODICE"].ToString()) ? row["VAR_CODICE"].ToString() : string.Empty;
                                desc.IdRegistro = !string.IsNullOrEmpty(row["ID_REGISTRO"].ToString()) ? row["ID_REGISTRO"].ToString() : string.Empty;
                                desc.CodRegistro = !string.IsNullOrEmpty(row["CODICE_REGISTRO"].ToString()) ? row["CODICE_REGISTRO"].ToString() : string.Empty;

                                listDescFasc.Add(desc);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                logger.Error("Errore nel Metodo GetListDescrizioniFascicolo " + e.Message);
                return null;
            }
            logger.Debug("FINE Metodo GetListDescrizioniFascicolo");
            return listDescFasc;

        }

        private int GetListDescrizioniFascicoloCount(string condition, string idAmm)
        {
            int nRec = 0;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_DESCRIZIONI_FASC_COUNT");
            q.setParam("condition", condition);
            q.setParam("idAmm", idAmm);

            string query = q.getSQL();
            logger.Debug("GetListDescrizioniFascicoloCount: " + query);

            string field;
            if (ExecuteScalar(out field, query))
                Int32.TryParse(field, out nRec);

            return nRec;
        }

        private string BindConditionFiltersDescFasc(List<DocsPaVO.fascicolazione.FiltroDescrizioniFascicolo> filters)
        {
            string condition = string.Empty;
            foreach (FiltroDescrizioniFascicolo f in filters)
            {
                switch (f.Argomento)
                {
                    case "CODICE":
                        condition += " AND UPPER(D.VAR_CODICE) LIKE '%" + f.Valore.ToUpper().Replace("'", "''") +"%'";
                        break;
                    case "DESCRIZIONE":
                        condition += " AND UPPER(D.VAR_DESCRIZIONE) LIKE '%" + f.Valore.ToUpper().Replace("'", "''") + "%'";
                        break;
                    case "REGISTRO":
                        string[] listaIdRegRf = f.Valore.Split('_');
                        string idRfReg = string.Empty;
                        foreach(string id in listaIdRegRf)
                        {
                            if (!string.IsNullOrEmpty(idRfReg))
                                idRfReg += ", ";
                            idRfReg += id;
                        }
                        condition += " AND D.ID_REGISTRO IN (" +idRfReg + ")";
                        break;
                }
            }

            
            return condition;
        }

        public bool AggiornaDescrizioneFascicolo(DescrizioneFascicolo descFasc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPA_DESCRIZIONI_FASC");
                q.setParam("systemId", descFasc.SystemId);
                q.setParam("codice", descFasc.Codice.Replace("'","''"));
                q.setParam("descrizione", descFasc.Descrizione.Replace("'", "''"));

                string query = q.getSQL();
                logger.Debug("AggiornaDescrizioneFascicolo: " + query);
                int rows = 0;
                if (!ExecuteNonQuery(query, out rows))
                {
                    result = false;
                }
            }
            catch(Exception ex)
            {
                logger.Error("Errore in AggiornaDescrizioneFascicolo " + ex.Message);
            }
            return result;
        }

        public bool EliminaDescrizioneFascicolo(string systemId, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_DESCRIZIONI_FASC");
                q.setParam("systemId", systemId);

                string query = q.getSQL();
                logger.Debug("EliminaDescrizioneFascicolo: " + query);
                if (!ExecuteNonQuery(query))
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Errore in EliminaDescrizioneFascicolo " + ex.Message);
            }
            return result;
        }

        public bool CheckPresenzaDescrizione(DescrizioneFascicolo descFasc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPA_DESCRIZIONI_FASC_BY_DESC");
                if(!string.IsNullOrEmpty(descFasc.Codice))
                    q.setParam("descrizione", " (UPPER(D.VAR_DESCRIZIONE) = '" + descFasc.Descrizione.ToUpper().Replace("'", "''") + "' OR UPPER(D.VAR_CODICE) ='" + descFasc.Codice.ToUpper().Replace("'", "''") + "')");
                else
                    q.setParam("descrizione", " UPPER(D.VAR_DESCRIZIONE) = '" + descFasc.Descrizione.ToUpper().Replace("'", "''") + "'");
                q.setParam("idRegistro", descFasc.IdRegistro);
                q.setParam("idAmm", descFasc.IdAmm);
                q.setParam("systemId", descFasc.SystemId);

                string query = q.getSQL();
                logger.Debug("CheckPresenzaDescrizione: " + query);
                string res; ;
                ExecuteScalar(out res, query);
                if (!string.IsNullOrEmpty(res) && Convert.ToInt32(res) > 0)
                    result = true;
            }
            catch (Exception ex)
            {
                logger.Error("Errore in CheckPresenzaDescrizione " + ex.Message);
            }
            return result;
        }
        #endregion

        public bool ExistsTrasmPendenteConWorkflowFascicolo(string idProject, string idRuoloInUO, string idPeople)
        {
            bool result = false;

            try
            {

                Query query = InitQuery.getInstance().getQuery("S_DPA_TRASM_PENDENTI_FASC_COUNT");
                query.setParam("idProject", idProject);
                query.setParam("idCorrGlobali", idRuoloInUO);
                query.setParam("idPeople", idPeople);

                string commandText = query.getSQL();
                logger.Debug("QUERY - " + commandText);

                using (DBProvider dbProvider = new DBProvider())
                {
                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                        if (field != "0") result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in ExistsTrasmPendenteConWorkflowFascicolo: " + e);
            }

            return result;
        }

        public bool ExistsTrasmPendenteSenzaWorkflowFascicolo(string idProject, string idRuoloInUO, string idPeople)
        {
            bool result = false;

            try
            {

                Query query = InitQuery.getInstance().getQuery("S_DPA_TRASM_IN_TODOLIST_FASC_COUNT");
                query.setParam("idProject", idProject);
                query.setParam("idCorrGlobali", idRuoloInUO);
                query.setParam("idPeople", idPeople);

                string commandText = query.getSQL();
                logger.Debug("QUERY - " + commandText);

                using (DBProvider dbProvider = new DBProvider())
                {
                    string field;
                    if (dbProvider.ExecuteScalar(out field, commandText))
                        if (field != "0") result = true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Errore in ExistsTrasmPendenteSenzaWorkflowFascicolo: " + e);
            }

            return result;
        }
    }
}

