using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using LcEmulatorServices.Interfaces;

namespace LcEmulatorServices
{
    /// <summary>
    /// Summary description for GeneratePdfService
    /// </summary>
    [WebService(Namespace = "http://adobe.com/idp/services")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class GeneratePdfService : System.Web.Services.WebService, IGeneratePdfServiceSoapBinding
    {
        [WebMethod]
        public mapItem[] CreatePDF(BLOB inputDocument, string fileName, string fileTypeSettings, string pdfSettings, string securitySettings, BLOB settingsDocument, BLOB xmpDocument)
        {
            GeneratePDFServiceManager sw = new GeneratePDFServiceManager();
            return sw.CreatePDF(inputDocument, fileName, fileTypeSettings, pdfSettings, securitySettings, settingsDocument, xmpDocument);
            //throw new NotImplementedException();
        }

        [WebMethod]
        public mapItem[] HtmlToPDF(string inputURL, string fileTypeSettings, string securitySettings, BLOB settingsDocument, BLOB xmpDocument)
        {
            throw new NotImplementedException();
        }

        [WebMethod]
        public mapItem[] ExportPDF(BLOB inputDocument, string fileName, string formatType, BLOB settingsDocument)
        {
            throw new NotImplementedException();
        }
    }
}
