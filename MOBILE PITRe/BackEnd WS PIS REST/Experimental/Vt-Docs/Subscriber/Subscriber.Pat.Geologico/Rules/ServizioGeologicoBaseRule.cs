using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Subscriber.Pat.Geologico.Rules
{
    /// <summary>
    /// Regola per la pubblicazione dei fascicoli per il Servizio Geologico
    /// </summary>
    public abstract class ServizioGeologicoBaseRule : Subscriber.Rules.BaseRule
    {
        #region Public Members

        /// <summary>
        /// Nome della regola 
        /// </summary>
        public override string RuleName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Sottoregole gestite dalla regola
        /// </summary>
        /// <returns></returns>
        public override string[] GetSubRules()
        {
            return new string[0];
        }

        /// <summary>
        /// Reperimento nome del template dell'oggetto gestito dalla regola
        /// </summary>
        /// <returns></returns>
        protected virtual string GetTemplateName()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        private const string FASCICOLO_OBJECT_TYPE = "Fascicolo";

        /// <summary>
        /// Esecuzione della regola
        /// </summary>
        protected override void InternalExecute()
        {
            _logger.Info("BEGIN");

            if (!this.AreEquals(this.ListenerRequest.EventInfo.PublishedObject.TemplateName, this.GetTemplateName()))
            {
                this.Response.Rule.Error = new ErrorInfo
                {
                    Id = Subscriber.ErrorCodes.INVALID_TEMPLATE_PER_RULE,
                    Message = string.Format(Subscriber.ErrorDescriptions.INVALID_TEMPLATE_PER_RULE, this.ListenerRequest.EventInfo.PublishedObject.TemplateName)
                };
            }
            else
            {
                try
                {
                    if (this.IsRuleEnabled(this.Response.Rule))
                    {
                        this.Dispatch();

                        this.Response.Rule.Computed = true;
                        this.Response.Rule.ComputeDate = DateTime.Now;

                        // Scrittura elemento di pubblicazione nell'history
                        this.WriteRuleHistory(this.Response.Rule);
                    }
                }
                catch (SubscriberException pubEx)
                {
                    // Eccezione non gestita di tipo "SubscriberException"
                    _logger.Error(pubEx.Message);

                    this.Response.Rule.Computed = false;
                    this.Response.Rule.ComputeDate = DateTime.Now;

                    this.Response.Rule.Error = new ErrorInfo
                    {
                        Id = pubEx.ErrorCode,
                        Message = pubEx.Message,
                        Stack = pubEx.ToString()
                    };

                    // Scrittura errore non gestito per la SubRule
                    this.WriteRuleHistory(this.Response.Rule);

                }
                catch (Exception ex)
                {
                    // Eccezione non gestita
                    _logger.Error(ex.Message);

                    this.Response.Rule.Computed = false;
                    this.Response.Rule.ComputeDate = DateTime.Now;

                    this.Response.Rule.Error = new ErrorInfo
                    {
                        Id = Subscriber.ErrorCodes.UNHANDLED_ERROR,
                        Message = ex.Message,
                        Stack = ex.ToString()
                    };

                    // Scrittura errore non gestito per la SubRule
                    this.WriteRuleHistory(this.Response.Rule);
                }
            }

            _logger.Info("END");
        }

        /// <summary>
        /// Scrittura esito dell'esecuzione della regola
        /// </summary>
        /// <param name="rule"></param>
        protected virtual void WriteRuleHistory(BaseRuleInfo rule)
        {
            // Scrittura elemento di pubblicazione nell'history
            RuleHistoryInfo historyInfo = RuleHistoryInfo.CreateInstance(rule);
            historyInfo.Author = this.ListenerRequest.EventInfo.Author;
            historyInfo.ObjectSnapshot = this.ListenerRequest.EventInfo.PublishedObject;
            historyInfo = DataAccess.RuleHistoryDataAdapter.SaveHistoryItem(historyInfo);
        }

        /// <summary>
        /// Pubblicazione del documento XML su ftp
        /// </summary>
        protected virtual void Dispatch()
        {
            // Solo se la regola è abilitata
            XmlDocument document = this.ToXmlDocument();

            using (MemoryStream stream = new MemoryStream())
            {
                // Save del documento XML su MemoryStream
                document.Save(stream);
                stream.Flush();

                // Pubblicazione del documento XML su FTP
                using (Dispatcher.Ftp.FtpDispatcher ftpDispatcher = new Dispatcher.Ftp.FtpDispatcher(this.GetXmlFileName(), this.Response.Rule))
                {
                    ftpDispatcher.Dispatch(stream);
                }
            }
        }

        /// <summary>
        /// Restituisce il nome del file xml da copiare nella cartella FTP
        /// </summary>
        /// <returns></returns>
        protected virtual string GetXmlFileName()
        {
            //return string.Format("{0}.xml", this.ListenerRequest.EventInfo.PublishedObject.IdObject);
            string str = this.GetPropertyValueAsString("CodiceFascicolo");
            if (string.IsNullOrEmpty(str))
                str = this.ListenerRequest.EventInfo.PublishedObject.IdObject;
            return string.Format("{0}.xml", (object)str.Replace("|", "-").Replace(".", "-").Replace("\\", "-").Replace("_", "-").Replace("/", "-"));

        }

        /// <summary>
        /// Conversione del fasciolo in formato XML
        /// </summary>
        /// <returns></returns>
        protected virtual System.Xml.XmlDocument ToXmlDocument()
        {
            XmlDocument document = new XmlDocument();

            // Write down the XML declaration
            XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0","utf-8",null); 

            // Creazione elemento root
            XmlElement rootNode  = this.CreateElement(document, "Fascicolo");
            document.InsertBefore(xmlDeclaration, document.DocumentElement); 
            document.AppendChild(rootNode);

            // Creazione attributo "Id"
            rootNode.Attributes.Append(this.CreateAttribute(document, "id", this.ListenerRequest.EventInfo.PublishedObject.IdObject));

            // Creazione attributo "Tipo"
            rootNode.Attributes.Append(this.CreateAttribute(document, "tipo", this.ListenerRequest.EventInfo.PublishedObject.TemplateName));

            // Creazione attributo "Codice"
            rootNode.Attributes.Append(this.CreateAttribute(document, "codice", this.GetPropertyValueAsString("CodiceFascicolo")));
            
            // Creazione attributo "Descrizione"
            rootNode.Attributes.Append(this.CreateAttribute(document, "descrizione", this.ListenerRequest.EventInfo.PublishedObject.Description));

            // Creazione attributo "DataApertura"
            rootNode.Attributes.Append(this.CreateAttribute(document, "dataApertura", this.GetPropertyValueAsString("AperturaFascicolo")));

            // Creazione attributo "DataChiusura"
            rootNode.Attributes.Append(this.CreateAttribute(document, "dataChiusura", this.GetPropertyValueAsString("ChiusuraFascicolo")));

            // Creazione attributo "Note"
            rootNode.Attributes.Append(this.CreateAttribute(document, "note", this.GetPropertyValueAsString("NoteFascicolo")));

            // Creazione elemento "CoordinateGps"
            XmlElement coordinateGps = this.CreateElement(document, "CoordinateGps");
            coordinateGps.Attributes.Append(this.CreateAttribute(document, "x", this.GetPropertyValueAsString("x")));
            coordinateGps.Attributes.Append(this.CreateAttribute(document, "y", this.GetPropertyValueAsString("y")));
            rootNode.AppendChild(coordinateGps);

            // Creazione elemento "CampiProfilo"
            XmlElement campiProfilo = this.CreateElement(document, "CampiProfilo");

            foreach (Property p in this.ListenerRequest.EventInfo.PublishedObject.Properties)
            {
                if (!p.Hidden)
                {
                    XmlElement campoProfilo = this.CreateElement(document, "Campo");
                    
                    string value = string.Empty;

                    if (!p.IsEmpty) 
                        value = p.Value.ToString();
                        
                    campoProfilo.Attributes.Append(this.CreateAttribute(document, "nome", p.Name));
                    campoProfilo.Attributes.Append(this.CreateAttribute(document, "valore", value));
                    campiProfilo.AppendChild(campoProfilo);
                }
            }

            rootNode.AppendChild(campiProfilo);

            return document;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="asAttribute"></param>
        /// <returns></returns>
        private XmlElement CreateElement(XmlDocument document, string name)
        {
            return document.CreateElement(name);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private XmlAttribute CreateAttribute(XmlDocument document, string name, string value)
        {
            XmlAttribute attribute = document.CreateAttribute(name);
            attribute.Value = value;
            return attribute;
        }


        #endregion
    }
}
