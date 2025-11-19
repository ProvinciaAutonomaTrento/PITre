// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.Domain;
using Pi3.App.InteropPitre.WebApi.Resources;
using Pi3.Core.SeedWork;

namespace Pi3.App.InteropPitre.WebApi.Application.Exceptions
{
    public class TransmissionException : Pi3Exception
    {
        public TransmissionException(string message) : base(message, ErrorDescriptions.ResourceManager, null, new { }) { }

        public TransmissionException(string message, List<ReceiverInfo> receiverInfos) : base(message, ErrorDescriptions.ResourceManager, null, new { })
        {
            this._receiverInfos = receiverInfos;
        }

        public List<ReceiverInfo>? ReceiverInfos { get { return _receiverInfos; } }

        private List<ReceiverInfo>? _receiverInfos;
    }
}
