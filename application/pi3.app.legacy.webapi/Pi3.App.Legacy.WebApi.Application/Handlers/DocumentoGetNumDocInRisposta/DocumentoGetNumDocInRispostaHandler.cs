// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.Core.Services.Configuration;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.DocumentoGetNumDocInRisposta
{
    public class DocumentoGetNumDocInRispostaHandler : IRequestHandler<Application.Requests.DocumentoGetNumDocInRisposta, DocumentoGetNumDocInRispostaResult>
    {
        public DocumentoGetNumDocInRispostaHandler(IPi3DbContext dbContext, ILogger<DocumentoGetNumDocInRispostaHandler> logger,
            IMediator mediator, IClaimsPrincipalService claimsPrincipalService,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;
            this._configurationService = configurationService;
        }

        public async Task<DocumentoGetNumDocInRispostaResult> Handle(Application.Requests.DocumentoGetNumDocInRisposta request, CancellationToken cancellationToken)
        {
            int count = 0;

            var idGruppo = !string.IsNullOrEmpty(request.idGruppo) ? request.idGruppo.AsLong() : 0;
            var idPeople = !string.IsNullOrEmpty(request.idPeople) ? request.idPeople.AsLong() : 0;
            var idAmm = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            var idRuoloPubblico = await this._configurationService.GetValue<long>(idAmm.ToString(), "ENABLE_FASCICOLO_PUBBLICO");
            var archivioDeposito = await this._configurationService.GetValue<long>("0", "ARCHIVIO_DEPOSITO");

            var idParentFilter = request.queryList.Where(q => q.Any(q2 => q2.argomento == "ID_PARENT")).FirstOrDefault();

            if (idParentFilter != null)
            {
                var idParent = idParentFilter.Select(q => q.valore).First().AsLong();

                var ids = new List<long?> { idGruppo, idPeople };

                if (idRuoloPubblico > 0)
                    ids.Add(idRuoloPubblico);

                if (archivioDeposito == 0)
                {
                    count = await (from p in this._dbContext.ProfileEntities.AsNoTracking()
                                    join s in this._dbContext.SecurityEntities.AsNoTracking()
                                        on p.SYSTEM_ID equals s.THING
                                    where p.ID_PARENT == idParent
                                        && ids.Contains(s.PERSONORGROUP)
                                        && s.ACCESSRIGHTS > 0
                                        && (p.CHA_IN_CESTINO ?? "0") != "1"
                                   select p.SYSTEM_ID)
                            .CountAsync();
                }
            }
            
            return new DocumentoGetNumDocInRispostaResult(count);
        }

        protected ILogger<DocumentoGetNumDocInRispostaHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
    }
}
