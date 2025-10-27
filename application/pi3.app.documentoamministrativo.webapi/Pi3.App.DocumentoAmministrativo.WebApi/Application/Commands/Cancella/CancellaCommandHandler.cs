// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Cancella
{
    public class CancellaCommandHandler : IRequestHandler<CancellaCommand, CancellaCommandResponse>
    {
        private readonly IPi3DbContext _context;
        private readonly IDocumentoAmministrativoRepository _repository;
        private readonly IClaimsPrincipalService _claimsPrincipalService;

        public CancellaCommandHandler(IPi3DbContext context,
            IDocumentoAmministrativoRepository repository,
            IClaimsPrincipalService claimsPrincipalService) {
            this._context = context;
            this._repository = repository;
            this._claimsPrincipalService = claimsPrincipalService;

            InitializeMapper();
        }

        public async Task<CancellaCommandResponse> Handle(CancellaCommand request, CancellationToken cancellationToken)
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

            aggregate.Recycle(new TextValue(request.Note.NoteCancellazione));

            await _repository.Update(aggregate);


            return new CancellaCommandResponse
            {
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
