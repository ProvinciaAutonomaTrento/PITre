// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SalvaDataScadenzaDoc
{
    public class SalvaDataScadenzaDocCommandHandler : IRequestHandler<SalvaDataScadenzaDocCommand, SalvaDataScadenzaDocCommandResponse>
    {
        public SalvaDataScadenzaDocCommandHandler(ILogger<SalvaDataScadenzaDocCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository repository)
        {

            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
        }


        public async Task<SalvaDataScadenzaDocCommandResponse> Handle(SalvaDataScadenzaDocCommand request, CancellationToken cancellationToken)
        {
            string idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            var aggregate = await this._repository.Get(idTenant, request.DocNumber, new ILoadBehavior[1]
            {
                new GetDocumentoAmministrativoLoadBehavior()
                {
                    LoadProfiles = false,
                    LoadClassifications = false,
                    LoadAllegati = false,
                    LoadAggregazioni = false,
                    LoadVersions = false,
                    LoadPermissions = false,
                    LoadMittentiDestinatari = false,
                    LoadRelatedElements = false
                }
            });

            DateTime? dtaScadenza = null;

            if (string.IsNullOrEmpty(request.DataScadenza))
            {
                var idTipoAttoAsLong = request.IdTipoAtto.AsLong();
                var scadenza = await this._dbContext.TipoAttoEntities.AsNoTracking().Where(t => t.SYSTEM_ID == idTipoAttoAsLong).Select(t => t.GG_SCADENZA).FirstOrDefaultAsync();
                if (scadenza != null && scadenza != 0)
                    dtaScadenza = (await this._dbContext.GetSystemDateTime()).AddDays((double)scadenza).Date;
            }

            if (!string.IsNullOrEmpty(request.DataScadenza))
            {
                dtaScadenza = request.DataScadenza.AsDateTime().Date;
            }

            aggregate.AssignDataScadenza(dtaScadenza);

            await this._repository.Update(aggregate);

            return new ();
        }


        #region Private Members

        protected readonly ILogger<SalvaDataScadenzaDocCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IDocumentoAmministrativoRepository _repository;

        #endregion

    }
}
