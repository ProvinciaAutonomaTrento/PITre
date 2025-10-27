// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectsByDocument
{
    public class GetProjectsByDocumentCommand: IRequest<GetProjectsByDocumentCommandResponse>
    {        
        public string idDocument { get; set; }
        public string signature {  get; set; }
    }

    public class GetProjectsByDocumentCommandResponse
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