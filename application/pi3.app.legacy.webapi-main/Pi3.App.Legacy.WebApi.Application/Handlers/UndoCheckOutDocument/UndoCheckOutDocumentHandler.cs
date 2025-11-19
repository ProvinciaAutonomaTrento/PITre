// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndoCheckOutDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.UndoCheckOutDocument;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UndoCheckOutDocument
{
    public class UndoCheckOutDocumentHandler : IRequestHandler<UndoCheckOutDocumentRequest, UndoCheckOutDocumentResult>
    {
        #region Public Members

        public UndoCheckOutDocumentHandler(
            ILogger<UndoCheckOutDocumentHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        }

        public async Task<UndoCheckOutDocumentResult> Handle(UndoCheckOutDocumentRequest request, CancellationToken cancellationToken)
        {
            var brokenRules = new List<DocsPaVO.Validations.BrokenRule>();
            string errorMessage = null!;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);
                var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true);

                var documentoAmministrativoAggregate = await
                    this._documentoAmministrativoRepository.Get(idTenant, request.checkOutStatus.IDDocument);

                if (!documentoAmministrativoAggregate.Reserved)
                    throw new UndoCheckOutDocumentHandlerPi3Exception(ErrorDescriptions.NotCheckedIn);

                if (documentoAmministrativoAggregate.ReservedIdUser != idUser
                    && documentoAmministrativoAggregate.ReservedIdGroup != idGroup)
                    throw new UndoCheckOutDocumentHandlerPi3Exception(ErrorDescriptions.NotCheckedInByUser);

                documentoAmministrativoAggregate.Unreserve();

                await this._documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);
            }
            catch (Pi3Exception pi3Ex)
            {
                errorMessage = pi3Ex.Message;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);

                brokenRules.Add(
                    new DocsPaVO.Validations.BrokenRule
                    {
                        Level = DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error,
                        Description = errorMessage
                    });
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                this._logger.LogCritical(exception: ex, message: ex.Message);

                brokenRules.Add(
                    new DocsPaVO.Validations.BrokenRule
                    {
                        ID = Descriptions.UndoCheckOutErrorCode,
                        Level = DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error,
                        Description = errorMessage
                    });
            }
            finally
            {
                if (!brokenRules.Any())
                {
                    await this._webMethodLoggerService.LogOK(
                         webMethodName: Descriptions.WebMethodName,
                         idObject: request.checkOutStatus.IDDocument,
                         objectDescription: String.Format(Descriptions.ObjectDescription, request.checkOutStatus.IDDocument));
                }
                else
                {
                    await this._webMethodLoggerService.LogKO(
                         webMethodName: Descriptions.WebMethodName,
                         idObject: request.checkOutStatus.IDDocument,
                         objectDescription: String.Format(Descriptions.ObjectDescription, request.checkOutStatus.IDDocument));
                }
            }

            return new UndoCheckOutDocumentResult(
                    new DocsPaVO.Validations.ValidationResultInfo()
                    {
                        Value = !brokenRules.Any(),
                        BrokenRules = brokenRules.ToArray()
                    });
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UndoCheckOutDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;

        #endregion
    }
}