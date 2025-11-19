// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.SearchUsers
{
    public class SearchUsersCommand : IRequest<SearchUsersCommandResponse>
    {
        public Filter[] Filters { get; set; }
    }

    public class SearchUsersCommandResponse
    {
        public User[] Users { get; set; }
        public string ErrorMessage { get; set; }
        public GetUsersResponseCode Code { get; set; }
    }
    public enum GetUsersResponseCode { OK, SYSTEM_ERROR }
}

