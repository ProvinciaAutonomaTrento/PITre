using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class AccettaDelegaRequest
    {
        public DelegaInfo Delega
        {
            get; 
            set;
        }

        public UserInfo UserInfo
        {
            get; 
            set;
        }

        public string IpAddress
        {
            get; 
            set;
        }

        public string SessionId
        {
            get; 
            set; 
        }

    }
}
