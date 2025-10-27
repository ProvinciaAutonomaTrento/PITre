// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Annullamento
{
    internal record AnnullamentoResult(ProcessorOutput output);

    internal record AnnullamentoRequest(byte[] content, string mailId) : IRequest<AnnullamentoResult>;

}
