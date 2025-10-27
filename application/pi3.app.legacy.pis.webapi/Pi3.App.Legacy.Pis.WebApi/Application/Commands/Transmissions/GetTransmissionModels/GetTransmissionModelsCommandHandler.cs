// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.DocumentMetadata.DocumentoInformatico;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.GetTransmissionModels
{
    // Richiede libreria MediatR
    public class GetTransmissionModelsCommandHandler : IRequestHandler<GetTransmissionModelsCommand, GetTransmissionModelsCommandResponse>
    {
        #region Public Members

        public GetTransmissionModelsCommandHandler(ILogger<GetTransmissionModelsCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetTransmissionModelsCommandResponse> Handle(GetTransmissionModelsCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetTransmissionModels - START");

            GetTransmissionModelsCommandResponse response = new GetTransmissionModelsCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controllo parametri richiesta
                string tipo = string.Empty;
                if (!string.IsNullOrEmpty(request.Type) && (request.Type.ToUpper().Equals("D") || request.Type.ToUpper().Equals("F")))
                {
                    tipo = request.Type;
                }
                else
                {
                    //Tipo non trovato
                    throw new RestException("TRANSMISSION_MODEL_TYPE");
                }
                if (request.Registers == null || !request.Registers.Any())
                {
                    throw new RestException("REQUIRED_REGISTER");
                }
                #endregion

                #region implementazione
                var registri = new List<DocsPaVO.utente.Registro>();
                foreach (Register regTemp in request.Registers)
                {
                    var reg = DBUtils.getRegistroByCodAOO(regTemp.Code, infoUtente.idAmministrazione, _pi3DbContext);
                    if (reg != null)
                    {
                        registri.Add(reg);
                    }
                    else
                    {
                        //Registro mancante
                        throw new RestException("REGISTER_NOT_FOUND");
                    }
                }
                List<ModelloTrasmEntity> modelloTrasmEntities = new List<ModelloTrasmEntity>();
                var idTenant = infoUtente.idAmministrazione.AsLong();
                var idPeople = infoUtente.idPeople.AsLong();
                var idCorrGlobali = infoUtente.idCorrGlobali.AsLong();


                var modelliTrasmQueryable = this._pi3DbContext.ModelloTrasmEntities.AsNoTracking().Where(m => m.ID_AMM == idTenant && m.CHA_TIPO_OGGETTO == request.Type);

                modelliTrasmQueryable = modelliTrasmQueryable.Where(m =>
                this._pi3DbContext.ModelloMittDestEntities.AsNoTracking()
                .Join(this._pi3DbContext.CorrGlobaliEntities.AsNoTracking(), d => d.ID_CORR_GLOBALI, c => c.SYSTEM_ID, (d, c) => new { d, c })
                .Count(j => j.d.ID_MODELLO == m.SYSTEM_ID && j.d.CHA_TIPO_URP == "R" && j.d.CHA_TIPO_MITT_DEST == "D" && (j.c.CHA_DISABLED_TRASM == "1" || j.c.DTA_FINE != null)) == 0);

                if (registri != null && registri.Any())
                {
                    List<long> idRegistri = new List<long>();
                    foreach (var reg in registri)
                    {
                        if (reg.chaRF == "0")
                            idRegistri.Add(reg.systemId.AsLong()); 
                    }


                    if (idRegistri.Any())
                        modelliTrasmQueryable = modelliTrasmQueryable.Where(m => idRegistri.Contains(m.ID_REGISTRO ?? 0)); 
                }

                modelliTrasmQueryable = modelliTrasmQueryable.Where(x => !this._pi3DbContext.ModelloMittDestEntities.AsNoTracking().Any(d => d.ID_MODELLO == x.SYSTEM_ID && d.HIDE_DOC_VERSIONS == "1"));

                var modelliTrasmQueryable_1 = modelliTrasmQueryable.Where(m => m.SINGLE == "1" && !this._pi3DbContext.AssDiagrammiEntities.AsNoTracking().Any(d => d.ID_MOD_TRASM == m.SYSTEM_ID));

                var modelliTrasmQueryable_2 = modelliTrasmQueryable.Join(this._pi3DbContext.ModelloMittDestEntities, m => m.SYSTEM_ID, d => d.ID_MODELLO, (m, d) => new { m, d })
                    .Where(j => !this._pi3DbContext.AssDiagrammiEntities.AsNoTracking().Any(d => d.ID_MOD_TRASM == j.m.SYSTEM_ID)
                            && (j.m.ID_PEOPLE == idPeople || j.m.ID_PEOPLE == null)
                            && (j.d.ID_CORR_GLOBALI == 0 || j.d.ID_CORR_GLOBALI == idCorrGlobali)
                            && j.d.CHA_TIPO_MITT_DEST == "M" && j.m.SINGLE == "0");

                modelloTrasmEntities.AddRange(await modelliTrasmQueryable_1.OrderBy(m => m.NOME).ToListAsync());

                modelloTrasmEntities.AddRange(await modelliTrasmQueryable_2.OrderBy(j => j.m.NOME).Select(j => j.m).ToListAsync());

                if (modelloTrasmEntities != null && modelloTrasmEntities.Any())
                {
                    List<TransmissionModel> models = new List<TransmissionModel>();
                    foreach (var entity in modelloTrasmEntities)
                    {
                        models.Add(new TransmissionModel()
                        {
                            Code = entity.CODICE,
                            Id = entity.SYSTEM_ID.ToString(),
                            Description = entity.NOME,
                            Type = entity.CHA_TIPO_OGGETTO
                        });
                    }
                    response.TransmissionModels = models.ToArray();
                }
                else
                {
                    //Modelli di trasmisssione non trovati
                    throw new RestException("TRANSMISSION_MODELS_NOT_FOUND");
                }


                #endregion

                response.Code = GetTransmModelsResponseCode.OK;

                _logger.LogInformation("end GetTransmissionModels");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetTransmissionModels: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetTransmissionModelsCommandResponse();
                response.Code = GetTransmModelsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetTransmissionModels");
                response = new GetTransmissionModelsCommandResponse();
                response.Code = GetTransmModelsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetTransmissionModelsCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
