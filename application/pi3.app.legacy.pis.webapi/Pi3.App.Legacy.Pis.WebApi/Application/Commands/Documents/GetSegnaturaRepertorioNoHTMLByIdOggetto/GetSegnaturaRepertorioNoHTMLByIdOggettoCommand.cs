// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSegnaturaRepertorioByIdObjectCustom
{
    public class GetSegnaturaRepertorioNoHTMLByIdOggettoCommand : IRequest<GetSegnaturaRepertorioNoHTMLByIdOggettoResponse>
    {
        public string DocNumber {get; set;}
        public string CodiceAmm {get; set;}
        public string IdObjectCustom { get; set; }
    }
    public class GetSegnaturaRepertorioNoHTMLByIdOggettoResponse
    {
        public string Output { get; set; }
        public GetSegnaturaRepertorioNoHTMLByIdOggettoResponse()
        {
            
        }
        public GetSegnaturaRepertorioNoHTMLByIdOggettoResponse(string output)
        {
            Output = output;
        }
    }
}
