// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetVersionsMainDocument
{
    public class GetVersionsMainDocumentCommand : IRequest<GetVersionsMainDocumentCommandResponse>
    {
        public InfoUtente InfoUser{ get; set; }
        public string DocNumber{ get; set; }
    }

    public class GetVersionsMainDocumentCommandResponse
    {
        public DocsPaVO.documento.Documento[] Output { get; set; }
    }
}
