// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocAccessRights
{
    // Richiede libreria MediatR
    public class GetDocAccessRightsCommandHandler : IRequestHandler<GetDocAccessRightsCommand, GetDocAccessRightsCommandResponse>
    {
        #region Public Members

        public GetDocAccessRightsCommandHandler(ILogger<GetDocAccessRightsCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetDocAccessRightsCommandResponse> Handle(GetDocAccessRightsCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetDocAccessRights - START");

            GetDocAccessRightsCommandResponse response = new GetDocAccessRightsCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                List<ObjectAccessRight> diritti = new List<ObjectAccessRight>();
                try
                {
                    if (!string.IsNullOrEmpty(request.IdDocument))
                    {
                        // verificare visibilit� documento
                        await _pi3DbContext.AssertSecurityRights(request.IdDocument, infoUtente.idPeople, infoUtente.idGruppo);
                    }
                    else
                    {
                        throw new RestException("REQUIRED_ID");
                    }
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                var query = from a in _pi3DbContext.SecurityEntities
                                join b in _pi3DbContext.CorrGlobaliEntities on a.PERSONORGROUP equals b.ID_PEOPLE into bGroup
                                from b in bGroup.DefaultIfEmpty()
                                join c in _pi3DbContext.CorrGlobaliEntities on a.PERSONORGROUP equals c.ID_GRUPPO into cGroup
                                from c in cGroup.DefaultIfEmpty()
                                where a.THING == request.IdDocument.AsLong()
                                select new
                                {
                                    Sec = a,
                                    CorrPeople = b,
                                    CorrRole = c
                                };

                    if (query != null && query.Any())
                    {
                        ObjectAccessRight oar = null;
                        foreach (var diritto in query)
                        {
                            oar = new ObjectAccessRight();
                            oar.IdObject = diritto.Sec.THING.ToString();
                            oar.AccessDate = diritto.Sec.TS_INSERIMENTO?.ToString("dd/MM/yyyy HH:mm:ss");
                            oar.AccessRights = diritto.Sec.ACCESSRIGHTS.ToString();
                            switch (diritto.Sec.CHA_TIPO_DIRITTO)
                            {
                                case "A":
                                    oar.AccessRightsType = "Acquired";
                                    break;
                                case "C":
                                    oar.AccessRightsType = "Storage";
                                    break;
                                case "D":
                                    oar.AccessRightsType = "Delegate";
                                    break;
                                case "P":
                                    oar.AccessRightsType = "Owner";
                                    break;
                                case "T":
                                    oar.AccessRightsType = "Transmission";
                                    break;
                                case "F":
                                    oar.AccessRightsType = "Project Transmission";
                                    break;
                                default:
                                    oar.AccessRightsType = "Acquired";
                                    break;
                            }
                            oar.Note = diritto.Sec.VAR_NOTE_SEC;
                            oar.SubjectCode = diritto.CorrRole!= null && !string.IsNullOrEmpty(diritto.CorrRole.VAR_COD_RUBRICA) ? diritto.CorrRole.VAR_COD_RUBRICA : diritto.CorrPeople.VAR_COD_RUBRICA;
                            oar.SubjectDescription = diritto.CorrRole != null && !string.IsNullOrEmpty(diritto.CorrRole.VAR_COD_RUBRICA) ? diritto.CorrRole.VAR_DESC_CORR : diritto.CorrPeople.VAR_DESC_CORR;
                            oar.SubjectType = diritto.CorrRole != null && !string.IsNullOrEmpty(diritto.CorrRole.VAR_COD_RUBRICA) ? diritto.CorrRole.CHA_TIPO_URP : diritto.CorrPeople.CHA_TIPO_URP;
                            oar.SubjectId = diritto.CorrRole != null && !string.IsNullOrEmpty(diritto.CorrRole.VAR_COD_RUBRICA) ? diritto.CorrRole.SYSTEM_ID.ToString() : diritto.CorrPeople.SYSTEM_ID.ToString();

                            diritti.Add(oar);

                        }
                        response.AccessRights = diritti;
                       
                    }
                    else
                    {
                        throw new Exception("Errore nel reperimendo dei diritti");
                    }
                

                #endregion

                response.Code = GetDocAccessRightsResponseCode.OK;

                _logger.LogInformation("end GetDocAccessRights");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetDocAccessRights: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetDocAccessRightsCommandResponse();
                response.Code = GetDocAccessRightsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetDocAccessRights");
                response = new GetDocAccessRightsCommandResponse();
                response.Code = GetDocAccessRightsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocAccessRightsCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}