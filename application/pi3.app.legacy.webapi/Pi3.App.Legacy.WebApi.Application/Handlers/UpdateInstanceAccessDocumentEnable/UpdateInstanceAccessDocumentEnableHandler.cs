// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.InstanceAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateInstanceAccessDocumentEnableRequest = Pi3.App.Legacy.WebApi.Application.Requests.UpdateInstanceAccessDocumentEnable;



namespace Pi3.App.Legacy.WebApi.Application.Handlers.UpdateInstanceAccessDocumentEnable
{
    public class UpdateInstanceAccessDocumentEnableHandler : IRequestHandler<UpdateInstanceAccessDocumentEnableRequest, UpdateInstanceAccessDocumentEnableResult>
    {
        protected readonly ILogger<UpdateInstanceAccessDocumentEnableHandler> _logger;
        protected readonly IPi3DbContext _dbContext;


        public async Task<bool> UpdateInstance(List<InstanceAccessDocument> listInstanceAccessDocument, DocsPaVO.utente.InfoUtente infoUtente) 
        {
            bool result = true;

            foreach (InstanceAccessDocument doc in listInstanceAccessDocument)
            {
                InstanceAccessDocEntity? iaToUpdate = await this._dbContext.InstanceAccessDocEntities.FirstOrDefaultAsync( ia => ia.SYSTEM_ID == doc.ID_INSTANCE_ACCESS_DOCUMENT.AsLong() );

                if (iaToUpdate != null)
                {
                    iaToUpdate.ENABLE = doc.ENABLE ? "1" : "0";
                    int rowsAffected = await ((DbContext)this._dbContext).SaveChangesAsync();
                    if ( rowsAffected < 1)
                    {
                        break;
                    }
                }
            }


            return result;
        }



        public UpdateInstanceAccessDocumentEnableHandler(
            ILogger<UpdateInstanceAccessDocumentEnableHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }


        public async Task<UpdateInstanceAccessDocumentEnableResult> Handle(UpdateInstanceAccessDocumentEnableRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            try
            {
                output = await this.UpdateInstance(request.listInstanceAccessDocument.ToList(), request.infoUtente);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output);
        }

    }
}
