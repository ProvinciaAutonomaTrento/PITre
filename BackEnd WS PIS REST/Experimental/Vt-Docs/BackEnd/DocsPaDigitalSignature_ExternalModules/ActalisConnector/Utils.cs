using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text.pdf;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ActalisConnector
{
    public class Utils
    {
        public enum SignFormat
        {
            UNKNOWN,
            CAdES,
            PAdES,
            XAdES
        }

        public static bool IsFileSigned(byte[] fileContent, out SignFormat signFormat)
        {
            // Il file è firmato se:
            // - E' un PDF con almeno una firma.
            // - E' un file di qualunque altro tipo

            // Esito della verifica
            bool fileHasSignatures = false;
            signFormat = SignFormat.UNKNOWN;

            // Se i primi quattro caratteri del file corrispondono a %PDF, molto probabilmente il file
            // è un PDF.
            if (fileContent[0] == '%' && fileContent[1] == 'P' && fileContent[2] == 'D' && fileContent[3] == 'F')
            {
                try
                {
                    PdfReader reader = new PdfReader(fileContent);
                    AcroFields acroFields = reader.AcroFields;
                    fileHasSignatures = acroFields.GetSignatureNames().Count > 0;
                    signFormat = SignFormat.PAdES;
                    return fileHasSignatures;
                }
                catch
                {
                    fileHasSignatures = false;
                }
            }
            else
            {
                //provo cades
                try
                {
                    // Altrimenti, probabilmente, il file potrebbe essere firmato in CAdES.
                    // In questo caso, lo si prova ad analizzare con l'X509Certificate.
                    X509Certificate certificate = new X509Certificate(fileContent);
                    fileHasSignatures = true;
                    signFormat = SignFormat.CAdES;
                    return fileHasSignatures;
                }
                catch
                {
                    fileHasSignatures = false;
                }

                // altrimenti xades
                try
                {
                    XmlDocument doc = new XmlDocument();
                    MemoryStream ms = new MemoryStream(fileContent);
                    doc.Load(ms);
                    XmlNodeList xnl = doc.GetElementsByTagName("ds:Signature");
                    //se non ci sono firme con ds:Signature (xades)  provo semplicemente Signature (xmldsig)
                    if (xnl.Count ==0)  xnl = doc.GetElementsByTagName("Signature");
                    
                    if (xnl.Count  ==1)
                    {
                        signFormat = SignFormat.XAdES;
                        fileHasSignatures = true;
                        return fileHasSignatures;
                    }
                }
                catch
                {
                    fileHasSignatures = false;
                }
            }
            return fileHasSignatures;
        }
        static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        public static String SerializeObject<t>(Object pObject)
        {
            try
            {
                String XmlizedString = null;
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(t));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                xs.Serialize(xmlTextWriter, pObject, ns);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
                return XmlizedString;
            }
            catch (Exception e) { System.Console.WriteLine(e); return null; }
        }
    }

}