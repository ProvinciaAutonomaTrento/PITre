using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Globalization;

namespace NttDataWA.Utils
{
    public class PDFDownloadHandler
    {

        #region Fields
        private char[] _commaSplitArray = new char[] { ',' };
        private char[] _dashSplitArray = new char[] { '-' };
        public static string[] _httpDateFormats = new string[] { "r", "dddd, dd-MMM-yy HH':'mm':'ss 'GMT'", "ddd MMM d HH':'mm':'ss yyyy" };
        private const int _bufferSize = 0x1000;
        #endregion

        #region Properties

        public string ContentType { get; private set; }

        public Byte[] FileContent { get; private set; }

        public DateTime FileModificationDate { get; private set; }

        private DateTime HttpModificationDate { get; set; }

        public long FileLength { get; private set; }

        private string EntityTag { get; set; }

        private string FileName { get; set; }

        private long[] RangesStartIndexes { get; set; }

        private long[] RangesEndIndexes { get; set; }

        private bool RangeRequest { get; set; }

        private bool MultipartRequest { get; set; }

        private HttpRequest Request { get; set; }

        private HttpResponse Response { get; set; }

        private bool ChangeSignature { get; set; }

        #endregion



        #region Constructor

        public PDFDownloadHandler(string contentType, string fileName, bool changeSignature, DateTime modificationDate, long fileLength, Byte[] content, HttpRequest request, HttpResponse response)
        {
            if (String.IsNullOrEmpty(contentType))
                throw new ArgumentNullException("contentType");
            Request = request;
            Response = response;
            FileContent = content;
            ContentType = contentType;
            FileName = fileName;
            FileLength = fileLength;
            ChangeSignature = changeSignature;
            FileModificationDate = modificationDate;
            //Modification date for header values comparisons purposes
            HttpModificationDate = modificationDate.ToUniversalTime();
            HttpModificationDate = new DateTime(HttpModificationDate.Year, HttpModificationDate.Month, HttpModificationDate.Day, HttpModificationDate.Hour, HttpModificationDate.Minute, HttpModificationDate.Second, DateTimeKind.Utc);
        }

        #endregion

        protected string GenerateEntityTag()
        {
            //Generate entity tag based on file name and length
            byte[] entityTagBytes = Encoding.ASCII.GetBytes(String.Format("{0}|{1}", FileName, FileLength.ToString()));
            return "W/"+Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(entityTagBytes));
        }



        private void GetRanges(HttpRequest request)
        {
            //Get "Range" header from request

            string rangesHeader = GetHeader(request, "Range");
            //Get "If-Range" header from request
            string ifRangeHeader = GetHeader(request, "If-Range", EntityTag);
            DateTime ifRangeHeaderDate;
            bool isIfRangeHeaderDate = DateTime.TryParseExact(ifRangeHeader, _httpDateFormats, null, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out ifRangeHeaderDate);
            //If there is no "Range" header,
            //or the entity tag from "If-Range" header does not match this entity tag,
            //or the modification date is greater than modification date from "If-Range" header

            if ((String.IsNullOrEmpty(rangesHeader) /*&& !SameETag(request)*/) || (!isIfRangeHeaderDate && ifRangeHeader != EntityTag) || (isIfRangeHeaderDate && HttpModificationDate > ifRangeHeaderDate))
            {
                //Return entire file
                RangesStartIndexes = new long[] { 0 };
                RangesEndIndexes = new long[] { FileLength - 1 };
                RangeRequest = false;
                MultipartRequest = false;
            }
            //Otherwise
            else
            {
                //Split "Range" header value into ranges
                string[] ranges = rangesHeader.Replace("bytes=", String.Empty).Split(_commaSplitArray);

                RangesStartIndexes = new long[ranges.Length];
                RangesEndIndexes = new long[ranges.Length];
                RangeRequest = true;
                MultipartRequest = (ranges.Length > 1);

                //Get the star and end index for the range 
                for (int i = 0; i < ranges.Length; i++)
                {
                    string[] currentRange = ranges[i].Split(_dashSplitArray);

                    if (String.IsNullOrEmpty(currentRange[1]))
                        RangesEndIndexes[i] = FileLength - 1;
                    else
                        RangesEndIndexes[i] = Int64.Parse(currentRange[1]);

                    if (String.IsNullOrEmpty(currentRange[0]))
                    {
                        RangesStartIndexes[i] = FileLength - 1 - RangesEndIndexes[i];
                        RangesEndIndexes[i] = FileLength - 1;
                    }
                    else
                        RangesStartIndexes[i] = Int64.Parse(currentRange[0]);
                    //long chunkSize = RangesEndIndexes[i] - RangesStartIndexes[i];
                    //if (FileLength > 100000000 && (chunkSize >= (FileLength / 2)))
                    //{
                    //    chunkSize = 65536;
                    //    RangesEndIndexes[i] = RangesStartIndexes[i] + chunkSize;
                    //}

                }
            }
        }

