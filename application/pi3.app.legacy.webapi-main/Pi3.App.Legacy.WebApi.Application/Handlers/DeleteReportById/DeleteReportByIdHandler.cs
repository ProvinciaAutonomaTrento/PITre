// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Notification;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DeleteReportById
{

    // Richiede libreria MediatR
    public class DeleteReportByIdHandler : IRequestHandler<Application.Requests.DeleteReportById, DeleteReportByIdResult>
    {
        #region Public Members

        public DeleteReportByIdHandler(ILogger<DeleteReportByIdHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
           
        }

        public async Task<DeleteReportByIdResult> Handle(Application.Requests.DeleteReportById request, CancellationToken cancellationToken)
        {
            try
            {
                var idReport = request.idReport.AsLong();

                List<long?> item = await GetItemPregressi(idReport);

                RemoveReport(item, idReport);
            }
            catch
            {
                return new DeleteReportByIdResult(false);
            }
            
            return new DeleteReportByIdResult(true);
        }

        #endregion

        #region Private Members

        protected virtual async Task<List<long?>> GetItemPregressi(long id)
        {
            return await _dbContext.AssPregressiEntities.Where(w => w.ID_PREGRESSO == id).OrderBy(o => o.SYSTEM_ID).Select(s => s.SYSTEM_ID).ToListAsync();
        }

        protected virtual async void RemoveReport(List<long?> items, long idReport)
        {
            try
            {
                if (items.Count > 0)
                {
                    foreach (var i in items)
                    {
                        var allegatoEntity = await _dbContext.AssAllegatoEntities.Where(w => w.ID_ITEM == i).FirstOrDefaultAsync();
                        if(allegatoEntity != null)
                        ((DbContext)_dbContext).Remove(allegatoEntity);
                    }

                    var ItemPregessiEntity =await  _dbContext.AssPregressiEntities.Where(w => w.ID_PREGRESSO == idReport).FirstOrDefaultAsync();
                    if(ItemPregessiEntity != null)
                    ((DbContext)_dbContext).Remove(ItemPregessiEntity);
                }

                var PregressiEntity =await _dbContext.PregressoEntities.Where(w => w.SYSTEM_ID == idReport).FirstOrDefaultAsync();
                if(PregressiEntity != null)
                ((DbContext)_dbContext).Remove(PregressiEntity);

                ((DbContext)_dbContext).SaveChanges();
            }
            catch(Exception ex)
            {                
                this._logger.LogError(ex, "rimozione report non effettuata, rollback operazioni intermedie, eccezione --> " + ex.Message);
                throw new Exception();
            }
                       
        }

        protected readonly ILogger<DeleteReportByIdHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        #endregion
    }

}
