// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.AddSegnaturaXml
{
    public class AddSegnaturaXmlCommand : IRequest<AddSegnaturaXmlCommandResponse>
    {
        public string DocNumber { get; set; }
    }


    public class AddSegnaturaXmlCommandResponse
    {

    }
}
