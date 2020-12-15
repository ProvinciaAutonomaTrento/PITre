using System;
using System.IO;
using System.Data;
//using Debugger = LogsManagement.Debugger;
using System.Configuration;
using System.Collections;
using System.Globalization;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text;
using ICSharpCode.SharpZipLib; 

namespace StampaPDF
{
	/// <summary>
	/// Summary description for Report.
	/// </summary>
	public class Report
	{
		DocumentPDF docPDF = null;
		StampaVO.Document docTemplate = null;

        public Report(System.IO.FileStream fileXML, string path) : this(fileXML,path,null,null)
        {

        }

        public Report(System.IO.FileStream fileXML, string path, string titolo) : this(fileXML, path, titolo, null)
        {

        }

		public Report(System.IO.FileStream fileXML, string path,string titolo, string overrideFooter) 
		{
			try
			{//Esegue il parsing del file XML e riempie un apposito ValueObject
				docTemplate = StampaXML.CreateDocTemplate(fileXML, path);
                if(!string.IsNullOrEmpty(titolo)) docTemplate.page.headerPage.text = titolo;
			}
			catch (ReportException re)
			{//Si è verificato un errore nel parsing del file XML
				//Il dettaglio è contenuto nel messaggio che viene propagato.
				throw re;
			}
			catch (Exception e)
			{//Si è verificato un errore nel parsing del file XML
			 //Il dettaglio è contenuto nel messaggio che viene propagato.
				throw new ReportException(ErrorCode.XmlFileNotFound, "Errore durante l'esame del file XML: " + e.Message);
			}
			try
			{
				docPDF = StampaPDF.createDocPDF(docTemplate,overrideFooter);
			}
			catch (ReportException re)
			{//Si è verificato un errore nella creazione del file PDF
				//Il dettaglio è contenuto nel messaggio che viene propagato.
				throw re;
			}
			catch (Exception e)
			{//Si è verificato un errore nella creazione del file PDF
			 //Il dettaglio è contenuto nel messaggio che viene propagato.
				throw new ReportException(ErrorCode.BadPDFFile, "Errore durante la creazione del file PDF: " + e.Message);
			}
	}


		public void appendTable(String target, DataTable dt, bool newPage)
		{ //Appende la tabella individuata dal target al file PDF.
		  //La tabella sarà formattata usando la specifica contenuta nel file XML e 
		  //popolata con i dati contenuti nel DataTable.
		  //La tabella viene stampata in una nuova pagina se newPage è true
			try
			{
				if (dt == null)
					return;
				if (docPDF==null)
					throw new ReportException(ErrorCode.NullPDFFile, "Impossibile aggiungere informazioni sul file PDF. Il file non esiste");
				if (!docPDF.IsOpen())
					docPDF.Open();
				docPDF = StampaPDF.appendData(dt, docPDF, docTemplate, target, newPage);
			}
			catch (ReportException re)
			{
				throw re;
			}
			catch (Exception e)
			{
				throw new ReportException(ErrorCode.BadPDFFile,"Impossibile aggiungere informazioni sul file PDF. "+e.Message);
			}
		}


        /// <summary>
        /// overloading per passare nome e cognome dell'utente loggato,in modo da evidenziare le trasmissioni di cui l'utente è destinatario
        /// Dimitri
        /// </summary>
        /// <param name="target"></param>
        /// <param name="dt"></param>
        /// <param name="newPage"></param>
        public void appendTable(String target, DataTable dt, bool newPage,DocsPaVO.utente.InfoUtente infoUt)
        { //Appende la tabella individuata dal target al file PDF.
            //La tabella sarà formattata usando la specifica contenuta nel file XML e 
            //popolata con i dati contenuti nel DataTable.
            //La tabella viene stampata in una nuova pagina se newPage è true
            try
            {
                if (dt == null)
                    return;
                if (docPDF == null)
                    throw new ReportException(ErrorCode.NullPDFFile, "Impossibile aggiungere informazioni sul file PDF. Il file non esiste");
                if (!docPDF.IsOpen())
                    docPDF.Open();
                docPDF = StampaPDF.appendData(dt, docPDF, docTemplate, target, newPage, infoUt);
            }
            catch (ReportException re)
            {
                throw re;
            }
            catch (Exception e)
            {
                throw new ReportException(ErrorCode.BadPDFFile, "Impossibile aggiungere informazioni sul file PDF. " + e.Message);
            }
        }

