// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.ContentElementAggregate.Events;
using Pi3.Core.AggregateModels.DocumentAggregate.Events;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Configuration.Internal;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.DomainEventHandlers
{
    public class DocumentoAmministrativoEventHandlers :
        IEventHandler<DocumentoAmministrativoCreatedEvent>,
        IEventHandler<RegistrazioneRichiestaEvent>,
        IEventHandler<ContentElementClassificationAddedEvent>,
        IEventHandler<ContentElementClassificationRemovedEvent>,
        IEventHandler<DocumentAddedInRecycleBinEvent>,
        IEventHandler<DocumentRestoredEvent>,
        IEventHandler<OggettoDelDocumentoChangedEvent>,
        IEventHandler<AggFascicoloAddedEvent>,
        IEventHandler<AggFascicoloRemovedEvent>,
        IEventHandler<DocumentoConsolidatoEvent>,
        IEventHandler<MezzoSpedizioneAssignedEvent>,
        IEventHandler<AnnullatoEvent>,
        IEventHandler<ElementProfileFieldAddedEvent>
    {
        #region Public Members

        public DocumentoAmministrativoEventHandlers(
            ILogger<DocumentoAmministrativoEventHandlers> logger,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public virtual async Task Handle(DocumentoAmministrativoCreatedEvent e)
        {
            await this.InternalHandle(e,
                async ()  => 
                {
                    var logDescription = string.Format(Logs.DOCUMENTOADDDOCGRIGIA, e.Id);

                    this._logger.LogInformation($"{nameof(Logs.DOCUMENTOADDDOCGRIGIA)}: {logDescription}");

                    await this._webMethodLoggerService.LogOK(nameof(Logs.DOCUMENTOADDDOCGRIGIA), e.Id, logDescription);
                });
        }

        public virtual async Task Handle(ContentElementClassificationAddedEvent e)
        {
            await this.InternalHandle(e,
                async () =>
                {
                    var logDescription = string.Format(Logs.DOCADDINCLASS, e.Id, e.Code);

                    this._logger.LogInformation($"{nameof(Logs.DOCADDINCLASS)}: {logDescription}");

                    await this._webMethodLoggerService.LogOK(nameof(Logs.DOCADDINCLASS), e.Id, logDescription);
                });
        }

        public virtual async Task Handle(ContentElementClassificationRemovedEvent e)
        {
            await this.InternalHandle(e,
                async () =>
                {
                    var logDescription = string.Format(Logs.DOCDELFROMFOLDER, e.Id, e.IdClassification);

                    this._logger.LogInformation($"{nameof(Logs.DOCDELFROMFOLDER)}: {logDescription}");

                    await this._webMethodLoggerService.LogOK(nameof(Logs.DOCDELFROMFOLDER), e.Id, logDescription);
                });
        }

        public virtual async Task Handle(RegistrazioneRichiestaEvent e)
        {
            await this.InternalHandle(e,
                async () =>
                {
                    long id = Pi3.Core.Extensions.StringExtensions.AsLong(e.Id);

                    var profileEntity = await this._dbContext.ProfileEntities.AsNoTracking()
                            .Where(p => p.SYSTEM_ID == id)
                            .Select(p => new { p.DOCNUMBER, p.VAR_SEGNATURA })
                            .FirstAsync();

                    var logFormat = e.Registrazione.Predisponi ? Logs.RECORDPREDISPOSED : Logs.DOCUMENTOPROTOCOLLA;
                    var nameOfLog = e.Registrazione.Predisponi ? nameof(Logs.RECORDPREDISPOSED) : nameof(Logs.DOCUMENTOPROTOCOLLA);

                    var logDescription = string.Format(logFormat, profileEntity.DOCNUMBER, profileEntity.VAR_SEGNATURA);

                    this._logger.LogInformation($"{nameOfLog}: {logDescription}");

                    await this._webMethodLoggerService.LogOK(nameOfLog, e.Id, logDescription);
                });
        }


        public virtual async Task Handle(DocumentAddedInRecycleBinEvent e)
        {
            await this.InternalHandle(e,
                async () =>
                {
                    var logDescription = string.Format(Logs.DOCUMENTOEXECRIMUOVISCHEDA, e.Id);

                    this._logger.LogInformation($"{nameof(Logs.DOCUMENTOEXECRIMUOVISCHEDA)}: {logDescription}");

                    await this._webMethodLoggerService.LogOK(nameof(Logs.DOCUMENTOEXECRIMUOVISCHEDA), e.Id, logDescription);
                });
        }

        public virtual async Task Handle(DocumentRestoredEvent e)
        {
            await this.InternalHandle(e,
                async () =>
                {
                    var logDescription = string.Format(Logs.DOCUMENTORIATTIVADOC, e.Id);

                    this._logger.LogInformation($"{nameof(Logs.DOCUMENTORIATTIVADOC)}: {logDescription}");

                    await this._webMethodLoggerService.LogOK(nameof(Logs.DOCUMENTORIATTIVADOC), e.Id, logDescription);
                });
        }

        public virtual async Task Handle(OggettoDelDocumentoChangedEvent e)
        {
            await this.InternalHandle(e,
                async () =>
                {
                    var logDescription = string.Format(Logs.MODIFIEDOBJECTPROTO, e.Id);

                    this._logger.LogInformation($"{nameof(Logs.MODIFIEDOBJECTPROTO)}: {logDescription}");

                    await this._webMethodLoggerService.LogOK(nameof(Logs.MODIFIEDOBJECTPROTO), e.Id, logDescription);
                });
        }

        public virtual async Task Handle(AggFascicoloAddedEvent @e)
        {
            await this.InternalHandle(e,
                async () =>
                {
                    var logDescription = string.Format(Logs.DOCADDINFOLDER, e.Id, e.IdFascicolo);

                    this._logger.LogInformation($"{nameof(Logs.DOCADDINFOLDER)}: {logDescription}");

                    await this._webMethodLoggerService.LogOK(nameof(Logs.DOCADDINFOLDER), e.Id, logDescription);
                });
        }

        public virtual async Task Handle(AggFascicoloRemovedEvent e)
        {
            await this.InternalHandle(e,
                async () =>
                {
                    var logDescription = string.Format(Logs.DOCDELFROMFOLDER, e.Id, e.IdFascicolo);

                    this._logger.LogInformation($"{nameof(Logs.DOCDELFROMFOLDER)}: {logDescription}");

                    await this._webMethodLoggerService.LogOK(nameof(Logs.DOCDELFROMFOLDER), e.Id, logDescription);
                });
        }

        public virtual async Task Handle(DocumentoConsolidatoEvent e)
        {
            await this.InternalHandle(e,
                async () =>
                {
                    long id = Pi3.Core.Extensions.StringExtensions.AsLong(e.Id);

                    var profileEntity = await this._dbContext.ProfileEntities.AsNoTracking()
                            .Where(p => p.SYSTEM_ID == id)
                            .Select(p => new { p.DOCNUMBER, p.VAR_SEGNATURA })
                            .FirstAsync();
                    
                    var protocollato = !string.IsNullOrWhiteSpace(profileEntity.VAR_SEGNATURA);

                    var logFormat = (protocollato ? Logs.CONSOLIDADOCUMENTOPROTOCOLLATO : Logs.CONSOLIDADOCUMENTONONPROTOCOLLATO);
                    var nameOfLog = "CONSOLIDADOCUMENTO";

                    var logDescription = string.Format(logFormat, profileEntity.DOCNUMBER, profileEntity.VAR_SEGNATURA);

                    this._logger.LogInformation($"{nameOfLog}: {logDescription}");

                    await this._webMethodLoggerService.LogOK(nameOfLog, e.Id, logDescription);
                });
        }

        public virtual async Task Handle(MezzoSpedizioneAssignedEvent e)
        {
            await this.InternalHandle(e,
                async () =>
                {
                    var logDescription = string.Format(Logs.DOCUMENTOSPEDIZIONE, e.Id);

                    this._logger.LogInformation($"{nameof(Logs.DOCUMENTOSPEDIZIONE)}: {logDescription}");

                    await this._webMethodLoggerService.LogOK(nameof(Logs.DOCUMENTOSPEDIZIONE), e.Id, logDescription);
                });           
        }

        public virtual async Task Handle(AnnullatoEvent e)
        {
            await this.InternalHandle(e,
                async () =>
                {
                    long id = Pi3.Core.Extensions.StringExtensions.AsLong(e.Id);

                    var profileEntity = await this._dbContext.ProfileEntities.AsNoTracking()
                            .Where(p => p.SYSTEM_ID == id)
                            .Select(p => new { p.VAR_SEGNATURA })
                            .FirstAsync();

                    var logDescription = string.Format(Logs.DOCUMENTOEXECANNULLAPROT, profileEntity.VAR_SEGNATURA);

                    this._logger.LogInformation($"{nameof(Logs.DOCUMENTOEXECANNULLAPROT)}: {logDescription}");

                    await this._webMethodLoggerService.LogOK(nameof(Logs.DOCUMENTOEXECANNULLAPROT), e.Id, logDescription);
                });
        }

        public virtual async Task Handle(ElementProfileFieldAddedEvent e)
        {
            await this.InternalHandle(e,
              async () =>
              {
                  var contatoreFieldValue = e.FieldValue as Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ContatoreRepertorioFieldValue;
                  
                  if (contatoreFieldValue! != null! && (contatoreFieldValue.Conta ?? false))
                  {
                      var logDescription = string.Format(Logs.DOCUMENTO_REPERTORIATO, contatoreFieldValue.Value);

                      this._logger.LogInformation($"{nameof(Logs.DOCUMENTO_REPERTORIATO)}: {logDescription}");

                      await this._webMethodLoggerService.LogOK(nameof(Logs.DOCUMENTO_REPERTORIATO), e.Id, logDescription);
                  }
              });
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoAmministrativoEventHandlers> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        protected virtual async Task InternalHandle(Event e, Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(
                    message: string.Format(Pi3.App.DocumentoAmministrativo.WebApi.Application.DomainEventHandlers.ErrorDescriptions.HandleErrorMessage, nameof(e)),
                    exception: pi3Ex);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(
                    message: string.Format(Pi3.App.DocumentoAmministrativo.WebApi.Application.DomainEventHandlers.ErrorDescriptions.HandleErrorMessage, nameof(e)),
                    exception: ex);
            }
        }

        #endregion
    }
}
