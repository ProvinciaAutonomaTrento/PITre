using System;
using System.Web;
using System.IO;


public class FileHandler : IHttpHandler
{
    public FileHandler()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    bool IHttpHandler.IsReusable
    {
        get { return true; }
    }

    void IHttpHandler.ProcessRequest(HttpContext context)
    {                       

        if (context.Request.HttpMethod.ToUpperInvariant() == "DELETE")
        {
            String file =  context.Request.PhysicalPath ;
            if (System.IO.File.Exists(file))
            {
                try
                {
                    System.IO.File.Delete(file);
                    context.Response.StatusCode = 200;
                }
                catch
                {
                    context.Response.StatusCode = 500;
                }
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        }
        else if (context.Request.HttpMethod.ToUpperInvariant() == "PUT")
        {
            String file = context.Request.PhysicalPath;
            if (!System.IO.File.Exists(file))
            {
                try
                {
                    using (FileStream writeStream = new FileStream(file, FileMode.Create, FileAccess.Write))
                    {
                        ReadWriteStream(context.Request.InputStream, writeStream);
                        context.Response.StatusCode = 200;
                    }
                }
                catch
                {
                    context.Response.StatusCode = 500;
                    File.Delete(file);
                }
            }
            else
            {
                context.Response.StatusCode =403;
            }
        }
    }

    private void ReadWriteStream(Stream readStream, Stream writeStream)
    {
        int Length = 256;
        Byte[] buffer = new Byte[Length];
        int bytesRead = readStream.Read(buffer, 0, Length);
        // write the required bytes
        while (bytesRead > 0)
        {
            writeStream.Write(buffer, 0, bytesRead);
            bytesRead = readStream.Read(buffer, 0, Length);
        }
        readStream.Close();
        writeStream.Close();
    }
}