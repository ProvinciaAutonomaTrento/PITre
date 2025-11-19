// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.fascicolazione;
using MediatR;
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetFoldersDocument
{
    // Richiede libreria MediatR
    public class FascicolazioneGetFoldersDocumentHandler : IRequestHandler<Application.Requests.FascicolazioneGetFoldersDocument, FascicolazioneGetFoldersDocumentResult>
    {
        #region Public Members

        public FascicolazioneGetFoldersDocumentHandler(ILogger<FascicolazioneGetFoldersDocumentHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;

            this._dbContext = dbContext;
        }

        public async Task<FascicolazioneGetFoldersDocumentResult> Handle(Application.Requests.FascicolazioneGetFoldersDocument request, CancellationToken cancellationToken)
        {
            //DocsPaVO.fascicolazione.Folder[] folders = null;
            List<Folder> folderList = new List<Folder>();
            long systemIdDocument = Convert.ToInt64(request.systemIdDocumento);

            try
            {
                this._dbContext.ProjectEntities
                    .Where(p => p.ID_FASCICOLO != p.ID_PARENT)
                    .Join(this._dbContext.ProjectComponentEntities, p => p.SYSTEM_ID, pc => pc.PROJECT_ID, (p, pc) => new { p, pc })
                    .Where(x => x.pc.LINK == systemIdDocument && x.pc.TYPE.Equals("D"))
                    .ToList()
                    .ForEach(f =>
                    {
                        folderList.Add(new Folder
                        {
                            systemID = f.p.SYSTEM_ID.ToString(),
                            idParent = f.p.ID_PARENT.ToString(),
                            idFascicolo = f.p.ID_FASCICOLO.ToString(),
                            descrizione = f.p.DESCRIPTION.ToString(),
                            dtaApertura = f.p.DTA_APERTURA.ToString().Trim()
                        });
                    });

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new FascicolazioneGetFoldersDocumentResult(folderList.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetFoldersDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        protected readonly IPi3DbContext _dbContext;


        #endregion
    }

}
