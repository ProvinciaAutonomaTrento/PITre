// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SearchProjects
{
    public class SearchProjectsCommand: SearchRequest, IRequest<SearchProjectsCommandResponse>
    {        
    }

    public class SearchProjectsCommandResponse
    {
        public Project[] Projects
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale dei fascicoli trovati
        /// </summary>
        public int TotalProjectsNumber
        {
            get;
            set;
        }

        public string ErrorMessage { get; set; }
        public SearchProjectsResponseCode Code { get; set; }
    }

    public enum SearchProjectsResponseCode { OK, SYSTEM_ERROR }
}