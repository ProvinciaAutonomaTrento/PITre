// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocumentFormat.OpenXml.Drawing.Charts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using GetListInfoFileDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetListInfoFileDocument;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetListInfoFileDocument
{
    public class GetListInfoFileDocumentHandler : IRequestHandler<GetListInfoFileDocumentRequest, GetListInfoFileDocumentResult>
    {
        #region Public Members

        public GetListInfoFileDocumentHandler(
            ILogger<GetListInfoFileDocumentHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetListInfoFileDocumentResult> Handle(GetListInfoFileDocumentRequest request, CancellationToken cancellationToken)
        {
            List<InfoFile> output = null!;

            try
            {
                output = await this._pi3DbContext.InfoFileEntities
                    .AsNoTracking()
                    .Where(i => i.ID_PROFILE == request.idProfile.AsLong()
                    || i.ID_DOCUMENTO_PRINCIPALE == request.idProfile.AsLong())
                    .OrderBy(i => i.SYSTEM_ID)
                    .Select(i => new InfoFile()
                    {
                        SystemId = i.SYSTEM_ID.ToString(),
                        IdProfile = i.ID_PROFILE.ToString(),
                        IdDocumentoPrincipale = (i.ID_DOCUMENTO_PRINCIPALE.HasValue ? i.ID_DOCUMENTO_PRINCIPALE.ToString() : null),
                        VersionId = i.VERSION_ID.ToString(),
                        DataAcquisizione = i.DTA_ACQUISIZIONE.AsDateTimeFormat(),
                        Estensione = i.VAR_ESTENSIONE,
                        NomeFile = i.VAR_NOME_FILE,
                        DescrizioneInfoFile = i.VAR_DESC_INFO_FILE,
                        Conforme = i.CHA_CONFORME == "1",
                        EstensioneConforme = i.CHA_ESTENSIONE_CONFORME == "1",
                        ContieneMacro = i.CHA_PRESENZA_MACRO == "1",
                        ContieneForms = i.CHA_PRESENZA_FORMS == "1",
                        ContieneJavascript = i.CHA_PRESENZA_JAVASCRIPT == "1",
                        Oggetto = this._pi3DbContext.ProfileEntities
                                    .AsNoTracking()
                                    .Where(p => p.SYSTEM_ID == request.idProfile.AsLong())
                                    .Select(p => p.VAR_PROF_OGGETTO)
                                    .First()
                    })
                    .ToListAsync();
            }
            catch (Pi3Exception pi3Ex)
            {
                output = null!;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                output = null!;
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }   

            return new GetListInfoFileDocumentResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetListInfoFileDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }
}