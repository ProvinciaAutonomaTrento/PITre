// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using DocsPaVO.documento;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CestinaDocumento
{
    public class CestinaDocumentoCommand : IRequest<CestinaDocumentoCommandResponse>
    {
        public InfoUtente Infoutente { get; set; }
        public SchedaDocumento SchedaDoc { get; set; }
        public string TipoDoc { get; set; }
        public string Note { get; set; }
    }

    public class CestinaDocumentoCommandResponse
    {
        public bool Output { get; set; }
        public string ErrorMsg { get; set; }
    }
}
