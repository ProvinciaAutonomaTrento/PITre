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
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AggiornaPassoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.AggiornaPassoDiFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AggiornaPassoDiFirma
{
    public class AggiornaPassoDiFirmaHandler : IRequestHandler<AggiornaPassoDiFirmaRequest, AggiornaPassoDiFirmaResult>
    {
        #region Public Members

        public AggiornaPassoDiFirmaHandler(ILogger<AggiornaPassoDiFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AggiornaPassoDiFirmaResult> Handle(AggiornaPassoDiFirmaRequest request, CancellationToken cancellationToken)
        {
            var output = false;
            var passo = request.passo;
            IDbContextTransaction? transaction = null;

            try
            {
                transaction = await ((DbContext)_dbContext).Database.BeginTransactionAsync();

                var idProcessoAsLong = passo.idProcesso.AsLong();
                var idPassoAsLong = passo.idPasso.AsLong();
                var processoFirmaEntity = await this._dbContext.SchemaProcessoFirmaEntities.FirstAsync(p => p.ID_PROCESSO == idProcessoAsLong);
                var passoFirmaEntity = await this._dbContext.PassoDiFirmaEntities.FirstAsync(p => p.ID_PASSO == idPassoAsLong);
                var numeroSequenzaOld = passoFirmaEntity.NUMERO_SEQUENZA;

                passoFirmaEntity.NUMERO_SEQUENZA = Convert.ToInt64(passo.numeroSequenza);
                passoFirmaEntity.TIPO_FIRMA = passo.Evento.CodiceAzione;
                passoFirmaEntity.TIPO_EVENTO = await this._dbContext.AnagraficaEventiEntities.AsNoTracking().Where(a => a.VAR_COD_AZIONE == passo.Evento.CodiceAzione).Select(a => a.ID_EVENTO).FirstAsync();
                passoFirmaEntity.NOTE = passo.note;
                passoFirmaEntity.ID_TIPO_RUOLO_COINVOLTO = passo.TpoRuoloCoinvolto != null && !string.IsNullOrEmpty(passo.TpoRuoloCoinvolto.systemId) ? passo.TpoRuoloCoinvolto.systemId.AsLong() : null;
                passoFirmaEntity.ID_RUOLO_COINVOLTO = passo.ruoloCoinvolto != null && !string.IsNullOrEmpty(passo.ruoloCoinvolto.idGruppo) ? passo.ruoloCoinvolto.idGruppo.AsLong() : null;
                passoFirmaEntity.ID_UTENTE_COINVOLTO = passo.utenteCoinvolto != null && !string.IsNullOrEmpty(passo.utenteCoinvolto.idPeople) ? passo.utenteCoinvolto.idPeople.AsLong() : null;
                passoFirmaEntity.ID_AOO = !string.IsNullOrEmpty(passo.IdAOO) ? passo.IdAOO.AsLong() : null;
                passoFirmaEntity.ID_RF = !string.IsNullOrEmpty(passo.IdRF) ? passo.IdRF.AsLong() : null;
                passoFirmaEntity.ID_MAIL_REGISTRO = !string.IsNullOrEmpty(passo.IdMailRegistro) ? passo.IdMailRegistro.AsLong() : null;
                passoFirmaEntity.ID_TIPOLOGIA = !string.IsNullOrEmpty(passo.IdTipologia) ? passo.IdTipologia.AsLong() : null;
                passoFirmaEntity.ID_STATO_DIAGRAMMA = !string.IsNullOrEmpty(passo.IdStatoDiagramma) ? passo.IdStatoDiagramma.AsLong() : null;
                passoFirmaEntity.CHA_AUTOMATICO = passo.IsAutomatico ? "1" : "0";
                passoFirmaEntity.CHA_FACOLTATIVO = passo.IsFacoltativo ? "1" : "0";
                passoFirmaEntity.VAR_POS_SEGNATURA = passo.PosizioneSegnaturaPermanente;
                passoFirmaEntity.CHA_POS_SEGNATURA = passo.ApplicaSegnaturaPermanente;
                passoFirmaEntity.TICK = "0";

                //Controllo se è stato aggiornato il numero di sequenza, in tal caso occorre aggiornare il numero di sequenza degli altri passi
                if(passo.numeroSequenza != numeroSequenzaOld)
                {
                    if(passo.numeroSequenza > numeroSequenzaOld)
                    {
                        var passoDiFirmaEntities = await this._dbContext.PassoDiFirmaEntities
                            .Where(p => p.ID_PROCESSO == idProcessoAsLong && p.NUMERO_SEQUENZA > numeroSequenzaOld && p.NUMERO_SEQUENZA <= passo.numeroSequenza)
                            .Select(p => p)
                            .ToListAsync();

                        passoDiFirmaEntities.ForEach(p =>
                        {
                            p.NUMERO_SEQUENZA = p.NUMERO_SEQUENZA - 1;
                        });
                    }
                    else
                    {
                        var passoDiFirmaEntities = await this._dbContext.PassoDiFirmaEntities
                            .Where(p => p.ID_PROCESSO == idProcessoAsLong && p.NUMERO_SEQUENZA >= passo.numeroSequenza && p.NUMERO_SEQUENZA <= numeroSequenzaOld)
                            .Select(p => p)
                            .ToListAsync();

                        passoDiFirmaEntities.ForEach(p =>
                        {
                            p.NUMERO_SEQUENZA = p.NUMERO_SEQUENZA + 1;
                        });
                    }
                }

                //Aggiorno le opzioni di notifica del passo(rimuovo tutti i tipi eventi e li reinserisco)
                //var passoEventoEntitiesToRemove = await this._dbContext.PassoEventoEntities.Where(p => p.ID_PASSO == idPassoAsLong).Select(p => p).ToListAsync();
                //var passoEventoEntity = await this._dbContext.AnagraficaEventiEntities.AsNoTracking()
                //    .Where(e => passo.idEventiDaNotificare.Contains(e.GRUPPO))
                //    .Select(p => new PassoEventoEntity()
                //    {
                //        ID_EVENTO = p.ID_EVENTO,
                //        ID_PASSO = passoFirmaEntity.ID_PASSO
                //    })
                //    .ToListAsync();
                //this._dbContext.PassoEventoEntities.RemoveRange(passoEventoEntitiesToRemove);
                //await this._dbContext.PassoEventoEntities.AddRangeAsync(passoEventoEntity);

                string gruppi = string.Empty;
                await ((DbContext)this._dbContext).Database.ExecuteSqlAsync($"DELETE FROM DPA_PASSO_DPA_EVENTO WHERE ID_PASSO={idPassoAsLong};");
                foreach (string gruppo in passo.idEventiDaNotificare)
                {
                    await ((DbContext)this._dbContext).Database.ExecuteSqlAsync($"INSERT INTO DPA_PASSO_DPA_EVENTO (ID_PASSO, ID_EVENTO) SELECT {idPassoAsLong}, ID_EVENTO FROM DPA_ANAGRAFICA_EVENTI WHERE GRUPPO = {gruppo}");
                }

                await ((DbContext)_dbContext).SaveChangesAsync();

                var passoDiFirmaEventoEntities = await this._dbContext.PassoDiFirmaEntities
                   .Join(this._dbContext.AnagraficaEventiEntities.AsNoTracking(), passo => passo.TIPO_EVENTO, evento => evento.ID_EVENTO, (passo, evento) => new { passo, evento })
                   .Where(p => p.passo.ID_PROCESSO == idProcessoAsLong)
                   .Select(p => new PassoFirmaEventoEntity()
                   {
                       PassoFirma = p.passo,
                       Evento = p.evento
                   })
                   .ToListAsync();


                //Se il passo è invalidato e non esistono altri passi invalidati, valido il processo
                var tick = processoFirmaEntity.TICK;
                if (!passoDiFirmaEventoEntities.Any(p => p.PassoFirma.TICK != null && p.PassoFirma.TICK != "0"))
                {
                    tick = "0";
                }

                //Aggiorna il tipo processo di firma: 1 se modello di firma, 0 se processo di firma
                var isModello = false;
                foreach (var passoFirma in passoDiFirmaEventoEntities)
                {
                    if (!passoFirma.Evento.CHA_TIPO_EVENTO.Equals("W") && passoFirma.PassoFirma.ID_RUOLO_COINVOLTO == null)
                    {
                        isModello = true;
                        break;
                    }
                    if (passoFirma.PassoFirma.CHA_AUTOMATICO == "1")
                    {

                        if (passoFirma.Evento.VAR_COD_AZIONE.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) ||
                            passoFirma.Evento.VAR_COD_AZIONE.Equals(Azione.DOCUMENTO_REPERTORIATO.ToString()) ||
                            passoFirma.Evento.VAR_COD_AZIONE.Equals(Azione.RECORD_PREDISPOSED.ToString()))
                        {
                            if (passoFirma.PassoFirma.ID_AOO == null || passoFirma.PassoFirma.ID_RF == null)
                            {
                                isModello = true;
                                break;
                            }
                            if (passoFirma.Evento.VAR_COD_AZIONE.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) && passoFirma.PassoFirma.ID_MAIL_REGISTRO == null)
                            {
                                isModello = true;
                                break;
                            }
                        }
                        if (passoFirma.Evento.VAR_COD_AZIONE.Equals(Azione.DOC_CAMBIO_STATO.ToString()) && (passoFirma.PassoFirma.ID_TIPOLOGIA == null || passoFirma.PassoFirma.ID_STATO_DIAGRAMMA == null))
                        {
                            isModello = true;
                            break;
                        }
                    }
                    if (passoFirma.PassoFirma.CHA_FACOLTATIVO == "1")
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
                processoFirmaEntity.TICK = tick;

                await ((DbContext)_dbContext).SaveChangesAsync();

                await transaction.CommitAsync();

                output = true;

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                if (transaction != null)
                    await transaction.RollbackAsync();
                output = false;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }

            return new AggiornaPassoDiFirmaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AggiornaPassoDiFirmaHandler> _logger;
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
