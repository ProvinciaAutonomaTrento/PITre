// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.Runtime.Serialization;

namespace Pi3.App.InteropPitre.WebApi.Application.Commands.InteroperabilityService.Domain
{
    public class ReceiverInfo
    {
        public string Code { get; set; }

        public string AOOCode { get; set; }

        public string AdministrationCode { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ReceiverInfo && (obj as ReceiverInfo).AdministrationCode == AdministrationCode && (obj as ReceiverInfo).AOOCode == AOOCode && (obj as ReceiverInfo).Code == Code;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode() + AdministrationCode.GetHashCode() + AOOCode.GetHashCode();
        }
    }
}
