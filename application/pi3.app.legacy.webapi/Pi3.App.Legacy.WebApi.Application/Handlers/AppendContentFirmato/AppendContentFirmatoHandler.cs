// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocumentFormat.OpenXml.Office2013.Word;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.App.Legacy.WebApi.Application.Sign;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppendContentFirmatoRequest = Pi3.App.Legacy.WebApi.Application.Requests.AppendContentFirmato;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AppendContentFirmato
{
    public class AppendContentFirmatoHandler : IRequestHandler<AppendContentFirmatoRequest, AppendContentFirmatoResult>
    {
        #region Public Members

        public AppendContentFirmatoHandler(ILogger<AppendContentFirmatoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<AppendContentFirmatoResult> Handle(AppendContentFirmatoRequest request, CancellationToken cancellationToken)
        {
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser);
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var groupId = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idPeopleDelegato = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);

            var fileRequest = request.fileRequest;
            AppendDocumentoFirmatoManagerResult appendDocumentoFirmatoResult = null;

            try
            {
                appendDocumentoFirmatoResult = await _mediator.Send(new Requests.AppendDocumentoFirmatoManager(request.sigedContent,
                        request.cofirma,
                        fileRequest,
                        request.infoUtente));

                if (appendDocumentoFirmatoResult.output)
                {
                    string method = "DOC_SIGNATURE";
                    string description =  Resources.SignDescriptionCADES;

                    if ((await _mediator.Send(new Requests.IsDocInLibroFirma(fileRequest.docNumber))).output)
                    {
                        await this.AggiornaDataEsecuzioneElemento(fileRequest.docNumber.AsLong(), DocsPaVO.LibroFirma.TipoStatoElemento.FIRMATO.ToString());
                        await this.SalvaStoricoIstanzaProcessoFirmaByDocnumber(fileRequest.docNumber.AsLong(), description, idPeople.AsLong(), userId, groupId, idPeopleDelegato);

                        //Inserisco nella coda del motore di Libro firma
                        await this._mediator.Send(
                            new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                            {
                                IdProfile = fileRequest.docNumber,
                                Evento = method,
                            }));
                    }

                    await this._webMethodLoggerService.LogOK(method, fileRequest.docNumber, description, null, "PITRE");
                }
            }
            catch (Exception ex) 
            { 
                this._logger.LogCritical(exception: ex, message: ex.Message);
                appendDocumentoFirmatoResult = new AppendDocumentoFirmatoManagerResult(false, null);
            }

            return new AppendContentFirmatoResult(appendDocumentoFirmatoResult.output, appendDocumentoFirmatoResult.fileRequest);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AppendContentFirmatoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        protected async Task AggiornaDataEsecuzioneElemento(long docNumber, string stato)
        {
            var elInLfEntity = await this._dbContext.ElementoInLibroFirmaEntities
                .Where(x => x.DOC_NUMBER == docNumber && x.DTA_ESECUZIONE == null)
                .FirstOrDefaultAsync();

            if (elInLfEntity != null)
            {
                elInLfEntity.DOC_NUMBER = docNumber;
                elInLfEntity.STATO_FIRMA = stato;
                elInLfEntity.DTA_ESECUZIONE = DateTime.Now;
            }

            int rowUpdated = await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected async Task SalvaStoricoIstanzaProcessoFirmaByDocnumber(long docNumber, string description, long idPeople, string userId, long groupId, long idPeopleDelegato)
        {
            var istanza = await this._dbContext.IstanzaProcessoFirmaEntities
                .Where(x => x.ID_DOCUMENTO == docNumber && x.STATO.Equals("IN_EXEC"))
                .Select(x => new { ID_ISTANZA = x.ID_ISTANZA, CHA_CAMBIO_STATO_DIAG = x.CHA_CAMBIO_STATO_DIAG })
                .FirstOrDefaultAsync();

            if (istanza != null)
            {
                long corrGlobaliGroup = await this._dbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == groupId).Select(x => x.SYSTEM_ID).FirstOrDefaultAsync();
                var istanzaProcessoFirmaStoEntity = new IstanzaProcFirmaStoEntity()
                {
                    ID_USER = userId,
                    DOC_NUMBER = docNumber,
                    ID_ISTANZA_PROCESSO = istanza.ID_ISTANZA,
                    DTA_DATE = DateTime.Now,
                    VAR_DESC_AZIONE = description.Replace("'", "''"),
                    ID_PEOPLE = idPeople,
                    ID_RUOLO = groupId == 0 ? 0 : corrGlobaliGroup,
                    ID_PEOPLE_DELEGATO = idPeopleDelegato,
                    CHA_CAMBIO_STATO_DIAG = istanza.CHA_CAMBIO_STATO_DIAG
                };

                await this._dbContext.IstanzaProcFirmaStoEntities.AddAsync(istanzaProcessoFirmaStoEntity);
                int rowsInserted = await ((Pi3DbContext)_dbContext).SaveChangesAsync();
            }
        }
        #endregion
    }
}