// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Annullamento
{
    public class AnnullamentoCommandHandler : IRequestHandler<AnnullamentoCommand, AnnullamentoCommandResponse>
    {
        private readonly IPi3DbContext _context;
        private readonly IDocumentoAmministrativoRepository _repository;
        private readonly IClaimsPrincipalService _claimsPrincipalService;

        public AnnullamentoCommandHandler(IPi3DbContext context,
            IDocumentoAmministrativoRepository repository,
            IClaimsPrincipalService claimsPrincipalService) {
            this._context = context;
            this._repository = repository;
            this._claimsPrincipalService = claimsPrincipalService;

            InitializeMapper();
        }

        public async Task<AnnullamentoCommandResponse> Handle(AnnullamentoCommand request, CancellationToken cancellationToken)
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

            aggregate.Annulla( new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Annullamento() { 
                Autore = new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Autore(request.IdAutore),
                Data = request.Data,
                Motivo = new TextValue(request.Motivo)
            });
            await _repository.Update(aggregate);

            return new AnnullamentoCommandResponse
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
