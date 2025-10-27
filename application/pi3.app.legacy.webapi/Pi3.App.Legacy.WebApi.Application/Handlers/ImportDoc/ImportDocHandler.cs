// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.PrjDocImport;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.WebApi.Application.Handlers.ImportAndAcquireDocument;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImportDocRequest = Pi3.App.Legacy.WebApi.Application.Requests.ImportDoc;
using DocsPaVO.utente;
using DocsPaVO.documento;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.App.Legacy.WebApi.Infrastructure.EF.Extensions;
using DocsPaVO.ProfilazioneDinamica;
using Microsoft.EntityFrameworkCore;
using Pi3.Core.Extensions;
using System.Collections;
using LinqKit;
using DocsPaVO.filtri;
using DocsPaVO.addressbook;
using DocsPaVO.rubrica;
using DocsPaVO.Note;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using System.Text.RegularExpressions;
using DocsPaVO.amministrazione;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Office2016.Excel;


namespace Pi3.App.Legacy.WebApi.Application.Handlers.ImportDoc
{
    public class ImportDocHandler : IRequestHandler<ImportDocRequest, ImportDocResult>
    {
        #region public members
        public ImportDocHandler(ILogger<ImportAndAcquireDocumentHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            IRubricaComuneService rubricaComuneService,
            IHttpContextAccessor httpContextAccessor,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository

        )
        {
            this._rubricaComuneService = rubricaComuneService;
            this._httpContextAccessor = httpContextAccessor;
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
        }

        public async Task<ImportDocResult> Handle(ImportDocRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.PrjDocImport.ImportResult output = null;
            DocsPaVO.PrjDocImport.ResultsContainer resultsContainer = request.resultsContainer;
            try
            {
                (output, resultsContainer) = await this.Import(
                    request.documentToImport, request.userInfo, request.role, request.serverPath,
                    request.isProfilationRequired, request.isRapidClassificationRequired, string.Empty,
                    request.isSmistamentoEnabled, request.protoType, request.resultsContainer, string.Empty, 
                    string.Empty, request.isEnabledPregressi, request.protoType == ProtoType.G,request.isImportAndAcquire);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = new DocsPaVO.PrjDocImport.ImportResult()
                {
                    Message = ex.Message,
                    Outcome = DocsPaVO.PrjDocImport.ImportResult.OutcomeEnumeration.KO
                };
            }
            return new(output, resultsContainer);
        }

        #endregion

        protected readonly ILogger<ImportAndAcquireDocumentHandler> _logger;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected IRubricaComuneService _rubricaComuneService;
        protected IHttpContextAccessor _httpContextAccessor;
        private readonly string BearerPrefix = "Bearer ";

        protected string? GetAuthToken()
        {
            //if (!this._httpContextAccessor.HttpContext!.Request.Headers.TryGetValue("Authorization", out StringValues authorizationStrings)) { return string.Empty; }

            //var authorizationHeader = authorizationStrings[0]!.StartsWith(BearerPrefix) ? authorizationStrings[0]!.Substring(BearerPrefix.Length) : authorizationStrings[0];
            var authorizationHeader = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>("KeyToken");

            return authorizationHeader;
        }

        private async Task<(ImportResult, ResultsContainer)> Import(
            DocumentRowData documentRowData,
            InfoUtente userInfo,
            Ruolo role,
            string serverPath,
            bool isProfilationRequired,
            bool isRapidClassificationRequired,
            string ftpAddress,
            bool isSmistamentoEnabled,
            ProtoType protoType,
            ResultsContainer resultsContainer,
            string ftpUsername,
            string ftpPassword,
            bool isEnabledPregressi,
            bool isGray,
            bool isImportAndAcquire
            )
        {
            ImportResult output = null;


            switch (protoType)
            {
                case ProtoType.ATT:
                    if (resultsContainer != null)
                        output = await this.ImportAttachment(
                            documentRowData,
                            userInfo,
                            role,
                            ftpAddress,
                            resultsContainer,
                            ftpUsername,
                            ftpPassword);
                    break;

                case ProtoType.A:
                case ProtoType.G:
                case ProtoType.I:
                case ProtoType.P:
                    output = await this.ImportDocuments(documentRowData,
                        userInfo,
                        role,
                        serverPath,
                        isProfilationRequired,
                        isRapidClassificationRequired,
                        ftpAddress,
                        isGray,
                        isSmistamentoEnabled,
                        protoType.ToString(),
                        ftpUsername,
                        ftpPassword,
                        isEnabledPregressi,
                        protoType, isImportAndAcquire, isImportAndAcquire);
                    break;
            }

            return (output, resultsContainer);
        }

        private async Task<ImportResult> ImportDocuments(
            DocumentRowData documentRowData,
            InfoUtente userInfo,
            Ruolo role,
            string serverPath,
            bool isProfilationRequired,
            bool isRapidClassificationRequired,
            string ftpAddress,
            bool isGray,
            bool isSmistamentoEnabled,
            string protoType,
            string ftpUsername,
            string ftpPassword,
            bool isEnabledPregressi,
            ProtoType pType,
            bool isImportAndAcquire,
            bool isStampaUnione = false)
        {
            ImportResult output = new ImportResult();
            List<string> creationProblems = new();

            if (documentRowData == null)
            {
                output = new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.OK,
                    Message = Resources.NoDocToImport
                };
            }

            #region Controllo abilitazione ruolo
            (bool hasAuth, string msg) = this.HasAuth(protoType, role);
            if (!hasAuth)
            {
                output = new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.KO,
                    Message = msg,
                    OtherInformation = creationProblems,
                    Ordinal = documentRowData.OrdinalNumber
                };
                return output;
            }
            #endregion

            bool dataIsValid = true;
            List<string> problems = new();

            switch (pType)
            {
                case ProtoType.A:
                    (dataIsValid, problems) = await this.IsPrArrDocumentValid(documentRowData, isProfilationRequired, isRapidClassificationRequired);
                    break;
                case ProtoType.G:
                    (dataIsValid, problems) = this.IsGrayDataValid(documentRowData, isProfilationRequired, isRapidClassificationRequired);
                    break;
                case ProtoType.I:
                    (dataIsValid, problems) = await this.IsPrIntDocumentValid(documentRowData, isProfilationRequired, isRapidClassificationRequired);
                    break;
                case ProtoType.P:
                    (dataIsValid, problems) = await this.IsPrPDocumentValid(documentRowData, isProfilationRequired, isRapidClassificationRequired);
                    break;
                default:
                    break;
            }
            output.OtherInformation.AddRange(problems);

