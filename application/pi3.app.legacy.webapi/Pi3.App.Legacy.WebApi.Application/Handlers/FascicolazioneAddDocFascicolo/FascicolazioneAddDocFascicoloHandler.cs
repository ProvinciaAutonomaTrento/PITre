// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.Notification;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneAddDocFascicolo
{

    // Richiede libreria MediatR
    public class FascicolazioneAddDocFascicoloHandler : IRequestHandler<Application.Requests.FascicolazioneAddDocFascicolo, FascicolazioneAddDocFascicoloResult>
    {
        #region Public Members

        public FascicolazioneAddDocFascicoloHandler(ILogger<FascicolazioneAddDocFascicoloHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IDistributedCache distributedCache, IWebMethodLoggerService webMethodLoggerService, IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._distributedCache = distributedCache;
            this._webMethodLoggerService = webMethodLoggerService;
            this._aggregazioneDocumentaleRepository = aggregazioneDocumentaleRepository;
        }

        public async Task<FascicolazioneAddDocFascicoloResult> Handle(Application.Requests.FascicolazioneAddDocFascicolo request, CancellationToken cancellationToken)
        {
            bool result = true;
            string msg = String.Empty;
            string idProfile = request.idProfile;
            DocsPaVO.fascicolazione.Fascicolo fascicolo = request.Fascicolo;
            bool fascRapida = request.fascRapida;
            var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser).ToString();
            var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup).ToString();

            long idGroupAsLong = idGroup.AsLong();
            long idUserAsLong = idUser.AsLong();

            try
            {
                //var folder = await this._dbContext.ProjectEntities
                //    .Where(x => x.ID_FASCICOLO == fascicolo.systemID.AsLong() &&
                //        this._dbContext.SecurityEntities.Any(s => x.SYSTEM_ID == s.THING &&
                //            (s.PERSONORGROUP == idUserAsLong || s.PERSONORGROUP == idGroupAsLong) &&
                //            s.ACCESSRIGHTS > 0) &&
                //        x.ID_PARENT == fascicolo.systemID.AsLong())
                //    .FirstOrDefaultAsync();

                DocsPaVO.fascicolazione.Folder folder = (await this._mediator.Send(new Application.Requests.FascicolazioneGetFolder(idUser, idGroup, fascicolo))).output;

                bool isSottofascicolo = folder.idParent != folder.idFascicolo;
                var aggregate = await this._aggregazioneDocumentaleRepository.Get(idTenantAsLong.ToString(), folder.systemID);

                if (isSottofascicolo)
                {
                    aggregate.AddIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc
                    {
                        Identiticativo = idProfile
                    }, folder.systemID);
                }
                else
                {
                    aggregate.AddIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc
                    {
                        Identiticativo = idProfile
                    });
                }

                await _aggregazioneDocumentaleRepository.Update(aggregate);

                if (result)
                {
                    if (fascicolo.tipo.Equals("P"))
                    {
                        await this._webMethodLoggerService.LogOK("FASCICOLOADDDOC", fascicolo.systemID, string.Format(Resources.LogFascicoloAddDocOK, idProfile, fascicolo.codice));
                        await this._webMethodLoggerService.LogOK(
                        "DOCADDINFASC",
                        idProfile, string.Format(Resources.LogDocAddInFascOK, idProfile, fascicolo.codice), null, "PITRE");

                        //Inserisco nella coda del motore di Libro firma
                        if ((await this._mediator.Send(new Requests.IsDocInLibroFirma(idProfile))).output)
                        {
                            await this._mediator.Send(
                            new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                            {
                                IdProfile = idProfile,
                                Evento = "DOC_ADD_INFASC",
                            }));
                        }
                    }
                    else
                    {
                        await this._webMethodLoggerService.LogOK(
                            "DOCADDINCLASS",
                            idProfile, string.Format(Resources.LogDocAddInClassOK, idProfile, fascicolo.codice), null, "PITRE");
                    }
                }
                else
                {
                    await this._webMethodLoggerService.LogKO("FASCICOLOADDDOC", idProfile, string.Format(Resources.LogFascicoloAddDocKO, idProfile, fascicolo.codice));
                    if (fascicolo.tipo.Equals("P"))
                        await this._webMethodLoggerService.LogKO(
                        "DOCADDINFASC",
                        idProfile, string.Format(Resources.LogDocAddInFascKO, idProfile, fascicolo.codice));
                    else
                        await this._webMethodLoggerService.LogKO(
                            "DOCADDINCLASS",
                            idProfile, string.Format(Resources.LogDocAddInClassKO, idProfile, fascicolo.codice));
                }


                await this._webMethodLoggerService.LogOK("FOLLOWFASCEXTAPP", fascicolo.systemID, string.Format(Resources.LogDocumentAddDocFascFollowFascExtApp, idProfile, fascicolo.descrizione), idProfile);

                try
                {
                    var output = ((DbContext)this._dbContext).Database.ExecuteSqlInterpolated($"DECLARE OUTPUT NUMBER; BEGIN SP_FOLLOW_DOMAINOBJECT({fascicolo.systemID}, {request.idProfile}, {FollowDomainObject.OperationFollow.AddDocFolder}, {0}, {0}, {0}, {string.Empty}, output); END;");
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                await this._webMethodLoggerService.LogKO("FASCICOLOADDDOC", idProfile, string.Format(Resources.LogFascicoloAddDocKO, idProfile, fascicolo.codice));
                if (fascicolo.tipo.Equals("P"))
                    await this._webMethodLoggerService.LogKO(
                    "DOCADDINFASC",
                    idProfile, string.Format(Resources.LogDocAddInFascKOWithExc, idProfile, fascicolo.codice, ex));
                else
                    await this._webMethodLoggerService.LogKO(
                        "DOCADDINCLASS",
                        idProfile, string.Format(Resources.LogDocAddInClassKOWithExc, idProfile, fascicolo.codice, ex));

                msg = ex.Message;
                result = false;
            }
            return new FascicolazioneAddDocFascicoloResult(result, msg);
        }



        #endregion

        #region Private Members
        protected readonly ILogger<FascicolazioneAddDocFascicoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;

        #endregion
    }

}
