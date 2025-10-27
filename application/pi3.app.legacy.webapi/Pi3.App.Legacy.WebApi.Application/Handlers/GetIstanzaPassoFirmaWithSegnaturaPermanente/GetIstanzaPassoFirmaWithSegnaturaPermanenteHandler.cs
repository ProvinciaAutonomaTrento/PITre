// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetIstanzaPassoFirmaWithSegnaturaPermanenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetIstanzaPassoFirmaWithSegnaturaPermanente;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetIstanzaPassoFirmaWithSegnaturaPermanente
{
    public class GetIstanzaPassoFirmaWithSegnaturaPermanenteHandler : IRequestHandler<GetIstanzaPassoFirmaWithSegnaturaPermanenteRequest, GetIstanzaPassoFirmaWithSegnaturaPermanenteResult>
    {
        #region Public Members

        public GetIstanzaPassoFirmaWithSegnaturaPermanenteHandler(ILogger<GetIstanzaPassoFirmaWithSegnaturaPermanenteHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<GetIstanzaPassoFirmaWithSegnaturaPermanenteResult> Handle(GetIstanzaPassoFirmaWithSegnaturaPermanenteRequest request, CancellationToken cancellationToken)
        {
            IstanzaPassoDiFirma output = null;

            try
            {
                long docnumber = Convert.ToInt64(request.docnumber);
                string[] cha_posizione_segnatura = new string[] { "0", "1" };
                string[] tipo_firma = new string[] { "RECORD_PREDISPOSED", "DOCUMENTO_REPERTORIATO" };
                var entity = await _dbContext.IstanzaProcessoFirmaEntities
                    .Join(_dbContext.IstanzaPassoFirmaEntities,
                    processo => processo.ID_ISTANZA,
                    passo => passo.ID_ISTANZA_PROCESSO,
                    (processo, passo) => new
                    {
                        processo,
                        passo
                    })
                    .Where(x => x.processo.ID_DOCUMENTO == docnumber
                    && x.processo.STATO == "IN_EXEC" && tipo_firma.Contains(x.passo.TIPO_FIRMA) && cha_posizione_segnatura.Contains(x.passo.CHA_POS_SEGNATURA)
                    )
                    .Select(x => new IstanzaPassoFirmaEntity
                    {
                        ID_RUOLO_COINVOLTO = x.passo.ID_RUOLO_COINVOLTO,
                        ID_UTENTE_COINVOLTO = x.passo.ID_UTENTE_COINVOLTO,
                        ID_UTENTE_LOCKER = x.passo.ID_UTENTE_LOCKER,
                        TIPO_FIRMA = x.passo.TIPO_FIRMA,
                        CHA_POS_SEGNATURA = x.passo.CHA_POS_SEGNATURA,
                        VAR_POS_SEGNATURA = x.passo.VAR_POS_SEGNATURA,
                        CHA_AUTOMATICO = x.passo.CHA_AUTOMATICO
                    }).FirstOrDefaultAsync();

                output = _mapper.Map<IstanzaPassoDiFirma>(entity);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new GetIstanzaPassoFirmaWithSegnaturaPermanenteResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetIstanzaPassoFirmaWithSegnaturaPermanenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IstanzaPassoFirmaEntity, IstanzaPassoDiFirma>()
                    .ForMember(dest => dest.RuoloCoinvolto, src => src.MapFrom(opt => new Ruolo { idGruppo = opt.ID_RUOLO_COINVOLTO.ToString() }))
                    .ForMember(dest => dest.UtenteCoinvolto, src => src.MapFrom(opt => new Utente { idPeople = opt.ID_UTENTE_COINVOLTO.ToString() }))
                    .ForMember(dest => dest.UtenteLocker, src => src.MapFrom(opt => opt.ID_UTENTE_LOCKER))
                    .ForMember(dest => dest.TipoFirma, src => src.MapFrom(opt => opt.TIPO_FIRMA))
                    .ForMember(dest => dest.ApplicaSegnaturaPermanente, src => src.MapFrom(opt => opt.CHA_POS_SEGNATURA))
                    .ForMember(dest => dest.PosizioneSegnaturaPermanente, src => src.MapFrom(opt => opt.VAR_POS_SEGNATURA))
                    .ForMember(dest => dest.IsAutomatico, src => src.MapFrom(opt => opt.CHA_AUTOMATICO == "1"));
            });
            _mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
