// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SearchDocuments
{
    public class SearchDocumentsCommand: SearchRequest, IRequest<SearchDocumentsCommandResponse>
    {        
    }

    public class SearchDocumentsCommandResponse
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