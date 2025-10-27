// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.areaConservazione;
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.AggiornaEsecuzioneElementoInLibroFirma;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using PutElectronicSignatureMassiveRequest = Pi3.App.Legacy.WebApi.Application.Requests.PutElectronicSignatureMassive;
using PutElectronicSignatureRequest = Pi3.App.Legacy.WebApi.Application.Requests.PutElectronicSignature;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.PutElectronicSignatureMassive
{
    public class PutElectronicSignatureMassiveHandler : IRequestHandler<PutElectronicSignatureMassiveRequest, PutElectronicSignatureMassiveResult>
    {
        #region Public Members

        public PutElectronicSignatureMassiveHandler(ILogger<PutElectronicSignatureMassiveHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<PutElectronicSignatureMassiveResult> Handle(PutElectronicSignatureMassiveRequest request, CancellationToken cancellationToken)
        {
            FirmaResult[] output = null;

            try
            {
                List<FirmaResult> listResult = new List<FirmaResult>();

                foreach(var file in request.approvingFiles)
                {
                    var result = await this._mediator.Send(new PutElectronicSignatureRequest(file, request.infoUtente, request.isAdvancementProcess));
                    listResult.Add(new FirmaResult()
                    {
                        fileRequest = file,
                        errore = result.message
                    });
                }

                output = listResult.ToArray();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = null;
            }

            return new PutElectronicSignatureMassiveResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<PutElectronicSignatureMassiveHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
