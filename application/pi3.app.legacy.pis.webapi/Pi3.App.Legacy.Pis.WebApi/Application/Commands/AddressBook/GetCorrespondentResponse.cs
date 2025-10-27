// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook
{
    public class GetCorrespondentResponse
    {
        public Correspondent Correspondent { get; set; }
        public string ErrorMessage { get; set; }
        public GetCorrespondentResponseCode Code { get; set; }
    }
    public enum GetCorrespondentResponseCode { OK, SYSTEM_ERROR }
}
