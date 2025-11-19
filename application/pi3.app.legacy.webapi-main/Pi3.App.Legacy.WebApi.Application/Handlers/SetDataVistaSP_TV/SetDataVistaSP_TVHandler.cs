// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Handlers.getModelliPerTrasmLite;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SetDataVistaSP_TVRequest = Pi3.App.Legacy.WebApi.Application.Requests.SetDataVistaSP_TV;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SetDataVistaSP_TV
{
    public class SetDataVistaSP_TVHandler : IRequestHandler<SetDataVistaSP_TVRequest, SetDataVistaSP_TVResult>
    {
        #region Public Members

        public SetDataVistaSP_TVHandler(ILogger<SetDataVistaSP_TVHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            ITrasmissioneRepository trasmissioneRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._trasmissioneRepository = trasmissioneRepository;
        }

        public async Task<SetDataVistaSP_TVResult> Handle(SetDataVistaSP_TVRequest request, CancellationToken cancellationToken)
        {
            bool output = true;
            var segnatura = string.Empty;
            List<TrasmSingolaEntity> trasmSingolaEntities = null;
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);

            try
            {
                var idObject = request.docNumber.AsLong();
                var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                var idCorrGlobaliGruppo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstAsync();
                var idCorrGlobaliPeople = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_PEOPLE == idPeople).Select(c => c.SYSTEM_ID).FirstAsync();

                if (request.docOrFasc.Equals("D"))
                {
                    var profileEntity = await this._dbContext.ProfileEntities.AsNoTracking()
                        .Where(p => p.DOCNUMBER == idObject)
                        .Select(p => new {p.SYSTEM_ID, p.VAR_SEGNATURA})
                        .FirstOrDefaultAsync();

                    if(profileEntity != null)
                    {
                        idObject = profileEntity.SYSTEM_ID;
                        segnatura = profileEntity.VAR_SEGNATURA;
                    }

                    trasmSingolaEntities = await this._dbContext.TrasmissioneEntities.AsNoTracking()
                        .Join(this._dbContext.TrasmSingolaEntities.AsNoTracking(), t => t.SYSTEM_ID, ts => ts.ID_TRASMISSIONE, (t, ts) => new { t, ts })
                        .Join(this._dbContext.RagioneTrasmissioneEntities.AsNoTracking(), j => j.ts.ID_RAGIONE, r => r.SYSTEM_ID, (j, r) => new
                        {
                            j.t,
                            j.ts,
                            r.CHA_TIPO_RAGIONE
                        })
                        .Where(j => j.t.ID_PROFILE == idObject && j.t.DTA_INVIO.HasValue
                                && (j.ts.ID_CORR_GLOBALE == idCorrGlobaliGruppo || j.ts.ID_CORR_GLOBALE == idCorrGlobaliPeople)
                                && (j.CHA_TIPO_RAGIONE == "N" || j.CHA_TIPO_RAGIONE == "I" || j.CHA_TIPO_RAGIONE == "S"))
                        .Select(j => j.ts)
                        .ToListAsync();
                }
                else
                {
                    trasmSingolaEntities = await this._dbContext.TrasmissioneEntities.AsNoTracking()
                        .Join(this._dbContext.TrasmSingolaEntities.AsNoTracking(), t => t.SYSTEM_ID, ts => ts.ID_TRASMISSIONE, (t, ts) => new { t, ts })
                        .Join(this._dbContext.RagioneTrasmissioneEntities.AsNoTracking(), j => j.ts.ID_RAGIONE, r => r.SYSTEM_ID, (j, r) => new
                        {
                            j.t,
                            j.ts,
                            r.CHA_TIPO_RAGIONE
                        })
                        .Where(j => j.t.ID_PROJECT == idObject && j.t.DTA_INVIO.HasValue
                                && (j.ts.ID_CORR_GLOBALE == idCorrGlobaliGruppo || j.ts.ID_CORR_GLOBALE == idCorrGlobaliPeople)
                                && (j.CHA_TIPO_RAGIONE == "N" || j.CHA_TIPO_RAGIONE == "I" || j.CHA_TIPO_RAGIONE == "S"))
                        .Select(j => j.ts)
                        .ToListAsync();
                }

                foreach (var trasm in trasmSingolaEntities)
                {
                    var inToDoList = await this._dbContext.TrasmUtenteEntities.AsNoTracking()
                        .AnyAsync(tu => tu.ID_TRASM_SINGOLA == trasm.SYSTEM_ID && tu.CHA_IN_TODOLIST == "1" && tu.ID_PEOPLE == idPeople);

                    if (inToDoList)
                    {
                        var aggregate = await _trasmissioneRepository.Get(idTenant, trasm.ID_TRASMISSIONE.ToString());

                        aggregate.Visto(idGruppo.ToString(), idPeople.ToString(), new Visto()
                        {
                            Data = DateTime.Now,
                            IdDelegato = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser),
                            UserIdDelegato = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserId),
                            NomeDelegato = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserName),
                            CognomeDelegato = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserSurname)
                        });

                        await this._trasmissioneRepository.Update(aggregate);
                    }
                }

                // Se il documento è stato ricevuto per IS, bisogna selezionare gli RF del ruolo che sono
                // destinatari della spedizione e per questi inviare una conferma di ricezione al mittente
                if (request.docOrFasc.Equals("D") 
                    && await this._dbContext.SimpInteropReceivedMessageEntities.AsNoTracking().AnyAsync(s => s.PROFILEID == idObject)
                    && !string.IsNullOrEmpty(segnatura))
                {
                    //TODO
                }
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                output = false;
            }

            var webMethodName = request.docOrFasc.Equals("F") ? "CHECKTRASMFOLDER" : "CHECKTRASMDOCUMENT";
            var objectDescription = request.docOrFasc.Equals("F") ? string.Format(Resources.LogDescriptionCheckTrasmFolder, request.docNumber, userId) : 
                string.Format(Resources.LogDescriptionCheckTrasmDocument, request.docNumber, userId);

            if (output)
                await this._webMethodLoggerService.LogOK(webMethodName, request.docNumber, objectDescription);
            else
                await this._webMethodLoggerService.LogKO(webMethodName, request.docNumber, objectDescription);

            return new SetDataVistaSP_TVResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SetDataVistaSP_TVHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected ITrasmissioneRepository _trasmissioneRepository;

        #endregion
    }
}
