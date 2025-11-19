// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetIstanzaPassoFirmaInAttesaByDocnumber
{

    // Richiede libreria MediatR
    public class GetIstanzaPassoFirmaInAttesaByDocnumberHandler : IRequestHandler<Application.Requests.GetIstanzaPassoFirmaInAttesaByDocnumber, GetIstanzaPassoFirmaInAttesaByDocnumberResult>
    {
        #region Public Members

        public GetIstanzaPassoFirmaInAttesaByDocnumberHandler(ILogger<GetIstanzaPassoFirmaInAttesaByDocnumberHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<GetIstanzaPassoFirmaInAttesaByDocnumberResult> Handle(Application.Requests.GetIstanzaPassoFirmaInAttesaByDocnumber request, CancellationToken cancellationToken)
        {
            IstanzaPassoDiFirma output = null;

            try
            {
                var docnumber = request.docnumber.AsLong();

                var istanzaPassoFirmaEntity = await this._dbContext.IstanzaProcessoFirmaEntities.AsNoTracking()
                    .Join(this._dbContext.IstanzaPassoFirmaEntities, processo => processo.ID_ISTANZA, passo => passo.ID_ISTANZA_PROCESSO, (processo, passo) => new { processo, passo })
                    .Where(j => j.passo.STATO_PASSO == "LOOK" && j.processo.ID_DOCUMENTO == docnumber && j.processo.STATO == "IN_EXEC")
                    .Select(j => j.passo)
                    .FirstOrDefaultAsync();

                if(istanzaPassoFirmaEntity != null)
                    output = this._mapper.Map<IstanzaPassoDiFirma>(istanzaPassoFirmaEntity);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new GetIstanzaPassoFirmaInAttesaByDocnumberResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetIstanzaPassoFirmaInAttesaByDocnumberHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IstanzaPassoFirmaEntity, IstanzaPassoDiFirma>()
                     .ForMember(dest => dest.RuoloCoinvolto, opt => opt.MapFrom(src => new DocsPaVO.utente.Ruolo(src.ID_RUOLO_COINVOLTO)))
                     .ForMember(dest => dest.UtenteCoinvolto, opt => opt.MapFrom(src => new DocsPaVO.utente.Utente(src.ID_UTENTE_COINVOLTO)))
                     .ForMember(dest => dest.UtenteLocker, opt => opt.MapFrom(src => src.ID_UTENTE_LOCKER))
                     .ForMember(dest => dest.TipoFirma, opt => opt.MapFrom(src => src.TIPO_FIRMA))
                     .ForMember(dest => dest.ApplicaSegnaturaPermanente, opt => opt.MapFrom(src => src.CHA_POS_SEGNATURA ?? "0"))
                     .ForMember(dest => dest.PosizioneSegnaturaPermanente, opt => opt.MapFrom(src => src.VAR_POS_SEGNATURA))
                     .ForMember(dest => dest.IsAutomatico, opt => opt.MapFrom(src => src.CHA_AUTOMATICO == "1"));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }

}
