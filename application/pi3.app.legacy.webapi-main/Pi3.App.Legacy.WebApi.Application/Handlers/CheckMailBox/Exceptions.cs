// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox
{
    public class CheckMailBoxPi3Exception : Pi3Exception
    {
        public CheckMailBoxPi3Exception(string message)
            : base(message)
        {
        }
    }

    public class RegisterNotFoundPi3Exception : NotFoundPi3Exception
    {
        public RegisterNotFoundPi3Exception(string message)
            : base(message)
        {
        }
    }

    public class ProviderNotFoundPi3Exception : NotFoundPi3Exception
    {
        public ProviderNotFoundPi3Exception(string message)
            : base(message)
        {
        }
    }
}
