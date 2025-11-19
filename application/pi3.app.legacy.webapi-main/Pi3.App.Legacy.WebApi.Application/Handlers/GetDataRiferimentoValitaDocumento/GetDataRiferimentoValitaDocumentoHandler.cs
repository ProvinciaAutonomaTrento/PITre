// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
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
using getDettaglioNoSecurityRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetDettaglioDocumentoNoSecurity;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetDataRiferimentoValitaDocumento
{

    // Richiede libreria MediatR
    public class GetDataRiferimentoValitaDocumentoHandler : IRequestHandler<Application.Requests.GetDataRiferimentoValitaDocumento, GetDataRiferimentoValitaDocumentoResult>
    {
        #region Public Members

        public GetDataRiferimentoValitaDocumentoHandler(ILogger<GetDataRiferimentoValitaDocumentoHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, IPi3DbContext dbContext, IDistributedCache distributedCache, IConfiguration configuration)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._distributedCache = distributedCache;
        }


        public async Task<GetDataRiferimentoValitaDocumentoResult> Handle(Application.Requests.GetDataRiferimentoValitaDocumento request, CancellationToken cancellationToken)
        {
            DateTime? output = null;

            try
            {
                var versionId = request.fileRequest.versionId.AsLong();
                var docnumber = request.fileRequest.docNumber.AsLong();

                var timestampDocEntities = await this._dbContext.TimestampDocEntities.AsNoTracking()
                    .Where(t => t.VERSION_ID == versionId && t.DOC_NUMBER == docnumber)
                    .OrderBy(t => t.DTA_CREAZIONE)
                    .FirstOrDefaultAsync();

                if (timestampDocEntities != null && timestampDocEntities.DTA_CREAZIONE != null)
                {
                    output = (DateTime)timestampDocEntities.DTA_CREAZIONE;
                }
                else
                {
                    var profileEntity = await this._dbContext.ProfileEntities.AsNoTracking()
                        .Where(p => p.DOCNUMBER == docnumber)
                        .Select(p => new
                        {
                            p.DTA_PROTO,
                            p.VAR_SEGNATURA,
                            p.CREATION_DATE
                        })
                        .FirstOrDefaultAsync();

                    if (profileEntity != null)
                    {
                        if (!string.IsNullOrEmpty(profileEntity.VAR_SEGNATURA))
                        {
                            output = (DateTime)profileEntity.DTA_PROTO;
                        }

                        if(output == null)
                        {
                            var dataRepertoriazione = await _dbContext.AssociazioneTemplatesEntities.AsNoTracking()
                                .Join(_dbContext.OggettiCustomEntities, a => a.ID_OGGETTO, c => c.SYSTEM_ID, (a, c) => new { a, c })
                                .Where(w => w.a.DOC_NUMBER == request.fileRequest.docNumber && w.c.REPERTORIO == 1 && (w.c.CAMPO_COMUNE ?? 0) == 0)
                                .Select(s => s.a.DTA_INS)
                                .FirstOrDefaultAsync();

                            if (dataRepertoriazione != null)
                            {
                                output = (DateTime)dataRepertoriazione;
                            }
                        }
                        if(output == null)
                        {
                            output = profileEntity.CREATION_DATE;
                        }
                    }
                }

                if (output == null)
                    output = DateTime.Now.Date;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = DateTime.Now.Date;
            }

            return new GetDataRiferimentoValitaDocumentoResult((DateTime)output);         
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDataRiferimentoValitaDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;

        #endregion
    }

}
