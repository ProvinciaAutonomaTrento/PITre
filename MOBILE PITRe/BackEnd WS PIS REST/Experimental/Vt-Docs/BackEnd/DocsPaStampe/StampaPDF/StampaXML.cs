using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Data;
//using Debugger = LogsManagement.Debugger;
using System.Collections;

namespace StampaPDF
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class StampaXML
	{
		protected static ErrorCode errorCode;
		protected static StampaVO.Document templatePDF;
		protected static XmlDocument xmlDoc;
		protected static XmlNode nodoReport;
		protected static bool m_bValid = true;

		

		public static StampaVO.Document CreateDocTemplate(FileStream  fileXML, string path)
		{
			StampaVO.Document templateDoc = null;
			try
			{
				xmlDoc = GetXmlDoc(fileXML,path);
				if (xmlDoc != null)
				{
					templateDoc = LoadXmlDoc(xmlDoc);
					templateDoc.pathName = path;
				}

			}
			catch(ReportException re)
			{
				throw re;
			}
			catch(Exception ex)
			{
				throw new ReportException(ErrorCode.InvalidXmlFile,"Errore nella creazione del template XML: "+ex.Message);
			}
			return templateDoc;

		}

 
		private static XmlDocument GetXmlDoc(FileStream fileXML, string path)
		{
			errorCode = ErrorCode.NoError;
			try
			{
				
				// Validazione file XML
				xmlDoc = new XmlDocument();
				
                // Modifica Sam: durante la creazione dei report mi sono accorto che path contiene
                // già il nome del file schema! Comunque finché non scopro se effettivamente l'istruzione
                // seguente è errata, l'ho commentata e c'ho messo quella che segue
                //string reportXSD = path + "XMLReport.xsd";
                string reportXSD;

                if (!path.EndsWith("XMLReport.xsd"))
                    reportXSD = path + "XMLReport.xsd";
                else
                    reportXSD = path;




				XmlTextReader xtr=new XmlTextReader(reportXSD, fileXML);

				xtr.WhitespaceHandling=WhitespaceHandling.None;
				XmlValidatingReader xvr=new XmlValidatingReader(xtr);
				xvr.ValidationType=System.Xml.ValidationType.Schema;
				xvr.EntityHandling=System.Xml.EntityHandling.ExpandCharEntities;
				xmlDoc.Load(xvr);
				//xmlDoc.Load(fileXML);
				return xmlDoc;
			}
			catch(Exception exc)
			{
				throw new ReportException(ErrorCode.InvalidXmlFile,"Errore nella convalida del del file XML: "+exc.Message);
			}
			//return null;
		}


		private static StampaVO.Document LoadXmlDoc(XmlDocument xmlDoc)
		{
			//carico xml
			StampaVO.Document templateDoc = null;
			nodoReport = xmlDoc.SelectSingleNode("report");
			if (nodoReport!=null && nodoReport.ChildNodes!=null && nodoReport.ChildNodes.Count>0)
			{
					templateDoc = new StampaVO.Document();
					//elemento pagina: style - header - footer :
					templateDoc.page = setPage();
					//elemento dati - testo - spazio - tabella
					templateDoc.dataToPrint = setDati();
			}
			return templateDoc;
		}

	
		private static StampaVO.Page setPage()
		{
			StampaVO.Page page = new StampaVO.Page();
			
			//PAGINA
			XmlNode nodoPagina = nodoReport.SelectSingleNode("pagina");
			if (nodoPagina!=null)
			{
				page.pageSize = Utils.getAttribute("formato",nodoPagina);
				if (Utils.getAttribute("orientamento",nodoPagina) != null && Utils.getAttribute("orientamento",nodoPagina).ToUpper().Equals("VERTICALE"))
					page.orientation = StampaVO.Page.VERTICALE;
				else 
					page.orientation = StampaVO.Page.ORIZZONTALE;
			}

			//HEADER
			XmlNode nodoHeader = nodoReport.SelectSingleNode("header");
			if (nodoHeader!=null)
			{
				page.headerPage = new StampaVO.Header();
				page.headerPage.align = Utils.getAttribute("align",nodoHeader);
				page.headerPage.border = Utils.getAttribute("border",nodoHeader);
				page.headerPage.bgcolor = Utils.getAttribute("bgcolor",nodoHeader);
                page.headerPage.text = nodoHeader.InnerText;
				page.headerPage.font = getFont(nodoHeader);
			}
			//FOOTER
			XmlNode nodoFooter = nodoReport.SelectSingleNode("footer");
			if (nodoFooter!=null)
			{
				page.footerPage = new StampaVO.Footer();
				page.footerPage.align = Utils.getAttribute("align",nodoFooter);
				page.footerPage.border = Utils.getAttribute("border",nodoFooter);
				page.footerPage.text = nodoFooter.InnerText;
				page.footerPage.font = getFont(nodoFooter);
			}
			//NUM PAGINE
			StampaVO.NumPagine numPagine = getNumPagine();
			page.numPagine = numPagine;
			if (numPagine != null)
			{
				if (numPagine.target.Equals("HEADER"))
					page.headerPage.numPagine = numPagine;
				else if (numPagine.target.Equals("FOOTER"))
					page.footerPage.numPagine = numPagine;
			}
			//DTA STAMPA
			StampaVO.DtaStampa dtaStampa = getDtaStampa();
			page.dtaStampa = dtaStampa;
			if (dtaStampa != null)
			{
				if (dtaStampa.target.ToUpper().Equals("HEADER"))
					page.headerPage.dtaStampa = dtaStampa;
				else
					page.footerPage.dtaStampa = dtaStampa;
			}

			//LOGO
			XmlNode nodoLogo = nodoReport.SelectSingleNode("logo");
			if (nodoLogo!=null)
			{
				page.logo = new StampaVO.Image();
				page.logo.align = Utils.getAttribute("align",nodoLogo);
				page.logo.border = Utils.getAttributeI("border",nodoLogo);
				page.logo.fileImage = Utils.getAttribute("file",nodoLogo);
			}


			return page;
		}

		private static ArrayList setDati()
		{
			ArrayList dati = new ArrayList();
			//XmlNode nodoDati = nodoReport.SelectSingleNode("dati");
			XmlNodeList nodoDati1 = nodoReport.SelectNodes("dati");

			foreach(XmlNode nodoDati in nodoDati1)
			//if (nodoDati!=null)
			{
				if (nodoDati.ChildNodes.Count>0)
				{							
					foreach (XmlNode nodo in nodoDati.ChildNodes)
					{
						if (nodo.Name.Equals("paragrafo"))
						{
							StampaVO.Paragraph par = getParagrafo(nodo);
							if (par != null)
								dati.Add(par);
						}
						else 
							if (nodo.Name.Equals("tabella"))
							{
								StampaVO.Table tab = getTabella(nodo);
								if (tab != null)
									dati.Add(tab);
							}
//							else if (nodo.Name.Equals("spazio"))
//									getSpazio(nodo);
					}
				}
			}

			return dati;
		}

		private static StampaVO.Font getFont(XmlNode nodo)
		{
			StampaVO.Font font = null;
			if (nodo!=null)
			{
				font = new StampaVO.Font();
				font.name = Utils.getAttribute("font",nodo);
				font.size = Utils.getAttributeF("size",nodo);
				font.style = Utils.getAttribute("style",nodo);
				font.color = Utils.getAttribute("color",nodo);
			}
			return font;
		}

		private static StampaVO.DtaStampa getDtaStampa()
		{
			StampaVO.DtaStampa dtaStampa = null;
			XmlNode nodoPagina = nodoReport.SelectSingleNode("pagina");
			XmlNode nodoDtaStampa = nodoPagina.SelectSingleNode("dta_stampa");
			if (nodoDtaStampa!=null)
			{
				dtaStampa = new StampaVO.DtaStampa(); 
				dtaStampa.align = Utils.getAttribute("align",nodoDtaStampa);
				dtaStampa.font = getFont(nodoDtaStampa);
				dtaStampa.target = Utils.getAttribute("target",nodoDtaStampa);
				dtaStampa.text = nodoDtaStampa.InnerText;
			}
			return dtaStampa;
		}

		private static StampaVO.NumPagine getNumPagine()
		{
			StampaVO.NumPagine numPagina = null;
			XmlNode nodoPagina = nodoReport.SelectSingleNode("pagina");
			XmlNode nodoNumPagina = nodoPagina.SelectSingleNode("num_pagine");
			if (nodoNumPagina!=null)
			{
				numPagina = new StampaVO.NumPagine(); 
				numPagina.align = Utils.getAttribute("align",nodoNumPagina);
				numPagina.font = getFont(nodoNumPagina);
				numPagina.target = Utils.getAttribute("target",nodoNumPagina);
				numPagina.total = Utils.getAttributeI("total",nodoNumPagina);
				numPagina.widthTot = Utils.getAttribute("widthTot",nodoNumPagina);
				numPagina.separator = Utils.getAttribute("separator",nodoNumPagina);
				numPagina.text = nodoNumPagina.InnerText;
			}
			return numPagina;
		}

		private static StampaVO.Paragraph getParagrafo(XmlNode nodo)
		{
			StampaVO.Paragraph paragrafo = null;
			if (nodo!=null)
			{
				paragrafo = new StampaVO.Paragraph();
				paragrafo.align = Utils.getAttribute("align",nodo);
				paragrafo.font = getFont(nodo);
				paragrafo.target = Utils.getAttribute("target",nodo);
				paragrafo.text = nodo.InnerText;
                if(!string.IsNullOrEmpty(Utils.getAttribute("indentationLeft",nodo)))
				    paragrafo.indentationLeft = Convert.ToInt16(Utils.getAttribute("indentationLeft",nodo));
                if(!string.IsNullOrEmpty(Utils.getAttribute("indentationRight",nodo)))
				    paragrafo.indentationRight = Convert.ToInt16(Utils.getAttribute("indentationRight",nodo));
			}
			return paragrafo;
		}
		private static StampaVO.Table getTabella(XmlNode nodoTable)
		{
			StampaVO.Table table = null;
			//XmlNode nodoTable = nodo.SelectSingleNode("table");
			if (nodoTable!=null)
			{
				table = new StampaVO.Table();
				//TODO: 
				/*table.font = getFont(nodoTable);
				
				table.width = Utils.getAttributeI("width",nodoTable);
				table.height = Utils.getAttributeI("height",nodoTable);*/
				table.target = Utils.getAttribute("target",nodoTable);
				table.padding = Utils.getAttributeI("padding",nodoTable);
				table.spacing = Utils.getAttributeI("spacing",nodoTable);
				table.headerTable = getHeaderTable(nodoTable);
				table.dataTable = getDataTable(nodoTable);
				table.columns = getColumns(nodoTable);
			}
			return table;
		}

		private static StampaVO.StyleTable getHeaderTable(XmlNode nodoTable)
		{
			XmlNode hTableNode = nodoTable.SelectSingleNode("header_tabella");
			//FONT E STILE
			StampaVO.StyleTable  style= new StampaVO.StyleTable();
			style.font = getFont(hTableNode);
			style.align = Utils.getAttribute("align",hTableNode);
			style.bgColor = Utils.getAttribute("bgcolor",hTableNode);
			style.vAlign = Utils.getAttribute("vAlign",hTableNode);
			return style;
		}

		private static StampaVO.StyleTable getDataTable(XmlNode nodoTable)
		{
			XmlNode hTableNode = nodoTable.SelectSingleNode("dati_tabella");
			//FONT E STILE
			StampaVO.StyleTable  style= new StampaVO.StyleTable();
			style.font = getFont(hTableNode);
			style.align = Utils.getAttribute("align",hTableNode);
			style.bgColor = Utils.getAttribute("bgcolor",hTableNode);
			style.vAlign = Utils.getAttribute("vAlign",hTableNode);
			return style;
		}

		private static StampaVO.Column[] getColumns(XmlNode nodoTable)
		{
			XmlNode nodoColonne = nodoTable.SelectSingleNode("colonne");
			ArrayList listCol = new ArrayList(); 
			StampaVO.Column[] colonne = null;
			if (nodoColonne.ChildNodes.Count>0)
			{							
				foreach (XmlNode nodo in nodoColonne.ChildNodes)
				{
					StampaVO.Column colonna = new StampaVO.Column();
					colonna.name = Utils.getAttribute("name",nodo);
					colonna.width = Utils.getAttribute("width",nodo);
					colonna.alias = Utils.getAttribute("alias",nodo);
					colonna.bgColor = Utils.getAttribute("bgColor",nodo);
					colonna.align = Utils.getAttribute("align",nodo);
					colonna.vAlign = Utils.getAttribute("valign",nodo);
					colonna.visible = Utils.getAttributeB("visible",nodo);
					//aggiungere il resto
					listCol.Add(colonna);
				}
				colonne = new StampaVO.Column[listCol.Count];
				listCol.CopyTo(colonne);
			}
			return colonne;
		}


		#region old code


		/*	
			private static XmlDocument ValidateXmlDoc(FileStream fileXML)
			{
				errorCode = ErrorCode.NoError;
		
				string pathReportXSD = AppDomain.CurrentDomain.BaseDirectory;
				pathReportXSD+= "Xml/XmlReport.xsd";
				FileStream fileXSD = new FileStream(pathReportXSD,System.IO.FileMode.Open, System.IO.FileAccess.Read);
				try 
				{
					// Validazione file XML
					xmlDoc = new XmlDocument();
					XmlSchemaCollection m_schSchemas = new XmlSchemaCollection(); 
					XmlTextReader xsdReader = new XmlTextReader(fileXSD);
					XmlSchema xmlSchema = XmlSchema.Read(xsdReader,new ValidationEventHandler(ValidationError));
					m_schSchemas.Add(xmlSchema);
 
			
					XmlTextReader xmlReader = new XmlTextReader(fileXML);
					XmlValidatingReader xValidator = new XmlValidatingReader(xmlReader);
					//Assign Schemas to the XmlValidatingReader object
					xValidator.Schemas.Add(m_schSchemas);
					xValidator.ValidationType = ValidationType.Auto; //set the ValidationType
					//Set the Event Handler for ValidationEventHandler
					//These events occur during Read and only if a ValidationType of DTD, XDR, Schema,  
					//or Auto is specified.
					//If no event handler is provided an XmlException is thrown on the first validation error  
					xValidator.ValidationEventHandler += new ValidationEventHandler(ValidationError);

					//Validate Document Node By Node
					while(xValidator.Read()) ; //empty body
					if (!m_bValid)
						throw new Exception("Errori di validazione");
				
					xmlDoc.Load(xValidator);
				
					fileXSD.Close();
					return xmlDoc;

				}
				catch (Exception e)
				{
					//Debugger.Write("Errore durante la validazione dell'XML", e);
					errorCode = ErrorCode.BadXmlFile;
					fileXSD.Close();
					return null;
				}
				return null;
			}

			private static void ValidationError(object sender, ValidationEventArgs arguments)
			{
				Console.Out.WriteLine(arguments.Message);
				m_bValid = false; //validation failed
			}

	*/


		/*   
		public static StampaVO.Document CreateDocTemplate(string xmlPathDoc)
		{
			StampaVO.Document templateDoc = null;

			xmlDoc = GetXmlDoc(xmlPathDoc);
			if (xmlDoc != null)
				templateDoc = LoadXmlDoc(xmlDoc);
			
			return templateDoc;
		}


		private static XmlDocument GetXmlDoc(string xmlPathDocument)
		{
			errorCode = ErrorCode.NoError;
			if (!File.Exists(xmlPathDocument))
			{
				errorCode = ErrorCode.BadXmlFile;
				return null ;
			}

			try
			{
				// Validazione file XML
				xmlDoc = new XmlDocument();
				XmlTextReader xtr=new XmlTextReader(xmlPathDocument);
				xtr.WhitespaceHandling=WhitespaceHandling.None;
				XmlValidatingReader xvr=new XmlValidatingReader(xtr);
				xvr.ValidationType=System.Xml.ValidationType.Schema;
				xvr.EntityHandling=System.Xml.EntityHandling.ExpandCharEntities;
				xmlDoc.Load(xvr);
				return xmlDoc;
			}
			catch(Exception exception)
			{
				//Debugger.Write("Errore durante la validazione dell'XML", exception);
				errorCode = ErrorCode.BadXmlFile;
				return null;
			}
			return null;
		}

  
  */


		#endregion

	}
	
}
