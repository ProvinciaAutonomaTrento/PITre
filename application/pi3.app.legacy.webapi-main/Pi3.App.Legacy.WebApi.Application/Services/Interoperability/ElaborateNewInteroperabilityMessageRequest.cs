// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Interoperabilita.Semplificata;
using Pi3.App.Legacy.WebApi.Application.Services.Interoperability.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.Interoperability
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
