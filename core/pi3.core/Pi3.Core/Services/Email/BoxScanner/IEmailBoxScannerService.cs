// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.Services.Factory;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.Email.BoxScanner
{
    public interface IEmailBoxScannerService : IService
    {
        string Provider { get; }

        //Task Scan(EmailBoxConfigurations emailBoxConfigurations, Func<EmailBoxScannerCallback, bool> callback);

        Task Scan(Action<EmailBoxConfigurations> loadEmailBoxConfigurations, Func<EmailBoxScannerCallback, bool> callback);

        Task<Email> Parse(Stream stream);
    }
}