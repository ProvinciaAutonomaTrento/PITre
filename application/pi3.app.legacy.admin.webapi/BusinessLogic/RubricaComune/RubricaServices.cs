// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using BusinessLogic.Interoperabilita.Semplificata;
using DocsPaVO.amministrazione;
using DocsPaVO.rubrica;
using DocsPaVO.RubricaComune;
using DocsPaVO.Settings;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Office2010.Word;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.File.ReportGenerator;
using Pi3.Core.Services.File.Spreadsheet;
using Serilog;
using System.DirectoryServices.Protocols;


namespace BusinessLogic.RubricaComune
{
    public class RubricaServices
    {
        private static ILogger logger = Log.ForContext(typeof(RubricaServices));

        private const string SEARCH_ERROR = "Si è verificato un errore nella ricerca da rubrica comune";


        public static DocsPaVO.utente.Corrispondente UpdateCorrispondente(DocsPaVO.utente.InfoUtente infoUtente, string codiceRubrica, IBaseRubricaComuneService rubricaComuneService)
        {
#if true   // chiama web service rubrica comune
            BusinessLogic.RubricaComune.Corrispondente elemento = GetElementoInRubricaComune(infoUtente, codiceRubrica, rubricaComuneService);

            if (elemento != null)
                return UpdateCorrispondente(infoUtente, elemento, rubricaComuneService);
            else
#endif
                return null;

        }


        private static DocsPaVO.utente.Corrispondente UpdateCorrispondente(InfoUtente infoUtente, BusinessLogic.RubricaComune.Corrispondente elemento, IBaseRubricaComuneService rubricaComuneService)
        {
            // Reperimento dei dati del corrispondente della rubrica comune da docspa
            logger.Debug("UpdateCorrispondente -  verifico se l'elemento è present in rubrica comune");
            DocsPaVO.utente.Corrispondente corrispondente = GetCorrispondenteRubricaComuneInDocsPa(infoUtente, elemento.Codice, string.Empty);
            if (corrispondente != null)
                logger.Debug("UpdateCorrispondente -  il corrispondente trovato è null");
            else
                logger.Debug("UpdateCorrispondente -  il corrispondente trovato non è null");
            logger.Debug("UpdateCorrispondente -  eseguo updateCorrispondente");
            bool isCorrModified = false;
            DocsPaVO.utente.Corrispondente corr = UpdateCorrispondente(infoUtente, corrispondente, elemento, rubricaComuneService, out isCorrModified);
            // aggiungo in DPA_MAIL_CORR_ESTERNI le caselle di posta associate al corrispondente di rubrica comune
            if (corr != null && (!string.IsNullOrEmpty(corr.systemId)))
            {
                if (elemento.Emails.Count > 0)
                {
                    List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();
                    foreach (Email mail in elemento.Emails)
                    {
                        listCaselle.Add(new MailCorrispondente
                        {
                            Email = mail.Indirizzo,
                            Note = mail.Note,
                            Principale = mail.Preferita == true ? "1" : "0"
                        });
                    }
                    Utenti.addressBookManager.InsertMailCorrispondente(listCaselle, corr.systemId);
                }
            }
            return corr;
        }

        public static DocsPaVO.utente.Corrispondente UpdateCorrispondente(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Corrispondente corrispondente, IBaseRubricaComuneService rubricaComuneService)
        {


            logger.Debug("UpdateCorrispondente start");
            logger.Debug("UpdateCorrispondente - GetElementoInRubricaComune -start");
            // chiamata a web service rubrica comune
            BusinessLogic.RubricaComune.Corrispondente elemento = GetElementoInRubricaComune(infoUtente, corrispondente.codiceRubrica, rubricaComuneService);


            if (elemento != null)
            {
                logger.Debug("UpdateCorrispondente: trovato elemento");
                bool isCorrModified = false;
                DocsPaVO.utente.Corrispondente corr = UpdateCorrispondente(infoUtente, corrispondente, elemento, rubricaComuneService, out isCorrModified);

                //Emanuela 20-01-2014: aggiunto l'inserimento in DPA_MAIL_CORR_ESTERNI per corretto aggiornamento del corrispondente appartenente ad una lista
                if (corr != null && (!string.IsNullOrEmpty(corr.systemId)) && isCorrModified)
                {
                    if (elemento.Emails.Count > 0)
                    {
                        List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();
                        foreach (Email mail in elemento.Emails)
                        {
                            listCaselle.Add(new MailCorrispondente
                            {
                                Email = mail.Indirizzo,
                                Note = mail.Note,
                                Principale = mail.Preferita == true ? "1" : "0"
                            });
                        }
                        Utenti.addressBookManager.InsertMailCorrispondente(listCaselle, corr.systemId);
                    }
                }

                return corr;
            }
            else
            {
                logger.Debug("UpdateCorrispondente:  NON trovato elemento");
                return null;
            }

        }

