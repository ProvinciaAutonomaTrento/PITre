// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using DocsPaVO.Validations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckOutDocumentWithFileRequest = Pi3.App.Legacy.WebApi.Application.Requests.CheckOutDocumentWithFile;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckOutDocumentWithFile
{
    public class CheckOutDocumentWithFileHandler : IRequestHandler<CheckOutDocumentWithFileRequest, CheckOutDocumentWithFileResult>
    {
        #region Public Members

        public CheckOutDocumentWithFileHandler(
            ILogger<CheckOutDocumentWithFileHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IDocumentBlobRepository documentBlobRepository
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._documentBlobRepository = documentBlobRepository;
        }

        public async Task<CheckOutDocumentWithFileResult> Handle(CheckOutDocumentWithFileRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

                var documentoAmministrativoAggregate = await
                        this._documentoAmministrativoRepository.Get(idTenant, request.idDocument);

                string idBlob = null!;
                if (documentoAmministrativoAggregate.CurrentVersion != null
                    && documentoAmministrativoAggregate.CurrentVersion.DocumentBlobRef! != null!)
                {
                    idBlob = documentoAmministrativoAggregate.CurrentVersion.DocumentBlobRef.IdBlob;
                }

                byte[] content = null!;

                if (!string.IsNullOrWhiteSpace(idBlob))
                {
                    // Reperimento del file acquisito per la versione corrente

                    var documentBlobAggregate = await this._documentBlobRepository.Get(idTenant, idBlob);

                    content = new byte[documentBlobAggregate.Stream.Length];
                    var read = documentBlobAggregate.Stream.Read(content, 0, content.Length);
                }
                else if (documentoAmministrativoAggregate.Profiles.Any())
                {
                    // File non acquisito per la versione corrente

                    var idTipoAtto = documentoAmministrativoAggregate.Profiles[0].Id.AsLong();

                    var tipoAttoEntity = await this._pi3DbContext.TipoAttoEntities
                        .AsNoTracking()
                        .Where(t => t.SYSTEM_ID == idTipoAtto)
                        .Select(t => new
                        {
                            t.PATH_MOD_1,
                            t.PATH_MOD_2
                        })
                        .FirstAsync();

                    if (!string.IsNullOrWhiteSpace(tipoAttoEntity.PATH_MOD_1))
                    {
                        var pathMod1 = tipoAttoEntity.PATH_MOD_1.PathAsUnixPath();

                        // Risulta definito in amministrazione un modello predefinito sulla tipologia documento
                        if (!File.Exists(pathMod1))
                        {
                            // File modello non trovato
                            throw new ModelloNotFoundPi3Exception(pathMod1);
                        }

                        content = File.ReadAllBytes(pathMod1);
                    }
                }

                if (content == null)
                {
                    // Non risulta definito in amministrazione un modello predefinito sulla tipologia documento,
                    // reperimento del modello di default
                    var modelType = Path.GetExtension(request.documentLocation).Replace(".", string.Empty).ToLower();
                    var modelContent = DefaultModels.ResourceManager.GetObject(modelType);
                    if (modelContent == null)
                    {
                        // Modello predefinito non trovato
                        throw new ModelloNotFoundPi3Exception(modelType);
                    }

                    if (modelContent.GetType() == typeof(string))
                        content = Encoding.UTF8.GetBytes((string)modelContent);
                    else
                        content = (byte[])modelContent;
                }

                if (content == null)
                { 
                    // Se il contenuto del modello non � stato trovato
                    return new CheckOutDocumentWithFileResult(
                        new ValidationResultInfo()
                        {
                            Value = false,
                            BrokenRules = new BrokenRule[1]
                            {
                                new BrokenRule()
                                {
                                    ID = Descriptions.ModelloNonTrovatoCode,
                                    Description = ErrorDescriptions.ModelloNonTrovato,
                                    Level = BrokenRule.BrokenRuleLevelEnum.Error
                                }
                            }
                        },
                        null!,
                        null!);
                }
                else
                {
                    var checkOutDocumentResult = await this._mediator.Send(
                        new Requests.CheckOutDocument(
                            request.idDocument,
                            request.documentNumber,
                            request.documentLocation,
                            request.machineName,
                            request.utente));

                    return new CheckOutDocumentWithFileResult(
                        checkOutDocumentResult.output,
                        checkOutDocumentResult.checkOutStatus,
                        content);
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);

                return new CheckOutDocumentWithFileResult(
                    new ValidationResultInfo()
                    {
                        Value = false,
                        BrokenRules = new BrokenRule[1]
                        {
                            new BrokenRule()
                            {
                                Level = BrokenRule.BrokenRuleLevelEnum.Error,
                                ID = Descriptions.CheckOutErrorCode,
                                Description = pi3Ex.Message
                            }
                        }
                    },
                    null!, 
                    null!);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);

                return new CheckOutDocumentWithFileResult(
                                   new ValidationResultInfo()
                                   {
                                       Value = false,
                                       BrokenRules = new BrokenRule[1]
                                       {
                                            new BrokenRule()
                                            {
                                                Level = BrokenRule.BrokenRuleLevelEnum.Error,
                                                ID = Descriptions.CheckOutErrorCode,
                                                Description = ex.Message
                                            }
                                        }
                                    },
                                    null!,
                                    null!);
            }
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CheckOutDocumentWithFileHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IDocumentBlobRepository _documentBlobRepository;

        #endregion
    }
}