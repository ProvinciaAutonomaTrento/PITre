// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Chilkat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Segnatura
{
    public class XmlParserManager
    {
        

        public static string BeautifyXml(string xmlIn)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlIn);
            // Save the document to a file and auto-indent the output.
            using (MemoryStream ms = new MemoryStream())
            {
                using (XmlTextWriter writer = new XmlTextWriter(ms, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    doc.Save(writer);

                    using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
                    {
                        ms.Position = 0;
                        return sr.ReadToEnd();
                    }
                }
            }
        }


    }

    public class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding encoding;

        public StringWriterWithEncoding() { }

        public StringWriterWithEncoding(Encoding encoding)
        {
            this.encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return encoding; }
        }

        public static string ToXmlString<T>(T value)
        {
            System.Xml.Serialization.XmlSerializerNamespaces namespaces = null;

            var settings = new XmlWriterSettings();
            settings.CheckCharacters = false;
            settings.Indent = true;
            using (var stream = new StringWriterWithEncoding(Encoding.UTF8))
            using (var writer = XmlWriter.Create(stream, settings))
            {
                var serializer = new XmlSerializer(value.GetType());
                serializer.Serialize(writer, value, namespaces);
                return stream.ToString();
            }

        }
    }
}
