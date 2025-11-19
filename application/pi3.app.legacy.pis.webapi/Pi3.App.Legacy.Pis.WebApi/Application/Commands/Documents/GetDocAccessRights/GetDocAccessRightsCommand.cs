// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocAccessRights
{
    public class GetDocAccessRightsCommand: IRequest<GetDocAccessRightsCommandResponse>
    {        
        public string IdDocument {  get; set; }
    }

    public class GetDocAccessRightsCommandResponse
    {
        public List<ObjectAccessRight> AccessRights
        {
            get;
            set;
        }
        public string ErrorMessage { get; set; }
        public GetDocAccessRightsResponseCode Code { get; set; }
    }

    public enum GetDocAccessRightsResponseCode { OK, SYSTEM_ERROR }
}