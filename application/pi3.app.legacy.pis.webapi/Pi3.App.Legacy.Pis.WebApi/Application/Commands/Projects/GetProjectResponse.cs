// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects
{
    public class GetProjectResponse
    {
        public Project Project { get; set; }
        public string ErrorMessage { get; set; }
        public GetProjectResponseCode Code { get; set; }
    }
    public enum GetProjectResponseCode { OK, SYSTEM_ERROR }
}
