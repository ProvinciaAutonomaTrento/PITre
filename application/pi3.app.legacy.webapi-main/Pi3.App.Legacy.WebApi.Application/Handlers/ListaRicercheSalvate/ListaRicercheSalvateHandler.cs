// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ListaRicercheSalvate
{

    // Richiede libreria MediatR
    public class ListaRicercheSalvateHandler : IRequestHandler<Pi3.App.Legacy.WebApi.Application.Requests.ListaRicercheSalvate, ListaRicercheSalvateResult>
    {
        #region Public Members

        public ListaRicercheSalvateHandler(ILogger<ListaRicercheSalvateHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }



        public async Task<ListaRicercheSalvateResult> Handle(Application.Requests.ListaRicercheSalvate request, CancellationToken cancellationToken)
        {


            var q1 = _dbContext.SalvaRicercaEntities.
                 Where(w => request.inADL ? w.CHA_IN_ADL == "1" : w.CHA_IN_ADL == "0" && w.TIPO == request.tipo
                 && w.ID_PEOPLE == request.idPeople);

            var q2 = _dbContext.SalvaRicercaEntities.
                 Where(w => request.inADL ? w.CHA_IN_ADL == "1" : w.CHA_IN_ADL == "0" && w.TIPO == request.tipo &&
                 w.ID_GRUPPO == request.idGruppo);

            if (!string.IsNullOrWhiteSpace(request.pgName))
            {
                q1 = q1.Where(w => w.VAR_PAGINA_RIC == request.pgName);
                q2 = q2.Where(w => w.VAR_PAGINA_RIC == request.pgName);
            }

            var qPeople = await q1.Select(s => new
            {
                s.SYSTEM_ID,
                s.VAR_DESCRIZIONE

            }).ToListAsync();

            var qGroups = await q2.Select(s => new
            {
                s.SYSTEM_ID,
                s.VAR_DESCRIZIONE
            }).ToListAsync();

            var qResult = qPeople.Union(qGroups).ToArray();

            DocsPaVO.ricerche.SearchItem[] sList = new DocsPaVO.ricerche.SearchItem[qResult.Count()];

            for (int i = 0; i < qResult.Count(); i++)
            {
                DocsPaVO.ricerche.SearchItem sItem = new DocsPaVO.ricerche.SearchItem();
                sItem.system_id = Convert.ToInt32(qResult[i].SYSTEM_ID);
                sItem.descrizione = qResult[i].VAR_DESCRIZIONE;

                sList[i] = sItem;
            }

            DocsPaVO.ricerche.SearchItemList outcome = new DocsPaVO.ricerche.SearchItemList();
            outcome.lista = sList;

            return new ListaRicercheSalvateResult(outcome);

        }

        #endregion

        #region Private Members

        protected readonly ILogger<ListaRicercheSalvateHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;



        #endregion
    }

}
