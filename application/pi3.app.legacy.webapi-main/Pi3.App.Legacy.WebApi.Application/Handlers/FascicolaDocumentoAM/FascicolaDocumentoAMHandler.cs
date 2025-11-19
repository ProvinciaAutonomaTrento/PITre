// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
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
using FascicolaDocumentoAMRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolaDocumentoAM;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolaDocumentoAM
{
    public class FascicolaDocumentoAMHandler : IRequestHandler<FascicolaDocumentoAMRequest, FascicolaDocumentoAMResult>
    {
        #region Public Members

        public FascicolaDocumentoAMHandler(ILogger<FascicolaDocumentoAMHandler> logger,
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

        public async Task<FascicolaDocumentoAMResult> Handle(FascicolaDocumentoAMRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            var msg = string.Empty;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var systemIdFascicoloAsLong = request.Fascicolo.systemID.AsLong();
                var idFolderFascicolo = await this._dbContext.ProjectEntities.AsNoTracking().Where(p => p.ID_PARENT == systemIdFascicoloAsLong).Select(p => p.SYSTEM_ID).FirstOrDefaultAsync();

                var aggregate = await this._repository.Get(idTenant, idFolderFascicolo.ToString(), new ILoadBehavior[1]
                {
                    new GetAggregatoDocumentaleLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadClassifications = true,
                        LoadFolderHierarchy = true,
                        LoadDocuments = false,
                        LoadNote = false,
                        LoadPermissions = false,
                        LoadProfilesMetadata = false,
                    }
                });

                if (request.Fascicolo.folderSelezionato != null)
                {
                    aggregate.AddIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc
                    {
                        Identiticativo = request.idProfile
                    }, request.Fascicolo.folderSelezionato.systemID);
                }
                else
                {
                    aggregate.AddIdDoc(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.IdDoc
                    {
                        Identiticativo = request.idProfile
                    });
                }

                await _repository.Update(aggregate);

                if (request.Fascicolo.tipo.Equals("P"))
                {
                    await this._webMethodLoggerService.LogOK("FASCICOLOADDDOC", request.Fascicolo.systemID, string.Format(Resources.LogFascicoloAddDoc, request.idProfile, request.Fascicolo.codice), null, "PITRE");
                    await this._webMethodLoggerService.LogOK("DOCADDINFASC", request.idProfile, string.Format(Resources.LogDocAddInFasc, request.idProfile, request.Fascicolo.codice), null, "PITRE");

                    //Inserisco nella coda del motore di Libro firma
                    if ((await this._mediator.Send(new Requests.IsDocInLibroFirma(request.idProfile))).output)
                    {
                        await this._mediator.Send(
                        new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                        {
                            IdProfile = request.idProfile,
                            Evento = "DOC_ADD_INFASC",
                        }));
                    }
                }
                else
                {
                    await this._webMethodLoggerService.LogOK("DOCADDINCLASS", request.idProfile, string.Format(Resources.LogDocAddInClass, request.idProfile, request.Fascicolo.codice), null, "PITRE");
                }
            }
            catch(Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);

                if (request.Fascicolo.tipo.Equals("P"))
                {
                    await this._webMethodLoggerService.LogKO("FASCICOLOADDDOC", request.Fascicolo.systemID, string.Format(Resources.LogFascicoloAddDoc, request.idProfile, request.Fascicolo.codice));
                    await this._webMethodLoggerService.LogKO("DOCADDINFASC", request.idProfile, string.Format(Resources.LogDocAddInFasc, request.idProfile, request.Fascicolo.codice));
                }
                else
                {
                    await this._webMethodLoggerService.LogKO("DOCADDINCLASS", request.idProfile, string.Format(Resources.LogDocAddInClass, request.idProfile, request.Fascicolo.codice));
                }

                msg = pi3Ex.Message;
                output = false;

            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);

                if (request.Fascicolo.tipo.Equals("P"))
                {
                    await this._webMethodLoggerService.LogKO("FASCICOLOADDDOC", request.Fascicolo.systemID, string.Format(Resources.LogFascicoloAddDoc, request.idProfile, request.Fascicolo.codice));
                    await this._webMethodLoggerService.LogKO("DOCADDINFASC", request.idProfile, string.Format(Resources.LogDocAddInFasc, request.idProfile, request.Fascicolo.codice));
                }
                else
                {
                    await this._webMethodLoggerService.LogKO("DOCADDINCLASS", request.idProfile, string.Format(Resources.LogDocAddInClass, request.idProfile, request.Fascicolo.codice));
                }

                output = false;
            }

            return new FascicolaDocumentoAMResult(output, msg);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolaDocumentoAMHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IAggregazioneDocumentaleRepository _repository;

        #endregion
    }
}
