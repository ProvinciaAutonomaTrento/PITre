// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma;
using Pi3.App.Legacy.WebApi.Application.Handlers.SalvaModificaStatoStartSignatureProcess;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SalvaModificaStato
{
    public class SalvaModificaStatoHandler : IRequestHandler<salvaModificaStato, salvaModificaStatoResult>
    {
        #region Public members
        public SalvaModificaStatoHandler(ILogger<SalvaModificaStatoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._configurationService = configurationService;
        }

        public async Task<salvaModificaStatoResult> Handle(salvaModificaStato request, CancellationToken cancellationToken)
        {
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, true);

            var idCorrGlobali = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(x => x.ID_GRUPPO == idGroup).Select(x => x.SYSTEM_ID).FirstAsync();

            StatoEntity? statoEntity = null;

            try
            {
                var diagrammiEntity = await this._dbContext.DiagrammiEntities
                    .FirstOrDefaultAsync(x => x.DOC_NUMBER == request.docNumber.AsLong());

                // statoOld
                string? currentState = default;

                statoEntity = await this._dbContext.StatoEntities.AsNoTracking().FirstOrDefaultAsync(x => x.SYSTEM_ID == request.idStato.AsLong());

                if (diagrammiEntity != null)
                {
                    this._logger.LogDebug($"salvaModificaStato - diagrammiEntity trovato. diagrammiEntity.SYSTEM_ID: {diagrammiEntity.SYSTEM_ID}, diagrammiEntity.ID_STATO: {diagrammiEntity.ID_STATO}, request.idStato: {request.idStato}, Instance: {instance}, IdTenant: {idTenant}, IdUser: {idPeople}, IdGroup: {idGroup}");

                    currentState = await this._dbContext.StatoEntities
                        .Where(x => x.SYSTEM_ID == diagrammiEntity.ID_STATO)
                        .Select(x => x.VAR_DESCRIZIONE)
                        .FirstAsync();

                    var trasmDiagrEntity = await this._dbContext.TrasmDiagrEntities.FirstOrDefaultAsync(x => x.ID_STATO == diagrammiEntity.ID_STATO && x.DOC_NUMBER == diagrammiEntity.DOC_NUMBER);

                    if (trasmDiagrEntity != null)
                        this._dbContext.TrasmDiagrEntities.Remove(trasmDiagrEntity);
                    
                    this._logger.LogDebug($"salvaModificaStato - diagrammiEntity.ID_STATO: {diagrammiEntity.ID_STATO}, request.idStato: {request.idStato}, Instance: {instance}, IdTenant: {idTenant}, IdUser: {idPeople}, IdGroup: {idGroup}");

                    if (diagrammiEntity.ID_STATO != request.idStato.AsLong())
                        diagrammiEntity.ID_STATO = request.idStato.AsLong();                    
                }
                else
                {
                    this._logger.LogDebug($"salvaModificaStato - diagrammiEntity non trovato, Instance: {instance}, IdTenant: {idTenant}, IdUser: {idPeople}, IdGroup: {idGroup}");

                    currentState = statoEntity.VAR_DESCRIZIONE;

                    diagrammiEntity = new DiagrammiEntity
                    {
                        DOC_NUMBER = request.docNumber.AsLong(),
                        ID_STATO = request.idStato.AsLong(),
                        ID_DIAGRAMMA = request.diagramma.SYSTEM_ID
                    };

                    await this._dbContext.DiagrammiEntities.AddAsync(diagrammiEntity);

                    //Se stato iniziale serve impostare la data di scadenza
                    if(statoEntity.STATO_INIZIALE == 1)
                    {
                        var idTipoDoc = await this._dbContext.ProfileEntities.AsNoTracking()
                            .Where(x => x.DOCNUMBER == request.docNumber.AsLong())
                            .Select(x => x.ID_TIPO_ATTO)
                            .FirstOrDefaultAsync();

                        await this._mediator.Send(new Application.Requests.salvaDataScadenzaDoc(request.docNumber, request.dataScadenza, idTipoDoc.ToString()));
                    }
                }

                // Inserimento storico
                var diagrammiStoEntity = new DiagrammiStoEntity
                {
                    ID_USER = request.idUtente,
                    ID_PEOPLE = idPeople,
                    ID_RUOLO = idCorrGlobali,
                    DTA_DATE = await _dbContext.GetSystemDateTime(),
                    DOC_NUMBER = request.docNumber.AsLong(),
                    VAR_DESC_OLD_STATO = currentState,
                    VAR_DESC_NEW_STATO = statoEntity.VAR_DESCRIZIONE,
                    ID_PEOPLE_DELEGATO = request.user?.delegato != null ? request.user.delegato.idPeople.AsLong() : 0
                };

                await this._dbContext.DiagrammiStoEntities.AddAsync(diagrammiStoEntity);

                // Libro firma
                if(statoEntity.ID_PROCESSO_FIRMA.HasValue 
                    && !(await this._dbContext.SchemaProcessoFirmaEntities.AnyAsync(x => x.ID_PROCESSO == statoEntity.ID_PROCESSO_FIRMA && x.CHA_MODELLO == "1")))
                {

                    var processo = (await this._mediator.Send(new Application.Requests.GetProcessoDiFirma(statoEntity.ID_PROCESSO_FIRMA.ToString(), request.user))).output;
                    var fileRequest = (await this._mediator.Send(new Application.Requests.GetVersionsMainDocument(request.user, request.docNumber))).output;
                    var avvioProcesso = await this._mediator.Send(new Application.Requests.AvvioProcessoDiFirma(processo,
                        fileRequest[0], request.user,
                        "A", string.Empty,
                        new OpzioniNotifica()
                        {
                            Notifica_interrotto = true,
                            Notifica_concluso = false
                        }, 
                        true));
                    if(!avvioProcesso.output)
                    {
                        throw new ErroreAvvioProcessoFirmaPi3Exception();
                    }
                }

                // Consolidamento
                var key = await this._configurationService.GetValue<string>("BE_CONSOLIDAMENTO");
                if (key == "1") 
                {
                    if(!string.IsNullOrWhiteSpace(statoEntity.STATO_CONSOLIDAMENTO) && statoEntity.STATO_CONSOLIDAMENTO != "0")
                    {
                        var profileEntity = await this._dbContext.ProfileEntities.FirstAsync(x => x.SYSTEM_ID == request.docNumber.AsLong());

                        var currentConsolidationState = profileEntity.CONSOLIDATION_STATE ?? "0";

                        if(currentConsolidationState.AsLong() >= statoEntity.STATO_CONSOLIDAMENTO.AsLong())
                        {
                            this._logger.LogWarning(Resources.LogAlreadyConsolidated, statoEntity.VAR_DESCRIZIONE, request.docNumber,
                                DocumentConsolidationStateDescriptionAttribute.GetDescription((DocumentConsolidationStateEnum)statoEntity.STATO_CONSOLIDAMENTO.AsLong()),
                                DocumentConsolidationStateDescriptionAttribute.GetDescription((DocumentConsolidationStateEnum)currentConsolidationState.AsLong()));
                        }
                        else
                        {
                            await this._mediator.Send(new Application.Requests.ConsolidateDocumentById(
                                request.user,
                                request.docNumber,
                                (DocumentConsolidationStateEnum)statoEntity.STATO_CONSOLIDAMENTO.AsLong()));

                            this._logger.LogDebug(Resources.LogConsolidationOK, statoEntity.VAR_DESCRIZIONE, request.docNumber,
                                DocumentConsolidationStateDescriptionAttribute.GetDescription((DocumentConsolidationStateEnum)statoEntity.STATO_CONSOLIDAMENTO.AsLong()));
                        }
                    }
                }

                var affectedRows = await ((DbContext)this._dbContext).SaveChangesAsync();

                this._logger.LogDebug($"salvaModificaStato - affectedRows: {affectedRows}, Instance: {instance}, IdTenant: {idTenant}, IdUser: {idPeople}, IdGroup: {idGroup}");


                await this._webMethodLoggerService.LogOK("DOC_CAMBIO_STATO", request.docNumber,
                    string.Format(Resources.LogCambioStato, statoEntity.VAR_DESCRIZIONE),
                    null, "PITRE", null, null, request.user.idPeople, request.user.userId, request.user.idGruppo);

                // Inserisco nella coda del motore di Libro firma
                if ((await this._mediator.Send(new Requests.IsDocInLibroFirma(request.docNumber))).output)
                {
                    await this._mediator.Send(
                    new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                    {
                        IdProfile = request.docNumber,
                        Evento = "DOC_CAMBIO_STATO",
                    }));
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null!, null!);

                await this._webMethodLoggerService.LogKO("DOC_CAMBIO_STATO", request.docNumber, string.Format(Resources.LogCambioStato, statoEntity.VAR_DESCRIZIONE));
            }

            return new salvaModificaStatoResult();
        }
        #endregion

        #region Private members
        protected ILogger<SalvaModificaStatoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IConfigurationService _configurationService;

        #endregion
    }
}
