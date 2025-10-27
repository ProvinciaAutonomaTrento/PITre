// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Doc.Application.Search.DocumentoAmministrativo;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using pi3.Core.Contracts.DocumentoAmministrativo;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Contracts.DocumentoAmministrativo.Index;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.TextExtractors;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Exceptions;
using Pi3.Search.DocumentoAmministrativo.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Doc.Search.DocumentoAmministrativo;

public class DocumentoAmministrativoIndexingService
{
    private readonly ILogger _logger;
    private readonly IOptions<IndexingOptions> _options;
    private readonly IConfigurationService _configurationService;
    private readonly IFileTextExtractorFactory _fileTextExtractorFactory;
    private readonly IIndexingService _IndexerService;
    private readonly IDocumentoAmministrativoRepository _documentRepository;
    private readonly IDocumentBlobRepository _documentBlobRepository;

    public DocumentoAmministrativoIndexingService(
      ILogger<DocumentoAmministrativoIndexingService> logger,
      IOptions<IndexingOptions> options,
      IConfigurationService configurationService,
      IDocumentoAmministrativoRepository documentRepository,
      IDocumentBlobRepository documentBlobRepository,
      IFileTextExtractorFactory fileTextExtractorFactory,
      IIndexingService IndexerService)
    {
        _logger = logger;
        _options = options;
        _configurationService = configurationService;
        _documentRepository = documentRepository;
        _documentBlobRepository = documentBlobRepository;
        _fileTextExtractorFactory = fileTextExtractorFactory;
        _IndexerService = IndexerService;
    }

    public async Task<AddOrUpdateDocumentoAmministrativoCommandResults> AddDocument(string idTenant, string idDocument)
    {
        string error = null;
        string warnings = null;
        bool handled = false;
        DateTime startDate = DateTime.Now;

        try
        {
            if (!await _documentRepository.Exists(idTenant, idDocument))
                throw new DocumentoAmministrativoNotFoundPi3Exception(idDocument);

            _logger.LogInformation($"Reperimento dati documento con Id '{idDocument}' per IdTenant '{idTenant}' in corso...");
            var documentAggregate = await _documentRepository.Get(idTenant, idDocument);
            _logger.LogInformation($"Reperimento dati documento con Id '{idDocument}' per IdTenant '{idTenant}' completato.");

            var idBlob = documentAggregate.CurrentVersion?.DocumentBlobRef?.IdBlob;

            _logger.LogInformation($"IdBlob: '{idBlob}'.");

            FileTextExtractionResultsModel fileTextExtractionResults = null;

            if (!this._options.Value.DocumentoAmministrativoIndexingOptions.BlobIndexingEnabled)
            {
                warnings = $"Indicizzazione del file non abilitata.";

                _logger.LogInformation(warnings);
            }
            else if (!string.IsNullOrWhiteSpace(idBlob))
            {
                // Se risulta associato un file alla versione corrente del documento

                if (!await this._documentBlobRepository.Exists(idTenant, idBlob))
                {
                    warnings = $"File con Id '{idBlob}' non trovato.";

                    _logger.LogInformation(warnings);
                }
                else
                {
                    _logger.LogInformation($"Caricamento contenuto del file in corso...");

                    // Caricamento del contenuto
                    var documentBlobAggregate = await this._documentBlobRepository.Get(idTenant, idBlob);

                    _logger.LogInformation($"Caricamento contenuto del file completato. FileSize: {documentBlobAggregate.FileSize}.");

                    _logger.LogInformation($"Estrazione del testo all'interno del file {documentBlobAggregate.FileName} in corso...");

                    // Estrazione del testo
                    var inputFileFormat = Path.GetExtension(documentBlobAggregate.FileName);

                    var creation = await this._fileTextExtractorFactory.TryCreate(documentBlobAggregate.FileName);

                    if (!creation.Success)
                    {
                        warnings = $"Formato file '{inputFileFormat}' non supportato.";

                        _logger.LogInformation(warnings);
                    }
                    else
                    {
                        var extractTextStartDate = DateTime.Now;

                        fileTextExtractionResults = await creation.Service.ExtractText(documentBlobAggregate.FileName, documentBlobAggregate.Stream);

                        _logger.LogInformation($"Estrazione del testo all'interno del file {documentBlobAggregate.FileName} completata. Elapsed seconds: {DateTime.Now.Subtract(extractTextStartDate).TotalSeconds}.");
                    }
                }
            }
            else
            {
                warnings = $"File non acquisito.";

                _logger.LogInformation(warnings);
            }

            _logger.LogInformation($"Indicizzazione documento in corso...");

            var doc = new DocumentoAmministrativoEntity();

            // Indicizzazione metadati documento
            doc.FillDocumentoAmministrativo(documentAggregate);

            // Indicizzazione contenuto documento se presente
            if (fileTextExtractionResults is not null) doc.FillDocumentContent(fileTextExtractionResults.ToString().ToLowerInvariant());

            foreach (var indexType in this._options.Value.DocumentoAmministrativoIndexingOptions.IndexTypes)
            {
                //foreach (var indexPath in await this.GetIndexPaths(indexType, documentAggregate))
                //{
                    _logger.LogInformation($"IndexType: {indexType}");

                    var result = await this._IndexerService.AddOrEditDocument(doc, indexType.ToString());
                if(!result)
                    _logger.LogInformation($"Error Importing: {indexType} {doc.Id}");

                //}
            }

            _logger.LogInformation($"Indicizzazione documento completata. Elapsed seconds: {DateTime.Now.Subtract(startDate).TotalSeconds}.");

            handled = true;

        }
        catch (Exception ex)
        {
            error = ex.ToString();

            _logger.LogError(ex, error);
        }

        return new AddOrUpdateDocumentoAmministrativoCommandResults()
        {
            RequestStatus = new RequestStatus()
            {
                Handled = handled,
                ProcessingDate = DateTime.Now,
                ProcessingElapsed = DateTime.Now.Subtract(startDate).TotalSeconds,
                Error = error,
                Warnings = warnings
            }
        };
    }

