// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.Protocols;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.SearchArchivePlans
{
    public class SearchArchivePlansCommand: Application.Commands.Documents.SearchRequest, IRequest<SearchArchivePlansCommandResponse>
    {        
    }

    public class SearchArchivePlansCommandResponse
    {
        public ArchivePlan[] ArchivePlans
        {
            get;
            set;
        }

        /// <summary>
        /// Numero totale dei fascicoli trovati
        /// </summary>
        public int TotalArchivePlansNumber
        {
            get;
            set;
        }

        public string ErrorMessage { get; set; }
        public SearchArchivePlansResponseCode Code { get; set; }
    }

    public enum SearchArchivePlansResponseCode { OK, SYSTEM_ERROR }
}