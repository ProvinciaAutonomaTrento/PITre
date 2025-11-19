// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Destinatari
{
    public class AddDestinatariCCCommandHandler : IRequestHandler<AddDestinatariCCCommand, AddDestinatariCCCommandResponse>
    {
        private readonly IPi3DbContext _context;
        private readonly IDocumentoAmministrativoRepository _repository;
        private readonly IClaimsPrincipalService _claimsPrincipalService;

        public AddDestinatariCCCommandHandler(IPi3DbContext context,
            IDocumentoAmministrativoRepository repository,
            IClaimsPrincipalService claimsPrincipalService) {
            this._context = context;
            this._repository = repository;
            this._claimsPrincipalService = claimsPrincipalService;

            InitializeMapper();
        }

        public async Task<AddDestinatariCCCommandResponse> Handle(AddDestinatariCCCommand request, CancellationToken cancellationToken)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            var aggregate = await _repository.Get(idTenant, request.Id,
                new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
                        LoadClassifications = true,
                        LoadAllegati = true,
                        LoadAggregazioni = true,
                        LoadVersions = true,
                        LoadPermissions = true,
                        LoadMittentiDestinatari = true
                    }
                });

            var idDestinatario = (await this._context.CorrGlobaliEntities.Where(corr => corr.VAR_CODICE.ToUpper() == request.Codice.ToUpper() &&
                corr.DTA_FINE == null).FirstOrDefaultAsync()).SYSTEM_ID;

            var oggetto = new Destinatario(idDestinatario.ToString());
            aggregate.AddDestinatarioCc(oggetto);

            await _repository.Update(aggregate);

            return new AddDestinatariCCCommandResponse
            {
                DocumentoAmministrativo = _mapper.Map<Documento>(aggregate)
            };

        }

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddDocumentoMapping();
            });
            _mapper = configuration.CreateMapper();

        }

    }
}
