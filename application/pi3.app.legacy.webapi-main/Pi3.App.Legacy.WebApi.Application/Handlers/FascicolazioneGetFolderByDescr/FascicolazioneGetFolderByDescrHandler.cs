// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetFolderByDescr
{

    // Richiede libreria MediatR
    public class FascicolazioneGetFolderByDescrHandler : IRequestHandler<Application.Requests.FascicolazioneGetFolderByDescr, FascicolazioneGetFolderByDescrResult>
    {
        #region Public Members

        public FascicolazioneGetFolderByDescrHandler(ILogger<FascicolazioneGetFolderByDescrHandler> logger, IPi3DbContext dbContext, IMediator mediator, IClaimsPrincipalService claimsPrincipalService)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;

            this.InitializeMapper();
        }

        public async Task<FascicolazioneGetFolderByDescrResult> Handle(Requests.FascicolazioneGetFolderByDescr request, CancellationToken cancellationToken)
        {
            var descFolder = request.descrFolder;
            

            long idAmm = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant); 
            long idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            long idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            if (!await _dbContext.HasSecurityRights(request.idFascicolo, idUser.ToString(), idGroup.ToString()))
            {
                return new FascicolazioneGetFolderByDescrResult(null);
            }

            var projectEntities = await this._dbContext.ProjectEntities.AsNoTracking()
                .Where(w => EF.Functions.Like(w.DESCRIPTION!.ToUpper(), $"%{descFolder.ToUpper()}%")
                                                && w.ID_FASCICOLO == request.idFascicolo.AsLong())
                .ToArrayAsync();

            var folder = this._mapper.Map<DocsPaVO.fascicolazione.Folder[]>(projectEntities);

            return new FascicolazioneGetFolderByDescrResult(folder);

        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetFolderByDescrHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProjectEntity, DocsPaVO.fascicolazione.Folder>()
                    .ForMember(dest => dest.systemID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.idFascicolo, src => src.MapFrom(opt => opt.ID_FASCICOLO))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.DESCRIPTION))
                    .ForMember(dest => dest.idParent, src => src.MapFrom(opt => opt.ID_PARENT))
                    .ForMember(dest => dest.livello, src => src.MapFrom(opt => opt.NUM_LIVELLO))
                    .ForMember(dest => dest.codicelivello, src => src.MapFrom(opt => opt.VAR_COD_LIV1))
                    .AfterMap((p, f) =>
                    {
                        f.childs = new DocsPaVO.fascicolazione.Folder[0];
                    });

            });

            this._mapper = configuration.CreateMapper();
        }

        #endregion
    }
}

