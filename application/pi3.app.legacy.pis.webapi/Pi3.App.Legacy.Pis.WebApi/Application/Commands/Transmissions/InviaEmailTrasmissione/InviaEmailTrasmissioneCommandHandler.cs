// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.RabbitMQ;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.Factory;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Chilkat.Services.Email.Sender;
using Pi3.Infrastructure.Graph.Services.Email.Sender;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.InviaEmailTrasmissione
{
    public class InviaEmailTrasmissioneCommandHandler : MessageQueueBaseCommandHandler<InviaEmailTrasmissioneCommand>
    {
        #region Public Members

        public InviaEmailTrasmissioneCommandHandler(ILogger<InviaEmailTrasmissioneCommandHandler> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        protected override async Task InternalHandle(IServiceProvider serviceProvider, InviaEmailTrasmissioneCommand message)
        {
            var claimsPrincipalService = serviceProvider.GetRequiredService<IClaimsPrincipalService>();
            var idTenant = claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            //IEmailSenderService senderService = serviceProvider.GetRequiredService<IEmailSenderService>();
            IPi3DbContext dbContext = serviceProvider.GetRequiredService<IPi3DbContext>();
            IConfigurationService configurationService = serviceProvider.GetRequiredService<IConfigurationService>();
            IFactoryService factoryService = serviceProvider.GetRequiredService<IFactoryService>();

            var peopleEntity = await dbContext.PeopleEntities.AsNoTracking()
                .Join(dbContext.TrasmUtenteEntities.AsNoTracking(), p => p.SYSTEM_ID, t => t.ID_PEOPLE, (p, t) => new { p, t })
                .Where(x => x.t.SYSTEM_ID == message.IdTrasmissioneUtente)
                .Select(x => x.p)
                .FirstAsync();

            var amministraEntity = await dbContext.AmministraEntities.FirstAsync(x => x.SYSTEM_ID == idTenant);

            var trasmissioneSingolaEntity = await dbContext.TrasmSingolaEntities.AsNoTracking()
                .Join(dbContext.TrasmUtenteEntities.AsNoTracking(), ts => ts.SYSTEM_ID, tu => tu.ID_TRASM_SINGOLA, (ts, tu) => new { ts, tu })
                .Where(x => x.tu.SYSTEM_ID == message.IdTrasmissioneUtente)
                .Select(x => x.ts)
                .FirstAsync();

            var trasmissioneEntity = await dbContext.TrasmissioneEntities.FirstAsync(x => x.SYSTEM_ID == trasmissioneSingolaEntity.ID_TRASMISSIONE);

            var senderMailAddress = peopleEntity.FROM_EMAIL_ADDRESS.NullIfWhiteSpace() ?? amministraEntity.FROM_EMAIL_ADDRESS;

            var tipoNotifica = !string.IsNullOrWhiteSpace(message.TipoNotifica) ?
                message.TipoNotifica :
                this.GetTipoNotificaUtente(peopleEntity);

            var subject = string.Empty;
            var body = string.Empty;

            List<EmailContentAttachment>? attachments = default;

            if (trasmissioneEntity.ID_PROFILE.HasValue)
            {
                var profileEntity = await dbContext.ProfileEntities.FirstAsync(x => x.SYSTEM_ID == trasmissioneEntity.ID_PROFILE);

                subject = string.Format(
                    Resources.EmailSubject,
                    string.Empty,
                    Resources.DocumentLabel,
                    profileEntity.VAR_PROF_OGGETTO!.Length < 172 ? profileEntity.VAR_PROF_OGGETTO : profileEntity.VAR_PROF_OGGETTO.Substring(0, 171) + " ...");

                body = await this.GetMessageBody(
                    dbContext,
                    profileEntity,
                    trasmissioneSingolaEntity,
                    trasmissioneEntity);

                if (tipoNotifica == "EA" || tipoNotifica == "ED") attachments = await this.GetMessageAttachments(serviceProvider, profileEntity.SYSTEM_ID, idTenant);

                if (tipoNotifica == "E" || tipoNotifica == "ED")
                {
                    var path = await configurationService.GetValue<string>("STATIC_ROOT_PATH", false, string.Empty);

                    var groupEntity = await dbContext.GroupEntities.AsNoTracking()
                         .Join(dbContext.CorrGlobaliEntities.AsNoTracking(),
                             g => g.SYSTEM_ID,
                             c => c.ID_GRUPPO,
                             (g, c) => new { g, c })
                         .Where(x => x.c.SYSTEM_ID == trasmissioneSingolaEntity.ID_CORR_GLOBALE)
                         .Select(x => x.g)
                         .FirstOrDefaultAsync();

                    var hasImage = await dbContext.ProfileEntities.AsNoTracking()
                                        .AnyAsync(x => x.SYSTEM_ID == profileEntity.SYSTEM_ID && x.EXT != null);

                    body += string.Format("<br>{0}</br>", this.GetLinkToObject(
                        idTenant,
                        profileEntity.SYSTEM_ID.ToString(),
                        groupEntity is not null ? groupEntity.SYSTEM_ID : null,
                        profileEntity.CHA_TIPO_PROTO,
                        hasImage,
                        path,
                        "D"));
                }
            }
            else
            {
                var projectEntity = await dbContext.ProjectEntities.FirstAsync(x => x.SYSTEM_ID == trasmissioneEntity.ID_PROJECT);

                subject = string.Format(
                    Resources.EmailSubject,
                    string.Empty,
                    Resources.FolderLabel,
                    projectEntity.DESCRIPTION!.Length < 172 ? projectEntity.DESCRIPTION : projectEntity.DESCRIPTION.Substring(0, 171) + " ...");

                body = await this.GetMessageBody
                    (dbContext,
                    projectEntity,
                    trasmissioneSingolaEntity,
                    trasmissioneEntity);

                if (tipoNotifica == "E" || tipoNotifica == "ED")
                {
                    var path = await configurationService.GetValue<string>("STATIC_ROOT_PATH", false, string.Empty);

                    var groupEntity = await dbContext.GroupEntities.AsNoTracking()
                        .Join(dbContext.CorrGlobaliEntities.AsNoTracking(),
                            g => g.SYSTEM_ID,
                            c => c.ID_GRUPPO,
                            (g, c) => new { g, c })
                        .Where(x => x.c.SYSTEM_ID == trasmissioneSingolaEntity.ID_CORR_GLOBALE)
                        .Select(x => x.g)
                        .FirstOrDefaultAsync();

                    body += string.Format("<br>{0}</br>", this.GetLinkToObject(
                        idTenant,
                        projectEntity.VAR_CODICE,
                        groupEntity is not null ? groupEntity.SYSTEM_ID : null,
                        null,
                        false,
                        path,
                        "F"));
                }
            }

            body = $"<font face='Arial'>{body}</font>";

            var instructions = new SendEmailInstructions
            {
                Sender = new EmailSender
                {
                    Address = senderMailAddress
                },
                To = new List<EmailRecipient>
                {
                    new EmailRecipient
                    {
                        Address = peopleEntity.EMAIL_ADDRESS,
                        DisplayName = peopleEntity.FULL_NAME
                    }
                },
                Subject = new Core.SeedWork.TextValue(subject),
                Body = new Core.SeedWork.TextValue(body),
                BodyIsHtml = true,
                Attachments = attachments
            };

            try
            {
                var provider = await dbContext.AssProviderLibEntities
                    .Where(x => x.PROVIDER_ID == amministraEntity.PROVIDER_ID)
                    .Select(x => x.LIB)
                    .FirstOrDefaultAsync();

                var creation = await factoryService.TryCreate<IEmailSenderService>(s => s.Provider == provider);

                if (!creation.Success)
                    throw new ProviderNotFoundPi3Exception(String.Format(Resources.ProviderNotFound, provider));

                if (!string.IsNullOrEmpty(tipoNotifica))
                {
                    var result = await creation.Service.SendEmail(
                        (configurations) =>
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
                    this._logger.LogInformation($"Email message sent to {peopleEntity.EMAIL_ADDRESS} - messageId: {result.MessageId}");
                }

            }
            catch (Exception ex)
            {
                this._logger.LogError($"Email delivery FAILED for {peopleEntity.EMAIL_ADDRESS} - {ex.Message}");
            }


        }

        private void LoadGraphSendEmailConfigurations(GraphSendEmailConfiguration configurations, AmministrazioneEntity amministraEntity)
        {
            configurations.TenantId = amministraEntity.MS_TENANT_ID;
            configurations.ClientId = amministraEntity.MS_CLIENT_ID;
            configurations.ClientSecret = amministraEntity.MS_CLIENT_SEC;
            configurations.MailBox = amministraEntity.FROM_EMAIL_ADDRESS;
        }

        private void LoadChilkatSendEmailConfigurations(ChilkatSendEmailConfiguration configurations, AmministrazioneEntity? amministraEntity)
        {
            configurations.Host = amministraEntity.VAR_SMTP;
            configurations.Port = amministraEntity.NUM_PORTA_SMTP.HasValue ? Convert.ToInt32(amministraEntity.NUM_PORTA_SMTP) : 0;
            configurations.RequireSsl = amministraEntity.CHA_SMTP_SSL == "1";
            configurations.UserName = amministraEntity.VAR_USER_SMTP;
            configurations.Password = amministraEntity.VAR_PWD_SMTP;
        }

        #endregion

        #region Private Members
        protected string GetTipoNotificaUtente(PeopleEntity entity)
        {
            switch (entity.CHA_NOTIFICA?.ToUpper())
            {
                case "E":
                    return entity.CHA_NOTIFICA_CON_ALLEGATO == "1" ? "ED" : "E";
                case "":
                case null:
                    return entity.CHA_NOTIFICA_CON_ALLEGATO == "1" ? "EA" : string.Empty;
                default:
                    return entity.CHA_NOTIFICA;
            }
        }

        protected async Task<string> GetMessageBody(IPi3DbContext dbContext, object entity, TrasmSingolaEntity trasmissioneSingolaEntity, TrasmissioneEntity trasmissioneEntity)
        {
            var ragioneEntity = await dbContext.RagioneTrasmissioneEntities.FirstAsync(x => x.SYSTEM_ID == trasmissioneSingolaEntity.ID_RAGIONE);

            var message = string.Empty;

            if (entity is ProfileEntity)
            {
                message = ragioneEntity.VAR_TESTO_MSG_NOTIFICA_DOC;

                if (string.IsNullOrWhiteSpace(message)) return string.Empty;

                var profileEntity = (ProfileEntity)entity;

                var mittProto = profileEntity.CHA_TIPO_PROTO == "A" ?
                await dbContext.DocArrivoParEntities.AsNoTracking()
                    .Join(dbContext.CorrGlobaliEntities.AsNoTracking(), d => d.ID_MITT_DEST, c => c.SYSTEM_ID, (d, c) => new { d, c })
                    .Where(x => x.d.ID_PROFILE == profileEntity.SYSTEM_ID
                    && (x.d.CHA_TIPO_MITT_DEST == "M" || x.d.CHA_TIPO_MITT_DEST == "MD"))
                    .OrderBy(x => x.d.SYSTEM_ID)
                    .Select(x => x.c.VAR_DESC_CORR)
                    .FirstAsync() ?? string.Empty :
                string.Empty;

                message = message.Replace(Resources.BodyTagDocumentDescription, profileEntity.VAR_PROF_OGGETTO.AsHtmlBold())
                    .Replace(Resources.BodyTagIdentifier, !string.IsNullOrWhiteSpace(profileEntity.VAR_SEGNATURA) ? profileEntity.VAR_SEGNATURA.AsHtmlBold() : profileEntity.DOCNUMBER.ToString().AsHtmlBold())
                    .Replace(Resources.BodyTagDocumentSender, mittProto.AsHtmlBoldOrStrikeThrough());
            }
            else if (entity is ProjectEntity)
            {
                message = ragioneEntity.VAR_TESTO_MSG_NOTIFICA_FASC;

                if (string.IsNullOrWhiteSpace(message)) return string.Empty;

                var projectEntity = (ProjectEntity)entity;

                message = message.Replace(Resources.BodyTagFolderCode, projectEntity.VAR_CODICE)
                    .Replace(Resources.BodyTagFolderDescription, projectEntity.DESCRIPTION);
            }
            else
            {
                return string.Empty;
            }

            message = message.Replace("\n", "<BR>")
                .Replace(Resources.BodyTagTransmissionReason, ragioneEntity.VAR_DESC_RAGIONE.AsHtmlBold())
                .Replace(Resources.BodyTagTransmissionRecipient, (await dbContext.CorrGlobaliEntities.FirstAsync(x => x.SYSTEM_ID == trasmissioneSingolaEntity.ID_CORR_GLOBALE)).VAR_DESC_CORR.AsHtmlBold())
                .Replace(Resources.BodyTagGeneralNotes, trasmissioneEntity.VAR_NOTE_GENERALI.AsHtmlBoldOrStrikeThrough())
                .Replace(Resources.BodyTagRecipientNotes, trasmissioneSingolaEntity.VAR_NOTE_SING.AsHtmlBoldOrStrikeThrough());

            return message;
        }

        protected string GetLinkToObject(long idTenant, string idObject, long? recipientGroupId, string? recordType, bool toFile, string path, string objType)
        {
            var message = string.Empty;

            switch (objType)
            {
                case "D":
                    var link = string.Empty;
                    if (recipientGroupId.HasValue)
                    {
                        if (toFile)
                        {
                            link = string.Format(Resources.LinkToFileWithRole, path, idObject, idObject, recipientGroupId.Value);
                            message = string.Format(Resources.BodyLinkToFile, link);
                        }

                        link = string.Format(Resources.LinkToDocumentWithRole, path, idTenant, idObject, recordType ?? string.Empty, recipientGroupId.Value);
                        message += string.Format(Resources.BodyLinkDocument, link);

                    }
                    else
                    {
                        if (toFile)
                        {
                            link = string.Format(Resources.LinkToFileNoRole, path, idTenant, idObject, recordType ?? string.Empty);
                            message = string.Format(Resources.BodyLinkToFile, link);
                        }

                        link = string.Format(Resources.LinkToDocumentNoRole, path, idTenant, idObject, recordType ?? string.Empty);
                        message += string.Format(Resources.BodyLinkDocument, link);
                    }
                    break;
                case "F":
                    message = string.Format(
                        Resources.BodyLinkToFolder,
                        recipientGroupId.HasValue ?
                            string.Format(Resources.LinkToFolderWithRole, path, idTenant, idObject, recipientGroupId.Value) :
                            string.Format(Resources.LinkToFolderNoRole, path, idTenant, idObject));
                    break;
            }

            return message;
        }

        protected async Task<List<EmailContentAttachment>> GetMessageAttachments(IServiceProvider serviceProvider, long docnumber, long idTenant)
        {
            var attachments = new List<EmailContentAttachment>();

            var blobRepository = serviceProvider.GetRequiredService<IDocumentBlobRepository>();
            var dbContext = serviceProvider.GetRequiredService<IPi3DbContext>();

            var mainDocumentEmailAttachment = await this.GetEmailContentAttachment(dbContext, blobRepository, docnumber, idTenant);

            if (mainDocumentEmailAttachment is not null) attachments.Add(mainDocumentEmailAttachment);

            var profileAttachmentsEntities = await dbContext.ProfileEntities.Where(x => x.ID_DOCUMENTO_PRINCIPALE == docnumber).ToListAsync();

            if (profileAttachmentsEntities.Any())
            {
                profileAttachmentsEntities.ForEach(async x =>
                {
                    var a = await this.GetEmailContentAttachment(dbContext, blobRepository, x.SYSTEM_ID, idTenant);
                    if (a is not null) attachments.Add(a);
                });
            }

            return attachments;
        }

        protected async Task<EmailContentAttachment?> GetEmailContentAttachment(IPi3DbContext dbContext, IDocumentBlobRepository blobRepository, long docnumber, long idTenant)
        {
            var maxVersionId = (await dbContext.VersionEntities.AsNoTracking()
                .Where(x => x.DOCNUMBER == docnumber)
                .OrderByDescending(x => x.VERSION)
                .FirstAsync()).VERSION_ID;

            var componentsEntity = await dbContext.ComponentEntities.FirstAsync(x => x.DOCNUMBER == docnumber && x.VERSION_ID == maxVersionId);

            if (!string.IsNullOrWhiteSpace(componentsEntity.PATH))
            {
                var blob = await blobRepository.Get(idTenant.ToString(), componentsEntity.PATH);
                byte[] content = null;

                using (var stream = new MemoryStream())
                {
                    await blob.Stream.CopyToAsync(stream);
                    content = stream.ToArray();
                }

                return new EmailContentAttachment
                {
                    Content = content,
                    ContentType = blob.ContentType,
                    FileName = blob.FileName
                };
            }
            else return null;
        }
        #endregion
    }

    public static class StringExtensions
    {
        public static string? NullIfWhiteSpace(this string? s)
        {
            return string.IsNullOrWhiteSpace(s) ? null : s;
        }

        public static string AsHtmlBold(this string? s)
        {
            return string.IsNullOrWhiteSpace(s) ? string.Empty : $"<B>{s}</B>";
        }

        public static string AsHtmlBoldOrStrikeThrough(this string? s)
        {
            return string.IsNullOrWhiteSpace(s) ? "---------" : $"<B>{s}</B>";
        }
    }
}
