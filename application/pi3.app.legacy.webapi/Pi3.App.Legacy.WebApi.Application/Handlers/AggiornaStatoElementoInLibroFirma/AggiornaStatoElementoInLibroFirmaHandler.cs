// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.AggiornaEsecuzioneElementoInLibroFirma;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using AggiornaStatoElementoInLibroFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.AggiornaStatoElementoInLibroFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AggiornaStatoElementoInLibroFirma
{

    public class AggiornaStatoElementoInLibroFirmaHandler : IRequestHandler<AggiornaStatoElementoInLibroFirmaRequest, AggiornaStatoElementoInLibroFirmaResult>
    {
        #region Public Members

        public AggiornaStatoElementoInLibroFirmaHandler(ILogger<AggiornaStatoElementoInLibroFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            ITrasmissioneRepository trasmissioneRepository,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._trasmissioneRepository = trasmissioneRepository;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<AggiornaStatoElementoInLibroFirmaResult> Handle(AggiornaStatoElementoInLibroFirmaRequest request, CancellationToken cancellationToken)
        {
            var output = false;
            var message = string.Empty;
            try
            {
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var delegatedIdUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);
                var idGruppo = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

                var idElementoAsLong = request.elemento.IdElemento.AsLong();

                var elementoInLibroFirmaEntity = await this._dbContext.ElementoInLibroFirmaEntities.FirstOrDefaultAsync(e => e.ID_ELEMENTO == idElementoAsLong);

                if (elementoInLibroFirmaEntity == null)
                    throw new ElementoNotFoundPi3Exception(idElementoAsLong);

                if (elementoInLibroFirmaEntity.ID_UTENTE_LOCKER != null && elementoInLibroFirmaEntity.ID_UTENTE_LOCKER != idUser)
                {
                    //L'elemento è stato preso in carico da un altro utente
                    var oggetto = await this._dbContext.ProfileEntities.AsNoTracking()
                        .Where(p => p.SYSTEM_ID == elementoInLibroFirmaEntity.DOC_NUMBER)
                        .Select(p => p.VAR_PROF_OGGETTO)
                        .FirstOrDefaultAsync();

                    var descUtenteLocker = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == elementoInLibroFirmaEntity.ID_UTENTE_LOCKER)
                        .Select(c => c.VAR_DESC_CORR)
                        .FirstOrDefaultAsync();

                    message = elementoInLibroFirmaEntity.ID_ELEMENTO.ToString() + "@" + oggetto + "@" + descUtenteLocker + "#";
                }
                else
                {
                    elementoInLibroFirmaEntity.STATO_FIRMA = request.nuovoStato;
                    elementoInLibroFirmaEntity.ID_UTENTE_LOCKER = request.nuovoStato.Equals(TipoStatoElemento.PROPOSTO.ToString()) || request.nuovoStato.Equals(TipoStatoElemento.IN_SOSPESO.ToString()) ? null : idUser;

                    if (elementoInLibroFirmaEntity.MODALITA == "A")
                    {
                        var istanzaProcessoFirma = await this._dbContext.IstanzaProcessoFirmaEntities
                            .Where(i => i.ID_ISTANZA == elementoInLibroFirmaEntity.ISTANZA_PROCESSO)
                            .FirstOrDefaultAsync();
                        if (istanzaProcessoFirma != null)
                        {
                            istanzaProcessoFirma.MOTIVO_RESPINGIMENTO = request.elemento.MotivoRespingimento;
                        }
                    }

                    await ((DbContext)_dbContext).SaveChangesAsync();
                }

                //Accetto la trasmissione
                if (string.IsNullOrEmpty(request.elemento.DataAccettazione) && request.nuovoStato != TipoStatoElemento.NO_COMPETENZA.ToString())
                {
                    var idTrasmSingola = request.elemento.IdTrasmSingola.AsLong();

                    var idTrasmissione = await this._dbContext.TrasmSingolaEntities.Where(s => s.SYSTEM_ID == idTrasmSingola).Select(s => s.ID_TRASMISSIONE).FirstAsync();
                    var aggregate = await this._trasmissioneRepository.Get(idTenant, idTrasmissione.ToString());

                    aggregate.Accetta(idGruppo.ToString(), idUser.ToString(), new Accetta()
                    {
                        Data = await _dbContext.GetSystemDateTime(),
                        IdDelegato = delegatedIdUser != 0 ? delegatedIdUser.ToString() : null
                    });

                    await _trasmissioneRepository.Update(aggregate);

                    await this._webMethodLoggerService.LogOK("ACCEPTTRASMDOCUMENT", 
                        request.elemento.InfoDocumento.Docnumber,
                        string.Format(Descriptions.LogAccettazioneDocumento, request.elemento.InfoDocumento.Docnumber));

                }

                output = true;

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                message = ex.Message;
                output = false;
            }

            return new AggiornaStatoElementoInLibroFirmaResult(output, message);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AggiornaStatoElementoInLibroFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }
}
