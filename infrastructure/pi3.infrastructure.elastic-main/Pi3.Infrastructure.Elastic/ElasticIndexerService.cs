// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using System.Runtime.CompilerServices;
using Elastic.Transport.Products.Elasticsearch;
using Microsoft.Extensions.Options;
using pi3.Core.Contracts.DocumentoAmministrativo;
using Doc.Search.DocumentoAmministrativo;
using static Doc.Search.DocumentoAmministrativo.DocumentoAmministrativoIndexingService;
using Elastic.Clients.Elasticsearch.Fluent;
using Elastic.Clients.Elasticsearch.Serialization;
using Elastic.Transport;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Pi3.Infrastructure.Elastic;

public class ElasticIndexerService : IIndexingService
{
    #region Public members

    public ElasticIndexerService(ILogger<ElasticIndexerService> logger, IOptions<IndexingOptions> options)
    {
        this._logger = logger;

        //var settings = new ElasticsearchClientSettings(new Uri(options.Value.ElasticSearchUri));
        //client = new ElasticsearchClient(settings);
        var nodePool = new SingleNodePool(new Uri(options.Value.ElasticSearchUri));
        var settings = new ElasticsearchClientSettings(
            nodePool,
            sourceSerializer: (defaultSerializer, settings) =>
                new DefaultSourceSerializer(settings, (val) => JsonSerializerOptionsProvider.GetOptions()));
        client = new ElasticsearchClient(settings);
    }

    public async Task<bool> AddOrEditDocument<T>(T document, string index)
    {
        var indexResponse = await client.IndexAsync(document, index.ToLowerInvariant()) ;
        if(indexResponse.ElasticsearchServerError is not null)
        {
            this._logger.LogError($"Errore nell'indicizzazione del file: {indexResponse.ElasticsearchServerError.Error}");
            throw new Exception(indexResponse.ElasticsearchServerError.Error.ToString());
        }
        return indexResponse.IsSuccess();
    }

    public async Task EditDocument<T>(T document)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteDocument(string id, string index)
    {
        var indexResponse = await client.DeleteAsync(index, id);
        if(indexResponse.ElasticsearchServerError is not null)
        {
            this._logger.LogError($"Errore nell'eliminazione dell'indicizzazione del file: {indexResponse.ElasticsearchServerError.Error}");
            throw new Exception(indexResponse.ElasticsearchServerError.Error.ToString());
        }
        return indexResponse.IsSuccess();
    }

    #endregion

    #region Private members

    protected readonly ILogger<ElasticIndexerService> _logger;
    protected readonly ElasticsearchClient client;

    #endregion
}
