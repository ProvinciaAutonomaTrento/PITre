using System;
using System.Data;
using DocsPaDB.Query_DocsPAWS;
using DocsPaVO.amministrazione;
using System.IO;
using System.Data.OleDb;
using System.Collections;
using System.Web;
using System.Xml;
using DocsPaVO.utente;
using RC = RubricaComune;
using log4net;
using BusinessLogic.Utenti;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BusinessLogic.Amministrazione
{
    /// <summary>
    /// Summary description for OrganigrammaManager.
    /// </summary>
    public class OrganigrammaManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(OrganigrammaManager));
        public OrganigrammaManager()
        {
        }

        #region PUBLIC

        public static string GetIDAmm(string codAmm)
        {
            string idAmm = null;
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            idAmm = dbAmm.GetIDAmm(codAmm);
            dbAmm = null;
            return idAmm;
        }

        public static string GetFormatoDominio(string idAmm)
        {
            string formato = null;
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            formato = dbAmm.GetFormatoDominio(idAmm);
            dbAmm = null;
            return formato;
        }

        public static string GetIDReg(string codReg)
        {
            string idAmm = null;
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            idAmm = dbAmm.GetIDReg(codReg);
            dbAmm = null;
            return idAmm;
        }

        /// <summary>
        /// Lista UO
        /// </summary>
        /// <param name="idParent">ID della UO padre</param>
        /// <param name="livello">livello UO</param>
        /// <param name="idAmm">ID Amministrazione</param>
        /// <returns></returns>
        public static ArrayList GetListUo(string idParent, string livello, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListUo(idParent, livello, idAmm);
            dbAmm = null;

            DocsPaVO.amministrazione.OrgUO uo = null;

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_UO_LIST"].Rows)
                {
                    uo = new DocsPaVO.amministrazione.OrgUO();

                    uo.IDCorrGlobale = row["IDCORRGLOBALE"].ToString();
                    uo.Codice = row["CODICE"].ToString();
                    uo.CodiceRubrica = row["CODICERUBRICA"].ToString();
                    uo.Descrizione = row["DESCRIZIONE"].ToString();
                    uo.IDAmministrazione = row["IDAMMINISTRAZIONE"].ToString();
                    uo.Livello = row["LIVELLO"].ToString();
                    uo.CodiceRegistroInterop = row["CODICEREGISTROINTEROP"].ToString();
                    uo.Ruoli = row["RUOLI"].ToString();
                    uo.SottoUo = row["SOTTOUO"].ToString();
                    uo.IDPeso = row["IDPESO"].ToString();

                    if (ds.Tables["AMM_UO_LIST"].Columns.Contains("CLASSIFICAUO"))
                        uo.Classifica = row["CLASSIFICAUO"].ToString();

                    if (ds.Tables["AMM_UO_LIST"].Columns.Contains("InteropRegistryId"))
                        uo.IdRegistroInteroperabilitaSemplificata = row["InteropRegistryId"].ToString();

                    if (ds.Tables["AMM_UO_LIST"].Columns.Contains("InteropRfId"))
                        uo.IdRfInteroperabilitaSemplificata = row["InteropRfId"].ToString();

                    retValue.Add(uo);

                    uo = null;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Lista UO
        /// </summary>
        /// <param name="idParent">ID della UO padre</param>
        /// <param name="livello">livello UO</param>
        /// <param name="idAmm">ID Amministrazione</param>
        /// <returns></returns>
        public static ArrayList GetListUoInReg(string idRegistro, string tipoRicerca, string ricerca)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListUoInReg(idRegistro, tipoRicerca, ricerca);
            dbAmm = null;

            DocsPaVO.amministrazione.OrgUO uo = null;

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_UO_LIST_IN_REG"].Rows)
                {
                    uo = new DocsPaVO.amministrazione.OrgUO();

                    uo.IDCorrGlobale = row["IDCORRGLOBALE"].ToString();
                    uo.Codice = row["CODICE"].ToString();
                    uo.Descrizione = row["DESCRIZIONE"].ToString();

                    retValue.Add(uo);

                    uo = null;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Lista Ruoli dell'utente
        /// </summary>
        /// <param name="idUO">ID UO</param>
        /// <returns></returns>
        public static ArrayList GetListRuoliUtente(string idPeople)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListRuoliUtente(idPeople);
            dbAmm = null;

            DocsPaVO.amministrazione.OrgRuolo ruolo = null;

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_RUOLI_LIST"].Rows)
                {
                    ruolo = new DocsPaVO.amministrazione.OrgRuolo();

                    ruolo.IDGruppo = row["IDGRUPPO"].ToString();
                    ruolo.Codice = row["CODICE"].ToString();
                    ruolo.Descrizione = row["DESCRIZIONE"].ToString();
                    ruolo.DiRiferimento = row["PREFERITO"].ToString();

                    retValue.Add(ruolo);

                    ruolo = null;
                }
            }

            return retValue;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmImpostaRuoloPreferito(InfoUtente infoUtente, string idPeople, string idGruppo)
        {
            DocsPaDocumentale.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);

            return organigrammaManager.ImpostaRuoloPreferito(idPeople, idGruppo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUO"></param>
        /// <param name="loadQualificheUtenti"></param>
        /// <returns></returns>
        public static ArrayList GetListRuoliUo(string idUO, bool loadQualificheUtenti)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListRuoli(idUO);
            dbAmm = null;

            DocsPaVO.amministrazione.OrgRuolo ruolo = null;

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_RUOLI_LIST"].Rows)
                {
                    ruolo = new DocsPaVO.amministrazione.OrgRuolo();

                    ruolo.IDCorrGlobale = row["IDCORRGLOBALE"].ToString();
                    ruolo.IDGruppo = row["IDGRUPPO"].ToString();
                    ruolo.IDTipoRuolo = row["IDTIPORUOLO"].ToString();
                    ruolo.CodiceTipoRuolo = row["CODICETIPORUOLO"].ToString();
                    ruolo.Codice = row["CODICE"].ToString();
                    ruolo.CodiceRubrica = row["CODICERUBRICA"].ToString();
                    ruolo.Descrizione = row["DESCRIZIONE"].ToString();
                    ruolo.DiRiferimento = row["DIRIFERIMENTO"].ToString();
                    ruolo.IDAmministrazione = row["IDAMMINISTRAZIONE"].ToString();
                    ruolo.Responsabile = row["RESPONSABILE"].ToString();
                    ruolo.Utenti = GetListUtentiRuolo(ruolo.IDGruppo, loadQualificheUtenti);
                    ruolo.IDPeso = row["IDPESO"].ToString();
                    ruolo.Segretario = row["SEGRETARIO"].ToString();
                    ruolo.DisabledTrasm = row["CHA_DISABLED_TRASM"].ToString();

                    retValue.Add(ruolo);

                    ruolo = null;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Lista Ruoli
        /// </summary>
        /// <param name="idUO">ID UO</param>
        /// <returns></returns>
        public static ArrayList GetListRuoliUo(string idUO)
        {
            return GetListRuoliUo(idUO, false);
        }

        /// <summary>
        /// Lista Ruoli Ricorsivo
        /// </summary>
        /// <param name="idUO">ID UO</param>
        /// <returns></returns>
        public static ArrayList GetListRuoliUoRic(string idUO, bool ricorsivo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListRuoli(idUO, ricorsivo);
            dbAmm = null;

            DocsPaVO.amministrazione.OrgRuolo ruolo = null;

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_RUOLI_LIST"].Rows)
                {
                    ruolo = new DocsPaVO.amministrazione.OrgRuolo();

                    ruolo.IDCorrGlobale = row["IDCORRGLOBALE"].ToString();
                    ruolo.IDGruppo = row["IDGRUPPO"].ToString();
                    ruolo.IDTipoRuolo = row["IDTIPORUOLO"].ToString();
                    ruolo.Codice = row["CODICE"].ToString();
                    ruolo.CodiceRubrica = row["CODICERUBRICA"].ToString();
                    ruolo.Descrizione = row["DESCRIZIONE"].ToString();
                    ruolo.DiRiferimento = row["DIRIFERIMENTO"].ToString();
                    ruolo.IDAmministrazione = row["IDAMMINISTRAZIONE"].ToString();
                    ruolo.Responsabile = row["RESPONSABILE"].ToString();
                    ruolo.Utenti = GetListUtentiRuolo(ruolo.IDGruppo);
                    ruolo.IDPeso = row["IDPESO"].ToString();

                    retValue.Add(ruolo);

                    ruolo = null;
                }
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idRuolo"></param>
        /// <param name="loadQualifiche"></param>
        /// <returns></returns>
        public static ArrayList GetListUtentiRuolo(string idRuolo, bool loadQualifiche)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListUtRuolo(idRuolo);
            dbAmm = null;

            DocsPaVO.amministrazione.OrgUtente utente = null;

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_UTENTI_LIST"].Rows)
                {
                    utente = new DocsPaVO.amministrazione.OrgUtente();

                    utente.IDCorrGlobale = row["IDCORRGLOBALE"].ToString();
                    utente.IDPeople = row["IDPEOPLE"].ToString();
                    utente.Codice = row["CODICE"].ToString();
                    utente.CodiceRubrica = row["CODICERUBRICA"].ToString();
                    utente.Nome = row["NOME"].ToString();
                    utente.Cognome = row["COGNOME"].ToString();
                    utente.IDAmministrazione = row["IDAMMINISTRAZIONE"].ToString();
                    utente.Matricola = (row["MATRICOLA"] != DBNull.Value ? row["MATRICOLA"].ToString() : null);
                    utente.Email = (row["EMAIL"] != DBNull.Value ? row["EMAIL"].ToString() : null);

                    if (loadQualifiche)
                    {
                        if (row.Table.Columns.Contains("IDUO"))
                        {
                            string idUO = (row["IDUO"] != DBNull.Value ? row["IDUO"].ToString() : string.Empty);
 
                            utente.Qualifiche = BusinessLogic.utenti.QualificheManager.GetPeopleGroupsQualifiche
                                (utente.IDAmministrazione, idUO, idRuolo, utente.IDPeople);
                        }
                    }

                    retValue.Add(utente);

                    utente = null;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Lista Utenti
        /// </summary>
        /// <param name="idRuolo">ID Ruolo</param>O</param>
        /// <returns></returns>
        public static ArrayList GetListUtentiRuolo(string idRuolo)
        {
            return GetListUtentiRuolo(idRuolo, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="ricercaPer"></param>
        /// <param name="testoDaRicercare"></param>
        /// <returns></returns>
        public static ArrayList GetListUtenti(string idAmm, string ricercaPer, string testoDaRicercare, string IDesclusi)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListUtenti(idAmm, ricercaPer, testoDaRicercare, IDesclusi);
            dbAmm = null;

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_UTENTI_LIST"].Rows)
                {
                    DocsPaVO.amministrazione.OrgUtente utente = new DocsPaVO.amministrazione.OrgUtente();

                    utente.IDCorrGlobale = row["IDCORRGLOBALE"].ToString();
                    utente.IDPeople = row["IDPEOPLE"].ToString();
                    utente.Codice = row["CODICE"].ToString();
                    utente.CodiceRubrica = row["CODICERUBRICA"].ToString();
                    utente.Nome = row["NOME"].ToString();
                    utente.Cognome = row["COGNOME"].ToString();
                    utente.IDAmministrazione = row["IDAMMINISTRAZIONE"].ToString();
                    utente.Matricola = (row["MATRICOLA"] != DBNull.Value ? row["MATRICOLA"].ToString() : null);
                    utente.Email = (row["EMAIL"] != DBNull.Value ? row["EMAIL"].ToString() : null);

                    retValue.Add(utente);
                }
            }

            return retValue;
        }

        public static ArrayList GetListUtenti(string idAmm, string ricercaPer, string testoDaRicercare)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListUtenti(idAmm, ricercaPer, testoDaRicercare);
            dbAmm = null;

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_UTENTI_LIST"].Rows)
                {
                    retValue.Add(CreateOrgUtente(row));
                }
            }

            return retValue;
        }

        /// <summary>
        /// Carica un oggetto OrgUtente *completo* di tutti i dati necessari per una modifica
        /// </summary>
        /// <param name="idPeople">id people</param>
        /// <returns>utente</returns>
        public static OrgUtente GetUtenteByIdPeople(string idPeople)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            DataSet ds = dbAmm.GetUtenteByIdPeople(idPeople);
            dbAmm = null;

            OrgUtente user = null;

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {               
               user = CreateOrgUtente(ds.Tables[0].Rows[0]);       
            }

            return user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idAmministrazione"></param>
        /// <param name="pagingContext"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.OrgUtente[] GetUtentiNodiTitolario(string idAmministrazione, string[] codiciNodiTitolario, DocsPaVO.filtri.FiltroRicerca[] filters, DocsPaVO.ricerche.SearchPagingContext pagingContext)
        {
            using (DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                return amm.GetUtentiNodiTitolario(idAmministrazione, codiciNodiTitolario, filters, pagingContext);
        }

        /// <summary>
        /// Creazione di un nuovo oggetto "OrgUtente"
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private static DocsPaVO.amministrazione.OrgUtente CreateOrgUtente(DataRow row)
        {
            string notificaTrasm = string.Empty;

            DocsPaVO.amministrazione.OrgUtente utente = new DocsPaVO.amministrazione.OrgUtente();

            utente.IDCorrGlobale = row["IDCORRGLOBALE"].ToString();
            utente.IDPeople = row["IDPEOPLE"].ToString();
            utente.UserId = row["USERID"].ToString();
            //utente.Password = row["PASSWORD"].ToString();
            utente.Nome = row["NOME"].ToString();
            utente.Cognome = row["COGNOME"].ToString();
            utente.Codice = row["CODICE"].ToString();
            utente.CodiceRubrica = row["CODICERUBRICA"].ToString();
            utente.Amministratore = row["AMMINISTRATORE"].ToString();
            utente.Email = row["EMAIL"].ToString();
            utente.FromEmail = row["FROM_EMAIL"].ToString();
            utente.Abilitato = row["ABILITATO"].ToString();
            string dominio = row["DOMINIO"].ToString();
            if (dominio.Contains("@"))
            {
                int inizio = dominio.IndexOf("@");
                utente.Dominio = dominio.Substring(inizio + 1);
            }
            else
                if (dominio.Contains("\\"))
                {
                    int fine = dominio.IndexOf("\\");
                    utente.Dominio = dominio.Substring(0, fine);
                }
                else
                    utente.Dominio = row["DOMINIO"].ToString();

            utente.Sede = row["SEDE"].ToString();

            // gestione Notifica Trasmissione
            if (row["NOTIFICA"].ToString() != "" && row["NOTIFICA"].ToString().Equals("E"))
            {
                notificaTrasm = "E";
                if (row["ALLEGATO_NOTIFICA"].ToString() != "" && row["ALLEGATO_NOTIFICA"].ToString().Equals("1"))
                {
                    notificaTrasm += "D";
                }
            }
            if (row["NOTIFICA"].ToString() == null || (row["NOTIFICA"].ToString() != null && row["NOTIFICA"].ToString().Equals("")))
            {
                //sono nel caso in cui si vuole ricevere solamante gli allegati
                if (row["ALLEGATO_NOTIFICA"].ToString() != "" && row["ALLEGATO_NOTIFICA"].ToString().Equals("1"))
                {
                    notificaTrasm = "E";
                    notificaTrasm += "A";
                }
            }
            utente.NotificaTrasm = notificaTrasm;
            notificaTrasm = string.Empty;

            utente.IDAmministrazione = row["IDAMMINISTRAZIONE"].ToString();

            if (row["NESSUNA_SCADENZA_PASSWORD"] != DBNull.Value)
                utente.NessunaScadenzaPassword = row["NESSUNA_SCADENZA_PASSWORD"].ToString().Equals("1");

            // Flag per forzare la sincronizzazione da ldap
            utente.SincronizzaLdap = (utente.Amministratore == "0" && (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(row, "LDAP_NEVER_SYNC", true, "1") == "0"));

            // Id utilizzato per determinare l'univocità dell'utente nell'ambito della sincronizzazione LDAP
            utente.IdSincronizzazioneLdap = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(row, "LDAP_ID_SYNC", true, utente.UserId);

            // Flag che indica se l'utente si autentica in LDAP
            utente.AutenticatoInLdap = (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(row, "LDAP_AUTHENTICATED", true, "0") == "1");

            utente.IdClientSideModelProcessor = Convert.ToInt32(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(row, "ID_CLIENT_MODEL_PROCESSOR", true, 0));

            //utente.SmartClientConfigurations.IsEnabled = (DocsPaUtils.Data.DataReaderHelper.GetValue<string>(row, "IS_ENABLED_SMART_CLIENT", true, "0") == "1");
            utente.SmartClientConfigurations.ComponentsType = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(row, "CHA_TIPO_COMPONENTI", true, "0");

            utente.SmartClientConfigurations.ApplyPdfConvertionOnScan =
                    ((utente.SmartClientConfigurations.ComponentsType!="0" && utente.SmartClientConfigurations.ComponentsType!="1") &&
                    DocsPaUtils.Data.DataReaderHelper.GetValue<string>(row, "SMART_CLIENT_PDF_CONV_ON_SCAN", true, "0") == "1");
            utente.DispositivoStampa = Convert.ToInt32(DocsPaUtils.Data.DataReaderHelper.GetValue<object>(row, "ID_DISPOSITIVO_STAMPA", true, 0));

            utente.Matricola = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(row, "MATRICOLA", true);
            utente.AbilitatoChiaviConfigurazione = false;
            if (row["ABILITATO_CHIAVI_CONFIG"].ToString() == "1")
                utente.AbilitatoChiaviConfigurazione = true;

            if (row.Table.Columns.Contains("ABILITATO_CENTRO_SERVIZI") && row["ABILITATO_CENTRO_SERVIZI"] != DBNull.Value)
            {

                if (row["ABILITATO_CENTRO_SERVIZI"].ToString() == "1")
                {
                    utente.AbilitatoCentroServizi = true;
                }

            }

            //
            // Mev CS 1.4
            if (row.Table.Columns.Contains("ABILITATO_ESIBIZIONE") && row["ABILITATO_ESIBIZIONE"] != DBNull.Value)
            {

                if (row["ABILITATO_ESIBIZIONE"].ToString() == "1")
                {
                    utente.AbilitatoEsibizione = true;
                }

            }
            // End Mev CS 1.4
            //

            return utente;
        }

        public static ArrayList GetUtenti()
        {
            DataSet ds = null;
            using (DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                ds = dbAmm.GetListUtenti();

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_UTENTI_LIST"].Rows)
                {
                    retValue.Add(CreateOrgUtente(row));
                }
            }

            return retValue;
        }


        public static ArrayList GetListUtenti(string idAmm)
        {
            DataSet ds = null;
            using (DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
                ds = dbAmm.GetListUtenti(idAmm);

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_UTENTI_LIST"].Rows)
                {
                    retValue.Add(CreateOrgUtente(row));
                }
            }

            return retValue;
        }

        /// <summary>
        /// Lista dei registri o degli RF o entrambi
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="idRuolo"></param>
        /// <param name="chaRF">1, se voglio solo gli RF, 0 solo i registri, "" tutti</param>
        /// <returns></returns>
        public static ArrayList GetListRegistriRF(string idAmm, string idRuolo, string chaRF)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListRegRF(idAmm, idRuolo, chaRF);
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
                    registro.IDAmministrazione = row["IDAMMINISTRAZIONE"].ToString();

                    if (idRuolo != null && idRuolo != string.Empty)
                    {
                        registro.Associato = row["ASSOCIATO"].ToString();
                    }
                    else
                    {
                        registro.Associato = null;
                    }
                    if (row["RF"] != null && row["RF"].ToString() != string.Empty)
                    {
                        registro.chaRF = row["RF"].ToString();
                    }
                    if (row["RFDISABILITATO"] != null && row["RFDISABILITATO"].ToString() != string.Empty)
                    {
                        registro.rfDisabled = row["RFDISABILITATO"].ToString();
                        if (row["RFDISABILITATO"].ToString().Equals("1"))
                            registro.Sospeso = true;
                        else
                            registro.Sospeso = false;
                    }
                    if (row["AOOCOLLEGATA"] != null && row["AOOCOLLEGATA"].ToString() != string.Empty)
                    {
                        registro.idAOOCollegata = row["AOOCOLLEGATA"].ToString();
                    }

                    registro.Stato = row["STATO"].ToString();
                    if (registro.Stato.Equals("S"))
                        registro.Sospeso = true;

                    retValue.Add(registro);

                    registro = null;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Lista Registri associati al ruolo
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public static ArrayList GetListRegistriAssRuolo(string idAmm, string idRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListRegAssRuolo(idAmm, idRuolo);
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
                    registro.IDAmministrazione = row["IDAMMINISTRAZIONE"].ToString();
                    registro.data_inizio = row["DATA_ASSOCIAZIONE"].ToString();
                    registro.data_ass_visibilita = row["DATA_VISIBILITA"].ToString();
                    if (row["RF"] != null && row["RF"].ToString() != string.Empty)
                    {
                        registro.rfDisabled = row["RF"].ToString();
                    }
                    retValue.Add(registro);

                    registro = null;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Lista Funzioni
        /// </summary>
        /// <param name="idAmm"></param>
        /// <param name="idRuolo"></param>
        /// <returns></returns>
        public static ArrayList GetListFunzioni(string idAmm, string idRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListFunz(idRuolo, idAmm);
            dbAmm = null;

            DocsPaVO.amministrazione.OrgTipoFunzione funzione = null;

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_FUNZIONI_LIST"].Rows)
                {
                    funzione = new DocsPaVO.amministrazione.OrgTipoFunzione();

                    funzione.IDTipoFunzione = row["IDTIPOFUNZIONE"].ToString();
                    funzione.Codice = row["CODICE"].ToString();
                    funzione.Descrizione = row["DESCRIZIONE"].ToString();
                    funzione.IDAmministrazione = idAmm;
                    funzione.Associato = row["ASSOCIATO"].ToString();

                    retValue.Add(funzione);

                    funzione = null;
                }
            }

            return retValue;
        }

        public static ArrayList GetListTipiRuolo(string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListTipiRuolo(idAmm);
            dbAmm = null;

            DocsPaVO.amministrazione.OrgTipoRuolo tipoRuolo = null;

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_TIPI_RUOLO_LIST"].Rows)
                {
                    tipoRuolo = new DocsPaVO.amministrazione.OrgTipoRuolo();

                    tipoRuolo.IDTipoRuolo = row["IDTIPORUOLO"].ToString();
                    tipoRuolo.Codice = row["CODICE"].ToString();
                    tipoRuolo.Descrizione = row["DESCRIZIONE"].ToString();
                    tipoRuolo.Livello = row["LIVELLO"].ToString();
                    tipoRuolo.IDAmministrazione = idAmm;

                    retValue.Add(tipoRuolo);

                    tipoRuolo = null;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Dati utente selezionato
        /// </summary>
        /// <param name="idCorrGlob"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.OrgUtente GetDatiUtente(string idCorrGlob)
        {
            string notificaTrasm = "";
            string dominio = "";

            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetDatiUt(idCorrGlob);
            dbAmm = null;

            DocsPaVO.amministrazione.OrgUtente utente = null;
            
            if (ds.Tables.Count > 0)
            {

                foreach (DataRow row in ds.Tables["AMM_DATI_UTENTE"].Rows)
                {
                    utente = new DocsPaVO.amministrazione.OrgUtente();

                    utente.IDCorrGlobale = idCorrGlob;
                    utente.IDPeople = row["IDPEOPLE"].ToString();
                    utente.UserId = row["USERID"].ToString();
                    utente.Codice = row["CODICE"].ToString();
                    utente.CodiceRubrica = row["CODICERUBRICA"].ToString();
                    utente.Nome = row["NOME"].ToString();
                    utente.Cognome = row["COGNOME"].ToString();
                    utente.Email = row["EMAIL"].ToString();
                    utente.Sede = row["SEDE"].ToString();
                    utente.Password = row["PASSWORD"].ToString();
                    utente.Abilitato = row["ABILITATO"].ToString();

                    DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
                    dominio = obj.GetUserNetworkAliases(row["IDPEOPLE"].ToString());
                    if (dominio != String.Empty && dominio != null)
                    {
                        int pos = dominio.IndexOf(@"\");
                        if (pos > 0)
                        {
                            dominio = dominio.Substring(0, pos);
                        }
                    }
                    utente.Dominio = dominio;

                    utente.Amministratore = row["AMMINISTRATORE"].ToString();

                    // gestione Notifica Trasmissione
                    if (row["NOTIFICA"].ToString() != "" && row["NOTIFICA"].ToString().Equals("E"))
                    {
                        notificaTrasm = "E";
                        if (row["ALLEGATO_NOTIFICA"].ToString() != "" && row["ALLEGATO_NOTIFICA"].ToString().Equals("1"))
                        {
                            notificaTrasm += "D";
                        }
                    }
                    utente.NotificaTrasm = notificaTrasm;

                    utente.IDAmministrazione = row["IDAMMINISTRAZIONE"].ToString();

                    utente.Matricola = (row["MATRICOLA"] != DBNull.Value ? row["MATRICOLA"].ToString() : null);

                    utente.AbilitatoChiaviConfigurazione = (row["abilitato_chiavi_config"].ToString() == "1" ? true : false);              
                }
            }

            return utente;
        }


        /// <summary>
        /// Recupera il ruolo responsabile della UO
        /// </summary>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.OrgRuolo AmmGetRuoloResponsabileUO(string idUo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.AmmGetRuoloResponsabileUO(idUo);
            dbAmm = null;

            DocsPaVO.amministrazione.OrgRuolo ruolo = null;

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["RUOLO_RESP"].Rows)
                {
                    ruolo = new DocsPaVO.amministrazione.OrgRuolo();

                    ruolo.IDCorrGlobale = row["SYSTEM_ID"].ToString();
                    ruolo.IDGruppo = row["ID_GRUPPO"].ToString();
                    ruolo.IDTipoRuolo = row["ID_TIPO_RUOLO"].ToString();
                    ruolo.Codice = row["VAR_CODICE"].ToString();
                    ruolo.CodiceRubrica = row["VAR_COD_RUBRICA"].ToString();
                    ruolo.Descrizione = row["VAR_DESC_CORR"].ToString();
                    ruolo.DiRiferimento = row["CHA_RIFERIMENTO"].ToString();
                    ruolo.IDAmministrazione = row["ID_AMM"].ToString();
                    ruolo.Responsabile = row["CHA_RESPONSABILE"].ToString();
                    ruolo.Utenti = GetListUtentiRuolo(ruolo.IDGruppo);

                }
            }

            return ruolo;

        }
        /// <summary>
        /// Inserimento nuova UO
        /// </summary>
        /// <param name="nuovaUO"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.EsitoOperazione AmmInsNuovaUO(DocsPaVO.amministrazione.OrgUO nuovaUO)
        {
            string queryCond = string.Empty;
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            //verifica se il codice è univoco
            DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
            if (nuovaUO.IDAmministrazione != null || nuovaUO.IDAmministrazione != string.Empty)
                queryCond = " AND ID_AMM = " + nuovaUO.IDAmministrazione;
            if (!obj.CheckUniqueCode("DPA_CORR_GLOBALI", "VAR_CODICE", nuovaUO.Codice, "AND DTA_FINE IS NULL" + queryCond))
            {
                esito.Codice = 1;
                esito.Descrizione = "codice già presente.";
            }
            else
            {
                if (dbAmm.AmmInsNuovaUO(nuovaUO))
                {
                    //					// se è interoperante, inserisce il canale (MAIL)
                    //					if(nuovaUO.CodiceRegistroInterop != null || nuovaUO.CodiceRegistroInterop != "")
                    //					{
                    //						string system_id = obj.GetUOByName (nuovaUO.Codice, nuovaUO.IDAmministrazione);
                    //						//if(!dbAmm.AmmInsCanaleCorr("(select system_id from dpa_corr_globali where var_codice = '" + nuovaUO.Codice + "' and id_amm = " + nuovaUO.IDAmministrazione + ")"))
                    //						if(!dbAmm.AmmInsCanaleCorr(system_id))
                    //						{
                    //								esito.Codice = 3;
                    //								esito.Descrizione = "si è verificato un errore: inserimento del canale preferenziale della UO " + nuovaUO.Descrizione;
                    //						}
                    //					}
                }
                else
                {
                    esito.Codice = 2;
                    esito.Descrizione = "si è verificato un errore: inserimento UO " + nuovaUO.Descrizione;
                }
            }

            obj = null;
            dbAmm = null;

            return esito;
        }

        /// <summary>
        /// Modifica dei dati della UO
        /// </summary>
        /// <param name="theUO"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.EsitoOperazione AmmModUO(DocsPaVO.amministrazione.OrgUO theUO, bool StoricizzUO)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            ////verifica se il codice è univoco
            //DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();			
            //if(!obj.CheckUniqueCode("DPA_CORR_GLOBALI","VAR_CODICE",theUO.Codice,"AND ID_AMM = " + theUO.IDAmministrazione))
            //{
            //    esito.Codice = 1;
            //    esito.Descrizione = "codice già presente.";
            //}
            //else
            //{
                if (!dbAmm.AmmModUO(theUO, StoricizzUO))
                {
                    esito.Codice = 2;
                    esito.Descrizione = "si è verificato un errore: modifica della UO " + theUO.Descrizione;
                }
            //}

            //			obj = null;
            dbAmm = null;

            return esito;
        }

        public static DocsPaVO.utente.Amministrazione AmmModificaUoTIBCO(string oldCodiceUO, DocsPaVO.amministrazione.OrgUO theUO, out bool result)
        {
            /*3 CASI:
             * INSERIMENTO NELLA TABELLA TIBCO:
             *  - accedo alla tabella TIBCO tramite codice_ipa_amm, codice_ipa_aoo e codice UO
             *  - se non lo trova invio email al referente dell'amministrazione e job
             *  - altrimenti modifica l'intero record
             * MODIFICA:
             *  - accedo alla tabella TIBCO tramite codice_ipa_amm, codice_ipa_aoo e codice UO
             *  - individuato il record aggiorno il codice UO
             */
                
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.AmmModificaUoTIBCO(oldCodiceUO, theUO, out result);
        }

        public static DocsPaVO.utente.Amministrazione AmmEliminaUoTIBCO(DocsPaVO.amministrazione.OrgUO theUO, out bool result)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.AmmEliminaUoTIBCO(theUO, out result);
        }



        public static void inviaNotificaMail(DocsPaVO.amministrazione.OrgUO theUO, DocsPaVO.utente.Amministrazione amm, string descrizioneAOO, string tipoOperazione, string oldCodiceUO)
        {
            string body = costruisciBodyEmail(theUO, amm, tipoOperazione, oldCodiceUO, descrizioneAOO);
            string[] emails = amm.email.Split(';');
            //Casella mittente a cui inviare la notifica della modifica della UO
            DocsPaVO.amministrazione.CasellaRegistro casellaMittente = new DocsPaVO.amministrazione.CasellaRegistro();
            casellaMittente.ServerSMTP = "relay.infotn.it";
        //    casellaMittente.UserMail = "KIT00430";
          //  casellaMittente.PwdMail = "WX2W8XVH";
            casellaMittente.PortaSMTP = 25 ;
            casellaMittente.SmtpSSL = "0";
            casellaMittente.PopSSL = "0";
            casellaMittente.SmtpSta = "0";
            casellaMittente.EmailRegistro = "amministrazioneFE@infotn.it";

            Interoperabilità.SvrPosta svr = new Interoperabilità.SvrPosta(casellaMittente.ServerSMTP, casellaMittente.UserMail, casellaMittente.PwdMail, casellaMittente.PortaSMTP.ToString(), Path.GetTempPath(), BusinessLogic.Interoperabilità.CMClientType.SMTP, casellaMittente.SmtpSSL, casellaMittente.PopSSL, casellaMittente.SmtpSta);
            svr.connect();

            //Invio la notifica a ciascuna email presente nell'array
            foreach (string m in emails)
                svr.sendMail(casellaMittente.EmailRegistro, m.Trim(), "Aggiornamento UO per indice IPA", body);
            svr.disconnect();
        }

        private static string costruisciBodyEmail(DocsPaVO.amministrazione.OrgUO theUO, DocsPaVO.utente.Amministrazione amm, string tipoOperazione, string oldCodiceUO, string descrizioneAOO)
        {
            string bodyMail = string.Empty;
            bodyMail = "Alla cortese attenzione del responsabile IPA<br>";
            switch (tipoOperazione)
            { 
                case "M":                   
                    bodyMail = bodyMail + "si segnala che in data " + DateTime.Today.ToLongDateString() + " è stata effettuata la seguente modifica:<br>";
                    bodyMail = bodyMail + "CODICE UO: "+ oldCodiceUO +"<br>NUOVO CODICE UO: " + theUO.Codice + "<br>DESCRIZIONE UO: " + theUO.Descrizione + ".<br>La UO è appartenente all'amministrazione - " + amm.descrizione + " ed è associata al registro di AOO - " + descrizioneAOO + ".<br>Si prega di effettuare l'aggiornamento sull'indice IPA.";
                    break;

                case "I":
                    bodyMail = bodyMail + "si segnala che in data " + DateTime.Today.ToLongDateString() + " è stata effettuato il seguente inserimento:<br>";
                    bodyMail = bodyMail + "CODICE UO: " + theUO.Codice + "<br>DESCRIZIONE UO: " + theUO.Descrizione + ".<br>La UO è appartenente all'amministrazione - " + amm.descrizione + " ed è associata al registro di AOO - " + descrizioneAOO + ".<br>Si prega di effettuare l'aggiornamento sull'indice IPA.";
                    break;

                case "C":
                    bodyMail = bodyMail + "si segnala che in data " + DateTime.Today.ToLongDateString() + " è stata effettuata la seguente eliminazione:<br>";
                    bodyMail = bodyMail + "CODICE UO: " + theUO.Codice + "<br>DESCRIZIONE UO: " + theUO.Descrizione + ".<br>La UO è appartenente all'amministrazione - " + amm.descrizione + " ed è associata al registro di AOO - " + descrizioneAOO + ".<br>Si prega di effettuare l'aggiornamento sull'indice IPA.";
                    break;
            }

            return bodyMail;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmEliminaUO(DocsPaVO.utente.InfoUtente infoUtente, string idCorrGlob)
        {
            // Reperimento delle configurazioni per la gestione della rubrica comune da docspa
            DocsPaVO.RubricaComune.ConfigurazioniRubricaComune config = RubricaComune.Configurazioni.GetConfigurazioni(infoUtente);

            string codiceRubricaComune = string.Empty;

            if (config.GestioneAbilitata)
            {
                // Reperimento del codice dell'unità organizzativa per la rubrica comune
                codiceRubricaComune = RubricaComune.RubricaServices.GetCodiceRubricaUO(infoUtente, idCorrGlob);
            }

            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            if (!dbAmm.AmmEliminaUO(idCorrGlob))
            {
                esito.Codice = 2;
                esito.Descrizione = "si è verificato un errore: cancellazione della UO";
            }
            else if (!string.IsNullOrEmpty(codiceRubricaComune))
            {
                // Dopo la cancellazione dell'unità organizzativa, 
                // viene rimosso (se esiste) il corrispondente elemento in rubrica comune
                RC.ElementiRubricaServices services = new RC.ElementiRubricaServices(config.ServiceRoot, config.SuperUserId, config.SuperUserPwd);

                RC.Proxy.Elementi.ElementoRubrica elementoRubrica = services.SearchSingle(codiceRubricaComune, RC.Proxy.Elementi.TipiRicercaParolaEnum.ParolaIntera);

                if (elementoRubrica != null)
                {
                    try
                    {
                        services.Delete(elementoRubrica);
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = "si è verificato un errore: cancellazione della UO in rubrica comune";

                        esito.Codice = 2;
                        esito.Descrizione = errorMessage;

                        logger.Debug(string.Format("{0}: {1}", errorMessage, ex.GetBaseException().Message));
                    }
                }
            }

            dbAmm = null;

            return esito;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmSpostaUO(DocsPaVO.amministrazione.OrgUO uoDaSpostare, DocsPaVO.amministrazione.OrgUO uoPadre)
        {
            logger.Debug("INIZIO Spostamento UO - ID: " + uoDaSpostare.IDCorrGlobale);

            int rowsAffected;
            string commandText = string.Empty;
            DocsPaUtils.Query queryDef = null;
            DocsPaDB.DBProvider dbProvider = null;

            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            //verifica se il codice è univoco sulla corr_globali
            DocsPaDB.Query_DocsPAWS.AmministrazioneXml obj = new DocsPaDB.Query_DocsPAWS.AmministrazioneXml();
            if (!obj.CheckCountCondition("DPA_CORR_GLOBALI", "UPPER(VAR_CODICE)=UPPER('" + uoDaSpostare.Codice + "') AND ID_AMM=" + uoDaSpostare.IDAmministrazione + " AND SYSTEM_ID NOT IN (" + uoDaSpostare.IDCorrGlobale + ")"))
            {
                esito.Codice = 1;
                esito.Descrizione = "codice già utilizzato da altra unità organizzativa";
            }
            else
            {
                /*		Possono esserci tre casi distinti:
						
                        la UO viene spostata sotto un altra UO...
						
                        1° caso - ...con lo stesso livello;
                        2° caso - ...con un livello più basso (spostamento verso l'alto)
                        3° caso - ...con un livello più alto (spostamento verso il basso)
						
                        Mentre per il primo caso bisogna solo modificare l'ID_PARENT della UO da spostare,
                        negli altri casi viene eseguita una Stored Procedure che ricalcola tutti i livelli
                        delle UO coinvolte nello spostamento.
                */


                //update dati UO su corrispondenti
                queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("U_DPACorrGlobali");
                queryDef.setParam("param1", "VAR_COD_RUBRICA = '" + uoDaSpostare.CodiceRubrica.Replace("'", "''") + "', VAR_CODICE = '" + uoDaSpostare.Codice.Replace("'", "''") + "', VAR_DESC_CORR = '" + uoDaSpostare.Descrizione.Replace("'", "''") + "', ID_PARENT = " + uoPadre.IDCorrGlobale);
                queryDef.setParam("param2", "SYSTEM_ID = " + uoDaSpostare.IDCorrGlobale);

                commandText = queryDef.getSQL();
                logger.Debug(commandText);

                dbProvider = new DocsPaDB.DBProvider();

                dbProvider.ExecuteNonQuery(commandText, out rowsAffected);
                if (rowsAffected == 0)
                {
                    esito.Codice = 2;
                    esito.Descrizione = "fallito aggiornamento della tabella dei corrispondenti";
                }
                else
                {
                    // prende il livello della UO padre
                    if (uoPadre.Livello.Equals(null) || uoPadre.Livello.Equals(string.Empty) || uoPadre.Livello.Equals(""))
                    {
                        queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_CORR_GLOB_GENERIC");
                        queryDef.setParam("param1", "NUM_LIVELLO");
                        queryDef.setParam("param2", "SYSTEM_ID = " + uoPadre.IDCorrGlobale);
                        commandText = queryDef.getSQL();
                        logger.Debug(commandText);
                        string livelloPadre;
                        dbProvider.ExecuteScalar(out livelloPadre, commandText);
                        if (livelloPadre != null)
                            uoPadre.Livello = livelloPadre;
                        else
                        {
                            esito.Codice = 3;
                            esito.Descrizione = "fallito reperimento del livello della UO padre";
                            return esito;
                        }
                    }

                    // re-imposta i livelli delle UO figlie alla UO da spostare
                    esito = UOManager.ImpostaLivelloUO(uoDaSpostare, uoPadre);
                }
            }

            dbAmm = null;
            obj = null;

            return esito;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="newRuolo"></param>
        /// <param name="computeAtipicita">Flag che indica se bisogna calcolare l'atipicità a seguito dell'inserimento del ruolo</param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.EsitoOperazione AmmInsNuovoRuolo(InfoUtente infoUtente, DocsPaVO.amministrazione.OrgRuolo newRuolo, bool computeAtipicita)
        {
            DocsPaVO.amministrazione.EsitoOperazione retValue = null;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {

                DocsPaDocumentale.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);
                retValue = organigrammaManager.InserisciRuolo(newRuolo, computeAtipicita);

                if (retValue.Codice == 0)
                    transactionContext.Complete();
            }

            return retValue;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmModRuolo(InfoUtente infoUtente, DocsPaVO.amministrazione.OrgRuolo ruolo)
        {
            DocsPaVO.amministrazione.EsitoOperazione retValue = null;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);
                retValue = organigrammaManager.ModificaRuolo(ruolo);

                if (retValue.Codice == 0)
                    transactionContext.Complete();

            }

            return retValue;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmOnlyDisabledRole(InfoUtente infoUtente, DocsPaVO.amministrazione.OrgRuolo ruolo)
        {
            DocsPaVO.amministrazione.EsitoOperazione retValue = null;
            DocsPaDocumentale.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);
            retValue = organigrammaManager.OnlyDisabledRole(ruolo);
            return retValue;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmEliminaRuolo(InfoUtente infoUtente, DocsPaVO.amministrazione.OrgRuolo ruolo)
        {
            DocsPaVO.amministrazione.EsitoOperazione retValue = null;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);
                retValue = organigrammaManager.EliminaRuolo(ruolo);

                if (retValue.Codice == 0 || retValue.Codice == 1 || retValue.Codice == 2 || retValue.Codice == 9)
                    transactionContext.Complete();
            }

            return retValue;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmSpostaRuolo(InfoUtente infoUtente, DocsPaVO.amministrazione.OrgRuolo ruolo)
        {
            DocsPaVO.amministrazione.EsitoOperazione retValue = null;

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);
                retValue = organigrammaManager.SpostaRuolo(ruolo);

                if (retValue.Codice == 0)
                    transactionContext.Complete();
            }

            return retValue;
        }

        /// <summary>
        /// Metodo per la storicizzazione del ruolo
        /// </summary>
        /// <param name="ruolo">Ruolo da storicizzare</param>
        /// <returns>Ruolo storicizzato</returns>
        public static DocsPaVO.amministrazione.OrgRuolo AmmStoricizzaRuolo(InfoUtente infoUtente, OrgRuolo ruolo)
        {
            DocsPaDocumentale.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);
            return organigrammaManager.HistoricizeRole(ruolo);
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmInsNuovoUtente(InfoUtente infoUtente, DocsPaVO.amministrazione.OrgUtente utente)
        {
            DocsPaVO.amministrazione.EsitoOperazione result = null;

            // Creazione del contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);
                result = organigrammaManager.InserisciUtente(utente);

                if (result.Codice == 0)
                {
                    // Se l'inserimento è andato a buon fine, viene completata la transazione
                    transactionContext.Complete();
                }
            }

            return result;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmInsRegistri(DocsPaVO.amministrazione.OrgRegistro[] listaRegistri, string idUO, string idCorrGlobRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();


            // prima elimina quelli che non sono stati selezionati
            if (dbAmm.AmmEliminaRegistri(listaRegistri, idCorrGlobRuolo))
            {
                // inserisce
                if (!dbAmm.AmmInsRegistri(listaRegistri, idCorrGlobRuolo))
                {
                    esito.Codice = 2;
                    esito.Descrizione = "si è verificato un errore: inserimento dei registri";
                }
                else
                {
                    if (!dbAmm.AmmInsUOReg(idUO))
                    {
                        esito.Codice = 3;
                        esito.Descrizione = "si è verificato un errore: associazione UO / registri (SP)";
                    }
                }
            }
            else
            {
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore: eliminazione dei registri non selezionati";
            }

            dbAmm = null;

            return esito;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmInsTipoFunzioni(DocsPaVO.amministrazione.OrgTipoFunzione[] listaFunzioni)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            // Creazione del contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

                // elimina
                string idRuoloInUO = listaFunzioni[0].Associato;
                if (!dbAmm.AmmEliminaTipoFunzioniRuolo(idRuoloInUO))
                {
                    esito.Codice = 1;
                    esito.Descrizione = "si è verificato un errore: eliminazione delle funzioni";
                }
                else
                {

                    // inserisce
                    if (!dbAmm.AmmInsTipoFunzioni(listaFunzioni))
                    {
                        esito.Codice = 2;
                        esito.Descrizione = "si è verificato un errore: inserimento delle funzioni";
                    }
                }

                dbAmm = null;

                if (esito.Codice == 0)
                    transactionContext.Complete();
            }

            return esito;

        }

        /// <summary>
        /// Modifica utente in organigramma
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.EsitoOperazione AmmModUtente(InfoUtente infoUtente, DocsPaVO.amministrazione.OrgUtente utente)
        {
            DocsPaVO.amministrazione.EsitoOperazione result = new DocsPaVO.amministrazione.EsitoOperazione();

            DocsPaDocumentale.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);

            // Creazione del contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                result = organigrammaManager.ModificaUtente(utente);

                if (result.Codice == 0 || result.Codice == 5 || result.Codice == 4 || result.Codice == 6)
                    // Se l'aggiornamento è andato a buon fine, viene completata la transazione
                    transactionContext.Complete();
            }

            return result;
        }

        /// <summary>
        /// Inserimento utente in un ruolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.EsitoOperazione AmmInsUtenteInRuolo(InfoUtente infoUtente, string idPeople, string idGruppo)
        {
            DocsPaVO.amministrazione.EsitoOperazione retValue = new DocsPaVO.amministrazione.EsitoOperazione();

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);
                retValue = organigrammaManager.InserisciUtenteInRuolo(idPeople, idGruppo);

                // Richiesta di INFOTN per riattivare utenti dormienti quando vengono inseriti in un ruolo
                // Forzo una modifica per riattivare l'utente
                try
                {
                    logger.Debug("Forza modifica utente sul documentale");

                    OrgUtente orgUser = GetUtenteByIdPeople(idPeople);
                    if (orgUser != null)
                    {
                        orgUser.Abilitato = "1";

                        // il db non accetta il valore 0, deve essere a null 
                        if (orgUser.DispositivoStampa == 0)
                            orgUser.DispositivoStampa = null;

                        EsitoOperazione esitoDocu = organigrammaManager.ModificaUtente(orgUser);
                        if (esitoDocu.Codice != 0)
                            logger.WarnFormat("Errore nella modifica dell'utente sul documentale: {0}", esitoDocu.Descrizione);
                    }
                    else
                        logger.Warn("Errore nel reperimento dell'utente da modificare");           

                }
                catch (Exception e)
                {
                    logger.Warn("Errore nella modifica dell'utente sul documentale", e);
                }               

                if (retValue.Codice == 0)
                    transactionContext.Complete();
            }

            return retValue;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmInsTrasmUtente(string idPeople, string idCorrGlobRuolo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

                if (!dbAmm.AmmInsTrasmUtente(idPeople, idCorrGlobRuolo))
                {
                    esito.Codice = 1;
                    esito.Descrizione = "si è verificato un errore: inserimento trasmissioni al nuovo utente del ruolo";
                }

                dbAmm = null;

                if (esito.Codice == 0)
                    transactionContext.Complete();
            }

            return esito;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmVerificaEliminazioneUtente(DocsPaVO.amministrazione.OrgUtente utente)
        {
            /* è possibile eliminare fisicamente l'utente solo se:							
                1 - non ha creato documenti o fascicoli
                2 - non è stato mittente o destinatario di documenti						
                3 - non è mittente o destinatario di trasmissioni																		
            */

            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            try
            {
                // possibili valori di ritorno:
                // 1 - non è possibile eliminare l'utente					
                // 9 - errore generico
                // 0 - tutto ok! è possibile eliminare l'utente

                esito.Codice = 1;
                esito.Descrizione = "non è possibile eliminare l'utente";

                // verifica punto 1
                string retValue = null;
                retValue = dbAmm.AmmVerificaSecurity(utente.IDPeople);
                if (retValue != null && retValue.Equals("0"))
                {
                    // verifica punto 2
                    retValue = null;
                    retValue = dbAmm.AmmVerificaMittDestDoc(utente.IDPeople);
                    if (retValue != null && retValue.Equals("0"))
                    {
                        // verifica punto 3
                        retValue = null;
                        retValue = dbAmm.AmmVerificaMittDestTrasm(utente.IDPeople);
                        if (retValue != null && retValue.Equals("0"))
                        {
                            esito.Codice = 0;
                            esito.Descrizione = "";
                        }
                    }
                }
                dbAmm = null;
            }
            catch
            {
                esito.Codice = 9;
                esito.Descrizione = "si è verificato un errore: verifica eliminazione utente";
            }

            return esito;
        }

        /// <summary>
        /// Cancellazione utente da amministrazione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="utente"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.EsitoOperazione AmmEliminaUtente(InfoUtente infoUtente, DocsPaVO.amministrazione.OrgUtente utente)
        {
            DocsPaVO.amministrazione.EsitoOperazione retValue = new DocsPaVO.amministrazione.EsitoOperazione();

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);

                retValue = organigrammaManager.EliminaUtente(utente);

                if (retValue.Codice == 0)
                    transactionContext.Complete();
            }

            return retValue;
        }

        /// <summary>
        /// Cancellazione utente da ruolo
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idPeople"></param>
        /// <param name="idGruppo"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.EsitoOperazione AmmEliminaUtenteInRuolo(InfoUtente infoUtente, string idPeople, string idGruppo)
        {
            DocsPaVO.amministrazione.EsitoOperazione retValue = new DocsPaVO.amministrazione.EsitoOperazione();

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDocumentale.Documentale.OrganigrammaManager organigrammaManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);

                retValue = organigrammaManager.EliminaUtenteDaRuolo(idPeople, idGruppo);

                if (retValue.Codice == 0)
                {
                    DataSet ds = new DataSet();
                    DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                    ds = dbAmm.UtenteUnicoInNotificheTrasm(idPeople, idGruppo);
                    if (ds.Tables["LISTA_MODELLI"] != null && ds.Tables["LISTA_MODELLI"].Rows.Count > 0)
                    {
                        retValue.Codice = 9; // non indica errore ma NOTA
                        retValue.Descrizione += "la persona eliminata dal ruolo è risulta essere\\nunico utente con notifica nei seguenti modelli di trasmissione:\\n";
                        foreach (DataRow row in ds.Tables["LISTA_MODELLI"].Rows)
                            retValue.Descrizione += " - " + row["NOME"].ToString() + "\\n";
                    }

                    transactionContext.Complete();
                }
            }

            return retValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPeople"></param>
        /// <param name="idCorrGlobGruppo"></param>
        /// <returns></returns>
        public static DocsPaVO.amministrazione.EsitoOperazione AmmEliminaADLUtente(string idPeople, string idCorrGlobGruppo)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            // Creazione contesto transazionale
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

                if (!dbAmm.AmmEliminaADLUtente(idPeople, idCorrGlobGruppo))
                {
                    esito.Codice = 1;
                    esito.Descrizione = "si è verificato un errore: eliminazione ADL utente";
                }

                dbAmm = null;

                if (esito.Codice == 0)
                    transactionContext.Complete();
            }

            return esito;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmVerificaUtenteLoggato(string userId, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            if (!dbAmm.AmmVerificaUtenteLoggato(userId, idAmm))
            {
                esito.Codice = 1;
                esito.Descrizione = "impossibile compiere azioni su questo utente poichè al momento è connesso a DocsPA";
            }
            dbAmm = null;

            return esito;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmVerificaUtenteRespStampaRep(string userId, string roleId, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new EsitoOperazione();

            ArrayList list = dbAmm.AmmVerificaUtenteRespStampaRep(userId, roleId, idAmm);
            if (list != null && list.Count > 0)
            {
                esito.Codice = 1;
                esito.Descrizione = "impossibile rimuovere questo utente in quanto è attualmente configurato come responsabile delle stampe dei seguenti repertori:";
                foreach (string desc in list)
                {
                    esito.Descrizione = esito.Descrizione + "\\n" + desc;
                }
            }
            dbAmm = null;

            return esito;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmVerificaTrasmRuolo(string idCorrGlobRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            if (!dbAmm.AmmVerificaTrasmRuolo(idCorrGlobRuolo))
            {
                esito.Codice = 1;
                esito.Descrizione = "il ruolo possiede trasmissioni che prevedono accettazione";
            }
            dbAmm = null;

            return esito;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmRifiutaTrasmConWF(string idCorrGlobRuolo, string idGruppo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            if (!dbAmm.AmmRifiutaTrasmConWF(idCorrGlobRuolo, idGruppo))
            {
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore: rifiuto delle trasmissioni a ruolo che prevedono accettazione";
            }
            dbAmm = null;

            return esito;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmSostituzioneUtente(string idPeopleNewUT, string idCorrGlobRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            if (!dbAmm.AmmSostituzioneUtente(idPeopleNewUT, idCorrGlobRuolo))
            {
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore: passaggio delle trasmissioni tra il nuovo ed il vecchio utente";
            }
            dbAmm = null;

            return esito;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmDisabilitaUtente(InfoUtente infoUtente, string idPeople)
        {
            // possibili valori di ritorno:
            // 1 - errore generico					
            // 9 - tutto ok con messaggio all'utente
            // 0 - tutto ok
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            DocsPaDocumentale.Documentale.OrganigrammaManager orgManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

                DataSet ds = new DataSet();                

                try
                {
                    string descEsito = "l'utente ora è disabilitato.\\n\\n";
                    descEsito += "Durante la procedura di disabilitazione il sistema ha verificato che:\\n\\n";

                    // verifica se questo utente è presente in uno o più ruoli e se questi ruoli hanno altri utenti oltre questo utente
                    ds = dbAmm.AmmVerificaRuoliUtenteConAltriUtenti(idPeople);

                    if (ds.Tables["RUOLI_UTENTE"] != null && ds.Tables["RUOLI_UTENTE"].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables["RUOLI_UTENTE"].Rows)
                        {
                            descEsito += "\\n- l'utente era presente nel ruolo: " + row["GRUPPO"].ToString() + "\\n";

                            //per ogni ruolo verifica se ci sono altri utenti oltre a lui...
                            if (row["ALTRIUTENTI"].ToString().Equals("0"))
                            {
                                descEsito += "- nel ruolo " + row["GRUPPO"].ToString() + " non erano presenti altri utenti;\\n";

                                // non ci sono quindi verifica se esistono trasmissioni a ruolo con workflow...	
                                if (!dbAmm.AmmVerificaTrasmRuolo(row["IDCORRGRUPPO"].ToString()))
                                {
                                    descEsito += "- il ruolo " + row["GRUPPO"].ToString() + " aveva trasmissioni che prevedevano accettazione\\n";

                                    //esistono trasmissioni, quindi le rifiuta...
                                    if (dbAmm.AmmRifiutaTrasmConWF(row["IDCORRGRUPPO"].ToString(), row["IDGRUPPO"].ToString()))
                                    {
                                        descEsito += "pertanto sono state rifiutate per mancanza di utenti nel ruolo.\\n\\n";
                                    }
                                }
                            }
                            else
                            {
                                // ci sono altri utenti
                                descEsito += "unitamente a: nr. " + row["ALTRIUTENTI"].ToString() + " utenti.\\n";
                            }

                            // disabilita l'associazione utente / ruolo
                            dbAmm.AmmEliminaUtenteInRuolo(idPeople, row["IDGRUPPO"].ToString());

                            // disabilita l'utente da ruolo sul documentale
                            // l'operazione non è bloccante se per qualche motivo l'utente lato dctm non è nel ruolo 
                            // devo continuare il lavoro
                            EsitoOperazione esitoDocu = orgManager.EliminaUtenteDaRuolo(idPeople, row["IDGRUPPO"].ToString());
                            if (esitoDocu.Codice != 0)
                                logger.WarnFormat("Errore nella disabilitazione dell'utente dal ruolo " + row["IDGRUPPO"].ToString(), esitoDocu.Descrizione);
                            
                            // elimina ADL
                            dbAmm.AmmEliminaADLUtente(idPeople, row["IDCORRGRUPPO"].ToString());
                        }
                    }
                    else
                    {
                        descEsito += "- l'utente non è presente in nessun ruolo nell'organigramma.\\n";
                    }

                    // verifica se l'utente è presente in modelli con cessione
                    DocsPaVO.amministrazione.EsitoOperazione esitoQ = new DocsPaVO.amministrazione.EsitoOperazione();
                    esitoQ = AmmUtentePresenteInModelliConCessione(idPeople);
                    if (esitoQ.Codice > 0)
                    {
                        descEsito += "\\n- l'utente era colui che ereditava i diritti nei seguenti modelli di trasmissione con CESSIONE:\\n";
                        descEsito += esitoQ.Descrizione;
                    }

                    dbAmm.DisabilitaUtenteByIdPeople(idPeople);

                    // aggiorna utente sul documentale (replica stato attivazione su DCTM)
                    // l'operazione non deve essere bloccante
                    try
                    {
                        logger.Debug("Disabilitazione utente sul documentale");                          

                        OrgUtente orgUser = GetUtenteByIdPeople(idPeople);
                        if (orgUser != null)
                        {                  
                            orgUser.Abilitato = "0";                        

                            // il db non accetta il valore 0, deve essere a null 
                            if (orgUser.DispositivoStampa == 0)
                                orgUser.DispositivoStampa = null;

                            EsitoOperazione esitoDocu = orgManager.ModificaUtente(orgUser);
                            if (esitoDocu.Codice != 0)
                                logger.WarnFormat("Errore nella disabilitazione dell'utente sul documentale: {0}", esitoDocu.Descrizione);
                        }
                        else
                            logger.Warn("Errore nel reperimento dell'utente da modificare");      
                    }
                    catch (Exception e)
                    {
                        logger.Warn("Errore nella disabilitazione dell'utente sul documentale", e);
                    }

                    dbAmm = null;                  

                    esito.Codice = 9;
                    esito.Descrizione = descEsito;
                }
                catch
                {
                    esito.Codice = 1;
                    esito.Descrizione = "si è verificato un errore: disabilitazione utente";
                }

                if (esito.Codice != 1)
                    transactionContext.Complete();
            }

            return esito;
        }

        public static string AmmGetIdPeopleByUserIdAndIdAmm(string userId, string IdAmm) 
        {
            // possibili valori di ritorno:
            // string.Empty - errore generico					
            // IdPeople - tutto ok
            string idPeople = string.Empty;
            try
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
                idPeople = dbAmm.GetIdPeopleByUserIdAndIdAmm(userId, IdAmm);
            }
            catch (Exception e) 
            { }

            return idPeople;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmAbilitaUtente(InfoUtente infoUtente, string idPeople)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            DocsPaDocumentale.Documentale.OrganigrammaManager orgManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);
           
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

                DataSet ds = new DataSet();

                try
                {
                    string descEsito = string.Empty;
                    string numRuoli = dbAmm.AmmVerificaRuoliUtente(idPeople);

                    if (!numRuoli.Equals("0"))
                    {
                        descEsito = "l'utente ora è abilitato ma non inserito in alcun ruolo.\\n\\n";
                        descEsito += "Durante la procedura di abilitazione il sistema ha verificato che:\\n\\n";

                        descEsito += "- l'utente era già stato inserito in nr. " + numRuoli + " ruoli\\n";
                        descEsito += dbAmm.AmmElencaExRuoliUtente(idPeople);
                        //if (dbAmm.AmmAbilitaUtenteInRuoli(idPeople))
                        //{
                        //    descEsito += "ed ora sono stati riassociati all'utente.";
                        //}
                    }
                    else
                    {
                        descEsito = "l'utente ora è abilitato.\\n\\n";
                        descEsito += "Durante la procedura di abilitazione il sistema ha verificato che:\\n\\n";

                        descEsito += "l'utente non è inserito in nessun ruolo in organigramma.\\n";
                        descEsito += "Tuttavia, essendo abilitato, potrebbe accedere al sistema DocsPA privo di ruolo con il quale operare.";
                    }

                    // aggiorna utente sul documentale (replica stato attivazione su DCTM)
                    // l'operazione non deve essere bloccante
                    try
                    {
                        logger.Debug("Abilitazione utente sul documentale");

                        OrgUtente orgUser = GetUtenteByIdPeople(idPeople);
                        if (orgUser != null)
                        {
                            orgUser.Abilitato = "1";

                            // il db non accetta il valore 0, deve essere a null 
                            if (orgUser.DispositivoStampa == 0)
                                orgUser.DispositivoStampa = null;

                            EsitoOperazione esitoDocu = orgManager.ModificaUtente(orgUser);
                            if (esitoDocu.Codice != 0)
                                logger.WarnFormat("Errore nell'abilitazione dell'utente sul documentale: {0}", esitoDocu.Descrizione);
                        }
                        else
                            logger.Warn("Errore nel reperimento dell'utente da modificare");
                    }
                    catch (Exception e)
                    {
                        logger.Warn("Errore nell'abilitazione dell'utente sul documentale", e);
                    }

                    dbAmm = null;

                    esito.Codice = 9;
                    esito.Descrizione = descEsito;
                }
                catch
                {
                    esito.Codice = 1;
                    esito.Descrizione = "si è verificato un errore: abilitazione utente";
                }

                if (esito.Codice != 1)
                    transactionContext.Complete();
            }

            return esito;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmUtentePresenteInModelliConCessione(string idPeople)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

                DataSet ds = new DataSet();
                ds = dbAmm.UtentePresenteInModelliConCessione(idPeople);
                if (ds.Tables["LISTA_MODELLI"] != null && ds.Tables["LISTA_MODELLI"].Rows.Count > 0)
                {
                    esito.Codice = 1;
                    foreach (DataRow row in ds.Tables["LISTA_MODELLI"].Rows)
                        esito.Descrizione += "     - [" + row["NOME"].ToString() + "]\\n";
                }
            }
            return esito;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmEstendeVisibRuolo(InfoUtente infoUtente, string idRegistro, string idCorrGlobRuolo, string idGruppo, string idCorrGlobUO, string idAmm, string livelloRuolo, bool escludiAtipicita)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            //Chiave per le verifica attivazione funzione di atipicità
            string valoreChiaveAtipicita = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ATIPICITA_DOC_FASC");
            if (string.IsNullOrEmpty(valoreChiaveAtipicita))
                valoreChiaveAtipicita = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "ATIPICITA_DOC_FASC");

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

                try
                {
                    // gestione della visibilità anche ai pari livello (solo per le UO superiori)
                    string pariLivello = System.Configuration.ConfigurationManager.AppSettings["EST_VIS_SUP_PARI_LIV"];
                    if (pariLivello != null && pariLivello.Equals("1"))
                    {
                        pariLivello = "1";
                    }
                    else
                    {
                        pariLivello = "0";
                    }

                    //Atipicita attiva
                    if (!string.IsNullOrEmpty(valoreChiaveAtipicita) && valoreChiaveAtipicita.Equals("1"))
                    {
                        if (dbAmm.AmmEstendeVisibRuoloAtipicita(infoUtente, idRegistro, idCorrGlobRuolo, idGruppo, idCorrGlobUO, livelloRuolo, pariLivello, escludiAtipicita))
                        {
                            if (!dbAmm.AmmInsDataAssVibRuoloReg(idCorrGlobRuolo, idRegistro))
                            {
                                esito.Codice = 3;
                                esito.Descrizione = "La visibilità è stata estesa correttamente ma si è verificato un errore durante l'aggiornamento dei dati di associazione ruolo/registro";
                            }
                        }
                        else
                        {
                            esito.Codice = 2;
                            esito.Descrizione = "ERRORE mentre venivano processati i documenti/fascicoli del registro";
                        }
                    }

                    //Atipicita non attiva
                    if (string.IsNullOrEmpty(valoreChiaveAtipicita) || (!string.IsNullOrEmpty(valoreChiaveAtipicita) && valoreChiaveAtipicita.Equals("0")))
                    {
                        if (dbAmm.AmmEstendeVisibRuolo(infoUtente, idRegistro, idCorrGlobRuolo, idGruppo, idCorrGlobUO, livelloRuolo, pariLivello))
                        {
                            if (!dbAmm.AmmInsDataAssVibRuoloReg(idCorrGlobRuolo, idRegistro))
                            {
                                esito.Codice = 3;
                                esito.Descrizione = "La visibilità è stata estesa correttamente ma si è verificato un errore durante l'aggiornamento dei dati di associazione ruolo/registro";
                            }
                        }
                        else
                        {
                            esito.Codice = 2;
                            esito.Descrizione = "ERRORE mentre venivano processati i documenti/fascicoli del registro";
                        }
                    }
                }
                catch (Exception ex)
                {
                    esito.Codice = 3;
                    esito.Descrizione = "si è verificato un errore: [" + ex.ToString() + "]";
                }

                if (esito.Codice == 0)
                    transactionContext.Complete();
            }

            return esito;
        }

        public static string GetLivelloTipoRuolo(string idCorrGlobRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return dbAmm.GetLivelloTipoRuolo(idCorrGlobRuolo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="codice"></param>
        /// <param name="descrizione"></param>
        /// <param name="idAmm"></param>
        /// <param name="searchHistoricized">True se bisogna ricercare fra gli storicizzati</param>
        /// <param name="searchByCodeExact">True se bisogna effettuare una query esatta su codice</param>
        /// <returns></returns>
        public static ArrayList AmmRicercaInOrg(string tipo, string codice, string descrizione, string idAmm, bool searchHistoricized, bool searchByCodeExact)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.AmmRicercaInOrg(tipo, codice, descrizione, idAmm, searchHistoricized, searchByCodeExact);
            dbAmm = null;

            DocsPaVO.amministrazione.OrgRisultatoRicerca risultato = null;

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_RISULTATO_LIST"].Rows)
                {
                    risultato = new DocsPaVO.amministrazione.OrgRisultatoRicerca();

                    risultato.Tipo = tipo;
                    risultato.IDCorrGlob = row["IDCORRGLOB"].ToString();
                    risultato.Codice = row["CODICE"].ToString();
                    risultato.Descrizione = row["DESCRIZIONE"].ToString();
                    risultato.IDParent = row["IDPARENT"].ToString();
                    risultato.DescParent = row["DESCPARENT"].ToString();
                    if (row.Table.Columns.Contains("MATRICOLA"))
                        risultato.Matricola = (row["MATRICOLA"] != DBNull.Value ? row["MATRICOLA"].ToString() : string.Empty);

                    switch (tipo)
                    {
                        case "R":
                            risultato.IDGruppo = row["IDGRUPPO"].ToString();

                            // Se DATAFINE è valorizzato, viene pulito il codice e viene messo in grassetto
                            if (row["DATAFINE"] != null && !String.IsNullOrEmpty(row["DATAFINE"].ToString()))
                                risultato.Codice = String.Format("<div style=\"text-decoration:line-through;\"><strong>{0}</strong></div>", risultato.Codice.Remove(risultato.Codice.LastIndexOf('_')));
                            break;
                        case "PN":
                        case "PC":
                            risultato.IDPeople = row["IDPEOPLE"].ToString();
                            break;
                    }

                    retValue.Add(risultato);

                    risultato = null;
                }
            }

            return retValue;
        }

        public static ArrayList AmmListaIDParentRicerca(string IDPartenza, string tipo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            ArrayList retValue = new ArrayList();
            int idParent = Convert.ToInt32(IDPartenza);

            retValue.Add(idParent);

            while (idParent > 0)
            {
                if (tipo.Equals("U") || tipo.Equals("R"))
                {
                    idParent = dbAmm.AmmListaIDParentRicercaUO(idParent);
                }
                else
                {
                    idParent = dbAmm.AmmListaIDParentRicercaUOdaRuolo(idParent);
                    tipo = "U";
                }

                if (idParent > 0)
                    retValue.Add(idParent);
            }

            dbAmm = null;

            return retValue;
        }

        public static DocsPaVO.amministrazione.OrgUO AmmGetDatiUOCorrente(string idUO)
        {
            DocsPaVO.amministrazione.OrgUO currentUO = new DocsPaVO.amministrazione.OrgUO();

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_AMM_GET_CURRENT_UO");
            queryDef.setParam("param1", idUO);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        currentUO.IDCorrGlobale = idUO;
                        currentUO.Codice = reader.GetValue(reader.GetOrdinal("Codice")).ToString();
                        currentUO.CodiceRubrica = reader.GetValue(reader.GetOrdinal("CodiceRubrica")).ToString();
                        currentUO.Descrizione = reader.GetValue(reader.GetOrdinal("Descrizione")).ToString();
                        currentUO.IDAmministrazione = reader.GetValue(reader.GetOrdinal("IDAmministrazione")).ToString();
                        currentUO.Livello = reader.GetValue(reader.GetOrdinal("Livello")).ToString();
                        currentUO.IDParent = reader.GetValue(reader.GetOrdinal("IDParent")).ToString();
                        currentUO.CodiceRegistroInterop = reader.GetValue(reader.GetOrdinal("CodiceRegistroInterop")).ToString();
                        currentUO.Ruoli = reader.GetValue(reader.GetOrdinal("Ruoli")).ToString();
                        currentUO.SottoUo = reader.GetValue(reader.GetOrdinal("SottoUo")).ToString();
                        currentUO.Classifica = reader.GetValue(reader.GetOrdinal("ClassificaUo")).ToString();
                    }
                }
            }
            return currentUO;

        }

        public static DocsPaVO.amministrazione.OrgDettagliGlobali AmmGetDatiStampaBuste(string idCorrGlob)
        {
            DocsPaVO.amministrazione.OrgDettagliGlobali dettagli = new DocsPaVO.amministrazione.OrgDettagliGlobali();

            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPADettGlob");
            queryDef.setParam("param1", idCorrGlob);

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                {
                    if (reader.Read())
                    {
                        dettagli.Indirizzo = reader.GetValue(reader.GetOrdinal("VAR_INDIRIZZO")).ToString();
                        dettagli.Citta = reader.GetValue(reader.GetOrdinal("VAR_CITTA")).ToString();
                        dettagli.Cap = reader.GetValue(reader.GetOrdinal("VAR_CAP")).ToString();
                        dettagli.Nazione = reader.GetValue(reader.GetOrdinal("VAR_NAZIONE")).ToString();
                        dettagli.Provincia = reader.GetValue(reader.GetOrdinal("VAR_PROVINCIA")).ToString();
                        dettagli.Telefono1 = reader.GetValue(reader.GetOrdinal("VAR_TELEFONO")).ToString();
                        dettagli.Telefono2 = reader.GetValue(reader.GetOrdinal("VAR_TELEFONO2")).ToString();
                        dettagli.Fax = reader.GetValue(reader.GetOrdinal("VAR_FAX")).ToString();
                        dettagli.Note = reader.GetValue(reader.GetOrdinal("VAR_NOTE")).ToString();
                        dettagli.CodiceFiscale = reader.GetValue(reader.GetOrdinal("VAR_COD_FISC")).ToString();
                        dettagli.PartitaIva = reader.GetValue(reader.GetOrdinal("VAR_COD_PI")).ToString();
                    }
                }
            }
            return dettagli;
        }

        public static DocsPaVO.documento.FileDocumento StampaOrgInPdf(XmlDocument xmlDoc)
        {
            string XSLUrl = HttpContext.Current.Server.MapPath(@"xml/xslfo.xsl");

            DocsPaVO.documento.FileDocumento filePdf = new DocsPaVO.documento.FileDocumento();

            //StampaPDF.StampaOrganigrammaPdf.StampaOrganigrammaPdfManager manager = new StampaPDF.StampaOrganigrammaPdf.StampaOrganigrammaPdfManager();

            StampaPDF.StampaOrganigramma manager = new StampaPDF.StampaOrganigramma();
            filePdf = manager.convertPDF(xmlDoc);

            return filePdf;
        }



        #endregion

        #region rf

        public static ArrayList GetListRuoli(string idAmm, string ricercaPer, string testoDaRicercare, string idRegistro, string IDesclusi)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListRuoli(idAmm, ricercaPer, testoDaRicercare, idRegistro, IDesclusi);
            dbAmm = null;

            DocsPaVO.amministrazione.OrgRuolo ruolo = null;

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_RUOLI_LIST"].Rows)
                {
                    ruolo = new DocsPaVO.amministrazione.OrgRuolo();

                    ruolo.IDCorrGlobale = row["IDCORRGLOBALE"].ToString();
                    ruolo.IDGruppo = row["IDGRUPPO"].ToString();
                    ruolo.Codice = row["CODICE"].ToString();
                    ruolo.CodiceRubrica = row["CODICERUBRICA"].ToString();
                    ruolo.IDAmministrazione = row["IDAMMINISTRAZIONE"].ToString();
                    ruolo.Descrizione = row["DESCRIZIONE"].ToString();
                    retValue.Add(ruolo);

                    ruolo = null;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Calcola e ritorna un arrayList di OrgRuolo associati ad un registro o ad un RF
        /// la cui systemId è passata in ingresso
        /// </summary>
        /// <param name="idRegistro"></param>
        /// <returns></returns>
        public static ArrayList GetListaRuoliAOO(string idRegistro)
        {


            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DataSet ds = dbAmm.GetListaRuoliAOO(idRegistro);
            dbAmm = null;

            DocsPaVO.amministrazione.OrgRuolo ruolo = null;

            ArrayList retValue = new ArrayList();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables["AMM_RUOLI_LIST"].Rows)
                {
                    ruolo = new DocsPaVO.amministrazione.OrgRuolo();

                    ruolo.IDCorrGlobale = row["IDRUOLO"].ToString();
                    ruolo.IDGruppo = row["GRUPPO"].ToString();
                    ruolo.CodiceRubrica = row["CODICE"].ToString();
                    ruolo.IDAmministrazione = row["IDAMMINISTRAZIONE"].ToString();
                    ruolo.Descrizione = row["DESCRIZIONE"].ToString();

                    retValue.Add(ruolo);

                    ruolo = null;
                }
            }

            return retValue;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmAssociazioneRFRuolo(string idRegistro, string idCorrGlobRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            // associa il ruolo al registro
            DocsPaVO.amministrazione.OrgRegistro[] listaRegistri = new DocsPaVO.amministrazione.OrgRegistro[1];

            DocsPaVO.amministrazione.OrgRegistro registro = new DocsPaVO.amministrazione.OrgRegistro();
            registro.IDRegistro = idRegistro;
            registro.Associato = idCorrGlobRuolo;
            listaRegistri[0] = registro;

            if (!dbAmm.AmmInsRegistri(listaRegistri, idCorrGlobRuolo))
            {
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore: associazione RF a ruolo";
            }

            dbAmm = null;

            return esito;
        }

        public static DocsPaVO.amministrazione.EsitoOperazione AmmDeleteAssociazioneRFRuolo(string idRegistro, string idCorrGlobRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            // associa il ruolo al registro
            DocsPaVO.amministrazione.OrgRegistro[] listaRegistri = new DocsPaVO.amministrazione.OrgRegistro[1];

            DocsPaVO.amministrazione.OrgRegistro registro = new DocsPaVO.amministrazione.OrgRegistro();
            registro.IDRegistro = idRegistro;
            registro.Associato = idCorrGlobRuolo;
            listaRegistri[0] = registro;

            if (!dbAmm.AmmEliminaAssociazioneRFRuolo(listaRegistri, idCorrGlobRuolo))
            {
                esito.Codice = 1;
                esito.Descrizione = "si è verificato un errore: dissassociazione RF dal ruolo";
            }

            dbAmm = null;

            return esito;
        }


        #endregion

        #region ORDINAMENTO
        /// <summary>
        /// Imposta un ordinamento dei ruoli o uo in organigramma
        /// </summary>
        /// <param name="idCorrGlobDaSpostare">ID del ruolo o uo da spostare (che va su o giù)</param>
        /// <param name="idPesoDaSpostare">Peso attuale del ruolo o uo da spostare</param>
        /// <param name="idCorrGlobSubisce">ID del ruolo o uo che passa sopra o sotto rispetto al corrispondente da spostare</param>
        /// <param name="idPesoSubisce">Peso attuale del ruolo o uo che passa sopra o sotto rispetto al corrispondente da spostare</param>        
        /// <returns>Oggetto EsitoOperazione</returns>
        public static DocsPaVO.amministrazione.EsitoOperazione AmmOrdinamento(string idCorrGlobDaSpostare, string idPesoDaSpostare, string idCorrGlobSubisce, string idPesoSubisce)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();

            bool result = false;

            DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider();

            try
            {
                dbProvider.BeginTransaction();

                int rowAffected = 0;

                // prima imposta il peso del corrispondente che subisce
                string commandText = "UPDATE DPA_CORR_GLOBALI SET ID_PESO_ORG = " + idPesoDaSpostare + " WHERE SYSTEM_ID = " + idCorrGlobSubisce;
                logger.Debug(commandText);

                result = dbProvider.ExecuteNonQuery(commandText, out rowAffected);

                if (result)
                {
                    if (rowAffected == 1)
                    {
                        rowAffected = 0;

                        // quindi imposta il peso del corrispondente da spostare
                        commandText = "UPDATE DPA_CORR_GLOBALI SET ID_PESO_ORG = " + idPesoSubisce + " WHERE SYSTEM_ID = " + idCorrGlobDaSpostare;
                        logger.Debug(commandText);

                        result = dbProvider.ExecuteNonQuery(commandText, out rowAffected);

                        if (result)
                        {
                            if (rowAffected == 0 || rowAffected > 1)
                            {
                                dbProvider.RollbackTransaction();
                                esito.Codice = 1;
                                esito.Descrizione = "impossibile eseguire il nuovo ordinamento";
                            }
                            else
                            {
                                dbProvider.CommitTransaction();
                            }
                        }
                        else
                        {
                            dbProvider.CommitTransaction();
                            esito.Codice = 1;
                            esito.Descrizione = "impossibile eseguire il nuovo ordinamento";
                        }
                    }
                    else
                    {
                        dbProvider.RollbackTransaction();
                        esito.Codice = 1;
                        esito.Descrizione = "impossibile eseguire il nuovo ordinamento";
                    }
                }
                else
                {
                    dbProvider.RollbackTransaction();
                    esito.Codice = 1;
                    esito.Descrizione = "impossibile eseguire il nuovo ordinamento";
                }
            }
            catch
            {
                dbProvider.RollbackTransaction();
                esito.Codice = 1;
                esito.Descrizione = "impossibile eseguire il nuovo ordinamento";
            }

            return esito;
        }
        #endregion

        public static bool importOrganigramma(byte[] dati, string nomeFile, string serverPath, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool warning = false;

            OleDbConnection xlsConn = new OleDbConnection();
            OleDbCommand xlsCmd = null;
            OleDbDataReader xlsReaderUO = null;
            OleDbDataReader xlsReaderRuoli = null;
            OleDbDataReader xlsReaderUtenti = null;

            DocsPaDB.Query_DocsPAWS.Amministrazione amministrazione = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            if (!Directory.Exists(serverPath + "\\Modelli\\Import\\"))
                Directory.CreateDirectory(serverPath + "\\Modelli\\Import\\");

            DocsPaDB.Utils.SimpleLog sl = new DocsPaDB.Utils.SimpleLog(serverPath + "\\Modelli\\Import\\logImportazioneOrganigramma");

            try
            {
                #region Scrittura del file
                FileStream fs1 = new FileStream(serverPath + "\\Modelli\\Import\\" + nomeFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fs1.Write(dati, 0, dati.Length);
                fs1.Close();
                #endregion Scrittura del file

                #region Connessione al file excel
                xlsConn.ConnectionString = "Provider=" + System.Configuration.ConfigurationManager.AppSettings["DS_PROVIDER"] + "Data Source=" + serverPath + "\\Modelli\\Import\\" + nomeFile + ";Extended Properties='" + System.Configuration.ConfigurationManager.AppSettings["DS_EXTENDED_PROPERTIES"] + "IMEX=1';";
                xlsConn.Open();
                sl.Log("");
                sl.Log("Connessione al file : " + nomeFile + " aperta");
                #endregion Connessione al file excel

                #region Inserimento UO
                sl.Log("");
                sl.Log("**** Inizio importazione UO - " + System.DateTime.Now.ToString());

                xlsCmd = new OleDbCommand("select * from [UO$]", xlsConn);
                xlsReaderUO = xlsCmd.ExecuteReader();

                while (xlsReaderUO.Read())
                {
                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReaderUO, 0) == "/")
                        break;

                    //Controllo campi obbligatori
                    if (string.IsNullOrEmpty(get_string(xlsReaderUO, 0)) || string.IsNullOrEmpty(get_string(xlsReaderUO, 2)) || string.IsNullOrEmpty(get_string(xlsReaderUO, 3)) || string.IsNullOrEmpty(get_string(xlsReaderUO, 4)))
                    {
                        sl.Log("");
                        sl.Log("ERROR : Campi obbligatori delle UO non inseriti nel modello");
                        return false;
                    }
                    else
                    {
                        //Recupero l'idAmministrazione e lo imposto all'infoUtente 
                        //perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                        string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(get_string(xlsReaderUO, 0).ToUpper());
                        infoUtente.idAmministrazione = idAmm;

                        //Recupero la system_id dell'Amministrazione dalla DPA_CORR_GLOBALI
                        string idAmmCorrGlobali = Amministrazione.AmministraManager.getIdAmmCorrGlobali(idAmm);

                        //Creo la nuova UO da inserire
                        OrgUO newUo = new OrgUO();
                        string livelloPadre = "0";

                        //Valorizzo i campi
                        if (string.IsNullOrEmpty(get_string(xlsReaderUO, 1)))
                        {
                            newUo.IDParent = idAmmCorrGlobali;
                        }
                        else
                        {
                            //UnitaOrganizzativa uoPadre = (UnitaOrganizzativa)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReaderUO, 1).ToUpper(), infoUtente);
                            UnitaOrganizzativa uoPadre = amministrazione.getUOByCod(get_string(xlsReaderUO, 1));
                            newUo.IDParent = uoPadre.systemId;
                            livelloPadre = uoPadre.livello;
                        }

                        newUo.Livello = get_string(xlsReaderUO, 2);
                        if (!livelloPadre.Equals("0"))
                        {
                            if (Convert.ToInt32(livelloPadre) + 1 != Convert.ToInt32(newUo.Livello))
                            {
                                sl.Log("");
                                sl.Log("ERROR - " + newUo.Codice + " - Livello della UO non corretto");
                                warning = true;
                            }
                            else
                            {
                                newUo.Codice = get_string(xlsReaderUO, 3).ToUpper();
                                newUo.CodiceRubrica = get_string(xlsReaderUO, 3).ToUpper();
                                newUo.Descrizione = get_string(xlsReaderUO, 4);
                                newUo.IDAmministrazione = idAmm;
                                newUo.CodiceRegistroInterop = get_string(xlsReaderUO, 5).ToUpper();

                                newUo.DettagliUo = new OrgDettagliGlobali();
                                newUo.DettagliUo.Indirizzo = get_string(xlsReaderUO, 6);
                                newUo.DettagliUo.Citta = get_string(xlsReaderUO, 7);
                                newUo.DettagliUo.Cap = get_string(xlsReaderUO, 8);

                                if (!string.IsNullOrEmpty(get_string(xlsReaderUO, 9)) && get_string(xlsReaderUO, 9).Length >= 2)
                                    newUo.DettagliUo.Provincia = get_string(xlsReaderUO, 9).Substring(0, 2);

                                newUo.DettagliUo.Nazione = get_string(xlsReaderUO, 10);
                                newUo.DettagliUo.Telefono1 = get_string(xlsReaderUO, 11);
                                newUo.DettagliUo.Telefono2 = get_string(xlsReaderUO, 12);
                                newUo.DettagliUo.Fax = get_string(xlsReaderUO, 13);

                                //Inserisco la nuova UO
                                EsitoOperazione esito = AmmInsNuovaUO(newUo);
                                if (esito.Codice != 0)
                                {
                                    sl.Log("");
                                    sl.Log("ERROR - " + newUo.Codice + " - " + esito.Descrizione);
                                    warning = true;
                                    //return false;
                                }
                                else
                                {
                                    sl.Log("");
                                    sl.Log("UO - Con codice " + newUo.Codice + " inserita correttamente.");
                                }
                            }
                        }
                        else
                        {
                            newUo.Codice = get_string(xlsReaderUO, 3).ToUpper();
                            newUo.CodiceRubrica = get_string(xlsReaderUO, 3).ToUpper();
                            newUo.Descrizione = get_string(xlsReaderUO, 4);
                            newUo.IDAmministrazione = idAmm;
                            newUo.CodiceRegistroInterop = get_string(xlsReaderUO, 5).ToUpper();

                            newUo.DettagliUo = new OrgDettagliGlobali();
                            newUo.DettagliUo.Indirizzo = get_string(xlsReaderUO, 6);
                            newUo.DettagliUo.Citta = get_string(xlsReaderUO, 7);
                            newUo.DettagliUo.Cap = get_string(xlsReaderUO, 8);

                            if (!string.IsNullOrEmpty(get_string(xlsReaderUO, 9)) && get_string(xlsReaderUO, 9).Length >= 2)
                                newUo.DettagliUo.Provincia = get_string(xlsReaderUO, 9).Substring(0, 2);

                            newUo.DettagliUo.Nazione = get_string(xlsReaderUO, 10);
                            newUo.DettagliUo.Telefono1 = get_string(xlsReaderUO, 11);
                            newUo.DettagliUo.Telefono2 = get_string(xlsReaderUO, 12);
                            newUo.DettagliUo.Fax = get_string(xlsReaderUO, 13);

                            //Inserisco la nuova UO
                            EsitoOperazione esito = AmmInsNuovaUO(newUo);
                            if (esito.Codice != 0)
                            {
                                sl.Log("");
                                sl.Log("ERROR - " + newUo.Codice + " - " + esito.Descrizione);
                                warning = true;
                                //return false;
                            }
                            else
                            {
                                sl.Log("");
                                sl.Log("UO - Con codice " + newUo.Codice + " inserita correttamente.");
                            }
                        }
                    }
                }
                sl.Log("");
                sl.Log("**** Fine importazione UO - " + System.DateTime.Now.ToString());
                #endregion Inserimento UO

                #region Inserimento Ruoli
                sl.Log("");
                sl.Log("**** Inizio importazione RUOLI - " + System.DateTime.Now.ToString());

                xlsCmd = new OleDbCommand("select * from [RUOLI$]", xlsConn);
                xlsReaderRuoli = xlsCmd.ExecuteReader();

                while (xlsReaderRuoli.Read())
                {
                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReaderRuoli, 0) == "/")
                        break;

                    //Controllo campi obbligatori
                    if (string.IsNullOrEmpty(get_string(xlsReaderRuoli, 0)) || string.IsNullOrEmpty(get_string(xlsReaderRuoli, 1)) || string.IsNullOrEmpty(get_string(xlsReaderRuoli, 2)) || string.IsNullOrEmpty(get_string(xlsReaderRuoli, 3)) || string.IsNullOrEmpty(get_string(xlsReaderRuoli, 4)))
                    {
                        sl.Log("");
                        sl.Log("ERROR : Campi obbligatori dei RUOLI non inseriti nel modello");
                        return false;
                    }
                    else
                    {
                        //Recupero l'idAmministrazione e lo imposto all'infoUtente 
                        //perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                        string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(get_string(xlsReaderRuoli, 0).ToUpper());
                        infoUtente.idAmministrazione = idAmm;

                        //Creo il nuovo ruolo da inserire
                        OrgRuolo newRuolo = new OrgRuolo();

                        //Valorizzo i campi
                        UnitaOrganizzativa uoPadre = (UnitaOrganizzativa)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReaderRuoli, 1).ToUpper(), infoUtente);
                        if (uoPadre != null)
                            newRuolo.IDUo = uoPadre.systemId;

                        ArrayList tipiRuolo = GetListTipiRuolo(idAmm);
                        foreach (OrgTipoRuolo tipoRuolo in tipiRuolo)
                        {
                            if (tipoRuolo.Codice.ToUpper() == get_string(xlsReaderRuoli, 2).ToUpper())
                                newRuolo.IDTipoRuolo = tipoRuolo.IDTipoRuolo;
                        }

                        newRuolo.Codice = get_string(xlsReaderRuoli, 3).ToUpper();
                        newRuolo.CodiceRubrica = get_string(xlsReaderRuoli, 3).ToUpper();
                        newRuolo.Descrizione = get_string(xlsReaderRuoli, 4);
                        newRuolo.IDAmministrazione = idAmm;
                        newRuolo.DiRiferimento = get_string(xlsReaderRuoli, 5);
                        newRuolo.Responsabile = get_string(xlsReaderRuoli, 6);
                        //Da aggiungere la proprietà segretario

                        //Inserisco il nuovo RUOLO
                        EsitoOperazione esito = AmmInsNuovoRuolo(infoUtente, newRuolo, false);
                        if (esito.Codice != 0)
                        {
                            sl.Log("");
                            sl.Log("ERROR - " + newRuolo.Codice + " - " + esito.Descrizione);
                            warning = true;
                            //return false;
                        }
                        else
                        {
                            sl.Log("");
                            sl.Log("RUOLO - Con codice " + newRuolo.Codice + " inserito correttamente.");
                        }
                    }
                }

                sl.Log("");
                sl.Log("**** Fine importazione RUOLI - " + System.DateTime.Now.ToString());
                #endregion Inserimento Ruoli

                #region Associazione Registri/Rf a Ruolo
                sl.Log("");
                sl.Log("**** Inizio associazione REGISTRI / RF a Ruolo - " + System.DateTime.Now.ToString());

                xlsCmd = new OleDbCommand("select * from [RUOLI$]", xlsConn);
                xlsReaderRuoli = xlsCmd.ExecuteReader();

                while (xlsReaderRuoli.Read())
                {
                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReaderRuoli, 0) == "/")
                        break;

                    //Controllo che siano presenti dei registri da associare
                    if (!string.IsNullOrEmpty(get_string(xlsReaderRuoli, 0)) || !string.IsNullOrEmpty(get_string(xlsReaderRuoli, 1)) || !string.IsNullOrEmpty(get_string(xlsReaderRuoli, 3)) || !string.IsNullOrEmpty(get_string(xlsReaderRuoli, 8)))
                    {
                        //Recupero l'idAmministrazione e lo imposto all'infoUtente 
                        //perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                        string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(get_string(xlsReaderRuoli, 0).ToUpper());
                        infoUtente.idAmministrazione = idAmm;

                        //Recupero l'idCorrGlobale della UO padre
                        UnitaOrganizzativa uoPadre = (UnitaOrganizzativa)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReaderRuoli, 1).ToUpper(), infoUtente);
                        string idUo = string.Empty;
                        if (uoPadre != null)
                            idUo = uoPadre.systemId;

                        //Recupero l'idCorrGlobale del ruolo
                        Ruolo ruolo = (Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReaderRuoli, 3).ToUpper(), infoUtente);
                        string idCorrGlobRuolo = string.Empty;
                        if (ruolo != null)
                            idCorrGlobRuolo = ruolo.systemId;

                        //Recupero la lista registri dell'amministrazione
                        ArrayList registriAmm = Amministrazione.RegistroManager.GetRegistri(get_string(xlsReaderRuoli, 0), "");

                        if (!string.IsNullOrEmpty(get_string(xlsReaderRuoli, 8)))
                        {
                            //Recupero la lista dei codici registro da associare
                            string[] codiciReg = get_string(xlsReaderRuoli, 8).Split(';');

                            //Creo la lista dei registri da associare
                            ArrayList registriDaAssociare = new ArrayList();

                            foreach (OrgRegistro registro in registriAmm)
                            {
                                registro.Associato = idCorrGlobRuolo;
                                for (int i = 0; i < codiciReg.Length; i++)
                                {
                                    if (registro.Codice.ToUpper() == codiciReg[i].ToString().ToUpper())
                                        registriDaAssociare.Add(registro);
                                }
                            }

                            //Associo i registri al ruolo
                            OrgRegistro[] listaRegistriDaAssociare = new OrgRegistro[registriDaAssociare.Count];
                            registriDaAssociare.CopyTo(listaRegistriDaAssociare);

                            EsitoOperazione esito = AmmInsRegistri(listaRegistriDaAssociare, idUo, idCorrGlobRuolo);
                            if (esito.Codice != 0)
                            {
                                sl.Log("");
                                sl.Log("ERROR : " + esito.Descrizione);
                                warning = true;
                                //return false;
                            }
                            else
                            {
                                sl.Log("");
                                sl.Log("Registri associati al ruolo con codice " + get_string(xlsReaderRuoli, 3).ToUpper());
                            }
                        }
                    }
                }

                sl.Log("");
                sl.Log("**** Fine associazione REGISTRI / RF a Ruolo - " + System.DateTime.Now.ToString());
                #endregion Associazione Registri/Rf a Ruolo

                #region Associazione Funzioni a Ruolo
                sl.Log("");
                sl.Log("**** Inizio associazione FUNZIONI a Ruolo - " + System.DateTime.Now.ToString());

                xlsCmd = new OleDbCommand("select * from [RUOLI$]", xlsConn);
                xlsReaderRuoli = xlsCmd.ExecuteReader();

                while (xlsReaderRuoli.Read())
                {
                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReaderRuoli, 0) == "/")
                        break;

                    //Controllo che siano presenti dei registri da associare
                    if (!string.IsNullOrEmpty(get_string(xlsReaderRuoli, 0)) || !string.IsNullOrEmpty(get_string(xlsReaderRuoli, 3)) || !string.IsNullOrEmpty(get_string(xlsReaderRuoli, 9)))
                    {
                        //Recupero l'idAmministrazione e lo imposto all'infoUtente 
                        //perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                        string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(get_string(xlsReaderRuoli, 0).ToUpper());
                        infoUtente.idAmministrazione = idAmm;

                        //Recupero l'idCorrGlobale del ruolo
                        Ruolo ruolo = (Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReaderRuoli, 3).ToUpper(), infoUtente);
                        string idCorrGlobRuolo = string.Empty;
                        if (ruolo != null)
                            idCorrGlobRuolo = ruolo.systemId;

                        //Recupero la lista delle funzioni dell'amministrazione
                        OrgTipoFunzione[] tipiFuzioneAmm = TipiFunzioneManager.GetTipiFunzione(true, idAmm);

                        if (!string.IsNullOrEmpty(get_string(xlsReaderRuoli, 9)))
                        {
                            //Recupero la lista dei codici funzione da associare
                            string[] codiciFuzioni = get_string(xlsReaderRuoli, 9).Split(';');

                            //Creo la lista delle funzioni da associare
                            ArrayList funzioniDaAssociare = new ArrayList();

                            foreach (OrgTipoFunzione funzione in tipiFuzioneAmm)
                            {
                                funzione.Associato = idCorrGlobRuolo;
                                for (int i = 0; i < codiciFuzioni.Length; i++)
                                {
                                    if (funzione.Codice.ToUpper() == codiciFuzioni[i].ToString().ToUpper())
                                        funzioniDaAssociare.Add(funzione);
                                }
                            }

                            //Associo le funzioni al ruolo
                            OrgTipoFunzione[] listaFunzioniDaAssociare = new OrgTipoFunzione[funzioniDaAssociare.Count];
                            funzioniDaAssociare.CopyTo(listaFunzioniDaAssociare);

                            EsitoOperazione esito = AmmInsTipoFunzioni(listaFunzioniDaAssociare);
                            if (esito.Codice != 0)
                            {
                                sl.Log("");
                                sl.Log("ERROR : " + esito.Descrizione);
                                warning = true;
                                //return false;
                            }
                            else
                            {
                                sl.Log("");
                                sl.Log("Funzioni associati al ruolo con codice " + get_string(xlsReaderRuoli, 3).ToUpper());
                            }
                        }
                    }
                }
                sl.Log("");
                sl.Log("**** Fine associazione FUNZIONI a Ruolo - " + System.DateTime.Now.ToString());
                #endregion Associazione Funzioni a Ruolo

                #region Inserimento Utenti
                sl.Log("");
                sl.Log("**** Inizio importazione UTENTI - " + System.DateTime.Now.ToString());

                xlsCmd = new OleDbCommand("select * from [UTENTI$]", xlsConn);
                xlsReaderUtenti = xlsCmd.ExecuteReader();

                while (xlsReaderUtenti.Read())
                {
                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReaderUtenti, 0) == "/")
                        break;

                    //Controllo campi obbligatori
                    if (string.IsNullOrEmpty(get_string(xlsReaderUtenti, 0)) || string.IsNullOrEmpty(get_string(xlsReaderUtenti, 1)) || string.IsNullOrEmpty(get_string(xlsReaderUtenti, 2)) || string.IsNullOrEmpty(get_string(xlsReaderUtenti, 3)))
                    {
                        sl.Log("");
                        sl.Log("ERROR : Campi obbligatori degli UTENTI non inseriti nel modello");
                        return false;
                    }
                    else
                    {
                        //Recupero l'idAmministrazione e lo imposto all'infoUtente 
                        //perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                        string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(get_string(xlsReaderUtenti, 1).ToUpper());
                        infoUtente.idAmministrazione = idAmm;

                        //Creo e valorirzzo l'utente da inserire
                        DocsPaVO.amministrazione.OrgUtente utente = new DocsPaVO.amministrazione.OrgUtente();
                        utente.UserId = get_string(xlsReaderUtenti, 0).ToUpper();
                        utente.CodiceRubrica = get_string(xlsReaderUtenti, 0).ToUpper();
                        utente.Codice = utente.CodiceRubrica;
                        utente.IDAmministrazione = idAmm;
                        utente.Nome = get_string(xlsReaderUtenti, 2);
                        utente.Cognome = get_string(xlsReaderUtenti, 3);
                        utente.Dominio = get_string(xlsReaderUtenti, 4);
                        utente.Password = get_string(xlsReaderUtenti, 5);
                        utente.Sede = get_string(xlsReaderUtenti, 7);
                        utente.Email = get_string(xlsReaderUtenti, 8);
                        utente.NotificaTrasm = get_string(xlsReaderUtenti, 9);
                        utente.Abilitato = get_string(xlsReaderUtenti, 10);
                        utente.Amministratore = get_string(xlsReaderUtenti, 11);

                        //Inserisco l'utente
                        EsitoOperazione esito = AmmInsNuovoUtente(infoUtente, utente);
                        if (esito.Codice != 0)
                        {
                            sl.Log("");
                            sl.Log("ERROR - " + utente.Codice + " - " + esito.Descrizione);
                            warning = true;
                            //return false;
                        }
                        else
                        {
                            sl.Log("");
                            sl.Log("UTENTE - Con codice " + utente.Codice + " inserito correttamente.");
                        }
                    }
                }

                sl.Log("");
                sl.Log("**** Fine importazione UTENTI - " + System.DateTime.Now.ToString());
                #endregion Inserimento Utenti

                #region Associazione Utenti a Ruolo
                sl.Log("");
                sl.Log("**** Inizio associazione UTENTI a ruolo - " + System.DateTime.Now.ToString());

                xlsCmd = new OleDbCommand("select * from [RUOLI$]", xlsConn);
                xlsReaderRuoli = xlsCmd.ExecuteReader();

                while (xlsReaderRuoli.Read())
                {
                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReaderRuoli, 0) == "/")
                        break;

                    //Controllo che siano presenti degli utenti da associare
                    if (!string.IsNullOrEmpty(get_string(xlsReaderRuoli, 0)) || !string.IsNullOrEmpty(get_string(xlsReaderRuoli, 3)) || !string.IsNullOrEmpty(get_string(xlsReaderRuoli, 10)))
                    {
                        //Recupero l'idAmministrazione e lo imposto all'infoUtente 
                        //perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                        string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(get_string(xlsReaderRuoli, 0).ToUpper());
                        infoUtente.idAmministrazione = idAmm;

                        //Recupero il ruolo a cui associare gli utenti
                        Ruolo ruolo = (Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReaderRuoli, 3).ToUpper(), infoUtente);
                        string idGruppo = string.Empty;
                        if (ruolo != null)
                            idGruppo = ruolo.idGruppo;

                        if (!string.IsNullOrEmpty(get_string(xlsReaderRuoli, 10)))
                        {
                            //Recupero i codici degli utenti da inserire
                            string[] codiciUtente = get_string(xlsReaderRuoli, 10).Split(';');

                            //Associo l'utente al ruolo
                            for (int i = 0; i < codiciUtente.Length; i++)
                            {
                                //Recupero l'utente da associare
                                Utente utente = (Utente)Utenti.UserManager.getCorrispondenteByCodRubrica(codiciUtente[i].ToUpper(), infoUtente);
                                string idPeople = string.Empty;
                                if (utente != null)
                                    idPeople = utente.idPeople;

                                EsitoOperazione esito = AmmInsUtenteInRuolo(infoUtente, idPeople, idGruppo);
                                if (esito.Codice != 0)
                                {
                                    sl.Log("");
                                    sl.Log("ERROR - " + codiciUtente[i] + " - " + esito.Descrizione);
                                    warning = true;
                                    //return false;
                                }
                                else
                                {
                                    sl.Log("");
                                    sl.Log("UTENTE - Con codice " + codiciUtente[i] + " associato al ruolo " + get_string(xlsReaderRuoli, 3));
                                }
                            }
                        }
                    }
                }

                sl.Log("");
                sl.Log("**** associazione UTENTI a ruolo - " + System.DateTime.Now.ToString());
                #endregion Associazione Utenti a Ruolo

                #region Associazione Ruolo Predefinito a Utente
                sl.Log("");
                sl.Log("**** Inizio impostazione RUOLO PREDEFINITO per utente - " + System.DateTime.Now.ToString());

                xlsCmd = new OleDbCommand("select * from [UTENTI$]", xlsConn);
                xlsReaderUtenti = xlsCmd.ExecuteReader();

                while (xlsReaderUtenti.Read())
                {
                    //Controllo se siamo arrivati all'ultima riga
                    if (get_string(xlsReaderUtenti, 0) == "/")
                        break;

                    //Controllo che sia presente un ruolo predefinito da associare
                    if (!string.IsNullOrEmpty(get_string(xlsReaderUtenti, 0)) || !string.IsNullOrEmpty(get_string(xlsReaderUtenti, 1)) || !string.IsNullOrEmpty(get_string(xlsReaderUtenti, 12)))
                    {
                        //Recupero l'idAmministrazione e lo imposto all'infoUtente 
                        //perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                        string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(get_string(xlsReaderUtenti, 1).ToUpper());
                        infoUtente.idAmministrazione = idAmm;

                        //Recupero l'utente
                        Utente utente = (Utente)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReaderUtenti, 0).ToUpper(), infoUtente);
                        string idPeople = string.Empty;
                        if (utente != null)
                            idPeople = utente.idPeople;

                        //Recupero il ruolo predefinito da associare all'utente
                        Ruolo ruolo = (Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(get_string(xlsReaderUtenti, 12).ToUpper(), infoUtente);
                        string idGruppo = string.Empty;
                        if (ruolo != null)
                            idGruppo = ruolo.idGruppo;

                        if (!string.IsNullOrEmpty(idPeople) && !string.IsNullOrEmpty(idGruppo))
                        {
                            EsitoOperazione esito = AmmImpostaRuoloPreferito(infoUtente, idPeople, idGruppo);
                            if (esito.Codice != 0)
                            {
                                sl.Log("");
                                sl.Log("ERROR - " + get_string(xlsReaderUtenti, 12) + " - " + esito.Descrizione);
                                warning = true;
                                //return false;
                            }
                            else
                            {
                                sl.Log("");
                                sl.Log("RUOLO - Con codice " + get_string(xlsReaderUtenti, 12) + " associato come predefinito all'utente " + get_string(xlsReaderUtenti, 0));
                            }
                        }
                    }
                }

                sl.Log("");
                sl.Log("**** Inizio impostazione RUOLO PREDEFINITO per utente - " + System.DateTime.Now.ToString());
                #endregion Associazione Ruolo Predefinito a Utente
            }
            catch (Exception ex)
            {
                sl.Log("");
                sl.Log("ERRROE : " + ex.Message);
                return false;
            }
            finally
            {
                if (xlsReaderUO != null)
                    xlsReaderUO.Close();
                if (xlsReaderRuoli != null)
                    xlsReaderRuoli.Close();
                if (xlsReaderUtenti != null)
                    xlsReaderUtenti.Close();
                if (xlsConn != null)
                    xlsConn.Close();
            }

            if (warning)
                return false;
            else
                return true;
        }

        public static System.Collections.ArrayList getLogImportOrganigramma(string serverPath)
        {
            System.Collections.ArrayList fileLog = new System.Collections.ArrayList();
            string sLine = string.Empty;

            try
            {
                StreamReader objReader = new StreamReader(serverPath + "\\Modelli\\Import\\logImportazioneOrganigramma.log");
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
                logger.Debug("Metodo \"getLogImportOrganigramma\" classe \"OrganigrammaManager\" ERRORE : " + e.Message);
                return fileLog;
            }
        }

        private static string get_string(OleDbDataReader dr, int field)
        {
            if (dr[field] == null || dr[field] == System.DBNull.Value)
                return "";
            else
                return dr[field].ToString();
        }

        public static EsitoOperazione CopyVisibility(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Security.CopyVisibility copyVisibility)
        {
            try
            {
                EsitoOperazione eo = new EsitoOperazione();
                DocsPaDocumentale.Documentale.OrganigrammaManager orgManager = new DocsPaDocumentale.Documentale.OrganigrammaManager(infoUtente);
                eo = orgManager.CopyVisibility(infoUtente, copyVisibility);

                if(eo.numDocCopiati == 0 && eo.numeroFascCopiati == 0 && eo.numeroDocinFascCopiati == 0)
                    UserLog.UserLog.WriteLog(infoUtente, "AMMCOPY", infoUtente.idPeople, "COPIA VISIBILITA' dal ruolo [ " + copyVisibility.codRuoloOrigine + " ] " + copyVisibility.descRuoloOrigine + " al ruolo  [ " + copyVisibility.codRuoloDestinazione + " ] " + copyVisibility.descRuoloDestinazione + " sono stati copiati " + eo.numDocCopiati + " documento(i), " + eo.numeroFascCopiati + " fascicolo(i), " + eo.numeroDocinFascCopiati + " documento(i) in fascicolo(i)/sottofascicolo(i). ", DocsPaVO.Logger.CodAzione.Esito.KO, copyVisibility.idAmm);
                else
                    UserLog.UserLog.WriteLog(infoUtente, "AMMCOPY", infoUtente.idPeople, "COPIA VISIBILITA' dal ruolo [ " + copyVisibility.codRuoloOrigine + " ] " + copyVisibility.descRuoloOrigine + " al ruolo  [ " + copyVisibility.codRuoloDestinazione + " ] " + copyVisibility.descRuoloDestinazione + " sono stati copiati " + eo.numDocCopiati + " documento(i), " + eo.numeroFascCopiati + " fascicolo(i), " + eo.numeroDocinFascCopiati + " documento(i) in fascicolo(i)/sottofascicolo(i). ", DocsPaVO.Logger.CodAzione.Esito.OK, copyVisibility.idAmm);

                if (eo.numDocCopiati != 0 || eo.numeroFascCopiati != 0 || eo.numeroDocinFascCopiati != 0)
                {
                    if (copyVisibility.estendiVisibilita.ToUpper().Equals("SI"))
                    {
                        string reportLogSuperiori = string.Empty;
                        if (eo.Descrizione.Contains("*"))
                        {
                            reportLogSuperiori += "COPIA VISIBILITA' dal ruolo [ " + copyVisibility.codRuoloOrigine + " ] al ruolo  [ " + copyVisibility.codRuoloDestinazione + " ] - Estensione visibilità documenti ai superiori - ";
                            string desc1 = eo.Descrizione.Split('*')[1];
                            // il risultato conterrà sicuramente due separatori "/" per dividere i seguenti casi:
                            // in ar_desc[0] ci saranno i risultati dell'estensione ai superiori della visibilità dei documenti
                            // in ar_desc[1] ci saranno i risultati dell'estensione ai superiori della visibilità dei fascicoli
                            // in ar_desc[2] ci saranno i risultati dell'estensione ai superiori della visibilità dei documenti in fascicoli
                            string[] ar_desc = desc1.Split('/');
                            // estensione visibilità documenti
                            if (ar_desc[0].Length > 0)
                            {
                                string repVisDoc = ar_desc[0].Substring(0, ar_desc[0].Length - 1);
                                if (repVisDoc.Contains('^'))
                                {
                                    string[] ar_repVisDoc = repVisDoc.Split('^');
                                    for (int i = 0; i < ar_repVisDoc.Length; i++)
                                        reportLogSuperiori += ar_repVisDoc[i] + " ";
                                }
                                UserLog.UserLog.WriteLog(infoUtente, "AMMCOPY", infoUtente.idPeople, reportLogSuperiori, DocsPaVO.Logger.CodAzione.Esito.OK, copyVisibility.idAmm);
                            }
                            reportLogSuperiori = "COPIA VISIBILITA' dal ruolo [ " + copyVisibility.codRuoloOrigine + " ] al ruolo  [ " + copyVisibility.codRuoloDestinazione + " ] - Estensione visibilità fascicoli ai superiori - ";
                            if (ar_desc[1].Length > 0)
                            {
                                string repVisFasc = ar_desc[1].Substring(0, ar_desc[1].Length - 1);
                                if (repVisFasc.Contains('^'))
                                {
                                    string[] ar_repVisFasc = repVisFasc.Split('^');
                                    for (int i = 0; i < ar_repVisFasc.Length; i++)
                                        reportLogSuperiori += ar_repVisFasc[i] + " ";
                                }
                                UserLog.UserLog.WriteLog(infoUtente, "AMMCOPY", infoUtente.idPeople, reportLogSuperiori, DocsPaVO.Logger.CodAzione.Esito.OK, copyVisibility.idAmm);
                            }
                            reportLogSuperiori = "COPIA VISIBILITA' dal ruolo [ " + copyVisibility.codRuoloOrigine + " ] al ruolo  [ " + copyVisibility.codRuoloDestinazione + " ] - Estensione visibilità documenti in fascicoli ai superiori - ";
                            if (ar_desc[2].Length > 0)
                            {
                                string repVisDocFasc = ar_desc[2].Substring(0, ar_desc[2].Length - 1);
                                if (repVisDocFasc.Contains('^'))
                                {
                                    string[] ar_repVisDocFasc = repVisDocFasc.Split('^');
                                    for (int i = 0; i < ar_repVisDocFasc.Length; i++)
                                        reportLogSuperiori += ar_repVisDocFasc[i] + " ";
                                }
                                UserLog.UserLog.WriteLog(infoUtente, "AMMCOPY", infoUtente.idPeople, reportLogSuperiori, DocsPaVO.Logger.CodAzione.Esito.OK, copyVisibility.idAmm);
                            }
                        }
                    }
                }
                return eo;

            }
            catch (Exception ex)
            {
                logger.Error("Metodo \"CopyVisibility\" classe \"OrganigrammaManager\" ERRORE : " + ex.Message);
                UserLog.UserLog.WriteLog(infoUtente, "AMMCOPY", infoUtente.idPeople, "COPIA VISIBILITA' dal ruolo [ " + copyVisibility.codRuoloOrigine + " ] " + copyVisibility.descRuoloOrigine + " al ruolo  [ " + copyVisibility.codRuoloDestinazione + " ] " + copyVisibility.descRuoloDestinazione, DocsPaVO.Logger.CodAzione.Esito.KO, copyVisibility.idAmm);
               
                return null;
            }
        }

        #region Spostamento ruolo

        /// <summary>
        /// Orchestratore per lo spostamento di un ruolo da una UO ad un'altra
        /// </summary>
        /// <param name="request">Dettagli sull'operazione da compiere</param>
        /// <returns>Dettagli sull'esito dell'operazione</returns>
        public static SaveChangesToRoleResponse SaveChangesToRole(SaveChangesToRoleRequest request)
        {
            SaveChangesToRoleResponse response = new SaveChangesToRoleResponse(request);

            switch (request.Phase)
            {
                case SaveChangesToRoleRequest.SaveChangesToRolePhase.Start:
                    // Fase fittizia di preinizializzazione (viene utilizzata per inizializzare il report)
                    response = new SaveChangesToRoleResponse();
                    response.Result = new EsitoOperazione() { Codice = 0 };
                    response.NextPhase = SaveChangesToRoleRequest.SaveChangesToRolePhase.Initialize;

                    break;
                case SaveChangesToRoleRequest.SaveChangesToRolePhase.Initialize:
                    // Inizializzazione della procedura
                    try
                    {
                        response = CheckChanges(request);

                        if (response.Result.Codice == 0)
                            response = InitializeMoveRole(request);
                    }
                    catch (Exception e)
                    {
                        response.Result = GetErrorDescription(request.Phase, e);
                    }

                    if (request.Historicize)
                        response.NextPhase = SaveChangesToRoleRequest.SaveChangesToRolePhase.CleanToDoList;
                    else
                        response.NextPhase = SaveChangesToRoleRequest.SaveChangesToRolePhase.SaveChanges;
                    break;

                case SaveChangesToRoleRequest.SaveChangesToRolePhase.CleanToDoList:
                    // Pulizia to do list (vengono rimosse solo le trasmissioni a ruolo). Viene impostata una nota
                    // con testo "Trasmissione accettata automaticamente in seguito a modifica del ruolo". Viene inoltre
                    // impostata la data visto per le trasmissioni che non prevedono accettazione.
                    // Le trasmissioni di tipo uno, verranno accettate dal primo utente diponibile,  quelle di tipo
                    // tutti verranno accettate da tutti gli utenti attivi
                    try
                    {
                        response = CleanToDoList(request);
                    }
                    catch (Exception e)
                    {
                        response.Result = GetErrorDescription(request.Phase, e);
                    }

                    response.NextPhase = SaveChangesToRoleRequest.SaveChangesToRolePhase.SaveChanges;
                    break;
                case SaveChangesToRoleRequest.SaveChangesToRolePhase.SaveChanges:
                    // Salvataggio modifiche
                    try
                    {
                        response = ExecuteSaveChangesToRole(request);

                        // Se non è richiesto di aggiornare i modelli di trasmissione, vengono cancellati
                        // quelli unicamente visibili al ruolo spostato / modificato
                        if (!request.UpdateTransModelsAssociation && response.Result.Codice == 0)
                        {
                            request.IdCorrGlobOldRole = response.IdCorrGlobOldRole;
                            request.ModifiedRole = response.ModifiedRole;
                            response.Result = RemoveRolesTransmissionModel(request);

                        }

                        // Se non è richiesta l'estensione della visibilità,
                        // viene calcolata 
                    }
                    catch (Exception e)
                    {
                        response.Result = GetErrorDescription(request.Phase, e);
                    }

                    if (request.ExtendVisibility == SaveChangesToRoleRequest.ExtendVisibilityOption.N)
                        // Se è attivo il calcolo dell'atipicità e si è compiuto uno spostamento o una modifica del tipo ruolo, 
                        // viene eseguito altrimenti vengono aggiornate le liste
                        if(DocsPaUtils.Configuration.InitConfigurationKeys.getInstance("0").ContainsKey("ATIPICITA_DOC_FASC") &&
                            DocsPaUtils.Configuration.InitConfigurationKeys.getInstance("0")["ATIPICITA_DOC_FASC"].ToString() == "1" &&
                            (request.IdOldRoleType != request.ModifiedRole.IDTipoRuolo|| (request.IdOldUO != request.ModifiedRole.IDUo && request.ComputeAtipicita)))
                            response.NextPhase = SaveChangesToRoleRequest.SaveChangesToRolePhase.CalculateAtipicita;
                        else
                            response.NextPhase = SaveChangesToRoleRequest.SaveChangesToRolePhase.UpdateLists;
                    else
                        response.NextPhase = SaveChangesToRoleRequest.SaveChangesToRolePhase.ExtendVisibility;

                    break;
                case SaveChangesToRoleRequest.SaveChangesToRolePhase.ExtendVisibility:
                    // Estensione della visibilità ai superiori gerarchici
                    try
                    {
                        response = ExtendVisibilityToHigherRoles(request);
                    }
                    catch (Exception e)
                    {
                        response.Result = GetErrorDescription(request.Phase, e);
                    }

                    // Se è attivo il calcolo di atipicità, viene eseguito altrimenti si passa all'aggiornamento
                    // delle liste
                    if (DocsPaUtils.Configuration.InitConfigurationKeys.getInstance("0").ContainsKey("ATIPICITA_DOC_FASC") &&
                        DocsPaUtils.Configuration.InitConfigurationKeys.getInstance("0")["ATIPICITA_DOC_FASC"].ToString() == "1" &&
                        (request.IdOldRoleType != request.ModifiedRole.IDTipoRuolo || (request.IdOldUO != request.ModifiedRole.IDUo && request.ComputeAtipicita)))
                        response.NextPhase = SaveChangesToRoleRequest.SaveChangesToRolePhase.CalculateAtipicita;
                    else
                        response.NextPhase = SaveChangesToRoleRequest.SaveChangesToRolePhase.UpdateLists;
                    break;
                case SaveChangesToRoleRequest.SaveChangesToRolePhase.CalculateAtipicita:
                    // Calcolo dell'atipicità
                    try
                    {
                        response = CalculateAtipicita(request);
                    }
                    catch (Exception e)
                    {
                        response.Result = GetErrorDescription(request.Phase, e);
                    }

                    response.NextPhase = SaveChangesToRoleRequest.SaveChangesToRolePhase.UpdateLists;
                    break;
                case SaveChangesToRoleRequest.SaveChangesToRolePhase.UpdateLists:
                    // Aggiornamento del proprietario delle liste di distribuzione
                    try
                    {
                        response = UpdateLists(request);

                    }
                    catch (Exception e)
                    {
                        response.Result = GetErrorDescription(request.Phase, e);
                    }

                    if (!request.UpdateTransModelsAssociation)
                        response.NextPhase = SaveChangesToRoleRequest.SaveChangesToRolePhase.UpdateTransmissionModelsAssociation;
                    else
                        response.NextPhase = SaveChangesToRoleRequest.SaveChangesToRolePhase.Finish;
                    break;
                case SaveChangesToRoleRequest.SaveChangesToRolePhase.UpdateTransmissionModelsAssociation:
                    // Riassociazione proprietà dei modelli di trasmissione
                    try
                    {
                        response = UpdateTransmissionModelsAssociation(request);
                    }
                    catch (Exception e)
                    {
                        response.Result = GetErrorDescription(request.Phase, e);
                    }

                    response.NextPhase = SaveChangesToRoleRequest.SaveChangesToRolePhase.Finish;
                    break;
            }

            return response;
        }

        /// <summary>
        /// Metodo per la creazione dell'esito di una operazione in caso di errore
        /// </summary>
        /// <param name="saveChangesPhase">Fase dell'operazione che ha generato eccezione</param>
        /// <param name="e">Eccezione sollevata dal metodo</param>
        /// <returns>Esito dell'operazione</returns>
        private static EsitoOperazione GetErrorDescription(SaveChangesToRoleRequest.SaveChangesToRolePhase saveChangesPhase, Exception e)
        {
            logger.DebugFormat("Errore durante l'esecuzione della fase {0} del metodo SaveChangesToRole", saveChangesPhase.ToString());
            logger.Debug("Eccezione sollevata", e);

            return new EsitoOperazione()
            {
                Codice = -1
            };

        }

        /// <summary>
        /// Metodo per la verifica delle modifiche effettuate al ruolo che si intente modificare
        /// </summary>
        /// <param name="request">Oggetto con le informazioni sul ruolo da modificare</param>
        /// <returns>Response con l'esito delle verifiche</returns>
        private static SaveChangesToRoleResponse CheckChanges(SaveChangesToRoleRequest request)
        {
            // Inizializzazione della response
            SaveChangesToRoleResponse response = new SaveChangesToRoleResponse(request);

            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();

            if (!amm.CheckIfRoleModified(request.ModifiedRole))
                response.Result = new EsitoOperazione() { Codice = -1, Descrizione = "Non è stata rilevata alcuna modifica da salvare" };

            // Controllo di unicità del nuovo codice rubrica
            bool storicizzato = false;
            String roleDesc = "";
            if(response.Result.Codice != -1 && amm.CheckCodiceRuoloDuplicato(request.ModifiedRole.IDCorrGlobale, request.ModifiedRole.Codice, request.ModifiedRole.IDAmministrazione, out storicizzato, out roleDesc))
                if(storicizzato)
                    response.Result = new EsitoOperazione() { Codice = -1, Descrizione = String.Format("Attenzione il codice è già utilizzato dal ruolo storicizzato \"{0}\"", roleDesc) };
                else
                    response.Result = new EsitoOperazione() { Codice = -1, Descrizione = "Attenzione esiste già un ruolo con lo stesso codice" };

            return response;
        }

        /// <summary>
        /// Inizializzazione dell'operazione di spostamento ruolo
        /// </summary>
        /// <param name="request">Request relativa all'azione di spostamento ruolo</param>
        /// <returns>Risultato dell'azione di inizializzazione</returns>
        private static SaveChangesToRoleResponse InitializeMoveRole(SaveChangesToRoleRequest request)
        {
            SaveChangesToRoleResponse response = new SaveChangesToRoleResponse(request);

            // Esito di successo per default
            response.Result = new EsitoOperazione()
            {
                Codice = 0,
                Descrizione = ""
            };

            // Recupero degli utenti associati al ruolo e salvataggio dell'id corr globali e id gruppo del
            // ruolo su cui operare
            response.Users = (OrgUtente[])GetListUtentiRuolo(request.ModifiedRole.IDGruppo).ToArray(typeof(OrgUtente));
            response.ModifiedRole = request.ModifiedRole;

            // Recupero dell'id tipo ruolo e dell'id della UO di appartenenza del ruolo
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            response.IdOldUO = utenti.GetUoIdFromRoleId(request.ModifiedRole.IDCorrGlobale);
            response.IdOldRoleType = utenti.GetRoleTypeId(request.ModifiedRole.IDGruppo);

            // Per poter proseguire, non ci devono essere utenti con delega attiva. Viene inoltre verificata la presenza
            // di to do list con trasmissioni a ruolo
            foreach (var user in response.Users)
            {
                DocsPaDB.Query_DocsPAWS.Deleghe delegheManager = new DocsPaDB.Query_DocsPAWS.Deleghe();
                bool haveDeleghe = delegheManager.CheckIfUserHaveDelegheInEsercizio(user.IDPeople, response.ModifiedRole.IDGruppo);

                // Se ci sono deleghe attive, viene forzata l'uscita dal ciclo e viene restituito un messaggio di errore
                if (haveDeleghe)
                {
                    response.Result.Codice = -1;
                    response.Result.Descrizione = "Almeno un utente nel ruolo ha delle deleghe attive. E' necessario revocare le deleghe per poter proseguire.";

                }

            }

            // Check sulla presenza di trasmissioni a ruolo pendenti per il ruolo da spostare solo se è richiesta storicizzazione
            if (request.Historicize)
            {
                bool haveTransm = Trasmissioni.TrasmManager.CheckIfUserHaveRoleTransmission(response.ModifiedRole.IDCorrGlobale);
                if (haveTransm)
                    response.MessageToShowToAdministrator = "Attenzione! Sono state rilevate delle trasmissioni pendenti indirizzate al ruolo da modificare. Cliccando su OK, queste trasmissioni verranno accettate massivamente. Cliccare su Annulla se si desidera pulire le liste delle cose da fare manualmente.";
            }

            return response;
        }

        /// <summary>
        /// Metodo per la pulizia della to do list
        /// </summary>
        /// <param name="request">Request con le informazioni sugli utenti di cui pulire la to do list</param>
        /// <returns>Response aggiornata</returns>
        private static SaveChangesToRoleResponse CleanToDoList(SaveChangesToRoleRequest request)
        {
            // Costruzione della response a partire dalla request
            SaveChangesToRoleResponse response = new SaveChangesToRoleResponse(request);

            // Recupero del dettaglio del ruolo
            Ruolo role = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(request.ModifiedRole.IDGruppo);

            // Esito dell'operazione
            EsitoOperazione result = new EsitoOperazione();

            // Pulizia della to do list di tutti gli utenti del ruolo
            foreach (var user in request.Users)
            {
                // Recupero del dettaglio dell'utente
                Utente u = BusinessLogic.Utenti.UserManager.getUtenteById(user.IDPeople);

                // Pulizia della todolist dell'utente
                result = BusinessLogic.Trasmissioni.TrasmManager.AccettazioneMassivaInTdl(
                    u,
                    role,
                    "Trasmissione accettata automaticamente in seguito a modifica del ruolo");

                // Impostazione del codice di ritorno e aggiornamento della descrizione
                if (response.Result.Codice == 0)
                    response.Result.Codice = result.Codice;

                if (!String.IsNullOrEmpty(result.Descrizione))
                    response.Result.Descrizione += "\n" + result.Descrizione;

            }

            // Restituzione della response
            return response;

        }

        /// <summary>
        /// Esecuzione salvatggio modifiche al ruolo
        /// </summary>
        /// <param name="request">Oggetto con le informazioni sul ruolo da modificare</param>
        /// <returns></returns>
        private static SaveChangesToRoleResponse ExecuteSaveChangesToRole(SaveChangesToRoleRequest request)
        {
            SaveChangesToRoleResponse response = new SaveChangesToRoleResponse(request);

            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext(IsolationLevel.ReadCommitted))
            {
                // Se è stato modificato il codice della UO di appartenenza, bisogna effettuare uno
                // spostamento altrimenti nisogna effettuare delle modifiche
                using (DocsPaDB.Query_DocsPAWS.Utenti users = new DocsPaDB.Query_DocsPAWS.Utenti())
                {
                    OrgRuolo newRole = null;

                    // Preventiva storicizzazione  se richiesta
                    if (request.Historicize)
                        newRole = AmmStoricizzaRuolo(request.UserInfo, request.ModifiedRole);

                    if (!request.Historicize || (request.Historicize && newRole != null))
                    {
                        response.IdCorrGlobOldRole = request.ModifiedRole.IDCorrGlobale;
                        response.IdOldRole = request.ModifiedRole.IDGruppo;

                        if(newRole != null)
                            response.ModifiedRole = newRole;

                        if (request.IdOldUO != request.ModifiedRole.IDUo)
                            response.Result = AmmSpostaRuolo(request.UserInfo, response.ModifiedRole);
                        else
                            response.Result = AmmModRuolo(request.UserInfo, response.ModifiedRole);
                    }
                    else
                        response.Result = new EsitoOperazione() { Codice = -1, Descrizione = "Errore durante la storicizzazione del ruolo" };
                }

                if (response.Result.Codice == 0)
                    transactionContext.Complete();

            }

            return response;
        }

        /// <summary>
        /// Metodo per la rimozione dei modelli di trasmissione visibili unicamente al ruolo
        /// </summary>
        /// <param name="request">Informazioni sul ruolo in fase di modifica</param>
        /// <returns>Esito dell'operazione di modifica</returns>
        public static EsitoOperazione RemoveRolesTransmissionModel(SaveChangesToRoleRequest request)
        {
            EsitoOperazione retVal = new EsitoOperazione();

            using (ModTrasmissioni modTrasmissioni = new ModTrasmissioni())
            {
                bool removed = modTrasmissioni.RemoveTransmissionModelsVisibleOnlyToRole(request.ModifiedRole.IDCorrGlobale);

                retVal.Codice = removed ? 0 : -1;
                retVal.Descrizione = removed ? String.Empty : "Errore durante l'eliminazione dei modelli di trasmissione visibili unicamente al ruolo.";
            }

            return retVal;
        }

        /// <summary>
        /// Metodo per il calcolo dell'atipicità
        /// </summary>
        /// <param name="request">Informazioni sull'operazione in corso</param>
        /// <returns>Response relativa alla fase di calcolo atipicità</returns>
        public static SaveChangesToRoleResponse CalculateAtipicita(SaveChangesToRoleRequest request)
        { 
            // Creazione della response a partire dalla request
            SaveChangesToRoleResponse response = new SaveChangesToRoleResponse(request);

            // Esecuzione del calcolo dell'atipicità
            DocsPaDocumentale.Documentale.OrganigrammaManager manager = new DocsPaDocumentale.Documentale.OrganigrammaManager(request.UserInfo);
            response.Result = manager.CalcolaAtipicita(request.ModifiedRole, request.IdOldRoleType, request.IdOldUO, request.ComputeAtipicita);

            return response;
        }

        /// <summary>
        /// Metodo per l'estensione della visibilità ai superiori gerarchici
        /// </summary>
        /// <param name="request">Informazioni sull'operazione in corso</param>
        /// <returns>Response relativa alla fase di estensione della visibilità ai superiori gerarchici</returns>
        public static SaveChangesToRoleResponse ExtendVisibilityToHigherRoles(SaveChangesToRoleRequest request)
        { 
            // Creazione della response a partire dalla request
            SaveChangesToRoleResponse response = new SaveChangesToRoleResponse(request);

            DocsPaDocumentale.Documentale.OrganigrammaManager manager = new DocsPaDocumentale.Documentale.OrganigrammaManager(request.UserInfo);
            response.Result = manager.ExtendVisibilityToHigherRoles(
                request.ModifiedRole.IDAmministrazione,
                request.ModifiedRole.IDGruppo,
                request.ExtendVisibility);

            
            return response;
        }

        /// <summary>
        /// Metodo per l'aggiornamento del proprietario delle liste di distribuzione
        /// </summary>
        /// <param name="request">Informazioni sul ruolo in fase di modifica / spostamento</param>
        /// <returns>Risultato dell'applicazione della pase di aggiornamento liste</returns>
        public static SaveChangesToRoleResponse UpdateLists(SaveChangesToRoleRequest request)
        {
            // Creazione della response
            SaveChangesToRoleResponse response = new SaveChangesToRoleResponse(request);

            ListeDistr lists = new ListeDistr();
            bool result = lists.UpdateListsOwner(request.ModifiedRole.IDGruppo, request.IdOldRole);

            response.Result.Codice = result ? 0 : -1;
            response.Result.Descrizione = result ? String.Empty : "Errore durante l'aggiornamento del ruolo proprietario delle liste di distribuzione";

            return response;
 
        }

        /// <summary>
        /// Aggiornamento dell'associazione modello di trasmissione - ruolo.
        /// </summary>
        /// <param name="request">Request con le informazioni sul ruolo da spostare</param>
        /// <returns>Response con l'esito dell'operazione corrente aggiornato</returns>
        private static SaveChangesToRoleResponse UpdateTransmissionModelsAssociation(SaveChangesToRoleRequest request)
        {
            // Creazione della response
            SaveChangesToRoleResponse response = new SaveChangesToRoleResponse(request);

            DocsPaDB.Query_DocsPAWS.ModTrasmissioni mt = new ModTrasmissioni();

            bool elaborated = true;

            elaborated = mt.UpdateTransmissionModelsAssociation(request.IdCorrGlobOldRole, request.ModifiedRole.IDCorrGlobale);

            // Creazione del risultato
            response.Result.Codice = elaborated ? 0 : -1;
            response.Result.Descrizione = elaborated ? String.Empty : "Errore durante l'aggiornamento dell'associazione fra modelli di trasmissione e ruolo";

            return response;
        }

        #endregion

        public static OrgRuolo GetRole(string idCorrGlobRuolo)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            return amm.GetRole(idCorrGlobRuolo);

        }

        // CH Metodo per verificare esistenza della UO che si vuole modificare
        public static bool ammCheckCodiceUODuplicato(string id, string codice, string idAmm)
        {
            DocsPaDB.Query_DocsPAWS.Amministrazione amm = new DocsPaDB.Query_DocsPAWS.Amministrazione();
            bool result = false;
            bool storicizzato = false;
            String uoDesc = "";
            if (amm.CheckCodiceRuoloDuplicato(id, codice, idAmm, out storicizzato, out uoDesc))
                result = true;
            return result;
        }
        
    }
}
