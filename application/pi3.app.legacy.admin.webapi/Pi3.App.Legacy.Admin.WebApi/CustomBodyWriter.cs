// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System.ServiceModel.Channels;
using System.Xml;

namespace Pi3.App.Legacy.Admin.WebApi
{
    public class CustomBodyWriter : BodyWriter
    {
        private readonly string messageContent;
        private readonly Dictionary<string, string> namespaces;

        public CustomBodyWriter(string messageContent, Dictionary<string, string> namespaces) : base(isBuffered: true)
        {
            //this.messageContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + messageContent;
            this.messageContent = messageContent;
            this.namespaces = namespaces;
        }


        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            // Can't Dispose this xmlWriter, 'cause it will close 'writer'.
            var xmlWriter = XmlWriter.Create(writer);

            //writer.Settings.OmitXmlDeclaration = false;
            xmlWriter.WriteStartDocument();
            // Applica i namespace al contenuto
            foreach (var ns in namespaces)
            {
                xmlWriter.WriteAttributeString(ns.Key.StartsWith("xmlns") ? ns.Key : "xmlns:" + ns.Key, ns.Value);
            }
            xmlWriter.WriteRaw(messageContent);
        }

        protected virtual IAsyncResult OnBeginWriteBodyContents(XmlDictionaryWriter writer, AsyncCallback callback, object state)
        {
            if (writer.WriteState == WriteState.Start)
                writer.WriteStartDocument();
            return Task.CompletedTask;
        }

    }
}
