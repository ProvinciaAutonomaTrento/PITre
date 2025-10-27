// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class MessageResponse
    {
        public string ResultMessage { get; set; }
        public string ErrorMessage { get; set; }
        public MessageResponseCode Code { get; set; }
    }
    public enum MessageResponseCode { OK, SYSTEM_ERROR }
}
