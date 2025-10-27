// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.LibroFirma;
using DocsPaVO.Mobile;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.GetIstanzaProcessoDiFirmaByIdIstanzaProcesso;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetIstanzaProcessoDiFirmaByDocnumberRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetIstanzaProcessoDiFirmaByDocnumber;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetIstanzaProcessoDiFirmaByDocnumber
{
    public class GetIstanzaProcessoDiFirmaByDocnumberHandler : IRequestHandler<GetIstanzaProcessoDiFirmaByDocnumberRequest, GetIstanzaProcessoDiFirmaByDocnumberResult>
    {
        #region Public Members

        public GetIstanzaProcessoDiFirmaByDocnumberHandler(ILogger<GetIstanzaProcessoDiFirmaByDocnumberHandler> logger,
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

        public async Task<GetIstanzaProcessoDiFirmaByDocnumberResult> Handle(GetIstanzaProcessoDiFirmaByDocnumberRequest request, CancellationToken cancellationToken)
        {
            List<IstanzaProcessoDiFirma> output = new List<IstanzaProcessoDiFirma>();

            try
            {
                var docnumber = request.docnumber.AsLong();

                var istanzaProcessoFirmaEntity = await this._dbContext.IstanzaProcessoFirmaEntities
                   .Join(this._dbContext.CorrGlobaliEntities, istanza => istanza.ID_RUOLO_PROPONENTE, ruoloProponente => ruoloProponente.ID_GRUPPO, (istanza, ruoloProponente) => new { istanza, ruoloProponente })
                   .Join(this._dbContext.CorrGlobaliEntities, j => j.istanza.ID_UTENTE_PROPONENTE, utenteProponente => utenteProponente.ID_PEOPLE, (j, utenteProponente) => new { j.istanza, j.ruoloProponente, utenteProponente })
                   .GroupJoin(this._dbContext.PeopleEntities, j => j.istanza.ID_PEOPLE_INTERRUZIONE, peopleInter => peopleInter.SYSTEM_ID, (j, peopleInter) => new { j.istanza, j.ruoloProponente, j.utenteProponente, peopleInter })
                   .SelectMany(j => j.peopleInter.DefaultIfEmpty(), (j, peopleInter) => new  { j.istanza, j.ruoloProponente, j.utenteProponente, peopleInter})
                   .GroupJoin(this._dbContext.PeopleEntities, j => j.istanza.ID_PEOPLE_DELEGATO_INTER, peopleDelegatoInter => peopleDelegatoInter.SYSTEM_ID, (j, peopleDelegatoInter) => new { j.istanza, j.ruoloProponente, j.utenteProponente, j.peopleInter, peopleDelegatoInter })
                   .SelectMany(j => j.peopleDelegatoInter.DefaultIfEmpty(), (j, peopleDelegatoInter) => new { j.istanza, j.ruoloProponente, j.utenteProponente, j.peopleInter, peopleDelegatoInter })
                   .GroupJoin(this._dbContext.PeopleEntities, j => j.istanza.ID_PEOPLE_DELEGATO, delegato => delegato.SYSTEM_ID, (j, delegato) => new { j.istanza, j.ruoloProponente, j.utenteProponente, j.peopleInter, j.peopleDelegatoInter, delegato })
                   .SelectMany(j => j.delegato.DefaultIfEmpty(), (j, delegato) =>
                   new IstanzaProcessiFirmaEntity()
                   {
                       IstanzaProcessoFirma = j.istanza,
                       RuoloProponente = new RuoloEntity()
                       {
                           ID_GROUP = j.ruoloProponente.ID_GRUPPO,
                           GROUP_DESCRIPTION = j.ruoloProponente.VAR_DESC_CORR,
                           GROUP_ID = j.ruoloProponente.VAR_COD_RUBRICA,
                           ID_CORR_GLOBALI_RUOLO = j.ruoloProponente.SYSTEM_ID
                       },
                       UtenteProponente = new UtenteEntity()
                       {
                           ID_CORR_GLOBALI_UTENTE = j.utenteProponente.SYSTEM_ID,
                           ID_USER = j.utenteProponente.ID_PEOPLE,
                           USER_DESCRIPTION = j.utenteProponente.VAR_DESC_CORR,
                           USER_ID = j.utenteProponente.VAR_COD_RUBRICA
                       },
                       DescUtenteDelegato = delegato.FULL_NAME,
                       DescUtenteDelegatoInterruzione = j.peopleDelegatoInter.FULL_NAME,
                       DescUtenteInterruzione = j.peopleInter.FULL_NAME
                   })
                   .Where(j => j.IstanzaProcessoFirma.ID_DOCUMENTO == docnumber)
                   .OrderByDescending(j => j.IstanzaProcessoFirma.ATTIVATO_IL)
                   .ToListAsync();

                if(istanzaProcessoFirmaEntity != null && istanzaProcessoFirmaEntity.Any())
                {
                    istanzaProcessoFirmaEntity.ForEach(p =>
                    {
                        p.istanzePassoDiFirma = this._dbContext.IstanzaPassoFirmaEntities
                        .Join(this._dbContext.AnagraficaEventiEntities, istanzaPasso => istanzaPasso.TIPO_EVENTO, evento => evento.ID_EVENTO, (istanzaPasso, evento) => new { istanzaPasso, evento })
                        .GroupJoin(this._dbContext.CorrGlobaliEntities, j => j.istanzaPasso.ID_RUOLO_COINVOLTO, ruolo => ruolo.ID_GRUPPO, (j, ruolo) => new { j.istanzaPasso, j.evento, ruoloCoinvolto = ruolo })
                        .SelectMany(j => j.ruoloCoinvolto.DefaultIfEmpty(), (j, ruolo) => new { j.istanzaPasso, j.evento, ruoloCoinvolto = ruolo })
                        .GroupJoin(this._dbContext.CorrGlobaliEntities, j => j.istanzaPasso.ID_UTENTE_COINVOLTO, utente => utente.ID_PEOPLE, (j, utente) => new { j.istanzaPasso, j.evento, j.ruoloCoinvolto, utenteCoinvolto = utente })
                        .SelectMany(j => j.utenteCoinvolto.DefaultIfEmpty(), (j, utente) => new { j.istanzaPasso, j.evento, j.ruoloCoinvolto, utenteCoinvolto = utente })
                        .Where(j => j.istanzaPasso.ID_ISTANZA_PROCESSO == p.IstanzaProcessoFirma.ID_ISTANZA)
                        .Select(j => new IstanzaPassiFirmaEntity()
                        {
                            IstanzaPassoFirma = j.istanzaPasso,
                            Evento = j.evento,
                            RuoloCoinvolto = new RuoloEntity()
                            {
                                ID_GROUP = j.ruoloCoinvolto.ID_GRUPPO,
                                GROUP_DESCRIPTION = j.ruoloCoinvolto.VAR_DESC_CORR,
                                GROUP_ID = j.ruoloCoinvolto.VAR_COD_RUBRICA,
                                ID_CORR_GLOBALI_RUOLO = j.ruoloCoinvolto.SYSTEM_ID
                            },
                            UtenteCoinvolto = new UtenteEntity()
                            {
                                ID_CORR_GLOBALI_UTENTE = j.utenteCoinvolto.SYSTEM_ID,
                                ID_USER = j.utenteCoinvolto.ID_PEOPLE,
                                USER_DESCRIPTION = j.utenteCoinvolto.VAR_DESC_CORR,
                                USER_ID = j.utenteCoinvolto.VAR_COD_RUBRICA
                            }
                        })
                        .OrderBy(j => j.IstanzaPassoFirma.NUMERO_SEQUENZA)
                        .ToList();
                    });

                    output = this._mapper.Map<IstanzaProcessoDiFirma[]>(istanzaProcessoFirmaEntity).ToList();
                }
            }       
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new GetIstanzaProcessoDiFirmaByDocnumberResult(output);

        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetIstanzaProcessoDiFirmaByDocnumberHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IstanzaProcessiFirmaEntity, IstanzaProcessoDiFirma>()
                     .ForMember(dest => dest.idIstanzaProcesso, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.ID_ISTANZA))
                     .ForMember(dest => dest.idProcesso, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.ID_PROCESSO))
                     .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.DESCRIZIONE))
                     .ForMember(dest => dest.dataAttivazione, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.ATTIVATO_IL.AsDateTimeFormat()))
                     .ForMember(dest => dest.dataChiusura, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.CONCLUSO_IL.AsDateTimeFormat()))
                     .ForMember(dest => dest.NoteDiAvvio, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.NOTE ?? string.Empty))
                     .ForMember(dest => dest.MotivoRespingimento, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.MOTIVO_RESPINGIMENTO ?? string.Empty))
                     .ForMember(dest => dest.DescUtenteDelegato, opt => opt.MapFrom(src => src.DescUtenteDelegato ?? string.Empty))
                     .ForMember(dest => dest.DescUtenteInterruzione, opt => opt.MapFrom(src => src.DescUtenteInterruzione ?? string.Empty))
                     .ForMember(dest => dest.DescUtenteDelegatoInterruzione, opt => opt.MapFrom(src => src.DescUtenteDelegatoInterruzione ?? string.Empty))
                     .ForMember(dest => dest.docAll, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.DOC_ALL ?? string.Empty))
                     .ForMember(dest => dest.statoProcesso, opt => opt.MapFrom(src => (TipoStatoProcesso)Enum.Parse(typeof(TipoStatoProcesso), src.IstanzaProcessoFirma.STATO)))
                     .ForMember(dest => dest.docNumber, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.ID_DOCUMENTO))
                     .ForMember(dest => dest.ChaInterroDa, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.CHA_INTERROTTO_DA != null ? Convert.ToChar(src.IstanzaProcessoFirma.CHA_INTERROTTO_DA) : '1'))
                     .ForMember(dest => dest.AttivatoPerPassaggioStato, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.CHA_CAMBIO_STATO_DIAG == "1"))
                     .ForMember(dest => dest.IdStatoInterruzione, opt => opt.MapFrom(src => src.IstanzaProcessoFirma.ID_STATO_INTERRUZIONE != null ? src.IstanzaProcessoFirma.ID_STATO_INTERRUZIONE.ToString() : string.Empty))
                     .AfterMap((src, dest) =>
                     {
                         dest.Notifiche = new OpzioniNotifica()
                         {
                             Notifica_concluso = src.IstanzaProcessoFirma.NOTIFICA_CONCLUSO == "1",
                             Notifica_interrotto = src.IstanzaProcessoFirma.NOTIFICA_INTERROTTO == "1",
                             NotificaErrore = src.IstanzaProcessoFirma.NOTIFICA_ERRORE == "1",
                             NotificaPresenzaDestNonInterop = src.IstanzaProcessoFirma.NOTIFICA_DEST_NON_INTEROP == "1"
                         };
                     });

                cfg.CreateMap<RuoloEntity, Ruolo>()
                  .ForMember(dest => dest.idGruppo, opt => opt.MapFrom(src => src.ID_GROUP))
                  .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.GROUP_ID))
                  .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.GROUP_DESCRIPTION))
                  .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.ID_CORR_GLOBALI_RUOLO));

                cfg.CreateMap<UtenteEntity, Utente>()
                   .ForMember(dest => dest.idPeople, opt => opt.MapFrom(src => src.ID_USER))
                   .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.USER_DESCRIPTION))
                   .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.USER_ID))
                   .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.ID_CORR_GLOBALI_UTENTE));

                cfg.CreateMap<AnagraficaEventiEntity, Evento>()
                   .ForMember(dest => dest.IdEvento, opt => opt.MapFrom(src => src.ID_EVENTO))
                   .ForMember(dest => dest.CodiceAzione, opt => opt.MapFrom(src => src.VAR_COD_AZIONE))
                   .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.DESCRIZIONE))
                   .ForMember(dest => dest.TipoEvento, opt => opt.MapFrom(src => src.CHA_TIPO_EVENTO))
                   .ForMember(dest => dest.Gruppo, opt => opt.MapFrom(src => src.GRUPPO))
                   .ForMember(dest => dest.Automatico, opt => opt.MapFrom(src => src.CHA_AUTOMATICO == "1"));

                cfg.CreateMap<IstanzaPassiFirmaEntity, IstanzaPassoDiFirma>()
                   .ForMember(dest => dest.CodiceTipoEvento, opt => opt.MapFrom(src => src.IstanzaPassoFirma.TIPO_FIRMA))
                   .ForMember(dest => dest.dataEsecuzione, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ESEGUITO_IL.AsDateTimeFormat()))
                   .ForMember(dest => dest.dataScadenza, opt => opt.MapFrom(src => src.IstanzaPassoFirma.SCADENZA.AsDateTimeFormat()))
                   .ForMember(dest => dest.statoPasso, opt => opt.MapFrom(src => (TipoStatoPasso)Enum.Parse(typeof(TipoStatoPasso), src.IstanzaPassoFirma.STATO_PASSO)))
                   .ForMember(dest => dest.idIstanzaPasso, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_ISTANZA_PASSO))
                   .ForMember(dest => dest.idIstanzaProcesso, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_ISTANZA_PROCESSO))
                   .ForMember(dest => dest.idNotificaEffettuata, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_NOTIFICA_EFFETTUATA.HasValue ? src.IstanzaPassoFirma.ID_NOTIFICA_EFFETTUATA.ToString() : string.Empty))
                   .ForMember(dest => dest.idPasso, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_PASSO))
                   .ForMember(dest => dest.motivoRespingimento, opt => opt.MapFrom(src => src.IstanzaPassoFirma.MOTIVO_RESPINGIMENTO ?? string.Empty))
                   .ForMember(dest => dest.numeroSequenza, opt => opt.MapFrom(src => src.IstanzaPassoFirma.NUMERO_SEQUENZA))
                   .ForMember(dest => dest.DescrizioneUtenteLocker, opt => opt.MapFrom(src => src.IstanzaPassoFirma.DESC_UTENTE_LOCKER))
                   .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.IstanzaPassoFirma.NOTE ?? string.Empty))
                   .ForMember(dest => dest.TipoFirma, opt => opt.MapFrom(src => src.IstanzaPassoFirma.TIPO_FIRMA))
                   .ForMember(dest => dest.IdAOO, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_AOO))
                   .ForMember(dest => dest.IdRF, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_RF))
                   .ForMember(dest => dest.IdMailRegistro, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_MAIL_REGISTRO))
                   .ForMember(dest => dest.IdTipologia, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_TIPOLOGIA))
                   .ForMember(dest => dest.IdStatoDiagramma, opt => opt.MapFrom(src => src.IstanzaPassoFirma.ID_STATO_DIAGRAMMA))
                   .ForMember(dest => dest.IsAutomatico, opt => opt.MapFrom(src => src.IstanzaPassoFirma.CHA_AUTOMATICO == "1"))
                   .ForMember(dest => dest.Errore, opt => opt.MapFrom(src => src.IstanzaPassoFirma.VAR_ERRORE))
                   .ForMember(dest => dest.ApplicaSegnaturaPermanente, opt => opt.MapFrom(src => src.IstanzaPassoFirma.CHA_POS_SEGNATURA))
                   .ForMember(dest => dest.PosizioneSegnaturaPermanente, opt => opt.MapFrom(src => src.IstanzaPassoFirma.VAR_POS_SEGNATURA));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class IstanzaProcessiFirmaEntity
        {
            public IstanzaProcessoFirmaEntity IstanzaProcessoFirma { get; set; }
            public string? DescUtenteDelegato { get; set; }
            public string? DescUtenteInterruzione { get; set; }
            public string? DescUtenteDelegatoInterruzione { get; set; }
            public RuoloEntity RuoloProponente { get; set; }
            public UtenteEntity UtenteProponente { get; set; }
            public List<IstanzaPassiFirmaEntity> istanzePassoDiFirma { get; set; }

        }

        protected class RuoloEntity
        {
            public long? ID_GROUP { get; set; }
            public string? GROUP_ID { get; set; }
            public string? GROUP_DESCRIPTION { get; set; }
            public long? ID_CORR_GLOBALI_RUOLO { get; set; }
        }

        protected class UtenteEntity
        {
            public long? ID_USER { get; set; }
            public string? USER_ID { get; set; }
            public string? USER_DESCRIPTION { get; set; }
            public long? ID_CORR_GLOBALI_UTENTE { get; set; }
        }

        protected class IstanzaPassiFirmaEntity
        {
            public IstanzaPassoFirmaEntity IstanzaPassoFirma { get; set; }
            public AnagraficaEventiEntity Evento { get; set; }
            public RuoloEntity? RuoloCoinvolto { get; set; }
            public UtenteEntity? UtenteCoinvolto { get; set; }
        }
        #endregion
    }
}
