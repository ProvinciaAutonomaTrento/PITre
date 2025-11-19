// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Notification;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.Validations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FascicolazioneDeleteDocFromFolderRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneDeleteDocFromFolder;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneDeleteDocFromFolder
{

    public class FascicolazioneDeleteDocFromFolderHandler : IRequestHandler<FascicolazioneDeleteDocFromFolderRequest, FascicolazioneDeleteDocFromFolderResult>
    {
        #region Public Members

        public FascicolazioneDeleteDocFromFolderHandler(ILogger<FascicolazioneDeleteDocFromFolderHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IAggregazioneDocumentaleRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<FascicolazioneDeleteDocFromFolderResult> Handle(FascicolazioneDeleteDocFromFolderRequest request, CancellationToken cancellationToken)
        {
            ValidationResultInfo output = null;
            var msg = string.Empty;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            var domainObject = request.folder.idFascicolo.Equals(request.folder.idParent) ? Resources.LogDeleteDocFromProject : Resources.LogDeleteDocFromFolder;
            var idFolder = request.folder.systemID.AsLong();

            try
            {
                var idProfileAsLong = request.idProfile.AsLong();
                if (await this._dbContext.CheckinCheckoutEntities.AnyAsync(c => c.ID_DOCUMENT == idProfileAsLong && c.DOCUMENT_NUMBER == idProfileAsLong))
                {
                    output = new ValidationResultInfo();
                    string ID = "DOCUMENTO_IN_CHECKOUT";
                    output.BrokenRules = new BrokenRule[] { new BrokenRule(ID, Resources.LogDocumentoBloccato) };
                    return new FascicolazioneDeleteDocFromFolderResult(output, msg);
                }

                var countDocInFascicolo = await _dbContext.ProjectComponentEntities.AsNoTracking().Where(x => x.LINK == idProfileAsLong).CountAsync();

                if (!string.IsNullOrEmpty(request.fascRapida) && request.fascRapida.ToUpper().Equals("TRUE") && countDocInFascicolo == 1)
                {
                    msg = Resources.LogFascicolazioneObbligatoria;
                    return new FascicolazioneDeleteDocFromFolderResult(output, msg);
                }

                var docToRemove = await _dbContext.ProjectComponentEntities.AsNoTracking().Where(pc => pc.PROJECT_ID == idFolder && pc.LINK == idProfileAsLong).FirstOrDefaultAsync();

                if (docToRemove != null)
                {
                    _dbContext.ProjectComponentEntities.RemoveRange(docToRemove);

                    if (countDocInFascicolo == 1)
                    {
                        var profile = await _dbContext.ProfileEntities.FirstAsync(pc => pc.SYSTEM_ID == idProfileAsLong);
                        profile.CHA_FASCICOLATO = "0";
                        profile.LAST_EDIT_DATE = await _dbContext.GetSystemDateTime();
                    }
                    else
                    {
                        if (docToRemove.CHA_FASC_PRIMARIA == "1")
                        {
                            ProjectComponentEntity? pcEntity = await _dbContext.ProjectComponentEntities
                                .Where(pc => pc.LINK == idProfileAsLong && docToRemove.PROJECT_ID != pc.PROJECT_ID)
                                .OrderBy(pc => pc.DTA_CLASS)
                                .FirstOrDefaultAsync();

                            if (pcEntity != null)
                                pcEntity.CHA_FASC_PRIMARIA = "1";
                        }
                    }

                    await ((DbContext)_dbContext).SaveChangesAsync();
                }

                await this._webMethodLoggerService.LogOK("FOLDERDELDOC", request.folder.idFascicolo, string.Format(domainObject, request.idProfile, request.folder.descrizione));
                await this._webMethodLoggerService.LogOK("DOCDELFROMFOLDER", request.idProfile, string.Format(domainObject, request.idProfile, request.folder.descrizione));

                await this._webMethodLoggerService.LogOK("FOLLOWFASCEXTAPP", request.folder.idFascicolo, string.Format(Resources.LogDocumentDelFollowFascExtApp, request.idProfile, request.folder.descrizione), request.idProfile);

                try
                {
                    var result = ((DbContext)this._dbContext).Database.ExecuteSqlInterpolated($"DECLARE OUTPUT NUMBER; BEGIN SP_FOLLOW_DOMAINOBJECT({request.folder.idFascicolo}, {request.idProfile}, {FollowDomainObject.OperationFollow.RemoveDocFolder}, {0}, {0}, {0}, {string.Empty}, output); END;");
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                await this._webMethodLoggerService.LogKO("FOLDERDELDOC", request.folder.systemID, string.Format(domainObject, request.idProfile, request.folder.descrizione));
                await this._webMethodLoggerService.LogKO("DOCDELFROMFOLDER", request.idProfile, string.Format(domainObject, request.idProfile, request.folder.descrizione));
                output = null;
            }

            return new FascicolazioneDeleteDocFromFolderResult(output, msg);
        }

        public async Task<FascicolazioneDeleteDocFromFolderResult> HandleOld(FascicolazioneDeleteDocFromFolderRequest request, CancellationToken cancellationToken)
        {
            ValidationResultInfo output = null;
            var msg = string.Empty;
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            var domainObject = request.folder.idFascicolo.Equals(request.folder.idParent) ? Resources.LogDeleteDocFromProject : Resources.LogDeleteDocFromFolder;
            var idFolder = request.folder.idFascicolo.Equals(request.folder.idParent) ? null : request.folder.systemID;
            
            string idProject = request.folder.idFascicolo.Equals(request.folder.idParent) ? request.folder.systemID : "";
            if (string.IsNullOrEmpty(idProject))
            {
                var idFasc = await this._dbContext.ProjectEntities.Where(p => p.ID_PARENT == request.folder.idFascicolo.AsLong()).Select(p => p.SYSTEM_ID).FirstOrDefaultAsync();
                idProject = idFasc.ToString();
            }

            try
            {
                var idProfileAsLong = request.idProfile.AsLong();
                if(await this._dbContext.CheckinCheckoutEntities.AnyAsync(c => c.ID_DOCUMENT == idProfileAsLong && c.DOCUMENT_NUMBER == idProfileAsLong))
                {
                    output = new ValidationResultInfo();
                    string ID = "DOCUMENTO_IN_CHECKOUT";
                    output.BrokenRules = new BrokenRule[] { new BrokenRule(ID, Resources.LogDocumentoBloccato) };
                    return new FascicolazioneDeleteDocFromFolderResult(output, msg);
                }

                if (!string.IsNullOrEmpty(request.fascRapida) && request.fascRapida.ToUpper().Equals("TRUE") 
                    && await this._dbContext.ProjectComponentEntities.AsNoTracking().CountAsync(p => p.LINK == idProfileAsLong) == 1)
                {
                    msg = Resources.LogFascicolazioneObbligatoria;
                    return new FascicolazioneDeleteDocFromFolderResult(output, msg);
                }
                var aggregate = await this._repository.Get(idTenant, idProject, new ILoadBehavior[1]
                {
                    new GetAggregatoDocumentaleLoadBehavior()
                    {
                        BypassSecurityCheck = false,
                        LoadFolderHierarchy = idFolder != null,
                        LoadDocuments = true,
                    }
                });

                aggregate.RemoveIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc()
                {
                     Identiticativo = request.idProfile
                },
                idFolder);

                await this._repository.Update(aggregate);

                await this._webMethodLoggerService.LogOK("FOLDERDELDOC", request.folder.idFascicolo, string.Format(domainObject, request.idProfile, request.folder.descrizione));
                await this._webMethodLoggerService.LogOK("DOCDELFROMFOLDER", request.idProfile, string.Format(domainObject, request.idProfile, request.folder.descrizione));

                await this._webMethodLoggerService.LogOK("FOLLOWFASCEXTAPP", request.folder.idFascicolo, string.Format(Resources.LogDocumentDelFollowFascExtApp, request.idProfile , request.folder.descrizione), request.idProfile);
                //TO DO SP_FOLLOW_DOMAINOBJECT (?)
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                await this._webMethodLoggerService.LogKO("FOLDERDELDOC", request.folder.systemID, string.Format(domainObject, request.idProfile, request.folder.descrizione));
                await this._webMethodLoggerService.LogKO("DOCDELFROMFOLDER", request.idProfile, string.Format(domainObject, request.idProfile, request.folder.descrizione));
                output = null;
            }

            return new FascicolazioneDeleteDocFromFolderResult(output, msg);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneDeleteDocFromFolderHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IAggregazioneDocumentaleRepository _repository;

        #endregion
    }
}