    public async Task UpdateDocument(string tenantId, string documentId)
    {
    }


    public async Task<DeleteDocumentoAmministrativoCommandResults> DeleteDocument(string idTenant, string idDocument)
    {
        string error = null;
        string warnings = null;
        bool handled = false;
        DateTime startDate = DateTime.Now;

        try
        {
            if (!await _documentRepository.Exists(idTenant, idDocument))
                throw new DocumentoAmministrativoNotFoundPi3Exception(idDocument);

            _logger.LogInformation($"Reperimento dati documento con Id '{idDocument}' per IdTenant '{idTenant}' in corso...");
            var documentAggregate = await _documentRepository.Get(idTenant, idDocument);
            _logger.LogInformation($"Reperimento dati documento con Id '{idDocument}' per IdTenant '{idTenant}' completato.");

            bool almostOneDeleted = false;

            foreach (var indexType in this._options.Value.DocumentoAmministrativoIndexingOptions.IndexTypes)
            {
                foreach (var indexPath in await this.GetIndexPaths(indexType, documentAggregate))
                {
                    _logger.LogInformation($"IndexType: {indexType} - Directory: {indexPath}");

                    _logger.LogInformation($"Rimozione documento con Id '{idDocument}' dall'indice in corso...");

                    await this._IndexerService.DeleteDocument(documentAggregate.Id, indexPath);

                    //if (!await this._IndexerService.ExistsDocument(indexPath, documentAggregate.Id))
                    //    this._logger.LogInformation(string.Format(ErrorDescriptions.DocumentoNonTrovato, request.IdDocument));
                    //else
                    //{
                    //    _logger.LogInformation($"Rimozione documento con Id '{request.IdDocument}' dall'indice in corso...");

                    //    var indexingStartDate = DateTime.Now;

                    //    await this._IndexerService.DeleteDocument(indexPath, documentAggregate.Id);

                    //    _logger.LogInformation($"Rimozione documento con Id '{request.IdDocument}' dall'indice completata. Elapsed seconds: {DateTime.Now.Subtract(indexingStartDate).TotalSeconds}.");

                    //    almostOneDeleted = true;
                    //}
                }
            }

            handled = almostOneDeleted;
        }
        catch (Exception ex)
        {
            error = ex.ToString();

            _logger.LogError(ex, error);
        }

        return new DeleteDocumentoAmministrativoCommandResults()
        {
            RequestStatus = new RequestStatus()
            {
                Handled = handled,
                ProcessingDate = DateTime.Now,
                ProcessingElapsed = DateTime.Now.Subtract(startDate).TotalSeconds,
                Error = error,
                Warnings = warnings
            }
        };

    }

    protected virtual async Task<string[]> GetIndexPaths(DocumentoAmministrativoIndexTypesEnum indexType, Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo documentAggregate)
    {
        var paths = new List<string>();
        var rootPath = Path.Combine(await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true), "Indexes");

