// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class LinkedDocument
    {
        public string Id { get; set; }
        public string Signature { get; set; }
        public string Object { get; set; }
        public string DocumentType { get; set; }
        public string LinkType { get; set; }
    }
}
