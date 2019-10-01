using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VtDocsWS.WebServices;
using log4net;

namespace VtDocsWS.Manager
{
    public class APSS_ServicesManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(APSS_ServicesManager));

        public static VtDocsWS.Services.APSS_Services.SendPublicationResult.SendPublicationResultResponse SendPublicationResult(VtDocsWS.Services.APSS_Services.SendPublicationResult.SendPublicationResultRequest request)
        {
            Services.APSS_Services.SendPublicationResult.SendPublicationResultResponse response = new Services.APSS_Services.SendPublicationResult.SendPublicationResultResponse();
            try
            {
                response.Success = true;
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.InfoUtente infoUtente = null;

                //Inizio controllo autenticazione utente
                infoUtente = Utils.CheckAuthentication(request, "SendPublicationResult");

                utente = BusinessLogic.Utenti.UserManager.getUtenteById(infoUtente.idPeople);
                if (utente == null)
                {
                    //Utente non trovato
                    throw new PisException("USER_NO_EXIST");
                }
                //Fine controllo autenticazione utente

                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new PisException("REQUIRED_ID");
                }

                if (string.IsNullOrEmpty(request.PublicationResult))
                {
                    throw new Exception("Publication Result Required");
                }

                DocsPaVO.ExternalServices.PubblicazioneAPSS pubb = BusinessLogic.Amministrazione.SistemiEsterni.APSSGetPubbByDocId(request.IdDocument);
                if (pubb == null)
                {
                    throw new Exception("Id documento non presente nella tabella di pubblicazione.");
                }
                //Reperimento ruolo utente
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                try
                {
                    ruolo = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(infoUtente.idGruppo);
                    if (ruolo == null)
                    {
                        //Ruolo non trovato
                        throw new PisException("ROLE_NO_EXIST");
                    }
                }
                catch
                {
                    //Ruolo non trovato
                    throw new PisException("ROLE_NO_EXIST");
                }

                pubb.PublishResult = request.PublicationResult;
                pubb.PublishReason = request.ResultReason;
                pubb.PublishedResultDate = DateTime.Now.ToString();

                if (request.PublicationResult == "OK")
                {
                    //DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();
                
                    //// flusso pubblicazione con modifica campo Data Esito Pubblicazione
                    //try
                    //{
                    //    if (!string.IsNullOrEmpty(request.IdDocument))
                    //    {
                    //        documento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, request.IdDocument, request.IdDocument);
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    throw new PisException("DOCUMENT_NOT_FOUND");
                    //}

                    //foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom ogg in documento.template.ELENCO_OGGETTI)
                    //{
                    //    if (ogg.DESCRIZIONE.ToUpper() == "ESITO PUBBLICAZIONE")
                    //    {
                    //        ogg.VALORE_DATABASE = DateTime.Now.ToString("dd/MM/yyyy");
                    //    }
                    //}

                    

                    if (!string.IsNullOrEmpty(pubb.IdSingleTrasm))
                    {
                        // flusso ripubblicazione con accettazione trasmissione
                        DocsPaVO.trasmissione.TrasmissioneUtente[] ArrayTrasmUtente= BusinessLogic.Trasmissioni.TrasmManager.getTrasmissioneUtenteInRuolo(infoUtente,pubb.IdSingleTrasm,utente);
                        if (ArrayTrasmUtente == null || ArrayTrasmUtente.Length < 1)
                        {
                            throw new Exception("Trasmissione relativa alla ripubblicazione non trovata");
                        }
                        DocsPaVO.trasmissione.TrasmissioneUtente trasmUtente = ArrayTrasmUtente[0];
                        trasmUtente.tipoRisposta = DocsPaVO.trasmissione.TipoRisposta.ACCETTAZIONE;
                        //trasmUtente.dataAccettata = DateTime.Now.ToString("dd/MM/yyyy");
                        trasmUtente.noteAccettazione = "Ripubblicazione effettuata.";
                        string idTrasm = BusinessLogic.Amministrazione.SistemiEsterni.APSSgetIdTrasmFromTUtente(trasmUtente.systemId);
                        string errore = "", mode="", idObj="";
                        bool effettuata = BusinessLogic.Trasmissioni.ExecTrasmManager.executeAccRifMethod(trasmUtente, idTrasm, ruolo, infoUtente, out errore, out mode, out idObj);
                        if (!effettuata)
                        {
                            throw new Exception("Errore nell'accettazione : "+errore);
                        }

                    }
                    bool daAggiornareUffRef = false;
                    //BusinessLogic.Documenti.DocSave.save(infoUtente, documento, false, out daAggiornareUffRef, ruolo);
                    //BusinessLogic.ProfilazioneDinamica.ProfilazioneDocumenti.salvaInserimentoUtenteProfDim(infoUtente, documento.template, request.IdDocument);
                    BusinessLogic.Amministrazione.SistemiEsterni.APSSUpdateEsitoPubb(request.IdDocument, DateTime.Now.ToString("dd/MM/yyyy")); 

                    // update tabella trasmissioni
                    BusinessLogic.Amministrazione.SistemiEsterni.APSSUpdateResultPubbInTable(pubb);
                    response.OperationResult = "OK";

                }
                else
                {
                    // funzionamento traslato in un altra classe

                    //if (!string.IsNullOrEmpty(pubb.IdSingleTrasm))
                    //{
                    //    // flusso ripubblicazione con rifiuto trasmissione
                    //    DocsPaVO.trasmissione.TrasmissioneUtente[] ArrayTrasmUtente = BusinessLogic.Trasmissioni.TrasmManager.getTrasmissioneUtenteInRuolo(infoUtente, pubb.IdSingleTrasm, utente);
                    //    if (ArrayTrasmUtente == null || ArrayTrasmUtente.Length < 1)
                    //    {
                    //        throw new Exception("Trasmissione relativa alla ripubblicazione non trovata");
                    //    }
                    //    DocsPaVO.trasmissione.TrasmissioneUtente trasmUtente = ArrayTrasmUtente[0];
                    //    trasmUtente.tipoRisposta = DocsPaVO.trasmissione.TipoRisposta.RIFIUTO;
                    //    //trasmUtente.dataAccettata = DateTime.Now.ToString("dd/MM/yyyy");
                    //    if (!string.IsNullOrEmpty(request.ResultReason))
                    //        trasmUtente.noteRifiuto = "Ripubblicazione non effettuata per la seguente ragione: " + request.ResultReason;
                    //    else
                    //        trasmUtente.noteRifiuto = "Ripubblicazione non effettuata";
                        
                    //    string idTrasm = BusinessLogic.Amministrazione.SistemiEsterni.APSSgetIdTrasmFromTUtente(trasmUtente.systemId);
                    //    string errore = "", mode = "", idObj = "";
                    //    bool effettuata = BusinessLogic.Trasmissioni.ExecTrasmManager.executeAccRifMethod(trasmUtente, idTrasm, ruolo, infoUtente, out errore, out mode, out idObj);
                    //    if (!effettuata)
                    //    {
                    //        throw new Exception("Errore nell'accettazione : " + errore);
                    //    }
                    //}

                    // update tabella trasmissioni
                    BusinessLogic.Amministrazione.SistemiEsterni.APSSUpdateResultPubbInTable(pubb);
                    response.OperationResult = "OK";
                }
            }
            catch (PisException pisEx)
            {
                logger.ErrorFormat("PISException: {0}, {1}", pisEx.ErrorCode, pisEx.Description);
                response.Error = new Services.ResponseError
                {
                    Code = pisEx.ErrorCode,
                    Description = pisEx.Description
                };

                response.Success = false;
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Eccezione Generica: APPLICATION_ERROR, {0}", ex.Message);
                response.Error = new Services.ResponseError
                {
                    Code = "APPLICATION_ERROR",
                    Description = ex.Message
                };

                response.Success = false;
            }

            return response;
        }
    }
}