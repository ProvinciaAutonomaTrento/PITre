// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.KeywordAggregate;
using Pi3.Core.AggregateModels.KeywordAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoAddParolaChiave
{

    // Richiede libreria MediatR
    public class DocumentoAddParolaChiaveHandler : IRequestHandler<Application.Requests.DocumentoAddParolaChiave, DocumentoAddParolaChiaveResult>
    {
        #region Public Members

        public DocumentoAddParolaChiaveHandler(ILogger<DocumentoAddParolaChiaveHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IKeywordRepository keywordRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._KeywordRepository = keywordRepository;
        }

        public async Task<DocumentoAddParolaChiaveResult> Handle(Application.Requests.DocumentoAddParolaChiave request, CancellationToken cancellationToken)
        {
            var keyword = request.parolaChiave;
            long idAmm = request.idAmministrazione.AsLong();

            try
            {

                if (await _dbContext.ParolaEntities.AnyAsync(w => w.VAR_DESC_PAROLA == keyword.descrizione && w.ID_AMM == idAmm))
                      throw new KeywordAlreadyExistPi3Exception();

                Keyword aggregate = new Keyword(request.idAmministrazione, DateTime.Now, new Core.SeedWork.TextValue(keyword.descrizione));

                await this._KeywordRepository.Add(aggregate);

                keyword = _dbContext.ParolaEntities.Where(w => w.VAR_DESC_PAROLA == aggregate.Name.ToString() && w.ID_AMM == idAmm).
                    Select(s => new ParolaChiave()
                    {
                        descrizione = s.VAR_DESC_PAROLA,
                        idAmministrazione = s.ID_AMM.ToString(),
                        systemId = s.SYSTEM_ID.ToString()

                    }).FirstOrDefault();

            }
            catch (Pi3Exception ex)
            {
                this._logger.LogError(exception: ex, message: "Errore in DocumentoAddParolaChiave: " + ex.Message);
                keyword = null;
            }

            return new DocumentoAddParolaChiaveResult(keyword);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoAddParolaChiaveHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IKeywordRepository _KeywordRepository;

        #endregion
    }

}
