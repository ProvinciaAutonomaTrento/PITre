// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.addressbook;
using DocsPaVO.amministrazione;
using DocsPaVO.areaConservazione;
using DocsPaVO.documento;
using DocsPaVO.FriendApplication;
using DocsPaVO.Interoperabilita.Segnatura;
using DocsPaVO.rubrica;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Drawing;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.StandardCase;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.RubricaComune;
using Pi3.Core.AggregateModels.CorrispondenteAggregate.Events;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Exceptions;
using Pi3.Core.AggregateModels.UOCorrispondenteAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.Factory;
using Pi3.Core.Services.File.FirmaDigitale2;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Chilkat.Services.Email.Sender;
using Pi3.Infrastructure.Graph.Services.Email.Sender;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Segnatura
{

    // Richiede libreria MediatR
    internal class SegnaturaHandler : IRequestHandler<SegnaturaRequest, SegnaturaResult>
    {
        #region Public Members

        public SegnaturaHandler(ILogger<SegnaturaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IRubricaComuneService rubricaComuneService,
            IHttpContextAccessor httpContextAccessor,
            IDocumentoAmministrativoRepository documentRepository,
            IWebMethodLoggerService webMethodLoggerService,
            IDocumentBlobRepository documentBlobRepository,
            IUOCorrispondenteRepository uOCorrispondenteRepository,
            IFirmaDigitale2Service firmaDigitale2Service,
            IConfigurationService configurationService,
            //IEmailSenderService emailSenderService,
            IFactoryService factoryService,
            IDocumentoAmministrativoRepository documentoAmministrativo)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._rubricaComuneService = rubricaComuneService;
            this._httpContextAccessor = httpContextAccessor;
            this._documentRepository = documentRepository;
            this._documentBlobRepository = documentBlobRepository;
            this._uoCorrispondenteRepository = uOCorrispondenteRepository;
            this._configurationService = configurationService;
            this._firmaDigitale2Service = firmaDigitale2Service;
            this._documentoAmministrativoRepository = documentoAmministrativo;
            this._webMethodLoggerService = webMethodLoggerService;
            //this._emailSenderService = emailSenderService;
            this._factoryService = factoryService;


            this.InitializeMapper();
        }


        public async Task<SegnaturaResult> Handle(SegnaturaRequest request, CancellationToken cancellationToken)
        {
            ProcessorOutput output = new ProcessorOutput();

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

            var ruoloEntity = await this._dbContext.CorrGlobaliEntities.FirstAsync(x => x.ID_GRUPPO == idGroup);
            var idRole = ruoloEntity?.SYSTEM_ID;

            var docnumber = string.Empty;

            var email = request.email;
            var message = request.message;
            var emailAddress = request.emailAddress;
            bool fatturaElDaPEC = false;

            string num_reg_mit = "";

            //Alcune integrazioni mettono nel tag descrizione della segnatura il nome del file
            bool searchAttachSegnaturabyDescription = false;

            MemoryStream memStream = null;
            StreamReader reader = null;

            var salvataggioMail = request.salvataggioMail;

            try
            {
                System.IO.FileStream fs = null;
                //Andrea De Marco - Aggiunto File Stream per inserimento allegato Segnatura.xml 
                System.IO.FileStream fsAllSegnatura = null;
                //End Andrea De Marco
                bool daAggiornareUffRef = false;
                //TO DO validazione segnatura 
                //TO DO declassamento a mail se segnatura vecchia - da fare nel validatore - ritornare anche searchAttachSegnaturabyDescription
                bool controlloBloccante = false;
                bool generataEccezione = false;
                bool protocolla_con_segnatura = false;
                string validaSegnErrorMessage = string.Empty;

                //creazione del doc con trattamento spazi bianchi
                SegnaturaInformaticaType segnatura = null;
                memStream = new MemoryStream(message.AttachmentContent);
                reader = new StreamReader(memStream);
                string xmlSegnatura = reader.ReadToEnd().Replace("xmlns=\"http://www.agid.gov.it/protocollo/pec/\"", "xmlns=\"http://www.agid.gov.it/protocollo/\"");

                var eccezioneBloccante = await this._configurationService.GetValue<string>("BE_ECCEZIONE_BLOCCANTE");
                var proseguiSigilloMancante = await this._configurationService.GetValue<string>("BE_NO_INVIO_ECC_SIGILLO_MANCANTE");
                var checkMailNotReadBase64 = await this._configurationService.GetValue<string>("BE_CHECK_MAIL_NOT_READ_BASE_64");
                ValidaSegnatura validator = new ValidaSegnatura(xmlSegnatura, message, email, this._dbContext, this._firmaDigitale2Service, eccezioneBloccante, proseguiSigilloMancante, checkMailNotReadBase64, out validaSegnErrorMessage, out searchAttachSegnaturabyDescription);

                if (validator.eccezioneXml != null)
                {
                    //Il bool è utilizzato come parametro opzionale per indicare il verificarsi dell'eccezione
                    //è utilizzato per impostare schedaDoc.interop = E
                    generataEccezione = true;
                    string numRegMitt = validator.numRegMitt;
                    num_reg_mit = numRegMitt;
                    //Andrea De Marco - Gestione Eccezione Segnatura.xml Controlli non Bloccanti - per ripristino commentare De Marco e decommentare il resto
                    if (validator.controlloBloccante)
                    {
                        controlloBloccante = true;
                        this._logger.LogDebug($"Invio Eccezione.xml, per il documento protocollato nr {numRegMitt} a {email.Sender.Address}");
                        await this.SendNotificaEccezione(request.reg, validator.eccezioneXml, numRegMitt, email.Sender.Address);
                        //msg_mail_elaborata = true;
                        //protocolla_con_segnatura = false;
                    }
                    else
                    {
                        controlloBloccante = false;
                        //not_ecc = validator.eccezioneXml;
                        protocolla_con_segnatura = false;
                    }

                }

                if (!controlloBloccante)
                {
                    if (generataEccezione)
                        output = (await this._mediator.Send(new StandardCaseRequest(
                                message.Id,
                                email,
                                request.reg,
                                request.emailAddress,
                                generataEccezione,
                                request.isPec,
                                request.salvataggioMail
                                ))).output;
                    else
                    {
                        segnatura = DeserializeObject<SegnaturaInformaticaType>(xmlSegnatura);
                        output = await this.EseguiSegnaturaAllegato6(segnatura, message, email, request.reg, searchAttachSegnaturabyDescription, emailAddress, idTenant, ruoloEntity, request.isPec, idPeople, salvataggioMail);
                    }
                }

                //if (output != null && !string.IsNullOrEmpty(validaSegnErrorMessage))
                //    output.ErrorMessage += !string.IsNullOrEmpty(output.ErrorMessage) ? $"{output.ErrorMessage}; {validaSegnErrorMessage}" : validaSegnErrorMessage;

                if (output != null && string.IsNullOrEmpty(output.ErrorMessage) && !string.IsNullOrEmpty(validaSegnErrorMessage))
                    output.ErrorMessage = validaSegnErrorMessage;

                if (output != null && output.Success == true && (output.DocNumber != null || output.DocNumber != 0))
                    docnumber = output.DocNumber.ToString();

                else if (!string.IsNullOrEmpty(output.ErrorMessage))
                {
                    if (output.ErrorMessage.Contains("CODINTEROP1"))
                    {
                        if (output.DocNumber != null)
                        {
                            docnumber = output.DocNumber.ToString();
                            if (fatturaElDaPEC)
                                await this.FattElDaPEC(output.DocNumber.ToString());
                        }
                        if (await this.MailElaborata(message.Id, "E", request.reg.systemId.AsLong(), docnumber.AsLong(), this._mailOrigine))
                            _logger.LogDebug(Resources.MailElaborata);
                        else
                            _logger.LogDebug(Resources.MailNonElaborata);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                output = new ProcessorOutput
                {
                    Success = false,
                    DocNumber = null,
                    ErrorMessage = ex.Message,
                    ProcessedAttachments = 0
                };

                // Se il predisposto è stato creato lo devo cestinare
                if (!string.IsNullOrWhiteSpace(docnumber))
                    await this.TrashDocument(docnumber.AsLong());
            }
            finally
            {
                reader?.Close();
                memStream?.Close();
            }

            return new SegnaturaResult(output);
        }

        private async Task SendNotificaEccezione(DocsPaVO.utente.Registro reg, string xmlString, string numRegMitt, string mailMitt)
        {
            bool esito = true;  //presume successo
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idRegistro = reg.systemId.AsLong();

            var infoRegEntity = await _dbContext.MailRegistriEntities
                     .Join(_dbContext.RegistroEntities,
                         m => m.ID_REGISTRO,
                         r => r.SYSTEM_ID,
                         (m, r) => new { m, r })
                     .Join(_dbContext.AmministraEntities,
                         j => j.r.ID_AMM,
                         a => a.SYSTEM_ID,
                         (j, a) => new { j.m, j.r, a })
                     .AsNoTracking()
                     .Where(j => j.m.ID_REGISTRO == idRegistro
                         && j.m.VAR_EMAIL_REGISTRO.Equals(reg.email))
                     .Select(j => new InfoRegEntity
                     {
                         SYSTEM_ID = j.r.SYSTEM_ID,
                         VAR_CODICE = j.r.VAR_CODICE,
                         ID_AMM = j.r.ID_AMM,
                         VAR_USER_MAIL = j.m.VAR_USER_MAIL,
                         VAR_PWD_MAIL = j.m.VAR_PWD_MAIL,
                         VAR_SERVER_SMTP = j.m.VAR_SERVER_SMTP,
                         NUM_PORTA_SMTP = j.m.NUM_PORTA_SMTP,
                         VAR_EMAIL_REGISTRO = j.m.VAR_EMAIL_REGISTRO,
                         VAR_CODICE_AMM = j.a.VAR_CODICE_AMM,
                         VAR_USER_SMTP = j.m.VAR_USER_SMTP,
                         CHA_STR_SEGNATURA = j.a.CHA_STR_SEGNATURA,
                         VAR_PWD_SMTP = j.m.VAR_PWD_SMTP,
                         CHA_POP_SSL = j.m.CHA_POP_SSL,
                         CHA_SMTP_SSL = j.m.CHA_SMTP_SSL,
                         CHA_SMTP_STA = j.m.CHA_SMTP_STA,
                         VAR_SERVER_IMAP = j.m.VAR_SERVER_IMAP,
                         NUM_PORTA_IMAP = j.m.NUM_PORTA_IMAP,
                         VAR_TIPO_CONNESSIONE = j.m.VAR_TIPO_CONNESSIONE,
                         VAR_INBOX_IMAP = j.m.VAR_INBOX_IMAP,
                         VAR_BOX_MAIL_ELABORATE = j.m.VAR_BOX_MAIL_ELABORATE,
                         VAR_MAIL_NON_ELABORATE = j.m.VAR_MAIL_NON_ELABORATE,
                         CHA_IMAP_SSL = j.m.CHA_IMAP_SSL,
                         VAR_SOLO_MAIL_PEC = j.m.VAR_SOLO_MAIL_PEC,
                         NUM_PORTA_POP = j.m.NUM_PORTA_POP,
                         VAR_SERVER_POP = j.m.VAR_SERVER_POP,
                         PROVIDER_ID = j.m.PROVIDER_ID,
                         MS_TENANT_ID = j.m.MS_TENANT_ID,
                         MS_CLIENT_ID = j.m.MS_CLIENT_ID,
                         MS_CLIENT_SEC = j.m.MS_CLIENT_SEC,
                         MS_FOLD_TO_READ = j.m.MS_FOLD_TO_READ
                     })
                     .FirstOrDefaultAsync();

            //Invio della mail
            var instructions = new SendEmailInstructions
            {
                Sender = new EmailSender
                {
                    Address = infoRegEntity.VAR_EMAIL_REGISTRO
                },
                To = new List<EmailRecipient>
                {
                    new EmailRecipient
                    {
                        Address = mailMitt
                    }
                },
                Subject = new Core.SeedWork.TextValue(Resources.EccezioneMailSubject),
                Body = new Core.SeedWork.TextValue(string.Format(Resources.EccezioneMailObject, numRegMitt)),
                BodyIsHtml = true,
                Attachments = new List<EmailContentAttachment>()
                {
                    new EmailContentAttachment()
                    {
                         Content = System.Text.Encoding.UTF8.GetBytes(xmlString),
                         ContentType = "text/xml",
                         FileName = Resources.FileNameEccezione
                    }
                },
            };

            var provider = await this._dbContext.AssProviderLibEntities
                .Where(x => x.PROVIDER_ID == infoRegEntity.PROVIDER_ID)
                .Select(x => x.LIB)
                .FirstOrDefaultAsync(); 

            var creation = await _factoryService.TryCreate<IEmailSenderService>(s => s.Provider == provider);

            if (!creation.Success)
                throw new ProviderNotFoundPi3Exception(String.Format(ErrorDescriptions.ProviderNotFound, provider));

            var result = await creation.Service.SendEmail(
                (configurations) =>
                {
                    switch (configurations)
                    {
                        case ChilkatSendEmailConfiguration chilkatEmailBoxConfigurations:
                            this.LoadChilkatSendEmailConfigurations((ChilkatSendEmailConfiguration)configurations, infoRegEntity);
                            break;
                        case GraphSendEmailConfiguration graphSendEmailConfigurations:
                            this.LoadGraphSendEmailConfigurations((GraphSendEmailConfiguration)configurations, infoRegEntity);
                            break;
                        default:
                            throw new SendMailPi3Exception(ErrorDescriptions.ProviderNonGestito);

                    }
                },
                instructions
                );
        }

        private void LoadGraphSendEmailConfigurations(GraphSendEmailConfiguration configurations, InfoRegEntity infoRegEntity)
        {
            configurations.TenantId = infoRegEntity.MS_TENANT_ID;
            configurations.ClientId = infoRegEntity.MS_CLIENT_ID;
            configurations.ClientSecret = infoRegEntity.MS_CLIENT_SEC;
            configurations.MailBox = infoRegEntity.VAR_EMAIL_REGISTRO;
        }

        private void LoadChilkatSendEmailConfigurations(ChilkatSendEmailConfiguration configurations, InfoRegEntity infoRegEntity)
        {
            configurations.Host = infoRegEntity.VAR_SERVER_SMTP;
            configurations.Port = infoRegEntity.NUM_PORTA_SMTP.HasValue ? Convert.ToInt32(infoRegEntity.NUM_PORTA_SMTP) : 0;
            configurations.StartTLS = !string.IsNullOrEmpty(infoRegEntity.CHA_SMTP_STA) && infoRegEntity.CHA_SMTP_STA == "1";
            configurations.RequireSsl = infoRegEntity.CHA_SMTP_SSL == "1";
            configurations.UserName = infoRegEntity.VAR_USER_SMTP;
            configurations.Password = Crypter.Decode(infoRegEntity.VAR_PWD_SMTP, infoRegEntity.VAR_USER_SMTP);
        }

        private async Task<ProcessorOutput> EseguiSegnaturaAllegato6(SegnaturaInformaticaType segnatura, AnalyzedMessage message, Core.Services.Email.BoxScanner.Email email, DocsPaVO.utente.Registro reg, bool searchAttachByDescription, string mailAddress, long idTenant, CorrGlobaliEntity? ruoloEntity, bool isPec, long idPeople, string salvataggioMail)
        {
            DocsPaVO.documento.SchedaDocumento sd = null;
            System.IO.FileStream fsAll = null;
            bool codint1 = false;
            string docnumber = string.Empty;
            string err = string.Empty;
            var success = true;
            var mailId = message.Id;
            IntestazioneType intestazione = segnatura.Intestazione;
            IdentificatoreType identificatore = intestazione.Identificatore;
            DocsPaVO.utente.Ruolo ruolo = this._mapper.Map<Ruolo>(ruoloEntity);
            int processedAttachments = 0;

            try
            {
                var registroEntity = await this._dbContext.RegistroEntities.AsNoTracking().FirstAsync(x => x.SYSTEM_ID == reg.systemId.AsLong());

                string mailOrigine = SegnaturaManager.GetIndirizzoTelematicoMittente(segnatura);

                if (string.IsNullOrEmpty(mailOrigine))
                    mailOrigine = email.Sender.Address;

                this._mailOrigine = mailOrigine;

                string codiceAmministrazione = identificatore.CodiceAmministrazione.Value.Trim();
                //Il campo di DB ha una lunghezza massima di 16 caratteri - CodiceAmministrazione non può essere più lungo di questo valore
                if (codiceAmministrazione.Length > 16)
                    codiceAmministrazione = codiceAmministrazione.Substring(0, 16);

                string codiceAOO = identificatore.CodiceAOO.Value.Trim();
                //Il campo di DB ha una lunghezza massima di 16 caratteri - CodiceAOO non può essere più lungo di questo valore
                if (codiceAOO.Length > 16)
                    codiceAOO = codiceAOO.Substring(0, 16);

                string numeroRegistrazione = identificatore.NumeroRegistrazione.Trim();
                string dataRegistrazione = identificatore.DataRegistrazione.ToString("dd/MM/yyyy");
                bool confermaRic = false;

                if (dataRegistrazione == null)
                {
                    _logger.LogDebug(Resources.DataRegistrazioneNonValida);
                    err = Resources.DataRegistrazioneNonValida;

                    if (await this.MailElaborata(mailId, "U"))
                        _logger.LogDebug(Resources.SospensioneEseguita);
                    else
                        _logger.LogDebug(Resources.SospensioneNonEseguita);

                    return new ProcessorOutput()
                    {
                        Success = false,
                        DocNumber = null,
                        ErrorMessage = err,
                        ProcessedAttachments = 0
                    };
                }

                string oggetto = intestazione.Oggetto.Trim();

                //MITTENTE
                string rows = "";
                DocsPaVO.addressbook.TipoUtente tipoMittente;
                if (codiceAmministrazione.Equals(reg.codAmministrazione))
                    tipoMittente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                else
                    tipoMittente = DocsPaVO.addressbook.TipoUtente.ESTERNO;

                //Su PiTRE la chiave RICERCA_COD non c'è, quindi uso il metodo che viene usato a chiave spenta

                (DocsPaVO.utente.Corrispondente mittente, rows) = await this.GetMittente(segnatura.Descrizione.Mittente, mailOrigine, codiceAmministrazione, codiceAOO, tipoMittente, reg, rows);

                if (mittente == null)
                {
                    //se il mittente è interno, allora ci si blocca!!!!!
                    if (tipoMittente == DocsPaVO.addressbook.TipoUtente.INTERNO)
                    {
                        //_logger.LogDebug("La mail viene sospesa: mittente non trovato, perchè il tipo mittente è interno");
                        _logger.LogDebug(Resources.MittenteNonValido);
                        err = Resources.MittenteNonValido;
                        if (await this.MailElaborata(mailId, "U"))
                            _logger.LogDebug(Resources.SospensioneEseguita);
                        else
                            _logger.LogDebug(Resources.SospensioneNonEseguita);

                        return new ProcessorOutput()
                        {
                            Success = false,
                            DocNumber = null,
                            ErrorMessage = err,
                            ProcessedAttachments = 0
                        };
                    }
                    else
                    {
                        mittente = await this.AddNewCorrispondente(segnatura.Descrizione.Mittente, mailOrigine, identificatore, reg);

                        //AGGIORNAMENTO CANALE PREFERENZIALE
                        await this.UpdateCanalePref(mittente);

                        //inserisco la mail associata al corrispondente esterno in DPA_MAIL_CORR_ESTERNI
                        List<DocsPaVO.utente.MailCorrispondente> casella = new List<DocsPaVO.utente.MailCorrispondente>();
                        casella.Add(new DocsPaVO.utente.MailCorrispondente()
                        {
                            Email = mittente.email,
                            Note = "",
                            Principale = "1"
                        });

                        bool inserted = await this.InsertMailCorrispondente(casella, mittente.systemId.AsLong());
                    }
                }

                //DESTINATARI
                (string infoDestinatari, confermaRic) = await this.GetInfoDestinatari(segnatura.Descrizione.Destinatario, confermaRic, mailAddress);

                bool tipoRiferimentoMIME = true;
                //DOCUMENTO PRINCIPALE
                string docPrincipaleName = "";
                string docPrincipaleDescrizione = "";

                if (segnatura.Descrizione.DocumentoPrimario != null && !string.IsNullOrEmpty(segnatura.Descrizione.DocumentoPrimario.mimeType))
                    tipoRiferimentoMIME = true;

                if (!tipoRiferimentoMIME)
                {
                    _logger.LogDebug(Resources.TipoRifNonValido);
                    err = Resources.TipoRifNonValido;

                    if (await this.MailElaborata(mailId, "U"))
                        _logger.LogDebug(Resources.SospensioneEseguita);
                    else
                        _logger.LogDebug(Resources.SospensioneNonEseguita);

                    return new ProcessorOutput()
                    {
                        Success = false,
                        DocNumber = null,
                        ErrorMessage = err,
                        ProcessedAttachments = 0
                    };
                }
                if (segnatura.Descrizione.DocumentoPrimario == null || string.IsNullOrEmpty(segnatura.Descrizione.DocumentoPrimario.nomeFile))
                {
                    _logger.LogDebug(Resources.NomeDocPrincipaleNonPresente);
                    err = Resources.NomeDocPrincipaleNonPresente;
                    if (await this.MailElaborata(mailId, "U"))
                        _logger.LogDebug(Resources.SospensioneEseguita);
                    else
                        _logger.LogDebug(Resources.SospensioneNonEseguita);

                    return new ProcessorOutput()
                    {
                        Success = false,
                        DocNumber = null,
                        ErrorMessage = err,
                        ProcessedAttachments = 0
                    };
                }
                if (tipoRiferimentoMIME)
                {
                    docPrincipaleName = segnatura.Descrizione.DocumentoPrimario.nomeFile;
                    docPrincipaleDescrizione = segnatura.Descrizione.DocumentoPrimario.Descrizione;
                }

                //CONTROLLO DI CONSISTENZA DEI NOMI DEI DOCUMENTI
                //Documento principale
                if (docPrincipaleName != null && !docPrincipaleName.Equals(""))
                {
                    var attachDocPrincipaleByName = email.Attachments.Where(x => x.FileName.Equals(docPrincipaleName)).FirstOrDefault();
                    var attachDocPrincipaleByDescrizione = email.Attachments.Where(x => x.FileName.Equals(docPrincipaleDescrizione)).FirstOrDefault();
                    if (attachDocPrincipaleByName == null && (!searchAttachByDescription || docPrincipaleDescrizione == null))
                    {
                        _logger.LogDebug(Resources.DocPrincipaleNonPresente);
                        err = Resources.DocPrincipaleNonPresente;
                        if (await this.MailElaborata(mailId, "U"))
                            _logger.LogDebug(Resources.SospensioneEseguita);
                        else
                            _logger.LogDebug(Resources.SospensioneNonEseguita);

                        return new ProcessorOutput()
                        {
                            Success = false,
                            DocNumber = null,
                            ErrorMessage = err,
                            ProcessedAttachments = 0
                        };
                    }

                    if (await this.GetApp(docPrincipaleName) == null)
                    {
                        _logger.LogDebug(Resources.FormatoFileDocPrincipaleNonGestito);
                        err = Resources.FormatoFileDocPrincipaleNonGestito;
                        if (await this.MailElaborata(mailId, "U"))
                            _logger.LogDebug(Resources.SospensioneEseguita);
                        else
                            _logger.LogDebug(Resources.SospensioneNonEseguita);

                        return new ProcessorOutput()
                        {
                            Success = false,
                            DocNumber = null,
                            ErrorMessage = err,
                            ProcessedAttachments = 0
                        };
                    }
                }

                //Allegati
                if (segnatura.Descrizione.Allegato != null)
                {
                    for (int ind = 0; ind < segnatura.Descrizione.Allegato.Count(); ind++)
                    {
                        // SA E SF Se tipoRiferimento è null o è valorizzato con "MIME", si deve procedere con l'analisi 
                        if (!string.IsNullOrEmpty(segnatura.Descrizione.Allegato[ind].mimeType))
                        {
                            //si verifica se e' specificato il nome dell'allegato
                            if (string.IsNullOrEmpty(segnatura.Descrizione.Allegato[ind].nomeFile))
                            {
                                _logger.LogDebug(String.Format(Resources.NomeAllegatoNonPresente, ind));
                                err = String.Format(Resources.NomeAllegatoNonPresente, ind);
                                if (await this.MailElaborata(mailId, "U"))
                                    _logger.LogDebug(Resources.SospensioneEseguita);
                                else
                                    _logger.LogDebug(Resources.SospensioneNonEseguita);

                                return new ProcessorOutput()
                                {
                                    Success = false,
                                    DocNumber = null,
                                    ErrorMessage = err,
                                    ProcessedAttachments = 0
                                };
                            }
                            string nome = segnatura.Descrizione.Allegato[ind].nomeFile;
                            string descrizioneAllegato = segnatura.Descrizione.Allegato[ind].Descrizione;
                            var attachAllegatoByName = email.Attachments.Where(x => x.FileName.Equals(nome)).FirstOrDefault(); //Funzionerà????
                            var attachAllegatoByDescrizione = email.Attachments.Where(x => x.FileName.Equals(descrizioneAllegato)).FirstOrDefault(); //Funzionerà????
                            if (attachAllegatoByName == null && (!searchAttachByDescription || attachAllegatoByDescrizione == null))
                            {
                                _logger.LogDebug(String.Format(Resources.AllegatoNonPresente, nome));
                                err = String.Format(Resources.AllegatoNonPresente, nome);

                                if (await this.MailElaborata(mailId, "U"))
                                    _logger.LogDebug(Resources.SospensioneEseguita);
                                else
                                    _logger.LogDebug(Resources.SospensioneNonEseguita);

                                return new ProcessorOutput()
                                {
                                    Success = false,
                                    DocNumber = null,
                                    ErrorMessage = err,
                                    ProcessedAttachments = 0
                                };
                            };
                            if (await this.GetApp(nome) == null)
                            {
                                _logger.LogDebug(String.Format(Resources.FormatoFileAllegatoNonGestito, nome));
                                err = String.Format(Resources.FormatoFileAllegatoNonGestito, nome);
                                if (await this.MailElaborata(mailId, "U"))
                                    _logger.LogDebug(Resources.SospensioneEseguita);
                                else
                                    _logger.LogDebug(Resources.SospensioneNonEseguita);

                                return new ProcessorOutput()
                                {
                                    Success = false,
                                    DocNumber = null,
                                    ErrorMessage = err,
                                    ProcessedAttachments = 0
                                };
                            }
                        }
                    }
                }

                //fatti i controlli, si procede con la protocollazione
                sd = new DocsPaVO.documento.SchedaDocumento();

                if (!string.IsNullOrEmpty(docPrincipaleName))
                    sd.appId = (await this.GetApp(docPrincipaleName)).application;
                //sd.idPeople = infoUtente.idPeople;
                //sd.userId = infoUtente.userId;
                DocsPaVO.documento.Oggetto ogg = new DocsPaVO.documento.Oggetto();
                ogg.descrizione = oggetto;
                sd.oggetto = ogg;
                sd.predisponiProtocollazione = true;

                sd.registro = await this.CaricaRegistroInScheda(reg);

                var documentTypeId = await this._dbContext.DocumentTypesEntities
                    .Where(x => x.TYPE_ID == Resources.Interoperabilita)
                    .Select(x => x.SYSTEM_ID).FirstOrDefaultAsync();
                sd.tipoProto = Resources.EtichettaProtoArrivo;
                sd.typeId = Resources.Interoperabilita;
                sd.mezzoSpedizione = documentTypeId.ToString();
                sd.descMezzoSpedizione = Resources.Interoperabilita;
                sd.interop = Resources.TipoInterop;

                DocsPaVO.documento.ProtocolloEntrata protEntr = new DocsPaVO.documento.ProtocolloEntrata();
                protEntr.mittente = mittente;
                protEntr.dataProtocolloMittente = dataRegistrazione;
                //OLD protEntr.descrizioneProtocolloMittente = codiceAOO + await this.GetSepSegnatura(reg.idAmministrazione.AsLong()) + numeroRegistrazione;
                protEntr.descrizioneProtocolloMittente = identificatore.CodiceRegistro + await this.GetSepSegnatura(reg.idAmministrazione.AsLong()) + numeroRegistrazione;
                if (confermaRic)
                    protEntr.invioConferma = "1";

                sd.protocollo = protEntr;

                //dati utente/ruolo/Uo del creatore. - SERVE?????
                //sd.protocollatore = new DocsPaVO.documento.Protocollatore(infoUtente, ruolo);

                if (await this.MantieniMailRicevutePendenti(reg.systemId.AsLong(), mailAddress))
                    sd.privato = "1";

                //sd = (await this._mediator.Send(new Requests.DocumentoAddDocGrigia(sd, new InfoUtente(), ruolo))).output;
                sd = await AddDocGrigia(sd, idTenant.ToString(), ruolo);

                docnumber = sd.docNumber;

                //modifica
                sd.documento_da_pec = isPec ? "1" : "0";

                ((DocsPaVO.documento.Documento)sd.documenti[0]).dataArrivo = email.DeliveryDate.ToString("dd/MM/yyyy HH:mm:ss");

                sd = (await this._mediator.Send(new Requests.DocumentoSaveDocumento(ruolo, new InfoUtente(), sd, false))).output;

                /* Update per sd.interop e sd.documento_da_pec */
                long sysIdAsLong = sd.systemId.AsLong();
                var profileEntity = await this._dbContext.ProfileEntities.Where(x => x.SYSTEM_ID == sysIdAsLong).FirstOrDefaultAsync();
                if (profileEntity != null)
                {
                    profileEntity.CHA_INTEROP = !string.IsNullOrEmpty(sd.interop) ? sd.interop : Resources.TipoInterop;
                    profileEntity.CHA_DOCUMENTO_DA_PEC = !string.IsNullOrEmpty(sd.documento_da_pec) ? sd.documento_da_pec : (isPec ? "1" : "0");
                    profileEntity.CHA_INVIO_CONFERMA = sd.protocollo.invioConferma == "1" ? "1" : "0";
                    ((DbContext)_dbContext).Update(profileEntity);
                    await ((DbContext)_dbContext).SaveChangesAsync();
                }

                //Mezzo spedizione
                bool mezzoSpedizioneCollegato = (await this._mediator.Send(new Requests.collegaMezzoSpedizioneDocumento(new InfoUtente(), documentTypeId.ToString(), sd.systemId))).output;

                //MULTI CASELLA: associazione documento mailAddress(necessario per l'invio della conferma di ricezione/ annullamento) 
                if (await this.InsertAssDocAddress(sd.systemId.AsLong(), reg.systemId.AsLong(), mailAddress))
                    _logger.LogDebug(Resources.InsertAssDocAddressLogOK);
                else
                    _logger.LogError(Resources.InsertAssDocAddressLogKO);

                //Upload file principale
                if (!string.IsNullOrEmpty(docPrincipaleName))
                {
                    DocsPaVO.documento.FileDocumento fd = new DocsPaVO.documento.FileDocumento();
                    var attachDocPrincipaleByName = email.Attachments.Where(x => x.FileName.Equals(docPrincipaleName)).FirstOrDefault(); //Funzionerà????
                    var attachDocPrincipaleByDescrizione = email.Attachments.Where(x => x.FileName.Equals(docPrincipaleDescrizione)).FirstOrDefault(); //Funzionerà????
                    if (attachDocPrincipaleByName == null && searchAttachByDescription)
                        fd.content = attachDocPrincipaleByDescrizione.Content;
                    else
                        fd.content = attachDocPrincipaleByName.Content;

                    fd.length = fd.content?.Length ?? 0;
                    fd.name = docPrincipaleName;

                    DocsPaVO.documento.FileRequest fRSch = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                    fRSch.fileName = docPrincipaleName;

                    if (fd.content.Length > 0)
                    {
                        var putFileResult = await this._mediator.Send(new Requests.DocumentoPutFileNoException(fRSch, fd, new InfoUtente()));
                        var putFileOk = putFileResult.output;
                        err = putFileResult.errorMessage;
                        if (!putFileOk)
                            throw new UploadFilePi3Exception(Resources.ErroreFilePrincipale);
                        else
                        {
                            bool tsrOk = await this.MatchTSR(email, fRSch, fd, idPeople);
                            await _mediator.Send(new Requests.ExtractXmlSuapRequest(sd, docPrincipaleName, fd.content));
                        }
                    }
                }
                //Upload allegati
                int countAllegati = 0;
                if (segnatura.Descrizione.Allegato != null)
                {
                    for (int i = 0; i < segnatura.Descrizione.Allegato.Count(); i++)
                    {
                        countAllegati = countAllegati + 1;
                        //estrazione dati dell'allegato
                        DocumentoType documentoAllegato = segnatura.Descrizione.Allegato[i];
                        if (!string.IsNullOrEmpty(documentoAllegato.nomeFile))
                        {
                            string nomeAllegato = documentoAllegato.nomeFile;
                            string descrizioneAllegato = documentoAllegato.Descrizione;
                            //TO DO dopo - BOUNCYCASTLE
                            /*
                            if (await this.FindTSRMatch(email.Attachments, nomeAllegato))
                                continue; //il TSR fa parte di un o dei doc, salto l'acquisizione e l'aggiunta dell'allegato
                            */
                            DocsPaVO.documento.Allegato all = new DocsPaVO.documento.Allegato();
                            all.docNumber = sd.docNumber;
                            all.fileName = GetFileName(nomeAllegato);
                            all.position = countAllegati;
                            all.descrizione = String.Format(Resources.AllegatoDescrizione, i);
                            if (!String.IsNullOrEmpty(all.fileName))
                                all.descrizione = all.fileName;

                            all.version = "0";
                            DocsPaVO.documento.Allegato res = (await this._mediator.Send(new Requests.DocumentoAggiungiAllegato(new InfoUtente(), all))).output;

                            if (res == null)
                            {
                                err = string.Format(Resources.ErroreAggiuntaAllegato, Convert.ToString(i + 1));
                                throw new AddAttachmentPi3Exception(String.Format(ErrorDescriptions.AddAttachmentError, Convert.ToString(i + 1), err));
                            }

                            DocsPaVO.documento.FileDocumento fdAll = new DocsPaVO.documento.FileDocumento();
                            var attachAllegatoByName = email.Attachments.Where(x => x.FileName.Equals(nomeAllegato)).FirstOrDefault(); //Funzionerà????
                            var attachAllegatoByDescrizione = email.Attachments.Where(x => x.FileName.Equals(descrizioneAllegato)).FirstOrDefault(); //Funzionerà????
                            if (attachAllegatoByName == null && searchAttachByDescription)
                                fdAll.content = attachAllegatoByDescrizione.Content;
                            else
                                fdAll.content = attachAllegatoByName.Content;

                            fdAll.length = fdAll.content.Length;
                            fdAll.name = nomeAllegato;
                            DocsPaVO.documento.FileRequest fRAll = (DocsPaVO.documento.FileRequest)sd.documenti[0];
                            fRAll = (DocsPaVO.documento.FileRequest)all;

                            fRAll.fileName = nomeAllegato;

                            if (fdAll.content.Length > 0)
                            {
                                var putFileResult = await this._mediator.Send(new Requests.DocumentoPutFileNoException(fRAll, fdAll, new InfoUtente()));
                                var putFileOk = putFileResult.output;
                                err = putFileResult.errorMessage;
                                if (!putFileOk)
                                    throw new UploadFilePi3Exception(Resources.ErroreFileAllegato);
                                else
                                {
                                    bool tsrOk = await this.MatchTSR(email, fRAll, fdAll, idPeople);
                                    await _mediator.Send(new Requests.ExtractXmlSuapRequest(sd, nomeAllegato, fdAll.content));

                                    processedAttachments++;
                                }
                            }
                        }
                    }
                }
                //Inserimento in allegati di segnatura.xml
                //estrazione dati dell'allegato
                countAllegati = countAllegati + 1;
                string nomeAllegato_Segnatura = Resources.NomeFileSegnatura;
                DocsPaVO.documento.Allegato all_Segnatura = new DocsPaVO.documento.Allegato();
                all_Segnatura.descrizione = Resources.DescrizioneAllegatoSegnatura;
                all_Segnatura.docNumber = sd.docNumber;
                all_Segnatura.fileName = GetFileName(nomeAllegato_Segnatura);
                all_Segnatura.version = "0";
                all_Segnatura.position = countAllegati;
                DocsPaVO.documento.Allegato res2 = (await this._mediator.Send(new Requests.DocumentoAggiungiAllegato(new InfoUtente(), all_Segnatura))).output;
                if (res2 == null)
                {
                    err = Resources.ErroreAggiuntaAllSegnatura;
                    //throw new Exception(err);
                    throw new AddAttachmentPi3Exception(String.Format(ErrorDescriptions.AddAttachSegnaturaError, err));
                }

                DocsPaVO.documento.FileDocumento fdAllSegnatura = new DocsPaVO.documento.FileDocumento();
                fdAllSegnatura.content = email.Attachments.Where(x => x.FileName.ToLower().Equals(Resources.NomeFileSegnatura)).Select(x => x.Content).FirstOrDefault();
                fdAllSegnatura.length = fdAllSegnatura.content.Length;
                fdAllSegnatura.name = nomeAllegato_Segnatura;
                DocsPaVO.documento.FileRequest fRAllSegnatura = new DocsPaVO.documento.FileRequest();
                fRAllSegnatura = (DocsPaVO.documento.FileRequest)all_Segnatura;

                fRAllSegnatura.fileName = nomeAllegato_Segnatura;

                if (fdAllSegnatura.content.Length > 0)
                {
                    var putFileResult = await this._mediator.Send(new Requests.DocumentoPutFileNoException(fRAllSegnatura, fdAllSegnatura, new InfoUtente()));
                    var putFileOk = putFileResult.output;
                    err = putFileResult.errorMessage;
                    if (!putFileOk)
                        throw new UploadFilePi3Exception(Resources.ErroreFileAllegato);
                    else
                    {
                        bool tsrOk = await this.MatchTSR(email, fRAllSegnatura, fdAllSegnatura, idPeople);
                        processedAttachments++;
                        //tipologia ENTESUAP ma sembra che non sia utilizzata 
                        //ParseExtraXmlFiles(sd, docPrincipaleName, fd.content, ruolo, idTenant);
                    }
                }

                //Inserimento allegato mail
                if (!string.IsNullOrEmpty(salvataggioMail) && salvataggioMail.Equals("1") && email.BinaryContent != null)
                {
                    string nomeMail = System.Guid.NewGuid().ToString().Substring(0, 25) + ".eml";
                    DocsPaVO.documento.Allegato allMail = new DocsPaVO.documento.Allegato();
                    allMail.descrizione = Resources.DescrizioneAllegatoMailRicevuta;
                    allMail.docNumber = sd.docNumber;
                    allMail.fileName = GetFileName(nomeMail);
                    allMail.version = "0";
                    allMail.numeroPagine = 0;
                    allMail.position = countAllegati + 1;
                    DocsPaVO.documento.Allegato res3 = (await this._mediator.Send(new Requests.DocumentoAggiungiAllegato(new InfoUtente(), allMail))).output;

                    if (res3 == null)
                    {
                        err = Resources.ErroreAggiuntaAllEmailRicevuta;
                        //throw new Exception(err);
                        throw new AddAttachmentPi3Exception(String.Format(ErrorDescriptions.AddAttachMailError, err));
                    }


                    DocsPaVO.documento.FileDocumento fdAllMail = new DocsPaVO.documento.FileDocumento();
                    fdAllMail.content = email.BinaryContent; //Funzionerà?
                    fdAllMail.length = fdAllMail.content.Length;
                    fdAllMail.name = nomeMail;
                    DocsPaVO.documento.FileRequest fRAllMail = new DocsPaVO.documento.FileRequest();
                    fRAllMail = (DocsPaVO.documento.FileRequest)allMail;

                    if (fdAllMail.content.Length > 0)
                    {
                        var putFileResult = await this._mediator.Send(new Requests.DocumentoPutFileNoException(fRAllMail, fdAllMail, new InfoUtente()));
                        var putFileOk = putFileResult.output;
                        err = putFileResult.errorMessage;
                        if (!putFileOk)
                            throw new UploadFilePi3Exception(Resources.ErroreFileAllegato);
                        else
                        {
                            bool tsrOk = await this.MatchTSR(email, fRAllMail, fdAllMail, idPeople);
                            //processedAttachments++;
                            //tipologia ENTESUAP ma sembra che non sia utilizzata 
                            //ParseExtraXmlFiles(sd, docPrincipaleName, fd.content, ruolo, idTenant);
                        }
                    }
                }

                //Trasmissione
                if (!string.IsNullOrEmpty(infoDestinatari) && infoDestinatari.Length > 248)
                    infoDestinatari = infoDestinatari.Substring(0, 248);

                bool mailPendente = await this.MantieniMailRicevutePendenti(reg.systemId.AsLong(), mailAddress);

                DocsPaVO.trasmissione.Trasmissione trasm = new DocsPaVO.trasmissione.Trasmissione();
                trasm.ruolo = ruolo;
                trasm.utente = (await this._mediator.Send(new Application.Requests.getUtenteById(idPeople.ToString()))).output;
                trasm.utente.dst = ""; // ?? poi vedo
                trasm.noteGenerali = "";
                DocsPaVO.documento.InfoDocumento infoDoc = new DocsPaVO.documento.InfoDocumento();
                //infoDoc.idProfile = sd.systemId;
                //infoDoc.docNumber = sd.docNumber;
                //infoDoc.oggetto = sd.oggetto.descrizione;
                //infoDoc.tipoProto = Resources.EtichettaProtoArrivo;
                //infoDoc.idRegistro = registroEntity.SYSTEM_ID.ToString();
                //trasm.infoDocumento = infoDoc; 
                trasm.tipoOggetto = DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO;
                trasm.infoDocumento = (await this._mediator.Send(new Application.Requests.GetInfoDocumento(new InfoUtente(), sd.systemId, sd.docNumber))).output;
                //costruzione singole trasmissioni
                var ragioneEntity = await this._dbContext.RagioneTrasmissioneEntities
                    .Where(x => x.CHA_TIPO_RAGIONE.Equals("I") && x.CHA_TIPO_DIRITTI.Equals("W") && x.ID_AMM == idTenant)
                    .FirstOrDefaultAsync();
                DocsPaVO.trasmissione.RagioneTrasmissione ragione = this._mapper.Map<RagioneTrasmissione>(ragioneEntity);

                // S. Furnari - 16/01/2013 - Sviluppo trasmissione documento ricevuto per interop interno solo a ruoli nella UO
                // destinataria della spedizione e non a tutta la AOO.
                List<Ruolo> ruoliDest = await this.GetRuoliDestTrasm(registroEntity, mailAddress);
                List<TrasmissioneSingola> trasmissioniSing = new List<TrasmissioneSingola>();
                foreach (var r in ruoliDest)
                {
                    //Aggiunta trasmissione singola
                    DocsPaVO.trasmissione.TrasmissioneSingola trSing = new DocsPaVO.trasmissione.TrasmissioneSingola();
                    trSing.ragione = ragione;
                    trSing.corrispondenteInterno = r;
                    trSing.tipoTrasm = "S";

                    trSing.tipoDest = DocsPaVO.trasmissione.TipoDestinatario.RUOLO;
                    if (mailPendente)
                        trSing.ragione.eredita = "0";

                    DocsPaVO.addressbook.QueryCorrispondente qc = new DocsPaVO.addressbook.QueryCorrispondente();
                    qc.codiceRubrica = r.codiceRubrica;
                    List<string> registri = new List<string>();
                    registri.Add(registroEntity.SYSTEM_ID.ToString());
                    qc.idRegistri = registri.ToArray();
                    qc.idAmministrazione = registroEntity.ID_AMM.ToString();
                    qc.getChildren = true;
                    qc.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                    qc.fineValidita = true;

                    List<DocsPaVO.utente.Corrispondente> utenti = (await this._mediator.Send(new Requests.AddressbookGetListaCorrispondenti(qc))).output.ToList();

                    List<DocsPaVO.trasmissione.TrasmissioneUtente> trasmissioniUt = new List<DocsPaVO.trasmissione.TrasmissioneUtente>();

                    foreach (var utente in utenti)
                    {
                        DocsPaVO.trasmissione.TrasmissioneUtente trUt = new DocsPaVO.trasmissione.TrasmissioneUtente();
                        trUt.utente = (DocsPaVO.utente.Utente)utente;
                        trasmissioniUt.Add(trUt);
                    }
                    trSing.trasmissioneUtente = trasmissioniUt.ToArray();
                    trasmissioniSing.Add(trSing);
                }
                trasm.trasmissioniSingole = trasmissioniSing.ToArray();
                DocsPaVO.trasmissione.Trasmissione result = null;
                string desc = string.Empty;
                string method;
                result = (await this._mediator.Send(new Requests.TrasmissioneSaveExecuteTrasm(string.Empty, trasm, new InfoUtente()))).output;

                if (result == null)
                {
                    codint1 = true;
                    err = String.Format(Resources.CODINT1, sd.docNumber);
                    //throw new TrasmissionePi3Exception(err, null, null);
                    return new ProcessorOutput()
                    {
                        Success = false,
                        DocNumber = sd.docNumber.AsLong(),
                        ErrorMessage = err,
                        ProcessedAttachments = processedAttachments
                    };
                }
            }
            catch (Pi3Exception pi3ex)
            {
                _logger.LogError(pi3ex, pi3ex.Message);

                success = false;
                if (string.IsNullOrEmpty(err))
                {
                    err = ErrorDescriptions.ErroreGenericoElaborazioneMail;
                }

                // Se il predisposto è stato creato lo devo cestinare
                if (!string.IsNullOrWhiteSpace(docnumber)) await this.TrashDocument(docnumber.AsLong());
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, ex.Message);

                success = false;
                if(string.IsNullOrEmpty(err))
                {
                    err = ErrorDescriptions.ErroreGenericoElaborazioneMail;
                }

                // Se il predisposto è stato creato lo devo cestinare
                if (!string.IsNullOrWhiteSpace(docnumber)) await this.TrashDocument(docnumber.AsLong());
            }

            return new ProcessorOutput()
            {
                Success = success,
                DocNumber = sd.docNumber.AsLong(),
                ErrorMessage = err,
                ProcessedAttachments = processedAttachments
            };
        }

        //N.B: metodo derivato da DocumentoAddDocGrigiaHandler, stesso metodoo anche in StandardCaseHandler
        private async Task<DocsPaVO.documento.SchedaDocumento> AddDocGrigia(DocsPaVO.documento.SchedaDocumento schedaDocumento, string idTenant, Ruolo ruolo)
        {
            DocsPaVO.documento.SchedaDocumento result = new DocsPaVO.documento.SchedaDocumento();
            var fileRequest = schedaDocumento.documenti == null ? new() : schedaDocumento.documenti[0];
            try
            {
                //INIZIO CREAZIONE DOCUMENTO
                var documentoAmministrativoAggregate = new DocumentoAmministrativo(idTenant, DateTime.Now,
                    new OggettoDelDocumento()
                    {
                        Id = schedaDocumento.oggetto.systemId,
                        Descrizione = new TextValue(schedaDocumento.oggetto.descrizione),
                    },
                new DatiRegistro()
                {
                     IdRegistro = schedaDocumento.registro.systemId
                },
                null,
                schedaDocumento.privato == "1" ? TipologieVisibilitaEnum.Privata : (schedaDocumento.personale == "1" ? TipologieVisibilitaEnum.Personale : TipologieVisibilitaEnum.Gerarchica)
                );

                await _documentoAmministrativoRepository.Add(documentoAmministrativoAggregate);

                schedaDocumento.systemId = documentoAmministrativoAggregate.Id;
                schedaDocumento.docNumber = documentoAmministrativoAggregate.Id;
                schedaDocumento.dataCreazione = documentoAmministrativoAggregate.CreationDate.AsDateFormat();
                schedaDocumento.accessRights = "255";

                fileRequest.docNumber = schedaDocumento.systemId;
                fileRequest.versionId = documentoAmministrativoAggregate.Versions[0].Id;
                if (schedaDocumento.documenti == null)
                    schedaDocumento.documenti = new DocsPaVO.documento.Documento[] { fileRequest };
                else
                    schedaDocumento.documenti[0] = fileRequest;

                //FINE CREAZIONE DOCUMENTO

                await this._webMethodLoggerService.LogOK("ADDDOCGRIGIA", schedaDocumento.systemId, string.Format("N.ro Doc.:" + schedaDocumento.systemId));



                result = schedaDocumento;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("ADDDOCGRIGIA", schedaDocumento.systemId, string.Format("N.ro Doc.: " + schedaDocumento.systemId));
                schedaDocumento = null;
            }

            return result;
        }

        private async System.Threading.Tasks.Task FattElDaPEC(string idDoc)
        {
            bool retVal = false;
            DocsPaVO.documento.FileDocumento fileAllFatt = null;
            string stringaXml = null;
            System.Xml.XmlDocument xmlDoc = null;
            var infoUtente = new InfoUtente();

            DocsPaVO.documento.SchedaDocumento doc = (await this._mediator.Send(new Requests.DocumentoGetDettaglioDocumentoNoSecurity(infoUtente, idDoc, idDoc))).output;

            foreach (DocsPaVO.documento.Allegato allFatt in doc.allegati)
            {
                if (!string.IsNullOrEmpty(allFatt.fileName) && allFatt.fileName.ToUpper().Contains(".XML"))
                {
                    fileAllFatt = (await this._mediator.Send(new Requests.DocumentoGetFile((DocsPaVO.documento.FileRequest)allFatt, infoUtente))).output;
                    if (fileAllFatt != null && !string.IsNullOrEmpty(fileAllFatt.fullName) && fileAllFatt.fullName.ToUpper().EndsWith("XML"))
                    {
                        stringaXml = "";
                        stringaXml = Encoding.UTF8.GetString(fileAllFatt.content);

                        stringaXml = stringaXml.Trim();
                        xmlDoc = new System.Xml.XmlDocument();
                        if (stringaXml.Contains("xml version=\"1.1\""))
                        {
                            _logger.LogDebug("Versione XML 1.1. Provo conversione");
                            stringaXml = stringaXml.Replace("xml version=\"1.1\"", "xml version=\"1.0\"");
                        }
                        try
                        {
                            xmlDoc.LoadXml(stringaXml);
                        }
                        catch (Exception bomUTF8)
                        {
                            string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                            if (stringaXml.StartsWith(byteOrderMarkUtf8))
                            {
                                stringaXml = stringaXml.Remove(0, byteOrderMarkUtf8.Length);
                            }
                            xmlDoc.LoadXml(stringaXml);
                        }
                        if (xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains(CheckMailBox.Resources.www_fatturapa_gov_it_sdi_fatturapa_v1) ||
                        xmlDoc.DocumentElement.NamespaceURI.ToLower().Contains(CheckMailBox.Resources.ivaservizi_agenziaentrate_gov_it_docs_xsd_fatture))
                        {
                            System.Xml.XmlNodeList fatture = xmlDoc.DocumentElement.SelectNodes("//*[name()='FatturaElettronicaBody']");
                            if (fatture.Count > 1)
                                throw new InvoiceErrorPi3Exception(Resources.ErroreLottoDiFatture);
                        }
                        else
                        {
                            #region Associazione dei campi
                            var templateId = await this._dbContext.TipoAttoEntities.Where(x => x.VAR_DESC_ATTO.ToLower().Equals(Resources.DescrizioneFatturaElettronicaTemplate)).Select(x => x.SYSTEM_ID).FirstOrDefaultAsync();
                            DocsPaVO.ProfilazioneDinamica.Templates template = (await this._mediator.Send(new Requests.getTemplateById(templateId.ToString()))).output;
                            if (template != null && template.CHA_ASSOC_MANUALE == "1")
                            {
                                string notePerElaborazioneXML = "";
                                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom in template.ELENCO_OGGETTI)
                                {
                                    try
                                    {

                                        if (!string.IsNullOrEmpty(oggettoCustom.CAMPO_XML_ASSOC))
                                        {
                                            bool associaSecondo = false;
                                            string[] mappings = oggettoCustom.CAMPO_XML_ASSOC.Split('<');
                                            string[] mappingXml = mappings[0].Split('>');
                                            string mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                            for (int i = 1; i < mappingXml.Length; i++)
                                            {
                                                mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                            }
                                            string valore = "";
                                            try
                                            {
                                                System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                valore = node.InnerXml; // valore dell'xml estratto
                                            }
                                            catch (Exception nodo)
                                            {
                                                if (mappings.Length > 1 && !string.IsNullOrEmpty(mappings[1]))
                                                    associaSecondo = true;
                                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_FORNITORE_1"))
                                                {
                                                    valore = "";
                                                }
                                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_NOTE_VER_FIRMA_1"))
                                                {
                                                    valore = "";
                                                }
                                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_IDSDI"))
                                                {
                                                    valore = "";
                                                }
                                                else if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_VERSIONE_1"))
                                                {
                                                    System.Xml.XmlElement root = xmlDoc.DocumentElement;
                                                    valore = root.Attributes["versione"].Value;
                                                }
                                                else
                                                {
                                                    //notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";                                                    
                                                    notePerElaborazioneXML += String.Format(Resources.NotePerElaborazioneXml1, oggettoCustom.DESCRIZIONE);
                                                }
                                            }

                                            if (!string.IsNullOrEmpty(valore) && !string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_CODFISC_1"))
                                            {
                                                if (!Char.IsLetter(valore[0]) || !Char.IsLetter(valore[1]) || !Char.IsLetter(valore[2]))
                                                    valore = "";
                                            }

                                            if (associaSecondo)
                                            {
                                                int associaSecondoI = 1;
                                                while (string.IsNullOrEmpty(valore) && mappings.Length > associaSecondoI && !string.IsNullOrEmpty(mappings[associaSecondoI]))
                                                {
                                                    mappingXml = mappings[associaSecondoI].Split('>');
                                                    mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                    for (int i = 1; i < mappingXml.Length; i++)
                                                    {
                                                        mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                    }
                                                    valore = "";
                                                    try
                                                    {
                                                        System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                        valore = node.InnerXml; // valore dell'xml estratto
                                                    }
                                                    catch (Exception nodo)
                                                    {

                                                    }
                                                    associaSecondoI++;
                                                }
                                                if (string.IsNullOrEmpty(valore))
                                                {
                                                    notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";

                                                }
                                            }

                                            // Estrazione dei campi CDATA
                                            if (valore.Contains("<![CDATA["))
                                            {
                                                valore = valore.Replace("<![CDATA[", "");
                                                valore = valore.Replace("]]>", "");
                                            }

                                            oggettoCustom.VALORE_DATABASE = valore;

                                            if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "CampoDiTesto")
                                            {
                                                if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("MULTINODE"))
                                                {
                                                    string separatore = oggettoCustom.OPZIONI_XML_ASSOC.Split('>')[1];

                                                    System.Xml.XmlNodeList multinode = xmlDoc.SelectNodes(mappingElemento);
                                                    if (multinode.Count > 1)
                                                    {
                                                        valore = "";
                                                        foreach (System.Xml.XmlNode nodoX in multinode)
                                                        {
                                                            if (!valore.Contains(nodoX.InnerXml))
                                                            {
                                                                valore += nodoX.InnerXml + separatore;
                                                            }
                                                        }
                                                    }
                                                    if (valore.Length > 180) valore = valore.Substring(0, 180);

                                                }
                                                #region Fornitore fattura elettronica
                                                if (!string.IsNullOrEmpty(oggettoCustom.OPZIONI_XML_ASSOC) && oggettoCustom.OPZIONI_XML_ASSOC.ToUpper().Contains("FATT_EL_FORNITORE_1"))
                                                {
                                                    if (string.IsNullOrEmpty(valore))
                                                    {
                                                        string mappingNome = "CedentePrestatore>DatiAnagrafici>Anagrafica>Nome";
                                                        mappingXml = mappingNome.Split('>');
                                                        mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                        for (int i = 1; i < mappingXml.Length; i++)
                                                        {
                                                            mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                        }
                                                        valore = "";
                                                        try
                                                        {
                                                            System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                            valore = node.InnerXml; // valore dell'xml estratto
                                                        }
                                                        catch (Exception nodo)
                                                        {
                                                            //notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";
                                                            notePerElaborazioneXML += String.Format(Resources.NotePerElaborazioneXml1, oggettoCustom.DESCRIZIONE);
                                                        }

                                                        string mappingCognome = "CedentePrestatore>DatiAnagrafici>Anagrafica>Cognome";
                                                        mappingXml = mappingCognome.Split('>');
                                                        mappingElemento = String.Format("//*[name()='{0}']", mappingXml[0]);
                                                        for (int i = 1; i < mappingXml.Length; i++)
                                                        {
                                                            mappingElemento += String.Format("/*[name()='{0}']", mappingXml[i]);
                                                        }
                                                        try
                                                        {
                                                            System.Xml.XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(mappingElemento);
                                                            valore += (" " + node.InnerXml); // valore dell'xml estratto
                                                        }
                                                        catch (Exception nodo)
                                                        {
                                                            //notePerElaborazioneXML += "XML " + oggettoCustom.DESCRIZIONE + ". ";
                                                            notePerElaborazioneXML += String.Format(Resources.NotePerElaborazioneXml1, oggettoCustom.DESCRIZIONE);
                                                        }
                                                    }
                                                }
                                                #endregion
                                                oggettoCustom.VALORE_DATABASE = valore;
                                            }
                                            if (!string.IsNullOrEmpty(valore))
                                            {
                                                if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "MenuATendina" || oggettoCustom.TIPO.DESCRIZIONE_TIPO == "SelezioneEsclusiva")
                                                {
                                                    string[] valoriAssociati1 = oggettoCustom.OPZIONI_XML_ASSOC.Split('>');
                                                    bool trovato = false;
                                                    for (int i = 0; i < valoriAssociati1.Length; i++)
                                                    {
                                                        string[] valoriAssociati2 = valoriAssociati1[i].Split('<');
                                                        if (valoriAssociati2[1] == valore)
                                                        {
                                                            oggettoCustom.VALORE_DATABASE = valoriAssociati2[0];
                                                            trovato = true;
                                                        }
                                                    }
                                                    if (!trovato)
                                                    {
                                                        //notePerElaborazioneXML += "Errore " + oggettoCustom.DESCRIZIONE + ". " + valore + " non valido. ";
                                                        notePerElaborazioneXML += string.Format(Resources.NotePerElaborazioneXml2, oggettoCustom.DESCRIZIONE, valore);
                                                    }

                                                }
                                                else if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Data")
                                                {
                                                    string conversione = oggettoCustom.OPZIONI_XML_ASSOC;
                                                    try
                                                    {

                                                        DateTime dtp = DateTime.ParseExact(valore.Substring(0, conversione.Length), conversione, System.Globalization.CultureInfo.InvariantCulture);

                                                        oggettoCustom.VALORE_DATABASE = dtp.ToString("dd/MM/yyyy");
                                                        if (!string.IsNullOrEmpty(oggettoCustom.FORMATO_ORA))
                                                        {
                                                            oggettoCustom.VALORE_DATABASE = dtp.ToString("dd/MM/yyyy " + oggettoCustom.FORMATO_ORA);
                                                        }
                                                    }
                                                    catch (Exception exData)
                                                    {
                                                        oggettoCustom.VALORE_DATABASE = "";
                                                        //notePerElaborazioneXML += "Errore " + oggettoCustom.DESCRIZIONE + ". " + valore + " non valido. ";
                                                        notePerElaborazioneXML += string.Format(Resources.NotePerElaborazioneXml2, oggettoCustom.DESCRIZIONE, valore);
                                                    }

                                                }
                                                else if (oggettoCustom.TIPO.DESCRIZIONE_TIPO == "Corrispondente")
                                                {
                                                    #region Da rifare con metodi da Frontend
                                                    DocsPaVO.rubrica.ParametriRicercaRubrica filtriRic = new DocsPaVO.rubrica.ParametriRicercaRubrica();
                                                    filtriRic.doRubricaComune = true;
                                                    filtriRic.tipoIE = DocsPaVO.addressbook.TipoUtente.ESTERNO;
                                                    filtriRic.caller = new DocsPaVO.rubrica.ParametriRicercaRubrica.CallerIdentity();
                                                    filtriRic.caller.IdRuolo = infoUtente.idGruppo;
                                                    filtriRic.caller.IdUtente = infoUtente.idPeople;
                                                    filtriRic.caller.filtroRegistroPerRicerca = string.Empty;
                                                    string tipoRicerca = oggettoCustom.OPZIONI_XML_ASSOC.Split('§')[0];
                                                    //if (tipoRicerca == "CODE")
                                                    //    filtriRic.codice = valore;
                                                    switch (tipoRicerca)
                                                    {
                                                        case "CODE":
                                                            filtriRic.codice = valore;
                                                            break;
                                                        case "DESCRIZIONE":
                                                            filtriRic.descrizione = valore;
                                                            break;
                                                        case "PIVA":
                                                            filtriRic.partitaIva = valore;
                                                            break;
                                                        case "CF":
                                                            filtriRic.codiceFiscale = valore;
                                                            break;
                                                        case "MAIL":
                                                            filtriRic.email = valore;
                                                            break;
                                                    }

                                                    DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica = new DocsPaVO.rubrica.SmistamentoRubrica();
                                                    List<ElementoRubrica> objElementiRubrica = (await this._mediator.Send(new Requests.rubricaGetElementiRubrica(filtriRic, infoUtente, smistamentoRubrica))).output.ToList();
                                                    if (objElementiRubrica != null && objElementiRubrica.Count > 0)
                                                    {
                                                        string sysId = "";

                                                        if (!string.IsNullOrEmpty(((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).systemId))
                                                        {
                                                            sysId = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).systemId;
                                                        }
                                                        else
                                                        {
                                                            var codCorr = ((DocsPaVO.rubrica.ElementoRubrica)objElementiRubrica[0]).codice;
                                                            DocsPaVO.utente.Corrispondente corr = (await this._mediator.Send(new Requests.AddressbookGetCorrispondenteBySystemId(codCorr))).output;

                                                            if ((corr == null) || (corr != null && string.IsNullOrEmpty(corr.systemId) && string.IsNullOrEmpty(corr.codiceRubrica)))
                                                            {
                                                                bool rubricaComuneAbilitata = (await this._mediator.Send(new Application.Requests.GetConfigurazioniRubricaComune(new InfoUtente()))).output.GestioneAbilitata;
                                                                if (rubricaComuneAbilitata)
                                                                {
                                                                    corr = await this.UpdateCorrispondente(codCorr);
                                                                }

                                                                if (corr != null)
                                                                    sysId = corr.systemId;
                                                            }
                                                        }
                                                        oggettoCustom.VALORE_DATABASE = sysId;
                                                    }
                                                    else
                                                    {
                                                        oggettoCustom.VALORE_DATABASE = string.Empty;
                                                        notePerElaborazioneXML += string.Format(Resources.NotePerElaborazioneXml3, oggettoCustom.DESCRIZIONE);
                                                    }
                                                    #endregion
                                                }
                                                else if ((oggettoCustom.TIPO.DESCRIZIONE_TIPO).Equals("CasellaDiSelezione"))
                                                {

                                                }
                                            }
                                            else
                                            {
                                                notePerElaborazioneXML += oggettoCustom.DESCRIZIONE + ". ";
                                            }

                                        }
                                    }
                                    catch (Exception ex1end)
                                    {
                                        oggettoCustom.VALORE_DATABASE = string.Empty;
                                        oggettoCustom.VALORI_SELEZIONATI = null;
                                    }

                                }


                                foreach (DocsPaVO.ProfilazioneDinamica.OggettoCustom notePerXML in template.ELENCO_OGGETTI)
                                {
                                    if (notePerXML.DESCRIZIONE.ToUpper() == Resources.NotePerXmlOggettoCustomDesc)
                                    {
                                        if (!string.IsNullOrEmpty(notePerElaborazioneXML))
                                        {
                                            notePerElaborazioneXML = string.Format(Resources.NotePerElaborazioneXml4, notePerElaborazioneXML);
                                            if (notePerElaborazioneXML.Length > 220)
                                            {
                                                notePerElaborazioneXML = notePerElaborazioneXML.Substring(0, 220) + "...";
                                            }
                                            notePerXML.VALORE_DATABASE = notePerElaborazioneXML;
                                        }
                                        else
                                            notePerXML.VALORE_DATABASE = Resources.ElaborazioneAvvenutaConSuccesso;
                                    }
                                }

                                doc.template = template;
                                doc.tipologiaAtto = new DocsPaVO.documento.TipologiaAtto();
                                doc.tipologiaAtto.systemId = template.SYSTEM_ID.ToString();
                                doc.tipologiaAtto.descrizione = template.DESCRIZIONE;
                                doc.daAggiornareTipoAtto = true;

                                bool daaggiornTemp = false;
                                //doc = BusinessLogic.Documenti.DocSave.save(infoUtente, doc, false, out daaggiornTemp, null);
                                doc = (await this._mediator.Send(new Requests.DocumentoSaveDocumento(null, infoUtente, doc, daaggiornTemp))).output;

                            }
                            else
                            {
                                throw new InvoiceErrorPi3Exception(Resources.TemplateFatturaNonTrovato);
                            }
                            #endregion

                            #region Caricamento allegati fattura se presenti
                            System.Xml.XmlNodeList listanodi = xmlDoc.DocumentElement.SelectNodes("//*[name()='Allegati']");
                            System.Xml.XmlNode nodoNome = null;
                            System.Xml.XmlNode nodoContent = null;
                            DocsPaVO.documento.FileRequest allegato = null;
                            DocsPaVO.documento.FileDocumento fileAllegato = null;
                            string erroreMessage = "";
                            bool caricaAllegato = true;
                            if (listanodi != null && listanodi.Count > 0)
                            {
                                foreach (System.Xml.XmlNode nodo in listanodi)
                                {
                                    nodoNome = null;
                                    nodoContent = null;
                                    caricaAllegato = true;

                                    foreach (System.Xml.XmlNode nodo1 in nodo.ChildNodes)
                                    {
                                        //Console.WriteLine(nodo1.Name);
                                        if (nodo1.Name.ToUpper() == "NOMEATTACHMENT") nodoNome = nodo1;
                                        if (nodo1.Name.ToUpper() == "ATTACHMENT") nodoContent = nodo1;
                                    }


                                    //byte[] contentNormal = Convert.FromBase64String(nodoContent.InnerXml);
                                    foreach (DocsPaVO.documento.Allegato alltempx1 in doc.allegati)
                                    {
                                        if (alltempx1.descrizione.ToUpper() == nodoNome.InnerXml.ToUpper()) caricaAllegato = false;
                                    }
                                    if (caricaAllegato)
                                    {
                                        allegato = new DocsPaVO.documento.Allegato
                                        {
                                            docNumber = doc.systemId,
                                            descrizione = nodoNome.InnerXml
                                        };
                                        allegato = (await this._mediator.Send(new Requests.DocumentoAggiungiAllegato(infoUtente, ((DocsPaVO.documento.Allegato)allegato)))).output;

                                        fileAllegato = new DocsPaVO.documento.FileDocumento
                                        {
                                            name = nodoNome.InnerXml,
                                            fullName = nodoNome.InnerXml,
                                            content = Convert.FromBase64String(nodoContent.InnerXml),
                                            length = Convert.FromBase64String(nodoContent.InnerXml).Length,
                                            bypassFileContentValidation = true
                                        };

                                        var putFileResult = await this._mediator.Send(new Requests.DocumentoPutFileNoException(allegato, fileAllegato, new InfoUtente()));
                                        var putFileOk = putFileResult.output;
                                        if (!putFileOk)
                                            throw new UploadFilePi3Exception(Resources.ErroreInCreazioneFile);
                                        else
                                            retVal = true;
                                    }
                                    else
                                    {
                                        _logger.LogDebug(Resources.AllegatoGiaPresente);
                                    }

                                }
                            }
                            #endregion

                        }
                    }
                }
            }
        }

        private async Task<bool> InsertMailCorrispondente(List<MailCorrispondente> listCaselleCorr, long idCorrispondente)
        {
            bool result = false;
            int rowsInserted = 0;
            //Elimino tutte le caselle precedentemente associate al corrispondente esterno in DPA_MAIL_CORR_ESTERNI
            var entitiesToRemove = await this._dbContext.MailCorrEsterniEntities.Where(x => x.ID_CORR == idCorrispondente).ToListAsync();

            if (entitiesToRemove != null && entitiesToRemove.Any())
            {
                this._dbContext.MailCorrEsterniEntities.RemoveRange(entitiesToRemove);
                int rowsDeleted = await ((DbContext)_dbContext).SaveChangesAsync();

                if (rowsDeleted > 0)
                {
                    this._logger.LogDebug($"InsertMailCorrispondente > Eliminate {rowsDeleted} righe in DPA_MAIL_CORR_ESTERNI per il corridpondente con id: {idCorrispondente}");
                    //Insert in DPA_MAIL_CORR_ESTERNI delle nuove caselle associate al corrispondente esterno 
                    foreach (MailCorrispondente c in listCaselleCorr)
                    {
                        var entityToInsert = new MailCorrEsterniEntity()
                        {
                            ID_CORR = idCorrispondente,
                            VAR_EMAIL = c.Email.Trim(),
                            VAR_PRINCIPALE = c.Principale,
                            VAR_NOTE = c.Note?.Replace("'", "''")
                        };

                        this._dbContext.MailCorrEsterniEntities.Add(entityToInsert);
                        rowsInserted = await ((DbContext)_dbContext).SaveChangesAsync();
                        result = rowsInserted > 0;

                    }
                }
            }
            else //inserito perché 
            {
                this._logger.LogDebug($"InsertMailCorrispondente > Non ci sono righe da eliminare in DPA_MAIL_CORR_ESTERNI per il corridpondente con id: {idCorrispondente}, inserisco solo");
                //Insert in DPA_MAIL_CORR_ESTERNI delle nuove caselle associate al corrispondente esterno 
                foreach (MailCorrispondente c in listCaselleCorr)
                {
                    var entityToInsert = new MailCorrEsterniEntity()
                    {
                        ID_CORR = idCorrispondente,
                        VAR_EMAIL = c.Email.Trim(),
                        VAR_PRINCIPALE = c.Principale,
                        VAR_NOTE = c.Note?.Replace("'", "''")
                    };

                    this._dbContext.MailCorrEsterniEntities.Add(entityToInsert);
                    rowsInserted = await ((DbContext)_dbContext).SaveChangesAsync();
                    result = rowsInserted > 0;
                }
            }
            return result;
        }

        private async Task<bool> InsertMailCorr(List<MailCorrispondente> listCaselleCorr, string idCorrispondente)
        {
            return true;
            /*
            DocsPaUtils.Query q;
            bool res = false;
            // Elimino tutte le caselle precedentemente associate al corrispondente esterno in DPA_MAIL_CORR_ESTERNI
            q = DocsPaUtils.InitQuery.getInstance().getQuery("D_DPA_MAIL_CORR_ESTERNO");
            q.setParam("idCorrispondente", idCorrispondente);
            //BeginTransaction();
            string commandText = q.getSQL();
            logger.Debug(commandText);
            res = ExecuteNonQuery(commandText);
            if (!res)
            {
                logger.Error("Errore nella cancellazione delle mail");
                //RollbackTransaction();
            }
            //else
            // CommitTransaction();

            if (res)
            {
                // Insert in DPA_MAIL_CORR_ESTERNI delle nuove caselle associate al corrispondente esterno 
                //BeginTransaction();
                foreach (MailCorrispondente c in listCaselleCorr)
                {
                    System.Text.StringBuilder recordInsert = new System.Text.StringBuilder();
                    q = DocsPaUtils.InitQuery.getInstance().getQuery("I_DPA_MAIL_CORR_ESTERNO");
                    recordInsert.Append("(\n");
                    recordInsert.Append(
                        ((DBType.ToUpper().Equals("ORACLE")) ? DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal("DPA_MAIL_CORR_ESTERNI") + "\n" : "\n") +
                         idCorrispondente + ",\n" +
                        "'" + c.Email.Trim() + "',\n " +
                        "'" + c.Principale + "',\n" +
                        "'" + (string.IsNullOrEmpty(c.Note) ? c.Note : c.Note.Replace("'", "''")) + "'");
                    recordInsert.Append(")");
                    q.setParam("value", recordInsert.ToString());
                    string commandText2 = q.getSQL();
                    logger.Debug(commandText2);

                    res = ExecuteNonQuery(commandText2);
                    if (!res)
                    {
                        logger.Error("Errore in inserimento delle mail");

                        //RollbackTransaction();
                        return res;
                    }
                }
                //CommitTransaction();
            }
            return res;
            */
        }

        private async System.Threading.Tasks.Task UpdateCanalePref(DocsPaVO.utente.Corrispondente? mittente)
        {

            DocsPaVO.utente.UnitaOrganizzativa uoMitt = new DocsPaVO.utente.UnitaOrganizzativa();
            if (mittente.GetType() == typeof(DocsPaVO.utente.UnitaOrganizzativa)) uoMitt = (DocsPaVO.utente.UnitaOrganizzativa)mittente;
            if (mittente.GetType() == typeof(DocsPaVO.utente.Ruolo)) uoMitt = ((DocsPaVO.utente.Ruolo)mittente).uo;
            if (mittente.GetType() == typeof(DocsPaVO.utente.Utente)) uoMitt = ((DocsPaVO.utente.Ruolo)((DocsPaVO.utente.Utente)mittente).ruoli[0]).uo;

            //Nuova Gestione
            //Si cercano le system_id dei possibili tipi di canale - mail o interop            
            /*
              SELECT
              SYSTEM_ID
              FROM DOCUMENTTYPES
              WHERE CHA_TIPO_CANALE IN ('M', 'I')
            */
            List<string?> tipiCanale = new List<string>() { "M", "I" };
            var tipoCanaleList = await this._dbContext.DocumentTypesEntities.Where(x => tipiCanale.Contains(x.CHA_TIPO_CANALE)).Select(x => x.SYSTEM_ID).ToListAsync();

            string idTipoCanale = string.Empty;
            string mailId = string.Empty;

            if (tipoCanaleList.Count != 0)
                mailId = tipoCanaleList[0].ToString();

            //if (tipoCanaleList.Count == 1)
            //    idTipoCanale = " AND ID_DOCUMENTTYPE = " + ds.Tables["TIPO_CAN"].Rows[0]["SYSTEM_ID"].ToString();
            //if (tipoCanaleList.Count == 2)
            //    idTipoCanale = " AND (ID_DOCUMENTTYPE = " + ds.Tables["TIPO_CAN"].Rows[0]["SYSTEM_ID"].ToString() + " OR ID_DOCUMENTTYPE = " + ds.Tables["TIPO_CAN"].Rows[1]["SYSTEM_ID"].ToString() + ")";

            //Si verifica se è già associato un tipo canale al corrispondente
            long uoMittSysId = uoMitt.systemId.AsLong();
            var canaliList = await this._dbContext.CanaleCorrEntities.Where(x => x.ID_CORR_GLOBALE == uoMittSysId && tipoCanaleList.Contains(x.ID_DOCUMENTTYPE)).ToListAsync();

            //Se il corrispondente non ha un tipo canale lo associamo
            if (canaliList.Count == 0 && !string.IsNullOrEmpty(mailId))
            {
                /*
                  INSERT INTO DPA_T_CANALE_CORR
                  (
                  DocsPaDbManagement.Functions.Functions.GetSystemIdColName(),
                  ID_CORR_GLOBALE,
                  ID_DOCUMENTTYPE,
                  CHA_PREFERITO
                  ) VALUES (
                  DocsPaDbManagement.Functions.Functions.GetSystemIdNextVal(""),
                  uoMitt.systemId,
                  mailId,
                  '1'
                  )
                 */

                CanaleCorrEntity entityToInsert = new CanaleCorrEntity()
                {
                    ID_CORR_GLOBALE = uoMittSysId,
                    ID_DOCUMENTTYPE = mailId.AsLong(),
                    CHA_PREFERITO = "1"
                };

                await this._dbContext.CanaleCorrEntities.AddAsync(entityToInsert);
                int rowsInserted = await ((DbContext)_dbContext).SaveChangesAsync();
                if (rowsInserted == 0)
                    this._logger.LogError("Inserimento in DPA_T_CANALE_CORR non riuscito");
            }
        }
        #endregion

        #region Private Members

        protected readonly ILogger<SegnaturaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IRubricaComuneService _rubricaComuneService;
        protected readonly IConfigurationService _configurationService;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IDocumentoAmministrativoRepository _documentRepository;
        protected readonly IDocumentBlobRepository _documentBlobRepository;
        protected readonly IUOCorrispondenteRepository _uoCorrispondenteRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IFirmaDigitale2Service _firmaDigitale2Service;
        protected readonly IFactoryService _factoryService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;

        private readonly string BearerPrefix = Resources.BearerPrefix;

        protected IMapper _mapper = null;
        private string _mailOrigine;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CorrGlobaliEntity, Ruolo>()
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.codiceRubrica, opt => opt.MapFrom(src => src.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.VAR_DESC_CORR));

                cfg.CreateMap<DocumentTypesEntity, MezzoSpedizione>()
                    .ForMember(dest => dest.IDSystem, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.chaTipoCanale, opt => opt.MapFrom(src => src.CHA_TIPO_CANALE))
                    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.DESCRIPTION));

                cfg.CreateMap<AppEntity, DocsPaVO.documento.Applicazione>()
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.DESCRIPTION))
                    .ForMember(dest => dest.mimeType, opt => opt.MapFrom(src => src.MIME_TYPE))
                    .ForMember(dest => dest.estensione, opt => opt.MapFrom(src => src.DEFAULT_EXTENSION));

                cfg.CreateMap<RagioneTrasmissioneEntity, RagioneTrasmissione>()
                    .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.VAR_DESC_RAGIONE))
                    .ForMember(dest => dest.risposta, opt => opt.MapFrom(src => src.CHA_RISPOSTA))
                    .ForMember(dest => dest.tipo, opt => opt.MapFrom(src => "N"))
                    .ForMember(dest => dest.tipoDestinatario, src => src.MapFrom(opt => RagioneTrasmissione.tipoGerarchiaStringa.Keys.OfType<TipoGerarchia>().FirstOrDefault(s => RagioneTrasmissione.tipoGerarchiaStringa[s].Equals(opt.CHA_TIPO_DEST))))
                    .ForMember(dest => dest.tipoDiritti, opt => opt.MapFrom(src => DocsPaVO.trasmissione.TipoDiritto.WRITE))
                    .ForMember(dest => dest.eredita, opt => opt.MapFrom(src => src.CHA_EREDITA));
            });

            this._mapper = configuration.CreateMapper();
        }

        #region Gestione corrispondenti e rubrica
        private async Task<DocsPaVO.utente.Corrispondente?> UpdateCorrispondente(string codiceRubrica)
        {
            //var codiceRubrica = corr.codiceRubrica;
            bool rubEstAttive = false;
            string idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            (string value, bool keyFound) = await this._configurationService.TryGetValue<string>(idTenant, "BE_ENABLE_RUBRICHE_ESTERNE");

            if (keyFound && !string.IsNullOrEmpty(value) && value.Equals("1"))
            {
                rubEstAttive = true;
            }

            (string beKey, bool beRubFound) = await this._configurationService.TryGetValue<string>("BE_RUBRICHE_ESTERNE");

            if (rubEstAttive && beRubFound && !string.IsNullOrEmpty(beKey) && !beKey.Equals("0"))
            {
                var elemento = await this.GetElementiInRubricaComCod(codiceRubrica, beKey);

                if (elemento != null)
                    return await this.GetDettAndUpdateCorr(elemento);
                else
                    return null;
            }
            return null;
        }

        private async Task<Services.RubricaComune.Corrispondente> GetElementiInRubricaComCod(string codice, string rubEsterna)
        {
            var elementiRubrica = new List<Services.RubricaComune.Corrispondente>();
            Services.RubricaComune.Corrispondente elemento = new();
            var criteriRicerca = new List<CriterioRicerca>
            {
                new CriterioRicerca { 
                    Campo = CampiRicercaEnum.Codice, 
                    Valore = codice.Replace("'", "''"), 
                    TipoRicercaParola = TipiRicercaParolaEnum.ParolaIntera 
                }
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
        protected string? GetAuthToken()
        {
            var authorizationHeader = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>("KeyToken");

            //if (!this._httpContextAccessor.HttpContext!.Request.Headers.TryGetValue("Authorization", out StringValues authorizationStrings)) { return string.Empty; }

            //var authorizationHeader = authorizationStrings[0]!.StartsWith(BearerPrefix) ? authorizationStrings[0]!.Substring(BearerPrefix.Length) : authorizationStrings[0];

            return authorizationHeader;
        }
        private async Task<(bool, string)> CorrInRubCom(string codiceRubrica, string idAmministrazione)
        {
            bool found = false;

            long corr = 0;
            if (!string.IsNullOrEmpty(codiceRubrica))
                corr = await (from a in this._dbContext.CorrGlobaliEntities.AsNoTracking()
                              where a.VAR_COD_RUBRICA != null &&
                              a.VAR_COD_RUBRICA.ToUpper().Equals(codiceRubrica.ToUpper()) &&
                              a.CHA_TIPO_CORR != null && a.CHA_TIPO_CORR.Equals("C")
                              select a.SYSTEM_ID).FirstOrDefaultAsync();
            if (corr != 0)
                found = true;

            return (found, corr != 0 ? corr.ToString() : string.Empty);
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
        private async Task<DocsPaVO.utente.Corrispondente> GetDettAndUpdateCorr(Services.RubricaComune.Corrispondente elemento)
        {
            DocsPaVO.utente.Corrispondente corr = null;

            (bool inRubCom, string sysId) = await this.CorrInRubCom(elemento.Codice, string.Empty);

            if (inRubCom)
            {
                corr = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteCompletoBySystemId(sysId, TipoUtente.ESTERNO, new InfoUtente()))).output;
                if (corr != null)
                {
                    corr.inRubricaComune = true;
                    corr.info = await this.GetDettagliCorr(corr.systemId);
                    corr.dettagli = true;
                }
            }

            (DocsPaVO.utente.Corrispondente corrispondente, bool isCorrModified) = await this.Update(corr, elemento);

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
        private async Task<(DocsPaVO.utente.Corrispondente, bool)> Update(DocsPaVO.utente.Corrispondente corrispondente, Services.RubricaComune.Corrispondente elemento)
        {
            var idPeople = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idGruppo = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var idRuoloInUo = await _dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();
            var infoUtente = new InfoUtente()
            {
                idPeople = idPeople.ToString(),
                idGruppo = idGruppo.ToString(),
                idCorrGlobali = idRuoloInUo.ToString()
            };

            bool isCorrModified = false;
            bool requestNew = (corrispondente == null);
            bool idDirty = false;
            string codFisc = "";
            string pIva = "";
            bool variazioneEmail = false;

            var elUrls = new List<string?>();
            if (!string.IsNullOrEmpty(elemento.UrlApiInteroperabilita))
            {
                elUrls.Add(elemento.UrlApiInteroperabilita);
            }

            if (!requestNew)
            {

                if (corrispondente.dettagli)
                {
                    if (corrispondente.info == null)
                    {
                        corrispondente.info = await this.GetDettagliCorr(corrispondente.systemId);
                    }

                    codFisc = ((DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)(corrispondente.info.Tables[0].Rows[0])).codiceFiscale ?? string.Empty;
                    pIva = ((DocsPaVO.addressbook.DettagliCorrispondente.CorrispondenteRow)(corrispondente.info.Tables[0].Rows[0])).partitaIva ?? string.Empty;
                }

                idDirty = (corrispondente.descrizione != elemento.Denominazione ||
                            (corrispondente.codiceAmm ?? string.Empty) != (elemento.Amministrazione ?? string.Empty) ||
                            (corrispondente.codiceAOO ?? string.Empty) != (elemento.AOO ?? string.Empty) ||
                            (corrispondente.email ?? string.Empty) != (elemento.Email ?? string.Empty) ||
                            codFisc.ToUpper() != (elemento.CodiceFiscale ?? string.Empty).ToUpper() ||
                            pIva.ToUpper() != (elemento.PartitaIva ?? string.Empty).ToUpper());

                if (!idDirty)
                {
                    var urlCount = !string.IsNullOrEmpty(elemento.UrlApiInteroperabilita) ? 1 : 0;
                    idDirty = urlCount != corrispondente.Url.Count;
                    if (!idDirty && urlCount == corrispondente.Url.Count && corrispondente.Url.Count > 0)
                    {
                        idDirty = corrispondente.Url[0].Url != elemento.UrlApiInteroperabilita;
                    }
                }

                if (!idDirty)
                {
                    if (elemento.Emails != null && elemento.Emails.Count > 0) //verifica se da RC torna che il corr ha almeno una mail, altrimenti tutto il controllo sotto non ha senso
                    {
                        if (elemento.Emails.Count > 1)
                            idDirty = elemento.Emails.Count != corrispondente.Emails.Count;
                        else
                            idDirty = (elemento.Email ?? string.Empty).ToLower() != (corrispondente.email ?? string.Empty).ToLower();

                        if (elemento.Emails.Count != corrispondente.Emails.Count)
                            variazioneEmail = true;
                    }

                    if (!idDirty)
                    {
                        foreach (var mail in elemento.Emails)
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


                if (((corrispondente.canalePref != null && (corrispondente.canalePref.typeId == Resources.InteroperabilityCode ||
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
                        idDirty = ((oldRow.indirizzo ?? string.Empty) != (elemento.Indirizzo ?? string.Empty) ||
                                            (oldRow.citta ?? string.Empty) != (elemento.Citta ?? string.Empty) ||
                                            (oldRow.cap ?? string.Empty) != (elemento.CAP ?? string.Empty) ||
                                            (oldRow.provincia ?? string.Empty) != (elemento.Provincia ?? string.Empty) ||
                                            (oldRow.nazione ?? string.Empty) != (elemento.Nazione ?? string.Empty) ||
                                            (oldRow.telefono ?? string.Empty) != (elemento.Telefono ?? string.Empty) ||
                                            (oldRow.fax ?? string.Empty) != (elemento.Fax ?? string.Empty) ||
                                            (oldRow.codiceFiscale ?? string.Empty) != (elemento.CodiceFiscale ?? string.Empty) ||
                                            (oldRow.partitaIva ?? string.Empty) != (elemento.PartitaIva ?? string.Empty));

                    }
                }
            }
            if (requestNew)
            {
                DocsPaVO.utente.Corrispondente newCorr = await this.GetNuovoCorr(elemento, elemento.Email, elUrls);
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
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

                corrispondente.descrizione = elemento.Denominazione;
                corrispondente.codiceAmm = elemento.Amministrazione;
                corrispondente.codiceAOO = elemento.AOO;
                corrispondente.email = elemento.Email;
                corrispondente.Url = GetInternalUrlsCollection(elUrls);
                corrispondente.canalePref = await GetCanaleCorrispondente(corrispondente, idTenant);
                DocsPaVO.utente.DatiModificaCorr datiModifica = this.GetDatiPerModifica(corrispondente.systemId, corrispondente.canalePref.systemId, elemento, elemento.Email, elUrls);

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
                    throw new AddressBookErrorPi3Exception(Resources.ModifyCorrFailed);
                }

                if (variazioneEmail)
                {
                    List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();

                    foreach (var mail in elemento.Emails)
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
                        throw new AddressBookErrorPi3Exception(Resources.ErrorModifyMail);
                    }

                }

            }
            isCorrModified = idDirty;
            return (corrispondente, isCorrModified);
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
        private List<DocsPaVO.utente.Corrispondente.UrlInfo> GetInternalUrlsCollection(List<string> urlInfo)
        {
            List<DocsPaVO.utente.Corrispondente.UrlInfo> retCollection = new();
            if (urlInfo != null)
                retCollection.AddRange(from url in urlInfo
                                       select new DocsPaVO.utente.Corrispondente.UrlInfo() { Url = url });

            return retCollection;

        }
        // CHAN |MAIL <=> email has val and aoo,amm are without val
        // CHAN |interop semp <=> aoo,amm has val and url
        // CHAN |interop classica <=> aoo,amm has val and has email 
        private async Task<DocsPaVO.utente.Canale?> GetCanaleCorrispondente(DocsPaVO.utente.Corrispondente corrispondente, string idTenant)
        {
            DocsPaVO.utente.Canale? output = null;
            bool canaleMail = false;
            bool canaleInterop = false;
            bool canaleInteropSemplificata = false;


            if (!string.IsNullOrEmpty(corrispondente.codiceAmm) && !string.IsNullOrEmpty(corrispondente.codiceAOO))
            {
                canaleInteropSemplificata = await this.IsEnabledSimplifiedInteroperability(idTenant) &&
                    corrispondente.Url != null &&
                    corrispondente.Url.Count > 0 &&
                    Uri.IsWellFormedUriString(corrispondente.Url[0].Url, UriKind.Absolute);
                canaleInterop = !string.IsNullOrEmpty(corrispondente.email) && !canaleInteropSemplificata;
            }
            else if (!string.IsNullOrEmpty(corrispondente.email))
            {
                canaleInterop = false;
                canaleInteropSemplificata = false;
                canaleMail = true;
            }

            //GET CHANNEL LIST
            var orderdChanEnt = await this._dbContext.DocumentTypesEntities.AsNoTracking().OrderBy(c => c.DESCRIPTION).ToListAsync();
            foreach (var c in orderdChanEnt)
            {
                if ((Channel.Interop == c.TYPE_ID && canaleInterop && !canaleInteropSemplificata) ||
                    (Channel.Mail == c.TYPE_ID && canaleMail) ||
                    (Channel.Lettera == c.TYPE_ID && !canaleMail && !canaleInterop && !canaleInteropSemplificata) ||
                    (Channel.Simpinterop == c.TYPE_ID && canaleInteropSemplificata))
                {
                    output = new()
                    {
                        systemId = c.SYSTEM_ID.ToString(),
                        typeId = c.TYPE_ID,
                        descrizione = c.DESCRIPTION ?? string.Empty,
                        tipoCanale = c.CHA_TIPO_CANALE ?? string.Empty
                    };
                    return output;

                }
            }
            return output;
        }

        private async Task<bool> IsEnabledSimplifiedInteroperability(string idAmm)
        {
            bool enabled = false;
            (string? enabledForAmm, bool keyFound) = await this._configurationService.TryGetValue<string>(idAmm, "INTEROP_SERVICE_ACTIVE");

            if (keyFound)
            {
                enabled = "1".Equals(enabledForAmm);
            }
            else
            {
                (string? en, bool found) = await this._configurationService.TryGetValue<string>("INTEROP_SERVICE_ACTIVE");
                enabled = "1".Equals(en);
            }


            return enabled;
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
        private async Task<DocsPaVO.utente.Corrispondente> GetNuovoCorr(Services.RubricaComune.Corrispondente elemento, string email, List<string> urls)
        {

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
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

            corrispondente.canalePref = await GetCanaleCorrispondente(corrispondente, idTenant);
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
        private async Task<List<Ruolo>> GetRuoliDestTrasm(RegistroEntity registroEntity, string mailAddress)
        {
            List<Ruolo> ruoliDestTrasm = new List<Ruolo>();
            var interopNoMail = await this._configurationService.GetValue<string>("INTEROP_INT_NO_MAIL");

            var j1 = this._dbContext.CorrGlobaliEntities.Join(this._dbContext.TipoRuoloEntities, a => a.ID_TIPO_RUOLO, d => d.SYSTEM_ID, (a, d) => new { a, d });
            var j2 = j1.Join(this._dbContext.CorrGlobaliEntities, j1 => j1.a.ID_UO, e => e.SYSTEM_ID, (j1, e) => new { a = j1.a, d = j1.d, e });
            var j3 = j2.Join(this._dbContext.RuoloRegistroEntities, j2 => j2.a.SYSTEM_ID, f => f.ID_RUOLO_IN_UO, (j2, f) => new { a = j2.a, d = j2.d, e = j2.e, f });
            var j4 = j3.SelectMany(t1 => this._dbContext.VisMailRegistriEntities.Where(t2 => t1.f.ID_RUOLO_IN_UO == t2.ID_RUOLO_IN_UO && t1.f.ID_REGISTRO == t2.ID_REGISTRO).DefaultIfEmpty(),
                (j3, v) => new JoinEntity { a = j3.a, d = j3.d, e = j3.e, f = j3.f, v = v });

            var predicate = PredicateBuilder.New<JoinEntity>();

            predicate = predicate.And(x => x.a.CHA_TIPO_URP.Equals("R") && !x.a.DTA_FINE.HasValue);
            predicate = predicate.And(x => x.f.ID_REGISTRO == registroEntity.SYSTEM_ID);
            predicate = predicate.And(x => (x.a.CHA_DISABLED_TRASM != "1" || x.a.CHA_DISABLED_TRASM == null));
            predicate = predicate.And(x => x.v.CHA_NOTIFICA.Equals("1"));
            if (string.IsNullOrEmpty(mailAddress) && interopNoMail != null && interopNoMail != "0")
                predicate = predicate.And(x => string.IsNullOrEmpty(x.v.VAR_EMAIL_REGISTRO));
            else if (!string.IsNullOrEmpty(mailAddress))
                predicate = predicate.And(x => x.v.VAR_EMAIL_REGISTRO.Equals(mailAddress) || string.IsNullOrEmpty(x.v.VAR_EMAIL_REGISTRO));
            var resultRuoloFun = await j4.Where(predicate)
                .Select(x => new
                {
                    SYSTEM_ID = x.a.SYSTEM_ID,
                    VAR_CODICE = x.a.VAR_CODICE,
                    VAR_COD_RUBRICA = x.a.VAR_COD_RUBRICA,
                    VAR_DESC_RUOLO = x.d.VAR_DESC_RUOLO,
                    ID_GRUPPO = x.a.ID_GRUPPO,
                    ID_UO = x.a.ID_UO,
                    NUM_LIVELLO = x.d.NUM_LIVELLO,
                    VAR_DESC_CORR = x.e.VAR_DESC_CORR
                })
                .ToListAsync();

            foreach (var item in resultRuoloFun)
            {
                ruoliDestTrasm.Add(new Ruolo()
                {
                    systemId = item.SYSTEM_ID.ToString(),
                    codiceCorrispondente = item.VAR_CODICE,
                    codiceRubrica = item.VAR_COD_RUBRICA,
                    descrizione = string.Concat(item.VAR_DESC_RUOLO, " ", item.VAR_DESC_CORR),
                    livello = item.NUM_LIVELLO.ToString(),
                    idGruppo = item.ID_GRUPPO.ToString(),
                    uo = new DocsPaVO.utente.UnitaOrganizzativa() { systemId = item.ID_UO.ToString() },
                    tipoCorrispondente = "R"
                });
            }

            return ruoliDestTrasm;
        }
        private async Task<bool> InsertAssDocAddress(long docNumber, long idRegistro, string mailAddress)
        {
            var assDocMailInterop = new AssDocMailInteropEntity()
            {
                ID_PROFILE = docNumber,
                ID_REGISTRO = idRegistro,
                VAR_EMAIL_REGISTRO = mailAddress
            };

            await this._dbContext.AssDocMailInteropEntities.AddAsync(assDocMailInterop);
            return (await ((DbContext)this._dbContext).SaveChangesAsync()) == 1;

        }
        private async Task<DocsPaVO.utente.Corrispondente?> AddNewCorrispondente(SoggettoType soggetto, string mailMitt, IdentificatoreType identificatore, DocsPaVO.utente.Registro reg)
        {
            string tempCodRubr = "";
            //query di insert
            System.Collections.ArrayList insertList = new System.Collections.ArrayList();
            //parametri comuni

            string codiceAOO = identificatore.CodiceAOO.Value.Trim();
            //Il campo di DB ha una lunghezza massima di 16 caratteri - CodiceAOO non può essere più lungo di questo valore
            if (codiceAOO.Length > 16)
                codiceAOO = codiceAOO.Substring(0, 16);

            string codiceAmm = identificatore.CodiceAmministrazione.Value.Trim();
            string tipoIE = "E";
            string tipoCorr = "S";
            string idParent = "0";
            bool transBegun = false;
            string codEdesc = string.Empty;
            string descAmm = string.Empty;
            string descrizioneAmm = string.Empty;

            Type t = soggetto.Item.GetType();
            switch (t.Name)
            {
                case "PersonaFisicaType":
                    break;
                case "PersonaGiuridicaType":
                    break;
                case "AmministrazioneType":
                    AmministrazioneType amministrazione = soggetto.Item as AmministrazioneType;
                    descrizioneAmm = amministrazione.DenominazioneAmministrazione.Trim();
                    break;
                case "AmministrazioneEsteraType":
                    break;
            }

            string descrInterAmm = null;
            if (descrizioneAmm.Length >= 7)
            {
                descrInterAmm = descrizioneAmm.Substring(0, 7);
            }
            else
            {
                descrInterAmm = descrizioneAmm;
            }

            transBegun = true;
            //modofica
            if (!string.IsNullOrEmpty(codiceAmm))
                codEdesc = codiceAmm + " - ";

            if (!string.IsNullOrEmpty(descrizioneAmm))
                codEdesc = codEdesc + descrizioneAmm + " - ";

            long sysId = await this.AddNewCorrAllegato6(tipoIE, reg, mailMitt, descrizioneAmm, codiceAmm, tipoCorr, codiceAOO, descrInterAmm, codEdesc);

            string codRubricaAmm = Resources.InteropPrefix + sysId.ToString();

            //Emanuela 14/04/2014: se la descrizione nella segnatura non è specificata, mette nel campo descrizione nella dpa_corr_globali
            //il codice rubrica(per impedire che venga inserito un valore vuoto)
            /* VALORE CHIAVE BE_RIC_MITT_INTEROP_BY_MAIL_DESC = 0 in PiTRE quindi questo pezzo non va fatto
            string valorechiave = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_RIC_MITT_INTEROP_BY_MAIL_DESC"); //In PiTre è spenta
            bool updateDescCorr = false;
            if (descrizioneAmm.Equals(string.Empty) && (valorechiave != null && valorechiave.Equals("0")))
                updateDescCorr = true;
            */

            var corrGlobaliInterop = await this._dbContext.CorrGlobaliEntities.Where(x => x.SYSTEM_ID == sysId).FirstOrDefaultAsync();
            corrGlobaliInterop.VAR_COD_RUBRICA = codRubricaAmm;

            //si inserisce il dettaglio su dpa_dett_globali
            //e il canale preferenziale sulla dpa_t_canale_corr
            var dettGlobaliEntity = new DettGlobaliEntity()
            {
                ID_CORR_GLOBALI = sysId,
            };

            await this._dbContext.DettGlobaliEntities.AddAsync(dettGlobaliEntity);
            int rowsInserted = await ((DbContext)_dbContext).SaveChangesAsync();
            if (rowsInserted == 0)
                _logger.LogError(string.Format("Riga in DPA_DETT_GLOBALI non inserita per corr globale con id {0}", sysId));


            //si setta l'id parent
            idParent = sysId.ToString();
            //si setta il codice rubrica temporaneo
            tempCodRubr = Resources.InteropPrefix + sysId;
            //si risparmiano ulteriori select: non sono necessarie, le UO bisogna metterle da zero!

            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
            qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.ESTERNO;
            qco.codiceRubrica = tempCodRubr;

            qco.idAmministrazione = reg.idAmministrazione;
            System.Collections.ArrayList idReg = new System.Collections.ArrayList();
            idReg.Add(reg.systemId);
            qco.idRegistri = new string[1] { reg.systemId };
            var resQuery = (await this._mediator.Send(new Requests.AddressbookGetListaCorrispondenti(
                                qco

                               ))).output;
            //System.Collections.ArrayList resQuery = null;//BusinessLogic.Utenti.addressBookManager.listaCorrispondentiEstMethod(qco);

            if (!string.IsNullOrEmpty(codiceAmm))
                ((DocsPaVO.utente.UnitaOrganizzativa)resQuery[0]).codDescAmministrizazione = codiceAmm + " - ";

            if (!string.IsNullOrEmpty(descAmm))
                ((DocsPaVO.utente.UnitaOrganizzativa)resQuery[0]).codDescAmministrizazione = descAmm + " - ";

            return ((DocsPaVO.utente.UnitaOrganizzativa)resQuery[0]);
        }
        private async Task<long> AddNewCorrAllegato6(string tipoIE, DocsPaVO.utente.Registro reg, string mailMitt, string descrizioneAmm, string codiceAmm, string tipoCorr, string codiceAOO, string descrInterAmm, string codEdesc)
        {
            var corrGlobaliEntity = new CorrGlobaliEntity()
            {
                VAR_EMAIL = mailMitt,
                COD_DESC_INTEROP = !string.IsNullOrEmpty(codEdesc) ? ReplaceApexes(codEdesc) : null,
                NUM_LIVELLO = 0,
                CHA_TIPO_IE = tipoIE,
                ID_REGISTRO = reg.systemId.AsLong(),
                ID_AMM = reg.idAmministrazione.AsLong(),
                VAR_DESC_CORR = ReplaceApexes(descrizioneAmm),
                ID_OLD = 0,
                //SU VECCHIO BE ERA QUESTA, MA NON VA:            DTA_INIZIO = System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", new CultureInfo("en-US")).AsDateTime(),
                DTA_INIZIO = await _dbContext.GetSystemDateTime(),
                ID_PARENT = 0,
                VAR_CODICE_AMM = codiceAmm,
                CHA_TIPO_CORR = tipoCorr,
                CHA_TIPO_URP = "U",
                CHA_PA = "1",
                VAR_CODICE_AOO = codiceAOO,
                VAR_COD_RUBRICA = Resources.InteropPrefix + ReplaceApexes(descrInterAmm)
            };

            await this._dbContext.CorrGlobaliEntities.AddAsync(corrGlobaliEntity);
            bool rowsInserted = (await ((DbContext)_dbContext).SaveChangesAsync()) > 0;

            return rowsInserted && (corrGlobaliEntity.SYSTEM_ID != null || corrGlobaliEntity.SYSTEM_ID != 0) ? corrGlobaliEntity.SYSTEM_ID : 0;
        }
        private async Task<(DocsPaVO.utente.Corrispondente, string)> GetMittente(SoggettoType mittente, string mailAddress, string codiceAmm, string codiceAOO, DocsPaVO.addressbook.TipoUtente tipoUtente, DocsPaVO.utente.Registro reg, string rows)
        {
            rows = "";
            //costruzione oggetto corrispondente	
            DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
            qco.tipoUtente = tipoUtente;
            qco.idAmministrazione = reg.idAmministrazione;
            System.Collections.ArrayList registri = new System.Collections.ArrayList();
            registri.Add(reg.systemId);
            qco.idRegistri = new string[1] { reg.systemId }; // (string[])registri.ToArray();
            //luluciani 3.10.10
            qco.fineValidita = true;
            /****************/
            string Amministrazione = string.Empty;
            var idGroup = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            var ruolo = await this._dbContext.CorrGlobaliEntities.FirstAsync(x => x.ID_GRUPPO == idGroup);
            var idRole = ruolo?.SYSTEM_ID;

            Type t = mittente.Item.GetType();
            switch (t.Name)
            {
                case "PersonaFisicaType":
                    PersonaFisicaType persona = mittente.Item as PersonaFisicaType;
                    qco.codiceUO = codiceAmm;
                    qco.cognomeUtente = persona.Cognome.Trim();
                    qco.nomeUtente = persona.Nome.Trim();
                    qco.email = mailAddress;
                    break;
                case "PersonaGiuridicaType":
                    PersonaGiuridicaType personaGiuridica = mittente.Item as PersonaGiuridicaType;
                    qco.codiceUO = codiceAmm;
                    qco.descrizioneUO = personaGiuridica.Denominazione.Trim();
                    qco.email = mailAddress;
                    break;
                case "AmministrazioneType":
                    AmministrazioneType amministrazione = mittente.Item as AmministrazioneType;
                    qco.descrizioneUO = amministrazione.DenominazioneAmministrazione.Trim();
                    Amministrazione = amministrazione.DenominazioneAmministrazione.Trim();
                    qco.email = mailAddress;
                    break;
                case "AmministrazioneEsteraType":
                    AmministrazioneEsteraType amministrazioneEstera = mittente.Item as AmministrazioneEsteraType;
                    qco.descrizioneUO = amministrazioneEstera.DenominazioneAmministrazione.Trim();
                    qco.email = mailAddress;
                    break;
            }

            //chiamata metodo addressboook
            List<DocsPaVO.utente.Corrispondente> risultatiRicerca;
            string descrizone = qco.descrizioneUO;
            if (tipoUtente == DocsPaVO.addressbook.TipoUtente.ESTERNO)
            {

                var Cfg_InteroperSegnaturaSoloUO = await this._configurationService.GetValue<string>("INTEROP_SEGNATURA_SOLO_UO");
                if (!string.IsNullOrEmpty(Cfg_InteroperSegnaturaSoloUO) && Cfg_InteroperSegnaturaSoloUO == "1") //Su PiTre è accesa
                {
                    //allora ricerco solo UO
                    qco.descrizioneRuolo = null;
                    qco.cognomeUtente = null;
                    qco.nomeUtente = null;
                }
                else
                {
                    if (!string.IsNullOrEmpty(qco.descrizioneUO))
                        descrizone = qco.descrizioneUO;
                    else
                        if (!string.IsNullOrEmpty(qco.descrizioneRuolo))
                        descrizone = qco.descrizioneRuolo;
                    else
                            if (!string.IsNullOrEmpty(qco.cognomeUtente) && !string.IsNullOrEmpty(qco.nomeUtente))
                        descrizone = qco.cognomeUtente + " " + qco.nomeUtente;
                }

                if (string.IsNullOrEmpty(qco.email))
                    qco.email = mailAddress;

                //Caso particolare, segnatura xml senza unità organizzativa (enac)
                if (string.IsNullOrEmpty(descrizone))
                {
                    if (!string.IsNullOrEmpty(Amministrazione))
                    {
                        descrizone = Amministrazione;
                    }
                    else
                    {
                        _logger.LogError("segnatura xml senza unità organizzativa: ATTENZIONE CASO NON GESTITO, RISOLVERE!!!");
                    }
                }

                DocsPaVO.utente.Corrispondente corrispondente = null;
                var valorechiave = await this._configurationService.GetValue<string>("BE_RIC_MITT_INTEROP_BY_MAIL_DESC");

                //Su PiTre la chiave è spenta, quindi si cerca solo per MAIL
                //RICERCA SOLO PER MAIL
                corrispondente = await this.GetCorrispondenteByEmailCodiceAmmCodiceAoo(qco.idAmministrazione.AsLong(), qco.email, reg.systemId.AsLong(), codiceAmm, codiceAOO);

                //Se non trovo niente nel registro selezionato, cerco in tutte le rubriche visibili al ruolo
                if (corrispondente == null || corrispondente.codiceRubrica.Contains("@") || (corrispondente.codiceRubrica.Length > 8 && corrispondente.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_")))
                {
                    List<DocsPaVO.utente.Registro> registriRuolo = (await this._mediator.Send(new Requests.UtenteGetRegistriWithRf(
                        idRole.ToString(),
                        string.Empty,
                        string.Empty,
                        false
                        ))).output.ToList();
                    DocsPaVO.utente.Corrispondente corrispondenteTemp = null;
                    foreach (var registro in registriRuolo)
                    {
                        if (!registro.systemId.Equals(reg.systemId))
                        {
                            corrispondenteTemp = await this.GetCorrispondenteByEmailCodiceAmmCodiceAoo(qco.idAmministrazione.AsLong(), qco.email, registro.systemId.AsLong(), codiceAmm, codiceAOO);
                            if (corrispondenteTemp != null && (!corrispondenteTemp.codiceRubrica.Contains("@") && (corrispondenteTemp.codiceRubrica.Length < 8 || !corrispondenteTemp.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_"))))
                            {
                                corrispondente = corrispondenteTemp;
                                break;
                            }
                        }
                    }
                }

                //FINE MODIFICA LULUCIANI
                if (corrispondente != null)
                {
                    if (!string.IsNullOrEmpty(codiceAmm))
                        corrispondente.codDescAmministrizazione = codiceAmm + "-";

                    if (!string.IsNullOrEmpty(Amministrazione))
                        corrispondente.codDescAmministrizazione = Amministrazione + "-";
                    risultatiRicerca = new List<DocsPaVO.utente.Corrispondente>() { corrispondente };
                }
                else
                    risultatiRicerca = new List<DocsPaVO.utente.Corrispondente>();
            }
            else
            {
                qco.tipoUtente = TipoUtente.INTERNO;
                risultatiRicerca = (await this._mediator.Send(new Requests.AddressbookGetListaCorrispondenti(qco))).output.ToList();
            }
            for (int i = 0; i < risultatiRicerca.Count; i++)
            {
                var Cfg_InteroperSegnaturaSoloUO = await this._configurationService.GetValue<string>("INTEROP_SEGNATURA_SOLO_UO");
                if (!string.IsNullOrEmpty(Cfg_InteroperSegnaturaSoloUO) && Cfg_InteroperSegnaturaSoloUO.Equals("1"))  //Su PiTre è accesa
                    if (tipoUtente == DocsPaVO.addressbook.TipoUtente.ESTERNO)
                        return ((DocsPaVO.utente.Corrispondente)risultatiRicerca[0], rows);
                    else if (IsCorrectMittente((DocsPaVO.utente.Corrispondente)risultatiRicerca[i], mittente))
                        return ((DocsPaVO.utente.Corrispondente)risultatiRicerca[i], rows);
            }
            return (null, rows);
        }
        private static bool IsCorrectMittente(DocsPaVO.utente.Corrispondente mittente, SoggettoType soggetto)
        {
            System.Collections.ArrayList uoGerarchia = new System.Collections.ArrayList();
            DocsPaVO.utente.UnitaOrganizzativa uo = null;
            string strAmmXml = string.Empty;
            Type t = soggetto.Item.GetType();
            switch (t.Name)
            {
                case "PersonaFisicaType":
                    PersonaFisicaType persona = soggetto.Item as PersonaFisicaType;

                    break;
                case "PersonaGiuridicaType":
                    PersonaGiuridicaType personaGiuridica = soggetto.Item as PersonaGiuridicaType;
                    break;
                case "AmministrazioneType":
                    AmministrazioneType amministrazione = soggetto.Item as AmministrazioneType;
                    strAmmXml = amministrazione.DenominazioneAmministrazione.Trim();
                    break;
                case "AmministrazioneEsteraType":
                    AmministrazioneEsteraType amministrazioneEstera = soggetto.Item as AmministrazioneEsteraType;
                    strAmmXml = amministrazioneEstera.DenominazioneAmministrazione.Trim();
                    break;
            }

            if (mittente.GetType().Equals(typeof(DocsPaVO.utente.Ruolo)))
            {
                uo = ((DocsPaVO.utente.Ruolo)mittente).uo;
            }
            else
                if (mittente.GetType().Equals(typeof(DocsPaVO.utente.Utente)))
            {
                uo = ((DocsPaVO.utente.Ruolo)((DocsPaVO.utente.Utente)mittente).ruoli[0]).uo;
            }
            else
                    if (mittente.GetType().Equals(typeof(DocsPaVO.utente.UnitaOrganizzativa)))
            {
                uo = (DocsPaVO.utente.UnitaOrganizzativa)mittente;
            }
            while (uo != null)
            {
                uoGerarchia.Add(uo.descrizione);
                uo = uo.parent;
            }
            //ora si fa il matching
            string strAmm = ((string)uoGerarchia[uoGerarchia.Count - 1]).ToUpper().Trim();

            if (!strAmm.Equals(strAmmXml)) return false;
            string strUo;
            string strUoXml;
            for (int i = uoGerarchia.Count - 1; i >= 0; i--)
            {
                strUo = ((string)uoGerarchia[i]).ToUpper().Trim();

                //verifica delle uo tra il file xml e l'organigramma
                if (!strUo.Equals(strAmmXml)) return false;
            }
            return true;
        }
        private async Task<DocsPaVO.utente.Corrispondente?> GetCorrispondenteByEmailCodiceAmmCodiceAoo(long id_amm, string mail, long id_reg, string codiceAmm, string codiceAoo)
        {
            DocsPaVO.utente.Corrispondente corr = null;
            try
            {
                var u1 = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(x => (x.VAR_EMAIL.ToUpper().Equals(mail.ToUpper()) &&
                            x.CHA_TIPO_IE.ToUpper().Equals("I")) &&
                            (x.ID_AMM == id_amm || x.ID_AMM == null) &&
                            (x.ID_REGISTRO == null || x.ID_REGISTRO == id_reg) &&
                            x.DTA_FINE == null)
                        .Select(x => new
                        {
                            SYSTEM_ID = x.SYSTEM_ID,
                            VAR_COD_RUBRICA = x.VAR_COD_RUBRICA,
                            CHA_TIPO_URP = x.CHA_TIPO_URP,
                            ID_GRUPPO = x.ID_GRUPPO,
                            ID_PEOPLE = x.ID_PEOPLE,
                            CHA_TIPO_IE = x.CHA_TIPO_IE,
                            CHA_TIPO_CORR = x.CHA_TIPO_CORR,
                            VAR_CODICE_AMM = x.VAR_CODICE_AMM,
                            VAR_CODICE_AOO = x.VAR_CODICE_AOO
                        });

                var u2 = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Join(this._dbContext.MailCorrEsterniEntities.AsNoTracking(),
                        a => a.SYSTEM_ID,
                        m => m.ID_CORR,
                        (a, m) => new { a, m })
                    .Where(x => (x.m.VAR_EMAIL.ToUpper().Equals(mail.ToUpper())) &&
                        (x.a.ID_AMM == id_amm || x.a.ID_AMM == null) &&
                        (x.a.ID_REGISTRO == null || x.a.ID_REGISTRO == id_reg) &&
                        x.a.DTA_FINE == null)
                    .Select(x => new
                    {
                        SYSTEM_ID = x.a.SYSTEM_ID,
                        VAR_COD_RUBRICA = x.a.VAR_COD_RUBRICA,
                        CHA_TIPO_URP = x.a.CHA_TIPO_URP,
                        ID_GRUPPO = x.a.ID_GRUPPO,
                        ID_PEOPLE = x.a.ID_PEOPLE,
                        CHA_TIPO_IE = x.a.CHA_TIPO_IE,
                        CHA_TIPO_CORR = x.a.CHA_TIPO_CORR,
                        VAR_CODICE_AMM = x.a.VAR_CODICE_AMM,
                        VAR_CODICE_AOO = x.a.VAR_CODICE_AOO
                    });

                var data = await u1.Union(u2).ToListAsync();

                var corr1 = (from row in data
                             where !string.IsNullOrEmpty(row.CHA_TIPO_CORR) && row.CHA_TIPO_CORR.Equals("C") &&
                                row.VAR_CODICE_AMM != null && row.VAR_CODICE_AMM.ToUpper().Equals(codiceAmm.ToUpper()) &&
                                row.VAR_CODICE_AOO != null && row.VAR_CODICE_AOO.ToUpper().Equals(codiceAoo.ToUpper())
                             select row).FirstOrDefault();

                if (corr1 == null)
                {
                    corr1 = (from row in data
                             where row.VAR_CODICE_AMM != null && row.VAR_CODICE_AMM.ToUpper().Equals(codiceAmm.ToUpper()) &&
                             row.VAR_CODICE_AOO != null && row.VAR_CODICE_AOO.ToUpper().Equals(codiceAoo.ToUpper()) &&
                             (!row.VAR_COD_RUBRICA.Contains("@") &&
                                (row.VAR_COD_RUBRICA.Length < 8 || !row.VAR_COD_RUBRICA.Substring(0, 8).ToUpper().Equals("INTEROP_")))
                             select row).FirstOrDefault();
                }

                if (corr1 != null)
                {
                    string sid = corr1.SYSTEM_ID.ToString();
                    string tipo = corr1.CHA_TIPO_URP;
                    string id_gruppo = corr1.ID_GRUPPO.ToString();
                    string id_people = corr1.ID_PEOPLE.ToString();
                    bool isInterno = corr1.CHA_TIPO_IE != null && corr1.CHA_TIPO_IE.Equals("I");
                    DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                    qco.systemId = sid;
                    qco.idAmministrazione = id_amm.ToString();
                    qco.fineValidita = true;

                    var tipoIE = new DocsPaVO.addressbook.TipoUtente();
                    if (isInterno)
                        tipoIE = DocsPaVO.addressbook.TipoUtente.INTERNO;
                    else
                        tipoIE = DocsPaVO.addressbook.TipoUtente.ESTERNO;

                    corr = (await this._mediator.Send(new Requests.AddressbookGetCorrispondenteCompletoBySystemId(sid, tipoIE, new InfoUtente())))?.output;

                    if (corr != null && !corr.codiceRubrica.Contains("@") && (corr.codiceRubrica.Length < 8 || !corr.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_")))
                        return corr;

                }

                foreach (var d in data)
                {
                    string sid = d.SYSTEM_ID.ToString();
                    string tipo = d.CHA_TIPO_URP;
                    string id_gruppo = d.ID_GRUPPO.ToString();
                    string id_people = d.ID_PEOPLE.ToString();
                    bool isInterno = d.CHA_TIPO_IE != null && d.CHA_TIPO_IE.Equals("I");
                    DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                    qco.systemId = sid;
                    qco.idAmministrazione = id_amm.ToString();
                    qco.fineValidita = true;

                    var tipoIE = new DocsPaVO.addressbook.TipoUtente();
                    if (isInterno)
                        tipoIE = DocsPaVO.addressbook.TipoUtente.INTERNO;
                    else
                        tipoIE = DocsPaVO.addressbook.TipoUtente.ESTERNO;

                    corr = (await this._mediator.Send(new Requests.AddressbookGetCorrispondenteCompletoBySystemId(sid, tipoIE, new InfoUtente())))?.output;

                    if (corr != null && !corr.codiceRubrica.Contains("@") && (corr.codiceRubrica.Length < 8 || !corr.codiceRubrica.Substring(0, 8).ToUpper().Equals("INTEROP_")))
                        return corr;
                }

                bool confAb = (await this._mediator.Send(new Application.Requests.GetConfigurazioniRubricaComune(new InfoUtente()))).output.GestioneAbilitata;
                if (confAb)
                {
                    try
                    {
                        if (corr is null)
                        {
                            if (!string.IsNullOrEmpty(mail))
                                corr = await this.UpdateCorrispondenteByEmail(mail);
                        }
                        else if (string.IsNullOrEmpty(corr.systemId) || corr.inRubricaComune)
                        {
                            corr = await this.UpdateCorrispondente(corr.codiceRubrica);
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError($"Errore in ricerca corrispondente in rubrica comune (SegnaturaHandler): {ex.Message}");
                    }
                }

                //if (corr == null)
                //    throw new Exception(string.Format(Resources.ErroreInRicercaCorr, mail));

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return corr;
        }
        private async Task<DocsPaVO.utente.Corrispondente> UpdateCorrispondenteByEmail(string email)
        {
            string idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            bool rubEstAttive = false;

            (string value, bool keyFound) = await this._configurationService.TryGetValue<string>(idTenant, "BE_ENABLE_RUBRICHE_ESTERNE");

            if (keyFound && !string.IsNullOrEmpty(value) && value.Equals("1"))
            {
                rubEstAttive = true;
            }

            (string beKey, bool beRubFound) = await this._configurationService.TryGetValue<string>("BE_RUBRICHE_ESTERNE");


            if (rubEstAttive && beRubFound && !string.IsNullOrEmpty(beKey) && !beKey.Equals("0"))
            {
                var elemento = await this.GetElementiInRubricaCom(email, beKey);

                if (elemento != null)
                    return await this.GetDettAndUpdateCorr(elemento);
                else
                    return null;
            }

            return null;

        }
        private async Task<Services.RubricaComune.Corrispondente> GetElementiInRubricaCom(string email, string rubEsterna)
        {
            var elementiRubrica = new List<Services.RubricaComune.Corrispondente>();
            Services.RubricaComune.Corrispondente elemento = null;
            var criteriRicerca = new List<CriterioRicerca>
            {
                new CriterioRicerca 
                { 
                    Campo = CampiRicercaEnum.Email,
                    Valore = email,
                    TipoRicercaParola = TipiRicercaParolaEnum.ParolaIntera
                }
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
                    elemento = new();
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
        private async Task<(string, bool)> GetInfoDestinatari(DestinatarioType[] destinatario, bool confermaRic, string mailAddress)
        {
            string result = "";
            string indirizzo = string.Empty;
            string indirizzoDest = string.Empty;
            string descrizione = string.Empty;
            for (int i = 0; i < destinatario.Count(); i++)
            {
                indirizzo = SegnaturaManager.GetIndirizzoTelematico(destinatario[i]);
                descrizione = SegnaturaManager.GetDescrizioneDestinatario(destinatario[i]);

                if (indirizzo != null && indirizzo.Equals(mailAddress) && destinatario[i].confermaRicezione)
                {
                    indirizzoDest = indirizzo;
                    confermaRic = true;
                }
                //Se indirizzoDest è vuoto nella segnatura non era specificato  l'email del destinatario quindi invio sempre la conferma
                if (string.IsNullOrEmpty(indirizzoDest))
                    confermaRic = true;

                result = result + "DESTINATARIO " + (i + 1) + ": " + descrizione + " Indirizzo: " + indirizzo + "/ ";
            }

            return (result, confermaRic);
        }
        #endregion

        #region Mail
        private async Task<bool> MailElaborata(string idMessage, string ragione)
        {
            var mailElaborataEntity = new MailElaborataEntity()
            {
                VAR_MESSAGE = idMessage.Replace("'", "''"),
                CHA_RAGIONE_ELAB = ragione,
                DTA_ELAB = DateTime.Now,
                ID_REGISTRO = null,
                ID_PROFILE = null,
                VAR_EMAIL = null
            };

            await this._dbContext.MailElaborataEntities.AddAsync(mailElaborataEntity);
            return await ((DbContext)this._dbContext).SaveChangesAsync() == 1;
        }

        private async Task<bool> MailElaborata(string idMessage, string ragione, long idRegistro, long docnumber, string email)
        {
            var mailElaborataEntity = new MailElaborataEntity()
            {
                VAR_MESSAGE = idMessage.Replace("'", "''"),
                CHA_RAGIONE_ELAB = ragione,
                DTA_ELAB = DateTime.Now,
                ID_REGISTRO = idRegistro,
                ID_PROFILE = docnumber,
                VAR_EMAIL = email
            };

            await this._dbContext.MailElaborataEntities.AddAsync(mailElaborataEntity);
            return await ((DbContext)this._dbContext).SaveChangesAsync() == 1;
        }
        private async Task<bool> MantieniMailRicevutePendenti(long registerId, string mailAddress)
        {
            var entities = await this._dbContext.MailRegistriEntities
                .Where(x => x.ID_REGISTRO == registerId && x.VAR_EMAIL_REGISTRO.Equals(mailAddress))
                .Select(x => new
                {
                    x.VAR_MAIL_RIC_PENDENTE,
                    x.VAR_SOLO_MAIL_PEC
                }).ToListAsync();

            return entities.Any(x => x.VAR_SOLO_MAIL_PEC != null && !x.VAR_SOLO_MAIL_PEC.Equals("1") && x.VAR_MAIL_RIC_PENDENTE != null && x.VAR_MAIL_RIC_PENDENTE.Equals("1"));
        }
        #endregion

        #region Documento
        private string GetFileName(string fileName)
        {
            string res = null;
            if (fileName.Substring(fileName.LastIndexOf(".") + 1).ToUpper().Equals("P7M"))
            {
                res = fileName.Substring(0, fileName.LastIndexOf("."));
            }
            else
            {
                res = fileName;
            }
            return res;
        }
        private async System.Threading.Tasks.Task TrashDocument(long docnumber)
        {
            var profileEntity = await this._dbContext.ProfileEntities.FirstAsync(x => x.DOCNUMBER == docnumber);

            profileEntity.CHA_IN_CESTINO = "1";

            var attachmentList = await this._dbContext.ProfileEntities
                .Where(x => x.ID_DOCUMENTO_PRINCIPALE == docnumber)
                .ToListAsync();

            attachmentList.ForEach(a =>
            {
                a.CHA_IN_CESTINO = "1";
            });

            await ((DbContext)this._dbContext).SaveChangesAsync();
        }
        private async Task<bool> InsertApp(string? estensione)
        {
            var result = false;

            try
            {
                if (!string.IsNullOrEmpty(estensione) && !await _dbContext.AppEntities.AnyAsync(a => a.APPLICATION.ToUpper() == "GEN_" + estensione.ToUpper()))
                {
                    var appEntity = new AppEntity()
                    {
                        APPLICATION = "GEN_" + estensione,
                        DESCRIPTION = "GEN_" + estensione,
                        FILING_SCHEME = 2,
                        DEFAULT_EXTENSION = estensione
                    };

                    await this._dbContext.AppEntities.AddAsync(appEntity);
                    result = await ((DbContext)this._dbContext).SaveChangesAsync() == 1;
                }
            }
            catch(Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);

                var pendingAppEntity = ((DbContext)_dbContext).ChangeTracker.Entries<AppEntity>()
                    .Where(a => a.State == EntityState.Added)
                    .FirstOrDefault();

                if(pendingAppEntity != null)
                    pendingAppEntity.State = EntityState.Detached;
            }

            return result;
        }
        private async Task<DocsPaVO.utente.Registro> CaricaRegistroInScheda(DocsPaVO.utente.Registro registro)
        {
            DocsPaVO.utente.Registro reg = registro;

            //se è un RF allora ricerco il registro relativo alla Aoo Collegata
            if (registro.chaRF != null && registro.chaRF == "1")
                reg = (await this._mediator.Send(new GetRegistroBySistemId(registro.idAOOCollegata))).output;

            return reg;
        }

        private async Task<DocsPaVO.documento.Applicazione> GetApp(string filename)
        {
            char[] dot = { '.' };
            string[] parts = filename.Split(dot);
            string suffix = parts[parts.Length - 1];
            if (suffix.ToUpper().Equals("P7M"))
                _logger.LogDebug("File p7m");

            _logger.LogDebug("Suffisso:" + suffix);
            DocsPaVO.documento.Applicazione res = null;

            System.Collections.ArrayList result = new System.Collections.ArrayList();
            System.Collections.ArrayList apps = await this.GetApplicazioni(suffix, result);
            _logger.LogDebug("App:" + apps.Count);

            if (apps.Count > 0)
                res = (DocsPaVO.documento.Applicazione)apps[0];
            return res;
        }

        private async Task<ArrayList> GetApplicazioni(string estensione, ArrayList res)
        {
            var apps = await this._dbContext.AppEntities
                .Where(x => x.DEFAULT_EXTENSION.ToUpper().Equals(estensione.ToUpper()))
                .ToListAsync();

            foreach (var app in apps)
                res.Add(this._mapper.Map<Applicazione>(app));

            if (apps.Count == 0)
            {
                if (await InsertApp(estensione))
                    return await this.GetApplicazioni(estensione, res);
                else
                    return res;
            }
            else
                return res;
        }
        #endregion

        #region Utilities
        private static t DeserializeObject<t>(String pXmlizedString)
        {
            XmlSerializer xs = new XmlSerializer(typeof(t));
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

            XmlTextReader reader = new XmlTextReader(new StringReader(pXmlizedString));
            try
            {
                return (t)xs.Deserialize(reader);
            }
            catch (Exception e) { System.Console.WriteLine(e); return default(t); }
        }

        private string ReplaceApexes(string sourceString)
        {
            return !string.IsNullOrEmpty(sourceString) ? sourceString.Replace("'", "''") : null;
        }
        static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }
        private async Task<string> GetSepSegnatura(long idAmministrazione)
        {
            return await this._dbContext.AmministraEntities.Where(x => x.SYSTEM_ID == idAmministrazione).Select(x => x.CHA_STR_SEGNATURA).FirstOrDefaultAsync() ?? Resources.SeparatoreAmm;
        }
        #endregion

        #region Marca temporale
        //private async Task<bool> FindTSRMatch(List<EmailContentAttachment> attachments, string tsrFileName)
        //{
        //    bool retval = false;
        //    if (!Path.GetExtension(tsrFileName).ToLowerInvariant().Contains("tsr"))
        //        return false;

        //    byte[] tsrFile = attachments.Where(x => x.FileName.Equals(tsrFileName)).Select(x => x.Content).FirstOrDefault();

        //    var Files = attachments.Select(x => x.Content).ToArray();

        //    foreach(var file in Files)
        //    {

        //    }
        //}

        private async Task<bool> MatchTSR(Core.Services.Email.BoxScanner.Email email, DocsPaVO.documento.FileRequest fr, FileDocumento fd, long idPeople)
        {
            bool retval = false;
            string[] tsrFiles = email.Attachments.Where(x => x.FileName.ToLowerInvariant().EndsWith(".tsr")).Select(x => x.FileName).ToArray();
            //Se non ci sono TSR
            if (tsrFiles.Length == 0)
                return false;

            //Il file è un TSR, non associo un TSR a un TSR
            if (System.IO.Path.GetExtension(fr.fileName).ToLowerInvariant() == ".tsr")
                return false;

            string fdFileName = fr.fileName;
            foreach (string file in tsrFiles)
            {
                byte[] tsrFile;
                if (fdFileName.ToLowerInvariant().Equals(file.ToLowerInvariant()))
                    tsrFile = fd.content;
                else
                    tsrFile = email.Attachments.Where(x => x.FileName.Equals(file)).Select(x => x.Content).FirstOrDefault();

                //if(await this.ConfrontaTSR(tsrFile, fd.content))
                //{
                DocsPaVO.areaConservazione.OutputResponseMarca resultMarca = await this.VerificaMarca(fd.content, tsrFile);
                if (resultMarca == null)
                    //throw new TimestampNullPi3Exception();
                    this._logger.LogError("ExecuteAndSaveTSR > Marca nulla");

                //Salvo la marca generata sul database
                var tsEntityToInsert = new TimestampDocEntity()
                {
                    DOC_NUMBER = fr.docNumber.AsLong(),
                    VERSION_ID = fr.versionId.AsLong(),
                    ID_PEOPLE = idPeople,
                    DTA_CREAZIONE = !string.IsNullOrEmpty(resultMarca.docm_date) ? resultMarca.docm_date.AsDateTime() : null,
                    DTA_SCADENZA = !string.IsNullOrEmpty(resultMarca.dsm) ? resultMarca.dsm.AsDateTime() : null,
                    NUM_SERIE = resultMarca.sernum,
                    S_N_CERTIFICATO = resultMarca.snCertificato,
                    ALG_HASH = resultMarca.algHash,
                    SOGGETTO = resultMarca.TSA.O,
                    PAESE = resultMarca.TSA.C,
                    TSR_FILE = resultMarca.marca
                };

                await this._dbContext.TimestampDocEntities.AddAsync(tsEntityToInsert);
                int rowsInserted = await ((DbContext)_dbContext).SaveChangesAsync();

                if (rowsInserted == 0)
                    this._logger.LogError("SegnaturaHandler > Errore nell'inserimento della marca temporale nella DPA_TIMESTAMP_DOC");
                else
                    retval = true;
                //}
            }
            return retval;
        }

        //private async Task<bool> ConfrontaTSR(byte[]? tsrFile, byte[] content) //SERVE?
        //{
        //    string algorithm = string.Empty;

        //    DocsPaVO.areaConservazione.OutputResponseMarca resultMarca = await this.VerificaMarca(content, tsrFile);

        //    if(resultMarca != null)
        //    {
        //        algorithm = resultMarca.algHash;
        //        byte[] contentHash = Org.BouncyCastle.Security.DigestUtilities.CalculateDigest(algorithm, fileToMatch);
        //    }

        //}
        private async Task<OutputResponseMarca> VerificaMarca(byte[] p7m, byte[] tSR)
        {
            OutputResponseMarca outTSR = null;
            var verificaMarcaResponse = await this._firmaDigitale2Service.Verifica(new VerificaRequest()
            {
                VerificaCompleta = false,
                FileFirmato = tSR,
                FileOriginale = p7m,
                DataVerifica = DateTime.Now,
                TipoVerifica = TipiVerifica.Incapsulata,
                ReturnFileOriginale = true,
                ReturnXmlCompleto = true,
            });

            if (verificaMarcaResponse != null)
            {
                outTSR = new OutputResponseMarca();
                XmlDocument doc = new XmlDocument();

                if (verificaMarcaResponse.Esito != null && verificaMarcaResponse.Esito.DatiGeneraliVerifica != null)
                    doc.LoadXml(verificaMarcaResponse.Esito.DatiGeneraliVerifica);
                else if (verificaMarcaResponse.Warning != null &&
                        verificaMarcaResponse.Warning.WarningFault != null &&
                        verificaMarcaResponse.Warning.WarningFault.Length != 0 &&
                        verificaMarcaResponse.Warning.DettaglioFirmaDigitale.FileMarcato &&
                        verificaMarcaResponse.Warning.DettaglioFirmaDigitale != null &&
                        verificaMarcaResponse.Warning.DettaglioFirmaDigitale.DatiGeneraliVerifica != null)
                {
                    doc.LoadXml(verificaMarcaResponse.Warning.DettaglioFirmaDigitale.DatiGeneraliVerifica);
                    for (int i = 0; i < verificaMarcaResponse.Warning.WarningFault.Length; i++)
                        outTSR.descrizioneErrore += !string.IsNullOrEmpty(outTSR.descrizioneErrore) ?
                            string.Concat(outTSR.descrizioneErrore, " - ", verificaMarcaResponse.Warning.WarningFault[i].ErrorMsg)
                            : verificaMarcaResponse.Warning.WarningFault[i].ErrorMsg;
                }

                if (doc != null)
                {
                    XmlNode node = doc.DocumentElement;
                    XmlNode timestampNode = node.SelectSingleNode("/deSign/timeStamp");
                    XmlNode certNode = timestampNode.SelectSingleNode("certificate");

                    var certBytes = Encoding.UTF8.GetBytes(certNode.InnerText);
                    var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(certBytes);

                    if (DateTime.Now.CompareTo(cert.NotAfter.ToLocalTime()) > 0)
                        outTSR.descrizioneErrore = Resources.ElapsedTimestamp;

                    outTSR.dsm = cert.NotAfter.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
                    outTSR.sernum = int.Parse(timestampNode.SelectSingleNode("timeStampSerial")?.InnerText, System.Globalization.NumberStyles.HexNumber).ToString() ?? string.Empty;
                    string hexHash = BitConverter.ToString(p7m.ComputeHashAsSha256()).Replace("-", "").ToLowerInvariant();
                    outTSR.fhash = hexHash;
                    if (!string.IsNullOrEmpty(timestampNode.SelectSingleNode("verificationTime")?.InnerText))
                    {
                        outTSR.docm = DateTime.ParseExact(timestampNode.SelectSingleNode("verificationTime")?.InnerText, "yyMMddHHmmssZ", CultureInfo.InvariantCulture).ToString("HH:mm:ss");
                        outTSR.docm_date = DateTime.ParseExact(timestampNode.SelectSingleNode("verificationTime")?.InnerText, "yyMMddHHmmssZ", CultureInfo.InvariantCulture).AsDateTimeFormat();
                    }
                    outTSR.marca = Convert.ToBase64String(tSR); //BitConverter.ToString(tSR.ComputeHashAsSha256()).Replace("-", "").ToLowerInvariant(); //Convert.ToBase64String(ParseHex(timestampNode.SelectSingleNode("timeStampImprint")?.InnerText));
                    outTSR.fromDate = cert.NotBefore.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss");
                    outTSR.snCertificato = int.Parse(cert.SerialNumber, System.Globalization.NumberStyles.HexNumber).ToString();
                    outTSR.TSA = new TSARFC2253()
                    {
                        TSARFC2253Name = String.Format("CN={0},OU={1},O={2},C={3}",
                            timestampNode.SelectSingleNode("issuer/CN")?.InnerText,
                            timestampNode.SelectSingleNode("issuer/OU")?.InnerText,
                            timestampNode.SelectSingleNode("issuer/O")?.InnerText,
                            timestampNode.SelectSingleNode("issuer/C")?.InnerText),
                        C = timestampNode.SelectSingleNode("issuer/C")?.InnerText,
                        CN = timestampNode.SelectSingleNode("issuer/CN")?.InnerText,
                        O = timestampNode.SelectSingleNode("issuer/O")?.InnerText,
                        OU = timestampNode.SelectSingleNode("issuer/OU")?.InnerText
                    };
                    System.Security.Cryptography.Oid oidHash = new System.Security.Cryptography.Oid(timestampNode.SelectSingleNode("timeStampImprintAlgorithm")?.InnerText);
                    outTSR.algHash = oidHash.FriendlyName;
                    outTSR.esito = "OK";
                }
            }
            return outTSR;
        }
        #endregion
        #endregion
    }

    internal class InfoRegEntity
    {
        public string? VAR_CODICE { get; internal set; }
        public long SYSTEM_ID { get; internal set; }
        public long? ID_AMM { get; internal set; }
        public string? VAR_USER_MAIL { get; internal set; }
        public string? VAR_PWD_MAIL { get; internal set; }
        public string? VAR_SERVER_SMTP { get; internal set; }
        public long? NUM_PORTA_SMTP { get; internal set; }
        public string? VAR_EMAIL_REGISTRO { get; internal set; }
        public string? VAR_CODICE_AMM { get; internal set; }
        public string? VAR_USER_SMTP { get; internal set; }
        public string? CHA_STR_SEGNATURA { get; internal set; }
        public string? VAR_PWD_SMTP { get; internal set; }
        public string? CHA_POP_SSL { get; internal set; }
        public string? CHA_SMTP_SSL { get; internal set; }
        public string? CHA_SMTP_STA { get; internal set; }
        public string? VAR_SERVER_IMAP { get; internal set; }
        public long? NUM_PORTA_IMAP { get; internal set; }
        public string? VAR_TIPO_CONNESSIONE { get; internal set; }
        public string? VAR_INBOX_IMAP { get; internal set; }
        public string? VAR_BOX_MAIL_ELABORATE { get; internal set; }
        public string? VAR_MAIL_NON_ELABORATE { get; internal set; }
        public string? CHA_IMAP_SSL { get; internal set; }
        public string? VAR_SOLO_MAIL_PEC { get; internal set; }
        public long? NUM_PORTA_POP { get; internal set; }
        public string? VAR_SERVER_POP { get; internal set; }
        public string? PROVIDER_ID { get; internal set; }
        public string? MS_TENANT_ID { get; internal set; }
        public string? MS_CLIENT_ID { get; internal set; }
        public string? MS_CLIENT_SEC { get; internal set; }
        public string? MS_FOLD_TO_READ { get; internal set; }
    }
}
