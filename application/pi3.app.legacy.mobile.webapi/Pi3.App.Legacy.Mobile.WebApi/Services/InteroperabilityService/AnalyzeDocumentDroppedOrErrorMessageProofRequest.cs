// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Interoperabilita.Semplificata;

namespace Pi3.App.Legacy.Mobile.WebApi.Services.InteroperabilityService
{
    public class AnalyzeDocumentDroppedOrErrorMessageProofRequest
    {
        public RecordInfo SenderRecordInfo { get; set; }

        public RecordInfo ReceiverRecordInfo { get; set; }

        public string Reason { get; set; }

        public string ReceiverUrl { get; set; }

        public OperationDiscriminator Operation { get; set; }

        public string ReceiverCode { get; set; }
    }

    public enum OperationDiscriminator
    {
        Drop,
        Error
    }
}
