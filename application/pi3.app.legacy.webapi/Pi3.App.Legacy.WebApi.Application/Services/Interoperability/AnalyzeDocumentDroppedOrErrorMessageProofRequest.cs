// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Interoperabilita.Semplificata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Services.Interoperability
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
