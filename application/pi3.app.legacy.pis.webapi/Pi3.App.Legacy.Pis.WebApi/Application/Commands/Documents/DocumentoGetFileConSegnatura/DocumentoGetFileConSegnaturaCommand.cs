// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetFileConSegnatura
{
    public class DocumentoGetFileConSegnaturaCommand : IRequest<DocumentoGetFileConSegnaturaCommandResponse>
    {
        public FileRequest fileRequest { get; set; }
        public SchedaDocumento sch { get; set; }
        public InfoUtente infoUtente { get; set; }
        public labelPdf position { get; set; }
        public bool Forced { get; set; }
    }

    public class DocumentoGetFileConSegnaturaCommandResponse
    {
        public FileDocumento fileDocumento { get; set; }
    }
}
