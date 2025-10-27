// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
#if false 
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Term = Lucene.Net.Index.Term;
using TermQuery = Lucene.Net.Search.TermQuery;
using LuceneLib = Lucene;
#else 
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Clients.Elasticsearch.Core.Search;
#endif
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using pi3.Core.Contracts.DocumentoAmministrativo;
using pi3.Core.Contracts.DocumentoAmministrativo.Query;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using System.Text.Json;
using System.Text.RegularExpressions;
using IndexManagement = Elastic.Clients.Elasticsearch.IndexManagement;
using static System.Net.Mime.MediaTypeNames;
using Pi3.Core.Contracts.DocumentoAmministrativo.Index;
using Lucene.Net.Search;
using Doc.Search.DocumentoAmministrativo;
using DocumentoAmministrativoIndexTypesEnum = Doc.Search.DocumentoAmministrativo.DocumentoAmministrativoIndexTypesEnum;
using ReIndex = Elastic.Clients.Elasticsearch.Core.Reindex;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using System.Runtime.InteropServices;
using Elastic.Clients.Elasticsearch.Core.Reindex;

namespace Pi3.Infrastructure.Elastic;

public class ElasticQueryService : ISearchService
{
    private readonly ILogger<ElasticQueryService> _logger;
    private readonly IClaimsPrincipalService _claimsPrincipalService;
    private readonly IOptions<IndexingOptions> _options;
    private readonly IConfigurationService _configurationService;
    private readonly ElasticsearchClient _client;

    public ElasticQueryService(
        ILogger<ElasticQueryService> logger,
        IClaimsPrincipalService claimsPrincipalService,
        IConfigurationService configurationService, 
        IOptions<IndexingOptions> options)
    {
        this._logger = logger;
        this._claimsPrincipalService = claimsPrincipalService;
        this._options = options;
        this._configurationService = configurationService;
        var settings = new ElasticsearchClientSettings(new Uri(options.Value.ElasticSearchUri));

        this._client = new ElasticsearchClient(settings);
    }

