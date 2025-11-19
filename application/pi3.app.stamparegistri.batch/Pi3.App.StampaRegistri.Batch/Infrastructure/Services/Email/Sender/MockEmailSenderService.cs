// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.Email.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.StampaRegistri.Batch.Infrastructure.Services.Email.Sender
{
    internal class MockEmailSenderService : IEmailSenderService
    {
        public string Provider => "MOCK";

        public async Task<EmailSended> SendEmail(Action<SendEmailConfigurations> loadSendEmailConfigurations, SendEmailInstructions instructions)
        {
            return new EmailSended()
            {
                MessageId = "MOCK"
            };
        }
    }
}
