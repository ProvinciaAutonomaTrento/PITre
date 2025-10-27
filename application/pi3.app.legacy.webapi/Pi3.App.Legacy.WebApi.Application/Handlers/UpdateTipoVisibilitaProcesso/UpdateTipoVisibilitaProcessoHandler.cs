// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateTipoVisibilitaProcessoRequest = Pi3.App.Legacy.WebApi.Application.Requests.UpdateTipoVisibilitaProcesso;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UpdateTipoVisibilitaProcesso
{
    public class UpdateTipoVisibilitaProcessoHandler : IRequestHandler<UpdateTipoVisibilitaProcessoRequest, UpdateTipoVisibilitaProcessoResult>
    {
        #region Public Members

        public UpdateTipoVisibilitaProcessoHandler(ILogger<UpdateTipoVisibilitaProcessoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<UpdateTipoVisibilitaProcessoResult> Handle(UpdateTipoVisibilitaProcessoRequest request, CancellationToken cancellationToken)
        {
            var output = false;

            try
            {
                var idProcesso = request.visibilita.IdProcesso.AsLong();
                var idGruppo = request.visibilita.Ruolo.idGruppo.AsLong();

                var visibilita = ((char)request.visibilita.TipoVisibilita).ToString();
                var concluso = request.visibilita.Notifica.Notifica_concluso ? "1" : "0";
                var interrotto = request.visibilita.Notifica.Notifica_interrotto ? "1" : "0";
                var errore = request.visibilita.Notifica.NotificaErrore ? "1" : "0";

                var rowsAffected = await ((DbContext)this._dbContext).Database.ExecuteSqlAsync($"UPDATE DPA_PROCESSO_FIRMA_VISIBILITA SET CHA_TIPO_VISIBILITA= {visibilita}, CHA_NOTIFICA_CONCLUSO = {concluso}, CHA_NOTIFICA_INTERROTTO = {interrotto}, CHA_NOTIFICA_ERRORE = {errore} WHERE ID_PROCESSO = {idProcesso} AND ID_GROUPS = {idGruppo} AND DTA_FINE IS NULL");

                //var processoVisibilitaFirmaEntity = await this._dbContext.ProcessoFirmaVisibilitaEntities
                //    .Where(p => p.ID_PROCESSO == idProcesso && p.ID_GROUPS == idGruppo && !p.DTA_FINE.HasValue)
                //    .FirstAsync();
                //processoVisibilitaFirmaEntity.CHA_TIPO_VISIBILITA = ((char)request.visibilita.TipoVisibilita).ToString();
                //processoVisibilitaFirmaEntity.CHA_NOTIFICA_CONCLUSO = request.visibilita.Notifica.Notifica_concluso ? "1" : "0";
                //processoVisibilitaFirmaEntity.CHA_NOTIFICA_INTERROTTO = request.visibilita.Notifica.Notifica_interrotto ? "1" : "0";
                //processoVisibilitaFirmaEntity.CHA_NOTIFICA_ERRORE = request.visibilita.Notifica.NotificaErrore ? "1" : "0";

                //await ((DbContext)_dbContext).SaveChangesAsync();

                output = rowsAffected != 0;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new UpdateTipoVisibilitaProcessoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UpdateTipoVisibilitaProcessoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
