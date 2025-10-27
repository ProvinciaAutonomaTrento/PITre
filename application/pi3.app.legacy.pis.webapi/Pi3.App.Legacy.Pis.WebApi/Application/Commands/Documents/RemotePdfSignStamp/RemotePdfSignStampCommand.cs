// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.RemotePdfSignStamp
{
    public class RemotePdfSignStampCommand : IRequest<RemotePdfSignStampCommandResponse>
    {
        public labelPdf labelPdf { get; set; }
        public InfoUtente infoUtente { get; set; }
        public FileRequest fr { get; set; }
        public SchedaDocumento schedaDoc { get; set; }
    }

    public class RemotePdfSignStampCommandResponse
    {
        public SchedaDocumento output { get; set; }
        public ResultSigilloElettronico result { get; set; }

    }
}
