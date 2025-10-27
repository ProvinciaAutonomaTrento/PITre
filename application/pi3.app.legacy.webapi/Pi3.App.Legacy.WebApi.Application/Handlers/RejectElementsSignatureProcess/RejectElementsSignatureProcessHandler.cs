// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RejectElementsSignatureProcessRequest = Pi3.App.Legacy.WebApi.Application.Requests.RejectElementsSignatureProcess;
using DocsPaVO.DiagrammaStato;
using System.Collections;
using DocsPaVO.utente;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RejectElementsSignatureProcess
{
    public class RejectElementsSignatureProcessHandler : IRequestHandler<RejectElementsSignatureProcessRequest, RejectElementsSignatureProcessResult>
    {
        #region Public Members

        public RejectElementsSignatureProcessHandler(ILogger<RejectElementsSignatureProcessHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<RejectElementsSignatureProcessResult> Handle(RejectElementsSignatureProcessRequest request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var elemento in request.elements)
                {
                    await this._mediator.Send(new Application.Requests.InterruzioneProcessoFirma(elemento.InfoDocumento.Docnumber, elemento.MotivoRespingimento, "T", request.infoUtente));

                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new RejectElementsSignatureProcessResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RejectElementsSignatureProcessHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        #endregion
    }
}
