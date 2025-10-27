// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Interoperabilita.Semplificata;

namespace Pi3.App.Legacy.Mobile.WebApi.Services.InteroperabilityService
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
