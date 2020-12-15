using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace ArubaConnector
{
    /// <summary>
    /// Summary description for RemotePdfStamper
    /// </summary>
    [WebService(Namespace = "http://nttdata.com/2014/RPDFStampSvc")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class RemotePdfStamper : System.Web.Services.WebService
    {
        [WebMethod]
        public  byte[] RemotePdfStamp(int Page, int LeftX, int LeftY, int RightX, int RightY, byte[] PdfContent, String StampText)
        {
            RemotePdfStamp rpdfS = new RemotePdfStamp();
            return rpdfS.Stamp(Page, LeftX, LeftY, RightX, RightY, PdfContent, StampText);
        }
    }
}