		public void appendParagraph(string target, string testo, bool newPage)
		{ //Sostituisce in testo specificato a quello contenuto nel paragrafo target del file XML
		  //Appende il paragrafo individuato dal target al file PDF.
		  //Il paragrafo viene stampato in una nuova pagina se newPage è true.
		  //Il paragrafo sarà formattato usando la specifica contenuta nel file XML; 
	 	  //i parametri indicati saranno sostituiti con il contenuto dell'array parameters.			
			try
			{
				if (docPDF==null)
					throw new ReportException(ErrorCode.NullPDFFile, "Impossibile aggiungere informazioni sul file PDF. Il file non esiste");
				if (!docPDF.IsOpen())
					docPDF.Open();
                docPDF = StampaPDF.appendData(testo, docPDF, docTemplate, target, newPage);
			}
			catch (ReportException re)
			{
				throw re;
			}
			catch (Exception e)
			{
				throw new ReportException(ErrorCode.BadPDFFile,"Impossibile aggiungere informazioni sul file PDF. "+e.Message);
			}
		}

		public void appendParagraph(String target, Hashtable parameters, bool newPage)
		{ //Appende il paragrafo individuato dal target al file PDF.
		  //Il paragrafo viene stampato in una nuova pagina se newPage è true.
		  //Il paragrafo sarà formattato usando la specifica contenuta nel file XML; 
	      //i parametri indicati saranno sostituiti con il contenuto dell'array parameters.
			try
			{
				if (docPDF==null)
					throw new ReportException(ErrorCode.NullPDFFile, "Impossibile aggiungere informazioni sul file PDF. Il file non esiste");
				if (!docPDF.IsOpen())
					docPDF.Open();
				docPDF = StampaPDF.appendData(parameters, docPDF, docTemplate, target, newPage);
			}
			catch (ReportException re)
			{
				throw re;
			}
			catch (Exception e)
			{
				throw new ReportException(ErrorCode.BadPDFFile,"Impossibile aggiungere informazioni sul file PDF. "+e.Message);
			}
		}



		public void replace(String target, Hashtable parameters)
		{//L'effetto di questo metodo è quello di sostituire, nel paragrafo individuato dal nome (target) specificato, 
		 //ogni parametro con il rispettivo valore fornito dal richiedente.
		 //La tabella parameters contiene le coppie <parametro(chiave), valore>
		 //Se il target è null, le chiavi saranno sostituite in tutto il template costituito dal file XML.
			try
			{
				if (parameters==null || parameters.Keys.Count==0)
					return;

				if (docTemplate != null)
				{
					if (docTemplate.dataToPrint!=null && docTemplate.dataToPrint.Count >0)
					{
						bool trovato = false;
						StampaVO.Paragraph paragrafo;
						for(int i=0; i<docTemplate.dataToPrint.Count && docTemplate.dataToPrint[i].GetType().Equals(typeof(StampaVO.Paragraph)); i++)
						{//Scorre tutti Data Element di tipo Paragrafo nel template...
							if (target == null || ((StampaVO.DataElement)docTemplate.dataToPrint[i]).target.Equals(target) )
							{//...Se il target non è impostato, l'operazione viene effettuata su tutti i Paragrafi;
								//Se il target coincide con uno dei Paragrafi, l'operazione verrà compiuta solamente sul Paragrafo indicato.
								if (target == null) 
									trovato = true;
								//Acquisisce il testo del Paragrafo
								paragrafo = (StampaVO.Paragraph)docTemplate.dataToPrint[i];
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
							}
						}
						if (!trovato)
							throw new ReportException(ErrorCode.InvalidXmlFile, "Impossibile completare l'aggiornamento dei dati: target non presente.");
					}
			
				}
				else 
					throw new ReportException(ErrorCode.InvalidXmlFile, "Impossibile completare l'aggiornamento dei dati: file XML vuoto.");
			}
			catch (ReportException re)
			{
				throw re;
			}
			catch (Exception e)
			{
			 throw e;}
		}



		public void printData(DataTable dt)
		{//Stampa il template standard (target={null, "default"}) 
		 //Esso è costituito da una sola tabella e nessun paragrafo parametrizzato.
			try
			{
				if (docPDF==null)
					throw new ReportException(ErrorCode.NullPDFFile, "Impossibile aggiungere informazioni sul file PDF. Il file non esiste");
				if (!docPDF.IsOpen())
					docPDF.Open();
				docPDF = StampaPDF.printDataPage(docPDF, docTemplate, dt);
			}
			catch (ReportException re)
			{
				throw re;	
			}
			catch (Exception e)
			{
				throw new ReportException(ErrorCode.BadPDFFile, "Impossibile completare la stampa del template standard: "+e.Message);	
			}
		}
		
		public MemoryStream getStream()
		{
			if (docPDF != null)
				return docPDF.memoryStream;
			return null;
		}
		
		public MemoryStream close()
		{
			if (docPDF==null)
				throw new ReportException(ErrorCode.BadPDFFile, "Impossibile chiudere il file. Il file non è aperto");
			try
			{
				if (docPDF.IsOpen())
					docPDF.Close();
			}
			catch (Exception e)
			{

			//Si è verificato un errore nella creazione del file PDF
			 //Il dettaglio è contenuto nel messaggio che viene propagato.
				throw new ReportException(ErrorCode.BadPDFFile, e.Message);

			}
			return docPDF.memoryStream;
			
		}


	}
}