        private bool ValidateRanges(HttpResponse response)
        {
            if (FileLength > Int32.MaxValue)
            {
                response.StatusCode = 413;
                return false;
            }

            for (int i = 0; i < RangesStartIndexes.Length; i++)
            {
                if (RangesStartIndexes[i] > FileLength - 1 || RangesEndIndexes[i] > FileLength - 1 || RangesStartIndexes[i] < 0 || RangesEndIndexes[i] < 0 || RangesEndIndexes[i] < RangesStartIndexes[i])
                {
                    response.StatusCode = 400;
                    return false;
                }

            }


            return true;
        }

        private string GetHeader(HttpRequest request, string header, string defaultValue = "")
        {
            return String.IsNullOrEmpty(request.Headers[header]) ? defaultValue : request.Headers[header].Replace("\"", String.Empty);
        }
        public bool SameETag(HttpRequest request)
        {
            bool sameETag = false;
            string noneMatchHeader = GetHeader(request, "If-None-Match");
            //if (String.IsNullOrEmpty(noneMatchHeader))
            //    sameETag = true;
            string[] entitiesTags = noneMatchHeader.Split(_commaSplitArray);
            foreach (string entityTag in entitiesTags)
            {
                if (EntityTag == entityTag)
                {
                    sameETag = true;
                }
            }

            return sameETag;
        }
        private bool ValidateModificationDate(HttpRequest request, HttpResponse response)
        {
            //First validate "If-Modified-Since" header
            string modifiedSinceHeader = GetHeader(request, "If-Modified-Since");


            if (!String.IsNullOrEmpty(modifiedSinceHeader))
            {
                DateTime modifiedSinceDate;
                DateTime.TryParseExact(modifiedSinceHeader, _httpDateFormats, null, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out modifiedSinceDate);

                if (HttpModificationDate <= modifiedSinceDate && SameETag(request))
                {
                    response.StatusCode = 304;
                    return false;
                }
            }

            //Then validate "If-Unmodified-Since" or "Unless-Modified-Since"
            string unmodifiedSinceHeader = GetHeader(request, "If-Unmodified-Since", GetHeader(request, "Unless-Modified-Since"));
            if (!String.IsNullOrEmpty(unmodifiedSinceHeader))
            {
                DateTime unmodifiedSinceDate;
                bool unmodifiedSinceDateParsed = DateTime.TryParseExact(unmodifiedSinceHeader, _httpDateFormats, null, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out unmodifiedSinceDate);

                if (HttpModificationDate > unmodifiedSinceDate)
                {
                    response.StatusCode = 412;
                    return false;
                }
            }

            return true;
        }

        protected void WriteEntireEntity(HttpResponse response)
        {
            response.BinaryWrite(FileContent);
        }

        protected void WriteEntityRange(HttpResponse response, long rangeStartIndex, long rangeEndIndex)
        {
            using (Stream stream = new MemoryStream(FileContent))
            {
                stream.Seek(rangeStartIndex, SeekOrigin.Begin);

                int bytesRemaining = Convert.ToInt32(rangeEndIndex - rangeStartIndex) + 1;
                byte[] buffer = new byte[_bufferSize];

                while (bytesRemaining > 0)
                {
                    int bytesRead = stream.Read(buffer, 0, _bufferSize < bytesRemaining ? _bufferSize : bytesRemaining);
                    response.OutputStream.Write(buffer, 0, bytesRead);
                    bytesRemaining -= bytesRead;
                }

                stream.Close();
            }
        }

        private int GetContentLength(string boundary)
        {
            int contentLength = 0;

            for (int i = 0; i < RangesStartIndexes.Length; i++)
            {
                contentLength += Convert.ToInt32(RangesEndIndexes[i] - RangesStartIndexes[i]) + 1;

                if (MultipartRequest)
                    contentLength += boundary.Length + ContentType.Length + RangesStartIndexes[i].ToString().Length + RangesEndIndexes[i].ToString().Length + FileLength.ToString().Length + 49;
            }

            if (MultipartRequest)
                contentLength += boundary.Length + 4;

            return contentLength;
        }

