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
using AmmGetMailRegistroRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetMailRegistro;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetMailRegistro
{

    public class AmmGetMailRegistroHandler : IRequestHandler<AmmGetMailRegistroRequest, AmmGetMailRegistroResult>
    {
        #region Public Members

        public AmmGetMailRegistroHandler(ILogger<AmmGetMailRegistroHandler> logger,
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

        public async Task<AmmGetMailRegistroResult> Handle(AmmGetMailRegistroRequest request, CancellationToken cancellationToken)
        {
            CasellaRegistro[] output = null;

            try
            {
                var idRegistroAsLong = request.idRegistro.AsLong();

                var mailRegistriEntities = await this._dbContext.MailRegistriEntities.AsNoTracking()
                    .Where(m => m.ID_REGISTRO == idRegistroAsLong && m.VAR_EMAIL_REGISTRO != null)
                    .OrderBy(m => m.VAR_PRINCIPALE)
                    .ToListAsync();

                if(mailRegistriEntities != null && mailRegistriEntities.Any())
                    output = this._mapper.Map<CasellaRegistro[]>(mailRegistriEntities);

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new AmmGetMailRegistroResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmGetMailRegistroHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MailRegistriEntity, CasellaRegistro>()
                    .ForMember(dest => dest.IdRegistro, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.EmailRegistro, src => src.MapFrom(opt => opt.VAR_EMAIL_REGISTRO))
                    .ForMember(dest => dest.UserMail, src => src.MapFrom(opt => opt.VAR_USER_MAIL))
                    .ForMember(dest => dest.PwdMail, src => src.MapFrom(opt => Crypter.Decode(opt.VAR_PWD_MAIL, opt.VAR_USER_MAIL)))
                    .ForMember(dest => dest.ServerSMTP, src => src.MapFrom(opt => opt.VAR_SERVER_SMTP))
                    .ForMember(dest => dest.SmtpSSL, src => src.MapFrom(opt => opt.CHA_SMTP_SSL ?? string.Empty))
                    .ForMember(dest => dest.PopSSL, src => src.MapFrom(opt => opt.CHA_POP_SSL ?? string.Empty))
                    .ForMember(dest => dest.PortaSMTP, src => src.MapFrom(opt => opt.NUM_PORTA_SMTP ?? 0))
                    .ForMember(dest => dest.SmtpSta, src => src.MapFrom(opt => opt.CHA_SMTP_STA ?? string.Empty))
                    .ForMember(dest => dest.ServerPOP, src => src.MapFrom(opt => opt.VAR_SERVER_POP))
                    .ForMember(dest => dest.PortaPOP, src => src.MapFrom(opt => opt.NUM_PORTA_POP ?? 0))
                    .ForMember(dest => dest.UserSMTP, src => src.MapFrom(opt => opt.VAR_USER_SMTP))
                    .ForMember(dest => dest.PwdSMTP, src => src.MapFrom(opt => Crypter.Decode(opt.VAR_PWD_SMTP, opt.VAR_USER_SMTP)))
                    .ForMember(dest => dest.IboxIMAP, src => src.MapFrom(opt => opt.VAR_INBOX_IMAP))
                    .ForMember(dest => dest.ServerIMAP, src => src.MapFrom(opt => opt.VAR_SERVER_IMAP))
                    .ForMember(dest => dest.PortaIMAP, src => src.MapFrom(opt => opt.NUM_PORTA_IMAP ?? 0))
                    .ForMember(dest => dest.TipoConnessione, src => src.MapFrom(opt => opt.VAR_TIPO_CONNESSIONE))
                    .ForMember(dest => dest.BoxMailElaborate, src => src.MapFrom(opt => opt.VAR_BOX_MAIL_ELABORATE))
                    .ForMember(dest => dest.MailNonElaborate, src => src.MapFrom(opt => opt.VAR_MAIL_NON_ELABORATE))
                    .ForMember(dest => dest.ImapSSL, src => src.MapFrom(opt => opt.CHA_IMAP_SSL))
                    .ForMember(dest => dest.SoloMailPEC, src => src.MapFrom(opt => opt.VAR_SOLO_MAIL_PEC))
                    .ForMember(dest => dest.RicevutaPEC, src => src.MapFrom(opt => opt.CHA_RICEVUTA_PEC ?? string.Empty))
                    .ForMember(dest => dest.Principale, src => src.MapFrom(opt => opt.VAR_PRINCIPALE ?? "0"))
                    .ForMember(dest => dest.Note, src => src.MapFrom(opt => opt.VAR_NOTE))
                    .ForMember(dest => dest.MailRicevutePendenti, src => src.MapFrom(opt => opt.VAR_MAIL_RIC_PENDENTE))
                    .ForMember(dest => dest.MessageSendMail, src => src.MapFrom(opt => opt.VAR_MESSAGE_SEND_MAIL))
                    .ForMember(dest => dest.OverwriteMessageAmm, src => src.MapFrom(opt => opt.CHA_OVERWRITE_MESSAGE_AMM == "1"))
                    .ForMember(dest => dest.Provider, src => src.MapFrom(opt => opt.PROVIDER_ID))
                    .ForMember(dest => dest.TenantId, src => src.MapFrom(opt => opt.MS_TENANT_ID))
                    .ForMember(dest => dest.ClientId, src => src.MapFrom(opt => opt.MS_CLIENT_ID))
                    .ForMember(dest => dest.ClientSecret, src => src.MapFrom(opt => opt.MS_CLIENT_SEC));
            });

            _mapper = configuration.CreateMapper();
            #endregion
        }
    }
}
