// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Core.Services.Configuration;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetTemplateFromPisVisibility;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.GetTemplateFascById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetDiagrammaAssociatoFasc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetTemplateByDescrizione;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Chilkat;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.CreateProjectWithArchivePlan
{
    // Richiede libreria MediatR
    public class CreateProjectWithArchivePlanCommandHandler : IRequestHandler<CreateProjectWithArchivePlanCommand, CreateProjectWithArchivePlanCommandResponse>
    {
        #region Public Members

        public CreateProjectWithArchivePlanCommandHandler(IConfigurationService configurationService,ILogger<CreateProjectWithArchivePlanCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IAggregazioneDocumentaleRepository aggDocRepository, IWebMethodLoggerService loggerService)
        {
            this._configurationService = configurationService;
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._loggerService = loggerService;
            this._aggDocRepository = aggDocRepository;
        }

        public async Task<CreateProjectWithArchivePlanCommandResponse> Handle(CreateProjectWithArchivePlanCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("CreateProjectWithArchivePlan - START");

            CreateProjectWithArchivePlanCommandResponse response = new CreateProjectWithArchivePlanCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region Controllo parametri richiesta
                if (request.Project == null)
                {
                    throw new RestException("REQUIRED_PROJECT");
                }

                if (request.Project == null || string.IsNullOrEmpty(request.Project.CodeNodeClassification))
                {
                    throw new RestException("REQUIRED_GENERAL_NODE");
                }

                if (request.Project.ClassificationScheme == null || string.IsNullOrEmpty(request.Project.ClassificationScheme.Id))
                {
                    throw new RestException("ID_CLASSIFICATIONSCHEME_REQUIRED");
                }
                (var pianoDiConservazioneEnabled, bool keyFound) = await this._configurationService.TryGetValue<string>(infoUtente.idAmministrazione, "ENABLE_PIANO_CONSERVAZIONE");

                if ((keyFound && "1".Equals(pianoDiConservazioneEnabled)) && (request.Project.ArchivePlan == null || string.IsNullOrEmpty(request.Project.ArchivePlan.Id)))
                {
                    throw new RestException("ARCHIVE_PLAN_ID_REQUIRED");
                }

                #endregion

                #region implementazione

                DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                bool setStatoDiagrammaIniziale = false;
                DocsPaVO.DiagrammaStato.Stato statoIniziale = null;
                var titolario = DBUtils.getTitolarioById(request.Project.ClassificationScheme.Id, _pi3DbContext);
                if (titolario == null || string.IsNullOrWhiteSpace(titolario.ID))
                {
                    throw new RestException("CLASSIFICATION_NOT_FOUND");
                }
                DocsPaVO.utente.Registro registro = null;

                if (request.Project.Register != null)
                {
                    if (!string.IsNullOrWhiteSpace(request.Project.Register.Id))
                    {
                        registro = DBUtils.getRegistro(request.Project.Register.Id, _pi3DbContext);
                    }
                    else if (!string.IsNullOrWhiteSpace(request.Project.Register.Code))
                    {
                        registro = DBUtils.getRegistroByCodAOO(request.Project.Register.Code, infoUtente.idAmministrazione, _pi3DbContext);
                    }
                    if (registro == null) throw new RestException("REGISTER_NOT_FOUND");
                }

                ProjectEntity classificazione = null;
                var classificazioni = from a in _pi3DbContext.ProjectEntities
                                      where a.ID_AMM == infoUtente.idAmministrazione.AsLong() &&
                                      a.VAR_CODICE == request.Project.CodeNodeClassification && a.ID_TITOLARIO == titolario.ID.AsLong()
                                      && a.CHA_TIPO_PROJ == "T"
                                      select a;
                if (classificazioni != null && classificazioni.Any())
                {
                    if (registro != null) classificazione = classificazioni.Where(x => x.ID_REGISTRO == registro.systemId.AsLong()).FirstOrDefault();
                    else classificazione = classificazioni.FirstOrDefault();
                    if (classificazione == null || classificazione.SYSTEM_ID == 0) throw new RestException("GENERAL_NODE_NOT_FOUND");
                    else
                    {
                        registro = DBUtils.getRegistro(classificazione.ID_REGISTRO.ToString(), _pi3DbContext);
                    }
                }
                else
                {
                    throw new RestException("GENERAL_NODE_NOT_FOUND");
                }

                ArchivePlan pianoConservazione = null;
                if (request.Project.ArchivePlan != null)
                {
                    Dictionary<string, string> fRicArchP = new Dictionary<string, string>();
                    fRicArchP.Add("CLASSIFICATION_SCHEME_ID", titolario.ID.ToString());
                    fRicArchP.Add("CLASSIFICATION_NODE_ID", classificazione.SYSTEM_ID.ToString());
                    if (request.Project.Template != null && request.Project.Template.Id.AsLong()> 0)
                    {
                        fRicArchP.Add("PROJECT_TEMPLATE_ID", request.Project.Template.Id.ToString());
                    }
                    var ris = DBUtils.SearchArchivePlansRest(fRicArchP, "0", "10000",this._pi3DbContext);
                    bool pianoConservazioneTrovato = false;
                    if (ris != null && ris.Count > 0)
                    {
                        foreach (var r in ris)
                        {
                            if (r.SystemId == request.Project.ArchivePlan.Id)
                            {
                                pianoConservazioneTrovato = true;
                                pianoConservazione = new()
                                {
                                    Id = r.SystemId,
                                };
                                break;
                            }
                        }
                    }
                    if (!pianoConservazioneTrovato)
                        throw new RestException("ARCHIVE_PLAN_NOT_FOUND");
                }

                AggregazioneDocumentale fasc = new AggregazioneDocumentale(infoUtente.idAmministrazione, DateTime.Now,
                    new Core.SeedWork.TextValue(request.Project.Description), TipiAggregazioneEnum.SerieDocumentale, null,
                    request.Project.Private ? TipologieVisibilitaEnum.Privata : TipologieVisibilitaEnum.Gerarchica);



                fasc.AddClassification(classificazione.SYSTEM_ID.ToString(), new Core.SeedWork.TextValue(classificazione.DESCRIPTION), pianoConservazione.Id, new Core.SeedWork.TextValue(pianoConservazione.Description));
                //if (registro != null)
                //    fasc.AssignRegistro(registro.systemId, registro.descrizione);

                // In creazione la collocazione fisica � la uo del ruolo creatore
                fasc.AssignCollocazioneFisica(new CollocazioneFisica() { Id = ruolo.uo.systemId, Descrizione = new Core.SeedWork.TextValue(ruolo.uo.descrizione), DataCollocazione = DateTime.Now, Cartaceo = false });
                var reqTemplate = request.Project.Template;

                if (reqTemplate != null && (!string.IsNullOrEmpty(reqTemplate.Id) || (!string.IsNullOrEmpty(reqTemplate.Name))))
                {
                    #region fetch template
                    DocsPaVO.ProfilazioneDinamica.Templates template = null;
                    if (!string.IsNullOrEmpty(reqTemplate.Id))
                    {
                        template = (await this._mediator.Send(new GetTemplateFascByIdCommand()
                        {
                            IdTemplate = reqTemplate.Id,
                        })).Output;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(reqTemplate.Name))
                        {
                            template = (await this._mediator.Send(new GetTemplateByDescrizioneCommand()
                            {
                                Desc = reqTemplate.Name
                            })).Output;
                        }
                    }
                    #endregion

                    #region check rights template
                    string tempSystemId = template.SYSTEM_ID.ToString();
                    var listaTemplate = DBUtils.GetListTemplatesLiteByRoleWithReadWriteRightsFasc(infoUtente.idAmministrazione, infoUtente.idGruppo, _pi3DbContext);
                    var inListaTemp = listaTemplate?.Select(t => t.system_id).Contains(template.SYSTEM_ID.ToString()) ?? false;
                    if (listaTemplate == null || !inListaTemp)
                    {
                        throw new RestException("TEMPLATE_NOT_ROLE_EDITABLE");
                    }
                    #endregion

                    template = (await this._mediator.Send(new GetTemplateFromPisVisibilityCommand(request.Project.Template, template, false, infoUtente.idGruppo, "P", "", infoUtente))).output;
                    if (template == null)
                    {
                        throw new RestException("TEMPLATE_NOT_FOUND");
                    }

                    fasc.AddProfile(template.SYSTEM_ID.ToString(), new TextValue(template.DESCRIZIONE));

                    foreach (var oggettoCustom in template.ELENCO_OGGETTI)
                    {
                        switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "Contatore":
                            case "ContatoreSottocontatore":
                                fasc.AddProfileField(
                                tempSystemId,
                                oggettoCustom.SYSTEM_ID.ToString(),
                                new TextValue(oggettoCustom.DESCRIZIONE),
                                oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                new ContatoreRepertorioFieldValue(oggettoCustom.ID_AOO_RF, oggettoCustom.CONTATORE_DA_FAR_SCATTARE, oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI"));
                                break;
                            case "CasellaDiSelezione":
                                fasc.AddProfileField(
                                tempSystemId,
                                oggettoCustom.SYSTEM_ID.ToString(),
                                new TextValue(oggettoCustom.DESCRIZIONE),
                                oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                new ElementFieldMultiValue(oggettoCustom.VALORI_SELEZIONATI.Select(s => new TextValue(s)).ToArray()));
                                break;
                            default:
                                fasc.AddProfileField(
                                tempSystemId,
                                oggettoCustom.SYSTEM_ID.ToString(),
                                new TextValue(oggettoCustom.DESCRIZIONE),
                                oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                new ElementFieldSingleValue(new TextValue(oggettoCustom.VALORE_DATABASE)));
                                break;
                        }
                    }

                    #region recupero diagramma

                    var idDiagramma = (await this._mediator.Send(new GetDiagrammaAssociatoFascCommand()
                    {
                        IdTipoFasc = template.SYSTEM_ID.ToString()
                    })).Output;

                    if (idDiagramma != 0)
                    {
                        setStatoDiagrammaIniziale = true;
                        diagramma = (await this._mediator.Send(new Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDiagrammaById.GetDiagrammaByIdCommand()
                        {
                            IdDiagramma = idDiagramma.ToString()
                        })).Output;
                        if (diagramma != null)
                        {
                            if (diagramma.STATI != null && diagramma.STATI.Count() > 0)
                            {
                                foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                {
                                    if (stato.STATO_INIZIALE == true) statoIniziale = stato;
                                }
                            }
                        }
                    }

                    #endregion
                }

                await _aggDocRepository.Add(fasc);

                if (fasc != null && !string.IsNullOrWhiteSpace(fasc.Id))
                {
                    var id_fascicolo = (from a in _pi3DbContext.ProjectEntities where a.SYSTEM_ID == fasc.Id.AsLong() select a.ID_FASCICOLO).FirstOrDefault();
                    if (id_fascicolo > 0)
                    {
                        response.Project= DBUtils.GetProjectWithArchivePlanFromDB(id_fascicolo.ToString(), _pi3DbContext);
                        await _loggerService.LogOK("FASCICOLAZIONENEWFASCICOLO",
                        id_fascicolo.ToString(), $"PIS REST: Creazione del Fascicolo ID: {id_fascicolo}, codice {response.Project.Code}",
                        null, infoUtente.codWorkingApplication);
                    }
                    else
                        throw new RestException("ERROR_PROJECT");

                }
                else
                {
                    throw new RestException("ERROR_PROJECT");
                }
                #endregion

                response.Code = GetProjectWithArchivePlanResponseCode.OK;

                _logger.LogInformation("end CreateProjectWithArchivePlan");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione CreateProjectWithArchivePlan: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new CreateProjectWithArchivePlanCommandResponse();
                response.Code = GetProjectWithArchivePlanResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione CreateProjectWithArchivePlan");
                response = new CreateProjectWithArchivePlanCommandResponse();
                response.Code = GetProjectWithArchivePlanResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<CreateProjectWithArchivePlanCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IAggregazioneDocumentaleRepository _aggDocRepository;
        protected readonly IWebMethodLoggerService _loggerService;
        protected readonly IConfigurationService _configurationService;

        #endregion
    }

}