        private static DocsPaVO.utente.Corrispondente UpdateCorrispondente(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Corrispondente corrispondente, BusinessLogic.RubricaComune.Corrispondente elemento, IBaseRubricaComuneService rubricaComuneService, out bool isCorrModified)
        {

            logger.Debug("UpdateCorrispondente start");
            // Se true, indica di creare un nuovo corrispondente
            bool requestNew = (corrispondente == null);
            bool idDirty = false;
            string codFisc = "";
            string pIva = "";
            bool variazioneEmail = false;

            string idOld = string.Empty;
            logger.Debug("requestNew " + requestNew.ToString());
            if (!requestNew)
            {


                //altrimenti idDirty##1 esce true
                if (elemento.Email == null)
                    elemento.Email = string.Empty;

                //altrimenti idDirty##1 esce true
                if (corrispondente.email == null)
                    corrispondente.email = string.Empty;
                try
                {
                    if (corrispondente.dettagli)
                    {
                        if (corrispondente.info == null)
                        {
                            corrispondente.info = GetDettagliCorrispondente(corrispondente.systemId);
                        }
                        codFisc = ((DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)(corrispondente.info.Tables[0].Rows[0])).codiceFiscale;
                        pIva = ((DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)(corrispondente.info.Tables[0].Rows[0])).partitaIva;

                    }
                }
                catch
                {

                }

                // Il corrispondente esiste già in docspa:
                // vengono effettuati dei controlli sui dati identificativi e su quelli aggiuntivi. 
                // Se sono cambiati rispetto alla versione docspa, quest'ultima viene storicizzata.


                #region controllo su cf e piva elemento 
                var CodiceFiscale = string.IsNullOrEmpty(elemento.CodiceFiscale) ? string.Empty : elemento.CodiceFiscale;
                var PIva = string.IsNullOrEmpty(elemento.PartitaIva) ? string.Empty : elemento.PartitaIva;
                #endregion

                idDirty = (corrispondente.descrizione != elemento.Denominazione ||
                            corrispondente.codiceAmm != elemento.Amministrazione ||
                            (!(string.IsNullOrEmpty(corrispondente.codiceAOO) && string.IsNullOrEmpty(elemento.AOO)) && corrispondente.codiceAOO != elemento.AOO) ||
                            corrispondente.email != elemento.Email ||
                            codFisc.ToUpper() != CodiceFiscale.ToUpper() ||
                            pIva.ToUpper() != PIva.ToUpper());

                logger.Debug("idDirty##1 = " + idDirty.ToString());

                if (!string.IsNullOrEmpty(corrispondente.descrizione))
                    logger.Debug("idDirty##1.corrispondente.descrizione = " + corrispondente.descrizione);
                if (!string.IsNullOrEmpty(elemento.Denominazione))
                    logger.Debug("idDirty##1. elemento.Descrizione = " + elemento.Denominazione);
                if (!string.IsNullOrEmpty(corrispondente.codiceAmm))
                    logger.Debug("idDirty##1.corrispondente.codiceAmm = " + corrispondente.codiceAmm);
                if (!string.IsNullOrEmpty(elemento.Amministrazione))
                    logger.Debug("idDirty##1. elemento.Amministrazione = " + elemento.Amministrazione);
                if (!string.IsNullOrEmpty(corrispondente.codiceAOO))
                    logger.Debug("idDirty##1.corrispondente.codiceAOO= " + corrispondente.codiceAOO);
                if (!string.IsNullOrEmpty(elemento.AOO))
                    logger.Debug("idDirty##1.elemento.AOO = " + elemento.AOO);
                if (!string.IsNullOrEmpty(corrispondente.email))
                    logger.Debug("idDirty##1.corrispondente.email= " + corrispondente.email);
                if (!string.IsNullOrEmpty(elemento.Email))
                    logger.Debug("idDirty##1.elemento.Email = " + elemento.Email);

                var elUrls = new List<string?>();
                if (elemento.UrlApiInteroperabilita != null)
                {
                    elUrls.Add(elemento.UrlApiInteroperabilita);
                }
                if (!idDirty && corrispondente.Url.Count > 0 && elUrls.Count > 0)
                    idDirty = corrispondente.Url[0].Url != elemento.UrlApiInteroperabilita;

                //if (!idDirty)
                //{

                //    // Verifica di cambiamenti avvenuti all'URL
                //    if (elemento.Urls == null) elemento.Urls = new RC.Proxy.Elementi.UrlInfo[0];
                //    idDirty = corrispondente.Url.Count != elemento.Urls.Length;
                //    if (!idDirty && corrispondente.Url.Count > 0 && elemento.Urls.Length > 0)
                //        idDirty = corrispondente.Url[0].Url != elemento.Urls[0].Url;
                //}



                logger.Debug("idDirty##2 = " + idDirty.ToString());
                // Verifica cambiamenti avvenuti sulle emails
                if (!idDirty)
                {

                    //old idDirty = elemento.Emails.Length != corrispondente.Emails.Count;
                    if (elemento.Emails != null && elemento.Emails.Count > 0) //verifica se da RC torna che il corr ha almeno una mail, altrimenti tutto il controllo sotto non ha senso
                    {
                        //if (corrispondente.Emails.Count > 1) //controlla se il corr ha almeno una mail.. non dovrebbe essere necessario perchè dopo l'intervento del 6/7/2012 Emails è non più vuoto.
                        if (elemento.Emails.Count > 1)
                            idDirty = elemento.Emails.Count != corrispondente.Emails.Count;
                        else
                            idDirty = elemento.Emails[0].Indirizzo.ToLower() != corrispondente.email.ToLower();

                        //// inserisco controllo se il numero delle email inserite in RC sia uguale a quelle riportate in rubrica locale
                        if (elemento.Emails.Count != corrispondente.Emails.Count)
                            variazioneEmail = true;
                    }

                    logger.Debug("idDirty##3 = " + idDirty.ToString());
                    if (!idDirty)
                    {
                        foreach (var mail in elemento.Emails)
                        {
                            idDirty &= !corrispondente.Emails.Contains(new MailCorrispondente() { Email = mail.Indirizzo });
                        }
                    }
                }

                logger.Debug("idDirty##4 = " + idDirty.ToString());

                // Se l'interoperabilità semplificata è disattivata ma il corrispondente salvato in Vt-Docs ha come
                // canale preferenziale quello ad essa relativo, bisogna fare in modo che il corrispondente venga
                // aggiornato. (Questo controllo viene fatto solo nel caso in cui sia valorizzato infoUtente, infatti
                // infoUtente non sarà valorizzato quando la chiamata arriva dal metodo per l'analisi del messaggio
                // di interoperabilità)
                if (infoUtente != null && ((corrispondente.canalePref != null && (corrispondente.canalePref.typeId == InteroperabilitaSemplificataManager.InteroperabilityCode ||
                    corrispondente.canalePref.tipoCanale == InteroperabilitaSemplificataManager.InteroperabilityCode))
                    && !InteroperabilitaSemplificataManager.IsEnabledSimplifiedInteroperability(infoUtente.idAmministrazione)) ||
                    (((corrispondente.canalePref != null && corrispondente.canalePref.typeId != InteroperabilitaSemplificataManager.InteroperabilityCode &
                    corrispondente.canalePref.tipoCanale != InteroperabilitaSemplificataManager.InteroperabilityCode)) &&
                    corrispondente.Url.Count > 0 && Uri.IsWellFormedUriString(corrispondente.Url[0].Url, UriKind.Absolute)))
                    idDirty = true;

                logger.Debug("idDirty##5 = " + idDirty.ToString());
                if (!idDirty)
                {
                    logger.Debug("get dettagli corr");
                    DocsPaVO.addressbook.DettagliCorrispondente oldDettagli = (DocsPaVO.addressbook.DettagliCorrispondente)corrispondente.info;

                    if (oldDettagli == null)
                    {
                        logger.Debug("get dettagli corr è null , avvio ricerca dettagli");
                        // Caricamento dei dettagli del corrispondente, qualora non siano stati reperiti
                        oldDettagli = GetDettagliCorrispondente(corrispondente.systemId);
                        logger.Debug("get dettagli corr è null , fine ricerca dettagli");
                    }

                    if (oldDettagli.Corrispondente.Rows.Count > 0)
                    {
                        logger.Debug("get dettagli corr è  pieno");
                        DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow oldRow = (DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)oldDettagli.Corrispondente.Rows[0];

                        idDirty = (oldRow.indirizzo != elemento.Indirizzo ||
                                        oldRow.citta != elemento.Citta ||
                                        oldRow.cap != elemento.CAP ||
                                        oldRow.provincia != elemento.Provincia ||
                                        oldRow.nazione != elemento.Nazione ||
                                        oldRow.telefono != elemento.Telefono ||
                                        oldRow.fax != elemento.Fax ||
                                        oldRow.codiceFiscale != elemento.CodiceFiscale ||
                                        oldRow.partitaIva != elemento.PartitaIva);
                        logger.Debug("idDirty##6 = " + idDirty.ToString());
                    }
                }
            }

            if (requestNew)
            {
                // Primo inserimento del corrispondente proveniente da rubrica comune in docspa
                DocsPaVO.utente.Corrispondente newCorr = GetNuovoCorrispondente(infoUtente, elemento);

                corrispondente = BusinessLogic.Utenti.addressBookManager.insertCorrispondente(newCorr, null);
                logger.Debug("insert corr ok");
                if (corrispondente != null && !string.IsNullOrEmpty(corrispondente.errore))
                {
                    // Errore in inserimento del corrispondente
                    if (!newCorr.inRubricaComune)
                        throw new ApplicationException(corrispondente.errore);
                    else
                    {
                        newCorr.errore = corrispondente.errore;
                        isCorrModified = true;
                        return newCorr;
                    }
                }
            }
            else if (idDirty)
            {
                // Modifica del corrispondente esistente
                corrispondente.descrizione = elemento.Denominazione;
                corrispondente.codiceAmm = elemento.Amministrazione;
                corrispondente.codiceAOO = elemento.AOO;
                corrispondente.email = elemento.Email;
                //corrispondente.Url = GetInternalUrlsCollection(elemento.Urls);

                corrispondente.Url = new List<DocsPaVO.utente.Corrispondente.UrlInfo>();

                corrispondente.Url.Add(new DocsPaVO.utente.Corrispondente.UrlInfo()
                {
                    Url = elemento.UrlApiInteroperabilita
                });

                // Aggiornamento del canale preferenziale,
                // nel caso sia presente la mail o meno
                corrispondente.canalePref = GetCanaleCorrispondente(corrispondente, infoUtente.idAmministrazione);

                DocsPaVO.utente.DatiModificaCorr datiModifica = GetDatiPerModifica(corrispondente.systemId, corrispondente.canalePref.systemId, elemento);

                using (DocsPaDB.Query_DocsPAWS.Utenti dbUtenti = new DocsPaDB.Query_DocsPAWS.Utenti())
                {
                    string newIdCorrGlobali;
                    string message;
                    if (dbUtenti.ModifyCorrispondenteEsterno(datiModifica, infoUtente, out newIdCorrGlobali, out message))
                    {
                        if (!string.IsNullOrEmpty(newIdCorrGlobali) && (!newIdCorrGlobali.Equals("0")))
                        {
                            // E' stato inserito un nuovo corrispondente
                            corrispondente.idOld = corrispondente.systemId;
                            corrispondente.systemId = newIdCorrGlobali;
                        }

                        // Verifica se è stato effettivamente inserito un nuovo corrispondente
                        DocsPaVO.addressbook.DettagliCorrispondente dettagli = new DocsPaVO.addressbook.DettagliCorrispondente();
                        dettagli.Corrispondente.AddCorrispondenteRow(elemento.Indirizzo, elemento.Citta, elemento.CAP, elemento.Provincia,
                                                                     elemento.Nazione, elemento.Telefono, string.Empty, elemento.Fax, elemento.CodiceFiscale, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, elemento.PartitaIva);
                        corrispondente.info = dettagli;
                    }
                    else
                        throw new ApplicationException("Errore nella modifica dei dati del corrispondente");

                    if (variazioneEmail)
                    {
                        List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();
                        foreach (Email mail in elemento.Emails)
                        {
                            listCaselle.Add(new MailCorrispondente
                            {
                                Email = mail.Indirizzo,
                                Note = mail.Note,
                                Principale = mail.Preferita == true ? "1" : "0"
                            });
                        }
                        using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                        {
                            if (dbUtenti.InsertMailCorr(listCaselle, corrispondente.systemId))
                            {
                                logger.Debug("Effettuata modifica delle mail del corrispondente");
                                transactionContext.Complete();
                            }
                            else
                                throw new ApplicationException("Errore nella modifica delle email del corrispondente");
                        }
                    }
                }
            }
            isCorrModified = idDirty;
            return corrispondente;
        }

