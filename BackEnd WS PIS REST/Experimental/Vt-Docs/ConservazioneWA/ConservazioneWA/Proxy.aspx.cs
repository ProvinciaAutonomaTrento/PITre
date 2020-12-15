using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using ConservazioneWA.Utils;

namespace ConservazioneWA
{
    public partial class _Proxy : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            string thisPage = this.Request.CurrentExecutionFilePath + "?u=";
            string proxyURL = string.Empty;
            try
            {
                proxyURL = HttpUtility.UrlDecode(Request.QueryString["u"].ToString());
            }
            catch { }

            if (proxyURL != string.Empty)
            {
                try
                {
                    string storageBase = ConservazioneManager.httpStorageRemoteUrlAddress();

                    Uri proxURI = new Uri(storageBase + proxyURL);
                    string lastArg = proxURI.Segments.LastOrDefault();
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(proxURI.OriginalString);
                    request.Method = "GET";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode.ToString().ToLower() == "ok")
                    {
                        string contentType = response.ContentType;
                        Stream content = response.GetResponseStream();
                        StreamReader contentReader = new StreamReader(content);
                        if (string.IsNullOrEmpty ( contentType))
                        {
                            if (lastArg.ToLower ().EndsWith (".html"))
                                contentType = "text/html";

                            if (lastArg.ToLower().EndsWith(".htm"))
                                contentType = "text/htm";

                            if (lastArg.ToLower().EndsWith(".css"))
                                contentType = "text/css";

                            if (lastArg.ToLower().EndsWith(".zip"))
                                contentType = "application/x-zip-compressed";
                        }
                        Response.ContentType = contentType;
                        if (contentType.ToLower().Contains("text"))
                        {

                           
                            string page = contentReader.ReadToEnd();

                            if (page.Contains("href=\"../static/main.css\""))
                                page = page.Replace("href=\"../static/main.css\"", "Href='" + thisPage + proxyURL.Replace(lastArg, string.Empty) + "../static/main.css'");

                            page = page.Replace("href=\"", "href=\"" + thisPage + proxyURL);

                            //href="../static/main.css"

                            page = page.Replace("href='", "href='" + thisPage + proxyURL);
                            page = page.Replace("src=\"", "src=\"" + thisPage + proxyURL);

                            if (page.Contains(lastArg))
                            {
                                if (!lastArg.EndsWith ("/"))
                                    page = page.Replace(lastArg, string.Empty);
                            }

                            if (Response.ContentType.ToLower().Contains("text/css"))
                            {

                                proxyURL = proxyURL.Replace(lastArg, string.Empty);
                                page = page.Replace("url(", "url(" + thisPage + proxyURL);
                            }

                            Response.Write(page);
                        }
                        else
                        {
                           // BinaryReader bcont = new BinaryReader(content);
                            //int len = (int)response.ContentLength;
                           // byte[] con = bcont.ReadBytes (len);
                                //content-disposition:attachment; filename=68702.zip
                                
                      
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                byte[] buffer = new byte[8192];
                                int bytesRead;
                                while ((bytesRead = content.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    memoryStream.Write(buffer, 0, bytesRead);
                                }
                                if (contentType.ToLower().Contains("zip"))
                                {
                                    Response.AddHeader("Content-Disposition", "attachment; filename=" + lastArg);
                                    Response.AddHeader("Content-Length", memoryStream.Length.ToString());
                                }
                                Response.BinaryWrite(memoryStream.ToArray());
                            }


                            
                        }

                    }
                }
                catch
                {
                    Response.StatusCode = 404;
                }
            }

        }
    }
}