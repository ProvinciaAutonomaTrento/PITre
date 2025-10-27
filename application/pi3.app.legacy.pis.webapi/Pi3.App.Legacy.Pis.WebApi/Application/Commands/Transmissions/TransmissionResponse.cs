// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions
{
    public class TransmissionResponse
    {
        public string TransmMessage { get; set; }
        public string ErrorMessage { get; set; }
        public TransmissionResponseCode Code { get; set; }
    }

    public enum TransmissionResponseCode { OK, SYSTEM_ERROR }
}

