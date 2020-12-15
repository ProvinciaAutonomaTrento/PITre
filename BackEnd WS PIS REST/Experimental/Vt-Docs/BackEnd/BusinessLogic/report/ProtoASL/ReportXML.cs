using System;
using System.Data;
using System.Xml;  
using System.Text;
using System.Xml.Schema;
using System.Reflection;
using System.IO;
using log4net;

namespace BusinessLogic.report.ProtoASL
{
	/// <summary>
	/// Summary description for ReportXML.
	/// </summary>
	public class ReportXML
	{
        private static ILog logger = LogManager.GetLogger(typeof(ReportXML));

		public ReportXML()
			{
				//
				// TODO: Add constructor logic here
				//
			}


        public string BuildXMLReport(string xsdPath)
        {
            DocsPaDB.DBProvider db = null;
            string result = String.Empty;
            string ProtoASLPathFolder = String.Empty;
            DateTime date = DateTime.UtcNow;
            XmlDocument xmlout = new XmlDocument();
            db = new DocsPaDB.DBProvider();
            DataSet dsout = new DataSet();
            bool esito = false;

            logger.Debug("BuildXMLReport : Chiamata webService \"DocsPaWSProtoASL\" il GIORNO: " + System.DateTime.Now.ToLongDateString() + " alle ORE: " + System.DateTime.Now.ToLongTimeString());

            ProtoASLPathFolder = System.Configuration.ConfigurationManager.AppSettings["ProtoASLPathFolder"];
            if (ProtoASLPathFolder.Equals(""))
                logger.Debug("BuildXMLReport : ProtoASLPathFolder non inserito. (verificare web.config - backend) return \"-1\"");

            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

            if (ProtoASLPathFolder != null && ProtoASLPathFolder != "")
            {
                string filename = date.Day + "_" + date.Month + "_" + date.Year + "-" + date.Hour + "_" + date.Minute + "_" + date.Second + "_ProtoAsl.xml";
                string fullFileName = ProtoASLPathFolder + filename;
                SimpleLog sl = new SimpleLog(fullFileName);
                if (sl == null)
                {
                    logger.Debug("BuildXMLReport : Errore nella creazione dell'istanza del file di log - return \"-1\"");
                    return "-1";
                }

                try
                {
                    sl.Log("Inizializzazione processo di generazione XML");
                    //estraggo i dati //QUERY :S_PROTOASL_GET_DATA
                    DocsPaUtils.Query queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("S_PROTOASL_GET_DATA");
                    if (queryMng == null)
                    {
                        logger.Debug("BuildXMLReport : Errore nella creazione dell'istanza del queryManager DocsPa - return \"-1\"");
                        return "-1";
                    }

                    string commandText = queryMng.getSQL();
                    sl.Log("Query ORACLE estrazione dati: (Descrizione query in Log DocsPA) ");
                    logger.Debug("ORA: - BuildXMLReport  - QUERY : " + commandText);
                    // store del tempo di lancio
                    date = DateTime.UtcNow;
                    sl.Log("Tempo di lancio da memorizzare in storico : " + date.ToString());


                    db.ExecuteQuery(dsout, commandText);


                    //aggiungo header standard per xml
                    XmlDeclaration xmlDeclaration = xmlout.CreateXmlDeclaration("1.0", "UTF-8", null);
                    xmlout.InsertBefore(xmlDeclaration, xmlout.DocumentElement);
                    //creo XML
                    if (dsout.Tables[0].Rows.Count > 0)
                    {
                        XmlNode rootNode = CreateStructure(xmlout, dsout.Tables[0]);
                        xmlout.AppendChild(rootNode);
                        xmlout.Save(fullFileName);
                        //validazione XML 
                        XmlValidator xmlv = new XmlValidator();
                        esito = xmlv.Validate(fullFileName, xsdPath, null);

                        if (!esito)
                        {

                            System.IO.File.Delete(fullFileName);
                            sl.Log("Esito validazione file XML generato(Errore in Log DocsPA): " + esito);
                            sl.Log("Eliminazione File XML Generato non validato");
                            logger.Debug("BuildXMLReport : Esito validazione file XML generato(Errore in Log DocsPA): " + esito);
                            logger.Debug("BuildXMLReport : Eliminazione File XML Generato non validato - return \"-1\"");

                            return result = "-1";

                        }
                        else
                        {
                            sl.Log("Esito validazione file XML generato: " + esito);
                            logger.Debug("BuildXMLReport : Esito validazione file XML generato : " + esito);

                            //aggiorno la tabella di storico
                            sl.Log("Storicizzazione dati elaborati");
                            logger.Debug("BuildXMLReport : Storicizzazione dati elaborati");

                            string lastSysId = dsout.Tables[0].Rows[dsout.Tables[0].Rows.Count - 1]["ID_PROFILE"].ToString();
                            sl.Log("Xml contiene n° " + dsout.Tables[0].Rows.Count.ToString() + " occorrenze");
                            logger.Debug("BuildXMLReport : Xml contiene n° " + dsout.Tables[0].Rows.Count.ToString() + " occorrenze");

                            //aggiorno i dati //QUERY :I_PROTOASL_STORICIZZA_DATI
                            queryMng = DocsPaUtils.InitQuery.getInstance().getQuery("I_PROTOASL_STORICIZZA_DATI");
                            queryMng.setParam("lastSysId", lastSysId);
                            queryMng.setParam("date", date.ToString());
                            queryMng.setParam("format", "dd/mm/yyyy:hh24:mi:ss");
                            commandText = queryMng.getSQL();
                            sl.Log("Query ORACLE storicizzazione dati:  (Descrizione query in Log DocsPA)");
                            logger.Debug("ORA: - BuildXMLReport  - QUERY : " + commandText);
                            esito = db.ExecuteNonQuery(commandText);
                            sl.Log("Esito storicizzazione: " + esito);
                            logger.Debug("BuildXMLReport : Esito storicizzazione: " + esito);

                            result = "1";

                        }
                    }
                    else
                    {
                        sl.Log("ATTENZIONE: non ci sono dati da produrre");
                        result = "0";
                        logger.Debug("BuildXMLReport  : ATTENZIONE: non ci sono dati da produrre - return \"0\"");
                    }
                }

                catch (Exception ex)
                {
                    sl.Log(ex.Message, "ERRORE", ex.StackTrace);
                    logger.Debug("BuildXMLReport  : ERRORE - MESSAGE : " + ex.Message + " STACKTRACE : " + ex.StackTrace);
                    result = "-1";
                }
                finally
                {
                    if (db != null)
                        db.Dispose();
                    sl.Log("Termine processo di generazione XML");
                    logger.Debug("BuildXMLReport  : Termine chiamata webService \"DocsPaWSProtoASL\" il GIORNO: " + System.DateTime.Now.ToLongDateString() + " alle ORE: " + System.DateTime.Now.ToLongTimeString());
                }
            }
            return result;
        }



