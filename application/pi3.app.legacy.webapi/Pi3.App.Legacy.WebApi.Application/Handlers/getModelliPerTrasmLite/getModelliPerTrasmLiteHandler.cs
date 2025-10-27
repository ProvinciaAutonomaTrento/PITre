// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.Modelli_Trasmissioni;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoInoltraDoc;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getModelliPerTrasmLiteRequest = Pi3.App.Legacy.WebApi.Application.Requests.getModelliPerTrasmLite;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getModelliPerTrasmLite
{

    public class getModelliPerTrasmLiteHandler : IRequestHandler<getModelliPerTrasmLiteRequest, getModelliPerTrasmLiteResult>
    {
        #region Public Members

        public getModelliPerTrasmLiteHandler(ILogger<getModelliPerTrasmLiteHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext
        )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<getModelliPerTrasmLiteResult> Handle(getModelliPerTrasmLiteRequest request, CancellationToken cancellationToken)
        {
            ModelloTrasmissione[] output = null;
            List<ModelloTrasmEntity> modelloTrasmEntities = new List<ModelloTrasmEntity>();
            var idTenant = request.idAmm.AsLong();
            var idPeople = request.idPeople.AsLong();
            var idCorrGlobali = request.idCorrGlobali.AsLong();

            try
            {
                var modelliTrasmQueryable = this._dbContext.ModelloTrasmEntities.AsNoTracking().Where(m => m.ID_AMM == idTenant && m.CHA_TIPO_OGGETTO == request.cha_tipo_oggetto);

                modelliTrasmQueryable = modelliTrasmQueryable.Where(m =>
                this._dbContext.ModelloMittDestEntities.AsNoTracking()
                .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(), d => d.ID_CORR_GLOBALI, c => c.SYSTEM_ID, (d, c) => new { d, c })
                .Count(j => j.d.ID_MODELLO == m.SYSTEM_ID && j.d.CHA_TIPO_URP == "R" && j.d.CHA_TIPO_MITT_DEST == "D" && (j.c.CHA_DISABLED_TRASM == "1" || j.c.DTA_FINE != null)
                    && j.c.CHA_TIPO_IE == "I" && j.c.ID_AMM == idTenant) == 0);

                if (request.registri != null && request.registri.Length != 0)
                {
                    List<long> idRegistri = new List<long>();
                    for (int i = 0; i < request.registri.Length; i++)
                    {
                        if(!string.IsNullOrEmpty(request.registri[i].systemId))
                            idRegistri.Add(request.registri[i].systemId.AsLong());
                    }
                       

                    if (request.AllReg)
                        idRegistri.Add(0);

                    modelliTrasmQueryable = modelliTrasmQueryable.Where(m => idRegistri.Contains(m.ID_REGISTRO ?? 0));
                }
                if(!string.IsNullOrEmpty(request.system_id))
                {
                    if (request.accessrights == "45")
                    {
                        modelliTrasmQueryable = modelliTrasmQueryable.Where(m => this._dbContext.ModelloMittDestEntities.AsNoTracking()
                        .Join(this._dbContext.RagioneTrasmissioneEntities.AsNoTracking(), d => d.ID_RAGIONE, r => r.SYSTEM_ID, (d, r) => new { d, r })
                        .First(j => j.d.ID_MODELLO == m.SYSTEM_ID && j.r.CHA_TIPO_DIRITTI != "R" && j.r.CHA_TIPO_DIRITTI != "C") == null
                        );
                    }
                }
                else
                { 
                    if(request.cha_tipo_oggetto == "D")
                    {
                        modelliTrasmQueryable = modelliTrasmQueryable.Where(m => this._dbContext.ModelloMittDestEntities.AsNoTracking().Count(d => d.ID_MODELLO == m.SYSTEM_ID && d.HIDE_DOC_VERSIONS == "1") == 0);
                    }
                }

                modelliTrasmQueryable = modelliTrasmQueryable.Where(m => !this._dbContext.ModelloMittDestEntities
                                .Join(this._dbContext.CorrGlobaliEntities, 
                                    m2 => m2.ID_CORR_GLOBALI, 
                                    c => c.SYSTEM_ID,
                                    (m2, c) => new { m2, c })
                                .Where(x => x.m2.ID_MODELLO == m.SYSTEM_ID && x.m2.CHA_TIPO_URP == "R" && x.m2.CHA_TIPO_MITT_DEST == "D" && x.c.CHA_DISABLED_TRASM == "1" 
                                        && x.c.CHA_TIPO_IE == "I" && x.c.ID_AMM == idTenant).Any());

                var modelliTrasmQueryable_1 = modelliTrasmQueryable.Where(m => m.SINGLE == "1" && !this._dbContext.AssDiagrammiEntities.AsNoTracking().Any(d => d.ID_MOD_TRASM == m.SYSTEM_ID));

                var modelliTrasmQueryable_2 = modelliTrasmQueryable.Join(this._dbContext.ModelloMittDestEntities, m => m.SYSTEM_ID, d => d.ID_MODELLO, (m, d) => new { m, d })
                    .Where(j => !this._dbContext.AssDiagrammiEntities.AsNoTracking().Any(d => d.ID_MOD_TRASM == j.m.SYSTEM_ID)
                            && (j.m.ID_PEOPLE == idPeople || j.m.ID_PEOPLE == null)
                            && (j.d.ID_CORR_GLOBALI == 0 || j.d.ID_CORR_GLOBALI == idCorrGlobali)
                            && j.d.CHA_TIPO_MITT_DEST == "M" && j.m.SINGLE == "0");

                modelloTrasmEntities.AddRange(await modelliTrasmQueryable_1.OrderBy(m => m.NOME).ToListAsync());

                modelloTrasmEntities.AddRange(await modelliTrasmQueryable_2.OrderBy(j => j.m.NOME).Select(j => j.m).ToListAsync());

                if (!string.IsNullOrEmpty(request.idTipoDoc))
                {
                    var modelliTrasmQueryable_3 = modelliTrasmQueryable.Where(m => m.SINGLE == "1");

                    var modelliTrasmQueryable_4 = modelliTrasmQueryable.Join(this._dbContext.ModelloMittDestEntities, m => m.SYSTEM_ID, d => d.ID_MODELLO, (m, d) => new { m, d })
                        .Where(j => (j.m.ID_PEOPLE == idPeople || j.m.ID_PEOPLE == null)
                           && (j.d.ID_CORR_GLOBALI == 0 || j.d.ID_CORR_GLOBALI == idCorrGlobali)
                           && j.d.CHA_TIPO_MITT_DEST == "M" && j.m.SINGLE == "0");

                    var idTipoDoc = request.idTipoDoc.AsLong();
                    long? idDiagramma = !string.IsNullOrEmpty(request.idDiagramma) ? request.idDiagramma.AsLong() : null;
                    long? idStato = !string.IsNullOrEmpty(request.idStato) ? request.idStato.AsLong() : null;
                    if (idDiagramma != null && idStato != null)
                    {
                        modelliTrasmQueryable_3 = modelliTrasmQueryable_3.Where(m => this._dbContext.AssDiagrammiEntities.AsNoTracking()
                        .Any(d => d.ID_MOD_TRASM == m.SYSTEM_ID && d.ID_TIPO_DOC == idTipoDoc && d.ID_DIAGRAMMA == idDiagramma && d.ID_STATO == idStato));

                        modelliTrasmQueryable_4 = modelliTrasmQueryable_4.Where(j => this._dbContext.AssDiagrammiEntities.AsNoTracking()
                        .Any(d => d.ID_MOD_TRASM == j.m.SYSTEM_ID && d.ID_TIPO_DOC == idTipoDoc && d.ID_DIAGRAMMA == idDiagramma && d.ID_STATO == idStato));
                    }
                    else
                    {
                        if(idDiagramma != null)
                        {
                            modelliTrasmQueryable_3 = modelliTrasmQueryable_3.Where(m => this._dbContext.AssDiagrammiEntities.AsNoTracking()
                                .Any(d => d.ID_MOD_TRASM == m.SYSTEM_ID && d.ID_TIPO_DOC == idTipoDoc && d.ID_DIAGRAMMA == idDiagramma));

                            modelliTrasmQueryable_4 = modelliTrasmQueryable_4.Where(j => this._dbContext.AssDiagrammiEntities.AsNoTracking()
                                .Any(d => d.ID_MOD_TRASM == j.m.SYSTEM_ID && d.ID_TIPO_DOC == idTipoDoc && d.ID_DIAGRAMMA == idDiagramma));
                        }
                        if(idStato != null)
                        {
                            modelliTrasmQueryable_3 = modelliTrasmQueryable_3.Where(m => this._dbContext.AssDiagrammiEntities.AsNoTracking()
                                .Any(d => d.ID_MOD_TRASM == m.SYSTEM_ID && d.ID_TIPO_DOC == idTipoDoc && d.ID_STATO == idStato));

                            modelliTrasmQueryable_4 = modelliTrasmQueryable_4.Where(j => this._dbContext.AssDiagrammiEntities.AsNoTracking()
                                .Any(d => d.ID_MOD_TRASM == j.m.SYSTEM_ID && d.ID_TIPO_DOC == idTipoDoc && d.ID_STATO == idStato));
                        }

                        if (idStato == null && idDiagramma == null)
                        {
                            modelliTrasmQueryable_3 = modelliTrasmQueryable_3.Where(m => this._dbContext.AssDiagrammiEntities.AsNoTracking()
                                .Any(d => d.ID_MOD_TRASM == m.SYSTEM_ID && d.ID_TIPO_DOC == idTipoDoc));

                            modelliTrasmQueryable_4 = modelliTrasmQueryable_4.Where(j => this._dbContext.AssDiagrammiEntities.AsNoTracking()
                                .Any(d => d.ID_MOD_TRASM == j.m.SYSTEM_ID && d.ID_TIPO_DOC == idTipoDoc));
                        }
                    }

                    modelloTrasmEntities.AddRange(await modelliTrasmQueryable_3.OrderBy(m => m.NOME).ToListAsync());

                    modelloTrasmEntities.AddRange(await modelliTrasmQueryable_4.OrderBy(j => j.m.NOME).Select(j => j.m ).ToListAsync());
                }

                
                 output = this._mapper.Map<ModelloTrasmissione[]>(modelloTrasmEntities);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                output = null;
            }

            return new getModelliPerTrasmLiteResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getModelliPerTrasmLiteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ModelloTrasmEntity, ModelloTrasmissione>()
                     .ForMember(dest => dest.SYSTEM_ID, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.NOME, opt => opt.MapFrom(src => src.NOME))
                     .ForMember(dest => dest.CODICE, opt => opt.MapFrom(src => "MT_" + src.SYSTEM_ID))
                     .ForMember(dest => dest.CEDE_DIRITTI, opt => opt.MapFrom(src => src.CHA_CEDE_DIRITTI))
                     .ForMember(dest => dest.ID_PEOPLE_NEW_OWNER, opt => opt.MapFrom(src => src.ID_PEOPLE_NEW_OWNER))
                     .ForMember(dest => dest.ID_GROUP_NEW_OWNER, opt => opt.MapFrom(src => src.ID_GROUP_NEW_OWNER))
                     .ForMember(dest => dest.NO_NOTIFY, opt => opt.MapFrom(src => src.NO_NOTIFY))
                     .ForMember(dest => dest.MANTIENI_LETTURA, opt => opt.MapFrom(src => src.CHA_MANTIENI_LETTURA))
                     .ForMember(dest => dest.MANTIENI_SCRITTURA, opt => opt.MapFrom(src => src.CHA_MANTIENI_SCRITTURA));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
