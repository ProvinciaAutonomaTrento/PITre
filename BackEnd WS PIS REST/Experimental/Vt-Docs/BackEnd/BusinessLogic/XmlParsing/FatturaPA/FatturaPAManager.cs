using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace BusinessLogic.XmlParsing.FatturaPA
{
    public class FatturaPAManager
    {
        public class allegati
        {
            public string nomeAttachment;
            public string formatoAttachment;
            public string descrizioneAttachment;
            public byte[] contenutoAttachment;
        }

        public bool isFatturaPA(byte[] xmlByteArray)
        {
            System.Xml.XmlDocument xd = new System.Xml.XmlDocument();
            System.IO.TextReader tr = new System.IO.StreamReader(new System.IO.MemoryStream(xmlByteArray));
            string fattura = tr.ReadToEnd();
            fattura = fattura.Replace("<?xml version=\"1.1", "<?xml version=\"1.0"); //FIX per l'xml 1.1 (che non viene processato da dotnet)
            try
            {
                xd.LoadXml(fattura);
                //controllo se il namespace è tipo fattura elettronica 
                if (xd.DocumentElement.NamespaceURI.ToLower().Equals("http://www.fatturapa.gov.it/sdi/fatturapa/v1.0"))
                    return true;

                if (xd.DocumentElement.NamespaceURI.ToLower().Equals("http://www.fatturapa.gov.it/sdi/fatturapa/v1.1"))
                    return true;

                if (xd.DocumentElement.NamespaceURI.ToLower().Equals("http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2"))
                    return true;
            }
            catch
            {
                return false;
            }
            return false;
        }


        public allegati[] getAllegatiFromFatturaPA(byte[] xmlByteArray)
        {
            List<allegati> retval = null; ;
            System.Xml.XmlDocument xd = new System.Xml.XmlDocument();
            System.IO.TextReader tr = new System.IO.StreamReader(new System.IO.MemoryStream(xmlByteArray));
            string fattura = tr.ReadToEnd();
            fattura = fattura.Replace("<?xml version=\"1.1", "<?xml version=\"1.0"); //FIX per l'xml 1.1 (che non viene processato da dotnet)
            try
            {
                xd.LoadXml(fattura);
                //controllo se il namespace è tipo fattura elettronica 
                XmlNodeList xnList = xd.GetElementsByTagName("Allegati");
                if (xnList.Count > 0)
                    retval = new List<allegati>();
                else
                    return null;

                foreach (XmlNode n in xnList)
                {
                    allegati al = new allegati();
                    foreach (XmlNode nd in n.ChildNodes)
                    {
                        switch (nd.Name)
                        {
                            case "NomeAttachment":
                                al.nomeAttachment = nd.InnerText;
                                break;
                            case "FormatoAttachment":
                                al.formatoAttachment = nd.InnerText;
                                break;
                            case "Attachment":
                                al.contenutoAttachment = Convert.FromBase64String(nd.InnerText);
                                break;
                            case "DescrizioneAttachment":
                                al.descrizioneAttachment = nd.InnerText;
                                break;

                        }

                    }
                    retval.Add(al);
                }
            }
            catch
            {
                return null;
            }

            return retval.ToArray();
        }

    }
}
