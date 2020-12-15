using System;
using System.Xml;
using System.IO;
using System.Data;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text;
using ICSharpCode.SharpZipLib; 
using System.Configuration;
using System.Collections;
using System.Globalization;


namespace StampaPDF
{
	/// <summary>
	/// Summary description for StampaPDF.
	/// </summary>
	public class StampaPDF
	{
		//per gestione TOT. page
		protected static StampaVO.Document docTmp;
		static PdfContentByte cb;
		static PdfTemplate template;
		static BaseFont bf = null;
		static PdfWriter writer;
		static Font fontPage;


		//Questa classe fa un override dei metodi classici della libreria PDF ed utilizza alcuni degli eventi per la stampa del numero di pagine
		class PDFPageEvents : PdfPageEventHelper 
		{	

			// we override the onOpenDocument method
			public void onOpenDocument(PdfWriter writer, Document document) 
			{
				try 
				{
					if (docTmp.page.numPagine == null)
						return;
					StampaVO.Font font = docTmp.page.numPagine.font;
					fontPage = FontFactory.GetFont(font.name, font.size, Utils.getFontStyle(font.style), Utils.getColor(font.color));
					bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
					cb = writer.DirectContent;
					template = cb.CreateTemplate(100, 20);
				}
				catch (Exception e)
				{
					throw new ReportException(ErrorCode.ErrorEventPDFFile,e.Message);
				}
			}


			// we override the onCloseDocument method
			public void onCloseDocument(PdfWriter writer, Document document) 
			{
				if (docTmp.page.numPagine == null)
					return;
				template.BeginText();
				template.SetFontAndSize(bf, fontPage.Size);
				template.ShowText((writer.PageNumber - 1).ToString());
				template.EndText();
			}

			// we override the onEndPage method
			public void onEndPage(PdfWriter writer, Document document) 
			{
				if(docTmp.page.numPagine==null)
					return;
				int pageN = writer.PageNumber;
				String text = docTmp.page.numPagine.text + pageN;
				if (docTmp.page.numPagine.total>0)
				{
					text += docTmp.page.numPagine.separator;
				}

				float len = bf.GetWidthPoint(text, fontPage.Size);
				float posX = getXPosition (len, docTmp.page.numPagine, document);//document.LeftMargin;
				float posY = document.BottomMargin;
				cb.BeginText();
				cb.SetFontAndSize(bf, fontPage.Size);
				if (docTmp.page.numPagine.target != null && (docTmp.page.numPagine.target.ToUpper().Equals("HEADER")))
					posY = document.Top;

				cb.SetTextMatrix(posX, posY);
				cb.ShowText(text);
				cb.EndText();
				cb.AddTemplate(template, posX + len, posY);

			}


		}

		private static float getXPosition(float len, StampaVO.NumPagine numPagine, Document document)
		{
			float posX=document.Right - len;
			if (numPagine.align !=null && numPagine.align.ToUpper().Equals("CENTER"))
				posX=(document.PageSize.Width/2) - len;
			if (numPagine.widthTot != null && !numPagine.widthTot.Equals("") )
				posX= posX - Int32.Parse(numPagine.widthTot);
			return posX;
		}

		public static DocumentPDF createDocPDF(StampaVO.Document templateDoc, string overrideFooter)
		{
			DocumentPDF docPDF = null;
			if (templateDoc == null)
				throw new ReportException(ErrorCode.InvalidValueObject,"Template di formato non valido");
			try
			{
				try
				{//Creazione del Documento vuoto
					docPDF = new DocumentPDF();
					docTmp = templateDoc;
					writer = PdfWriter.GetInstance(docPDF,docPDF.memoryStream);
					PDFPageEvents events = new PDFPageEvents();
					writer.PageEvent = events;

				}
				catch (Exception e)
				{
					writer.Close();
					throw new ReportException(ErrorCode.BadPDFFile,e.Message);	
				}

				try
				{
					//Settaggio delle proprietà della pagina
					docPDF = setPage(templateDoc, docPDF);
				}
				catch(Exception ex)
				{
					writer.Close();
					throw new ReportException(ErrorCode.IncompletePDFFile,"Fallita impostazione della Pagina: "+ex.Message);
				}

				try
				{
					//Creazione dell'header
					docPDF = setHeader(templateDoc,docPDF);
				}
				catch(Exception ex)
				{
					writer.Close();
					throw new ReportException(ErrorCode.IncompletePDFFile,"Fallita impostazione dell'Header: "+ex.Message);
				}

				try
				{
					//Creazione del footer
					docPDF = setFooter(templateDoc,docPDF,overrideFooter);
				}
				catch(Exception ex)
				{
					writer.Close();
					throw new ReportException(ErrorCode.IncompletePDFFile,"Fallita impostazione del Footer: "+ex.Message);
				}

				try
				{
					if (!docPDF.AddCreationDate())
						throw new Exception();
				}
				catch(Exception ex)
				{
					throw new ReportException(ErrorCode.IncompletePDFFile,"Fallita impostazione della Data di Stampa: "+ex.Message);
				}

				docPDF.Open();

				try
				{
					docPDF = setLogo(templateDoc,docPDF);
				}
				catch(Exception ex)
				{
					docPDF.Close();
					writer.Close();
					throw new ReportException(ErrorCode.IncompletePDFFile,"Fallita impostazione del Logo: "+ex.Message);
				}
			}
			catch (ReportException re)
			{
				docPDF.Close();
				writer.Close();
				docPDF = null;			
				throw re;
			}
			catch (Exception e)
			{
				docPDF.Close();
				writer.Close();
				docPDF = null;			
				throw new ReportException(ErrorCode.GenericError,e.Message);
			}

			return docPDF;
				
		}
		

