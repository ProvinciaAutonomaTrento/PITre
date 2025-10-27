// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSegnaturaRepertorio
{
    public class GetSegnaturaRepertorioCommand : IRequest<GetSegnaturaRepertorioResponse>
    {
        public string Docnumber { get; set; }
        public string CodiceAmm { get; set; }
    }
    public class GetSegnaturaRepertorioResponse
    {
        public string Output { get; set; }

        public GetSegnaturaRepertorioResponse()
        {
        }

        public GetSegnaturaRepertorioResponse(string output)
        {
            Output = output;
        }
    }
}
