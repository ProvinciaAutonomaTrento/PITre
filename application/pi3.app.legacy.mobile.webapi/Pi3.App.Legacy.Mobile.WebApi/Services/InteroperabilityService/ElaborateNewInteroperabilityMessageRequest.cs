// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Interoperabilita.Semplificata;
using Pi3.App.Legacy.Mobile.WebApi.Services.InteroperabilityService.Domain;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.Legacy.Mobile.WebApi.Services.InteroperabilityService
{
    public class ElaborateNewInteroperabilityMessageRequest
    {
        [Required]
        public InteroperabilityMessage InteroperabilityMessage { get; set; }
    }

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

        public class SenderInfo
        {
            public string Url { get; set; }
            public string Code { get; set; }
            public string AdministrationId { get; set; }
            public string UserId { get; set; }
            public string FileManagerUrl { get; set; }
        }
    }
}
