// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.areaLavoro;
using DocsPaVO.documento;
using DocsPaVO.ProfilazioneDinamica;
using DocumentFormat.OpenXml.ExtendedProperties;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentoGetAreaLavoroPaging1Request = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetAreaLavoroPaging1;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetAreaLavoroPaging1
{
    public class DocumentoGetAreaLavoroPaging1Handler : IRequestHandler<DocumentoGetAreaLavoroPaging1Request, DocumentoGetAreaLavoroPaging1Result>
    {
        #region Public Members

        public DocumentoGetAreaLavoroPaging1Handler(ILogger<DocumentoGetAreaLavoroPaging1Handler> logger,
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

        public async Task<DocumentoGetAreaLavoroPaging1Result> Handle(DocumentoGetAreaLavoroPaging1Request request, CancellationToken cancellationToken)
        {
            AreaLavoro output = new AreaLavoro();
            var numTotPage = 0;
            var nRec = 0;

            try
            {
                var idPeople = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                var idGruppo = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                var idRuoloInUO = await _dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstAsync();
                var chaDaProto = request.chaDaProto; //voglio solo i protocollati, non i predisposti..caso di ricerca doc a cui rispondere
                var filters = request.query;

                var queryable = _dbContext.AreaLavoroEntities
                    .Join(_dbContext.ProfileEntities,
                        a => a.ID_PROFILE,
                        p => p.SYSTEM_ID,
                        (a, p) => new { a, p})
                    .Where(j => j.a.ID_PEOPLE == idPeople && j.a.ID_RUOLO_IN_UO == idRuoloInUO && j.a.ID_PROFILE != null
                        && (j.p.CHA_IN_CESTINO ?? "0") == "0" && j.p.CHA_DA_PROTO == chaDaProto)
                    .Select(j => new AreaLavoroProfileEntity()
                    {
                        AreaLavoro = j.a,
                        Profile = j.p
                    });

                if (!string.IsNullOrEmpty(request.idRegistro))
                {
                    var idRegistro = request.idRegistro.AsLong();
                    queryable = queryable.Where(j => j.AreaLavoro.ID_REGISTRO == idRegistro);
                }

                if(request.tipoDoc != TipoDocumento.TUTTI)
                {
                    queryable = queryable.Where(j => j.AreaLavoro.CHA_TIPO_DOC == GetTipoDocumento(request.tipoDoc));
                }

                if(filters != null)
                {
                    foreach(var filter in filters)
                    {
                        foreach (var f in filter.Where(f => !string.IsNullOrEmpty(f.valore)))
                        {
                            switch(f.argomento)
                            {
                                case "OGGETTO":
                                    queryable = queryable.Where(j => j.Profile.VAR_PROF_OGGETTO.ToUpper().Contains(f.valore.ToUpper()));
                                    break;
                                case "TIPO_ATTO":
                                    var idTipoAtto = f.valore.AsLong();
                                    queryable = queryable.Where(j => j.Profile.ID_TIPO_ATTO == idTipoAtto);
                                    break;
                                case "PROFILAZIONE_DINAMICA":
                                    if(f.template != null && f.template.ELENCO_OGGETTI != null)
                                    {
                                        queryable = GetQueryableProfilazioneDinamica(f.template, queryable);
                                    }
                                    break;
                            }
                        }
                    }
                }

                nRec = await queryable.CountAsync();
                if(nRec > 0)
                {
                    numTotPage = (nRec / 10);
                    int startRow = ((request.numPage * 10) - 10) + 1;

                    var profileEntity = await queryable.AsNoTracking()
                        .OrderByDescending(j => (j.Profile.DTA_PROTO ?? j.Profile.CREATION_TIME))
                        .Select(j => j.Profile)
                        .Skip(startRow - 1)
                        .Take(10)
                        .ToListAsync();

                    var infoDocumento = _mapper.Map<InfoDocumento[]>(profileEntity);
                    output.lista = infoDocumento.ToArray();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                output = null;
            }

            return new DocumentoGetAreaLavoroPaging1Result(output, numTotPage, nRec);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetAreaLavoroPaging1Handler> _logger;
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
                    .ForMember(dest => dest.docNumber, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.tipoProto, src => src.MapFrom(opt => opt.CHA_TIPO_PROTO))
                    .ForMember(dest => dest.idRegistro, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.dataApertura, src => src.MapFrom(opt => opt.DTA_PROTO.HasValue ? opt.DTA_PROTO.AsDateFormat() : opt.CREATION_TIME.AsDateFormat()))
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


        protected class AreaLavoroProfileEntity
        {
            public AreaLavoroEntity AreaLavoro { get; set; }
            public ProfileEntity Profile { get; set; }
        }

        protected string GetTipoDocumento(TipoDocumento tipoDocumento)
        {
            var tipoDoc = string.Empty;

            if (tipoDocumento == TipoDocumento.ARRIVO)
                tipoDoc = "A";

            if (tipoDocumento == TipoDocumento.GRIGIO)
                tipoDoc = "G";

            if (tipoDocumento == TipoDocumento.PARTENZA)
                tipoDoc = "P";

            if (tipoDocumento == TipoDocumento.INTERNO)
                tipoDoc = "I";

            return tipoDoc;
        }

        protected IQueryable<AreaLavoroProfileEntity> GetQueryableProfilazioneDinamica(Templates template, IQueryable<AreaLavoroProfileEntity> queryable)
        {
            var idTipoAttoAsLong = template.ID_TIPO_ATTO.AsLong();

            if (template.ELENCO_OGGETTI.Count() == 0)
            {
                queryable = queryable.Where(j => _dbContext.AssociazioneTemplatesEntities.Any(t => t.ID_TEMPLATE == idTipoAttoAsLong && t.DOC_NUMBER == j.Profile.SYSTEM_ID.ToString()));
            }
            else
            {
                foreach (var oggetto in template.ELENCO_OGGETTI)
                {
                    var idOggetto = Convert.ToInt64(oggetto.SYSTEM_ID);

                    switch (oggetto.TIPO.DESCRIZIONE_TIPO)
                    {
                        case "CampoDiTesto":
                            if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                            {
                                queryable = queryable.Where(j => _dbContext.AssociazioneTemplatesEntities.Any(t => t.ID_TEMPLATE == idTipoAttoAsLong
                                    && t.DOC_NUMBER == j.Profile.SYSTEM_ID.ToString() && t.ID_OGGETTO == idOggetto
                                    && t.VALORE_OGGETTO_DB.Contains(oggetto.VALORE_DATABASE)));

                            }
                            break;
                        case "CasellaDiSelezione":
                            foreach (var valoreSelezionato in oggetto.VALORI_SELEZIONATI.Where(v => string.IsNullOrEmpty(v)))
                            {
                                queryable = queryable.Where(j => _dbContext.AssociazioneTemplatesEntities.Any(t => t.ID_TEMPLATE == idTipoAttoAsLong
                                    && t.DOC_NUMBER == j.Profile.SYSTEM_ID.ToString() && t.ID_OGGETTO == idOggetto
                                    && t.VALORE_OGGETTO_DB == valoreSelezionato));
                            }
                            break;
                        case "MenuATendina":
                            if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                            {
                                queryable = queryable.Where(j => _dbContext.AssociazioneTemplatesEntities.Any(t => t.ID_TEMPLATE == idTipoAttoAsLong
                                    && t.DOC_NUMBER == j.Profile.SYSTEM_ID.ToString() && t.ID_OGGETTO == idOggetto
                                    && t.VALORE_OGGETTO_DB == oggetto.VALORE_DATABASE));

                            }
                            break;
                        case "SelezioneEsclusiva":
                            if (!string.IsNullOrEmpty(oggetto.VALORE_DATABASE))
                            {
                                queryable = queryable.Where(j => _dbContext.AssociazioneTemplatesEntities.Any(t => t.ID_TEMPLATE == idTipoAttoAsLong
                                    && t.DOC_NUMBER == j.Profile.SYSTEM_ID.ToString() && t.ID_OGGETTO == idOggetto
                                    && t.VALORE_OGGETTO_DB == oggetto.VALORE_DATABASE));

                            }
                            break;
                        case "Contatore":
                            break;
                        case "Data":
                            break;
                        case "Corrispondente":
                            break;
                        case "ContatoreSottocontatore":
                            break;
                        case "OggettoEsterno":
                            break;
                    }
                }
            }

            return queryable;
        }

        #endregion
    }
}