using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using VtDocs.BusinessServices.Entities;
using DocsPaVO.documento;

namespace DocsPaWS.BusinessServices
{
    /// <summary>
    /// 
    /// </summary>
    [WebService(Namespace = "http://www.valueteam.com/VtDocs/Business/DocumentServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Script.Services.ScriptService()]
    public class DocumentServices : BusinessServices, VtDocs.BusinessServices.Document.IDocumentServices
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public virtual VtDocs.BusinessServices.Entities.Document.NewDocumentoResponse NewDocumento(VtDocs.BusinessServices.Entities.Document.NewDocumentoRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.NewDocumentoResponse response = new VtDocs.BusinessServices.Entities.Document.NewDocumentoResponse();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
                {
                    response.Documento = ws.NewSchedaDocumento(request.InfoUtente);

                    if (response.Documento == null)
                        throw new ApplicationException("Errore nella predisposizione del nuovo documento");

                    if (request.DocumentType == VtDocs.BusinessServices.Entities.Document.DocumentTypesEnum.NP)
                    {
                        response.Documento.tipoProto = VtDocs.BusinessServices.Entities.Document.DocumentTypesConstants.NP;
                        response.Documento.protocollo = null;
                    }
                    else if (request.DocumentType == VtDocs.BusinessServices.Entities.Document.DocumentTypesEnum.Arrivo)
                    {
                        response.Documento.tipoProto = VtDocs.BusinessServices.Entities.Document.DocumentTypesConstants.Arrivo;
                        response.Documento.protocollo = new DocsPaVO.documento.ProtocolloEntrata();
                    }
                    else if (request.DocumentType == VtDocs.BusinessServices.Entities.Document.DocumentTypesEnum.Partenza)
                    {
                        response.Documento.tipoProto = VtDocs.BusinessServices.Entities.Document.DocumentTypesConstants.Partenza;
                        response.Documento.protocollo = new DocsPaVO.documento.ProtocolloUscita();
                    }
                    else if (request.DocumentType == VtDocs.BusinessServices.Entities.Document.DocumentTypesEnum.Interno)
                    {
                        response.Documento.tipoProto = VtDocs.BusinessServices.Entities.Document.DocumentTypesConstants.Interno;
                        response.Documento.protocollo = new DocsPaVO.documento.ProtocolloInterno();
                    }

                    response.MittentiMultipliEnabled = ws.isEnableMittentiMultipli();

                    // ----------------------------------------------------------------------------------------------
                    // TODO: DA FARE METODO A PARTE

                    // Reperimento lista registri
                    if (request.GetRegistri)
                    {
                        // Reperimento della lista dei registri accessibili dall'utente
                        System.Collections.ArrayList listRegistri = ws.UtenteGetRegistri(request.InfoUtente.idCorrGlobali);

                        if (listRegistri == null)
                            throw new ApplicationException("Si è verificato un errore nel reperimento dei registri di protocollo");

                        if (listRegistri != null && listRegistri.Count == 0)
                            throw new ApplicationException("Nessun registro di protocollo disponibile per l'utente");

                        response.RegistriProtocollo = (DocsPaVO.utente.Registro[])listRegistri.ToArray(typeof(DocsPaVO.utente.Registro));
                    }

                    // ----------------------------------------------------------------------------------------------
                    // TODO: DA FARE METODO A PARTE

                    // Reperimento lista mezzi di spedizione
                    if (request.GetMezzoSpedizione)
                    {
                        System.Collections.ArrayList listMezziSpedizione = ws.AmmListaMezzoSpedizione(request.InfoUtente.idAmministrazione, false);
                        if (listMezziSpedizione == null)
                            throw new ApplicationException("Si è verificato un errore nel reperimento dei mezzi di spedizione");
                        if (listMezziSpedizione != null && listMezziSpedizione.Count == 0)
                            throw new ApplicationException("Nessun mezzo di spedizione disponibile per l'amministrazione corrente");
                        response.MezziSpedizione = (DocsPaVO.amministrazione.MezzoSpedizione[])listMezziSpedizione.ToArray(typeof(DocsPaVO.amministrazione.MezzoSpedizione));
                    }

                    // ----------------------------------------------------------------------------------------------
                    // TODO: DA FARE METODO A PARTE

                    // Reperimento delle tipologie documento disponibili
                    if (request.GetTipologieDocumento)
                    {
                        System.Collections.ArrayList listTipologie = ws.getTipoAttoPDInsRic(request.InfoUtente.idAmministrazione, request.InfoUtente.idGruppo, "2");

                        if (listTipologie != null)
                            response.TipiDocumento = (DocsPaVO.documento.TipologiaAtto[])listTipologie.ToArray(typeof(DocsPaVO.documento.TipologiaAtto));
                    }


                    // ----------------------------------------------------------------------------------------------
                    // TODO: DA FARE METODO A PARTE
                    // Reperimento della lista dei modelli trasmissione disponibili
                    if (request.GetModelliTrasmissione)
                    {
                        DocsPaVO.utente.Registro[] registri = null;

                        if (!request.GetRegistri)
                        {
                            // TODO
                        }
                        else
                            registri = response.RegistriProtocollo;

                        System.Collections.ArrayList listModelli = ws.getModelliPerTrasmLite(request.InfoUtente.idAmministrazione,
                                                                                  registri,
                                                                                  request.InfoUtente.idPeople,
                                                                                  request.InfoUtente.idCorrGlobali,
                                                                                  string.Empty,
                                                                                  string.Empty,
                                                                                  string.Empty,
                                                                                  "D",
                                                                                  string.Empty,
                                                                                  request.InfoUtente.idGruppo,
                                                                                  true, // estrazione di tutti i modelli indipendentemente dal registro
                                                                                  string.Empty);

                        if (listModelli != null)
                        {
                            response.ModelliTrasmissione = (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione[])
                                                listModelli.ToArray(typeof(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione));
                        }
                    }


                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response = new VtDocs.BusinessServices.Entities.Document.NewDocumentoResponse();
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipoDocumento"></param>
        /// <returns></returns>
        protected string GetDocNumberByCampoProfilato(VtDocs.BusinessServices.Entities.Document.GetDocumentoRequest.FiltroPerTipoDocumento tipoDocumento)
        {
            string docNumber = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                DocsPaUtils.Query queryDef = null;

                if (!string.IsNullOrEmpty(tipoDocumento.StatoDocumento))
                {
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_DOCNUMBER_BY_VALORE_CAMPO_PROFILATO_E_STATO");
                    queryDef.setParam("descrizioneStato", tipoDocumento.StatoDocumento);
                }
                else if (!string.IsNullOrEmpty(tipoDocumento.StatoDocumentoDiversoDa))
                {
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_DOCNUMBER_BY_VALORE_CAMPO_PROFILATO_E_STATO_DIVERSO_DA");
                    queryDef.setParam("descrizioneStato", tipoDocumento.StatoDocumentoDiversoDa);
                }
                else
                {
                    queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_DOCNUMBER_BY_VALORE_CAMPO_PROFILATO");
                }

                string descrizioneTipoDocumento = string.Empty;

                if (string.IsNullOrEmpty(tipoDocumento.DescrizioneTipoDocumento))
                    // Se la tipologia non è indicata esplicitamente, ricerca per campi comuni
                    descrizioneTipoDocumento = "CAMPI COMUNI";
                else
                    descrizioneTipoDocumento = tipoDocumento.DescrizioneTipoDocumento;

                queryDef.setParam("descrizioneTipoAtto", descrizioneTipoDocumento);

                if (tipoDocumento.CampiProfilati != null)
                {
                    string filtro = string.Empty;

                    foreach (var filtroCampoProfilato in tipoDocumento.CampiProfilati)
                    {
                        if (!string.IsNullOrEmpty(filtro))
                            filtro += " or ";
                        
                        filtro += string.Format("upper(oc.Descrizione) = upper('{0}') and upper(Valore_Oggetto_Db) = UPPER('{1}')",
                                    filtroCampoProfilato.NomeCampoProfilato, filtroCampoProfilato.ValoreCampoProfilato);
                    }

                    if (string.IsNullOrEmpty(filtro))
                        queryDef.setParam("filtroCampiProfilati", string.Empty);
                    else
                        queryDef.setParam("filtroCampiProfilati", string.Format(" and ({0})", filtro));
                }

                string commandText = queryDef.getSQL();
                
                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                {
                    docNumber = field;
                }
            }

            return docNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public virtual VtDocs.BusinessServices.Entities.Document.GetDocumentoResponse GetDocumento(VtDocs.BusinessServices.Entities.Document.GetDocumentoRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.GetDocumentoResponse response = new VtDocs.BusinessServices.Entities.Document.GetDocumentoResponse();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
                {
                    string idProfile = string.Empty;
                    string docNumber = string.Empty;
                    
                    if (request.TipoDocumento != null)
                    {
                        // Ricerca del documento per campo profilato
                        idProfile = this.GetDocNumberByCampoProfilato(request.TipoDocumento);
                        docNumber = idProfile;
                    }
                    else
                    {
                        idProfile = request.IdProfile;
                        docNumber = request.DocNumber;
                    }

                    response.Documento = ws.DocumentoGetDettaglioDocumento(request.InfoUtente, idProfile, docNumber);

                    if (response.Documento == null)
                        throw new ApplicationException("Errore nel reperimento dei dati del documento");

                    if (response.Documento.template != null &&
                        response.Documento.template.ELENCO_OGGETTI != null)
                    {
                        List<DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue> listCampiStoricizzati = new List<DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue>();

                        foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom obj in response.Documento.template.ELENCO_OGGETTI)
                        {
                            listCampiStoricizzati.Add(new DocsPaVO.ProfilazioneDinamica.StoricoProfilatiOldValue
                            {
                                IDTemplate = response.Documento.template.ID_TIPO_ATTO,
                                ID_Doc_Fasc = response.Documento.systemId,
                                ID_Oggetto = obj.SYSTEM_ID.ToString(),
                                Valore = obj.VALORE_DATABASE,
                                Tipo_Ogg_Custom = obj.TIPO.DESCRIZIONE_TIPO,
                                ID_People = request.InfoUtente.idPeople,
                                ID_Ruolo_In_UO = request.InfoUtente.idCorrGlobali
                            });
                        }

                        response.Documento.template.OLD_OGG_CUSTOM = new ArrayList(listCampiStoricizzati);
                    }

                    if (request.GetStatoDocumento && response.Documento.template != null)
                    {
                        // Reperimento dello stato corrente del documento, solo se a questo risulta associata una tipologia
                        response.StatoCorrente = ws.getStatoDoc(response.Documento.systemId);
                    }

                    //reperimento abilitazione mittenti multipli
                    response.EnableMittMult = ws.isEnableMittentiMultipli();

                    //reperimento lista mezzi di spedizione
                    if (request.GetMezzoSpedizione)
                    {
                        System.Collections.ArrayList listMezziSpedizione = ws.AmmListaMezzoSpedizione(request.InfoUtente.idAmministrazione, false);
                        if (listMezziSpedizione == null)
                            throw new ApplicationException("Si è verificato un errore nel reperimento dei mezzi di spedizione");
                        if (listMezziSpedizione != null && listMezziSpedizione.Count == 0)
                            throw new ApplicationException("Nessun mezzo di spedizione disponibile per l'amministrazione corrente");
                        response.MezziSpedizione = (DocsPaVO.amministrazione.MezzoSpedizione[])listMezziSpedizione.ToArray(typeof(DocsPaVO.amministrazione.MezzoSpedizione));
                    }

                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Documento = null;
                response.EnableMittMult = false;
                response.MezziSpedizione = null;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public virtual VtDocs.BusinessServices.Entities.Document.SaveDocumentoResponse SaveDocumento(VtDocs.BusinessServices.Entities.Document.SaveDocumentoRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.SaveDocumentoResponse response = new VtDocs.BusinessServices.Entities.Document.SaveDocumentoResponse();

            try
            {
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
                    {
                        DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(request.InfoUtente.idGruppo);

                        DocsPaVO.documento.SchedaDocumento scheda = request.Documento;

                        if (string.IsNullOrEmpty(scheda.codiceApplicazione))
                        {
                            // Impostazione del codice dell'applicazione in cui viene creato il documento
                            scheda.codiceApplicazione = request.InfoUtente.codWorkingApplication;
                        }

                        if (!string.IsNullOrEmpty(scheda.systemId))
                        {
                            // Modalità di modifica
                            bool daAggiornareUffRef;

                            // Impostazione dei flag per forzare il salvataggio
                            scheda.daAggiornarePrivato = true;

                            scheda = ws.DocumentoSaveDocumento(ruolo, request.InfoUtente, request.Documento, false, out daAggiornareUffRef);
                        }
                        else
                        {
                            if (scheda.tipoProto == VtDocs.BusinessServices.Entities.Document.DocumentTypesConstants.NP)
                            {
                                // Modalità di inserimento documento grigio
                                scheda = ws.DocumentoAddDocGrigia(scheda, request.InfoUtente, ruolo);
                            }
                            else
                            {
                                DocsPaVO.documento.ResultProtocollazione result;
                                scheda = ws.DocumentoProtocolla(scheda, request.InfoUtente, ruolo, out result);

                                if (result != DocsPaVO.documento.ResultProtocollazione.OK || scheda == null)
                                {
                                    throw new ApplicationException(string.Format("Errore nella protocollazione del documento: {0}", result));
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(request.IdFascicolo))
                        {
                            // Fascicolazione del documento
                            VtDocs.BusinessServices.Entities.Document.AddFascicoloResponse responseFascicolazione =
                                                this.AddFascicolo(
                                                                new VtDocs.BusinessServices.Entities.Document.AddFascicoloRequest
                                                                {
                                                                    InfoUtente = request.InfoUtente,
                                                                    IdFascicolo = request.IdFascicolo,
                                                                    IdProfile = scheda.systemId,
                                                                    GetFascicoli = false,
                                                                    TrowOnError = true
                                                                }
                                                            );

                            if (!responseFascicolazione.Success)
                                throw new ApplicationException(responseFascicolazione.Exception);
                        }

                        if (!string.IsNullOrEmpty(request.IdModelloTrasmissione))
                        {
                            // Trasmissione tramite modello
                            //bool setDataVistaResult = BusinessLogic.Documenti.DocManager.setDataVistaSP_TV(request.InfoUtente, request.Documento.systemId, "D");

                            VtDocs.BusinessServices.Trasmissioni.ModelliTrasmissione modelliTx = new VtDocs.BusinessServices.Trasmissioni.ModelliTrasmissione(request.InfoUtente);

                            modelliTx.Execute(scheda.systemId, DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO, request.IdModelloTrasmissione);
                        }

                        if (!string.IsNullOrEmpty(request.IdStatoDestinazione))
                        {
                            // Passaggio di stato del documento
                            DocsPaVO.DiagrammaStato.DiagrammaStato workflow = BusinessLogic.DiagrammiStato.DiagrammiStato.getDgByIdTipoFasc(scheda.template.SYSTEM_ID.ToString(), request.InfoUtente.idAmministrazione);

                            if (workflow != null)
                            {
                                DocsPaVO.DiagrammaStato.Stato[] stati = (DocsPaVO.DiagrammaStato.Stato[])
                                                    workflow.STATI.ToArray(typeof(DocsPaVO.DiagrammaStato.Stato));

                                DocsPaVO.DiagrammaStato.Stato statoDestinazione = stati.FirstOrDefault(e => e.DESCRIZIONE.ToLowerInvariant() == request.NomeStatoDestinazione.ToLowerInvariant());
                                if (statoDestinazione == null)
                                    throw new ApplicationException("Lo stato richiesto non è definito per il diagramma");

                                // Verifica se lo stato corrente del documento è diverso da quello richiesto
                                bool canExecute = true;

                                DocsPaVO.DiagrammaStato.Stato statoCorrente = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(scheda.systemId);
                                
                                if (statoCorrente != null)
                                    canExecute = (statoCorrente.SYSTEM_ID != statoDestinazione.SYSTEM_ID);

                                if (canExecute)
                                {
                                    string dataScadenza = string.Empty;

                                    // TODO: gestire la data scadenza


                                    // Passaggio di stato del documento
                                    BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStatoFasc(
                                                        scheda.systemId,
                                                        request.IdStatoDestinazione,
                                                        workflow,
                                                        request.InfoUtente.userId,
                                                        request.InfoUtente,
                                                        dataScadenza);

                                    // Esecuzione eventuale trasmissione associata allo stato del workflow
                                    VtDocs.BusinessServices.Trasmissioni.ModelliTrasmissione modelliTrasmissione = new VtDocs.BusinessServices.Trasmissioni.ModelliTrasmissione(request.InfoUtente);

                                    modelliTrasmissione.ExecuteStato(scheda.systemId, DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO, request.IdStatoDestinazione);
                                }
                            }
                        }
                        else if (!string.IsNullOrEmpty(request.NomeStatoDestinazione))
                        {
                            
                            // Passaggio di stato del documento
                            DocsPaVO.DiagrammaStato.DiagrammaStato workflow = BusinessLogic.DiagrammiStato.DiagrammiStato.getDgByIdTipoDoc(scheda.template.SYSTEM_ID.ToString(), request.InfoUtente.idAmministrazione);

                            if (workflow != null)
                            {
                                DocsPaVO.DiagrammaStato.Stato[] stati = (DocsPaVO.DiagrammaStato.Stato[])
                                        workflow.STATI.ToArray(typeof(DocsPaVO.DiagrammaStato.Stato));

                                DocsPaVO.DiagrammaStato.Stato statoDestinazione = stati.FirstOrDefault(e => e.DESCRIZIONE.ToLowerInvariant() == request.NomeStatoDestinazione.ToLowerInvariant());
                                if (statoDestinazione == null)
                                    throw new ApplicationException("Lo stato richiesto non è definito per il diagramma");

                                // Verifica se lo stato corrente del documento è diverso da quello richiesto
                                bool canExecute = true;

                                DocsPaVO.DiagrammaStato.Stato statoCorrente = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(scheda.systemId);
                                
                                if (statoCorrente != null)
                                    canExecute = (statoCorrente.SYSTEM_ID != statoDestinazione.SYSTEM_ID);

                                if (canExecute)
                                {
                                    string dataScadenza = string.Empty;

                                    // TODO: gestire la data scadenza

                                    // Passaggio di stato del documento
                                    BusinessLogic.DiagrammiStato.DiagrammiStato.salvaModificaStato(
                                                        scheda.systemId,
                                                        statoDestinazione.SYSTEM_ID.ToString(),
                                                        workflow,
                                                        request.InfoUtente.userId,
                                                        request.InfoUtente,
                                                        dataScadenza);

                                    // Esecuzione eventuale trasmissione associata allo stato del workflow
                                    VtDocs.BusinessServices.Trasmissioni.ModelliTrasmissione modelliTrasmissione = new VtDocs.BusinessServices.Trasmissioni.ModelliTrasmissione(request.InfoUtente);

                                    modelliTrasmissione.ExecuteStato(scheda.systemId, DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO, statoDestinazione.SYSTEM_ID.ToString());
                                }
                            }
                        }

                        response.StatoCorrente = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(scheda.systemId);

                        response.Documento = scheda;
                        response.Success = true;

                        transactionContext.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Documento = null;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per la cancellazione di un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public virtual VtDocs.BusinessServices.Entities.Document.DeleteDocumentoResponse DeleteDocumento(VtDocs.BusinessServices.Entities.Document.DeleteDocumentoRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.DeleteDocumentoResponse response = new VtDocs.BusinessServices.Entities.Document.DeleteDocumentoResponse();

            try
            {
                if (request.MettiInCestino)
                {
                    DocsPaVO.documento.SchedaDocumento schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglio(request.InfoUtente, request.IdDocumento, request.IdDocumento);

                    string errorMessage;
                    response.Success = BusinessLogic.Documenti.DocManager.CestinaDocumento(request.InfoUtente,
                                                                            schedaDocumento,
                                                                            string.Empty, 
                                                                            request.NoteCestino, 
                                                                            out errorMessage);

                    if (!response.Success)
                        throw new ApplicationException(errorMessage);
                }
                else
                {
                    DocsPaVO.documento.InfoDocumento infoDocumento = BusinessLogic.Documenti.DocManager.GetInfoDocumento(request.InfoUtente, request.IdDocumento, request.IdDocumento);

                    response.Success = BusinessLogic.Documenti.DocManager.EliminaDoc(request.InfoUtente, infoDocumento);

                    if (!response.Success)
                        throw new ApplicationException("Si è verificato un errore nella cancellazione del documento");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per la creazione di un allegato ad un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public virtual VtDocs.BusinessServices.Entities.Document.CreateAllegatoResponse CreateAllegato(VtDocs.BusinessServices.Entities.Document.CreateAllegatoRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.CreateAllegatoResponse response = new VtDocs.BusinessServices.Entities.Document.CreateAllegatoResponse();

            try
            {
                //if (!string.IsNullOrEmpty(request.IdDocumentoPrincipale))
                //{
                    using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
                    {
                        // Richiesta di inserimento di un allegato

                        //DocsPaVO.documento.Allegato allegato = new DocsPaVO.documento.Allegato
                        //{
                        //    docNumber = request.IdDocumentoPrincipale,
                        //    descrizione = request.Oggetto
                        //};

                        // Acquisizione dell'allegato
                        response.Allegato = BusinessLogic.Documenti.AllegatiManager.aggiungiAllegato(request.InfoUtente, request.Allegato);

                        response.Success = (response.Allegato != null);
                    }
                //}
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Allegato = null;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per l'upload di un file da associare ad una versione del documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public virtual VtDocs.BusinessServices.Entities.Document.UploadFileResponse UploadFile(VtDocs.BusinessServices.Entities.Document.UploadFileRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.UploadFileResponse response = new VtDocs.BusinessServices.Entities.Document.UploadFileResponse();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
                {
                    // Determina se creare o meno una nuova versione del documento
                    response.IsNewVersion = string.IsNullOrEmpty(request.FileRequest.versionId);

                    DocsPaVO.documento.FileRequest fileRequest = request.FileRequest;
                    string fileName = fileRequest.fileName;

                    if (response.IsNewVersion)
                    {
                        fileRequest = ws.DocumentoAggiungiVersione(fileRequest, request.InfoUtente);

                        if (fileRequest == null)
                            throw new ApplicationException("Errore nella creazione della nuova versione");
                    }

                    DocsPaVO.documento.FileDocumento inputFileDocument = new DocsPaVO.documento.FileDocumento
                    {
                        cartaceo = request.FileRequest.cartaceo,
                        name = fileName,
                        fullName = fileName,
                        content = request.Content,
                        contentType = request.ContentType,
                        length = request.Content.Length
                    };

                    fileRequest = ws.DocumentoPutFile(fileRequest, inputFileDocument, request.InfoUtente);

                    if (fileRequest == null)
                        throw new ApplicationException("Errore nell'upload del documento");

                    response.Success = true;
                    response.FileRequest = fileRequest;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.FileRequest = null;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        ///// <summary>
        ///// Servizio per l'upload di un file da associare ad una versione del documento
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //[WebMethod()]
        //public virtual VtDocs.BusinessServices.Entities.Document.UploadFileResponse UploadFile(VtDocs.BusinessServices.Entities.Document.UploadFileRequest request)
        //{
        //    VtDocs.BusinessServices.Entities.Document.UploadFileResponse response = new VtDocs.BusinessServices.Entities.Document.UploadFileResponse();

        //    try
        //    {
        //        using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
        //        {
        //            // Determina se creare o meno una nuova versione del documento
        //            response.IsNewVersion = string.IsNullOrEmpty(request.VersionId);

        //            DocsPaVO.documento.FileRequest inputFileRequest = null;

        //            if (response.IsNewVersion)
        //            {
        //                inputFileRequest = new DocsPaVO.documento.FileRequest
        //                {
        //                    descrizione = request.VersionDescription,
        //                    docNumber = request.IdProfile
        //                };

        //                inputFileRequest = ws.DocumentoAggiungiVersione(inputFileRequest, request.InfoUtente);
        //            }
        //            else
        //            {
        //                // Reperimento del documento richiesto
        //                DocsPaVO.documento.SchedaDocumento documento = ws.DocumentoGetDettaglioDocumento(request.InfoUtente, request.IdProfile, request.IdProfile);

        //                inputFileRequest = (DocsPaVO.documento.FileRequest) documento.documenti[0];
        //                inputFileRequest.descrizione = request.VersionDescription;
        //            }

        //            DocsPaVO.documento.FileDocumento inputFileDocument = new DocsPaVO.documento.FileDocumento
        //                {
        //                    cartaceo = request.IsCartaceo,
        //                    name = request.FileName,
        //                    fullName = request.FileName,
        //                    content = request.Content,
        //                    contentType = request.ContentType,
        //                    length = request.Content.Length
        //                };

        //            response.FileRequest = ws.DocumentoPutFile(inputFileRequest, inputFileDocument, request.InfoUtente);

        //            if (response.FileRequest == null)
        //                throw new ApplicationException("TODO");

        //            response.Success = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        response.FileRequest = null;

        //        if (request.TrowOnError)
        //            throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
        //        else
        //            response.Exception = ex.Message;
        //    }

        //    return response;
        //}

        /// <summary>
        /// Servizio per il download di un file associato ad una versione
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.DownloadFileResponse DownloadFile(VtDocs.BusinessServices.Entities.Document.DownloadFileRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.DownloadFileResponse response = new VtDocs.BusinessServices.Entities.Document.DownloadFileResponse();
            
            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
                {
                    if (!string.IsNullOrEmpty(request.RepositoryContextToken))
                    {
                        DocsPaVO.documento.SessionRepositoryContext repositoryContext = new DocsPaVO.documento.SessionRepositoryContext
                        {
                            Token = request.RepositoryContextToken,
                            Owner = request.InfoUtente
                        };

                        DocsPaVO.documento.FileRequest version = null;

                        if (request.VersionLabel.StartsWith("A"))
                            version = new DocsPaVO.documento.Allegato();
                        else
                            version = new DocsPaVO.documento.FileRequest();

                        version.repositoryContext = repositoryContext;
                        version.docNumber = request.DocNumber;
                        version.versionId = request.VersionId;
                        version.version = request.Version;
                        version.versionLabel = request.VersionLabel;

                        response.File = ws.DocumentoGetFileFirmato(version, request.InfoUtente);

                        if (response.File == null)
                            throw new ApplicationException("Errore nel reperimento del contenuto del file");
                    }
                    else
                    {
                        // Reperimento del documento richiesto
                        DocsPaVO.documento.SchedaDocumento documento = ws.DocumentoGetDettaglioDocumento(
                                                        request.InfoUtente,
                                                        request.IdProfile,
                                                        request.DocNumber);

                        if (documento == null)
                            throw new ApplicationException("Errore nel reperimento dei dati del documento");

                        DocsPaVO.documento.FileRequest[] versions = (DocsPaVO.documento.FileRequest[])
                                            documento.documenti.ToArray(typeof(DocsPaVO.documento.FileRequest));

                        DocsPaVO.documento.FileRequest version = null;

                        if (!string.IsNullOrEmpty(request.VersionId))
                        {
                            version = versions.FirstOrDefault(e => e.versionId == request.VersionId);
                            if (version == null)
                                throw new ApplicationException("Errore nel reperimento della versione richiesta");
                        }
                        else
                        {
                            // Reperimento ultima versione se non specificato
                            version = (DocsPaVO.documento.FileRequest)documento.documenti[0];
                        }

                        response.File = ws.DocumentoGetFileFirmato(version, request.InfoUtente);

                        if (response.File == null)
                            throw new ApplicationException("Errore nel reperimento del contenuto del file");
                    }

                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.File = null;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il salvataggio dei metadati di una versione del documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public virtual VtDocs.BusinessServices.Entities.Document.SaveVersioneResponse SaveVersione(VtDocs.BusinessServices.Entities.Document.SaveVersioneRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.SaveVersioneResponse response = new VtDocs.BusinessServices.Entities.Document.SaveVersioneResponse();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
                {
                    if (string.IsNullOrEmpty(request.FileRequest.versionId))
                    {
                        // Modalità di inserimento di una nuova versione al documento
                        DocsPaVO.documento.FileRequest newVersion = ws.DocumentoAggiungiVersione(request.FileRequest, request.InfoUtente);

                        if (newVersion == null)
                            throw new ApplicationException("Errore nella creazione della versione del documento");
                        else
                            response.FileRequest = newVersion;
                    }
                    else
                    {
                        if (request.FileRequest.GetType().Equals(typeof(DocsPaVO.documento.Allegato)))
                        {
                            if (ws.DocumentoModificaAllegato(request.InfoUtente, (DocsPaVO.documento.Allegato)request.FileRequest,null))
                                response.FileRequest = request.FileRequest;
                            else
                                throw new ApplicationException("Errore nella modifica della versione dell'allegato");
                        }
                        else
                        {
                            if (ws.DocumentoModificaVersione(request.InfoUtente, request.FileRequest))
                                response.FileRequest = request.FileRequest;
                            else
                                throw new ApplicationException("Errore nella modifica della versione del documento");
                        }
                    }

                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.FileRequest = null;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei fascicoli che contengono un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.GetFascicoliResponse GetFascicoli(VtDocs.BusinessServices.Entities.Document.GetFascicoliRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.GetFascicoliResponse response = new VtDocs.BusinessServices.Entities.Document.GetFascicoliResponse();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
                {
                    System.Collections.ArrayList list = ws.FascicolazioneGetFascicoliDaDocNoSecurity(request.InfoUtente, request.IdProfile);

                    if (list != null)
                    {
                        response.Fascicoli = (DocsPaVO.fascicolazione.Fascicolo[])list.ToArray(typeof(DocsPaVO.fascicolazione.Fascicolo));

                        response.Success = true;
                    }
                    else
                    {
                        throw new ApplicationException("Errore nel reperimento delle fascicolazioni del documento");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Fascicoli = null;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per l'inserimento del documento in un fascicolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.AddFascicoloResponse AddFascicolo(VtDocs.BusinessServices.Entities.Document.AddFascicoloRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.AddFascicoloResponse response = new VtDocs.BusinessServices.Entities.Document.AddFascicoloResponse();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
                {
                    // Reperimento id del folder
                    string idFolder = this.getIdFolderCType(request.IdFascicolo);

                    // Reperimento del folder in cui inserire il fascicolo
                    DocsPaVO.fascicolazione.Folder folder = ws.FascicolazioneGetFolderById(request.InfoUtente.idPeople, request.InfoUtente.idGruppo, idFolder);

                    string msg;
                    if (!ws.FascicolazioneAddDocFolder(request.InfoUtente,
                                                            request.IdProfile,
                                                            folder,
                                                            string.Empty,
                                                            out msg))
                    {
                        throw new ApplicationException(msg);
                    }

                    if (request.GetFascicoli)
                    {
                        // Restituizione della lista aggiornata dei fascicoli
                        System.Collections.ArrayList list = ws.FascicolazioneGetFascicoliDaDocNoSecurity(request.InfoUtente, request.IdProfile);

                        if (list != null)
                            response.Fascicoli = (DocsPaVO.fascicolazione.Fascicolo[])list.ToArray(typeof(DocsPaVO.fascicolazione.Fascicolo));
                        else
                            throw new ApplicationException("Errore nel reperimento delle fascicolazioni del documento");
                    }

                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per la rimozione del documento da un fascicolo
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.RemoveFascicoloResponse RemoveFascicolo(VtDocs.BusinessServices.Entities.Document.RemoveFascicoloRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.RemoveFascicoloResponse response = new VtDocs.BusinessServices.Entities.Document.RemoveFascicoloResponse();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
                {
                    // Reperimento id del folder
                    string idFolder = this.getIdFolderCType(request.IdFascicolo);

                    // Reperimento del folder da cui rimuovere il fascicolo
                    DocsPaVO.fascicolazione.Folder folder = ws.FascicolazioneGetFolderById(request.InfoUtente.idPeople, request.InfoUtente.idGruppo, idFolder);

                    // TODO: GESTIRE IL FLAG FASCICOLAZIONE RAPIDA
                    string msg;
                    DocsPaVO.Validations.ValidationResultInfo result = ws.FascicolazioneDeleteDocFromFolder(request.InfoUtente, request.IdProfile, folder, string.Empty, out msg);

                    if (result != null &&
                        result.BrokenRules != null &&
                        result.BrokenRules.Count > 0)
                    {
                        foreach (DocsPaVO.Validations.BrokenRule br in result.BrokenRules)
                        {
                            msg += Environment.NewLine + br.Description;
                        }

                        throw new ApplicationException(msg);
                    }
                    else
                    {
                        if (request.GetFascicoli)
                        {
                            // Restituizione della lista aggiornata dei fascicoli
                            System.Collections.ArrayList list = ws.FascicolazioneGetFascicoliDaDocNoSecurity(request.InfoUtente, request.IdProfile);

                            if (list != null)
                                response.Fascicoli = (DocsPaVO.fascicolazione.Fascicolo[])list.ToArray(typeof(DocsPaVO.fascicolazione.Fascicolo));
                            else
                                throw new ApplicationException("Errore nel reperimento delle fascicolazioni del documento");
                        }

                        response.Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento di un template documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.GetTemplateResponse GetTemplate(VtDocs.BusinessServices.Entities.Document.GetTemplateRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.GetTemplateResponse response = new VtDocs.BusinessServices.Entities.Document.GetTemplateResponse();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
                {
                    response.Template = ws.getTemplateById(request.IdTemplate);

                    if (response.Template != null && response.Template.SYSTEM_ID == 0)
                        throw new ApplicationException(string.Format("Template con Id '{0}' non trovato", request.IdTemplate));
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento delle trasmissioni ricevute 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.GetMyToDoListResponse GetMyToDoList(VtDocs.BusinessServices.Entities.Document.GetMyToDoListRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.GetMyToDoListResponse response = new VtDocs.BusinessServices.Entities.Document.GetMyToDoListResponse();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWS.DocsPaWebService())
                {
                    DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(request.InfoUtente.idGruppo);

                    string registriSeparatedValue = "0";
                    foreach (DocsPaVO.utente.Registro r in ruolo.registri)
                        registriSeparatedValue += "," + r.systemId;

                    List<DocsPaVO.filtri.FiltroRicerca> filtri = new List<DocsPaVO.filtri.FiltroRicerca>();

                    filtri.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = "TO_DO_LIST" });
                    filtri.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = "NO_CERCA_INFERIORI" });

                    if (!string.IsNullOrEmpty(request.Ragione))
                    {
                        filtri.Add(
                            new DocsPaVO.filtri.FiltroRicerca
                            {
                                argomento = DocsPaVO.filtri.trasmissione.listaArgomentiNascosti.RAGIONE.ToString(),
                                valore = request.Ragione
                            });
                    }

                    int totalRecordCount;
                    int totalTxNonViste;
                    System.Collections.ArrayList list = ws.getMyNewTodoList(
                                    request.InfoUtente,
                                    registriSeparatedValue,
                                    filtri.ToArray(),
                                    request.PagingContext.CurrentPageNumber,
                                    request.PagingContext.PageSize,
                                    out totalRecordCount,
                                    out totalTxNonViste);

                    if (list == null)
                        throw new ApplicationException("Errore nel reperimento dei dati della ToDoList");


                    response.PagingContext = request.PagingContext;
                    response.PagingContext.ItemsCount = totalRecordCount;

                    response.MyToDoList = (DocsPaVO.trasmissione.infoToDoList[])list.ToArray(typeof(DocsPaVO.trasmissione.infoToDoList));
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento dei contatori statistici dei documenti
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.GetContatoriDocumentiResponse GetContatoriDocumenti(VtDocs.BusinessServices.Entities.Document.GetContatoriDocumentiRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.GetContatoriDocumentiResponse response = new VtDocs.BusinessServices.Entities.Document.GetContatoriDocumentiResponse();

            try
            {
                if (string.IsNullOrEmpty(request.IdUO))
                {
                    throw new ApplicationException("Nessuna UO fornita");
                }

                if (request.DescrizioneTipiDocumento == null ||
                    request.DescrizioneTipiDocumento != null && request.DescrizioneTipiDocumento.Count == 0)
                {
                    throw new ApplicationException("Nessuna tipologia documento fornita");
                }

                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_CONTATORI_DOCUMENTI_PER_TIPOLOGIA");

                    queryDef.setParam("idUO", request.IdUO);

                    string filtroTipiDocumento = string.Empty;

                    foreach (string tipo in request.DescrizioneTipiDocumento)
                    {
                        if (!string.IsNullOrEmpty(filtroTipiDocumento))
                            filtroTipiDocumento += ", ";

                        filtroTipiDocumento += "'" + tipo.Replace("'", "''") + "'";
                    }

                    queryDef.setParam("descrizioneTipiDocumento", filtroTipiDocumento);
                    queryDef.setParam("dataInizio", DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("01/01/{0}", request.AnnoCreazione), true));
                    queryDef.setParam("dataFine", DocsPaDbManagement.Functions.Functions.ToDateBetween(string.Format("31/12/{0}", request.AnnoCreazione), false));

                    string commandText = queryDef.getSQL();

                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            if (response.Contatori == null)
                                response.Contatori = new List<VtDocs.BusinessServices.Entities.Document.GetContatoriDocumentiResponse.ContatoreDocumento>();

                            response.Contatori.Add(
                                new VtDocs.BusinessServices.Entities.Document.GetContatoriDocumentiResponse.ContatoreDocumento
                                {
                                    Descrizione = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(reader, "DESCRIZIONE", false),
                                    Totale = DocsPaUtils.Data.DataReaderHelper.GetValue<int>(reader, "TOTALE", false)
                                });
                        }
                    }
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response = new VtDocs.BusinessServices.Entities.Document.GetContatoriDocumentiResponse();
                response.Success = false;
                response.Contatori = null;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoResponse TrasmettiDocumentoInternal(VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoResponse response = new VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoResponse();

            try
            {
                using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
                {
                    VtDocs.BusinessServices.Trasmissioni.TrasmissioneManuale trasmissioneManuale = new VtDocs.BusinessServices.Trasmissioni.TrasmissioneManuale(request.InfoUtente, VtDocs.BusinessServices.Trasmissioni.TrasmissioneManuale.TipiTrasmissioneEnum.Utente);

                    foreach (var txUtente in request.TrasmissioniUtente)
                    {
                        if (txUtente.SalvaSenzaInviare)
                        {
                            trasmissioneManuale.Save(request.IdDocumento,
                                                    DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO,
                                                    txUtente.Ragione,
                                                    txUtente.NoteGenerali,
                                                    txUtente.NotificaInToDoList,
                                                    txUtente.IdUtentiDestinatari);
                        }
                        else
                        {
                            trasmissioneManuale.Execute(request.IdDocumento,
                                                    DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO,
                                                    txUtente.Ragione,
                                                    txUtente.NoteGenerali,
                                                    txUtente.NotificaInToDoList,
                                                    txUtente.IdUtentiDestinatari);
                        }
                    }

                    trasmissioneManuale = new VtDocs.BusinessServices.Trasmissioni.TrasmissioneManuale(request.InfoUtente, VtDocs.BusinessServices.Trasmissioni.TrasmissioneManuale.TipiTrasmissioneEnum.Ruolo);

                    foreach (var txRuolo in request.TrasmissioniRuolo)
                    {
                        if (txRuolo.SalvaSenzaInviare)
                        {
                            trasmissioneManuale.Save(request.IdDocumento,
                                                    DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO,
                                                    txRuolo.Ragione,
                                                    txRuolo.NoteGenerali,
                                                    txRuolo.NotificaInToDoList,
                                                    txRuolo.IdRuoliDestinatari);
                        }
                        else
                        {
                            trasmissioneManuale.Execute(request.IdDocumento,
                                                    DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO,
                                                    txRuolo.Ragione,
                                                    txRuolo.NoteGenerali,
                                                    txRuolo.NotificaInToDoList,
                                                    txRuolo.IdRuoliDestinatari);
                        }
                    }

                    response.Success = true;

                    transactionContext.Complete();
                }
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        protected void TrasmettiDocumentoInternalCompleted(IAsyncResult ar)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected delegate VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoResponse TrasmettiDocumentoDelegate(VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoRequest request);

        /// <summary>
        /// Servizio per la trasmissione di un documento 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoResponse TrasmettiDocumento(VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoRequest request)
        {
            if (request.Asincrono)
            {
                // Trasmissione effettuata in modalità asincrona
                TrasmettiDocumentoDelegate del = new TrasmettiDocumentoDelegate(this.TrasmettiDocumentoInternal);
                IAsyncResult result = del.BeginInvoke(request, new AsyncCallback(this.TrasmettiDocumentoInternalCompleted), this);

                return new VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoResponse
                {
                    Success = true
                };
            }
            else
            {
                return this.TrasmettiDocumentoInternal(request);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        private delegate VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoOrganigrammaResponse TrasmettiDocumentoOrganigrammaDelegate(VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoOrganigrammaRequest request);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        public VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoOrganigrammaResponse TrasmettiDocumentoOrganigrammaAsync(VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoOrganigrammaRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoOrganigrammaResponse response = new VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoOrganigrammaResponse();

            try
            {
                List<string> destinatari = new List<string>();
                List<string> recipientsList = new List<string>();


                    
                if (request.IdUtentiDestinatari != null)
                {
                    destinatari.AddRange(request.IdUtentiDestinatari);
                }

                

                if (request.IdRuoliDestinatari != null)
                {
                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    {
                        // Reperimento degli utenti facenti parte del ruolo
                        foreach (var idRuolo in request.IdRuoliDestinatari)
                        {
                            if (!string.IsNullOrEmpty(idRuolo))
                            {
                                string commandText = string.Format("select pg.PEOPLE_SYSTEM_ID as IdPeople from PEOPLEGROUPS pg inner join DPA_CORR_GLOBALI cg on pg.GROUPS_SYSTEM_ID = cg.ID_GRUPPO where cg.SYSTEM_ID = {0} and pg.DTA_FINE is null", idRuolo);

                                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                                {
                                    while (reader.Read())
                                    {
                                        destinatari.Add(reader.GetValue(reader.GetOrdinal("IdPeople")).ToString());
                                    }
                                }
                            }
                        }
                    }
                }

                if (request.IdUODestinatarie != null)
                {
                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                    {
                        // Reperimento degli utenti facenti parte dell'UO
                        foreach (var idUO in request.IdUODestinatarie)
                        {
                            if (!string.IsNullOrEmpty(idUO))
                            {
                                DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UTENTI_IN_ORGANIGRAMMA");

                                queryDef.setParam("dbUser", DocsPaDbManagement.Functions.Functions.GetDbUserSession());
                                queryDef.setParam("idUo", idUO);
                                queryDef.setParam("codiceQualifica", request.CodiceQualificaDiscriminanteUO);
                                queryDef.setParam("customFilters", string.Empty);

                                string commandText = queryDef.getSQL();

                                using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                                {
                                    while (reader.Read())
                                    {
                                        destinatari.Add(reader.GetValue(reader.GetOrdinal("ID_UTENTE")).ToString());

                                        if (!reader.IsDBNull(reader.GetOrdinal("EMAIL_ADDRESS")))
                                        {
                                            String mailAddress = reader.GetString(reader.GetOrdinal("EMAIL_ADDRESS"));

                                            if (!String.IsNullOrEmpty(mailAddress))
                                            {
                                                recipientsList.Add(mailAddress);
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
              
                VtDocs.BusinessServices.Trasmissioni.TrasmissioneManuale trasmissioneManuale = new VtDocs.BusinessServices.Trasmissioni.TrasmissioneManuale(request.InfoUtente, VtDocs.BusinessServices.Trasmissioni.TrasmissioneManuale.TipiTrasmissioneEnum.Utente);

                trasmissioneManuale.Execute(
                                        request.IdDocumento,
                                        DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO,
                                        string.Empty,
                                        string.Empty,
                                        request.NotificaInToDoList,
                                        destinatari.Distinct().ToArray());
 /*               
                if (
                    (!String.IsNullOrEmpty(request.MessageEngineObj.ServicesUrl))
                    && (!String.IsNullOrEmpty(request.MessageEngineObj.MessageEngine))
                    && (request.MessageEngineObj.MessageEngine.Equals("1"))
                    )
                {

                    using (MessageEngineSimple.ProxySimple.MessageEngineSimpleServices ws = new MessageEngineSimple.ProxySimple.MessageEngineSimpleServices())
                    {
                        ws.Url = request.MessageEngineObj.ServicesUrl;

                        ws.SendMessage(request.MessageEngineObj.Interface,
                                    request.MessageEngineObj.Action,
                                    request.MessageEngineObj.RecipientType,
                                    request.MessageEngineObj.MessageType,
                                    request.MessageEngineObj.From,
                                    recipientsList.ToArray<string>(),
                                    request.MessageEngineObj.Subject,
                                    request.MessageEngineObj.Body);
                    }

                }
*/          
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        public void TrasmettiDocumentoOrganigrammaAsyncCompleted(IAsyncResult ar)
        {
            // TODO: NOTIFICA AL MITTENTE DI AVVENUTA TRASMISSIONE
        }

        /// <summary>
        /// Servizio per la trasmissione di un documento ad un'organigramma
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoOrganigrammaResponse TrasmettiDocumentoOrganigramma(VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoOrganigrammaRequest request)
        {
            // Avvio task di verifica asincrono
            TrasmettiDocumentoOrganigrammaDelegate del = new TrasmettiDocumentoOrganigrammaDelegate(this.TrasmettiDocumentoOrganigrammaAsync);
            IAsyncResult result = del.BeginInvoke(request, new AsyncCallback(this.TrasmettiDocumentoOrganigrammaAsyncCompleted), this);

            return new VtDocs.BusinessServices.Entities.Document.TrasmettiDocumentoOrganigrammaResponse
            {
                Success = true
            };
        }

        /// <summary>
        /// Servizio per il reperimento degli elementi di visibilità del documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.GetVisibilitaResponse GetVisibilita(VtDocs.BusinessServices.Entities.Document.GetVisibilitaRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.GetVisibilitaResponse response = new VtDocs.BusinessServices.Entities.Document.GetVisibilitaResponse();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWebService())
                {
                    response.Diritti = (DocsPaVO.documento.DirittoOggetto[])
                        ws.DocumentoGetVisibilita(request.InfoUtente, request.IdDocumento, request.VisualizzaRevocati).ToArray(typeof(DocsPaVO.documento.DirittoOggetto));

                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento degli stati di un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.GetDiagrammaResponse GetDiagramma(VtDocs.BusinessServices.Entities.Document.GetDiagrammaRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.GetDiagrammaResponse response = new VtDocs.BusinessServices.Entities.Document.GetDiagrammaResponse();

            try
            {
                if (!string.IsNullOrEmpty(request.IdDiagramma))
                {
                    // Reperimento del diagramma di stato a partire dall'identificativo univoco
                    response.Diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(request.IdDiagramma);

                    if (response.Diagramma == null)
                        throw new ApplicationException("Errore nel reperimento dei dati del diagramma di stato");
                }
                else if (!string.IsNullOrEmpty(request.DescrizioneDiagramma))
                {
                    var diagrammiArrayList = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammi(request.InfoUtente.idAmministrazione);

                    if (diagrammiArrayList != null)
                    {
                        DocsPaVO.DiagrammaStato.DiagrammaStato[] diagramami = (DocsPaVO.DiagrammaStato.DiagrammaStato[]) 
                            diagrammiArrayList.ToArray(typeof(DocsPaVO.DiagrammaStato.DiagrammaStato));

                        response.Diagramma = diagramami.FirstOrDefault(e => e.DESCRIZIONE.Equals(request.DescrizioneDiagramma, StringComparison.InvariantCultureIgnoreCase));

                        if (response.Diagramma == null)
                            throw new ApplicationException("Errore nel reperimento dei dati del diagramma di stato");
                    }
                    else
                    {
                        throw new ApplicationException("Errore nel reperimento dei dati del diagramma di stato");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.IdTemplate))
                    {
                        // Reperimento diagramma di stato
                        int idDiagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaAssociato(request.IdTemplate);

                        response.Diagramma = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaById(idDiagramma.ToString());

                        if (response.Diagramma == null)
                            throw new ApplicationException("Errore nel reperimento dei dati del diagramma di stato");

                        response.Success = true;
                    }

                    if (!string.IsNullOrEmpty(request.IdDocumento))
                    {
                        response.StatoDocumento = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(request.IdDocumento);

                        if (response.StatoDocumento == null)
                            throw new ApplicationException("Errore nel reperimento dei dati dello stato corrente del documento");
                    }
                }
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //private static DocsPaVO.ProfilazioneDinamica.Templates _templatesCampiComuni = null;

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="infoUtente"></param>
        ///// <returns></returns>
        //protected DocsPaVO.ProfilazioneDinamica.Templates GetTemplatesCampiComuni(DocsPaVO.utente.InfoUtente infoUtente)
        //{
        //    if (_templatesCampiComuni == null)
        //    {
        //        string idTipoDocumento;

        //        using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
        //            dbProvider.ExecuteScalar(out idTipoDocumento, "(SELECT SYSTEM_ID FROM DPA_TIPO_ATTO WHERE VAR_DESC_ATTO = 'CAMPI COMUNI')");

        //        // Reperimento template documento
        //        var getTemplateResponse = this.GetTemplate(
        //                                    new VtDocs.BusinessServices.Entities.Document.GetTemplateRequest
        //                                    {
        //                                        InfoUtente = infoUtente,
        //                                        IdTemplate = idTipoDocumento,
        //                                        TrowOnError = false
        //                                    }
        //                                );

        //        if (getTemplateResponse.Success)
        //        {
        //            _templatesCampiComuni = getTemplateResponse.Template;
        //        }
        //    }

        //    return _templatesCampiComuni;
        //}

        /// <summary>
        /// Servizio per la ricerca dei documenti a partire dai filtri forniti in ingresso
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.RicercaResponse Ricerca(VtDocs.BusinessServices.Entities.Document.RicercaRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.RicercaResponse response = new VtDocs.BusinessServices.Entities.Document.RicercaResponse();

            try
            {
                List<DocsPaVO.filtri.FiltroRicerca> filtriRicercaLegacy = null;

                if (request.FiltriRicercaLegacy != null)
                {
                    filtriRicercaLegacy = request.FiltriRicercaLegacy.ToList();
                }
                else
                {
                    filtriRicercaLegacy = new List<DocsPaVO.filtri.FiltroRicerca>();
                }

                if (request.FiltriRicerca != null)
                {
                    List<DocsPaVO.filtri.FiltroRicerca> listFiltriRicerca = new List<DocsPaVO.filtri.FiltroRicerca>();

                    listFiltriRicerca.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO.ToString(), valore = "tipo" });

                    if (request.FiltriRicerca.GetProtocolliArrivo)
                        listFiltriRicerca.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.PROT_ARRIVO.ToString(), valore = bool.TrueString.ToLowerInvariant() });

                    if (request.FiltriRicerca.GetProtocolliPartenza)
                        listFiltriRicerca.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.PROT_PARTENZA.ToString(), valore = bool.TrueString.ToLowerInvariant() });

                    if (request.FiltriRicerca.GetProtocolliInterni)
                        listFiltriRicerca.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.PROT_INTERNO.ToString(), valore = bool.TrueString.ToLowerInvariant() });

                    if (request.FiltriRicerca.GetNonProtocollati)
                        listFiltriRicerca.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.GRIGIO.ToString(), valore = bool.TrueString.ToLowerInvariant() });

                    if (!string.IsNullOrEmpty(request.FiltriRicerca.Oggetto))
                        listFiltriRicerca.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.OGGETTO.ToString(), valore = request.FiltriRicerca.Oggetto });

                    if (!string.IsNullOrEmpty(request.FiltriRicerca.IdTipoDocumento))
                        listFiltriRicerca.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.TIPO_ATTO.ToString(), valore = request.FiltriRicerca.IdTipoDocumento });

                    if (!string.IsNullOrEmpty(request.FiltriRicerca.IdStatoDocumento))
                    {
                        listFiltriRicerca.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.CONDIZIONE_STATO_DOCUMENTO.ToString(), valore = "Equals" });

                        listFiltriRicerca.Add(new DocsPaVO.filtri.FiltroRicerca
                        {
                            argomento = DocsPaVO.filtri.ricerca.listaArgomenti.DIAGRAMMA_STATO_DOC.ToString(),
                            valore = string.Format(" AND (DPA_DIAGRAMMI.DOC_NUMBER = A.DOCNUMBER AND DPA_DIAGRAMMI.ID_STATO = {0}) ", request.FiltriRicerca.IdStatoDocumento)
                        });
                    }

                    if (request.FiltriRicerca.AnnoProtocollazione > 0)
                        listFiltriRicerca.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.ANNO_PROTOCOLLO.ToString(), valore = request.FiltriRicerca.AnnoProtocollazione.ToString() });

                    if (!string.IsNullOrEmpty(request.FiltriRicerca.CodiceApplicazione))
                        // Filtro per codice applicazione
                        listFiltriRicerca.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.COD_EXT_APP.ToString(), valore = request.FiltriRicerca.CodiceApplicazione });

                    if (request.Ordinamento == null)
                    {
                        listFiltriRicerca.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.ORACLE_FIELD_FOR_ORDER.ToString(), valore = "NVL (a.dta_proto, a.creation_time)" });
                        listFiltriRicerca.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.SQL_FIELD_FOR_ORDER.ToString(), valore = "ISNULL (a.dta_proto, a.creation_time)" });

                        // Condizione di ordinamento predefinita
                        listFiltriRicerca.Add(new DocsPaVO.filtri.FiltroRicerca { argomento = DocsPaVO.filtri.ricerca.listaArgomenti.ORDER_DIRECTION.ToString(), valore = "DESC" });
                    }

                    filtriRicercaLegacy.AddRange(listFiltriRicerca);
                }

                int pageNumber, pageSize;
                int numTotPage, nRec;

                if (request.PagingContext != null)
                {
                    pageNumber = request.PagingContext.CurrentPageNumber;
                    pageSize = request.PagingContext.PageSize;
                }
                else
                {
                    pageNumber = 1;
                    pageSize = Int32.MaxValue;
                }

                string idTipoDocumento = request.FiltriRicerca.IdTipoDocumento;

                if (string.IsNullOrEmpty(idTipoDocumento) && 
                    request.FiltriRicerca.CampiComuni != null &&
                    request.FiltriRicerca.CampiComuni.Count > 0)
                {
                    using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                        dbProvider.ExecuteScalar(out idTipoDocumento, "(SELECT SYSTEM_ID FROM DPA_TIPO_ATTO WHERE VAR_DESC_ATTO = 'CAMPI COMUNI')");
                }

                VtDocs.BusinessServices.Entities.Document.GetTemplateResponse getTemplateResponse = null;

                List<DocsPaVO.Grid.Field> campiRichiesti = new List<DocsPaVO.Grid.Field>();

                if (!string.IsNullOrEmpty(idTipoDocumento))
                {
                    // Reperimento template documento
                    getTemplateResponse = this.GetTemplate(
                                                new VtDocs.BusinessServices.Entities.Document.GetTemplateRequest
                                                {
                                                    InfoUtente = request.InfoUtente,
                                                    IdTemplate = idTipoDocumento,
                                                    TrowOnError = false
                                                }
                                            );

                    if (getTemplateResponse.Success)
                    {
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                        getTemplateResponse.Template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));


                        // Filtri per campi comuni della tipologia documentale
                        if (request.FiltriRicerca.CampiComuni != null)
                        {
                            foreach (var campoComune in request.FiltriRicerca.CampiComuni)
                            {
                                DocsPaVO.ProfilazioneDinamica.OggettoCustom obj = oggettiCustom.FirstOrDefault(e => e.DESCRIZIONE == campoComune.Nome);

                                if (obj != null)
                                    obj.VALORE_DATABASE = campoComune.Valore;
                            }

                            filtriRicercaLegacy.Add(new DocsPaVO.filtri.FiltroRicerca
                            {
                                argomento = DocsPaVO.filtri.fascicolazione.listaArgomenti.PROFILAZIONE_DINAMICA.ToString(),
                                valore = "Profilazione Dinamica",
                                template = getTemplateResponse.Template
                            });
                        }


                        // Estrazione dei campi della tipologia documentale
                        if (request.CampiRichiestiInOutput != null && request.CampiRichiestiInOutput.Length > 0)
                        {
                            foreach (VtDocs.BusinessServices.Entities.Document.RicercaRequest.CampoRicerca campo in request.CampiRichiestiInOutput)
                            {
                                DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = oggettiCustom.FirstOrDefault(e => e.DESCRIZIONE.ToLowerInvariant() == campo.NomeCampo.ToLowerInvariant());

                                if (oggettoCustom != null)
                                {
                                    campiRichiesti.Add(new DocsPaVO.Grid.Field
                                    {
                                        FieldId = oggettoCustom.DESCRIZIONE,
                                        AssociatedTemplateName = getTemplateResponse.Template.DESCRIZIONE,
                                        AssociatedTemplateId = getTemplateResponse.Template.SYSTEM_ID.ToString(),
                                        CustomObjectId = oggettoCustom.SYSTEM_ID
                                    });
                                }
                            }
                        }
                    }
                    else
                        throw new ApplicationException("Errore nel reperimento della tipologia documentale");
                }

                if (request.Ordinamento != null)
                {
                    if (getTemplateResponse != null)
                    {
                        DocsPaVO.ProfilazioneDinamica.OggettoCustom[] oggettiCustom = (DocsPaVO.ProfilazioneDinamica.OggettoCustom[])
                                getTemplateResponse.Template.ELENCO_OGGETTI.ToArray(typeof(DocsPaVO.ProfilazioneDinamica.OggettoCustom));

                        DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom = oggettiCustom.FirstOrDefault(e => e.DESCRIZIONE.ToLowerInvariant() == request.Ordinamento.Campo.NomeCampo.ToLowerInvariant());

                        if (oggettoCustom != null)
                        {
                            string nomeCampo = string.Format("A{0}", oggettoCustom.SYSTEM_ID);

                            DocsPaVO.filtri.FiltroRicerca orderDirection = new DocsPaVO.filtri.FiltroRicerca();
                            orderDirection.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.PROFILATION_FIELD_FOR_ORDER.ToString();
                            orderDirection.nomeCampo = nomeCampo;
                            orderDirection.valore = oggettoCustom.SYSTEM_ID.ToString();
                            filtriRicercaLegacy.Add(orderDirection);

                            // Ordinamento dati (se richiesto)
                            orderDirection = new DocsPaVO.filtri.FiltroRicerca();
                            orderDirection.argomento = DocsPaVO.filtri.ricerca.listaArgomenti.ORDER_DIRECTION.ToString();
                            orderDirection.nomeCampo = nomeCampo;
                            orderDirection.valore = request.Ordinamento.Direzione.ToString();
                            filtriRicercaLegacy.Add(orderDirection);
                        }
                        else
                        {
                            using (DocsPaDB.Query_DocsPAWS.Grids gridManager = new DocsPaDB.Query_DocsPAWS.Grids())
                            {
                                DocsPaVO.Grid.Grid standardGrid = gridManager.GetStandardGridForUser(request.InfoUtente, DocsPaVO.Grid.Grid.GridTypeEnumeration.Document);

                                DocsPaVO.Grid.Field field = standardGrid.Fields.FirstOrDefault(fld => fld.FieldId == request.Ordinamento.Campo.NomeCampo);
                                
                                if (field != null)
                                {
                                    filtriRicercaLegacy.Add
                                        (
                                            new DocsPaVO.filtri.FiltroRicerca
                                            {
                                                argomento = DocsPaVO.filtri.ricerca.listaArgomenti.ORACLE_FIELD_FOR_ORDER.ToString(),
                                                valore = field.OracleDbColumnName,
                                                nomeCampo = field.FieldId
                                            }
                                        );

                                    filtriRicercaLegacy.Add
                                             (
                                                 new DocsPaVO.filtri.FiltroRicerca
                                                 {
                                                     argomento = DocsPaVO.filtri.ricerca.listaArgomenti.SQL_FIELD_FOR_ORDER.ToString(),
                                                     valore = field.SqlServerDbColumnName,
                                                     nomeCampo = field.FieldId
                                                 }
                                             );
                                }
                            }

                            filtriRicercaLegacy.Add(
                                new DocsPaVO.filtri.FiltroRicerca 
                                { 
                                    argomento = DocsPaVO.filtri.ricerca.listaArgomenti.ORDER_DIRECTION.ToString(), 
                                    nomeCampo = request.Ordinamento.Campo.NomeCampo,
                                    valore = request.Ordinamento.Direzione.ToString() 
                                });
                        }
                    }
                }

                DocsPaVO.filtri.FiltroRicerca[][] filtri = new DocsPaVO.filtri.FiltroRicerca[1][];
                filtri[0] = filtriRicercaLegacy.ToArray();


                List<DocsPaVO.ricerche.SearchResultInfo> idProfileList;
                System.Collections.ArrayList genericList = BusinessLogic.Documenti.InfoDocManager.getQueryPagingCustom(
                                                                    request.InfoUtente,
                                                                    filtri,
                                                                    pageNumber,
                                                                    pageSize,
                                                                    request.SecurityCheck,
                                                                    false,
                                                                    false,
                                                                    campiRichiesti.ToArray(),
                                                                    null,
                                                                    out numTotPage,
                                                                    out nRec,
                                                                    false,
                                                                    out idProfileList);

                if (genericList == null)
                    throw new ApplicationException("Errore nella ricerca dei documenti");

                response.List = (DocsPaVO.Grids.SearchObject[])genericList.ToArray(typeof(DocsPaVO.Grids.SearchObject));

                if (request.GetSegnaturaRepertorio)
                {
                    string codiceAmministrazione = "INPS";

                    List<DocsPaVO.Grids.SearchObject> responseList = new List<DocsPaVO.Grids.SearchObject>(response.List);

                    foreach (var item in response.List)
                    {
                        string dataAnnullamento;
                        string segnatura = BusinessLogic.Documenti.DocManager.GetSegnaturaRepertorio(item.SearchObjectID,
                                                                                codiceAmministrazione,
                                                                                false,
                                                                                out dataAnnullamento);

                        item.SearchObjectField.Add
                            (
                                new DocsPaVO.Grids.SearchObjectField
                                {
                                    SearchObjectFieldID = "SEGNATURA_CONTATORE",
                                    SearchObjectFieldValue = segnatura
                                }
                            );
                    }
                }

                response.PagingContext = new PagingContext
                {
                    CurrentPageNumber = request.PagingContext.CurrentPageNumber,
                    PageSize = request.PagingContext.PageSize,
                    ItemsCount = nRec
                };

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.GetTemplatesResponse GetTemplates(VtDocs.BusinessServices.Entities.Document.GetTemplatesRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.GetTemplatesResponse response = new VtDocs.BusinessServices.Entities.Document.GetTemplatesResponse();

            try
            {
                using (DocsPaWS.DocsPaWebService ws = new DocsPaWebService())
                {
                    System.Collections.ArrayList listTipologie = ws.getTipoAttoPDInsRic(request.InfoUtente.idAmministrazione, request.InfoUtente.idGruppo, "2");

                    if (listTipologie != null)
                        response.Templates = (DocsPaVO.documento.TipologiaAtto[])listTipologie.ToArray(typeof(DocsPaVO.documento.TipologiaAtto));
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento delle trasmissioni effettuate per un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.GetTrasmissioniEffettuateResponse GetTrasmissioniEffettuate(VtDocs.BusinessServices.Entities.Document.GetTrasmissioniEffettuateRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.GetTrasmissioniEffettuateResponse response = new VtDocs.BusinessServices.Entities.Document.GetTrasmissioniEffettuateResponse();

            try
            {
                using (DocsPaWebService ws = new DocsPaWebService())
                {
                    int requestedPage = 1;
                    int pageSize = Int32.MaxValue;

                    if (request.PagingContext != null)
                    {
                        requestedPage = request.PagingContext.CurrentPageNumber;
                        pageSize = request.PagingContext.PageSize; 
                    }

                    DocsPaVO.ricerche.SearchPagingContext pc = new DocsPaVO.ricerche.SearchPagingContext
                    {
                         Page = requestedPage,
                         PageSize = pageSize
                    };

                    response.Trasmissioni = ws.GetInfoTrasmissioniEffettuate(request.InfoUtente, request.IdDocumento, "D", ref pc);

                    response.PagingContext = request.PagingContext;
                    response.PagingContext.ItemsCount = pc.RecordCount;
                }
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Servizio per il reperimento degli utenti destinatari di tutte le trasmissioni di un documento
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.GetUtentiDestinatariTrasmissioniResponse GetUtentiDestinatariTrasmissioni(VtDocs.BusinessServices.Entities.Document.GetUtentiDestinatariTrasmissioniRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.GetUtentiDestinatariTrasmissioniResponse response = new VtDocs.BusinessServices.Entities.Document.GetUtentiDestinatariTrasmissioniResponse();

            try
            {
                using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
                {
                    DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_UTENTI_DESTINATARI_TRASMISSIONI_DOCUMENTO");

                    queryDef.setParam("idDocumento", request.IdDocumento);

                    string commandText = queryDef.getSQL();

                    using (System.Data.IDataReader reader = dbProvider.ExecuteReader(commandText))
                    {
                        while (reader.Read())
                        {
                            response.Utenti.Add(
                                new VtDocs.BusinessServices.Entities.Document.GetUtentiDestinatariTrasmissioniResponse.DestinatarioTrasmissione
                                {
                                    Id = reader.GetValue(reader.GetOrdinal("ID")).ToString(),
                                    IdUtente = reader.GetString(reader.GetOrdinal("ID_UTENTE")),
                                    Utente = reader.GetString(reader.GetOrdinal("UTENTE")),
                                    DataInvio = reader.GetValue(reader.GetOrdinal("DATA_INVIO")).ToString(),
                                    DataVista = reader.GetValue(reader.GetOrdinal("DATA_VISTA")).ToString(),
                                    DataAccettazione = reader.GetValue(reader.GetOrdinal("DATA_ACCETTAZIONE")).ToString(),
                                    DataRifiuto = reader.GetValue(reader.GetOrdinal("DATA_RIFIUTO")).ToString(),
                                    Ragione = reader.GetString(reader.GetOrdinal("RAGIONE")),
                                    IdTrasmissione = reader.GetValue(reader.GetOrdinal("ID_TRASMISSIONE")).ToString(),
                                    Email = (reader.IsDBNull(reader.GetOrdinal("EMAIL_DESTINATARIO")) ? string.Empty : reader.GetValue(reader.GetOrdinal("EMAIL_DESTINATARIO")).ToString())
                                });
                        }
                    }
                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }


        /// <summary>
        /// Reperimento ID da docspa del record di tipo "C" per il fascicolo o per il nodo di titolario (F o T)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idForT"></param>
        /// <returns></returns>
        protected virtual string getIdFolderCType(string idForT)
        {
            string recordC = string.Empty;

            using (DocsPaDB.DBProvider dbProvider = new DocsPaDB.DBProvider())
            {
                // Query per stabilire la tipologia del record fornito in ingresso
                string commandText = string.Format("select cha_tipo_proj from project where system_id = {0}", idForT);

                string field;
                if (dbProvider.ExecuteScalar(out field, commandText))
                {
                    if (field.Equals("C"))
                    {
                        recordC = idForT;
                    }
                    else
                    {
                        if (field.Equals("T"))
                        {
                            // Il record è di tipo "T", reperimento record di tipo "C" 
                            commandText = string.Format("select system_id from project where id_parent in (select system_id from project where id_parent = {0} and cha_tipo_proj = 'F')", idForT);
                        }
                        else if (field.Equals("F"))
                        {
                            // Il record è di tipo "F", reperimento record di tipo "C" 
                            commandText = string.Format("select system_id from project where id_parent = {0}", idForT);
                        }

                        if (dbProvider.ExecuteScalar(out field, commandText))
                        {
                            recordC = field;
                        }
                    }
                }
            }

            return recordC;
        }

        /// <summary>
        /// Servizio per il reperimento degli storici sullo stato dei documenti e/o dei campi profilati 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.GetStoricoDiagrammaProfilatiStatoResponse GetStoricoDiagrammaProfilatiStato (VtDocs.BusinessServices.Entities.Document.GetStoricoDiagrammaProfilatiStatoRequest request)
        {

            VtDocs.BusinessServices.Entities.Document.GetStoricoDiagrammaProfilatiStatoResponse response = new VtDocs.BusinessServices.Entities.Document.GetStoricoDiagrammaProfilatiStatoResponse();
            response.ListaStoricoDiagrammaStato = (request.DiagrammaStato) ? (new List<VtDocs.BusinessServices.Entities.Document.StoricoDiagrammaStato>()) : (null);
            response.ListaStoricoProfilatiStato = (request.ProfilatiStato) ? (new List<VtDocs.BusinessServices.Entities.Document.StoricoProfilatiStato>()) : (null);

            try
            {
                response.ListaStoricoDiagrammaStato = new List<VtDocs.BusinessServices.Entities.Document.StoricoDiagrammaStato>();

                if (request.DiagrammaStato)
                {
                    DataTable dt = new DataTable();
                    DataRow dr = dt.NewRow();
                    DataSet set = BusinessLogic.DiagrammiStato.DiagrammiStato.getDiagrammaStoricoDoc(request.DocNumber.ToString());

                    for (int i = 0; i < set.Tables[0].Rows.Count; i++)
                    {                  
                        dr = set.Tables[0].Rows[i];
                        VtDocs.BusinessServices.Entities.Document.StoricoDiagrammaStato diag = new VtDocs.BusinessServices.Entities.Document.StoricoDiagrammaStato();
                        diag.Utente = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(dr, "Utente", true);

                        string dataModifica = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(dr, "Data", true);
                        if (!string.IsNullOrEmpty(dataModifica))
                        {
                            DateTime data;
                            if (DateTime.TryParse(dataModifica, out data))
                                diag.Data = data;
                        }
                        diag.VecchioStato = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(dr, "Vecchio stato", true);
                        diag.NuovoStato = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(dr, "Nuovo stato", true);
                        diag.Ruolo = DocsPaUtils.Data.DataReaderHelper.GetValue<string>(dr, "Ruolo", true);

                        response.ListaStoricoDiagrammaStato.Add(diag);
                    }
                }

                if (request.ProfilatiStato)
                {
                    var responseGetDocumento = this.GetDocumento(new VtDocs.BusinessServices.Entities.Document.GetDocumentoRequest
                    {
                        InfoUtente = request.InfoUtente,
                        IdProfile = request.DocNumber.ToString(),
                        DocNumber = request.DocNumber.ToString(),
                        TrowOnError = true
                    });

                    if (responseGetDocumento.Success && responseGetDocumento.Documento.template != null)
                    {
                        ArrayList result = BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.getListaStoricoAtto(
                                        responseGetDocumento.Documento.template.SYSTEM_ID.ToString(),
                                        responseGetDocumento.Documento.docNumber, request.InfoUtente.idGruppo);

                        foreach (DocsPaVO.ProfilazioneDinamica.StoricoProfilati ogg in result)
                        {
                            VtDocs.BusinessServices.Entities.Document.StoricoProfilatiStato prof = new VtDocs.BusinessServices.Entities.Document.StoricoProfilatiStato();
                            prof.Utente = ogg.utente.descrizione;
                            prof.Ruolo = ogg.ruolo.descrizione;
                            
                            DateTime dataModifica;
                            if (DateTime.TryParse(ogg.dta_modifica, out dataModifica))
                                prof.Data =  dataModifica;

                            prof.Campo = ogg.oggetto.DESCRIZIONE;
                            prof.VecchioValore = ogg.var_desc_modifica;

                            response.ListaStoricoProfilatiStato.Add(prof);
                        }
                    }

                }

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }


        /// <summary>
        /// Servizio per la cancellazione di un allegato
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebMethod()]
        public VtDocs.BusinessServices.Entities.Document.DeleteAllegatoResponse DeleteAllegato(VtDocs.BusinessServices.Entities.Document.DeleteAllegatoRequest request)
        {
            VtDocs.BusinessServices.Entities.Document.DeleteAllegatoResponse response = new VtDocs.BusinessServices.Entities.Document.DeleteAllegatoResponse();
            Allegato allegatoDaEliminare = null;
            response.Success = false;

            try
            {
                if (request.idDocumento != 0 && request.idDocumentoAllegato != 0)
                {
                    ArrayList array = BusinessLogic.Documenti.AllegatiManager.getAllegati(request.idDocumento.ToString(), null);

                    foreach (DocsPaVO.documento.Allegato all in array)
                    {
                        if (all.docNumber.Equals(request.idDocumentoAllegato.ToString()))
                        {
                            allegatoDaEliminare = all;
                            break;
                        }
                    }

                    if (allegatoDaEliminare != null)
                    {
                        BusinessLogic.Documenti.AllegatiManager.rimuoviAllegato(allegatoDaEliminare, request.InfoUtente);
                        response.Success = true;
                    }

                }
            }
            catch (Exception ex)
            {
                response.Success = false;

                if (request.TrowOnError)
                    throw DocsPaUtils.Exceptions.SoapExceptionFactory.Create(ex);
                else
                    response.Exception = ex.Message;
            }

            return response;
        }


    }


}
