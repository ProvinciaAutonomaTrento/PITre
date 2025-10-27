// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetMailRegistro;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.Factory;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Chilkat.Services.Email.Sender;
using Pi3.Infrastructure.Graph.Services.Email.Sender;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using trasmissioniSendSollecito_newWARequest = Pi3.App.Legacy.WebApi.Application.Requests.trasmissioniSendSollecito_newWA;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.trasmissioniSendSollecito_newWA
{
    public class trasmissioniSendSollecito_newWAHandler : IRequestHandler<trasmissioniSendSollecito_newWARequest, trasmissioniSendSollecito_newWAResult>
    {
        #region Public Members

        public trasmissioniSendSollecito_newWAHandler(ILogger<trasmissioniSendSollecito_newWAHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            //IEmailSenderService emailSenderService,
            IFactoryService factoryService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            //this._emailSenderService = emailSenderService;
            this._factoryService = factoryService;
        }

        public async Task<trasmissioniSendSollecito_newWAResult> Handle(trasmissioniSendSollecito_newWARequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

                var amministraEntity = await _dbContext.AmministraEntities.AsNoTracking()
                    .Where(a => a.SYSTEM_ID == idTenant)
                    .FirstAsync();

                var priorita = request.trasm.infoDocumento != null ? request.trasm.infoDocumento.evidenza : null;

                var oggetto = request.trasm.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO ? Resources.documento : Resources.fascicolo;
                var subject = string.Format(Resources.Subject, oggetto);

                var bodyMail = string.Empty;
                foreach (var currTrasmSing in request.trasm.trasmissioniSingole)
                {
                    bodyMail = string.Format(Resources.bodyMailRagione, request.trasm.dataInvio, currTrasmSing.ragione.descrizione);

                    if (currTrasmSing.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.RUOLO)
                        bodyMail += string.Format(Resources.bodyMailRuolo, ((DocsPaVO.utente.Ruolo)currTrasmSing.corrispondenteInterno).descrizione);

                    if (!string.IsNullOrEmpty(currTrasmSing.dataScadenza))
                        bodyMail += string.Format(Resources.bodyMailScadeza, currTrasmSing.dataScadenza);

                    bodyMail += string.Format(Resources.bodyMailOggetto, oggetto);

                    if (!string.IsNullOrEmpty(request.trasm.noteGenerali))
                        bodyMail += string.Format(Resources.bodyMailNoteGenerali, request.trasm.noteGenerali);

                    if (!string.IsNullOrEmpty(currTrasmSing.noteSingole))
                        bodyMail += string.Format(Resources.bodyMailNoteSingole, currTrasmSing.noteSingole);

                    if (request.trasm.infoDocumento != null)
                    {
                        bodyMail += string.Format(Resources.bodyMailInfoDocumento, request.trasm.infoDocumento.oggetto);

                        if (!string.IsNullOrEmpty(request.trasm.infoDocumento.segnatura))
                            bodyMail += string.Format(Resources.bodyMailSegnatura, request.trasm.infoDocumento.segnatura);

                        bodyMail += "<br />";

                        var isImmagineAcquisita = await _dbContext.ComponentEntities.AsNoTracking()
                            .AnyAsync(c => c.VERSION_ID == (_dbContext.VersionEntities.AsNoTracking()
                                .Where(v => v.DOCNUMBER == request.trasm.infoDocumento.docNumber.AsLong())
                                .OrderByDescending(v => v.VERSION_ID)
                                .Select(v => v.VERSION_ID)
                                .First())
                                && c.FILE_SIZE > 0);

                        //Link all'immagine del documento
                        if (isImmagineAcquisita)
                            bodyMail += string.Format(Resources.bodyMailLinkImmagineDocumento, request.path, idTenant.ToString(), request.trasm.infoDocumento.docNumber, request.trasm.infoDocumento.idProfile);

                        //Link alla scheda documento
                        bodyMail += string.Format(Resources.bodyMailLinkSchedaDocumento, request.path, idTenant.ToString(), request.trasm.infoDocumento.docNumber, request.trasm.infoDocumento.tipoProto);

                    }
                    else
                    {
                        bodyMail += string.Format(Resources.bodyMailInfoFascicolo, request.trasm.infoFascicolo.codice, request.trasm.infoFascicolo.descrizione);
                        bodyMail += string.Format(Resources.bodyMailLinkFascicolo, request.path, idTenant.ToString(), request.trasm.infoFascicolo.codice);
                    }

                    foreach (var trasmUtente in currTrasmSing.trasmissioneUtente)
                    {
                        var people = await _dbContext.PeopleEntities.AsNoTracking()
                            .Where(p => p.SYSTEM_ID == trasmUtente.utente.idPeople.AsLong()
                                && p.DISABLED == "N" && p.CHA_NOTIFICA == "E" && p.EMAIL_ADDRESS != null)
                            .Select(p => new
                            {
                                p.EMAIL_ADDRESS,
                                p.CHA_NOTIFICA
                            })
                            .FirstOrDefaultAsync();
                        if (people != null)
                        {
                            var instructions = new SendEmailInstructions
                            {
                                Sender = new EmailSender
                                {
                                    Address = amministraEntity.FROM_EMAIL_ADDRESS
                                },
                                To = new List<EmailRecipient>
                            {
                                new EmailRecipient
                                {
                                    Address = people.EMAIL_ADDRESS
                                }
                            },
                                Subject = new Core.SeedWork.TextValue(subject),
                                Body = new Core.SeedWork.TextValue(bodyMail),
                                BodyIsHtml = true
                            };

                            try
                            {
                                StringDictionary arguments = new StringDictionary();
                                //TO DO distinguere per provider
                                arguments.Add("Host", amministraEntity.VAR_SMTP!);
                                arguments.Add("Port", amministraEntity.NUM_PORTA_SMTP!.ToString());
                                arguments.Add("RequireSsl", amministraEntity?.CHA_SMTP_SSL == "1" ? "true" : "false");
                                arguments.Add("UserName", amministraEntity?.VAR_USER_SMTP);
                                arguments.Add("Password", Crypter.Decode(amministraEntity?.VAR_PWD_SMTP, amministraEntity.VAR_USER_SMTP));

                                var provider = await this._dbContext.AssProviderLibEntities
                                .Where(x => x.PROVIDER_ID == amministraEntity.PROVIDER_ID)
                                .Select(x => x.LIB)
                                .FirstOrDefaultAsync();

                                var creation = await _factoryService.TryCreate<IEmailSenderService>(s => s.Provider == provider);

                                if (!creation.Success)
                                    throw new ProviderNotFoundPi3Exception(String.Format(Resources.ProviderNotFound, provider));

                                var result = await creation.Service.SendEmail((configurations) =>
                                {
                                    switch (configurations)
                                    {
                                        case ChilkatSendEmailConfiguration chilkatEmailBoxConfigurations:
                                            this.LoadChilkatSendEmailConfigurations((ChilkatSendEmailConfiguration)configurations, amministraEntity);
                                            break;
                                        case GraphSendEmailConfiguration graphSendEmailConfigurations:
                                            this.LoadGraphSendEmailConfigurations((GraphSendEmailConfiguration)configurations, amministraEntity);
                                            break;
                                        default:
                                            throw new SendMailPi3Exception(Resources.ProviderNonGestito);

                                    }
                                },
                                instructions
                                );

                                this._logger.LogInformation($"Email message sent to {trasmUtente.utente.email} - messageId: {result.MessageId}");
                            }
                            catch (Exception ex)
                            {
                                this._logger.LogError($"Email delivery FAILED for {trasmUtente.utente.email} - {ex.Message}");
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new trasmissioniSendSollecito_newWAResult(output);
        }

        private void LoadGraphSendEmailConfigurations(GraphSendEmailConfiguration configurations, AmministrazioneEntity amministraEntity)
        {
            configurations.TenantId = amministraEntity.MS_TENANT_ID;
            configurations.ClientId = amministraEntity.MS_CLIENT_ID;
            configurations.ClientSecret = amministraEntity.MS_CLIENT_SEC;
            configurations.MailBox = amministraEntity.FROM_EMAIL_ADDRESS;
        }

        private void LoadChilkatSendEmailConfigurations(ChilkatSendEmailConfiguration configurations, AmministrazioneEntity amministraEntity)
        {
            configurations.Host = amministraEntity.VAR_SMTP!;
            configurations.Port = Convert.ToInt32(amministraEntity.NUM_PORTA_SMTP)!;
            configurations.RequireSsl = amministraEntity?.CHA_SMTP_SSL == "1";
            configurations.StartTLS = !string.IsNullOrEmpty(amministraEntity.CHA_SMTP_STA) && amministraEntity.CHA_SMTP_STA == "1";
            configurations.UserName = amministraEntity?.VAR_USER_SMTP;
            configurations.Password = Crypter.Decode(amministraEntity?.VAR_PWD_SMTP, amministraEntity?.VAR_USER_SMTP);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<trasmissioniSendSollecito_newWAHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        //protected IEmailSenderService _emailSenderService; 
        protected readonly IFactoryService _factoryService;

        #endregion
    }
}