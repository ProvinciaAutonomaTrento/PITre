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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getOggettoById
{
    // Richiede libreria MediatR
    public class getOggettoByIdHandler : IRequestHandler<Application.Requests.getOggettoById, getOggettoByIdResult>
    {
        #region Public Members

        public getOggettoByIdHandler(ILogger<getOggettoByIdHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getOggettoByIdResult> Handle(Application.Requests.getOggettoById request, CancellationToken cancellationToken)
        {
            DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoResult = null;
            long idOggetto = request.idOggetto.AsLong();

            try
            {
                var ogg = await _dbContext.OggettiCustomEntities.AsNoTracking()
                    .Join(_dbContext.TipoOggettoEntities.AsNoTracking(),
                        oc => oc.ID_TIPO_OGGETTO,
                        t => t.SYSTEM_ID,
                        (oc, t) => new { oc, DECSRIZIONE_TIPO_OGGETTO = t.DESCRIZIONE })
                    .Join(_dbContext.OggettiCustomCompEntities.AsNoTracking(),
                        j => j.oc.SYSTEM_ID,
                        cc => cc.ID_OGG_CUSTOM,
                        (j, cc) => new { j.oc, j.DECSRIZIONE_TIPO_OGGETTO, cc })
                    .Where(j => j.oc.SYSTEM_ID == idOggetto)
                    .Select(j => new DocsPaVO.ProfilazioneDinamica.OggettoCustom()
                    {
                        SYSTEM_ID = Convert.ToInt32(j.oc.SYSTEM_ID),
                        DESCRIZIONE = j.oc.DESCRIZIONE,
                        ORIZZONTALE_VERTICALE = j.oc.ORIZZONTALE_VERTICALE,
                        CAMPO_OBBLIGATORIO = j.oc.CAMPO_OBBLIGATORIO,
                        MULTILINEA = j.oc.MULTILINEA ?? string.Empty,
                        NUMERO_DI_LINEE = j.oc.NUMERO_DI_LINEE,
                        NUMERO_DI_CARATTERI = j.oc.NUMERO_DI_CARATTERI,
                        CAMPO_DI_RICERCA = j.oc.CAMPO_DI_RICERCA,
                        POSIZIONE = j.cc.POSIZIONE.ToString(),
                        RESETTA_CONTATORE_INIZIO_ANNO = j.oc.RESET_ANNO,
                        FORMATO_CONTATORE = j.oc.RESET_ANNO,
                        TIPO_RICERCA_CORR = j.oc.RICERCA_CORR,
                        ID_RUOLO_DEFAULT = j.oc.ID_R_DEFAULT,
                        CAMPO_COMUNE = j.oc.CAMPO_COMUNE.ToString(),
                        TIPO_CONTATORE = j.oc.CHA_TIPO_TAR,
                        CONTA_DOPO = j.oc.CONTA_DOPO.ToString(),
                        REPERTORIO = j.oc.REPERTORIO.ToString(),
                        MODULO_SOTTOCONTATORE = j.oc.MODULO_SOTTOCONTATORE.ToString(),
                        CONS_REPERTORIO = j.oc.CHA_CONS_REPERTORIO,
                        TIPO = new DocsPaVO.ProfilazioneDinamica.TipoOggetto()
                        {
                            SYSTEM_ID = Convert.ToInt32(j.oc.ID_TIPO_OGGETTO),
                            DESCRIZIONE_TIPO = j.DECSRIZIONE_TIPO_OGGETTO,
                        },
                        CONFIG_OBJ_EST = j.DECSRIZIONE_TIPO_OGGETTO.Equals("OGGETTOESTERNO") ? j.oc.CONFIG_OBJ_EST : string.Empty
                    })
                    .ToListAsync();

                if (ogg.Any())
                    oggettoResult = ogg.FirstOrDefault();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new getOggettoByIdResult(oggettoResult);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getOggettoByIdHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
