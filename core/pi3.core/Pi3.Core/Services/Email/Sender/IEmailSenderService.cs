// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.Email.BoxScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.Email.Sender
{
    public interface IEmailSenderService : IService
    {
        string Provider { get; }

        Task<EmailSended> SendEmail(Action<SendEmailConfigurations> loadSendEmailConfigurations, SendEmailInstructions instructions);
    }
}