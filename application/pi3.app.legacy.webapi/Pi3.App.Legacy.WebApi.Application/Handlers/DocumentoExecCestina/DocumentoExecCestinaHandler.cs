// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
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
using DocumentoExecCestinaRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoExecCestina;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoExecCestina
{
    public class DocumentoExecCestinaHandler : IRequestHandler<DocumentoExecCestinaRequest, DocumentoExecCestinaResult>
    {
        #region Public Members

        public DocumentoExecCestinaHandler(ILogger<DocumentoExecCestinaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._repository = repository;
        }

        public async Task<DocumentoExecCestinaResult> Handle(DocumentoExecCestinaRequest request, CancellationToken cancellationToken)
        {
            bool output = true;
            var errorMsg = string.Empty;

            try
            {
                var idAmministrazione = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idProfile = request.schedaDoc.docNumber.AsLong();

                var aggregate = await this._repository.Get(idAmministrazione, request.schedaDoc.docNumber, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = false,
                        LoadClassifications = false,
                        LoadAllegati = true,
                        LoadAggregazioni = false,
                        LoadVersions = false,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true
                    }
                });

                if (aggregate.Reserved)
                { 
                    if(await this._dbContext.CheckinCheckoutEntities.AsNoTracking().AnyAsync(c => c.ID_DOCUMENT == idProfile && c.ID_USER == idPeople))
                    {
                        aggregate.Unreserve();
                    }
                    else
                    {
                        throw new DocumentoBloccatoException();
                    }
                }

                aggregate.Recycle(new TextValue(request.note));
                await this._repository.Update(aggregate);

                await this._webMethodLoggerService.LogOK("DOCUMENTOEXECRIMUOVISCHEDA", request.schedaDoc.systemId, string.Format(Resources.LogSpostamentoInCestino, request.schedaDoc.docNumber));

                //l'evento FOLLOW_DOC_EXT_APP scatta in seguito all' annullamento di un protocollo
                await this._webMethodLoggerService.LogKO("FOLLOWDOCEXTAPP", request.schedaDoc.systemId, string.Format(Resources.LogFollowRemovedDoc, request.schedaDoc.systemId));
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                output = false;
                errorMsg = ex.Message;
                await this._webMethodLoggerService.LogKO("DOCUMENTOEXECRIMUOVISCHEDA", request.schedaDoc.systemId, string.Format(Resources.LogSpostamentoInCestino, request.schedaDoc.docNumber));
            }

            return new DocumentoExecCestinaResult(output, errorMsg);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoExecCestinaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected IDocumentoAmministrativoRepository _repository;

        #endregion
    }
}