        private static DocsPaVO.utente.Corrispondente GetCorrispondenteRubricaComuneInDocsPa(InfoUtente infoUtente, string codiceRubrica, string idAmministrazione)
        {
            DocsPaVO.utente.Corrispondente corr = null;
            //logger.Debug("GetCorrispondenteRubricaComuneInDocsPa -  start");
            // Verifica se il corrispondente esiste già in rubrica docspa e,
            // se si, effettua un controllo con i dati identificativi. 
            // Se sono cambiati rispetto alla versione docspa, il corrispondente locale
            // viene impostato come storicizzato
            using (DocsPaDB.Query_DocsPAWS.Utenti dbUtenti = new DocsPaDB.Query_DocsPAWS.Utenti())
            {
                string id;
                if (dbUtenti.ContainsCorrispondenteDaRubricaComune(codiceRubrica, idAmministrazione, out id))
                {
                    // Reperimento corrispondente esistente
                    //                    logger.Debug("GetCorrispondenteRubricaComuneInDocsPa -  Reperimento corrispondente esistente");
                    //corr = (DocsPaVO.utente.UnitaOrganizzativa)dbUtenti.GetCorrispondenteBySystemID(id);
                    corr = dbUtenti.GetCorrispondenteBySystemID(id);

                    if (corr != null)
                    {
                        //                        logger.Debug("GetCorrispondenteRubricaComuneInDocsPa -  il corrispondente esistente");
                        corr.inRubricaComune = true;

                        // Reperimento dei dettagli del corrispondente
                        //                        logger.Debug("GetCorrispondenteRubricaComuneInDocsPa -  Reperimento dei dettagli del corrispondente");
                        corr.info = GetDettagliCorrispondente(corr.systemId);
                        //                        logger.Debug("GetCorrispondenteRubricaComuneInDocsPa -  Reperimento dei dettagli del corrispondente ok");
                        corr.dettagli = true;
                    }
                }
            }
            //            logger.Debug("GetCorrispondenteRubricaComuneInDocsPa -  end;");
            return corr;
        }

