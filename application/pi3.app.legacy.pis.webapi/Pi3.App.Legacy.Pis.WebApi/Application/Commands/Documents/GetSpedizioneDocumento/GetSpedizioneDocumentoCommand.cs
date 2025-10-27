// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.Spedizione;
using DocsPaVO.utente;
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SendDocument;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSpedizioneDocumento
{
    public class GetSpedizioneDocumentoCommand : IRequest<GetSpedizioneDocumentoCommandResponse>
    {
        public InfoUtente InfoUtente { get; set; }
        public SchedaDocumento Documento { get; set; }
    }

    public class GetSpedizioneDocumentoCommandResponse
    {
        public SpedizioneDocumento SpedizioneDocumento { get; set; }

    }
}
