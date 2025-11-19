// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Annullamento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox
{
    class Validator
    {
        public List<string> ValidationErrorList = new List<string>();

        public Validator(string directory, string file)
        {
            LocalDtdResolver resolver = new LocalDtdResolver();
            // Set the validation settings.
            XmlReaderSettings settings = new XmlReaderSettings();
            //settings.DtdProcessing = DtdProcessing.Parse;  //4.0
            settings.ProhibitDtd = false;
            settings.ValidationType = ValidationType.DTD;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
            settings.XmlResolver = resolver;

            // Create the XmlReader object.
            string path = Path.Combine(directory, file);
            XmlReader reader = XmlReader.Create(path, settings);

            // Parse the file. 
            while (reader.Read()) ;
        }

        public Validator(string xmlString)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            //settings.ProhibitDtd = false;
            XmlReader xreader = XmlTextReader.Create(new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(xmlString)), settings);
            XmlDocument doc = new XmlDocument();
            doc.Load(xreader);

            //Se è presente il namespase valido con Xsd, altrimenti con DTD
            if (string.IsNullOrEmpty(doc.DocumentElement.NamespaceURI) || doc.DocumentElement.NamespaceURI.Contains(Resources.www_digitPa_gov_it_protocollo))
            {
                ValidationErrorList.Add(ErrorDescriptions.SegnaturaNonValida);
            }
            else
            {
                try
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
                        using (MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(XsdResources.xmldsig_core_schema)))
                        using (var reader = XmlReader.Create(ms, new XmlReaderSettings()
                        {
                            DtdProcessing = DtdProcessing.Ignore // important
                        }))
                            schemas.Add(@"http://www.w3.org/2000/09/xmldsig#", reader);
                        
                        using (MemoryStream ms = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(XsdResources.Segnatura)))
                        using (var reader = XmlReader.Create(ms, new XmlReaderSettings()
                        {
                            DtdProcessing = DtdProcessing.Ignore // important
                        }))
                            schemas.Add(null, reader);

                        xdoc.Validate(schemas, ValidationCallBack);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

        }

        public Validator(string xmlString, string path, string filename)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            settings.ProhibitDtd = false;
            XmlReader xreader = XmlTextReader.Create(new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(xmlString)), settings);
            XmlDocument doc = new XmlDocument();
            doc.Load(xreader);

            //Se è presente il namespase valido con Xsd, altrimenti con DTD
            if (string.IsNullOrEmpty(doc.DocumentElement.NamespaceURI) || doc.DocumentElement.NamespaceURI.Contains(Resources.www_digitPa_gov_it_protocollo))
            {
                bool isSignatureValid = true;//interoperabilita.InteroperabilitaEccezioni.isSignatureValid(System.IO.Path.Combine(path, filename));
                if (!isSignatureValid)
                    ValidationErrorList.Add("Ricevuta non valida");
            }
            else
            {
                StringReader sr = new StringReader(xmlString);
                XmlTextReader xtr = new XmlTextReader(sr);

                XmlSchemaCollection myschema = new XmlSchemaCollection();
                //Incapsulo XML reader dentro Validiting reader
                XmlValidatingReader vReader = new XmlValidatingReader(xtr);
                vReader.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                String filenameXsd = string.Empty;
                filenameXsd = AppDomain.CurrentDomain.BaseDirectory + @"xml\segnaturaProtocollo2021\pec_message.xsd";
                myschema.Add(null, filenameXsd);
                string xmldsigXsd = AppDomain.CurrentDomain.BaseDirectory + @"xml\segnaturaProtocollo2021\xmldsig-core-schema.xsd";
                myschema.Add("http://www.w3.org/2000/09/xmldsig#", xmldsigXsd);
                //xml.Schemas.Add("http://www.w3.org/2000/09/xmldsig#", XmlReader.Create(XmldsigStream));
                vReader.ValidationType = ValidationType.Schema;
                vReader.Schemas.Add(myschema);

                //Leggo il file
                //Se ci fossero errori viene chiamato l'handler
                while (vReader.Read()) ;
                xtr.Close();
            }

        }

        // Display any validation errors.
        private void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            //Console.WriteLine("Validation Error: {0}", e.Message);
            if (e.Severity == XmlSeverityType.Error)
                ValidationErrorList.Add(e.Message);
        }

    }
}
