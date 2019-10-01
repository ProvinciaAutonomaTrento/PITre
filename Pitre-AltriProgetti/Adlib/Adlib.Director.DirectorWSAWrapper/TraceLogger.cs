////////////////////////////////////////////////////////////////////////////
/*
Copyright (C) 1997-2010 Adlib Software
All rights reserved.

DISCLAIMER OF WARRANTIES:
 
Permission is granted to copy this Sample Code for internal use only, 
provided that this permission notice and warranty disclaimer appears in all copies.
 
SAMPLE CODE IS LICENSED TO YOU AS-IS.
 
ADLIB SOFTWARE AND ITS SUPPLIERS AND LICENSORS DISCLAIM ALL WARRANTIES, 
EITHER EXPRESS OR IMPLIED, IN SUCH SAMPLE CODE, INCLUDING THE WARRANTY OF 
NON-INFRINGEMENT AND THE IMPLIED WARRANTIES OF MERCHANTABILITY OR FITNESS FOR A 
PARTICULAR PURPOSE. IN NO EVENT WILL ADLIB SOFTWARE OR ITS LICENSORS OR SUPPLIERS 
BE LIABLE FOR ANY DAMAGES ARISING OUT OF THE USE OF OR INABILITY TO USE THE SAMPLE 
APPLICATION OR SAMPLE CODE, DISTRIBUTION OF THE SAMPLE APPLICATION OR SAMPLE CODE, 
OR COMBINATION OF THE SAMPLE APPLICATION OR SAMPLE CODE WITH ANY OTHER CODE. 
IN NO EVENT SHALL ADLIB SOFTWARE OR ITS LICENSORS AND SUPPLIERS BE LIABLE FOR ANY 
LOST REVENUE, LOST PROFITS OR DATA, OR FOR DIRECT, INDIRECT, SPECIAL, CONSEQUENTIAL, 
INCIDENTAL OR PUNITIVE DAMAGES, HOWEVER CAUSED AND REGARDLESS OF THE THEORY OF LIABILITY, 
EVEN IF ADLIB SOFTWARE OR ITS LICENSORS OR SUPPLIERS HAVE BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
*/
////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;


namespace Adlib.Director.DirectorWSAWrapper
{
    [Serializable()]
    public class TraceResults
    {
        private List<TraceHelper.TraceRecord> _TraceList = new List<Adlib.Director.DirectorWSAWrapper.TraceHelper.TraceRecord>();
        public TraceHelper.TraceRecord[] TraceList
        {
            get { return _TraceList.ToArray(); }
            //set { _TraceList = value; }
        }


    }

    [Serializable()]
    public class TraceLogger
    {
        private List<TraceHelper.TraceRecord> _TraceList = new List<Adlib.Director.DirectorWSAWrapper.TraceHelper.TraceRecord>();
        private string _OutputFilePath = null;
        private bool _Enable = false;
        private bool _WrapSoapMessageInCDATA = false;

        private const string PutJobFilesMethodName = "PutJobFiles";
        private const string PutJobFilesBinaryBegin = "<jobFileBuffers><base64Binary>";
        private const string PutJobFilesBinaryEnd = "</base64Binary></jobFileBuffers>";
        
        private const string GetJobFilesMethodName = "GetJobFiles";
        private const string GetJobFilesBinaryBegin = "<GetJobFilesResult><base64Binary>";
        private const string GetJobFilesBinaryEnd = "</base64Binary></GetJobFilesResult>";

        private const string CDATAStart = "<![CDATA[";
        private const string CDATAEnd = "]]>";


        
        private static TraceLogger _TraceLoggerInstance;
        private static object _LockObject = new object();
        private TraceLogger()
        {
            FileInfo fiAssembly = new FileInfo(Assembly.GetCallingAssembly().Location);
            _OutputFilePath = Path.Combine(fiAssembly.DirectoryName, "TempAdlibTraceLog.xml");
        }

        public static TraceLogger GetInstance()
        {
            if (_TraceLoggerInstance == null)
            {
                lock (_LockObject)
                {
                    _TraceLoggerInstance = new TraceLogger();
                }
            }
            return _TraceLoggerInstance;
        }
        [XmlIgnore]
        public bool Enable
        {
            get { return _Enable; }
            set { _Enable = value; }
        }

