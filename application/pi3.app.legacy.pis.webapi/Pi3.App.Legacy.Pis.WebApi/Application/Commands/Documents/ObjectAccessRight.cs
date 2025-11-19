// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class ObjectAccessRight
    {
        public string IdObject { get; set; }
        public string AccessRights { get; set; }
        public string AccessRightsType { get; set; }
        public string SubjectDescription { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectType { get; set; }
        public string SubjectId { get; set; }
        public string AccessDate { get; set; }
        public string Note { get; set; }
    }
}
