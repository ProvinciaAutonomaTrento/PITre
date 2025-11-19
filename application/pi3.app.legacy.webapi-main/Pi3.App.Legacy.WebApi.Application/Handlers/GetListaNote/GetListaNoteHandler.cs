// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetListaNote
{
    // Richiede libreria MediatR
    public class GetListaNoteHandler : IRequestHandler<Application.Requests.GetListaNote, GetListaNoteResult>
    {
        #region Public Members

        public GetListaNoteHandler(ILogger<GetListaNoteHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetListaNoteResult> Handle(Application.Requests.GetListaNote request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.Note.NotaElenco> result = new List<DocsPaVO.Note.NotaElenco>();
            int numNote = 0;
            string idRF = request.idRF;
            string descNota = request.descNota;
            var idGroupAsString = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup).ToString();
            var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            try
            {
                var idCorrGlobaliRuolo = this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == idGroup).Select(x => x.SYSTEM_ID).FirstOrDefault();
                if (idCorrGlobaliRuolo == null)
                    throw new CorrGlobaliByGroupIdNotFoundPi3Exception(idGroup);

                ArrayList listaRegRFRuolo = new ArrayList();
                listaRegRFRuolo = GetListaRegistriRfRuolo(idCorrGlobaliRuolo); 

                switch(idRF) 
                {
                    case "T":
                        numNote = string.IsNullOrEmpty(descNota) ? this._dbContext.ElencoNoteEntities.Count(x => listaRegRFRuolo.Contains(x.ID_REG_RF)) : this._dbContext.ElencoNoteEntities.Count(x => listaRegRFRuolo.Contains(x.ID_REG_RF) && x.VAR_DESC_NOTA.ToUpper().Contains(descNota.ToUpper()));
                        break;
                    default:
                        numNote = string.IsNullOrEmpty(descNota) ? this._dbContext.ElencoNoteEntities.Count(x => x.ID_REG_RF == idRF.AsLong()) : this._dbContext.ElencoNoteEntities.Count(x => x.ID_REG_RF == idRF.AsLong() && x.VAR_DESC_NOTA.ToUpper().Contains(descNota.ToUpper()));
                        break;
                }

                if(numNote > 0)
                {
                    List<ElencoNoteEntity> elencoNote = new List<ElencoNoteEntity>();
                    switch (idRF)
                    {
                        case "T":
                            elencoNote = string.IsNullOrEmpty(descNota) ? this._dbContext.ElencoNoteEntities.Where(x => listaRegRFRuolo.Contains(x.ID_REG_RF)).ToList() : this._dbContext.ElencoNoteEntities.Where(x => listaRegRFRuolo.Contains(x.ID_REG_RF) && x.VAR_DESC_NOTA.ToUpper().Contains(descNota.ToUpper())).ToList();
                            break;
                        default:
                            elencoNote = string.IsNullOrEmpty(descNota) ? this._dbContext.ElencoNoteEntities.Where(x => x.ID_REG_RF == idRF.AsLong()).ToList() : this._dbContext.ElencoNoteEntities.Where(x => x.ID_REG_RF == idRF.AsLong() && x.VAR_DESC_NOTA.ToUpper().Contains(descNota.ToUpper())).ToList();
                            break;
                    }

                    //ordino e creo note
                    elencoNote.OrderBy(x => x.ID_REG_RF).ThenBy(x => x.VAR_DESC_NOTA).ToList().ForEach(n => result.Add(new DocsPaVO.Note.NotaElenco()
                    {
                        idNota = n.SYSTEM_ID.ToString(),
                        descNota = n.VAR_DESC_NOTA,
                        codRegRf = n.COD_REG_RF,
                        idRegRf = n.ID_REG_RF.ToString()
                    }));
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                numNote = 0;
            }

            return new GetListaNoteResult(result.ToArray(), numNote);
        }


        #endregion

        #region Private Members

        protected readonly ILogger<GetListaNoteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        private ArrayList GetListaRegistriRfRuolo(long idCorrGlobaliRuolo)
        {
            ArrayList listaRegRf = new ArrayList();
            var join = this._dbContext.RuoloRegistroEntities.Join(this._dbContext.RegistroEntities, rreg => rreg.ID_REGISTRO, reg => reg.SYSTEM_ID, (rreg, reg) => new { rreg, reg });

            var query = join.Where(x => x.rreg.ID_RUOLO_IN_UO == idCorrGlobaliRuolo && x.reg.CHA_RF.Equals("1")).Select(x => new
            {
                SYSTEM_ID = x.reg.SYSTEM_ID,
                VAR_CODICE = x.reg.VAR_CODICE,
                CHA_PREFERITO = x.rreg.CHA_PREFERITO
            }).OrderByDescending(x => x.CHA_PREFERITO == null).ThenBy(x => x.VAR_CODICE).ToList();

            query.ForEach(x =>
            {
                listaRegRf.Add(x.SYSTEM_ID.ToString());
            });

            return listaRegRf;
        }

        #endregion
    }

}
