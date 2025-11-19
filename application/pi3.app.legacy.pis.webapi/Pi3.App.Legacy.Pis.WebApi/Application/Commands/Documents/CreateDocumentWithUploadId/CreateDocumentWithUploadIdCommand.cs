// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentWithUploadId
{
    public class CreateDocumentWithUploadIdCommand : IRequest<CreateDocumentWithUploadIdCommandResponse>
    {
        /// <summary>
        /// Nel caso di protocollo specificare il registro
        /// </summary>
        public string CodeRegister
        {
            get;
            set;
        }

        /// <summary>
        /// Documento che si vuole creare
        /// </summary>
        public Document Document
        {
            get;
            set;
        }

        /// <summary>
        /// Documento principale
        /// </summary>
        public Guid? MainDocumentUploadId
        {
            get;
            set;
        }

        /// <summary>
        /// Allegati del documento
        /// </summary>
        public Guid[]? AttachmentsUploadIds
        {
            get;
            set;
        }

        /// <summary>
        /// Codice dell'RF in cui si vuole protocollare (opzionale)
        /// </summary>
        public string CodeRF
        {
            get;
            set;
        }
    }

    public class CreateDocumentWithUploadIdCommandResponse
    {
        public Document Document { get; set; }

        public CreateDocumentWithUploadIdResponseCode Code { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum CreateDocumentWithUploadIdResponseCode { OK, SYSTEM_ERROR }
}
