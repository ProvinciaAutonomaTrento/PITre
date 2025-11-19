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
using GetListaIdTipiFascByIdPianoConservazioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetListaIdTipiFascByIdPianoConservazione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetListaIdTipiFascByIdPianoConservazione
{
    public class GetListaIdTipiFascByIdPianoConservazioneHandler : IRequestHandler<GetListaIdTipiFascByIdPianoConservazioneRequest, GetListaIdTipiFascByIdPianoConservazioneResult>
    {
        protected readonly ILogger<GetListaIdTipiFascByIdPianoConservazioneHandler> _logger;
        protected readonly IPi3DbContext _dbContext;

        public GetListaIdTipiFascByIdPianoConservazioneHandler(
            ILogger<GetListaIdTipiFascByIdPianoConservazioneHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }

        public async Task<GetListaIdTipiFascByIdPianoConservazioneResult> Handle(GetListaIdTipiFascByIdPianoConservazioneRequest request, CancellationToken cancellationToken)
        {
            List<string> output = null;
            try
            {
                output = await (from p in this._dbContext.PianoConsTipoFascEntities.AsNoTracking()
                                where p.ID_PIANO_CONSERVAZIONE == request.idPianoConservazione.AsLong()
                                select p.ID_TIPO_FASC.ToString()).ToListAsync();
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new(output);
        }
    }
}
