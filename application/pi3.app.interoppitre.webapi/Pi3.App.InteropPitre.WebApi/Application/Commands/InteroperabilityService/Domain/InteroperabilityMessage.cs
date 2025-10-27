// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.Runtime.Serialization;

namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.Domain
{
    public class InteroperabilityMessage
    {
        public RecordInfo Record { get; set; }

        public SenderInfo Sender { get; set; }

        public List<ReceiverInfo> Receivers { get; set; }

        public DocumentInfo MainDocument { get; set; }

        public List<DocumentInfo> Attachments { get; set; }

        public string Note { get; set; }

        public bool IsPrivate { get; set; }

        public string ReceiverAdministrationCode { get; set; }

        public InteroperabilityMessage()
        {
            Attachments = new List<DocumentInfo>();
            MainDocument = new DocumentInfo();
            Receivers = new List<ReceiverInfo>();
            Record = new RecordInfo();
            Sender = new SenderInfo();
        }
    }
}
