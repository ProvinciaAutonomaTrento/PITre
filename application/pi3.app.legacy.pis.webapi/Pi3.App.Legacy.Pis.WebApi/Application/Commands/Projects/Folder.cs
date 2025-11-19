// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects
{
    public class Folder
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string IdParent { get; set; }
        public string IdProject { get; set; }
        public string CreationDate { get; set; }
    }
}
