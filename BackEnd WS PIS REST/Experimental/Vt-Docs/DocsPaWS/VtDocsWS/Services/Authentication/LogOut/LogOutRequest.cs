using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace VtDocsWS.Services.Authentication.LogOut
{
    /// <summary>
    /// Oggetto contenente i dati di richiesta da fornire al servizio di "LogOut"
    /// </summary>
   [DataContract]
    public class LogOutRequest : Request
    {

    }
}