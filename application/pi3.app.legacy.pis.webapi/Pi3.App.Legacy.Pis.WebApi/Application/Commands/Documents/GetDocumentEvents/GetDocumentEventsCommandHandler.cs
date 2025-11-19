// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentEvents
{
    // Richiede libreria MediatR
    public class GetDocumentEventsCommandHandler : IRequestHandler<GetDocumentEventsCommand, GetDocumentEventsCommandResponse>
    {
        #region Public Members

        public GetDocumentEventsCommandHandler(ILogger<GetDocumentEventsCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetDocumentEventsCommandResponse> Handle(GetDocumentEventsCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetDocumentEvents - START");

            GetDocumentEventsCommandResponse response = new GetDocumentEventsCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (string.IsNullOrEmpty(request.IdDocument))
                {
                    throw new RestException("REQUIRED_ID");
                }
                #endregion

                #region implementazione

                string[] eventiModifica = { "ADD_VERSION", "AGG_DOC_GRIGIO", "AGG_PROT", "ANN_PRED", "ANNULLA_PROTO", "ANNULL_PROTOCOLLO",
                                              "CESTINA_DOC", "CONSOLIDADOCUMENTO", "CREATE_DOC_AND_ADD_IN_FASC", "DEL_DOC", "DOC_ADD_INFASC",
                                              "DOC_ADD_INFOLDER", "DOC_CAMBIO_STATO", "DOC_CAMBIO_STATO_ADMIN", "DOC_DEL_FROM_FOLDER",
                                              "DOC_NEW_ALLEGATO", "DOC_PROT", "DOC_RIMUOVI_ALLEGATO", "DOC_RIMUOVI_VERSIONE", "DOC_SIGNATURE",
                                              "DOC_SIGNATURE_P", "DOCUMENTOCONVERSIONEPDF", "DOCUMENTO_EXEC_ANNULLA_REPERTORIO",
                                              "DOCUMENTO_REPERTORIATO", "DOCUMENTOTIMESTAMP", "DOC_VERIFIED", "MODIFICADOCSTATOFINALE",
                                              "MODIFIED_OBJECT_DOC", "MODIFIED_OBJECT_PROTO", "MOD_MITT_DEST", "MOD_MITT_INT", "PUT_FILE",
                                              "RECORD_PREDISPOSED", "SCAMBIA_DOC","DOC_VERIFIED","DOC_STEP_OVER","DOC_SIGNATURE_P",
                                              "DOC_SIGNATURE","INTERROTTO_PROCESSO_DOCUMENTO_DAL_PROPONENTE","INTERROTTO_PROCESSO_DOCUMENTO_DAL_TITOLARE",
                                              "CONCLUSIONE_PROCESSO_LF_DOCUMENTO","TRONCAMENTO_PROCESSO","AVVIATO_PROCESSO_DI_FIRMA_DOCUMENTO",
                                              "AVVIATO_PROCESSO_DI_FIRMA_ALLEGATO","INTERROTTO_PROCESSO_DOCUMENTO_DA_ADMIN" };
                ArrayList evMod = new ArrayList(eventiModifica);

                if (request.IdDocument != null)
                {
                    List<DocsPaVO.documento.LogDocumento> eventi = DBUtils.getListaLogEventiDoc(request.IdDocument, _pi3DbContext);
                    if (eventi != null && eventi.Count > 0)
                    {
                        List<LogEvent> logEventi = new List<LogEvent>();
                        LogEvent loge = null;
                        foreach (DocsPaVO.documento.LogDocumento evento in eventi)
                        {
                            // inserire controllo sul tipo degli eventi
                            if (!request.AllEvents && !evMod.Contains(evento.codAzione.ToUpper())) continue;
                            loge = new LogEvent();
                            loge.ActionCode = evento.codAzione;
                            loge.ActionDate = evento.dataAzione;
                            loge.AdministrationId = evento.idAmm;
                            loge.Id = evento.systemId;
                            loge.ObjectDescription = evento.descrOggetto;
                            loge.OperationExecuted = evento.chaEsito;
                            loge.OperatorDescription = evento.descProduttore;
                            loge.OperatorGroupID = evento.idGruppoOperatore;
                            loge.OperatorPeopleID = evento.idPeopleOPeratore;
                            loge.OperatorUsername = evento.userIdOperatore;
                            logEventi.Add(loge);

                        }

                        response.Events = logEventi;
                    }
                    else
                    {
                        throw new RestException("DOCUMENT_NOT_FOUND");
                    }
                }

                #endregion

                response.Code = GetDocumentEventsResponseCode.OK;

                _logger.LogInformation("end GetDocumentEvents");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetDocumentEvents: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetDocumentEventsCommandResponse();
                response.Code = GetDocumentEventsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetDocumentEvents");
                response = new GetDocumentEventsCommandResponse();
                response.Code = GetDocumentEventsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocumentEventsCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}