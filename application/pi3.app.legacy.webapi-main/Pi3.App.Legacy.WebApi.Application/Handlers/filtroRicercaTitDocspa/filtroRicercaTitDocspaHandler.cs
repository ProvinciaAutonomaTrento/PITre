// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ProfilazioneDinamicaLite;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using filtroRicercaTitDocspaRequest = Pi3.App.Legacy.WebApi.Application.Requests.filtroRicercaTitDocspa;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.filtroRicercaTitDocspa
{
    public class filtroRicercaTitDocspaHandler : IRequestHandler<filtroRicercaTitDocspaRequest, filtroRicercaTitDocspaResult>
    {
        #region Public Members

        public filtroRicercaTitDocspaHandler(
            ILogger<filtroRicercaTitDocspaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<filtroRicercaTitDocspaResult> Handle(filtroRicercaTitDocspaRequest request, CancellationToken cancellationToken)
        {
            string output = null!;

            try
            {
                var query = this._pi3DbContext.ProjectEntities.AsNoTracking()
                            .Where(result => result.CHA_TIPO_PROJ == "T"
                                    && result.ID_AMM == request.idAmm.AsLong()
                                    && (result.ID_REGISTRO == null || result.ID_REGISTRO == request.idRegistro.AsLong())
                                    && (
                                    this._pi3DbContext.SecurityEntities.AsNoTracking()
                                        .Where(s => s.THING == result.SYSTEM_ID
                                                && s.ACCESSRIGHTS > 0
                                                && s.PERSONORGROUP == request.idGruppo.AsLong())
                                        .Select(s => s.THING)
                                        .Any()))
                            .OrderBy(result => result.NUM_LIVELLO)
                            .Select(result => new
                            {
                                ID = result.SYSTEM_ID,
                                CODICE = result.VAR_CODICE,
                                DESCRIZIONE = result.DESCRIPTION,
                                LIVELLO = result.NUM_LIVELLO,
                                IDPARENT = result.ID_PARENT,
                                IDTITOLARIO = result.ID_TITOLARIO,
                                NOTE = result.VAR_NOTE
                            });

                if (!string.IsNullOrWhiteSpace(request.codice))
                    query = query.Where(result => result.CODICE.ToUpper() == request.codice.ToUpper());

                if (!string.IsNullOrWhiteSpace(request.descrizione))
                    query = query.Where(result => result.DESCRIZIONE.ToUpper().Contains(request.descrizione.ToUpper()));

                if (!string.IsNullOrWhiteSpace(request.idTitolario))
                    query = query.Where(result => result.IDTITOLARIO == request.idTitolario.AsLong());

                if (!string.IsNullOrWhiteSpace(request.note))
                    query = query.Where(result => result.NOTE.ToUpper().Contains(request.note.ToUpper()));

                if (!string.IsNullOrWhiteSpace(request.indice))
                {
                    query = query.Join(this._pi3DbContext.AssIndxSisEntities.AsNoTracking(),
                                        p => p.ID,
                                        a => a.ID_PROJECT,
                                        (p, a) => new { Project = p, AssIndexSis = a })
                                    .Join(this._pi3DbContext.IndexSisEntities.AsNoTracking(),
                                        result => result.AssIndexSis.ID_INDICE_SIS,
                                        indxSys => indxSys.SYSTEM_ID,
                                        (a, i) => new
                                        {
                                            Project = a.Project,
                                            AssIndexSys = a.AssIndexSis,
                                            IndexSys = i
                                        })
                                    .Where(result => result.IndexSys.VOCE_INDICE.ToUpper() == request.indice.ToUpper())
                                    .Select(result => new
                                    {
                                        ID = result.Project.ID,
                                        CODICE = result.Project.CODICE,
                                        DESCRIZIONE = result.Project.DESCRIZIONE,
                                        LIVELLO = result.Project.LIVELLO,
                                        IDPARENT = result.Project.IDPARENT,
                                        IDTITOLARIO = result.Project.IDTITOLARIO,
                                        NOTE = result.Project.NOTE
                                    })
                                    .AsQueryable();
                }

                var result = await query.OrderBy(x => x.LIVELLO).ThenBy(x => x.CODICE).ToListAsync();

                output = result.AsDataSet(dataSetName: "NewDataSet", tableName: "QUERY").GetXml();

                return new filtroRicercaTitDocspaResult(output);
            }
            catch (Pi3Exception pi3Ex)
            {
                output = null!;

                this._logger.LogError(pi3Ex, pi3Ex.Message);
            }
            catch (Exception ex)
            {
                output = null!;

                this._logger.LogCritical(ex, ex.Message);
            }

            return new filtroRicercaTitDocspaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<filtroRicercaTitDocspaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }
}