// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Services.WebMethodLogger;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CestinaDocumento
{
    public class CestinaDocumentoCommandHandler : IRequestHandler<CestinaDocumentoCommand, CestinaDocumentoCommandResponse>
    {
        public CestinaDocumentoCommandHandler(ILogger<CestinaDocumentoCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._repository = repository;
        }

        public async Task<CestinaDocumentoCommandResponse> Handle(CestinaDocumentoCommand request, CancellationToken cancellationToken)
        {
            bool output = true;
            var errorMsg = string.Empty;

            try
            {
                var idAmministrazione = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idProfile = request.SchedaDoc.docNumber.AsLong();

                var aggregate = await this._repository.Get(idAmministrazione, request.SchedaDoc.docNumber, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadClassifications = false,
                        LoadAllegati = true,
                        LoadAggregazioni = false,
                        LoadVersions = false,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = false
                    }
                });

                if (aggregate.Reserved)
                {
                    if (await this._dbContext.CheckinCheckoutEntities.AsNoTracking().AnyAsync(c => c.ID_DOCUMENT == idProfile && c.ID_USER == idPeople))
                    {
                        aggregate.Unreserve();
                    }
                    else
                    {
                        throw new DocumentoBloccatoException();
                    }
                }

                aggregate.Recycle(new TextValue(request.Note));
                await this._repository.Update(aggregate);

                await this._webMethodLoggerService.LogOK("DOCUMENTOEXECRIMUOVISCHEDA", request.SchedaDoc.systemId, string.Format(Resources.LogSpostamentoInCestino, request.SchedaDoc.docNumber));
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception : ex, message: ex.Message);
                output = false;
                errorMsg = ex.Message;
                await this._webMethodLoggerService.LogKO("DOCUMENTOEXECRIMUOVISCHEDA", request.SchedaDoc.systemId, string.Format(Resources.LogSpostamentoInCestino, request.SchedaDoc.docNumber));
            }

            return new()
            {
                Output = output, 
                ErrorMsg = errorMsg
            };
        }


        #region Private Members
        protected readonly ILogger<CestinaDocumentoCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected IDocumentoAmministrativoRepository _repository;
        #endregion
    }
}
