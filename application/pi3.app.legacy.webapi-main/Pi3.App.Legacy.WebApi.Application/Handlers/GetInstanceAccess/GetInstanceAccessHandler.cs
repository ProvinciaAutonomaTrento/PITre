// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.InstanceAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetInstanceAccessRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetInstanceAccess;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetInstanceAccess
{
    public class GetInstanceAccessHandler : IRequestHandler<GetInstanceAccessRequest, GetInstanceAccessResult>
    {
        #region Public Members

        public GetInstanceAccessHandler(ILogger<GetInstanceAccessHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetInstanceAccessResult> Handle(GetInstanceAccessRequest request, CancellationToken cancellationToken)
        {
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            List<InstanceAccess> output = new List<InstanceAccess>();

            var instanceAccessEntity = await (from i in this._dbContext.InstanceAccessEntities.AsNoTracking()
                                              join c in this._dbContext.CorrGlobaliEntities on i.ID_RICHIEDENTE equals c.SYSTEM_ID into ic
                                              from x in ic.DefaultIfEmpty()
                                              where i.ID_PEOPLE_PROPRIETARIO == idPeople && i.ID_GRUPPO_PROPRIETARIO == idGruppo
                                              select new
                                              {
                                                  ID_INSTANCE_ACCESS = i.SYSTEM_ID,
                                                  i.DESCRIPTION,
                                                  i.DTA_CREAZIONE,
                                                  i.DTA_CHIUSURA,
                                                  i.ID_PEOPLE_PROPRIETARIO,
                                                  i.ID_GRUPPO_PROPRIETARIO,
                                                  i.ID_RICHIEDENTE,
                                                  i.DTA_RICHIESTA,
                                                  i.ID_DOCUMENTO_RICHIESTO,
                                                  i.NOTE,
                                                  i.CHA_STATO_DOWNLOAD_INOLTRO,
                                                  CODICE_RUBRICA_RICHIEDENTE = (x == null ? null : x.VAR_COD_RUBRICA),
                                                  DESCRIZIONE_RICHIEDENTE = (x == null ? null : x.VAR_DESC_CORR)
                                              })
                                             .OrderByDescending(i => i.DTA_CREAZIONE)
                                             .ToListAsync();

            instanceAccessEntity.ForEach(i =>
                output.Add(new InstanceAccess
                {
                    ID_INSTANCE_ACCESS = i.ID_INSTANCE_ACCESS.ToString(),
                    DESCRIPTION = i.DESCRIPTION,
                    CREATION_DATE = i.DTA_CREAZIONE,
                    CLOSE_DATE = i.DTA_CHIUSURA == null ? DateTime.MinValue : (DateTime)i.DTA_CHIUSURA,
                    ID_PEOPLE_OWNER = i.ID_PEOPLE_PROPRIETARIO.ToString(),
                    ID_GROUPS_OWNER = i.ID_GRUPPO_PROPRIETARIO.ToString(),
                    RICHIEDENTE = i.ID_RICHIEDENTE == null ? null  : new DocsPaVO.utente.Corrispondente
                    {
                        systemId = i.ID_RICHIEDENTE.ToString(),
                        codiceRubrica = i.CODICE_RUBRICA_RICHIEDENTE,
                        descrizione = i.DESCRIZIONE_RICHIEDENTE
                    },
                    REQUEST_DATE = i.DTA_RICHIESTA == null ? DateTime.MinValue : (DateTime)i.DTA_RICHIESTA,
                    ID_DOCUMENT_REQUEST = i.ID_DOCUMENTO_RICHIESTO == null ? string.Empty : i.ID_DOCUMENTO_RICHIESTO.ToString(),
                    NOTE = i.NOTE ?? string.Empty,
                    STATE_DOWNLOAD_FORWARD = !string.IsNullOrEmpty(i.CHA_STATO_DOWNLOAD_INOLTRO) ? Convert.ToChar(i.CHA_STATO_DOWNLOAD_INOLTRO) : '0'
                })
            );

            return new GetInstanceAccessResult(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetInstanceAccessHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
