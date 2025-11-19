// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox
{
    public class AnalyzedMessage
    {
        public string Id { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Indica il tipo di messaggio ricevuto sulla base dell'analisi degli allegati
        /// </summary>
        public MailProcessedType? EmailAttachmentType { get; set; } = null;

        /// <summary>
        /// Indica se nella mail è stato riconosciuto un DSN
        /// </summary>
        public bool HasDeliveryStatusNotification { get; set; } = false;

        /// <summary>
        /// Indica se la mail ha generato un'eccezione
        /// </summary>
        public bool HasErrors { get; set; } = false;

        /// <summary>
        /// Indica se la mail ha generato un'eccezione bloccante
        /// </summary>
        public bool HasFatalError { get; set; } = false;

        /// <summary>
        /// Indica se la mail è stata correttamente elaborata
        /// </summary>
        public bool Processed { get; set; } = false;

        /// <summary>
        /// Eventuali messaggi di errore ottenuti
        /// </summary>
        public List<string> ErrorMessages { get; set; } = new List<string>();

        public byte[] AttachmentContent { get; set; } = null!;
        public int? ProcessedAttachments { get; set; }
        public long? DocNumber { get; set; }
        public bool AlreadyProcessed { get; set; }
    }
}
