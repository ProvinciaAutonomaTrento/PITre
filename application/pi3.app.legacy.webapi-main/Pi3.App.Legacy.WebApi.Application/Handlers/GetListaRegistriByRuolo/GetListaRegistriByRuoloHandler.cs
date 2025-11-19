// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetListaRegistriByRuolo
{
    public class GetListaRegistriByRuoloHandler : IRequestHandler<Application.Requests.GetListaRegistriByRuolo, GetListaRegistriByRuoloResult>
    {
        private readonly ILogger _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IMediator _mediator;
        private readonly IPi3DbContext _dbContext;

        public GetListaRegistriByRuoloHandler(
            ILogger<GetListaRegistriByRuoloHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }


        public async Task<GetListaRegistriByRuoloResult> Handle(Application.Requests.GetListaRegistriByRuolo request, CancellationToken cancellationToken)
        {
            Registro[] output = null; 

            this._logger.LogInformation("[GetListaRegistriByRuolo.Handle] - START");
            if(! long.TryParse(request.IdRuolo, out long idRuolo) )
            {
                this._logger.LogWarning("[GetListaRegistriByRuolo.Handle] - Parametro IdRuolo non valido");
                throw new ArgumentNullException(nameof(request.IdRuolo));
            }
            ArrayList registers = new ArrayList();

            await Task.Run(() => {

                var query = this._dbContext.RegistroEntities.
                    Join(
                        this._dbContext.RuoloRegistroEntities,
                        r => r.SYSTEM_ID,
                        rr => rr.ID_REGISTRO,
                        (r, rr) => new
                        {
                            registro = r,
                            ruolo = rr
                        })
                    .Join(this._dbContext.AmministraEntities, g => g.registro.ID_AMM, a => a.SYSTEM_ID, (g, a) => new
                    {
                        g.registro,
                        g.ruolo,
                        amministra = a
                    })
                    .Where(w => "0".Equals(w.registro.CHA_RF) && w.ruolo.ID_RUOLO_IN_UO == idRuolo)
                    .OrderBy(j => j.registro.CHA_STATO)
                    .ThenByDescending(j => j.ruolo.CHA_PREFERITO)
                    .ThenBy( o => o.registro.VAR_PREG != null)
                    .ThenBy(o => o.registro.VAR_CODICE)
                    .ThenBy(o => o.registro.VAR_DESC_REGISTRO)
                    .Select(s => new
                    {
                        s.registro.SYSTEM_ID,
                        s.registro.VAR_CODICE,
                        s.registro.NUM_RIF,
                        s.registro.VAR_DESC_REGISTRO,
                        s.registro.VAR_EMAIL_REGISTRO,
                        s.registro.CHA_STATO,
                        s.registro.ID_AMM,
                        s.amministra.VAR_CODICE_AMM,
                        s.registro.DIRITTO_RUOLO_AOO,
                        s.registro.INVIO_RICEVUTA_MANUALE,
                        s.registro.FLAG_WSPIA,
                        s.registro.DTA_OPEN,
                        s.registro.DTA_CLOSE,
                        s.registro.DTA_ULTIMO_PROTO,
                        s.registro.ID_RUOLO_AOO,
                        s.registro.ID_RUOLO_RESP,
                        s.registro.ID_PEOPLE_AOO,
                        s.registro.CHA_AUTO_INTEROP,
                        s.registro.VAR_PREG,
                        s.registro.ANNO_PREG,
                        s.registro.VAR_CODICE_IPA
                    });

                var elementi = query.ToList();
                foreach (var item in elementi)
                {
                    DocsPaVO.utente.Registro reg = new()
                    {
                        systemId = item.SYSTEM_ID.ToString(),
                        codRegistro = item.VAR_CODICE,
                        codice = item.NUM_RIF?.ToString(),
                        descrizione = item.VAR_DESC_REGISTRO,
                        email = item.VAR_EMAIL_REGISTRO,
                        stato = item.CHA_STATO,
                        idAmministrazione = item.ID_AMM?.ToString(),
                        codAmministrazione = item.VAR_CODICE_AMM?.ToString(),
                        dataApertura = item.DTA_OPEN.HasValue ? item.DTA_OPEN.Value.ToString("dd/MM/yyyy") : String.Empty,
                        dataChiusura = item.DTA_CLOSE.HasValue ? item.DTA_CLOSE.Value.ToString("dd/MM/yyyy") : String.Empty,
                        dataUltimoProtocollo = item.DTA_ULTIMO_PROTO.HasValue ? item.DTA_ULTIMO_PROTO.Value.ToString("dd/MM/yyyy") : String.Empty,
                        idRuoloAOO = item.ID_RUOLO_AOO?.ToString(),
                        idRuoloResp = item.ID_RUOLO_RESP?.ToString(),
                        idUtenteAOO = item.ID_PEOPLE_AOO?.ToString(),
                        autoInterop = item.CHA_AUTO_INTEROP,
                        Diritto_Ruolo_AOO = item.DIRITTO_RUOLO_AOO.ToString(),
                        invioRicevutaManuale = item.INVIO_RICEVUTA_MANUALE?.ToString() ?? "0",
                        FlagWspia = item.FLAG_WSPIA?.ToString() ?? "0",
                        flag_pregresso = "1".Equals(item.VAR_PREG),
                        anno_pregresso = String.IsNullOrEmpty(item.VAR_PREG) ? null : item.ANNO_PREG?.ToString(),
                        codiceIpa = item.VAR_CODICE_IPA
                    };
                    registers.Add(reg);
                }

            }, cancellationToken);

            if(registers != null && registers.Count > 0)
                output = registers.Cast<Registro>().ToArray();

            this._logger.LogInformation("[GetListaRegistriByRuolo.Handle] - END");
            return new(output);
        }
    }
}
