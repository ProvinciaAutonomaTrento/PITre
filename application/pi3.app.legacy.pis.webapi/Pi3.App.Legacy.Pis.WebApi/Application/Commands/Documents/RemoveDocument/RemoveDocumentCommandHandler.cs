// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CestinaDocumento;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.VerificaDirittiCestinaDocumento;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using StackExchange.Redis;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.RemoveDocument
{
    // Richiede libreria MediatR
    public class RemoveDocumentCommandHandler : IRequestHandler<RemoveDocumentCommand, RemoveDocumentCommandResponse>
    {
        #region Public Members

        public RemoveDocumentCommandHandler(
            IWebMethodLoggerService loggerService,
            ILogger<RemoveDocumentCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._loggerService = loggerService;
        }

        public async Task<RemoveDocumentCommandResponse> Handle(RemoveDocumentCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("RemoveDocument - START");

            RemoveDocumentCommandResponse response = new RemoveDocumentCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (string.IsNullOrWhiteSpace(request.IdDocument))
                    throw new RestException("REQUIRED_ID");
                #endregion

                #region implementazione
                // Controllo visibilit� documento
                try
                {
                    await _pi3DbContext.AssertSecurityRights(request.IdDocument, infoUtente.idPeople, infoUtente.idGruppo, SecurityRightTypesEnum.Write);
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }
                SchedaDocumento? schedaDocumento = null;
                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        schedaDocumento = await this.BuildSchedaDocumento(request.IdDocument);
                    }
                }
                catch(Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                if (schedaDocumento != null)
                {
                    var dirittiReq = await this._mediator.Send(new VerificaDirittiCestinaDocumentoCommand()
                    {
                        InfoUtente = infoUtente,
                        SchedaDoc = schedaDocumento
                    });
                    var verificaDirittiCestino = dirittiReq.Output;

                    if (string.IsNullOrEmpty(infoUtente.diSistema) || infoUtente.diSistema != "1")
                    {
                        if (!RestUtils.IsRuoloAuthorized(ruolo, "DO_PRO_RIMUOVI"))
                            throw new RestException("REMOVE_UNAUTHORIZED", Messages.RoleNotAuth);
                    }

                    var cestResp = await this._mediator.Send(new CestinaDocumentoCommand()
                    {
                        Infoutente = infoUtente,
                        SchedaDoc = schedaDocumento,
                        Note = request.RemovalNote,
                        TipoDoc = null
                    });

                    if(cestResp == null || !string.IsNullOrEmpty(cestResp.ErrorMsg))
                    {
                        throw new Exception(cestResp.ErrorMsg);
                    }
                    else
                    {
                        response.ResultMessage = string.Format(Messages.DocDeleted, request.IdDocument);
                    }

                }
                else
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }
                #endregion

                response.Code = MessageResponseCode.OK;

                _logger.LogInformation("end RemoveDocument");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione RemoveDocument: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new RemoveDocumentCommandResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione RemoveDocument");
                response = new RemoveDocumentCommandResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<RemoveDocumentCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IWebMethodLoggerService _loggerService;

        private async Task<SchedaDocumento> BuildSchedaDocumento(string idDocument)
        {
            SchedaDocumento schedaDocumento = new SchedaDocumento();
            var profileEntity = await this._pi3DbContext.ProfileEntities.AsNoTracking().FirstAsync(p => p.SYSTEM_ID == idDocument.AsLong());
            schedaDocumento.docNumber = profileEntity.DOCNUMBER.ToString();

            AuthorGroupEntity authorGroupEntity = null;
            if (profileEntity.ID_RUOLO_CREATORE.HasValue)
                authorGroupEntity = await this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(cg => cg.SYSTEM_ID == profileEntity.ID_RUOLO_CREATORE)
                    .Select(cg => new AuthorGroupEntity() { ID_GRUPPO = cg.ID_GRUPPO }).FirstAsync();

            AuthorUOEntity uoCreatoreEntity = null;
            if (profileEntity.ID_UO_CREATORE.HasValue)
                uoCreatoreEntity = await this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(uo => uo.SYSTEM_ID == profileEntity.ID_UO_CREATORE)
                    .Select(uo => new AuthorUOEntity() { SYSTEM_ID = uo.SYSTEM_ID, VAR_CODICE = uo.VAR_CODICE }).FirstOrDefaultAsync();

            schedaDocumento.creatoreDocumento = new CreatoreDocumento(
            idPeople: profileEntity.AUTHOR.GetValueOrDefault().ToString(),
            idRuolo: authorGroupEntity != null ? authorGroupEntity.ID_GRUPPO.ToString() : null,
            idUo: uoCreatoreEntity != null ? uoCreatoreEntity.SYSTEM_ID.ToString() : null,
            codiceUo: uoCreatoreEntity != null ? uoCreatoreEntity.VAR_CODICE : null)
            {
                idCorrGlob_Ruolo = profileEntity.ID_RUOLO_CREATORE.GetValueOrDefault().ToString(),
                idCorrGlob_UO = uoCreatoreEntity != null ? uoCreatoreEntity.SYSTEM_ID.ToString() : null,
                uo_codiceCorrGlobali = uoCreatoreEntity != null ? uoCreatoreEntity.VAR_CODICE.ToString() : null,
                idPeopleDelegato = profileEntity.ID_PEOPLE_DELEGATO.GetValueOrDefault().ToString()
            };

            if (profileEntity.ID_TIPO_ATTO.HasValue)
            {
                var tipoAttoEntity = await this._pi3DbContext
                    .TipoAttoEntities
                    .AsNoTracking()
                    .FirstAsync(ta => ta.SYSTEM_ID == profileEntity.ID_TIPO_ATTO);

                schedaDocumento.tipologiaAtto = new TipologiaAtto()
                {
                    systemId = tipoAttoEntity.SYSTEM_ID.ToString(),
                    descrizione = tipoAttoEntity.VAR_DESC_ATTO
                };
            }

            return schedaDocumento;
        }

        protected class AuthorUOEntity
        {
            public long SYSTEM_ID { get; set; }
            public string VAR_CODICE { get; set; }
        }
        protected class AuthorGroupEntity
        {
            public long? ID_GRUPPO { get; set; }
        }
        #endregion
    }

}