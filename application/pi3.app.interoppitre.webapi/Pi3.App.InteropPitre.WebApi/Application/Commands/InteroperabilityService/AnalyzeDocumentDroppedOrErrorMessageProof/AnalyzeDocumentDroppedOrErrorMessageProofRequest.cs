// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.Domain;

namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.AnalyzeDocumentDroppedOrErrorMessageProof
{
    public class AnalyzeDocumentDroppedOrErrorMessageProofRequest : IRequest
    {
        public RecordInfo SenderRecordInfo { get; set; }

        public RecordInfo ReceiverRecordInfo { get; set; }

        public string Reason { get; set; }

        public string ReceiverUrl { get; set; }

        public OperationDiscriminator Operation { get; set; }

        public string ReceiverCode { get; set; }
    }
}
