// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.Services.WebMethodLogger;
using DocsPaVO.InstanceAccess.Metadata;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using DocsPaVO.Notification;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneAddDocFolder
{
    public class FascicolazioneAddDocFolderHandler : IRequestHandler<Application.Requests.FascicolazioneAddDocFolder, FascicolazioneAddDocFolderResult>
    {

        public FascicolazioneAddDocFolderHandler(ILogger<FascicolazioneAddDocFolderHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository, IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._aggregazioneDocumentaleRepository = aggregazioneDocumentaleRepository;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<FascicolazioneAddDocFolderResult> Handle(Application.Requests.FascicolazioneAddDocFolder request, CancellationToken cancellationToken)
        {           
            string idProfile = request.idProfile;
            string idFolder = request.Folder.systemID;
            string IdTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            long idProfileAsLong = idProfile.AsLong();
            long idFolderAsLong = request.Folder.systemID.AsLong();
            var idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idUserAsString = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser).ToString();
            var idGroupAsString = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup).ToString();
            var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            string domainObject = (request.Folder.idParent.Equals(request.Folder.idFascicolo) ? "fascicolo" : "sottofascicolo");
            bool canAddDocInFolder = true;
            bool output = true;
            string msg = string.Empty;
            string descr = string.Empty;
            string codiceFascicolo = string.Empty;           

            await this._dbContext.AssertSecurityRights(request.idProfile.ToString(), idUser.ToString(), idGroup.ToString());
            await this._dbContext.AssertSecurityRights(request.Folder.systemID.ToString(), idUser.ToString(), idGroup.ToString());

            var fascEntity = await this._dbContext.ProjectEntities.FirstOrDefaultAsync(p => p.SYSTEM_ID == request.Folder.idFascicolo.AsLong() && p.CHA_TIPO_PROJ == "F");

            if (request.Folder.descrizione.ToUpper().Equals("ROOT FOLDER") && !string.IsNullOrEmpty(request.descrFasc))
            {
                descr = request.descrFasc;
            }
            else
            {               
                descr = request.Folder.descrizione;
                codiceFascicolo = " del fascicolo: " + fascEntity.VAR_CODICE;
            }

            try
            {
                //verifico se il documento è bloccato
                var checkedOut = this._dbContext.CheckinCheckoutEntities.Any(c => c.ID_DOCUMENT == request.idProfile.AsLong() && c.DOCUMENT_NUMBER == idProfileAsLong);
                if (checkedOut)
                {
                    output = false;
                    msg = string.Format("Il documento con ID {0} risulta bloccato e non può essere inserito nel folder con ID {1}", idProfile, request.Folder.systemID);
                }
                else {
                    var occorrenzeInFolders = this._dbContext.ProjectComponentEntities.Any(pc => pc.LINK == idProfileAsLong && pc.PROJECT_ID == idFolderAsLong);
                    if (!occorrenzeInFolders)
                    {
                        var chaTipoFasc = await
                            (from prj in _dbContext.ProjectEntities
                             join prj2 in _dbContext.ProjectEntities on prj.SYSTEM_ID equals prj2.ID_FASCICOLO //into gj
                             //from subTipoatto in gj.DefaultIfEmpty()
                             where prj2.SYSTEM_ID == idFolderAsLong
                             select prj.CHA_TIPO_FASCICOLO).FirstAsync();

                        //Fascicolo generale
                        if (!String.IsNullOrEmpty(chaTipoFasc) && chaTipoFasc == "G")
                        {
                            //verifico che il titolario sia APERTO                        
                            var listaTitolari = await (from c in this._dbContext.ProjectEntities.AsNoTracking()
                                                        where c.SYSTEM_ID == request.Folder.idFascicolo.AsLong() && c.CHA_TIPO_FASCICOLO == "G"
                                                        select c.ID_TITOLARIO).ToListAsync();

                            var statoTitolario = await (from c in this._dbContext.ProjectEntities.AsNoTracking()
                                                  where listaTitolari.Contains(c.SYSTEM_ID)
                                                  select c.CHA_STATO).FirstOrDefaultAsync();


                            if (!string.IsNullOrEmpty(statoTitolario))
                            {
                                canAddDocInFolder = (statoTitolario == "A");
                            }
                            else
                                { canAddDocInFolder = true; }

                        }
                        else 
                        {
                            var statoFascicolo = fascEntity.CHA_STATO;

                            if (!string.IsNullOrEmpty(statoFascicolo))
                            {
                                canAddDocInFolder = (statoFascicolo == "A");
                            }
                            else
                            { canAddDocInFolder = true; }
                        }
                        if (canAddDocInFolder)
                        {
                            var idFolderPricipal = await this._dbContext.ProjectEntities.Where(p => p.ID_PARENT == request.Folder.idFascicolo.AsLong() && p.ID_FASCICOLO == request.Folder.idFascicolo.AsLong()).Select(p => p.SYSTEM_ID).FirstAsync();
                            AggregazioneDocumentale fascicolo = await this._aggregazioneDocumentaleRepository.Get(IdTenant, idFolderPricipal.ToString(), new ILoadBehavior[1] { new GetAggregatoDocumentaleLoadBehavior() { BypassSecurityCheck = true, LoadFolderHierarchy = true } });
                            IdDoc doc = new IdDoc()
                            {
                                Identiticativo = idProfile

                            };
                            fascicolo.AddIdDoc(doc, idFolder);
                            await _aggregazioneDocumentaleRepository.Update(fascicolo);

                            output = true;

                            //setAs400InFolder? Non trovo la chiave nel codice sorgente. Le chiavi sono sicuramente allineate con prod

                            //calcolaAtipicità da non considerare sul cloud -Luca
                            await this._webMethodLoggerService.LogOK(
                                "FOLDERADDDOC",
                                request.Folder.idFascicolo, string.Format(ErrorDescription.LogDescriptionInsertDocInFasc, idProfile, domainObject, descr + " " + codiceFascicolo), null, "PITRE");

                            await this._webMethodLoggerService.LogOK(
                               "DOCADDINFOLDER",
                               idProfile, string.Format(ErrorDescription.LogDescriptionInsertDocInFasc, idProfile, domainObject, descr + " " + codiceFascicolo), null, "PITRE");
                            await this._webMethodLoggerService.LogOK("FOLLOWFASCEXTAPP", request.Folder.idFascicolo, string.Format(Resources.LogAddDocFolderFollowFascExtApp, idProfile, descr), idProfile);

                            try
                            {
                                var result = ((DbContext)this._dbContext).Database.ExecuteSqlInterpolated($"DECLARE OUTPUT NUMBER; BEGIN SP_FOLLOW_DOMAINOBJECT({request.Folder.idFascicolo}, {request.idProfile}, {FollowDomainObject.OperationFollow.AddDocFolder}, {0}, {0}, {0}, {string.Empty}, output); END;");
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    else
                    {
                        output = false;
                        msg = "Il documento risulta già classificato nel fascicolo richiesto";
                    }
                }
            }
            catch (Exception ex)
            {              
                await this._webMethodLoggerService.LogKO(
                    "FOLDERADDDOC",
                    idFolder, string.Format(ErrorDescription.LogDescriptionInsertDocInFasc, idProfile, idFolder, descr));

                await this._webMethodLoggerService.LogKO(
                   "DOCADDINFOLDER",
                   idProfile, string.Format(ErrorDescription.LogDescriptionInsertDocInFasc, idProfile, idFolder, descr));
                //LogDescriptionInsertDocInFasc
                //BusinessLogic.UserLog.UserLog.WriteLog(infoutente, "FOLDERADDDOC", Folder.systemID, "Inserimento doc " + idProfile + " in " + domainObject + ": " + descr, DocsPaVO.Logger.CodAzione.Esito.KO);
                //BusinessLogic.UserLog.UserLog.WriteLog(infoutente, "DOCADDINFOLDER", idProfile, "Inserimento doc " + idProfile + " in " + domainObject + ": " + descr, DocsPaVO.Logger.CodAzione.Esito.KO);

                _logger.LogError(exception: ex, message: ex.Message);
                msg = ex.Message;
                output = false;
            }
            return new FascicolazioneAddDocFolderResult(output, msg);
        }

        #region Private Members

        protected readonly ILogger<FascicolazioneAddDocFolderHandler> _logger;
        protected readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IPi3DbContext _dbContext;


        //private DocsPaVO.Security.InfoAtipicita CalcolaAtipicita(DocsPaVO.utente.InfoUtente infoUtente, string idDocFasc, DocsPaVO.Security.InfoAtipicita.TipoOggettoAtipico tipo)
        //{
        //    DocsPaVO.Security.InfoAtipicita infoAtipicita = null;
        //    return infoAtipicita;
        //}

        #endregion
    }
}
