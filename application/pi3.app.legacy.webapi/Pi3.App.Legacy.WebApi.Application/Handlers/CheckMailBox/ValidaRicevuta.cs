// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Linq;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox
{
    public class ValidaRicevuta
    {
        public List<string> ValidationErrorList = new List<string>();

        public ValidaRicevuta(string xmlString, byte[] xmlContent, out string errorMessage)
        {
            //Contiene eventuali errori di IsSignatureValid
            errorMessage = string.Empty;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            settings.ProhibitDtd = false;
            XmlReader xreader = XmlTextReader.Create(new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(xmlString)), settings);
            XmlDocument doc = new XmlDocument();
            doc.Load(xreader);

            //Se è presente il namespase valido con Xsd, altrimenti con DTD
            if (string.IsNullOrEmpty(doc.DocumentElement.NamespaceURI) || doc.DocumentElement.NamespaceURI.Contains(Resources.www_digitPa_gov_it_protocollo))
            {
                //Validazione Vecchia Normativa (XSD o DTD)
                bool isSignatureValid = IsSignatureValid(xmlContent, xmlString, out errorMessage);
                if (!isSignatureValid)
                    ValidationErrorList.Add("Ricevuta non valida");
            }
            else
            {
                //Validazione Nuova normativa 2021
                XDocument xdoc = null;
                var settings2 = new XmlReaderSettings();
                settings2.DtdProcessing = DtdProcessing.Ignore;
                settings2.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                StringReader sr2 = new StringReader(xmlString);

                using (XmlReader xr = XmlReader.Create(sr2, settings2))
                {
                    xdoc = XDocument.Load(xr);
                    var schemas = new XmlSchemaSet();


                    using (MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(CheckMailBoxResources.pec_message)))
                    using (var reader = XmlReader.Create(ms, new XmlReaderSettings()
                    {
                        DtdProcessing = DtdProcessing.Ignore // important
                    }))
                    {
                        schemas.Add(null, reader);
                    }

                    using (MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(CheckMailBoxResources.Segnatura)))
                    using (var reader = XmlReader.Create(ms, new XmlReaderSettings()
                    {
                        DtdProcessing = DtdProcessing.Ignore // important
                    }))
                    {
                        schemas.Add(null, reader);
                    }

                    using (MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(CheckMailBoxResources.xmldsig_core_schema)))
                    using (var reader = XmlReader.Create(ms, new XmlReaderSettings()
                    {
                        DtdProcessing = DtdProcessing.Ignore // important
                    }))
                    {
                        schemas.Add(@"http://www.w3.org/2000/09/xmldsig#", reader);
                    }

                    xdoc.Validate(schemas, ValidationCallBack);
                }
            }

        }


        public bool IsSignatureValid(byte[] xmlContent, string xmlString, out string errMessage)
        {
            bool result = true;
            errMessage = string.Empty;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            settings.ProhibitDtd = false;

            MemoryStream xms = new MemoryStream(xmlContent);
            XmlReader xreader = XmlTextReader.Create(xms, settings);

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(xreader);

                //Se è presente il namespase valido con Xsd, altrimenti con DTD
                if (string.IsNullOrEmpty(doc.DocumentElement.NamespaceURI))
                {
                    XDocument xdoc = null;
                    var settings2 = new XmlReaderSettings();
                    settings2.DtdProcessing = DtdProcessing.Parse;
                    settings2.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                    StringReader sr2 = new StringReader(xmlString);

                    using (XmlReader xr = XmlReader.Create(sr2, settings2))
                    {
                        xdoc = XDocument.Load(xr);
                        var schemas = new XmlSchemaSet();
                        using (MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(CheckMailBoxResources.SegnaturaDTD)))
                        using (var reader = XmlReader.Create(ms, new XmlReaderSettings()
                        {
                            DtdProcessing = DtdProcessing.Parse // important
                        }))
                        {
                            schemas.Add(null, reader);
                        }


                        xdoc.Validate(schemas, ValidationCallBack);
                    }
                }
                else
                {
                    XDocument xdoc = null;
                    var settings2 = new XmlReaderSettings();
                    settings2.DtdProcessing = DtdProcessing.Ignore;
                    settings2.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                    StringReader sr2 = new StringReader(xmlString);

                    using (XmlReader xr = XmlReader.Create(sr2, settings2))
                    {
                        xdoc = XDocument.Load(xr);
                        var schemas = new XmlSchemaSet();
                        using (MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(CheckMailBoxResources.Segnatura_OLD_XSD)))
                        using (var reader = XmlReader.Create(ms, new XmlReaderSettings()
                        {
                            DtdProcessing = DtdProcessing.Ignore // important
                        }))
                        {
                            schemas.Add(null, reader);
                        }


                        xdoc.Validate(schemas, ValidationCallBack);
                    }

                }
            }
            catch (Exception ex)
            {
                // Costruzione in out del messaggio di errore
                errMessage = string.Format("Non valido {0}", ex.Message);
                result = false;
            }
            finally
            {
                xreader.Close();
            }

            return result;
        }

        private void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error)
                ValidationErrorList.Add(e.Message);
        }

    }
}