    public async Task<GetElementsQueryResults> GetElements(GetElementsQuery request)
    {
        var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
        var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);
        var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true);

        //await CreateNewIndexWithMappingAsync();

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
                queryModel = new DocumentiAmministrativiQueryModel() { CreationYear = 2021};// fino a qui ok

            //var client = new ElasticsearchClient();
            var indices = _client.Indices.GetAsync(new IndexManagement.GetIndexRequest(Indices.All));

            // questo dovrebbe essere ok
            var betterIndex = await this.FindBetterIndex(idTenant, queryModel);

            idGroup = "139226";

            var response = await _client.SearchAsync<DocumentoAmministrativoEntity>(s => s
                .Index(betterIndex.ToString().ToLower())
                .From(0)
                .Size(10)
                .Query(q =>
                    SetQuery(q, idUser, idGroup, queryModel, betterIndex)
                )
            );

            if (response.IsValidResponse)
            {
                var tweet = response.Documents.FirstOrDefault();
            }


            DateTime perfStartDate = DateTime.Now;

            //LuceneLib.Net.Store.Directory directory = null; // await this.OpenDirectory(betterIndex, idTenant, queryModel);

            this._logger.LogInformation($"OpenDirectory TotalMilliseconds: {DateTime.Now.Subtract(perfStartDate).TotalMilliseconds}.");
            perfStartDate = DateTime.Now;

            var total = 0;
            var elements = new List<ElementResult>();

            var indici = await _client.Indices.GetAsync(new IndexManagement.GetIndexRequest(Indices.All));
            var indexExists = indici.Indices.ContainsKey("documentoamministrativo");

            if (indexExists)
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

                    //using (IndexReader reader = DirectoryReader.Open(directory))
                    //{
                        this._logger.LogInformation($"DirectoryReader.Open TotalMilliseconds: {DateTime.Now.Subtract(perfStartDate).TotalMilliseconds}.");
                        perfStartDate = DateTime.Now;

                        BooleanQuery aggregateQuery = new BooleanQuery();


                        // Filtri security, solamente se l'utente non dispone di privilegi di amministrazione
                        BooleanQuery secAggregateQuery = new BooleanQuery();
                        secAggregateQuery.MinimumNumberShouldMatch = 1;

                        // in teoria ok
                        //secAggregateQuery.Add(new TermQuery(new Term("Permissions.IdUser", idUser)), Occur.SHOULD);
                        //secAggregateQuery.Add(new TermQuery(new Term("Permissions.IdGroup", idGroup)), Occur.SHOULD);
                        //secAggregateQuery.Add(new TermQuery(new Term("Permissions.IdOwnerGroup", idGroup)), Occur.SHOULD);

                        //aggregateQuery.Add(secAggregateQuery, Occur.MUST);

                        //if (betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate &&
                        //    betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate_Profile &&
                        //    (queryModel?.CreationYear.HasValue ?? false))
                        //{
                        //    aggregateQuery.Add(LuceneLib.Net.Search.NumericRangeQuery.NewInt32Range("CreationYear", queryModel.CreationYear.Value, queryModel.CreationYear.Value, true, true), Occur.MUST);
                        //}

                        //if (betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate &&
                        //    betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate_Profile &&
                        //    !string.IsNullOrWhiteSpace(queryModel?.IdProfile ?? null))
                        //{
                        //    aggregateQuery.Add(new TermQuery(new Term("Profiles.Id", queryModel.IdProfile)), Occur.MUST);
                        //}

                        if (betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro &&
                            (queryModel?.AnnoRegistrazione.HasValue ?? false))
                        {
                            //var annoRegistrazioneQuery = new BooleanQuery();
                            //annoRegistrazioneQuery.MinimumNumberShouldMatch = 1;

                            //annoRegistrazioneQuery.Add(LuceneLib.Net.Search.NumericRangeQuery.NewInt32Range("DatiRegistrazione.AnnoProtocollazione", queryModel.AnnoRegistrazione.Value, queryModel.AnnoRegistrazione.Value, true, true), Occur.SHOULD);
                            //annoRegistrazioneQuery.Add(LuceneLib.Net.Search.NumericRangeQuery.NewInt32Range("DatiRegistrazione.AnnoRegistrazione", queryModel.AnnoRegistrazione.Value, queryModel.AnnoRegistrazione.Value, true, true), Occur.SHOULD);

                            //aggregateQuery.Add(annoRegistrazioneQuery, Occur.MUST);
                        }

                        // ok
                        if (betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro &&
                            !string.IsNullOrWhiteSpace(queryModel?.IdRegistro ?? null))
                        {
                            //aggregateQuery.Add(new TermQuery(new Term("DatiRegistrazione.IdRegistro", queryModel.IdRegistro)), Occur.MUST);
                        }

                        Sort sortBy = null;

                        if (!string.IsNullOrWhiteSpace(queryModel?.ContainsText ?? null))
                        {
                            var escapedPredicate = string.Empty;// QueryParser.Escape(queryModel.ContainsText.Trim());

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

                            //var fields = MultiFields.GetFields(reader).Except(this.ExcludeFields).Select(f => f).ToArray();

                            //var queryParser = new MultiFieldQueryParser(
                            //                Version,
                            //                fields,
                            //                analyzer);

                            //queryParser.AutoGeneratePhraseQueries = true;

                            //var inputQuery = queryParser.Parse(escapedPredicate);

                            //aggregateQuery.Add(inputQuery, Occur.MUST);

                            // Ordinamento per rilevanza rispetto al testo inserito
                            sortBy = new Sort(new SortField("score", SortFieldType.SCORE));
                        }
                        else
                        {
                            // Ordinamento predefinito per data creazione
                            sortBy = new Sort(new SortField("CreationDate", SortFieldType.STRING, true));
                        }
                        this._logger.LogInformation($"aggregateQuery: '{aggregateQuery.ToString()}'.");
                        IndexSearcher searcher = null; // new IndexSearcher(reader);

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

                            elements.Add(new ElementResult()
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
                    //}
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

    private IDisposable CreateAnalyzer()
    {
        throw new NotImplementedException();
    }

    private void SetQuery(QueryDescriptor<DocumentoAmministrativoEntity> q, string idUser, string idGroup, 
        DocumentiAmministrativiQueryModel queryModel, DocumentoAmministrativoIndexTypesEnum betterIndex )
    {
        //secAggregateQuery.Add(new TermQuery(new Term("Permissions.IdUser", idUser)), Occur.SHOULD);
        //secAggregateQuery.Add(new TermQuery(new Term("Permissions.IdGroup", idGroup)), Occur.SHOULD);
        //secAggregateQuery.Add(new TermQuery(new Term("Permissions.IdOwnerGroup", idGroup)), Occur.SHOULD);
        q.Bool(b => b
            .Must(m => m
                .Nested(n => n
                    .Path(p => p.Permissions)
                    .Query(nq => nq
                        .Bool(nb => nb
                            .Should(sh => sh
                                .Term(t => t
                                    .Field(f => f.Permissions.First().IdUser)
                                    .Value(idUser)
                                ),
                                sh => sh
                                .Term(t => t
                                    .Field(f => f.Permissions.First().IdGroup)
                                    .Value(idGroup)
                                ),
                                sh => sh
                                .Term(t => t
                                    .Field(f => f.Permissions.First().IdOwnerGroup)
                                    .Value(idGroup)
                                )
                            )
                            .MinimumShouldMatch(1) // Assicura che almeno una delle condizioni should sia soddisfatta
                        )
                    )
                )
            )
        );
        // Aggiungi la query di range solo se betterIndex ha un determinato valore
        if (betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate &&
            betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate_Profile &&
            (queryModel?.CreationYear.HasValue ?? false))
        {
            //    aggregateQuery.Add(LuceneLib.Net.Search.NumericRangeQuery.NewInt32Range("CreationYear", queryModel.CreationYear.Value, queryModel.CreationYear.Value, true, true), Occur.MUST);
            q.Bool(b => b
                .Must(m => m
                    .Range(r => r.NumberRange( d => d
                            .Field(f => f.CreationYear)
                            .Gte(queryModel.CreationYear)
                            .Lte(queryModel.CreationYear)
                        )
                    )
                )
            );
        }
        return;

        if (betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro &&
            (queryModel?.AnnoRegistrazione.HasValue ?? false))
        {
            //var annoRegistrazioneQuery = new BooleanQuery();
            //annoRegistrazioneQuery.MinimumNumberShouldMatch = 1;

            // questo dato non c'è nel modello
            //annoRegistrazioneQuery.Add(LuceneLib.Net.Search.NumericRangeQuery.NewInt32Range("DatiRegistrazione.AnnoProtocollazione", queryModel.AnnoRegistrazione.Value, queryModel.AnnoRegistrazione.Value, true, true), Occur.SHOULD);
            //annoRegistrazioneQuery.Add(LuceneLib.Net.Search.NumericRangeQuery.NewInt32Range("DatiRegistrazione.AnnoRegistrazione", queryModel.AnnoRegistrazione.Value, queryModel.AnnoRegistrazione.Value, true, true), Occur.SHOULD);

            //q.Range(r => r.NumberRange(d => d
            //        .Field(f => f.DatiRegistrazione.Ann)
            //        .Gte(queryModel.CreationYear)
            //        .Lte(queryModel.CreationYear)
            //    )
            //);
        }


        if (betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro &&
            !string.IsNullOrWhiteSpace(queryModel?.IdRegistro ?? null))
        {
            q.Nested(n => n
                .Path(p => p.DatiRegistrazione)
                .Query(nq => nq
                    .Bool(b => b
                        .Should(sh => sh
                            .Term(t => t
                                .Field(f => f.DatiRegistrazione.IdRegistro)
                                .Value(queryModel.IdRegistro)
                            )
                        )
                        .MinimumShouldMatch(1) // Assicura che almeno una delle condizioni should sia soddisfatta
                    )
                )
            );
        }

        if (betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate &&
            betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate_Profile &&
            !string.IsNullOrWhiteSpace(queryModel?.IdProfile ?? null))
        {
            //            aggregateQuery.Add(new TermQuery(new Term("Profiles.Id", queryModel.IdProfile)), Occur.MUST);

            q.Nested(n => n
                .Path(p => p.Profiles)
                .Query(nq => nq
                    .Bool(b => b
                        .Should(sh => sh
                            .Term(t => t
                                .Field(f => f.Profiles.First().Id)
                                .Value(queryModel.IdProfile)
                            )
                        )
                        .MinimumShouldMatch(1) // Assicura che almeno una delle condizioni should sia soddisfatta
                    )
                )
            );
        }

        if (betterIndex != DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro &&
            (queryModel?.AnnoRegistrazione.HasValue ?? false))
        {

            //annoRegistrazioneQuery.Add(LuceneLib.Net.Search.NumericRangeQuery.NewInt32Range("DatiRegistrazione.AnnoProtocollazione", queryModel.AnnoRegistrazione.Value, queryModel.AnnoRegistrazione.Value, true, true), Occur.SHOULD);
            //annoRegistrazioneQuery.Add(LuceneLib.Net.Search.NumericRangeQuery.NewInt32Range("DatiRegistrazione.AnnoRegistrazione", queryModel.AnnoRegistrazione.Value, queryModel.AnnoRegistrazione.Value, true, true), Occur.SHOULD);

            //aggregateQuery.Add(annoRegistrazioneQuery, Occur.MUST);
        }



    }

    public async Task UpdateIndexMappingAsync()
    {
        var putMappingRequest = new PutMappingRequest("tenant")
        {
            Properties = new Properties
            {
                {
                    "permissions", new NestedProperty
                    {
                        Properties = new Properties
                        {
                            { "idGroup", GetTextProperty() },
                            { "idMember", GetTextProperty() },
                            { "idOwnerGroup", GetTextProperty() },
                            { "idOwnerUser", GetTextProperty() },
                            { "memberName", GetTextProperty() },
                            { "permissionType", GetTextProperty() },
                            { "rightType", GetTextProperty() }
                        }
                    }
                }
            }
        };

        var response = await _client.Indices.PutMappingAsync(putMappingRequest);

        if (response.IsValidResponse)
        {
            _logger.LogInformation("Index mapping updated successfully.");
        }
        else
        {
            _logger.LogError("Failed to update index mapping: {0}", response.ElasticsearchServerError);
        }

        TextProperty GetTextProperty() {
            return new TextProperty
            {
                Fields = new Properties
                                    {
                                        { "keyword", new KeywordProperty { IgnoreAbove = 256 } }
                                    }
            };
        }
    }

    public async Task CreateNewIndexWithMappingAsync()
    {
        var newIndexName = "tenant";

        var createIndexRequest = new CreateIndexRequest(newIndexName)
        {
            Mappings = new TypeMapping
            {
                Properties = new Properties
                {
                    {
                        "permissions", new NestedProperty
                        {
                            Properties = new Properties
                            {
                                { "idGroup", GetTextProperty() },
                                { "idMember", GetTextProperty() },
                                { "idOwnerGroup", GetTextProperty() },
                                { "idOwnerUser", GetTextProperty() },
                                { "memberName", GetTextProperty() },
                                { "permissionType", GetTextProperty() },
                                { "rightType", GetTextProperty() }
                            }
                        }
                    },
                    {
                        "classifications", new NestedProperty
                        {
                            Properties = new Properties
                            {
                                { "id", GetTextProperty() },
                                { "name", GetTextProperty() },
                            }
                        }
                    },
                    {
                        "aggregazioni", new NestedProperty
                        {
                            Properties = new Properties
                            {
                                { "id", GetTextProperty() },
                                { "denominazione", GetTextProperty() },
                                { "tipologia", GetTextProperty() },
                            }
                        }
                    },
                    {
                        "datiRegistrazione", new NestedProperty
                        {
                            Properties = new Properties
                            {
                                { "codiceRegistro", GetTextProperty() },
                                { "idRegistro", GetTextProperty() },
                                { "isRegistrato", new BooleanProperty() },
                                { "tipoRegistro", GetTextProperty() },
                                { "tipologiaFlusso", GetTextProperty() },

                                { "numeroProtocollo", new LongNumberProperty()},
                                { "dataProtocollazione", new DateProperty()},
                                { "annoProtocollo", new IntegerNumberProperty()},

                                { "numeroRegistrazione", new LongNumberProperty()},
                                { "dataRegistrazione", new DateProperty()},
                                { "annoRegistrazione", new IntegerNumberProperty()},
                            }
                        }
                    },
  
                    // Aggiungi altre proprietà necessarie qui
                    //{ "otherProperty1", new TextProperty() },
                    //{ "otherProperty2", new DateProperty() }
                }
            }
        };

        var response = await _client.Indices.CreateAsync(createIndexRequest);

        if (response.IsValidResponse)
        {
            _logger.LogInformation("New index created successfully.");
        }
        else
        {
            _logger.LogError("Failed to create new index: {0}", response.ElasticsearchServerError);
        }

        TextProperty GetTextProperty()
        {
            return new TextProperty
            {
                Fields = new Properties
                                    {
                                        { "keyword", new KeywordProperty { IgnoreAbove = 256 } }
                                    }
            };
        }

    }

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

    //protected virtual LuceneVersion Version
    //{
    //    get
    //    {
    //        return LuceneVersion.LUCENE_48;
    //    }
    //}

    //protected virtual Analyzer CreateAnalyzer()
    //{
    //    return new Lucene.Net.Analysis.Standard.StandardAnalyzer(this.Version);
    //}

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

    protected virtual async Task<DocumentoAmministrativoIndexTypesEnum> FindBetterIndex(string idTenant, DocumentiAmministrativiQueryModel? queryModel = null)
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

    //protected virtual async Task<LuceneLib.Net.Store.Directory> OpenDirectory(DocumentoAmministrativoIndexTypesEnum index, string idTenant, DocumentiAmministrativiQueryModel? queryModel = null)
    //{
    //    string path = string.Empty;
    //    var rootPath = Path.Combine(await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true), "Indexes");

    //    switch (index)
    //    {
    //        case DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate_Profile:
    //            {
    //                path = Path.Combine(rootPath, nameof(DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate_Profile), idTenant, queryModel.CreationYear.Value.ToString(), queryModel.IdProfile);

    //                break;
    //            }
    //        case DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate:
    //            {
    //                path = Path.Combine(rootPath, nameof(DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate), idTenant, queryModel.CreationYear.Value.ToString());

    //                break;
    //            }
    //        case DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro:
    //            {
    //                path = Path.Combine(rootPath, nameof(DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro), idTenant, queryModel.AnnoRegistrazione.Value.ToString(), queryModel.IdRegistro);

    //                break;
    //            }
    //        case DocumentoAmministrativoIndexTypesEnum.Tenant:
    //            {
    //                path = Path.Combine(rootPath, nameof(DocumentoAmministrativoIndexTypesEnum.Tenant), idTenant);

    //                break;
    //            }
    //    }

    //    path = path.PathAsUnixPath();

    //    return FSDirectory.Open(path);
    //}
}
