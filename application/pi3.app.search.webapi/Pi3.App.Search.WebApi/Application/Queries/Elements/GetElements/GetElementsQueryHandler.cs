// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Pi3.App.Search.WebApi.Application.Queries.Elements.GetElements;
using LuceneLib = Lucene;
using static Lucene.Net.Util.Fst.Util;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Extensions;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;

namespace Pi3.App.Search.WebApi.Application.Queries.Elements.GetElements
{
    public class GetElementsQueryHandler : IRequestHandler<GetElementsQuery, GetElementsQueryResults>
    {
        #region Public Members

        public GetElementsQueryHandler(
            ILogger<GetElementsQueryHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IOptions<LuceneIndexingOptions> options,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._options = options;
            this._configurationService = configurationService;
        }

        public async virtual Task<GetElementsQueryResults> Handle(GetElementsQuery request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);

            this._logger.LogInformation($"idTenant: {idTenant}, request: {JsonSerializer.Serialize<GetElementsQuery>(request)}");

            var results = new List<ResultsByElementType>();

            foreach (var type in request.Types)
            {
                if (type != TypesEnum.DocumentoAmministrativo)
                    throw new NotSupportedPi3Exception(ErrorDescriptions.ElementTypeNonSupportato, ErrorDescriptions.ResourceManager, type);

                DocumentiAmministrativiQueryModel queryModel = null;

                if (!string.IsNullOrWhiteSpace(request.Query))
                {
                    try
                    {
                        queryModel = JsonSerializer.Deserialize<DocumentiAmministrativiQueryModel>(Regex.Unescape(request.Query));
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(exception: ex, message: ErrorDescriptions.FormatoQueryNonSupportato);

                        throw new NotSupportedPi3Exception(ErrorDescriptions.FormatoQueryNonSupportato, ErrorDescriptions.ResourceManager);
                    }
                }
                else
                    queryModel = new DocumentiAmministrativiQueryModel();

                var betterIndex = this.FindBetterIndex(idTenant, queryModel);

                DateTime perfStartDate = DateTime.Now;

                var directory = await this.OpenDirectory(betterIndex, idTenant, queryModel);

                this._logger.LogInformation($"OpenDirectory TotalMilliseconds: {DateTime.Now.Subtract(perfStartDate).TotalMilliseconds}.");
                perfStartDate = DateTime.Now;

                var total = 0;
                var elements = new List<Application.Queries.Elements.GetElements.ElementResult>();

                if (DirectoryReader.IndexExists(directory))
                {
                    this._logger.LogInformation($"DirectoryReader.IndexExists TotalMilliseconds: {DateTime.Now.Subtract(perfStartDate).TotalMilliseconds}.");
                    perfStartDate = DateTime.Now;

                    this._logger.LogInformation("Indice esistente in directory.");

                    using (var analyzer = CreateAnalyzer())
                    {
                        this._logger.LogInformation($"CreateAnalyzer TotalMilliseconds: {DateTime.Now.Subtract(perfStartDate).TotalMilliseconds}.");
                        perfStartDate = DateTime.Now;

                        this._logger.LogInformation($"Create IndexWriterConfig TotalMilliseconds: {DateTime.Now.Subtract(perfStartDate).TotalMilliseconds}.");
                        perfStartDate = DateTime.Now;

                        using (IndexReader reader = DirectoryReader.Open(directory))
                        {
                            this._logger.LogInformation($"DirectoryReader.Open TotalMilliseconds: {DateTime.Now.Subtract(perfStartDate).TotalMilliseconds}.");
                            perfStartDate = DateTime.Now;

                            BooleanQuery aggregateQuery = new BooleanQuery();

                            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true);

                            // Filtri security, solamente se l'utente non dispone di privilegi di amministrazione
                            BooleanQuery secAggregateQuery = new BooleanQuery();
                            secAggregateQuery.MinimumNumberShouldMatch = 1;

                            secAggregateQuery.Add(new TermQuery(new Term("Permissions.IdUser", idUser)), Occur.SHOULD);
                            secAggregateQuery.Add(new TermQuery(new Term("Permissions.IdGroup", idGroup)), Occur.SHOULD);
                            secAggregateQuery.Add(new TermQuery(new Term("Permissions.IdOwnerGroup", idGroup)), Occur.SHOULD);

                            aggregateQuery.Add(secAggregateQuery, Occur.MUST);

                            if (betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate &&
                                betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate_Profile &&
                                (queryModel?.CreationYear.HasValue ?? false))
                            {
                                aggregateQuery.Add(LuceneLib.Net.Search.NumericRangeQuery.NewInt32Range("CreationYear", queryModel.CreationYear.Value, queryModel.CreationYear.Value, true, true), Occur.MUST);
                            }

                            if (betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate &&
                                betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate_Profile &&
                                !string.IsNullOrWhiteSpace(queryModel?.IdProfile ?? null))
                            {
                                aggregateQuery.Add(new TermQuery(new Term("Profiles.Id", queryModel.IdProfile)), Occur.MUST);
                            }

                            if (betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro &&
                                (queryModel?.AnnoRegistrazione.HasValue ?? false))
                            {
                                var annoRegistrazioneQuery = new BooleanQuery();
                                annoRegistrazioneQuery.MinimumNumberShouldMatch = 1;

                                annoRegistrazioneQuery.Add(LuceneLib.Net.Search.NumericRangeQuery.NewInt32Range("DatiRegistrazione.AnnoProtocollazione", queryModel.AnnoRegistrazione.Value, queryModel.AnnoRegistrazione.Value, true, true), Occur.SHOULD);
                                annoRegistrazioneQuery.Add(LuceneLib.Net.Search.NumericRangeQuery.NewInt32Range("DatiRegistrazione.AnnoRegistrazione", queryModel.AnnoRegistrazione.Value, queryModel.AnnoRegistrazione.Value, true, true), Occur.SHOULD);

                                aggregateQuery.Add(annoRegistrazioneQuery, Occur.MUST);
                            }

                            if (betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro &&
                                !string.IsNullOrWhiteSpace(queryModel?.IdRegistro ?? null))
                            {
                                aggregateQuery.Add(new TermQuery(new Term("DatiRegistrazione.IdRegistro", queryModel.IdRegistro)), Occur.MUST);
                            }

                            Sort sortBy = null;

                            if (!string.IsNullOrWhiteSpace(queryModel?.ContainsText ?? null))
                            {
                                var escapedPredicate = QueryParser.Escape(queryModel.ContainsText.Trim());

                                switch (queryModel.ContainsTextMode)
                                {
                                    case ContainsTextModesEnum.AlmostOne:
                                        // La ricerca richiesta non è per frase esatta,
                                        // aggiunge l'asterisco alla fine di ogni parola,
                                        // il QueryParser traduce le singole query in TermQuery

                                        escapedPredicate = String.Join(" ", escapedPredicate
                                            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                            .Select(x => x + "*"));

                                        break;
                                    case ContainsTextModesEnum.MatchExact:
                                        // Se la ricerca richiesta è per frase esatta, 
                                        // racchiude il testo da ricercare in doppi apici
                                        // in modo tale che QueryParser traduca le singole query in PhraseQuery
                                        int index = 0, count = 1;
                                        while (index <= escapedPredicate.Length - 1)
                                        {
                                            if (escapedPredicate[index] == ' ' || escapedPredicate[index] == '\n' || escapedPredicate[index] == '\t')
                                            {
                                                count++;
                                            }
                                            index++;
                                        }

                                        if (count > 1)
                                            escapedPredicate = $"\"{escapedPredicate}\"";

                                        break;
                                }

                                this._logger.LogInformation($"escapedPredicate: '{escapedPredicate}'.");

                                var fields = MultiFields.GetFields(reader).Except(this.ExcludeFields).Select(f => f).ToArray();

                                var queryParser = new MultiFieldQueryParser(
                                                Version,
                                                fields,
                                                analyzer);

                                queryParser.AutoGeneratePhraseQueries = true;

                                var inputQuery = queryParser.Parse(escapedPredicate);

                                aggregateQuery.Add(inputQuery, Occur.MUST);

                                // Ordinamento per rilevanza rispetto al testo inserito
                                sortBy = new Sort(new SortField("score", SortFieldType.SCORE));
                            }
                            else
                            {
                                // Ordinamento predefinito per data creazione
                                sortBy = new Sort(new SortField("CreationDate", SortFieldType.STRING, true));
                            }

                            this._logger.LogInformation($"aggregateQuery: '{aggregateQuery.ToString()}'.");

                            var searcher = new IndexSearcher(reader);

                            DateTime startDate = DateTime.Now;

                            var topDocs = searcher.Search(aggregateQuery, (request.Skip ?? 0) + (request.Take ?? 50), sortBy);

                            this._logger.LogInformation($"topDocs: '{topDocs.ScoreDocs.Length}' - TotalMilliseconds: {DateTime.Now.Subtract(startDate).TotalMilliseconds}.");

                            for (int i = (request.Skip ?? 0) + (request.Take ?? 50) - (request.Take ?? 50); i < topDocs.ScoreDocs.Length; i++)
                            {
                                var sd = topDocs.ScoreDocs[i];

                                var doc = searcher.Doc(sd.Doc);

                                total = topDocs.TotalHits;

                                var asJson = doc.Get("DocumentoAmministrativoAsJson");

                                var deserialized = JsonSerializer.Deserialize<JsonElement>(asJson);

                                elements.Add(new Application.Queries.Elements.GetElements.ElementResult()
                                {
                                    Score = (!float.IsNaN(sd.Score) ? sd.Score : 0),
                                    Id = deserialized.GetProperty("Id").GetString(),
                                    IdTenant = deserialized.GetProperty("IdTenant").GetString(),
                                    TypeName = deserialized.GetProperty("TypeName").GetString(),
                                    Name = deserialized.GetProperty("Name").Deserialize<TextValue>(),
                                    Description = deserialized.GetProperty("OggettoDelDocumento").GetProperty("Descrizione").Deserialize<TextValue>(),
                                    CreationDate = deserialized.GetProperty("CreationDate").GetDateTime()
                                });
                            }
                        }
                    }
                }
                else
                {
                    this._logger.LogInformation("Indice non esistente.");
                }

