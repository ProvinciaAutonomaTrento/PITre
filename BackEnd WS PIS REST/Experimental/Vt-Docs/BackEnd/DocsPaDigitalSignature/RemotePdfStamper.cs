using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace BusinessLogic.Documenti.DigitalSignature
{
	public class RemotePdfStamper
	{
        byte[] _stampedPDF;

        /// <summary>
        /// Stamped PDF result
        /// </summary>
        public byte[] StampedPDF
        {
            get { return _stampedPDF; }
        }

        public RemotePdfStamper(byte[] PdfFile, string  StampText)
        {
            string serviceUrl = Config.RemotePdfStamperServiceUrl();
            PdfBox  box  = Config.RemotePdfStamperCoords();
            externalStamper.RemotePdfStamper stamper = new externalStamper.RemotePdfStamper();
            stamper.Url = serviceUrl;
            //chiamata AL WS
            _stampedPDF = stamper.RemotePdfStamp(box.Page, box.LeftX, box.LeftY, box.RightX, box.RightY, PdfFile, StampText);
        }

        public RemotePdfStamper(int Page, int LeftX, int LeftY, int RightX, int RightY, byte[] PdfFile, string StampText)
        {
            
            string serviceUrl = Config.RemotePdfStamperServiceUrl();
            externalStamper.RemotePdfStamper stamper = new externalStamper.RemotePdfStamper();
            stamper.Url = serviceUrl;
            //chiamata al WS
            _stampedPDF = stamper.RemotePdfStamp(Page, LeftX, LeftY, RightX, RightY, PdfFile, StampText);
        }

        class Config
        {
            public static string RemotePdfStamperServiceUrl()
            {
                try
                {
                    return ConfigurationManager.AppSettings["REMOTE_PDF_STAMPER_SERVICEURL"];
                }
                catch
                {
                    return null;
                }
            }

            public static PdfBox RemotePdfStamperCoords()
            {
                string pdfStampSetting = ConfigurationManager.AppSettings["REMOTE_PDF_STAMPER_COORDS"];
                if (string.IsNullOrEmpty (pdfStampSetting ))
                {
                    pdfStampSetting = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_REMOTE_PDF_STAMPER_COORDS");
                }

                // nulla in config non posso continuare -> magari in futuro possiamo pensare a dei config.
                if (string.IsNullOrEmpty(pdfStampSetting))
                    return null;

                PdfBox retval = new PdfBox(pdfStampSetting);
                
                return retval;
            }
        }

        class PdfBox
        {
            public int Page;
            public int LeftX;
            public int LeftY;
            public int RightX;
            public int RightY;

            public PdfBox()
            {

            }

            public PdfBox (string ConfigString)
            {
                //Stringa in config es -> 1;100;100;200;100
                //                   Pagina;LeX;LeY;RiX;RiY

                string [] valori = ConfigString.Split (';');
                if (valori.Length == 5)
                {
                    Int32.TryParse (valori[0],out this.Page);
                    Int32.TryParse (valori[1],out this.LeftX);
                    Int32.TryParse (valori[2],out this.LeftY);
                    Int32.TryParse (valori[3],out this.RightX);
                    Int32.TryParse (valori[4],out this.RightY);
                }
            }
        }

	}

   
}
