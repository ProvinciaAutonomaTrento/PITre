// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetDocInCestinoFiltro
{

    // Richiede libreria MediatR
    public class DocumentoGetDocInCestinoFiltroHandler : IRequestHandler<Requests.DocumentoGetDocInCestinoFiltro, DocumentoGetDocInCestinoFiltroResult>
    {
        #region Public Members

        public DocumentoGetDocInCestinoFiltroHandler(ILogger<DocumentoGetDocInCestinoFiltroHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IDistributedCache distributedCache)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._distributedCache = distributedCache;
            InitializeMapper();
        }

        public async Task<DocumentoGetDocInCestinoFiltroResult> Handle(Requests.DocumentoGetDocInCestinoFiltro request, CancellationToken cancellationToken)
        {
            List<InfoDocumento> listaDoc = new List<InfoDocumento>();
            DocsPaVO.filtri.FiltroRicerca[][] filtriRicerca = request.filtriRicerca;
            try
            {
                var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
                var idRegistroList = await this._dbContext.RegistroEntities.AsNoTracking()
                .Where(r => r.ID_AMM == idTenantAsLong && r.CHA_RF == "0")
                .Select(r => r.SYSTEM_ID)
                .ToListAsync();

                if (idRegistroList == null || idRegistroList.Count == 0)
                    throw new RegistroNotFoundPi3Exception();

                /*ENABLE_CANC_DOC_TRASMESSI?*/

                var queryString = this._dbContext.ProfileEntities
                    .Where(a => (a.CHA_DA_PROTO.Equals("1") &&
                            idRegistroList.Contains((long)a.ID_REGISTRO) || a.CHA_TIPO_PROTO == "G") &&
                            ((a.CHA_IN_CESTINO ?? "0") == "1") && a.ID_DOCUMENTO_PRINCIPALE == null &&
                            this._dbContext.PeopleEntities.Where(x => x.SYSTEM_ID == a.AUTHOR).Select(x => x.ID_AMM).FirstOrDefault() == idTenantAsLong)
                    .Select(x => new JoinEntity { a = x });


                for (int i = 0; i < filtriRicerca.Length; i++)
                {
                    //if (filtriRicerca[i].Any(x => x.argomento.Equals("TIPO_FILE_ACQUISITO")))
                    //    queryString = queryString.Join(this._dbContext.ComponentEntities, p => p.a.DOCNUMBER, c => c.DOCNUMBER, (p, c) => new JoinEntity { a = p.a, c = c });

                    for (int j = 0; j < filtriRicerca[i].Length; j++)
                    {
                        DocsPaVO.filtri.FiltroRicerca f = filtriRicerca[i][j];
                        TimeSpan ts = new TimeSpan(0, 0, 0);
                        if (!string.IsNullOrEmpty(f.valore))
                        {
                            switch (f.argomento)
                            {
                                case "TIPO": //Non presente da FE
                                    queryString = f.valore.Equals("T") ? queryString.Where(x => new String[] { "A", "P", "I", "G" }.Contains(x.a.CHA_TIPO_PROTO))
                                        : queryString.Where(x => x.a.CHA_TIPO_PROTO.Equals(f.valore));
                                    break;
                                case "TIPO_ATTO": //Non presente da FE
                                    long idTipoAttoAsLong = f.valore.AsLong();
                                    queryString = queryString.Where(x => x.a.ID_TIPO_ATTO == idTipoAttoAsLong);
                                    break;
                                case "PROFILAZIONE_DINAMICA": //Non presente da FE
                                    break;
                                case "NUM_PROTOCOLLO": //Non presente da FE
                                    break;
                                case "NUM_PROTOCOLLO_AL": //Non presente da FE
                                    break;
                                case "NUM_PROTOCOLLO_DAL": //Non presente da FE
                                    break;
                                case "DATA_PROT_IL": //Non presente da FE
                                    break;
                                case "DATA_PROT_SUCCESSIVA_AL": //Non presente da FE
                                    break;
                                case "DATA_PROT_PRECEDENTE_IL": //Non presente da FE
                                    break;
                                case "ANNO_PROTOCOLLO": //Non presente da FE
                                    break;
                                case "DOCNUMBER":
                                    long idDocAsLong = f.valore.AsLong();
                                    queryString = queryString.Where(x => x.a.DOCNUMBER == idDocAsLong);
                                    break;
                                case "DOCNUMBER_DAL":
                                    long idDocFromAsLong = f.valore.AsLong();
                                    queryString = queryString.Where(x => x.a.DOCNUMBER >= idDocFromAsLong);
                                    break;
                                case "DOCNUMBER_AL":
                                    long idDocToAsLong = f.valore.AsLong();
                                    queryString = queryString.Where(x => x.a.DOCNUMBER <= idDocToAsLong);
                                    break;
                                case "DATA_CREAZIONE_IL":
                                    var creationDate = f.valore.AsDateTime();
                                    creationDate = creationDate.Date + ts;
                                    queryString = queryString.Where(x => x.a.CREATION_DATE >= creationDate && x.a.CREATION_DATE <= creationDate.AddDays(1));
                                    break;
                                case "DATA_CREAZIONE_SUCCESSIVA_AL":
                                    var dateFrom = f.valore.AsDateTime();
                                    dateFrom = dateFrom.Date + ts;
                                    queryString = queryString.Where(x => x.a.CREATION_DATE >= dateFrom);
                                    break;
                                case "DATA_CREAZIONE_PRECEDENTE_IL":
                                    var dateTo = f.valore.AsDateTime();
                                    dateTo = dateTo.Date + ts;
                                    queryString = queryString.Where(x => x.a.CREATION_DATE <= dateTo);
                                    break;
                                case "OGGETTO":
                                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("&&", RegexOptions.None, TimeSpan.FromSeconds(5));
                                    string[] list = regex.Split(f.valore);
                                    queryString = queryString.Where(x => x.a.VAR_PROF_OGGETTO.ToUpper().Contains(list[0].ToUpper().Replace("'", "''")));
                                    for (int k = 1; k < list.Length; k++)
                                    {
                                        string val = list[k];
                                        queryString = queryString.Where(x => x.a.VAR_PROF_OGGETTO.ToUpper().Contains(val.ToUpper().Replace("'", "''")));
                                    }
                                    break;
                                case "ID_MITT_DEST": //Non presente da FE
                                    break;
                                case "COD_MITT_DEST": //Non presente da FE
                                    break;
                                case "MITT_DEST": //Non presente da FE
                                    break;
                                case "ID_DESTINATARIO": //Non presente da FE
                                    break;
                                case "ID_DESCR_DESTINATARIO": //Non presente da FE
                                    break;
                                case "FIRMATO":
                                    queryString = (f.valore.Equals("0") || f.valore.Equals("1")) ?
                                        queryString.Where(x => IPi3DbContextMappedFunctions.GetChaFirmato((long)x.a.DOCNUMBER) == f.valore) :
                                        queryString.Where(x => IPi3DbContextMappedFunctions.GetChaImg((long)x.a.DOCNUMBER) != "0");
                                    break;
                                case "TIPO_FILE_ACQUISITO":
                                    queryString = queryString
                                        .Join(this._dbContext.ComponentEntities, p => p.a.DOCNUMBER, c => c.DOCNUMBER, (p, c) => new JoinEntity { a = p.a, c = c })
                                        .Where(x => x.c.EXT.ToUpper().Equals(f.valore.ToUpper()) &&
                                            x.c.VERSION_ID == this._dbContext.VersionEntities.Join(this._dbContext.ComponentEntities, v => v.VERSION_ID, c => c.VERSION_ID, (v, c) => new { VERSION_ID = v.VERSION_ID, DOC_NUMBER = v.DOCNUMBER }).Where(c => c.DOC_NUMBER == x.a.DOCNUMBER).Max(v => v.VERSION_ID));
                                    break;
                            }
                        }
                    }
                }

                var resultList = await queryString.Select(x => new ProfileEntity
                {
                    SYSTEM_ID = x.a.SYSTEM_ID,
                    DOCNUMBER = x.a.DOCNUMBER,
                    DTA_ANNULLA = x.a.DTA_ANNULLA,
                    VAR_PROF_OGGETTO = x.a.VAR_PROF_OGGETTO,
                    ID_REGISTRO = x.a.ID_REGISTRO,
                    CHA_TIPO_PROTO = x.a.CHA_TIPO_PROTO,
                    CHA_EVIDENZA = x.a.CHA_EVIDENZA,
                    NUM_ANNO_PROTO = x.a.NUM_ANNO_PROTO,
                    CREATION_DATE = x.a.CREATION_TIME,
                    NUM_PROTO = x.a.NUM_PROTO,
                    VAR_SEGNATURA = x.a.VAR_SEGNATURA,
                    DTA_PROTO = x.a.DTA_PROTO,
                    CHA_IMG = IPi3DbContextMappedFunctions.GetChaImg((long)x.a.DOCNUMBER),
                    CHA_PRIVATO = x.a.CHA_PRIVATO,
                    CHA_PERSONALE = x.a.CHA_PERSONALE,
                    CHA_IN_CESTINO = x.a.CHA_IN_CESTINO,
                    VAR_NOTE_CESTINO = x.a.VAR_NOTE_CESTINO,
                    AUTHOR = x.a.AUTHOR,
                    ID_DOCUMENTO_PRINCIPALE = x.a.ID_DOCUMENTO_PRINCIPALE
                })
                .OrderByDescending(x => x.SYSTEM_ID)
                .ToListAsync();

                listaDoc = _mapper.Map<List<InfoDocumento>>(resultList);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new DocumentoGetDocInCestinoFiltroResult(listaDoc.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetDocInCestinoFiltroHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
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

        protected class JoinEntity
        {
            public ProfileEntity a { get; set; }
            public ComponentEntity c { get; set; }
        }
        #endregion
    }

}
