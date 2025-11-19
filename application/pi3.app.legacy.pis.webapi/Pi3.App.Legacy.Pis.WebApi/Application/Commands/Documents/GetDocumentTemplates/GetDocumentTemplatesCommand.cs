// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocumentTemplates
{
    public class GetDocumentTemplatesCommand: IRequest<GetDocumentTemplatesCommandResponse>
    {        
    }

    public class GetDocumentTemplatesCommandResponse
    {
        public Template[] Templates { get; set; }
        public string ErrorMessage { get; set; }
        public GetTemplatesResponseCode Code { get; set; }
    }
    public enum GetTemplatesResponseCode { OK, SYSTEM_ERROR }

}