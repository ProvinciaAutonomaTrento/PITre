// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.CheckInOut;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckOutDocumentWithFile;
using Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoAddParolaChiave;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
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
using CheckOutDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.CheckOutDocument;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckOutDocument
{
    public class CheckOutDocumentHandler : IRequestHandler<CheckOutDocumentRequest, CheckOutDocumentResult>
    {
        #region Public Members

        public CheckOutDocumentHandler(
            ILogger<CheckOutDocumentWithFileHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        }

        public async Task<CheckOutDocumentResult> Handle(CheckOutDocumentRequest request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.Validations.BrokenRule> brokenRules = new List<DocsPaVO.Validations.BrokenRule>();
            string errorMessage = null!;
            CheckOutStatus checkOutStatus = null!;
            DocumentoAmministrativo documentoAmministrativoAggregate = null!;
            bool wasCheckedOut = false;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

                documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(idTenant, request.idDocument);

                if (documentoAmministrativoAggregate.Consolidamento != null
                    && documentoAmministrativoAggregate.Consolidamento.Stato == Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.StatiConsolidamentoEnum.Livello1)
                {
                    // Il documento � in stato consolidato
                    brokenRules.Add(new DocsPaVO.Validations.BrokenRule(Descriptions.DocumentoConsolidatoCode, ErrorDescriptions.DocumentoConsolidato));
                }
                else if (documentoAmministrativoAggregate.Reserved)
                {
                    // Il documento � gi� in checkout
                    brokenRules.Add(new DocsPaVO.Validations.BrokenRule(Descriptions.DocumentoGiaInCheckOutCode, ErrorDescriptions.DocumentoGiaInCheckOut));
                }
                else if (await (from p in this._pi3DbContext.ProfileEntities
                             join c in this._pi3DbContext.CheckinCheckoutEntities
                                 on p.SYSTEM_ID equals c.ID_DOCUMENT
                             where p.ID_DOCUMENTO_PRINCIPALE == documentoAmministrativoAggregate.Id.AsLong()
                             select p.SYSTEM_ID)
                            .AnyAsync())
                {
                    // Almeno uno degli allegati � gi� in checkout
                    brokenRules.Add(new DocsPaVO.Validations.BrokenRule(Descriptions.DocumentoGiaInCheckOutCode, ErrorDescriptions.DocumentoGiaInCheckOut));
                }

                // Checkout del documento
                documentoAmministrativoAggregate.Reserve(
                    reserveDate: DateTime.Now,
                    reserveIdUser: request.utente.idPeople,
                    reserveIdGroup: request.utente.idGruppo,
                    documentLocation: request.documentLocation,
                    machineName: request.machineName);

                await this._documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);

                wasCheckedOut = true;

                checkOutStatus = new CheckOutStatus()
                {
                    ID = await _pi3DbContext.CheckinCheckoutEntities
                            .AsNoTracking()
                            .Where(p => p.ID_DOCUMENT == documentoAmministrativoAggregate.Id.AsLong())
                            .Select(p => p.SYSTEM_ID.ToString())
                            .FirstAsync(),
                    IDDocument = documentoAmministrativoAggregate.Id.ToString(),
                    DocumentNumber = request.documentNumber,
                    IDUser = documentoAmministrativoAggregate.ReservedIdUser,
                    UserName = await this._pi3DbContext.PeopleEntities
                        .AsNoTracking()
                        .Where(p => p.SYSTEM_ID == documentoAmministrativoAggregate.ReservedIdUser.AsLong())
                        .Select(p => p.USER_ID)
                        .FirstAsync(),
                    IDRole = documentoAmministrativoAggregate.ReservedIdGroup,
                    RoleName = await this._pi3DbContext.GroupEntities
                        .AsNoTracking()
                        .Where(p => p.SYSTEM_ID == documentoAmministrativoAggregate.ReservedIdGroup.AsLong())
                        .Select(p => p.GROUP_ID)
                        .FirstAsync(),
                    CheckOutDate = documentoAmministrativoAggregate.ReservedDate.Value,
                    DocumentLocation = documentoAmministrativoAggregate.ReservedDocumentLocation,
                    Segnature = documentoAmministrativoAggregate.IdDoc.Segnatura,
                    MachineName = documentoAmministrativoAggregate.ReservedMachineName,
                    IsAllegato = false,
                    InConversionePdf = await this._pi3DbContext.ConvPdfServerEntities
                                .AsNoTracking()
                                .Where(c => c.ID_PROFILE == documentoAmministrativoAggregate.Id.AsLong())
                                .Select(c => c.SYSTEM_ID)
                                .AnyAsync()
                };
            }
            catch (Pi3Exception pi3Ex)
            {
                errorMessage = pi3Ex.Message;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);

                brokenRules.Add(new DocsPaVO.Validations.BrokenRule { Level = DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error, Description = errorMessage });
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                this._logger.LogCritical(exception: ex, message: ex.Message);

                brokenRules.Add(new DocsPaVO.Validations.BrokenRule { Level = DocsPaVO.Validations.BrokenRule.BrokenRuleLevelEnum.Error, Description = errorMessage });
            }
            finally
            {
                if (!brokenRules.Any())
                {
                    await this._webMethodLoggerService.LogOK(
                         webMethodName: Descriptions.WebMethodName,
                         idObject: request.documentNumber,
                         objectDescription: String.Format(Descriptions.ObjectDescription, request.documentNumber));
                }
                else
                {
                    if (wasCheckedOut)
                    {
                        // In caso di errore, annulla l'eventuale checkout
                        try
                        {
                            documentoAmministrativoAggregate.Unreserve();
                            await this._documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);
                        }
                        catch
                        {}
                    }

                    await this._webMethodLoggerService.LogKO(
                         webMethodName: Descriptions.WebMethodName,
                         idObject: request.documentNumber,
                         objectDescription: String.Format(Descriptions.ObjectDescription, request.documentNumber));
                }
            }

            return new CheckOutDocumentResult(
                new DocsPaVO.Validations.ValidationResultInfo()
                {
                    Value = !brokenRules.Any(),
                    BrokenRules = brokenRules.ToArray()
                },
                checkOutStatus);
        }

        #endregion

        #region Private Members


        protected readonly ILogger<CheckOutDocumentWithFileHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;

        #endregion
    }
}