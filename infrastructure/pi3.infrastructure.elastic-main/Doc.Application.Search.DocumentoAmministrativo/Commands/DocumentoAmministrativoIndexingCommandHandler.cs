// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Doc.Search.DocumentoAmministrativo;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.TextExtractors;
using Pi3.Search.DocumentoAmministrativo.Models;

namespace Doc.Application.Search.DocumentoAmministrativo.Commands;

public class DocumentoAmministrativoIndexingCommandHandler
    : IRequestHandler<AddOrUpdateDocumentoAmministrativoCommand, AddOrUpdateDocumentoAmministrativoCommandResults>,
        IRequestHandler<DeleteDocumentoAmministrativoCommand, DeleteDocumentoAmministrativoCommandResults>
{
    #region Public Members

    public DocumentoAmministrativoIndexingCommandHandler(
      ILogger<DocumentoAmministrativoIndexingCommandHandler> logger,
      IOptions<IndexingOptions> options,
      IConfigurationService configurationService,
      IDocumentoAmministrativoRepository documentRepository,
      IDocumentBlobRepository documentBlobRepository,
      IFileTextExtractorFactory fileTextExtractorFactory,
      DocumentoAmministrativoIndexingService documentoAmministrativoIndexingService)
    {
        _logger = logger;
        _options = options;
        _configurationService = configurationService;
        _documentRepository = documentRepository;
        _documentBlobRepository = documentBlobRepository;
        _fileTextExtractorFactory = fileTextExtractorFactory;
        this.documentoAmministrativoIndexingService = documentoAmministrativoIndexingService;
    }

    public async Task<AddOrUpdateDocumentoAmministrativoCommandResults> Handle(AddOrUpdateDocumentoAmministrativoCommand request, CancellationToken cancellationToken)
    {
        return await documentoAmministrativoIndexingService.AddDocument(request.IdTenant, request.IdDocument);

        //string error = null;
        //string warnings = null;
        //bool handled = false;
        //DateTime startDate = DateTime.Now;

        //try
        //{
        //    if (!await _documentRepository.Exists(request.IdTenant, request.IdDocument))
        //        throw new DocumentoAmministrativoNotFoundPi3Exception(request.IdDocument);

        //    _logger.LogInformation($"Reperimento dati documento con Id '{request.IdDocument}' per IdTenant '{request.IdTenant}' in corso...");
        //    var documentAggregate = await _documentRepository.Get(request.IdTenant, request.IdDocument);
        //    _logger.LogInformation($"Reperimento dati documento con Id '{request.IdDocument}' per IdTenant '{request.IdTenant}' completato.");

        //    var idBlob = documentAggregate.CurrentVersion?.DocumentBlobRef?.IdBlob;

        //    _logger.LogInformation($"IdBlob: '{idBlob}'.");

        //    FileTextExtractionResultsModel fileTextExtractionResults = null;
            
        //    if (!this._options.Value.DocumentoAmministrativoIndexingOptions.BlobIndexingEnabled)
        //    {
        //        warnings = $"Indicizzazione del file non abilitata.";

        //        _logger.LogInformation(warnings);
        //    }
        //    else if (!string.IsNullOrWhiteSpace(idBlob))
        //    {
        //        // Se risulta associato un file alla versione corrente del documento

        //        if (!await this._documentBlobRepository.Exists(request.IdTenant, idBlob))
        //        {
        //            warnings = $"File con Id '{idBlob}' non trovato.";

        //            _logger.LogInformation(warnings);
        //        }
        //        else
        //        {
        //            _logger.LogInformation($"Caricamento contenuto del file in corso...");

        //            // Caricamento del contenuto
        //            var documentBlobAggregate = await this._documentBlobRepository.Get(request.IdTenant, idBlob);                           

        //            _logger.LogInformation($"Caricamento contenuto del file completato. FileSize: {documentBlobAggregate.FileSize}.");
                    
        //            _logger.LogInformation($"Estrazione del testo all'interno del file {documentBlobAggregate.FileName} in corso...");

        //            // Estrazione del testo
        //            var inputFileFormat = Path.GetExtension(documentBlobAggregate.FileName);

        //            var creation = await this._fileTextExtractorFactory.TryCreate(documentBlobAggregate.FileName);

        //            if (!creation.Success)
        //            {
        //                warnings = $"Formato file '{inputFileFormat}' non supportato.";

        //                _logger.LogInformation(warnings);
        //            }
        //            else
        //            {
        //                var extractTextStartDate = DateTime.Now;

        //                fileTextExtractionResults = await creation.Service.ExtractText(documentBlobAggregate.FileName, documentBlobAggregate.Stream);

        //                _logger.LogInformation($"Estrazione del testo all'interno del file {documentBlobAggregate.FileName} completata. Elapsed seconds: {DateTime.Now.Subtract(extractTextStartDate).TotalSeconds}.");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        warnings = $"File non acquisito.";

        //        _logger.LogInformation(warnings);
        //    }

        //    _logger.LogInformation($"Indicizzazione documento in corso...");

        //    var doc = new Infrastructure.Services.Elastic.Entities.DocumentoAmministrativoEntity();

        //    // Indicizzazione metadati documento
        //    doc.FillDocumentoAmministrativo(documentAggregate);

        //    // Indicizzazione contenuto documento se presente
        //    if (fileTextExtractionResults is not null) doc.FillDocumentContent(fileTextExtractionResults.ToString().ToLowerInvariant());

        //    foreach(var indexType in this._options.Value.DocumentoAmministrativoIndexingOptions.IndexTypes)
        //    {
        //        foreach(var indexPath in await this.GetIndexPaths(indexType, documentAggregate))
        //        {
        //            _logger.LogInformation($"IndexType: {indexType} - Directory: {indexPath}");

        //            await this._IndexerService.AddOrEditDocument(doc, indexPath);
        //        }
        //    }

        //    _logger.LogInformation($"Indicizzazione documento completata. Elapsed seconds: {DateTime.Now.Subtract(startDate).TotalSeconds}.");

        //    handled = true;

        //}
        //catch (Exception ex)
        //{
        //    error = ex.ToString();

        //    _logger.LogError(ex, error);
        //}

        //return new AddOrUpdateDocumentoAmministrativoCommandResults()
        //{
        //    RequestStatus = new RequestStatus()
        //    {
        //        Handled = handled,
        //        ProcessingDate = DateTime.Now,
        //        ProcessingElapsed = DateTime.Now.Subtract(startDate).TotalSeconds,
        //        Error = error,
        //        Warnings = warnings
        //    }
        //};
    }
    
    public async Task<DeleteDocumentoAmministrativoCommandResults> Handle(DeleteDocumentoAmministrativoCommand request, CancellationToken cancellationToken)
    {
        return await documentoAmministrativoIndexingService.DeleteDocument(request.IdTenant, request.IdDocument);

        //string error = null;
        //string warnings = null;
        //bool handled = false;
        //DateTime startDate = DateTime.Now;

        //try
        //{
        //    if (!await _documentRepository.Exists(request.IdTenant, request.IdDocument))
        //        throw new DocumentoAmministrativoNotFoundPi3Exception(request.IdDocument);

        //    _logger.LogInformation($"Reperimento dati documento con Id '{request.IdDocument}' per IdTenant '{request.IdTenant}' in corso...");
        //    var documentAggregate = await _documentRepository.Get(request.IdTenant, request.IdDocument);
        //    _logger.LogInformation($"Reperimento dati documento con Id '{request.IdDocument}' per IdTenant '{request.IdTenant}' completato.");

        //    bool almostOneDeleted = false;

        //    foreach (var indexType in this._options.Value.DocumentoAmministrativoIndexingOptions.IndexTypes)
        //    {
        //        foreach (var indexPath in await this.GetIndexPaths(indexType, documentAggregate))
        //        {
        //            _logger.LogInformation($"IndexType: {indexType} - Directory: {indexPath}");

        //            _logger.LogInformation($"Rimozione documento con Id '{request.IdDocument}' dall'indice in corso...");

        //            await this._IndexerService.DeleteDocument(documentAggregate.Id, indexPath);

        //            //if (!await this._IndexerService.ExistsDocument(indexPath, documentAggregate.Id))
        //            //    this._logger.LogInformation(string.Format(ErrorDescriptions.DocumentoNonTrovato, request.IdDocument));
        //            //else
        //            //{
        //            //    _logger.LogInformation($"Rimozione documento con Id '{request.IdDocument}' dall'indice in corso...");

        //            //    var indexingStartDate = DateTime.Now;

        //            //    await this._IndexerService.DeleteDocument(indexPath, documentAggregate.Id);

        //            //    _logger.LogInformation($"Rimozione documento con Id '{request.IdDocument}' dall'indice completata. Elapsed seconds: {DateTime.Now.Subtract(indexingStartDate).TotalSeconds}.");

        //            //    almostOneDeleted = true;
        //            //}
        //        }
        //    }

        //    handled = almostOneDeleted;
        //}
        //catch (Exception ex)
        //{
        //    error = ex.ToString();

        //    _logger.LogError(ex, error);
        //}

        //return new DeleteDocumentoAmministrativoCommandResults()
        //{
        //    RequestStatus = new RequestStatus()
        //    {
        //        Handled = handled,
        //        ProcessingDate = DateTime.Now,
        //        ProcessingElapsed = DateTime.Now.Subtract(startDate).TotalSeconds,
        //        Error = error,
        //        Warnings = warnings
        //    }
        //};
    }

    #endregion

    #region Private Members

    protected readonly ILogger<DocumentoAmministrativoIndexingCommandHandler> _logger;
    protected readonly IOptions<IndexingOptions> _options;
    protected readonly IConfigurationService _configurationService;
    protected readonly IDocumentoAmministrativoRepository _documentRepository;
    protected readonly IDocumentBlobRepository _documentBlobRepository;
    private readonly DocumentoAmministrativoIndexingService documentoAmministrativoIndexingService;
    protected readonly IFileTextExtractorFactory _fileTextExtractorFactory;


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

    //protected virtual string[] GetIndexPaths(DocumentoAmministrativoIndexPartition partition, string instance, Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo documentoAggregate)
    //{
    //    List<string> path = new List<string>();
        
    //    switch (partition.PartitionBy)
    //    {
    //        case DocumentoAmministrativoIndexPartitionTypesEnum.Tenant:
    //            path.Add(Path.Combine(partition.Directory, instance, documentoAggregate.IdTenant));
    //            break;

    //        case DocumentoAmministrativoIndexPartitionTypesEnum.Tenant_CreationDate:
    //            path.Add(Path.Combine(partition.Directory, instance,
    //                documentoAggregate.IdTenant, documentoAggregate.CreationDate.Year.ToString()));
    //            break;

    //        case DocumentoAmministrativoIndexPartitionTypesEnum.Tenant_AnnoRegistrazione_Registro:
    //            if (documentoAggregate.DatiRegistrazione == null
    //                || (documentoAggregate.DatiRegistrazione != null && !documentoAggregate.DatiRegistrazione.IsRegistrato))
    //            {
    //                // non supportato
    //            }
    //            else if (documentoAggregate.DatiRegistrazione is DatiRegistrazioneProtocollo)
    //            {
    //                var datiRegistrazione = (DatiRegistrazioneProtocollo)documentoAggregate.DatiRegistrazione;

    //                path.Add(Path.Combine(partition.Directory,
    //                    instance,
    //                    documentoAggregate.IdTenant,
    //                    datiRegistrazione.DataProtocollazione.Value.Year.ToString(),
    //                    datiRegistrazione.IdRegistro));
    //            }
    //            else if (documentoAggregate.DatiRegistrazione is DatiRegistrazioneRepertorio)
    //            {
    //                var datiRegistrazione = (DatiRegistrazioneRepertorio)documentoAggregate.DatiRegistrazione;

    //                path.Add(Path.Combine(partition.Directory,
    //                    instance,
    //                    documentoAggregate.IdTenant,
    //                    datiRegistrazione.DataRegistrazione.Value.Year.ToString(),
    //                    datiRegistrazione.IdRegistro));
    //            }

    //            break;
    //        case DocumentoAmministrativoIndexPartitionTypesEnum.Tenant_CreationDate_Profile:
    //            foreach (var p in documentoAggregate.Profiles)
    //            {
    //                path.Add(Path.Combine(partition.Directory,
    //                    instance,
    //                    documentoAggregate.IdTenant,
    //                    documentoAggregate.CreationDate.Year.ToString(),
    //                    p.Id));
    //            }

    //            break;
    //    }

    //    return path.ToArray();
    //}

    #endregion
}