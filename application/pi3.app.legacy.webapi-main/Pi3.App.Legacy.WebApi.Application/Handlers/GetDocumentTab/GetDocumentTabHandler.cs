// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using GetDocumentTabRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetDocumentTab;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetDocumentTab
{
    public class GetDocumentTabHandler : IRequestHandler<GetDocumentTabRequest, GetDocumentTabResult>
    {
        #region Public Members

        public GetDocumentTabHandler(ILogger<GetDocumentTabHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetDocumentTabResult> Handle(GetDocumentTabRequest request, CancellationToken cancellationToken)
        {

            DocsPaVO.documento.Tab result = null;
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            long documentId = request.documentId.AsLong();

            try
            {
                var idFascicoliList = await this._dbContext.ProjectEntities.
                    Join(this._dbContext.ProjectComponentEntities, a => a.SYSTEM_ID, b => b.PROJECT_ID, (a, b) => new { a, b }).
                    Where(c => c.b.LINK == documentId).
                    Select(c => c.a.ID_FASCICOLO).ToListAsync();


                var countFoldersInProject = await this._dbContext.ProjectEntities.
                    Where(prj => prj.CHA_TIPO_PROJ == 'F'.ToString() && idFascicoliList.Contains(prj.SYSTEM_ID)).
                    Select(prj => prj.SYSTEM_ID).CountAsync();



                var countDocsInTrasmissions = await this._dbContext.TrasmissioneEntities.
                    Where(a => a.ID_PROFILE == documentId).
                    Select(prj => prj.SYSTEM_ID).CountAsync();


                var countDeleteSecurity = await this._dbContext.DeletedSecurityEntities.
                    Where(s => s.THING == documentId && !this._dbContext.DeletedSecurityEntities.Any(sec => s.THING == sec.THING && sec.ACCESSRIGHTS > 20 && sec.PERSONORGROUP == s.PERSONORGROUP)).
                    Select(s => s).CountAsync();

                bool DeletedSecurityBool = false;

                if (!countDeleteSecurity.ToString().Equals("0"))
                {
                    DeletedSecurityBool = true;
                }


                result = new DocsPaVO.documento.Tab()
                {
                    TransmissionsNumber = countDocsInTrasmissions.ToString(),
                    ClassificationsNumber = countFoldersInProject.ToString(),
                    DeletedSecurity = DeletedSecurityBool


                };


            }
            catch (Exception ex)
            {

                this._logger.LogWebMethodError(ex);
            }

            return new GetDocumentTabResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocumentTabHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}