        public static DocsPaVO.addressbook.DettagliCorrispondente GetDettagliCorrispondente(string idCorrispondente)
        {
            DocsPaVO.addressbook.QueryCorrispondente qc = new DocsPaVO.addressbook.QueryCorrispondente();
            qc.systemId = idCorrispondente;

            return BusinessLogic.Utenti.addressBookManager.dettagliCorrispondenteMethod(qc);
        }

#if false
        // chiama web service rubrica comune
        private static BusinessLogic.RubricaComune.Corrispondente GetElementoInRubricaComuneByEmail(DocsPaVO.utente.InfoUtente infoUtente, string email)
        {
            // Reperimento delle configurazioni per la gestione della rubrica comune da docspa
            ConfigurazioniRubricaComune config = Configurazioni.GetConfigurazioni(infoUtente);

            if (config.GestioneAbilitata)
            {
                RC.ElementiRubricaServices services = new global::RubricaComune.ElementiRubricaServices(config.ServiceRoot, config.SuperUserId, config.SuperUserPwd);

                try
                {
                    // Impostazione criterio di ricerca per email
                    RC.Proxy.Elementi.CriterioRicerca criterioRicerca = new global::RubricaComune.Proxy.Elementi.CriterioRicerca();
                    criterioRicerca.Nome = "email";
                    criterioRicerca.TipoRicerca = global::RubricaComune.Proxy.Elementi.TipiRicercaParolaEnum.ParolaIntera;
                    criterioRicerca.Valore = email;

                    RC.Proxy.Elementi.OpzioniRicerca opzioniRicerca = new global::RubricaComune.Proxy.Elementi.OpzioniRicerca();

                    opzioniRicerca.CriteriRicerca = new global::RubricaComune.Proxy.Elementi.CriteriRicerca();
                    opzioniRicerca.CriteriRicerca.Criteri = new global::RubricaComune.Proxy.Elementi.CriterioRicerca[1] { criterioRicerca };

                    RC.Proxy.Elementi.ElementoRubrica[] elementi = services.Search(ref opzioniRicerca);

                    if (elementi.Length > 0)
                        return elementi[0];
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    logger.Error(SEARCH_ERROR, ex);

                    return null;
                }
            }
            else
                return null;
        }

#endif

        /// <summary>
        /// Creazione di un elemento contenente gli attributi
        /// di un corrispondente docspa da inviare a rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idUO">
        /// Id dell'uo docspa da inviare a rubrica comune
        /// </param>
        /// <returns></returns>
        public static ElementoRubricaUO GetElementoRubricaUO(InfoUtente infoUtente, string idUO)
        {
            ElementoRubricaUO elemento = null;

            if (!string.IsNullOrEmpty(idUO))
            {
                // Reperimento dell'uo da inviare a rubrica comune
                OrgUO uo = GetUO(idUO);

                if (uo != null && !string.IsNullOrEmpty(uo.Codice))
                {
                    // Reperimento dati dell'amministrazione
                    InfoAmministrazione amministrazione = GetInfoAmministrazione(uo.IDAmministrazione);

                    // Creazione oggetto elemento rubrica da inviare
                    elemento = new ElementoRubricaUO();

                    // Impostazione dell'uo da inviare a rubrica comune
                    elemento.UO = uo;
                    // Reperimento codice uo per rubrica comune
                    elemento.CodiceRubrica = GetCodiceRubricaUO(amministrazione, uo.Codice);
                    //elemento.CodiceRubrica = GetCodiceRubrica(amministrazione, uo.Descrizione);

                    // Reperimento descrizione uo per rubrica comune
                    elemento.DescrizioneRubrica = GetDescrizioneRubrica(amministrazione, uo.Descrizione);

                    // Codice dell'amministrazione
                    elemento.Amministrazione = DocsPaDB.Utils.Personalization.getInstance(uo.IDAmministrazione).getCodiceAmministrazione();

                    // Codice AOO
                    elemento.AOO = uo.CodiceRegistroInterop;
                }
            }

            return elemento;
        }

