// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.Spedizione;
using DocsPaVO.trasmissione;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetReportSpedizioniDocumentiRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetReportSpedizioniDocumenti;
using GetReportSpedizioniSearchRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetReportSpedizioniSearch;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetReportSpedizioniDocumenti
{
    public class GetReportSpedizioniDocumentiHandler : IRequestHandler<GetReportSpedizioniDocumentiRequest, GetReportSpedizioniDocumentiResult>
    {
        #region Public Members

        public GetReportSpedizioniDocumentiHandler(ILogger<GetReportSpedizioniDocumentiHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetReportSpedizioniDocumentiResult> Handle(GetReportSpedizioniDocumentiRequest request, CancellationToken cancellationToken)
        {
            InfoDocumentoSpedito[] output = null;

            try
            {
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

                if (request.idDocumenti == null || request.idDocumenti.Count() == 0)
                    throw new ListaIdDocumentiVuotaPi3Exception();
                request.filters.idDocumenti = request.idDocumenti.ToList();
                output = (await this._mediator.Send(new GetReportSpedizioniSearchRequest(request.filters, idGroup.ToString(), false))).output;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new GetReportSpedizioniDocumentiResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetReportSpedizioniDocumentiHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
