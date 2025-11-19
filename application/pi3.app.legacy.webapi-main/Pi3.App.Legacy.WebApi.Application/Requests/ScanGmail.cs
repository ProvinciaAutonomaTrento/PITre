// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record ScanGmailResult(string AuthorizationUrl);

    public record ScanGmail(
        [Required(AllowEmptyStrings = false)] string Instance,
        [Required(AllowEmptyStrings = false)] string CodiceAmministrazione,
        [Required(AllowEmptyStrings = false)] string CodiceRegistro) : IRequest<ScanGmailResult>;

    public record ScanGmailCallbackSummary
        (
            DateTime BeginDate,
            DateTime EndDate,
            double TotalSeconds,
            int MessageCount,
            IReadOnlyList<ScanGmailCallbackMessageSummary> Messages
        );

    public record ScanGmailCallbackMessageSummary(
            string MessageId,
            string From,
            string To,
            string Cc,
            string Bcc,
            string Subject,
            int Attachments,
            DateTime ProcessedDate,
            DateTime ProcessedEndDate,
            double TotalSeconds,
            bool Handled);

    public record ScanGmailCallbackResult(ScanGmailCallbackSummary summary);
    
    public record ScanGmailCallback(
        [Required(AllowEmptyStrings = false)] string Code, 
        string? State,
        [Required(AllowEmptyStrings = false)] string Instance,
        [Required(AllowEmptyStrings = false)] string CodiceAmministrazione,
        [Required(AllowEmptyStrings = false)] string CodiceRegistro) : IRequest<ScanGmailCallbackResult>;
}
