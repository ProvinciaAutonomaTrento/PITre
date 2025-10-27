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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.StandardCase
{
    internal record StandardCaseResult(ProcessorOutput output);

    internal record StandardCaseRequest(string messageId, Email email, Registro reg, string? emailAddress, bool eccSegnatura, bool isPec, string salvataggioMail) : IRequest<StandardCaseResult>;
}
