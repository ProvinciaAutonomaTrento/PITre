using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.IO.Compression;

namespace Compression
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpCompressionModule : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(this.OnBeginRequest);
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            // Get the application object to gain access to the request's details
            HttpApplication app = (HttpApplication)sender;
            // Accepted encodings

            string encodings = app.Request.Headers.Get("Accept-Encoding");

            // No encodings; stop the HTTP Module processing
            if (encodings == null)
                return;

            // Current response stream
            Stream baseStream = app.Response.Filter;

            // Find out the requested encoding
            encodings = encodings.ToLower();

            if (encodings.Contains("gzip"))
            {
                app.Response.Filter = new GZipStream(baseStream,
                    CompressionMode.Compress);
                app.Response.AppendHeader("Content-Encoding", "gzip");
                app.Context.Trace.Warn("GZIP compression on");
            }
            else if (encodings.Contains("deflate"))
            {
                app.Response.Filter = new DeflateStream(baseStream,
                    CompressionMode.Compress);
                app.Response.AppendHeader("Content-Encoding", "deflate");
                app.Context.Trace.Warn("Deflate compression on");
            }
        }
    }
}