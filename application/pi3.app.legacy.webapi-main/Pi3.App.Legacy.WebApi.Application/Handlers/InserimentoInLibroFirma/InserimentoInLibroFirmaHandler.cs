// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Distributed;
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InserimentoInLibroFirma
{

    // Richiede libreria MediatR
    public class InserimentoInLibroFirmaHandler : IRequestHandler<Application.Requests.InserimentoInLibroFirma, InserimentoInLibroFirmaResult>
    {
        #region Public Members

        public InserimentoInLibroFirmaHandler(ILogger<InserimentoInLibroFirmaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IDistributedCache distributedCache)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._distributedCache = distributedCache;
        }

        public async Task<InserimentoInLibroFirmaResult> Handle(Application.Requests.InserimentoInLibroFirma request, CancellationToken cancellationToken)
        {
            bool result = false;
            DocsPaVO.LibroFirma.ElementoInLibroFirma elemento = request.elemento;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            IDbContextTransaction? transaction = null;

            try
            {
                transaction = await ((DbContext)this._dbContext).Database.BeginTransactionAsync();
                var elementToAdd = new ElementoInLibroFirmaEntity
                {
                    ID_RUOLO_TITOLARE = elemento.IdRuoloTitolare.AsLong(),
                    TIPO_FIRMA = elemento.TipoFirma.ToString(),
                    STATO_FIRMA = elemento.StatoFirma.ToString(),
                    NOTE = elemento.Note?.Replace("'", "''"),
                    RUOLO_PROPONENTE = elemento.RuoloProponente.idGruppo,
                    UTENTE_PROPONENTE = elemento.UtenteProponente.idPeople,
                    MODALITA = elemento.Modalita.ToString(),
                    DATA_INSERIMENTO = DateTime.Now,
                    DOC_NUMBER = elemento.InfoDocumento.Docnumber.AsLong(),
                    VERSION_ID = elemento.InfoDocumento.VersionId.AsLong(),
                    NUM_VERSIONE = elemento.InfoDocumento.NumVersione,
                    ID_UTENTE_TITOLARE = elemento.IdUtenteTitolare.AsLong(),
                    ID_UTENTE_LOCKER = elemento.IdUtenteLocker.AsLong(),
                    ID_DOC_PRINCIPALE = elemento.InfoDocumento.Docnumber.AsLong(),
                    ID_TRASM_SINGOLA = elemento.IdTrasmSingola.AsLong(),
                    DTA_ACCETTAZIONE = elemento.DataAccettazione.AsDateTime()
                };

                this._dbContext.ElementoInLibroFirmaEntities.Add(elementToAdd);


                int rowIns = await ((DbContext)_dbContext).SaveChangesAsync();

                if (rowIns > 0)
                    result = true;
                if (transaction != null)
                    await transaction.CommitAsync(); 

            }
            catch (Exception ex)
            {
                if (transaction != null)
                    await transaction.RollbackAsync();
                _logger.LogError(ex, null, null);
            }

            return new InserimentoInLibroFirmaResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InserimentoInLibroFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;

        #endregion
    }

}
