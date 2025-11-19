// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.addressbook;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using DocsPaVO.utente;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteCompletoBySystemId;
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetCorrispondenteBySystemId
{
    public class AddressbookGetCorrispondenteBySystemIdCommandHandler : IRequestHandler<AddressbookGetCorrispondenteBySystemIdCommand, AddressbookGetCorrispondenteBySystemIdCommandResponse>
    {
        public AddressbookGetCorrispondenteBySystemIdCommandHandler(
            ILogger<AddressbookGetCorrispondenteBySystemIdCommandHandler>  logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AddressbookGetCorrispondenteBySystemIdCommandResponse> Handle(AddressbookGetCorrispondenteBySystemIdCommand request, CancellationToken cancellationToken)
        {
            Corrispondente output = null;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
                long idCorrGlobali = 0;

                var systemId = string.Empty;
                var tipoIE = string.Empty;

                if (long.TryParse(request.SystemId, out idCorrGlobali))
                {
                    var entity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == idCorrGlobali)
                        .Select(c => new
                        {
                            c.CHA_TIPO_IE,
                            c.SYSTEM_ID
                        })
                        .FirstOrDefaultAsync();

                    if (entity != null)
                    {
                        systemId = entity.SYSTEM_ID.ToString();
                        tipoIE = entity.CHA_TIPO_IE;
                    }
                }
                else
                {
                    var entity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.VAR_DESC_CORR.ToUpper() == request.SystemId.ToUpper() && (c.ID_AMM == null || c.ID_AMM == idTenant))
                        .Select(c => new
                        {
                            c.CHA_TIPO_IE,
                            c.SYSTEM_ID
                        })
                        .FirstOrDefaultAsync();

                    if (entity != null)
                    {
                        systemId = entity.SYSTEM_ID.ToString();
                        tipoIE = entity.CHA_TIPO_IE;
                    }
                }

                if (!string.IsNullOrEmpty(systemId))
                {
                    var tipoUtente = "GLOBALE";
                    if (tipoIE == "I")
                        tipoUtente = "INTERNO";
                    if (tipoIE == "E")
                        tipoUtente = "ESTERNO";

                    var getCorrispondente = await this._mediator.Send(new AddressbookGetCorrispondenteCompletoBySystemIdCommand() 
                    {
                        SystemId = systemId, 
                        TipoIE = tipoUtente, 
                        u = null
                    });

                    output = getCorrispondente.output;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new()
            {
                Output = output
            };
        }


        #region Private Members
        protected readonly ILogger<AddressbookGetCorrispondenteBySystemIdCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        #endregion
    }
}
