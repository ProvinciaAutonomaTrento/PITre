// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Azure;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using Microsoft.Kiota.Abstractions;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Crmf;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Core.Services.Email.Sender;
using Pi3.Infrastructure.Graph.Services.Email.BoxScanner;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pi3Core = Pi3.Core.Services.Email.BoxScanner;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;
using Microsoft.Graph.Models.Security;
using System.Net.Mail;

namespace Pi3.Infrastructure.Graph.Services.Email.Sender
{
    public class GraphEmailSenderService : IEmailSenderService
    {
        public GraphEmailSenderService(ILogger<GraphEmailSenderService> logger)
        {
            this._logger = logger;
        }

        public virtual string Provider => "MSGRAPH";

        public async Task<EmailSended> SendEmail(Action<SendEmailConfigurations> loadSendEmailConfigurations, SendEmailInstructions instructions)
        {
            var configurations = new GraphSendEmailConfiguration();

            loadSendEmailConfigurations(configurations);

            EmailSended emailSended = null;
            Message messageSent = null;

            Validator.ValidateObject(configurations, new ValidationContext(configurations), true);
            Validator.ValidateObject(instructions, new ValidationContext(instructions), true);

            this.AssertArgument(configurations, "TenantId");
            this.AssertArgument(configurations, "ClientId");
            this.AssertArgument(configurations, "ClientSecret");
            this.AssertArgument(configurations, "MailBox");

            this._clientId = configurations.ClientId;
            this._tenantId = configurations.TenantId;
            this._clientSecret = configurations.ClientSecret;
            this._mailbox = configurations.MailBox;
            this._scopes = new string[] { UrlResources.ScopeUrl };
            var fromAddress = instructions.Sender.Address;

            try
            {
                #region Login
                this._scopes = new string[] { Resources.ScopeUrl };

                var options = new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
                };

                var clientSecretCredential = new ClientSecretCredential(
                        this._tenantId, this._clientId, this._clientSecret, options);

                this._graphClient = new GraphServiceClient(clientSecretCredential, this._scopes);

                #endregion

                #region Allegati
                var attachments = instructions.Attachments;
                List<Microsoft.Graph.Models.Attachment> msAttachments = new List<Microsoft.Graph.Models.Attachment>();
                if (attachments != null && attachments.Count() > 0)
                {
                    msAttachments = new List<Microsoft.Graph.Models.Attachment>();
                    foreach (var att in attachments)
                    {
                        msAttachments.Add(new FileAttachment
                        {
                            OdataType = "#microsoft.graph.fileAttachment",
                            Name = att.FileName,
                            ContentType = att.ContentType,
                            ContentBytes = att.Content
                        });
                    }
                }
                #endregion

                #region Destinatari
                List<Recipient> toList = new List<Recipient>();
                foreach (var to in instructions.To)
                    toList.Add(new Recipient
                    {
                        EmailAddress = new EmailAddress { Address = to.Address }
                    });

                List<Recipient> ccList = new List<Recipient>();
                if (instructions.Cc != null && instructions.Cc.Count() > 0)
                {
                    foreach (var cc in instructions.Cc)
                        ccList.Add(new Recipient
                        {
                            EmailAddress = new EmailAddress { Address = cc.Address }
                        });
                }

                List<Recipient> bccList = new List<Recipient>();
                if (instructions.Bcc != null && instructions.Bcc.Count() > 0)
                {
                    foreach (var bcc in instructions.Bcc)
                        bccList.Add(new Recipient
                        {
                            EmailAddress = new EmailAddress { Address = bcc.Address }
                        });
                }
                #endregion

                Message message = new()
                {
                    //InternetMessageHeaders = new List<InternetMessageHeader>()
                    //{
                    //    new InternetMessageHeader
                    //    {
                    //        Name = "X-Prefer",
                    //        Value = "IdType=\"ImmutableId\""
                    //    }
                    //},
                    Subject = instructions.Subject.Value,
                    Body = new ItemBody
                    {
                        ContentType = instructions.BodyIsHtml ? BodyType.Html : BodyType.Text,
                        Content = instructions.Body.Value.ToString()
                    },
                    From = new Recipient
                    {
                        EmailAddress = new EmailAddress { Address = fromAddress }
                    },
                    ToRecipients = toList,
                    CcRecipients = ccList,
                    BccRecipients = bccList,
                    Attachments = msAttachments,
                    IsDraft = true
                };

                #region Request
                var requestBody = new SendMailPostRequestBody
                {
                    Message = message,
                    SaveToSentItems = true
                };
                #endregion

                //Salvo in bozze
                messageSent = await this._graphClient.Users[fromAddress].Messages.PostAsync(message);

                //await this._graphClient.Users[fromAddress].SendMail.PostAsync(requestBody);
                await this._graphClient.Users[fromAddress].Messages[messageSent.Id].Send.PostAsync();

                emailSended = new EmailSended()
                {
                    MessageId = messageSent.InternetMessageId
                };
            }
            catch (Exception ex)
            {
                //elimina messaggio in bozza
                if (messageSent != null && !string.IsNullOrEmpty(messageSent.Id))
                    await this._graphClient.Users[fromAddress].Messages[messageSent.Id].DeleteAsync();
                throw new GraphSendEmailPi3Exception(ex.Message);
            }
            return emailSended;
        }

