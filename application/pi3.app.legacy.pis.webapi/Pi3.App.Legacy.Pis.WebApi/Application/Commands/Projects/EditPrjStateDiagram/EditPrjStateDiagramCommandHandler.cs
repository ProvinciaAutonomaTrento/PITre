// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DiagrammaStato;
using DocsPaVO.fascicolazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Org.BouncyCastle.Math.EC;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDiagrammaById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.GetTemplateFascById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetFascicoloById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetDiagrammaAssociatoFasc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetFascicoloDaCodiceNoSecurity;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetStatoFasc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SalvaModificaStatoFasc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.EditPrjStateDiagram
{
    // Richiede libreria MediatR
    public class EditPrjStateDiagramCommandHandler : IRequestHandler<EditPrjStateDiagramCommand, EditPrjStateDiagramCommandResponse>
    {
        #region Public Members

        public EditPrjStateDiagramCommandHandler(ILogger<EditPrjStateDiagramCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<EditPrjStateDiagramCommandResponse> Handle(EditPrjStateDiagramCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("EditPrjStateDiagram - START");
            var classificationSchemeId = request.ClassificationSchemeId;
            var codeProject = request.CodeProject;
            var idProject = request.IdProject;
            var stateOfDiagram = request.StateOfDiagram;

            EditPrjStateDiagramCommandResponse response = new EditPrjStateDiagramCommandResponse();
            DocsPaVO.fascicolazione.Fascicolo fascicolo = new DocsPaVO.fascicolazione.Fascicolo();

            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli
                if (string.IsNullOrEmpty(classificationSchemeId) && string.IsNullOrEmpty(codeProject) && string.IsNullOrEmpty(idProject))
                {
                    throw new RestException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(classificationSchemeId) || !string.IsNullOrEmpty(codeProject)) && !string.IsNullOrEmpty(idProject))
                {
                    throw new RestException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(classificationSchemeId) && string.IsNullOrEmpty(codeProject)) || (string.IsNullOrEmpty(classificationSchemeId) && !string.IsNullOrEmpty(codeProject)))
                {
                    throw new RestException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }

                if (string.IsNullOrEmpty(stateOfDiagram))
                {
                    throw new RestException("REQUIRED_STATE_OF_DIAGRAM");
                }
                #endregion

                #region implementazione

                try
                {
                    if (!string.IsNullOrEmpty(idProject))
                    {
                        fascicolo = (await this._mediator.Send(new FascicolazioneGetFascicoloByIdCommand()
                        {
                            InfoUtente = infoUtente,
                            IdFascicolo = idProject
                        })).Output;
                    }
                    else if (!string.IsNullOrEmpty(codeProject) && (!string.IsNullOrEmpty(classificationSchemeId)))
                    {
                        DocsPaVO.fascicolazione.Fascicolo[] fascicoli = (await this._mediator.Send(new GetFascicoloDaCodiceNoSecurityCommand()
                        {
                            CodiceFasc = codeProject,
                            IdAmm = infoUtente.idAmministrazione,
                            Titolari = classificationSchemeId,
                            SoloGenerali = false,
                        })).Output; 

                        if (fascicoli != null && fascicoli.Length > 0)
                        {
                            fascicolo = (await this._mediator.Send(new FascicolazioneGetFascicoloByIdCommand()
                            {
                                InfoUtente = infoUtente,
                                IdFascicolo = fascicoli[0].systemID
                            })).Output;
                        }
                        else
                        {
                            throw new RestException("PROJECT_NOT_FOUND");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }

                if (fascicolo != null)
                {
                    var idTemp = await this._pi3DbContext.ProjectEntities.AsNoTracking().Where(p => p.SYSTEM_ID == fascicolo.systemID.AsLong() && p.ID_TIPO_FASC.HasValue).Select(p => p.ID_TIPO_FASC).FirstOrDefaultAsync();


                    fascicolo.template = (await this._mediator.Send(new GetTemplateFascByIdCommand()
                    {
                        IdTemplate = idTemp != null ? idTemp.ToString() : string.Empty
                    })).Output;

                    if (fascicolo.template != null)
                    {
                        int idDiagramma = 0;
                        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                        DocsPaVO.DiagrammaStato.Stato statoAttuale = null;
                        idDiagramma = (await this._mediator.Send(new GetDiagrammaAssociatoFascCommand()
                        {
                            IdTipoFasc = fascicolo.template.SYSTEM_ID.ToString()
                        })).Output;

                        if (idDiagramma != 0)
                        {
                            diagramma = (await this._mediator.Send(new GetDiagrammaByIdCommand()
                            {
                                IdDiagramma = idDiagramma.ToString()
                            })).Output;

                            if (diagramma != null &&
                               (diagramma.STATI != null && diagramma.STATI.Count() > 0))
                            {
                                statoAttuale = ( await this._mediator.Send(new GetStatoFascCommand()
                                {
                                    IdProject = fascicolo.systemID
                                })).Output;
                                bool result = false;

                                if(statoAttuale == null)
                                {
                                    foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                    {
                                        if (stato.DESCRIZIONE.ToUpper().Equals(stateOfDiagram.ToUpper()))
                                        {
                                            await this._mediator.Send(new SalvaModificaStatoFascCommand()
                                            {
                                                IdProject = fascicolo.systemID,
                                                IdStato = stato.SYSTEM_ID.ToString(),
                                                Diagramma = diagramma,
                                                IdUtente = infoUtente.idPeople,
                                                User = infoUtente,
                                                DataScadenza = string.Empty
                                            });
                                            result = true;
                                            response.ResultMessage = string.Format(Resource.SuccessfullStateUpdate, fascicolo.codice, stateOfDiagram);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < diagramma.PASSI.Count(); i++)
                                    {
                                        DocsPaVO.DiagrammaStato.Passo step = (DocsPaVO.DiagrammaStato.Passo)diagramma.PASSI[i];
                                        if (step.STATO_PADRE.SYSTEM_ID == statoAttuale.SYSTEM_ID)
                                        {
                                            for (int j = 0; j < step.SUCCESSIVI.Count(); j++)
                                            {
                                                if (((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).DESCRIZIONE.ToUpper().Equals(stateOfDiagram.ToUpper()))
                                                {
                                                    await this._mediator.Send(new SalvaModificaStatoFascCommand()
                                                    {
                                                        IdProject = fascicolo.systemID,
                                                        IdStato = ((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).SYSTEM_ID.ToString(),
                                                        Diagramma = diagramma,
                                                        IdUtente = infoUtente.idPeople,
                                                        User = infoUtente,
                                                        DataScadenza = string.Empty
                                                    });
                                                    result = true;
                                                    response.ResultMessage = string.Format(Resource.SuccessfullStateUpdate, fascicolo.codice, stateOfDiagram);

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!result)
                                {
                                    throw new RestException("STATEOFDIAGRAM_NOT_FOUND");
                                }
                            }
                        }
                        else
                        {
                            throw new RestException("DIAGRAM_NOT_FOUND");
                        }
                    }
                    else
                    {
                        throw new RestException("TEMPLATE_NOT_FOUND");
                    }
                }
                else
                {
                    throw new RestException("PROJECT_NOT_FOUND");
                }
                #endregion

                response.Code = Documents.MessageResponseCode.OK;

                _logger.LogInformation("end EditPrjStateDiagram");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione EditPrjStateDiagram: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new EditPrjStateDiagramCommandResponse();
                response.Code = Documents.MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione EditPrjStateDiagram");
                response = new EditPrjStateDiagramCommandResponse();
                response.Code = Documents.MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<EditPrjStateDiagramCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}