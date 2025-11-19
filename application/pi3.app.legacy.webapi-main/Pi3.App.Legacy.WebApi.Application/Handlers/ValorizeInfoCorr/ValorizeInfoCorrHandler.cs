// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Smistamento;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ValorizeInfoCorrRequest = Pi3.App.Legacy.WebApi.Application.Requests.ValorizeInfoCorr;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ValorizeInfoCorr
{
    public class ValorizeInfoCorrHandler : IRequestHandler<ValorizeInfoCorrRequest, ValorizeInfoCorrResult>
    {
        #region Public Members

        public ValorizeInfoCorrHandler(ILogger<ValorizeInfoCorrHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<ValorizeInfoCorrResult> Handle(ValorizeInfoCorrRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var dettagliCorr = new DocsPaVO.addressbook.DettagliCorrispondente();

                dettagliCorr.Corrispondente.AddCorrispondenteRow(
                    request.address,
                    request.city,
                    request.zipCode,
                    request.district,
                    request.country,
                    request.phone,
                    request.phone2,
                    request.fax,
                    request.taxId.Trim(),
                    request.note,
                    request.place,
                    request.birthPlace,
                    request.birthDay,
                    request.title,
                    request.commercialId.Trim());

                request.corr.info = dettagliCorr;
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(pi3Ex, pi3Ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, ex.Message);
            }

            return new ValorizeInfoCorrResult(request.corr);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ValorizeInfoCorrHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}