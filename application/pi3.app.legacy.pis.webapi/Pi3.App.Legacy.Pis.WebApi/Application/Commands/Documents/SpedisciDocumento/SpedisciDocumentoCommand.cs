// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.Spedizione;
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SpedisciDocumento
{
    public class SpedisciDocumentoCommand : IRequest<SpedisciDocumentoCommandResponse>
    {
        public InfoUtente InfoUtente { get; set; }
        public SchedaDocumento Documento { get; set; }
        public SpedizioneDocumento InfoSpedizione { get; set; }
    }

    public class SpedisciDocumentoCommandResponse
    {
        public SpedizioneDocumento SpedizioneDocumento { get; set; }

    }
}
