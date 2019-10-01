using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.IO;
using LcEmulatorServices.Interfaces;
using System.Net;
namespace LcEmulatorServices
{
    public class GeneratePDFServiceManager 
    {

        public mapItem[] HtmlToPDF(string inputURL, string fileTypeSettings, string securitySettings, BLOB settingsDocument, BLOB xmpDocument)
        {
            WebClient wc = new WebClient();
            byte[] inputContent = wc.DownloadData(inputURL);
            string fileName = "File.html";

            List<mapItem> retvalLst = Invoker(fileName, inputContent);
            return retvalLst.ToArray();
        }


        public mapItem[] CreatePDF(BLOB inputDocument, string fileName, string fileTypeSettings, string pdfSettings, string securitySettings, BLOB settingsDocument, BLOB xmpDocument)
        {

            byte[] inputContent = inputDocument.binaryData;
            List<mapItem> retvalLst = Invoker(fileName, inputContent);
            return retvalLst.ToArray();
        }

        private List<mapItem> Invoker(string fileName, byte[] inputContent)
        {
            bool useShared= true;
            string useAdlib = ConfigurationManager.AppSettings["USE_ADLIB_JOB_TIKET"];
            if (!String.IsNullOrEmpty(useAdlib))
            {
                if (useAdlib.Equals("1"))
                    useShared = false;
                else
                    useShared = true;

            }
            byte[] content;
            List<mapItem> retvalLst = new List<mapItem>();
            if (useShared)
            {
                string inputFolder = ConfigurationManager.AppSettings["PDFCONVERTER_INPUT_FOLDER"];
                string outputFolder = ConfigurationManager.AppSettings["PDFCONVERTER_OUTPUT_FOLDER"];
                string errorFolder = ConfigurationManager.AppSettings["PDFCONVERTER_ERROR_FOLDER"];

                SharedFolderManager.Manager mgr = new SharedFolderManager.Manager();
                content = mgr.convertToPdf(fileName, inputContent, inputFolder,outputFolder,errorFolder);
            }
            else
            {
                string userID = ConfigurationManager.AppSettings["ADLIB_USER_ID"];
                string pass = ConfigurationManager.AppSettings["ADLIB_PASS"];
                string svcUrl = ConfigurationManager.AppSettings["ADLIB_SVCURL"];
                AdlibJobManager.Manager mgr = new AdlibJobManager.Manager(svcUrl, userID, pass);
                content = mgr.convertToPdf(fileName, inputContent);
            }

            retvalLst.Add(convertedDocMapItem(content));
            return retvalLst;
        }


 

        private mapItem convertedDocMapItem (byte[] content)
        {
            return new mapItem { key = "ConvertedDoc", value = new BLOB { binaryData = content } };
        }
    }
}