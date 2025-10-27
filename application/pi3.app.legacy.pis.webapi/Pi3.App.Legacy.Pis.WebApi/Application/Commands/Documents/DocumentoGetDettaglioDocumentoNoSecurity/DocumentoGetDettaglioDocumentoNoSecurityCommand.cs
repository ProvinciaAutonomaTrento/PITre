// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using DocsPaVO.documento;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetDettaglioDocumentoNoSecurity
{
    public class DocumentoGetDettaglioDocumentoNoSecurityCommand : IRequest<DocumentoGetDettaglioDocumentoNoSecurityCommandResponse>
    {
        public InfoUtente Infoutente { get; set; }
        public string IdProfile { get; set; }
        public string DocNumber { get; set; }
    }

    public class DocumentoGetDettaglioDocumentoNoSecurityCommandResponse 
    {
        public SchedaDocumento Output{ get; set; }
    }
}
