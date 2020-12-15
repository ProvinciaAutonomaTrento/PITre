using System;
using System.Data;
using System.Collections;
using log4net;


namespace BusinessLogic.Utenti
{
    /// <summary>
    /// </summary>
    public class DirittiManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(DirittiManager));
        /// <summary>
        /// </summary>
        /// <param name="infoDoc"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getListaDiritti(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, bool cercaRimossi)
        {
            DataSet dataSet = new DataSet();
            ArrayList listaDiritti = new ArrayList();
            string IDAMM = string.Empty;
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            //1-- inserimento ruoli
            try
            {
                utenti.GetListaDiritti(out dataSet, idProfile);
                //modifica per anomalia visibilità 27102004 PA
                //si presuppone che l'id amministrazione sia univoco
                //pertanto viene assegnato alla variabile IDAMM ,anche 
                //se viene effettuato un ciclo sul dataset
                //NB: Da verificare

                foreach (DataRow ruoloRow in dataSet.Tables["DIRITTI_RUOLI"].Rows)
                {
                    DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto();
                    DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();

                    #region Codice Commentato
                    /*ruolo.systemId=ruoloRow["SYSTEM_ID"].ToString();
					ruolo.codiceRubrica=ruoloRow["VAR_COD_RUBRICA"].ToString();
					ruolo.descrizione=ruoloRow["VAR_DESC_RUOLO"].ToString();*/
                    #endregion

                    DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                    qco.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();
                    qco.getChildren = false;
                    if (ruoloRow["ID_REGISTRO"] != null && !ruoloRow["ID_REGISTRO"].ToString().Equals(""))
                    {
                        System.Collections.ArrayList reg = new System.Collections.ArrayList();
                        reg.Add(ruoloRow["ID_REGISTRO"].ToString());
                        qco.idRegistri = reg;
                    }
                    if (ruoloRow["ID_AMM"] != null && !ruoloRow["ID_AMM"].ToString().Equals(""))
                    {
                        qco.idAmministrazione = ruoloRow["ID_AMM"].ToString();
                        IDAMM = qco.idAmministrazione;
                    }
                    qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;

                    //gadamo 16.12.2008
                    qco.fineValidita = false; // voglio recuperare anche i disabilitati ( non mette "DTA_FINE IS NULL" nella query nel metodo Utenti.ListaCorrispondentiInt(qco) )

                    ruolo = (DocsPaVO.utente.Ruolo)BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco)[0];
                    dirittoOggetto.idObj = idProfile;
                    dirittoOggetto.soggetto = ruolo;
                    dirittoOggetto.tipoDiritto = getDiritto(ruoloRow["CHA_TIPO_DIRITTO"].ToString());
                    dirittoOggetto.accessRights = Convert.ToInt32(ruoloRow["ACCESSRIGHTS"]);
                    dirittoOggetto.deleted = false;
                    dirittoOggetto.personorgroup = ruoloRow["PERSONORGROUP"].ToString();

                    if (ruoloRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
                        dirittoOggetto.hideDocVersions = (ruoloRow["HIDE_DOC_VERSIONS"].ToString() == "1");
                    
                    listaDiritti.Add(dirittoOggetto);
                    logger.Debug("Ruolo inserito");
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione DIRITTI_RUOLI", e);
                listaDiritti = null;
                return listaDiritti;
            }

            //2-- inserimento utenti
            try
            {
                //VERONICA IDAMM potrebbe essere vuoto
                if (IDAMM == "")
                {
                    IDAMM = infoUtente.idAmministrazione;
                }
                utenti.GetListaRuoli(out dataSet, idProfile, IDAMM);

                logger.Debug("Utenti:" + dataSet.Tables["DIRITTI_UTENTI"].Rows.Count);

                DocsPaVO.addressbook.QueryCorrispondente qco1 = new DocsPaVO.addressbook.QueryCorrispondente();
                            
                System.Collections.ArrayList utentiInt = new ArrayList();
                DocsPaVO.utente.Corrispondente  soggettoPropr = null;
                if (dataSet.Tables["DIRITTI_UTENTI"].Rows.Count > 1)
                {
                    foreach (DataRow utenteRow in dataSet.Tables["DIRITTI_UTENTI"].Rows)
                    {
                        DocsPaVO.documento.TipoDiritto tipoDiritto = getDiritto(utenteRow["CHA_TIPO_DIRITTO"].ToString());
                        if (tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_PROPRIETARIO))
                        {
                            qco1.codiceRubrica = utenteRow["VAR_COD_RUBRICA"].ToString();
                            qco1.getChildren = false;
                            if (utenteRow["ID_REGISTRO"] != null && !utenteRow["ID_REGISTRO"].ToString().Equals(""))
                            {
                                System.Collections.ArrayList reg = new System.Collections.ArrayList();
                                reg.Add(utenteRow["ID_REGISTRO"].ToString());
                                qco1.idRegistri = reg;
                            }
                            if (utenteRow["ID_AMM"] != null && !utenteRow["ID_AMM"].ToString().Equals(""))
                            {
                                qco1.idAmministrazione = utenteRow["ID_AMM"].ToString();
                            }
                            qco1.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;

                            //gadamo 16.12.2008
                            qco1.fineValidita = false; // voglio recuperare anche i disabilitati ( non mette "DTA_FINE IS NULL" nella query nel metodo Utenti.ListaCorrispondentiInt(qco) )


                            utentiInt = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco1);
                            //DocsPaVO.utente.Utente utProprietario = new DocsPaVO.utente.Utente();
                            //utentiInt = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco1);
                            if (utentiInt != null && utentiInt.Count > 0)
                            {
                                soggettoPropr = (DocsPaVO.utente.Utente)utentiInt[0];
                                //soggettoPropr = utProprietario;
                            }
                        }
                    }
                }
                
                foreach (DataRow utenteRow in dataSet.Tables["DIRITTI_UTENTI"].Rows)
                {
                    DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto();
                    DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                    DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                    qco.codiceRubrica = utenteRow["VAR_COD_RUBRICA"].ToString();
                    qco.getChildren = false;
                    if (utenteRow["ID_REGISTRO"] != null && !utenteRow["ID_REGISTRO"].ToString().Equals(""))
                    {
                        System.Collections.ArrayList reg = new System.Collections.ArrayList();
                        reg.Add(utenteRow["ID_REGISTRO"].ToString());
                        qco.idRegistri = reg;
                    }
                    if (utenteRow["ID_AMM"] != null && !utenteRow["ID_AMM"].ToString().Equals(""))
                    {
                        qco.idAmministrazione = utenteRow["ID_AMM"].ToString();
                    }
                    qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;

                    //gadamo 16.12.2008
                    qco.fineValidita = false; // voglio recuperare anche i disabilitati ( non mette "DTA_FINE IS NULL" nella query nel metodo Utenti.ListaCorrispondentiInt(qco) )

                    
                    utentiInt = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco);
                    if (utentiInt != null && utentiInt.Count > 0)
                    {
                        utente = (DocsPaVO.utente.Utente)utentiInt[0];
                        dirittoOggetto.idObj = idProfile;
                        
                        dirittoOggetto.tipoDiritto = getDiritto(utenteRow["CHA_TIPO_DIRITTO"].ToString());
                        if (dirittoOggetto.tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_DELEGATO))
                        {
                            dirittoOggetto.soggetto = utente;
                            dirittoOggetto.soggetto.descrizione = utente.descrizione + " delegato da " + soggettoPropr.descrizione;
                        }
                        else
                            dirittoOggetto.soggetto = utente;
                        dirittoOggetto.accessRights = Convert.ToInt32(utenteRow["ACCESSRIGHTS"]);
                        dirittoOggetto.deleted = false;
                        dirittoOggetto.personorgroup = utenteRow["PERSONORGROUP"].ToString();

                        if (utenteRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
                            dirittoOggetto.hideDocVersions = (utenteRow["HIDE_DOC_VERSIONS"].ToString() == "1");
                        
                        listaDiritti.Add(dirittoOggetto);
                        logger.Debug("Utente inserito");
                    }
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione DIRITTI_UTENTI", e);
                listaDiritti = null;
                return listaDiritti;
            }

            //3-- inserimento ruoli rimossi
            //gestione diritti rimossi
            if (cercaRimossi)
            {
                try
                {
                    utenti.GetListaDiritti_Deleted(out dataSet, idProfile);
                    foreach (DataRow ruoloRow in dataSet.Tables["DIRITTI_RUOLI_DELETED"].Rows)
                    {
                        DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto();
                        DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                        DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                        qco.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();
                        qco.getChildren = false;
                        if (ruoloRow["ID_REGISTRO"] != null && !ruoloRow["ID_REGISTRO"].ToString().Equals(""))
                        {
                            System.Collections.ArrayList reg = new System.Collections.ArrayList();
                            reg.Add(ruoloRow["ID_REGISTRO"].ToString());
                            qco.idRegistri = reg;
                        }
                        if (ruoloRow["ID_AMM"] != null && !ruoloRow["ID_AMM"].ToString().Equals(""))
                        {
                            qco.idAmministrazione = ruoloRow["ID_AMM"].ToString();
                            IDAMM = qco.idAmministrazione;
                        }
                        qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;

                        ruolo = (DocsPaVO.utente.Ruolo)BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco)[0];
                        dirittoOggetto.idObj = idProfile;
                        dirittoOggetto.soggetto = ruolo;
                        dirittoOggetto.tipoDiritto = getDiritto(ruoloRow["CHA_TIPO_DIRITTO"].ToString());
                        dirittoOggetto.accessRights = Convert.ToInt32(ruoloRow["ACCESSRIGHTS"]);
                        dirittoOggetto.deleted = true;
                        dirittoOggetto.note = ruoloRow["NOTE"].ToString();
                        dirittoOggetto.personorgroup = ruoloRow["PERSONORGROUP"].ToString();

                        if (ruoloRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
                            dirittoOggetto.hideDocVersions = (ruoloRow["HIDE_DOC_VERSIONS"].ToString() == "1");

                        listaDiritti.Add(dirittoOggetto);
                        logger.Debug("Ruolo rimosso inserito");
                    }
                }
                catch (Exception e)
                {
                    logger.Debug("Errore nella gestione DIRITTI_RUOLI_DELETED", e);
                    listaDiritti = null;
                    return listaDiritti;
                }

                //4-- inserimento utenti rimossi
                try
                {
                    //VERONICA IDAMM potrebbe essere vuoto
                    if (IDAMM == "")
                    {
                        IDAMM = infoUtente.idAmministrazione;
                    }
                    utenti.GetListaRuoli_Deleted(out dataSet, idProfile, IDAMM);

                    logger.Debug("Utenti:" + dataSet.Tables["DIRITTI_UTENTI_DELETED"].Rows.Count);
                    foreach (DataRow utenteRow in dataSet.Tables["DIRITTI_UTENTI_DELETED"].Rows)
                    {
                        DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto();
                        DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                        DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                        qco.codiceRubrica = utenteRow["VAR_COD_RUBRICA"].ToString();
                        qco.getChildren = false;
                        if (utenteRow["ID_REGISTRO"] != null && !utenteRow["ID_REGISTRO"].ToString().Equals(""))
                        {
                            System.Collections.ArrayList reg = new System.Collections.ArrayList();
                            reg.Add(utenteRow["ID_REGISTRO"].ToString());
                            qco.idRegistri = reg;
                        }
                        if (utenteRow["ID_AMM"] != null && !utenteRow["ID_AMM"].ToString().Equals(""))
                        {
                            qco.idAmministrazione = utenteRow["ID_AMM"].ToString();
                        }
                        qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                        System.Collections.ArrayList utentiInt = new ArrayList();
                        utentiInt = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco);
                        if (utentiInt != null && utentiInt.Count > 0)
                        {
                            utente = (DocsPaVO.utente.Utente)utentiInt[0];
                            dirittoOggetto.idObj = idProfile;
                            dirittoOggetto.soggetto = utente;
                            dirittoOggetto.tipoDiritto = getDiritto(utenteRow["CHA_TIPO_DIRITTO"].ToString());
                            dirittoOggetto.accessRights = Convert.ToInt32(utenteRow["ACCESSRIGHTS"]);
                            dirittoOggetto.deleted = true;
                            dirittoOggetto.note = utenteRow["NOTE"].ToString();
                            dirittoOggetto.personorgroup = utenteRow["PERSONORGROUP"].ToString();

                            if (utenteRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
                                dirittoOggetto.hideDocVersions = (utenteRow["HIDE_DOC_VERSIONS"].ToString() == "1");

                            listaDiritti.Add(dirittoOggetto);
                            logger.Debug("Utente rimosso inserito");
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Debug("Errore nella gestione DIRITTI_UTENTI_DELETED", e);
                    listaDiritti = null;
                    return listaDiritti;
                }
            }
            return listaDiritti;
        }

        public static System.Collections.ArrayList getListaStoriaDiritti(string idProfile, string tipoObj, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DataSet dataSet = new DataSet();
            ArrayList listaDiritti = new ArrayList();
            string IDAMM = string.Empty;
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            try
            {
                utenti.GetListaStoriaDiritti(out dataSet, idProfile, tipoObj);
                foreach (DataRow row in dataSet.Tables["STORIA_DIRITTI"].Rows)
                {
                    DocsPaVO.documento.StoriaDirittoDocumento storia = new DocsPaVO.documento.StoriaDirittoDocumento();
                    storia.utente = row["USERID_OPERATORE"].ToString();
                    storia.ruolo = row["var_desc_corr"].ToString();
                    storia.data = row["dta_azione"].ToString();
                    storia.descrizione = row["var_desc_oggetto"].ToString();

                    if (storia.descrizione.StartsWith("Revoca"))
                        storia.codOperazione = "REVOCA";
                    if (storia.descrizione.StartsWith("Ripristino"))
                        storia.codOperazione = "RIPRISTINO";                    
                    if(storia.descrizione.StartsWith("Cede"))
                        storia.codOperazione = "CESSIONE";

                    listaDiritti.Add(storia);
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione LISTA STORIA DIRITTI: ", e);
                listaDiritti = null;
                return listaDiritti;
            }
            return listaDiritti;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static DocsPaVO.documento.TipoDiritto getDiritto(string str)
        {
            if (str.Equals("P"))
            {
                return DocsPaVO.documento.TipoDiritto.TIPO_PROPRIETARIO;
            }
            if (str.Equals("T"))
            {
                return DocsPaVO.documento.TipoDiritto.TIPO_TRASMISSIONE;
            }
            if (str.Equals("F"))
            {
                return DocsPaVO.documento.TipoDiritto.TIPO_TRASMISSIONE_IN_FASCICOLO;
            }
            if (str.Equals("S"))
            {
                return DocsPaVO.documento.TipoDiritto.TIPO_SOSPESO;
            }
            if (str.Equals("D"))
            {
                return DocsPaVO.documento.TipoDiritto.TIPO_DELEGATO;
            }
            // INTEGRAZIONE POLICY-PARER
            // MEV Responsabile della conservazione
            // Aggiunto un nuovo tipo diritto, "CONSERVAZIONE"
            if (str.Equals("C"))
            {
                return DocsPaVO.documento.TipoDiritto.TIPO_CONSERVAZIONE;
            }
            return DocsPaVO.documento.TipoDiritto.TIPO_ACQUISITO;
        }

        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static DocsPaVO.fascicolazione.TipoDiritto getDirittoFasc(string str)
        {
            if (str.Equals("P"))
            {
                return DocsPaVO.fascicolazione.TipoDiritto.TIPO_PROPRIETARIO;
            }
            if (str.Equals("T"))
            {
                return DocsPaVO.fascicolazione.TipoDiritto.TIPO_TRASMISSIONE;
            }
            //if (str.Equals("F"))
            //{
            //    return DocsPaVO.fascicolazione.TipoDiritto.TIPO_TRASMISSIONE_IN_FASCICOLO;
            //}
            if (str.Equals("S"))
            {
                return DocsPaVO.fascicolazione.TipoDiritto.TIPO_SOSPESO;
            }
            if (str.Equals("D"))
            {
                return DocsPaVO.fascicolazione.TipoDiritto.TIPO_DELEGATO;
            }
            return DocsPaVO.fascicolazione.TipoDiritto.TIPO_ACQUISITO;
        }

        /// <summary>
        /// </summary>
        /// <param name="infoFasc"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getListaDirittiFasc(DocsPaVO.fascicolazione.InfoFascicolo infoFasc, bool cercaRimossi,string rootFolder)
        {
            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();			
            //db.openConnection();
            System.Collections.ArrayList listaDiritti = new ArrayList();
            try
            {
                listaDiritti = BusinessLogic.Fascicoli.ProjectsManager.getVisibilita(/*db,*/ infoFasc.idFascicolo, cercaRimossi, rootFolder);
                //ritorna la lista dei diritti
                //db.closeConnection();
                logger.Debug("Fine metodo");

            }
            catch (Exception e)
            {
                //db.closeConnection();
                logger.Debug("Errore nella gestione degli utenti (getListaDirittiFasc)", e);
                throw e;
            }
            return listaDiritti;
        }

        public static ArrayList getListaStoriciMittenti(string idProfile, string tipo)
        {
            DocsPaDB.Query_DocsPAWS.Utenti user = new DocsPaDB.Query_DocsPAWS.Utenti();
            return user.getListaStoriciMittenti(idProfile, tipo);
        }

        public static System.Collections.ArrayList getListaDirittiSemplificato(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, bool cercaRimossi)
        {
            DataSet dataSet = new DataSet();
            ArrayList listaDiritti = new ArrayList();
            string IDAMM = string.Empty;
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();

            //1-- inserimento ruoli
            try
            {
                utenti.GetListaDirittiSemplificataRuoli(out dataSet, idProfile);

                foreach (DataRow ruoloRow in dataSet.Tables["DIRITTI_RUOLI"].Rows)
                {
                    DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto();
                    DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                    ruolo.dta_fine = ruoloRow["dta_fine"].ToString();
                    ruolo.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();

                    // Visualizzazione del pulsante della storia del ruolo se il ruolo ha una storia
                    ruolo.ShowHistory = ruoloRow["ShowHistory"] != DBNull.Value ? ruoloRow["ShowHistory"].ToString() : String.Empty;

                    // Impostazione dell'id del ruolo
                    ruolo.systemId = ruoloRow["system_id"].ToString();

                    if (ruoloRow["ID_REGISTRO"] != null && !ruoloRow["ID_REGISTRO"].ToString().Equals(""))
                    {
                        System.Collections.ArrayList reg = new System.Collections.ArrayList();
                        reg.Add(ruoloRow["ID_REGISTRO"].ToString());
                        ruolo.registri = reg;
                    }
                    if (ruoloRow["ID_AMM"] != null && !ruoloRow["ID_AMM"].ToString().Equals(""))
                    {
                        ruolo.idAmministrazione = ruoloRow["ID_AMM"].ToString();
                        IDAMM = ruolo.idAmministrazione;
                    }

                    if (ruoloRow["VAR_DESC_CORR"] != null && !ruoloRow["VAR_DESC_CORR"].ToString().Equals(""))
                    {
                        ruolo.descrizione = ruoloRow["VAR_DESC_CORR"].ToString();
                    }

                    if (ruoloRow["ID_UO"] != null && !ruoloRow["ID_UO"].ToString().Equals(""))
                    {
                        ruolo.uo = new DocsPaVO.utente.UnitaOrganizzativa();
                        ruolo.uo.systemId = ruoloRow["ID_UO"].ToString();
                        ruolo.uo.idAmministrazione = IDAMM;
                        if (ruoloRow["DESC_UO"] != null && !ruoloRow["DESC_UO"].ToString().Equals(""))
                        {
                            ruolo.uo.descrizione = ruoloRow["DESC_UO"].ToString();
                        }
                        if (ruoloRow["CHA_TIPO_UO"] != null && !ruoloRow["CHA_TIPO_UO"].ToString().Equals(""))
                        {
                            ruolo.uo.tipoIE = ruoloRow["CHA_TIPO_UO"].ToString();
                        }
                        if (ruoloRow["VAR_COD_UO"] != null && !ruoloRow["VAR_COD_UO"].ToString().Equals(""))
                        {
                            ruolo.uo.codiceRubrica = ruoloRow["VAR_COD_UO"].ToString();
                        }
                    }

                    if (ruoloRow["CHA_TIPO_URP"] != null && !ruoloRow["CHA_TIPO_URP"].ToString().Equals(""))
                    {
                        ruolo.tipoCorrispondente = ruoloRow["CHA_TIPO_URP"].ToString();
                    }

                    if (ruoloRow["CHA_TIPO_IE"] != null && !ruoloRow["CHA_TIPO_IE"].ToString().Equals(""))
                    {
                        ruolo.tipoIE = ruoloRow["CHA_TIPO_IE"].ToString();
                    }
                    ruolo.idGruppo = ruoloRow["PERSONORGROUP"].ToString();
                    dirittoOggetto.idObj = idProfile;
                    dirittoOggetto.soggetto = ruolo;
                    dirittoOggetto.tipoDiritto = getDiritto(ruoloRow["CHA_TIPO_DIRITTO"].ToString());
                    dirittoOggetto.accessRights = Convert.ToInt32(ruoloRow["ACCESSRIGHTS"]);
                    dirittoOggetto.deleted = false;
                    dirittoOggetto.personorgroup = ruoloRow["PERSONORGROUP"].ToString();

                    if (ruoloRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
                        dirittoOggetto.hideDocVersions = (ruoloRow["HIDE_DOC_VERSIONS"].ToString() == "1");

                    if (ruoloRow["TS_INSERIMENTO"] != DBNull.Value)
                        dirittoOggetto.dtaInsSecurity = ruoloRow["TS_INSERIMENTO"].ToString();
                    if (ruoloRow["VAR_NOTE_SEC"] != DBNull.Value)
                        dirittoOggetto.noteSecurity = ruoloRow["VAR_NOTE_SEC"].ToString();

                    

                    listaDiritti.Add(dirittoOggetto);
                    logger.Debug("Ruolo inserito");
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione DIRITTI_RUOLI SEMPLIFICATI", e);
                listaDiritti = null;
                return listaDiritti;
            }

            //2-- inserimento utenti
            try
            {
                //VERONICA IDAMM potrebbe essere vuoto
                if (IDAMM == "")
                {
                    IDAMM = infoUtente.idAmministrazione;
                }

                utenti.GetListaRuoliSemplificato(out dataSet, idProfile, IDAMM);

                logger.Debug("Utenti:" + dataSet.Tables["DIRITTI_UTENTI"].Rows.Count);

                foreach (DataRow utenteRow in dataSet.Tables["DIRITTI_UTENTI"].Rows)
                {
                    DocsPaVO.utente.Utente utenteTemp = new DocsPaVO.utente.Utente();
                    DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto();
                    DocsPaVO.documento.TipoDiritto tipoDiritto = getDiritto(utenteRow["CHA_TIPO_DIRITTO"].ToString());
                    utenteTemp.codiceRubrica = utenteRow["VAR_COD_RUBRICA"].ToString();
                    if (utenteRow["ID_REGISTRO"] != null && !utenteRow["ID_REGISTRO"].ToString().Equals(""))
                    {
                        string reg = utenteRow["ID_REGISTRO"].ToString();
                        utenteTemp.idRegistro = reg;
                    }
                    if (utenteRow["ID_AMM"] != null && !utenteRow["ID_AMM"].ToString().Equals(""))
                    {
                        utenteTemp.idAmministrazione = utenteRow["ID_AMM"].ToString();
                    }

                    if (utenteRow["VAR_DESC_CORR"] != null && !utenteRow["VAR_DESC_CORR"].ToString().Equals(""))
                    {
                        utenteTemp.descrizione = utenteRow["VAR_DESC_CORR"].ToString();
                    }

                    if (utenteRow["CHA_TIPO_URP"] != null && !utenteRow["CHA_TIPO_URP"].ToString().Equals(""))
                    {
                        utenteTemp.tipoCorrispondente = utenteRow["CHA_TIPO_URP"].ToString();
                    }

                    if (utenteRow["CHA_TIPO_IE"] != null && !utenteRow["CHA_TIPO_IE"].ToString().Equals(""))
                    {
                        utenteTemp.tipoIE = utenteRow["CHA_TIPO_IE"].ToString();
                    }

                    utenteTemp.idPeople = utenteRow["PERSONORGROUP"].ToString();

                    dirittoOggetto.idObj = idProfile;
                    dirittoOggetto.soggetto = utenteTemp;
                    dirittoOggetto.tipoDiritto = getDiritto(utenteRow["CHA_TIPO_DIRITTO"].ToString());
                    dirittoOggetto.accessRights = Convert.ToInt32(utenteRow["ACCESSRIGHTS"]);
                    dirittoOggetto.deleted = false;
                    dirittoOggetto.personorgroup = utenteRow["PERSONORGROUP"].ToString();

                    if (utenteRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
                        dirittoOggetto.hideDocVersions = (utenteRow["HIDE_DOC_VERSIONS"].ToString() == "1");

                    if (dirittoOggetto.tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_DELEGATO))
                    {
                        dirittoOggetto.soggetto = utenteTemp;
                        dirittoOggetto.soggetto.descrizione = utenteTemp.descrizione + " delegato da " + utenteTemp.descrizione;
                    }
                    else
                        dirittoOggetto.soggetto = utenteTemp;

                    listaDiritti.Add(dirittoOggetto);
                    logger.Debug("Utente inserito");
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione DIRITTI_UTENTI", e);
                listaDiritti = null;
                return listaDiritti;
            }

            //3-- inserimento ruoli rimossi
            //gestione diritti rimossi
            if (cercaRimossi)
            {
                try
                {
                    utenti.GetListaDiritti_Deleted_Semplificato(out dataSet, idProfile);
                    foreach (DataRow ruoloRow in dataSet.Tables["DIRITTI_RUOLI_DELETED"].Rows)
                    {
                        DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto();
                        DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                        ruolo.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();

                        if (ruoloRow["ID_REGISTRO"] != null && !ruoloRow["ID_REGISTRO"].ToString().Equals(""))
                        {
                            System.Collections.ArrayList reg = new System.Collections.ArrayList();
                            reg.Add(ruoloRow["ID_REGISTRO"].ToString());
                            ruolo.registri = reg;
                        }
                        if (ruoloRow["ID_AMM"] != null && !ruoloRow["ID_AMM"].ToString().Equals(""))
                        {
                            ruolo.idAmministrazione = ruoloRow["ID_AMM"].ToString();
                            IDAMM = ruolo.idAmministrazione;
                        }

                        if (ruoloRow["VAR_DESC_CORR"] != null && !ruoloRow["VAR_DESC_CORR"].ToString().Equals(""))
                        {
                            ruolo.descrizione = ruoloRow["VAR_DESC_CORR"].ToString();
                        }

                        if (ruoloRow["ID_UO"] != null && !ruoloRow["ID_UO"].ToString().Equals(""))
                        {
                            ruolo.uo = new DocsPaVO.utente.UnitaOrganizzativa();
                            ruolo.uo.systemId = ruoloRow["ID_UO"].ToString();
                            ruolo.uo.idAmministrazione = IDAMM;
                            if (ruoloRow["DESC_UO"] != null && !ruoloRow["DESC_UO"].ToString().Equals(""))
                            {
                                ruolo.uo.descrizione = ruoloRow["DESC_UO"].ToString();
                            }
                            if (ruoloRow["CHA_TIPO_UO"] != null && !ruoloRow["CHA_TIPO_UO"].ToString().Equals(""))
                            {
                                ruolo.uo.tipoIE = ruoloRow["CHA_TIPO_UO"].ToString();
                            }
                            if (ruoloRow["VAR_COD_UO"] != null && !ruoloRow["VAR_COD_UO"].ToString().Equals(""))
                            {
                                ruolo.uo.codiceRubrica = ruoloRow["VAR_COD_UO"].ToString();
                            }
                        }

                        if (ruoloRow["CHA_TIPO_URP"] != null && !ruoloRow["CHA_TIPO_URP"].ToString().Equals(""))
                        {
                            ruolo.tipoCorrispondente = ruoloRow["CHA_TIPO_URP"].ToString();
                        }

                        if (ruoloRow["CHA_TIPO_IE"] != null && !ruoloRow["CHA_TIPO_IE"].ToString().Equals(""))
                        {
                            ruolo.tipoIE = ruoloRow["CHA_TIPO_IE"].ToString();
                        }

                        ruolo.idGruppo = ruoloRow["PERSONORGROUP"].ToString();
                        dirittoOggetto.idObj = idProfile;
                        dirittoOggetto.soggetto = ruolo;
                        dirittoOggetto.tipoDiritto = getDiritto(ruoloRow["CHA_TIPO_DIRITTO"].ToString());
                        dirittoOggetto.accessRights = Convert.ToInt32(ruoloRow["ACCESSRIGHTS"]);
                        dirittoOggetto.deleted = true;
                        dirittoOggetto.personorgroup = ruoloRow["PERSONORGROUP"].ToString();

                        if (ruoloRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
                            dirittoOggetto.hideDocVersions = (ruoloRow["HIDE_DOC_VERSIONS"].ToString() == "1");

                        listaDiritti.Add(dirittoOggetto);
                        logger.Debug("Ruolo rimosso inserito");
                    }
                }
                catch (Exception e)
                {
                    logger.Debug("Errore nella gestione DIRITTI_RUOLI_DELETED", e);
                    listaDiritti = null;
                    return listaDiritti;
                }

                //4-- inserimento utenti rimossi
                try
                {
                    //VERONICA IDAMM potrebbe essere vuoto
                    if (IDAMM == "")
                    {
                        IDAMM = infoUtente.idAmministrazione;
                    }
                    utenti.GetListaRuoli_Deleted(out dataSet, idProfile, IDAMM);

                    logger.Debug("Utenti:" + dataSet.Tables["DIRITTI_UTENTI_DELETED"].Rows.Count);
                    foreach (DataRow utenteRow in dataSet.Tables["DIRITTI_UTENTI_DELETED"].Rows)
                    {
                        DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto();
                        DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                        DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                        qco.codiceRubrica = utenteRow["VAR_COD_RUBRICA"].ToString();
                        qco.getChildren = false;
                        if (utenteRow["ID_REGISTRO"] != null && !utenteRow["ID_REGISTRO"].ToString().Equals(""))
                        {
                            System.Collections.ArrayList reg = new System.Collections.ArrayList();
                            reg.Add(utenteRow["ID_REGISTRO"].ToString());
                            qco.idRegistri = reg;
                        }
                        if (utenteRow["ID_AMM"] != null && !utenteRow["ID_AMM"].ToString().Equals(""))
                        {
                            qco.idAmministrazione = utenteRow["ID_AMM"].ToString();
                        }
                        qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                        System.Collections.ArrayList utentiInt = new ArrayList();
                        utentiInt = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco);
                        if (utentiInt != null && utentiInt.Count > 0)
                        {
                            utente = (DocsPaVO.utente.Utente)utentiInt[0];
                            dirittoOggetto.idObj = idProfile;
                            dirittoOggetto.soggetto = utente;
                            dirittoOggetto.tipoDiritto = getDiritto(utenteRow["CHA_TIPO_DIRITTO"].ToString());
                            dirittoOggetto.accessRights = Convert.ToInt32(utenteRow["ACCESSRIGHTS"]);
                            dirittoOggetto.deleted = true;
                            dirittoOggetto.note = utenteRow["NOTE"].ToString();
                            dirittoOggetto.personorgroup = utenteRow["PERSONORGROUP"].ToString();

                            if (utenteRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
                                dirittoOggetto.hideDocVersions = (utenteRow["HIDE_DOC_VERSIONS"].ToString() == "1");

                            listaDiritti.Add(dirittoOggetto);
                            logger.Debug("Utente rimosso inserito");
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Debug("Errore nella gestione DIRITTI_UTENTI_DELETED", e);
                    listaDiritti = null;
                    return listaDiritti;
                }
            }
            return listaDiritti;
        }

        /// <summary>
        /// </summary>
        /// <param name="infoFasc"></param>
        /// <returns></returns>
        public static System.Collections.ArrayList getListaDirittiFascSemplificata(DocsPaVO.fascicolazione.InfoFascicolo infoFasc, bool cercaRimossi, string rootFolder)
        {
            //DocsPaWS.Utils.Database db=DocsPaWS.Utils.dbControl.getDatabase();			
            //db.openConnection();
            System.Collections.ArrayList listaDiritti = new ArrayList();
            try
            {
                listaDiritti = BusinessLogic.Fascicoli.ProjectsManager.getVisibilitaSemplificata(/*db,*/ infoFasc.idFascicolo, cercaRimossi, rootFolder);
                //ritorna la lista dei diritti
                //db.closeConnection();
                logger.Debug("Fine metodo");

            }
            catch (Exception e)
            {
                //db.closeConnection();
                logger.Debug("Errore nella gestione degli utenti (getListaDirittiFascSemplificata)", e);
                throw e;
            }
            return listaDiritti;
        }

		public static System.Collections.ArrayList getListaDirittiSemplificataWithFilter(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, bool cercaRimossi, DocsPaVO.filtri.FilterVisibility[] filters)
		{
			DataSet dataSet = new DataSet();
			ArrayList listaDiritti = new ArrayList();
			string IDAMM = string.Empty;
			DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
			//bool proprietario = false;
            // Modifica 14-05-2013
            // Descrizioni del delegante e delegato 
            string descDelegante = string.Empty;
            string descDelegato = string.Empty;
            string personorgroupsDelegato = string.Empty;

			//1-- inserimento ruoli
			try
			{
				utenti.GetListaDirittiSemplificataRuoliWithFilter(out dataSet, idProfile, filters,infoUtente); //, out proprietario);

				foreach (DataRow ruoloRow in dataSet.Tables["DIRITTI_RUOLI"].Rows)
				{
					DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto();
					DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
					DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
					DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
					ruolo.dta_fine = ruoloRow["dta_fine"].ToString();
					ruolo.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();

                     //Visualizzazione del pulsante della storia del ruolo se il ruolo ha una storia
					ruolo.ShowHistory = ruoloRow["ShowHistory"] != DBNull.Value ? ruoloRow["ShowHistory"].ToString() : String.Empty;

					// Impostazione dell'id del ruolo
					ruolo.systemId = ruoloRow["system_id"].ToString();

					if (ruoloRow["ID_AMM"] != null && !string.IsNullOrEmpty(ruoloRow["ID_AMM"].ToString()))
					{
						ruolo.idAmministrazione = ruoloRow["ID_AMM"].ToString();
						IDAMM = ruolo.idAmministrazione;
					}

					if (ruoloRow["VAR_DESC_CORR"] != null && !string.IsNullOrEmpty(ruoloRow["VAR_DESC_CORR"].ToString()))
					{
						if (ruoloRow["CHA_TIPO_URP"].ToString() == "R")
							ruolo.descrizione = ruoloRow["VAR_DESC_CORR"].ToString();
						else if (ruoloRow["CHA_TIPO_URP"].ToString() == "P")
							utente.descrizione = ruoloRow["VAR_DESC_CORR"].ToString();
					}

					if (ruoloRow["ID_UO"] != null && !string.IsNullOrEmpty(ruoloRow["ID_UO"].ToString()))
					{
						ruolo.uo = new DocsPaVO.utente.UnitaOrganizzativa();
						ruolo.uo.systemId = ruoloRow["ID_UO"].ToString();
						ruolo.uo.idAmministrazione = IDAMM;
						if (ruoloRow["var_desc_corr"] != null && !string.IsNullOrEmpty(ruoloRow["var_desc_corr"].ToString()))
						{
							ruolo.uo.descrizione = ruoloRow["var_desc_corr"].ToString();
						}
						if (ruoloRow["cha_tipo_ie"] != null && !string.IsNullOrEmpty(ruoloRow["cha_tipo_ie"].ToString()))
						{
							ruolo.uo.tipoIE = ruoloRow["cha_tipo_ie"].ToString();
						}
						if (ruoloRow["var_cod_rubrica"] != null && !string.IsNullOrEmpty(ruoloRow["var_cod_rubrica"].ToString()))
						{
							ruolo.uo.codiceRubrica = ruoloRow["var_cod_rubrica"].ToString();
						}
					}

					if (ruoloRow["CHA_TIPO_URP"] != null && !string.IsNullOrEmpty(ruoloRow["CHA_TIPO_URP"].ToString()))
					{
						if (ruoloRow["CHA_TIPO_URP"].ToString() == "R")
						{
							dirittoOggetto.soggetto = ruolo;
							ruolo.tipoCorrispondente = ruoloRow["CHA_TIPO_URP"].ToString();
                            if (ruoloRow["desc_uo"] != null && !string.IsNullOrEmpty(ruoloRow["desc_uo"].ToString()))
                            {
                                ruolo.uo.descrizione = ruoloRow["desc_uo"].ToString();
                            }
						}
						else if (ruoloRow["CHA_TIPO_URP"].ToString() == "P")
						{
							dirittoOggetto.soggetto = utente;
							utente.tipoCorrispondente = ruoloRow["CHA_TIPO_URP"].ToString();
						}
					}

					if (ruoloRow["CHA_TIPO_IE"] != null && !string.IsNullOrEmpty(ruoloRow["CHA_TIPO_IE"].ToString()))
					{
						ruolo.tipoIE = ruoloRow["CHA_TIPO_IE"].ToString();
					}

					if (ruoloRow["CHA_TIPO_URP"].ToString() == "R")
						ruolo.idGruppo = ruoloRow["PERSONORGROUP"].ToString();
					else if(ruoloRow["CHA_TIPO_URP"].ToString() == "P")
						utente.idPeople = ruoloRow["PERSONORGROUP"].ToString();

					dirittoOggetto.idObj = idProfile;
					dirittoOggetto.tipoDiritto = getDiritto(ruoloRow["CHA_TIPO_DIRITTO"].ToString());
					dirittoOggetto.accessRights = Convert.ToInt32(ruoloRow["ACCESSRIGHTS"]);
					dirittoOggetto.deleted = false;

					dirittoOggetto.removed = ruoloRow["RIMOSSO"].ToString();
					if (dirittoOggetto.removed == "1")
					{
						dirittoOggetto.deleted = true;
                        //if (ruoloRow["HIDE_DOC_VERSIONS"] != null && ruoloRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
                        //    dirittoOggetto.hideDocVersions = (ruoloRow["HIDE_DOC_VERSIONS"].ToString() == "1");
					}

					dirittoOggetto.personorgroup = ruoloRow["PERSONORGROUP"].ToString();

                    //if (ruoloRow["HIDE_DOC_VERSIONS"] != null && ruoloRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
                    //    dirittoOggetto.hideDocVersions = (ruoloRow["HIDE_DOC_VERSIONS"].ToString() == "1");

					if (ruoloRow["TS_INSERIMENTO"] != DBNull.Value)
						dirittoOggetto.dtaInsSecurity = ruoloRow["TS_INSERIMENTO"].ToString();
                    if (ruoloRow["note"] != DBNull.Value)
                        dirittoOggetto.noteSecurity = ruoloRow["note"].ToString();

                    // Modifica 14-05-2013
                    // Verifico della presenza di un delegato:
                    // Memorizzo le informazioni del Delegante/Proprietario e del Delegato
                    // Presumo che esista solo un delegante e un delegato
                    // Delegante:
                    if (ruoloRow["CHA_TIPO_DIRITTO"].ToString() == "P" && ruoloRow["ACCESSRIGHTS"].ToString() == "0")
                        descDelegante = ruoloRow["VAR_DESC_CORR"].ToString();
                    // Delegato:
                    if (ruoloRow["CHA_TIPO_DIRITTO"].ToString() == "D")
                    {
                        descDelegato = ruoloRow["VAR_DESC_CORR"].ToString();
                        personorgroupsDelegato = ruoloRow["PERSONORGROUP"].ToString();
                    }
                    dirittoOggetto.DiSistema = ruoloRow["diSistema"].ToString();

                    dirittoOggetto.CopiaVisibilita = ruoloRow.Table.Columns.Contains("CHA_COPIA_VISIBILITA") ? ruoloRow["CHA_COPIA_VISIBILITA"].ToString() : "0";

					listaDiritti.Add(dirittoOggetto);
					logger.Debug("Ruolo inserito");
				}
			}
			catch (Exception e)
			{
				logger.Debug("Errore nella gestione DIRITTI_RUOLI SEMPLIFICATI", e);
				listaDiritti = null;
				return listaDiritti;
			}

			//2-- inserimento utenti
			//try
			//{
			//    //VERONICA IDAMM potrebbe essere vuoto
			//    if (IDAMM == "")
			//    {
			//        IDAMM = infoUtente.idAmministrazione;
			//    }
			
				//if (proprietario == true)
				//{
				//    utenti.GetListaRuoliSemplificato(out dataSet, idProfile, IDAMM);

				//    logger.Debug("Utenti:" + dataSet.Tables["DIRITTI_UTENTI"].Rows.Count);

				//    foreach (DataRow utenteRow in dataSet.Tables["DIRITTI_UTENTI"].Rows)
				//    {
				//        DocsPaVO.utente.Utente utenteTemp = new DocsPaVO.utente.Utente();
				//        DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto();
				//        DocsPaVO.documento.TipoDiritto tipoDiritto = getDiritto(utenteRow["CHA_TIPO_DIRITTO"].ToString());
				//        utenteTemp.codiceRubrica = utenteRow["VAR_COD_RUBRICA"].ToString();
				//        if (utenteRow["ID_REGISTRO"] != null && !utenteRow["ID_REGISTRO"].ToString().Equals(""))
				//        {
				//            string reg = utenteRow["ID_REGISTRO"].ToString();
				//            utenteTemp.idRegistro = reg;
				//        }
				//        if (utenteRow["ID_AMM"] != null && !utenteRow["ID_AMM"].ToString().Equals(""))
				//        {
				//            utenteTemp.idAmministrazione = utenteRow["ID_AMM"].ToString();
				//        }

				//        if (utenteRow["VAR_DESC_CORR"] != null && !utenteRow["VAR_DESC_CORR"].ToString().Equals(""))
				//        {
				//            utenteTemp.descrizione = utenteRow["VAR_DESC_CORR"].ToString();
				//        }

				//        if (utenteRow["CHA_TIPO_URP"] != null && !utenteRow["CHA_TIPO_URP"].ToString().Equals(""))
				//        {
				//            utenteTemp.tipoCorrispondente = utenteRow["CHA_TIPO_URP"].ToString();
				//        }

				//        if (utenteRow["CHA_TIPO_IE"] != null && !utenteRow["CHA_TIPO_IE"].ToString().Equals(""))
				//        {
				//            utenteTemp.tipoIE = utenteRow["CHA_TIPO_IE"].ToString();
				//        }

				//        utenteTemp.idPeople = utenteRow["PERSONORGROUP"].ToString();

				//        dirittoOggetto.idObj = idProfile;
				//        dirittoOggetto.soggetto = utenteTemp;
				//        dirittoOggetto.tipoDiritto = getDiritto(utenteRow["CHA_TIPO_DIRITTO"].ToString());
				//        dirittoOggetto.accessRights = Convert.ToInt32(utenteRow["ACCESSRIGHTS"]);
				//        dirittoOggetto.deleted = false;
				//        dirittoOggetto.personorgroup = utenteRow["PERSONORGROUP"].ToString();

				//        if (utenteRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
				//            dirittoOggetto.hideDocVersions = (utenteRow["HIDE_DOC_VERSIONS"].ToString() == "1");

				//        if (dirittoOggetto.tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_DELEGATO))
				//        {
				//            dirittoOggetto.soggetto = utenteTemp;
				//            dirittoOggetto.soggetto.descrizione = utenteTemp.descrizione + " delegato da " + utenteTemp.descrizione;
				//        }
				//        else
				//            dirittoOggetto.soggetto = utenteTemp;

				//        listaDiritti.Add(dirittoOggetto);
				//        logger.Debug("Utente inserito");
				//    }

				//}
			//}
			//catch (Exception e)
			//{
			//    logger.Debug("Errore nella gestione DIRITTI_UTENTI", e);
			//    listaDiritti = null;
			//    return listaDiritti;
			//}

			//3-- inserimento ruoli rimossi
			//gestione diritti rimossi
			if (cercaRimossi)
			{
			//    try
			//    {
			//        utenti.GetListaDiritti_Deleted_Semplificato(out dataSet, idProfile);
			//        foreach (DataRow ruoloRow in dataSet.Tables["DIRITTI_RUOLI_DELETED"].Rows)
			//        {
			//            DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto();
			//            DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
			//            ruolo.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();

			//            if (ruoloRow["ID_REGISTRO"] != null && !ruoloRow["ID_REGISTRO"].ToString().Equals(""))
			//            {
			//                System.Collections.ArrayList reg = new System.Collections.ArrayList();
			//                reg.Add(ruoloRow["ID_REGISTRO"].ToString());
			//                ruolo.registri = reg;
			//            }
			//            if (ruoloRow["ID_AMM"] != null && !ruoloRow["ID_AMM"].ToString().Equals(""))
			//            {
			//                ruolo.idAmministrazione = ruoloRow["ID_AMM"].ToString();
			//                IDAMM = ruolo.idAmministrazione;
			//            }

			//            if (ruoloRow["VAR_DESC_CORR"] != null && !ruoloRow["VAR_DESC_CORR"].ToString().Equals(""))
			//            {
			//                ruolo.descrizione = ruoloRow["VAR_DESC_CORR"].ToString();
			//            }

			//            if (ruoloRow["ID_UO"] != null && !ruoloRow["ID_UO"].ToString().Equals(""))
			//            {
			//                ruolo.uo = new DocsPaVO.utente.UnitaOrganizzativa();
			//                ruolo.uo.systemId = ruoloRow["ID_UO"].ToString();
			//                ruolo.uo.idAmministrazione = IDAMM;
			//                if (ruoloRow["DESC_UO"] != null && !ruoloRow["DESC_UO"].ToString().Equals(""))
			//                {
			//                    ruolo.uo.descrizione = ruoloRow["DESC_UO"].ToString();
			//                }
			//                if (ruoloRow["CHA_TIPO_UO"] != null && !ruoloRow["CHA_TIPO_UO"].ToString().Equals(""))
			//                {
			//                    ruolo.uo.tipoIE = ruoloRow["CHA_TIPO_UO"].ToString();
			//                }
			//                if (ruoloRow["VAR_COD_UO"] != null && !ruoloRow["VAR_COD_UO"].ToString().Equals(""))
			//                {
			//                    ruolo.uo.codiceRubrica = ruoloRow["VAR_COD_UO"].ToString();
			//                }
			//            }

			//            if (ruoloRow["CHA_TIPO_URP"] != null && !ruoloRow["CHA_TIPO_URP"].ToString().Equals(""))
			//            {
			//                ruolo.tipoCorrispondente = ruoloRow["CHA_TIPO_URP"].ToString();
			//            }

			//            if (ruoloRow["CHA_TIPO_IE"] != null && !ruoloRow["CHA_TIPO_IE"].ToString().Equals(""))
			//            {
			//                ruolo.tipoIE = ruoloRow["CHA_TIPO_IE"].ToString();
			//            }

			//            ruolo.idGruppo = ruoloRow["PERSONORGROUP"].ToString();
			//            dirittoOggetto.idObj = idProfile;
			//            dirittoOggetto.soggetto = ruolo;
			//            dirittoOggetto.tipoDiritto = getDiritto(ruoloRow["CHA_TIPO_DIRITTO"].ToString());
			//            dirittoOggetto.accessRights = Convert.ToInt32(ruoloRow["ACCESSRIGHTS"]);
			//            dirittoOggetto.deleted = true;
			//            dirittoOggetto.personorgroup = ruoloRow["PERSONORGROUP"].ToString();

			//            if (ruoloRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
			//                dirittoOggetto.hideDocVersions = (ruoloRow["HIDE_DOC_VERSIONS"].ToString() == "1");

			//            listaDiritti.Add(dirittoOggetto);
			//            logger.Debug("Ruolo rimosso inserito");
			//        }
			//    }
			//    catch (Exception e)
			//    {
			//        logger.Debug("Errore nella gestione DIRITTI_RUOLI_DELETED", e);
			//        listaDiritti = null;
			//        return listaDiritti;
			//    }

				//4-- inserimento utenti rimossi
				//try
				//{
				//    //VERONICA IDAMM potrebbe essere vuoto
				//    if (IDAMM == "")
				//    {
				//        IDAMM = infoUtente.idAmministrazione;
				//    }
				//    //utenti.GetListaRuoli_Deleted(out dataSet, idProfile, IDAMM);

				//    logger.Debug("Utenti:" + dataSet.Tables["DIRITTI_RUOLI"].Rows.Count);
				//    foreach (DataRow utenteRow in dataSet.Tables["DIRITTI_RUOLI"].Rows)
				//    {
				//        DocsPaVO.documento.DirittoOggetto dirittoOggetto = new DocsPaVO.documento.DirittoOggetto();
				//        DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
				//        DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
				//        qco.codiceRubrica = utenteRow["VAR_COD_RUBRICA"].ToString();
				//        qco.getChildren = false;
				//        if (utenteRow["ID_REGISTRO"] != null && !utenteRow["ID_REGISTRO"].ToString().Equals(""))
				//        {
				//            System.Collections.ArrayList reg = new System.Collections.ArrayList();
				//            reg.Add(utenteRow["ID_REGISTRO"].ToString());
				//            qco.idRegistri = reg;
				//        }
				//        if (utenteRow["ID_AMM"] != null && !utenteRow["ID_AMM"].ToString().Equals(""))
				//        {
				//            qco.idAmministrazione = utenteRow["ID_AMM"].ToString();
				//        }

				//        qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
				//        System.Collections.ArrayList utentiInt = new ArrayList();
				//        utentiInt = BusinessLogic.Utenti.addressBookManager.listaCorrispondentiIntMethod(qco);
				//        if (utentiInt != null && utentiInt.Count > 0)
				//        {
				//            utente = (DocsPaVO.utente.Utente)utentiInt[0];
				//            dirittoOggetto.idObj = idProfile;
				//            dirittoOggetto.soggetto = utente;
				//            dirittoOggetto.tipoDiritto = getDiritto(utenteRow["CHA_TIPO_DIRITTO"].ToString());
				//            dirittoOggetto.accessRights = Convert.ToInt32(utenteRow["ACCESSRIGHTS"]);
				//            dirittoOggetto.deleted = true;
				//            dirittoOggetto.note = utenteRow["var_note_sec"].ToString();
				//            dirittoOggetto.personorgroup = utenteRow["PERSONORGROUP"].ToString();

				//            if (utenteRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
				//                dirittoOggetto.hideDocVersions = (utenteRow["HIDE_DOC_VERSIONS"].ToString() == "1");

				//            listaDiritti.Add(dirittoOggetto);
				//            logger.Debug("Utente rimosso inserito");
				//        }
				//    }
				//}
				//catch (Exception e)
				//{
				//    logger.Debug("Errore nella gestione DIRITTI_UTENTI_DELETED", e);
				//    listaDiritti = null;
				//    return listaDiritti;
				//}
			}
            //
            // Modifica 14-05-2013
            // Delegante - Delegato
            // Solo se il delegato è presente faccio l'aggiornamento dell'elemento nella lista
            if (!string.IsNullOrEmpty(descDelegato) && !string.IsNullOrEmpty(descDelegante))
            {
                // Ricerco all'interno della lista il delgato
                foreach (DocsPaVO.documento.DirittoOggetto d in listaDiritti)
                {
                    if (d.tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_DELEGATO) && (!string.IsNullOrEmpty(personorgroupsDelegato) && d.personorgroup.Equals(personorgroupsDelegato)))
                        d.soggetto.descrizione = d.soggetto.descrizione + " delegato da " + descDelegante;
                }
            }

			return listaDiritti;
		}

        public static System.Collections.ArrayList getListaDirittiSemplificataWithFilterFasc(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, bool cercaRimossi, DocsPaVO.filtri.FilterVisibility[] filters, string rootFolder)
        {
            DataSet dataSet = new DataSet();
            ArrayList listaDiritti = new ArrayList();
            string IDAMM = string.Empty;
            DocsPaDB.Query_DocsPAWS.Utenti utenti = new DocsPaDB.Query_DocsPAWS.Utenti();
            //bool proprietario = false;
            // Modifica 14-05-2013
            // Descrizioni del delegante e delegato 
            string descDelegante = string.Empty;
            string descDelegato = string.Empty;
            string personorgroupsDelegato = string.Empty;

            //1-- inserimento ruoli
            try
            {
                utenti.GetListaDirittiSemplificataRuoliWithFilter(out dataSet, idProfile, filters, infoUtente); //, out proprietario);

                foreach (DataRow ruoloRow in dataSet.Tables["DIRITTI_RUOLI"].Rows)
                {
                    DocsPaVO.fascicolazione.DirittoOggetto dirittoOggetto = new DocsPaVO.fascicolazione.DirittoOggetto();
                    DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                    DocsPaVO.utente.Utente utente = new DocsPaVO.utente.Utente();
                    DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                    ruolo.dta_fine = ruoloRow["dta_fine"].ToString();
                    ruolo.codiceRubrica = ruoloRow["VAR_COD_RUBRICA"].ToString();

                    //Visualizzazione del pulsante della storia del ruolo se il ruolo ha una storia
                    ruolo.ShowHistory = ruoloRow["ShowHistory"] != DBNull.Value ? ruoloRow["ShowHistory"].ToString() : String.Empty;

                    // Impostazione dell'id del ruolo
                    ruolo.systemId = ruoloRow["system_id"].ToString();

                    if (ruoloRow["ID_AMM"] != null && !string.IsNullOrEmpty(ruoloRow["ID_AMM"].ToString()))
                    {
                        ruolo.idAmministrazione = ruoloRow["ID_AMM"].ToString();
                        IDAMM = ruolo.idAmministrazione;
                    }

                    if (ruoloRow["VAR_DESC_CORR"] != null && !string.IsNullOrEmpty(ruoloRow["VAR_DESC_CORR"].ToString()))
                    {
                        if (ruoloRow["CHA_TIPO_URP"].ToString() == "R")
                            ruolo.descrizione = ruoloRow["VAR_DESC_CORR"].ToString();
                        else if (ruoloRow["CHA_TIPO_URP"].ToString() == "P")
                            utente.descrizione = ruoloRow["VAR_DESC_CORR"].ToString();
                    }

                    if (ruoloRow["ID_UO"] != null && !string.IsNullOrEmpty(ruoloRow["ID_UO"].ToString()))
                    {
                        ruolo.uo = new DocsPaVO.utente.UnitaOrganizzativa();
                        ruolo.uo.systemId = ruoloRow["ID_UO"].ToString();
                        ruolo.uo.idAmministrazione = IDAMM;
                        if (ruoloRow["var_desc_corr"] != null && !string.IsNullOrEmpty(ruoloRow["var_desc_corr"].ToString()))
                        {
                            ruolo.uo.descrizione = ruoloRow["var_desc_corr"].ToString();
                        }
                        if (ruoloRow["cha_tipo_ie"] != null && !string.IsNullOrEmpty(ruoloRow["cha_tipo_ie"].ToString()))
                        {
                            ruolo.uo.tipoIE = ruoloRow["cha_tipo_ie"].ToString();
                        }
                        if (ruoloRow["var_cod_rubrica"] != null && !string.IsNullOrEmpty(ruoloRow["var_cod_rubrica"].ToString()))
                        {
                            ruolo.uo.codiceRubrica = ruoloRow["var_cod_rubrica"].ToString();
                        }
                    }

                    if (ruoloRow["CHA_TIPO_URP"] != null && !string.IsNullOrEmpty(ruoloRow["CHA_TIPO_URP"].ToString()))
                    {
                        if (ruoloRow["CHA_TIPO_URP"].ToString() == "R")
                        {
                            dirittoOggetto.soggetto = ruolo;
                            ruolo.tipoCorrispondente = ruoloRow["CHA_TIPO_URP"].ToString();
                            if (ruoloRow["desc_uo"] != null && !string.IsNullOrEmpty(ruoloRow["desc_uo"].ToString()))
                            {
                                ruolo.uo.descrizione = ruoloRow["desc_uo"].ToString();
                            }
                        }
                        else if (ruoloRow["CHA_TIPO_URP"].ToString() == "P")
                        {
                            dirittoOggetto.soggetto = utente;
                            utente.tipoCorrispondente = ruoloRow["CHA_TIPO_URP"].ToString();
                        }
                    }

                    if (ruoloRow["CHA_TIPO_IE"] != null && !string.IsNullOrEmpty(ruoloRow["CHA_TIPO_IE"].ToString()))
                    {
                        ruolo.tipoIE = ruoloRow["CHA_TIPO_IE"].ToString();
                    }

                    if (ruoloRow["CHA_TIPO_URP"].ToString() == "R")
                        ruolo.idGruppo = ruoloRow["PERSONORGROUP"].ToString();
                    else if (ruoloRow["CHA_TIPO_URP"].ToString() == "P")
                        utente.idPeople = ruoloRow["PERSONORGROUP"].ToString();

                    dirittoOggetto.idObj = idProfile;
                    dirittoOggetto.tipoDiritto = getDirittoFasc(ruoloRow["CHA_TIPO_DIRITTO"].ToString());
                    dirittoOggetto.accessRights = Convert.ToInt32(ruoloRow["ACCESSRIGHTS"]);
                    dirittoOggetto.deleted = false;
                    dirittoOggetto.rootFolder = rootFolder;

                    dirittoOggetto.removed = ruoloRow["RIMOSSO"].ToString();
                    if (dirittoOggetto.removed == "1")
                    {
                        dirittoOggetto.deleted = true;
                        //if (ruoloRow["HIDE_DOC_VERSIONS"] != null && ruoloRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
                        //    dirittoOggetto.hideDocVersions = (ruoloRow["HIDE_DOC_VERSIONS"].ToString() == "1");
                    }

                    dirittoOggetto.personorgroup = ruoloRow["PERSONORGROUP"].ToString();

                    //if (ruoloRow["HIDE_DOC_VERSIONS"] != null && ruoloRow["HIDE_DOC_VERSIONS"] != DBNull.Value)
                    //    dirittoOggetto.hideDocVersions = (ruoloRow["HIDE_DOC_VERSIONS"].ToString() == "1");

                    if (ruoloRow["TS_INSERIMENTO"] != DBNull.Value)
                        dirittoOggetto.dtaInsSecurity = ruoloRow["TS_INSERIMENTO"].ToString();
                    if (ruoloRow["note"] != DBNull.Value)
                        dirittoOggetto.noteSecurity = ruoloRow["note"].ToString();

                    // Modifica 14-05-2013
                    // Verifico della presenza di un delegato:
                    // Memorizzo le informazioni del Delegante/Proprietario e del Delegato
                    // Presumo che esista solo un delegante e un delegato
                    // Delegante:
                    if (ruoloRow["CHA_TIPO_DIRITTO"].ToString() == "P" && ruoloRow["ACCESSRIGHTS"].ToString() == "0")
                        descDelegante = ruoloRow["VAR_DESC_CORR"].ToString();
                    // Delegato:
                    if (ruoloRow["CHA_TIPO_DIRITTO"].ToString() == "D")
                    {
                        descDelegato = ruoloRow["VAR_DESC_CORR"].ToString();
                        personorgroupsDelegato = ruoloRow["PERSONORGROUP"].ToString();
                    }

                    listaDiritti.Add(dirittoOggetto);
                    logger.Debug("Ruolo inserito");
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione DIRITTI_RUOLI SEMPLIFICATI", e);
                listaDiritti = null;
                return listaDiritti;
            }

            //3-- inserimento ruoli rimossi
            //gestione diritti rimossi
            if (cercaRimossi)
            {

            }
            //
            // Modifica 14-05-2013
            // Delegante - Delegato
            // Solo se il delegato è presente faccio l'aggiornamento dell'elemento nella lista
            if (!string.IsNullOrEmpty(descDelegato) && !string.IsNullOrEmpty(descDelegante))
            {
                // Ricerco all'interno della lista il delgato
                foreach (DocsPaVO.documento.DirittoOggetto d in listaDiritti)
                {
                    if (d.tipoDiritto.Equals(DocsPaVO.documento.TipoDiritto.TIPO_DELEGATO) && (!string.IsNullOrEmpty(personorgroupsDelegato) && d.personorgroup.Equals(personorgroupsDelegato)))
                        d.soggetto.descrizione = d.soggetto.descrizione + " delegato da " + descDelegante;
                }
            }

            return listaDiritti;
        }
    }
}