                results.Add(new ResultsByElementType()
                {
                    Type = TypesEnum.DocumentoAmministrativo,
                    Total = total,
                    Elements = elements.AsReadOnly()
                });
            }

            return new GetElementsQueryResults()
            {
                Results = results.AsReadOnly()
            };
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetElementsQueryHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IOptions<LuceneIndexingOptions> _options;
        protected readonly IConfigurationService _configurationService;

        protected enum ContainsTextModesEnum
        {
            AlmostOne,
            MatchExact,
        }

        protected class DocumentiAmministrativiQueryModel : ValueObject
        {
            public int? CreationYear { get; init; }

            public string? IdProfile { get; init; }

            public int? AnnoRegistrazione { get; init; }

            public string? IdRegistro { get; init; }

            public string? ContainsText { get; init; }

            public ContainsTextModesEnum? ContainsTextMode { get; init; } = ContainsTextModesEnum.AlmostOne;
        }

        protected virtual LuceneVersion Version
        {
            get
            {
                return LuceneVersion.LUCENE_48;
            }
        }

        protected virtual Analyzer CreateAnalyzer()
        {
            return new StandardAnalyzer(this.Version);
        }

        protected virtual IReadOnlyList<string> ExcludeFields
        {
            get
            {
                return new List<string>()
                {
                    "IdAsNumeric",
                    "IdTenant",
                    "TypeName",
                    "Profiles.Id",
                    "Profiles.Fields.Id",
                    "Permissions.IdMember",
                    "Permissions.IdUser",
                    "Permissions.IdGroup",
                    "Permissions.PermissionType",
                    "Permissions.RightType",
                    "RelatedElements.Id",
                    "RelatedElements.AsParent",
                    "Classifications.Id",
                    "Versions.Id",
                    "Versions.IdBlob",
                    "Versions.FileSize",
                    "IdParentDocument",
                    "Annullato",
                    "DatiRegistrazione.IsRegistrato",
                    "DatiRegistrazione.TipologiaFlusso",
                    "DatiRegistrazione.TipoRegistro",
                    "DatiRegistrazione.IdRegistro",
                    "Riservato",
                    "Aggregazioni.Tipologia",
                    "Aggregazioni.Id",
                    "IdDoc.Identiticativo",
                    "IdDoc.ImprontaCrittograficaDelDocumento",
                    "IdIdentificativoDocumentoPrimario.Identiticativo",
                    "IdIdentificativoDocumentoPrimario.ImprontaCrittograficaDelDocumento",
                    "Allegati.IdDoc.Identiticativo",
                    "Allegati.IdDoc.ImprontaCrittograficaDelDocumento"
                };
            }
        }

