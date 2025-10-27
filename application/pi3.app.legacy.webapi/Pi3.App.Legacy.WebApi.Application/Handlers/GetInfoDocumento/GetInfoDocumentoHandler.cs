// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.documento;
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
using GetInfoDocumentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetInfoDocumento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetInfoDocumento
{
    public class GetInfoDocumentoHandler : IRequestHandler<GetInfoDocumentoRequest, GetInfoDocumentoResult>
    {
        #region Public Members

        public GetInfoDocumentoHandler(ILogger<GetInfoDocumentoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<GetInfoDocumentoResult> Handle(GetInfoDocumentoRequest request, CancellationToken cancellationToken)
        {
            InfoDocumento output = null;
            var idProfileAsLong = request.idProfile.AsLong();

            var profileEntity = await this._dbContext.ProfileEntities.AsNoTracking()
                .Where(p => p.DOCNUMBER == idProfileAsLong)
                .Select(p => new ProfileEntity
                {
                    SYSTEM_ID = p.SYSTEM_ID,
                    DOCNUMBER = p.DOCNUMBER,
                    VAR_PROF_OGGETTO = p.VAR_PROF_OGGETTO,
                    DTA_ANNULLA =p.DTA_ANNULLA,
                    ID_REGISTRO = p.ID_REGISTRO,
                    CHA_TIPO_PROTO = p.CHA_TIPO_PROTO,
                    CHA_EVIDENZA = p.CHA_EVIDENZA,
                    CREATION_DATE = p.CREATION_DATE,
                    NUM_PROTO = p.NUM_PROTO,
                    VAR_SEGNATURA = p.VAR_SEGNATURA,
                    ID_TIPO_ATTO = p.ID_TIPO_ATTO,
                    DTA_PROTO = p.DTA_PROTO,
                    EXT = p.EXT,
                    CHA_PRIVATO = p.CHA_PRIVATO,
                    CHA_PERSONALE = p.CHA_PERSONALE,
                    ID_DOCUMENTO_PRINCIPALE = p.ID_DOCUMENTO_PRINCIPALE,
                    AUTHOR = p.AUTHOR,
                    IN_LIBROFIRMA = p.IN_LIBROFIRMA
                })
                .FirstOrDefaultAsync();

            if(profileEntity != null)
            {
                output = this._mapper.Map<InfoDocumento>(profileEntity);
                output.autore = await this._dbContext.PeopleEntities.AsNoTracking().Where(a => a.SYSTEM_ID == profileEntity.AUTHOR).Select(a => a.USER_ID).FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(output.idTipoAtto))
                    output.tipoAtto = await this._dbContext.TipoAttoEntities.AsNoTracking().Where(a => a.SYSTEM_ID == profileEntity.ID_TIPO_ATTO).Select(a => a.VAR_DESC_ATTO).FirstOrDefaultAsync();

                output.ultimaNota = await this._dbContext.NoteEntities.AsNoTracking().Where(a => a.IDOGGETTOASSOCIATO == idProfileAsLong && a.TIPOVISIBILITA == "T" && a.TIPOOGGETTOASSOCIATO == "D")
                    .OrderByDescending(a => a.DATACREAZIONE)
                    .Select(a => a.TESTO).FirstOrDefaultAsync();          
            }

            return new GetInfoDocumentoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetInfoDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProfileEntity, InfoDocumento>()
                     .ForMember(dest => dest.idProfile, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.docNumber, opt => opt.MapFrom(src => src.DOCNUMBER))
                     .ForMember(dest => dest.tipoProto, opt => opt.MapFrom(src => src.CHA_TIPO_PROTO))
                     .ForMember(dest => dest.idRegistro, opt => opt.MapFrom(src => src.ID_REGISTRO ?? null))
                     .ForMember(dest => dest.oggetto, opt => opt.MapFrom(src => src.VAR_PROF_OGGETTO.Replace("<", "&lt;").Replace(">", "&gt;")))
                     .ForMember(dest => dest.evidenza, opt => opt.MapFrom(src => src.CHA_EVIDENZA))
                     .ForMember(dest => dest.privato, opt => opt.MapFrom(src => src.CHA_PRIVATO))
                     .ForMember(dest => dest.personale, opt => opt.MapFrom(src => src.CHA_PERSONALE))
                     .ForMember(dest => dest.numProt, opt => opt.MapFrom(src => src.NUM_PROTO))
                     .ForMember(dest => dest.dataApertura, opt => opt.MapFrom(src => src.DTA_PROTO.AsDateFormat() ?? src.CREATION_DATE.AsDateFormat()))
                     .ForMember(dest => dest.segnatura, opt => opt.MapFrom(src => src.VAR_SEGNATURA))
                     .ForMember(dest => dest.dataAnnullamento, opt => opt.MapFrom(src => src.DTA_ANNULLA.AsDateFormat() ?? null))
                     .ForMember(dest => dest.acquisitaImmagine, opt => opt.MapFrom(src => src.EXT))
                     .ForMember(dest => dest.idTipoAtto, opt => opt.MapFrom(src => src.ID_TIPO_ATTO))
                     .ForMember(dest => dest.allegato, opt => opt.MapFrom(src => src.ID_DOCUMENTO_PRINCIPALE > 0));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
