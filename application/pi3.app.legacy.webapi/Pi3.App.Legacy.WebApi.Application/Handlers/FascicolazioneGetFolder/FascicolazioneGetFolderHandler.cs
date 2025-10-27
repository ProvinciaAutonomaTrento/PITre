// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.fascicolazione;
using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetFolder
{
    // Richiede libreria MediatR
    public class FascicolazioneGetFolderHandler : IRequestHandler<Application.Requests.FascicolazioneGetFolder, FascicolazioneGetFolderResult>
    {
        #region Public Members

        public FascicolazioneGetFolderHandler(ILogger<FascicolazioneGetFolderHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            InitializeMapper();
        }

        public async Task<FascicolazioneGetFolderResult> Handle(Application.Requests.FascicolazioneGetFolder request, CancellationToken cancellationToken)
        {
            DocsPaVO.fascicolazione.Folder objFolder = null;
            long idPeople = Convert.ToInt64(request.idPeople);
            long idGruppo = Convert.ToInt64(request.idGruppo);
            long systemIdFascicolo = Convert.ToInt64(request.fascicolo.systemID);

            try
            {
                bool checkSecurityDocumento = this._dbContext.SecurityEntities.Any(x => x.THING == systemIdFascicolo && (x.PERSONORGROUP == idPeople || x.PERSONORGROUP == idGruppo));

                var entities = await this._dbContext.ProjectEntities.AsNoTracking()
                    .Where(x => x.ID_FASCICOLO == systemIdFascicolo && checkSecurityDocumento)
                    .OrderBy(x => x.DESCRIPTION)
                    .ToListAsync();

                //Mapper per la folder
                var parentFolder = entities.FirstOrDefault(x => x.ID_PARENT == systemIdFascicolo);
                objFolder = _mapper.Map<DocsPaVO.fascicolazione.Folder>(parentFolder);

                long parentSystemId = Convert.ToInt64(objFolder.systemID);

                //Figli - con ricorsione
                objFolder.childs = this.GetFolderChildren(entities, parentSystemId);                

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new FascicolazioneGetFolderResult(objFolder);
        }

        private Folder[] GetFolderChildren(List<ProjectEntity> entities, long parentSystemId)
        {
            List<Folder> children = new List<Folder>();
            if(entities.Count != 0)
            {
                entities.Where(x => x.ID_PARENT == parentSystemId).OrderBy(x => x.DESCRIPTION, new AlphaNumericComparer()).ToList().ForEach(x =>
                {
                    Folder f = _mapper.Map<Folder>(x);
                    long parentSystemId = Convert.ToInt64(f.systemID);
                    f.childs = GetFolderChildren(entities, parentSystemId);
                    children.Add(f);
                });
            }
            return children.ToArray();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetFolderHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
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

        #endregion
    }

    public class AlphaNumericComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            int firstNumber, secondNumber;
            bool firstIsNumber = int.TryParse(x, out firstNumber);
            bool secondIsNumber = int.TryParse(y, out secondNumber);

            if (!firstIsNumber)
                return !secondIsNumber ? firstNumber.CompareTo(secondNumber) : -1;

            return !secondIsNumber ? 1 : x.CompareTo(y);
        }
    }
}
