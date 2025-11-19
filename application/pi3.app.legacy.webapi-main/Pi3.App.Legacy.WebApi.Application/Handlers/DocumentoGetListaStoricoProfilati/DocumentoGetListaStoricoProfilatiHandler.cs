// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.ProfilazioneDinamica;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.ExistsTrasmPendenteSenzaWorkflowDocumento;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoGetListaStoricoProfilatiRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetListaStoricoProfilati;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetListaStoricoProfilati
{
    public class DocumentoGetListaStoricoProfilatiHandler : IRequestHandler<DocumentoGetListaStoricoProfilatiRequest, DocumentoGetListaStoricoProfilatiResult>
    {
        #region Public Members

        public DocumentoGetListaStoricoProfilatiHandler(ILogger<DocumentoGetListaStoricoProfilatiHandler> logger,
          IClaimsPrincipalService claimsPrincipalService,
          IMediator mediator,
          IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DocumentoGetListaStoricoProfilatiResult> Handle(DocumentoGetListaStoricoProfilatiRequest request, CancellationToken cancellationToken)
        {
            StoricoProfilati[] output = null;

            try
            {
                var idTemplate = request.id_tipo_atto.AsLong();
                var idProfile = request.doc_number.AsLong();
                var idRuolo = request.idGroup.AsLong();

                var profileStoEntity = await this._dbContext.ProfilStoEntities.AsNoTracking()
                    .Join(this._dbContext.AROggCustomDocEntities, profil => profil.ID_OGG_CUSTOM, assOggRuolo => assOggRuolo.ID_OGGETTO_CUSTOM, (profil, assOggRuolo) => new { profil, assOggRuolo })
                    .Join(this._dbContext.PeopleEntities.AsNoTracking(), j => j.profil.ID_PEOPLE, people => people.SYSTEM_ID, (j, people) => new {j.profil, j.assOggRuolo, people})
                    .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(), j => j.profil.ID_RUOLO_IN_UO, ruolo => ruolo.SYSTEM_ID, (j, ruolo) => new {j.profil, j.assOggRuolo, j.people, ruolo})
                    .Join(this._dbContext.OggettiCustomEntities.AsNoTracking(), j => j.profil.ID_OGG_CUSTOM, ogg => ogg.SYSTEM_ID, (j, ogg) => new { j.profil, j.assOggRuolo, j.people, j.ruolo, ogg })
                    .Where(j => j.profil.ID_TEMPLATE == j.assOggRuolo.ID_TEMPLATE && j.assOggRuolo.ID_RUOLO == idRuolo && j.profil.ID_PROFILE == idProfile && j.profil.ID_TEMPLATE == idTemplate)
                    .Select(j => new
                    {
                        j.profil.DTA_MODIFICA,
                        j.profil.VAR_DESC_MODIFICA,
                        j.profil.ID_OGG_CUSTOM,
                        DESCRIZIONE_OGGETTO = j.ogg.DESCRIZIONE,
                        j.people.VAR_COGNOME,
                        j.people.VAR_NOME,
                        VRA_DESC_RUOLO = j.ruolo.VAR_DESC_CORR
                    })
                    .OrderByDescending(p => p.DTA_MODIFICA)
                    .ThenBy(p => p.ID_OGG_CUSTOM)
                    .ToListAsync();

                if(profileStoEntity != null)
                {
                    output = new StoricoProfilati[profileStoEntity.Count];
                    for (int i = 0; i < profileStoEntity.Count; i++)
                    {
                        output[i] = new StoricoProfilati()
                        {
                            dta_modifica = profileStoEntity[i].DTA_MODIFICA.AsDateTimeFormat(),
                            var_desc_modifica = profileStoEntity[i].VAR_DESC_MODIFICA,
                            utente = new DocsPaVO.utente.Utente()
                            {
                                descrizione = string.Format("{0} {1}", profileStoEntity[i].VAR_COGNOME, profileStoEntity[i].VAR_NOME)
                            },
                            ruolo = new DocsPaVO.utente.Ruolo()
                            {
                                descrizione = profileStoEntity[i].VRA_DESC_RUOLO
                            },
                            oggetto = new OggettoCustom()
                            {
                                DESCRIZIONE = profileStoEntity[i].DESCRIZIONE_OGGETTO
                            }
                        };
                    }
                }
                else
                {
                    output = new StoricoProfilati[0];
                }
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new DocumentoGetListaStoricoProfilatiResult(output);
        }

        #endregion

        #region Private Members

        protected ILogger<DocumentoGetListaStoricoProfilatiHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected IMediator _mediator;
        protected IPi3DbContext _dbContext;

        #endregion
    }
}
