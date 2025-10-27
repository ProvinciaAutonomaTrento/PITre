// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using PulisciUnitaOrganizzativaRequest = Pi3.App.Legacy.WebApi.Application.Requests.PulisciUnitaOrganizzativa;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.PulisciUnitaOrganizzativa
{
    public class PulisciUnitaOrganizzativaHandler : IRequestHandler<PulisciUnitaOrganizzativaRequest, PulisciUnitaOrganizzativaResult>
    {
        #region Public Members

        public PulisciUnitaOrganizzativaHandler(ILogger<PulisciUnitaOrganizzativaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<PulisciUnitaOrganizzativaResult> Handle(PulisciUnitaOrganizzativaRequest request, CancellationToken cancellationToken)
        {

            bool output = false;

            long idUo = request.idUo.AsLong();

            var tipoUrp = "R";
            var CHA_TIPO_IE = "I";

            IQueryable<CorrGlobaliEntity> CorrGlobaliEntity = this._dbContext.CorrGlobaliEntities
                .Where(x => x.CHA_TIPO_URP.ToUpper() == tipoUrp
                && x.CHA_TIPO_IE == CHA_TIPO_IE
                && x.ID_UO == idUo
                && x.DTA_FINE == null);


            List<Ruolo>? listaRuoli = await GetListaRuoli(idUo, CorrGlobaliEntity);

            output = await PulisciUnitaOrganizzativa(listaRuoli, idUo);


            return new PulisciUnitaOrganizzativaResult(output);

        }


        #endregion

        #region Private Members

        protected readonly ILogger<PulisciUnitaOrganizzativaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        private async Task<List<Ruolo>?> GetListaRuoli(long idUo, IQueryable<CorrGlobaliEntity> entity)
        {
            var tipoUrp = "R";
            var tipoIE = "I";

            return await entity.Where(x => x.CHA_TIPO_URP == tipoUrp && x.CHA_TIPO_IE == tipoIE && x.ID_UO == idUo && x.DTA_FINE == null).Select(x => new Ruolo() {
                systemId = x.SYSTEM_ID.ToString(),
                idGruppo = x.ID_GRUPPO.ToString() }).ToListAsync();

        }


        protected virtual async Task<bool> PulisciUnitaOrganizzativa(List<Ruolo>? listaRuoli, long idUo)
        {
            var result = true;
            try
            {
                foreach (Ruolo r in listaRuoli)
                {
                    long idGruppo = r.idGruppo.AsLong();
                    long systemId = r.systemId.AsLong();

                    var NotifyEntity = await _dbContext.NotifyEntities.Where(w => w.ID_GROUP_RECEIVER == idGruppo).ToListAsync();
                    if (NotifyEntity != null)
                        _dbContext.NotifyEntities.RemoveRange(NotifyEntity);

                    var UltimiDocVisualizzatiEntity = await _dbContext.UltimiDocVisualizzatiEntities.Where(w => w.ID_GRUPPO == idGruppo).ToListAsync();
                    if (UltimiDocVisualizzatiEntity != null)
                        _dbContext.UltimiDocVisualizzatiEntities.RemoveRange(UltimiDocVisualizzatiEntity);

                    var AreaLavoroEntity = await _dbContext.AreaLavoroEntities.Where(w => w.ID_RUOLO_IN_UO == systemId).ToListAsync();
                    if (AreaLavoroEntity != null)
                        _dbContext.AreaLavoroEntities.RemoveRange(AreaLavoroEntity);

                    var ElementoInLibroFirmaEntity = await _dbContext.ElementoInLibroFirmaEntities.Where(w => w.ID_RUOLO_TITOLARE == idGruppo).ToListAsync();
                    if (ElementoInLibroFirmaEntity != null)
                        _dbContext.ElementoInLibroFirmaEntities.RemoveRange(ElementoInLibroFirmaEntity);

                    var idsIstanzaProcessoFirmaEntity = await _dbContext.IstanzaProcessoFirmaEntities.AsNoTracking().Where(z => z.ID_RUOLO_PROPONENTE == idGruppo).Select(z => z.ID_ISTANZA).ToListAsync();
                    var IstanzaPassoFirmaEntity = _dbContext.IstanzaPassoFirmaEntities.Where(w => idsIstanzaProcessoFirmaEntity.Contains(w.ID_ISTANZA_PROCESSO)).ToListAsync();
                    if (IstanzaPassoFirmaEntity != null)
                        _dbContext.ElementoInLibroFirmaEntities.RemoveRange(ElementoInLibroFirmaEntity);

                    var IstanzaProcessoFirmaEntity = await _dbContext.IstanzaProcessoFirmaEntities.AsNoTracking().Where(z => z.ID_RUOLO_PROPONENTE == idGruppo).ToListAsync();
                    if (IstanzaProcessoFirmaEntity != null)
                        _dbContext.IstanzaProcessoFirmaEntities.RemoveRange(IstanzaProcessoFirmaEntity);
                }


                var profileEntity = await _dbContext.ProfileEntities
                        .Where(p => p.ID_UO_CREATORE == idUo && (p.CHA_IN_CESTINO ?? "0") == "0" )
                        .ToListAsync();

                foreach (var x in profileEntity)
                    x.CHA_IN_CESTINO = "1";

                ((DbContext)_dbContext).SaveChanges();
            }
            catch (Exception ex)
            {
                result = false;
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return result;
        }

        #endregion
    }
}