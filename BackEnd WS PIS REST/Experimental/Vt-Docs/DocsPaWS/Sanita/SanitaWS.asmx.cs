using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using DocsPaVO.utente;
using DocsPaWS.Sanita.VO.Responses;
using DocsPaWS.Sanita.VO.Requests;

namespace DocsPaWS.Sanita
{
    /// <summary>
    /// Summary description for SanitaWS
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SanitaWS : System.Web.Services.WebService
    {

        [WebMethod]
        public ImportAttestatiResponse ImportAttestati(ImportAttestatiRequest request)
        {
            return SanitaManager.ImportAttestati(request);
        }

        [WebMethod]
        public InteropCreazioneDocResponse InteropCreazioneDoc(InteropCreazioneDocRequest request)
        {
            return SanitaManager.InteropCreazioneDoc(request);
        }
    }
}
