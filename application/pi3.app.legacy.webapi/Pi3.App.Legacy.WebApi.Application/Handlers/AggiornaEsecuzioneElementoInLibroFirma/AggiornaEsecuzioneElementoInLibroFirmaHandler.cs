// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DiagrammaStato;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AggiornaEsecuzioneElementoInLibroFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.AggiornaEsecuzioneElementoInLibroFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AggiornaEsecuzioneElementoInLibroFirma
{
    public class AggiornaEsecuzioneElementoInLibroFirmaHandler : IRequestHandler<AggiornaEsecuzioneElementoInLibroFirmaRequest, AggiornaEsecuzioneElementoInLibroFirmaResult>
    {
        #region Public Members

        public AggiornaEsecuzioneElementoInLibroFirmaHandler(ILogger<AggiornaEsecuzioneElementoInLibroFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AggiornaEsecuzioneElementoInLibroFirmaResult> Handle(AggiornaEsecuzioneElementoInLibroFirmaRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var docnumberAsLong = request.docnumber.AsLong();

                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
                var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var delegatedIdUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);

                var elementoInLibroFirmaEntity = await this._dbContext.ElementoInLibroFirmaEntities.FirstOrDefaultAsync(e => e.DOC_NUMBER == docnumberAsLong && e.DTA_ESECUZIONE == null);
                if (request.eseguitaFirma)
                {
                    elementoInLibroFirmaEntity.DTA_ESECUZIONE = await _dbContext.GetSystemDateTime();
                    elementoInLibroFirmaEntity.STATO_FIRMA = request.stato.ToString();

                    //Salvo nella tabella di storico
                    var corrGlobaliGruppo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();
                    var istanzaProcessoFirmaEntity = await this._dbContext.IstanzaProcessoFirmaEntities.FirstAsync(i => i.ID_DOCUMENTO == docnumberAsLong && i.STATO == "IN_EXEC");
                    var istanzaProcFirmaStoEntity = new IstanzaProcFirmaStoEntity
                    {
                        ID_USER = userId,
                        DOC_NUMBER = istanzaProcessoFirmaEntity.ID_DOCUMENTO,
                        ID_ISTANZA_PROCESSO = istanzaProcessoFirmaEntity.ID_ISTANZA,
                        DTA_DATE = await _dbContext.GetSystemDateTime(),
                        VAR_DESC_AZIONE = request.descAzione,
                        ID_PEOPLE = idUser,
                        ID_RUOLO = corrGlobaliGruppo,
                        ID_PEOPLE_DELEGATO = delegatedIdUser != null ? delegatedIdUser : 0,
                        CHA_CAMBIO_STATO_DIAG = istanzaProcessoFirmaEntity.CHA_CAMBIO_STATO_DIAG
                    };
                    await this._dbContext.IstanzaProcFirmaStoEntities.AddAsync(istanzaProcFirmaStoEntity);
                }
                else
                {
                    elementoInLibroFirmaEntity.ERRORE_FIRMA = request.errore;
                }

                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new AggiornaEsecuzioneElementoInLibroFirmaResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AggiornaEsecuzioneElementoInLibroFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