		private static DocumentPDF setPage(StampaVO.Document templateDoc, DocumentPDF docPDF)
		{//Imposta la dimensione del foglio e i margini
			//PAGE SIZE & ORIENTATION
			bool isRotated = false;
			Rectangle rect = new Rectangle(PageSize.A4);
			switch(templateDoc.page.pageSize) 
			{
				case "A4":
					rect = PageSize.A4;
					break;
				case "A3":
					rect = PageSize.A3;
					break;
				case "_11X17":
					rect = PageSize._11X17;
					break;
				case "LETTER":
					rect = PageSize.LETTER;
					break;
				default: 
					rect = PageSize.A4.Rotate();
					isRotated = true;
					break;
			}

			if (templateDoc.page.orientation.Equals(StampaVO.Page.ORIZZONTALE) && !isRotated)
				rect = rect.Rotate();
			docPDF.SetPageSize(rect);

			//PAGE MARGINS
			if (templateDoc.page.margins!=null)
			{
                docPDF.SetMargins(templateDoc.page.margins[0], templateDoc.page.margins[1], templateDoc.page.margins[2], templateDoc.page.margins[3]);
                /*
				if (!templateDoc.page.margins[0].Equals(0))
					docPDF.Left(templateDoc.page.margins[0]);
				if (!templateDoc.page.margins[1].Equals(0))
					docPDF.Right(templateDoc.page.margins[1]);
				if (!templateDoc.page.margins[2].Equals(0))
					docPDF.Top(templateDoc.page.margins[2]);
				if (!templateDoc.page.margins[3].Equals(0))
					docPDF.Bottom(templateDoc.page.margins[3]);
                */ 
			}
			return docPDF;
		}

		private static DocumentPDF setHeader(StampaVO.Document templateDoc, DocumentPDF docPDF)
		{
			//Creazione dell'header
			//TODO: gestire il testo, il numPagine, la dtaStampa
			
			StampaVO.Font font = templateDoc.page.headerPage.font;
			Font font1 = FontFactory.GetFont(font.name, font.size, Utils.getFontStyle(font.style), Utils.getColor(font.color));
			string str_header = templateDoc.page.headerPage.text;
			if (templateDoc.page.headerPage.dtaStampa!=null)
			{
				if (str_header != null && !str_header.Equals(""))
					str_header += "\n";
				str_header += templateDoc.page.headerPage.dtaStampa.text + DateTime.Parse(DateTime.Now.ToString()).ToShortDateString();
			}
				Phrase p = new Phrase(str_header, font1);
			
			HeaderFooter header = new HeaderFooter(p,false);
			if (templateDoc.page.headerPage.border == null || templateDoc.page.headerPage.border.Equals("0"))
				header.Border = Rectangle.NO_BORDER;
			header.Alignment = Utils.getAlign(templateDoc.page.headerPage.align);
			docPDF.Header = header;
			header.BackgroundColor = Utils.getColor(templateDoc.page.headerPage.bgcolor);
			return docPDF;
		}

