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
using System.Collections;
using DocsPaVO.documento;
using Pi3.App.Legacy.WebApi.Application.Extensions;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetTipoAttoPDInsRic
{
    public class GetTipoAttoPDInsRicHandler : IRequestHandler<getTipoAttoPDInsRic, getTipoAttoPDInsRicResult>
    {
        public GetTipoAttoPDInsRicHandler(ILogger<GetTipoAttoPDInsRicHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getTipoAttoPDInsRicResult> Handle(getTipoAttoPDInsRic request, CancellationToken cancellationToken)
        {
            TipologiaAtto[] output = new TipologiaAtto[0];
            long diritto = Convert.ToInt64(request.diritti);
            long idAmm = Convert.ToInt64(request.idAmministrazione);
            long idRuolo = Convert.ToInt64(request.idGruppo);
            List<long?> diritti = new List<long?> { 1, 2 };
            List<TipologiaAtto> result = new List<TipologiaAtto>();

            try
            {
                if (diritto == 1)
                {
                    var q1 = await (from a in _dbContext.TipoAttoEntities
                              from b in _dbContext.VisTipoDocEntities
                              where (a.ID_AMM == null || a.ID_AMM == idAmm) && 
                              a.SYSTEM_ID == b.ID_TIPO_DOC && 
                              (b.ID_RUOLO == 0 || 
                              (b.ID_RUOLO == idRuolo && diritti.Contains(b.DIRITTI)))
                              || (a.IPERDOCUMENTO == 1 && (a.ID_AMM == null || a.ID_AMM == idAmm)
                              ) orderby a.PESO descending,a.VAR_DESC_ATTO ascending
                              select new
                              {
                                  SYSTEM_ID = a.SYSTEM_ID,
                                  VAR_DESCRIZIONE_ATTO = a.VAR_DESC_ATTO
                              }).Distinct()
                              .ToListAsync();

                    foreach (var q in q1)
                    {
                        result.Add(new TipologiaAtto()
                        {
                            systemId = q.SYSTEM_ID.ToString(),
                            descrizione = q.VAR_DESCRIZIONE_ATTO
                        });
                    }

                    //return new getTipoAttoPDInsRicResult(new System.Collections.ArrayList(q1.ToList()));
                }
                else if (diritto == 2)
                {
                    var q2 = await (from a in _dbContext.TipoAttoEntities
                                    join b in _dbContext.VisTipoDocEntities on a.SYSTEM_ID equals b.ID_TIPO_DOC
                                    where (a.ID_AMM == idAmm || a.ID_AMM == null) && (!a.IN_ESERCIZIO.Equals("NO") || a.IN_ESERCIZIO == null)
                                    && (a.ABILITATO_SI_NO != 0 || a.ABILITATO_SI_NO == null) && (b.ID_RUOLO == 0 || b.ID_RUOLO == idRuolo && b.DIRITTI == 2)
                                    orderby a.PESO descending, a.VAR_DESC_ATTO ascending
                                    select new TipoAttoEntity()
                                    {
                                        SYSTEM_ID = a.SYSTEM_ID,
                                        VAR_DESC_ATTO = a.VAR_DESC_ATTO
                                    }).Distinct().ToListAsync();

                    foreach (var value in q2)
                    {
                        DocsPaVO.documento.TipologiaAtto ritorno = new DocsPaVO.documento.TipologiaAtto()
                        {
                            systemId = value.SYSTEM_ID.ToString(),
                            descrizione = value.VAR_DESC_ATTO
                        };

                        result.Add(ritorno);
                    }
                }

                if (result != null && result.Count > 0)
                    output = result.OrderBy(x => x.descrizione).ToArray();
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }


            return new getTipoAttoPDInsRicResult(output);
        }

        protected ILogger<GetTipoAttoPDInsRicHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;
        
    }
}
