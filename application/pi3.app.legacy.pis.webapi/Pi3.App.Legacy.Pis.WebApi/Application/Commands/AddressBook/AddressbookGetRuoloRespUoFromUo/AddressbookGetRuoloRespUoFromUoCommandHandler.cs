// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddressbookGetRuoloRespUoFromUo
{
    public class AddressbookGetRuoloRespUoFromUoCommandHandler : IRequestHandler<AddressbookGetRuoloRespUoFromUoCommand, AddressbookGetRuoloRespUoFromUoCommandResponse>
    {
        public AddressbookGetRuoloRespUoFromUoCommandHandler(
            ILogger<AddressbookGetRuoloRespUoFromUoCommandHandler> logger, 
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

        public async Task<AddressbookGetRuoloRespUoFromUoCommandResponse> Handle(AddressbookGetRuoloRespUoFromUoCommand request, CancellationToken cancellationToken)
        {
            string result = string.Empty;

            string idUo = request.IdCorrGlobaliUo;
            string tipoRuolo = request.TipoRuolo;
            string idCorr = request.IdCorr;

            try
            {
                long idUoAsLong = idUo.AsLong();
                bool noRuoloResp = false;
                bool isIdParentNull = false;
                long idParent = 0;
                long idUoAppo = 0;
                while (!isIdParentNull && !noRuoloResp)
                {
                    if (tipoRuolo.Equals("R"))
                    {
                        idUoAppo = this._dbContext.CorrGlobaliEntities
                            .Where(x => x.ID_UO == idUoAsLong && x.CHA_TIPO_URP.Equals("R") &&
                            x.CHA_RIFERIMENTO.Equals("1") &&
                            x.DTA_FINE == null)
                            .Select(x => x.SYSTEM_ID)
                            .FirstOrDefault();
                    }
                    else
                    {
                        idUoAppo = this._dbContext.CorrGlobaliEntities
                            .Where(x => x.ID_UO == idUoAsLong && x.CHA_TIPO_URP.Equals("R") &&
                            x.CHA_SEGRETARIO.Equals("1"))
                            .Select(x => x.SYSTEM_ID)
                            .FirstOrDefault();
                    }

                    if (idUoAppo != null && idUoAppo != idCorr.AsLong())
                    {
                        result = idUoAppo.ToString();
                        noRuoloResp = true;
                    }
                    else
                    {
                        idParent = this._dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == idUoAsLong).Select(x => x.ID_PARENT).FirstOrDefault() ?? 0;
                        if (idParent > 0)
                            idUoAsLong = idParent;
                        else
                        {
                            result = "0";
                            isIdParentNull = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new()
            {
                Output = result
            };
        }

        #region Private Members

        protected readonly ILogger<AddressbookGetRuoloRespUoFromUoCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion

    }
}
