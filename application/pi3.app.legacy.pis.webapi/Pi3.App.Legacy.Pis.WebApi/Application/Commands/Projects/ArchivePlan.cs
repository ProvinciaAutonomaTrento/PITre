// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects
{
    public class ArchivePlan
    {
        public string Id { get; set; }
        public string ClassificationNodeCode { get; set; }
        public string ClassificationNodeId { get; set; }

        public string ProceedingVoice { get; set; }
        public string ProceedingNumber { get; set; }
        public string Description { get; set; }
        public string ArchivePeriod { get; set; }

        public string ArchivePeriodYears { get; set; }
        public string ProjectClosureNotes { get; set; }
        public string DocumentDiscardabilityNotes { get; set; }
        public string DocumentNotes { get; set; }
        //public string ClassificationSchemeId { get; set; }
        public ClassificationScheme ClassificationScheme { get; set; }
        //public string AdministrationId { get; set; }
        //public string RegisterId { get; set; }

        public Register Register { get; set; }

        /// <summary>
        /// Lista di informazioni aggiuntive
        /// </summary>
        public List<FieldLite> OtherArchivePlanInfos
        {
            get;
            set;
        }
    }
}
