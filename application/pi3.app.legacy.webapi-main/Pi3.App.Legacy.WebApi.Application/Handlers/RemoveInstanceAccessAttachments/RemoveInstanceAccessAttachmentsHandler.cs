// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.InstanceAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoveInstanceAccessAttachmentsRequest = Pi3.App.Legacy.WebApi.Application.Requests.RemoveInstanceAccessAttachments;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.RemoveInstanceAccessAttachments
{
    public class RemoveInstanceAccessAttachmentsHandler : IRequestHandler<RemoveInstanceAccessAttachmentsRequest, RemoveInstanceAccessAttachmentsResult>
    {


        protected readonly ILogger<RemoveInstanceAccessAttachmentsHandler> _logger;
        protected readonly IPi3DbContext _dbContext;


        private async Task<bool> RemoveInstanceAccessAttachment(List<InstanceAccessAttachments> listInstanceAccessAttachments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            int rowAffected;
            using var transaction = await ((DbContext)this._dbContext).Database.BeginTransactionAsync();
            try
            {
                if (listInstanceAccessAttachments != null && listInstanceAccessAttachments.Count > 0)
                {
                    foreach (InstanceAccessAttachments instanceAccessAttachments in listInstanceAccessAttachments)
                    {
                        InstanceAccessAttEntity? attToDelete = await this._dbContext.InstanceAccessAttEntities.Where(att =>
                        att.ID_INST_ACC_DOC == instanceAccessAttachments.ID_INSTANCE_ACCESS_DOCUMENT.AsLong() &&
                        att.ID_ATTACH == instanceAccessAttachments.ID_ATTACH.AsLong()).FirstOrDefaultAsync();

                        if (attToDelete != null)
                        {
                            this._dbContext.InstanceAccessAttEntities.Remove(attToDelete);
                            rowAffected = await ((DbContext)this._dbContext).SaveChangesAsync();
                            
                            if(rowAffected <= 0)
                            {
                                result = false;
                                break;
                            }
                        }
                    }

                }

                if (result)
                {
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                }
            }
            catch (Exception ex)
            {
                result = false;
                transaction.Rollback();
            }
            return result;

        }


        public RemoveInstanceAccessAttachmentsHandler(
            ILogger<RemoveInstanceAccessAttachmentsHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }




        public async Task<RemoveInstanceAccessAttachmentsResult> Handle(RemoveInstanceAccessAttachmentsRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                output = await this.RemoveInstanceAccessAttachment(request.listInstanceAccessAttachments.ToList(),request.infoUtente);
            }

            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new(output);
        }


    }
}