        /// <summary>
        /// Reperimento dell'uo per rubrica comune
        /// </summary>
        /// <param name="idUO"></param>
        /// <returns></returns>
        private static OrgUO GetUO(string idUO)
        {
            // Reperimento dell'uo da inviare a rubrica comune
            OrgUO uo = Amministrazione.OrganigrammaManager.AmmGetDatiUOCorrente(idUO);

            if (uo != null)
                uo.DettagliUo = BusinessLogic.Amministrazione.OrganigrammaManager.AmmGetDatiStampaBuste(idUO);

            return uo;
        }

        /// <summary>
        /// Reperimento oggetto che rappresenta i dati dell'amministrazione
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        private static InfoAmministrazione GetInfoAmministrazione(string idAmm)
        {
            return Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(idAmm);
        }

        /// <summary>
        /// Reperimento del codice univoco dell'uo per rubrica comune,
        /// come composizione del codice dell'amministrazione e del codice uo
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="uo"></param>
        /// <returns></returns>
        private static string GetCodiceRubricaUO(InfoAmministrazione amministrazione, String codiceCorr)
        {
            return string.Format("{0}-{1}", amministrazione.Codice, codiceCorr);
        }

        /// <summary>
        /// Reperimento della descrizione dell'uo per rurbica comune,
        /// come composizione del codice dell'amministrazione e della descrizione dell'uo o dell'RF
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="uo"></param>
        /// <returns></returns>
        private static string GetDescrizioneRubrica(InfoAmministrazione amministrazione, String descrizione)
        {
            return string.Format("{0} - {1}", amministrazione.Codice, descrizione);
        }

        /// <summary>
        /// Creazione di un elemento contenente gli attributi
        /// di un RF da inviare a rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idRF">
        /// Id dell'RF da inviare a rubrica comune
        /// </param>
        /// <returns>Elemento inserito in Rubrica Comune</returns>
        public static ElementoRubricaRF GetElementoRubricaRF(InfoUtente infoUtente, string idRF)
        {
            ElementoRubricaRF elemento = null;

            if (!String.IsNullOrEmpty(idRF))
            {
                // Reperimento dell'RF da inviare a rubrica comune
                RaggruppamentoFunzionale rf = BusinessLogic.Utenti.RegistriManager.GetRaggruppamentoFunzionaleRC(idRF);

                if (rf != null)
                {
                    // Reperimento dati dell'amministrazione
                    InfoAmministrazione amministrazione = GetInfoAmministrazione(rf.idAmministrazione);

                    // Creazione oggetto elemento rubrica da inviare
                    elemento = new ElementoRubricaRF();

                    // Impostazione dell'RF da inviare a rubrica comune
                    elemento.RF = rf;

                    // Reperimento codice RF per rubrica comune
                    elemento.CodiceRubrica = GetCodiceRubricaRF(amministrazione, rf.Codice);

                    // Reperimento descrizione uo per rubrica comune
                    elemento.DescrizioneRubrica = GetDescrizioneRubrica(amministrazione, rf.descrizione);

                    // Dati per l'interoperabilità, se presente l'elemento rubrica è configurato per l'interoperabilità
                    //if (!string.IsNullOrEmpty(uo.CodiceRegistroInterop))
                    //    elemento.EMailRegistro = GetEMailRegistroInterop(uo.IDAmministrazione, uo.CodiceRegistroInterop);

                    // Codice dell'amministrazione
                    elemento.Amministrazione = amministrazione.Codice;

                    // Codice AOO
                    elemento.AOO = rf.codiceAOO;

                }
            }

            return elemento;
        }

        /// <summary>
        /// Reperimento del codice univoco dell'rf per rubrica comune,
        /// come composizione del codice dell'amministrazione e del codice uo
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="uo"></param>
        /// <returns></returns>
        private static string GetCodiceRubricaRF(InfoAmministrazione amministrazione, String codiceCorr)
        {
            return string.Format("{0}-{1}", amministrazione.Codice, codiceCorr);
        }

        /// <summary>
        /// Reperimento dei dati di un elemento da rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codiceRubrica"></param>
        /// <param name="filtraInterni">Flag utilizzato per indicare se il metodo deve filtrare i corrispondenti interni all'amministrazioune del chiamante</param>
        /// <returns></returns>
        public static DocsPaVO.rubrica.ElementoRubrica GetElementoRubricaComune(DocsPaVO.utente.InfoUtente infoUtente, string codiceRubrica, bool filtraInterni, IBaseRubricaComuneService rubricaComuneService)
        {
            // chiamata a servizio esterno

            BusinessLogic.RubricaComune.Corrispondente elemento = GetElementoInRubricaComune(infoUtente, codiceRubrica, rubricaComuneService);

            if (elemento != null && (!filtraInterni || !InternoInAmministrazione(elemento, infoUtente)))
                return MapElementoRubrica(elemento);
            else
                return null;

        }

