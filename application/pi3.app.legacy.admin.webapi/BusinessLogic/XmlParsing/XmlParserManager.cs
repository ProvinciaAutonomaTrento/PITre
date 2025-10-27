// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Serilog;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BusinessLogic.XmlParsing
{
    public class XmlParserManager
    {
        private static ILogger logger = Log.ForContext(typeof(XmlParserManager));

#if false // importato tutto il file ma non usato
        public static void parseExtraXmlfiles(DocsPaVO.documento.SchedaDocumento schedaDoc, string fileName, byte[] filecontents, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            //controllo dei file.
            //descrittore SUAP
            if (fileName.ToLower().EndsWith("suap.xml"))
            {
                suap.SuapManager s = new suap.SuapManager("SUAPENTE");


                bool retval = s.ImportSuapEnteXMLIntoTemplate(infoUtente, ruolo, schedaDoc, filecontents);
                if (retval)
                    logger.DebugFormat("suap {0} processato correttamente", fileName);
                else
                    logger.DebugFormat("Errore: suap {0} NON processato", fileName);

            }
        }

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

#endif

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
