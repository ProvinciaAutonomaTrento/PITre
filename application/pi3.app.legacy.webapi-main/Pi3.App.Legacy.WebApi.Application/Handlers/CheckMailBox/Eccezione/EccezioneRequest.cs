// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;
using Pi3.Core.Services.Email.BoxScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.CheckMailboxHandler;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Eccezione
{
    internal record EccezioneResult(ProcessorOutput output);

    internal record EccezioneRequest(Pi3.Core.Services.Email.BoxScanner.Email email, AnalyzedMessage message, Registro reg) : IRequest<EccezioneResult>;

}
