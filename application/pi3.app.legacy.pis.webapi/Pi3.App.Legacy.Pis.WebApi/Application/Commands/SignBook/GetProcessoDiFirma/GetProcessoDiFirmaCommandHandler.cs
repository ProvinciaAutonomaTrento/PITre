// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetFunzioniRuolo;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.AvvioProcessoDiFirma;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetProcessoDiFirma
{
    public class GetProcessoDiFirmaCommandHandler : IRequestHandler<GetProcessoDiFirmaCommand, GetProcessoDiFirmaCommandResponse>
    {

        public GetProcessoDiFirmaCommandHandler(ILogger<GetProcessoDiFirmaCommandHandler> logger,
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

        public async Task<GetProcessoDiFirmaCommandResponse> Handle(GetProcessoDiFirmaCommand request, CancellationToken cancellationToken)
        {
            ProcessoFirma output = null;
            try
            {
                var idProcessoAsLong = request.IdProcesso.AsLong();
                var processoEntity = await this._dbContext.SchemaProcessoFirmaEntities.AsNoTracking()
                    .Where(p => p.ID_PROCESSO == idProcessoAsLong)
                    .FirstOrDefaultAsync();

                if (processoEntity == null)
                {
                    throw new ProcessoFirmaNotFoundPi3Exception(idProcessoAsLong);
                }

                var passiFirmaEntities = await this._dbContext.PassoDiFirmaEntities
                    .Join(this._dbContext.AnagraficaEventiEntities, passo => passo.TIPO_EVENTO, evento => evento.ID_EVENTO, (passo, evento) => new { passo, evento })
                    .GroupJoin(this._dbContext.CorrGlobaliEntities, j => j.passo.ID_RUOLO_COINVOLTO, ruolo => ruolo.ID_GRUPPO, (j, ruolo) => new { j.passo, j.evento, ruoloCoinvolto = ruolo })
                    .SelectMany(j => j.ruoloCoinvolto.DefaultIfEmpty(), (j, ruolo) => new { j.passo, j.evento, ruoloCoinvolto = ruolo })
                    .GroupJoin(this._dbContext.CorrGlobaliEntities, j => j.passo.ID_UTENTE_COINVOLTO, utente => utente.ID_PEOPLE, (j, utente) => new { j.passo, j.evento, j.ruoloCoinvolto, utenteCoinvolto = utente })
                    .SelectMany(j => j.utenteCoinvolto.DefaultIfEmpty(), (j, utente) => new { j.passo, j.evento, j.ruoloCoinvolto, utenteCoinvolto = utente })
                    .Where(j => j.passo.ID_PROCESSO == idProcessoAsLong)
                    .OrderBy(j => j.passo.NUMERO_SEQUENZA)
                    .Select(j => new PassoFirmaDiProcessoEntity()
                    {
                        PassoFirma = new PassoDiFirmaEntity()
                        {
                            ID_PASSO = j.passo.ID_PASSO,
                            ID_PROCESSO = j.passo.ID_PROCESSO,
                            NUMERO_SEQUENZA = j.passo.NUMERO_SEQUENZA,
                            TIPO_FIRMA = j.passo.TIPO_FIRMA,
                            TIPO_EVENTO = j.passo.TIPO_EVENTO,
                            NOTE = j.passo.NOTE,
                            SCADENZA = j.passo.SCADENZA,
                            ELIMINATO = j.passo.ELIMINATO,
                            TICK = j.passo.TICK,
                            ID_TIPO_RUOLO_COINVOLTO = j.passo.ID_TIPO_RUOLO_COINVOLTO,
                            ID_RUOLO_COINVOLTO = j.passo.ID_RUOLO_COINVOLTO,
                            ID_UTENTE_COINVOLTO = j.passo.ID_UTENTE_COINVOLTO,
                            ID_AOO = j.passo.ID_AOO,
                            ID_RF = j.passo.ID_RF,
                            ID_MAIL_REGISTRO = j.passo.ID_MAIL_REGISTRO,
                            CHA_AUTOMATICO = j.passo.CHA_AUTOMATICO,
                            CHA_FACOLTATIVO = j.passo.CHA_FACOLTATIVO,
                            ID_TIPOLOGIA = j.passo.ID_TIPOLOGIA,
                            ID_STATO_DIAGRAMMA = j.passo.ID_STATO_DIAGRAMMA,
                            VAR_POS_SEGNATURA = j.passo.VAR_POS_SEGNATURA,
                            CHA_POS_SEGNATURA = j.passo.CHA_POS_SEGNATURA
                        },
                        Evento = new AnagraficaEventiEntity()
                        {
                            ID_EVENTO = j.evento.ID_EVENTO,
                            VAR_COD_AZIONE = j.evento.VAR_COD_AZIONE,
                            CHA_TIPO_EVENTO = j.evento.CHA_TIPO_EVENTO,
                            DESCRIZIONE = j.evento.DESCRIZIONE,
                            GRUPPO = j.evento.GRUPPO,
                            CHA_AUTOMATICO = j.evento.CHA_AUTOMATICO
                        },
                        RuoloCoinvolto = new RuoloCoinvoltoEntity()
                        {
                            ID_GROUP = j.ruoloCoinvolto.ID_GRUPPO,
                            GROUP_ID = j.ruoloCoinvolto.VAR_COD_RUBRICA,
                            GROUP_DESCRIPTION = j.ruoloCoinvolto.VAR_DESC_CORR,
                            ID_CORR_GLOBALI = j.ruoloCoinvolto.SYSTEM_ID
                        },
                        UtenteCoinvolto = new UtenteCoinvoltoEntity()
                        {
                            ID_USER = j.utenteCoinvolto.ID_PEOPLE,
                            USER_ID = j.utenteCoinvolto.VAR_COD_RUBRICA,
                            USER_DESCRIPTION = j.utenteCoinvolto.VAR_DESC_CORR,
                            ID_CORR_GLOBALI = j.utenteCoinvolto.SYSTEM_ID
                        }
                    })
                    .AsNoTracking()
                    .ToListAsync();


                output = this._mapper.Map<ProcessoFirma>(processoEntity);
                if (passiFirmaEntities != null && passiFirmaEntities.Count > 0)
                {
                    output.passi = this._mapper.Map<PassoFirma[]>(passiFirmaEntities).ToList();
                    output.passi.ForEach(async p =>
                    {
                        p.idEventiDaNotificare = await this._dbContext.PassoEventoEntities.AsNoTracking()
                        .Join(this._dbContext.AnagraficaEventiEntities.AsNoTracking(), passo => passo.ID_EVENTO, evento => evento.ID_EVENTO, (passo, evento) => new { passo.ID_PASSO, evento.GRUPPO })
                        .Where(j => j.ID_PASSO == p.idPasso.AsLong())
                        .Select(j => j.GRUPPO)
                        .Distinct()
                        .ToListAsync();

                        if (!string.IsNullOrEmpty(p.ruoloCoinvolto.systemId))
                        {
                            p.ruoloCoinvolto.funzioni = (await this._mediator.Send(new GetFunzioniRuoloCommand()
                            {
                                IdCorrGlobali = p.ruoloCoinvolto.systemId
                            })).Funzioni;
                        }
                    });

                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new()
            {
                Output = output
            };
        }

        #region Private Members

        protected readonly ILogger<GetProcessoDiFirmaCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SchemaProcessoFirmaEntity, ProcessoFirma>()
                     .ForMember(dest => dest.idProcesso, opt => opt.MapFrom(src => src.ID_PROCESSO))
                     .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.NOME))
                     .ForMember(dest => dest.IdStatoInterruzione, opt => opt.MapFrom(src => src.ID_STATO_INTERRUZIONE != null ? src.ID_STATO_INTERRUZIONE.ToString() : string.Empty))
                     .ForMember(dest => dest.isInvalidated, opt => opt.MapFrom(src => src.TICK == "1"))
                     .ForMember(dest => dest.IsProcessModel, opt => opt.MapFrom(src => src.CHA_MODELLO == "1"));

                cfg.CreateMap<AnagraficaEventiEntity, Evento>()
                    .ForMember(dest => dest.IdEvento, opt => opt.MapFrom(src => src.ID_EVENTO))
                    .ForMember(dest => dest.CodiceAzione, opt => opt.MapFrom(src => src.VAR_COD_AZIONE))
                    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.DESCRIZIONE))
                    .ForMember(dest => dest.TipoEvento, opt => opt.MapFrom(src => src.CHA_TIPO_EVENTO))
                    .ForMember(dest => dest.Gruppo, opt => opt.MapFrom(src => src.GRUPPO))
                    .ForMember(dest => dest.Automatico, opt => opt.MapFrom(src => src.CHA_AUTOMATICO == "1"));

                cfg.CreateMap<RuoloCoinvoltoEntity, Ruolo>()
                   .ForMember(dest => dest.idGruppo, opt => opt.MapFrom(src => src.ID_GROUP))
                   .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.GROUP_ID))
                   .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.GROUP_DESCRIPTION))
                   .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.ID_CORR_GLOBALI));

                cfg.CreateMap<UtenteCoinvoltoEntity, Utente>()
                   .ForMember(dest => dest.idPeople, opt => opt.MapFrom(src => src.ID_USER))
                   .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.USER_DESCRIPTION))
                   .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.USER_ID))
                   .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.ID_CORR_GLOBALI));

                cfg.CreateMap<PassoFirmaDiProcessoEntity, PassoFirma>()
                    .ForMember(dest => dest.idPasso, opt => opt.MapFrom(src => src.PassoFirma.ID_PASSO))
                    .ForMember(dest => dest.numeroSequenza, opt => opt.MapFrom(src => src.PassoFirma.NUMERO_SEQUENZA))
                    .ForMember(dest => dest.idProcesso, opt => opt.MapFrom(src => src.PassoFirma.ID_PROCESSO))
                    .ForMember(dest => dest.note, opt => opt.MapFrom(src => src.PassoFirma.NOTE))
                    .ForMember(dest => dest.Invalidated, opt => opt.MapFrom(src => src.PassoFirma.TICK))
                    .ForMember(dest => dest.IdAOO, opt => opt.MapFrom(src => src.PassoFirma.ID_AOO))
                    .ForMember(dest => dest.IdRF, opt => opt.MapFrom(src => src.PassoFirma.ID_RF))
                    .ForMember(dest => dest.IdMailRegistro, opt => opt.MapFrom(src => src.PassoFirma.ID_MAIL_REGISTRO))
                    .ForMember(dest => dest.IdTipologia, opt => opt.MapFrom(src => src.PassoFirma.ID_TIPOLOGIA))
                    .ForMember(dest => dest.IdStatoDiagramma, opt => opt.MapFrom(src => src.PassoFirma.ID_STATO_DIAGRAMMA))
                    .ForMember(dest => dest.IsAutomatico, opt => opt.MapFrom(src => src.PassoFirma.CHA_AUTOMATICO == "1"))
                    .ForMember(dest => dest.IsFacoltativo, opt => opt.MapFrom(src => src.PassoFirma.CHA_FACOLTATIVO == "1"))
                    .ForMember(dest => dest.ApplicaSegnaturaPermanente, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.PassoFirma.CHA_POS_SEGNATURA) ? "0" : src.PassoFirma.CHA_POS_SEGNATURA))
                    .ForMember(dest => dest.PosizioneSegnaturaPermanente, opt => opt.MapFrom(src => src.PassoFirma.VAR_POS_SEGNATURA))
                    .AfterMap((src, dest) =>
                    {
                        dest.IsModello = false;
                        if (!src.Evento.CHA_TIPO_EVENTO.Equals("W") && src.PassoFirma.ID_RUOLO_COINVOLTO == null)
                        {
                            dest.IsModello = true;
                        }
                        if (src.PassoFirma.CHA_AUTOMATICO == "1")
                        {

                            if (src.Evento.VAR_COD_AZIONE.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) ||
                                src.Evento.VAR_COD_AZIONE.Equals(Azione.DOCUMENTO_REPERTORIATO.ToString()) ||
                                src.Evento.VAR_COD_AZIONE.Equals(Azione.RECORD_PREDISPOSED.ToString()))
                            {
                                if (src.PassoFirma.ID_AOO == null || src.PassoFirma.ID_RF == null)
                                {
                                    dest.IsModello = true;
                                }
                                if (src.Evento.VAR_COD_AZIONE.Equals(Azione.DOCUMENTOSPEDISCI.ToString()) && src.PassoFirma.ID_MAIL_REGISTRO == null)
                                {
                                    dest.IsModello = true;
                                }
                            }
                            if (src.Evento.VAR_COD_AZIONE.Equals(Azione.DOC_CAMBIO_STATO.ToString()) && (src.PassoFirma.ID_TIPOLOGIA == null || src.PassoFirma.ID_STATO_DIAGRAMMA == null))
                            {
                                dest.IsModello = true;
                            }
                        }
                    });
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class PassoFirmaDiProcessoEntity
        {
            public PassoDiFirmaEntity PassoFirma { get; set; }
            public AnagraficaEventiEntity Evento { get; set; }
            public RuoloCoinvoltoEntity RuoloCoinvolto { get; set; }
            public UtenteCoinvoltoEntity? UtenteCoinvolto { get; set; }
        }

        protected class RuoloCoinvoltoEntity
        {
            public long? ID_GROUP { get; set; }
            public string? GROUP_ID { get; set; }
            public string? GROUP_DESCRIPTION { get; set; }
            public long? ID_CORR_GLOBALI { get; set; }
        }

        protected class UtenteCoinvoltoEntity
        {
            public long? ID_USER { get; set; }
            public string? USER_ID { get; set; }
            public string? USER_DESCRIPTION { get; set; }
            public long? ID_CORR_GLOBALI { get; set; }
        }

        #endregion


    }
}
