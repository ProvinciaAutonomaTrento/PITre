// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Serilog;
using System.Collections;
using System.Data;

namespace BusinessLogic.Utenti;

public class DirittiManager
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(DirittiManager));
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
            DocsPaVO.utente.Corrispondente soggettoPropr = null;
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
                        dirittoOggetto.soggetto.descrizione = utente.descrizione + " sostituto di " + soggettoPropr.descrizione;
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

}
