// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using Pi3.Core.Services.Email.BoxScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.CheckMailboxHandler;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Segnatura
{
    internal record SegnaturaResult(ProcessorOutput output);

    internal record SegnaturaRequest(AnalyzedMessage message, Email email, Registro reg, string emailAddress, bool isPec, bool fattElDaPec, string salvataggioMail) : IRequest<SegnaturaResult>;
}
