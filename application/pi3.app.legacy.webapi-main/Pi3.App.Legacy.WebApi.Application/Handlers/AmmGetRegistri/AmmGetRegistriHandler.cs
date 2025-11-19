// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.amministrazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.EnqueueServerPdfConversion;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmmGetRegistriRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetRegistri;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetRegistri
{
    public class AmmGetRegistriHandler : IRequestHandler<AmmGetRegistriRequest, AmmGetRegistriResult>
    {
        #region Public Members

        public AmmGetRegistriHandler(ILogger<AmmGetRegistriHandler> logger,
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

        public async Task<AmmGetRegistriResult> Handle(AmmGetRegistriRequest request, CancellationToken cancellationToken)
        {
            OrgRegistro[] output = null;

            try
            {
                var codAmministrazione = request.codiceAmministrazione.ToUpper();
                var idAmministrazione = await this._dbContext.AmministraEntities.AsNoTracking()
                    .Where(a => a.VAR_CODICE_AMM.ToUpper().Equals(codAmministrazione))
                    .Select(a => a.SYSTEM_ID)
                    .FirstOrDefaultAsync();

                var queryable = this._dbContext.RegistroEntities.AsNoTracking()
                    .Where(r => r.ID_AMM == idAmministrazione);
                if (!string.IsNullOrEmpty(request.chaRF))
                    queryable = queryable.Where(r => r.CHA_RF == request.chaRF);

                var registroEntities = await queryable
                    .OrderBy(r => r.VAR_CODICE)
                    .ToListAsync();

                 output = _mapper.Map<OrgRegistro[]>(registroEntities);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new AmmGetRegistriResult(output);
        }

        #endregion

        #region Private Members


        protected readonly ILogger<AmmGetRegistriHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegistroEntity, OrgRegistro>()
                    .ForMember(dest => dest.IDRegistro, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.Codice, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.VAR_DESC_REGISTRO))
                    .ForMember(dest => dest.IDAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.Stato, src => src.MapFrom(opt => opt.CHA_STATO))
                    .ForMember(dest => dest.flag_pregresso, src => src.MapFrom(opt => (opt.VAR_PREG == null || opt.VAR_PREG != "1") ? false : true))
                    .ForMember(dest => dest.anno_pregresso, src => src.MapFrom(opt => (opt.VAR_PREG == null || opt.VAR_PREG != "1") ? string.Empty : opt.ANNO_PREG))
                    .ForMember(dest => dest.invioRicevutaManuale, src => src.MapFrom(opt => opt.INVIO_RICEVUTA_MANUALE))
                    .ForMember(dest => dest.Sospeso, src => src.MapFrom(opt => opt.CHA_STATO == "S" || opt.CHA_DISABILITATO != null))
                    .ForMember(dest => dest.AperturaAutomatica, src => src.MapFrom(opt => opt.CHA_AUTOMATICO != "0"))
                    .ForMember(dest => dest.ID_PEOPLE_AOO, src => src.MapFrom(opt => opt.ID_PEOPLE_AOO))
                    .ForMember(dest => dest.ID_RUOLO_AOO, src => src.MapFrom(opt => opt.ID_RUOLO_AOO))
                    .ForMember(dest => dest.autoInterop, src => src.MapFrom(opt => opt.CHA_AUTO_INTEROP))
                    .ForMember(dest => dest.chaRF, src => src.MapFrom(opt => opt.CHA_RF))
                    .ForMember(dest => dest.idAOOCollegata, src => src.MapFrom(opt => opt.ID_AOO_COLLEGATA))
                    .ForMember(dest => dest.rfDisabled, src => src.MapFrom(opt => opt.CHA_DISABILITATO ?? string.Empty))
                    .ForMember(dest => dest.Diritto_Ruolo_AOO, src => src.MapFrom(opt => opt.DIRITTO_RUOLO_AOO))
                    .ForMember(dest => dest.idRuoloResp, src => src.MapFrom(opt => opt.ID_RUOLO_RESP != null ? opt.ID_RUOLO_RESP.ToString() : string.Empty))
                    .ForMember(dest => dest.idUtenteResp, src => src.MapFrom(opt => opt.ID_UTENTE_RESP != null ? opt.ID_UTENTE_RESP.ToString() : string.Empty))
                    .ForMember(dest => dest.codiceIpa, src => src.MapFrom(opt => opt.VAR_CODICE_IPA != null ? opt.VAR_CODICE_IPA.ToString() : string.Empty))
                    .AfterMap((src, dest) =>
                    {
                        dest.Mail = new OrgRegistro.MailRegistro()
                        {
                            Email = src.VAR_EMAIL_REGISTRO,
                            UserID = src.VAR_USER_MAIL,
                            Password = string.Empty,
                            ServerSMTP = src.VAR_SERVER_SMTP,
                            SMTPssl = src.CHA_SMTP_SSL,
                            POPssl = src.CHA_POP_SSL,
                            PortaSMTP = src.NUM_PORTA_SMTP != null ? Convert.ToInt32(src.NUM_PORTA_SMTP) : 0,
                            SMTPsslSTA = src.CHA_SMTP_SSL,
                            ServerPOP = src.VAR_SERVER_POP,
                            PortaPOP = src.NUM_PORTA_POP != null ? Convert.ToInt32(src.NUM_PORTA_POP) : 0,
                            UserSMTP = src.VAR_USER_SMTP,
                            PasswordSMTP = string.Empty,
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
        }
        #endregion
    }
}
