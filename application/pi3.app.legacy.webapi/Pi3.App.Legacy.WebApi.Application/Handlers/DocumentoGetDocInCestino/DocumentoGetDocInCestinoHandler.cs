// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.ProspettiRiepilogativi;
using DocumentFormat.OpenXml.Bibliography;
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
using DocumentoGetDocInCestinoRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetDocInCestino;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetDocInCestino
{

    public class DocumentoGetDocInCestinoHandler : IRequestHandler<DocumentoGetDocInCestinoRequest, DocumentoGetDocInCestinoResult>
    {
        #region Public Members

        public DocumentoGetDocInCestinoHandler(ILogger<DocumentoGetDocInCestinoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<DocumentoGetDocInCestinoResult> Handle(DocumentoGetDocInCestinoRequest request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var idRegistro = await this._dbContext.RegistroEntities.AsNoTracking()
                .Where(r => r.ID_AMM == idTenant && r.CHA_RF == "0")
                .Select(r => r.SYSTEM_ID)
                .ToListAsync();

            if (idRegistro == null || idRegistro.Count == 0)
                throw new RegistroNotFoundPi3Exception();

            var currentDate = DateTime.Now.AddDays(-60);
            var initDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0);
            var endDate = DateTime.Now.AddHours(23).AddMinutes(59).AddSeconds(59);

            var profileEnity = await this._dbContext.ProfileEntities.AsNoTracking()
                .Join(this._dbContext.PeopleEntities.AsNoTracking(), profile => profile.AUTHOR, people => people.SYSTEM_ID, (profile, people) => new
                {
                    profile.SYSTEM_ID, 
                    profile.DOCNUMBER,
                    profile.DTA_ANNULLA,
                    profile.VAR_PROF_OGGETTO,
                    profile.ID_REGISTRO,
                    profile.CHA_TIPO_PROTO,
                    profile.CHA_EVIDENZA,
                    profile.NUM_ANNO_PROTO,
                    profile.CREATION_TIME,
                    profile.NUM_PROTO,
                    profile.VAR_SEGNATURA,
                    profile.DTA_PROTO,
                    profile.EXT,
                    profile.CHA_PRIVATO,
                    profile.CHA_PERSONALE,
                    profile.CHA_IN_CESTINO,
                    profile.VAR_NOTE_CESTINO,
                    profile.AUTHOR,
                    profile.ID_DOCUMENTO_PRINCIPALE,
                    profile.CHA_DA_PROTO,
                    people.ID_AMM
                })
                .Where(p => p.ID_AMM == idTenant && ((p.CHA_DA_PROTO == "1" && idRegistro.Contains((long)p.ID_REGISTRO)) || p.CHA_TIPO_PROTO == "G")
                    && ((p.CHA_IN_CESTINO ?? "0") == "1") && p.ID_DOCUMENTO_PRINCIPALE == null
                    && p.CREATION_TIME >= initDate && p.CREATION_TIME <= endDate)
                .OrderByDescending(p => p.SYSTEM_ID)
                .Select(p => new ProfileEntity
                {
                    SYSTEM_ID = p.SYSTEM_ID,
                    DOCNUMBER = p.DOCNUMBER,
                    DTA_ANNULLA = p.DTA_ANNULLA,
                    VAR_PROF_OGGETTO = p.VAR_PROF_OGGETTO,
                    ID_REGISTRO = p.ID_REGISTRO,
                    CHA_TIPO_PROTO = p.CHA_TIPO_PROTO,
                    CHA_EVIDENZA = p.CHA_EVIDENZA,
                    NUM_ANNO_PROTO = p.NUM_ANNO_PROTO,
                    CREATION_TIME = p.CREATION_TIME,
                    NUM_PROTO = p.NUM_PROTO,
                    VAR_SEGNATURA = p.VAR_SEGNATURA,
                    DTA_PROTO = p.DTA_PROTO,
                    EXT = p.EXT,
                    CHA_PRIVATO = p.CHA_PRIVATO,
                    CHA_PERSONALE = p.CHA_PERSONALE,
                    CHA_IN_CESTINO = p.CHA_IN_CESTINO,
                    VAR_NOTE_CESTINO = p.VAR_NOTE_CESTINO,
                    AUTHOR = p.AUTHOR,
                    ID_DOCUMENTO_PRINCIPALE = p.ID_DOCUMENTO_PRINCIPALE,
                    CHA_DA_PROTO = p.CHA_DA_PROTO
                })
                .ToListAsync();

            var output = _mapper.Map<InfoDocumento[]>(profileEnity);

            return new DocumentoGetDocInCestinoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetDocInCestinoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProfileEntity, InfoDocumento>()
                    .ForMember(dest => dest.idProfile, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.docNumber, src => src.MapFrom(opt => opt.DOCNUMBER))
                    .ForMember(dest => dest.tipoProto, src => src.MapFrom(opt => opt.CHA_TIPO_PROTO))
                    .ForMember(dest => dest.idRegistro, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.dataApertura, src => src.MapFrom(opt => opt.DTA_PROTO != null ? opt.DTA_PROTO.AsDateFormat() : opt.CREATION_TIME.AsDateFormat()))
                    .ForMember(dest => dest.oggetto, src => src.MapFrom(opt => opt.VAR_PROF_OGGETTO))
                    .ForMember(dest => dest.autore, src => src.MapFrom(opt => opt.AUTHOR))
                    .ForMember(dest => dest.noteCestino, src => src.MapFrom(opt => opt.VAR_NOTE_CESTINO))
                    .ForMember(dest => dest.evidenza, src => src.MapFrom(opt => opt.CHA_EVIDENZA))
                    .ForMember(dest => dest.privato, src => src.MapFrom(opt => opt.CHA_PRIVATO))
                    .ForMember(dest => dest.personale, src => src.MapFrom(opt => opt.CHA_PERSONALE))
                    .ForMember(dest => dest.numProt, src => src.MapFrom(opt => opt.NUM_PROTO))
                    .ForMember(dest => dest.segnatura, src => src.MapFrom(opt => opt.VAR_SEGNATURA))
                    .ForMember(dest => dest.dataAnnullamento, src => src.MapFrom(opt => opt.DTA_ANNULLA))
                    .ForMember(dest => dest.acquisitaImmagine, src => src.MapFrom(opt => opt.EXT ?? "0"))
                    .ForMember(dest => dest.allegato, src => src.MapFrom(opt => opt.ID_DOCUMENTO_PRINCIPALE > 0));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
