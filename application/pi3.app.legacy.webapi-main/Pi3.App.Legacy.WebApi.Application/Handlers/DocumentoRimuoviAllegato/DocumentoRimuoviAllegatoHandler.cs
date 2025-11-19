// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoRimuoviAllegato
{
    public class DocumentoRimuoviAllegatoHandler : IRequestHandler<Application.Requests.DocumentoRimuoviAllegato, DocumentoRimuoviAllegatoResult>
    {
        #region Public Members

        public DocumentoRimuoviAllegatoHandler(ILogger<DocumentoRimuoviAllegatoHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            ISessionRepositoryService sessionRepositoryService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._sessionRepositoryService = sessionRepositoryService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        }

        public async Task<DocumentoRimuoviAllegatoResult> Handle(Application.Requests.DocumentoRimuoviAllegato request, CancellationToken cancellationToken)
        {
            var result = false;
            var idDocumentoPrincipale = string.Empty;

            try
            {
                string idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                if (request.allegato.repositoryContext != null)
                {
                    if(await _sessionRepositoryService.FileExists(request.allegato.repositoryContext, request.allegato))
                        await _sessionRepositoryService.RemoveFile(request.allegato.repositoryContext, request.allegato);

                    result = true;
                }
                else
                {
                    idDocumentoPrincipale = request.schedaDocumento.docNumber;
                    var idAllegatoAsLong = request.allegato.docNumber.AsLong();

                    var profileEntity = await _dbContext.ProfileEntities.FirstOrDefaultAsync(p => p.SYSTEM_ID == idAllegatoAsLong);
                    if(profileEntity != null)
                        _dbContext.ProfileEntities.Remove(profileEntity);

                    var versionsEntities = await _dbContext.VersionEntities
                        .Where(v => v.DOCNUMBER == idAllegatoAsLong)
                        .ToListAsync();
                    if (versionsEntities != null)
                        _dbContext.VersionEntities.RemoveRange(versionsEntities);

                    var componentsEntities = await _dbContext.ComponentEntities
                        .Where(v => v.DOCNUMBER == idAllegatoAsLong)
                        .ToListAsync();
                    if (componentsEntities != null)
                        _dbContext.ComponentEntities.RemoveRange(componentsEntities);

                    var notifyEntities = await _dbContext.NotifyEntities
                        .Where(v => v.ID_OBJECT == idAllegatoAsLong)
                        .ToListAsync();
                    if (notifyEntities != null)
                        _dbContext.NotifyEntities.RemoveRange(notifyEntities);

                    var infoFilentities = await _dbContext.InfoFileEntities
                        .Where(v => v.ID_PROFILE == idAllegatoAsLong)
                        .ToListAsync();
                    if (infoFilentities != null)
                        _dbContext.InfoFileEntities.RemoveRange(infoFilentities);

                    await ((DbContext)_dbContext).SaveChangesAsync();

                    if (componentsEntities != null)
                    {
                        foreach (var c in componentsEntities)
                        {
                            if (!string.IsNullOrEmpty(c.PATH) && File.Exists(c.PATH))
                                File.Delete(c.PATH);
                        }
                    }

                    result = true;

                    await this._webMethodLoggerService.LogOK("DOCUMENTORIMUOVIALLEGATO", idDocumentoPrincipale,
                        string.Format(Resources.LogAddRimozioneAllegato, request.allegato.descrizione, idDocumentoPrincipale));

                    //traccio l'evento FOLLOW_DOC_EXT_APP
                    await this._webMethodLoggerService.LogOK("FOLLOWDOCEXTAPP", idDocumentoPrincipale,
                        string.Format(Resources.LogAddRimozioneAllegatoFollowDocExtApp, request.allegato.descrizione, idDocumentoPrincipale));
                }
            }
            catch (Exception ex)
            {
                await this._webMethodLoggerService.LogKO("DOCUMENTORIMUOVIALLEGATO", idDocumentoPrincipale, 
                    string.Format(Resources.LogAddRimozioneAllegato, request.allegato.descrizione, idDocumentoPrincipale));

                this._logger.LogCritical(exception: ex, message: ex.Message);

                result = false;
            }

            return new DocumentoRimuoviAllegatoResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoRimuoviAllegatoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly ISessionRepositoryService _sessionRepositoryService;

        #endregion
    }
}