		private static DocumentPDF setFooter(StampaVO.Document templateDoc, DocumentPDF docPDF, string overrideFooter)
		{
			//Creazione del footer
			//TODO: gestire numPagine, la dtaStampa ???
			if (templateDoc.page.footerPage == null)
				return docPDF;
			StampaVO.Font font = templateDoc.page.footerPage.font;
			Font font1  = FontFactory.GetFont(font.name, font.size, Utils.getFontStyle(font.style), Utils.getColor(font.color));
			string str_footer = templateDoc.page.footerPage.text;
			if (templateDoc.page.footerPage.dtaStampa!=null)
			{
				if (str_footer != null && !str_footer.Equals(""))
					str_footer += "\n";
                if (string.IsNullOrEmpty(overrideFooter))
                {
                    str_footer += templateDoc.page.footerPage.dtaStampa.text + DateTime.Parse(DateTime.Now.ToString()).ToShortDateString();
                }
                else
                {
                    str_footer += templateDoc.page.footerPage.dtaStampa.text + overrideFooter;                
                }
			}
			Phrase p = new Phrase(str_footer, font1);
			HeaderFooter footer;
            #region OLD GESTIONE NUM PAGE
            //Phrase pPage = null;
            //if (templateDoc.page.footerPage.numPagine != null)
            //{
            //    string str_Page = templateDoc.page.footerPage.numPagine.text;
            //    StampaVO.Font fontPage = templateDoc.page.footerPage.numPagine.font;
            //    Font font2 = FontFactory.GetFont(fontPage.name, fontPage.size, Utils.getFontStyle(fontPage.style), Utils.getColor(fontPage.color));
            //    pPage = new Phrase(str_Page, font2);
            //}

            //if (pPage != null)
            //    footer = new HeaderFooter(pPage, p);  //il true dovrebbe dare il num. di pagina
            //else
            //    footer = new HeaderFooter(p, false);
            #endregion

            footer = new HeaderFooter(p, false);
            if (templateDoc.page.footerPage.border == null || templateDoc.page.footerPage.border.Equals("0"))
                footer.Border = Rectangle.NO_BORDER;
            footer.Alignment = Utils.getAlign(templateDoc.page.footerPage.align);
            docPDF.Footer = footer;
		
			return docPDF;
		}

	
		public static DocumentPDF printDataPage(DocumentPDF docPDF, StampaVO.Document templateDoc, DataTable dt)
		{//Stampa nel PDF gli elementi che contengono dati parametrizzati (Tabelle o Paragrafi)
		 //A seconda della tipologia esegue l'uno o l'altro metodo.
			if (templateDoc!=null && templateDoc.dataToPrint!= null && templateDoc.dataToPrint.Count > 0)
			{
				for (int i=0; i<templateDoc.dataToPrint.Count; i++)
				{
					string target = ((StampaVO.DataElement)templateDoc.dataToPrint[i]).target;
					if ( target == null || target.Equals(""))
					{
						if (templateDoc.dataToPrint[i].GetType().Equals(typeof(StampaVO.Table)))
							printTable((StampaVO.Table)templateDoc.dataToPrint[i], dt, docPDF);
						else
							if (templateDoc.dataToPrint[i].GetType().Equals(typeof(StampaVO.Paragraph)))
							printParagraph((StampaVO.Paragraph)templateDoc.dataToPrint[i], docPDF);
					}
				}
			}
			//else 
			//	docPDF = printNoData(docPDF);
			return docPDF;
		}

