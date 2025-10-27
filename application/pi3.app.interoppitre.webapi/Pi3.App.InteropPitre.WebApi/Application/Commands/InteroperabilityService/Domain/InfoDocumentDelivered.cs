// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.Domain
{
    public class InfoDocumentDelivered
    {
        public DocumentInfo MainDocument { get; set; }

        public List<DocumentInfo> Attachments { get; set; }
    }
}
