// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectFolders
{
    public class GetProjectFoldersCommand: IRequest<GetProjectFoldersCommandResponse>
    {        
        public string idProject {  get; set; }
        public string classificationSchemeId { get; set; }
        public string codeProject { get; set; }
    }

    public class GetProjectFoldersCommandResponse
    {
        public string IdProject { get; set; }

        public string ProjectDescription { get; set; }

        public Folder[] Folders { get; set; }

        public string ErrorMessage { get; set; }
        public GetProjectFoldersResponseCode Code { get; set; }
    }
    public enum GetProjectFoldersResponseCode { OK, SYSTEM_ERROR }
}