// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetSignatureProcesses
{
    // Richiede libreria MediatR
    public class GetSignatureProcessesCommandHandler : IRequestHandler<GetSignatureProcessesCommand, GetSignatureProcessesCommandResponse>
    {
        #region Public Members

        public GetSignatureProcessesCommandHandler(ILogger<GetSignatureProcessesCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetSignatureProcessesCommandResponse> Handle(GetSignatureProcessesCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetSignatureProcesses - START");

            GetSignatureProcessesCommandResponse response = new GetSignatureProcessesCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione

                /*
                var processoEntities = await this._pi3DbContext.SchemaProcessoFirmaEntities.AsNoTracking()
                    .Where(p => p.RUOLO_AUTORE == infoUtente.idGruppo.AsLong())
                    .OrderByDescending(p => p.DTA_CREAZIONE)
                    .ThenByDescending(p => p.ID_PROCESSO)
                    .ToListAsync();
                */

                var tipoVisProprietario = new List<string> {"P", "T" };
                var tipoVisMonitoratore = new List<string> {"M", "T" };

                var processoEntities = await (from p in this._pi3DbContext.SchemaProcessoFirmaEntities.AsNoTracking()
                                       join v in this._pi3DbContext.ProcessoFirmaVisibilitaEntities.AsNoTracking()
                                            on p.ID_PROCESSO equals v.ID_PROCESSO into ProcessoVisibilita
                                       from pv in ProcessoVisibilita.DefaultIfEmpty()
                                       where pv.ID_GROUPS == infoUtente.idGruppo.AsLong() &&
                                            p.ELIMINATO == null //&&
                                            //tipoVisProprietario.Contains(pv.CHA_TIPO_VISIBILITA) &&
                                            //tipoVisMonitoratore.Contains(pv.CHA_TIPO_VISIBILITA) 
                                       orderby p.NOME.ToUpper()
                                       select p)
                                       .Distinct()
                                       .ToListAsync();

                if (processoEntities== null || processoEntities.Count<1)
                    throw new RestException("SIGN_PROCESS_NOT_FOUND");
                List<SignatureProcess> processiInResp = new List<SignatureProcess>();
                foreach(var proc in processoEntities)
                {
                    var processo = await SignBookUtils.GetProcessoFirma(proc.ID_PROCESSO, proc, _pi3DbContext);
                    processiInResp.Add(new SignatureProcess(processo));
                }
                response.Processes = processiInResp.ToArray();
                response.TotalProcessesNumber = processiInResp.Count;
                                
				#endregion
                
                response.Code = GetSignatureProcessesResponseCode.OK;

                _logger.LogInformation("end GetSignatureProcesses");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetSignatureProcesses: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetSignatureProcessesCommandResponse();
                response.Code = GetSignatureProcessesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetSignatureProcesses");
                response = new GetSignatureProcessesCommandResponse();
                response.Code = GetSignatureProcessesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetSignatureProcessesCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}