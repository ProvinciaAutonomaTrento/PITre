// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuplicaProcessoFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.DuplicaProcessoFirma;
using InsertProcessoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.InsertProcessoDiFirma;
using GetProcessoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetProcessoDiFirma;
using DocsPaVO.DiagrammaStato;
using Pi3.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DuplicaProcessoFirma
{

    public class DuplicaProcessoFirmaHandler : IRequestHandler<DuplicaProcessoFirmaRequest, DuplicaProcessoFirmaResult>
    {
        #region Public Members

        public DuplicaProcessoFirmaHandler(ILogger<DuplicaProcessoFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DuplicaProcessoFirmaResult> Handle(DuplicaProcessoFirmaRequest request, CancellationToken cancellationToken)
        {
            ResultProcessoFirma resultCreazioneProcesso = ResultProcessoFirma.KO;
            ProcessoFirma output = null;
            IDbContextTransaction? transaction = null;

            try
            {
                transaction = await ((DbContext)_dbContext).Database.BeginTransactionAsync();

                output = new ProcessoFirma()
                {
                    nome = request.nomeNuovoProcesso,
                    IsProcessModel = request.processoOld.IsProcessModel,
                    isInvalidated = request.processoOld.isInvalidated,
                };
                var insertProcessoDiFirmaResult = (await this._mediator.Send(new InsertProcessoDiFirmaRequest(output, request.utente)));
                resultCreazioneProcesso = insertProcessoDiFirmaResult.resultCreazioneProcesso;
                output = insertProcessoDiFirmaResult.output;
                if(output != null && resultCreazioneProcesso.Equals(ResultProcessoFirma.OK))
                {
                    var idProcessoOld = request.processoOld.idProcesso.AsLong();
                    var idProcessoNew = output.idProcesso.AsLong();

                    //Procedo con la creazione dei passi
                    var passiFirmaEntitiesToInsert = await this._dbContext.PassoDiFirmaEntities.AsNoTracking()
                        .Where(p => p.ID_PROCESSO == idProcessoOld)
                        .Select(p => new PassoDiFirmaEntity()
                        {
                            ID_PROCESSO = idProcessoNew,
                            NUMERO_SEQUENZA = p.NUMERO_SEQUENZA,
                            TIPO_FIRMA = p.TIPO_FIRMA,
                            TIPO_EVENTO = p.TIPO_EVENTO,
                            NOTE = p.NOTE,
                            ID_RUOLO_COINVOLTO = p.ID_RUOLO_COINVOLTO,
                            ID_UTENTE_COINVOLTO = p.ID_UTENTE_COINVOLTO,
                            SCADENZA = p.SCADENZA,
                            ELIMINATO = p.ELIMINATO,
                            TICK = p.TICK,
                            ID_TIPO_RUOLO_COINVOLTO = p.ID_TIPO_RUOLO_COINVOLTO,
                            CHA_AUTOMATICO = p.CHA_AUTOMATICO,
                            ID_AOO = p.ID_AOO,
                            ID_RF = p.ID_RF,
                            ID_MAIL_REGISTRO = p.ID_MAIL_REGISTRO,
                            CHA_FACOLTATIVO = p.CHA_FACOLTATIVO,
                            CHA_POS_SEGNATURA = p.CHA_POS_SEGNATURA,
                            VAR_POS_SEGNATURA = p.VAR_POS_SEGNATURA
                        })
                        .ToListAsync();

                    await this._dbContext.PassoDiFirmaEntities.AddRangeAsync(passiFirmaEntitiesToInsert);

                    //Procedo con la copia delle notifich dei passi
                    //var passoFirmaEventoEntitiesToInsert = passiFirmaEntitiesToInsert
                    //    .Join(this._dbContext.PassoDiFirmaEntities, passoNew => passoNew.NUMERO_SEQUENZA, passoOld => passoOld.NUMERO_SEQUENZA, (passoNew, passoOld) => new { passoNew, passoOld })
                    //    .Join(this._dbContext.PassoEventoEntities, j => j.passoOld.ID_PASSO, evento => evento.ID_PASSO, (j, evento) => new { j.passoOld, j.passoNew, evento })
                    //    .Where(j => j.passoOld.ID_PROCESSO == idProcessoOld && j.passoNew.ID_PROCESSO == idProcessoNew)
                    //    .Select(j => new PassoEventoEntity()
                    //    {
                    //        ID_EVENTO = j.evento.ID_EVENTO,
                    //        ID_PASSO = j.passoNew.ID_PASSO
                    //    })
                    //    .ToList();

                    //await this._dbContext.PassoEventoEntities.AddRangeAsync(passoFirmaEventoEntitiesToInsert);

                    await ((DbContext)this._dbContext).Database.ExecuteSqlAsync($"INSERT INTO DPA_PASSO_DPA_EVENTO (ID_PASSO, ID_EVENTO) SELECT P2.ID_PASSO, E.ID_EVENTO FROM DPA_PASSO_DI_FIRMA P1, DPA_PASSO_DI_FIRMA P2, DPA_PASSO_DPA_EVENTO E WHERE P1.ID_PROCESSO={idProcessoOld} AND P2.ID_PROCESSO = {idProcessoNew} AND E.ID_PASSO = P1.ID_PASSO AND P1.NUMERO_SEQUENZA = P2.NUMERO_SEQUENZA");

                    if (request.copiaVisibilita)
                    {
                        var dta_inizio = await this._dbContext.GetSystemDateTime();
                        var processoFirmaVisibilitaEntitiesToInsert = await this._dbContext.ProcessoFirmaVisibilitaEntities
                            .Where(v => v.ID_PROCESSO == idProcessoOld && v.DTA_FINE == null)
                            .Select(v => new ProcessoFirmaVisibilitaEntity()
                            {
                                ID_PROCESSO = idProcessoNew,
                                ID_GROUPS = v.ID_GROUPS,
                                CHA_TIPO_VISIBILITA = v.CHA_TIPO_VISIBILITA,
                                CHA_NOTIFICA_CONCLUSO = v.CHA_NOTIFICA_CONCLUSO,
                                CHA_NOTIFICA_INTERROTTO = v.CHA_NOTIFICA_INTERROTTO,
                                CHA_NOTIFICA_ERRORE = v.CHA_NOTIFICA_ERRORE,
                                DTA_INIZIO = dta_inizio
                            })
                            .ToListAsync();

                        await this._dbContext.ProcessoFirmaVisibilitaEntities.AddRangeAsync(processoFirmaVisibilitaEntitiesToInsert);
                    }

                    await ((DbContext)_dbContext).SaveChangesAsync();

                    await transaction.CommitAsync();

                    output = (await this._mediator.Send(new GetProcessoDiFirmaRequest(idProcessoNew.ToString(), request.utente))).output;

                    resultCreazioneProcesso = ResultProcessoFirma.OK;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                if (transaction != null)
                    await transaction.RollbackAsync();
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }

            return new DuplicaProcessoFirmaResult(output, resultCreazioneProcesso);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DuplicaProcessoFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
