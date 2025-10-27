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
using UpdateInstanceAccessDocumentsRequest = Pi3.App.Legacy.WebApi.Application.Requests.UpdateInstanceAccessDocuments;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UpdateInstanceAccessDocuments
{
    public class UpdateInstanceAccessDocumentsHandler : IRequestHandler<UpdateInstanceAccessDocumentsRequest, UpdateInstanceAccessDocumentsResult>
    {
        protected readonly ILogger<UpdateInstanceAccessDocumentsHandler> _logger;
        protected readonly IPi3DbContext _dbContext;



        private async Task<bool> UpdateInstanceAccessDocuments(List<InstanceAccessDocument> listInstanceAccessDocument, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;


            if (listInstanceAccessDocument != null && listInstanceAccessDocument.Count > 0)
            {
                foreach (InstanceAccessDocument doc in listInstanceAccessDocument)
                {
                    InstanceAccessDocEntity? instAccDocToUpdate = await this._dbContext.InstanceAccessDocEntities.FirstOrDefaultAsync(ia => ia.SYSTEM_ID == doc.ID_INSTANCE_ACCESS_DOCUMENT.AsLong());
                    if ( instAccDocToUpdate != null)
                    {
                        instAccDocToUpdate.TIPO_RICHIESTA = doc.TYPE_REQUEST;

                        //int rowsAffected = await ((DbContext)this._dbContext).SaveChangesAsync();
                        //if (rowsAffected == 0)
                        //{
                        //    result = false;
                        //    break;
                        //}
                    }

                   
                }
                int rowsAffected = await ((DbContext)this._dbContext).SaveChangesAsync();
                if (rowsAffected == 0)
                {
                    result = false;
                    //break;
                }
            }
            return result;

        }



        public UpdateInstanceAccessDocumentsHandler(
            ILogger<UpdateInstanceAccessDocumentsHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }


        public async Task<UpdateInstanceAccessDocumentsResult> Handle(UpdateInstanceAccessDocumentsRequest request , CancellationToken cancellationToken)
        {
            bool output = false;
            try
            {
                output = await this.UpdateInstanceAccessDocuments(request.listInstanceAccessDocuments.ToList(),request.infoUtente);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new(output);
        }



    }
}
