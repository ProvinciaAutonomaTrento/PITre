// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class GetDocumentResponse
    {
        public Document Document { get; set; }

        public GetDocumentResponseCode Code { get; set; }
        public string ErrorMessage { get; set; }
    }

    public enum GetDocumentResponseCode { OK, SYSTEM_ERROR }
}
