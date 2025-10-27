// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Pi3.App.RubricaComune.Infrastructure.EF.Entities;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace Pi3.App.RubricaComune.WebApi.Application.Queries.Corrispondenti.Search
{
    public class SearchHandler : IRequestHandler<SearchRequest, SearchResponse>
    {
        #region Public Members

        public SearchHandler(ILogger<SearchHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IRubricaComuneDbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this._mapper = this.InitializeMapper();

            this.InitializeDbFieldMapping();
        }


        public async Task<SearchResponse> Handle(SearchRequest request, CancellationToken cancellationToken)
        {
            var corrsFromIpa = new List<Corrispondente>();
            var queryable = this._dbContext.ElementiRubricaEntities.AsNoTracking().AsQueryable<ElementoRubricaEntity>();
            bool searchInIpa = false;
            Expression? filterExpression = null;

            int totaleCorrispondenti = 0;
            List<ElementoRubricaEntity> corrispondenti = new List<ElementoRubricaEntity>();

            var criteriRicerca = (request.CriteriRicerca ?? Array.Empty<CriterioRicerca>()).ToList();
            var rubricaEsternaField = criteriRicerca.FirstOrDefault(x => x.Campo.Equals(CampiRicercaEnum.RubricaEsterna));
            var soloRubEsternaField = criteriRicerca.FirstOrDefault(x => x.Campo.Equals(CampiRicercaEnum.SoloRubricaEsterna));
            var emailField = criteriRicerca.FirstOrDefault(x => x.Campo.Equals(CampiRicercaEnum.Email));
            var searchRubricaComune = soloRubEsternaField?.Valore != "1";

            if (emailField != null && !string.IsNullOrEmpty(emailField.Valore))
                queryable = queryable
                    .Where( e => this._dbContext.EmailEntities.AsNoTracking()
                        .Where(em => e.ID == em.IDELEMENTORUBRICA
                                && EF.Functions.Like(em.EMAIL.ToUpper(),$"%{emailField.Valore.ToUpper()}%"))
                    .Any());

            SearchIpaResponse searchIpaResponse = new();
            if (rubricaEsternaField != null)
            {
                switch (rubricaEsternaField.Valore)
                {
                    case "IPA":
                    default:
                        searchInIpa = true;
                        var req = new SearchIpaRequest()
                        {
                            CriteriRicerca = request.CriteriRicerca,
                            CriteriOrdinamento = request.CriteriOrdinamento,
                        };
                        searchIpaResponse = (await this._mediator.Send(req));
                        break;
                }
                criteriRicerca.Remove(rubricaEsternaField);
            }

            if (searchRubricaComune)
            {
                foreach (var criterioRicerca in criteriRicerca.Where(v => v.Valore != null && this._dbFields.ContainsKey(v.Campo)))
                {
                    if (string.IsNullOrEmpty(criterioRicerca.Valore))
                        continue;
                    var parameter = Expression.Parameter(typeof(ElementoRubricaEntity));

                    var property = Expression.Property(parameter, this._dbFields[criterioRicerca.Campo]);
                    var constant = Expression.Constant(criterioRicerca.ToSql());
                    var efConstant = Expression.Constant(EF.Functions);


                    var propertyToUpper = Expression.Call(property, "ToUpper", Type.EmptyTypes);
                    var comparison = Expression.Call(typeof(DbFunctionsExtensions).GetMethod("Like", new Type[] { typeof(DbFunctions), typeof(string), typeof(string) }), efConstant, propertyToUpper, constant);
                    filterExpression = filterExpression is null
                        ? comparison
                        : Expression.And(filterExpression, comparison);

                    var lambda = Expression.Lambda<Func<ElementoRubricaEntity, bool>>(filterExpression, parameter);
                    filterExpression = null;
                    queryable = queryable.Where(lambda);
                }
                queryable = queryable.Join(this._dbContext.UtentiEntities.AsNoTracking(), (e) => e.IDUTENTECREATORE, u => u.ID, (e, u) => e);
                totaleCorrispondenti = await queryable.CountAsync(cancellationToken: cancellationToken);
                if (request.Pagina == 0)
                {
                    corrispondenti = await queryable.ToListAsync(cancellationToken: cancellationToken);
                }
                else
                {
                    var skip = (request.Pagina * request.ElementiPerPagina) - request.ElementiPerPagina;
                    corrispondenti = await queryable.Skip(skip).Take(request.ElementiPerPagina).ToListAsync(cancellationToken: cancellationToken);
                }
            }
            List<Corrispondente?> corrs = new();
            foreach (var e in corrispondenti)
            {
                corrs.Add(await this.SetEmail(CreateCorrispondente(e)));
            }
            
            if (searchInIpa)
            {
                if(searchIpaResponse.Corrispondenti != null && searchIpaResponse.Corrispondenti.Count > 0)
                {
                    corrs.AddRange(searchIpaResponse.Corrispondenti.Where(c => c != null));
                    totaleCorrispondenti += searchIpaResponse.Corrispondenti.Count;
                }
            }

            if(soloRubEsternaField != null && soloRubEsternaField.Valore != null && soloRubEsternaField.Valore.Equals("1"))
            {
                if (searchIpaResponse.Corrispondenti != null && searchIpaResponse.Corrispondenti.Count > 0)
                {
                    corrs = searchIpaResponse.Corrispondenti.Where(c => c != null).ToList();
                    totaleCorrispondenti = searchIpaResponse.Corrispondenti.Count;
                }
                    
            }

            return new SearchResponse()
            {
                TotaleCorrispondenti = totaleCorrispondenti,
                TotalePagine = ((totaleCorrispondenti % request.ElementiPerPagina) > 0 ? (totaleCorrispondenti / request.ElementiPerPagina) + 1 : (totaleCorrispondenti / request.ElementiPerPagina)),
                Corrispondenti = corrs.AsReadOnly()
            };
            

            
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SearchHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IRubricaComuneDbContext _dbContext;
        protected IMapper _mapper;
        private readonly Dictionary<CampiRicercaEnum, string> _dbFields = new();

        private void InitializeDbFieldMapping()
        {
            _dbFields.Add(CampiRicercaEnum.Codice, "CODICE");
            _dbFields.Add(CampiRicercaEnum.Denominazione, "DESCRIZIONE");
            _dbFields.Add(CampiRicercaEnum.CodiceFiscale, "VAR_COD_FISC");
            _dbFields.Add(CampiRicercaEnum.PartitaIva, "VAR_COD_PI");
            _dbFields.Add(CampiRicercaEnum.Fax, "FAX");
            _dbFields.Add(CampiRicercaEnum.Citta, "CITTA");
            _dbFields.Add(CampiRicercaEnum.Cap, "CAP");
            _dbFields.Add(CampiRicercaEnum.Provincia, "PROVINCIA");
            _dbFields.Add(CampiRicercaEnum.Nazione, "NAZIONE");
            _dbFields.Add(CampiRicercaEnum.Indirizzo, "INDIRIZZO");
            _dbFields.Add(CampiRicercaEnum.Aoo, "AOO");
            _dbFields.Add(CampiRicercaEnum.Telefono, "TELEFONO");
            _dbFields.Add(CampiRicercaEnum.Url, "URL");
        }

        private async Task<Corrispondente?> SetEmail(Corrispondente? entity)
        {
            if (entity == null)
                return null;

            var ems = await this._dbContext.EmailEntities.AsNoTracking().Where(em => entity.Id != null && em.IDELEMENTORUBRICA == entity.Id.AsLong())
                .Select(em => new GetEmails.Email
                {
                    Indirizzo = em.EMAIL,
                    Note = em.NOTE,
                    Preferita = em.PREFERITA == 1
                }).ToListAsync();

            var pref = ems.FirstOrDefault(em => em.Preferita ?? false);

            if (pref != null)
                entity.Email = pref.Indirizzo;
            else
                entity.Email = string.Empty;

            entity.Emails = ems;


            bool amm = false;
            bool aoo = false;
            bool emails = false;
            bool urls = false;
            string canale = "";

            if (entity != null && !string.IsNullOrEmpty(entity.Amministrazione))
                amm = true;

            if (entity != null && !string.IsNullOrEmpty(entity.AOO))
                aoo = true;

            if (entity != null && !string.IsNullOrEmpty(entity.UrlApiInteroperabilita))
                urls = true;

            if (entity != null && entity.Emails != null && entity.Emails.Count > 0)
                emails = true;

            if (amm && aoo && urls)
                canale = "INTEROPERABILITA PITRE";
            else
                if (amm && aoo && emails && !urls)
                canale = "INTEROPERABILITA";
            else
                if (emails && !urls && !(amm && aoo))
                canale = "MAIL";
            else
                if (!emails && !urls && !(amm && aoo))
                canale = "LETTERA";

            if(entity != null)
                entity.Canale = canale;

            return entity;
        }

        private static Corrispondente CreateCorrispondente(ElementoRubricaEntity entity)
        {
            

            return new Corrispondente()
            {
                Id = entity.ID.ToString(),
                Codice = entity.CODICE,
                Denominazione = entity.DESCRIZIONE,
                UrlApiInteroperabilita = entity.URL,
                Pubblicato = entity.CHA_PUBBLICATO == "1",
                CodiceFiscale = entity.VAR_COD_FISC,
                PartitaIva = entity.VAR_COD_PI,
                DataCreazione = entity.DATACREAZIONE ?? DateTime.Now,
                DataUltimaModifica = entity.DATAULTIMAMODIFICA ?? DateTime.Now,
                AOO = entity.AOO,
                Amministrazione = entity.AMMINISTRAZIONE,
                Indirizzo = entity.INDIRIZZO,
                CAP = entity.CAP,
                Citta = entity.CITTA,
                Fax = entity.FAX,
                Nazione = entity.NAZIONE,
                Provincia = entity.PROVINCIA,
                Telefono = entity.TELEFONO,
                Tipo = entity.TIPOCORRISPONDENTE == "UO" ? Tipi.UnitaOrganizzativa : Tipi.RaggruppamentoFunzionale,

            };
        }

        protected IMapper InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                Corrispondente.CreateMapFromElementoRubricaEntity(cfg);
            });

            return configuration.CreateMapper();
        }

        #endregion
    }

}
