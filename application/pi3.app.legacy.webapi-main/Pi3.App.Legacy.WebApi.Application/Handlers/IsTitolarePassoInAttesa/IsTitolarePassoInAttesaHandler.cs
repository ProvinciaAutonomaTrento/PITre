// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.LibroFirma;
using DocsPaVO.Mobile;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.utente;
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
using IsTitolarePassoInAttesaRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsTitolarePassoInAttesa;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsTitolarePassoInAttesa
{
    public class IsTitolarePassoInAttesaHandler : IRequestHandler<IsTitolarePassoInAttesaRequest, IsTitolarePassoInAttesaResult>
    {
        #region Public Members

        public IsTitolarePassoInAttesaHandler(ILogger<IsTitolarePassoInAttesaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<IsTitolarePassoInAttesaResult> Handle(IsTitolarePassoInAttesaRequest request, CancellationToken cancellationToken)
        {
            bool output = true;

            try
            {
                long idDocumento = request.docNumber.AsLong();
                long idUtenteCoinvolto = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
                long idRuoloCoinvolto = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
                string azioneRichiesta = request.azione.ToString();

                var istanzaProcessoFirmaEntity = await this._dbContext.IstanzaProcessoFirmaEntities.AsNoTracking()
                    .Join(this._dbContext.IstanzaPassoFirmaEntities, processo => processo.ID_ISTANZA, passo => passo.ID_ISTANZA_PROCESSO, (processo, passo) => new { processo, passo })
                    .Where(p => p.passo.STATO_PASSO == "LOOK" && p.processo.ID_DOCUMENTO == idDocumento && p.processo.STATO == "IN_EXEC")
                    .Select(p => new
                    {
                        p.passo.ID_RUOLO_COINVOLTO,
                        p.passo.ID_UTENTE_COINVOLTO,
                        p.passo.ID_UTENTE_LOCKER,
                        p.passo.TIPO_FIRMA
                    })
                    .FirstOrDefaultAsync();
                    
                if(istanzaProcessoFirmaEntity != null)
                {
                    if (istanzaProcessoFirmaEntity.ID_RUOLO_COINVOLTO != idRuoloCoinvolto 
                        || (istanzaProcessoFirmaEntity.ID_UTENTE_COINVOLTO > 0 && istanzaProcessoFirmaEntity.ID_UTENTE_COINVOLTO != idUtenteCoinvolto)
                        || (istanzaProcessoFirmaEntity.ID_UTENTE_LOCKER > 0 && istanzaProcessoFirmaEntity.ID_UTENTE_COINVOLTO != idUtenteCoinvolto)
                        || istanzaProcessoFirmaEntity.TIPO_FIRMA != azioneRichiesta)
                        output = false;
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new IsTitolarePassoInAttesaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsTitolarePassoInAttesaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
