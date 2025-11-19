// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.DiagrammaStato;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetFascicoloByIdNoSecurity;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.NotaAggregate;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FascicolazioneSetFascicoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneSetFascicolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneSetFascicolo
{
    public class FascicolazioneSetFascicoloHandler : IRequestHandler<FascicolazioneSetFascicoloRequest, FascicolazioneSetFascicoloResult>
    {
        #region Public Members

        public FascicolazioneSetFascicoloHandler(ILogger<FascicolazioneSetFascicoloHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IAggregazioneDocumentaleRepository repository,
            INotaRepository repositoryNota)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
            this._webMethodLoggerService = webMethodLoggerService;
            this._repositoryNota = repositoryNota;
        }

        public async Task<FascicolazioneSetFascicoloResult> Handle(FascicolazioneSetFascicoloRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            int isID = 0;
            var fascicolo = request.fascicolo;
            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var idFascicoloAsLong = fascicolo.systemID.AsLong();

                var idFolder = await this._dbContext.ProjectEntities.AsNoTracking().Where(p => p.ID_PARENT == idFascicoloAsLong).Select(p => p.SYSTEM_ID).FirstAsync();
                var aggregate = await this._repository.Get(idTenant, idFolder.ToString(), new ILoadBehavior[1]
                {
                    new GetAggregatoDocumentaleLoadBehavior()
                    {

                        BypassSecurityCheck = false,
                        LoadFolderHierarchy = false,
                        LoadProfiles = true
                    }
                });

                var isChiuso = aggregate.DataChiusura != null;

                if (!isChiuso && !string.IsNullOrEmpty(fascicolo.chiusura))
                {
                    aggregate.Chiudi(fascicolo.chiusura.AsDateTime());
                }
                else if (isChiuso && !string.IsNullOrEmpty(fascicolo.apertura))
                {
                    aggregate.Apri(fascicolo.apertura.AsDateTime());
                }
                else
                {
                    if (string.IsNullOrEmpty(fascicolo.descrizione))
                        throw new DescriptionCannotBeEmptyPi3Exception();

                    aggregate.ChangeDescription(new TextValue(fascicolo.descrizione));

                    aggregate.AssignCollocazioneFisica(new Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects.CollocazioneFisica()
                    {
                        Id = fascicolo.idUoLF,
                        DataCollocazione = !string.IsNullOrEmpty(fascicolo.dtaLF) ? fascicolo.dtaLF.AsDateTime() : null,
                        Descrizione = new TextValue(fascicolo.descrizioneUOLF),
                        Cartaceo = fascicolo.cartaceo
                    });

                    //DATA SCADENZA
                    if (fascicolo.template != null && fascicolo.template.SYSTEM_ID != 0)
                    {
                        if (aggregate.Profiles == null || aggregate.Profiles.Count == 0)
                        {
                            //Inserimento campi profilati
                            aggregate.AddProfile(fascicolo.template.SYSTEM_ID.ToString(), new TextValue(fascicolo.template.DESCRIZIONE));

                            foreach (var oggettoCustom in fascicolo.template.ELENCO_OGGETTI)
                            {
                                switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                                {
                                    case "Contatore":
                                    case "ContatoreSottocontatore":
                                        aggregate.AddProfileField(
                                        fascicolo.template.SYSTEM_ID.ToString(),
                                        oggettoCustom.SYSTEM_ID.ToString(),
                                        new TextValue(oggettoCustom.DESCRIZIONE),
                                        oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                        new ContatoreRepertorioFieldValue(oggettoCustom.ID_AOO_RF, oggettoCustom.CONTATORE_DA_FAR_SCATTARE, oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI"));
                                        break;
                                    case "CasellaDiSelezione":
                                        aggregate.AddProfileField(
                                        fascicolo.template.SYSTEM_ID.ToString(),
                                        oggettoCustom.SYSTEM_ID.ToString(),
                                        new TextValue(oggettoCustom.DESCRIZIONE),
                                        oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                        new ElementFieldMultiValue(oggettoCustom.VALORI_SELEZIONATI.Select(s => new TextValue(s)).ToArray()));
                                        break;
                                    default:
                                        aggregate.AddProfileField(
                                        fascicolo.template.SYSTEM_ID.ToString(),
                                        oggettoCustom.SYSTEM_ID.ToString(),
                                        new TextValue(oggettoCustom.DESCRIZIONE),
                                        oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                        new ElementFieldSingleValue(new TextValue(oggettoCustom.VALORE_DATABASE)));
                                        break;
                                }
                            }
                        }
                        else
                        {
                            //Modifica campi profilati
                            foreach (var oggettoCustom in fascicolo.template.ELENCO_OGGETTI)
                            {
                                switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                                {
                                    case "Contatore":
                                    case "ContatoreSottocontatore":
                                        aggregate.ChangeProfileFieldValue(
                                        fascicolo.template.SYSTEM_ID.ToString(),
                                        oggettoCustom.SYSTEM_ID.ToString(),
                                        new ContatoreRepertorioFieldValue(oggettoCustom.ID_AOO_RF, oggettoCustom.CONTATORE_DA_FAR_SCATTARE, oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI"));
                                        break;
                                    case "CasellaDiSelezione":
                                        aggregate.ChangeProfileFieldValue(
                                        fascicolo.template.SYSTEM_ID.ToString(),
                                        oggettoCustom.SYSTEM_ID.ToString(),
                                        new ElementFieldMultiValue(oggettoCustom.VALORI_SELEZIONATI.Select(s => new TextValue(s)).ToArray()));
                                        break;
                                    default:
                                        aggregate.ChangeProfileFieldValue(
                                        fascicolo.template.SYSTEM_ID.ToString(),
                                        oggettoCustom.SYSTEM_ID.ToString(),
                                        new ElementFieldSingleValue(new TextValue(oggettoCustom.VALORE_DATABASE)));
                                        break;
                                }
                            }
                        }
                    }                   
                }

                await this._repository.Update(aggregate);

                var description = Resources.LogFascicolazioneSetFascicolo;
                if (isChiuso && fascicolo.stato.Equals("A"))
                    description = Resources.LogFascicolazioneSetFascicoloApertura;
                if (!isChiuso && fascicolo.stato.Equals("C"))
                    description = Resources.LogFascicolazioneSetFascicoloChiusura;

                await this._webMethodLoggerService.LogOK("FASCICOLOMODIFICA", fascicolo.systemID, string.Format(description, fascicolo.codice));


                await this._webMethodLoggerService.LogOK("FOLLOWFASCEXTAPP", fascicolo.systemID, string.Format(Resources.LogDocumentSetFascicoloFollowFascExtApp, fascicolo.codice));
            }
            catch(Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                await this._webMethodLoggerService.LogKO("FASCICOLOMODIFICA", fascicolo.systemID, string.Format(Resources.LogFascicolazioneSetFascicolo, fascicolo.codice));
                output = false;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, null, null);
                await this._webMethodLoggerService.LogKO("FASCICOLOMODIFICA", fascicolo.systemID, string.Format(Resources.LogFascicolazioneSetFascicolo, fascicolo.codice));
                output = false;
            }

            return new FascicolazioneSetFascicoloResult(output, fascicolo);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneSetFascicoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IAggregazioneDocumentaleRepository _repository;
        protected readonly INotaRepository _repositoryNota;

        #endregion
    }
}
