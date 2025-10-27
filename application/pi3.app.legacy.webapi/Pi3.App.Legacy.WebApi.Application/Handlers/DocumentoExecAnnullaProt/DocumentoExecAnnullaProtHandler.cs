// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using MediatR;
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
using DocumentoExecAnnullaProtRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoExecAnnullaProt;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoExecAnnullaProt
{
    // Richiede libreria MediatR
    public class DocumentoExecAnnullaProtHandler : IRequestHandler<DocumentoExecAnnullaProtRequest, DocumentoExecAnnullaProtResult>
    {
        #region Public Members

        public DocumentoExecAnnullaProtHandler(ILogger<DocumentoExecAnnullaProtHandler> logger,
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

        public async Task<DocumentoExecAnnullaProtResult> Handle(DocumentoExecAnnullaProtRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            SchedaDocumento schedaDocumento = request.schedaDocumento;
            try
            {
                var idAmministrazione = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);

                var aggregate = await this._repository.Get(idAmministrazione, schedaDocumento.docNumber, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
                        LoadClassifications = true,
                        LoadAllegati = false,
                        LoadAggregazioni = false,
                        LoadVersions = false,
                        LoadPermissions = false,
                        LoadMittentiDestinatari = true
                    }
                });

                if (string.IsNullOrEmpty(request.protAnn.dataAnnullamento))
                    request.protAnn.dataAnnullamento = (await _dbContext.GetSystemDateTime()).AsDateTimeFormat();

                aggregate.Annulla(new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Annullamento
                {
                    Motivo = new TextValue(request.protAnn.autorizzazione),
                    Data = request.protAnn.dataAnnullamento.AsDateTime(),
                    Autore = new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Autore(idPeople)
                });
                await this._repository.Update(aggregate);

                schedaDocumento.protocollo.protocolloAnnullato = request.protAnn;
                output = true;
                await this._webMethodLoggerService.LogOK("DOCUMENTOEXECANNULLAPROT", schedaDocumento.systemId, string.Format(Resources.LogAnnullamentoProtocollo, schedaDocumento.protocollo.segnatura));

                //l'evento FOLLOW_DOC_EXT_APP scatta in seguito all' annullamento di un protocollo
                await this._webMethodLoggerService.LogKO("FOLLOWDOCEXTAPP", schedaDocumento.systemId, string.Format(Resources.LogFollowCancelProto, schedaDocumento.protocollo.segnatura));
            }
            catch (Exception ex)
            {
                await this._webMethodLoggerService.LogKO("DOCUMENTOEXECANNULLAPROT", schedaDocumento.systemId, string.Format(Resources.LogAnnullamentoProtocollo, schedaDocumento.protocollo.segnatura));
                this._logger.LogWebMethodError(ex);
                output = false;
            }
            return new DocumentoExecAnnullaProtResult(output, schedaDocumento);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoExecAnnullaProtHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected IDocumentoAmministrativoRepository _repository;

        #endregion
    }
}
