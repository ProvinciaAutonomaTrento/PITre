using System;
using System.Reflection;
using System.Collections;
using System.Data;
using DocsPaUtils.Data;
using log4net;
using System.Text.RegularExpressions;
using DocsPaUtils;
using System.Collections.Generic;

namespace DocsPaDB.Query_DocsPAWS
{
    /// <summary>
    /// Summary description for Rubrica.
    /// </summary>
    public class Rubrica : DBProvider
    {
        private ILog logger = LogManager.GetLogger(typeof(Rubrica));
        private DocsPaVO.utente.InfoUtente _user;

        public Rubrica(DocsPaVO.utente.InfoUtente u)
        {
            _user = u;
        }

        static Rubrica()
        {
            //			Rubrica r = new Rubrica(null);
            //			Type t = r.GetType().BaseType;
            //			MethodInfo mi;
            //			if ((mi = t.GetMethod ("CheckStoredProcedures")) != null) 
            //			{
            //				bool created = (bool) mi.Invoke (r, new object[] { "rubrica" });
            //				logger.Debug (created ? "Create SP per Rubrica" : "SP per Rubrica già esistenti");
            //			}
        }

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

        private string ricercaFullText(string campo, string andStr, string ricerca)
        {
            string nome_campo = string.Empty;
            switch (campo)
            {
                case "DESCRIZIONE":
                    nome_campo = "VAR_DESC_CORR";
                    break;

                case "CITTA":
                    nome_campo = "var_citta";
                    break;

                case "LOCALITA":
                    nome_campo = "var_localita";
                    break;

                case "EMAIL":
                    nome_campo = "VAR_EMAIL";
                    break;
            }

            if (Cfg_USE_TEXT_INDEX == "0")
            {
                Regex regex = new Regex("&&");
                string[] lista = regex.Split(ricerca);
                if (lista[0].ToUpper().Contains("'"))
                    andStr += "UPPER(" + nome_campo + ") LIKE '%" + lista[0].ToUpper().Replace("'", "''") + "%'";
                else
                    andStr += "UPPER(" + nome_campo + ") LIKE '%" + lista[0].ToUpper() + "%'";
                for (int i = 1; i < lista.Length; i++)
                {
                    if (lista[i].ToUpper().Contains("'"))
                        andStr += " AND UPPER(" + nome_campo + ") LIKE '%" + lista[i].ToUpper().Replace("'", "''") + "%'";
                    else
                        andStr += " AND UPPER(" + nome_campo + ") LIKE '%" + lista[i].ToUpper() + "%'";
                }
            }
            else if (Cfg_USE_TEXT_INDEX == "1")
            {
                // da rivedere il caso di utilizzo della chiave USE_TEXT_INDEX = 1
                // il codice sotto dovrebbe essere giusto, ma nel caso si decidesse di utilizzare questa modalità
                // di ricerca di oracle, occorre sicuramente una verifica.

                //f.valore.Replace("&&", " ");
                //andStr += "a.SYSTEM_id in ( \n " +
                //                        " select system_id from table(fulltext_onvar_prof_oggetto ( \n " +
                //                        "'" + f.valore.ToUpper().Replace("'", "''") + "', \n" +
                //                        dtaCreazSucc + ", \n" +
                //                        dtaCreazPreced + ", \n" +
                //                        dtaProtoSucc + ", \n" +
                //                        dtaProtoPreced + " \n" + "))) \n";
            }
            else if (Cfg_USE_TEXT_INDEX == "2")
            {
                string value = DocsPaUtils.Functions.Functions.ReplaceApexes(ricerca).ToUpper();
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
                andStr += DocsPaDbManagement.Functions.Functions.GetContainsTextQuery(nome_campo, value);
                if (casoA)
                    andStr += " and upper(" + nome_campo + ") like upper('%" + valueA + "%')";
            }

            return andStr;
        }

        public ArrayList GetElementiRubrica(DocsPaVO.rubrica.ParametriRicercaRubrica qc)
        {
            string qry = "";
            ArrayList elementi = new ArrayList();
            DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
            bool no_filtro_aoo = obj.isFiltroAooEnabled();
            string userDb = string.Empty;
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
            if (dbType.ToUpper() == "SQL")
                userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession() + ".";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RICERCA_ELEMENTI_RUBRICA");
            DataSet ds = new DataSet();
            try
            {
                ArrayList sp_params = new ArrayList();
                if (qc.parent != null && qc.parent != "")
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("id_amm", Convert.ToInt32(_user.idAmministrazione), 0, DirectionParameter.ParamInput, System.Data.DbType.Decimal));

                    switch (qc.tipoIE)
                    {
                        case DocsPaVO.addressbook.TipoUtente.INTERNO:
                        default:
                            sp_params.Add(new DocsPaUtils.Data.ParameterSP("cha_tipo_ie", "I", 1, DirectionParameter.ParamInput, System.Data.DbType.String));
                            break;

                        case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                            sp_params.Add(new DocsPaUtils.Data.ParameterSP("cha_tipo_ie", "E", 1, DirectionParameter.ParamInput, System.Data.DbType.String));
                            break;
                    }
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("var_cod_rubrica", qc.parent, 128, DirectionParameter.ParamInput, System.Data.DbType.String));

                    int corr_types = 0;
                    corr_types += (qc.doUo ? 1 : 0);
                    corr_types += (qc.doRuoli ? 2 : 0);
                    corr_types += (qc.doUtenti ? 4 : 0);
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("corr_types", corr_types, 0, DirectionParameter.ParamInput, System.Data.DbType.Decimal));

