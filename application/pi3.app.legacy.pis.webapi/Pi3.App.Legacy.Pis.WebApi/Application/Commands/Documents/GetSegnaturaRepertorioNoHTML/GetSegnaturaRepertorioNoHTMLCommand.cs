// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSegnaturaRepertorioNoHTML
{
    public class GetSegnaturaRepertorioNoHTMLCommand : IRequest<GetSegnaturaRepertorioNoHTMLResponse>
    {
        public string DocNumber { get; set; }
        public string CodiceAmm { get; set; }
    }
    public class GetSegnaturaRepertorioNoHTMLResponse
    {
        public string Output { get; set; }
        public GetSegnaturaRepertorioNoHTMLResponse()
        {
            
        }
        public GetSegnaturaRepertorioNoHTMLResponse(string output)
        {
            Output = output;
        }
    }
}