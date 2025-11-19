// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocument
{
    public class GetDocumentCommand: IRequest<GetDocumentCommandResponse>
    {        
        /// <summary>
        /// Id del documento
        /// </summary>
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// Segnatura del documento
        /// </summary>
        public string Signature
        {
            get;
            set;
        }

        /// <summary>
        /// Se true oltre ai metadati restituisce anche il contenuto del file principale e allegati
        /// </summary>
        public bool GetFile
        {
            get;
            set;
        }

        /// <summary>
        /// Se pari a uno, restituisce il file con l'estensione della firma (p7m)
        /// </summary>
        public string GetFileWithSignature
        {
            get;
            set;
        }
    }

    public class GetDocumentCommandResponse: GetDocumentResponse
    {
	}
}