// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.Runtime.Serialization;

namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.Domain
{
    public class SenderInfo
    {
        public string Url { get; set; }

        public string Code { get; set; }

        public string AdministrationId { get; set; }

        public string UserId { get; set; }

        public string FileManagerUrl { get; set; }
    }
}
