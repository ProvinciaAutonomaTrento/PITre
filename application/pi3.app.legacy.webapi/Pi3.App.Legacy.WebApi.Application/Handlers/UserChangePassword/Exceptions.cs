// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UserChangePassword
{
    public class UserNotFoundPi3Exception : NotFoundPi3Exception
    {
        public UserNotFoundPi3Exception()
            : base(ErrorDescriptions.UserNotFound, null, ErrorDescriptions.ResourceManager)
        {
        }
    }

    public class PasswordEqualityPi3Exception : Pi3Exception
    {
        public PasswordEqualityPi3Exception()
            : base(ErrorDescriptions.PasswordEquality, null, ErrorDescriptions.ResourceManager)
        {
        }
    }
}