        switch (indexType)
        {
            case DocumentoAmministrativoIndexTypesEnum.Tenant:
                //paths.Add(Path.Combine(rootPath, nameof(DocumentoAmministrativoIndexTypesEnum.Tenant), documentAggregate.IdTenant).PathAsUnixPath());
                paths.Add(string.Format("{0}_{1}",
                    nameof(DocumentoAmministrativoIndexTypesEnum.Tenant),
                    documentAggregate.IdTenant
                    ));
                break;

            case DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate:
                //paths.Add(Path.Combine(rootPath, nameof(DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate), documentAggregate.IdTenant, documentAggregate.CreationDate.Year.ToString()).PathAsUnixPath());
                paths.Add(string.Format("{0}_{1}_{2}",
                    nameof(DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate),
                    documentAggregate.IdTenant,
                    documentAggregate.CreationDate.Year.ToString()
                    ));
                break;

            case DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro:
                if (documentAggregate.DatiRegistrazione == null
                    || (documentAggregate.DatiRegistrazione != null && !documentAggregate.DatiRegistrazione.IsRegistrato))
                {
                    // non supportato
                }
                else if (documentAggregate.DatiRegistrazione is DatiRegistrazioneProtocollo)
                {
                    var datiRegistrazione = (DatiRegistrazioneProtocollo)documentAggregate.DatiRegistrazione;
                    //paths.Add(Path.Combine(rootPath, nameof(DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro), documentAggregate.IdTenant, datiRegistrazione.DataProtocollazione.Value.Year.ToString(), datiRegistrazione.IdRegistro).PathAsUnixPath());
                    paths.Add(string.Format("{0}_{1}_{2}_{3}",
                    nameof(DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro),
                    documentAggregate.IdTenant,
                    datiRegistrazione.DataProtocollazione.Value.Year.ToString(),
                    datiRegistrazione.IdRegistro
                    ));
                }
                else if (documentAggregate.DatiRegistrazione is DatiRegistrazioneRepertorio)
                {
                    var datiRegistrazione = (DatiRegistrazioneRepertorio)documentAggregate.DatiRegistrazione;

                    //paths.Add(Path.Combine(rootPath, nameof(DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro), documentAggregate.IdTenant, datiRegistrazione.DataRegistrazione.Value.Year.ToString(), datiRegistrazione.IdRegistro).PathAsUnixPath());
                    paths.Add(string.Format("{0}_{1}_{2}_{3}",
                    nameof(DocumentoAmministrativoIndexTypesEnum.Tenant_AnnoRegistrazione_Registro),
                    documentAggregate.IdTenant,
                    datiRegistrazione.DataRegistrazione.Value.Year.ToString(),
                    datiRegistrazione.IdRegistro
                    ));
                }

                break;
            case DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate_Profile:
                foreach (var p in documentAggregate.Profiles)
                {
                    //paths.Add(Path.Combine(rootPath, nameof(DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate_Profile), documentAggregate.IdTenant, documentAggregate.CreationDate.Year.ToString(), p.Id).PathAsUnixPath());
                    paths.Add(string.Format("{0}_{1}_{2}_{3}",
                    nameof(DocumentoAmministrativoIndexTypesEnum.Tenant_CreationDate_Profile),
                    documentAggregate.IdTenant,
                    documentAggregate.CreationDate.Year.ToString(),
                    p.Id
                    ));
                }

                break;
        }

        return paths.ToArray();
    }

    public class RegistrazioneJsonConverter : JsonConverter<Registrazione>
    {
        public override Registrazione Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                if (doc.RootElement.TryGetProperty("DatiRegistrazione", out JsonElement tipoRegistroElement))
                {
                    string tipoRegistro = tipoRegistroElement.GetString();
                    switch (tipoRegistro)
                    {
                        case nameof(RegistrazioneProtocollo):
                            return JsonSerializer.Deserialize<RegistrazioneProtocollo>(doc.RootElement.GetRawText(), options);
                        case nameof(RegistrazioneRepertorio):
                            return JsonSerializer.Deserialize<RegistrazioneRepertorio>(doc.RootElement.GetRawText(), options);
                    }
                }
            }
            throw new JsonException("TipoRegistro non valido");
        }

        public override void Write(Utf8JsonWriter writer, Registrazione value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }

    public static class JsonSerializerOptionsProvider
    {
        public static JsonSerializerOptions GetOptions()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new RegistrazioneJsonConverter()
            }
            };
            return options;
        }
    }
}
