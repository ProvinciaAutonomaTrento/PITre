using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Xml;
using System.Xml.Schema;
using System.IO;

namespace BusinessLogic.XmlParsing
{
    public class XmlParserManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(XmlParserManager));
        public static void parseExtraXmlfiles (DocsPaVO.documento.SchedaDocumento schedaDoc, string fileName, byte [] filecontents, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
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

 

    }
}
