// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.addressbook;
using DocsPaVO.utente;
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
using AddressbookGetCorrispondenteByIdPeopleRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddressbookGetCorrispondenteByIdPeople;
using AddressbookGetCorrispondenteCompletoBySystemIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddressbookGetCorrispondenteCompletoBySystemId;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AddressbookGetCorrispondenteByIdPeople
{
    public class AddressbookGetCorrispondenteByIdPeopleHandler : IRequestHandler<AddressbookGetCorrispondenteByIdPeopleRequest, AddressbookGetCorrispondenteByIdPeopleResult>
    {
        #region Public Members

        public AddressbookGetCorrispondenteByIdPeopleHandler(ILogger<AddressbookGetCorrispondenteByIdPeopleHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AddressbookGetCorrispondenteByIdPeopleResult> Handle(AddressbookGetCorrispondenteByIdPeopleRequest request, CancellationToken cancellationToken)
        {
            Corrispondente output = null;

            try
            {
                var idPeopleAsLong = request.idPeople.AsLong();
                string tipoIE = null;
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                switch (request.tipoIE)
                {
                    case TipoUtente.INTERNO:
                        tipoIE = "I";
                        break;
                    case DocsPaVO.addressbook.TipoUtente.ESTERNO:
                        tipoIE = "E";
                        break;
                    case DocsPaVO.addressbook.TipoUtente.GLOBALE:
                        tipoIE = null;
                        break;
                }

                var entity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.ID_PEOPLE == idPeopleAsLong && c.CHA_TIPO_IE == tipoIE && c.ID_AMM == idTenant && c.DTA_FINE == null)
                    .Select(c => new
                    {
                        c.CHA_TIPO_IE,
                        c.SYSTEM_ID
                    })
                    .FirstOrDefaultAsync();

                if (entity != null)
                {
                    var tipoUtente = TipoUtente.GLOBALE;
                    if (tipoIE == "I")
                        tipoUtente = TipoUtente.INTERNO;
                    if (tipoIE == "E")
                        tipoUtente = TipoUtente.ESTERNO;

                    output = (await this._mediator.Send(new AddressbookGetCorrispondenteCompletoBySystemIdRequest(entity.SYSTEM_ID.ToString(), tipoUtente, null))).output;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new AddressbookGetCorrispondenteByIdPeopleResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AddressbookGetCorrispondenteByIdPeopleHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
