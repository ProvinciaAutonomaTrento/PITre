// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers;
using System.DirectoryServices.ActiveDirectory;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects
{
    public class ProjectWithArchivePlan : Project
    {        
        /// <summary>
        /// Piano di archiviazione
        /// </summary>
        public ArchivePlan ArchivePlan { get; set; }

        /// <summary>
        /// Ulteriori informazioni
        /// </summary>
        public List<FieldLite> OtherProjectInfos { get; set; }
    }
}