		#region Validazione XML generato

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
                    logger.Debug("ERRORE della validazione XML: " + _stringBuilder.ToString());
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
                    logger.Debug("ERRORE della validazione XML: " + _stringBuilder.ToString());
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
                logger.Debug("ERRORE della validazione XML: " + _stringBuilder.ToString());

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="v"></param>
            /// <returns></returns>
            public bool xmlValidatorFromDTD(XmlDocument document)
            {
                bool retValue = false;

                using (MemoryStream stream = new MemoryStream())
                {
                    // Save del contenuto xml in un oggetto MemoryStream
                    document.Save(stream);
                    stream.Position = 0;

                    //creazione del doc con trattamento spazi bianchi
                    BusinessLogic.Interoperabilità.InteropResolver my = new BusinessLogic.Interoperabilità.InteropResolver();

                    XmlTextReader xtr = new XmlTextReader(stream);
                    xtr.WhitespaceHandling = WhitespaceHandling.None;

                    XmlValidatingReader xvr = new XmlValidatingReader(xtr);
                    xvr.ValidationType = System.Xml.ValidationType.DTD;
                    xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
                    xvr.XmlResolver = my;

                    XmlDocument doc = new XmlDocument();

                    try
                    {
                        doc.Load(xvr);

                        retValue = true;
                    }
                    catch (System.Xml.Schema.XmlSchemaException e)
                    {
                        logger.Debug("Errore nella validazione del file segnatura.XML: Eccezione:" + e.Message);

                        retValue = false;
                    }
                    finally
                    {
                        xvr.Close();
                        xtr.Close();
                    }
                }

                return retValue;
            }

            public bool xmlValidatorFromDTD(string xmlPath)
            {
                //creazione del doc con trattamento spazi bianchi
                XmlDocument doc = new XmlDocument();
                BusinessLogic.Interoperabilità.InteropResolver my = new BusinessLogic.Interoperabilità.InteropResolver();
                XmlTextReader xtr = new XmlTextReader(xmlPath);
                xtr.WhitespaceHandling = WhitespaceHandling.None;
                XmlValidatingReader xvr = new XmlValidatingReader(xtr);
                xvr.ValidationType = System.Xml.ValidationType.DTD;
                xvr.EntityHandling = System.Xml.EntityHandling.ExpandCharEntities;
                xvr.XmlResolver = my;
                bool esito = true;
                try
                {
                    doc.Load(xvr);
                }
                catch (System.Xml.Schema.XmlSchemaException e)
                {
                    logger.Debug("Errore nella validazione del file segnatura.XML: Eccezione:" + e.Message);
                    esito = false;
                }
                finally
                {
                    xvr.Close();
                    xtr.Close();
                }
                return esito;
            }
        }