        public Task<EmailSended> SendEmail_old(Action<SendEmailConfigurations> loadSendEmailConfigurations, SendEmailInstructions instructions)
        {
            var configurations = new GraphSendEmailConfiguration();

            loadSendEmailConfigurations(configurations);

            //Non uso GraphClient perché non restituisce codici di risposta. 
            //Uso RestSharp

            EmailSended emailSended = null;

            Validator.ValidateObject(configurations, new ValidationContext(configurations), true);
            Validator.ValidateObject(instructions, new ValidationContext(instructions), true);

            this.AssertArgument(configurations, "TenantId");
            this.AssertArgument(configurations, "ClientId");
            this.AssertArgument(configurations, "ClientSecret");
            this.AssertArgument(configurations, "MailBox");

            this._clientId = configurations.ClientId;
            this._tenantId = configurations.TenantId;
            this._clientSecret = configurations.ClientSecret;
            this._mailbox = configurations.MailBox;
            this._scopes = new string[] { UrlResources.ScopeUrl };

            var attachments = instructions.Attachments;
            List<MSAttachment> msAttachments = null;
            if (attachments != null && attachments.Count() > 0)
            {
                msAttachments = new List<MSAttachment>();
                foreach (var att in attachments)
                {
                    msAttachments.Add(new MSAttachment
                    {
                        OdataType = "#microsoft.graph.fileAttachment",
                        Name = att.FileName,
                        ContentType = att.ContentType,
                        ContentBytes = Convert.ToBase64String(att.Content)
                    });
                }
            }

            List<MSRecipient> toList = new List<MSRecipient>();
            foreach (var to in instructions.To)
                toList.Add(new MSRecipient
                {
                    EmailAddress = new MSEmailAddress { Address = to.Address }
                });

            List<MSRecipient> ccList = new List<MSRecipient>();
            foreach (var cc in instructions.Cc)
                ccList.Add(new MSRecipient
                {
                    EmailAddress = new MSEmailAddress { Address = cc.Address }
                });

            List<MSRecipient> bccList = new List<MSRecipient>();
            foreach (var bcc in instructions.Bcc)
                bccList.Add(new MSRecipient
                {
                    EmailAddress = new MSEmailAddress { Address = bcc.Address }
                });

            MSSendMessageRequest requestBody = null;

            try
            {
                #region Costruzione messaggio 
                requestBody = new MSSendMessageRequest
                {
                    Message = new MSMessage
                    {
                        Subject = instructions.Subject.Value,
                        Body = new MSItemBody
                        {
                            ContentType = instructions.BodyIsHtml ? BodyType.Html.ToString() : BodyType.Text.ToString(),
                            Content = instructions.Body.ToString()
                        },
                        From = new MSRecipient
                        {
                            EmailAddress = new MSEmailAddress { Address = instructions.Sender.Address }
                        },
                        ToRecipients = toList,
                        CcRecipients = ccList,
                        BccRecipients = bccList,
                        Attachments = msAttachments,
                    },
                };
                #endregion
            }
            catch (Exception exc)
            {
                throw new Exception(String.Format("Errore nella creazione del messaggio. {0}", exc.Message));
            }

            try
            {
                //Uso RestSharp
                if (requestBody != null)
                {
                    #region Autenticazione
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    RestClient authClient = null;
                    RestRequest authRequest = null;
                    string token = string.Empty;

                    string authUrl = UrlResources.AuthUrl;
                    authClient = new RestClient(authUrl);
                    string authSendUrl = "/{tenant}/oauth2/v2.0/token";

                    authRequest = new RestRequest(authSendUrl, RestSharp.Method.Post);
                    authRequest.AddUrlSegment("tenant", this._tenantId);
                    authRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    authRequest.AddParameter("scope", this._scopes[0]);
                    authRequest.AddParameter("client_secret", this._clientSecret);
                    authRequest.AddParameter("grant_type", "client_credentials");
                    authRequest.AddParameter("client_id", this._clientId);

                    var authResponse = authClient.Execute(authRequest);

                    if (authResponse.StatusCode == HttpStatusCode.OK && authResponse.IsSuccessful)
                    {
                        var responseToken = System.Text.Json.JsonSerializer.Deserialize<MSGetTokenResponse>(authResponse.Content);
                        if (responseToken.access_token != null)
                        {
                            token = responseToken.access_token;
                        }
                    }
                    #endregion

                    #region Invio messaggio 
                    RestClient client = null;
                    RestRequest request = null;

                    string graphServicesUrl = UrlResources.GraphServicesUrl;
                    client = new RestClient(graphServicesUrl);
                    string sendUrl = "users/{mail}/sendMail";
                    request = new RestRequest(sendUrl, RestSharp.Method.Post);
                    request.AddUrlSegment("mail", this._mailbox);
                    request.AddHeader("Authorization", $"Bearer {token}");

                    JsonSerializerSettings config = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    string jsonString = JsonConvert.SerializeObject(requestBody, Formatting.Indented, config);
                    //string jsonString = System.Text.Json.JsonSerializer.Serialize(requestBody);
                    request.AddJsonBody(jsonString);
                    //request.AddParameter("application/json", jsonString, ParameterType.RequestBody);

                    var response = client.Execute(request);
                    if (response != null)
                    {
                        var content = !string.IsNullOrEmpty(response.Content) ? JObject.Parse(response.Content) : null;

                        if (response.StatusCode == HttpStatusCode.Accepted)
                        {
                            this._logger.LogDebug($"Response status {response.StatusCode}");
                        }
                        else
                        {
                            if (content != null)
                            {
                                string errorCode = (content["error"])["code"].ToString();
                                string errorMsg = (content["error"])["message"].ToString();
                                this._logger.LogDebug($"Response status {response.StatusCode}, errore {errorCode} - {errorMsg}");
                                throw new GraphSendEmailPi3Exception(errorMsg);
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception exc)
            {
                this._logger.LogError($"Errore nell'invio del messaggio {exc.Message}.");
                throw new GraphSendEmailPi3Exception(exc.Message);
            }

            return Task.FromResult(emailSended);
        }

        protected ILogger<GraphEmailSenderService> _logger;
        private string _tenantId;
        private string _clientId;
        private string _clientSecret;
        private string _mailbox;
        private string[] _scopes;
        private GraphServiceClient _graphClient;



        protected virtual void AssertArgument(GraphSendEmailConfiguration configurations, string argument)
        {
            //if (!configurations.Arguments!.ContainsKey(argument))
            //    throw new GraphArgumentNotFoundPi3Exception(argument);
            if (configurations.GetType().GetProperty(argument).GetValue(configurations) == null)
                throw new GraphArgumentNotFoundPi3Exception(argument);
        }

    }


}
