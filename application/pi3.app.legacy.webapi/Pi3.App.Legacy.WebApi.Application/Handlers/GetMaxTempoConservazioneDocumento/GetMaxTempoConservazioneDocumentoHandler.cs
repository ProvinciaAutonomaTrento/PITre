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
using GetMaxTempoConservazioneDocumentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetMaxTempoConservazioneDocumento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetMaxTempoConservazioneDocumento
{
    public class GetMaxTempoConservazioneDocumentoHandler : IRequestHandler<GetMaxTempoConservazioneDocumentoRequest, GetMaxTempoConservazioneDocumentoResult>
    {

        protected readonly ILogger<GetMaxTempoConservazioneDocumentoHandler> _logger;
        protected readonly IPi3DbContext _dbContext;

        public GetMaxTempoConservazioneDocumentoHandler(
            ILogger<GetMaxTempoConservazioneDocumentoHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }

        private async Task<string> GetMaxTempo(string idProfile)
        {
            string result = string.Empty;
            long? maxTempoCon = await ( from f1 in this._dbContext.ProjectEntities.AsNoTracking()
                       from f2 in this._dbContext.ProjectEntities.AsNoTracking()
                       from c in this._dbContext.ProjectComponentEntities.AsNoTracking()
                       from t in this._dbContext.PianoConservazioneEntities.AsNoTracking()
                       from a in this._dbContext.PianoConsAssTempoConsEntities.AsNoTracking()
                       where c.LINK == idProfile.AsLong() &&
                       c.PROJECT_ID == f1.SYSTEM_ID &&
                       f1.ID_FASCICOLO == f2.SYSTEM_ID &&
                       f2.CHA_TIPO_FASCICOLO != null &&
                       f2.CHA_TIPO_FASCICOLO.Equals("P") &&
                       t.SYSTEM_ID == f2.ID_PIANO_CONSERVAZIONE &&
                       a.TEMPO_CONSERVAZIONE.Equals(t.TEMPO_CONSERVAZIONE)
                       select new
                       {
                           a.TEMPO_CONSERVAZIONE_IN_ANNI,
                           a.TEMPO_CONSERVAZIONE
                       }).Union(from p in this._dbContext.ProfileEntities.AsNoTracking()
                                from f1 in this._dbContext.ProjectEntities.AsNoTracking()
                                from f2 in this._dbContext.ProjectEntities.AsNoTracking()
                                from c in this._dbContext.ProjectComponentEntities.AsNoTracking()
                                from t in this._dbContext.PianoConservazioneEntities.AsNoTracking()
                                from a in this._dbContext.PianoConsAssTempoConsEntities.AsNoTracking()
                                from pt in this._dbContext.PianoConsTipoAttoEntities.AsNoTracking()
                                where p.SYSTEM_ID == idProfile.AsLong() &&
                                p.SYSTEM_ID == c.LINK &&
                                c.PROJECT_ID == f1.SYSTEM_ID &&
                                f1.ID_FASCICOLO == f2.SYSTEM_ID &&
                                f2.CHA_TIPO_FASCICOLO != null &&
                                f2.CHA_TIPO_FASCICOLO.Equals("G") &&
                                t.ID_CLASSIFICAZIONE == f2.ID_PARENT &&
                                a.TEMPO_CONSERVAZIONE.Equals(t.TEMPO_CONSERVAZIONE) &&
                                pt.ID_TIPO_ATTO == p.ID_TIPO_ATTO &&
                                t.SYSTEM_ID == pt.ID_PIANO_CONSERVAZIONE
                                select new
                                {
                                    a.TEMPO_CONSERVAZIONE_IN_ANNI,
                                    a.TEMPO_CONSERVAZIONE
                                } ).MaxAsync(e => e.TEMPO_CONSERVAZIONE_IN_ANNI);

            if( maxTempoCon != null)
            {
                result = await (from p in this._dbContext.PianoConsAssTempoConsEntities.AsNoTracking()
                                where p.TEMPO_CONSERVAZIONE_IN_ANNI == maxTempoCon
                                select p.TEMPO_CONSERVAZIONE).FirstOrDefaultAsync() ?? result;

            }

            return result;
        }

        public async Task<GetMaxTempoConservazioneDocumentoResult> Handle(GetMaxTempoConservazioneDocumentoRequest request, CancellationToken cancellationToken)
        {
            string output = string.Empty;   
            try
            {
                output = await this.GetMaxTempo(request.idProfile);
            }
            catch ( Exception ex )
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output);
        }
    }
}