        protected virtual DocumentoAmministrativoIndexTypesEnum FindBetterIndex(string idTenant, DocumentiAmministrativiQueryModel? queryModel = null)
        {
            DocumentoAmministrativoIndexTypesEnum? indexType = null;

            if (queryModel.CreationYear.HasValue && !string.IsNullOrWhiteSpace(queryModel.IdProfile))
            {
                indexType = this._options.Value.DocumentoAmministrativoIndexingOptions.IndexTypes.FirstOrDefault(p => p == DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate_Profile);
            }
            else if (queryModel.AnnoRegistrazione.HasValue && !string.IsNullOrWhiteSpace(queryModel.IdRegistro))
            {
                indexType = this._options.Value.DocumentoAmministrativoIndexingOptions.IndexTypes.FirstOrDefault(p => p == DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro);
            }
            else if (queryModel.CreationYear.HasValue)
            {
                indexType = this._options.Value.DocumentoAmministrativoIndexingOptions.IndexTypes.FirstOrDefault(p => p == DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate);
            }

            if (!indexType.HasValue)
            {
                indexType = this._options.Value.DocumentoAmministrativoIndexingOptions.IndexTypes.FirstOrDefault(p => p == DocumentoAmministrativoIndexTypesEnum.Tenant);

                if (indexType == null)
                    throw new OpzioneIndiceNotFoundPi3Exception(idTenant);
            }

            return indexType.Value;
        }

