// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetTemplateProject
{
    public class GetTemplateProjectCommand: IRequest<GetTemplateProjectCommandResponse>
    {        
        public string descriptionTemplate {  get; set; }
        public string idTemplate { get; set; }
    }

    public class GetTemplateProjectCommandResponse
    {
        public Template Template { get; set; }
        public string ErrorMessage { get; set; }
        public GetTemplateResponseCode Code { get; set; }
    }
    public enum GetTemplateResponseCode { OK, SYSTEM_ERROR }

}