// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.LibroFirma;
using DocsPaVO.ProfilazioneDinamica;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetElementiLibroFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetElementiLibroFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetElementiLibroFirma
{
    public class GetElementiLibroFirmaHandler : IRequestHandler<GetElementiLibroFirmaRequest, GetElementiLibroFirmaResult>
    {
        #region Public Members

        public GetElementiLibroFirmaHandler(ILogger<GetElementiLibroFirmaHandler> logger,
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

        public async Task<GetElementiLibroFirmaResult> Handle(GetElementiLibroFirmaRequest request, CancellationToken cancellationToken)
        {
            ElementoInLibroFirma[] output = null;
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            try
            {
                var elementiEntities = await (from elementi in this._dbContext.ElementoInLibroFirmaEntities
                                        join profile in this._dbContext.ProfileEntities on elementi.DOC_NUMBER equals profile.SYSTEM_ID
                                        join tipoAtto in this._dbContext.TipoAttoEntities on profile.ID_TIPO_ATTO equals tipoAtto.SYSTEM_ID into ta
                                        from tipoAtto in ta.DefaultIfEmpty()
                                        join components in this._dbContext.ComponentEntities on elementi.VERSION_ID equals components.VERSION_ID into c
                                        from components in c.DefaultIfEmpty()
                                        join istanzaProcesso in this._dbContext.IstanzaProcessoFirmaEntities on elementi.ISTANZA_PROCESSO equals istanzaProcesso.ID_ISTANZA into i
                                        from istanzaProcesso in i.DefaultIfEmpty()
                                        where elementi.ID_RUOLO_TITOLARE == idGruppo
                                        && (elementi.ID_UTENTE_TITOLARE == null || elementi.ID_UTENTE_TITOLARE == idPeople)
                                        && (elementi.ID_UTENTE_LOCKER == null || elementi.ID_UTENTE_LOCKER == idPeople)
                                        && elementi.DTA_ESECUZIONE == null &&  elementi.ID_TRASM_SINGOLA != null
                                        select new
                                        {
                                            ElementiInLibroFirma = elementi,
                                            Docnumber = profile.SYSTEM_ID,
                                            Oggetto =  profile.VAR_PROF_OGGETTO,
                                            NumeroProtocollo = profile.NUM_PROTO,
                                            TipoProtocollo = profile.CHA_TIPO_PROTO,
                                            IdDocumentoPrincipale = profile.ID_DOCUMENTO_PRINCIPALE,
                                            DataCreazione = profile.CREATION_TIME,
                                            DataProtocollo = profile.DTA_PROTO,
                                            Firmato = profile.CHA_FIRMATO,
                                            IdRegistro = profile.ID_REGISTRO,
                                            TipoAtto = tipoAtto.VAR_DESC_ATTO,
                                            MotivoRespingimento = istanzaProcesso.MOTIVO_RESPINGIMENTO,
                                            FileSize = components.FILE_SIZE,
                                            TipoFirna = components.CHA_TIPO_FIRMA
                                        })
                                        .OrderByDescending(e => e.ElementiInLibroFirma.DATA_INSERIMENTO)
                                        .ThenByDescending(e => e.ElementiInLibroFirma.ID_ELEMENTO)
                                        .AsNoTracking()
                                        .ToListAsync();

                if(elementiEntities != null && elementiEntities.Count > 0)
                {
                    List<ElementoInLibroFirma> elementi = new List<ElementoInLibroFirma>();

                    foreach (var e in elementiEntities)
                    {
                        var elemento = _mapper.Map<ElementoInLibroFirma>(e.ElementiInLibroFirma);

                        var idRuoloProponenteAsLong = e.ElementiInLibroFirma.RUOLO_PROPONENTE.AsLong();
                        var idPeopleProponenteAsLong = e.ElementiInLibroFirma.UTENTE_PROPONENTE.AsLong();
                        var gruppoProponenteEntity = await this._dbContext.GroupEntities
                        .Where(g => g.SYSTEM_ID == idRuoloProponenteAsLong)
                        .Select(g => new
                        {
                            g.GROUP_ID,
                            g.GROUP_NAME
                        })
                        .AsNoTracking()
                        .FirstAsync();

                        var peopleProponenteEntity = await this._dbContext.PeopleEntities
                        .Where(p => p.SYSTEM_ID == idPeopleProponenteAsLong)
                        .Select(p => new
                        {
                            p.USER_ID,
                            p.FULL_NAME
                        })
                        .AsNoTracking()
                        .FirstAsync();

                        var idCorrGlobaliGruppo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(p => p.ID_GRUPPO == idRuoloProponenteAsLong).Select(p => p.SYSTEM_ID).FirstOrDefaultAsync();
                        var idCorrGlobaliPeople = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(p => p.ID_PEOPLE == idPeopleProponenteAsLong).Select(p => p.SYSTEM_ID).FirstOrDefaultAsync();
                        var oggettoDocumentoPrincipale = e.IdDocumentoPrincipale != null ?
                                    await this._dbContext.ProfileEntities.AsNoTracking()
                                    .Where(p => p.SYSTEM_ID == e.IdDocumentoPrincipale)
                                    .Select(p => p.VAR_PROF_OGGETTO).FirstOrDefaultAsync() : string.Empty; 

                        elemento.RuoloProponente = new DocsPaVO.utente.Ruolo
                        {
                            idGruppo = e.ElementiInLibroFirma.RUOLO_PROPONENTE,
                            systemId = idCorrGlobaliGruppo != null ? idCorrGlobaliGruppo.ToString() : string.Empty,
                            codiceRubrica = gruppoProponenteEntity.GROUP_ID,
                            descrizione = gruppoProponenteEntity.GROUP_NAME
                        };

                        elemento.UtenteProponente = new DocsPaVO.utente.Utente
                        {
                            idPeople = e.ElementiInLibroFirma.UTENTE_PROPONENTE,
                            descrizione = peopleProponenteEntity.FULL_NAME,
                            userId = peopleProponenteEntity.USER_ID,
                            systemId = idCorrGlobaliPeople != null ? idCorrGlobaliPeople.ToString() : string.Empty
                        };

                        elemento.MotivoRespingimento = e.MotivoRespingimento ?? string.Empty;
                        elemento.FileSize = (long)e.FileSize;
                        elemento.FileOriginaleFirmato = e.Firmato != null ? e.Firmato : "0";

                        elemento.InfoDocumento = new InfoDocLibroFirma
                        {
                            Docnumber = e.Docnumber.ToString(),
                            Oggetto = e.Oggetto,
                            DataCreazione = e.DataCreazione.AsDateTimeFormat(),
                            DataProtocollo = e.DataProtocollo.AsDateTimeFormat(),
                            NumProto = e.NumeroProtocollo.ToString(),
                            TipoProto = e.TipoProtocollo,
                            TipologiaDocumento = e.TipoAtto,
                            IdDocumentoPrincipale = e.IdDocumentoPrincipale.ToString(),
                            OggettoDocumentoPrincipale = oggettoDocumentoPrincipale,
                            VersionId = e.ElementiInLibroFirma.VERSION_ID.ToString(),
                            Destinatario = await GetDestinatari(e.Docnumber, e.TipoProtocollo),
                            NumAllegato = Convert.ToInt32(e.ElementiInLibroFirma.NUM_ALL),
                            NumVersione = Convert.ToInt32(e.ElementiInLibroFirma.NUM_VERSIONE),
                            IdRegistro = e.IdRegistro.ToString(),
                            Fascicoli = await GetClassCatTipo(e.Docnumber, idPeople, idGruppo)
                        };

                        elemento.TipoFirmaFile = await GetTipoFirmaDocumento(e.Docnumber);

                        elementi.Add(elemento);
                    }

                    output = elementi.ToArray();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new GetElementiLibroFirmaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetElementiLibroFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ElementoInLibroFirmaEntity, ElementoInLibroFirma>()
                    .ForMember(dest => dest.IdElemento, src => src.MapFrom(opt => opt.ID_ELEMENTO))
                    .ForMember(dest => dest.StatoFirma, src => src.MapFrom(opt => (TipoStatoElemento)Enum.Parse(typeof(TipoStatoElemento), opt.STATO_FIRMA)))
                    .ForMember(dest => dest.TipoFirma, src => src.MapFrom(opt => opt.TIPO_FIRMA))
                    .ForMember(dest => dest.Modalita, src => src.MapFrom(opt => opt.MODALITA))
                    .ForMember(dest => dest.DataInserimento, src => src.MapFrom(opt => opt.DATA_INSERIMENTO.AsDateTimeFormat()))
                    .ForMember(dest => dest.DataScadenza, src => src.MapFrom(opt => opt.SCADENZA.AsDateTimeFormat()))
                    .ForMember(dest => dest.DataAccettazione, src => src.MapFrom(opt => opt.DTA_ACCETTAZIONE.AsDateTimeFormat()))
                    .ForMember(dest => dest.IdRuoloTitolare, src => src.MapFrom(opt => opt.ID_RUOLO_TITOLARE))
                    .ForMember(dest => dest.DescProponenteDelegato, src => src.MapFrom(opt => opt.ID_PEOPLE_PROPONENTE_DELEGATO == 0 ? null : opt.ID_PEOPLE_PROPONENTE_DELEGATO))
                    .ForMember(dest => dest.IdUtenteTitolare, src => src.MapFrom(opt => opt.ID_UTENTE_TITOLARE))
                    .ForMember(dest => dest.IdUtenteLocker, src => src.MapFrom(opt => opt.ID_UTENTE_LOCKER))
                    .ForMember(dest => dest.Note, src => src.MapFrom(opt => opt.NOTE ?? string.Empty))
                    .ForMember(dest => dest.IdIstanzaProcesso, src => src.MapFrom(opt => opt.ISTANZA_PROCESSO))
                    .ForMember(dest => dest.IdIstanzaPasso, src => src.MapFrom(opt => opt.ID_ISTANZA_PASSO))
                    .ForMember(dest => dest.IdTrasmSingola, src => src.MapFrom(opt => opt.ID_TRASM_SINGOLA))
                    .ForMember(dest => dest.ErroreFirma, src => src.MapFrom(opt => opt.ERRORE_FIRMA != null ? opt.ERRORE_FIRMA : string.Empty));
            });

            _mapper = configuration.CreateMapper();
        }

        protected async Task<string> GetTipoFirmaDocumento(long docnumber)
        {
            var tipoFirma = string.Empty;

            var lastVersionId = await _dbContext.VersionEntities.AsNoTracking()
                            .Where(v => v.DOCNUMBER == docnumber)
                            .OrderByDescending(v => v.VERSION_ID)
                            .Select(v => v.VERSION_ID)
                            .FirstAsync();

            var componentsEntity = await _dbContext.ComponentEntities
                .Where(c => c.VERSION_ID == lastVersionId)
                .Select(c => new
                {
                    c.CHA_TIPO_FIRMA,
                    c.VAR_NOMEORIGINALE,
                    c.CHA_FIRMATO
                })
                .FirstAsync();

            tipoFirma = !string.IsNullOrEmpty(componentsEntity.CHA_TIPO_FIRMA) ? componentsEntity.CHA_TIPO_FIRMA : DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA;

            var firmato = componentsEntity.CHA_FIRMATO ?? string.Empty;

            //Controllo anche l'estenzione del file
            if (firmato.Equals("1") && (tipoFirma.Equals(DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA) || tipoFirma.Equals(DocsPaVO.documento.TipoFirma.ELETTORNICA)))
            {
                var fileName = componentsEntity.VAR_NOMEORIGINALE ?? string.Empty;
                if (!string.IsNullOrEmpty(fileName) && fileName.ToUpper().EndsWith("P7M"))
                    tipoFirma = tipoFirma.Equals(DocsPaVO.documento.TipoFirma.ELETTORNICA) ? DocsPaVO.documento.TipoFirma.CADES_ELETTORNICA : DocsPaVO.documento.TipoFirma.CADES;
            }

            return tipoFirma;
        }

        private async Task<string> GetDestinatari(long docnumber, string tipoProto)
        {
            var result = string.Empty;

            if (tipoProto == "P" || tipoProto == "I")
            {
                var entities = await this._dbContext.DocArrivoParEntities
                    .Join(this._dbContext.CorrGlobaliEntities, doc => doc.ID_MITT_DEST, corr => corr.SYSTEM_ID, (doc, corr) => new { doc, corr })
                    .Where(j => j.doc.ID_PROFILE == docnumber)
                    .Select(j => new { j.corr.VAR_DESC_CORR, j.doc.CHA_TIPO_MITT_DEST, j.doc.ID_MITT_DEST })
                    .OrderByDescending(j => j.CHA_TIPO_MITT_DEST)
                    .AsNoTracking()
                    .ToListAsync();

                foreach (var entity in entities)
                {
                    if(result != null && result.Length >= (3900 - 128))
                    {
                        result = result + "...";
                        break;
                    }

                    if (entity.CHA_TIPO_MITT_DEST == "D")
                        result = !string.IsNullOrEmpty(result) ? result + "; " + entity.VAR_DESC_CORR + " (D)" : entity.VAR_DESC_CORR;

                    if (entity.CHA_TIPO_MITT_DEST == "C")
                        result = !string.IsNullOrEmpty(result) ? result + "; " + entity.VAR_DESC_CORR + " (CC)" : entity.VAR_DESC_CORR;

                    if(tipoProto == "P" && entity.CHA_TIPO_MITT_DEST == "L")
                        result = !string.IsNullOrEmpty(result) ? result + "; " + 
                            (await this._dbContext.ListeDistrEntities.Where(l => l.ID_LISTA_DPA_CORR == entity.ID_MITT_DEST)
                            .Join(this._dbContext.CorrGlobaliEntities, lista => lista.SYSTEM_ID, corr => corr.SYSTEM_ID, (lista, corr) => new { corr.VAR_DESC_CORR, corr.DTA_FINE })
                            .Where(j => j.DTA_FINE == null)
                            .Select( j=> j.VAR_DESC_CORR + " (D)").ToListAsync())
                            : entity.VAR_DESC_CORR;
                }
            }

            return result;
        }

        private async Task<InfoFascicoloAppartenenza[]> GetClassCatTipo(long docnumber, long idPeople, long idRuolo)
        {
            InfoFascicoloAppartenenza[] result = null;

            var listIdFascicolo = this._dbContext.ProjectEntities
                .Join(this._dbContext.ProjectComponentEntities, project => project.SYSTEM_ID, projectComp => projectComp.PROJECT_ID, (project, projectComp) => new { project.ID_FASCICOLO, projectComp.LINK })
                .Where(j => j.LINK == docnumber)
                .Select(j => j.ID_FASCICOLO)
                .ToList();

            var entities = await this._dbContext.ProjectEntities.Where(p => p.CHA_TIPO_PROJ == "F" && p.CHA_TIPO_FASCICOLO != "G" && listIdFascicolo.Contains(p.SYSTEM_ID))
                .Select(p => new
                {
                    p.VAR_CODICE, 
                    p.CHA_TIPO_FASCICOLO,
                    p.SYSTEM_ID
                })
                .ToListAsync();

            if(entities != null && entities.Count > 0)
            {
                result = new InfoFascicoloAppartenenza[entities.Count];
                var i = 0;
                
                foreach (var p in entities)
                {
                    result[i] = new InfoFascicoloAppartenenza
                    {
                        CodiceFascicolo = p.VAR_CODICE,
                        TipoFascicolo = p.CHA_TIPO_FASCICOLO,
                        IdProject = p.SYSTEM_ID.ToString(),
                        SicurezzaUtente = (await this._dbContext.GetSecurityRights(p.SYSTEM_ID.ToString(), idPeople.ToString(), idRuolo.ToString())) != SecurityRightTypesEnum.Deny ? "1" : "0"
                    };
                    i++;
                }
            }
            
            return result;
        }

        #endregion
    }
}
