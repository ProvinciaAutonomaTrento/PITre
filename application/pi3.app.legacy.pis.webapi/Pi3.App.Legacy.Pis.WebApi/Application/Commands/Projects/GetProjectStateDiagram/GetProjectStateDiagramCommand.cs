// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectStateDiagram
{
    public class GetProjectStateDiagramCommand: IRequest<GetProjectStateDiagramCommandResponse>
    {        
        public string idProject {  get; set; }

        public string classificationSchemeId { get; set; }
        public string codeProject { get; set; }

    }

    public class GetProjectStateDiagramCommandResponse
    {
        public StateOfDiagram StateOfDiagram { get; set; }
        public string ErrorMessage { get; set; }
        public GetStateOfDiagramResponseCode Code { get; set; }
    }
    public enum GetStateOfDiagramResponseCode { OK, SYSTEM_ERROR }
}