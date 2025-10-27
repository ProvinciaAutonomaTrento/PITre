// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodoTitolarioSecurityRequest = Pi3.App.Legacy.WebApi.Application.Requests.NodoTitolarioSecurity;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.NodoTitolarioSecurity
{
    public class NodoTitolarioSecurityHandler : IRequestHandler<NodoTitolarioSecurityRequest, NodoTitolarioSecurityResult>
    {
        #region Public Members

        public NodoTitolarioSecurityHandler(ILogger<NodoTitolarioSecurityHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<NodoTitolarioSecurityResult> Handle(NodoTitolarioSecurityRequest request, CancellationToken cancellationToken)
        {
            var output = new DataSet();

            var childEntities = await this._dbContext.ProjectEntities.AsNoTracking()
                    .Where(x => x.CHA_TIPO_PROJ == "T" && x.ID_AMM == request.idAmm.AsLong() && x.ID_PARENT == request.idTitolario.AsLong())
                    .ToListAsync();

            if (string.IsNullOrEmpty(request.idParent) || request.idParent == "0")
            {
                var projectEntities = await this._dbContext.ProjectEntities.AsNoTracking()
                    .Where(x => x.CHA_TIPO_PROJ == "T" && x.SYSTEM_ID == request.idTitolario.AsLong())
                    .Select(x => new
                    {
                        IDRECORD = x.SYSTEM_ID,
                        CODICE = x.VAR_CODICE,
                        DESCRIZIONE = x.DESCRIPTION,
                        LIVELLO = x.NUM_LIVELLO,
                        REGISTRO = x.ID_REGISTRO,
                        IDPARENT = x.ID_PARENT,
                        CODLIV = x.VAR_COD_LIV1,
                        STATO = x.CHA_STATO,
                        DATA_ATTIVAZIONE = x.DTA_ATTIVAZIONE.AsDateTimeFormat(),
                        NUMMESICONSERVAZIONE = x.NUM_MESI_CONSERVAZIONE ?? 0,
                        FIGLIO = childEntities.Count
                    }).OrderBy(x => x.CODLIV).ToListAsync();

                output = projectEntities.AsDataSet();
            }
            else
            {
                var projectEntities = await this._dbContext.ProjectEntities.AsNoTracking()
                    .Join(this._dbContext.SecurityEntities.AsNoTracking(), a => a.SYSTEM_ID, s => s.THING, (a, s) => new { a, s })
                    .Where(x => x.a.CHA_TIPO_PROJ == "T"
                        && x.a.ID_AMM == request.idAmm.AsLong()
                        && x.a.ID_PARENT == request.idParent.AsLong()
                        && x.s.ACCESSRIGHTS > 0
                        && x.s.PERSONORGROUP == request.idGruppo.AsLong()
                        && (x.a.ID_REGISTRO == null || x.a.ID_REGISTRO == request.idRegistro.AsLong()))
                    .Select(x => new
                    {
                        IDRECORD = x.a.SYSTEM_ID,
                        CODICE = x.a.VAR_CODICE,
                        DESCRIZIONE = x.a.DESCRIPTION,
                        LIVELLO = x.a.NUM_LIVELLO,
                        REGISTRO = x.a.ID_REGISTRO,
                        IDPARENT = x.a.ID_PARENT,
                        CODLIV = x.a.VAR_COD_LIV1,
                        NUMMESICONSERVAZIONE = x.a.NUM_MESI_CONSERVAZIONE ?? 0,
                        FIGLIO = childEntities.Count
                    }).OrderBy(x => x.CODLIV).ToListAsync();

                output = projectEntities.AsDataSet();
            }

            return new NodoTitolarioSecurityResult(output?.GetXml() ?? string.Empty);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<NodoTitolarioSecurityHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}