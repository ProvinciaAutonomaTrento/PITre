// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneratePDFInSyncModRequest = Pi3.App.Legacy.WebApi.Application.Requests.GeneratePDFInSyncMod;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GeneratePDFInSyncMod
{
    public class GeneratePDFInSyncModHandler : IRequestHandler<GeneratePDFInSyncModRequest, GeneratePDFInSyncModResult>
    {
        #region Public Members

        public GeneratePDFInSyncModHandler(
            ILogger<GeneratePDFInSyncModHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IFileConverterFactory fileConverterFactory)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._fileConverterFactory = fileConverterFactory;
        }

        public async Task<GeneratePDFInSyncModResult> Handle(GeneratePDFInSyncModRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.documento.FileDocumento output = null!;

            try
            {
                var creation = await this._fileConverterFactory.TryCreate(request.docToConvert.name);

                if (creation.Success)
                {
                    var convert = await creation.Service.Convert(request.docToConvert.name, request.docToConvert.content, FileConverterOutputFormatsEnum.ToPdf);

                    output = request.docToConvert;
                    output.fullName = $"{Path.GetFileNameWithoutExtension(request.docToConvert.fullName)}.pdf";
                    output.name = $"{Path.GetFileNameWithoutExtension(request.docToConvert.name)}.pdf";
                    output.contentType = convert.ContentType;
                    output.content = convert.Content;
                    output.estensioneFile = "pdf";
                    output.nomeOriginale = $"{Path.GetFileNameWithoutExtension(request.docToConvert.nomeOriginale)}.pdf";
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                output = null!;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                output = null!;
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new GeneratePDFInSyncModResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GeneratePDFInSyncModHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IFileConverterFactory _fileConverterFactory;

        #endregion
    }
}