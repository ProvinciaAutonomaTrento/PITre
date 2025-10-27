// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DiagrammaStato;
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
using GetRapportoVersamentoFascicoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetRapportoVersamentoFascicolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetRapportoVersamentoFascicolo
{
    public class GetRapportoVersamentoFascicoloHandler : IRequestHandler<GetRapportoVersamentoFascicoloRequest, GetRapportoVersamentoFascicoloResult>
    {
        protected readonly ILogger<GetRapportoVersamentoFascicoloHandler> _logger;
        protected readonly IPi3DbContext _dbContext;


        private async Task<string> GetStatoConservazioneFascicolo(string idProject)
        {
            string retVal = string.Empty;
            retVal = await this._dbContext.VersamentoFascicoliEntities.AsNoTracking().Where(v => v.ID_PROJECT == idProject.AsLong()).Select(v => v.CHA_STATO).FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(retVal))
                retVal = "N";
            return retVal;
        }


        private async Task<string> GetRapporto(string idProject)
        {
            string result = string.Empty;

            string xmlString = await this._dbContext.VersamentoFascicoliEntities.AsNoTracking().Where( v => v.ID_PROJECT == idProject.AsLong()).Select(v => v.VAR_FILE_RISPOSTA).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(xmlString))
            {
                var xml = new System.Xml.XmlDocument();
                xml.LoadXml(xmlString);

                string stato = await GetStatoConservazioneFascicolo(idProject);
                string rapporto = string.Empty;

                if (stato == "C" || stato == "R")
                {
                    result = xml.InnerXml;
                }
            }

            return result;
        }

        public GetRapportoVersamentoFascicoloHandler(
            ILogger<GetRapportoVersamentoFascicoloHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<GetRapportoVersamentoFascicoloResult> Handle(GetRapportoVersamentoFascicoloRequest request , CancellationToken cancellationToken)
        {
            string output = string.Empty;
            try
            {
                output = await this.GetRapporto(request.idProject);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new(output);
        }
    }
}
