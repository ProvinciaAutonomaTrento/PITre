using System;
using System.Collections.Generic;
using System.Text;
using DocsPaVO.RubricaComune;
using DocsPaVO.utente;
using DocsPaVO.amministrazione;
using RC = RubricaComune;
using log4net;
using System.Linq;
using BusinessLogic.interoperabilita.Semplificata;

namespace BusinessLogic.RubricaComune
{
    /// <summary>
    /// Servizi per l'utilizzo della rubrica comune da DocsPa
    /// </summary>
    public sealed class RubricaServices
    {
        private static ILog logger = LogManager.GetLogger(typeof(RubricaServices));

        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public RubricaServices()
        { }

        /// <summary>
        /// Ricerca degli elementi in rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="filtri"></param>
        /// <remarks>
        /// </remarks>
        /// <returns>
        /// </returns>
        public static DocsPaVO.rubrica.ElementoRubrica[] GetElementiRubricaComune(InfoUtente infoUtente, DocsPaVO.RubricaComune.FiltriRubricaComune filtri)
        {
            List<DocsPaVO.rubrica.ElementoRubrica> list = new List<DocsPaVO.rubrica.ElementoRubrica>();

            // Ricerca degli elementi in rubrica comune
            foreach (RC.Proxy.Elementi.ElementoRubrica item in RicercaInRubricaComune(infoUtente, filtri))
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

        public static DocsPaVO.utente.DatiModificaCorr[] GetElementiRubricaComuneForExport(InfoUtente infoUtente, DocsPaVO.RubricaComune.FiltriRubricaComune filtri)
        {
            List<DocsPaVO.utente.DatiModificaCorr> list = new List<DocsPaVO.utente.DatiModificaCorr>();

            // Ricerca degli elementi in rubrica comune
            foreach (RC.Proxy.Elementi.ElementoRubrica item in RicercaInRubricaComune(infoUtente, filtri))
            {
                if (!InternoInAOO(item, infoUtente))
                {
                    DocsPaVO.utente.DatiModificaCorr corr = new DocsPaVO.utente.DatiModificaCorr();
                    corr.codice = string.Empty;
                    corr.codRubrica = item.Codice;
                    corr.codiceAmm = item.Amministrazione;
                    corr.codiceAoo = item.AOO;
                    corr.tipoCorrispondente = item.TipoCorrispondente.ToString();
                    corr.descCorr = item.Descrizione;
                    corr.cognome = string.Empty;
                    corr.nome = string.Empty;
                    corr.indirizzo = item.Indirizzo;
                    corr.cap = item.Cap;
                    corr.citta = item.Citta;
                    corr.provincia = item.Provincia;
                    corr.localita = string.Empty;
                    corr.nazione = item.Nazione;
                    corr.codFiscale = item.CodiceFiscale;
                    corr.telefono = item.Telefono;
                    corr.telefono2 = string.Empty;
                    corr.fax = item.Fax;
                    corr.email = item.Email;
                    corr.note = string.Empty;
                    corr.partitaIva = item.PartitaIva;
                    corr.descrizioneCanalePreferenziale = item.Canale;

                    list.Add(corr);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Reperimento dei dati di un elemento da rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codiceRubrica"></param>
        /// <param name="filtraInterni">Flag utilizzato per indicare se il metodo deve filtrare i corrispondenti interni all'amministrazioune del chiamante</param>
        /// <returns></returns>
        public static DocsPaVO.rubrica.ElementoRubrica GetElementoRubricaComune(DocsPaVO.utente.InfoUtente infoUtente, string codiceRubrica, bool filtraInterni)
        {
            RC.Proxy.Elementi.ElementoRubrica elemento = GetElementoInRubricaComune(infoUtente, codiceRubrica);

            if (elemento != null && (!filtraInterni || !InternoInAmministrazione(elemento, infoUtente)))
                return MapElementoRubrica(elemento);
            else
                return null;
        }

        /// <summary>
        /// Aggiornamento dei dati di un corrispondente docspa prelevato da rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="corrispondente"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Corrispondente UpdateCorrispondente(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Corrispondente corrispondente)
        {
            logger.Debug("UpdateCorrispondente start");
            logger.Debug("UpdateCorrispondente - GetElementoInRubricaComune -start");
            RC.Proxy.Elementi.ElementoRubrica elemento = GetElementoInRubricaComune(infoUtente, corrispondente.codiceRubrica);


            if (elemento != null)
            {
                logger.Debug("UpdateCorrispondente: trovato elemento");
                bool isCorrModified = false;
                Corrispondente corr = UpdateCorrispondente(infoUtente, corrispondente, elemento, out isCorrModified);

                //Emanuela 20-01-2014: aggiunto l'inserimento in DPA_MAIL_CORR_ESTERNI per corretto aggiornamento del corrispondente appartenente ad una lista
                if (corr != null && (!string.IsNullOrEmpty(corr.systemId)) && isCorrModified)
                {
                    if (elemento.Emails.Length > 0)
                    {
                        List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();
                        foreach (RC.Proxy.Elementi.EmailInfo mail in elemento.Emails)
                        {
                            listCaselle.Add(new MailCorrispondente
                            {
                                Email = mail.Email,
                                Note = mail.Note,
                                Principale = (mail.Preferita) ? "1" : "0"
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

        /// <summary>
        /// Reperimento di un corrispondente in docspa importato da rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codiceRubrica"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Corrispondente UpdateCorrispondente(DocsPaVO.utente.InfoUtente infoUtente, string codiceRubrica)
        {
            RC.Proxy.Elementi.ElementoRubrica elemento = GetElementoInRubricaComune(infoUtente, codiceRubrica);

            if (elemento != null)
                return UpdateCorrispondente(infoUtente, elemento);
            else
                return null;
        }

        /// <summary>
        /// Reperimento di un corrispondente in docspa importato da rubrica comune a partire dall'email
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.Corrispondente UpdateCorrispondenteByEmail(DocsPaVO.utente.InfoUtente infoUtente, string email, string codiceAmm = "", string codiceAoo = "")
        {
            logger.Debug("UpdateCorrispondenteByEmail -  verifico se l'elemento è present in rubrica comune");
            RC.Proxy.Elementi.ElementoRubrica elemento;
            if (!string.IsNullOrEmpty(codiceAmm) && !string.IsNullOrEmpty(codiceAoo))
            {
                elemento = GetElementoInRubricaComuneByEmailCodiceAmmCodiceAoo(infoUtente, email, codiceAmm, codiceAoo);
            }
            else
            {
                elemento = GetElementoInRubricaComuneByEmail(infoUtente, email);
            }

            if (elemento != null)
            {
                // Aggiornamento del corrispondente da rubrica comune in rubrica locale
                logger.Debug("UpdateCorrispondenteByEmail -  l'elemento è present in rubrica comune");
                logger.Debug("UpdateCorrispondenteByEmail -  sto per eseguire l'updateCorrispondente");
                return UpdateCorrispondente(infoUtente, elemento);
            }
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idUO"></param>
        /// <returns></returns>
        public static string GetCodiceRubricaUO(InfoUtente infoUtente, string idUO)
        {
            string codiceRubrica = string.Empty;

            if (!string.IsNullOrEmpty(idUO))
            {
                // Reperimento dell'uo da inviare a rubrica comune
                OrgUO uo = GetUO(idUO);

                if (uo != null && !string.IsNullOrEmpty(uo.Codice))
                {
                    // Reperimento dati dell'amministrazione
                    InfoAmministrazione amministrazione = GetInfoAmministrazione(uo.IDAmministrazione);

                    // Reperimento codice uo per rubrica comune
                    codiceRubrica = GetCodiceRubricaUO(amministrazione, uo.Codice);
                }
            }

            return codiceRubrica;
        }

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
        /// Reperimento di un corrispondente in docspa importato da rubrica comune a partire dall'email
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="email">email del corrispondente da trovare</param>
        /// <param name="descrizione">descrizione del corrisponedente da ricercare</param>
        /// <returns></returns>
        public static DocsPaVO.utente.Corrispondente UpdateCorrispondenteByEmailAndDescrizione(DocsPaVO.utente.InfoUtente infoUtente, string email, string descrizione)
        {
            RC.Proxy.Elementi.ElementoRubrica elemento = GetElementoInRubricaComuneByEmailAndDescrizione(infoUtente, email, descrizione);

            if (elemento != null)
                // Aggiornamento del corrispondente da rubrica comune in rubrica locale
                return UpdateCorrispondente(infoUtente, elemento);
            else
                return null;
        }

        /// <summary>
        /// Cerco di capire se il mio corrispondente è federato tramite il (metodo IAZZI)  
        /// Se non è presente in RC non lo è
        /// Se è presente ma Urls è nullo o vuoto non lo è 
        /// Se Urls è valorizzato allora probabilemente è un corrispondete federato
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool isCorrispondenteFederatoByEmail(DocsPaVO.utente.InfoUtente infoUtente, string email)
        {

            RC.Proxy.Elementi.ElementoRubrica elemento = GetElementoInRubricaComuneByEmail(infoUtente, email);
            if (elemento != null)
            {
                //Se l'elemento Urls è null non è federato
                if (elemento.Urls == null)
                    return false;
                //Se non sono presenti elementi in urls allora neanche è federato (lista vuota)
                if (elemento.Urls.Length == 0)
                    return false;

                //Se il primo elemeno nella lista è null o vuoto allora non è federato (non credo che il secondo elemento sia valorizzato)
                //in caso di valorizzazione allota torno true
                if (String.IsNullOrEmpty(elemento.Urls[0].Url))
                    return false;
                else
                    return true;
            }
            else
            {
                //Ha tornato null, quindi non trovato in RC (o il servizio RC è down o non configurato)
                return false;
            }

        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        private const string SEARCH_ERROR = "Si è verificato un errore nella ricerca da rubrica comune";

        /// <summary>
        /// Mapping oggetto ElementoRubrica della rubrica comune con l'oggetto ElementoRubrica della rubrica docspa
        /// </summary>
        /// <param name="elemento"></param>
        /// <returns></returns>
        private static DocsPaVO.rubrica.ElementoRubrica MapElementoRubrica(RC.Proxy.Elementi.ElementoRubrica elemento)
        {
            DocsPaVO.rubrica.ElementoRubrica elementoRubrica = new DocsPaVO.rubrica.ElementoRubrica();

            elementoRubrica.codice = elemento.Codice;
            elementoRubrica.descrizione = elemento.Descrizione;
            elementoRubrica.interno = false;
            //elementoRubrica.tipo = "U";
            elementoRubrica.tipo = elemento.TipoCorrispondente == RC.Proxy.Elementi.Tipo.RF ? "F" : "U";
            elementoRubrica.has_children = false;
            elementoRubrica.canale = elemento.Canale;

            // Impostazione delle informazioni della rubrica comune per il corrispondente
            elementoRubrica.rubricaComune = new InfoElementoRubricaComune();
            elementoRubrica.rubricaComune.IdRubricaComune = elemento.Id;

            elementoRubrica.isRubricaComune = true;

            return elementoRubrica;
        }

        /// <summary>
        /// Ricerca elementi in rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="filtri"></param>
        /// <returns></returns>
        private static RC.Proxy.Elementi.ElementoRubrica[] RicercaInRubricaComune(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.RubricaComune.FiltriRubricaComune filtri)
        {
            RC.Proxy.Elementi.ElementoRubrica[] list = new global::RubricaComune.Proxy.Elementi.ElementoRubrica[0];

            // Reperimento delle configurazioni per la gestione della rubrica comune da docspa
            ConfigurazioniRubricaComune config = Configurazioni.GetConfigurazioni(infoUtente);

            if (config.GestioneAbilitata)
            {
                RC.ElementiRubricaServices services = new global::RubricaComune.ElementiRubricaServices(config.ServiceRoot, config.SuperUserId, config.SuperUserPwd);

                // Reperimento delle opzioni di ricerca elementi in rubrica comune
                RC.Proxy.Elementi.OpzioniRicerca opzioni = GetOpzioniRicercaRubricaComune(filtri);

                try
                {
                    list = services.Search(ref opzioni);
                }
                catch (Exception ex)
                {
                    logger.Error(SEARCH_ERROR, ex);
                }
            }

            return list;
        }

        /// <summary>
        /// Reperimento di un elemento in rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static RC.Proxy.Elementi.ElementoRubrica GetElementoInRubricaComune(DocsPaVO.utente.InfoUtente infoUtente, int id)
        {
            // Reperimento delle configurazioni per la gestione della rubrica comune da docspa
            ConfigurazioniRubricaComune config = Configurazioni.GetConfigurazioni(infoUtente);

            if (config.GestioneAbilitata)
            {
                RC.ElementiRubricaServices services = new global::RubricaComune.ElementiRubricaServices(config.ServiceRoot, config.SuperUserId, config.SuperUserPwd);

                try
                {
                    return services.Get(id);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        private static RC.Proxy.Elementi.ElementoRubrica GetElementoInRubricaComuneByEmail(DocsPaVO.utente.InfoUtente infoUtente, string email)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        private static RC.Proxy.Elementi.ElementoRubrica GetElementoInRubricaComuneByEmailCodiceAmmCodiceAoo(DocsPaVO.utente.InfoUtente infoUtente, string email,string codiceAmm, string codiceAoo)
        {
            // Reperimento delle configurazioni per la gestione della rubrica comune da docspa
            ConfigurazioniRubricaComune config = Configurazioni.GetConfigurazioni(infoUtente);

            if (config.GestioneAbilitata)
            {
                RC.ElementiRubricaServices services = new global::RubricaComune.ElementiRubricaServices(config.ServiceRoot, config.SuperUserId, config.SuperUserPwd);
                global::RubricaComune.Proxy.Elementi.CriterioRicerca criteri = new RC.Proxy.Elementi.CriterioRicerca();
          
                try
                {
                    List<RC.Proxy.Elementi.CriterioRicerca> criteriRicerca = new List<RC.Proxy.Elementi.CriterioRicerca>();

                    // Impostazione criterio di ricerca per email
                    RC.Proxy.Elementi.CriterioRicerca criterioRicerca = new global::RubricaComune.Proxy.Elementi.CriterioRicerca();
                    criterioRicerca.Nome = "email";
                    criterioRicerca.TipoRicerca = global::RubricaComune.Proxy.Elementi.TipiRicercaParolaEnum.ParolaIntera;
                    criterioRicerca.Valore = email;
                    criteriRicerca.Add(criterioRicerca);

                    // Impostazione criterio di ricerca per codice aoo
                    criterioRicerca = new global::RubricaComune.Proxy.Elementi.CriterioRicerca();
                    criterioRicerca.Nome = "aoo";
                    criterioRicerca.TipoRicerca = global::RubricaComune.Proxy.Elementi.TipiRicercaParolaEnum.ParolaIntera;
                    criterioRicerca.Valore = codiceAoo;
                    criteriRicerca.Add(criterioRicerca);

                    // Impostazione criterio di ricerca per codice amministrazione
                    criterioRicerca = new global::RubricaComune.Proxy.Elementi.CriterioRicerca();
                    criterioRicerca.Nome = "amministrazione";
                    criterioRicerca.TipoRicerca = global::RubricaComune.Proxy.Elementi.TipiRicercaParolaEnum.ParolaIntera;
                    criterioRicerca.Valore = codiceAmm;
                    criteriRicerca.Add(criterioRicerca);

                    RC.Proxy.Elementi.OpzioniRicerca opzioniRicerca = new global::RubricaComune.Proxy.Elementi.OpzioniRicerca();
                    opzioniRicerca.CriteriRicerca = new global::RubricaComune.Proxy.Elementi.CriteriRicerca();
                    opzioniRicerca.CriteriRicerca.Criteri = criteriRicerca.ToArray();

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



        /// <summary>
        /// funzione che restituisce gli elementi in rubrica comune ricercati per mail e descrizione
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="email"></param>
        /// <param name="descrizione"></param>
        /// <returns></returns>
        private static RC.Proxy.Elementi.ElementoRubrica GetElementoInRubricaComuneByEmailAndDescrizione(DocsPaVO.utente.InfoUtente infoUtente, string email, string descrizione)
        {
            // Reperimento delle configurazioni per la gestione della rubrica comune da docspa
            ConfigurazioniRubricaComune config = Configurazioni.GetConfigurazioni(infoUtente);

            if (config.GestioneAbilitata)
            {
                RC.ElementiRubricaServices services = new global::RubricaComune.ElementiRubricaServices(config.ServiceRoot, config.SuperUserId, config.SuperUserPwd);

                try
                {
                    List<RC.Proxy.Elementi.CriterioRicerca> criteriRicerca = new List<RC.Proxy.Elementi.CriterioRicerca>();

                    // Impostazione criterio di ricerca per email
                    RC.Proxy.Elementi.CriterioRicerca criterioRicerca = new global::RubricaComune.Proxy.Elementi.CriterioRicerca();
                    criterioRicerca.Nome = "email";
                    criterioRicerca.TipoRicerca = global::RubricaComune.Proxy.Elementi.TipiRicercaParolaEnum.ParolaIntera;
                    criterioRicerca.Valore = email;
                    criteriRicerca.Add(criterioRicerca);

                    // Impostazione criterio di ricerca per descrizione
                    criterioRicerca = new global::RubricaComune.Proxy.Elementi.CriterioRicerca();
                    criterioRicerca.Nome = "descrizione";
                    criterioRicerca.TipoRicerca = global::RubricaComune.Proxy.Elementi.TipiRicercaParolaEnum.ParteDellaParola;
                    criterioRicerca.Valore = descrizione;
                    criteriRicerca.Add(criterioRicerca);

                    RC.Proxy.Elementi.OpzioniRicerca opzioniRicerca = new global::RubricaComune.Proxy.Elementi.OpzioniRicerca();
                    opzioniRicerca.CriteriRicerca = new global::RubricaComune.Proxy.Elementi.CriteriRicerca();
                    opzioniRicerca.CriteriRicerca.Criteri = criteriRicerca.ToArray();

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
        /// <summary>
        /// Reperimento di un elemento in rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codice"></param>
        /// <returns></returns>
        private static RC.Proxy.Elementi.ElementoRubrica GetElementoInRubricaComune(DocsPaVO.utente.InfoUtente infoUtente, string codice)
        {
            // Reperimento delle configurazioni per la gestione della rubrica comune da docspa
            ConfigurazioniRubricaComune config = Configurazioni.GetConfigurazioni(infoUtente);
            logger.Debug("GetElementoInRubricaComune START");
            if (config.GestioneAbilitata)
            {

                RC.ElementiRubricaServices services = new global::RubricaComune.ElementiRubricaServices(config.ServiceRoot, config.SuperUserId, config.SuperUserPwd);

                try
                {
                    logger.Debug("cerco in rC questo codice" + codice);
                    return services.SearchSingle(codice);
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

        /// <summary>
        /// Creazione dell'oggetto opzioni per la ricerca di elementi in rubrica comune
        /// </summary>
        /// <param name="filtri"></param>
        /// <returns></returns>
        private static RC.Proxy.Elementi.OpzioniRicerca GetOpzioniRicercaRubricaComune(DocsPaVO.RubricaComune.FiltriRubricaComune filtri)
        {
            RC.Proxy.Elementi.OpzioniRicerca opzioni = null;

            if (filtri != null)
            {
                opzioni = new global::RubricaComune.Proxy.Elementi.OpzioniRicerca();

                List<RC.Proxy.Elementi.CriterioRicerca> list = new List<global::RubricaComune.Proxy.Elementi.CriterioRicerca>();

                if (!string.IsNullOrEmpty(filtri.Codice))
                {
                    list.Add(GetCriterioRicerca("Codice", filtri.Codice, filtri.RicercaParolaIntera));
                }

                if (!string.IsNullOrEmpty(filtri.Descrizione))
                {
                    list.Add(GetCriterioRicerca("Descrizione", filtri.Descrizione, filtri.RicercaParolaIntera));
                }

                if (!string.IsNullOrEmpty(filtri.Citta))
                {
                    list.Add(GetCriterioRicerca("Citta", filtri.Citta, filtri.RicercaParolaIntera));
                }


                if (!string.IsNullOrEmpty(filtri.Mail))
                {
                    list.Add(GetCriterioRicerca("email", filtri.Mail, false));
                }

                if (!string.IsNullOrEmpty(filtri.CodiceFiscale))
                {
                    list.Add(GetCriterioRicerca("Var_cod_fisc", filtri.CodiceFiscale, false));
                }

                if (!string.IsNullOrEmpty(filtri.PartitaIva))
                {
                    list.Add(GetCriterioRicerca("Var_cod_pi", filtri.PartitaIva, false));
                }


                if (list.Count > 0)
                {
                    RC.Proxy.Elementi.CriteriRicerca criteriRicerca = new global::RubricaComune.Proxy.Elementi.CriteriRicerca();
                    criteriRicerca.Criteri = list.ToArray();
                    opzioni.CriteriRicerca = criteriRicerca;
                }
            }

            return opzioni;
        }

        /// <summary>
        /// Creazione di un singolo criterio di filtro
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="valore"></param>
        /// <param name="parolaIntera"></param>
        /// <returns></returns>
        private static RC.Proxy.Elementi.CriterioRicerca GetCriterioRicerca(string nome, object valore, bool parolaIntera)
        {
            RC.Proxy.Elementi.CriterioRicerca criterio = new global::RubricaComune.Proxy.Elementi.CriterioRicerca();
            criterio.Nome = nome;
            criterio.Valore = valore;
            if (parolaIntera)
                criterio.TipoRicerca = global::RubricaComune.Proxy.Elementi.TipiRicercaParolaEnum.ParolaIntera;
            else
                criterio.TipoRicerca = global::RubricaComune.Proxy.Elementi.TipiRicercaParolaEnum.ParteDellaParola;
            return criterio;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="corrispondente"></param>
        /// <param name="elemento"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.Corrispondente UpdateCorrispondente(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Corrispondente corrispondente, RC.Proxy.Elementi.ElementoRubrica elemento, out bool isCorrModified)
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
                idDirty = (corrispondente.descrizione != elemento.Descrizione ||
                            corrispondente.codiceAmm != elemento.Amministrazione ||
                            corrispondente.codiceAOO != elemento.AOO ||
                            corrispondente.email != elemento.Email ||
                            codFisc.ToUpper() != elemento.CodiceFiscale.ToUpper() ||
                            pIva.ToUpper() != elemento.PartitaIva.ToUpper());

                logger.Debug("idDirty##1 = " + idDirty.ToString());

                if (!string.IsNullOrEmpty(corrispondente.descrizione))
                    logger.Debug("idDirty##1.corrispondente.descrizione = " + corrispondente.descrizione);
                if (!string.IsNullOrEmpty(elemento.Descrizione))
                    logger.Debug("idDirty##1. elemento.Descrizione = " + elemento.Descrizione);
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

                if (!idDirty)
                {

                    // Verifica di cambiamenti avvenuti all'URL
                    if (elemento.Urls == null) elemento.Urls = new RC.Proxy.Elementi.UrlInfo[0];
                    idDirty = corrispondente.Url.Count != elemento.Urls.Length;
                    if (!idDirty && corrispondente.Url.Count > 0 && elemento.Urls.Length > 0)
                        idDirty = corrispondente.Url[0].Url != elemento.Urls[0].Url;
                }
                logger.Debug("idDirty##2 = " + idDirty.ToString());
                // Verifica cambiamenti avvenuti sulle emails
                if (!idDirty)
                {

                    //old idDirty = elemento.Emails.Length != corrispondente.Emails.Count;
                    if (elemento.Emails != null && elemento.Emails.Length > 0) //verifica se da RC torna che il corr ha almeno una mail, altrimenti tutto il controllo sotto non ha senso
                    {
                        //if (corrispondente.Emails.Count > 1) //controlla se il corr ha almeno una mail.. non dovrebbe essere necessario perchè dopo l'intervento del 6/7/2012 Emails è non più vuoto.
                        if (elemento.Emails.Length > 1)
                            idDirty = elemento.Emails.Length != corrispondente.Emails.Count;
                        else
                            idDirty = elemento.Emails[0].Email.ToLower() != corrispondente.email.ToLower();

                        //// inserisco controllo se il numero delle email inserite in RC sia uguale a quelle riportate in rubrica locale
                        if (elemento.Emails.Length != corrispondente.Emails.Count)
                            variazioneEmail = true;
                    }

                    logger.Debug("idDirty##3 = " + idDirty.ToString());
                    if (!idDirty)
                    {
                        foreach (var mail in elemento.Emails)
                        {
                            idDirty &= !corrispondente.Emails.Contains(new MailCorrispondente() { Email = mail.Email });
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
                                        oldRow.cap != elemento.Cap ||
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
                corrispondente.descrizione = elemento.Descrizione;
                corrispondente.codiceAmm = elemento.Amministrazione;
                corrispondente.codiceAOO = elemento.AOO;
                corrispondente.email = elemento.Email;
                corrispondente.Url = GetInternalUrlsCollection(elemento.Urls);

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
                        dettagli.Corrispondente.AddCorrispondenteRow(elemento.Indirizzo, elemento.Citta, elemento.Cap, elemento.Provincia,
                                                                     elemento.Nazione, elemento.Telefono, string.Empty, elemento.Fax, elemento.CodiceFiscale, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, elemento.PartitaIva);
                        corrispondente.info = dettagli;
                    }
                    else
                        throw new ApplicationException("Errore nella modifica dei dati del corrispondente");

                    if (variazioneEmail)
                    {
                        List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();
                        foreach (RC.Proxy.Elementi.EmailInfo mail in elemento.Emails)
                        {
                            listCaselle.Add(new MailCorrispondente
                            {
                                Email = mail.Email,
                                Note = mail.Note,
                                Principale = (mail.Preferita) ? "1" : "0"
                            });
                        }

                        if (dbUtenti.InsertMailCorr(listCaselle, corrispondente.systemId))
                            logger.Debug("Effettuata modifica delle mail del corrispondente");
                        else
                            throw new ApplicationException("Errore nella modifica delle email del corrispondente");
                    }
                }
            }
            isCorrModified = idDirty;
            return corrispondente;
        }

        /// <summary>
        /// Metodo per la conversione di elementi UrlInfo della rubrica comune ad elementi UrlInfo interni
        /// </summary>
        /// <param name="urlInfo">Array da convertire</param>
        /// <returns>Lista degli url interni</returns>
        private static List<Corrispondente.UrlInfo> GetInternalUrlsCollection(RC.Proxy.Elementi.UrlInfo[] urlInfo)
        {
            List<Corrispondente.UrlInfo> retCollection = new List<Corrispondente.UrlInfo>();
            retCollection.AddRange(from url in urlInfo
                                   select new Corrispondente.UrlInfo() { Url = url.Url });

            return retCollection;

        }

        /// <summary>
        /// Reperimento di un corrispondente docspa aggiornato con i dati del corrispondente in rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="elemento"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.Corrispondente UpdateCorrispondente(InfoUtente infoUtente, RC.Proxy.Elementi.ElementoRubrica elemento)
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
            Corrispondente corr = UpdateCorrispondente(infoUtente, corrispondente, elemento, out isCorrModified);
            // aggiungo in DPA_MAIL_CORR_ESTERNI le caselle di posta associate al corrispondente di rubrica comune
            if (corr != null && (!string.IsNullOrEmpty(corr.systemId)))
            {
                if (elemento.Emails.Length > 0)
                {
                    List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();
                    foreach (RC.Proxy.Elementi.EmailInfo mail in elemento.Emails)
                    {
                        listCaselle.Add(new MailCorrispondente
                        {
                            Email = mail.Email,
                            Note = mail.Note,
                            Principale = (mail.Preferita) ? "1" : "0"
                        });
                    }
                    Utenti.addressBookManager.InsertMailCorrispondente(listCaselle, corr.systemId);
                }
            }
            return corr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="corrispondente"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creazione di un nuovo corrispondente predisposto per l'inserimento in docspa
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="elemento"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.Corrispondente GetNuovoCorrispondente(InfoUtente infoUtente, RC.Proxy.Elementi.ElementoRubrica elemento)
        {
            Corrispondente corrispondente = InitializeSpecificAttributes(elemento.TipoCorrispondente, elemento.Codice);


            // Indica che il corrispondente proviene da rubrica comune
            corrispondente.inRubricaComune = true;
            //corrispondente.codice = elemento.Codice;
            corrispondente.codiceRubrica = elemento.Codice;
            corrispondente.descrizione = elemento.Descrizione;
            corrispondente.email = elemento.Email;
            corrispondente.codiceAmm = elemento.Amministrazione;
            corrispondente.codiceAOO = elemento.AOO;
            corrispondente.tipoIE = "E";
            //corrispondente.tipoCorrispondente = "U";
            corrispondente.indirizzo = elemento.Indirizzo;
            corrispondente.Url = GetInternalUrlsCollection(elemento.Urls);

            // Reperimento del canale da associare al corrispondente
            corrispondente.canalePref = GetCanaleCorrispondente(corrispondente, infoUtente.idAmministrazione);

            // Impostazione dei dettagli del corrispondente
            DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();

            dettagliCorrispondente.Corrispondente.AddCorrispondenteRow
                (
                    elemento.Indirizzo, elemento.Citta, elemento.Cap, elemento.Provincia, elemento.Nazione,
                    elemento.Telefono, string.Empty, elemento.Fax, elemento.CodiceFiscale, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, elemento.PartitaIva
                );

            corrispondente.dettagli = true;
            corrispondente.info = dettagliCorrispondente;

            return corrispondente;
        }

        /// <summary>
        /// Metodo per l'inizializzazione degli attributi specifici di UO o RF
        /// </summary>
        /// <param name="type"></param>
        /// <param name="codice"></param>
        /// <returns></returns>
        private static Corrispondente InitializeSpecificAttributes(RC.Proxy.Elementi.Tipo type, String codice)
        {
            Corrispondente corrispondente;

            switch (type)
            {
                case global::RubricaComune.Proxy.Elementi.Tipo.UO:
                    corrispondente = new UnitaOrganizzativa() { codice = codice, tipoCorrispondente = "U" };
                    break;
                case global::RubricaComune.Proxy.Elementi.Tipo.RF:
                    corrispondente = new RaggruppamentoFunzionale() { Codice = codice, tipoCorrispondente = "F" };
                    break;
                default:
                    corrispondente = new Corrispondente();
                    break;

            }

            return corrispondente;
        }

        /// <summary>
        /// Reperimento oggetto DatiModificaCorr necessario per la modifica del corrispondente
        /// </summary>
        /// <param name="idOld"></param>
        /// <param name="elemento"></param>
        /// <returns></returns>
        private static DocsPaVO.utente.DatiModificaCorr GetDatiPerModifica(string idOld, string idCanalePref, RC.Proxy.Elementi.ElementoRubrica elemento)
        {
            DocsPaVO.utente.DatiModificaCorr datiModifica = new DocsPaVO.utente.DatiModificaCorr();

            datiModifica.idCorrGlobali = idOld;
            datiModifica.idCanalePref = idCanalePref;
            datiModifica.codice = elemento.Codice;
            datiModifica.codRubrica = elemento.Codice;
            datiModifica.descCorr = elemento.Descrizione;
            datiModifica.codiceAmm = elemento.Amministrazione;
            datiModifica.codiceAoo = elemento.AOO;
            datiModifica.indirizzo = elemento.Indirizzo;
            datiModifica.citta = elemento.Citta;
            datiModifica.cap = elemento.Cap;
            datiModifica.provincia = elemento.Provincia;
            datiModifica.nazione = elemento.Nazione;
            datiModifica.telefono = elemento.Telefono;
            datiModifica.telefono2 = string.Empty;
            datiModifica.email = elemento.Email;
            datiModifica.fax = elemento.Fax;
            datiModifica.codFiscale = string.Empty;
            datiModifica.nome = string.Empty;
            datiModifica.inRubricaComune = true;
            datiModifica.Urls = GetInternalUrlsCollection(elemento.Urls);
            datiModifica.codFiscale = elemento.CodiceFiscale;
            datiModifica.partitaIva = elemento.PartitaIva;
            if (elemento.TipoCorrispondente.ToString().Equals("RF"))
                datiModifica.tipoCorrispondente = "F";
            else
                if (elemento.TipoCorrispondente.ToString().Equals("UO"))
                    datiModifica.tipoCorrispondente = "U";

            return datiModifica;
        }

        /// <summary>
        /// Reperimento della copia dei dati del corrispondente proveniente da rubrica comune in docspa
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codiceRubrica"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Reperimento dei dettagli del corrispondente
        /// </summary>
        /// <param name="idCorrispondente"></param>
        /// <returns></returns>
        public static DocsPaVO.addressbook.DettagliCorrispondente GetDettagliCorrispondente(string idCorrispondente)
        {
            DocsPaVO.addressbook.QueryCorrispondente qc = new DocsPaVO.addressbook.QueryCorrispondente();
            qc.systemId = idCorrispondente;

            return BusinessLogic.Utenti.addressBookManager.dettagliCorrispondenteMethod(qc);
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
        /// Reperimento oggetto che rappresenta i dati dell'amministrazione
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        private static InfoAmministrazione GetInfoAmministrazione(string idAmm)
        {
            return Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(idAmm);
        }


        /// <summary>
        /// Indica se includere o meno l'elemento rubrica come risultato di una ricerca.
        /// L'elemento viene escluso se fa parte dell'amministrazione corrente (che vedrà già l'elemento come UO interna).
        /// </summary>
        /// <param name="elementoRubrica"></param>
        /// <param name="infoUtente"></param>
        /// <returns></returns>
        private static bool InternoInAmministrazione(RC.Proxy.Elementi.ElementoRubrica elementoRubrica, InfoUtente infoUtente)
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


        private static bool InternoInAOO(RC.Proxy.Elementi.ElementoRubrica elementoRubrica, InfoUtente infoUtente)
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

        #endregion
    }
}
