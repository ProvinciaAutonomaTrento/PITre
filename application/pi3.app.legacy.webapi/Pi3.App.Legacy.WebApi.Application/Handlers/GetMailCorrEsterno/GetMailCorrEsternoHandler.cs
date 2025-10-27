// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetMailCorrEsternoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetMailCorrEsterno;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetMailCorrEsterno
{
    public class GetMailCorrEsternoHandler : IRequestHandler<GetMailCorrEsternoRequest, GetMailCorrEsternoResult>
    {
        #region Public Members

        public GetMailCorrEsternoHandler(ILogger<GetMailCorrEsternoHandler> logger,
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

        public async Task<GetMailCorrEsternoResult> Handle(GetMailCorrEsternoRequest request, CancellationToken cancellationToken)
        {
            List<MailCorrispondente> output = new List<DocsPaVO.utente.MailCorrispondente>();

            try
            {
                var idCorrispondenteAsLong = request.idCorrispondente.AsLong();

                var mailCorrEsterniEntity = await this._dbContext.MailCorrEsterniEntities.AsNoTracking().Where(m => m.ID_CORR == idCorrispondenteAsLong).ToListAsync();

                if (mailCorrEsterniEntity != null)
                    output = this._mapper.Map<List<MailCorrispondente>>(mailCorrEsterniEntity);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new GetMailCorrEsternoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetMailCorrEsternoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MailCorrEsterniEntity, MailCorrispondente>()
                     .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.VAR_EMAIL))
                     .ForMember(dest => dest.Principale, opt => opt.MapFrom(src => src.VAR_PRINCIPALE ?? string.Empty))
                     .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.VAR_NOTE));
            });

            _mapper = configuration.CreateMapper();
            #endregion
        }
    } 
}
