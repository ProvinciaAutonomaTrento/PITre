// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
using DocsPaVO.Mobile.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.ConsolidateDocumentById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetVersionsMainDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SalvaDataScadenzaDoc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.AvvioProcessoDiFirma;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetProcessoDiFirma;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Services.Configuration;
using Pi3.Infrastructure.Legacy.EF.Services.WebMethodLogger;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SalvaModificaStato
{
    public class SalvaModificaStatoCommandHandler : IRequestHandler<SalvaModificaStatoCommand, SalvaModificaStatoCommandResponse>
    {
        public SalvaModificaStatoCommandHandler(ILogger<SalvaModificaStatoCommandHandler> logger,
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

        public async Task<SalvaModificaStatoCommandResponse> Handle(SalvaModificaStatoCommand request, CancellationToken cancellationToken)
        {
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            var idCorrGlobali = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(x => x.ID_GRUPPO == idGroup).Select(x => x.SYSTEM_ID).FirstAsync();

            StatoEntity? statoEntity = null;

            try
            {
                var diagrammiEntity = await this._dbContext.DiagrammiEntities
                    .FirstOrDefaultAsync(x => x.DOC_NUMBER == request.DocNumber.AsLong());

                // statoOld
                string? currentState = default;

                statoEntity = await this._dbContext.StatoEntities.AsNoTracking().FirstOrDefaultAsync(x => x.SYSTEM_ID == request.IdStato.AsLong());

                if (diagrammiEntity != null)
                {
                    currentState = await this._dbContext.StatoEntities
                        .Where(x => x.SYSTEM_ID == diagrammiEntity.ID_STATO)
                        .Select(x => x.VAR_DESCRIZIONE)
                        .FirstAsync();

                    var trasmDiagrEntity = await this._dbContext.TrasmDiagrEntities.FirstOrDefaultAsync(x => x.ID_STATO == diagrammiEntity.ID_STATO && x.DOC_NUMBER == diagrammiEntity.DOC_NUMBER);

                    if (trasmDiagrEntity != null)
                    {
                        this._dbContext.TrasmDiagrEntities.Remove(trasmDiagrEntity);
                    }

                    if (diagrammiEntity.ID_STATO != request.IdStato.AsLong())
                    {
                        diagrammiEntity.ID_STATO = request.IdStato.AsLong();
                    }
                }
                else
                {
                    currentState = statoEntity.VAR_DESCRIZIONE;

                    diagrammiEntity = new DiagrammiEntity
                    {
                        DOC_NUMBER = request.DocNumber.AsLong(),
                        ID_STATO = request.IdStato.AsLong(),
                        ID_DIAGRAMMA = request.Diagramma.SYSTEM_ID
                    };

                    await this._dbContext.DiagrammiEntities.AddAsync(diagrammiEntity);

                    //Se stato iniziale serve impostare la data di scadenza
                    if (statoEntity.STATO_INIZIALE == 1)
                    {
                        var idTipoDoc = await this._dbContext.ProfileEntities.AsNoTracking()
                            .Where(x => x.DOCNUMBER == request.DocNumber.AsLong())
                        .Select(x => x.ID_TIPO_ATTO)
                        .FirstOrDefaultAsync();

                        await this._mediator.Send(new SalvaDataScadenzaDocCommand()
                        {
                            DocNumber = request.DocNumber,
                            DataScadenza = request.dataScadenza,
                            IdTipoAtto = idTipoDoc.ToString()
                        });
                    }
                }

                // Inserimento storico
                var diagrammiStoEntity = new DiagrammiStoEntity
                {
                    ID_USER = request.IdUtente,
                    ID_PEOPLE = idPeople,
                    ID_RUOLO = idCorrGlobali,
                    DTA_DATE = await _dbContext.GetSystemDateTime(),
                    DOC_NUMBER = request.DocNumber.AsLong(),
                    VAR_DESC_OLD_STATO = currentState,
                    VAR_DESC_NEW_STATO = statoEntity.VAR_DESCRIZIONE,
                    ID_PEOPLE_DELEGATO = request.User?.delegato != null ? request.User.delegato.idPeople.AsLong() : 0
                };

                await this._dbContext.DiagrammiStoEntities.AddAsync(diagrammiStoEntity);

                // Libro firma
                if (statoEntity.ID_PROCESSO_FIRMA.HasValue
                    && !(await this._dbContext.SchemaProcessoFirmaEntities.AnyAsync(x => x.ID_PROCESSO == statoEntity.ID_PROCESSO_FIRMA && x.CHA_MODELLO == "1")))
                {

                    var processo = (await this._mediator.Send(new GetProcessoDiFirmaCommand()
                    {
                        IdProcesso = statoEntity.ID_PROCESSO_FIRMA.ToString(),
                        InfoUtente = request.User
                    })).Output;
                    var fileRequest = (await this._mediator.Send(new GetVersionsMainDocumentCommand()
                    {
                        InfoUser = request.User,
                        DocNumber = request.DocNumber
                    })).Output;
                    var avvioProcesso = await this._mediator.Send(new AvvioProcessoDiFirmaCommand()
                    {
                        processoDiFirma = processo,
                        file = fileRequest[0],
                        modalita = "A",
                        note = string.Empty,
                        opzioniNotifiche = new OpzioniNotifica()
                        {
                            Notifica_interrotto = true,
                            Notifica_concluso = false
                        },
                        daCambioStato = true,
                    });
                    if (!avvioProcesso.output)
                    {
                        throw new ErroreAvvioProcessoFirmaPi3Exception();
                    }
                }

                // Consolidamento
                var key = await this._configurationService.GetValue<string>("BE_CONSOLIDAMENTO");
                if (key == "1")
                {
                    if (!string.IsNullOrWhiteSpace(statoEntity.STATO_CONSOLIDAMENTO) && statoEntity.STATO_CONSOLIDAMENTO != "0")
                    {
                        var profileEntity = await this._dbContext.ProfileEntities.FirstAsync(x => x.SYSTEM_ID == request.DocNumber.AsLong());

                        var currentConsolidationState = profileEntity.CONSOLIDATION_STATE ?? "0";

                        if (currentConsolidationState.AsLong() >= statoEntity.STATO_CONSOLIDAMENTO.AsLong())
                        {
                            this._logger.LogWarning(Resources.LogAlreadyConsolidated, statoEntity.VAR_DESCRIZIONE, request.DocNumber,
                                DocumentConsolidationStateDescriptionAttribute.GetDescription((DocumentConsolidationStateEnum)statoEntity.STATO_CONSOLIDAMENTO.AsLong()),
                                DocumentConsolidationStateDescriptionAttribute.GetDescription((DocumentConsolidationStateEnum)currentConsolidationState.AsLong()));
                        }
                        else
                        {
                            await this._mediator.Send(new ConsolidateDocumentByIdCommand()
                            {
                                UserInfo = request.User,
                                IdDocument = request.DocNumber,
                                ToState = (DocumentConsolidationStateEnum)statoEntity.STATO_CONSOLIDAMENTO.AsLong()
                            });

                            this._logger.LogDebug(Resources.LogConsolidationOK, statoEntity.VAR_DESCRIZIONE, request.DocNumber,
                                DocumentConsolidationStateDescriptionAttribute.GetDescription((DocumentConsolidationStateEnum)statoEntity.STATO_CONSOLIDAMENTO.AsLong()));
                        }
                    }
                }

                await ((DbContext)this._dbContext).SaveChangesAsync();

                await this._webMethodLoggerService.LogOK("DOC_CAMBIO_STATO", request.DocNumber,
                    string.Format(Resources.LogCambioStato, statoEntity.VAR_DESCRIZIONE),
                    null, "PITRE", null, null, request.User.idPeople, request.User.userId, request.User.idGruppo);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                await this._webMethodLoggerService.LogKO("DOC_CAMBIO_STATO", request.DocNumber, string.Format(Resources.LogCambioStato, statoEntity.VAR_DESCRIZIONE));
            }

            return new ();
        }


        #region Private members
        protected ILogger<SalvaModificaStatoCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IConfigurationService _configurationService;

        #endregion
    }
}
