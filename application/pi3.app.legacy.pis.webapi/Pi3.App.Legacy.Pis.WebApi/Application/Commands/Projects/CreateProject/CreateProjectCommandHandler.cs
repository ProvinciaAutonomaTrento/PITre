// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using Pi3.Core.AggregateModels.NotaAggregate;
using DocsPaVO.fascicolazione;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using StackExchange.Redis;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.GetTemplateFascById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetTemplateByDescrizione;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetTemplateFromPisVisibility;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetDiagrammaAssociatoFasc;
using DocsPaVO.DiagrammaStato;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SalvaModificaStatoFasc;
using DocsPaVO;
using Pi3.Core.Services.Configuration;


namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.CreateProject
{
    // Richiede libreria MediatR
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, CreateProjectCommandResponse>
    {
        #region Public Members

        public CreateProjectCommandHandler(IConfigurationService configurationService, INotaRepository repositoryNota, ILogger<CreateProjectCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IAggregazioneDocumentaleRepository aggDocRepository, IWebMethodLoggerService loggerService)
        {
            this._configurationService = configurationService;
            this._repositoryNota = repositoryNota;
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._aggDocRepository = aggDocRepository;
            this._loggerService = loggerService;
        }

        public async Task<CreateProjectCommandResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("CreateProject - START");

            CreateProjectCommandResponse response = new CreateProjectCommandResponse();
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
                        registro = classificazione.ID_REGISTRO.HasValue ? DBUtils.getRegistro(classificazione.ID_REGISTRO.ToString(), _pi3DbContext) : null;
                    }
                }
                else
                {
                    throw new RestException("GENERAL_NODE_NOT_FOUND");
                }


                var pisArchivePlans = from a in _pi3DbContext.PisAppsArchivePlanEntities where a.COD_CLASSIFICA == classificazione.VAR_CODICE && (registro == null || a.ID_REGISTRO == registro.systemId.AsLong()) && a.ID_AMM == infoUtente.idAmministrazione.AsLong() select a;
                ArchivePlan piano = null;
                long? idPianoArchiviazione = null;
                idPianoArchiviazione = pisArchivePlans.Where(x =>
                (x.ID_PEOPLE == infoUtente.idPeople.AsLong() ||
                x.ID_GRUPPO == infoUtente.idGruppo.AsLong() ||
                x.CODE_APPLICATION.ToUpper() == infoUtente.codWorkingApplication.ToUpper()) &&
                x.COD_CLASSIFICA.ToLower() == request.Project.CodeNodeClassification.ToLower() &&
                x.ID_TIPO_FASC == (request.Project.Template != null && !string.IsNullOrEmpty(request.Project.Template.Id) ? request.Project.Template.Id.AsLong() : null)
                ).Select(x => x.ID_PIANO_CONSERVAZIONE).FirstOrDefault();


                (var pianoDiConservazioneEnabled, bool keyFound) = await this._configurationService.TryGetValue<string>(infoUtente.idAmministrazione, "ENABLE_PIANO_CONSERVAZIONE");
                if (keyFound && "1".Equals(pianoDiConservazioneEnabled))
                {
                    if (idPianoArchiviazione == null)
                    {
                        throw new RestException("ARCHIVEPLAN_NOT_FOUND");
                    }
                    piano = DBUtils.GetArchivePlanFromDB(idPianoArchiviazione.ToString(), _pi3DbContext);
                    if (piano == null || string.IsNullOrWhiteSpace(piano.Id))
                        throw new RestException("ARCHIVEPLAN_NOT_FOUND");
                }

                TipiAggregazioneEnum tipiAggregazione = piano != null ? TipiAggregazioneEnum.SerieDocumentale : TipiAggregazioneEnum.Fascicolo;
                TipologieFascicoloEnum? tipologiaFascicolo = tipiAggregazione == TipiAggregazioneEnum.Fascicolo ? TipologieFascicoloEnum.ProcedimentoAmministrativo : null;

                AggregazioneDocumentale fasc = new AggregazioneDocumentale(infoUtente.idAmministrazione, DateTime.Now,
                    new Core.SeedWork.TextValue(request.Project.Description), tipiAggregazione, tipologiaFascicolo,
                    request.Project.Private ? TipologieVisibilitaEnum.Privata : TipologieVisibilitaEnum.Gerarchica);


                if (piano is not null)
                {
                    fasc.AddClassification(classificazione.SYSTEM_ID.ToString(), new Core.SeedWork.TextValue(classificazione.DESCRIPTION), piano.Id, new Core.SeedWork.TextValue(piano.Description));
                }
                else
                {
                    fasc.AddClassification(classificazione.SYSTEM_ID.ToString(), new Core.SeedWork.TextValue(classificazione.DESCRIPTION));
                }
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
                string systemId = string.Empty;
                if (!string.IsNullOrEmpty(fasc.Id))
                {
                    var projectEntity = await this._pi3DbContext.ProjectEntities.AsNoTracking()
                        .Where(p => p.SYSTEM_ID == fasc.Id.AsLong())
                        .Select(p => new
                        {
                            p.ID_FASCICOLO,
                            p.DESCRIPTION
                        }).FirstAsync();

                    systemId = projectEntity.ID_FASCICOLO.ToString();
                }
                #region aggiunta note
                if (request.Project.Note != null)
                {
                    foreach (var note in request.Project.Note)
                    {
                        var aggregateNota = new Pi3.Core.AggregateModels.NotaAggregate.Nota(infoUtente.idAmministrazione, DateTime.Now, new TextValue(note.Description), null,
                            new AutoreNota()
                            {
                                IdUtente = infoUtente.idPeople,
                                IdRuolo = infoUtente.idGruppo,
                                IdUtenteDelegato = string.Empty
                            },
                            systemId,
                            TipiOggettoEnum.Fascicolo,
                            TipoAccessoNotaEnum.Pubblica,
                            request.Project.Register != null ? request.Project.Register.Id : null
                            );

                        await this._repositoryNota.Add(aggregateNota);
                    }
                }

                #endregion

                if (fasc != null && !string.IsNullOrWhiteSpace(fasc.Id))
                {
                    var id_fascicolo = (from a in _pi3DbContext.ProjectEntities where a.SYSTEM_ID == fasc.Id.AsLong() select a.ID_FASCICOLO).FirstOrDefault();
                    if (id_fascicolo > 0)
                    {
                        response.Project = DBUtils.getProjectFromDB(id_fascicolo.ToString(), _pi3DbContext);
                        await _loggerService.LogOK("FASCICOLAZIONENEWFASCICOLO",
                        id_fascicolo.ToString(), $"PIS REST: Creazione del Fascicolo ID: {id_fascicolo}, codice {response.Project.Code}",
                        null, infoUtente.codWorkingApplication);
                    }
                    else
                        throw new RestException("ERROR_PROJECT");


                    #region set statediagram
                    if (setStatoDiagrammaIniziale)
                    {
                        await this._mediator.Send(new SalvaModificaStatoFascCommand()
                        {
                            IdProject = id_fascicolo.ToString(),
                            IdStato = statoIniziale.SYSTEM_ID.ToString(),
                            Diagramma = diagramma,
                            User = infoUtente,
                            IdUtente = infoUtente.idPeople
                        });
                    }

                    response.Project = DBUtils.getProjectFromDB(id_fascicolo.ToString(), this._pi3DbContext);
                    if (setStatoDiagrammaIniziale)
                    {
                        response.Project.Template.StateDiagram.StateOfDiagram[0] = DBUtils.GetStateOfDiagram(statoIniziale, diagramma.SYSTEM_ID.ToString());
                    }

                    #endregion
                }
                else
                {
                    throw new RestException("ERROR_PROJECT");
                }



                #endregion
                response.Code = GetProjectResponseCode.OK;

                _logger.LogInformation("end CreateProject");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione CreateProject: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new CreateProjectCommandResponse();
                response.Code = GetProjectResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione CreateProject");
                response = new CreateProjectCommandResponse();
                response.Code = GetProjectResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<CreateProjectCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IAggregazioneDocumentaleRepository _aggDocRepository;
        protected readonly IWebMethodLoggerService _loggerService;
        protected INotaRepository _repositoryNota;
        protected readonly IConfigurationService _configurationService;

        #endregion
    }

}