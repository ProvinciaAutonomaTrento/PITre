// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentsInProject
{
    public class GetDocumentsInProjectCommand: IRequest<GetDocumentsInProjectCommandResponse>
    {
        /// <summary>
        /// Codice del fascicolo
        /// </summary>
        public string CodeProject
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

        /// <summary>
        /// Pagina che si desidera visualizzare
        /// </summary>
        public int PageNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Quanti elementi sono presenti nella pagina se la ricerca � paginata
        /// </summary>
        public int ElementsInPage
        {
            get;
            set;
        }

        /// <summary>
        /// System id del fascicolo
        /// </summary>
        public string IdProject
        {
            get;
            set;
        }
    }

    public class GetDocumentsInProjectCommandResponse
    {
        /// <summary>
        /// Documenti cercati
        /// </summary>
        public Document[] Documents
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale dei documenti trovati
        /// </summary>
        public int TotalDocumentsNumber
        {
            get;
            set;
        }

        public string ErrorMessage { get; set; }
        public SearchDocumentsResponseCode Code { get; set; }
    }

    public enum SearchDocumentsResponseCode { OK, SYSTEM_ERROR }
}