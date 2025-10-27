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
using InsertProcessoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.InsertProcessoDiFirma;
using InsertPassoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.InsertPassoDiFirma;
using Microsoft.EntityFrameworkCore.Storage;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InsertProcessoDiFirma
{
    public class InsertProcessoDiFirmaHandler : IRequestHandler<InsertProcessoDiFirmaRequest, InsertProcessoDiFirmaResult>
    {
        #region Public Members

        public InsertProcessoDiFirmaHandler(ILogger<InsertProcessoDiFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<InsertProcessoDiFirmaResult> Handle(InsertProcessoDiFirmaRequest request, CancellationToken cancellationToken)
        {
            ProcessoFirma output = request.processoDiFirma;
            ResultProcessoFirma resultProcessoFirma = ResultProcessoFirma.KO;
            IDbContextTransaction? transaction = null;

            try
            {
                if (((DbContext)_dbContext).Database.CurrentTransaction == null)
                    transaction = await ((DbContext)_dbContext).Database.BeginTransactionAsync();

                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

                var ruoloAutoreAsLong = request.infoUtente.idGruppo.AsLong();
                long? idPeopleAutoreAsLong = !string.IsNullOrEmpty(request.infoUtente.idPeople) ? request.infoUtente.idPeople.AsLong() : null;

                var nomeProcessoUpper = output.nome.ToUpper();

                var exists = await this._dbContext.SchemaProcessoFirmaEntities.AsNoTracking()
                    .AnyAsync(p => p.NOME.ToUpper() == nomeProcessoUpper && p.RUOLO_AUTORE == ruoloAutoreAsLong);

                if(exists)
                {
                    resultProcessoFirma = ResultProcessoFirma.EXISTING_PROCESS_NAME;
                    throw new ProcessoFirmaNomeEsistentePi3Exception(output.nome);
                }

                var processoFirmaEntity = new SchemaProcessoFirmaEntity()
                {
                    NOME = output.nome,
                    RUOLO_AUTORE = ruoloAutoreAsLong,
                    UTENTE_AUTORE = idPeopleAutoreAsLong,
                    CHA_MODELLO = output.IsProcessModel ? "1" : "0",
                    ID_AMM = idTenant,
                    DTA_CREAZIONE = await this._dbContext.GetSystemDateTime()
                };
                await this._dbContext.SchemaProcessoFirmaEntities.AddAsync(processoFirmaEntity);

                if(output.passi != null && output.passi.Count() > 0)
                {
                    foreach (var passo in output.passi)
                    {
                        passo.idProcesso = processoFirmaEntity.ID_PROCESSO.ToString();
                        passo.idPasso = (await this._mediator.Send(new InsertPassoDiFirmaRequest(passo, request.infoUtente))).output.idPasso;
                    }
                }
                else
                {
                    output.passi = new List<PassoFirma>();
                }

                await ((DbContext)_dbContext).SaveChangesAsync();

                if(transaction != null)
                    await transaction.CommitAsync();

                output.idProcesso = processoFirmaEntity.ID_PROCESSO.ToString();

                resultProcessoFirma = ResultProcessoFirma.OK;

            }
            catch (ProcessoFirmaNomeEsistentePi3Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                if(transaction != null)
                    await transaction.RollbackAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                if (transaction != null)
                    await transaction.RollbackAsync();
                output = null;
            }
            finally
            {
                if (transaction != null)
                    transaction.Dispose();
            }

            return new InsertProcessoDiFirmaResult(output, resultProcessoFirma);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InsertProcessoDiFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
