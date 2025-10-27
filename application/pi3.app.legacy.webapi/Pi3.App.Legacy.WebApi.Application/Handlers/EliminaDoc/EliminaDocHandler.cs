// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.InstanceAccess;
using DocsPaVO.Interoperabilita.Semplificata;
using DocsPaVO.Mobile;
using DocsPaVO.Notification;
using DocsPaVO.utente;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.Interoperability;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.EliminaDoc
{

    // Richiede libreria MediatR
    public class EliminaDocHandler : IRequestHandler<Application.Requests.EliminaDoc, EliminaDocResult>
    {
        #region Public Members

        public EliminaDocHandler(ILogger<EliminaDocHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IConfigurationService configurationService,
            IDocumentoAmministrativoRepository documentRepository,
            IInteroperabilityService interopService,
            IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._configurationService = configurationService;
            this._documentRepository = documentRepository;
            this._interopService = interopService;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<EliminaDocResult> Handle(Application.Requests.EliminaDoc request, CancellationToken cancellationToken)
        {
            bool result = false;
            DocsPaVO.documento.InfoDocumento infoDoc = request.infoDoc;

            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            try
            {
                var tenantCode = await this._dbContext.AmministraEntities.Where(x => x.SYSTEM_ID == idTenant).Select(x => x.VAR_CODICE_AMM).FirstOrDefaultAsync();
                long idProfileAsLong = infoDoc.idProfile.AsLong();
                var isConfigConsolidamentoEnabled = await this._configurationService.GetValue<string>("0", "BE_CONSOLIDAMENTO");
                bool canExecuteAction = await CanExecuteAction(infoDoc.idProfile, ConsolidationActionsDeniedEnum.DeleteDocument, true);
                if (!string.IsNullOrEmpty(isConfigConsolidamentoEnabled) && isConfigConsolidamentoEnabled.Equals("1") && canExecuteAction)
                {
                    bool isDocumentReceivedWithIS = (await this._mediator.Send(new Application.Requests.IsDocumentReceivedWithIS(infoDoc.idProfile))).output;
                    string docSignature = await this._dbContext.ProfileEntities.Where(x => x.SYSTEM_ID == idProfileAsLong).Select(x => x.VAR_SEGNATURA).FirstOrDefaultAsync();

                    if (isDocumentReceivedWithIS && !string.IsNullOrEmpty(docSignature))
                        throw new ProtoReceivedWithISPi3Exception();

                    RecordInfo sender = null, receiver = null;
                    String senderUrl = String.Empty, receiverCode = String.Empty;

                    if (isDocumentReceivedWithIS)
                    {
                        var senderRecordInfoEntity = await this._dbContext.SimpInteropReceivedMessageEntities
                            .Where(x => x.PROFILEID == idProfileAsLong)
                            .Select(x => new
                            {
                                SENDER_ADMINISTRATION_CODE = x.SENDERADMINISTRATIONCODE,
                                SENDER_AOO_CODE = x.AOOCODE,
                                SENDER_RECORD_DATE = x.RECORDDATE.AsDateTimeFormat(),
                                SENDER_RECORD_NUMBER = x.RECORDNUMBER,
                                SENDER_URL = x.SENDERURL,
                                RECEIVER_CODE = x.RECEIVERCODE
                            })
                            .FirstOrDefaultAsync();

                        if (senderRecordInfoEntity != null)
                        {
                            sender = new RecordInfo()
                            {
                                AdministrationCode = senderRecordInfoEntity.SENDER_ADMINISTRATION_CODE,
                                AOOCode = senderRecordInfoEntity.SENDER_AOO_CODE,
                                RecordDate = DateTime.Parse(senderRecordInfoEntity.SENDER_RECORD_DATE),
                                RecordNumber = senderRecordInfoEntity.SENDER_RECORD_NUMBER.ToString()
                            };

                            senderUrl = senderRecordInfoEntity?.SENDER_URL ?? string.Empty;
                            receiverCode = senderRecordInfoEntity?.RECEIVER_CODE ?? string.Empty;
                        }

                        var profileReceiverInfoEntity = await this._dbContext.ProfileEntities
                            .Join(this._dbContext.CorrGlobaliEntities, p => p.ID_UO_PROT, cg => cg.SYSTEM_ID, (p, cg) => new { p, cg })
                            .Join(this._dbContext.AmministraEntities, j1 => j1.cg.ID_AMM, a => a.SYSTEM_ID, (j1, a) => new { p = j1.p, cg = j1.cg, a = a })
                            .Join(this._dbContext.RegistroEntities, j2 => j2.p.ID_REGISTRO, r => r.SYSTEM_ID, (j2, r) => new { p = j2.p, cg = j2.cg, a = j2.a, r })
                            .Where(x => x.p.SYSTEM_ID == idProfileAsLong)
                            .Select(x => new
                            {
                                RECEIVER_AOO_CODE = x.r.VAR_CODICE,
                                RECEIVER_RECORD_NUMBER = x.p.NUM_PROTO,
                                RECEIVER_RECORD_DATE = x.p.DTA_PROTO.AsDateTimeFormat(),
                                RECEIVER_ADMINISTRATOR_CODE = x.a.VAR_CODICE_AMM
                            })
                            .FirstOrDefaultAsync();

                        if (profileReceiverInfoEntity != null)
                        {
                            receiver = new RecordInfo()
                            {
                                AdministrationCode = profileReceiverInfoEntity.RECEIVER_ADMINISTRATOR_CODE,
                                AOOCode = profileReceiverInfoEntity.RECEIVER_AOO_CODE,
                                RecordDate = DateTime.Parse(profileReceiverInfoEntity.RECEIVER_RECORD_DATE),
                                RecordNumber = profileReceiverInfoEntity.RECEIVER_RECORD_NUMBER.ToString()
                            };
                        }
                    }

                    if (!infoDoc.allegato)
                    {
                        List<Allegato> allegati = (await this._mediator.Send(new Application.Requests.DocumentoGetAllegati(infoDoc.docNumber, string.Empty, string.Empty))).output.ToList();
                        foreach (DocsPaVO.documento.Allegato allegato in allegati)
                        {
                            var allegatoAggregateToDelete = await this._documentRepository.Get(
                                idTenant.ToString(), 
                                allegato.docNumber,
                                new ILoadBehavior[1] { 
                                    new GetDocumentoAmministrativoLoadBehavior() { 
                                        BypassSecurityCheck = true,
                                        LoadVersions = false,
                                    }
                            });
                            if (allegatoAggregateToDelete == null)
                                throw new AttachmentNotFoundPi3Exception();

                            this._documentRepository.Delete(allegatoAggregateToDelete);
                            int attRowDeleted = await ((DbContext)_dbContext).SaveChangesAsync();
                        }
                    }

                    long docNumberAsLong = infoDoc.docNumber.AsLong();
                    var pathList = await this._dbContext.ComponentEntities
                        .Join(this._dbContext.VersionEntities, c => c.VERSION_ID, v => v.VERSION_ID, (c, v) => new
                        {
                            PATH = c.PATH,
                            DOCNUMBER = c.DOCNUMBER,
                        })
                        .Where(x => x.DOCNUMBER == docNumberAsLong)
                        .Select(x => x.PATH)
                        .ToListAsync();

                    bool removed = await this.Remove(idProfileAsLong, idTenant.ToString());

                    if (removed)
                    {
                        foreach (string path in pathList)
                        {
                            if (File.Exists(path))
                                File.Delete(path);
                        }

                        if (sender != null && receiver != null)
                            await this.SendDocumentDroppedOrExceptionProofToSender(sender, receiver, Resources.NotAdmCompetenceMsg, false, senderUrl, receiverCode, tenantCode, this.GetAuthToken());
                    }
                }

                result = true;
                await this._webMethodLoggerService.LogOK("ELIMINADOC", infoDoc.docNumber, string.Format(Resources.LogEliminaDoc, infoDoc.docNumber));
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("ELIMINADOC", infoDoc.docNumber, string.Format(Resources.LogEliminaDoc, infoDoc.docNumber));
            }

            return new EliminaDocResult(result);
        }



        #endregion

        #region Private Members

        protected readonly ILogger<EliminaDocHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IConfigurationService _configurationService;
        protected readonly IDocumentoAmministrativoRepository _documentRepository;
        protected readonly IInteroperabilityService _interopService;
        protected IHttpContextAccessor _httpContextAccessor;

        private readonly string BearerPrefix = "Bearer ";

        protected string? GetAuthToken()
        {
            if (!this._httpContextAccessor.HttpContext!.Request.Headers.TryGetValue("Authorization", out StringValues authorizationStrings)) { return string.Empty; }

            var authorizationHeader = authorizationStrings[0];// !.StartsWith(BearerPrefix) ? authorizationStrings[0]!.Substring(BearerPrefix.Length) : authorizationStrings[0];

            return authorizationHeader;
        }

        private async Task SendDocumentDroppedOrExceptionProofToSender(RecordInfo sender, RecordInfo receiver, string reason, bool dropped, string senderUrl, string receiverCode, string tenant, string authToken)
        {
            AnalyzeDocumentDroppedOrErrorMessageProofRequest request = new AnalyzeDocumentDroppedOrErrorMessageProofRequest()
            {
                SenderRecordInfo = sender,
                ReceiverRecordInfo = receiver,
                Reason = reason,
                Operation = dropped ? OperationDiscriminator.Drop : OperationDiscriminator.Error,
                ReceiverCode = receiverCode,
                ReceiverUrl = senderUrl
            };

            var senderUrlSplit = new Uri(senderUrl).Segments;
            var instance = (senderUrlSplit[Array.IndexOf(senderUrlSplit, "InteroperabilityService") - 1]).Replace("/", "");

            await this._interopService.AnalyzeDocumentDroppedOrErrorMessageProof(instance, authToken, tenant, request);
        }


        private async Task<bool> Remove(long idProfileAsLong, string idTenant)
        {
            var transaction = await ((DbContext)this._dbContext).Database.BeginTransactionAsync();
            try
            {
                //1. Rimozione trasmissioni
                var trasmToDelete = await this._dbContext.TrasmissioneEntities.Where(x => x.ID_PROFILE == idProfileAsLong).ToListAsync();
                var TrasmSingoleToDelete = await this._dbContext.TrasmSingolaEntities.Where(x => trasmToDelete.Select(x => x.SYSTEM_ID).Contains((long)x.ID_TRASMISSIONE)).ToListAsync();
                var TrasmUtenteToDelete = await this._dbContext.TrasmUtenteEntities.Where(x => TrasmSingoleToDelete.Select(x => x.SYSTEM_ID).Contains((long)x.ID_TRASM_SINGOLA)).ToListAsync();

                this._dbContext.TrasmUtenteEntities.RemoveRange(TrasmUtenteToDelete);
                this._dbContext.TrasmSingolaEntities.RemoveRange(TrasmSingoleToDelete);
                this._dbContext.TrasmissioneEntities.RemoveRange(trasmToDelete);

                //2. Rimozione classifiche/fascicoli
                var projectComponentsToDelete = await this._dbContext.ProjectComponentEntities.Where(x => x.LINK == idProfileAsLong).ToListAsync();

                this._dbContext.ProjectComponentEntities.RemoveRange(projectComponentsToDelete);

                //3.Rimozione versioni
                var versionsToDelete = await this._dbContext.VersionEntities.Where(x => x.DOCNUMBER == idProfileAsLong).ToListAsync();

                this._dbContext.VersionEntities.RemoveRange(versionsToDelete);

                //4. Rimozione components
                var componentsToDelete = await this._dbContext.ComponentEntities.Where(x => x.DOCNUMBER == idProfileAsLong).ToListAsync();

                this._dbContext.ComponentEntities.RemoveRange(componentsToDelete);

                //5. Rimozione area lavoro
                var areaLavoroEntities = await this._dbContext.AreaLavoroEntities.Where(x => x.ID_PROFILE == idProfileAsLong).ToListAsync();

                this._dbContext.AreaLavoroEntities.RemoveRange(areaLavoroEntities);

                //6. Rimozione profParole
                var profParoleEntities = await this._dbContext.ProfParoleEntities.Where(x => x.ID_PROFILE == idProfileAsLong).ToListAsync();

                this._dbContext.ProfParoleEntities.RemoveRange(profParoleEntities);


                //8. Rimozione security
                var securityEntities = await this._dbContext.SecurityEntities.Where(x => x.THING == idProfileAsLong).ToListAsync();

                this._dbContext.SecurityEntities.RemoveRange(securityEntities);

                //9. Rimozione todolist
                await ((DbContext)this._dbContext)
                 .Database
                 .ExecuteSqlRawAsync("DELETE FROM dpa_todolist WHERE id_profile = {0}", idProfileAsLong);

                //10. Rimozione notifiche
                var notifyEntities = await this._dbContext.NotifyEntities.Where(x => x.ID_OBJECT == idProfileAsLong).ToListAsync();

                this._dbContext.NotifyEntities.RemoveRange(notifyEntities);

                //11. Rimozione diagrammi
                var diagrammiEntities = await this._dbContext.DiagrammiEntities.Where(x => x.DOC_NUMBER == idProfileAsLong).ToListAsync();

                this._dbContext.DiagrammiEntities.RemoveRange(diagrammiEntities);

                //12. Rimozione infofile
                var infoFileEntities = await this._dbContext.InfoFileEntities.Where(x => x.ID_PROFILE == idProfileAsLong).ToListAsync();

                this._dbContext.InfoFileEntities.RemoveRange(infoFileEntities);

                //13. Rimozione campi tipologia
                var assTemplatesEntities = await this._dbContext.AssociazioneTemplatesEntities.Where(x => x.DOC_NUMBER == idProfileAsLong.ToString()).ToListAsync();

                this._dbContext.AssociazioneTemplatesEntities.RemoveRange(assTemplatesEntities);

                //7. Rimozione profile
                var profileEntity = await this._dbContext.ProfileEntities.Where(x => x.SYSTEM_ID == idProfileAsLong).FirstOrDefaultAsync();

                this._dbContext.ProfileEntities.Remove(profileEntity);

                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            catch(Exception ex)
            {
               await transaction.RollbackAsync();
               throw ex;
            }
            await transaction.CommitAsync();
            return true;
        }

        private async Task<bool> CanExecuteAction(string idProfile, Enum action, bool throwOnError)
        {
            long idProfileAsLong = idProfile.AsLong();
            DocsPaVO.documento.DocumentConsolidationStateInfo actualState = await GetState(idProfileAsLong);
            if (actualState != null && actualState.State == DocsPaVO.documento.DocumentConsolidationStateEnum.None)
                return true;

            DocsPaVO.documento.DocumentConsolidationStateEnum actionApplyState = DocumentConsolidationAttribute.GetState(action);

            bool canExecute = (actualState.State < actionApplyState);

            if (!canExecute && throwOnError)
                throw new ConsolidatedStatePi3Exception();

            return canExecute;
        }

        private async Task<DocumentConsolidationStateInfo> GetState(long idProfile)
        {

            var state = await this._dbContext.ProfileEntities.Where(x => x.SYSTEM_ID == idProfile).Select(x => new
            {
                CONSOLIDATION_STATE = x.CONSOLIDATION_STATE,
                CONSOLIDATION_AUTHOR = x.CONSOLIDATION_AUTHOR,
                CONSOLIDATION_ROLE = x.CONSOLIDATION_ROLE,
                CONSOLIDATION_DATE = x.CONSOLIDATION_DATE
            }).FirstOrDefaultAsync();

            var stateInfo = new DocumentConsolidationStateInfo()
            {
                State = !string.IsNullOrEmpty(state.CONSOLIDATION_STATE) ? (DocsPaVO.documento.DocumentConsolidationStateEnum)Enum.Parse(typeof(DocsPaVO.documento.DocumentConsolidationStateEnum), state.CONSOLIDATION_STATE, true)
                    : DocumentConsolidationStateEnum.None,
                Author = state.CONSOLIDATION_AUTHOR?.ToString(),
                Role = state.CONSOLIDATION_ROLE?.ToString(),
                Date = state.CONSOLIDATION_DATE?.ToString()
            };


            return stateInfo;
        }

        private enum ConsolidationActionsDeniedEnum
        {
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            AddVersions,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            RemoveVersions,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            ModifyVersions,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            AddAttatchments,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            RemoveAttatchments,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            ModifyAttatchments,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            DeleteDocument,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            SignDocument,
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
            PrepareProtocol,            // Predisponi alla protocollazione
            [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step2)]
            CancelProtocol,             // Annullamento protocollo
        }

        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
        private class DocumentConsolidationAttribute : Attribute
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="state"></param>
            public DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum state)
            {
                this.State = state;
            }

            /// <summary>
            /// Stato di consolidamento del documento
            /// </summary>
            public DocsPaVO.documento.DocumentConsolidationStateEnum State
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="enumValue"></param>
            /// <returns></returns>
            public static DocsPaVO.documento.DocumentConsolidationStateEnum GetState(Enum enumValue)
            {
                FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

                DocumentConsolidationAttribute[] attributes = (DocumentConsolidationAttribute[])
                        fi.GetCustomAttributes(typeof(DocumentConsolidationAttribute), false);

                if (attributes.Length > 0)
                    return attributes[0].State;
                else
                    return DocsPaVO.documento.DocumentConsolidationStateEnum.None;
            }
        }
        #endregion
    }

}
