// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DiagrammaStato;
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetDettaglioDocumentoNoSecurity;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDiagrammaById;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetStatoDoc;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.ImportPreviousDocument;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SalvaModificaStato;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecAutoTrasmByIdStatus;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;
using System.Security.Cryptography.Xml;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.EditDocStateDiagram
{
    // Richiede libreria MediatR
    public class EditDocStateDiagramCommandHandler : IRequestHandler<EditDocStateDiagramCommand, EditDocStateDiagramCommandResponse>
    {
        #region Public Members

        public EditDocStateDiagramCommandHandler(ILogger<EditDocStateDiagramCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IWebMethodLoggerService loggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._loggerService = loggerService;
        }

        public async Task<EditDocStateDiagramCommandResponse> Handle(EditDocStateDiagramCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("EditDocStateDiagram - START");

            EditDocStateDiagramCommandResponse response = new EditDocStateDiagramCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                {
                    throw new RestException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.IdDocument) && !string.IsNullOrEmpty(request.Signature))
                {
                    throw new RestException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                if (string.IsNullOrEmpty(request.StateOfDiagram))
                {
                    throw new RestException("REQUIRED_STATE_OF_DIAGRAM");
                }
                #endregion

                #region implementazione
                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();
                // Controllo visibilit� documento
                try
                {
                    long? idProfile = !string.IsNullOrEmpty(request.IdDocument) ? request.IdDocument.AsLong() : null;
                    if (idProfile == null && !string.IsNullOrEmpty(request.Signature))
                    {
                        idProfile = await _pi3DbContext.ProfileEntities.AsNoTracking()
                                .Where(p => p.VAR_SEGNATURA != null && p.VAR_SEGNATURA.ToUpper() == request.Signature.ToUpper())
                                .Select(p => p.SYSTEM_ID)
                                .FirstOrDefaultAsync();
                    }

                    await _pi3DbContext.AssertSecurityRights(idProfile.ToString(), infoUtente.idPeople, infoUtente.idGruppo);

                    documento = (await this._mediator.Send(new DocumentoGetDettaglioDocumentoNoSecurityCommand()
                    {
                        Infoutente = infoUtente,
                        DocNumber = idProfile.ToString(),
                        IdProfile = idProfile.ToString()
                    })).Output;
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                if (documento != null)
                {
                    if (documento.template != null)
                    {
                        
                        DocsPaVO.DiagrammaStato.DiagrammaStato diagramma = null;
                        DocsPaVO.DiagrammaStato.Stato statoAttuale = null;
                        var idDiagramma = DBUtils.GetDiagrammaAssociato(documento.template.SYSTEM_ID.ToString(), this._pi3DbContext);
                        if( idDiagramma != 0)
                        {
                            diagramma = (await this._mediator.Send(new GetDiagrammaByIdCommand()
                            {
                                IdDiagramma = idDiagramma.ToString()
                            })).Output;

                            if (diagramma != null)
                            {
                                if (diagramma.STATI != null && diagramma.STATI.Count() > 0)
                                {
                                    statoAttuale = (await this._mediator.Send( new GetStatoDocCommand()
                                    {
                                        DocNumber = documento.docNumber
                                    })).Output;
                                    bool result = false;

                                    if (statoAttuale == null)
                                    {
                                        foreach (DocsPaVO.DiagrammaStato.Stato stato in diagramma.STATI)
                                        {
                                            if (stato.DESCRIZIONE.ToUpper().Equals(request.StateOfDiagram.ToUpper()))
                                            {
                                                var salvaRes = (await this._mediator.Send(new SalvaModificaStatoCommand()
                                                {
                                                    DocNumber = documento.docNumber,
                                                    IdStato = stato.SYSTEM_ID.ToString(),
                                                    Diagramma = diagramma,
                                                    IdUtente = infoUtente.idPeople,
                                                    User = infoUtente,
                                                    dataScadenza = string.Empty
                                                }));
                                                result = true;
                                                await this._mediator.Send(new ExecAutoTrasmByIdStatusCommand()
                                                {
                                                    InfoUt = infoUtente,
                                                    Doc = documento,
                                                    Stato = stato,
                                                    IdTemplate = documento.template.SYSTEM_ID.ToString()
                                                });

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
                                                    if (((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).DESCRIZIONE.ToUpper().Equals(request.StateOfDiagram.ToUpper()))
                                                    {
                                                        var salvaRes = (await this._mediator.Send(new SalvaModificaStatoCommand()
                                                        {
                                                            DocNumber = documento.docNumber,
                                                            IdStato = ((DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j]).SYSTEM_ID.ToString(),
                                                            Diagramma = diagramma,
                                                            IdUtente = infoUtente.idPeople,
                                                            User = infoUtente,
                                                            dataScadenza = string.Empty
                                                        }));
                                                        result = true;
                                                        await this._mediator.Send(new ExecAutoTrasmByIdStatusCommand()
                                                        {
                                                            InfoUt = infoUtente,
                                                            Doc = documento,
                                                            Stato = (DocsPaVO.DiagrammaStato.Stato)step.SUCCESSIVI[j],
                                                            IdTemplate = documento.template.SYSTEM_ID.ToString()
                                                        });
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    response.ResultMessage = string.Format(Messages.ResMsg, documento.systemId, request.StateOfDiagram);
                                    if (!result)
                                    {
                                        throw new RestException("STATEOFDIAGRAM_NOT_FOUND");
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(request.StateOfDiagram))
                                        {
                                            await _loggerService.LogOK("DOC_CAMBIO_STATO",
                                            documento.systemId, string.Format(Messages.StateReached, request.StateOfDiagram.ToUpper()),
                                            null, infoUtente.codWorkingApplication);
                                        }
                                        else
                                        {
                                            await _loggerService.LogOK("DOC_CAMBIO_STATO",
                                            documento.systemId, string.Format(Messages.StateUp, documento.systemId),
                                            null, infoUtente.codWorkingApplication);
                                        }
                                    }
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
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }
                #endregion

                response.Code = MessageResponseCode.OK;

                _logger.LogInformation("end EditDocStateDiagram");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione EditDocStateDiagram: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new EditDocStateDiagramCommandResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione EditDocStateDiagram");
                response = new EditDocStateDiagramCommandResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<EditDocStateDiagramCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IWebMethodLoggerService _loggerService;

        #endregion
    }

}