// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.InstanceAccess.Metadata;
using DocsPaVO.Interoperabilita;
using DocsPaVO.utente;
using Google.Apis.Gmail.v1;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.EmailCallback;
using Pi3.App.Legacy.WebApi.Application.Handlers.ScanGmailCallback.Exceptions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.Google;
using Pi3.App.Legacy.WebApi.Application.Services.Principal;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.Interoperabilita.MailAccountCheckResponse;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Services.WebMethodLogger;
using DocsPaVO.trasmissione;
using AutoMapper;
using System.Globalization;
using Pi3.App.Legacy.WebApi.Application.Services.Aac;
using Microsoft.Extensions.Caching.Distributed;
using Pi3.App.Legacy.WebApi.Application.Services.Aac.Domain;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ScanGmailCallback
{
    public class ScanGmailCallbackHandler : IRequestHandler<Requests.ScanGmailCallback, Requests.ScanGmailCallbackResult>
    {
        #region Public Members

        public ScanGmailCallbackHandler(
            ILogger<ScanGmailCallbackHandler> logger,
            IAuthProviderFactoryService authProviderFactoryService,
            IConfiguration configuration,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            IEmailBoxScannerService emailBoxScannerService,
            IDocumentoAmministrativoRepository documentoAmministrativo,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentBlobRepository documentBlobRepository,
            IAacFetcherService _aacFetcherService)
        {
            this._logger = logger;
            this._authProviderFactoryService = authProviderFactoryService;
            this._configuration = configuration;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._emailBoxScannerService = emailBoxScannerService;
            this._webMethodLoggerService = webMethodLoggerService;
            this._documentBlobRepository = documentBlobRepository;
            this._documentoAmministrativoRepository = documentoAmministrativo;
            this._aacFetcherService = _aacFetcherService;
            this._options = configuration.GetSection("AACOptions").Get<AacOptions>();
            this._cacheExpInMinutes = configuration.GetSection("DistributedCacheServiceOptions:ExpireCache").Value ?? string.Empty;
            this._keyTokenName = configuration.GetSection("DistributedCacheServiceOptions:KeyTokenName").Value ?? "KeyToken";

            this.InitializeMapper();
        }

        public async virtual Task<ScanGmailCallbackResult> Handle(Requests.ScanGmailCallback request, CancellationToken cancellationToken)
        {
            var code = request.Code;
            var state = request.State;
            var instance = request.Instance;
            var codiceAmministrazione = request.CodiceAmministrazione;
            var codiceRegistro = request.CodiceRegistro;

            this._logger.LogInformation($"ScanGmail - AuthCallback - code: {code}, state: {state}, instance: {instance}, codiceAmministrazione: {codiceAmministrazione}, codiceRegistro: {codiceRegistro}");

            DateTime beginDate = DateTime.Now;
            List<ScanGmailCallbackMessageSummary> messageSummaries = new();

            var authProvider = _authProviderFactoryService.Create(instance, codiceAmministrazione, codiceRegistro);

            var redirectUrl = this._configuration[$"GoogleOptions:{instance}-{codiceAmministrazione}-{codiceRegistro}:RedirectUrl"];

            if (string.IsNullOrWhiteSpace(redirectUrl))
                throw new ArgumentNullException(nameof(redirectUrl));

            this._logger.LogInformation($"ScanGmail - AuthCallback - redirectUrl: {redirectUrl}");

            var cred = await authProvider.GetCredentialWithCodeAsync(code, redirectUrl!);

            var service = new GmailService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = cred
            });

            var gmailProfile = service.Users.GetProfile("me").Execute();
            var userGmailEmail = gmailProfile.EmailAddress;

            await this.CreatePi3Identity(instance, codiceAmministrazione, codiceRegistro, userGmailEmail);

            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            this._logger.LogInformation($"ClaimsPrincipal inizializzato.");

            const string labelIds = "INBOX";

            var messageRequest = service.Users.Messages.List("me");
            messageRequest.LabelIds = labelIds;
            messageRequest.IncludeSpamTrash = false;

            var response = await messageRequest.ExecuteAsync();

            this._logger.LogInformation($"Caricamento messaggi da cartella {labelIds} completato. Letti {response.Messages?.Count} messaggi.");

            var item = 1;

            var reg = await GetRegistro(codiceRegistro, userGmailEmail);

            var existsScanAlreadyInProgress = await this._pi3DbContext.CheckMailboxEntities
                    .AsNoTracking()
                    .AnyAsync(c => c.CONCLUDED == "0" && c.IDREG == reg.systemId.AsLong() && c.MAIL == reg.email);
            if (existsScanAlreadyInProgress)
                throw new ScanGmailCallbackPi3Exception(ErrorDescriptions.ScanAlreadyInProgress, null, reg.email);

            var idCorrGlobaliRuolo = await this._pi3DbContext.CorrGlobaliEntities.Where(x => x.ID_GRUPPO == idGroup).Select(x => x.SYSTEM_ID).FirstOrDefaultAsync();
            var idCorrGlobaliUtente = await this._pi3DbContext.CorrGlobaliEntities.Where(x => x.ID_PEOPLE == idPeople).Select(x => x.SYSTEM_ID).FirstOrDefaultAsync();

            var jobEntity = new JobEntity();

            await this._pi3DbContext.JobEntities.AddAsync(jobEntity);
            await ((DbContext)this._pi3DbContext).SaveChangesAsync();

            var checkMailboxEntity = new CheckMailboxEntity
            {
                IDJOB = jobEntity.ID,
                IDUSER = idCorrGlobaliUtente,
                IDROLE = idCorrGlobaliRuolo,
                IDREG = reg.systemId.AsLong(),
                MAIL = userGmailEmail,
                TOTAL = 0,
                ELABORATE = 0,
                CONCLUDED = "0"
            };

            await this._pi3DbContext.CheckMailboxEntities.AddAsync(checkMailboxEntity);
            await ((DbContext)this._pi3DbContext).SaveChangesAsync();

            foreach (var message in response.Messages ?? new List<Google.Apis.Gmail.v1.Data.Message>())
            {
                string messageId = message.Id;
                string from = null!;
                string to = null!;
                string cc = null!;
                string bcc = null!;
                string subject = null!;
                int attachments = 0;
                var handled = false;

                DateTime startProcessedMessage = DateTime.Now;

                try
                {
                    this._logger.LogInformation($"Analisi messaggio {item} di {response.Messages.Count} in corso...");

                    var getEmailRequest = service.Users.Messages.Get("me", message.Id);

                    var email = await getEmailRequest.ExecuteAsync();

                    var parsedEmail = await GmailMessageParser.Parse(email, service, this._emailBoxScannerService);

                    from = (parsedEmail.Sender ?? new EmailSender()).Address;
                    to = string.Join(";", (parsedEmail.To ?? new List<EmailRecipient>()).Select(t => t.Address));
                    cc = string.Join(";", (parsedEmail.Cc ?? new List<EmailRecipient>()).Select(t => t.Address));
                    bcc = string.Join(";", (parsedEmail.Bcc ?? new List<EmailRecipient>()).Select(t => t.Address));
                    subject = (parsedEmail.Subject ?? new TextValue(string.Empty)).ToString();
                    attachments = parsedEmail.Attachments.Count;

                    var emailCallbackResponse = await this._mediator.Send(new EmailCallbackRequest()
                    {
                        Email = parsedEmail,
                        Item = item,
                        Total = response.Messages.Count(),
                        Registro = reg,
                        IdCheckMailbox = checkMailboxEntity.ID,
                        EmailAddress = reg.email
                    });

                    handled = emailCallbackResponse.Output;

                    this._logger.LogInformation($"Analisi messaggio {item} di {response.Messages.Count} completato.");
                }
                catch (Pi3Exception pi3Ex)
                {
                    handled = false;
                    _logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                }
                catch (Exception ex)
                {
                    handled = false;
                    _logger.LogCritical(exception: ex, message: ex.Message);
                }
                finally
                {
                    DateTime endProcessedMessage = DateTime.Now;

                    messageSummaries.Add(new ScanGmailCallbackMessageSummary(
                        messageId,
                        from,
                        to,
                        cc,
                        bcc,
                        subject,
                        attachments,
                        startProcessedMessage,
                        endProcessedMessage,
                        (endProcessedMessage.Subtract(startProcessedMessage).TotalSeconds),
                        handled));
                }

                item++;
            }

            DateTime endDate = DateTime.Now;

            checkMailboxEntity.CONCLUDED = "1";
            checkMailboxEntity.DTA_CONCLUDED = await _pi3DbContext.GetSystemDateTime();

            bool concluded = (await ((DbContext)this._pi3DbContext).SaveChangesAsync()) == 1;

            #region Creazione report - SOLO SE CI SONO STATE MAIL ELABORATE
            var infoReportMailboxResult = (await this._mediator.Send(new Application.Requests.InfoReportMailbox(checkMailboxEntity.ID.ToString()))).output;

            var reportResult = (await this._mediator.Send(new Application.Requests.ExportRicercaCasellaIstituzionale(new DocsPaVO.Report.PrintReportRequestDataset()
            {
                UserInfo = new InfoUtente(),
                SearchFilters = null,
                ContextName = Resources.ReportContextName,
                Title = string.Format(Resources.ReportTitle, userGmailEmail),
                SubTitle = string.Format(Resources.ReportSubTitle, checkMailboxEntity.DTA_CONCLUDED?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), checkMailboxEntity.DTA_CONCLUDED?.ToString("HH:mm")),
                ReportType = DocsPaVO.Report.ReportTypeEnum.PDF,
                ReportKey = Resources.ReportKey,
                InputDataset = this.ConvertToDataset(infoReportMailboxResult)
            }))).Document;

            if (infoReportMailboxResult.MailProcessedList.Count() > 0)
            {
                var dataCreazione = await this._pi3DbContext.GetSystemDateTime();
                var documentoAmministrativoAggregate = new DocumentoAmministrativo(idTenant.ToString(),
                    dataCreazione,
                    new OggettoDelDocumento()
                    {
                        Descrizione = new TextValue(String.Format(Resources.ReportSubject, reg.codice, checkMailboxEntity.DTA_CONCLUDED?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), checkMailboxEntity.DTA_CONCLUDED?.ToString("HH:mm"))),
                    },
                    new DatiRegistro()
                    {
                        IdRegistro = reg.systemId
                    });

                if (reportResult != null)
                {
                    var newDocumentBlobAggregate = new DocumentBlob(idTenant.ToString(), DateTime.Now, new TextValue(reportResult.name));
                    newDocumentBlobAggregate.UploadStream(new MemoryStream(reportResult.content), reportResult.name);
                    newDocumentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                    await _documentBlobRepository.Add(newDocumentBlobAggregate);

                    documentoAmministrativoAggregate.AssignDocumentBlobRef(
                        new DocumentBlobRef()
                        {
                            IdBlob = newDocumentBlobAggregate.Id,
                            FileName = newDocumentBlobAggregate.FileName,
                            ContentType = newDocumentBlobAggregate.ContentType,
                            FileSize = newDocumentBlobAggregate.FileSize,
                            CreationDate = await _pi3DbContext.GetSystemDateTime(),
                            Hash = newDocumentBlobAggregate.Hash,
                            HashName = HashNamesEnum.SHA256,
                            Cartaceo = false,
                            SegnaturaPermanente = false,
                            TipoFirma = TipoFirmaEnum.Nessuna
                        },
                        new TargetVersionBehavior()
                        {
                            CreateNewVersion = true
                        });
                }

                await _documentoAmministrativoRepository.Add(documentoAmministrativoAggregate);

                await this._webMethodLoggerService.LogOK("DOCUMENTOADDDOCGRIGIA", documentoAmministrativoAggregate.Id, string.Format(Resources.LogDocumentoAddDocGrigio, documentoAmministrativoAggregate.Id));
                #endregion

                #region Trasmissione
                if (!string.IsNullOrEmpty(documentoAmministrativoAggregate.Id))
                {
                    Ruolo role = await GetRuolo(idCorrGlobaliRuolo);
                    Utente user = (await this._mediator.Send(new Application.Requests.getUtenteById(idPeople.ToString()))).output;
                    Trasmissione trasm = new Trasmissione();
                    TrasmissioneSingola trasmS = new TrasmissioneSingola();
                    TrasmissioneUtente trasmU = new TrasmissioneUtente();

                    trasm.DataDocFasc = documentoAmministrativoAggregate.CreationDate.ToString();
                    trasm.infoDocumento = (await this._mediator.Send(new Application.Requests.GetInfoDocumento(new InfoUtente(), documentoAmministrativoAggregate.Id, documentoAmministrativoAggregate.Id))).output;
                    trasm.ruolo = role;
                    trasm.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                    trasm.utente = user;

                    trasmS.ragione = (await this._mediator.Send(new Application.Requests.GetRagioneNotifica(idTenant.ToString()))).output;
                    trasmS.tipoDest = TipoDestinatario.RUOLO;
                    trasmS.tipoTrasm = "S";
                    trasmS.corrispondenteInterno = role;

                    List<DocsPaVO.trasmissione.TrasmissioneUtente> trasmissioniUt = new List<DocsPaVO.trasmissione.TrasmissioneUtente>();

                    DocsPaVO.addressbook.QueryCorrispondente qc = new DocsPaVO.addressbook.QueryCorrispondente();
                    qc.codiceRubrica = role.codiceRubrica;
                    List<string> registri = new List<string>();
                    registri.Add(reg.systemId);
                    qc.idRegistri = registri.ToArray();
                    qc.idAmministrazione = reg.idAmministrazione;
                    qc.getChildren = true;
                    qc.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                    qc.fineValidita = true;
                    List<DocsPaVO.utente.Corrispondente> utenti = (await this._mediator.Send(new Requests.AddressbookGetListaCorrispondenti(qc))).output.ToList();

                    foreach (var utente in utenti)
                    {
                        DocsPaVO.trasmissione.TrasmissioneUtente trUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                        trUt.utente = (DocsPaVO.utente.Utente)utente;
                        trasmU.daNotificare = true;
                        trasmissioniUt.Add(trUt);
                    }
                    trasmS.trasmissioneUtente = trasmissioniUt.ToArray();

                    trasm.trasmissioniSingole = new TrasmissioneSingola[1];
                    trasm.trasmissioniSingole[0] = trasmS;
                    Trasmissione trasmRes = (await this._mediator.Send(new Requests.TrasmissioneSaveExecuteTrasm(string.Empty, trasm, new InfoUtente()))).output;

                    if (trasmRes == null)
                        this._logger.LogError($"Trasmissione al ruolo {role.codice} non effettuata.");

                }
            }
            #endregion

            #region Eliminazione riga DPA_CHECK_MAILBOX
            if (concluded)
            {
                this._pi3DbContext.CheckMailboxEntities.Remove(checkMailboxEntity);
                await ((DbContext)_pi3DbContext).SaveChangesAsync();
            }
            #endregion

            return new ScanGmailCallbackResult(
                new ScanGmailCallbackSummary(beginDate, endDate, endDate.Subtract(beginDate).TotalSeconds,
                    response.Messages.Count, messageSummaries.AsReadOnly()));
        }

        private async Task<Ruolo> GetRuolo(long idCorrGlobaliRuolo)
        {
            var corrGlobaliEntity = await this._pi3DbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == idCorrGlobaliRuolo).FirstOrDefaultAsync();

            return corrGlobaliEntity == null ? null : this._mapper.Map<Ruolo>(corrGlobaliEntity);
        }

        private System.Data.DataSet ConvertToDataset(MailAccountCheckResponse checkResponse)
        {
            DataSet dataSet = new DataSet("checkResponseDataSet");
            DataTable mailProcessedTable = new DataTable("MailProcessedList");

            mailProcessedTable.Columns.Add("Mail ID", typeof(string));
            mailProcessedTable.Columns.Add("Mittente", typeof(string));
            mailProcessedTable.Columns.Add("Oggetto", typeof(string));
            mailProcessedTable.Columns.Add("Data", typeof(string));
            mailProcessedTable.Columns.Add("Allegati", typeof(string));
            mailProcessedTable.Columns.Add("Errori", typeof(string));
            //mailProcessedTable.Columns.Add("system_id", typeof(int));

            dataSet.Tables.Add(mailProcessedTable);
            int sysId = 0;
            string nd = "N.D.";

            foreach (MailProcessed mailProcessed in checkResponse.MailProcessedList)
            {
                DataRow newRow = mailProcessedTable.NewRow();
                //newRow["system_id"] = sysId++;
                newRow["Mail ID"] = mailProcessed.MailID;
                newRow["Mittente"] = mailProcessed.From;
                newRow["Oggetto"] = mailProcessed.Subject;
                newRow["Data"] = mailProcessed.Date.ToString();
                newRow["Allegati"] = mailProcessed.CountAttatchments;
                newRow["Errori"] = mailProcessed.ErrorMessage ?? "OK";
                mailProcessedTable.Rows.Add(newRow);
            }
            return dataSet;
        }


        #endregion

        #region Private Members

        protected readonly ILogger<ScanGmailCallbackHandler> _logger;
        protected readonly IAuthProviderFactoryService _authProviderFactoryService;
        protected readonly IConfiguration _configuration;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IEmailBoxScannerService _emailBoxScannerService;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        protected readonly string _keyTokenName;
        protected readonly IAacFetcherService _aacFetcherService;
        protected readonly IDistributedCache _distributedCache;
        protected readonly AacOptions _options;
        protected readonly string _cacheExpInMinutes;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CorrGlobaliEntity, Ruolo>()
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.VAR_DESC_CORR));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class AacOptions
        {
            public string GrantType { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string Scope { get; set; }
            public string BaseUrl { get; set; }
        }

        protected virtual async Task CreatePi3Identity(
                string instance,
                string codiceAmministrazione,
                string codiceRegistro,
                string userGmailEmail)
        {
            var idAmministrazione = await this._pi3DbContext.AmministraEntities.AsNoTracking()
                           .Where(a => a.VAR_CODICE_AMM!.ToUpper() == codiceAmministrazione.ToUpper())
                           .Select(a => a.SYSTEM_ID)
                           .FirstOrDefaultAsync();

            if (idAmministrazione == 0)
                throw new ScanGmailCallbackPi3Exception(ErrorDescriptions.AmministratzioneNonTrovata, null, codiceAmministrazione);

            var idRegistro = await this._pi3DbContext.RegistroEntities.AsNoTracking()
                            .Where(r => r.VAR_CODICE!.ToUpper() == codiceRegistro.ToUpper())
                            .Select(r => r.SYSTEM_ID)
                            .FirstOrDefaultAsync();

            if (idRegistro == 0)
                throw new ScanGmailCallbackPi3Exception(ErrorDescriptions.RegistroNonTrovato, null, codiceRegistro);

            const string funzioneAutorizzativa = "DO_SCARICO_GMAIL";

            var idTipoFunzioneScaricoGmail = await this._pi3DbContext.FunzioneEntities.AsNoTracking()
                                        .Where(f => f.COD_FUNZIONE!.ToUpper() == funzioneAutorizzativa
                                                && f.ID_AMM == idAmministrazione)
                                        .Select(f => f.ID_TIPO_FUNZIONE)
                                        .FirstOrDefaultAsync();

            if (idTipoFunzioneScaricoGmail == 0)
                throw new ScanGmailCallbackPi3Exception(ErrorDescriptions.FunzioneAutorizzativaNonConfigurata, null, funzioneAutorizzativa);

            const string automaticUserId = "UT_GMAIL";
            if (!await this._pi3DbContext.PeopleEntities.AsNoTracking()
                .Where(p => p.USER_ID!.ToUpper() == automaticUserId)
                .AnyAsync())
            {
                throw new ScanGmailCallbackPi3Exception(ErrorDescriptions.UtenteAutomaticoNonConfigurato, null, automaticUserId);
            }

            var groupCode = await (from cgR in this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                                   join rReg in this._pi3DbContext.RuoloRegistroEntities.AsNoTracking() on cgR.SYSTEM_ID equals rReg.ID_RUOLO_IN_UO
                                   join vis in this._pi3DbContext.VisMailRegistriEntities.AsNoTracking() on rReg.ID_RUOLO_IN_UO equals vis.ID_RUOLO_IN_UO
                                   join tfr in this._pi3DbContext.TipoFRuoloEntities.AsNoTracking() on cgR.SYSTEM_ID equals tfr.ID_RUOLO_IN_UO
                                   where cgR.CHA_TIPO_URP == "R"
                                    && rReg.ID_REGISTRO == vis.ID_REGISTRO
                                    && cgR.CHA_DISABLED_TRASM != "1"
                                    && (vis.VAR_EMAIL_REGISTRO!.ToUpper() == userGmailEmail.ToUpper()
                                        || vis.VAR_EMAIL_REGISTRO == null)
                                    && tfr.ID_TIPO_FUNZ == idTipoFunzioneScaricoGmail
                                    && vis.CHA_NOTIFICA == "1"
                                   select cgR.VAR_COD_RUBRICA)
                      .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(groupCode))
                throw new ScanGmailCallbackPi3Exception(ErrorDescriptions.NessunRuoloAutorizzato, null, userGmailEmail);

            ((ClaimsPrincipalService)this._claimsPrincipalService).Current =
                this._claimsPrincipalService.Current ?? new ClaimsPrincipal();

            var claimsPrincipal = this._claimsPrincipalService.Current;

            claimsPrincipal!.AddIdentity(new ClaimsIdentity(
                authenticationType: "Pi3Authentication"));

            var claimsIdentity = claimsPrincipal.Identities.First(i => i.AuthenticationType == "Pi3Authentication");

            claimsIdentity.AddClaim(new Claim(Pi3ClaimTypes.Instance, instance.ToString()));

            var userContext = await
                         (from p in this._pi3DbContext.PeopleEntities.AsNoTracking()
                          join a in this._pi3DbContext.AmministraEntities.AsNoTracking() on p.ID_AMM equals a.SYSTEM_ID
                          join cg in this._pi3DbContext.CorrGlobaliEntities.AsNoTracking() on p.SYSTEM_ID equals cg.ID_PEOPLE
                          join pg in this._pi3DbContext.PeopleGroupEntities.AsNoTracking() on p.SYSTEM_ID equals pg.PEOPLE_SYSTEM_ID
                          join g in this._pi3DbContext.GroupEntities.AsNoTracking() on pg.GROUPS_SYSTEM_ID equals g.SYSTEM_ID
                          join cg1 in this._pi3DbContext.CorrGlobaliEntities.AsNoTracking() on g.SYSTEM_ID equals cg1.ID_GRUPPO
                          where
                             a!.VAR_CODICE_AMM!.ToUpper() == codiceAmministrazione.ToUpper()
                             && p!.USER_ID!.ToUpper() == automaticUserId.ToUpper()
                             && g!.GROUP_ID!.ToUpper() == groupCode!.ToUpper()
                             && pg.DTA_FINE == null
                          select new
                          {
                              PEOPLE_SYSTEM_ID = p.SYSTEM_ID,
                              PEOPLE_USER_ID = p.USER_ID,
                              PEOPLE_VAR_COGNOME = p.VAR_COGNOME,
                              PEOPLE_VAR_NOME = p.VAR_NOME,
                              PEOPLE_CHA_AMMINISTRATORE = p.CHA_AMMINISTRATORE,
                              AMM_SYSTEM_ID = a.SYSTEM_ID,
                              AMM_VAR_CODICE_AMM = a.VAR_CODICE_AMM,
                              AMM_VAR_DESC_AMM = a.VAR_DESC_AMM,
                              CORRGLOBALI_SYSTEM_ID = cg.SYSTEM_ID,
                              GROUPS_SYSTEM_ID = g.SYSTEM_ID,
                              GROUPS_GROUP_ID = g.GROUP_ID,
                              GROUPS_GROUP_NAME = g.GROUP_NAME,
                              CORRGLOBALI_GROUPS_SYSTEM_ID = cg1.SYSTEM_ID,
                          })
                         .FirstOrDefaultAsync();

            if (userContext == null)
                throw new UnauthorizedPi3Exception(ErrorDescriptions.UtenteNonAutorizzato, ErrorDescriptions.ResourceManager, automaticUserId, groupCode, userGmailEmail);

            claimsIdentity.AddClaim(new Claim(Pi3ClaimTypes.IdUser, userContext.PEOPLE_SYSTEM_ID.ToString()));
            claimsIdentity.AddClaim(new Claim(Pi3ClaimTypes.UserId, userContext.PEOPLE_USER_ID));
            claimsIdentity.AddClaim(new Claim(Pi3ClaimTypes.UserName, userContext.PEOPLE_VAR_NOME));
            claimsIdentity.AddClaim(new Claim(Pi3ClaimTypes.UserSurname, userContext.PEOPLE_VAR_COGNOME));
            claimsIdentity.AddClaim(new Claim(Pi3ClaimTypes.Admin, (userContext.PEOPLE_CHA_AMMINISTRATORE == "1").ToString()));
            claimsIdentity.AddClaim(new Claim(Pi3ClaimTypes.IdGroup, userContext.GROUPS_SYSTEM_ID.ToString()));
            claimsIdentity.AddClaim(new Claim(Pi3ClaimTypes.GroupCode, userContext.GROUPS_GROUP_ID));
            claimsIdentity.AddClaim(new Claim(Pi3ClaimTypes.GroupDescription, userContext.GROUPS_GROUP_NAME));
            claimsIdentity.AddClaim(new Claim(Pi3ClaimTypes.IdTenant, userContext.AMM_SYSTEM_ID.ToString()));
            claimsIdentity.AddClaim(new Claim(Pi3ClaimTypes.TenantCode, userContext.AMM_VAR_CODICE_AMM));
            claimsIdentity.AddClaim(new Claim(Pi3ClaimTypes.TenantDescription, userContext.AMM_VAR_DESC_AMM));
            claimsIdentity.AddClaim(new Claim(Pi3ClaimTypes.Instance, instance));

            claimsIdentity.AddClaim(new Claim("KeyToken", await this.GetAccessToken()));

            claimsIdentity.AddClaims(await (from tfr in this._pi3DbContext.TipoFRuoloEntities.AsNoTracking()
                                            join tf in this._pi3DbContext.TipoFunzioneEntities.AsNoTracking() on tfr.ID_TIPO_FUNZ equals tf.SYSTEM_ID
                                            join f in this._pi3DbContext.FunzioneEntities.AsNoTracking() on tf.SYSTEM_ID equals f.ID_TIPO_FUNZIONE
                                            where tfr.ID_RUOLO_IN_UO == userContext.CORRGLOBALI_GROUPS_SYSTEM_ID
                                            select f.COD_FUNZIONE)
                        .Distinct()
                        .Select(f => new Claim(Pi3ClaimTypes.Authorization, f))
                        .ToListAsync());
        }

        private async Task<Registro?> GetRegistro(string codiceRegistro, string userGmailEmail)
        {
            return await (from r in this._pi3DbContext.RegistroEntities.AsNoTracking()
                          join m in this._pi3DbContext.MailRegistriEntities.AsNoTracking()
                             on r.SYSTEM_ID equals m.ID_REGISTRO
                          where r.VAR_CODICE.ToUpper().Equals(codiceRegistro.ToUpper()) &&
                             m.VAR_EMAIL_REGISTRO.ToUpper().Equals(userGmailEmail.ToUpper())
                          select new DocsPaVO.utente.Registro()
                          {
                              systemId = r.SYSTEM_ID.ToString(),
                              codRegistro = r.VAR_CODICE,
                              chaRF = r.CHA_RF,
                              email = m.VAR_EMAIL_REGISTRO,
                              idAmministrazione = r.ID_AMM.ToString(),
                              codice = r.NUM_RIF.ToString(),
                              idAOOCollegata = r.ID_AOO_COLLEGATA.ToString(),
                          })
                             .FirstOrDefaultAsync();
        }

        private async Task<string> GetAccessToken()
        {
            var parms = new AacRequestBody()
            {
                Scope = this._options.Scope,
                ClientSecret = this._options.ClientSecret,
                ClientId = this._options.ClientId,
                GrantType = this._options.GrantType,
            };
            var aacResponse = await this._aacFetcherService.GetToken(parms);
            var token = aacResponse.AccessToken;
            return !string.IsNullOrEmpty(token) ? $"Bearer {token}" : string.Empty;
        }

        #endregion
    }
}