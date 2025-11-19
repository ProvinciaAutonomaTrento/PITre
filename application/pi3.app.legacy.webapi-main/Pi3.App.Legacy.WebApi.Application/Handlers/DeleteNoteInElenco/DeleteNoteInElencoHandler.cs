// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.ModNotaInElenco;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.NotaRFAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaRFAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DeleteNoteInElenco
{

    // Richiede libreria MediatR
    public class DeleteNoteInElencoHandler : IRequestHandler<Application.Requests.DeleteNoteInElenco, DeleteNoteInElencoResult>
    {
        #region Public Members

        public DeleteNoteInElencoHandler(ILogger<DeleteNoteInElencoHandler> logger,
           IClaimsPrincipalService claimsPrincipalService,
           IMediator mediator,
           IPi3DbContext dbContext,
           INotaRFRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
        }

        public async Task<DeleteNoteInElencoResult> Handle(Application.Requests.DeleteNoteInElenco request, CancellationToken cancellationToken)
        {
            var output = true;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            try
            {
                foreach (DocsPaVO.Note.NotaElenco nota in request.listaNote)
                {
                    var aggregate = await this._repository.Get(idTenant, nota.idNota);

                    if (aggregate == null)
                        throw new NoteNotFoundPi3Exception(nota.idNota);

                    await this._repository.Delete(aggregate);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new DeleteNoteInElencoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DeleteNoteInElencoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly INotaRFRepository _repository;

        #endregion
    }

}
