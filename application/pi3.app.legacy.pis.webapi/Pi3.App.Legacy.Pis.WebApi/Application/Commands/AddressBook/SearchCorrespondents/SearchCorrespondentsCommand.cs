// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.SearchCorrespondents
{
    public class SearchCorrespondentsCommand : IRequest<SearchCorrespondentsCommandResponse>
    {
        public Filter[] Filters { get; set; }
    }

    public class SearchCorrespondentsCommandResponse
    {
        public Correspondent[] Correspondents
        {
            get;
            set;
        }
        public string ErrorMessage { get; set; }
        public SearchCorrespondentsResponseCode Code { get; set; }
    }

    public enum SearchCorrespondentsResponseCode { OK, SYSTEM_ERROR }
}
