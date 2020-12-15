using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LcEmulatorServices.Interfaces;

namespace LcEmulatorServices
{
    /// <summary>
    /// Summary description for ProcessBarcodeForm
    /// </summary>
    [WebService(Namespace = "http://adobe.com/idp/services")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ProcessBarcodeForm : System.Web.Services.WebService, IProcessBarcodeFormSoapBinding
    {
        public XML invoke(BLOB imageDocument)
        {
            throw new NotImplementedException();
        }
    }
}
