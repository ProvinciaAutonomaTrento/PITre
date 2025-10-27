// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ricerche;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Spreadsheet;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getCountDocumentiInFolderCustomRequest = Pi3.App.Legacy.WebApi.Application.Requests.getCountDocumentiInFolderCustom;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getCountDocumentiInFolderCustom
{
    public class getCountDocumentiInFolderCustomHandler : IRequestHandler<getCountDocumentiInFolderCustomRequest, getCountDocumentiInFolderCustomResult>
    {
        protected readonly ILogger<getCountDocumentiInFolderCustomHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IPersonaCorrispondenteRepository _personaCorrispondenteRepository;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IConfigurationService _configurationService;
       
        public getCountDocumentiInFolderCustomHandler(
            ILogger<getCountDocumentiInFolderCustomHandler> logger,
            IPi3DbContext dbContext,
            IPersonaCorrispondenteRepository personaCorrispondenteRepository,
            IClaimsPrincipalService claimsPrincipalService,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._personaCorrispondenteRepository = personaCorrispondenteRepository;
            this._claimsPrincipalService = claimsPrincipalService;
            this._configurationService = configurationService;
        }

        public async Task<getCountDocumentiInFolderCustomResult> Handle(getCountDocumentiInFolderCustomRequest request,CancellationToken cancellationToken)
        {
            int output = 0;
            List<SearchResultInfo> idProfiles = new();
            try
            {
                output = await this.GetCountDocumentiInFolder(request.folder);
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new(output, idProfiles.ToArray());
        }

        private async Task<int> GetCountDocumentiInFolder(DocsPaVO.fascicolazione.Folder objFolder)
        {
            string idAmm = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true)!;
            string idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true)!;
            string idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true)!;

            string idRuoloPubblico = await this._configurationService.GetValue<string>(idAmm, "ENABLE_FASCICOLO_PUBBLICO");

            if (string.IsNullOrEmpty(idRuoloPubblico))
                idRuoloPubblico = "0";

            string?[] personGroupParams = { idGruppo, idPeople, idRuoloPubblico };

            int output = await (from b in this._dbContext.ProjectComponentEntities.AsNoTracking()
                                from a in this._dbContext.ProfileEntities.AsNoTracking()
                                let security = this._dbContext.SecurityEntities.AsNoTracking().Where(s => a.SYSTEM_ID == s.THING && s.ACCESSRIGHTS > 0 && s.PERSONORGROUP != null && personGroupParams.Contains(s.PERSONORGROUP.ToString())).Any()
                                where b.LINK == a.SYSTEM_ID && a.CHA_IN_CESTINO == null && security && b.PROJECT_ID == objFolder.systemID.AsLong()
                                select b.LINK).CountAsync();

            return output;
        }
    }
}
