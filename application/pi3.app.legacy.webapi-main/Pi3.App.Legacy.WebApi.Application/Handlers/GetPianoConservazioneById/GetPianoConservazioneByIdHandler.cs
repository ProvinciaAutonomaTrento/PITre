// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO;
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
using GetPianoConservazioneByIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetPianoConservazioneById;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetPianoConservazioneById
{
    public class GetPianoConservazioneByIdHandler : IRequestHandler<GetPianoConservazioneByIdRequest, GetPianoConservazioneByIdResult>
    {

        protected readonly ILogger<GetPianoConservazioneByIdHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        public GetPianoConservazioneByIdHandler(
            ILogger<GetPianoConservazioneByIdHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<GetPianoConservazioneByIdResult> Handle(GetPianoConservazioneByIdRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.PianoConservazione output = new DocsPaVO.PianoConservazione();

            try
            {
                var tempOut = await this._dbContext.PianoConservazioneEntities.AsNoTracking().Where(c => c.SYSTEM_ID == request.idPianoConservazione.AsLong()).Select( c => new 
                {
                    SystemId = c.SYSTEM_ID.ToString(),
                    CodiceClassificazione = c.CODICE_CLASSIFICAZIONE,
                    IdClassificazione = c.ID_CLASSIFICAZIONE,
                    VoceProcedimento = c.VOCE_PROCEDIMENTO,
                    NumeroProcedimento = c.NUMERO_PROCEDIMENTO,
                    TipologiaFascicolo = c.TIPOLOGIA_FASCICOLO,
                    TempoConservazione = c.TEMPO_CONSERVAZIONE,
                    NoteChiusuraFascicolo = c.NOTE_CHIUSURA_FASCICOLO,
                    NoteScartabilitaDocumenti = c.NOTE_SCARTABILITA_DOC,
                    NoteDocumenti = c.NOTE_DOCUMENTI

                }).FirstOrDefaultAsync();

                if(tempOut != null)
                {
                    output = new()
                    {
                        SystemId = tempOut.SystemId,
                        CodiceClassificazione = tempOut.CodiceClassificazione,
                        IdClassificazione = tempOut.IdClassificazione != null ? tempOut.IdClassificazione.ToString() : string.Empty,
                        VoceProcedimento = tempOut.VoceProcedimento ?? string.Empty,
                        NumeroProcedimento = tempOut.NumeroProcedimento ?? string.Empty,
                        TipologiaFascicolo = tempOut.TipologiaFascicolo ?? string.Empty,
                        TempoConservazione = tempOut.TempoConservazione ?? string.Empty,
                        NoteChiusuraFascicolo = tempOut.NoteChiusuraFascicolo ?? string.Empty,
                        NoteScartabilitaDocumenti = tempOut.NoteScartabilitaDocumenti ?? string.Empty,
                        NoteDocumenti = tempOut.NoteDocumenti ?? string.Empty
                    };
                }

            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new(output);
        }


    }
}
