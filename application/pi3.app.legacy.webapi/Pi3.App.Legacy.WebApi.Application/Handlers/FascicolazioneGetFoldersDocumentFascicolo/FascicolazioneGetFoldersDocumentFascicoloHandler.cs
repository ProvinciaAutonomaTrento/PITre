// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.fascicolazione;
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
using FascicolazioneGetFoldersDocumentFascicoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneGetFoldersDocumentFascicolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetFoldersDocumentFascicolo
{
    public class FascicolazioneGetFoldersDocumentFascicoloHandler : IRequestHandler<FascicolazioneGetFoldersDocumentFascicoloRequest, FascicolazioneGetFoldersDocumentFascicoloResult>
    {
        protected readonly ILogger<FascicolazioneGetFoldersDocumentFascicoloHandler> _logger;
        protected readonly IPi3DbContext _dbContext;

        public FascicolazioneGetFoldersDocumentFascicoloHandler(
            ILogger<FascicolazioneGetFoldersDocumentFascicoloHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }


        public async Task<FascicolazioneGetFoldersDocumentFascicoloResult> Handle(FascicolazioneGetFoldersDocumentFascicoloRequest request,CancellationToken cancellationToken)
        {
            List<Folder> output = new();
            try
            {
                var folderEnts = await (from p in this._dbContext.ProjectEntities.AsNoTracking()
                                  from pc in this._dbContext.ProjectComponentEntities.AsNoTracking()
                                  where (pc.PROJECT_ID == p.SYSTEM_ID) &&
                                  (pc.LINK == request.systemIdDocumento.AsLong()) &&
                                  (pc.TYPE != null && pc.TYPE.Equals("D")) &&
                                  (p.ID_FASCICOLO != p.ID_PARENT) &&
                                  (p.ID_FASCICOLO == request.systemIdFascicolo.AsLong())
                                  orderby p.SYSTEM_ID ascending
                                  select new
                                  {
                                      ID_FOLDER = p.SYSTEM_ID ,
                                      ID_PARENT_FOLDER = p.ID_PARENT,
                                      ID_FASCICOLO = p.ID_FASCICOLO ,
                                      FOLDER_DESCRIPTION = p.DESCRIPTION ,
                                      p.CHA_TIPO_FASCICOLO,
                                      APERTURA = p.DTA_APERTURA 
                                  }).ToListAsync();

                folderEnts.ForEach((row) =>
                {
                    output.Add(new Folder()
                    {
                        systemID = row.ID_FOLDER.ToString(),
                        idParent = row.ID_PARENT_FOLDER != null ? row.ID_PARENT_FOLDER.ToString() : null,
                        idFascicolo = row.ID_FASCICOLO != null ? row.ID_FASCICOLO.ToString() : null,
                        descrizione = row.FOLDER_DESCRIPTION,
                        dtaApertura = row.APERTURA.HasValue ? row.APERTURA.HasValue.ToString().Trim() : null,
                        livello = "",
                        codicelivello = ""
                    });
                });
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output.ToArray());
        }


    }
}
