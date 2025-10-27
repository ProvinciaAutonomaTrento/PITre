// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Spedizione;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetReportSpedizioniRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetReportSpedizioni;
using GetReportSpedizioniSearchRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetReportSpedizioniSearch;
using DocsPaVO.DiagrammaStato;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetReportSpedizioni
{
    public class GetReportSpedizioniHandler : IRequestHandler<GetReportSpedizioniRequest, GetReportSpedizioniResult>
    {
        #region Public Members

        public GetReportSpedizioniHandler(ILogger<GetReportSpedizioniHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetReportSpedizioniResult> Handle(GetReportSpedizioniRequest request, CancellationToken cancellationToken)
        {
            InfoDocumentoSpedito[] output = null;

            try
            {
                FiltriReportSpedizioni filter = request.filters;
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

                if (!string.IsNullOrEmpty(request.filters.IdDocumento))
                {
                    filter.idDocumenti = new List<string> { request.filters.IdDocumento };
                    output = (await this._mediator.Send(new GetReportSpedizioniSearchRequest(request.filters, string.Empty, true))).output;
                }
                else
                {
                    output = (await this._mediator.Send(new GetReportSpedizioniSearchRequest(request.filters, idGroup.ToString(), true))).output;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new GetReportSpedizioniResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetReportSpedizioniHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
