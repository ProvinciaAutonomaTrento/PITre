// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetFascicoloDaCodiceNoSecurity
{
    public class GetFascicoloDaCodiceNoSecurityCommandHandler : IRequestHandler<GetFascicoloDaCodiceNoSecurityCommand, GetFascicoloDaCodiceNoSecurityCommandResponse>
    {
        public GetFascicoloDaCodiceNoSecurityCommandHandler(
            ILogger<GetFascicoloDaCodiceNoSecurityCommandHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<GetFascicoloDaCodiceNoSecurityCommandResponse> Handle(GetFascicoloDaCodiceNoSecurityCommand request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.fascicolazione.Fascicolo> output = null;
            try
            {
                output = await this.GetFascicoloDaCodiceNoSec(request.CodiceFasc, request.IdAmm, request.Titolari, request.SoloGenerali);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            return new()
            {
                Output = output != null ? output.ToArray() : null
            };
        }


        #region Private Members
        protected readonly ILogger<GetFascicoloDaCodiceNoSecurityCommandHandler> _logger;
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
                baseQuery = baseQuery.Where(e => e.ID_TITOLARIO == titolari.AsLong());
            }

            await baseQuery.ForEachAsync((fasc) =>
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

        #endregion
    }
}
