// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using InsertVisibilitaProcessoRequest = Pi3.App.Legacy.WebApi.Application.Requests.InsertVisibilitaProcesso;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InsertVisibilitaProcesso
{

    public class InsertVisibilitaProcessoHandler : IRequestHandler<InsertVisibilitaProcessoRequest, InsertVisibilitaProcessoResult>
    {
        #region Public Members

        public InsertVisibilitaProcessoHandler(ILogger<InsertVisibilitaProcessoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<InsertVisibilitaProcessoResult> Handle(InsertVisibilitaProcessoRequest request, CancellationToken cancellationToken)
        {
            var output = false;

            try
            {
                if(request.visibilita != null && request.visibilita.Any())
                {
                    List<ProcessoFirmaVisibilitaEntity> processoFirmaVisibilitaEntities = new List<ProcessoFirmaVisibilitaEntity>();
                    var sysdate = await this._dbContext.GetSystemDateTime();
                    foreach(var visibilita in request.visibilita)
                    {
                        var idProcesso = visibilita.IdProcesso.AsLong();
                        var idGruppo = visibilita.Ruolo.idGruppo.AsLong();

                        if (!await this._dbContext.ProcessoFirmaVisibilitaEntities.AnyAsync(v => v.ID_PROCESSO == idProcesso && v.ID_GROUPS == idGruppo && !v.DTA_FINE.HasValue))
                        {
                            processoFirmaVisibilitaEntities.Add(new ProcessoFirmaVisibilitaEntity()
                            {
                                ID_PROCESSO = idProcesso,
                                ID_GROUPS = idGruppo,
                                CHA_TIPO_VISIBILITA = ((char)visibilita.TipoVisibilita).ToString(),
                                CHA_NOTIFICA_CONCLUSO = visibilita.Notifica.Notifica_concluso ? "1" : "0",
                                CHA_NOTIFICA_INTERROTTO = visibilita.Notifica.Notifica_interrotto ? "1" : "0",
                                CHA_NOTIFICA_ERRORE = visibilita.Notifica.NotificaErrore ? "1" : "0",
                                DTA_INIZIO = sysdate
                            });
                        }
                    }

                    await this._dbContext.ProcessoFirmaVisibilitaEntities.AddRangeAsync(processoFirmaVisibilitaEntities);
                    await ((DbContext)_dbContext).SaveChangesAsync();
                }

                output = true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new InsertVisibilitaProcessoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InsertVisibilitaProcessoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
