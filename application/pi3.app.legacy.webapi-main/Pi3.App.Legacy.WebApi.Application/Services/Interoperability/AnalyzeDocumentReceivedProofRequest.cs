// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Interoperabilita.Semplificata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.Interoperability
{
    public class AnalyzeDocumentReceivedProofRequest
    {
        public AnalyzeDocumentReceivedProofRequest() { }
        public AnalyzeDocumentReceivedProofRequest(RecordInfo senderRecordInfo, RecordInfo receiverRecordInfo, string receiverUrl, string receiverCode)
        {
            SenderRecordInfo = senderRecordInfo;
            ReceiverRecordInfo = receiverRecordInfo;
            ReceiverUrl = receiverUrl;
            ReceiverCode = receiverCode;
        }

        public RecordInfo SenderRecordInfo { get; set; }

        public RecordInfo ReceiverRecordInfo { get; set; }

        public string ReceiverUrl { get; set; }

        public string ReceiverCode { get; set; }
    }
}
