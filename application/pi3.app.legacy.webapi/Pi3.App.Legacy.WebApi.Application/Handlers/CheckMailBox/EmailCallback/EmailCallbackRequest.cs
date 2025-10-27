// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.Core.Services.Email.BoxScanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.EmailCallback
{
    public class EmailCallbackRequest : IRequest<EmailCallbackResponse>
    {
        public Email Email { get; init; } = null!;

        public int Item { get; init; }

        public int Total { get; init; }

        public long IdCheckMailbox { get; init; }

        public bool ProcessOnlyPec { get; init; }

        public EmailBoxTypeEnum EmailBoxType { get; init; }

        public string EmailAddress { get; init; } = null!;

        public long IdRegister { get; init; }

        public DocsPaVO.utente.Registro Registro { get; init; }
    }
}
