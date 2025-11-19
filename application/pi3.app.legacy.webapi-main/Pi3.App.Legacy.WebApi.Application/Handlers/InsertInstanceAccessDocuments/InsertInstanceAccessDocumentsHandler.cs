// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.InstanceAccess;
using DocsPaVO.ProfilazioneDinamicaLite;
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
using InsertInstanceAccessDocumentsRequest = Pi3.App.Legacy.WebApi.Application.Requests.InsertInstanceAccessDocuments;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.InsertInstanceAccessDocuments
{
    public class InsertInstanceAccessDocumentsHandler : IRequestHandler<InsertInstanceAccessDocumentsRequest, InsertInstanceAccessDocumentsResult>
    {


        protected readonly ILogger<InsertInstanceAccessDocumentsHandler> _logger;
        protected readonly IPi3DbContext _dbContext;









        private async Task<bool> InsertInstanceAccessAttachments(List<InstanceAccessAttachments> listInstanceAccessAttachments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = true;
            if (listInstanceAccessAttachments != null && listInstanceAccessAttachments.Count > 0)
            {
                foreach (InstanceAccessAttachments instanceAccessAttachments in listInstanceAccessAttachments)
                {

                    InstanceAccessAttEntity att = new()
                    {
                        ID_INST_ACC_DOC = instanceAccessAttachments.ID_INSTANCE_ACCESS_DOCUMENT.AsLong(),
                        ID_ATTACH = instanceAccessAttachments.ID_ATTACH.AsLong(),
                        ENABLE = instanceAccessAttachments.ENABLE ? "1" : "0"
                    };

                    this._dbContext.InstanceAccessAttEntities.Add(att);
                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }
            }
            return result;

        }


        private async Task<bool> InsertInstanceAccessDoc(List<InstanceAccessDocument> listInstanceAccessDocuments, DocsPaVO.utente.InfoUtente infoUtente)
        {
            using var transaction = await ((DbContext)this._dbContext).Database.BeginTransactionAsync();
            bool result = true;
            string idInstanceAccessDoc = string.Empty;

            try
            {
                foreach (InstanceAccessDocument instanceAccessDocument in listInstanceAccessDocuments)
                {
                    InstanceAccessDocEntity doc = new()
                    {
                        ID_INST_ACC = instanceAccessDocument.ID_INSTANCE_ACCESS.AsLong(),
                        DOCNUMBER = instanceAccessDocument.INFO_DOCUMENT.DOCNUMBER.AsLong(),
                        ID_PROJECT = (instanceAccessDocument.INFO_PROJECT == null || string.IsNullOrEmpty(instanceAccessDocument.INFO_PROJECT.ID_PROJECT)) ? null : instanceAccessDocument.INFO_PROJECT.ID_PROJECT.AsLong() ,
                        ID_PARENT = (instanceAccessDocument.INFO_PROJECT == null || string.IsNullOrEmpty(instanceAccessDocument.INFO_PROJECT.ID_PARENT)) ? null : instanceAccessDocument.INFO_PROJECT.ID_PARENT.AsLong(),
                        TIPO_RICHIESTA = string.IsNullOrEmpty(instanceAccessDocument.TYPE_REQUEST) ? string.Empty :instanceAccessDocument.TYPE_REQUEST,
                        ENABLE = instanceAccessDocument.ENABLE ? "1" : "0"
                    };

                    try
                    {
                        this._dbContext.InstanceAccessDocEntities.Add(doc);
                        await ((DbContext)this._dbContext).SaveChangesAsync();
                        idInstanceAccessDoc = doc.SYSTEM_ID.ToString();
                        if (!string.IsNullOrEmpty(idInstanceAccessDoc) && instanceAccessDocument.ATTACHMENTS != null)
                        {
                            foreach (InstanceAccessAttachments att in instanceAccessDocument.ATTACHMENTS)
                            {
                                att.ID_INSTANCE_ACCESS_DOCUMENT = idInstanceAccessDoc;
                            }
                            await InsertInstanceAccessAttachments(instanceAccessDocument.ATTACHMENTS, infoUtente);
                        }
                    }

                    catch(Exception ex)
                    {

                        throw ex;
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

            catch(Exception ex)
            {
                result = false;
                transaction.Rollback();

            }
            return result;
        }
        public InsertInstanceAccessDocumentsHandler(
            ILogger<InsertInstanceAccessDocumentsHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }


        public async Task<InsertInstanceAccessDocumentsResult> Handle(InsertInstanceAccessDocumentsRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                output = await InsertInstanceAccessDoc(request.listInstanceAccessDocuments.ToList(),request.infoUtente);
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new(output);
        }

    }
}
