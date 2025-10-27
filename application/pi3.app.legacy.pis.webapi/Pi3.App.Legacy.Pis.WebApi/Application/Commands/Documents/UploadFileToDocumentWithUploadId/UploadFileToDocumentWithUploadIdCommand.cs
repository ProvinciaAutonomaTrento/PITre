// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.UploadFileToDocumentWithUploadId
{
    public class UploadFileToDocumentWithUploadIdCommand: IRequest<UploadFileToDocumentWithUploadIdCommandResponse>
    {
        /// <summary>
        /// DocNumber del documento a cui associare il documento o una nuova versione
        /// </summary>
        public string IdDocument
        {
            get;
            set;
        }

        /// <summary>
        /// File da acquisire
        /// </summary>
        public Guid? UploadId 
        { 
            get; 
            set; 
        } 
        
        /// <summary>
        /// Se true indica la creazione di un nuovo allegato per il documento
        /// </summary>
        public bool CreateAttachment
        {
            get;
            set;
        }

        /// <summary>
        /// Descrizione in caso di nuovo allegato
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Se true converte il file in pdf/a
        /// </summary>
        public bool CovertToPDFA
        {
            get;
            set;
        }

        /// <summary>
        /// Hash del file passato nell'attributo Domain.File File
        /// </summary>
        public string HashFile
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo di Attachment Esterno=E ; Utente=U; se null o Empy  = U
        /// </summary>
        public string AttachmentType
        {
            get;
            set;
        }
    }

    public class UploadFileToDocumentWithUploadIdCommandResponse
    {
        public string ErrorMessage { get; set; }
        public UploadFileToDocumentWithUploadIdResponseCode Code { get; set; }
        public string ResultMessage { get; set; }
    }

    public enum UploadFileToDocumentWithUploadIdResponseCode { OK, SYSTEM_ERROR }
}