        #endregion

        #region generazione nodi XML

        private  XmlNode CreateNode(XmlDocument document,string nodeName,string nodeValue)
		{
			XmlNode node=document.CreateNode(XmlNodeType.Element,nodeName,string.Empty);
			if (nodeValue!=string.Empty)
				node.InnerText=nodeValue;
			return node;
		}


		private  XmlNode CreateStructure(XmlDocument document,DataTable table)
		{
			XmlNode node=CreateNode(document,"tipoProtocollo","");

			AppendNodiProtocollo(node,document,table);

			return node;
		}


		private  void AppendNodiProtocollo(XmlNode parentNode, XmlDocument document, DataTable table)
		{	
			//ciclo tutto il dataset
			for (int i=0; i< table.Rows.Count; i++)
			{
				XmlNode node=document.CreateNode(XmlNodeType.Element, "protocollo", "");

				AppendNodiTipoP(node,document,table.Rows[i]);

				parentNode.AppendChild(node);
			}
		}


		private  void AppendNodiTipoP(XmlNode parentNode, XmlDocument document,DataRow riga)
		{
			if (this.IsNullOrEmptyField(riga,"ID_PROFILE"))parentNode.AppendChild(CreateNode(document,"num",riga["ID_PROFILE"].ToString()));
			if (this.IsNullOrEmptyField(riga,"NUM_PROTO"))parentNode.AppendChild(CreateNode(document,"numeroProtocollo",riga["NUM_PROTO"].ToString()));
			if (this.IsNullOrEmptyField(riga,"OPERAZIONE"))parentNode.AppendChild(CreateNode(document,"operazione",riga["OPERAZIONE"].ToString().Trim()));
			if (this.IsNullOrEmptyField(riga,"VAR_PROTO_IN"))parentNode.AppendChild(CreateNode(document,"protocolloMittente",riga["VAR_PROTO_IN"].ToString().Trim()));
			if (this.IsNullOrEmptyField(riga,"SETTORE"))parentNode.AppendChild(CreateNode(document,"settore",riga["SETTORE"].ToString().Trim()));
			if (this.IsNullOrEmptyField(riga,"OPERATORE"))parentNode.AppendChild(CreateNode(document,"operatore",riga["OPERATORE"].ToString().Trim()));
			if (this.IsNullOrEmptyField(riga,"CHA_TIPO_PROTO"))parentNode.AppendChild(CreateNode(document,"arrivoPartenza",riga["CHA_TIPO_PROTO"].ToString()));
			if (this.IsNullOrEmptyField(riga,"DTA_PROTO"))parentNode.AppendChild(CreateNode(document,"dataProtocollo",NormalizeDatetime(riga["DTA_PROTO"].ToString())));
			if (this.IsNullOrEmptyField(riga,"CODFASC"))parentNode.AppendChild(CreateNode(document,"fascicolazione",riga["CODFASC"].ToString().Trim()));
			if (this.IsNullOrEmptyField(riga,"MITTENTE"))parentNode.AppendChild(CreateNode(document,"mittente",riga["MITTENTE"].ToString().Trim()));
			if (this.IsNullOrEmptyField(riga,"DTA_PROTO_IN"))parentNode.AppendChild(CreateNode(document,"dataMittente",NormalizeDatetime(riga["DTA_PROTO_IN"].ToString())));
			if (this.IsNullOrEmptyField(riga,"VAR_PROF_OGGETTO"))parentNode.AppendChild(CreateNode(document,"oggetto",riga["VAR_PROF_OGGETTO"].ToString().Trim()));
			if (this.IsNullOrEmptyField(riga,"VAR_NOTE"))parentNode.AppendChild(CreateNode(document,"note",riga["VAR_NOTE"].ToString().Trim()));
			if (this.IsNullOrEmptyField(riga,"DATATRASMISSIONE"))parentNode.AppendChild(CreateNode(document,"dataTrasmissione",NormalizeDatetime(riga["DATATRASMISSIONE"].ToString())));
			if (this.IsNullOrEmptyField(riga,"AMBITO"))parentNode.AppendChild(CreateNode(document,"ambito",riga["AMBITO"].ToString().Trim()));
		}

		#endregion

		#region utils
		private bool IsNullOrEmptyField(DataRow row,string fieldName)
		{
			return (row[fieldName]!=DBNull.Value && row[fieldName].ToString()!=string.Empty);
		}
		private string NormalizeDatetime(string date)
		{
			DateTime data = Convert.ToDateTime(date);

			return data.ToString("dd/MM/yyyy");

		}
		#endregion