        [XmlArray("SoapTraceList")]
        [XmlArrayItem("SoapTraceRecord")]
        public List<TraceHelper.TraceRecord> TraceList
        {
            get
            {
                if (_TraceList == null)
                {
                    _TraceList = new List<TraceHelper.TraceRecord>();
                }
                return _TraceList;
            }
            set
            {
                _TraceList = value;
            }
        }
        
        public void AddTraceRecord(TraceHelper.TraceRecord traceRecord)
        {
            if (_Enable && traceRecord!= null)
            {
                if (_WrapSoapMessageInCDATA)
                {
                    traceRecord.SoapMessage = CDATAStart + traceRecord.SoapMessage + CDATAEnd;
                }
                else
                {
                    // we need to cut binary file data which will be in PutJobFiles and GetJobFiles calls,
                    // otherwise XML will be huge and bad-formatted
                    if (string.Compare(traceRecord.MethodName, PutJobFilesMethodName, false) == 0)
                    {
                        try
                        {
                            int messageLength = traceRecord.SoapMessage.Length;
                            int indexBeginBinary = traceRecord.SoapMessage.IndexOf(PutJobFilesBinaryBegin);
                            int indexEndBinary = traceRecord.SoapMessage.IndexOf(PutJobFilesBinaryEnd);
                            if (indexBeginBinary > 0 && indexEndBinary > 0)
                            {
                                indexBeginBinary += PutJobFilesBinaryBegin.Length;
                                if (indexBeginBinary < messageLength && indexEndBinary < messageLength)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.Append(traceRecord.SoapMessage.Substring(0, indexBeginBinary));
                                    sb.Append("**Here goes your file's binary data in base64Binary encoding.**");
                                    sb.Append(traceRecord.SoapMessage.Substring(indexEndBinary, messageLength - indexEndBinary));
                                    traceRecord.SoapMessage = sb.ToString();
                                }
                            }
                        }
                        catch { }

                    }
                    if (string.Compare(traceRecord.MethodName, GetJobFilesMethodName, false) == 0)
                    {
                        try
                        {
                            int messageLength = traceRecord.SoapMessage.Length;
                            int indexBeginBinary = traceRecord.SoapMessage.IndexOf(GetJobFilesBinaryBegin);
                            int indexEndBinary = traceRecord.SoapMessage.IndexOf(GetJobFilesBinaryEnd);
                            if (indexBeginBinary > 0 && indexEndBinary > 0)
                            {
                                indexBeginBinary += GetJobFilesBinaryBegin.Length;
                                if (indexBeginBinary < messageLength && indexEndBinary < messageLength)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    sb.Append(traceRecord.SoapMessage.Substring(0, indexBeginBinary));
                                    sb.Append("**Here goes your file's binary data in base64Binary encoding.**");
                                    sb.Append(traceRecord.SoapMessage.Substring(indexEndBinary, messageLength - indexEndBinary));
                                    traceRecord.SoapMessage = sb.ToString();
                                }
                            }
                        }
                        catch { }

                    }
                }
                lock (_LockObject)
                {
                    _TraceList.Add(traceRecord);
                }
            }
        }

        public void Clear()
        {
            lock(_LockObject)
            {
                _TraceList.Clear();
            }
        }

        public string GetTraceXmlText()
        {
            string traceXmlText = null;
            if (_TraceList.Count == 0)
            {
                return null;
            }
            try
            {
                traceXmlText = Serialize.SerializeObject(this, typeof(TraceLogger));

            }
            catch 
            {

            }
            return traceXmlText;
        }
        public string GetFileWithData()
        {
            if (File.Exists(_OutputFilePath))
            {
                File.Delete(_OutputFilePath);
            }

            if (_TraceList.Count == 0)
            {
                return null;
            }
            try
            {
                string traceOutput = Serialize.SerializeObject(this, typeof(TraceLogger));

                System.IO.FileStream fs = new System.IO.FileStream(_OutputFilePath, FileMode.OpenOrCreate | System.IO.FileMode.Append, System.IO.FileAccess.Write);
                System.IO.StreamWriter streamWriter = new StreamWriter(fs);
                streamWriter.Write(traceOutput);
                streamWriter.Flush();
                streamWriter.Close();
                fs.Close();
            }
            catch 
            {

            }
            return _OutputFilePath;
        }
    }
}
