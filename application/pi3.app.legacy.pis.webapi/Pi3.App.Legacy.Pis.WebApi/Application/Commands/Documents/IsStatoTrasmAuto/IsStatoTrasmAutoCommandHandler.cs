// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using DocsPaVO.Modelli_Trasmissioni;
using Pi3.Core.Extensions;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetModelloByID;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.IsStatoTrasmAuto
{
    public class IsStatoTrasmAutoCommandHandler : IRequestHandler<IsStatoTrasmAutoCommand, IsStatoTrasmAutoCommandResponse>
    {
        public IsStatoTrasmAutoCommandHandler(ILogger<IsStatoTrasmAutoCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<IsStatoTrasmAutoCommandResponse> Handle(IsStatoTrasmAutoCommand request, CancellationToken cancellationToken)
        {
            ModelloTrasmissione[] output = null;

            try
            {
                if (!string.IsNullOrEmpty(request.IdStato))
                {
                    long idStato = request.IdStato.AsLong();
                    long idTemplate = request.IdTemplate.AsLong();
                    long idAmm = request.IdAmm.AsLong();

                    var assDiagrammiEntities = await this._dbContext.AssDiagrammiEntities.Where(a => a.ID_STATO == idStato && a.ID_TIPO_DOC == idTemplate).Select(a => new { a.ID_MOD_TRASM, a.TRASM_AUT }).ToListAsync();
                    if (assDiagrammiEntities != null && assDiagrammiEntities.Count > 0)
                    {
                        List<ModelloTrasmissione> modelli = new List<ModelloTrasmissione>();
                        foreach (var a in assDiagrammiEntities)
                        {
                            if (a.TRASM_AUT == 1)
                            {
                                var modello = await this._mediator.Send(new GetModelloByIDCommand()
                                {
                                    IdAmm = request.IdAmm,
                                    IdModello = a.ID_MOD_TRASM.ToString()
                                });
                                modelli.Add(modello.Output);

                            }
                        }
                        output = modelli.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new()
            {
                Output = output
            };


        }

        #region Private Members


        protected readonly ILogger<IsStatoTrasmAutoCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        #endregion
    }
}
