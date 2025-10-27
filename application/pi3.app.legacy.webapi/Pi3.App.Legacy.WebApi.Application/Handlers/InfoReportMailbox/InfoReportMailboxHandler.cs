// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocumentFormat.OpenXml.Math;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using InfoReportMailboxRequest = Pi3.App.Legacy.WebApi.Application.Requests.InfoReportMailbox;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InfoReportMailbox
{
    public class InfoReportMailboxHandler : IRequestHandler<InfoReportMailboxRequest, InfoReportMailboxResult>
    {
        #region Public Members

        public InfoReportMailboxHandler(ILogger<InfoReportMailboxHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<InfoReportMailboxResult> Handle(InfoReportMailboxRequest request, CancellationToken cancellationToken)
        {
            var output = new DocsPaVO.Interoperabilita.MailAccountCheckResponse();

            try
            {
                var checkMailboxEntity = await this._dbContext.CheckMailboxEntities.AsNoTracking()
                        .Join(this._dbContext.RegistroEntities.AsNoTracking(), m => m.IDREG, r => r.SYSTEM_ID, (m, r) => new { m, r })
                        .Where(x => x.m.ID == request.idCheckMailbox.AsLong())
                        .Select(x => new
                        {
                            x.r.VAR_CODICE,
                            x.m.MAIL,
                            x.m.ERRORMESSAGE,
                            x.m.MAILSERVER,
                            x.m.MAILUSERID,
                            x.m.DTA_CONCLUDED
                        }).FirstOrDefaultAsync();

                if (checkMailboxEntity is not null)
                {
                    output.Registro = checkMailboxEntity.VAR_CODICE ?? string.Empty;
                    output.MailAddress = checkMailboxEntity.MAIL ?? string.Empty;
                    output.MailServer = checkMailboxEntity.MAILSERVER ?? string.Empty;
                    output.MailUserID = checkMailboxEntity.MAILUSERID ?? string.Empty;
                    output.ErrorMessage = checkMailboxEntity.ERRORMESSAGE ?? string.Empty;
                    output.DtaConcluded = checkMailboxEntity?.DTA_CONCLUDED.GetValueOrDefault() ?? DateTime.MinValue;

                    var reportMailboxEntities = await this._dbContext.ReportMailboxEntities.AsNoTracking()
                    .Where(x => x.ID_CHECK_MAILBOX == request.idCheckMailbox.AsLong())
                    .ToListAsync();

                    if (reportMailboxEntities.Any())
                    {
                        var mailProcessedList = new List<DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed>();

                        reportMailboxEntities.ForEach(x =>
                        {
                            var item = new DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed
                            {
                                Subject = x.SUBJECT ?? string.Empty,
                                Date = x.DATE_MAIL.GetValueOrDefault(),
                                MailID = x.MAILID ?? string.Empty,
                                From = x.FROM_MAIL ?? string.Empty,
                                CountAttatchments = x.COUNT_ATTACHMENTS != null ? (int)x.COUNT_ATTACHMENTS : 0,
                                ErrorMessage = x.ERROR ?? string.Empty
                            };

                            var typeResult = new DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailProcessedType();
                            var receiptResult = new DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta();

                            if (Enum.TryParse(x.TYPE, out typeResult)) item.ProcessedType = typeResult;

                            if (Enum.TryParse(x.RECEIPT, out receiptResult)) item.PecXRicevuta = receiptResult;

                            mailProcessedList.Add(item);
                        });

                        output.MailProcessedList = mailProcessedList.ToArray();
                    }

                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new InfoReportMailboxResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InfoReportMailboxHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}