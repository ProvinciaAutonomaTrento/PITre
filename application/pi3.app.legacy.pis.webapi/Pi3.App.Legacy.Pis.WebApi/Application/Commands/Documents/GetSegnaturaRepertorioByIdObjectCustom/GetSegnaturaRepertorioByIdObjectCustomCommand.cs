// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSegnaturaRepertorioByIdObjectCustom
{
    public class GetSegnaturaRepertorioByIdObjectCustomCommand : IRequest<GetSegnaturaRepertorioByIdObjectCustomResponse>
    {
        public string DocNumber {get; set;}
        public string CodiceAmm {get; set;}
        public string IdObjectCustom { get; set; }
    }
    public class GetSegnaturaRepertorioByIdObjectCustomResponse
    {
        public string Output { get; set; }
        public GetSegnaturaRepertorioByIdObjectCustomResponse()
        {
            
        }
        public GetSegnaturaRepertorioByIdObjectCustomResponse(string output)
        {
            Output = output;
        }
    }
}
