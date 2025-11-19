// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Email.BoxScanner;
using Pi3.Core.Services.Factory;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.Email.Sender
{
    public abstract class SendEmailConfigurations : ValueObject
    {
        //public StringDictionary? Arguments { get; init; } = null;
    }
}
