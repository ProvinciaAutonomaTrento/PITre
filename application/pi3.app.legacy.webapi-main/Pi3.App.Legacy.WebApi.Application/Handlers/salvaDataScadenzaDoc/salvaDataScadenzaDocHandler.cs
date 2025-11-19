// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using salvaDataScadenzaDocRequest = Pi3.App.Legacy.WebApi.Application.Requests.salvaDataScadenzaDoc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.salvaDataScadenzaDoc
{

    public class salvaDataScadenzaDocHandler : IRequestHandler<salvaDataScadenzaDocRequest, salvaDataScadenzaDocResult>
    {
        #region Public Members

        public salvaDataScadenzaDocHandler(ILogger<salvaDataScadenzaDocHandler> logger, 
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

        public async Task<salvaDataScadenzaDocResult> Handle(salvaDataScadenzaDocRequest request, CancellationToken cancellationToken)
        {
            string idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            var aggregate = await this._repository.Get(idTenant, request.docNumber, new ILoadBehavior[1]
            {
                new GetDocumentoAmministrativoLoadBehavior()
                {
                    LoadProfiles = false,
                    LoadClassifications = false,
                    LoadAllegati = false,
                    LoadAggregazioni = false,
                    LoadVersions = false,
                    LoadPermissions = false,
                    LoadMittentiDestinatari = true,
                    LoadRelatedElements = false            
                }
            });

            DateTime? dtaScadenza = null;

            if(string.IsNullOrEmpty(request.dataScadenza))
            {
                var idTipoAttoAsLong = request.idTipoAtto.AsLong();
                var scadenza = await this._dbContext.TipoAttoEntities.AsNoTracking().Where(t => t.SYSTEM_ID == idTipoAttoAsLong).Select(t => t.GG_SCADENZA).FirstOrDefaultAsync();
                if (scadenza != null && scadenza != 0)
                    dtaScadenza = (await this._dbContext.GetSystemDateTime()).AddDays((double)scadenza).Date;
            }
            
            if(!string.IsNullOrEmpty(request.dataScadenza))
            {
                dtaScadenza = request.dataScadenza.AsDateTime().Date;
            }

            aggregate.AssignDataScadenza(dtaScadenza);

            await this._repository.Update(aggregate);

            return new salvaDataScadenzaDocResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<salvaDataScadenzaDocHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IDocumentoAmministrativoRepository _repository;

        #endregion
    }
}
