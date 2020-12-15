using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using P3SBCLib;
using P3SBCLib.Contracts;
using log4net;

namespace P3SBCLib
{
    /// <summary>
    /// Implementazione dei servizi per l'interazione del sistema SBC con il sistema PITRE
    /// </summary>
    public class P3SBCServices : IP3SBCServices
    {
        private ILog logger = LogManager.GetLogger(typeof(P3SBCServices));
        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        public P3SBCServices()
        { }

        /// <summary>
        /// Servizio per il download del file associato ad un documento in PITRE
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="numeroDocumento">Numero del documento
        /// <remarks>
        /// In PITRE, corrisponde all'identificativo univoco del documento
        /// </remarks>
        /// </param>
        /// <returns>Oggetto contenente i metadati e il file associato al documento</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        public Documento DownloadDocumento(string userIdRichiedente, string numeroDocumento)
        {
            try
            {
                this.CheckStringParameter("userIdRichiedente", userIdRichiedente);
                this.CheckStringParameter("numeroDocumento", numeroDocumento);

                // Reperimento oggetto InfoUtente
                DocsPaVO.utente.InfoUtente infoUtente = DocsPaServices.DocsPaServiceHelper.Impersonate(userIdRichiedente);

                return this.GetFile(infoUtente, numeroDocumento);
            }
            catch (Exception ex)
            {
                logger.Error(ex);

                throw new FaultException<P3OperationFault>(new P3OperationFault { Message = ex.Message }, new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Servizio per l'upload del file associato ad un documento in PITRE 
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="numeroDocumento">Numero del documento
        /// <remarks>
        /// In PITRE, corrisponde all'identificativo univoco del documento
        /// </remarks>
        /// </param>
        /// <param name="documento">Oggetto contenente il file da associare al documento</param>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        public void UploadDocumento(string userIdRichiedente, string numeroDocumento, Documento documento)
        {
            try
            {
                this.CheckStringParameter("userIdRichiedente", userIdRichiedente);
                this.CheckStringParameter("numeroDocumento", numeroDocumento);
                this.CheckObjectParameter("documento", documento);

                // Reperimento oggetto InfoUtente
                DocsPaVO.utente.InfoUtente infoUtente = DocsPaServices.DocsPaServiceHelper.Impersonate(userIdRichiedente);

                this.SetFile(infoUtente, numeroDocumento, documento);
            }
            catch (Exception ex)
            {
                logger.Debug(ex);

                throw new FaultException<P3OperationFault>(new P3OperationFault { Message = ex.Message }, new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Servizio per la ricerca dei documenti in PITRE
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="filtriRicerca">Criteri per filtrare i risultati della ricerca</param>
        /// <param name="criteriPaginazione">Opzionale. Consente di specificare i criteri per paginare i risultati della ricerca. Se non definito, non sarà applicata alcuna paginazione ai dati</param>
        /// <returns>Esito della ricerca documenti effettuata</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        public ElencoInfoDocumenti GetDocumenti(string userIdRichiedente, FiltroRicercaDocumenti filtriRicerca, CriteriPaginazione criteriPaginazione)
        {
            try
            {
                this.CheckStringParameter("userIdRichiedente", userIdRichiedente);

                // Reperimento oggetto InfoUtente
                DocsPaVO.utente.InfoUtente infoUtente = DocsPaServices.DocsPaServiceHelper.Impersonate(userIdRichiedente);

                if (criteriPaginazione == null)
                {
                    // Se la paginazione non è impostata
                    criteriPaginazione = new CriteriPaginazione
                    {
                        Pagina = 1,
                        OggettiPerPagina = Int32.MaxValue
                    };
                }

                DocsPaVO.ricerche.SearchPagingContext pagingContext = new DocsPaVO.ricerche.SearchPagingContext
                {
                    Page = criteriPaginazione.Pagina,
                    PageSize = criteriPaginazione.OggettiPerPagina,
                    GetIdProfilesList = false
                };

                DocsPaVO.filtri.FiltroRicerca[] filtri = this.GetFiltriRicercaDocumenti(infoUtente, filtriRicerca, false);

                var doc = from c in DocsPaServices.DocsPaServiceHelper.GetDocumenti(infoUtente, filtri, pagingContext)
                          select this.CreateDocumento(infoUtente, c);

                ElencoInfoDocumenti retValue = new ElencoInfoDocumenti();
                criteriPaginazione.TotaleOggetti = pagingContext.RecordCount;
                criteriPaginazione.TotalePagine = pagingContext.PageCount;

                retValue.Documenti = doc.ToArray<InfoDocumento>();
                retValue.Paginazione = criteriPaginazione;
                return retValue;
            }
            catch (Exception ex)
            {
                logger.Debug(ex);

                throw new FaultException<P3OperationFault>(new P3OperationFault { Message = ex.Message }, new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Servizio per il reperimento dei dati di un documento in SBC a partire dal numero
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta </param>
        /// <param name="numeroDocumento">Numero del documento
        /// <remarks>
        /// In PITRE, corrisponde all'identificativo univoco del documento
        /// </remarks>
        /// </param>
        /// <param name="downloadContent">
        /// Indica se caricare contestualmente al servizio il file associato al documento
        /// </param>
        /// <returns>Metadati del documento</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        public InfoDocumento GetDocumento(string userIdRichiedente, string numeroDocumento, bool downloadContent)
        {
            try
            {
                this.CheckStringParameter("userIdRichiedente", userIdRichiedente);
                this.CheckStringParameter("numeroDocumento", numeroDocumento);

                // Reperimento oggetto InfoUtente
                DocsPaVO.utente.InfoUtente infoUtente = DocsPaServices.DocsPaServiceHelper.Impersonate(userIdRichiedente);

                DocsPaVO.documento.InfoDocumento infoDocumento = DocsPaServices.DocsPaServiceHelper.GetDocumento(infoUtente, numeroDocumento);

                if (infoDocumento == null)
                    throw new ApplicationException(string.Format("Documento con numero {0} non trovato", numeroDocumento));

                InfoDocumento documento = CreateDocumento(infoUtente, infoDocumento);

                if (downloadContent)
                {
                    documento.Documento = this.GetFile(infoUtente, numeroDocumento);
                    documento.DocumentoSpecified = (documento.Documento != null);
                }

                return documento;
            }
            catch (Exception ex)
            {
                logger.Debug(ex);

                throw new FaultException<P3OperationFault>(new P3OperationFault { Message = ex.Message }, new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Servizio per l'associazione di un documento in una pratica
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="numeroPratica">Numero della pratica</param>
        /// <param name="numeroDocumento">Numero del documento</param>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        public void AddDocumentoPratica(string userIdRichiedente, string numeroPratica, string numeroDocumento)
        {
            try
            {
                this.CheckStringParameter("userIdRichiedente", userIdRichiedente);
                this.CheckStringParameter("numeroPratica", numeroPratica);
                this.CheckStringParameter("numeroDocumento", numeroDocumento);

                // Reperimento oggetto InfoUtente
                DocsPaVO.utente.InfoUtente infoUtente = DocsPaServices.DocsPaServiceHelper.Impersonate(userIdRichiedente);

                this.AddDocumentoPratica(infoUtente, numeroPratica, numeroDocumento);
            }
            catch (Exception ex)
            {
                logger.Debug(ex);

                throw new FaultException<P3OperationFault>(new P3OperationFault { Message = ex.Message }, new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Rimozione di un documento da una pratica
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="numeroPratica">Numero della pratica</param>
        /// <param name="numeroDocumento">Numero del documento</param>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        public void RemoveDocumentoPratica(string userIdRichiedente, string numeroPratica, string numeroDocumento)
        {
            try
            {
                this.CheckStringParameter("userIdRichiedente", userIdRichiedente);
                this.CheckStringParameter("numeroPratica", numeroPratica);
                this.CheckStringParameter("numeroDocumento", numeroDocumento);

                // Reperimento oggetto InfoUtente
                DocsPaVO.utente.InfoUtente infoUtente = DocsPaServices.DocsPaServiceHelper.Impersonate(userIdRichiedente);

                this.RemoveDocumentoPratica(infoUtente, numeroPratica, numeroDocumento);
            }
            catch (Exception ex)
            {
                logger.Debug(ex);

                throw new FaultException<P3OperationFault>(new P3OperationFault { Message = ex.Message }, new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Servizio per la ricerca delle pratiche in SBC (fascicoli in PITRE).
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="filtriRicerca">Criteri per filtrare i risultati della ricerca. E' necessario specificare almeno l'attributo di ricerca "Classificazione"</param>
        /// <param name="criteriPaginazione">Opzionale. Consente di specificare i criteri per paginare i risultati della ricerca. Se non definito, non sarà applicata alcuna paginazione ai dati</param>
        /// <returns>Esito della ricerca pratiche effettuata</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        public ElencoPratiche GetPratiche(string userIdRichiedente, FiltriRicercaPratiche filtriRicerca, CriteriPaginazione criteriPaginazione)
        {
            try
            {
                this.CheckStringParameter("userIdRichiedente", userIdRichiedente);
                this.CheckFiltriRicercaPratiche(filtriRicerca);

                // Reperimento oggetto InfoUtente
                DocsPaVO.utente.InfoUtente infoUtente = DocsPaServices.DocsPaServiceHelper.Impersonate(userIdRichiedente);

                if (criteriPaginazione == null)
                {
                    // Se la paginazione non è impostata
                    criteriPaginazione = new CriteriPaginazione
                    {
                        Pagina = 1,
                        OggettiPerPagina = Int32.MaxValue
                    };
                }

                DocsPaVO.ricerche.SearchPagingContext pagingContext = new DocsPaVO.ricerche.SearchPagingContext
                {
                    Page = criteriPaginazione.Pagina,
                    PageSize = criteriPaginazione.OggettiPerPagina,
                    GetIdProfilesList = false
                };

                List<Pratica> pratiche = new List<Pratica>();

                // Reperimento filtri per la ricerca delle pratiche
                DocsPaVO.filtri.FiltroRicerca[] filtri = this.GetFiltriRicercaPratiche(infoUtente, filtriRicerca);

                DocsPaVO.filtri.FiltroRicerca[] filtriRicercaDocumenti = null;

                if (filtriRicerca.FiltroDocumenti != null)
                {
                    // Reperimento dei filtri per documenti contenuti nei fascicoli
                    filtriRicercaDocumenti = this.GetFiltriRicercaDocumenti(infoUtente, filtriRicerca.FiltroDocumenti, true);
                }

                foreach (DocsPaVO.fascicolazione.Fascicolo fascicolo in DocsPaServices.DocsPaServiceHelper.GetFascicoli(infoUtente, filtriRicerca.ClassificazionePratica, filtriRicerca.CercaInClassificazioniFiglie, filtri, filtriRicercaDocumenti, pagingContext))
                {
                    Pratica pratica = this.CreatePratica(infoUtente, fascicolo);

                    // Se è stato specificato il filtro "SoloPraticheNonAssegnate",
                    // si escludono dal risultato finale le pratiche cui l'oggetto bene non
                    // è stato valorizzato
                    //if (!filtriRicerca.SoloPraticheNonAssegnate ||
                    //    (filtriRicerca.SoloPraticheNonAssegnate && !pratica.BeneSpecified))
                    pratiche.Add(pratica);
                    //else
                    //    pagingContext.SetRecordCount(pagingContext.RecordCount - 1);
                }

                ElencoPratiche retValue = new ElencoPratiche();
                criteriPaginazione.TotaleOggetti = pagingContext.RecordCount;
                criteriPaginazione.TotalePagine = pagingContext.PageCount;

                retValue.Pratiche = pratiche.ToArray();
                retValue.Paginazione = criteriPaginazione;
                return retValue;
            }
            catch (Exception ex)
            {
                logger.Debug(ex);

                throw new FaultException<P3OperationFault>(new P3OperationFault { Message = ex.Message }, new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Servizio per il reperimento di una Pratica in SBC
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="numeroPratica">Numero della pratica in SBC</param>
        /// <remarks>
        /// Corrisponde ad un Fascicolo in PITRE
        /// </remarks>
        /// <returns>Metadati della pratica richiesta</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        public Pratica GetPratica(string userIdRichiedente, string numeroPratica)
        {
            try
            {
                this.CheckStringParameter("userIdRichiedente", userIdRichiedente);
                this.CheckStringParameter("numeroPratica", numeroPratica);

                // Reperimento oggetto InfoUtente
                DocsPaVO.utente.InfoUtente infoUtente = DocsPaServices.DocsPaServiceHelper.Impersonate(userIdRichiedente);

                // Reperimento del fascicolo
                DocsPaVO.fascicolazione.Fascicolo fascicolo = DocsPaServices.DocsPaServiceHelper.GetFascicolo(infoUtente, numeroPratica);

                return this.CreatePratica(infoUtente, fascicolo);
            }
            catch (Exception ex)
            {
                logger.Debug(ex);

                throw new FaultException<P3OperationFault>(new P3OperationFault { Message = ex.Message }, new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Servizio per il reperimento di una o più Pratiche in SBC
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="numeriPratica">
        /// Identificativi di una o più pratiche SBC per cui si vogliono reperire i metadati
        /// </param>
        /// <remarks>
        /// Corrisponde ad un Fascicolo in PITRE
        /// </remarks>
        /// <returns>Metadati delle pratiche richieste</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        public Pratica[] GetPraticheDaNumeri(string userIdRichiedente, string[] numeriPratica)
        {
            var pratiche = from c in numeriPratica
                           select this.GetPratica(userIdRichiedente, c);

            return pratiche.ToArray<Pratica>();
        }

        /// <summary>
        /// Servizio per l'inserimento di un fascicolo in PITRE (pratica in SBC) di una particolare tipologia già configurata 
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="pratica">Dati della pratica SBC da inserire come fascicolo in PITRE</param>
        /// <returns>Metadati della pratica inserita</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        public Pratica InsertPratica(string userIdRichiedente, Pratica pratica)
        {
            try
            {
                this.CheckStringParameter("userIdRichiedente", userIdRichiedente);
                this.CheckObjectParameter("pratica", pratica);
                this.CheckStringParameter("Descrizione", pratica.Descrizione);

                //if (!pratica.BeneSpecified || pratica.Bene == null)
                //    throw new ApplicationException("Nessun Bene specificato per la pratica");

                // Reperimento oggetto InfoUtente
                DocsPaVO.utente.InfoUtente infoUtente = DocsPaServices.DocsPaServiceHelper.Impersonate(userIdRichiedente);

                // Reperimento oggetto template per la profilazione (il fascicolo deve essere della tipologia BENE)
                string idTemplate = DocsPaServices.DocsPaServiceHelper.GetTemplateTipoFascicolo(infoUtente, pratica.Tipo).SYSTEM_ID.ToString();
                DocsPaVO.ProfilazioneDinamica.Templates templateProfilazione = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(idTemplate);

                List<DocsPaVO.ProfilazioneDinamica.OggettoCustom> oggettiCustom = new List<DocsPaVO.ProfilazioneDinamica.OggettoCustom>((DocsPaVO.ProfilazioneDinamica.OggettoCustom[])templateProfilazione.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom)));

                if (pratica.BeneSpecified && pratica.Bene != null)
                {
                    // Impostazione valore per i campi custom per la tipologia bene (se specificato)
                    DocsPaServices.DocsPaServiceHelper.SetValoreOggettoCustom(oggettiCustom, DocsPaServices.TipoPratica.PRESENZA_BENE, "Presente");
                    DocsPaServices.DocsPaServiceHelper.SetValoreOggettoCustom(oggettiCustom, DocsPaServices.TipoPratica.CODICE_BENE, pratica.Bene.Codice);
                    DocsPaServices.DocsPaServiceHelper.SetValoreOggettoCustom(oggettiCustom, DocsPaServices.TipoPratica.DESCRIZIONE_BENE, pratica.Bene.Descrizione);
                }
                else
                {
                    // Impostazione valori di default del bene (se non specificato)
                    DocsPaServices.DocsPaServiceHelper.SetValoreOggettoCustom(oggettiCustom, DocsPaServices.TipoPratica.PRESENZA_BENE, "Non Presente");
                    DocsPaServices.DocsPaServiceHelper.SetValoreOggettoCustom(oggettiCustom, DocsPaServices.TipoPratica.CODICE_BENE, string.Empty);
                    DocsPaServices.DocsPaServiceHelper.SetValoreOggettoCustom(oggettiCustom, DocsPaServices.TipoPratica.DESCRIZIONE_BENE, string.Empty);
                }

                DocsPaVO.utente.Ruolo ruoloPreferito = DocsPaServices.DocsPaServiceHelper.GetRuoloPreferito(infoUtente);

                DocsPaVO.fascicolazione.Classificazione classificazione = DocsPaServices.DocsPaServiceHelper.GetClassificazione(infoUtente, pratica.ClassificazionePratica);
                classificazione.registro = DocsPaServices.DocsPaServiceHelper.GetRegistroRuoloPreferito(infoUtente, ruoloPreferito);

                // La data di apertura è impostata automaticamente dal sistema
                DateTime dataApertura = DateTime.Now;

                DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo
                {
                    descrizione = pratica.Descrizione,
                    apertura = dataApertura.ToString("dd/MM/yyyy"),
                    codiceGerarchia = pratica.ClassificazionePratica,
                    template = templateProfilazione,
                    tipo = templateProfilazione.SYSTEM_ID.ToString(),
                    codUltimo = BusinessLogic.Fascicoli.FascicoloManager.getFascNumRif(classificazione.systemID, classificazione.idRegistroNodoTit),
                    idRegistro = classificazione.idRegistroNodoTit
                };

                DocsPaVO.fascicolazione.ResultCreazioneFascicolo resultCreazione;
                fascicolo = BusinessLogic.Fascicoli.FascicoloManager.newFascicolo(classificazione, fascicolo, infoUtente, ruoloPreferito, false, out resultCreazione);

                if (resultCreazione != DocsPaVO.fascicolazione.ResultCreazioneFascicolo.OK)
                    throw new ApplicationException(resultCreazione.ToString());

                pratica.Numero = fascicolo.codice;
                pratica.DataApertura = dataApertura;

                if (pratica.ProtocolloSpecified && pratica.Protocollo != null)
                {
                    // Se è specificato il protocollo di apertura pratica, viene inserito nella pratica
                    this.AddDocumentoPratica(infoUtente, pratica.Numero, pratica.Protocollo.Numero);
                }

                if (pratica.ProvvedimentoSpecified && pratica.Provvedimento != null)
                {
                    // Se è specificato il provvedimento, viene inserito nella pratica
                    this.AddDocumentoPratica(infoUtente, pratica.Numero, pratica.Provvedimento.Numero);
                }

                // Gestione diagramma di stato
                if (fascicolo.template != null)
                {
                    // Esecuzione del primo step del diagramma di stato associato al fascicolo
                    DocsPaServices.DocsPaWorkflowServiceHelper.SetOnFirstWorkflowStep(infoUtente, fascicolo);
                }

                return pratica;
            }
            catch (Exception ex)
            {
                logger.Debug(ex);

                throw new FaultException<P3OperationFault>(new P3OperationFault { Message = ex.Message }, new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Servizio per l'aggiornamento dei dati di un fascicolo esistente in PITRE (pratica in SBC)
        /// </summary>
        /// <param name="userIdRichiedente">UserID dell'utente PITRE che esegue la richiesta</param>
        /// <param name="pratica">Dati della pratica SBC da modificare nel corrispondente fascicolo in PITRE</param>
        /// <returns>Metadati della pratica aggiornata</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        public Pratica UpdatePratica(string userIdRichiedente, Pratica pratica)
        {
            try
            {
                this.CheckStringParameter("userIdRichiedente", userIdRichiedente);
                this.CheckObjectParameter("pratica", pratica);

                //if (!pratica.BeneSpecified || pratica.Bene == null)
                //    throw new ApplicationException("Nessun Bene specificato per la pratica");

                // Reperimento oggetto InfoUtente
                DocsPaVO.utente.InfoUtente infoUtente = DocsPaServices.DocsPaServiceHelper.Impersonate(userIdRichiedente);

                // Aggiornamento dei dati della pratica
                DocsPaVO.fascicolazione.Fascicolo fascicolo = DocsPaServices.DocsPaServiceHelper.GetFascicolo(infoUtente, pratica.Numero);

                fascicolo.descrizione = pratica.Descrizione;

                if (pratica.BeneSpecified && pratica.Bene != null)
                {
                    List<DocsPaVO.ProfilazioneDinamica.OggettoCustom> oggettiCustom = new List<DocsPaVO.ProfilazioneDinamica.OggettoCustom>((DocsPaVO.ProfilazioneDinamica.OggettoCustom[])fascicolo.template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom)));
                    DocsPaServices.DocsPaServiceHelper.SetValoreOggettoCustom(oggettiCustom, DocsPaServices.TipoPratica.PRESENZA_BENE, "Presente");
                    DocsPaServices.DocsPaServiceHelper.SetValoreOggettoCustom(oggettiCustom, DocsPaServices.TipoPratica.CODICE_BENE, pratica.Bene.Codice);
                    DocsPaServices.DocsPaServiceHelper.SetValoreOggettoCustom(oggettiCustom, DocsPaServices.TipoPratica.DESCRIZIONE_BENE, pratica.Bene.Descrizione);
                }

                try
                {
                    BusinessLogic.Fascicoli.FascicoloManager.setFascicolo(infoUtente, fascicolo);
                }
                catch
                {
                    throw new ApplicationException(string.Format("Errore nell'aggiornamento della pratica {0}", pratica.Numero));
                }

                if (pratica.ProtocolloSpecified && pratica.Protocollo != null)
                {
                    // Se è specificato il protocollo di apertura pratica, viene inserito nella pratica
                    this.AddDocumentoPratica(infoUtente, pratica.Numero, pratica.Protocollo.Numero);
                }

                if (pratica.ProvvedimentoSpecified && pratica.Provvedimento != null)
                {
                    // Se è specificato il provvedimento, viene inserito nella pratica
                    this.AddDocumentoPratica(infoUtente, pratica.Numero, pratica.Provvedimento.Numero);
                }

                return pratica;
            }
            catch (Exception ex)
            {
                logger.Debug(ex);

                throw new FaultException<P3OperationFault>(new P3OperationFault { Message = ex.Message }, new FaultReason(ex.Message));
            }
        }

        /// <summary>
        /// Servizio per la ricerca degli utenti attivi in PITRE
        /// </summary>
        /// <param name="filtriRicerca">
        /// Criteri per filtrare i risultati della ricerca
        /// <remarks>
        /// Come filtro obbligatorio è necessario definire almeno una Classificazione,
        /// in quanto saranno reperiti solamente gli utenti che ne avranno la visibilità 
        /// </remarks>
        /// </param>
        /// <param name="criteriPaginazione">Opzionale. Consente di specificare i criteri per paginare i risultati della ricerca. Se non definito, non sarà applicata alcuna paginazione ai dati</param>
        /// <returns>Esito della ricerca utenti effettuata</returns>
        /// <exception cref="P3SBCLib.Contracts.P3OperationFault">
        /// Errore nell'esecuzione dell'operazione
        /// </exception>
        public ElencoUtenti GetUtenti(FiltriRicercaUtenti filtriRicerca, CriteriPaginazione criteriPaginazione)
        {
            try
            {
                this.CheckFiltriRicercaUtenti(filtriRicerca);

                if (criteriPaginazione == null)
                {
                    // Se la paginazione non è impostata
                    criteriPaginazione = new CriteriPaginazione
                    {
                        Pagina = 1,
                        OggettiPerPagina = Int32.MaxValue
                    };
                }

                DocsPaVO.ricerche.SearchPagingContext pagingContext = new DocsPaVO.ricerche.SearchPagingContext
                {
                    Page = criteriPaginazione.Pagina,
                    PageSize = criteriPaginazione.OggettiPerPagina
                };

                // Reperimento filtri di ricerca per gli utenti
                DocsPaVO.filtri.FiltroRicerca[] filtri = this.GetFiltriRicercaUtenti(filtriRicerca);

                var a = from c in DocsPaServices.DocsPaServiceHelper.GetUtenti(filtriRicerca.Classificazioni, filtri, pagingContext)
                        select new Utente { UserId = c.UserId, Cognome = c.Cognome, Nome = c.Nome, Email = c.Email };

                criteriPaginazione.TotaleOggetti = pagingContext.RecordCount;
                criteriPaginazione.TotalePagine = pagingContext.PageCount;

                ElencoUtenti retValue = new ElencoUtenti();
                retValue.Utenti = a.ToArray<Utente>();
                retValue.Paginazione = criteriPaginazione;
                return retValue;
            }
            catch (Exception ex)
            {
                logger.Debug(ex);

                throw new FaultException<P3OperationFault>(new P3OperationFault { Message = ex.Message }, new FaultReason(ex.Message));
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Associazione di un documento ad una pratica
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="numeroPratica"></param>
        /// <param name="numeroDocumento"></param>
        protected virtual void AddDocumentoPratica(DocsPaVO.utente.InfoUtente infoUtente, string numeroPratica, string numeroDocumento)
        {
            this.CheckValiditaDocumentoInPratica(infoUtente, numeroDocumento);

            try
            {
                string msg = string.Empty;
                bool outValue = BusinessLogic.Fascicoli.FascicoloManager.addDocFascicolo(infoUtente,
                                    numeroDocumento,
                                    DocsPaServices.DocsPaServiceHelper.GetFascicolo(infoUtente, numeroPratica).systemID, false, out msg);
            }
            catch
            {
                throw new ApplicationException(string.Format("Errore nell'inserimento del documento {0} nella pratica {1}", numeroDocumento, numeroPratica));
            }
        }

        /// <summary>
        /// Rimozione di un documento associato ad una pratica
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="numeroPratica"></param>
        /// <param name="numeroDocumento"></param>
        protected void RemoveDocumentoPratica(DocsPaVO.utente.InfoUtente infoUtente, string numeroPratica, string numeroDocumento)
        {
            this.CheckValiditaDocumentoInPratica(infoUtente, numeroDocumento);

            try
            {
                DocsPaVO.fascicolazione.Fascicolo fascicolo = DocsPaServices.DocsPaServiceHelper.GetFascicolo(infoUtente, numeroPratica);

                string msg;
                DocsPaVO.Validations.ValidationResultInfo result =
                            BusinessLogic.Fascicoli.FolderManager.RemoveDocumentFromProject(infoUtente,
                                numeroDocumento,
                                DocsPaServices.DocsPaServiceHelper.GetFolderFascicolo(infoUtente, numeroPratica),
                                string.Empty,
                                out msg);
            }
            catch
            {
                throw new ApplicationException(string.Format("Errore nella rimozione del documento {0} della pratica {1}", numeroDocumento, numeroPratica));
            }
        }

        /// <summary>
        /// Reperimento file associato al documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="numeroDocumento"></param>
        /// <returns></returns>
        protected Documento GetFile(DocsPaVO.utente.InfoUtente infoUtente, string numeroDocumento)
        {
            // Download del file associato al documento
            DocsPaVO.documento.FileDocumento fileDocumento = DocsPaServices.DocsPaServiceHelper.GetFile(infoUtente, numeroDocumento);

            if (fileDocumento != null)
            {
                return new Documento
                {
                    Nomefile = System.IO.Path.GetFileName(fileDocumento.fullName),
                    Content = fileDocumento.content,
                    MimeType = fileDocumento.contentType
                };
            }
            else
                return null;
        }

        /// <summary>
        /// Reperimento del nome del file acquisito per la versione corrente del documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="numeroDocumento"></param>
        /// <returns></returns>
        protected string GetFileName(DocsPaVO.utente.InfoUtente infoUtente, string numeroDocumento)
        {
            return DocsPaServices.DocsPaServiceHelper.GetFileName(infoUtente, numeroDocumento);
        }

        /// <summary>
        /// Impostazione del file associato al documento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="numeroDocumento"></param>
        /// <param name="documento"></param>
        protected void SetFile(DocsPaVO.utente.InfoUtente infoUtente, string numeroDocumento, Documento documento)
        {
            DocsPaServices.DocsPaServiceHelper.SetFile(infoUtente, numeroDocumento, documento.Nomefile, documento.Content);
        }

        /// <summary>
        /// Creazione oggetto InfoDocumento
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="infoDocumento"></param>
        /// <returns></returns>
        protected virtual InfoDocumento CreateDocumento(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.InfoDocumento infoDocumento)
        {
            string sottoTipologia = null;

            if (!string.IsNullOrEmpty(infoDocumento.tipoAtto))
            {
                DocsPaVO.ProfilazioneDinamica.Templates templates =
                    BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getTemplateDettagli(infoDocumento.docNumber);

                sottoTipologia = (from c in templates.ELENCO_OGGETTI.Cast<DocsPaVO.ProfilazioneDinamica.OggettoCustom>()
                                  where c.DESCRIZIONE == "Sotto Tipologia"
                                  select c.VALORE_DATABASE).ElementAtOrDefault<string>(0);
            }

            return new InfoDocumento
            {
                Numero = infoDocumento.idProfile,
                Protocollo = infoDocumento.numProt,
                Segnatura = infoDocumento.segnatura,
                Data = DocsPaUtils.Functions.Functions.ToDate(infoDocumento.dataApertura),
                Oggetto = infoDocumento.oggetto,
                Stato = DocsPaServices.DocsPaServiceHelper.GetDescrizioneStatoDocumento(infoUtente, infoDocumento),
                Tipo = infoDocumento.tipoAtto,
                SottoTipologia = sottoTipologia,
                OperatoreInserimento = infoDocumento.autore,
                Soggetto = infoDocumento.mittDoc,
                Note = DocsPaServices.DocsPaServiceHelper.GetNotaDocumento(infoUtente, infoDocumento),
                Nomefile = this.GetFileName(infoUtente, infoDocumento.idProfile)
            };
        }

        /// <summary>
        /// Creazione oggetto pratica
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="fascicolo"></param>
        /// <returns></returns>
        protected virtual Pratica CreatePratica(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            Pratica pratica = new Pratica
            {
                Numero = fascicolo.codice,
                Descrizione = fascicolo.descrizione,
                Tipo = (fascicolo.template != null ? fascicolo.template.DESCRIZIONE : string.Empty),
                DataApertura = DocsPaUtils.Functions.Functions.ToDate(fascicolo.apertura),
                ClassificazionePratica = fascicolo.codiceGerarchia,
                ProtocolloSpecified = false,
                ProvvedimentoSpecified = false
            };

            // Caricamento dati del bene nella pratica
            this.LoadBeneInPratica(pratica, fascicolo);

            // Caricamento del documento di tipo "Documento di Apertura"
            pratica.Protocollo = this.GetDocumentoPratica(infoUtente, P3SBCLib.DocsPaServices.TipoDocumentoAperturaPratica.TIPOLOGIA, pratica);
            pratica.ProtocolloSpecified = (pratica.Protocollo != null);

            // Caricamento del documento di tipo "Provvedimento"
            pratica.Provvedimento = this.GetDocumentoPratica(infoUtente, P3SBCLib.DocsPaServices.TipoDocumentoProvvedimento.TIPOLOGIA, pratica);
            pratica.ProvvedimentoSpecified = (pratica.Provvedimento != null);

            return pratica;
        }

        /// <summary>
        /// Reperimento della prima occorrenza di un documento di una determinata tipologia in una pratica
        /// </summary>
        /// <param name="tipoDocumento"></param>
        /// <param name="pratica"></param>
        /// <remarks>
        /// Il metodo è necessario per reperire il documento di una tipologia (Apertura Pratica, Provvedimento)
        /// per l'associazione con la pratica
        /// </remarks>
        /// <returns></returns>
        protected virtual InfoDocumento GetDocumentoPratica(DocsPaVO.utente.InfoUtente infoUtente, string tipoDocumento, Pratica pratica)
        {
            FiltroRicercaDocumenti filtro = new FiltroRicercaDocumenti
            {
                TipoDocumento = tipoDocumento,
                NumeroPratica = pratica.Numero,
            };

            DocsPaVO.filtri.FiltroRicerca[] filtri = this.GetFiltriRicercaDocumenti(infoUtente, filtro, false);

            var doc = from c in DocsPaServices.DocsPaServiceHelper.GetDocumenti(infoUtente,
                                filtri,
                                new DocsPaVO.ricerche.SearchPagingContext { Page = 1, PageSize = Int32.MaxValue, GetIdProfilesList = false })
                      select this.CreateDocumento(infoUtente, c);

            return doc.FirstOrDefault();
        }

        /// <summary>
        /// Creazione di un nuovo oggetto custom clonando un oggetto custom
        /// </summary>
        /// <param name="fromOggettoCustom"></param>
        /// <param name="valoreDatabase"></param>
        /// <param name="campoComune"></param>
        /// <param name="tipoRicercaStringa"></param>
        /// <returns></returns>
        protected DocsPaVO.ProfilazioneDinamica.OggettoCustom CreateOggettoCustom(DocsPaVO.ProfilazioneDinamica.OggettoCustom fromOggettoCustom, string valoreDatabase, bool campoComune, DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum tipoRicercaStringa)
        {
            DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = null;

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, fromOggettoCustom);
                stream.Position = 0;
                oggettoCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom)formatter.Deserialize(stream);
                oggettoCustom.VALORE_DATABASE = valoreDatabase;
                oggettoCustom.TIPO_RICERCA_STRINGA = tipoRicercaStringa;
                oggettoCustom.CAMPO_COMUNE = (campoComune ? "1" : "0");
            }

            return oggettoCustom;
        }

        /// <summary>
        /// Caricamento dati del bene nella pratica
        /// </summary>
        /// <param name="praticaSbc"></param>
        /// <param name="fascicoloPitre"></param>
        /// <returns></returns>
        protected virtual void LoadBeneInPratica(Pratica praticaSbc, DocsPaVO.fascicolazione.Fascicolo fascicoloPitre)
        {
            if (fascicoloPitre.template != null && fascicoloPitre.template.ELENCO_OGGETTI.Count > 0)
            {
                var oggettiCustom = fascicoloPitre.template.ELENCO_OGGETTI.Cast<DocsPaVO.ProfilazioneDinamica.OggettoCustom>();

                DocsPaVO.ProfilazioneDinamica.OggettoCustom campoPresenzaBene = oggettiCustom.Where(e => e.DESCRIZIONE == DocsPaServices.TipoPratica.PRESENZA_BENE).FirstOrDefault();

                if (campoPresenzaBene != null && campoPresenzaBene.VALORE_DATABASE == "Presente")
                {
                    string codice = (from c in oggettiCustom
                                     where c.DESCRIZIONE == DocsPaServices.TipoPratica.CODICE_BENE
                                     select c.VALORE_DATABASE).ElementAtOrDefault<string>(0);

                    string descrizione = (from c in oggettiCustom
                                          where c.DESCRIZIONE == DocsPaServices.TipoPratica.DESCRIZIONE_BENE
                                          select c.VALORE_DATABASE).ElementAtOrDefault<string>(0);

                    if (!string.IsNullOrEmpty(codice) || !string.IsNullOrEmpty(descrizione))
                    {
                        praticaSbc.Bene = new Bene { Codice = codice, Descrizione = descrizione };
                        praticaSbc.BeneSpecified = true;
                    }
                }
            }
            else
            {
                praticaSbc.BeneSpecified = false;
                praticaSbc.Bene = null;
            }
        }

        /// <summary>
        /// Creazione filtri di ricerca per le pratiche
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="filtriRicerca"></param>
        /// <returns></returns>
        protected DocsPaVO.filtri.FiltroRicerca[] GetFiltriRicercaPratiche(DocsPaVO.utente.InfoUtente infoUtente, FiltriRicercaPratiche filtriRicerca)
        {
            List<DocsPaVO.filtri.FiltroRicerca> filtri = new List<DocsPaVO.filtri.FiltroRicerca>();

            // Filtro per codice del fascicolo (numero pratica in SBC)
            AppendFiltroRicerca(filtri, DocsPaVO.filtri.fascicolazione.listaArgomenti.CODICE_FASCICOLO.ToString(), filtriRicerca.NumeroPratica);
            AppendFiltroRicerca(filtri, DocsPaVO.filtri.fascicolazione.listaArgomenti.INCLUDI_FASCICOLI_FIGLI.ToString(), (filtriRicerca.CercaInClassificazioniFiglie ? "S" : "N"));

            // Verifica se deve essere effettuata la ricerca per campi profilati
            bool ricercaProfilazione = (!string.IsNullOrEmpty(filtriRicerca.TipoPratica) ||
                                        filtriRicerca.SoloPraticheNonAssegnate ||
                                        (filtriRicerca.CodiciBene != null && filtriRicerca.CodiciBene.Length > 0));

            if (ricercaProfilazione)
            {
                // Tipo fascicolo campi comuni
                const string TIPO_FASCICOLO_CAMPI_COMUNI = "CAMPI COMUNI";

                string tipoPratica = string.Empty;

                if (string.IsNullOrEmpty(filtriRicerca.TipoPratica))
                    // La ricerca viene effettuata per campi comuni (se non specificato un tipo fascicolo)
                    tipoPratica = TIPO_FASCICOLO_CAMPI_COMUNI;
                else
                    tipoPratica = filtriRicerca.TipoPratica;

                // Reperimento dell'id template
                string idTemplate = DocsPaServices.DocsPaServiceHelper.GetTemplateTipoFascicolo(infoUtente, tipoPratica).SYSTEM_ID.ToString();

                // Reperimento dell'id della tipologia del fascicolo a partire dal tipo pratica
                AppendFiltroRicerca(filtri, DocsPaVO.filtri.fascicolazione.listaArgomenti.TIPOLOGIA_FASCICOLO.ToString(), idTemplate);

                // Reperimento dell'oggetto template associato al fascicolo
                DocsPaVO.ProfilazioneDinamica.Templates template = BusinessLogic.ProfilazioneDinamica.ProfilazioneFascicoli.getTemplateFascById(idTemplate);

                List<DocsPaVO.ProfilazioneDinamica.OggettoCustom> oggettiCustom = new List<DocsPaVO.ProfilazioneDinamica.OggettoCustom>((DocsPaVO.ProfilazioneDinamica.OggettoCustom[])template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom)));

                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustomPresenzaBene =
                    DocsPaServices.DocsPaServiceHelper.GetOggettoCustom(oggettiCustom,
                                    DocsPaServices.TipoPratica.PRESENZA_BENE);

                oggettoCustomPresenzaBene.CAMPO_COMUNE = "1";

                if (filtriRicerca.SoloPraticheNonAssegnate)
                {
                    // Ricerca delle sole pratiche cui il bene non è stato ancora materialmente assegnato

                    // Impostazione del valore del campo come "Non Presente"
                    oggettoCustomPresenzaBene.VALORE_DATABASE = "Non Presente";
                }
                else
                {
                    // Ricerca di tutte le pratiche indipendentemente dalla presenza o meno del bene

                    // Impostazione del valore del campo come ""
                    oggettoCustomPresenzaBene.VALORE_DATABASE = string.Empty;

                    if (filtriRicerca.CodiciBene != null && filtriRicerca.CodiciBene.Length > 0)
                    {
                        // Ricerca degli oggetti nel template con l'operatore OR
                        template.OPERATORE_RICERCA_OGGETTI = DocsPaVO.ProfilazioneDinamica.OperatoriRicercaOggettiCustomEnum.Or;

                        // Reperimento oggetto custom del codice bene
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustomBene = DocsPaServices.DocsPaServiceHelper.GetOggettoCustom(oggettiCustom, DocsPaServices.TipoPratica.CODICE_BENE);

                        // Filtri sui beni specificati (campi custom)
                        var items = from bene in filtriRicerca.CodiciBene
                                    select this.CreateOggettoCustom(oggettoCustomBene, bene, true, DocsPaVO.ProfilazioneDinamica.TipoRicercaStringaEnum.PAROLA_INTERA);

                        template.ELENCO_OGGETTI = new System.Collections.ArrayList(items.ToArray<DocsPaVO.ProfilazioneDinamica.OggettoCustom>());
                    }
                }


                if (template.ELENCO_OGGETTI.Count > 0)
                {
                    filtri.Add(new DocsPaVO.filtri.FiltroRicerca
                    {
                        argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.PROFILAZIONE_DINAMICA.ToString(),
                        valore = "Profilazione Dinamica",
                        template = template
                    });
                }
            }

            // Ricerca dei soli fascicoli procedimentali
            AppendFiltroRicerca(filtri, DocsPaVO.filtri.fascicolazione.listaArgomenti.TIPO_FASCICOLO.ToString(), "P");

            // Filtro per data apertura pratica
            this.AppendFiltriRicercaData(filtri, DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_IL.ToString(),
                                                DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_SUCCESSIVA_AL.ToString(),
                                                DocsPaVO.filtri.fascicolazione.listaArgomenti.APERTURA_PRECEDENTE_IL.ToString(),
                                                filtriRicerca.DataAperturaPratica, filtriRicerca.DataAperturaPraticaFinale);

            return filtri.ToArray();
        }

        /// <summary>
        /// Creazione filtri di ricerca per i documenti
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="filtriRicerca"></param>
        /// <param name="ricercaPratiche"></param>
        /// <returns></returns>
        protected DocsPaVO.filtri.FiltroRicerca[] GetFiltriRicercaDocumenti(DocsPaVO.utente.InfoUtente infoUtente, FiltroRicercaDocumenti filtriRicerca, bool ricercaPratiche)
        {
            List<DocsPaVO.filtri.FiltroRicerca> filtri = new List<DocsPaVO.filtri.FiltroRicerca>();

            // Inserimento dei filtri di ricerca
            this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.PROT_ARRIVO.ToString(), Boolean.TrueString);
            this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.PROT_PARTENZA.ToString(), Boolean.TrueString);
            this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.PROT_INTERNO.ToString(), Boolean.TrueString);
            this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.GRIGIO.ToString(), Boolean.TrueString);
            this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.ALLEGATO.ToString(), Boolean.FalseString);
            this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.PREDISPOSTO.ToString(), Boolean.FalseString);
            this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.DA_PROTOCOLLARE.ToString(), "0");

            if (ricercaPratiche)
                this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.TIPO.ToString(), "T");
            else
                this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.TIPO.ToString(), "tipo");

            this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.DOCNUMBER.ToString(), filtriRicerca.Numero);
            this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.OGGETTO.ToString(), filtriRicerca.Oggetto);
            this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.MITT_DEST.ToString(), filtriRicerca.Soggetto);
            this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.SEGNATURA.ToString(), filtriRicerca.Segnatura);
            this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.NUM_PROTOCOLLO.ToString(), filtriRicerca.Protocollo);

            // Filtro su data di protocollo
            this.AppendFiltriRicercaData(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.DATA_PROT_IL.ToString(),
                                                DocsPaVO.filtri.ricerca.listaArgomenti.DATA_PROT_SUCCESSIVA_AL.ToString(),
                                                DocsPaVO.filtri.ricerca.listaArgomenti.DATA_PROT_PRECEDENTE_IL.ToString(),
                                                filtriRicerca.DataDocumento, filtriRicerca.DataDocumentoFinale);

            // Filtro per data di creazione documento
            this.AppendFiltriRicercaData(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.DATA_CREAZIONE_IL.ToString(),
                                                DocsPaVO.filtri.ricerca.listaArgomenti.DATA_CREAZIONE_SUCCESSIVA_AL.ToString(),
                                                DocsPaVO.filtri.ricerca.listaArgomenti.DATA_CREAZIONE_PRECEDENTE_IL.ToString(),
                                                filtriRicerca.DataCreazioneDocumento, filtriRicerca.DataCreazioneDocumentoFinale);

            if (!string.IsNullOrEmpty(filtriRicerca.TipoDocumento))
            {
                string idTemplate = DocsPaServices.DocsPaServiceHelper.GetTemplateTipoDocumento(infoUtente, filtriRicerca.TipoDocumento).SYSTEM_ID.ToString();

                this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.TIPO_ATTO.ToString(), idTemplate);
            }

            this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.OGGETTO.ToString(), filtriRicerca.Oggetto);
            this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.MITT_DEST.ToString(), filtriRicerca.Soggetto);

            // Ricerca dei documenti contenuti nella pratica richiesta
            if (!string.IsNullOrEmpty(filtriRicerca.NumeroPratica))
                this.AppendFiltroRicerca(filtri, DocsPaVO.filtri.ricerca.listaArgomenti.CODICE_FASCICOLO.ToString(), filtriRicerca.NumeroPratica);

            return filtri.ToArray();
        }

        /// <summary>
        /// Reperimento dei filtri di ricerca per gli utenti
        /// </summary>
        /// <param name="filtriRicerca"></param>
        /// <returns></returns>
        protected DocsPaVO.filtri.FiltroRicerca[] GetFiltriRicercaUtenti(FiltriRicercaUtenti filtriRicerca)
        {
            List<DocsPaVO.filtri.FiltroRicerca> filtri = new List<DocsPaVO.filtri.FiltroRicerca>();

            AppendFiltroRicerca(filtri, "UserId", filtriRicerca.UserId);
            AppendFiltroRicerca(filtri, "Cognome", filtriRicerca.Cognome);
            AppendFiltroRicerca(filtri, "Nome", filtriRicerca.Nome);
            AppendFiltroRicerca(filtri, "Email", filtriRicerca.Email);
            AppendFiltroRicerca(filtri, "Sede", filtriRicerca.Sede);

            return filtri.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filtri"></param>
        /// <param name="argomento"></param>
        /// <param name="valore"></param>
        protected void AppendFiltroRicerca(List<DocsPaVO.filtri.FiltroRicerca> filtri, string argomento, string valore)
        {
            if (!string.IsNullOrEmpty(argomento) && !string.IsNullOrEmpty(valore))
                filtri.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = argomento, valore = valore.Replace("'", "''") });
        }

        /// <summary>
        /// Accodamento filtri per data e intervallo di date
        /// </summary>
        /// <param name="filtri"></param>
        /// <param name="filtroData"></param>
        /// <param name="filtroDataIniziale"></param>
        /// <param name="filtroDataFinale"></param>
        /// <param name="dataIniziale"></param>
        /// <param name="dataFinale"></param>
        protected void AppendFiltriRicercaData(List<DocsPaVO.filtri.FiltroRicerca> filtri,
                                                string filtroData, string filtroDataIniziale, string filtroDataFinale,
                                                Nullable<DateTime> dataIniziale, Nullable<DateTime> dataFinale)
        {
            if (dataIniziale.HasValue && dataFinale.HasValue)
            {
                this.AppendFiltroRicerca(filtri, filtroDataIniziale, dataIniziale.Value.ToString("dd/MM/yyyy"));
                this.AppendFiltroRicerca(filtri, filtroDataFinale, dataFinale.Value.ToString("dd/MM/yyyy"));
            }
            else if (dataIniziale.HasValue)
                AppendFiltroRicerca(filtri, filtroData, dataIniziale.Value.ToString("dd/MM/yyyy"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userIdRichiedente"></param>
        /// <returns></returns>
        protected DocsPaVO.utente.InfoUtente Impersonate(string userIdRichiedente)
        {
            return DocsPaServices.DocsPaServiceHelper.Impersonate(userIdRichiedente);
        }

        /// <summary>
        /// Verifica della validità del documento nell'ambito della gestione della pratica,
        /// ovvero se classificabile o meno
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="numeroDocumento"></param>
        /// <returns></returns>
        protected void CheckValiditaDocumentoInPratica(DocsPaVO.utente.InfoUtente infoUtente, string numeroDocumento)
        {
            DocsPaVO.documento.InfoDocumento infoDocumento = null;

            try
            {
                infoDocumento = BusinessLogic.Documenti.DocManager.GetInfoDocumento(infoUtente, numeroDocumento, numeroDocumento);
            }
            catch
            { }

            if (infoDocumento == null)
                throw new ApplicationException(string.Format("Documento {0} inesistente o non accessibile per l'utente", numeroDocumento));

            if (infoDocumento.allegato)
                throw new ApplicationException(string.Format("Il documento {0} è un allegato e non può essere gestito direttamente nella pratica", numeroDocumento));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        protected void CheckStringParameter(string parameterName, string parameterValue)
        {
            if (string.IsNullOrEmpty(parameterValue))
                throw new ApplicationException(string.Format("Parametro '{0}' mancante", parameterName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        protected void CheckObjectParameter(string parameterName, object parameterValue)
        {
            if (parameterValue == null)
                throw new ApplicationException(string.Format("Parametro '{0}' mancante", parameterName));
        }

        /// <summary>
        /// Controllo validità filtri di ricerca per gli utenti
        /// </summary>
        /// <param name="filtriRicerca"></param>
        protected virtual void CheckFiltriRicercaUtenti(FiltriRicercaUtenti filtriRicerca)
        {
            this.CheckObjectParameter("filtriRicerca", filtriRicerca);

            if (filtriRicerca.Classificazioni == null ||
                (filtriRicerca.Classificazioni != null && filtriRicerca.Classificazioni.Length == 0))
                throw new ApplicationException("Filtro di ricerca utenti per classificazioni mancante");

            foreach (string item in filtriRicerca.Classificazioni)
            {
                if (string.IsNullOrEmpty(item))
                    throw new ApplicationException("Filtro di ricerca utenti per classificazioni non valido");
            }
        }

        /// <summary>
        /// Controllo validità filtri di ricerca per gli utenti
        /// </summary>
        /// <param name="filtriRicerca"></param>
        protected virtual void CheckFiltriRicercaPratiche(FiltriRicercaPratiche filtriRicerca)
        {
            this.CheckObjectParameter("filtriRicerca", filtriRicerca);

            if (string.IsNullOrEmpty(filtriRicerca.ClassificazionePratica))
                throw new ApplicationException("Filtro di ricerca pratiche per classificazione mancante");
        }

        #endregion
    }
}
