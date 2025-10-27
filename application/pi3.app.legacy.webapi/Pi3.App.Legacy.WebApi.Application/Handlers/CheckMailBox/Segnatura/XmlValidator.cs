// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Segnatura
{
    public class XmlValidator
    {
        // salvo lo stato della validazione
        private bool _isValid = true;
        private StringBuilder _stringBuilder = new StringBuilder();

        //Propretà per fornire al chiamante il risultato della validazione
        public string ValidationErrors
        {
            get
            {
                return _stringBuilder.ToString();
            }
        }


        public bool Validate(String xmlString, Byte[] xsdFileContents, string targetNameSpace)
        {
            MemoryStream ms = new MemoryStream(xsdFileContents);
            XmlReader xr = XmlReader.Create(ms);
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);
            document.Schemas.Add(targetNameSpace, xr);
            try
            {
                document.Validate(null);
                return true;
            }
            catch (XmlSchemaValidationException)
            {
                return false;
            }
        }


        public bool ValidateXmlString(string xmlString, string xsdPath, string targetNameSpace)
        {
            XmlTextReader reader = null;
            XmlValidatingReader vReader = null;
            XmlSchemaCollection myschema = new XmlSchemaCollection();
            try
            {
                //Creo XML reader
                StringReader sr = new StringReader(xmlString);
                reader = new XmlTextReader(sr);
                //Incapsulo XML reader dentro Validiting reader
                vReader = new XmlValidatingReader(reader);
                vReader.ValidationEventHandler
                    += new ValidationEventHandler(vReader_ValidationEventHandler);

                myschema.Add(targetNameSpace, xsdPath);

                vReader.ValidationType = ValidationType.Schema;
                vReader.Schemas.Add(myschema);

                //Leggo il file
                //Se ci fossero errori viene chiamato l'handler
                while (vReader.Read())
                { }
            }
            catch (Exception ex)
            {
                _isValid = false;
            }
            finally
            {
                if (reader.ReadState != ReadState.Closed)
                    reader.Close();
                if (vReader.ReadState != ReadState.Closed)
                    vReader.Close();
            }

            return _isValid;
        }


        // Metodo che valida
        public bool Validate(string xmlPath, string xsdPath, string targetNameSpace)
        {
            XmlTextReader reader = null;
            XmlValidatingReader vReader = null;
            XmlSchemaCollection myschema = new XmlSchemaCollection();
            try
            {
                //Creo XML reader
                reader = new XmlTextReader(xmlPath);
                //Incapsulo XML reader dentro Validiting reader
                vReader = new XmlValidatingReader(reader);
                vReader.ValidationEventHandler
                    += new ValidationEventHandler(vReader_ValidationEventHandler);

                myschema.Add(targetNameSpace, xsdPath);

                vReader.ValidationType = ValidationType.Schema;
                vReader.Schemas.Add(myschema);

                //Leggo il file
                //Se ci fossero errori viene chiamato l'handler
                while (vReader.Read())
                { }
            }
            catch (Exception ex)
            {
                _isValid = false;
            }
            finally
            {
                if (reader.ReadState != ReadState.Closed)
                    reader.Close();
                if (vReader.ReadState != ReadState.Closed)
                    vReader.Close();
            }

            return _isValid;
        }


        //Event Handler per cattuare tutti gli errori di validazione
        private void vReader_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            _isValid = false;
            _stringBuilder.AppendFormat("\r\nValidation Error: {0}", e.Message);

        }

    }
}
