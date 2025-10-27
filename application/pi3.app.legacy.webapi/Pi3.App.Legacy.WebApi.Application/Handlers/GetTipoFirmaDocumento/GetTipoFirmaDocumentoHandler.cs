// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
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
using GetTipoFirmaDocumentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetTipoFirmaDocumento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetTipoFirmaDocumento
{
    public class GetTipoFirmaDocumentoHandler : IRequestHandler<GetTipoFirmaDocumentoRequest, GetTipoFirmaDocumentoResult>
    {
        #region Public Members

        public GetTipoFirmaDocumentoHandler(ILogger<GetTipoFirmaDocumentoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetTipoFirmaDocumentoResult> Handle(GetTipoFirmaDocumentoRequest request, CancellationToken cancellationToken)
        {
            var output = TipoFirma.NESSUNA_FIRMA;

            try
            {
                var docnumber = request.docnumber.AsLong();

                var tipoFirma = await this._dbContext.ComponentEntities.AsNoTracking()
                    .Where(c => c.DOCNUMBER == docnumber && c.VERSION_ID == (this._dbContext.VersionEntities.AsNoTracking().Where(v => v.DOCNUMBER == docnumber).Max(v => v.VERSION_ID)))
                    .Select(c => new
                    {
                        c.CHA_FIRMATO,
                        c.CHA_TIPO_FIRMA,
                        c.VAR_NOMEORIGINALE
                    })
                    .FirstOrDefaultAsync();

                if(tipoFirma != null)
                {
                    output = !string.IsNullOrEmpty(tipoFirma.CHA_TIPO_FIRMA) ? tipoFirma.CHA_TIPO_FIRMA : TipoFirma.NESSUNA_FIRMA;
                    if(tipoFirma.CHA_FIRMATO == "1" && (output == TipoFirma.NESSUNA_FIRMA || output == TipoFirma.ELETTORNICA))
                    {
                        var fileName = tipoFirma.VAR_NOMEORIGINALE != null ? tipoFirma.VAR_NOMEORIGINALE : string.Empty;
                        if (!string.IsNullOrEmpty(fileName) && fileName.ToUpper().EndsWith("P7M"))
                            output = output.Equals(TipoFirma.ELETTORNICA) ? TipoFirma.CADES_ELETTORNICA : TipoFirma.CADES;
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new GetTipoFirmaDocumentoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetTipoFirmaDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
