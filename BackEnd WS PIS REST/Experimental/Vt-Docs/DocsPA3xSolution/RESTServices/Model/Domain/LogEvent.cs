using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
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