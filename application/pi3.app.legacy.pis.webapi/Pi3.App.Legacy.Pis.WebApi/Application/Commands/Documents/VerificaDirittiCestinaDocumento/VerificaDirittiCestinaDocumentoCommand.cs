// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.VerificaDirittiCestinaDocumento
{
    public class VerificaDirittiCestinaDocumentoCommand : IRequest<VerificaDirittiCestinaDocumentoCommandResponse>
    {
        public DocsPaVO.utente.InfoUtente InfoUtente { get; set; }
        public DocsPaVO.documento.SchedaDocumento SchedaDoc { get; set; }
    }

    public class VerificaDirittiCestinaDocumentoCommandResponse
    {
        public string Output { get; set; }
    }
}
