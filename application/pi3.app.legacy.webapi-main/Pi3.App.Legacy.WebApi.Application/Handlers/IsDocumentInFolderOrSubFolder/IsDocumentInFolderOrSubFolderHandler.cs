// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.fascicolazione;
using DocsPaVO.Mobile;
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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsDocumentInFolderOrSubFolderRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsDocumentInFolderOrSubFolder;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsDocumentInFolderOrSubFolder
{
    public class IsDocumentInFolderOrSubFolderHandler : IRequestHandler<IsDocumentInFolderOrSubFolderRequest, IsDocumentInFolderOrSubFolderResult>
    {


        protected readonly ILogger<IsDocumentInFolderOrSubFolderHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IPi3DbContext _dbContext;



        private IEnumerable<FascOrFolderBaseInfo> ExtractBaseInfo(List<ExtractInfo>? dataSet)
        {
            List<FascOrFolderBaseInfo> retVal = new List<FascOrFolderBaseInfo>();

            foreach (var row in dataSet)
                retVal.Add(new FascOrFolderBaseInfo()
                {
                    Id =  row.ID_FASC_OR_SOTTOFASC.ToString(),
                    Type = row.FAR_OR_SOTTOFASC.ToString() == "F" ? FascOrFolderBaseInfo.TypeEnum.Fascicolo : FascOrFolderBaseInfo.TypeEnum.Folder
                });

            return retVal;
        }


        private class ExtractInfo
        {
            public long? ID_FASC_OR_SOTTOFASC { get; set; }
            public string FAR_OR_SOTTOFASC { get; set; }
        }


        public IsDocumentInFolderOrSubFolderHandler(
            ILogger<IsDocumentInFolderOrSubFolderHandler> logger,
            IMediator mediator,
            IClaimsPrincipalService claimsPrincipalService,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;
            this._dbContext = dbContext;
        }


        public async Task<IsDocumentInFolderOrSubFolderResult> Handle(IsDocumentInFolderOrSubFolderRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            try
            {
                this._logger.LogDebug("IsDocumentInFolderOrSubFolder");

                List<FascOrFolderBaseInfo> retVal = new List<FascOrFolderBaseInfo>();

                List<long?> inQuery = await this._dbContext.ProjectComponentEntities.AsNoTracking().Where(
                    row => row.LINK == request.idProfile.AsLong()
                    ).Select(pc => pc.PROJECT_ID).ToListAsync();

                List<ExtractInfo>? res = (List<ExtractInfo>?) await this._dbContext.ProjectEntities.AsNoTracking().Where(p => inQuery.Contains(p.SYSTEM_ID) && p.ID_FASCICOLO == p.ID_PARENT).Select(project => new ExtractInfo
                {
                    ID_FASC_OR_SOTTOFASC = project.ID_FASCICOLO,
                    FAR_OR_SOTTOFASC = "F"
                }).Union(
                    this._dbContext.ProjectEntities.AsNoTracking().Where(p => inQuery.Contains(p.SYSTEM_ID) && p.ID_FASCICOLO != p.ID_PARENT).Select(project => new ExtractInfo
                    {
                        ID_FASC_OR_SOTTOFASC = project.ID_FASCICOLO,
                        FAR_OR_SOTTOFASC = "F"
                    })).ToListAsync();

                if(res != null)
                {
                    retVal.AddRange(ExtractBaseInfo(res));
                    if (request.project.folderSelezionato != null) {
                        output = retVal.Where(e => e.Id == request.project.folderSelezionato.systemID).Count() > 0;
                    }
                    else
                    {
                        output = retVal.Where(e => e.Id == request.project.systemID).Count() > 0;
                    }


                }



            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new IsDocumentInFolderOrSubFolderResult(output);
        }


    }
}