        // chiamata a servizio esterno
        /// <summary>
        /// Reperimento di un elemento in rubrica comune e rubrica esterna
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codice"></param>
        /// <returns></returns>
        private static BusinessLogic.RubricaComune.Corrispondente GetElementoInRubricaComune(DocsPaVO.utente.InfoUtente infoUtente, string codice, IBaseRubricaComuneService rubricaComuneService)
        {
            BusinessLogic.RubricaComune.Corrispondente retValue = null;

            // Reperimento delle configurazioni per la gestione della rubrica comune da docspa
            ConfigurazioniRubricaComune config = Configurazioni.GetConfigurazioni(infoUtente);
            logger.Debug("GetElementoInRubricaComune START");
            if (config.GestioneAbilitata)
            {

                try
                {
                    logger.Debug("cerco in rC questo codice" + codice);

                    bool rubricheEsterneAttive = false;
                    if (DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_ENABLE_RUBRICHE_ESTERNE") != null &&
                        DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_ENABLE_RUBRICHE_ESTERNE").ToString().Equals("1"))
                        rubricheEsterneAttive = true;

                    //2020: filtro su ricerca rubriche esterne
                    string rubricheEsterne = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_RUBRICHE_ESTERNE");
                    if (rubricheEsterneAttive && !string.IsNullOrEmpty(rubricheEsterne) && rubricheEsterne != "0")
                    {
                        //List<RC.Proxy.Elementi.CriterioRicerca> criteriRicerca = new List<RC.Proxy.Elementi.CriterioRicerca>();
                        var criteriRicerca = new List<CriterioRicerca>
    {
        new CriterioRicerca { Campo = CampiRicercaEnum.Codice, Valore = string.IsNullOrEmpty(codice) ? codice : codice.TrimEnd(), TipoRicercaParola = BusinessLogic.RubricaComune.TipiRicercaParolaEnum.ParolaIntera },
        new CriterioRicerca { Campo = CampiRicercaEnum.RubricaEsterna, Valore = string.IsNullOrEmpty(rubricheEsterne) ? rubricheEsterne.ToUpper() : rubricheEsterne.TrimEnd(), TipoRicercaParola = BusinessLogic.RubricaComune.TipiRicercaParolaEnum.ParteDellaParola  }

    };
                        string authToken = HeaderValue.AddressBookAuthToken.Token.Value;
                        SearchResponse elementi = rubricaComuneService.Search(authToken, new BusinessLogic.RubricaComune.SearchRequest
                        {
                            CriteriRicerca = criteriRicerca,
                            ElementiPerPagina = 50,
                            Pagina = 0
                        }).Result;

                        if (elementi.Corrispondenti.Count > 0)
                            //                            retValue = elementi[0];
                            retValue = elementi.Corrispondenti[0];
                    }
                    #region commentato vecchio
                    //                    using (var scope = serviceProvider.CreateScope())
                    //                    {
                    //                        var configurationService = scope.ServiceProvider.GetRequiredService<IFileConverterFactory>();
                    //                        DocsPaVO.Report.PrintReportResponse response = null;


                    //                        var criteriRicerca = new List<CriterioRicerca>;



                    //                        criterioRicerca = new global::RubricaComune.Proxy.Elementi.CriterioRicerca();
                    //                        criterioRicerca.Nome = "rubrica_esterna";
                    //                        criterioRicerca.TipoRicerca = global::RubricaComune.Proxy.Elementi.TipiRicercaParolaEnum.ParteDellaParola;
                    //                        criterioRicerca.Valore = rubricheEsterne.ToUpper();
                    //                        criteriRicerca.Add(criterioRicerca);

                    //                        RC.Proxy.Elementi.OpzioniRicerca opzioniRicerca = new global::RubricaComune.Proxy.Elementi.OpzioniRicerca();
                    //                        opzioniRicerca.CriteriRicerca = new global::RubricaComune.Proxy.Elementi.CriteriRicerca();
                    //                        opzioniRicerca.CriteriRicerca.Criteri = criteriRicerca.ToArray();

                    //                        RC.Proxy.Elementi.ElementoRubrica[] elementi = services.Search(ref opzioniRicerca);

                    //                        if (elementi.Length > 0)
                    //                            retValue = elementi[0];
                    //                    } 
                    #endregion
                    else
                    {
                        //List<RC.Proxy.Elementi.CriterioRicerca> criteriRicerca = new List<RC.Proxy.Elementi.CriterioRicerca>();
                        var criteriRicerca = new List<CriterioRicerca>
    {
        new CriterioRicerca { Campo = CampiRicercaEnum.Codice, Valore = string.IsNullOrEmpty(codice) ? codice : codice.TrimEnd(), TipoRicercaParola = BusinessLogic.RubricaComune.TipiRicercaParolaEnum.ParolaIntera }

    };

                        string authToken = HeaderValue.AddressBookAuthToken.Token.Value;
                        SearchResponse elementi = rubricaComuneService.Search(authToken, new BusinessLogic.RubricaComune.SearchRequest
                        {
                            CriteriRicerca = criteriRicerca,
                            ElementiPerPagina = 1,
                            Pagina = 1
                        }).Result;



                        if (elementi.Corrispondenti.Count > 0)
                            //                            retValue = elementi[0];
                            retValue = elementi.Corrispondenti[0];
                    }

                    return retValue;
                }
                catch (Exception ex)
                {
                    logger.Error(SEARCH_ERROR, ex);

                    return null;
                }
            }
            else
                return retValue;
        }

        public static DocsPaVO.rubrica.ElementoRubrica[] GetElementiRubricaComune(InfoUtente infoUtente, DocsPaVO.RubricaComune.FiltriRubricaComune filtri, IBaseRubricaComuneService baseRubricaComuneService)
        {
            List<DocsPaVO.rubrica.ElementoRubrica> list = new List<DocsPaVO.rubrica.ElementoRubrica>();

            // Ricerca degli elementi in rubrica comune
            // chiamata a servizio esterno
            var utenti = RicercaInRubricaComune(infoUtente, filtri, baseRubricaComuneService).Corrispondenti;
            foreach (var item in utenti)
            {

                if (!InternoInAOO(item, infoUtente))
                {
                    // Per ciascun elemento in rubrica comune viene creato un corrispondente
                    // oggetto "ElementoRubrica" per l'utilizzo trasparente nella rubrica docspa

                    list.Add(MapElementoRubrica(item));
                }
            }

            return list.ToArray();
        }

        public static ElementoRubrica MapElementoRubrica(Corrispondente elemento)
        {

            DocsPaVO.rubrica.ElementoRubrica elementoRubrica = new DocsPaVO.rubrica.ElementoRubrica();

            elementoRubrica.codice = elemento.Codice;
            elementoRubrica.descrizione = elemento.Denominazione;
            elementoRubrica.interno = false;
            //elementoRubrica.tipo = "U";
            elementoRubrica.tipo = elemento.Tipo == BusinessLogic.RubricaComune.Tipi.RaggruppamentoFunzionale ? "F" : "U";
            elementoRubrica.has_children = false;
            elementoRubrica.canale = elemento.Canale;

            // Impostazione delle informazioni della rubrica comune per il corrispondente
            elementoRubrica.rubricaComune = new InfoElementoRubricaComune();
            elementoRubrica.rubricaComune.IdRubricaComune = (int)elemento.Id.AsLong();

            elementoRubrica.isRubricaComune = true;

            //2020: Aggiungo l'informazione della rubrica esterna da cui viene (es. IPA)
            //if (!string.IsNullOrEmpty(elemento.UtenteCreatore) && Enum.IsDefined(typeof(RubricaEsterna), elemento.UtenteCreatore))
            //elementoRubrica.rubricaEsterna = elemento.RubricaEsterna;

            return elementoRubrica;
        }

        private static bool InternoInAOO(Corrispondente elementoRubrica, InfoUtente infoUtente)
        {
            bool retValue = false;

            using (DocsPaDB.Query_DocsPAWS.Amministrazione amministrazioneDb = new DocsPaDB.Query_DocsPAWS.Amministrazione())
            {
                if (!string.IsNullOrEmpty(elementoRubrica.AOO))
                {
                    //Estraggo tutti i registri visibili all'utente e vado a verificare se il codice AOO di uno di questi registri
                    //coincide con il codice AOO dell'elementoRubrica.
                    DocsPaDB.Query_DocsPAWS.Utenti reg = new DocsPaDB.Query_DocsPAWS.Utenti();
                    List<Registro> registri = reg.GetListaRegistriRfRuolo(infoUtente.idCorrGlobali, "0", "").Cast<Registro>().ToList();
                    retValue = (from registro in registri where registro.codRegistro.Equals(elementoRubrica.AOO) select registro).FirstOrDefault() != null;
                }
            }

            return retValue;
        }

        private static SearchResponse RicercaInRubricaComune(InfoUtente infoUtente, FiltriRubricaComune filtri, IBaseRubricaComuneService baseRubricaComuneService)
        {
            var criteriRicerca = new List<CriterioRicerca>
    {
        new CriterioRicerca { Campo = CampiRicercaEnum.Codice, Valore = string.IsNullOrEmpty(filtri.Codice) ? filtri.Codice : filtri.Codice.TrimEnd() },
        new CriterioRicerca { Campo = CampiRicercaEnum.Denominazione, Valore = string.IsNullOrEmpty(filtri.Descrizione) ? filtri.Descrizione : filtri.Descrizione.TrimEnd()  },
        new CriterioRicerca { Campo = CampiRicercaEnum.Citta, Valore = string.IsNullOrEmpty(filtri.Citta) ? filtri.Citta : filtri.Citta.TrimEnd()  },
        new CriterioRicerca { Campo = CampiRicercaEnum.Email, Valore = string.IsNullOrEmpty(filtri.Mail) ? filtri.Mail : filtri.Mail.TrimEnd() },
        new CriterioRicerca { Campo = CampiRicercaEnum.CodiceFiscale, Valore = string.IsNullOrEmpty(filtri.CodiceFiscale) ? filtri.CodiceFiscale : filtri.CodiceFiscale.TrimEnd()  },
        new CriterioRicerca { Campo = CampiRicercaEnum.PartitaIva, Valore = string.IsNullOrEmpty(filtri.PartitaIva) ? filtri.PartitaIva : filtri.PartitaIva.TrimEnd()  }

    };

            string authToken = HeaderValue.AddressBookAuthToken.Token.Value;
            if (!string.IsNullOrEmpty(authToken))
                logger.Debug("Token Autenticazione per RUBRICA COMUNE presente");
            else
                logger.Debug("Token Autenticazione per RUBRICA COMUNE non presente");

            SearchResponse response = baseRubricaComuneService.Search(authToken, new BusinessLogic.RubricaComune.SearchRequest
            {
                CriteriRicerca = criteriRicerca,
                ElementiPerPagina = 50,
                Pagina = 0
            }).Result;

            return response;
        }

        private static bool InternoInAmministrazione(BusinessLogic.RubricaComune.Corrispondente elementoRubrica, InfoUtente infoUtente)
        {
            bool retValue = false;

            using (DocsPaDB.Query_DocsPAWS.Amministrazione amministrazioneDb = new DocsPaDB.Query_DocsPAWS.Amministrazione())
            {
                if (!string.IsNullOrEmpty(elementoRubrica.Amministrazione))
                {
                    retValue = (infoUtente.idAmministrazione == amministrazioneDb.GetIDAmm(elementoRubrica.Amministrazione));
                }
            }

            return retValue;
        }

        private static DocsPaVO.utente.Corrispondente GetNuovoCorrispondente(InfoUtente infoUtente, BusinessLogic.RubricaComune.Corrispondente elemento)
        {
            DocsPaVO.utente.Corrispondente corrispondente = InitializeSpecificAttributes(elemento.Tipo, elemento.Codice);


            // Indica che il corrispondente proviene da rubrica comune
            corrispondente.inRubricaComune = true;
            //corrispondente.codice = elemento.Codice;
            corrispondente.codiceRubrica = elemento.Codice;
            corrispondente.descrizione = elemento.Denominazione;
            corrispondente.email = elemento.Email;
            corrispondente.codiceAmm = elemento.Amministrazione;
            corrispondente.codiceAOO = elemento.AOO;
            corrispondente.tipoIE = "E";
            //corrispondente.tipoCorrispondente = "U";
            corrispondente.indirizzo = elemento.Indirizzo;
            //corrispondente.Url = GetInternalUrlsCollection(elemento.Urls);
            corrispondente.Url = new List<DocsPaVO.utente.Corrispondente.UrlInfo>();
            corrispondente.Url.Add(new DocsPaVO.utente.Corrispondente.UrlInfo()
            {
                Url = elemento.UrlApiInteroperabilita
            });

            // Reperimento del canale da associare al corrispondente
            corrispondente.canalePref = GetCanaleCorrispondente(corrispondente, infoUtente.idAmministrazione);

            // Impostazione dei dettagli del corrispondente
            DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();

            dettagliCorrispondente.Corrispondente.AddCorrispondenteRow
                (
                    elemento.Indirizzo, elemento.Citta, elemento.CAP, elemento.Provincia, elemento.Nazione,
                    elemento.Telefono, string.Empty, elemento.Fax, elemento.CodiceFiscale, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, elemento.PartitaIva
                );

            corrispondente.dettagli = true;
            corrispondente.info = dettagliCorrispondente;
            //NON ESISTE NEL NUOVO
            //if (!string.IsNullOrEmpty(elemento.UtenteCreatore) && Enum.IsDefined(typeof(RubricaEsterna), elemento.UtenteCreatore))
            //    corrispondente.rubricaEsterna = elemento.UtenteCreatore;

            return corrispondente;
        }

        private static DocsPaVO.utente.Corrispondente InitializeSpecificAttributes(BusinessLogic.RubricaComune.Tipi type, String codice)
        {
            DocsPaVO.utente.Corrispondente corrispondente;

            switch (type)
            {
                case BusinessLogic.RubricaComune.Tipi.UnitaOrganizzativa:
                    corrispondente = new UnitaOrganizzativa() { codice = codice, tipoCorrispondente = "U" };
                    break;
                case BusinessLogic.RubricaComune.Tipi.RaggruppamentoFunzionale:
                    corrispondente = new RaggruppamentoFunzionale() { Codice = codice, tipoCorrispondente = "F" };
                    break;
                default:
                    corrispondente = new DocsPaVO.utente.Corrispondente();
                    break;

            }

            return corrispondente;
        }

        private static DocsPaVO.utente.Canale GetCanaleCorrispondente(DocsPaVO.utente.Corrispondente corrispondente, String idAmministrazione)
        {
            // Reperimento del canale da associare al corrispondente
            bool canaleMail = false;
            bool canaleInterop = false;
            bool canaleInteropSemplificata = false;

            if (!String.IsNullOrEmpty(corrispondente.codiceAmm) &&
                !String.IsNullOrEmpty(corrispondente.codiceAOO))
            {
                // Se è valorizzato l'url, il canale preferenziale è interoperabilità semplificata
                // altrimenti se è valorizzata la mail è interiperabilità classica
                canaleInteropSemplificata = InteroperabilitaSemplificataManager.IsEnabledSimplifiedInteroperability(idAmministrazione) && corrispondente.Url != null && corrispondente.Url.Count > 0 && Uri.IsWellFormedUriString(corrispondente.Url[0].Url, UriKind.Absolute);
                canaleInterop = !String.IsNullOrEmpty(corrispondente.email) && !canaleInteropSemplificata;

            }
            else if (!string.IsNullOrEmpty(corrispondente.email))
            {
                // Se il corrispondente ha valorizzato solamente la mail e non 
                // i dati che lo definiscono come interoperante (amministrazione, aoo)
                // il canale preferenziale è MAIL
                canaleInterop = false;
                canaleInteropSemplificata = false;
                canaleMail = true;
            }

            // Possibili canali preferenziali
            const string INTEROPERABILITA = "INTEROPERABILITA"; // con mail, aoo e amministrazione
            const string MAIL = "MAIL"; // solo con la mail valorizzata per il corrispondente
            const string LETTERA = "LETTERA";
            const string INTEROP_SEMPLIFICATA = InteroperabilitaSemplificataManager.InteroperabilityCode;   // con Url, aoo e amministrazione valorizzati


            using (DocsPaDB.Query_DocsPAWS.Amministrazione dbAmm = new DocsPaDB.Query_DocsPAWS.Amministrazione())
            {
                foreach (DocsPaVO.utente.Canale canale in dbAmm.GetListCanali())
                {
                    if ((canale.typeId == INTEROPERABILITA && canaleInterop && !canaleInteropSemplificata) ||
                        (canale.typeId == MAIL && canaleMail) ||
                        (canale.typeId == LETTERA && !canaleMail && !canaleInterop && !canaleInteropSemplificata) ||
                        (canale.typeId == INTEROP_SEMPLIFICATA && canaleInteropSemplificata))
                    {
                        return canale;
                    }
                }
            }

            return null;
        }

        private static DocsPaVO.utente.DatiModificaCorr GetDatiPerModifica(string idOld, string idCanalePref, BusinessLogic.RubricaComune.Corrispondente elemento)
        {
            DocsPaVO.utente.DatiModificaCorr datiModifica = new DocsPaVO.utente.DatiModificaCorr();

            datiModifica.idCorrGlobali = idOld;
            datiModifica.idCanalePref = idCanalePref;
            datiModifica.codice = elemento.Codice;
            datiModifica.codRubrica = elemento.Codice;
            datiModifica.descCorr = elemento.Denominazione;
            datiModifica.codiceAmm = elemento.Amministrazione;
            datiModifica.codiceAoo = elemento.AOO;
            datiModifica.indirizzo = elemento.Indirizzo;
            datiModifica.citta = elemento.Citta;
            datiModifica.cap = elemento.CAP;
            datiModifica.provincia = elemento.Provincia;
            datiModifica.nazione = elemento.Nazione;
            datiModifica.telefono = elemento.Telefono;
            datiModifica.telefono2 = string.Empty;
            datiModifica.email = elemento.Email;
            datiModifica.fax = elemento.Fax;
            datiModifica.codFiscale = string.Empty;
            datiModifica.nome = string.Empty;
            datiModifica.inRubricaComune = true;
            //datiModifica.Urls = GetInternalUrlsCollection(elemento.Urls);
            datiModifica.Urls = new List<DocsPaVO.utente.Corrispondente.UrlInfo>();
            datiModifica.Urls.Add(new DocsPaVO.utente.Corrispondente.UrlInfo()
            {
                Url = elemento.UrlApiInteroperabilita
            });
            datiModifica.codFiscale = elemento.CodiceFiscale;
            datiModifica.partitaIva = elemento.PartitaIva;
            if (elemento.Tipo.Equals(BusinessLogic.RubricaComune.Tipi.RaggruppamentoFunzionale))
                datiModifica.tipoCorrispondente = "F";
            else
                if (elemento.Tipo.Equals(BusinessLogic.RubricaComune.Tipi.UnitaOrganizzativa))
                datiModifica.tipoCorrispondente = "U";

            return datiModifica;
        }

    }
}
