// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetAllegati
{
    public class DocumentoGetAllegatiCommand : IRequest<DocumentoGetAllegatiCommandResponse>
    {
        public string DocNumber { get; set; }
        public string FilterAllegatiPec { get; set; }
        public string SimplifiedInteroperabilityId { get; set; }
    }
    public class DocumentoGetAllegatiCommandResponse
    {
        public DocsPaVO.documento.Allegato[] output { get; set; }

        public DocumentoGetAllegatiCommandResponse()
        {

        }
        public DocumentoGetAllegatiCommandResponse(DocsPaVO.documento.Allegato[] _output)
        {
            output = _output;
        }
    }
}
