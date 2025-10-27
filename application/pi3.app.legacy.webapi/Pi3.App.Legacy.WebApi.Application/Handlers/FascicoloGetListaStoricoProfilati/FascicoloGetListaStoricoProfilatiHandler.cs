// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.LibroFirma;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.RicercaLite;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.InsertProcessoDiFirma;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FascicoloGetListaStoricoProfilatiRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicoloGetListaStoricoProfilati;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicoloGetListaStoricoProfilati
{
    public class FascicoloGetListaStoricoProfilatiHandler : IRequestHandler<FascicoloGetListaStoricoProfilatiRequest, FascicoloGetListaStoricoProfilatiResult>
    {
        #region Public Members

        public FascicoloGetListaStoricoProfilatiHandler(ILogger<FascicoloGetListaStoricoProfilatiHandler> logger,
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

        public async Task<FascicoloGetListaStoricoProfilatiResult> Handle(FascicoloGetListaStoricoProfilatiRequest request, CancellationToken cancellationToken)
        {
            StoricoProfilati[] output = null;
            try
            {
                var idTemplate = request.id_tipo_fasc.AsLong();
                var idProject = request.idProject.AsLong();

                var profileStoEntity = await this._dbContext.ProfilFascStoEntities.AsNoTracking()
                   .Join(this._dbContext.PeopleEntities.AsNoTracking(), profil => profil.ID_PEOPLE, people => people.SYSTEM_ID, (profil, people) => new { profil, people })
                   .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(), j => j.profil.ID_RUOLO_IN_UO, ruolo => ruolo.SYSTEM_ID, (j, ruolo) => new { j.profil, j.people, ruolo })
                   .Join(this._dbContext.OggettiCustomFascEntities.AsNoTracking(), j => j.profil.ID_OGG_CUSTOM, ogg => ogg.SYSTEM_ID, (j, ogg) => new { j.profil, j.people, j.ruolo, ogg })
                   .Where(j => j.profil.ID_TEMPLATE == idTemplate && j.profil.ID_PROJECT == idProject)
                   .OrderByDescending(j => j.profil.DTA_MODIFICA)
                   .ThenBy(j => j.profil.ID_OGG_CUSTOM)
                   .Select(j => new ListaStoricoProfilatiFascicoloEntity()
                    { 
                        DTA_MODIFICA = j.profil.DTA_MODIFICA,
                        VAR_DESC_MODIFICA = j.profil.VAR_DESC_MODIFICA,
                        utente = new PeopleEntity()
                        {
                            SYSTEM_ID = j.people.SYSTEM_ID,
                            USER_ID = j.people.USER_ID,
                            FULL_NAME = j.people.FULL_NAME,
                            VAR_NOME = j.people.VAR_NOME,
                            VAR_COGNOME = j.people.VAR_COGNOME
                        },
                        ruolo = new CorrGlobaliEntity()
                        {
                            SYSTEM_ID = j.ruolo.SYSTEM_ID,
                            VAR_DESC_CORR = j.ruolo.VAR_DESC_CORR,
                            VAR_COD_RUBRICA = j.ruolo.VAR_COD_RUBRICA
                        },
                        oggetto = new OggettiCustomEntity()
                        {
                            SYSTEM_ID = j.ogg.SYSTEM_ID,
                            DESCRIZIONE = j.ogg.DESCRIZIONE
                        }
                    })
                   .ToListAsync();

                output = this._mapper.Map<StoricoProfilati[]>(profileStoEntity);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new FascicoloGetListaStoricoProfilatiResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicoloGetListaStoricoProfilatiHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ListaStoricoProfilatiFascicoloEntity, StoricoProfilati>()
                     .ForMember(dest => dest.dta_modifica, opt => opt.MapFrom(src => src.DTA_MODIFICA.AsDateTimeFormat()))
                     .ForMember(dest => dest.var_desc_modifica, opt => opt.MapFrom(src => src.VAR_DESC_MODIFICA));

                cfg.CreateMap<PeopleEntity, Utente>()
                    .ForMember(dest => dest.idPeople, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.FULL_NAME))
                    .ForMember(dest => dest.userId, opt => opt.MapFrom(src => src.USER_ID))
                    .ForMember(dest => dest.nome, opt => opt.MapFrom(src => src.VAR_NOME))
                    .ForMember(dest => dest.cognome, opt => opt.MapFrom(src => src.VAR_COGNOME));

                cfg.CreateMap<CorrGlobaliEntity, Ruolo>()
                   .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                   .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.VAR_DESC_CORR))
                   .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.VAR_COD_RUBRICA));

                cfg.CreateMap<OggettiCustomEntity, OggettoCustom>()
                   .ForMember(dest => dest.SYSTEM_ID, opt => opt.MapFrom(src => src.SYSTEM_ID))
                   .ForMember(dest => dest.DESCRIZIONE, opt => opt.MapFrom(src => src.DESCRIZIONE));
            });

            this._mapper = configuration.CreateMapper();
        }
        protected class ListaStoricoProfilatiFascicoloEntity
        {
            public DateTime? DTA_MODIFICA { get; set; }
            public PeopleEntity? utente { get; set; }
            public CorrGlobaliEntity? ruolo { get; set; }
            public OggettiCustomEntity? oggetto { get; set; }
            public string? VAR_DESC_MODIFICA { get; set; }
        }

        #endregion
    }
}
