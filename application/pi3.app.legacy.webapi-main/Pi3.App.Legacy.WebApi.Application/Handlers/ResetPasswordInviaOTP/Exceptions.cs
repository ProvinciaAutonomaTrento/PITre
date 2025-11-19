// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ResetPasswordInviaOTP
{
    public class UserIdNotFoundPi3Exception : NotFoundPi3Exception
    {
        public UserIdNotFoundPi3Exception(string id)
            : base(ErrorDescriptions.UserIdNotFound, null, ErrorDescriptions.ResourceManager, id)
        {
        }
    }

    public class EmailNotValidPi3Exception : Pi3Exception
    {
        public EmailNotValidPi3Exception(string id)
            : base(ErrorDescriptions.EmailNotValid, null, ErrorDescriptions.ResourceManager, id)
        {
        }

    }

    public class UtenteDiDominioPi3Exception : Pi3Exception
    {
        public UtenteDiDominioPi3Exception(string id)
            : base(ErrorDescriptions.UtenteDiDominio, null, ErrorDescriptions.ResourceManager, id)
        {
        }

    }

    public class SendMailPi3Exception : Pi3Exception
    {
        public SendMailPi3Exception(string message)
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
