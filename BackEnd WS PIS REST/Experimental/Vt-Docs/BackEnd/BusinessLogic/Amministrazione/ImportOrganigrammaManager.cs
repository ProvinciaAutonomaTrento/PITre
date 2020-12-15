using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DocsPaVO.amministrazione;
using log4net;
using DocsPaVO.utente;
using System.Collections;
using DocsPaVO.Validations;

namespace BusinessLogic.Amministrazione
{
    public class ImportOrganigrammaManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(OrganigrammaManager));

        public static EsitoOperazione ImportOrganigramma(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtenteAmm, DataSet dataSet)
        {
            DocsPaVO.amministrazione.EsitoOperazione esito = new DocsPaVO.amministrazione.EsitoOperazione();
            esito.Codice = 0;
            esito.Descrizione = "Operazione avvenuta con successo";
            DocsPaDB.Utils.SimpleLog simpleLog = null;

            //Creazione file di log specifico per l'importazione organigramma
            try
            {
                String logPath = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_LOG_PATH");
                
                logPath = logPath.Replace("\\%DATA", "");
                if (logPath.EndsWith("\\"))
                    simpleLog = new DocsPaDB.Utils.SimpleLog(logPath + "logImportazioneOrganigramma" + DateTime.Now.ToString("yyyyMMdd_hh_mm_ss"));
                else
                    simpleLog = new DocsPaDB.Utils.SimpleLog(logPath + "\\logImportazioneOrganigramma" + DateTime.Now.ToString("yyyyMMdd_hh_mm_ss"));
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in ImportOrganigrammaManager.cs  - Metodo: ImportOrganigramma - ", ex);
                esito.Codice = 1;
                esito.Descrizione = "ERROR - Errore nella creazione del file di log specifico per l'importazione organigramma, controllare i log dell'applicativo";
            }
            
            if (esito.Codice != 1)
            {
                //REGISTRI
                #region REGISTRI
                simpleLog.Log("");
                simpleLog.Log("**** Inizio importazione REGISTRI - " + System.DateTime.Now.ToString());
                try
                {
                    if (dataSet.Tables.Contains("REGISTRI"))
                    {
                        gestioneRegistri(infoUtenteAmm, dataSet, simpleLog);
                    }
                    else
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING - Tabella REGISTRI inesistente");
                    }
                }
                catch (Exception ex)
                {
                    esito.Codice = 1;
                    esito.Descrizione = "ERROR - Errore nella importazione dei REGISTRI, controllare il file di log";
                    simpleLog.Log("");
                    simpleLog.Log("ERROR - Errore nella importazione dei REGISTRI - " + ex.Message);
                }

                simpleLog.Log("");
                simpleLog.Log("**** Fine importazione REGISTRI - " + System.DateTime.Now.ToString());
                #endregion REGISTRI

                //UTENTI
                #region UTENTI
                simpleLog.Log("");
                simpleLog.Log("**** Inizio importazione UTENTI - " + System.DateTime.Now.ToString());
                try
                {
                    if (dataSet.Tables.Contains("UTENTI"))
                    {
                        gestioneUtenti(infoUtenteAmm, dataSet, simpleLog);
                    }
                    else
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING - Tabella UTENTI inesistente");
                    }
                }
                catch (Exception ex)
                {
                    esito.Codice = 1;
                    esito.Descrizione = "ERROR - Errore nella importazione degli UTENTI, controllare il file di log";
                    simpleLog.Log("");
                    simpleLog.Log("ERROR - Errore nella importazione degli UTENTI - " + ex.Message);
                }

                simpleLog.Log("");
                simpleLog.Log("**** Fine importazione UTENTI - " + System.DateTime.Now.ToString());
                #endregion UTENTI

                //UO
                #region UO
                simpleLog.Log("");
                simpleLog.Log("**** Inizio importazione UO - " + System.DateTime.Now.ToString());

                try
                {
                    if (dataSet.Tables.Contains("UO"))
                    {
                        gestioneUO(infoUtenteAmm, dataSet, simpleLog);
                    }
                    else
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING - Tabella UO inesistente");
                    }
                }
                catch (Exception ex)
                {
                    esito.Codice = 1;
                    esito.Descrizione = "ERROR - Errore nella importazione delle UO, controllare il file di log";
                    simpleLog.Log("");
                    simpleLog.Log("ERROR - Errore nella importazione delle UO - " + ex.Message);
                }

                simpleLog.Log("");
                simpleLog.Log("**** Fine importazione UO - " + System.DateTime.Now.ToString());
                #endregion UO

                //RUOLI
                #region RUOLI
                simpleLog.Log("");
                simpleLog.Log("**** Inizio importazione RUOLI - " + System.DateTime.Now.ToString());

                try
                {
                    if (dataSet.Tables.Contains("RUOLI"))
                    {
                        gestioneRuoli(infoUtenteAmm, dataSet, simpleLog);
                    }
                    else
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING - Tabella RUOLI inesistente");
                    }
                }
                catch (Exception ex)
                {
                    esito.Codice = 1;
                    esito.Descrizione = "ERROR - Errore nella importazione dei RUOLI, controllare il file di log";
                    simpleLog.Log("");
                    simpleLog.Log("ERROR - Errore nella importazioen dei RUOLI - " + ex.Message);
                }

                simpleLog.Log("");
                simpleLog.Log("**** Inizio importazione RUOLI - " + System.DateTime.Now.ToString());
                #endregion

                //ASSOCIAZIONE REGISTRI/RF A RUOLO
                #region ASSOCIAZIONE REGISTRI/RF A RUOLO
                simpleLog.Log("");
                simpleLog.Log("**** Inizio associazione registri/rf a RUOLO - " + System.DateTime.Now.ToString());

                try
                {
                    if (dataSet.Tables.Contains("RUOLI"))
                    {
                        assRegistriRfRuolo(infoUtenteAmm, dataSet, simpleLog);
                    }
                    else
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING - Tabella RUOLI inesistente");
                    }
                }
                catch (Exception ex)
                {
                    esito.Codice = 1;
                    esito.Descrizione = "ERROR - Errore nella associazione registri/rf a RUOLO, controllare il file di log";
                    simpleLog.Log("");
                    simpleLog.Log("ERROR - Errore nella associazione registri/rf a RUOLO - " + ex.Message);
                }

                simpleLog.Log("");
                simpleLog.Log("**** Fine associazione registri/rf a RUOLO - " + System.DateTime.Now.ToString());
                #endregion

                //ASSOCIAZIONE FUNZIONI A RUOLO
                #region ASSOCIAZIONE FUNZIONI A RUOLO
                simpleLog.Log("");
                simpleLog.Log("**** Inizio associazione funzioni a RUOLO - " + System.DateTime.Now.ToString());

                try
                {
                    if (dataSet.Tables.Contains("RUOLI"))
                    {
                        assFunzioniRuolo(infoUtenteAmm, dataSet, simpleLog);
                    }
                    else
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING - Tabella RUOLI inesistente");
                    }
                }
                catch (Exception ex)
                {
                    esito.Codice = 1;
                    esito.Descrizione = "ERROR - Errore nella associazione funzioni a RUOLO, controllare il file di log";
                    simpleLog.Log("");
                    simpleLog.Log("ERROR - Errore nella associazione funzioni a RUOLO - " + ex.Message);
                }

                simpleLog.Log("");
                simpleLog.Log("**** Fine associazione funzioni a RUOLO - " + System.DateTime.Now.ToString());
                #endregion

                //ASSOCIAZIONE UTENTI A RUOLO
                #region ASSOCIAZIONE UTENTI A RUOLO
                simpleLog.Log("");
                simpleLog.Log("**** Inizio associazione utenti a RUOLO - " + System.DateTime.Now.ToString());

                try
                {
                    if (dataSet.Tables.Contains("RUOLI"))
                    {
                        assUtentiRuolo(infoUtenteAmm, dataSet, simpleLog);
                    }
                    else
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING - Tabella RUOLI inesistente");
                    }
                }
                catch (Exception ex)
                {
                    esito.Codice = 1;
                    esito.Descrizione = "ERROR - Errore nella associazione utenti a RUOLO, controllare il file di log";
                    simpleLog.Log("");
                    simpleLog.Log("ERROR - Errore nella associazione utenti a RUOLO - " + ex.Message);
                }

                simpleLog.Log("");
                simpleLog.Log("**** Fine associazione utenti a RUOLO - " + System.DateTime.Now.ToString());
                #endregion

                //ASSOCIAZIONE QUALIFICHE A UTENTE
                #region ASSOCIAZIONE QUALIFICHE A UTENTE
                simpleLog.Log("");
                simpleLog.Log("**** Inizio associazione qualifiche a UTENTE - " + System.DateTime.Now.ToString());

                try
                {
                    if (dataSet.Tables.Contains("QUALIFICHE"))
                    {
                        assQualificheUtenti(infoUtenteAmm, dataSet, simpleLog);
                    }
                    else
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING - Tabella QUALIFICHE inesistente");
                    }
                }
                catch (Exception ex)
                {
                    esito.Codice = 1;
                    esito.Descrizione = "ERROR - Errore nella associazione qualifiche a UTENTE, controllare il file di log";
                    simpleLog.Log("");
                    simpleLog.Log("ERROR - Errore nella associazione qualifiche a UTENTE - " + ex.Message);
                }

                simpleLog.Log("");
                simpleLog.Log("**** Fine associazione qualifiche a UTENTE - " + System.DateTime.Now.ToString());
                #endregion

                //ASSOCIAZIONE RUOLO DI RIFERIMENTO AD UTENTE
                #region ASSOCIAZIONE RUOLO DI RIFERIMENTO AD UTENTE
                simpleLog.Log("");
                simpleLog.Log("**** Inizio associazione ruolo di riferimento a UTENTE - " + System.DateTime.Now.ToString());

                try
                {
                    if (dataSet.Tables.Contains("UTENTI"))
                    {
                        assRuoloRiferimentoUtente(infoUtenteAmm, dataSet, simpleLog);
                    }
                    else
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING - Tabella UTENTI inesistente");
                    }
                }
                catch (Exception ex)
                {
                    esito.Codice = 1;
                    esito.Descrizione = "ERROR - Errore nella associazione qualifiche a UTENTE, controllare il file di log";
                    simpleLog.Log("");
                    simpleLog.Log("ERROR - Errore nella associazione qualifiche a UTENTE - " + ex.Message);
                }

                simpleLog.Log("");
                simpleLog.Log("**** Fine associazione ruolo di riferimento a UTENTE - " + System.DateTime.Now.ToString());
                #endregion
            }

            
            return esito;
        }

        private static void gestioneUO(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtenteAmm, DataSet dataSet, DocsPaDB.Utils.SimpleLog simpleLog)
        {   
            foreach (DataRow dr in dataSet.Tables["UO"].Rows)
            {
                //Controllo campi obbligatori
                if(!String.IsNullOrEmpty(dr["COD_AMMINISTRAZIONE"].ToString()) && 
                   !String.IsNullOrEmpty(dr["LIVELLO_UO"].ToString()) &&
                   !String.IsNullOrEmpty(dr["CODICE_UO"].ToString()) &&
                   !String.IsNullOrEmpty(dr["DESCRIZIONE"].ToString()))
                {
                    //Recupero l'idAmministrazione e lo imposto all'infoUtenteAmministratore perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                    string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(dr["COD_AMMINISTRAZIONE"].ToString().ToUpper());
                    infoUtenteAmm.idAmministrazione = idAmm;

                    //Recupero la system_id dell'Amministrazione dalla DPA_CORR_GLOBALI
                    string idAmmCorrGlobali = Amministrazione.AmministraManager.getIdAmmCorrGlobali(infoUtenteAmm.idAmministrazione);

                    //Creo la UO
                    OrgUO uo = new OrgUO();

                    //Valorizzo i campi
                    if (String.IsNullOrEmpty(dr["COD_UO_PADRE"].ToString()))
                    {
                        uo.IDParent = idAmmCorrGlobali;
                    }
                    else
                    {
                        UnitaOrganizzativa uoPadre = (UnitaOrganizzativa)Utenti.UserManager.getCorrispondenteByCodRubrica(dr["COD_UO_PADRE"].ToString().ToUpper(), infoUtenteAmm);
                        if (uoPadre != null)
                        {
                            uo.IDParent = uoPadre.systemId;
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("ERROR (UO) - Codice UO padre " + dr["COD_UO_PADRE"].ToString() + " non trovato");
                            continue;
                        }
                    }

                    uo.Livello = dr["LIVELLO_UO"].ToString();
                    uo.Codice = dr["CODICE_UO"].ToString();
                    uo.CodiceRubrica = dr["CODICE_UO"].ToString();
                    uo.Descrizione = dr["DESCRIZIONE"].ToString();
                    uo.IDAmministrazione = infoUtenteAmm.idAmministrazione;
                    uo.CodiceRegistroInterop = dr["COD_REGISTRO_INTEROP"].ToString();

                    uo.DettagliUo = new OrgDettagliGlobali();
                    uo.DettagliUo.Indirizzo = dr["INDIRIZZO"].ToString();
                    uo.DettagliUo.Citta = dr["CITTA"].ToString();
                    uo.DettagliUo.Cap = dr["CAP"].ToString();

                    if (!string.IsNullOrEmpty(dr["PROVINCIA"].ToString()) && dr["PROVINCIA"].ToString().Length >= 2)
                        uo.DettagliUo.Provincia = dr["PROVINCIA"].ToString().Substring(0, 2);

                    uo.DettagliUo.Nazione = dr["NAZIONE"].ToString();
                    uo.DettagliUo.Telefono1 = dr["TELEFONO_1"].ToString();
                    uo.DettagliUo.Telefono2 = dr["TELEFONO_2"].ToString();
                    uo.DettagliUo.Fax = dr["FAX"].ToString();

                    //Inserimento
                    if(dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("I"))
                    {
                        EsitoOperazione esito = OrganigrammaManager.AmmInsNuovaUO(uo);
                        if (esito.Codice != 0)
                        {
                            simpleLog.Log("");
                            simpleLog.Log("ERROR (UO) - " + uo.Codice + " - " + esito.Descrizione);                            
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("INSERITA UO - Con codice " + uo.Codice);
                        }
                    }

                    //Modifica
                    if (dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("U"))
                    {
                        //Mi serve recuperare l'IDCorrGlob della UO
                        ArrayList listUo = OrganigrammaManager.AmmRicercaInOrg("U", uo.Codice, null, infoUtenteAmm.idAmministrazione, false, true);
                        if (listUo != null && listUo.Count == 1)
                        {
                            uo.IDCorrGlobale = ((OrgRisultatoRicerca)listUo[0]).IDCorrGlob;
                            EsitoOperazione esito = OrganigrammaManager.AmmModUO(uo, false);
                            if (esito.Codice != 0)
                            {
                                simpleLog.Log("");
                                simpleLog.Log("ERROR (UO) - " + uo.Codice + " - " + esito.Descrizione);
                            }
                            else
                            {
                                simpleLog.Log("");
                                simpleLog.Log("MODIFICATA UO - Con codice " + uo.Codice);
                            }
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("WARNING (UO) - UO con codice " + uo.Codice + " non modificata perchè non trovata");                            
                        }
                    }

                    //Cancellazione
                    if (dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("D"))
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING (UO) - Richiesta di cancellazione UO - Con codice " + uo.Codice + " non effettuata perchè non gestita");
                    }

                    //Sposamento
                    if (dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("M"))
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING (UO) - Richiesta di spostamento UO - Con codice " + uo.Codice + " non effettuata perchè non gestita");
                    }
                }
                else
                {
                    simpleLog.Log("");
                    simpleLog.Log("ERROR (UO) - Campi obbligatori UO non presenti");
                }
            }
        }

        private static void gestioneUtenti(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtenteAmm, DataSet dataSet, DocsPaDB.Utils.SimpleLog simpleLog)
        {
            foreach (DataRow dr in dataSet.Tables["UTENTI"].Rows)
            {
                //Controllo campi obbligatori
                if (!String.IsNullOrEmpty(dr["COD_AMMINISTRAZIONE"].ToString()) &&
                   !String.IsNullOrEmpty(dr["USER_ID"].ToString()) &&
                   !String.IsNullOrEmpty(dr["NOME"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COGNOME"].ToString()) &&
                   !String.IsNullOrEmpty(dr["MATRICOLA"].ToString()))
                {
                    //Recupero l'idAmministrazione e lo imposto all'infoUtenteAmministratore perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                    string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(dr["COD_AMMINISTRAZIONE"].ToString().ToUpper());
                    infoUtenteAmm.idAmministrazione = idAmm;

                    //Creo l' UTENTE
                    DocsPaVO.amministrazione.OrgUtente utente = new DocsPaVO.amministrazione.OrgUtente();
                    
                    //Valorizzo i campi
                    utente.UserId = dr["USER_ID"].ToString().ToUpper(); 
                    utente.CodiceRubrica = dr["USER_ID"].ToString().ToUpper();  
                    utente.Codice = utente.CodiceRubrica;
                    utente.IDAmministrazione = idAmm;
                    utente.Nome = dr["NOME"].ToString();
                    utente.Cognome = dr["COGNOME"].ToString();
                    utente.Dominio = dr["DOMINIO"].ToString();
                    utente.Password = dr["PASSWORD"].ToString();
                    utente.Sede = dr["SEDE"].ToString();
                    utente.Email = dr["EMAIL"].ToString();
                    utente.NotificaTrasm = dr["NOTIFICA"].ToString();
                    utente.Abilitato = dr["ABILITATO"].ToString();
                    utente.Amministratore = dr["AMMINISTRATORE"].ToString();
                    utente.Matricola = dr["MATRICOLA"].ToString();
                    
                    //Inserimento
                    if (dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("I"))
                    {
                        EsitoOperazione esito = OrganigrammaManager.AmmInsNuovoUtente(infoUtenteAmm, utente);
                        if (esito.Codice != 0)
                        {
                            simpleLog.Log("");
                            simpleLog.Log("ERROR (UTENTE) - " + utente.UserId + " - " + esito.Descrizione);
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("INSERITO UTENTE - Con userId " + utente.UserId);
                        }
                    }

                    //Modifica
                    if (dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("U"))
                    {
                        //Mi serve recuperare l'idPeople e l'idCorrGlobale dell'utente
                        Utente utenteRicercato = Utenti.UserManager.getUtente(utente.UserId, infoUtenteAmm.idAmministrazione);
                        if (utenteRicercato != null && !String.IsNullOrEmpty(utenteRicercato.idPeople))
                        {
                            utente.IDPeople = utenteRicercato.idPeople;
                            Corrispondente corrRicercato = Utenti.UserManager.getCorrispondenteByIdPeople(utente.IDPeople, DocsPaVO.addressbook.TipoUtente.INTERNO, infoUtenteAmm);
                            utente.IDCorrGlobale = corrRicercato.systemId;

                            utente.Password = string.Empty;
                            EsitoOperazione esito = OrganigrammaManager.AmmModUtente(infoUtenteAmm, utente);
                            if (esito.Codice != 0)
                            {
                                simpleLog.Log("");
                                simpleLog.Log("ERROR (UTENTE) - " + utente.UserId + " - " + esito.Descrizione);
                            }
                            else
                            {
                                simpleLog.Log("");
                                simpleLog.Log("MODIFICATO UTENTE - Con userId " + utente.UserId);
                            }
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("WARNING (UTENTE) - Utente con userId " + utente.UserId + " non modificato perchè non trovato");
                        }
                    }

                    //Cancellazione
                    if (dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("D"))
                    {
                        //Mi serve recuperare l'idPeople dell'utente
                        Utente utenteRicercato = Utenti.UserManager.getUtente(utente.UserId, infoUtenteAmm.idAmministrazione);
                        if (utenteRicercato != null && !String.IsNullOrEmpty(utenteRicercato.idPeople))
                        {
                            utente.IDPeople = utenteRicercato.idPeople;
                            EsitoOperazione esito = OrganigrammaManager.AmmEliminaUtente(infoUtenteAmm, utente);
                            if (esito.Codice != 0)
                            {
                                simpleLog.Log("");
                                simpleLog.Log("ERROR (UTENTE) - " + utente.UserId + " - " + esito.Descrizione);
                            }
                            else
                            {
                                simpleLog.Log("");
                                simpleLog.Log("ELIMINATO UTENTE - Con userId " + utente.UserId);
                            }
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("WARNING (UTENTE) - Utente con userId " + utente.UserId + " non eliminato perchè non trovato");
                        }
                    }

                    //Sposamento
                    //Questo tipo di operazione per quanto riguarda la lista degli utente non è gestita
                    //L'associazione utente ruoli viene definita nella lista dei ruoli
                }
                else
                {
                    simpleLog.Log("");
                    simpleLog.Log("ERROR (UTENTE) - Campi obbligatori UTENTI non presenti");
                }
            }
        }

        private static void gestioneRuoli(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtenteAmm, DataSet dataSet, DocsPaDB.Utils.SimpleLog simpleLog)
        {
            foreach (DataRow dr in dataSet.Tables["RUOLI"].Rows)
            {
                //Controllo campi obbligatori
                if (!String.IsNullOrEmpty(dr["COD_AMMINISTRAZIONE"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_UO_PADRE"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_TIPO_RUOLO"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_RUOLO"].ToString()) &&
                   !String.IsNullOrEmpty(dr["DESCRIZIONE"].ToString()))
                {
                    //Recupero l'idAmministrazione e lo imposto all'infoUtenteAmministratore perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                    string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(dr["COD_AMMINISTRAZIONE"].ToString().ToUpper());
                    infoUtenteAmm.idAmministrazione = idAmm;

                    //Creo il nuovo ruolo da inserire
                    OrgRuolo ruolo = new OrgRuolo();

                    //Valorizzo i campi
                    UnitaOrganizzativa uoPadre = (UnitaOrganizzativa)Utenti.UserManager.getCorrispondenteByCodRubrica(dr["COD_UO_PADRE"].ToString().ToUpper(), infoUtenteAmm);
                    if (uoPadre != null)
                        ruolo.IDUo = uoPadre.systemId;

                    ArrayList tipiRuolo = OrganigrammaManager.GetListTipiRuolo(idAmm);
                    foreach (OrgTipoRuolo tipoRuolo in tipiRuolo)
                    {
                        if (tipoRuolo.Codice.ToUpper() == dr["COD_TIPO_RUOLO"].ToString().ToUpper())
                            ruolo.IDTipoRuolo = tipoRuolo.IDTipoRuolo;
                    }

                    ruolo.Codice = dr["COD_RUOLO"].ToString().ToUpper();
                    ruolo.CodiceRubrica = dr["COD_RUOLO"].ToString().ToUpper();
                    ruolo.Descrizione = dr["DESCRIZIONE"].ToString().ToUpper();
                    ruolo.IDAmministrazione = idAmm;
                    ruolo.DiRiferimento = dr["DI_RIFERIMENTO"].ToString();
                    ruolo.Responsabile = dr["RESPONSABILE"].ToString();

                    //Inserimento
                    if (dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("I"))
                    {
                        EsitoOperazione esito = OrganigrammaManager.AmmInsNuovoRuolo(infoUtenteAmm, ruolo, false);
                        if (esito.Codice != 0)
                        {
                            simpleLog.Log("");
                            simpleLog.Log("ERROR (RUOLI) - " + ruolo.Codice + " - " + esito.Descrizione);
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("INSERITO RUOLO - Con codice " + ruolo.Codice);
                        }
                    }

                    //Modifica
                    if (dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("U"))
                    {
                        //Mi serve recuperare il ruolo da modificare
                        Ruolo ruoloRicercato = (Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(dr["COD_RUOLO"].ToString().ToUpper(), infoUtenteAmm);
                        ruolo.IDGruppo = ruoloRicercato.idGruppo;
                        //ruolo.IDAmministrazione = ruoloRicercato.idAmministrazione;
                        //ruolo.IDUo = ruoloRicercato.uo.systemId;
                        //ruolo.IDTipoRuolo = ruoloRicercato.tipoRuolo.systemId;
                        ruolo.IDCorrGlobale = ruoloRicercato.systemId;

                        EsitoOperazione esito = OrganigrammaManager.AmmModRuolo(infoUtenteAmm, ruolo);
                        if (esito.Codice != 0)
                        {
                            simpleLog.Log("");
                            simpleLog.Log("ERROR (RUOLI) - " + ruolo.Codice + " - " + esito.Descrizione);
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("MODIFICATO RUOLO - Con codice " + ruolo.Codice);
                        }
                    }

                    //Cancellazione
                    if (dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("D"))
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING (RUOLI) - Richiesta di cancellazione Ruolo - Con codice " + ruolo.Codice + " non effettuata perchè non gestita");
                    }

                    //Sposamento
                    if (dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("M"))
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING (RUOLI) - Richiesta di spostamento Ruolo - Con codice " + ruolo.Codice + " non effettuata perchè non gestita");
                    }
                }
                else
                {
                    simpleLog.Log("");
                    simpleLog.Log("ERROR (RUOLI) - Campi obbligatori RUOLI non presenti");
                }
            }
        }

        private static void gestioneRegistri(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtenteAmm, DataSet dataSet, DocsPaDB.Utils.SimpleLog simpleLog)
        {
            foreach (DataRow dr in dataSet.Tables["REGISTRI"].Rows)
            {
                //Controllo campi obbligatori
                if (!String.IsNullOrEmpty(dr["CODICE"].ToString()) &&
                   !String.IsNullOrEmpty(dr["DESCRIZIONE"].ToString()))
                {
                    //Recupero l'idAmministrazione e lo imposto all'infoUtenteAmministratore perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                    string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(dr["COD_AMMINISTRAZIONE"].ToString().ToUpper());
                    infoUtenteAmm.idAmministrazione = idAmm;

                    //Creo il REGISTRO
                    OrgRegistro registro = new OrgRegistro();

                    //Valorizzo i campi
                    registro.Codice = dr["CODICE"].ToString().ToUpper();
                    registro.Descrizione = dr["DESCRIZIONE"].ToString().ToUpper();
                    registro.IDAmministrazione = idAmm;
                    registro.CodiceAmministrazione = dr["COD_AMMINISTRAZIONE"].ToString().ToUpper();
                    //Di defualt considero l'elemento un REGISTRO e non un RF
                    registro.chaRF = "0";

                    //Inserimento
                    if(dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("I"))
                    {
                        DocsPaVO.Validations.ValidationResultInfo esito = Amministrazione.RegistroManager.InsertRegistro(registro);
                        if (!esito.Value)
                        {
                            if (esito.BrokenRules != null && esito.BrokenRules.Count > 0)
                            {
                                DocsPaVO.Validations.BrokenRule br = ((DocsPaVO.Validations.BrokenRule)esito.BrokenRules[0]);
                                simpleLog.Log("");
                                simpleLog.Log("ERROR (REGISTRO) - Registro con codice " + registro.Codice + " " + br.Description);
                            }
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("INSERITO REGISTRO - Con codice" + registro.Codice);                                                    
                        }
                    }

                    //Modifica
                    if (dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("U"))
                    {
                        //Mi serve recuperare l'IDRegistro
                        OrgRegistro[] registri = ((OrgRegistro[]) RegistroManager.GetRegistri(dr["COD_AMMINISTRAZIONE"].ToString().ToUpper(), registro.chaRF).ToArray(typeof(OrgRegistro)));
                        OrgRegistro registroRicercato = registri.Where(reg => reg.Codice == dr["CODICE"].ToString().ToUpper()).FirstOrDefault<OrgRegistro>();
                        if (registroRicercato != null)
                        {
                            registro.IDRegistro = registroRicercato.IDRegistro;
                            DocsPaVO.Validations.ValidationResultInfo esito = Amministrazione.RegistroManager.UpdateRegistro(registro);
                            if (!esito.Value)
                            {
                                if (esito.BrokenRules != null && esito.BrokenRules.Count > 0)
                                {
                                    DocsPaVO.Validations.BrokenRule br = ((DocsPaVO.Validations.BrokenRule)esito.BrokenRules[0]);
                                    simpleLog.Log("");
                                    simpleLog.Log("ERROR (REGISTRO) - Registro con codice " + registro.Codice + " " + br.Description);
                                }
                            }
                            else
                            {
                                simpleLog.Log("");
                                simpleLog.Log("MODIFICATO REGISTRO - Con codice " + registro.Codice);
                            }
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("WARNING (REGISTRO) - Registro con codice " + registro.Codice + " non modificato perchè non trovato");     
                        }
                    }

                    //Cancellazione
                    if (dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("D"))
                    {
                        //Mi serve recuperare l'IDRegistro
                        OrgRegistro[] registri = ((OrgRegistro[])RegistroManager.GetRegistri(dr["COD_AMMINISTRAZIONE"].ToString().ToUpper(), registro.chaRF).ToArray(typeof(OrgRegistro)));
                        OrgRegistro registroRicercato = registri.Where(reg => reg.Codice == dr["CODICE"].ToString().ToUpper()).FirstOrDefault<OrgRegistro>();
                        if (registroRicercato != null)
                        {
                            registro.IDRegistro = registroRicercato.IDRegistro;
                            DocsPaVO.Validations.ValidationResultInfo esito = Amministrazione.RegistroManager.DeleteRegistro(registro);
                            if (!esito.Value)
                            {
                                if (esito.BrokenRules != null && esito.BrokenRules.Count > 0)
                                {
                                    DocsPaVO.Validations.BrokenRule br = ((DocsPaVO.Validations.BrokenRule)esito.BrokenRules[0]);
                                    simpleLog.Log("");
                                    simpleLog.Log("ERROR (REGISTRO) - Registro con codice " + registro.Codice + " " + br.Description);
                                }
                            }
                            else
                            {
                                simpleLog.Log("");
                                simpleLog.Log("ELIMINATO REGISTRO - Con codice " + registro.Codice);
                            }
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("WARNING (REGISTRO) - Registro con codice " + registro.Codice + " non eliminato perchè non trovato");
                        }
                    }   
                 
                    //Sposamento
                    //Questo tipo di operazione per quanto riguarda la lista dei registri non è gestita            
                }
                else
                {
                    simpleLog.Log("");
                    simpleLog.Log("ERROR - Campi obbligatori REGISTRI non presenti");
                }
            }
        }

        private static void assRegistriRfRuolo(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtenteAmm, DataSet dataSet, DocsPaDB.Utils.SimpleLog simpleLog)
        {
            foreach (DataRow dr in dataSet.Tables["RUOLI"].Rows)
            {
                //Controllo campi obbligatori
                if (!String.IsNullOrEmpty(dr["COD_AMMINISTRAZIONE"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_UO_PADRE"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_TIPO_RUOLO"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_RUOLO"].ToString()) &&
                   !String.IsNullOrEmpty(dr["DESCRIZIONE"].ToString()))
                {
                    //Controllo che siano presenti dei registri da associare
                    if (!String.IsNullOrEmpty(dr["REGISTRI_RF"].ToString()))
                    {
                        //Recupero l'idAmministrazione e lo imposto all'infoUtenteAmministratore perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                        string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(dr["COD_AMMINISTRAZIONE"].ToString().ToUpper());
                        infoUtenteAmm.idAmministrazione = idAmm;

                        //Recupero l'idCorrGlobale della UO padre
                        UnitaOrganizzativa uoPadre = (UnitaOrganizzativa)Utenti.UserManager.getCorrispondenteByCodRubrica(dr["COD_UO_PADRE"].ToString().ToUpper(), infoUtenteAmm);
                        string idUo = string.Empty;
                        if (uoPadre != null)
                        {
                            idUo = uoPadre.systemId;
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("ERROR (ASSOCIAZIONE REGISTRI/RF RUOLO) - UO padre inesistente");
                        }

                        //Recupero l'idCorrGlobale del ruolo
                        Ruolo ruolo = (Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(dr["COD_RUOLO"].ToString().ToUpper(), infoUtenteAmm);
                        string idCorrGlobRuolo = string.Empty;
                        if (ruolo != null)
                        {
                            idCorrGlobRuolo = ruolo.systemId;
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("ERROR (ASSOCIAZIONE REGISTRI/RF RUOLO) - Ruolo con codice " + dr["COD_RUOLO"].ToString().ToUpper() + " inesistente");
                        }

                        //Recupero la lista registri dell'amministrazione
                        ArrayList registriAmm = Amministrazione.RegistroManager.GetRegistri(dr["COD_AMMINISTRAZIONE"].ToString().ToUpper(), "");
                        if (registriAmm == null)
                        {
                            simpleLog.Log("");
                            simpleLog.Log("ERROR (ASSOCIAZIONE REGISTRI/RF RUOLO) - Registri amministrazione inesistenti");
                        }


                        if(!String.IsNullOrEmpty(idUo) && !String.IsNullOrEmpty(idCorrGlobRuolo) && registriAmm != null)
                        {
                            //Recupero la lista dei codici registro da associare
                            string[] codiciReg = dr["REGISTRI_RF"].ToString().ToUpper().Split(';');

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

                            EsitoOperazione esito = OrganigrammaManager.AmmInsRegistri(listaRegistriDaAssociare, idUo, idCorrGlobRuolo);
                            if (esito.Codice != 0)
                            {
                                simpleLog.Log("");
                                simpleLog.Log("ERROR (ASSOCIAZIONE REGISTRI/RF RUOLO) - " + dr["COD_RUOLO"].ToString().ToUpper() + " - " + esito.Descrizione);
                            }
                            else
                            {
                                simpleLog.Log("");
                                simpleLog.Log("Registri associati al ruolo con codice - " + dr["COD_RUOLO"].ToString().ToUpper());                                
                            }
                        }
                    }
                    else
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING (ASSOCIAZIONE REGISTRI/RF RUOLO) - Per il ruolo con codice - " + dr["COD_RUOLO"].ToString().ToUpper() + " non ci sono registri/rf da associare");
                    }
                }
                else
                {
                    simpleLog.Log("");
                    simpleLog.Log("ERROR (ASSOCIAZIONE REGISTRI/RF RUOLO) - Campi obbligatori RUOLI non presenti");
                }
            }
        }

        private static void assFunzioniRuolo(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtenteAmm, DataSet dataSet, DocsPaDB.Utils.SimpleLog simpleLog)
        {
            foreach (DataRow dr in dataSet.Tables["RUOLI"].Rows)
            {
                //Controllo campi obbligatori
                if (!String.IsNullOrEmpty(dr["COD_AMMINISTRAZIONE"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_UO_PADRE"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_TIPO_RUOLO"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_RUOLO"].ToString()) &&
                   !String.IsNullOrEmpty(dr["DESCRIZIONE"].ToString()))
                {
                    //Controllo che siano presenti delle funzioni da associare
                    if (!String.IsNullOrEmpty(dr["FUNZIONI"].ToString()))
                    {
                        //Recupero l'idAmministrazione e lo imposto all'infoUtenteAmministratore perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                        string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(dr["COD_AMMINISTRAZIONE"].ToString().ToUpper());
                        infoUtenteAmm.idAmministrazione = idAmm;

                        //Recupero l'idCorrGlobale del ruolo
                        Ruolo ruolo = (Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(dr["COD_RUOLO"].ToString().ToUpper(), infoUtenteAmm);
                        string idCorrGlobRuolo = string.Empty;
                        if (ruolo != null)
                        {
                            idCorrGlobRuolo = ruolo.systemId;
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("ERROR (ASSOCIAZIONE FUNZIONI RUOLO) - Ruolo con codice " + dr["COD_RUOLO"].ToString().ToUpper() + " inesistente");
                        }
                        
                        //Recupero la lista delle funzioni dell'amministrazione
                        OrgTipoFunzione[] tipiFuzioneAmm = TipiFunzioneManager.GetTipiFunzione(true, idAmm);
                        if (tipiFuzioneAmm == null)
                        {
                            simpleLog.Log("");
                            simpleLog.Log("ERROR (ASSOCIAZIONE FUNZIONI RUOLO) - Tipi Funzione amministrazione inesistenti");
                        }

                        if (!String.IsNullOrEmpty(idCorrGlobRuolo) && tipiFuzioneAmm != null)
                        {
                            //Recupero la lista dei codici funzione da associare
                            string[] codiciFuzioni = dr["FUNZIONI"].ToString().Split(';');

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

                            EsitoOperazione esito = OrganigrammaManager.AmmInsTipoFunzioni(listaFunzioniDaAssociare);
                            if (esito.Codice != 0)
                            {
                                simpleLog.Log("");
                                simpleLog.Log("ERROR (ASSOCIAZIONE FUNZIONI RUOLO) - " + dr["COD_RUOLO"].ToString().ToUpper() + " - " + esito.Descrizione);
                            }
                            else
                            {
                                simpleLog.Log("");
                                simpleLog.Log("Funzioni associate al ruolo con codice - " + dr["COD_RUOLO"].ToString().ToUpper());
                            }
                        }
                    }
                    else
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING (ASSOCIAZIONE FUNZIONI RUOLO) - Per il ruolo con codice - " + dr["COD_RUOLO"].ToString().ToUpper() + " non ci sono funzioni da associare");
                    }
                }
                else
                {
                    simpleLog.Log("");
                    simpleLog.Log("ERROR - Campi obbligatori RUOLI non presenti");
                }
            }
        }

        private static void assUtentiRuolo(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtenteAmm, DataSet dataSet, DocsPaDB.Utils.SimpleLog simpleLog)
        {
            foreach (DataRow dr in dataSet.Tables["RUOLI"].Rows)
            {
                //Controllo campi obbligatori
                if (!String.IsNullOrEmpty(dr["COD_AMMINISTRAZIONE"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_UO_PADRE"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_TIPO_RUOLO"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_RUOLO"].ToString()) &&
                   !String.IsNullOrEmpty(dr["DESCRIZIONE"].ToString()))
                {
                    //Controllo che siano presenti degli utenti da associare
                    if (!String.IsNullOrEmpty(dr["UTENTI"].ToString()))
                    {
                        //Recupero l'idAmministrazione e lo imposto all'infoUtenteAmministratore perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                        string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(dr["COD_AMMINISTRAZIONE"].ToString().ToUpper());
                        infoUtenteAmm.idAmministrazione = idAmm;

                        //Recupero il ruolo a cui associare gli utenti
                        Ruolo ruolo = (Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(dr["COD_RUOLO"].ToString().ToUpper(), infoUtenteAmm);
                        string idGruppo = string.Empty;
                        if (ruolo != null)
                        {
                            idGruppo = ruolo.idGruppo;
                        }
                        else
                        {
                            simpleLog.Log("");
                            simpleLog.Log("ERROR (ASSOCIAZIONE UTENTI RUOLO) - Ruolo con codice " + dr["COD_RUOLO"].ToString().ToUpper() + " inesistente");
                        }

                        if (!String.IsNullOrEmpty(idGruppo))
                        {
                            //Elimino gli utenti dal ruolo
                            ArrayList utentiInRuolo = OrganigrammaManager.GetListUtentiRuolo(idGruppo);
                            if (utentiInRuolo != null)
                            {
                                foreach (OrgUtente ut in utentiInRuolo)
                                {
                                    EsitoOperazione esito = OrganigrammaManager.AmmEliminaUtenteInRuolo(infoUtenteAmm, ut.IDPeople, idGruppo);
                                    if (esito.Codice != 0)
                                    {
                                        simpleLog.Log("");
                                        simpleLog.Log("ERROR (ASSOCIAZIONE UTENTI RUOLO) - Utente con codice " + ut.UserId + " - " + esito.Descrizione);
                                    }
                                    else
                                    {
                                        simpleLog.Log("");
                                        simpleLog.Log("Utente con codice " + ut.UserId + " eliminato dal ruolo con codice - " + dr["COD_RUOLO"].ToString().ToUpper());
                                    }
                                }
                            }

                            //Recupero i codici degli utenti da inserire
                            string[] codiciUtente = dr["UTENTI"].ToString().Split(';');

                            //Associo l'utente al ruolo
                            for (int i = 0; i < codiciUtente.Length; i++)
                            {
                                //Recupero l'utente da associare
                                Utente utente = (Utente)Utenti.UserManager.getCorrispondenteByCodRubrica(codiciUtente[i].ToUpper(), infoUtenteAmm);
                                string idPeople = string.Empty;
                                if (utente != null)
                                    idPeople = utente.idPeople;

                                EsitoOperazione esito = OrganigrammaManager.AmmInsUtenteInRuolo(infoUtenteAmm, idPeople, idGruppo);
                                if (esito.Codice != 0)
                                {
                                    simpleLog.Log("");
                                    simpleLog.Log("ERROR (ASSOCIAZIONE UTENTI RUOLO) - Ruolo con codice " + dr["COD_RUOLO"].ToString().ToUpper() + " - " + esito.Descrizione);
                                }
                                else
                                {
                                    simpleLog.Log("");
                                    simpleLog.Log("Utente con codice " + codiciUtente[i].ToUpper() + " associato al ruolo con codice - " + dr["COD_RUOLO"].ToString().ToUpper());
                                }
                            }
                        }
                    }
                    else
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING (ASSOCIAZIONE UTENTI RUOLO) - Per il ruolo con codice - " + dr["COD_RUOLO"].ToString().ToUpper() + " non ci sono utenti da associare");
                    }                
                }
                else
                {
                    simpleLog.Log("");
                    simpleLog.Log("ERROR - Campi obbligatori RUOLI non presenti");
                }
            }
        }

        private static void assQualificheUtenti(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtenteAmm, DataSet dataSet, DocsPaDB.Utils.SimpleLog simpleLog)
        {
            foreach (DataRow dr in dataSet.Tables["QUALIFICHE"].Rows)
            {
                //Controllo campi obbligatori
                if (!String.IsNullOrEmpty(dr["COD_AMMINISTRAZIONE"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_UO"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_RUOLO"].ToString()) &&
                   !String.IsNullOrEmpty(dr["USER_ID"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COD_QUALIFICHE"].ToString()))
                {
                    //Controllo che siano presenti delle qualifiche da associare
                    if (!String.IsNullOrEmpty(dr["COD_QUALIFICHE"].ToString()))
                    {
                        //Recupero l'idAmministrazione e lo imposto all'infoUtenteAmministratore perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                        string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(dr["COD_AMMINISTRAZIONE"].ToString().ToUpper());
                        infoUtenteAmm.idAmministrazione = idAmm;

                        //Recupero la uo a cui associare le qualifiche
                        UnitaOrganizzativa uo = (UnitaOrganizzativa)Utenti.UserManager.getCorrispondenteByCodRubrica(dr["COD_UO"].ToString().ToUpper(), infoUtenteAmm);
                        if (uo == null)
                        {
                            simpleLog.Log("");
                            simpleLog.Log("WARNING (ASSOCIAZIONE QUALIFICHE UTENTI) - UO con codice - " + dr["COD_UO"].ToString().ToUpper() + " inesistente");
                        }

                        //Recupero il ruolo a cui associare le qualifiche
                        Ruolo ruolo = (Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(dr["COD_RUOLO"].ToString().ToUpper(), infoUtenteAmm);
                        if (ruolo == null)
                        {
                            simpleLog.Log("");
                            simpleLog.Log("WARNING (ASSOCIAZIONE QUALIFICHE UTENTI) - RUOLO con codice - " + dr["COD_RUOLO"].ToString().ToUpper() + " inesistente");
                        }

                        //Recupero l'utente a cui associare le qualifiche
                        Utente utente = (Utente)Utenti.UserManager.getCorrispondenteByCodRubrica(dr["USER_ID"].ToString().ToUpper(), infoUtenteAmm);
                        if (utente == null)
                        {
                            simpleLog.Log("");
                            simpleLog.Log("WARNING (ASSOCIAZIONE QUALIFICHE UTENTI) - UTENTE con codice - " + dr["USER_ID"].ToString().ToUpper() + " inesistente");
                        }

                        //Recupero la lista delle qualifiche per l'amministrazione di interesse
                        List<DocsPaVO.Qualifica.Qualifica> listQualifiche =  utenti.QualificheManager.GetQualifiche(Convert.ToInt32(idAmm));
                        if (listQualifiche == null)
                        {
                            simpleLog.Log("");
                            simpleLog.Log("WARNING (ASSOCIAZIONE QUALIFICHE UTENTI) - QUALIFICHE per l'amministrazione con codice - " + dr["COD_AMMINISTRAZIONE"].ToString().ToUpper() + " inesistenti");
                        }

                        //Recupero la lista delle qualifiche associate all'utente
                        List<DocsPaVO.Qualifica.PeopleGroupsQualifiche> listQualificheUtente = null;
                        if(utente != null && !String.IsNullOrEmpty(utente.idPeople))
                        {
                            //listQualificheUtente = utenti.QualificheManager.GetPeopleGroupsQualificheByIdPeople(utente.idPeople);
                            listQualificheUtente = utenti.QualificheManager.GetPeopleGroupsQualifiche(idAmm, uo.systemId, ruolo.idGruppo, utente.idPeople);
                        }

                        if (uo != null && ruolo != null && utente != null && !String.IsNullOrEmpty(uo.systemId) && !String.IsNullOrEmpty(ruolo.idGruppo) && !String.IsNullOrEmpty(utente.idPeople) && listQualifiche != null)
                        {
                            //Inserimento
                            if (dr["TIPO_OPERAZIONE"].ToString().ToUpper().Equals("I"))
                            {
                                ValidationResultInfo retValue = new ValidationResultInfo();
                                DocsPaDB.Query_DocsPAWS.Utenti dbUtenti = new DocsPaDB.Query_DocsPAWS.Utenti();

                                //Elimino le qualifiche associate all'utente
                                if(listQualificheUtente != null)
                                {
                                    foreach(DocsPaVO.Qualifica.PeopleGroupsQualifiche q in listQualificheUtente)
                                    {
                                        dbUtenti.DeletePeopleGroups(q.SYSTEM_ID.ToString());
                                    }
                                }

                                //Recupero i codici delle qualifiche da inserire
                                string[] codiciQualifiche = dr["COD_QUALIFICHE"].ToString().Split(';');

                                for (int i = 0; i < codiciQualifiche.Length; i++)
                                {
                                    DocsPaVO.Qualifica.Qualifica qualifica = listQualifiche.Where(q => q.CODICE == codiciQualifiche[i]).FirstOrDefault();

                                    if (qualifica != null)
                                    {
                                        //Creo e valorizzo l'oggetto qualifica da associare
                                        DocsPaVO.Qualifica.PeopleGroupsQualifiche pgq = new DocsPaVO.Qualifica.PeopleGroupsQualifiche();
                                        pgq.CODICE = qualifica.CODICE;
                                        pgq.DESCRIZIONE = qualifica.DESCRIZIONE;
                                        pgq.ID_AMM = qualifica.ID_AMM;
                                        pgq.ID_UO = Convert.ToInt32(uo.systemId);
                                        pgq.ID_GRUPPO = Convert.ToInt32(ruolo.idGruppo);
                                        pgq.ID_PEOPLE = Convert.ToInt32(utente.idPeople);                                    
                                        pgq.ID_QUALIFICA = qualifica.SYSTEM_ID;

                                        dbUtenti.InsertPeopleGroupsQual(pgq.ID_AMM.ToString(), pgq.ID_UO.ToString(), pgq.ID_GRUPPO.ToString(), pgq.ID_PEOPLE.ToString(), pgq.ID_QUALIFICA.ToString());
                                        simpleLog.Log("");
                                        simpleLog.Log("Qualifica con codice - " + codiciQualifiche[i] + " associata all'utente - " + dr["USER_ID"].ToString().ToUpper());
                                    }
                                    else
                                    {
                                        simpleLog.Log("");
                                        simpleLog.Log("WARNING (ASSOCIAZIONE QUALIFICHE UTENTI) - Qualifica con codice - " + codiciQualifiche[i] + " inesistente");
                                    }                                                
                                }
                            }

                            //Modifica
                            //Questo tipo di operazione per quanto riguarda la lista delle qualifiche non è gestita

                            //Cancellazione
                            //Questo tipo di operazione per quanto riguarda la lista delle qualifiche non è gestita
                        
                            //Sposamento
                            //Questo tipo di operazione per quanto riguarda la lista delle qualifiche non è gestita
                        }
                    }
                    else
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING (ASSOCIAZIONE QUALIFICHE UTENTI) - Per l'utente con user_id - " + dr["USER_ID"].ToString().ToUpper() + " non ci sono qualifiche da associare");
                    }
                }
                else
                {
                    simpleLog.Log("");
                    simpleLog.Log("ERROR - Campi obbligatori QUALIFICHE non presenti");
                }
            }
        }

        private static void assRuoloRiferimentoUtente(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtenteAmm, DataSet dataSet, DocsPaDB.Utils.SimpleLog simpleLog)
        {
            foreach (DataRow dr in dataSet.Tables["UTENTI"].Rows)
            {
                //Controllo campi obbligatori
                if (!String.IsNullOrEmpty(dr["COD_AMMINISTRAZIONE"].ToString()) &&
                   !String.IsNullOrEmpty(dr["USER_ID"].ToString()) &&
                   !String.IsNullOrEmpty(dr["NOME"].ToString()) &&
                   !String.IsNullOrEmpty(dr["COGNOME"].ToString()) &&
                   !String.IsNullOrEmpty(dr["MATRICOLA"].ToString()))
                {
                    //Controllo che sia presente un ruolo di riferimento da associare
                    if (!String.IsNullOrEmpty(dr["COD_RUOLO_RIFERIMENTO"].ToString()))
                    {
                        //Recupero l'idAmministrazione e lo imposto all'infoUtenteAmministratore perchè essendo un infoUtente che viene dall'amministrazione non ha questo parametro impostato
                        string idAmm = BusinessLogic.Amministrazione.OrganigrammaManager.GetIDAmm(dr["COD_AMMINISTRAZIONE"].ToString().ToUpper());
                        infoUtenteAmm.idAmministrazione = idAmm;

                        //Recupero il ruolo di riferimento per l'utente
                        Ruolo ruolo = (Ruolo)Utenti.UserManager.getCorrispondenteByCodRubrica(dr["COD_RUOLO_RIFERIMENTO"].ToString().ToUpper(), infoUtenteAmm);
                        if (ruolo == null)
                        {
                            simpleLog.Log("");
                            simpleLog.Log("WARNING (UTENTE) - RUOLO con codice - " + dr["COD_RUOLO_RIFERIMENTO"].ToString().ToUpper() + " inesistente");
                        }

                        //Recupero l'utente a cui associare il ruolo di riferimento
                        Utente utente = (Utente)Utenti.UserManager.getCorrispondenteByCodRubrica(dr["USER_ID"].ToString().ToUpper(), infoUtenteAmm);
                        if (utente == null)
                        {
                            simpleLog.Log("");
                            simpleLog.Log("WARNING (UTENTE) - UTENTE con codice - " + dr["USER_ID"].ToString().ToUpper() + " inesistente");
                        }

                        if (ruolo != null && utente != null && !String.IsNullOrEmpty(ruolo.idGruppo) && !String.IsNullOrEmpty(utente.idPeople))
                        {
                            EsitoOperazione esito = OrganigrammaManager.AmmImpostaRuoloPreferito(infoUtenteAmm, utente.idPeople, ruolo.idGruppo);
                            if (esito.Codice != 0)
                            {
                                simpleLog.Log("");
                                simpleLog.Log("ERROR (UTENTE) - Utente con codice " + utente.userId + " - " + esito.Descrizione);
                            }
                            else
                            {
                                simpleLog.Log("");
                                simpleLog.Log("Ruolo con codice " + ruolo.codiceCorrispondente + " associato come predefinito all'utente " + utente.userId);
                            }
                        }                        
                    }
                    else
                    {
                        simpleLog.Log("");
                        simpleLog.Log("WARNING (UTENTE) - Per l'utente con user_id - " + dr["USER_ID"].ToString().ToUpper() + " non c'è un ruolo di riferimento da associare");
                    }
                }
                else
                {
                    simpleLog.Log("");
                    simpleLog.Log("ERROR (UTENTE) - Campi obbligatori UTENTI non presenti");
                }
            }
        }
    }
}
