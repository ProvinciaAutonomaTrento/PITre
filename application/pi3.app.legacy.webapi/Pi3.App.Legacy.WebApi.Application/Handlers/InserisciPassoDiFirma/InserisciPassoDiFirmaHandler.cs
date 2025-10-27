// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DiagrammaStato;
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
using InserisciPassoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.InserisciPassoDiFirma;
using InsertPassoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.InsertPassoDiFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InserisciPassoDiFirma
{
    public class InserisciPassoDiFirmaHandler : IRequestHandler<InserisciPassoDiFirmaRequest, InserisciPassoDiFirmaResult>
    {
        #region Public Members

        public InserisciPassoDiFirmaHandler(ILogger<InserisciPassoDiFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<InserisciPassoDiFirmaResult> Handle(InserisciPassoDiFirmaRequest request, CancellationToken cancellationToken)
        {
            PassoFirma output = null;
            IDbContextTransaction? transaction = null;

            try
            {
                transaction = await ((DbContext)_dbContext).Database.BeginTransactionAsync();

                var idProcessoAsLong = request.passo.idProcesso.AsLong();
                output = (await this._mediator.Send(new InsertPassoDiFirmaRequest(request.passo, request.infoUtente))).output;

                //Aggiorna il tipo processo di firma: 1 se modello di firma, 0 se processo di firma
                var processoFirmaEntity = await this._dbContext.SchemaProcessoFirmaEntities.Where(p => p.ID_PROCESSO == idProcessoAsLong).FirstAsync();
                var passoDiFirmaEventoEntities = await this._dbContext.PassoDiFirmaEntities
                   .Join(this._dbContext.AnagraficaEventiEntities, passo => passo.TIPO_EVENTO, evento => evento.ID_EVENTO, (passo, evento) => new { passo, evento })
                   .Where(p => p.passo.ID_PROCESSO == idProcessoAsLong)
                   .Select(p => new PassoFirmaEventoEntity()
                   {
                       PassoFirma = p.passo,
                       Evento = p.evento
                   })
                   .ToListAsync();

                //Aggiorno il numero di sequenza dei passi successivi
                var passoDiFirmaEntities = passoDiFirmaEventoEntities
                    .Where(p => p.PassoFirma.NUMERO_SEQUENZA >= output.numeroSequenza && p.PassoFirma.ID_PASSO != output.idPasso.AsLong())
                    .Select(p => p.PassoFirma).ToList();
                foreach (var passo in passoDiFirmaEntities)
                {
                    passo.NUMERO_SEQUENZA = passo.NUMERO_SEQUENZA + 1;
                }

                var isModello = false;
                foreach (var passo in passoDiFirmaEventoEntities)
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
                    if (passo.PassoFirma.CHA_FACOLTATIVO == "1")
                    {
                        isModello = true;
                        break;
                    }
                }

                //Se non esiste un passo di tipo cambio stato automatico e tipologia selezionata, svuoto anche lo stato interruzione del processo(qualora esso è stato inserito)
                long? idStatoInterruzione = processoFirmaEntity.ID_STATO_INTERRUZIONE;
                if (!passoDiFirmaEventoEntities.Any(p => p.Evento.VAR_COD_AZIONE.Equals(Azione.DOC_CAMBIO_STATO.ToString()) && p.PassoFirma.ID_TIPOLOGIA != null))
                {
                    idStatoInterruzione = null;
                }
                processoFirmaEntity.CHA_MODELLO = isModello ? "1" : "0";
                processoFirmaEntity.ID_STATO_INTERRUZIONE = idStatoInterruzione;

                await ((DbContext)_dbContext).SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;

                if (transaction != null)
                    await transaction.RollbackAsync();
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }

            return new InserisciPassoDiFirmaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InserisciPassoDiFirmaHandler> _logger;
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
