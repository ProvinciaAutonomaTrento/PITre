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
using GetFascicoloDaCodiceNoSecurityRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetFascicoloDaCodiceNoSecurity;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetFascicoloDaCodiceNoSecurity
{
    public class GetFascicoloDaCodiceNoSecurityHandler : IRequestHandler<GetFascicoloDaCodiceNoSecurityRequest, GetFascicoloDaCodiceNoSecurityResult>
    {
        protected readonly ILogger<GetFascicoloDaCodiceNoSecurityHandler> _logger;
        protected readonly IPi3DbContext _dbContext;

        
        private async Task<List<DocsPaVO.fascicolazione.Fascicolo>> GetFascicoloDaCodiceNoSec(string codiceFasc, string idAmm, string titolari, bool soloGenerali)
        {
            List<DocsPaVO.fascicolazione.Fascicolo> folders = new();

            var baseQuery = (from p in this._dbContext.ProjectEntities.AsNoTracking()
                             where p.VAR_CODICE != null && p.VAR_CODICE.Equals(codiceFasc) && p.ID_AMM == idAmm.AsLong()
                             orderby p.VAR_COD_LIV1 ascending
                             select new
                             {
                                 p.SYSTEM_ID,
                                 p.VAR_CODICE,
                                 COD_TITOLARIO = IPi3DbContextMappedFunctions.GetDescTitolario(p.ID_TITOLARIO.GetValueOrDefault()),
                                 p.DESCRIPTION,
                                 p.ID_TITOLARIO
                             });

            if (!string.IsNullOrEmpty(titolari))
            {
                baseQuery = baseQuery.Where( e => e.ID_TITOLARIO == titolari.AsLong());
            }

            await baseQuery.ForEachAsync( (fasc) =>
            {
                DocsPaVO.fascicolazione.Fascicolo fld = new DocsPaVO.fascicolazione.Fascicolo();
                fld.systemID = fasc.SYSTEM_ID.ToString();
                fld.codice = fasc.VAR_CODICE;
                fld.codiceRegistroNodoTit = fasc.COD_TITOLARIO;
                fld.descrizione = fasc.DESCRIPTION;
                fld.idTitolario = fasc.ID_TITOLARIO != null ? fasc.ID_TITOLARIO.ToString() : string.Empty;
                folders.Add(fld);

            });


            return folders;
        }

        


        public GetFascicoloDaCodiceNoSecurityHandler(
            ILogger<GetFascicoloDaCodiceNoSecurityHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<GetFascicoloDaCodiceNoSecurityResult> Handle(GetFascicoloDaCodiceNoSecurityRequest request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.fascicolazione.Fascicolo> output = null;
            try
            {
                output = await this.GetFascicoloDaCodiceNoSec(request.codiceFasc,request.idAmm,request.titolari,request.soloGenerali);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output != null ? output.ToArray() : null);
        }
    }
}
