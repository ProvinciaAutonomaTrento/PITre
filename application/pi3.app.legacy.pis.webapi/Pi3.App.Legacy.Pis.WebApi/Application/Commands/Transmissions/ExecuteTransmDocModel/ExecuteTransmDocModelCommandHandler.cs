// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Modelli_Trasmissioni;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetDettaglioDocumentoNoSecurity;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetModelloByID;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetModelliPerTrasmLite;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.TransmissionExecuteDocTransmFromModelCodeSoloConNotifica;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmDocModel
{
    // Richiede libreria MediatR
    public class ExecuteTransmDocModelCommandHandler : IRequestHandler<ExecuteTransmDocModelCommand, ExecuteTransmDocModelCommandResponse>
    {
        #region Public Members

        public ExecuteTransmDocModelCommandHandler(ILogger<ExecuteTransmDocModelCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<ExecuteTransmDocModelCommandResponse> Handle(ExecuteTransmDocModelCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("ExecuteTransmDocModel - START");

            ExecuteTransmDocModelCommandResponse response = new ExecuteTransmDocModelCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region Controllo parametri richiesta
                if (string.IsNullOrEmpty(request.DocumentId))
                {
                    throw new RestException("REQUIRED_ID");
                }


                if (string.IsNullOrEmpty(request.IdModel))
                {
                    throw new RestException("REQUIRED_ID_MODEL");
                }
                #endregion

                #region implementazione
                DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione model = new DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione();
                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();
                // Controllo visibilità documento
                try
                {
                    long? idProfile = !string.IsNullOrEmpty(request.DocumentId) ? request.DocumentId.AsLong() : null;

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
                bool controlloPerUtDiSistema = true;

                if (documento != null)
                {
                    try
                    {
                        DocsPaVO.utente.Registro[] registri = null;
                        ArrayList registriArr = DBUtils.getListaRegistriRfRuolo(ruolo.systemId, "0", this._pi3DbContext);

                        if (registriArr != null && registriArr.Count > 0)
                        {
                            int i = 0;
                            registri = new DocsPaVO.utente.Registro[registriArr.Count];
                            foreach (DocsPaVO.utente.Registro reg in registriArr)
                            {
                                registri[i] = reg;
                                i++;
                            }
                        }
                        var modReqDoc = new GetModelliPerTrasmLiteCommand()
                        {
                            IdAmm = infoUtente.idAmministrazione,
                            Registri = registri ?? new DocsPaVO.utente.Registro[0],
                            IdPeople = infoUtente.idPeople,
                            IdCorrGlobali = infoUtente.idCorrGlobali,
                            IdRuoloUtente = infoUtente.idGruppo,
                            ChaTipoOggetto = "D"
                        };
                        var modReqFasc = new GetModelliPerTrasmLiteCommand()
                        {
                            IdAmm = infoUtente.idAmministrazione,
                            Registri = registri ?? new DocsPaVO.utente.Registro[0],
                            IdPeople = infoUtente.idPeople,
                            IdCorrGlobali = infoUtente.idCorrGlobali,
                            IdRuoloUtente = infoUtente.idGruppo,
                            ChaTipoOggetto = "F"
                        };
                        var modelliResp = (await this._mediator.Send(modReqDoc)).Output;
                        var modelliFascResp = (await this._mediator.Send(modReqFasc)).Output;

                        List<ModelloTrasmissione> modelli = new();
                        if(modelliResp != null)
                        {
                            modelli = modelliResp.ToList();
                        }
                        if(modelliFascResp != null)
                        {
                            modelli.AddRange(modelliFascResp.ToList());
                        }
                        string idModello = request.IdModel;
                        foreach (DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione mod in modelli)
                        {
                            if (mod.SYSTEM_ID.ToString().Equals(idModello))
                            {
                                model = (await this._mediator.Send(new GetModelloByIDCommand()
                                {
                                    IdAmm = infoUtente.idAmministrazione, 
                                    IdModello = idModello
                                })).Output;
                                if (!string.IsNullOrEmpty(infoUtente.diSistema) && infoUtente.diSistema == "1")
                                {
                                    foreach (DocsPaVO.Modelli_Trasmissioni.RagioneDest rdest in model.RAGIONI_DESTINATARI)
                                    {
                                        if (rdest.RAGIONE != "COMPETENZA_SIST_ESTERNI") 
                                        { 
                                            controlloPerUtDiSistema = false; 
                                            break; 
                                        }
                                    }
                                }
                                break;
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        throw new RestException("DOCUMENT_NOT_FOUND");
                    }
                }
                if (!controlloPerUtDiSistema)
                {
                    throw new Exception(Messages.InvalidTrasm);
                }

                bool success = false;

                if (model.SYSTEM_ID != 0)
                {
                    success = (await this._mediator.Send(new TransmissionExecuteDocTransmFromModelCodeSoloConNotificaCommand()
                    {
                        InfoUtente = infoUtente,
                        Documento = documento,
                        ModelCode = model.CODICE,
                        Role = ruolo
                    })).Output;
                }
                else
                {
                    throw new RestException("TRANSMISSION_MODEL_NOT_FOUND");
                }

                if (!success)
                {
                    throw new Exception("APPLICATION_ERROR");
                }
                #endregion

                response.Code = TransmissionResponseCode.OK;
                response.TransmMessage = Messages.TrasmExecd;

                _logger.LogInformation("end ExecuteTransmDocModel");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione ExecuteTransmDocModel: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new ExecuteTransmDocModelCommandResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione ExecuteTransmDocModel");
                response = new ExecuteTransmDocModelCommandResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<ExecuteTransmDocModelCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
