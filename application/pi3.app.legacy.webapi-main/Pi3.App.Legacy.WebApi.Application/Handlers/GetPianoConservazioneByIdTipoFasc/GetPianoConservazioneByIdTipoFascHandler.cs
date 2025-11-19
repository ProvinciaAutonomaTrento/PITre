// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetPianoConservazioneByIdTipoFascRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetPianoConservazioneByIdTipoFasc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetPianoConservazioneByIdTipoFasc
{
    public class GetPianoConservazioneByIdTipoFascHandler : IRequestHandler<GetPianoConservazioneByIdTipoFascRequest,GetPianoConservazioneByIdTipoFascResult>
    {
        protected readonly ILogger<GetPianoConservazioneByIdTipoFascHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        public GetPianoConservazioneByIdTipoFascHandler(
            ILogger<GetPianoConservazioneByIdTipoFascHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }


        public async Task<GetPianoConservazioneByIdTipoFascResult> Handle(GetPianoConservazioneByIdTipoFascRequest request,CancellationToken cancellationToken)
        {
            DocsPaVO.PianoConservazione output = null;

            try
            {
                var pianoToBeMapped = await (from c in this._dbContext.PianoConservazioneEntities.AsNoTracking()
                    from t in this._dbContext.PianoConsTipoFascEntities.AsNoTracking()
                    where (t.ID_TIPO_FASC == request.idTipoFasc.AsLong()) &&
                    (c.SYSTEM_ID == t.ID_PIANO_CONSERVAZIONE)
                    select new
                    {
                        c.SYSTEM_ID,
                        c.CODICE_CLASSIFICAZIONE,
                        c.ID_CLASSIFICAZIONE,
                        c.NUMERO_PROCEDIMENTO,
                        c.TIPOLOGIA_FASCICOLO,
                        c.TEMPO_CONSERVAZIONE,
                        c.ID_REGISTRO
                    }).FirstOrDefaultAsync();

                if(pianoToBeMapped != null)
                {
                    output = new()
                    {
                        SystemId = pianoToBeMapped.SYSTEM_ID.ToString(),
                        IdClassificazione = pianoToBeMapped.ID_CLASSIFICAZIONE != null ? pianoToBeMapped.ID_CLASSIFICAZIONE.ToString() : string.Empty,
                        NumeroProcedimento = string.IsNullOrEmpty(pianoToBeMapped.NUMERO_PROCEDIMENTO) ? string.Empty : pianoToBeMapped.NUMERO_PROCEDIMENTO,
                        TipologiaFascicolo = pianoToBeMapped.TIPOLOGIA_FASCICOLO,
                        TempoConservazione = pianoToBeMapped.TEMPO_CONSERVAZIONE,
                        CodiceClassificazione = pianoToBeMapped.CODICE_CLASSIFICAZIONE,
                        IdRegistro = !pianoToBeMapped.ID_REGISTRO.HasValue ? "0" : pianoToBeMapped.ID_REGISTRO.ToString()
                    };
                }


            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output);
        }
    }
}
