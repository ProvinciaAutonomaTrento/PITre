// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.StampaRepertori.Batch.Infrastructure.Services.StampaRepertori
{
    public class UserNotFoundPi3Exception : NotFoundPi3Exception
    {
        public UserNotFoundPi3Exception()
            : base()
        {
              
        }
    }

    public class RoleNotFoundPi3Exception : NotFoundPi3Exception
    {
        public RoleNotFoundPi3Exception()
            :base()
        {
            
        }
    }

    public class EmailProviderNotFoundPi3Exception : NotFoundPi3Exception
    {
        public EmailProviderNotFoundPi3Exception(string message)
            : base(message)
        {

        }
    }

}
