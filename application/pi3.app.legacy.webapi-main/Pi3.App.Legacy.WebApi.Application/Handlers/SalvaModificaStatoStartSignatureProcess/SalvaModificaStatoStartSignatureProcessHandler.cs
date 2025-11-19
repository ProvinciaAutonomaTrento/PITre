// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.GetDescrizioneTipoDocumento;
using Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalvaModificaStatoStartSignatureProcessRequest = Pi3.App.Legacy.WebApi.Application.Requests.SalvaModificaStatoStartSignatureProcess;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SalvaModificaStatoStartSignatureProcess
{
    public class SalvaModificaStatoStartSignatureProcessHandler : IRequestHandler<SalvaModificaStatoStartSignatureProcessRequest, SalvaModificaStatoStartSignatureProcessResult>
    {
        #region Public Members

        public SalvaModificaStatoStartSignatureProcessHandler(ILogger<SalvaModificaStatoStartSignatureProcessHandler> logger, 
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

        public async Task<SalvaModificaStatoStartSignatureProcessResult> Handle(SalvaModificaStatoStartSignatureProcessRequest request, CancellationToken cancellationToken)
        {
            bool output = true;
            ResultProcessoFirma resultAvvioProcesso = ResultProcessoFirma.OK;
            StatoEntity? newStatoEntity = null;
            try
            {
                var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
                var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idCorrGlobaliGruppo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstAsync();

                var docnumber = request.file.docNumber.AsLong();
                var idNewState = request.idStato.AsLong();

                var diagrammiEntity = await this._dbContext.DiagrammiEntities
                    .FirstOrDefaultAsync(s => s.DOC_NUMBER == docnumber);

                newStatoEntity = await this._dbContext.StatoEntities.AsNoTracking().FirstOrDefaultAsync(s => s.SYSTEM_ID == idNewState);
                
                if(newStatoEntity == null)
                    throw new IdStatoDiagrammaNotFoundPi3Exception(idNewState);
                

                var descOldStato = newStatoEntity.VAR_DESCRIZIONE;

                //Faccio update
                if (diagrammiEntity != null)
                {
                    var idCurrentState = diagrammiEntity.ID_STATO;

                    //In ogni caso poichè è stata effettuata una modifica dello stato, elimino lo storico delle trasmissioni
                    //che mi serviva per verificare le accettazioni per il passaggio di stato in automatico.
                    var trasmDiagrEntity = await this._dbContext.TrasmDiagrEntities
                        .AsNoTracking()
                        .Where(t => t.DOC_NUMBER == docnumber && t.ID_STATO == idCurrentState)
                        .FirstOrDefaultAsync();
                    if(trasmDiagrEntity != null)
                        this._dbContext.TrasmDiagrEntities.Remove(trasmDiagrEntity);

                    if(idNewState != idCurrentState)
                    {
                        diagrammiEntity.ID_STATO = idNewState;

                        descOldStato = await this._dbContext.StatoEntities.AsNoTracking().Where(s => s.SYSTEM_ID == idCurrentState).Select(s => s.VAR_DESCRIZIONE).FirstOrDefaultAsync();
                    }
                }
                else
                {
                    //Faccio inserimento
                    diagrammiEntity = new DiagrammiEntity()
                    {
                        DOC_NUMBER = docnumber,
                        ID_STATO = newStatoEntity.SYSTEM_ID,
                        ID_DIAGRAMMA = request.diagramma.SYSTEM_ID
                    };

                    await this._dbContext.DiagrammiEntities.AddAsync(diagrammiEntity);

                    //Se stato iniziale imposto la data di scadenza
                    if (newStatoEntity.STATO_INIZIALE == 1)
                    {
                        var idTipoDoc = await this._dbContext.ProfileEntities.AsNoTracking()
                             .Where(x => x.DOCNUMBER == docnumber)
                             .Select(x => x.ID_TIPO_ATTO)
                             .FirstAsync();

                        await this._mediator.Send(new Application.Requests.salvaDataScadenzaDoc(docnumber.ToString(), request.dataScadenza, idTipoDoc.ToString()));
                    }
                }

                var diagrammiStoEntity = new DiagrammiStoEntity()
                {
                    ID_USER = userId,
                    DOC_NUMBER = docnumber,
                    DTA_DATE = await _dbContext.GetSystemDateTime(),
                    VAR_DESC_OLD_STATO = descOldStato,
                    VAR_DESC_NEW_STATO = newStatoEntity.VAR_DESCRIZIONE,
                    ID_PEOPLE = idPeople,
                    ID_RUOLO = idCorrGlobaliGruppo
                };

                await this._dbContext.DiagrammiStoEntities.AddAsync(diagrammiStoEntity);

                //Avvio del processo di firma
                var avvioProcesso = await this._mediator.Send(new Application.Requests.AvvioProcessoDiFirma(request.processoDiFirma, request.file, request.infoUtente, request.modalita, request.note, request.opzioniNotifiche, true));
                output = avvioProcesso.output;
                resultAvvioProcesso = avvioProcesso.resultAvvioProcesso;

                if(!output)
                    throw new ErroreAvvioProcessoFirmaPi3Exception();

                // Consolidamento
                var key = await this._configurationService.GetValue<string>("BE_CONSOLIDAMENTO");
                if (key == "1")
                {
                    if (!string.IsNullOrWhiteSpace(newStatoEntity.STATO_CONSOLIDAMENTO) && newStatoEntity.STATO_CONSOLIDAMENTO != "0")
                    {
                        var profileEntity = await this._dbContext.ProfileEntities.FirstAsync(x => x.SYSTEM_ID == docnumber);

                        var currentConsolidationState = profileEntity.CONSOLIDATION_STATE ?? "0";

                        if (currentConsolidationState.AsLong() >= newStatoEntity.STATO_CONSOLIDAMENTO.AsLong())
                        {
                            this._logger.LogWarning(Resources.LogAlreadyConsolidated, newStatoEntity.VAR_DESCRIZIONE, docnumber.ToString(),
                                DocumentConsolidationStateDescriptionAttribute.GetDescription((DocumentConsolidationStateEnum)newStatoEntity.STATO_CONSOLIDAMENTO.AsLong()),
                                DocumentConsolidationStateDescriptionAttribute.GetDescription((DocumentConsolidationStateEnum)currentConsolidationState.AsLong()));
                        }
                        else
                        {
                            await this._mediator.Send(new Application.Requests.ConsolidateDocumentById(
                                request.infoUtente,
                                docnumber.ToString(),
                                (DocumentConsolidationStateEnum)newStatoEntity.STATO_CONSOLIDAMENTO.AsLong()));

                            this._logger.LogDebug(Resources.LogConsolidationOK, newStatoEntity.VAR_DESCRIZIONE, docnumber.ToString(),
                                DocumentConsolidationStateDescriptionAttribute.GetDescription((DocumentConsolidationStateEnum)newStatoEntity.STATO_CONSOLIDAMENTO.AsLong()));
                        }
                    }
                }

                await ((DbContext)_dbContext).SaveChangesAsync();

                await this._webMethodLoggerService.LogOK("DOC_CAMBIO_STATO", request.file.docNumber, string.Format(Resources.LogCambioStato, newStatoEntity.VAR_DESCRIZIONE), null, "PITRE");

                //Inserisco nella coda del motore di Libro firma
                await this._mediator.Send(
                    new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                    {
                        IdProfile = request.file.docNumber,
                        Evento = "DOC_CAMBIO_STATO",
                    }));
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("DOC_CAMBIO_STATO", request.file.docNumber, string.Format(Resources.LogCambioStato, newStatoEntity.VAR_DESCRIZIONE));
                output = false;
            }

            return new SalvaModificaStatoStartSignatureProcessResult(output, resultAvvioProcesso);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SalvaModificaStatoStartSignatureProcessHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IConfigurationService _configurationService;

        #endregion
    }
}