        protected virtual async Task<LuceneLib.Net.Store.Directory> OpenDirectory(DocumentoAmministrativoIndexTypesEnum index, string idTenant, DocumentiAmministrativiQueryModel? queryModel = null)
        {
            string path = string.Empty;
            var rootPath = Path.Combine(await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true), "Indexes");

            switch (index)
            {
                case DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate_Profile:
                    {
                        path = Path.Combine(rootPath, nameof(DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate_Profile), idTenant, queryModel.CreationYear.Value.ToString(), queryModel.IdProfile);

                        break;
                    }
                case DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate:
                    {
                        path = Path.Combine(rootPath, nameof(DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate), idTenant, queryModel.CreationYear.Value.ToString());

                        break;
                    }
                case DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro:
                    {
                        path = Path.Combine(rootPath, nameof(DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro), idTenant, queryModel.AnnoRegistrazione.Value.ToString(), queryModel.IdRegistro);

                        break;
                    }
                case DocumentoAmministrativoIndexTypesEnum.Tenant:
                    {
                        path = Path.Combine(rootPath, nameof(DocumentoAmministrativoIndexTypesEnum.Tenant), idTenant);

                        break;
                    }
            }

            path = path.PathAsUnixPath();

            return FSDirectory.Open(path);
        }

        #endregion
    }
}
