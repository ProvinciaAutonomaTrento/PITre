// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.Domain;
using Pi3.App.InteropPitre.WebApi.Extensions;
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.Repository;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Text.Json;

namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.AnalyzeDocumentDroppedOrErrorMessageProof
{
    public class AnalyzeDocumentDroppedOrErrorMessageProofHandler : IRequestHandler<AnalyzeDocumentDroppedOrErrorMessageProofRequest>
    {
        #region Public Members

        public AnalyzeDocumentDroppedOrErrorMessageProofHandler(ILogger<AnalyzeDocumentDroppedOrErrorMessageProofHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService, IDistributedCache distributedCache, IDbUtilsService repositoryService, IConfigurationService configurationService, IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._distributedCache = distributedCache;
            this._repositoryService = repositoryService;
            this._configurationService = configurationService;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task Handle(AnalyzeDocumentDroppedOrErrorMessageProofRequest request, CancellationToken cancellationToken)
        {
            this._logger.LogInformation($"New request: {JsonSerializer.Serialize<AnalyzeDocumentDroppedOrErrorMessageProofRequest>(request)}");

            try
            {
                var instance = this._httpContextAccessor.HttpContext?.GetRouteValue("Instance")!.ToString();
                this._claimsPrincipalService.Current.SetPi3Claim(Pi3ClaimTypes.Instance, instance);

                var idAmm = await this._dbContext.AmministraEntities
                        .Where(x => x.VAR_CODICE_AMM!.ToUpper() == request.SenderRecordInfo.AdministrationCode.ToUpper())
                        .Select(x => x.SYSTEM_ID)
                        .FirstOrDefaultAsync();

                var idAOO = await this._dbContext.RegistroEntities.AsNoTracking()
                    .Join(this._dbContext.AmministraEntities.AsNoTracking(), r => r.ID_AMM, a => a.SYSTEM_ID, (r, a) => new { r, a })
                    .Where(x => x.a.VAR_CODICE_AMM == request.SenderRecordInfo.AdministrationCode
                        && x.r.VAR_CODICE == request.SenderRecordInfo.AOOCode)
                    .Select(x => x.r.SYSTEM_ID)
                    .FirstOrDefaultAsync();

                var receivers = await this._repositoryService.GetCorrespondentsId(request.ReceiverCode);

                var statoInvioEntities = await this._dbContext.StatoInvioEntities
                    .Join(this._dbContext.ProfileEntities, s => s.ID_PROFILE, p => p.SYSTEM_ID, (s, p) => new { s, p })
                    .Where(x => x.p.NUM_PROTO == request.SenderRecordInfo.RecordNumber.AsLong()
                            && x.p.DTA_PROTO == request.SenderRecordInfo.RecordDate
                            && x.p.ID_REGISTRO == idAOO
                            && x.s.VAR_CODICE_AMM == request.ReceiverRecordInfo.AdministrationCode
                            && x.s.VAR_CODICE_AOO == request.ReceiverRecordInfo.AOOCode
                            && x.s.VAR_INDIRIZZO == request.ReceiverUrl
                            && receivers.Any(y => x.s.ID_CORR_GLOBALE.ToString() == y))
                    .Select(x => x.s)
                    .ToListAsync();

                foreach (var s in statoInvioEntities)
                {
                    s.CHA_ANNULLATO = (request.Operation == OperationDiscriminator.Drop) ? "1" : "E";
                    s.STATUS_C_MASK = (request.Operation == OperationDiscriminator.Drop) ? "VNVVVNN" : "XNVNNXN";
                    s.VAR_MOTIVO_ANNULLA = request.Reason;
                }

                await ((DbContext)this._dbContext).SaveChangesAsync();

                // Aggiunta logger
                var logMessage = request.Operation == OperationDiscriminator.Drop ?
                    string.Format(LoggerDescriptions.MessageProcessingCancellationOK, request.SenderRecordInfo.RecordNumber, request.SenderRecordInfo.RecordDate.ToString()) :
                    string.Format(LoggerDescriptions.MessageProcessingExceptionOK, request.SenderRecordInfo.RecordNumber, request.SenderRecordInfo.RecordDate.ToString());

                var senderUrl = await this._configurationService.GetValue<string>("INTEROP_SERVICE_URL");

                if (request.Operation == OperationDiscriminator.Error)
                {
                    var profileEntity = await this._dbContext.ProfileEntities.AsNoTracking()
                        .Where(x => x.NUM_PROTO == request.SenderRecordInfo.RecordNumber.AsLong()
                        && x.DTA_PROTO == request.SenderRecordInfo.RecordDate
                        && x.ID_REGISTRO == idAOO)
                        .Select(x => new
                        {
                            x.SYSTEM_ID,
                            x.AUTHOR
                        }).FirstOrDefaultAsync();

                    // Tipo notifica
                    var notificationTypeEntity = await this._dbContext.TipoNotificaEntities.AsNoTracking()
                        .Where(x => x.VAR_CODICE_NOTIFICA == "eccezione")
                        .FirstAsync();

                    var entity = new NotificaEntity
                    {
                        DOCNUMBER = profileEntity.SYSTEM_ID,
                        VAR_MITTENTE = senderUrl,
                        VAR_DESTINATARIO = request.ReceiverCode,
                        VAR_MSGID = string.Empty,
                        ID_TIPO_NOTIFICA = notificationTypeEntity.SYSTEM_ID,
                        VAR_CONSEGNA = "1",
                        VAR_GIORNO_ORA = DateTime.Now,
                        VAR_RISPOSTE = string.Empty,
                        VAR_OGGETTO = request.Reason,
                        VERSION_ID = null
                    };

                    await this._dbContext.NotificaEntities.AddAsync(entity);

                    // Aggiornamento status mask non necessario perché è una notifica di eccezione
                    // E' corretto?

                    // Ruolo autore ultima spedizione IS
                    var roleEntity = await this._dbContext.SendStoEntities.AsNoTracking()
                        .Where(x => x.ID_PROFILE == profileEntity.SYSTEM_ID
                                && x.MAIL_MITTENTE == "N.A.")
                        .OrderByDescending(x => x.SYSTEM_ID)
                        .Select(x => x.ID_GROUP_SENDER)
                        .FirstOrDefaultAsync();

                    //await this.ImpersonateUser(profileEntity.AUTHOR, roleEntity);

                    var logDescription = string.Format(LoggerDescriptions.LogMessageExceptionNotification, request.Reason, request.ReceiverCode);

                    await this._webMethodLoggerService.LogOK("EXCEPTION_SEND_SIMPLIFIED_INTEROPERABILITY",
                        profileEntity.SYSTEM_ID.ToString(),
                        logDescription,
                        null,
                        null,
                        false,
                        idAmm.ToString(),
                        profileEntity.AUTHOR.ToString(),
                        null,
                        roleEntity.ToString()
                        );
                }
            }
            catch(Exception ex)
            {
                this._logger.LogError($"Unhandled Exception: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AnalyzeDocumentDroppedOrErrorMessageProofHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IDbUtilsService _repositoryService;
        protected readonly IConfigurationService _configurationService;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        #endregion
    }
}
