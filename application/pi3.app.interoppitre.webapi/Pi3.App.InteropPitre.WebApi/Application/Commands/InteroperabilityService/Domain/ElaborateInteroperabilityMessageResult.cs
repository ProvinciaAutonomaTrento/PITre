// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.Runtime.Serialization;

namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.Domain
{
    public class ElaborateInteroperabilityMessageResult
    {
        public string? MessageId { get; set; }

        public List<ElaborateInteroperabilitySingleMessage> SingleRequestErrors { get; set; }

        public InfoDocumentDelivered? DocumentDelivered { get; set; }

        public ElaborateInteroperabilityMessageResult()
        {
            SingleRequestErrors = new List<ElaborateInteroperabilitySingleMessage>();
        }
    }
}
