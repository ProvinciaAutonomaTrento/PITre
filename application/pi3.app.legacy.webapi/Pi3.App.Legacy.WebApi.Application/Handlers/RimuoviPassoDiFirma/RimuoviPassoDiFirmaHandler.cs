// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.LibroFirma;
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
using RimuoviPassoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.RimuoviPassoDiFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RimuoviPassoDiFirma
{
    public class RimuoviPassoDiFirmaHandler : IRequestHandler<RimuoviPassoDiFirmaRequest, RimuoviPassoDiFirmaResult>
    {
        #region Public Members

        public RimuoviPassoDiFirmaHandler(ILogger<RimuoviPassoDiFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<RimuoviPassoDiFirmaResult> Handle(RimuoviPassoDiFirmaRequest request, CancellationToken cancellationToken)
        {
            var output = false;

            try
            {
                var idPasso = request.passo.idPasso.AsLong();
                var idProcesso = request.passo.idProcesso.AsLong();

                var processoFirmaEntity = await this._dbContext.SchemaProcessoFirmaEntities
                    .Where(p => p.ID_PROCESSO == idProcesso)
                    .FirstAsync();

                var passoDiFirmaEventoEntities = await this._dbContext.PassoDiFirmaEntities
                    .Join(this._dbContext.AnagraficaEventiEntities, passo => passo.TIPO_EVENTO, evento => evento.ID_EVENTO, (passo, evento) => new { passo, evento })
                    .Where(p => p.passo.ID_PROCESSO == idProcesso)
                    .Select(p => new PassoFirmaEventoEntity()
                    {
                        PassoFirma = p.passo,
                        Evento = p.evento
                    })
                    .ToListAsync();

                var passoDiFirmaEntities = passoDiFirmaEventoEntities.Select(p => p.PassoFirma).ToList();

                var passoFirmaToRemoveEntity = passoDiFirmaEntities.First(p => p.ID_PASSO == idPasso);

                //var passoEventoEntities = await this._dbContext.PassoEventoEntities
                //    .Where(p => p.ID_PASSO == idPasso)
                //    .ToListAsync();

                //Aggiorno il numero di sequenza dei passi successivi
                foreach(var passo in passoDiFirmaEntities.Where(p => p.NUMERO_SEQUENZA > passoFirmaToRemoveEntity.NUMERO_SEQUENZA))
                {
                    passo.NUMERO_SEQUENZA = passo.NUMERO_SEQUENZA - 1;
                }

                //Se il passo è invalidato e non esistono altri passi invalidati, valido il processo
                var tick = processoFirmaEntity.TICK;
                if (request.passo.Invalidated != 0 && !passoDiFirmaEntities.Any(p => p.ID_PASSO != idPasso && p.TICK != null && p.TICK != "0"))
                {
                    tick = "0";
                }

                //Aggiorna il tipo processo di firma: 1 se modello di firma, 0 se processo di firma
                var isModello = false;
                foreach(var passo in passoDiFirmaEventoEntities.Where(p => p.PassoFirma.ID_PASSO != idPasso))
                {
                    if (!passo.Evento.CHA_TIPO_EVENTO.Equals("W") && passo.PassoFirma.ID_RUOLO_COINVOLTO == null)
                    {
                        isModello = true;
                        break;
                    }
                    if (passo.PassoFirma.CHA_AUTOMATICO == "1")
                    {

                        if (passo.Evento.VAR_COD_AZIONE.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) ||
                            passo.Evento.VAR_COD_AZIONE.Equals(Azione.DOCUMENTO_REPERTORIATO.ToString()) ||
                            passo.Evento.VAR_COD_AZIONE.Equals(Azione.RECORD_PREDISPOSED.ToString()))
                        {
                            if (passo.PassoFirma.ID_AOO == null || passo.PassoFirma.ID_RF == null)
                            {
                                isModello = true;
                                break;
                            }
                            if (passo.Evento.VAR_COD_AZIONE.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) && passo.PassoFirma.ID_MAIL_REGISTRO == null)
                            {
                                isModello = true;
                                break;
                            }
                        }
                        if (passo.Evento.VAR_COD_AZIONE.Equals(Azione.DOC_CAMBIO_STATO.ToString()) && (passo.PassoFirma.ID_TIPOLOGIA == null || passo.PassoFirma.ID_STATO_DIAGRAMMA == null))
                        {
                            isModello = true;
                            break;
                        }
                    }
                    if(passo.PassoFirma.CHA_FACOLTATIVO == "1")
                    {
                        isModello = true;
                        break;
                    }
                }

                //Se non esiste un passo di tipo cambio stato automatico e tipologia selezionata, svuoto anche lo stato interruzione del processo(qualora esso è stato inserito)
                long? idStatoInterruzione = processoFirmaEntity.ID_STATO_INTERRUZIONE;
                if(!passoDiFirmaEventoEntities.Any(p => p.PassoFirma.ID_PASSO != idPasso && p.Evento.VAR_COD_AZIONE.Equals(Azione.DOC_CAMBIO_STATO.ToString()) && p.PassoFirma.ID_TIPOLOGIA != null))
                {
                    idStatoInterruzione = null;
                }
                processoFirmaEntity.CHA_MODELLO = isModello ? "1" : "0";
                processoFirmaEntity.ID_STATO_INTERRUZIONE = idStatoInterruzione;
                processoFirmaEntity.TICK = tick;

                this._dbContext.PassoDiFirmaEntities.Remove(passoFirmaToRemoveEntity);
                await ((DbContext)this._dbContext).Database.ExecuteSqlAsync($"DELETE FROM DPA_PASSO_DPA_EVENTO WHERE ID_PASSO={idPasso}");
                //this._dbContext.PassoEventoEntities.RemoveRange(passoEventoEntities);

                await ((DbContext)_dbContext).SaveChangesAsync();

                output = true;
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new RimuoviPassoDiFirmaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RimuoviPassoDiFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected class PassoFirmaEventoEntity
        {
            public PassoDiFirmaEntity PassoFirma { get; set; }
            public AnagraficaEventiEntity Evento { get; set; }
        }

        #endregion
    }
}
