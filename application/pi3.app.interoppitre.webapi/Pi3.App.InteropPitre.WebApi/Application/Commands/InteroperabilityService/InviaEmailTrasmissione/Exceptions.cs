// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;

namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.InviaEmailTrasmissione
{
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
