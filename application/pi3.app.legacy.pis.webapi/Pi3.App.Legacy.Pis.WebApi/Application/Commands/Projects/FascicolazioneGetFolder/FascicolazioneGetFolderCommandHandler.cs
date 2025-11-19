// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.FascicolazioneGetFolder
{
    public class FascicolazioneGetFolderCommandHandler : IRequestHandler<FascicolazioneGetFolderCommand, FascicolazioneGetFolderCommandResponse>
    {
        public FascicolazioneGetFolderCommandHandler(ILogger<FascicolazioneGetFolderCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {

            this._logger = logger;
            this._mediator = mediator;
            this._dbContext = dbContext;
            InitializeMapper();

        }

        public async Task<FascicolazioneGetFolderCommandResponse> Handle(FascicolazioneGetFolderCommand request, CancellationToken cancellationToken)
        {
            DocsPaVO.fascicolazione.Folder objFolder = null;
            long idPeople = Convert.ToInt64(request.IdPeople);
            long idGruppo = Convert.ToInt64(request.IdGruppo);
            long systemIdFascicolo = Convert.ToInt64(request.Fascicolo.systemID);

            try
            {
                bool checkSecurityDocumento = this._dbContext.SecurityEntities.Any(x => x.THING == systemIdFascicolo && (x.PERSONORGROUP == idPeople || x.PERSONORGROUP == idGruppo));

                var entities = await this._dbContext.ProjectEntities.AsNoTracking()
                    .Where(x => x.ID_FASCICOLO == systemIdFascicolo && checkSecurityDocumento)
                    .OrderBy(x => x.DESCRIPTION)
                    .ToListAsync();

                var parentFolder = entities.FirstOrDefault(x => x.ID_PARENT == systemIdFascicolo);
                objFolder = _mapper.Map<DocsPaVO.fascicolazione.Folder>(parentFolder);

                long parentSystemId = Convert.ToInt64(objFolder.systemID);

                objFolder.childs = this.GetFolderChildren(entities, parentSystemId);

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new()
            {
                Output = objFolder
            };
        }

        #region Private Members
        protected readonly ILogger<FascicolazioneGetFolderCommandHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProjectEntity, DocsPaVO.fascicolazione.Folder>()
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.DESCRIPTION))
                    .ForMember(dest => dest.systemID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.idFascicolo, src => src.MapFrom(opt => opt.ID_FASCICOLO))
                    .ForMember(dest => dest.idParent, src => src.MapFrom(opt => opt.ID_PARENT))
                    .ForMember(dest => dest.livello, src => src.MapFrom(opt => opt.NUM_LIVELLO))
                    .ForMember(dest => dest.codicelivello, src => src.MapFrom(opt => opt.VAR_COD_LIV1));
            });

            _mapper = configuration.CreateMapper();
        }
        private DocsPaVO.fascicolazione.Folder[] GetFolderChildren(List<ProjectEntity> entities, long parentSystemId)
        {
            List<DocsPaVO.fascicolazione.Folder> children = new List<DocsPaVO.fascicolazione.Folder>();
            if (entities.Count != 0)
            {
                entities.Where(x => x.ID_PARENT == parentSystemId).ToList().ForEach(x =>
                {
                    DocsPaVO.fascicolazione.Folder f = _mapper.Map<DocsPaVO.fascicolazione.Folder>(x);
                    long parentSystemId = Convert.ToInt64(f.systemID);
                    f.childs = GetFolderChildren(entities, parentSystemId);
                    children.Add(f);
                });
            }
            return children.ToArray();
        }
        #endregion
    }
}
