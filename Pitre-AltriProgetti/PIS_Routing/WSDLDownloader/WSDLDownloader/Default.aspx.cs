using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;



namespace WSDLDownloader
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        protected void Download_AddressBook_Click(object sender, EventArgs e)
        {
            //
            //New Version
            string URIAddressBook = string.Empty;
            //URIAddressBook = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/AddressBook.svc?wsdl";
            URIAddressBook = System.Configuration.ConfigurationManager.AppSettings["URI_AddressBook"];

            string fileName = "AddressBook.wsdl";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URIAddressBook);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string HTML = reader.ReadToEnd();

            Response.ContentType = "Text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.Write(HTML);
            Response.End();

            reader.Close();
            reader.Dispose();
            //
            //

            //
            //oldVersion
            #region oldVersion
            //string URIAddressBook = string.Empty;
            //URIAddressBook = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/AddressBook.svc?wsdl";

            //string fileName = "AddressBook.wsdl";

            ////string filePath = @"C:/";
            //string filePath = string.Empty;
            //filePath = System.Configuration.ConfigurationManager.AppSettings["FILE_PATH"];

            //// Create a new WebClient instance.
            //WebClient myWebClient = new WebClient();

            //try
            //{
            //    // Download the Web resource and save it into the current filesystem folder.
            //    myWebClient.DownloadFile(URIAddressBook, @filePath + fileName);
            //    RegisterStartupScript("Download Success", "<script>alert('Download avvenuto con successo.');</script>");
            //}
            //catch (Exception ex)
            //{
            //    RegisterStartupScript("Errore Download AddressBook", "<script>alert('Non è stato possibile completare il download del file. Contattare l'amministratore del sistema.');</script>");
            //}
            #endregion
            //
            //
        }

        protected void Download_Authentication_Click(object sender, EventArgs e)
        {
            //
            //New Version
            string URIAuthentication = string.Empty;
            //URIAuthentication = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Authentication.svc?wsdl";
            URIAuthentication = System.Configuration.ConfigurationManager.AppSettings["URI_Authentication"];

            string fileName = "Authentication.wsdl";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URIAuthentication);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string HTML = reader.ReadToEnd();

            Response.ContentType = "Text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.Write(HTML);
            Response.End();

            reader.Close();
            reader.Dispose();
            //
            //

            //
            //oldVersion
            #region oldVersion
            //string URIAuthentication = string.Empty;
            //URIAuthentication = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Authentication.svc?wsdl";

            //string fileName = "Authentication.wsdl";

            ////string filePath = @"C:/";
            //string filePath = string.Empty;
            //filePath = System.Configuration.ConfigurationManager.AppSettings["FILE_PATH"];

            //// Create a new WebClient instance.
            //WebClient myWebClient = new WebClient();

            //try
            //{
            //    // Download the Web resource and save it into the current filesystem folder.
            //    myWebClient.DownloadFile(URIAuthentication, @filePath + fileName);
            //    RegisterStartupScript("Download Success", "<script>alert('Download avvenuto con successo.');</script>");
            //}
            //catch (Exception ex)
            //{
            //    RegisterStartupScript("Errore Download Authentication", "<script>alert('Non è stato possibile completare il download del file. Contattare l'amministratore del sistema.');</script>");
            //}
            #endregion
            //
            //
        }

        protected void Download_ClassificationSchemes_Click(object sender, EventArgs e)
        {
            //
            //New Version
            string URIClassificationSchemes = string.Empty;
            //URIClassificationSchemes = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/ClassificationSchemes.svc?wsdl";
            URIClassificationSchemes = System.Configuration.ConfigurationManager.AppSettings["URI_ClassificationSchemes"];

            string fileName = "ClassificationSchemes.wsdl";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URIClassificationSchemes);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string HTML = reader.ReadToEnd();

            Response.ContentType = "Text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.Write(HTML);
            Response.End();

            reader.Close();
            reader.Dispose();
            //
            //

            //
            //oldVersion
            #region oldVersion
            //string URIClassificationSchemes = string.Empty;
            //URIClassificationSchemes = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/ClassificationSchemes.svc?wsdl";

            //string fileName = "ClassificationSchemes.wsdl";

            ////string filePath = @"C:/";
            //string filePath = string.Empty;
            //filePath = System.Configuration.ConfigurationManager.AppSettings["FILE_PATH"];

            //// Create a new WebClient instance.
            //WebClient myWebClient = new WebClient();

            //try
            //{
            //    // Download the Web resource and save it into the current filesystem folder.
            //    myWebClient.DownloadFile(URIClassificationSchemes, @filePath + fileName);
            //    RegisterStartupScript("Download Success", "<script>alert('Download avvenuto con successo.');</script>");
            //}
            //catch (Exception ex)
            //{
            //    RegisterStartupScript("Errore Download ClassificationSchemes", "<script>alert('Non è stato possibile completare il download del file. Contattare l'amministratore del sistema.');</script>");
            //}
            #endregion
            //
            //
        }

        protected void Download_Documents_Click(object sender, EventArgs e)
        {
            //
            //New Version
            string URIDocuments = string.Empty;
            //URIDocuments = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Documents.svc?wsdl";
            URIDocuments = System.Configuration.ConfigurationManager.AppSettings["URI_Documents"];

            string fileName = "Documents.wsdl";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URIDocuments);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string HTML = reader.ReadToEnd();

            Response.ContentType = "Text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.Write(HTML);
            Response.End();

            reader.Close();
            reader.Dispose();
            //
            //

            //
            //oldVersion
            #region oldVersion
            //string URIDocuments = string.Empty;
            //URIDocuments = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Documents.svc?wsdl";

            //string fileName = "Documents.wsdl";

            ////string filePath = @"C:/";
            //string filePath = string.Empty;
            //filePath = System.Configuration.ConfigurationManager.AppSettings["FILE_PATH"];

            //// Create a new WebClient instance.
            //WebClient myWebClient = new WebClient();

            //try
            //{
            //    // Download the Web resource and save it into the current filesystem folder.
            //    myWebClient.DownloadFile(URIDocuments, @filePath + fileName);
            //    RegisterStartupScript("Download Success", "<script>alert('Download avvenuto con successo.');</script>");
            //}
            //catch (Exception ex)
            //{
            //    RegisterStartupScript("Errore Download Documents", "<script>alert('Non è stato possibile completare il download del file. Contattare l'amministratore del sistema.');</script>");
            //}
            #endregion
            //
            //
        }

        protected void Download_Projects_Click(object sender, EventArgs e)
        {
            //
            //New Version
            string URIProjects = string.Empty;
            //URIProjects = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Projects.svc?wsdl";
            URIProjects = System.Configuration.ConfigurationManager.AppSettings["URI_Projects"];

            string fileName = "Projects.wsdl";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URIProjects);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string HTML = reader.ReadToEnd();

            Response.ContentType = "Text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.Write(HTML);
            Response.End();

            reader.Close();
            reader.Dispose();
            //
            //

            //
            //oldVersion
            #region oldVersion
            //string URIProjects = string.Empty;
            //URIProjects = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Projects.svc?wsdl";

            //string fileName = "Projects.wsdl";

            ////string filePath = @"C:/";
            //string filePath = string.Empty;
            //filePath = System.Configuration.ConfigurationManager.AppSettings["FILE_PATH"];

            //// Create a new WebClient instance.
            //WebClient myWebClient = new WebClient();

            //try
            //{
            //    // Download the Web resource and save it into the current filesystem folder.
            //    myWebClient.DownloadFile(URIProjects, @filePath + fileName);
            //    RegisterStartupScript("Download Success", "<script>alert('Download avvenuto con successo.');</script>");
            //}
            //catch (Exception ex)
            //{
            //    RegisterStartupScript("Errore Download Projects", "<script>alert('Non è stato possibile completare il download del file. Contattare l'amministratore del sistema.');</script>");
            //}
            #endregion
            //
            //
        }

        protected void Download_Registers_Click(object sender, EventArgs e)
        {
            //
            //New Version
            string URIRegisters = string.Empty;
            //URIRegisters = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Registers.svc?wsdl";
            URIRegisters = System.Configuration.ConfigurationManager.AppSettings["URI_Registers"];

            string fileName = "Registers.wsdl";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URIRegisters);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string HTML = reader.ReadToEnd();

            Response.ContentType = "Text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.Write(HTML);
            Response.End();

            reader.Close();
            reader.Dispose();
            //
            //

            //
            //oldVersion
            #region oldVersion
            //string URIRegisters = string.Empty;
            //URIRegisters = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Registers.svc?wsdl";

            //string fileName = "Registers.wsdl";

            ////string filePath = @"C:/";
            //string filePath = string.Empty;
            //filePath = System.Configuration.ConfigurationManager.AppSettings["FILE_PATH"];

            //// Create a new WebClient instance.
            //WebClient myWebClient = new WebClient();

            //try
            //{
            //    // Download the Web resource and save it into the current filesystem folder.
            //    myWebClient.DownloadFile(URIRegisters, @filePath + fileName);
            //    RegisterStartupScript("Download Success", "<script>alert('Download avvenuto con successo.');</script>");
            //}
            //catch (Exception ex)
            //{
            //    RegisterStartupScript("Errore Download Registers", "<script>alert('Non è stato possibile completare il download del file. Contattare l'amministratore del sistema.');</script>");
            //}
            #endregion
            //
            //
        }

        protected void Download_Roles_Click(object sender, EventArgs e)
        {
            //
            //New Version
            string URIRoles = string.Empty;
            //URIRoles = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Roles.svc?wsdl";
            URIRoles = System.Configuration.ConfigurationManager.AppSettings["URI_Roles"];

            string fileName = "Roles.wsdl";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URIRoles);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string HTML = reader.ReadToEnd();

            Response.ContentType = "Text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.Write(HTML);
            Response.End();

            reader.Close();
            reader.Dispose();
            //
            //

            //
            //oldVersion
            #region oldVersion
            //string URIRoles = string.Empty;
            //URIRoles = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Roles.svc?wsdl";

            //string fileName = "Roles.wsdl";

            ////string filePath = @"C:/";
            //string filePath = string.Empty;
            //filePath = System.Configuration.ConfigurationManager.AppSettings["FILE_PATH"];

            //// Create a new WebClient instance.
            //WebClient myWebClient = new WebClient();

            //try
            //{
            //    // Download the Web resource and save it into the current filesystem folder.
            //    myWebClient.DownloadFile(URIRoles, @filePath + fileName);
            //    RegisterStartupScript("Download Success", "<script>alert('Download avvenuto con successo.');</script>");
            //}
            //catch (Exception ex)
            //{
            //    RegisterStartupScript("Errore Download Roles", "<script>alert('Non è stato possibile completare il download del file. Contattare l'amministratore del sistema.');</script>");
            //}
            #endregion
            //
            //
        }

        protected void Download_Token_Click(object sender, EventArgs e)
        {
            //
            //New Version
            string URIToken = string.Empty;
            //URIToken = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Token.svc?wsdl";
            URIToken = System.Configuration.ConfigurationManager.AppSettings["URI_Token"];

            string fileName = "Token.wsdl";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URIToken);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string HTML = reader.ReadToEnd();

            Response.ContentType = "Text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.Write(HTML);
            Response.End();

            reader.Close();
            reader.Dispose();
            //
            //

            //
            //Old Version
            #region oldVersion
            //string URIToken = string.Empty;
            //URIToken = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Token.svc?wsdl";

            //string fileName = "Token.wsdl";

            ////string filePath = @"C:/";
            //string filePath = string.Empty;
            //filePath = System.Configuration.ConfigurationManager.AppSettings["FILE_PATH"];

            //// Create a new WebClient instance.
            //WebClient myWebClient = new WebClient();

            //try
            //{
            //    // Download the Web resource and save it into the current filesystem folder.
            //    myWebClient.DownloadFile(URIToken, @filePath + fileName);
            //    RegisterStartupScript("Download Success", "<script>alert('Download avvenuto con successo.');</script>");
            //}
            //catch (Exception ex)
            //{
            //    RegisterStartupScript("Errore Download Token", "<script>alert('Non è stato possibile completare il download del file. Contattare l'amministratore del sistema.');</script>");
            //}
            #endregion
            //
            //
        }

        protected void Download_Transmissions_Click(object sender, EventArgs e)
        {

            //
            //New Version
            string URITransmissions = string.Empty;
            URITransmissions = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Transmissions.svc?wsdl";
            URITransmissions = System.Configuration.ConfigurationManager.AppSettings["URI_Transmissions"];

            string fileName = "Transmissions.wsdl";

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(URITransmissions);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string HTML = reader.ReadToEnd();

            Response.ContentType = "Text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.Write(HTML);
            Response.End();

            reader.Close();
            reader.Dispose();
            //
            //

            //
            //Old Version
            #region oldVersion
            //string URITransmissions = string.Empty;
            //URITransmissions = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Transmissions.svc?wsdl";

            //string fileName = "Transmissions.wsdl";

            ////string filePath = @"C:/";
            //string filePath = string.Empty;
            //filePath = System.Configuration.ConfigurationManager.AppSettings["FILE_PATH"];

            //// Create a new WebClient instance.
            //WebClient myWebClient = new WebClient();

            //try
            //{
            //    // Download the Web resource and save it into the current filesystem folder.
            //    myWebClient.DownloadFile(URITransmissions, @filePath + fileName);
            //    RegisterStartupScript("Download Success", "<script>alert('Download avvenuto con successo.');</script>");
            //}
            //catch (Exception ex)
            //{
            //    RegisterStartupScript("Errore Download Transmission", "<script>alert('Non è stato possibile completare il download del file. Contattare l'amministratore del sistema.');</script>");
            //}
            #endregion
            //
            //

        }

        //
        //Link Di Prova
        protected void Download_Prova_Click(object sender, EventArgs e)
        {
            string url = "http://172.20.15.28/infotn_coll-be/VtDocsWS/WebServices/Transmissions.svc?wsdl";
            
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string HTML = reader.ReadToEnd();
            
            Response.ContentType = "Text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=Transmissions.wsdl");
            Response.Write(HTML);
            Response.End();
            
            reader.Close();
            reader.Dispose();
        }
    }
}