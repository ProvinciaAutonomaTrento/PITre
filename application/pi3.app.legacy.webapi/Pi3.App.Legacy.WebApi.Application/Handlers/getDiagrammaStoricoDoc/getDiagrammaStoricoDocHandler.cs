// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DiagrammaStato;
using DocsPaVO.documento;
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
using getDiagrammaStoricoDocRequest = Pi3.App.Legacy.WebApi.Application.Requests.getDiagrammaStoricoDoc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getDiagrammaStoricoDoc
{
    public class getDiagrammaStoricoDocHandler : IRequestHandler<getDiagrammaStoricoDocRequest, getDiagrammaStoricoDocResult>
    {
        #region Public Members

        public getDiagrammaStoricoDocHandler(ILogger<getDiagrammaStoricoDocHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getDiagrammaStoricoDocResult> Handle(getDiagrammaStoricoDocRequest request, CancellationToken cancellationToken)
        {
            var docnumberAsLong = request.docNumber.AsLong();

            var diagrammaStoricoEntities = await this._dbContext.DiagrammiStoEntities.AsNoTracking()
                .Where(d => d.DOC_NUMBER == docnumberAsLong)
                .Select(d => new DiagrammaStoricoEntities()
                {
                    ID_USER = d.ID_USER,
                    DTA_DATE = d.DTA_DATE,
                    VAR_DESC_OLD_STATO = d.VAR_DESC_OLD_STATO,
                    VAR_DESC_NEW_STATO = d.VAR_DESC_NEW_STATO,
                    ID_PEOPLE = d.ID_PEOPLE,
                    ID_RUOLO = d.ID_RUOLO,
                    ID_PEOPLE_DELEGATO = d.ID_PEOPLE_DELEGATO
                })
                .Union(
                     this._dbContext.IstanzaProcFirmaStoEntities.AsNoTracking()
                    .Where(d => d.DOC_NUMBER == docnumberAsLong && d.CHA_CAMBIO_STATO_DIAG == "1")
                    .Select(d => new DiagrammaStoricoEntities()
                    {
                        ID_USER = d.ID_USER,
                        DTA_DATE = d.DTA_DATE,
                        VAR_DESC_OLD_STATO = string.Empty,
                        VAR_DESC_NEW_STATO = d.VAR_DESC_AZIONE,
                        ID_PEOPLE = d.ID_PEOPLE,
                        ID_RUOLO = d.ID_RUOLO,
                        ID_PEOPLE_DELEGATO = d.ID_PEOPLE_DELEGATO
                    })
                )
                .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(), d => d.ID_RUOLO, g => g.SYSTEM_ID, (d, g) => new { d, g })
                .Join(this._dbContext.PeopleEntities.AsNoTracking(), j => j.d.ID_PEOPLE, p => p.SYSTEM_ID, (j, p) => new { j.d, j.g, p })
                .GroupJoin(this._dbContext.PeopleEntities, j => j.d.ID_PEOPLE_DELEGATO, pd => pd.SYSTEM_ID, (j, pd) => new { j.d, j.g, j.p, pd })
                .SelectMany(j => j.pd.DefaultIfEmpty(), (j, pd) => new { j.d, j.g, j.p, pd })
                .Select(j => new
                {
                    RUOLO = j.g.VAR_DESC_CORR,
                    UTENTE = j.p.FULL_NAME,
                    DATA = j.d.DTA_DATE,
                    VECCHIO_STATO = j.d.VAR_DESC_OLD_STATO,
                    NUOVO_STATO = j.d.VAR_DESC_NEW_STATO,
                    PEOPLE_DELEGATO = j.pd != null ? j.pd.FULL_NAME : string.Empty
                })
                .OrderByDescending(d => d.DATA)
                .ToListAsync();

            DataSet storico = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("Ruolo");
            dt.Columns.Add("Utente");
            dt.Columns.Add("Data");
            dt.Columns.Add("Vecchio stato");
            dt.Columns.Add("Nuovo Stato");

            diagrammaStoricoEntities.ForEach(d =>
            {
                DataRow dr = dt.NewRow();

                dr["Ruolo"] = d.RUOLO;
                dr["Utente"] = string.IsNullOrEmpty(d.PEOPLE_DELEGATO) ? d.UTENTE : string.Format(Resources.SostitutoDi, d.PEOPLE_DELEGATO, d.UTENTE);
                dr["Data"] = d.DATA.AsDateTimeFormat();
                dr["Vecchio stato"] = d.VECCHIO_STATO;
                dr["Nuovo Stato"] = d.NUOVO_STATO;

                dt.Rows.Add(dr);
            });

            storico.Tables.Add(dt);

            return new getDiagrammaStoricoDocResult(storico);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getDiagrammaStoricoDocHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected class DiagrammaStoricoEntities
        {
            public string? ID_USER { get; set; }
            public DateTime? DTA_DATE { get; set; }
            public string? VAR_DESC_OLD_STATO { get; set; }
            public string? VAR_DESC_NEW_STATO { get; set; }
            public long? ID_PEOPLE { get; set; }
            public long? ID_RUOLO { get; set; }
            public long? ID_PEOPLE_DELEGATO { get; set; }
        }

        #endregion
    }
}
