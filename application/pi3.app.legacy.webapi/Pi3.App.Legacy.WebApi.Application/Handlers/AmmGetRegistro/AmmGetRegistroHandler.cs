// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
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
using AmmGetRegistroRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetRegistro;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetRegistro
{
    public class AmmGetRegistroHandler : IRequestHandler<AmmGetRegistroRequest, AmmGetRegistroResult>
    {
        #region Public Members

        public AmmGetRegistroHandler(ILogger<AmmGetRegistroHandler> logger,
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

        public async Task<AmmGetRegistroResult> Handle(AmmGetRegistroRequest request, CancellationToken cancellationToken)
        {
            OrgRegistro output = null;

            try
            {
                var idRegistroAsLong = request.idRegistro.AsLong();

                var registroEntity = await this._dbContext.RegistroEntities.AsNoTracking()
                    .Join(this._dbContext.AmministraEntities.AsNoTracking(),
                        r => r.ID_AMM,
                        a => a.SYSTEM_ID,
                        (r, a) => new { r, a })
                    .Where(j => j.r.SYSTEM_ID == idRegistroAsLong)
                    .Select(j => new RegistroAmmEntities()
                    {
                        ID_REGISTRO = j.r.SYSTEM_ID,
                        CODICE = j.r.VAR_CODICE,
                        DESCRIZIONE = j.r.VAR_DESC_REGISTRO,
                        ID_AMMINISTRAZIONE = j.r.ID_AMM,
                        CODICE_AMMINISTRAZIONE = j.a.VAR_CODICE_AMM,
                        EMAIL = j.r.VAR_EMAIL_REGISTRO,
                        USER_EMAIL = j.r.VAR_USER_MAIL,
                        PWD_EMAIL = j.r.VAR_PWD_MAIL,
                        SERVER_SMTP = j.r.VAR_SERVER_SMTP,
                        PORTA_SMTP = j.r.NUM_PORTA_SMTP,
                        SERVER_POP = j.r.VAR_SERVER_POP,
                        PORTA_POP = j.r.NUM_PORTA_POP,
                        APERTURA_AUTOMATICA = j.r.CHA_AUTOMATICO,
                        USER_SMTP = j.r.VAR_USER_SMTP,
                        PWD_SMTP = j.r.VAR_PWD_SMTP,
                        ID_PEOPLE_AOO = j.r.ID_PEOPLE_AOO,
                        ID_RUOLO_AOO = j.r.ID_RUOLO_AOO,
                        CHA_SMTP_SSL = j.r.CHA_SMTP_SSL,
                        CHA_POP_SSL = j.r.CHA_POP_SSL,
                        CHA_SMTP_STA = j.r.CHA_SMTP_STA,
                        CHA_AUTO_INTEROP = j.r.CHA_AUTO_INTEROP,
                        CHA_RF = j.r.CHA_RF,
                        ID_AOO_COLLEGATA = j.r.ID_AOO_COLLEGATA,
                        CHA_DISABILITATO = j.r.CHA_DISABILITATO,
                        DIRITTO_RUOLO_AOO = j.r.DIRITTO_RUOLO_AOO,
                        STATO = j.r.CHA_STATO,
                        ID_RUOLO_RESP = j.r.ID_RUOLO_RESP,
                        VAR_SERVER_IMAP = j.r.VAR_SERVER_IMAP,
                        NUM_PORTA_IMAP = j.r.NUM_PORTA_IMAP,
                        VAR_TIPO_CONNESSIONE = j.r.VAR_TIPO_CONNESSIONE,
                        VAR_INBOX_IMAP = j.r.VAR_INBOX_IMAP,
                        VAR_BOX_MAIL_ELABORATE = j.r.VAR_BOX_MAIL_ELABORATE,
                        VAR_MAIL_NON_ELABORATE = j.r.VAR_MAIL_NON_ELABORATE,
                        CHA_IMAP_SSL = j.r.CHA_IMAP_SSL,
                        VAR_SOLO_MAIL_PEC = j.r.VAR_SOLO_MAIL_PEC,
                        CHA_RICEVUTA_PEC = j.r.CHA_RICEVUTA_PEC,
                        INVIO_RICEVUTA_MANUALE = j.r.INVIO_RICEVUTA_MANUALE,
                        VAR_PREG = j.r.VAR_PREG,
                        ANNO_PREG = j.r.ANNO_PREG,
                        VAR_MAIL_RIC_PENDENTE = j.r.VAR_MAIL_RIC_PENDENTE,
                        ID_UTENTE_RESP = j.r.ID_UTENTE_RESP,
                        VAR_CODICE_IPA = j.r.VAR_CODICE_IPA
                    })
                    .FirstOrDefaultAsync();

                if(registroEntity != null)
                    output = this._mapper.Map<OrgRegistro>(registroEntity);

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new AmmGetRegistroResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmGetRegistroHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected class RegistroAmmEntities
        {
            public long? ID_REGISTRO { get; set; }
            public string? CODICE { get; set; }
            public string? DESCRIZIONE { get; set; }
            public long? ID_AMMINISTRAZIONE { get; set; }
            public string? CODICE_AMMINISTRAZIONE { get; set; }
            public string? EMAIL { get; set; }
            public string? USER_EMAIL { get; set; }
            public string? PWD_EMAIL { get; set; }
            public string? SERVER_SMTP { get; set; }
            public long? PORTA_SMTP { get; set; }
            public string? SERVER_POP { get; set; }
            public long? PORTA_POP { get; set; }
            public string? APERTURA_AUTOMATICA { get; set; }
            public string? USER_SMTP { get; set; }
            public string? PWD_SMTP { get; set; }
            public long? ID_PEOPLE_AOO { get; set; }
            public long? ID_RUOLO_AOO { get; set; }
            public string? CHA_SMTP_SSL { get; set; }
            public string? CHA_POP_SSL { get; set; }
            public string? CHA_SMTP_STA { get; set; }
            public string? CHA_AUTO_INTEROP { get; set; }
            public string? CHA_RF { get; set; }
            public long? ID_AOO_COLLEGATA { get; set; }
            public string? CHA_DISABILITATO { get; set; }
            public long? DIRITTO_RUOLO_AOO { get; set; }
            public string? STATO { get; set; }
            public long? ID_RUOLO_RESP { get; set; }
            public string? VAR_SERVER_IMAP { get; set; }
            public long? NUM_PORTA_IMAP { get; set; }
            public string? VAR_TIPO_CONNESSIONE { get; set; }
            public string? VAR_INBOX_IMAP { get; set; }
            public string? VAR_BOX_MAIL_ELABORATE { get; set; }
            public string? VAR_MAIL_NON_ELABORATE { get; set; }
            public string? CHA_IMAP_SSL { get; set; }
            public string? VAR_SOLO_MAIL_PEC { get; set; }
            public string? CHA_RICEVUTA_PEC { get; set; }
            public long? INVIO_RICEVUTA_MANUALE { get; set; }
            public string? VAR_PREG { get; set; }
            public string? ANNO_PREG { get; set; }
            public string? VAR_MAIL_RIC_PENDENTE { get; set; }
            public long? ID_UTENTE_RESP { get; set; }
            public string? VAR_CODICE_IPA { get; set; }
        }

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegistroAmmEntities, OrgRegistro>()
                    .ForMember(dest => dest.IDRegistro, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.Codice, src => src.MapFrom(opt => opt.CODICE))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.DESCRIZIONE))
                    .ForMember(dest => dest.IDAmministrazione, src => src.MapFrom(opt => opt.ID_AMMINISTRAZIONE))
                    .ForMember(dest => dest.CodiceAmministrazione, src => src.MapFrom(opt => opt.CODICE_AMMINISTRAZIONE))
                    .ForMember(dest => dest.Stato, src => src.MapFrom(opt => opt.STATO))
                    .ForMember(dest => dest.flag_pregresso, src => src.MapFrom(opt => opt.VAR_PREG == "1"))
                    .ForMember(dest => dest.anno_pregresso, src => src.MapFrom(opt => opt.VAR_PREG == "1" ? opt.ANNO_PREG : string.Empty))
                    .ForMember(dest => dest.invioRicevutaManuale, src => src.MapFrom(opt => opt.INVIO_RICEVUTA_MANUALE))
                    .ForMember(dest => dest.Sospeso, src => src.MapFrom(opt => opt.STATO == "S" || opt.CHA_DISABILITATO != null))
                    .ForMember(dest => dest.AperturaAutomatica, src => src.MapFrom(opt => opt.APERTURA_AUTOMATICA != "0"))
                    .ForMember(dest => dest.ID_PEOPLE_AOO, src => src.MapFrom(opt => opt.ID_PEOPLE_AOO))
                    .ForMember(dest => dest.ID_RUOLO_AOO, src => src.MapFrom(opt => opt.ID_RUOLO_AOO))
                    .ForMember(dest => dest.autoInterop, src => src.MapFrom(opt => opt.CHA_AUTO_INTEROP))
                    .ForMember(dest => dest.chaRF, src => src.MapFrom(opt => opt.CHA_RF))
                    .ForMember(dest => dest.idAOOCollegata, src => src.MapFrom(opt => opt.ID_AOO_COLLEGATA))
                    .ForMember(dest => dest.invioRicevutaManuale, src => src.MapFrom(opt => opt.INVIO_RICEVUTA_MANUALE))
                    .ForMember(dest => dest.rfDisabled, src => src.MapFrom(opt => opt.CHA_DISABILITATO ?? string.Empty))
                    .ForMember(dest => dest.Diritto_Ruolo_AOO, src => src.MapFrom(opt => opt.DIRITTO_RUOLO_AOO))
                    .ForMember(dest => dest.idRuoloResp, src => src.MapFrom(opt => opt.ID_RUOLO_RESP != null ? opt.ID_RUOLO_RESP.ToString() : string.Empty))
                    .ForMember(dest => dest.idUtenteResp, src => src.MapFrom(opt => opt.ID_UTENTE_RESP != null ? opt.ID_UTENTE_RESP.ToString() : string.Empty))
                    .ForMember(dest => dest.codiceIpa, src => src.MapFrom(opt => opt.VAR_CODICE_IPA != null ? opt.VAR_CODICE_IPA.ToString() : string.Empty))
                    .AfterMap((src, dest) =>
                    {
                        dest.Mail = new OrgRegistro.MailRegistro()
                        {
                            Email = src.EMAIL,
                            UserID = src.USER_EMAIL,
                            Password = Crypter.Decode(src.PWD_EMAIL, src.USER_EMAIL),
                            ServerSMTP = src.SERVER_SMTP,
                            SMTPssl = src.CHA_SMTP_SSL,
                            POPssl = src.CHA_POP_SSL,
                            PortaSMTP = src.PORTA_SMTP != null ? Convert.ToInt32(src.PORTA_SMTP) : 0,
                            SMTPsslSTA = src.CHA_SMTP_SSL,
                            ServerPOP = src.SERVER_POP,
                            PortaPOP = src.PORTA_POP != null ? Convert.ToInt32(src.PORTA_POP) : 0,
                            UserSMTP = src.USER_SMTP,
                            PasswordSMTP = Crypter.Decode(src.PWD_SMTP, src.USER_SMTP),
                            inbox = src.VAR_INBOX_IMAP,
                            serverImap = src.VAR_SERVER_IMAP,
                            portaIMAP = src.NUM_PORTA_IMAP != null ? Convert.ToInt32(src.NUM_PORTA_IMAP) : 0,
                            tipoPosta = src.VAR_TIPO_CONNESSIONE,
                            mailElaborate = src.VAR_BOX_MAIL_ELABORATE,
                            mailNonElaborate = src.VAR_MAIL_NON_ELABORATE,
                            IMAPssl = src.CHA_IMAP_SSL,
                            soloMailPec = src.VAR_SOLO_MAIL_PEC,
                            pecTipoRicevuta = src.CHA_RICEVUTA_PEC,
                            MailRicevutePendenti = src.VAR_MAIL_RIC_PENDENTE
                        };
                    });
            });

            _mapper = configuration.CreateMapper();

            #endregion
        }
    }
}