        private bool ValidateEntityTag(HttpRequest request, HttpResponse response)
        {
            //Get "If-Match" header from request
            string matchHeader = GetHeader(request, "If-Match");

            //If header exists and it's value is different from "*"
            if (!String.IsNullOrEmpty(matchHeader) && matchHeader != "*")
            {
                //Split header value into list of etity tags
                string[] entitiesTags = matchHeader.Split(_commaSplitArray);
                int entitieTagIndex;
                for (entitieTagIndex = 0; entitieTagIndex < entitiesTags.Length; entitieTagIndex++)
                {
                    if (EntityTag == entitiesTags[entitieTagIndex])
                        break;
                }

                //If our entity tag wasn't found
                if (entitieTagIndex >= entitiesTags.Length)
                {
                    //Set proper response status code
                    response.StatusCode = 412;
                    return false;
                }
            }

            //Get "If-None-Match" header from request
            string noneMatchHeader = GetHeader(request, "If-None-Match");

            //If header exists
            if (!String.IsNullOrEmpty(noneMatchHeader))
            {
                //If header value equals "*"
                if (noneMatchHeader == "*")
                {
                    //Set proper response status code
                    response.StatusCode = 412;
                    return false;
                }

                //Split header value into list of etity tags
                string[] entitiesTags = noneMatchHeader.Split(_commaSplitArray);
                foreach (string entityTag in entitiesTags)
                {
                    if (EntityTag == entityTag)
                    {
                        //Set proper response status code
                        response.AddHeader("ETag", String.Format("\"{0}\"", entityTag));
                        response.StatusCode = 304;
                        return false;
                    }
                }
            }

            return true;
        }

        public bool Validate(HttpRequest request, HttpResponse response)
        {
            return ValidateRanges(Response) && ValidateEntityTag(Request, Response) && ValidateModificationDate(Request, Response);
        }


        public void ManageMultipartDownload(/*Byte[] content, String name, HttpRequest request, HttpResponse response*/)
        {

            //Request = request;
            //Response = response;
            //FileName = name;
            ////if (FileContent == null)
            ////    FileContent = File.ReadAllBytes(FilePath);
            //FileLength = content.Length;
            //FileContent = content;
            //ContentType = "application/pdf";
            //Generate entity tag
            EntityTag = GenerateEntityTag();
            //Get ranges from request
            GetRanges(Request);

            //If all validations are successful
            if (ChangeSignature || Validate(Request, Response))
            {
                //Set common headers
                //Response.Buffer = false;
                Response.AddHeader("Last-Modified", FileModificationDate.ToString("r"));
                Response.AddHeader("ETag", String.Format("\"{0}\"", EntityTag));
                //Response.Cache.SetCacheability(HttpCacheability.Public); //required for etag output
                //Response.Cache.SetETag(EntityTag);
                Response.AddHeader("Accept-Ranges", "bytes");
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", "inline;filename=" + FileName);
                //If this is not range Request
                if (!RangeRequest)
                {
                    //Set standard headers
                    Response.AddHeader("Content-Length", FileLength.ToString());
                    //Response.AddHeader("Content-Type", "application/pdf");
                    //Set status code
                    Response.StatusCode = 200;

                    //If this is not HEAD Request
                    if (!Request.HttpMethod.Equals("HEAD"))
                    {
                        //Write entire file to Response
                        Console.WriteLine("Request.HttpMethod : " + Request.HttpMethod);
                        Console.WriteLine("Write entire file to Response");
                        WriteEntireEntity(Response);
                    }
                }
                //If this is range Request
                else
                {
                    string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

                    //Compute and set content length
                    Response.AddHeader("Content-Length", GetContentLength(boundary).ToString());

                    //If this is not multipart Request
                    if (!MultipartRequest)
                    {
                        //Set content range and type
                        Response.AddHeader("Content-Range", String.Format("bytes {0}-{1}/{2}", RangesStartIndexes[0], RangesEndIndexes[0], FileLength));
                        Response.ContentType = ContentType;
                    }
                    //Otherwise
                    else
                        //Set proper content type
                        Response.ContentType = String.Format("multipart/byteranges; boundary={0}", boundary);

                    //Set status code
                    Response.StatusCode = 206;

                    //If this not HEAD Request
                    if (!Request.HttpMethod.Equals("HEAD"))
                    {
                        //For each Requested range
                        for (int i = 0; i < RangesStartIndexes.Length; i++)
                        {
                            //If this is multipart Request
                            if (MultipartRequest)
                            {
                                //Write additional multipart info
                                Response.Write(String.Format("--{0}\r\n", boundary));
                                Response.Write(String.Format("Content-Type: {0}\r\n", ContentType));
                                Response.Write(String.Format("Content-Range: bytes {0}-{1}/{2}\r\n\r\n", RangesStartIndexes[i], RangesEndIndexes[i], FileLength));
                            }

                            //Write range (with multipart separator if required)
                            if (Response.IsClientConnected)
                            {
                                Console.Write("WriteEntityRange");
                                WriteEntityRange(Response, RangesStartIndexes[i], RangesEndIndexes[i]);
                                if (MultipartRequest)
                                    Response.Write("\r\n");
                                Response.Flush();
                            }
                            else
                                return;
                        }

                        //If this is multipart Request
                        if (MultipartRequest)
                            Response.Write(String.Format("--{0}--", boundary));
                    }
                }
            }
        }
    }
}
