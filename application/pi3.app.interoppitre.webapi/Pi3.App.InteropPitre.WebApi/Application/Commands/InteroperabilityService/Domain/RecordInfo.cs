// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.Runtime.Serialization;

namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.Domain
{
    public class RecordInfo
    {
        public string AdministrationCode { get; set; }

        public string RecordNumber { get; set; }

        public DateTime RecordDate { get; set; }

        public string AOOCode { get; set; }

        public string Subject { get; set; }
    }
}
