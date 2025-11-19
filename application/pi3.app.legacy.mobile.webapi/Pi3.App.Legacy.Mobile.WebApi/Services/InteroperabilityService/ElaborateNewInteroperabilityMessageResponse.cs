// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later

using Pi3.App.Legacy.Mobile.WebApi.Services.InteroperabilityService.Domain;

namespace Pi3.App.Legacy.Mobile.WebApi.Services.InteroperabilityService
{
    public class ElaborateNewInteroperabilityMessageResponse
    {
        public ElaborateInteroperabilityMessageResult Result { get; set; }
    }

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

    public class ElaborateInteroperabilitySingleMessage
    {
        public List<ReceiverInfo> Receivers { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class InfoDocumentDelivered
    {
        public DocumentInfo MainDocument { get; set; }

        public List<DocumentInfo> Attachments { get; set; }
    }
}
