// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente.Repertori.RequestAndResponse;
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ChangeRepertorioState
{
    public class ChangeRepertorioStateHandler : IRequestHandler<Requests.ChangeRepertorioState, ChangeRepertorioStateResult>
    {
        #region Public members
        public ChangeRepertorioStateHandler(ILogger<ChangeRepertorioStateHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

        }
        public async Task<ChangeRepertorioStateResult> Handle(Requests.ChangeRepertorioState request, CancellationToken cancellationToken)
        {
            var output = new ChangeRepertorioStateResponse();

            var inEsercizio = false;

            var repertoriQueryable = this._dbContext.RegistriRepertorioEntities
                .Where(x => x.COUNTERID == request.request.CounterId.AsLong())
                .AsQueryable();

            if (string.IsNullOrWhiteSpace(request.request.RegistryId)) repertoriQueryable = repertoriQueryable.Where(x => x.REGISTRYID == null).AsQueryable();
            else repertoriQueryable = repertoriQueryable.Where(x => x.REGISTRYID == request.request.RegistryId.AsLong()).AsQueryable();

            if (string.IsNullOrWhiteSpace(request.request.RfId)) repertoriQueryable = repertoriQueryable.Where(x => x.RFID == null).AsQueryable();
            else repertoriQueryable = repertoriQueryable.Where(x=> x.RFID == request.request.RfId.AsLong()).AsQueryable();

            var repertorioEntity = await repertoriQueryable.FirstOrDefaultAsync();

            var tipoAttoEntity = await this._dbContext.TipoAttoEntities.FirstOrDefaultAsync(x => x.SYSTEM_ID == repertorioEntity.TIPOLOGYID);

            // Cambio dello stato di tutte le istanze del contatore su tutti i registri e gli RF
            if (repertorioEntity.COUNTERSTATE == "O")
            {
                await this.UpdateRegistriRepertorio(request.request.CounterId, "C");
                tipoAttoEntity.IN_ESERCIZIO = "NO";
            }
            else
            {
                await this.UpdateRegistriRepertorio(request.request.CounterId, "O");
                tipoAttoEntity.IN_ESERCIZIO = "SI";
            }

            var oggettiCustomEntities = await this._dbContext.OggettiCustomEntities
                .Join(this._dbContext.OggettiCustomCompEntities, a => a.SYSTEM_ID, b => b.ID_OGG_CUSTOM, (a, b) => new { a, b.ID_TEMPLATE })
                .Join(this._dbContext.TipoOggettoEntities, c => c.a.ID_TIPO_OGGETTO, d => d.SYSTEM_ID, (c, d) => new { c, d.DESCRIZIONE })
                .Where(x => x.c.ID_TEMPLATE == repertorioEntity.TIPOLOGYID
                && x.DESCRIZIONE!.ToLower() == "contatore")
                .Select(x => x.c.a)
                .ToListAsync();

            foreach (var oggettoCustomEntity in oggettiCustomEntities)
            {
                var contatoreCustomEntity = await this._dbContext.ContCustomDocEntities.FirstOrDefaultAsync(x => x.ID_OGG == oggettoCustomEntity.SYSTEM_ID);

                if (contatoreCustomEntity is not null) contatoreCustomEntity.SOSPESO = !inEsercizio ? "SI" : "NO";
            }

            await ((DbContext)this._dbContext).SaveChangesAsync();

            return new ChangeRepertorioStateResult(new ChangeRepertorioStateResponse { ChangeStateResult = true });
        }
        #endregion

        #region Private members
        protected ILogger<ChangeRepertorioStateHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;

        private async Task UpdateRegistriRepertorio(string counterId, string counterState)
        {
            await ((DbContext)this._dbContext).Database.ExecuteSqlAsync($"UPDATE DPA_REGISTRI_REPERTORIO SET COUNTERSTATE={counterState} WHERE COUNTERID={counterId}");
        }


        #endregion
    }
}
