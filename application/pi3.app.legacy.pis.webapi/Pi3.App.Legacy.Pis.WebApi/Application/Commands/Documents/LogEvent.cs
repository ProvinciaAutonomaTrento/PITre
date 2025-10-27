// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class LogEvent
    {
        public string Id { get; set; }
        public string ActionDate { get; set; }
        public string OperatorUsername { get; set; }
        public string OperatorPeopleID { get; set; }
        public string OperatorGroupID { get; set; }
        public string AdministrationId { get; set; }
        public string ObjectDescription { get; set; }
        public string ActionCode { get; set; }
        public string OperationExecuted { get; set; }
        public string OperatorDescription { get; set; }
    }
}