		public static DocumentPDF appendData(DataTable dt, DocumentPDF docPDF, StampaVO.Document templateDoc, string target, bool newPage)
		{
			try 
			{
				if (templateDoc != null)
				{
					if (templateDoc.dataToPrint!=null && templateDoc.dataToPrint.Count >0)
					{
						bool trovato = false;
						StampaVO.Table table;
						for(int i=0; (i<templateDoc.dataToPrint.Count && !trovato); i++)
						{
							if (((StampaVO.DataElement)templateDoc.dataToPrint[i]).target.Equals(target))
							{
								table = (StampaVO.Table)templateDoc.dataToPrint[i];
								if (newPage)
									docPDF.NewPage();
                                if (dt.TableName != "REP_TRASMISSIONI")
                                {
                                    docPDF = printTable(table, dt, docPDF);
                                }
                                else
                                {
                                    docPDF = printCustomTable(table, dt, docPDF);
                                }
								trovato = true;
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				docPDF.Close();
				writer.Close();
				throw new ReportException(ErrorCode.IncompletePDFFile,"Errore nella scrittura dei dati: "+ex.Message);
			}
			return docPDF;
		}

        /// <summary>
        /// overloading per passare nome e cognome dell'utente loggato,in modo da evidenziare le trasmissioni di cui l'utente è destinatario
        /// Dimitri
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="docPDF"></param>
        /// <param name="templateDoc"></param>
        /// <param name="target"></param>
        /// <param name="newPage"></param>
        /// <param name="Utente"></param>
        /// <returns></returns>
        public static DocumentPDF appendData(DataTable dt, DocumentPDF docPDF, StampaVO.Document templateDoc, string target, bool newPage,DocsPaVO.utente.InfoUtente infoUt)
        {
            try
            {
                if (templateDoc != null)
                {
                    if (templateDoc.dataToPrint != null && templateDoc.dataToPrint.Count > 0)
                    {
                        bool trovato = false;
                        StampaVO.Table table;
                        for (int i = 0; (i < templateDoc.dataToPrint.Count && !trovato); i++)
                        {
                            if (((StampaVO.DataElement)templateDoc.dataToPrint[i]).target.Equals(target))
                            {
                                table = (StampaVO.Table)templateDoc.dataToPrint[i];
                                if (newPage)
                                    docPDF.NewPage();
                                if (dt.TableName != "REP_TRASMISSIONI")
                                {
                                    docPDF = printTable(table, dt, docPDF);
                                }
                                else
                                {
                                    docPDF = printCustomTable(table, dt, docPDF,infoUt);
                                }
                                trovato = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                docPDF.Close();
                writer.Close();
                throw new ReportException(ErrorCode.IncompletePDFFile, "Errore nella scrittura dei dati: " + ex.Message);
            }
            return docPDF;
        }
		
		public static DocumentPDF appendData(string testo, DocumentPDF docPDF, StampaVO.Document templateDoc, string target, bool newPage)
		{
			try 
			{
				if (templateDoc != null)
				{
					if (templateDoc.dataToPrint!=null && templateDoc.dataToPrint.Count >0)
					{
						bool trovato = false;
						StampaVO.Paragraph paragrafo;
						for(int i=0; (i<templateDoc.dataToPrint.Count && !trovato); i++)
						{
							if (((StampaVO.DataElement)templateDoc.dataToPrint[i]).target.Equals(target))
							{
								paragrafo = (StampaVO.Paragraph)templateDoc.dataToPrint[i];
								if (testo!=null && !testo.Equals(""))
									paragrafo.text = testo;
								if (newPage)
									docPDF.NewPage();
								docPDF = printParagraph(paragrafo,docPDF);
								trovato = true;
							}
						}
					}
			
				}
			}
			catch(Exception ex)
			{
				docPDF.Close();
				writer.Close();
				throw new ReportException(ErrorCode.IncompletePDFFile,"Errore nella scrittura dei dati: "+ex.Message);
			}
			return docPDF;
		}

		public static DocumentPDF appendData(Hashtable parameters, DocumentPDF docPDF, StampaVO.Document templateDoc, string target, bool newPage)
		{
			try 
			{
				if (templateDoc != null)
				{
					if (templateDoc.dataToPrint!=null && templateDoc.dataToPrint.Count >0)
					{
						bool trovato = false;
						StampaVO.Paragraph paragrafo;
						for(int i=0; (i<templateDoc.dataToPrint.Count && !trovato); i++)
						{
							if (target == null || ((StampaVO.DataElement)templateDoc.dataToPrint[i]).target.Equals(target))
							{
								if (target == null)
									trovato = true;

								paragrafo = (StampaVO.Paragraph)templateDoc.dataToPrint[i];
								//sostituzione parametri
								if (!paragrafo.text.Equals(""))
								{//Sostituisce i parametri
									foreach( string key in parameters.Keys)
									{
										string pval = parameters[key].ToString();
										if (pval==null) 
											pval="";
										paragrafo.text = paragrafo.text.Replace(key ,pval);
									}
								}
								if (newPage)
									docPDF.NewPage();
								docPDF = printParagraph(paragrafo,docPDF);
								trovato = true;
							}
						}
					}
			
				}
			}
			catch(Exception ex)
			{
				docPDF.Close();
				writer.Close();
				throw new ReportException(ErrorCode.IncompletePDFFile,"Errore nella scrittura dei dati: "+ex.Message);
			}
			return docPDF;
		}

		private static DocumentPDF printNoData(DocumentPDF docPDF)
		{
			//TODO:
			return null;
		}


        protected static DocumentPDF printCustomTable(StampaVO.Table tableTmp, DataTable dt, DocumentPDF docPDF)
        {
            if (dt == null)
                return docPDF;

            //** Operazioni Preliminari
            //reupero del numero di colonne dal DataTable  
            int col = tableTmp.columns.Length;
            int col_visible = 0;
            for (int j = 0; j < tableTmp.columns.Length; j++)
            {
                if (tableTmp.columns[j].visible)
                    col_visible++;
            }
            try
            {

                //creazione della tabella
                iTextSharp.text.Table aTable = new iTextSharp.text.Table(col_visible);

                //Adattamento delle colonne al contenuto
                aTable.Padding = tableTmp.padding;
                aTable.Spacing = tableTmp.spacing;
                //aTable.WidthPercentage = 100;
                aTable.Width = 100;
                aTable.Alignment = Utils.getAlign(tableTmp.align);
                int[] widths = getColWidths(tableTmp, col_visible);

                aTable.SetWidths(widths);
                //aTable.hasToFitPageCells();
                aTable.TableFitsPage = true;

                //** Aggiunta automatica dell'header della tabella
                for (int k = 0; k < col; k++)
                {
                    if (((StampaVO.Column)tableTmp.columns[k]).visible)
                    {
                        StampaVO.Font font = tableTmp.headerTable.font;
                        Font font1 = FontFactory.GetFont(font.name, font.size, Utils.getFontStyle(font.style), Utils.getColor(font.color));
                        string testo = ((StampaVO.Column)tableTmp.columns[k]).alias;
                        string[] testoSplit = testo.Split(';');
                        string testo_1 = string.Empty;
                        if (testoSplit.Length > 1)
                            testo_1 = @testoSplit[0] + "\n" + testoSplit[1];
                        else
                            testo_1 = testoSplit[0];
                        Cell c = new Cell(new Phrase(testo_1, font1));
                        if (((StampaVO.Column)tableTmp.columns[k]).name == "DESCR" || ((StampaVO.Column)tableTmp.columns[k]).name == "MITT_UT" || ((StampaVO.Column)tableTmp.columns[k]).name == "DEST" || ((StampaVO.Column)tableTmp.columns[k]).name == "NOTE_GENER")
                            c.HorizontalAlignment = Utils.getAlign("LEFT");
                        else
                            c.HorizontalAlignment = Utils.getAlign(tableTmp.headerTable.align);
                        c.VerticalAlignment = Utils.getAlign(tableTmp.headerTable.vAlign);
                        c.NoWrap = true;
                        c.BackgroundColor = Utils.getColor(tableTmp.headerTable.bgColor);
                        aTable.AddCell(c);
                    }
                }
                aTable.EndHeaders();

                //** Popolamento automatico della tabella
                //Scansione dei dati
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Creazione delle celle
                    for (int h = 0; h < col; h++)
                    {

                        if (((StampaVO.Column)tableTmp.columns[h]).visible)
                        {

                            StampaVO.Font font = tableTmp.dataTable.font;
                            string style = font.style;

                            Font font1 = FontFactory.GetFont(font.name, font.size, Utils.getFontStyle(style), Utils.getColor(font.color));
                            // Font font1 = FontFactory.GetFont(font.name, font.size, Utils.getFontStyle(font.style), Utils.getColor(font.color));
                            string column_name = tableTmp.columns[h].name;
                            Cell c1;

                            if (column_name == "ID_REG_PROTO_ANNO")
                            {
                                string s = string.Empty;
                                if (dt.Rows[i]["COD_REG"].ToString() != "")
                                    s = @dt.Rows[i]["ID"].ToString() + "\n" + dt.Rows[i]["COD_REG"].ToString() + " - " + dt.Rows[i]["NUM_PROTO"].ToString() + " - " + dt.Rows[i]["ANNO"].ToString();
                                else
                                    s = dt.Rows[i]["ID"].ToString() + "\n Non Protocollato";
                                c1 = new Cell(new Phrase(s, font1));
                                c1.HorizontalAlignment = Utils.getAlign(tableTmp.columns[h].align);
                                c1.VerticalAlignment = Utils.getAlign(tableTmp.columns[h].vAlign);
                                aTable.AddCell(c1, new System.Drawing.Point(i + 1, h));
                            }
                            if (column_name == "MITT_UT")
                            {
                                string s = @dt.Rows[i][column_name].ToString();
                                c1 = new Cell(new Phrase(s, font1));
                                c1.HorizontalAlignment = Utils.getAlign(tableTmp.columns[h].align);
                                c1.VerticalAlignment = Utils.getAlign(tableTmp.columns[h].vAlign);
                                aTable.AddCell(c1, new System.Drawing.Point(i + 1, h));
                            }
                            if (column_name == "MITT_RU")
                            {
                                string s = dt.Rows[i]["MITT_RU"].ToString();
                                c1 = new Cell(new Phrase(s, font1));
                                c1.HorizontalAlignment = Utils.getAlign(tableTmp.columns[h].align);
                                c1.VerticalAlignment = Utils.getAlign(tableTmp.columns[h].vAlign);
                                aTable.AddCell(c1, new System.Drawing.Point(i + 1, h));
                            }

                            //Aggiunta note individuali alle generali
                            if (column_name == "NOTE_GENER")
                            {
                                if (dt.Columns.Contains("NOTE_INDIVID") && dt.Rows[i]["NOTE_INDIVID"] != null)
                                {
                                    string s = @dt.Rows[i]["NOTE_GENER"].ToString() + Environment.NewLine + "--------------" + Environment.NewLine + dt.Rows[i]["NOTE_INDIVID"].ToString();
                                    c1 = new Cell(new Phrase(s, font1));
                                    c1.HorizontalAlignment = Utils.getAlign(tableTmp.columns[h].align);
                                    c1.VerticalAlignment = Utils.getAlign(tableTmp.columns[h].vAlign);
                                    aTable.AddCell(c1, new System.Drawing.Point(i + 1, h));
                                }

                            }

                            if (column_name != "ID_REG_PROTO_ANNO" && column_name != "MITT_UT" && column_name != "MITT_RU" && column_name != "NUM_PROTO" && column_name != "ANNO" && column_name != "NOTE_GENER")
                            {
                                c1 = new Cell(new Phrase(dt.Rows[i][column_name].ToString(), font1));
                                c1.HorizontalAlignment = Utils.getAlign(tableTmp.columns[h].align);
                                c1.VerticalAlignment = Utils.getAlign(tableTmp.columns[h].vAlign);
                                aTable.AddCell(c1, new System.Drawing.Point(i + 1, h));
                            }

                        }
                    }
                }

                //     aTable.Complete();
                //     aTable.FlushContent();

                docPDF.Add(aTable);
            }

            catch (Exception ex)
            {
                docPDF.Close();
                writer.Close();
                throw new ReportException(ErrorCode.IncompletePDFFile, "Errore nella scrittura dei dati: " + ex.Message);
            }
            return docPDF;
        }
       
       /// <summary>
        /// overloading per passare nome e cognome dell'utente loggato,in modo da evidenziare le trasmissioni di cui l'utente è destinatario
        /// Dimitri
       /// </summary>
       /// <param name="tableTmp"></param>
       /// <param name="dt"></param>
       /// <param name="docPDF"></param>
        /// <param name="infoUt"></param>
       /// <returns></returns>
        protected static DocumentPDF printCustomTable(StampaVO.Table tableTmp, DataTable dt, DocumentPDF docPDF,DocsPaVO.utente.InfoUtente infoUt)
        {
            if (dt == null)
                return docPDF;

            //** Operazioni Preliminari
            //reupero del numero di colonne dal DataTable  
            int col = tableTmp.columns.Length;
            int col_visible = 0;
            for (int j = 0; j < tableTmp.columns.Length; j++)
            {
                if (tableTmp.columns[j].visible)
                    col_visible++;
            }
            try
            {

                //creazione della tabella
                iTextSharp.text.Table aTable = new iTextSharp.text.Table(col_visible);

                //Adattamento delle colonne al contenuto
                aTable.Padding = tableTmp.padding;
                aTable.Spacing = tableTmp.spacing;
                //aTable.WidthPercentage = 100;
                aTable.Width = 100;
                aTable.Alignment = Utils.getAlign(tableTmp.align);
                int[] widths = getColWidths(tableTmp, col_visible);

                aTable.SetWidths(widths);
                //aTable.hasToFitPageCells();
                aTable.TableFitsPage = true;
                
                //** Aggiunta automatica dell'header della tabella
                for (int k = 0; k < col; k++)
                {
                    if (((StampaVO.Column)tableTmp.columns[k]).visible)
                    {
                        StampaVO.Font font = tableTmp.headerTable.font;
                        Font font1 = FontFactory.GetFont(font.name, font.size, Utils.getFontStyle(font.style), Utils.getColor(font.color));
                        string testo = ((StampaVO.Column)tableTmp.columns[k]).alias;
                        string[] testoSplit = testo.Split(';');
                        string testo_1 = string.Empty;
                        if (testoSplit.Length > 1)
                            testo_1 = @testoSplit[0] + "\n" + testoSplit[1];
                        else
                            testo_1 = testoSplit[0];
                        Cell c = new Cell(new Phrase(testo_1, font1));
                        if (((StampaVO.Column)tableTmp.columns[k]).name == "DESCR" || ((StampaVO.Column)tableTmp.columns[k]).name == "MITT_UT" || ((StampaVO.Column)tableTmp.columns[k]).name == "DEST" || ((StampaVO.Column)tableTmp.columns[k]).name == "NOTE_GENER")
                            c.HorizontalAlignment = Utils.getAlign("LEFT");
                        else
                            c.HorizontalAlignment = Utils.getAlign(tableTmp.headerTable.align);
                        c.VerticalAlignment = Utils.getAlign(tableTmp.headerTable.vAlign);
                        c.NoWrap = true;
                        c.BackgroundColor = Utils.getColor(tableTmp.headerTable.bgColor);
                        aTable.AddCell(c);
                    }
                }
                aTable.EndHeaders();

                //** Popolamento automatico della tabella
                //Scansione dei dati
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Creazione delle celle
                    for (int h = 0; h < col; h++)
                    {

                        if (((StampaVO.Column)tableTmp.columns[h]).visible)
                        {
                            
                            StampaVO.Font font = tableTmp.dataTable.font;
                            string style = font.style;
                            string column_name = tableTmp.columns[h].name;
                            string evidenziaDest = "";
                            if (dt.Rows[i]["SYSTEM_ID_DEST_UT"].ToString() == infoUt.idPeople && column_name == "DEST_UT")
                            {
                                evidenziaDest = "  *";
                                style = "BOLD";
                            }
                           
                            Font font1 = FontFactory.GetFont(font.name, font.size, Utils.getFontStyle(style), Utils.getColor(font.color));
                           // Font font1 = FontFactory.GetFont(font.name, font.size, Utils.getFontStyle(font.style), Utils.getColor(font.color));
                            
                            Cell c1;
                            
                            if (column_name == "ID_REG_PROTO_ANNO")
                            {
                                string s = string.Empty;
                                if (dt.Rows[i]["COD_REG"].ToString() != "")
                                    s = @dt.Rows[i]["ID"].ToString() + "\n" + dt.Rows[i]["COD_REG"].ToString() + " - " + dt.Rows[i]["NUM_PROTO"].ToString() + " - " + dt.Rows[i]["ANNO"].ToString();
                                else
                                    s = dt.Rows[i]["ID"].ToString() + "\n Non Protocollato";
                                c1 = new Cell(new Phrase(s, font1));
                                c1.HorizontalAlignment = Utils.getAlign(tableTmp.columns[h].align);
                                c1.VerticalAlignment = Utils.getAlign(tableTmp.columns[h].vAlign);
                                aTable.AddCell(c1, new System.Drawing.Point(i + 1, h));
                            }
                            if (column_name == "MITT_UT")
                            {
                                string s = @dt.Rows[i][column_name].ToString();
                                c1 = new Cell(new Phrase(s, font1));
                                c1.HorizontalAlignment = Utils.getAlign(tableTmp.columns[h].align);
                                c1.VerticalAlignment = Utils.getAlign(tableTmp.columns[h].vAlign);
                                aTable.AddCell(c1, new System.Drawing.Point(i + 1, h));
                            }
                            if (column_name == "MITT_RU")
                            {
                                string s = dt.Rows[i]["MITT_RU"].ToString();
                                c1 = new Cell(new Phrase(s, font1));
                                c1.HorizontalAlignment = Utils.getAlign(tableTmp.columns[h].align);
                                c1.VerticalAlignment = Utils.getAlign(tableTmp.columns[h].vAlign);
                                aTable.AddCell(c1, new System.Drawing.Point(i + 1, h));
                            }

                            //Aggiunta note individuali alle generali
                            if (column_name == "NOTE_GENER")
                            {
                                if (dt.Rows[i]["NOTE_INDIVID"] != null)
                                {
                                    string s = @dt.Rows[i]["NOTE_GENER"].ToString() + Environment.NewLine + "--------------" + Environment.NewLine;
                                    if (dt.Rows[i]["SYSTEM_ID_MITT_UT"].ToString() == infoUt.idPeople)
                                        s += dt.Rows[i]["NOTE_INDIVID"].ToString();
                                    c1 = new Cell(new Phrase(s, font1));
                                    c1.HorizontalAlignment = Utils.getAlign(tableTmp.columns[h].align);
                                    c1.VerticalAlignment = Utils.getAlign(tableTmp.columns[h].vAlign);
                                    aTable.AddCell(c1, new System.Drawing.Point(i + 1, h));
                                }

                            }

                            if (column_name == "DEST_UT")
                            {
                                if (dt.Rows[i]["DEST_UT"] != null)
                                {
                                    string s = @dt.Rows[i]["DEST_UT"].ToString() + evidenziaDest;
                                    c1 = new Cell(new Phrase(s, font1));
                                    c1.HorizontalAlignment = Utils.getAlign(tableTmp.columns[h].align);
                                    c1.VerticalAlignment = Utils.getAlign(tableTmp.columns[h].vAlign);
                                    aTable.AddCell(c1, new System.Drawing.Point(i + 1, h));
                                }

                            }

                            if (column_name != "ID_REG_PROTO_ANNO" && column_name != "MITT_UT" && column_name != "MITT_RU" && column_name != "NUM_PROTO" && column_name != "ANNO" && column_name != "NOTE_GENER" && column_name != "DEST_UT")
                            {
                                c1 = new Cell(new Phrase(dt.Rows[i][column_name].ToString(), font1));
                                c1.HorizontalAlignment = Utils.getAlign(tableTmp.columns[h].align);
                                c1.VerticalAlignment = Utils.getAlign(tableTmp.columns[h].vAlign);
                                aTable.AddCell(c1, new System.Drawing.Point(i + 1, h));
                            }

                        }
                    }
                }
                
           //     aTable.Complete();
           //     aTable.FlushContent();

                docPDF.Add(aTable);
            }
          
            catch (Exception ex)
            {
                docPDF.Close();
                writer.Close();
                throw new ReportException(ErrorCode.IncompletePDFFile, "Errore nella scrittura dei dati: " + ex.Message);
            }
            return docPDF;
        }

        
		protected static DocumentPDF printTable(StampaVO.Table tableTmp, DataTable dt, DocumentPDF docPDF)
		{
			if (dt == null)
				return docPDF;
			
			//** Operazioni Preliminari
			//reupero del numero di colonne dal DataTable  
			int col = tableTmp.columns.Length;
			int col_visible = 0;
			for (int j=0; j < tableTmp.columns.Length; j++)
			{
				if (tableTmp.columns[j].visible)
					col_visible++;
            }

            try 
			{
				//creazione della tabella
				iTextSharp.text.Table aTable = new iTextSharp.text.Table(col_visible);
                
				//Adattamento delle colonne al contenuto
				aTable.Padding = tableTmp.padding;
				aTable.Spacing = tableTmp.spacing;
				aTable.Width = 100;
                aTable.Alignment = Utils.getAlign(tableTmp.align);
				int[] widths = getColWidths(tableTmp,col_visible);

				aTable.SetWidths(widths);
                aTable.TableFitsPage = true;
				
				//** Aggiunta automatica dell'header della tabella
				for(int k = 0;k < col;k++)
				{
					if (((StampaVO.Column)tableTmp.columns[k]).visible)
					{
						StampaVO.Font font = tableTmp.headerTable.font;
						Font font1  = FontFactory.GetFont(font.name, font.size, Utils.getFontStyle(font.style), Utils.getColor(font.color));
						string testo = ((StampaVO.Column)tableTmp.columns[k]).alias;
						Cell c = new Cell(new Phrase(testo,font1));
						c.HorizontalAlignment = Utils.getAlign(tableTmp.headerTable.align);
						c.VerticalAlignment = Utils.getAlign(tableTmp.headerTable.vAlign);
						//c.NoWrap=true;
						c.BackgroundColor = Utils.getColor(tableTmp.headerTable.bgColor);
						aTable.AddCell(c);
					}
				}

				aTable.EndHeaders();

				//** Popolamento automatico della tabella
				//Scansione dei dati
				for(int i = 0;i<dt.Rows.Count;i++)
				{
					//Creazione delle celle
					for(int h = 0;h < col;h++)
					{
						if (((StampaVO.Column)tableTmp.columns[h]).visible)
						{
							StampaVO.Font font = tableTmp.dataTable.font;
							Font font1  = FontFactory.GetFont(font.name, font.size, Utils.getFontStyle(font.style), Utils.getColor(font.color));
							string column_name = tableTmp.columns[h].name;
							Cell c1 = new Cell(new Phrase(dt.Rows[i][column_name].ToString(),font1));
							c1.HorizontalAlignment = Utils.getAlign(tableTmp.columns[h].align);
							c1.VerticalAlignment = Utils.getAlign(tableTmp.columns[h].vAlign);
                            if (!string.IsNullOrEmpty(tableTmp.columns[h].bgColor))
                                c1.BackgroundColor = Utils.getColor(tableTmp.columns[h].bgColor);
							aTable.AddCell(c1,new System.Drawing.Point(i+1,h));

						}			
					}
				}

				docPDF.Add(aTable);
			}
			catch(Exception ex)
            {
				docPDF.Close();
				writer.Close();
				throw new ReportException(ErrorCode.IncompletePDFFile,"Errore nella scrittura dei dati: "+ex.Message);
			}
			return docPDF;
		}
    

		private static DocumentPDF printParagraph(StampaVO.Paragraph paragraph, DocumentPDF docPDF)
		{
			try 
			{
				string[] testo; 
				testo = paragraph.text.Split('&');
				StampaVO.Font font = paragraph.font;
				Font font1  = FontFactory.GetFont(font.name, font.size, Utils.getFontStyle(font.style), Utils.getColor(font.color));
				for (int i = 0; i < testo.Length; i++)
				{
					Paragraph p = new Paragraph(new Phrase(testo[i],font1));
                    p.IndentationLeft = paragraph.indentationLeft;
                    p.IndentationRight = paragraph.indentationRight;
                    p.Alignment = Utils.getAlign(paragraph.align);				
					docPDF.Add(p);
				}
			}
			catch(Exception ex)
			{
				docPDF.Close();
				writer.Close();
				throw new ReportException(ErrorCode.IncompletePDFFile,"Errore nella scrittura dei dati: "+ex.Message);
			}
			return docPDF;
		}

		private static DocumentPDF setLogo(StampaVO.Document templateDoc, DocumentPDF docPDF)
		{
			if (templateDoc.page.logo!= null && templateDoc.page.logo.fileImage != null && !templateDoc.page.logo.fileImage.Equals(""))
			{
				//NB: attenzione ai file in formato gif
                string[] pathProspettiRiepilogativi = templateDoc.pathName.Split(new string[]{"ProspettiRiepilogativi"},StringSplitOptions.None);
                if (pathProspettiRiepilogativi.Length > 0)
                {
                    string fileLogo = pathProspettiRiepilogativi[0] + "\\ProspettiRiepilogativi\\Frontend\\images\\" + templateDoc.page.logo.fileImage;
                    Image logo = Image.GetInstance(fileLogo);
                    logo.Alignment = Utils.getImageAlign(templateDoc.page.logo.align);
                    logo.Alt = "logo";
                    logo.BackgroundColor = Utils.getColor(templateDoc.page.logo.bgColor);
                    docPDF.Add(logo);
                }
			}
			return docPDF;
		}

	
		private static int[] getColWidths(StampaVO.Table table,int col_visible)
		{		
			int[] widths = new int[col_visible];
			for (int i=0; i < table.columns.Length; i++)
			{
				if (table.columns[i].visible)
				{
					string width = table.columns[i].width;
					if (width==null || width.Equals(""))
						width = StampaVO.Column.COLUMN_WIDTH;
					widths[i]= Int32.Parse(width);
				}
			}
			
			return widths;
		}



        /// <summary>
        /// Unisce gli stream pdf contenuti nella lista passata in un unico stream pdf
        /// </summary>
        /// <param name="files">lista stream pdf</param>
        /// <returns></returns>
        public static byte[] MergePDFs(System.Collections.Generic.List<byte[]> files)
        {
            MemoryStream ms = null;
            byte[] result = null;
            ArrayList master = null;

            ms = new MemoryStream();
            master = new ArrayList();
            int f = 0;
            Document document = null;
            PdfCopy writer = null;
            while (f < files.Count)
            {
                PdfReader reader = new PdfReader(files[f]);
                reader.ConsolidateNamedDestinations();
                int n = reader.NumberOfPages;
                if (f == 0)
                {
                    document = new Document(reader.GetPageSizeWithRotation(1));
                    writer = new PdfCopy(document, ms);
                    document.Open();
                }
                for (int i = 0; i < n; )
                {
                    ++i;
                    if (writer != null)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        writer.AddPage(page);
                    }
                }
                PRAcroForm form = reader.AcroForm;
                if (form != null && writer != null)
                {
                    writer.CopyAcroForm(reader);
                }
                f++;
            }
            if (document != null) document.Close();
            result = ms.ToArray();
            ms.Close();
            return result;
        }


	
	}
}
