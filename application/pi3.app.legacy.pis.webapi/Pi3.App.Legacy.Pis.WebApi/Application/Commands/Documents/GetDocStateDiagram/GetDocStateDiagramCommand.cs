// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocStateDiagram
{
    public class GetDocStateDiagramCommand: IRequest<GetDocStateDiagramCommandResponse>
    {        
        public string idDocument {  get; set; }
        public string signature { get; set; }
    }

    public class GetDocStateDiagramCommandResponse
    {
        public StateOfDiagram StateOfDiagram { get; set; }
        public string ErrorMessage { get; set; }
        public GetStateOfDiagramResponseCode Code { get; set; }
    }
    public enum GetStateOfDiagramResponseCode { OK, SYSTEM_ERROR }
}