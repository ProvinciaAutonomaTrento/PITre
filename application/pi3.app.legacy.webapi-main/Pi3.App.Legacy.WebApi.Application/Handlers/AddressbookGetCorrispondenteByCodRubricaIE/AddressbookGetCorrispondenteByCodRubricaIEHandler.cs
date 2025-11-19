// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.AddressbookGetCorrispondenteCompletoBySystemId;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddressbookGetCorrispondenteByCodRubricaIERequest = Pi3.App.Legacy.WebApi.Application.Requests.AddressbookGetCorrispondenteByCodRubricaIE;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AddressbookGetCorrispondenteByCodRubricaIE
{

    public class AddressbookGetCorrispondenteByCodRubricaIEHandler : IRequestHandler<AddressbookGetCorrispondenteByCodRubricaIERequest, AddressbookGetCorrispondenteByCodRubricaIEResult>
    {
        #region Public Members

        public AddressbookGetCorrispondenteByCodRubricaIEHandler(ILogger<AddressbookGetCorrispondenteByCodRubricaIEHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AddressbookGetCorrispondenteByCodRubricaIEResult> Handle(AddressbookGetCorrispondenteByCodRubricaIERequest request, CancellationToken cancellationToken)
        {
            Corrispondente output = null;

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            try
            {
                string tipoIE = string.Empty;

                switch(request.tipoIE)
                {
                    case DocsPaVO.addressbook.TipoUtente.INTERNO:
                        tipoIE = "I";
                        break;

                    case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                        tipoIE = "E";
                        break;
                }

                var predicate = PredicateBuilder.New<CorrGlobaliEntity>(true);
                predicate.And(x => x.VAR_COD_RUBRICA.ToUpper() == request.codice.ToUpper());
                predicate.And(x => x.ID_AMM == idTenant);

                if (!string.IsNullOrWhiteSpace(tipoIE)) predicate.And(x => x.CHA_TIPO_IE == tipoIE);

                var corrGlobaliEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking().FirstOrDefaultAsync(predicate);

                if (corrGlobaliEntity is null)
                    throw new CorrispondenteNotFoundPi3Exception(request.codice);

                output = (await this._mediator.Send(new Requests.AddressbookGetCorrispondenteCompletoBySystemId(
                        corrGlobaliEntity.SYSTEM_ID.ToString(),
                        request.tipoIE,
                        request.u))).output;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new AddressbookGetCorrispondenteByCodRubricaIEResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AddressbookGetCorrispondenteByCodRubricaIEHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
