using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LcEmulatorServices.Interfaces;

namespace LcEmulatorServices
{
    /// <summary>
    /// Summary description for ProcessForm
    /// </summary>
    [WebService(Namespace = "http://adobe.com/idp/services")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ProcessForm : System.Web.Services.WebService, IProcessFormSoapBinding
    {
        public bool invoke(BLOB formPdf, out XML xmlOutput)
        {
            throw new NotImplementedException();
        }
    }
}
