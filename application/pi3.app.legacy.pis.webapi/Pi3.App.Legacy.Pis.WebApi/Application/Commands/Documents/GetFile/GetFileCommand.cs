// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.utente;
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetRole;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetFile
{
    public class GetFileCommand : IRequest<GetFileCommandResponse>
    {
        public FileRequest fileRequest { get; set; }
        public bool getFile { get; set; }
        public InfoUtente infoUtente { get; set; }
        public bool segnatura { get; set; }
        public bool timbro { get; set; }
        public string path { get; set; }
        public SchedaDocumento schedaDoc { get; set; }
        public bool fileConFirma { get; set; }
        public labelPdf position { get; set; } = null;

    }

    public class GetFileCommandResponse
    {
        public File File { get; set; }

    }
}
