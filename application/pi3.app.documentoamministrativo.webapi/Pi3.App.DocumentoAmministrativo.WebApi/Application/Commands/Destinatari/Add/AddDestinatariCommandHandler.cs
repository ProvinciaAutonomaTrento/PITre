// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Destinatari.Add
{
    public class AddDestinatariCommandHandler : IRequestHandler<AddDestinatariCommand, AddDestinatariCommandResponse>
    {
        private readonly IPi3DbContext _context;
        private readonly IDocumentoAmministrativoRepository _repository;
        private readonly IClaimsPrincipalService _claimsPrincipalService;

        public AddDestinatariCommandHandler(IPi3DbContext context,
            IDocumentoAmministrativoRepository repository,
            IClaimsPrincipalService claimsPrincipalService)
        {
            _context = context;
            _repository = repository;
            _claimsPrincipalService = claimsPrincipalService;

            InitializeMapper();
        }

        public async Task<AddDestinatariCommandResponse> Handle(AddDestinatariCommand request, CancellationToken cancellationToken)
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

            var idDestinatario = (await _context.CorrGlobaliEntities.Where(corr => corr.VAR_CODICE.ToUpper() == request.Codice.ToUpper() &&
                corr.DTA_FINE == null).FirstOrDefaultAsync()).SYSTEM_ID;

            var oggetto = new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Destinatario(idDestinatario.ToString());
            aggregate.AddDestinatario(oggetto);

            await _repository.Update(aggregate);

            return new AddDestinatariCommandResponse
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
