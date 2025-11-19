// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.CreateDocumentAndAddInProject
{
    public class CreateDocumentAndAddInProjectCommand: IRequest<CreateDocumentAndAddInProjectCommandResponse>
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

    public class CreateDocumentAndAddInProjectCommandResponse
    {
        public Document Document { get; set; }

        public CreateDocumentResponseCode Code { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum CreateDocumentResponseCode { OK, SYSTEM_ERROR }
}