// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
using GetPianoConservazioneByIdTipoAttoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetPianoConservazioneByIdTipoAtto;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetPianoConservazioneByIdTipoAtto
{
    public class GetPianoConservazioneByIdTipoAttoHandler : IRequestHandler<GetPianoConservazioneByIdTipoAttoRequest, GetPianoConservazioneByIdTipoAttoResult>
    {

        protected readonly ILogger<GetPianoConservazioneByIdTipoAttoHandler> _logger;
        protected readonly IPi3DbContext _dbContext;

        public GetPianoConservazioneByIdTipoAttoHandler(
            ILogger<GetPianoConservazioneByIdTipoAttoHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }
        private async Task<DocsPaVO.PianoConservazione> GetPiano(string idTipoAtto)
        {
            DocsPaVO.PianoConservazione result = null;

            var pianoConservazione = await (from c in this._dbContext.PianoConservazioneEntities.AsNoTracking()
             from t in this._dbContext.PianoConsTipoAttoEntities.AsNoTracking()
             where t.ID_TIPO_ATTO == idTipoAtto.AsLong() && c.SYSTEM_ID == t.ID_PIANO_CONSERVAZIONE
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

            if(pianoConservazione != null)
            {
                result = new DocsPaVO.PianoConservazione()
                {
                    SystemId = pianoConservazione.SYSTEM_ID.ToString(),
                    IdClassificazione = pianoConservazione.ID_CLASSIFICAZIONE.ToString(),
                    NumeroProcedimento = string.IsNullOrEmpty(pianoConservazione.NUMERO_PROCEDIMENTO) ? string.Empty : pianoConservazione.NUMERO_PROCEDIMENTO,
                    TipologiaFascicolo = pianoConservazione.TIPOLOGIA_FASCICOLO,
                    TempoConservazione = pianoConservazione.TEMPO_CONSERVAZIONE,
                    CodiceClassificazione = pianoConservazione.CODICE_CLASSIFICAZIONE,
                    IdRegistro = pianoConservazione.ID_REGISTRO == null ? "0" : pianoConservazione.ID_REGISTRO.ToString()
                };
            }

            return result;
        }

        public async Task<GetPianoConservazioneByIdTipoAttoResult> Handle(GetPianoConservazioneByIdTipoAttoRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.PianoConservazione output = null;
            try
            {
                output = await this.GetPiano(request.idTipoAtto);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output);
        }
    }
}
