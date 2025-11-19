// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.ConsolidateDocumentById
{
    public class ConsolidateDocumentByIdCommand : IRequest<ConsolidateDocumentByIdCommandResponse>
    {
        public DocsPaVO.utente.InfoUtente UserInfo { get; set; }
        public string IdDocument { get; set; }
        public DocsPaVO.documento.DocumentConsolidationStateEnum ToState { get; set; }
    }

    public class ConsolidateDocumentByIdCommandResponse
    {
        public DocumentConsolidationStateInfo Output{ get; set; }
    }
}
