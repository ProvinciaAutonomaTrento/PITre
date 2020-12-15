using System;
using System.ServiceModel.Dispatcher;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.ServiceModel.Channels;
using System.IO;

namespace Adlib.Director.DirectorWSAWrapper
{
    public class JobManagementServiceClientMessageInspector : IClientMessageInspector
    {
        public interface IMessageSink
        {
            bool WriteMessage(string message);
        }

        private IMessageSink messageSink;
        public IMessageSink MessageSink
        {
            get
            {
                lock (_syncObject)
                {
                    return messageSink;
                }
            }
            set
            {
                lock (_syncObject)
                {
                    messageSink = value;
                }
            }
        }

        private static readonly object _syncObject = new object();

        public JobManagementServiceClientMessageInspector()
        {
        }

        ~JobManagementServiceClientMessageInspector()
        {
        }



        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            lock (_syncObject)
            {
                if (MessageSink != null)
                {
                    // so bizarre...but if you don't, you'll ruin comms between the service and client
                    MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
                    reply = buffer.CreateMessage();
                    // so bizarre...but if you don't, you'll ruin comms between the service and client

                    Message msg = buffer.CreateMessage();
                    StringBuilder sb = new StringBuilder();
                    XmlWriter xw = XmlWriter.Create(sb);
                    msg.WriteBody(xw);
                    xw.Close();

                    MessageSink.WriteMessage(String.Format("Received ({0}):", DateTime.Now.ToUniversalTime().ToString()));
                    MessageSink.WriteMessage(msg.ToString());
                    MessageSink.WriteMessage("Body:");
                    MessageSink.WriteMessage(sb.ToString());
                    buffer.Close();
                }
            }
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            lock (_syncObject)
            {
                if (MessageSink != null)
                {
                    // so bizarre...but if you don't, you'll ruin comms between the service and client
                    MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
                    request = buffer.CreateMessage();
                    // so bizarre...but if you don't, you'll ruin comms between the service and client

                    MessageSink.WriteMessage(String.Format("Sending ({0}):", DateTime.Now.ToUniversalTime().ToString()));
                    MessageSink.WriteMessage(buffer.CreateMessage().ToString());
                    buffer.Close();
                }
            }

            return null;
        }

    }
}