		#region SimpleLog
		public class SimpleLog
		{

			private StreamWriter sw = null;
			string logFullFileName = String.Empty;

			public SimpleLog (string fullFileName)
			{
				try 
				{
					logFullFileName = fullFileName+".log";
					if (!File.Exists (logFullFileName))
					{
						/*Se il file non esiste dobbiamo crearlo*/
						sw = File.CreateText(logFullFileName);
                        logger.Debug("BuildXMLReport : file di log " + logFullFileName+" creato correttamente.");
					}
					else
					{
						sw = (StreamWriter)File.AppendText(logFullFileName);
                        logger.Debug("BuildXMLReport : file di log " + logFullFileName + " esistente, appendo informazioni.");					
					}
				}
				catch(Exception ex)
				{
					sw = null;
                    logger.Debug("BuildXMLReport : errore nella creazione del file di log " + logFullFileName + " MESSAGE : " + ex.Message + " STACKTRACE : " + ex.StackTrace);
                    throw ex;
				}
				finally
				{
					Close();
				}

			}

			public SimpleLog()
			{

			}

			public string Log (string exceptionType,string msg,string error, string function, string stackTrace)
			{
				if (sw != null) 
				{
					try 
					{
						sw = (StreamWriter)File.AppendText(logFullFileName);
						sw.WriteLine("");
						sw.WriteLine("-----------------------------------------------------------------------------------------------");
						sw.WriteLine(DateTime.UtcNow+" - Genera Proto ASL XML: ");
						sw.WriteLine("");
						sw.WriteLine("ECCEZIONE: "+exceptionType);
						sw.WriteLine("");
						sw.WriteLine("ERRORE: "+error);
						sw.WriteLine("");
						sw.WriteLine("FUNZIONE: "+function +": "+ msg);
						sw.WriteLine(stackTrace);
						sw.WriteLine("-----------------------------------------------------------------------------------------------");
						sw.WriteLine("");
						Close();
					}
					catch (Exception ex)
					{
                        logger.Debug("BuildXMLReport : errore nella scritura delle informazioni nel log MESSAGE : " + ex.Message + " STACKTRACE : " + ex.StackTrace);					
						throw ex;
					}
				}
				return logFullFileName;
			}

			public string Log (string exceptionType,string msg, string stackTrace)
			{
				if (sw != null) 
				{
					try 
					{
						sw = (StreamWriter)File.AppendText(logFullFileName);
						sw.WriteLine("");
						sw.WriteLine("-----------------------------------------------------------------------------------------------");
						sw.WriteLine(DateTime.UtcNow+" - Genera Proto ASL XML: ");
						sw.WriteLine("");
						sw.WriteLine("ECCEZIONE: "+exceptionType);
						sw.WriteLine("");
						sw.WriteLine(msg);
						sw.WriteLine("");
						sw.WriteLine(stackTrace);
						sw.WriteLine("-----------------------------------------------------------------------------------------------");
						sw.WriteLine("");

						Close();
					}
					catch (Exception ex)
					{
                        logger.Debug("BuildXMLReport : errore nella scritura delle informazioni nel log MESSAGE : " + ex.Message + " STACKTRACE : " + ex.StackTrace);					
						throw ex;
					}
				}
				return logFullFileName;
			}
			public string Log (string msg)
			{
				if (sw != null) 
				{
					try 
					{
						sw = (StreamWriter)File.AppendText(logFullFileName);
						sw.WriteLine("");
						sw.WriteLine("-----------------------------------------------------------------------------------------------");
						sw.WriteLine(DateTime.UtcNow +" - Genera Proto ASL XML: ");
						sw.WriteLine("");
						sw.WriteLine("MESSAGE: "+msg);
						sw.WriteLine("");
						sw.WriteLine("-----------------------------------------------------------------------------------------------");
						sw.WriteLine("");
						Close();
					}
					catch (Exception ex)
					{
                        logger.Debug("BuildXMLReport : errore nella scritura delle informazioni nel log MESSAGE : " + ex.Message + " STACKTRACE : " + ex.StackTrace);					
						throw ex;
					}
				}
				return logFullFileName;
			}

			public void Close()
			{
				if (sw != null) 
				{
					try 
					{
						sw.Flush();
						sw.Close();
					}
					catch(Exception ex) 
					{
                        logger.Debug("BuildXMLReport : errore nella close del file di log MESSAGE : " + ex.Message + " STACKTRACE : " + ex.StackTrace);
                        throw ex;
					}
				}
			}

		}
		#endregion

	}

	
}
