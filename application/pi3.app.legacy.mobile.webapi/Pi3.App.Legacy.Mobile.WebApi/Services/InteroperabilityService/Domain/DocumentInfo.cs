// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;

namespace Pi3.App.Legacy.Mobile.WebApi.Services.InteroperabilityService.Domain
{
    public class DocumentInfo
    {
        public string Name { get; set; }
        public int NumberOfPages { get; set; }
        public string DocumentServerLocation { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string DocumentNumber { get; set; }
        public string VersionLabel { get; set; }
        public string VersionId { get; set; }
        public string Version { get; set; }
        public string Fingerprint { get; set; }
        public TipoFirmaEnum Signature { get; set; }
    }
}
