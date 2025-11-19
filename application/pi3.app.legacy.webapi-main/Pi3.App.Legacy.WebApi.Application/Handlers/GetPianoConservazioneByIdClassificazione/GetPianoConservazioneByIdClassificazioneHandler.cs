// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetPianoConservazioneByIdClassificazioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetPianoConservazioneByIdClassificazione;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetPianoConservazioneByIdClassificazione
{
    public class GetPianoConservazioneByIdClassificazioneHandler : IRequestHandler<GetPianoConservazioneByIdClassificazioneRequest, GetPianoConservazioneByIdClassificazioneResult>
    {
        protected readonly ILogger<GetPianoConservazioneByIdClassificazioneHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        public GetPianoConservazioneByIdClassificazioneHandler(
            ILogger<GetPianoConservazioneByIdClassificazioneHandler> logger,
            IPi3DbContext dbContext    
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }


        public async Task<GetPianoConservazioneByIdClassificazioneResult> Handle(GetPianoConservazioneByIdClassificazioneRequest request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.PianoConservazione> output = new();
            try
            {
                PianoConservazione piano = new();

                var pianiCons = await (from c in this._dbContext.PianoConservazioneEntities.AsNoTracking()
                 where (c.ID_CLASSIFICAZIONE == request.idClassificazione.AsLong() && c.DTA_FINE == null)
                 select new
                 {
                     c.SYSTEM_ID,
                     c.ID_CLASSIFICAZIONE,
                     c.NUMERO_PROCEDIMENTO,
                     c.TIPOLOGIA_FASCICOLO,
                     c.TEMPO_CONSERVAZIONE
                 }).ToListAsync();


                if( pianiCons != null )
                {
                    pianiCons.ForEach((row =>
                    {
                        piano = new DocsPaVO.PianoConservazione()
                        {
                            SystemId = row.SYSTEM_ID.ToString(),
                            IdClassificazione = row.ID_CLASSIFICAZIONE != null ? row.ID_CLASSIFICAZIONE.ToString() : string.Empty,
                            NumeroProcedimento = row.NUMERO_PROCEDIMENTO,
                            TipologiaFascicolo = row.TIPOLOGIA_FASCICOLO,
                            TempoConservazione = row.TEMPO_CONSERVAZIONE
                        };

                        output.Add(piano);
                    }));
                }
            }
            catch (Exception ex)
            {
                output = new();
            }
            return new(output);
        }
    }
}
