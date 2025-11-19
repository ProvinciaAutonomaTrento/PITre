// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoSetFlagDaInviareRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoSetFlagDaInviare;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoSetFlagDaInviare
{
    public class DocumentoSetFlagDaInviareHandler : IRequestHandler<DocumentoSetFlagDaInviareRequest, DocumentoSetFlagDaInviareResult>
    {

        protected readonly ILogger<DocumentoSetFlagDaInviareHandler> _logger;
        protected readonly IPi3DbContext _dbContext;

        private async Task<DocsPaVO.documento.SchedaDocumento> setFlagDaInviare(DocsPaVO.documento.SchedaDocumento schedaDoc)
        {
            List<VersionEntity> verToUpdate = await this._dbContext.VersionEntities.Where( v => 
            ( v.DOCNUMBER == schedaDoc.docNumber.AsLong()) &&
            ( v.CHA_DA_INVIARE != "0" )).ToListAsync();

            verToUpdate.ForEach((v) => {
                v.CHA_DA_INVIARE = "0";
            });

            await ((DbContext)this._dbContext).SaveChangesAsync();


            List<VersionEntity> verToUpdate2 = await this._dbContext.VersionEntities.Where(v =>
            v.VERSION_ID == ((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).versionId.AsLong()
            ).ToListAsync();

            verToUpdate.ForEach((v) => {
                v.CHA_DA_INVIARE = "1";
            });

            await ((DbContext)this._dbContext).SaveChangesAsync();

            for (int i = 1; i < schedaDoc.documenti.Count(); i++)
            {
                ((DocsPaVO.documento.Documento)schedaDoc.documenti[i]).daInviare = "0";
            }
            ((DocsPaVO.documento.Documento)schedaDoc.documenti[0]).daInviare = "1";
            return schedaDoc;

        }



        public DocumentoSetFlagDaInviareHandler(
            ILogger<DocumentoSetFlagDaInviareHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }

        public async Task<DocumentoSetFlagDaInviareResult> Handle(DocumentoSetFlagDaInviareRequest request, CancellationToken cancellationToken)
        {

            DocsPaVO.documento.SchedaDocumento output = null;
            try
            {
                output = await this.setFlagDaInviare(request.schedaDocumento);
            }
            catch(Exception ex)
            {
                this._logger.LogError(ex,ex.Message);
            }


            return new(output);
        }


    }
}
