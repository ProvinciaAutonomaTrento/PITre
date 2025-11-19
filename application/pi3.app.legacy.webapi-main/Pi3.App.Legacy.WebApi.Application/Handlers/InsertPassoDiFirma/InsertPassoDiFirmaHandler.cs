// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.DiagrammaStato;
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
using InsertPassoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.InsertPassoDiFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InsertPassoDiFirma
{
    public class InsertPassoDiFirmaHandler : IRequestHandler<InsertPassoDiFirmaRequest, InsertPassoDiFirmaResult>
    {
        #region Public Members

        public InsertPassoDiFirmaHandler(ILogger<InsertPassoDiFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<InsertPassoDiFirmaResult> Handle(InsertPassoDiFirmaRequest request, CancellationToken cancellationToken)
        {
            PassoFirma output = request.passo;

            try
            {
                var passoFirmaEntity = new PassoDiFirmaEntity()
                {
                    ID_PROCESSO = output.idProcesso.AsLong(),
                    NUMERO_SEQUENZA = Convert.ToInt64(output.numeroSequenza),
                    TIPO_FIRMA = output.Evento.CodiceAzione,
                    TIPO_EVENTO = await this._dbContext.AnagraficaEventiEntities.AsNoTracking().Where(a => a.VAR_COD_AZIONE == output.Evento.CodiceAzione).Select(a => a.ID_EVENTO).FirstAsync(),
                    NOTE = output.note,
                    ID_TIPO_RUOLO_COINVOLTO = output.TpoRuoloCoinvolto != null && !string.IsNullOrEmpty(output.TpoRuoloCoinvolto.systemId) ? output.TpoRuoloCoinvolto.systemId.AsLong() : null,
                    ID_RUOLO_COINVOLTO = output.ruoloCoinvolto != null && !string.IsNullOrEmpty(output.ruoloCoinvolto.idGruppo) ? output.ruoloCoinvolto.idGruppo.AsLong() : null,
                    ID_UTENTE_COINVOLTO = output.utenteCoinvolto != null && !string.IsNullOrEmpty(output.utenteCoinvolto.idPeople) ? output.utenteCoinvolto.idPeople.AsLong() : null,
                    ID_AOO = !string.IsNullOrEmpty(output.IdAOO) ? output.IdAOO.AsLong() : null,
                    ID_RF = !string.IsNullOrEmpty(output.IdRF) ? output.IdRF.AsLong() : null,
                    ID_MAIL_REGISTRO = !string.IsNullOrEmpty(output.IdMailRegistro) ? output.IdMailRegistro.AsLong() : null,
                    ID_TIPOLOGIA = !string.IsNullOrEmpty(output.IdTipologia) ? output.IdTipologia.AsLong() : null,
                    ID_STATO_DIAGRAMMA = !string.IsNullOrEmpty(output.IdStatoDiagramma) ? output.IdStatoDiagramma.AsLong() : null,
                    CHA_AUTOMATICO = output.IsAutomatico ? "1" : "0",
                    CHA_FACOLTATIVO = output.IsFacoltativo ? "1" : "0",
                    VAR_POS_SEGNATURA = output.PosizioneSegnaturaPermanente,
                    CHA_POS_SEGNATURA = output.ApplicaSegnaturaPermanente,
                    TICK = "0"
                };

                await this._dbContext.PassoDiFirmaEntities.AddAsync(passoFirmaEntity);
                output.idPasso = passoFirmaEntity.ID_PASSO.ToString();

                if (output.idEventiDaNotificare != null && output.idEventiDaNotificare.Any())
                {
                    //var passoEventoEntity = await this._dbContext.AnagraficaEventiEntities.AsNoTracking()
                    //    .Where(e => output.idEventiDaNotificare.Contains(e.GRUPPO))
                    //    .Select(p => new PassoEventoEntity()
                    //    {
                    //        ID_EVENTO = p.ID_EVENTO,
                    //        ID_PASSO = passoFirmaEntity.ID_PASSO
                    //    })
                    //    .ToListAsync();
                    //await this._dbContext.PassoEventoEntities.AddRangeAsync(passoEventoEntity);

                    foreach (string gruppo in output.idEventiDaNotificare)
                    {
                        await ((DbContext)this._dbContext).Database.ExecuteSqlAsync($"INSERT INTO DPA_PASSO_DPA_EVENTO (ID_PASSO, ID_EVENTO) SELECT {passoFirmaEntity.ID_PASSO}, ID_EVENTO FROM DPA_ANAGRAFICA_EVENTI WHERE GRUPPO = {gruppo}");
                    }
                }

                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new InsertPassoDiFirmaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InsertPassoDiFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
