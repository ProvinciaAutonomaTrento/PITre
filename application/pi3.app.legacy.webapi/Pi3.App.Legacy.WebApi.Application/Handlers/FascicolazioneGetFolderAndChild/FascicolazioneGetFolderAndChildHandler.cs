// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetFolderAndChild
{

    // Richiede libreria MediatR
    public class FascicolazioneGetFolderAndChildHandler : IRequestHandler<Application.Requests.FascicolazioneGetFolderAndChild, FascicolazioneGetFolderAndChildResult>
    {
        #region Public Members

        public FascicolazioneGetFolderAndChildHandler(ILogger<FascicolazioneGetFolderAndChildHandler> logger, IPi3DbContext dbContext, IMediator mediator, IClaimsPrincipalService claimsPrincipalService)
        {
            this._logger = logger; 
            this._dbContext = dbContext;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;

            this.InitializeMapper();
        }

        public async Task<FascicolazioneGetFolderAndChildResult> Handle(Application.Requests.FascicolazioneGetFolderAndChild request, CancellationToken cancellationToken)
        {
            long idAmm = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            long idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            long idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            if (!await _dbContext.HasSecurityRights(request.folder.idFascicolo, idUser.ToString(), idGroup.ToString()))
            {
                return new FascicolazioneGetFolderAndChildResult(null);
            }

            var folder = this._mapper.Map<DocsPaVO.fascicolazione.Folder>(await this._dbContext.ProjectEntities.
                FirstOrDefaultAsync(w => w.SYSTEM_ID == request.folder.systemID.AsLong()));

       
                var childs = await this.GetChildFolders(long.Parse(folder.systemID));

                if (childs.Length > 0)
                {
                    List<DocsPaVO.fascicolazione.Folder> childList = new List<DocsPaVO.fascicolazione.Folder>();
                    childList.AddRange(childs);
                    folder.childs = childList.ToArray();
                }
            


            return new FascicolazioneGetFolderAndChildResult(folder);

        }

        #endregion

        #region Private Members

        private async Task<DocsPaVO.fascicolazione.Folder[]> GetChildFolders(long idParent)
        {
            var childs = await this._dbContext.ProjectEntities.Where(f => f.ID_PARENT == idParent).Select(f => this._mapper.Map<DocsPaVO.fascicolazione.Folder>(f)).ToArrayAsync();

            foreach (var child in childs)
            {
                var nextChilds = await this.GetChildFolders(long.Parse(child.systemID));
                if (nextChilds.Length > 0)
                {
                    List<DocsPaVO.fascicolazione.Folder> nextChildList = new List<DocsPaVO.fascicolazione.Folder>();
                    nextChildList.AddRange(childs);
                    child.childs = nextChilds.ToArray();
                }
            }

            return childs;
        }

        protected readonly ILogger<FascicolazioneGetFolderAndChildHandler> _logger;
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