                    if (this.ExecuteStoredProcedure("dpa3_get_children", sp_params, ds) != 1)
                        throw new Exception(this.LastExceptionMessage);
                }
                else
                {
                    // Non sono estratti i corrispondenti con tipologia "C", 
                    // ovvero gli elementi inseriti da rubrica comune

                    qry += DocsPaDbManagement.Functions.Functions.getNVL("a.cha_tipo_corr", "'X'") + " != 'C' AND ( (a.id_amm=" + _user.idAmministrazione + ") OR a.id_amm is null) AND ";
                    //if (qc.caller.filtroRegistroPerRicerca != null && qc.caller.filtroRegistroPerRicerca != "") {
                    //    qry += "id_registro in (" + qc.caller.filtroRegistroPerRicerca + ") AND ";
                    //}
                    switch (qc.tipoIE)
                    {
                        case DocsPaVO.addressbook.TipoUtente.INTERNO:
                            if (qc.doListe || qc.doRF)
                                qry += "(a.cha_tipo_ie='I' OR a.CHA_TIPO_IE IS NULL) AND ";
                            else
                                qry += "(a.cha_tipo_ie='I') AND ";
                            break;

                        case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                            if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE)
                                || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ESTERNI_AMM)
                                //modifica del 13/05/2009
                                || qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO
                                //fine modifica del 13/05/2009
                                )
                            {
                                qry += "(a.cha_tipo_ie='E') AND ";
                            }
                            break;

                        case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                            break;
                    }

                    if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                        qry += String.Format("(upper(a.var_cod_rubrica) = upper('{0}')) AND ", qc.codice.Replace("'", "''"));
                    else if (qc.codice != null && qc.codice != "")
                        qry += String.Format("(upper(a.var_cod_rubrica) LIKE upper('%{0}%')) AND ", qc.codice.Replace("'", "''"));

                    if (qc.descrizione != null && qc.descrizione != "")
                        qry += String.Format("(upper(a.var_desc_corr) LIKE upper('%{0}%')) AND ", qc.descrizione.Replace("'", "''"));
                    //qry = ricercaFullText("DESCRIZIONE", qry, qc.descrizione) + " AND ";

                    if (qc.descrizioneIndicizzata != null && qc.descrizioneIndicizzata != "")
                    {
                        if (dbType.ToUpper() == "SQL")
                            qry += String.Format("Contains(a.var_desc_corr,'%{0}%') AND ", qc.descrizioneIndicizzata.Replace("'", "''"));
                        else
                            qry += String.Format("contains(a.var_desc_corr,'%{0}%')>0 AND ", qc.descrizioneIndicizzata.Replace("'", "''"));

                    }

                    if (qc.citta != null && qc.citta != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_citta) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.citta.Replace("'", "''"));
                    }

                    if (qc.localita != null && qc.localita != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_localita) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.localita.Replace("'", "''"));
                    }

                    //if (qc.cf_piva != null && qc.cf_piva != "")
                    //{
                    //    qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_cod_fiscale) = upper('{0}')) and id_corr_globali = a.system_id)) AND ", qc.cf_piva.Replace("'", "''"));
                    //}

                    if (!string.IsNullOrEmpty(qc.email))
                    {
                        qry += String.Format("(upper(a.VAR_EMAIL) LIKE upper('%{0}%')) AND ", qc.email.Replace("'", "''"));
                    }

                    if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_cod_fisc) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.codiceFiscale.Replace("'", "''"));
                    }

                    if (qc.partitaIva != null && qc.partitaIva != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_cod_pi) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.partitaIva.Replace("'", "''"));
                    }

                    //filtro per system_ID
                    if (qc.systemId != null && qc.systemId != "")
                    {
                        qry += String.Format("a.system_id = " + qc.systemId + " AND ");
                    }

                    //                    if (qc.doUo || qc.doRuoli || qc.doUtenti || qc.doListe || qc.doRF)
                    if (qc.doUo || qc.doRuoli || qc.doUtenti || qc.doRF)
                    {
                        qry += "a.cha_tipo_urp in (";
                        if (qc.doUo)
                            qry += "'U',";

                        if (qc.doRuoli)
                            qry += "'R',";

                        if (qc.doUtenti)
                            qry += "'P',";

                        //if (qc.doListe)
                        //    qry += "'L',";

                        if (qc.doRF)
                            qry += "'F',";

                        if (qry.EndsWith(","))
                            qry = qry.Substring(0, qry.Length - 1) + ")";
                    }
                    else
                    {
                        if (!qc.doListe)
                            return elementi;
                    }

                    if (qry.EndsWith(" AND "))
                        qry = qry.Substring(0, qry.Length - " AND ".Length);


                    //La condizione sui ruoli disabilitati (DTA_FINE valorizzata) non viene considerata per calltype di ricerca
                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CREATOR ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_OWNER_AUTHOR ||
                        //IACOZZILLI GIORDANO 25/06/2013
                        //Aggiungo il calltype per il deposito:
                         qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEP_OSITO ||
                        //FINE
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_TODOLIST ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_NO_FILTRI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORRISPONDENTE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI  ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                        )
                    {
                        //qry += " AND a.dta_fine IS NULL";
                    }
                    else
                    {
                        qry += " AND a.dta_fine IS NULL";
                    }

                    if (!qc.extSystems && qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MODELLO_TRASM)
                    {
                        qry += " AND a.cha_system_role != '1'";
                    }

                    if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORRISPONDENTE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO)
                        && qc.caller != null && !string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca))
                    {
                        qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") or a.id_registro is null)";
                    }

                    #region codice commentato
                    // #### NUOVO GESTIONE PER PITRE ####
                    // RICERCA DEI I CORRISPONDENTI GLOBALI DA DOC PROTOCOLLO
                    //if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE
                    //    &&
                    //    (qc.caller.IdRegistro != null || qc.caller.IdRegistro != string.Empty)
                    //    &&
                    //    (qc.doUo || qc.doRuoli || qc.doUtenti)
                    //    )
                    //{
                    //   qry += " AND (id_registro is null ";

                    //   ////dato un registro, ricerco anche tra gli rf ad esso associati che il ruolo può vedere
                    //    Utenti ut = new Utenti();
                    //    ArrayList listaRf = ut.GetListaRegistriRfRuolo(qc.caller.IdRuolo, "", "");

                    //    if (listaRf != null && listaRf.Count > 0)
                    //   {
                    //       foreach (DocsPaVO.utente.Registro reg in listaRf)
                    //        {
                    //            qry += " or id_registro = " + reg.systemId;
                    //        }
                    //    }
                    //    qry += " )";
                    //}

                    ////CASO in cui si è selezionata la voce Esterni AOO.
                    ////in tal caso ricerco solamente tra 
                    //if (
                    //    (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN
                    //    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
                    //    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                    //    &&
                    //      (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO)
                    //    &&
                    //    (qc.caller.IdRegistro != null || qc.caller.IdRegistro != string.Empty)
                    //    &&
                    //    (qc.doUo || qc.doRuoli || qc.doUtenti)
                    //    )
                    //{
                    //    qry += " AND CHA_TIPO_IE = 'I'";
                    //}
                    #endregion

                    if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INGRESSO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_ESTERNI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                        )
                        &&
                        (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE || qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO)
                        &&
                        (qc.caller.IdRegistro != null || qc.caller.IdRegistro != string.Empty)
                        &&
                        (qc.doUo || qc.doRuoli || qc.doUtenti)
                        )
                    {
                        //se è un solo registro verifico se è un RF
                        bool cha_rf = false;
                        if (qc.caller.filtroRegistroPerRicerca.IndexOf(",") == -1)
                        {
                            DocsPaVO.utente.Registro reg = GetRegistro(qc.caller.filtroRegistroPerRicerca);
                            if (reg != null && reg.chaRF == "1")
                                cha_rf = true;
                        }
                        #region codice commentato
                        //qry += " AND (id_registro IN (" + qc.caller.IdRegistro + ") or id_registro is null)";
                        //MODIFICA BUG: non carica valori in rubrica per le liste di distribuzione --> registro null
                        //qry += " AND (id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") ";
                        ////se seleziono un RF non deve cercare per id_registro = null
                        //if (cha_rf)
                        //{
                        //    qry += ")";
                        //}
                        //else
                        //{
                        //    qry += " or id_registro is null)";
                        //}
                        #endregion
                        //  if (!no_filtro_aoo)
                        //   {
                        if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                            qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") or a.id_registro is null)";
                        if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && cha_rf)
                            qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + "))";
                        if (string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                            qry += " AND a.id_registro is null";
                        //  }
                    }


                    // ###############     MODELLI DI TRASMISSIONE    #################
                    // se:		è stata chiamata da modelli di trasmissione o dalla funzionalità trova e sostituisci
                    // allora:	la query deve filtrare anche per registro		
                    if (
                        (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MODELLO_TRASM ||
                            qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_REPLACE_ROLE ||
                            qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE)
                        &&
                        (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO)
                        )
                    {
                        if (qc.caller.IdRegistro != null && qc.caller.IdRegistro != string.Empty)
                        {
                            q.setParam("param2", ",DPA_L_RUOLO_REG ");
                            qry += " AND DPA_L_RUOLO_REG.ID_RUOLO_IN_UO = A.SYSTEM_ID AND " +
                                " DPA_L_RUOLO_REG.id_registro = " + qc.caller.IdRegistro;
                        }
                        else
                        {
                            q.setParam("param2", ",DPA_L_RUOLO_REG ");
                            qry += " AND DPA_L_RUOLO_REG.ID_RUOLO_IN_UO = A.SYSTEM_ID ";
                        }
                    }

                    //############### Ruolo Interoperabilità nomail ####################
                    if (
                        (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_REG_NOMAIL
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_RESP_REG)
                        &&
                        (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO)
                        &&
                        (qc.caller.IdRegistro != null && qc.caller.IdRegistro != string.Empty)
                        )
                    {
                        q.setParam("param2", ",DPA_L_RUOLO_REG ");
                        qry += " AND DPA_L_RUOLO_REG.ID_RUOLO_IN_UO = A.SYSTEM_ID AND " +
                            " DPA_L_RUOLO_REG.id_registro = " + qc.caller.IdRegistro;
                    }

                    if (qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_REG_NOMAIL &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_RESP_REG &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MODELLO_TRASM &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_REPLACE_ROLE &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE)
                    {
                        q.setParam("param2", "");
                    }

                    // Se si usa la rubrica da GESTIONE RUBRICA vengono restituiti solamente
                    //corrispondenti esterni all'amministrazione creati sul registri visibili al ruolo corrente
                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ESTERNI_AMM
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA)
                       || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST)
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO)
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO)
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_NO_FILTRI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_NO_UO)
                    {
                        //se è un solo registro verifico se è un RF
                        bool cha_rf = false;
                        if (qc.caller != null && !string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && qc.caller.filtroRegistroPerRicerca.IndexOf(",") == -1)
                        {
                            DocsPaVO.utente.Registro reg = GetRegistro(qc.caller.filtroRegistroPerRicerca);
                            if (reg != null && reg.chaRF == "1")
                                cha_rf = true;
                        }

                        #region codice commentato
                        //qry += " AND (id_registro IN (" + qc.caller.IdRegistro + ") or id_registro is null)";

                        //MODIFICA BUG: non carica valori in rubrica per le liste di distribuzione --> registro null
                        //qry += " AND (id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") ";
                        ////se seleziono un RF non deve cercare per id_registro = null
                        //if (cha_rf)
                        //{
                        //    qry += ")";
                        //}
                        //else
                        //{
                        //    qry += " or id_registro is null)";
                        //}
                        #endregion
                        if (!no_filtro_aoo)
                        {
                            if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                                qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") or a.id_registro is null)";
                            if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && cha_rf)
                                qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + "))";
                            if (string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                                qry += " AND a.id_registro is null";
                        }
                    }
                    #region codice commentato
                    //sabrina: liste di distribuzione
                    //corrispondenti esterni all'amministrazione creati sui registri visibili al ruolo corrente

                    //if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE)
                    //{
                    //    if (qc.caller != null && (qc.caller.IdRegistro != null && !qc.caller.IdRegistro.Equals("")))
                    //        qry += " AND (ID_REGISTRO " + qc.caller.IdRegistro + " OR ID_REGISTRO IS NULL)";
                    //}
                    #endregion

                    //####### Liste di distribuzione ######
                    if (qc.doListe)
                    {
                        //E' selezionata solo la ricerca delle liste quindi niente UNION
                        if (!qc.doUo && !qc.doRuoli && !qc.doUtenti && !qc.doRF)
                        {
                            qry = "(a.id_amm='" + _user.idAmministrazione + "') AND " +
                                    "(a.cha_tipo_urp in ('L'))";
                            if ((qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals("")) || (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals("")))
                            {
                                qry += " AND (";
                                bool res = false;
                                // PALUMBO: intervento per reperire le liste utente
                                //if (qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals(""))
                                if (qc.caller != null && qc.caller.IdPeople != null && !qc.caller.IdPeople.Equals(""))
                                {
                                    //qry += "(a.id_people_liste=" + qc.caller.IdUtente + " AND a.id_gruppo_liste is null) ";
                                    qry += "(a.id_people_liste=" + qc.caller.IdPeople + " AND a.id_gruppo_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                if (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals(""))
                                {
                                    qry += " (a.id_gruppo_liste= (select id_gruppo from dpa_corr_globali where system_id = " + _user.idGruppo + " ) AND a.id_people_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                qry += " (a.id_people_liste is null AND a.id_gruppo_liste is null)) AND " +
                                        "a.dta_fine IS NULL ";
                            }
                        }
                        //E' una ricerca combinata liste con utenti, ruoli o uffici
                        else
                        {
                            qry += " UNION ";

                            qry += " SELECT /*+index (a)*/ " +
                                "a.var_cod_rubrica, " +
                                "a.var_desc_corr, " +
                                "(CASE WHEN a.cha_tipo_ie = 'I' THEN 1 ELSE 0 END) AS interno, a.cha_tipo_urp, a.system_id, " + userDb + "getcodreg (a.id_registro) cod_reg_rf, a.id_registro, a.CHA_DISABLED_TRASM, a.DTA_FINE, a.VAR_NOME, a.VAR_COGNOME, dett.var_cod_fisc as VAR_COD_FISC, dett.var_cod_pi as VAR_COD_PI,a.ID_PEOPLE " +
                                "FROM dpa_corr_globali a LEFT JOIN dpa_dett_globali dett ON dett.id_corr_globali = a.system_id " +
                                "WHERE " +
                                "(a.id_amm='" + _user.idAmministrazione + "') AND " +
                                "(a.cha_tipo_urp in ('L'))";
                            if ((qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals("")) || (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals("")))
                            {
                                qry += " AND (";
                                bool res = false;
                                // PALUMBO: intervento per reperire le liste utente
                                //if (qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals(""))
                                if (qc.caller != null && qc.caller.IdPeople != null && !qc.caller.IdPeople.Equals(""))
                                {
                                    //qry += "(a.id_people_liste=" + qc.caller.IdUtente + " AND a.id_gruppo_liste is null) ";
                                    qry += "(a.id_people_liste=" + qc.caller.IdPeople + " AND a.id_gruppo_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                if (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals(""))
                                {
                                    qry += " (a.id_gruppo_liste= (select id_gruppo from dpa_corr_globali where system_id = " + _user.idGruppo + " ) AND a.id_people_liste is null) ";
                                    //qry += " (a.id_gruppo_liste=" + _user.idGruppo + " AND a.id_people_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                qry += " (a.id_people_liste is null AND a.id_gruppo_liste is null))";
                            }

                            //"((id_people_liste="+qc.caller.IdUtente+" AND id_gruppo_liste is null) "+
                            //"OR (id_gruppo_liste=" + _user.idGruppo + " AND id_people_liste is null) "+
                            //"OR (id_people_liste is null AND id_gruppo_liste is null))";
                            if (qc.localita != null && qc.localita != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_localita) LIKE upper('%{0}%')) and id_corr_globali = a.system_id))", qc.localita.Replace("'", "''"));
                            }

                            //if (qc.cf_piva != null && qc.cf_piva != "")
                            //{
                            //    qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fiscale) = upper('{0}')) and id_corr_globali = a.system_id)) ", qc.cf_piva.Replace("'", "''"));
                            //}

                            if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fisc) LIKE upper('%{0}%')) and id_corr_globali = a.system_id))", qc.codiceFiscale.Replace("'", "''"));
                            }

                            if (qc.partitaIva != null && qc.partitaIva != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_pi) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) ", qc.partitaIva.Replace("'", "''"));
                            }

                            if (!string.IsNullOrEmpty(qc.email))
                            {
                                qry += String.Format(" AND (upper(a.VAR_EMAIL) LIKE upper('%{0}%')) ", qc.email.Replace("'", "''"));
                            }
                        }

                        //Verifico eventuali condizioni ulteriori per la ricerca. Codice-Descrizione-Registro
                        //Codice
                        if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                            qry += String.Format(" AND (upper(a.var_cod_rubrica) = upper('{0}')) ", qc.codice.Replace("'", "''"));
                        else if (qc.codice != null && qc.codice != "")
                            qry += String.Format(" AND (upper(a.var_cod_rubrica) LIKE upper('%{0}%')) ", qc.codice.Replace("'", "''"));

                        //Descrizione
                        if (qc.descrizione != null && qc.descrizione != "")
                        {
                            qry += String.Format(" AND (upper(a.var_desc_corr) LIKE upper('%{0}%')) ", qc.descrizione.Replace("'", "''"));
                            //qry += " AND ";
                            //qry = ricercaFullText("DESCRIZIONE", qry, qc.descrizione);
                        }
                        //Registro - Eventualmente da implementare

                    }

                    #region RF
                    if (qc.doRF)
                    {
                        //E' selezionata solo la ricerca degli RF quindi niente UNION
                        if (!qc.doUo && !qc.doRuoli && !qc.doUtenti && !qc.doListe)
                        {
                            if (qry.IndexOf("a.cha_tipo_urp in ('F')") == -1)
                                qry += " ( (a.id_amm=" + _user.idAmministrazione + ") OR a.id_amm is null) AND cha_tipo_urp = 'F' ";
                        }
                        //E' una ricerca combinata RF con utenti, ruoli, uffici o liste
                        else
                        {
                            qry += " UNION ";

                            qry += " SELECT /*+index (a)*/ " +
                                "a.var_cod_rubrica, " +
                                "a.var_desc_corr, " +
                                "(CASE WHEN a.cha_tipo_ie = 'I' THEN 1 ELSE 0 END) AS interno, a.cha_tipo_urp, a.system_id , " + userDb + "getcodreg (a.id_registro) cod_reg_rf, a.id_registro, a.CHA_DISABLED_TRASM, a.DTA_FINE, a.VAR_NOME, a.VAR_COGNOME, dett.var_cod_fisc as VAR_COD_FISC, dett.var_cod_pi as VAR_COD_PI, a.ID_PEOPLE " +
                                "FROM dpa_corr_globali a LEFT JOIN dpa_dett_globali dett ON dett.id_corr_globali = a.system_id " +
                                "WHERE " +
                                "(a.id_amm='" + _user.idAmministrazione + "') AND a.cha_tipo_urp = 'F' ";

                            //Verifico eventuali condizioni ulteriori per la ricerca. Codice-Descrizione-Registro
                            //Codice
                            if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                                qry += String.Format(" AND (upper(a.var_cod_rubrica) = upper('{0}')) ", qc.codice.Replace("'", "''"));
                            else if (qc.codice != null && qc.codice != "")
                                qry += String.Format(" AND (upper(a.var_cod_rubrica) LIKE upper('%{0}%')) ", qc.codice.Replace("'", "''"));

                            //Descrizione
                            if (qc.descrizione != null && qc.descrizione != "")
                            {
                                qry += String.Format(" AND (upper(a.var_desc_corr) LIKE upper('%{0}%')) ", qc.descrizione.Replace("'", "''"));
                                //qry += " AND ";
                                //qry = ricercaFullText("DESCRIZIONE", qry, qc.descrizione);
                            }

                            //Registro - Eventualmente da implementare

                            if (qc.localita != null && qc.localita != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_localita) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) ", qc.localita.Replace("'", "''"));
                            }

                            //if (qc.cf_piva != null && qc.cf_piva != "")
                            //{
                            //    qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fiscale) = upper('{0}')) and id_corr_globali = a.system_id)) ", qc.cf_piva.Replace("'", "''"));
                            //}


                            if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fisc) LIKE upper('%{0}%')) and id_corr_globali = a.system_id))", qc.codiceFiscale.Replace("'", "''"));
                            }

                            if (qc.partitaIva != null && qc.partitaIva != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_pi) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) ", qc.partitaIva.Replace("'", "''"));
                            }

                            if (!string.IsNullOrEmpty(qc.email))
                            {
                                qry += String.Format(" AND (upper(a.VAR_EMAIL) LIKE upper('%{0}%')) ", qc.email.Replace("'", "''"));
                            }

                        }

                        //if (qc.caller.filtroRegistroPerRicerca != null && qc.caller.filtroRegistroPerRicerca != "")
                        //{
                        //    qry += " and ID_REGISTRO in (" + qc.caller.filtroRegistroPerRicerca + ")";
                        //}
                    }
                    #endregion

                    bool nessun_sottoposto = false;

                    //INSERITO PER TROVARE SOLTANTO I RUOLI SOTTOPOSTI
                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO)
                    {
                        ArrayList listaRuoliInf;
                        DataSet dsTempRuoli = new DataSet();
                        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                        DocsPaVO.utente.Ruolo ruoloGlobale = utenti.getRuoloById(qc.caller.IdRuolo);
                        DocsPaVO.utente.Ruolo mioRuolo = utenti.GetRuoloByIdGruppo(ruoloGlobale.idGruppo);
                        DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                        DocsPaVO.trasmissione.TipoOggetto tipo = new DocsPaVO.trasmissione.TipoOggetto();
                        listaRuoliInf = gerarchia.getGerarchiaInf(mioRuolo, null, null, tipo);
                        listaRuoliInf.Add(mioRuolo);

                        string addConstant = null;

                        if (listaRuoliInf.Count != 0)
                        {
                            addConstant = " AND A.SYSTEM_ID IN (";

                            for (int i = 0; i < listaRuoliInf.Count; i++)
                            {
                                addConstant = addConstant + ((DocsPaVO.utente.Ruolo)listaRuoliInf[i]).systemId;
                                if (i < listaRuoliInf.Count - 1)
                                {
                                    if (i % 998 == 0 && i > 0)
                                    {

                                    }
                                    else
                                    {
                                        addConstant += ", ";
                                    }
                                }
                                else
                                {
                                    addConstant += ")";
                                }
                            }
                            qry += addConstant;
                        }
                        else
                        {
                            nessun_sottoposto = true;
                        }
                    }

                    qry += " order by cha_tipo_urp desc, var_desc_corr";

                    q.setParam("param1", qry);

                    string mySql = q.getSQL();
                    logger.Debug(mySql);

                    if (nessun_sottoposto == true && qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO)
                    {
                        mySql = "";
                    }

                    if (!this.ExecuteQuery(out ds, mySql))
                        throw new Exception(this.LastExceptionMessage);
                }

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DocsPaVO.rubrica.ElementoRubrica er = new DocsPaVO.rubrica.ElementoRubrica();
                    er.codice = dr["var_cod_rubrica"].ToString();
                    er.descrizione = dr["var_desc_corr"].ToString();
                    er.interno = (Convert.ToDecimal(dr["interno"]) == 1);
                    er.tipo = dr["cha_tipo_urp"].ToString();
                    er.systemId = dr["system_id"].ToString();
                    er.has_children = false;
                    //CODICI E ID DEI REGISTRI MODIFICA 22/07/2010 Fabio
                    if (dr.Table.Columns.Contains("COD_REG_RF"))
                    {
                        er.codiceRegistro = dr["COD_REG_RF"].ToString();
                    }

                    if (dr.Table.Columns.Contains("ID_REGISTRO"))
                    {
                        er.idRegistro = dr["ID_REGISTRO"].ToString();
                    }

                    if (dr.Table.Columns.Contains("CHA_DISABLED_TRASM") && dr["CHA_DISABLED_TRASM"] != null && dr["CHA_DISABLED_TRASM"].ToString() == "1")
                    {
                        er.disabledTrasm = true;
                    }

                    if (dr.Table.Columns.Contains("DTA_FINE") && dr["DTA_FINE"] != null && !string.IsNullOrEmpty(dr["DTA_FINE"].ToString()))
                    {
                        er.disabled = true;
                    }

                    if (dr.Table.Columns.Contains("VAR_NOME"))
                    {
                        er.nome = dr["VAR_NOME"].ToString();
                    }

                    if (dr.Table.Columns.Contains("VAR_COGNOME"))
                    {
                        er.cognome = dr["VAR_COGNOME"].ToString();
                    }

                    //if (dr.Table.Columns.Contains("COD_FISC"))
                    //{
                    //    er.cf_piva = dr["COD_FISC"].ToString();
                    //}

                    if (dr.Table.Columns.Contains("VAR_COD_FISC"))
                    {
                        er.codiceFiscale = dr["VAR_COD_FISC"].ToString();
                    }

                    if (dr.Table.Columns.Contains("VAR_COD_PI"))
                    {
                        er.partitaIva = dr["VAR_COD_PI"].ToString();
                    }

                    if (dr.Table.Columns.Contains("ID_PEOPLE"))
                    {
                        er.idPeople = dr["ID_PEOPLE"].ToString();
                    }

                    elementi.Add(er);
                }

                return elementi;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ArrayList GetElementiRubricaPaging(DocsPaVO.rubrica.ParametriRicercaRubrica qc, int firstRowNum, int maxRowForPage, out int totale)
        {
            int total = 0;
            int tot = 0;
            string qry = "";
            ArrayList elementi = new ArrayList();
            DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
            bool no_filtro_aoo = obj.isFiltroAooEnabled();
            string userDb = string.Empty;
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];

            //Laura 22 Febbraio 2013
            if (dbType.ToUpper() == "SQL")
            {
                userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession() + ".";
                tot = this.GetCountElementiRubricaPaging(qc, firstRowNum, maxRowForPage);
            }

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RICERCA_ELEMENTI_RUBRICA_PAGING");


            if (dbType.ToUpper() == "SQL")
                q.setParam("dbuser", userDb);

            q.setParam("startRow", (firstRowNum + 1).ToString());
            q.setParam("endRow", (firstRowNum + maxRowForPage).ToString());

            DataSet ds = new DataSet();
            try
            {
                ArrayList sp_params = new ArrayList();
                if (qc.parent != null && qc.parent != "")
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("id_amm", Convert.ToInt32(_user.idAmministrazione), 0, DirectionParameter.ParamInput, System.Data.DbType.Decimal));

                    switch (qc.tipoIE)
                    {
                        case DocsPaVO.addressbook.TipoUtente.INTERNO:
                        default:
                            sp_params.Add(new DocsPaUtils.Data.ParameterSP("cha_tipo_ie", "I", 1, DirectionParameter.ParamInput, System.Data.DbType.String));
                            break;

                        case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                            sp_params.Add(new DocsPaUtils.Data.ParameterSP("cha_tipo_ie", "E", 1, DirectionParameter.ParamInput, System.Data.DbType.String));
                            break;
                    }
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("var_cod_rubrica", qc.parent, 128, DirectionParameter.ParamInput, System.Data.DbType.String));

                    int corr_types = 0;
                    corr_types += (qc.doUo ? 1 : 0);
                    corr_types += (qc.doRuoli ? 2 : 0);
                    corr_types += (qc.doUtenti ? 4 : 0);
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("corr_types", corr_types, 0, DirectionParameter.ParamInput, System.Data.DbType.Decimal));

                    if (this.ExecuteStoredProcedure("dpa3_get_children", sp_params, ds) != 1)
                        throw new Exception(this.LastExceptionMessage);
                }
                else
                {
                    // Non sono estratti i corrispondenti con tipologia "C", 
                    // ovvero gli elementi inseriti da rubrica comune
                    //La condizione sui ruoli disabilitati (DTA_FINE valorizzata) non viene considerata per calltype di ricerca
                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CREATOR ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_OWNER_AUTHOR ||
                        //IACOZZILLI GIORDANO 25/06/2013
                        //Aggiungo il calltype per il deposito:
                         qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEP_OSITO ||
                        //FINE
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_TODOLIST ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_NO_FILTRI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORRISPONDENTE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                        )
                    {
                        qry += "(" + DocsPaDbManagement.Functions.Functions.getNVL("a.cha_tipo_corr", "'X'") + " != 'C' OR (" + DocsPaDbManagement.Functions.Functions.getNVL("a.cha_tipo_corr", "'X'") + " = 'C' AND DTA_FINE IS NOT NULL))" + "AND ( (a.id_amm=" + _user.idAmministrazione + ") OR a.id_amm is null) AND ";
                    }
                    else
                    {
                        qry += DocsPaDbManagement.Functions.Functions.getNVL("a.cha_tipo_corr", "'X'") + " != 'C' AND ( (a.id_amm=" + _user.idAmministrazione + ") OR a.id_amm is null) AND ";
                    }
                    
                    //if (qc.caller.filtroRegistroPerRicerca != null && qc.caller.filtroRegistroPerRicerca != "") {
                    //    qry += "id_registro in (" + qc.caller.filtroRegistroPerRicerca + ") AND ";
                    //}
                    switch (qc.tipoIE)
                    {
                        case DocsPaVO.addressbook.TipoUtente.INTERNO:
                            if (qc.doListe || qc.doRF)
                                qry += "(a.cha_tipo_ie='I' OR a.CHA_TIPO_IE IS NULL) AND ";
                            else
                                qry += "(a.cha_tipo_ie='I') AND ";
                            break;

                        case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                            if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE)
                                || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ESTERNI_AMM)
                                //modifica del 13/05/2009
                                || qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO
                                //fine modifica del 13/05/2009
                                )
                            {
                                qry += "(a.cha_tipo_ie='E') AND ";
                            }
                            break;

                        case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                            break;
                    }

                    if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                        qry += String.Format("(upper(a.var_cod_rubrica) = upper('{0}')) AND ", qc.codice.Replace("'", "''"));
                    else if (qc.codice != null && qc.codice != "")
                        qry += String.Format("(upper(a.var_cod_rubrica) LIKE upper('%{0}%')) AND ", qc.codice.Replace("'", "''"));

                    if (qc.descrizione != null && qc.descrizione != "")
                        qry += String.Format("(upper(a.var_desc_corr) LIKE upper('%{0}%')) AND ", qc.descrizione.Replace("'", "''"));
                    //qry = ricercaFullText("DESCRIZIONE", qry, qc.descrizione) + " AND ";

                    if (qc.citta != null && qc.citta != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_citta) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.citta.Replace("'", "''"));
                    }

                    if (qc.localita != null && qc.localita != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_localita) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.localita.Replace("'", "''"));
                    }

                    //if (qc.cf_piva != null && qc.cf_piva != "")
                    //{
                    //    qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_cod_fiscale) = upper('{0}')) and id_corr_globali = a.system_id)) AND ", qc.cf_piva.Replace("'", "''"));
                    //}

                    if (!string.IsNullOrEmpty(qc.email))
                    {
                        qry += String.Format("(upper(a.VAR_EMAIL) LIKE upper('%{0}%')) AND ", qc.email.Replace("'", "''"));
                    }

                    if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_cod_fisc) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.codiceFiscale.Replace("'", "''"));
                    }

                    if (qc.partitaIva != null && qc.partitaIva != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_cod_pi) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.partitaIva.Replace("'", "''"));
                    }

                    //filtro per system_ID
                    if (qc.systemId != null && qc.systemId != "")
                    {
                        qry += String.Format("a.system_id = " + qc.systemId + " AND ");
                    }

                    //                    if (qc.doUo || qc.doRuoli || qc.doUtenti || qc.doListe || qc.doRF)
                    if (qc.doUo || qc.doRuoli || qc.doUtenti || qc.doRF)
                    {
                        qry += "a.cha_tipo_urp in (";
                        if (qc.doUo)
                            qry += "'U',";

                        if (qc.doRuoli)
                            qry += "'R',";

                        if (qc.doUtenti)
                            qry += "'P',";

                        //if (qc.doListe)
                        //    qry += "'L',";

                        if (qc.doRF)
                            qry += "'F',";

                        if (qry.EndsWith(","))
                            qry = qry.Substring(0, qry.Length - 1) + ")";
                    }
                    else
                    {
                        if (!qc.doListe)
                        {
                            //Laura 22 Febbraio 2013
                            if (tot != 0)
                                totale = tot;
                            else
                                totale = total;
                            return elementi;
                        }
                    }

                    if (qry.EndsWith(" AND "))
                        qry = qry.Substring(0, qry.Length - " AND ".Length);


                    //La condizione sui ruoli disabilitati (DTA_FINE valorizzata) non viene considerata per calltype di ricerca
                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CREATOR ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_OWNER_AUTHOR ||
                        //IACOZZILLI GIORDANO 25/06/2013
                        //Aggiungo il calltype per il deposito:
                         qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEP_OSITO ||
                        //FINE
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_TODOLIST ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_NO_FILTRI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORRISPONDENTE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                        )
                    {
                        //qry += " AND a.dta_fine IS NULL";
                    }
                    else
                    {
                        qry += " AND a.dta_fine IS NULL";
                    }

                    if (!qc.extSystems)
                    {
                        qry += " AND a.cha_system_role !='1'";
                    }

                    if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORRISPONDENTE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO)
                        && qc.caller != null && !string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca))
                    {
                        qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") or a.id_registro is null)";
                    }

                    #region codice commentato
                    // #### NUOVO GESTIONE PER PITRE ####
                    // RICERCA DEI I CORRISPONDENTI GLOBALI DA DOC PROTOCOLLO
                    //if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE
                    //    &&
                    //    (qc.caller.IdRegistro != null || qc.caller.IdRegistro != string.Empty)
                    //    &&
                    //    (qc.doUo || qc.doRuoli || qc.doUtenti)
                    //    )
                    //{
                    //   qry += " AND (id_registro is null ";

                    //   ////dato un registro, ricerco anche tra gli rf ad esso associati che il ruolo può vedere
                    //    Utenti ut = new Utenti();
                    //    ArrayList listaRf = ut.GetListaRegistriRfRuolo(qc.caller.IdRuolo, "", "");

                    //    if (listaRf != null && listaRf.Count > 0)
                    //   {
                    //       foreach (DocsPaVO.utente.Registro reg in listaRf)
                    //        {
                    //            qry += " or id_registro = " + reg.systemId;
                    //        }
                    //    }
                    //    qry += " )";
                    //}

                    ////CASO in cui si è selezionata la voce Esterni AOO.
                    ////in tal caso ricerco solamente tra 
                    //if (
                    //    (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN
                    //    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
                    //    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                    //    &&
                    //      (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO)
                    //    &&
                    //    (qc.caller.IdRegistro != null || qc.caller.IdRegistro != string.Empty)
                    //    &&
                    //    (qc.doUo || qc.doRuoli || qc.doUtenti)
                    //    )
                    //{
                    //    qry += " AND CHA_TIPO_IE = 'I'";
                    //}
                    #endregion

                    if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INGRESSO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_ESTERNI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                        || qc.calltype== DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE
                        )
                        &&
                        (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE || qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO)
                        &&
                        (qc.caller.IdRegistro != null || qc.caller.IdRegistro != string.Empty)
                        &&
                        (qc.doUo || qc.doRuoli || qc.doUtenti)
                        )
                    {
                        //se è un solo registro verifico se è un RF
                        bool cha_rf = false;
                        if (qc.caller.filtroRegistroPerRicerca.IndexOf(",") == -1)
                        {
                            DocsPaVO.utente.Registro reg = GetRegistro(qc.caller.filtroRegistroPerRicerca);
                            if (reg != null && reg.chaRF == "1")
                                cha_rf = true;
                        }
                        #region codice commentato
                        //qry += " AND (id_registro IN (" + qc.caller.IdRegistro + ") or id_registro is null)";
                        //MODIFICA BUG: non carica valori in rubrica per le liste di distribuzione --> registro null
                        //qry += " AND (id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") ";
                        ////se seleziono un RF non deve cercare per id_registro = null
                        //if (cha_rf)
                        //{
                        //    qry += ")";
                        //}
                        //else
                        //{
                        //    qry += " or id_registro is null)";
                        //}
                        #endregion
                        //  if (!no_filtro_aoo)
                        //   {
                        if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                            qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") or a.id_registro is null)";
                        if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && cha_rf)
                            qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + "))";
                        if (string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                            qry += " AND a.id_registro is null";
                        //  }
                    }


                    // ###############     MODELLI DI TRASMISSIONE    #################
                    // se:		è stata chiamata da modelli di trasmissione o dalla funzionalità trova e sostituisci
                    // allora:	la query deve filtrare anche per registro		
                    if (
                        (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MODELLO_TRASM ||
                            qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_REPLACE_ROLE ||
                            qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE)
                        &&
                        (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO)
                        )
                    {
                        if (qc.caller.IdRegistro != null && qc.caller.IdRegistro != string.Empty)
                        {
                            q.setParam("param2", " LEFT JOIN DPA_L_RUOLO_REG ON DPA_L_RUOLO_REG.ID_RUOLO_IN_UO = A.SYSTEM_ID ");
                            qry += " AND DPA_L_RUOLO_REG.id_registro = " + qc.caller.IdRegistro;
                        }
                        else
                        {
                            q.setParam("param2", " LEFT JOIN DPA_L_RUOLO_REG ON DPA_L_RUOLO_REG.ID_RUOLO_IN_UO = A.SYSTEM_ID ");
                            //q.setParam("param2", ",DPA_L_RUOLO_REG ");
                        }
                    }

                    //############### Ruolo Interoperabilità nomail ####################
                    if (
                        (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_REG_NOMAIL
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_RESP_REG)
                        &&
                        (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO)
                        &&
                        (qc.caller.IdRegistro != null && qc.caller.IdRegistro != string.Empty)
                        )
                    {
                        q.setParam("param2", " LEFT JOIN DPA_L_RUOLO_REG ON DPA_L_RUOLO_REG.ID_RUOLO_IN_UO = A.SYSTEM_ID ");
                        qry += " AND DPA_L_RUOLO_REG.id_registro = " + qc.caller.IdRegistro;
                    }

                    if (qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_REG_NOMAIL &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_RESP_REG &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MODELLO_TRASM &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_REPLACE_ROLE &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_VIS_UTENTE)
                    {
                        q.setParam("param2", "");
                    }

                    // Se si usa la rubrica da GESTIONE RUBRICA vengono restituiti solamente
                    //corrispondenti esterni all'amministrazione creati sul registri visibili al ruolo corrente
                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ESTERNI_AMM
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA)
                       || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST)
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO)
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO)
                       // || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_NO_FILTRI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_NO_UO)
                    {
                        //se è un solo registro verifico se è un RF
                        bool cha_rf = false;
                        if (qc.caller != null && !string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && qc.caller.filtroRegistroPerRicerca.IndexOf(",") == -1)
                        {
                            DocsPaVO.utente.Registro reg = GetRegistro(qc.caller.filtroRegistroPerRicerca);
                            if (reg != null && reg.chaRF == "1")
                                cha_rf = true;
                        }

                        if (!no_filtro_aoo)
                        {
                            if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                                qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") or a.id_registro is null)";
                            if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && cha_rf)
                                qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + "))";
                            if (string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                                qry += " AND a.id_registro is null";
                        }
                    }

                    //####### Liste di distribuzione ######
                    if (qc.doListe)
                    {
                        //E' selezionata solo la ricerca delle liste quindi niente UNION
                        if (!qc.doUo && !qc.doRuoli && !qc.doUtenti && !qc.doRF)
                        {
                            qry = "(a.id_amm='" + _user.idAmministrazione + "') AND " +
                                    "(a.cha_tipo_urp in ('L'))";
                            if ((qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals("")) || (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals("")))
                            {
                                qry += " AND (";
                                bool res = false;
                                if (qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals(""))
                                {
                                    qry += "(a.id_people_liste=" + qc.caller.IdUtente + " AND a.id_gruppo_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                if (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals(""))
                                {
                                    qry += " (a.id_gruppo_liste=" + _user.idGruppo + " AND a.id_people_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                qry += " (a.id_people_liste is null AND a.id_gruppo_liste is null)) AND " +
                                        "a.dta_fine IS NULL ";
                            }
                        }
                        //E' una ricerca combinata liste con utenti, ruoli o uffici
                        else
                        {
                            string rn = "";
                            if (dbType.ToUpper() != "SQL")
                            {
                                rn = ", ROWNUM RN ";
                            }
                            qry += " UNION ";

                            qry += " SELECT a.var_cod_rubrica, DOCUMENTTYPES.DESCRIPTION AS canale," +
                                //Laura 25 Febbraio 2013
                                " " +
                                "a.var_desc_corr, " +
                                "(CASE WHEN a.cha_tipo_ie = 'I' THEN 1 ELSE 0 END) AS interno, a.cha_tipo_urp, a.system_id, " + userDb + "getcodreg (a.id_registro) cod_reg_rf, a.id_registro, a.CHA_DISABLED_TRASM, a.DTA_FINE, a.VAR_NOME, a.VAR_COGNOME, dett.var_cod_fisc as VAR_COD_FISC, dett.var_cod_pi as VAR_COD_PI,a.ID_PEOPLE, a.ID_PEOPLE_LISTE, a.ID_GRUPPO_LISTE " + rn +
                                "FROM dpa_corr_globali a LEFT JOIN dpa_dett_globali dett ON dett.id_corr_globali = a.system_id " +
                                "LEFT JOIN DPA_T_CANALE_CORR ON a.system_id = DPA_T_CANALE_CORR.ID_CORR_GLOBALE " +
                                "LEFT JOIN DOCUMENTTYPES ON DPA_T_CANALE_CORR.ID_DOCUMENTTYPE = DOCUMENTTYPES.SYSTEM_ID " +
                                "WHERE " +
                                "(a.id_amm='" + _user.idAmministrazione + "') AND " +
                                "(a.cha_tipo_urp in ('L'))";
                            if ((qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals("")) || (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals("")))
                            {
                                qry += " AND (";
                                bool res = false;
                                if (qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals(""))
                                {
                                    qry += "(a.id_people_liste=" + qc.caller.IdUtente + " AND a.id_gruppo_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                if (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals(""))
                                {
                                    qry += " (a.id_gruppo_liste=" + _user.idGruppo + " AND a.id_people_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                qry += " (a.id_people_liste is null AND a.id_gruppo_liste is null))";
                            }

                            //"((id_people_liste="+qc.caller.IdUtente+" AND id_gruppo_liste is null) "+
                            //"OR (id_gruppo_liste=" + _user.idGruppo + " AND id_people_liste is null) "+
                            //"OR (id_people_liste is null AND id_gruppo_liste is null))";
                            if (qc.localita != null && qc.localita != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_localita) LIKE upper('%{0}%')) and id_corr_globali = a.system_id))", qc.localita.Replace("'", "''"));
                            }

                            //if (qc.cf_piva != null && qc.cf_piva != "")
                            //{
                            //    qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fiscale) = upper('{0}')) and id_corr_globali = a.system_id)) ", qc.cf_piva.Replace("'", "''"));
                            //}

                            if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fisc) LIKE upper('%{0}%')) and id_corr_globali = a.system_id))", qc.codiceFiscale.Replace("'", "''"));
                            }

                            if (qc.partitaIva != null && qc.partitaIva != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_pi) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) ", qc.partitaIva.Replace("'", "''"));
                            }

                            if (!string.IsNullOrEmpty(qc.email))
                            {
                                qry += String.Format(" AND (upper(a.VAR_EMAIL) LIKE upper('%{0}%')) ", qc.email.Replace("'", "''"));
                            }
                        }

                        //Verifico eventuali condizioni ulteriori per la ricerca. Codice-Descrizione-Registro
                        //Codice
                        if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                            qry += String.Format(" AND (upper(a.var_cod_rubrica) = upper('{0}')) ", qc.codice.Replace("'", "''"));
                        else if (qc.codice != null && qc.codice != "")
                            qry += String.Format(" AND (upper(a.var_cod_rubrica) LIKE upper('%{0}%')) ", qc.codice.Replace("'", "''"));

                        //Descrizione
                        if (qc.descrizione != null && qc.descrizione != "")
                        {
                            qry += String.Format(" AND (upper(a.var_desc_corr) LIKE upper('%{0}%')) ", qc.descrizione.Replace("'", "''"));
                            //qry += " AND ";
                            //qry = ricercaFullText("DESCRIZIONE", qry, qc.descrizione);
                        }
                        //Registro - Eventualmente da implementare

                    }

                    #region RF
                    if (qc.doRF)
                    {
                        //E' selezionata solo la ricerca degli RF quindi niente UNION
                        if (!qc.doUo && !qc.doRuoli && !qc.doUtenti && !qc.doListe)
                        {
                            if (qry.IndexOf("a.cha_tipo_urp in ('F')") == -1)
                                qry += " ( (a.id_amm=" + _user.idAmministrazione + ") OR a.id_amm is null) AND cha_tipo_urp = 'F' ";
                        }
                        //E' una ricerca combinata RF con utenti, ruoli, uffici o liste
                        else
                        {
                            qry += " UNION ";

                            qry += " SELECT a.var_cod_rubrica, DOCUMENTTYPES.DESCRIPTION AS canale, " +
                                " " +
                                "a.var_desc_corr, " +
                                "(CASE WHEN a.cha_tipo_ie = 'I' THEN 1 ELSE 0 END) AS interno, a.cha_tipo_urp, a.system_id , " + userDb + "getcodreg (a.id_registro) cod_reg_rf, a.id_registro, a.CHA_DISABLED_TRASM, a.DTA_FINE, a.VAR_NOME, a.VAR_COGNOME, dett.var_cod_fisc as VAR_COD_FISC, dett.var_cod_pi as VAR_COD_PI, a.ID_PEOPLE, a.ID_PEOPLE_LISTE, a.ID_GRUPPO_LISTE, ROWNUM RN " +
                                "FROM dpa_corr_globali a LEFT JOIN dpa_dett_globali dett ON dett.id_corr_globali = a.system_id " +
                                "LEFT JOIN DPA_T_CANALE_CORR ON a.system_id = DPA_T_CANALE_CORR.ID_CORR_GLOBALE " +
                                "LEFT JOIN DOCUMENTTYPES ON DPA_T_CANALE_CORR.ID_DOCUMENTTYPE = DOCUMENTTYPES.SYSTEM_ID " +
                                "WHERE " +
                                "(a.id_amm='" + _user.idAmministrazione + "') AND a.cha_tipo_urp = 'F' ";

                            //Verifico eventuali condizioni ulteriori per la ricerca. Codice-Descrizione-Registro
                            //Codice
                            if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                                qry += String.Format(" AND (upper(a.var_cod_rubrica) = upper('{0}')) ", qc.codice.Replace("'", "''"));
                            else if (qc.codice != null && qc.codice != "")
                                qry += String.Format(" AND (upper(a.var_cod_rubrica) LIKE upper('%{0}%')) ", qc.codice.Replace("'", "''"));

                            //Descrizione
                            if (qc.descrizione != null && qc.descrizione != "")
                            {
                                qry += String.Format(" AND (upper(a.var_desc_corr) LIKE upper('%{0}%')) ", qc.descrizione.Replace("'", "''"));
                                //qry += " AND ";
                                //qry = ricercaFullText("DESCRIZIONE", qry, qc.descrizione);
                            }

                            //Registro - Eventualmente da implementare

                            if (qc.localita != null && qc.localita != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_localita) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) ", qc.localita.Replace("'", "''"));
                            }

                            //if (qc.cf_piva != null && qc.cf_piva != "")
                            //{
                            //    qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fiscale) = upper('{0}')) and id_corr_globali = a.system_id)) ", qc.cf_piva.Replace("'", "''"));
                            //}


                            if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fisc) LIKE upper('%{0}%')) and id_corr_globali = a.system_id))", qc.codiceFiscale.Replace("'", "''"));
                            }

                            if (qc.partitaIva != null && qc.partitaIva != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_pi) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) ", qc.partitaIva.Replace("'", "''"));
                            }

                            if (!string.IsNullOrEmpty(qc.email))
                            {
                                qry += String.Format(" AND (upper(a.VAR_EMAIL) LIKE upper('%{0}%')) ", qc.email.Replace("'", "''"));
                            }

                        }

                        //if (qc.caller.filtroRegistroPerRicerca != null && qc.caller.filtroRegistroPerRicerca != "")
                        //{
                        //    qry += " and ID_REGISTRO in (" + qc.caller.filtroRegistroPerRicerca + ")";
                        //}
                    }
                    #endregion

                    bool nessun_sottoposto = false;

                    //INSERITO PER TROVARE SOLTANTO I RUOLI SOTTOPOSTI
                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO)
                    {
                        ArrayList listaRuoliInf;
                        DataSet dsTempRuoli = new DataSet();
                        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                        DocsPaVO.utente.Ruolo ruoloGlobale = utenti.getRuoloById(qc.caller.IdRuolo);
                        DocsPaVO.utente.Ruolo mioRuolo = utenti.GetRuoloByIdGruppo(ruoloGlobale.idGruppo);
                        DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                        DocsPaVO.trasmissione.TipoOggetto tipo = new DocsPaVO.trasmissione.TipoOggetto();
                        listaRuoliInf = gerarchia.getGerarchiaInf(mioRuolo, null, null, tipo);
                        listaRuoliInf.Add(mioRuolo);

                        string addConstant = null;

                        if (listaRuoliInf.Count != 0)
                        {
                            addConstant = " AND ( A.SYSTEM_ID IN (";

                            for (int i = 0; i < listaRuoliInf.Count; i++)
                            {
                                addConstant = addConstant + ((DocsPaVO.utente.Ruolo)listaRuoliInf[i]).systemId;
                                if (i < listaRuoliInf.Count - 1)
                                {
                                    if (i % 998 == 0 && i > 0)
                                    {
                                        addConstant += ") OR A.SYSTEM_ID IN (";
                                    }
                                    else
                                    {
                                        addConstant += ", ";
                                    }
                                }
                                else
                                {
                                    addConstant += ")";
                                }
                            }

                            addConstant += ") ";
                            qry += addConstant;
                        }
                        else
                        {
                            nessun_sottoposto = true;
                        }
                    }

                    string orderby = "cha_tipo_urp desc, var_desc_corr";

                    //Laura 25 Febbraio 2013
                    if (dbType.ToUpper() == "SQL")
                    {
                        q.setParam("orderby", orderby);
                    }
                    else
                        qry += " order by cha_tipo_urp desc, var_desc_corr";

                    q.setParam("param1", qry);

                    string mySql = q.getSQL();
                    logger.Debug(mySql);

                    if (nessun_sottoposto == true && qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO)
                    {
                        mySql = "";
                    }

                    if (!this.ExecuteQuery(out ds, mySql))
                        throw new Exception(this.LastExceptionMessage);
                }

                bool first = true;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DocsPaVO.rubrica.ElementoRubrica er = new DocsPaVO.rubrica.ElementoRubrica();
                    er.codice = dr["var_cod_rubrica"].ToString();
                    er.descrizione = dr["var_desc_corr"].ToString();
                    er.interno = (Convert.ToDecimal(dr["interno"]) == 1);
                    er.tipo = dr["cha_tipo_urp"].ToString();
                    er.systemId = dr["system_id"].ToString();
                    er.canale = dr["canale"].ToString();
                    er.has_children = false;
                    //Laura 22 Febbraio 2013
                    if (dbType.ToUpper() != "SQL")

                        //ABBATANGELI DA RIPRISTINARE PER LA VERA PAGINAZIONE MA VERIFICARE ORACLE 10G
                        //if (first) total = int.Parse(dr["TOTALE"].ToString());
                        if (first) total = ds.Tables[0].Rows.Count;


                    //CODICI E ID DEI REGISTRI MODIFICA 22/07/2010 Fabio
                    if (dr.Table.Columns.Contains("COD_REG_RF"))
                    {
                        er.codiceRegistro = dr["COD_REG_RF"].ToString();
                    }

                    if (dr.Table.Columns.Contains("ID_REGISTRO"))
                    {
                        er.idRegistro = dr["ID_REGISTRO"].ToString();
                    }

                    if (dr.Table.Columns.Contains("CHA_DISABLED_TRASM") && dr["CHA_DISABLED_TRASM"] != null && dr["CHA_DISABLED_TRASM"].ToString() == "1")
                    {
                        er.disabledTrasm = true;
                    }

                    //************************
                    //IACOZZILLI GIORDANO:
                    //DEPOSITO: devo mettere una if per dar modo alla mia form di 
                    //ricerca di visualizzzare gli utenti per il D&D
                    //************************
                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEP_OSITO)
                    {
                        if (dr.Table.Columns.Contains("DTA_FINE") && dr["DTA_FINE"] != null && !string.IsNullOrEmpty(dr["DTA_FINE"].ToString()))
                        {
                            er.disabled = false;
                        }
                    }
                    else
                    {
                        if (dr.Table.Columns.Contains("DTA_FINE") && dr["DTA_FINE"] != null && !string.IsNullOrEmpty(dr["DTA_FINE"].ToString()))
                        {
                            er.disabled = true;
                        }
                    }
                    //************************
                    //IACOZZILLI GIORDANO:
                    //FINE
                    //************************
                    if (dr.Table.Columns.Contains("VAR_NOME"))
                    {
                        er.nome = dr["VAR_NOME"].ToString();
                    }

                    if (dr.Table.Columns.Contains("VAR_COGNOME"))
                    {
                        er.cognome = dr["VAR_COGNOME"].ToString();
                    }

                    //if (dr.Table.Columns.Contains("COD_FISC"))
                    //{
                    //    er.cf_piva = dr["COD_FISC"].ToString();
                    //}

                    if (dr.Table.Columns.Contains("VAR_COD_FISC"))
                    {
                        er.codiceFiscale = dr["VAR_COD_FISC"].ToString();
                    }

                    if (dr.Table.Columns.Contains("VAR_COD_PI"))
                    {
                        er.partitaIva = dr["VAR_COD_PI"].ToString();
                    }

                    if (dr.Table.Columns.Contains("ID_PEOPLE"))
                    {
                        er.idPeople = dr["ID_PEOPLE"].ToString();
                    }

                    if (dr.Table.Columns.Contains("ID_PEOPLE_LISTE"))
                    {
                        er.idPeopleLista = dr["ID_PEOPLE_LISTE"].ToString();
                    }

                    if (dr.Table.Columns.Contains("ID_GRUPPO_LISTE"))
                    {
                        er.idGruppoLista = dr["ID_GRUPPO_LISTE"].ToString();
                    }

                    elementi.Add(er);
                }
                //Laura 22 Febbraio 2013
                if (tot != 0)
                    totale = tot;
                else
                    totale = total;
                return elementi;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }








        public DocsPaVO.rubrica.ElementoRubrica GetElementoRubrica(string cod, string condRegistri)
        {
            DocsPaVO.rubrica.ElementoRubrica er = null;
            string srch_condition = "";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RICERCA_ELEMENTO_RUBRICA");
            try
            {
                if (cod.IndexOf(@"\") < 0)
                {
                    srch_condition = String.Format("upper(var_cod_rubrica)=upper('{0}') and (cha_tipo_corr != 'C' or cha_tipo_corr is null)", cod.Replace("'", "''"));
                }
                else
                {
                    string[] flds = System.Text.RegularExpressions.Regex.Split(cod, @"\\");
                    srch_condition = String.Format("upper(var_cod_rubrica)=upper('{0}') and (cha_tipo_corr != 'C' or cha_tipo_corr is null) and cha_tipo_ie='{1}'", flds[1], flds[0]);
                }

                srch_condition += String.Format(" and id_amm={0}", _user.idAmministrazione);

                if (!string.IsNullOrEmpty(condRegistri))
                    srch_condition += condRegistri;

                q.setParam("param1", srch_condition);

                string mySql = q.getSQL();
                logger.Debug(mySql);

                DataSet ds = new DataSet();
                if (!this.ExecuteQuery(out ds, "corrispondenti", mySql))
                    throw new Exception(this.LastExceptionMessage);

                if (ds.Tables["corrispondenti"].Rows.Count != 0)
                {
                    DataRow dr = ds.Tables["corrispondenti"].Rows[0];
                    er = new DocsPaVO.rubrica.ElementoRubrica();
                    //28 marzo 2008
                    er.systemId = dr["system_id"].ToString();
                    er.codice = dr["var_cod_rubrica"].ToString();
                    er.descrizione = dr["var_desc_corr"].ToString();
                    er.interno = (Convert.ToDecimal(dr["interno"]) == 1);
                    er.tipo = dr["cha_tipo_urp"].ToString();
                    er.has_children = (Convert.ToDecimal(dr["has_children"]) == 1);
                    if (dr.Table.Columns.Contains("CHA_DISABLED_TRASM") && dr["CHA_DISABLED_TRASM"] != null && dr["CHA_DISABLED_TRASM"].ToString() == "1")
                    {
                        er.disabledTrasm = true;
                    }
                    if (!string.IsNullOrEmpty(dr["dta_fine"].ToString()))
                        er.disabled = true;
                    else
                        er.disabled = false;
                }
                return er;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cod"></param>
        /// <returns></returns>
        public DocsPaVO.rubrica.ElementoRubrica GetElementoRubricaSimple(string cod)
        {
            DocsPaVO.rubrica.ElementoRubrica er = null;
            string srch_condition = "";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RICERCA_ELEMENTO_RUBRICA_SIMPLE");
            try
            {
                if (cod.IndexOf(@"\") < 0)
                {
                    srch_condition = String.Format("upper(var_cod_rubrica)=upper('{0}')", cod);
                }
                else
                {
                    string[] flds = System.Text.RegularExpressions.Regex.Split(cod, @"\\");
                    srch_condition = String.Format("upper(var_cod_rubrica)=upper('{0}') and cha_tipo_ie='{1}'", flds[1], flds[0]);
                }

                srch_condition += String.Format(" and id_amm='{0}'", _user.idAmministrazione);

                q.setParam("param1", srch_condition);

                string mySql = q.getSQL();
                logger.Debug(mySql);

                DataSet ds = new DataSet();
                if (!this.ExecuteQuery(out ds, "corrispondenti", mySql))
                    throw new Exception(this.LastExceptionMessage);

                if (ds.Tables["corrispondenti"].Rows.Count != 0)
                {
                    DataRow dr = ds.Tables["corrispondenti"].Rows[0];
                    er = new DocsPaVO.rubrica.ElementoRubrica();
                    //28 marzo 2008
                    er.systemId = dr["system_id"].ToString();
                    er.codice = dr["var_cod_rubrica"].ToString();
                    er.descrizione = dr["var_desc_corr"].ToString();
                    er.interno = (Convert.ToDecimal(dr["interno"]) == 1);
                    er.tipo = dr["cha_tipo_urp"].ToString();
                    er.has_children = false;//(Convert.ToDecimal (dr["has_children"]) == 1);
                }
                return er;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ArrayList GetChildrenElement(string elementID, string childrensType)
        {
            ArrayList results = new ArrayList();
            DocsPaUtils.Query q;
            string queryString;
            string querySelector = "S_RICERCA_FIGLI_ELEMENTO_UO";
            if (childrensType.ToUpper() == "R")
                querySelector = "S_RICERCA_FIGLI_ELEMENTO_RUOLO";

            q = DocsPaUtils.InitQuery.getInstance().getQuery(querySelector);
            q.setParam("param1", elementID);
            queryString = q.getSQL();

            DataSet ds = new DataSet();
            if (!this.ExecuteQuery(out ds, queryString))
                throw new Exception(this.LastExceptionMessage);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DocsPaVO.rubrica.ElementoRubrica er = new DocsPaVO.rubrica.ElementoRubrica();
                er.codice = dr["var_cod_rubrica"].ToString();
                er.descrizione = dr["var_desc_corr"].ToString();
                er.interno = (dr["cha_tipo_ie"].ToString() == "I");
                er.tipo = dr["cha_tipo_urp"].ToString();
                er.has_children = false;
                er.systemId = dr["system_id"].ToString();
                results.Add(er);
            }

            logger.Debug(queryString);

            return results;
        }

        public ArrayList GetGerarchiaElemento(string codice, DocsPaVO.addressbook.TipoUtente tipoIE)
        {
            ArrayList results = new ArrayList();

            //Controllo se il codice è di un utente
            DocsPaUtils.Query q;
            string queryString;
            q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RICERCA_TIPO_URP");
            q.setParam("param1", codice);
            q.setParam("param2", (tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO) ? "I" : "E");
            q.setParam("param3", _user.idAmministrazione);


            queryString = q.getSQL();
            string tipoURP = "";
            logger.Debug(queryString);
            this.ExecuteScalar(out tipoURP, queryString);
            if (tipoURP.ToUpper().Equals("P"))
            {
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_GERARCHIA_RUBRICA");
                q.setParam("codice", codice);
                q.setParam("idAmm", _user.idAmministrazione);
                q.setParam("tipo", (tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO) ? "I" : "E");
                if (dbType.ToUpper() == "SQL")
                    q.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                queryString = q.getSQL();
                DataSet ds = new DataSet();
                if (!this.ExecuteQuery(out ds, "ruoli", queryString))
                    throw new Exception(this.LastExceptionMessage);
                foreach (DataRow row in ds.Tables["ruoli"].Rows)
                {
                    string[] gerarchia = System.Text.RegularExpressions.Regex.Split((string)row["gerarchia"].ToString(), ":");
                    Dictionary<int, string> ordinamento = new Dictionary<int, string>();
                    string ret = "(";
                    for (int i = 0; i < gerarchia.Length; i++)
                    {
                        ordinamento.Add(i, gerarchia[i].ToUpper());

                        ret = ret + "'" + gerarchia[i].ToUpper() + "'";
                        if (i < gerarchia.Length - 1)
                        {
                            if (i % 998 == 0 && i > 0)
                            {
                                ret = ret + ") OR VAR_COD_RUBRICA IN (";
                            }
                            else
                            {
                                ret += ", ";
                            }
                        }
                        else
                        {
                            ret += ")";
                        }
                    }


                    if (!string.IsNullOrEmpty(ret))
                    {
                        Dictionary<string, DocsPaVO.rubrica.ElementoRubrica> elementi = GetElementRubricaFromGerarchia(ret, tipoIE);
                        if (elementi != null && elementi.Count > 0)
                        {
                            for (int y = 0; y < ordinamento.Count; y++)
                            {
                                string cod = ordinamento[y];
                                results.Add(elementi[cod]);
                            }
                        }
                    }
                }
            }
            else
            {
                /*
                ArrayList sp_params = new ArrayList();
                DocsPaUtils.Data.ParameterSP res;
                //res = new DocsPaUtils.Data.ParameterSP("codes", "",4000,  DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.String);
                res = new DocsPaUtils.Data.ParameterSP("codes", "", 4000, DocsPaUtils.Data.DirectionParameter.ParamOutput, System.Data.DbType.String);
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("id_amm", _user.idAmministrazione));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("cod", codice.Replace("'", "''")));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("tipo_ie", (tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO) ? "I" : "E"));
                sp_params.Add(new DocsPaUtils.Data.ParameterSP("id_Ruolo", ""));
                sp_params.Add(res);
                this.ExecuteStoredProcedure("dpa3_get_hierarchy", sp_params, null);
                */
                string cha_tipo_ie = (tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO) ? "I" : "E";
                string query = string.Empty;
                if (dbType.ToUpper() == "SQL")
                    query = "SELECT @dbuser@.get_hierarchy(" + _user.idAmministrazione + ", '" + codice.Replace("'", "''") + "', '" + cha_tipo_ie + "','" + string.Empty + "' ) AS GERARCHIA FROM DUAL ";
                else
                    query = "SELECT get_hierarchy(" + _user.idAmministrazione + ", '" + codice.Replace("'", "''") + "', '" + cha_tipo_ie + "','" + string.Empty + "' ) AS GERARCHIA FROM DUAL ";
                DataSet ds = new DataSet();
                logger.Debug(query);
                this.ExecuteQuery(ds, query);
                string res = string.Empty;
                if (ds.Tables[0].Rows.Count != 0)
                {
                    res = ds.Tables[0].Rows[0]["GERARCHIA"].ToString();
                }

                string[] gerarchia = System.Text.RegularExpressions.Regex.Split((string)res, ":");

                foreach (string cod in gerarchia)
                    if (!string.IsNullOrEmpty(cod))
                        results.Add(GetElementoRubrica(cod, ""));
            }

            return results;
        }

        public void CheckChildrenExistence(ref DocsPaVO.rubrica.ElementoRubrica[] ers, string idAmm)
        {
            CheckChildrenExistence(ref ers, true, true, true, idAmm);
        }

        public void CheckChildrenExistence(ref DocsPaVO.rubrica.ElementoRubrica[] ers, bool checkUo, bool checkRuoli, bool checkUtenti, string idAmm)
        {

            if (ers == null || ers.Length == 0)
                return;

            DocsPaUtils.Query q;
            DataSet ds = null;
            try
            {

                string codes = "";
                //OLD
                for (int n = 0; n < ers.Length; n++)
                {

                    //old codes += String.Format("'{0}',", ers[n].codice.Replace("'", "''").ToUpper());
                    codes = String.Format("'{0}',", ers[n].codice.Replace("'", "''").ToUpper());
                    //				//NEW
                    //				for (int n = 0; n < ers.Length; n ++) 
                    //					codes += String.Format ("'{0}',", ers[n].systemID);


                    q = DocsPaUtils.InitQuery.getInstance().getQuery("S_CHECK_CHILDREN_EXISTENCE");
                    codes = codes.Substring(0, codes.Length - 1);
                    q.setParam("param1", codes);
                    if (!string.IsNullOrEmpty(idAmm) && !idAmm.Equals("0"))
                        q.setParam("idAmm", " AND ID_AMM = " + idAmm);
                    else
                        q.setParam("idAmm", "");

                    // La condizione "and null is not null"
                    // serve a definire una costante FALSE da poter usare
                    // con un operatore relazionale, altrimenti in PL/SQL questa
                    // query non funziona...
                    q.setParam("checkUo", checkUo ? "" : "and null is not null");
                    q.setParam("checkRuoli", checkRuoli ? "" : "and null is not null");
                    q.setParam("checkUtenti", checkUtenti ? "" : "and null is not null");

                    string mySql = q.getSQL();
                    logger.Debug(mySql);
                    if (ds == null) ds = new DataSet();

                    if (codes != null && codes.Trim() != "")
                        if (!this.ExecuteQuery(ds, "elementi", mySql))
                            throw new Exception(this.LastExceptionMessage);


                }
                foreach (DataRow dr in ds.Tables["elementi"].Rows)
                {
                    for (int n = 0; n < ers.Length; n++)
                        if (ers[n].codice == dr["var_cod_rubrica"].ToString())
                            ers[n].has_children = (Convert.ToInt64(dr["has_children"]) == 1);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ArrayList SearchRange(string[] codici, DocsPaVO.addressbook.TipoUtente tipoIE)
        {
            ArrayList elementi = new ArrayList();

            if (codici == null || codici.Length == 0)
                return elementi;

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RICERCA_ELEMENTI_RUBRICA");
            DataSet ds = new DataSet();

            string qry = "(id_amm=" + _user.idAmministrazione + ") AND ";
            switch (tipoIE)
            {
                case DocsPaVO.addressbook.TipoUtente.INTERNO:
                    qry += "(cha_tipo_ie='I' OR cha_tipo_ie is NULL) AND ";
                    break;

                case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                    qry += "(cha_tipo_ie='E' OR cha_tipo_ie is NULL) AND ";
                    break;
            }

            bool resultCorr = false;
            for (int n = 0; n < codici.Length; n++)
            {
                qry += " UPPER(VAR_COD_RUBRICA)='" + codici[n].ToUpper().Replace("'", "''") + "' OR ";
                //qry += " SYSTEM_ID='" + codici[n] + "') OR";
                resultCorr = true;
                /*
                //IF per le liste di distribuzione
                if(!codici[n].StartsWith("@"))
                {
                    qry += " VAR_COD_RUBRICA='" + codici[n] + "' OR ";
                    resultCorr = true;
                }
                */
            }

            if (resultCorr)
            {
                qry = qry.Substring(0, qry.Length - 3);
                q.setParam("param1", qry);

                string mySql = q.getSQL();
                logger.Debug(mySql);

                if (!this.ExecuteQuery(out ds, mySql))
                    throw new Exception(this.LastExceptionMessage);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DocsPaVO.rubrica.ElementoRubrica er = new DocsPaVO.rubrica.ElementoRubrica();
                    er.codice = dr["var_cod_rubrica"].ToString();
                    er.descrizione = dr["var_desc_corr"].ToString();
                    er.interno = (Convert.ToInt64(dr["interno"]) == 1);
                    er.tipo = dr["cha_tipo_urp"].ToString();
                    er.has_children = false;
                    er.systemId = dr["system_id"].ToString();
                    elementi.Add(er);
                }
            }

            return elementi;
        }

        public ArrayList GetRootItems(DocsPaVO.addressbook.TipoUtente tipoIE)
        {
            ArrayList elementi = new ArrayList();
            DataSet ds = new DataSet();

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_ROOT_ITEMS");
            q.setParam("id_amm", _user.idAmministrazione);
            if (tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE)
                q.setParam("tipo_ie", "");
            else
                q.setParam("tipo_ie", "cha_tipo_ie='" + ((tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO) ? "I" : "E") + "' and ");

            string mySql = q.getSQL();
            logger.Debug(mySql);

            if (!this.ExecuteQuery(out ds, mySql))
                throw new Exception(this.LastExceptionMessage);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                DocsPaVO.rubrica.ElementoRubrica er = new DocsPaVO.rubrica.ElementoRubrica();
                er.systemId = dr["system_id"].ToString();
                er.codice = dr["var_cod_rubrica"].ToString();
                er.descrizione = dr["var_desc_corr"].ToString();
                er.interno = (Convert.ToInt64(dr["interno"]) == 1);
                er.tipo = dr["cha_tipo_urp"].ToString();
                er.has_children = false;
                elementi.Add(er);
            }
            return elementi;
        }

        public bool UoIsOnRegistro(string cod_uo, string id_registro, string id_amm)
        {
            string id_uo;
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_DPA_UO_REG");
            q.setParam("cod_uo", cod_uo);
            q.setParam("id_reg", id_registro);
            q.setParam("id_amm", id_amm);

            string mySql = q.getSQL();
            logger.Debug(mySql);

            if (!ExecuteScalar(out id_uo, mySql))
                return false;
            else
                return (id_uo != null);
        }

        public string[] GetUoInterneAoo(string id_reg)
        {
            DataSet ds = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UO_IN_AOO_2");
            q.setParam("id_reg", id_reg);

            string mySql = q.getSQL();
            logger.Debug(mySql);

            if (!this.ExecuteQuery(out ds, mySql))
                throw new Exception(this.LastExceptionMessage);

            string[] res = new string[ds.Tables[0].Rows.Count];
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                res[i] = (string)ds.Tables[0].Rows[i][0].ToString();
            }
            return res;
        }

        public string[] GetUoInterneAooWithReg(string idAmm)
        {
            DataSet ds = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UO_IN_AOO_WITH_REG");
            q.setParam("param1", idAmm);
            string mySql = q.getSQL();

            logger.Debug(mySql);

            if (!this.ExecuteQuery(out ds, mySql))
                throw new Exception(this.LastExceptionMessage);

            string[] res = new string[ds.Tables[0].Rows.Count];
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                res[i] = (string)ds.Tables[0].Rows[i][0].ToString();
            }
            return res;
        }

        public string[] getUtenteInternoAOO(string idPeople, string id_reg)
        {
            DataSet ds = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("GET_UTENTE_INTERNO_AOO");
            q.setParam("param1", idPeople);
            q.setParam("param2", id_reg);

            string mySql = q.getSQL();
            logger.Debug(mySql);

            if (!this.ExecuteQuery(out ds, mySql))
                throw new Exception(this.LastExceptionMessage);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return new string[0];
            }
            else
            {
                string[] res = new string[ds.Tables[0].Rows.Count];
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    res[i] = (string)ds.Tables[0].Rows[i][0].ToString();
                }
                return res;
            }
        }

        #region VerificaDipendenzaCodRubrica

        /// <summary>
        /// Ritorna la systemId della Uo a cui appartiente il ruolo protocollatore
        /// </summary>
        /// <param name="codiceUoAppartenenza"></param>
        /// <returns></returns>
        /// 
        public string GetIdUoAppartenenza(string codiceUoAppartenenza)
        {
            string systeIdUoAppartenenza = "";
            string query = "SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE upper(VAR_COD_RUBRICA) = '" + codiceUoAppartenenza.ToUpper() + "'";
            DataSet ds = new DataSet();
            logger.Debug(query);
            this.ExecuteQuery(ds, query);
            systeIdUoAppartenenza = ds.Tables[0].Rows[0][0].ToString();
            return systeIdUoAppartenenza;
        }

        //Metodi che permettono la verifica della dipendeza (Padre-Figlio) di due codici rubrica
        //Il primo parametro nella firma del metodo è sempre un codice di una UO,
        //il secondo puo' essere un codice di un Utente, un Ruolo o una UO
        public bool verificaDipendezaCodRubrica(string codiceUoAppartenenza, string codiceRubrica)
        {
            bool result = false;
            string query = "SELECT SYSTEM_ID FROM DPA_CORR_GLOBALI WHERE upper(VAR_COD_RUBRICA) = '" + codiceUoAppartenenza.ToUpper() + "'";
            DataSet ds = new DataSet();
            logger.Debug(query);
            this.ExecuteQuery(ds, query);
            string systeIdUoAppartenenza = ds.Tables[0].Rows[0][0].ToString();

            query = "SELECT SYSTEM_ID,CHA_TIPO_URP FROM DPA_CORR_GLOBALI WHERE upper(VAR_COD_RUBRICA) = '" + codiceRubrica.ToUpper() + "'";
            ds = new DataSet();
            logger.Debug(query);
            this.ExecuteQuery(ds, query);
            if (ds.Tables[0].Rows.Count != 0)
            {
                string systemId = ds.Tables[0].Rows[0][0].ToString();
                string chaTipoUrp = ds.Tables[0].Rows[0][1].ToString();

                if (chaTipoUrp == "U")
                    result = verificaDipendenzaUo(systeIdUoAppartenenza, systemId);
                if (chaTipoUrp == "R")
                    result = verificaDipendenzaRuolo(systeIdUoAppartenenza, systemId);
                if (chaTipoUrp == "P")
                    result = verificaDipendenzaUtente(systeIdUoAppartenenza, systemId);
            }
            return result;
        }

        //Metodi che permettono la verifica della dipendeza (Padre-Figlio) di due codici rubrica
        //Il primo parametro nella firma del metodo è sempre un codice di una UO,
        //il secondo puo' essere un codice di un Utente, un Ruolo o una UO
        public bool verificaDipendenzaCodRubrica(string systeIdUoAppartenenza, string codiceRubrica)
        {

            string query = "";
            bool result = false;
            query = "SELECT SYSTEM_ID,CHA_TIPO_URP FROM DPA_CORR_GLOBALI WHERE upper(VAR_COD_RUBRICA) = '" + codiceRubrica.ToUpper() + "'";
            DataSet ds = new DataSet();
            logger.Debug(query);
            this.ExecuteQuery(ds, query);
            if (ds.Tables[0].Rows.Count != 0)
            {
                string systemId = ds.Tables[0].Rows[0][0].ToString();
                string chaTipoUrp = ds.Tables[0].Rows[0][1].ToString();

                if (chaTipoUrp == "U")
                    result = verificaDipendenzaUo(systeIdUoAppartenenza, systemId);
                if (chaTipoUrp == "R")
                    result = verificaDipendenzaRuolo(systeIdUoAppartenenza, systemId);
                if (chaTipoUrp == "P")
                    result = verificaDipendenzaUtente(systeIdUoAppartenenza, systemId);
            }
            return result;
        }

        private bool verificaDipendenzaUo(string systeIdUoAppartenenza, string systemId)
        {
            bool result = false;
            while (true)
            {
                string query = "SELECT ID_PARENT FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID = " + systemId;
                DataSet ds = new DataSet();
                this.ExecuteQuery(ds, query);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0].ToString() == systeIdUoAppartenenza)
                    {
                        result = true;
                        break;
                    }
                    else
                    {
                        systemId = ds.Tables[0].Rows[0][0].ToString();
                    }
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        private bool verificaDipendenzaRuolo(string systeIdUoAppartenenza, string systemId)
        {
            bool result = false;
            string query = "SELECT ID_UO FROM DPA_CORR_GLOBALI WHERE SYSTEM_ID = " + systemId;
            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, query);
            if (ds.Tables[0].Rows.Count > 0)
                result = verificaDipendenzaUo(systeIdUoAppartenenza, ds.Tables[0].Rows[0][0].ToString());
            return result;
        }

        private bool verificaDipendenzaUtente(string systeIdUoAppartenenza, string systemId)
        {
            bool result = false;
            string query = "select DISTINCT id_uo from peoplegroups,groups,dpa_corr_globali " +
                "where " +
                "peoplegroups.people_system_id = (select id_people from dpa_corr_globali where system_id = " + systemId + ") " +
                "and " +
                "peoplegroups.groups_system_id = groups.system_id " +
                "and " +
                "UPPER(groups.group_id) = UPPER(var_cod_rubrica) and peoplegroups.dta_fine is null";
            DataSet ds = new DataSet();
            this.ExecuteQuery(ds, query);
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    result = verificaDipendenzaUo(systeIdUoAppartenenza, ds.Tables[0].Rows[i][0].ToString());
                    if (result)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        #endregion VerificaDipendezaCodRubrica

        public DocsPaVO.rubrica.ElementoRubrica GetElementoRubricaSimpleBySystemId(string systemId)
        {
            DocsPaVO.rubrica.ElementoRubrica er = null;
            string srch_condition = "";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RICERCA_ELEMENTO_RUBRICA_SIMPLE");
            try
            {
                srch_condition = "system_id = " + systemId;
                srch_condition += String.Format(" and id_amm='{0}'", _user.idAmministrazione);
                q.setParam("param1", srch_condition);

                string mySql = q.getSQL();
                logger.Debug(mySql);

                DataSet ds = new DataSet();
                if (!this.ExecuteQuery(out ds, "corrispondenti", mySql))
                    throw new Exception(this.LastExceptionMessage);

                if (ds.Tables["corrispondenti"].Rows.Count != 0)
                {
                    DataRow dr = ds.Tables["corrispondenti"].Rows[0];
                    er = new DocsPaVO.rubrica.ElementoRubrica();
                    //28 marzo 2008
                    er.codiceRegistro = dr["cod_reg"].ToString();
                    er.systemId = dr["system_id"].ToString();
                    er.codice = dr["var_cod_rubrica"].ToString();
                    er.descrizione = dr["var_desc_corr"].ToString();
                    er.interno = (Convert.ToDecimal(dr["interno"]) == 1);
                    er.tipo = dr["cha_tipo_urp"].ToString();
                    er.has_children = false;//(Convert.ToDecimal (dr["has_children"]) == 1);
                }
                return er;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private DocsPaVO.utente.Registro GetRegistro(string idRegistro)
        {
            DocsPaVO.utente.Registro reg = null;
            logger.Debug("getRegistro");
            if (!(idRegistro != null && !idRegistro.Equals("")))
            {
                return null;
            }

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_DPAElRegistri");
            string firstParam = "SYSTEM_ID, VAR_CODICE, CHA_STATO, ID_AMM, VAR_DESC_REGISTRO, " +
                " VAR_EMAIL_REGISTRO, " + DocsPaDbManagement.Functions.Functions.ToChar("DTA_OPEN", false) +
                " AS DTA_OPEN, " + DocsPaDbManagement.Functions.Functions.ToChar("DTA_CLOSE", false) +
                " AS DTA_CLOSE, " + DocsPaDbManagement.Functions.Functions.ToChar("DTA_ULTIMO_PROTO", false) +
                " AS DTA_ULTIMO_PROTO, CHA_AUTO_INTEROP, CHA_RF ";
            q.setParam("param1", firstParam);
            q.setParam("param2", "SYSTEM_ID=" + idRegistro);
            string queryString = q.getSQL();
            logger.Debug(queryString);

            DataSet dataSet;
            this.ExecuteQuery(out dataSet, queryString);

            if (dataSet.Tables[0].Rows.Count > 0)
            {
                DataRow row = dataSet.Tables[0].Rows[0];

                reg = new DocsPaVO.utente.Registro();

                reg.systemId = row[0].ToString();
                reg.codRegistro = row[1].ToString();
                reg.stato = row[2].ToString();
                reg.idAmministrazione = row[3].ToString();
                reg.descrizione = row[4].ToString();
                reg.email = row[5].ToString();
                reg.dataApertura = row[6].ToString().Trim();
                reg.dataChiusura = row[7].ToString().Trim();
                reg.dataUltimoProtocollo = row[8].ToString();
                reg.autoInterop = row[9].ToString();
                reg.chaRF = row[10].ToString();
            }
            dataSet.Dispose();

            if (reg != null)
            {
                // Reperimento dati relativi all'amministrazione legata al registro
                string commandText = "SELECT A.SYSTEM_ID,A.VAR_CODICE_AMM FROM DPA_AMMINISTRA A,DPA_EL_REGISTRI R WHERE R.SYSTEM_ID=@idRegistro@ AND R.ID_AMM=A.SYSTEM_ID";

                q = new DocsPaUtils.Query(commandText);
                q.setParam("idRegistro", idRegistro);
                commandText = q.getSQL();

                try
                {
                    using (IDataReader reader = this.ExecuteReader(commandText))
                    {
                        if (reader == null)
                        {
                            throw new Exception("Errore in GetRegistro");
                        }
                        if (reader.Read())
                        {
                            reg.codice = reader.GetInt32(reader.GetOrdinal("SYSTEM_ID")).ToString();
                            reg.codAmministrazione = reader.GetString(reader.GetOrdinal("VAR_CODICE_AMM"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Debug(ex.Message);
                    reg = null;
                }
                finally
                {
                    this.CloseConnection();
                }
            }

            return reg;
        }

        public System.Collections.ArrayList GetListaCorrExport(DocsPaVO.utente.InfoUtente infoUtente, string registri)
        {
            logger.Debug("GetListaCorrExport");
            ArrayList lista = new ArrayList();
            try
            {
                DocsPaUtils.Query q;
                DataSet dataSet;
                q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RICERCA_ALL_CORRISPONDENTI");
                string paramRegistri = "";
                string paramIdAmm = "";
                if (!string.IsNullOrEmpty(registri))
                {
                    paramRegistri = " AND ( r.system_id in (" + registri + ") OR r.system_id IS NULL)";
                }
                q.setParam("registri", paramRegistri);
                if (!string.IsNullOrEmpty(infoUtente.idAmministrazione))
                {
                    q.setParam("idamm", infoUtente.idAmministrazione);
                }
                else
                    q.setParam("idamm", "");


                string sql = q.getSQL();
                logger.Debug(sql);
                this.ExecuteQuery(out dataSet, "CORRISPONDENTI", sql);
                foreach (DataRow dataRow in dataSet.Tables["CORRISPONDENTI"].Rows)
                {
                    DocsPaVO.utente.DatiModificaCorr corr = new DocsPaVO.utente.DatiModificaCorr();
                    corr.codice = dataRow["var_codice"].ToString();
                    corr.codRubrica = dataRow["var_cod_rubrica"].ToString();
                    corr.codiceAmm = dataRow["var_codice_amm"].ToString();
                    corr.codiceAoo = dataRow["var_codice_aoo"].ToString();
                    corr.tipoCorrispondente = dataRow["cha_tipo_urp"].ToString();
                    corr.descCorr = dataRow["var_desc_corr"].ToString();
                    corr.cognome = dataRow["var_cognome"].ToString();
                    corr.nome = dataRow["var_nome"].ToString();
                    corr.indirizzo = dataRow["var_indirizzo"].ToString();
                    corr.cap = dataRow["var_cap"].ToString();
                    corr.citta = dataRow["var_citta"].ToString();
                    corr.provincia = dataRow["var_provincia"].ToString();
                    corr.localita = dataRow["var_localita"].ToString();
                    corr.nazione = dataRow["var_nazione"].ToString();
                    corr.codFiscale = dataRow["var_cod_fisc"].ToString();
                    corr.telefono = dataRow["var_telefono"].ToString();
                    corr.telefono2 = dataRow["var_telefono2"].ToString();
                    corr.fax = dataRow["var_fax"].ToString();
                    corr.email = dataRow["var_email"].ToString();
                    corr.note = dataRow["var_note"].ToString();
                    corr.partitaIva = dataRow["var_cod_pi"].ToString();
                    corr.descrizioneCanalePreferenziale = dataRow["description"].ToString();
                    if (corr == null)
                    {
                        logger.Debug("corr null");
                    }
                    else
                    {
                        logger.Debug("corr pieno");
                    }
                    lista.Add(corr);

                }
                dataSet.Dispose();
            }
            catch (Exception e)
            {
                logger.Debug(e.Message);
                //this.CloseConnection();
                logger.Debug("Errore nella gestione dei corrispondenti (Query - GetListaCorrExport)", e);
                throw new Exception("F_System");
            }
            return lista;
        }

        public ArrayList GetCorrespondentsByFilter(DocsPaVO.rubrica.ParametriRicercaRubrica qc)
        {
            string qry = "";
            ArrayList elementi = new ArrayList();
            DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
            bool no_filtro_aoo = obj.isFiltroAooEnabled();
            string userDb = string.Empty;
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
            if (dbType.ToUpper() == "SQL")
                userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession() + ".";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RICERCA_ALL_CORRISPONDENTI_BY_FILTER");
            DataSet ds = new DataSet();
            try
            {
                if (dbType.ToUpper() == "SQL")
                    q.setParam("dbuser",  DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                ArrayList sp_params = new ArrayList();
                if (qc.parent != null && qc.parent != "")
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("id_amm", Convert.ToInt32(_user.idAmministrazione), 0, DirectionParameter.ParamInput, System.Data.DbType.Decimal));

                    switch (qc.tipoIE)
                    {
                        case DocsPaVO.addressbook.TipoUtente.INTERNO:
                        default:
                            sp_params.Add(new DocsPaUtils.Data.ParameterSP("cha_tipo_ie", "I", 1, DirectionParameter.ParamInput, System.Data.DbType.String));
                            break;

                        case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                            sp_params.Add(new DocsPaUtils.Data.ParameterSP("cha_tipo_ie", "E", 1, DirectionParameter.ParamInput, System.Data.DbType.String));
                            break;
                    }
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("var_cod_rubrica", qc.parent, 128, DirectionParameter.ParamInput, System.Data.DbType.String));

                    int corr_types = 0;
                    corr_types += (qc.doUo ? 1 : 0);
                    corr_types += (qc.doRuoli ? 2 : 0);
                    corr_types += (qc.doUtenti ? 4 : 0);
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("corr_types", corr_types, 0, DirectionParameter.ParamInput, System.Data.DbType.Decimal));

                    if (this.ExecuteStoredProcedure("dpa3_get_children", sp_params, ds) != 1)
                        throw new Exception(this.LastExceptionMessage);
                }
                else
                {
                    // Non sono estratti i corrispondenti con tipologia "C", 
                    // ovvero gli elementi inseriti da rubrica comune

                    qry += DocsPaDbManagement.Functions.Functions.getNVL("a.cha_tipo_corr", "'X'") + " != 'C' AND  ( (a.id_amm=" + _user.idAmministrazione + ") OR a.id_amm is null) AND ";
                    //if (qc.caller.filtroRegistroPerRicerca != null && qc.caller.filtroRegistroPerRicerca != "") {
                    //    qry += "id_registro in (" + qc.caller.filtroRegistroPerRicerca + ") AND ";
                    //}
                    switch (qc.tipoIE)
                    {
                        case DocsPaVO.addressbook.TipoUtente.INTERNO:
                            if (qc.doListe || qc.doRF)
                                qry += "(a.cha_tipo_ie='I' OR a.CHA_TIPO_IE IS NULL) AND ";
                            else
                                qry += "(a.cha_tipo_ie='I') AND ";
                            break;

                        case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                            if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE)
                                || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ESTERNI_AMM)
                                //modifica del 13/05/2009
                                || qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO
                                //fine modifica del 13/05/2009
                                )
                            {
                                qry += "(a.cha_tipo_ie='E') AND ";
                            }
                            break;

                        case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                            break;
                    }

                    if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                        qry += String.Format("(upper(a.var_cod_rubrica) = upper('{0}')) AND ", qc.codice.Replace("'", "''"));
                    else if (qc.codice != null && qc.codice != "")
                        qry += String.Format("(upper(a.var_cod_rubrica) LIKE upper('%{0}%')) AND ", qc.codice.Replace("'", "''"));

                    if (qc.descrizione != null && qc.descrizione != "")
                        qry += String.Format("(upper(a.var_desc_corr) LIKE upper('%{0}%')) AND ", qc.descrizione.Replace("'", "''"));
                    //qry = ricercaFullText("DESCRIZIONE", qry, qc.descrizione) + " AND ";

                    if (qc.citta != null && qc.citta != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_citta) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.citta.Replace("'", "''"));
                    }

                    if (qc.localita != null && qc.localita != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_localita) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.localita.Replace("'", "''"));
                    }

                    //if (qc.cf_piva != null && qc.cf_piva != "")
                    //{
                    //    qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_cod_fiscale) = upper('{0}')) and id_corr_globali = a.system_id)) AND ", qc.cf_piva.Replace("'", "''"));
                    //}

                    if (!string.IsNullOrEmpty(qc.email))
                    {
                        qry += String.Format("(upper(a.VAR_EMAIL) LIKE upper('%{0}%')) AND ", qc.email.Replace("'", "''"));
                    }

                    if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_cod_fisc) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.codiceFiscale.Replace("'", "''"));
                    }

                    if (qc.partitaIva != null && qc.partitaIva != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_cod_pi) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.partitaIva.Replace("'", "''"));
                    }

                    //filtro per system_ID
                    if (qc.systemId != null && qc.systemId != "")
                    {
                        qry += String.Format("a.system_id = " + qc.systemId + " AND ");
                    }

                    //                    if (qc.doUo || qc.doRuoli || qc.doUtenti || qc.doListe || qc.doRF)
                    if (qc.doUo || qc.doRuoli || qc.doUtenti || qc.doRF)
                    {
                        qry += "a.cha_tipo_urp in (";
                        if (qc.doUo)
                            qry += "'U',";

                        if (qc.doRuoli)
                            qry += "'R',";

                        if (qc.doUtenti)
                            qry += "'P',";

                        //if (qc.doListe)
                        //    qry += "'L',";

                        if (qc.doRF)
                            qry += "'F',";

                        if (qry.EndsWith(","))
                            qry = qry.Substring(0, qry.Length - 1) + ")";
                    }
                    else
                    {
                        if (!qc.doListe)
                            return elementi;
                    }

                    if (qry.EndsWith(" AND "))
                        qry = qry.Substring(0, qry.Length - " AND ".Length);


                    //La condizione sui ruoli disabilitati (DTA_FINE valorizzata) non viene considerata per calltype di ricerca
                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CREATOR ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_OWNER_AUTHOR ||
                        //IACOZZILLI GIORDANO 25/06/2013
                        //Aggiungo il calltype per il deposito:
                         qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEP_OSITO ||
                        //FINE
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_TODOLIST ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_NO_FILTRI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORRISPONDENTE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                        )
                    {
                        //qry += " AND a.dta_fine IS NULL";
                    }
                    else
                    {
                        qry += " AND a.dta_fine IS NULL";
                    }

                    if (!qc.extSystems && qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MODELLO_TRASM)
                    {
                        qry += " AND a.cha_system_role != '1'";
                    }

                    if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORRISPONDENTE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO)
                        && qc.caller != null && !string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca))
                    {
                        qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") or a.id_registro is null)";
                    }

                    #region codice commentato
                    // #### NUOVO GESTIONE PER PITRE ####
                    // RICERCA DEI I CORRISPONDENTI GLOBALI DA DOC PROTOCOLLO
                    //if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE
                    //    &&
                    //    (qc.caller.IdRegistro != null || qc.caller.IdRegistro != string.Empty)
                    //    &&
                    //    (qc.doUo || qc.doRuoli || qc.doUtenti)
                    //    )
                    //{
                    //   qry += " AND (id_registro is null ";

                    //   ////dato un registro, ricerco anche tra gli rf ad esso associati che il ruolo può vedere
                    //    Utenti ut = new Utenti();
                    //    ArrayList listaRf = ut.GetListaRegistriRfRuolo(qc.caller.IdRuolo, "", "");

                    //    if (listaRf != null && listaRf.Count > 0)
                    //   {
                    //       foreach (DocsPaVO.utente.Registro reg in listaRf)
                    //        {
                    //            qry += " or id_registro = " + reg.systemId;
                    //        }
                    //    }
                    //    qry += " )";
                    //}

                    ////CASO in cui si è selezionata la voce Esterni AOO.
                    ////in tal caso ricerco solamente tra 
                    //if (
                    //    (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN
                    //    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
                    //    || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO)
                    //    &&
                    //      (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO)
                    //    &&
                    //    (qc.caller.IdRegistro != null || qc.caller.IdRegistro != string.Empty)
                    //    &&
                    //    (qc.doUo || qc.doRuoli || qc.doUtenti)
                    //    )
                    //{
                    //    qry += " AND CHA_TIPO_IE = 'I'";
                    //}
                    #endregion

                    if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INGRESSO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_ESTERNI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                        )
                        &&
                        (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE || qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO)
                        &&
                        (qc.caller.IdRegistro != null || qc.caller.IdRegistro != string.Empty)
                        &&
                        (qc.doUo || qc.doRuoli || qc.doUtenti)
                        )
                    {
                        //se è un solo registro verifico se è un RF
                        bool cha_rf = false;
                        if (qc.caller.filtroRegistroPerRicerca.IndexOf(",") == -1)
                        {
                            DocsPaVO.utente.Registro reg = GetRegistro(qc.caller.filtroRegistroPerRicerca);
                            if (reg != null && reg.chaRF == "1")
                                cha_rf = true;
                        }
                        #region codice commentato
                        //qry += " AND (id_registro IN (" + qc.caller.IdRegistro + ") or id_registro is null)";
                        //MODIFICA BUG: non carica valori in rubrica per le liste di distribuzione --> registro null
                        //qry += " AND (id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") ";
                        ////se seleziono un RF non deve cercare per id_registro = null
                        //if (cha_rf)
                        //{
                        //    qry += ")";
                        //}
                        //else
                        //{
                        //    qry += " or id_registro is null)";
                        //}
                        #endregion
                        //  if (!no_filtro_aoo)
                        //   {
                        if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                            qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") or a.id_registro is null)";
                        if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && cha_rf)
                            qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + "))";
                        if (string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                            qry += " AND a.id_registro is null";
                        //  }
                    }


                    // ###############     MODELLI DI TRASMISSIONE    #################
                    // se:		è stata chiamata da modelli di trasmissione o dalla funzionalità trova e sostituisci
                    // allora:	la query deve filtrare anche per registro		
                    if (
                        (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MODELLO_TRASM ||
                            qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_REPLACE_ROLE ||
                            qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE)
                        &&
                        (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO)
                        )
                    {
                        if (qc.caller.IdRegistro != null && qc.caller.IdRegistro != string.Empty)
                        {
                            q.setParam("param2", ",DPA_L_RUOLO_REG ");
                            qry += " AND DPA_L_RUOLO_REG.ID_RUOLO_IN_UO = A.SYSTEM_ID AND " +
                                " DPA_L_RUOLO_REG.id_registro = " + qc.caller.IdRegistro;
                        }
                        else
                        {
                            q.setParam("param2", ",DPA_L_RUOLO_REG ");
                            qry += " AND DPA_L_RUOLO_REG.ID_RUOLO_IN_UO = A.SYSTEM_ID ";
                        }
                    }

                    //############### Ruolo Interoperabilità nomail ####################
                    if (
                        (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_REG_NOMAIL
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_RESP_REG)
                        &&
                        (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO)
                        &&
                        (qc.caller.IdRegistro != null && qc.caller.IdRegistro != string.Empty)
                        )
                    {
                        q.setParam("param2", ",DPA_L_RUOLO_REG ");
                        qry += " AND DPA_L_RUOLO_REG.ID_RUOLO_IN_UO = A.SYSTEM_ID AND " +
                            " DPA_L_RUOLO_REG.id_registro = " + qc.caller.IdRegistro;
                    }

                    if (qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_REG_NOMAIL &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_RESP_REG &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MODELLO_TRASM &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_REPLACE_ROLE &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE)
                    {
                        q.setParam("param2", "");
                    }

                    // Se si usa la rubrica da GESTIONE RUBRICA vengono restituiti solamente
                    //corrispondenti esterni all'amministrazione creati sul registri visibili al ruolo corrente
                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ESTERNI_AMM
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA)
                       || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST)
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO)
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO)
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_NO_FILTRI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_NO_UO)
                    {
                        //se è un solo registro verifico se è un RF
                        bool cha_rf = false;
                        if (qc.caller != null && !string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && qc.caller.filtroRegistroPerRicerca.IndexOf(",") == -1)
                        {
                            DocsPaVO.utente.Registro reg = GetRegistro(qc.caller.filtroRegistroPerRicerca);
                            if (reg != null && reg.chaRF == "1")
                                cha_rf = true;
                        }

                        #region codice commentato
                        //qry += " AND (id_registro IN (" + qc.caller.IdRegistro + ") or id_registro is null)";

                        //MODIFICA BUG: non carica valori in rubrica per le liste di distribuzione --> registro null
                        //qry += " AND (id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") ";
                        ////se seleziono un RF non deve cercare per id_registro = null
                        //if (cha_rf)
                        //{
                        //    qry += ")";
                        //}
                        //else
                        //{
                        //    qry += " or id_registro is null)";
                        //}
                        #endregion
                        if (!no_filtro_aoo)
                        {
                            if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                                qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") or a.id_registro is null)";
                            if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && cha_rf)
                                qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + "))";
                            if (string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                                qry += " AND a.id_registro is null";
                        }
                    }
                    #region codice commentato
                    //sabrina: liste di distribuzione
                    //corrispondenti esterni all'amministrazione creati sui registri visibili al ruolo corrente

                    //if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE)
                    //{
                    //    if (qc.caller != null && (qc.caller.IdRegistro != null && !qc.caller.IdRegistro.Equals("")))
                    //        qry += " AND (ID_REGISTRO " + qc.caller.IdRegistro + " OR ID_REGISTRO IS NULL)";
                    //}
                    #endregion

                    //####### Liste di distribuzione ######
                    if (qc.doListe)
                    {
                        //E' selezionata solo la ricerca delle liste quindi niente UNION
                        if (!qc.doUo && !qc.doRuoli && !qc.doUtenti && !qc.doRF)
                        {
                            qry = "(a.id_amm='" + _user.idAmministrazione + "') AND " +
                                    "(a.cha_tipo_urp in ('L'))";
                            if ((qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals("")) || (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals("")))
                            {
                                qry += " AND (";
                                bool res = false;
                                // PALUMBO: intervento per reperire le liste utente
                                //if (qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals(""))
                                if (qc.caller != null && qc.caller.IdPeople != null && !qc.caller.IdPeople.Equals(""))
                                {
                                    //qry += "(a.id_people_liste=" + qc.caller.IdUtente + " AND a.id_gruppo_liste is null) ";
                                    qry += "(a.id_people_liste=" + qc.caller.IdPeople + " AND a.id_gruppo_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                if (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals(""))
                                {
                                    qry += " (a.id_gruppo_liste=" + _user.idGruppo + " AND a.id_people_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                qry += " (a.id_people_liste is null AND a.id_gruppo_liste is null)) AND " +
                                        "a.dta_fine IS NULL ";
                            }
                        }
                        //E' una ricerca combinata liste con utenti, ruoli o uffici
                        else
                        {
                            qry += " UNION ";

                            qry += " SELECT /*+index (a)*/ " +
                                "a.var_cod_rubrica, " +
                                "a.var_desc_corr, " +
                                "(CASE WHEN a.cha_tipo_ie = 'I' THEN 1 ELSE 0 END) AS interno, a.cha_tipo_urp, a.system_id, " + userDb + "getcodreg (a.id_registro) cod_reg_rf, a.id_registro, a.CHA_DISABLED_TRASM, a.DTA_FINE, a.VAR_NOME, a.VAR_COGNOME, dett.var_cod_fisc as VAR_COD_FISC, dett.var_cod_pi as VAR_COD_PI,a.ID_PEOPLE, " +
                                "a.var_codice_amm, a.var_codice_aoo, dett.var_indirizzo,  dett.var_cap, dett.var_citta, dett.var_provincia,dett.var_localita, dett.var_nazione, dett.var_telefono, dett.var_telefono2, dett.Var_Fax, Id_Old, MAIL_E_NOTE_CORR_ESTERNI(a.system_id) as VAR_EMAIL, dett.Var_Note, DOCUMENTTYPES.DESCRIPTION " +
                                "FROM dpa_corr_globali a LEFT JOIN dpa_dett_globali dett ON dett.id_corr_globali = a.system_id LEFT JOIN DPA_T_CANALE_CORR ON a.system_id = DPA_T_CANALE_CORR.ID_CORR_GLOBALE LEFT JOIN DOCUMENTTYPES ON DPA_T_CANALE_CORR.ID_DOCUMENTTYPE = DOCUMENTTYPES.SYSTEM_ID " +
                                "WHERE " +
                                "(a.id_amm='" + _user.idAmministrazione + "') AND " +
                                "(a.cha_tipo_urp in ('L'))";
                            if ((qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals("")) || (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals("")))
                            {
                                qry += " AND (";
                                bool res = false;
                                // PALUMBO: intervento per reperire le liste utente
                                //if (qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals(""))
                                if (qc.caller != null && qc.caller.IdPeople != null && !qc.caller.IdPeople.Equals(""))
                                {
                                    //qry += "(a.id_people_liste=" + qc.caller.IdUtente + " AND a.id_gruppo_liste is null) ";
                                    qry += "(a.id_people_liste=" + qc.caller.IdPeople + " AND a.id_gruppo_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                if (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals(""))
                                {
                                    qry += " (a.id_gruppo_liste=" + _user.idGruppo + " AND a.id_people_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                qry += " (a.id_people_liste is null AND a.id_gruppo_liste is null))";
                            }

                            //"((id_people_liste="+qc.caller.IdUtente+" AND id_gruppo_liste is null) "+
                            //"OR (id_gruppo_liste=" + _user.idGruppo + " AND id_people_liste is null) "+
                            //"OR (id_people_liste is null AND id_gruppo_liste is null))";
                            if (qc.localita != null && qc.localita != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_localita) LIKE upper('%{0}%')) and id_corr_globali = a.system_id))", qc.localita.Replace("'", "''"));
                            }

                            //if (qc.cf_piva != null && qc.cf_piva != "")
                            //{
                            //    qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fiscale) = upper('{0}')) and id_corr_globali = a.system_id)) ", qc.cf_piva.Replace("'", "''"));
                            //}

                            if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fisc) LIKE upper('%{0}%')) and id_corr_globali = a.system_id))", qc.codiceFiscale.Replace("'", "''"));
                            }

                            if (qc.partitaIva != null && qc.partitaIva != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_pi) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) ", qc.partitaIva.Replace("'", "''"));
                            }

                            if (!string.IsNullOrEmpty(qc.email))
                            {
                                qry += String.Format(" AND (upper(a.VAR_EMAIL) LIKE upper('%{0}%')) ", qc.email.Replace("'", "''"));
                            }
                        }

                        //Verifico eventuali condizioni ulteriori per la ricerca. Codice-Descrizione-Registro
                        //Codice
                        if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                            qry += String.Format(" AND (upper(a.var_cod_rubrica) = upper('{0}')) ", qc.codice.Replace("'", "''"));
                        else if (qc.codice != null && qc.codice != "")
                            qry += String.Format(" AND (upper(a.var_cod_rubrica) LIKE upper('%{0}%')) ", qc.codice.Replace("'", "''"));

                        //Descrizione
                        if (qc.descrizione != null && qc.descrizione != "")
                        {
                            qry += String.Format(" AND (upper(a.var_desc_corr) LIKE upper('%{0}%')) ", qc.descrizione.Replace("'", "''"));
                            //qry += " AND ";
                            //qry = ricercaFullText("DESCRIZIONE", qry, qc.descrizione);
                        }
                        //Registro - Eventualmente da implementare

                    }

                    #region RF
                    if (qc.doRF)
                    {
                        //E' selezionata solo la ricerca degli RF quindi niente UNION
                        if (!qc.doUo && !qc.doRuoli && !qc.doUtenti && !qc.doListe)
                        {
                            if (qry.IndexOf("a.cha_tipo_urp in ('F')") == -1)
                                qry += " ( (a.id_amm=" + _user.idAmministrazione + ") OR a.id_amm is null) AND cha_tipo_urp = 'F' ";
                        }
                        //E' una ricerca combinata RF con utenti, ruoli, uffici o liste
                        else
                        {
                            qry += " UNION ";

                            qry += " SELECT /*+index (a)*/ " +
                                "a.var_cod_rubrica, " +
                                "a.var_desc_corr, " +
                                "(CASE WHEN a.cha_tipo_ie = 'I' THEN 1 ELSE 0 END) AS interno, a.cha_tipo_urp, a.system_id , " + userDb + "getcodreg (a.id_registro) cod_reg_rf, a.id_registro, a.CHA_DISABLED_TRASM, a.DTA_FINE, a.VAR_NOME, a.VAR_COGNOME, dett.var_cod_fisc as VAR_COD_FISC, dett.var_cod_pi as VAR_COD_PI, a.ID_PEOPLE, " +
                                "  a.var_codice_amm, a.var_codice_aoo, dett.var_indirizzo,  dett.var_cap, dett.var_citta, dett.var_provincia,dett.var_localita, dett.var_nazione, dett.var_telefono, dett.var_telefono2, dett.Var_Fax, Id_Old, MAIL_E_NOTE_CORR_ESTERNI(a.system_id) as VAR_EMAIL, dett.Var_Note, DOCUMENTTYPES.DESCRIPTION " +
                                "FROM dpa_corr_globali a LEFT JOIN dpa_dett_globali dett ON dett.id_corr_globali = a.system_id LEFT JOIN DPA_T_CANALE_CORR ON a.system_id = DPA_T_CANALE_CORR.ID_CORR_GLOBALE LEFT JOIN DOCUMENTTYPES ON DPA_T_CANALE_CORR.ID_DOCUMENTTYPE = DOCUMENTTYPES.SYSTEM_ID " +
                                "WHERE " +
                                "(a.id_amm='" + _user.idAmministrazione + "') AND a.cha_tipo_urp = 'F' ";

                            //Verifico eventuali condizioni ulteriori per la ricerca. Codice-Descrizione-Registro
                            //Codice
                            if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                                qry += String.Format(" AND (upper(a.var_cod_rubrica) = upper('{0}')) ", qc.codice.Replace("'", "''"));
                            else if (qc.codice != null && qc.codice != "")
                                qry += String.Format(" AND (upper(a.var_cod_rubrica) LIKE upper('%{0}%')) ", qc.codice.Replace("'", "''"));

                            //Descrizione
                            if (qc.descrizione != null && qc.descrizione != "")
                            {
                                qry += String.Format(" AND (upper(a.var_desc_corr) LIKE upper('%{0}%')) ", qc.descrizione.Replace("'", "''"));
                                //qry += " AND ";
                                //qry = ricercaFullText("DESCRIZIONE", qry, qc.descrizione);
                            }

                            //Registro - Eventualmente da implementare

                            if (qc.localita != null && qc.localita != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_localita) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) ", qc.localita.Replace("'", "''"));
                            }

                            //if (qc.cf_piva != null && qc.cf_piva != "")
                            //{
                            //    qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fiscale) = upper('{0}')) and id_corr_globali = a.system_id)) ", qc.cf_piva.Replace("'", "''"));
                            //}


                            if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fisc) LIKE upper('%{0}%')) and id_corr_globali = a.system_id))", qc.codiceFiscale.Replace("'", "''"));
                            }

                            if (qc.partitaIva != null && qc.partitaIva != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_pi) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) ", qc.partitaIva.Replace("'", "''"));
                            }

                            if (!string.IsNullOrEmpty(qc.email))
                            {
                                qry += String.Format(" AND (upper(a.VAR_EMAIL) LIKE upper('%{0}%')) ", qc.email.Replace("'", "''"));
                            }

                        }

                        //if (qc.caller.filtroRegistroPerRicerca != null && qc.caller.filtroRegistroPerRicerca != "")
                        //{
                        //    qry += " and ID_REGISTRO in (" + qc.caller.filtroRegistroPerRicerca + ")";
                        //}
                    }
                    #endregion

                    bool nessun_sottoposto = false;

                    //INSERITO PER TROVARE SOLTANTO I RUOLI SOTTOPOSTI
                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO)
                    {
                        ArrayList listaRuoliInf;
                        DataSet dsTempRuoli = new DataSet();
                        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                        DocsPaVO.utente.Ruolo ruoloGlobale = utenti.getRuoloById(qc.caller.IdRuolo);
                        DocsPaVO.utente.Ruolo mioRuolo = utenti.GetRuoloByIdGruppo(ruoloGlobale.idGruppo);
                        DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                        DocsPaVO.trasmissione.TipoOggetto tipo = new DocsPaVO.trasmissione.TipoOggetto();
                        listaRuoliInf = gerarchia.getGerarchiaInf(mioRuolo, null, null, tipo);
                        listaRuoliInf.Add(mioRuolo);

                        string addConstant = null;

                        if (listaRuoliInf.Count != 0)
                        {
                            addConstant = " AND A.SYSTEM_ID IN (";

                            for (int i = 0; i < listaRuoliInf.Count; i++)
                            {
                                addConstant = addConstant + ((DocsPaVO.utente.Ruolo)listaRuoliInf[i]).systemId;
                                if (i < listaRuoliInf.Count - 1)
                                {
                                    if (i % 998 == 0 && i > 0)
                                    {

                                    }
                                    else
                                    {
                                        addConstant += ", ";
                                    }
                                }
                                else
                                {
                                    addConstant += ")";
                                }
                            }
                            qry += addConstant;
                        }
                        else
                        {
                            nessun_sottoposto = true;
                        }
                    }

                    qry += " order by cha_tipo_urp desc, var_desc_corr";

                    q.setParam("param1", qry);

                    string mySql = q.getSQL();
                    logger.Debug(mySql);

                    if (nessun_sottoposto == true && qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO)
                    {
                        mySql = "";
                    }

                    if (!this.ExecuteQuery(out ds, mySql))
                        throw new Exception(this.LastExceptionMessage);
                }

                foreach (DataRow dataRow in ds.Tables[0].Rows)
                {
                    DocsPaVO.utente.DatiModificaCorr corr = new DocsPaVO.utente.DatiModificaCorr();
                    corr.codice = dataRow["var_codice"].ToString();
                    corr.codRubrica = dataRow["var_cod_rubrica"].ToString();
                    corr.codiceAmm = dataRow["var_codice_amm"].ToString();
                    corr.codiceAoo = dataRow["var_codice_aoo"].ToString();
                    corr.tipoCorrispondente = dataRow["cha_tipo_urp"].ToString();
                    corr.descCorr = dataRow["var_desc_corr"].ToString();
                    corr.cognome = dataRow["var_cognome"].ToString();
                    corr.nome = dataRow["var_nome"].ToString();
                    corr.indirizzo = dataRow["var_indirizzo"].ToString();
                    corr.cap = dataRow["var_cap"].ToString();
                    corr.citta = dataRow["var_citta"].ToString();
                    corr.provincia = dataRow["var_provincia"].ToString();
                    corr.localita = dataRow["var_localita"].ToString();
                    corr.nazione = dataRow["var_nazione"].ToString();
                    corr.codFiscale = dataRow["var_cod_fisc"].ToString();
                    corr.telefono = dataRow["var_telefono"].ToString();
                    corr.telefono2 = dataRow["var_telefono2"].ToString();
                    corr.fax = dataRow["var_fax"].ToString();
                    corr.email = dataRow["var_email"].ToString();
                    corr.note = dataRow["var_note"].ToString();
                    corr.partitaIva = dataRow["var_cod_pi"].ToString();
                    corr.descrizioneCanalePreferenziale = dataRow["description"].ToString();

                    elementi.Add(corr);
                }

                return elementi;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] GetUoInterneAooNoReg()
        {
            DataSet ds = new DataSet();
            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UO_IN_AOO_NO_REG");
            //q.setParam("id_reg", id_reg);

            string mySql = q.getSQL();
            logger.Debug(mySql);

            if (!this.ExecuteQuery(out ds, mySql))
                throw new Exception(this.LastExceptionMessage);

            string[] res = new string[ds.Tables[0].Rows.Count];
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                res[i] = (string)ds.Tables[0].Rows[i][0].ToString();
            }
            return res;
        }

        public bool IsDestinatarioInterno(DocsPaVO.utente.Corrispondente destinatario, string id_registro_documento)
        {

            try
            {

                logger.Debug("idDestinatarioInterno - start");

                DocsPaUtils.Query query = DocsPaUtils.InitQuery.getInstance().getQuery("S_ISCORRISPONDENTEINTERNO");
                if (dbType.ToUpper() == "SQL")
                    query.setParam("dbuser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());

                query.setParam("idcorrglobali", destinatario.systemId);
                if (!string.IsNullOrEmpty(id_registro_documento))
                    query.setParam("idregistro", id_registro_documento);
                else
                    query.setParam("idregistro", "0");

                logger.Debug("idDestinatarioInterno - query: " + query.getSQL());
                using (DBProvider dbProvider = new DBProvider())
                {
                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(query.getSQL()))
                    {
                        if (reader.Read())
                        {
                            int result = reader.GetInt32(reader.GetOrdinal("risultato"));
                            if (result == 1)
                                return true;
                        }
                    }
                }
                logger.Debug("idDestinatarioInterno - end");
            }
            catch (Exception e)
            {
                logger.Debug("errore in isDestinatarioInterno: - errore: " + e.Message + " ; stackTrace: " + e.StackTrace);
            }

            return false;
        }

        /// <summary>
        /// Attributi di un corrispondente della dpa_corr_globali
        /// </summary>
        public enum CorrAttributes
        {
            /// <summary>
            /// Descrizione del corrispondente
            /// </summary>
            VAR_DESC_CORR

        }

        /// <summary>
        /// Metodo per il recupero di un attributo di un corrispondente a partire dal suo codice
        /// </summary>
        /// <param name="corrCode">Codice del corrispondente</param>
        /// <param name="attribute">Attributo da estrarre</param>
        /// <returns>Attributo richiesto o il codice passato per parametro se non è stato individuato il corrispondente</returns>
        public static String GetCorrAttribute(String corrCode, CorrAttributes attribute)
        {
            String retVal = corrCode;
            String query = String.Format("Select {0} From dpa_corr_globali Where var_codice = '{1}'",
                attribute,
                corrCode);

            using (DBProvider dbProvider = new DBProvider())
            {
                using (IDataReader dataReader = dbProvider.ExecuteReader(query))
                {
                    while (dataReader.Read())
                    {
                        retVal = dataReader[0].ToString();
                    }
                }
            }

            return retVal;

        }

        public Dictionary<string, DocsPaVO.rubrica.ElementoRubrica> GetElementRubricaFromGerarchia(string cod, DocsPaVO.addressbook.TipoUtente tipoIE)
        {
            Dictionary<string, DocsPaVO.rubrica.ElementoRubrica> array = new Dictionary<string, DocsPaVO.rubrica.ElementoRubrica>();
            string srch_condition = "";

            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RICERCA_ELEMENTO_RUBRICA");
            try
            {
                srch_condition = " upper(var_cod_rubrica) in " + cod + " ";

                srch_condition += String.Format(" and id_amm={0}", _user.idAmministrazione);

                srch_condition += " AND CHA_TIPO_IE = '" + (tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO ? "I" : "E") + "' ";

                string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
                if (dbType.ToUpper() == "SQL")
                {
                    srch_condition += " and DTA_FINE is null and cha_tipo_urp != 'F' order by ISNULL (NUM_LIVELLO,100000) asc, has_children asc, CHA_TIPO_URP desc, VAR_DESC_CORR asc";
                }
                else
                {
                    srch_condition += " and DTA_FINE is null and cha_tipo_urp <> 'F' order by NUM_LIVELLO asc NULLS LAST, has_children asc, CHA_TIPO_URP desc, VAR_DESC_CORR asc";
                }

                q.setParam("param1", srch_condition);

                string mySql = q.getSQL();
                logger.Debug(mySql);

                DataSet ds = new DataSet();
                if (!this.ExecuteQuery(out ds, "corrispondenti", mySql))
                    throw new Exception(this.LastExceptionMessage);

                if (ds.Tables["corrispondenti"].Rows.Count != 0)
                {
                    DocsPaVO.rubrica.ElementoRubrica er = null;
                    foreach (DataRow dr in ds.Tables["corrispondenti"].Rows)
                    {
                        er = new DocsPaVO.rubrica.ElementoRubrica();
                        //28 marzo 2008
                        er.systemId = dr["system_id"].ToString();
                        er.codice = dr["var_cod_rubrica"].ToString();
                        er.descrizione = dr["var_desc_corr"].ToString();
                        er.interno = (Convert.ToDecimal(dr["interno"]) == 1);
                        er.tipo = dr["cha_tipo_urp"].ToString();
                        er.has_children = (Convert.ToDecimal(dr["has_children"]) == 1);
                        if (dr.Table.Columns.Contains("CHA_DISABLED_TRASM") && dr["CHA_DISABLED_TRASM"] != null && dr["CHA_DISABLED_TRASM"].ToString() == "1")
                        {
                            er.disabledTrasm = true;
                        }
                        if (!string.IsNullOrEmpty(dr["dta_fine"].ToString()))
                            er.disabled = true;
                        else
                            er.disabled = false;

                        if (!array.ContainsKey(er.codice.ToUpper()))
                        {
                            array.Add(er.codice.ToUpper(), er);
                        }
                    }
                }
                return array;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Laura 22 Febbraio 2013
        public int GetCountElementiRubricaPaging(DocsPaVO.rubrica.ParametriRicercaRubrica qc, int firstRowNum, int maxRowForPage)
        {
            int total = 0;
            int totale = 0;
            string qry = "";
            ArrayList elementi = new ArrayList();
            DocsPaDB.Query_Utils.Utils obj = new DocsPaDB.Query_Utils.Utils();
            bool no_filtro_aoo = obj.isFiltroAooEnabled();
            string userDb = string.Empty;
            string dbType = System.Configuration.ConfigurationManager.AppSettings["DBType"];
            if (dbType.ToUpper() == "SQL")
            {
                userDb = DocsPaDbManagement.Functions.Functions.GetDbUserSession() + ".";
            }






            DocsPaUtils.Query q = DocsPaUtils.InitQuery.getInstance().getQuery("S_RICERCA_COUNT_ELEMENTI_RUBRICA_PAGING");


            if (dbType.ToUpper() == "SQL")
                q.setParam("dbuser", userDb);

            //q.setParam("startRow", (firstRowNum + 1).ToString());
            //q.setParam("endRow", (firstRowNum + maxRowForPage).ToString());

            DataSet ds = new DataSet();
            try
            {
                ArrayList sp_params = new ArrayList();
                if (qc.parent != null && qc.parent != "")
                {
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("id_amm", Convert.ToInt32(_user.idAmministrazione), 0, DirectionParameter.ParamInput, System.Data.DbType.Decimal));

                    switch (qc.tipoIE)
                    {
                        case DocsPaVO.addressbook.TipoUtente.INTERNO:
                        default:
                            sp_params.Add(new DocsPaUtils.Data.ParameterSP("cha_tipo_ie", "I", 1, DirectionParameter.ParamInput, System.Data.DbType.String));
                            break;

                        case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                            sp_params.Add(new DocsPaUtils.Data.ParameterSP("cha_tipo_ie", "E", 1, DirectionParameter.ParamInput, System.Data.DbType.String));
                            break;
                    }
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("var_cod_rubrica", qc.parent, 128, DirectionParameter.ParamInput, System.Data.DbType.String));

                    int corr_types = 0;
                    corr_types += (qc.doUo ? 1 : 0);
                    corr_types += (qc.doRuoli ? 2 : 0);
                    corr_types += (qc.doUtenti ? 4 : 0);
                    sp_params.Add(new DocsPaUtils.Data.ParameterSP("corr_types", corr_types, 0, DirectionParameter.ParamInput, System.Data.DbType.Decimal));

                    if (this.ExecuteStoredProcedure("dpa3_get_children", sp_params, ds) != 1)
                        throw new Exception(this.LastExceptionMessage);
                }
                else
                {
                    // Non sono estratti i corrispondenti con tipologia "C", 
                    // ovvero gli elementi inseriti da rubrica comune

                    qry += DocsPaDbManagement.Functions.Functions.getNVL("a.cha_tipo_corr", "'X'") + " != 'C' AND ( (a.id_amm=" + _user.idAmministrazione + ") OR a.id_amm is null) AND ";
                    //if (qc.caller.filtroRegistroPerRicerca != null && qc.caller.filtroRegistroPerRicerca != "") {
                    //    qry += "id_registro in (" + qc.caller.filtroRegistroPerRicerca + ") AND ";
                    //}
                    switch (qc.tipoIE)
                    {
                        case DocsPaVO.addressbook.TipoUtente.INTERNO:
                            if (qc.doListe || qc.doRF)
                                qry += "(a.cha_tipo_ie='I' OR a.CHA_TIPO_IE IS NULL) AND ";
                            else
                                qry += "(a.cha_tipo_ie='I') AND ";
                            break;

                        case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                            if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE)
                                || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ESTERNI_AMM)
                                //modifica del 13/05/2009
                                || qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO
                                //fine modifica del 13/05/2009
                                )
                            {
                                qry += "(a.cha_tipo_ie='E') AND ";
                            }
                            break;

                        case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                            break;
                    }

                    if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                        qry += String.Format("(upper(a.var_cod_rubrica) = upper('{0}')) AND ", qc.codice.Replace("'", "''"));
                    else if (qc.codice != null && qc.codice != "")
                        qry += String.Format("(upper(a.var_cod_rubrica) LIKE upper('%{0}%')) AND ", qc.codice.Replace("'", "''"));

                    if (qc.descrizione != null && qc.descrizione != "")
                        qry += String.Format("(upper(a.var_desc_corr) LIKE upper('%{0}%')) AND ", qc.descrizione.Replace("'", "''"));
                    //qry = ricercaFullText("DESCRIZIONE", qry, qc.descrizione) + " AND ";

                    if (qc.citta != null && qc.citta != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_citta) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.citta.Replace("'", "''"));
                    }

                    if (qc.localita != null && qc.localita != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_localita) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.localita.Replace("'", "''"));
                    }

                    //if (qc.cf_piva != null && qc.cf_piva != "")
                    //{
                    //    qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_cod_fiscale) = upper('{0}')) and id_corr_globali = a.system_id)) AND ", qc.cf_piva.Replace("'", "''"));
                    //}

                    if (!string.IsNullOrEmpty(qc.email))
                    {
                        qry += String.Format("(upper(a.VAR_EMAIL) LIKE upper('%{0}%')) AND ", qc.email.Replace("'", "''"));
                    }

                    if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_cod_fisc) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.codiceFiscale.Replace("'", "''"));
                    }

                    if (qc.partitaIva != null && qc.partitaIva != "")
                    {
                        qry += String.Format("(exists (select system_id from dpa_dett_globali where (upper(var_cod_pi) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) AND ", qc.partitaIva.Replace("'", "''"));
                    }

                    //filtro per system_ID
                    if (qc.systemId != null && qc.systemId != "")
                    {
                        qry += String.Format("a.system_id = " + qc.systemId + " AND ");
                    }

                    //                    if (qc.doUo || qc.doRuoli || qc.doUtenti || qc.doListe || qc.doRF)
                    if (qc.doUo || qc.doRuoli || qc.doUtenti || qc.doRF)
                    {
                        qry += "a.cha_tipo_urp in (";
                        if (qc.doUo)
                            qry += "'U',";

                        if (qc.doRuoli)
                            qry += "'R',";

                        if (qc.doUtenti)
                            qry += "'P',";

                        //if (qc.doListe)
                        //    qry += "'L',";

                        if (qc.doRF)
                            qry += "'F',";

                        if (qry.EndsWith(","))
                            qry = qry.Substring(0, qry.Length - 1) + ")";
                    }
                    else
                    {
                        if (!qc.doListe)
                        {
                            totale = total;
                            return totale;
                        }
                    }

                    if (qry.EndsWith(" AND "))
                        qry = qry.Substring(0, qry.Length - " AND ".Length);


                    //La condizione sui ruoli disabilitati (DTA_FINE valorizzata) non viene considerata per calltype di ricerca
                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CREATOR ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_OWNER_AUTHOR ||
                        //IACOZZILLI GIORDANO 25/06/2013
                        //Aggiungo il calltype per il deposito:
                         qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEP_OSITO ||
                        //FINE
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_TODOLIST ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI_CORR_INT ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_DOCUMENTI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_DEST_FOR_SEARCH_MODELLI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_NO_FILTRI ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORRISPONDENTE
                        )
                    {
                        //qry += " AND a.dta_fine IS NULL";
                    }
                    else
                    {
                        qry += " AND a.dta_fine IS NULL";
                    }

                    if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORRISPONDENTE ||
                        qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_CORR_NON_STORICIZZATO)
                        && qc.caller != null && !string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca))
                    {
                        qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") or a.id_registro is null)";
                    }


                    if ((qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INGRESSO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_USCITA_SEMPLIFICATO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MULTIPLI_SEMPLIFICATO
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_ESTERNI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_VIS_UTENTE) //ABBATANGELI - TESTARE CALLTYPE_VIS_UTENTE
                        &&
                        (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.GLOBALE || qc.tipoIE == DocsPaVO.addressbook.TipoUtente.ESTERNO)
                        &&
                        (qc.caller.IdRegistro != null || qc.caller.IdRegistro != string.Empty)
                        &&
                        (qc.doUo || qc.doRuoli || qc.doUtenti)
                        )
                    {
                        //se è un solo registro verifico se è un RF
                        bool cha_rf = false;
                        if (qc.caller.filtroRegistroPerRicerca.IndexOf(",") == -1)
                        {
                            DocsPaVO.utente.Registro reg = GetRegistro(qc.caller.filtroRegistroPerRicerca);
                            if (reg != null && reg.chaRF == "1")
                                cha_rf = true;
                        }

                        //  if (!no_filtro_aoo)
                        //   {
                        if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                            qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") or a.id_registro is null)";
                        if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && cha_rf)
                            qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + "))";
                        if (string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                            qry += " AND a.id_registro is null";
                        //  }
                    }


                    // ###############     MODELLI DI TRASMISSIONE    #################
                    // se:		è stata chiamata da modelli di trasmissione o dalla funzionalità trova e sostituisci
                    // allora:	la query deve filtrare anche per registro		
                    if (
                        (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MODELLO_TRASM ||
                            qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_REPLACE_ROLE ||
                            qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE ||
                            qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_VIS_RUOLO) //ABBATANGELI - TESTARE CALLTYPE_VIS_RUOLO
                        &&
                        (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO)
                        )
                    {
                        if (qc.caller.IdRegistro != null && qc.caller.IdRegistro != string.Empty)
                        {
                            q.setParam("param2", ",DPA_L_RUOLO_REG ");
                            qry += " AND DPA_L_RUOLO_REG.ID_RUOLO_IN_UO = A.SYSTEM_ID AND " +
                                " DPA_L_RUOLO_REG.id_registro = " + qc.caller.IdRegistro;
                        }
                        else
                        {
                            q.setParam("param2", ",DPA_L_RUOLO_REG ");
                            qry += " AND DPA_L_RUOLO_REG.ID_RUOLO_IN_UO = A.SYSTEM_ID ";
                        }
                    }

                    //############### Ruolo Interoperabilità nomail ####################
                    if (
                        (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_REG_NOMAIL
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_RESP_REG)
                        &&
                        (qc.tipoIE == DocsPaVO.addressbook.TipoUtente.INTERNO)
                        &&
                        (qc.caller.IdRegistro != null && qc.caller.IdRegistro != string.Empty)
                        )
                    {
                        q.setParam("param2", ",DPA_L_RUOLO_REG ");
                        qry += " AND DPA_L_RUOLO_REG.ID_RUOLO_IN_UO = A.SYSTEM_ID AND " +
                            " DPA_L_RUOLO_REG.id_registro = " + qc.caller.IdRegistro;
                    }

                    if (qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_REG_NOMAIL &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RUOLO_RESP_REG &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MITT_MODELLO_TRASM &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_REPLACE_ROLE &&
                        qc.calltype != DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_FIND_ROLE)
                    {
                        q.setParam("param2", "");
                    }

                    // Se si usa la rubrica da GESTIONE RUBRICA vengono restituiti solamente
                    //corrispondenti esterni all'amministrazione creati sul registri visibili al ruolo corrente
                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_MANAGE
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_ESTERNI_AMM
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_ESTESA)
                       || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTDEST)
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_MITTINTERMEDIO)
                        || (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_COMPLETAMENTO)
                        || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_EST_CON_DISABILITATI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_CON_DISABILITATI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST_CON_DISABILITATI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_NO_FILTRI
                         || qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_CORR_INT_NO_UO)
                    {
                        //se è un solo registro verifico se è un RF
                        bool cha_rf = false;
                        if (qc.caller != null && !string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && qc.caller.filtroRegistroPerRicerca.IndexOf(",") == -1)
                        {
                            DocsPaVO.utente.Registro reg = GetRegistro(qc.caller.filtroRegistroPerRicerca);
                            if (reg != null && reg.chaRF == "1")
                                cha_rf = true;
                        }

                        #region codice commentato
                        //qry += " AND (id_registro IN (" + qc.caller.IdRegistro + ") or id_registro is null)";

                        //MODIFICA BUG: non carica valori in rubrica per le liste di distribuzione --> registro null
                        //qry += " AND (id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") ";
                        ////se seleziono un RF non deve cercare per id_registro = null
                        //if (cha_rf)
                        //{
                        //    qry += ")";
                        //}
                        //else
                        //{
                        //    qry += " or id_registro is null)";
                        //}
                        #endregion
                        if (!no_filtro_aoo)
                        {
                            if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                                qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + ") or a.id_registro is null)";
                            if (!string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && cha_rf)
                                qry += " AND (a.id_registro IN (" + qc.caller.filtroRegistroPerRicerca + "))";
                            if (string.IsNullOrEmpty(qc.caller.filtroRegistroPerRicerca) && !cha_rf)
                                qry += " AND a.id_registro is null";
                        }
                    }
                    #region codice commentato
                    //sabrina: liste di distribuzione
                    //corrispondenti esterni all'amministrazione creati sui registri visibili al ruolo corrente

                    //if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_LISTE_DISTRIBUZIONE)
                    //{
                    //    if (qc.caller != null && (qc.caller.IdRegistro != null && !qc.caller.IdRegistro.Equals("")))
                    //        qry += " AND (ID_REGISTRO " + qc.caller.IdRegistro + " OR ID_REGISTRO IS NULL)";
                    //}
                    #endregion

                    //####### Liste di distribuzione ######
                    if (qc.doListe)
                    {
                        //E' selezionata solo la ricerca delle liste quindi niente UNION
                        if (!qc.doUo && !qc.doRuoli && !qc.doUtenti && !qc.doRF)
                        {
                            qry = "(a.id_amm='" + _user.idAmministrazione + "') AND " +
                                    "(a.cha_tipo_urp in ('L'))";
                            if ((qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals("")) || (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals("")))
                            {
                                qry += " AND (";
                                bool res = false;
                                if (qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals(""))
                                {
                                    qry += "(a.id_people_liste=" + qc.caller.IdUtente + " AND a.id_gruppo_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                if (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals(""))
                                {
                                    qry += " (a.id_gruppo_liste=" + _user.idGruppo + " AND a.id_people_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                qry += " (a.id_people_liste is null AND a.id_gruppo_liste is null)) AND " +
                                        "a.dta_fine IS NULL ";
                            }
                        }
                        //E' una ricerca combinata liste con utenti, ruoli o uffici
                        else
                        {
                            qry += " UNION ";
                            //Laura 25 Febbraio 2013
                            qry += " SELECT  a.system_id " +
                                "FROM dpa_corr_globali a LEFT JOIN dpa_dett_globali dett ON dett.id_corr_globali = a.system_id LEFT JOIN DOCSADM.DPA_T_CANALE_CORR ON a.system_id = DOCSADM.DPA_T_CANALE_CORR.ID_CORR_GLOBALE " +
                                "LEFT JOIN DOCSADM.DOCUMENTTYPES ON DOCSADM.DPA_T_CANALE_CORR.ID_DOCUMENTTYPE = DOCSADM.DOCUMENTTYPES.SYSTEM_ID " +
                                "WHERE " +
                                "(a.id_amm='" + _user.idAmministrazione + "') AND " +
                                "(a.cha_tipo_urp in ('L'))";
                            if ((qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals("")) || (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals("")))
                            {
                                qry += " AND (";
                                bool res = false;
                                if (qc.caller != null && qc.caller.IdUtente != null && !qc.caller.IdUtente.Equals(""))
                                {
                                    qry += "(a.id_people_liste=" + qc.caller.IdUtente + " AND a.id_gruppo_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                if (_user != null && _user.idGruppo != null && !_user.idGruppo.Equals(""))
                                {
                                    qry += " (a.id_gruppo_liste=" + _user.idGruppo + " AND a.id_people_liste is null) ";
                                    res = true;
                                }
                                if (res)
                                    qry += " OR";
                                qry += " (a.id_people_liste is null AND a.id_gruppo_liste is null))";
                            }

                            //"((id_people_liste="+qc.caller.IdUtente+" AND id_gruppo_liste is null) "+
                            //"OR (id_gruppo_liste=" + _user.idGruppo + " AND id_people_liste is null) "+
                            //"OR (id_people_liste is null AND id_gruppo_liste is null))";
                            if (qc.localita != null && qc.localita != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_localita) LIKE upper('%{0}%')) and id_corr_globali = a.system_id))", qc.localita.Replace("'", "''"));
                            }

                            //if (qc.cf_piva != null && qc.cf_piva != "")
                            //{
                            //    qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fiscale) = upper('{0}')) and id_corr_globali = a.system_id)) ", qc.cf_piva.Replace("'", "''"));
                            //}

                            if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fisc) LIKE upper('%{0}%')) and id_corr_globali = a.system_id))", qc.codiceFiscale.Replace("'", "''"));
                            }

                            if (qc.partitaIva != null && qc.partitaIva != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_pi) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) ", qc.partitaIva.Replace("'", "''"));
                            }

                            if (!string.IsNullOrEmpty(qc.email))
                            {
                                qry += String.Format(" AND (upper(a.VAR_EMAIL) LIKE upper('%{0}%')) ", qc.email.Replace("'", "''"));
                            }
                        }

                        //Verifico eventuali condizioni ulteriori per la ricerca. Codice-Descrizione-Registro
                        //Codice
                        if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                            qry += String.Format(" AND (upper(a.var_cod_rubrica) = upper('{0}')) ", qc.codice.Replace("'", "''"));
                        else if (qc.codice != null && qc.codice != "")
                            qry += String.Format(" AND (upper(a.var_cod_rubrica) LIKE upper('%{0}%')) ", qc.codice.Replace("'", "''"));

                        //Descrizione
                        if (qc.descrizione != null && qc.descrizione != "")
                        {
                            qry += String.Format(" AND (upper(a.var_desc_corr) LIKE upper('%{0}%')) ", qc.descrizione.Replace("'", "''"));
                            //qry += " AND ";
                            //qry = ricercaFullText("DESCRIZIONE", qry, qc.descrizione);
                        }
                        //Registro - Eventualmente da implementare

                    }

                    #region RF
                    if (qc.doRF)
                    {
                        //E' selezionata solo la ricerca degli RF quindi niente UNION
                        if (!qc.doUo && !qc.doRuoli && !qc.doUtenti && !qc.doListe)
                        {
                            if (qry.IndexOf("a.cha_tipo_urp in ('F')") == -1)
                                qry += " ( (a.id_amm=" + _user.idAmministrazione + ") OR a.id_amm is null) AND cha_tipo_urp = 'F' ";
                        }
                        //E' una ricerca combinata RF con utenti, ruoli, uffici o liste
                        else
                        {
                            qry += " UNION ";
                            //Laura 25 Febbraio 2013
                            qry += " SELECT a.system_id " +
                                "FROM dpa_corr_globali a LEFT JOIN dpa_dett_globali dett ON dett.id_corr_globali = a.system_id LEFT JOIN DOCSADM.DPA_T_CANALE_CORR ON a.system_id = DOCSADM.DPA_T_CANALE_CORR.ID_CORR_GLOBALE " +
                                "LEFT JOIN DOCSADM.DOCUMENTTYPES ON DOCSADM.DPA_T_CANALE_CORR.ID_DOCUMENTTYPE = DOCSADM.DOCUMENTTYPES.SYSTEM_ID " +
                                "WHERE " +
                                "(a.id_amm='" + _user.idAmministrazione + "') AND a.cha_tipo_urp = 'F' ";

                            //Verifico eventuali condizioni ulteriori per la ricerca. Codice-Descrizione-Registro
                            //Codice
                            if (qc.codice != null && qc.codice != "" && qc.queryCodiceEsatta)
                                qry += String.Format(" AND (upper(a.var_cod_rubrica) = upper('{0}')) ", qc.codice.Replace("'", "''"));
                            else if (qc.codice != null && qc.codice != "")
                                qry += String.Format(" AND (upper(a.var_cod_rubrica) LIKE upper('%{0}%')) ", qc.codice.Replace("'", "''"));

                            //Descrizione
                            if (qc.descrizione != null && qc.descrizione != "")
                            {
                                qry += String.Format(" AND (upper(a.var_desc_corr) LIKE upper('%{0}%')) ", qc.descrizione.Replace("'", "''"));
                                //qry += " AND ";
                                //qry = ricercaFullText("DESCRIZIONE", qry, qc.descrizione);
                            }

                            //Registro - Eventualmente da implementare

                            if (qc.localita != null && qc.localita != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_localita) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) ", qc.localita.Replace("'", "''"));
                            }

                            //if (qc.cf_piva != null && qc.cf_piva != "")
                            //{
                            //    qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fiscale) = upper('{0}')) and id_corr_globali = a.system_id)) ", qc.cf_piva.Replace("'", "''"));
                            //}


                            if (qc.codiceFiscale != null && qc.codiceFiscale != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_fisc) LIKE upper('%{0}%')) and id_corr_globali = a.system_id))", qc.codiceFiscale.Replace("'", "''"));
                            }

                            if (qc.partitaIva != null && qc.partitaIva != "")
                            {
                                qry += String.Format(" AND (exists (select system_id from dpa_dett_globali where (upper(var_cod_pi) LIKE upper('%{0}%')) and id_corr_globali = a.system_id)) ", qc.partitaIva.Replace("'", "''"));
                            }

                            if (!string.IsNullOrEmpty(qc.email))
                            {
                                qry += String.Format(" AND (upper(a.VAR_EMAIL) LIKE upper('%{0}%')) ", qc.email.Replace("'", "''"));
                            }

                        }

                        //if (qc.caller.filtroRegistroPerRicerca != null && qc.caller.filtroRegistroPerRicerca != "")
                        //{
                        //    qry += " and ID_REGISTRO in (" + qc.caller.filtroRegistroPerRicerca + ")";
                        //}
                    }
                    #endregion

                    bool nessun_sottoposto = false;

                    //INSERITO PER TROVARE SOLTANTO I RUOLI SOTTOPOSTI
                    if (qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO)
                    {
                        ArrayList listaRuoliInf;
                        DataSet dsTempRuoli = new DataSet();
                        DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
                        DocsPaVO.utente.Ruolo ruoloGlobale = utenti.getRuoloById(qc.caller.IdRuolo);
                        DocsPaVO.utente.Ruolo mioRuolo = utenti.GetRuoloByIdGruppo(ruoloGlobale.idGruppo);
                        DocsPaDB.Utils.Gerarchia gerarchia = new DocsPaDB.Utils.Gerarchia();
                        DocsPaVO.trasmissione.TipoOggetto tipo = new DocsPaVO.trasmissione.TipoOggetto();
                        listaRuoliInf = gerarchia.getGerarchiaInf(mioRuolo, null, null, tipo);
                        listaRuoliInf.Add(mioRuolo);

                        string addConstant = null;

                        if (listaRuoliInf.Count != 0)
                        {
                            addConstant = " AND A.SYSTEM_ID IN (";

                            for (int i = 0; i < listaRuoliInf.Count; i++)
                            {
                                addConstant = addConstant + ((DocsPaVO.utente.Ruolo)listaRuoliInf[i]).systemId;
                                if (i < listaRuoliInf.Count - 1)
                                {
                                    if (i % 998 == 0 && i > 0)
                                    {

                                    }
                                    else
                                    {
                                        addConstant += ", ";
                                    }
                                }
                                else
                                {
                                    addConstant += ")";
                                }
                            }
                            qry += addConstant;
                        }
                        else
                        {
                            nessun_sottoposto = true;
                        }
                    }

                    //qry += " order by cha_tipo_urp desc, var_desc_corr";

                    q.setParam("param1", qry);

                    string mySql = q.getSQL();
                    logger.Debug(mySql);

                    if (nessun_sottoposto == true && qc.calltype == DocsPaVO.rubrica.ParametriRicercaRubrica.CallType.CALLTYPE_RICERCA_TRASM_SOTTOPOSTO)
                    {
                        mySql = "";
                    }

                    if (!this.ExecuteQuery(out ds, mySql))
                        throw new Exception(this.LastExceptionMessage);


                    //if (ds != null)
                    //    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    //    {
                    //        //SearchResultInfo temp = new SearchResultInfo();
                    //        //temp.Id = dataRow["SYSTEM_ID"].ToString();
                    //        //temp.Codice = dataRow["CODICE"].ToString();
                    //        //idProfiles.Add(temp);
                    //    }


                    if (ds != null && ds.Tables != null && ds.Tables[0] != null && ds.Tables[0].Rows != null)
                    {
                        totale = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
                    }
                }

                return totale;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
    }


}
