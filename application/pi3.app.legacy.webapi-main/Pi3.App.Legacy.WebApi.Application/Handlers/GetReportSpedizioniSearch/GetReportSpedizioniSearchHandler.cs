// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.FormatiDocumento;
using DocsPaVO.Spedizione;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.GetReportSpedizioni;
using Pi3.App.Legacy.WebApi.Application.Handlers.GetReportSpedizioniDocumenti;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetReportSpedizioniSearchRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetReportSpedizioniSearch;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetReportSpedizioniSearch
{
    public class GetReportSpedizioniSearchHandler : IRequestHandler<GetReportSpedizioniSearchRequest, GetReportSpedizioniSearchResult>
    {
        #region Public Members

        public GetReportSpedizioniSearchHandler(ILogger<GetReportSpedizioniSearchHandler> logger,
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

        public async Task<GetReportSpedizioniSearchResult> Handle(GetReportSpedizioniSearchRequest request, CancellationToken cancellationToken)
        {
            InfoDocumentoSpedito[] output = null;

            try
            {
                List<long> idDocumenti = null;
                if (request.filters.idDocumenti == null || !request.filters.idDocumenti.Any())        
                {
                    long idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
                    long idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
                    long?[] personOrGroup = new long?[] { idGroup, idUser };

                    var queryableIdDocument = this._dbContext.StatoInvioEntities
                    .Join(this._dbContext.ProfileEntities, statoInvio => statoInvio.ID_PROFILE, profile => profile.SYSTEM_ID, (statoInvio, profile) => new { statoInvio, profile })
                    .Join(this._dbContext.DocumentTypesEntities, j => j.statoInvio.ID_DOCUMENTTYPE, documentType => documentType.SYSTEM_ID, (j, documentType) => new { j.statoInvio, j.profile, documentType })
                    .Join(this._dbContext.CorrGlobaliEntities, j => j.statoInvio.ID_CORR_GLOBALE, corr => corr.SYSTEM_ID, (j, corr) => new ReportSpedizioneEntity()
                    {
                        ID_PROFILE = j.statoInvio.ID_PROFILE,
                        DOCNAME = j.profile.DOCNAME,
                        NUM_PROTO = j.profile.NUM_PROTO,
                        VAR_PROF_OGGETTO = j.profile.VAR_PROF_OGGETTO,
                        MEZZO_SPEDIZIONE = j.documentType.DESCRIPTION,
                        TYPE_ID_MEZZO = j.documentType.TYPE_ID,
                        ID_DESTINATARIO = corr.SYSTEM_ID,
                        DESTINATARIO = corr.VAR_DESC_CORR,
                        MAIL_DESTINATARIO = j.statoInvio.VAR_INDIRIZZO,
                        DATA_SPEDIZIONE = j.statoInvio.DTA_SPEDIZIONE ?? DateTime.MinValue,
                        STATO = j.statoInvio.STATUS_C_MASK ?? string.Empty,
                        NUM_PROTO_DESTINATARIO = j.statoInvio.VAR_PROTO_DEST,
                        DATA_PROTO_DESTINATARIO = j.statoInvio.DTA_PROTO_DEST,
                        MOTIVO_ANNULLA = j.statoInvio.VAR_MOTIVO_ANNULLA
                    })
                    .Where(s => this._dbContext.SecurityEntities.Any(e => e.THING == s.ID_PROFILE && personOrGroup.Contains(e.PERSONORGROUP) && e.ACCESSRIGHTS > 0));
                    queryableIdDocument = ApplyFilter(queryableIdDocument, request.filters, idGroup.ToString(), true, null);

                    var idDocumentSpediti = await queryableIdDocument
                        .GroupBy(s => s.ID_PROFILE)
                        .Select(g => new
                        {
                            ID_PROFILE = g.Key, DTA_SPEDIZIONE = g.Max(s => s.DATA_SPEDIZIONE),
                        })
                        .OrderByDescending(s =>s.DTA_SPEDIZIONE)
                        .ToListAsync();

                    if(idDocumentSpediti != null && idDocumentSpediti.Any())
                        idDocumenti = idDocumentSpediti.Select(s => (long)s.ID_PROFILE).ToList();
                }
                else
                {
                    idDocumenti = request.filters.idDocumenti.Select(i => i.AsLong()).ToList();
                }

                if (idDocumenti != null && idDocumenti.Any())
                {
                    var queryable = this._dbContext.StatoInvioEntities
                        .Join(this._dbContext.ProfileEntities, statoInvio => statoInvio.ID_PROFILE, profile => profile.SYSTEM_ID, (statoInvio, profile) => new { statoInvio, profile })
                        .Join(this._dbContext.DocumentTypesEntities, j => j.statoInvio.ID_DOCUMENTTYPE, documentType => documentType.SYSTEM_ID, (j, documentType) => new { j.statoInvio, j.profile, documentType })
                        .Join(this._dbContext.CorrGlobaliEntities, j => j.statoInvio.ID_CORR_GLOBALE, corr => corr.SYSTEM_ID, (j, corr) => new { j.statoInvio, j.profile, j.documentType, corr })
                        .Join(this._dbContext.DocArrivoParEntities, j => j.statoInvio.ID_PROFILE, docArrivoPar => docArrivoPar.ID_PROFILE, (j, docArrivoPar) => new ReportSpedizioneEntity()
                        {
                            ID_PROFILE = j.statoInvio.ID_PROFILE,
                            DOCNAME = j.profile.DOCNAME,
                            NUM_PROTO = j.profile.NUM_PROTO,
                            VAR_PROF_OGGETTO = j.profile.VAR_PROF_OGGETTO,
                            MEZZO_SPEDIZIONE = j.documentType.DESCRIPTION,
                            TYPE_ID_MEZZO = j.documentType.TYPE_ID,
                            ID_DESTINATARIO = j.corr.SYSTEM_ID,
                            DESTINATARIO = j.corr.VAR_DESC_CORR,
                            MAIL_DESTINATARIO = j.statoInvio.VAR_INDIRIZZO,
                            DATA_SPEDIZIONE = j.statoInvio.DTA_SPEDIZIONE ?? DateTime.MinValue,
                            STATO = j.statoInvio.STATUS_C_MASK ?? string.Empty,
                            TIPO_DESTINATARIO = docArrivoPar.CHA_TIPO_MITT_DEST,
                            NUM_PROTO_DESTINATARIO = j.statoInvio.VAR_PROTO_DEST,
                            DATA_PROTO_DESTINATARIO = j.statoInvio.DTA_PROTO_DEST,
                            MOTIVO_ANNULLA = j.statoInvio.VAR_MOTIVO_ANNULLA,
                            MAIL_MITTENTE = this._dbContext.SendStoEntities
                                            .Where(s => s.ESITO == "Spedito"
                                            && s.DTA_SPEDIZIONE >= j.statoInvio.DTA_SPEDIZIONE
                                            && s.ID_PROFILE != null
                                            && s.ID_PROFILE == j.statoInvio.ID_PROFILE
                                            && s.ID_CORR_GLOBALE != null
                                            && s.ID_CORR_GLOBALE == j.statoInvio.ID_CORR_GLOBALE)
                                            .Max(s => s.MAIL_MITTENTE),
                            ID_DOC_ARRIVO_PAR = docArrivoPar.SYSTEM_ID,
                            ID_MITT_DEST = docArrivoPar.ID_MITT_DEST
                        })
                        .Where(s => s.ID_DESTINATARIO == s.ID_MITT_DEST);

                    queryable = ApplyFilter(queryable, request.filters, request.idGruppo, request.allReceivers, idDocumenti);

                    var reporSpedizione = await queryable
                        .OrderBy(s => s.ID_DOC_ARRIVO_PAR)
                        .Select(s => s)
                        .ToListAsync();

                    if (reporSpedizione != null)
                    {
                        List<InfoDocumentoSpedito> infoDocSpediti = new List<InfoDocumentoSpedito>();
                        foreach (var idDocument in idDocumenti)
                        {
                            var infoDoc = new InfoDocumentoSpedito();
                            infoDoc.InfoSpedizione = InfoDocumentoSpedito.TipoInfoSpedizione.Effettuato;

                            List<InfoSpedizione> infoSpedizioDoc = new List<InfoSpedizione>();
                            reporSpedizione.Where(r => r.ID_PROFILE == idDocument).ForEach(r =>
                            {
                                infoDoc.IDDocumento = r.ID_PROFILE.ToString();
                                infoDoc.Protocollo = r.DOCNAME;
                                infoDoc.DescrizioneDocumento = r.VAR_PROF_OGGETTO;
                                infoSpedizioDoc.Add(this._mapper.Map<InfoSpedizione>(r));
                            });

                            if (infoSpedizioDoc.Any())
                            {
                                //Aggiorno l'infoSpedizione
                                if (infoSpedizioDoc.Any(s => s.Azione_Info == InfoSpedizione.TipologiaAzione.Rispedire))
                                {
                                    infoDoc.InfoSpedizione = InfoDocumentoSpedito.TipoInfoSpedizione.Errore;
                                }
                                else if (infoSpedizioDoc.Any(s => s.Azione_Info == InfoSpedizione.TipologiaAzione.Attendere))
                                {
                                    infoDoc.InfoSpedizione = InfoDocumentoSpedito.TipoInfoSpedizione.Warning;
                                }

                                infoDoc.Spedizioni = infoSpedizioDoc;
                                infoDocSpediti.Add(infoDoc);
                            }
                        }

                        output = infoDocSpediti.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new GetReportSpedizioniSearchResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetReportSpedizioniSearchHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ReportSpedizioneEntity, InfoSpedizione>()
                     .ForMember(dest => dest.MezzoSpedizione, opt => opt.MapFrom(src => src.MEZZO_SPEDIZIONE))
                     .ForMember(dest => dest.IdMezzoSpedizione, opt => opt.MapFrom(src => src.TYPE_ID_MEZZO))
                     .ForMember(dest => dest.NominativoDestinatario, opt => opt.MapFrom(src => src.DESTINATARIO))
                     .ForMember(dest => dest.EMailDestinatario, opt => opt.MapFrom(src => src.MAIL_DESTINATARIO))
                     .ForMember(dest => dest.EMailMittente, opt => opt.MapFrom(src => src.MAIL_MITTENTE ?? string.Empty))
                     .ForMember(dest => dest.DataSpedizione, opt => opt.MapFrom(src => src.DATA_SPEDIZIONE.AsDateTimeFormat()))
                     .ForMember(dest => dest.ProtocolloDestinatario, opt => opt.MapFrom(src => src.NUM_PROTO_DESTINATARIO ?? string.Empty))
                     .ForMember(dest => dest.DataProtDest, opt => opt.MapFrom(src => src.DATA_PROTO_DESTINATARIO.HasValue ? src.DATA_PROTO_DESTINATARIO.AsDateTimeFormat() : string.Empty))
                     .ForMember(dest => dest.MotivoAnnullEccezione, opt => opt.MapFrom(src => src.MOTIVO_ANNULLA ?? string.Empty))
                     .AfterMap((src, dest) =>
                     {
                         if (src.TYPE_ID_MEZZO == "SIMPLIFIEDINTEROPERABILITY" || (src.MAIL_DESTINATARIO != null && src.MAIL_DESTINATARIO.Contains("InteroperabilityService.svc")))
                         {
                             dest.EMailDestinatario = "N.A.";
                             dest.EMailMittente = "N.A.";
                         }

                         dest.TipoDestinatario = string.Empty;
                         if (src.TIPO_DESTINATARIO == "D" || src.TIPO_DESTINATARIO == "F" || src.TIPO_DESTINATARIO == "L")
                         {
                             dest.TipoDestinatario = "D";
                         }
                         if (src.TIPO_DESTINATARIO == "C")
                         {
                             dest.TipoDestinatario = "CC";
                         }

                         if (!string.IsNullOrEmpty(src.STATO))
                         {
                             switch (src.STATO.Substring(0, 1))
                             {
                                 case "A":
                                     dest.Azione_Info = InfoSpedizione.TipologiaAzione.Attendere;
                                     break;
                                 case "V":
                                     dest.Azione_Info = InfoSpedizione.TipologiaAzione.OK;
                                     break;
                                 case "X":
                                     dest.Azione_Info = InfoSpedizione.TipologiaAzione.Rispedire;
                                     break;
                                 default:
                                     dest.Azione_Info = InfoSpedizione.TipologiaAzione.Nessuna;
                                     break;
                             }

                             dest.TipoRicevuta_Accettazione = GetStatoRicevuta(src.STATO.Substring(1, 1));
                             dest.TipoRicevuta_Consegna = GetStatoRicevuta(src.STATO.Substring(2, 1));
                             dest.TipoRicevuta_Conferma = GetStatoRicevuta(src.STATO.Substring(3, 1));
                             dest.TipoRicevuta_Annullamento = GetStatoRicevuta(src.STATO.Substring(4, 1));
                             dest.TipoRicevuta_Eccezione = GetStatoRicevuta(src.STATO.Substring(5, 1));

                         }

                         // Problema associazione valori sbagliati Statusmask
                         if (src.MEZZO_SPEDIZIONE != "INTEROPERABILITA" && src.TYPE_ID_MEZZO != "SIMPLIFIEDINTEROPERABILITY")
                         {
                             dest.TipoRicevuta_Annullamento = InfoSpedizione.TipologiaStatoRicevuta.AttendereCausaMezzo;
                             dest.TipoRicevuta_Conferma = InfoSpedizione.TipologiaStatoRicevuta.AttendereCausaMezzo;

                         }
                         else
                         {
                             if ((!src.DATA_PROTO_DESTINATARIO.HasValue || string.IsNullOrEmpty(src.NUM_PROTO_DESTINATARIO))
                                 && (dest.Azione_Info == InfoSpedizione.TipologiaAzione.OK || dest.TipoRicevuta_Conferma == InfoSpedizione.TipologiaStatoRicevuta.OK))
                             {
                                 dest.Azione_Info = InfoSpedizione.TipologiaAzione.Attendere;
                                 dest.TipoRicevuta_Conferma = InfoSpedizione.TipologiaStatoRicevuta.Attendere;
                                 dest.TipoRicevuta_Annullamento = InfoSpedizione.TipologiaStatoRicevuta.Attendere;
                             }
                         }
                     });
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class ReportSpedizioneEntity
        {
            public long? ID_PROFILE { get; set; }
            public long? ID_DOC_ARRIVO_PAR { get; set; }
            public string? DOCNAME { get; set; }
            public long? NUM_PROTO { get; set; }
            public string? VAR_PROF_OGGETTO { get; set; }
            public string? MEZZO_SPEDIZIONE { get; set; }
            public string? TYPE_ID_MEZZO { get; set; }
            public long? ID_DESTINATARIO { get; set; }
            public string? DESTINATARIO { get; set; }
            public string? MAIL_DESTINATARIO { get; set; }
            public DateTime DATA_SPEDIZIONE { get; set; }
            public string STATO { get; set; }
            public string? TIPO_DESTINATARIO { get; set; }
            public long? ID_MITT_DEST { get; set; }
            public string? NUM_PROTO_DESTINATARIO { get; set; }
            public DateTime? DATA_PROTO_DESTINATARIO { get; set; }
            public string? MOTIVO_ANNULLA { get; set; }
            public string? MAIL_MITTENTE { get; set; }
        }

        protected IQueryable<ReportSpedizioneEntity> ApplyFilter(IQueryable<ReportSpedizioneEntity> queryable, FiltriReportSpedizioni filters, string idGruppo, bool allReceivers, List<long> idDocumenti)
        {

            if (idDocumenti != null && idDocumenti.Any())
            {
                queryable = queryable.Where(s => idDocumenti.Contains(s.ID_PROFILE ?? 0));

            }
            if (string.IsNullOrEmpty(filters.IdDocumento))
            {
                switch (filters.FiltroData)
                {
                    case FiltriReportSpedizioni.TipoFiltroData.Intervallo:
                    case FiltriReportSpedizioni.TipoFiltroData.MeseCorrente:
                    case FiltriReportSpedizioni.TipoFiltroData.SettimanaCorrente:
                        queryable = queryable.Where(s => s.DATA_SPEDIZIONE.Date >= filters.DataDa.Date && s.DATA_SPEDIZIONE.Date <= filters.DataA.Date);
                        break;
                    case FiltriReportSpedizioni.TipoFiltroData.Oggi:
                    case FiltriReportSpedizioni.TipoFiltroData.ValoreSingolo:
                        queryable = queryable.Where(s => s.DATA_SPEDIZIONE.Date == filters.DataDa.Date);
                        break;
                }

                //Filtro ruolo
                if (filters.VisibilitaDoc == FiltriReportSpedizioni.TipoVisibilitaDocumenti.AllDocByRuolo)
                {
                    var idGroup = idGruppo.AsLong();

                    queryable = queryable.Where(s =>
                    (this._dbContext.SendStoEntities.Where(i => i.ID_GROUP_SENDER == idGroup && i.ESITO == "Spedito"
                        && i.DTA_SPEDIZIONE >= s.DATA_SPEDIZIONE
                        && i.ID_PROFILE != null && i.ID_PROFILE == s.ID_PROFILE
                        && i.ID_CORR_GLOBALE == s.ID_DESTINATARIO)
                    .Select(i => i.ID_PROFILE).ToList()).Contains(s.ID_PROFILE)
                    );
                }
            }

            if (allReceivers)
            {
                // filtro registro e mail mittente
                if (!string.IsNullOrEmpty(filters.IdRegMailMittente) && !string.IsNullOrEmpty(filters.MailMittente))
                {
                    var IdRegMailMittente = filters.IdRegMailMittente.AsLong();
                    queryable = queryable
                        .Where(s => this._dbContext.SendStoEntities
                        .Any(x => x.ID_PROFILE != null && x.ID_PROFILE == s.ID_PROFILE && x.MAIL_MITTENTE == filters.MailMittente
                        && x.ID_REG_MAIL_MITTENTE == IdRegMailMittente
                        && x.ESITO == "Spedito" && x.DTA_SPEDIZIONE >= s.DATA_SPEDIZIONE && x.ID_CORR_GLOBALE == s.ID_DESTINATARIO));
                }
                ExpressionStarter<ReportSpedizioneEntity> predicateTipoRicevuta = null;

                //In EF il substring parte da 0 -> quindi s.STATO.Substring(2, 1) == "V" diventa s.STATO.Substring(1, 1) == "V"
                if (filters.TipoRicevuta_Accettazione)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<ReportSpedizioneEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATO.Substring(1, 1) == "V");
                }

                if (filters.TipoRicevuta_MancataAccettazione)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<ReportSpedizioneEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATO.Substring(1, 1) == "X");
                }

                if (filters.TipoRicevuta_AvvenutaConsegna)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<ReportSpedizioneEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATO.Substring(2, 1) == "V");
                }

                if (filters.TipoRicevuta_MancataConsegna)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<ReportSpedizioneEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATO.Substring(2, 1) == "X");
                }

                if (filters.TipoRicevuta_ConfermaRicezione)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<ReportSpedizioneEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATO.Substring(3, 1) == "V");
                }

                if (filters.TipoRicevuta_AnnullamentoProtocollazione)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<ReportSpedizioneEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATO.Substring(4, 1) == "V");
                }

                if (filters.TipoRicevuta_Eccezione)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<ReportSpedizioneEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATO.Substring(5, 1) == "X");
                }

                if (filters.TipoRicevuta_ConErrori)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<ReportSpedizioneEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATO.Substring(6, 1) == "V");
                }

                if (predicateTipoRicevuta != null)
                    queryable = queryable.Where(predicateTipoRicevuta);
            }
            else
            {
                ExpressionStarter<StatoInvioEntity> predicateTipoRicevuta = null;

                if (filters.TipoRicevuta_Accettazione)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<StatoInvioEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATUS_C_MASK.Substring(1, 1) == "V");
                }

                if (filters.TipoRicevuta_MancataAccettazione)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<StatoInvioEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATUS_C_MASK.Substring(1, 1) == "X");
                }

                if (filters.TipoRicevuta_AvvenutaConsegna)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<StatoInvioEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATUS_C_MASK.Substring(2, 1) == "V");
                }

                if (filters.TipoRicevuta_MancataConsegna)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<StatoInvioEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATUS_C_MASK.Substring(2, 1) == "X");
                }

                if (filters.TipoRicevuta_ConfermaRicezione)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<StatoInvioEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATUS_C_MASK.Substring(3, 1) == "V");
                }

                if (filters.TipoRicevuta_AnnullamentoProtocollazione)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<StatoInvioEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATUS_C_MASK.Substring(4, 1) == "V");
                }

                if (filters.TipoRicevuta_Eccezione)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<StatoInvioEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATUS_C_MASK.Substring(5, 1) == "X");
                }

                if (filters.TipoRicevuta_ConErrori)
                {
                    if (predicateTipoRicevuta == null)
                        predicateTipoRicevuta = PredicateBuilder.New<StatoInvioEntity>();

                    predicateTipoRicevuta = predicateTipoRicevuta.Or(s => s.STATUS_C_MASK.Substring(6, 1) == "V");
                }

                if (predicateTipoRicevuta != null)
                {
                    var queryableTipoRicevuta = this._dbContext.StatoInvioEntities.Where(predicateTipoRicevuta);
                    queryable = queryable.Where(s => queryableTipoRicevuta.Any(t => t.ID_PROFILE == s.ID_PROFILE));
                }
            }

            //Esito Complessivo
            if (idDocumenti == null)
            {
                ExpressionStarter<ReportSpedizioneEntity> predicateEsitoComplessivo = null;
                if (filters.TipoRicevuta_EsitoOK)
                {
                    if (predicateEsitoComplessivo == null)
                        predicateEsitoComplessivo = PredicateBuilder.New<ReportSpedizioneEntity>();

                    predicateEsitoComplessivo = predicateEsitoComplessivo
                        .Or(s => !this._dbContext.StatoInvioEntities.Any(e => e.ID_PROFILE == s.ID_PROFILE && e.STATUS_C_MASK.StartsWith("A"))
                        && !this._dbContext.StatoInvioEntities.Any(e => e.ID_PROFILE == s.ID_PROFILE && e.STATUS_C_MASK.StartsWith("X")));
                }
                if (filters.TipoRicevuta_EsitoAttesa)
                {
                    if (predicateEsitoComplessivo == null)
                        predicateEsitoComplessivo = PredicateBuilder.New<ReportSpedizioneEntity>();

                    predicateEsitoComplessivo = predicateEsitoComplessivo
                        .Or(s => this._dbContext.StatoInvioEntities.Any(e => e.ID_PROFILE == s.ID_PROFILE && e.STATUS_C_MASK.StartsWith("A"))
                        && !this._dbContext.StatoInvioEntities.Any(e => e.ID_PROFILE == s.ID_PROFILE && e.STATUS_C_MASK.StartsWith("X")));
                }
                if (filters.TipoRicevuta_EsitoKO)
                {
                    if (predicateEsitoComplessivo == null)
                        predicateEsitoComplessivo = PredicateBuilder.New<ReportSpedizioneEntity>();

                    predicateEsitoComplessivo = predicateEsitoComplessivo
                        .Or(s => this._dbContext.StatoInvioEntities.Any(e => e.ID_PROFILE == s.ID_PROFILE && e.STATUS_C_MASK.StartsWith("X")));
                }
                if (predicateEsitoComplessivo != null)
                {
                    queryable = queryable.Where(predicateEsitoComplessivo);
                }
            }
            return queryable;
        }

        protected InfoSpedizione.TipologiaStatoRicevuta GetStatoRicevuta(string charMask)
        {
            /*
             * Spiegazione funzionamento campo status_c_mask
             * Nella tabella dpa_stato_invio è presente un campo di 7 caratteri denominato status_c_mask. I caratteri indicano
             * Essi possono assumere i valori A (Attendere), V (OK), X (KO), N(Non prevista)
             * 1 - Esito generale
             * Indica se una spedizione ha raggiunto l'esito finale, se è in stato di attesa o se ha avuto un errore. Per le spedizioni via mail
             * basta l'accettazione per esito positivo, per le mail certificate serve la consegna, per i corrispondenti interoperabili serve la 
             * conferma. Viene controllata dal metodo GetStatoAzione sottostante.
             * 2 - Ricevuta di accettazione
             * Mail inviata dal provider nel caso di accettazione. Prevista per mail, PEC, PEC Interoperante
             * 3 - Ricevuta di consegna
             * Ricevuta che indica la consegna al destinatario. Prevista per PEC, PEC Int., Int.Semplificata.
             * 4 - Ricevuta di conferma
             * Indica se il destinatario ha protocollato il documento spedito. Prevista per PEC Int. e Int. Semplificata
             * 5 - Ricevuta di annullamento
             * Nel caso il documento protocollato venga annullato
             * 6 - Eccezione
             * Nel caso ci siano degli errori per i destinatari interoperanti
             * 7 - Con errori
             * Questo riguarda gli errori generici come DNS etc.
             */

            switch (charMask)
            {
                case "A": return InfoSpedizione.TipologiaStatoRicevuta.Attendere;
                case "V": return InfoSpedizione.TipologiaStatoRicevuta.OK;
                case "X": return InfoSpedizione.TipologiaStatoRicevuta.KO;
                case "N": return InfoSpedizione.TipologiaStatoRicevuta.AttendereCausaMezzo;
                default: return InfoSpedizione.TipologiaStatoRicevuta.Nessuna;
            }
        }

        #endregion
    }
}
