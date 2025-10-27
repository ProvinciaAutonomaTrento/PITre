// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.DesktopApps;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.GetDesktopApp;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getCanaleBySystemIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.getCanaleBySystemId;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getCanaleBySystemId
{
    public class getCanaleBySystemIdHandler : IRequestHandler<getCanaleBySystemIdRequest, getCanaleBySystemIdResult>
    {
        #region Public Members

        public getCanaleBySystemIdHandler(ILogger<getCanaleBySystemIdHandler> logger,
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

        public async Task<getCanaleBySystemIdResult> Handle(getCanaleBySystemIdRequest request, CancellationToken cancellationToken)
        {
            Canale output = new Canale();

            try
            {
                var systemIdAsLong = request.idDocumenttypes.AsLong();

                var documentTypeEntity = await this._dbContext.DocumentTypesEntities.AsNoTracking().FirstOrDefaultAsync(d => d.SYSTEM_ID == systemIdAsLong && d.DISABLED == null);

                if (documentTypeEntity != null)
                    output = this._mapper.Map<Canale>(documentTypeEntity);

            }
            catch (Exception ex)
            {
                output = null;
                this._logger.LogError(ex, null, null);
            }

            return new getCanaleBySystemIdResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getCanaleBySystemIdHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DocumentTypesEntity, Canale>()
                     .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.DESCRIPTION))
                     .ForMember(dest => dest.tipoCanale, opt => opt.MapFrom(src => src.CHA_TIPO_CANALE))
                     .ForMember(dest => dest.typeId, opt => opt.MapFrom(src => src.TYPE_ID));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
