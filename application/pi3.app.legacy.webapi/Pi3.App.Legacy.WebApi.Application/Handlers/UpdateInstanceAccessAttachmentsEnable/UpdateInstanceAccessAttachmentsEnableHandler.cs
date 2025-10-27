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
using UpdateInstanceAccessAttachmentsEnableRequest = Pi3.App.Legacy.WebApi.Application.Requests.UpdateInstanceAccessAttachmentsEnable;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UpdateInstanceAccessAttachmentsEnable
{
    public class UpdateInstanceAccessAttachmentsEnableHandler : IRequestHandler<UpdateInstanceAccessAttachmentsEnableRequest, UpdateInstanceAccessAttachmentsEnableResult>
    {

        protected readonly ILogger<UpdateInstanceAccessAttachmentsEnableHandler> _logger;
        protected readonly IPi3DbContext _dbContext;




        private async Task<bool> UpdateInstance(List<InstanceAccessAttachments> listInstanceAccessAttachments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            int rowAffected;

            foreach (InstanceAccessAttachments att in listInstanceAccessAttachments)
            {
                InstanceAccessAttEntity? iaToUpdate = await this._dbContext.InstanceAccessAttEntities.FirstOrDefaultAsync( ia => ia.SYSTEM_ID == att.SYSTEM_ID.AsLong());
            
                if(iaToUpdate != null)
                {
                    iaToUpdate.ENABLE = att.ENABLE ? "1" : "0";
                    rowAffected = await ((DbContext)this._dbContext).SaveChangesAsync();
                    if(rowAffected < 1)
                    {
                        result = false;
                        break;
                    }
                }
            
            }

            return result;

        }

        public UpdateInstanceAccessAttachmentsEnableHandler(
            ILogger<UpdateInstanceAccessAttachmentsEnableHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }


        public async Task<UpdateInstanceAccessAttachmentsEnableResult> Handle(UpdateInstanceAccessAttachmentsEnableRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            try
            {
                output = await this.UpdateInstance(request.listInstanceAccessAttachments.ToList(), request.infoUtente);
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new(output);
        }



    }
}
