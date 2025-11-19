// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetInfoFile
{
    public class DocumentoGetInfoFileCommand : IRequest<DocumentoGetInfoFileCommandResponse>
    {
        public FileRequest fileRequest { get; set; }
        public InfoUtente infoUtente { get; set; }
    }
    public class DocumentoGetInfoFileCommandResponse
    {
        public FileDocumento output { get; set; }
        public DocumentoGetInfoFileCommandResponse()
        {

        }
        public DocumentoGetInfoFileCommandResponse(FileDocumento _output)
        {
            output = _output;
        }
    }
}
