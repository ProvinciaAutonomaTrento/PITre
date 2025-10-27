// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pi3.Core.Services.Email.BoxScanner;
using static Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.CheckMailboxHandler;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Daticert
{
    internal record DaticertResult(ProcessorOutput output);

    internal record DatiCertRequest(Email email, string idRegistro, AnalyzedMessage message) : IRequest<DaticertResult>;
}
