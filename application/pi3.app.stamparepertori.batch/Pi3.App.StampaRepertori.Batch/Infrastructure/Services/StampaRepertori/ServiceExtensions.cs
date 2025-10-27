// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.Services.Email.Sender;
using Pi3.Core.Services.File.ReportGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.StampaRepertori.Batch.Infrastructure.Services.StampaRepertori
{
    internal static class ServiceExtensions
    {

        public static List<EmailRecipient> AsEmailRecipients(this List<string> recipients)
        {
            var list = new List<EmailRecipient>();

            recipients.ForEach(r => list.Add(new EmailRecipient
            {
                Address = r
            }));

            return list;
        }


    }
}