            if (dataIsValid)
            {
                try
                {
                    var creationOutcome = await this.CreateDocument(documentRowData, userInfo, role, serverPath, isGray, isRapidClassificationRequired, ftpAddress, isSmistamentoEnabled, protoType, ftpUsername, ftpPassword, isEnabledPregressi, isImportAndAcquire, isStampaUnione);

                    creationProblems = creationOutcome.Problems;

                    output.OtherInformation.AddRange(creationProblems);

                    if (output.OtherInformation.Count > 0)
                    {
                        if (creationOutcome.FileAcquired)
                        {
                            output.Outcome = ImportResult.OutcomeEnumeration.Warnings;
                            output.Message = string.Format(Trasm.FileAc, creationOutcome.IdentificationData);
                        }
                        else
                        {
                            output.Outcome = ImportResult.OutcomeEnumeration.FileNotAcquired;
                            output.Message = string.Format(Trasm.DocCreatedFileAcFailed, creationOutcome.IdentificationData);
                        }
                    }
                    else
                    {
                        output.Outcome = ImportResult.OutcomeEnumeration.OK;
                        output.Message = string.Format(Trasm.ImportOk, creationOutcome.IdentificationData);
                    }

                    output.DocNumber = creationOutcome.Docnumber;
                    output.Ordinal = documentRowData.OrdinalNumber;
                }
                catch (Pi3Exception ex)
                {
                    output = new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = ex.Message,
                        Ordinal = documentRowData.OrdinalNumber
                    };
                }
                catch (Exception ex)
                {
                    output = new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = string.Empty,
                        Ordinal = documentRowData.OrdinalNumber
                    };
                }                
            }

            return output;
        }

        private class CreationDocumentRes
        {
            public List<string> Problems { get; set; }
            public string IdentificationData { get; set; }
            public string Docnumber { get; set; }
            public string IdProfile { get; set; }
            public bool FileAcquired { get; set; }
        }

        private async Task<CreationDocumentRes> CreateDocument(
            DocumentRowData rowData,
            InfoUtente userInfo,
            Ruolo role,
            string serverPath,
            bool isGray,
            bool isRapidClassificationRequired,
            string ftpAddress,
            bool isSmistamentoEnabled,
            string protoType,
            string ftpUsername,
            string ftpPassword,
            bool isEnabledPregressi,
            bool isImportAndAcquire,
            bool isStampaUnione = false)
        {
            string identificationData = string.Empty;
            string docNumber = string.Empty;
            string idProfile = string.Empty;
            bool fileAcquired = false;
            List<string> problems = new List<string>();
            string administrationSyd = (await this._mediator.Send(new Application.Requests.getIdAmmByCod(rowData.AdminCode))).output;
            string registrySyd = await this.GetIdRegistro(administrationSyd, rowData.RegCode);
            string rfSyd = string.Empty;
            string titolarioSyd = string.Empty;

            switch (protoType.ToUpper())
            {
                case "A":
                case "P":
                case "I":
                    isGray = false;
                    break;
            }

            if ("0".Equals(administrationSyd) || string.IsNullOrEmpty(administrationSyd))
                throw new NoAdmFoundPi3Exception(string.Format(Resources.CantFetchDataAmm,rowData.AdminCode));

            if (!string.IsNullOrEmpty(rowData.RFCode))
            {
                rfSyd = await this.GetRfId(rowData.RFCode, administrationSyd);
            }

            titolarioSyd = await this.GetIdTit(rowData.Titolario, administrationSyd);

            var res = await this.GetDocScheda(rowData, userInfo, role, administrationSyd, registrySyd, rfSyd, protoType, isSmistamentoEnabled, isEnabledPregressi);
            var schedaDocumento = res.Item1;
            problems.AddRange(res.Item2);
            bool existsProtocol = false;

            if (!isGray)
            {
                if (!protoType.Equals("A"))
                {
                    existsProtocol = (await this._mediator.Send(new Application.Requests.DocumentoCercaDuplicatiInfo(schedaDocumento, null))).output.Equals(DocsPaVO.documento.RicercaDuplicati.EsitoRicercaDuplicatiEnum.NessunDuplicato);

                }
            }
            if (existsProtocol)
                throw new Exception(Resources.ProtoExists);

            List<string> projectIds = new();

            if ((rowData.ProjectCodes != null && rowData.ProjectCodes.Length > 0) ||
                !string.IsNullOrEmpty(rowData.ProjectDescription) ||
                !string.IsNullOrEmpty(rowData.FolderDescrition) ||
                !string.IsNullOrEmpty(rowData.NodeCode) ||
                !string.IsNullOrEmpty(rowData.ProjectTipology))
            {
                var projRes = await this.GetProjectsForClassification(rowData, userInfo, role, administrationSyd, registrySyd, rfSyd, titolarioSyd, isSmistamentoEnabled);
                projectIds = projRes.Item1;
                problems.AddRange(projRes.Item2);
            }

            if (projectIds.Count == 0 && isRapidClassificationRequired)
                throw new NoProjectFoundPi3Exception(Resources.NoViableFascFound);

            if (schedaDocumento.template != null && schedaDocumento.template.SYSTEM_ID != 0)
            {
                (string? val, bool keyFound) = await this._configurationService.TryGetValue<string>(userInfo.idAmministrazione, "ENABLE_PIANO_CONSERVAZIONE");
                if (keyFound && string.IsNullOrEmpty(val) && val.Equals("1"))
                {
                    var templates = await this.GetTipoAttoByRuoloAndPianoConsIdFasc(administrationSyd, role.idGruppo, "2", projectIds);
                    if (templates != null && templates.Count > 0)
                    {
                        TipologiaAtto template = (from t in templates where t.systemId.Equals(schedaDocumento.template.SYSTEM_ID.ToString()) select t).FirstOrDefault();
                        if (template == null)
                        {
                            throw new Exception(Resources.DocCantBeTyped);
                        }
                    }
                }
            }

            bool isPredisposto = rowData.Predisposto;

            // 4. Creazione documento, allegato o protocollo
            if (!isGray && !isPredisposto && isStampaUnione)
            {
                schedaDocumento = (await this._mediator.Send(new Application.Requests.DocumentoAddDocGrigia(schedaDocumento, userInfo, role))).output;                

                if (!isGray)
                {
                    var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                    var documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(idTenant, schedaDocumento.systemId, new ILoadBehavior[1]
                   {
                        new GetDocumentoAmministrativoLoadBehavior()
                        {
                            LoadProfiles = true,
                            LoadProfilesMetadata = true,
                            LoadClassifications = true,
                            LoadAllegati = true,
                            LoadAggregazioni = true,
                            LoadVersions = true,
                            LoadPermissions = true,
                            LoadMittentiDestinatari = true,
                            LoadKeywords = true,
                            LoadNote = true
                        }
                   });

                    documentoAmministrativoAggregate.Predisponi((TipologiaFlussoEnum)schedaDocumento.tipoProto.AsTipologiaFlusso());

                    var tipologiaFlusso = schedaDocumento.tipoProto.AsTipologiaFlusso();
                    switch (tipologiaFlusso)
                    {
                        case TipologiaFlussoEnum.E:
                            var protocolloEntrata = (ProtocolloEntrata)schedaDocumento.protocollo;

                            if (!string.IsNullOrEmpty(schedaDocumento.docNumber) && protocolloEntrata.daAggiornareMittente)
                            {
                                documentoAmministrativoAggregate.ChangeMittente(new Mittente(new PG()
                                {
                                    DenominazioneUfficio = new TextValue(protocolloEntrata.mittente.descrizione),
                                },
                                    protocolloEntrata.mittente.systemId));
                            }
                            else
                            {
                                documentoAmministrativoAggregate.AssignMittente(new Mittente(new PG()
                                {
                                    DenominazioneUfficio = new TextValue(protocolloEntrata.mittente.descrizione),
                                },
                                protocolloEntrata.mittente.systemId));
                            }


                            if (protocolloEntrata.daAggiornareMittentiMultipli)
                            {
                                documentoAmministrativoAggregate.MittentiMultipli.ForEach(mm =>
                                {
                                    documentoAmministrativoAggregate.RemoveMittenteMultiplo(mm);
                                });
                            }
                            if (protocolloEntrata.mittenti == null)
                                protocolloEntrata.mittenti = new DocsPaVO.utente.Corrispondente[0];
                            protocolloEntrata.mittenti.ForEach(mm =>
                            {
                                documentoAmministrativoAggregate.AddMittenteMultiplo(new Mittente(
                                new PG()
                                {
                                    DenominazioneUfficio = new TextValue(mm.descrizione)
                                },
                                mm.systemId));
                            });

                            if (protocolloEntrata.mittenteIntermedio != null)
                            {
                                if (protocolloEntrata.daAggiornareMittenteIntermedio)
                                    documentoAmministrativoAggregate.RemoveMittenteIntermedio();

                                documentoAmministrativoAggregate.AssignMittenteIntermedio(new Mittente(
                                   new PG()
                                   {
                                       DenominazioneUfficio = new TextValue(protocolloEntrata.mittenteIntermedio.descrizione)
                                   },
                                   protocolloEntrata.mittenteIntermedio.systemId));
                            }
                            break;
                        case TipologiaFlussoEnum.U:
                        case TipologiaFlussoEnum.I:
                            var protocollo = tipologiaFlusso == TipologiaFlussoEnum.U ? (ProtocolloUscita)schedaDocumento.protocollo : (ProtocolloInterno)schedaDocumento.protocollo;

                            if (protocollo.mittente != null)
                                documentoAmministrativoAggregate.AssignMittente(new Mittente(new PG()
                                {
                                    DenominazioneUfficio = new TextValue(protocollo.mittente.descrizione),
                                },
                                protocollo.mittente.systemId));

                            protocollo.destinatari.ForEach(d =>
                            {
                                documentoAmministrativoAggregate.AddDestinatario(new Destinatario(
                                new PG()
                                {
                                    DenominazioneUfficio = new TextValue(d.descrizione)
                                },
                                d.systemId));
                            });

                            if (protocollo.destinatariConoscenza == null)
                                protocollo.destinatariConoscenza = new DocsPaVO.utente.Corrispondente[0];
                            protocollo.destinatariConoscenza.ForEach(dcc =>
                            {
                                documentoAmministrativoAggregate.AddDestinatarioCc(new Destinatario(
                                    new PG()
                                    {
                                        DenominazioneUfficio = new TextValue(dcc.descrizione)
                                    },
                                    dcc.systemId));
                            });
                            break;
                    }

                    await _documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);
                }
            }
            else
            {
                schedaDocumento = (await this._mediator.Send(new Application.Requests.DocumentoAddDocGrigia(schedaDocumento, userInfo, role))).output;

                fileAcquired = true;
                try
                {
                    if (isImportAndAcquire)
                    {
                        await this.AcquireFileFromModel(rowData, userInfo, role, isSmistamentoEnabled, schedaDocumento, ftpAddress, ftpUsername, ftpPassword);
                    }
                    else
                    {
                        await this.AcquireFile(rowData, userInfo, role, isSmistamentoEnabled, schedaDocumento, ftpAddress, ftpUsername, ftpPassword);
                    }
                }
                catch (Exception ex)
                {
                    fileAcquired = false;
                    problems.Add(ex.Message);
                    this._logger.LogError(exception: ex, message: ex.Message);
                }

                if (!isGray)
                {
                    var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                    var documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(idTenant, schedaDocumento.systemId, new ILoadBehavior[1]
                   {
                        new GetDocumentoAmministrativoLoadBehavior()
                        {
                            LoadProfiles = true,
                            LoadProfilesMetadata = true,
                            LoadClassifications = true,
                            LoadAllegati = true,
                            LoadAggregazioni = true,
                            LoadVersions = true,
                            LoadPermissions = true,
                            LoadMittentiDestinatari = true,
                            LoadKeywords = true,
                            LoadNote = true
                        }
                   });

                    documentoAmministrativoAggregate.Predisponi((TipologiaFlussoEnum)schedaDocumento.tipoProto.AsTipologiaFlusso());

                    var tipologiaFlusso = schedaDocumento.tipoProto.AsTipologiaFlusso();
                    switch (tipologiaFlusso)
                    {
                        case TipologiaFlussoEnum.E:
                            var protocolloEntrata = (ProtocolloEntrata)schedaDocumento.protocollo;

                            if (!string.IsNullOrEmpty(schedaDocumento.docNumber) && protocolloEntrata.daAggiornareMittente)
                            {
                                documentoAmministrativoAggregate.ChangeMittente(new Mittente(new PG()
                                {
                                    DenominazioneUfficio = new TextValue(protocolloEntrata.mittente.descrizione),
                                },
                                    protocolloEntrata.mittente.systemId));
                            }
                            else
                            {
                                documentoAmministrativoAggregate.AssignMittente(new Mittente(new PG()
                                {
                                    DenominazioneUfficio = new TextValue(protocolloEntrata.mittente.descrizione),
                                },
                                protocolloEntrata.mittente.systemId));
                            }

                            if (protocolloEntrata.daAggiornareMittentiMultipli)
                            {
                                documentoAmministrativoAggregate.MittentiMultipli.ForEach(mm =>
                                {
                                    documentoAmministrativoAggregate.RemoveMittenteMultiplo(mm);
                                });
                            }
                            if (protocolloEntrata.mittenti == null)
                                protocolloEntrata.mittenti = new DocsPaVO.utente.Corrispondente[0];
                            protocolloEntrata.mittenti.ForEach(mm =>
                            {
                                documentoAmministrativoAggregate.AddMittenteMultiplo(new Mittente(
                                new PG()
                                {
                                    DenominazioneUfficio = new TextValue(mm.descrizione)
                                },
                                mm.systemId));
                            });

                            if (protocolloEntrata.mittenteIntermedio != null)
                            {
                                if (protocolloEntrata.daAggiornareMittenteIntermedio)
                                    documentoAmministrativoAggregate.RemoveMittenteIntermedio();

                                documentoAmministrativoAggregate.AssignMittenteIntermedio(new Mittente(
                                   new PG()
                                   {
                                       DenominazioneUfficio = new TextValue(protocolloEntrata.mittenteIntermedio.descrizione)
                                   },
                                   protocolloEntrata.mittenteIntermedio.systemId));
                            }
                            break;
                        case TipologiaFlussoEnum.U:
                        case TipologiaFlussoEnum.I:
                            var protocollo = tipologiaFlusso == TipologiaFlussoEnum.U ? (ProtocolloUscita)schedaDocumento.protocollo : (ProtocolloInterno)schedaDocumento.protocollo;
                            
                            if (protocollo.mittente != null)
                                documentoAmministrativoAggregate.AssignMittente(new Mittente(new PG()
                                {
                                    DenominazioneUfficio = new TextValue(protocollo.mittente.descrizione),
                                },
                                protocollo.mittente.systemId));
                            
                            protocollo.destinatari.ForEach(d =>
                            {
                                documentoAmministrativoAggregate.AddDestinatario(new Destinatario(
                                new PG()
                                {
                                    DenominazioneUfficio = new TextValue(d.descrizione)
                                },
                                d.systemId));
                            });

                            if (protocollo.destinatariConoscenza == null)
                                protocollo.destinatariConoscenza = new DocsPaVO.utente.Corrispondente[0];
                            protocollo.destinatariConoscenza.ForEach(dcc =>
                            {
                                documentoAmministrativoAggregate.AddDestinatarioCc(new Destinatario(
                                    new PG()
                                    {
                                        DenominazioneUfficio = new TextValue(dcc.descrizione)
                                    },
                                    dcc.systemId));
                            });
                            break;
                    }

                    await _documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);
                }
            }

            // 6. Fascicolazione
            await this.AddDocToProjects(userInfo, schedaDocumento.systemId, projectIds, isRapidClassificationRequired);

            //Protocollo i documenti
            if ( !isPredisposto)
            {
                switch (protoType.ToUpper())
                {
                    case "A":
                    case "P":
                    case "I":
                        schedaDocumento = await this.GetDataProtocollo(schedaDocumento);
                        if (!fileAcquired && !isImportAndAcquire)
                        {
                            problems.Add(Resources.DocNonAcqu);
                        }
                        else
                        {
                            try
                            {
                                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                                var documentoAmministrativoAggregate = await this._documentoAmministrativoRepository.Get(idTenant, schedaDocumento.systemId, new ILoadBehavior[1]
                               {
                                    new GetDocumentoAmministrativoLoadBehavior()
                                    {
                                        LoadProfiles = true,
                                        LoadProfilesMetadata = true,
                                        LoadClassifications = true,
                                        LoadAllegati = true,
                                        LoadAggregazioni = true,
                                        LoadVersions = true,
                                        LoadPermissions = true,
                                        LoadMittentiDestinatari = true,
                                        LoadKeywords = true,
                                        LoadNote = true
                                    }
                               });
                                documentoAmministrativoAggregate.RichiediRegistrazione(new DatiRichiestaRegistrazione()
                                {
                                    DatiRegistro = new DatiRegistro()
                                    {
                                        IdRegistro = !string.IsNullOrEmpty(schedaDocumento.id_rf_prot) ? schedaDocumento.id_rf_prot : schedaDocumento.registro.systemId
                                    }
                                });

                                await _documentoAmministrativoRepository.Update(documentoAmministrativoAggregate);

                                var tipologiaFlusso = schedaDocumento.tipoProto.AsTipologiaFlusso();
                                if (tipologiaFlusso == TipologiaFlussoEnum.U)
                                    await this._mediator.Send(new Requests.AddAllegatoSegnaturaXML(schedaDocumento.docNumber));

                                schedaDocumento.protocollo.segnatura = documentoAmministrativoAggregate.IdDoc.Segnatura;
                                schedaDocumento.protocollo.dataProtocollazione = (documentoAmministrativoAggregate.DatiRegistrazione as DatiRegistrazioneProtocollo).DataProtocollazione.AsDateTimeFormat();
                                schedaDocumento.protocollo.numero = (documentoAmministrativoAggregate.DatiRegistrazione as DatiRegistrazioneProtocollo).NumeroProtocollo.ToString();

                            }
                            catch (Exception ex)
                            {
                                problems.Add(Resources.ErrProto);
                                this._logger.LogError(message: ex.Message, exception: ex);
                            }
                        }

                        if (isImportAndAcquire)
                        {
                            fileAcquired = true;
                            try
                            {
                                if (isImportAndAcquire)
                                {
                                    await this.AcquireFileFromModel(rowData, userInfo, role, isSmistamentoEnabled, schedaDocumento, ftpAddress, ftpUsername, ftpPassword);
                                }
                                else
                                {
                                    await this.AcquireFile(rowData, userInfo, role, isSmistamentoEnabled, schedaDocumento, ftpAddress, ftpUsername, ftpPassword);
                                }
                            }
                            catch (Exception ex)
                            {
                                fileAcquired = false;
                                problems.Add(ex.Message);
                                this._logger.LogError(exception: ex, message: ex.Message);
                            }
                        }
                        break;
                }
            }
            else
            {
                if (!fileAcquired)
                {
                    problems.Add(Resources.FileNotAc3);
                }
            }

            // 7. Trasmissione documento
            problems.AddRange(await this.TransmitDocument(schedaDocumento, rowData, userInfo, role, serverPath));

            // 8. Salvataggio del documento nell'area di lavoro se richiesto
            if (rowData.InWorkingArea)
            {
                var tempProblems = await this.SaveDocumentInWorkingArea(schedaDocumento, userInfo, role);
                problems.AddRange(tempProblems);
            }

            bool grigio = isGray || isPredisposto;
            identificationData = this.GetIdentificationData(schedaDocumento, grigio);

            docNumber = schedaDocumento.docNumber;
            idProfile = schedaDocumento.systemId;

            return new()
            {
                Docnumber = docNumber,
                IdProfile = idProfile,
                Problems = problems,
                FileAcquired = fileAcquired,
                IdentificationData = identificationData
            };
        }

        private async Task AcquireFileFromModel(DocumentRowData rowData, InfoUtente userInfo, Ruolo role, bool isSmistamentoEnabled, DocsPaVO.documento.SchedaDocumento schedaDocumento, string ftpAddress, string ftpUsername, string ftpPassword)
        {
            FileRequest fileRequest;
            FileDocumento fileDocumento;
            string tipologia = rowData.DocumentTipology;
            byte[] content = await this.BuildDocumentFromTemplate(rowData, schedaDocumento, userInfo, tipologia, role, isSmistamentoEnabled);
            fileDocumento = new FileDocumento();
            fileDocumento.name = System.IO.Path.GetFileName(schedaDocumento.systemId + ".rtf");
            fileDocumento.fullName = schedaDocumento.systemId + ".rtf";
            fileDocumento.estensioneFile = "rtf";
            fileDocumento.length = content.Length;
            fileDocumento.content = content;
            fileRequest = (FileRequest)schedaDocumento.documenti[0];

            try
            {
                var t = (await this._mediator.Send(new Application.Requests.DocumentoPutFile(fileRequest, fileDocumento, userInfo)));
            }
            catch (Exception e)
            {
                this._logger.LogError(exception:e,message:e.Message);
                throw new Exception(Trasm.AcquErr);
            }
        }

        private async Task<byte[]> BuildDocumentFromTemplate(DocumentRowData rowData, DocsPaVO.documento.SchedaDocumento schedaDocumento, InfoUtente userInfo, string templateName, Ruolo role, bool isSmistamentoEnabled)
        {
            byte[] output = null;
            var template = await this.GetTemplate(userInfo, templateName);

            string path = template.PATH_MODELLO_STAMPA_UNIONE;

            if(string.IsNullOrEmpty(path))
                path = template.PATH_MODELLO_1;


            path = path.PathAsUnixPath();
            
            /*
            string path = template.PATH_MODELLO_STAMPA_UNIONE;
            if (string.IsNullOrEmpty(path)) path = template.PATH_MODELLO_1;
            */
            if (!File.Exists(path))
            {
                throw new Exception(Trasm.NoModelFound);
            }
            try
            {
                byte[] input = GetFileContent(path);
                RtfBuilder tempBuilder = new();

                await HandleCampiComuni(userInfo, tempBuilder, schedaDocumento);

                foreach (ProfilationFieldInformation temp in rowData.DocumentProfilationData)
                {
                    await GetPlaceHolderHandler(template, temp.Label).Invoke(schedaDocumento, tempBuilder, rowData, temp.Label, temp.Values, userInfo, role, isSmistamentoEnabled);
                }
                return tempBuilder.ReplacePlaceholders(input);
            }
            catch (Exception e)
            {
                this._logger.LogDebug(Trasm.CreationDocErr + e);
                throw new Exception(Trasm.CreationAssErr);
            }

            return output;

        }

        #region place holder
        private PlaceholderHandler GetPlaceHolderHandler(Templates temp, string oggettoCustomName)
        {
            OggettoCustom oggCustom = GetOggettoCustom(temp, oggettoCustomName);
            if (oggCustom == null) return HandleTextPlaceholder;
            if ("Corrispondente".Equals(oggCustom.TIPO.DESCRIZIONE_TIPO)) return HandleCorrispondentePlaceholder;
            if ("Contatore".Equals(oggCustom.TIPO.DESCRIZIONE_TIPO)) return HandleContatorePlaceholder;
            if ("Link".Equals(oggCustom.TIPO.DESCRIZIONE_TIPO))
            {
                if ("INTERNO".Equals(oggCustom.TIPO_LINK))
                {
                    if ("DOCUMENTO".Equals(oggCustom.TIPO_OBJ_LINK))
                    {
                        return HandleInternalLinkDocPlaceholder;
                    }
                    else
                    {
                        return HandleInternalLinkFascPlaceholder;
                    }
                }
                else
                {
                    return HandleExternalLinkPlaceholder;
                }
            }
            if (oggCustom != null && "OggettoEsterno".Equals(oggCustom.TIPO.DESCRIZIONE_TIPO))
            {
                return HandleOggettoEsterno;
            }
            return HandleTextPlaceholder;

        }

        private async Task HandleOggettoEsterno(DocsPaVO.documento.SchedaDocumento sd, RtfBuilder builder, DocumentRowData rowData,
            string label, string[] values, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, bool isEnabledSmistamento)
        {
            if (values.Length < 2) return;
            string value = values[0] + " - " + values[1];
            builder.AddTextPlaceholder(label, value);
        }

        private async Task HandleExternalLinkPlaceholder(DocsPaVO.documento.SchedaDocumento sd, RtfBuilder builder, DocumentRowData rowData,
            string label, string[] values, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, bool isEnabledSmistamento)
        {
            if (values.Length < 2) return;
            builder.AddLinkPlaceholder(label, values[0], values[1]);
        }

        private async Task HandleInternalLinkFascPlaceholder(DocsPaVO.documento.SchedaDocumento sd, RtfBuilder builder, DocumentRowData rowData,
            string label, string[] values, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, bool isEnabledSmistamento)
        {
            if (values.Length < 2) return;
            DocsPaVO.fascicolazione.Fascicolo fasc = await GetInfoFascicolo(values[1], infoUtente);
            string feLink = infoUtente.urlWA + "/";
            if (fasc != null)
            {
                builder.AddLinkPlaceholder(label, values[0], feLink + "visualizzaOggetto.aspx?idAmministrazione=" + infoUtente.idAmministrazione + "&tipoOggetto=F&idObj=" + fasc.codice);
            }
        }

        private async Task<DocsPaVO.fascicolazione.Fascicolo> GetInfoFascicolo(string idFascicolo, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.fascicolazione.Fascicolo fasc = null;
            try
            {
                var f = (await this._mediator.Send(new Application.Requests.FascicolazioneGetFascicoloById(idFascicolo, infoUtente))).output;
            }
            catch (Exception e) { }
            if (fasc == null) return null;
            int result = (await this._mediator.Send(new Application.Requests.VerificaACL("F", fasc.systemID, infoUtente))).output;
            if (result != 2) return null;
            return fasc;
        }

        private async Task HandleInternalLinkDocPlaceholder(DocsPaVO.documento.SchedaDocumento sd, RtfBuilder builder, DocumentRowData rowData,
            string label, string[] values, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, bool isEnabledSmistamento)
        {
            if (values.Length < 2) return;

            BaseInfoDoc infoDoc = await GetInfoDocumento(values[1], infoUtente);
            if (infoDoc == null) return;
            bool acquisito = infoDoc.HaveFile;
            string feLink = infoUtente.urlWA + "/";
            if (!acquisito)
            {
                builder.AddLinkPlaceholder(label, values[0], feLink + "visualizzaOggetto.aspx?idAmministrazione=" + infoUtente.idAmministrazione + "&tipoOggetto=D&idObj=" + values[1]);
            }
            else
            {
                builder.AddLinkPlaceholder(label, values[0], feLink + "visualizzaLink.aspx?docNumber=" + values[1]);
            }
        }


        private async Task<BaseInfoDoc> GetInfoDocumento(string idDoc, DocsPaVO.utente.InfoUtente infoUtente)
        {
            BaseInfoDoc infoDoc = null;
            try
            {
                List<BaseInfoDoc> infos = (await this._mediator.Send(new Application.Requests.GetBaseInfoForDocument(idDoc, null, null))).output;
                if (infos.Count == 0) return null;
                infoDoc = infos[0];
            }
            catch (Exception e) { }
            if (infoDoc == null) return null;
            int result = (await this._mediator.Send(new Application.Requests.VerificaACL("D", idDoc, infoUtente))).output;
            if (result != 2) return null;
            return infoDoc;
        }

        private async Task HandleContatorePlaceholder(DocsPaVO.documento.SchedaDocumento sd, RtfBuilder builder, DocumentRowData rowData,
            string label, string[] values, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, bool isEnabledSmistamento)
        {
            OggettoCustom oggCust = sd.template.getOggettoCustom(label);
            string value = "";
            if (!string.IsNullOrEmpty(oggCust.FORMATO_CONTATORE))
            {
                value = oggCust.FORMATO_CONTATORE;
                value = value.Replace("ANNO", oggCust.ANNO);
                value = value.Replace("CONTATORE", oggCust.VALORE_DATABASE);
                string codiceAmministrazione = (await this._mediator.Send(new Application.Requests.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione))).output.Codice;
                value = value.Replace("COD_AMM", codiceAmministrazione);
                value = value.Replace("COD_UO", oggCust.CODICE_DB);
                int fine = oggCust.DATA_INSERIMENTO.LastIndexOf(".");
                value = value.Replace("gg/mm/aaaa hh:mm", oggCust.DATA_INSERIMENTO.Substring(0, fine));
                value = value.Replace("gg/mm/aaaa", oggCust.DATA_INSERIMENTO.Substring(0, 10));
                if (!string.IsNullOrEmpty(oggCust.ID_AOO_RF) && oggCust.ID_AOO_RF != "0")
                {
                    Registro reg = (await this._mediator.Send(new Application.Requests.GetRegistroBySistemId(oggCust.ID_AOO_RF))).output;
                    if (reg != null)
                    {
                        value = value.Replace("RF", reg.codRegistro);
                        value = value.Replace("AOO", reg.codRegistro);
                    }
                }
            }
            else
            {
                value = oggCust.VALORE_DATABASE;
            }
            builder.AddTextPlaceholder(label, value);
        }

        private async Task HandleTextPlaceholder(DocsPaVO.documento.SchedaDocumento sd, RtfBuilder builder, DocumentRowData rowData,
            string label, string[] values, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, bool isEnabledSmistamento)
        {
            builder.AddTextPlaceholder(label, values, TextPosition.VERTICAL);
        }
        private async Task HandleCorrispondentePlaceholder(DocsPaVO.documento.SchedaDocumento sd, RtfBuilder builder, DocumentRowData rowData,
            string label, string[] values, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, bool isEnabledSmistamento)
        {
            try
            {
                OggettoCustom oggCust = sd.template.getOggettoCustom(label);
                string idAmm = await this.GetIdAmm(rowData.AdminCode);
                DocsPaVO.utente.Corrispondente corr = await this.FindCorrispondente(values[0], oggCust, infoUtente, ruolo, rowData.RFCode != null ? rowData.RFCode : string.Empty, rowData.RegCode != null ? rowData.RegCode : string.Empty, idAmm, isEnabledSmistamento);
                if (corr != null)
                {
                    DocsPaVO.utente.Corrispondente corrInd = await this.GetDettagliIndirizzoCorrispondente(corr.systemId);
                    CorrispondenteInfo corrInfo = new CorrispondenteInfo(corr, corrInd, builder);
                    builder.AddTextPlaceholder(label, corrInfo.Descrizione);
                    builder.AddTextPlaceholder(label + "$indirizzo", corrInfo.Indirizzo);
                    builder.AddTextPlaceholder(label + "$telefono", corrInfo.Telefono);
                    builder.AddTextPlaceholder(label + "$indirizzo$telefono", corrInfo.IndirizzoTelefono);
                }
            }
            catch (Exception e) { }
        }

        private async Task<string> GetIdAmm(string code)
        {
            var sysId = await (from a in this._dbContext.AmministraEntities.AsNoTracking()
                               where a.VAR_CODICE_AMM != null && a.VAR_CODICE_AMM.ToUpper().Contains(code.ToUpper())
                               select a.SYSTEM_ID
             ).FirstOrDefaultAsync();

            return sysId != null ? sysId.ToString() : null;
        }


        private enum TextPosition
        {
            VERTICAL, HORIZONTAL
        }

        private delegate Task PlaceholderHandler(DocsPaVO.documento.SchedaDocumento sd, RtfBuilder builder, DocumentRowData rowData,
            string label, string[] values, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo, bool isEnabledSmistamento);

        private OggettoCustom GetOggettoCustom(Templates t, string descr)
        {
            foreach (object temp in t.ELENCO_OGGETTI)
            {
                OggettoCustom oggCust = (OggettoCustom)temp;
                if (descr.ToUpper().Equals(oggCust.DESCRIZIONE.ToUpper())) return oggCust;
            }
            return null;
        }
        #endregion

        private async Task HandleCampiComuni(InfoUtente infoUtente, RtfBuilder builder, DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            try
            {
                DocsPaVO.utente.Corrispondente corr = new DocsPaVO.utente.Corrispondente();
                DocsPaVO.amministrazione.OrgDettagliGlobali dettCorr = new DocsPaVO.amministrazione.OrgDettagliGlobali();
                DocsPaVO.amministrazione.InfoAmministrazione infoAmm = (await this._mediator.Send(new Application.Requests.AmmGetInfoAmmCorrente(infoUtente.idAmministrazione))).output;
                if (infoAmm != null)
                    builder.AddTextPlaceholder(DocumentCommonFields.AMMINISTRAZIONE, infoAmm.Descrizione);
                // OGGETTO
                builder.AddTextPlaceholder(DocumentCommonFields.OGGETTO, schedaDocumento.oggetto.descrizione);
                // DATA CREAZIONE
                builder.AddTextPlaceholder(DocumentCommonFields.DATA_CREAZIONE, schedaDocumento.dataCreazione);

                // ID DOCUMENTO
                builder.AddTextPlaceholder(DocumentCommonFields.ID_DOCUMENTO, schedaDocumento.docNumber);

                // NOTE 
                // Reperimento dell'ultima nota visibile a tutti
                string testoNote = string.Empty;

                foreach (DocsPaVO.Note.InfoNota nota in await this.GetNote(infoUtente, new DocsPaVO.Note.AssociazioneNota(DocsPaVO.Note.AssociazioneNota.OggettiAssociazioniNotaEnum.Documento, schedaDocumento.systemId), new()))
                {
                    if (nota.TipoVisibilita == DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti)
                    {
                        testoNote = nota.Testo;
                        break;
                    }
                }

                builder.AddTextPlaceholder(DocumentCommonFields.NOTE, testoNote);
                // TIPOLOGIA
                if (schedaDocumento.tipologiaAtto != null)
                    builder.AddTextPlaceholder(DocumentCommonFields.TIPOLOGIA, schedaDocumento.tipologiaAtto.descrizione);

                if (schedaDocumento.creatoreDocumento != null)
                {
                    // CREATORE					
                    if (schedaDocumento.creatoreDocumento.idPeople != null && schedaDocumento.creatoreDocumento.idPeople != string.Empty)
                    {
                        corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteBySystemId(await this.GetIdUtCorr(schedaDocumento.creatoreDocumento.idPeople)))).output;
                        builder.AddTextPlaceholder(DocumentCommonFields.CREATORE, corr.descrizione);
                    }

                    // RUOLO CREATORE				
                    if (schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo != null && schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo != string.Empty)
                    {
                        corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteBySystemId(schedaDocumento.creatoreDocumento.idCorrGlob_Ruolo))).output;
                        builder.AddTextPlaceholder(DocumentCommonFields.RUOLO_CREATORE, corr.descrizione);
                    }
                    else
                        builder.AddTextPlaceholder(DocumentCommonFields.RUOLO_CREATORE, string.Empty);

                    // UO CREATORE				
                    if (schedaDocumento.creatoreDocumento.idCorrGlob_UO != null && schedaDocumento.creatoreDocumento.idCorrGlob_UO != string.Empty)
                    {
                        //descrizione
                        corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteBySystemId(schedaDocumento.creatoreDocumento.idCorrGlob_UO))).output;
                        builder.AddTextPlaceholder(DocumentCommonFields.UO_CREATORE, corr.descrizione);

                        // UO PADRE

                        string idCorrGlobali = string.Empty;
                        if (schedaDocumento.protocollatore != null)
                            idCorrGlobali = schedaDocumento.protocollatore.uo_idCorrGlobali;
                        else
                            idCorrGlobali = schedaDocumento.creatoreDocumento.idCorrGlob_UO;

                        long idParent = await this.AmmListaIdParentRicercaUO(Convert.ToInt32(idCorrGlobali));
                        if (idParent != 0)
                        {
                            corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteBySystemId(Convert.ToString(idParent)))).output;
                            builder.AddTextPlaceholder(DocumentCommonFields.UO_PADRE, corr.descrizione);
                            builder.AddTextPlaceholder(DocumentCommonFields.COD_UO_PADRE, corr.codiceRubrica);

                            //dettagli
                            dettCorr = await this.AmmGetDatiStamp(corr.systemId);
                            if (dettCorr != null)
                            {
                                builder.AddTextPlaceholder(DocumentCommonFields.INDIRIZZO_UO_PADRE, dettCorr.Indirizzo);
                                builder.AddTextPlaceholder(DocumentCommonFields.CITTA_UO_PADRE, dettCorr.Citta);
                                builder.AddTextPlaceholder(DocumentCommonFields.CAP_UO_PADRE, dettCorr.Cap);
                                builder.AddTextPlaceholder(DocumentCommonFields.PROVINCIA_UO_PADRE, dettCorr.Provincia);
                                builder.AddTextPlaceholder(DocumentCommonFields.TEL1_UO_PADRE, dettCorr.Telefono1);
                                builder.AddTextPlaceholder(DocumentCommonFields.TEL2_UO_PADRE, dettCorr.Telefono2);
                                builder.AddTextPlaceholder(DocumentCommonFields.FAX_UO_PADRE, dettCorr.Fax);
                            }
                        }
                        else
                        {
                            builder.AddTextPlaceholder(DocumentCommonFields.UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(DocumentCommonFields.COD_UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(DocumentCommonFields.INDIRIZZO_UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(DocumentCommonFields.CITTA_UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(DocumentCommonFields.CAP_UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(DocumentCommonFields.PROVINCIA_UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(DocumentCommonFields.TEL1_UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(DocumentCommonFields.TEL2_UO_PADRE, string.Empty);
                            builder.AddTextPlaceholder(DocumentCommonFields.FAX_UO_PADRE, string.Empty);
                        }

                        //dettagli
                        dettCorr = await this.AmmGetDatiStamp(schedaDocumento.creatoreDocumento.idCorrGlob_UO);
                        if (dettCorr != null)
                        {
                            builder.AddTextPlaceholder(DocumentCommonFields.INDIRIZZO_UO_CREATORE, dettCorr.Indirizzo);
                            builder.AddTextPlaceholder(DocumentCommonFields.CITTA_UO_CREATORE, dettCorr.Citta);
                            builder.AddTextPlaceholder(DocumentCommonFields.CAP_UO_CREATORE, dettCorr.Cap);
                            builder.AddTextPlaceholder(DocumentCommonFields.NAZIONE_UO_CREATORE, dettCorr.Nazione);
                            builder.AddTextPlaceholder(DocumentCommonFields.PROVINCIA_UO_CREATORE, dettCorr.Provincia);
                            builder.AddTextPlaceholder(DocumentCommonFields.TEL1_UO_CREATORE, dettCorr.Telefono1);
                            builder.AddTextPlaceholder(DocumentCommonFields.TEL2_UO_CREATORE, dettCorr.Telefono2);
                            builder.AddTextPlaceholder(DocumentCommonFields.FAX_UO_CREATORE, dettCorr.Fax);
                        }
                    }
                    else
                    {
                        builder.AddTextPlaceholder(DocumentCommonFields.UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(DocumentCommonFields.INDIRIZZO_UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(DocumentCommonFields.CITTA_UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(DocumentCommonFields.CAP_UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(DocumentCommonFields.NAZIONE_UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(DocumentCommonFields.PROVINCIA_UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(DocumentCommonFields.TEL1_UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(DocumentCommonFields.TEL2_UO_CREATORE, string.Empty);
                        builder.AddTextPlaceholder(DocumentCommonFields.FAX_UO_CREATORE, string.Empty);
                    }
                }
                //Ruolo responsabile della UO
                if (schedaDocumento.creatoreDocumento != null && schedaDocumento.creatoreDocumento.idCorrGlob_UO != null && schedaDocumento.creatoreDocumento.idCorrGlob_UO != string.Empty)
                {
                    // ruolo responsabile UO
                    OrgRuolo ruoloResp = await this.AmmGetRuoloResponsabileUO(schedaDocumento.creatoreDocumento.idCorrGlob_UO);
                    if (ruoloResp != null)
                    {
                        builder.AddTextPlaceholder(DocumentCommonFields.COD_RESP_UO, ruoloResp.CodiceRubrica);
                        builder.AddTextPlaceholder(DocumentCommonFields.DESC_RESP_UO, ruoloResp.Descrizione);
                        if (ruoloResp.Utenti != null && ruoloResp.Utenti.Count() > 0)
                        {
                            string listaUtenti = string.Empty;

                            for (int i = 0; i < ruoloResp.Utenti.Count(); i++)
                            {
                                DocsPaVO.amministrazione.OrgUtente ut = ((DocsPaVO.amministrazione.OrgUtente)ruoloResp.Utenti[i]);
                                listaUtenti += ut.Nome + " " + ut.Cognome + ",";
                            }
                            if (listaUtenti != "")
                                listaUtenti = listaUtenti.Substring(0, listaUtenti.Length - 1);

                            builder.AddTextPlaceholder(DocumentCommonFields.LISTA_UTENTE_RESP_UO, listaUtenti);
                        }
                        else
                        {
                            builder.AddTextPlaceholder(DocumentCommonFields.LISTA_UTENTE_RESP_UO, string.Empty);
                        }
                    }
                    else
                    {
                        builder.AddTextPlaceholder(DocumentCommonFields.COD_RESP_UO, string.Empty);
                        builder.AddTextPlaceholder(DocumentCommonFields.DESC_RESP_UO, string.Empty);
                        builder.AddTextPlaceholder(DocumentCommonFields.LISTA_UTENTE_RESP_UO, string.Empty);
                    }

                }
                // Classificazioni del documento

                string classifiche = string.Empty;
                var fascicoliD = (await this._mediator.Send(new Application.Requests.FascicolazioneGetFascicoliDaDoc(infoUtente, schedaDocumento.systemId))).output;
                foreach (DocsPaVO.fascicolazione.Fascicolo item in fascicoliD)
                {
                    if (classifiche != string.Empty)
                        classifiche += builder.NewLine();
                    classifiche += item.codice;
                }

                builder.AddTextPlaceholder(DocumentCommonFields.CLASSIFICHE, classifiche);

                // campi del documento protocollato
                if (schedaDocumento.protocollo != null)
                {
                    // NUMERO PROTOCOLLO
                    builder.AddTextPlaceholder(DocumentCommonFields.NUM_PROTOCOLLO, this._handleNullString(schedaDocumento.protocollo.numero));

                    // SEGNATURA
                    builder.AddTextPlaceholder(DocumentCommonFields.SEGNATURA, this._handleNullString(schedaDocumento.protocollo.segnatura));

                    // DATA PROTOCOLLO
                    builder.AddTextPlaceholder(DocumentCommonFields.DATA_PROTOCOLLO, this._handleNullString(schedaDocumento.protocollo.dataProtocollazione));

                    // DATA ORA PROTOCOLLO
                    builder.AddTextPlaceholder(DocumentCommonFields.DATA_ORA_PROTOCOLLO, this._handleNullString(await this.GetDataOraProtocollo(schedaDocumento.docNumber)));

                    // REGISTRO
                    builder.AddTextPlaceholder(DocumentCommonFields.REGISTRO, schedaDocumento.registro.descrizione);

                    // CODICE REGISTRO
                    builder.AddTextPlaceholder(DocumentCommonFields.CODICE_REGISTRO, schedaDocumento.registro.codRegistro);

                    // NUMERO ALLEGATI
                    var listaAllegati = (await this._mediator.Send(new Application.Requests.DocumentoGetAllegati(schedaDocumento.docNumber, string.Empty, string.Empty))).output;
                    if (listaAllegati != null)
                        builder.AddTextPlaceholder(DocumentCommonFields.NUM_ALLEGATI, Convert.ToString(listaAllegati.Count()));

                    if (schedaDocumento.protocollatore != null)
                    {
                        // PROTOCOLLATORE						
                        if (schedaDocumento.protocollatore.utente_idPeople != null && schedaDocumento.protocollatore.utente_idPeople != string.Empty)
                        {
                            corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteBySystemId(await this.GetIdUtCorr(schedaDocumento.protocollatore.utente_idPeople)))).output;
                            builder.AddTextPlaceholder(DocumentCommonFields.PROTOCOLLATORE, corr.descrizione);
                        }

                        // RUOLO PROTOCOLLATORE					
                        if (schedaDocumento.protocollatore.ruolo_idCorrGlobali != null && schedaDocumento.protocollatore.ruolo_idCorrGlobali != string.Empty)
                        {
                            corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteBySystemId((schedaDocumento.protocollatore.ruolo_idCorrGlobali)))).output;
                            builder.AddTextPlaceholder(DocumentCommonFields.RUOLO_PROTOCOLLATORE, corr.descrizione);
                        }

                        // UO PROTOCOLLATORE					
                        if (schedaDocumento.protocollatore.uo_idCorrGlobali != null && schedaDocumento.protocollatore.uo_idCorrGlobali != string.Empty)
                        {
                            //descrizione
                            corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteBySystemId((schedaDocumento.protocollatore.uo_idCorrGlobali)))).output;
                            builder.AddTextPlaceholder(DocumentCommonFields.UO_PROTOCOLLATORE, corr.descrizione);

                            //dettagli
                            dettCorr = await this.AmmGetDatiStamp(schedaDocumento.protocollatore.uo_idCorrGlobali);
                            if (dettCorr != null)
                            {
                                builder.AddTextPlaceholder(DocumentCommonFields.INDIRIZZO_UO_PROT, dettCorr.Indirizzo);
                                builder.AddTextPlaceholder(DocumentCommonFields.CITTA_UO_PROT, dettCorr.Citta);
                                builder.AddTextPlaceholder(DocumentCommonFields.CAP_UO_PROT, dettCorr.Cap);
                                builder.AddTextPlaceholder(DocumentCommonFields.NAZIONE_UO_PROT, dettCorr.Nazione);
                                builder.AddTextPlaceholder(DocumentCommonFields.PROVINCIA_UO_PROT, dettCorr.Provincia);
                                builder.AddTextPlaceholder(DocumentCommonFields.TEL1_UO_PROT, dettCorr.Telefono1);
                                builder.AddTextPlaceholder(DocumentCommonFields.TEL2_UO_PROT, dettCorr.Telefono2);
                                builder.AddTextPlaceholder(DocumentCommonFields.FAX_UO_PROT, dettCorr.Fax);
                            }

                            // UO PADRE
                            long idParent = await this.AmmListaIdParentRicercaUO(Convert.ToInt32(schedaDocumento.protocollatore.uo_idCorrGlobali));
                            if (idParent != 0)
                            {
                                corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteBySystemId((Convert.ToString(idParent))))).output;
                                builder.AddTextPlaceholder(DocumentCommonFields.UO_PADRE, corr.descrizione);
                                builder.AddTextPlaceholder(DocumentCommonFields.COD_UO_PADRE, corr.codiceRubrica);

                                //dettagli
                                dettCorr = await this.AmmGetDatiStamp(corr.systemId);
                                if (dettCorr != null)
                                {
                                    builder.AddTextPlaceholder(DocumentCommonFields.INDIRIZZO_UO_PADRE, dettCorr.Indirizzo);
                                    builder.AddTextPlaceholder(DocumentCommonFields.CITTA_UO_PADRE, dettCorr.Citta);
                                    builder.AddTextPlaceholder(DocumentCommonFields.CAP_UO_PADRE, dettCorr.Cap);
                                    builder.AddTextPlaceholder(DocumentCommonFields.PROVINCIA_UO_PADRE, dettCorr.Provincia);
                                    builder.AddTextPlaceholder(DocumentCommonFields.TEL1_UO_PADRE, dettCorr.Telefono1);
                                    builder.AddTextPlaceholder(DocumentCommonFields.TEL2_UO_PADRE, dettCorr.Telefono2);
                                    builder.AddTextPlaceholder(DocumentCommonFields.FAX_UO_PADRE, dettCorr.Fax);
                                }
                            }
                            else
                            {
                                builder.AddTextPlaceholder(DocumentCommonFields.UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.COD_UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.INDIRIZZO_UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.CITTA_UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.CAP_UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.PROVINCIA_UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.TEL1_UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.TEL2_UO_PADRE, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.FAX_UO_PADRE, string.Empty);
                            }
                        }
                    }

                    if (schedaDocumento.tipoProto != null)
                    {
                        string listaDestinatari = string.Empty;
                        string listaDestinatariIndirizzi = string.Empty;
                        string listaDestinatariTelefono = string.Empty;
                        string listaDestinatariIndirizzoTelefono = string.Empty;
                        string listaMittentiMultipli = string.Empty;
                        string listaMittentiMultipliIndirizzo = string.Empty;
                        string listaMittentiMultipliTelefono = string.Empty;
                        string listaMittentiMultipliIndirizzoTelefono = string.Empty;
                        string mittenteIndirizzo = string.Empty;
                        string mittenteTelefono = string.Empty;
                        string mittenteIndirizzoTelefono = string.Empty;
                        // Protocollo in INGRESSO (Arrivo)
                        if (schedaDocumento.tipoProto.Equals("A"))
                        {
                            DocsPaVO.documento.ProtocolloEntrata prot = new DocsPaVO.documento.ProtocolloEntrata();
                            prot = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;
                            // MITTENTE
                            if (prot.mittente != null)
                            {
                                DocsPaVO.utente.Corrispondente mittIndirizzo = await this.GetDettagliIndirizzoCorrispondente(prot.mittente.systemId);
                                CorrispondenteInfo mittInfo = new CorrispondenteInfo(prot.mittente, mittIndirizzo, builder);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE, mittInfo.Descrizione);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_INDIRIZZO, mittInfo.Indirizzo);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_TELEFONO, mittInfo.Telefono);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, mittInfo.IndirizzoTelefono);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty);


                            }
                            if (prot.mittenti != null && prot.mittenti.Count() > 0)
                            {
                                foreach (DocsPaVO.utente.Corrispondente mittMult in prot.mittenti)
                                {
                                    if (listaMittentiMultipli != string.Empty)
                                        listaMittentiMultipli += builder.NewLine();
                                    DocsPaVO.utente.Corrispondente corrIndirizzo = await this.GetDettagliIndirizzoCorrispondente(mittMult.systemId);
                                    CorrispondenteInfo mittMultInfo = new CorrispondenteInfo(mittMult, corrIndirizzo, builder);
                                    listaMittentiMultipli += mittMultInfo.Descrizione;

                                    if (listaMittentiMultipliIndirizzo != string.Empty)
                                        listaMittentiMultipliIndirizzo += builder.NewLine();
                                    listaMittentiMultipliIndirizzo += mittMultInfo.Indirizzo;

                                    if (listaMittentiMultipliTelefono != string.Empty)
                                        listaMittentiMultipliTelefono += builder.NewLine();
                                    listaMittentiMultipliTelefono += mittMultInfo.Telefono;

                                    if (listaMittentiMultipliIndirizzoTelefono != string.Empty)
                                        listaMittentiMultipliIndirizzoTelefono += builder.NewLine();
                                    listaMittentiMultipliIndirizzoTelefono += mittMultInfo.IndirizzoTelefono;
                                }
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTI_MULTIPLI, listaMittentiMultipli);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTI_MULTIPLI_INDIRIZZO, listaMittentiMultipliIndirizzo);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTI_MULTIPLI_TELEFONO, listaMittentiMultipliTelefono);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTI_MULTIPLI_INIDIRIZZO_TELEFONO, listaMittentiMultipliIndirizzoTelefono);


                            }
                            else
                            {
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTI_MULTIPLI, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTI_MULTIPLI_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTI_MULTIPLI_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTI_MULTIPLI_INIDIRIZZO_TELEFONO, string.Empty);
                            }

                            if (prot.ufficioReferente != null)
                            {
                                // CODICE UFFICIO REFERENTE
                                builder.AddTextPlaceholder(DocumentCommonFields.UFF_REF_COD, prot.ufficioReferente.codiceRubrica);

                                // DESCRIZIONE UFFICIO REFERENTE
                                builder.AddTextPlaceholder(DocumentCommonFields.UFF_REF_DESC, prot.ufficioReferente.descrizione);
                            }
                        }

                        // Protocollo in USCITA (Partenza)
                        if (schedaDocumento.tipoProto.Equals("P"))
                        {
                            DocsPaVO.documento.ProtocolloUscita prot = new DocsPaVO.documento.ProtocolloUscita();
                            prot = (DocsPaVO.documento.ProtocolloUscita)schedaDocumento.protocollo;
                            // MITTENTE
                            if (prot.mittente != null)
                            {

                                DocsPaVO.utente.Corrispondente mittIndirizzo = await this.GetDettagliIndirizzoCorrispondente(prot.mittente.systemId);
                                CorrispondenteInfo mittInfo = new CorrispondenteInfo(prot.mittente, mittIndirizzo, builder);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE, mittInfo.Descrizione);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_INDIRIZZO, mittInfo.Indirizzo);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_TELEFONO, mittInfo.Telefono);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, mittInfo.IndirizzoTelefono);
                            }
                            else
                            {
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, string.Empty);
                            }
                            // DESTINATARI								
                            if (prot.destinatari != null && prot.destinatari.Count() > 0)
                            {
                                foreach (DocsPaVO.utente.Corrispondente utCorr in prot.destinatari)
                                {
                                    DocsPaVO.utente.Corrispondente corrIndirizzo = await this.GetDettagliIndirizzoCorrispondente(utCorr.systemId);
                                    CorrispondenteInfo destInfo = new CorrispondenteInfo(utCorr, corrIndirizzo, builder);
                                    if (listaDestinatari != string.Empty)
                                        listaDestinatari += builder.NewLine();
                                    listaDestinatari += destInfo.Descrizione;

                                    if (listaDestinatariIndirizzi != string.Empty)
                                        listaDestinatariIndirizzi += builder.NewLine();
                                    listaDestinatariIndirizzi += destInfo.Indirizzo;

                                    if (listaDestinatariTelefono != string.Empty)
                                        listaDestinatariTelefono += builder.NewLine();
                                    listaDestinatariTelefono += destInfo.Telefono;

                                    if (listaDestinatariIndirizzoTelefono != string.Empty)
                                        listaDestinatariIndirizzoTelefono += builder.NewLine();
                                    listaDestinatariIndirizzoTelefono += destInfo.IndirizzoTelefono;
                                }
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI, listaDestinatari);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_INDIRIZZO, listaDestinatariIndirizzi);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_TELEFONO, listaDestinatariTelefono);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, listaDestinatariIndirizzoTelefono);

                            }
                            else
                            {
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty);
                            }
                            // DESTINATARI CC
                            listaDestinatari = string.Empty;
                            listaDestinatariIndirizzi = string.Empty;
                            listaDestinatariTelefono = string.Empty;
                            listaDestinatariIndirizzoTelefono = string.Empty;
                            if (prot.destinatariConoscenza != null && prot.destinatariConoscenza.Count() > 0)
                            {
                                foreach (DocsPaVO.utente.Corrispondente utCorr in prot.destinatariConoscenza)
                                {
                                    DocsPaVO.utente.Corrispondente corrIndirizzo = await this.GetDettagliIndirizzoCorrispondente(utCorr.systemId);
                                    CorrispondenteInfo corrInfo = new CorrispondenteInfo(utCorr, corrIndirizzo, builder);
                                    if (listaDestinatari != string.Empty)
                                        listaDestinatari += builder.NewLine();
                                    listaDestinatari += corrInfo.Descrizione;
                                    if (listaDestinatariIndirizzi != string.Empty)
                                        listaDestinatariIndirizzi += builder.NewLine();
                                    listaDestinatariIndirizzi += corrInfo.Indirizzo;

                                    if (listaDestinatariTelefono != string.Empty)
                                        listaDestinatariTelefono += builder.NewLine();
                                    listaDestinatariTelefono += corrInfo.Telefono;

                                    if (listaDestinatariIndirizzoTelefono != string.Empty)
                                        listaDestinatariIndirizzoTelefono += builder.NewLine();
                                    listaDestinatariIndirizzoTelefono += corrInfo.IndirizzoTelefono;
                                }

                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC, listaDestinatari);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, listaDestinatariIndirizzi);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_TELEFONO, listaDestinatariTelefono);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, listaDestinatariIndirizzoTelefono);
                            }
                            else
                            {
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty);
                            }
                            if (prot.ufficioReferente != null)
                            {
                                // CODICE UFFICIO REFERENTE
                                builder.AddTextPlaceholder(DocumentCommonFields.UFF_REF_COD, prot.ufficioReferente.codiceRubrica);

                                // DESCRIZIONE UFFICIO REFERENTE
                                builder.AddTextPlaceholder(DocumentCommonFields.UFF_REF_DESC, prot.ufficioReferente.descrizione);
                            }
                        }
                        // Protocollo INTERNO
                        if (schedaDocumento.tipoProto.Equals("I"))
                        {
                            DocsPaVO.documento.ProtocolloInterno prot = new DocsPaVO.documento.ProtocolloInterno();
                            prot = (DocsPaVO.documento.ProtocolloInterno)schedaDocumento.protocollo;

                            // MITTENTE					
                            if (prot.mittente != null)
                            {

                                DocsPaVO.utente.Corrispondente mittIndirizzo = await this.GetDettagliIndirizzoCorrispondente(prot.mittente.systemId);
                                CorrispondenteInfo mittInfo = new CorrispondenteInfo(prot.mittente, mittIndirizzo, builder);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE, mittInfo.Descrizione);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_INDIRIZZO, mittInfo.Indirizzo);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_TELEFONO, mittInfo.Telefono);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, mittInfo.IndirizzoTelefono);
                            }
                            else
                            {
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, string.Empty);
                            }
                            // DESTINATARI								
                            if (prot.destinatari != null && prot.destinatari.Count() > 0)
                            {
                                foreach (DocsPaVO.utente.Corrispondente utCorr in prot.destinatari)
                                {
                                    DocsPaVO.utente.Corrispondente corrIndirizzo = await this.GetDettagliIndirizzoCorrispondente(utCorr.systemId);
                                    CorrispondenteInfo corrInfo = new CorrispondenteInfo(utCorr, corrIndirizzo, builder);
                                    if (listaDestinatari != string.Empty)
                                        listaDestinatari += builder.NewLine();
                                    listaDestinatari += corrInfo.Descrizione;
                                    if (listaDestinatariIndirizzi != string.Empty)
                                        listaDestinatariIndirizzi += builder.NewLine();
                                    listaDestinatariIndirizzi += corrInfo.Indirizzo;

                                    if (listaDestinatariTelefono != string.Empty)
                                        listaDestinatariTelefono += builder.NewLine();
                                    listaDestinatariTelefono += corrInfo.Telefono;

                                    if (listaDestinatariIndirizzoTelefono != string.Empty)
                                        listaDestinatariIndirizzoTelefono += builder.NewLine();
                                    listaDestinatariIndirizzoTelefono += corrInfo.IndirizzoTelefono;
                                }
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI, listaDestinatari);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_INDIRIZZO, listaDestinatariIndirizzi);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_TELEFONO, listaDestinatariTelefono);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, listaDestinatariIndirizzoTelefono);
                            }
                            else
                            {
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty);
                            }
                            // DESTINATARI CC
                            listaDestinatari = string.Empty;
                            listaDestinatariIndirizzi = string.Empty;
                            listaDestinatariIndirizzoTelefono = string.Empty;
                            listaDestinatariTelefono = string.Empty;
                            if (prot.destinatariConoscenza != null && prot.destinatariConoscenza.Count() > 0)
                            {
                                foreach (DocsPaVO.utente.Corrispondente utCorr in prot.destinatariConoscenza)
                                {
                                    DocsPaVO.utente.Corrispondente corrIndirizzo = await this.GetDettagliIndirizzoCorrispondente(utCorr.systemId);
                                    CorrispondenteInfo corrInfo = new CorrispondenteInfo(utCorr, corrIndirizzo, builder);
                                    if (listaDestinatari != string.Empty)
                                        listaDestinatari += builder.NewLine();
                                    listaDestinatari += corrInfo.Descrizione;
                                    if (listaDestinatariIndirizzi != string.Empty)
                                        listaDestinatariIndirizzi += builder.NewLine();
                                    listaDestinatariIndirizzi += corrInfo.Indirizzo;

                                    if (listaDestinatariTelefono != string.Empty)
                                        listaDestinatariTelefono += builder.NewLine();
                                    listaDestinatariTelefono += corrInfo.Telefono;

                                    if (listaDestinatariIndirizzoTelefono != string.Empty)
                                        listaDestinatariIndirizzoTelefono += builder.NewLine();
                                    listaDestinatariIndirizzoTelefono += corrInfo.IndirizzoTelefono;
                                }
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC, listaDestinatari);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, listaDestinatariIndirizzi);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_TELEFONO, listaDestinatariTelefono);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, listaDestinatariIndirizzoTelefono);
                            }
                            else
                            {
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty);
                                builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty);
                            }
                            if (prot.ufficioReferente != null)
                            {
                                // CODICE UFFICIO REFERENTE
                                builder.AddTextPlaceholder(DocumentCommonFields.UFF_REF_COD, prot.ufficioReferente.codiceRubrica);

                                // DESCRIZIONE UFFICIO REFERENTE
                                builder.AddTextPlaceholder(DocumentCommonFields.UFF_REF_DESC, prot.ufficioReferente.descrizione);
                            }
                        }

                    }
                }
                else
                {
                    builder.AddTextPlaceholder(DocumentCommonFields.NUM_PROTOCOLLO, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.SEGNATURA, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.DATA_PROTOCOLLO, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.REGISTRO, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.CODICE_REGISTRO, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.PROTOCOLLATORE, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.RUOLO_PROTOCOLLATORE, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.UO_PROTOCOLLATORE, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.INDIRIZZO_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.CITTA_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.CAP_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.NAZIONE_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.PROVINCIA_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.TEL1_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.TEL2_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.FAX_UO_PROT, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_INDIRIZZO, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_TELEFONO, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.MITTENTE_INDIRIZZO_TELEFONO, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_INDIRIZZO, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_TELEFONO, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_INDIRIZZO_TELEFONO, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_TELEFONO, string.Empty);
                    builder.AddTextPlaceholder(DocumentCommonFields.DESTINATARI_CC_INDIRIZZO_TELEFONO, string.Empty);
                }
                this._logger.LogDebug(Rtf.EndOfHandleCampiCom);
            }
            catch (Exception e)
            {
                this._logger.LogDebug(Rtf.excpHandleCampiCom + e);
            }
        }


        private async Task<DocsPaVO.amministrazione.OrgRuolo> AmmGetRuoloResponsabileUO(string idUo)
        {

            var ruoloResp = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                                   where a.ID_UO == idUo.AsLong() && a.CHA_RESPONSABILE != null && a.CHA_RESPONSABILE.Equals("1") && !a.DTA_FINE.HasValue
                                   select new DocsPaVO.amministrazione.OrgRuolo()
                                   {
                                       IDCorrGlobale = a.SYSTEM_ID.ToString(),
                                       IDGruppo = a.ID_GRUPPO != null ? a.ID_GRUPPO.ToString() : null,
                                       IDTipoRuolo = a.ID_TIPO_RUOLO != null ? a.ID_TIPO_RUOLO.ToString() : null,
                                       Codice = a.VAR_CODICE,
                                       CodiceRubrica = a.VAR_COD_RUBRICA,
                                       Descrizione = a.VAR_DESC_CORR,
                                       DiRiferimento = a.CHA_RIFERIMENTO,
                                       IDAmministrazione = a.ID_AMM != null ? a.ID_AMM.ToString() : null,
                                       Responsabile = a.CHA_RESPONSABILE
                                   }).FirstOrDefaultAsync();

            if (ruoloResp != null)
            {
                ruoloResp.Utenti = (await this._mediator.Send(new Application.Requests.AmmGetListUtentiRuolo(ruoloResp.IDGruppo))).output;
            }

            return ruoloResp;
        }

        private async Task<DocsPaVO.amministrazione.OrgDettagliGlobali> AmmGetDatiStamp(string idCorrGlob)
        {
            var dett = await this._dbContext.DettGlobaliEntities.AsNoTracking().Where(c => c.ID_CORR_GLOBALI == idCorrGlob.AsLong()).FirstOrDefaultAsync();
            DocsPaVO.amministrazione.OrgDettagliGlobali output = null;
            if (dett != null)
            {
                output = new()
                {
                    Indirizzo = dett.VAR_INDIRIZZO,
                    Citta = dett.VAR_CITTA,
                    Cap = dett.VAR_CAP,
                    Nazione = dett.VAR_NAZIONE,
                    Provincia = dett.VAR_PROVINCIA,
                    Telefono1 = dett.VAR_TELEFONO,
                    Telefono2 = dett.VAR_TELEFONO2,
                    Fax = dett.VAR_FAX,
                    Note = dett.VAR_NOTE,
                    CodiceFiscale = dett.VAR_COD_FISC,
                    PartitaIva = dett.VAR_COD_PI

                };
            }
            return output;

        }

        private async Task<string> GetDataOraProtocollo(string idProto)
        {
            var data = await (from p in this._dbContext.ProfileEntities.AsNoTracking()
                              where p.SYSTEM_ID == idProto.AsLong()
                              select p.DTA_PROTO).FirstOrDefaultAsync();
            return data != null ? data.AsDateTimeFormat() : null;
        }

        private async Task<string> GetIdUtCorr(string idPeople)
        {
            var sysId = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_PEOPLE == idPeople.AsLong()).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();

            return sysId != null ? sysId.ToString() : string.Empty;
        }

        private async Task<long> AmmListaIdParentRicercaUO(int idUoParent)
        {
            long output = 0;

            var parent = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.SYSTEM_ID == idUoParent).FirstOrDefaultAsync();
            if (parent != null)
            {
                output = parent.ID_PARENT != null ? (long)parent.ID_PARENT : 0;
            }

            return output;
        }
        private string _handleNullString(string value)
        {
            return value == null ? string.Empty : value;
        }
        private async Task<List<InfoNota>> GetNote(InfoUtente infoUtente, AssociazioneNota oggettoAssociato, FiltroRicercaNote filtroRicerca)
        {
            List<InfoNota> output = new();

            string idRuoloInUo = string.IsNullOrEmpty(infoUtente.idCorrGlobali) ? "0" : infoUtente.idCorrGlobali;
            var reg = await this._dbContext.RuoloRegistroEntities.AsNoTracking().Where(r => r.ID_RUOLO_IN_UO != null).Select(r => r.ID_REGISTRO).ToListAsync();

            var idRuoloCreatore = string.IsNullOrEmpty(infoUtente.idGruppo) ? "0" : infoUtente.idGruppo;

            var noteQuery = (from n in this._dbContext.NoteEntities.AsNoTracking()
                             join p in this._dbContext.PeopleEntities.AsNoTracking() on n.IDUTENTECREATORE equals p.SYSTEM_ID into pj
                             from pi in pj.DefaultIfEmpty()
                             join g in this._dbContext.GroupEntities.AsNoTracking() on n.IDRUOLOCREATORE equals g.SYSTEM_ID into gj
                             from gi in gj.DefaultIfEmpty()
                             where (n.IDOGGETTOASSOCIATO == oggettoAssociato.Id.AsLong()) && (n.TIPOVISIBILITA != null && n.TIPOVISIBILITA.Equals("T") ||
                             (n.TIPOVISIBILITA != null && n.TIPOVISIBILITA.Equals("F") && reg.Contains(n.IDRFASSOCIATO) ||
                             (n.TIPOVISIBILITA != null && n.TIPOVISIBILITA.Equals("P") && n.IDUTENTECREATORE == infoUtente.idPeople.AsLong()) ||
                             (n.TIPOVISIBILITA != null && n.TIPOVISIBILITA.Equals("R") && n.IDRUOLOCREATORE == idRuoloCreatore.AsLong())
                             ))
                             select new
                             {
                                 n.SYSTEM_ID,
                                 n.TESTO,
                                 n.DATACREAZIONE,
                                 n.IDUTENTECREATORE,
                                 n.IDRUOLOCREATORE,
                                 n.TIPOVISIBILITA,
                                 n.TIPOOGGETTOASSOCIATO,
                                 n.IDOGGETTOASSOCIATO,
                                 n.IDRFASSOCIATO,
                                 pi.USER_ID,
                                 pi.FULL_NAME,
                                 gi.GROUP_ID,
                                 gi.GROUP_NAME,
                                 n.IDPEOPLEDELEGATO,
                             });
            if (!string.IsNullOrEmpty(filtroRicerca.Testo))
            {
                noteQuery = noteQuery.Where(n => n.TESTO != null && n.TESTO.Contains(filtroRicerca.Testo.Replace("'", "''")));
            }

            if (oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Documento)
            {
                noteQuery = noteQuery.Where(n => n.TIPOOGGETTOASSOCIATO != null && n.TIPOOGGETTOASSOCIATO.Equals("D"));
            }
            else if (oggettoAssociato.TipoOggetto == AssociazioneNota.OggettiAssociazioniNotaEnum.Fascicolo)
            {
                noteQuery = noteQuery.Where(n => n.TIPOOGGETTOASSOCIATO != null && n.TIPOOGGETTOASSOCIATO.Equals("F"));

            }
            var maxNumChar = this.GetNumeroMassimoCaratteri(filtroRicerca);
            var note = await noteQuery.ToListAsync();
            foreach (var n in note)
            {
                InfoNota nota = new();
                nota.Id = n.SYSTEM_ID.ToString();

                if (maxNumChar > 0)
                {
                    if (maxNumChar > n.TESTO.Length)
                        nota.Testo = n.TESTO;
                    else
                    {
                        nota.Testo = n.TESTO.Substring(0, maxNumChar);
                    }
                }
                else
                {
                    nota.Testo = n.TESTO;
                }

                nota.DataCreazione = n.DATACREAZIONE;
                nota.TipoVisibilita = this.GetTipoVisibilita(n.TIPOVISIBILITA);

                InfoUtenteCreatoreNota creatore = new InfoUtenteCreatoreNota()
                {
                    IdUtente = n.IDUTENTECREATORE.ToString(),
                    IdRuolo = n.IDRUOLOCREATORE.ToString(),
                    DescrizioneUtente = n.USER_ID,
                    DescrizioneRuolo = n.GROUP_NAME
                };

                nota.UtenteCreatore = creatore;

                nota.SolaLettura = !n.IDUTENTECREATORE.Equals(infoUtente.idPeople);

                if (n.IDRFASSOCIATO != null)
                {
                    nota.IdRfAssociato = n.IDRFASSOCIATO.ToString();
                }
                string idPeopleDelegato = n.IDPEOPLEDELEGATO != null ? n.IDPEOPLEDELEGATO.ToString() : null;

                if (!string.IsNullOrEmpty(idPeopleDelegato) && !idPeopleDelegato.Equals("0"))
                {
                    nota.IdPeopleDelegato = idPeopleDelegato;
                    nota.DescrPeopleDelegato = await this.GetDescUtenteNoFiltroDisabled(idPeopleDelegato);
                }
                else
                {
                    nota.IdPeopleDelegato = string.Empty;
                    nota.DescrPeopleDelegato = string.Empty;
                }

                output.Add(nota);
            }



            return output;
        }


        private async Task<string> GetDescUtenteNoFiltroDisabled(string idPeopleDelegato)
        {
            var ut = await this._dbContext.PeopleEntities.FirstOrDefaultAsync(c => c.SYSTEM_ID == idPeopleDelegato.AsLong());
            string desc = string.Empty;
            if (ut != null)
            {
                desc = ut.VAR_COGNOME + " " + ut.VAR_NOME;
            }
            return desc;

        }



        private TipiVisibilitaNotaEnum GetTipoVisibilita(string visibilita)
        {
            if (visibilita.Equals("T"))
                return TipiVisibilitaNotaEnum.Tutti;
            else if (visibilita.Equals("F"))
                return TipiVisibilitaNotaEnum.RF;
            else if (visibilita.Equals("R"))
                return TipiVisibilitaNotaEnum.Ruolo;
            else if (visibilita.Equals("P"))
                return TipiVisibilitaNotaEnum.Personale;
            else
                return TipiVisibilitaNotaEnum.Tutti;
        }


        private int GetNumeroMassimoCaratteri(FiltroRicercaNote filtroRicerca)
        {
            if (filtroRicerca != null)
                return filtroRicerca.NumeroMassimoCaratteriTesto;
            else
                return 0;
        }



        #region corrispondente info
        private class CorrispondenteInfo
        {

            private DocsPaVO.utente.Corrispondente _corrispondente;
            private DocsPaVO.utente.Corrispondente _indirizzo;
            private RtfBuilder _builder;

            public CorrispondenteInfo(DocsPaVO.utente.Corrispondente corrispondente, DocsPaVO.utente.Corrispondente indirizzo, RtfBuilder builder)
            {
                _corrispondente = corrispondente;
                _indirizzo = indirizzo;
                _builder = builder;
            }

            public string Descrizione
            {
                get
                {
                    return _corrispondente.descrizione;
                }
            }

            public string Indirizzo
            {
                get
                {
                    string res = Descrizione;
                    if (_indirizzo == null) return res;
                    if (!string.IsNullOrEmpty(_indirizzo.indirizzo))
                    {
                        res += _builder.NewLine() + _indirizzo.indirizzo;
                    }
                    if (!string.IsNullOrEmpty(_indirizzo.cap) || !string.IsNullOrEmpty(_indirizzo.citta) || !string.IsNullOrEmpty(_indirizzo.localita))
                    {
                        res += _builder.NewLine() + _indirizzo.cap;
                        if (!string.IsNullOrEmpty(_indirizzo.cap) && !string.IsNullOrEmpty(_indirizzo.citta)) res += "-";
                        res += _indirizzo.citta;
                        if (!string.IsNullOrEmpty(_indirizzo.citta) && !string.IsNullOrEmpty(_indirizzo.localita)) res += "-";
                        res += _indirizzo.localita;
                    }
                    return res;
                }
            }

            public string Telefono
            {
                get
                {
                    string res = Descrizione;
                    if (!string.IsNullOrEmpty(TelefonoNoDescr))
                    {
                        res += _builder.NewLine() + TelefonoNoDescr;
                    }
                    return res;
                }
            }

            private string TelefonoNoDescr
            {
                get
                {
                    string res = string.Empty;
                    if (_indirizzo == null) return res;
                    if (!string.IsNullOrEmpty(_indirizzo.telefono1) || !string.IsNullOrEmpty(_indirizzo.telefono2))
                    {
                        res += _indirizzo.telefono1;
                        if (!string.IsNullOrEmpty(_indirizzo.telefono1) && !string.IsNullOrEmpty(_indirizzo.telefono2)) res += "-";
                        res += _indirizzo.telefono2;
                    }
                    return res;
                }
            }

            public string IndirizzoTelefono
            {
                get
                {
                    string res = Indirizzo;
                    if (!string.IsNullOrEmpty(TelefonoNoDescr))
                    {
                        res += _builder.NewLine() + TelefonoNoDescr;
                    }
                    return res;
                }
            }


        }
        #endregion

        private async Task<DocsPaVO.utente.Corrispondente> GetDettagliIndirizzoCorrispondente(string systemId)
        {
            DocsPaVO.utente.Corrispondente corr = new();

            var dett = await (from c in this._dbContext.DettGlobaliEntities.AsNoTracking()
                              where c.ID_CORR_GLOBALI == systemId.AsLong()
                              select c).FirstOrDefaultAsync();

            if (dett != null)
            {
                corr.indirizzo = dett.VAR_INDIRIZZO;
                corr.cap = dett.VAR_CAP;
                corr.prov = dett.VAR_PROVINCIA;
                corr.citta = dett.VAR_CITTA;
                corr.localita = dett.VAR_LOCALITA;
                corr.telefono1 = dett.VAR_TELEFONO;
                corr.telefono2 = dett.VAR_TELEFONO2;
                corr.note = dett.VAR_NOTE;
            }

            return corr;
        }


        private class RtfBuilder
        {
            private Dictionary<string, string> _placeholders;
            private int MAX_CHECK_LENGTH = 1000;

            public RtfBuilder()
            {
                _placeholders = new Dictionary<string, string>();
            }
            public void AddTextPlaceholder(string placeholder, string[] texts, TextPosition position)
            {
                string separator = " ";
                if (position == TextPosition.VERTICAL) separator = NewLine();
                string res = formatValue(texts[0]);
                for (int i = 1; i < texts.Length; i++)
                {
                    res += separator + formatValue(texts[i]);
                }
                string placeholderName = formatPlaceholder(placeholder).ToUpper();
                if (!_placeholders.ContainsKey(placeholderName)) _placeholders.Add(placeholderName, res);
            }
            public void AddTextPlaceholder(string placeholder, string text)
            {
                if (!_placeholders.ContainsKey(formatPlaceholder(placeholder).ToUpper())) _placeholders.Add(formatPlaceholder(placeholder).ToUpper(), formatValue(text));
            }

            public void AddLinkPlaceholder(string placeholder, string label, string href)
            {
                string placeholderName = formatPlaceholder(placeholder).ToUpper();
                if (!_placeholders.ContainsKey(placeholderName)) _placeholders.Add(placeholderName, "{\\field{\\*\\fldinst{HYPERLINK \"" + href + "\"}}{\\fldrslt {" + formatValue(label) + "}}}");
            }

            private string formatPlaceholder(string placeholder)
            {
                return "#" + placeholder + "#";
            }

            private string formatValue(string value)
            {
                if (!string.IsNullOrEmpty(value))
                 return value.Replace("\\", "\\'5C").Replace("\\'5Cpar", "\\par");
                else
                    return string.Empty;
            }

            public byte[] ReplacePlaceholders(byte[] input)
            {
                List<byte> res = new List<byte>();
                for (int i = 0; i < input.Length; i++)
                {
                    char temp = (char)input[i];
                    if (temp == '#')
                    {
                        i = HandlePlaceholder(i, input, res);
                    }
                    else
                    {
                        res.Add(input[i]);
                    }
                }
                return res.ToArray();
            }

            private int HandlePlaceholder(int index, byte[] input, List<byte> output)
            {
                string placeholder = "#";
                int j = 0;
                bool endFound = false;
                while (j < MAX_CHECK_LENGTH && !endFound)
                {
                    j++;
                    placeholder = placeholder + (char)input[index + j];
                    if (input[index + j] == '#') endFound = true;
                }
                string value = placeholder;
                string key = GetTextFromRtf(placeholder).ToUpper();
                if (_placeholders.ContainsKey(key))
                {
                    value = _placeholders[key];
                }
                foreach (char temp in value)
                {
                    output.Add((byte)temp);
                }
                return index + j;
            }

            private string GetTextFromRtf(string rtf)
            {
                Regex rex = new Regex("(\\\\\\n?[A-Za-z0-9]+[ ]?)|(})|({)|(\\r)(\\n)", RegexOptions.None, TimeSpan.FromSeconds(5));
                string temp1 = rex.Replace(rtf, "");

                Regex rex2 = new Regex("([ ]+)", RegexOptions.None, TimeSpan.FromSeconds(5));
                string temp2 = rex2.Replace(temp1, " ");

                return temp2;
            }

            public string NewLine()
            {
                return " \\par ";
            }
        }

        private byte[] GetFileContent(string path)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new System.IO.FileInfo(path).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }

        private async Task<DocsPaVO.ProfilazioneDinamica.Templates> GetTemplate(InfoUtente userInfo, string templateName)
        {
            var tempList = await this.GetTemplates(userInfo.idAmministrazione);
            DocsPaVO.ProfilazioneDinamica.Templates template = null;
            foreach (object obj in tempList)
            {
                string descrizione = ((Templates)obj).DESCRIZIONE;
                if (templateName.ToUpper().Equals(descrizione.ToUpper()))
                {
                    template = (await this._mediator.Send(new Application.Requests.getTemplateById("" + ((Templates)obj).SYSTEM_ID))).output;
                }
            }
            if (template == null)
            {
                throw new Exception(Trasm.TempNotFound + templateName);
            }
            return template;

        }

        private async Task<List<DocsPaVO.ProfilazioneDinamica.Templates>> GetTemplates(string idAmministrazione)
        {
            var templates = await (from t in this._dbContext.TipoAttoEntities.AsNoTracking()
                                   where t.ID_AMM == idAmministrazione.AsLong() 
                                   && t.ABILITATO_SI_NO != null 
                                   && t.ABILITATO_SI_NO == 1
                                   select t).OrderBy(t => t.VAR_DESC_ATTO).ToListAsync();

            List<DocsPaVO.ProfilazioneDinamica.Templates> output = new();
            templates.ForEach(t =>
            {
                DocsPaVO.ProfilazioneDinamica.Templates temp = new()
                {
                    SYSTEM_ID = Convert.ToInt32(t.SYSTEM_ID),
                    ID_TIPO_ATTO = t.SYSTEM_ID.ToString(),
                    DESCRIZIONE = t.VAR_DESC_ATTO,
                    ABILITATO_SI_NO = t.ABILITATO_SI_NO.ToString(),
                    IN_ESERCIZIO = t.IN_ESERCIZIO,
                    PATH_MODELLO_1 = t.PATH_MOD_1,
                    PATH_MODELLO_2 = t.PATH_MOD_2,
                    PATH_MODELLO_1_EXT = t.EXT_MOD_1,
                    PATH_MODELLO_2_EXT = t.EXT_MOD_2,
                    PATH_MODELLO_STAMPA_UNIONE = t.PATH_MOD_SU,
                    PATH_MODELLO_EXCEL = t.PATH_MOD_EXC,
                    PATH_XSD_ASSOCIATO = t.PATH_XSD_ASSOCIATO,
                    PATH_ALLEGATO_1 = t.PATH_ALL_1,
                    SCADENZA = t.GG_SCADENZA.ToString(),
                    PRE_SCADENZA = t.GG_PRE_SCADENZA.ToString(),
                    PRIVATO = t.CHA_PRIVATO ?? "0",
                    ID_AMMINISTRAZIONE = t.ID_AMM.ToString(),
                    CODICE_CLASSIFICA = t.COD_CLASS,
                    CODICE_MODELLO_TRASM = t.COD_MOD_TRASM,
                    IPER_FASC_DOC = (t.IPERDOCUMENTO != null && t.IPERDOCUMENTO == 1) ? "1" : "0",
                    NUM_MESI_CONSERVAZIONE = t.NUM_MESI_CONSERVAZIONE.ToString(),
                    IS_TYPE_INSTANCE = Convert.ToChar(t.IS_TYPE_INSTANCE),
                    INVIO_CONSERVAZIONE = t.CHA_INVIO_CONSERVAZIONE ?? "0",
                    CHA_ASSOC_MANUALE = t.CHA_ASSOC_MANUALE ?? "0",
                    ID_CONTESTO_PROCEDURALE = t.ID_CONTESTO_PROCEDURALE.ToString()
                };
                output.Add(temp);
            });

            return output;
        }

        private string GetIdentificationData(DocsPaVO.documento.SchedaDocumento schedaDocumento, bool isGray)
        {
            // La stringa da restituire
            string toReturn = string.Empty;

            // Se docType è un documento grigio
            if (isGray)
                // L'identificativo è l'id del documento
                toReturn = string.Format(Trasm.MissingId, schedaDocumento.systemId);
            else
                // Altrimenti l'identificativo è costituito dalla segnatura e dall'id del documento
                toReturn = string.Format(Trasm.IdWIthSig, schedaDocumento.systemId, schedaDocumento.protocollo.segnatura);

            // Restituzione della descrizione
            return toReturn;

        }

        private async Task<List<string>> SaveDocumentInWorkingArea(DocsPaVO.documento.SchedaDocumento schedaDocumento, InfoUtente userInfo, Ruolo role)
        {
            Funzione[] functions = role.funzioni;
            List<string> toReturn = new List<string>();
            Funzione canAddInADL = functions.Where(e => e.codice == "DO_ADD_ADL").FirstOrDefault();

            if (canAddInADL == null)
                toReturn.Add(Resources.CantAddInAdlRole);
            else
            {
                try
                {
                    await this._mediator.Send(new Application.Requests.DocumentoExecAddLavoro(schedaDocumento.systemId, schedaDocumento.tipoProto, null, userInfo, schedaDocumento.registro.systemId));
                }
                catch (Exception e)
                {
                    toReturn.Add(Resources.CantAddInAdl);
                }

            }

            return toReturn;
        }



        private async Task<List<string>> TransmitDocument(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocumentRowData rowData, InfoUtente userInfo, Ruolo role, string serverPath)
        {
            #region Dichiarazione variabili

            // La lista dei problemi
            List<string> problems = new List<string>();

            // Il modello di trasmissione da utilizzare per inviare il fascicolo
            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione transmModel = null;

            // Il risultato dell'operazione di invio
            bool trasmRes;

            // Un valore utilizzato per tenere traccia del fatto che si è
            // verificata un'eccezione
            bool haveException = false;

            #endregion
            List<string> tempList = rowData.TransmissionModelCode.Where(e => !string.IsNullOrEmpty(e)).ToList();
            // Per ogni codice di modello di trasmissione...
            foreach (string modelCode in tempList)
            {
                // Azzeramento del flag eccezione
                haveException = false;

                // Azzeramento del flag trasmRes
                trasmRes = false;

                try
                {
                    // Si prova ad effettuare la trasmissione
                    (trasmRes, transmModel) = (await this.TransmissionExecuteDocTransmFromModelCode(
                        userInfo,
                        serverPath,
                        schedaDocumento,
                        modelCode,
                        role));

                }
                catch (Exception e)
                {
                    haveException = true;

                }

                // Se si è verificata un'eccezione o se la trasmissione non è partita,
                // significa che si è verificato qualche problema
                if (!trasmRes || haveException)
                {
                    // Se trasmModel è valorizzato e non si riferisce a documenti
                    if (transmModel != null && transmModel.CHA_TIPO_OGGETTO != "D")
                        // Si segnala il problema all'utente
                        problems.Add(
                        string.Format(Trasm.TrasmModelError, modelCode.Trim()));
                    else
                        // altrimenti si segnala un errore generico
                        if (haveException)
                        problems.Add(string.Format(Trasm.TrasmGenErr, schedaDocumento.systemId, modelCode.Trim()));
                    else
                        // altrimenti il modello non è stato reprito correttamente
                        problems.Add(string.Format(Trasm.TrasmValoreNonRep, modelCode));
                }

            }

            return problems;

        }




        private async Task<(bool, DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione)> TransmissionExecuteDocTransmFromModelCode(DocsPaVO.utente.InfoUtente userInfo, string serverPath, DocsPaVO.documento.SchedaDocumento scheda,
            string modelCode, DocsPaVO.utente.Ruolo role)
        {
            int lastUnderscore = -1;

            // L'id del modello da recuperare
            string modelId = string.Empty;

            // Il modello recuperato
            DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione model = null;

            // L'oggetto trasmissione
            DocsPaVO.trasmissione.Trasmissione transmission = null;

            // Un oggetto ragione destinatario utilizzato durante la creazione delle
            // trasmissioni singole
            DocsPaVO.Modelli_Trasmissioni.RagioneDest ragDest;

            // L'array list dei destinatari
            ArrayList destinatari;

            // Le informazioni sul corrispondente cui inviare la trasmissione
            DocsPaVO.utente.Corrispondente corr;

            // La ragione di trasmissione
            DocsPaVO.trasmissione.RagioneTrasmissione ragione;

            // Il valore da restituire
            bool output = false;



            try
            {
                lastUnderscore = modelCode.LastIndexOf('_');

                // Se è stato trovato un '_' si preleva l'id altrimenti l'id è pari al codice
                if (lastUnderscore != -1)
                    modelId = modelCode.Substring(lastUnderscore + 1);
                else
                    modelId = modelCode;


                model = (await this._mediator.Send(new Application.Requests.getModelloByID(role.idAmministrazione, modelId))).output;

                if (model != null && model.CHA_TIPO_OGGETTO == "D")
                {
                    transmission = new DocsPaVO.trasmissione.Trasmissione();
                    transmission.noteGenerali = model.VAR_NOTE_GENERALI;

                    transmission.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                    transmission.infoDocumento = (await this._mediator.Send(new Application.Requests.GetInfoDocumento(userInfo, scheda.systemId, scheda.docNumber))).output;

                    transmission.utente = await this.GetUtente(userInfo.idPeople);
                    transmission.ruolo = (await this._mediator.Send(new Application.Requests.GetRuolo(userInfo.idCorrGlobali))).output;

                    transmission.NO_NOTIFY = model.NO_NOTIFY;


                    for (int i = 0; i < model.RAGIONI_DESTINATARI.Count(); i++)
                    {
                        // Recupero delli ragioni di trasmissione
                        ragDest = (DocsPaVO.Modelli_Trasmissioni.RagioneDest)model.RAGIONI_DESTINATARI[i];

                        // Creazione della lista dei destinatari
                        destinatari = new ArrayList(ragDest.DESTINATARI);

                        // Per ogni destinatario...
                        foreach (DocsPaVO.Modelli_Trasmissioni.MittDest mittDest in destinatari)
                        {
                            // ...prelevamento dei dati sul corrispondente
                            corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteByCodRubrica(mittDest.VAR_COD_RUBRICA, userInfo, "", false))).output;

                            // ...prelevamento della ragione di trasmissione
                            ragione = (await this._mediator.Send(new Application.Requests.getRagioneById(mittDest.ID_RAGIONE.ToString()))).output;

                            // ...aggiunta della trasmissione singola
                            transmission = await this.AddTrasmissioneSingola(
                                transmission,
                                corr,
                                ragione,
                                mittDest.VAR_NOTE_SING,
                                mittDest.CHA_TIPO_TRASM);
                        }
                    }

                    if (userInfo.delegato != null)
                        // ...impostazione dell'idPeople del delegato
                        transmission.delegato = ((DocsPaVO.utente.InfoUtente)(userInfo.delegato)).idPeople;

                    transmission = (await this._mediator.Send(new Application.Requests.TrasmissioneSaveExecuteTrasm(serverPath, transmission, userInfo))).output;

                    // Il risultato da restituire è successo
                    output = true;
                }

            }
            catch (Exception ex)
            {
                output = false;
            }

            return (output, model);

        }



        private async Task<DocsPaVO.trasmissione.Trasmissione> AddTrasmissioneSingola(DocsPaVO.trasmissione.Trasmissione trasmissione, DocsPaVO.utente.Corrispondente corr, DocsPaVO.trasmissione.RagioneTrasmissione ragione, string note, string tipoTrasm)
        {
            List<DocsPaVO.trasmissione.TrasmissioneSingola> trasmSingola = new();
            List<DocsPaVO.trasmissione.TrasmissioneUtente> trasmUtente = new();

            if (corr != null && corr.tipoCorrispondente == "R")
            {
                DocsPaVO.utente.Corrispondente corrispondente = (await this._mediator.Send(new Application.Requests.GetRuoloById(corr.systemId))).Output;
                if (corrispondente != null && corrispondente.disabledTrasm)
                    return trasmissione;
            }

            if (trasmissione.trasmissioniSingole != null)
            {
                // controllo se esiste la trasmissione singola associata a corrispondente selezionato
                for (int i = 0; i < trasmissione.trasmissioniSingole.Count(); i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneSingola ts = (DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i];
                    if (ts.corrispondenteInterno.systemId != null && ts.corrispondenteInterno.systemId.Equals(corr.systemId))
                    {
                        if (ts.daEliminare)
                        {
                            ((DocsPaVO.trasmissione.TrasmissioneSingola)trasmissione.trasmissioniSingole[i]).daEliminare = false;
                            return trasmissione;
                        }
                        else
                            return trasmissione;
                    }
                }
            }

            // Aggiungo la trasmissione singola
            DocsPaVO.trasmissione.TrasmissioneSingola trasmissioneSingola = new DocsPaVO.trasmissione.TrasmissioneSingola();
            trasmissioneSingola.tipoTrasm = tipoTrasm;
            trasmissioneSingola.corrispondenteInterno = corr;
            trasmissioneSingola.ragione = ragione;
            trasmissioneSingola.noteSingole = note;

            // Aggiungo la lista di trasmissioniUtente
            if (corr is DocsPaVO.utente.Ruolo)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                var listaUtenti = await QueryUtenti(corr);
                if (listaUtenti.Count() == 0)
                {
                    trasmissione.listaDestinatariNonRaggiungibili.Add("Nessun utente per il ruolo " + corr.codiceCorrispondente + " (" + corr.descrizione + ").");
                    trasmissioneSingola = null;
                }
                //ciclo per utenti se dest è gruppo o ruolo
                for (int i = 0; i < listaUtenti.Count(); i++)
                {
                    DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                    trasmissioneUtente.utente = (DocsPaVO.utente.Utente)listaUtenti[i];
                    trasmUtente.Add(trasmissioneUtente);
                }
            }

            if (corr is DocsPaVO.utente.Utente)
            {
                trasmissioneSingola.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.UTENTE;
                DocsPaVO.trasmissione.TrasmissioneUtente trasmissioneUtente = new DocsPaVO.trasmissione.TrasmissioneUtente();
                trasmissioneUtente.utente = (DocsPaVO.utente.Utente)corr;
                trasmUtente.Add(trasmissioneUtente);
            }

            trasmissioneSingola.trasmissioneUtente = trasmUtente.ToArray();

            if (corr is DocsPaVO.utente.UnitaOrganizzativa)
            {
                DocsPaVO.utente.UnitaOrganizzativa theUo = (DocsPaVO.utente.UnitaOrganizzativa)corr;
                DocsPaVO.addressbook.QueryCorrispondenteAutorizzato qca = new DocsPaVO.addressbook.QueryCorrispondenteAutorizzato();
                qca.ragione = trasmissioneSingola.ragione;
                qca.ruolo = trasmissione.ruolo;
                qca.queryCorrispondente = new DocsPaVO.addressbook.QueryCorrispondente();
                qca.queryCorrispondente.fineValidita = true;

                var ruoli = (await this._mediator.Send(new Application.Requests.AddressbookGetRuoliRiferimentoAutorizzati(qca, theUo))).output;

                if (ruoli == null || ruoli.Count() == 0)
                {
                    //Popola una lista con tutti i destinatari non raggiungibili
                    trasmissione.listaDestinatariNonRaggiungibili.Add("Manca un ruolo di riferimento per la UO: " + corr.codiceCorrispondente + " (" + corr.descrizione + ") " + ".");
                }
                foreach (DocsPaVO.utente.Ruolo r in ruoli)
                    trasmissione = await AddTrasmissioneSingola(trasmissione, r, ragione, note, tipoTrasm);

                return trasmissione;
            }

            if (trasmissioneSingola != null)
                trasmSingola.Add(trasmissioneSingola);

            trasmissione.trasmissioniSingole = trasmSingola.ToArray();

            return trasmissione;
        }

        private async Task<DocsPaVO.utente.Corrispondente[]> QueryUtenti(DocsPaVO.utente.Corrispondente corr)
        {

            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();

            qco.codiceRubrica = corr.codiceRubrica;
            qco.getChildren = true;
            qco.idAmministrazione = corr.idAmministrazione;
            qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
            qco.fineValidita = true;

            return (await this._mediator.Send(new Application.Requests.AddressbookGetListaCorrispondenti(qco))).output;
        }


        private async Task<DocsPaVO.utente.Utente> GetUtente(string idPeople)
        {
            DocsPaVO.utente.Utente utente = null;
            var ut = await (from p in this._dbContext.PeopleEntities.AsNoTracking()
                            where (p.DISABLED == null || p.DISABLED.ToUpper().Equals("Y")) && p.SYSTEM_ID == idPeople.AsLong()
                            select new
                            {
                                p.SYSTEM_ID,
                                p.USER_ID,
                                p.ID_AMM,
                                p.VAR_COGNOME,
                                p.VAR_NOME,
                                p.VAR_TELEFONO,
                                p.EMAIL_ADDRESS,
                                p.FROM_EMAIL_ADDRESS,
                                p.CHA_NOTIFICA,
                                p.CHA_AMMINISTRATORE,
                                p.CHA_NOTIFICA_CON_ALLEGATO,
                                p.VAR_SEDE,
                                p.MATRICOLA
                            }).FirstOrDefaultAsync();

            if (ut != null)
            {

                utente = new DocsPaVO.utente.Utente();
                utente.idPeople = ut.SYSTEM_ID.ToString();
                utente.userId = ut.USER_ID;
                utente.descrizione = ut.VAR_COGNOME + " " + ut.VAR_NOME;
                utente.telefono = ut.VAR_TELEFONO;
                utente.email = ut.EMAIL_ADDRESS;
                utente.notifica = ut.CHA_NOTIFICA;
                utente.amministratore = FromCharToBool(ut.CHA_AMMINISTRATORE);
                utente.assegnante = FromCharToBool(ut.CHA_AMMINISTRATORE);
                utente.assegnatario = FromCharToBool(ut.CHA_AMMINISTRATORE);
                utente.idAmministrazione = ut.ID_AMM != null ? ut.ID_AMM.ToString() : null;
                utente.notificaConAllegato = FromCharToBool(ut.CHA_NOTIFICA_CON_ALLEGATO);
                utente.sede = ut.VAR_SEDE;
                utente.matricola = ut.MATRICOLA;
                utente.tipoCorrispondente = "P";
                utente.cognome = ut.VAR_COGNOME;
                utente.nome = ut.VAR_NOME;

            }

            return utente;
        }

        private bool FromCharToBool(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Equals("1"))
                return true;
            else
                return false;
        }


        private async Task<DocsPaVO.documento.SchedaDocumento> GetDataProtocollo(DocsPaVO.documento.SchedaDocumento schedaDoc)
        {

            string data = schedaDoc.protocollo.dataProtocollazione;

            if (data != null)
            {

                data = data.Trim();

            }

            bool protocollazioneLibera = false;
            (string val, bool keyFound) = await this._configurationService.TryGetValue<string>("PROTOCOLLAZIONE_LIBERA");
            if (keyFound && !string.IsNullOrEmpty(val) && val.ToUpper().Equals("TRUE"))
            {
                protocollazioneLibera = bool.Parse(val.ToLower());
            }

            bool flagWSPIA = false;

            if (schedaDoc != null && schedaDoc.registro.FlagWspia != null && schedaDoc.registro.FlagWspia.Equals("1"))
                flagWSPIA = true;
            else
                flagWSPIA = false;

            protocollazioneLibera = protocollazioneLibera && flagWSPIA;


            if (!protocollazioneLibera)
            {

                if (!(data != null && !data.Equals("")))
                {
                    if (schedaDoc.registro.dataApertura.IndexOf(" ") > 0)
                    {
                        schedaDoc.protocollo.dataProtocollazione = schedaDoc.registro.dataApertura.Substring(0, schedaDoc.registro.dataApertura.IndexOf(" "));
                    }
                    else
                    {
                        schedaDoc.protocollo.dataProtocollazione = schedaDoc.registro.dataApertura;
                    }
                    data = schedaDoc.protocollo.dataProtocollazione.Trim();
                }
                schedaDoc.protocollo.anno = schedaDoc.registro.dataApertura.Substring(schedaDoc.registro.dataApertura.LastIndexOf("/") + 1);

            }
            else
                if (string.IsNullOrEmpty(schedaDoc.protocollo.anno))
            {
                schedaDoc.protocollo.anno = schedaDoc.registro.dataApertura.Substring(schedaDoc.registro.dataApertura.LastIndexOf("/") + 1);
            }

            return schedaDoc;
        }



        protected async Task AddDocToProjects(InfoUtente userInfo, string idProfile, List<string> projectIds, bool isRapidClassificationRequired)
        {
            DocsPaVO.fascicolazione.Fascicolo f = null;
            bool firstProject = true;


            foreach (string projectId in projectIds)
            {
                f = (await this._mediator.Send(new Application.Requests.FascicolazioneGetFascicoloById(projectId, userInfo))).output;
                await this._mediator.Send(new Application.Requests.FascicolazioneAddDocFascicolo(userInfo, idProfile, f, firstProject));
                firstProject = false;
            }

        }


        private async Task<List<DocsPaVO.documento.TipologiaAtto>> GetTipoAttoByRuoloAndPianoConsIdFasc(string idAmministrazione, string idGruppo, string diritti, List<string> idProject)
        {

            var baseQuery = (from a in this._dbContext.TipoAttoEntities.AsNoTracking()
                             from b in this._dbContext.VisTipoDocEntities.AsNoTracking()
                             where (a.ID_AMM == null || a.ID_AMM == idAmministrazione.AsLong()) &&
                             a.SYSTEM_ID == b.ID_TIPO_DOC
                             select new TipoAttoWithVis()
                             {
                                 A = a,
                                 B = b
                             });


            var predicate = PredicateBuilder.New<TipoAttoWithVis>();

            var subQueryRes = await (from p in this._dbContext.ProjectEntities.AsNoTracking()
                                     from c in this._dbContext.PianoConservazioneEntities.AsNoTracking()
                                     from a in this._dbContext.PianoConsTipoAttoEntities.AsNoTracking()
                                     where idProject.Contains(p.SYSTEM_ID.ToString()) && (c.SYSTEM_ID == p.ID_PIANO_CONSERVAZIONE ||
                                     (c.ID_CLASSIFICAZIONE == p.ID_PARENT && p.CHA_TIPO_FASCICOLO != null && p.CHA_TIPO_FASCICOLO.Equals("G"))) && a.ID_PIANO_CONSERVAZIONE == c.SYSTEM_ID
                                     select a.ID_TIPO_ATTO
                             ).ToListAsync();

            predicate = predicate.And(row => subQueryRes.Contains(row.A.SYSTEM_ID));
            List<long?> rights;
            if (diritti == "1")
            {
                rights = new() { 1, 2 };
                predicate = predicate.And((row => row.B.ID_RUOLO == 0 || (row.B.ID_RUOLO == idGruppo.AsLong() && rights.Contains(row.B.DIRITTI))));
                predicate = predicate.Or(row => row.A.IPERDOCUMENTO == 1 && (row.A.ID_AMM == null || row.A.ID_AMM == idAmministrazione.AsLong()));

            }
            if (diritti == "2")
            {
                rights = new() { 2 };
                predicate = predicate.And(row => (row.A.IN_ESERCIZIO == null || !row.A.IN_ESERCIZIO.Equals("NO")) && (row.A.ABILITATO_SI_NO == null || row.A.ABILITATO_SI_NO != 0));
                predicate = predicate.And(row => row.B.ID_RUOLO == 0 || (row.B.ID_RUOLO == idGruppo.AsLong() && rights.Contains(row.B.DIRITTI)));
            }


            baseQuery = baseQuery.DistinctBy(row => new
            {
                row.A.SYSTEM_ID,
                row.A.VAR_DESC_ATTO,
                row.A.PESO
            }).OrderByDescending(row => row.A.PESO).ThenBy(row => row.A.VAR_DESC_ATTO);

            List<DocsPaVO.documento.TipologiaAtto> output = new();

            foreach (var rowTa in baseQuery)
            {
                DocsPaVO.documento.TipologiaAtto t = new()
                {
                    systemId = rowTa.A.SYSTEM_ID.ToString(),
                    descrizione = rowTa.A.VAR_DESC_ATTO
                };
                output.Add(t);
            }

            return output;
        }

        private class TipoAttoWithVis
        {
            public TipoAttoEntity A { get; set; }
            public VisTipoDocEntity B { get; set; }
        }

        private async Task<(List<string>, List<string>)> GetProjectsForClassification(DocumentRowData rowData, InfoUtente userInfo, Ruolo role, string administrationSyd, string registrySyd, string rfSyd, string idTitolario, bool isEnabledSmistamento)
        {
            var registry = await this.GetRegistroById(registrySyd);
            List<string> problems = new();
            DocsPaVO.fascicolazione.Fascicolo fascicolo = null;
            List<string> projectIds = new();

            if (registry == null)
            {
                problems.Add(string.Format(Resources.RegCodeNotRf, rowData.RegCode));
            }

            if (rowData.ProjectCodes != null && registry != null)
            {
                foreach (string cod in rowData.ProjectCodes)
                {
                    fascicolo = (await this._mediator.Send(new Application.Requests.FascicolazioneGetFascicoloDaCodice2(administrationSyd, role.idGruppo, userInfo.idPeople,
                    cod, registry, true, true, idTitolario))).output;

                    if (fascicolo != null && !string.IsNullOrEmpty(fascicolo.systemID))
                    {
                        projectIds.Add(fascicolo.systemID);
                    }
                    else
                    {
                        problems.Add(string.Format(Resources.FascNotFound, cod));
                    }
                }

            }
            else
            {
                var filters = await this.GetFilters(
                    rowData,
                    registrySyd,
                    rfSyd,
                    idTitolario,
                    administrationSyd,
                    role,
                    userInfo,
                    isEnabledSmistamento);

                var projects = (await this._mediator.Send(new Application.Requests.FascicolazioneGetListaFascicoliPagingCustom(
                    userInfo,
                    null,
                    registry,
                    filters,
                    false,
                    true,
                    false,
                    1,
                    2,
                    true,
                    null,
                    false,
                    false,
                    null,
                    null,
                    false
                    ))).idProjectList;
                if (projects.Count() > 1)
                {
                    problems.Add(Resources.ProjectProblem);
                }
                else
                    projectIds.Add(projects[0].Id);
            }
            return (projectIds, problems);
        }

        private async Task<FiltroRicerca[]> GetFilters(DocumentRowData rowData, string registrySyd, string rfSyd, string idTitolario, string administrationSyd, Ruolo role, InfoUtente userInfo, bool isSmistamentoEnabled)
        {
            #region Dichiarazione variabili

            // La lista dei filtri da restituire
            List<FiltroRicerca> toReturn;

            // Filtro temporaneo
            FiltroRicerca tempFilter;

            // La lista dei template
            Templates[] templates = null;

            // Il template da utilizzare per la profilazione
            Templates template = null;

            // I campi
            string[] values;

            // Il tipo di utente da ricercare
            TipoUtente userType;

            #endregion

            // Creazione della lista dei filtri
            toReturn = new List<FiltroRicerca>();

            #region Filtro Codice Fascicolo

            // Se è valorizzato il codice fascicolo, si procede alla creazione
            // di un filtro per esso
            if (rowData.ProjectCodes != null && !string.IsNullOrEmpty(rowData.ProjectCodes[0]))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "NUMERO_FASCICOLO";

                // Impostazione del valore
                tempFilter.valore = rowData.ProjectCodes[0];

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);

            }

            #endregion

            #region Filtro Descrizione Fascicolo

            // Se è valorizzata la descrizione del fascicolo,
            // si procede alla creazione di un filtro per essa
            if (!string.IsNullOrEmpty(rowData.ProjectDescription))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "TITOLO";

                // Impostazione del valore
                tempFilter.valore = rowData.ProjectDescription;

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);
            }

            #endregion

            #region Filtro Descrizione Sottofascicolo

            // Se è valorizzata la descrizione del sottofascicolo,
            // si procede alla creazione di un filtro per essa
            if (!string.IsNullOrEmpty(rowData.FolderDescrition))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "SOTTOFASCICOLO";

                // Impostazione del valore
                tempFilter.valore = rowData.FolderDescrition;

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);
            }

            #endregion

            #region Filtro Titolario

            // Se il parametro idTitolario è valorizzato,
            // viene creato un filtro per esso
            if (!string.IsNullOrEmpty(idTitolario))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "ID_TITOLARIO";

                // Impostazione del valore
                tempFilter.valore = idTitolario;

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);
            }

            #endregion

            #region Filtro Codice Nodo

            // Se è valorizzato il codice nodo,
            // si procede alla creazione di un filtro per esso
            if (!string.IsNullOrEmpty(rowData.NodeCode))
            {
                // Creazione di un filtro temporaneo
                tempFilter = new FiltroRicerca();

                // Impostazione del nome del campo
                tempFilter.argomento = "CODICE_CLASSIFICA";

                // Impostazione del valore
                tempFilter.valore = rowData.NodeCode;

                // Aggiunta del filtro alla lista dei filtri
                toReturn.Add(tempFilter);
            }

            #endregion

            #region Filtro Tipologia Fascicolo e Campi Profilati

            // Se è valorizzato il campo tipologia, vengono creati,
            // i filtri di ricerca per la profilazione
            if (!string.IsNullOrEmpty(rowData.ProjectTipology))
            {

                try
                {
                    // Prelevamento della lista dei template creati per l'amministrazione
                    templates = (await this._mediator.Send(new Application.Requests.getTipoFascFromRuolo(
                        administrationSyd,
                        role.idGruppo,
                        "1"))).output;

                    // Prelevamento del template di interesse
                    template = (await this._mediator.Send(new Application.Requests.getTemplateFascById(templates.Where(
                        e => e.DESCRIZIONE.ToUpper().TrimEnd().TrimStart() == rowData.ProjectTipology.TrimEnd().TrimStart()).
                        FirstOrDefault().SYSTEM_ID.ToString()))).output;
                }
                catch (Exception e)
                {
                }

                // Se è stato rilevato un template, si procede con la
                // compilazione dei campi relativi alla profilazione
                if (template != null)
                {
                    // Il primo campo riporta l'id del template
                    // Creazione di un filtro temporaneo
                    tempFilter = new FiltroRicerca();

                    // Impostazione della tipologia fascicolo
                    tempFilter.argomento = "TIPOLOGIA_FASCICOLO";

                    // Impostazione del valore
                    tempFilter.valore = template.SYSTEM_ID.ToString();

                    // Aggiunta del filtro alla lista dei filtri
                    toReturn.Add(tempFilter);

                    // Aggiunta delle informazioni sul template
                    tempFilter = new FiltroRicerca();

                    // Impostazione dell'argomento
                    tempFilter.argomento = "PROFILAZIONE_DINAMICA";

                    // Impostazione del valore
                    tempFilter.valore = " Profilazione Dinamica";

                    // Impostazione del template
                    tempFilter.template = template;

                    // Per ogni oggetto custom...
                    foreach (OggettoCustom obj in template.ELENCO_OGGETTI)
                    {
                        // ...switch sul tipo di oggetto
                        switch (obj.TIPO.DESCRIZIONE_TIPO.ToUpper())
                        {
                            case "CONTATORE":
                                try
                                {
                                    // Prelevamento dei valori assegnati al campo
                                    values = rowData.GetProjectProfilationField(obj.DESCRIZIONE);

                                    // Nel caso di contatore bisogna impostare per prima cosa
                                    // l'id del registro
                                    obj.ID_AOO_RF = await this.GetIdRegistro(
                                        values[0],
                                        rowData.AdminCode);

                                    // Se l'array dei valori contiene il secondo elemento, ...
                                    if (values.Length > 1)
                                        // ...viene impostato il valore minimo
                                        obj.VALORE_DATABASE = values[1] + "@";

                                    // Se l'array contiene il terzo campo...
                                    if (values.Length > 2)
                                        // ...viene impostato il valore massimo
                                        obj.VALORE_DATABASE += values[2];

                                }
                                catch (Exception e)
                                { }

                                break;

                            case "DATA":
                                // Prelevamento dei valori associati al campo
                                values = rowData.GetProjectProfilationField(obj.DESCRIZIONE);

                                // Se l'array è valorizzato...
                                if (values != null)
                                {
                                    // ...se contiene il primo elemento...
                                    if (values.Length > 0)
                                        // ...il primo campo è il valore data minimo
                                        obj.VALORE_DATABASE = values[0] + "@";

                                    // ...se è presente anche il secondo elemento...
                                    if (values.Length > 1)
                                        // ...il secondo è il valore di data massima
                                        obj.VALORE_DATABASE += values[1];

                                }

                                break;

                            case "CORRISPONDENTE":
                                try
                                {
                                    switch (obj.TIPO_RICERCA_CORR.ToUpper())
                                    {
                                        case "INTERNO":
                                            userType = TipoUtente.INTERNO;
                                            break;

                                        case "ESTERNO":
                                            userType = TipoUtente.ESTERNO;
                                            break;

                                        default:
                                            userType = TipoUtente.GLOBALE;
                                            break;

                                    }

                                    // Prelevamento del system id del corrispondente
                                    obj.VALORE_DATABASE = (await this.GetCorrispondenteByCode(
                                        ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST,
                                        rowData.GetProjectProfilationField(obj.DESCRIZIONE)[0],
                                        role,
                                        userInfo,
                                        registrySyd,
                                        rfSyd,
                                        isSmistamentoEnabled,
                                        userType)).systemId;

                                }
                                catch (Exception e) { }

                                break;

                            case "CASELLADISELEZIONE":
                                try
                                {
                                    // In questo caso è possibile che siano selezionati
                                    // più valori
                                    obj.VALORI_SELEZIONATI = rowData.GetProjectProfilationField(obj.DESCRIZIONE);
                                }
                                catch (Exception e) { }

                                break;

                            default:
                                try
                                {
                                    // In tutti gli alti casi è ammesso un solo valore
                                    obj.VALORE_DATABASE = rowData.GetProjectProfilationField(obj.DESCRIZIONE)[0];
                                }
                                catch (Exception e)
                                {
                                }

                                break;

                        }

                    }

                    // Aggiunta del filtro alla lista dei filtri
                    toReturn.Add(tempFilter);

                }

            }

            #endregion

            // Restituzione della lista dei filtri
            return toReturn.ToArray<FiltroRicerca>();

        }


        private async Task<(DocsPaVO.documento.SchedaDocumento, List<string>)> GetDocScheda(
            DocumentRowData rowData,
            InfoUtente userInfo,
            Ruolo role,
            string administrationSyd,
            string registrySyd,
            string rfSyd,
            string protoType,
            bool isSmistamentoEnabled,
            bool isEnabledPregressi)
        {
            List<string> problems = new();
            DocsPaVO.documento.SchedaDocumento output = new();

            output = (await this._mediator.Send(new Application.Requests.NewSchedaDocumento(userInfo))).output;


            output.oggetto = await this.LoadDocumentObject(
                userInfo,
                rowData.ObjCode,
                rowData.Obj,
                administrationSyd,
                registrySyd);

            if (rowData.Note != null)
            {
                output.noteDocumento = new List<InfoNota>();
                output.noteDocumento.Add(rowData.Note);
            }
            output.cod_rf_prot = rowData.RFCode.ToUpper();
            output.id_rf_prot = rfSyd;

            output.registro = (await this._mediator.Send(new Application.Requests.GetRegistroBySistemId(registrySyd))).output;
            if (output.registro == null)
                throw new Exception(string.Format(Resources.RegNotFound, rowData.RegCode));

            output.tipoProto = protoType;

            switch (protoType)
            {

                case "A":
                    output.protocollo = await this.CreateProtocolEntObject(rowData, registrySyd, rfSyd, administrationSyd, isSmistamentoEnabled, userInfo, role);
                    break;
                case "G":
                    output.protocollo = this.CreateProtocolGreyObject(rowData, registrySyd, rfSyd, administrationSyd, isSmistamentoEnabled, userInfo, role);
                    break;
                case "I":
                    output.protocollo = await this.CreateProtocolIntObject(rowData, registrySyd, rfSyd, administrationSyd, isSmistamentoEnabled, userInfo, role);
                    break;
                case "P":
                    output.protocollo = await this.CreateProtocolUscObject(rowData, registrySyd, rfSyd, administrationSyd, isSmistamentoEnabled, userInfo, role);
                    break;

            }

            if (isEnabledPregressi && output.protocollo != null && rowData.ProtocolNumber != null)
            {
                output.protocollo.numero = rowData.ProtocolNumber;
                output.protocollo.dataProtocollazione = rowData.ProtocolDate.ToString("dd/MM/yyyy");
            }

            if (isEnabledPregressi)
            {
                string codiceRuolo = null;
                string codiceUtente = null;
                if (!string.IsNullOrEmpty(rowData.CodiceRuoloCreatore))
                    codiceRuolo = rowData.CodiceRuoloCreatore.ToUpper();

                if (!string.IsNullOrEmpty(rowData.CodiceUtenteCreatore))
                    codiceUtente = rowData.CodiceUtenteCreatore.ToUpper();


                if ((codiceRuolo != null) && (codiceUtente != null))
                {
                    DocsPaVO.utente.Ruolo ruolo = (DocsPaVO.utente.Ruolo)(await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteByCodRubricaIE(codiceRuolo, DocsPaVO.addressbook.TipoUtente.INTERNO, userInfo))).output;
                    DocsPaVO.utente.Utente utente = (DocsPaVO.utente.Utente)(await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteByCodRubricaIE(codiceUtente, DocsPaVO.addressbook.TipoUtente.INTERNO, userInfo))).output;

                    bool exists = false;
                    if (ruolo != null)
                    {
                        var ruoli = (await this._mediator.Send(new Application.Requests.GetListaRuoliUtente(utente.idPeople))).output;
                        foreach (Ruolo r in ruoli)
                        {
                            if (r.codiceRubrica == ruolo.codiceRubrica)
                            {
                                exists = true;
                                break;
                            }
                        }
                    }

                    if (exists)
                    {
                        output.creatoreDocumento = new CreatoreDocumento(this.GetInfoUtente(utente, ruolo), ruolo);
                    }
                    else
                    {
                        throw new Exception(string.Format(Resources.UserNotInRole, utente.codiceCorrispondente, ruolo.codiceCorrispondente));
                    }

                }
                else
                {
                    output.creatoreDocumento = new CreatoreDocumento(userInfo, role);
                }

            }

            if (!string.IsNullOrEmpty(rowData.DocumentTipology))
            {
                problems = await this.CompileDocumentProfilationFields(rowData, output, role,
                    userInfo,
                    administrationSyd,
                    isSmistamentoEnabled);
            }



            if (rowData is RDEDocumentRowData)
            {
                // Casting ad oggetto RDEDocumentRowData
                RDEDocumentRowData emergencyData = (RDEDocumentRowData)rowData;

                output.datiEmergenza = new DatiEmergenza();
                output.datiEmergenza.dataProtocollazioneEmergenza = emergencyData.EmergencyProtocolDate + " " + emergencyData.EmergencyProtocolTime;
                output.datiEmergenza.protocolloEmergenza = emergencyData.EmergencyProtocolSignature;

                // Se il protocollo è in Arrivo, vengono impostati i dati sul protocollo mittente
                if (protoType.ToUpper() == "A")
                {
                    ProtocolloEntrata inProto = (ProtocolloEntrata)output.protocollo;

                    inProto.dataProtocolloMittente = emergencyData.SenderProtocolDate;
                    inProto.descrizioneProtocolloMittente = emergencyData.SenderProtocolNumber;
                    ((Documento)output.documenti[0]).dataArrivo = emergencyData.ArrivalDate + " " + emergencyData.ArrivalTime;

                }

            }


            return (output, problems);
        }


        private async Task<Protocollo> CreateProtocolEntObject(DocumentRowData rowData, string registrySyd, string rfSyd, string administrationSyd, bool isSmistamentoEnabled, InfoUtente userInfo, Ruolo role)
        {
            // Creazione dell'oggetto ProtocolloEntrata
            ProtocolloEntrata inProto = new ProtocolloEntrata();

            // Calcolo del mittente del protocollo
            // Se è valorizzata la proprietà CorrDesc della rowData, significa che
            // il corrispondente è di tipo occasionale
            if (rowData.CorrDesc != null && rowData.CorrDesc.Count > 0)
            {
                // Creazione del corrispondente
                inProto.mittente = new DocsPaVO.utente.Corrispondente();

                // Impostazione della descrizione del corrispondente
                inProto.mittente.descrizione = rowData.CorrDesc[0];

                // Impostazione dell'id amministrazione
                inProto.mittente.idAmministrazione = administrationSyd;

                // Impostazione del tipo corrispondente ad O
                inProto.mittente.tipoCorrispondente = "O";

            }

            if (rowData.CorrCode != null && rowData.CorrCode.Count > 0)
                // Altrimenti si procede con il caricamento delle informazioni sul
                // corrispondente
                inProto.mittente = await this.GetCorrispondenteByCode(
                    ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN,
                    rowData.CorrCode[0].Trim(),
                    role,
                    userInfo,
                    registrySyd,
                    rfSyd,
                    isSmistamentoEnabled,
                    TipoUtente.GLOBALE);

            // Se non è stato ptrovato il corrispondente, eccezione
            if (inProto.mittente == null)
                throw new Exception(Resources.CantFindSend);

            // Restituzione dell'oggetto con le informazioni sul protocollo
            return inProto;

        }
        private Protocollo CreateProtocolGreyObject(DocumentRowData rowData, string registrySyd, string rfSyd, string administrationSyd, bool isSmistamentoEnabled, InfoUtente userInfo, Ruolo role)
        {
            return null;
        }

        private async Task<Protocollo> CreateProtocolIntObject(DocumentRowData rowData, string registrySyd, string rfSyd, string administrationSyd, bool isSmistamentoEnabled, InfoUtente userInfo, Ruolo role)
        {
            ProtocolloInterno ownProto = new ProtocolloInterno();

            // Il corrispondente da inserire
            DocsPaVO.utente.Corrispondente corr;

            List<DocsPaVO.utente.Corrispondente> destinatari = new();
            List<DocsPaVO.utente.Corrispondente> destinatariConoscenza = new();
            string[] corrToAdd;


            foreach (string corrDesc in rowData.CorrCode)
            {
                // Spezzettamento dei dati sul corrispondente
                corrToAdd = corrDesc.Split('#');

                if (corrToAdd.Length != 3)
                    throw new Exception(string.Format(Resources.SpecCorrInv, corrDesc));

                switch (corrToAdd[1].ToUpper().Trim())
                {
                    case "M":       // Mittente del protocollo
                        corr = await this.GetCorrispondenteByCode(
                            ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_MITT,
                            corrToAdd[0].Trim(),
                            role,
                            userInfo,
                            registrySyd,
                            rfSyd,
                            isSmistamentoEnabled,
                            DocsPaVO.addressbook.TipoUtente.GLOBALE);

                        // Impostazione del mittente, se individuato
                        if (corr != null)
                            ownProto.mittente = corr;
                        break;

                    case "D":       // Destinatario principale
                        corr = await this.GetCorrispondenteByCode(
                            ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST,
                            corrToAdd[0].Trim(),
                            role,
                            userInfo,
                            registrySyd,
                            rfSyd,
                            isSmistamentoEnabled,
                            DocsPaVO.addressbook.TipoUtente.GLOBALE);

                        if (corr != null)
                            destinatari.Add(corr);
                        break;

                    case "CC":      // Destinatario in copia
                        corr = await this.GetCorrispondenteByCode(
                            ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST,
                            corrToAdd[0].Trim(),
                            role,
                            userInfo,
                            registrySyd,
                            rfSyd,
                            isSmistamentoEnabled,
                            DocsPaVO.addressbook.TipoUtente.GLOBALE);

                        if (corr != null)
                            destinatariConoscenza.Add(corr);
                        break;

                }

            }

            ownProto.destinatari = destinatari.ToArray();
            ownProto.destinatariConoscenza = destinatariConoscenza.ToArray();
            // Aggiornamento di destinatari, mittenti e destinatari in conoscenza
            ownProto.daAggiornareDestinatari = true;
            ownProto.daAggiornareMittente = true;
            ownProto.daAggiornareDestinatariConoscenza = true;

            // Restituzione dell'oggetto con le informazioni sul protocollo
            return ownProto;
        }

        private async Task<Protocollo> CreateProtocolUscObject(DocumentRowData rowData, string registrySyd, string rfSyd, string administrationSyd, bool isSmistamentoEnabled, InfoUtente userInfo, Ruolo role)
        {
            ProtocolloUscita outProto = new ProtocolloUscita();

            DocsPaVO.utente.Corrispondente corr;
            List<DocsPaVO.utente.Corrispondente> destinatari = new();
            List<DocsPaVO.utente.Corrispondente> destinatariConoscenza = new();

            string[] corrToAdd;

            // Creazione lista destinatari e destinatari conoscenza

            if (rowData.CorrDesc != null)
            {
                // Per ogni corrispondente in CorrDesc...
                foreach (string corrDesc in rowData.CorrDesc)
                {
                    corrToAdd = corrDesc.Split('#');

                    if (corrToAdd.Length != 3)
                        throw new Exception(string.Format(Resources.SpecCorrInv, corrDesc));

                    // Creazione del corrispondente
                    corr = new DocsPaVO.utente.Corrispondente();

                    // Impostazione della descrizione del corrispondente
                    corr.descrizione = corrToAdd[0];

                    // Impostazione dell'id amministrazione
                    corr.idAmministrazione = administrationSyd;

                    // Impostazione del tipo corrispondente ad O
                    corr.tipoCorrispondente = "O";

                    switch (corrToAdd[1].ToUpper().Trim())
                    {
                        case "D":       // Destinatario principale
                            destinatari.Add(corr);
                            break;

                        case "CC":      // Destinatario in copia
                            destinatariConoscenza.Add(corr);
                            break;

                    }

                }
            }

            if (rowData.CorrCode != null)
            {
                // Per ogni codice corrispondente in CorrCode...
                foreach (string corrDesc in rowData.CorrCode)
                {
                    // Spezzettamento dei dati sul corrispondente
                    corrToAdd = corrDesc.Split('#');

                    // Se non ci sono più tre elementi, eccezione
                    // Tre elementi poiché il formato con cui è scritto il codice è
                    // <Codice>#D|CC#
                    if (corrToAdd.Length != 3)
                        throw new Exception(string.Format(Resources.SpecCorrInv, corrDesc));

                    // A seconda del tipo di corrispondente bisogna intraprendere
                    // azioni differenti
                    switch (corrToAdd[1].ToUpper().Trim())
                    {
                        case "M":       // Mittente del protocollo
                            // Reperimento del corrispondente
                            corr = await this.GetCorrispondenteByCode(
                                ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_MITT,
                                corrToAdd[0].Trim(),
                                role,
                                userInfo,
                                registrySyd,
                                rfSyd,
                                isSmistamentoEnabled,
                                DocsPaVO.addressbook.TipoUtente.GLOBALE);

                            // Impostazione del mittente, se individuato
                            if (corr != null)
                                outProto.mittente = corr;
                            break;

                        case "D":       // Destinatario principale
                            // Reperimento del corrispondente
                            corr = await this.GetCorrispondenteByCode(
                                ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT,
                                corrToAdd[0].Trim(),
                                role,
                                userInfo,
                                registrySyd,
                                rfSyd,
                                isSmistamentoEnabled,
                                DocsPaVO.addressbook.TipoUtente.GLOBALE);

                            // Aggiunta del corrispondente alla lista dei destinatari, se individuato
                            if (corr != null)
                                destinatari.Add(corr);
                            break;

                        case "CC":      // Destinatario in copia
                            // Reperimento del corrispondente
                            corr = await this.GetCorrispondenteByCode(
                                ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT,
                                corrToAdd[0].Trim(),
                                role,
                                userInfo,
                                registrySyd,
                                rfSyd,
                                isSmistamentoEnabled,
                                DocsPaVO.addressbook.TipoUtente.GLOBALE);

                            // Aggiunta del corrispondente alla lista dei destinatari, se individuato
                            if (corr != null)
                                destinatariConoscenza.Add(corr);
                            break;

                    }

                }

            }

            // Se non è stato impostato il mittente, si considera come tale
            // l'uo di appartenenza dell'utente che ha lanciato la procedura
            if (outProto.mittente == null)
                outProto.mittente = role.uo;

            // Aggiornamento di destinatari, mittenti e destinatari in conoscenza
            outProto.daAggiornareDestinatari = true;
            outProto.daAggiornareMittente = true;
            outProto.daAggiornareDestinatariConoscenza = true;


            outProto.destinatari = destinatari.ToArray();
            outProto.destinatariConoscenza = destinatariConoscenza.ToArray();
            // Restituzione dell'oggetto con le informazioni sul protocollo
            return outProto;
        }


        private async Task<List<string>> CompileDocumentProfilationFields(DocumentRowData rowData, DocsPaVO.documento.SchedaDocumento documentScheda, Ruolo role, InfoUtente userInfo, string administrationId, bool isSmistamentoEnabled)
        {
            var template = await (from a in this._dbContext.TipoAttoEntities.AsNoTracking()
                                  where a.ID_AMM == administrationId.AsLong() &&
                                  a.ABILITATO_SI_NO == 1 && a.VAR_DESC_ATTO != null && a.VAR_DESC_ATTO.ToUpper().Equals(rowData.DocumentTipology.ToUpper())
                                  select a.SYSTEM_ID).FirstOrDefaultAsync();

            if (template == null)
            {
                throw new Exception(string.Format(Resources.TemplateNotFound, rowData.DocumentTipology));
            }

            var temp = (await this._mediator.Send(new Application.Requests.getTemplateById(template.ToString()))).output;

            var visibilityRights = (await this._mediator.Send(new Application.Requests.getDirittiCampiTipologiaDoc(role.idGruppo, temp.SYSTEM_ID.ToString()))).output;

            // Se tutto è andato bene, si può procedere alla compilazione dei campi
            // profilati
            (documentScheda.template, List<string> toReturn) = await this.CompileProfilationObjects(
                    rowData,
                    role,
                    userInfo,
                    temp,
                    visibilityRights,
                    administrationId,
                    isSmistamentoEnabled);

            if (documentScheda.template != null)
            {
                documentScheda.tipologiaAtto = new TipologiaAtto();
                documentScheda.tipologiaAtto.descrizione = documentScheda.template.DESCRIZIONE;
                documentScheda.tipologiaAtto.systemId = documentScheda.template.SYSTEM_ID.ToString();
                documentScheda.daAggiornareTipoAtto = true;

                if (documentScheda.template.PRIVATO == "1")
                    documentScheda.privato = "1";
            }

            // Restituzione degli eventuali problemi
            return toReturn;

        }



        private async Task<(Templates, List<string>)> CompileProfilationObjects(
            DocumentRowData rowData,
            Ruolo role,
            InfoUtente userInfo,
            Templates template,
            AssDocFascRuoli[] visibilityRights,
            string adminID,
            bool isSmistamentoEnabled
            )
        {
            List<string> problems = new();
            // La lista dei valori associati ad una determinata
            // etichetta
            string[] fieldValues = null;

            // I diritti associati ad un determinato campo
            AssDocFascRuoli rights = null;


            foreach (OggettoCustom obj in template.ELENCO_OGGETTI)
            {
                rights = visibilityRights.Where(
                    e => e.ID_OGGETTO_CUSTOM == obj.SYSTEM_ID.ToString()).FirstOrDefault();

                fieldValues = rowData.GetDocumentProfilationField(obj.DESCRIZIONE);

                // Se fieldValues è valorizzato...
                if (fieldValues != null)
                {
                    // ...compilazione del campo profilato
                    problems.AddRange(await this.CompileProfilationField(
                        obj,
                        rights,
                        fieldValues,
                        role,
                        userInfo,
                        rowData.RFCode != null ? rowData.RFCode : string.Empty,
                        adminID,
                        rowData.RegCode != null ? rowData.RegCode : string.Empty,
                        isSmistamentoEnabled));

                }
                else
                    // Altrimenti se l'utente ha diritti di modifica sul campo e il campo è
                    // obbligatorio, viene lanciata un'eccezione
                    if (rights != null && rights.INS_MOD_OGG_CUSTOM == "1" &&
                        obj.CAMPO_OBBLIGATORIO == "SI")
                    throw new Exception(string.Format(Resources.CampoNonVal, obj.DESCRIZIONE));

            }
            return (template, problems);
        }


        private async Task<List<string>> CompileProfilationField(OggettoCustom customObject, AssDocFascRuoli rights, string[] fieldValues, Ruolo role, InfoUtente userInfo, string RFCode, string administrationId, string registryCode, bool isEnabledSmistamento)
        {
            #region Dichiarazione variabili

            Registro registry;

            string rfSyd = string.Empty;

            string registrySyd = string.Empty;

            TipoUtente userType;

            List<string> toReturn;

            ValoreOggetto objectValue;

            #endregion

            toReturn = new List<string>();

            switch (customObject.TIPO.DESCRIZIONE_TIPO.ToUpper())
            {
                case "CASELLADISELEZIONE":
                    // Nel caso della casella di selezione è possibile che sia selezionato
                    // più di un valore
                    // Se il ruolo può modificare il campo...
                    if (rights.INS_MOD_OGG_CUSTOM == "1")
                        // ...vengono impostati i valori
                        // Per ogni stringa contenuta all'interno dei valori selezionati,
                        // bisogna ricercare l'oggetto con le informazioni sull'opzione da selezionare
                        // ricavandone la posizione ed inserendo tale descrizione nella stessa posizione
                        // ma nell'array VALORI_SELEZIONATI
                        foreach (string selectedValue in fieldValues)
                        {
                            objectValue = customObject.ELENCO_VALORI.Where(e => e.VALORE.ToUpper().Equals(selectedValue.ToUpper())).FirstOrDefault();

                            customObject.VALORI_SELEZIONATI[Array.IndexOf(customObject.ELENCO_VALORI, objectValue)] = selectedValue;
                        }
                    else
                        // ...altrimenti non si può impostare il valore. Si procede quindi
                        // all'aggiunta di un messaggio di avviso alla lista dei "warnings"
                        toReturn.Add(string.Format(Resources.ModifyRightTemplate, customObject.DESCRIZIONE));
                    break;

                case "CORRISPONDENTE":
                    // Se il ruolo possiede i diritti di modifica sul campo...
                    if (rights.INS_MOD_OGG_CUSTOM == "1")
                    {
                        var corr = await this.FindCorrispondente(fieldValues[0], customObject, userInfo, role, RFCode, registryCode, administrationId, isEnabledSmistamento);
                        customObject.VALORE_DATABASE = corr.systemId;
                    }
                    else
                        // Altrimenti si aggiunge un messaggio alla lista dei warnings
                        toReturn.Add(string.Format(Resources.CorrHasNoRights, customObject.DESCRIZIONE));
                    break;

                case "CONTATORE":
                case "CONTATORESOTTOCONTATORE":
                    try
                    {
                        // Reperimento dei dati sul registro
                        registry = await this.GetRegistroByCodAoo(fieldValues[0].ToUpper(), administrationId);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(string.Format(Resources.RegNotFound, fieldValues[0]));
                    }

                    switch (customObject.TIPO_CONTATORE.ToUpper())
                    {
                        case "A":   // Contatore di AOO
                            customObject.ID_AOO_RF = registry.systemId;
                            break;
                        case "R":   // Contatore di RF
                            // Se il registro non è un registro di RF, eccezione
                            if (!(registry.chaRF == "1"))
                                throw new Exception(string.Format(Resources.RegCodeNotRf, fieldValues[0]));

                            // Impostazione dell'id registro
                            customObject.ID_AOO_RF = registry.systemId;

                            break;
                    }

                    // Se il contatore è abilitato allo scatto differito...
                    if (customObject.CONTA_DOPO == "0")
                        // Se il ruolo ha diritti di modifica sul contatore...
                        if (rights.INS_MOD_OGG_CUSTOM == "1")
                            // Il contatore deve scattare
                            customObject.CONTATORE_DA_FAR_SCATTARE = true;

                    break;
                case "LINK":
                    if (fieldValues.Length < 2) throw new Exception(Resources.LinkError);
                    if ("INTERNO".Equals(customObject.TIPO_LINK))
                    {
                        if ("DOCUMENTO".Equals(customObject.TIPO_OBJ_LINK))
                        {
                            InfoDocumento infoDoc = null;
                            try
                            {
                                infoDoc = (await this._mediator.Send(new Application.Requests.GetInfoDocumento(userInfo, fieldValues[1], null))).output;
                            }
                            catch (Exception e)
                            {
                                throw new Exception(string.Format(Resources.DocNotFoundWithId, fieldValues[1]));
                            }
                            if (infoDoc == null) throw new Exception(string.Format(Resources.DocNotFoundWithId, fieldValues[1]));
                            string errorMessage = "";
                            var res = (await this._mediator.Send(new Application.Requests.VerificaACL("D", infoDoc.idProfile, userInfo)));
                            int result = res.output;
                            errorMessage = res.errorMessage;
                            if (result != 2)
                            {
                                throw new Exception(string.Format(Resources.DocNotFoundWithId, fieldValues[1]));
                            }
                        }
                        else
                        {
                            DocsPaVO.fascicolazione.Fascicolo fasc = null;
                            try
                            {
                                fasc = (await this._mediator.Send(new Application.Requests.FascicolazioneGetFascicoloDaCodice(userInfo, fieldValues[1], null, false, true))).output;
                            }
                            catch (Exception e)
                            {
                                throw new Exception(string.Format(Resources.MessFascNotFound, fieldValues[1]));
                            }
                            if (fasc == null) throw new Exception(string.Format(Resources.MessFascNotFound, fieldValues[1]));
                            string errorMessage = "";
                            var res = (await this._mediator.Send(new Application.Requests.VerificaACL("F", fasc.systemID, userInfo)));
                            int result = res.output;
                            errorMessage = res.errorMessage;
                            if (result != 2)
                            {
                                throw new Exception(string.Format(Resources.FascNoRights, fieldValues[1]));
                            }
                        }
                    }
                    customObject.VALORE_DATABASE = fieldValues[0] + "||||" + fieldValues[1];
                    break;
                case "OGGETTOESTERNO":
                    if (fieldValues.Length < 2) throw new Exception(Resources.ExtObjectReqValues);
                    customObject.MANUAL_INSERT = true;
                    customObject.CODICE_DB = fieldValues[0];
                    customObject.VALORE_DATABASE = fieldValues[1];
                    break;
                default:
                    // In tutti gli altri casi il valore è uno solo
                    // Se il ruolo ha diritti di modofica, viene impostato
                    // il valore altrimenti viene inserito un messaggio nella
                    // lista dei warning
                    if (rights.INS_MOD_OGG_CUSTOM == "1")
                        customObject.VALORE_DATABASE = fieldValues[0];
                    else
                        // ...altrimenti non si può impostare il valore. Si procede quindi
                        // all'aggiunta di un messaggio di avviso alla lista dei "warnings"
                        toReturn.Add(string.Format(Resources.NoModRightsExtObj, customObject.DESCRIZIONE));
                    break;

            }

            // Restituzione della lista delle eventuali segnalazioni
            return toReturn;

        }


        private async Task<DocsPaVO.utente.Corrispondente> GetCorrispondenteByCode(
            ParametriRicercaRubrica.CallType callType,
            string corrCode,
            Ruolo role,
            InfoUtente userInfo,
            string registrySyd,
            string rfSyd,
            bool isEnabledSmistamento,
            TipoUtente userTypeForProject)
        {
            ParametriRicercaRubrica searchParameters;

            // L'oggetto per memorizzare le impostazioni sullo smistamento
            SmistamentoRubrica smistamentoRubrica;

            // Il corrispondente da restituire
            DocsPaVO.utente.Corrispondente output = null;

            searchParameters = new ParametriRicercaRubrica();

            // Impostazione del call type
            searchParameters.calltype = callType;

            // Impostazione del codice da ricercare
            searchParameters.codice = corrCode;

            // Impostazione del flag per la ricerca del codice esatta
            searchParameters.queryCodiceEsatta = true;

            // Creazione del caller
            searchParameters.caller = new ParametriRicercaRubrica.CallerIdentity();

            // Impostazione del calltype
            searchParameters.caller.IdRuolo = role.systemId;

            // Impostazione dell'id utente
            searchParameters.caller.IdUtente = userInfo.idPeople;

            // Impostazione dell'id registro
            searchParameters.caller.IdRegistro = registrySyd;

            // Impostazione del filtro registro per la ricerca
            searchParameters.caller.filtroRegistroPerRicerca = registrySyd;

            // La ricerca va effettuata su Uffici, Utenti, Ruoli, RF
            searchParameters.doUo = true;
            searchParameters.doUtenti = true;
            searchParameters.doRuoli = true;
            searchParameters.doRF = true;

            bool confAb = (await this._mediator.Send(new Application.Requests.GetConfigurazioniRubricaComune(userInfo))).output.GestioneAbilitata;
            searchParameters.doRubricaComune = confAb;

            smistamentoRubrica = new SmistamentoRubrica();

            // Abilitazione smistamento
            smistamentoRubrica.smistamento = isEnabledSmistamento ? "1" : "0";

            // Impostazione calltype
            smistamentoRubrica.calltype = callType;

            // Impostazione informazioni sull'utente
            smistamentoRubrica.infoUt = userInfo;

            // Impostazione ruolo
            smistamentoRubrica.ruoloProt = role;

            // Impostazione dell'id del registro
            smistamentoRubrica.idRegistro = registrySyd;

            switch (callType)
            {
                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_IN:
                    if (!string.IsNullOrEmpty(rfSyd))
                        searchParameters.caller.filtroRegistroPerRicerca += ", " + rfSyd;
                    searchParameters.doRubricaComune = confAb;
                    searchParameters.doRubricaComune = true;
                    smistamentoRubrica.daFiltrareSmistamento = "0";
                    searchParameters.tipoIE = TipoUtente.GLOBALE;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT_MITT:
                    smistamentoRubrica.daFiltrareSmistamento = "0";
                    searchParameters.tipoIE = TipoUtente.INTERNO;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_OUT:
                    searchParameters.doListe = true;
                    searchParameters.doRubricaComune = confAb;
                    if (!string.IsNullOrEmpty(rfSyd))
                        searchParameters.caller.filtroRegistroPerRicerca += ", " + rfSyd;

                    smistamentoRubrica.daFiltrareSmistamento = "1";
                    searchParameters.tipoIE = TipoUtente.GLOBALE;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_MITT:
                    smistamentoRubrica.daFiltrareSmistamento = "0";
                    searchParameters.tipoIE = TipoUtente.INTERNO;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_PROTO_INT_DEST:
                    searchParameters.doListe = true;
                    smistamentoRubrica.daFiltrareSmistamento = "1";
                    searchParameters.tipoIE = TipoUtente.INTERNO;

                    break;

                case ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST:
                    if (!string.IsNullOrEmpty(rfSyd))
                        searchParameters.caller.filtroRegistroPerRicerca += ", " + rfSyd;
                    searchParameters.doRubricaComune = true;
                    searchParameters.tipoIE = userTypeForProject;
                    smistamentoRubrica.daFiltrareSmistamento = "0";

                    break;

            }

            var searchRes = await this._mediator.Send(new Application.Requests.rubricaGetElementiRubrica(searchParameters, userInfo, smistamentoRubrica));

            if (searchRes.output.Count() == 0)
            {
                throw new NoCorrsFoundFoundPi3Exception(string.Format(Resources.NoCorrFound2, corrCode));
            }

            if (searchRes.output.Count() > 1)
            {
                throw new MultipleCorrsFoundPi3Exception(string.Format(Resources.MultipleCorrFoundByCode, corrCode, searchRes.output.Count()));
            }

            ElementoRubrica temp = searchRes.output.FirstOrDefault();

            if (temp.isRubricaComune)
            {
                output = await this.UpdateCorrispondente(userInfo, temp.codice);
            }
            else
            {
                output = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteCompletoBySystemId(temp.systemId, DocsPaVO.addressbook.TipoUtente.GLOBALE, userInfo))).output;
            }
            return output;
        }


        private async Task<DocsPaVO.utente.Corrispondente> FindCorrispondente(
            string codice,
            OggettoCustom customObject,
            InfoUtente userInfo,
            Ruolo role,
            string RFCode,
            string registryCode,
            string administrationId,
            bool isEnabledSmistamento)
        {

            string rfSysId = string.Empty;
            string regSysId = string.Empty;

            if (!string.IsNullOrEmpty(RFCode))
            {
                try
                {
                    rfSysId = await this.GetRfId(RFCode, administrationId);
                }
                catch (Exception ex)
                {
                    // register is not rf
                    throw new Exception(string.Format(Resources.RegCodeNotRf, RFCode));
                }
            }


            regSysId = await this.GetIdRegistro(administrationId, registryCode);

            if (string.IsNullOrEmpty(regSysId))
            {
                throw new Exception(string.Format(Resources.RegNotFound, registryCode));
            }

            if (string.IsNullOrEmpty(codice))
            {
                throw new Exception(Resources.InvalidCode);
            }
            DocsPaVO.addressbook.TipoUtente userType = TipoUtente.GLOBALE;


            switch (customObject.TIPO_RICERCA_CORR.ToUpper())
            {
                case "INTERNI":
                    userType = TipoUtente.INTERNO;
                    break;

                case "ESTERNI":
                    userType = TipoUtente.ESTERNO;
                    break;

                default:
                    userType = TipoUtente.GLOBALE;
                    break;
            }


            bool confAb = (await this._mediator.Send(new Application.Requests.GetConfigurazioniRubricaComune(userInfo))).output.GestioneAbilitata;


            var searchParameters = new ParametriRicercaRubrica();
            searchParameters.calltype = ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST;
            searchParameters.codice = codice;
            searchParameters.queryCodiceEsatta = true;
            searchParameters.caller = new ParametriRicercaRubrica.CallerIdentity();
            searchParameters.caller.IdRuolo = role.systemId;
            searchParameters.caller.IdUtente = userInfo.idPeople;
            searchParameters.caller.IdRegistro = regSysId;
            searchParameters.caller.filtroRegistroPerRicerca = regSysId;
            searchParameters.doUo = true;
            searchParameters.doUtenti = true;
            searchParameters.doRuoli = true;
            searchParameters.doRF = true;
            searchParameters.doRubricaComune = confAb;

            var smistamentoRubrica = new SmistamentoRubrica();
            smistamentoRubrica.smistamento = isEnabledSmistamento ? "1" : "0";
            smistamentoRubrica.calltype = ParametriRicercaRubrica.CallType.CALLTYPE_CORR_EST;
            smistamentoRubrica.infoUt = userInfo;
            smistamentoRubrica.ruoloProt = role;
            smistamentoRubrica.idRegistro = regSysId;


            if (!string.IsNullOrEmpty(rfSysId))
                searchParameters.caller.filtroRegistroPerRicerca += ", " + rfSysId;
            searchParameters.doRubricaComune = true;
            searchParameters.tipoIE = userType;
            smistamentoRubrica.daFiltrareSmistamento = "0";


            var searchRes = await this._mediator.Send(new Application.Requests.rubricaGetElementiRubrica(searchParameters, userInfo, smistamentoRubrica));

            if (searchRes.output.Count() == 0)
            {
                throw new Exception(string.Format(Resources.NoCorrFound2, codice));
            }

            if (searchRes.output.Count() > 1)
            {
                throw new Exception(string.Format(Resources.MultipleCorrFoundByCode, codice, searchRes.output.Count()));
            }

            ElementoRubrica temp = searchRes.output.FirstOrDefault();
            DocsPaVO.utente.Corrispondente output = new();

            if (temp.isRubricaComune)
            {
                output = await this.UpdateCorrispondente(userInfo, temp.codice);
            }
            else
            {
                output = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteCompletoBySystemId(temp.systemId, DocsPaVO.addressbook.TipoUtente.GLOBALE, userInfo))).output;
            }
            return output;

        }


        private async Task<DocsPaVO.utente.Corrispondente> UpdateCorrispondente(DocsPaVO.utente.InfoUtente infoUtente, string codiceRubrica)
        {
            bool confAb = (await this._mediator.Send(new Application.Requests.GetConfigurazioniRubricaComune(infoUtente))).output.GestioneAbilitata;
            string idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            if (confAb)
            {
                bool rubEstAttive = false;

                (string value, bool keyFound) = await this._configurationService.TryGetValue<string>(idTenant, "BE_ENABLE_RUBRICHE_ESTERNE");

                if (keyFound && !string.IsNullOrEmpty(value) && value.Equals("1"))
                {
                    rubEstAttive = true;
                }

                (string beKey, bool beRubFound) = await this._configurationService.TryGetValue<string>("BE_RUBRICHE_ESTERNE");


                if (rubEstAttive && beRubFound && !string.IsNullOrEmpty(beKey) && !beKey.Equals("0"))
                {
                    var elemento = await this.GetElementiInRubricaCom(codiceRubrica, beKey);

                    if (elemento != null)
                        return await this.GetDettAndUpdateCorr(infoUtente, elemento);
                    else
                        return null;
                }

            }
            return null;

        }


        private async Task<DocsPaVO.utente.Corrispondente> GetDettAndUpdateCorr(DocsPaVO.utente.InfoUtente infoUtente, Services.RubricaComune.Corrispondente elemento)
        {
            DocsPaVO.utente.Corrispondente corr = null;

            (bool inRubCom, string sysId) = await this.CorrInRubCom(elemento.Codice, string.Empty);

            if (inRubCom)
            {
                corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteCompletoBySystemId(sysId, TipoUtente.ESTERNO, infoUtente))).output;
                if (corr != null)
                {
                    corr.inRubricaComune = true;
                    corr.info = await this.GetDettagliCorr(corr.systemId);
                    corr.dettagli = true;
                }
            }

            (DocsPaVO.utente.Corrispondente corrispondente, bool isCorrModified) = await this.Update(infoUtente, corr, elemento);

            if (corrispondente != null && (!string.IsNullOrEmpty(corrispondente.systemId)))
            {
                var elementoEmails = await this._rubricaComuneService.GetEmails(this.GetAuthToken(), Convert.ToInt32(elemento.Id));
                var elementoEmail = elementoEmails.FirstOrDefault(e => e.Preferita == true);
                var elUrls = new List<string?>();
                if (elemento.UrlApiInteroperabilita != null)
                {
                    elUrls.Add(elemento.UrlApiInteroperabilita);
                }

                if (elementoEmails.Count > 0)
                {
                    List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();
                    foreach (var mail in elementoEmails)
                    {
                        listCaselle.Add(new MailCorrispondente
                        {
                            Email = mail.Indirizzo,
                            Note = mail.Note,
                            Principale = (mail.Preferita == true) ? "1" : "0"
                        });
                    }
                    var re = await this._mediator.Send(new Application.Requests.InsertMailCorrispondenteEsterno(listCaselle, corr.systemId));
                }
            }
            return corrispondente;
        }


        private async Task<(DocsPaVO.utente.Corrispondente, bool)> Update(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Corrispondente corrispondente, Services.RubricaComune.Corrispondente elemento)
        {
            bool isCorrModified = false;
            bool requestNew = (corrispondente == null);
            bool idDirty = false;
            string codFisc = "";
            string pIva = "";
            bool variazioneEmail = false;

            var elementoEmails = await this._rubricaComuneService.GetEmails(this.GetAuthToken(), Convert.ToInt32(elemento.Id));
            var elementoEmail = elementoEmails.FirstOrDefault(e => e.Preferita == true);
            var elUrls = new List<string?>();
            if (elemento.UrlApiInteroperabilita != null)
            {
                elUrls.Add(elemento.UrlApiInteroperabilita);
            }

            if (!requestNew)
            {

                if (elementoEmail == null)
                    elementoEmail = new()
                    {
                        Indirizzo = string.Empty
                    };

                if (corrispondente.email == null)
                    corrispondente.email = string.Empty;


                if (corrispondente.dettagli)
                {
                    if (corrispondente.info == null)
                    {
                        corrispondente.info = await this.GetDettagliCorr(corrispondente.systemId);
                        codFisc = ((DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)(corrispondente.info.Tables[0].Rows[0])).codiceFiscale;
                        pIva = ((DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)(corrispondente.info.Tables[0].Rows[0])).partitaIva;
                    }
                }


                idDirty = (corrispondente.descrizione != elemento.Denominazione ||
                            corrispondente.codiceAmm != elemento.Amministrazione ||
                            (!(string.IsNullOrEmpty(corrispondente.codiceAOO) && string.IsNullOrEmpty(elemento.AOO)) && corrispondente.codiceAOO != elemento.AOO) ||
                            corrispondente.email != elementoEmail.Indirizzo ||
                            codFisc.ToUpper() != elemento.CodiceFiscale?.ToUpper() ||
                            pIva.ToUpper() != elemento.PartitaIva?.ToUpper());



                idDirty = corrispondente.Url.Count != elUrls.Count;
                if (!idDirty && corrispondente.Url.Count > 0 && elUrls.Count > 0)
                    idDirty = corrispondente.Url[0].Url != elemento.UrlApiInteroperabilita;




                if (!idDirty)
                {

                    if (elementoEmails != null && elementoEmails.Count > 0) //verifica se da RC torna che il corr ha almeno una mail, altrimenti tutto il controllo sotto non ha senso
                    {
                        if (elementoEmails.Count > 1)
                            idDirty = elementoEmails.Count != corrispondente.Emails.Count;
                        else
                            idDirty = elementoEmail.Indirizzo.ToLower() != corrispondente.email.ToLower();

                        if (elementoEmails.Count != corrispondente.Emails.Count)
                            variazioneEmail = true;
                    }

                    if (!idDirty)
                    {
                        foreach (var mail in elementoEmails)
                        {
                            idDirty &= !corrispondente.Emails.Contains(new MailCorrispondente() { Email = mail.Indirizzo });
                        }
                    }
                }
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

                (string isEnabledSimplifiedInterop, bool keyFound) = await this._configurationService.TryGetValue<string>(idTenant, "INTEROP_SERVICE_ACTIVE");

                if (string.IsNullOrEmpty(isEnabledSimplifiedInterop))
                {
                    (isEnabledSimplifiedInterop, keyFound) = await this._configurationService.TryGetValue<string>("INTEROP_SERVICE_ACTIVE");
                }
                bool IsEnabledSimplifiedInteroperability = isEnabledSimplifiedInterop != null ? isEnabledSimplifiedInterop.Equals("1") : false;


                if (infoUtente != null && ((corrispondente.canalePref != null && (corrispondente.canalePref.typeId == Resources.InteroperabilityCode ||
                        corrispondente.canalePref.tipoCanale == Resources.InteroperabilityCode))
                        && !IsEnabledSimplifiedInteroperability) ||
                        (((corrispondente.canalePref != null && corrispondente.canalePref.typeId != Resources.InteroperabilityCode &
                        corrispondente.canalePref.tipoCanale != Resources.InteroperabilityCode)) &&
                        corrispondente.Url.Count > 0 && Uri.IsWellFormedUriString(corrispondente.Url[0].Url, UriKind.Absolute)))
                    idDirty = true;

                if (!idDirty)
                {
                    DocsPaVO.addressbook.DettagliCorrispondente oldDettagli = (DocsPaVO.addressbook.DettagliCorrispondente)corrispondente.info;

                    if (oldDettagli == null)
                    {
                        oldDettagli = await this.GetDettagliCorr(corrispondente.systemId);
                    }
                    if (oldDettagli.Corrispondente.Rows.Count > 0)
                    {
                        DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow oldRow = (DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)oldDettagli.Corrispondente.Rows[0];
                        idDirty = (oldRow.indirizzo != elemento.Indirizzo ||
                                        oldRow.citta != elemento.Citta ||
                                        oldRow.cap != elemento.CAP ||
                                        oldRow.provincia != elemento.Provincia ||
                                        oldRow.nazione != elemento.Nazione ||
                                        oldRow.telefono != elemento.Telefono ||
                                        oldRow.fax != elemento.Fax ||
                                        oldRow.codiceFiscale != elemento.CodiceFiscale ||
                                        oldRow.partitaIva != elemento.PartitaIva);
                    }
                }
            }
            if (requestNew)
            {
                DocsPaVO.utente.Corrispondente newCorr = await this.GetNuovoCorr(infoUtente, elemento, elementoEmail.Indirizzo, elUrls);
                corrispondente = (await this._mediator.Send(new Application.Requests.AddressbookInsertCorrispondente(newCorr, null, infoUtente))).output;
                if (corrispondente != null && !string.IsNullOrEmpty(corrispondente.errore))
                {
                    if (!newCorr.inRubricaComune)
                        throw new ApplicationException(corrispondente.errore);
                    else
                    {
                        newCorr.errore = corrispondente.errore;
                        isCorrModified = true;
                        return (corrispondente, isCorrModified);
                    }
                }
            }
            else if (idDirty)
            {
                corrispondente.descrizione = elemento.Denominazione;
                corrispondente.codiceAmm = elemento.Amministrazione;
                corrispondente.codiceAOO = elemento.AOO;
                corrispondente.email = elementoEmail.Indirizzo;
                corrispondente.Url = GetInternalUrlsCollection(elUrls);
                corrispondente.canalePref = await GetCanaleCorrispondente(corrispondente, infoUtente.idAmministrazione);
                DocsPaVO.utente.DatiModificaCorr datiModifica = this.GetDatiPerModifica(corrispondente.systemId, corrispondente.canalePref.systemId, elemento, elementoEmail.Indirizzo, elUrls);

                string newIdCorrGlobali;
                string message;

                var res = (await this._mediator.Send(new Application.Requests.CorrispondentiDeleteModifyCorrispondenteEsternoWithId(infoUtente, datiModifica, 0, "M")));

                if (res.output)
                {
                    var newCorrispondente = res.newIdCorr;
                    if (!string.IsNullOrEmpty(newCorrispondente) && (!newCorrispondente.Equals("0")))
                    {
                        corrispondente.idOld = corrispondente.systemId;
                        corrispondente.systemId = newCorrispondente;
                    }

                    DocsPaVO.addressbook.DettagliCorrispondente dettagli = new DocsPaVO.addressbook.DettagliCorrispondente();
                    dettagli.Corrispondente.AddCorrispondenteRow(elemento.Indirizzo, elemento.Citta, elemento.CAP, elemento.Provincia,
                                                                 elemento.Nazione, elemento.Telefono, string.Empty, elemento.Fax, elemento.CodiceFiscale,
                                                                 string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, elemento.PartitaIva);
                    corrispondente.info = dettagli;
                }
                else
                {
                    throw new Exception(Resources.ModifyCorrFailed);
                }

                if (variazioneEmail)
                {
                    List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();

                    foreach (var mail in elementoEmails)
                    {
                        listCaselle.Add(new MailCorrispondente
                        {
                            Email = mail.Indirizzo,
                            Note = mail.Note,
                            Principale = mail.Preferita == true ? "1" : "0"
                        });
                    }

                    var re = await this._mediator.Send(new Application.Requests.InsertMailCorrispondenteEsterno(listCaselle, corrispondente.systemId));
                    if (!re.output)
                    {
                        throw new Exception(Resources.ErrorModifyMail);
                    }

                }

            }
            isCorrModified = idDirty;
            return (corrispondente, isCorrModified);
        }


        private DocsPaVO.utente.DatiModificaCorr GetDatiPerModifica(string idOld, string idCanalePref, Services.RubricaComune.Corrispondente elemento, string email, List<string> urls)
        {
            DocsPaVO.utente.DatiModificaCorr datiModifica = new DocsPaVO.utente.DatiModificaCorr();

            datiModifica.idCorrGlobali = idOld;
            datiModifica.idCanalePref = idCanalePref;
            datiModifica.codice = elemento.Codice;
            datiModifica.codRubrica = elemento.Codice;
            datiModifica.descCorr = elemento.Denominazione;
            datiModifica.codiceAmm = elemento.Amministrazione;
            datiModifica.codiceAoo = elemento.AOO;
            datiModifica.indirizzo = elemento.Indirizzo;
            datiModifica.citta = elemento.Citta;
            datiModifica.cap = elemento.CAP;
            datiModifica.provincia = elemento.Provincia;
            datiModifica.nazione = elemento.Nazione;
            datiModifica.telefono = elemento.Telefono;
            datiModifica.telefono2 = string.Empty;
            datiModifica.email = email;
            datiModifica.fax = elemento.Fax;
            datiModifica.codFiscale = string.Empty;
            datiModifica.nome = string.Empty;
            datiModifica.inRubricaComune = true;
            datiModifica.Urls = GetInternalUrlsCollection(urls);
            datiModifica.codFiscale = elemento.CodiceFiscale;
            datiModifica.partitaIva = elemento.PartitaIva;
            if (elemento.Tipo.ToString().Equals("RaggruppamentoFunzionale"))
                datiModifica.tipoCorrispondente = "F";
            else
                if (elemento.Tipo.ToString().Equals("UnitaOrganizzativa"))
                datiModifica.tipoCorrispondente = "U";

            return datiModifica;
        }

        private async Task<DocsPaVO.utente.Corrispondente> GetNuovoCorr(InfoUtente infoUtente, Services.RubricaComune.Corrispondente elemento, string email, List<string> urls)
        {
            var corrispondente = this.InitializeSpecificAttributes(elemento.Tipo, elemento.Codice);
            corrispondente.inRubricaComune = true;
            corrispondente.codiceRubrica = elemento.Codice;
            corrispondente.descrizione = elemento.Denominazione;
            corrispondente.email = email;
            corrispondente.codiceAmm = elemento.Amministrazione;
            corrispondente.codiceAOO = elemento.AOO;
            corrispondente.tipoIE = "E";
            corrispondente.indirizzo = elemento.Indirizzo;
            corrispondente.Url = GetInternalUrlsCollection(urls);

            corrispondente.canalePref = await GetCanaleCorrispondente(corrispondente, infoUtente.idAmministrazione);
            DocsPaVO.addressbook.DettagliCorrispondente dettagliCorrispondente = new DocsPaVO.addressbook.DettagliCorrispondente();

            dettagliCorrispondente.Corrispondente.AddCorrispondenteRow
                (
                    elemento.Indirizzo, elemento.Citta, elemento.CAP, elemento.Provincia, elemento.Nazione,
                    elemento.Telefono, string.Empty, elemento.Fax, elemento.CodiceFiscale, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, elemento.PartitaIva
                );

            corrispondente.dettagli = true;
            corrispondente.info = dettagliCorrispondente;

            return corrispondente;
        }


        private List<DocsPaVO.utente.Corrispondente.UrlInfo> GetInternalUrlsCollection(List<string> urlInfo)
        {
            List<DocsPaVO.utente.Corrispondente.UrlInfo> retCollection = new();
            if (urlInfo != null)
                retCollection.AddRange(from url in urlInfo
                                       select new DocsPaVO.utente.Corrispondente.UrlInfo() { Url = url });

            return retCollection;

        }

        private async Task<Canale> GetCanaleCorrispondente(DocsPaVO.utente.Corrispondente corrispondente, string idAmministrazione)
        {
            bool canaleMail = false;
            bool canaleInterop = false;
            bool canaleInteropSemplificata = false;

            if (!string.IsNullOrEmpty(corrispondente.codiceAmm) && !string.IsNullOrEmpty(corrispondente.codiceAOO))
            {
                (string isEnabledSimplifiedInterop, bool keyFound) = await this._configurationService.TryGetValue<string>(idAmministrazione, "INTEROP_SERVICE_ACTIVE");

                if (string.IsNullOrEmpty(isEnabledSimplifiedInterop))
                {
                    (isEnabledSimplifiedInterop, keyFound) = await this._configurationService.TryGetValue<string>("INTEROP_SERVICE_ACTIVE");
                }
                canaleInteropSemplificata = isEnabledSimplifiedInterop != null ? isEnabledSimplifiedInterop.Equals("1") : false;

                canaleInterop = !string.IsNullOrEmpty(corrispondente.email) && !canaleInteropSemplificata;
            }
            else if (!string.IsNullOrEmpty(corrispondente.email))
            {

                canaleInterop = false;
                canaleInteropSemplificata = false;
                canaleMail = true;
            }

            foreach (var canale in await this.GetListaCanale())
            {
                if ((canale.typeId == Channel.Interop && canaleInterop && !canaleInteropSemplificata) ||
                    (canale.typeId == Channel.Mail && canaleMail) ||
                    (canale.typeId == Channel.Lettera && !canaleMail && !canaleInterop && !canaleInteropSemplificata) ||
                    (canale.typeId == Channel.Simpinterop && canaleInteropSemplificata))
                {
                    return canale;
                }
            }
            return null;
        }




        private async Task<List<Canale>> GetListaCanale()
        {
            List<Canale> canali = new();

            canali = await this._dbContext.DocumentTypesEntities.AsNoTracking().OrderBy(a => a.DESCRIPTION).Select(a => new Canale()
            {
                systemId = a.SYSTEM_ID.ToString(),
                typeId = a.TYPE_ID,
                descrizione = a.DESCRIPTION,
                tipoCanale = a.CHA_TIPO_CANALE
            }).ToListAsync();

            return canali;
        }




        private DocsPaVO.utente.Corrispondente InitializeSpecificAttributes(Tipi tipo, string codice)
        {
            DocsPaVO.utente.Corrispondente corrispondente = new();

            switch (tipo)
            {
                case Tipi.UnitaOrganizzativa:
                    corrispondente = new DocsPaVO.utente.UnitaOrganizzativa() { codice = codice, tipoCorrispondente = "U" };
                    break;
                case Tipi.RaggruppamentoFunzionale:
                    corrispondente = new DocsPaVO.utente.RaggruppamentoFunzionale() { Codice = codice, tipoCorrispondente = "F" };
                    break;
                default:
                    corrispondente = new DocsPaVO.utente.Corrispondente();
                    break;

            }

            return corrispondente;
        }





        private async Task<DocsPaVO.addressbook.DettagliCorrispondente> GetDettagliCorr(string sysId)
        {

            DocsPaVO.addressbook.DettagliCorrispondente dett = new();

            var dettCorr = await (from a in this._dbContext.DettGlobaliEntities.AsNoTracking()
                                  where a.ID_CORR_GLOBALI == sysId.AsLong()
                                  select a).FirstOrDefaultAsync();

            if (dettCorr != null)
            {
                dett.Corrispondente.AddCorrispondenteRow(
                    dettCorr.VAR_INDIRIZZO ?? string.Empty,
                    dettCorr.VAR_CITTA ?? string.Empty,
                    dettCorr.VAR_CAP ?? string.Empty,
                    dettCorr.VAR_PROVINCIA ?? string.Empty,
                    dettCorr.VAR_NAZIONE ?? string.Empty,
                    dettCorr.VAR_TELEFONO ?? string.Empty,
                    dettCorr.VAR_TELEFONO2 ?? string.Empty,
                    dettCorr.VAR_FAX ?? string.Empty,
                    dettCorr.VAR_COD_FISC ?? string.Empty,
                    dettCorr.VAR_NOTE ?? string.Empty,
                    dettCorr.VAR_LOCALITA ?? string.Empty,
                    dettCorr.VAR_LUOGO_NASCITA ?? string.Empty,
                    dettCorr.DTA_NASCITA != null ? dettCorr.DTA_NASCITA : string.Empty,
                    dettCorr.VAR_TITOLO ?? string.Empty,
                    dettCorr.VAR_COD_PI ?? string.Empty
                    );
            }
            else
            {
                dett.Corrispondente.AddCorrispondenteRow("", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            }
            return dett;
        }



        private async Task<(bool, string)> CorrInRubCom(string codiceRubrica, string idAmministrazione)
        {
            bool found = false;
            var corr = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                              where a.VAR_COD_RUBRICA != null &&
                              a.VAR_COD_RUBRICA.ToUpper().Equals(codiceRubrica.ToUpper()) &&
                              a.CHA_TIPO_CORR != null && a.CHA_TIPO_CORR.Equals("C")
                              select a.SYSTEM_ID).FirstOrDefaultAsync();
            if (corr != null)
                found = true;

            return (found, corr != null ? corr.ToString() : string.Empty);
        }



        private async Task<Services.RubricaComune.Corrispondente> GetElementiInRubricaCom(string codice, string rubEsterna)
        {
            var elementiRubrica = new List<Services.RubricaComune.Corrispondente>();
            Services.RubricaComune.Corrispondente elemento = new();
            var criteriRicerca = new List<CriterioRicerca>
            {
                new CriterioRicerca { Campo = CampiRicercaEnum.Codice, Valore = codice.Replace("'", "''") }
            };

            if (!string.IsNullOrEmpty(rubEsterna))
            {
                criteriRicerca.Add(
                    new()
                    {
                        Campo = CampiRicercaEnum.RubricaEsterna,
                        Valore = rubEsterna.ToUpper()
                    });

            }

            try
            {
                var response = await this._rubricaComuneService.Search(this.GetAuthToken(), new SearchRequest
                {
                    CriteriRicerca = criteriRicerca,
                    ElementiPerPagina = 50,
                    Pagina = 1
                });

                if (response.Corrispondenti.Any())
                {
                    response.Corrispondenti.ForEach(c => elementiRubrica.Add(c));
                    elemento = elementiRubrica.FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return elemento;


        }



        private async Task<Oggetto> LoadDocumentObject(InfoUtente userInfo,
            string objCode,
            string obj,
            string administrationSyd,
            string registrySyd)
        {
            QueryOggetto objectQuery = new();
            Oggetto output = new();

            if (!string.IsNullOrEmpty(objCode))
            {
                objectQuery.idAmministrazione = administrationSyd;
                objectQuery.idRegistri = new object[]
                    {
                        registrySyd
                    };
                objectQuery.queryCodice = objCode;

                var oggs = (await this._mediator.Send(new Application.Requests.DocumentoGetListaOggetti(objectQuery))).output;
                if (oggs == null || oggs.Count() != 1)
                    throw new Exception(string.Format(Resources.ErrorSearchingObjectCode, objCode));
                output = oggs[0] as Oggetto;
            }
            else
            {
                output = new Oggetto();
                output.idRegistro = registrySyd;
                output.descrizione = obj;
            }
            if (output == null)
                throw new Exception(string.Format(Resources.ErrorSearchingObjectCode, objCode));

            return output;
        }


        private (bool, string) HasAuth(string protoType, Ruolo role)
        {
            var functions = role.funzioni;
            bool canCreateDocument = true;
            string msgCanCreateDocument = string.Empty;

            switch (protoType)
            {
                case "A":
                    canCreateDocument = functions.Where(e => e.codice == "DO_NUOVOPROT").FirstOrDefault() != null && functions.Where(e => e.codice == "PROTO_IN").FirstOrDefault() != null;
                    msgCanCreateDocument = Resources.RoleNotAuthPA;
                    break;
                case "P":
                    canCreateDocument = functions.Where(e => e.codice == "DO_NUOVOPROT").FirstOrDefault() != null && functions.Where(e => e.codice == "PROTO_OUT").FirstOrDefault() != null;
                    msgCanCreateDocument = Resources.RoleNotAuthPP;
                    break;
                case "I":
                    canCreateDocument = functions.Where(e => e.codice == "DO_NUOVOPROT").FirstOrDefault() != null && functions.Where(e => e.codice == "PROTO_OWN").FirstOrDefault() != null;
                    msgCanCreateDocument = Resources.RoleNotAuthPI;
                    break;
            }

            return (canCreateDocument, msgCanCreateDocument);
        }

        private async Task<ImportResult> ImportAttachment(
            DocumentRowData documentRowData,
            InfoUtente userInfo,
            Ruolo role,
            string ftpAddress,
            ResultsContainer resultContainer,
            string ftpUsername,
            string ftpPassword
            )
        {
            #region variable definition and inst
            ImportResult importResult = null;
            ImportResult documentReport = null;
            bool validParameters = false;
            List<string> creationProblems = null;
            #endregion


            if (documentRowData == null)
            {
                importResult = new ImportResult()
                {
                    Outcome = ImportResult.OutcomeEnumeration.OK,
                    Message = Resources.NoDocToImport
                };
            }
            else
            {
                importResult = new();
                validParameters = true;
                creationProblems = new List<string>();

                (bool isDataValid, var problems) = this.IsAttachmentDataValid(documentRowData);
                importResult.OtherInformation.AddRange(problems);

                if (isDataValid)
                {
                    if (!string.IsNullOrEmpty(documentRowData.MainDocumentId))
                    {
                        documentReport = await this.CheckMainDocumentExistence(userInfo, documentRowData.MainDocumentId);
                        if (documentReport == null)
                        {
                            importResult.Outcome = ImportResult.OutcomeEnumeration.KO;
                            importResult.Message = Resources.CantAddAttachment;
                            importResult.OtherInformation.Add(string.Format(Resources.CantAddAttachment, documentRowData.MainDocumentId));
                        }
                        else
                        {

                            string identificationData = await this.CreateAttachment(documentRowData,
                                    userInfo,
                                    role,
                                    ftpAddress,
                                    documentReport,
                                    ftpUsername,
                                    ftpPassword);

                            importResult.Outcome = ImportResult.OutcomeEnumeration.OK;
                            importResult.Message = string.Format(Resources.AttachmentSuccessfullyAddded, identificationData);
                        }

                    }
                    else if (!string.IsNullOrEmpty(documentRowData.MainOrdinal))
                    {
                        documentReport = this.GetDocumentType(
                                documentRowData.MainOrdinal,
                                resultContainer);
                        // Se il documentReport è null -> tipologia non riconosciuta
                        if (documentReport == null)
                        {
                            importResult.Outcome = ImportResult.OutcomeEnumeration.KO;
                            importResult.Message = string.Format(Resources.InvalidMainOrdnal, documentRowData.MainOrdinal);
                        }
                        else
                        {

                            string identificationData = await this.CreateAttachment(documentRowData,
                                    userInfo,
                                    role,
                                    ftpAddress,
                                    documentReport,
                                    ftpUsername,
                                    ftpPassword);
                            importResult.Outcome = ImportResult.OutcomeEnumeration.OK;
                            importResult.Message = string.Format(Resources.AttachmentSuccessfullyAddded, identificationData);
                        }

                    }
                    importResult.Ordinal = documentRowData.OrdinalNumber;

                }
                else
                {
                    importResult = new ImportResult()
                    {
                        Outcome = ImportResult.OutcomeEnumeration.KO,
                        Message = Resources.InvalidParams,
                        OtherInformation = creationProblems,
                        Ordinal = documentRowData.OrdinalNumber
                    };
                }

            }
            return importResult;
        }

        private async Task<byte[]> DownloadFileFromUserTempFolder(DocsPaVO.PrjDocImport.DocumentRowData rowData,
            InfoUtente userInfo,
            bool isAttachment = false)
        {
            FileRequest fileRequest = null;
            FileDocumento fileDocumento = null;

            #region file content acquisition
            byte[] fileContent = null;
            #endregion
            var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);
            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);
            var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);

            string userPath = System.IO.Path.Combine(
                            repositoryRootPath,
                            tenantCode.ToUpper(),
                            "Import",
                            "Documents",
                            idUser).PathAsUnixPath();

            string userTempDedicatedFolder = System.IO.Path.Combine(userPath, DateTime.Now.ToString("yyyyMMdd")).PathAsUnixPath();

            if (isAttachment)
            {
                userTempDedicatedFolder = System.IO.Path.Combine(userTempDedicatedFolder, "Attachments").PathAsUnixPath();
            }
            string filePath = System.IO.Path.Combine(userTempDedicatedFolder, rowData.Pathname).PathAsUnixPath();

            if (!System.IO.File.Exists(filePath))
            {
                throw new Exception("File non accessibile");
            }

            fileContent = System.IO.File.ReadAllBytes(filePath);

            
            return fileContent;

        }
        private async Task AcquireFile(DocumentRowData rowData, InfoUtente userInfo, Ruolo role, bool isSmistamentoEnabled, DocsPaVO.documento.SchedaDocumento schedaDocumento, string ftpAddress, string ftpUsername, string ftpPassword)
        {
            byte[] fileContent;
            FileRequest fileRequest;
            FileDocumento fileDocumento;
            if (string.IsNullOrEmpty(rowData.Pathname)) return;

            try
            {
                if (schedaDocumento.documentoPrincipale == null)
                {
                    fileContent = await this.DownloadFileFromUserTempFolder(rowData, userInfo);
                }
                else
                {
                    fileContent = await this.DownloadFileFromUserTempFolder(rowData, userInfo, true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            fileDocumento = new FileDocumento();

            // Impostazione del nome del file
            fileDocumento.name = System.IO.Path.GetFileName(rowData.Pathname);

            // Impostazione del full name
            fileDocumento.fullName = rowData.Pathname;

            // Impostazione del path
            fileDocumento.path = System.IO.Path.GetPathRoot(rowData.Pathname);

            // Impostazione della grandezza del file
            fileDocumento.length = fileContent.Length;

            // Impostazione del content del documento
            fileDocumento.content = fileContent;

            fileRequest = (FileRequest)schedaDocumento.documenti[0];
            fileRequest.repositoryContext = null;

            await this._mediator.Send(new Application.Requests.DocumentoPutFile(fileRequest, fileDocumento, userInfo));
        }

        private async Task AcquireFile(string path, string administrationCode, InfoUtente userInfo, string ftpAddress, DocsPaVO.documento.Allegato attachment, string ftpUsername, string ftpPassword, DocumentRowData rowData)
        {
            byte[] fileContent;
            FileRequest fileRequest;
            FileDocumento fileDocumento;
            if (string.IsNullOrEmpty(rowData.Pathname)) return;

            try
            {
                fileContent = await this.DownloadFileFromUserTempFolder(rowData, userInfo, true);
            }
            catch (Exception ex)
            {
                return;
            }

            fileDocumento = new FileDocumento();

            // Impostazione del nome del file
            fileDocumento.name = System.IO.Path.GetFileName(path);

            // Impostazione del full name
            fileDocumento.fullName = path;

            // Impostazione del path
            fileDocumento.path = System.IO.Path.GetPathRoot(path);

            // Impostazione della grandezza del file
            fileDocumento.length = fileContent.Length;

            // Impostazione del content del documento
            fileDocumento.content = fileContent;

            fileRequest = (FileRequest)attachment;

            await this._mediator.Send(new Application.Requests.DocumentoPutFile(fileRequest, fileDocumento, userInfo));

        }




        private DocsPaVO.utente.InfoUtente GetInfoUtente(DocsPaVO.utente.Utente user, DocsPaVO.utente.Ruolo role)
        {
            DocsPaVO.utente.InfoUtente retVal = new DocsPaVO.utente.InfoUtente();

            retVal.idPeople = user.idPeople;
            retVal.dst = user.dst;
            retVal.idAmministrazione = user.idAmministrazione;
            retVal.userId = user.userId;
            retVal.sede = user.sede;
            if (user.urlWA != null)
                retVal.urlWA = user.urlWA;
            if (role != null)
            {
                retVal.idCorrGlobali = role.systemId;
                retVal.idGruppo = role.idGruppo;
            }

            return retVal;
        }



        #region members for attachment creation
        private async Task<string> CreateAttachment(
            DocumentRowData rowData,
            InfoUtente userInfo,
            Ruolo role,
            string ftpAddress,
            ImportResult importResults,
            string ftpUsername,
            string ftpPassword)
        {
            string identificationData = string.Empty;

            if (string.IsNullOrEmpty(importResults.DocNumber))
            {
                throw new Exception(Resources.FailedAttachmentCreation);
            }

            var attachment = this.CreateAttachmentObject(rowData.Obj, importResults.DocNumber);

            var attachRes = (await this._mediator.Send(new Application.Requests.DocumentoAggiungiAllegato(userInfo, attachment))).output;

            if (attachRes == null)
            {
                throw new Exception(Resources.FailedAddingAttachment);
            }

            if (!string.IsNullOrEmpty(rowData.Pathname))
            {
                await this.AcquireFile(
                    rowData.Pathname,
                    rowData.AdminCode,
                    userInfo,
                    ftpAddress,
                    attachment,
                    ftpUsername,
                    ftpPassword,
                    rowData);
            }
            identificationData = string.Format(Resources.IdentData, attachment.docNumber);


            return identificationData;
        }
        private DocsPaVO.documento.Allegato CreateAttachmentObject(string description, string docNumber)
        {

            DocsPaVO.documento.Allegato toReturn;
            toReturn = new();
            toReturn.descrizione = description;
            toReturn.docNumber = docNumber;
            return toReturn;

        }
        #endregion

        #region methods for the gray doc validation
        private (bool, List<string>) IsGrayDataValid(
            DocumentRowData rowData,
            bool isProfilationRequired,
            bool isRapidClassificationRequired)
        {
            List<string> validationProblems = new();
            bool dataIsValid = true;

            if (string.IsNullOrEmpty(rowData.OrdinalNumber))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.OrdinalFieldIsRequired);
            }
            if (string.IsNullOrEmpty(rowData.AdminCode))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.CodAmIsRequired);
            }
            if (string.IsNullOrEmpty(rowData.RegCode))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.CodRegRequired);
            }

            if (string.IsNullOrEmpty(rowData.ObjCode) &&
                string.IsNullOrEmpty(rowData.Obj))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.ObjAndObjcodeEmpty);
            }

            if (!string.IsNullOrEmpty(rowData.ObjCode) &&
                !string.IsNullOrEmpty(rowData.Obj))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.ObjAndObjcodeBothVal);
            }

            if (rowData.ProjectCodes != null && rowData.ProjectCodes.Length > 0 &&
                (!string.IsNullOrEmpty(rowData.ProjectDescription) ||
                 !string.IsNullOrEmpty(rowData.FolderDescrition) ||
                 !string.IsNullOrEmpty(rowData.NodeCode) ||
                 !string.IsNullOrEmpty(rowData.ProjectTipology)))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.CodFascAndProfFound);
            }

            if (isProfilationRequired && string.IsNullOrEmpty(rowData.DocumentTipology))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.TipNotFound);
            }

            if (isRapidClassificationRequired &&
                rowData.ProjectCodes == null &&
                string.IsNullOrEmpty(rowData.ProjectDescription) &&
                string.IsNullOrEmpty(rowData.FolderDescrition) &&
                string.IsNullOrEmpty(rowData.NodeCode) &&
                string.IsNullOrEmpty(rowData.ProjectTipology))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.ClassRequiredButDataNotFound);

            }

            return (dataIsValid, validationProblems);
        }

        #endregion

        #region methods for prot arr validation  
        private async Task<(bool, List<string>)> IsPrArrDocumentValid(
            DocumentRowData rowData,
            bool isProfilationRequired,
            bool isRapidClassificationRequired)
        {

            List<string> validationProblems = new();
            bool dataIsValid = true;
            bool rfIsRequired = false;

            if (string.IsNullOrEmpty(rowData.OrdinalNumber))
            {
                dataIsValid = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add(Resources.NumProtoReq);
                else
                    validationProblems.Add(Resources.OrdinalFieldIsRequired);
            }
            else
            {
                int ordinal = 0;
                if (!int.TryParse(rowData.OrdinalNumber, out ordinal))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.OrdinalFieldIsRequired);
                }

            }

            if (string.IsNullOrEmpty(rowData.AdminCode))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.CodAmIsRequired);
            }
            if (string.IsNullOrEmpty(rowData.RegCode))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.CodRegRequired);
            }

            string administrationSyd = (await this._mediator.Send(new Application.Requests.getIdAmmByCod(rowData.AdminCode))).output;

            if (administrationSyd == null)
            {
                dataIsValid = false;
                validationProblems.Add(string.Format(Resources.CantFetchDataAmm, rowData.AdminCode));
            }

            if (!string.IsNullOrEmpty(administrationSyd))
            {
                rfIsRequired = await this.AmmHasRf(administrationSyd);

                if (rfIsRequired && string.IsNullOrEmpty(rowData.RFCode))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.RfRequired);
                }
            }


            if (string.IsNullOrEmpty(rowData.ObjCode) &&
                string.IsNullOrEmpty(rowData.Obj))
            {
                dataIsValid = false;
                if (rowData is RDEDocumentRowData)
                    validationProblems.Add(Resources.ObjNotFound);
                else
                    validationProblems.Add(Resources.ObjAndObjcodeEmpty);
            }
            if (!string.IsNullOrEmpty(rowData.ObjCode) &&
                !string.IsNullOrEmpty(rowData.Obj))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.ObjAndObjcodeEmpty);
            }

            if (rowData.CorrCode == null &&
                rowData.CorrDesc == null)
            {
                dataIsValid = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add(Resources.CorrDataNotFound);
                else
                    validationProblems.Add(Resources.CorrDataInvalid);
            }

            if (rowData.ProjectCodes != null && rowData.ProjectCodes.Length > 0 &&
                (!string.IsNullOrEmpty(rowData.ProjectDescription) ||
                 !string.IsNullOrEmpty(rowData.FolderDescrition) ||
                 !string.IsNullOrEmpty(rowData.NodeCode) ||
                 !string.IsNullOrEmpty(rowData.ProjectTipology)))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.CodFascAndProfFound);
            }

            if (isProfilationRequired && string.IsNullOrEmpty(rowData.DocumentTipology) && !(rowData is RDEDocumentRowData))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.TipNotFound);
            }

            if (isRapidClassificationRequired &&
                rowData.ProjectCodes == null &&
                string.IsNullOrEmpty(rowData.ProjectDescription) &&
                string.IsNullOrEmpty(rowData.FolderDescrition) &&
                string.IsNullOrEmpty(rowData.NodeCode) &&
                string.IsNullOrEmpty(rowData.ProjectTipology))
            {
                dataIsValid = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add(Resources.CodClassNotFound);
                else
                    validationProblems.Add(Resources.ClassRequiredButDataNotFound);
            }


            if (rowData is RDEDocumentRowData)
            {
                RDEDocumentRowData converted = rowData as RDEDocumentRowData;
                // Campo Stringa protocollo emergenza obbligatorio
                if (string.IsNullOrEmpty(converted.EmergencyProtocolSignature))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.StringProtObb);
                }

                // Se il campo Data protocollo emergenza non è valorizzato, non si può procedere
                if (string.IsNullOrEmpty(converted.EmergencyProtocolDate))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.DtaProtoObb);
                }

                // Se il campo Data protocollo emergenza non è valorizzato, non si può procedere
                if (string.IsNullOrEmpty(converted.EmergencyProtocolTime))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.OraProtoObb);

                }

                if (!string.IsNullOrEmpty(converted.EmergencyProtocolDate) &&
                    !string.IsNullOrEmpty(converted.EmergencyProtocolTime))
                {
                    string dataEmerg = converted.EmergencyProtocolDate + " " + converted.EmergencyProtocolTime;
                    DateTime dataEmergOut;

                    try
                    {
                        dataEmergOut = dataEmerg.AsDateTime();

                        if (dataEmergOut > System.DateTime.Now)
                        {
                            dataIsValid = false;
                            validationProblems.Add(Resources.DtaProtoEmInvalid);
                        }
                    }
                    catch (Exception ex)
                    {
                        dataIsValid = false;
                        validationProblems.Add(Resources.DtaProtoEmInvalidFormat);
                    }
                }

                if ((!string.IsNullOrEmpty(converted.ArrivalTime) && string.IsNullOrEmpty(converted.ArrivalDate)) ||
                    (!string.IsNullOrEmpty(converted.ArrivalDate) && string.IsNullOrEmpty(converted.ArrivalTime)))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.DtaArrPartInvalid);
                }

                if (!string.IsNullOrEmpty(converted.SenderProtocolDate) &&
                    DateTime.Parse(converted.SenderProtocolDate) > DateTime.Now)
                {
                    validationProblems.Add(Resources.DtaProtoInvalid);
                    dataIsValid = false;
                }

                if (!string.IsNullOrEmpty(converted.ArrivalDate))
                {
                    try
                    {
                        var arrivalDate = converted.ArrivalDate.AsDateTime();
                        // La data di arrivo deve essere minore della
                        // data attuale 
                        if (arrivalDate > DateTime.Now)
                        {
                            validationProblems.Add(Resources.DtaArrInvalid);
                            dataIsValid = false;
                        }

                        if (!string.IsNullOrEmpty(converted.SenderProtocolDate) &&
                            !string.IsNullOrEmpty(converted.ArrivalDate) &&
                            DateTime.Parse(converted.SenderProtocolDate) > arrivalDate)
                        {
                            dataIsValid = false;
                            validationProblems.Add(Resources.DtaProtoMittInvalid);
                        }
                    }
                    catch
                    {
                        validationProblems.Add(Resources.DtaOraInv);
                        dataIsValid = false;
                    }

                }
            }
            return (dataIsValid, validationProblems);
        }

        #endregion

        #region methods for prot part
        private async Task<(bool, List<string>)> IsPrPDocumentValid(
            DocumentRowData rowData,
            bool isProfilationRequired,
            bool isRapidClassificationRequired)
        {
            bool rfIsRequired = false;
            List<string> validationProblems = new();
            bool dataIsValid = true;

            if (string.IsNullOrEmpty(rowData.OrdinalNumber))
            {
                dataIsValid = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add(Resources.NumProtoReq);
                else
                    validationProblems.Add(Resources.OrdinalFieldIsRequired);
            }
            else
            {
                int ordinal = 0;
                if (!int.TryParse(rowData.OrdinalNumber, out ordinal))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.OrdinalFieldIsRequired);
                }

            }
            if (string.IsNullOrEmpty(rowData.AdminCode))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.CodAmIsRequired);
            }
            if (string.IsNullOrEmpty(rowData.RegCode))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.CodRegRequired);
            }
            string administrationSyd = (await this._mediator.Send(new Application.Requests.getIdAmmByCod(rowData.AdminCode))).output;

            if (administrationSyd == null)
            {
                dataIsValid = false;
                validationProblems.Add(string.Format(Resources.CantFetchDataAmm, rowData.AdminCode));
            }

            if (!string.IsNullOrEmpty(administrationSyd))
            {
                rfIsRequired = await this.AmmHasRf(administrationSyd);

                if (rfIsRequired && string.IsNullOrEmpty(rowData.RFCode))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.RfRequired);
                }
            }

            if (string.IsNullOrEmpty(rowData.ObjCode) &&
                string.IsNullOrEmpty(rowData.Obj))
            {
                dataIsValid = false;
                if (rowData is RDEDocumentRowData)
                    validationProblems.Add(Resources.ObjNotFound);
                else
                    validationProblems.Add(Resources.ObjAndObjcodeEmpty);
            }
            if (!string.IsNullOrEmpty(rowData.ObjCode) &&
                !string.IsNullOrEmpty(rowData.Obj))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.ObjAndObjcodeEmpty);
            }


            if (rowData.CorrCode == null &&
                rowData.CorrDesc == null)
            {
                dataIsValid = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add(Resources.CorrDataNotFound);
                else
                    validationProblems.Add(Resources.CorrDataInvalid);
            }
            bool primCorrFound = false;
            var sendersNum = 0;
            if (rowData.CorrCode != null)
            {
                // Per ogni codice corrispondente contenuto nella lista...
                foreach (string corrCode in rowData.CorrCode)
                {
                    // ...se contiene #M#, si incrementa il contatore di mittenti
                    if (corrCode.ToUpper().Contains("#M#"))
                        sendersNum++;

                    // ...se contiene #D#, significa che esiste almeno un corrispondente
                    // principale
                    if (corrCode.ToUpper().Contains("#D#") &&
                        corrCode.Trim().Length > 3)
                        primCorrFound = true;

                }

            }


            var sendersNumInCodCorr = 0;
            // Se il campo 'Codice Corrispondenti' è valorizzato...
            if (rowData.CorrDesc != null)
            {
                // Per ogni codice corrispondente contenuto nella lista...
                foreach (string corrDesc in rowData.CorrDesc)
                {
                    // ...se contiene #M#, si incrementa il contatore di mittenti
                    if (corrDesc.ToUpper().Contains("#M#"))
                        sendersNumInCodCorr++;

                    // ...se contiene #D#, significa che esiste almeno un corrispondente
                    // principale
                    if (corrDesc.ToUpper().Contains("#D#") &&
                        corrDesc.Trim().Length > 3)
                        primCorrFound = true;

                }

                // Se sono stati individuati mittenti, errore
                if (sendersNumInCodCorr != 0)
                {
                    dataIsValid = false;

                    if (rowData is RDEDocumentRowData)
                        validationProblems.Add(Resources.InvalidMitt);
                    else
                        validationProblems.Add(Resources.CorrFieldCantHaveSenderData);
                }

            }

            // Se ne sono stati trovati più mittenti, la validazione non passa
            if (sendersNum > 1)
            {
                dataIsValid = false;
                validationProblems.Add(string.Format(Resources.MultipleSendersFound, sendersNum));
            }
            
            // Se non è stato individuato alcun destinatario primario, errore
            if (!primCorrFound)
            {
                dataIsValid = false;
                validationProblems.Add(Resources.NoCorrFound);
            }
            if (rowData.ProjectCodes != null && rowData.ProjectCodes.Length > 0 &&
               (!string.IsNullOrEmpty(rowData.ProjectDescription) ||
                !string.IsNullOrEmpty(rowData.FolderDescrition) ||
                !string.IsNullOrEmpty(rowData.NodeCode) ||
                !string.IsNullOrEmpty(rowData.ProjectTipology)))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.CodFascAndProfFound);
            }

            if (isProfilationRequired && string.IsNullOrEmpty(rowData.DocumentTipology) && !(rowData is RDEDocumentRowData))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.TipNotFound);
            }

            if (isRapidClassificationRequired &&
                rowData.ProjectCodes == null &&
                string.IsNullOrEmpty(rowData.ProjectDescription) &&
                string.IsNullOrEmpty(rowData.FolderDescrition) &&
                string.IsNullOrEmpty(rowData.NodeCode) &&
                string.IsNullOrEmpty(rowData.ProjectTipology))
            {
                dataIsValid = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add(Resources.CodClassNotFound);
                else
                    validationProblems.Add(Resources.ClassRequiredButDataNotFound);
            }


            if (rowData is RDEDocumentRowData)
            {
                var converted = ((RDEDocumentRowData)rowData);

                if (string.IsNullOrEmpty(converted.EmergencyProtocolSignature))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.StringProtObb);
                }

                // Se il campo Data protocollo emergenza non è valorizzato, non si può procedere
                if (string.IsNullOrEmpty(converted.EmergencyProtocolDate))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.DtaProtoObb);
                }

                // Se il campo Data protocollo emergenza non è valorizzato, non si può procedere
                if (string.IsNullOrEmpty(converted.EmergencyProtocolTime))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.OraProtoObb);

                }

                if (!string.IsNullOrEmpty(converted.EmergencyProtocolDate) &&
                    !string.IsNullOrEmpty(converted.EmergencyProtocolTime))
                {
                    string dataEmerg = converted.EmergencyProtocolDate + " " + converted.EmergencyProtocolTime;
                    DateTime dataEmergOut;

                    try
                    {
                        dataEmergOut = dataEmerg.AsDateTime();

                        if (dataEmergOut > System.DateTime.Now)
                        {
                            dataIsValid = false;
                            validationProblems.Add(Resources.DtaProtoEmInvalid);
                        }
                    }
                    catch (Exception ex)
                    {
                        dataIsValid = false;
                        validationProblems.Add(Resources.DtaProtoEmInvalidFormat);
                    }
                }
            }
            return (dataIsValid, validationProblems);
        }
        #endregion


        #region method for prot int validation 
        private async Task<(bool, List<string>)> IsPrIntDocumentValid(
            DocumentRowData rowData,
            bool isProfilationRequired,
            bool isRapidClassificationRequired)
        {

            List<string> validationProblems = new();
            bool dataIsValid = true;
            bool rfIsRequired = false;
            bool corrOk = true;

            if (string.IsNullOrEmpty(rowData.OrdinalNumber))
            {
                dataIsValid = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add(Resources.NumProtoReq);
                else
                    validationProblems.Add(Resources.OrdinalFieldIsRequired);
            }
            else
            {
                int ordinal = 0;
                if (!int.TryParse(rowData.OrdinalNumber, out ordinal))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.OrdinalFieldIsRequired);
                }

            }

            if (string.IsNullOrEmpty(rowData.AdminCode))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.CodAmIsRequired);
            }
            if (string.IsNullOrEmpty(rowData.RegCode))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.CodRegRequired);
            }

            string administrationSyd = (await this._mediator.Send(new Application.Requests.getIdAmmByCod(rowData.AdminCode))).output;

            if (administrationSyd == null)
            {
                dataIsValid = false;
                validationProblems.Add(string.Format(Resources.CantFetchDataAmm, rowData.AdminCode));
            }

            if (!string.IsNullOrEmpty(administrationSyd))
            {
                rfIsRequired = await this.AmmHasRf(administrationSyd);

                if (rfIsRequired && string.IsNullOrEmpty(rowData.RFCode))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.RfRequired);
                }
            }

            if (string.IsNullOrEmpty(rowData.ObjCode) &&
                string.IsNullOrEmpty(rowData.Obj))
            {
                dataIsValid = false;
                if (rowData is RDEDocumentRowData)
                    validationProblems.Add(Resources.ObjNotFound);
                else
                    validationProblems.Add(Resources.ObjAndObjcodeEmpty);
            }
            if (!string.IsNullOrEmpty(rowData.ObjCode) &&
                !string.IsNullOrEmpty(rowData.Obj))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.ObjAndObjcodeEmpty);
            }

            var primCorrFound = false;
            var sendersNum = 0;
            // Se il campo 'Codice Corrispondenti' è valorizzato ma non contiene #D#,
            // significa che non è stato specificato un destinatario principale e di
            // conseguenza la validazione non passa.
            // Se contine più di un #M#, significa che sono stati specificati più mittenti
            // e di conseguenza la validazione non passa.
            // Se il campo 'Codice Corrispondenti' è valorizzato...


            if (rowData.CorrCode != null)
            {
                // Per ogni codice corrispondente contenuto nella lista...
                foreach (string corrCode in rowData.CorrCode)
                {
                    // ...se contiene #M#, si incrementa il contatore di mittenti
                    if (corrCode.ToUpper().Contains("#M#"))
                        sendersNum++;

                    // ...se contiene #D#, significa che esiste almeno un corrispondente
                    // principale
                    if (corrCode.ToUpper().Contains("#D#") &&
                        corrCode.Trim().Length > 3)
                        primCorrFound = true;

                }

            }

            if (corrOk)
            {
                // Se non sono stati individuati mittenti, errore
                if (sendersNum == 0)
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.NoSenderFound);
                }

                // Se sono stati trovati più mittenti, la validazione non passa
                if (sendersNum > 1)
                {
                    dataIsValid = false;
                    validationProblems.Add(string.Format(Resources.MultipleSendersFound, sendersNum));
                }

                // Se non è stato individuato alcun destinatario primario, errore
                if (!primCorrFound)
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.NoPrimaryDestFound);
                }
            }

            if (rowData.ProjectCodes != null && rowData.ProjectCodes.Length > 0 &&
                (!string.IsNullOrEmpty(rowData.ProjectDescription) ||
                 !string.IsNullOrEmpty(rowData.FolderDescrition) ||
                 !string.IsNullOrEmpty(rowData.NodeCode) ||
                 !string.IsNullOrEmpty(rowData.ProjectTipology)))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.CodFascAndProfFound);
            }

            if (isProfilationRequired && string.IsNullOrEmpty(rowData.DocumentTipology) && !(rowData is RDEDocumentRowData))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.TipNotFound);
            }

            if (isRapidClassificationRequired &&
                rowData.ProjectCodes == null &&
                string.IsNullOrEmpty(rowData.ProjectDescription) &&
                string.IsNullOrEmpty(rowData.FolderDescrition) &&
                string.IsNullOrEmpty(rowData.NodeCode) &&
                string.IsNullOrEmpty(rowData.ProjectTipology))
            {
                dataIsValid = false;

                if (rowData is RDEDocumentRowData)
                    validationProblems.Add(Resources.CodClassNotFound);
                else
                    validationProblems.Add(Resources.ClassRequiredButDataNotFound);
            }

            if (rowData is RDEDocumentRowData)
            {
                var converted = ((RDEDocumentRowData)rowData);

                if (string.IsNullOrEmpty(converted.EmergencyProtocolSignature))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.StringProtObb);
                }

                // Se il campo Data protocollo emergenza non è valorizzato, non si può procedere
                if (string.IsNullOrEmpty(converted.EmergencyProtocolDate))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.DtaProtoObb);
                }

                // Se il campo Data protocollo emergenza non è valorizzato, non si può procedere
                if (string.IsNullOrEmpty(converted.EmergencyProtocolTime))
                {
                    dataIsValid = false;
                    validationProblems.Add(Resources.OraProtoObb);

                }

                if (!string.IsNullOrEmpty(converted.EmergencyProtocolDate) &&
                    !string.IsNullOrEmpty(converted.EmergencyProtocolTime))
                {
                    string dataEmerg = converted.EmergencyProtocolDate + " " + converted.EmergencyProtocolTime;
                    DateTime dataEmergOut;

                    try
                    {
                        dataEmergOut = dataEmerg.AsDateTime();

                        if (dataEmergOut > System.DateTime.Now)
                        {
                            dataIsValid = false;
                            validationProblems.Add(Resources.DtaProtoEmInvalid);
                        }
                    }
                    catch (Exception ex)
                    {
                        dataIsValid = false;
                        validationProblems.Add(Resources.DtaProtoEmInvalidFormat);
                    }
                }
            }


            return (dataIsValid, validationProblems);
        }
        #endregion

        #region method for attach validation

        // checks if attachment has ordinal number and (has MainDocumentId or MainOrdinal) and has AdministrationCode and description
        private (bool, List<string>) IsAttachmentDataValid(DocumentRowData rowData)
        {
            List<string> validationProblems = new();
            bool dataIsValid = true;

            if (string.IsNullOrEmpty(rowData.OrdinalNumber))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.OrdinalFieldIsRequired);
            }

            if (string.IsNullOrEmpty(rowData.MainDocumentId) && string.IsNullOrEmpty(rowData.MainOrdinal))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.PrincipalOrdinalOrMainDocIdIsRequired);
            }

            if (string.IsNullOrEmpty(rowData.AdminCode))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.CodAmIsRequired);
            }

            if (string.IsNullOrEmpty(rowData.Obj))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.DescriptionIsRequired);
            }
            if (string.IsNullOrEmpty(rowData.ObjCode) &&
                string.IsNullOrEmpty(rowData.Obj))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.ObjAndObjcodeEmpty);
            }

            if (!string.IsNullOrEmpty(rowData.ObjCode) &&
                !string.IsNullOrEmpty(rowData.Obj))
            {
                dataIsValid = false;
                validationProblems.Add(Resources.ObjAndObjcodeBothVal);
            }


            return (dataIsValid, validationProblems);
        }
        #endregion

        #region validation utils
        private ImportResult GetDocumentType(string mainOrdinal, ResultsContainer container)
        {
            ImportResult toReturn = null;

            List<ImportResult> documentTypeReport = null;

            // Espressione regolare di contollo numerico
            Regex isNumeric = new Regex(@"^\d+$", RegexOptions.None, TimeSpan.FromSeconds(5));

            // L'indice da cui partire per cominciare a tagliare la stringa
            // codificata
            int startIndex = 1;


            // Se mainOrdinal contiene almeno due caratteri...
            if (mainOrdinal.Trim().Length > 1)
            {
                // Individuazione della tipologia di documento (fra A, P, I)
                switch (mainOrdinal[0].ToString().ToUpper())
                {
                    case "A":   // Arrivo
                        documentTypeReport = container.InDocument;
                        break;

                    case "P":   // Partenza
                        documentTypeReport = container.OutDocument;
                        break;

                    case "I":   // Interno
                        documentTypeReport = container.OwnDocument;
                        break;

                }

                // Se il documento non è stato identificato, si prova a vedere
                // se è un NP (Non protocollato)
                if (documentTypeReport == null && mainOrdinal.Substring(0, 2).ToUpper() == "NP")
                {
                    documentTypeReport = container.GrayDocument;
                    startIndex = 2;
                }

                // Se documentType è stato identificato, si procede con l'analisi
                // di validità dell'ordinale.
                /*
                 * Principio di validità.
                 * 
                 * La lunghezza dell'ordinale del principale deve contenere almeno
                 * un carattere in più rispetto alla stringa di specifica
                 * della tipologia di documento.
                 * L'ordinale, posizionato a partire dalla posizione Len(numCharID)
                 * fino a Len(mainOrdinal) deve essere un numero.
                 * 
                 */
                if (documentTypeReport != null &&
                    mainOrdinal.Trim().Length > startIndex &&
                    isNumeric.Match(mainOrdinal.Substring(startIndex)).Success)
                    toReturn = documentTypeReport.Where(
                        e => e.Ordinal == mainOrdinal.Substring(startIndex)).FirstOrDefault();


            }

            // Restituzione del risultato
            return toReturn;

        }
        private async Task<ImportResult> CheckMainDocumentExistence(InfoUtente userInfo, string mainDocumentId)
        {
            ImportResult toReturn = new ImportResult();

            DocsPaVO.documento.SchedaDocumento document = new DocsPaVO.documento.SchedaDocumento();
            try
            {
                document = (await this._mediator.Send(new Application.Requests.DocumentoGetDettaglioDocumentoNoSecurity(userInfo, mainDocumentId, mainDocumentId))).output;
                if (document != null)
                {
                    toReturn.IdProfile = document.docNumber;
                    toReturn.DocNumber = document.docNumber;
                }
            }
            catch (Exception ex)
            {
                document = null;
                toReturn = null;
                this._logger.LogWebMethodError(ex);
            }

            return toReturn;
        }


        private async Task<bool> AmmHasRf(string sysId)
        {
            bool output = false;
            var amm = await this._dbContext.AmministraEntities.AsNoTracking().FirstOrDefaultAsync(a => a.SYSTEM_ID == sysId.AsLong());
            if (amm != null && amm.VAR_FORMATO_SEGNATURA != null && amm.VAR_FORMATO_SEGNATURA.Contains("COD_RF_PROT"))
            {
                output = true;
            }
            return output;
        }
        #endregion


        private async Task<DocsPaVO.utente.Registro> GetRegistroByCodAoo(string codAoo, string idAmministrazione)
        {
            DocsPaVO.utente.Registro reg = null;
            if (!(codAoo != null && !codAoo.Equals("")))
            {
                return reg;
            }

            var r = await (from a in this._dbContext.RegistroEntities.AsNoTracking()
                           where a.VAR_CODICE != null && a.VAR_CODICE.ToUpper().Contains(codAoo.ToUpper()) && a.ID_AMM == idAmministrazione.AsLong()
                           select new Registro()
                           {
                               systemId = a.SYSTEM_ID.ToString(),
                               codRegistro = a.VAR_CODICE,
                               codice = a.NUM_RIF != null ? a.NUM_RIF.ToString() : null,
                               descrizione = a.VAR_DESC_REGISTRO,
                               email = a.VAR_EMAIL_REGISTRO,
                               stato = a.CHA_STATO,
                               idAmministrazione = a.ID_AMM != null ? a.ID_AMM.ToString() : string.Empty,
                               dataApertura = a.DTA_OPEN != null ? a.DTA_OPEN.AsDateFormat() : null,
                               dataChiusura = a.DTA_CLOSE != null ? a.DTA_CLOSE.AsDateFormat() : null,
                               dataUltimoProtocollo = a.DTA_ULTIMO_PROTO != null ? a.DTA_ULTIMO_PROTO.AsDateFormat() : null,
                               idRuoloAOO = a.ID_RUOLO_AOO != null ? a.ID_RUOLO_AOO.ToString() : null,
                               idRuoloResp = a.ID_RUOLO_RESP != null ? a.ID_RUOLO_RESP.ToString() : null,
                               idUtenteAOO = a.ID_PEOPLE_AOO != null ? a.ID_PEOPLE_AOO.ToString() : null,
                               autoInterop = a.CHA_AUTO_INTEROP,
                               chaRF = a.CHA_RF,
                               rfDisabled = a.CHA_DISABILITATO,
                               idAOOCollegata = a.ID_AOO_COLLEGATA != null ? a.ID_AOO_COLLEGATA.ToString() : null,
                               invioRicevutaManuale = a.INVIO_RICEVUTA_MANUALE != null ? a.INVIO_RICEVUTA_MANUALE.ToString() : null,
                           }).FirstOrDefaultAsync();

            r.codAmministrazione = await this.GetCodiceAmm(reg.idAmministrazione);

            return r;
        }
        private async Task<DocsPaVO.utente.Registro> GetRegistroById(string registrySid)
        {
            DocsPaVO.utente.Registro reg = null;
            if (!(registrySid != null && !registrySid.Equals("")))
            {
                return reg;
            }

            var r = await (from a in this._dbContext.RegistroEntities.AsNoTracking()
                           where a.SYSTEM_ID == registrySid.AsLong()
                           select new Registro()
                           {
                               systemId = a.SYSTEM_ID.ToString(),
                               codRegistro = a.VAR_CODICE,
                               codice = a.NUM_RIF != null ? a.NUM_RIF.ToString() : null,
                               descrizione = a.VAR_DESC_REGISTRO,
                               email = a.VAR_EMAIL_REGISTRO,
                               stato = a.CHA_STATO,
                               idAmministrazione = a.ID_AMM != null ? a.ID_AMM.ToString() : string.Empty,
                               dataApertura = a.DTA_OPEN != null ? a.DTA_OPEN.AsDateFormat() : null,
                               dataChiusura = a.DTA_CLOSE != null ? a.DTA_CLOSE.AsDateFormat() : null,
                               dataUltimoProtocollo = a.DTA_ULTIMO_PROTO != null ? a.DTA_ULTIMO_PROTO.AsDateFormat() : null,
                               idRuoloAOO = a.ID_RUOLO_AOO != null ? a.ID_RUOLO_AOO.ToString() : null,
                               idRuoloResp = a.ID_RUOLO_RESP != null ? a.ID_RUOLO_RESP.ToString() : null,
                               idUtenteAOO = a.ID_PEOPLE_AOO != null ? a.ID_PEOPLE_AOO.ToString() : null,
                               autoInterop = a.CHA_AUTO_INTEROP,
                               chaRF = a.CHA_RF,
                               rfDisabled = a.CHA_DISABILITATO,
                               idAOOCollegata = a.ID_AOO_COLLEGATA != null ? a.ID_AOO_COLLEGATA.ToString() : null,
                               invioRicevutaManuale = a.INVIO_RICEVUTA_MANUALE != null ? a.INVIO_RICEVUTA_MANUALE.ToString() : null,
                           }).FirstOrDefaultAsync();

            r.codAmministrazione = await this.GetCodiceAmm(r.idAmministrazione);

            return r;
        }
        private async Task<string> GetCodiceAmm(string idAmm)
        {
            string cod = await this._dbContext.AmministraEntities.AsNoTracking().Where(a => a.SYSTEM_ID == idAmm.AsLong()).Select(a => a.VAR_CODICE_AMM).FirstOrDefaultAsync();
            return cod;
        }

        private async Task<string> GetIdRegistro(string sysIdAmm, string codiceReg)
        {
            var regId = await this._dbContext.RegistroEntities.AsNoTracking().
                Where(r => r.ID_AMM == sysIdAmm.AsLong() && r.VAR_CODICE != null && r.VAR_CODICE.ToUpper().Equals(codiceReg.ToUpper())).
                Select(r => r.SYSTEM_ID).FirstOrDefaultAsync();
            return regId != null ? regId.ToString() : string.Empty;
        }

        private async Task<string> GetRfId(string rfCode, string administrationId)
        {
            var reg = await this._dbContext.RegistroEntities.AsNoTracking().
                Where(r => r.ID_AMM == administrationId.AsLong() && r.VAR_CODICE != null && r.VAR_CODICE.ToUpper().Equals(rfCode.ToUpper())).FirstOrDefaultAsync();

            if (reg == null)
            {
                throw new NoRfFoundPi3Exception(string.Format(Resources.RegNotFound, rfCode));
            }

            if (reg.CHA_RF == null && !reg.CHA_RF.Trim().Equals("1"))
            {
                throw new NoRfFoundPi3Exception(string.Format(Resources.RegNotRF, rfCode));
            }
            return reg.SYSTEM_ID.ToString();
        }

        private async Task<string> GetIdTit(string titolarioName, string administrationSyd)
        {
            string titSid = string.Empty;
            if (string.IsNullOrEmpty(titolarioName))
            {
                var ti = await (from a in this._dbContext.AmministraEntities.AsNoTracking()
                                from p in this._dbContext.ProjectEntities.AsNoTracking()
                                where p.ID_AMM == a.SYSTEM_ID &&
                                p.CHA_STATO != null &&
                                p.CHA_STATO.Equals("A") &&
                                p.ID_AMM == administrationSyd.AsLong() &&
                                p.ID_TITOLARIO == 0 &&
                                p.VAR_CODICE != null &&
                                p.VAR_CODICE.Equals("T") &&
                                p.ID_PARENT == 0
                                select p).FirstOrDefaultAsync();
                titSid = ti != null ? ti.SYSTEM_ID.ToString() : string.Empty;
            }
            else
            {
                var titolari = (await this._mediator.Send(new Application.Requests.getTitolariUtilizzabili(administrationSyd))).output;
                var t = titolari.Where(e => ((DateTime.Parse(e.DataAttivazione).Year.ToString() == titolarioName) && (e.Stato == OrgStatiTitolarioEnum.Chiuso))).FirstOrDefault();
                if (t != null)
                {
                    titSid = t.ID;
                }

            }
            return titSid;
        }




    }
}
