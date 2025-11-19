// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.ImportPreviousDocument
{
    public class ImportPreviousDocumentCommand: IRequest<ImportPreviousDocumentCommandResponse>
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
        /// Codice dell'RF in cui si vuole protocollare (opzionale)
        /// </summary>
        public string CodeRF
        {
            get;
            set;
        }
    }

    public class ImportPreviousDocumentCommandResponse
    {
        public Document Document { get; set; }

        public CreateDocumentResponseCode Code { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum CreateDocumentResponseCode { OK, SYSTEM_ERROR }

}