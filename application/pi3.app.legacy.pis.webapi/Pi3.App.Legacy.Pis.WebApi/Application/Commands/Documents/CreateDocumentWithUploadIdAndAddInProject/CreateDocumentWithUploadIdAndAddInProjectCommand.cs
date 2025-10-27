// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentWithUploadIdAndAddInProject
{
    public class CreateDocumentWithUploadIdAndAddInProjectCommand : IRequest<CreateDocumentWithUploadIdAndAddInProjectCommandResponse>
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

        /// <summary>
        /// Codice del fascicolo nel quale fascicolare il documento, il codice prende soltanto i fascicoli nei titolari attivi
        /// </summary>
        public string CodeProject
        {
            get;
            set;
        }

        /// <summary>
        /// Id del fascicolo nel quale fascicolare il documento
        /// </summary>
        public string IdProject
        {
            get;
            set;
        }

        /// <summary>
        /// Id del titolario
        /// </summary>
        public string ClassificationSchemeId
        {
            get;
            set;
        }
    }

    public class CreateDocumentWithUploadIdAndAddInProjectCommandResponse
    {
        public Document Document { get; set; }

        public CreateDocumentWithUploadIdResponseCode Code { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum CreateDocumentWithUploadIdResponseCode { OK, SYSTEM_ERROR }
}
