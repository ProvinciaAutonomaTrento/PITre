// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocumentFormat.OpenXml.Drawing.Charts;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO_GetListaStoriciMittenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.DO_GetListaStoriciMittente;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DO_GetListaStoriciMittente
{
    public class DO_GetListaStoriciMittenteHandler : IRequestHandler<DO_GetListaStoriciMittenteRequest, DO_GetListaStoriciMittenteResult>
    {
        #region Public Members

        public DO_GetListaStoriciMittenteHandler(
            ILogger<DO_GetListaStoriciMittenteHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<DO_GetListaStoriciMittenteResult> Handle(DO_GetListaStoriciMittenteRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.documento.StoricoMittente[] output = null!;

            try
            {
                var query = from cs in this._pi3DbContext.CorrStoEntities.AsNoTracking()
                            join cg in this._pi3DbContext.CorrGlobaliEntities.AsNoTracking() on cs.ID_MITT_DEST equals cg.SYSTEM_ID
                            where cs.ID_PROFILE == request.idProfile.AsLong()
                            orderby cs.DTA_MODIFICA
                            select new 
                            { 
                                cs.SYSTEM_ID,
                                cs.DTA_MODIFICA,
                                cs.VAR_MOTIVO,
                                cs.CHA_TIPO_MITT_DES,
                                cs.ID_PEOPLE,
                                cs.ID_RUOLO_IN_UO,
                                cg.VAR_DESC_CORR,
                                cg.VAR_COD_RUBRICA                                
                            };

                if (request.tipo == "D")
                    query = query.Where(s => s.CHA_TIPO_MITT_DES == "C" || s.CHA_TIPO_MITT_DES == "D");
                else
                    query = query.Where(s => s.CHA_TIPO_MITT_DES == request.tipo);

                output = await query.Select(s =>
                    new DocsPaVO.documento.StoricoMittente()
                    {
                        systemId = s.SYSTEM_ID.ToString(),
                        dataModifica = (s.DTA_MODIFICA.HasValue ? s.DTA_MODIFICA.Value.AsDateTimeFormat() : null),
                        motivo = s.VAR_MOTIVO,
                        descrizione = s.VAR_DESC_CORR,
                        cod_rubrica = s.VAR_COD_RUBRICA, 
                        utente = this._pi3DbContext.PeopleEntities.AsNoTracking()
                            .Where(p => p.SYSTEM_ID == s.ID_PEOPLE)
                            .Select(p => new DocsPaVO.utente.Utente()
                            {
                                idPeople = p.SYSTEM_ID.ToString(),
                                idAmministrazione = (p.ID_AMM.HasValue ? p.ID_AMM.ToString() : null),
                                userId = p.USER_ID,
                                descrizione = p.FULL_NAME,
                                nome = p.VAR_NOME,
                                cognome = p.VAR_COGNOME
                            })
                            .First(),
                        ruolo = this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(cg => cg.SYSTEM_ID == s.ID_RUOLO_IN_UO)
                            .Select(cg => new DocsPaVO.utente.Ruolo()
                            {
                                systemId = cg.SYSTEM_ID.ToString(),
                                idGruppo = (cg.ID_GRUPPO.HasValue ? cg.ID_GRUPPO.ToString() : null),
                                codice = cg.VAR_CODICE,
                                descrizione = cg.VAR_DESC_CORR
                            })
                            .First()
                    })
                    .ToArrayAsync();
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(pi3Ex, pi3Ex.Message);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, ex.Message);
            }

            return new DO_GetListaStoriciMittenteResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DO_GetListaStoriciMittenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }
}