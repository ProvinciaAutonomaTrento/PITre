// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.CheckMailboxHandler;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Conferma
{
    internal record ConfermaResult(ProcessorOutput output);

    internal record ConfermaRequest(Pi3.Core.Services.Email.BoxScanner.Email email, AnalyzedMessage message, DocsPaVO.utente.Registro Reg, string MailId, string MailAddress) : IRequest<ConfermaResult>;
}
