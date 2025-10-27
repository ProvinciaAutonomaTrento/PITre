// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.Factory;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Chilkat.Services.Email.Sender;
using Pi3.Infrastructure.Graph.Services.Email.Sender;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SendMailSupportForDocumentDeliveryRequest = Pi3.App.Legacy.WebApi.Application.Requests.SendMailSupportForDocumentDelivery;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SendMailSupportForDocumentDelivery
{
    public class SendMailSupportForDocumentDeliveryHandler : IRequestHandler<SendMailSupportForDocumentDeliveryRequest>
    {
        #region Public Members

        public SendMailSupportForDocumentDeliveryHandler(ILogger<SendMailSupportForDocumentDeliveryHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            //IEmailSenderService emailSenderService,
            IFactoryService factoryService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
            //this._emailSenderService = emailSenderService;
            this._factoryService = factoryService;
        }

        public async Task Handle(SendMailSupportForDocumentDeliveryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

                var recipient = await this._configurationService.GetValue<string>("BE_EMAIL_ADDRESS_SUPPORT");

                var amministraEntity = await this._dbContext.AmministraEntities.AsNoTracking().FirstAsync(x => x.SYSTEM_ID == idTenant);

                var profileEntity = await this._dbContext.ProfileEntities.AsNoTracking().FirstAsync(x => x.SYSTEM_ID == request.schedaDocumento.systemId.AsLong());

                var peopleEntity = await this._dbContext.PeopleEntities.AsNoTracking().FirstAsync(x => x.SYSTEM_ID == request.infoUtente.idPeople.AsLong());

                var bodyLines = new List<string>
            {
                $"<font face='Arial'>{Resources.BodyTenantLabel}: <B>{amministraEntity.VAR_CODICE_AMM} - {amministraEntity.VAR_DESC_AMM}</B><br />",
                $"{Resources.BodyUserLabel}: <B>{peopleEntity.VAR_COGNOME} {peopleEntity.VAR_NOME} ({request.ruolo.descrizione})</B><br />",
                $"{Resources.BodyDocumentIdLabel}: <B>{profileEntity.DOCNUMBER}</B><br />",
                $"{Resources.BodySubjectLabel}: <B>{profileEntity.VAR_PROF_OGGETTO}</B><br />",
                $"{Resources.BodyDeliveryDate}: <B>{DateTime.Now}</B><br />"
            };

                var body = string.Join(string.Empty, bodyLines);

                StringDictionary arguments = new StringDictionary();
                //TO DO distinguere per provider
                arguments.Add("Host", amministraEntity.VAR_SMTP);
                arguments.Add("Port", amministraEntity.NUM_PORTA_SMTP.ToString());
                arguments.Add("RequireSsl", (amministraEntity.CHA_SMTP_SSL ?? "0") == "1" ? "true" : "false");
                arguments.Add("UserName", amministraEntity.VAR_USER_SMTP);
                arguments.Add("Password", Decode(amministraEntity.VAR_PWD_SMTP, amministraEntity.VAR_USER_SMTP));

                var provider = await this._dbContext.AssProviderLibEntities
                    .Where(x => x.PROVIDER_ID == amministraEntity.PROVIDER_ID)
                    .Select(x => x.LIB)
                    .FirstOrDefaultAsync();

                var creation = await _factoryService.TryCreate<IEmailSenderService>(s => s.Provider == provider);

                if (!creation.Success)
                    throw new ProviderNotFoundPi3Exception(String.Format(Resources.ProviderNotFound, provider));

                await creation.Service.SendEmail((configurations) =>
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
                new SendEmailInstructions
                {
                    Sender = new EmailSender { Address = amministraEntity.FROM_EMAIL_ADDRESS },
                    To = new List<EmailRecipient> { new EmailRecipient { Address = recipient } },
                    Subject = new Core.SeedWork.TextValue { Value = Resources.BodySubjectLabel },
                    Body = new Core.SeedWork.TextValue { Value = body },
                    BodyIsHtml = true,
                });
            }
            catch(Exception ex)
            {
                this._logger.LogError(ex.Message);
            }
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
            configurations.Host = amministraEntity.VAR_SMTP;
            configurations.Port = amministraEntity.NUM_PORTA_SMTP.HasValue ? Convert.ToInt32(amministraEntity.NUM_PORTA_SMTP) : 0;
            configurations.StartTLS = !string.IsNullOrEmpty(amministraEntity.CHA_SMTP_STA) && amministraEntity.CHA_SMTP_STA == "1";
            configurations.RequireSsl = amministraEntity.CHA_SMTP_SSL == "1";
            configurations.UserName = amministraEntity.VAR_USER_SMTP;
            configurations.Password = Decode(amministraEntity.VAR_PWD_SMTP, amministraEntity.VAR_USER_SMTP);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SendMailSupportForDocumentDeliveryHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        //protected IEmailSenderService _emailSenderService;
        protected readonly IFactoryService _factoryService;

        private static string Decode(string? password, string? key)
        {
            try
            {
                string decode = password ?? string.Empty;

                if (!string.IsNullOrEmpty(key))
                {
                    using (var aes = Aes.Create())
                    {
                        aes.KeySize = 128;
                        aes.BlockSize = 128;

                        aes.Key = ASCIIEncoding.ASCII.GetBytes(genera_key_16(key));//chiave);
                        aes.IV = ASCIIEncoding.ASCII.GetBytes(genera_key_16(key));//iv);

                        byte[] input = Convert.FromBase64String(password ?? string.Empty);
                        byte[] output = aes.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
                        decode = Encoding.UTF8.GetString(output);
                        //caso di mancanza di password
                        if (decode.Equals(" "))
                            decode = string.Empty;
                    }
                }
                return decode;
            }
            catch (Exception)
            {
                return password ?? string.Empty;
            }
        }

        private static string genera_key_16(string stringa)
        {
            if (stringa.Length < 16)
                for (int i = stringa.Length; i < 16; i++)
                    stringa = "0" + stringa;
            else
                return stringa.Substring(0, 16);

            return stringa;
        }

        #endregion
    }
}