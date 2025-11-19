// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.InteropPitre.WebApi.Infrastructure.Services.Repository;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Text.Json;

namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.AnalyzeDocumentReceivedProof
{ 
    public class AnalyzeDocumentReceivedProofHandler : IRequestHandler<AnalyzeDocumentReceivedProofRequest>
    {
        #region Public Members

        public AnalyzeDocumentReceivedProofHandler(ILogger<AnalyzeDocumentReceivedProofHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext,
            IDbUtilsService repositoryService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repositoryService = repositoryService;
        }

        public async Task Handle(AnalyzeDocumentReceivedProofRequest request, CancellationToken cancellationToken)
        {
            try
            {
                this._logger.LogInformation($"New request: {JsonSerializer.Serialize<AnalyzeDocumentReceivedProofRequest>(request)}");

                var idTenantSender = await this._dbContext.AmministraEntities
                    .Where(x => x.VAR_CODICE_AMM!.ToUpper() == request.SenderRecordInfo.AdministrationCode.ToUpper())
                    .Select(x => x.SYSTEM_ID)
                    .FirstOrDefaultAsync();

                var idAOOSender = await this._dbContext.RegistroEntities
                    .Where(x => x.VAR_CODICE!.ToUpper() == request.SenderRecordInfo.AOOCode.ToUpper() && x.ID_AMM == idTenantSender)
                    .Select(x => x.SYSTEM_ID)
                    .FirstOrDefaultAsync();

                var recordInfo = string.Format("{0}{1}{2}{1}{3}",
                    request.ReceiverRecordInfo.RecordNumber,
                    await this.GetSeparator(request.SenderRecordInfo.AdministrationCode),
                    request.ReceiverRecordInfo.AOOCode,
                    request.ReceiverRecordInfo.RecordDate.Year
                    );

                var receivers = await this._repositoryService.GetCorrespondentsId(request.ReceiverCode);

                var statoInvioEntities = await this._dbContext.ProfileEntities
                    .Join(this._dbContext.StatoInvioEntities, p => p.SYSTEM_ID, s => s.ID_PROFILE, (p, s) => new { p, s })
                    .Where(x => x.p.NUM_PROTO == request.SenderRecordInfo.RecordNumber.AsLong()
                            && x.p.DTA_PROTO == request.SenderRecordInfo.RecordDate
                            && x.p.ID_REGISTRO == idAOOSender
                            && x.s.VAR_CODICE_AMM!.ToUpper() == request.ReceiverRecordInfo.AdministrationCode.ToUpper()
                            && x.s.VAR_CODICE_AOO!.ToUpper() == request.ReceiverRecordInfo.AOOCode.ToUpper()
                            && x.s.VAR_INDIRIZZO!.ToUpper() == request.ReceiverUrl.ToUpper()
                            && receivers.Any(y => x.s.ID_CORR_GLOBALE.ToString() == y)
                            )
                    .Select(x => x.s)
                    .ToListAsync();

                foreach(var s in statoInvioEntities)
                {
                    s.VAR_PROTO_DEST = recordInfo;
                    s.DTA_PROTO_DEST = request.ReceiverRecordInfo.RecordDate;
                    s.STATUS_C_MASK = "VNVVANN";
                    s.CHA_ANNULLATO = null;
                    s.VAR_MOTIVO_ANNULLA = null;
                }

                await ((DbContext)this._dbContext).SaveChangesAsync();

                await this._repositoryService.InsertLog(string.Empty, string.Format(LoggerDescriptions.MessageProcessingOk, request.SenderRecordInfo.RecordNumber, request.SenderRecordInfo.RecordDate), (long)ErrorMessageEnum.None);
            }
            catch(Exception ex)
            {
                this._logger.LogError($"Unhandled Exception: {ex.Message}");
                await this._repositoryService.InsertLog(string.Empty, string.Format(LoggerDescriptions.MessageProcessingFailed, request.SenderRecordInfo.RecordNumber, request.SenderRecordInfo.RecordDate), (long)ErrorMessageEnum.Error);
                throw;
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AnalyzeDocumentReceivedProofHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDbUtilsService _repositoryService;

        protected async Task<string> GetSeparator(string administrationCode)
        {
            var query = await this._dbContext.AmministraEntities.Where(a => a.VAR_CODICE_AMM == administrationCode)
                .Select(x => new { x.CHA_STR_SEGNATURA })
                .FirstOrDefaultAsync();

            return query?.CHA_STR_SEGNATURA ?? "/";
        }

        #endregion
    }
}
