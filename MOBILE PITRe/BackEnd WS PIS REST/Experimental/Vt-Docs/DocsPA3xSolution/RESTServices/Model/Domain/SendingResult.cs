using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTServices.Model.Domain
{
    public class SendingResult
    {
        public string CorrespondentId
        {
            get;
            set;
        }
        public string CorrespondentDescription
        {
            get;
            set;
        }
        public string Mail
        { get; set; }

        public string PrefChannel
        { get; set; }

        public string SendResult
        { get; set; }
    }
}