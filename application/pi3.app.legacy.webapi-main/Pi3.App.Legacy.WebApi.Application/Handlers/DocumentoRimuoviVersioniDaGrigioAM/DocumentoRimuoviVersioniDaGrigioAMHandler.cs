// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.PrjDocImport;
using DocumentFormat.OpenXml.Bibliography;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.RemotePdfSignStamp;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoRimuoviVersioniDaGrigioAMRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoRimuoviVersioniDaGrigioAM;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoRimuoviVersioniDaGrigioAM
{
    public class DocumentoRimuoviVersioniDaGrigioAMHandler : IRequestHandler<DocumentoRimuoviVersioniDaGrigioAMRequest, DocumentoRimuoviVersioniDaGrigioAMResult>
    {
        #region Public Members

        public DocumentoRimuoviVersioniDaGrigioAMHandler(ILogger<DocumentoRimuoviVersioniDaGrigioAMHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        }

        public async Task<DocumentoRimuoviVersioniDaGrigioAMResult> Handle(DocumentoRimuoviVersioniDaGrigioAMRequest request, CancellationToken cancellationToken)
        {
            var result = new ImportResult();
            var idProfile = request.idProfile;
            try
            {
                var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var numVersionRemoved = 0;

                var documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(idTenant, idProfile, new ILoadBehavior[1]
                {
                    new GetDocumentoAmministrativoLoadBehavior()
                    {
                        LoadProfiles = true,
                        LoadClassifications = false,
                        LoadAllegati = true,
                        LoadAggregazioni = false,
                        LoadVersions = true,
                        LoadPermissions = true,
                        LoadMittentiDestinatari = true,
                        LoadKeywords = false,
                        LoadNote = false
                    }
                });

                if(documentoAmministrativoAggregate.Consolidamento != null)
                    throw new DocumentoConsolidatoPi3Exception();

                if (documentoAmministrativoAggregate.Reserved)
                    throw new DocumentoBloccatoPi3Exception(documentoAmministrativoAggregate.ReservedIdUser);

                if (documentoAmministrativoAggregate.Permissions == null ||
                    !documentoAmministrativoAggregate.Permissions.Any(p => p.RightType == Core.AggregateModels.ContentElementAggregate.ValueObjects.ContentElementRightTypesEnum.FullControlAllowed
                    || p.RightType == Core.AggregateModels.ContentElementAggregate.ValueObjects.ContentElementRightTypesEnum.WriteAllowed))
                    throw new UnauthorizedPi3Exception();

                if (documentoAmministrativoAggregate.DatiRegistrazione != null && documentoAmministrativoAggregate.DatiRegistrazione.IsRegistrato)
                    throw new DocumentoProtocollatoPi3Exception();

                var index = request.type == RemoveVersionType.ALL_BUT_LAST_TWO ? 2 : 1;

                if(documentoAmministrativoAggregate.Versions.Count > index)
                {
                    for( var i = 0; i < documentoAmministrativoAggregate.Versions.Count - index; i++)
                    {
                        documentoAmministrativoAggregate.RemoveVersion(documentoAmministrativoAggregate.Versions[i].Id);
                        numVersionRemoved++;
                    }

                    await _documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);
                }

                if (documentoAmministrativoAggregate.Allegati != null && documentoAmministrativoAggregate.Allegati.Count > 0)
                {   
                    foreach (var a in documentoAmministrativoAggregate.Allegati)
                    {
                        var documentoAmministrativoAllegatoAggregate = await this._documentoAmministrativoRepository.Get(idTenant, a.IdDoc.Identiticativo, new ILoadBehavior[1]
                            {
                                new GetDocumentoAmministrativoLoadBehavior()
                                {
                                    LoadProfiles = false,
                                    LoadClassifications = false,
                                    LoadAllegati = false,
                                    LoadAggregazioni = false,
                                    LoadVersions = true,
                                    LoadPermissions = false,
                                    LoadMittentiDestinatari = false,
                                    LoadKeywords = false,
                                    LoadNote = false
                                }
                            });

                        if (documentoAmministrativoAllegatoAggregate.Versions.Count > index)
                        {
                            for (var i = 0; i < documentoAmministrativoAllegatoAggregate.Versions.Count - index; i++)
                            {
                                documentoAmministrativoAllegatoAggregate.RemoveVersion(documentoAmministrativoAllegatoAggregate.Versions[i].Id);
                                numVersionRemoved++;
                            }

                            await _documentoAmministrativoRepository.Update(documentoAmministrativoAllegatoAggregate);
                        }
                    }
                }

                if (numVersionRemoved == 0)
                    throw new VersioniNotFoundPi3Exception();

                result = new ImportResult()
                {
                    DocNumber = request.idProfile,
                    IdProfile = request.idProfile,
                    Outcome = ImportResult.OutcomeEnumeration.OK,
                    Message = string.Format(Resources.LogNumeroVersioniRimosse, numVersionRemoved.ToString())
                };

                await this._webMethodLoggerService.LogOK("DOCUMENTORIMUOVIVERSIONE",
                    idProfile, string.Format(Resources.LogRimuoviVersioneDaGrigioAM, idProfile));
            }
            catch (Pi3Exception ex)
            {
                await this._webMethodLoggerService.LogKO("DOCUMENTORIMUOVIVERSIONE",
                    idProfile, string.Format(Resources.LogRimuoviVersioneDaGrigioAM, idProfile));

                this._logger.LogError(exception: ex, message: ex.Message);
                result = new ImportResult()
                {
                    DocNumber = request.idProfile,
                    IdProfile = request.idProfile,
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                await this._webMethodLoggerService.LogKO("DOCUMENTORIMUOVIVERSIONE",
                    idProfile, string.Format(Resources.LogRimuoviVersioneDaGrigioAM, idProfile));

                this._logger.LogCritical(exception: ex, message: ex.Message);
                result = null;
            }

            return new DocumentoRimuoviVersioniDaGrigioAMResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoRimuoviVersioniDaGrigioAMHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;

        #endregion
    }
}