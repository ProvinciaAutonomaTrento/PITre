// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.amministrazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getNodiFromProtoTitRequest = Pi3.App.Legacy.WebApi.Application.Requests.getNodiFromProtoTit;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getNodiFromProtoTit
{
    public class getNodiFromProtoTitHandler : IRequestHandler<getNodiFromProtoTitRequest, getNodiFromProtoTitResult>
    {
        #region Public Members

        public getNodiFromProtoTitHandler(
            ILogger<getNodiFromProtoTitHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<getNodiFromProtoTitResult> Handle(getNodiFromProtoTitRequest request, CancellationToken cancellationToken)
        {
            OrgNodoTitolario[] output = null!;
            
            try
            {
                var query = this._pi3DbContext.ProjectEntities
                    .AsNoTracking()
                    .Where(p => p.ID_TITOLARIO == request.idTitolario.AsLong()
                            && p.NUM_PROT_TIT == request.numProtoPratica
                            && p.CHA_TIPO_PROJ == "T");

                if (request.registro != null)
                    query = query.Where(p => p.ID_REGISTRO == null || p.ID_REGISTRO == request.registro.systemId.AsLong());

                output = await (query.Select(p => new OrgNodoTitolario()
                                {
                                    Codice = p.VAR_CODICE!,
                                    Descrizione = p.DESCRIPTION!,
                                    numProtoTit = p.NUM_PROT_TIT!,
                                    IDRegistroAssociato = (p.ID_REGISTRO.HasValue ? p.ID_REGISTRO.ToString() : null)!,
                                    ID_Titolario = (p.ID_TITOLARIO.HasValue ? p.ID_TITOLARIO.ToString() : null)!
                                }))
                                .ToArrayAsync();
            }
            catch (Pi3Exception pi3Ex)
            {
                output = null!;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                output = null!;
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new getNodiFromProtoTitResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getNodiFromProtoTitHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }
}