using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class RevocaDelegheRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public List<Delega> Deleghe
        {
            get;
            set;
        }
    }
}
