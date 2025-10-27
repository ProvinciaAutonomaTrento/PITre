// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.FriendApplication;
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
using DocumentoAggiungiAllegatoRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoAggiungiAllegato;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoAggiungiAllegato
{
    public class DocumentoAggiungiAllegatoHandler : IRequestHandler<DocumentoAggiungiAllegatoRequest, DocumentoAggiungiAllegatoResult>
    {
        #region Public Members

        public DocumentoAggiungiAllegatoHandler(ILogger<DocumentoAggiungiAllegatoHandler> logger,
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

        public async Task<DocumentoAggiungiAllegatoResult> Handle(DocumentoAggiungiAllegatoRequest request, CancellationToken cancellationToken)
        {
            Allegato output = request.allegato;
            var schedaDocnumber = request.allegato.docNumber;
            try
            {
                var idAmministrazione = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                if (request.allegato.repositoryContext != null)
                {
                    output.version = "1";
                    output.versionId = request.allegato.position.ToString();
                    output.versionLabel = string.Format("A{0:0#}", request.allegato.position.ToString());
                    output.subVersion = "!";
                    output.fileName = string.Empty;
                    output.fileSize = "0";
                }
                else
                {
                    if (string.IsNullOrEmpty(request.allegato.versionLabel))
                    {
                        var aggregate = new Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo(
                            idAmministrazione,
                            DateTime.Now,
                            new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.OggettoDelDocumento() { Descrizione = new TextValue(request.allegato.descrizione) },
                            null,
                            null,
                            null,
                            new Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.IdDoc() { Identiticativo = request.allegato.docNumber }
                            );

                        aggregate.ChangeNumeroPagineAllegato(request.allegato.numeroPagine);
                        await this._repository.Add(aggregate);

                        output.docNumber = aggregate.Id;
                        output.versionId = aggregate.Versions[0].Id;
                        var docnumberAsLong = output.docNumber.AsLong();
                        var versionIdAsLong = output.versionId.AsLong();

                        output.fileName = await this._dbContext.ComponentEntities.AsNoTracking()
                            .Where(c => c.DOCNUMBER == docnumberAsLong && c.VERSION_ID == versionIdAsLong)
                            .Select(c => c.PATH)
                            .FirstOrDefaultAsync();
                    }
                }

                var versionLabel = string.Format("A{0:0#}", output.position);
                await this._webMethodLoggerService.LogOK("DOCUMENTOAGGIUNGIALLEGATO", request.allegato.docNumber, string.Format(Resources.LogDocumentoAggiungiAllegato, request.allegato.docNumber, versionLabel));
                await this._webMethodLoggerService.LogOK("DOCNEWALLEGATO", schedaDocnumber, string.Format(Resources.LogDocNewAllegato, schedaDocnumber, versionLabel));
            }
            catch(Exception ex)
            {
                await this._webMethodLoggerService.LogKO("DOCUMENTOAGGIUNGIALLEGATO", request.allegato.docNumber, string.Format(Resources.LogDocumentoAggiungiAllegato, request.allegato.docNumber, "0"));
                await this._webMethodLoggerService.LogKO("DOCNEWALLEGATO", schedaDocnumber, string.Format(Resources.LogDocNewAllegato, schedaDocnumber, "0"));
                this._logger.LogWebMethodError(ex);
                output = null;
            }

            return new DocumentoAggiungiAllegatoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoAggiungiAllegatoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected IDocumentoAmministrativoRepository _repository;

        #endregion
    }
}
