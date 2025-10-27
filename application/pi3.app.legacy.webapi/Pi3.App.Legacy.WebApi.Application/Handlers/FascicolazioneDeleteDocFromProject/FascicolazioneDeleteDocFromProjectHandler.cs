// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.Notification;
using DocsPaVO.Validations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneDeleteDocFromProject
{

    // Richiede libreria MediatR
    public class FascicolazioneDeleteDocFromProjectHandler : IRequestHandler<Requests.FascicolazioneDeleteDocFromProject, FascicolazioneDeleteDocFromProjectResult>
    {
        #region Public Members

        public FascicolazioneDeleteDocFromProjectHandler(ILogger<FascicolazioneDeleteDocFromProjectHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
            IPi3DbContext dbContext, IDistributedCache distributedCache, IWebMethodLoggerService webMethodLoggerService, IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._distributedCache = distributedCache;
            this._webMethodLoggerService = webMethodLoggerService;
            this._aggregazioneDocumentaleRepository = aggregazioneDocumentaleRepository;
        }

        public async Task<FascicolazioneDeleteDocFromProjectResult> Handle(Application.Requests.FascicolazioneDeleteDocFromProject request, CancellationToken cancellationToken)
        {
            DocsPaVO.Validations.ValidationResultInfo result = null;
            string msg = string.Empty;
            string idProfile = request.idProfile;
            DocsPaVO.fascicolazione.Fascicolo fasc = request.fasc;
            DocsPaVO.fascicolazione.Folder folder = request.folder;
            string fascRapida = request.fascRapida;
            var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var idProfileAsLong = request.idProfile.AsLong();
            var idFascicolo = request.folder.idFascicolo.AsLong();

            try
            {
                if (await this._dbContext.CheckinCheckoutEntities.AnyAsync(c => c.ID_DOCUMENT == idProfileAsLong && c.DOCUMENT_NUMBER == idProfileAsLong))
                {
                    result = new ValidationResultInfo();
                    string ID = "DOCUMENTO_IN_CHECKOUT";
                    result.BrokenRules = new BrokenRule[] { new BrokenRule(ID, Resources.LogDocumentoBloccato) };
                    return new FascicolazioneDeleteDocFromProjectResult(result, msg);
                }

                var countDocInFascicolo = await _dbContext.ProjectComponentEntities.AsNoTracking().Where(x => x.LINK == idProfileAsLong).CountAsync();

                var docToRemove = await _dbContext.ProjectComponentEntities.AsNoTracking()
                    .Join(_dbContext.ProjectEntities.AsNoTracking(),
                        pc => pc.PROJECT_ID,
                        p => p.SYSTEM_ID,
                        (pc, p) => new { pc, p })
                    .Where(j => j.p.ID_FASCICOLO == idFascicolo && j.pc.LINK == idProfileAsLong)
                    .Select(j => j.pc)
                    .ToListAsync();

                if (fascRapida != null && fascRapida.ToUpper().Equals("TRUE") && countDocInFascicolo == docToRemove.Count())
                {
                    msg = Resources.AttemptToRemoveLastDocError;
                    return new FascicolazioneDeleteDocFromProjectResult(result, msg);
                }

                if (docToRemove.Any())
                {
                    _dbContext.ProjectComponentEntities.RemoveRange(docToRemove);

                    if (countDocInFascicolo == docToRemove.Count())
                    {
                        var profile = await _dbContext.ProfileEntities.FirstAsync(pc => pc.SYSTEM_ID == idProfileAsLong);
                        profile.CHA_FASCICOLATO = "0";
                        profile.LAST_EDIT_DATE = await _dbContext.GetSystemDateTime();
                    }
                    else
                    {
                        if (docToRemove.Any(d => d.CHA_FASC_PRIMARIA == "1"))
                        {
                            var projectId = docToRemove.Select(d => d.PROJECT_ID).ToList();
                            ProjectComponentEntity? pcEntity = await _dbContext.ProjectComponentEntities
                                .Where(pc => pc.LINK == idProfileAsLong && !projectId.Contains(pc.PROJECT_ID))
                                .OrderBy(pc => pc.DTA_CLASS)
                                .FirstOrDefaultAsync();

                            if (pcEntity != null)
                                pcEntity.CHA_FASC_PRIMARIA = "1";
                        }
                    }

                    await ((DbContext)_dbContext).SaveChangesAsync();
                }

                await this._webMethodLoggerService.LogOK("FOLDERDELDOC", folder.systemID, string.Format(Resources.FolderDelDocOK, idProfile, fasc.codice));
                await this._webMethodLoggerService.LogOK("DOCDELFROMFOLDER", idProfile, string.Format(Resources.DocDelFromFolderOK, idProfile, fasc.codice));
                await this._webMethodLoggerService.LogOK("FASCICOLODELDOC", fasc.systemID, string.Format(Resources.FascicoloDelDocOK, idProfile, fasc.codice));
                await this._webMethodLoggerService.LogOK("FOLLOWFASCEXTAPP", request.folder.idFascicolo, string.Format(Resources.LogDocumentDelFromPrjFollowFascExtApp, request.idProfile, request.folder.descrizione), request.idProfile);

                try
                {
                    var output = ((DbContext)this._dbContext).Database.ExecuteSqlInterpolated($"DECLARE OUTPUT NUMBER; BEGIN SP_FOLLOW_DOMAINOBJECT({request.folder.idFascicolo}, {request.idProfile}, {FollowDomainObject.OperationFollow.RemoveDocFolder}, {0}, {0}, {0}, {string.Empty}, output); END;");
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                await this._webMethodLoggerService.LogKO("FOLDERDELDOC", folder.systemID, string.Format(Resources.FolderDelDocKO, idProfile, fasc.codice));
                await this._webMethodLoggerService.LogKO("DOCDELFROMFOLDER", idProfile, string.Format(Resources.DocDelFromFolderKO, idProfile, fasc.codice));
                await this._webMethodLoggerService.LogKO("FASCICOLODELDOC", folder.systemID, string.Format(Resources.FascicoloDelDocKO, idProfile, fasc.codice));

                msg = ex.Message;
            }

            return new FascicolazioneDeleteDocFromProjectResult(result, msg);
        }


        public async Task<FascicolazioneDeleteDocFromProjectResult> HandleOld(Application.Requests.FascicolazioneDeleteDocFromProject request, CancellationToken cancellationToken)
        {
            DocsPaVO.Validations.ValidationResultInfo result = null;
            string msg = string.Empty;
            string idProfile = request.idProfile;
            DocsPaVO.fascicolazione.Fascicolo fasc = request.fasc;
            DocsPaVO.fascicolazione.Folder folder = request.folder;
            string fascRapida = request.fascRapida;
            var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            try
            {
                if (fascRapida != null && fascRapida.ToUpper().Equals("TRUE")
                        && (await _dbContext.ProjectComponentEntities.Where(x => x.LINK == idProfile.AsLong()).CountAsync()) == 1)
                {
                    msg = Resources.AttemptToRemoveLastDocError;
                    return new FascicolazioneDeleteDocFromProjectResult(result, msg);
                }

                var aggregate = await this._aggregazioneDocumentaleRepository.Get(idTenantAsLong.ToString(), folder.systemID);

                aggregate.RemoveIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc
                {
                    Identiticativo = idProfile
                });

                await _aggregazioneDocumentaleRepository.Update(aggregate);

                await this._webMethodLoggerService.LogOK("FOLDERDELDOC", folder.systemID, string.Format(Resources.FolderDelDocOK, idProfile, fasc.codice));
                await this._webMethodLoggerService.LogOK("DOCDELFROMFOLDER", idProfile, string.Format(Resources.DocDelFromFolderOK, idProfile, fasc.codice));
                await this._webMethodLoggerService.LogOK("FASCICOLODELDOC", fasc.systemID, string.Format(Resources.FascicoloDelDocOK, idProfile, fasc.codice));
                await this._webMethodLoggerService.LogOK("FOLLOWFASCEXTAPP", request.folder.systemID, string.Format(Resources.LogDocumentDelFromPrjFollowFascExtApp, request.idProfile, request.folder.descrizione), request.idProfile);

                //TO DO SP_FOLLOW_DOMAINOBJECT (?)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                await this._webMethodLoggerService.LogKO("FOLDERDELDOC", folder.systemID, string.Format(Resources.FolderDelDocKO, idProfile, fasc.codice));
                await this._webMethodLoggerService.LogKO("DOCDELFROMFOLDER", idProfile, string.Format(Resources.DocDelFromFolderKO, idProfile, fasc.codice));
                await this._webMethodLoggerService.LogKO("FASCICOLODELDOC", folder.systemID, string.Format(Resources.FascicoloDelDocKO, idProfile, fasc.codice));

                msg = ex.Message;
            }

            return new FascicolazioneDeleteDocFromProjectResult(result, msg);
        }


        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneDeleteDocFromProjectHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;

        #endregion
    }

}
