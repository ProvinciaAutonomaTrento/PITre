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
using System.Web.Services;
using System.Web.Services.Protocols;
using System.IO;
namespace Adlib.Director.DirectorWSAWrapper.TraceHelper
{
    public enum TraceMessageType
    {
        SoapResponse,
        SoapRequest
    }
    [Serializable()]
    public class TraceRecord
    {
        private TraceMessageType _MessageType;
        private string _MethodName;
        private DateTime _MessageDT;
        private string _SoapMessage;

        public TraceRecord() { }
        public TraceRecord(TraceMessageType messageType, string methodName, string message)
        {
            _MessageType = messageType;
            _MethodName = methodName;
            _SoapMessage = message;
            _MessageDT = DateTime.Now;
        }

        public string MethodName
        {
            get { return _MethodName; }
            set { _MethodName = value; }
        }
        public TraceMessageType MessageType
        {
            get { return _MessageType; }
            set { _MessageType = value; }
        }
        public DateTime MessageDT
        {
            get { return _MessageDT; }
            set { _MessageDT = value; }
        }
        public string SoapMessage
        {
            get { return _SoapMessage; }
            set { _SoapMessage = value; }
        }
    
    }
    /* IMPORTANT!!!!
     Please not, if DirectorWSA WebReference was updated (due to interface changes), following line shoild be added
     to major method declarations in Reference.cs file.
        [Adlib.Director.DirectorWSAWrapper.TraceHelper.AdlibTraceSoapExtension]
      Sample:
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://adlibsoftware.com/BeginJobTransaction", RequestNamespace="http://adlibsoftware.com/", ResponseNamespace="http://adlibsoftware.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [Adlib.Director.DirectorWSAWrapper.TraceHelper.AdlibTraceSoapExtension]
        public BeginJobTransactionResponse BeginJobTransaction(System.Guid adminScopeId, JobKey jobKey, ProcessRulesRequest rulesRequest, Metadata[] metadata)
    
     */
    public class AdlibTraceSoapExtension : SoapExtension
    {
        Stream oldStream;
        Stream newStream;

        public override Stream ChainStream(Stream stream)
        {
            oldStream = stream;
            newStream = new MemoryStream();
            return newStream;
        }

        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return null;
        }

        public override object GetInitializer(Type WebServiceType)
        {
            return null;
        }

        public override void Initialize(object initializer)
        {
        }

        public override void ProcessMessage(SoapMessage message)
        {
            switch (message.Stage)
            {
                case SoapMessageStage.BeforeSerialize:
                    break;
                case SoapMessageStage.AfterSerialize:
                   TraceOutput(message);
                    break;
                case SoapMessageStage.BeforeDeserialize:
                    TraceInput(message);
                    break;
                case SoapMessageStage.AfterDeserialize:
                    break;
                default:
                    throw new Exception("invalid stage");
            }
        }

        public void TraceOutput(SoapMessage message)
        {
            newStream.Position = 0;

            TraceMessageType soapType = (message is SoapServerMessage) ? TraceMessageType.SoapResponse : TraceMessageType.SoapRequest;
            string soapMessage = GetText(newStream);
            newStream.Position = 0;
            CopyStream(newStream, oldStream);
            TraceRecord traceRecord = new TraceRecord(soapType, message.MethodInfo.Name, soapMessage);
            try
            {
                TraceLogger.GetInstance().AddTraceRecord(traceRecord);
            }
            catch 
            {

            }

        }

        public void TraceInput(SoapMessage message)
        {
            CopyStream(oldStream, newStream);

            TraceMessageType soapType = (message is SoapServerMessage) ? TraceMessageType.SoapRequest : TraceMessageType.SoapResponse;
            newStream.Position = 0;
            string soapMessage = GetText(newStream);
            TraceRecord traceRecord = new TraceRecord(soapType, message.MethodInfo.Name, soapMessage);
            try
            {
                TraceLogger.GetInstance().AddTraceRecord(traceRecord);
            }
            catch 
            {

            }
            newStream.Position = 0;
          }

        void CopyStream(Stream from, Stream to)
        {
            TextReader reader = new StreamReader(from);
            TextWriter writer = new StreamWriter(to);
            writer.WriteLine(reader.ReadToEnd());
            writer.Flush();
        }
        string GetText(Stream from)
        {
            string resultString = "";
            TextReader reader = new StreamReader(from);
            resultString = reader.ReadToEnd();
            return resultString;
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class AdlibTraceSoapExtensionAttribute : SoapExtensionAttribute
    {
        private int priority;

        public override Type ExtensionType
        {
            get { return typeof(AdlibTraceSoapExtension); }
        }

        public override int Priority
        {
            get { return priority; }
            set { priority = value; }
        }
    }
}
