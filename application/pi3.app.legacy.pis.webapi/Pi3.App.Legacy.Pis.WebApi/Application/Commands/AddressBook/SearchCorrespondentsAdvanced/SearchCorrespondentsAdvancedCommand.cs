// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.SearchCorrespondentsAdvanced
{
    public class SearchCorrespondentsAdvancedCommand : IRequest<SearchCorrespondentsAdvancedCommandResponse>
    {
        public Filter[] Filters { get; set; }
    }

    public class SearchCorrespondentsAdvancedCommandResponse
    {
        public CorrespondentAdvanced[] Correspondents
        {
            get;
            set;
        }
        public string ErrorMessage { get; set; }
        public SearchCorrAdvancedResponseCode Code { get; set; }
    }

    public enum SearchCorrAdvancedResponseCode { OK, SYSTEM_ERROR }
}

